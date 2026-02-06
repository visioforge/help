---
title: Installer Integration Guide - DirectShow SDKs
description: Integrate DirectShow filters into WiX, NSIS, InstallShield installers with custom actions, registration, and dependency management.
---

# Installer Integration Guide

## Overview

This guide provides comprehensive instructions for integrating VisioForge DirectShow filters into Windows installers. It covers multiple installer technologies, custom actions for filter registration, dependency management, and best practices.

---
## Prerequisites
Before creating an installer, ensure you understand:
- [Redistributable Files](redistributable-files.md) - Files to include in installer
- [Filter Registration](filter-registration.md) - Registration mechanisms
- Target platform architecture (x86/x64)
- Visual C++ Redistributable requirements
---

## Installer Technologies Overview

### WiX Toolset

**Best For**: Enterprise applications, MSI-based deployments, IT automation

**Advantages**:

- XML-based declarative syntax
- Native MSI support
- Excellent Windows Installer integration
- Group Policy deployment support
- Active development and community

**Requirements**:

- WiX Toolset 3.x or 4.x
- Visual Studio integration (optional)
- .wixproj project files

[View WiX Examples →](#wix-toolset-examples)

---
### NSIS (Nullsoft Scriptable Install System)
**Best For**: Lightweight installers, custom UI, portable applications
**Advantages**:
- Small installer size
- Highly customizable
- Simple scripting language
- No runtime dependencies
- Fast execution
**Requirements**:
- NSIS 3.x compiler
- .nsi script files
[View NSIS Examples →](#nsis-examples)
---

### InstallShield

**Best For**: Commercial applications, complex installations, advanced features

**Advantages**:

- Professional GUI designer
- Built-in prerequisites detection
- Multi-platform support
- Suite/bundle creation
- Visual Studio integration

**Requirements**:

- InstallShield Limited Edition (Visual Studio) or Professional
- .ism project files

[View InstallShield Guide →](#installshield-integration)

---
### Inno Setup
**Best For**: Simple installers, small applications, freeware
**Advantages**:
- Free and open-source
- Pascal scripting support
- Unicode support
- Good documentation
- Active community
**Requirements**:
- Inno Setup 6.x compiler
- .iss script files
[View Inno Setup Examples →](#inno-setup-examples)
---

## WiX Toolset Examples

### Basic Filter Installation

Create a complete WiX installer for a DirectShow filter with automatic registration.

#### Product.wxs

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*"
           Name="MyApp with DirectShow Filters"
           Language="1033"
           Version="1.0.0.0"
           Manufacturer="Your Company"
           UpgradeCode="YOUR-GUID-HERE">

    <Package InstallerVersion="200"
             Compressed="yes"
             InstallScope="perMachine"
             Platform="x64" />

    <MajorUpgrade DowngradeErrorMessage="A newer version is already installed." />

    <MediaTemplate EmbedCab="yes" />

    <!-- Installation directory structure -->
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFiles64Folder">
        <Directory Id="INSTALLFOLDER" Name="MyApp">
          <Directory Id="FilterFolder" Name="Filters" />
        </Directory>
      </Directory>
    </Directory>

    <!-- Feature definition -->
    <Feature Id="ProductFeature" Title="MyApp" Level="1">
      <ComponentGroupRef Id="FilterComponents" />
      <ComponentGroupRef Id="ApplicationComponents" />
    </Feature>

    <!-- Custom actions for registration -->
    <CustomAction Id="RegisterFilters"
                  Directory="FilterFolder"
                  ExeCommand="cmd.exe /c regsvr32 /s VisioForge_FFMPEG_Source_x64.ax"
                  Execute="deferred"
                  Impersonate="no"
                  Return="check" />

    <CustomAction Id="UnregisterFilters"
                  Directory="FilterFolder"
                  ExeCommand="cmd.exe /c regsvr32 /s /u VisioForge_FFMPEG_Source_x64.ax"
                  Execute="deferred"
                  Impersonate="no"
                  Return="ignore" />

    <InstallExecuteSequence>
      <Custom Action="RegisterFilters" After="InstallFiles">NOT Installed</Custom>
      <Custom Action="UnregisterFilters" Before="RemoveFiles">Installed</Custom>
    </InstallExecuteSequence>

  </Product>
</Wix>
```

#### Filters.wxs (Component Definition)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>
    <ComponentGroup Id="FilterComponents" Directory="FilterFolder">

      <!-- FFMPEG Source Filter -->
      <Component Id="FFMPEGSourceFilter" Guid="YOUR-GUID-1">
        <File Id="FFMPEGSourceAX"
              Source="$(var.SourceDir)\VisioForge_FFMPEG_Source_x64.ax"
              KeyPath="yes" />
      </Component>

      <!-- FFmpeg Dependencies -->
      <Component Id="FFMPEGLibraries" Guid="YOUR-GUID-2">
        <File Id="avcodec58" Source="$(var.SourceDir)\avcodec-58.dll" />
        <File Id="avdevice58" Source="$(var.SourceDir)\avdevice-58.dll" />
        <File Id="avfilter7" Source="$(var.SourceDir)\avfilter-7.dll" />
        <File Id="avformat58" Source="$(var.SourceDir)\avformat-58.dll" />
        <File Id="avutil56" Source="$(var.SourceDir)\avutil-56.dll" />
        <File Id="swresample3" Source="$(var.SourceDir)\swresample-3.dll" />
        <File Id="swscale5" Source="$(var.SourceDir)\swscale-5.dll" />
      </Component>

    </ComponentGroup>
  </Fragment>
</Wix>
```

#### VCRedist.wxs (Prerequisite Check)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>

    <!-- Detect Visual C++ Redistributable 2015-2022 -->
    <Property Id="VCREDIST2022_X64">
      <RegistrySearch Id="VCRedist2022x64"
                      Root="HKLM"
                      Key="SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64"
                      Name="Installed"
                      Type="raw" />
    </Property>

    <Condition Message="This application requires Visual C++ 2015-2022 Redistributable (x64). Please install it from https://aka.ms/vs/17/release/vc_redist.x64.exe">
      <![CDATA[Installed OR VCREDIST2022_X64]]>
    </Condition>

  </Fragment>
</Wix>
```

#### Building WiX Installer

```bash
# Using WiX 3.x command line
candle.exe Product.wxs Filters.wxs VCRedist.wxs -ext WixUIExtension
light.exe -out MyApp.msi Product.wixobj Filters.wixobj VCRedist.wixobj -ext WixUIExtension

# Using WiX 4.x (newer syntax)
wix build Product.wxs Filters.wxs VCRedist.wxs -ext WixToolset.UI.wixext -out MyApp.msi
```

---
### Advanced WiX: Self-Extracting Bundle
Create a bundle that includes Visual C++ Redistributable.
#### Bundle.wxs
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi"
     xmlns:bal="http://schemas.microsoft.com/wix/BalExtension"
     xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">
  <Bundle Name="MyApp Complete Setup"
          Version="1.0.0.0"
          Manufacturer="Your Company"
          UpgradeCode="YOUR-BUNDLE-GUID">
    <BootstrapperApplicationRef Id="WixStandardBootstrapperApplication.RtfLicense">
      <bal:WixStandardBootstrapperApplication
        LicenseFile="License.rtf"
        LogoFile="Logo.png" />
    </BootstrapperApplicationRef>
    <Chain>
      <!-- Install VC++ Redistributable first -->
      <PackageGroupRef Id="VCRedist2022x64" />
      <!-- Then install main application -->
      <MsiPackage SourceFile="MyApp.msi"
                  DisplayName="MyApp"
                  Vital="yes" />
    </Chain>
  </Bundle>
  <!-- VC++ Redistributable package group -->
  <Fragment>
    <PackageGroup Id="VCRedist2022x64">
      <ExePackage Id="VCRedist2022x64"
                  Cache="no"
                  Compressed="yes"
                  PerMachine="yes"
                  Permanent="yes"
                  Vital="yes"
                  SourceFile="VC_redist.x64.exe"
                  InstallCommand="/install /quiet /norestart"
                  DetectCondition="VCREDIST2022_X64"
                  InstallCondition="NOT VCREDIST2022_X64" />
    </PackageGroup>
  </Fragment>
</Wix>
```
Build bundle:
```bash
# WiX 3.x
candle.exe Bundle.wxs -ext WixBalExtension
light.exe -out MyAppSetup.exe Bundle.wixobj -ext WixBalExtension
# WiX 4.x
wix build Bundle.wxs -ext WixToolset.Bal.wixext -out MyAppSetup.exe
```
---

### WiX: Custom C++ DLL for Registration

For more control, create a custom action DLL.

#### CustomActions.cpp

```cpp
#include <windows.h>
#include <msiquery.h>
#include <strsafe.h>

#pragma comment(lib, "msi.lib")

// Forward declarations
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
typedef HRESULT (STDAPICALLTYPE *LPFNDLLUNREGISTERSERVER)();

// Helper function to write to MSI log
void LogMessage(MSIHANDLE hInstall, LPCTSTR message)
{
    PMSIHANDLE hRecord = MsiCreateRecord(1);
    MsiRecordSetString(hRecord, 0, message);
    MsiProcessMessage(hInstall, INSTALLMESSAGE_INFO, hRecord);
}

// Custom action: Register DirectShow filters
extern "C" __declspec(dllexport) UINT __stdcall RegisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR installDir[MAX_PATH];
    DWORD installDirSize = MAX_PATH;

    // Get INSTALLFOLDER property
    if (MsiGetProperty(hInstall, TEXT("INSTALLFOLDER"), installDir, &installDirSize) != ERROR_SUCCESS)
    {
        LogMessage(hInstall, TEXT("Failed to get INSTALLFOLDER property"));
        return ERROR_INSTALL_FAILURE;
    }

    LogMessage(hInstall, TEXT("Registering DirectShow filters..."));

    // Build path to filter
    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, installDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("Filters\\VisioForge_FFMPEG_Source_x64.ax"));

    // Load filter DLL
    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        TCHAR errorMsg[512];
        StringCchPrintf(errorMsg, 512, TEXT("Failed to load filter: %s (Error: %d)"),
                       filterPath, GetLastError());
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }

    // Get DllRegisterServer function
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");

    if (!pfnRegister)
    {
        LogMessage(hInstall, TEXT("DllRegisterServer not found in filter"));
        FreeLibrary(hModule);
        return ERROR_INSTALL_FAILURE;
    }

    // Register filter
    HRESULT hr = pfnRegister();
    FreeLibrary(hModule);

    if (SUCCEEDED(hr))
    {
        LogMessage(hInstall, TEXT("DirectShow filters registered successfully"));
        return ERROR_SUCCESS;
    }
    else
    {
        TCHAR errorMsg[256];
        StringCchPrintf(errorMsg, 256, TEXT("Filter registration failed: HRESULT 0x%08X"), hr);
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }
}

// Custom action: Unregister DirectShow filters
extern "C" __declspec(dllexport) UINT __stdcall UnregisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR installDir[MAX_PATH];
    DWORD installDirSize = MAX_PATH;

    if (MsiGetProperty(hInstall, TEXT("INSTALLFOLDER"), installDir, &installDirSize) != ERROR_SUCCESS)
    {
        // Don't fail uninstall if we can't get the path
        return ERROR_SUCCESS;
    }

    LogMessage(hInstall, TEXT("Unregistering DirectShow filters..."));

    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, installDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("Filters\\VisioForge_FFMPEG_Source_x64.ax"));

    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        // Filter may already be deleted, don't fail
        return ERROR_SUCCESS;
    }

    LPFNDLLUNREGISTERSERVER pfnUnregister =
        (LPFNDLLUNREGISTERSERVER)GetProcAddress(hModule, "DllUnregisterServer");

    if (pfnUnregister)
    {
        pfnUnregister();
    }

    FreeLibrary(hModule);
    LogMessage(hInstall, TEXT("DirectShow filters unregistered"));

    return ERROR_SUCCESS;
}

// DLL entry point
BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID reserved)
{
    return TRUE;
}
```

#### CustomActions.wxs

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>

    <!-- Binary for custom actions -->
    <Binary Id="CustomActionsDLL" SourceFile="$(var.CustomActions.TargetPath)" />

    <!-- Define custom actions -->
    <CustomAction Id="RegisterFiltersCA"
                  BinaryKey="CustomActionsDLL"
                  DllEntry="RegisterDirectShowFilters"
                  Execute="deferred"
                  Impersonate="no"
                  Return="check" />

    <CustomAction Id="UnregisterFiltersCA"
                  BinaryKey="CustomActionsDLL"
                  DllEntry="UnregisterDirectShowFilters"
                  Execute="deferred"
                  Impersonate="no"
                  Return="ignore" />

    <!-- Schedule custom actions -->
    <InstallExecuteSequence>
      <Custom Action="RegisterFiltersCA" After="InstallFiles">
        NOT Installed
      </Custom>
      <Custom Action="UnregisterFiltersCA" Before="RemoveFiles">
        Installed
      </Custom>
    </InstallExecuteSequence>

  </Fragment>
</Wix>
```

---
## NSIS Examples
### Basic NSIS Installer
Create a complete NSIS installer script.
#### Installer.nsi
```nsis
; MyApp Installer with DirectShow Filters
; NSIS 3.x script
;--------------------------------
; Includes
!include "MUI2.nsh"
!include "x64.nsh"
;--------------------------------
; General
Name "MyApp"
OutFile "MyAppSetup.exe"
Unicode True
; Default installation folder
InstallDir "$PROGRAMFILES64\MyApp"
; Get installation folder from registry if available
InstallDirRegKey HKLM "Software\MyApp" "InstallDir"
; Request application privileges
RequestExecutionLevel admin
;--------------------------------
; Interface Settings
!define MUI_ABORTWARNING
!define MUI_ICON "installer.ico"
!define MUI_UNICON "uninstaller.ico"
;--------------------------------
; Pages
!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;--------------------------------
; Languages
!insertmacro MUI_LANGUAGE "English"
;--------------------------------
; Version Information
VIProductVersion "1.0.0.0"
VIAddVersionKey "ProductName" "MyApp"
VIAddVersionKey "CompanyName" "Your Company"
VIAddVersionKey "FileDescription" "MyApp Installer"
VIAddVersionKey "FileVersion" "1.0.0.0"
;--------------------------------
; Installer Sections
Section "MyApp (required)" SecMain
  SectionIn RO
  ; Set output path
  SetOutPath "$INSTDIR"
  ; Install main application files
  File "MyApp.exe"
  File "MyApp.exe.config"
  ; Create Filters subdirectory
  CreateDirectory "$INSTDIR\Filters"
  SetOutPath "$INSTDIR\Filters"
  ; Install FFMPEG Source Filter
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
  File "Filters\avcodec-58.dll"
  File "Filters\avdevice-58.dll"
  File "Filters\avfilter-7.dll"
  File "Filters\avformat-58.dll"
  File "Filters\avutil-56.dll"
  File "Filters\swresample-3.dll"
  File "Filters\swscale-5.dll"
  ; Register DirectShow filter
  DetailPrint "Registering DirectShow filters..."
  ExecWait 'regsvr32 /s "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"' $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "Filter registration failed. Code: $0"
  ${EndIf}
  ; Store installation folder
  WriteRegStr HKLM "Software\MyApp" "InstallDir" $INSTDIR
  ; Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  ; Create Start Menu shortcuts
  CreateDirectory "$SMPROGRAMS\MyApp"
  CreateShortcut "$SMPROGRAMS\MyApp\MyApp.lnk" "$INSTDIR\MyApp.exe"
  CreateShortcut "$SMPROGRAMS\MyApp\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  ; Add/Remove Programs entry
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayName" "MyApp"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayIcon" "$INSTDIR\MyApp.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "Publisher" "Your Company"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayVersion" "1.0.0.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoRepair" 1
SectionEnd
;--------------------------------
; Optional Sections
Section "VLC Source Filter" SecVLC
  SetOutPath "$INSTDIR\Filters"
  ; Install VLC Source filter
  File "Filters\VisioForge_VLC_Source.ax"
  File "Filters\libvlc.dll"
  File "Filters\libvlccore.dll"
  ; Install VLC plugins directory
  SetOutPath "$INSTDIR\Filters\plugins"
  File /r "Filters\plugins\*.*"
  ; Register VLC Source filter
  DetailPrint "Registering VLC Source filter..."
  ExecWait 'regsvr32 /s "$INSTDIR\Filters\VisioForge_VLC_Source.ax"'
SectionEnd
;--------------------------------
; Section Descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecMain} "Main application files and FFMPEG Source filter (required)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecVLC} "VLC Source filter for additional format support (optional)"
!insertmacro MUI_FUNCTION_DESCRIPTION_END
;--------------------------------
; Installer Functions
Function .onInit
  ; Check if 64-bit Windows
  ${If} ${RunningX64}
    ; OK
  ${Else}
    MessageBox MB_OK|MB_ICONSTOP "This application requires 64-bit Windows."
    Abort
  ${EndIf}
  ; Check for Visual C++ Redistributable 2015-2022
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64" "Installed"
  ${If} $0 != 1
    MessageBox MB_YESNO|MB_ICONQUESTION "Visual C++ 2015-2022 Redistributable (x64) is required.$\n$\nDownload and install now?" IDYES download IDNO skip
    download:
      ExecShell "open" "https://aka.ms/vs/17/release/vc_redist.x64.exe"
      Abort
    skip:
  ${EndIf}
FunctionEnd
;--------------------------------
; Uninstaller Section
Section "Uninstall"
  ; Unregister filters
  DetailPrint "Unregistering DirectShow filters..."
  ExecWait 'regsvr32 /s /u "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"'
  ExecWait 'regsvr32 /s /u "$INSTDIR\Filters\VisioForge_VLC_Source.ax"'
  ; Remove files
  Delete "$INSTDIR\MyApp.exe"
  Delete "$INSTDIR\MyApp.exe.config"
  Delete "$INSTDIR\Uninstall.exe"
  ; Remove Filters directory
  Delete "$INSTDIR\Filters\*.ax"
  Delete "$INSTDIR\Filters\*.dll"
  RMDir /r "$INSTDIR\Filters\plugins"
  RMDir "$INSTDIR\Filters"
  ; Remove installation directory
  RMDir "$INSTDIR"
  ; Remove Start Menu shortcuts
  Delete "$SMPROGRAMS\MyApp\MyApp.lnk"
  Delete "$SMPROGRAMS\MyApp\Uninstall.lnk"
  RMDir "$SMPROGRAMS\MyApp"
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp"
  DeleteRegKey HKLM "Software\MyApp"
SectionEnd
```
#### Building NSIS Installer
```bash
# Compile with NSIS
makensis.exe Installer.nsi
# Or use NSIS compiler GUI
# File > Load Script > Select Installer.nsi > Test Installer
```
---

### NSIS: Silent Installation Support

Add silent installation parameters.

```nsis
; Add to .onInit function

; Check for silent mode
${GetParameters} $R0
${GetOptions} $R0 "/S" $0
${IfNot} ${Errors}
  ; Silent mode - skip prerequisite checks
  Goto silent_mode
${EndIf}

; Normal checks here...

silent_mode:
  ; Continue with installation

; For silent uninstall, add to uninstaller:
; Run with: Uninstall.exe /S
```

---
### NSIS: Custom Plugin for Registration
Create a custom NSIS plugin for more control.
#### FilterRegistration.cpp (NSIS Plugin)
```cpp
#include <windows.h>
#include "pluginapi.h"
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
// Register filter function
extern "C" void __declspec(dllexport) RegisterFilter(
    HWND hwndParent,
    int string_size,
    TCHAR *variables,
    stack_t **stacktop,
    extra_parameters *extra)
{
    EXDLL_INIT();
    // Pop filter path from stack
    TCHAR filterPath[MAX_PATH];
    popstring(filterPath);
    // Load DLL
    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        pushstring(_T("ERROR"));
        return;
    }
    // Get registration function
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");
    if (!pfnRegister)
    {
        FreeLibrary(hModule);
        pushstring(_T("ERROR"));
        return;
    }
    // Register
    HRESULT hr = pfnRegister();
    FreeLibrary(hModule);
    pushstring(SUCCEEDED(hr) ? _T("OK") : _T("ERROR"));
}
BOOL WINAPI DllMain(HANDLE hInst, ULONG ul_reason_for_call, LPVOID lpReserved)
{
    return TRUE;
}
```
Usage in NSIS script:
```nsis
; Load plugin
FilterRegistration::RegisterFilter "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"
Pop $0
${If} $0 == "ERROR"
    MessageBox MB_OK "Filter registration failed"
${EndIf}
```
---

## InstallShield Integration

### Basic InstallShield Project Setup

1. **Create New Project**:
   - File > New Project
   - Select "Basic MSI Project"
   - Set project name and location

2. **Add Files**:
   - Application Files view
   - Add filter files to `[INSTALLDIR]\Filters`
   - Add application executables

3. **Add Custom Action**:

#### Method 1: Using regsvr32

1. Go to **Behavior and Logic** > **Custom Actions**
2. Right-click **Install** > **New Custom Action**
3. Set properties:
   - Name: `Register DirectShow Filters`
   - Type: `Stored in the Directory Table`
   - Working Directory: `[INSTALLDIR]Filters`
   - Filename: `regsvr32.exe`
   - Command Line: `/s VisioForge_FFMPEG_Source_x64.ax`
   - Run: `Deferred Execution in System Context`
   - Condition: `NOT Installed`

4. For uninstall:
   - Name: `Unregister DirectShow Filters`
   - Command Line: `/s /u VisioForge_FFMPEG_Source_x64.ax`
   - Sequence: Before **RemoveFiles**
   - Condition: `Installed`

#### Method 2: Using Custom DLL

1. Create C++ DLL with registration code (similar to WiX example above)
2. Add DLL to **Support Files** in InstallShield
3. Create custom action:
   - Type: `DLL from the installation`
   - DLL Name: `CustomActions.dll`
   - Function: `RegisterDirectShowFilters`

### InstallShield: Prerequisite Configuration

1. Go to **Redistributables** view
2. Add **Microsoft Visual C++ 2015-2022 Redistributable (x64)**:
   - Right-click > **Add Prerequisite**
   - Browse to `VC_redist.x64.exe`
   - Set: **Install Before This Application**

---
## Inno Setup Examples
### Basic Inno Setup Script
#### Setup.iss
```pascal
; MyApp Setup Script for Inno Setup 6.x
[Setup]
AppName=MyApp
AppVersion=1.0
DefaultDirName={autopf}\MyApp
DefaultGroupName=MyApp
UninstallDisplayIcon={app}\MyApp.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Output
OutputBaseFilename=MyAppSetup
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
MinVersion=10.0
[Files]
; Main application
Source: "MyApp.exe"; DestDir: "{app}"; Flags: ignoreversion
; FFMPEG Source Filter
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion regserver restartreplace uninsrestartdelete
Source: "Filters\avcodec-58.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avdevice-58.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avfilter-7.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avformat-58.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avutil-56.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\swresample-3.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\swscale-5.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Icons]
Name: "{group}\MyApp"; Filename: "{app}\MyApp.exe"
Name: "{group}\Uninstall MyApp"; Filename: "{uninstallexe}"
[Run]
; Optionally launch application after install
Filename: "{app}\MyApp.exe"; Description: "Launch MyApp"; Flags: nowait postinstall skipifsilent
[Registry]
Root: HKLM; Subkey: "Software\MyApp"; ValueType: string; ValueName: "InstallDir"; ValueData: "{app}"; Flags: uninsdeletekey
[Code]
// Check for Visual C++ Redistributable
function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
  VCInstalled: Cardinal;
begin
  Result := True;
  // Check if VC++ 2015-2022 is installed
  if not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64',
                            'Installed', VCInstalled) or (VCInstalled <> 1) then
  begin
    if MsgBox('Visual C++ 2015-2022 Redistributable (x64) is required.' + #13#10 +
              'Download and install now?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://aka.ms/vs/17/release/vc_redist.x64.exe', '', '', SW_SHOW, ewNoWait, ResultCode);
      Result := False;  // Abort installation
    end;
  end;
end;
```
#### Advanced Inno Setup: Custom Registration
```pascal
[Files]
; Don't use regserver flag - we'll register manually
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Code]
// Import Windows API functions
function LoadLibrary(lpFileName: String): THandle;
  external 'LoadLibraryW@kernel32.dll stdcall';
function FreeLibrary(hModule: THandle): Boolean;
  external 'FreeLibrary@kernel32.dll stdcall';
function GetProcAddress(hModule: THandle; lpProcName: AnsiString): Longword;
  external 'GetProcAddress@kernel32.dll stdcall';
type
  TDllRegisterServer = function: HRESULT;
// Register DirectShow filter
function RegisterDirectShowFilter(FilterPath: String): Boolean;
var
  hModule: THandle;
  DllRegisterServer: TDllRegisterServer;
  RegisterFunc: Longword;
  hr: HRESULT;
begin
  Result := False;
  hModule := LoadLibrary(FilterPath);
  if hModule = 0 then
  begin
    Log('Failed to load filter: ' + FilterPath);
    Exit;
  end;
  try
    RegisterFunc := GetProcAddress(hModule, 'DllRegisterServer');
    if RegisterFunc = 0 then
    begin
      Log('DllRegisterServer not found');
      Exit;
    end;
    @DllRegisterServer := Pointer(RegisterFunc);
    hr := DllRegisterServer();
    Result := Succeeded(hr);
    if Result then
      Log('Filter registered successfully')
    else
      Log('Filter registration failed: ' + IntToHex(hr, 8));
  finally
    FreeLibrary(hModule);
  end;
end;
// Called after installation
procedure CurStepChanged(CurStep: TSetupStep);
var
  FilterPath: String;
begin
  if CurStep = ssPostInstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    if not RegisterDirectShowFilter(FilterPath) then
    begin
      MsgBox('Warning: DirectShow filter registration failed.' + #13#10 +
             'You may need to register it manually.', mbError, MB_OK);
    end;
  end;
end;
// Called before uninstallation
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
  FilterPath: String;
begin
  if CurUninstallStep = usUninstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    // Unregister using regsvr32
    Exec('regsvr32.exe', '/s /u "' + FilterPath + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
```
---

## Silent Installation

### Silent Installation Parameters

#### MSI (WiX, InstallShield MSI)

```bash
# Silent install
msiexec /i MyApp.msi /quiet /norestart

# Silent install with log
msiexec /i MyApp.msi /quiet /norestart /l*v install.log

# Silent uninstall
msiexec /x MyApp.msi /quiet /norestart

# Silent install with custom install directory
msiexec /i MyApp.msi /quiet INSTALLFOLDER="C:\CustomPath\MyApp"
```

#### NSIS

```bash
# Silent install
MyAppSetup.exe /S

# Silent install with custom directory
MyAppSetup.exe /S /D=C:\CustomPath\MyApp

# Silent uninstall
Uninstall.exe /S
```

#### Inno Setup

```bash
# Silent install
MyAppSetup.exe /SILENT

# Very silent (no progress)
MyAppSetup.exe /VERYSILENT

# Silent with custom directory
MyAppSetup.exe /SILENT /DIR="C:\CustomPath\MyApp"

# Silent uninstall
unins000.exe /SILENT
```

---
## Bundling Dependencies
### Visual C++ Redistributable
#### Option 1: Download Bootstrapper
```xml
<!-- WiX Bundle.wxs -->
<ExePackage Id="VCRedist2022"
            DownloadUrl="https://aka.ms/vs/17/release/vc_redist.x64.exe"
            InstallCommand="/install /quiet /norestart"
            DetectCondition="VCREDIST2022_X64" />
```
#### Option 2: Include Redistributable
```nsis
; NSIS
Section "VC++ Redistributable"
  File "Prerequisites\VC_redist.x64.exe"
  ExecWait '"$INSTDIR\VC_redist.x64.exe" /install /quiet /norestart'
  Delete "$INSTDIR\VC_redist.x64.exe"
SectionEnd
```
#### Option 3: Merge Modules (WiX)
```xml
<DirectoryRef Id="TARGETDIR">
  <Merge Id="VCRedist" SourceFile="$(var.VCRedistMergeModule)" DiskId="1" Language="0"/>
</DirectoryRef>
<Feature Id="VCRedist" Title="Visual C++ Runtime" AllowAdvertise="no" Display="hidden" Level="1">
  <MergeRef Id="VCRedist"/>
</Feature>
```
---

## Best Practices

### Registration Timing

1. **Install Sequence**:

   ```
   InstallFiles
   ↓
   Register Filters (Custom Action)
   ↓
   InstallFinalize
   ```

2. **Uninstall Sequence**:

   ```
   Unregister Filters (Custom Action)
   ↓
   RemoveFiles
   ↓
   UninstallFinalize
   ```

### Error Handling

**Always**:

- Log registration attempts
- Check HRESULT values
- Provide user feedback on failure
- Don't fail entire installation if registration fails
- Allow manual registration post-install

**Example error handling**:

```cpp
HRESULT hr = RegisterFilter(filterPath);
if (FAILED(hr))
{
    if (hr == REGDB_E_CLASSNOTREG)
        LogError("Class not registered - check dependencies");
    else if (hr == E_ACCESSDENIED)
        LogError("Access denied - requires admin privileges");
    else
        LogError("Registration failed with HRESULT: 0x%08X", hr);
}
```

### Rollback Support

Ensure proper rollback if installation fails:

```xml
<!-- WiX rollback example -->
<CustomAction Id="RegisterFiltersRollback"
              Directory="FilterFolder"
              ExeCommand="regsvr32 /s /u VisioForge_FFMPEG_Source_x64.ax"
              Execute="rollback"
              Impersonate="no" />

<InstallExecuteSequence>
  <Custom Action="RegisterFiltersRollback" Before="RegisterFiltersCA">
    NOT Installed
  </Custom>
  <Custom Action="RegisterFiltersCA" After="InstallFiles">
    NOT Installed
  </Custom>
</InstallExecuteSequence>
```

### Admin Privileges

**Always require** admin/elevated privileges:

```xml
<!-- WiX -->
<Package InstallScope="perMachine" InstallPrivileges="elevated" />
```

```nsis
; NSIS
RequestExecutionLevel admin
```

```pascal
{ Inno Setup }
PrivilegesRequired=admin
```

### Architecture Considerations

```xml
<!-- WiX: Separate packages for x86/x64 -->
<Product Platform="x64">
  <!-- x64 content -->
</Product>

<Product Platform="x86">
  <!-- x86 content -->
</Product>
```

```nsis
; NSIS: Runtime architecture detection
${If} ${RunningX64}
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
${Else}
  File "Filters\VisioForge_FFMPEG_Source_x86.ax"
${EndIf}
```

---
## Testing Installation
### Manual Testing Checklist
- [ ] Install on clean Windows 10/11
- [ ] Verify all files copied
- [ ] Check filter registration (GraphEdit/GraphStudioNext)
- [ ] Test application functionality
- [ ] Uninstall completely
- [ ] Verify no files remain
- [ ] Check registry cleanup
- [ ] Test upgrade scenario
- [ ] Test repair functionality
- [ ] Test silent installation
- [ ] Test on different user accounts
### Automated Testing
```powershell
# PowerShell test script
$installerPath = ".\MyAppSetup.msi"
$logPath = ".\install_test.log"
# Install silently
Start-Process msiexec.exe -ArgumentList "/i `"$installerPath`" /quiet /l*v `"$logPath`"" -Wait
# Check if filter registered
$filterCLSID = "{1974D893-83E4-4F89-9908-795C524CC17E}"
$regPath = "HKLM:\SOFTWARE\Classes\CLSID\$filterCLSID"
if (Test-Path $regPath) {
    Write-Host "Filter registered successfully" -ForegroundColor Green
} else {
    Write-Host "Filter registration failed" -ForegroundColor Red
    Exit 1
}
# Uninstall
Start-Process msiexec.exe -ArgumentList "/x `"$installerPath`" /quiet" -Wait
# Verify cleanup
if (Test-Path $regPath) {
    Write-Host "Filter not unregistered" -ForegroundColor Red
    Exit 1
} else {
    Write-Host "Uninstall successful" -ForegroundColor Green
}
```
---

## Troubleshooting

### Common Issues

#### Registration Fails with Access Denied

**Cause**: Insufficient privileges

**Solution**:

```xml
<!-- Ensure deferred execution with system context -->
<CustomAction Execute="deferred" Impersonate="no" />
```

#### Filter Works in Development but not After Install

**Cause**: Missing dependencies or incorrect paths

**Solution**:

- Use Dependency Walker to check all DLL dependencies
- Ensure all DLLs in same directory as filter
- Check PATH environment variable

#### Silent Install Hangs

**Cause**: User interaction required

**Solution**:

```bash
# Add /norestart parameter
msiexec /i MyApp.msi /quiet /norestart
```

#### Uninstall Leaves Registry Entries

**Cause**: Unregistration custom action not running

**Solution**:

```xml
<!-- Set Return="ignore" for unregistration -->
<CustomAction Return="ignore" />
```

---
## See Also
### Documentation
- [Filter Registration](filter-registration.md) - Manual registration methods
- [Redistributable Files](redistributable-files.md) - Files to include in installer
- [Deployment Overview](index.md) - Complete deployment guide
### External Resources
- [WiX Toolset Documentation](https://docs.firegiant.com/wix/)
- [NSIS Documentation](https://nsis.sourceforge.io/Docs/)
- [Inno Setup Documentation](https://jrsoftware.org/ishelp/)
- [Windows Installer (MSI) Documentation](https://learn.microsoft.com/en-us/windows/win32/Msi/windows-installer-portal)