---
title: .NET SDKs - Platform & Feature Matrix
description: .NET SDK cross-platform support for video/audio codecs, hardware acceleration, capture devices on Windows, Linux, macOS, Android, iOS.
---

# .NET SDK: Supported Features and Platforms

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Discover the comprehensive feature set and broad platform compatibility of VisioForge .NET SDKs. The tables below detail supported input/output formats, video/audio codecs, hardware acceleration, capture devices, and network protocols across Windows, Linux, macOS, Android, and iOS.

## Input and output file formats

| Output formats | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
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

Also, cross-platform engines support all formats supported by FFMPEG and GStreamer.

## Video encoders and decoders

SDK supports the following video codecs:

| Encoders   | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
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

### GPU-accelerated encoding and decoding

The table below shows the support for hardware-accelerated encoding and decoding for each codec and platform.

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

(1) - MediaCodec compatible encoders and decoders, if supported by hardware

(2) - only on Apple Silicon

## Audio encoders and decoders

The table below shows the support for audio codecs for each platform.

| Encoders | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
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

Also, you can use any other audio or video encoder available in FFMPEG or GStreamer.

## Devices

The table below shows the support for capture devices for each platform.

| Devices                                 | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
|-----------------------------------------|:--------:|:--------:|:-------:|:--------:|:--------:|
| Webcams and other local capture sources | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| IP cameras and NVR (including ONVIF)    | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Screen                                  | &#x2714; | &#x2714; | &#x2714;| &#x2714; | &#x2714; |
| Blackmagic Decklink (input and output)  | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Camcorders                              | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| GenICam-supported USB3/GigE cameras     | &#x2714; | &#x2714; | &#x2714;| &#x2718; | &#x2718; |
| Teledyne/FLIR GigE/USB3 cameras         | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |
| Basler GigE/USB3 cameras                | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |
| Allied Vision GigE/USB3 cameras         | &#x2714; | &#x2718; | &#x2718;| &#x2718; | &#x2718; |

## Network protocols

The table below shows the support for network protocols for each platform.

| Protocols                     | Windows  |  Linux   |  MacOS  | Android  |    iOS   |
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
