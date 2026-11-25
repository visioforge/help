---
title: FFMPEG Encoder Filter Pin Settings and Codec Options
description: Set FFmpeg encoder options, frame decimation and audio filters per pin of the FFMPEG Encoder DirectShow filter, with WebM, Ogg Speex and FLAC samples.
tags:
  - DirectShow
  - C++
  - C#
  - Windows
  - Encoding
  - FFmpeg
  - WebM
  - VP8
  - VP9
  - Vorbis
  - Opus
  - Speex
  - FLAC
primary_api_classes:
  - IFFmpegSinkPinSettings2
  - IFFmpegSinkPinSettings
  - IFFmpegSinkSettings
  - IFileSinkFilter

---

# FFMPEG Encoder Pin Settings Reference

## Overview

**VisioForge FFMPEG Encoder** (`VisioForge_FFMPEG_Encoder.ax`) is a DirectShow renderer that encodes and muxes its inputs with FFmpeg. Each connected input pin is one stream of the output file: video, audio or subtitles. The filter adds a new input pin each time one is connected.

| Item | Value |
|---|---|
| Filter CLSID | `{69F244AB-31A9-4B74-81CB-CF8C9FACB2EC}` |
| `IFFmpegSinkSettings` (filter) | `{CAEC87C3-24B5-404C-9985-FA18D2B78A4A}` |
| `IFFmpegSinkPinSettings` (input pin) | `{496D3BA7-0630-499E-A730-577D4A049F9F}` |
| `IFFmpegSinkPinSettings2` (input pin) | `{1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B}`, since filter version 2026.11.25.1 |

The output file name is set with the standard `IFileSinkFilter::SetFileName`, the container with `IFFmpegSinkSettings::SetFormat`. Each input pin takes its codec and encoder settings through `IFFmpegSinkPinSettings`, and the finer FFmpeg settings through `IFFmpegSinkPinSettings2`:

- **`SetCodecOption`** passes any FFmpeg encoder option by name: VP8/VP9 deadline, key-frame placement and resizing, Speex VBR, the FLAC block size, and anything else the encoder takes.
- **`SetVideoDecimate`** keeps one of every *n* video frames, for a low-frame-rate recording of a full-rate source.
- **`SetAudioFilter`** runs the audio through an FFmpeg filter graph before the encoder: gain control, noise reduction, equalization.

This filter is not the one described in [FFmpeg Encoder Interface (IVFFFMPEGEncoder)](ffmpeg.md); that page covers an older filter with a different CLSID.

## Platform and Usage Limits

- Windows only, as a DirectShow filter, in x86 and x64 builds. There is no ARM64 build.
- The filter needs the FFmpeg DLLs of its own architecture beside it; see [FFMPEG Encoder redistributable files](../../deployment/redistributable-files.md#ffmpeg-encoder).
- Set the pin settings after the pin is connected and before the graph runs. They are read when the output opens, on the move from stopped to paused or running; a change while the graph runs takes effect on the next run.
- A setting the encoder refuses fails that open: `IMediaControl::Run` (or `Pause`) returns a failure HRESULT and no file is written. The reason is written to the debug output (`OutputDebugString`, visible in DebugView), for example `Session open failed: libvorbis has no option 'no_such_option'`.

## Formats and Codecs

`IFFmpegSinkSettings::SetFormat` takes `FFMPEG_SINK_FORMAT`. `FFMPEG_SINK_FORMAT_FLAC` (a native `.flac` file) is new in 2026.11.25.1:

| Value | Name | Value | Name |
|---|---|---|---|
| 0 | `FFMPEG_SINK_FORMAT_AUTO` (from the file name) | 8 | `FFMPEG_SINK_FORMAT_WEBM` |
| 1 | `FFMPEG_SINK_FORMAT_MP4` | 9 | `FFMPEG_SINK_FORMAT_M4A` |
| 2 | `FFMPEG_SINK_FORMAT_MKV` | 10 | `FFMPEG_SINK_FORMAT_MOV` |
| 3 | `FFMPEG_SINK_FORMAT_FLV` | 11 | `FFMPEG_SINK_FORMAT_MPEG1` |
| 4 | `FFMPEG_SINK_FORMAT_OGG` | 12 | `FFMPEG_SINK_FORMAT_MPEG2` |
| 5 | `FFMPEG_SINK_FORMAT_MPEG_TS` | 13 | `FFMPEG_SINK_FORMAT_MPEG_TS_KLV` |
| 6 | `FFMPEG_SINK_FORMAT_VOB` | 14 | `FFMPEG_SINK_FORMAT_FLAC` |
| 7 | `FFMPEG_SINK_FORMAT_AVI` | | |

`IFFmpegSinkPinSettings::SetAudioCodec` takes `FFMPEG_SINK_AUDIO_CODEC`. `FFMPEG_SINK_AUDIO_CODEC_SPEEX` (libspeex, in Ogg) is new in 2026.11.25.1:

| Value | Name | Value | Name |
|---|---|---|---|
| 0 | `FFMPEG_SINK_AUDIO_CODEC_AUTO` | 4 | `FFMPEG_SINK_AUDIO_CODEC_FLAC` |
| 1 | `FFMPEG_SINK_AUDIO_CODEC_MP2` | 5 | `FFMPEG_SINK_AUDIO_CODEC_VORBIS` |
| 2 | `FFMPEG_SINK_AUDIO_CODEC_MP3` | 6 | `FFMPEG_SINK_AUDIO_CODEC_OPUS` |
| 3 | `FFMPEG_SINK_AUDIO_CODEC_AAC` | 7 | `FFMPEG_SINK_AUDIO_CODEC_SPEEX` |

Speex encodes at 8, 16 or 32 kHz only. Without `SetAudioSampleRate`, the filter takes the supported rate nearest the source, so a 44.1 kHz source is encoded at 32 kHz; set 16000 for wideband speech at a lower bit rate. MP2 takes only the standard Layer II bit rates, and each at some sample rates only: 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 or 384 kbps at 32, 44.1 and 48 kHz; 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144 or 160 kbps at 16, 22.05 and 24 kHz. A bit rate set with `SetAudioBitRate` or the `b`/`ab` option must be one of them, or the start fails; with none set, MP2 uses 384 kbps at 32 to 48 kHz and 160 kbps at 16 to 24 kHz. Without `SetAudioSampleRate`, it takes the rate nearest the source among those that take its bit rate, such as 32 kHz for 192 kbps from a 22.05 kHz source.

## IFFmpegSinkPinSettings2 Methods

`IFFmpegSinkPinSettings2` derives from `IFFmpegSinkPinSettings`, so one pointer reaches both. Query it on the connected input pin.

### SetCodecOption

```cpp
HRESULT SetCodecOption(LPCSTR key, LPCSTR value);
```

Sets an option of this pin's encoder by its FFmpeg AVOption name: the generic ones (`g`, `b`, `maxrate`, `bufsize`, `flags`, `global_quality`, ...) and the encoder's own (`deadline`, `cpu-used`, `vbr_bitrate`, ...). The names are the ones `ffmpeg -h encoder=<name>` lists. Command-line stream specifiers such as `b:v` or `q:a` are not option names: write `b` or `global_quality`.

- An option overrides the pin's other settings: `g` = `10` wins over `SetVideoGOPSize(24)`.
- Setting the same key again replaces its value.
- An option the encoder does not know fails the open; it is never silently dropped.
- Options that would change the frame format the pin takes from its connection return `E_INVALIDARG` at once: `pixel_format`, `video_size`, `time_base`, `ar`, `ch_layout`, and the command-line spellings `pix_fmt`, `s`, `ac`, `sample_fmt`. Set the size, sample rate and channels with `SetVideoWidth`/`SetVideoHeight`, `SetAudioSampleRate` and `SetAudioChannelMask` instead.

**`frame_size`** (audio frame size in samples) holds only where the encoder keeps the size it is given:

| Encoder | `frame_size` |
|---|---|
| FLAC | Taken: the encoder writes blocks of that many samples (for example 1152 or 4096). |
| AAC, Opus, MP3, MP2, Vorbis, Speex | Fails the open unless it equals the encoder's own size. Opus sets its size from `frame_duration` (milliseconds): use that option instead. |
| PCM, and any encoder that takes frames of every size | Fails the open (it would pad the last frame with silence up to the size). |
| Any video encoder | Fails the open. |

`0` asks for the encoder's own size on every encoder. The value is read as FFmpeg reads it, so `0x480` is 1152. A larger frame size also drops more audio at an input discontinuity (a gap in the source timestamps), where the part short of one encoder frame is dropped.

**Options added to this filter's FFmpeg build** (2026.11.25.1), for the settings of the earlier WebM and Ogg filters:

| Encoder | Option | Values | Effect |
|---|---|---|---|
| VP8, VP9 (libvpx) | `kf-mode` | `auto`, `disabled` | `disabled` places no key frames after the first one, also on VP9, which otherwise forces one every `kf_max_dist` frames. |
| VP8 | `resize-allowed` | `0`, `1` | Lets the encoder lower the picture size when the bit rate is short. |
| VP8 | `resize-up-thresh` | 0 to 100 | Buffer level, in percent, above which the encoder scales back up. |
| VP8 | `resize-down-thresh` | 0 to 100 | Buffer level, in percent, below which the encoder scales down. |
| VP8 | `buf-optimal-sz` | milliseconds | The decoder buffer level the rate control aims at. |
| Speex | `vbr_bitrate` | bits per second | VBR at the highest quality whose rate fits this bit rate, capped at it. Stereo adds about 800 bps of side information. |
| Speex | `vbr_max_bitrate` | bits per second, 0 = none | Caps a VBR stream: with `vbr_bitrate`, or with `flags` = `+qscale` and `global_quality`. |

Left unset (the default -1 or 0), each keeps FFmpeg's own behaviour.

### SetVideoDecimate

```cpp
HRESULT SetVideoDecimate(LONG decimate);
```

Keeps the first of every `decimate` video frames and drops the rest; the kept frames keep their own timestamps. `0` or `1` keeps every frame. Use it to record a 25 fps camera at 5 fps (`decimate` = 5) without a frame-rate filter in the graph.

The encoder is told the kept frame rate, so the bit rate set on the pin still holds. The GOP set with `SetVideoGOPSize` counts input frames and is rounded to the nearest kept frame, so the key-frame interval in time holds too; a `g` codec option counts kept frames. The setting is ignored on an audio pin.

### SetAudioFilter

```cpp
HRESULT SetAudioFilter(LPCSTR graph);
```

Runs the pin's audio through an FFmpeg filter graph, written as for the `ffmpeg -af` option, before the resampler and the encoder. An empty string removes it. A graph FFmpeg cannot build fails the open. The setting is ignored on a video pin.

| Graph | Use |
|---|---|
| `speechnorm` | Automatic gain control for speech (stands in for the Speex AGC). |
| `afftdn` | Noise reduction (stands in for the Speex denoiser). |
| `speechnorm,afftdn=nr=20` | Both, chained. |
| `highpass=f=100` | Cut rumble below 100 Hz. |

On an input discontinuity the graph is drained onto the old timeline and started again, so filters with memory, such as `speechnorm`, restart there.

## Interface Definitions

### C++ Definition

The full header, `IFFmpegSinkPinSettings.h`, ships with the filter; the new interface is:

```cpp
MIDL_INTERFACE("1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B")
IFFmpegSinkPinSettings2 : IFFmpegSinkPinSettings {
    STDMETHOD(SetCodecOption)(LPCSTR key, LPCSTR value) = 0;
    STDMETHOD(SetVideoDecimate)(LONG decimate) = 0;
    STDMETHOD(SetAudioFilter)(LPCSTR graph) = 0;
};

_COM_SMARTPTR_TYPEDEF(IFFmpegSinkPinSettings2, __uuidof(IFFmpegSinkPinSettings2));
```

### C# Definition

A COM interface in C# lists the methods of its base interfaces first, in their order. The profile and level parameters are declared as `int` here; their values are those of `FFMPEG_SINK_VIDEO_PROFILE` and `FFMPEG_SINK_VIDEO_LEVEL` in the C++ header (`0` = automatic).

<!-- vf-compile -->
```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    public enum FFMPEG_SINK_FORMAT
    {
        AUTO, MP4, MKV, FLV, OGG, MPEG_TS, VOB, AVI, WEBM, M4A, MOV, MPEG1, MPEG2, MPEG_TS_KLV, FLAC
    }

    public enum FFMPEG_SINK_VIDEO_CODEC
    {
        AUTO, H264_MF, H264_QSV, H264_NVENC, H264_AMF, HEVC_MF, HEVC_QSV, HEVC_NVENC, HEVC_AMF,
        MPEG1, MPEG2, VP8, VP9, MPEG4, OPENH264
    }

    public enum FFMPEG_SINK_AUDIO_CODEC
    {
        AUTO, MP2, MP3, AAC, FLAC, VORBIS, OPUS, SPEEX
    }

    public enum FFMPEG_SINK_SUBTITLE_CODEC
    {
        AUTO, SUBRIP, ASS
    }

    [ComImport]
    [Guid("CAEC87C3-24B5-404C-9985-FA18D2B78A4A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFFmpegSinkSettings
    {
        [PreserveSig] int SetFormat(FFMPEG_SINK_FORMAT format);
    }

    [ComImport]
    [Guid("1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFFmpegSinkPinSettings2
    {
        // IFFmpegSinkPinSettings
        [PreserveSig] int SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC codec);
        [PreserveSig] int SetVideoBitRate(int bitRate);
        [PreserveSig] int SetVideoWidth(int width);
        [PreserveSig] int SetVideoHeight(int height);
        [PreserveSig] int SetVideoProfile(int profile);
        [PreserveSig] int SetVideoLevel(int level);
        [PreserveSig] int SetVideoGOPSize(int gopSize);
        [PreserveSig] int SetVideoMaxBFrames(int maxBFrames);
        [PreserveSig] int SetVideoThreadCount(int threadCount);
        [PreserveSig] int SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC codec);
        [PreserveSig] int SetAudioBitRate(int bitRate);
        [PreserveSig] int SetAudioSampleRate(int sampleRate);
        [PreserveSig] int SetAudioChannelMask(uint channelMask);
        [PreserveSig] int SetSubtitleCodec(FFMPEG_SINK_SUBTITLE_CODEC codec);

        // IFFmpegSinkPinSettings2
        [PreserveSig] int SetCodecOption([MarshalAs(UnmanagedType.LPStr)] string key,
                                         [MarshalAs(UnmanagedType.LPStr)] string value);
        [PreserveSig] int SetVideoDecimate(int decimate);
        [PreserveSig] int SetAudioFilter([MarshalAs(UnmanagedType.LPStr)] string graph);
    }
}
```

`IFFmpegSinkSettings` declares only its first method here; it has a second one, `WriteKLVData`, for MPEG-TS KLV output.

## Usage Examples

The examples start once the source is connected to the FFMPEG Encoder, after `IFileSinkFilter::SetFileName` and `IFFmpegSinkSettings::SetFormat`, and before `IMediaControl::Run`.

### C++ Example - WebM VP8 at 5 fps, No Key Frames After the First

```cpp
#include <dshow.h>
#include "IFFmpegSinkPinSettings.h"

// For each connected input pin of the FFMPEG Encoder:
HRESULT ConfigureWebMPin(IPin* pin)
{
    AM_MEDIA_TYPE mt = {};
    HRESULT hr = pin->ConnectionMediaType(&mt);
    if (FAILED(hr)) return hr;
    const bool isVideo = mt.majortype == MEDIATYPE_Video;
    FreeMediaType(mt);

    IFFmpegSinkPinSettings2Ptr settings;
    hr = pin->QueryInterface(IID_PPV_ARGS(&settings));
    if (FAILED(hr)) return hr;   // a filter older than 2026.11.25.1

    if (isVideo) {
        settings->SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC_VP8);
        settings->SetVideoBitRate(500000);
        settings->SetVideoDecimate(5);                       // 25 fps in, 5 fps out
        settings->SetCodecOption("deadline", "realtime");
        settings->SetCodecOption("kf-mode", "disabled");
    } else {
        settings->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_VORBIS);
        settings->SetAudioBitRate(96000);
    }
    return S_OK;
}
```

### C++ Example - Ogg Speex Voice Recording with AGC and Noise Reduction

```cpp
// Format: IFFmpegSinkSettings::SetFormat(FFMPEG_SINK_FORMAT_OGG)
IFFmpegSinkPinSettings2Ptr audio;
hr = audioPin->QueryInterface(IID_PPV_ARGS(&audio));
if (SUCCEEDED(hr)) {
    audio->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_SPEEX);
    audio->SetAudioSampleRate(16000);                        // wideband
    audio->SetCodecOption("vbr_bitrate", "16000");           // VBR, at most 16 kbps
    audio->SetAudioFilter("speechnorm,afftdn");
}
```

### C++ Example - FLAC File with 4096-Sample Blocks

```cpp
// Format: IFFmpegSinkSettings::SetFormat(FFMPEG_SINK_FORMAT_FLAC)
IFFmpegSinkPinSettings2Ptr audio;
hr = audioPin->QueryInterface(IID_PPV_ARGS(&audio));
if (SUCCEEDED(hr)) {
    audio->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_FLAC);
    audio->SetCodecOption("frame_size", "4096");
    audio->SetCodecOption("compression_level", "8");
}
```

### C# Example - Setting Options on the Encoder Pins

This fragment uses DirectShowLib and the definitions above.

```csharp
using System;
using System.Runtime.InteropServices;
using DirectShowLib;
using VisioForge.DirectShowAPI;

// encoder: the FFMPEG Encoder IBaseFilter, already connected to the source.
void ConfigurePins(IBaseFilter encoder)
{
    encoder.EnumPins(out IEnumPins pins);
    var pin = new IPin[1];
    while (pins.Next(1, pin, IntPtr.Zero) == 0)
    {
        var mt = new AMMediaType();
        if (pin[0].ConnectionMediaType(mt) == 0 && pin[0] is IFFmpegSinkPinSettings2 settings)
        {
            if (mt.majorType == MediaType.Video)
            {
                settings.SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC.VP9);
                settings.SetCodecOption("deadline", "realtime");
                settings.SetCodecOption("cpu-used", "8");
            }
            else if (mt.majorType == MediaType.Audio)
            {
                settings.SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC.OPUS);
                settings.SetAudioFilter("speechnorm");
            }
        }

        DsUtils.FreeAMMediaType(mt);
        Marshal.ReleaseComObject(pin[0]);
    }

    Marshal.ReleaseComObject(pins);
}
```

## Troubleshooting

### Run Fails After Setting an Option

Watch the debug output (DebugView) for `Session open failed:`. The line names the cause:

- `<encoder> has no option '<key>'` - the name is wrong for this encoder, or a command-line spelling such as `b:v` was used. Check `ffmpeg -h encoder=<name>`.
- `<encoder> does not take frame_size <n>` - see the `frame_size` table above; the same text also names a value that is not a number from 0 up, such as `-1` or `abc`.
- An FFmpeg error text, such as `Invalid argument` - the value is out of the option's range, or the audio filter graph does not build.

### SetCodecOption Returns E_INVALIDARG

The key changes the frame format the pin derives from its connection. Use the pin's own setter: `SetVideoWidth`/`SetVideoHeight`, `SetAudioSampleRate`, `SetAudioChannelMask`.

### QueryInterface for IFFmpegSinkPinSettings2 Returns E_NOINTERFACE

The installed `VisioForge_FFMPEG_Encoder.ax` is older than 2026.11.25.1. Update the filter together with the FFmpeg DLLs beside it.

## See Also

- [FFmpeg Encoder Interface (IVFFFMPEGEncoder)](ffmpeg.md)
- [Redistributable Files](../../deployment/redistributable-files.md#ffmpeg-encoder)
- [Encoding Filters Pack Overview](../index.md)
