---
title: Remove DirectShow Filters in Windows
description: Properly uninstall DirectShow filters with manual techniques, troubleshooting steps, and best practices for .NET multimedia applications.
---

# Remove DirectShow Filters in Windows

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

DirectShow filters are essential components for multimedia applications in Windows environments. They enable software to process audio and video data efficiently. However, there may be situations where you need to uninstall these filters, such as when upgrading your application, resolving conflicts, or completely removing a software package. This guide provides detailed instructions on how to properly uninstall DirectShow filters from your system.

## Understanding DirectShow Filters

DirectShow is a multimedia framework and API designed by Microsoft for software developers to perform various operations with media files. It's built on the Component Object Model (COM) architecture and uses a modular approach where each processing step is handled by a separate component called a filter.

Filters are categorized into three main types:

- **Source filters**: Read data from files, capture devices, or network streams
- **Transform filters**: Process or modify the data (compression, decompression, effects)
- **Rendering filters**: Display video or play audio

When SDK components are installed, they register DirectShow filters in the Windows Registry, making them available to any application that uses the DirectShow framework.

## Why Uninstall DirectShow Filters?

There are several reasons why you might need to uninstall DirectShow filters:

1. **Version conflicts**: Newer versions of the SDK might require removing older filters
2. **System cleanup**: Removing unused components to maintain system efficiency
3. **Troubleshooting**: Resolving issues with multimedia applications
4. **Complete software removal**: Ensuring no components remain after uninstalling the main application
5. **Re-registration**: Sometimes uninstalling and reinstalling filters can resolve registration issues

## Methods for Uninstalling DirectShow Filters

### Method 1: Using the SDK Installer (Recommended)

The most straightforward way to uninstall DirectShow filters is through the SDK (or redist) installer itself. SDK packages include uninstallation routines that properly remove all components, including DirectShow filters.

### Method 2: Manual Unregistration with regsvr32

If automatic uninstallation isn't possible or you need to unregister specific filters, you can use the `regsvr32` command-line tool:

1. Open Command Prompt as Administrator (right-click on Command Prompt and select "Run as administrator")
2. Use the following command syntax to unregister a filter:

   ```cmd
   regsvr32 /u "C:\path\to\filter.dll"
   ```

3. Replace `C:\path\to\filter.dll` with the actual path to the DirectShow filter file
4. Press Enter to execute the command

For example, to unregister a filter located at `C:\Program Files\Common Files\FilterFolder\example_filter.dll`, you would use:

```cmd
regsvr32 /u "C:\Program Files\Common Files\FilterFolder\example_filter.dll"
```

You should see a confirmation dialog indicating successful unregistration.

## Finding DirectShow Filter Locations

Before you can manually unregister filters, you need to know their locations. Here are several methods to find installed DirectShow filters:

### Using GraphStudio

[GraphStudio](https://github.com/cplussharp/graph-studio-next) is a powerful open-source tool for working with DirectShow filters. To find filter locations:

1. Download and install GraphStudio
2. Launch the application with administrator privileges
3. Go to "Graph > Insert Filters"
4. Browse through the list of installed filters
5. Right-click on a filter and select "Properties"
6. Note the "File:" path shown in the properties dialog

This method provides the exact file path needed for manual unregistration.

### Using System Registry

You can also find DirectShow filters through the Windows Registry:

1. Press `Win + R` to open the Run dialog
2. Type `regedit` and press Enter to open Registry Editor
3. Navigate to `HKEY_CLASSES_ROOT\CLSID`
4. Use the Search function (Ctrl+F) to find filter names
5. Look for the "InprocServer32" key under the filter's CLSID, which contains the file path

## Platform Considerations (x86 vs x64)

DirectShow filters are platform-specific, meaning 32-bit (x86) and 64-bit (x64) versions are separate components. If you've installed both versions, you need to unregister each one separately.

For x64 systems:

- 64-bit filters are typically installed in `C:\Windows\System32`
- 32-bit filters are typically installed in `C:\Windows\SysWOW64`

Use the appropriate version of `regsvr32` for each platform:

- For 64-bit filters: `C:\Windows\System32\regsvr32.exe`
- For 32-bit filters: `C:\Windows\SysWOW64\regsvr32.exe`

## Troubleshooting Filter Uninstallation

If you encounter issues during filter uninstallation, try these troubleshooting steps:

### Unable to Unregister Filter

If you receive an error like "DllUnregisterServer failed with error code 0x80004005":

1. Ensure you're running Command Prompt as Administrator
2. Verify that the path to the filter is correct
3. Check if the filter file exists and isn't in use by any application
4. Close any applications that might be using DirectShow filters
5. In some cases, a system restart may be necessary before unregistration

### Filter Still Present After Unregistration

If a filter appears to be still registered after attempting to unregister it:

1. Use GraphStudio to check if the filter is still listed
2. Look for multiple instances of the filter in different locations
3. Check both 32-bit and 64-bit registry locations
4. Try using the Microsoft-provided tool "OleView" to inspect COM registrations

## Verifying Successful Uninstallation

After uninstalling DirectShow filters, verify the removal was successful:

1. Use GraphStudio to check if the filters no longer appear in the available filters list
2. Check the registry for any remaining entries related to the filters
3. Test any applications that previously used the filters to ensure they handle the absence gracefully

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples for working with DirectShow and multimedia applications in .NET.