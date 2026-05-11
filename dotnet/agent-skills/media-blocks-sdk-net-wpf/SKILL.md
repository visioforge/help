---
name: media-blocks-sdk-net-wpf
description: Integrate VisioForge Media Blocks SDK .NET into a Windows WPF application. Covers the graph-based pipeline model (MediaBlocksPipeline, source/sink/transform blocks), the single NuGet package, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when building custom media pipelines (capture, transcode, mix, stream, record) on a WPF app — for simpler webcam-only capture, use video-capture-sdk-net-wpf instead.
---

# Media Blocks SDK .NET — WPF integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a Windows WPF application. Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `MP4SinkBlock`, `VideoRendererBlock`, `TeeBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `await pipeline.StartAsync()`. Compared to the higher-level Video Capture SDK (a single `VideoCaptureCore` god-object), Media Blocks gives you full control over the topology — splitting streams with tees, mixing sources, transcoding without preview, swapping sinks at runtime — at the cost of having to wire every edge yourself.

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Building a **custom pipeline** that the high-level capture/playback SDKs don't expose: split-with-tee preview + record, multi-source mix, transcode without preview, dynamic source switch, stream + record with separate encoders.
- Capturing → encoding → muxing to MP4 / MPEG-TS / WebM with explicit control over each stage.
- Network streaming sinks (RTSP server, SRT, RIST, NDI, WebRTC WHIP, YouTube/Facebook RTMP) wired into a custom graph.
- Pipelines you want to inspect — `pipeline.GetDiagramAsImage()` returns a SkiaSharp bitmap of the live graph.

## When NOT to use this skill

- **Plain webcam capture** (preview + record to MP4, no custom topology): `video-capture-sdk-net-wpf` is dramatically less code.
- **Playback only** (play a file or network stream, no capture / no custom graph): `media-player-sdk-net-wpf`.
- **WinForms host instead of WPF**: same SDK, different UI shell → `media-blocks-sdk-net-winforms`.
- **Cross-platform** (macOS, iOS, Android, Linux, browser): same SDK family, different host → `media-blocks-sdk-net-{maui,avalonia,uno,android,ios,macos}`. The block API is identical across platforms; only the `VideoView` and the project SDK differ.

## Project setup

### Target framework

Media Blocks SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. Unlike Video Capture SDK, the official Media Blocks WPF samples use the plain **`Microsoft.NET.Sdk`** (not `Microsoft.NET.Sdk.WindowsDesktop`) with `<UseWPF>true</UseWPF>` — this is intentional and required for the cross-platform `VisioForge.Core` reference graph to resolve correctly. Don't switch to `WindowsDesktop`.

### NuGet packages

Three packages are required for a Windows WPF capture-and-record pipeline — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/encoders). Unlike Video Capture SDK, the redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

The redist version (`2026.4.29` here) tracks the underlying GStreamer/libav rebuild cadence and lags the wrapper version (`2026.5.4`) on purpose — pin both to the values shipped in the upstream sample's csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper. Mismatches between wrapper and redist in either direction are undefined behaviour and surface as `DllNotFoundException` or `Element 'X' not found` errors at pipeline start.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Capture Demo (`_DEMOS/Media Blocks SDK/WPF/CSharp/Simple Capture Demo/`). Changes vs upstream: kept the multi-target framework variant (`net10.0-windows` shown); demo-only properties are unchanged. The bundled file builds standalone against the public NuGet packages.

### Project platform

Media Blocks WPF samples use `<PlatformTarget>x64</PlatformTarget>` — this differs from Video Capture SDK (which uses AnyCPU). The reason is the redist packages are split per-architecture (`VisioForge.CrossPlatform.Core.Windows.x64` vs `.x86`); referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous. To support both architectures from a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop the `<PlatformTarget>` line. The SDK is Windows-only on this host platform; for macOS / iOS / Android / Linux you switch to a different redist package family and a different UI host (Avalonia/MAUI/Uno).

## Pipeline model

This is the core mental shift from Video Capture SDK. There is no `Core.Start()` / `Core.Stop()` god-object — instead you build a directed graph of blocks and run it.

The five concepts you need:

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple pipelines per process are fine.
2. **Source blocks** (`SystemVideoSourceBlock`, `SystemAudioSourceBlock`, `RTSPSourceBlock`, `UniversalSourceBlock` for files, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `VPXEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, `AudioMixerBlock`, …) — accept on input pads, produce on output pads.
4. **Sink blocks** — terminate the graph. **Renderer sinks** drive the UI / speakers (`VideoRendererBlock` bound to a `VideoView`, `AudioRendererBlock` bound to a system audio output). **File / network sinks** write/transmit (`MP4SinkBlock`, `MPEGTSSinkBlock`, `WebMSinkBlock`, `RTSPServerBlock`, `SRTMPEGTSSinkBlock`, …). Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `(sink as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video)` to create the video input pad, again for audio.
5. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. For tees and dynamic-input muxers you address `block.Outputs[i]` / the pad created by `CreateNewInput`. Connections must be made before `StartAsync`; reconnecting at runtime is supported by specific blocks (live-source-switch, bridge) but not the general case.

Topology example (Simple Capture Demo, video preview + MP4 record):

```text
SystemVideoSourceBlock  --video-->  TeeBlock(video)  --[0]-->  VideoRendererBlock (VideoView1)
                                                     --[1]-->  H264EncoderBlock --> MP4SinkBlock(video pad)
SystemAudioSourceBlock  --audio-->  TeeBlock(audio)  --[0]-->  AudioRendererBlock (speakers)
                                                     --[1]-->  AACEncoderBlock  --> MP4SinkBlock(audio pad)
```

`MP4SinkBlock` is the same instance on both audio and video paths — its dynamic inputs collect the streams to mux.

Engine boot is required before any pipeline construction or device enumeration: `await VisioForgeX.InitSDKAsync()` once at app startup, `VisioForgeX.DestroySDK()` once at shutdown. The first `InitSDKAsync` call on a fresh machine builds a plugin-registry cache (~2-5 s); subsequent launches are instant.

## License registration

Call `await pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += Pipeline_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _pipeline.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-pipeline apps every `MediaBlocksPipeline` instance needs its own call before its `StartAsync`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

The bundled `references/MainWindow.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `Window_Loaded` right after `_pipeline = new MediaBlocksPipeline();` and before `await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();`.

## Hello-World pipeline

Minimum viable preview-only pipeline — a self-contained `MainWindow` you can drop into a fresh WPF project. Replace `YourApp` with your project's `<RootNamespace>` from the csproj:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="MainWindow" Height="450" Width="800"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid>
        <WPF:VideoView x:Name="VideoView1" Background="Black" />
        <Button Content="Start" Width="80" Height="30"
                HorizontalAlignment="Left" VerticalAlignment="Top" Margin="10"
                Click="StartButton_Click"/>
    </Grid>
</Window>
```

Code-behind:

```csharp
// MainWindow.xaml.cs
using System;
using System.Linq;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

namespace YourApp
{
    public partial class MainWindow : Window
    {
        private MediaBlocksPipeline _pipeline;
        private SystemVideoSourceBlock _videoSource;
        private VideoRendererBlock _videoRenderer;

        public MainWindow() => InitializeComponent();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Engine boot is mandatory before any block construction or device enumeration.
            // First run on a fresh machine takes 2-5 s while the plugin registry is built;
            // subsequent runs are instant.
            await VisioForgeX.InitSDKAsync();

            _pipeline = new MediaBlocksPipeline();
            _pipeline.OnError += (s, args) =>
                Dispatcher.Invoke(() => MessageBox.Show(args.Message));

            // For a purchased licence, add these two lines here:
            //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
            //   await _pipeline.SetLicenseCertificateAsync(cert);
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // async-void event handlers must catch — an exception otherwise escapes to
            // AppDomain.UnhandledException and silently terminates the app. Common triggers
            // on first run: trial expired, missing native DLLs, no devices, COM init failure.
            try
            {
                var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
                if (devices.Length == 0)
                {
                    MessageBox.Show("No video capture devices found.");
                    return;
                }

                // Let the SDK pick the device's default format. Setting Format = device.VideoFormats[0]
                // breaks on virtual cameras / IP shims that enumerate with an empty format list.
                var settings = new VideoCaptureDeviceSourceSettings(devices[0]);
                _videoSource = new SystemVideoSourceBlock(settings);

                // VideoRendererBlock binds to the WPF VideoView at construction time.
                // IsSync = false is the standard preview setting — frames render as fast
                // as they arrive instead of being clock-throttled (which can stutter on
                // capture sources whose timestamps drift).
                _videoRenderer = new VideoRendererBlock(_pipeline, VideoView1) { IsSync = false };

                _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);

                await _pipeline.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Pipeline start failed: {ex.Message}");
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop → block-Dispose → pipeline-Dispose → DestroySDK is the correct
            // shutdown order. ClearBlocks/DisposeAsync on the pipeline does NOT
            // dispose blocks wired only via Connect — those remain owned by the
            // caller and leak native GStreamer resources unless we dispose them
            // explicitly. Skipping StopAsync before DisposeAsync leaks the
            // GStreamer worker threads; skipping DestroySDK leaks the plugin registry.
            if (_pipeline != null)
            {
                await _pipeline.StopAsync();

                _videoSource?.Dispose();
                _videoSource = null;
                _videoRenderer?.Dispose();
                _videoRenderer = null;

                await _pipeline.DisposeAsync();
                _pipeline = null;
            }

            VisioForgeX.DestroySDK();
        }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `MainWindow.xaml`) is the full upstream sample — it adds device-enumeration combo boxes, the audio path, the tee + encoder + muxer record path with MP4 / MPEG-TS / WebM options, recording-time display, and `pipeline.GetDiagramAsImage()` diagnostic export. Use it as a copy-paste starting template when you outgrow the snippet above.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*' " or "libgstreamer-*.dll not found"

**Cause**: missing redist NuGet (`VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Libav.Windows.x64.UPX`), or `<PlatformTarget>` doesn't match the redist's architecture (e.g. `<PlatformTarget>x86</PlatformTarget>` with the `.x64` redist), or the wrapper and redist versions drifted apart enough that the native ABI changed.

**Fix**: reference both redist packages from the "NuGet packages" section, set `<PlatformTarget>x64</PlatformTarget>` to match, and pin the redist version to the value used by the upstream sample for your wrapper version (do not blindly bump). For 32-bit deployment swap `.x64` for `.x86` in both redist names and set `<PlatformTarget>x86</PlatformTarget>`.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Every pipeline instance in the process needs its own call.

### 3. Pipeline `OnError` fires with "Element 'X' not found" / "no element 'h264parse'" / "Codec not found"

**Cause**: an encoder/muxer/sink block in the graph requires native plugins from a redist that isn't referenced. For example, `WebMSinkBlock` + `VPXEncoderBlock` + `VorbisEncoderBlock` need both core and libav redists; some H.264 hardware paths also need the libav redist.

**Fix**: ensure both `VisioForge.CrossPlatform.Core.Windows.x64` and `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` are referenced (the libav package is what most "missing element" errors trace back to). For specialised sinks (NDI, Decklink, WHIP) check the matching upstream sample's csproj for additional redist packages.

### 4. `await _pipeline.StartAsync()` deadlocks / preview is black on first frame

**Cause A — UI thread blocking**: calling `_pipeline.Stop()` (synchronous overload) or `Dispose()` from the UI thread while a renderer block is still pumping frames to a `VideoView` deadlocks because the renderer needs the UI thread to release the last frame. Always use the async forms.

**Cause B — VideoView has no HWND yet**: `VideoRendererBlock` was constructed and `pipeline.StartAsync` ran before the WPF `VideoView1` element had a valid HWND. Most often happens when start logic runs from a constructor or `Loaded`-handler too early.

**Fix**: use `await _pipeline.StopAsync()` and `await _pipeline.DisposeAsync()` everywhere, defer pipeline start to `ContentRendered` or a button click (the upstream sample uses an explicit "Start" button — copy that pattern), and always create `VideoRendererBlock` *after* the `VideoView` is realised in the visual tree.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run on a fresh machine takes 2-5 s during `VisioForgeX.InitSDKAsync()` (registry build); second run is instant.
- [ ] Webcam preview appears within ~1 s after clicking Start on a machine with a real or virtual webcam.
- [ ] Stopping and restarting the pipeline does not leak — always `await _pipeline.StopAsync()` then `_pipeline.ClearBlocks()` or `await _pipeline.DisposeAsync()` before reuse.
- [ ] On clean shutdown, `Window_Closing` runs `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] If recording to a file sink: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `DisposeAsync`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaBlocksPipeline` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="MainWindow.xaml"`).
- `references/MainWindow.xaml` — XAML for the main window. Declares `xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and hosts `<WPF:VideoView Name="VideoView1" />` along with source-/format-/output-selection controls.
- `references/MainWindow.xaml.cs` — full code-behind with device enumeration, the audio path, tee + encoder + MP4/MPEG-TS/WebM muxer record path, recording-time display, and `OnError` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-winforms` — same SDK on WinForms.
    - `media-blocks-sdk-net-avalonia` — same SDK on Avalonia (cross-platform).
    - `media-blocks-sdk-net-maui` — same SDK on MAUI (Windows + macOS + iOS + Android).
    - `media-blocks-sdk-net-uno` — same SDK on Uno (Windows + macOS + iOS + Android + Linux + browser).
    - `video-capture-sdk-net-wpf` — high-level capture-and-record API on WPF; use this if you don't need a custom pipeline.
