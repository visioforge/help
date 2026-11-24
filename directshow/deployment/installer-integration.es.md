---
title: Desplegar filtros DirectShow con WiX, NSIS e Inno Setup
description: Integre filtros DirectShow en instaladores WiX, NSIS, InstallShield con acciones personalizadas, registro y gestión de dependencias.
tags:
  - DirectShow
  - C++
  - Windows

---

# Guía de integración con instaladores

## Resumen

Esta guía proporciona instrucciones completas para integrar filtros VisioForge DirectShow en instaladores Windows. Cubre múltiples tecnologías de instaladores, acciones personalizadas para registro de filtros, gestión de dependencias y mejores prácticas.

---

## Prerrequisitos

Antes de crear un instalador, asegúrese de entender:

- [Archivos redistribuibles](redistributable-files.md) - Archivos a incluir en el instalador
- [Registro de filtros](filter-registration.md) - Mecanismos de registro
- Arquitectura de plataforma objetivo (x86/x64)
- Requisitos de Visual C++ Redistributable

---

## Resumen de tecnologías de instaladores

### WiX Toolset

**Mejor para**: Aplicaciones empresariales, despliegues basados en MSI, automatización IT

**Ventajas**:

- Sintaxis declarativa basada en XML
- Soporte nativo MSI
- Excelente integración con Windows Installer
- Soporte de despliegue de Política de Grupo
- Desarrollo y comunidad activos

**Requisitos**:

- WiX Toolset 3.x o 4.x
- Integración con Visual Studio (opcional)
- Archivos de proyecto .wixproj

[Ver ejemplos WiX →](#ejemplos-wix-toolset)

---

### NSIS (Nullsoft Scriptable Install System)

**Mejor para**: Instaladores ligeros, UI personalizada, aplicaciones portátiles

**Ventajas**:

- Tamaño pequeño del instalador
- Altamente personalizable
- Lenguaje de scripting simple
- Sin dependencias de runtime
- Ejecución rápida

**Requisitos**:

- Compilador NSIS 3.x
- Archivos de script .nsi

[Ver ejemplos NSIS →](#ejemplos-nsis)

---

### InstallShield

**Mejor para**: Aplicaciones comerciales, instalaciones complejas, características avanzadas

**Ventajas**:

- Diseñador GUI profesional
- Detección integrada de prerrequisitos
- Soporte multi-plataforma
- Creación de suites/paquetes
- Integración con Visual Studio

**Requisitos**:

- InstallShield Limited Edition (Visual Studio) o Professional
- Archivos de proyecto .ism

[Ver guía InstallShield →](#integracion-installshield)

---

### Inno Setup

**Mejor para**: Instaladores simples, aplicaciones pequeñas, software gratuito

**Ventajas**:

- Gratuito y de código abierto
- Soporte de scripting Pascal
- Soporte Unicode
- Buena documentación
- Comunidad activa

**Requisitos**:

- Compilador Inno Setup 6.x
- Archivos de script .iss

[Ver ejemplos Inno Setup →](#ejemplos-inno-setup)

---

## Ejemplos WiX Toolset

### Instalación básica de filtros

Cree un instalador WiX completo para un filtro DirectShow con registro automático.

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

    <!-- Estructura de directorios de instalación -->
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFiles64Folder">
        <Directory Id="INSTALLFOLDER" Name="MyApp">
          <Directory Id="FilterFolder" Name="Filters" />
        </Directory>
      </Directory>
    </Directory>

    <!-- Definición de características -->
    <Feature Id="ProductFeature" Title="MyApp" Level="1">
      <ComponentGroupRef Id="FilterComponents" />
      <ComponentGroupRef Id="ApplicationComponents" />
    </Feature>

    <!-- Enlaza VCRedist.wxs para que se ejecute su comprobación del runtime -->
    <PropertyRef Id="VCREDIST2022_X64" />

    <!-- Acciones personalizadas para registro -->
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

    <!-- Reversión: desregistrar el filtro si la instalación falla después de RegisterFilters -->
    <CustomAction Id="RegisterFiltersRollback"
                  Directory="FilterFolder"
                  ExeCommand="cmd.exe /c regsvr32 /s /u VisioForge_FFMPEG_Source_x64.ax"
                  Execute="rollback"
                  Impersonate="no"
                  Return="ignore" />

    <!-- Reversión de una desinstalación: volver a registrar el filtro -->
    <CustomAction Id="UnregisterFiltersRollback"
                  Directory="FilterFolder"
                  ExeCommand="cmd.exe /c regsvr32 /s VisioForge_FFMPEG_Source_x64.ax"
                  Execute="rollback"
                  Impersonate="no"
                  Return="ignore" />

    <InstallExecuteSequence>
      <Custom Action="RegisterFiltersRollback" Before="RegisterFilters">NOT Installed</Custom>
      <Custom Action="RegisterFilters" After="InstallFiles">NOT Installed OR REINSTALL</Custom>
      <Custom Action="UnregisterFiltersRollback" Before="UnregisterFilters">REMOVE="ALL"</Custom>
      <Custom Action="UnregisterFilters" Before="RemoveFiles">REMOVE="ALL"</Custom>
    </InstallExecuteSequence>

  </Product>
</Wix>
```

#### Filters.wxs (definición de componentes)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>
    <ComponentGroup Id="FilterComponents" Directory="FilterFolder">

      <!-- Filtro FFMPEG Source -->
      <Component Id="FFMPEGSourceFilter" Guid="YOUR-GUID-1">
        <File Id="FFMPEGSourceAX"
              Source="$(var.SourceDir)\VisioForge_FFMPEG_Source_x64.ax"
              KeyPath="yes" />
      </Component>

      <!-- Dependencias FFMPEG (VCRedist.wxs comprueba el runtime de VC++ 2015-2022) -->
      <Component Id="FFMPEGLibraries" Guid="YOUR-GUID-2">
        <File Id="avcodec63" Source="$(var.SourceDir)\avcodec-63.dll" />
        <File Id="avformat63" Source="$(var.SourceDir)\avformat-63.dll" />
        <File Id="avutil61" Source="$(var.SourceDir)\avutil-61.dll" />
        <File Id="swresample7" Source="$(var.SourceDir)\swresample-7.dll" />
        <File Id="swscale10" Source="$(var.SourceDir)\swscale-10.dll" />
        <File Id="cacert" Source="$(var.SourceDir)\cacert.pem" />
        <File Id="dav1dLicense" Source="$(var.SourceDir)\dav1d-license.txt" />
      </Component>

    </ComponentGroup>
  </Fragment>
</Wix>
```

#### VCRedist.wxs (verificación de prerrequisitos)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>

    <!-- Detectar Visual C++ Redistributable 2015-2022 -->
    <Property Id="VCREDIST2022_X64">
      <RegistrySearch Id="VCRedist2022x64"
                      Root="HKLM"
                      Key="SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64"
                      Name="Installed"
                      Type="raw"
                      Win64="no" />
    </Property>

    <Condition Message="This application requires Visual C++ 2015-2022 Redistributable (x64). Please install it from https://aka.ms/vs/17/release/vc_redist.x64.exe">
      <![CDATA[Installed OR VCREDIST2022_X64 = "#1"]]>
    </Condition>

  </Fragment>
</Wix>
```

#### Construyendo instalador WiX

```bash
# Usando WiX 3.x línea de comandos
candle.exe -arch x64 Product.wxs Filters.wxs VCRedist.wxs -ext WixUIExtension
light.exe -out MyApp.msi Product.wixobj Filters.wixobj VCRedist.wixobj -ext WixUIExtension

# Usando WiX 4.x (sintaxis más nueva); convierta antes las fuentes WiX 3 anteriores: wix convert *.wxs
wix build -arch x64 Product.wxs Filters.wxs VCRedist.wxs -ext WixToolset.UI.wixext -out MyApp.msi
```

---

### WiX avanzado: Paquete autoextraíble

Cree un paquete que incluya Visual C++ Redistributable.
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
      <!-- Instalar VC++ Redistributable primero -->
      <PackageGroupRef Id="VCRedist2022x64" />
      <!-- Luego instalar aplicación principal -->
      <MsiPackage SourceFile="MyApp.msi"
                  DisplayName="MyApp"
                  Vital="yes" />
    </Chain>
  </Bundle>
  <!-- Grupo de paquetes VC++ Redistributable -->
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
Construir paquete:
```bash
# WiX 3.x
candle.exe Bundle.wxs -ext WixBalExtension
light.exe -out MyAppSetup.exe Bundle.wixobj -ext WixBalExtension
# WiX 4.x
wix build Bundle.wxs -ext WixToolset.Bal.wixext -out MyAppSetup.exe
```
---

### WiX: DLL C++ personalizada para registro

Para más control, cree una acción personalizada DLL.

#### CustomActions.cpp

```cpp
#include <windows.h>
#include <msiquery.h>
#include <strsafe.h>

#pragma comment(lib, "msi.lib")

// Compile esta DLL como x64: una DLL de acción personalizada se ejecuta en un proceso de su propia arquitectura,
// así que solo una DLL x64 puede cargar el filtro x64

// Declaraciones hacia adelante
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
typedef HRESULT (STDAPICALLTYPE *LPFNDLLUNREGISTERSERVER)();

// Función auxiliar para escribir en log MSI
void LogMessage(MSIHANDLE hInstall, LPCTSTR message)
{
    PMSIHANDLE hRecord = MsiCreateRecord(1);
    MsiRecordSetString(hRecord, 0, message);
    MsiProcessMessage(hInstall, INSTALLMESSAGE_INFO, hRecord);
}

// Acción personalizada: Registrar filtros DirectShow
extern "C" __declspec(dllexport) UINT __stdcall RegisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR filterDir[MAX_PATH];
    DWORD filterDirSize = MAX_PATH;

    // Una acción personalizada diferida solo lee CustomActionData, que CustomActions.wxs establece en [FilterFolder]
    if (MsiGetProperty(hInstall, TEXT("CustomActionData"), filterDir, &filterDirSize) != ERROR_SUCCESS
        || filterDirSize == 0)
    {
        LogMessage(hInstall, TEXT("CustomActionData (filter folder) is empty or longer than MAX_PATH"));
        return ERROR_INSTALL_FAILURE;
    }

    LogMessage(hInstall, TEXT("Registering DirectShow filters..."));

    // Construir ruta al filtro
    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, filterDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("VisioForge_FFMPEG_Source_x64.ax"));

    // Cargar DLL del filtro
    HMODULE hModule = LoadLibraryEx(filterPath, NULL, LOAD_WITH_ALTERED_SEARCH_PATH);
    if (!hModule)
    {
        TCHAR errorMsg[512];
        StringCchPrintf(errorMsg, 512, TEXT("Failed to load filter: %s (Error: %d)"),
                       filterPath, GetLastError());
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }

    // Obtener función DllRegisterServer
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");

    if (!pfnRegister)
    {
        LogMessage(hInstall, TEXT("DllRegisterServer not found in filter"));
        FreeLibrary(hModule);
        return ERROR_INSTALL_FAILURE;
    }

    // Registrar filtro
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

// Acción personalizada: Desregistrar filtros DirectShow
extern "C" __declspec(dllexport) UINT __stdcall UnregisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR filterDir[MAX_PATH];
    DWORD filterDirSize = MAX_PATH;

    if (MsiGetProperty(hInstall, TEXT("CustomActionData"), filterDir, &filterDirSize) != ERROR_SUCCESS
        || filterDirSize == 0)
    {
        // No fallar desinstalación si no podemos obtener la ruta
        LogMessage(hInstall, TEXT("CustomActionData (filter folder) is empty or longer than MAX_PATH, filter not unregistered"));
        return ERROR_SUCCESS;
    }

    LogMessage(hInstall, TEXT("Unregistering DirectShow filters..."));

    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, filterDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("VisioForge_FFMPEG_Source_x64.ax"));

    HMODULE hModule = LoadLibraryEx(filterPath, NULL, LOAD_WITH_ALTERED_SEARCH_PATH);
    if (!hModule)
    {
        // Puede que el filtro ya se haya eliminado: no fallar, pero registrar por qué no se cargó
        TCHAR errorMsg[512];
        StringCchPrintf(errorMsg, 512, TEXT("Failed to load filter: %s (Error: %d)"),
                       filterPath, GetLastError());
        LogMessage(hInstall, errorMsg);
        return ERROR_SUCCESS;
    }

    LPFNDLLUNREGISTERSERVER pfnUnregister =
        (LPFNDLLUNREGISTERSERVER)GetProcAddress(hModule, "DllUnregisterServer");

    HRESULT hr = pfnUnregister ? pfnUnregister() : E_NOTIMPL;
    FreeLibrary(hModule);

    TCHAR resultMsg[256];
    StringCchPrintf(resultMsg, 256, TEXT("DirectShow filters unregistered: HRESULT 0x%08X"), hr);
    LogMessage(hInstall, resultMsg);

    return ERROR_SUCCESS;
}

// Punto de entrada de la DLL
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

    <!-- Binario para acciones personalizadas -->
    <Binary Id="CustomActionsDLL" SourceFile="$(var.CustomActions.TargetPath)" />

    <!-- Una acción diferida no puede leer INSTALLFOLDER: pase la carpeta del filtro en CustomActionData -->
    <CustomAction Id="SetRegisterFiltersCA" Property="RegisterFiltersCA" Value="[FilterFolder]" />
    <CustomAction Id="SetUnregisterFiltersCA" Property="UnregisterFiltersCA" Value="[FilterFolder]" />

    <!-- Definir acciones personalizadas -->
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

    <!-- Programar acciones personalizadas -->
    <InstallExecuteSequence>
      <Custom Action="SetRegisterFiltersCA" Before="RegisterFiltersCA">
        NOT Installed OR REINSTALL
      </Custom>
      <Custom Action="RegisterFiltersCA" After="InstallFiles">
        NOT Installed OR REINSTALL
      </Custom>
      <Custom Action="SetUnregisterFiltersCA" Before="UnregisterFiltersCA">
        REMOVE="ALL"
      </Custom>
      <Custom Action="UnregisterFiltersCA" Before="RemoveFiles">
        REMOVE="ALL"
      </Custom>
    </InstallExecuteSequence>

  </Fragment>
</Wix>
```

---

## Ejemplos NSIS

### Instalador NSIS básico

Cree un script de instalador NSIS completo.
#### Installer.nsi
```nsis
; Instalador MyApp con filtros DirectShow
; Script NSIS 3.x
;--------------------------------
; Incluye
!include "MUI2.nsh"
!include "x64.nsh"
;--------------------------------
; General
Name "MyApp"
OutFile "MyAppSetup.exe"
Unicode True
; Carpeta de instalación predeterminada
InstallDir "$PROGRAMFILES64\MyApp"
; Obtener carpeta de instalación del registro si está disponible
InstallDirRegKey HKLM "Software\MyApp" "InstallDir"
; Solicitar privilegios de aplicación
RequestExecutionLevel admin
;--------------------------------
; Configuración de interfaz
!define MUI_ABORTWARNING
!define MUI_ICON "installer.ico"
!define MUI_UNICON "uninstaller.ico"
;--------------------------------
; Páginas
!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;--------------------------------
; Idiomas
!insertmacro MUI_LANGUAGE "English"
;--------------------------------
; Información de versión
VIProductVersion "1.0.0.0"
VIAddVersionKey "ProductName" "MyApp"
VIAddVersionKey "CompanyName" "Your Company"
VIAddVersionKey "FileDescription" "MyApp Installer"
VIAddVersionKey "FileVersion" "1.0.0.0"
;--------------------------------
; Secciones del instalador
Section "MyApp (required)" SecMain
  SectionIn RO
  ; Establecer ruta de salida
  SetOutPath "$INSTDIR"
  ; Instalar archivos de aplicación principales
  File "MyApp.exe"
  File "MyApp.exe.config"
  ; Crear subdirectorio Filters
  CreateDirectory "$INSTDIR\Filters"
  SetOutPath "$INSTDIR\Filters"
  ; Instalar filtro FFMPEG Source
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
  File "Filters\avcodec-63.dll"
  File "Filters\avformat-63.dll"
  File "Filters\avutil-61.dll"
  File "Filters\swresample-7.dll"
  File "Filters\swscale-10.dll"
  File "Filters\cacert.pem"
  File "Filters\dav1d-license.txt"
  ; Registrar filtro DirectShow
  DetailPrint "Registering DirectShow filters..."
  ; Los instaladores NSIS son de 32 bits: $WINDIR\sysnative\regsvr32.exe ejecuta directamente el regsvr32 de 64 bits para el filtro x64
  ExecWait '"$WINDIR\sysnative\regsvr32.exe" /s "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"' $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "Filter registration failed. Code: $0"
  ${EndIf}
  ; Almacenar carpeta de instalación
  WriteRegStr HKLM "Software\MyApp" "InstallDir" $INSTDIR
  ; Crear desinstalador
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  ; Crear accesos directos del menú Inicio
  CreateDirectory "$SMPROGRAMS\MyApp"
  CreateShortcut "$SMPROGRAMS\MyApp\MyApp.lnk" "$INSTDIR\MyApp.exe"
  CreateShortcut "$SMPROGRAMS\MyApp\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  ; Entrada Agregar/Quitar Programas
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayName" "MyApp"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayIcon" "$INSTDIR\MyApp.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "Publisher" "Your Company"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayVersion" "1.0.0.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoRepair" 1
SectionEnd
;--------------------------------
; Secciones opcionales
Section "VLC Source Filter" SecVLC
  SetOutPath "$INSTDIR\Filters"
  ; Instalar filtro VLC Source
  File "Filters\VisioForge_VLC_Source_x64.ax"
  File "Filters\libvlc.dll"
  File "Filters\libvlccore.dll"
  ; Instalar directorio plugins VLC
  SetOutPath "$INSTDIR\Filters\plugins"
  File /r "Filters\plugins\*.*"
  ; Registrar filtro VLC Source
  DetailPrint "Registering VLC Source filter..."
  ExecWait '"$WINDIR\sysnative\regsvr32.exe" /s "$INSTDIR\Filters\VisioForge_VLC_Source_x64.ax"' $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "Filter registration failed. Code: $0"
  ${EndIf}
SectionEnd
;--------------------------------
; Descripciones de sección
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecMain} "Main application files and FFMPEG Source filter (required)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecVLC} "VLC Source filter for additional format support (optional)"
!insertmacro MUI_FUNCTION_DESCRIPTION_END
;--------------------------------
; Funciones del instalador
Function .onInit
  ; Verificar si Windows 64-bit
  ${If} ${RunningX64}
    ; OK
  ${Else}
    MessageBox MB_OK|MB_ICONSTOP "This application requires 64-bit Windows."
    Abort
  ${EndIf}
  ; Verificar Visual C++ Redistributable 2015-2022
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
; Sección desinstalador
Section "Uninstall"
  ; Desregistrar filtros
  DetailPrint "Unregistering DirectShow filters..."
  ExecWait '"$WINDIR\sysnative\regsvr32.exe" /s /u "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"'
  ExecWait '"$WINDIR\sysnative\regsvr32.exe" /s /u "$INSTDIR\Filters\VisioForge_VLC_Source_x64.ax"'
  ; Remover archivos
  Delete "$INSTDIR\MyApp.exe"
  Delete "$INSTDIR\MyApp.exe.config"
  Delete "$INSTDIR\Uninstall.exe"
  ; Remover directorio Filters
  Delete "$INSTDIR\Filters\*.ax"
  Delete "$INSTDIR\Filters\*.dll"
  Delete "$INSTDIR\Filters\cacert.pem"
  Delete "$INSTDIR\Filters\dav1d-license.txt"
  RMDir /r "$INSTDIR\Filters\plugins"
  RMDir "$INSTDIR\Filters"
  ; Remover directorio de instalación
  RMDir "$INSTDIR"
  ; Remover accesos directos del menú Inicio
  Delete "$SMPROGRAMS\MyApp\MyApp.lnk"
  Delete "$SMPROGRAMS\MyApp\Uninstall.lnk"
  RMDir "$SMPROGRAMS\MyApp"
  ; Remover claves de registro
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp"
  DeleteRegKey HKLM "Software\MyApp"
SectionEnd
```
#### Construyendo instalador NSIS
```bash
# Compilar con NSIS
makensis.exe Installer.nsi
# O usar GUI del compilador NSIS
# File > Load Script > Seleccionar Installer.nsi > Test Installer
```
---

### NSIS: Soporte de instalación silenciosa

Agregar parámetros de instalación silenciosa.

```nsis
; Agregar a función .onInit

; Verificar modo silencioso
${GetParameters} $R0
${GetOptions} $R0 "/S" $0
${IfNot} ${Errors}
  ; Modo silencioso - omitir verificaciones de prerrequisitos
  Goto silent_mode
${EndIf}

; Verificaciones normales aquí...

silent_mode:
  ; Continuar con instalación

; Para desinstalación silenciosa, agregar al desinstalador:
; Ejecutar con: Uninstall.exe /S
```

---

### NSIS: Plugin personalizado para registro

Cree un plugin NSIS para más control. Los instaladores NSIS y sus plugins son procesos de 32 bits, por lo que un plugin solo puede cargar y registrar el filtro x86 (`VisioForge_FFMPEG_Source.ax`). Registre el filtro x64 con el `regsvr32` de 64 bits, como en el script básico anterior. El filtro x86 necesita su propia carpeta con las DLL de FFmpeg x86, `cacert.pem`, `dav1d-license.txt` y el runtime de VC++ x86 (`msvcp140.dll`, `vcruntime140.dll`); el uso siguiente lo instala en `$INSTDIR\Filters\x86`.
#### FilterRegistration.cpp (plugin NSIS)
```cpp
#include <windows.h>
#include "pluginapi.h"
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
// Función de registro de filtro
extern "C" void __declspec(dllexport) RegisterFilter(
    HWND hwndParent,
    int string_size,
    TCHAR *variables,
    stack_t **stacktop,
    extra_parameters *extra)
{
    EXDLL_INIT();
    // Sacar ruta de filtro de la pila
    TCHAR filterPath[MAX_PATH];
    popstring(filterPath);
    // Cargar DLL
    HMODULE hModule = LoadLibraryEx(filterPath, NULL, LOAD_WITH_ALTERED_SEARCH_PATH);
    if (!hModule)
    {
        pushstring(_T("ERROR"));
        return;
    }
    // Obtener función de registro
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");
    if (!pfnRegister)
    {
        FreeLibrary(hModule);
        pushstring(_T("ERROR"));
        return;
    }
    // Registrar
    HRESULT hr = pfnRegister();
    FreeLibrary(hModule);
    pushstring(SUCCEEDED(hr) ? _T("OK") : _T("ERROR"));
}
BOOL WINAPI DllMain(HANDLE hInst, ULONG ul_reason_for_call, LPVOID lpReserved)
{
    return TRUE;
}
```
Uso en script NSIS:
```nsis
; En la sección de instalación: la carpeta del filtro x86 (el .ax x86, las DLL de FFmpeg, cacert.pem, dav1d-license.txt y el runtime de VC++)
SetOutPath "$INSTDIR\Filters\x86"
File "Filters\x86\*.*"
; Cargar plugin
FilterRegistration::RegisterFilter "$INSTDIR\Filters\x86\VisioForge_FFMPEG_Source.ax"
Pop $0
${If} $0 == "ERROR"
    MessageBox MB_OK "Filter registration failed"
${EndIf}

; En la sección Uninstall, antes de eliminar los archivos: NSIS es de 32 bits, así que $SYSDIR contiene el regsvr32 de 32 bits
ExecWait '"$SYSDIR\regsvr32.exe" /s /u "$INSTDIR\Filters\x86\VisioForge_FFMPEG_Source.ax"'
RMDir /r "$INSTDIR\Filters\x86"
```
---

## Integración InstallShield {: #integracion-installshield }

### Configuración básica de proyecto InstallShield

1. **Crear nuevo proyecto**:
   - File > New Project
   - Seleccionar "Basic MSI Project"
   - Establecer nombre de proyecto y ubicación

2. **Agregar archivos**:
   - Vista **Application Files**
   - Agregar archivos de filtro a `[INSTALLDIR]\Filters`
   - Agregar ejecutables de aplicación

3. **Agregar acción personalizada**:

#### Método 1: Usando regsvr32

1. Ir a **Behavior and Logic** > **Custom Actions**
2. Clic derecho en **Install** > **New Custom Action**
3. Establecer propiedades:
   - Name: `Register DirectShow Filters`
   - Type: `Stored in the Directory Table`
   - Working Directory: `[INSTALLDIR]Filters`
   - Filename: `[System64Folder]regsvr32.exe`
   - Command Line: `/s VisioForge_FFMPEG_Source_x64.ax`
   - Run: `Deferred Execution in System Context`
   - Condition: `NOT Installed OR REINSTALL`

4. Para desinstalar:
   - Name: `Unregister DirectShow Filters`
   - Type: `Stored in the Directory Table`
   - Working Directory: `[INSTALLDIR]Filters`
   - Filename: `[System64Folder]regsvr32.exe`
   - Command Line: `/s /u VisioForge_FFMPEG_Source_x64.ax`
   - Run: `Deferred Execution in System Context`
   - Sequence: antes de **RemoveFiles**
   - Condition: `REMOVE="ALL"`

#### Método 2: Usando DLL personalizada

1. Crear DLL C++ con código de registro (similar al ejemplo WiX arriba), compilada como x64 para el filtro x64
2. Agregar DLL a **Support Files** en InstallShield
3. Crear acción personalizada:
   - Type: `DLL from the installation`
   - DLL Name: `CustomActions.dll`
   - Function: `RegisterDirectShowFilters`

### InstallShield: Configuración de prerrequisitos

1. Ir a la vista **Redistributables**
2. Agregar **Microsoft Visual C++ 2015-2022 Redistributable (x64)**:
   - Clic derecho > **Add Prerequisite**
   - Examinar a `VC_redist.x64.exe`
   - Establecer: **Install Before This Application**

---

## Ejemplos Inno Setup

### Script básico Inno Setup

#### Setup.iss
```pascal
; Script de configuración MyApp para Inno Setup 6.x
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
; Aplicación principal
Source: "MyApp.exe"; DestDir: "{app}"; Flags: ignoreversion
; Filtro FFMPEG Source
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion regserver restartreplace uninsrestartdelete
Source: "Filters\avcodec-63.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avformat-63.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avutil-61.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\swresample-7.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\swscale-10.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\cacert.pem"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\dav1d-license.txt"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Icons]
Name: "{group}\MyApp"; Filename: "{app}\MyApp.exe"
Name: "{group}\Uninstall MyApp"; Filename: "{uninstallexe}"
[Run]
; Opcionalmente lanzar aplicación después de instalar
Filename: "{app}\MyApp.exe"; Description: "Launch MyApp"; Flags: nowait postinstall skipifsilent
[Registry]
Root: HKLM; Subkey: "Software\MyApp"; ValueType: string; ValueName: "InstallDir"; ValueData: "{app}"; Flags: uninsdeletekey
[Code]
// Verificar Visual C++ Redistributable
function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
  VCInstalled: Cardinal;
begin
  Result := True;
  // Verificar si VC++ 2015-2022 está instalado
  if not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64',
                            'Installed', VCInstalled) or (VCInstalled <> 1) then
  begin
    if MsgBox('Visual C++ 2015-2022 Redistributable (x64) is required.' + #13#10 +
              'Download and install now?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://aka.ms/vs/17/release/vc_redist.x64.exe', '', '', SW_SHOW, ewNoWait, ResultCode);
      Result := False;  // Abortar instalación
    end;
  end;
end;
```
#### Inno Setup avanzado: Registro personalizado
```pascal
[Files]
; Sin el flag regserver: la sección [Code] registra el filtro e informa de un fallo
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Code]
// RegisterServer y UnregisterServer ejecutan el regsvr32 de la arquitectura que indica su primer parámetro,
// así que el Setup de 32 bits registra el filtro x64 con el regsvr32 de 64 bits
procedure CurStepChanged(CurStep: TSetupStep);
var
  FilterPath: String;
begin
  if CurStep = ssPostInstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    try
      RegisterServer(True, FilterPath, False);
      Log('Filter registered successfully');
    except
      Log('Filter registration failed: ' + GetExceptionMessage);
      MsgBox('Warning: DirectShow filter registration failed.' + #13#10 +
             'You may need to register it manually.', mbError, MB_OK);
    end;
  end;
end;
// Llamado antes de desinstalación
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  FilterPath: String;
begin
  if CurUninstallStep = usUninstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    if not UnregisterServer(True, FilterPath, False) then
      Log('Filter unregistration failed: ' + FilterPath);
  end;
end;
```
---

## Instalación silenciosa

### Parámetros de instalación silenciosa

#### MSI (WiX, InstallShield MSI)

```bash
# Instalar silenciosamente
msiexec /i MyApp.msi /quiet /norestart

# Instalar silenciosamente con log
msiexec /i MyApp.msi /quiet /norestart /l*v install.log

# Desinstalar silenciosamente
msiexec /x MyApp.msi /quiet /norestart

# Instalar silenciosamente con directorio de instalación personalizado
msiexec /i MyApp.msi /quiet INSTALLFOLDER="C:\CustomPath\MyApp"
```

#### NSIS

```bash
# Instalar silenciosamente
MyAppSetup.exe /S

# Instalar silenciosamente con directorio personalizado
MyAppSetup.exe /S /D=C:\CustomPath\MyApp

# Desinstalar silenciosamente
Uninstall.exe /S
```

#### Inno Setup

```bash
# Instalar silenciosamente
MyAppSetup.exe /SILENT

# Muy silencioso (sin progreso)
MyAppSetup.exe /VERYSILENT

# Instalar silenciosamente con directorio personalizado
MyAppSetup.exe /SILENT /DIR="C:\CustomPath\MyApp"

# Desinstalar silenciosamente
unins000.exe /SILENT
```

---

## Empaquetado de dependencias

### Visual C++ Redistributable

#### Opción 1: Bootstrapper de descarga
```xml
<!-- WiX Bundle.wxs -->
<ExePackage Id="VCRedist2022"
            DownloadUrl="https://aka.ms/vs/17/release/vc_redist.x64.exe"
            InstallCommand="/install /quiet /norestart"
            DetectCondition="VCREDIST2022_X64" />
```
#### Opción 2: Incluir el redistribuible
```nsis
; NSIS
Section "VC++ Redistributable"
  File "Prerequisites\VC_redist.x64.exe"
  ExecWait '"$INSTDIR\VC_redist.x64.exe" /install /quiet /norestart'
  Delete "$INSTDIR\VC_redist.x64.exe"
SectionEnd
```
#### Opción 3: Módulos de mezcla (WiX)
```xml
<DirectoryRef Id="TARGETDIR">
  <Merge Id="VCRedist" SourceFile="$(var.VCRedistMergeModule)" DiskId="1" Language="0"/>
</DirectoryRef>
<Feature Id="VCRedist" Title="Visual C++ Runtime" AllowAdvertise="no" Display="hidden" Level="1">
  <MergeRef Id="VCRedist"/>
</Feature>
```
---

## Mejores prácticas

### Timing de registro

1. **Secuencia de instalación**:

   ```
   InstallFiles
   ↓
   Registrar filtros (acción personalizada)
   ↓
   InstallFinalize
   ```

2. **Secuencia de desinstalación**:

   ```
   Desregistrar filtros (acción personalizada)
   ↓
   RemoveFiles
   ↓
   UninstallFinalize
   ```

### Manejo de errores

**Siempre**:

- Registrar intentos de registro
- Verificar valores HRESULT
- Proporcionar retroalimentación de usuario en caso de fallo
- No fallar instalación completa si registro falla
- Permitir registro manual post-instalación

**Ejemplo de manejo de errores**:

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

### Soporte de reversión

Asegurar reversión apropiada si instalación falla:

```xml
<!-- Ejemplo de reversión WiX -->
<CustomAction Id="RegisterFiltersRollback"
              Directory="FilterFolder"
              ExeCommand="cmd.exe /c regsvr32 /s /u VisioForge_FFMPEG_Source_x64.ax"
              Execute="rollback"
              Impersonate="no"
              Return="ignore" />

<InstallExecuteSequence>
  <Custom Action="RegisterFiltersRollback" Before="RegisterFiltersCA">
    NOT Installed
  </Custom>
  <Custom Action="RegisterFiltersCA" After="InstallFiles">
    NOT Installed OR REINSTALL
  </Custom>
</InstallExecuteSequence>
```

### Privilegios de admin

**Siempre requerir** privilegios admin/elevados:

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

### Consideraciones de arquitectura

```xml
<!-- WiX: Paquetes separados para x86/x64 -->
<Product Platform="x64">
  <!-- Contenido x64 -->
</Product>

<Product Platform="x86">
  <!-- Contenido x86 -->
</Product>
```

```nsis
; NSIS: Detección de arquitectura runtime
${If} ${RunningX64}
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
${Else}
  File "Filters\VisioForge_FFMPEG_Source.ax"
${EndIf}
```

---

## Probando instalación

### Lista de verificación de pruebas manuales

- [ ] Instalar en Windows 10/11 limpio
- [ ] Verificar todos los archivos copiados
- [ ] Verificar registro de filtro (GraphEdit/GraphStudioNext)
- [ ] Probar funcionalidad de aplicación
- [ ] Desinstalar completamente
- [ ] Verificar no quedan archivos
- [ ] Verificar limpieza de registro
- [ ] Probar escenario de actualización
- [ ] Probar funcionalidad de reparación
- [ ] Probar instalación silenciosa
- [ ] Probar en diferentes cuentas de usuario
### Pruebas automatizadas
```powershell
# Script de prueba PowerShell
$installerPath = ".\MyAppSetup.msi"
$logPath = ".\install_test.log"
# Instalar silenciosamente
Start-Process msiexec.exe -ArgumentList "/i `"$installerPath`" /quiet /l*v `"$logPath`"" -Wait
# Verificar si filtro registrado
$filterCLSID = "{C5255DE3-50A7-4714-B763-D99E96E4CD52}"
$regPath = "HKLM:\SOFTWARE\Classes\CLSID\$filterCLSID"
if (Test-Path $regPath) {
    Write-Host "Filter registered successfully" -ForegroundColor Green
} else {
    Write-Host "Filter registration failed" -ForegroundColor Red
    Exit 1
}
# Desinstalar
Start-Process msiexec.exe -ArgumentList "/x `"$installerPath`" /quiet" -Wait
# Verificar limpieza
if (Test-Path $regPath) {
    Write-Host "Filter not unregistered" -ForegroundColor Red
    Exit 1
} else {
    Write-Host "Uninstall successful" -ForegroundColor Green
}
```
---

## Solución de problemas

### Problemas comunes

#### Registro falla con acceso denegado

**Causa**: Privilegios insuficientes

**Solución**:

```xml
<!-- Asegurar ejecución diferida con contexto de sistema -->
<CustomAction Execute="deferred" Impersonate="no" />
```

#### Filtro funciona en desarrollo pero no después de instalar

**Causa**: Dependencias faltantes o rutas incorrectas

**Solución**:

- Usar Dependency Walker para verificar todas las dependencias DLL
- Asegurar todas las DLL en mismo directorio que filtro
- Verificar variable de entorno PATH

#### Instalación silenciosa se cuelga

**Causa**: Interacción de usuario requerida

**Solución**:

```bash
# Agregar parámetro /norestart
msiexec /i MyApp.msi /quiet /norestart
```

#### Desinstalación deja entradas de registro

**Causa**: Acción personalizada de desregistro no ejecutándose

**Solución**:

```xml
<!-- Establecer Return="ignore" para desregistro -->
<CustomAction Return="ignore" />
```

---

## Ver también

### Documentación

- [Registro de filtros](filter-registration.md) - Métodos de registro manual
- [Archivos redistribuibles](redistributable-files.md) - Archivos a incluir en instalador
- [Resumen de despliegue](index.md) - Guía completa de despliegue
### Recursos externos

- [Documentación WiX Toolset](https://docs.firegiant.com/wix/)
- [Documentación NSIS](https://nsis.sourceforge.io/Docs/)
- [Documentación Inno Setup](https://jrsoftware.org/ishelp/)
- [Documentación Windows Installer (MSI)](https://learn.microsoft.com/en-us/windows/win32/Msi/windows-installer-portal)