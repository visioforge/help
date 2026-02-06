---
title: Filtro Fuente VLC - Referencia de Interfaces
description: Interfaces de la familia IVlcSrc para audio multi-pista, soporte de subtítulos y opciones personalizadas de línea de comandos VLC en aplicaciones DirectShow.
---

# Referencia de Interfaces del Filtro Fuente VLC

## Descripción General

El filtro DirectShow de Fuente VLC expone tres interfaces progresivas (`IVlcSrc`, `IVlcSrc2`, `IVlcSrc3`) que proporcionan control integral sobre la reproducción de medios, selección de pistas de audio/subtítulos y configuración de VLC. Estas interfaces permiten a los desarrolladores aprovechar el potente framework de medios de VLC dentro de aplicaciones DirectShow.

## Jerarquía de Interfaces

```
IUnknown
  └── IVlcSrc
        └── IVlcSrc2
              └── IVlcSrc3
```

Cada interfaz extiende la anterior, añadiendo nuevas capacidades mientras mantiene compatibilidad hacia atrás.

---
## Interfaz IVlcSrc
La interfaz base que proporciona capacidades esenciales de carga de archivos y selección de pistas.
### Definición de la Interfaz
- **Nombre de la Interfaz**: `IVlcSrc`
- **GUID**: `{77493EB7-6D00-41C5-9535-7C593824E892}`
- **Hereda de**: `IUnknown`
- **Archivo de Cabecera**: `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)
### Métodos
#### SetFile
Establece el archivo de medios o URL a reproducir.
**Sintaxis (C++)**:
```cpp
HRESULT SetFile(WCHAR *file);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetFile([MarshalAs(UnmanagedType.LPWStr)] string file);
```
**Parámetros**:
- `file`: Cadena de caracteres anchos que contiene la ruta del archivo o URL.
**Retorna**: `S_OK` (0) en éxito, código de error de lo contrario.
**Fuentes Soportadas**:
- Archivos locales: `C:\Videos\movie.mp4`
- Transmisiones HTTP: `https://example.com/stream.m3u8`
- Transmisiones RTSP: `rtsp://example.com/live`
- Listas de reproducción HLS: `https://example.com/playlist.m3u8`
- Transmisiones DASH: `https://example.com/manifest.mpd`
- Transmisiones DVB-T/C/S
- Recursos compartidos de red: `\\server\share\video.mkv`
**Ejemplo (C++)**:
```cpp
IVlcSrc* pVlcSrc = nullptr;
pFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
pVlcSrc->SetFile(L"C:\\Videos\\movie.mkv");
pVlcSrc->Release();
```
**Ejemplo (C#)**:
```csharp
var vlcSrc = filter as IVlcSrc;
if (vlcSrc != null)
{
    vlcSrc.SetFile(@"C:\Videos\movie.mkv");
}
```
---

### Gestión de Pistas de Audio

#### GetAudioTracksCount

Recupera el número total de pistas de audio disponibles.

**Sintaxis (C++)**:

```cpp
HRESULT GetAudioTracksCount(int *count);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int GetAudioTracksCount(out int count);
```

**Parámetros**:

- `count`: [out] Recibe el número de pistas de audio.

**Retorna**: `S_OK` (0) en éxito.

**Notas de Uso**:

- Llamar después de que el archivo esté cargado y el grafo de filtros construido
- Retorna 0 si no hay pistas de audio disponibles o el archivo no está cargado

**Ejemplo (C++)**:

```cpp
int audioCount = 0;
pVlcSrc->GetAudioTracksCount(&audioCount);
printf("Pistas de audio: %d\n", audioCount);
```

---
#### GetAudioTrackInfo
Recupera información sobre una pista de audio específica.
**Sintaxis (C++)**:
```cpp
HRESULT GetAudioTrackInfo(int number, int *id, WCHAR *name);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int GetAudioTrackInfo(int number, out int id,
                      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);
```
**Parámetros**:
- `number`: Índice de pista basado en cero (0 a count-1).
- `id`: [out] Recibe el ID de la pista.
- `name`: [out] Buffer para recibir el nombre de la pista (debe estar pre-asignado, mínimo 256 caracteres).
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Pre-asignar buffer de nombre con al menos 256 caracteres anchos
- Los nombres de pista típicamente incluyen información de idioma y códec
- El ID de pista se usa con SetAudioTrack()
**Ejemplo (C++)**:
```cpp
int audioCount = 0;
pVlcSrc->GetAudioTracksCount(&audioCount);
for (int i = 0; i < audioCount; i++)
{
    int id = 0;
    WCHAR name[256] = {0};
    pVlcSrc->GetAudioTrackInfo(i, &id, name);
    wprintf(L"Pista %d - ID: %d, Nombre: %s\n", i, id, name);
}
```
**Ejemplo (C#)**:
```csharp
int count = 0;
vlcSrc.GetAudioTracksCount(out count);
for (int i = 0; i < count; i++)
{
    int id;
    var name = new StringBuilder(256);
    vlcSrc.GetAudioTrackInfo(i, out id, name);
    Console.WriteLine($"Pista {i} - ID: {id}, Nombre: {name}");
}
```
---

#### GetAudioTrack

Recupera el ID de la pista de audio actualmente activa.

**Sintaxis (C++)**:

```cpp
HRESULT GetAudioTrack(int *id);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int GetAudioTrack(out int id);
```

**Parámetros**:

- `id`: [out] Recibe el ID de la pista de audio actual.

**Retorna**: `S_OK` (0) en éxito.

**Ejemplo (C++)**:

```cpp
int currentTrack = 0;
pVlcSrc->GetAudioTrack(&currentTrack);
printf("ID de pista de audio actual: %d\n", currentTrack);
```

---
#### SetAudioTrack
Establece la pista de audio activa por ID.
**Sintaxis (C++)**:
```cpp
HRESULT SetAudioTrack(int id);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetAudioTrack(int id);
```
**Parámetros**:
- `id`: El ID de la pista a activar (obtenido de GetAudioTrackInfo).
**Retorna**: `S_OK` (0) en éxito, código de error si el ID de pista es inválido.
**Notas de Uso**:
- Puede llamarse durante la reproducción para cambiar pistas dinámicamente
- Use -1 para deshabilitar todas las pistas de audio
- El cambio de pista puede causar una breve interrupción del audio
**Ejemplo (C++)**:
```cpp
// Cambiar a la segunda pista de audio
int trackId = 0;
pVlcSrc->GetAudioTrackInfo(1, &trackId, nullptr);
pVlcSrc->SetAudioTrack(trackId);
```
**Ejemplo (C#)**:
```csharp
// Cambiar a la primera pista de audio
int trackId;
var name = new StringBuilder(256);
vlcSrc.GetAudioTrackInfo(0, out trackId, name);
vlcSrc.SetAudioTrack(trackId);
```
---

### Gestión de Pistas de Subtítulos

#### GetSubtitlesCount

Recupera el número total de pistas de subtítulos disponibles.

**Sintaxis (C++)**:

```cpp
HRESULT GetSubtitlesCount(int *count);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int GetSubtitlesCount(out int count);
```

**Parámetros**:

- `count`: [out] Recibe el número de pistas de subtítulos.

**Retorna**: `S_OK` (0) en éxito.

**Ejemplo (C++)**:

```cpp
int subtitleCount = 0;
pVlcSrc->GetSubtitlesCount(&subtitleCount);
printf("Pistas de subtítulos: %d\n", subtitleCount);
```

---
#### GetSubtitleInfo
Recupera información sobre una pista de subtítulos específica.
**Sintaxis (C++)**:
```cpp
HRESULT GetSubtitleInfo(int number, int *id, WCHAR *name);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int GetSubtitleInfo(int number, out int id,
                    [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);
```
**Parámetros**:
- `number`: Índice de pista basado en cero (0 a count-1).
- `id`: [out] Recibe el ID de la pista de subtítulos.
- `name`: [out] Buffer para recibir el nombre de la pista de subtítulos (mínimo 256 caracteres).
**Retorna**: `S_OK` (0) en éxito.
**Ejemplo (C++)**:
```cpp
int subCount = 0;
pVlcSrc->GetSubtitlesCount(&subCount);
for (int i = 0; i < subCount; i++)
{
    int id = 0;
    WCHAR name[256] = {0};
    pVlcSrc->GetSubtitleInfo(i, &id, name);
    wprintf(L"Subtítulo %d - ID: %d, Nombre: %s\n", i, id, name);
}
```
---

#### GetSubtitle

Recupera el ID de la pista de subtítulos actualmente activa.

**Sintaxis (C++)**:

```cpp
HRESULT GetSubtitle(int *id);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int GetSubtitle(out int id);
```

**Parámetros**:

- `id`: [out] Recibe el ID de la pista de subtítulos actual.

**Retorna**: `S_OK` (0) en éxito.

---
#### SetSubtitle
Establece la pista de subtítulos activa por ID.
**Sintaxis (C++)**:
```cpp
HRESULT SetSubtitle(int id);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetSubtitle(int id);
```
**Parámetros**:
- `id`: El ID de la pista de subtítulos a activar.
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Use -1 para deshabilitar subtítulos
- El renderizado de subtítulos es realizado por el renderizador interno de VLC
- Puede cambiarse durante la reproducción
**Ejemplo (C++)**:
```cpp
// Habilitar primera pista de subtítulos
int subtitleId = 0;
pVlcSrc->GetSubtitleInfo(0, &subtitleId, nullptr);
pVlcSrc->SetSubtitle(subtitleId);
// Deshabilitar subtítulos
pVlcSrc->SetSubtitle(-1);
```
---

## Interfaz IVlcSrc2

Extiende `IVlcSrc` con soporte de parámetros de línea de comandos VLC personalizados.

### Definición de la Interfaz

- **Nombre de la Interfaz**: `IVlcSrc2`
- **GUID**: `{CCE122C0-172C-4626-B4B6-42B039E541CB}`
- **Hereda de**: `IVlcSrc`
- **Archivo de Cabecera**: `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)

### Métodos

#### SetCustomCommandLine

Establece parámetros de línea de comandos VLC personalizados.

**Sintaxis (C++)**:

```cpp
HRESULT SetCustomCommandLine(char* params[], int length);
```

**Sintaxis (C#)**:

```csharp
[PreserveSig]
int SetCustomCommandLine([In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] IntPtr[] params_,
                         int size);
```

**Parámetros**:

- `params_`: Array de punteros IntPtr a cadenas codificadas en UTF-8 que contienen parámetros de línea de comandos VLC.
- `size`: Número de parámetros en el array.

**Retorna**: `S_OK` (0) en éxito.

**Notas de Uso**:

- Debe llamarse **antes** de cargar el archivo de medios con SetFile()
- Los parámetros deben convertirse a IntPtr UTF-8 nativo usando StringHelper.NativeUtf8FromString()
- La memoria asignada para parámetros IntPtr debe liberarse después de la llamada usando Marshal.FreeHGlobal()
- Los parámetros se pasan directamente a la inicialización de libVLC
- Los parámetros inválidos se ignoran con advertencias en el log de VLC
- Use la sintaxis estándar de línea de comandos VLC (vea la documentación de VLC)

**Parámetros VLC Comunes**:

| Parámetro | Descripción | Valor de Ejemplo |
|-----------|-------------|------------------|
| `--network-caching` | Caché de red en ms | `1000` |
| `--file-caching` | Caché de archivo en ms | `300` |
| `--live-caching` | Caché de transmisión en vivo en ms | `300` |
| `--avcodec-hw` | Aceleración por hardware | `any`, `dxva2`, `d3d11va` |
| `--verbose` | Verbosidad del log | `2` |
| `--rtsp-tcp` | Forzar RTSP sobre TCP | (bandera, sin valor) |
| `--no-audio` | Deshabilitar audio | (bandera, sin valor) |
| `--sout-mux-caching` | Caché del muxer de salida | `1000` |

**Ejemplo (C++)**:

```cpp
IVlcSrc2* pVlcSrc2 = nullptr;
pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);

// Configurar para RTSP de baja latencia
char* params[] = {
    "--network-caching=300",
    "--rtsp-tcp",
    "--avcodec-hw=d3d11va",
    "--verbose=2"
};

pVlcSrc2->SetCustomCommandLine(params, 4);
pVlcSrc2->SetFile(L"rtsp://192.168.1.100/stream");

pVlcSrc2->Release();
```

**Ejemplo (C#)**:

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VisioForge.Core.Helpers;

var vlcSrc2 = filter as IVlcSrc2;
if (vlcSrc2 != null)
{
    // Habilitar aceleración por hardware y ajustar caché
    var parameters = new List<string>
    {
        "--avcodec-hw=any",
        "--network-caching=1000",
        "--file-caching=300"
    };

    // Convertir cadenas a array de IntPtr UTF-8 nativo
    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, parameters.Count);
        vlcSrc2.SetFile(@"C:\Videos\movie.mkv");
    }
    finally
    {
        // Liberar memoria no administrada asignada
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

**Ejemplo (Delphi)**:

```delphi
var
  VlcSrc2: IVlcSrc2;
  Params: array[0..2] of PAnsiChar;
begin
  if Succeeded(Filter.QueryInterface(IID_IVlcSrc2, VlcSrc2)) then
  begin
    Params[0] := '--network-caching=500';
    Params[1] := '--rtsp-tcp';
    Params[2] := '--avcodec-hw=dxva2';

    VlcSrc2.SetCustomCommandLine(@Params, 3);
    VlcSrc2.SetFile('rtsp://example.com/stream');
  end;
end;
```

---
## Interfaz IVlcSrc3
Extiende `IVlcSrc2` con capacidad de sobrescritura de tasa de cuadros.
### Definición de la Interfaz
- **Nombre de la Interfaz**: `IVlcSrc3`
- **GUID**: `{3DFBED0C-E4A8-401C-93EF-CBBFB65223DD}`
- **Hereda de**: `IVlcSrc2`
- **Archivo de Cabecera**: `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)
### Métodos
#### SetDefaultFrameRate
Establece una tasa de cuadros predeterminada para medios sin información de tasa de cuadros.
**Sintaxis (C++)**:
```cpp
HRESULT SetDefaultFrameRate(double frameRate);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetDefaultFrameRate(double frameRate);
```
**Parámetros**:
- `frameRate`: Tasa de cuadros en cuadros por segundo (ej., 29.97, 30.0, 25.0, 60.0).
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Debe llamarse **antes** de cargar el archivo de medios
- Se usa cuando los medios fuente no especifican tasa de cuadros
- Particularmente útil para transmisiones de red sin información de temporización
- Valores comunes: 23.976, 24.0, 25.0, 29.97, 30.0, 50.0, 59.94, 60.0
**Ejemplo (C++)**:
```cpp
IVlcSrc3* pVlcSrc3 = nullptr;
pFilter->QueryInterface(IID_IVlcSrc3, (void**)&pVlcSrc3);
// Establecer tasa de cuadros predeterminada para transmisión de cámara IP MJPEG
pVlcSrc3->SetDefaultFrameRate(30.0);
pVlcSrc3->SetFile(L"http://192.168.1.50/video.mjpg");
pVlcSrc3->Release();
```
**Ejemplo (C#)**:
```csharp
var vlcSrc3 = filter as IVlcSrc3;
if (vlcSrc3 != null)
{
    // Establecer tasa de cuadros PAL para transmisión DV
    vlcSrc3.SetDefaultFrameRate(25.0);
    vlcSrc3.SetFile(@"dv://0");
}
```
---

## Ejemplos de Uso Completo

### Ejemplo 1: Reproducción de Película Multi-Idioma (C++)

```cpp
#include <dshow.h>
#include "ivlcsrc.h"

void PlayMovieWithAudioSelection(IBaseFilter* pVlcFilter)
{
    HRESULT hr;
    IVlcSrc* pVlcSrc = nullptr;

    hr = pVlcFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
    if (FAILED(hr))
        return;

    // Cargar película
    pVlcSrc->SetFile(L"C:\\Movies\\multilang_movie.mkv");

    // Construir y ejecutar el grafo aquí...
    // (IGraphBuilder::RenderFile, IMediaControl::Run, etc.)

    // Enumerar pistas de audio
    int audioCount = 0;
    pVlcSrc->GetAudioTracksCount(&audioCount);

    wprintf(L"Pistas de audio disponibles:\n");
    for (int i = 0; i < audioCount; i++)
    {
        int id = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetAudioTrackInfo(i, &id, name);
        wprintf(L"  [%d] %s (ID: %d)\n", i, name, id);
    }

    // Seleccionar pista de audio en inglés (asumiendo que es la pista 1)
    int englishTrackId = 0;
    pVlcSrc->GetAudioTrackInfo(1, &englishTrackId, nullptr);
    pVlcSrc->SetAudioTrack(englishTrackId);

    // Habilitar subtítulos
    int subCount = 0;
    pVlcSrc->GetSubtitlesCount(&subCount);
    if (subCount > 0)
    {
        int subId = 0;
        pVlcSrc->GetSubtitleInfo(0, &subId, nullptr);
        pVlcSrc->SetSubtitle(subId);
    }

    pVlcSrc->Release();
}
```

### Ejemplo 2: Transmisión RTSP de Baja Latencia (C#)

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;
using VisioForge.Core.Helpers;
using VisioForge.DirectShowAPI;

public class VLCRTSPPlayer
{
    public void SetupLowLatencyRTSP(IBaseFilter vlcFilter)
    {
        // Obtener interfaz IVlcSrc3 (versión más alta)
        var vlcSrc3 = vlcFilter as IVlcSrc3;
        if (vlcSrc3 == null)
            throw new NotSupportedException("IVlcSrc3 no disponible");

        // Configurar VLC para latencia mínima
        var parameters = new List<string>
        {
            "--network-caching=50",       // Buffer de red mínimo
            "--live-caching=50",          // Buffer en vivo mínimo
            "--rtsp-tcp",                 // Usar transporte TCP
            "--no-audio-time-stretch",    // Deshabilitar estiramiento de audio
            "--avcodec-hw=d3d11va",      // Decodificación por hardware
            "--verbose=0"                 // Reducir logging
        };

        // Convertir a array de IntPtr
        var array = new IntPtr[parameters.Count];
        for (int i = 0; i < parameters.Count; i++)
        {
            array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
        }

        try
        {
            int hr = vlcSrc3.SetCustomCommandLine(array, parameters.Count);
            DsError.ThrowExceptionForHR(hr);

            // Establecer tasa de cuadros para cámara IP
            hr = vlcSrc3.SetDefaultFrameRate(25.0);
            DsError.ThrowExceptionForHR(hr);

            // Cargar transmisión RTSP
            hr = vlcSrc3.SetFile("rtsp://admin:password@192.168.1.100:554/stream1");
            DsError.ThrowExceptionForHR(hr);
        }
        finally
        {
            // Liberar memoria asignada
            for (int i = 0; i < array.Length; i++)
            {
                Marshal.FreeHGlobal(array[i]);
            }
        }

        // Construir grafo de filtros e iniciar reproducción...
    }
}
```

### Ejemplo 3: UI de Cambio de Pistas de Subtítulos (Delphi)

```delphi
unit VLCSubtitles;

interface

uses
  Winapi.Windows, System.Classes, Vcl.Controls, Vcl.StdCtrls,
  DSPack, ivlcsrc;

type
  TSubtitleForm = class(TForm)
    ComboBoxSubtitles: TComboBox;
    procedure FormCreate(Sender: TObject);
    procedure ComboBoxSubtitlesChange(Sender: TObject);
  private
    FVlcSrc: IVlcSrc;
    FSubtitleIDs: TArray<Integer>;
    procedure LoadSubtitleTracks;
  public
    procedure SetVLCFilter(Filter: IBaseFilter);
  end;

implementation

procedure TSubtitleForm.SetVLCFilter(Filter: IBaseFilter);
begin
  if Succeeded(Filter.QueryInterface(IID_IVlcSrc, FVlcSrc)) then
  begin
    LoadSubtitleTracks;
  end;
end;

procedure TSubtitleForm.LoadSubtitleTracks;
var
  Count, I, ID: Integer;
  Name: array[0..255] of WideChar;
begin
  ComboBoxSubtitles.Clear;
  ComboBoxSubtitles.Items.Add('Deshabilitado');

  if FVlcSrc.GetSubtitlesCount(Count) = S_OK then
  begin
    SetLength(FSubtitleIDs, Count + 1);
    FSubtitleIDs[0] := -1; // Deshabilitado

    for I := 0 to Count - 1 do
    begin
      if FVlcSrc.GetSubtitleInfo(I, ID, Name) = S_OK then
      begin
        FSubtitleIDs[I + 1] := ID;
        ComboBoxSubtitles.Items.Add(Name);
      end;
    end;
  end;

  ComboBoxSubtitles.ItemIndex := 0;
end;

procedure TSubtitleForm.ComboBoxSubtitlesChange(Sender: TObject);
var
  Index: Integer;
begin
  Index := ComboBoxSubtitles.ItemIndex;
  if (Index >= 0) and (Index < Length(FSubtitleIDs)) then
  begin
    FVlcSrc.SetSubtitle(FSubtitleIDs[Index]);
  end;
end;

end.
```

## Mejores Prácticas

### Gestión de Pistas

1. **Siempre enumerar pistas después de construir el grafo de filtros** - La información de pistas no está disponible hasta que la fuente esté cargada
2. **Manejar archivos sin audio/subtítulos de forma elegante** - Verificar el conteo antes de acceder a las pistas
3. **Pre-asignar buffers de nombre con 256 caracteres** - Previene desbordamientos de buffer
4. **Cachear IDs de pistas** - No llamar repetidamente GetAudioTrackInfo/GetSubtitleInfo

### Configuración de VLC

1. **Usar IVlcSrc3 cuando esté disponible** - Proporciona el conjunto completo de características
2. **Establecer parámetros personalizados antes de cargar el archivo** - Los parámetros solo se aplican en la inicialización
3. **Probar parámetros VLC independientemente** - Usar línea de comandos VLC para verificar que los parámetros funcionan
4. **Usar valores de caché apropiados**:
   - Archivos locales: 300ms
   - Transmisiones de red: 1000-3000ms
   - Transmisiones de baja latencia: 50-300ms

### Aceleración por Hardware

1. **Habilitar decodificación por hardware para H.264/H.265**:

   ```cpp
   "--avcodec-hw=any"  // Auto-detectar mejor método
   ```

2. **Opciones específicas de plataforma**:
   - Windows: `d3d11va`, `dxva2`
   - Todas las plataformas: `any` (auto-detectar)

### Rendimiento

1. **Minimizar caché de red para transmisiones en vivo** - Reduce latencia
2. **Usar RTSP sobre TCP cuando UDP falle** - Más confiable a través de firewalls
3. **Habilitar logging verboso solo para depuración** - Reduce sobrecarga de rendimiento

## Interfaces Relacionadas

- **IFileSourceFilter** - Interfaz DirectShow estándar alternativa para cargar archivos
- **IAMStreamSelect** - Estándar DirectShow para selección de transmisiones (también soportado por el filtro VLC)
- **IMediaSeeking** - Control de búsqueda en medios
- **IBasicVideo** - Control de ventana de video

## Vea También

- [Descripción General del Filtro Fuente VLC](index.es.md)
- [Documentación de Línea de Comandos VLC](https://www.videolan.org/doc/)
- [Ejemplos de Código](examples.es.md)
