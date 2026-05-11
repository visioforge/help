using System.Threading;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;

namespace RTSP_Webcam_Server.Services;

/// <summary>
/// Singleton-scoped VisioForge service. Owns the SDK lifecycle and the
/// MediaBlocksPipeline. Registered with AddSingleton in Program.cs and
/// initialized once at app startup.
/// </summary>
public class VisioForgeService : IAsyncDisposable
{
    private MediaBlocksPipeline? _pipeline;
    private RTSPServerBlock? _rtspBlock;
    private SystemVideoSourceBlock? _cameraSource;
    private bool _isInitialized;
    private bool _isStreaming;

    // Serializes init/start/stop so two concurrent Blazor circuits can't both
    // build a pipeline (the IsInitialized / IsStreaming checks are not atomic
    // with the state mutations that follow them).
    private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

    public bool IsInitialized => _isInitialized;
    public bool IsStreaming => _isStreaming;
    public string? RtspUrl { get; private set; }

    public async Task InitializeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_isInitialized) return;

            await Task.Run(() =>
            {
                VisioForgeX.InitSDK();
                _isInitialized = true;
            });
        }
        finally
        {
            _gate.Release();
        }
    }

    public VideoCaptureDeviceInfo[] GetAvailableCameras()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("VisioForge SDK is not initialized.");

        return DeviceEnumerator.Shared.VideoSources();
    }

    public async Task<bool> StartStreamingAsync(VideoCaptureDeviceInfo cameraInfo, int port = 8554)
    {
        await _gate.WaitAsync();
        try
        {
            return await StartStreamingCoreAsync(cameraInfo, port);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<bool> StartStreamingCoreAsync(VideoCaptureDeviceInfo cameraInfo, int port)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("VisioForge SDK is not initialized.");

        if (_isStreaming)
            return false;

        try
        {
            _pipeline = new MediaBlocksPipeline();

            // For a purchased licence, load the .vflicense bytes and call
            //   await _pipeline.SetLicenseCertificateAsync(certBytes);
            // before StartAsync. Without it the pipeline runs in 30-day trial mode.

            var videoSourceSettings = new VideoCaptureDeviceSourceSettings(cameraInfo);

            // GetHDVideoFormatAndFrameRate returns null when the device exposes no HD-class
            // format, so fall back to "HD or any" and then to the first available format.
            var formatInfo = cameraInfo.GetHDOrAnyVideoFormatAndFrameRate(out var frameRate);
            if (formatInfo == null && cameraInfo.VideoFormats != null && cameraInfo.VideoFormats.Count > 0)
            {
                formatInfo = cameraInfo.VideoFormats[0];
            }

            if (formatInfo == null)
            {
                throw new InvalidOperationException($"Camera '{cameraInfo.DisplayName}' exposes no usable video format.");
            }

            videoSourceSettings.Format = formatInfo.ToFormat();
            if (!frameRate.IsEmpty)
            {
                videoSourceSettings.Format.FrameRate = frameRate;
            }

            _cameraSource = new SystemVideoSourceBlock(videoSourceSettings);

            var rtspServerSettings = new RTSPServerSettings(H264EncoderBlock.GetDefaultSettings(), null)
            {
                Port = port,
            };

            _rtspBlock = new RTSPServerBlock(rtspServerSettings);
            RtspUrl = _rtspBlock.Settings.URL;

            _pipeline.Connect(_cameraSource.Output, _rtspBlock.Input);

            await _pipeline.StartAsync();

            _isStreaming = true;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting stream: {ex.Message}");
            await StopStreamingCoreAsync();
            return false;
        }
    }

    public async Task StopStreamingAsync()
    {
        await _gate.WaitAsync();
        try
        {
            await StopStreamingCoreAsync();
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task StopStreamingCoreAsync()
    {
        if (!_isStreaming) return;

        try
        {
            if (_pipeline != null)
            {
                await _pipeline.StopAsync();
                await _pipeline.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping stream: {ex.Message}");
        }
        finally
        {
            _pipeline = null;
            _cameraSource = null;
            _rtspBlock = null;
            RtspUrl = null;
            _isStreaming = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            await StopStreamingCoreAsync();

            if (_isInitialized)
            {
                VisioForgeX.DestroySDK();
                _isInitialized = false;
            }
        }
        finally
        {
            _gate.Release();
        }

        _gate.Dispose();
    }
}
