---
title: Guía de inicio rápido de pipeline multimedia en C# .NET
description: Comience con VisioForge Media Blocks SDK — instalación, arquitectura de pipeline, conexión de bloques y tutoriales de procesamiento multimedia.
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - MediaBlocksPipeline
  - IMediaBlock
  - MediaBlockPad
  - IMediaBlocksPipeline
  - MediaBlock
  - SystemVideoSourceBlock
  - VideoRendererBlock
  - DeviceEnumerator

order: 20
sidebar_label: Primeros pasos
---

# Media Blocks SDK .NET - Guía de inicio rápido para desarrolladores

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Esta guía proporciona un recorrido completo para integrar el Media Blocks SDK .NET en sus aplicaciones. El SDK está construido alrededor de una arquitectura de pipeline modular, permitiéndole crear, conectar y gestionar bloques de procesamiento multimedia para video, audio y más. Ya sea que esté construyendo herramientas de procesamiento de video, soluciones de streaming o aplicaciones multimedia, esta guía le ayudará a comenzar rápida y correctamente.

## Proceso de instalación del SDK

El SDK se distribuye como un paquete NuGet para fácil integración en sus proyectos .NET. Instálelo usando:

```bash
dotnet add package VisioForge.DotNet.MediaBlocks
```

Este paquete contiene únicamente el SDK administrado. El motor nativo —GStreamer y su conjunto de plugins— viene en un paquete redistribuible aparte que se añade junto a él. Una aplicación que solo lleva el paquete administrado compila y después se detiene al arrancar: `VisioForgeX.InitSDKAsync` lanza `DllNotFoundException` antes de que se abra ninguna ventana.

Para una aplicación Windows x64:

```bash
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

El primero contiene el motor; toda aplicación lo necesita. El segundo añade los decodificadores y codificadores FFmpeg (libav) y se recomienda en toda aplicación.

Utilice la arquitectura del proceso en ejecución: `.x86` para una compilación de 32 bits y `.arm64` para una compilación para Windows on Arm (allí no hay NDI); un redistribuible de otra arquitectura no se carga en el proceso. Una aplicación AnyCPU normalmente se ejecuta como x64 en un Windows de 64 bits y como ARM64 en Windows on Arm.

Un paquete redistribuible debe ser la versión más reciente en la fecha de su versión de `VisioForge.DotNet.MediaBlocks` o antes: nunca posterior, nunca muy anterior. Fíjelo con `--version` cuando sea necesario: `dotnet add package VisioForge.CrossPlatform.Core.Windows.x64 --version <version>`.

Las demás plataformas utilizan su propio paquete redistribuible:

- **macOS, Mac Catalyst** - `VisioForge.CrossPlatform.Core.macOS`; Mac Catalyst además necesita un destino de compilación que copia las bibliotecas nativas al bundle de la aplicación ([despliegue en macOS](../../deployment-x/macOS.md))
- **iOS** - `VisioForge.CrossPlatform.Core.iOS` ([despliegue en iOS](../../deployment-x/iOS.md))
- **Android** - `VisioForge.CrossPlatform.Core.Android`, y la biblioteca de bindings de Java ([despliegue en Android](../../deployment-x/Android.md))
- **Linux** - `VisioForge.CrossPlatform.Core.Linux.x64` o `.arm64`, y GStreamer 1.20 o posterior desde la distribución ([despliegue en Ubuntu](../../deployment-x/Ubuntu.md))

Para los paquetes necesarios, consulte la [guía de instalación detallada](../../install/index.md); los opcionales están en la [guía de despliegue](../../deployment-x/index.md).

## Su primer pipeline

El pipeline mínimo útil tiene dos bloques: una fuente y un renderizador. Este muestra la primera cámara encontrada en el equipo:

```csharp
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

// Llamar desde el hilo de UI cuando la ventana ya exista.
async Task<MediaBlocksPipeline> StartCameraAsync(IVideoView videoView)
{
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];

    var pipeline = new MediaBlocksPipeline();
    var source = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(device));
    var renderer = new VideoRendererBlock(pipeline, videoView);

    pipeline.AddBlock(source);
    pipeline.AddBlock(renderer);
    pipeline.Connect(source.Output, renderer.Input);

    await pipeline.StartAsync();

    return pipeline;
}
```

- El SDK debe inicializarse primero con `await VisioForgeX.InitSDKAsync();` — vea el paso 1 más abajo.
- ¿No hay cámara conectada? Omita la línea `device` y use el patrón de prueba: `var source = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());` en lugar de `SystemVideoSourceBlock`. Las líneas de conexión e inicio son las mismas.
- Guarde el pipeline devuelto en un campo y deténgalo al cerrar la ventana: `await pipeline.StopAsync(); pipeline.Dispose();`.
- Selección de dispositivo y formato, capturas y particularidades por plataforma: [Tutorial de Aplicación de Visor de Cámara](camera.md).

## Conceptos principales y arquitectura

### MediaBlocksPipeline

- La clase central para gestionar el flujo de datos de medios entre bloques de procesamiento.
- Maneja la adición de bloques, conexión, gestión de estado y manejo de eventos.
- Implementa `IMediaBlocksPipeline` y expone eventos como `OnError`, `OnStart`, `OnPause`, `OnResume`, `OnStop` y `OnLoop`.

### MediaBlock e interfaces

- Cada unidad de procesamiento es un `MediaBlock` (o una clase derivada), implementando la interfaz `IMediaBlock`.
- Interfaces clave:
  - `IMediaBlock`: Interfaz base para todos los bloques. Define propiedades para `Name`, `Type`, `Input`, `Inputs`, `Output`, `Outputs` y un método para el contexto de pipeline del bloque.
  - `IMediaBlockDynamicInputs`: Para bloques que soportan creación dinámica de entradas (ej. mezcladores).
  - `IMediaBlockRenderer`: Para bloques que renderizan medios (ej. renderizadores de video/audio), con propiedad para controlar sincronización de stream.
  - `IMediaBlockSettings`: Para objetos de configuración que pueden crear bloques.

### Pads y tipos de medios

- Los bloques se conectan vía objetos `MediaBlockPad`, que tienen una dirección (`In`/`Out`) y un tipo de medio (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`).
- Los pads pueden conectarse/desconectarse, y su estado puede consultarse.

### Tipos de bloques

- El SDK proporciona una amplia gama de tipos de bloques incorporados (listados en el enum `MediaBlockType`) para fuentes, sinks, renderizadores, efectos y más.
- Catálogo de bloques: [Fuentes](../Sources/index.md), [Renderizado de video](../VideoRendering/index.md), [Sinks](../Sinks/index.md), [Procesamiento de video](../VideoProcessing/index.md).

## Crear y gestionar un pipeline

### 1. Inicializar el SDK

```csharp
using VisioForge.Core;

// Inicializar el SDK al inicio de la aplicación (await devuelve el control cuando las librerías nativas están cargadas)
await VisioForgeX.InitSDKAsync();
```

### 2. Crear un pipeline y bloques

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

// Crear una nueva instancia de pipeline
var pipeline = new MediaBlocksPipeline();

// Ejemplo: Crear una fuente de video virtual y un renderizador de video
var virtualSource = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // VideoView1 es su control de UI

// Agregar bloques al pipeline
pipeline.AddBlock(virtualSource);
pipeline.AddBlock(videoRenderer);
```

`VideoView1` es el control `VisioForge.Core.UI.WPF.VideoView` de una aplicación WPF. Declárelo en XAML con `xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` y `<WPF:VideoView x:Name="VideoView1" />`. Windows Forms usa `VisioForge.Core.UI.WinForms.VideoView` del mismo ensamblado; MAUI, Avalonia, WinUI y Uno incluyen su propio `VideoView` en sus paquetes de UI.

### 3. Conectar Bloques

```csharp
// Conectar la salida de la fuente a la entrada del renderizador
pipeline.Connect(virtualSource.Output, videoRenderer.Input);
```

- También puede usar `pipeline.Connect(sourceBlock, targetBlock)` para conectar pads predeterminados, o conectar múltiples pads para grafos complejos.
- Para bloques que soportan entradas dinámicas, use la interfaz `IMediaBlockDynamicInputs`.

### 4. Iniciar y detener el pipeline

```csharp
// Iniciar el pipeline asíncronamente
await pipeline.StartAsync();

// ... después, detener el procesamiento
await pipeline.StopAsync();
```

### 5. Limpieza de recursos

```csharp
// Eliminar el pipeline cuando termine
pipeline.Dispose();
```

### 6. Limpieza del SDK

```csharp
// Liberar todos los recursos del SDK al cerrar la aplicación
VisioForgeX.DestroySDK();
```

## Manejo de errores y eventos

- Suscríbase a eventos del pipeline para manejo robusto de errores y estado:

```csharp
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Error del pipeline: {args.Message}");
    // Implementar su lógica de manejo de errores aquí
};

pipeline.OnStart += (sender, args) =>
{
    Console.WriteLine("Pipeline iniciado");
};

pipeline.OnStop += (sender, args) =>
{
    Console.WriteLine("Pipeline detenido");
};
```

## Características avanzadas

- **Adición/Eliminación Dinámica de Bloques:** Puede agregar o eliminar bloques en tiempo de ejecución según sea necesario.
- **Gestión de Pads:** Use métodos de `MediaBlockPad` para consultar y gestionar conexiones de pads.
- **Selección de Decodificador Hardware/Software:** Use métodos auxiliares en `MediaBlocksPipeline` para aceleración de hardware.
- **Reproducción de Segmentos:** Establezca propiedades `StartPosition` y `StopPosition` para reproducción parcial.
- **Depuración:** Exporte grafos del pipeline para depuración usando los métodos proporcionados.

## Lecturas adicionales y ejemplos

- [Implementación Completa de Pipeline](pipeline.md)
- [Guía de Desarrollo de Reproductor de Medios](player.md)
- [Tutorial de Aplicación de Visor de Cámara](camera.md)
- [Repositorio GitHub con ejemplos de código](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)

Para cada propiedad y valor predeterminado de un bloque, consulte la [referencia de API](https://api.visioforge.org/dotnet/api/index.html).
