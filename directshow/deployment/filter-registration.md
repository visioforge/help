---
title: Register DirectShow Filters — regsvr32, C++, and C# Methods
description: Register VisioForge DirectShow filters using manual regsvr32, programmatic methods, and installer automation with troubleshooting tips.
tags:
  - DirectShow
  - C++
  - Windows
  - Encoding
  - Mixing
  - MP4
  - H.264
  - AAC
  - C#
primary_api_classes:
  - FFMPEGSource
  - IBaseFilter

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
- `visioforge_vlc_source_sdk_setup_x86.exe`, `visioforge_vlc_source_sdk_setup_x64.exe` - VLC Source Filter
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
REM Open Command Prompt as Administrator
REM Right-click Start → Command Prompt (Admin)

REM Register x86 (32-bit) filter
regsvr32 "C:\Path\To\Filter.ax"

REM Register x64 (64-bit) filter
regsvr32 "C:\Path\To\Filter_x64.ax"

REM Unregister filter
regsvr32 /u "C:\Path\To\Filter.ax"
```

#### SDK-Specific Examples

**FFMPEG Source Filter**:
```batch
REM x86
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist\x86\VisioForge_FFMPEG_Source.ax"

REM x64
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist\x64\VisioForge_FFMPEG_Source_x64.ax"
```

**VLC Source Filter**:
```batch
REM x86
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge VLC Source DirectShow SDK\Redist\x86\VisioForge_VLC_Source.ax"

REM x64
regsvr32 "C:\Program Files\VisioForge\VisioForge VLC Source DirectShow SDK x64\Redist\x64\VisioForge_VLC_Source_x64.ax"
```

**Processing Filters Pack** (multiple filters):
```batch
REM Video Effects
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Effects_Pro.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Effects_Pro_x64.ax"

REM Video Mixer
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Mixer.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Mixer_x64.ax"

REM Audio Enhancer
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Audio_Enhancer.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Audio_Enhancer_x64.ax"
```

**Encoding Filters Pack** (multiple filters):
```batch
REM NVENC Encoder
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_NVENC.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_NVENC_x64.ax"

REM H.264 Encoder
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_H264_Encoder.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_H264_Encoder_x64.ax"

REM AAC Encoder
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_AAC_Encoder.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_AAC_Encoder_x64.ax"

REM MP4 Muxer
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_MP4_Muxer.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_MP4_Muxer_x64.ax"
```

**Virtual Camera SDK**:
```batch
REM Virtual Camera Driver
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Virtual_Camera.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Virtual_Camera_x64.ax"

REM Push Source Filter
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Push_Video_Source.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Push_Video_Source_x64.ax"
```

---
### Method 3: Programmatic Registration (C++)
Register filters programmatically from your application code.
#### Using LoadLibraryEx and DllRegisterServer
```cpp
#include <windows.h>
#include <iostream>
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
HRESULT RegisterFilter(const wchar_t* filterPath)
{
    // LOAD_WITH_ALTERED_SEARCH_PATH: the filter's own DLLs (FFmpeg, VLC) load from the filter's folder
    HMODULE hModule = LoadLibraryExW(filterPath, nullptr, LOAD_WITH_ALTERED_SEARCH_PATH);
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
    // LOAD_WITH_ALTERED_SEARCH_PATH: the filter's own DLLs (FFmpeg, VLC) load from the filter's folder
    HMODULE hModule = LoadLibraryExW(filterPath, nullptr, LOAD_WITH_ALTERED_SEARCH_PATH);
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
    const wchar_t* filterPath = L"C:\\Program Files (x86)\\VisioForge\\FFMPEG Source DirectShow filter\\Redist\\x64\\VisioForge_FFMPEG_Source_x64.ax";
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
Several VisioForge filter setups (VLC Source, Virtual Camera SDK, Video Encryption SDK, the Encoding and the Processing Filters Packs) ship a `reg_special.exe` utility next to their filters. It registers every `.ax` and `.ocx` file in its own folder:
```cpp
#include <windows.h>
#include <shellapi.h>
// reg_special.exe registers every .ax and .ocx file in its own folder; -u unregisters them.
// It always exits with 0: the result for each file goes to reg_special.log in that folder.
HRESULT RunRegSpecial(const wchar_t* regSpecialPath, bool unregister)
{
    // Execute registration utility
    SHELLEXECUTEINFO sei = { sizeof(sei) };
    sei.lpVerb = L"runas";  // Run as administrator
    sei.lpFile = regSpecialPath;  // full path to reg_special.exe in the filters folder
    sei.lpParameters = unregister ? L"-u" : nullptr;
    sei.nShow = SW_HIDE;
    sei.fMask = SEE_MASK_NOCLOSEPROCESS;
    if (!ShellExecuteEx(&sei))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    // Wait for completion
    WaitForSingleObject(sei.hProcess, INFINITE);
    CloseHandle(sei.hProcess);
    return S_OK;
}
```
---

### Method 4: Programmatic Registration (.NET/C#)

Register filters from .NET applications using P/Invoke.

<!-- vf-compile -->
```csharp
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

public class FilterRegistration
{
    // LOAD_WITH_ALTERED_SEARCH_PATH: the filter's own DLLs (FFmpeg, VLC) load from the filter's folder
    private const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

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
        IntPtr hModule = LoadLibraryEx(filterPath, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
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
        IntPtr hModule = LoadLibraryEx(filterPath, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
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
        string filterPath = @"C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist\x64\VisioForge_FFMPEG_Source_x64.ax";

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
REM Open Registry Editor
regedit
REM Navigate to:
HKEY_CLASSES_ROOT\CLSID\{GUID}
REM Example for FFMPEG Source:
REM HKEY_CLASSES_ROOT\CLSID\{C5255DE3-50A7-4714-B763-D99E96E4CD52}
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
        { 0xC5255DE3, 0x50A7, 0x4714, { 0xB7, 0x63, 0xD9, 0x9E, 0x96, 0xE4, 0xCD, 0x52 } };
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
Guid ffmpegSourceClsid = new Guid("C5255DE3-50A7-4714-B763-D99E96E4CD52");
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
REM Always run as Administrator for filter registration
REM Right-click Command Prompt → Run as Administrator
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
**VisioForge.FFMPEGSource.manifest** (the file name must match the assembly name; place it in the application folder, next to the .ax file, its DLLs and `cacert.pem`):
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
  <assemblyIdentity
    type="win32"
    name="VisioForge.FFMPEGSource"
    version="1.0.0.0"/>
  <file name="VisioForge_FFMPEG_Source_x64.ax">
    <comClass
      clsid="{C5255DE3-50A7-4714-B763-D99E96E4CD52}"
      threadingModel="Both"/>
    <comClass
      clsid="{934BBFAE-DE28-46C9-A2EC-85C84A1992BA}"
      threadingModel="Both"/>
    <comClass
      clsid="{D6EA52DC-1F28-4155-8CC8-081FED639473}"
      threadingModel="Both"/>
  </file>
</assembly>
```
For a 32-bit application, name the x86 filter in the `file` element: `VisioForge_FFMPEG_Source.ax`, with the DLLs and `cacert.pem` of the `x86` folder next to it.

**Application manifest**: add the `dependency` element below to the manifest embedded in your .exe.

- .NET: add it to the project's `app.manifest` (Add > New Item > Application Manifest File) and keep that file's `trustInfo` and `compatibility` sections. The project file must reference it: `<ApplicationManifest>app.manifest</ApplicationManifest>` (Visual Studio adds this line).
- C++: put it in a manifest file of its own and list that file under Manifest Tool > Input and Output > Additional Manifest Files.
- A `YourApp.exe.manifest` file next to the .exe is used only when the .exe has no embedded manifest.

The minimal manifest below shows the element:
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
- Graph building (`RenderFile`, Intelligent Connect) does not find the filters: the filter mapper lists only registered filters. Create them with `CoCreateInstance` and connect the pins yourself (`IGraphBuilder::AddFilter`, `ConnectDirect`)
- Only your own process sees the filters: a filter that other applications must find, such as the Virtual Camera or the Virtual Audio Card, needs registration (`regsvr32`)

---

## Batch Registration Scripts

### Register All Filters (Batch Script)

```batch
@echo off
setlocal
echo Registering VisioForge DirectShow Filters...
echo.

REM Check for Administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

REM Folder where the FFMPEG Source setup installs both filters
set INSTALL_PATH=C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist
set FAILED=
set FOUND=

REM Register FFMPEG Source (x64 and x86)
echo Registering FFMPEG Source...
call :register "%INSTALL_PATH%\x64\VisioForge_FFMPEG_Source_x64.ax"
call :register "%INSTALL_PATH%\x86\VisioForge_FFMPEG_Source.ax"

REM Register the other filters your application ships the same way, from their own folders:
REM call :register "<filter folder>\<filter>.ax"

echo.
if not defined FOUND (
    echo No VisioForge filter files found. Check INSTALL_PATH.
    pause
    exit /b 1
)
if defined FAILED (
    echo Registration finished with errors.
    pause
    exit /b 1
)
echo Registration complete!
pause
exit /b 0

REM Register one filter and report the result
:register
if not exist %1 (
    echo   [SKIPPED] %~nx1 - not found
    exit /b
)
set FOUND=1
regsvr32 /s %1
if %errorLevel% equ 0 (
    echo   [OK] %~nx1
) else (
    echo   [FAILED] %~nx1
    set FAILED=1
)
exit /b
```

### Unregister All Filters

```batch
@echo off
setlocal
echo Unregistering VisioForge DirectShow Filters...
echo.

REM Check for Administrator privileges
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

set INSTALL_PATH=C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist
set FAILED=
set FOUND=

REM Unregister all filters
call :unregister "%INSTALL_PATH%\x64\VisioForge_FFMPEG_Source_x64.ax"
call :unregister "%INSTALL_PATH%\x86\VisioForge_FFMPEG_Source.ax"

REM Unregister the other filters your application ships the same way:
REM call :unregister "<filter folder>\<filter>.ax"

echo.
if not defined FOUND (
    echo No VisioForge filter files found. Check INSTALL_PATH.
    pause
    exit /b 1
)
if defined FAILED (
    echo Unregistration finished with errors.
    pause
    exit /b 1
)
echo Unregistration complete!
pause
exit /b 0

REM Unregister one filter and report the result
:unregister
if not exist %1 (
    echo   [SKIPPED] %~nx1 - not found
    exit /b
)
set FOUND=1
regsvr32 /s /u %1
if %errorLevel% equ 0 (
    echo   [OK] %~nx1
) else (
    echo   [FAILED] %~nx1
    set FAILED=1
)
exit /b
```

---
## See Also

- [Redistributable Files](redistributable-files.md) - Complete file list for each SDK
- [Installer Integration](installer-integration.md) - Creating custom installers
- [Deployment Overview](index.md) - Main deployment guide