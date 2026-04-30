---
title: DirectShow SDK Redistributable Files for Deployment
description: Complete list of redistributable files for VisioForge DirectShow SDKs with dependencies, architecture files, and deployment requirements.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - Decoding
  - Mixing
  - Screen Capture
  - WebM
  - H.264
  - MP3

---

# DirectShow SDKs - Redistributable Files Reference

## Overview

This document provides a complete list of files required to redistribute each DirectShow SDK with your application. All files must be included in your installer or deployment package.

---
## FFMPEG Source Filter
### Core Files
#### x86 (32-bit)
**Filter**:
- `VisioForge_FFMPEG_Source.ax` - Main DirectShow filter
**FFmpeg Libraries** (required):
- `avcodec-58.dll` - Video/audio codec library
- `avdevice-58.dll` - Device handling
- `avfilter-7.dll` - Audio/video filtering
- `avformat-58.dll` - Container format handling
- `avutil-56.dll` - Utility functions
- `swresample-3.dll` - Audio resampling
- `swscale-5.dll` - Video scaling and color conversion
**Total Size**: ~80-100 MB
#### x64 (64-bit)
**Filter**:
- `VisioForge_FFMPEG_Source_x64.ax` - Main DirectShow filter (64-bit)
**FFmpeg Libraries** (required):
- `avcodec-58.dll` - 64-bit version
- `avdevice-58.dll` - 64-bit version
- `avfilter-7.dll` - 64-bit version
- `avformat-58.dll` - 64-bit version
- `avutil-56.dll` - 64-bit version
- `swresample-3.dll` - 64-bit version
- `swscale-5.dll` - 64-bit version
**Total Size**: ~90-110 MB
### Installation Directory Structure
```
YourApp\
├── VisioForge_FFMPEG_Source.ax          (x86)
├── VisioForge_FFMPEG_Source_x64.ax      (x64)
├── avcodec-58.dll
├── avdevice-58.dll
├── avfilter-7.dll
├── avformat-58.dll
├── avutil-56.dll
├── swresample-3.dll
└── swscale-5.dll
```
### License Files
- `license.rtf` - SDK license agreement (include in installer)
### Dependencies
- **Visual C++ Redistributable 2015-2022** (x86 or x64)
  - Download: https://aka.ms/vs/17/release/vc_redist.x64.exe
---

## VLC Source Filter

### Core Files

#### x86 (32-bit) Only

**Filter**:
- `VisioForge_VLC_Source.ax` - Main DirectShow filter

**VLC Libraries** (required):
- `libvlc.dll` - VLC core library
- `libvlccore.dll` - VLC core functionality

**VLC Plugins Directory** (required):
- `plugins\` - Complete VLC plugins folder (~100+ plugin DLLs)
  - `plugins\access\` - Input protocols
  - `plugins\audio_filter\` - Audio processing
  - `plugins\audio_mixer\` - Audio mixing
  - `plugins\audio_output\` - Audio output
  - `plugins\codec\` - Codecs
  - `plugins\control\` - Control interfaces
  - `plugins\demux\` - Demultiplexers
  - `plugins\misc\` - Miscellaneous
  - `plugins\packetizer\` - Packetizers
  - `plugins\services_discovery\` - Service discovery
  - `plugins\stream_filter\` - Stream filters
  - `plugins\stream_out\` - Stream output
  - `plugins\text_renderer\` - Text rendering
  - `plugins\video_chroma\` - Color conversion
  - `plugins\video_filter\` - Video filters
  - `plugins\video_output\` - Video output
  - `plugins\visualization\` - Visualizations

**VLC Data Directories**:
- `locale\` - Localization files (optional, ~50+ language folders)
- `lua\` - Lua scripts for playlists and extensions
- `hrtfs\` - HRTF audio files
  - `dodeca_and_7channel_3DSL_HRTF.sofa`

**Total Size**: ~150-200 MB (with all plugins and locales)

### Installation Directory Structure

```
YourApp\
├── VisioForge_VLC_Source.ax
├── libvlc.dll
├── libvlccore.dll
├── plugins\
│   ├── access\
│   ├── audio_filter\
│   ├── codec\
│   └── ... (all plugin directories)
├── locale\           (optional)
├── lua\
└── hrtfs\
```

### License Files

- `license.rtf` - SDK license agreement

### Dependencies

- **Visual C++ Redistributable 2015-2022** (x86)

### Important Notes

- **All VLC plugins must be included** - Missing plugins will cause playback failures for certain formats
- **Maintain directory structure** - VLC expects plugins in `plugins\` subdirectory
- **No x64 version** - VLC Source Filter is 32-bit only

---
## Processing Filters Pack
### Core Filters
#### x86 (32-bit)
**Video Processing**:
- `VisioForge_Video_Effects_Pro.ax` - Video effects filter (35+ effects)
- `VisioForge_Video_Mixer.ax` - Multi-source video mixer
- `VisioForge_Screen_Capture_DD.ax` - DirectDraw screen capture
**Audio Processing**:
- `VisioForge_Audio_Enhancer.ax` - Audio enhancement filter
- `VisioForge_Audio_Effects_4.ax` - Audio effects (optional)
- `VisioForge_Audio_Mixer.ax` - Audio mixer
**Base Filters** (required):
- `VisioForge_BaseFilters.ax` - Core base filter library
- `VisioForge_AsyncEx.ax` - Async file reader (optional)
**Helper Libraries**:
- `VisioForge_MFP.dll` - Media Foundation helper
- `VisioForge_MFPX.dll` - Extended MF functions
#### x64 (64-bit)
**Video Processing**:
- `VisioForge_Video_Effects_Pro_x64.ax`
- `VisioForge_Video_Mixer_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Audio Processing**:
- `VisioForge_Audio_Enhancer_x64.ax`
- `VisioForge_Audio_Mixer_x64.ax`
**Base Filters** (required):
- `VisioForge_BaseFilters_x64.ax`
- `VisioForge_AsyncEx_x64.ax` (optional)
**Helper Libraries**:
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### LAV Filters (Optional but Recommended)
LAV Filters provide additional codec support and are included with Processing Filters Pack.
#### x86
**LAV Filters**:
- `LAVSplitter.ax` - Source splitter
- `LAVVideo.ax` - Video decoder
- `LAVAudio.ax` - Audio decoder
**FFmpeg Libraries for LAV**:
- `avcodec-lav-58.dll`
- `avformat-lav-58.dll`
- `avfilter-lav-7.dll`
- `avresample-lav-4.dll`
- `avutil-lav-56.dll`
- `swscale-lav-5.dll`
**Additional Libraries**:
- `libbluray.dll` - Blu-ray support
- `IntelQuickSyncDecoder.dll` - Intel QuickSync hardware decoding
**Manifest**:
- `LAVFilters.Dependencies.manifest`
**License**:
- `COPYING` - LAV Filters license (LGPL)
#### x64
Same files as x86 but 64-bit versions.
### Installation Directory Structure
```
YourApp\
├── Filters\
│   ├── VisioForge_Video_Effects_Pro.ax
│   ├── VisioForge_Video_Effects_Pro_x64.ax
│   ├── VisioForge_Video_Mixer.ax
│   ├── VisioForge_Video_Mixer_x64.ax
│   ├── VisioForge_Audio_Enhancer.ax
│   ├── VisioForge_Audio_Enhancer_x64.ax
│   ├── VisioForge_BaseFilters.ax
│   ├── VisioForge_BaseFilters_x64.ax
│   ├── VisioForge_MFP.dll
│   ├── VisioForge_MFP64.dll
│   ├── VisioForge_MFPX.dll
│   └── VisioForge_MFPX64.dll
└── LAV\
    ├── x86\
    │   ├── LAVSplitter.ax
    │   ├── LAVVideo.ax
    │   ├── LAVAudio.ax
    │   ├── avcodec-lav-58.dll
    │   └── ... (other LAV files)
    └── x64\
        ├── LAVSplitter.ax
        ├── LAVVideo.ax
        └── ... (other LAV files)
```
### License Files
- `license.rtf` - VisioForge SDK license
- `VisioForge_AsyncEx_license.htm` - Async filter license
- `VisioForge_Audio_Effects_4_note.txt` - Audio effects notes
- `COPYING` - LAV Filters license (in LAV directory)
### Total Size
- **Without LAV Filters**: ~20-30 MB
- **With LAV Filters**: ~80-100 MB
---

## Encoding Filters Pack

### Core Filters

#### x86 (32-bit)

**Video Encoders**:
- `VisioForge_NVENC.ax` - NVIDIA hardware encoder
- `VisioForge_H264_Encoder.ax` - H.264 software encoder
- `VisioForge_H264_Encoder_v9.ax` - H.264 encoder v9
- `VisioForge_H264_Decoder.ax` - H.264 decoder
- `VisioForge_WebM_VP8_Encoder.ax` - VP8 encoder
- `VisioForge_WebM_VP9_Encoder.ax` - VP9 encoder (in x64)
- `VisioForge_WebM_VP8_Decoder.ax` - VP8 decoder
- `VisioForge_WebM_VP9_Decoder.ax` - VP9 decoder

**Audio Encoders**:
- `VisioForge_AAC_Encoder.ax` - AAC encoder
- `VisioForge_AAC_Encoder_v10.ax` - AAC encoder v10
- `VisioForge_LAME.ax` - MP3 encoder (LAME)
- `VisioForge_WebM_Vorbis_Encoder.ax` - Vorbis encoder
- `VisioForge_WebM_Vorbis_Decoder.ax` - Vorbis decoder

**Muxers/Demuxers**:
- `VisioForge_MP4_Muxer.ax` - MP4 container muxer
- `VisioForge_MP4_Muxer_v10.ax` - MP4 muxer v10
- `VisioForge_MF_Mux.ax` - Media Foundation muxer
- `VisioForge_WebM_Mux.ax` - WebM muxer
- `VisioForge_WebM_Split.ax` - WebM splitter
- `VisioForge_WebM_Source.ax` - WebM source
- `VisioForge_WebM_Ogg_Source.ax` - Ogg source
- `VisioForge_SSF_Muxer.ax` - SSF muxer

**Network**:
- `VisioForge_RTSP_Sink.ax` - RTSP sink
- `VisioForge_RTSP_Source_Live555.ax` - RTSP source

**Base Filters** (required):
- `VisioForge_BaseFilters.ax`

**Helper Libraries** (required):
- `VisioForge_MFP.dll` - Media Foundation helper
- `VisioForge_MFP64.dll` - 64-bit MF helper
- `VisioForge_MFPX.dll` - Extended MF functions
- `VisioForge_MFPX64.dll` - 64-bit extended MF
- `VisioForge_MFT.dll` - Media Foundation Transform

**Intel QuickSync** (optional):
- `libmfxsw32.dll` - QuickSync software library
- `libmfxxp32.dll` - QuickSync XP library

#### x64 (64-bit)

**Video Encoders**:
- `VisioForge_NVENC_x64.ax`
- `VisioForge_H264_Encoder_x64.ax`
- `VisioForge_H264_Encoder_v9_x64.ax`
- `VisioForge_H264_Decoder_x64.ax`
- `VisioForge_WebM_VP8_Encoder_x64.ax`
- `VisioForge_WebM_VP9_Encoder_x64.ax`
- `VisioForge_WebM_VP8_Decoder_x64.ax`
- `VisioForge_WebM_VP9_Decoder_x64.ax`

**Audio Encoders**:
- `VisioForge_AAC_Encoder_x64.ax`
- `VisioForge_AAC_Encoder_v10_x64.ax`
- `VisioForge_LAME_x64.ax`
- `VisioForge_WebM_Vorbis_Encoder_x64.ax`
- `VisioForge_WebM_Vorbis_Decoder_x64.ax`

**Muxers/Demuxers**:
- `VisioForge_MP4_Muxer_x64.ax`
- `VisioForge_MP4_Muxer_v10_x64.ax`
- `VisioForge_MF_Mux_x64.ax`
- `VisioForge_WebM_Mux_x64.ax`
- `VisioForge_WebM_Split_x64.ax`
- `VisioForge_WebM_Source_x64.ax`
- `VisioForge_WebM_Ogg_Source_x64.ax`
- `VisioForge_SSF_Muxer_x64.ax`

**Network**:
- `VisioForge_RTSP_Sink_x64.ax`
- `VisioForge_RTSP_Sink_X_x64.ax`
- `VisioForge_RTSP_Source_Live555_x64.ax`

**Base Filters** (required):
- `VisioForge_BaseFilters_x64.ax`

**Helper Libraries** (same as x86):
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
- `VisioForge_MFT64.dll`

**Intel QuickSync** (optional):
- `libmfxsw64.dll`
- `libmfxxp64.dll`

### FFMPEG Encoder

The FFMPEG Encoder has its own set of FFmpeg libraries:

#### x86

**Filter**:
- `VisioForge_FFMPEG_Encoder.ax`

**FFmpeg Libraries**:
- `avcodec-58.dll`
- `avdevice-58.dll`
- `avfilter-7.dll`
- `avformat-58.dll`
- `avutil-56.dll`
- `swresample-3.dll`
- `swscale-5.dll`
- `ffmedia.dll` - VisioForge FFmpeg wrapper

**Info**:
- `vfffmpeg_info.txt` - FFmpeg build information

#### x64

Same files as x86 but 64-bit versions.

### Installation Directory Structure

```
YourApp\
├── Filters\
│   ├── VisioForge_NVENC.ax
│   ├── VisioForge_NVENC_x64.ax
│   ├── VisioForge_H264_Encoder.ax
│   ├── VisioForge_H264_Encoder_x64.ax
│   ├── VisioForge_AAC_Encoder.ax
│   ├── VisioForge_AAC_Encoder_x64.ax
│   ├── VisioForge_MP4_Muxer.ax
│   ├── VisioForge_MP4_Muxer_x64.ax
│   ├── VisioForge_BaseFilters.ax
│   ├── VisioForge_BaseFilters_x64.ax
│   ├── VisioForge_MFP.dll
│   ├── VisioForge_MFP64.dll
│   ├── VisioForge_MFPX.dll
│   ├── VisioForge_MFPX64.dll
│   ├── VisioForge_MFT.dll
│   ├── VisioForge_MFT64.dll
│   ├── libmfxsw32.dll        (QuickSync)
│   ├── libmfxsw64.dll        (QuickSync)
│   └── ... (other filters)
└── FFMPEG\
    ├── x86\
    │   ├── VisioForge_FFMPEG_Encoder.ax
    │   ├── avcodec-58.dll
    │   ├── avformat-58.dll
    │   ├── ffmedia.dll
    │   └── ... (other FFmpeg DLLs)
    └── x64\
        ├── VisioForge_FFMPEG_Encoder_x64.ax
        └── ... (FFmpeg DLLs)
```

### License Files

- `license.rtf` - SDK license

### Total Size

- **Core Filters Only**: ~40-60 MB
- **With FFMPEG Encoder**: ~120-150 MB
- **Complete Pack**: ~150-180 MB

### Hardware Requirements

- **NVENC**: Requires NVIDIA GPU (GeForce GTX 600+ or Quadro K+) and drivers
- **QuickSync**: Requires Intel CPU with integrated graphics (4th gen+)

---
## Virtual Camera SDK
### Core Files
#### x86 (32-bit)
**Virtual Camera Drivers**:
- `VisioForge_Virtual_Camera.ax` - Virtual camera device driver
- `VisioForge_Virtual_Audio_Card.ax` - Virtual audio device driver
**Source Filters**:
- `VisioForge_Push_Video_Source.ax` - Push source for streaming to virtual camera
- `VisioForge_Screen_Capture_DD.ax` - DirectDraw screen capture
**Processing** (included):
- `VisioForge_Video_Effects_Pro.ax` - Video effects
**Base Filters** (required):
- `VisioForge_BaseFilters.ax`
**Helper Libraries** (required):
- `VisioForge_MFP.dll`
- `VisioForge_MFPX.dll`
**Runtime** (required):
- `vcomp140.dll` - Visual C++ OpenMP runtime
#### x64 (64-bit)
**Virtual Camera Drivers**:
- `VisioForge_Virtual_Camera_x64.ax`
- `VisioForge_Virtual_Audio_Card_x64.ax`
**Source Filters**:
- `VisioForge_Push_Video_Source_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Processing**:
- `VisioForge_Video_Effects_Pro_x64.ax`
**Base Filters** (required):
- `VisioForge_BaseFilters_x64.ax`
**Helper Libraries** (required):
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### Installation Directory Structure
```
YourApp\
├── VisioForge_Virtual_Camera.ax
├── VisioForge_Virtual_Camera_x64.ax
├── VisioForge_Virtual_Audio_Card.ax
├── VisioForge_Virtual_Audio_Card_x64.ax
├── VisioForge_Push_Video_Source.ax
├── VisioForge_Push_Video_Source_x64.ax
├── VisioForge_Screen_Capture_DD.ax
├── VisioForge_Screen_Capture_DD_x64.ax
├── VisioForge_Video_Effects_Pro.ax
├── VisioForge_Video_Effects_Pro_x64.ax
├── VisioForge_BaseFilters.ax
├── VisioForge_BaseFilters_x64.ax
├── VisioForge_MFP.dll
├── VisioForge_MFP64.dll
├── VisioForge_MFPX.dll
├── VisioForge_MFPX64.dll
└── vcomp140.dll
```
### License Files
- `license.rtf` - SDK license
### Total Size
~15-20 MB
### Important Notes
- Virtual camera devices appear in video conferencing apps (Zoom, Teams, Skype, etc.)
- Supports up to 4 virtual camera instances
- Requires driver installation (included in installer)
---

## Common Dependencies

### Visual C++ Redistributables

All SDKs require Visual C++ Redistributable 2015-2022.

**Download Links**:
- x86: https://aka.ms/vs/17/release/vc_redist.x86.exe
- x64: https://aka.ms/vs/17/release/vc_redist.x64.exe

**Installation Check** (programmatic):
```cpp
// Check if VC++ Redistributable is installed
bool IsVCRedistInstalled()
{
    HKEY hKey;
    LONG result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
        L"SOFTWARE\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\x64",
        0, KEY_READ, &hKey);

    if (result == ERROR_SUCCESS)
    {
        RegCloseKey(hKey);
        return true;
    }
    return false;
}
```

### Registration Utility

All SDKs include:
- `reg_special.exe` - Custom registration utility

This tool can be used instead of `regsvr32` for filter registration.

---
## Deployment Checklist
### Minimum Required Files
For each SDK, you must include:
1. ✅ **Filter Files** - All .ax files for your architecture (x86/x64)
2. ✅ **Base Filters** - VisioForge_BaseFilters.ax (if required by SDK)
3. ✅ **Helper DLLs** - VisioForge_MFP*.dll, VisioForge_MFPX*.dll
4. ✅ **Dependencies** - FFmpeg DLLs, VLC libraries, etc.
5. ✅ **License File** - license.rtf (display in installer)
6. ✅ **VC++ Redistributable** - Bundle or download in installer
### Optional Files
- 📄 **LAV Filters** - Enhanced codec support (Processing Filters Pack)
- 📄 **QuickSync DLLs** - Intel hardware encoding (Encoding Filters Pack)
- 📄 **VLC Locale** - Multi-language support (VLC Source)
- 📄 **Registration Utility** - reg_special.exe (alternative to regsvr32)
### Architecture Considerations
**32-bit Application**:
- Include only x86 (.ax) files
- No need for x64 versions
**64-bit Application**:
- Include only x64 (_x64.ax) files
- No need for x86 versions
**AnyCPU/.NET Application**:
- Include both x86 and x64 versions
- Register both during installation
- Application will use appropriate architecture at runtime
---

## File Size Summary

| SDK | Minimum Size | With All Options |
|-----|--------------|------------------|
| **FFMPEG Source** | ~80 MB (x86) | ~190 MB (both arch) |
| **VLC Source** | ~150 MB | ~200 MB (with locales) |
| **Processing Filters** | ~20 MB | ~180 MB (with LAV) |
| **Encoding Filters** | ~40 MB | ~300 MB (complete) |
| **Virtual Camera** | ~15 MB | ~35 MB (both arch) |

---
## Testing Deployment Package
Before releasing, verify all files are included:
```batch
@echo off
echo Testing Filter Registration...
REM Test each filter
regsvr32 /s "VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% neq 0 (
    echo ERROR: FFMPEG Source failed to register
    echo Check if all FFmpeg DLLs are present
    exit /b 1
)
REM Test filter creation
YourTestApp.exe
echo All filters registered successfully!
```
---

## See Also

- [Filter Registration](filter-registration.md) - How to register filters
- [Installer Integration](installer-integration.md) - Creating installers
- [Deployment Overview](index.md) - Main deployment guide
