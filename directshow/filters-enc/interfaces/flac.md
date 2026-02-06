---
title: FLAC Encoder Interface Reference
description: FLAC encoder DirectShow interface with encoding levels, LPC configuration, block sizes, and compression for lossless audio encoding.
---

# FLAC Encoder Interface Reference

## Overview

The **IFLACEncodeSettings** interface provides complete control over FLAC (Free Lossless Audio Codec) audio encoding parameters in DirectShow filter graphs. FLAC is a lossless audio compression format that reduces file size without any loss in audio quality, making it ideal for archival, professional audio production, and high-fidelity music distribution.

This interface allows developers to configure encoding quality levels, Linear Predictive Coding (LPC) parameters, block sizes, mid-side stereo coding, and Rice partition orders to achieve optimal compression efficiency for different types of audio content.

**Interface GUID**: `{A6096781-2A65-4540-A536-011235D0A5FE}`

**Inherits From**: `IUnknown`

## Interface Definitions

### C# Definition

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// FLAC (Free Lossless Audio Codec) encoder configuration interface.
    /// Provides comprehensive control over FLAC encoding parameters for lossless audio compression.
    /// </summary>
    [ComImport]
    [Guid("A6096781-2A65-4540-A536-011235D0A5FE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFLACEncodeSettings
    {
        /// <summary>
        /// Checks if encoding settings can be modified at the current time.
        /// </summary>
        /// <returns>True if settings can be modified, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool canModifySettings();

        /// <summary>
        /// Sets the FLAC encoding level (compression quality).
        /// </summary>
        /// <param name="inLevel">Encoding level (0-8, where 8 is highest compression, slowest)</param>
        /// <returns>True if successful, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setEncodingLevel(uint inLevel);

        /// <summary>
        /// Sets the Linear Predictive Coding (LPC) order.
        /// Higher values provide better compression but slower encoding.
        /// </summary>
        /// <param name="inLPCOrder">LPC order (typically 0-32)</param>
        /// <returns>True if successful, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setLPCOrder(uint inLPCOrder);

        /// <summary>
        /// Sets the audio block size for encoding.
        /// Larger blocks can provide better compression but increase latency.
        /// </summary>
        /// <param name="inBlockSize">Block size in samples (typically 192-4608)</param>
        /// <returns>True if successful, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setBlockSize(uint inBlockSize);

        /// <summary>
        /// Enables or disables mid-side stereo coding for 2-channel audio.
        /// Can improve compression for stereo audio with correlated channels.
        /// </summary>
        /// <param name="inUseMidSideCoding">True to enable mid-side coding, false to disable</param>
        /// <returns>True if successful, false otherwise</returns>
        /// <remarks>Only applicable for 2-channel (stereo) audio</remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useMidSideCoding([In, MarshalAs(UnmanagedType.Bool)] bool inUseMidSideCoding);

        /// <summary>
        /// Enables or disables adaptive mid-side stereo coding.
        /// Automatically decides whether to use mid-side coding on a per-block basis.
        /// Overrides useMidSideCoding and is generally faster.
        /// </summary>
        /// <param name="inUseAdaptiveMidSideCoding">True to enable adaptive mid-side coding, false to disable</param>
        /// <returns>True if successful, false otherwise</returns>
        /// <remarks>Only for 2-channel audio. Overrides useMidSideCoding setting. Generally provides better performance.</remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useAdaptiveMidSideCoding([In, MarshalAs(UnmanagedType.Bool)] bool inUseAdaptiveMidSideCoding);

        /// <summary>
        /// Enables or disables exhaustive model search for best compression.
        /// Significantly slower but can provide better compression ratios.
        /// </summary>
        /// <param name="inUseExhaustiveModelSearch">True to enable exhaustive search, false to disable</param>
        /// <returns>True if successful, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useExhaustiveModelSearch([In, MarshalAs(UnmanagedType.Bool)] bool inUseExhaustiveModelSearch);

        /// <summary>
        /// Sets the Rice partition order range for entropy coding.
        /// Controls the trade-off between compression efficiency and encoding speed.
        /// </summary>
        /// <param name="inMin">Minimum Rice partition order</param>
        /// <param name="inMax">Maximum Rice partition order</param>
        /// <returns>True if successful, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setRicePartitionOrder(uint inMin, uint inMax);

        /// <summary>
        /// Gets the current encoding level setting.
        /// </summary>
        /// <returns>Current encoding level (0-8)</returns>
        [PreserveSig]
        int encoderLevel();

        /// <summary>
        /// Gets the current Linear Predictive Coding (LPC) order.
        /// </summary>
        /// <returns>Current LPC order</returns>
        [PreserveSig]
        uint LPCOrder();

        /// <summary>
        /// Gets the current block size setting.
        /// </summary>
        /// <returns>Current block size in samples</returns>
        [PreserveSig]
        uint blockSize();

        /// <summary>
        /// Gets the minimum Rice partition order.
        /// </summary>
        /// <returns>Minimum Rice partition order</returns>
        [PreserveSig]
        uint riceMin();

        /// <summary>
        /// Gets the maximum Rice partition order.
        /// </summary>
        /// <returns>Maximum Rice partition order</returns>
        [PreserveSig]
        uint riceMax();

        /// <summary>
        /// Checks if mid-side stereo coding is enabled.
        /// </summary>
        /// <returns>True if mid-side coding is enabled, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingMidSideCoding();

        /// <summary>
        /// Checks if adaptive mid-side stereo coding is enabled.
        /// </summary>
        /// <returns>True if adaptive mid-side coding is enabled, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingAdaptiveMidSideCoding();

        /// <summary>
        /// Checks if exhaustive model search is enabled.
        /// </summary>
        /// <returns>True if exhaustive model search is enabled, false otherwise</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingExhaustiveModel();
    }
}
```

### C++ Definition

```cpp
#include <unknwn.h>

// {A6096781-2A65-4540-A536-011235D0A5FE}
DEFINE_GUID(IID_IFLACEncodeSettings,
    0xa6096781, 0x2a65, 0x4540, 0xa5, 0x36, 0x01, 0x12, 0x35, 0xd0, 0xa5, 0xfe);

/// <summary>
/// FLAC (Free Lossless Audio Codec) encoder configuration interface.
/// Provides comprehensive control over FLAC encoding parameters for lossless audio compression.
/// </summary>
DECLARE_INTERFACE_(IFLACEncodeSettings, IUnknown)
{
    /// <summary>
    /// Checks if encoding settings can be modified at the current time.
    /// </summary>
    /// <returns>TRUE if settings can be modified, FALSE otherwise</returns>
    STDMETHOD_(BOOL, canModifySettings)(THIS) PURE;

    /// <summary>
    /// Sets the FLAC encoding level (compression quality).
    /// </summary>
    /// <param name="inLevel">Encoding level (0-8, where 8 is highest compression, slowest)</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    STDMETHOD_(BOOL, setEncodingLevel)(THIS_
        unsigned long inLevel
        ) PURE;

    /// <summary>
    /// Sets the Linear Predictive Coding (LPC) order.
    /// Higher values provide better compression but slower encoding.
    /// </summary>
    /// <param name="inLPCOrder">LPC order (typically 0-32)</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    STDMETHOD_(BOOL, setLPCOrder)(THIS_
        unsigned long inLPCOrder
        ) PURE;

    /// <summary>
    /// Sets the audio block size for encoding.
    /// Larger blocks can provide better compression but increase latency.
    /// </summary>
    /// <param name="inBlockSize">Block size in samples (typically 192-4608)</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    STDMETHOD_(BOOL, setBlockSize)(THIS_
        unsigned long inBlockSize
        ) PURE;

    /// <summary>
    /// Enables or disables mid-side stereo coding for 2-channel audio.
    /// Can improve compression for stereo audio with correlated channels.
    /// </summary>
    /// <param name="inUseMidSideCoding">TRUE to enable mid-side coding, FALSE to disable</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    /// <remarks>Only applicable for 2-channel (stereo) audio</remarks>
    STDMETHOD_(BOOL, useMidSideCoding)(THIS_
        BOOL inUseMidSideCoding
        ) PURE;

    /// <summary>
    /// Enables or disables adaptive mid-side stereo coding.
    /// Automatically decides whether to use mid-side coding on a per-block basis.
    /// Overrides useMidSideCoding and is generally faster.
    /// </summary>
    /// <param name="inUseAdaptiveMidSideCoding">TRUE to enable adaptive mid-side coding, FALSE to disable</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    /// <remarks>Only for 2-channel audio. Overrides useMidSideCoding setting. Generally provides better performance.</remarks>
    STDMETHOD_(BOOL, useAdaptiveMidSideCoding)(THIS_
        BOOL inUseAdaptiveMidSideCoding
        ) PURE;

    /// <summary>
    /// Enables or disables exhaustive model search for best compression.
    /// Significantly slower but can provide better compression ratios.
    /// </summary>
    /// <param name="inUseExhaustiveModelSearch">TRUE to enable exhaustive search, FALSE to disable</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    STDMETHOD_(BOOL, useExhaustiveModelSearch)(THIS_
        BOOL inUseExhaustiveModelSearch
        ) PURE;

    /// <summary>
    /// Sets the Rice partition order range for entropy coding.
    /// Controls the trade-off between compression efficiency and encoding speed.
    /// </summary>
    /// <param name="inMin">Minimum Rice partition order</param>
    /// <param name="inMax">Maximum Rice partition order</param>
    /// <returns>TRUE if successful, FALSE otherwise</returns>
    STDMETHOD_(BOOL, setRicePartitionOrder)(THIS_
        unsigned long inMin,
        unsigned long inMax
        ) PURE;

    /// <summary>
    /// Gets the current encoding level setting.
    /// </summary>
    /// <returns>Current encoding level (0-8)</returns>
    STDMETHOD_(int, encoderLevel)(THIS) PURE;

    /// <summary>
    /// Gets the current Linear Predictive Coding (LPC) order.
    /// </summary>
    /// <returns>Current LPC order</returns>
    STDMETHOD_(unsigned long, LPCOrder)(THIS) PURE;

    /// <summary>
    /// Gets the current block size setting.
    /// </summary>
    /// <returns>Current block size in samples</returns>
    STDMETHOD_(unsigned long, blockSize)(THIS) PURE;

    /// <summary>
    /// Gets the minimum Rice partition order.
    /// </summary>
    /// <returns>Minimum Rice partition order</returns>
    STDMETHOD_(unsigned long, riceMin)(THIS) PURE;

    /// <summary>
    /// Gets the maximum Rice partition order.
    /// </summary>
    /// <returns>Maximum Rice partition order</returns>
    STDMETHOD_(unsigned long, riceMax)(THIS) PURE;

    /// <summary>
    /// Checks if mid-side stereo coding is enabled.
    /// </summary>
    /// <returns>TRUE if mid-side coding is enabled, FALSE otherwise</returns>
    STDMETHOD_(BOOL, isUsingMidSideCoding)(THIS) PURE;

    /// <summary>
    /// Checks if adaptive mid-side stereo coding is enabled.
    /// </summary>
    /// <returns>TRUE if adaptive mid-side coding is enabled, FALSE otherwise</returns>
    STDMETHOD_(BOOL, isUsingAdaptiveMidSideCoding)(THIS) PURE;

    /// <summary>
    /// Checks if exhaustive model search is enabled.
    /// </summary>
    /// <returns>TRUE if exhaustive model search is enabled, FALSE otherwise</returns>
    STDMETHOD_(BOOL, isUsingExhaustiveModel)(THIS) PURE;
};
```

### Delphi Definition

```delphi
uses
  ActiveX, ComObj;

const
  IID_IFLACEncodeSettings: TGUID = '{A6096781-2A65-4540-A536-011235D0A5FE}';

type
  /// <summary>
  /// FLAC (Free Lossless Audio Codec) encoder configuration interface.
  /// Provides comprehensive control over FLAC encoding parameters for lossless audio compression.
  /// </summary>
  IFLACEncodeSettings = interface(IUnknown)
    ['{A6096781-2A65-4540-A536-011235D0A5FE}']

    /// <summary>
    /// Checks if encoding settings can be modified at the current time.
    /// </summary>
    /// <returns>True if settings can be modified, false otherwise</returns>
    function canModifySettings: BOOL; stdcall;

    /// <summary>
    /// Sets the FLAC encoding level (compression quality).
    /// </summary>
    /// <param name="inLevel">Encoding level (0-8, where 8 is highest compression, slowest)</param>
    /// <returns>True if successful, false otherwise</returns>
    function setEncodingLevel(inLevel: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Sets the Linear Predictive Coding (LPC) order.
    /// Higher values provide better compression but slower encoding.
    /// </summary>
    /// <param name="inLPCOrder">LPC order (typically 0-32)</param>
    /// <returns>True if successful, false otherwise</returns>
    function setLPCOrder(inLPCOrder: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Sets the audio block size for encoding.
    /// Larger blocks can provide better compression but increase latency.
    /// </summary>
    /// <param name="inBlockSize">Block size in samples (typically 192-4608)</param>
    /// <returns>True if successful, false otherwise</returns>
    function setBlockSize(inBlockSize: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Enables or disables mid-side stereo coding for 2-channel audio.
    /// Can improve compression for stereo audio with correlated channels.
    /// </summary>
    /// <param name="inUseMidSideCoding">True to enable mid-side coding, false to disable</param>
    /// <returns>True if successful, false otherwise</returns>
    /// <remarks>Only applicable for 2-channel (stereo) audio</remarks>
    function useMidSideCoding(inUseMidSideCoding: BOOL): BOOL; stdcall;

    /// <summary>
    /// Enables or disables adaptive mid-side stereo coding.
    /// Automatically decides whether to use mid-side coding on a per-block basis.
    /// Overrides useMidSideCoding and is generally faster.
    /// </summary>
    /// <param name="inUseAdaptiveMidSideCoding">True to enable adaptive mid-side coding, false to disable</param>
    /// <returns>True if successful, false otherwise</returns>
    /// <remarks>Only for 2-channel audio. Overrides useMidSideCoding setting. Generally provides better performance.</remarks>
    function useAdaptiveMidSideCoding(inUseAdaptiveMidSideCoding: BOOL): BOOL; stdcall;

    /// <summary>
    /// Enables or disables exhaustive model search for best compression.
    /// Significantly slower but can provide better compression ratios.
    /// </summary>
    /// <param name="inUseExhaustiveModelSearch">True to enable exhaustive search, false to disable</param>
    /// <returns>True if successful, false otherwise</returns>
    function useExhaustiveModelSearch(inUseExhaustiveModelSearch: BOOL): BOOL; stdcall;

    /// <summary>
    /// Sets the Rice partition order range for entropy coding.
    /// Controls the trade-off between compression efficiency and encoding speed.
    /// </summary>
    /// <param name="inMin">Minimum Rice partition order</param>
    /// <param name="inMax">Maximum Rice partition order</param>
    /// <returns>True if successful, false otherwise</returns>
    function setRicePartitionOrder(inMin: Cardinal; inMax: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Gets the current encoding level setting.
    /// </summary>
    /// <returns>Current encoding level (0-8)</returns>
    function encoderLevel: Integer; stdcall;

    /// <summary>
    /// Gets the current Linear Predictive Coding (LPC) order.
    /// </summary>
    /// <returns>Current LPC order</returns>
    function LPCOrder: Cardinal; stdcall;

    /// <summary>
    /// Gets the current block size setting.
    /// </summary>
    /// <returns>Current block size in samples</returns>
    function blockSize: Cardinal; stdcall;

    /// <summary>
    /// Gets the minimum Rice partition order.
    /// </summary>
    /// <returns>Minimum Rice partition order</returns>
    function riceMin: Cardinal; stdcall;

    /// <summary>
    /// Gets the maximum Rice partition order.
    /// </summary>
    /// <returns>Maximum Rice partition order</returns>
    function riceMax: Cardinal; stdcall;

    /// <summary>
    /// Checks if mid-side stereo coding is enabled.
    /// </summary>
    /// <returns>True if mid-side coding is enabled, false otherwise</returns>
    function isUsingMidSideCoding: BOOL; stdcall;

    /// <summary>
    /// Checks if adaptive mid-side stereo coding is enabled.
    /// </summary>
    /// <returns>True if adaptive mid-side coding is enabled, false otherwise</returns>
    function isUsingAdaptiveMidSideCoding: BOOL; stdcall;

    /// <summary>
    /// Checks if exhaustive model search is enabled.
    /// </summary>
    /// <returns>True if exhaustive model search is enabled, false otherwise</returns>
    function isUsingExhaustiveModel: BOOL; stdcall;
  end;
```

## Method Reference

### Configuration Check

#### canModifySettings

Checks if encoding settings can be modified at the current time. This is useful to verify the encoder is in a state where configuration changes are allowed (typically before the filter graph starts running).

**Returns**: `true` if settings can be modified, `false` otherwise

**Example Usage**:
```csharp
if (flacEncoder.canModifySettings())
{
    // Safe to modify encoder settings
    flacEncoder.setEncodingLevel(5);
}
```

### Encoding Configuration Methods

#### setEncodingLevel

Sets the FLAC encoding level, which controls the compression quality and encoding speed trade-off.

**Parameters**:
- `inLevel` - Encoding level (0-8):
  - 0 = Fastest encoding, lowest compression
  - 5 = Balanced (recommended for most uses)
  - 8 = Slowest encoding, highest compression

**Returns**: `true` if successful, `false` otherwise

**Recommended Values**:
- **Fast archival**: Level 3-5
- **High-quality archival**: Level 6-8
- **Real-time encoding**: Level 0-2

#### setLPCOrder

Sets the Linear Predictive Coding (LPC) order, which affects compression efficiency and encoding speed.

**Parameters**:
- `inLPCOrder` - LPC order value (typically 0-32)
  - 0 = No LPC (fastest)
  - 12 = Default for most audio
  - 32 = Maximum compression (slowest)

**Returns**: `true` if successful, `false` otherwise

**Note**: Higher LPC orders provide better compression but significantly increase encoding time.

#### setBlockSize

Sets the audio block size for encoding. The block size affects both compression efficiency and latency.

**Parameters**:
- `inBlockSize` - Block size in samples
  - Common values: 192, 576, 1152, 2304, 4608
  - Default is typically 4096 for 44.1kHz audio

**Returns**: `true` if successful, `false` otherwise

**Recommendations**:
- **Low latency**: 192-1152 samples
- **Standard archival**: 4096 samples
- **Maximum compression**: 4608 samples

#### useMidSideCoding

Enables or disables mid-side stereo coding for 2-channel audio. Mid-side coding can improve compression for stereo audio where the left and right channels are highly correlated.

**Parameters**:
- `inUseMidSideCoding` - `true` to enable, `false` to disable

**Returns**: `true` if successful, `false` otherwise

**Note**: Only applicable for 2-channel (stereo) audio. Most music benefits from mid-side coding.

#### useAdaptiveMidSideCoding

Enables or disables adaptive mid-side stereo coding. This mode automatically decides whether to use mid-side coding on a per-block basis, providing better compression than fixed mid-side coding.

**Parameters**:
- `inUseAdaptiveMidSideCoding` - `true` to enable, `false` to disable

**Returns**: `true` if successful, `false` otherwise

**Note**:
- Only for 2-channel audio
- Overrides `useMidSideCoding` setting
- Generally provides better performance than fixed mid-side coding
- Recommended for most stereo encoding scenarios

#### useExhaustiveModelSearch

Enables or disables exhaustive model search for finding the best compression predictor.

**Parameters**:
- `inUseExhaustiveModelSearch` - `true` to enable, `false` to disable

**Returns**: `true` if successful, `false` otherwise

**Warning**: Exhaustive search significantly slows down encoding (often 2-4x slower) but can provide marginally better compression (typically 1-3% file size reduction).

**Recommended**: Only enable for archival of critical audio where encoding time is not a concern.

#### setRicePartitionOrder

Sets the Rice partition order range for entropy coding. Rice coding is the final compression stage in FLAC.

**Parameters**:
- `inMin` - Minimum Rice partition order (typically 0-2)
- `inMax` - Maximum Rice partition order (typically 3-8)

**Returns**: `true` if successful, `false` otherwise

**Typical Values**:
- Fast encoding: min=0, max=3
- Standard encoding: min=0, max=6
- Maximum compression: min=0, max=8

### Status Query Methods

#### encoderLevel

Gets the current encoding level setting.

**Returns**: Current encoding level (0-8)

#### LPCOrder

Gets the current Linear Predictive Coding (LPC) order.

**Returns**: Current LPC order value

#### blockSize

Gets the current block size setting.

**Returns**: Current block size in samples

#### riceMin

Gets the minimum Rice partition order.

**Returns**: Minimum Rice partition order

#### riceMax

Gets the maximum Rice partition order.

**Returns**: Maximum Rice partition order

#### isUsingMidSideCoding

Checks if fixed mid-side stereo coding is enabled.

**Returns**: `true` if mid-side coding is enabled, `false` otherwise

#### isUsingAdaptiveMidSideCoding

Checks if adaptive mid-side stereo coding is enabled.

**Returns**: `true` if adaptive mid-side coding is enabled, `false` otherwise

#### isUsingExhaustiveModel

Checks if exhaustive model search is enabled.

**Returns**: `true` if exhaustive model search is enabled, `false` otherwise

## Usage Examples

### C# Example - High Quality Archival

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class FLACArchivalEncoder
{
    public void ConfigureHighQualityArchival(IBaseFilter audioEncoder)
    {
        // Query the FLAC encoder interface
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null)
        {
            Console.WriteLine("Error: Filter does not support IFLACEncodeSettings");
            return;
        }

        // Check if we can modify settings
        if (!flacEncoder.canModifySettings())
        {
            Console.WriteLine("Warning: Cannot modify encoder settings at this time");
            return;
        }

        // High quality archival settings
        flacEncoder.setEncodingLevel(8);              // Maximum compression
        flacEncoder.setLPCOrder(12);                  // Good LPC order for music
        flacEncoder.setBlockSize(4096);               // Standard block size for 44.1kHz
        flacEncoder.useAdaptiveMidSideCoding(true);   // Adaptive mid-side for stereo
        flacEncoder.useExhaustiveModelSearch(true);   // Best possible compression
        flacEncoder.setRicePartitionOrder(0, 8);      // Maximum Rice partition range

        Console.WriteLine("FLAC encoder configured for high-quality archival:");
        Console.WriteLine($"  Encoding Level: {flacEncoder.encoderLevel()}");
        Console.WriteLine($"  LPC Order: {flacEncoder.LPCOrder()}");
        Console.WriteLine($"  Block Size: {flacEncoder.blockSize()}");
        Console.WriteLine($"  Adaptive Mid-Side: {flacEncoder.isUsingAdaptiveMidSideCoding()}");
        Console.WriteLine($"  Exhaustive Search: {flacEncoder.isUsingExhaustiveModel()}");
        Console.WriteLine($"  Rice Partition: {flacEncoder.riceMin()}-{flacEncoder.riceMax()}");
    }
}
```

### C# Example - Fast Real-Time Encoding

```csharp
public class FLACRealTimeEncoder
{
    public void ConfigureFastEncoding(IBaseFilter audioEncoder)
    {
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null || !flacEncoder.canModifySettings())
            return;

        // Fast encoding settings for real-time use
        flacEncoder.setEncodingLevel(2);              // Fast encoding
        flacEncoder.setLPCOrder(8);                   // Lower LPC for speed
        flacEncoder.setBlockSize(1152);               // Smaller blocks for lower latency
        flacEncoder.useAdaptiveMidSideCoding(true);   // Still good compression
        flacEncoder.useExhaustiveModelSearch(false);  // Disable for speed
        flacEncoder.setRicePartitionOrder(0, 4);      // Reduced Rice partition range

        Console.WriteLine("FLAC encoder configured for fast real-time encoding");
        Console.WriteLine($"  Encoding Level: {flacEncoder.encoderLevel()}");
        Console.WriteLine($"  LPC Order: {flacEncoder.LPCOrder()}");
        Console.WriteLine($"  Block Size: {flacEncoder.blockSize()} (lower latency)");
    }
}
```

### C# Example - Balanced Music Encoding

```csharp
public class FLACMusicEncoder
{
    public void ConfigureBalancedMusic(IBaseFilter audioEncoder)
    {
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null || !flacEncoder.canModifySettings())
            return;

        // Balanced settings for music encoding (good compression, reasonable speed)
        flacEncoder.setEncodingLevel(5);              // Balanced compression
        flacEncoder.setLPCOrder(12);                  // Standard LPC for music
        flacEncoder.setBlockSize(4096);               // Optimal for 44.1kHz
        flacEncoder.useAdaptiveMidSideCoding(true);   // Adaptive mid-side
        flacEncoder.useExhaustiveModelSearch(false);  // Not needed for music
        flacEncoder.setRicePartitionOrder(0, 6);      // Good Rice partition range

        Console.WriteLine("FLAC encoder configured for balanced music encoding");
    }
}
```

### C++ Example - High Quality Archival

```cpp
#include <dshow.h>
#include <iostream>
#include "IFLACEncodeSettings.h"

void ConfigureHighQualityFLAC(IBaseFilter* pAudioEncoder)
{
    IFLACEncodeSettings* pFLACEncoder = NULL;
    HRESULT hr = S_OK;

    // Query the FLAC encoder interface
    hr = pAudioEncoder->QueryInterface(IID_IFLACEncodeSettings,
                                       (void**)&pFLACEncoder);
    if (FAILED(hr) || !pFLACEncoder)
    {
        std::cout << "Error: Filter does not support IFLACEncodeSettings" << std::endl;
        return;
    }

    // Check if we can modify settings
    if (!pFLACEncoder->canModifySettings())
    {
        std::cout << "Warning: Cannot modify encoder settings" << std::endl;
        pFLACEncoder->Release();
        return;
    }

    // Configure high quality archival settings
    pFLACEncoder->setEncodingLevel(8);              // Maximum compression
    pFLACEncoder->setLPCOrder(12);                  // Good LPC order
    pFLACEncoder->setBlockSize(4096);               // Standard block size
    pFLACEncoder->useAdaptiveMidSideCoding(TRUE);   // Adaptive mid-side
    pFLACEncoder->useExhaustiveModelSearch(TRUE);   // Best compression
    pFLACEncoder->setRicePartitionOrder(0, 8);      // Maximum range

    // Display configuration
    std::cout << "FLAC encoder configured for high-quality archival:" << std::endl;
    std::cout << "  Encoding Level: " << pFLACEncoder->encoderLevel() << std::endl;
    std::cout << "  LPC Order: " << pFLACEncoder->LPCOrder() << std::endl;
    std::cout << "  Block Size: " << pFLACEncoder->blockSize() << std::endl;
    std::cout << "  Adaptive Mid-Side: "
              << (pFLACEncoder->isUsingAdaptiveMidSideCoding() ? "Yes" : "No") << std::endl;
    std::cout << "  Exhaustive Search: "
              << (pFLACEncoder->isUsingExhaustiveModel() ? "Yes" : "No") << std::endl;

    pFLACEncoder->Release();
}
```

### C++ Example - Fast Real-Time Encoding

```cpp
void ConfigureFastFLAC(IBaseFilter* pAudioEncoder)
{
    IFLACEncodeSettings* pFLACEncoder = NULL;
    HRESULT hr = pAudioEncoder->QueryInterface(IID_IFLACEncodeSettings,
                                               (void**)&pFLACEncoder);
    if (SUCCEEDED(hr) && pFLACEncoder)
    {
        if (pFLACEncoder->canModifySettings())
        {
            // Fast encoding configuration
            pFLACEncoder->setEncodingLevel(2);              // Fast
            pFLACEncoder->setLPCOrder(8);                   // Lower LPC
            pFLACEncoder->setBlockSize(1152);               // Smaller blocks
            pFLACEncoder->useAdaptiveMidSideCoding(TRUE);   // Still good
            pFLACEncoder->useExhaustiveModelSearch(FALSE);  // Disabled for speed
            pFLACEncoder->setRicePartitionOrder(0, 4);      // Reduced range

            std::cout << "FLAC encoder configured for fast real-time encoding" << std::endl;
        }
        pFLACEncoder->Release();
    }
}
```

### Delphi Example - High Quality Archival

```delphi
uses
  DirectShow9, ActiveX;

procedure ConfigureHighQualityFLAC(AudioEncoder: IBaseFilter);
var
  FLACEncoder: IFLACEncodeSettings;
  hr: HRESULT;
begin
  // Query the FLAC encoder interface
  hr := AudioEncoder.QueryInterface(IID_IFLACEncodeSettings, FLACEncoder);
  if Failed(hr) or (FLACEncoder = nil) then
  begin
    WriteLn('Error: Filter does not support IFLACEncodeSettings');
    Exit;
  end;

  try
    // Check if we can modify settings
    if not FLACEncoder.canModifySettings then
    begin
      WriteLn('Warning: Cannot modify encoder settings');
      Exit;
    end;

    // Configure high quality archival settings
    FLACEncoder.setEncodingLevel(8);              // Maximum compression
    FLACEncoder.setLPCOrder(12);                  // Good LPC order
    FLACEncoder.setBlockSize(4096);               // Standard block size
    FLACEncoder.useAdaptiveMidSideCoding(True);   // Adaptive mid-side
    FLACEncoder.useExhaustiveModelSearch(True);   // Best compression
    FLACEncoder.setRicePartitionOrder(0, 8);      // Maximum range

    // Display configuration
    WriteLn('FLAC encoder configured for high-quality archival:');
    WriteLn('  Encoding Level: ', FLACEncoder.encoderLevel);
    WriteLn('  LPC Order: ', FLACEncoder.LPCOrder);
    WriteLn('  Block Size: ', FLACEncoder.blockSize);
    WriteLn('  Adaptive Mid-Side: ', FLACEncoder.isUsingAdaptiveMidSideCoding);
    WriteLn('  Exhaustive Search: ', FLACEncoder.isUsingExhaustiveModel);

  finally
    FLACEncoder := nil;
  end;
end;
```

### Delphi Example - Balanced Music Encoding

```delphi
procedure ConfigureBalancedMusicFLAC(AudioEncoder: IBaseFilter);
var
  FLACEncoder: IFLACEncodeSettings;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IFLACEncodeSettings, FLACEncoder)) then
  begin
    try
      if FLACEncoder.canModifySettings then
      begin
        // Balanced settings for music
        FLACEncoder.setEncodingLevel(5);              // Balanced
        FLACEncoder.setLPCOrder(12);                  // Standard for music
        FLACEncoder.setBlockSize(4096);               // Optimal
        FLACEncoder.useAdaptiveMidSideCoding(True);   // Adaptive
        FLACEncoder.useExhaustiveModelSearch(False);  // Not needed
        FLACEncoder.setRicePartitionOrder(0, 6);      // Good range

        WriteLn('FLAC encoder configured for balanced music encoding');
      end;
    finally
      FLACEncoder := nil;
    end;
  end;
end;
```

## Best Practices

### Encoding Level Selection

**Level 0-2**: Fast encoding, suitable for real-time applications
- Use when encoding speed is critical
- Typical compression: 50-55% of original size

**Level 3-5**: Balanced encoding (recommended for most uses)
- Good balance between speed and compression
- Typical compression: 45-50% of original size
- **Level 5 is recommended** for general-purpose archival

**Level 6-8**: Maximum compression, slower encoding
- Use for long-term archival where storage space is critical
- Typical compression: 40-45% of original size
- Encoding can be 2-5x slower than level 5

### Mid-Side Stereo Coding

- **Always enable** `useAdaptiveMidSideCoding` for stereo audio
- Adaptive mode automatically determines when mid-side coding helps
- Provides better compression than fixed mid-side mode
- No significant performance penalty

### LPC Order Recommendations

**Music and General Audio**:
- Use LPC order 12 for most music encoding
- Higher orders (16-32) provide minimal benefit for music
- Lower orders (8) are suitable for speech

**Classical and High-Dynamic Range**:
- Consider LPC order 16-32 for orchestral recordings
- Provides better prediction for complex harmonic content

### Block Size Selection

**Sample Rate Considerations**:
- 44.1kHz: 4096 samples (default, ~93ms)
- 48kHz: 4608 samples (~96ms)
- 96kHz: 4608-8192 samples

**Latency Requirements**:
- Real-time: 192-1152 samples
- Standard archival: 4096 samples
- Maximum compression: 4608 samples

### Exhaustive Model Search

**When to Enable**:
- Critical archival projects where every byte counts
- Unlimited encoding time available
- File size reduction is paramount

**When to Disable** (recommended for most users):
- Real-time or near-real-time encoding
- Large batch encoding projects
- Compression improvement is typically <3%
- Encoding time increases 2-4x

### Rice Partition Order

**Fast Encoding**: `setRicePartitionOrder(0, 3)`
**Standard Encoding**: `setRicePartitionOrder(0, 6)` (recommended)
**Maximum Compression**: `setRicePartitionOrder(0, 8)`

## Troubleshooting

### Settings Cannot Be Modified

**Symptom**: `canModifySettings()` returns `false`

**Causes**:
1. Filter graph is already running
2. Encoder is actively processing audio
3. Filter is in an incorrect state

**Solutions**:
- Stop the filter graph before modifying settings
- Configure encoder before connecting filter pins
- Query settings before starting playback/capture

### Poor Compression Ratio

**Symptom**: FLAC files are larger than expected

**Possible Causes**:
1. Low encoding level (0-2)
2. Source audio is already compressed (MP3, AAC)
3. Source audio has high noise floor
4. Inappropriate block size for sample rate

**Solutions**:
- Increase encoding level to 5-8
- **Never re-encode already compressed audio** - FLAC cannot improve quality
- Use noise reduction on source audio before encoding
- Adjust block size to match sample rate (see recommendations above)

### Encoding Too Slow

**Symptom**: Real-time encoding cannot keep up with audio stream

**Solutions**:
1. Reduce encoding level to 0-3
2. Disable exhaustive model search
3. Reduce LPC order to 8
4. Reduce Rice partition max to 4
5. Use smaller block sizes (1152 or less)

### Audio Pops or Clicks in Encoded Output

**Symptom**: Audible artifacts in encoded FLAC files

**Possible Causes**:
1. Encoder cannot process fast enough (buffer underruns)
2. Incompatible block size with sample rate
3. Hardware performance issues

**Solutions**:
- Reduce encoding complexity (lower level, disable exhaustive search)
- Use standard block sizes for the sample rate
- Increase DirectShow buffer sizes
- Reduce system load during encoding

### Stereo Encoding Issues

**Symptom**: Stereo audio sounds incorrect or mono

**Check**:
- Verify input is actually stereo (2 channels)
- Mid-side coding only works with stereo input
- Check if adaptive mid-side is enabled for best results
- Verify filter graph audio format (use GraphEdit to inspect)

## Technical Notes

### FLAC Encoding Process

FLAC encoding involves several stages:
1. **Blocking**: Audio divided into blocks
2. **Prediction**: LPC analysis predicts sample values
3. **Mid-Side Coding**: Optional stereo decorrelation (for 2-channel audio)
4. **Residual Encoding**: Rice coding compresses prediction errors
5. **Frame Assembly**: Blocks assembled into FLAC frames

### Performance Characteristics

**CPU Usage by Setting**:
- Encoding Level: ~10% increase per level
- LPC Order: ~5% increase per 4 orders
- Exhaustive Search: 200-400% increase
- Mid-Side Coding: ~2-5% increase

**Memory Requirements**:
- Minimal: ~512KB working memory
- Larger blocks require more memory
- No significant dependency on audio duration

### Compatibility

FLAC files encoded with any settings combination are compatible with all FLAC decoders. Higher compression settings only affect encoding time and file size, not decoder compatibility or playback quality.

---
## See Also
- [LAME MP3 Encoder Interface](lame.md)
- [Audio Codecs Reference](../codecs-reference.md)
- [Encoding Filters Pack Overview](../index.md)