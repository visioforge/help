---
title: Matrice des plateformes Unity du Media Blocks SDK .NET
description: Fonctionnalités par target de build Unity — sources, sinks, encoders, capture et effets sur Windows, Android, macOS et iOS.
sidebar_label: Matrice des plateformes
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

# Matrice des plateformes

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Cette page liste ce qui fonctionne sur quel target de build Unity — la matrice est filtrée au
runtime livré dans le `.unitypackage` cumulatif. Pour la matrice de fonctionnalités du SDK qui
couvre tous les types de projet .NET (pas seulement Unity), consultez
[Matrice des plateformes (.NET)](../../platform-matrix.md).

Légende : ✅ pris en charge · ⚠️ partiel · ❌ non pris en charge

## Targets de build

| Target de build | Architecture | Backend de scripting | Version minimum |
|---|---|---|---|
| Windows Editor + Standalone Player | x86_64 | Mono *(par défaut)* ou IL2CPP | Windows 10 1809 / Server 2019 |
| macOS Editor + Standalone Player | Universel arm64 + x86_64 | Mono *(par défaut)* ou IL2CPP | macOS 11 Big Sur |
| Android Standalone Player | arm64-v8a | IL2CPP *(obligatoire)* | Android 7.0 / API 24 |
| iOS Standalone Player | appareil arm64 | IL2CPP *(obligatoire)* | iOS 15 |

Les autres targets de build (Linux, WebGL, UWP, tvOS, visionOS, SDKs console) ne font pas
partie du paquet aujourd'hui.

## Blocs source

| Source | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `UniversalSourceBlock` (fichier / URL via decodebin) | ✅ | ✅ | ✅ | ✅ |
| `RTSPSourceBlock` (RTSP / RTSPS en direct) | ✅ | ✅ | ✅ | ✅ |
| `VirtualVideoSourceBlock` (motif de test) | ✅ | ✅ | ✅ | ✅ |
| `HTTPMJPEGSourceBlock` | ✅ | ✅ | ✅ | ✅ |
| `NDISourceBlock` | ✅ | ❌ | ✅ | ❌ |
| `DecklinkVideoSourceBlock` | ✅ | ❌ | ✅ | ❌ |
| `SystemVideoSourceBlock` (caméra) | ✅ DirectShow / MediaFoundation | ✅ Camera2 | ✅ AVFoundation | ✅ AVFoundation |
| `SystemAudioSourceBlock` (micro) | ✅ WASAPI | ✅ OpenSL | ✅ CoreAudio | ✅ AVAudio |
| `ScreenSourceBlock` (capture bureau) | ✅ | ❌ | ✅ | ❌ |

## Blocs sink

| Sink | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| `BufferSinkBlock` (callback frame brut vers `RawImage`) | ✅ | ✅ | ✅ | ✅ |
| `AudioRendererBlock` (périphérique audio par défaut) | ✅ WASAPI | ✅ OpenSL | ✅ CoreAudio | ✅ AVAudio |
| `MP4SinkBlock` (mux vers fichier) | ✅ | ✅ | ✅ | ✅ |
| `MKVSinkBlock` (mux vers fichier) | ✅ | ✅ | ✅ | ✅ |
| `WebMSinkBlock` (mux vers fichier) | ✅ | ✅ | ✅ | ✅ |
| `RTSPServerBlock` (sortie) | ✅ | ⚠️ pas de port-forward auto | ✅ | ⚠️ pas de port-forward auto |
| `RTMPSinkBlock` (sortie) | ✅ | ✅ | ✅ | ✅ |
| `SRTSinkBlock` (sortie) | ✅ | ✅ | ✅ | ✅ |
| `HLSSinkBlock` (sortie) | ✅ | ✅ | ✅ | ✅ |

Le renderer intégré d'Unity n'est pas exposé via `VideoRendererBlock` — rendez via
`BufferSinkBlock` + `VisioForgeVideoView`, qui uploade chaque frame dans un `Texture2D` que
vous attachez à un `RawImage` (ou à n'importe quel material).

## Codecs

### Décodage vidéo

| Codec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| H.264 | ✅ | ✅ HW | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| H.265 / HEVC | ✅ | ✅ HW | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| AV1 | ✅ SW (libdav1d) | ✅ HW si disponible | ✅ HW si disponible | ✅ HW si disponible |
| VP8 / VP9 | ✅ | ✅ | ✅ | ✅ |
| MPEG-4 partie 2 | ✅ | ✅ | ✅ | ✅ |
| MPEG-2 | ✅ | ✅ | ✅ | ✅ |
| MJPEG | ✅ | ✅ | ✅ | ✅ |
| ProRes | ✅ | ⚠️ SW seulement | ✅ HW | ⚠️ SW seulement |

### Encodage vidéo

| Codec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| H.264 | ✅ NVENC / QSV / SW | ✅ HW (MediaCodec) | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| H.265 / HEVC | ✅ NVENC / QSV / SW | ✅ HW (MediaCodec) | ✅ HW (VideoToolbox) | ✅ HW (VideoToolbox) |
| AV1 | ✅ SW (SVT-AV1) | ⚠️ dépend de l'appareil | ⚠️ SW seulement | ⚠️ SW seulement |
| VP8 / VP9 | ✅ SW | ✅ SW | ✅ SW | ✅ SW |
| MJPEG | ✅ | ✅ | ✅ | ✅ |

### Décodage / encodage audio

| Codec | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| AAC | ✅ | ✅ | ✅ | ✅ |
| MP3 | ✅ | ✅ | ✅ | ✅ |
| Opus | ✅ | ✅ | ✅ | ✅ |
| FLAC | ✅ | ✅ | ✅ | ✅ |
| Vorbis | ✅ | ✅ | ✅ | ✅ |
| AC-3 / E-AC-3 | ✅ décodage | ✅ décodage | ✅ décodage | ✅ décodage |

## Effets et traitement

| Fonctionnalité | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| Callback frame RGBA `BufferSinkBlock` | ✅ | ✅ | ✅ | ✅ |
| Effets audio (volume, EQ, normalize) | ✅ | ✅ | ✅ | ✅ |
| Effets vidéo (color, transform, deinterlace) | ✅ | ✅ | ✅ | ✅ |
| `TextOverlayBlock` / `ImageOverlayBlock` (texte / image sur vidéo) | ✅ | ✅ | ✅ | ✅ |
| `VideoMixerBlock` / `AudioMixerBlock` (composition en direct) | ✅ | ✅ | ✅ | ✅ |

## Protocoles réseau

| Protocole | Windows | Android | macOS | iOS |
|---|---|---|---|---|
| RTSP (TCP / UDP / multicast) | ✅ | ✅ | ✅ | ✅ |
| RTSPS (TLS) | ✅ | ✅ | ✅ | ✅ |
| RTMP / RTMPS | ✅ | ✅ | ✅ | ✅ |
| SRT (caller / listener) | ✅ | ✅ | ✅ | ✅ |
| HLS (lecture / écriture) | ✅ | ✅ | ✅ | ✅ |
| HTTP / HTTPS | ✅ | ✅ | ✅ | ✅ |
| WebRTC | ✅ | ✅ | ✅ | ✅ |
| NDI | ✅ | ❌ | ✅ | ❌ |

WebRTC est pris en charge sur toutes les cibles. Comme tout déploiement WebRTC, il nécessite un
serveur de signalisation et une configuration ICE / STUN / TURN pour votre réseau — ce câblage est
spécifique à l'application, pas une limitation de plateforme.

## Distribution du paquet cumulatif

Le `.unitypackage` cumulatif contient les plateformes pour lesquelles il a été opté lors de
sa construction. Le paquet livré depuis `https://files.visioforge.com/unity/` embarque :

- Windows-x64 (toujours)
- Android arm64
- macOS Universel
- iOS appareil arm64

Si une compilation privée a omis une plateforme en laissant de côté son option `-Include*`, les
slots `PluginImporter` du paquet pour cette plateforme sont absents et Unity échouera à
charger le SDK quand on basculera vers le Build Target de cette plateforme.

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — installation + ciblage
- [Démarrage et cycle de vie](bootstrap.md) — démarrage du runtime inter-plateformes
- [Compilation pour Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
- [Matrice des plateformes (.NET, surface complète du SDK)](../../platform-matrix.md) —
  prise en charge des fonctionnalités sur chaque hôte .NET (WPF, WinForms, MAUI, Avalonia,
  Uno, Unity, .NET pur)
- [Aperçu du Media Blocks SDK .NET](../../mediablocks/index.md) — le catalogue complet de blocs
