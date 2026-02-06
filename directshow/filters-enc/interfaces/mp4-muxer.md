---
title: MP4 Muxer DirectShow Filter Interface Reference
description: MP4 muxer DirectShow interfaces with threading configuration, timing correction, and live streaming options for MP4 container output.
---

# MP4 Muxer Interface Reference

## Overview

The MP4 muxer DirectShow filters provide interfaces for configuring MP4 (MPEG-4 Part 14) container output. These interfaces allow developers to control threading behavior, timing correction, and special handling for live streaming scenarios.

Two muxer interfaces are available:
- **IMP4MuxerConfig**: Basic MP4 muxer configuration for threading and timing
- **IMP4V10MuxerConfig**: Advanced configuration for version 10 muxer with timing flags and live streaming control

## IMP4MuxerConfig Interface

### Overview

The **IMP4MuxerConfig** interface provides basic configuration for MP4 multiplexing, controlling single-threaded operation and timing correction behavior.

**Interface GUID**: `{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}`

**Inherits From**: `IUnknown`

### Interface Definitions

#### C# Definition

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// MP4 muxer configuration interface.
    /// Controls threading and timing behavior for MP4 container creation.
    /// </summary>
    [ComImport]
    [Guid("99DC9BE5-0AFA-45d4-8370-AB021FB07CF4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMP4MuxerConfig
    {
        /// <summary>
        /// Gets the single-thread processing state.
        /// </summary>
        /// <param name="pValue">Receives true if single-threaded mode is enabled, false otherwise</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int get_SingleThread([Out] [MarshalAs(UnmanagedType.Bool)] out bool pValue);

        /// <summary>
        /// Enables or disables single-threaded processing.
        /// When enabled, all muxer operations run on a single thread for deterministic behavior.
        /// </summary>
        /// <param name="value">True to enable single-threaded mode, false for multi-threaded</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int put_SingleThread([In] [MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Gets the timing correction state.
        /// </summary>
        /// <param name="pValue">Receives true if timing correction is enabled, false otherwise</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int get_CorrectTiming([Out] [MarshalAs(UnmanagedType.Bool)] out bool pValue);

        /// <summary>
        /// Enables or disables timing correction.
        /// When enabled, the muxer adjusts timestamps to correct timing drift and inconsistencies.
        /// </summary>
        /// <param name="value">True to enable timing correction, false to disable</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int put_CorrectTiming([In] [MarshalAs(UnmanagedType.Bool)] bool value);
    }
}
```

#### C++ Definition

```cpp
#include <unknwn.h>

// {99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}
DEFINE_GUID(IID_IMP4MuxerConfig,
    0x99dc9be5, 0x0afa, 0x45d4, 0x83, 0x70, 0xab, 0x02, 0x1f, 0xb0, 0x7c, 0xf4);

/// <summary>
/// MP4 muxer configuration interface.
/// Controls threading and timing behavior.
/// </summary>
DECLARE_INTERFACE_(IMP4MuxerConfig, IUnknown)
{
    /// <summary>
    /// Gets the single-thread processing state.
    /// </summary>
    /// <param name="pValue">Pointer to receive single-thread enabled state</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(get_SingleThread)(THIS_
        BOOL* pValue
        ) PURE;

    /// <summary>
    /// Enables or disables single-threaded processing.
    /// </summary>
    /// <param name="value">TRUE to enable single-threaded mode, FALSE for multi-threaded</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(put_SingleThread)(THIS_
        BOOL value
        ) PURE;

    /// <summary>
    /// Gets the timing correction state.
    /// </summary>
    /// <param name="pValue">Pointer to receive timing correction enabled state</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(get_CorrectTiming)(THIS_
        BOOL* pValue
        ) PURE;

    /// <summary>
    /// Enables or disables timing correction.
    /// </summary>
    /// <param name="value">TRUE to enable timing correction, FALSE to disable</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(put_CorrectTiming)(THIS_
        BOOL value
        ) PURE;
};
```

#### Delphi Definition

```delphi
uses
  ActiveX, ComObj;

const
  IID_IMP4MuxerConfig: TGUID = '{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}';

type
  /// <summary>
  /// MP4 muxer configuration interface.
  /// </summary>
  IMP4MuxerConfig = interface(IUnknown)
    ['{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}']

    /// <summary>
    /// Gets the single-thread processing state.
    /// </summary>
    function get_SingleThread(out pValue: BOOL): HRESULT; stdcall;

    /// <summary>
    /// Enables or disables single-threaded processing.
    /// </summary>
    function put_SingleThread(value: BOOL): HRESULT; stdcall;

    /// <summary>
    /// Gets the timing correction state.
    /// </summary>
    function get_CorrectTiming(out pValue: BOOL): HRESULT; stdcall;

    /// <summary>
    /// Enables or disables timing correction.
    /// </summary>
    function put_CorrectTiming(value: BOOL): HRESULT; stdcall;
  end;
```

### Method Reference

#### get_SingleThread / put_SingleThread

Controls whether the muxer processes data using a single thread or multiple threads.

**Single-Threaded Mode (enabled)**:
- All muxing operations run on one thread
- Deterministic, predictable behavior
- Easier debugging and troubleshooting
- Slightly lower performance on multi-core systems
- **Recommended for**: Scenarios requiring consistent, reproducible output

**Multi-Threaded Mode (disabled)**:
- Muxer can use multiple threads for processing
- Better performance on multi-core processors
- Non-deterministic operation order
- **Recommended for**: High-performance encoding with multiple streams

**Default**: Typically multi-threaded (false)

**Example**:
```csharp
// Enable single-threaded mode for consistent output
mp4Muxer.put_SingleThread(true);
```

#### get_CorrectTiming / put_CorrectTiming

Enables or disables automatic timestamp correction for audio and video streams.

**Timing Correction Enabled (true)**:
- Muxer automatically adjusts timestamps to correct drift
- Fixes timing inconsistencies from source filters
- Ensures proper A/V synchronization
- Adds small processing overhead
- **Recommended for**: Most scenarios, especially with live sources

**Timing Correction Disabled (false)**:
- Timestamps passed through without modification
- Assumes source filters provide accurate timestamps
- Slightly better performance
- **Use only when**: Source provides guaranteed accurate timestamps

**Default**: Typically enabled (true)

**Example**:
```csharp
// Enable timing correction for A/V sync
mp4Muxer.put_CorrectTiming(true);
```

---
## IMP4V10MuxerConfig Interface
### Overview
The **IMP4V10MuxerConfig** interface provides advanced configuration for the version 10 MP4 muxer, including timing override flags and live streaming control.
**Interface GUID**: `{9E26CE8B-6708-4535-AAA4-23F9A97C7937}`
**Inherits From**: `IUnknown`
### MP4V10Flags Enumeration
```csharp
/// <summary>
/// MP4 v10 muxer configuration flags.
/// </summary>
[Flags]
public enum MP4V10Flags
{
    /// <summary>
    /// No special flags.
    /// </summary>
    None = 0,
    /// <summary>
    /// Time override mode - allows manual timestamp control.
    /// </summary>
    TimeOverride = 0x00000001,
    /// <summary>
    /// Time adjust mode - enables automatic timestamp adjustment.
    /// </summary>
    TimeAdjust = 0x00000002
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
    /// MP4 v10 muxer flags.
    /// </summary>
    [Flags]
    public enum MP4V10Flags
    {
        /// <summary>
        /// Default - no special flags.
        /// </summary>
        None = 0,
        /// <summary>
        /// Time override - allows manual timestamp control.
        /// </summary>
        TimeOverride = 0x00000001,
        /// <summary>
        /// Time adjust - enables automatic timestamp adjustment.
        /// </summary>
        TimeAdjust = 0x00000002
    }
    /// <summary>
    /// MP4 version 10 muxer configuration interface.
    /// Provides advanced timing control and live streaming options.
    /// </summary>
    [ComImport]
    [Guid("9E26CE8B-6708-4535-AAA4-23F9A97C7937")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMP4V10MuxerConfig
    {
        /// <summary>
        /// Sets the muxer configuration flags.
        /// </summary>
        /// <param name="value">Combination of MP4V10Flags values</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetFlags([In] uint value);
        /// <summary>
        /// Gets the current muxer configuration flags.
        /// </summary>
        /// <param name="pValue">Receives the current flags</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int GetFlags([Out] out uint pValue);
        /// <summary>
        /// Disables live streaming optimizations.
        /// When disabled, muxer uses standard file-based output mode.
        /// </summary>
        /// <param name="liveDisabled">True to disable live mode, false to enable</param>
        /// <returns>HRESULT (0 for success)</returns>
        [PreserveSig]
        int SetLiveDisabled([MarshalAs(UnmanagedType.Bool)] bool liveDisabled);
    }
}
```
#### C++ Definition
```cpp
#include <unknwn.h>
// {9E26CE8B-6708-4535-AAA4-23F9A97C7937}
DEFINE_GUID(IID_IMP4V10MuxerConfig,
    0x9e26ce8b, 0x6708, 0x4535, 0xaa, 0xa4, 0x23, 0xf9, 0xa9, 0x7c, 0x79, 0x37);
/// <summary>
/// MP4 v10 muxer flags.
/// </summary>
enum MP4V10Flags
{
    MP4V10_NONE = 0,
    MP4V10_TIME_OVERRIDE = 0x00000001,
    MP4V10_TIME_ADJUST = 0x00000002
};
/// <summary>
/// MP4 version 10 muxer configuration interface.
/// Provides advanced timing control and live streaming options.
/// </summary>
DECLARE_INTERFACE_(IMP4V10MuxerConfig, IUnknown)
{
    /// <summary>
    /// Sets the muxer configuration flags.
    /// </summary>
    /// <param name="value">Combination of MP4V10Flags values</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(SetFlags)(THIS_
        unsigned long value
        ) PURE;
    /// <summary>
    /// Gets the current muxer configuration flags.
    /// </summary>
    /// <param name="pValue">Pointer to receive current flags</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(GetFlags)(THIS_
        unsigned long* pValue
        ) PURE;
    /// <summary>
    /// Disables live streaming optimizations.
    /// </summary>
    /// <param name="liveDisabled">TRUE to disable live mode, FALSE to enable</param>
    /// <returns>S_OK for success</returns>
    STDMETHOD(SetLiveDisabled)(THIS_
        BOOL liveDisabled
        ) PURE;
};
```
#### Delphi Definition
```delphi
uses
  ActiveX, ComObj;
const
  IID_IMP4V10MuxerConfig: TGUID = '{9E26CE8B-6708-4535-AAA4-23F9A97C7937}';
  // MP4V10Flags constants
  MP4V10_NONE = 0;
  MP4V10_TIME_OVERRIDE = $00000001;
  MP4V10_TIME_ADJUST = $00000002;
type
  /// <summary>
  /// MP4 version 10 muxer configuration interface.
  /// </summary>
  IMP4V10MuxerConfig = interface(IUnknown)
    ['{9E26CE8B-6708-4535-AAA4-23F9A97C7937}']
    /// <summary>
    /// Sets the muxer configuration flags.
    /// </summary>
    function SetFlags(value: Cardinal): HRESULT; stdcall;
    /// <summary>
    /// Gets the current muxer configuration flags.
    /// </summary>
    function GetFlags(out pValue: Cardinal): HRESULT; stdcall;
    /// <summary>
    /// Disables live streaming optimizations.
    /// </summary>
    function SetLiveDisabled(liveDisabled: BOOL): HRESULT; stdcall;
  end;
```
### Method Reference
#### SetFlags / GetFlags
Sets or retrieves the muxer configuration flags that control timing behavior.
**MP4V10Flags Values**:
**None (0)**:
- Standard operation
- Default timestamp handling
- No special timing modifications
**TimeOverride (0x00000001)**:
- Enables manual timestamp override
- Allows application to control timestamps directly
- Disables automatic timestamp generation
- **Use when**: Application needs full control over timing
**TimeAdjust (0x00000002)**:
- Enables automatic timestamp adjustment
- Muxer corrects timing drift and irregularities
- Similar to IMP4MuxerConfig::CorrectTiming
- **Use for**: Sources with inconsistent timestamps
**Combining Flags**:
```csharp
// Enable both time override and adjust
uint flags = (uint)(MP4V10Flags.TimeOverride | MP4V10Flags.TimeAdjust);
mp4V10Muxer.SetFlags(flags);
```
#### SetLiveDisabled
Controls whether the muxer operates in live streaming mode or file-based mode.
**Live Mode Enabled** (liveDisabled = false):
- Optimized for live/real-time streaming
- Minimal buffering
- Lower latency
- Progressive MP4 output (can be played while being written)
- **Use for**: Live streaming to file, network streaming output
**Live Mode Disabled** (liveDisabled = true):
- Standard file-based muxing
- Can perform multi-pass optimization
- Complete MP4 structure written at end
- May require seeking in output file
- **Use for**: File-based encoding, post-processing scenarios
**Example**:
```csharp
// Enable file-based mode (disable live optimizations)
mp4V10Muxer.SetLiveDisabled(true);
```
## Usage Examples
### C# Example - Standard MP4 File Creation
```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;
public class MP4MuxerStandardConfig
{
    public void ConfigureStandardMP4(IBaseFilter mp4Muxer)
    {
        // Query the standard MP4 muxer interface
        var muxerConfig = mp4Muxer as IMP4MuxerConfig;
        if (muxerConfig == null)
        {
            Console.WriteLine("Error: Filter does not support IMP4MuxerConfig");
            return;
        }
        // Configure for standard file-based encoding
        muxerConfig.put_SingleThread(false);     // Multi-threaded for performance
        muxerConfig.put_CorrectTiming(true);     // Enable timing correction
        Console.WriteLine("MP4 muxer configured for standard file creation");
        // Verify configuration
        muxerConfig.get_SingleThread(out bool singleThread);
        muxerConfig.get_CorrectTiming(out bool correctTiming);
        Console.WriteLine($"  Single-threaded: {singleThread}");
        Console.WriteLine($"  Timing correction: {correctTiming}");
    }
}
```
### C# Example - Deterministic Output
```csharp
public class MP4MuxerDeterministicConfig
{
    public void ConfigureDeterministicMP4(IBaseFilter mp4Muxer)
    {
        var muxerConfig = mp4Muxer as IMP4MuxerConfig;
        if (muxerConfig == null)
            return;
        // Configure for deterministic, reproducible output
        muxerConfig.put_SingleThread(true);      // Single-threaded for consistency
        muxerConfig.put_CorrectTiming(true);     // Enable timing correction
        Console.WriteLine("MP4 muxer configured for deterministic output");
        Console.WriteLine("  Suitable for regression testing and validation");
    }
}
```
### C# Example - Live Streaming to File (MP4 V10)
```csharp
public class MP4V10LiveStreamingConfig
{
    public void ConfigureLiveStreaming(IBaseFilter mp4V10Muxer)
    {
        // Query the MP4 v10 muxer interface
        var muxerV10Config = mp4V10Muxer as IMP4V10MuxerConfig;
        if (muxerV10Config == null)
        {
            Console.WriteLine("Error: Filter does not support IMP4V10MuxerConfig");
            return;
        }
        // Configure for live streaming to file
        muxerV10Config.SetLiveDisabled(false);   // Enable live mode
        // Enable timing adjustment for live sources
        uint flags = (uint)MP4V10Flags.TimeAdjust;
        muxerV10Config.SetFlags(flags);
        Console.WriteLine("MP4 v10 muxer configured for live streaming");
        // Verify configuration
        muxerV10Config.GetFlags(out uint currentFlags);
        Console.WriteLine($"  Flags: 0x{currentFlags:X8}");
        Console.WriteLine($"  Time Adjust: {((currentFlags & (uint)MP4V10Flags.TimeAdjust) != 0)}");
    }
}
```
### C# Example - Manual Timestamp Control (MP4 V10)
```csharp
public class MP4V10ManualTimestampConfig
{
    public void ConfigureManualTimestamps(IBaseFilter mp4V10Muxer)
    {
        var muxerV10Config = mp4V10Muxer as IMP4V10MuxerConfig;
        if (muxerV10Config == null)
            return;
        // Configure for manual timestamp control
        muxerV10Config.SetLiveDisabled(true);    // Disable live mode
        // Enable time override for manual control
        uint flags = (uint)MP4V10Flags.TimeOverride;
        muxerV10Config.SetFlags(flags);
        Console.WriteLine("MP4 v10 muxer configured for manual timestamp control");
        Console.WriteLine("  Application must provide accurate timestamps");
    }
}
```
### C++ Example - Standard Configuration
```cpp
#include <dshow.h>
#include <iostream>
#include "IMP4MuxerConfig.h"
void ConfigureMP4Muxer(IBaseFilter* pMp4Muxer)
{
    IMP4MuxerConfig* pMuxerConfig = NULL;
    HRESULT hr = S_OK;
    // Query the MP4 muxer interface
    hr = pMp4Muxer->QueryInterface(IID_IMP4MuxerConfig,
                                   (void**)&pMuxerConfig);
    if (FAILED(hr) || !pMuxerConfig)
    {
        std::cout << "Error: Filter does not support IMP4MuxerConfig" << std::endl;
        return;
    }
    // Configure muxer
    pMuxerConfig->put_SingleThread(FALSE);     // Multi-threaded
    pMuxerConfig->put_CorrectTiming(TRUE);     // Enable timing correction
    // Verify configuration
    BOOL singleThread, correctTiming;
    pMuxerConfig->get_SingleThread(&singleThread);
    pMuxerConfig->get_CorrectTiming(&correctTiming);
    std::cout << "MP4 muxer configured:" << std::endl;
    std::cout << "  Single-threaded: " << (singleThread ? "Yes" : "No") << std::endl;
    std::cout << "  Timing correction: " << (correctTiming ? "Yes" : "No") << std::endl;
    pMuxerConfig->Release();
}
```
### C++ Example - Live Streaming (MP4 V10)
```cpp
#include "IMP4V10MuxerConfig.h"
void ConfigureMP4V10LiveStreaming(IBaseFilter* pMp4V10Muxer)
{
    IMP4V10MuxerConfig* pMuxerV10Config = NULL;
    HRESULT hr = pMp4V10Muxer->QueryInterface(IID_IMP4V10MuxerConfig,
                                               (void**)&pMuxerV10Config);
    if (SUCCEEDED(hr) && pMuxerV10Config)
    {
        // Configure for live streaming
        pMuxerV10Config->SetLiveDisabled(FALSE);     // Enable live mode
        // Enable timing adjustment
        unsigned long flags = MP4V10_TIME_ADJUST;
        pMuxerV10Config->SetFlags(flags);
        std::cout << "MP4 v10 muxer configured for live streaming" << std::endl;
        pMuxerV10Config->Release();
    }
}
```
### Delphi Example - Standard Configuration
```delphi
uses
  DirectShow9, ActiveX;
procedure ConfigureMP4Muxer(Mp4Muxer: IBaseFilter);
var
  MuxerConfig: IMP4MuxerConfig;
  SingleThread, CorrectTiming: BOOL;
  hr: HRESULT;
begin
  // Query the MP4 muxer interface
  hr := Mp4Muxer.QueryInterface(IID_IMP4MuxerConfig, MuxerConfig);
  if Failed(hr) or (MuxerConfig = nil) then
  begin
    WriteLn('Error: Filter does not support IMP4MuxerConfig');
    Exit;
  end;
  try
    // Configure muxer
    MuxerConfig.put_SingleThread(False);     // Multi-threaded
    MuxerConfig.put_CorrectTiming(True);     // Enable timing correction
    // Verify configuration
    MuxerConfig.get_SingleThread(SingleThread);
    MuxerConfig.get_CorrectTiming(CorrectTiming);
    WriteLn('MP4 muxer configured:');
    WriteLn('  Single-threaded: ', SingleThread);
    WriteLn('  Timing correction: ', CorrectTiming);
  finally
    MuxerConfig := nil;
  end;
end;
```
### Delphi Example - Live Streaming (MP4 V10)
```delphi
procedure ConfigureMP4V10LiveStreaming(Mp4V10Muxer: IBaseFilter);
var
  MuxerV10Config: IMP4V10MuxerConfig;
  Flags: Cardinal;
begin
  if Succeeded(Mp4V10Muxer.QueryInterface(IID_IMP4V10MuxerConfig, MuxerV10Config)) then
  begin
    try
      // Configure for live streaming
      MuxerV10Config.SetLiveDisabled(False);     // Enable live mode
      // Enable timing adjustment
      Flags := MP4V10_TIME_ADJUST;
      MuxerV10Config.SetFlags(Flags);
      WriteLn('MP4 v10 muxer configured for live streaming');
    finally
      MuxerV10Config := nil;
    end;
  end;
end;
```
## Best Practices
### When to Use IMP4MuxerConfig
**Use IMP4MuxerConfig when**:
- You need basic muxer configuration
- Working with standard MP4 output
- Simple timing correction is sufficient
- Don't need advanced live streaming features
**Typical Configuration**:
```csharp
mp4Muxer.put_SingleThread(false);    // Multi-threaded for performance
mp4Muxer.put_CorrectTiming(true);    // Enable timing correction
```
### When to Use IMP4V10MuxerConfig
**Use IMP4V10MuxerConfig when**:
- Need advanced timing control
- Working with live streaming scenarios
- Require manual timestamp override
- Need progressive MP4 output
**Live Streaming Configuration**:
```csharp
mp4V10Muxer.SetLiveDisabled(false);               // Enable live mode
mp4V10Muxer.SetFlags((uint)MP4V10Flags.TimeAdjust); // Auto timing adjustment
```
### Single-Threaded vs Multi-Threaded
**Use Single-Threaded Mode when**:
- Debugging muxer behavior
- Need deterministic, reproducible output
- Running automated tests
- Troubleshooting timing issues
**Use Multi-Threaded Mode when**:
- Performance is critical
- Encoding high-resolution video (1080p+)
- System has multiple CPU cores available
- Standard production encoding
### Timing Correction
**Always Enable Timing Correction when**:
- Working with live sources (cameras, capture devices)
- Sources may have timestamp inconsistencies
- Combining multiple streams (audio + video)
- Need reliable A/V synchronization
**Can Disable Timing Correction when**:
- Source provides guaranteed accurate timestamps
- File-based encoding with pre-validated timestamps
- Performance is absolutely critical
- Using manual timestamp control (TimeOverride flag)
### Live Streaming Optimization
**Enable Live Mode** (SetLiveDisabled = false) **when**:
- Encoding for real-time streaming
- Output needs to be playable while being written
- Creating progressive MP4 files
- Low latency is important
**Disable Live Mode** (SetLiveDisabled = true) **when**:
- Creating files for post-processing
- Need complete MP4 structure at end
- Can perform multi-pass optimization
- Output file will only be played after completion
## Troubleshooting
### Audio/Video Sync Issues
**Symptoms**: Audio and video drift out of sync over time
**Solutions**:
1. Enable timing correction: `put_CorrectTiming(true)`
2. For v10 muxer, use TimeAdjust flag: `SetFlags((uint)MP4V10Flags.TimeAdjust)`
3. Verify source filters provide accurate timestamps
4. Check that audio and video sample rates are correct
### File Cannot Be Played While Recording
**Symptom**: MP4 file only playable after encoding completes
**Cause**: Live mode is disabled
**Solution**:
- Use IMP4V10MuxerConfig interface
- Enable live mode: `SetLiveDisabled(false)`
- This creates progressive MP4 files playable during encoding
### Inconsistent File Output
**Symptoms**: Same input produces different output files
**Cause**: Multi-threaded operation with race conditions
**Solutions**:
1. Enable single-threaded mode: `put_SingleThread(true)`
2. Enable timing correction: `put_CorrectTiming(true)`
3. Use TimeAdjust flag for v10 muxer
### Performance Issues
**Symptoms**: Encoding slower than expected, high CPU usage
**Possible Causes**:
1. Single-threaded mode on multi-core system
2. Excessive timing correction overhead
**Solutions**:
- Disable single-threaded mode: `put_SingleThread(false)`
- If sources have accurate timestamps, can try disabling timing correction
- Ensure video encoder (not muxer) is the performance bottleneck
- Consider hardware encoding (NVENC, QuickSync)
### Corrupted MP4 Files
**Symptoms**: MP4 file won't play or has errors
**Possible Causes**:
1. Timing correction disabled with poor timestamps
2. Incorrect live mode setting for use case
3. Muxer stopped before proper finalization
**Solutions**:
- Enable timing correction for live sources
- Match live mode setting to use case (live vs file-based)
- Ensure proper filter graph shutdown and stream finalization
- Verify all streams end properly (send EC_COMPLETE event)
---

## See Also

- [H.264 Encoder Interface](h264.md)
- [AAC Encoder Interfaces](aac.md)
- [Muxers Reference](../muxers-reference.md)
- [Encoding Filters Pack Overview](../index.md)
