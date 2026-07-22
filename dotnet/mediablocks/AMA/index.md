---
title: AMD AMA Hardware Video Encoding & Decoding in C# .NET
description: Encode and decode H.264, HEVC, and AV1 on AMD Alveo accelerators with the AMA Video SDK in VisioForge Media Blocks SDK for .NET on Linux.
sidebar_label: AMA
tags:
  - Media Blocks SDK
  - .NET
  - Linux
  - Streaming
  - AV1
  - H.265
primary_api_classes:
  - AMAH264EncoderSettings
  - AMAHEVCEncoderSettings
  - AMAAV1EncoderSettings
  - AMAH264DecoderSettings
  - AMAHEVCDecoderSettings
  - AMAAV1DecoderSettings
  - AMAScalerBlock
  - AMAScalerSettings

---

# AMD AMA Blocks - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The **AMD AMA Video SDK** is AMD's hardware media-acceleration platform for AMD Alveo media accelerators. It ships a family of GStreamer elements (`ama_h264enc`, `ama_h265enc`, `ama_av1enc`, `ama_h264dec`, `ama_h265dec`, `ama_av1dec`, `ama_scaler`, `ama_upload`, `ama_download`) that run H.264, HEVC, and AV1 encoding, decoding, and scaling entirely on the accelerator. VisioForge Media Blocks SDK wraps this platform so you can offload video processing to AMD hardware from a standard C# pipeline. The AMA platform is distinct from AMD AMF (which drives Radeon GPUs) — AMA targets dedicated Alveo media accelerators such as the **Alveo MA35D**, the device currently supported.

**Key facts:**

- **Platform:** AMD AMA Video SDK on AMD Alveo media accelerators (currently the Alveo MA35D).
- **Codecs:** H.264, HEVC (H.265), and AV1 — hardware encode and decode.
- **Operating system:** Linux x86_64 only. There is no Windows or macOS AMA support.
- **Device selection:** the `Device` property picks the accelerator — `-1` (default) auto-selects, `0`/`1`/`2`… pin a specific card when several AMD devices are present.
- **Memory model:** encoding/decoding/scaling run in device memory; the SDK inserts `ama_upload` / `ama_download` automatically when data crosses to or from system memory.
- **Distribution:** VisioForge ships only the managed wrapper. The AMA runtime (kernel driver + GStreamer plugins) is installed separately from AMD; no AMD binaries are bundled with the SDK.

## Prerequisites

Before the AMA blocks can be used, the target machine must have:

- Linux x86_64 (Ubuntu, Alma/RHEL, or Debian per the AMD AMA SDK support matrix).
- An AMD Alveo media accelerator installed and provisioned (kernel driver, huge pages, IOMMU / Above-4G decoding enabled in the host firmware).
- The AMD AMA Video SDK installed, including its GStreamer plugins, so the `ama_*` elements are discoverable by GStreamer.

Every AMA settings and block class exposes a static `IsAvailable()` method that probes for the required `ama_*` elements. Call it before building a pipeline and fall back to a software or other-vendor encoder when it returns `false`:

```csharp
if (AMAH264EncoderSettings.IsAvailable())
{
    // AMA H.264 encoder is present — use hardware encoding.
}
```

## AMA H.264 encoder

Hardware H.264 encoder backed by the `ama_h264enc` element. Configure it with `AMAH264EncoderSettings` (implements `IH264EncoderSettings`) and pass it to a standard `H264EncoderBlock`.

### Block info

Name: H264EncoderBlock (with `AMAH264EncoderSettings`).

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | H264 | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

// AMD AMA (Alveo) hardware H.264 encoder. Device = -1 auto-selects the accelerator.
var h264Settings = new AMAH264EncoderSettings
{
    Device = -1,
    Bitrate = 6000, // Kbps
    RateControl = AMARateControl.CBR
};
var h264EncoderBlock = new H264EncoderBlock(h264Settings);
pipeline.Connect(fileSource.VideoOutput, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Linux (x86_64) with the AMD AMA Video SDK.

## AMA HEVC encoder

Hardware HEVC (H.265) encoder backed by the `ama_h265enc` element. Configure it with `AMAHEVCEncoderSettings` (implements `IHEVCEncoderSettings`) and pass it to a standard `HEVCEncoderBlock`.

### Block info

Name: HEVCEncoderBlock (with `AMAHEVCEncoderSettings`).

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | HEVC | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->HEVCEncoderBlock;
    HEVCEncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

var hevcSettings = new AMAHEVCEncoderSettings
{
    Device = -1,
    Bitrate = 6000, // Kbps
    TuneMetrics = AMATuneMetrics.VMAF
};
var hevcEncoderBlock = new HEVCEncoderBlock(hevcSettings);
pipeline.Connect(fileSource.VideoOutput, hevcEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(hevcEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Linux (x86_64) with the AMD AMA Video SDK.

## AMA AV1 encoder

Hardware AV1 encoder backed by the `ama_av1enc` element. Configure it with `AMAAV1EncoderSettings` (implements `IAV1EncoderSettings`) and pass it to a standard `AV1EncoderBlock`. AV1 adds a `DeviceType` property (`AMAAV1DeviceType`) that selects the AV1 engine.

### Block info

Name: AV1EncoderBlock (with `AMAAV1EncoderSettings`).

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | AV1 | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AV1EncoderBlock;
    AV1EncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

var av1Settings = new AMAAV1EncoderSettings
{
    Device = -1,
    DeviceType = AMAAV1DeviceType.Type1,
    Bitrate = 6000 // Kbps
};
var av1EncoderBlock = new AV1EncoderBlock(av1Settings);
pipeline.Connect(fileSource.VideoOutput, av1EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(av1EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Linux (x86_64) with the AMD AMA Video SDK.

## Common encoder settings

The three AMA encoder settings classes share the same core properties (AV1 additionally exposes `DeviceType`, and its QP range is 0–255 rather than 0–51):

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Device` | int | −1 | Accelerator to encode on. −1 auto-selects. |
| `Slice` | int | −1 | Sub-engine (slice) of the device. −1 = auto. |
| `Bitrate` | uint | 5000 | Target bitrate in Kbps. |
| `MaxBitrate` | int | −1 | Peak bitrate in Kbps for VBR/CVBR. −1 = auto. |
| `RateControl` | `AMARateControl` | Auto | Rate-control mode. |
| `QPMode` | `AMAQPMode` | Auto | QP control mode. |
| `QP` | int | −1 | Fixed QP in constant-QP mode. −1 = disabled. |
| `MinQP` / `MaxQP` | int | 0 / 51 (255 for AV1) | Allowed QP range. |
| `BFrames` | int | −1 | B-frames between P-frames. −1 = auto. |
| `GOPLength` | int | −1 | Maximum I-frame distance. −1 = auto. |
| `Tier` | `AMATier` | Auto | Encoding tier. |
| `TuneMetrics` | `AMATuneMetrics` | VQ | Objective quality metric to tune for. |
| `SpatialAQ` / `TemporalAQ` | bool | true | Adaptive-quantization toggles (with `*Gain` 0–255). |
| `LookaheadDepth` | int | −1 | Look-ahead depth. −1 = auto. |
| `LatencyMs` | int | −1 | Encoder latency target (ms). −1 = auto. |

## AMA decoders

Hardware decoders for H.264 (`ama_h264dec`), HEVC (`ama_h265dec`), and AV1 (`ama_av1dec`). Configure them with `AMAH264DecoderSettings` / `AMAHEVCDecoderSettings` / `AMAAV1DecoderSettings` (implementing `IH264/HEVC/AV1DecoderSettings`) and pass them to the corresponding decoder block. When a decoder feeds a system-memory consumer, the SDK appends `ama_download` automatically.

### Block info

Name: `H264DecoderBlock` / `HEVCDecoderBlock` / `AV1DecoderBlock` (with an AMA settings object).

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | Encoded video | 1 |
| Output video | Uncompressed video | 1 |

### Settings

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Device` | int | −1 | Accelerator to decode on. −1 auto-selects. |
| `LowLatency` | bool | false | Enable low-latency decoding. |
| `LatencyLogging` | bool | false | Log latency information to syslog. |
| `AllowDownscaling` | bool | false | Allow decoder-level downscaling. |

### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock-->UniversalDemuxBlock;
    UniversalDemuxBlock-->HEVCDecoderBlock;
    HEVCDecoderBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// AMD AMA (Alveo) hardware HEVC decoder.
var hevcDecoder = new HEVCDecoderBlock(new AMAHEVCDecoderSettings { Device = -1 });

var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), hevcDecoder.Input);
pipeline.Connect(hevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Platforms

Linux (x86_64) with the AMD AMA Video SDK.

## AMA scaler

The `AMAScalerBlock` resizes video (and optionally halves the frame rate) on the accelerator using the `ama_scaler` element. The block wraps the chain `ama_upload ! ama_scaler ! capsfilter ! ama_download`, so it accepts system-memory frames on its input and emits system-memory frames downstream — you can drop it into any pipeline without managing device memory yourself.

### Block info

Name: `AMAScalerBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | Video (system memory) | 1 |
| Output video | Video (system memory) | 1 |

### Settings

`AMAScalerSettings`:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Width` / `Height` | int | 0 | Target output size in pixels (144–7580). 0 leaves the dimension unconstrained. |
| `Device` | int | −1 | Accelerator to scale on. −1 auto-selects. |
| `Framerate` | `AMAScalerFramerate` | Auto | Output frame-rate mode (Auto / Full / Half). |

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AMAScalerBlock;
    AMAScalerBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

// Scale to 1280x720 on the AMD accelerator.
var scaler = new AMAScalerBlock(new AMAScalerSettings(1280, 720));
pipeline.Connect(fileSource.VideoOutput, scaler.Input);

var h264EncoderBlock = new H264EncoderBlock(new AMAH264EncoderSettings());
pipeline.Connect(scaler.Output, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Linux (x86_64) with the AMD AMA Video SDK.

## Full hardware transcode

An AMA decoder, the AMA scaler, and an AMA encoder chained together run every stage — decode, scale, and encode — on the AMD accelerator. Each block is self-contained: it accepts and emits system-memory frames, so the SDK moves data on and off the device (`ama_download` / `ama_upload`) at each block boundary while the decode, scale, and encode work itself runs on the accelerator:

```mermaid
graph LR;
    Source-->AMADecoder-->AMAScaler-->AMAEncoder-->Sink;
```

Wire an `H264DecoderBlock` (with `AMAH264DecoderSettings`), an `AMAScalerBlock`, and an `HEVCEncoderBlock` (with `AMAHEVCEncoderSettings`) in sequence to transcode H.264 → scaled HEVC, with every stage running on the AMD accelerator.

## Enums reference

| Enum | Members |
| --- | --- |
| `AMARateControl` | Auto, CQP, CBR, VBR, CVBR |
| `AMAQPMode` | Auto, RelativeLoad, Uniform |
| `AMATier` | Auto, Main, High |
| `AMATuneMetrics` | VQ, PSNR, SSIM, VMAF |
| `AMAAV1DeviceType` | Any, Type1, Type2 |
| `AMAScalerFramerate` | Auto, Full, Half |

## Selecting the device

Every AMA settings object (`AMA*EncoderSettings`, `AMA*DecoderSettings`, and `AMAScalerSettings`) exposes a `Device` property that decides which AMD accelerator runs the work — set it on the settings object you pass to the block (the blocks themselves have no `Device` property):

- `Device = -1` (default) — the AMA runtime auto-selects an available accelerator.
- `Device = 0` / `1` / `2`… — pins the work to a specific card, which is how you choose one Alveo accelerator when several AMD devices are installed in the same host.

Because AMA targets dedicated Alveo media accelerators rather than a Radeon GPU, selecting an AMA settings class already directs the work to the accelerator, not to any integrated AMD graphics.

## Limitations

- **Linux x86_64 only.** The AMA elements do not exist on Windows or macOS; the settings classes are compiled only into the Linux build of the SDK.
- **MediaBlocks pipeline only.** The AMA encoders run on the direct Media Blocks pipeline. They are not supported on the `VideoEditCoreX` encodebin output path and raise a `NotSupportedException` there — build AMA encoding with `H264EncoderBlock` / `HEVCEncoderBlock` / `AV1EncoderBlock` instead.
- The AMA runtime must be installed separately (see [Prerequisites](#prerequisites)); the SDK bundles no AMD binaries.

## FAQ

### Does VisioForge support AMD AMA and Alveo hardware encoding?

Yes. VisioForge Media Blocks SDK wraps the AMD AMA Video SDK to provide H.264, HEVC, and AV1 hardware encoding and decoding, plus a hardware scaler, on AMD Alveo media accelerators. Support is Linux x86_64 only and requires the AMD AMA Video SDK to be installed.

### How do I choose which AMD device the AMA encoder runs on?

Set the `Device` property on the AMA settings object. `-1` (the default) auto-selects an available accelerator; `0`, `1`, `2`, and so on pin the encode, decode, or scale to a specific card when multiple AMD devices are present in the host.

### Is AMA hardware acceleration available on Windows or macOS?

No. The AMD AMA Video SDK is Linux x86_64 only, so the AMA encoder, decoder, and scaler blocks are available only on Linux. On Windows or macOS, use the AMF, NVENC, QSV, or software encoders instead.

### What is the difference between AMD AMF and AMD AMA encoders?

AMD AMF drives Radeon GPUs and is available on Windows, Linux, and macOS through classes such as `AMFH264EncoderSettings`. AMD AMA is a separate platform that drives dedicated Alveo media accelerators (such as the MA35D) on Linux, exposed through the `AMA*` classes. They target different hardware and are configured with different settings classes.

### How do I run a full transcode on the AMD accelerator?

Chain an AMA decoder, the `AMAScalerBlock`, and an AMA encoder. Each stage runs on the AMD accelerator; the blocks exchange frames through system memory, and the SDK moves data on and off the device automatically (`ama_download` / `ama_upload`) at each boundary.

## See Also

- [Video Encoders](../VideoEncoders/index.md) — all H.264, HEVC, AV1, and VP9 encoder blocks, including the AMD AMF, NVENC, and QSV hardware encoders.
- [Video Decoders](../VideoDecoders/index.md) — hardware and software decoder blocks for every supported codec.
- [Linux-Specific Blocks](../_Linux/index.md) — blocks and features available on Linux builds of the SDK.
- [HEVC Encoding Guide](../../general/video-encoders/hevc.md) — hardware HEVC encoding across AMD, NVIDIA, and Intel.
- [AV1 Encoding Guide](../../general/video-encoders/av1.md) — AV1 hardware and software encoding options.
