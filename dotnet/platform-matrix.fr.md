---
title: Matrice multiplateforme des codecs et périphériques .NET
description: Matrice multiplateforme du SDK .NET VisioForge pour codecs vidéo/audio, accélération matérielle et périphériques sur Windows, Linux, macOS, Android et iOS.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS

---

# SDK .NET : fonctionnalités et plateformes prises en charge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Découvrez l'ensemble complet de fonctionnalités et la large compatibilité de plateformes des SDK .NET VisioForge. Les tableaux ci-dessous détaillent les formats d'entrée et de sortie pris en charge, les codecs vidéo et audio, l'accélération matérielle, les périphériques de capture et les protocoles réseau sur Windows, Linux, macOS, Android et iOS.

## Formats de fichiers d'entrée et de sortie

| Formats de sortie | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|----------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| MP4            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WebM           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MKV            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| AVI            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| ASF (WMV/WMA)  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-PS        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-TS        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MOV            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MXF            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WMA            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WAV            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MP3            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| OGG            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |

De plus, les moteurs multiplateformes prennent en charge tous les formats pris en charge par FFMPEG et GStreamer.

## Encodeurs et décodeurs vidéo

Le SDK prend en charge les codecs vidéo suivants :

| Encodeurs   | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| H264       | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| H264/HEVC  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| VP8/VP9    | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| AV1        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MJPEG      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WMV        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-4 ASP | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| GIF        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-1     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-2     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Theora     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| DNxHD      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| DV         | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |

### Encodage et décodage accélérés par GPU

Le tableau ci-dessous présente la prise en charge de l'encodage et du décodage accélérés matériellement pour chaque codec et plateforme.

| Codec     | Matériel    | Windows  |  Linux   |  MacOS  | Android   |    iOS   |
|-----------|:-----------:|:--------:|:--------:|:-------:|:---------:|:--------:|
| H264/HEVC | Intel       | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| H264/HEVC | Nvidia      | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| H264/HEVC | AMD         | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| H264/HEVC | Apple       | &#x2718; | &#x2718; | D / E   | &#x2718;  | D / E    |
| H264/HEVC | Android (1) | &#x2718; | &#x2718; | &#x2718;| D / E     | &#x2718; |
| AV1       | Intel       | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| AV1       | Nvidia      | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| AV1       | AMD         | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| AV1       | Apple       | &#x2718; | &#x2718; | D       | &#x2718;  | D        |
| AV1       | Android (1) | &#x2718; | &#x2718; | &#x2718;| D         | &#x2718; |
| VP9       | Intel       | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| VP9       | Nvidia      | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| VP9       | AMD         | D / E    | D / E    | D / E   | &#x2718;  | &#x2718; |
| VP9       | Apple       | &#x2718; | &#x2718; | D (2)   | &#x2718;  | &#x2718; |
| VP9       | Android (1) | &#x2718; | &#x2718; | &#x2718;| D / E     | &#x2718; |

(1) — Encodeurs et décodeurs compatibles MediaCodec, si pris en charge par le matériel

(2) — uniquement sur Apple Silicon

## Encodeurs et décodeurs audio

Le tableau ci-dessous présente la prise en charge des codecs audio pour chaque plateforme.

| Encodeurs | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|----------|:--------:|:--------:|:-------:|:--------:|:--------:|
| AAC      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MP3      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Vorbis   | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| OPUS     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Speex    | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| FLAC     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MP2      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WMA      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| OPUS     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Wavpack  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |

De plus, vous pouvez utiliser tout autre encodeur audio ou vidéo disponible dans FFMPEG ou GStreamer.

## Périphériques

Le tableau ci-dessous présente la prise en charge des périphériques de capture pour chaque plateforme.

| Périphériques                                 | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|-----------------------------------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| Webcams et autres sources de capture locales | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Caméras IP et NVR (y compris ONVIF)    | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Écran                                  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Blackmagic Decklink (entrée et sortie)  | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Caméscopes                              | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Caméras USB3/GigE compatibles GenICam   | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Caméras GigE/USB3 Teledyne/FLIR         | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |
| Caméras GigE/USB3 Basler                | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |
| Caméras GigE/USB3 Allied Vision         | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |

## Protocoles réseau

Le tableau ci-dessous présente la prise en charge des protocoles réseau pour chaque plateforme.

| Protocoles                     | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|-------------------------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| RTP/RTSP                      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| RTMP (YouTube, Facebook Live) | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| SRT                           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| UDP                           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| TCP                           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| HTTP                          | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| NDI                           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| VNC (source)                  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| GenICam (source)              | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| AWS S3                        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
