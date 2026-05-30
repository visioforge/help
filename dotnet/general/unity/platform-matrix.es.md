---
title: Matriz de plataformas Unity del Media Blocks SDK .NET
description: Soporte de funciones por target de build Unity — fuentes, sinks, encoders, captura y efectos en Windows x64, Android, macOS Standalone e iOS device.
sidebar_label: Matriz de plataformas
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

# Matriz de plataformas

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta página enumera qué funciona en qué target de build Unity — la matriz está filtrada al
runtime que se distribuye en el `.unitypackage` acumulativo. Para la matriz de funciones del
SDK que cubre todos los tipos de proyecto .NET (no solo Unity), consulta
[Matriz de plataformas (.NET)](../../platform-matrix.md).

Leyenda: ✅ soportado · ⚠️ parcial · ❌ no soportado

## Targets de build

| Target de build | Arquitectura | Backend de scripting | Versión mínima |
|---|---|---|---|
| Windows Editor + Standalone Player | x86_64 | Mono *(predeterminado)* o IL2CPP | Windows 10 1809 / Server 2019 |
| macOS Editor + Standalone Player | Universal arm64 + x86_64 | Mono *(predeterminado)* o IL2CPP | macOS 11 Big Sur |
| Android Standalone Player | arm64-v8a | IL2CPP *(obligatorio)* | Android 7.0 / API 24 |
| iOS Standalone Player | dispositivo arm64 | IL2CPP *(obligatorio)* | iOS 15 |

Otros targets de build (Linux, WebGL, UWP, tvOS, visionOS, SDKs de consola) no son parte del
paquete hoy.

## Bloques fuente

| Fuente | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `UniversalSourceBlock` (archivo / URL vía decodebin) | ✅ | ✅ | ✅ | ✅ |
| `RTSPSourceBlock` (RTSP / RTSPS en vivo) | ✅ | ✅ | ✅ | ✅ |
| `VirtualVideoSourceBlock` (patrón de prueba) | ✅ | ✅ | ✅ | ✅ |
| `HTTPMJPEGSourceBlock` | ✅ | ✅ | ✅ | ✅ |
| `NDISourceBlock` | ✅ | ❌ | ✅ | ❌ |
| `DecklinkVideoSourceBlock` | ✅ | ❌ | ✅ | ❌ |
| `SystemVideoSourceBlock` (cámara) | ✅ DirectShow / MediaFoundation | ✅ Camera2 | ✅ AVFoundation | ✅ AVFoundation |
| `SystemAudioSourceBlock` (micrófono) | ✅ WASAPI | ✅ OpenSL | ✅ CoreAudio | ✅ AVAudio |
| `ScreenSourceBlock` (captura de escritorio) | ✅ | ❌ | ✅ | ❌ |

## Bloques sink

| Sink | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `BufferSinkBlock` (callback de fotograma raw a `RawImage`) | ✅ | ✅ | ✅ | ✅ |
| `AudioRendererBlock` (dispositivo de audio predeterminado) | ✅ WASAPI | ✅ OpenSL | ✅ CoreAudio | ✅ AVAudio |
| `MP4SinkBlock` (mux a archivo) | ✅ | ✅ | ✅ | ✅ |
| `MKVSinkBlock` (mux a archivo) | ✅ | ✅ | ✅ | ✅ |
| `WebMSinkBlock` (mux a archivo) | ✅ | ✅ | ✅ | ✅ |
| `RTSPServerBlock` (salida) | ✅ | ⚠️ sin port-forward automático | ✅ | ⚠️ sin port-forward automático |
| `RTMPSinkBlock` (salida) | ✅ | ✅ | ✅ | ✅ |
| `SRTSinkBlock` (salida) | ✅ | ✅ | ✅ | ✅ |
| `HLSSinkBlock` (salida) | ✅ | ✅ | ✅ | ✅ |

El renderer integrado de Unity no se expone vía `VideoRendererBlock` — renderiza mediante
`BufferSinkBlock` + `VisioForgeVideoView`, que sube cada fotograma a un `Texture2D` que
adjuntas a un `RawImage` (o cualquier material).

## Códecs

### Decodificación de video

| Códec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| H.264 | ✅ | ✅ HW | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| H.265 / HEVC | ✅ | ✅ HW | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| AV1 | ✅ SW (libdav1d) | ✅ HW donde esté disponible | ✅ HW donde esté disponible | ✅ HW donde esté disponible |
| VP8 / VP9 | ✅ | ✅ | ✅ | ✅ |
| MPEG-4 parte 2 | ✅ | ✅ | ✅ | ✅ |
| MPEG-2 | ✅ | ✅ | ✅ | ✅ |
| MJPEG | ✅ | ✅ | ✅ | ✅ |
| ProRes | ✅ | ⚠️ solo SW | ✅ HW | ⚠️ solo SW |

### Codificación de video

| Códec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| H.264 | ✅ NVENC / QSV / SW | ✅ HW (MediaCodec) | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| H.265 / HEVC | ✅ NVENC / QSV / SW | ✅ HW (MediaCodec) | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| AV1 | ✅ SW (SVT-AV1) | ⚠️ dependiente del dispositivo | ⚠️ solo SW | ⚠️ solo SW |
| VP8 / VP9 | ✅ SW | ✅ SW | ✅ SW | ✅ SW |
| MJPEG | ✅ | ✅ | ✅ | ✅ |

### Decodificación / codificación de audio

| Códec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| AAC | ✅ | ✅ | ✅ | ✅ |
| MP3 | ✅ | ✅ | ✅ | ✅ |
| Opus | ✅ | ✅ | ✅ | ✅ |
| FLAC | ✅ | ✅ | ✅ | ✅ |
| Vorbis | ✅ | ✅ | ✅ | ✅ |
| AC-3 / E-AC-3 | ✅ decodificación | ✅ decodificación | ✅ decodificación | ✅ decodificación |

## Efectos y procesamiento

| Función | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| Callback de fotograma RGBA `BufferSinkBlock` | ✅ | ✅ | ✅ | ✅ |
| Efectos de audio (volumen, EQ, normalize) | ✅ | ✅ | ✅ | ✅ |
| Efectos de video (color, transform, deinterlace) | ✅ | ✅ | ✅ | ✅ |
| `TextOverlayBlock` / `ImageOverlayBlock` (texto / imagen sobre video) | ✅ | ✅ | ✅ | ✅ |
| `VideoMixerBlock` / `AudioMixerBlock` (composición en vivo) | ✅ | ✅ | ✅ | ✅ |

## Protocolos de red

| Protocolo | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| RTSP (TCP / UDP / multicast) | ✅ | ✅ | ✅ | ✅ |
| RTSPS (TLS) | ✅ | ✅ | ✅ | ✅ |
| RTMP / RTMPS | ✅ | ✅ | ✅ | ✅ |
| SRT (caller / listener) | ✅ | ✅ | ✅ | ✅ |
| HLS (lectura / escritura) | ✅ | ✅ | ✅ | ✅ |
| HTTP / HTTPS | ✅ | ✅ | ✅ | ✅ |
| WebRTC | ⚠️ contacta con soporte | ⚠️ contacta con soporte | ⚠️ contacta con soporte | ⚠️ contacta con soporte |
| NDI | ✅ | ❌ | ✅ | ❌ |

WebRTC requiere configuración por despliegue (servidor de signalling, ICE / STUN / TURN).
Contacta con soporte para discutir el cableado de producción.

## Distribución del paquete acumulativo

El `.unitypackage` acumulativo contiene las plataformas para las que se optó cuando se
construyó. El paquete distribuido desde `https://files.visioforge.com/unity/` lleva:

- Windows-x64 (siempre)
- Android arm64
- macOS Universal
- iOS dispositivo arm64

Si un build privado omitió una plataforma con el switch `-Include*` correspondiente fuera, los
slots `PluginImporter` del paquete para esa plataforma están ausentes y Unity fallará al cargar
el SDK cuando se cambie al Build Target de esa plataforma.

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — instalación + targeting
- [Bootstrap y ciclo de vida](bootstrap.md) — arranque del runtime entre plataformas
- [Compilar para Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
- [Matriz de plataformas (.NET, superficie completa del SDK)](../../platform-matrix.md) —
  soporte de funciones en cada host .NET (WPF, WinForms, MAUI, Avalonia, Uno, Unity, .NET puro)
- [Resumen del Media Blocks SDK .NET](../../mediablocks/index.md) — el catálogo completo de bloques
