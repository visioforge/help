---
title: IFFmpegSourceSettings - API filtro FFMPEG DirectShow
description: Interfaz IFFmpegSourceSettings con aceleración por hardware, modos de buffering, opciones FFmpeg personalizadas y callbacks para DirectShow.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Decoding
  - RTSP
  - C#
primary_api_classes:
  - IFFmpegSourceSettings
  - IFileSourceFilter
  - IFFMPEGSourceSettings

---

# Referencia de Interfaz IFFMPEGSourceSettings

## Descripción General

La interfaz `IFFMPEGSourceSettings` proporciona opciones de configuración avanzadas para el filtro DirectShow FFMPEG Source. Esta interfaz permite a los desarrolladores controlar la aceleración por hardware, el comportamiento del buffering, opciones FFmpeg personalizadas y varios callbacks para la reproducción de medios.

> Los headers .NET / C++ / Delphi escriben esta interfaz como `IFFMPEGSourceSettings` (todo en mayúsculas `FFMPEG`). Las rutas heredadas de archivos `IFFmpegSourceSettings.h` / `.cs` mantienen el nombre histórico de archivo en mayúsculas/minúsculas mixtas — pero el símbolo expuesto por los tres lenguajes es la forma toda en mayúsculas. Los dos son identificadores equivalentes en C#; esta página usa la forma toda en mayúsculas de manera consistente con examples.md / index.md.

## Definición de Interfaz

- **Nombre de Interfaz**: `IFFMPEGSourceSettings`
- **CLSID del Filtro al que consulta**: `{1974D893-83E4-4F89-9908-795C524CC17E}` (el filtro FFMPEG Source — el IID de la interfaz en sí es distinto; consulte el header).
- **Hereda De**: `IUnknown`

### Archivos de Definición de Interfaz

Las definiciones completas de interfaz están disponibles en GitHub:

- **C# (.NET)**: [IFFmpegSourceSettings.cs](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/dotnet/IFFmpegSourceSettings.cs)
- **Header C++**: [IFFmpegSourceSettings.h](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/cpp/FFMPEG%20Source/IFFmpegSourceSettings.h)
- **Delphi**: [VCFiltersAPI.pas](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/delphi/VCFiltersAPI.pas) (buscar `IFFMPEGSourceSettings`)

Todas las definiciones de interfaz incluyen:

- Firmas de métodos completas con atributos de marshalling apropiados
- Definiciones de delegados de callback
- Tipos de enumeración (modos de buffering, tipos de medios)
- Documentación de uso y ejemplos

## Referencia de Métodos

### Aceleración por Hardware

#### GetHWAccelerationEnabled

Obtiene el estado actual de aceleración por hardware.

**Sintaxis (C++)**:

```cpp
BOOL GetHWAccelerationEnabled();
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
bool GetHWAccelerationEnabled();
```

**Retorna**: `TRUE` si la aceleración por hardware está habilitada, `FALSE` en caso contrario.

**Predeterminado**: `TRUE`

---
#### SetHWAccelerationEnabled
Habilita o deshabilita la aceleración de decodificación de video por hardware.
**Sintaxis (C++)**:
```cpp
HRESULT SetHWAccelerationEnabled(BOOL enabled);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetHWAccelerationEnabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
```
**Parámetros**:
- `enabled`: Establezca a `TRUE` para habilitar aceleración por hardware, `FALSE` para deshabilitar.
**Retorna**: `S_OK` (0) en éxito, código de error en caso contrario.
**Notas de Uso**:
- Debe llamarse **antes** de conectar filtros de video downstream
- Cuando está habilitado, el filtro intenta usar decodificación por hardware (DXVA, NVDEC, QuickSync, etc.)
- Retrocede a decodificación por software si la aceleración por hardware no está disponible
- La aceleración por hardware mejora significativamente el rendimiento para códecs H.264, H.265, VP9 y AV1
**Ejemplo (C++)**:
```cpp
IFFmpegSourceSettings* pSettings = nullptr;
pFilter->QueryInterface(IID_IFFmpegSourceSettings, (void**)&pSettings);
// Habilitar aceleración por hardware
pSettings->SetHWAccelerationEnabled(TRUE);
pSettings->Release();
```
**Ejemplo (C#)**:
```csharp
var settings = filter as IFFmpegSourceSettings;
if (settings != null)
{
    // Habilitar aceleración por hardware
    settings.SetHWAccelerationEnabled(true);
}
```
---

### Configuración de Timeout de Carga

#### GetLoadTimeOut

Obtiene el valor actual del timeout de carga de fuente.

**Sintaxis (C++)**:

```cpp
DWORD GetLoadTimeOut();
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
uint GetLoadTimeOut();
```

**Retorna**: Valor de timeout en milisegundos.

**Predeterminado**: `15000` (15 segundos)

---
#### SetLoadTimeOut
Establece la duración del timeout para operaciones de carga de fuente.
**Sintaxis (C++)**:
```cpp
HRESULT SetLoadTimeOut(DWORD milliseconds);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetLoadTimeOut(uint milliseconds);
```
**Parámetros**:
- `milliseconds`: Duración del timeout en milisegundos.
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Debe llamarse **antes** de cargar el archivo/URL fuente
- Particularmente importante para streams de red que pueden tener tiempos de conexión lentos
- Establezca valores más altos para conexiones de red lentas o archivos grandes
- Establezca valores más bajos para fallar rápidamente en fuentes inalcanzables
**Ejemplo (C++)**:
```cpp
// Establecer timeout de 30 segundos para streams de red
pSettings->SetLoadTimeOut(30000);
// Cargar stream RTSP
IFileSourceFilter* pFileSource = nullptr;
pFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
pFileSource->Load(L"rtsp://example.com/stream", nullptr);
```
---

### Configuración de Buffering

#### GetBufferingMode

Obtiene el modo de buffering actual.

**Sintaxis (C++)**:

```cpp
FFMPEG_SOURCE_BUFFERING_MODE GetBufferingMode();
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
FFMPEG_SOURCE_BUFFERING_MODE GetBufferingMode();
```

**Retorna**: Modo de buffering actual (ver enumeración abajo).

**Predeterminado**: `FFMPEG_SOURCE_BUFFERING_MODE_AUTO`

---
#### SetBufferingMode
Establece el modo de buffering para fuentes en vivo.
**Sintaxis (C++)**:
```cpp
HRESULT SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE mode);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE mode);
```
**Parámetros**:
- `mode`: Modo de buffering a usar.
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Debe llamarse **antes** de cargar la fuente
- Afecta latencia y estabilidad para streams en vivo
**Modos de Buffering**:
| Modo | Valor | Descripción | Caso de Uso |
|------|-------|-------------|-------------|
| `FFMPEG_SOURCE_BUFFERING_MODE_AUTO` | 0 | Detectar automáticamente si se necesita buffering | Predeterminado - recomendado para la mayoría de escenarios |
| `FFMPEG_SOURCE_BUFFERING_MODE_ON` | 1 | Forzar buffering habilitado | Usar para streams de red inestables |
| `FFMPEG_SOURCE_BUFFERING_MODE_OFF` | 2 | Forzar buffering deshabilitado | Usar para streams en vivo de baja latencia |
**Ejemplo (C++)**:
```cpp
// Deshabilitar buffering para stream RTSP de baja latencia
pSettings->SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE_OFF);
pSettings->SetLoadTimeOut(5000); // timeout de 5 segundos
```
**Ejemplo (C#)**:
```csharp
// Habilitar buffering para red inestable
settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.ON);
```
---

### Opciones FFmpeg Personalizadas

#### SetCustomOption

Establece una opción FFmpeg personalizada para el demuxer o decodificador.

**Sintaxis (C++)**:

```cpp
HRESULT SetCustomOption(LPSTR name, LPSTR value);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int SetCustomOption([MarshalAs(UnmanagedType.LPStr)] string name,
                     [MarshalAs(UnmanagedType.LPStr)] string value);
```

**Parámetros**:

- `name`: Nombre de la opción (cadena ASCII).
- `value`: Valor de la opción (cadena ASCII).

**Retorna**: `S_OK` (0) en éxito.

**Notas de Uso**:

- Debe llamarse **antes** de cargar la fuente
- Permite pasar cualquier opción AVFormatContext o AVCodecContext de FFmpeg
- Las opciones se pasan directamente a las bibliotecas FFmpeg
- Las opciones inválidas se ignoran con una advertencia

**Opciones Comunes**:

| Opción | Valor | Descripción |
|--------|-------|-------------|
| `rtsp_transport` | `tcp` o `udp` | Forzar protocolo de transporte RTSP |
| `timeout` | Microsegundos | Timeout de red para protocolos |
| `buffer_size` | Bytes | Tamaño de buffer de entrada |
| `analyzeduration` | Microsegundos | Duración para analizar stream |
| `probesize` | Bytes | Tamaño de datos a sondear |
| `fflags` | `nobuffer` | Deshabilitar buffering |
| `threads` | Número | Cantidad de hilos del decodificador |

**Ejemplo (C++)**:

```cpp
// Configurar RTSP para usar transporte TCP
pSettings->SetCustomOption("rtsp_transport", "tcp");

// Establecer timeout de red a 5 segundos
pSettings->SetCustomOption("timeout", "5000000"); // 5 segundos en microsegundos

// Aumentar tamaño de sondeo para mejor detección de formato
pSettings->SetCustomOption("probesize", "10000000"); // 10MB
```

**Ejemplo (C#)**:

```csharp
// Configuración de baja latencia
settings.SetCustomOption("fflags", "nobuffer");
settings.SetCustomOption("flags", "low_delay");
settings.SetCustomOption("probesize", "32");
```

---
#### ClearCustomOptions
Limpia todas las opciones personalizadas establecidas previamente.
**Sintaxis (C++)**:
```cpp
HRESULT ClearCustomOptions();
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int ClearCustomOptions();
```
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Debe llamarse **antes** de cargar la fuente
- Restablece todas las opciones personalizadas a los valores predeterminados de FFmpeg
**Ejemplo (C++)**:
```cpp
pSettings->ClearCustomOptions();
```
---

### Configuración de Callbacks

#### SetDataCallback

Establece una función de callback para recibir datos de video/audio decodificados.

**Sintaxis (C++)**:

```cpp
HRESULT SetDataCallback(FFMPEGDataCallbackDelegate callback);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int SetDataCallback([MarshalAs(UnmanagedType.FunctionPtr)] FFMPEGDataCallbackDelegate callback);
```

**Parámetros**:

- `callback`: Puntero a función de callback.

**Retorna**: `S_OK` (0) en éxito.

**Firma del Callback (C++)**:

```cpp
typedef HRESULT(_stdcall* FFMPEGDataCallbackDelegate) (
    BYTE* buffer,        // Puntero al buffer de datos
    int bufferLen,       // Longitud del buffer en bytes
    int dataType,        // 0 = video, 1 = audio
    LONGLONG startTime,  // Marca de tiempo de inicio (unidades de 100 nanosegundos)
    LONGLONG stopTime    // Marca de tiempo de fin (unidades de 100 nanosegundos)
);
```

**Notas de Uso**:

- El callback se invoca para cada frame/muestra de audio decodificado
- Se llama desde el hilo de streaming del filtro - mantener el procesamiento mínimo
- Los datos del buffer son válidos solo durante la ejecución del callback
- Retornar `S_OK` del callback para continuar el procesamiento

**Ejemplo (C++)**:

```cpp
HRESULT __stdcall DataCallback(BYTE* buffer, int bufferLen, int dataType,
                                LONGLONG startTime, LONGLONG stopTime)
{
    if (dataType == 0) // Video
    {
        // Procesar frame de video
        ProcessVideoFrame(buffer, bufferLen, startTime);
    }
    else // Audio
    {
        // Procesar datos de audio
        ProcessAudioData(buffer, bufferLen);
    }
    return S_OK;
}

// Establecer callback
pSettings->SetDataCallback(&DataCallback);
```

---
#### SetTimestampCallback
Establece una función de callback para recibir información de marcas de tiempo.
**Sintaxis (C++)**:
```cpp
HRESULT SetTimestampCallback(FFMPEGTimestampCallbackDelegate callback);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetTimestampCallback([MarshalAs(UnmanagedType.FunctionPtr)] FFMPEGTimestampCallbackDelegate callback);
```
**Parámetros**:
- `callback`: Puntero a función de callback.
**Retorna**: `S_OK` (0) en éxito.
**Firma del Callback (C++)**:
```cpp
typedef HRESULT(_stdcall* FFMPEGTimestampCallbackDelegate) (
    int mediaType,              // 0 = video, 1 = audio
    __int64 demuxerStartTime,   // Tiempo de inicio del demuxer
    __int64 streamStartTime,    // Tiempo de inicio del stream
    __int64 timestamp           // Marca de tiempo actual
);
```
**Notas de Uso**:
- Útil para análisis de marcas de tiempo y depuración de sincronización
- Se llama para cada frame/muestra decodificada
---

### Control de Audio
#### SetAudioEnabled
Habilita o deshabilita el procesamiento de stream de audio.
**Sintaxis (C++)**:
```cpp
HRESULT SetAudioEnabled(BOOL enabled);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetAudioEnabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
```
**Parámetros**:
- `enabled`: Establezca a `TRUE` para habilitar audio, `FALSE` para deshabilitar.
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Debe llamarse **antes** de cargar la fuente
- Cuando está deshabilitado, los streams de audio no se decodifican (ahorra CPU/memoria)
- Útil para aplicaciones solo video

**Ejemplo (C++)**:

```cpp
// Deshabilitar audio para procesamiento solo de video
pSettings->SetAudioEnabled(FALSE);
```

## Interfaces Relacionadas
- **IFileSourceFilter** - Interfaz DirectShow estándar para cargar archivos/URLs
- **IAMStreamSelect** - Seleccionar entre múltiples streams de audio/video
- **IMediaSeeking** - Buscar posiciones específicas en los medios
- **IAMStreamConfig** - Configurar formato de video/audio
## Ver También
### Documentación
- [Descripción del Filtro FFMPEG Source](index.md) - Descripción del producto y características
- [Ejemplos de Código](examples.md) - Muestras de código funcionales completas
### Definiciones de Interfaz
- [Interfaz C# (.NET)](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/dotnet/IFFmpegSourceSettings.cs) - Definición completa de interfaz .NET
- [Header de Interfaz C++](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/cpp/FFMPEG%20Source/IFFmpegSourceSettings.h) - Archivo header C++
- [Interfaz Delphi](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/delphi/VCFiltersAPI.pas) - Definición de interfaz Delphi
### Muestras Funcionales
- [Repositorio de Muestras GitHub](https://github.com/visioforge/directshow-samples) - Ejemplos funcionales completos para todas las plataformas
### Recursos Externos
- [Documentación FFmpeg](https://ffmpeg.org/documentation.html) - Documentación de bibliotecas FFmpeg
- [SDK DirectShow](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow) - Documentación Microsoft DirectShow