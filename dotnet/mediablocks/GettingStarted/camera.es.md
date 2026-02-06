---
title: Creación de Aplicaciones de Cámara con Media Blocks SDK
description: Crea aplicaciones de cámara con Media Blocks SDK: enumeración de dispositivos, selección de formato, renderizado y captura multiplataforma.
---

# Construcción de Aplicaciones de Cámara con Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Esta guía completa demuestra cómo crear una aplicación de visualización de cámara completamente funcional usando Media Blocks SDK .Net. El SDK proporciona un framework robusto para capturar, procesar y mostrar flujos de video en múltiples plataformas incluyendo Windows, macOS, iOS y Android.

## Visión General de la Arquitectura

Para crear una aplicación de visor de cámara, necesitará entender dos componentes fundamentales:

1. **System Video Source** - Captura el flujo de video desde dispositivos de cámara conectados
2. **Video Renderer** - Muestra el video capturado en pantalla con configuraciones ajustables

Estos componentes trabajan juntos dentro de una arquitectura de pipeline que gestiona el procesamiento de medios.

## Bloques de Medios Esenciales

Para construir una aplicación de cámara, necesita agregar los siguientes bloques a su pipeline:

- **[Bloque System Video Source](../Sources/index.md)** - Se conecta y lee desde dispositivos de cámara
- **[Bloque Video Renderer](../VideoRendering/index.md)** - Muestra el video con opciones de renderizado configurables

## Configuración del Pipeline

### Creando el Pipeline Base

Primero, cree un objeto pipeline que gestionará el flujo de medios:

```csharp
using VisioForge.Core.MediaBlocks;

// Inicializar el pipeline
var pipeline = new MediaBlocksPipeline();

// Agregar manejo de errores
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Error del pipeline: {args.Message}");
};
```

### Enumeración de Dispositivos de Cámara

Antes de agregar una fuente de cámara, necesita enumerar los dispositivos disponibles y seleccionar uno:

```csharp
// Obtener todos los dispositivos de video disponibles de forma asíncrona
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();

// Mostrar dispositivos disponibles (útil para selección del usuario)
foreach (var device in videoDevices)
{
    Console.WriteLine($"Dispositivo: {device.Name} [{device.API}]");
}

// Seleccionar el primer dispositivo disponible
var selectedDevice = videoDevices[0];
```

### Selección de Formato de Cámara

Cada cámara soporta diferentes resoluciones y tasas de frames. Puede enumerar y seleccionar el formato óptimo:

```csharp
// Mostrar formatos disponibles para el dispositivo seleccionado
foreach (var format in selectedDevice.VideoFormats)
{
    Console.WriteLine($"Formato: {format.Width}x{format.Height} {format.Format}");
    
    // Mostrar tasas de frames disponibles para este formato
    foreach (var frameRate in format.FrameRateList)
    {
        Console.WriteLine($"  Tasa de Frames: {frameRate}");
    }
}

// Seleccionar el formato óptimo (en este ejemplo, buscamos resolución HD)
var hdFormat = selectedDevice.GetHDVideoFormatAndFrameRate(out var frameRate);
var formatToUse = hdFormat ?? selectedDevice.VideoFormats[0];
```

## Configuración de Ajustes de Cámara

### Creando Configuraciones de Fuente

Configure los ajustes de fuente de cámara con su dispositivo y formato seleccionados:

```csharp
// Crear configuraciones de cámara con el dispositivo y formato seleccionados
var videoSourceSettings = new VideoCaptureDeviceSourceSettings(selectedDevice)
{
    Format = formatToUse.ToFormat()
};

// Establecer la tasa de frames deseada (seleccionando la más alta disponible)
if (formatToUse.FrameRateList.Count > 0)
{
    videoSourceSettings.Format.FrameRate = formatToUse.FrameRateList.Max();
}

// Opcional: Habilitar forzar tasa de frames para mantener temporización consistente
videoSourceSettings.Format.ForceFrameRate = true;

// Configuraciones específicas de plataforma
#if __ANDROID__
// Configuraciones específicas de Android
videoSourceSettings.VideoStabilization = true;
#elif __IOS__ && !__MACCATALYST__
// Configuraciones específicas de iOS
videoSourceSettings.Position = IOSVideoSourcePosition.Back;
videoSourceSettings.Orientation = IOSVideoSourceOrientation.Portrait;
#endif
```

### Creando el Bloque Video Source

Ahora cree el bloque system video source con sus configuraciones establecidas:

```csharp
// Crear el bloque de fuente de video
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);
```

## Configuración de Visualización de Video

### Creando el Video Renderer

Agregue un renderizador de video para mostrar el video capturado:

```csharp
// Crear el renderizador de video y conectarlo a su componente de UI
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Opcional: Configurar ajustes del renderizador
videoRenderer.Settings.IsSync = true;
```

### Configuración Avanzada del Renderizador

Para más control sobre el renderizado de video, puede personalizar los ajustes del renderizador:

```csharp
// Habilitar capacidades de captura de imagen
videoRenderer.Settings.EnableSnapshot = true;

// Configurar overlay de subtítulos si es necesario
videoRenderer.SubtitleEnabled = false;
```

## Conectando el Pipeline

Conecte la fuente de video al renderizador para establecer el flujo de medios:

```csharp
// Conectar la salida de la fuente de video a la entrada del renderizador
pipeline.Connect(videoSource.Output, videoRenderer.Input);
```

## Gestión del Ciclo de Vida del Pipeline

### Iniciando el Pipeline

Inicie el pipeline para comenzar a capturar y mostrar video:

```csharp
// Iniciar el pipeline de forma asíncrona
await pipeline.StartAsync();
```

### Tomando Capturas de Pantalla

Capture imágenes fijas del flujo de video:

```csharp
// Tomar una captura de pantalla y guardarla como archivo JPEG
await videoRenderer.Snapshot_SaveAsync("captura_camara.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, 90);

// O obtener la captura como bitmap para procesamiento adicional
var bitmap = await videoRenderer.Snapshot_GetAsync();
```

### Deteniendo el Pipeline

Cuando termine, detenga correctamente el pipeline:

```csharp
// Detener el pipeline de forma asíncrona
await pipeline.StopAsync();
```

## Consideraciones Específicas de Plataforma

El Media Blocks SDK soporta desarrollo multiplataforma con optimizaciones específicas:

- **Windows**: Soporta tanto APIs de Media Foundation como Kernel Streaming
- **macOS/iOS**: Utiliza AVFoundation para rendimiento óptimo
- **Android**: Proporciona acceso a características de cámara como estabilización y orientación

## Manejo de Errores y Solución de Problemas

Implemente manejo de errores apropiado para asegurar una aplicación estable:

```csharp
try
{
    // Operaciones del pipeline
    await pipeline.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error al iniciar pipeline: {ex.Message}");
    // Manejar la excepción apropiadamente
}
```

## Ejemplo de Implementación Completa

Este ejemplo demuestra una implementación completa de visor de cámara:

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

public class CameraViewerExample
{
    private MediaBlocksPipeline _pipeline;
    private SystemVideoSourceBlock _videoSource;
    private VideoRendererBlock _videoRenderer;
    
    public async Task InitializeAsync(IVideoView videoView)
    {
        // Crear pipeline
        _pipeline = new MediaBlocksPipeline();
        _pipeline.OnError += (s, e) => Console.WriteLine(e.Message);
        
        // Enumerar dispositivos
        var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (devices.Length == 0)
        {
            throw new Exception("No se encontraron dispositivos de cámara");
        }
        
        // Seleccionar dispositivo y formato
        var device = devices[0];
        var format = device.GetHDOrAnyVideoFormatAndFrameRate(out var frameRate);
        
        // Crear configuraciones
        var settings = new VideoCaptureDeviceSourceSettings(device);
        if (format != null)
        {
            settings.Format = format.ToFormat();
            if (frameRate != null && !frameRate.IsEmpty)
            {
                settings.Format.FrameRate = frameRate;
            }
        }
        
        // Crear bloques
        _videoSource = new SystemVideoSourceBlock(settings);
        _videoRenderer = new VideoRendererBlock(_pipeline, videoView);
        
        // Construir pipeline
        _pipeline.AddBlock(_videoSource);
        _pipeline.AddBlock(_videoRenderer);
        _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);
        
        // Iniciar pipeline
        await _pipeline.StartAsync();
    }
    
    public async Task StopAsync()
    {
        if (_pipeline != null)
        {
            await _pipeline.StopAsync();
            _pipeline.Dispose();
        }
    }
    
    public async Task<bool> TakeSnapshotAsync(string filename)
    {
        return await _videoRenderer.Snapshot_SaveAsync(filename, 
            SkiaSharp.SKEncodedImageFormat.Jpeg, 90);
    }
}
```

## Conclusión

Con Media Blocks SDK .Net, construir aplicaciones de cámara potentes se vuelve sencillo. La arquitectura basada en componentes proporciona flexibilidad y rendimiento en todas las plataformas mientras abstrae las complejidades de la integración de dispositivos de cámara.

Para ejemplos de código fuente completos, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo).
