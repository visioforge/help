#if (__IOS__ && !__MACCATALYST__) || __ANDROID__
#define MOBILE
#endif

using System;
using System.Diagnostics;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Helpers;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

namespace SimpleCaptureMB
{
    /// <summary>
    /// Main page. Hosts a Media Blocks pipeline that does either preview-only or
    /// preview + record-to-MP4 from camera + microphone. Demonstrates the cross-platform
    /// integration pattern for Media Blocks SDK on .NET MAUI.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private MediaBlocksPipeline _pipeline;
        private SystemVideoSourceBlock _videoSource;
        private MediaBlock _audioSource;
#if !MOBILE
        private AudioRendererBlock _audioOutput;
#endif
        private MediaBlock _videoRenderer;
        private TeeBlock _videoTee;
        private TeeBlock _audioTee;
        private MP4SinkBlock _mp4Sink;
        private H264EncoderBlock _videoEncoder;
        private AACEncoderBlock _audioEncoder;

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

            this.BindingContext = this;

            // Init must happen on the UI thread before any pipeline construction.
            // When integrating a purchased license, call
            //   await _pipeline.SetLicenseCertificateAsync(certBytes);
            // on every MediaBlocksPipeline AFTER construction and BEFORE its
            // StartAsync. See SKILL.md "License registration".
            VisioForgeX.InitSDK();
        }

        private void CreateEngine()
        {
            _pipeline = new MediaBlocksPipeline();

            // GetVideoView() pulls the per-OS native IVideoView out of the MAUI VideoView wrapper.
            IVideoView vv = videoView.GetVideoView();
            _videoRenderer = new VideoRendererBlock(_pipeline, vv) { IsSync = false };

            _pipeline.OnError += Core_OnError;
        }

        private async void MainPage_Loaded(object? sender, EventArgs e)
        {
            try
            {
#if __ANDROID__ || __MACOS__ || __MACCATALYST__ || __IOS__
                await RequestCameraPermissionAsync();
                await RequestMicPermissionAsync();
#endif

#if __IOS__ && !__MACCATALYST__
                RequestPhotoPermission();
#endif

                _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
                if (_cameras.Length > 0)
                {
                    btCamera.Text = _cameras[0].DisplayName;
                }

                _mics = await DeviceEnumerator.Shared.AudioSourcesAsync(null);
                if (_mics.Length > 0)
                {
                    btMic.Text = _mics[0].DisplayName;
                }

                _speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(null);
                if (_speakers.Length > 0)
                {
                    btSpeakers.Text = _speakers[0].DisplayName;
                }

                Window.Destroying += Window_Destroying;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading page: {ex.Message}");
                await DisplayAlert("Error", $"Failed to initialize devices: {ex.Message}", "OK");
            }
        }

        private async Task RequestCameraPermissionAsync()
        {
            var result = await Permissions.RequestAsync<Permissions.Camera>();
            if (result != PermissionStatus.Granted)
            {
                if (Permissions.ShouldShowRationale<Permissions.Camera>())
                {
                    if (await DisplayAlert(null, "You need to allow access to the Camera", "OK", "Cancel"))
                        await RequestCameraPermissionAsync();
                }
            }
        }

        private async Task RequestMicPermissionAsync()
        {
            var result = await Permissions.RequestAsync<Permissions.Microphone>();
            if (result != PermissionStatus.Granted)
            {
                if (Permissions.ShouldShowRationale<Permissions.Microphone>())
                {
                    if (await DisplayAlert(null, "You need to allow access to the Microphone", "OK", "Cancel"))
                        await RequestMicPermissionAsync();
                }
            }
        }

#if __IOS__ && !__MACCATALYST__
        private void RequestPhotoPermission()
        {
            Photos.PHPhotoLibrary.RequestAuthorization(status =>
            {
                if (status == Photos.PHAuthorizationStatus.Authorized)
                {
                    Debug.WriteLine("Photo library access granted.");
                }
            });
        }
#endif

        private async void Window_Destroying(object? sender, EventArgs e)
        {
            try
            {
                if (_pipeline != null)
                {
                    _pipeline.OnError -= Core_OnError;
                    await _pipeline.StopAsync();
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }
                VisioForgeX.DestroySDK();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        private void Core_OnError(object? sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
        {
            Debug.WriteLine(e.Message);
        }

        private async Task StopAllAsync()
        {
            if (_pipeline == null) return;

            await _pipeline.StopAsync();
            await _pipeline.DisposeAsync();
            _pipeline = null;

            _videoEncoder?.Dispose(); _videoEncoder = null;
            _mp4Sink?.Dispose(); _mp4Sink = null;
            _videoSource?.Dispose(); _videoSource = null;
            _audioSource?.Dispose(); _audioSource = null;
            _videoTee?.Dispose(); _videoTee = null;
            _audioTee?.Dispose(); _audioTee = null;
            _audioEncoder?.Dispose(); _audioEncoder = null;
            _videoRenderer?.Dispose(); _videoRenderer = null;
#if !MOBILE
            _audioOutput?.Dispose(); _audioOutput = null;
#endif
        }

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
                        if (success) Debug.WriteLine("Video saved to Photos library.");
                        else Debug.WriteLine($"Error saving video: {error?.LocalizedDescription}");
                    });
                }
            });
        }
#endif

        private async void btStop_Clicked(object? sender, EventArgs e)
        {
            try
            {
                bool capture = _mp4Sink != null;
                string filename = null;
                if (capture) filename = _mp4Sink.GetFilenameOrURL();

                await StopAllAsync();

                if (capture && !string.IsNullOrEmpty(filename))
                {
#if __IOS__ && !__MACCATALYST__
                    AddVideoToPhotosLibrary(filename);
#elif __ANDROID__
                    await PhotoGalleryHelper.AddVideoToGalleryAsync(filename);
#endif
                }

                btStartPreview.Text = "PREVIEW";
                btStartCapture.Text = "CAPTURE";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping capture: {ex.Message}");
                await DisplayAlert("Error", $"Failed to stop capture: {ex.Message}", "OK");
            }
        }

        private void btCamera_Clicked(object? sender, EventArgs e)
        {
            if (_cameras == null || _cameras.Length == 0) return;
            _cameraSelectedIndex = (_cameraSelectedIndex + 1) % _cameras.Length;
            btCamera.Text = _cameras[_cameraSelectedIndex].DisplayName;
        }

        private void btMic_Clicked(object? sender, EventArgs e)
        {
            if (_mics == null || _mics.Length == 0) return;
            _micSelectedIndex = (_micSelectedIndex + 1) % _mics.Length;
            btMic.Text = _mics[_micSelectedIndex].DisplayName;
        }

        private void btSpeakers_Clicked(object? sender, EventArgs e)
        {
            if (_speakers == null || _speakers.Length == 0) return;
            _speakerSelectedIndex = (_speakerSelectedIndex + 1) % _speakers.Length;
            btSpeakers.Text = _speakers[_speakerSelectedIndex].DisplayName;
        }

        /// <summary>
        /// Build the source half of the graph (camera + mic). When connect=true, also wires
        /// straight into the renderer/audio-output for preview-only mode.
        /// </summary>
        private async Task ConfigurePreviewAsync(bool connect)
        {
#if !MOBILE
            var speaker = _speakers.FirstOrDefault(d => d.DisplayName == btSpeakers.Text)
                          ?? _speakers.FirstOrDefault();
            if (speaker == null)
            {
                await DisplayAlert("Error", "No audio output device available", "OK");
                return;
            }
            _audioOutput = new AudioRendererBlock(speaker) { IsSync = false };
#endif

            VideoCaptureDeviceSourceSettings videoSourceSettings = null;
            var deviceName = btCamera.Text;
            if (!string.IsNullOrEmpty(deviceName))
            {
                var device = _cameras.FirstOrDefault(x => x.DisplayName == deviceName);
                if (device != null)
                {
                    var formatItem = device.GetVideoFormatAndFrameRate(1280, 720, out var frameRate);
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

            if (videoSourceSettings == null)
            {
                await DisplayAlert("Error", "Unable to configure camera settings", "OK");
                return;
            }

            _videoSource = new SystemVideoSourceBlock(videoSourceSettings);

            IAudioCaptureDeviceSourceSettings audioSourceSettings = null;
            deviceName = btMic.Text;
            if (!string.IsNullOrEmpty(deviceName))
            {
                var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                    .FirstOrDefault(x => x.DisplayName == deviceName);
                if (device != null)
                {
                    var formatItem = device.GetDefaultFormat();
                    audioSourceSettings = device.CreateSourceSettings(formatItem);
                }
            }

            if (audioSourceSettings == null)
            {
                await DisplayAlert("Error", "Unable to configure microphone — pick an input device or check permissions.", "OK");
                return;
            }

            _audioSource = new SystemAudioSourceBlock(audioSourceSettings);

            if (connect)
            {
                _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);
#if !MOBILE
                _pipeline.Connect(_audioSource.Output, _audioOutput.Input);
#endif
            }
        }

        /// <summary>
        /// Generate a writable per-OS filename for the recorded MP4. iOS / Android scoped storage
        /// rules differ from desktop, so this branches per TFM.
        /// </summary>
        private string GenerateFilename()
        {
            DateTime now = DateTime.Now;
#if __ANDROID__
            string filename;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                var context = Android.App.Application.Context;
                var moviesDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryMovies);
                if (moviesDir != null && !moviesDir.Exists()) moviesDir.Mkdirs();
                filename = Path.Combine(moviesDir?.AbsolutePath ?? context.FilesDir.AbsolutePath,
                    $"visioforge_{now.Hour}_{now.Minute}_{now.Second}.mp4");
            }
            else
            {
                var dcimDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
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
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#endif
            return filename;
        }

        /// <summary>
        /// Add encoder + MP4 sink branches via tees so preview keeps running while recording.
        /// </summary>
        private void ConfigureCapture()
        {
            _videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);
            _audioTee = new TeeBlock(2, MediaBlockPadMediaType.Audio);

            _pipeline.Connect(_videoSource.Output, _videoTee.Input);
            _pipeline.Connect(_audioSource.Output, _audioTee.Input);

            _pipeline.Connect(_videoTee.Outputs[0], _videoRenderer.Input);
#if !MOBILE
            _pipeline.Connect(_audioTee.Outputs[0], _audioOutput.Input);
#endif

            _videoEncoder = new H264EncoderBlock(H264EncoderBlock.GetDefaultSettings());
            _pipeline.Connect(_videoTee.Outputs[1], _videoEncoder.Input);

            _audioEncoder = new AACEncoderBlock(AACEncoderBlock.GetDefaultSettings());
            _pipeline.Connect(_audioTee.Outputs[1], _audioEncoder.Input);

            var filename = GenerateFilename();
            var sinkSettings = new MP4SinkSettings(filename);
            _mp4Sink = new MP4SinkBlock(sinkSettings);

            _pipeline.Connect(_videoEncoder.Output, _mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));
            _pipeline.Connect(_audioEncoder.Output, _mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));
        }

        private async void btStartPreview_Clicked(object? sender, EventArgs e)
        {
            try
            {
                await StopAllAsync();
                CreateEngine();

                if (_pipeline.State == PlaybackState.Free)
                {
                    await ConfigurePreviewAsync(true);
                    await _pipeline.StartAsync();
                    btStartPreview.Text = "PAUSE";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting preview: {ex.Message}");
                await DisplayAlert("Error", $"Failed to start preview: {ex.Message}", "OK");
                btStartPreview.Text = "PREVIEW";
            }
        }

        private async void btStartCapture_Clicked(object? sender, EventArgs e)
        {
            try
            {
                await StopAllAsync();
                CreateEngine();

                if (_pipeline.State == PlaybackState.Free)
                {
                    await ConfigurePreviewAsync(false);
                    ConfigureCapture();
                    await _pipeline.StartAsync();
                    btStartCapture.Text = "PAUSE";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting capture: {ex.Message}");
                await DisplayAlert("Error", $"Failed to start capture: {ex.Message}", "OK");
                btStartCapture.Text = "CAPTURE";
            }
        }
    }
}
