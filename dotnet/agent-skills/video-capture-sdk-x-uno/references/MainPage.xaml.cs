#if (__IOS__ && !__MACCATALYST__) || __ANDROID__
#define MOBILE
#endif

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using SysDebug = System.Diagnostics.Debug;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Helpers;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;
using VisioForge.Core.VideoCaptureX;

#if __IOS__ || __MACCATALYST__
using AVFoundation;
#endif

#if __IOS__ && !__MACCATALYST__
using Foundation;
#endif

#if __ANDROID__
using Android.Widget;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Core.App;
#endif

#if NET_WINDOWS
using Windows.Media.Capture;
#endif

namespace SimpleCaptureUno;

public sealed partial class MainPage : Page
{
    private VideoCaptureCoreX? _core;

    private VideoCaptureDeviceInfo[]? _cameras;
    private int _cameraSelectedIndex = 0;

    private AudioCaptureDeviceInfo[]? _mics;
    private int _micSelectedIndex = 0;

    private AudioOutputDeviceInfo[]? _speakers;
    private int _speakerSelectedIndex = 0;

    private SolidColorBrush? _defaultButtonBackground;

    public MainPage()
    {
        this.InitializeComponent();

        Loaded += MainPage_Loaded;
        Unloaded += MainPage_Unloaded;
    }

    private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_core != null)
            {
                _core.OnError -= Core_OnError;
                await _core.StopAsync();
                await _core.DisposeAsync();
                _core = null;
            }
        }
        catch (Exception ex)
        {
            SysDebug.WriteLine($"Error during unload: {ex.Message}");
        }
        finally
        {
            VisioForgeX.DestroySDK();
        }
    }

#if __IOS__ || __MACCATALYST__
    private async Task<bool> EnsureApplePermissionsAsync()
    {
        var cameraStatus = AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Video);
        if (cameraStatus != AVAuthorizationStatus.Authorized)
        {
            var granted = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVAuthorizationMediaType.Video);
            if (!granted) return false;
        }

        var micStatus = AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Audio);
        if (micStatus != AVAuthorizationStatus.Authorized)
        {
            var granted = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVAuthorizationMediaType.Audio);
            if (!granted) return false;
        }

        return true;
    }
#endif

#if __ANDROID__
    private async Task<bool> EnsureAndroidPermissionsAsync()
    {
        var activity = ContextHelper.Current as Android.App.Activity;
        if (activity == null) return false;

        var cameraPermission = ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.Camera);
        var micPermission = ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.RecordAudio);

        if (cameraPermission != Permission.Granted || micPermission != Permission.Granted)
        {
            var permissions = new[]
            {
                Android.Manifest.Permission.Camera,
                Android.Manifest.Permission.RecordAudio
            };

            ActivityCompat.RequestPermissions(activity, permissions, 1);
            await Task.Delay(1000);

            cameraPermission = ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.Camera);
            micPermission = ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.RecordAudio);

            return cameraPermission == Permission.Granted && micPermission == Permission.Granted;
        }

        return true;
    }
#endif

#if NET_WINDOWS
    private async Task<bool> EnsureWindowsPermissionsAsync()
    {
        try
        {
            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo
            };

            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(settings);
            mediaCapture.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }
#endif

    private void ShowStatus(string message, string color = "#00FF00")
    {
        SysDebug.WriteLine($"ShowStatus: {message}");
        DispatcherQueue.TryEnqueue(() =>
        {
            lblStatus.Text = message;
            lblStatus.Foreground = new SolidColorBrush(GetColorFromHex(color));
        });
    }

    private static Windows.UI.Color GetColorFromHex(string hex)
    {
        hex = hex.Replace("#", string.Empty);
        byte a = 255;
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        return Windows.UI.Color.FromArgb(a, r, g, b);
    }

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _defaultButtonBackground = btStartCapture.Background as SolidColorBrush;

#if __IOS__ || __MACCATALYST__
            var permissionsOk = await EnsureApplePermissionsAsync();
#elif __ANDROID__
            var permissionsOk = await EnsureAndroidPermissionsAsync();
#elif NET_WINDOWS
            var permissionsOk = await EnsureWindowsPermissionsAsync();
#else
            var permissionsOk = true;
#endif
            if (!permissionsOk)
            {
                ShowStatus("Permissions required", "#FF0000");
                return;
            }

            await VisioForgeX.InitSDKAsync();

            // VideoView (Uno) implements IVideoView directly — pass it straight to the core ctor.
            _core = new VideoCaptureCoreX(videoView);

            // For a purchased licence, add these two lines here:
            //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
            //   await _core.SetLicenseCertificateAsync(cert);

            _core.OnError += Core_OnError;

            _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
            if (_cameras.Length > 0) btCamera.Content = _cameras[0].DisplayName;

            _mics = await DeviceEnumerator.Shared.AudioSourcesAsync(null);
            if (_mics.Length > 0) btMic.Content = _mics[0].DisplayName;

            _speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(null);
            if (_speakers.Length > 0) btSpeakers.Content = _speakers[0].DisplayName;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
            // Prefer the back-facing camera on mobile platforms.
            if (_cameras.Length > 1)
            {
                btCamera.Content = _cameras[1].DisplayName;
                _cameraSelectedIndex = 1;
            }

            btStartCapture.IsEnabled = true;
            await StartPreview();
#elif __MACCATALYST__
            ShowStatus("Ready");
#endif
        }
        catch (Exception ex)
        {
            SysDebug.WriteLine($"Error during initialization: {ex.Message}");
            ShowStatus("Init failed", "#FF0000");
        }
    }

    private void Core_OnError(object? sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
    {
        SysDebug.WriteLine(e.Message);
    }

    private async Task StopAllAsync()
    {
        if (_core == null) return;
        await _core.StopAsync();
    }

    private async Task StopCaptureAsync()
    {
        if (_core == null) return;

        var output = _core.Outputs_Get(0);
        var filename = output?.GetFilename();

        await _core.StopCaptureAsync(0);
        btStartCapture.Background = _defaultButtonBackground;
        btStartCapture.Content = "CAPTURE";

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
        if (!string.IsNullOrEmpty(filename))
        {
            await PhotoGalleryHelper.AddVideoToGalleryAsync(filename);
        }
#endif
        ShowStatus($"Saved: {filename}");
    }

    private async void btCamera_Clicked(object sender, RoutedEventArgs e)
    {
        if (_cameras == null || _cameras.Length < 2) return;

        _cameraSelectedIndex++;
        if (_cameraSelectedIndex >= _cameras.Length) _cameraSelectedIndex = 0;
        btCamera.Content = _cameras[_cameraSelectedIndex].DisplayName;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
        await StopAllAsync();
        await StartPreview();
#endif
    }

    private void btMic_Clicked(object sender, RoutedEventArgs e)
    {
        if (_mics == null || _mics.Length < 2) return;
        _micSelectedIndex++;
        if (_micSelectedIndex >= _mics.Length) _micSelectedIndex = 0;
        btMic.Content = _mics[_micSelectedIndex].DisplayName;
    }

    private void btSpeakers_Clicked(object sender, RoutedEventArgs e)
    {
        if (_speakers == null || _speakers.Length < 2) return;
        _speakerSelectedIndex++;
        if (_speakerSelectedIndex >= _speakers.Length) _speakerSelectedIndex = 0;
        btSpeakers.Content = _speakers[_speakerSelectedIndex].DisplayName;
    }

    private async Task StartPreview()
    {
        if (_core == null) return;
        if (_core.State == PlaybackState.Play || _core.State == PlaybackState.Pause) return;

#if MOBILE
        // No system speakers on mobile — leave audio output disabled.
        _core.Audio_Play = false;
#else
        var speakerName = btSpeakers.Content?.ToString();
        if (!string.IsNullOrEmpty(speakerName))
        {
            var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())
                .FirstOrDefault(d => d.DisplayName == speakerName);
            if (audioOutputDevice != null)
            {
                _core.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);
                _core.Audio_Play = true;
            }
        }
#endif

        VideoCaptureDeviceSourceSettings? videoSourceSettings = null;
        var deviceName = btCamera.Content?.ToString();
        if (!string.IsNullOrEmpty(deviceName))
        {
            var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                .FirstOrDefault(x => x.DisplayName == deviceName);
            if (device != null)
            {
                var formatItem = device.GetHDVideoFormatAndFrameRate(out var frameRate);
                if (formatItem != null)
                {
                    videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
                    {
                        Format = formatItem.ToFormat()
                    };
                    videoSourceSettings.Format.FrameRate = frameRate;
                }
            }
        }

        _core.Video_Source = videoSourceSettings;

        if (videoSourceSettings == null)
        {
            ShowStatus("Camera error", "#FF0000");
            return;
        }

        IVideoCaptureBaseAudioSourceSettings? audioSourceSettings = null;
        var micName = btMic.Content?.ToString();
        if (!string.IsNullOrEmpty(micName))
        {
            var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                .FirstOrDefault(x => x.DisplayName == micName);
            if (device != null)
            {
                var formatItem = device.GetDefaultFormat();
                audioSourceSettings = device.CreateSourceSettingsVC(formatItem);
            }
        }

        _core.Audio_Source = audioSourceSettings;
        _core.Audio_Record = true;

        _core.Outputs_Clear();
        _core.Outputs_Add(new MP4Output(GenerateFilename(),
            H264EncoderBlock.GetDefaultSettings(), new MP3EncoderSettings()), false);

        await _core.StartAsync();

        btStartPreview.Content = "STOP";
        ShowStatus("Preview");
    }

    private string GenerateFilename()
    {
        DateTime now = DateTime.Now;
#if __ANDROID__
        string filename;
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
        {
            // Android 10+ — scoped storage, app-specific external dir, no special permission needed.
            var context = Android.App.Application.Context;
            var moviesDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryMovies);
            if (moviesDir != null && !moviesDir.Exists()) moviesDir.Mkdirs();

            filename = Path.Combine(moviesDir?.AbsolutePath ?? context.FilesDir!.AbsolutePath,
                $"visioforge_{now.Hour}_{now.Minute}_{now.Second}.mp4");
        }
        else
        {
            // Android 9 and below — public DCIM/Camera; needs WRITE_EXTERNAL_STORAGE.
            var dcimDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
            var cameraDir = new Java.IO.File(dcimDir, "Camera");
            if (!cameraDir.Exists()) cameraDir.Mkdirs();

            filename = Path.Combine(cameraDir.AbsolutePath, $"visioforge_{now.Hour}_{now.Minute}_{now.Second}.mp4");
        }
#elif __IOS__ && !__MACCATALYST__
        var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..",
            "Library", $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#elif __MACCATALYST__
        // Sandbox disabled in Entitlements.plist — write directly to ~/Movies.
        var moviesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Movies");
        if (!Directory.Exists(moviesFolder)) Directory.CreateDirectory(moviesFolder);
        var filename = Path.Combine(moviesFolder, $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#else
        var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#endif
        return filename;
    }

    private async void btStartPreview_Clicked(object sender, RoutedEventArgs e)
    {
        if (_core == null) return;

        switch (_core.State)
        {
            case PlaybackState.Play:
                await StopAllAsync();
                btStartPreview.Content = "PREVIEW";
                btStartCapture.IsEnabled = false;
                ShowStatus("Stopped");
                break;
            case PlaybackState.Free:
                await StartPreview();
                btStartCapture.IsEnabled = true;
                break;
        }
    }

    private async void btStartCapture_Clicked(object sender, RoutedEventArgs e)
    {
        if (_core == null || _core.State != PlaybackState.Play) return;

        var currentBrush = btStartCapture.Background as SolidColorBrush;
        if (currentBrush == null || currentBrush.Color != Microsoft.UI.Colors.Red)
        {
            await _core.StartCaptureAsync(0, GenerateFilename());
            btStartCapture.Background = new SolidColorBrush(Microsoft.UI.Colors.Red);
            btStartCapture.Content = "STOP";
            ShowStatus("Recording", "#FF0000");
        }
        else
        {
            await StopCaptureAsync();
        }
    }
}
