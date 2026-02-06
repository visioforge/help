---
title: AAC Encoder Interface Reference
description: AAC encoder DirectShow interfaces with bitrate control, profile configuration, sample rate, and advanced encoding for professional audio.
---

# AAC Encoder Interface Reference

## Overview

The AAC (Advanced Audio Coding) encoder DirectShow filters provide interfaces for high-quality audio encoding to the AAC format. AAC is the successor to MP3, offering better sound quality at the same bitrate and is the standard audio codec for MP4, M4A, and streaming applications.

Two AAC encoder interfaces are available:
- **IMonogramAACEncoder**: Simple configuration interface using a single configuration structure
- **IVFAACEncoder**: Comprehensive interface with individual property methods for fine-grained control

## IMonogramAACEncoder Interface

### Overview

The **IMonogramAACEncoder** interface provides a simple, structure-based configuration approach for AAC encoding. Configuration is performed using the `AACConfig` structure that contains all essential encoding parameters.

**Interface GUID**: `{B2DE30C0-1441-4451-A0CE-A914FD561D7F}`

**Inherits From**: `IUnknown`

### AACConfig Structure

```csharp
/// <summary>
/// AAC encoder configuration structure.
/// </summary>
public struct AACConfig
{
    /// <summary>
    /// AAC version/profile (typically 2 for AAC-LC, 4 for AAC-HE)
    /// </summary>
    public int version;

    /// <summary>
    /// Object type / profile:
    /// 2 = AAC-LC (Low Complexity) - recommended for most uses
    /// 5 = AAC-HE (High Efficiency)
    /// 29 = AAC-HEv2 (High Efficiency version 2)
    /// </summary>
    public int object_type;

    /// <summary>
    /// Output format type (0 = Raw AAC, 1 = ADTS)
    /// </summary>
    public int output_type;

    /// <summary>
    /// Target bitrate in bits per second (e.g., 128000 for 128 kbps)
    /// </summary>
    public int bitrate;
}
```

### AACInfo Structure

```csharp
/// <summary>
/// AAC encoder runtime information.
/// </summary>
public struct AACInfo
{
    /// <summary>
    /// Input sample rate in Hz (e.g., 44100, 48000)
    /// </summary>
    public int samplerate;

    /// <summary>
    /// Number of audio channels (1 = mono, 2 = stereo, 6 = 5.1, etc.)
    /// </summary>
    public int channels;

    /// <summary>
    /// AAC frame size in samples (typically 1024 for AAC-LC)
    /// </summary>
    public int frame_size;

    /// <summary>
    /// Total number of frames encoded
    /// </summary>
    public long frames_done;
}
```

### Interface Definitions

#### C# Definition

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// AAC encoder configuration structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AACConfig
    {
        public int version;
        public int object_type;
        public int output_type;
        public int bitrate;
    }

    /// <summary>
    /// AAC encoder runtime information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AACInfo
    {
        public int samplerate;
        public int channels;
        public int frame_size;
        public long frames_done;
    }

    /// <summary>
    /// Monogram AAC encoder configuration interface.
    /// Provides structure-based configuration for AAC encoding.
    /// </summary>
    [ComImport]
    [Guid("B2DE30C0-1441-4451-A0CE-A914FD561D7F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMonogramAACEncoder
    {
        /// <summary>
        /// Gets the current AAC encoder configuration.
        /// </summary>
        /// <param name="config">Reference to AACConfig structure to receive current settings</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetConfig(ref AACConfig config);

        /// <summary>
        /// Sets the AAC encoder configuration.
        /// </summary>
        /// <param name="config">Reference to AACConfig structure containing desired settings</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetConfig(ref AACConfig config);
    }
}
```

#### C++ Definition

```cpp
#include <unknwn.h>

// {B2DE30C0-1441-4451-A0CE-A914FD561D7F}
DEFINE_GUID(IID_IMonogramAACEncoder,
    0xb2de30c0, 0x1441, 0x4451, 0xa0, 0xce, 0xa9, 0x14, 0xfd, 0x56, 0x1d, 0x7f);

/// <summary>
/// AAC encoder configuration structure.
/// </summary>
struct AACConfig
{
    int version;
    int object_type;
    int output_type;
    int bitrate;
};

/// <summary>
/// AAC encoder runtime information.
/// </summary>
struct AACInfo
{
    int samplerate;
    int channels;
    int frame_size;
    __int64 frames_done;
};

/// <summary>
/// Monogram AAC encoder configuration interface.
/// </summary>
DECLARE_INTERFACE_(IMonogramAACEncoder, IUnknown)
{
    /// <summary>
    /// Gets the current AAC encoder configuration.
    /// </summary>
    /// <param name="config">Pointer to AACConfig structure to receive settings</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(GetConfig)(THIS_
        AACConfig* config
        ) PURE;

    /// <summary>
    /// Sets the AAC encoder configuration.
    /// </summary>
    /// <param name="config">Pointer to AACConfig structure with desired settings</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(SetConfig)(THIS_
        const AACConfig* config
        ) PURE;
};
```

#### Delphi Definition

```delphi
uses
  ActiveX, ComObj;

const
  IID_IMonogramAACEncoder: TGUID = '{B2DE30C0-1441-4451-A0CE-A914FD561D7F}';

type
  /// <summary>
  /// AAC encoder configuration structure.
  /// </summary>
  TAACConfig = record
    version: Integer;
    object_type: Integer;
    output_type: Integer;
    bitrate: Integer;
  end;

  /// <summary>
  /// AAC encoder runtime information.
  /// </summary>
  TAACInfo = record
    samplerate: Integer;
    channels: Integer;
    frame_size: Integer;
    frames_done: Int64;
  end;

  /// <summary>
  /// Monogram AAC encoder configuration interface.
  /// </summary>
  IMonogramAACEncoder = interface(IUnknown)
    ['{B2DE30C0-1441-4451-A0CE-A914FD561D7F}']

    /// <summary>
    /// Gets the current AAC encoder configuration.
    /// </summary>
    function GetConfig(var config: TAACConfig): HRESULT; stdcall;

    /// <summary>
    /// Sets the AAC encoder configuration.
    /// </summary>
    function SetConfig(const config: TAACConfig): HRESULT; stdcall;
  end;
```

---
## IVFAACEncoder Interface
### Overview
The **IVFAACEncoder** interface provides comprehensive, property-based configuration for AAC encoding with individual getter/setter methods for each parameter. This interface offers finer control and is easier to use for incremental configuration changes.
**Interface GUID**: `{0BEF7533-39E6-42a5-863F-E087FAB5D84F}`
**Inherits From**: `IUnknown`
### Interface Definitions
#### C# Definition
```csharp
using System;
using System.Runtime.InteropServices;
namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// VisioForge AAC encoder configuration interface.
    /// Provides comprehensive property-based control over AAC encoding parameters.
    /// </summary>
    [ComImport]
    [Guid("0BEF7533-39E6-42a5-863F-E087FAB5D84F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFAACEncoder
    {
        /// <summary>
        /// Forces a specific input sample rate. Set to 0 to accept any rate.
        /// </summary>
        /// <param name="ulSampleRate">Sample rate in Hz (e.g., 44100, 48000). 0 = any rate</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetInputSampleRate(uint ulSampleRate);
        /// <summary>
        /// Gets the configured input sample rate.
        /// </summary>
        /// <param name="pulSampleRate">Receives sample rate in Hz. 0 if not fixed</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetInputSampleRate(out uint pulSampleRate);
        /// <summary>
        /// Sets the number of input channels.
        /// </summary>
        /// <param name="nChannels">Number of channels (1=mono, 2=stereo, 6=5.1, etc.)</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetInputChannels(short nChannels);
        /// <summary>
        /// Gets the number of input channels.
        /// </summary>
        /// <param name="pnChannels">Receives the number of channels</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetInputChannels(out short pnChannels);
        /// <summary>
        /// Sets the target bitrate. Set to -1 to use maximum bitrate.
        /// </summary>
        /// <param name="ulBitRate">Bitrate in bits per second (e.g., 128000). -1 = maximum</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetBitRate(uint ulBitRate);
        /// <summary>
        /// Gets the configured bitrate.
        /// </summary>
        /// <param name="pulBitRate">Receives bitrate in bps. -1 if set to maximum</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetBitRate(out uint pulBitRate);
        /// <summary>
        /// Sets the AAC profile type.
        /// </summary>
        /// <param name="uProfile">Profile: 2=AAC-LC, 5=AAC-HE, 29=AAC-HEv2</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetProfile(uint uProfile);
        /// <summary>
        /// Gets the current AAC profile.
        /// </summary>
        /// <param name="puProfile">Receives the profile type</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetProfile(out uint puProfile);
        /// <summary>
        /// Sets the output format.
        /// </summary>
        /// <param name="uFormat">Format: 0=Raw AAC, 1=ADTS</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetOutputFormat(uint uFormat);
        /// <summary>
        /// Gets the output format.
        /// </summary>
        /// <param name="puFormat">Receives the output format</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetOutputFormat(out uint puFormat);
        /// <summary>
        /// Sets the time shift value for timestamp adjustment.
        /// </summary>
        /// <param name="timeShift">Time shift in milliseconds</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetTimeShift(int timeShift);
        /// <summary>
        /// Gets the time shift value.
        /// </summary>
        /// <param name="ptimeShift">Receives the time shift in milliseconds</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetTimeShift(out int ptimeShift);
        /// <summary>
        /// Enables or disables Low Frequency Effects (LFE) channel.
        /// </summary>
        /// <param name="lfe">1 to enable LFE, 0 to disable</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetLFE(uint lfe);
        /// <summary>
        /// Gets the LFE channel state.
        /// </summary>
        /// <param name="p">Receives LFE state (1=enabled, 0=disabled)</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetLFE(out uint p);
        /// <summary>
        /// Enables or disables Temporal Noise Shaping (TNS).
        /// TNS improves encoding of transient sounds.
        /// </summary>
        /// <param name="tns">1 to enable TNS, 0 to disable</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetTNS(uint tns);
        /// <summary>
        /// Gets the TNS state.
        /// </summary>
        /// <param name="p">Receives TNS state (1=enabled, 0=disabled)</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetTNS(out uint p);
        /// <summary>
        /// Enables or disables Mid-Side stereo coding.
        /// Can improve compression for stereo audio.
        /// </summary>
        /// <param name="v">1 to enable mid-side coding, 0 to disable</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetMidSide(uint v);
        /// <summary>
        /// Gets the mid-side coding state.
        /// </summary>
        /// <param name="p">Receives mid-side state (1=enabled, 0=disabled)</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetMidSide(out uint p);
    }
}
```
#### C++ Definition
```cpp
#include <unknwn.h>
// {0BEF7533-39E6-42a5-863F-E087FAB5D84F}
DEFINE_GUID(IID_IVFAACEncoder,
    0x0bef7533, 0x39e6, 0x42a5, 0x86, 0x3f, 0xe0, 0x87, 0xfa, 0xb5, 0xd8, 0x4f);
/// <summary>
/// VisioForge AAC encoder configuration interface.
/// </summary>
DECLARE_INTERFACE_(IVFAACEncoder, IUnknown)
{
    STDMETHOD(SetInputSampleRate)(THIS_
        unsigned long ulSampleRate
        ) PURE;
    STDMETHOD(GetInputSampleRate)(THIS_
        unsigned long* pulSampleRate
        ) PURE;
    STDMETHOD(SetInputChannels)(THIS_
        short nChannels
        ) PURE;
    STDMETHOD(GetInputChannels)(THIS_
        short* pnChannels
        ) PURE;
    STDMETHOD(SetBitRate)(THIS_
        unsigned long ulBitRate
        ) PURE;
    STDMETHOD(GetBitRate)(THIS_
        unsigned long* pulBitRate
        ) PURE;
    STDMETHOD(SetProfile)(THIS_
        unsigned long uProfile
        ) PURE;
    STDMETHOD(GetProfile)(THIS_
        unsigned long* puProfile
        ) PURE;
    STDMETHOD(SetOutputFormat)(THIS_
        unsigned long uFormat
        ) PURE;
    STDMETHOD(GetOutputFormat)(THIS_
        unsigned long* puFormat
        ) PURE;
    STDMETHOD(SetTimeShift)(THIS_
        int timeShift
        ) PURE;
    STDMETHOD(GetTimeShift)(THIS_
        int* ptimeShift
        ) PURE;
    STDMETHOD(SetLFE)(THIS_
        unsigned long lfe
        ) PURE;
    STDMETHOD(GetLFE)(THIS_
        unsigned long* p
        ) PURE;
    STDMETHOD(SetTNS)(THIS_
        unsigned long tns
        ) PURE;
    STDMETHOD(GetTNS)(THIS_
        unsigned long* p
        ) PURE;
    STDMETHOD(SetMidSide)(THIS_
        unsigned long v
        ) PURE;
    STDMETHOD(GetMidSide)(THIS_
        unsigned long* p
        ) PURE;
};
```
#### Delphi Definition
```delphi
uses
  ActiveX, ComObj;
const
  IID_IVFAACEncoder: TGUID = '{0BEF7533-39E6-42a5-863F-E087FAB5D84F}';
type
  /// <summary>
  /// VisioForge AAC encoder configuration interface.
  /// </summary>
  IVFAACEncoder = interface(IUnknown)
    ['{0BEF7533-39E6-42a5-863F-E087FAB5D84F}']
    function SetInputSampleRate(ulSampleRate: Cardinal): HRESULT; stdcall;
    function GetInputSampleRate(out pulSampleRate: Cardinal): HRESULT; stdcall;
    function SetInputChannels(nChannels: SmallInt): HRESULT; stdcall;
    function GetInputChannels(out pnChannels: SmallInt): HRESULT; stdcall;
    function SetBitRate(ulBitRate: Cardinal): HRESULT; stdcall;
    function GetBitRate(out pulBitRate: Cardinal): HRESULT; stdcall;
    function SetProfile(uProfile: Cardinal): HRESULT; stdcall;
    function GetProfile(out puProfile: Cardinal): HRESULT; stdcall;
    function SetOutputFormat(uFormat: Cardinal): HRESULT; stdcall;
    function GetOutputFormat(out puFormat: Cardinal): HRESULT; stdcall;
    function SetTimeShift(timeShift: Integer): HRESULT; stdcall;
    function GetTimeShift(out ptimeShift: Integer): HRESULT; stdcall;
    function SetLFE(lfe: Cardinal): HRESULT; stdcall;
    function GetLFE(out p: Cardinal): HRESULT; stdcall;
    function SetTNS(tns: Cardinal): HRESULT; stdcall;
    function GetTNS(out p: Cardinal): HRESULT; stdcall;
    function SetMidSide(v: Cardinal): HRESULT; stdcall;
    function GetMidSide(out p: Cardinal): HRESULT; stdcall;
  end;
```
## AAC Profiles and Configuration
### AAC Profiles
**AAC-LC (Low Complexity) - Profile 2** (Recommended):
- Best quality-to-bitrate ratio
- Lowest computational complexity
- Universal decoder support
- Use for: Music, podcasts, video soundtracks
- Bitrate range: 64-320 kbps
**AAC-HE (High Efficiency) - Profile 5**:
- Optimized for low bitrates
- Uses Spectral Band Replication (SBR)
- Better quality than AAC-LC at low bitrates (<= 64 kbps)
- Use for: Streaming, voice, low-bitrate applications
- Bitrate range: 32-80 kbps
**AAC-HEv2 (High Efficiency version 2) - Profile 29**:
- Further optimized for very low bitrates
- Uses Parametric Stereo (PS) in addition to SBR
- Best for mono/stereo at extremely low bitrates
- Use for: Voice streaming, very low bandwidth
- Bitrate range: 16-40 kbps
### Output Formats
**Raw AAC (Format 0)**:
- Pure AAC bitstream without container
- Requires external container (MP4, M4A, MKV)
- Use for: Muxing into MP4/M4A files
- Smallest output size
**ADTS (Audio Data Transport Stream) - Format 1**:
- AAC with frame headers
- Self-contained, can be played directly
- Slightly larger than raw AAC
- Use for: Standalone AAC files, streaming
- Better error resilience
### Recommended Bitrates
| Use Case | Channels | Profile | Bitrate | Notes |
|----------|----------|---------|---------|-------|
| Voice/Podcast (mono) | 1 | AAC-LC | 64-96 kbps | Clear speech |
| Voice/Podcast (stereo) | 2 | AAC-LC | 96-128 kbps | High quality speech |
| Music (stereo) standard | 2 | AAC-LC | 128-192 kbps | Good quality |
| Music (stereo) high quality | 2 | AAC-LC | 256-320 kbps | Excellent quality |
| Low-bandwidth streaming | 2 | AAC-HE | 48-64 kbps | Acceptable quality |
| Very low bandwidth | 1-2 | AAC-HEv2 | 24-40 kbps | Basic quality |
| 5.1 surround | 6 | AAC-LC | 384-512 kbps | Cinema quality |
## Usage Examples
### C# Example - IMonogramAACEncoder (High Quality Music)
```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;
public class MonogramAACHighQuality
{
    public void ConfigureHighQualityMusic(IBaseFilter audioEncoder)
    {
        // Query the Monogram AAC encoder interface
        var aacEncoder = audioEncoder as IMonogramAACEncoder;
        if (aacEncoder == null)
        {
            Console.WriteLine("Error: Filter does not support IMonogramAACEncoder");
            return;
        }
        // Configure high quality stereo music encoding
        var config = new AACConfig
        {
            version = 2,            // AAC version 2
            object_type = 2,        // AAC-LC profile
            output_type = 0,        // Raw AAC (for MP4 muxing)
            bitrate = 192000        // 192 kbps
        };
        int hr = aacEncoder.SetConfig(ref config);
        if (hr == 0)
        {
            Console.WriteLine("AAC encoder configured for high quality music:");
            Console.WriteLine("  Profile: AAC-LC");
            Console.WriteLine("  Bitrate: 192 kbps");
            Console.WriteLine("  Output: Raw AAC for MP4 container");
        }
        else
        {
            Console.WriteLine($"Error configuring AAC encoder: 0x{hr:X8}");
        }
    }
}
```
### C# Example - IVFAACEncoder (Comprehensive Configuration)
```csharp
public class VFAACHighQualityMusic
{
    public void ConfigureComprehensive(IBaseFilter audioEncoder)
    {
        // Query the VisioForge AAC encoder interface
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
        {
            Console.WriteLine("Error: Filter does not support IVFAACEncoder");
            return;
        }
        // Configure comprehensive stereo music encoding
        vfAacEncoder.SetInputSampleRate(48000);     // 48 kHz
        vfAacEncoder.SetInputChannels(2);            // Stereo
        vfAacEncoder.SetBitRate(256000);            // 256 kbps
        vfAacEncoder.SetProfile(2);                 // AAC-LC
        vfAacEncoder.SetOutputFormat(0);            // Raw AAC
        vfAacEncoder.SetTNS(1);                     // Enable TNS
        vfAacEncoder.SetMidSide(1);                 // Enable mid-side coding
        vfAacEncoder.SetLFE(0);                     // No LFE (stereo only)
        vfAacEncoder.SetTimeShift(0);               // No time shift
        Console.WriteLine("VisioForge AAC encoder configured:");
        // Verify configuration
        vfAacEncoder.GetBitRate(out uint bitrate);
        vfAacEncoder.GetProfile(out uint profile);
        vfAacEncoder.GetInputChannels(out short channels);
        Console.WriteLine($"  Bitrate: {bitrate / 1000} kbps");
        Console.WriteLine($"  Profile: {(profile == 2 ? "AAC-LC" : profile.ToString())}");
        Console.WriteLine($"  Channels: {channels}");
    }
}
```
### C# Example - Low Bitrate Streaming (AAC-HE)
```csharp
public class VFAACLowBitrateStreaming
{
    public void ConfigureLowBitrate(IBaseFilter audioEncoder)
    {
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
            return;
        // Configure for low-bitrate streaming
        vfAacEncoder.SetInputSampleRate(44100);     // 44.1 kHz
        vfAacEncoder.SetInputChannels(2);            // Stereo
        vfAacEncoder.SetBitRate(64000);             // 64 kbps
        vfAacEncoder.SetProfile(5);                 // AAC-HE (High Efficiency)
        vfAacEncoder.SetOutputFormat(1);            // ADTS for streaming
        vfAacEncoder.SetTNS(1);                     // Enable TNS
        vfAacEncoder.SetMidSide(1);                 // Enable mid-side
        vfAacEncoder.SetLFE(0);                     // No LFE
        Console.WriteLine("AAC-HE configured for low-bitrate streaming");
        Console.WriteLine("  64 kbps stereo with ADTS output");
    }
}
```
### C# Example - Voice/Podcast Encoding
```csharp
public class VFAACVoicePodcast
{
    public void ConfigureVoicePodcast(IBaseFilter audioEncoder)
    {
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
            return;
        // Configure for voice/podcast (mono)
        vfAacEncoder.SetInputSampleRate(44100);     // 44.1 kHz
        vfAacEncoder.SetInputChannels(1);            // Mono
        vfAacEncoder.SetBitRate(80000);             // 80 kbps
        vfAacEncoder.SetProfile(2);                 // AAC-LC
        vfAacEncoder.SetOutputFormat(0);            // Raw AAC
        vfAacEncoder.SetTNS(1);                     // Enable TNS for speech
        vfAacEncoder.SetMidSide(0);                 // N/A for mono
        vfAacEncoder.SetLFE(0);                     // No LFE
        Console.WriteLine("AAC configured for voice/podcast");
        Console.WriteLine("  80 kbps mono AAC-LC");
    }
}
```
### C++ Example - IMonogramAACEncoder
```cpp
#include <dshow.h>
#include <iostream>
#include "IMonogramAACEncoder.h"
void ConfigureMonogramAAC(IBaseFilter* pAudioEncoder)
{
    IMonogramAACEncoder* pAACEncoder = NULL;
    HRESULT hr = S_OK;
    // Query the Monogram AAC encoder interface
    hr = pAudioEncoder->QueryInterface(IID_IMonogramAACEncoder,
                                       (void**)&pAACEncoder);
    if (FAILED(hr) || !pAACEncoder)
    {
        std::cout << "Error: Filter does not support IMonogramAACEncoder" << std::endl;
        return;
    }
    // Configure high quality music encoding
    AACConfig config;
    config.version = 2;         // AAC version 2
    config.object_type = 2;     // AAC-LC
    config.output_type = 0;     // Raw AAC
    config.bitrate = 192000;    // 192 kbps
    hr = pAACEncoder->SetConfig(&config);
    if (SUCCEEDED(hr))
    {
        std::cout << "AAC encoder configured for high quality music" << std::endl;
        std::cout << "  Profile: AAC-LC" << std::endl;
        std::cout << "  Bitrate: 192 kbps" << std::endl;
    }
    pAACEncoder->Release();
}
```
### C++ Example - IVFAACEncoder
```cpp
#include "IVFAACEncoder.h"
void ConfigureVFAAC(IBaseFilter* pAudioEncoder)
{
    IVFAACEncoder* pVFAACEncoder = NULL;
    HRESULT hr = pAudioEncoder->QueryInterface(IID_IVFAACEncoder,
                                               (void**)&pVFAACEncoder);
    if (SUCCEEDED(hr) && pVFAACEncoder)
    {
        // Configure comprehensive stereo encoding
        pVFAACEncoder->SetInputSampleRate(48000);   // 48 kHz
        pVFAACEncoder->SetInputChannels(2);          // Stereo
        pVFAACEncoder->SetBitRate(256000);          // 256 kbps
        pVFAACEncoder->SetProfile(2);               // AAC-LC
        pVFAACEncoder->SetOutputFormat(0);          // Raw AAC
        pVFAACEncoder->SetTNS(1);                   // Enable TNS
        pVFAACEncoder->SetMidSide(1);               // Enable mid-side
        pVFAACEncoder->SetLFE(0);                   // No LFE
        std::cout << "VisioForge AAC encoder configured" << std::endl;
        pVFAACEncoder->Release();
    }
}
```
### Delphi Example - IMonogramAACEncoder
```delphi
uses
  DirectShow9, ActiveX;
procedure ConfigureMonogramAAC(AudioEncoder: IBaseFilter);
var
  AACEncoder: IMonogramAACEncoder;
  Config: TAACConfig;
  hr: HRESULT;
begin
  // Query the Monogram AAC encoder interface
  hr := AudioEncoder.QueryInterface(IID_IMonogramAACEncoder, AACEncoder);
  if Failed(hr) or (AACEncoder = nil) then
  begin
    WriteLn('Error: Filter does not support IMonogramAACEncoder');
    Exit;
  end;
  try
    // Configure high quality music encoding
    Config.version := 2;         // AAC version 2
    Config.object_type := 2;     // AAC-LC
    Config.output_type := 0;     // Raw AAC
    Config.bitrate := 192000;    // 192 kbps
    hr := AACEncoder.SetConfig(Config);
    if Succeeded(hr) then
    begin
      WriteLn('AAC encoder configured for high quality music');
      WriteLn('  Profile: AAC-LC');
      WriteLn('  Bitrate: 192 kbps');
    end;
  finally
    AACEncoder := nil;
  end;
end;
```
### Delphi Example - IVFAACEncoder
```delphi
procedure ConfigureVFAAC(AudioEncoder: IBaseFilter);
var
  VFAACEncoder: IVFAACEncoder;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IVFAACEncoder, VFAACEncoder)) then
  begin
    try
      // Configure comprehensive stereo encoding
      VFAACEncoder.SetInputSampleRate(48000);   // 48 kHz
      VFAACEncoder.SetInputChannels(2);          // Stereo
      VFAACEncoder.SetBitRate(256000);          // 256 kbps
      VFAACEncoder.SetProfile(2);               // AAC-LC
      VFAACEncoder.SetOutputFormat(0);          // Raw AAC
      VFAACEncoder.SetTNS(1);                   // Enable TNS
      VFAACEncoder.SetMidSide(1);               // Enable mid-side
      VFAACEncoder.SetLFE(0);                   // No LFE
      WriteLn('VisioForge AAC encoder configured');
    finally
      VFAACEncoder := nil;
    end;
  end;
end;
```
## Best Practices
### Profile Selection
**Use AAC-LC (Profile 2) when**:
- Encoding music or high-quality audio
- Bitrate >= 96 kbps
- Maximum decoder compatibility required
- **Recommended for most scenarios**
**Use AAC-HE (Profile 5) when**:
- Bitrate constraints (32-80 kbps)
- Streaming over limited bandwidth
- Voice/speech content acceptable at lower quality
- Mobile/web streaming applications
**Use AAC-HEv2 (Profile 29) when**:
- Extremely limited bandwidth (< 40 kbps)
- Voice-only content
- Mono or stereo only (not multichannel)
### Bitrate Guidelines
**Mono Speech/Podcast**:
- Minimum: 48-64 kbps (AAC-LC)
- Recommended: 80-96 kbps (AAC-LC)
- High quality: 128 kbps (AAC-LC)
**Stereo Music**:
- Minimum: 96-128 kbps (AAC-LC)
- Recommended: 192-256 kbps (AAC-LC)
- High quality: 256-320 kbps (AAC-LC)
**Streaming Applications**:
- Low bandwidth: 48-64 kbps (AAC-HE, stereo)
- Standard bandwidth: 96-128 kbps (AAC-LC, stereo)
- High bandwidth: 192-256 kbps (AAC-LC, stereo)
### Output Format Selection
**Use Raw AAC (Format 0) when**:
- Muxing into MP4, M4A, or MKV containers
- Container provides framing and synchronization
- **Recommended for most video/multimedia applications**
**Use ADTS (Format 1) when**:
- Creating standalone .aac files
- Streaming without container
- Better error recovery needed
- Testing/debugging audio independently
### Feature Flags
**Temporal Noise Shaping (TNS)**:
- **Enable** for all encoding scenarios
- Improves transient response
- Better quality for percussive sounds
- Minimal computational overhead
**Mid-Side Stereo Coding**:
- **Enable** for stereo music encoding
- Improves compression efficiency
- Better stereo imaging
- No benefit for mono or uncorrelated stereo
**Low Frequency Effects (LFE)**:
- **Enable** only for 5.1/7.1 surround sound
- Dedicated subwoofer channel (.1)
- Disable for stereo/mono
## Troubleshooting
### Low Audio Quality
**Symptoms**: Muffled sound, artifacts, poor clarity
**Possible Causes**:
1. Bitrate too low for content
2. Wrong profile for bitrate
3. TNS disabled
**Solutions**:
- Increase bitrate to recommended levels (see tables above)
- For low bitrates (<= 80 kbps), use AAC-HE instead of AAC-LC
- Enable TNS: `SetTNS(1)`
- For music, ensure bitrate >= 128 kbps with AAC-LC
### Encoder Initialization Failures
**Symptoms**: SetConfig or Set methods return error codes
**Possible Causes**:
1. Unsupported sample rate
2. Invalid bitrate for profile
3. Incompatible channel configuration
**Solutions**:
- Use standard sample rates: 44100, 48000 Hz
- Verify bitrate is appropriate for profile
- Check channel count matches source audio
- For AAC-HE, keep bitrate <= 128 kbps
### File Won't Play
**Symptoms**: AAC file doesn't play in media players
**Possible Causes**:
1. Raw AAC without container
2. Unsupported profile
3. Corrupted stream
**Solutions**:
- Use ADTS output format (`SetOutputFormat(1)`) for standalone files
- Use Raw AAC (`SetOutputFormat(0)`) only with MP4/M4A container
- Verify player supports AAC profile (HE/HEv2 may not be supported on old players)
- Ensure proper stream finalization in filter graph
### Compatibility Issues
**Symptoms**: AAC plays on some devices but not others
**Possible Causes**:
1. Advanced profile not supported (AAC-HE/HEv2)
2. Non-standard configuration
**Solutions**:
- Use AAC-LC (Profile 2) for maximum compatibility
- Use standard sample rates (44100 or 48000 Hz)
- Keep bitrates within recommended ranges
- Avoid very low bitrates (< 64 kbps) for AAC-LC
---

## See Also

- [LAME MP3 Encoder Interface](lame.md)
- [FLAC Encoder Interface](flac.md)
- [Audio Codecs Reference](../codecs-reference.md)
- [MP4 Muxer Interface](mp4-muxer.md)
- [Encoding Filters Pack Overview](../index.md)
