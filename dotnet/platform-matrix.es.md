---
title: SDKs .NET - Matriz de Plataformas y Características
description: Soporte multiplataforma de SDK .NET para codecs de video/audio, aceleración de hardware, dispositivos de captura en Windows, Linux, macOS, Android, iOS.
---

# SDK .NET: Características y Plataformas Soportadas

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Descubre el conjunto completo de características y amplia compatibilidad de plataformas de los SDKs .NET de VisioForge. Las tablas a continuación detallan formatos de entrada/salida soportados, codecs de video/audio, aceleración de hardware, dispositivos de captura y protocolos de red en Windows, Linux, macOS, Android e iOS.

## Formatos de archivo de entrada y salida

| Formatos de salida | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|--------------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| MP4                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WebM               | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MKV                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| AVI                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| ASF (WMV/WMA)      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-PS            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-TS            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MOV                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MXF                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WMA                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WAV                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MP3                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| OGG                | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |

Además, los motores multiplataforma soportan todos los formatos soportados por FFMPEG y GStreamer.

## Codificadores y decodificadores de video

El SDK soporta los siguientes codecs de video:

| Codificadores | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|---------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| H264          | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| H264/HEVC     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| VP8/VP9       | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| AV1           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MJPEG         | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WMV           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-4 ASP    | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| GIF           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-1        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MPEG-2        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Theora        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| DNxHD         | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| DV            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |

### Codificación y decodificación acelerada por GPU

La tabla a continuación muestra el soporte para codificación y decodificación acelerada por hardware para cada codec y plataforma.

| Codec     | Hardware    | Windows  |  Linux   |  MacOS  | Android   |    iOS   |
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

(1) - Codificadores y decodificadores compatibles con MediaCodec, si son soportados por el hardware

(2) - solo en Apple Silicon

## Codificadores y decodificadores de audio

La tabla a continuación muestra el soporte para codecs de audio en cada plataforma.

| Codificadores | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|---------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| AAC           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MP3           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Vorbis        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| OPUS          | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Speex         | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| FLAC          | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| MP2           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| WMA           | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| OPUS          | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Wavpack       | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |

También puedes usar cualquier otro codificador de audio o video disponible en FFMPEG o GStreamer.

## Dispositivos

La tabla a continuación muestra el soporte para dispositivos de captura en cada plataforma.

| Dispositivos                                    | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|-------------------------------------------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| Webcams y otras fuentes de captura locales      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Cámaras IP y NVR (incluyendo ONVIF)             | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Pantalla                                        | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Blackmagic Decklink (entrada y salida)          | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Videocámaras                                    | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Cámaras USB3/GigE soportadas por GenICam        | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Cámaras GigE/USB3 Teledyne/FLIR                 | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |
| Cámaras GigE/USB3 Basler                        | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |
| Cámaras GigE/USB3 Allied Vision                 | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |

## Protocolos de red

La tabla a continuación muestra el soporte para protocolos de red en cada plataforma.

| Protocolos                        | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|-----------------------------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| RTP/RTSP                          | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| RTMP (YouTube, Facebook Live)     | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| SRT                               | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| UDP                               | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| TCP                               | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| HTTP                              | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| NDI                               | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| VNC (fuente)                      | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| GenICam (fuente)                  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| AWS S3                            | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
