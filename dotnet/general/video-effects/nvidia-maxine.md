---
title: NVIDIA Maxine AI Video Enhancement & Upscaling in C#
description: Enhance video with NVIDIA Maxine AI in C# .NET — super resolution, upscaling, denoise, and artifact reduction on RTX GPUs with the VisioForge SDK.
sidebar_label: NVIDIA Maxine
tags:
  - Video Capture SDK
  - Media Player SDK
  - .NET
  - NVIDIA Maxine
  - AI
  - Super Resolution
  - Windows
primary_api_classes:
  - MaxineDenoiseVideoEffect
  - MaxineArtifactReductionVideoEffect
  - MaxineSuperResSettings
  - MaxineUpscaleSettings
---

# NVIDIA Maxine AI Video Enhancement in C# .NET

NVIDIA Maxine brings GPU-accelerated, AI-powered video enhancement to the VisioForge classic engines
(`VideoCaptureCore` and `MediaPlayerCore`). The effects run on the NVIDIA Maxine SDK and require an
**NVIDIA RTX GPU** (Tensor cores). They are Windows-only.

| Effect | API type | Applied via |
| --- | --- | --- |
| Denoise | `MaxineDenoiseVideoEffect` | `Video_Effects_Add` |
| Artifact reduction | `MaxineArtifactReductionVideoEffect` | `Video_Effects_Add` |
| Super resolution | `MaxineSuperResSettings` | `Video_Resize` |
| Upscale | `MaxineUpscaleSettings` | `Video_Resize` |

## Requirements

- NVIDIA RTX GPU (GeForce RTX 2060 or higher) with current drivers.
- The NVIDIA Maxine SDK with its **models directory** on disk — most constructors take the path to it.
- Windows 10/11. These effects target the DirectShow-backed `VideoCaptureCore` / `MediaPlayerCore` engines.

The denoise/artifact effect classes live in `VisioForge.Core.Types.VideoEffects` (denoise in
`VisioForge.Core.Types.VideoEffects.NvidiaMaxine`); the super-resolution and upscale settings live in
`VisioForge.Core.Types`.

!!! note "Enable the effect pipeline"
    Filter effects added with `Video_Effects_Add` only run when the effect pipeline is enabled. Set
    `Video_Effects_Enabled = true` once before adding effects — it is `false` by default.

## Denoise

Removes camera/sensor noise while preserving detail. `Strength` ranges 0.0–1.0 (default 0.7).

```csharp
using VisioForge.Core.Types.VideoEffects.NvidiaMaxine;

VideoCapture1.Video_Effects_Enabled = true; // effects are off by default

var denoise = new MaxineDenoiseVideoEffect(modelsDir, strength: 0.7f);
VideoCapture1.Video_Effects_Add(denoise);
```

## Artifact reduction

Removes compression artifacts (blocking, ringing, banding). Pick the mode to match the source bitrate.

```csharp
using VisioForge.Core.Types.VideoEffects;

VideoCapture1.Video_Effects_Enabled = true;

var artifactReduction = new MaxineArtifactReductionVideoEffect(
    modelsDir,
    mode: MaxineArtifactReductionEffectMode.LowBitrate);
VideoCapture1.Video_Effects_Add(artifactReduction);
```

| `MaxineArtifactReductionEffectMode` | Use for |
| --- | --- |
| `HighBitrate` | 10+ Mbps sources; gentler, preserves gradients and fine detail. |
| `LowBitrate` | Below ~5 Mbps; aggressive removal of strong artifacts (default). |

## Super resolution

AI upscaling to a target height. Assign the settings to `Video_Resize` and enable resize/crop. Width is
computed to preserve the aspect ratio.

```csharp
using VisioForge.Core.Types;

VideoCapture1.Video_Resize = new MaxineSuperResSettings(modelsDir, height: 2160)
{
    Mode = MaxineSuperResolutionEffectMode.HQSource,
};
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

| `MaxineSuperResolutionEffectMode` | Use for |
| --- | --- |
| `HQSource` | High-quality/high-bitrate sources; prioritizes artifact removal (default). |
| `LQSource` | Heavily compressed sources; prioritizes detail enhancement. |

## Upscale

A lighter-weight upscaler (vs. super resolution) with a tunable `Strength` (0.0–1.0, default 0.4).

```csharp
using VisioForge.Core.Types;

VideoCapture1.Video_Resize = new MaxineUpscaleSettings(modelsDir, height: 1080, strength: 0.4f);
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

## Media Player engine

The same effects apply to the `MediaPlayerCore` engine for AI enhancement during playback. Set
`MediaPlayer1.Video_Effects_Enabled = true`, then use `MediaPlayer1.Video_Effects_Add(...)` for
denoise/artifact, and assign `MediaPlayer1.Video_Resize` for super resolution/upscale. The player has no
`Video_ResizeOrCrop_Enabled` flag — set `Video_Resize` before starting playback; it is applied when the
playback graph is built (changing it during active playback only takes effect on the next start).

## Demos

- **Nvidia Maxine Demo** (Video Capture, WPF) — [Nvidia Maxine Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Nvidia%20Maxine%20Demo).
- **Nvidia Maxine Player** (Media Player, WPF) — [Nvidia Maxine Player](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Nvidia%20Maxine%20Player).

## See also

- [Effects Reference](effects-reference.md) — full catalog of CPU, GPU, and AI effects.
- [Adding Effects](add.md)
