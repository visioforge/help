---
title: TVFVideoCapture Deployment Guide for Delphi
description: Complete step-by-step deployment instructions for the TVFVideoCapture library in Delphi projects. Learn how to properly install necessary components, register DirectShow filters, and configure your development environment for successful application deployment.
sidebar_label: Deployment
---

# Complete TVFVideoCapture Library Deployment Guide

When distributing applications built with the TVFVideoCapture library, you'll need to deploy several framework components to ensure proper functionality on end-user systems. This guide covers all deployment scenarios to help you create reliable installations.

## Deployment Options Overview

You have two primary approaches for deploying the necessary components: automatic installers for simpler deployment or manual installation for more customized setups.

## Automatic Silent Installers (Requires Admin Rights)

These pre-configured installers handle dependencies automatically and can be integrated into your application's installation process:

### Essential Components

- **Base Package** (mandatory for all deployments) 
  - [Delphi Version](http://files.visioforge.com/redists_delphi/redist_video_capture_base_delphi.exe)
  - [ActiveX Version](http://files.visioforge.com/redists_delphi/redist_video_capture_base_ax.exe)

### Optional Feature Components

- **FFMPEG Package** (required for file or IP camera sources)
  - [x86 Architecture](http://files.visioforge.com/redists_delphi/redist_video_capture_ffmpeg.exe)

- **MP4 Output Support**
  - [x86 Architecture](https://files.visioforge.com/redists_delphi/redist_video_capture_mp4.exe)

- **VLC Source Package** (alternative option for file or IP camera sources)
  - [x86 Architecture](http://files.visioforge.com/redists_delphi/redist_video_capture_vlc.exe)

## Manual Installation Process (Requires Admin Rights)

For more control over the deployment process, follow these detailed steps:

### Step 1: Install Required Dependencies

1. Deploy Visual C++ 2010 SP1 redistributables:
   - [x86 Architecture](http://files.visioforge.com/shared/vcredist_2010_x86.exe)
   - [x64 Architecture](http://files.visioforge.com/shared/vcredist_2010_x64.exe)

### Step 2: Deploy Core Components

1. Copy all Media Foundation Platform (MFP) DLLs from the `Redist\Filters` directory to your application folder
2. For ActiveX implementations: copy and register the OCX file using [regsvr32.exe](https://support.microsoft.com/en-us/help/249873/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages)

### Step 3: Register DirectShow Filters

Using [regsvr32.exe](https://support.microsoft.com/en-us/help/249873/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages), register these essential DirectShow filters:

- `VisioForge_Audio_Effects_4.ax`
- `VisioForge_Dump.ax`
- `VisioForge_RGB2YUV.ax`
- `VisioForge_Screen_Capture.ax`
- `VisioForge_Video_Effects_Pro.ax`
- `VisioForge_Video_Mixer.ax`
- `VisioForge_Video_Resize.ax`
- `VisioForge_WavDest.ax`
- `VisioForge_YUV2RGB.ax`
- `VisioForge_FFMPEG_Source.ax`

> **Important:** Add the filter directory to the system PATH environment variable if your application executable resides in a different folder.

## Advanced Component Installation

### FFMPEG Integration

1. Copy all files from `Redist\FFMPEG` folder to your deployment
2. Add the FFMPEG folder to the Windows system PATH variable
3. Register all .ax files from the FFMPEG folder

### VLC Integration

1. Copy all files from the `Redist\VLC` folder
2. Register the included .ax file using regsvr32.exe
3. Create an environment variable named `VLC_PLUGIN_PATH` pointing to the `VLC\plugins` directory

### Audio Output Support (LAME)

1. Copy `lame.ax` from the `Redist\Formats` folder
2. Register the `lame.ax` file using regsvr32.exe

### Container Format Support

- **WebM Support:** Install free codecs from [xiph.org](https://www.xiph.com)
- **Matroska Support:** Deploy `Haali Matroska Splitter`

### MP4 Output Configuration

#### Modern Encoder Setup

1. Copy appropriate library files:
   - `libmfxsw32.dll` (for 32-bit deployments)
   - `libmfxsw64.dll` (for 64-bit deployments)
2. Register required components:
   - `VisioForge_H264_Encoder.ax`
   - `VisioForge_MP4_Muxer.ax`
   - `VisioForge_AAC_Encoder.ax`
   - `VisioForge_Video_Resize.ax`

#### Legacy Encoder Setup (for older systems)

1. Copy appropriate library files:
   - `libmfxxp32.dll` (for 32-bit deployments)
   - `libmfxxp64.dll` (for 64-bit deployments)
2. Register required components:
   - `VisioForge_H264_Encoder_XP.ax`
   - `VisioForge_MP4_Muxer_XP.ax`
   - `VisioForge_AAC_Encoder_XP.ax`
   - `VisioForge_Video_Resize.ax`

## Bulk Registration Utility

To simplify DirectShow filter registration, you can use the `reg_special.exe` utility from the framework setup. Place this executable in your filter directory and run it with administrator privileges to register all filters at once.

---

For additional code samples and implementation examples, visit our [GitHub repository](https://github.com/visioforge/). If you encounter any difficulties with deployment, please contact [technical support](https://support.visioforge.com/) for personalized assistance.
