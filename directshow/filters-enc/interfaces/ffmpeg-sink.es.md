---
title: Pines del filtro FFMPEG Encoder y opciones del códec
description: Opciones del codificador FFmpeg, diezmado de fotogramas y filtros de audio por pin del filtro DirectShow FFMPEG Encoder, con ejemplos WebM, Ogg Speex y FLAC.
tags:
  - DirectShow
  - C++
  - C#
  - Windows
  - Encoding
  - FFmpeg
  - WebM
  - VP8
  - VP9
  - Vorbis
  - Opus
  - Speex
  - FLAC
primary_api_classes:
  - IFFmpegSinkPinSettings2
  - IFFmpegSinkPinSettings
  - IFFmpegSinkSettings
  - IFileSinkFilter

---

# Referencia de la configuración de pines de FFMPEG Encoder

## Descripción general

**VisioForge FFMPEG Encoder** (`VisioForge_FFMPEG_Encoder.ax`) es un renderizador DirectShow que codifica y multiplexa sus entradas con FFmpeg. Cada pin de entrada conectado es un flujo del archivo de salida: video, audio o subtítulos. El filtro añade un nuevo pin de entrada cada vez que se conecta uno.

| Elemento | Valor |
|---|---|
| CLSID del filtro | `{69F244AB-31A9-4B74-81CB-CF8C9FACB2EC}` |
| `IFFmpegSinkSettings` (filtro) | `{CAEC87C3-24B5-404C-9985-FA18D2B78A4A}` |
| `IFFmpegSinkPinSettings` (pin de entrada) | `{496D3BA7-0630-499E-A730-577D4A049F9F}` |
| `IFFmpegSinkPinSettings2` (pin de entrada) | `{1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B}`, desde la versión 2026.11.25.1 del filtro |

El nombre del archivo de salida se establece con el método estándar `IFileSinkFilter::SetFileName`, y el contenedor con `IFFmpegSinkSettings::SetFormat`. Cada pin de entrada recibe su códec y la configuración del codificador mediante `IFFmpegSinkPinSettings`, y los ajustes más finos de FFmpeg mediante `IFFmpegSinkPinSettings2`:

- **`SetCodecOption`** pasa cualquier opción del codificador FFmpeg por su nombre: el deadline de VP8/VP9, la colocación de fotogramas clave y el redimensionado, el VBR de Speex, el tamaño de bloque de FLAC y todo lo demás que acepte el codificador.
- **`SetVideoDecimate`** conserva uno de cada *n* fotogramas de video, para grabar a baja frecuencia de fotogramas una fuente a frecuencia completa.
- **`SetAudioFilter`** pasa el audio por un grafo de filtros FFmpeg antes del codificador: control de ganancia, reducción de ruido, ecualización.

Este filtro no es el que se describe en [Interfaz de FFmpeg Encoder (IVFFFMPEGEncoder)](ffmpeg.md); esa página trata un filtro anterior con otro CLSID.

## Límites de plataforma y de uso

- Solo Windows, como filtro DirectShow, en compilaciones x86 y x64. No hay compilación ARM64.
- El filtro necesita a su lado las DLL de FFmpeg de su misma arquitectura; consulte [Archivos redistribuibles de FFMPEG Encoder](../../deployment/redistributable-files.md#codificador-ffmpeg).
- Establezca la configuración del pin después de conectarlo y antes de ejecutar el grafo. Se lee al abrir la salida, en el paso de detenido a pausado o en ejecución; un cambio mientras el grafo se ejecuta se aplica en la siguiente ejecución.
- Un ajuste que el codificador rechaza hace fallar esa apertura: `IMediaControl::Run` (o `Pause`) devuelve un HRESULT de error y no se escribe ningún archivo. El motivo se escribe en la salida de depuración (`OutputDebugString`, visible en DebugView), por ejemplo `Session open failed: libvorbis has no option 'no_such_option'`.

## Formatos y códecs

`IFFmpegSinkSettings::SetFormat` recibe `FFMPEG_SINK_FORMAT`. `FFMPEG_SINK_FORMAT_FLAC` (un archivo `.flac` nativo) es nuevo en 2026.11.25.1:

| Valor | Nombre | Valor | Nombre |
|---|---|---|---|
| 0 | `FFMPEG_SINK_FORMAT_AUTO` (según el nombre del archivo) | 8 | `FFMPEG_SINK_FORMAT_WEBM` |
| 1 | `FFMPEG_SINK_FORMAT_MP4` | 9 | `FFMPEG_SINK_FORMAT_M4A` |
| 2 | `FFMPEG_SINK_FORMAT_MKV` | 10 | `FFMPEG_SINK_FORMAT_MOV` |
| 3 | `FFMPEG_SINK_FORMAT_FLV` | 11 | `FFMPEG_SINK_FORMAT_MPEG1` |
| 4 | `FFMPEG_SINK_FORMAT_OGG` | 12 | `FFMPEG_SINK_FORMAT_MPEG2` |
| 5 | `FFMPEG_SINK_FORMAT_MPEG_TS` | 13 | `FFMPEG_SINK_FORMAT_MPEG_TS_KLV` |
| 6 | `FFMPEG_SINK_FORMAT_VOB` | 14 | `FFMPEG_SINK_FORMAT_FLAC` |
| 7 | `FFMPEG_SINK_FORMAT_AVI` | | |

`IFFmpegSinkPinSettings::SetAudioCodec` recibe `FFMPEG_SINK_AUDIO_CODEC`. `FFMPEG_SINK_AUDIO_CODEC_SPEEX` (libspeex, en Ogg) es nuevo en 2026.11.25.1:

| Valor | Nombre | Valor | Nombre |
|---|---|---|---|
| 0 | `FFMPEG_SINK_AUDIO_CODEC_AUTO` | 4 | `FFMPEG_SINK_AUDIO_CODEC_FLAC` |
| 1 | `FFMPEG_SINK_AUDIO_CODEC_MP2` | 5 | `FFMPEG_SINK_AUDIO_CODEC_VORBIS` |
| 2 | `FFMPEG_SINK_AUDIO_CODEC_MP3` | 6 | `FFMPEG_SINK_AUDIO_CODEC_OPUS` |
| 3 | `FFMPEG_SINK_AUDIO_CODEC_AAC` | 7 | `FFMPEG_SINK_AUDIO_CODEC_SPEEX` |

Speex codifica solo a 8, 16 o 32 kHz. Sin `SetAudioSampleRate`, el filtro toma la frecuencia admitida más cercana a la de la fuente, de modo que una fuente de 44,1 kHz se codifica a 32 kHz; establezca 16000 para voz de banda ancha con una tasa de bits menor. MP2 admite solo las tasas de bits estándar de Layer II, y cada una solo con ciertas frecuencias de muestreo: 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 o 384 kbps a 32, 44,1 y 48 kHz; 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144 o 160 kbps a 16, 22,05 y 24 kHz. Una tasa de bits establecida con `SetAudioBitRate` o con la opción `b`/`ab` debe ser una de ellas, o el inicio falla; sin ninguna, MP2 usa 384 kbps a 32-48 kHz y 160 kbps a 16-24 kHz. Sin `SetAudioSampleRate`, toma la frecuencia más cercana a la de la fuente entre las que admiten su tasa de bits, por ejemplo 32 kHz para 192 kbps desde una fuente de 22,05 kHz.

## Métodos de IFFmpegSinkPinSettings2

`IFFmpegSinkPinSettings2` deriva de `IFFmpegSinkPinSettings`, de modo que un solo puntero da acceso a ambas. Consúltela en el pin de entrada conectado.

### SetCodecOption

```cpp
HRESULT SetCodecOption(LPCSTR key, LPCSTR value);
```

Establece una opción del codificador de este pin por su nombre de AVOption de FFmpeg: las genéricas (`g`, `b`, `maxrate`, `bufsize`, `flags`, `global_quality`, ...) y las propias del codificador (`deadline`, `cpu-used`, `vbr_bitrate`, ...). Los nombres son los que lista `ffmpeg -h encoder=<nombre>`. Los especificadores de flujo de la línea de comandos, como `b:v` o `q:a`, no son nombres de opción: escriba `b` o `global_quality`.

- Una opción prevalece sobre los demás ajustes del pin: `g` = `10` gana a `SetVideoGOPSize(24)`.
- Volver a establecer la misma clave reemplaza su valor.
- Una opción que el codificador no conoce hace fallar la apertura; nunca se descarta en silencio.
- Las opciones que cambiarían el formato de fotograma que el pin toma de su conexión devuelven `E_INVALIDARG` de inmediato: `pixel_format`, `video_size`, `time_base`, `ar`, `ch_layout`, y las grafías de la línea de comandos `pix_fmt`, `s`, `ac`, `sample_fmt`. Establezca el tamaño, la frecuencia de muestreo y los canales con `SetVideoWidth`/`SetVideoHeight`, `SetAudioSampleRate` y `SetAudioChannelMask`.

**`frame_size`** (tamaño del fotograma de audio en muestras) solo se mantiene donde el codificador conserva el tamaño que recibe:

| Codificador | `frame_size` |
|---|---|
| FLAC | Se acepta: el codificador escribe bloques de esa cantidad de muestras (por ejemplo 1152 o 4096). |
| AAC, Opus, MP3, MP2, Vorbis, Speex | Hace fallar la apertura salvo que sea igual al tamaño propio del codificador. Opus fija su tamaño con `frame_duration` (milisegundos): use esa opción. |
| PCM, y cualquier codificador que acepte fotogramas de cualquier tamaño | Hace fallar la apertura (rellenaría el último fotograma con silencio hasta ese tamaño). |
| Cualquier codificador de video | Hace fallar la apertura. |

`0` pide el tamaño propio del codificador en todos los codificadores. El valor se lee como lo lee FFmpeg, así que `0x480` es 1152. Un tamaño de fotograma mayor también descarta más audio en una discontinuidad de la entrada (un salto en las marcas de tiempo de la fuente), donde se descarta la parte que no llega a un fotograma del codificador.

**Opciones añadidas a la compilación de FFmpeg de este filtro** (2026.11.25.1), para los ajustes de los anteriores filtros WebM y Ogg:

| Codificador | Opción | Valores | Efecto |
|---|---|---|---|
| VP8, VP9 (libvpx) | `kf-mode` | `auto`, `disabled` | `disabled` no coloca fotogramas clave después del primero, también en VP9, que de otro modo fuerza uno cada `kf_max_dist` fotogramas. |
| VP8 | `resize-allowed` | `0`, `1` | Permite al codificador reducir el tamaño de imagen cuando falta tasa de bits. |
| VP8 | `resize-up-thresh` | 0 a 100 | Nivel del búfer, en porcentaje, por encima del cual el codificador vuelve a escalar hacia arriba. |
| VP8 | `resize-down-thresh` | 0 a 100 | Nivel del búfer, en porcentaje, por debajo del cual el codificador escala hacia abajo. |
| VP8 | `buf-optimal-sz` | milisegundos | El nivel del búfer del decodificador al que apunta el control de tasa. |
| Speex | `vbr_bitrate` | bits por segundo | VBR con la mayor calidad cuya tasa cabe en esta tasa de bits, limitado a ella. El estéreo añade unos 800 bps de información lateral. |
| Speex | `vbr_max_bitrate` | bits por segundo, 0 = sin límite | Limita un flujo VBR: con `vbr_bitrate`, o con `flags` = `+qscale` y `global_quality`. |

Sin establecer (el valor predeterminado -1 o 0), cada una mantiene el comportamiento propio de FFmpeg.

### SetVideoDecimate

```cpp
HRESULT SetVideoDecimate(LONG decimate);
```

Conserva el primero de cada `decimate` fotogramas de video y descarta el resto; los fotogramas conservados mantienen sus propias marcas de tiempo. `0` o `1` conserva todos los fotogramas. Úselo para grabar una cámara de 25 fps a 5 fps (`decimate` = 5) sin un filtro de frecuencia de fotogramas en el grafo.

Al codificador se le indica la frecuencia de los fotogramas conservados, así que la tasa de bits establecida en el pin se mantiene. El GOP establecido con `SetVideoGOPSize` cuenta fotogramas de entrada y se redondea al fotograma conservado más cercano, así que el intervalo de fotogramas clave en el tiempo también se mantiene; una opción de códec `g` cuenta fotogramas conservados. El ajuste se ignora en un pin de audio.

### SetAudioFilter

```cpp
HRESULT SetAudioFilter(LPCSTR graph);
```

Pasa el audio del pin por un grafo de filtros FFmpeg, escrito como para la opción `ffmpeg -af`, antes del remuestreador y del codificador. Una cadena vacía lo elimina. Un grafo que FFmpeg no puede construir hace fallar la apertura. El ajuste se ignora en un pin de video.

| Grafo | Uso |
|---|---|
| `speechnorm` | Control automático de ganancia para voz (sustituye al AGC de Speex). |
| `afftdn` | Reducción de ruido (sustituye al reductor de ruido de Speex). |
| `speechnorm,afftdn=nr=20` | Ambos, encadenados. |
| `highpass=f=100` | Elimina el retumbo por debajo de 100 Hz. |

En una discontinuidad de la entrada, el grafo se vacía sobre la línea de tiempo anterior y se reinicia, así que los filtros con memoria, como `speechnorm`, vuelven a empezar en ese punto.

## Definiciones de la interfaz

### Definición C++

El encabezado completo, `IFFmpegSinkPinSettings.h`, se distribuye con el filtro; la nueva interfaz es:

```cpp
MIDL_INTERFACE("1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B")
IFFmpegSinkPinSettings2 : IFFmpegSinkPinSettings {
    STDMETHOD(SetCodecOption)(LPCSTR key, LPCSTR value) = 0;
    STDMETHOD(SetVideoDecimate)(LONG decimate) = 0;
    STDMETHOD(SetAudioFilter)(LPCSTR graph) = 0;
};

_COM_SMARTPTR_TYPEDEF(IFFmpegSinkPinSettings2, __uuidof(IFFmpegSinkPinSettings2));
```

### Definición C\#

Una interfaz COM en C# enumera primero los métodos de sus interfaces base, en su orden. Los parámetros de perfil y nivel se declaran aquí como `int`; sus valores son los de `FFMPEG_SINK_VIDEO_PROFILE` y `FFMPEG_SINK_VIDEO_LEVEL` en el encabezado C++ (`0` = automático).

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    public enum FFMPEG_SINK_FORMAT
    {
        AUTO, MP4, MKV, FLV, OGG, MPEG_TS, VOB, AVI, WEBM, M4A, MOV, MPEG1, MPEG2, MPEG_TS_KLV, FLAC
    }

    public enum FFMPEG_SINK_VIDEO_CODEC
    {
        AUTO, H264_MF, H264_QSV, H264_NVENC, H264_AMF, HEVC_MF, HEVC_QSV, HEVC_NVENC, HEVC_AMF,
        MPEG1, MPEG2, VP8, VP9, MPEG4, OPENH264
    }

    public enum FFMPEG_SINK_AUDIO_CODEC
    {
        AUTO, MP2, MP3, AAC, FLAC, VORBIS, OPUS, SPEEX
    }

    public enum FFMPEG_SINK_SUBTITLE_CODEC
    {
        AUTO, SUBRIP, ASS
    }

    [ComImport]
    [Guid("CAEC87C3-24B5-404C-9985-FA18D2B78A4A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFFmpegSinkSettings
    {
        [PreserveSig] int SetFormat(FFMPEG_SINK_FORMAT format);
    }

    [ComImport]
    [Guid("1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFFmpegSinkPinSettings2
    {
        // IFFmpegSinkPinSettings
        [PreserveSig] int SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC codec);
        [PreserveSig] int SetVideoBitRate(int bitRate);
        [PreserveSig] int SetVideoWidth(int width);
        [PreserveSig] int SetVideoHeight(int height);
        [PreserveSig] int SetVideoProfile(int profile);
        [PreserveSig] int SetVideoLevel(int level);
        [PreserveSig] int SetVideoGOPSize(int gopSize);
        [PreserveSig] int SetVideoMaxBFrames(int maxBFrames);
        [PreserveSig] int SetVideoThreadCount(int threadCount);
        [PreserveSig] int SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC codec);
        [PreserveSig] int SetAudioBitRate(int bitRate);
        [PreserveSig] int SetAudioSampleRate(int sampleRate);
        [PreserveSig] int SetAudioChannelMask(uint channelMask);
        [PreserveSig] int SetSubtitleCodec(FFMPEG_SINK_SUBTITLE_CODEC codec);

        // IFFmpegSinkPinSettings2
        [PreserveSig] int SetCodecOption([MarshalAs(UnmanagedType.LPStr)] string key,
                                         [MarshalAs(UnmanagedType.LPStr)] string value);
        [PreserveSig] int SetVideoDecimate(int decimate);
        [PreserveSig] int SetAudioFilter([MarshalAs(UnmanagedType.LPStr)] string graph);
    }
}
```

`IFFmpegSinkSettings` declara aquí solo su primer método; tiene un segundo, `WriteKLVData`, para la salida MPEG-TS KLV.

## Ejemplos de uso

Los ejemplos empiezan una vez que la fuente está conectada a FFMPEG Encoder, después de `IFileSinkFilter::SetFileName` e `IFFmpegSinkSettings::SetFormat`, y antes de `IMediaControl::Run`.

### Ejemplo C++: WebM VP8 a 5 fps, sin fotogramas clave después del primero

```cpp
#include <dshow.h>
#include "IFFmpegSinkPinSettings.h"

// Para cada pin de entrada conectado de FFMPEG Encoder:
HRESULT ConfigureWebMPin(IPin* pin)
{
    AM_MEDIA_TYPE mt = {};
    HRESULT hr = pin->ConnectionMediaType(&mt);
    if (FAILED(hr)) return hr;
    const bool isVideo = mt.majortype == MEDIATYPE_Video;
    FreeMediaType(mt);

    IFFmpegSinkPinSettings2Ptr settings;
    hr = pin->QueryInterface(IID_PPV_ARGS(&settings));
    if (FAILED(hr)) return hr;   // un filtro anterior a 2026.11.25.1

    if (isVideo) {
        settings->SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC_VP8);
        settings->SetVideoBitRate(500000);
        settings->SetVideoDecimate(5);                       // 25 fps de entrada, 5 fps de salida
        settings->SetCodecOption("deadline", "realtime");
        settings->SetCodecOption("kf-mode", "disabled");
    } else {
        settings->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_VORBIS);
        settings->SetAudioBitRate(96000);
    }
    return S_OK;
}
```

### Ejemplo C++: grabación de voz Ogg Speex con AGC y reducción de ruido

```cpp
// Formato: IFFmpegSinkSettings::SetFormat(FFMPEG_SINK_FORMAT_OGG)
IFFmpegSinkPinSettings2Ptr audio;
hr = audioPin->QueryInterface(IID_PPV_ARGS(&audio));
if (SUCCEEDED(hr)) {
    audio->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_SPEEX);
    audio->SetAudioSampleRate(16000);                        // banda ancha
    audio->SetCodecOption("vbr_bitrate", "16000");           // VBR, como máximo 16 kbps
    audio->SetAudioFilter("speechnorm,afftdn");
}
```

### Ejemplo C++: archivo FLAC con bloques de 4096 muestras

```cpp
// Formato: IFFmpegSinkSettings::SetFormat(FFMPEG_SINK_FORMAT_FLAC)
IFFmpegSinkPinSettings2Ptr audio;
hr = audioPin->QueryInterface(IID_PPV_ARGS(&audio));
if (SUCCEEDED(hr)) {
    audio->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_FLAC);
    audio->SetCodecOption("frame_size", "4096");
    audio->SetCodecOption("compression_level", "8");
}
```

### Ejemplo C#: opciones en los pines del codificador

Este fragmento usa DirectShowLib y las definiciones anteriores.

```csharp
using System;
using System.Runtime.InteropServices;
using DirectShowLib;
using VisioForge.DirectShowAPI;

// encoder: el IBaseFilter de FFMPEG Encoder, ya conectado a la fuente.
void ConfigurePins(IBaseFilter encoder)
{
    encoder.EnumPins(out IEnumPins pins);
    var pin = new IPin[1];
    while (pins.Next(1, pin, IntPtr.Zero) == 0)
    {
        var mt = new AMMediaType();
        if (pin[0].ConnectionMediaType(mt) == 0 && pin[0] is IFFmpegSinkPinSettings2 settings)
        {
            if (mt.majorType == MediaType.Video)
            {
                settings.SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC.VP9);
                settings.SetCodecOption("deadline", "realtime");
                settings.SetCodecOption("cpu-used", "8");
            }
            else if (mt.majorType == MediaType.Audio)
            {
                settings.SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC.OPUS);
                settings.SetAudioFilter("speechnorm");
            }
        }

        DsUtils.FreeAMMediaType(mt);
        Marshal.ReleaseComObject(pin[0]);
    }

    Marshal.ReleaseComObject(pins);
}
```

## Solución de problemas

### Run falla después de establecer una opción

Observe la salida de depuración (DebugView) en busca de `Session open failed:`. La línea indica la causa:

- `<encoder> has no option '<key>'`: el nombre no es válido para este codificador, o se usó una grafía de la línea de comandos como `b:v`. Consulte `ffmpeg -h encoder=<nombre>`.
- `<encoder> does not take frame_size <n>`: consulte la tabla de `frame_size` anterior; el mismo texto indica también un valor que no es un número a partir de 0, como `-1` o `abc`.
- Un texto de error de FFmpeg, como `Invalid argument`: el valor está fuera del rango de la opción, o el grafo de filtros de audio no se construye.

### SetCodecOption devuelve E_INVALIDARG

La clave cambia el formato de fotograma que el pin obtiene de su conexión. Use el método propio del pin: `SetVideoWidth`/`SetVideoHeight`, `SetAudioSampleRate`, `SetAudioChannelMask`.

### QueryInterface de IFFmpegSinkPinSettings2 devuelve E_NOINTERFACE

El `VisioForge_FFMPEG_Encoder.ax` instalado es anterior a 2026.11.25.1. Actualice el filtro junto con las DLL de FFmpeg que lo acompañan.

## Ver también

- [Interfaz de FFmpeg Encoder (IVFFFMPEGEncoder)](ffmpeg.md)
- [Archivos redistribuibles](../../deployment/redistributable-files.md#codificador-ffmpeg)
- [Resumen del Encoding Filters Pack](../index.md)
