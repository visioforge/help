---
title: FFMPEG Encoder DirectShow Filter Interface Reference
description: FFMPEG encoder DirectShow interface for FLV, MPEG-1, MPEG-2, VCD, SVCD, DVD, and Transport Stream with audio/video configuration.
---

# FFMPEG Encoder Interface Reference

## Overview

The **IVFFFMPEGEncoder** interface provides comprehensive configuration for encoding video and audio to various formats using the FFMPEG library. This powerful encoder supports multiple output formats including Flash Video (FLV), MPEG-1, MPEG-2, VCD, SVCD, DVD, and MPEG-2 Transport Stream.

The encoder uses a structure-based configuration approach where all encoding parameters are set at once, providing a simple yet complete interface for professional video encoding to legacy and streaming formats.

**Interface GUID**: `{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}`

**Filter CLSID**: `{554AB365-B293-4C1D-9245-E8DB01F027F7}`

**Inherits From**: `IUnknown`

## Filter and Interface GUIDs

```csharp
// FFMPEG Encoder Filter CLSID
public static readonly Guid CLSID_FFMPEGEncoder =
    new Guid("554AB365-B293-4C1D-9245-E8DB01F027F7");

// IVFFFMPEGEncoder Interface IID
public static readonly Guid IID_IVFFFMPEGEncoder =
    new Guid("17B8FF7D-A67F-45CE-B425-0E4F607D8C60");
```

## Output Formats

### VFFFMPEGDLLOutputFormat Enumeration

```csharp
/// <summary>
/// FFMPEG encoder output format options.
/// </summary>
public enum VFFFMPEGDLLOutputFormat
{
    /// <summary>
    /// Flash Video (.flv) - Web streaming format
    /// </summary>
    FLV = 0,

    /// <summary>
    /// MPEG-1 (.mpg) - Standard MPEG-1 video
    /// </summary>
    MPEG1 = 1,

    /// <summary>
    /// MPEG-1 VCD - Video CD compliant format
    /// Resolution: 352x240 (NTSC) or 352x288 (PAL)
    /// Bitrate: 1150 kbps
    /// </summary>
    MPEG1VCD = 2,

    /// <summary>
    /// MPEG-2 (.mpg) - Standard MPEG-2 video
    /// </summary>
    MPEG2 = 3,

    /// <summary>
    /// MPEG-2 Transport Stream (.ts) - Broadcasting and streaming
    /// </summary>
    MPEG2TS = 4,

    /// <summary>
    /// MPEG-2 SVCD - Super Video CD compliant format
    /// Resolution: 480x480 (NTSC) or 480x576 (PAL)
    /// Bitrate: 2500 kbps
    /// </summary>
    MPEG2SVCD = 5,

    /// <summary>
    /// MPEG-2 DVD - DVD-Video compliant format
    /// Resolution: 720x480 (NTSC) or 720x576 (PAL)
    /// Bitrate: Up to 9800 kbps
    /// </summary>
    MPEG2DVD = 6
}
```

### TV System Standards

```csharp
/// <summary>
/// Television system standards for video encoding.
/// </summary>
public enum VFFFMPEGDLLTVSystem
{
    /// <summary>
    /// No specific TV system / Auto-detect
    /// </summary>
    None = 0,

    /// <summary>
    /// PAL (Phase Alternating Line)
    /// 25 fps, 576 lines
    /// Used in: Europe, Asia, Australia, Africa
    /// </summary>
    PAL = 1,

    /// <summary>
    /// NTSC (National Television System Committee)
    /// 29.97 fps, 480 lines
    /// Used in: North America, Japan, South Korea
    /// </summary>
    NTSC = 2,

    /// <summary>
    /// Film standard
    /// 24 fps
    /// Used for: Cinema, film transfers
    /// </summary>
    Film = 3
}
```

## FFMPEGOutputSettings Structure

```csharp
/// <summary>
/// Complete configuration structure for FFMPEG encoder.
/// Contains all audio, video, and output format settings.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FFMPEGOutputSettings
{
    /// <summary>
    /// Output filename with path.
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Filename;

    /// <summary>
    /// True if audio stream is included in output.
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool AudioAvailable;

    /// <summary>
    /// Audio bitrate in bits per second (e.g., 128000 for 128 kbps).
    /// </summary>
    public int AudioBitrate;

    /// <summary>
    /// Audio sample rate in Hz (e.g., 44100, 48000).
    /// </summary>
    public int AudioSamplerate;

    /// <summary>
    /// Number of audio channels (1 = mono, 2 = stereo).
    /// </summary>
    public int AudioChannels;

    /// <summary>
    /// Video frame width in pixels.
    /// </summary>
    public int VideoWidth;

    /// <summary>
    /// Video frame height in pixels.
    /// </summary>
    public int VideoHeight;

    /// <summary>
    /// Display aspect ratio width (e.g., 16 for 16:9).
    /// </summary>
    public int AspectRatioW;

    /// <summary>
    /// Display aspect ratio height (e.g., 9 for 16:9).
    /// </summary>
    public int AspectRatioH;

    /// <summary>
    /// Video bitrate in bits per second (e.g., 5000000 for 5 Mbps).
    /// </summary>
    public int VideoBitrate;

    /// <summary>
    /// Maximum video bitrate for VBR encoding (bits per second).
    /// </summary>
    public int VideoMaxRate;

    /// <summary>
    /// Minimum video bitrate for VBR encoding (bits per second).
    /// </summary>
    public int VideoMinRate;

    /// <summary>
    /// Video buffer size in bits (affects latency and smoothness).
    /// </summary>
    public int VideoBufferSize;

    /// <summary>
    /// True to enable interlaced encoding (for broadcast TV).
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool Interlace;

    /// <summary>
    /// GOP (Group of Pictures) size - keyframe interval.
    /// Typical: 12-15 for MPEG-2, 30-60 for web video.
    /// </summary>
    public int VideoGopSize;

    /// <summary>
    /// TV system standard (PAL, NTSC, Film, or None).
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public VFFFMPEGDLLTVSystem TVSystem;

    /// <summary>
    /// Output format (FLV, MPEG-1, MPEG-2, etc.).
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public VFFFMPEGDLLOutputFormat OutputFormat;
}
```

## Interface Definitions

### C# Definition

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// FFMPEG encoder configuration interface.
    /// Provides comprehensive encoding to multiple formats using FFMPEG library.
    /// </summary>
    [ComImport]
    [Guid("17B8FF7D-A67F-45CE-B425-0E4F607D8C60")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFFFMPEGEncoder
    {
        /// <summary>
        /// Sets the complete FFMPEG encoder configuration.
        /// All encoding parameters must be set at once via this structure.
        /// </summary>
        /// <param name="settings">Complete encoder settings structure</param>
        [PreserveSig]
        void set_settings([In] FFMPEGOutputSettings settings);
    }
}
```

### C++ Definition

```cpp
#include <unknwn.h>

// {17B8FF7D-A67F-45CE-B425-0E4F607D8C60}
DEFINE_GUID(IID_IVFFFMPEGEncoder,
    0x17b8ff7d, 0xa67f, 0x45ce, 0xb4, 0x25, 0xe, 0x4f, 0x60, 0x7d, 0x8c, 0x60);

// {554AB365-B293-4C1D-9245-E8DB01F027F7}
DEFINE_GUID(CLSID_FFMPEGEncoder,
    0x554ab365, 0xb293, 0x4c1d, 0x92, 0x45, 0xe8, 0xdb, 0x01, 0xf0, 0x27, 0xf7);

/// <summary>
/// Output format enumeration.
/// </summary>
enum FFOutputFormat
{
    of_FLV = 0,
    of_MPEG1 = 1,
    of_MPEG1VCD = 2,
    of_MPEG2 = 3,
    of_MPEG2TS = 4,
    of_MPEG2SVCD = 5,
    of_MPEG2DVD = 6
};

/// <summary>
/// TV system enumeration.
/// </summary>
enum video_tv_system_t
{
    video_norm_unknown = 0,
    video_norm_pal = 1,
    video_norm_ntsc = 2,
    video_norm_film = 3
};

/// <summary>
/// FFMPEG encoder output settings structure.
/// </summary>
struct CVFOutputSettings
{
    wchar_t* filename;

    BOOL audioAvailable;
    int audioBitrate;
    int audioSamplerate;
    int audioChannels;

    int videoWidth;
    int videoHeight;
    int aspectRatioW;
    int aspectRatioH;
    int videoBitrate;
    int videoMaxRate;
    int videoMinRate;
    int videoBufferSize;
    BOOL interlace;
    int videoGopSize;
    int tvSystem;

    int outputFormat;
};

/// <summary>
/// FFMPEG encoder configuration interface.
/// </summary>
DECLARE_INTERFACE_(IVFFFMPEGEncoder, IUnknown)
{
    /// <summary>
    /// Sets the complete FFMPEG encoder configuration.
    /// </summary>
    /// <param name="settings">Complete encoder settings structure</param>
    STDMETHOD(set_settings)(THIS_
        CVFOutputSettings settings
        ) PURE;
};
```

### Delphi Definition

```delphi
uses
  ActiveX, ComObj;

const
  IID_IVFFFMPEGEncoder: TGUID = '{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}';
  CLSID_FFMPEGEncoder: TGUID = '{554AB365-B293-4C1D-9245-E8DB01F027F7}';

type
  /// <summary>
  /// FFMPEG output format enumeration.
  /// </summary>
  TVFFFMPEGDLLOutputFormat = (
    FLV,
    MPEG1,
    MPEG1VCD,
    MPEG2,
    MPEG2TS,
    MPEG2SVCD,
    MPEG2DVD
  );

  /// <summary>
  /// TV system enumeration.
  /// </summary>
  TVFFFMPEGDLLTVSystem = (
    None,
    PAL,
    NTSC,
    Film
  );

  /// <summary>
  /// FFMPEG encoder output settings structure.
  /// </summary>
  TFFMPEGOutputSettings = record
    Filename: PWideChar;
    AudioAvailable: BOOL;
    AudioBitrate: Integer;
    AudioSamplerate: Integer;
    AudioChannels: Integer;
    VideoWidth: Integer;
    VideoHeight: Integer;
    AspectRatioW: Integer;
    AspectRatioH: Integer;
    VideoBitrate: Integer;
    VideoMaxRate: Integer;
    VideoMinRate: Integer;
    VideoBufferSize: Integer;
    Interlace: BOOL;
    VideoGopSize: Integer;
    TVSystem: Integer;
    OutputFormat: Integer;
  end;

  /// <summary>
  /// FFMPEG encoder configuration interface.
  /// </summary>
  IVFFFMPEGEncoder = interface(IUnknown)
    ['{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}']

    /// <summary>
    /// Sets the complete FFMPEG encoder configuration.
    /// </summary>
    procedure set_settings(const settings: TFFMPEGOutputSettings); stdcall;
  end;
```

## Output Format Specifications

### FLV (Flash Video)

**Format**: Adobe Flash Video (.flv)
**Use Cases**: Web streaming, legacy Flash content
**Typical Settings**:
- Resolution: 640x480, 854x480, 1280x720
- Video Bitrate: 500-2500 kbps
- Audio: MP3, 64-128 kbps, 44100 Hz
- GOP: 30-60 frames

### MPEG-1

**Format**: MPEG-1 video (.mpg)
**Use Cases**: Basic video, legacy systems, web
**Typical Settings**:
- Resolution: 352x240 (NTSC), 352x288 (PAL)
- Video Bitrate: 1150 kbps
- Audio: MPEG Layer 2, 224 kbps, 44100 Hz
- GOP: 12-15 frames

### MPEG-1 VCD (Video CD)

**Format**: Video CD compliant MPEG-1 (.mpg)
**Use Cases**: VCD authoring, disc distribution
**Required Settings**:
- Resolution: 352x240 (NTSC), 352x288 (PAL)
- Video Bitrate: 1150 kbps (fixed)
- Audio: MPEG Layer 2, 224 kbps, 44100 Hz
- GOP: 12 frames (NTSC), 15 frames (PAL)
- TV System: Must match (NTSC or PAL)

### MPEG-2

**Format**: MPEG-2 video (.mpg)
**Use Cases**: DVD authoring, broadcast, high quality
**Typical Settings**:
- Resolution: 720x480 (NTSC), 720x576 (PAL)
- Video Bitrate: 4000-9800 kbps
- Audio: MPEG Layer 2 or AC-3, 192-448 kbps
- GOP: 12-15 frames

### MPEG-2 TS (Transport Stream)

**Format**: MPEG-2 Transport Stream (.ts)
**Use Cases**: Broadcasting, streaming, Blu-ray
**Typical Settings**:
- Resolution: 720x480, 1280x720, 1920x1080
- Video Bitrate: 3000-15000 kbps
- Audio: MPEG Layer 2 or AC-3
- GOP: 12-30 frames
- Better error resilience than MPEG-2 PS

### MPEG-2 SVCD (Super Video CD)

**Format**: Super Video CD compliant MPEG-2 (.mpg)
**Use Cases**: SVCD authoring, disc distribution
**Required Settings**:
- Resolution: 480x480 (NTSC), 480x576 (PAL)
- Video Bitrate: 2500 kbps (typical)
- Audio: MPEG Layer 2, 224 kbps, 44100 Hz
- GOP: 12-15 frames
- TV System: Must match (NTSC or PAL)

### MPEG-2 DVD (DVD-Video)

**Format**: DVD-Video compliant MPEG-2 (.mpg)
**Use Cases**: DVD authoring, professional distribution
**Required Settings**:
- Resolution: 720x480 (NTSC), 720x576 (PAL)
- Video Bitrate: 4000-9800 kbps
- Audio: AC-3 or PCM, up to 448 kbps
- GOP: 12 frames (NTSC), 15 frames (PAL)
- TV System: Must match (NTSC or PAL)
- Interlaced: Typically enabled for broadcast

## Usage Examples

### C# Example - DVD-Quality MPEG-2 (NTSC)

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class FFMPEGDVDEncoder
{
    public void ConfigureDVDNTSC(IBaseFilter ffmpegEncoder)
    {
        // Query the FFMPEG encoder interface
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
        {
            Console.WriteLine("Error: Filter does not support IVFFFMPEGEncoder");
            return;
        }

        // Configure DVD-compliant MPEG-2 encoding (NTSC)
        var settings = new FFMPEGOutputSettings
        {
            // Output file
            Filename = "C:\\output\\movie.mpg",

            // Audio settings
            AudioAvailable = true,
            AudioBitrate = 224000,      // 224 kbps
            AudioSamplerate = 48000,    // 48 kHz for DVD
            AudioChannels = 2,           // Stereo

            // Video settings - DVD NTSC specs
            VideoWidth = 720,
            VideoHeight = 480,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 6000000,     // 6 Mbps
            VideoMaxRate = 9800000,     // 9.8 Mbps max for DVD
            VideoMinRate = 0,
            VideoBufferSize = 1835008,  // Standard DVD buffer
            Interlace = true,           // DVD is interlaced
            VideoGopSize = 12,          // 12 frames for NTSC

            // Format settings
            TVSystem = VFFFMPEGDLLTVSystem.NTSC,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2DVD
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for DVD NTSC:");
        Console.WriteLine("  Resolution: 720x480 (16:9)");
        Console.WriteLine("  Video: 6 Mbps MPEG-2, interlaced");
        Console.WriteLine("  Audio: 224 kbps, 48 kHz stereo");
        Console.WriteLine("  GOP: 12 frames");
    }
}
```

### C# Example - Web Streaming FLV

```csharp
public class FFMPEGWebStreaming
{
    public void ConfigureFLV(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configure Flash Video for web streaming
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\video.flv",

            // Audio settings
            AudioAvailable = true,
            AudioBitrate = 96000,       // 96 kbps
            AudioSamplerate = 44100,
            AudioChannels = 2,

            // Video settings - 720p web streaming
            VideoWidth = 1280,
            VideoHeight = 720,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 2000000,     // 2 Mbps
            VideoMaxRate = 2500000,     // 2.5 Mbps max
            VideoMinRate = 1500000,     // 1.5 Mbps min
            VideoBufferSize = 2000000,
            Interlace = false,          // Progressive for web
            VideoGopSize = 60,          // 2 seconds at 30fps

            TVSystem = VFFFMPEGDLLTVSystem.None,
            OutputFormat = VFFFMPEGDLLOutputFormat.FLV
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for FLV web streaming:");
        Console.WriteLine("  720p @ 2 Mbps, progressive");
    }
}
```

### C# Example - VCD Compliant MPEG-1 (PAL)

```csharp
public class FFMPEGVCDEncoder
{
    public void ConfigureVCDPAL(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configure VCD-compliant MPEG-1 (PAL)
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\vcd.mpg",

            // Audio settings - VCD spec
            AudioAvailable = true,
            AudioBitrate = 224000,      // 224 kbps required
            AudioSamplerate = 44100,    // 44.1 kHz required
            AudioChannels = 2,

            // Video settings - VCD PAL specs
            VideoWidth = 352,
            VideoHeight = 288,          // PAL resolution
            AspectRatioW = 4,
            AspectRatioH = 3,
            VideoBitrate = 1150000,     // 1150 kbps required
            VideoMaxRate = 1150000,
            VideoMinRate = 1150000,
            VideoBufferSize = 327680,   // VCD buffer size
            Interlace = false,
            VideoGopSize = 15,          // 15 frames for PAL

            TVSystem = VFFFMPEGDLLTVSystem.PAL,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG1VCD
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for VCD PAL:");
        Console.WriteLine("  352x288 @ 1150 kbps");
    }
}
```

### C# Example - MPEG-2 Transport Stream

```csharp
public class FFMPEGMPEG2TS
{
    public void ConfigureMPEG2TS(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configure MPEG-2 Transport Stream for broadcasting
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\stream.ts",

            // Audio settings
            AudioAvailable = true,
            AudioBitrate = 192000,
            AudioSamplerate = 48000,
            AudioChannels = 2,

            // Video settings - 1080i HD broadcast
            VideoWidth = 1920,
            VideoHeight = 1080,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 12000000,    // 12 Mbps
            VideoMaxRate = 15000000,    // 15 Mbps max
            VideoMinRate = 8000000,     // 8 Mbps min
            VideoBufferSize = 8000000,
            Interlace = true,           // Broadcast is interlaced
            VideoGopSize = 15,

            TVSystem = VFFFMPEGDLLTVSystem.NTSC,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2TS
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for MPEG-2 TS:");
        Console.WriteLine("  1080i HD broadcast stream");
    }
}
```

### C++ Example - DVD NTSC

```cpp
#include <dshow.h>
#include <iostream>
#include "InterfaceDefine.h"

void ConfigureFFMPEGDVD(IBaseFilter* pFFMPEGEncoder)
{
    IVFFFMPEGEncoder* pEncoder = NULL;
    HRESULT hr = S_OK;

    // Query the FFMPEG encoder interface
    hr = pFFMPEGEncoder->QueryInterface(IID_IVFFFMPEGEncoder,
                                        (void**)&pEncoder);
    if (FAILED(hr) || !pEncoder)
    {
        std::cout << "Error: Filter does not support IVFFFMPEGEncoder" << std::endl;
        return;
    }

    // Configure DVD-compliant MPEG-2 encoding (NTSC)
    CVFOutputSettings settings;
    ZeroMemory(&settings, sizeof(settings));

    settings.filename = L"C:\\output\\movie.mpg";

    // Audio settings
    settings.audioAvailable = TRUE;
    settings.audioBitrate = 224000;
    settings.audioSamplerate = 48000;
    settings.audioChannels = 2;

    // Video settings - DVD NTSC specs
    settings.videoWidth = 720;
    settings.videoHeight = 480;
    settings.aspectRatioW = 16;
    settings.aspectRatioH = 9;
    settings.videoBitrate = 6000000;
    settings.videoMaxRate = 9800000;
    settings.videoMinRate = 0;
    settings.videoBufferSize = 1835008;
    settings.interlace = TRUE;
    settings.videoGopSize = 12;

    settings.tvSystem = video_norm_ntsc;
    settings.outputFormat = of_MPEG2DVD;

    pEncoder->set_settings(settings);

    std::cout << "FFMPEG encoder configured for DVD NTSC" << std::endl;

    pEncoder->Release();
}
```

### C++ Example - FLV Web Streaming

```cpp
void ConfigureFFMPEGFLV(IBaseFilter* pFFMPEGEncoder)
{
    IVFFFMPEGEncoder* pEncoder = NULL;
    HRESULT hr = pFFMPEGEncoder->QueryInterface(IID_IVFFFMPEGEncoder,
                                                (void**)&pEncoder);
    if (SUCCEEDED(hr) && pEncoder)
    {
        CVFOutputSettings settings;
        ZeroMemory(&settings, sizeof(settings));

        settings.filename = L"C:\\output\\video.flv";

        // Audio settings
        settings.audioAvailable = TRUE;
        settings.audioBitrate = 96000;
        settings.audioSamplerate = 44100;
        settings.audioChannels = 2;

        // Video settings - 720p web
        settings.videoWidth = 1280;
        settings.videoHeight = 720;
        settings.aspectRatioW = 16;
        settings.aspectRatioH = 9;
        settings.videoBitrate = 2000000;
        settings.videoMaxRate = 2500000;
        settings.videoMinRate = 1500000;
        settings.videoBufferSize = 2000000;
        settings.interlace = FALSE;
        settings.videoGopSize = 60;

        settings.tvSystem = video_norm_unknown;
        settings.outputFormat = of_FLV;

        pEncoder->set_settings(settings);

        std::cout << "FFMPEG FLV encoder configured" << std::endl;

        pEncoder->Release();
    }
}
```

### Delphi Example - DVD PAL

```delphi
uses
  DirectShow9, ActiveX;

procedure ConfigureFFMPEGDVDPAL(FFMPEGEncoder: IBaseFilter);
var
  Encoder: IVFFFMPEGEncoder;
  Settings: TFFMPEGOutputSettings;
  hr: HRESULT;
begin
  hr := FFMPEGEncoder.QueryInterface(IID_IVFFFMPEGEncoder, Encoder);
  if Failed(hr) or (Encoder = nil) then
  begin
    WriteLn('Error: Filter does not support IVFFFMPEGEncoder');
    Exit;
  end;

  try
    ZeroMemory(@Settings, SizeOf(Settings));

    Settings.Filename := 'C:\output\movie.mpg';

    // Audio settings
    Settings.AudioAvailable := True;
    Settings.AudioBitrate := 224000;
    Settings.AudioSamplerate := 48000;
    Settings.AudioChannels := 2;

    // Video settings - DVD PAL specs
    Settings.VideoWidth := 720;
    Settings.VideoHeight := 576;
    Settings.AspectRatioW := 16;
    Settings.AspectRatioH := 9;
    Settings.VideoBitrate := 6000000;
    Settings.VideoMaxRate := 9800000;
    Settings.VideoMinRate := 0;
    Settings.VideoBufferSize := 1835008;
    Settings.Interlace := True;
    Settings.VideoGopSize := 15;

    Settings.TVSystem := Ord(PAL);
    Settings.OutputFormat := Ord(MPEG2DVD);

    Encoder.set_settings(Settings);

    WriteLn('FFMPEG encoder configured for DVD PAL');
  finally
    Encoder := nil;
  end;
end;
```

## Best Practices

### Format-Specific Recommendations

**For DVD Authoring (MPEG2DVD)**:
- Always match TV system to target region (NTSC for Americas/Japan, PAL for Europe/Asia)
- Use 720x480 for NTSC, 720x576 for PAL
- Enable interlacing for broadcast compatibility
- GOP: 12 frames (NTSC), 15 frames (PAL)
- Video bitrate: 4-9.8 Mbps
- Audio: 48 kHz, 224-448 kbps

**For VCD/SVCD (MPEG1VCD, MPEG2SVCD)**:
- Strictly follow format specifications (resolution, bitrate)
- Match TV system to target region
- Use exact specified bitrates for best compatibility
- Test on target hardware (standalone VCD/SVCD players)

**For Web Streaming (FLV)**:
- Use progressive (non-interlaced) encoding
- Lower bitrates for broader reach (500-2500 kbps)
- GOP: 30-60 frames for seek points every 1-2 seconds
- Consider modern alternatives (MP4/H.264) for better quality

**For Broadcasting (MPEG2TS)**:
- Transport Stream has better error resilience
- Use for live streaming and broadcast applications
- Higher bitrates acceptable (10-15 Mbps for HD)
- Match interlacing to broadcast standard

### Resolution and Aspect Ratio

**Standard Resolutions by Format**:

| Format | NTSC Resolution | PAL Resolution | Aspect Ratio |
|--------|----------------|----------------|--------------|
| VCD | 352x240 | 352x288 | 4:3 |
| SVCD | 480x480 | 480x576 | 4:3 or 16:9 |
| DVD | 720x480 | 720x576 | 4:3 or 16:9 |
| FLV/MPEG-2 | Any | Any | Any |

**Aspect Ratio Settings**:
- 4:3 standard: `AspectRatioW = 4, AspectRatioH = 3`
- 16:9 widescreen: `AspectRatioW = 16, AspectRatioH = 9`
- Ensure display aspect ratio matches pixel aspect ratio

### GOP Size Guidelines

**VCD/SVCD/DVD**:
- NTSC: 12 frames (0.4 seconds at 29.97 fps)
- PAL: 15 frames (0.6 seconds at 25 fps)
- Required for format compliance

**Web Streaming**:
- 30-60 frames (1-2 seconds)
- Shorter GOP: Better seeking, larger file
- Longer GOP: Smaller file, slower seeking

**Broadcast**:
- 12-15 frames for high quality
- Match to TV system standard

### Bitrate Configuration

**VBR (Variable Bitrate)**:
- Set `VideoBitrate` (average), `VideoMinRate`, `VideoMaxRate`
- Better quality for same average bitrate
- Use for file-based encoding

**CBR (Constant Bitrate)**:
- Set all three bitrates to the same value
- Predictable file size and bandwidth
- Use for streaming and broadcasting

### Audio Settings

**Sample Rates**:
- 44100 Hz: CD quality, VCD/SVCD
- 48000 Hz: DVD, professional broadcast
- Match source when possible

**Bitrates**:
- Mono speech: 64-96 kbps
- Stereo music: 128-224 kbps
- DVD audio: 192-448 kbps

## Troubleshooting

### Encoder Initialization Fails

**Symptoms**: `set_settings` call fails or crashes

**Possible Causes**:
1. Invalid filename or path
2. Incorrect format specifications
3. Unsupported resolution/bitrate combination

**Solutions**:
- Ensure output directory exists and is writable
- Verify resolution matches format requirements
- Check bitrate is within format limits
- For VCD/SVCD/DVD, strictly follow specifications

### Output File Won't Play

**Symptoms**: Encoded file doesn't play or has errors

**Possible Causes**:
1. Format specifications not met
2. Wrong TV system for format
3. Interlacing mismatch
4. GOP size issues

**Solutions**:
- For DVD/VCD/SVCD: Use exact specification parameters
- Match TV system to target region
- Enable interlacing for DVD/broadcast
- Use standard GOP sizes (12 for NTSC, 15 for PAL)

### Poor Video Quality

**Symptoms**: Blocky, blurry, or artifact-laden video

**Possible Causes**:
1. Bitrate too low for resolution
2. Incorrect GOP size
3. Bitrate range too restrictive

**Solutions**:
- Increase video bitrate (see format recommendations)
- For VBR, widen min/max bitrate range
- Use appropriate GOP size for format
- Ensure resolution matches format specifications

### A/V Sync Issues

**Symptoms**: Audio and video drift out of sync

**Possible Causes**:
1. Incorrect sample rate
2. Wrong frame rate for TV system
3. Buffer size issues

**Solutions**:
- Use 48000 Hz for DVD, 44100 Hz for VCD
- Ensure TV system matches source frame rate
- Increase `VideoBufferSize` for complex content
- Verify source audio/video are synchronized

### DVD/VCD Won't Play on Hardware

**Symptoms**: File plays on computer but not on standalone player

**Possible Causes**:
1. Format specifications not strictly followed
2. Wrong TV system
3. Non-compliant GOP size or bitrate

**Solutions**:
- **Critical**: Use exact format specifications
- VCD: 352x240/288, 1150 kbps, GOP 12/15
- DVD: 720x480/576, max 9.8 Mbps, GOP 12/15
- Match TV system to player region
- Enable interlacing for DVD
- Test with software DVD/VCD player first

### Large File Sizes

**Symptoms**: Output files larger than expected

**Possible Causes**:
1. Bitrate too high
2. CBR instead of VBR
3. Small GOP size

**Solutions**:
- Reduce video bitrate
- Use VBR with appropriate min/max range
- Increase GOP size (for non-DVD/VCD formats)
- Consider more efficient formats (H.264/MP4 instead of MPEG-2)

---
## See Also
- [H.264 Encoder Interface](h264.md)
- [Video Codecs Reference](../codecs-reference.md)
- [MP4 Muxer Interface](mp4-muxer.md)
- [Encoding Filters Pack Overview](../index.md)