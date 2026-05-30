---
title: Unity Media Blocks SDK .NET — platform feature matrix
description: Feature support by Unity build target — sources, sinks, encoders, capture, and effects across Windows x64, Android, macOS Standalone, and iOS device.
sidebar_label: Platform matrix
order: 59
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Platform Matrix
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - RTSPSourceBlock
  - BufferSinkBlock
---

# Platform matrix

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

This page lists what works on which Unity build target — the matrix is filtered to the runtime
that ships in the cumulative `.unitypackage`. For the SDK-wide feature matrix that covers all
.NET project types (not just Unity), see [Platform matrix (.NET)](../../platform-matrix.md).

Legend: ✅ supported · ⚠️ partial · ❌ not supported

## Build targets

| Build target | Architecture | Scripting backend | Min. version |
|---|---|---|---|
| Windows Editor + Standalone Player | x86_64 | Mono *(default)* or IL2CPP | Windows 10 1809 / Server 2019 |
| macOS Editor + Standalone Player | Universal arm64 + x86_64 | Mono *(default)* or IL2CPP | macOS 11 Big Sur |
| Android Standalone Player | arm64-v8a | IL2CPP *(mandatory)* | Android 7.0 / API 24 |
| iOS Standalone Player | device arm64 | IL2CPP *(mandatory)* | iOS 15 |

Other build targets (Linux, WebGL, UWP, tvOS, visionOS, console SDKs) are not part of the
package today.

## Source blocks

| Source | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `UniversalSourceBlock` (file / URL via decodebin) | ✅ | ✅ | ✅ | ✅ |
| `RTSPSourceBlock` (live RTSP / RTSPS) | ✅ | ✅ | ✅ | ✅ |
| `VirtualVideoSourceBlock` (test pattern) | ✅ | ✅ | ✅ | ✅ |
| `HTTPMJPEGSourceBlock` | ✅ | ✅ | ✅ | ✅ |
| `NDISourceBlock` | ✅ | ❌ | ✅ | ❌ |
| `DecklinkVideoSourceBlock` | ✅ | ❌ | ✅ | ❌ |
| `SystemVideoSourceBlock` (camera) | ✅ DirectShow / MediaFoundation | ✅ Camera2 | ✅ AVFoundation | ✅ AVFoundation |
| `SystemAudioSourceBlock` (microphone) | ✅ WASAPI | ✅ OpenSL | ✅ CoreAudio | ✅ AVAudio |
| `ScreenSourceBlock` (desktop capture) | ✅ | ❌ | ✅ | ❌ |

## Sink blocks

| Sink | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `BufferSinkBlock` (raw frame callback into `RawImage`) | ✅ | ✅ | ✅ | ✅ |
| `AudioRendererBlock` (system default device) | ✅ WASAPI | ✅ OpenSL | ✅ CoreAudio | ✅ AVAudio |
| `MP4SinkBlock` (file mux) | ✅ | ✅ | ✅ | ✅ |
| `MKVSinkBlock` (file mux) | ✅ | ✅ | ✅ | ✅ |
| `WebMSinkBlock` (file mux) | ✅ | ✅ | ✅ | ✅ |
| `RTSPServerBlock` (egress) | ✅ | ⚠️ no auto port-forward | ✅ | ⚠️ no auto port-forward |
| `RTMPSinkBlock` (egress) | ✅ | ✅ | ✅ | ✅ |
| `SRTSinkBlock` (egress) | ✅ | ✅ | ✅ | ✅ |
| `HLSSinkBlock` (egress) | ✅ | ✅ | ✅ | ✅ |

Unity's built-in renderer is not exposed via `VideoRendererBlock` — render through
`BufferSinkBlock` + `VisioForgeVideoView`, which uploads each frame into a `Texture2D` you
attach to a `RawImage` (or any material).

## Codecs

### Video decode

| Codec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| H.264 | ✅ | ✅ HW | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| H.265 / HEVC | ✅ | ✅ HW | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| AV1 | ✅ SW (libdav1d) | ✅ HW where available | ✅ HW where available | ✅ HW where available |
| VP8 / VP9 | ✅ | ✅ | ✅ | ✅ |
| MPEG-4 part 2 | ✅ | ✅ | ✅ | ✅ |
| MPEG-2 | ✅ | ✅ | ✅ | ✅ |
| MJPEG | ✅ | ✅ | ✅ | ✅ |
| ProRes | ✅ | ⚠️ SW only | ✅ HW | ⚠️ SW only |

### Video encode

| Codec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| H.264 | ✅ NVENC / QSV / SW | ✅ HW (MediaCodec) | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| H.265 / HEVC | ✅ NVENC / QSV / SW | ✅ HW (MediaCodec) | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| AV1 | ✅ SW (SVT-AV1) | ⚠️ device-dependent | ⚠️ SW only | ⚠️ SW only |
| VP8 / VP9 | ✅ SW | ✅ SW | ✅ SW | ✅ SW |
| MJPEG | ✅ | ✅ | ✅ | ✅ |

### Audio decode / encode

| Codec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| AAC | ✅ | ✅ | ✅ | ✅ |
| MP3 | ✅ | ✅ | ✅ | ✅ |
| Opus | ✅ | ✅ | ✅ | ✅ |
| FLAC | ✅ | ✅ | ✅ | ✅ |
| Vorbis | ✅ | ✅ | ✅ | ✅ |
| AC-3 / E-AC-3 | ✅ decode | ✅ decode | ✅ decode | ✅ decode |

## Effects and processing

| Feature | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `BufferSinkBlock` RGBA frame callback | ✅ | ✅ | ✅ | ✅ |
| Audio effects (volume, EQ, normalize) | ✅ | ✅ | ✅ | ✅ |
| Video effects (color, transform, deinterlace) | ✅ | ✅ | ✅ | ✅ |
| `TextOverlayBlock` / `ImageOverlayBlock` (text / image on video) | ✅ | ✅ | ✅ | ✅ |
| `VideoMixerBlock` / `AudioMixerBlock` (live compositing) | ✅ | ✅ | ✅ | ✅ |

## Network protocols

| Protocol | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| RTSP (TCP / UDP / multicast) | ✅ | ✅ | ✅ | ✅ |
| RTSPS (TLS) | ✅ | ✅ | ✅ | ✅ |
| RTMP / RTMPS | ✅ | ✅ | ✅ | ✅ |
| SRT (caller / listener) | ✅ | ✅ | ✅ | ✅ |
| HLS (read / write) | ✅ | ✅ | ✅ | ✅ |
| HTTP / HTTPS | ✅ | ✅ | ✅ | ✅ |
| WebRTC | ⚠️ contact support | ⚠️ contact support | ⚠️ contact support | ⚠️ contact support |
| NDI | ✅ | ❌ | ✅ | ❌ |

WebRTC requires per-deployment configuration (signalling server, ICE / STUN / TURN). Reach out
to support to discuss the production wiring.

## Cumulative-package shipping

The cumulative `.unitypackage` contains the platforms its build was opted into. The package
shipped from `https://files.visioforge.com/unity/` carries:

- Windows-x64 (always)
- Android arm64
- macOS Universal
- iOS device arm64

If a private build skipped a platform with the matching `-Include*` switch left out, the
package's `PluginImporter` slots for that platform are absent and Unity will fail to load the
SDK when the Build Target is switched to it.

## See also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — install + targeting
- [Bootstrap and lifecycle](bootstrap.md) — runtime bring-up across platforms
- [Build for Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
- [Platform matrix (.NET, full SDK surface)](../../platform-matrix.md) — feature support across
  every .NET host (WPF, WinForms, MAUI, Avalonia, Uno, Unity, raw .NET)
- [Media Blocks SDK .NET overview](../../mediablocks/index.md) — the full block catalog
