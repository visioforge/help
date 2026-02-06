---
title: TVFVideoEdit Library Deployment Guide
description: Deploy TVFVideoEdit in Delphi and ActiveX applications with automatic installers or manual setup for required components and dependencies.
---

# TVFVideoEdit Library Deployment Guide

## Introduction

The TVFVideoEdit library provides powerful video editing capabilities for your Delphi and ActiveX applications. This guide explains how to properly deploy all necessary components to ensure your application functions correctly on end-user systems without requiring the full development framework.

## Deployment Options

You have two primary methods for deploying the TVFVideoEdit library components: automatic installers or manual installation. Each approach has specific advantages depending on your distribution requirements.

### Automatic Silent Installers

For streamlined deployment, we offer silent installer packages that handle all necessary component installation without user interaction:

#### Required Base Package

* **Base components** (always required):
  * [Delphi version](https://files.visioforge.com/redists_delphi/redist_video_edit_base_delphi.exe)
  * [ActiveX version](https://files.visioforge.com/redists_delphi/redist_video_edit_base_ax.exe)

#### Optional Feature Packages

* **FFMPEG package** (required for file and IP camera support (only for FFMPEG source engine)):
  * [x86 architecture](https://files.visioforge.com/redists_delphi/redist_video_edit_ffmpeg.exe)

* **MP4 output package** (for MP4 video creation):
  * [x86 architecture](https://files.visioforge.com/redists_delphi/redist_video_edit_mp4.exe)

### Manual Installation Process

For situations where you need precise control over component deployment, follow these detailed steps:

1. **Install Visual C++ Dependencies**
   * Install VC++ 2010 SP1 redistributable:
     * [x86 version](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
     * [x64 version](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

2. **Deploy Core Media Foundation Components**
   * Copy all MFP DLLs from the `Redist\Filters` directory to your application folder

3. **Register DirectShow Filters**
   * Copy and COM-register these essential DirectShow filters using [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5):
     * `VisioForge_Audio_Effects_4.ax`
     * `VisioForge_Dump.ax`
     * `VisioForge_RGB2YUV.ax`
     * `VisioForge_Screen_Capture.ax`
     * `VisioForge_Video_Effects_Pro.ax`
     * `VisioForge_Video_Mixer.ax`
     * `VisioForge_Video_Resize.ax`
     * `VisioForge_WavDest.ax`
     * `VisioForge_YUV2RGB.ax`
     * `VisioForge_FFMPEG_Source.ax`

4. **Configure Path Settings**
   * Add the folder containing these filters to the system environment variable `PATH` if your application executable resides in a different directory

## Additional Components Installation

### FFMPEG Integration

To enable advanced media format support:

* Copy all files from the `Redist\FFMPEG` folder
* Add this folder to the Windows system `PATH` variable
* Register all .ax files from the `Redist\FFMPEG` folder

### VLC Support

For extended format compatibility:

* Copy all files from the `Redist\VLC` folder
* COM-register the .ax file using regsvr32.exe
* Create an environment variable named `VLC_PLUGIN_PATH`
* Set its value to point to the `VLC\plugins` folder

### Audio Output Support

For MP3 encoding capabilities:

* Copy the lame.ax file from the `Redist\Formats` folder
* Register the lame.ax file using regsvr32.exe

### WebM Format Support

For WebM encoding and decoding:

* Install the necessary free codecs available from the [xiph.org website](https://www.xiph.org/dshow/)

### Matroska Container Support

For MKV format compatibility:

* Install [Haali Matroska Splitter](https://haali.net/mkv/) for proper encoding and decoding

### MP4 H264/AAC Output - Modern Encoder

For high-quality MP4 creation with modern codecs:

* Copy `libmfxsw32.dll` / `libmfxsw64.dll` files
* Register these DirectShow filters:
  * `VisioForge_H264_Encoder.ax`
  * `VisioForge_MP4_Muxer.ax`
  * `VisioForge_AAC_Encoder.ax`
  * `VisioForge_Video_Resize.ax`

### MP4 H264/AAC Output - Legacy Encoder

For compatibility with older systems:

* Copy `libmfxxp32.dll` / `libmfxxp64.dll` files
* Register these DirectShow filters:
  * `VisioForge_H264_Encoder_XP.ax`
  * `VisioForge_MP4_Muxer_XP.ax`
  * `VisioForge_AAC_Encoder_XP.ax`
  * `VisioForge_Video_Resize.ax`

## Bulk Registration Utility

To simplify the registration process for multiple DirectShow filters:

* Place the `reg_special.exe` utility from the redistributable package into the folder containing your filters
* Run it with administrator privileges to register all compatible filters in that directory

## Troubleshooting Tips

Common issues during deployment often include:

* Missing dependencies
* Incorrect registration of COM components
* Path configuration problems
* Insufficient user permissions

Ensure all required files are properly deployed and registered before launching your application.

---
Please contact [our support team](https://support.visioforge.com/) if you encounter any issues with this deployment process. Visit our [GitHub repository](https://github.com/visioforge/) for additional code samples and implementation examples.