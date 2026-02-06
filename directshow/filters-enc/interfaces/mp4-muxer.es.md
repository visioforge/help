---
title: Referencia de Interfaz: DirectShow MP4 Muxer
description: Interfaces DirectShow de MP4 muxer con configuración de hilos, corrección de tiempo y opciones de streaming en vivo para salida de contenedor MP4.
---

# Referencia de Interfaz MP4 Muxer

## Resumen

Los filtros DirectShow MP4 muxer proporcionan interfaces para configurar la salida del contenedor MP4 (MPEG-4 Parte 14). Estas interfaces permiten a los desarrolladores controlar el comportamiento de los hilos, la corrección de tiempo y el manejo especial para escenarios de streaming en vivo.

Hay dos interfaces de muxer disponibles:
- **IMP4MuxerConfig**: Configuración básica de MP4 muxer para hilos y tiempo
- **IMP4V10MuxerConfig**: Configuración avanzada para muxer versión 10 con banderas de tiempo y control de streaming en vivo

## Interfaz IMP4MuxerConfig

### Resumen

La interfaz **IMP4MuxerConfig** proporciona configuración básica para multiplexación MP4, controlando la operación de un solo hilo y el comportamiento de corrección de tiempo.

**GUID de Interfaz**: `{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}`

**Hereda De**: `IUnknown`

### Definiciones de Interfaz

#### Definición C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interfaz de configuración de MP4 muxer.
    /// Controla el comportamiento de hilos y tiempo para la creación de contenedores MP4.
    /// </summary>
    [ComImport]
    [Guid("99DC9BE5-0AFA-45d4-8370-AB021FB07CF4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMP4MuxerConfig
    {
        /// <summary>
        /// Obtiene el estado de procesamiento de un solo hilo.
        /// </summary>
        /// <param name="pValue">Recibe true si el modo de un solo hilo está habilitado, false en caso contrario</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int get_SingleThread([Out] [MarshalAs(UnmanagedType.Bool)] out bool pValue);

        /// <summary>
        /// Habilita o deshabilita el procesamiento de un solo hilo.
        /// Cuando está habilitado, todas las operaciones del muxer se ejecutan en un solo hilo para un comportamiento determinista.
        /// </summary>
        /// <param name="value">True para habilitar el modo de un solo hilo, false para multi-hilo</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int put_SingleThread([In] [MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Obtiene el estado de corrección de tiempo.
        /// </summary>
        /// <param name="pValue">Recibe true si la corrección de tiempo está habilitada, false en caso contrario</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int get_CorrectTiming([Out] [MarshalAs(UnmanagedType.Bool)] out bool pValue);

        /// <summary>
        /// Habilita o deshabilita la corrección de tiempo.
        /// Cuando está habilitado, el muxer ajusta las marcas de tiempo para corregir la deriva y las inconsistencias de tiempo.
        /// </summary>
        /// <param name="value">True para habilitar la corrección de tiempo, false para deshabilitar</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int put_CorrectTiming([In] [MarshalAs(UnmanagedType.Bool)] bool value);
    }
}
```

#### Definición C++

```cpp
#include <unknwn.h>

// {99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}
DEFINE_GUID(IID_IMP4MuxerConfig,
    0x99dc9be5, 0x0afa, 0x45d4, 0x83, 0x70, 0xab, 0x02, 0x1f, 0xb0, 0x7c, 0xf4);

/// <summary>
/// Interfaz de configuración de MP4 muxer.
/// Controla el comportamiento de hilos y tiempo.
/// </summary>
DECLARE_INTERFACE_(IMP4MuxerConfig, IUnknown)
{
    /// <summary>
    /// Obtiene el estado de procesamiento de un solo hilo.
    /// </summary>
    /// <param name="pValue">Puntero para recibir el estado habilitado de un solo hilo</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(get_SingleThread)(THIS_
        BOOL* pValue
        ) PURE;

    /// <summary>
    /// Habilita o deshabilita el procesamiento de un solo hilo.
    /// </summary>
    /// <param name="value">TRUE para habilitar el modo de un solo hilo, FALSE para multi-hilo</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(put_SingleThread)(THIS_
        BOOL value
        ) PURE;

    /// <summary>
    /// Obtiene el estado de corrección de tiempo.
    /// </summary>
    /// <param name="pValue">Puntero para recibir el estado habilitado de corrección de tiempo</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(get_CorrectTiming)(THIS_
        BOOL* pValue
        ) PURE;

    /// <summary>
    /// Habilita o deshabilita la corrección de tiempo.
    /// </summary>
    /// <param name="value">TRUE para habilitar la corrección de tiempo, FALSE para deshabilitar</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(put_CorrectTiming)(THIS_
        BOOL value
        ) PURE;
};
```

#### Definición Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IMP4MuxerConfig: TGUID = '{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}';

type
  /// <summary>
  /// Interfaz de configuración de MP4 muxer.
  /// </summary>
  IMP4MuxerConfig = interface(IUnknown)
    ['{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}']

    /// <summary>
    /// Obtiene el estado de procesamiento de un solo hilo.
    /// </summary>
    function get_SingleThread(out pValue: BOOL): HRESULT; stdcall;

    /// <summary>
    /// Habilita o deshabilita el procesamiento de un solo hilo.
    /// </summary>
    function put_SingleThread(value: BOOL): HRESULT; stdcall;

    /// <summary>
    /// Obtiene el estado de corrección de tiempo.
    /// </summary>
    function get_CorrectTiming(out pValue: BOOL): HRESULT; stdcall;

    /// <summary>
    /// Habilita o deshabilita la corrección de tiempo.
    /// </summary>
    function put_CorrectTiming(value: BOOL): HRESULT; stdcall;
  end;
```

### Referencia de Métodos

#### get_SingleThread / put_SingleThread

Controla si el muxer procesa datos usando un solo hilo o múltiples hilos.

**Modo de Un Solo Hilo (habilitado)**:
- Todas las operaciones de muxing se ejecutan en un hilo
- Comportamiento determinista y predecible
- Depuración y solución de problemas más fáciles
- Rendimiento ligeramente inferior en sistemas multi-núcleo
- **Recomendado para**: Escenarios que requieren salida consistente y reproducible

**Modo Multi-Hilo (deshabilitado)**:
- El muxer puede usar múltiples hilos para procesamiento
- Mejor rendimiento en procesadores multi-núcleo
- Orden de operación no determinista
- **Recomendado para**: Codificación de alto rendimiento con múltiples flujos

**Predeterminado**: Típicamente multi-hilo (false)

**Ejemplo**:
```csharp
// Habilitar modo de un solo hilo para salida consistente
mp4Muxer.put_SingleThread(true);
```

#### get_CorrectTiming / put_CorrectTiming

Habilita o deshabilita la corrección automática de marcas de tiempo para flujos de audio y video.

**Corrección de Tiempo Habilitada (true)**:
- El muxer ajusta automáticamente las marcas de tiempo para corregir la deriva
- Corrige inconsistencias de tiempo de filtros fuente
- Asegura sincronización A/V adecuada
- Agrega pequeña sobrecarga de procesamiento
- **Recomendado para**: La mayoría de los escenarios, especialmente con fuentes en vivo

**Corrección de Tiempo Deshabilitada (false)**:
- Las marcas de tiempo pasan sin modificación
- Asume que los filtros fuente proporcionan marcas de tiempo precisas
- Rendimiento ligeramente mejor
- **Usar solo cuando**: La fuente proporciona marcas de tiempo precisas garantizadas

**Predeterminado**: Típicamente habilitado (true)

**Ejemplo**:
```csharp
// Habilitar corrección de tiempo para sincronización A/V
mp4Muxer.put_CorrectTiming(true);
```

---
## Interfaz IMP4V10MuxerConfig
### Resumen
La interfaz **IMP4V10MuxerConfig** proporciona configuración avanzada para el muxer MP4 versión 10, incluyendo banderas de anulación de tiempo y control de streaming en vivo.
**GUID de Interfaz**: `{9E26CE8B-6708-4535-AAA4-23F9A97C7937}`
**Hereda De**: `IUnknown`
### Enumeración MP4V10Flags
```csharp
/// <summary>
/// Banderas de configuración de muxer MP4 v10.
/// </summary>
[Flags]
public enum MP4V10Flags
{
    /// <summary>
    /// Sin banderas especiales.
    /// </summary>
    None = 0,
    /// <summary>
    /// Modo de anulación de tiempo - permite control manual de marcas de tiempo.
    /// </summary>
    TimeOverride = 0x00000001,
    /// <summary>
    /// Modo de ajuste de tiempo - habilita ajuste automático de marcas de tiempo.
    /// </summary>
    TimeAdjust = 0x00000002
}
```
### Definiciones de Interfaz
#### Definición C#
```csharp
using System;
using System.Runtime.InteropServices;
namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Banderas de muxer MP4 v10.
    /// </summary>
    [Flags]
    public enum MP4V10Flags
    {
        /// <summary>
        /// Predeterminado - sin banderas especiales.
        /// </summary>
        None = 0,
        /// <summary>
        /// Anulación de tiempo - permite control manual de marcas de tiempo.
        /// </summary>
        TimeOverride = 0x00000001,
        /// <summary>
        /// Ajuste de tiempo - habilita ajuste automático de marcas de tiempo.
        /// </summary>
        TimeAdjust = 0x00000002
    }
    /// <summary>
    /// Interfaz de configuración de muxer MP4 versión 10.
    /// Proporciona control de tiempo avanzado y opciones de streaming en vivo.
    /// </summary>
    [ComImport]
    [Guid("9E26CE8B-6708-4535-AAA4-23F9A97C7937")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMP4V10MuxerConfig
    {
        /// <summary>
        /// Establece las banderas de configuración del muxer.
        /// </summary>
        /// <param name="value">Combinación de valores MP4V10Flags</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetFlags([In] uint value);
        /// <summary>
        /// Obtiene las banderas de configuración actuales del muxer.
        /// </summary>
        /// <param name="pValue">Recibe las banderas actuales</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int GetFlags([Out] out uint pValue);
        /// <summary>
        /// Deshabilita optimizaciones de streaming en vivo.
        /// Cuando está deshabilitado, el muxer usa el modo de salida estándar basado en archivos.
        /// </summary>
        /// <param name="liveDisabled">True para deshabilitar el modo en vivo, false para habilitar</param>
        /// <returns>HRESULT (0 para éxito)</returns>
        [PreserveSig]
        int SetLiveDisabled([MarshalAs(UnmanagedType.Bool)] bool liveDisabled);
    }
}
```
#### Definición C++
```cpp
#include <unknwn.h>
// {9E26CE8B-6708-4535-AAA4-23F9A97C7937}
DEFINE_GUID(IID_IMP4V10MuxerConfig,
    0x9e26ce8b, 0x6708, 0x4535, 0xaa, 0xa4, 0x23, 0xf9, 0xa9, 0x7c, 0x79, 0x37);
/// <summary>
/// Banderas de muxer MP4 v10.
/// </summary>
enum MP4V10Flags
{
    MP4V10_NONE = 0,
    MP4V10_TIME_OVERRIDE = 0x00000001,
    MP4V10_TIME_ADJUST = 0x00000002
};
/// <summary>
/// Interfaz de configuración de muxer MP4 versión 10.
/// Proporciona control de tiempo avanzado y opciones de streaming en vivo.
/// </summary>
DECLARE_INTERFACE_(IMP4V10MuxerConfig, IUnknown)
{
    /// <summary>
    /// Establece las banderas de configuración del muxer.
    /// </summary>
    /// <param name="value">Combinación de valores MP4V10Flags</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(SetFlags)(THIS_
        unsigned long value
        ) PURE;
    /// <summary>
    /// Obtiene las banderas de configuración actuales del muxer.
    /// </summary>
    /// <param name="pValue">Puntero para recibir las banderas actuales</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(GetFlags)(THIS_
        unsigned long* pValue
        ) PURE;
    /// <summary>
    /// Deshabilita optimizaciones de streaming en vivo.
    /// </summary>
    /// <param name="liveDisabled">TRUE para deshabilitar el modo en vivo, FALSE para habilitar</param>
    /// <returns>S_OK para éxito</returns>
    STDMETHOD(SetLiveDisabled)(THIS_
        BOOL liveDisabled
        ) PURE;
};
```
#### Definición Delphi
```delphi
uses
  ActiveX, ComObj;
const
  IID_IMP4V10MuxerConfig: TGUID = '{9E26CE8B-6708-4535-AAA4-23F9A97C7937}';
  // Constantes MP4V10Flags
  MP4V10_NONE = 0;
  MP4V10_TIME_OVERRIDE = $00000001;
  MP4V10_TIME_ADJUST = $00000002;
type
  /// <summary>
  /// Interfaz de configuración de muxer MP4 versión 10.
  /// </summary>
  IMP4V10MuxerConfig = interface(IUnknown)
    ['{9E26CE8B-6708-4535-AAA4-23F9A97C7937}']
    /// <summary>
    /// Establece las banderas de configuración del muxer.
    /// </summary>
    function SetFlags(value: Cardinal): HRESULT; stdcall;
    /// <summary>
    /// Obtiene las banderas de configuración actuales del muxer.
    /// </summary>
    function GetFlags(out pValue: Cardinal): HRESULT; stdcall;
    /// <summary>
    /// Deshabilita optimizaciones de streaming en vivo.
    /// </summary>
    function SetLiveDisabled(liveDisabled: BOOL): HRESULT; stdcall;
  end;
```
### Referencia de Métodos
#### SetFlags / GetFlags
Establece o recupera las banderas de configuración del muxer que controlan el comportamiento del tiempo.
**Valores MP4V10Flags**:
**None (0)**:
- Operación estándar
- Manejo predeterminado de marcas de tiempo
- Sin modificaciones especiales de tiempo
**TimeOverride (0x00000001)**:
- Habilita anulación manual de marcas de tiempo
- Permite a la aplicación controlar las marcas de tiempo directamente
- Deshabilita generación automática de marcas de tiempo
- **Usar cuando**: La aplicación necesita control total sobre el tiempo
**TimeAdjust (0x00000002)**:
- Habilita ajuste automático de marcas de tiempo
- El muxer corrige la deriva e irregularidades de tiempo
- Similar a IMP4MuxerConfig::CorrectTiming
- **Usar para**: Fuentes con marcas de tiempo inconsistentes
**Combinando Banderas**:
```csharp
// Habilitar tanto anulación de tiempo como ajuste
uint flags = (uint)(MP4V10Flags.TimeOverride | MP4V10Flags.TimeAdjust);
mp4V10Muxer.SetFlags(flags);
```
#### SetLiveDisabled
Controla si el muxer opera en modo de streaming en vivo o modo basado en archivos.
**Modo En Vivo Habilitado** (liveDisabled = false):
- Optimizado para streaming en vivo/tiempo real
- Búfer mínimo
- Menor latencia
- Salida MP4 progresiva (se puede reproducir mientras se escribe)
- **Usar para**: Streaming en vivo a archivo, salida de streaming de red
**Modo En Vivo Deshabilitado** (liveDisabled = true):
- Muxing estándar basado en archivos
- Puede realizar optimización de múltiples pasadas
- Estructura MP4 completa escrita al final
- Puede requerir búsqueda en archivo de salida
- **Usar para**: Codificación basada en archivos, escenarios de post-procesamiento
**Ejemplo**:
```csharp
// Habilitar modo basado en archivos (deshabilitar optimizaciones en vivo)
mp4V10Muxer.SetLiveDisabled(true);
```
## Ejemplos de Uso
### Ejemplo C# - Creación Estándar de Archivo MP4
```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;
public class MP4MuxerStandardConfig
{
    public void ConfigureStandardMP4(IBaseFilter mp4Muxer)
    {
        // Consultar la interfaz estándar de muxer MP4
        var muxerConfig = mp4Muxer as IMP4MuxerConfig;
        if (muxerConfig == null)
        {
            Console.WriteLine("Error: El filtro no soporta IMP4MuxerConfig");
            return;
        }
        // Configurar para codificación estándar basada en archivos
        muxerConfig.put_SingleThread(false);     // Multi-hilo para rendimiento
        muxerConfig.put_CorrectTiming(true);     // Habilitar corrección de tiempo
        Console.WriteLine("MP4 muxer configurado para creación estándar de archivos");
        // Verificar configuración
        muxerConfig.get_SingleThread(out bool singleThread);
        muxerConfig.get_CorrectTiming(out bool correctTiming);
        Console.WriteLine($"  Un solo hilo: {singleThread}");
        Console.WriteLine($"  Corrección de tiempo: {correctTiming}");
    }
}
```
### Ejemplo C# - Salida Determinista
```csharp
public class MP4MuxerDeterministicConfig
{
    public void ConfigureDeterministicMP4(IBaseFilter mp4Muxer)
    {
        var muxerConfig = mp4Muxer as IMP4MuxerConfig;
        if (muxerConfig == null)
            return;
        // Configurar para salida determinista y reproducible
        muxerConfig.put_SingleThread(true);      // Un solo hilo para consistencia
        muxerConfig.put_CorrectTiming(true);     // Habilitar corrección de tiempo
        Console.WriteLine("MP4 muxer configurado para salida determinista");
        Console.WriteLine("  Adecuado para pruebas de regresión y validación");
    }
}
```
### Ejemplo C# - Streaming en Vivo a Archivo (MP4 V10)
```csharp
public class MP4V10LiveStreamingConfig
{
    public void ConfigureLiveStreaming(IBaseFilter mp4V10Muxer)
    {
        // Consultar la interfaz de muxer MP4 v10
        var muxerV10Config = mp4V10Muxer as IMP4V10MuxerConfig;
        if (muxerV10Config == null)
        {
            Console.WriteLine("Error: El filtro no soporta IMP4V10MuxerConfig");
            return;
        }
        // Configurar para streaming en vivo a archivo
        muxerV10Config.SetLiveDisabled(false);   // Habilitar modo en vivo
        // Habilitar ajuste de tiempo para fuentes en vivo
        uint flags = (uint)MP4V10Flags.TimeAdjust;
        muxerV10Config.SetFlags(flags);
        Console.WriteLine("MP4 v10 muxer configurado para streaming en vivo");
        // Verificar configuración
        muxerV10Config.GetFlags(out uint currentFlags);
        Console.WriteLine($"  Banderas: 0x{currentFlags:X8}");
        Console.WriteLine($"  Ajuste de Tiempo: {((currentFlags & (uint)MP4V10Flags.TimeAdjust) != 0)}");
    }
}
```
### Ejemplo C# - Control Manual de Marcas de Tiempo (MP4 V10)
```csharp
public class MP4V10ManualTimestampConfig
{
    public void ConfigureManualTimestamps(IBaseFilter mp4V10Muxer)
    {
        var muxerV10Config = mp4V10Muxer as IMP4V10MuxerConfig;
        if (muxerV10Config == null)
            return;
        // Configurar para control manual de marcas de tiempo
        muxerV10Config.SetLiveDisabled(true);    // Deshabilitar modo en vivo
        // Habilitar anulación de tiempo para control manual
        uint flags = (uint)MP4V10Flags.TimeOverride;
        muxerV10Config.SetFlags(flags);
        Console.WriteLine("MP4 v10 muxer configurado para control manual de marcas de tiempo");
        Console.WriteLine("  La aplicación debe proporcionar marcas de tiempo precisas");
    }
}
```
### Ejemplo C++ - Configuración Estándar
```cpp
#include <dshow.h>
#include <iostream>
#include "IMP4MuxerConfig.h"
void ConfigureMP4Muxer(IBaseFilter* pMp4Muxer)
{
    IMP4MuxerConfig* pMuxerConfig = NULL;
    HRESULT hr = S_OK;
    // Consultar la interfaz de muxer MP4
    hr = pMp4Muxer->QueryInterface(IID_IMP4MuxerConfig,
                                   (void**)&pMuxerConfig);
    if (FAILED(hr) || !pMuxerConfig)
    {
        std::cout << "Error: El filtro no soporta IMP4MuxerConfig" << std::endl;
        return;
    }
    // Configurar muxer
    pMuxerConfig->put_SingleThread(FALSE);     // Multi-hilo
    pMuxerConfig->put_CorrectTiming(TRUE);     // Habilitar corrección de tiempo
    // Verificar configuración
    BOOL singleThread, correctTiming;
    pMuxerConfig->get_SingleThread(&singleThread);
    pMuxerConfig->get_CorrectTiming(&correctTiming);
    std::cout << "MP4 muxer configurado:" << std::endl;
    std::cout << "  Un solo hilo: " << (singleThread ? "Sí" : "No") << std::endl;
    std::cout << "  Corrección de tiempo: " << (correctTiming ? "Sí" : "No") << std::endl;
    pMuxerConfig->Release();
}
```
### Ejemplo C++ - Streaming en Vivo (MP4 V10)
```cpp
#include "IMP4V10MuxerConfig.h"
void ConfigureMP4V10LiveStreaming(IBaseFilter* pMp4V10Muxer)
{
    IMP4V10MuxerConfig* pMuxerV10Config = NULL;
    HRESULT hr = pMp4V10Muxer->QueryInterface(IID_IMP4V10MuxerConfig,
                                               (void**)&pMuxerV10Config);
    if (SUCCEEDED(hr) && pMuxerV10Config)
    {
        // Configurar para streaming en vivo
        pMuxerV10Config->SetLiveDisabled(FALSE);     // Habilitar modo en vivo
        // Habilitar ajuste de tiempo
        unsigned long flags = MP4V10_TIME_ADJUST;
        pMuxerV10Config->SetFlags(flags);
        std::cout << "MP4 v10 muxer configurado para streaming en vivo" << std::endl;
        pMuxerV10Config->Release();
    }
}
```
### Ejemplo Delphi - Configuración Estándar
```delphi
uses
  DirectShow9, ActiveX;
procedure ConfigureMP4Muxer(Mp4Muxer: IBaseFilter);
var
  MuxerConfig: IMP4MuxerConfig;
  SingleThread, CorrectTiming: BOOL;
  hr: HRESULT;
begin
  // Consultar la interfaz de muxer MP4
  hr := Mp4Muxer.QueryInterface(IID_IMP4MuxerConfig, MuxerConfig);
  if Failed(hr) or (MuxerConfig = nil) then
  begin
    WriteLn('Error: El filtro no soporta IMP4MuxerConfig');
    Exit;
  end;
  try
    // Configurar muxer
    MuxerConfig.put_SingleThread(False);     // Multi-hilo
    MuxerConfig.put_CorrectTiming(True);     // Habilitar corrección de tiempo
    // Verificar configuración
    MuxerConfig.get_SingleThread(SingleThread);
    MuxerConfig.get_CorrectTiming(CorrectTiming);
    WriteLn('MP4 muxer configurado:');
    WriteLn('  Un solo hilo: ', SingleThread);
    WriteLn('  Corrección de tiempo: ', CorrectTiming);
  finally
    MuxerConfig := nil;
  end;
end;
```
### Ejemplo Delphi - Streaming en Vivo (MP4 V10)
```delphi
procedure ConfigureMP4V10LiveStreaming(Mp4V10Muxer: IBaseFilter);
var
  MuxerV10Config: IMP4V10MuxerConfig;
  Flags: Cardinal;
begin
  if Succeeded(Mp4V10Muxer.QueryInterface(IID_IMP4V10MuxerConfig, MuxerV10Config)) then
  begin
    try
      // Configurar para streaming en vivo
      MuxerV10Config.SetLiveDisabled(False);     // Habilitar modo en vivo
      // Habilitar ajuste de tiempo
      Flags := MP4V10_TIME_ADJUST;
      MuxerV10Config.SetFlags(Flags);
      WriteLn('MP4 v10 muxer configurado para streaming en vivo');
    finally
      MuxerV10Config := nil;
    end;
  end;
end;
```
## Mejores Prácticas
### Cuándo Usar IMP4MuxerConfig
**Use IMP4MuxerConfig cuando**:
- Necesita configuración básica de muxer
- Trabaja con salida MP4 estándar
- La corrección de tiempo simple es suficiente
- No necesita características avanzadas de streaming en vivo
**Configuración Típica**:
```csharp
mp4Muxer.put_SingleThread(false);    // Multi-hilo para rendimiento
mp4Muxer.put_CorrectTiming(true);    // Habilitar corrección de tiempo
```
### Cuándo Usar IMP4V10MuxerConfig
**Use IMP4V10MuxerConfig cuando**:
- Necesita control de tiempo avanzado
- Trabaja con escenarios de streaming en vivo
- Requiere anulación manual de marcas de tiempo
- Necesita salida MP4 progresiva
**Configuración de Streaming en Vivo**:
```csharp
mp4V10Muxer.SetLiveDisabled(false);               // Habilitar modo en vivo
mp4V10Muxer.SetFlags((uint)MP4V10Flags.TimeAdjust); // Ajuste automático de tiempo
```
### Un Solo Hilo vs Multi-Hilo
**Use Modo de Un Solo Hilo cuando**:
- Depura el comportamiento del muxer
- Necesita salida determinista y reproducible
- Ejecuta pruebas automatizadas
- Soluciona problemas de tiempo
**Use Modo Multi-Hilo cuando**:
- El rendimiento es crítico
- Codifica video de alta resolución (1080p+)
- El sistema tiene múltiples núcleos de CPU disponibles
- Codificación de producción estándar
### Corrección de Tiempo
**Siempre Habilite Corrección de Tiempo cuando**:
- Trabaja con fuentes en vivo (cámaras, dispositivos de captura)
- Las fuentes pueden tener inconsistencias de marcas de tiempo
- Combina múltiples flujos (audio + video)
- Necesita sincronización A/V confiable
**Puede Deshabilitar Corrección de Tiempo cuando**:
- La fuente proporciona marcas de tiempo precisas garantizadas
- Codificación basada en archivos con marcas de tiempo pre-validadas
- El rendimiento es absolutamente crítico
- Usa control manual de marcas de tiempo (bandera TimeOverride)
### Optimización de Streaming en Vivo
**Habilitar Modo En Vivo** (SetLiveDisabled = false) **cuando**:
- Codifica para streaming en tiempo real
- La salida necesita ser reproducible mientras se escribe
- Crea archivos MP4 progresivos
- La baja latencia es importante
**Deshabilitar Modo En Vivo** (SetLiveDisabled = true) **cuando**:
- Crea archivos para post-procesamiento
- Necesita estructura MP4 completa al final
- Puede realizar optimización de múltiples pasadas
- El archivo de salida solo se reproducirá después de completarse
## Solución de Problemas
### Problemas de Sincronización Audio/Video
**Síntomas**: Audio y video se desincronizan con el tiempo
**Soluciones**:
1. Habilitar corrección de tiempo: `put_CorrectTiming(true)`
2. Para muxer v10, usar bandera TimeAdjust: `SetFlags((uint)MP4V10Flags.TimeAdjust)`
3. Verificar que los filtros fuente proporcionen marcas de tiempo precisas
4. Verificar que las tasas de muestreo de audio y video sean correctas
### El Archivo No Se Puede Reproducir Mientras Se Graba
**Síntoma**: Archivo MP4 solo reproducible después de completar la codificación
**Causa**: El modo en vivo está deshabilitado
**Solución**:
- Usar interfaz IMP4V10MuxerConfig
- Habilitar modo en vivo: `SetLiveDisabled(false)`
- Esto crea archivos MP4 progresivos reproducibles durante la codificación
### Salida de Archivo Inconsistente
**Síntomas**: La misma entrada produce diferentes archivos de salida
**Causa**: Operación multi-hilo con condiciones de carrera
**Soluciones**:
1. Habilitar modo de un solo hilo: `put_SingleThread(true)`
2. Habilitar corrección de tiempo: `put_CorrectTiming(true)`
3. Usar bandera TimeAdjust para muxer v10
### Problemas de Rendimiento
**Síntomas**: Codificación más lenta de lo esperado, alto uso de CPU
**Posibles Causas**:
1. Modo de un solo hilo en sistema multi-núcleo
2. Sobrecarga excesiva de corrección de tiempo
**Soluciones**:
- Deshabilitar modo de un solo hilo: `put_SingleThread(false)`
- Si las fuentes tienen marcas de tiempo precisas, puede intentar deshabilitar la corrección de tiempo
- Asegurar que el codificador de video (no el muxer) sea el cuello de botella de rendimiento
- Considerar codificación por hardware (NVENC, QuickSync)
### Archivos MP4 Corruptos
**Síntomas**: El archivo MP4 no se reproduce o tiene errores
**Posibles Causas**:
1. Corrección de tiempo deshabilitada con malas marcas de tiempo
2. Configuración incorrecta de modo en vivo para el caso de uso
3. Muxer detenido antes de finalización adecuada
**Soluciones**:
- Habilitar corrección de tiempo para fuentes en vivo
- Coincidir configuración de modo en vivo con caso de uso (en vivo vs basado en archivos)
- Asegurar apagado adecuado del gráfico de filtros y finalización de flujo
- Verificar que todos los flujos terminen adecuadamente (enviar evento EC_COMPLETE)
---

## Ver También

- [Interfaz de Codificador H.264](h264.es.md)
- [Interfaces de Codificador AAC](aac.es.md)
- [Referencia de Muxers](../muxers-reference.es.md)
- [Resumen del Paquete de Filtros de Codificación](../index.es.md)
