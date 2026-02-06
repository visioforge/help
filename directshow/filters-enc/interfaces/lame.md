---
title: LAME MP3 Encoder - Interface Reference
description: IAudioEncoderProperties interface for LAME MP3 encoding with variable and constant bitrate modes and quality configuration.
---

# LAME MP3 Encoder Interface Reference

## Overview

The `IAudioEncoderProperties` interface provides comprehensive control over LAME MP3 audio encoding. LAME (LAME Ain't an MP3 Encoder) is a high-quality MP3 encoder that produces excellent audio quality with efficient compression.

This interface allows configuration of bitrate, quality, variable bitrate (VBR) settings, and various encoding flags for optimal MP3 output.

## Interface Definition

- **Interface Name**: `IAudioEncoderProperties`
- **GUID**: `{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}`
- **Inherits From**: `IUnknown`

## Interface Definitions

### C# Definition

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// LAME MP3 encoder interface.
    /// </summary>
    /// <remarks>
    /// Configuring MPEG audio encoder parameters with unspecified
    /// input stream type may lead to misbehavior and confusing
    /// results. In most cases the specified parameters will be
    /// overridden by defaults for the input media type.
    /// To achieve proper results use this interface on the
    /// audio encoder filter with input pin connected to a valid source.
    /// </remarks>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("595EB9D1-F454-41AD-A1FA-EC232AD9DA52")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEncoderProperties
    {
        // PES Output Control
        [PreserveSig]
        int get_PESOutputEnabled(out int dwEnabled);

        [PreserveSig]
        int set_PESOutputEnabled([In] int dwEnabled);

        // Bitrate Configuration
        [PreserveSig]
        int get_Bitrate(out int dwBitrate);

        [PreserveSig]
        int set_Bitrate([In] int dwBitrate);

        // Variable Bitrate (VBR)
        [PreserveSig]
        int get_Variable(out int dwVariable);

        [PreserveSig]
        int set_Variable([In] int dwVariable);

        [PreserveSig]
        int get_VariableMin(out int dwmin);

        [PreserveSig]
        int set_VariableMin([In] int dwmin);

        [PreserveSig]
        int get_VariableMax(out int dwmax);

        [PreserveSig]
        int set_VariableMax([In] int dwmax);

        // Quality Settings
        [PreserveSig]
        int get_Quality(out int dwQuality);

        [PreserveSig]
        int set_Quality([In] int dwQuality);

        [PreserveSig]
        int get_VariableQ(out int dwVBRq);

        [PreserveSig]
        int set_VariableQ([In] int dwVBRq);

        // Source Information
        [PreserveSig]
        int get_SourceSampleRate(out int dwSampleRate);

        [PreserveSig]
        int get_SourceChannels(out int dwChannels);

        // Output Configuration
        [PreserveSig]
        int get_SampleRate(out int dwSampleRate);

        [PreserveSig]
        int set_SampleRate([In] int dwSampleRate);

        [PreserveSig]
        int get_ChannelMode(out int dwChannelMode);

        [PreserveSig]
        int set_ChannelMode([In] int dwChannelMode);

        // Flags
        [PreserveSig]
        int get_CRCFlag(out int dwFlag);

        [PreserveSig]
        int set_CRCFlag([In] int dwFlag);

        [PreserveSig]
        int get_OriginalFlag(out int dwFlag);

        [PreserveSig]
        int set_OriginalFlag([In] int dwFlag);

        [PreserveSig]
        int get_CopyrightFlag(out int dwFlag);

        [PreserveSig]
        int set_CopyrightFlag([In] int dwFlag);

        [PreserveSig]
        int get_EnforceVBRmin(out int dwFlag);

        [PreserveSig]
        int set_EnforceVBRmin([In] int dwFlag);

        [PreserveSig]
        int get_VoiceMode(out int dwFlag);

        [PreserveSig]
        int set_VoiceMode([In] int dwFlag);

        [PreserveSig]
        int get_KeepAllFreq(out int dwFlag);

        [PreserveSig]
        int set_KeepAllFreq([In] int dwFlag);

        [PreserveSig]
        int get_StrictISO(out int dwFlag);

        [PreserveSig]
        int set_StrictISO([In] int dwFlag);

        [PreserveSig]
        int get_NoShortBlock(out int dwDisable);

        [PreserveSig]
        int set_NoShortBlock([In] int dwDisable);

        [PreserveSig]
        int get_XingTag(out int dwXingTag);

        [PreserveSig]
        int set_XingTag([In] int dwXingTag);

        [PreserveSig]
        int get_ForceMS(out int dwFlag);

        [PreserveSig]
        int set_ForceMS([In] int dwFlag);

        [PreserveSig]
        int get_ModeFixed(out int dwFlag);

        [PreserveSig]
        int set_ModeFixed([In] int dwFlag);

        // Configuration Management
        [PreserveSig]
        int get_ParameterBlockSize(out byte pcBlock, out int pdwSize);

        [PreserveSig]
        int set_ParameterBlockSize([In] byte pcBlock, [In] int dwSize);

        [PreserveSig]
        int DefaultAudioEncoderProperties();

        [PreserveSig]
        int LoadAudioEncoderPropertiesFromRegistry();

        [PreserveSig]
        int SaveAudioEncoderPropertiesToRegistry();

        [PreserveSig]
        int InputTypeDefined();
    }
}
```

### C++ Definition

```cpp
#include <unknwn.h>

// {595EB9D1-F454-41AD-A1FA-EC232AD9DA52}
static const GUID IID_IAudioEncoderProperties =
{ 0x595eb9d1, 0xf454, 0x41ad, { 0xa1, 0xfa, 0xec, 0x23, 0x2a, 0xd9, 0xda, 0x52 } };

DECLARE_INTERFACE_(IAudioEncoderProperties, IUnknown)
{
    // PES Output
    STDMETHOD(get_PESOutputEnabled)(THIS_ int* dwEnabled) PURE;
    STDMETHOD(set_PESOutputEnabled)(THIS_ int dwEnabled) PURE;

    // Bitrate
    STDMETHOD(get_Bitrate)(THIS_ int* dwBitrate) PURE;
    STDMETHOD(set_Bitrate)(THIS_ int dwBitrate) PURE;

    // Variable Bitrate
    STDMETHOD(get_Variable)(THIS_ int* dwVariable) PURE;
    STDMETHOD(set_Variable)(THIS_ int dwVariable) PURE;
    STDMETHOD(get_VariableMin)(THIS_ int* dwmin) PURE;
    STDMETHOD(set_VariableMin)(THIS_ int dwmin) PURE;
    STDMETHOD(get_VariableMax)(THIS_ int* dwmax) PURE;
    STDMETHOD(set_VariableMax)(THIS_ int dwmax) PURE;

    // Quality
    STDMETHOD(get_Quality)(THIS_ int* dwQuality) PURE;
    STDMETHOD(set_Quality)(THIS_ int dwQuality) PURE;
    STDMETHOD(get_VariableQ)(THIS_ int* dwVBRq) PURE;
    STDMETHOD(set_VariableQ)(THIS_ int dwVBRq) PURE;

    // Source Information
    STDMETHOD(get_SourceSampleRate)(THIS_ int* dwSampleRate) PURE;
    STDMETHOD(get_SourceChannels)(THIS_ int* dwChannels) PURE;

    // Output Configuration
    STDMETHOD(get_SampleRate)(THIS_ int* dwSampleRate) PURE;
    STDMETHOD(set_SampleRate)(THIS_ int dwSampleRate) PURE;
    STDMETHOD(get_ChannelMode)(THIS_ int* dwChannelMode) PURE;
    STDMETHOD(set_ChannelMode)(THIS_ int dwChannelMode) PURE;

    // Flags
    STDMETHOD(get_CRCFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_CRCFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_OriginalFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_OriginalFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_CopyrightFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_CopyrightFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_EnforceVBRmin)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_EnforceVBRmin)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_VoiceMode)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_VoiceMode)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_KeepAllFreq)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_KeepAllFreq)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_StrictISO)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_StrictISO)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_NoShortBlock)(THIS_ int* dwDisable) PURE;
    STDMETHOD(set_NoShortBlock)(THIS_ int dwDisable) PURE;
    STDMETHOD(get_XingTag)(THIS_ int* dwXingTag) PURE;
    STDMETHOD(set_XingTag)(THIS_ int dwXingTag) PURE;
    STDMETHOD(get_ForceMS)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_ForceMS)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_ModeFixed)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_ModeFixed)(THIS_ int dwFlag) PURE;

    // Configuration Management
    STDMETHOD(get_ParameterBlockSize)(THIS_ byte* pcBlock, int* pdwSize) PURE;
    STDMETHOD(set_ParameterBlockSize)(THIS_ byte* pcBlock, int dwSize) PURE;
    STDMETHOD(DefaultAudioEncoderProperties)(THIS) PURE;
    STDMETHOD(LoadAudioEncoderPropertiesFromRegistry)(THIS) PURE;
    STDMETHOD(SaveAudioEncoderPropertiesToRegistry)(THIS) PURE;
    STDMETHOD(InputTypeDefined)(THIS) PURE;
};
```

### Delphi Definition

```delphi
uses
  ActiveX, ComObj;

const
  IID_IAudioEncoderProperties: TGUID = '{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}';

type
  IAudioEncoderProperties = interface(IUnknown)
    ['{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}']

    // PES Output
    function get_PESOutputEnabled(out dwEnabled: Integer): HRESULT; stdcall;
    function set_PESOutputEnabled(dwEnabled: Integer): HRESULT; stdcall;

    // Bitrate
    function get_Bitrate(out dwBitrate: Integer): HRESULT; stdcall;
    function set_Bitrate(dwBitrate: Integer): HRESULT; stdcall;

    // Variable Bitrate
    function get_Variable(out dwVariable: Integer): HRESULT; stdcall;
    function set_Variable(dwVariable: Integer): HRESULT; stdcall;
    function get_VariableMin(out dwmin: Integer): HRESULT; stdcall;
    function set_VariableMin(dwmin: Integer): HRESULT; stdcall;
    function get_VariableMax(out dwmax: Integer): HRESULT; stdcall;
    function set_VariableMax(dwmax: Integer): HRESULT; stdcall;

    // Quality
    function get_Quality(out dwQuality: Integer): HRESULT; stdcall;
    function set_Quality(dwQuality: Integer): HRESULT; stdcall;
    function get_VariableQ(out dwVBRq: Integer): HRESULT; stdcall;
    function set_VariableQ(dwVBRq: Integer): HRESULT; stdcall;

    // Source Information
    function get_SourceSampleRate(out dwSampleRate: Integer): HRESULT; stdcall;
    function get_SourceChannels(out dwChannels: Integer): HRESULT; stdcall;

    // Output Configuration
    function get_SampleRate(out dwSampleRate: Integer): HRESULT; stdcall;
    function set_SampleRate(dwSampleRate: Integer): HRESULT; stdcall;
    function get_ChannelMode(out dwChannelMode: Integer): HRESULT; stdcall;
    function set_ChannelMode(dwChannelMode: Integer): HRESULT; stdcall;

    // Flags
    function get_CRCFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_CRCFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_OriginalFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_OriginalFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_CopyrightFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_CopyrightFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_EnforceVBRmin(out dwFlag: Integer): HRESULT; stdcall;
    function set_EnforceVBRmin(dwFlag: Integer): HRESULT; stdcall;
    function get_VoiceMode(out dwFlag: Integer): HRESULT; stdcall;
    function set_VoiceMode(dwFlag: Integer): HRESULT; stdcall;
    function get_KeepAllFreq(out dwFlag: Integer): HRESULT; stdcall;
    function set_KeepAllFreq(dwFlag: Integer): HRESULT; stdcall;
    function get_StrictISO(out dwFlag: Integer): HRESULT; stdcall;
    function set_StrictISO(dwFlag: Integer): HRESULT; stdcall;
    function get_NoShortBlock(out dwDisable: Integer): HRESULT; stdcall;
    function set_NoShortBlock(dwDisable: Integer): HRESULT; stdcall;
    function get_XingTag(out dwXingTag: Integer): HRESULT; stdcall;
    function set_XingTag(dwXingTag: Integer): HRESULT; stdcall;
    function get_ForceMS(out dwFlag: Integer): HRESULT; stdcall;
    function set_ForceMS(dwFlag: Integer): HRESULT; stdcall;
    function get_ModeFixed(out dwFlag: Integer): HRESULT; stdcall;
    function set_ModeFixed(dwFlag: Integer): HRESULT; stdcall;

    // Configuration Management
    function get_ParameterBlockSize(out pcBlock: Byte; out pdwSize: Integer): HRESULT; stdcall;
    function set_ParameterBlockSize(pcBlock: Byte; dwSize: Integer): HRESULT; stdcall;
    function DefaultAudioEncoderProperties: HRESULT; stdcall;
    function LoadAudioEncoderPropertiesFromRegistry: HRESULT; stdcall;
    function SaveAudioEncoderPropertiesToRegistry: HRESULT; stdcall;
    function InputTypeDefined: HRESULT; stdcall;
  end;
```

---
## Methods Reference
### Bitrate Configuration
#### set_Bitrate / get_Bitrate
Sets or retrieves the target compression bitrate in Kbits/s.
**Parameters**:
- `dwBitrate`: Bitrate in kilobits per second
**Common MP3 Bitrates**:
- **320 kbps** - Highest quality, near-transparent
- **256 kbps** - Very high quality
- **192 kbps** - High quality (recommended for music)
- **128 kbps** - Standard quality (acceptable for most content)
- **96 kbps** - Lower quality, smaller files
- **64 kbps** - Voice/podcast quality
**Example (C#)**:
```csharp
var lame = audioEncoder as IAudioEncoderProperties;
if (lame != null)
{
    // Set high quality 192 kbps
    lame.set_Bitrate(192);
}
```
---

### Variable Bitrate (VBR)

#### set_Variable / get_Variable

Enables or disables variable bitrate mode.

**Parameters**:
- `dwVariable`: 1 to enable VBR, 0 to disable (CBR mode)

**Usage Notes**:
- VBR provides better quality-to-size ratio than CBR
- VBR allocates more bits to complex audio passages
- CBR provides predictable file sizes
- VBR is recommended for music archival

#### set_VariableMin / get_VariableMin

Sets the minimum bitrate for VBR mode.

**Parameters**:
- `dwmin`: Minimum bitrate in kbps

#### set_VariableMax / get_VariableMax

Sets the maximum bitrate for VBR mode.

**Parameters**:
- `dwmax`: Maximum bitrate in kbps

**Example (C#)**:
```csharp
// Enable VBR with 128-256 kbps range
lame.set_Variable(1);
lame.set_VariableMin(128);
lame.set_VariableMax(256);
lame.set_VariableQ(4); // VBR quality level
```

---
### Quality Settings
#### set_Quality / get_Quality
Sets the encoding quality for CBR mode.
**Parameters**:
- `dwQuality`: Quality level (0-9)
  - **0** - Highest quality (slowest)
  - **2** - Near-highest quality (recommended)
  - **5** - Good quality/speed balance
  - **7** - Faster encoding, lower quality
  - **9** - Lowest quality (fastest)
**Example (C++)**:
```cpp
IAudioEncoderProperties* pLame = nullptr;
pFilter->QueryInterface(IID_IAudioEncoderProperties, (void**)&pLame);
// High quality CBR encoding
pLame->set_Bitrate(192);
pLame->set_Quality(2);
pLame->Release();
```
#### set_VariableQ / get_VariableQ
Sets the quality level for VBR mode.
**Parameters**:
- `dwVBRq`: VBR quality (0-9)
  - **0** - Highest quality (~245 kbps)
  - **2** - Very high quality (~190 kbps)
  - **4** - High quality (~165 kbps) - recommended
  - **6** - Medium quality (~130 kbps)
  - **9** - Lowest quality (~65 kbps)
---

### Channel Mode

#### set_ChannelMode / get_ChannelMode

Sets the stereo encoding mode.

**Parameters**:
- `dwChannelMode`: Channel mode value
  - **0** - Stereo
  - **1** - Joint Stereo (recommended)
  - **2** - Dual Channel
  - **3** - Mono

**Usage Notes**:
- Joint Stereo provides best quality at lower bitrates
- Use Stereo for critical listening at high bitrates
- Mono reduces file size for speech/podcasts

**Example (C#)**:
```csharp
// Joint stereo for music at 192 kbps
lame.set_ChannelMode(1);
lame.set_Bitrate(192);
```

---
### Encoding Flags
#### set_CRCFlag / get_CRCFlag
Enables CRC error protection.
**Parameters**:
- `dwFlag`: 1 to enable, 0 to disable
**Usage**: Adds error detection, minimal size increase (~0.2%)
#### set_CopyrightFlag / get_CopyrightFlag
Sets the copyright flag in MP3 header.
**Parameters**:
- `dwFlag`: 1 if copyrighted, 0 otherwise
#### set_OriginalFlag / get_OriginalFlag
Sets the original/copy flag.
**Parameters**:
- `dwFlag`: 1 for original, 0 for copy
#### set_VoiceMode / get_VoiceMode
Optimizes encoding for voice content.
**Parameters**:
- `dwFlag`: 1 to enable voice optimization
**Usage**: Improves quality for speech at lower bitrates
**Example (C#)**:
```csharp
// Optimize for podcast/voice content
lame.set_VoiceMode(1);
lame.set_Bitrate(64);
lame.set_ChannelMode(3); // Mono
```
#### set_XingTag / get_XingTag
Adds Xing VBR tag for accurate seeking.
**Parameters**:
- `dwFlag`: 1 to add tag (recommended for VBR)
**Usage**: Essential for VBR files to enable proper seeking
---

## Configuration Management

### SaveAudioEncoderPropertiesToRegistry

Saves the current encoder configuration to the registry.

**Usage Notes**:
- Must be called after changing properties
- Settings persist between sessions
- Requires appropriate registry permissions

### LoadAudioEncoderPropertiesFromRegistry

Loads encoder configuration from the registry.

### DefaultAudioEncoderProperties

Resets all encoder properties to default values based on input stream type.

### InputTypeDefined

Checks if the input format has been specified.

**Returns**:
- `S_OK` - Input type is defined, encoder can be configured
- `E_FAIL` - Input type not specified, configuration may fail

---
## Complete Examples
### Example 1: High Quality Music Encoding (C#)
```csharp
using VisioForge.DirectShowAPI;
public void ConfigureHighQualityMP3(IBaseFilter audioEncoder)
{
    var lame = audioEncoder as IAudioEncoderProperties;
    if (lame == null)
        return;
    // Check if input is connected
    if (lame.InputTypeDefined() != 0)
    {
        Console.WriteLine("Warning: Input not connected, using defaults");
    }
    // High quality VBR settings
    lame.set_Variable(1);              // Enable VBR
    lame.set_VariableQ(2);             // Very high quality
    lame.set_VariableMin(192);         // Min 192 kbps
    lame.set_VariableMax(320);         // Max 320 kbps
    // Joint stereo for efficiency
    lame.set_ChannelMode(1);
    // Quality flags
    lame.set_XingTag(1);               // Add VBR tag
    lame.set_OriginalFlag(1);          // Mark as original
    lame.set_CopyrightFlag(1);         // Set copyright
    // Save settings
    lame.SaveAudioEncoderPropertiesToRegistry();
}
```
### Example 2: Podcast/Voice Encoding (C++)
```cpp
#include "LAME.h"
HRESULT ConfigurePodcastMP3(IBaseFilter* pAudioEncoder)
{
    HRESULT hr;
    IAudioEncoderProperties* pLame = nullptr;
    hr = pAudioEncoder->QueryInterface(IID_IAudioEncoderProperties,
                                       (void**)&pLame);
    if (FAILED(hr))
        return hr;
    // Voice-optimized settings
    pLame->set_VoiceMode(1);           // Voice optimization
    pLame->set_Bitrate(64);            // 64 kbps for speech
    pLame->set_Quality(5);             // Balanced quality
    pLame->set_ChannelMode(3);         // Mono
    // Disable VBR for predictable file size
    pLame->set_Variable(0);
    // Add Xing tag for compatibility
    pLame->set_XingTag(1);
    // Save configuration
    pLame->SaveAudioEncoderPropertiesToRegistry();
    pLame->Release();
    return S_OK;
}
```
### Example 3: Standard Music Encoding (Delphi)
```delphi
procedure ConfigureStandardMP3(AudioEncoder: IBaseFilter);
var
  Lame: IAudioEncoderProperties;
  hr: HRESULT;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IAudioEncoderProperties, Lame)) then
  begin
    // Standard VBR music settings
    Lame.set_Variable(1);              // Enable VBR
    Lame.set_VariableQ(4);             // High quality (~165 kbps avg)
    Lame.set_VariableMin(128);         // Min 128 kbps
    Lame.set_VariableMax(256);         // Max 256 kbps
    // Joint stereo
    Lame.set_ChannelMode(1);
    // Essential flags
    Lame.set_XingTag(1);               // VBR tag for seeking
    // Save to registry
    Lame.SaveAudioEncoderPropertiesToRegistry;
    Lame := nil;
  end;
end;
```
---

## Best Practices

### Quality Recommendations

1. **Music Archival**: VBR Q0-Q2 (245-190 kbps average)
2. **Music Distribution**: VBR Q4 (165 kbps) or CBR 192 kbps
3. **Streaming**: CBR 128 kbps
4. **Podcasts/Speech**: CBR 64 kbps mono with voice mode

### Performance Tips

1. **Use Joint Stereo** at bitrates below 192 kbps
2. **Enable VBR** for better quality-to-size ratio
3. **Add Xing Tag** for VBR files
4. **Use Voice Mode** for speech content at <96 kbps

### Configuration Workflow

1. Connect input pin before configuring
2. Check `InputTypeDefined()` before setting properties
3. Configure all desired properties
4. Call `SaveAudioEncoderPropertiesToRegistry()`
5. Verify settings with get methods

---
## Troubleshooting
### Issue: Settings Not Applied
**Solution**:
```csharp
// Ensure input is connected first
if (lame.InputTypeDefined() == 0)
{
    // Configure settings
    lame.set_Bitrate(192);
    lame.SaveAudioEncoderPropertiesToRegistry();
}
else
{
    // Connect input first, then configure
}
```
### Issue: Poor Quality Output
**Solutions**:
- Increase VBR quality: `set_VariableQ(2)` or lower
- Increase CBR bitrate: `set_Bitrate(192)` or higher
- Use better quality setting: `set_Quality(2)`
- Disable voice mode for music: `set_VoiceMode(0)`
### Issue: Large File Sizes
**Solutions**:
```cpp
// Use VBR instead of high CBR
pLame->set_Variable(1);
pLame->set_VariableQ(4);        // ~165 kbps average
pLame->set_VariableMax(192);    // Cap maximum bitrate
```
---

## See Also

- [Encoding Filters Pack Overview](../index.md)
- [Audio Codecs Reference](../codecs-reference.md)
- [AAC Encoder](aac.md)
- [FLAC Encoder](flac.md)
