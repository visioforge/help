---
title: Integración con Instaladores para SDKs DirectShow
description: Integre filtros DirectShow en instaladores WiX, NSIS, InstallShield con acciones personalizadas, registro y gestión de dependencias.
---

# Guía de Integración con Instaladores

## Resumen

Esta guía proporciona instrucciones completas para integrar filtros VisioForge DirectShow en instaladores Windows. Cubre múltiples tecnologías de instaladores, acciones personalizadas para registro de filtros, gestión de dependencias y mejores prácticas.

---
## Prerrequisitos
Antes de crear un instalador, asegúrese de entender:
- [Archivos Redistribuibles](redistributable-files.md) - Archivos a incluir en el instalador
- [Registro de Filtros](filter-registration.md) - Mecanismos de registro
- Arquitectura de plataforma objetivo (x86/x64)
- Requisitos de Visual C++ Redistributable
---

## Resumen de Tecnologías de Instaladores

### WiX Toolset

**Mejor Para**: Aplicaciones empresariales, despliegues basados en MSI, automatización IT

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

[Ver Ejemplos WiX →](#ejemplos-wix-toolset)

---
### NSIS (Nullsoft Scriptable Install System)
**Mejor Para**: Instaladores ligeros, UI personalizada, aplicaciones portátiles
**Ventajas**:
- Tamaño pequeño del instalador
- Altamente personalizable
- Lenguaje de scripting simple
- Sin dependencias de runtime
- Ejecución rápida
**Requisitos**:
- Compilador NSIS 3.x
- Archivos de script .nsi
[Ver Ejemplos NSIS →](#ejemplos-nsis)
---

### InstallShield

**Mejor Para**: Aplicaciones comerciales, instalaciones complejas, características avanzadas

**Ventajas**:

- Diseñador GUI profesional
- Detección integrada de prerrequisitos
- Soporte multi-plataforma
- Creación de suites/paquetes
- Integración con Visual Studio

**Requisitos**:

- InstallShield Limited Edition (Visual Studio) o Professional
- Archivos de proyecto .ism

[Ver Guía InstallShield →](#integracion-installshield)

---
### Inno Setup
**Mejor Para**: Instaladores simples, aplicaciones pequeñas, software gratuito
**Ventajas**:
- Gratuito y de código abierto
- Soporte de scripting Pascal
- Soporte Unicode
- Buena documentación
- Comunidad activa
**Requisitos**:
- Compilador Inno Setup 6.x
- Archivos de script .iss
[Ver Ejemplos Inno Setup →](#ejemplos-inno-setup)
---

## Ejemplos WiX Toolset

### Instalación Básica de Filtros

Cree un instalador WiX completo para un filtro DirectShow con registro automático.

#### Product.wxs

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*"
           Name="MyApp con Filtros DirectShow"
           Language="1033"
           Version="1.0.0.0"
           Manufacturer="Su Empresa"
           UpgradeCode="SU-GUID-AQUÍ">

    <Package InstallerVersion="200"
             Compressed="yes"
             InstallScope="perMachine"
             Platform="x64" />

    <MajorUpgrade DowngradeErrorMessage="Ya está instalada una versión más nueva." />

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

    <InstallExecuteSequence>
      <Custom Action="RegisterFilters" After="InstallFiles">NOT Installed</Custom>
      <Custom Action="UnregisterFilters" Before="RemoveFiles">Installed</Custom>
    </InstallExecuteSequence>

  </Product>
</Wix>
```

#### Filters.wxs (Definición de Componentes)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>
    <ComponentGroup Id="FilterComponents" Directory="FilterFolder">

      <!-- Filtro Fuente FFMPEG -->
      <Component Id="FFMPEGSourceFilter" Guid="SU-GUID-1">
        <File Id="FFMPEGSourceAX"
              Source="$(var.SourceDir)\VisioForge_FFMPEG_Source_x64.ax"
              KeyPath="yes" />
      </Component>

      <!-- Dependencias FFMPEG -->
      <Component Id="FFMPEGLibraries" Guid="SU-GUID-2">
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

#### VCRedist.wxs (Verificación de Prerrequisitos)

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
                      Type="raw" />
    </Property>

    <Condition Message="Esta aplicación requiere Visual C++ 2015-2022 Redistributable (x64). Por favor instálelo desde https://aka.ms/vs/17/release/vc_redist.x64.exe">
      <![CDATA[Installed OR VCREDIST2022_X64]]>
    </Condition>

  </Fragment>
</Wix>
```

#### Construyendo Instalador WiX

```bash
# Usando WiX 3.x línea de comandos
candle.exe Product.wxs Filters.wxs VCRedist.wxs -ext WixUIExtension
light.exe -out MyApp.msi Product.wixobj Filters.wixobj VCRedist.wixobj -ext WixUIExtension

# Usando WiX 4.x (sintaxis más nueva)
wix build Product.wxs Filters.wxs VCRedist.wxs -ext WixToolset.UI.wixext -out MyApp.msi
```

---
### WiX Avanzado: Paquete Autoextraíble
Cree un paquete que incluya Visual C++ Redistributable.
#### Bundle.wxs
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi"
     xmlns:bal="http://schemas.microsoft.com/wix/BalExtension"
     xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">
  <Bundle Name="Configuración Completa MyApp"
          Version="1.0.0.0"
          Manufacturer="Su Empresa"
          UpgradeCode="SU-BUNDLE-GUID">
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

### WiX: DLL C++ Personalizada para Registro

Para más control, cree una acción personalizada DLL.

#### CustomActions.cpp

```cpp
#include <windows.h>
#include <msiquery.h>
#include <strsafe.h>

#pragma comment(lib, "msi.lib")

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
    TCHAR installDir[MAX_PATH];
    DWORD installDirSize = MAX_PATH;

    // Obtener propiedad INSTALLFOLDER
    if (MsiGetProperty(hInstall, TEXT("INSTALLFOLDER"), installDir, &installDirSize) != ERROR_SUCCESS)
    {
        LogMessage(hInstall, TEXT("Error al obtener propiedad INSTALLFOLDER"));
        return ERROR_INSTALL_FAILURE;
    }

    LogMessage(hInstall, TEXT("Registrando filtros DirectShow..."));

    // Construir ruta al filtro
    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, installDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("Filters\\VisioForge_FFMPEG_Source_x64.ax"));

    // Cargar DLL del filtro
    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        TCHAR errorMsg[512];
        StringCchPrintf(errorMsg, 512, TEXT("Error al cargar filtro: %s (Error: %d)"),
                       filterPath, GetLastError());
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }

    // Obtener función DllRegisterServer
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");

    if (!pfnRegister)
    {
        LogMessage(hInstall, TEXT("DllRegisterServer no encontrado en filtro"));
        FreeLibrary(hModule);
        return ERROR_INSTALL_FAILURE;
    }

    // Registrar filtro
    HRESULT hr = pfnRegister();
    FreeLibrary(hModule);

    if (SUCCEEDED(hr))
    {
        LogMessage(hInstall, TEXT("Filtros DirectShow registrados exitosamente"));
        return ERROR_SUCCESS;
    }
    else
    {
        TCHAR errorMsg[256];
        StringCchPrintf(errorMsg, 256, TEXT("Registro de filtro falló: HRESULT 0x%08X"), hr);
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }
}

// Acción personalizada: Desregistrar filtros DirectShow
extern "C" __declspec(dllexport) UINT __stdcall UnregisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR installDir[MAX_PATH];
    DWORD installDirSize = MAX_PATH;

    if (MsiGetProperty(hInstall, TEXT("INSTALLFOLDER"), installDir, &installDirSize) != ERROR_SUCCESS)
    {
        // No fallar desinstalación si no podemos obtener la ruta
        return ERROR_SUCCESS;
    }

    LogMessage(hInstall, TEXT("Desregistrando filtros DirectShow..."));

    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, installDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("Filters\\VisioForge_FFMPEG_Source_x64.ax"));

    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        // El filtro puede ya estar eliminado, no fallar
        return ERROR_SUCCESS;
    }

    LPFNDLLUNREGISTERSERVER pfnUnregister =
        (LPFNDLLUNREGISTERSERVER)GetProcAddress(hModule, "DllUnregisterServer");

    if (pfnUnregister)
    {
        pfnUnregister();
    }

    FreeLibrary(hModule);
    LogMessage(hInstall, TEXT("Filtros DirectShow desregistrados"));

    return ERROR_SUCCESS;
}
```

#### CustomActions.wxs

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>

    <!-- Binario para acciones personalizadas -->
    <Binary Id="CustomActionsDLL" SourceFile="$(var.CustomActions.TargetPath)" />

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
## Ejemplos NSIS
### Instalador NSIS Básico
Cree un script de instalador NSIS completo.
#### Installer.nsi
```nsis
; Instalador MyApp con Filtros DirectShow
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
; Configuración de Interfaz
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
; Información de Versión
VIProductVersion "1.0.0.0"
VIAddVersionKey "ProductName" "MyApp"
VIAddVersionKey "CompanyName" "Su Empresa"
VIAddVersionKey "FileDescription" "Instalador MyApp"
VIAddVersionKey "FileVersion" "1.0.0.0"
;--------------------------------
; Secciones del Instalador
Section "MyApp (requerido)" SecMain
  SectionIn RO
  ; Establecer ruta de salida
  SetOutPath "$INSTDIR"
  ; Instalar archivos de aplicación principales
  File "MyApp.exe"
  File "MyApp.exe.config"
  ; Crear subdirectorio Filters
  CreateDirectory "$INSTDIR\Filters"
  SetOutPath "$INSTDIR\Filters"
  ; Instalar Filtro Fuente FFMPEG
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
  File "Filters\avcodec-58.dll"
  File "Filters\avdevice-58.dll"
  File "Filters\avfilter-7.dll"
  File "Filters\avformat-58.dll"
  File "Filters\avutil-56.dll"
  File "Filters\swresample-3.dll"
  File "Filters\swscale-5.dll"
  ; Registrar filtro DirectShow
  DetailPrint "Registrando filtros DirectShow..."
  ExecWait 'regsvr32 /s "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"' $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "Registro de filtro falló. Código: $0"
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
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "Publisher" "Su Empresa"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayVersion" "1.0.0.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoRepair" 1
SectionEnd
;--------------------------------
; Secciones Opcionales
Section "Filtro Fuente VLC" SecVLC
  SetOutPath "$INSTDIR\Filters"
  ; Instalar filtro fuente VLC
  File "Filters\VisioForge_VLC_Source.ax"
  File "Filters\libvlc.dll"
  File "Filters\libvlccore.dll"
  ; Instalar directorio plugins VLC
  SetOutPath "$INSTDIR\Filters\plugins"
  File /r "Filters\plugins\*.*"
  ; Registrar filtro fuente VLC
  DetailPrint "Registrando filtro fuente VLC..."
  ExecWait 'regsvr32 /s "$INSTDIR\Filters\VisioForge_VLC_Source.ax"'
SectionEnd
;--------------------------------
; Descripciones de Sección
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecMain} "Archivos de aplicación principales y filtro fuente FFMPEG (requerido)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecVLC} "Filtro fuente VLC para soporte de formato adicional (opcional)"
!insertmacro MUI_FUNCTION_DESCRIPTION_END
;--------------------------------
; Funciones del Instalador
Function .onInit
  ; Verificar si Windows 64-bit
  ${If} ${RunningX64}
    ; OK
  ${Else}
    MessageBox MB_OK|MB_ICONSTOP "Esta aplicación requiere Windows 64-bit."
    Abort
  ${EndIf}
  ; Verificar Visual C++ Redistributable 2015-2022
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64" "Installed"
  ${If} $0 != 1
    MessageBox MB_YESNO|MB_ICONQUESTION "Visual C++ 2015-2022 Redistributable (x64) es requerido.$\n$\n¿Descargar e instalar ahora?" IDYES download IDNO skip
    download:
      ExecShell "open" "https://aka.ms/vs/17/release/vc_redist.x64.exe"
      Abort
    skip:
  ${EndIf}
FunctionEnd
;--------------------------------
; Sección Desinstalador
Section "Uninstall"
  ; Desregistrar filtros
  DetailPrint "Desregistrando filtros DirectShow..."
  ExecWait 'regsvr32 /s /u "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"'
  ExecWait 'regsvr32 /s /u "$INSTDIR\Filters\VisioForge_VLC_Source.ax"'
  ; Remover archivos
  Delete "$INSTDIR\MyApp.exe"
  Delete "$INSTDIR\MyApp.exe.config"
  Delete "$INSTDIR\Uninstall.exe"
  ; Remover directorio Filters
  Delete "$INSTDIR\Filters\*.ax"
  Delete "$INSTDIR\Filters\*.dll"
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
#### Construyendo Instalador NSIS
```bash
# Compilar con NSIS
makensis.exe Installer.nsi
# O usar GUI del compilador NSIS
# Archivo > Cargar Script > Seleccionar Installer.nsi > Probar Instalador
```
---

### NSIS: Soporte de Instalación Silenciosa

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
### NSIS: Plugin Personalizado para Registro
Cree un plugin NSIS para más control.
#### FilterRegistration.cpp (Plugin NSIS)
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
    HMODULE hModule = LoadLibrary(filterPath);
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
; Cargar plugin
FilterRegistration::RegisterFilter "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"
Pop $0
${If} $0 == "ERROR"
    MessageBox MB_OK "Registro de filtro falló"
${EndIf}
```
---

## Integración InstallShield {: #integracion-installshield }

### Configuración Básica de Proyecto InstallShield

1. **Crear Nuevo Proyecto**:
   - Archivo > Nuevo Proyecto
   - Seleccionar "Proyecto Básico MSI"
   - Establecer nombre de proyecto y ubicación

2. **Agregar Archivos**:
   - Vista Archivos de Aplicación
   - Agregar archivos de filtro a `[INSTALLDIR]\Filters`
   - Agregar ejecutables de aplicación

3. **Agregar Acción Personalizada**:

#### Método 1: Usando regsvr32

1. Ir a **Comportamiento y Lógica** > **Acciones Personalizadas**
2. Clic derecho **Instalar** > **Nueva Acción Personalizada**
3. Establecer propiedades:
   - Nombre: `Registrar Filtros DirectShow`
   - Tipo: `Almacenado en la Tabla de Directorios`
   - Directorio de Trabajo: `[INSTALLDIR]Filters`
   - Nombre de Archivo: `regsvr32.exe`
   - Línea de Comando: `/s VisioForge_FFMPEG_Source_x64.ax`
   - Ejecutar: `Ejecución Diferida en Contexto de Sistema`
   - Condición: `NOT Installed`

4. Para desinstalar:
   - Nombre: `Desregistrar Filtros DirectShow`
   - Línea de Comando: `/s /u VisioForge_FFMPEG_Source_x64.ax`
   - Secuencia: Antes de **RemoveFiles**
   - Condición: `Installed`

#### Método 2: Usando DLL Personalizada

1. Crear DLL C++ con código de registro (similar al ejemplo WiX arriba)
2. Agregar DLL a **Archivos de Soporte** en InstallShield
3. Crear acción personalizada:
   - Tipo: `DLL de la instalación`
   - Nombre DLL: `CustomActions.dll`
   - Función: `RegisterDirectShowFilters`

### InstallShield: Configuración de Prerrequisitos

1. Ir a vista **Redistribuibles**
2. Agregar **Microsoft Visual C++ 2015-2022 Redistributable (x64)**:
   - Clic derecho > **Agregar Prerrequisito**
   - Examinar a `VC_redist.x64.exe`
   - Establecer: **Instalar Antes de Esta Aplicación**

---
## Ejemplos Inno Setup
### Script Básico Inno Setup
#### Setup.iss
```pascal
; Script de Configuración MyApp para Inno Setup 6.x
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
; Filtro Fuente FFMPEG
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
Name: "{group}\Desinstalar MyApp"; Filename: "{uninstallexe}"
[Run]
; Opcionalmente lanzar aplicación después de instalar
Filename: "{app}\MyApp.exe"; Description: "Lanzar MyApp"; Flags: nowait postinstall skipifsilent
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
    if MsgBox('Visual C++ 2015-2022 Redistributable (x64) es requerido.' + #13#10 +
              '¿Descargar e instalar ahora?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://aka.ms/vs/17/release/vc_redist.x64.exe', '', '', SW_SHOW, ewNoWait, ResultCode);
      Result := False;  // Abortar instalación
    end;
  end;
end;
```
#### Inno Setup Avanzado: Registro Personalizado
```pascal
[Files]
; No usar flag regserver - registraremos manualmente
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Code]
// Importar funciones API de Windows
function LoadLibrary(lpFileName: String): THandle;
  external 'LoadLibraryW@kernel32.dll stdcall';
function FreeLibrary(hModule: THandle): Boolean;
  external 'FreeLibrary@kernel32.dll stdcall';
function GetProcAddress(hModule: THandle; lpProcName: AnsiString): Longword;
  external 'GetProcAddress@kernel32.dll stdcall';
type
  TDllRegisterServer = function: HRESULT;
// Registrar filtro DirectShow
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
    Log('Error al cargar filtro: ' + FilterPath);
    Exit;
  end;
  try
    RegisterFunc := GetProcAddress(hModule, 'DllRegisterServer');
    if RegisterFunc = 0 then
    begin
      Log('DllRegisterServer no encontrado');
      Exit;
    end;
    @DllRegisterServer := Pointer(RegisterFunc);
    hr := DllRegisterServer();
    Result := Succeeded(hr);
    if Result then
      Log('Filtro registrado exitosamente')
    else
      Log('Registro de filtro falló: ' + IntToHex(hr, 8));
  finally
    FreeLibrary(hModule);
  end;
end;
// Llamado después de instalación
procedure CurStepChanged(CurStep: TSetupStep);
var
  FilterPath: String;
begin
  if CurStep = ssPostInstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    if not RegisterDirectShowFilter(FilterPath) then
    begin
      MsgBox('Advertencia: Registro de filtro DirectShow falló.' + #13#10 +
             'Puede necesitar registrarlo manualmente.', mbError, MB_OK);
    end;
  end;
end;
// Llamado antes de desinstalación
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
  FilterPath: String;
begin
  if CurUninstallStep = usUninstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    // Desregistrar usando regsvr32
    Exec('regsvr32.exe', '/s /u "' + FilterPath + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
```
---

## Instalación Silenciosa

### Parámetros de Instalación Silenciosa

#### MSI (WiX, InstallShield MSI)

```bash
# Instalar silenciosamente
msiexec /i MyApp.msi /quiet /norestart

# Instalar silenciosamente con log
msiexec /i MyApp.msi /quiet /norestart /l*v install.log

# Instalar silenciosamente con directorio de instalación personalizado
msiexec /i MyApp.msi /quiet INSTALLFOLDER="C:\RutaPersonalizada\MyApp"
```

#### NSIS

```bash
# Instalar silenciosamente
MyAppSetup.exe /S

# Instalar silenciosamente con directorio personalizado
MyAppSetup.exe /S /D=C:\RutaPersonalizada\MyApp

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
MyAppSetup.exe /SILENT /DIR="C:\RutaPersonalizada\MyApp"

# Desinstalar silenciosamente
unins000.exe /SILENT
```

---
## Empaquetado de Dependencias
### Visual C++ Redistributable
#### Opción 1: Bootstrapper de Descarga
```xml
<!-- WiX Bundle.wxs -->
<ExePackage Id="VCRedist2022"
            DownloadUrl="https://aka.ms/vs/17/release/vc_redist.x64.exe"
            InstallCommand="/install /quiet /norestart"
            DetectCondition="VCREDIST2022_X64" />
```
#### Opción 2: Incluir Redistributable
```nsis
; NSIS
Section "VC++ Redistributable"
  File "Prerequisites\VC_redist.x64.exe"
  ExecWait '"$INSTDIR\VC_redist.x64.exe" /install /quiet /norestart'
  Delete "$INSTDIR\VC_redist.x64.exe"
SectionEnd
```
#### Opción 3: Módulos de Mezcla (WiX)
```xml
<DirectoryRef Id="TARGETDIR">
  <Merge Id="VCRedist" SourceFile="$(var.VCRedistMergeModule)" DiskId="1" Language="0"/>
</DirectoryRef>
<Feature Id="VCRedist" Title="Runtime Visual C++" AllowAdvertise="no" Display="hidden" Level="1">
  <MergeRef Id="VCRedist"/>
</Feature>
```
---

## Mejores Prácticas

### Timing de Registro

1. **Secuencia de Instalación**:

   ```
   InstallFiles
   ↓
   Registrar Filtros (Acción Personalizada)
   ↓
   InstallFinalize
   ```

2. **Secuencia de Desinstalación**:

   ```
   Desregistrar Filtros (Acción Personalizada)
   ↓
   RemoveFiles
   ↓
   UninstallFinalize
   ```

### Manejo de Errores

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
        LogError("Clase no registrada - verificar dependencias");
    else if (hr == E_ACCESSDENIED)
        LogError("Acceso denegado - requiere privilegios admin");
    else
        LogError("Registro falló con HRESULT: 0x%08X", hr);
}
```

### Soporte de Reversión

Asegurar reversión apropiada si instalación falla:

```xml
<!-- Ejemplo de reversión WiX -->
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

### Privilegios de Admin

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

### Consideraciones de Arquitectura

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
  File "Filters\VisioForge_FFMPEG_Source_x86.ax"
${EndIf}
```

---
## Probando Instalación
### Lista de Verificación de Pruebas Manuales
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
### Pruebas Automatizadas
```powershell
# Script de prueba PowerShell
$installerPath = ".\MyAppSetup.msi"
$logPath = ".\install_test.log"
# Instalar silenciosamente
Start-Process msiexec.exe -ArgumentList "/i `"$installerPath`" /quiet /l*v `"$logPath`"" -Wait
# Verificar si filtro registrado
$filterCLSID = "{1974D893-83E4-4F89-9908-795C524CC17E}"
$regPath = "HKLM:\SOFTWARE\Classes\CLSID\$filterCLSID"
if (Test-Path $regPath) {
    Write-Host "Filtro registrado exitosamente" -ForegroundColor Green
} else {
    Write-Host "Registro de filtro falló" -ForegroundColor Red
    Exit 1
}
# Desinstalar
Start-Process msiexec.exe -ArgumentList "/x `"$installerPath`" /quiet" -Wait
# Verificar limpieza
if (Test-Path $regPath) {
    Write-Host "Filtro no desregistrado" -ForegroundColor Red
    Exit 1
} else {
    Write-Host "Desinstalación exitosa" -ForegroundColor Green
}
```
---

## Solución de Problemas

### Problemas Comunes

#### Registro Falla con Acceso Denegado

**Causa**: Privilegios insuficientes

**Solución**:

```xml
<!-- Asegurar ejecución diferida con contexto de sistema -->
<CustomAction Execute="deferred" Impersonate="no" />
```

#### Filtro Funciona en Desarrollo pero no Después de Instalar

**Causa**: Dependencias faltantes o rutas incorrectas

**Solución**:

- Usar Dependency Walker para verificar todas las dependencias DLL
- Asegurar todas las DLL en mismo directorio que filtro
- Verificar variable de entorno PATH

#### Instalación Silenciosa se Cuelga

**Causa**: Interacción de usuario requerida

**Solución**:

```bash
# Agregar parámetro /norestart
msiexec /i MyApp.msi /quiet /norestart
```

#### Desinstalación Deja Entradas de Registro

**Causa**: Acción personalizada de desregistro no ejecutándose

**Solución**:

```xml
<!-- Establecer Return="ignore" para desregistro -->
<CustomAction Return="ignore" />
```

---
## Ver También
### Documentación
- [Registro de Filtros](filter-registration.md) - Métodos de registro manual
- [Archivos Redistribuibles](redistributable-files.md) - Archivos a incluir en instalador
- [Resumen de Despliegue](index.md) - Guía completa de despliegue
### Recursos Externos
- [Documentación WiX Toolset](https://docs.firegiant.com/wix/)
- [Documentación NSIS](https://nsis.sourceforge.io/Docs/)
- [Documentación Inno Setup](https://jrsoftware.org/ishelp/)
- [Documentación Windows Installer (MSI)](https://learn.microsoft.com/en-us/windows/win32/Msi/windows-installer-portal)