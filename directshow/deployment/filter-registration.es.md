---
title: Registrar filtros DirectShow con regsvr32, C++ y C# .NET
description: Registre filtros DirectShow de VisioForge usando regsvr32 manual, métodos programáticos y automatización de instaladores con consejos de solución de problemas.
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

# Guía de registro de filtros DirectShow

## Descripción general

Los filtros DirectShow deben estar registrados con Windows antes de poder usarse en aplicaciones. Esta guía cubre todos los métodos de registro para los filtros DirectShow de VisioForge.

---
## Métodos de registro
### Método 1: Registro automático (instalador)
El método recomendado para usuarios finales es usar el instalador oficial.

**Instaladores disponibles**:

- `visioforge_ffmpeg_source_filter_setup.exe` - FFMPEG Source DirectShow Filter
- `visioforge_vlc_source_sdk_setup_x86.exe`, `visioforge_vlc_source_sdk_setup_x64.exe` - VLC Source DirectShow Filter
- `visioforge_processing_filters_pack_setup.exe` - Processing Filters Pack
- `visioforge_encoding_filters_pack_setup.exe` - Encoding Filters Pack
- `visioforge_virtual_camera_sdk_setup.exe` - Virtual Camera SDK

**Pasos de instalación**:

1. Ejecute el instalador como administrador
2. Siga el asistente de instalación
3. Los filtros se registran automáticamente
4. No se requieren pasos adicionales

---

### Método 2: Registro manual (regsvr32)

Para desarrollo y pruebas, puede registrar manualmente los filtros usando la utilidad `regsvr32` de Windows.

#### Comando de registro

```batch
REM Abra el Símbolo del sistema como administrador
REM Clic derecho en Inicio → Símbolo del sistema (Admin)

REM Registrar filtro x86 (32-bit)
regsvr32 "C:\Path\To\Filter.ax"

REM Registrar filtro x64 (64-bit)
regsvr32 "C:\Path\To\Filter_x64.ax"

REM Desregistrar filtro
regsvr32 /u "C:\Path\To\Filter.ax"
```

#### Ejemplos específicos por SDK

**FFMPEG Source DirectShow Filter**:
```batch
REM x86
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist\x86\VisioForge_FFMPEG_Source.ax"

REM x64
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist\x64\VisioForge_FFMPEG_Source_x64.ax"
```

**VLC Source DirectShow Filter**:
```batch
REM x86
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge VLC Source DirectShow SDK\Redist\x86\VisioForge_VLC_Source.ax"

REM x64
regsvr32 "C:\Program Files\VisioForge\VisioForge VLC Source DirectShow SDK x64\Redist\x64\VisioForge_VLC_Source_x64.ax"
```

**Processing Filters Pack** (múltiples filtros):
```batch
REM Efectos de video
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Effects_Pro.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Effects_Pro_x64.ax"

REM Mezclador de video
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Mixer.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Video_Mixer_x64.ax"

REM Mejorador de audio
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Audio_Enhancer.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Processing Filters Pack\Redist\Filters\VisioForge_Audio_Enhancer_x64.ax"
```

**Encoding Filters Pack** (múltiples filtros):
```batch
REM Codificador NVENC
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_NVENC.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_NVENC_x64.ax"

REM Codificador H.264
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_H264_Encoder.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_H264_Encoder_x64.ax"

REM Codificador AAC
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_AAC_Encoder.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_AAC_Encoder_x64.ax"

REM Muxer MP4
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_MP4_Muxer.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Encoding Filters Pack\Redist\Filters\VisioForge_MP4_Muxer_x64.ax"
```

**Virtual Camera SDK**:
```batch
REM Controlador de cámara virtual
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Virtual_Camera.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Virtual_Camera_x64.ax"

REM Filtro Push Source
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Push_Video_Source.ax"
regsvr32 "C:\Program Files\VisioForge\VisioForge Virtual Camera SDK\Redist\VisioForge_Push_Video_Source_x64.ax"
```

---
### Método 3: Registro programático (C++)
Registre filtros programáticamente desde el código de su aplicación.
#### Usando LoadLibraryEx y DllRegisterServer
```cpp
#include <windows.h>
#include <iostream>
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
HRESULT RegisterFilter(const wchar_t* filterPath)
{
    // LOAD_WITH_ALTERED_SEARCH_PATH: las DLL propias del filtro (FFmpeg, VLC) se cargan desde la carpeta del filtro
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
    // LOAD_WITH_ALTERED_SEARCH_PATH: las DLL propias del filtro (FFmpeg, VLC) se cargan desde la carpeta del filtro
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
// Uso
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
#### Usando la utilidad reg_special.exe
Varios setups de filtros de VisioForge (VLC Source, Virtual Camera SDK, Video Encryption SDK, el Encoding Filters Pack y el Processing Filters Pack) incluyen una utilidad `reg_special.exe` junto a sus filtros. Registra todos los archivos `.ax` y `.ocx` de su propia carpeta:
```cpp
#include <windows.h>
#include <shellapi.h>
// reg_special.exe registra todos los archivos .ax y .ocx de su propia carpeta; -u los desregistra.
// Siempre termina con 0: el resultado de cada archivo se escribe en reg_special.log de esa carpeta.
HRESULT RunRegSpecial(const wchar_t* regSpecialPath, bool unregister)
{
    // Ejecutar utilidad de registro
    SHELLEXECUTEINFO sei = { sizeof(sei) };
    sei.lpVerb = L"runas";  // Ejecutar como administrador
    sei.lpFile = regSpecialPath;  // ruta completa a reg_special.exe en la carpeta de los filtros
    sei.lpParameters = unregister ? L"-u" : nullptr;
    sei.nShow = SW_HIDE;
    sei.fMask = SEE_MASK_NOCLOSEPROCESS;
    if (!ShellExecuteEx(&sei))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    // Esperar finalización
    WaitForSingleObject(sei.hProcess, INFINITE);
    CloseHandle(sei.hProcess);
    return S_OK;
}
```
---

### Método 4: Registro programático (.NET/C#)

Registre filtros desde aplicaciones .NET usando P/Invoke.

```csharp
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

public class FilterRegistration
{
    // LOAD_WITH_ALTERED_SEARCH_PATH: las DLL propias del filtro (FFmpeg, VLC) se cargan desde la carpeta del filtro
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

    // Alternativa: Usar Process.Start con regsvr32
    public static void RegisterFilterWithRegsvr32(string filterPath)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "regsvr32.exe",
            Arguments = $"/s \"{filterPath}\"",  // /s = silencioso
            Verb = "runas",  // Ejecutar como administrador
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

// Ejemplo de uso
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
## Verificación de registro
### Método 1: Usando GraphEdit/GraphStudioNext

1. Inicie GraphEdit (Windows SDK) o GraphStudioNext
2. Clic en "Graph" → "Insert Filters"
3. Busque el nombre del filtro (ej., "FFMPEG Source", "VLC Source")
4. Si el filtro aparece en la lista, el registro fue exitoso
### Método 2: Usando el Editor del Registro
```batch
REM Abrir Editor del Registro
regedit
REM Navegar a:
HKEY_CLASSES_ROOT\CLSID\{GUID}
REM Ejemplo para FFMPEG Source:
REM HKEY_CLASSES_ROOT\CLSID\{C5255DE3-50A7-4714-B763-D99E96E4CD52}
```
### Método 3: Verificación programática (C++)
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
// Uso
int main()
{
    CoInitialize(NULL);
    // CLSID del filtro FFMPEG Source
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
### Método 4: Verificación programática (.NET/C#)
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
// Uso
Guid ffmpegSourceClsid = new Guid("C5255DE3-50A7-4714-B763-D99E96E4CD52");
if (IsFilterRegistered(ffmpegSourceClsid))
{
    Console.WriteLine("FFMPEG Source filter is registered");
}
```
---

## Solución de problemas

### Problema: "DllRegisterServer failed" o "Error 0x80004005"

**Causas**:

- No se está ejecutando como administrador
- Dependencias faltantes (DLLs)
- Arquitectura incorrecta (x86 vs x64)

**Soluciones**:

1. **Ejecutar como administrador**:
   ```batch
   # Clic derecho en Símbolo del sistema → Ejecutar como administrador
   regsvr32 "C:\Path\To\Filter.ax"
   ```

2. **Verificar dependencias**:
   Use Dependency Walker o Dependencies.exe para verificar DLLs faltantes:
   ```batch
   # Descargue Dependencies de: https://github.com/lucasg/Dependencies
   Dependencies.exe "C:\Path\To\Filter.ax"
   ```

3. **Verificar arquitectura**:
   ```batch
   # Para aplicación 32-bit, registre filtro 32-bit
   regsvr32 "C:\Path\To\Filter.ax"

   # Para aplicación 64-bit, registre filtro 64-bit
   regsvr32 "C:\Path\To\Filter_x64.ax"
   ```

### Problema: "The module was loaded but the entry-point was not found"

**Causa**: El archivo no es un filtro DirectShow válido o está corrupto.

**Soluciones**:

- Verifique la integridad del archivo
- Vuelva a descargar o reinstale el SDK
- Verifique que el archivo sea un filtro DirectShow (extensión .ax)

### Problema: Filtro registrado pero no encontrado en aplicaciones

**Causas**:

- Desajuste 32-bit/64-bit
- Filtro registrado en HKEY incorrecto (por usuario vs todo el sistema)

**Soluciones**:

1. **Coincidir arquitectura de aplicación**:
   - App 32-bit necesita filtro 32-bit
   - App 64-bit necesita filtro 64-bit

2. **Registro a nivel de sistema**:
   ```batch
   # Ejecute Símbolo del sistema como administrador
   # Esto registra a nivel de sistema (HKEY_LOCAL_MACHINE)
   regsvr32 "C:\Path\To\Filter.ax"
   ```

3. **Verificar ambos registros**:
   - `HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID`
   - `HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID`

### Problema: Acceso denegado

**Causa**: Permisos insuficientes.

**Solución**:
```batch
REM Siempre ejecute como administrador para registro de filtros
REM Clic derecho en Símbolo del sistema → Ejecutar como administrador
```

### Problema: Registro exitoso pero el filtro no funciona

**Causas**:

- Clave de licencia faltante
- Dependencias de tiempo de ejecución faltantes
- Ruta de instalación incorrecta

**Soluciones**:

1. **Verificar licencia**:
   - Verifique si la licencia de prueba ha expirado
   - Asegúrese de que la clave de licencia esté correctamente activada

2. **Verificar dependencias de runtime**:
   - FFMPEG Source: Requiere DLLs de FFmpeg (avcodec, avformat, etc.)
   - VLC Source: Requiere bibliotecas VLC (libvlc.dll, libvlccore.dll, plugins/)
   - NVENC: Requiere GPU NVIDIA y controladores
   - Processing/Encoding: Puede requerir Visual C++ Redistributables

3. **Verificar ubicaciones de archivos**:
   Todas las DLLs dependientes deben estar en el mismo directorio que el archivo .ax o en el PATH del sistema.

---
## COM sin registro (avanzado)
Para despliegue xcopy sin registro, use COM sin registro con archivos manifest.
### Creando archivo manifest
**VisioForge.FFMPEGSource.manifest** (el nombre del archivo debe coincidir con el nombre del ensamblado; colóquelo en la carpeta de la aplicación, junto al archivo .ax, sus DLL y `cacert.pem`):
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
Para una aplicación de 32 bits, indique el filtro x86 en el elemento `file`: `VisioForge_FFMPEG_Source.ax`, con las DLL y el `cacert.pem` de la carpeta `x86` junto a él.

**Manifiesto de la aplicación**: añada el elemento `dependency` siguiente al manifiesto incrustado en su .exe.

- .NET: añádalo al `app.manifest` del proyecto (Agregar > Nuevo elemento > Archivo de manifiesto de aplicación) y conserve sus secciones `trustInfo` y `compatibility`. El archivo de proyecto debe hacer referencia a él: `<ApplicationManifest>app.manifest</ApplicationManifest>` (Visual Studio agrega esta línea).
- C++: póngalo en un archivo de manifiesto propio e indique ese archivo en Herramienta de manifiesto > Entrada y salida > Archivos de manifiesto adicionales.
- Un archivo `YourApp.exe.manifest` junto al .exe solo se usa si el .exe no tiene un manifiesto incrustado.

El manifiesto mínimo siguiente muestra el elemento:
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
**Limitaciones**:

- Más complejo de configurar
- Requiere archivos manifest
- La construcción automática del grafo (`RenderFile`, Intelligent Connect) no encuentra los filtros: el filter mapper solo lista los filtros registrados. Créelos con `CoCreateInstance` y conecte usted mismo los pines (`IGraphBuilder::AddFilter`, `ConnectDirect`)
- Solo su propio proceso ve los filtros: un filtro que otras aplicaciones deben encontrar, como la Virtual Camera o la Virtual Audio Card, necesita registro (`regsvr32`)

---

## Scripts de registro por lotes

### Registrar todos los filtros (script batch)

```batch
@echo off
setlocal
echo Registering VisioForge DirectShow Filters...
echo.

REM Verificar privilegios de administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

REM Carpeta donde el setup de FFMPEG Source instala ambos filtros
set INSTALL_PATH=C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist
set FAILED=
set FOUND=

REM Registrar FFMPEG Source (x64 y x86)
echo Registering FFMPEG Source...
call :register "%INSTALL_PATH%\x64\VisioForge_FFMPEG_Source_x64.ax"
call :register "%INSTALL_PATH%\x86\VisioForge_FFMPEG_Source.ax"

REM Registre del mismo modo los demás filtros que distribuye su aplicación, desde sus propias carpetas:
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

REM Registrar un filtro e informar del resultado
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

### Desregistrar todos los filtros

```batch
@echo off
setlocal
echo Unregistering VisioForge DirectShow Filters...
echo.

REM Verificar privilegios de administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

set INSTALL_PATH=C:\Program Files (x86)\VisioForge\FFMPEG Source DirectShow filter\Redist
set FAILED=
set FOUND=

REM Desregistrar todos los filtros
call :unregister "%INSTALL_PATH%\x64\VisioForge_FFMPEG_Source_x64.ax"
call :unregister "%INSTALL_PATH%\x86\VisioForge_FFMPEG_Source.ax"

REM Desregistre del mismo modo los demás filtros que distribuye su aplicación:
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

REM Desregistrar un filtro e informar del resultado
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
## Ver también

- [Archivos redistribuibles](redistributable-files.md) - Lista completa de archivos para cada SDK
- [Integración con instaladores](installer-integration.md) - Creando instaladores personalizados
- [Descripción general de despliegue](index.md) - Guía principal de despliegue