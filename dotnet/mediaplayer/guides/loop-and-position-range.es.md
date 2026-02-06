---
title: Bucle y Rango de Posición - Media Player SDK
description: Implementa reproducción en bucle y control de segmentos en .NET. Aprende características de DirectShow y GStreamer para bucles de video.
keywords: bucle de video, bucle reproductor multimedia, reproducción rango posición, reproducción segmento, bucle DirectShow, bucle GStreamer, SDK reproductor video, reproducción continua, bucle video quiosco, bucle sin interrupciones
sidebar_label: Modo Bucle y Rango Posición
order: 2

---

# Modo de Bucle y Reproducción por Rango de Posición

Esta guía explica cómo usar el modo de bucle y las funciones de posición de inicio y fin personalizadas (rango de posición) en el SDK de Media Player para MediaPlayerCore (motor DirectShow) y MediaPlayerCoreX (motor GStreamer).

## Descripción General

Ambos motores de Media Player admiten:

- **Modo de Bucle**: Reiniciar automáticamente la reproducción cuando el medio llega al final
- **Rango de Posición**: Reproducir solo un segmento específico del medio entre posiciones de inicio y fin
- **Modo Combinado**: Reproducir un segmento específico del medio continuamente en bucle

Estas características son útiles para:

- Crear bucles de video para quioscos o pantallas
- Vista previa de segmentos específicos
- Prueba de porciones específicas de archivos multimedia
- Crear bucles de video de fondo sin interrupciones
- Aplicaciones educativas que muestran contenido repetido

## MediaPlayerCore (Motor DirectShow)

MediaPlayerCore es el motor de reproductor de medios basado en DirectShow exclusivo para Windows.

### Propiedades del Modo de Bucle

#### Propiedad `Loop`

Habilita o deshabilita la reproducción automática en bucle.

```csharp
// Habilitar modo de bucle
mediaPlayer.Loop = true;

// Deshabilitar modo de bucle
mediaPlayer.Loop = false;
```

**Valor predeterminado**: `false`

**Comportamiento**:
- Cuando está habilitado, la reproducción se reinicia automáticamente desde el principio al llegar al final
- El evento `OnLoop` se dispara cada vez que se reinicia la reproducción
- Para listas de reproducción, repite toda la lista, no archivos individuales

#### Propiedad `Loop_DoNotSeekToBeginning`

Controla si se debe buscar al principio cuando se reinicia el bucle.

```csharp
// Reiniciar sin buscar al principio (bucle sin interrupciones)
mediaPlayer.Loop_DoNotSeekToBeginning = true;

// Buscar al principio antes de reiniciar (predeterminado)
mediaPlayer.Loop_DoNotSeekToBeginning = false;
```

**Valor predeterminado**: `false`

**Comportamiento**:
- Solo afecta el comportamiento cuando `Loop` es `true`
- Cuando es `true`, reinicia desde la posición actual sin buscar
- Mejora el rendimiento y evita fallos visuales durante las transiciones de bucle
- Útil para bucles sin interrupciones de contenido

### Propiedades de Rango de Posición

#### Propiedad `Selection_Active`

Habilita o deshabilita la selección de rango de reproducción.

```csharp
// Habilitar reproducción por rango de posición
mediaPlayer.Selection_Active = true;

// Deshabilitar reproducción por rango (reproducir archivo completo)
mediaPlayer.Selection_Active = false;
```

**Valor predeterminado**: `false`

**Comportamiento**:
- Cuando está activo, la reproducción está limitada entre `Selection_Start` y `Selection_Stop`
- El reproductor se detiene o repite automáticamente al llegar a `Selection_Stop`
- Útil para reproducir segmentos específicos o crear vistas previas de clips

#### Propiedad `Selection_Start`

Establece la posición de inicio para la reproducción basada en rango.

```csharp
// Iniciar reproducción a los 30 segundos
mediaPlayer.Selection_Start = TimeSpan.FromSeconds(30);

// Iniciar reproducción a los 2 minutos 15 segundos
mediaPlayer.Selection_Start = new TimeSpan(0, 2, 15);
```

**Tipo**: `TimeSpan`

**Requisitos**:
- Solo se usa cuando `Selection_Active` es `true`
- Debe ser menor que `Selection_Stop`
- Debe estar dentro de la duración del medio
- El reproductor busca automáticamente esta posición al iniciar

#### Propiedad `Selection_Stop`

Establece la posición de fin para la reproducción basada en rango.

```csharp
// Detener reproducción a 1 minuto
mediaPlayer.Selection_Stop = TimeSpan.FromSeconds(60);

// Reproducir hasta el final del archivo
mediaPlayer.Selection_Stop = TimeSpan.Zero;
```

**Tipo**: `TimeSpan`

**Requisitos**:
- Solo se usa cuando `Selection_Active` es `true`
- Debe ser mayor que `Selection_Start`
- Use `TimeSpan.Zero` para reproducir hasta el final del archivo multimedia
- Cuando la reproducción alcanza esta posición, se detiene (o repite si `Loop` está habilitado)

### Eventos de MediaPlayerCore

#### Evento `OnLoop`

Se dispara cada vez que la reproducción se reinicia en modo de bucle.

```csharp
mediaPlayer.OnLoop += (sender, e) =>
{
    Console.WriteLine("¡Reproducción en bucle!");
    // Actualizar contador de bucles, realizar acciones en punto de bucle, etc.
};
```

**Cuándo se dispara**:
- Solo cuando la propiedad `Loop` es `true`
- Cada vez que la reproducción cicla del final al principio
- Después de buscar al principio (si corresponde)

**Casos de uso**:
- Rastrear iteraciones de bucle
- Actualizar contadores de bucle en la interfaz de usuario
- Realizar acciones en cada punto de bucle
- Registrar estadísticas de reproducción

### Ejemplos de Código para MediaPlayerCore

#### Ejemplo 1: Modo de Bucle Básico

```csharp
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;

// Crear instancia del reproductor de medios
var player = new MediaPlayerCore();

// Habilitar modo de bucle
player.Loop = true;

// Suscribirse al evento de bucle
player.OnLoop += (sender, e) =>
{
    Console.WriteLine($"Iteración de bucle en {DateTime.Now}");
};

// Establecer fuente y reproducir
player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\sample.mp4");
await player.PlayAsync();
```

#### Ejemplo 2: Bucle Sin Interrupciones Sin Búsqueda

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Habilitar bucle sin interrupciones (sin buscar al principio)
player.Loop = true;
player.Loop_DoNotSeekToBeginning = true;

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\background.mp4");
await player.PlayAsync();
```

#### Ejemplo 3: Reproducir Segmento Específico

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Habilitar rango de posición
player.Selection_Active = true;

// Reproducir desde 1 minuto a 2 minutos
player.Selection_Start = TimeSpan.FromMinutes(1);
player.Selection_Stop = TimeSpan.FromMinutes(2);

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\long-video.mp4");
await player.PlayAsync();
```

#### Ejemplo 4: Bucle de Segmento Específico

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Habilitar tanto bucle como rango de posición
player.Loop = true;
player.Selection_Active = true;

// Segmento de bucle de 30 a 45 segundos
player.Selection_Start = TimeSpan.FromSeconds(30);
player.Selection_Stop = TimeSpan.FromSeconds(45);

// Rastrear recuento de bucles
int loopCount = 0;
player.OnLoop += (sender, e) =>
{
    loopCount++;
    Console.WriteLine($"Bucle de segmento #{loopCount}");
};

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\video.mp4");
await player.PlayAsync();
```

#### Ejemplo 5: Actualización Dinámica de Rango de Posición

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

player.Selection_Active = true;
player.Selection_Start = TimeSpan.FromSeconds(10);
player.Selection_Stop = TimeSpan.FromSeconds(20);

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\video.mp4");
await player.PlayAsync();

// Más tarde, durante la reproducción, actualizar el rango
await Task.Delay(5000);

// Cambiar a un segmento diferente
player.Selection_Start = TimeSpan.FromSeconds(30);
player.Selection_Stop = TimeSpan.FromSeconds(40);

// Buscar nueva posición de inicio
player.Position_Set_Time(player.Selection_Start);
```

## MediaPlayerCoreX (Motor GStreamer)

MediaPlayerCoreX es el motor de reproductor de medios basado en GStreamer multiplataforma, compatible con Windows, Linux, macOS, Android e iOS.

### Propiedad de Modo de Bucle

#### Propiedad `Loop`

Habilita o deshabilita la reproducción automática en bucle.

```csharp
// Habilitar modo de bucle
mediaPlayer.Loop = true;

// Deshabilitar modo de bucle
mediaPlayer.Loop = false;
```

**Valor predeterminado**: `false`

**Comportamiento**:
- Cuando está habilitado, la reproducción se reinicia automáticamente cuando se alcanza el fin de flujo (EOS)
- El `MediaBlocksPipeline` subyacente maneja la lógica del bucle
- El evento `OnLoop` se dispara cada vez que se reinicia la reproducción
- Reinicia la reproducción sin interrupciones sin sobrecarga de búsqueda

### Propiedades de Rango de Posición

#### Propiedad `Segment_Start`

Establece la posición de inicio para la reproducción de segmento.

```csharp
// Iniciar reproducción a los 45 segundos
mediaPlayer.Segment_Start = TimeSpan.FromSeconds(45);

// Iniciar reproducción a los 3 minutos
mediaPlayer.Segment_Start = TimeSpan.FromMinutes(3);
```

**Tipo**: `TimeSpan`

**Valor predeterminado**: `TimeSpan.Zero`

**Comportamiento**:
- Define dónde debe comenzar la reproducción
- El reproductor busca automáticamente esta posición al iniciar
- Se usa en combinación con `Segment_Stop` para definir el rango de reproducción
- Se aplica a través de la propiedad subyacente `MediaBlocksPipeline.StartPosition`

#### Propiedad `Segment_Stop`

Establece la posición de fin para la reproducción de segmento.

```csharp
// Detener reproducción a los 2 minutos
mediaPlayer.Segment_Stop = TimeSpan.FromMinutes(2);

// Reproducir hasta el final (sin posición de detención)
mediaPlayer.Segment_Stop = TimeSpan.Zero;
```

**Tipo**: `TimeSpan`

**Valor predeterminado**: `TimeSpan.Zero`

**Comportamiento**:
- Define dónde debe terminar la reproducción
- Cuando la reproducción alcanza esta posición, se activa el fin de flujo (EOS)
- Si `Loop` está habilitado, la reproducción se reinicia desde `Segment_Start`
- Use `TimeSpan.Zero` para ninguna posición de detención (reproducir hasta el final)
- Se aplica a través de la propiedad subyacente `MediaBlocksPipeline.StopPosition`

### Eventos de MediaPlayerCoreX

#### Evento `OnLoop`

Se dispara cada vez que la reproducción se reinicia en modo de bucle.

```csharp
mediaPlayer.OnLoop += (sender, e) =>
{
    Console.WriteLine("¡Reproducción en bucle!");
};
```

**Cuándo se dispara**:
- Solo cuando la propiedad `Loop` es `true`
- Cuando se alcanza el fin de flujo (EOS)
- Antes de que se reinicie la reproducción

### Ejemplos de Código para MediaPlayerCoreX

#### Ejemplo 1: Modo de Bucle Básico

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

// Crear instancia del reproductor de medios
var player = new MediaPlayerCoreX();

// Habilitar modo de bucle
player.Loop = true;

// Suscribirse al evento de bucle
player.OnLoop += (sender, e) =>
{
    Console.WriteLine($"Iteración de bucle en {DateTime.Now}");
};

// Establecer fuente y reproducir
var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\sample.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Ejemplo 2: Reproducir Segmento Específico

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Establecer rango de segmento: reproducir de 30s a 90s
player.Segment_Start = TimeSpan.FromSeconds(30);
player.Segment_Stop = TimeSpan.FromSeconds(90);

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\long-video.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Ejemplo 3: Bucle de Segmento Específico

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Habilitar bucle y establecer segmento
player.Loop = true;
player.Segment_Start = TimeSpan.FromMinutes(1);
player.Segment_Stop = TimeSpan.FromMinutes(2);

// Rastrear recuento de bucles
int loopCount = 0;
player.OnLoop += (sender, e) =>
{
    loopCount++;
    Console.WriteLine($"Bucle de segmento #{loopCount}");
};

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\video.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Ejemplo 4: Video en Bucle Multiplataforma

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Habilitar bucle sin interrupciones
player.Loop = true;

// Para ruta de archivo de video multiplataforma
string videoPath;
#if ANDROID
    videoPath = "/storage/emulated/0/Movies/background.mp4";
#elif IOS
    videoPath = Path.Combine(NSBundle.MainBundle.BundlePath, "background.mp4");
#else
    videoPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.MyVideos), "background.mp4");
#endif

var source = new UniversalSourceSettings()
{
    URI = new Uri(videoPath)
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Ejemplo 5: Vista Previa de Segmento con Actualización de Interfaz de Usuario

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Modo vista previa: mostrar clips de 10 segundos
TimeSpan clipDuration = TimeSpan.FromSeconds(10);

async Task PreviewSegment(TimeSpan startTime)
{
    player.Segment_Start = startTime;
    player.Segment_Stop = startTime + clipDuration;
    
    // Buscar posición de inicio
    await player.Position_SetAsync(startTime);
    
    Console.WriteLine($"Vista previa de segmento: {startTime} a {startTime + clipDuration}");
}

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\movie.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();

// Vista previa del primer segmento
await PreviewSegment(TimeSpan.FromSeconds(0));

// Vista previa del segmento que comienza a los 30 segundos
await Task.Delay(11000);
await PreviewSegment(TimeSpan.FromSeconds(30));
```

#### Ejemplo 6: Bucle con Pausa al Detener

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Modo de bucle con pausa en lugar de detención al final
player.Loop = true;
player.PauseOnStop = true; // Pausar en lugar de detener en EOS

player.OnLoop += (sender, e) =>
{
    Console.WriteLine("Fin alcanzado, pausando brevemente antes del bucle...");
    
    // Esperar antes de reanudar (si es necesario)
    Task.Delay(1000).ContinueWith(_ => 
    {
        // La reproducción se reiniciará automáticamente debido a Loop = true
    });
};

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\video.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

## Mejores Prácticas

### Consideraciones de Rendimiento

1. **Bucle Sin Interrupciones (MediaPlayerCore)**:
   - Use `Loop_DoNotSeekToBeginning = true` para bucles sin interrupciones sin fallos visuales
   - Pruebe con su formato de medio específico para obtener mejores resultados

2. **Segmentos Cortos**:
   - Para segmentos muy cortos (< 1 segundo), asegúrese de que el medio esté correctamente indexado
   - Algunos formatos pueden no admitir búsqueda con precisión de fotograma

3. **Multiplataforma (MediaPlayerCoreX)**:
   - Pruebe en todas las plataformas de destino ya que el comportamiento de GStreamer puede variar ligeramente
   - Use códecs de video apropiados que admitan búsqueda (H.264, H.265)

### Casos de Uso Comunes

#### Bucle de Video para Quiosco
```csharp
// MediaPlayerCore (Windows)
player.Loop = true;
player.Loop_DoNotSeekToBeginning = true;

// MediaPlayerCoreX (Multiplataforma)
player.Loop = true;
```

#### Ventana de Vista Previa
```csharp
// MediaPlayerCore
player.Selection_Active = true;
player.Selection_Start = previewStart;
player.Selection_Stop = previewEnd;

// MediaPlayerCoreX
player.Segment_Start = previewStart;
player.Segment_Stop = previewEnd;
```

#### Bucle Continuo de Segmento
```csharp
// MediaPlayerCore
player.Loop = true;
player.Selection_Active = true;
player.Selection_Start = segmentStart;
player.Selection_Stop = segmentEnd;

// MediaPlayerCoreX
player.Loop = true;
player.Segment_Start = segmentStart;
player.Segment_Stop = segmentEnd;
```

### Solución de Problemas

#### El Bucle No Funciona

**MediaPlayerCore**:
- Asegúrese de que la propiedad `Loop` esté configurada antes de llamar a `PlayAsync()`
- Verifique que el evento `OnStop` no esté llamando al método `Stop()`
- Verifique que el archivo multimedia no esté dañado

**MediaPlayerCoreX**:
- Asegúrese de que la propiedad `Loop` esté configurada antes de llamar a `PlayAsync()`
- Verifique que GStreamer esté inicializado correctamente
- Verifique que el pipeline no se esté eliminando prematuramente

#### El Rango de Posición No Es Preciso

**MediaPlayerCore**:
- Asegúrese de que `Selection_Active` esté configurado en `true`
- Verifique que `Selection_Start` < `Selection_Stop`
- Algunos formatos multimedia pueden no admitir búsqueda con precisión de fotograma

**MediaPlayerCoreX**:
- Verifique que `Segment_Start` y `Segment_Stop` sean válidos
- Use formatos multimedia que admitan búsqueda (MP4, MKV con indexación adecuada)
- Verifique que el archivo multimedia admita búsqueda (no todas las fuentes de transmisión lo hacen)

#### La Reproducción de Segmento Comienza en la Posición Incorrecta

**Ambos Motores**:
- Asegúrese de que las posiciones estén configuradas antes de llamar a `PlayAsync()`
- Espere a que el medio esté completamente cargado antes de establecer las posiciones
- Use búsqueda basada en fotogramas clave para mejor precisión

## Tabla de Comparación de Propiedades

| Característica | MediaPlayerCore | MediaPlayerCoreX |
|----------------|-----------------|------------------|
| **Modo de Bucle** | `Loop` (bool) | `Loop` (bool) |
| **Bucle Sin Interrupciones** | `Loop_DoNotSeekToBeginning` (bool) | Integrado (sin propiedad adicional) |
| **Rango Activo** | `Selection_Active` (bool) | Implícito (cuando se establecen propiedades de Segment) |
| **Posición de Inicio** | `Selection_Start` (TimeSpan) | `Segment_Start` (TimeSpan) |
| **Posición de Fin** | `Selection_Stop` (TimeSpan) | `Segment_Stop` (TimeSpan) |
| **Evento de Bucle** | `OnLoop` | `OnLoop` |
| **Plataforma** | Solo Windows | Multiplataforma |
| **Motor** | DirectShow | GStreamer |

## Documentación Relacionada

- [Referencia de API de MediaPlayerCore](https://api.visioforge.org/dotnet/api/VisioForge.Core.MediaPlayerX.MediaPlayerCoreX.html)
- [Referencia de API de MediaPlayerCoreX](https://api.visioforge.org/dotnet/api/VisioForge.Core.MediaPlayerX.MediaPlayerCoreX.html)

## Ver También

- [Descripción General del SDK de Media Player](../index.md)
- [Ejemplos de Código](../code-samples/index.md)
- [Guías Adicionales](index.md)
