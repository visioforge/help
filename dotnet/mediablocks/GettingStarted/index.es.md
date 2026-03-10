---
title: Guía de Inicio Rápido de Pipeline Multimedia en C# .NET
description: Comience con VisioForge Media Blocks SDK: instalación, arquitectura de pipeline, conexión de bloques y tutoriales de procesamiento multimedia.
---

# Media Blocks SDK .Net - Guía de Inicio Rápido para Desarrolladores

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Esta guía proporciona un recorrido completo para integrar el Media Blocks SDK .Net en tus aplicaciones. El SDK está construido alrededor de una arquitectura de pipeline modular, permitiéndote crear, conectar y gestionar bloques de procesamiento multimedia para video, audio y más. Ya sea que estés construyendo herramientas de procesamiento de video, soluciones de streaming o aplicaciones multimedia, esta guía te ayudará a comenzar rápida y correctamente.

## Proceso de Instalación del SDK

El SDK se distribuye como un paquete NuGet para fácil integración en tus proyectos .Net. Instálalo usando:

```bash
dotnet add package VisioForge.DotNet.MediaBlocks
```

Para requisitos específicos de plataforma y detalles adicionales de instalación, consulta la [guía de instalación detallada](../../install/index.md).

## Conceptos Principales y Arquitectura

### MediaBlocksPipeline

- La clase central para gestionar el flujo de datos de medios entre bloques de procesamiento.
- Maneja la adición de bloques, conexión, gestión de estado y manejo de eventos.
- Implementa `IMediaBlocksPipeline` y expone eventos como `OnError`, `OnStart`, `OnPause`, `OnResume`, `OnStop` y `OnLoop`.

### MediaBlock e Interfaces

- Cada unidad de procesamiento es un `MediaBlock` (o una clase derivada), implementando la interfaz `IMediaBlock`.
- Interfaces clave:
  - `IMediaBlock`: Interfaz base para todos los bloques. Define propiedades para `Name`, `Type`, `Input`, `Inputs`, `Output`, `Outputs` y métodos para contexto de pipeline y exportación YAML.
  - `IMediaBlockDynamicInputs`: Para bloques que soportan creación dinámica de entradas (ej. mezcladores).
  - `IMediaBlockInternals`/`IMediaBlockInternals2`: Para gestión interna del pipeline, construcción y lógica post-conexión.
  - `IMediaBlockRenderer`: Para bloques que renderizan medios (ej. renderizadores de video/audio), con propiedad para controlar sincronización de stream.
  - `IMediaBlockSink`/`IMediaBlockSource`: Para bloques que actúan como sinks (salidas) o fuentes (entradas).
  - `IMediaBlockSettings`: Para objetos de configuración que pueden crear bloques.

### Pads y Tipos de Medios

- Los bloques se conectan vía objetos `MediaBlockPad`, que tienen una dirección (`In`/`Out`) y un tipo de medio (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`).
- Los pads pueden conectarse/desconectarse, y su estado puede consultarse.

### Tipos de Bloques

- El SDK proporciona una amplia gama de tipos de bloques incorporados (ver enum `MediaBlockType` en el código fuente) para fuentes, sinks, renderizadores, efectos y más.

## Creando y Gestionando un Pipeline

### 1. Inicializar el SDK (si es requerido)

```csharp
using VisioForge.Core;

// Inicializar el SDK al inicio de la aplicación
VisioForgeX.InitSDK();
```

### 2. Crear un Pipeline y Bloques

```csharp
using VisioForge.Core.MediaBlocks;

// Crear una nueva instancia de pipeline
var pipeline = new MediaBlocksPipeline();

// Ejemplo: Crear una fuente de video virtual y un renderizador de video
var virtualSource = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // VideoView1 es tu control de UI

// Agregar bloques al pipeline
pipeline.AddBlock(virtualSource);
pipeline.AddBlock(videoRenderer);
```

### 3. Conectar Bloques

```csharp
// Conectar la salida de la fuente a la entrada del renderizador
pipeline.Connect(virtualSource.Output, videoRenderer.Input);
```

- También puedes usar `pipeline.Connect(sourceBlock, targetBlock)` para conectar pads predeterminados, o conectar múltiples pads para grafos complejos.
- Para bloques que soportan entradas dinámicas, usa la interfaz `IMediaBlockDynamicInputs`.

### 4. Iniciar y Detener el Pipeline

```csharp
// Iniciar el pipeline asíncronamente
await pipeline.StartAsync();

// ... después, detener el procesamiento
await pipeline.StopAsync();
```

### 5. Limpieza de Recursos

```csharp
// Eliminar el pipeline cuando termines
pipeline.Dispose();
```

### 6. Limpieza del SDK (si es requerido)

```csharp
// Liberar todos los recursos del SDK al cerrar la aplicación
VisioForgeX.DestroySDK();
```

## Manejo de Errores y Eventos

- Suscríbete a eventos del pipeline para manejo robusto de errores y estado:

```csharp
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Error del pipeline: {args.Message}");
    // Implementa tu lógica de manejo de errores aquí
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

## Características Avanzadas

- **Adición/Eliminación Dinámica de Bloques:** Puedes agregar o eliminar bloques en tiempo de ejecución según sea necesario.
- **Gestión de Pads:** Usa métodos de `MediaBlockPad` para consultar y gestionar conexiones de pads.
- **Selección de Decodificador Hardware/Software:** Usa métodos auxiliares en `MediaBlocksPipeline` para aceleración de hardware.
- **Reproducción de Segmentos:** Establece propiedades `StartPosition` y `StopPosition` para reproducción parcial.
- **Depuración:** Exporta grafos del pipeline para depuración usando los métodos proporcionados.

## Ejemplo: Configuración Mínima de Pipeline

```csharp
using VisioForge.Core.MediaBlocks;

var pipeline = new MediaBlocksPipeline();
var source = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var renderer = new VideoRendererBlock(pipeline, videoViewControl);

pipeline.AddBlock(source);
pipeline.AddBlock(renderer);
pipeline.Connect(source.Output, renderer.Input);
await pipeline.StartAsync();
// ...
await pipeline.StopAsync();
pipeline.Dispose();
```

## Referencia: Interfaces Clave

- `IMediaBlock`: Interfaz base para todos los bloques.
- `IMediaBlockDynamicInputs`: Para bloques con soporte de entrada dinámica.
- `IMediaBlockInternals`, `IMediaBlockInternals2`: Para lógica interna del pipeline.
- `IMediaBlockRenderer`: Para bloques renderizadores.
- `IMediaBlockSink`, `IMediaBlockSource`: Para bloques sink/fuente.
- `IMediaBlockSettings`: Para objetos de configuración de bloques.
- `IMediaBlocksPipeline`: Interfaz principal del pipeline.
- `MediaBlockPad`, `MediaBlockPadDirection`, `MediaBlockPadMediaType`: Para gestión de pads.

## Lecturas Adicionales y Ejemplos

- [Implementación Completa de Pipeline](pipeline.md)
- [Guía de Desarrollo de Reproductor de Medios](player.md)
- [Tutorial de Aplicación de Visor de Cámara](camera.md)
- [Repositorio GitHub con ejemplos de código](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)

Para una lista completa de tipos de bloques y uso avanzado, consulta la referencia de API del SDK y el código fuente.
