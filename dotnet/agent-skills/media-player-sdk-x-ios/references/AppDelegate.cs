using System.Diagnostics;
using Photos;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Apple;
using Timer = System.Timers.Timer;

namespace MediaPlayer;

/// <summary>
/// The app delegate.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    /// <summary>
    /// The media player instance.
    /// </summary>
    private MediaPlayerCoreX _player;

    /// <summary>
    /// The video view (a VisioForge.Core.UI.Apple.VideoView returned to us as a UIView).
    /// </summary>
    private UIView _videoView;

    /// <summary>
    /// The view controller (built in CustomViewController.cs).
    /// </summary>
    private CustomViewController _vc;

    /// <summary>
    /// True while a Position_SetAsync seek is in flight.
    /// </summary>
    private volatile bool _isSeeking;

    /// <summary>
    /// The position update timer (1 Hz).
    /// </summary>
    private System.Timers.Timer _tmPosition = new Timer(1000);

    /// <summary>
    /// Gets or sets the window.
    /// </summary>
    public override UIWindow? Window { get; set; }

    /// <summary>
    /// Show error message.
    /// </summary>
    private void ShowMessage(string message)
    {
        var alert = new UIAlertController
        {
            Title = "Error",
            Message = message
        };
        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
        var rootViewController = Window?.RootViewController;
        rootViewController?.PresentViewController(alert, true, null);
    }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // Build the window FIRST. The VideoView's MTKView grabs its drawable
        // surface lazily; building it before the window is real is a black-preview pitfall.
        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        // CustomViewController hands us back the embedded VideoView as a UIView,
        // and exposes SelectButton / PlayButton / PositionSlider for wiring below.
        _vc = new CustomViewController(Window, out _videoView);

        _vc.SelectButton.TouchUpInside += (sender, args) =>
        {
            _vc.OpenFilePicker();
        };

        _vc.PlayButton.TouchUpInside += async (sender, args) =>
        {
            if (_player != null)
            {
                await StopAllAsync();
                _vc.PlayButton.SetTitle("PLAY", UIControlState.Normal);
            }
            else
            {
                var uri = _vc.GetURL();
                if (uri == null)
                {
                    ShowMessage("Please select a file first.");
                    return;
                }

                await StopAllAsync();
                await CreateEngineAsync();

                await _player.PlayAsync();
                _tmPosition.Start();

                _vc.PlayButton.SetTitle("STOP", UIControlState.Normal);
            }
        };

        _vc.PositionSlider.ValueChanged += async (sender, args) =>
        {
            if (_player == null || _isSeeking)
            {
                return;
            }

            _isSeeking = true;
            await _player.Position_SetAsync(TimeSpan.FromSeconds(_vc.PositionSlider.Value), seekToKeyframe: true);
            _isSeeking = false;
        };

        // Triggers the photo-library permission prompt on first launch.
        // Required because the file picker is allowed to return Photos URLs.
        RequestPhotoLibraryPermissions(_ => { });

        Window.RootViewController = _vc;
        Window.MakeKeyAndVisible();

        // Mandatory: must run AFTER the window is key-and-visible (so the VideoView
        // has a backing Metal drawable) and BEFORE any MediaPlayerCoreX construction.
        // First call builds the GStreamer plugin-registry cache (~1-3 s); subsequent runs are instant.
        VisioForgeX.InitSDK();

        _tmPosition.Elapsed += async (sender, e) => await UpdatePositionAsync();

        return true;
    }

    /// <summary>
    /// Will terminate — best-effort teardown so the player is stopped and the SDK is released
    /// before the process exits. WillTerminate is synchronous, so use the sync Stop/Dispose
    /// overloads (StopAsync may need to marshal back to this same thread → deadlock).
    /// </summary>
    public override void WillTerminate(UIApplication application)
    {
        try
        {
            _tmPosition.Stop();

            if (_player != null)
            {
                _player.OnError -= PlayerOnError;
                _player.OnStart -= PlayerOnStart;
                _player.OnStop -= PlayerOnStop;
                _player.Stop();
                _player.Dispose();
                _player = null;
            }

            VisioForgeX.DestroySDK();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async Task UpdatePositionAsync()
    {
        if (_player == null || _isSeeking)
        {
            return;
        }

        var position = await _player.Position_GetAsync();
        UIApplication.SharedApplication.InvokeOnMainThread(() =>
        {
            _vc.PositionSlider.Value = (float)position.TotalSeconds;
        });
    }

    private void RequestPhotoLibraryPermissions(Action<PHAuthorizationStatus> completionHandler)
    {
        PHPhotoLibrary.RequestAuthorization(status => completionHandler(status));
    }

    private async Task StopAllAsync()
    {
        if (_player == null || _player.State == PlaybackState.Free)
        {
            return;
        }

        _tmPosition.Stop();

        if (_player != null)
        {
            await _player.StopAsync();
            await _player.DisposeAsync();
            _player = null;
        }
    }

    private async Task CreateEngineAsync()
    {
        if (_player != null)
        {
            await _player.StopAsync();
            await _player.DisposeAsync();
        }

        _player = new MediaPlayerCoreX(_videoView as IVideoView);

        _player.OnError += PlayerOnError;
        _player.OnStart += PlayerOnStart;
        _player.OnStop += PlayerOnStop;

        // UniversalSourceSettings probes the URL and reports stream counts so we
        // only enable Video_Play / Audio_Play for streams that actually exist.
        // Forgetting this on an audio-only file leaves Video_Play=true and the
        // engine wires a dead video tee — harmless but wastes time on first frame.
        var sourceSettings = await UniversalSourceSettings.CreateAsync(_vc.GetURL());

        _player.Video_Play = sourceSettings.GetInfo().VideoStreams.Count > 0;
        _player.Audio_Play = sourceSettings.GetInfo().AudioStreams.Count > 0;

        await _player.OpenAsync(sourceSettings);

        // To register a purchased licence, add right here:
        // var path = NSBundle.MainBundle.PathForResource("license", "vflicense");
        // var cert = File.ReadAllBytes(path);
        // await _player.SetLicenseCertificateAsync(cert);
    }

    private async void PlayerOnStop(object sender, StopEventArgs e)
    {
        await StopAllAsync();

        UIApplication.SharedApplication.InvokeOnMainThread(() =>
        {
            _vc.PositionSlider.Value = 0;
            _vc.PlayButton.SetTitle("PLAY", UIControlState.Normal);
        });
    }

    private void PlayerOnError(object sender, ErrorsEventArgs e)
    {
        Debug.WriteLine(e.Message);
    }

    private void PlayerOnStart(object sender, EventArgs e)
    {
        UIApplication.SharedApplication.InvokeOnMainThread(async () =>
        {
            try
            {
                if (_player == null)
                {
                    return;
                }

                var duration = (float)(await _player.DurationAsync()).TotalSeconds;
                _vc.PositionSlider.MaxValue = duration;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        });
    }
}
