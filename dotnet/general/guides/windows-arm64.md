---
title: Run the VisioForge .NET SDK Natively on Windows on Arm
description: Build a native Windows on Arm app with Media Blocks SDK .NET. ARM64 runtime packages, project settings, and the packaging traps of Smart App Control.
sidebar_label: Windows on ARM64
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - ARM64
  - WinForms
  - GStreamer
  - NuGet
  - Deployment
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock
---

# Run the VisioForge .NET SDK Natively on Windows on Arm

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk){ .md-button .md-button--primary target="_blank" }

On a Windows on Arm machine an x64 build of your app runs under emulation. Media Blocks SDK .NET also
runs as a native ARM64 process, which is faster and uses the machine's own ARM64 libraries. This page
shows what to set in the project file, which packages to reference, and where the two known problems are.

## What runs natively

| Engine | In a native ARM64 process |
|---|---|
| Media Blocks SDK .NET (`MediaBlocksPipeline`) | Yes. GStreamer, the codec plugins, the video renderer and the file decoders are ARM64. |
| `VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX` | Yes. |
| `VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore` (DirectShow) | Partly. `VisioForge.DotNet.Core.Redist.Base.arm64` carries the base filters, the video effects, the video and audio mixers, the WASAPI audio renderer and Desktop Duplication screen capture; `Redist.MP4.arm64` the MP4, MPEG-TS, MKV and HLS outputs with the Media Foundation H.264/H.265 encoders. The LAV filters and the legacy x64 DirectShow encoders and muxers have no ARM64 build: a DirectShow app that decodes files through LAV fails in an ARM64 process, so build it for x64 and let Windows run it under emulation. |

NDI, ElevenLabs and the Basler, Allied Vision and Spinnaker camera SDKs have no ARM64 build, and
their blocks report an error in an ARM64 process. GenICam cameras work natively through
`VisioForge.CrossPlatform.GenICam.Windows.arm64`.

## Project settings

```xml
<PropertyGroup>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>ARM64</PlatformTarget>
  <RuntimeIdentifier>win-arm64</RuntimeIdentifier>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.arm64" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.arm64" Version="2026.*" />
</ItemGroup>
```

`PlatformTarget` and the packages have to agree. With `PlatformTarget` set to `x64` and the ARM64
packages referenced, the app still builds as an x64 process, runs under emulation and tries to load x64
natives.

`VisioForge.DotNet.MediaBlocks` is the same managed package for every architecture; only the native
runtime packages differ. `Core.Windows.arm64` holds the GStreamer runtime. `Libav.Windows.arm64` holds the FFmpeg plugins
(`gstlibav`) that decode H.264, HEVC and the other common formats through the file source; the Core
package does not include them. Add `VisioForge.CrossPlatform.Core.Windows.Adds.arm64` only for the
optional plugins it carries, as you would on x64.

Build and publish on any machine, including an x64 one:

```powershell
dotnet build -c Release -r win-arm64
dotnet publish -c Release -r win-arm64
```

The output folder holds the same `arm64` subfolder with the native runtime that the x64 build puts in
`x64`, so no extra copy step is needed.

## Known problems

**Duplicate publish output (`NETSDK1152`).** Earlier ARM64 packages shipped the FFmpeg DLLs in both
`Core.Windows.arm64` and `Libav.Windows.arm64`. `dotnet build` passed, and `dotnet publish -r win-arm64`
failed with *Found multiple publish output files with the same relative path*. Fixed in the current
packages: keep both references on a current 2026 release and publish works.

**Smart App Control.** On a clean Windows 11 Arm installation Smart App Control refuses to load anything
that is not Authenticode-signed. The files inside the released VisioForge packages are signed; your
application EXE is not, unless you sign it, and Smart App Control blocks it, so the app does not start. If
you build the SDK from source, its assemblies are unsigned too, and the SDK reports a silent "module not
found". Sign the EXE and the DLLs you ship, or develop on a machine with Smart App Control off or in
evaluation mode.

## Complete sample

**[ARM64 Player Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/ARM64%20Player%20Demo)**
is a WinForms app that plays a file into a live preview and prints the process architecture, so you can
see it really runs as `Arm64`. Its project file is the settings above plus the ARM64 runtime packages, and
its readme lists the ARM64 differences in detail.
