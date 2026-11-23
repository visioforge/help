---
title: .NET SDK Troubleshooting — Common Errors and Fixes
description: Fix common VisioForge .NET SDK errors — missing filters and DLLs, MF Mux, REGDB_E_CLASSNOTREG, target framework, MSB3821, trial overlay and NVENC.
sidebar_label: Troubleshooting
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - Deployment
  - NuGet
  - Troubleshooting
  - License Activation
search:
  boost: 2
primary_api_classes:
  - VideoCaptureCore
  - MediaPlayerCore
  - VideoEditCore
  - VideoView

---

# Troubleshooting Common .NET SDK Errors

These are the errors developers run into most often when they first build or deploy an application with Video Capture SDK .NET, Media Player SDK .NET, Video Edit SDK .NET or Media Blocks SDK .NET. Most of them are deployment problems: a native component the application needs is not next to it or not registered.

When none of the answers below fits, implement the `OnError` event of your engine object, reproduce the problem and [send the logs](sendlogs.md) to support.

## Deployment errors

### "Unable to create filter" or "Unable to add … filter"

Messages such as `Unable to create filter: VisioForge Resize`, `Unable to add Video Effects filter`, `Unable to create LAV Splitter Source filter` or `Unable to add filter: … REGDB_E_CLASSNOTREG (0x80040154)` come from the Windows engines (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`). They mean that a DirectShow filter the engine needs is not available on that computer.

The fix is to deploy the redistributables:

1. **NuGet (recommended):** add the `VisioForge.DotNet.Core.Redist.*` packages your features need — the base package, the package of your SDK and the feature packages (MP4, FFMPEG, LAV, VLC and so on). Their files are copied to the output folder on build and are loaded from there without registration.
2. **Installer or manual deployment:** run the redistributable installer, or copy and register the filters as the deployment guide describes.

The package bitness must match the process: an x64 process cannot load x86 filters. If it works on the development machine but not on a clean one, compare the output folders — a package that is only installed on the development machine is the usual cause.

The full package lists are in the deployment guides: [Video Capture SDK](../videocapture/deployment.md), [Media Player SDK](../mediaplayer/deployment.md), [Video Edit SDK](../videoedit/deployment.md).

### "Unable to create VisioForge MF Mux filter"

The Media Foundation outputs of `VideoCaptureCore` — `MP4HWOutput`, `MPEGTSOutput` and the other outputs that use Microsoft or GPU encoders — write the file through the VisioForge MF Mux filter, which ships in the MP4 redistributable. Add the `VisioForge.DotNet.Core.Redist.MP4.x64` package (or `.x86`, `.arm64` to match your platform). The same package is required by the low-latency RTSP source.

### DllNotFoundException: Unable to load DLL 'VisioForge_MFP64.dll'

`VisioForge_MFP64.dll` (and `VisioForge_MFP.dll` for x86) is a native library of the SDK. It comes with the `VisioForge.DotNet.Core.Redist.Base` packages for the Windows engines and with the `VisioForge.CrossPlatform.Core.Windows` packages for the X engines and Media Blocks SDK .NET. The exception means it is not in the application folder or its bitness does not match the process:

- Add the redistributable package (for example `VisioForge.DotNet.Core.Redist.Base.x64`) to the application project itself, not only to a class library it references.
- Build for an explicit platform (`x64` or `x86`) rather than AnyCPU when you deploy only one set of redistributables.
- If the packages are referenced but the file is still missing, remove and re-add them (or delete the `bin` and `obj` folders) so the copy step runs again.

### Could not find libgstreamer-1.0 or VideoCaptureCore is not found

The Windows engines (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) and the Windows UI controls only exist in the Windows build of the SDK. A project whose target framework has no `-windows` suffix (for example `net8.0`) gets the cross-platform build: the Windows types are not found by the compiler, and an X engine or Media Blocks pipeline looks for the Linux GStreamer libraries (`libgstreamer-1.0.so`) on start.

Use a Windows target framework, for example:

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

Also check that the application runs as the same platform (x64 or x86) as the redistributable packages you added.

### Error MSB3821 on .resx files in a downloaded sample

`Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file` means Windows marked the files of a downloaded ZIP archive as untrusted. Unblock the archive before you extract it, or clone the [samples repository](https://github.com/visioforge/.Net-SDK-s-samples) with git. The steps are in [WinForms RESX files build issue](../deployment-x/Windows.md#winforms-resx-files-build-issue).

## Upgrading from older versions

### Old class names such as VFMP4v8v10Output or the VideoCapture control are not found

Old output class names map to today's API: `VFMP4v8v10Output` (renamed `VFMP4Output` in version 14.2) is now `MP4Output`. Version 15 split the single `VideoCapture`, `MediaPlayer` and `VideoEdit` UI controls into a `VideoView` control that renders video and an engine class (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) that keeps the familiar property and method API. Create the engine in code and pass it the `VideoView`, as the [samples](https://github.com/visioforge/.Net-SDK-s-samples) do. For the current version, see the [migration guide](../migration-from-v15.md).

## Licensing and activation { #licensing }

### A "TRIAL" or "UNREGISTERED" mark is drawn on the video

That overlay is drawn by the 30-day trial: an orange TRIAL tag and the purchase page, visioforge.com/buy, in the top-left corner. `VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`, `VideoCaptureCoreX` and `MediaPlayerCoreX` add a line naming the edition the features in use require (STANDARD, PROFESSIONAL or PREMIUM). Earlier SDK versions draw the word UNREGISTERED instead of TRIAL. A purchased license comes as a `.vflicense` certificate file: load it into every engine instance with `SetLicenseCertificateAsync` before you start it. See [Video Capture SDK .NET](../videocapture/index.md) for an example. If the trial overlay is visible but nothing plays, the message is not the cause — check the `OnError` event for the real error.

## Hardware encoding

### The NVIDIA NVENC encoder works on one PC but not on another

When NVENC fails on one computer (an `Unable to activate encoder` message, no output file) while it works on another, check the following:

- Install the latest driver from the NVIDIA website; the drivers that Windows Update installs can be older than the NVENC version the SDK uses.
- GeForce cards limit the number of encoding sessions that may run at the same time. Close other applications that encode (OBS, browsers sharing the screen, recorders) and test again.
- Older GPUs do not support every codec, for example HEVC. Try H.264 first.
- Test the same settings with the demo application of the SDK. If the demo fails too, [send the logs](sendlogs.md).

Use another encoder (Intel Quick Sync, AMD AMF or a software encoder) as the fallback for machines without a working NVENC. The encoder options are in [H.264 encoders](video-encoders/h264.md).
