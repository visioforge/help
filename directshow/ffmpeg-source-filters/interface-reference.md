---
title: FFMPEG Source Filter - Interface Reference
description: IFFmpegSourceSettings interface with hardware acceleration, buffering modes, custom FFmpeg options, and callbacks for DirectShow.
---

# IFFmpegSourceSettings Interface Reference

## Overview

The `IFFmpegSourceSettings` interface provides advanced configuration options for the FFMPEG Source DirectShow filter. This interface enables developers to control hardware acceleration, buffering behavior, custom FFmpeg options, and various callbacks for media playback.

## Interface Definition

- **Interface Name**: `IFFmpegSourceSettings`
- **GUID**: `{1974D893-83E4-4F89-9908-795C524CC17E}`
- **Inherits From**: `IUnknown`

### Interface Definition Files

Complete interface definitions are available on GitHub:

- **C# (.NET)**: [IFFmpegSourceSettings.cs](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/dotnet/IFFmpegSourceSettings.cs)
- **C++ Header**: [IFFmpegSourceSettings.h](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/cpp/FFMPEG%20Source/IFFmpegSourceSettings.h)
- **Delphi**: [VCFiltersAPI.pas](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/delphi/VCFiltersAPI.pas) (search for `IFFMPEGSourceSettings`)

All interface definitions include:

- Complete method signatures with proper marshalling attributes
- Callback delegate definitions
- Enumeration types (buffering modes, media types)
- Usage documentation and examples

## Methods Reference

### Hardware Acceleration

#### GetHWAccelerationEnabled

Retrieves the current hardware acceleration state.

**Syntax (C++)**:

```cpp
BOOL GetHWAccelerationEnabled();
```

**Syntax (C#)**:

```csharp
[PreserveSig]
bool GetHWAccelerationEnabled();
```

**Returns**: `TRUE` if hardware acceleration is enabled, `FALSE` otherwise.

**Default**: `TRUE`

---
#### SetHWAccelerationEnabled
Enables or disables hardware video decoding acceleration.
**Syntax (C++)**:
```cpp
HRESULT SetHWAccelerationEnabled(BOOL enabled);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetHWAccelerationEnabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
```
**Parameters**:
- `enabled`: Set to `TRUE` to enable hardware acceleration, `FALSE` to disable.
**Returns**: `S_OK` (0) on success, error code otherwise.
**Usage Notes**:
- Must be called **before** connecting downstream video filters
- When enabled, the filter attempts to use hardware decoding (DXVA, NVDEC, QuickSync, etc.)
- Falls back to software decoding if hardware acceleration is unavailable
- Hardware acceleration significantly improves performance for H.264, H.265, VP9, and AV1 codecs
**Example (C++)**:
```cpp
IFFmpegSourceSettings* pSettings = nullptr;
pFilter->QueryInterface(IID_IFFmpegSourceSettings, (void**)&pSettings);
// Enable hardware acceleration
pSettings->SetHWAccelerationEnabled(TRUE);
pSettings->Release();
```
**Example (C#)**:
```csharp
var settings = filter as IFFmpegSourceSettings;
if (settings != null)
{
    // Enable hardware acceleration
    settings.SetHWAccelerationEnabled(true);
}
```
---

### Load Timeout Configuration

#### GetLoadTimeOut

Retrieves the current source loading timeout value.

**Syntax (C++)**:

```cpp
DWORD GetLoadTimeOut();
```

**Syntax (C#)**:

```csharp
[PreserveSig]
uint GetLoadTimeOut();
```

**Returns**: Timeout value in milliseconds.

**Default**: `15000` (15 seconds)

---
#### SetLoadTimeOut
Sets the timeout duration for source loading operations.
**Syntax (C++)**:
```cpp
HRESULT SetLoadTimeOut(DWORD milliseconds);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetLoadTimeOut(uint milliseconds);
```
**Parameters**:
- `milliseconds`: Timeout duration in milliseconds.
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Must be called **before** loading the source file/URL
- Particularly important for network streams that may have slow connection times
- Set higher values for slow network connections or large files
- Set lower values to fail fast on unreachable sources
**Example (C++)**:
```cpp
// Set 30-second timeout for network streams
pSettings->SetLoadTimeOut(30000);
// Load RTSP stream
IFileSourceFilter* pFileSource = nullptr;
pFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
pFileSource->Load(L"rtsp://example.com/stream", nullptr);
```
---

### Buffering Configuration

#### GetBufferingMode

Retrieves the current buffering mode.

**Syntax (C++)**:

```cpp
FFMPEG_SOURCE_BUFFERING_MODE GetBufferingMode();
```

**Syntax (C#)**:

```csharp
[PreserveSig]
FFMPEG_SOURCE_BUFFERING_MODE GetBufferingMode();
```

**Returns**: Current buffering mode (see enumeration below).

**Default**: `FFMPEG_SOURCE_BUFFERING_MODE_AUTO`

---
#### SetBufferingMode
Sets the buffering mode for live sources.
**Syntax (C++)**:
```cpp
HRESULT SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE mode);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE mode);
```
**Parameters**:
- `mode`: Buffering mode to use.
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Must be called **before** loading the source
- Affects latency and stability for live streams
**Buffering Modes**:
| Mode | Value | Description | Use Case |
|------|-------|-------------|----------|
| `FFMPEG_SOURCE_BUFFERING_MODE_AUTO` | 0 | Automatically detect if buffering is needed | Default - recommended for most scenarios |
| `FFMPEG_SOURCE_BUFFERING_MODE_ON` | 1 | Force buffering enabled | Use for unstable network streams |
| `FFMPEG_SOURCE_BUFFERING_MODE_OFF` | 2 | Force buffering disabled | Use for low-latency live streams |
**Example (C++)**:
```cpp
// Disable buffering for low-latency RTSP stream
pSettings->SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE_OFF);
pSettings->SetLoadTimeOut(5000); // 5-second timeout
```
**Example (C#)**:
```csharp
// Enable buffering for unstable network
settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.ON);
```
---

### Custom FFmpeg Options

#### SetCustomOption

Sets a custom FFmpeg option for the demuxer or decoder.

**Syntax (C++)**:

```cpp
HRESULT SetCustomOption(LPSTR name, LPSTR value);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int SetCustomOption([MarshalAs(UnmanagedType.LPStr)] string name,
                     [MarshalAs(UnmanagedType.LPStr)] string value);
```

**Parameters**:

- `name`: Option name (ASCII string).
- `value`: Option value (ASCII string).

**Returns**: `S_OK` (0) on success.

**Usage Notes**:

- Must be called **before** loading the source
- Allows passing any FFmpeg AVFormatContext or AVCodecContext option
- Options are passed directly to FFmpeg libraries
- Invalid options are ignored with a warning

**Common Options**:

| Option | Value | Description |
|--------|-------|-------------|
| `rtsp_transport` | `tcp` or `udp` | Force RTSP transport protocol |
| `timeout` | Microseconds | Network timeout for protocols |
| `buffer_size` | Bytes | Input buffer size |
| `analyzeduration` | Microseconds | Duration to analyze stream |
| `probesize` | Bytes | Size of data to probe |
| `fflags` | `nobuffer` | Disable buffering |
| `threads` | Number | Decoder thread count |

**Example (C++)**:

```cpp
// Configure RTSP to use TCP transport
pSettings->SetCustomOption("rtsp_transport", "tcp");

// Set network timeout to 5 seconds
pSettings->SetCustomOption("timeout", "5000000"); // 5 seconds in microseconds

// Increase probe size for better format detection
pSettings->SetCustomOption("probesize", "10000000"); // 10MB
```

**Example (C#)**:

```csharp
// Low-latency configuration
settings.SetCustomOption("fflags", "nobuffer");
settings.SetCustomOption("flags", "low_delay");
settings.SetCustomOption("probesize", "32");
```

---
#### ClearCustomOptions
Clears all previously set custom options.
**Syntax (C++)**:
```cpp
HRESULT ClearCustomOptions();
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int ClearCustomOptions();
```
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Must be called **before** loading the source
- Resets all custom options to FFmpeg defaults
**Example (C++)**:
```cpp
pSettings->ClearCustomOptions();
```
---

### Callback Configuration

#### SetDataCallback

Sets a callback function to receive decoded video/audio data.

**Syntax (C++)**:

```cpp
HRESULT SetDataCallback(FFMPEGDataCallbackDelegate callback);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int SetDataCallback([MarshalAs(UnmanagedType.FunctionPtr)] FFMPEGDataCallbackDelegate callback);
```

**Parameters**:

- `callback`: Pointer to callback function.

**Returns**: `S_OK` (0) on success.

**Callback Signature (C++)**:

```cpp
typedef HRESULT(_stdcall* FFMPEGDataCallbackDelegate) (
    BYTE* buffer,        // Pointer to data buffer
    int bufferLen,       // Buffer length in bytes
    int dataType,        // 0 = video, 1 = audio
    LONGLONG startTime,  // Start timestamp (100-nanosecond units)
    LONGLONG stopTime    // Stop timestamp (100-nanosecond units)
);
```

**Usage Notes**:

- Callback is invoked for each decoded frame/audio sample
- Called from filter's streaming thread - keep processing minimal
- Buffer data is valid only during callback execution
- Return `S_OK` from callback to continue processing

**Example (C++)**:

```cpp
HRESULT __stdcall DataCallback(BYTE* buffer, int bufferLen, int dataType,
                                LONGLONG startTime, LONGLONG stopTime)
{
    if (dataType == 0) // Video
    {
        // Process video frame
        ProcessVideoFrame(buffer, bufferLen, startTime);
    }
    else // Audio
    {
        // Process audio data
        ProcessAudioData(buffer, bufferLen);
    }
    return S_OK;
}

// Set callback
pSettings->SetDataCallback(&DataCallback);
```

---
#### SetTimestampCallback
Sets a callback function to receive timestamp information.
**Syntax (C++)**:
```cpp
HRESULT SetTimestampCallback(FFMPEGTimestampCallbackDelegate callback);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetTimestampCallback([MarshalAs(UnmanagedType.FunctionPtr)] FFMPEGTimestampCallbackDelegate callback);
```
**Parameters**:
- `callback`: Pointer to callback function.
**Returns**: `S_OK` (0) on success.
**Callback Signature (C++)**:
```cpp
typedef HRESULT(_stdcall* FFMPEGTimestampCallbackDelegate) (
    int mediaType,              // 0 = video, 1 = audio
    __int64 demuxerStartTime,   // Demuxer start time
    __int64 streamStartTime,    // Stream start time
    __int64 timestamp           // Current timestamp
);
```
**Usage Notes**:
- Useful for timestamp analysis and synchronization debugging
- Called for each decoded frame/sample
---

### Audio Control

#### SetAudioEnabled

Enables or disables audio stream processing.

**Syntax (C++)**:

```cpp
HRESULT SetAudioEnabled(BOOL enabled);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int SetAudioEnabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
```

**Parameters**:

- `enabled`: Set to `TRUE` to enable audio, `FALSE` to disable.

**Returns**: `S_OK` (0) on success.

**Usage Notes**:

- Must be called **before** loading the source
- When disabled, audio streams are not decoded (saves CPU/memory)
- Useful for video-only applications

**Example (C++)**:

```cpp
// Disable audio for video-only processing
pSettings->SetAudioEnabled(FALSE);
```

## Related Interfaces

- **IFileSourceFilter** - Standard DirectShow interface for loading files/URLs
- **IAMStreamSelect** - Select between multiple audio/video streams
- **IMediaSeeking** - Seek to specific positions in the media
- **IAMStreamConfig** - Configure video/audio format

## See Also

### Documentation

- [FFMPEG Source Filter Overview](index.md) - Product overview and features
- [Code Examples](examples.md) - Complete working code samples

### Interface Definitions

- [C# Interface (.NET)](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/dotnet/IFFmpegSourceSettings.cs) - Complete .NET interface definition
- [C++ Interface Header](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/cpp/FFMPEG%20Source/IFFmpegSourceSettings.h) - C++ header file
- [Delphi Interface](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/delphi/VCFiltersAPI.pas) - Delphi interface definition

### Working Samples

- [GitHub Samples Repository](https://github.com/visioforge/directshow-samples) - Complete working examples for all platforms

### External Resources

- [FFmpeg Documentation](https://ffmpeg.org/documentation.html) - FFmpeg library documentation
- [DirectShow SDK](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow) - Microsoft DirectShow documentation
