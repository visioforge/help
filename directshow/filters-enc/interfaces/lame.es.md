---
title: Codificador LAME MP3 - Referencia de Interfaz
description: Interfaz IAudioEncoderProperties para codificación LAME MP3 con modos de tasa de bits variable y constante y configuración de calidad.
---

# Referencia de la Interfaz del Codificador LAME MP3

## Descripción General

La interfaz `IAudioEncoderProperties` proporciona control integral sobre la codificación de audio LAME MP3. LAME (LAME Ain't an MP3 Encoder) es un codificador MP3 de alta calidad que produce excelente calidad de audio con compresión eficiente.

Esta interfaz permite la configuración de tasa de bits, calidad, configuraciones de tasa de bits variable (VBR) y varias banderas de codificación para una salida MP3 óptima.

## Definición de la Interfaz

- **Nombre de la Interfaz**: `IAudioEncoderProperties`
- **GUID**: `{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}`
- **Hereda de**: `IUnknown`

## Definiciones de la Interfaz

### Definición en C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interfaz del codificador LAME MP3.
    /// </summary>
    /// <remarks>
    /// Configurar los parámetros del codificador de audio MPEG con
    /// tipo de flujo de entrada no especificado puede llevar a mal
    /// comportamiento y resultados confusos. En la mayoría de los casos
    /// los parámetros especificados serán sobrescritos por los valores
    /// predeterminados para el tipo de medio de entrada.
    /// Para lograr resultados apropiados use esta interfaz en el
    /// filtro codificador de audio con el pin de entrada conectado a una fuente válida.
    /// </remarks>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("595EB9D1-F454-41AD-A1FA-EC232AD9DA52")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEncoderProperties
    {
        // Control de Salida PES
        [PreserveSig]
        int get_PESOutputEnabled(out int dwEnabled);

        [PreserveSig]
        int set_PESOutputEnabled([In] int dwEnabled);

        // Configuración de Tasa de Bits
        [PreserveSig]
        int get_Bitrate(out int dwBitrate);

        [PreserveSig]
        int set_Bitrate([In] int dwBitrate);

        // Tasa de Bits Variable (VBR)
        [PreserveSig]
        int get_Variable(out int dwVariable);

        [PreserveSig]
        int set_Variable([In] int dwVariable);

        [PreserveSig]
        int get_VariableMin(out int dwmin);

        [PreserveSig]
        int set_VariableMin([In] int dwmin);

        [PreserveSig]
        int get_VariableMax(out int dwmax);

        [PreserveSig]
        int set_VariableMax([In] int dwmax);

        // Configuraciones de Calidad
        [PreserveSig]
        int get_Quality(out int dwQuality);

        [PreserveSig]
        int set_Quality([In] int dwQuality);

        [PreserveSig]
        int get_VariableQ(out int dwVBRq);

        [PreserveSig]
        int set_VariableQ([In] int dwVBRq);

        // Información de Fuente
        [PreserveSig]
        int get_SourceSampleRate(out int dwSampleRate);

        [PreserveSig]
        int get_SourceChannels(out int dwChannels);

        // Configuración de Salida
        [PreserveSig]
        int get_SampleRate(out int dwSampleRate);

        [PreserveSig]
        int set_SampleRate([In] int dwSampleRate);

        [PreserveSig]
        int get_ChannelMode(out int dwChannelMode);

        [PreserveSig]
        int set_ChannelMode([In] int dwChannelMode);

        // Banderas
        [PreserveSig]
        int get_CRCFlag(out int dwFlag);

        [PreserveSig]
        int set_CRCFlag([In] int dwFlag);

        [PreserveSig]
        int get_OriginalFlag(out int dwFlag);

        [PreserveSig]
        int set_OriginalFlag([In] int dwFlag);

        [PreserveSig]
        int get_CopyrightFlag(out int dwFlag);

        [PreserveSig]
        int set_CopyrightFlag([In] int dwFlag);

        [PreserveSig]
        int get_EnforceVBRmin(out int dwFlag);

        [PreserveSig]
        int set_EnforceVBRmin([In] int dwFlag);

        [PreserveSig]
        int get_VoiceMode(out int dwFlag);

        [PreserveSig]
        int set_VoiceMode([In] int dwFlag);

        [PreserveSig]
        int get_KeepAllFreq(out int dwFlag);

        [PreserveSig]
        int set_KeepAllFreq([In] int dwFlag);

        [PreserveSig]
        int get_StrictISO(out int dwFlag);

        [PreserveSig]
        int set_StrictISO([In] int dwFlag);

        [PreserveSig]
        int get_NoShortBlock(out int dwDisable);

        [PreserveSig]
        int set_NoShortBlock([In] int dwDisable);

        [PreserveSig]
        int get_XingTag(out int dwXingTag);

        [PreserveSig]
        int set_XingTag([In] int dwXingTag);

        [PreserveSig]
        int get_ForceMS(out int dwFlag);

        [PreserveSig]
        int set_ForceMS([In] int dwFlag);

        [PreserveSig]
        int get_ModeFixed(out int dwFlag);

        [PreserveSig]
        int set_ModeFixed([In] int dwFlag);

        // Gestión de Configuración
        [PreserveSig]
        int get_ParameterBlockSize(out byte pcBlock, out int pdwSize);

        [PreserveSig]
        int set_ParameterBlockSize([In] byte pcBlock, [In] int dwSize);

        [PreserveSig]
        int DefaultAudioEncoderProperties();

        [PreserveSig]
        int LoadAudioEncoderPropertiesFromRegistry();

        [PreserveSig]
        int SaveAudioEncoderPropertiesToRegistry();

        [PreserveSig]
        int InputTypeDefined();
    }
}
```

### Definición en C++

```cpp
#include <unknwn.h>

// {595EB9D1-F454-41AD-A1FA-EC232AD9DA52}
static const GUID IID_IAudioEncoderProperties =
{ 0x595eb9d1, 0xf454, 0x41ad, { 0xa1, 0xfa, 0xec, 0x23, 0x2a, 0xd9, 0xda, 0x52 } };

DECLARE_INTERFACE_(IAudioEncoderProperties, IUnknown)
{
    // Salida PES
    STDMETHOD(get_PESOutputEnabled)(THIS_ int* dwEnabled) PURE;
    STDMETHOD(set_PESOutputEnabled)(THIS_ int dwEnabled) PURE;

    // Tasa de Bits
    STDMETHOD(get_Bitrate)(THIS_ int* dwBitrate) PURE;
    STDMETHOD(set_Bitrate)(THIS_ int dwBitrate) PURE;

    // Tasa de Bits Variable
    STDMETHOD(get_Variable)(THIS_ int* dwVariable) PURE;
    STDMETHOD(set_Variable)(THIS_ int dwVariable) PURE;
    STDMETHOD(get_VariableMin)(THIS_ int* dwmin) PURE;
    STDMETHOD(set_VariableMin)(THIS_ int dwmin) PURE;
    STDMETHOD(get_VariableMax)(THIS_ int* dwmax) PURE;
    STDMETHOD(set_VariableMax)(THIS_ int dwmax) PURE;

    // Calidad
    STDMETHOD(get_Quality)(THIS_ int* dwQuality) PURE;
    STDMETHOD(set_Quality)(THIS_ int dwQuality) PURE;
    STDMETHOD(get_VariableQ)(THIS_ int* dwVBRq) PURE;
    STDMETHOD(set_VariableQ)(THIS_ int dwVBRq) PURE;

    // Información de Fuente
    STDMETHOD(get_SourceSampleRate)(THIS_ int* dwSampleRate) PURE;
    STDMETHOD(get_SourceChannels)(THIS_ int* dwChannels) PURE;

    // Configuración de Salida
    STDMETHOD(get_SampleRate)(THIS_ int* dwSampleRate) PURE;
    STDMETHOD(set_SampleRate)(THIS_ int dwSampleRate) PURE;
    STDMETHOD(get_ChannelMode)(THIS_ int* dwChannelMode) PURE;
    STDMETHOD(set_ChannelMode)(THIS_ int dwChannelMode) PURE;

    // Banderas
    STDMETHOD(get_CRCFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_CRCFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_OriginalFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_OriginalFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_CopyrightFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_CopyrightFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_EnforceVBRmin)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_EnforceVBRmin)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_VoiceMode)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_VoiceMode)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_KeepAllFreq)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_KeepAllFreq)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_StrictISO)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_StrictISO)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_NoShortBlock)(THIS_ int* dwDisable) PURE;
    STDMETHOD(set_NoShortBlock)(THIS_ int dwDisable) PURE;
    STDMETHOD(get_XingTag)(THIS_ int* dwXingTag) PURE;
    STDMETHOD(set_XingTag)(THIS_ int dwXingTag) PURE;
    STDMETHOD(get_ForceMS)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_ForceMS)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_ModeFixed)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_ModeFixed)(THIS_ int dwFlag) PURE;

    // Gestión de Configuración
    STDMETHOD(get_ParameterBlockSize)(THIS_ byte* pcBlock, int* pdwSize) PURE;
    STDMETHOD(set_ParameterBlockSize)(THIS_ byte* pcBlock, int dwSize) PURE;
    STDMETHOD(DefaultAudioEncoderProperties)(THIS) PURE;
    STDMETHOD(LoadAudioEncoderPropertiesFromRegistry)(THIS) PURE;
    STDMETHOD(SaveAudioEncoderPropertiesToRegistry)(THIS) PURE;
    STDMETHOD(InputTypeDefined)(THIS) PURE;
};
```

### Definición en Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IAudioEncoderProperties: TGUID = '{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}';

type
  IAudioEncoderProperties = interface(IUnknown)
    ['{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}']

    // Salida PES
    function get_PESOutputEnabled(out dwEnabled: Integer): HRESULT; stdcall;
    function set_PESOutputEnabled(dwEnabled: Integer): HRESULT; stdcall;

    // Tasa de Bits
    function get_Bitrate(out dwBitrate: Integer): HRESULT; stdcall;
    function set_Bitrate(dwBitrate: Integer): HRESULT; stdcall;

    // Tasa de Bits Variable
    function get_Variable(out dwVariable: Integer): HRESULT; stdcall;
    function set_Variable(dwVariable: Integer): HRESULT; stdcall;
    function get_VariableMin(out dwmin: Integer): HRESULT; stdcall;
    function set_VariableMin(dwmin: Integer): HRESULT; stdcall;
    function get_VariableMax(out dwmax: Integer): HRESULT; stdcall;
    function set_VariableMax(dwmax: Integer): HRESULT; stdcall;

    // Calidad
    function get_Quality(out dwQuality: Integer): HRESULT; stdcall;
    function set_Quality(dwQuality: Integer): HRESULT; stdcall;
    function get_VariableQ(out dwVBRq: Integer): HRESULT; stdcall;
    function set_VariableQ(dwVBRq: Integer): HRESULT; stdcall;

    // Información de Fuente
    function get_SourceSampleRate(out dwSampleRate: Integer): HRESULT; stdcall;
    function get_SourceChannels(out dwChannels: Integer): HRESULT; stdcall;

    // Configuración de Salida
    function get_SampleRate(out dwSampleRate: Integer): HRESULT; stdcall;
    function set_SampleRate(dwSampleRate: Integer): HRESULT; stdcall;
    function get_ChannelMode(out dwChannelMode: Integer): HRESULT; stdcall;
    function set_ChannelMode(dwChannelMode: Integer): HRESULT; stdcall;

    // Banderas
    function get_CRCFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_CRCFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_OriginalFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_OriginalFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_CopyrightFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_CopyrightFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_EnforceVBRmin(out dwFlag: Integer): HRESULT; stdcall;
    function set_EnforceVBRmin(dwFlag: Integer): HRESULT; stdcall;
    function get_VoiceMode(out dwFlag: Integer): HRESULT; stdcall;
    function set_VoiceMode(dwFlag: Integer): HRESULT; stdcall;
    function get_KeepAllFreq(out dwFlag: Integer): HRESULT; stdcall;
    function set_KeepAllFreq(dwFlag: Integer): HRESULT; stdcall;
    function get_StrictISO(out dwFlag: Integer): HRESULT; stdcall;
    function set_StrictISO(dwFlag: Integer): HRESULT; stdcall;
    function get_NoShortBlock(out dwDisable: Integer): HRESULT; stdcall;
    function set_NoShortBlock(dwDisable: Integer): HRESULT; stdcall;
    function get_XingTag(out dwXingTag: Integer): HRESULT; stdcall;
    function set_XingTag(dwXingTag: Integer): HRESULT; stdcall;
    function get_ForceMS(out dwFlag: Integer): HRESULT; stdcall;
    function set_ForceMS(dwFlag: Integer): HRESULT; stdcall;
    function get_ModeFixed(out dwFlag: Integer): HRESULT; stdcall;
    function set_ModeFixed(dwFlag: Integer): HRESULT; stdcall;

    // Gestión de Configuración
    function get_ParameterBlockSize(out pcBlock: Byte; out pdwSize: Integer): HRESULT; stdcall;
    function set_ParameterBlockSize(pcBlock: Byte; dwSize: Integer): HRESULT; stdcall;
    function DefaultAudioEncoderProperties: HRESULT; stdcall;
    function LoadAudioEncoderPropertiesFromRegistry: HRESULT; stdcall;
    function SaveAudioEncoderPropertiesToRegistry: HRESULT; stdcall;
    function InputTypeDefined: HRESULT; stdcall;
  end;
```

---
## Referencia de Métodos
### Configuración de Tasa de Bits
#### set_Bitrate / get_Bitrate
Establece o recupera la tasa de bits de compresión objetivo en Kbits/s.
**Parámetros**:
- `dwBitrate`: Tasa de bits en kilobits por segundo
**Tasas de Bits MP3 Comunes**:
- **320 kbps** - Máxima calidad, casi transparente
- **256 kbps** - Muy alta calidad
- **192 kbps** - Alta calidad (recomendado para música)
- **128 kbps** - Calidad estándar (aceptable para la mayoría del contenido)
- **96 kbps** - Menor calidad, archivos más pequeños
- **64 kbps** - Calidad para voz/podcasts
**Ejemplo (C#)**:
```csharp
var lame = audioEncoder as IAudioEncoderProperties;
if (lame != null)
{
    // Establecer alta calidad 192 kbps
    lame.set_Bitrate(192);
}
```
---

### Tasa de Bits Variable (VBR)

#### set_Variable / get_Variable

Habilita o deshabilita el modo de tasa de bits variable.

**Parámetros**:
- `dwVariable`: 1 para habilitar VBR, 0 para deshabilitar (modo CBR)

**Notas de Uso**:
- VBR proporciona mejor relación calidad-tamaño que CBR
- VBR asigna más bits a pasajes de audio complejos
- CBR proporciona tamaños de archivo predecibles
- VBR se recomienda para archivado de música

#### set_VariableMin / get_VariableMin

Establece la tasa de bits mínima para el modo VBR.

**Parámetros**:
- `dwmin`: Tasa de bits mínima en kbps

#### set_VariableMax / get_VariableMax

Establece la tasa de bits máxima para el modo VBR.

**Parámetros**:
- `dwmax`: Tasa de bits máxima en kbps

**Ejemplo (C#)**:
```csharp
// Habilitar VBR con rango 128-256 kbps
lame.set_Variable(1);
lame.set_VariableMin(128);
lame.set_VariableMax(256);
lame.set_VariableQ(4); // Nivel de calidad VBR
```

---
### Configuraciones de Calidad
#### set_Quality / get_Quality
Establece la calidad de codificación para el modo CBR.
**Parámetros**:
- `dwQuality`: Nivel de calidad (0-9)
  - **0** - Máxima calidad (más lento)
  - **2** - Casi máxima calidad (recomendado)
  - **5** - Buen balance calidad/velocidad
  - **7** - Codificación más rápida, menor calidad
  - **9** - Menor calidad (más rápido)
**Ejemplo (C++)**:
```cpp
IAudioEncoderProperties* pLame = nullptr;
pFilter->QueryInterface(IID_IAudioEncoderProperties, (void**)&pLame);
// Codificación CBR de alta calidad
pLame->set_Bitrate(192);
pLame->set_Quality(2);
pLame->Release();
```
#### set_VariableQ / get_VariableQ
Establece el nivel de calidad para el modo VBR.
**Parámetros**:
- `dwVBRq`: Calidad VBR (0-9)
  - **0** - Máxima calidad (~245 kbps)
  - **2** - Muy alta calidad (~190 kbps)
  - **4** - Alta calidad (~165 kbps) - recomendado
  - **6** - Calidad media (~130 kbps)
  - **9** - Menor calidad (~65 kbps)
---

### Modo de Canales

#### set_ChannelMode / get_ChannelMode

Establece el modo de codificación estéreo.

**Parámetros**:
- `dwChannelMode`: Valor del modo de canales
  - **0** - Estéreo
  - **1** - Joint Stereo (recomendado)
  - **2** - Canal Dual
  - **3** - Mono

**Notas de Uso**:
- Joint Stereo proporciona mejor calidad a tasas de bits más bajas
- Use Estéreo para escucha crítica a altas tasas de bits
- Mono reduce el tamaño del archivo para voz/podcasts

**Ejemplo (C#)**:
```csharp
// Joint stereo para música a 192 kbps
lame.set_ChannelMode(1);
lame.set_Bitrate(192);
```

---
### Banderas de Codificación
#### set_CRCFlag / get_CRCFlag
Habilita la protección de errores CRC.
**Parámetros**:
- `dwFlag`: 1 para habilitar, 0 para deshabilitar
**Uso**: Agrega detección de errores, aumento mínimo de tamaño (~0.2%)
#### set_CopyrightFlag / get_CopyrightFlag
Establece la bandera de copyright en el encabezado MP3.
**Parámetros**:
- `dwFlag`: 1 si tiene copyright, 0 de lo contrario
#### set_OriginalFlag / get_OriginalFlag
Establece la bandera de original/copia.
**Parámetros**:
- `dwFlag`: 1 para original, 0 para copia
#### set_VoiceMode / get_VoiceMode
Optimiza la codificación para contenido de voz.
**Parámetros**:
- `dwFlag`: 1 para habilitar optimización de voz
**Uso**: Mejora la calidad para voz a tasas de bits más bajas
**Ejemplo (C#)**:
```csharp
// Optimizar para contenido de podcast/voz
lame.set_VoiceMode(1);
lame.set_Bitrate(64);
lame.set_ChannelMode(3); // Mono
```
#### set_XingTag / get_XingTag
Agrega etiqueta Xing VBR para búsqueda precisa.
**Parámetros**:
- `dwFlag`: 1 para agregar etiqueta (recomendado para VBR)
**Uso**: Esencial para archivos VBR para habilitar búsqueda apropiada
---

## Gestión de Configuración

### SaveAudioEncoderPropertiesToRegistry

Guarda la configuración actual del codificador en el registro.

**Notas de Uso**:
- Debe llamarse después de cambiar propiedades
- Las configuraciones persisten entre sesiones
- Requiere permisos de registro apropiados

### LoadAudioEncoderPropertiesFromRegistry

Carga la configuración del codificador desde el registro.

### DefaultAudioEncoderProperties

Restablece todas las propiedades del codificador a los valores predeterminados basados en el tipo de flujo de entrada.

### InputTypeDefined

Verifica si el formato de entrada ha sido especificado.

**Retorna**:
- `S_OK` - El tipo de entrada está definido, el codificador puede configurarse
- `E_FAIL` - Tipo de entrada no especificado, la configuración puede fallar

---
## Ejemplos Completos
### Ejemplo 1: Codificación de Música de Alta Calidad (C#)
```csharp
using VisioForge.DirectShowAPI;
public void ConfigureHighQualityMP3(IBaseFilter audioEncoder)
{
    var lame = audioEncoder as IAudioEncoderProperties;
    if (lame == null)
        return;
    // Verificar si la entrada está conectada
    if (lame.InputTypeDefined() != 0)
    {
        Console.WriteLine("Advertencia: Entrada no conectada, usando valores predeterminados");
    }
    // Configuraciones VBR de alta calidad
    lame.set_Variable(1);              // Habilitar VBR
    lame.set_VariableQ(2);             // Muy alta calidad
    lame.set_VariableMin(192);         // Mín 192 kbps
    lame.set_VariableMax(320);         // Máx 320 kbps
    // Joint stereo para eficiencia
    lame.set_ChannelMode(1);
    // Banderas de calidad
    lame.set_XingTag(1);               // Agregar etiqueta VBR
    lame.set_OriginalFlag(1);          // Marcar como original
    lame.set_CopyrightFlag(1);         // Establecer copyright
    // Guardar configuraciones
    lame.SaveAudioEncoderPropertiesToRegistry();
}
```
### Ejemplo 2: Codificación de Podcast/Voz (C++)
```cpp
#include "LAME.h"
HRESULT ConfigurePodcastMP3(IBaseFilter* pAudioEncoder)
{
    HRESULT hr;
    IAudioEncoderProperties* pLame = nullptr;
    hr = pAudioEncoder->QueryInterface(IID_IAudioEncoderProperties,
                                       (void**)&pLame);
    if (FAILED(hr))
        return hr;
    // Configuraciones optimizadas para voz
    pLame->set_VoiceMode(1);           // Optimización de voz
    pLame->set_Bitrate(64);            // 64 kbps para voz
    pLame->set_Quality(5);             // Calidad balanceada
    pLame->set_ChannelMode(3);         // Mono
    // Deshabilitar VBR para tamaño de archivo predecible
    pLame->set_Variable(0);
    // Agregar etiqueta Xing para compatibilidad
    pLame->set_XingTag(1);
    // Guardar configuración
    pLame->SaveAudioEncoderPropertiesToRegistry();
    pLame->Release();
    return S_OK;
}
```
### Ejemplo 3: Codificación de Música Estándar (Delphi)
```delphi
procedure ConfigureStandardMP3(AudioEncoder: IBaseFilter);
var
  Lame: IAudioEncoderProperties;
  hr: HRESULT;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IAudioEncoderProperties, Lame)) then
  begin
    // Configuraciones VBR estándar para música
    Lame.set_Variable(1);              // Habilitar VBR
    Lame.set_VariableQ(4);             // Alta calidad (~165 kbps promedio)
    Lame.set_VariableMin(128);         // Mín 128 kbps
    Lame.set_VariableMax(256);         // Máx 256 kbps
    // Joint stereo
    Lame.set_ChannelMode(1);
    // Banderas esenciales
    Lame.set_XingTag(1);               // Etiqueta VBR para búsqueda
    // Guardar en registro
    Lame.SaveAudioEncoderPropertiesToRegistry;
    Lame := nil;
  end;
end;
```
---

## Mejores Prácticas

### Recomendaciones de Calidad

1. **Archivado de Música**: VBR Q0-Q2 (245-190 kbps promedio)
2. **Distribución de Música**: VBR Q4 (165 kbps) o CBR 192 kbps
3. **Streaming**: CBR 128 kbps
4. **Podcasts/Voz**: CBR 64 kbps mono con modo de voz

### Consejos de Rendimiento

1. **Use Joint Stereo** a tasas de bits por debajo de 192 kbps
2. **Habilite VBR** para mejor relación calidad-tamaño
3. **Agregue Etiqueta Xing** para archivos VBR
4. **Use Modo de Voz** para contenido de voz a <96 kbps

### Flujo de Trabajo de Configuración

1. Conectar pin de entrada antes de configurar
2. Verificar `InputTypeDefined()` antes de establecer propiedades
3. Configurar todas las propiedades deseadas
4. Llamar `SaveAudioEncoderPropertiesToRegistry()`
5. Verificar configuraciones con métodos get

---
## Solución de Problemas
### Problema: Configuraciones No Aplicadas
**Solución**:
```csharp
// Asegurar que la entrada esté conectada primero
if (lame.InputTypeDefined() == 0)
{
    // Configurar ajustes
    lame.set_Bitrate(192);
    lame.SaveAudioEncoderPropertiesToRegistry();
}
else
{
    // Conectar entrada primero, luego configurar
}
```
### Problema: Salida de Mala Calidad
**Soluciones**:
- Aumentar calidad VBR: `set_VariableQ(2)` o menor
- Aumentar tasa de bits CBR: `set_Bitrate(192)` o mayor
- Usar mejor configuración de calidad: `set_Quality(2)`
- Deshabilitar modo de voz para música: `set_VoiceMode(0)`
### Problema: Tamaños de Archivo Grandes
**Soluciones**:
```cpp
// Usar VBR en lugar de CBR alto
pLame->set_Variable(1);
pLame->set_VariableQ(4);        // ~165 kbps promedio
pLame->set_VariableMax(192);    // Limitar tasa de bits máxima
```
---

## Vea También

- [Descripción General del Pack de Filtros de Codificación](../index.es.md)
- [Referencia de Códecs de Audio](../codecs-reference.es.md)
- [Codificador AAC](aac.es.md)
- [Codificador FLAC](flac.es.md)
