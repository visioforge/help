---
title: Ref Interfaz Filtro DirectShow Codificador FFMPEG
description: Interfaz DirectShow del codificador FFMPEG para FLV, MPEG-1, MPEG-2, VCD, SVCD, DVD y Transport Stream con configuración de audio/video.
---

# Referencia de la Interfaz del Codificador FFMPEG

## Descripción General

La interfaz **IVFFFMPEGEncoder** proporciona una configuración completa para codificar video y audio en varios formatos utilizando la biblioteca FFMPEG. Este potente codificador admite múltiples formatos de salida, incluidos Flash Video (FLV), MPEG-1, MPEG-2, VCD, SVCD, DVD y MPEG-2 Transport Stream.

El codificador utiliza un enfoque de configuración basado en estructuras donde todos los parámetros de codificación se establecen a la vez, proporcionando una interfaz simple pero completa para la codificación de video profesional a formatos heredados y de transmisión.

**GUID de la Interfaz**: `{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}`

**CLSID del Filtro**: `{554AB365-B293-4C1D-9245-E8DB01F027F7}`

**Hereda de**: `IUnknown`

## GUIDs del Filtro y la Interfaz

```csharp
// CLSID del Filtro Codificador FFMPEG
public static readonly Guid CLSID_FFMPEGEncoder =
    new Guid("554AB365-B293-4C1D-9245-E8DB01F027F7");

// IID de la Interfaz IVFFFMPEGEncoder
public static readonly Guid IID_IVFFFMPEGEncoder =
    new Guid("17B8FF7D-A67F-45CE-B425-0E4F607D8C60");
```

## Formatos de Salida

### Enumeración VFFFMPEGDLLOutputFormat

```csharp
/// <summary>
/// Opciones de formato de salida del codificador FFMPEG.
/// </summary>
public enum VFFFMPEGDLLOutputFormat
{
    /// <summary>
    /// Flash Video (.flv) - Formato de transmisión web
    /// </summary>
    FLV = 0,

    /// <summary>
    /// MPEG-1 (.mpg) - Video MPEG-1 estándar
    /// </summary>
    MPEG1 = 1,

    /// <summary>
    /// MPEG-1 VCD - Formato compatible con Video CD
    /// Resolución: 352x240 (NTSC) o 352x288 (PAL)
    /// Tasa de bits: 1150 kbps
    /// </summary>
    MPEG1VCD = 2,

    /// <summary>
    /// MPEG-2 (.mpg) - Video MPEG-2 estándar
    /// </summary>
    MPEG2 = 3,

    /// <summary>
    /// MPEG-2 Transport Stream (.ts) - Radiodifusión y transmisión
    /// </summary>
    MPEG2TS = 4,

    /// <summary>
    /// MPEG-2 SVCD - Formato compatible con Super Video CD
    /// Resolución: 480x480 (NTSC) o 480x576 (PAL)
    /// Tasa de bits: 2500 kbps
    /// </summary>
    MPEG2SVCD = 5,

    /// <summary>
    /// MPEG-2 DVD - Formato compatible con DVD-Video
    /// Resolución: 720x480 (NTSC) o 720x576 (PAL)
    /// Tasa de bits: Hasta 9800 kbps
    /// </summary>
    MPEG2DVD = 6
}
```

### Estándares del Sistema de TV

```csharp
/// <summary>
/// Estándares del sistema de televisión para codificación de video.
/// </summary>
public enum VFFFMPEGDLLTVSystem
{
    /// <summary>
    /// Sin sistema de TV específico / Detección automática
    /// </summary>
    None = 0,

    /// <summary>
    /// PAL (Phase Alternating Line)
    /// 25 fps, 576 líneas
    /// Utilizado en: Europa, Asia, Australia, África
    /// </summary>
    PAL = 1,

    /// <summary>
    /// NTSC (National Television System Committee)
    /// 29.97 fps, 480 líneas
    /// Utilizado en: América del Norte, Japón, Corea del Sur
    /// </summary>
    NTSC = 2,

    /// <summary>
    /// Estándar de cine
    /// 24 fps
    /// Utilizado para: Cine, transferencias de películas
    /// </summary>
    Film = 3
}
```

## Estructura FFMPEGOutputSettings

```csharp
/// <summary>
/// Configuración completa para el codificador FFMPEG.
/// Contiene todos los ajustes de audio, video y formato de salida.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FFMPEGOutputSettings
{
    /// <summary>
    /// Nombre del archivo de salida con ruta.
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Filename;

    /// <summary>
    /// Verdadero si el flujo de audio está incluido en la salida.
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool AudioAvailable;

    /// <summary>
    /// Tasa de bits de audio en bits por segundo (por ejemplo, 128000 para 128 kbps).
    /// </summary>
    public int AudioBitrate;

    /// <summary>
    /// Frecuencia de muestreo de audio en Hz (por ejemplo, 44100, 48000).
    /// </summary>
    public int AudioSamplerate;

    /// <summary>
    /// Número de canales de audio (1 = mono, 2 = estéreo).
    /// </summary>
    public int AudioChannels;

    /// <summary>
    /// Ancho del cuadro de video en píxeles.
    /// </summary>
    public int VideoWidth;

    /// <summary>
    /// Alto del cuadro de video en píxeles.
    /// </summary>
    public int VideoHeight;

    /// <summary>
    /// Ancho de la relación de aspecto de visualización (por ejemplo, 16 para 16:9).
    /// </summary>
    public int AspectRatioW;

    /// <summary>
    /// Alto de la relación de aspecto de visualización (por ejemplo, 9 para 16:9).
    /// </summary>
    public int AspectRatioH;

    /// <summary>
    /// Tasa de bits de video en bits por segundo (por ejemplo, 5000000 para 5 Mbps).
    /// </summary>
    public int VideoBitrate;

    /// <summary>
    /// Tasa de bits de video máxima para codificación VBR (bits por segundo).
    /// </summary>
    public int VideoMaxRate;

    /// <summary>
    /// Tasa de bits de video mínima para codificación VBR (bits por segundo).
    /// </summary>
    public int VideoMinRate;

    /// <summary>
    /// Tamaño del búfer de video en bits (afecta la latencia y la fluidez).
    /// </summary>
    public int VideoBufferSize;

    /// <summary>
    /// Verdadero para habilitar la codificación entrelazada (para TV de radiodifusión).
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool Interlace;

    /// <summary>
    /// Tamaño del GOP (Grupo de Imágenes) - intervalo de fotogramas clave.
    /// Típico: 12-15 para MPEG-2, 30-60 para video web.
    /// </summary>
    public int VideoGopSize;

    /// <summary>
    /// Estándar del sistema de TV (PAL, NTSC, Film o None).
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public VFFFMPEGDLLTVSystem TVSystem;

    /// <summary>
    /// Formato de salida (FLV, MPEG-1, MPEG-2, etc.).
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public VFFFMPEGDLLOutputFormat OutputFormat;
}
```

## Definiciones de Interfaz

### Definición en C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interfaz de configuración del codificador FFMPEG.
    /// Proporciona codificación completa a múltiples formatos utilizando la biblioteca FFMPEG.
    /// </summary>
    [ComImport]
    [Guid("17B8FF7D-A67F-45CE-B425-0E4F607D8C60")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFFFMPEGEncoder
    {
        /// <summary>
        /// Establece la configuración completa del codificador FFMPEG.
        /// Todos los parámetros de codificación deben establecerse a la vez a través de esta estructura.
        /// </summary>
        /// <param name="settings">Estructura completa de configuración del codificador</param>
        [PreserveSig]
        void set_settings([In] FFMPEGOutputSettings settings);
    }
}
```

### Definición en C++

```cpp
#include <unknwn.h>

// {17B8FF7D-A67F-45CE-B425-0E4F607D8C60}
DEFINE_GUID(IID_IVFFFMPEGEncoder,
    0x17b8ff7d, 0xa67f, 0x45ce, 0xb4, 0x25, 0xe, 0x4f, 0x60, 0x7d, 0x8c, 0x60);

// {554AB365-B293-4C1D-9245-E8DB01F027F7}
DEFINE_GUID(CLSID_FFMPEGEncoder,
    0x554ab365, 0xb293, 0x4c1d, 0x92, 0x45, 0xe8, 0xdb, 0x01, 0xf0, 0x27, 0xf7);

/// <summary>
/// Enumeración de formato de salida.
/// </summary>
enum FFOutputFormat
{
    of_FLV = 0,
    of_MPEG1 = 1,
    of_MPEG1VCD = 2,
    of_MPEG2 = 3,
    of_MPEG2TS = 4,
    of_MPEG2SVCD = 5,
    of_MPEG2DVD = 6
};

/// <summary>
/// Enumeración del sistema de TV.
/// </summary>
enum video_tv_system_t
{
    video_norm_unknown = 0,
    video_norm_pal = 1,
    video_norm_ntsc = 2,
    video_norm_film = 3
};

/// <summary>
/// Estructura de configuración de salida del codificador FFMPEG.
/// </summary>
struct CVFOutputSettings
{
    wchar_t* filename;

    BOOL audioAvailable;
    int audioBitrate;
    int audioSamplerate;
    int audioChannels;

    int videoWidth;
    int videoHeight;
    int aspectRatioW;
    int aspectRatioH;
    int videoBitrate;
    int videoMaxRate;
    int videoMinRate;
    int videoBufferSize;
    BOOL interlace;
    int videoGopSize;
    int tvSystem;

    int outputFormat;
};

/// <summary>
/// Interfaz de configuración del codificador FFMPEG.
/// </summary>
DECLARE_INTERFACE_(IVFFFMPEGEncoder, IUnknown)
{
    /// <summary>
    /// Establece la configuración completa del codificador FFMPEG.
    /// </summary>
    /// <param name="settings">Estructura completa de configuración del codificador</param>
    STDMETHOD(set_settings)(THIS_
        CVFOutputSettings settings
        ) PURE;
};
```

### Definición en Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IVFFFMPEGEncoder: TGUID = '{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}';
  CLSID_FFMPEGEncoder: TGUID = '{554AB365-B293-4C1D-9245-E8DB01F027F7}';

type
  /// <summary>
  /// Enumeración de formato de salida FFMPEG.
  /// </summary>
  TVFFFMPEGDLLOutputFormat = (
    FLV,
    MPEG1,
    MPEG1VCD,
    MPEG2,
    MPEG2TS,
    MPEG2SVCD,
    MPEG2DVD
  );

  /// <summary>
  /// Enumeración del sistema de TV.
  /// </summary>
  TVFFFMPEGDLLTVSystem = (
    None,
    PAL,
    NTSC,
    Film
  );

  /// <summary>
  /// Estructura de configuración de salida del codificador FFMPEG.
  /// </summary>
  TFFMPEGOutputSettings = record
    Filename: PWideChar;
    AudioAvailable: BOOL;
    AudioBitrate: Integer;
    AudioSamplerate: Integer;
    AudioChannels: Integer;
    VideoWidth: Integer;
    VideoHeight: Integer;
    AspectRatioW: Integer;
    AspectRatioH: Integer;
    VideoBitrate: Integer;
    VideoMaxRate: Integer;
    VideoMinRate: Integer;
    VideoBufferSize: Integer;
    Interlace: BOOL;
    VideoGopSize: Integer;
    TVSystem: Integer;
    OutputFormat: Integer;
  end;

  /// <summary>
  /// Interfaz de configuración del codificador FFMPEG.
  /// </summary>
  IVFFFMPEGEncoder = interface(IUnknown)
    ['{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}']

    /// <summary>
    /// Establece la configuración completa del codificador FFMPEG.
    /// </summary>
    procedure set_settings(const settings: TFFMPEGOutputSettings); stdcall;
  end;
```

## Especificaciones de Formato de Salida

### FLV (Flash Video)

**Formato**: Adobe Flash Video (.flv)
**Casos de Uso**: Transmisión web, contenido Flash heredado
**Configuración Típica**:
- Resolución: 640x480, 854x480, 1280x720
- Tasa de bits de video: 500-2500 kbps
- Audio: MP3, 64-128 kbps, 44100 Hz
- GOP: 30-60 fotogramas

### MPEG-1

**Formato**: Video MPEG-1 (.mpg)
**Casos de Uso**: Video básico, sistemas heredados, web
**Configuración Típica**:
- Resolución: 352x240 (NTSC), 352x288 (PAL)
- Tasa de bits de video: 1150 kbps
- Audio: MPEG Layer 2, 224 kbps, 44100 Hz
- GOP: 12-15 fotogramas

### MPEG-1 VCD (Video CD)

**Formato**: MPEG-1 compatible con Video CD (.mpg)
**Casos de Uso**: Autoría de VCD, distribución en disco
**Configuración Requerida**:
- Resolución: 352x240 (NTSC), 352x288 (PAL)
- Tasa de bits de video: 1150 kbps (fija)
- Audio: MPEG Layer 2, 224 kbps, 44100 Hz
- GOP: 12 fotogramas (NTSC), 15 fotogramas (PAL)
- Sistema de TV: Debe coincidir (NTSC o PAL)

### MPEG-2

**Formato**: Video MPEG-2 (.mpg)
**Casos de Uso**: Autoría de DVD, radiodifusión, alta calidad
**Configuración Típica**:
- Resolución: 720x480 (NTSC), 720x576 (PAL)
- Tasa de bits de video: 4000-9800 kbps
- Audio: MPEG Layer 2 o AC-3, 192-448 kbps
- GOP: 12-15 fotogramas

### MPEG-2 TS (Transport Stream)

**Formato**: MPEG-2 Transport Stream (.ts)
**Casos de Uso**: Radiodifusión, transmisión, Blu-ray
**Configuración Típica**:
- Resolución: 720x480, 1280x720, 1920x1080
- Tasa de bits de video: 3000-15000 kbps
- Audio: MPEG Layer 2 o AC-3
- GOP: 12-30 fotogramas
- Mejor resistencia a errores que MPEG-2 PS

### MPEG-2 SVCD (Super Video CD)

**Formato**: MPEG-2 compatible con Super Video CD (.mpg)
**Casos de Uso**: Autoría de SVCD, distribución en disco
**Configuración Requerida**:
- Resolución: 480x480 (NTSC), 480x576 (PAL)
- Tasa de bits de video: 2500 kbps (típica)
- Audio: MPEG Layer 2, 224 kbps, 44100 Hz
- GOP: 12-15 fotogramas
- Sistema de TV: Debe coincidir (NTSC o PAL)

### MPEG-2 DVD (DVD-Video)

**Formato**: MPEG-2 compatible con DVD-Video (.mpg)
**Casos de Uso**: Autoría de DVD, distribución profesional
**Configuración Requerida**:
- Resolución: 720x480 (NTSC), 720x576 (PAL)
- Tasa de bits de video: 4000-9800 kbps
- Audio: AC-3 o PCM, hasta 448 kbps
- GOP: 12 fotogramas (NTSC), 15 fotogramas (PAL)
- Sistema de TV: Debe coincidir (NTSC o PAL)
- Entrelazado: Típicamente habilitado para radiodifusión

## Ejemplos de Uso

### Ejemplo en C# - MPEG-2 Calidad DVD (NTSC)

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class FFMPEGDVDEncoder
{
    public void ConfigureDVDNTSC(IBaseFilter ffmpegEncoder)
    {
        // Consultar la interfaz del codificador FFMPEG
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
        {
            Console.WriteLine("Error: El filtro no admite IVFFFMPEGEncoder");
            return;
        }

        // Configurar codificación MPEG-2 compatible con DVD (NTSC)
        var settings = new FFMPEGOutputSettings
        {
            // Archivo de salida
            Filename = "C:\\output\\movie.mpg",

            // Configuración de audio
            AudioAvailable = true,
            AudioBitrate = 224000,      // 224 kbps
            AudioSamplerate = 48000,    // 48 kHz para DVD
            AudioChannels = 2,           // Estéreo

            // Configuración de video - Especificaciones DVD NTSC
            VideoWidth = 720,
            VideoHeight = 480,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 6000000,     // 6 Mbps
            VideoMaxRate = 9800000,     // 9.8 Mbps máx para DVD
            VideoMinRate = 0,
            VideoBufferSize = 1835008,  // Búfer DVD estándar
            Interlace = true,           // DVD es entrelazado
            VideoGopSize = 12,          // 12 fotogramas para NTSC

            // Configuración de formato
            TVSystem = VFFFMPEGDLLTVSystem.NTSC,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2DVD
        };

        encoder.set_settings(settings);

        Console.WriteLine("Codificador FFMPEG configurado para DVD NTSC:");
        Console.WriteLine("  Resolución: 720x480 (16:9)");
        Console.WriteLine("  Video: 6 Mbps MPEG-2, entrelazado");
        Console.WriteLine("  Audio: 224 kbps, 48 kHz estéreo");
        Console.WriteLine("  GOP: 12 fotogramas");
    }
}
```

### Ejemplo en C# - Transmisión Web FLV

```csharp
public class FFMPEGWebStreaming
{
    public void ConfigureFLV(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configurar Flash Video para transmisión web
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\video.flv",

            // Configuración de audio
            AudioAvailable = true,
            AudioBitrate = 96000,       // 96 kbps
            AudioSamplerate = 44100,
            AudioChannels = 2,

            // Configuración de video - Transmisión web 720p
            VideoWidth = 1280,
            VideoHeight = 720,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 2000000,     // 2 Mbps
            VideoMaxRate = 2500000,     // 2.5 Mbps máx
            VideoMinRate = 1500000,     // 1.5 Mbps mín
            VideoBufferSize = 2000000,
            Interlace = false,          // Progresivo para web
            VideoGopSize = 60,          // 2 segundos a 30fps

            TVSystem = VFFFMPEGDLLTVSystem.None,
            OutputFormat = VFFFMPEGDLLOutputFormat.FLV
        };

        encoder.set_settings(settings);

        Console.WriteLine("Codificador FFMPEG configurado para transmisión web FLV:");
        Console.WriteLine("  720p @ 2 Mbps, progresivo");
    }
}
```

### Ejemplo en C# - MPEG-1 Compatible con VCD (PAL)

```csharp
public class FFMPEGVCDEncoder
{
    public void ConfigureVCDPAL(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configurar MPEG-1 compatible con VCD (PAL)
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\vcd.mpg",

            // Configuración de audio - Especificación VCD
            AudioAvailable = true,
            AudioBitrate = 224000,      // 224 kbps requerido
            AudioSamplerate = 44100,    // 44.1 kHz requerido
            AudioChannels = 2,

            // Configuración de video - Especificaciones VCD PAL
            VideoWidth = 352,
            VideoHeight = 288,          // Resolución PAL
            AspectRatioW = 4,
            AspectRatioH = 3,
            VideoBitrate = 1150000,     // 1150 kbps requerido
            VideoMaxRate = 1150000,
            VideoMinRate = 1150000,
            VideoBufferSize = 327680,   // Tamaño de búfer VCD
            Interlace = false,
            VideoGopSize = 15,          // 15 fotogramas para PAL

            TVSystem = VFFFMPEGDLLTVSystem.PAL,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG1VCD
        };

        encoder.set_settings(settings);

        Console.WriteLine("Codificador FFMPEG configurado para VCD PAL:");
        Console.WriteLine("  352x288 @ 1150 kbps");
    }
}
```

### Ejemplo en C# - MPEG-2 Transport Stream

```csharp
public class FFMPEGMPEG2TS
{
    public void ConfigureMPEG2TS(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configurar MPEG-2 Transport Stream para radiodifusión
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\stream.ts",

            // Configuración de audio
            AudioAvailable = true,
            AudioBitrate = 192000,
            AudioSamplerate = 48000,
            AudioChannels = 2,

            // Configuración de video - Radiodifusión HD 1080i
            VideoWidth = 1920,
            VideoHeight = 1080,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 12000000,    // 12 Mbps
            VideoMaxRate = 15000000,    // 15 Mbps máx
            VideoMinRate = 8000000,     // 8 Mbps mín
            VideoBufferSize = 8000000,
            Interlace = true,           // Radiodifusión es entrelazada
            VideoGopSize = 15,

            TVSystem = VFFFMPEGDLLTVSystem.NTSC,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2TS
        };

        encoder.set_settings(settings);

        Console.WriteLine("Codificador FFMPEG configurado para MPEG-2 TS:");
        Console.WriteLine("  Flujo de radiodifusión HD 1080i");
    }
}
```

### Ejemplo en C++ - DVD NTSC

```cpp
#include <dshow.h>
#include <iostream>
#include "InterfaceDefine.h"

void ConfigureFFMPEGDVD(IBaseFilter* pFFMPEGEncoder)
{
    IVFFFMPEGEncoder* pEncoder = NULL;
    HRESULT hr = S_OK;

    // Consultar la interfaz del codificador FFMPEG
    hr = pFFMPEGEncoder->QueryInterface(IID_IVFFFMPEGEncoder,
                                        (void**)&pEncoder);
    if (FAILED(hr) || !pEncoder)
    {
        std::cout << "Error: El filtro no admite IVFFFMPEGEncoder" << std::endl;
        return;
    }

    // Configurar codificación MPEG-2 compatible con DVD (NTSC)
    CVFOutputSettings settings;
    ZeroMemory(&settings, sizeof(settings));

    settings.filename = L"C:\\output\\movie.mpg";

    // Configuración de audio
    settings.audioAvailable = TRUE;
    settings.audioBitrate = 224000;
    settings.audioSamplerate = 48000;
    settings.audioChannels = 2;

    // Configuración de video - Especificaciones DVD NTSC
    settings.videoWidth = 720;
    settings.videoHeight = 480;
    settings.aspectRatioW = 16;
    settings.aspectRatioH = 9;
    settings.videoBitrate = 6000000;
    settings.videoMaxRate = 9800000;
    settings.videoMinRate = 0;
    settings.videoBufferSize = 1835008;
    settings.interlace = TRUE;
    settings.videoGopSize = 12;

    settings.tvSystem = video_norm_ntsc;
    settings.outputFormat = of_MPEG2DVD;

    pEncoder->set_settings(settings);

    std::cout << "Codificador FFMPEG configurado para DVD NTSC" << std::endl;

    pEncoder->Release();
}
```

### Ejemplo en C++ - Transmisión Web FLV

```cpp
void ConfigureFFMPEGFLV(IBaseFilter* pFFMPEGEncoder)
{
    IVFFFMPEGEncoder* pEncoder = NULL;
    HRESULT hr = pFFMPEGEncoder->QueryInterface(IID_IVFFFMPEGEncoder,
                                                (void**)&pEncoder);
    if (SUCCEEDED(hr) && pEncoder)
    {
        CVFOutputSettings settings;
        ZeroMemory(&settings, sizeof(settings));

        settings.filename = L"C:\\output\\video.flv";

        // Configuración de audio
        settings.audioAvailable = TRUE;
        settings.audioBitrate = 96000;
        settings.audioSamplerate = 44100;
        settings.audioChannels = 2;

        // Configuración de video - Web 720p
        settings.videoWidth = 1280;
        settings.videoHeight = 720;
        settings.aspectRatioW = 16;
        settings.aspectRatioH = 9;
        settings.videoBitrate = 2000000;
        settings.videoMaxRate = 2500000;
        settings.videoMinRate = 1500000;
        settings.videoBufferSize = 2000000;
        settings.interlace = FALSE;
        settings.videoGopSize = 60;

        settings.tvSystem = video_norm_unknown;
        settings.outputFormat = of_FLV;

        pEncoder->set_settings(settings);

        std::cout << "Codificador FLV FFMPEG configurado" << std::endl;

        pEncoder->Release();
    }
}
```

### Ejemplo en Delphi - DVD PAL

```delphi
uses
  DirectShow9, ActiveX;

procedure ConfigureFFMPEGDVDPAL(FFMPEGEncoder: IBaseFilter);
var
  Encoder: IVFFFMPEGEncoder;
  Settings: TFFMPEGOutputSettings;
  hr: HRESULT;
begin
  hr := FFMPEGEncoder.QueryInterface(IID_IVFFFMPEGEncoder, Encoder);
  if Failed(hr) or (Encoder = nil) then
  begin
    WriteLn('Error: El filtro no admite IVFFFMPEGEncoder');
    Exit;
  end;

  try
    ZeroMemory(@Settings, SizeOf(Settings));

    Settings.Filename := 'C:\output\movie.mpg';

    // Configuración de audio
    Settings.AudioAvailable := True;
    Settings.AudioBitrate := 224000;
    Settings.AudioSamplerate := 48000;
    Settings.AudioChannels := 2;

    // Configuración de video - Especificaciones DVD PAL
    Settings.VideoWidth := 720;
    Settings.VideoHeight := 576;
    Settings.AspectRatioW := 16;
    Settings.AspectRatioH := 9;
    Settings.VideoBitrate := 6000000;
    Settings.VideoMaxRate := 9800000;
    Settings.VideoMinRate := 0;
    Settings.VideoBufferSize := 1835008;
    Settings.Interlace := True;
    Settings.VideoGopSize := 15;

    Settings.TVSystem := Ord(PAL);
    Settings.OutputFormat := Ord(MPEG2DVD);

    Encoder.set_settings(Settings);

    WriteLn('Codificador FFMPEG configurado para DVD PAL');
  finally
    Encoder := nil;
  end;
end;
```

## Mejores Prácticas

### Recomendaciones Específicas por Formato

**Para Autoría de DVD (MPEG2DVD)**:
- Siempre haga coincidir el sistema de TV con la región de destino (NTSC para América/Japón, PAL para Europa/Asia)
- Use 720x480 para NTSC, 720x576 para PAL
- Habilite el entrelazado para compatibilidad con radiodifusión
- GOP: 12 fotogramas (NTSC), 15 fotogramas (PAL)
- Tasa de bits de video: 4-9.8 Mbps
- Audio: 48 kHz, 224-448 kbps

**Para VCD/SVCD (MPEG1VCD, MPEG2SVCD)**:
- Siga estrictamente las especificaciones de formato (resolución, tasa de bits)
- Haga coincidir el sistema de TV con la región de destino
- Use las tasas de bits exactas especificadas para la mejor compatibilidad
- Pruebe en hardware de destino (reproductores VCD/SVCD independientes)

**Para Transmisión Web (FLV)**:
- Use codificación progresiva (no entrelazada)
- Tasas de bits más bajas para un alcance más amplio (500-2500 kbps)
- GOP: 30-60 fotogramas para puntos de búsqueda cada 1-2 segundos
- Considere alternativas modernas (MP4/H.264) para mejor calidad

**Para Radiodifusión (MPEG2TS)**:
- Transport Stream tiene mejor resistencia a errores
- Úselo para transmisión en vivo y aplicaciones de radiodifusión
- Tasas de bits más altas aceptables (10-15 Mbps para HD)
- Haga coincidir el entrelazado con el estándar de radiodifusión

### Resolución y Relación de Aspecto

**Resoluciones Estándar por Formato**:

| Formato | Resolución NTSC | Resolución PAL | Relación de Aspecto |
|---------|-----------------|----------------|---------------------|
| VCD | 352x240 | 352x288 | 4:3 |
| SVCD | 480x480 | 480x576 | 4:3 o 16:9 |
| DVD | 720x480 | 720x576 | 4:3 o 16:9 |
| FLV/MPEG-2 | Cualquiera | Cualquiera | Cualquiera |

**Configuración de Relación de Aspecto**:
- Estándar 4:3: `AspectRatioW = 4, AspectRatioH = 3`
- Pantalla ancha 16:9: `AspectRatioW = 16, AspectRatioH = 9`
- Asegúrese de que la relación de aspecto de visualización coincida con la relación de aspecto de píxeles

### Pautas de Tamaño de GOP

**VCD/SVCD/DVD**:
- NTSC: 12 fotogramas (0.4 segundos a 29.97 fps)
- PAL: 15 fotogramas (0.6 segundos a 25 fps)
- Requerido para cumplimiento de formato

**Transmisión Web**:
- 30-60 fotogramas (1-2 segundos)
- GOP más corto: Mejor búsqueda, archivo más grande
- GOP más largo: Archivo más pequeño, búsqueda más lenta

**Radiodifusión**:
- 12-15 fotogramas para alta calidad
- Haga coincidir con el estándar del sistema de TV

### Configuración de Tasa de Bits

**VBR (Tasa de Bits Variable)**:
- Establezca `VideoBitrate` (promedio), `VideoMinRate`, `VideoMaxRate`
- Mejor calidad para la misma tasa de bits promedio
- Úselo para codificación basada en archivos

**CBR (Tasa de Bits Constante)**:
- Establezca las tres tasas de bits al mismo valor
- Tamaño de archivo y ancho de banda predecibles
- Úselo para transmisión y radiodifusión

### Configuración de Audio

**Frecuencias de Muestreo**:
- 44100 Hz: Calidad CD, VCD/SVCD
- 48000 Hz: DVD, radiodifusión profesional
- Haga coincidir con la fuente cuando sea posible

**Tasas de Bits**:
- Voz mono: 64-96 kbps
- Música estéreo: 128-224 kbps
- Audio DVD: 192-448 kbps

## Solución de Problemas

### La Inicialización del Codificador Falla

**Síntomas**: La llamada a `set_settings` falla o se bloquea

**Posibles Causas**:
1. Nombre de archivo o ruta no válidos
2. Especificaciones de formato incorrectas
3. Combinación de resolución/tasa de bits no admitida

**Soluciones**:
- Asegúrese de que el directorio de salida exista y sea escribible
- Verifique que la resolución coincida con los requisitos del formato
- Compruebe que la tasa de bits esté dentro de los límites del formato
- Para VCD/SVCD/DVD, siga estrictamente las especificaciones

### El Archivo de Salida No Se Reproduce

**Síntomas**: El archivo codificado no se reproduce o tiene errores

**Posibles Causas**:
1. Especificaciones de formato no cumplidas
2. Sistema de TV incorrecto para el formato
3. Desajuste de entrelazado
4. Problemas de tamaño de GOP

**Soluciones**:
- Para DVD/VCD/SVCD: Use parámetros de especificación exactos
- Haga coincidir el sistema de TV con la región de destino
- Habilite el entrelazado para DVD/radiodifusión
- Use tamaños de GOP estándar (12 para NTSC, 15 para PAL)

### Mala Calidad de Video

**Síntomas**: Video pixelado, borroso o con artefactos

**Posibles Causas**:
1. Tasa de bits demasiado baja para la resolución
2. Tamaño de GOP incorrecto
3. Rango de tasa de bits demasiado restrictivo

**Soluciones**:
- Aumente la tasa de bits de video (ver recomendaciones de formato)
- Para VBR, amplíe el rango de tasa de bits mín/máx
- Use el tamaño de GOP apropiado para el formato
- Asegúrese de que la resolución coincida con las especificaciones del formato

### Problemas de Sincronización A/V

**Síntomas**: El audio y el video se desincronizan

**Posibles Causas**:
1. Frecuencia de muestreo incorrecta
2. Velocidad de fotogramas incorrecta para el sistema de TV
3. Problemas de tamaño de búfer

**Soluciones**:
- Use 48000 Hz para DVD, 44100 Hz para VCD
- Asegúrese de que el sistema de TV coincida con la velocidad de fotogramas de la fuente
- Aumente `VideoBufferSize` para contenido complejo
- Verifique que el audio/video de origen estén sincronizados

### DVD/VCD No Se Reproduce en Hardware

**Síntomas**: El archivo se reproduce en la computadora pero no en el reproductor independiente

**Posibles Causas**:
1. Especificaciones de formato no seguidas estrictamente
2. Sistema de TV incorrecto
3. Tamaño de GOP o tasa de bits no compatibles

**Soluciones**:
- **Crítico**: Use especificaciones de formato exactas
- VCD: 352x240/288, 1150 kbps, GOP 12/15
- DVD: 720x480/576, máx 9.8 Mbps, GOP 12/15
- Haga coincidir el sistema de TV con la región del reproductor
- Habilite el entrelazado para DVD
- Pruebe primero con un reproductor de DVD/VCD por software
