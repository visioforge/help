using System.Diagnostics;
using ObjCRuntime;
using UniformTypeIdentifiers;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Apple;

namespace SimpleMediaPlayerMBMac;

/// <summary>
/// The view controller.
/// </summary>
public partial class ViewController : NSViewController
{
    /// <summary>
    /// The media player instance.
    /// </summary>
    private MediaPlayerCoreX _player;

    /// <summary>
    /// The position-update timer.
    /// </summary>
    private Timer _timer;

    /// <summary>
    /// True while the timer is pushing a value into the slider — guards the slider's
    /// "value changed" handler from feeding the value back into Position_SetAsync.
    /// </summary>
    private bool _timerFlag;

    /// <summary>
    /// Indicates that a seek request is currently in flight.
    /// </summary>
    private bool _seekInFlight;

    /// <summary>
    /// The video view.
    /// </summary>
    private VideoView _videoView;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewController"/> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    protected ViewController(NativeHandle handle) : base(handle)
    {
        // This constructor is required if the view controller is loaded from a xib or a storyboard.
        // Do not put any initialization here, use ViewDidLoad instead.
    }

    /// <summary>
    /// Gets or sets the represented object.
    /// </summary>
    public override NSObject RepresentedObject
    {
        get => base.RepresentedObject;
        set => base.RepresentedObject = value;
    }

    /// <summary>
    /// View did load.
    /// </summary>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        _videoView = new VideoView(new CGRect(0, 0, videoViewHost.Bounds.Width, videoViewHost.Bounds.Height));
        videoViewHost.AddSubview(_videoView);

        // Mandatory engine boot for Media Player X on macOS — capture's auto-init pattern
        // does NOT apply here; calling MediaPlayerCoreX without this throws on first frame.
        VisioForgeX.InitSDK();

        edFilename.StringValue = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "video.mp4");

        _timer = new Timer(OnTimer);
    }

    /// <summary>
    /// Position-update timer tick — reads current position + duration and updates the slider.
    /// </summary>
    public async void OnTimer(object stateInfo)
    {
        var player = _player;
        if (player == null) return;

        _timerFlag = true;
        try
        {
            var position = await player.Position_GetAsync();
            var duration = await player.DurationAsync();

            if (_player == null) return;

            InvokeOnMainThread(() =>
            {
                slPosition.MaxValue = duration.TotalSeconds;

                if (slPosition.MaxValue >= position.TotalSeconds)
                {
                    slPosition.DoubleValue = position.TotalSeconds;
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            _timerFlag = false;
        }
    }

    /// <summary>
    /// Show a modal informational alert.
    /// </summary>
    private void ShowMessage(string text)
    {
        var alert = new NSAlert
        {
            AlertStyle = NSAlertStyle.Informational,
            InformativeText = text,
            MessageText = "Message"
        };
        alert.RunModal();
    }

    /// <summary>
    /// Open the file in <c>edFilename</c> and start playback.
    /// </summary>
    private async Task StartAsync()
    {
        if (View.Window.Delegate == null)
        {
            View.Window.Delegate = new CustomWindowDelegate(this);
        }

        if (!File.Exists(edFilename.StringValue))
        {
            Debug.WriteLine("Input file is not found!");
            return;
        }

        var sourceSettings = await UniversalSourceSettings.CreateAsync(edFilename.StringValue);
        if (sourceSettings == null)
        {
            Debug.WriteLine("Unable to create source!");
            return;
        }

        _player = new MediaPlayerCoreX(_videoView);

        // For a purchased licence, register it BEFORE OpenAsync:
        //   var cert = File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
        //   await _player.SetLicenseCertificateAsync(cert);

        await _player.OpenAsync(sourceSettings);

        await _player.PlayAsync();

        _timer.Change(0, 1000);
    }

    /// <summary>
    /// Stop playback and release the player.
    /// </summary>
    public async Task StopAsync()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);

        if (_player == null) return;

        await _player.StopAsync();
        await _player.DisposeAsync();

        _player = null;
    }

    partial void btStart_Click(NSObject sender)
    {
        InvokeOnMainThread(async () => { await StartAsync(); });
    }

    partial void btStop_Click(NSObject sender)
    {
        InvokeOnMainThread(async () => { await StopAsync(); });
    }

    partial void btOpen_Click(NSObject sender)
    {
        var dlg = NSOpenPanel.OpenPanel;
        dlg.CanChooseFiles = true;
        dlg.CanChooseDirectories = false;
        // Use AllowedContentTypes (macOS 12+) instead of deprecated AllowedFileTypes
        dlg.AllowedContentTypes = new[]
        {
            UTTypes.Mpeg4Movie,
            UTTypes.QuickTimeMovie,
            UTTypes.Movie,
            UTTypes.Audio,
            UTTypes.MP3,
            UTTypes.Wav
        };

        if (dlg.RunModal() == 1)
        {
            var url = dlg.Urls[0].Path;
            if (url != null) edFilename.StringValue = url;
        }
    }

    partial void slPositionChanged(NSObject sender)
    {
        if (sender is not NSSlider slider)
        {
            return;
        }

        var value = slider.FloatValue;

        InvokeOnMainThread(async () =>
        {
            if (!_timerFlag && _player != null)
            {
                if (_seekInFlight)
                {
                    return;
                }

                _seekInFlight = true;
                try
                {
                    await _player.Position_SetAsync(TimeSpan.FromSeconds(value));
                }
                finally
                {
                    _seekInFlight = false;
                }
            }
        });
    }
}

/// <summary>
/// Custom Window delegate — stops the player and tears the SDK down on close.
/// </summary>
public class CustomWindowDelegate : NSWindowDelegate
{
    private readonly ViewController _viewController;
    private bool _shuttingDown;

    public CustomWindowDelegate(ViewController viewController)
    {
        _viewController = viewController;
    }

    public override bool WindowShouldClose(NSObject sender)
    {
        // Second pass: async teardown finished, let the close proceed.
        if (_shuttingDown)
        {
            return true;
        }

        // Defer the close. WindowShouldClose runs on the main run loop, and StopAsync may need
        // to marshal back to it (renderer flush), so blocking here with .GetAwaiter().GetResult()
        // can deadlock. Run the async cleanup off the main loop, then close the window
        // programmatically when it completes — the _shuttingDown guard makes the re-entrant
        // WindowShouldClose return true.
        var window = sender as NSWindow ?? _viewController.View?.Window;
        _ = Task.Run(async () =>
        {
            // Stop the player before destroying the SDK so the pipeline drains cleanly.
            try { await _viewController.StopAsync(); } catch (Exception ex) { Debug.WriteLine(ex); }

            InvokeOnMainThread(() =>
            {
                VisioForgeX.DestroySDK();
                _shuttingDown = true;
                window?.Close();
            });
        });

        return false;
    }
}
