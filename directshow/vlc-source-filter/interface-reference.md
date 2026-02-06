---
title: VLC Source Filter - Interface Reference
description: IVlcSrc family interfaces for multi-track audio, subtitle support, and custom VLC command-line options in DirectShow applications.
---

# VLC Source Filter Interface Reference

## Overview

The VLC Source DirectShow filter exposes three progressive interfaces (`IVlcSrc`, `IVlcSrc2`, `IVlcSrc3`) that provide comprehensive control over media playback, audio/subtitle track selection, and VLC configuration. These interfaces enable developers to leverage VLC's powerful media framework within DirectShow applications.

## Interface Hierarchy

```
IUnknown
  └── IVlcSrc
        └── IVlcSrc2
              └── IVlcSrc3
```

Each interface extends the previous one, adding new capabilities while maintaining backward compatibility.

---
## IVlcSrc Interface
The base interface providing essential file loading and track selection capabilities.
### Interface Definition
- **Interface Name**: `IVlcSrc`
- **GUID**: `{77493EB7-6D00-41C5-9535-7C593824E892}`
- **Inherits From**: `IUnknown`
- **Header File**: `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)
### Methods
#### SetFile
Sets the media file or URL to play.
**Syntax (C++)**:
```cpp
HRESULT SetFile(WCHAR *file);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetFile([MarshalAs(UnmanagedType.LPWStr)] string file);
```
**Parameters**:
- `file`: Wide-character string containing the file path or URL.
**Returns**: `S_OK` (0) on success, error code otherwise.
**Supported Sources**:
- Local files: `C:\Videos\movie.mp4`
- HTTP streams: `https://example.com/stream.m3u8`
- RTSP streams: `rtsp://example.com/live`
- HLS playlists: `https://example.com/playlist.m3u8`
- DASH streams: `https://example.com/manifest.mpd`
- DVB-T/C/S broadcasts
- Network shares: `\\server\share\video.mkv`
**Example (C++)**:
```cpp
IVlcSrc* pVlcSrc = nullptr;
pFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
pVlcSrc->SetFile(L"C:\\Videos\\movie.mkv");
pVlcSrc->Release();
```
**Example (C#)**:
```csharp
var vlcSrc = filter as IVlcSrc;
if (vlcSrc != null)
{
    vlcSrc.SetFile(@"C:\Videos\movie.mkv");
}
```
---

### Audio Track Management

#### GetAudioTracksCount

Retrieves the total number of available audio tracks.

**Syntax (C++)**:

```cpp
HRESULT GetAudioTracksCount(int *count);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int GetAudioTracksCount(out int count);
```

**Parameters**:

- `count`: [out] Receives the number of audio tracks.

**Returns**: `S_OK` (0) on success.

**Usage Notes**:

- Call after the file is loaded and the filter graph is built
- Returns 0 if no audio tracks are available or file not loaded

**Example (C++)**:

```cpp
int audioCount = 0;
pVlcSrc->GetAudioTracksCount(&audioCount);
printf("Audio tracks: %d\n", audioCount);
```

---
#### GetAudioTrackInfo
Retrieves information about a specific audio track.
**Syntax (C++)**:
```cpp
HRESULT GetAudioTrackInfo(int number, int *id, WCHAR *name);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int GetAudioTrackInfo(int number, out int id,
                      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);
```
**Parameters**:
- `number`: Zero-based track index (0 to count-1).
- `id`: [out] Receives the track ID.
- `name`: [out] Buffer to receive the track name (must be pre-allocated, minimum 256 characters).
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Pre-allocate name buffer with at least 256 wide characters
- Track names typically include language and codec information
- Track ID is used with SetAudioTrack()
**Example (C++)**:
```cpp
int audioCount = 0;
pVlcSrc->GetAudioTracksCount(&audioCount);
for (int i = 0; i < audioCount; i++)
{
    int id = 0;
    WCHAR name[256] = {0};
    pVlcSrc->GetAudioTrackInfo(i, &id, name);
    wprintf(L"Track %d - ID: %d, Name: %s\n", i, id, name);
}
```
**Example (C#)**:
```csharp
int count = 0;
vlcSrc.GetAudioTracksCount(out count);
for (int i = 0; i < count; i++)
{
    int id;
    var name = new StringBuilder(256);
    vlcSrc.GetAudioTrackInfo(i, out id, name);
    Console.WriteLine($"Track {i} - ID: {id}, Name: {name}");
}
```
---

#### GetAudioTrack

Retrieves the ID of the currently active audio track.

**Syntax (C++)**:

```cpp
HRESULT GetAudioTrack(int *id);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int GetAudioTrack(out int id);
```

**Parameters**:

- `id`: [out] Receives the current audio track ID.

**Returns**: `S_OK` (0) on success.

**Example (C++)**:

```cpp
int currentTrack = 0;
pVlcSrc->GetAudioTrack(&currentTrack);
printf("Current audio track ID: %d\n", currentTrack);
```

---
#### SetAudioTrack
Sets the active audio track by ID.
**Syntax (C++)**:
```cpp
HRESULT SetAudioTrack(int id);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetAudioTrack(int id);
```
**Parameters**:
- `id`: The track ID to activate (obtained from GetAudioTrackInfo).
**Returns**: `S_OK` (0) on success, error code if track ID is invalid.
**Usage Notes**:
- Can be called during playback to switch tracks dynamically
- Use -1 to disable all audio tracks
- Track switching may cause brief audio interruption
**Example (C++)**:
```cpp
// Switch to second audio track
int trackId = 0;
pVlcSrc->GetAudioTrackInfo(1, &trackId, nullptr);
pVlcSrc->SetAudioTrack(trackId);
```
**Example (C#)**:
```csharp
// Switch to first audio track
int trackId;
var name = new StringBuilder(256);
vlcSrc.GetAudioTrackInfo(0, out trackId, name);
vlcSrc.SetAudioTrack(trackId);
```
---

### Subtitle Track Management

#### GetSubtitlesCount

Retrieves the total number of available subtitle tracks.

**Syntax (C++)**:

```cpp
HRESULT GetSubtitlesCount(int *count);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int GetSubtitlesCount(out int count);
```

**Parameters**:

- `count`: [out] Receives the number of subtitle tracks.

**Returns**: `S_OK` (0) on success.

**Example (C++)**:

```cpp
int subtitleCount = 0;
pVlcSrc->GetSubtitlesCount(&subtitleCount);
printf("Subtitle tracks: %d\n", subtitleCount);
```

---
#### GetSubtitleInfo
Retrieves information about a specific subtitle track.
**Syntax (C++)**:
```cpp
HRESULT GetSubtitleInfo(int number, int *id, WCHAR *name);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int GetSubtitleInfo(int number, out int id,
                    [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);
```
**Parameters**:
- `number`: Zero-based track index (0 to count-1).
- `id`: [out] Receives the subtitle track ID.
- `name`: [out] Buffer to receive the subtitle track name (minimum 256 characters).
**Returns**: `S_OK` (0) on success.
**Example (C++)**:
```cpp
int subCount = 0;
pVlcSrc->GetSubtitlesCount(&subCount);
for (int i = 0; i < subCount; i++)
{
    int id = 0;
    WCHAR name[256] = {0};
    pVlcSrc->GetSubtitleInfo(i, &id, name);
    wprintf(L"Subtitle %d - ID: %d, Name: %s\n", i, id, name);
}
```
---

#### GetSubtitle

Retrieves the ID of the currently active subtitle track.

**Syntax (C++)**:

```cpp
HRESULT GetSubtitle(int *id);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int GetSubtitle(out int id);
```

**Parameters**:

- `id`: [out] Receives the current subtitle track ID.

**Returns**: `S_OK` (0) on success.

---
#### SetSubtitle
Sets the active subtitle track by ID.
**Syntax (C++)**:
```cpp
HRESULT SetSubtitle(int id);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetSubtitle(int id);
```
**Parameters**:
- `id`: The subtitle track ID to activate.
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Use -1 to disable subtitles
- Subtitle rendering is performed by VLC's internal renderer
- Can be switched during playback
**Example (C++)**:
```cpp
// Enable first subtitle track
int subtitleId = 0;
pVlcSrc->GetSubtitleInfo(0, &subtitleId, nullptr);
pVlcSrc->SetSubtitle(subtitleId);
// Disable subtitles
pVlcSrc->SetSubtitle(-1);
```
---

## IVlcSrc2 Interface

Extends `IVlcSrc` with custom VLC command-line parameter support.

### Interface Definition

- **Interface Name**: `IVlcSrc2`
- **GUID**: `{CCE122C0-172C-4626-B4B6-42B039E541CB}`
- **Inherits From**: `IVlcSrc`
- **Header File**: `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)

### Methods

#### SetCustomCommandLine

Sets custom VLC command-line parameters.

**Syntax (C++)**:

```cpp
HRESULT SetCustomCommandLine(char* params[], int length);
```

**Syntax (C#)**:

```csharp
[PreserveSig]
int SetCustomCommandLine([In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] IntPtr[] params_,
                         int size);
```

**Parameters**:

- `params_`: Array of IntPtr pointers to UTF-8 encoded strings containing VLC command-line parameters.
- `size`: Number of parameters in the array.

**Returns**: `S_OK` (0) on success.

**Usage Notes**:

- Must be called **before** loading the media file with SetFile()
- Parameters must be converted to native UTF-8 IntPtr using StringHelper.NativeUtf8FromString()
- Memory allocated for IntPtr parameters must be freed after the call using Marshal.FreeHGlobal()
- Parameters are passed directly to libVLC initialization
- Invalid parameters are ignored with warnings in VLC log
- Use standard VLC command-line syntax (see VLC documentation)

**Common VLC Parameters**:

| Parameter | Description | Example Value |
|-----------|-------------|---------------|
| `--network-caching` | Network caching in ms | `1000` |
| `--file-caching` | File caching in ms | `300` |
| `--live-caching` | Live stream caching in ms | `300` |
| `--avcodec-hw` | Hardware acceleration | `any`, `dxva2`, `d3d11va` |
| `--verbose` | Logging verbosity | `2` |
| `--rtsp-tcp` | Force RTSP over TCP | (flag, no value) |
| `--no-audio` | Disable audio | (flag, no value) |
| `--sout-mux-caching` | Output muxer caching | `1000` |

**Example (C++)**:

```cpp
IVlcSrc2* pVlcSrc2 = nullptr;
pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);

// Configure for low-latency RTSP
char* params[] = {
    "--network-caching=300",
    "--rtsp-tcp",
    "--avcodec-hw=d3d11va",
    "--verbose=2"
};

pVlcSrc2->SetCustomCommandLine(params, 4);
pVlcSrc2->SetFile(L"rtsp://192.168.1.100/stream");

pVlcSrc2->Release();
```

**Example (C#)**:

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VisioForge.Core.Helpers;

var vlcSrc2 = filter as IVlcSrc2;
if (vlcSrc2 != null)
{
    // Enable hardware acceleration and adjust caching
    var parameters = new List<string>
    {
        "--avcodec-hw=any",
        "--network-caching=1000",
        "--file-caching=300"
    };

    // Convert strings to native UTF-8 IntPtr array
    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, parameters.Count);
        vlcSrc2.SetFile(@"C:\Videos\movie.mkv");
    }
    finally
    {
        // Free allocated unmanaged memory
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

**Example (Delphi)**:

```delphi
var
  VlcSrc2: IVlcSrc2;
  Params: array[0..2] of PAnsiChar;
begin
  if Succeeded(Filter.QueryInterface(IID_IVlcSrc2, VlcSrc2)) then
  begin
    Params[0] := '--network-caching=500';
    Params[1] := '--rtsp-tcp';
    Params[2] := '--avcodec-hw=dxva2';

    VlcSrc2.SetCustomCommandLine(@Params, 3);
    VlcSrc2.SetFile('rtsp://example.com/stream');
  end;
end;
```

---
## IVlcSrc3 Interface
Extends `IVlcSrc2` with frame rate override capability.
### Interface Definition
- **Interface Name**: `IVlcSrc3`
- **GUID**: `{3DFBED0C-E4A8-401C-93EF-CBBFB65223DD}`
- **Inherits From**: `IVlcSrc2`
- **Header File**: `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)
### Methods
#### SetDefaultFrameRate
Sets a default frame rate for media without frame rate information.
**Syntax (C++)**:
```cpp
HRESULT SetDefaultFrameRate(double frameRate);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetDefaultFrameRate(double frameRate);
```
**Parameters**:
- `frameRate`: Frame rate in frames per second (e.g., 29.97, 30.0, 25.0, 60.0).
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Must be called **before** loading the media file
- Used when source media doesn't specify frame rate
- Particularly useful for network streams without timing information
- Common values: 23.976, 24.0, 25.0, 29.97, 30.0, 50.0, 59.94, 60.0
**Example (C++)**:
```cpp
IVlcSrc3* pVlcSrc3 = nullptr;
pFilter->QueryInterface(IID_IVlcSrc3, (void**)&pVlcSrc3);
// Set default frame rate for MJPEG IP camera stream
pVlcSrc3->SetDefaultFrameRate(30.0);
pVlcSrc3->SetFile(L"http://192.168.1.50/video.mjpg");
pVlcSrc3->Release();
```
**Example (C#)**:
```csharp
var vlcSrc3 = filter as IVlcSrc3;
if (vlcSrc3 != null)
{
    // Set PAL frame rate for DV stream
    vlcSrc3.SetDefaultFrameRate(25.0);
    vlcSrc3.SetFile(@"dv://0");
}
```
---

## Complete Usage Examples

### Example 1: Multi-Language Movie Playback (C++)

```cpp
#include <dshow.h>
#include "ivlcsrc.h"

void PlayMovieWithAudioSelection(IBaseFilter* pVlcFilter)
{
    HRESULT hr;
    IVlcSrc* pVlcSrc = nullptr;

    hr = pVlcFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
    if (FAILED(hr))
        return;

    // Load movie
    pVlcSrc->SetFile(L"C:\\Movies\\multilang_movie.mkv");

    // Build and run the graph here...
    // (IGraphBuilder::RenderFile, IMediaControl::Run, etc.)

    // Enumerate audio tracks
    int audioCount = 0;
    pVlcSrc->GetAudioTracksCount(&audioCount);

    wprintf(L"Available audio tracks:\n");
    for (int i = 0; i < audioCount; i++)
    {
        int id = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetAudioTrackInfo(i, &id, name);
        wprintf(L"  [%d] %s (ID: %d)\n", i, name, id);
    }

    // Select English audio track (assuming it's track 1)
    int englishTrackId = 0;
    pVlcSrc->GetAudioTrackInfo(1, &englishTrackId, nullptr);
    pVlcSrc->SetAudioTrack(englishTrackId);

    // Enable subtitles
    int subCount = 0;
    pVlcSrc->GetSubtitlesCount(&subCount);
    if (subCount > 0)
    {
        int subId = 0;
        pVlcSrc->GetSubtitleInfo(0, &subId, nullptr);
        pVlcSrc->SetSubtitle(subId);
    }

    pVlcSrc->Release();
}
```

### Example 2: Low-Latency RTSP Stream (C#)

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;
using VisioForge.Core.Helpers;
using VisioForge.DirectShowAPI;

public class VLCRTSPPlayer
{
    public void SetupLowLatencyRTSP(IBaseFilter vlcFilter)
    {
        // Get IVlcSrc3 interface (highest version)
        var vlcSrc3 = vlcFilter as IVlcSrc3;
        if (vlcSrc3 == null)
            throw new NotSupportedException("IVlcSrc3 not available");

        // Configure VLC for minimal latency
        var parameters = new List<string>
        {
            "--network-caching=50",       // Minimal network buffer
            "--live-caching=50",          // Minimal live buffer
            "--rtsp-tcp",                 // Use TCP transport
            "--no-audio-time-stretch",    // Disable audio stretching
            "--avcodec-hw=d3d11va",      // Hardware decoding
            "--verbose=0"                 // Reduce logging
        };

        // Convert to IntPtr array
        var array = new IntPtr[parameters.Count];
        for (int i = 0; i < parameters.Count; i++)
        {
            array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
        }

        try
        {
            int hr = vlcSrc3.SetCustomCommandLine(array, parameters.Count);
            DsError.ThrowExceptionForHR(hr);

            // Set frame rate for IP camera
            hr = vlcSrc3.SetDefaultFrameRate(25.0);
            DsError.ThrowExceptionForHR(hr);

            // Load RTSP stream
            hr = vlcSrc3.SetFile("rtsp://admin:password@192.168.1.100:554/stream1");
            DsError.ThrowExceptionForHR(hr);
        }
        finally
        {
            // Free allocated memory
            for (int i = 0; i < array.Length; i++)
            {
                Marshal.FreeHGlobal(array[i]);
            }
        }

        // Build filter graph and start playback...
    }
}
```

### Example 3: Subtitle Track Switching UI (Delphi)

```delphi
unit VLCSubtitles;

interface

uses
  Winapi.Windows, System.Classes, Vcl.Controls, Vcl.StdCtrls,
  DSPack, ivlcsrc;

type
  TSubtitleForm = class(TForm)
    ComboBoxSubtitles: TComboBox;
    procedure FormCreate(Sender: TObject);
    procedure ComboBoxSubtitlesChange(Sender: TObject);
  private
    FVlcSrc: IVlcSrc;
    FSubtitleIDs: TArray<Integer>;
    procedure LoadSubtitleTracks;
  public
    procedure SetVLCFilter(Filter: IBaseFilter);
  end;

implementation

procedure TSubtitleForm.SetVLCFilter(Filter: IBaseFilter);
begin
  if Succeeded(Filter.QueryInterface(IID_IVlcSrc, FVlcSrc)) then
  begin
    LoadSubtitleTracks;
  end;
end;

procedure TSubtitleForm.LoadSubtitleTracks;
var
  Count, I, ID: Integer;
  Name: array[0..255] of WideChar;
begin
  ComboBoxSubtitles.Clear;
  ComboBoxSubtitles.Items.Add('Disabled');

  if FVlcSrc.GetSubtitlesCount(Count) = S_OK then
  begin
    SetLength(FSubtitleIDs, Count + 1);
    FSubtitleIDs[0] := -1; // Disabled

    for I := 0 to Count - 1 do
    begin
      if FVlcSrc.GetSubtitleInfo(I, ID, Name) = S_OK then
      begin
        FSubtitleIDs[I + 1] := ID;
        ComboBoxSubtitles.Items.Add(Name);
      end;
    end;
  end;

  ComboBoxSubtitles.ItemIndex := 0;
end;

procedure TSubtitleForm.ComboBoxSubtitlesChange(Sender: TObject);
var
  Index: Integer;
begin
  Index := ComboBoxSubtitles.ItemIndex;
  if (Index >= 0) and (Index < Length(FSubtitleIDs)) then
  begin
    FVlcSrc.SetSubtitle(FSubtitleIDs[Index]);
  end;
end;

end.
```

## Best Practices

### Track Management

1. **Always enumerate tracks after building the filter graph** - Track information is not available until the source is loaded
2. **Handle files with no audio/subtitles gracefully** - Check count before accessing tracks
3. **Pre-allocate name buffers with 256 characters** - Prevents buffer overruns
4. **Cache track IDs** - Don't repeatedly call GetAudioTrackInfo/GetSubtitleInfo

### VLC Configuration

1. **Use IVlcSrc3 when available** - Provides full feature set
2. **Set custom parameters before loading file** - Parameters only apply at initialization
3. **Test VLC parameters independently** - Use VLC command-line to verify parameters work
4. **Use appropriate caching values**:
   - Local files: 300ms
   - Network streams: 1000-3000ms
   - Low-latency streams: 50-300ms

### Hardware Acceleration

1. **Enable hardware decoding for H.264/H.265**:

   ```cpp
   "--avcodec-hw=any"  // Auto-detect best method
   ```

2. **Platform-specific options**:
   - Windows: `d3d11va`, `dxva2`
   - All platforms: `any` (auto-detect)

### Performance

1. **Minimize network caching for live streams** - Reduces latency
2. **Use RTSP over TCP when UDP fails** - More reliable through firewalls
3. **Enable verbose logging for debugging only** - Reduces performance overhead

## Related Interfaces

- **IFileSourceFilter** - Alternative standard DirectShow interface for loading files
- **IAMStreamSelect** - DirectShow standard for stream selection (also supported by VLC filter)
- **IMediaSeeking** - Seek control in media
- **IBasicVideo** - Video window control

## See Also

- [VLC Source Filter Overview](index.md)
- [VLC Command-Line Documentation](https://www.videolan.org/doc/)
- [Code Examples](examples.md)
