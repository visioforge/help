#if (__IOS__ && !__MACCATALYST__) || __ANDROID__
#define MOBILE
#endif

using System;
using System.ComponentModel;
using System.Diagnostics;

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

namespace SimpleCapture
{
    /// <summary>
    /// MAUI cross-platform Video Capture SDK X demo. Uses the high-level
    /// VideoCaptureCoreX god-object (preview + record from a single instance)
    /// rather than building a Media Blocks graph by hand.
    ///
    /// Engine boot model is different from the WPF host: here we DON'T call
    /// VisioForgeX.InitSDKAsync() before constructing VideoCaptureCoreX — the
    /// MAUI handler chain initializes the native runtime when AddVisioForgeHandlers()
    /// is registered in MauiProgram. We DO call VisioForgeX.DestroySDK() on shutdown.
    /// </summary>
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private VideoCaptureCoreX _core;

        private VideoCaptureDeviceInfo[] _cameras;
        private int _cameraSelectedIndex = 0;

        private AudioCaptureDeviceInfo[] _mics;
        private int _micSelectedIndex = 0;

        private AudioOutputDeviceInfo[] _speakers;
        private int _speakerSelectedIndex = 0;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
            this.BindingContext = this;
        }

        private void MainPage_Unloaded(object? sender, EventArgs e)
        {
            _core?.Dispose();
            _core = null;
            VisioForgeX.DestroySDK();
        }

        // ---- Permissions ----------------------------------------------------

        private async Task RequestCameraPermissionAsync()
        {
            var result = await Permissions.RequestAsync<Permissions.Camera>();
            if (result != PermissionStatus.Granted && Permissions.ShouldShowRationale<Permissions.Camera>())
            {
                if (await DisplayAlertAsync(null, "You need to allow access to the Camera", "OK", "Cancel"))
                    await RequestCameraPermissionAsync();
            }
        }

        private async Task RequestMicPermissionAsync()
        {
            var result = await Permissions.RequestAsync<Permissions.Microphone>();
            if (result != PermissionStatus.Granted && Permissions.ShouldShowRationale<Permissions.Microphone>())
            {
                if (await DisplayAlertAsync(null, "You need to allow access to the Microphone", "OK", "Cancel"))
                    await RequestMicPermissionAsync();
            }
        }

#if __IOS__ && !__MACCATALYST__
        private void RequestPhotoPermission()
        {
            Photos.PHPhotoLibrary.RequestAuthorization(status =>
            {
                if (status == Photos.PHAuthorizationStatus.Authorized)
                    Debug.WriteLine("Photo library access granted.");
            });
        }
#endif

#if __ANDROID__
        private async Task RequestStoragePermissionAsync()
        {
            // Android 13+ (API 33+): granular media permissions
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
            {
                var result = await Permissions.RequestAsync<Permissions.Media>();
                if (result != PermissionStatus.Granted && Permissions.ShouldShowRationale<Permissions.Media>())
                {
                    if (await DisplayAlertAsync(null, "You need to allow access to save videos", "OK", "Cancel"))
                        await RequestStoragePermissionAsync();
                }
            }
            else if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                // Android 10-12: scoped storage covers app-specific dirs without permission.
                return;
            }
            else
            {
                var result = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (result != PermissionStatus.Granted && Permissions.ShouldShowRationale<Permissions.StorageWrite>())
                {
                    if (await DisplayAlertAsync(null, "You need to allow storage access to save videos", "OK", "Cancel"))
                        await RequestStoragePermissionAsync();
                }
            }
        }
#endif

        // ---- Lifecycle ------------------------------------------------------

        private async void MainPage_Loaded(object? sender, EventArgs e)
        {
#if __ANDROID__ || __MACOS__ || __MACCATALYST__ || __IOS__
            await RequestCameraPermissionAsync();
            await RequestMicPermissionAsync();
#endif
#if __ANDROID__
            await RequestStoragePermissionAsync();
#endif
#if __IOS__ && !__MACCATALYST__
            RequestPhotoPermission();
#endif

            // The MAUI VideoView yields the platform IVideoView the engine binds to.
            IVideoView vv = videoView.GetVideoView();
            _core = new VideoCaptureCoreX(vv);
            _core.OnError += Core_OnError;

            // For a purchased licence, register here BEFORE StartAsync. Ship the
            // .vflicense as a MauiAsset (csproj: <MauiAsset Include="Resources\Raw\license.vflicense" />)
            // and load it via FileSystem.OpenAppPackageFileAsync — see SKILL.md "License registration".
            //
            //   using var stream = await FileSystem.OpenAppPackageFileAsync("license.vflicense");
            //   using var ms = new MemoryStream();
            //   await stream.CopyToAsync(ms);
            //   await _core.SetLicenseCertificateAsync(ms.ToArray());

            _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
            if (_cameras.Length > 0) btCamera.Text = _cameras[0].DisplayName;

            _mics = await DeviceEnumerator.Shared.AudioSourcesAsync(null);
            if (_mics.Length > 0) btMic.Text = _mics[0].DisplayName;

            _speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(null);
            if (_speakers.Length > 0) btSpeakers.Text = _speakers[0].DisplayName;

            Window.Destroying += Window_Destroying;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
            // Mobile: prefer front camera if available, auto-start preview.
            if (_cameras.Length > 1)
            {
                btCamera.Text = _cameras[1].DisplayName;
                _cameraSelectedIndex = 1;
            }

            btStartCapture.IsEnabled = true;
            await StartPreview();
#endif
        }

        private async void Window_Destroying(object? sender, EventArgs e)
        {
            if (_core != null)
            {
                _core.OnError -= Core_OnError;
                await _core.StopAsync();
                await _core.DisposeAsync();
                _core = null;
            }
            VisioForgeX.DestroySDK();
        }

        private void Core_OnError(object? sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
        {
            Debug.WriteLine(e.Message);
        }

        private async Task StopAllAsync()
        {
            if (_core != null)
                await _core.StopAsync();
        }

        // ---- iOS Photos library save (recorded MP4 → Photos) ---------------

#if __IOS__ && !__MACCATALYST__
        private void AddVideoToPhotosLibrary(string filePath)
        {
            var fileUrl = Foundation.NSUrl.FromFilename(filePath);
            Photos.PHPhotoLibrary.RequestAuthorization(status =>
            {
                if (status == Photos.PHAuthorizationStatus.Authorized)
                {
                    Photos.PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
                    {
                        Photos.PHAssetChangeRequest.FromVideo(fileUrl);
                    }, (success, error) =>
                    {
                        if (success) Console.WriteLine("Video saved to Photos library.");
                        else Console.WriteLine($"Error saving video: {error?.LocalizedDescription}");
                    });
                }
            });
        }
#endif

        private async Task StopCaptureAsync()
        {
            await _core.StopCaptureAsync(0);
            btStartCapture.Text = "CAPTURE";

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
            // Save recorded file to the device gallery on mobile platforms.
            var output = _core.Outputs_Get(0);
            string filename = output.GetFilename();
            await PhotoGalleryHelper.AddVideoToGalleryAsync(filename);
#endif
        }

        // ---- Device-cycle buttons ------------------------------------------

        private async void btCamera_Clicked(object? sender, EventArgs e)
        {
            if (_cameras == null || _cameras.Length < 2) return;
            _cameraSelectedIndex = (_cameraSelectedIndex + 1) % _cameras.Length;
            btCamera.Text = _cameras[_cameraSelectedIndex].DisplayName;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
            await StopAllAsync();
            await StartPreview();
#endif
        }

        private void btMic_Clicked(object? sender, EventArgs e)
        {
            if (_mics == null || _mics.Length < 2) return;
            _micSelectedIndex = (_micSelectedIndex + 1) % _mics.Length;
            btMic.Text = _mics[_micSelectedIndex].DisplayName;
        }

        private void btSpeakers_Clicked(object? sender, EventArgs e)
        {
            if (_speakers == null || _speakers.Length < 2) return;
            _speakerSelectedIndex = (_speakerSelectedIndex + 1) % _speakers.Length;
            btSpeakers.Text = _speakers[_speakerSelectedIndex].DisplayName;
        }

        // ---- Preview + record ----------------------------------------------

        private async Task StartPreview()
        {
            if (_core.State == PlaybackState.Play || _core.State == PlaybackState.Pause)
                return;

#if MOBILE
            // Mobile: the system audio output is fixed; don't route via Audio_OutputDevice.
            _core.Audio_Play = false;
#else
            var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())
                .First(d => d.DisplayName == btSpeakers.Text);
            _core.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);
            _core.Audio_Play = true;
#endif

            // Video source.
            VideoCaptureDeviceSourceSettings videoSourceSettings = null;
            var deviceName = btCamera.Text;
            if (!string.IsNullOrEmpty(deviceName))
            {
                var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                    .FirstOrDefault(x => x.DisplayName == deviceName);
                if (device != null)
                {
                    var fmt = device.GetHDVideoFormatAndFrameRate(out var frameRate);
                    if (fmt != null)
                    {
                        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
                        {
                            Format = fmt.ToFormat()
                        };
                        videoSourceSettings.Format.FrameRate = frameRate;
                    }
                }
            }
            _core.Video_Source = videoSourceSettings;

            if (videoSourceSettings == null)
                await DisplayAlertAsync("Error", "Unable to configure camera settings", "OK");

            // Audio source.
            IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;
            deviceName = btMic.Text;
            if (!string.IsNullOrEmpty(deviceName))
            {
                var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                    .FirstOrDefault(x => x.DisplayName == deviceName);
                if (device != null)
                {
                    var formatItem = device.GetDefaultFormat();
                    audioSourceSettings = device.CreateSourceSettingsVC(formatItem);
                }
            }
            _core.Audio_Source = audioSourceSettings;
            _core.Audio_Record = true;

            // Configure MP4 output (deferred — StartCaptureAsync below kicks recording).
            _core.Outputs_Clear();
            _core.Outputs_Add(new MP4Output(GenerateFilename(),
                                             H264EncoderBlock.GetDefaultSettings(),
                                             new MP3EncoderSettings()), false);

            await _core.StartAsync();
            btStartPreview.Text = "STOP";
        }

        private string GenerateFilename()
        {
            DateTime now = DateTime.Now;
#if __ANDROID__
            string filename;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                // Android 10+: app-specific external Movies dir; no permissions needed.
                var context = Android.App.Application.Context;
                var moviesDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryMovies);
                if (moviesDir != null && !moviesDir.Exists()) moviesDir.Mkdirs();
                filename = Path.Combine(moviesDir?.AbsolutePath ?? context.FilesDir.AbsolutePath,
                    $"visioforge_{now.Hour}_{now.Minute}_{now.Second}.mp4");
            }
            else
            {
                var dcimDir = Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryDcim);
                var cameraDir = new Java.IO.File(dcimDir, "Camera");
                if (!cameraDir.Exists()) cameraDir.Mkdirs();
                filename = Path.Combine(cameraDir.AbsolutePath, $"visioforge_{now.Hour}_{now.Minute}_{now.Second}.mp4");
            }
#elif __IOS__ && !__MACCATALYST__
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..",
                "Library", $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#elif __MACCATALYST__
            var filename = Path.Combine("/tmp", $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#else
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#endif
            return filename;
        }

        private async void btStartPreview_Clicked(object? sender, EventArgs e)
        {
            if (_core == null) return;

            switch (_core.State)
            {
                case PlaybackState.Play:
                    await StopAllAsync();
                    btStartPreview.Text = "PREVIEW";
                    btStartCapture.IsEnabled = false;
                    break;
                case PlaybackState.Free:
                    await StartPreview();
                    btStartCapture.IsEnabled = true;
                    break;
            }
        }

        private async void btStartCapture_Clicked(object? sender, EventArgs e)
        {
            if (_core == null || _core.State != PlaybackState.Play) return;

            if (btStartCapture.BackgroundColor != Colors.Red)
            {
                await _core.StartCaptureAsync(0, GenerateFilename());
                btStartCapture.BackgroundColor = Colors.Red;
                btStartCapture.Text = "STOP";
            }
            else
            {
                await StopCaptureAsync();
            }
        }
    }
}
