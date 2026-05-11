---
name: media-blocks-sdk-net-blazor
description: Integrate VisioForge Media Blocks SDK into a Blazor Server application. Covers the graph-based pipeline model running server-side (one pipeline per logical scenario, owned by a singleton DI service), the single .NET wrapper plus per-OS native redist NuGet packages, license registration, the strict no-Blazor-WebAssembly constraint, and the most common Blazor pitfalls (DI lifetime mismatches, Razor circuit disposal vs pipeline disposal, capture-device permissions on the server host, server-side codec licensing, trial-period expiry / unlicensed build). Use for browser-based UIs that drive a server-side media pipeline (RTSP server, file recording, transcoding, broadcasting) — for a desktop UI use media-blocks-sdk-net-{wpf,winforms}.
---

# Media Blocks SDK .NET — Blazor Server integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a Blazor **Server** application. Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `RTSPServerBlock`, `MP4SinkBlock`, `UniversalSourceBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `await pipeline.StartAsync()`. The Razor UI is just a remote control: every pipeline runs **on the server process** (the same machine hosting the ASP.NET Core app); the browser only sees status text, button clicks, and form posts. There is no `VideoView` in Blazor — to expose the live media to the user you publish it from the server (RTSP server, MP4 file, RTMP push, HLS, WebRTC WHIP, …) and let the browser consume that URL with a separate player or `<video>` element.

Pinned NuGet version: **`2026.5.4`** (matches the [official RTSP Webcam Blazor Server demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Blazor)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Building a **browser admin UI** for a server-side capture/transcode/streaming pipeline (start/stop a webcam-to-RTSP feed, kick off an RTSP-pull-and-record job, manage multiple concurrent streams).
- The compute and the capture devices live on the server: USB webcam plugged into the server box, an IP/RTSP camera reachable from the server LAN, an SDI capture card, or files on server-local storage.
- Any scenario where the high-level Video Capture SDK isn't enough — split-with-tee preview-recording, multi-source mix, transcode-only, dynamic source switch — and the orchestrator UI happens to be Blazor.

## When NOT to use this skill

- **Blazor WebAssembly** (`Microsoft.NET.Sdk.BlazorWebAssembly`, browser-side `.wasm` execution): **not supported**. Media Blocks calls native code (GStreamer-equivalent runtime, libav, hardware capture APIs) through `VisioForge.CrossPlatform.*` redist packages — none of these run in the browser sandbox. There is no WebAssembly redist and no plan to ship one. If you need a media pipeline in Blazor WASM you have to call out to a separate server-side service (REST/SignalR/gRPC) that runs Media Blocks; that server-side service is the project this skill applies to.
- **Capture device on the user's machine** (laptop's webcam, the user's mic): the server-side `SystemVideoSourceBlock` enumerates devices on the *server host*, not on the connected client. Users can't share their local webcam through this. For client-side capture use `getUserMedia` + WebRTC and ingest the WebRTC stream into Media Blocks server-side via the WebRTC source block.
- **Plain webcam capture and record** with no custom topology, on a desktop UI: `video-capture-sdk-net-wpf` / `video-capture-sdk-net-winforms` is dramatically less code.
- **Desktop UI** (WPF, WinForms, Avalonia, MAUI): same SDK, different host → `media-blocks-sdk-net-wpf`, `media-blocks-sdk-net-winforms`, `media-blocks-sdk-net-avalonia`, `media-blocks-sdk-net-maui`.

## Project setup

### Project SDK and target framework

The csproj uses **`Microsoft.NET.Sdk.Web`** (ASP.NET Core), not `Microsoft.NET.Sdk` or `Microsoft.NET.Sdk.WindowsDesktop`. The official Blazor sample multi-targets per OS so the same project builds on Windows, macOS, and Linux:

```xml
<PropertyGroup>
  <TargetFramework Condition="$([MSBuild]::IsOsPlatform('OSX'))">net10.0-macos</TargetFramework>
  <TargetFramework Condition="$([MSBuild]::IsOsPlatform('Windows'))">net10.0-windows</TargetFramework>
  <TargetFramework Condition="$([MSBuild]::IsOsPlatform('Linux'))">net10.0</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

If you only deploy on one OS, drop the conditions and pin a single TFM. The Windows TFM is `net*.0-windows` (not `-windows10.0.x.x`) — Media Blocks does not need the Windows App SDK, and `net10.0-windows` resolves the Windows-specific capture APIs that the redist binds to.

### NuGet packages

The .NET wrapper is a single package; the native redist is per-OS and **not transitive** — you must reference it explicitly under an OS condition:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
</ItemGroup>
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-macos'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
</ItemGroup>
```

The redist version (`2026.4.29` here) tracks the underlying GStreamer/libav rebuild cadence and lags the wrapper version (`2026.5.4`) on purpose — pin both to the values shipped in the upstream sample's csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper. Mismatches surface as `DllNotFoundException` or `Element 'X' not found` errors at pipeline start.

For Linux deployment add the matching `VisioForge.CrossPlatform.Core.Linux.x64` package; for ARM64 hosts (Apple Silicon, Raspberry Pi, ARM Linux) swap `.x64` for the `.arm64` variant. See `references/Sample.csproj` for a complete working file.

## Pipeline model

The five concepts (identical to all other Media Blocks hosts):

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple pipelines per process are fine.
2. **Source blocks** (`SystemVideoSourceBlock`, `SystemAudioSourceBlock`, `RTSPSourceBlock`, `UniversalSourceBlock` for files, `WebRTCSourceBlock`, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, …) — accept on input pads, produce on output pads.
4. **Sink blocks** terminate the graph. In Blazor Server you almost always pick a **publishing sink** (`RTSPServerBlock`, `MP4SinkBlock`, `WebMSinkBlock`, `MPEGTSSinkBlock`, `RTMPSinkBlock`, `HLSSinkBlock`, `WebRTCWHIPSinkBlock`, …) instead of a renderer sink — there is no on-screen `VideoRendererBlock` because there is no Win32 HWND to render into. `VideoRendererBlock` exists in the SDK but only makes sense in WPF/WinForms/MAUI hosts that own a `VideoView`.
5. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `(sink as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video)` then again for audio.

Topology for the bundled RTSP webcam sample:

```text
SystemVideoSourceBlock  --video-->  RTSPServerBlock (encodes with H.264 internally, serves rtsp://host:8554/...)
```

Engine boot is required before any pipeline construction or device enumeration: `VisioForgeX.InitSDK()` (or `await VisioForgeX.InitSDKAsync()`) once at app startup, `VisioForgeX.DestroySDK()` once at shutdown. The first call on a fresh machine builds a plugin-registry cache (~2-5 s); subsequent launches are instant.

## Wiring it into ASP.NET Core / Blazor Server

The pattern that works for every non-trivial Media Blocks Blazor app is **a singleton DI service that owns the SDK lifecycle and the active pipelines** — *not* a `MediaBlocksPipeline` instantiated in a Razor component's `@code`. Razor components have a per-circuit lifetime: every browser tab opens a SignalR circuit and gets its own component instance, and `Dispose` runs when the user closes the tab or the circuit times out. Putting a pipeline in component state ties native worker threads to a UI lifetime that can vanish at any moment.

`Program.cs`:

```csharp
using YourApp.Services;
using VisioForge.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// One VisioForgeService per process, holding InitSDK + active pipelines.
builder.Services.AddSingleton<VisioForgeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Initialize the SDK once, before any request can hit the page.
var visioForgeService = app.Services.GetRequiredService<VisioForgeService>();
await visioForgeService.InitializeAsync();

app.Run();
```

The Razor component injects the service and calls methods on it; component disposal does not stop the pipeline. See `references/Services/VisioForgeService.cs` for the full singleton shape (Init / GetAvailableCameras / StartStreamingAsync / StopStreamingAsync / `IAsyncDisposable.DisposeAsync`) and `references/Pages/Index.razor` for the Razor side.

## License registration

Call `await pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`. In the singleton-service pattern this means inside `StartStreamingAsync` (or wherever you `new MediaBlocksPipeline()`):

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += (s, args) => Console.WriteLine($"Pipeline error: {args.Message}");

var cert = await System.IO.File.ReadAllBytesAsync("path/to/your.vflicense");
await _pipeline.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — older `SetLicenseCertificate(string)` and `SetLicenseCertificate(Stream)` overloads were removed. For multi-pipeline apps every `MediaBlocksPipeline` instance needs its own call before its `StartAsync`. Where the bytes come from (env var, embedded resource, ASP.NET Core configuration, secrets manager, …) is your application's choice.

The bundled `references/Services/VisioForgeService.cs` runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `StartStreamingAsync` right after `_pipeline = new MediaBlocksPipeline();`.

## Common deployment failures

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'" at first pipeline construction

**Cause**: missing redist NuGet for the host OS, or `<PlatformTarget>` doesn't match the redist's architecture (e.g. `x86` build with the `x64` redist), or wrapper and redist versions drifted apart enough that the native ABI changed. Common on Linux/Docker deploys when the Windows redist condition matched on the dev machine but the deploy didn't pull the Linux redist.

**Fix**: reference the matching redist for every OS your csproj targets, set `<PlatformTarget>x64</PlatformTarget>` to match, and pin the redist version to the value used by the upstream sample for your wrapper version. For ARM64 hosts swap `.x64` for `.arm64` in the redist names.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on `StartAsync`

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync`. Every pipeline instance the singleton creates needs its own call.

### 3. `GetAvailableCameras()` returns an empty array on a Linux/Docker host

**Cause**: the server process can't see capture devices. On Linux, USB webcams need V4L2 kernel modules and the process needs read access to `/dev/video*`; in Docker the device isn't passed through unless you `--device=/dev/video0` (and similar for audio). On Windows server hosts, RDP sessions and Service-account contexts may not see USB devices the interactive logon sees. The SDK does **not** emulate or share the connected browser client's webcam — `getUserMedia` is a separate world.

**Fix**: enumerate devices on the server console (not over the Razor UI) to confirm the OS sees them; run the dev test on the same account/session the production service will run in; for Docker/Kubernetes pass the device(s) through explicitly. If the requirement is "the user's local webcam" you need a WebRTC-ingest pipeline, not `SystemVideoSourceBlock`.

### 4. Pipeline keeps running after the browser tab closes (or — opposite — pipeline dies when the tab closes)

**Cause**: pipeline was instantiated in component-scoped state instead of in a singleton service. Razor circuits are per-tab and per-connection; if the user closes the tab or loses Wi-Fi for the SignalR reconnect timeout, the component's `Dispose` runs and tears down the pipeline mid-stream — or, if you over-corrected and disposed nothing on circuit close, every accidental refresh leaks a fresh pipeline.

**Fix**: keep the `MediaBlocksPipeline` inside an `AddSingleton<VisioForgeService>` (or scoped to the user's account if you need per-user streams, but never to the Razor circuit). The component only calls `StartStreamingAsync` / `StopStreamingAsync` on the injected service. The component's `Dispose` does nothing pipeline-related — the singleton owns the lifetime, and the singleton is disposed by the DI container on app shutdown via `IAsyncDisposable.DisposeAsync` (which calls `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()`).

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh `Microsoft.NET.Sdk.Web` project (no missing-DLL warnings during build).
- [ ] First app start on a fresh machine takes 2-5 s during `VisioForgeService.InitializeAsync()` (registry build); subsequent starts are instant.
- [ ] `GetAvailableCameras()` returns a non-empty array on the **server host** (not via the browser); if it doesn't, the OS or the service account can't see the device — fix that before debugging the SDK.
- [ ] Starting a pipeline from the Razor UI returns within ~1 s and the published URL (`rtsp://host:port/...`, the file path, the RTMP target, …) is reachable from a separate client (VLC, FFmpeg, `<video>`).
- [ ] Closing the browser tab does **not** stop the active stream — the singleton keeps it alive until `StopStreamingAsync` is called or the app shuts down.
- [ ] On clean shutdown, `IAsyncDisposable.DisposeAsync` on the service runs `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` in that order. Verify by gracefully stopping the app (Ctrl-C in `dotnet run`, `systemctl stop`, container SIGTERM) and checking the output file is finalised correctly.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaBlocksPipeline` instance the service creates, before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy it into a fresh project folder, drop in a default `_Host.cshtml` / `_Imports.razor` / `Shared/MainLayout.razor` from a Blazor Server template, and `dotnet build` succeeds against the public NuGet packages:

- `references/Sample.csproj` — minimal working `Microsoft.NET.Sdk.Web` csproj, multi-OS targeted, version-pinned to the same NuGet release as the prose.
- `references/Program.cs` — ASP.NET Core entry point: registers `VisioForgeService` as a singleton, maps the Blazor hub, calls `InitializeAsync` before `app.Run()`.
- `references/App.razor` — standard Blazor router.
- `references/Pages/Index.razor` — single-page UI with camera dropdown, RTSP port input, Start/Stop buttons, and live status. The whole pipeline lifecycle is delegated to the injected `VisioForgeService`; the component itself holds no SDK state.
- `references/Services/VisioForgeService.cs` — the singleton DI service. Owns `VisioForgeX.InitSDK / DestroySDK`, the active `MediaBlocksPipeline`, and `IAsyncDisposable.DisposeAsync` cleanup. Builds a `SystemVideoSourceBlock → RTSPServerBlock` pipeline. Use as a copy-paste starting template; replace the topology inside `StartStreamingAsync` for your own scenario (file recording, RTMP push, multi-source mix, …) without touching the lifetime/DI scaffolding.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Blazor>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-wpf` — same SDK on WPF (desktop, has `VideoView` for in-process preview).
    - `media-blocks-sdk-net-winforms` — same SDK on WinForms.
    - `media-blocks-sdk-net-avalonia` — same SDK on Avalonia (cross-platform desktop).
    - `media-blocks-sdk-net-maui` — same SDK on MAUI (Windows + macOS + iOS + Android).
    - `video-capture-sdk-net-wpf` — high-level capture-and-record API on WPF; use this if you don't need a custom pipeline and a desktop UI is acceptable.
