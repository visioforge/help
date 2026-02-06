---
title: Media Blocks Pipeline: Funcionalidad Principal
description: Cree pipelines de medios para reproducción, grabación y streaming con bloques modulares, conexiones y gestión de recursos en Media Blocks SDK.
---

# Media Blocks Pipeline: Funcionalidad Principal

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Visión General de la Estructura de Pipeline y Bloques

El Media Blocks SDK está construido alrededor de la clase `MediaBlocksPipeline`, que gestiona una colección de bloques de procesamiento modulares. Cada bloque implementa la interfaz `IMediaBlock` o una de sus variantes especializadas. Los bloques están conectados vía pads de entrada y salida, permitiendo cadenas de procesamiento de medios flexibles.

### Interfaces Principales de Bloques

- **IMediaBlock**: Interfaz base para todos los bloques. Expone propiedades para nombre, tipo, pads de entrada/salida, y métodos para conversión YAML y recuperación de contexto de pipeline.
- **IMediaBlockDynamicInputs**: Para bloques (como muxers) que pueden crear nuevas entradas dinámicamente. Métodos: `CreateNewInput(mediaType)` y `GetInput(mediaType)`.
- **IMediaBlockInternals**: Métodos internos para integración de pipeline (ej., `SetContext`, `Build`, `CleanUp`, `GetElement`, `GetCore`).
- **IMediaBlockInternals2**: Para lógica post-conexión (`PostConnect()`).
- **IMediaBlockRenderer**: Para bloques renderizadores, expone propiedad `IsSync`.
- **IMediaBlockSettings**: Para objetos de configuraciones que pueden crear un bloque (`CreateBlock()`).
- **IMediaBlockSink**: Para bloques sink, expone getter/setter de filename/URL.
- **IMediaBlockSource**: Para bloques source (actualmente solo accesores de pads comentados).

### Pads y Tipos de Medios

- **MediaBlockPad**: Representa un punto de conexión (entrada/salida) en un bloque. Tiene dirección (`In`/`Out`), tipo de medio (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`), y lógica de conexión.
- **Conexión de pads**: Use `pipeline.Connect(outputPad, inputPad)` o `pipeline.Connect(block1.Output, block2.Input)`. Para entradas dinámicas, use `CreateNewInput()` en el bloque sink.

## Configurando su Entorno de Pipeline

### Creando una Nueva Instancia de Pipeline

El primer paso para trabajar con Media Blocks es instanciar un objeto pipeline:

```csharp
using VisioForge.Core.MediaBlocks;

// Crear una instancia de pipeline estándar
var pipeline = new MediaBlocksPipeline();

// Opcionalmente, puede asignar un nombre a su pipeline para identificación más fácil
pipeline.Name = "MainVideoPlayer";
```

### Implementando Manejo de Errores Robusto

Las aplicaciones de medios deben manejar varios escenarios de error que pueden ocurrir durante la operación. Implementar manejo de errores apropiado asegura que su aplicación permanezca estable:

```csharp
// Suscribirse a eventos de error para capturar y manejar excepciones
pipeline.OnError += (sender, args) =>
{
    // Registrar el mensaje de error
    Debug.WriteLine($"Error de pipeline ocurrido: {args.Message}");
    
    // Implementar recuperación de error apropiada basada en el mensaje
    if (args.Message.Contains("Access denied"))
    {
        // Manejar problemas de permisos
    }
    else if (args.Message.Contains("File not found"))
    {
        // Manejar errores de archivo faltante
    }
};
```

## Gestionando Temporización y Navegación de Medios

### Recuperando Información de Duración y Posición

El control preciso de temporización es esencial para aplicaciones de medios:

```csharp
// Obtener la duración total del medio (devuelve TimeSpan.Zero para streams en vivo)
var duration = await pipeline.DurationAsync();
Console.WriteLine($"Duración del medio: {duration.TotalSeconds} segundos");

// Obtener la posición actual de reproducción
var position = await pipeline.Position_GetAsync();
Console.WriteLine($"Posición actual: {position.TotalSeconds} segundos");
```

### Implementando Funcionalidad de Búsqueda

Permita a sus usuarios navegar a través del contenido de medios con operaciones de búsqueda:

```csharp
// Búsqueda básica a una posición de tiempo específica
await pipeline.Position_SetAsync(TimeSpan.FromSeconds(10));

// Búsqueda con alineación de keyframe para navegación más eficiente
await pipeline.Position_SetAsync(TimeSpan.FromMinutes(2), seekToKeyframe: true);

// Búsqueda avanzada con posiciones de inicio y fin para reproducción parcial
await pipeline.Position_SetRangeAsync(
    TimeSpan.FromSeconds(30),  // Posición de inicio
    TimeSpan.FromSeconds(60)   // Posición de fin
);
```

## Controlando el Flujo de Ejecución del Pipeline

### Iniciando Reproducción de Medios

Controle la reproducción de medios con estos métodos esenciales:

```csharp
// Iniciar reproducción inmediatamente
await pipeline.StartAsync();

// Precargar medio sin iniciar reproducción (útil para reducir retraso de inicio)
await pipeline.StartAsync(onlyPreload: true);
await pipeline.ResumeAsync(); // Iniciar el pipeline precargado cuando esté listo
```

### Gestionando Estados de Reproducción

Monitoree y controle el estado actual de ejecución del pipeline:

```csharp
// Verificar el estado actual del pipeline
var state = pipeline.State;
if (state == PlaybackState.Play)
{
    Console.WriteLine("El pipeline está reproduciendo actualmente");
}

// Suscribirse a eventos importantes de cambio de estado
pipeline.OnStart += (sender, args) =>
{
    Console.WriteLine("La reproducción del pipeline ha iniciado");
    UpdateUIForPlaybackState();
};

pipeline.OnStop += (sender, args) =>
{
    Console.WriteLine("La reproducción del pipeline se ha detenido");
    Console.WriteLine($"Detenido en posición: {args.Position.TotalSeconds} segundos");
    ResetPlaybackControls();
};

pipeline.OnPause += (sender, args) =>
{
    Console.WriteLine("La reproducción del pipeline está pausada");
    UpdatePauseButtonState();
};

pipeline.OnResume += (sender, args) =>
{
    Console.WriteLine("La reproducción del pipeline ha reanudado");
    UpdatePlayButtonState();
};
```

### Operaciones de Pausa y Reanudación

Implemente funcionalidad de pausa y reanudación para mejor experiencia de usuario:

```csharp
// Pausar la reproducción actual
await pipeline.PauseAsync();

// Reanudar reproducción desde estado pausado
await pipeline.ResumeAsync();
```

### Deteniendo la Ejecución del Pipeline

Termine apropiadamente las operaciones del pipeline:

```csharp
// Operación de detención estándar
await pipeline.StopAsync();

// Forzar detención en escenarios sensibles al tiempo (puede afectar integridad del archivo de salida)
await pipeline.StopAsync(force: true);
```

## Construyendo Cadenas de Procesamiento de Medios

### Conectando Bloques de Procesamiento de Medios

El verdadero poder del Media Blocks SDK viene de conectar bloques especializados para crear cadenas de procesamiento:

```csharp
// Conexión básica entre dos bloques
pipeline.Connect(block1.Output, block2.Input);

// Conectar bloques con tipos de medios específicos
pipeline.Connect(videoSource.GetOutputPadByType(MediaBlockPadMediaType.Video), 
                 videoEncoder.GetInputPadByType(MediaBlockPadMediaType.Video));
```

Diferentes bloques pueden tener múltiples entradas y salidas especializadas:

- E/S Estándar: propiedades `Input` y `Output`
- E/S específica de medios: `VideoOutput`, `AudioOutput`, `VideoInput`, `AudioInput`
- Arrays de E/S: `Inputs[]` y `Outputs[]` para bloques complejos

### Trabajando con Bloques de Entrada Dinámica

Algunos bloques sink avanzados crean entradas dinámicamente bajo demanda:

```csharp
// Crear un muxer MP4 especializado para grabación
var mp4Muxer = new MP4SinkBlock();
mp4Muxer.FilePath = "output_recording.mp4";

// Solicitar una nueva entrada de video del muxer
var videoInput = mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Video);

// Conectar una fuente de video a la entrada recién creada
pipeline.Connect(videoSource.Output, videoInput);

// Similar para audio
var audioInput = mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Audio);
pipeline.Connect(audioSource.Output, audioInput);
```

Esta flexibilidad permite escenarios complejos de procesamiento de medios con múltiples flujos de entrada.

## Gestión Apropiada de Recursos

### Eliminando Recursos del Pipeline

Las aplicaciones de medios pueden consumir recursos significativos del sistema. Siempre elimine apropiadamente los objetos pipeline:

```csharp
// Patrón de eliminación síncrona
try
{
    // Usar pipeline
}
finally
{
    pipeline.Dispose();
}
```

Para aplicaciones modernas, use el patrón asíncrono para prevenir congelamiento de UI:

```csharp
// Eliminación asíncrona (preferida para aplicaciones UI)
try
{
    // Usar pipeline
}
finally
{
    await pipeline.DisposeAsync();
}
```

### Usando Declaraciones 'using' para Limpieza Automática

Aproveche las características del lenguaje C# para gestión automática de recursos:

```csharp
// Eliminación automática con declaración 'using'
using (var pipeline = new MediaBlocksPipeline())
{
    // Configurar y usar pipeline
    await pipeline.StartAsync();
    // El pipeline será automáticamente eliminado al salir de este bloque
}

// Declaración using de C# 8.0+
using var pipeline = new MediaBlocksPipeline();
// El pipeline será eliminado cuando el método contenedor termine
```

## Características Avanzadas del Pipeline

### Control de Velocidad de Reproducción

Ajuste la velocidad de reproducción para efectos de cámara lenta o avance rápido:

```csharp
// Obtener velocidad de reproducción actual
double currentRate = await pipeline.Rate_GetAsync();

// Establecer velocidad de reproducción (1.0 es velocidad normal)
await pipeline.Rate_SetAsync(0.5);  // Cámara lenta (mitad de velocidad)
await pipeline.Rate_SetAsync(2.0);  // Doble velocidad
```

### Configuración de Reproducción en Bucle

Implemente funcionalidad de reproducción continua:

```csharp
// Habilitar bucle para reproducción continua
pipeline.Loop = true;

// Escuchar eventos de bucle
pipeline.OnLoop += (sender, args) =>
{
    Console.WriteLine("El medio ha vuelto al inicio");
    UpdateLoopCounter();
};
```

### Modo Debug para Desarrollo

Habilite características de depuración durante el desarrollo:

```csharp
// Habilitar modo debug para logging más detallado
pipeline.Debug_Mode = true;
pipeline.Debug_Dir = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.MyDocuments), "PipelineDebugLogs");
```

## Referencia de Tipos de Bloques

El SDK proporciona una amplia gama de tipos de bloques para fuentes, procesamiento y sinks. Vea el enum `MediaBlockType` en el código fuente para una lista completa de tipos de bloques disponibles.

## Notas

- El pipeline soporta tanto métodos síncronos como asíncronos para iniciar, detener y eliminar. Prefiera métodos asíncronos en UI o aplicaciones de larga duración.
- Hay eventos disponibles para manejo de errores, cambios de estado e información de flujos.
- Use la interfaz correcta para cada tipo de bloque para acceder a características especializadas (ej., entradas dinámicas, renderizado, configuraciones).
