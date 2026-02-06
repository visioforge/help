---
title: Guía de Registro de Filtros DirectShow
description: Registre filtros DirectShow de VisioForge usando regsvr32 manual, métodos programáticos y automatización de instaladores con consejos de solución de problemas.
---

# Guía de Registro de Filtros DirectShow

## Descripción General

Los filtros DirectShow deben estar registrados con Windows antes de poder usarse en aplicaciones. Esta guía cubre todos los métodos de registro para los filtros DirectShow de VisioForge.

---
## Métodos de Registro
### Método 1: Registro Automático (Instalador)
El método recomendado para usuarios finales es usar el instalador oficial.
**Instaladores Disponibles**:
- `visioforge_ffmpeg_source_filter_setup.exe` - Filtro Fuente FFMPEG
- `visioforge_vlc_source_filter_setup.exe` - Filtro Fuente VLC
- `visioforge_processing_filters_pack_setup.exe` - Paquete de Filtros de Procesamiento
- `visioforge_encoding_filters_pack_setup.exe` - Paquete de Filtros de Codificación
- `visioforge_virtual_camera_sdk_setup.exe` - SDK de Cámara Virtual
**Pasos de Instalación**:
1. Ejecute el instalador como Administrador
2. Siga el asistente de instalación
3. Los filtros se registran automáticamente
4. No se requieren pasos adicionales
---

### Método 2: Registro Manual (regsvr32)

Para desarrollo y pruebas, puede registrar manualmente los filtros usando la utilidad `regsvr32` de Windows.

#### Comando de Registro

```batch
# Abra el Símbolo del sistema como Administrador
# Clic derecho en Inicio → Símbolo del sistema (Admin)

# Registrar filtro x86 (32-bit)
regsvr32 "C:\Ruta\Al\Filtro.ax"

# Registrar filtro x64 (64-bit)
regsvr32 "C:\Ruta\Al\Filtro_x64.ax"

# Desregistrar filtro
regsvr32 /u "C:\Ruta\Al\Filtro.ax"
```

#### Ejemplos Específicos por SDK

**Filtro Fuente FFMPEG**:
```batch
# x86
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source.ax"

# x64
regsvr32 "C:\Program Files\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
```

**Filtro Fuente VLC**:
```batch
# Solo x86
regsvr32 "C:\Program Files (x86)\VisioForge\VLC Source\VisioForge_VLC_Source.ax"
```

**Paquete de Filtros de Procesamiento** (múltiples filtros):
```batch
# Efectos de Video
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Effects_Pro.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"

# Mezclador de Video
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Mixer.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Mixer_x64.ax"

# Mejorador de Audio
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Audio_Enhancer.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Audio_Enhancer_x64.ax"
```

**Paquete de Filtros de Codificación** (múltiples filtros):
```batch
# Codificador NVENC
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_NVENC.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_NVENC_x64.ax"

# Codificador H.264
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_H264_Encoder.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_H264_Encoder_x64.ax"

# Codificador AAC
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_AAC_Encoder.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_AAC_Encoder_x64.ax"

# Muxer MP4
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_MP4_Muxer.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_MP4_Muxer_x64.ax"
```

**SDK de Cámara Virtual**:
```batch
# Controlador de Cámara Virtual
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Virtual_Camera.ax"
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Virtual_Camera_x64.ax"

# Filtro Push Source
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Push_Video_Source.ax"
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Push_Video_Source_x64.ax"
```

---
### Método 3: Registro Programático (C++)
Registre filtros programáticamente desde el código de su aplicación.
#### Usando LoadLibrary y DllRegisterServer
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
        std::wcerr << L"Error al cargar filtro: " << filterPath << std::endl;
        std::wcerr << L"Código de error: " << error << std::endl;
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
        std::wcout << L"Filtro registrado exitosamente: " << filterPath << std::endl;
    }
    else
    {
        std::wcerr << L"Registro falló con HRESULT: " << std::hex << hr << std::endl;
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
// Uso
int main()
{
    const wchar_t* filterPath = L"C:\\Program Files\\VisioForge\\FFMPEG Source\\VisioForge_FFMPEG_Source_x64.ax";
    HRESULT hr = RegisterFilter(filterPath);
    if (SUCCEEDED(hr))
    {
        std::cout << "¡Filtro registrado exitosamente!" << std::endl;
    }
    else
    {
        std::cout << "Error al registrar filtro" << std::endl;
    }
    return 0;
}
```
#### Usando la Utilidad reg_special.exe
Los SDKs de VisioForge incluyen una utilidad `reg_special.exe` para registro simplificado:
```cpp
#include <windows.h>
#include <shellapi.h>
HRESULT RegisterWithUtility(const wchar_t* filterPath)
{
    // Construir línea de comando
    wchar_t cmdLine[MAX_PATH * 2];
    swprintf_s(cmdLine, L"reg_special.exe /regserver \"%s\"", filterPath);
    // Ejecutar utilidad de registro
    SHELLEXECUTEINFO sei = { sizeof(sei) };
    sei.lpVerb = L"runas";  // Ejecutar como administrador
    sei.lpFile = L"reg_special.exe";
    sei.lpParameters = cmdLine;
    sei.nShow = SW_HIDE;
    sei.fMask = SEE_MASK_NOCLOSEPROCESS;
    if (!ShellExecuteEx(&sei))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    // Esperar finalización
    WaitForSingleObject(sei.hProcess, INFINITE);
    DWORD exitCode;
    GetExitCodeProcess(sei.hProcess, &exitCode);
    CloseHandle(sei.hProcess);
    return (exitCode == 0) ? S_OK : E_FAIL;
}
```
---

### Método 4: Registro Programático (.NET/C#)

Registre filtros desde aplicaciones .NET usando P/Invoke.

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
                $"Error al cargar filtro: {filterPath}");
        }

        try
        {
            IntPtr procAddress = GetProcAddress(hModule, "DllRegisterServer");
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception("Función DllRegisterServer no encontrada");
            }

            DllRegisterServerDelegate registerServer =
                Marshal.GetDelegateForFunctionPointer<DllRegisterServerDelegate>(procAddress);

            int result = registerServer();

            if (result != 0)
            {
                throw new COMException($"Registro falló con HRESULT: 0x{result:X8}");
            }

            Console.WriteLine($"Filtro registrado exitosamente: {filterPath}");
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
                throw new Exception("Función DllUnregisterServer no encontrada");
            }

            DllUnregisterServerDelegate unregisterServer =
                Marshal.GetDelegateForFunctionPointer<DllUnregisterServerDelegate>(procAddress);

            int result = unregisterServer();

            if (result != 0)
            {
                throw new COMException($"Desregistro falló con HRESULT: 0x{result:X8}");
            }

            Console.WriteLine($"Filtro desregistrado exitosamente: {filterPath}");
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
                throw new Exception($"regsvr32 falló con código de salida: {process.ExitCode}");
            }
        }
    }
}

// Ejemplo de uso
class Program
{
    static void Main(string[] args)
    {
        string filterPath = @"C:\Program Files\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax";

        try
        {
            FilterRegistration.RegisterFilter(filterPath);
            Console.WriteLine("¡Filtro registrado exitosamente!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

---
## Verificación de Registro
### Método 1: Usando GraphEdit/GraphStudioNext
1. Inicie GraphEdit (Windows SDK) o GraphStudioNext
2. Clic en "Graph" → "Insert Filters"
3. Busque el nombre del filtro (ej., "FFMPEG Source", "VLC Source")
4. Si el filtro aparece en la lista, el registro fue exitoso
### Método 2: Usando el Editor de Registro
```batch
# Abrir Editor de Registro
regedit
# Navegar a:
HKEY_CLASSES_ROOT\CLSID\{GUID}
# Ejemplo para FFMPEG Source:
# HKEY_CLASSES_ROOT\CLSID\{1974D893-83E4-4F89-9908-795C524CC17E}
```
### Método 3: Verificación Programática (C++)
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
    // CLSID del Filtro Fuente FFMPEG
    CLSID ffmpegSourceClsid =
        { 0x1974D893, 0x83E4, 0x4F89, { 0x99, 0x08, 0x79, 0x5C, 0x52, 0x4C, 0xC1, 0x7E } };
    if (IsFilterRegistered(ffmpegSourceClsid))
    {
        std::cout << "El filtro FFMPEG Source está registrado" << std::endl;
    }
    else
    {
        std::cout << "El filtro FFMPEG Source NO está registrado" << std::endl;
    }
    CoUninitialize();
    return 0;
}
```
### Método 4: Verificación Programática (.NET/C#)
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
Guid ffmpegSourceClsid = new Guid("1974D893-83E4-4F89-9908-795C524CC17E");
if (IsFilterRegistered(ffmpegSourceClsid))
{
    Console.WriteLine("El filtro FFMPEG Source está registrado");
}
```
---

## Solución de Problemas

### Problema: "DllRegisterServer failed" o "Error 0x80004005"

**Causas**:
- No se está ejecutando como Administrador
- Dependencias faltantes (DLLs)
- Arquitectura incorrecta (x86 vs x64)

**Soluciones**:

1. **Ejecutar como Administrador**:
   ```batch
   # Clic derecho en Símbolo del sistema → Ejecutar como administrador
   regsvr32 "C:\Ruta\Al\Filtro.ax"
   ```

2. **Verificar Dependencias**:
   Use Dependency Walker o Dependencies.exe para verificar DLLs faltantes:
   ```batch
   # Descargue Dependencies de: https://github.com/lucasg/Dependencies
   Dependencies.exe "C:\Ruta\Al\Filtro.ax"
   ```

3. **Verificar Arquitectura**:
   ```batch
   # Para aplicación 32-bit, registre filtro 32-bit
   regsvr32 "C:\Ruta\Al\Filtro.ax"

   # Para aplicación 64-bit, registre filtro 64-bit
   regsvr32 "C:\Ruta\Al\Filtro_x64.ax"
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

1. **Coincidir Arquitectura de Aplicación**:
   - App 32-bit necesita filtro 32-bit
   - App 64-bit necesita filtro 64-bit

2. **Registro a Nivel de Sistema**:
   ```batch
   # Ejecute Símbolo del sistema como Administrador
   # Esto registra a nivel de sistema (HKEY_LOCAL_MACHINE)
   regsvr32 "C:\Ruta\Al\Filtro.ax"
   ```

3. **Verificar Ambos Registros**:
   - `HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID`
   - `HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID`

### Problema: Acceso Denegado

**Causa**: Permisos insuficientes.

**Solución**:
```batch
# Siempre ejecute como Administrador para registro de filtros
# Clic derecho en Símbolo del sistema → Ejecutar como administrador
```

### Problema: Registro exitoso pero el filtro no funciona

**Causas**:
- Clave de licencia faltante
- Dependencias de tiempo de ejecución faltantes
- Ruta de instalación incorrecta

**Soluciones**:

1. **Verificar Licencia**:
   - Verifique si la licencia de prueba ha expirado
   - Asegúrese de que la clave de licencia esté correctamente activada

2. **Verificar Dependencias de Runtime**:
   - FFMPEG Source: Requiere DLLs de FFmpeg (avcodec, avformat, etc.)
   - VLC Source: Requiere bibliotecas VLC (libvlc.dll, libvlccore.dll, plugins/)
   - NVENC: Requiere GPU NVIDIA y controladores
   - Processing/Encoding: Puede requerir Visual C++ Redistributables

3. **Verificar Ubicaciones de Archivos**:
   Todas las DLLs dependientes deben estar en el mismo directorio que el archivo .ax o en el PATH del sistema.

---
## COM Sin Registro (Avanzado)
Para despliegue xcopy sin registro, use COM sin registro con archivos manifest.
### Creando Archivo Manifest
**filter.manifest** (colocar junto al archivo .ax):
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
**application.exe.manifest** (colocar junto a su .exe):
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
- Puede no funcionar con todos los filtros DirectShow
- Los filtros registrados en el sistema tienen precedencia
---

## Scripts de Registro por Lotes

### Registrar Todos los Filtros (Script Batch)

```batch
@echo off
echo Registrando Filtros DirectShow de VisioForge...
echo.

REM Verificar privilegios de Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: ¡Este script debe ejecutarse como Administrador!
    pause
    exit /b 1
)

REM Establecer ruta de instalación
set INSTALL_PATH=C:\Program Files\VisioForge

REM Registrar FFMPEG Source
echo Registrando FFMPEG Source...
regsvr32 /s "%INSTALL_PATH%\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% equ 0 (
    echo   [OK] FFMPEG Source registrado
) else (
    echo   [FALLÓ] Registro de FFMPEG Source falló
)

REM Registrar VLC Source
echo Registrando VLC Source...
regsvr32 /s "%INSTALL_PATH%\VLC Source\VisioForge_VLC_Source.ax"
if %errorLevel% equ 0 (
    echo   [OK] VLC Source registrado
) else (
    echo   [FALLÓ] Registro de VLC Source falló
)

REM Registrar Filtros de Procesamiento
echo Registrando Filtros de Procesamiento...
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Mixer_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Audio_Enhancer_x64.ax"
echo   [OK] Filtros de Procesamiento registrados

REM Registrar Filtros de Codificación
echo Registrando Filtros de Codificación...
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_NVENC_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_H264_Encoder_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_AAC_Encoder_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_MP4_Muxer_x64.ax"
echo   [OK] Filtros de Codificación registrados

echo.
echo ¡Registro completo!
pause
```

### Desregistrar Todos los Filtros

```batch
@echo off
echo Desregistrando Filtros DirectShow de VisioForge...
echo.

REM Verificar privilegios de Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: ¡Este script debe ejecutarse como Administrador!
    pause
    exit /b 1
)

set INSTALL_PATH=C:\Program Files\VisioForge

REM Desregistrar todos los filtros
regsvr32 /s /u "%INSTALL_PATH%\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\VLC Source\VisioForge_VLC_Source.ax"
regsvr32 /s /u "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Mixer_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\Encoding Filters\VisioForge_NVENC_x64.ax"

echo ¡Desregistro completo!
pause
```

---
## Ver También
- [Archivos Redistributables](redistributable-files.md) - Lista completa de archivos para cada SDK
- [Integración con Instaladores](installer-integration.md) - Creando instaladores personalizados
- [Descripción General de Despliegue](index.md) - Guía principal de despliegue