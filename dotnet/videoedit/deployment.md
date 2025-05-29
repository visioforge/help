---
title: Video Edit SDK .Net Deployment Guide | Windows
description: Complete guide to deploying Video Edit SDK .Net on Windows systems. Learn all deployment methods including NuGet packages, silent installers, and manual setups for both x86 and x64 architectures. Master DirectShow filters and component integration for professional video editing applications.
sidebar_label: Deployment

---

# Comprehensive Deployment Guide for Video Edit SDK .Net

## Introduction to VideoEditCore Deployment

[!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net)

The VisioForge Video Edit SDK for .Net provides a powerful set of tools for video processing, editing, and analysis in Windows environments. This comprehensive guide details the deployment options for ensuring the SDK functions correctly on target systems.

For applications built with the AnyCPU configuration, you must deploy both x86 and x64 redistributables to ensure compatibility across different processor architectures. This guide covers all deployment methods, from simple NuGet packages to detailed manual installations.

## Deployment Options Overview

The SDK offers three primary deployment approaches:

1. **NuGet Packages**: Simplest method requiring no administrative privileges
2. **Automatic Installers**: Silent installation with administrative rights
3. **Manual Installation**: Custom deployment with granular control over components

Each approach has distinct advantages depending on your application requirements, distribution method, and target environment constraints.

## Cross-Platform Deployment with VideoEditCoreX

For developers seeking cross-platform compatibility, VisioForge offers the VideoEditCoreX engine. This modern implementation supports Windows, macOS, and Linux environments.

For detailed instructions on deploying the cross-platform version, please refer to our dedicated [cross-platform deployment guide](../deployment-x/index.md). The remainder of this document focuses on the Windows-specific VideoEditCore engine.

## VideoEditCore Engine (Windows-only)

The Windows-specific VideoEditCore engine provides extensive video editing capabilities optimized for Windows environments. Below are the comprehensive deployment options available.

### NuGet Package Deployment (No Administrative Rights Required)

NuGet packages offer the simplest deployment method, requiring no administrative privileges on the target system. This approach automatically copies the necessary files to your application folder during the build process.

#### Required NuGet Packages

**Base Components (Always Required)**:

- SDK Base Package: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/)
- Video Edit SDK Package: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

**Format-Specific Components**:

- MP4 Output: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- WebM Output: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)
- XIPH Formats (Ogg, Vorbis, FLAC): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

**Media Source Components**:

- FFMPEG (File output/network streaming): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
- VLC Source (File/IP camera): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64/)
- LAV Filters: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

Implementation is straightforward: add the required packages to your application project, and after building, the necessary redistributable files will be automatically included in your application folder.

### Automatic Silent Installers (Administrative Rights Required)

For scenarios where administrative rights are available, silent installers provide a streamlined deployment solution. These installers can be integrated into your application's setup process for seamless SDK deployment.

**Base Components**:

- Base Package (Always Required): [x86](http://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

**Media Source Components**:

- FFMPEG Package: [x86](http://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- VLC Source Package: [x86](http://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)
- LAV Filters: [x86](http://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

**Format-Specific Components**:

- XIPH Formats (Ogg, Vorbis, FLAC): [x86](http://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](http://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

**Installation and Uninstallation**:

- To install: Run the appropriate executable with administrative privileges
- To uninstall: Run the executable with administrative privileges and the parameters "/x //"
- .NET assemblies can be installed in the Global Assembly Cache (GAC) or used directly from a local folder

### Manual Installation (Advanced)

Manual installation provides the highest level of control over the deployment process. This approach is recommended for advanced scenarios where specific components must be customized or for deployment environments with unique constraints.

#### Step-by-Step Manual Installation Process

1. **Runtime Dependencies**:
   - For applications with administrative privileges: Install VC++ 2022 (v143) runtime (x86/x64) and OpenMP runtime DLLs using executable redistributables or MSM modules
   - For applications without administrative privileges: Copy the VC++ 2022 (v143) runtime (x86/x64) and OpenMP runtime DLLs directly to the application folder

2. **Core Components**:
   - Copy the VisioForge_MFP/VisioForge_MFPX (or x64 versions) DLLs from Redist\Filters to your application folder
   - Copy .NET assemblies to the application folder or install them to the Global Assembly Cache (GAC)

3. **DirectShow Filters**:
   - Copy and COM-register SDK DirectShow filters using [regsvr32.exe](https://support.microsoft.com/en-us/help/249873/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages) or an equivalent method
   - If your application executable is in a different folder, add the folder containing the filters to the system PATH environment variable

## Essential DirectShow Filters Reference

### Core Functionality Filters

**Basic Video Processing**:

- VisioForge_Video_Effects_Pro.ax - Core video effects processing
- VisioForge_Audio_Mixer.ax - Audio mixing and processing
- VisioForge_MP3_Splitter.ax - MP3 format handling
- VisioForge_H264_Decoder.ax - H.264 video decoding

**Audio Processing**:

- VisioForge_Audio_Effects_4.ax - Legacy audio effects processing

### Streaming Filters

**RTSP Streaming**:

- VisioForge_RTSP_Sink.ax - RTSP streaming output
- All MP4 filters (legacy/modern) except Muxer

**SSF Streaming**:

- VisioForge_SSF_Muxer.ax - SSF format multiplexer
- All MP4 filters (legacy/modern) except Muxer

**RTSP/RTMP/HTTP Sources**:

- VisioForge_RTSP_Source.ax - RTSP stream input
- VisioForge_RTSP_Source_Live555.ax - RTSP with Live555 library
- VisioForge_IP_HTTP_Source.ax - HTTP source input
- FFMPEG, VLC, or LAV filters as needed

### Media Source Filters

**VLC Source**:

- VisioForge_VLC_Source.ax - VLC-based media input
- Complete deployment requires:
  - Copying all files from Redist\VLC folder
  - COM registration of .ax files
  - Adding environment variable VLC_PLUGIN_PATH pointing to VLC\plugins folder

**FFMPEG Source**:

- VisioForge_FFMPEG_Source.ax - FFMPEG-based media input
- Copy all files from Redist\FFMPEG folder and add to Windows PATH

**Memory Source**:

- VisioForge_AsyncEx.ax - Memory-based source input

**LAV Source**:

- Copy all files from Redist\LAV\x86(x64)
- Register all .ax files

### Format-Specific Filters

**WebM Decoding**:

- VisioForge_WebM_Ogg_Source.ax - WebM/Ogg container support
- VisioForge_WebM_Source.ax - WebM format source
- VisioForge_WebM_Split.ax - WebM demuxing
- VisioForge_WebM_Vorbis_Decoder.ax - Vorbis audio decoder
- VisioForge_WebM_VP8_Decoder.ax - VP8 video decoder
- VisioForge_WebM_VP9_Decoder.ax - VP9 video decoder

**FLAC Source**:

- VisioForge_Xiph_FLAC_Source.ax - FLAC audio format support

**Ogg Vorbis Source**:

- VisioForge_Xiph_Ogg_Demux2.ax - Ogg container demuxer
- VisioForge_Xiph_Vorbis_Decoder.ax - Vorbis audio decoder

### Advanced Functionality Filters

**Video Encryption**:

- VisioForge_Encryptor_v8.ax - Version 8 encryption
- VisioForge_Encryptor_v9.ax - Version 9 encryption

**GPU Acceleration**:

- VisioForge_DXP.dll / VisioForge_DXP64.dll - DirectX 11 GPU effects

### Simplified Filter Registration

For convenient registration of multiple DirectShow filters, place the `reg_special.exe` utility from the SDK redistributable into the folder containing the filters and run it with administrator privileges.

## Additional Resources

For code samples and implementation examples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).

For technical support, documentation updates, and community discussions, visit the [VisioForge Developer Portal](https://support.visioforge.com/).
