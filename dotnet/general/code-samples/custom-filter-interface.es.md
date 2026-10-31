---
title: Guía de interfaces de filtros DirectShow personalizados .NET
description: Implemente interfaces de filtros DirectShow personalizados en .NET con acceso y manipulación de IBaseFilter para aplicaciones multimedia.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - Windows
  - Capture
  - C#
  - NuGet
primary_api_classes:
  - IBaseFilter
  - FilterEventArgs
  - VideoCaptureCore

---

# Uso de interfaces de filtros DirectShow personalizados

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

*Nota: La API mostrada en esta guía es la misma en todos nuestros productos SDK, incluyendo Video Capture SDK .NET, Video Edit SDK .NET y Media Player SDK .NET.*

DirectShow es un framework multimedia poderoso que permite a los desarrolladores realizar operaciones complejas en flujos de medios. Una de sus fortalezas clave es la capacidad de trabajar con interfaces de filtros personalizados, dándole control preciso sobre el procesamiento de medios. Esta guía le guiará a través de la implementación y utilización de interfaces de filtros DirectShow personalizados en sus aplicaciones .NET.

## Entender los filtros DirectShow

DirectShow usa una arquitectura basada en filtros donde cada filtro realiza una operación específica en el flujo de medios. Estos filtros están conectados en un grafo, creando un pipeline para el procesamiento de medios.

### Componentes clave de DirectShow

- **Filtro**: Un componente que procesa datos de medios
- **Pin**: Puntos de conexión entre filtros
- **Grafo de Filtros**: El pipeline completo de filtros conectados
- **IBaseFilter**: La interfaz fundamental que todos los filtros DirectShow implementan

## Comenzar con interfaces de filtros personalizados

Para trabajar con filtros DirectShow en .NET, necesitará:

1. Agregar las referencias apropiadas a su proyecto
2. Acceder al filtro a través de eventos apropiados
3. Convertir el filtro a la interfaz que necesita
4. Implementar su lógica personalizada

### Referencias de proyecto requeridas

Para acceder a la funcionalidad DirectShow, incluya el paquete apropiado en su proyecto:

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="X.X.X" />
```

También puede agregar la referencia al ensamblado `VisioForge.Core` directamente a su proyecto.

## Implementar acceso a interfaz de filtro personalizado

Nuestro SDK proporciona varios eventos que le dan acceso a los filtros a medida que se agregan al grafo de filtros. Aquí está cómo usarlos efectivamente:

### Acceder a filtros en Video Capture SDK

El Video Capture SDK ofrece el evento `OnFilterAdded` que se dispara cada vez que un filtro se agrega al grafo. Este evento proporciona acceso a cada filtro a través de sus argumentos de evento.

```cs
// Suscribirse al evento OnFilterAdded
videoCaptureCore.OnFilterAdded += VideoCaptureCore_OnFilterAdded;

// Implementación del manejador de eventos
private void VideoCaptureCore_OnFilterAdded(object sender, FilterEventArgs eventArgs)
{
    // Acceder a la interfaz del filtro DirectShow
    IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
    
    // Ahora puede trabajar con el filtro a través de la interfaz IBaseFilter
    if (baseFilter != null)
    {
        // El código de manipulación de filtro personalizado va aquí
    }
}
```

## Trabajar con la interfaz IBaseFilter

La interfaz `IBaseFilter` es la base de los filtros DirectShow. Esto es lo que puede hacer con ella:

### Recuperar información del filtro

```cs
private void ObtenerInfoFiltro(IBaseFilter filter)
{
    FilterInfo filterInfo = new FilterInfo();
    int hr = filter.QueryFilterInfo(out filterInfo);
    
    if (hr >= 0)
    {
        Console.WriteLine($"Nombre del Filtro: {filterInfo.achName}");
        
        // No olvidar liberar la referencia al grafo de filtros
        if (filterInfo.pGraph != null)
        {
            Marshal.ReleaseComObject(filterInfo.pGraph);
        }
    }
}
```

### Enumerar pines del filtro

```cs
private void EnumerarPinesFiltro(IBaseFilter filter)
{
    IEnumPins enumPins;
    int hr = filter.EnumPins(out enumPins);
    
    if (hr >= 0 && enumPins != null)
    {
        IPin[] pins = new IPin[1];
        int fetched;
        
        while (enumPins.Next(1, pins, out fetched) == 0 && fetched > 0)
        {
            PinInfo pinInfo = new PinInfo();
            pins[0].QueryPinInfo(out pinInfo);
            
            Console.WriteLine($"Nombre del Pin: {pinInfo.name}, Dirección: {pinInfo.dir}");
            
            // Liberar pin e info
            if (pinInfo.filter != null)
                Marshal.ReleaseComObject(pinInfo.filter);
                
            Marshal.ReleaseComObject(pins[0]);
        }
        
        Marshal.ReleaseComObject(enumPins);
    }
}
```

## Identificar el filtro correcto

Al trabajar con el evento `OnFilterAdded`, recuerde que puede ser llamado múltiples veces a medida que varios filtros se agregan al grafo. Para trabajar con un filtro específico, necesitará identificarlo correctamente:

```cs
private void VideoCaptureCore_OnFilterAdded(object sender, FilterEventArgs eventArgs)
{
    IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
    
    if (baseFilter != null)
    {
        FilterInfo filterInfo = new FilterInfo();
        baseFilter.QueryFilterInfo(out filterInfo);
        
        // Verificar si este es el filtro que estamos buscando
        if (filterInfo.achName == "Video Capture Device")
        {
            // Este es nuestro filtro objetivo, realizar operaciones específicas
            ConfigurarFiltroCaptura(baseFilter);
        }
        
        // Liberar la referencia al grafo de filtros
        if (filterInfo.pGraph != null)
        {
            Marshal.ReleaseComObject(filterInfo.pGraph);
        }
    }
}
```

## Configuración avanzada de filtros

Una vez que tiene acceso a la interfaz del filtro, puede realizar configuraciones avanzadas:

### Establecer propiedades del filtro

```cs
private void EstablecerPropiedadFiltro(IBaseFilter filter, Guid propertySet, int propertyId, object propertyValue)
{
    IKsPropertySet propertySetInterface = filter as IKsPropertySet;
    
    if (propertySetInterface != null)
    {
        // Convertir valor de propiedad a array de bytes
        byte[] propertyData = ConvertirAArrayBytes(propertyValue);
        
        // Establecer la propiedad
        int hr = propertySetInterface.Set(
            propertySet,
            propertyId,
            IntPtr.Zero,
            0,
            propertyData,
            propertyData.Length
        );
        
        Marshal.ReleaseComObject(propertySetInterface);
    }
}
```

### Recuperar propiedades del filtro

```cs
private object ObtenerPropiedadFiltro(IBaseFilter filter, Guid propertySet, int propertyId, Type propertyType)
{
    IKsPropertySet propertySetInterface = filter as IKsPropertySet;
    object result = null;
    
    if (propertySetInterface != null)
    {
        int dataSize = Marshal.SizeOf(propertyType);
        byte[] propertyData = new byte[dataSize];
        int returnedDataSize;
        
        // Obtener la propiedad
        int hr = propertySetInterface.Get(
            propertySet,
            propertyId,
            IntPtr.Zero,
            0,
            propertyData,
            propertyData.Length,
            out returnedDataSize
        );
        
        if (hr >= 0)
        {
            result = ConvertirDesdeArrayBytes(propertyData, propertyType);
        }
        
        Marshal.ReleaseComObject(propertySetInterface);
    }
    
    return result;
}
```

## Casos de uso comunes para interfaces de filtros personalizados

### Filtros de procesamiento de video

Al trabajar con video, podría necesitar acceder a propiedades específicas de dispositivos de cámara:

```cs
private void ConfigurarFiltroCaptura(IBaseFilter captureFilter)
{
    // Acceder y establecer propiedades de cámara
    IAMCameraControl cameraControl = captureFilter as IAMCameraControl;
    
    if (cameraControl != null)
    {
        // Establecer exposición
        cameraControl.Set(CameraControlProperty.Exposure, 0, CameraControlFlags.Manual);
        
        // Establecer enfoque
        cameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Manual);
        
        Marshal.ReleaseComObject(cameraControl);
    }
}
```

### Filtros de procesamiento de audio

Para procesamiento de audio, podría querer ajustar configuraciones de volumen o calidad de audio:

```cs
private void ConfigurarFiltroAudio(IBaseFilter audioFilter)
{
    // Acceder a interfaz de volumen
    IBasicAudio basicAudio = audioFilter as IBasicAudio;
    
    if (basicAudio != null)
    {
        // Establecer volumen (0 a -10000, donde 0 es máx y -10000 es mín)
        basicAudio.put_Volume(-2000); // 80% volumen
        
        Marshal.ReleaseComObject(basicAudio);
    }
}
```

## Manejar recursos correctamente

Al trabajar con interfaces DirectShow, es crucial liberar correctamente los objetos COM para prevenir fugas de memoria:

```cs
private void LiberarObjetoCom(object comObject)
{
    if (comObject != null)
    {
        Marshal.ReleaseComObject(comObject);
    }
}
```

## Ejemplo completo

Aquí hay un ejemplo más completo que demuestra encontrar y configurar un filtro de captura de video:

```cs
using System;
using System.Runtime.InteropServices;
using VisioForge.Libs.DirectShowLib;  // Namespace público — IBaseFilter, FilterInfo, IPin, etc.

public class EjemploFiltroPersonalizado
{
    private VideoCaptureCore captureCore;
    
    public void Inicializar()
    {
        captureCore = new VideoCaptureCore();
        captureCore.OnFilterAdded += CaptureCore_OnFilterAdded;
        
        // Configurar fuente
        // ...
        
        // Iniciar captura
        captureCore.Start();
    }
    
    private void CaptureCore_OnFilterAdded(object sender, FilterEventArgs eventArgs)
    {
        IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
        
        if (baseFilter != null)
        {
            // Obtener información del filtro
            FilterInfo filterInfo = new FilterInfo();
            baseFilter.QueryFilterInfo(out filterInfo);
            
            Console.WriteLine($"Filtro agregado: {filterInfo.achName}");
            
            // Verificar si este es el filtro de captura de video
            if (filterInfo.achName.Contains("Video Capture"))
            {
                ConfigurarFiltroCaptura(baseFilter);
            }
            
            // Liberar referencia al grafo de filtros
            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }
        }
    }
    
    private void ConfigurarFiltroCaptura(IBaseFilter captureFilter)
    {
        // Su código de configuración de filtro aquí
    }
    
    public void Limpiar()
    {
        if (captureCore != null)
        {
            captureCore.Stop();
            captureCore.OnFilterAdded -= CaptureCore_OnFilterAdded;
            captureCore.Dispose();
            captureCore = null;
        }
    }
}
```

## Componentes de sistema requeridos

Para usar la funcionalidad DirectShow en su aplicación, asegúrese de que sus usuarios finales tengan los siguientes componentes instalados:

- DirectX Runtime (incluido con Windows)
- Componentes redistribuibles del SDK

## Conclusión

Trabajar con interfaces de filtros DirectShow personalizados le da capacidades poderosas para el procesamiento de medios en sus aplicaciones .NET. Siguiendo los patrones descritos en esta guía, puede acceder y manipular los componentes DirectShow subyacentes para lograr control preciso sobre sus aplicaciones multimedia.

Para asistencia adicional con la implementación de estas técnicas, por favor contacte a nuestro equipo de soporte. Visite nuestro repositorio de GitHub para más ejemplos de código y ejemplos de implementación.
