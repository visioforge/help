---
title: Referencia de la Interfaz del Codificador AAC
description: Interfaces DirectShow del codificador AAC: control de tasa de bits, configuración de perfil, frecuencia de muestreo y codificación para audio profesional.
---

# Referencia de la Interfaz del Codificador AAC

## Descripción General

Los filtros DirectShow del codificador AAC (Codificación de Audio Avanzada) proporcionan interfaces para la codificación de audio de alta calidad al formato AAC. AAC es el sucesor de MP3, ofreciendo mejor calidad de sonido a la misma tasa de bits y es el códec de audio estándar para MP4, M4A y aplicaciones de transmisión.

Están disponibles dos interfaces de codificador AAC:
- **IMonogramAACEncoder**: Interfaz de configuración simple que utiliza una única estructura de configuración.
- **IVFAACEncoder**: Interfaz completa con métodos de propiedad individuales para un control detallado.

## Interfaz IMonogramAACEncoder

### Descripción General

La interfaz **IMonogramAACEncoder** proporciona un enfoque de configuración simple basado en estructuras para la codificación AAC. La configuración se realiza utilizando la estructura `AACConfig` que contiene todos los parámetros esenciales de codificación.

**GUID de la Interfaz**: `{B2DE30C0-1441-4451-A0CE-A914FD561D7F}`

**Hereda de**: `IUnknown`

### Estructura AACConfig

```csharp
/// <summary>
/// Estructura de configuración del codificador AAC.
/// </summary>
public struct AACConfig
{
    /// <summary>
    /// Versión/perfil AAC (típicamente 2 para AAC-LC, 4 para AAC-HE)
    /// </summary>
    public int version;

    /// <summary>
    /// Tipo de objeto / perfil:
    /// 2 = AAC-LC (Baja Complejidad) - recomendado para la mayoría de usos
    /// 5 = AAC-HE (Alta Eficiencia)
    /// 29 = AAC-HEv2 (Alta Eficiencia versión 2)
    /// </summary>
    public int object_type;

    /// <summary>
    /// Tipo de formato de salida (0 = AAC Raw, 1 = ADTS)
    /// </summary>
    public int output_type;

    /// <summary>
    /// Tasa de bits objetivo en bits por segundo (ej., 128000 para 128 kbps)
    /// </summary>
    public int bitrate;
}
```

### Estructura AACInfo

```csharp
/// <summary>
/// Información en tiempo de ejecución del codificador AAC.
/// </summary>
public struct AACInfo
{
    /// <summary>
    /// Frecuencia de muestreo de entrada en Hz (ej., 44100, 48000)
    /// </summary>
    public int samplerate;

    /// <summary>
    /// Número de canales de audio (1 = mono, 2 = estéreo, 6 = 5.1, etc.)
    /// </summary>
    public int channels;

    /// <summary>
    /// Tamaño del marco AAC en muestras (típicamente 1024 para AAC-LC)
    /// </summary>
    public int frame_size;

    /// <summary>
    /// Número total de marcos codificados
    /// </summary>
    public long frames_done;
}
```

### Definiciones de Interfaz

#### Definición en C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Estructura de configuración del codificador AAC.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AACConfig
    {
        public int version;
        public int object_type;
        public int output_type;
        public int bitrate;
    }

    /// <summary>
    /// Información en tiempo de ejecución del codificador AAC.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AACInfo
    {
        public int samplerate;
        public int channels;
        public int frame_size;
        public long frames_done;
    }

    /// <summary>
    /// Interfaz de configuración del codificador Monogram AAC.
    /// Proporciona configuración basada en estructuras para la codificación AAC.
    /// </summary>
    [ComImport]
    [Guid("B2DE30C0-1441-4451-A0CE-A914FD561D7F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMonogramAACEncoder
    {
        /// <summary>
        /// Obtiene la configuración actual del codificador AAC.
        /// </summary>
        /// <param name="config">Referencia a la estructura AACConfig para recibir la configuración actual</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetConfig(ref AACConfig config);

        /// <summary>
        /// Establece la configuración del codificador AAC.
        /// </summary>
        /// <param name="config">Referencia a la estructura AACConfig que contiene la configuración deseada</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetConfig(ref AACConfig config);
    }
}
```

#### Definición en C++

```cpp
#include <unknwn.h>

// {B2DE30C0-1441-4451-A0CE-A914FD561D7F}
DEFINE_GUID(IID_IMonogramAACEncoder,
    0xb2de30c0, 0x1441, 0x4451, 0xa0, 0xce, 0xa9, 0x14, 0xfd, 0x56, 0x1d, 0x7f);

/// <summary>
/// Estructura de configuración del codificador AAC.
/// </summary>
struct AACConfig
{
    int version;
    int object_type;
    int output_type;
    int bitrate;
};

/// <summary>
/// Información en tiempo de ejecución del codificador AAC.
/// </summary>
struct AACInfo
{
    int samplerate;
    int channels;
    int frame_size;
    __int64 frames_done;
};

/// <summary>
/// Interfaz de configuración del codificador Monogram AAC.
/// </summary>
DECLARE_INTERFACE_(IMonogramAACEncoder, IUnknown)
{
    /// <summary>
    /// Obtiene la configuración actual del codificador AAC.
    /// </summary>
    /// <param name="config">Puntero a la estructura AACConfig para recibir la configuración</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(GetConfig)(THIS_
        AACConfig* config
        ) PURE;

    /// <summary>
    /// Establece la configuración del codificador AAC.
    /// </summary>
    /// <param name="config">Puntero a la estructura AACConfig con la configuración deseada</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(SetConfig)(THIS_
        const AACConfig* config
        ) PURE;
};
```

#### Definición en Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IMonogramAACEncoder: TGUID = '{B2DE30C0-1441-4451-A0CE-A914FD561D7F}';

type
  /// <summary>
  /// Estructura de configuración del codificador AAC.
  /// </summary>
  TAACConfig = record
    version: Integer;
    object_type: Integer;
    output_type: Integer;
    bitrate: Integer;
  end;

  /// <summary>
  /// Información en tiempo de ejecución del codificador AAC.
  /// </summary>
  TAACInfo = record
    samplerate: Integer;
    channels: Integer;
    frame_size: Integer;
    frames_done: Int64;
  end;

  /// <summary>
  /// Interfaz de configuración del codificador Monogram AAC.
  /// </summary>
  IMonogramAACEncoder = interface(IUnknown)
    ['{B2DE30C0-1441-4451-A0CE-A914FD561D7F}']

    /// <summary>
    /// Obtiene la configuración actual del codificador AAC.
    /// </summary>
    function GetConfig(var config: TAACConfig): HRESULT; stdcall;

    /// <summary>
    /// Establece la configuración del codificador AAC.
    /// </summary>
    function SetConfig(const config: TAACConfig): HRESULT; stdcall;
  end;
```

---
## Interfaz IVFAACEncoder
### Descripción General
La interfaz **IVFAACEncoder** proporciona una configuración completa basada en propiedades para la codificación AAC con métodos getter/setter individuales para cada parámetro. Esta interfaz ofrece un control más fino y es más fácil de usar para cambios de configuración incrementales.
**GUID de la Interfaz**: `{0BEF7533-39E6-42a5-863F-E087FAB5D84F}`
**Hereda de**: `IUnknown`
### Definiciones de Interfaz
#### Definición en C#
```csharp
using System;
using System.Runtime.InteropServices;
namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interfaz de configuración del codificador VisioForge AAC.
    /// Proporciona control completo basado en propiedades sobre los parámetros de codificación AAC.
    /// </summary>
    [ComImport]
    [Guid("0BEF7533-39E6-42a5-863F-E087FAB5D84F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFAACEncoder
    {
        /// <summary>
        /// Fuerza una frecuencia de muestreo de entrada específica. Establecer en 0 para aceptar cualquier frecuencia.
        /// </summary>
        /// <param name="ulSampleRate">Frecuencia de muestreo en Hz (ej., 44100, 48000). 0 = cualquier frecuencia</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetInputSampleRate(uint ulSampleRate);
        /// <summary>
        /// Obtiene la frecuencia de muestreo de entrada configurada.
        /// </summary>
        /// <param name="pulSampleRate">Recibe la frecuencia de muestreo en Hz. 0 si no está fija</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetInputSampleRate(out uint pulSampleRate);
        /// <summary>
        /// Establece el número de canales de entrada.
        /// </summary>
        /// <param name="nChannels">Número de canales (1=mono, 2=estéreo, 6=5.1, etc.)</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetInputChannels(short nChannels);
        /// <summary>
        /// Obtiene el número de canales de entrada.
        /// </summary>
        /// <param name="pnChannels">Recibe el número de canales</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetInputChannels(out short pnChannels);
        /// <summary>
        /// Establece la tasa de bits objetivo. Establecer en -1 para usar la tasa de bits máxima.
        /// </summary>
        /// <param name="ulBitRate">Tasa de bits en bits por segundo (ej., 128000). -1 = máximo</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetBitRate(uint ulBitRate);
        /// <summary>
        /// Obtiene la tasa de bits configurada.
        /// </summary>
        /// <param name="pulBitRate">Recibe la tasa de bits en bps. -1 si está configurado al máximo</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetBitRate(out uint pulBitRate);
        /// <summary>
        /// Establece el tipo de perfil AAC.
        /// </summary>
        /// <param name="uProfile">Perfil: 2=AAC-LC, 5=AAC-HE, 29=AAC-HEv2</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetProfile(uint uProfile);
        /// <summary>
        /// Obtiene el perfil AAC actual.
        /// </summary>
        /// <param name="puProfile">Recibe el tipo de perfil</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetProfile(out uint puProfile);
        /// <summary>
        /// Establece el formato de salida.
        /// </summary>
        /// <param name="uFormat">Formato: 0=AAC Raw, 1=ADTS</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetOutputFormat(uint uFormat);
        /// <summary>
        /// Obtiene el formato de salida.
        /// </summary>
        /// <param name="puFormat">Recibe el formato de salida</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetOutputFormat(out uint puFormat);
        /// <summary>
        /// Establece el valor de desplazamiento de tiempo para el ajuste de marca de tiempo.
        /// </summary>
        /// <param name="timeShift">Desplazamiento de tiempo en milisegundos</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetTimeShift(int timeShift);
        /// <summary>
        /// Obtiene el valor de desplazamiento de tiempo.
        /// </summary>
        /// <param name="ptimeShift">Recibe el desplazamiento de tiempo en milisegundos</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetTimeShift(out int ptimeShift);
        /// <summary>
        /// Habilita o deshabilita el canal de Efectos de Baja Frecuencia (LFE).
        /// </summary>
        /// <param name="lfe">1 para habilitar LFE, 0 para deshabilitar</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetLFE(uint lfe);
        /// <summary>
        /// Obtiene el estado del canal LFE.
        /// </summary>
        /// <param name="p">Recibe el estado LFE (1=habilitado, 0=deshabilitado)</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetLFE(out uint p);
        /// <summary>
        /// Habilita o deshabilita el Modelado de Ruido Temporal (TNS).
        /// TNS mejora la codificación de sonidos transitorios.
        /// </summary>
        /// <param name="tns">1 para habilitar TNS, 0 para deshabilitar</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetTNS(uint tns);
        /// <summary>
        /// Obtiene el estado de TNS.
        /// </summary>
        /// <param name="p">Recibe el estado TNS (1=habilitado, 0=deshabilitado)</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetTNS(out uint p);
        /// <summary>
        /// Habilita o deshabilita la codificación estéreo Mid-Side.
        /// Puede mejorar la compresión para audio estéreo.
        /// </summary>
        /// <param name="v">1 para habilitar codificación mid-side, 0 para deshabilitar</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetMidSide(uint v);
        /// <summary>
        /// Obtiene el estado de codificación mid-side.
        /// </summary>
        /// <param name="p">Recibe el estado mid-side (1=habilitado, 0=deshabilitado)</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetMidSide(out uint p);
    }
}
```
#### Definición en C++
```cpp
#include <unknwn.h>
// {0BEF7533-39E6-42a5-863F-E087FAB5D84F}
DEFINE_GUID(IID_IVFAACEncoder,
    0x0bef7533, 0x39e6, 0x42a5, 0x86, 0x3f, 0xe0, 0x87, 0xfa, 0xb5, 0xd8, 0x4f);
/// <summary>
/// Interfaz de configuración del codificador VisioForge AAC.
/// </summary>
DECLARE_INTERFACE_(IVFAACEncoder, IUnknown)
{
    STDMETHOD(SetInputSampleRate)(THIS_
        unsigned long ulSampleRate
        ) PURE;
    STDMETHOD(GetInputSampleRate)(THIS_
        unsigned long* pulSampleRate
        ) PURE;
    STDMETHOD(SetInputChannels)(THIS_
        short nChannels
        ) PURE;
    STDMETHOD(GetInputChannels)(THIS_
        short* pnChannels
        ) PURE;
    STDMETHOD(SetBitRate)(THIS_
        unsigned long ulBitRate
        ) PURE;
    STDMETHOD(GetBitRate)(THIS_
        unsigned long* pulBitRate
        ) PURE;
    STDMETHOD(SetProfile)(THIS_
        unsigned long uProfile
        ) PURE;
    STDMETHOD(GetProfile)(THIS_
        unsigned long* puProfile
        ) PURE;
    STDMETHOD(SetOutputFormat)(THIS_
        unsigned long uFormat
        ) PURE;
    STDMETHOD(GetOutputFormat)(THIS_
        unsigned long* puFormat
        ) PURE;
    STDMETHOD(SetTimeShift)(THIS_
        int timeShift
        ) PURE;
    STDMETHOD(GetTimeShift)(THIS_
        int* ptimeShift
        ) PURE;
    STDMETHOD(SetLFE)(THIS_
        unsigned long lfe
        ) PURE;
    STDMETHOD(GetLFE)(THIS_
        unsigned long* p
        ) PURE;
    STDMETHOD(SetTNS)(THIS_
        unsigned long tns
        ) PURE;
    STDMETHOD(GetTNS)(THIS_
        unsigned long* p
        ) PURE;
    STDMETHOD(SetMidSide)(THIS_
        unsigned long v
        ) PURE;
    STDMETHOD(GetMidSide)(THIS_
        unsigned long* p
        ) PURE;
};
```
#### Definición en Delphi
```delphi
uses
  ActiveX, ComObj;
const
  IID_IVFAACEncoder: TGUID = '{0BEF7533-39E6-42a5-863F-E087FAB5D84F}';
type
  /// <summary>
  /// Interfaz de configuración del codificador VisioForge AAC.
  /// </summary>
  IVFAACEncoder = interface(IUnknown)
    ['{0BEF7533-39E6-42a5-863F-E087FAB5D84F}']
    function SetInputSampleRate(ulSampleRate: Cardinal): HRESULT; stdcall;
    function GetInputSampleRate(out pulSampleRate: Cardinal): HRESULT; stdcall;
    function SetInputChannels(nChannels: SmallInt): HRESULT; stdcall;
    function GetInputChannels(out pnChannels: SmallInt): HRESULT; stdcall;
    function SetBitRate(ulBitRate: Cardinal): HRESULT; stdcall;
    function GetBitRate(out pulBitRate: Cardinal): HRESULT; stdcall;
    function SetProfile(uProfile: Cardinal): HRESULT; stdcall;
    function GetProfile(out puProfile: Cardinal): HRESULT; stdcall;
    function SetOutputFormat(uFormat: Cardinal): HRESULT; stdcall;
    function GetOutputFormat(out puFormat: Cardinal): HRESULT; stdcall;
    function SetTimeShift(timeShift: Integer): HRESULT; stdcall;
    function GetTimeShift(out ptimeShift: Integer): HRESULT; stdcall;
    function SetLFE(lfe: Cardinal): HRESULT; stdcall;
    function GetLFE(out p: Cardinal): HRESULT; stdcall;
    function SetTNS(tns: Cardinal): HRESULT; stdcall;
    function GetTNS(out p: Cardinal): HRESULT; stdcall;
    function SetMidSide(v: Cardinal): HRESULT; stdcall;
    function GetMidSide(out p: Cardinal): HRESULT; stdcall;
  end;
```
## Perfiles AAC y Configuración
### Perfiles AAC
**AAC-LC (Baja Complejidad) - Perfil 2** (Recomendado):
- Mejor relación calidad-tasa de bits
- Menor complejidad computacional
- Soporte universal de decodificadores
- Uso para: Música, podcasts, bandas sonoras de video
- Rango de tasa de bits: 64-320 kbps
**AAC-HE (Alta Eficiencia) - Perfil 5**:
- Optimizado para tasas de bits bajas
- Utiliza Replicación de Banda Espectral (SBR)
- Mejor calidad que AAC-LC a tasas de bits bajas (<= 64 kbps)
- Uso para: Transmisión, voz, aplicaciones de baja tasa de bits
- Rango de tasa de bits: 32-80 kbps
**AAC-HEv2 (Alta Eficiencia versión 2) - Perfil 29**:
- Optimizado aún más para tasas de bits muy bajas
- Utiliza Estéreo Paramétrico (PS) además de SBR
- Mejor para mono/estéreo a tasas de bits extremadamente bajas
- Uso para: Transmisión de voz, ancho de banda muy bajo
- Rango de tasa de bits: 16-40 kbps
### Formatos de Salida
**AAC Raw (Formato 0)**:
- Flujo de bits AAC puro sin contenedor
- Requiere contenedor externo (MP4, M4A, MKV)
- Uso para: Multiplexación en archivos MP4/M4A
- Tamaño de salida más pequeño
**ADTS (Flujo de Transporte de Datos de Audio) - Formato 1**:
- AAC con encabezados de marco
- Autónomo, se puede reproducir directamente
- Ligeramente más grande que AAC raw
- Uso para: Archivos AAC independientes, transmisión
- Mejor resistencia a errores
### Tasas de Bits Recomendadas
| Caso de Uso | Canales | Perfil | Tasa de Bits | Notas |
|----------|----------|---------|---------|-------|
| Voz/Podcast (mono) | 1 | AAC-LC | 64-96 kbps | Voz clara |
| Voz/Podcast (estéreo) | 2 | AAC-LC | 96-128 kbps | Voz de alta calidad |
| Música (estéreo) estándar | 2 | AAC-LC | 128-192 kbps | Buena calidad |
| Música (estéreo) alta calidad | 2 | AAC-LC | 256-320 kbps | Calidad excelente |
| Transmisión de bajo ancho de banda | 2 | AAC-HE | 48-64 kbps | Calidad aceptable |
| Ancho de banda muy bajo | 1-2 | AAC-HEv2 | 24-40 kbps | Calidad básica |
| Sonido envolvente 5.1 | 6 | AAC-LC | 384-512 kbps | Calidad de cine |
## Ejemplos de Uso
### Ejemplo en C# - IMonogramAACEncoder (Música de Alta Calidad)
```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;
public class MonogramAACHighQuality
{
    public void ConfigureHighQualityMusic(IBaseFilter audioEncoder)
    {
        // Consultar la interfaz del codificador Monogram AAC
        var aacEncoder = audioEncoder as IMonogramAACEncoder;
        if (aacEncoder == null)
        {
            Console.WriteLine("Error: El filtro no soporta IMonogramAACEncoder");
            return;
        }
        // Configurar codificación de música estéreo de alta calidad
        var config = new AACConfig
        {
            version = 2,            // Versión AAC 2
            object_type = 2,        // Perfil AAC-LC
            output_type = 0,        // AAC Raw (para multiplexación MP4)
            bitrate = 192000        // 192 kbps
        };
        int hr = aacEncoder.SetConfig(ref config);
        if (hr == 0)
        {
            Console.WriteLine("Codificador AAC configurado para música de alta calidad:");
            Console.WriteLine("  Perfil: AAC-LC");
            Console.WriteLine("  Tasa de bits: 192 kbps");
            Console.WriteLine("  Salida: AAC Raw para contenedor MP4");
        }
        else
        {
            Console.WriteLine($"Error configurando el codificador AAC: 0x{hr:X8}");
        }
    }
}
```
### Ejemplo en C# - IVFAACEncoder (Configuración Completa)
```csharp
public class VFAACHighQualityMusic
{
    public void ConfigureComprehensive(IBaseFilter audioEncoder)
    {
        // Consultar la interfaz del codificador VisioForge AAC
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
        {
            Console.WriteLine("Error: El filtro no soporta IVFAACEncoder");
            return;
        }
        // Configurar codificación de música estéreo completa
        vfAacEncoder.SetInputSampleRate(48000);     // 48 kHz
        vfAacEncoder.SetInputChannels(2);            // Estéreo
        vfAacEncoder.SetBitRate(256000);            // 256 kbps
        vfAacEncoder.SetProfile(2);                 // AAC-LC
        vfAacEncoder.SetOutputFormat(0);            // AAC Raw
        vfAacEncoder.SetTNS(1);                     // Habilitar TNS
        vfAacEncoder.SetMidSide(1);                 // Habilitar codificación mid-side
        vfAacEncoder.SetLFE(0);                     // Sin LFE (solo estéreo)
        vfAacEncoder.SetTimeShift(0);               // Sin desplazamiento de tiempo
        Console.WriteLine("Codificador VisioForge AAC configurado:");
        // Verificar configuración
        vfAacEncoder.GetBitRate(out uint bitrate);
        vfAacEncoder.GetProfile(out uint profile);
        vfAacEncoder.GetInputChannels(out short channels);
        Console.WriteLine($"  Tasa de bits: {bitrate / 1000} kbps");
        Console.WriteLine($"  Perfil: {(profile == 2 ? "AAC-LC" : profile.ToString())}");
        Console.WriteLine($"  Canales: {channels}");
    }
}
```
### Ejemplo en C# - Transmisión de Baja Tasa de Bits (AAC-HE)
```csharp
public class VFAACLowBitrateStreaming
{
    public void ConfigureLowBitrate(IBaseFilter audioEncoder)
    {
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
            return;
        // Configurar para transmisión de baja tasa de bits
        vfAacEncoder.SetInputSampleRate(44100);     // 44.1 kHz
        vfAacEncoder.SetInputChannels(2);            // Estéreo
        vfAacEncoder.SetBitRate(64000);             // 64 kbps
        vfAacEncoder.SetProfile(5);                 // AAC-HE (Alta Eficiencia)
        vfAacEncoder.SetOutputFormat(1);            // ADTS para transmisión
        vfAacEncoder.SetTNS(1);                     // Habilitar TNS
        vfAacEncoder.SetMidSide(1);                 // Habilitar mid-side
        vfAacEncoder.SetLFE(0);                     // Sin LFE
        Console.WriteLine("AAC-HE configurado para transmisión de baja tasa de bits");
        Console.WriteLine("  64 kbps estéreo con salida ADTS");
    }
}
```
### Ejemplo en C# - Codificación de Voz/Podcast
```csharp
public class VFAACVoicePodcast
{
    public void ConfigureVoicePodcast(IBaseFilter audioEncoder)
    {
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
            return;
        // Configurar para voz/podcast (mono)
        vfAacEncoder.SetInputSampleRate(44100);     // 44.1 kHz
        vfAacEncoder.SetInputChannels(1);            // Mono
        vfAacEncoder.SetBitRate(80000);             // 80 kbps
        vfAacEncoder.SetProfile(2);                 // AAC-LC
        vfAacEncoder.SetOutputFormat(0);            // AAC Raw
        vfAacEncoder.SetTNS(1);                     // Habilitar TNS para voz
        vfAacEncoder.SetMidSide(0);                 // N/A para mono
        vfAacEncoder.SetLFE(0);                     // Sin LFE
        Console.WriteLine("AAC configurado para voz/podcast");
        Console.WriteLine("  80 kbps mono AAC-LC");
    }
}
```
### Ejemplo en C++ - IMonogramAACEncoder
```cpp
#include <dshow.h>
#include <iostream>
#include "IMonogramAACEncoder.h"
void ConfigureMonogramAAC(IBaseFilter* pAudioEncoder)
{
    IMonogramAACEncoder* pAACEncoder = NULL;
    HRESULT hr = S_OK;
    // Consultar la interfaz del codificador Monogram AAC
    hr = pAudioEncoder->QueryInterface(IID_IMonogramAACEncoder,
                                       (void**)&pAACEncoder);
    if (FAILED(hr) || !pAACEncoder)
    {
        std::cout << "Error: El filtro no soporta IMonogramAACEncoder" << std::endl;
        return;
    }
    // Configurar codificación de música de alta calidad
    AACConfig config;
    config.version = 2;         // Versión AAC 2
    config.object_type = 2;     // AAC-LC
    config.output_type = 0;     // AAC Raw
    config.bitrate = 192000;    // 192 kbps
    hr = pAACEncoder->SetConfig(&config);
    if (SUCCEEDED(hr))
    {
        std::cout << "Codificador AAC configurado para música de alta calidad" << std::endl;
        std::cout << "  Perfil: AAC-LC" << std::endl;
        std::cout << "  Tasa de bits: 192 kbps" << std::endl;
    }
    pAACEncoder->Release();
}
```
### Ejemplo en C++ - IVFAACEncoder
```cpp
#include "IVFAACEncoder.h"
void ConfigureVFAAC(IBaseFilter* pAudioEncoder)
{
    IVFAACEncoder* pVFAACEncoder = NULL;
    HRESULT hr = pAudioEncoder->QueryInterface(IID_IVFAACEncoder,
                                               (void**)&pVFAACEncoder);
    if (SUCCEEDED(hr) && pVFAACEncoder)
    {
        // Configurar codificación estéreo completa
        pVFAACEncoder->SetInputSampleRate(48000);   // 48 kHz
        pVFAACEncoder->SetInputChannels(2);          // Estéreo
        pVFAACEncoder->SetBitRate(256000);          // 256 kbps
        pVFAACEncoder->SetProfile(2);               // AAC-LC
        pVFAACEncoder->SetOutputFormat(0);          // AAC Raw
        pVFAACEncoder->SetTNS(1);                   // Habilitar TNS
        pVFAACEncoder->SetMidSide(1);               // Habilitar mid-side
        pVFAACEncoder->SetLFE(0);                   // Sin LFE
        std::cout << "Codificador VisioForge AAC configurado" << std::endl;
        pVFAACEncoder->Release();
    }
}
```
### Ejemplo en Delphi - IMonogramAACEncoder
```delphi
uses
  DirectShow9, ActiveX;
procedure ConfigureMonogramAAC(AudioEncoder: IBaseFilter);
var
  AACEncoder: IMonogramAACEncoder;
  Config: TAACConfig;
  hr: HRESULT;
begin
  // Consultar la interfaz del codificador Monogram AAC
  hr := AudioEncoder.QueryInterface(IID_IMonogramAACEncoder, AACEncoder);
  if Failed(hr) or (AACEncoder = nil) then
  begin
    WriteLn('Error: El filtro no soporta IMonogramAACEncoder');
    Exit;
  end;
  try
    // Configurar codificación de música de alta calidad
    Config.version := 2;         // Versión AAC 2
    Config.object_type := 2;     // AAC-LC
    Config.output_type := 0;     // AAC Raw
    Config.bitrate := 192000;    // 192 kbps
    hr := AACEncoder.SetConfig(Config);
    if Succeeded(hr) then
    begin
      WriteLn('Codificador AAC configurado para música de alta calidad');
      WriteLn('  Perfil: AAC-LC');
      WriteLn('  Tasa de bits: 192 kbps');
    end;
  finally
    AACEncoder := nil;
  end;
end;
```
### Ejemplo en Delphi - IVFAACEncoder
```delphi
procedure ConfigureVFAAC(AudioEncoder: IBaseFilter);
var
  VFAACEncoder: IVFAACEncoder;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IVFAACEncoder, VFAACEncoder)) then
  begin
    try
      // Configurar codificación estéreo completa
      VFAACEncoder.SetInputSampleRate(48000);   // 48 kHz
      VFAACEncoder.SetInputChannels(2);          // Estéreo
      VFAACEncoder.SetBitRate(256000);          // 256 kbps
      VFAACEncoder.SetProfile(2);               // AAC-LC
      VFAACEncoder.SetOutputFormat(0);          // AAC Raw
      VFAACEncoder.SetTNS(1);                   // Habilitar TNS
      VFAACEncoder.SetMidSide(1);               // Habilitar mid-side
      VFAACEncoder.SetLFE(0);                   // Sin LFE
      WriteLn('Codificador VisioForge AAC configurado');
    finally
      VFAACEncoder := nil;
    end;
  end;
end;
```
## Mejores Prácticas
### Selección de Perfil
**Usar AAC-LC (Perfil 2) cuando**:
- Codificación de música o audio de alta calidad
- Tasa de bits >= 96 kbps
- Se requiere máxima compatibilidad de decodificadores
- **Recomendado para la mayoría de escenarios**
**Usar AAC-HE (Perfil 5) cuando**:
- Restricciones de tasa de bits (32-80 kbps)
- Transmisión sobre ancho de banda limitado
- Contenido de voz/habla aceptable a menor calidad
- Aplicaciones de transmisión móvil/web
**Usar AAC-HEv2 (Perfil 29) cuando**:
- Ancho de banda extremadamente limitado (< 40 kbps)
- Contenido solo de voz
- Solo mono o estéreo (no multicanal)
### Pautas de Tasa de Bits
**Voz/Podcast Mono**:
- Mínimo: 48-64 kbps (AAC-LC)
- Recomendado: 80-96 kbps (AAC-LC)
- Alta calidad: 128 kbps (AAC-LC)
**Música Estéreo**:
- Mínimo: 96-128 kbps (AAC-LC)
- Recomendado: 192-256 kbps (AAC-LC)
- Alta calidad: 256-320 kbps (AAC-LC)
**Aplicaciones de Transmisión**:
- Bajo ancho de banda: 48-64 kbps (AAC-HE, estéreo)
- Ancho de banda estándar: 96-128 kbps (AAC-LC, estéreo)
- Alto ancho de banda: 192-256 kbps (AAC-LC, estéreo)
### Selección de Formato de Salida
**Usar AAC Raw (Formato 0) cuando**:
- Multiplexación en contenedores MP4, M4A o MKV
- El contenedor proporciona encuadre y sincronización
- **Recomendado para la mayoría de aplicaciones de video/multimedia**
**Usar ADTS (Formato 1) cuando**:
- Creación de archivos .aac independientes
- Transmisión sin contenedor
- Se necesita mejor recuperación de errores
- Pruebas/depuración de audio de forma independiente
### Banderas de Características
**Modelado de Ruido Temporal (TNS)**:
- **Habilitar** para todos los escenarios de codificación
- Mejora la respuesta transitoria
- Mejor calidad para sonidos de percusión
- Sobrecarga computacional mínima
**Codificación Estéreo Mid-Side**:
- **Habilitar** para codificación de música estéreo
- Mejora la eficiencia de compresión
- Mejor imagen estéreo
- Sin beneficio para mono o estéreo no correlacionado
**Efectos de Baja Frecuencia (LFE)**:
- **Habilitar** solo para sonido envolvente 5.1/7.1
- Canal de subwoofer dedicado (.1)
- Deshabilitar para estéreo/mono
## Solución de Problemas
### Baja Calidad de Audio
**Síntomas**: Sonido apagado, artefactos, poca claridad
**Posibles Causas**:
1. Tasa de bits demasiado baja para el contenido
2. Perfil incorrecto para la tasa de bits
3. TNS deshabilitado
**Soluciones**:
- Aumentar la tasa de bits a los niveles recomendados (ver tablas arriba)
- Para tasas de bits bajas (<= 80 kbps), usar AAC-HE en lugar de AAC-LC
- Habilitar TNS: `SetTNS(1)`
- Para música, asegurar tasa de bits >= 128 kbps con AAC-LC
### Fallos de Inicialización del Codificador
**Síntomas**: Los métodos SetConfig o Set devuelven códigos de error
**Posibles Causas**:
1. Frecuencia de muestreo no soportada
2. Tasa de bits inválida para el perfil
3. Configuración de canales incompatible
**Soluciones**:
- Usar frecuencias de muestreo estándar: 44100, 48000 Hz
- Verificar que la tasa de bits sea apropiada para el perfil
- Comprobar que el recuento de canales coincida con el audio de origen
- Para AAC-HE, mantener la tasa de bits <= 128 kbps
### El Archivo No Se Reproduce
**Síntomas**: El archivo AAC no se reproduce en reproductores multimedia
**Posibles Causas**:
1. AAC Raw sin contenedor
2. Perfil no soportado
3. Flujo corrupto
**Soluciones**:
- Usar formato de salida ADTS (`SetOutputFormat(1)`) para archivos independientes
- Usar AAC Raw (`SetOutputFormat(0)`) solo con contenedor MP4/M4A
- Verificar que el reproductor soporte el perfil AAC (HE/HEv2 puede no ser soportado en reproductores antiguos)
- Asegurar la finalización adecuada del flujo en el gráfico de filtros
### Problemas de Compatibilidad
**Síntomas**: AAC se reproduce en algunos dispositivos pero no en otros
**Posibles Causas**:
1. Perfil avanzado no soportado (AAC-HE/HEv2)
2. Configuración no estándar
**Soluciones**:
- Usar AAC-LC (Perfil 2) para máxima compatibilidad
- Usar frecuencias de muestreo estándar (44100 o 48000 Hz)
- Mantener las tasas de bits dentro de los rangos recomendados
- Evitar tasas de bits muy bajas (< 64 kbps) para AAC-LC
---

## Ver También

- [Referencia de la Interfaz del Codificador LAME MP3](lame.md)
- [Referencia de la Interfaz del Codificador FLAC](flac.md)
- [Referencia de Códecs de Audio](../codecs-reference.md)
- [Interfaz del Multiplexor MP4](mp4-muxer.md)
- [Descripción General del Paquete de Filtros de Codificación](../index.md)
