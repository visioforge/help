---
title: Media Player SDK .Net Deployment Guide
description: Step-by-step deployment instructions for Media Player SDK .Net applications. Learn how to deploy using NuGet packages, silent installers, and manual configuration. Includes runtime dependencies, DirectShow filters, and environment setup for Windows and cross-platform development.
sidebar_label: Deployment Guide

---

# Media Player SDK .Net Deployment Guide

[!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

This comprehensive guide covers all deployment scenarios for the Media Player SDK .Net, ensuring your applications work correctly across different environments. Whether you're developing cross-platform applications or Windows-specific solutions, this guide provides the necessary steps for successful deployment.

## Engine Types Overview

The Media Player SDK .Net offers two primary engine types, each designed for specific deployment scenarios:

### MediaPlayerCoreX Engine (Cross-Platform)

MediaPlayerCoreX is our cross-platform solution that works across multiple operating systems. For detailed deployment instructions specific to this engine, refer to the main [Cross-Platform Deployment Guide](../deployment-x/index.md).

### MediaPlayerCore Engine (Windows-Only)

The MediaPlayerCore engine is optimized specifically for Windows environments. When deploying applications that use this engine on computers without the SDK pre-installed, you must include the necessary SDK components with your application.

> **Important**: For AnyCPU applications, you should deploy both x86 and x64 redistributables to ensure compatibility across different system architectures.

## Deployment Options

There are three primary methods for deploying the Media Player SDK .Net components:

1. Using NuGet packages (recommended for most scenarios)
2. Using automatic silent installers (requires administrative privileges)
3. Manual installation (for complete control over the deployment process)

## NuGet Package Deployment

NuGet packages provide the simplest deployment method, automatically handling the inclusion of necessary files in your application folder during the build process.

### Required NuGet Packages

#### Core Packages (Always Required)

* **SDK Base Package**:
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/)
  * [x64 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/)
* **Media Player SDK Package**:
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x86/)
  * [x64 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x64/)

#### Feature-Specific Packages (Add as Needed)

##### Media Format Support

* **FFMPEG Package** (for file playback using FFMPEG source mode):
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)
  * [x64 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
* **MP4 Output Package**:
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  * [x64 Version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
* **WebM Output Package**:
  * [x86 Version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)

##### Source Support

* **VLC Source Package** (for file/IP camera sources):
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86/)
  * [x64 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64/)

##### Audio Format Support

* **XIPH Formats Package** (Ogg, Vorbis, FLAC output/source):
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/)
  * [x64 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

##### Filter Support

* **LAV Filters Package**:
  * [x86 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/)
  * [x64 Version](http://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Automatic Silent Installers

For scenarios where you prefer installer-based deployment, the SDK offers automatic silent installers that require administrative privileges.

### Available Installers

#### Core Components

* **Base Package** (always required):
  * [x86 Installer](http://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe)
  * [x64 Installer](http://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

#### Media Format Support

* **FFMPEG Package** (for file/IP camera sources):
  * [x86 Installer](http://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe)
  * [x64 Installer](http://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)

#### Source Support

* **VLC Source Package** (for file/IP camera sources):
  * [x86 Installer](http://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe)
  * [x64 Installer](http://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)

#### Audio Format Support

* **XIPH Formats Package** (Ogg, Vorbis, FLAC output/source):
  * [x86 Installer](http://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe)
  * [x64 Installer](http://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

#### Filter Support

* **LAV Filters Package**:
  * [x86 Installer](http://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe)
  * [x64 Installer](http://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

> **Note**: To uninstall any installed package, run the executable with administrative privileges using the parameters: `/x //`

## Manual Installation

For advanced deployment scenarios requiring precise control over component installation, follow these steps:

### Step 1: Runtime Dependencies

* **With Administrative Privileges**: Install the VC++ 2022 (v143) runtime (x86/x64) and OpenMP runtime DLLs using redistributable executables or MSM modules.
* **Without Administrative Privileges**: Copy the VC++ 2022 (v143) runtime (x86/x64) and OpenMP runtime DLLs directly to your application folder.

### Step 2: Core Components

* Copy the VisioForge_MFP/VisioForge_MFPX (or x64 versions) DLLs from the Redist\Filters directory to your application folder.

### Step 3: .NET Assemblies

* Either copy the .NET assemblies to your application folder or install them to the Global Assembly Cache (GAC).

### Step 4: DirectShow Filters

* Copy and COM-register SDK DirectShow filters using [regsvr32.exe](https://support.microsoft.com/en-us/help/249873/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages) or another suitable method.

### Step 5: Environment Configuration

* Add the folder containing the filters to the system PATH environment variable if your application executable is located in a different directory.

## DirectShow Filter Configuration

The SDK uses various DirectShow filters for specific functionality. Below is a comprehensive list organized by feature category:

### Basic Feature Filters

* VisioForge_Video_Effects_Pro.ax
* VisioForge_MP3_Splitter.ax
* VisioForge_H264_Decoder.ax
* VisioForge_Audio_Mixer.ax

### Audio Effect Filters

* VisioForge_Audio_Effects_4.ax (legacy audio effects)

### Streaming Support Filters

#### RTSP Streaming

* VisioForge_RTSP_Sink.ax
* MP4 filters (legacy/modern, excluding muxer)

#### SSF Streaming

* VisioForge_SSF_Muxer.ax
* MP4 filters (legacy/modern, excluding muxer)

### Source Filters

#### VLC Source

* VisioForge_VLC_Source.ax
* Complete Redist\VLC folder with COM registration
* VLC_PLUGIN_PATH environment variable pointing to VLC\plugins folder

#### FFMPEG Source

* VisioForge_FFMPEG_Source.ax
* Complete Redist\FFMPEG folder, added to the Windows PATH variable

#### Memory Source

* VisioForge_AsyncEx.ax

#### WebM Decoding

* VisioForge_WebM_Ogg_Source.ax
* VisioForge_WebM_Source.ax
* VisioForge_WebM_Split.ax
* VisioForge_WebM_Vorbis_Decoder.ax
* VisioForge_WebM_VP8_Decoder.ax
* VisioForge_WebM_VP9_Decoder.ax

#### Network Streaming Sources

* VisioForge_RTSP_Source.ax
* VisioForge_RTSP_Source_Live555.ax
* FFMPEG, VLC or LAV filters

#### Audio Format Sources

* VisioForge_Xiph_FLAC_Source.ax (FLAC source)
* VisioForge_Xiph_Ogg_Demux2.ax (Ogg Vorbis source)
* VisioForge_Xiph_Vorbis_Decoder.ax (Ogg Vorbis source)

### Special Feature Filters

#### Video Encryption

* VisioForge_Encryptor_v8.ax
* VisioForge_Encryptor_v9.ax

#### GPU Acceleration

* VisioForge_DXP.dll / VisioForge_DXP64.dll (DirectX 11 GPU video effects)

#### LAV Source

* Complete contents of redist\LAV\x86(x64), with all .ax files registered

### Filter Registration Tip

To simplify the COM registration process for all DirectShow filters in a directory, place the "reg_special.exe" file from the SDK redist into the filters folder and run it with administrative privileges.

---

For more code samples and examples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
