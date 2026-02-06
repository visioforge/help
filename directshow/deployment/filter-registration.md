---
title: DirectShow Filter Registration Guide
description: Register VisioForge DirectShow filters using manual regsvr32, programmatic methods, and installer automation with troubleshooting tips.
---

# DirectShow Filter Registration Guide

## Overview

DirectShow filters must be registered with Windows before they can be used in applications. This guide covers all registration methods for VisioForge DirectShow filters.

---
## Registration Methods
### Method 1: Automatic Registration (Installer)
The recommended method for end-users is to use the official installer.
**Available Installers**:
- `visioforge_ffmpeg_source_filter_setup.exe` - FFMPEG Source Filter
- `visioforge_vlc_source_filter_setup.exe` - VLC Source Filter
- `visioforge_processing_filters_pack_setup.exe` - Processing Filters Pack
- `visioforge_encoding_filters_pack_setup.exe` - Encoding Filters Pack
- `visioforge_virtual_camera_sdk_setup.exe` - Virtual Camera SDK
**Installation Steps**:
1. Run installer as Administrator
2. Follow installation wizard
3. Filters are automatically registered
4. No additional steps required
---

### Method 2: Manual Registration (regsvr32)

For development and testing, you can manually register filters using Windows `regsvr32` utility.

#### Registration Command

```batch
# Open Command Prompt as Administrator
# Right-click Start → Command Prompt (Admin)

# Register x86 (32-bit) filter
regsvr32 "C:\Path\To\Filter.ax"

# Register x64 (64-bit) filter
regsvr32 "C:\Path\To\Filter_x64.ax"

# Unregister filter
regsvr32 /u "C:\Path\To\Filter.ax"
```

#### SDK-Specific Examples

**FFMPEG Source Filter**:
```batch
# x86
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source.ax"

# x64
regsvr32 "C:\Program Files\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
```

**VLC Source Filter**:
```batch
# x86 only
regsvr32 "C:\Program Files (x86)\VisioForge\VLC Source\VisioForge_VLC_Source.ax"
```

**Processing Filters Pack** (multiple filters):
```batch
# Video Effects
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Effects_Pro.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"

# Video Mixer
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Mixer.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Mixer_x64.ax"

# Audio Enhancer
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Audio_Enhancer.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Audio_Enhancer_x64.ax"
```

**Encoding Filters Pack** (multiple filters):
```batch
# NVENC Encoder
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_NVENC.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_NVENC_x64.ax"

# H.264 Encoder
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_H264_Encoder.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_H264_Encoder_x64.ax"

# AAC Encoder
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_AAC_Encoder.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_AAC_Encoder_x64.ax"

# MP4 Muxer
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_MP4_Muxer.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_MP4_Muxer_x64.ax"
```

**Virtual Camera SDK**:
```batch
# Virtual Camera Driver
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Virtual_Camera.ax"
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Virtual_Camera_x64.ax"

# Push Source Filter
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Push_Video_Source.ax"
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Push_Video_Source_x64.ax"
```

---
### Method 3: Programmatic Registration (C++)
Register filters programmatically from your application code.
#### Using LoadLibrary and DllRegisterServer
```cpp
#include <windows.h>
#include <iostream>
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
HRESULT RegisterFilter(const wchar_t* filterPath)
{
    HMODULE hModule = LoadLibraryW(filterPath);
    if (!hModule)
    {
        DWORD error = GetLastError();
        std::wcerr << L"Failed to load filter: " << filterPath << std::endl;
        std::wcerr << L"Error code: " << error << std::endl;
        return HRESULT_FROM_WIN32(error);
    }
    LPFNDLLREGISTERSERVER pfnDllRegisterServer =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");
    if (!pfnDllRegisterServer)
    {
        FreeLibrary(hModule);
        return E_FAIL;
    }
    HRESULT hr = pfnDllRegisterServer();
    FreeLibrary(hModule);
    if (SUCCEEDED(hr))
    {
        std::wcout << L"Filter registered successfully: " << filterPath << std::endl;
    }
    else
    {
        std::wcerr << L"Registration failed with HRESULT: " << std::hex << hr << std::endl;
    }
    return hr;
}
HRESULT UnregisterFilter(const wchar_t* filterPath)
{
    HMODULE hModule = LoadLibraryW(filterPath);
    if (!hModule)
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    typedef HRESULT (STDAPICALLTYPE *LPFNDLLUNREGISTERSERVER)();
    LPFNDLLUNREGISTERSERVER pfnDllUnregisterServer =
        (LPFNDLLUNREGISTERSERVER)GetProcAddress(hModule, "DllUnregisterServer");
    if (!pfnDllUnregisterServer)
    {
        FreeLibrary(hModule);
        return E_FAIL;
    }
    HRESULT hr = pfnDllUnregisterServer();
    FreeLibrary(hModule);
    return hr;
}
// Usage
int main()
{
    const wchar_t* filterPath = L"C:\\Program Files\\VisioForge\\FFMPEG Source\\VisioForge_FFMPEG_Source_x64.ax";
    HRESULT hr = RegisterFilter(filterPath);
    if (SUCCEEDED(hr))
    {
        std::cout << "Filter registered successfully!" << std::endl;
    }
    else
    {
        std::cout << "Failed to register filter" << std::endl;
    }
    return 0;
}
```
#### Using reg_special.exe Utility
VisioForge SDKs include a `reg_special.exe` utility for simplified registration:
```cpp
#include <windows.h>
#include <shellapi.h>
HRESULT RegisterWithUtility(const wchar_t* filterPath)
{
    // Build command line
    wchar_t cmdLine[MAX_PATH * 2];
    swprintf_s(cmdLine, L"reg_special.exe /regserver \"%s\"", filterPath);
    // Execute registration utility
    SHELLEXECUTEINFO sei = { sizeof(sei) };
    sei.lpVerb = L"runas";  // Run as administrator
    sei.lpFile = L"reg_special.exe";
    sei.lpParameters = cmdLine;
    sei.nShow = SW_HIDE;
    sei.fMask = SEE_MASK_NOCLOSEPROCESS;
    if (!ShellExecuteEx(&sei))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    // Wait for completion
    WaitForSingleObject(sei.hProcess, INFINITE);
    DWORD exitCode;
    GetExitCodeProcess(sei.hProcess, &exitCode);
    CloseHandle(sei.hProcess);
    return (exitCode == 0) ? S_OK : E_FAIL;
}
```
---

### Method 4: Programmatic Registration (.NET/C#)

Register filters from .NET applications using P/Invoke.

```csharp
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

public class FilterRegistration
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int DllRegisterServerDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int DllUnregisterServerDelegate();

    public static void RegisterFilter(string filterPath)
    {
        IntPtr hModule = LoadLibrary(filterPath);
        if (hModule == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                $"Failed to load filter: {filterPath}");
        }

        try
        {
            IntPtr procAddress = GetProcAddress(hModule, "DllRegisterServer");
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception("DllRegisterServer function not found");
            }

            DllRegisterServerDelegate registerServer =
                Marshal.GetDelegateForFunctionPointer<DllRegisterServerDelegate>(procAddress);

            int result = registerServer();

            if (result != 0)
            {
                throw new COMException($"Registration failed with HRESULT: 0x{result:X8}");
            }

            Console.WriteLine($"Filter registered successfully: {filterPath}");
        }
        finally
        {
            FreeLibrary(hModule);
        }
    }

    public static void UnregisterFilter(string filterPath)
    {
        IntPtr hModule = LoadLibrary(filterPath);
        if (hModule == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            IntPtr procAddress = GetProcAddress(hModule, "DllUnregisterServer");
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception("DllUnregisterServer function not found");
            }

            DllUnregisterServerDelegate unregisterServer =
                Marshal.GetDelegateForFunctionPointer<DllUnregisterServerDelegate>(procAddress);

            int result = unregisterServer();

            if (result != 0)
            {
                throw new COMException($"Unregistration failed with HRESULT: 0x{result:X8}");
            }

            Console.WriteLine($"Filter unregistered successfully: {filterPath}");
        }
        finally
        {
            FreeLibrary(hModule);
        }
    }

    // Alternative: Use Process.Start with regsvr32
    public static void RegisterFilterWithRegsvr32(string filterPath)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "regsvr32.exe",
            Arguments = $"/s \"{filterPath}\"",  // /s = silent
            Verb = "runas",  // Run as administrator
            UseShellExecute = true,
            CreateNoWindow = true
        };

        using (var process = System.Diagnostics.Process.Start(startInfo))
        {
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"regsvr32 failed with exit code: {process.ExitCode}");
            }
        }
    }
}

// Usage example
class Program
{
    static void Main(string[] args)
    {
        string filterPath = @"C:\Program Files\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax";

        try
        {
            FilterRegistration.RegisterFilter(filterPath);
            Console.WriteLine("Filter registered successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

---
## Registration Verification
### Method 1: Using GraphEdit/GraphStudioNext
1. Launch GraphEdit (Windows SDK) or GraphStudioNext
2. Click "Graph" → "Insert Filters"
3. Search for filter name (e.g., "FFMPEG Source", "VLC Source")
4. If filter appears in list, registration succeeded
### Method 2: Using Registry Editor
```batch
# Open Registry Editor
regedit
# Navigate to:
HKEY_CLASSES_ROOT\CLSID\{GUID}
# Example for FFMPEG Source:
# HKEY_CLASSES_ROOT\CLSID\{1974D893-83E4-4F89-9908-795C524CC17E}
```
### Method 3: Programmatic Verification (C++)
```cpp
#include <dshow.h>
bool IsFilterRegistered(const CLSID& filterClsid)
{
    IBaseFilter* pFilter = nullptr;
    HRESULT hr = CoCreateInstance(filterClsid, NULL, CLSCTX_INPROC_SERVER,
        IID_IBaseFilter, (void**)&pFilter);
    if (SUCCEEDED(hr) && pFilter)
    {
        pFilter->Release();
        return true;
    }
    return false;
}
// Usage
int main()
{
    CoInitialize(NULL);
    // FFMPEG Source Filter CLSID
    CLSID ffmpegSourceClsid =
        { 0x1974D893, 0x83E4, 0x4F89, { 0x99, 0x08, 0x79, 0x5C, 0x52, 0x4C, 0xC1, 0x7E } };
    if (IsFilterRegistered(ffmpegSourceClsid))
    {
        std::cout << "FFMPEG Source filter is registered" << std::endl;
    }
    else
    {
        std::cout << "FFMPEG Source filter is NOT registered" << std::endl;
    }
    CoUninitialize();
    return 0;
}
```
### Method 4: Programmatic Verification (.NET/C#)
```csharp
using System;
using System.Runtime.InteropServices;
public static bool IsFilterRegistered(Guid clsid)
{
    try
    {
        Type comType = Type.GetTypeFromCLSID(clsid, throwOnError: false);
        if (comType == null)
            return false;
        object instance = Activator.CreateInstance(comType);
        if (instance != null)
        {
            Marshal.ReleaseComObject(instance);
            return true;
        }
    }
    catch
    {
        return false;
    }
    return false;
}
// Usage
Guid ffmpegSourceClsid = new Guid("1974D893-83E4-4F89-9908-795C524CC17E");
if (IsFilterRegistered(ffmpegSourceClsid))
{
    Console.WriteLine("FFMPEG Source filter is registered");
}
```
---

## Troubleshooting

### Issue: "DllRegisterServer failed" or "Error 0x80004005"

**Causes**:
- Not running as Administrator
- Missing dependencies (DLLs)
- Wrong architecture (x86 vs x64)

**Solutions**:

1. **Run as Administrator**:
   ```batch
   # Right-click Command Prompt → Run as Administrator
   regsvr32 "C:\Path\To\Filter.ax"
   ```

2. **Check Dependencies**:
   Use Dependency Walker or Dependencies.exe to check for missing DLLs:
   ```batch
   # Download Dependencies from: https://github.com/lucasg/Dependencies
   Dependencies.exe "C:\Path\To\Filter.ax"
   ```

3. **Verify Architecture**:
   ```batch
   # For 32-bit application, register 32-bit filter
   regsvr32 "C:\Path\To\Filter.ax"

   # For 64-bit application, register 64-bit filter
   regsvr32 "C:\Path\To\Filter_x64.ax"
   ```

### Issue: "The module was loaded but the entry-point was not found"

**Cause**: File is not a valid DirectShow filter or is corrupted.

**Solutions**:
- Verify file integrity
- Re-download or reinstall SDK
- Check if file is a DirectShow filter (.ax extension)

### Issue: Filter registered but not found in applications

**Causes**:
- 32-bit/64-bit mismatch
- Filter registered in wrong HKEY (per-user vs system-wide)

**Solutions**:

1. **Match Application Architecture**:
   - 32-bit app needs 32-bit filter
   - 64-bit app needs 64-bit filter

2. **System-Wide Registration**:
   ```batch
   # Run Command Prompt as Administrator
   # This registers system-wide (HKEY_LOCAL_MACHINE)
   regsvr32 "C:\Path\To\Filter.ax"
   ```

3. **Check Both Registries**:
   - `HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID`
   - `HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID`

### Issue: Access Denied

**Cause**: Insufficient permissions.

**Solution**:
```batch
# Always run as Administrator for filter registration
# Right-click Command Prompt → Run as Administrator
```

### Issue: Registration succeeds but filter doesn't work

**Causes**:
- Missing license key
- Missing runtime dependencies
- Incorrect installation path

**Solutions**:

1. **Verify License**:
   - Check if trial license is expired
   - Ensure license key is properly activated

2. **Check Runtime Dependencies**:
   - FFMPEG Source: Requires FFmpeg DLLs (avcodec, avformat, etc.)
   - VLC Source: Requires VLC libraries (libvlc.dll, libvlccore.dll, plugins/)
   - NVENC: Requires NVIDIA GPU and drivers
   - Processing/Encoding: May require Visual C++ Redistributables

3. **Verify File Locations**:
   All dependent DLLs must be in same directory as .ax file or in system PATH.

---
## Registration-Free COM (Advanced)
For xcopy deployment without registration, use registration-free COM with manifest files.
### Creating Manifest File
**filter.manifest** (place next to .ax file):
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
  <assemblyIdentity
    type="win32"
    name="VisioForge.FFMPEGSource"
    version="1.0.0.0"/>
  <file name="VisioForge_FFMPEG_Source_x64.ax">
    <comClass
      clsid="{1974D893-83E4-4F89-9908-795C524CC17E}"
      threadingModel="Both"/>
  </file>
</assembly>
```
**application.exe.manifest** (place next to your .exe):
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
  <assemblyIdentity
    type="win32"
    name="YourApplication"
    version="1.0.0.0"/>
  <dependency>
    <dependentAssembly>
      <assemblyIdentity
        type="win32"
        name="VisioForge.FFMPEGSource"
        version="1.0.0.0"/>
    </dependentAssembly>
  </dependency>
</assembly>
```
**Limitations**:
- More complex to set up
- Requires manifest files
- May not work with all DirectShow filters
- System-registered filters take precedence
---

## Batch Registration Scripts

### Register All Filters (Batch Script)

```batch
@echo off
echo Registering VisioForge DirectShow Filters...
echo.

REM Check for Administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

REM Set installation path
set INSTALL_PATH=C:\Program Files\VisioForge

REM Register FFMPEG Source
echo Registering FFMPEG Source...
regsvr32 /s "%INSTALL_PATH%\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% equ 0 (
    echo   [OK] FFMPEG Source registered
) else (
    echo   [FAILED] FFMPEG Source registration failed
)

REM Register VLC Source
echo Registering VLC Source...
regsvr32 /s "%INSTALL_PATH%\VLC Source\VisioForge_VLC_Source.ax"
if %errorLevel% equ 0 (
    echo   [OK] VLC Source registered
) else (
    echo   [FAILED] VLC Source registration failed
)

REM Register Processing Filters
echo Registering Processing Filters...
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Mixer_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Audio_Enhancer_x64.ax"
echo   [OK] Processing Filters registered

REM Register Encoding Filters
echo Registering Encoding Filters...
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_NVENC_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_H264_Encoder_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_AAC_Encoder_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_MP4_Muxer_x64.ax"
echo   [OK] Encoding Filters registered

echo.
echo Registration complete!
pause
```

### Unregister All Filters

```batch
@echo off
echo Unregistering VisioForge DirectShow Filters...
echo.

REM Check for Administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

set INSTALL_PATH=C:\Program Files\VisioForge

REM Unregister all filters
regsvr32 /s /u "%INSTALL_PATH%\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\VLC Source\VisioForge_VLC_Source.ax"
regsvr32 /s /u "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Mixer_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\Encoding Filters\VisioForge_NVENC_x64.ax"

echo Unregistration complete!
pause
```

---
## See Also
- [Redistributable Files](redistributable-files.md) - Complete file list for each SDK
- [Installer Integration](installer-integration.md) - Creating custom installers
- [Deployment Overview](index.md) - Main deployment guide