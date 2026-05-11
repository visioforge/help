using System.Diagnostics;
using AVFoundation;
using ObjCRuntime;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;
using VisioForge.Core.UI.Apple;
using VisioForge.Core.VideoCaptureX;

namespace SimpleVideoCaptureMB;

public partial class ViewController : NSViewController
{
    private VideoCaptureCoreX _core;

    private VideoViewGL _videoView;

    protected ViewController(NativeHandle handle) : base(handle)
    {
        // This constructor is required if the view controller is loaded from a xib or a storyboard.
        // Do not put any initialization here, use ViewDidLoad instead.
    }

    public override NSObject RepresentedObject
    {
        get => base.RepresentedObject;
        set => base.RepresentedObject = value;
        // Update the view, if already loaded.
    }

        /// <summary>
        /// View did load.
        /// </summary>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Do any additional setup after loading the view.
        AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, granted =>
        {
            // Handle the response here. 'granted' is true if permission is given.
            Debug.WriteLine($"Camera access: {granted}");
        });

        InvokeOnMainThread(async () =>
        {
            try
            {
                await LoadDevicesAsync();

                View.Window.Delegate = new CustomWindowDelegate(this);
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        });
    }

        /// <summary>
        /// Load devices async.
        /// </summary>
    private async Task LoadDevicesAsync()
    {
        // video sources
        var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();
        cbVideoSource.RemoveAll();

        foreach (var item in videoSources) cbVideoSource.Add(new NSString(item.DisplayName));

        if (videoSources.Length > 0) cbVideoSource.Select(new NSString(videoSources[0].DisplayName));

        cbVideoSource.SelectionChanged += cbVideoSource_SelectionChanged;
        cbVideoFormat.SelectionChanged += cbVideoFormat_SelectionChanged;

        cbVideoSource_SelectionChanged(cbVideoSource, EventArgs.Empty);

        // audio sources
        var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();
        cbAudioSource.RemoveAll();

        foreach (var item in audioSources) cbAudioSource.Add(new NSString(item.DisplayName));

        if (audioSources.Length > 0) cbAudioSource.Select(new NSString(audioSources[0].DisplayName));

        cbAudioSource.SelectionChanged += cbAudioSource_SelectionChanged;

        cbAudioSource_SelectionChanged(cbAudioSource, EventArgs.Empty);

        // audio outputs
        var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
        cbAudioOutput.RemoveAll();

        foreach (var item in audioOutputs) cbAudioOutput.Add(new NSString(item.DisplayName));

        if (audioOutputs.Length > 0) cbAudioOutput.Select(new NSString(audioOutputs[0].DisplayName));
    }

        /// <summary>
        /// Handles the cb video format selection changed event.
        /// </summary>
    private async void cbVideoFormat_SelectionChanged(object sender, EventArgs e)
    {
        cbVideoFrameRate.RemoveAll();

        if (cbVideoSource.SelectedIndex != -1 && e != null)
        {
            var deviceName = cbVideoSource.SelectedValue.ToString();
            var format = cbVideoFormat.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(format))
            {
                var device =
                    (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x =>
                        x.DisplayName == deviceName);
                if (device != null)
                {
                    var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
                    if (formatItem != null)
                    {
                        // build int range from tuple (min, max)
                        var frameRateList = formatItem.GetFrameRateRangeAsStringList();
                        foreach (var item in frameRateList) cbVideoFrameRate.Add(new NSString(item));

                        if (frameRateList.Length > 0) cbVideoFrameRate.Select(new NSString(frameRateList[0]));
                    }
                }
            }
        }
    }

        /// <summary>
        /// Handles the cb audio source selection changed event.
        /// </summary>
    private async void cbAudioSource_SelectionChanged(object sender, EventArgs e)
    {
        if (cbAudioSource.SelectedIndex != -1 && e != null)
        {
            var deviceName = cbAudioSource.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(deviceName))
            {
                cbAudioFormat.RemoveAll();

                var device =
                    (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault(x =>
                        x.DisplayName == deviceName);
                if (device != null)
                {
                    foreach (var format in device.Formats) cbAudioFormat.Add(new NSString(format.Name));

                    if (device.Formats.Count > 0) cbAudioFormat.Select(new NSString(device.Formats[0].Name));
                }
            }
        }
    }

        /// <summary>
        /// Handles the cb video source selection changed event.
        /// </summary>
    private async void cbVideoSource_SelectionChanged(object sender, EventArgs e)
    {
        if (cbVideoSource.SelectedIndex != -1)
        {
            var deviceName = cbVideoSource.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(deviceName))
            {
                cbVideoFormat.RemoveAll();

                var device =
                    (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x =>
                        x.DisplayName == deviceName);
                if (device != null)
                {
                    foreach (var item in device.VideoFormats) cbVideoFormat.Add(new NSString(item.Name));

                    if (device.VideoFormats.Count > 0) cbVideoFormat.Select(new NSString(device.VideoFormats[0].Name));
                }
            }
        }
    }

        /// <summary>
        /// Start async.
        /// </summary>
    private async Task StartAsync()
    {
        // Create the GL view once and reuse it across Start/Stop cycles. Recreating it on every
        // Start (without removing the old one in StopAsync) leaves stale overlapping GL subviews
        // accumulating in videoViewHost.
        if (_videoView == null)
        {
            _videoView = new VideoViewGL(new CGRect(0, 0, videoViewHost.Bounds.Width, videoViewHost.Bounds.Height));
            videoViewHost.AddSubview(_videoView);
        }

        _core = new VideoCaptureCoreX(_videoView);

        // video source
        VideoCaptureDeviceSourceSettings videoSourceSettings = null;

        var deviceName = cbVideoSource.StringValue;
        var format = cbVideoFormat.StringValue;
        if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(format))
        {
            var device =
                (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
            if (device != null)
            {
                var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
                if (formatItem != null)
                {
                    videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
                    {
                        Format = formatItem.ToFormat()
                    };

                    videoSourceSettings.Format.FrameRate =
                        new VideoFrameRate(Convert.ToDouble(cbVideoFrameRate.StringValue));
                }
            }
        }

        _core.Video_Source = videoSourceSettings;

        // audio source
        IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;

        deviceName = cbAudioSource.StringValue;
        format = cbAudioFormat.StringValue;
        if (!string.IsNullOrEmpty(deviceName))
        {
            var device =
                (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
            if (device != null)
            {
                var formatItem = device.Formats.FirstOrDefault(x => x.Name == format);
                if (formatItem != null) audioSourceSettings = device.CreateSourceSettingsVC(formatItem.ToFormat());
            }
        }

        _core.Audio_Source = audioSourceSettings;

        // audio renderer (optional — only when the selected output device resolves)
        var audioOutput = (await DeviceEnumerator.Shared.AudioOutputsAsync())
            .FirstOrDefault(device => device.DisplayName == cbAudioOutput.StringValue);
        if (audioOutput != null)
        {
            _core.Audio_OutputDevice = new AudioRendererSettings(audioOutput);
        }

        // start
        await _core.StartAsync();
    }

        /// <summary>
        /// Stop async.
        /// </summary>
    public async Task StopAsync()
    {
        if (_core == null)
        {
            return;
        }

        await _core.StopAsync();
        await _core.DisposeAsync();

        _core = null;
    }

    partial void btStartClick(NSObject sender)
    {
        InvokeOnMainThread(async () =>
        {
            try { await StartAsync(); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        });
    }

    partial void btStopClick(NSObject sender)
    {
        InvokeOnMainThread(async () =>
        {
            try { await StopAsync(); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        });
    }
}

// Custom Window delegate — stops capture and tears the SDK down on close.
public class CustomWindowDelegate : NSWindowDelegate
{
    private readonly ViewController _viewController;
    private bool _shuttingDown;

    public CustomWindowDelegate(ViewController viewController)
    {
        _viewController = viewController;
    }

    /// <summary>
    /// Window should close.
    /// </summary>
    public override bool WindowShouldClose(NSObject sender)
    {
        // Second pass: async teardown finished, let the close proceed.
        if (_shuttingDown)
        {
            return true;
        }

        // Defer the close. WindowShouldClose runs on the main run loop, and StopAsync may need
        // to marshal back to it (renderer flush), so blocking here can deadlock. Run the async
        // cleanup off the main loop, then close the window programmatically when it completes —
        // the _shuttingDown guard makes the re-entrant WindowShouldClose return true. This avoids
        // tearing the SDK down while a capture pipeline is still running.
        var window = sender as NSWindow ?? _viewController.View?.Window;
        _ = Task.Run(async () =>
        {
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
