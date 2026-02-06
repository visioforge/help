---
title: Media Player Library Deployment for Delphi & ActiveX
description: Deploy TVFMediaPlayer in Delphi and ActiveX - automated and manual installation methods, codec setup, DirectShow filters, and dependencies.
---

# Deployment Guide for TVFMediaPlayer

Deploying applications built with the TVFMediaPlayer library requires ensuring that all necessary components are correctly installed and configured on the target machine. This guide provides detailed instructions for both automated and manual deployment methods, catering to different scenarios and technical requirements. Whether you prefer the simplicity of silent installers or the granular control of manual setup, this document covers the essential steps to successfully deploy your Delphi or ActiveX media player application.

## Understanding Deployment Requirements

Before deploying your application, it's crucial to understand the dependencies of the TVFMediaPlayer library. The library relies on several core components, including base runtimes, specific codecs (like FFMPEG or VLC for certain sources), and Microsoft Visual C++ Redistributables. The deployment method you choose will determine how these dependencies are handled.

### Core Components

* **Base Library:** Contains the essential engine and DirectShow filters for basic playback functionality.
* **Codec Packages:** Optional but often necessary for supporting a wide range of media formats and network streams (e.g., IP cameras). FFMPEG and VLC are common choices provided.
* **Runtime Dependencies:** Microsoft Visual C++ Redistributable packages are required for the core library components to function correctly.

Choosing the right deployment strategy depends on factors like user privileges on the target machine, the need for unattended installation, and the specific features of your application (e.g., which media sources it needs to support).

## Method 1: Automated Installation (Admin Rights Required)

Using the provided silent installers is the most straightforward method for deploying the TVFMediaPlayer library components. These installers handle the registration of necessary files and ensure all dependencies are correctly placed. This method requires administrative privileges on the target machine as it involves system-level changes like registering COM components and potentially modifying the system PATH.

### Available Installers

VisioForge provides separate installers for the base library and optional codec packages, with versions for both Delphi and ActiveX, and for x86 and x64 architectures.

#### Base Package (Mandatory)

This package installs the core TVFMediaPlayer components and essential DirectShow filters. It's always required, regardless of the media sources your application uses. Choose the installer corresponding to your development environment (Delphi or ActiveX) and target architecture (x86 or x64).

* **Delphi:**
  * [x86 Installer](https://files.visioforge.com/redists_delphi/redist_media_player_base_delphi.exe)
  * [x64 Installer](https://files.visioforge.com/redists_delphi/redist_media_player_base_delphi_x64.exe)
* **ActiveX:**
  * [x86 Installer](https://files.visioforge.com/redists_delphi/redist_media_player_base_ax.exe)
  * [x64 Installer](https://files.visioforge.com/redists_delphi/redist_media_player_base_ax_x64.exe)

#### FFMPEG Package (Optional - For File/IP Camera Sources)

If your application needs to play local files or stream from IP cameras using the FFMPEG engine, you must deploy this package. FFMPEG provides a wide range of codec support.

* **FFMPEG:**
  * [x86 Installer](https://files.visioforge.com/redists_delphi/redist_media_player_ffmpeg.exe)
  * *Note: An x64 FFMPEG installer link was not explicitly provided in the original source; assume x86 covers most needs or consult VisioForge documentation for x64 specifics if required.*

#### VLC Source Package (Optional - For File/IP Camera Sources)

As an alternative or addition to FFMPEG, you can use the VLC engine for file and IP camera sources. This requires deploying the VLC package. Ensure you select the correct architecture.

* **VLC:**
  * [x86 Installer](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe)
  * [x64 Installer](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)

### Installer Usage

These installers are designed for silent execution, making them suitable for inclusion in larger application setup routines or for deployment via scripts. Run the executable(s) with administrator privileges on the target machine.

```bash
# Example: Running the base Delphi x86 installer silently
redist_media_player_base_delphi.exe /S
```

*(Note: The exact silent switch might vary; consult the installer documentation or use standard switches like `/S`, `/silent`, or `/q` if `/S` doesn't work).*

## Method 2: Manual Installation (Admin Rights Recommended)

Manual installation offers more control but requires careful execution of each step. This method is suitable when automated installers cannot be used, or when deploying to environments with specific restrictions. While some steps might be achievable without full admin rights, registering COM components typically requires elevation.

### Prerequisites

Before copying library files, ensure the necessary runtime dependencies are present on the target system.

#### Install VC++ 2010 SP1 Redistributable

The TVFMediaPlayer library relies on the Microsoft Visual C++ 2010 SP1 runtime. Install the appropriate version (x86 or x64) for your application's target architecture.

* **VC++ 2010 SP1:**
  * [x86 Redistributable](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
  * [x64 Redistributable](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

Run these installers before proceeding with the library file deployment.

### Deploying Core Library Files

Follow these steps to manually install the base library components:

1. **Copy Core DLLs:** Locate the `Redist\Filters` folder within your TVFMediaPlayer installation directory. Copy all the DLL files from this folder to a deployment directory on the target machine. A common practice is to place these DLLs in the same folder as your application's executable.
2. **Register DirectShow Filters:** The core functionality relies on several DirectShow filters (`.ax` files). These must be registered with the Windows operating system using Component Object Model (COM) registration.
    * **Identify Filters:** The key filters to register are:
        * `VisioForge_Audio_Effects_4.ax`
        * `VisioForge_Dump.ax`
        * `VisioForge_RGB2YUV.ax`
        * `VisioForge_Video_Effects_Pro.ax`
        * `VisioForge_YUV2RGB.ax`
        * *(Note: Other `.ax` files might be present; register all `.ax` files found in the `Redist\Filters` directory).*
    * **Registration Method:** Use the `regsvr32.exe` command-line tool, which is part of Windows. Open an Command Prompt **as Administrator** and run the command for each `.ax` file.

        ```bash
        # Example: Registering a filter (run from the directory containing the .ax file)
        regsvr32.exe VisioForge_Video_Effects_Pro.ax
        ```

        Alternatively, VisioForge provides a utility `reg_special.exe` in the redistributables. Copy this utility to the folder containing the `.ax` files and run it with administrator privileges to register all filters in that directory automatically. Refer to Microsoft's documentation for troubleshooting `regsvr32.exe` errors: [How to use the Regsvr32 tool](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5).
3. **Update System PATH (Optional but Recommended):** If the filter DLLs and `.ax` files are placed in a directory separate from your application's executable, you must add the path to this directory to the system's `PATH` environment variable. This allows the operating system and your application to locate these essential files. Failure to do this can result in "DLL not found" or filter registration errors.

### Deploying Optional Packages Manually

#### FFMPEG Deployment

1. **Copy Files:** Copy the entire contents of the `Redist\FFMPEG` folder from your TVFMediaPlayer installation to a deployment directory on the target machine (e.g., a subfolder within your application's installation directory).
2. **Update System PATH:** Add the full path to the folder where you copied the FFMPEG files to the Windows system `PATH` environment variable. This is crucial for the library to find and load the FFMPEG components.

#### VLC Deployment (Example: x86)

1. **Copy Files:** Copy the entire contents of the `Redist\VLC` folder (specifically the x86 version if applicable) to a deployment directory.
2. **Register VLC Filter:** Locate the `.ax` file within the copied VLC files (e.g., `axvlc.dll` or similar, though the original text only generically mentions ".ax file") and register it using `regsvr32.exe` with administrator privileges.
3. **Set Environment Variable:** Create a new system environment variable named `VLC_PLUGIN_PATH`. Set its value to the full path of the `plugins` subfolder within the directory where you copied the VLC files (e.g., `C:\YourApp\VLC\plugins`). This tells the VLC engine where to find its necessary plugin modules.

## Verification and Troubleshooting

After deployment, thoroughly test your application on the target machine.

* Check basic playback functionality.
* Test any specific features that rely on optional packages (FFMPEG or VLC), such as playing various file formats or connecting to IP cameras.
* If errors occur, double-check:
  * Admin rights during installation/registration.
  * Correct installation of VC++ Redistributables.
  * Successful registration of all `.ax` files (check `regsvr32.exe` output).
  * Accurate configuration of `PATH` and `VLC_PLUGIN_PATH` environment variables.
  * Correct architecture (x86/x64) match between your application, the library components, and runtime dependencies.

---
Need further assistance? Contact [VisioForge Support](https://support.visioforge.com/). Explore more examples on our [GitHub](https://github.com/visioforge/).