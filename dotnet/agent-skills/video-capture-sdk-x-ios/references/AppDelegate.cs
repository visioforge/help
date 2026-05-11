using Photos;
using System.Diagnostics;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.UI.Apple;
using VisioForge.Core.VideoCaptureX;

namespace SimpleVideoCaptureX;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    private VideoCaptureCoreX _player;
    private string _filename;
    private int _cameraIndex = 1;
    private VideoCaptureDeviceInfo[] _cameras;
    private UIView _videoView;
    private UIButton _btSelectCamera;
    private UIButton _btStartCapture;
    private bool _isCapturing;
    private bool _isFrontCamera = true;

    public override UIWindow Window { get; set; }

    /// <summary>
    /// Build the VideoCaptureCoreX engine, wire events, pick a camera, and queue an
    /// MP4 output (autostart=false so the user can toggle Start/Stop manually).
    /// </summary>
    private async Task CreateEngineAsync()
    {
        try
        {
            // The renderer binds to _videoView via the cross-platform IVideoView
            // contract — VisioForge.Core.UI.Apple.VideoView implements it. If the
            // cast returns null the preview will silently stay black.
            _player = new VideoCaptureCoreX(_videoView as IVideoView);

            // VideoCaptureCoreX defaults Video_Play to false — without this the pipeline
            // runs (capture session is up, camera LED lights) but the video tee is never
            // connected to the renderer bound to _videoView, so preview stays black.
            _player.Video_Play = true;
            _player.OnError += PlayerOnError;
            _player.OnOutputStopped += async (sender, e) =>
            {
                // Async delay instead of Thread.Sleep — SDK rule forbids Thread.Sleep, and
                // the event may fire on either the main thread (freezing UI for 500 ms) or
                // on a GStreamer bus worker (blocking the bus and risking missed messages).
                // Wrap in try/catch because this handler is async void.
                try
                {
                    if (!string.IsNullOrEmpty(_filename))
                    {
                        await Task.Delay(500);
                        InvokeOnMainThread(() =>
                        {
                            SaveVideoToPhotoLibrary(_filename);
                            _filename = null;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"OnOutputStopped handler failed: {ex.Message}");
                }
            };

            if (_cameras == null)
            {
                _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
            }

            if (_cameras.Length == 0)
            {
                // Simulator hits this branch — there is no camera device. Test on hardware.
                Debug.WriteLine("No video sources found");
                return;
            }

            if (_cameraIndex >= _cameras.Length)
            {
                _cameraIndex = 0;
            }

            VideoCaptureDeviceSourceSettings videoSourceSettings = null;
            var device = _cameras[_cameraIndex];
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

            if (videoSourceSettings == null)
            {
                Debug.WriteLine("Unable to configure camera settings");
                return;
            }

            _player.Video_Source = videoSourceSettings;
            _player.Outputs_Clear();

            // VideoToolbox-backed H.264 — hardware-accelerated, no extra redist.
            var videoEncoder = new AppleMediaH264EncoderSettings();
            // AVENCAAC is gst-libav-backed and isn't bundled in VisioForge.CrossPlatform.Core.iOS,
            // so passing an audio encoder here would fail at pipeline link time. Use null for
            // video-only recording, or switch to VOAACEncoderSettings (portable) when audio is needed.
            GenerateFilename();
            var mp4Output = new MP4Output(_filename, videoEncoder, null);
            _player.Outputs_Add(mp4Output, autostart: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void RequestPhotoLibraryPermissions(Action<PHAuthorizationStatus> completionHandler)
    {
        PHPhotoLibrary.RequestAuthorization(status => completionHandler(status));
    }

    private async Task DestroyEngineAsync()
    {
        if (_player != null)
        {
            _player.OnError -= PlayerOnError;
            await _player.DisposeAsync();
            _player = null;
        }
    }

    private void PlayerOnError(object sender, ErrorsEventArgs e) => Debug.WriteLine(e.Message);

    /// <summary>
    /// Bottom control bar — shutter (Start/Stop record) + camera-flip button.
    /// </summary>
    private void AddButtons(UIView parent)
    {
        var controlBar = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromWhiteAlpha(0f, 0.35f)
        };
        parent.AddSubview(controlBar);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            controlBar.LeadingAnchor.ConstraintEqualTo(parent.LeadingAnchor),
            controlBar.TrailingAnchor.ConstraintEqualTo(parent.TrailingAnchor),
            controlBar.BottomAnchor.ConstraintEqualTo(parent.BottomAnchor),
            controlBar.HeightAnchor.ConstraintEqualTo(120f)
        });

        _btStartCapture = new UIButton(UIButtonType.Custom)
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.Clear
        };
        _btStartCapture.Layer.CornerRadius = 34f;
        _btStartCapture.Layer.BorderWidth = 4f;
        _btStartCapture.Layer.BorderColor = UIColor.White.CGColor;
        UpdateCaptureButtonAppearance();

        _btStartCapture.TouchUpInside += async (sender, e) =>
        {
            if (_player == null) return;

            // Optimistic UI update + rollback on throw — without try/catch around the
            // body, an SDK exception (out-of-disk, codec error) escapes to AppDomain
            // and terminates the process; without rollback the red button shows the
            // wrong state.
            _btStartCapture.Enabled = false;
            var wasCapturing = _isCapturing;
            try
            {
                if (wasCapturing)
                {
                    _isCapturing = false;
                    UpdateCaptureButtonAppearance();
                    await _player.StopCaptureAsync(0);
                }
                else
                {
                    GenerateFilename();
                    _isCapturing = true;
                    UpdateCaptureButtonAppearance();
                    await _player.StartCaptureAsync(0, _filename);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Capture toggle failed: {ex.Message}");
                _isCapturing = wasCapturing;
                UpdateCaptureButtonAppearance();
            }
            finally
            {
                _btStartCapture.Enabled = true;
            }
        };

        controlBar.AddSubview(_btStartCapture);
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _btStartCapture.CenterXAnchor.ConstraintEqualTo(controlBar.CenterXAnchor),
            _btStartCapture.CenterYAnchor.ConstraintEqualTo(controlBar.SafeAreaLayoutGuide.BottomAnchor, -44f),
            _btStartCapture.WidthAnchor.ConstraintEqualTo(68f),
            _btStartCapture.HeightAnchor.ConstraintEqualTo(68f)
        });

        var flipImage = UIImage.GetSystemImage(
            "arrow.triangle.2.circlepath.camera.fill",
            UIImageSymbolConfiguration.Create(26f, UIImageSymbolWeight.Regular));

        _btSelectCamera = new UIButton(UIButtonType.System)
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            TintColor = UIColor.White
        };
        _btSelectCamera.SetImage(flipImage, UIControlState.Normal);
        _btSelectCamera.TouchUpInside += async (sender, e) =>
        {
            // Disable the button while a flip is in flight so rapid double-taps don't
            // interleave two Stop/Start sequences against the same _player instance.
            _btSelectCamera.Enabled = false;
            try
            {
                if (_player != null)
                {
                    await StopCamera();
                }

                _isFrontCamera = !_isFrontCamera;

                if (_cameras == null || _cameras.Length == 0)
                {
                    _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
                }
                _cameraIndex = FindCameraIndex(_cameras, preferFront: _isFrontCamera);

                _isCapturing = false;
                UpdateCaptureButtonAppearance();
                await StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera flip failed: {ex.Message}");
            }
            finally
            {
                _btSelectCamera.Enabled = true;
            }
        };

        controlBar.AddSubview(_btSelectCamera);
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            _btSelectCamera.CenterYAnchor.ConstraintEqualTo(_btStartCapture.CenterYAnchor),
            _btSelectCamera.LeadingAnchor.ConstraintEqualTo(_btStartCapture.TrailingAnchor, 48f),
            _btSelectCamera.WidthAnchor.ConstraintEqualTo(44f),
            _btSelectCamera.HeightAnchor.ConstraintEqualTo(44f)
        });
    }

    /// <summary>
    /// Resolve front/back preference to an actual index by querying VideoCaptureDeviceFacing,
    /// not hard-coded 0/1 — iPads without a rear camera and Multi-Cam rigs enumerate in a
    /// different order. Falls back to the opposite-facing camera if the preferred one is
    /// missing, then to the first device.
    /// </summary>
    private static int FindCameraIndex(VideoCaptureDeviceInfo[] cameras, bool preferFront)
    {
        if (cameras == null || cameras.Length == 0) return 0;

        var preferred = preferFront ? VideoCaptureDeviceFacing.Front : VideoCaptureDeviceFacing.Back;
        for (var i = 0; i < cameras.Length; i++)
            if (cameras[i].Facing == preferred) return i;

        var fallback = preferFront ? VideoCaptureDeviceFacing.Back : VideoCaptureDeviceFacing.Front;
        for (var i = 0; i < cameras.Length; i++)
            if (cameras[i].Facing == fallback) return i;

        return 0;
    }

    private void UpdateCaptureButtonAppearance()
    {
        // ToArray() snapshot — iterating Subviews while calling RemoveFromSuperview mutates
        // the underlying NSArray during enumeration. Dispose so the managed UIView peer
        // releases its CALayer + auto-layout constraints right away instead of waiting for GC.
        foreach (var sub in _btStartCapture.Subviews.ToArray())
        {
            if (sub.Tag == 99)
            {
                sub.RemoveFromSuperview();
                sub.Dispose();
            }
        }

        var inner = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            UserInteractionEnabled = false,
            Tag = 99,
            BackgroundColor = UIColor.SystemRed
        };
        inner.Layer.CornerRadius = _isCapturing ? 6f : 26f;

        _btStartCapture.AddSubview(inner);
        NSLayoutConstraint.ActivateConstraints(new[]
        {
            inner.CenterXAnchor.ConstraintEqualTo(_btStartCapture.CenterXAnchor),
            inner.CenterYAnchor.ConstraintEqualTo(_btStartCapture.CenterYAnchor),
            inner.WidthAnchor.ConstraintEqualTo(_isCapturing ? 28f : 52f),
            inner.HeightAnchor.ConstraintEqualTo(_isCapturing ? 28f : 52f)
        });
    }

    private void SaveVideoToPhotoLibrary(string filePath)
    {
        PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
        {
            var creationRequest = PHAssetCreationRequest.CreationRequestForAsset();
            creationRequest.AddResource(PHAssetResourceType.Video, NSUrl.FromFilename(filePath), null);
        }, (success, error) =>
        {
            Debug.WriteLine(success
                ? "Video saved to photo library"
                : $"Photo library save failed: {error?.LocalizedDescription}");
        });
    }

    private async Task StopCamera()
    {
        if (_player == null) return;

        if (_player.IsCaptureStarted(0))
        {
            await _player.StopCaptureAsync(0);
        }
        await _player.StopAsync();
        await DestroyEngineAsync();
    }

    private async Task StartAsync()
    {
        await StopCamera();
        await CreateEngineAsync();
        await _player.StartAsync();
    }

    private void CreateVideoView(UIView view)
    {
        // VisioForge.Core.UI.Apple.VideoView is the iOS-specific UIView subclass that
        // implements IVideoView. The renderer's Metal layer attaches here.
        _videoView = new VideoView(view.Bounds)
        {
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
            BackgroundColor = UIColor.Black,
            DisableAspectRatioResize = true
        };
        view.AddSubview(_videoView);
    }

    private void GenerateFilename()
    {
        // App-sandbox Library directory — writable, not iCloud-backed, survives app updates.
        // Documents would also work; pick Library to keep recordings out of the user-visible
        // file picker until SaveVideoToPhotoLibrary copies them into Photos.
        var libraryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library");
        if (!Directory.Exists(libraryPath))
        {
            Directory.CreateDirectory(libraryPath);
        }

        _filename = Path.Combine(libraryPath, $"video_{DateTime.Now.Ticks}.mp4");
    }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        Window = new UIWindow(UIScreen.MainScreen.Bounds);
        var vc = new UIViewController();

        // Install the VC into the window BEFORE building the subview hierarchy so
        // vc.View.Bounds is non-zero when VideoView/Metal initialises. Touching
        // vc.View on an orphan VC fires loadView with Bounds = CGRect.Empty —
        // Metal drawables created against a zero-sized layer don't gain a surface
        // even after AutoresizingMask later resizes the UIView, so the camera
        // records fine (LED on) but preview stays black.
        Window.RootViewController = vc;
        Window.MakeKeyAndVisible();
        vc.View.LayoutIfNeeded();

        CreateVideoView(vc.View);
        AddButtons(vc.View);

        RequestPhotoLibraryPermissions(_ => { /* user-prompt only; nothing to do here */ });

        // VisioForgeX.InitSDK() is synchronous on iOS (unlike WPF/MAUI's InitSDKAsync).
        // First call builds the GStreamer plugin-registry cache in the app sandbox
        // (~1-3 s on first launch); subsequent launches are instant. Must run before
        // any VideoCaptureCoreX construction or DeviceEnumerator query.
        VisioForgeX.InitSDK();

        // Async-void lambda passed to InvokeOnMainThread — try/catch is mandatory.
        // Without it, permission denial / missing-format / StartAsync throws escape
        // to UIKit's runloop and terminate the process.
        InvokeOnMainThread(async () =>
        {
            try
            {
                await StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Initial capture start failed: {ex}");
            }
        });

        return true;
    }
}
