---
title: NVENC Encoder - Interface Reference
description: INVEncConfig interface for NVIDIA NVENC hardware video encoding with H.264 and H.265 codecs and GPU acceleration configuration.
---

# INVEncConfig Interface Reference

## Overview

The `INVEncConfig` interface provides comprehensive control over NVIDIA NVENC hardware video encoding. This interface extends the standard DirectShow `IAMVideoCompression` interface with NVENC-specific configuration options for H.264 and H.265 encoding.

NVENC is NVIDIA's dedicated hardware encoder available on GeForce, Quadro, and Tesla GPUs, offering high-performance video encoding with minimal CPU usage.

## Filter and Interface GUIDs

- **Filter CLSID**: `CLSID_NVEncoder`
  `{6EEC9161-7276-430B-A1197-0D4C3BCC87E5}`

- **Interface**: `INVEncConfig`
  **GUID**: `{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}`
  **Inherits From**: `IAMVideoCompression`
  **Header File**: `Intf.h` (C++)

- **Interface**: `INVEncConfig2`
  **GUID**: `{2A741FB6-6DE1-460B-8FCA-76DB478C9357}`
  **Inherits From**: `IUnknown`
  **Header File**: `Intf2.h` (C++)

## Interface Definitions

### C++ Definition (INVEncConfig)

```cpp
#include <strmif.h>

// {9A2AC42C-3E3D-4E6A-84E5-D097292D496B}
static const GUID IID_INVEncConfig =
{ 0x9a2ac42c, 0x3e3d, 0x4e6a, { 0x84, 0xe5, 0xd0, 0x97, 0x29, 0x2d, 0x49, 0x6b } };

// {6EEC9161-7276-430B-A1197-0D4C3BCC87E5}
static const GUID CLSID_NVEncoder =
{ 0x6eec9161, 0x7276, 0x430b, { 0xa1, 0x97, 0xd, 0x4c, 0x3b, 0xcc, 0x87, 0xe5 } };

MIDL_INTERFACE("9A2AC42C-3E3D-4E6A-84E5-D097292D496B")
INVEncConfig : public IAMVideoCompression
{
public:
    virtual HRESULT STDMETHODCALLTYPE SetDeviceType(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetDeviceType(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetPictureStructure(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetPictureStructure(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetNumBuffers(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetNumBuffers(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetRateControl(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetRateControl(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetPreset(GUID v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetPreset(GUID *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetQp(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetQp(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetBFrames(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetBFrames(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetGOP(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetGOP(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetBitrate(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetBitrate(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetVbvBitrate(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetVbvBitrate(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetVbvSize(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetVbvSize(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetProfile(GUID v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetProfile(GUID *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetLevel(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetLevel(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetCodec(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetCodec(int *v) = 0;
};
```

### C# Definition (INVEncConfig)

```csharp
using System;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// NVENC encoder configuration interface.
    /// Provides hardware-accelerated H.264/H.265 encoding on NVIDIA GPUs.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("9A2AC42C-3E3D-4E6A-84E5-D097292D496B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INVEncConfig
    {
        // Note: Also inherits IAMVideoCompression methods
        // (put_KeyFrameRate, get_KeyFrameRate, put_PFramesPerKeyFrame, etc.)

        /// <summary>Sets the CUDA device index for encoding.</summary>
        /// <param name="v">Device index (0 for first GPU, 1 for second, etc.)</param>
        [PreserveSig]
        int SetDeviceType(int v);

        /// <summary>Gets the CUDA device index.</summary>
        [PreserveSig]
        int GetDeviceType(out int v);

        /// <summary>Sets picture structure (progressive or interlaced).</summary>
        /// <param name="v">0 = Progressive, 1 = Interlaced</param>
        [PreserveSig]
        int SetPictureStructure(int v);

        /// <summary>Gets picture structure.</summary>
        [PreserveSig]
        int GetPictureStructure(out int v);

        /// <summary>Sets number of encoding buffers.</summary>
        /// <param name="v">Buffer count (typically 4-8)</param>
        [PreserveSig]
        int SetNumBuffers(int v);

        /// <summary>Gets number of encoding buffers.</summary>
        [PreserveSig]
        int GetNumBuffers(out int v);

        /// <summary>Sets rate control mode.</summary>
        /// <param name="v">0 = CQP, 1 = VBR, 2 = CBR</param>
        [PreserveSig]
        int SetRateControl(int v);

        /// <summary>Gets rate control mode.</summary>
        [PreserveSig]
        int GetRateControl(out int v);

        /// <summary>Sets encoding preset.</summary>
        /// <param name="v">Preset GUID (P1-P7)</param>
        [PreserveSig]
        int SetPreset(Guid v);

        /// <summary>Gets encoding preset.</summary>
        [PreserveSig]
        int GetPreset(out Guid v);

        /// <summary>Sets quantization parameter for CQP mode.</summary>
        /// <param name="v">QP value (0-51, lower = higher quality)</param>
        [PreserveSig]
        int SetQp(int v);

        /// <summary>Gets quantization parameter.</summary>
        [PreserveSig]
        int GetQp(out int v);

        /// <summary>Sets number of B-frames.</summary>
        /// <param name="v">B-frame count (0-4)</param>
        [PreserveSig]
        int SetBFrames(int v);

        /// <summary>Gets number of B-frames.</summary>
        [PreserveSig]
        int GetBFrames(out int v);

        /// <summary>Sets GOP (Group of Pictures) size.</summary>
        /// <param name="v">GOP size in frames</param>
        [PreserveSig]
        int SetGOP(int v);

        /// <summary>Gets GOP size.</summary>
        [PreserveSig]
        int GetGOP(out int v);

        /// <summary>Sets target bitrate.</summary>
        /// <param name="v">Bitrate in bits per second</param>
        [PreserveSig]
        int SetBitrate(int v);

        /// <summary>Gets target bitrate.</summary>
        [PreserveSig]
        int GetBitrate(out int v);

        /// <summary>Sets VBV buffer bitrate.</summary>
        /// <param name="v">VBV bitrate in bps</param>
        [PreserveSig]
        int SetVbvBitrate(int v);

        /// <summary>Gets VBV buffer bitrate.</summary>
        [PreserveSig]
        int GetVbvBitrate(out int v);

        /// <summary>Sets VBV buffer size.</summary>
        /// <param name="v">VBV size in bits</param>
        [PreserveSig]
        int SetVbvSize(int v);

        /// <summary>Gets VBV buffer size.</summary>
        [PreserveSig]
        int GetVbvSize(out int v);

        /// <summary>Sets encoding profile.</summary>
        /// <param name="v">Profile GUID (Baseline, Main, High, etc.)</param>
        [PreserveSig]
        int SetProfile(Guid v);

        /// <summary>Gets encoding profile.</summary>
        [PreserveSig]
        int GetProfile(out Guid v);

        /// <summary>Sets profile level.</summary>
        /// <param name="v">Level value (30, 31, 40, 41, 50, 51, etc.)</param>
        [PreserveSig]
        int SetLevel(int v);

        /// <summary>Gets profile level.</summary>
        [PreserveSig]
        int GetLevel(out int v);

        /// <summary>Sets video codec.</summary>
        /// <param name="v">0 = H.264, 1 = H.265</param>
        [PreserveSig]
        int SetCodec(int v);

        /// <summary>Gets video codec.</summary>
        [PreserveSig]
        int GetCodec(out int v);
    }

    /// <summary>
    /// NVENC configuration interface 2 - availability check.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("2A741FB6-6DE1-460B-8FCA-76DB478C9357")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INVEncConfig2
    {
        /// <summary>Checks if NVENC is available on the system.</summary>
        /// <param name="result">True if NVENC is available</param>
        /// <param name="status">NVENC status code</param>
        [PreserveSig]
        int CheckNVENCAvailable([MarshalAs(UnmanagedType.Bool)] out bool result, out int status);
    }
}
```

### Delphi Definition (INVEncConfig)

```delphi
uses
  ActiveX, ComObj;

const
  IID_INVEncConfig: TGUID = '{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}';
  IID_INVEncConfig2: TGUID = '{2A741FB6-6DE1-460B-8FCA-76DB478C9357}';
  CLSID_NVEncoder: TGUID = '{6EEC9161-7276-430B-A1197-0D4C3BCC87E5}';

type
  /// <summary>
  /// NVENC encoder configuration interface.
  /// Extends IAMVideoCompression with NVENC-specific settings.
  /// </summary>
  INVEncConfig = interface(IUnknown)
    ['{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}']

    // Note: Also inherits IAMVideoCompression methods

    function SetDeviceType(v: Integer): HRESULT; stdcall;
    function GetDeviceType(out v: Integer): HRESULT; stdcall;

    function SetPictureStructure(v: Integer): HRESULT; stdcall;
    function GetPictureStructure(out v: Integer): HRESULT; stdcall;

    function SetNumBuffers(v: Integer): HRESULT; stdcall;
    function GetNumBuffers(out v: Integer): HRESULT; stdcall;

    function SetRateControl(v: Integer): HRESULT; stdcall;
    function GetRateControl(out v: Integer): HRESULT; stdcall;

    function SetPreset(v: TGUID): HRESULT; stdcall;
    function GetPreset(out v: TGUID): HRESULT; stdcall;

    function SetQp(v: Integer): HRESULT; stdcall;
    function GetQp(out v: Integer): HRESULT; stdcall;

    function SetBFrames(v: Integer): HRESULT; stdcall;
    function GetBFrames(out v: Integer): HRESULT; stdcall;

    function SetGOP(v: Integer): HRESULT; stdcall;
    function GetGOP(out v: Integer): HRESULT; stdcall;

    function SetBitrate(v: Integer): HRESULT; stdcall;
    function GetBitrate(out v: Integer): HRESULT; stdcall;

    function SetVbvBitrate(v: Integer): HRESULT; stdcall;
    function GetVbvBitrate(out v: Integer): HRESULT; stdcall;

    function SetVbvSize(v: Integer): HRESULT; stdcall;
    function GetVbvSize(out v: Integer): HRESULT; stdcall;

    function SetProfile(v: TGUID): HRESULT; stdcall;
    function GetProfile(out v: TGUID): HRESULT; stdcall;

    function SetLevel(v: Integer): HRESULT; stdcall;
    function GetLevel(out v: Integer): HRESULT; stdcall;

    function SetCodec(v: Integer): HRESULT; stdcall;
    function GetCodec(out v: Integer): HRESULT; stdcall;
  end;

  /// <summary>
  /// NVENC configuration interface 2 - availability check.
  /// </summary>
  INVEncConfig2 = interface(IUnknown)
    ['{2A741FB6-6DE1-460B-8FCA-76DB478C9357}']

    function CheckNVENCAvailable(out result: BOOL; out status: Integer): HRESULT; stdcall;
  end;
```

## Hardware Requirements

### GPU Generations

| GPU Generation | H.264 | H.265 | Quality | Notes |
|----------------|-------|-------|---------|-------|
| **Kepler** (GTX 600/700) | ✓ | ✗ | Basic | 1st generation NVENC |
| **Maxwell** (GTX 900) | ✓ | ✓ | Good | 2nd gen, HEVC support added |
| **Pascal** (GTX 10XX) | ✓ | ✓ | Better | 3rd gen, improved quality |
| **Turing** (RTX 20XX) | ✓ | ✓ | Excellent | 7th gen, B-frame support |
| **Ampere** (RTX 30XX) | ✓ | ✓ | Excellent | 8th gen, AV1 support |
| **Ada/Hopper** (RTX 40XX) | ✓ | ✓ | Best | Latest generation |

### Performance Capabilities

- **1080p @ 60fps**: All NVENC generations
- **4K @ 60fps**: Maxwell and newer
- **8K @ 30fps**: Turing and newer
- **Simultaneous Streams**: 3-5 (varies by GPU)

---
## Methods Reference
All methods inherited from `IAMVideoCompression` are available. The following are NVENC-specific extensions:
### Device Configuration
#### SetDeviceType / GetDeviceType
Sets or retrieves the CUDA device index for encoding.
**Syntax (C++)**:
```cpp
HRESULT SetDeviceType(int v);
HRESULT GetDeviceType(int *v);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetDeviceType(int v);
[PreserveSig]
int GetDeviceType(out int v);
```
**Parameters**:
- `v`: CUDA device index (0 for first GPU, 1 for second GPU, etc.)
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Must be called **before** connecting the encoder filter
- Use 0 for systems with single GPU
- For multi-GPU systems, select the GPU to use for encoding
- Query available CUDA devices using CUDA API or NVIDIA tools
**Example (C++)**:
```cpp
INVEncConfig* pNVEnc = nullptr;
pFilter->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
// Use first GPU
pNVEnc->SetDeviceType(0);
pNVEnc->Release();
```
---

### Picture Structure

#### SetPictureStructure / GetPictureStructure

Sets the picture coding type (progressive or interlaced).

**Syntax (C++)**:
```cpp
HRESULT SetPictureStructure(int v);
HRESULT GetPictureStructure(int *v);
```

**Parameters**:
- `v`: Picture structure type
  - `0` - Progressive (frame-based)
  - `1` - Interlaced (field-based)

**Returns**: `S_OK` on success.

**Usage Notes**:
- Default is progressive (0)
- Use interlaced (1) only for broadcast/DVD content
- Progressive is recommended for modern content

**Example (C++)**:
```cpp
// Set progressive encoding
pNVEnc->SetPictureStructure(0);
```

---
### Buffer Configuration
#### SetNumBuffers / GetNumBuffers
Sets the number of encoding buffers.
**Syntax (C++)**:
```cpp
HRESULT SetNumBuffers(int v);
HRESULT GetNumBuffers(int *v);
```
**Parameters**:
- `v`: Number of buffers (typically 4-8)
**Returns**: `S_OK` on success.
**Usage Notes**:
- More buffers = higher latency but smoother encoding
- Fewer buffers = lower latency but potential frame drops
- Recommended values:
  - Low latency: 4 buffers
  - Normal: 6 buffers
  - High quality: 8 buffers
**Example (C++)**:
```cpp
// Low latency configuration
pNVEnc->SetNumBuffers(4);
```
---

### Rate Control

#### SetRateControl / GetRateControl

Sets the rate control mode for bitrate management.

**Syntax (C++)**:
```cpp
HRESULT SetRateControl(int v);
HRESULT GetRateControl(int *v);
```

**Parameters**:
- `v`: Rate control mode
  - `0` - **CQP** (Constant Quantization Parameter) - Fixed quality
  - `1` - **VBR** (Variable Bitrate) - Variable bitrate, target quality
  - `2` - **CBR** (Constant Bitrate) - Fixed bitrate for streaming

**Returns**: `S_OK` on success.

**Rate Control Mode Details**:

| Mode | Bitrate Behavior | Use Case | Quality | File Size |
|------|------------------|----------|---------|-----------|
| **CQP** | Varies widely | Archival, highest quality | Excellent | Unpredictable |
| **VBR** | Varies moderately | File storage, YouTube | Very Good | Moderate |
| **CBR** | Constant | Live streaming, broadcasting | Good | Predictable |

**Example (C++)**:
```cpp
// Use CBR for live streaming
pNVEnc->SetRateControl(2);
pNVEnc->SetBitrate(5000000); // 5 Mbps
```

**Example (C#)**:
```csharp
// Use VBR for file recording
nvenc.SetRateControl(1);
nvenc.SetBitrate(8000000); // 8 Mbps target
```

---
### Preset Configuration
#### SetPreset / GetPreset
Sets the encoding preset which balances speed and quality.
**Syntax (C++)**:
```cpp
HRESULT SetPreset(GUID v);
HRESULT GetPreset(GUID *v);
```
**Parameters**:
- `v`: Preset GUID from NVENC SDK
**Preset Options** (typical values):
| Preset | Description | Speed | Quality | Use Case |
|--------|-------------|-------|---------|----------|
| **P1** | Fastest | ★★★★★ | ★☆☆☆☆ | Real-time low-latency |
| **P2** | Faster | ★★★★☆ | ★★☆☆☆ | Live streaming |
| **P3** | Fast | ★★★☆☆ | ★★★☆☆ | Standard streaming |
| **P4** | Medium | ★★☆☆☆ | ★★★★☆ | Balanced (recommended) |
| **P5** | Slow | ★☆☆☆☆ | ★★★★☆ | High quality streaming |
| **P6** | Slower | ☆☆☆☆☆ | ★★★★★ | Archive quality |
| **P7** | Slowest | ☆☆☆☆☆ | ★★★★★ | Maximum quality |
**Usage Notes**:
- P4 is recommended for most use cases
- P1-P2 for low-latency applications
- P6-P7 for maximum quality (slower encoding)
- Preset affects: motion estimation, lookahead, subpixel motion
**Example (C++)**:
```cpp
// Use P4 preset (balanced)
GUID presetP4 = /* GUID for P4 preset */;
pNVEnc->SetPreset(presetP4);
```
---

### Quality Parameter (QP)

#### SetQp / GetQp

Sets the quantization parameter for CQP mode.

**Syntax (C++)**:
```cpp
HRESULT SetQp(int v);
HRESULT GetQp(int *v);
```

**Parameters**:
- `v`: QP value (0-51)
  - Lower values = higher quality, larger files
  - Higher values = lower quality, smaller files
  - Typical range: 18-28

**Returns**: `S_OK` on success.

**Usage Notes**:
- Only effective when using CQP rate control mode
- Ignored in CBR/VBR modes
- Recommended values:
  - High quality: 18-22
  - Medium quality: 23-26
  - Low quality: 27-30

**Example (C++)**:
```cpp
// High quality CQP encoding
pNVEnc->SetRateControl(0); // CQP mode
pNVEnc->SetQp(20);         // High quality
```

---
### B-Frames Configuration
#### SetBFrames / GetBFrames
Sets the number of B-frames between I and P frames.
**Syntax (C++)**:
```cpp
HRESULT SetBFrames(int v);
HRESULT GetBFrames(int *v);
```
**Parameters**:
- `v`: Number of B-frames (0-4)
  - `0` - No B-frames (lowest latency)
  - `1-2` - Moderate compression improvement
  - `3-4` - Best compression (higher latency)
**Returns**: `S_OK` on success.
**Usage Notes**:
- B-frames improve compression efficiency
- More B-frames = higher latency
- Requires Turing (RTX 20XX) or newer for full support
- Recommended values:
  - Low latency: 0
  - Streaming: 2
  - Recording: 3
**Example (C++)**:
```cpp
// Low latency - disable B-frames
pNVEnc->SetBFrames(0);
// High quality recording - use B-frames
pNVEnc->SetBFrames(3);
```
---

### GOP Configuration

#### SetGOP / GetGOP

Sets the Group of Pictures (keyframe interval) size.

**Syntax (C++)**:
```cpp
HRESULT SetGOP(int v);
HRESULT GetGOP(int *v);
```

**Parameters**:
- `v`: GOP size in frames
  - Typical values: 30-300 frames
  - Frame rate × seconds = GOP size
  - Example: 60 fps × 2 seconds = 120 GOP size

**Returns**: `S_OK` on success.

**Usage Notes**:
- Smaller GOP = better seeking, larger file
- Larger GOP = better compression, poor seeking
- For streaming: 2-4 seconds (fps × 2-4)
- For recording: 5-10 seconds

**Example (C++)**:
```cpp
// 2-second GOP for 30fps streaming
pNVEnc->SetGOP(60);

// 5-second GOP for 60fps recording
pNVEnc->SetGOP(300);
```

---
### Bitrate Configuration
#### SetBitrate / GetBitrate
Sets the target bitrate for encoding.
**Syntax (C++)**:
```cpp
HRESULT SetBitrate(int v);
HRESULT GetBitrate(int *v);
```
**Parameters**:
- `v`: Bitrate in bits per second (bps)
**Returns**: `S_OK` on success.
**Recommended Bitrates**:
| Resolution | Framerate | Bitrate (H.264) | Bitrate (H.265) |
|------------|-----------|-----------------|-----------------|
| 720p | 30 fps | 2.5-4 Mbps | 1.5-2.5 Mbps |
| 720p | 60 fps | 4-6 Mbps | 2.5-4 Mbps |
| 1080p | 30 fps | 4-6 Mbps | 2.5-4 Mbps |
| 1080p | 60 fps | 8-12 Mbps | 5-8 Mbps |
| 1440p | 30 fps | 10-15 Mbps | 6-10 Mbps |
| 1440p | 60 fps | 15-25 Mbps | 10-15 Mbps |
| 4K | 30 fps | 25-40 Mbps | 15-25 Mbps |
| 4K | 60 fps | 45-70 Mbps | 30-45 Mbps |
**Example (C++)**:
```cpp
// 1080p @ 60fps streaming
pNVEnc->SetBitrate(10000000); // 10 Mbps
```
---

### VBV Buffer Configuration

#### SetVbvBitrate / GetVbvBitrate

Sets the VBV (Video Buffering Verifier) buffer bitrate.

**Syntax (C++)**:
```cpp
HRESULT SetVbvBitrate(int v);
HRESULT GetVbvBitrate(int *v);
```

**Parameters**:
- `v`: VBV bitrate in bps (usually same as or higher than target bitrate)

**Usage Notes**:
- Controls maximum bitrate spikes
- Typically set to 1.0-1.5× target bitrate
- Important for streaming to prevent buffer underruns

---
#### SetVbvSize / GetVbvSize
Sets the VBV buffer size.
**Syntax (C++)**:
```cpp
HRESULT SetVbvSize(int v);
HRESULT GetVbvSize(int *v);
```
**Parameters**:
- `v`: VBV buffer size in bits
**Usage Notes**:
- Larger buffer = smoother bitrate but higher latency
- Smaller buffer = lower latency but more bitrate variance
- Typical: 1-2 seconds of video at target bitrate
**Example (C++)**:
```cpp
// 10 Mbps stream with 2-second buffer
pNVEnc->SetBitrate(10000000);
pNVEnc->SetVbvBitrate(12000000);  // 1.2× bitrate
pNVEnc->SetVbvSize(20000000);     // 2 seconds
```
---

### Profile Configuration

#### SetProfile / GetProfile

Sets the H.264/H.265 encoding profile.

**Syntax (C++)**:
```cpp
HRESULT SetProfile(GUID v);
HRESULT GetProfile(GUID *v);
```

**Parameters**:
- `v`: Profile GUID

**H.264 Profiles**:
- **Baseline** - Basic features, mobile compatibility
- **Main** - Standard features, most devices
- **High** - Advanced features, HD/4K content

**H.265 Profiles**:
- **Main** - 8-bit, 4:2:0
- **Main 10** - 10-bit, HDR support

**Usage Notes**:
- Use High profile for H.264 in most cases
- Use Main profile for maximum compatibility
- HEVC Main 10 for HDR content

---
### Level Configuration
#### SetLevel / GetLevel
Sets the profile level (resolution/bitrate constraints).
**Syntax (C++)**:
```cpp
HRESULT SetLevel(int v);
HRESULT GetLevel(int *v);
```
**Parameters**:
- `v`: Level value (see H.264/H.265 level table)
**Common H.264 Levels**:
- **30** (3.0) - SD video
- **31** (3.1) - 720p @ 30fps
- **40** (4.0) - 1080p @ 30fps
- **41** (4.1) - 1080p @ 60fps
- **50** (5.0) - 4K @ 30fps
- **51** (5.1) - 4K @ 60fps
**Example (C++)**:
```cpp
// 1080p @ 60fps
pNVEnc->SetLevel(41);
```
---

### Codec Selection

#### SetCodec / GetCodec

Sets the video codec to use.

**Syntax (C++)**:
```cpp
HRESULT SetCodec(int v);
HRESULT GetCodec(int *v);
```

**Parameters**:
- `v`: Codec type
  - `0` - H.264/AVC
  - `1` - H.265/HEVC

**Returns**: `S_OK` on success.

**Usage Notes**:
- H.264 for maximum compatibility
- H.265 for better compression (40-50% smaller files)
- H.265 requires Maxwell (GTX 900) or newer GPU

**Example (C++)**:
```cpp
// Use H.265
pNVEnc->SetCodec(1);
```

---
## INVEncConfig2 Methods
### CheckNVENCAvailable
Checks if NVENC hardware encoding is available on the system.
**Syntax (C++)**:
```cpp
HRESULT CheckNVENCAvailable(BOOL* result, int* status);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int CheckNVENCAvailable([MarshalAs(UnmanagedType.Bool)] out bool result, out int status);
```
**Parameters**:
- `result`: Receives `TRUE` if NVENC is available, `FALSE` otherwise
- `status`: Receives NVENC status code (vendor-specific)
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Call this before attempting to use NVENC encoder
- Returns `FALSE` if:
  - No NVIDIA GPU is present
  - GPU doesn't support NVENC (pre-Kepler)
  - NVIDIA drivers are not installed
  - NVENC library is not available
- The status code provides additional diagnostic information
**Example (C++)**:
```cpp
#include "Intf2.h"
HRESULT CheckNVENCSupport(IBaseFilter* pEncoder)
{
    HRESULT hr;
    INVEncConfig2* pNVEnc2 = nullptr;
    hr = pEncoder->QueryInterface(IID_INVEncConfig2, (void**)&pNVEnc2);
    if (FAILED(hr))
    {
        // INVEncConfig2 not supported by this filter
        return hr;
    }
    BOOL available = FALSE;
    int status = 0;
    hr = pNVEnc2->CheckNVENCAvailable(&available, &status);
    if (SUCCEEDED(hr))
    {
        if (available)
        {
            printf("NVENC is available (status: %d)\n", status);
            // Proceed with NVENC configuration
        }
        else
        {
            printf("NVENC not available (status: %d)\n", status);
            // Fall back to software encoder
        }
    }
    pNVEnc2->Release();
    return hr;
}
```
**Example (C#)**:
```csharp
using VisioForge.DirectShowAPI;
public bool IsNVENCAvailable(IBaseFilter encoder)
{
    var nvenc2 = encoder as INVEncConfig2;
    if (nvenc2 == null)
    {
        // INVEncConfig2 not supported
        return false;
    }
    bool available;
    int status;
    int hr = nvenc2.CheckNVENCAvailable(out available, out status);
    if (hr == 0)
    {
        if (available)
        {
            Console.WriteLine($"NVENC is available (status: {status})");
            return true;
        }
        else
        {
            Console.WriteLine($"NVENC not available (status: {status})");
            return false;
        }
    }
    return false;
}
```
**Example (Delphi)**:
```delphi
function CheckNVENCSupport(Encoder: IBaseFilter): Boolean;
var
  NVEnc2: INVEncConfig2;
  Available: BOOL;
  Status: Integer;
  hr: HRESULT;
begin
  Result := False;
  if Succeeded(Encoder.QueryInterface(IID_INVEncConfig2, NVEnc2)) then
  begin
    hr := NVEnc2.CheckNVENCAvailable(Available, Status);
    if Succeeded(hr) then
    begin
      if Available then
      begin
        WriteLn(Format('NVENC is available (status: %d)', [Status]));
        Result := True;
      end
      else
      begin
        WriteLn(Format('NVENC not available (status: %d)', [Status]));
      end;
    end;
    NVEnc2 := nil;
  end;
end;
```
---

## Complete Configuration Examples

### Example 1: Low Latency Streaming (C++)

```cpp
#include "Intf.h"

HRESULT ConfigureLowLatencyNVENC(IBaseFilter* pEncoder)
{
    HRESULT hr;
    INVEncConfig* pNVEnc = nullptr;

    hr = pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
    if (FAILED(hr))
        return hr;

    // Basic configuration
    pNVEnc->SetDeviceType(0);           // First GPU
    pNVEnc->SetCodec(0);                // H.264
    pNVEnc->SetPictureStructure(0);     // Progressive

    // Low latency settings
    pNVEnc->SetRateControl(2);          // CBR
    pNVEnc->SetBitrate(5000000);        // 5 Mbps
    pNVEnc->SetBFrames(0);              // No B-frames
    pNVEnc->SetGOP(60);                 // 2-second GOP (30fps)
    pNVEnc->SetNumBuffers(4);           // Minimal buffering

    // Fast preset
    GUID presetP2 = /* P2 GUID */;
    pNVEnc->SetPreset(presetP2);

    // Profile/Level for 1080p30
    GUID highProfile = /* High Profile GUID */;
    pNVEnc->SetProfile(highProfile);
    pNVEnc->SetLevel(40);               // Level 4.0

    pNVEnc->Release();
    return S_OK;
}
```

### Example 2: High Quality Recording (C#)

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class NVENCHighQualityRecording
{
    public void ConfigureNVENC(IBaseFilter encoder)
    {
        var nvenc = encoder as INVEncConfig;
        if (nvenc == null)
            throw new NotSupportedException("NVENC not available");

        // Basic configuration
        nvenc.SetDeviceType(0);          // First GPU
        nvenc.SetCodec(1);               // H.265 for better compression
        nvenc.SetPictureStructure(0);    // Progressive

        // High quality VBR settings
        nvenc.SetRateControl(1);         // VBR
        nvenc.SetBitrate(15000000);      // 15 Mbps average
        nvenc.SetBFrames(3);             // Use B-frames
        nvenc.SetGOP(300);               // 5-second GOP (60fps)
        nvenc.SetNumBuffers(8);          // More buffering for quality

        // Quality preset
        Guid presetP6 = /* P6 GUID */;
        nvenc.SetPreset(presetP6);

        // HEVC Main profile for 4K
        Guid hevcMain = /* HEVC Main GUID */;
        nvenc.SetProfile(hevcMain);
        nvenc.SetLevel(51);              // Level 5.1 for 4K60

        // VBV configuration
        nvenc.SetVbvBitrate(20000000);   // 20 Mbps max
        nvenc.SetVbvSize(30000000);      // 2-second buffer
    }
}
```

### Example 3: Balanced Streaming (C++)

```cpp
HRESULT ConfigureBalancedStreaming(IBaseFilter* pEncoder)
{
    INVEncConfig* pNVEnc = nullptr;
    pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);

    // Device and codec
    pNVEnc->SetDeviceType(0);
    pNVEnc->SetCodec(0);                // H.264 for compatibility

    // Balanced CBR streaming
    pNVEnc->SetRateControl(2);          // CBR
    pNVEnc->SetBitrate(8000000);        // 8 Mbps
    pNVEnc->SetBFrames(2);              // Moderate B-frames
    pNVEnc->SetGOP(120);                // 2-second GOP (60fps)
    pNVEnc->SetNumBuffers(6);           // Standard buffering

    // Balanced preset P4
    GUID presetP4 = /* P4 GUID */;
    pNVEnc->SetPreset(presetP4);

    // 1080p60 profile/level
    GUID highProfile = /* High Profile GUID */;
    pNVEnc->SetProfile(highProfile);
    pNVEnc->SetLevel(41);

    // VBV for streaming
    pNVEnc->SetVbvBitrate(10000000);    // 1.25× bitrate
    pNVEnc->SetVbvSize(16000000);       // 2-second buffer

    pNVEnc->Release();
    return S_OK;
}
```

---
## Best Practices
### General Recommendations
1. **Use P4 preset as default** - Best balance of quality and performance
2. **CBR for streaming** - Predictable bitrate for network delivery
3. **VBR for recording** - Better quality for file storage
4. **Disable B-frames for low latency** - Reduces encoding delay
5. **Match GOP to framerate** - 2-4 seconds typical (fps × 2-4)
### Quality Optimization
1. **Higher preset = better quality** - Use P5-P7 when encoding time allows
2. **More B-frames = better compression** - Use 3 for recording
3. **Appropriate bitrate** - Don't go too low, quality suffers significantly
4. **VBV buffer size** - 1-2 seconds at target bitrate
### Performance Optimization
1. **Lower preset = faster encoding** - Use P1-P3 for real-time
2. **Disable B-frames** - Reduces latency and complexity
3. **Fewer encoding buffers** - Lower latency but potential drops
4. **Select appropriate GPU** - Use SetDeviceType() for multi-GPU systems
### Compatibility
1. **Use H.264 High profile** - Maximum compatibility
2. **Set correct level** - Match resolution and framerate
3. **CBR for streaming** - More compatible with players/servers
4. **Standard GOP size** - 2-4 seconds
---

## Troubleshooting

### Issue: NVENC Not Available

**Symptoms**: QueryInterface fails for INVEncConfig

**Solutions**:
- Verify NVIDIA GPU is installed
- Check GPU generation (Kepler or newer required)
- Update NVIDIA drivers to latest version
- Verify DirectShow filter is registered

### Issue: Poor Quality Output

**Solutions**:
```cpp
// Increase bitrate
pNVEnc->SetBitrate(15000000);  // Higher bitrate

// Use better preset
pNVEnc->SetPreset(presetP6);   // Slower but better

// Add B-frames
pNVEnc->SetBFrames(3);         // Better compression
```

### Issue: High Latency

**Solutions**:
```cpp
// Disable B-frames
pNVEnc->SetBFrames(0);

// Use faster preset
pNVEnc->SetPreset(presetP1);

// Reduce buffers
pNVEnc->SetNumBuffers(4);

// Smaller GOP
pNVEnc->SetGOP(30);  // 1 second at 30fps
```

### Issue: Bitrate Spikes

**Solutions**:
```cpp
// Use CBR instead of VBR
pNVEnc->SetRateControl(2);

// Configure VBV properly
pNVEnc->SetVbvBitrate(bitrate * 1.2);
pNVEnc->SetVbvSize(bitrate * 2);
```

---
## Performance Benchmarks
### Typical Encoding Performance
| Resolution | Preset | GPU Generation | FPS (approx) |
|------------|--------|----------------|--------------|
| 1080p | P1 | Pascal+ | 200-300 |
| 1080p | P4 | Pascal+ | 150-200 |
| 1080p | P7 | Pascal+ | 60-100 |
| 4K | P1 | Turing+ | 90-120 |
| 4K | P4 | Turing+ | 60-90 |
| 4K | P7 | Turing+ | 30-50 |
### Quality Comparison (PSNR)
| Preset | Quality vs x264 | Speed vs x264 |
|--------|-----------------|---------------|
| P1 | -2 dB | 100× faster |
| P4 | -0.5 dB | 50× faster |
| P7 | ≈ equal | 20× faster |
---

## Related Interfaces

- **IAMVideoCompression** - Base DirectShow compression interface
- **IBaseFilter** - DirectShow filter base interface
- **IMediaControl** - Graph control (run, stop)

## See Also

- [Encoding Filters Pack Overview](../index.md)
- [Codecs Reference](../codecs-reference.md)
- [Code Examples](../examples.md)
- [NVIDIA NVENC Documentation](https://developer.nvidia.com/video-codec-sdk)
