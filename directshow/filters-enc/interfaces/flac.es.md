---
title: Referencia de la Interfaz del Codificador FLAC
description: Interfaz DirectShow del codificador FLAC con niveles de codificación, configuración LPC, tamaños de bloque y compresión para codificación de audio sin pérdida.
---

# Referencia de la Interfaz del Codificador FLAC

## Descripción General

La interfaz **IFLACEncodeSettings** proporciona un control completo sobre los parámetros de codificación de audio FLAC (Free Lossless Audio Codec) en gráficos de filtros DirectShow. FLAC es un formato de compresión de audio sin pérdida que reduce el tamaño del archivo sin ninguna pérdida en la calidad del audio, lo que lo hace ideal para archivo, producción de audio profesional y distribución de música de alta fidelidad.

Esta interfaz permite a los desarrolladores configurar niveles de calidad de codificación, parámetros de Codificación Predictiva Lineal (LPC), tamaños de bloque, codificación estéreo mid-side y órdenes de partición Rice para lograr una eficiencia de compresión óptima para diferentes tipos de contenido de audio.

**GUID de la Interfaz**: `{A6096781-2A65-4540-A536-011235D0A5FE}`

**Hereda de**: `IUnknown`

## Definiciones de Interfaz

### Definición en C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interfaz de configuración del codificador FLAC (Free Lossless Audio Codec).
    /// Proporciona un control completo sobre los parámetros de codificación FLAC para la compresión de audio sin pérdida.
    /// </summary>
    [ComImport]
    [Guid("A6096781-2A65-4540-A536-011235D0A5FE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFLACEncodeSettings
    {
        /// <summary>
        /// Comprueba si la configuración de codificación se puede modificar en el momento actual.
        /// </summary>
        /// <returns>Verdadero si la configuración se puede modificar, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool canModifySettings();

        /// <summary>
        /// Establece el nivel de codificación FLAC (calidad de compresión).
        /// </summary>
        /// <param name="inLevel">Nivel de codificación (0-8, donde 8 es la mayor compresión, más lento)</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setEncodingLevel(uint inLevel);

        /// <summary>
        /// Establece el orden de Codificación Predictiva Lineal (LPC).
        /// Valores más altos proporcionan mejor compresión pero una codificación más lenta.
        /// </summary>
        /// <param name="inLPCOrder">Orden LPC (típicamente 0-32)</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setLPCOrder(uint inLPCOrder);

        /// <summary>
        /// Establece el tamaño del bloque de audio para la codificación.
        /// Bloques más grandes pueden proporcionar mejor compresión pero aumentan la latencia.
        /// </summary>
        /// <param name="inBlockSize">Tamaño del bloque en muestras (típicamente 192-4608)</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setBlockSize(uint inBlockSize);

        /// <summary>
        /// Habilita o deshabilita la codificación estéreo mid-side para audio de 2 canales.
        /// Puede mejorar la compresión para audio estéreo con canales correlacionados.
        /// </summary>
        /// <param name="inUseMidSideCoding">Verdadero para habilitar la codificación mid-side, falso para deshabilitar</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        /// <remarks>Solo aplicable para audio de 2 canales (estéreo)</remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useMidSideCoding([In, MarshalAs(UnmanagedType.Bool)] bool inUseMidSideCoding);

        /// <summary>
        /// Habilita o deshabilita la codificación estéreo mid-side adaptativa.
        /// Decide automáticamente si usar codificación mid-side bloque por bloque.
        /// Anula useMidSideCoding y generalmente es más rápido.
        /// </summary>
        /// <param name="inUseAdaptiveMidSideCoding">Verdadero para habilitar la codificación mid-side adaptativa, falso para deshabilitar</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        /// <remarks>Solo para audio de 2 canales. Anula la configuración useMidSideCoding. Generalmente proporciona mejor rendimiento.</remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useAdaptiveMidSideCoding([In, MarshalAs(UnmanagedType.Bool)] bool inUseAdaptiveMidSideCoding);

        /// <summary>
        /// Habilita o deshabilita la búsqueda exhaustiva de modelos para la mejor compresión.
        /// Significativamente más lento pero puede proporcionar mejores relaciones de compresión.
        /// </summary>
        /// <param name="inUseExhaustiveModelSearch">Verdadero para habilitar la búsqueda exhaustiva, falso para deshabilitar</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useExhaustiveModelSearch([In, MarshalAs(UnmanagedType.Bool)] bool inUseExhaustiveModelSearch);

        /// <summary>
        /// Establece el rango de orden de partición Rice para la codificación de entropía.
        /// Controla el compromiso entre la eficiencia de compresión y la velocidad de codificación.
        /// </summary>
        /// <param name="inMin">Orden de partición Rice mínimo</param>
        /// <param name="inMax">Orden de partición Rice máximo</param>
        /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setRicePartitionOrder(uint inMin, uint inMax);

        /// <summary>
        /// Obtiene la configuración actual del nivel de codificación.
        /// </summary>
        /// <returns>Nivel de codificación actual (0-8)</returns>
        [PreserveSig]
        int encoderLevel();

        /// <summary>
        /// Obtiene el orden actual de Codificación Predictiva Lineal (LPC).
        /// </summary>
        /// <returns>Orden LPC actual</returns>
        [PreserveSig]
        uint LPCOrder();

        /// <summary>
        /// Obtiene la configuración actual del tamaño de bloque.
        /// </summary>
        /// <returns>Tamaño de bloque actual en muestras</returns>
        [PreserveSig]
        uint blockSize();

        /// <summary>
        /// Obtiene el orden de partición Rice mínimo.
        /// </summary>
        /// <returns>Orden de partición Rice mínimo</returns>
        [PreserveSig]
        uint riceMin();

        /// <summary>
        /// Obtiene el orden de partición Rice máximo.
        /// </summary>
        /// <returns>Orden de partición Rice máximo</returns>
        [PreserveSig]
        uint riceMax();

        /// <summary>
        /// Comprueba si la codificación estéreo mid-side está habilitada.
        /// </summary>
        /// <returns>Verdadero si la codificación mid-side está habilitada, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingMidSideCoding();

        /// <summary>
        /// Comprueba si la codificación estéreo mid-side adaptativa está habilitada.
        /// </summary>
        /// <returns>Verdadero si la codificación mid-side adaptativa está habilitada, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingAdaptiveMidSideCoding();

        /// <summary>
        /// Comprueba si la búsqueda exhaustiva de modelos está habilitada.
        /// </summary>
        /// <returns>Verdadero si la búsqueda exhaustiva de modelos está habilitada, falso en caso contrario</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingExhaustiveModel();
    }
}
```

### Definición en C++

```cpp
#include <unknwn.h>

// {A6096781-2A65-4540-A536-011235D0A5FE}
DEFINE_GUID(IID_IFLACEncodeSettings,
    0xa6096781, 0x2a65, 0x4540, 0xa5, 0x36, 0x01, 0x12, 0x35, 0xd0, 0xa5, 0xfe);

/// <summary>
/// Interfaz de configuración del codificador FLAC (Free Lossless Audio Codec).
/// Proporciona un control completo sobre los parámetros de codificación FLAC para la compresión de audio sin pérdida.
/// </summary>
DECLARE_INTERFACE_(IFLACEncodeSettings, IUnknown)
{
    /// <summary>
    /// Comprueba si la configuración de codificación se puede modificar en el momento actual.
    /// </summary>
    /// <returns>TRUE si la configuración se puede modificar, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, canModifySettings)(THIS) PURE;

    /// <summary>
    /// Establece el nivel de codificación FLAC (calidad de compresión).
    /// </summary>
    /// <param name="inLevel">Nivel de codificación (0-8, donde 8 es la mayor compresión, más lento)</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, setEncodingLevel)(THIS_
        unsigned long inLevel
        ) PURE;

    /// <summary>
    /// Establece el orden de Codificación Predictiva Lineal (LPC).
    /// Valores más altos proporcionan mejor compresión pero una codificación más lenta.
    /// </summary>
    /// <param name="inLPCOrder">Orden LPC (típicamente 0-32)</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, setLPCOrder)(THIS_
        unsigned long inLPCOrder
        ) PURE;

    /// <summary>
    /// Establece el tamaño del bloque de audio para la codificación.
    /// Bloques más grandes pueden proporcionar mejor compresión pero aumentan la latencia.
    /// </summary>
    /// <param name="inBlockSize">Tamaño del bloque en muestras (típicamente 192-4608)</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, setBlockSize)(THIS_
        unsigned long inBlockSize
        ) PURE;

    /// <summary>
    /// Habilita o deshabilita la codificación estéreo mid-side para audio de 2 canales.
    /// Puede mejorar la compresión para audio estéreo con canales correlacionados.
    /// </summary>
    /// <param name="inUseMidSideCoding">TRUE para habilitar la codificación mid-side, FALSE para deshabilitar</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    /// <remarks>Solo aplicable para audio de 2 canales (estéreo)</remarks>
    STDMETHOD_(BOOL, useMidSideCoding)(THIS_
        BOOL inUseMidSideCoding
        ) PURE;

    /// <summary>
    /// Habilita o deshabilita la codificación estéreo mid-side adaptativa.
    /// Decide automáticamente si usar codificación mid-side bloque por bloque.
    /// Anula useMidSideCoding y generalmente es más rápido.
    /// </summary>
    /// <param name="inUseAdaptiveMidSideCoding">TRUE para habilitar la codificación mid-side adaptativa, FALSE para deshabilitar</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    /// <remarks>Solo para audio de 2 canales. Anula la configuración useMidSideCoding. Generalmente proporciona mejor rendimiento.</remarks>
    STDMETHOD_(BOOL, useAdaptiveMidSideCoding)(THIS_
        BOOL inUseAdaptiveMidSideCoding
        ) PURE;

    /// <summary>
    /// Habilita o deshabilita la búsqueda exhaustiva de modelos para la mejor compresión.
    /// Significativamente más lento pero puede proporcionar mejores relaciones de compresión.
    /// </summary>
    /// <param name="inUseExhaustiveModelSearch">TRUE para habilitar la búsqueda exhaustiva, FALSE para deshabilitar</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, useExhaustiveModelSearch)(THIS_
        BOOL inUseExhaustiveModelSearch
        ) PURE;

    /// <summary>
    /// Establece el rango de orden de partición Rice para la codificación de entropía.
    /// Controla el compromiso entre la eficiencia de compresión y la velocidad de codificación.
    /// </summary>
    /// <param name="inMin">Orden de partición Rice mínimo</param>
    /// <param name="inMax">Orden de partición Rice máximo</param>
    /// <returns>TRUE si tiene éxito, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, setRicePartitionOrder)(THIS_
        unsigned long inMin,
        unsigned long inMax
        ) PURE;

    /// <summary>
    /// Obtiene la configuración actual del nivel de codificación.
    /// </summary>
    /// <returns>Nivel de codificación actual (0-8)</returns>
    STDMETHOD_(int, encoderLevel)(THIS) PURE;

    /// <summary>
    /// Obtiene el orden actual de Codificación Predictiva Lineal (LPC).
    /// </summary>
    /// <returns>Orden LPC actual</returns>
    STDMETHOD_(unsigned long, LPCOrder)(THIS) PURE;

    /// <summary>
    /// Obtiene la configuración actual del tamaño de bloque.
    /// </summary>
    /// <returns>Tamaño de bloque actual en muestras</returns>
    STDMETHOD_(unsigned long, blockSize)(THIS) PURE;

    /// <summary>
    /// Obtiene el orden de partición Rice mínimo.
    /// </summary>
    /// <returns>Orden de partición Rice mínimo</returns>
    STDMETHOD_(unsigned long, riceMin)(THIS) PURE;

    /// <summary>
    /// Obtiene el orden de partición Rice máximo.
    /// </summary>
    /// <returns>Orden de partición Rice máximo</returns>
    STDMETHOD_(unsigned long, riceMax)(THIS) PURE;

    /// <summary>
    /// Comprueba si la codificación estéreo mid-side está habilitada.
    /// </summary>
    /// <returns>TRUE si la codificación mid-side está habilitada, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, isUsingMidSideCoding)(THIS) PURE;

    /// <summary>
    /// Comprueba si la codificación estéreo mid-side adaptativa está habilitada.
    /// </summary>
    /// <returns>TRUE si la codificación mid-side adaptativa está habilitada, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, isUsingAdaptiveMidSideCoding)(THIS) PURE;

    /// <summary>
    /// Comprueba si la búsqueda exhaustiva de modelos está habilitada.
    /// </summary>
    /// <returns>TRUE si la búsqueda exhaustiva de modelos está habilitada, FALSE en caso contrario</returns>
    STDMETHOD_(BOOL, isUsingExhaustiveModel)(THIS) PURE;
};
```

### Definición en Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IFLACEncodeSettings: TGUID = '{A6096781-2A65-4540-A536-011235D0A5FE}';

type
  /// <summary>
  /// Interfaz de configuración del codificador FLAC (Free Lossless Audio Codec).
  /// Proporciona un control completo sobre los parámetros de codificación FLAC para la compresión de audio sin pérdida.
  /// </summary>
  IFLACEncodeSettings = interface(IUnknown)
    ['{A6096781-2A65-4540-A536-011235D0A5FE}']

    /// <summary>
    /// Comprueba si la configuración de codificación se puede modificar en el momento actual.
    /// </summary>
    /// <returns>Verdadero si la configuración se puede modificar, falso en caso contrario</returns>
    function canModifySettings: BOOL; stdcall;

    /// <summary>
    /// Establece el nivel de codificación FLAC (calidad de compresión).
    /// </summary>
    /// <param name="inLevel">Nivel de codificación (0-8, donde 8 es la mayor compresión, más lento)</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    function setEncodingLevel(inLevel: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Establece el orden de Codificación Predictiva Lineal (LPC).
    /// Valores más altos proporcionan mejor compresión pero una codificación más lenta.
    /// </summary>
    /// <param name="inLPCOrder">Orden LPC (típicamente 0-32)</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    function setLPCOrder(inLPCOrder: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Establece el tamaño del bloque de audio para la codificación.
    /// Bloques más grandes pueden proporcionar mejor compresión pero aumentan la latencia.
    /// </summary>
    /// <param name="inBlockSize">Tamaño del bloque en muestras (típicamente 192-4608)</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    function setBlockSize(inBlockSize: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Habilita o deshabilita la codificación estéreo mid-side para audio de 2 canales.
    /// Puede mejorar la compresión para audio estéreo con canales correlacionados.
    /// </summary>
    /// <param name="inUseMidSideCoding">Verdadero para habilitar la codificación mid-side, falso para deshabilitar</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    /// <remarks>Solo aplicable para audio de 2 canales (estéreo)</remarks>
    function useMidSideCoding(inUseMidSideCoding: BOOL): BOOL; stdcall;

    /// <summary>
    /// Habilita o deshabilita la codificación estéreo mid-side adaptativa.
    /// Decide automáticamente si usar codificación mid-side bloque por bloque.
    /// Anula useMidSideCoding y generalmente es más rápido.
    /// </summary>
    /// <param name="inUseAdaptiveMidSideCoding">Verdadero para habilitar la codificación mid-side adaptativa, falso para deshabilitar</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    /// <remarks>Solo para audio de 2 canales. Anula la configuración useMidSideCoding. Generalmente proporciona mejor rendimiento.</remarks>
    function useAdaptiveMidSideCoding(inUseAdaptiveMidSideCoding: BOOL): BOOL; stdcall;

    /// <summary>
    /// Habilita o deshabilita la búsqueda exhaustiva de modelos para la mejor compresión.
    /// Significativamente más lento pero puede proporcionar mejores relaciones de compresión.
    /// </summary>
    /// <param name="inUseExhaustiveModelSearch">Verdadero para habilitar la búsqueda exhaustiva, falso para deshabilitar</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    function useExhaustiveModelSearch(inUseExhaustiveModelSearch: BOOL): BOOL; stdcall;

    /// <summary>
    /// Establece el rango de orden de partición Rice para la codificación de entropía.
    /// Controla el compromiso entre la eficiencia de compresión y la velocidad de codificación.
    /// </summary>
    /// <param name="inMin">Orden de partición Rice mínimo</param>
    /// <param name="inMax">Orden de partición Rice máximo</param>
    /// <returns>Verdadero si tiene éxito, falso en caso contrario</returns>
    function setRicePartitionOrder(inMin: Cardinal; inMax: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Obtiene la configuración actual del nivel de codificación.
    /// </summary>
    /// <returns>Nivel de codificación actual (0-8)</returns>
    function encoderLevel: Integer; stdcall;

    /// <summary>
    /// Obtiene el orden actual de Codificación Predictiva Lineal (LPC).
    /// </summary>
    /// <returns>Orden LPC actual</returns>
    function LPCOrder: Cardinal; stdcall;

    /// <summary>
    /// Obtiene la configuración actual del tamaño de bloque.
    /// </summary>
    /// <returns>Tamaño de bloque actual en muestras</returns>
    function blockSize: Cardinal; stdcall;

    /// <summary>
    /// Obtiene el orden de partición Rice mínimo.
    /// </summary>
    /// <returns>Orden de partición Rice mínimo</returns>
    function riceMin: Cardinal; stdcall;

    /// <summary>
    /// Obtiene el orden de partición Rice máximo.
    /// </summary>
    /// <returns>Orden de partición Rice máximo</returns>
    function riceMax: Cardinal; stdcall;

    /// <summary>
    /// Comprueba si la codificación estéreo mid-side está habilitada.
    /// </summary>
    /// <returns>Verdadero si la codificación mid-side está habilitada, falso en caso contrario</returns>
    function isUsingMidSideCoding: BOOL; stdcall;

    /// <summary>
    /// Comprueba si la codificación estéreo mid-side adaptativa está habilitada.
    /// </summary>
    /// <returns>Verdadero si la codificación mid-side adaptativa está habilitada, falso en caso contrario</returns>
    function isUsingAdaptiveMidSideCoding: BOOL; stdcall;

    /// <summary>
    /// Comprueba si la búsqueda exhaustiva de modelos está habilitada.
    /// </summary>
    /// <returns>Verdadero si la búsqueda exhaustiva de modelos está habilitada, falso en caso contrario</returns>
    function isUsingExhaustiveModel: BOOL; stdcall;
  end;
```

## Referencia de Métodos

### Comprobación de Configuración

#### canModifySettings

Comprueba si la configuración de codificación se puede modificar en el momento actual. Esto es útil para verificar que el codificador esté en un estado donde se permitan cambios de configuración (típicamente antes de que el gráfico de filtros comience a ejecutarse).

**Devuelve**: `true` si la configuración se puede modificar, `false` en caso contrario

**Ejemplo de Uso**:
```csharp
if (flacEncoder.canModifySettings())
{
    // Seguro para modificar la configuración del codificador
    flacEncoder.setEncodingLevel(5);
}
```

### Métodos de Configuración de Codificación

#### setEncodingLevel

Establece el nivel de codificación FLAC, que controla el compromiso entre la calidad de compresión y la velocidad de codificación.

**Parámetros**:
- `inLevel` - Nivel de codificación (0-8):
  - 0 = Codificación más rápida, menor compresión
  - 5 = Equilibrado (recomendado para la mayoría de los usos)
  - 8 = Codificación más lenta, mayor compresión

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Valores Recomendados**:
- **Archivo rápido**: Nivel 3-5
- **Archivo de alta calidad**: Nivel 6-8
- **Codificación en tiempo real**: Nivel 0-2

#### setLPCOrder

Establece el orden de Codificación Predictiva Lineal (LPC), que afecta la eficiencia de compresión y la velocidad de codificación.

**Parámetros**:
- `inLPCOrder` - Valor de orden LPC (típicamente 0-32)
  - 0 = Sin LPC (más rápido)
  - 12 = Predeterminado para la mayoría de los audios
  - 32 = Compresión máxima (más lento)

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Nota**: Órdenes LPC más altos proporcionan mejor compresión pero aumentan significativamente el tiempo de codificación.

#### setBlockSize

Establece el tamaño del bloque de audio para la codificación. El tamaño del bloque afecta tanto a la eficiencia de compresión como a la latencia.

**Parámetros**:
- `inBlockSize` - Tamaño del bloque en muestras
  - Valores comunes: 192, 576, 1152, 2304, 4608
  - El valor predeterminado es típicamente 4096 para audio de 44.1kHz

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Recomendaciones**:
- **Baja latencia**: 192-1152 muestras
- **Archivo estándar**: 4096 muestras
- **Compresión máxima**: 4608 muestras

#### useMidSideCoding

Habilita o deshabilita la codificación estéreo mid-side para audio de 2 canales. La codificación mid-side puede mejorar la compresión para audio estéreo donde los canales izquierdo y derecho están altamente correlacionados.

**Parámetros**:
- `inUseMidSideCoding` - `true` para habilitar, `false` para deshabilitar

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Nota**: Solo aplicable para audio de 2 canales (estéreo). La mayoría de la música se beneficia de la codificación mid-side.

#### useAdaptiveMidSideCoding

Habilita o deshabilita la codificación estéreo mid-side adaptativa. Este modo decide automáticamente si usar codificación mid-side bloque por bloque, proporcionando mejor compresión que la codificación mid-side fija.

**Parámetros**:
- `inUseAdaptiveMidSideCoding` - `true` para habilitar, `false` para deshabilitar

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Nota**:
- Solo para audio de 2 canales
- Anula la configuración `useMidSideCoding`
- Generalmente proporciona mejor rendimiento que la codificación mid-side fija
- Recomendado para la mayoría de los escenarios de codificación estéreo

#### useExhaustiveModelSearch

Habilita o deshabilita la búsqueda exhaustiva de modelos para encontrar el mejor predictor de compresión.

**Parámetros**:
- `inUseExhaustiveModelSearch` - `true` para habilitar, `false` para deshabilitar

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Advertencia**: La búsqueda exhaustiva ralentiza significativamente la codificación (a menudo 2-4 veces más lento) pero puede proporcionar una compresión marginalmente mejor (típicamente 1-3% de reducción del tamaño del archivo).

**Recomendado**: Habilitar solo para el archivo de audio crítico donde el tiempo de codificación no es una preocupación.

#### setRicePartitionOrder

Establece el rango de orden de partición Rice para la codificación de entropía. La codificación Rice es la etapa final de compresión en FLAC.

**Parámetros**:
- `inMin` - Orden de partición Rice mínimo (típicamente 0-2)
- `inMax` - Orden de partición Rice máximo (típicamente 3-8)

**Devuelve**: `true` si tiene éxito, `false` en caso contrario

**Valores Típicos**:
- Codificación rápida: min=0, max=3
- Codificación estándar: min=0, max=6
- Compresión máxima: min=0, max=8

### Métodos de Consulta de Estado

#### encoderLevel

Obtiene la configuración actual del nivel de codificación.

**Devuelve**: Nivel de codificación actual (0-8)

#### LPCOrder

Obtiene el orden actual de Codificación Predictiva Lineal (LPC).

**Devuelve**: Valor de orden LPC actual

#### blockSize

Obtiene la configuración actual del tamaño de bloque.

**Devuelve**: Tamaño de bloque actual en muestras

#### riceMin

Obtiene el orden de partición Rice mínimo.

**Devuelve**: Orden de partición Rice mínimo

#### riceMax

Obtiene el orden de partición Rice máximo.

**Devuelve**: Orden de partición Rice máximo

#### isUsingMidSideCoding

Comprueba si la codificación estéreo mid-side fija está habilitada.

**Devuelve**: `true` si la codificación mid-side está habilitada, `false` en caso contrario

#### isUsingAdaptiveMidSideCoding

Comprueba si la codificación estéreo mid-side adaptativa está habilitada.

**Devuelve**: `true` si la codificación mid-side adaptativa está habilitada, `false` en caso contrario

#### isUsingExhaustiveModel

Comprueba si la búsqueda exhaustiva de modelos está habilitada.

**Devuelve**: `true` si la búsqueda exhaustiva de modelos está habilitada, `false` en caso contrario

## Ejemplos de Uso

### Ejemplo en C# - Archivo de Alta Calidad

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class FLACArchivalEncoder
{
    public void ConfigureHighQualityArchival(IBaseFilter audioEncoder)
    {
        // Consultar la interfaz del codificador FLAC
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null)
        {
            Console.WriteLine("Error: El filtro no admite IFLACEncodeSettings");
            return;
        }

        // Comprobar si podemos modificar la configuración
        if (!flacEncoder.canModifySettings())
        {
            Console.WriteLine("Advertencia: No se puede modificar la configuración del codificador en este momento");
            return;
        }

        // Configuración de archivo de alta calidad
        flacEncoder.setEncodingLevel(8);              // Compresión máxima
        flacEncoder.setLPCOrder(12);                  // Buen orden LPC para música
        flacEncoder.setBlockSize(4096);               // Tamaño de bloque estándar para 44.1kHz
        flacEncoder.useAdaptiveMidSideCoding(true);   // Mid-side adaptativo para estéreo
        flacEncoder.useExhaustiveModelSearch(true);   // Mejor compresión posible
        flacEncoder.setRicePartitionOrder(0, 8);      // Rango de partición Rice máximo

        Console.WriteLine("Codificador FLAC configurado para archivo de alta calidad:");
        Console.WriteLine($"  Nivel de Codificación: {flacEncoder.encoderLevel()}");
        Console.WriteLine($"  Orden LPC: {flacEncoder.LPCOrder()}");
        Console.WriteLine($"  Tamaño de Bloque: {flacEncoder.blockSize()}");
        Console.WriteLine($"  Mid-Side Adaptativo: {flacEncoder.isUsingAdaptiveMidSideCoding()}");
        Console.WriteLine($"  Búsqueda Exhaustiva: {flacEncoder.isUsingExhaustiveModel()}");
        Console.WriteLine($"  Partición Rice: {flacEncoder.riceMin()}-{flacEncoder.riceMax()}");
    }
}
```

### Ejemplo en C# - Codificación Rápida en Tiempo Real

```csharp
public class FLACRealTimeEncoder
{
    public void ConfigureFastEncoding(IBaseFilter audioEncoder)
    {
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null || !flacEncoder.canModifySettings())
            return;

        // Configuración de codificación rápida para uso en tiempo real
        flacEncoder.setEncodingLevel(2);              // Codificación rápida
        flacEncoder.setLPCOrder(8);                   // LPC más bajo para velocidad
        flacEncoder.setBlockSize(1152);               // Bloques más pequeños para menor latencia
        flacEncoder.useAdaptiveMidSideCoding(true);   // Aún buena compresión
        flacEncoder.useExhaustiveModelSearch(false);  // Deshabilitar para velocidad
        flacEncoder.setRicePartitionOrder(0, 4);      // Rango de partición Rice reducido

        Console.WriteLine("Codificador FLAC configurado para codificación rápida en tiempo real");
        Console.WriteLine($"  Nivel de Codificación: {flacEncoder.encoderLevel()}");
        Console.WriteLine($"  Orden LPC: {flacEncoder.LPCOrder()}");
        Console.WriteLine($"  Tamaño de Bloque: {flacEncoder.blockSize()} (menor latencia)");
    }
}
```

### Ejemplo en C# - Codificación de Música Equilibrada

```csharp
public class FLACMusicEncoder
{
    public void ConfigureBalancedMusic(IBaseFilter audioEncoder)
    {
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null || !flacEncoder.canModifySettings())
            return;

        // Configuración equilibrada para codificación de música (buena compresión, velocidad razonable)
        flacEncoder.setEncodingLevel(5);              // Compresión equilibrada
        flacEncoder.setLPCOrder(12);                  // LPC estándar para música
        flacEncoder.setBlockSize(4096);               // Óptimo para 44.1kHz
        flacEncoder.useAdaptiveMidSideCoding(true);   // Mid-side adaptativo
        flacEncoder.useExhaustiveModelSearch(false);  // No necesario para música
        flacEncoder.setRicePartitionOrder(0, 6);      // Buen rango de partición Rice

        Console.WriteLine("Codificador FLAC configurado para codificación de música equilibrada");
    }
}
```

### Ejemplo en C++ - Archivo de Alta Calidad

```cpp
#include <dshow.h>
#include <iostream>
#include "IFLACEncodeSettings.h"

void ConfigureHighQualityFLAC(IBaseFilter* pAudioEncoder)
{
    IFLACEncodeSettings* pFLACEncoder = NULL;
    HRESULT hr = S_OK;

    // Consultar la interfaz del codificador FLAC
    hr = pAudioEncoder->QueryInterface(IID_IFLACEncodeSettings,
                                       (void**)&pFLACEncoder);
    if (FAILED(hr) || !pFLACEncoder)
    {
        std::cout << "Error: El filtro no admite IFLACEncodeSettings" << std::endl;
        return;
    }

    // Comprobar si podemos modificar la configuración
    if (!pFLACEncoder->canModifySettings())
    {
        std::cout << "Advertencia: No se puede modificar la configuración del codificador" << std::endl;
        pFLACEncoder->Release();
        return;
    }

    // Configurar ajustes de archivo de alta calidad
    pFLACEncoder->setEncodingLevel(8);              // Compresión máxima
    pFLACEncoder->setLPCOrder(12);                  // Buen orden LPC
    pFLACEncoder->setBlockSize(4096);               // Tamaño de bloque estándar
    pFLACEncoder->useAdaptiveMidSideCoding(TRUE);   // Mid-side adaptativo
    pFLACEncoder->useExhaustiveModelSearch(TRUE);   // Mejor compresión
    pFLACEncoder->setRicePartitionOrder(0, 8);      // Rango máximo

    // Mostrar configuración
    std::cout << "Codificador FLAC configurado para archivo de alta calidad:" << std::endl;
    std::cout << "  Nivel de Codificación: " << pFLACEncoder->encoderLevel() << std::endl;
    std::cout << "  Orden LPC: " << pFLACEncoder->LPCOrder() << std::endl;
    std::cout << "  Tamaño de Bloque: " << pFLACEncoder->blockSize() << std::endl;
    std::cout << "  Mid-Side Adaptativo: "
              << (pFLACEncoder->isUsingAdaptiveMidSideCoding() ? "Sí" : "No") << std::endl;
    std::cout << "  Búsqueda Exhaustiva: "
              << (pFLACEncoder->isUsingExhaustiveModel() ? "Sí" : "No") << std::endl;

    pFLACEncoder->Release();
}
```

### Ejemplo en C++ - Codificación Rápida en Tiempo Real

```cpp
void ConfigureFastFLAC(IBaseFilter* pAudioEncoder)
{
    IFLACEncodeSettings* pFLACEncoder = NULL;
    HRESULT hr = pAudioEncoder->QueryInterface(IID_IFLACEncodeSettings,
                                               (void**)&pFLACEncoder);
    if (SUCCEEDED(hr) && pFLACEncoder)
    {
        if (pFLACEncoder->canModifySettings())
        {
            // Configuración de codificación rápida
            pFLACEncoder->setEncodingLevel(2);              // Rápido
            pFLACEncoder->setLPCOrder(8);                   // LPC más bajo
            pFLACEncoder->setBlockSize(1152);               // Bloques más pequeños
            pFLACEncoder->useAdaptiveMidSideCoding(TRUE);   // Aún bueno
            pFLACEncoder->useExhaustiveModelSearch(FALSE);  // Deshabilitado para velocidad
            pFLACEncoder->setRicePartitionOrder(0, 4);      // Rango reducido

            std::cout << "Codificador FLAC configurado para codificación rápida en tiempo real" << std::endl;
        }
        pFLACEncoder->Release();
    }
}
```

### Ejemplo en Delphi - Archivo de Alta Calidad

```delphi
uses
  DirectShow9, ActiveX;

procedure ConfigureHighQualityFLAC(AudioEncoder: IBaseFilter);
var
  FLACEncoder: IFLACEncodeSettings;
  hr: HRESULT;
begin
  // Consultar la interfaz del codificador FLAC
  hr := AudioEncoder.QueryInterface(IID_IFLACEncodeSettings, FLACEncoder);
  if Failed(hr) or (FLACEncoder = nil) then
  begin
    WriteLn('Error: El filtro no admite IFLACEncodeSettings');
    Exit;
  end;

  try
    // Comprobar si podemos modificar la configuración
    if not FLACEncoder.canModifySettings then
    begin
      WriteLn('Advertencia: No se puede modificar la configuración del codificador');
      Exit;
    end;

    // Configurar ajustes de archivo de alta calidad
    FLACEncoder.setEncodingLevel(8);              // Compresión máxima
    FLACEncoder.setLPCOrder(12);                  // Buen orden LPC
    FLACEncoder.setBlockSize(4096);               // Tamaño de bloque estándar
    FLACEncoder.useAdaptiveMidSideCoding(True);   // Mid-side adaptativo
    FLACEncoder.useExhaustiveModelSearch(True);   // Mejor compresión
    FLACEncoder.setRicePartitionOrder(0, 8);      // Rango máximo

    // Mostrar configuración
    WriteLn('Codificador FLAC configurado para archivo de alta calidad:');
    WriteLn('  Nivel de Codificación: ', FLACEncoder.encoderLevel);
    WriteLn('  Orden LPC: ', FLACEncoder.LPCOrder);
    WriteLn('  Tamaño de Bloque: ', FLACEncoder.blockSize);
    WriteLn('  Mid-Side Adaptativo: ', FLACEncoder.isUsingAdaptiveMidSideCoding);
    WriteLn('  Búsqueda Exhaustiva: ', FLACEncoder.isUsingExhaustiveModel);

  finally
    FLACEncoder := nil;
  end;
end;
```

### Ejemplo en Delphi - Codificación de Música Equilibrada

```delphi
procedure ConfigureBalancedMusicFLAC(AudioEncoder: IBaseFilter);
var
  FLACEncoder: IFLACEncodeSettings;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IFLACEncodeSettings, FLACEncoder)) then
  begin
    try
      if FLACEncoder.canModifySettings then
      begin
        // Configuración equilibrada para música
        FLACEncoder.setEncodingLevel(5);              // Equilibrado
        FLACEncoder.setLPCOrder(12);                  // Estándar para música
        FLACEncoder.setBlockSize(4096);               // Óptimo
        FLACEncoder.useAdaptiveMidSideCoding(True);   // Adaptativo
        FLACEncoder.useExhaustiveModelSearch(False);  // No necesario
        FLACEncoder.setRicePartitionOrder(0, 6);      // Buen rango

        WriteLn('Codificador FLAC configurado para codificación de música equilibrada');
      end;
    finally
      FLACEncoder := nil;
    end;
  end;
end;
```

## Mejores Prácticas

### Selección del Nivel de Codificación

**Nivel 0-2**: Codificación rápida, adecuada para aplicaciones en tiempo real
- Úselo cuando la velocidad de codificación sea crítica
- Compresión típica: 50-55% del tamaño original

**Nivel 3-5**: Codificación equilibrada (recomendada para la mayoría de los usos)
- Buen equilibrio entre velocidad y compresión
- Compresión típica: 45-50% del tamaño original
- **Se recomienda el Nivel 5** para archivo de propósito general

**Nivel 6-8**: Compresión máxima, codificación más lenta
- Úselo para archivo a largo plazo donde el espacio de almacenamiento es crítico
- Compresión típica: 40-45% del tamaño original
- La codificación puede ser 2-5 veces más lenta que el nivel 5

### Codificación Estéreo Mid-Side

- **Habilite siempre** `useAdaptiveMidSideCoding` para audio estéreo
- El modo adaptativo determina automáticamente cuándo ayuda la codificación mid-side
- Proporciona mejor compresión que el modo mid-side fijo
- Sin penalización significativa de rendimiento

### Recomendaciones de Orden LPC

**Música y Audio General**:
- Use orden LPC 12 para la mayoría de la codificación de música
- Órdenes más altos (16-32) proporcionan un beneficio mínimo para la música
- Órdenes más bajos (8) son adecuados para voz

**Clásica y Alto Rango Dinámico**:
- Considere orden LPC 16-32 para grabaciones orquestales
- Proporciona mejor predicción para contenido armónico complejo

### Selección del Tamaño de Bloque

**Consideraciones de Frecuencia de Muestreo**:
- 44.1kHz: 4096 muestras (predeterminado, ~93ms)
- 48kHz: 4608 muestras (~96ms)
- 96kHz: 4608-8192 muestras

**Requisitos de Latencia**:
- Tiempo real: 192-1152 muestras
- Archivo estándar: 4096 muestras
- Compresión máxima: 4608 muestras

### Búsqueda Exhaustiva de Modelos

**Cuándo Habilitar**:
- Proyectos de archivo críticos donde cada byte cuenta
- Tiempo de codificación ilimitado disponible
- La reducción del tamaño del archivo es primordial

**Cuándo Deshabilitar** (recomendado para la mayoría de los usuarios):
- Codificación en tiempo real o casi en tiempo real
- Grandes proyectos de codificación por lotes
- La mejora de la compresión es típicamente <3%
- El tiempo de codificación aumenta 2-4 veces

### Orden de Partición Rice

**Codificación Rápida**: `setRicePartitionOrder(0, 3)`
**Codificación Estándar**: `setRicePartitionOrder(0, 6)` (recomendado)
**Compresión Máxima**: `setRicePartitionOrder(0, 8)`

## Solución de Problemas

### La Configuración No Se Puede Modificar

**Síntoma**: `canModifySettings()` devuelve `false`

**Causas**:
1. El gráfico de filtros ya se está ejecutando
2. El codificador está procesando audio activamente
3. El filtro está en un estado incorrecto

**Soluciones**:
- Detenga el gráfico de filtros antes de modificar la configuración
- Configure el codificador antes de conectar los pines del filtro
- Consulte la configuración antes de iniciar la reproducción/captura

### Mala Relación de Compresión

**Síntoma**: Los archivos FLAC son más grandes de lo esperado

**Posibles Causas**:
1. Nivel de codificación bajo (0-2)
2. El audio de origen ya está comprimido (MP3, AAC)
3. El audio de origen tiene un alto nivel de ruido
4. Tamaño de bloque inapropiado para la frecuencia de muestreo

**Soluciones**:
- Aumente el nivel de codificación a 5-8
- **Nunca recodifique audio ya comprimido** - FLAC no puede mejorar la calidad
- Use reducción de ruido en el audio de origen antes de codificar
- Ajuste el tamaño del bloque para que coincida con la frecuencia de muestreo (ver recomendaciones arriba)

### Codificación Demasiado Lenta

**Síntoma**: La codificación en tiempo real no puede seguir el ritmo del flujo de audio

**Soluciones**:
1. Reduzca el nivel de codificación a 0-3
2. Deshabilite la búsqueda exhaustiva de modelos
3. Reduzca el orden LPC a 8
4. Reduzca el máximo de partición Rice a 4
5. Use tamaños de bloque más pequeños (1152 o menos)

### Chasquidos o Clics en la Salida Codificada

**Síntoma**: Artefactos audibles en archivos FLAC codificados

**Posibles Causas**:
1. El codificador no puede procesar lo suficientemente rápido (desbordamiento de búfer)
2. Tamaño de bloque incompatible con la frecuencia de muestreo
3. Problemas de rendimiento del hardware
