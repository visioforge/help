---
title: Virtual Camera .NET Push API — Send Frames to a Webcam
description: Push BGRA, BGR24 or YUY2 frames and PCM audio from C# to the VisioForge virtual webcam and microphone with the VisioForge.DotNet.VirtualCamera package.
sidebar_label: .NET Push API
order: 2
tags:
  - Virtual Camera SDK
  - .NET
  - C#
  - Windows
  - Virtual Camera
primary_api_classes:
  - VirtualVideoWriter
  - VirtualAudioWriter

---

# Virtual Camera .NET Push API

The `VisioForge.DotNet.VirtualCamera` NuGet package sends video frames and audio samples from your .NET application
straight to the VisioForge Virtual Camera and Virtual Audio Card. Zoom, OBS Studio and browsers see the camera as an
ordinary webcam, and DirectShow applications also see the Virtual Audio Card as a microphone. No DirectShow graph is
needed on your side.

The package has no dependency on the rest of the VisioForge SDK. It targets .NET Framework 4.7.2, .NET 8 and .NET 10
on Windows. Its types live in the `VisioForge.VirtualCamera` namespace, so it can be referenced next to
`VisioForge.DotNet.Core`, which carries the same writers in `VisioForge.Core.VirtualCamera.Push`. A file that also
imports `VisioForge.Core.VirtualCamera.Push` qualifies `VirtualVideoWriter` and `VirtualAudioWriter`, and one that imports
`VisioForge.Core` qualifies `BaseContext`; a `using` alias works as well.

## Install

```xml
<PackageReference Include="VisioForge.DotNet.VirtualCamera" Version="[2026.11.6]" />
<!-- The virtual camera and virtual audio card filters -->
<PackageReference Include="VisioForge.DotNet.Core.Redist.VirtualCamera.x64" Version="[2026.11.6]" />
```

`Redist.VirtualCamera.x64` fits whatever your application's architecture: the consumer application loads the filter,
and Zoom, OBS and browsers are 64-bit (see below for other consumers). Each `Redist.VirtualCamera` package
brings the `Redist.Shared` package of its architecture, with the `VisioForge_MFP*.dll` and `VisioForge_MFPX*.dll` its
filters load (and `vcomp140.dll` on x64). An application that is not x64 and calls `PushFrameBgra` or `PushFrameBgr24`
also references `Redist.Shared` of its own architecture (`.x86` or `.arm64`) for the color conversion.

## Register the Filters

The source filters must be COM-registered once on the machine, with administrator rights. The redist package copies
them to the application folder:

```csharp
using System;
using System.Diagnostics;
using System.IO;

var dir = AppContext.BaseDirectory;
var args = $"/s \"{Path.Combine(dir, "VisioForge_Virtual_Camera_x64.ax")}\" " +
           $"\"{Path.Combine(dir, "VisioForge_Virtual_Audio_Card_x64.ax")}\"";

// One UAC prompt for both filters.
var info = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "regsvr32.exe"), args)
{
    UseShellExecute = true,
    Verb = "runas",
};

using (var process = Process.Start(info))
{
    process.WaitForExit();
    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException($"regsvr32 failed with exit code {process.ExitCode}.");
    }
}
```

The consumer application loads the filter build that matches its own bitness, whatever the bitness of the application
that pushes the frames. The x64 filters above serve 64-bit Zoom, OBS and browsers. For 32-bit consumers, also
register `VisioForge_Virtual_Camera.ax` and `VisioForge_Virtual_Audio_Card.ax` from the `.x86` redist; for ARM64-native
consumers, `VisioForge_Virtual_Camera_arm64.ax` and `VisioForge_Virtual_Audio_Card_arm64.ax` from the `arm64` subfolder
that the `.arm64` redist creates in the application folder.

An installer can register the same files instead; see [Filter Registration](../deployment/filter-registration.md).

## Push Video

```csharp
using VisioForge.VirtualCamera;

const int width = 1280, height = 720;

// The last argument is your Virtual Camera SDK license key; null shows the TRIAL banner.
using (var video = new VirtualVideoWriter(width, height, 30, "VideoStream1", null, "YOUR-LICENSE-KEY"))
{
    video.Start();

    // Every 1/30 s: a BGRA frame, top row first, stride in bytes.
    video.PushFrameBgra(bgraPixels, width * 4);

    video.Stop();
}
```

`VirtualVideoWriter` takes frames in three layouts:

| Method | Byte order | Typical source |
|---|---|---|
| `PushFrameBgra(data, stride)` | B, G, R, A per pixel, rows top to bottom; alpha is ignored | WPF `Bgra32` (`Pbgra32` with opaque pixels), GDI+ `Format32bppArgb`, Direct3D `B8G8R8A8` |
| `PushFrameBgr24(data, stride)` | B, G, R per pixel, rows top to bottom | WPF `Bgr24`, GDI+ `Format24bppRgb`, OpenCV `CV_8UC3` |
| `PushFrame(data)` | YUY2, exactly `width * height * 2` bytes | your own encoder or converter |

The width must be even. `PushFrameBgra` and `PushFrameBgr24` convert to YUY2 natively in `VisioForge_MFPX64.dll`
(`VisioForge_MFPX.dll` on x86, `VisioForge_MFPX_ARM64.dll` on ARM64). Each push returns `false` when no buffer is free, so a slow consumer drops a frame rather
than blocking your thread.

## Push Audio

```csharp
// 48 kHz stereo 16-bit PCM for the "VisioForge Virtual Audio Card" microphone.
using (var audio = new VirtualAudioWriter(48000, 2, 16, "AudioStream1", null))
{
    audio.Start();

    audio.PushSamples(pcmBytes); // a whole number of 4-byte sample frames

    audio.Stop();
}
```

Timestamps come from the number of bytes pushed, so push audio at the rate it is played.

## Logging

The writers take a `BaseContext` for their warnings and errors; `null` sends them to `System.Diagnostics.Trace`. To
route them elsewhere, derive from `BaseContext` and override `Error` and `Warning`.

## Licensing

Without a license key the virtual camera draws a TRIAL strip across the picture. Pass the key you received with your
Virtual Camera SDK purchase to the `VirtualVideoWriter` constructor; the key in
`HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\License` works as well.

## Demo

The **Virtual Camera Push Demo** (WPF, C#) in the [SDK samples repository](https://github.com/visioforge/.Net-SDK-s-samples)
draws a moving picture with WPF, pushes it with `PushFrameBgra`, sends a 440 Hz tone to the virtual microphone and
registers the filters from a button.

## See Also

* [Virtual Camera SDK](index.md)
* [Examples](examples.md)
