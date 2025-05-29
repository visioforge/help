---
title: Video Capture SDK .NET Deployment Guide
description: Learn how to deploy Video Capture SDK .NET applications using NuGet packages, silent installers, or manual installation. Includes step-by-step instructions for x86/x64 architectures and component-specific deployment options.
sidebar_label: Deployment
order: 0

---

# Comprehensive Deployment Guide for Video Capture SDK .Net

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net)

When deploying the Video Capture SDK .Net to systems without the SDK pre-installed, proper component deployment is essential for functionality. For AnyCPU applications, both x86 and x64 redistributables must be deployed to ensure compatibility across different system architectures.

## Engine Options Overview

### VideoCaptureCoreX Engine (Cross-Platform Compatibility)

For cross-platform deployment scenarios, refer to our comprehensive [deployment guide](../deployment-x/index.md) which details platform-specific requirements and configuration options.

### VideoCaptureCore Engine (Windows Platform)

The VideoCaptureCore engine is optimized specifically for Windows environments and offers multiple deployment approaches based on your application requirements and target environment constraints.

## Deployment Methods

### NuGet Package Distribution (No Administrator Privileges Required)

The NuGet package approach provides a streamlined deployment method that doesn't require administrator privileges, making it ideal for restricted environments or when deploying to multiple systems without elevated access.

Add the required NuGet packages to your application project, and after building, the necessary redistributable files will be automatically included in your application folder. This method simplifies dependency management while ensuring all required components are available.

#### Essential NuGet Packages

**Core Components (Required):**

- SDK Base Package: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64)
- Video Capture SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64)

**Feature-Specific Packages:**

- FFMPEG Integration (for file output/network streaming): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64)
- MP4 Output Support: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64)
- VLC Source Integration (for file/IP camera sources): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64)
- WebM Output Format: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86)
- XIPH Format Support (Ogg, Vorbis, FLAC): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64)
- LAV Filters: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64)
- Virtual Camera Support: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VirtualCamera.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VirtualCamera.x64)

> **Note:** When using the Virtual Camera package, additional registration of camera files is required as outlined in the Manual Installation section if you want the virtual camera to be accessible from external applications.

### Silent Installer Deployment (Administrator Privileges Required)

For scenarios where administrator access is available, silent installers provide a streamlined deployment approach that handles component registration automatically.

**Core Components:**

- Base Package (required): [x86](http://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)
- .NET Assemblies: Can be installed in the Global Assembly Cache (GAC) or used from a local folder

**Feature-Specific Installers:**

- FFMPEG Integration: [x86](http://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- MP4 Output Support: [x86](http://files.visioforge.com/redists_net/redist_dotnet_mp4_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_mp4_x64.exe)
- VLC Source Integration: [x86](http://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)
- Additional Format Support: WebM ([x86](http://files.visioforge.com/redists_net/redist_dotnet_webm_x86.exe)) and XIPH formats ([x86](http://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe))
- LAV Filters: [x86](http://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

> **Uninstallation Note:** To remove the package, run the installer executable with administrator privileges using the `/x //` parameter.

### Manual Installation Process

For complete control over the deployment process or in environments with specific requirements, manual installation provides the most flexibility:

1. **Runtime Dependencies:** Install or copy the VC++ 2022 (v143) runtime (x86/x64) and OpenMP runtime DLLs. With admin rights, use exe redist or MSM modules; otherwise, copy directly to the application folder.

2. **Core Components:** Copy the `VisioForge_MFP`/`VisioForge_MFPX` DLLs (or x64 versions) from the `Redist\Filters` folder to your application directory.

3. **.NET Assemblies:** Either copy the assemblies to your application folder or install them to the GAC.

4. **DirectShow Filters:** Copy the SDK DirectShow filters to either your application folder or a designated redist folder (configured via the `CustomRedist_Path` property).

5. **Configuration:** Set the `CustomRedist_Enabled` property to `true` in the Window Load event.

6. **Architecture Handling:** For LAV filters (which use identical names for both x64 and x86 versions), use separate redist folders for each architecture.

7. **Path Configuration:** If your application executable resides in a different location, add the filter folder to the system `PATH` environment variable.

#### Core Components

**Basic Features:**

- Base Filters: VisioForge_BaseFilters.ax / VisioForge_BaseFilters_x64.ax
- Video Effects: VisioForge_Video_Effects_Pro.ax / VisioForge_Video_Effects_Pro_x64.ax
- Audio Processing: VisioForge_MP3_Splitter.ax / VisioForge_MP3_Splitter_x64.ax, VisioForge_Audio_Mixer.ax / VisioForge_Audio_Mixer_x64.ax

**Legacy Audio Effects:**

- VisioForge_Audio_Effects_4.ax / VisioForge_Audio_Effects_4_x64.ax

#### Format-Specific Components

**MP3 Output:**

- VisioForge_LAME.ax / VisioForge_LAME_x64.ax

**MP4/M4A Output:**

- Legacy Version: VisioForge_AAC_Encoder.ax, VisioForge_H264_Encoder_XP.ax, VisioForge_MP4_Muxer.ax with supporting libraries
- Version 10: VisioForge_AAC_Encoder_v10.ax, VisioForge_H264_Encoder.ax, VisioForge_MP4_Muxer_v10.ax with supporting libraries
- Version 11/HW Encoding: VisioForge_MFT.dll, VisioForge_MF_Mux.ax (with x64 variants)

**WebM Output:**

- Muxer: VisioForge_WebM_Mux.ax / VisioForge_WebM_Mux_x64.ax
- Encoders: VisioForge_WebM_Vorbis_Encoder.ax, VisioForge_WebM_VP8_Encoder.ax
- Audio Enhancement: VisioForge_Audio_Enhancer.ax / VisioForge_Audio_Enhancer_x64.ax

**Ogg/FLAC Support:**

- FLAC: VisioForge_Xiph_FLAC_Encoder.ax / VisioForge_Xiph_FLAC_Encoder_x64.ax
- Ogg Vorbis: VisioForge_Xiph_Ogg_Mux.ax, VisioForge_Xiph_Vorbis_Encoder.ax (with x64 variants)

#### Streaming and Source Components

**RTSP Streaming:**

- VisioForge_RTSP_Sink.ax / VisioForge_RTSP_Sink_x64.ax
- MP4 filters (excluding Muxer)

**VLC Source Integration:**

- VisioForge_VLC_Source.ax / VisioForge_VLC_Source_x64.ax
- Requires copying all files from Redist\VLC folder, COM registration, and proper environment variable configuration

**FFMPEG Integration:**

- VisioForge_FFMPEG_Source.ax / VisioForge_FFMPEG_Source_x64.ax
- Requires all files from Redist\FFMPEG folder and PATH variable updates

**RTSP/RTMP/HTTP Source Support:**

- VisioForge_RTSP_Source.ax, VisioForge_RTSP_Source_Live555.ax
- Requires FFMPEG, VLC, or LAV filters

#### Specialized Components

**Screen Capture:**

- VisioForge_Screen_Capture_DD.ax / VisioForge_Screen_Capture_DD_x64.ax

**Audio Capture:**

- VisioForge_WhatYouHear_Source.ax / VisioForge_WhatYouHear_Source_x64.ax

**Virtual Camera:**

- VisioForge_Virtual_Camera.ax / VisioForge_Virtual_Camera_x64.ax
- VisioForge_Virtual_Audio_Card.ax / VisioForge_Virtual_Audio_Card_x64.ax

**Video Processing:**

- Push Source: VisioForge_Push_Video_Source.ax / VisioForge_Push_Video_Source_x64.ax
- Network Streaming: VisioForge_Network_Streamer_Audio.ax, VisioForge_Network_Streamer_Video.ax
- Video Encryption: Multiple components including Decryptors, Encoders, and supporting libraries
- Picture-In-Picture: VisioForge_Video_Mixer.ax / VisioForge_Video_Mixer_x64.ax

#### Filter Registration

For COM registration of all DirectShow filters in a specific folder, you can deploy the `reg_special.exe` utility from the SDK to the filters directory and run it with administrator privileges to automate the registration process.
