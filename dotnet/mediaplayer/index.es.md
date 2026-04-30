---
title: Media Player SDK .NET - Reproductor de Video en C#
description: SDK de reproductor de video para C# y .NET — reproduce MP4, AVI, MKV, transmite RTSP/HLS. API multiplataforma para Windows, macOS, Linux, Android, iOS.
sidebar_label: Media Player SDK .NET
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Streaming
primary_api_classes:
  - UniversalSourceSettings
  - MediaPlayerCoreX
  - VideoView

---

# Media Player SDK para C# .NET — Reproductor de Video, Reproductor de Audio y API de Streaming

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El Media Player SDK para .NET es una API de reproductor de video en C# que te permite reproducir archivos de video y audio, streams de red (RTSP, HLS, MPEG-DASH) y contenido especial como videos de 360 grados en tus aplicaciones .NET. Reemplaza el código de reproducción DirectShow de bajo nivel y la integración con Windows Media Player SDK con una API async moderna — abre un archivo, inicia la reproducción y controla la búsqueda y el volumen en pocas líneas de C#.

El SDK usa decodificación basada en GStreamer con aceleración por hardware y se ejecuta en Windows, macOS, Linux, Android e iOS. Ya sea que necesites construir un reproductor de video de escritorio, integrar reproducción de medios en una app de kiosko o transmitir feeds RTSP desde cámaras IP, la API lo cubre.

## Inicio Rápido

### 1. Instalar Paquetes NuGet

```bash
dotnet add package VisioForge.DotNet.MediaPlayer
```

Agrega dependencias nativas específicas de plataforma:

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

Para la lista completa de paquetes y soporte de frameworks de UI (WinForms, WPF, MAUI, Avalonia), consulta la [Guía de Instalación](../install/index.md).

### 2. Inicializar el SDK

Llama a `InitSDKAsync()` una vez al iniciar la aplicación antes de usar cualquier funcionalidad de reproducción:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. Reproducir Video en C# (Ejemplo Mínimo)

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

// Crear instancia del reproductor vinculada a un control VideoView
var player = new MediaPlayerCoreX(videoView);

// Abrir un archivo de video
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("C:\\Videos\\sample.mp4"));
await player.OpenAsync(source);

// Iniciar reproducción
await player.PlayAsync();

// ... cuando termine:
await player.StopAsync();
await player.DisposeAsync();
```

### 4. Limpieza al Cerrar

```csharp
VisioForgeX.DestroySDK();
```

## Flujo de Trabajo Principal

Toda aplicación de reproductor de medios sigue el mismo patrón:

1. **Inicializar SDK** — `VisioForgeX.InitSDKAsync()` (una vez por vida de la aplicación)
2. **Crear reproductor** — `new MediaPlayerCoreX(videoView)` para video, o sin vista para solo audio
3. **Abrir medio** — `await player.OpenAsync(source)` con una ruta de archivo o URL de stream
4. **Reproducir** — `await player.PlayAsync()`
5. **Controlar reproducción** — buscar con `Position_SetAsync()`, pausar con `PauseAsync()`, ajustar volumen
6. **Detener** — `await player.StopAsync()`
7. **Liberar** — `await player.DisposeAsync()` libera todos los recursos
8. **Destruir SDK** — `VisioForgeX.DestroySDK()` al cerrar la aplicación

## Escenarios Comunes de Reproductor de Video en C#

### Reproducir Archivo de Video en C# (MP4, AVI, MKV)

Abre y reproduce cualquier archivo de video soportado con detección automática de codecs:

```csharp
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("movie.mp4"));
await player.OpenAsync(source);
await player.PlayAsync();
```

Ver tutorial completo: [Construir un Reproductor de Video en C#](guides/video-player-csharp.md)

### Reproducir Stream RTSP en C#

Reproduce video en vivo desde cámaras IP y fuentes RTSP. Soporta RTSP, HTTP, HLS y MPEG-DASH:

```csharp
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("rtsp://admin:password@192.168.1.100:554/stream"));
await player.OpenAsync(source);
await player.PlayAsync();
```

### Reproducción Solo de Audio

Reproduce archivos de audio (MP3, AAC, FLAC, WAV) sin vista de video:

```csharp
var player = new MediaPlayerCoreX(); // no se necesita VideoView
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("music.mp3"));
await player.OpenAsync(source);
await player.PlayAsync();
```

### Extraer Fotograma del Video

Captura una imagen fija de la posición de reproducción actual:

```csharp
// Guarda el frame actual directamente a disco — el camino más simple.
bool saved = await player.Snapshot_SaveAsync("screenshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, quality: 85);

// O toma el VideoFrameX crudo para procesamiento en memoria (el caller posee el buffer de píxeles).
var frame = await player.Snapshot_GetAsync();
if (frame != null)
{
    // frame.Data (IntPtr), frame.DataSize, frame.Width, frame.Height, frame.Stride, frame.Format.
    // Libera el buffer no manejado cuando termines:
    frame.Free();
}
```

Ver: [Obtener un Fotograma Específico de un Archivo de Video](code-samples/get-frame-from-video-file.md)

### Control de Búsqueda, Volumen y Velocidad

```csharp
// Buscar una posición específica
await player.Position_SetAsync(TimeSpan.FromSeconds(30));

// Obtener posición actual y duración
var position = await player.Position_GetAsync();
var duration = await player.DurationAsync();

// Ajustar volumen (0.0 a 1.0)
player.Audio_OutputDevice_Volume = 0.8;

// Cambiar velocidad de reproducción (0.25x a 4.0x)
await player.Rate_SetAsync(1.5);
```

### Reproducción en Bucle y Reproducción de Segmentos

Reproduce un segmento específico de un archivo o repite continuamente:

```csharp
// Habilitar modo bucle
player.Loop = true;

// Reproducir un rango de tiempo específico
player.Segment_Start = TimeSpan.FromSeconds(10);
player.Segment_Stop = TimeSpan.FromSeconds(60);
```

Ver: [Modo Bucle y Reproducción por Rango de Posición](guides/loop-and-position-range.md)

## Formatos Soportados

| Categoría | Formatos |
| --------- | -------- |
| Contenedores de Video | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Formatos de Audio | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Codecs de Video | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Protocolos de Streaming | RTSP, HTTP, HLS, MPEG-DASH |

## Soporte de Plataformas

| Plataforma | Frameworks de UI | Notas |
| ---------- | ---------------- | ----- |
| Windows x64 | WinForms, WPF, WinUI, MAUI, Avalonia, Consola | Conjunto completo de características incluyendo motor DirectShow |
| macOS | MAUI, Avalonia, Consola | Renderizado basado en AVFoundation |
| Linux x64 | Avalonia, Consola | Decodificación basada en GStreamer |
| Android | MAUI | Via integración MAUI |
| iOS | MAUI | Via integración MAUI |

Para implementaciones multiplataforma, consulte la [Guía del reproductor Avalonia](guides/avalonia-player.md) (escritorio, incluye Linux) o la [Guía del reproductor .NET MAUI](guides/maui-player.md) (móvil + escritorio).

## Documentación para Desarrolladores

### Guías

* [Construir un Reproductor de Video en C# (WinForms / WPF)](guides/video-player-csharp.md)
* [Construir un Reproductor de Video en VB.NET](guides/video-player-vb-net.md)
* [Implementación de Reproductor Avalonia](guides/avalonia-player.md)
* [Reproductor .NET MAUI](guides/maui-player.md)
* [Implementación de Reproductor Android](guides/android-player.md)
* [Modo Bucle y Rango de Posición](guides/loop-and-position-range.md)
* [Todas las Guías](guides/index.md)

### Ejemplos de Código

* [Obtener un Fotograma de un Archivo de Video](code-samples/get-frame-from-video-file.md)
* [Reproducir un Fragmento de un Archivo](code-samples/play-fragment-file.md)
* [Mostrar el Primer Fotograma](code-samples/show-first-frame.md)
* [Reproducción desde Memoria](code-samples/memory-playback.md)
* [API de Lista de Reproducción](code-samples/playlist-api.md)
* [Reproducción de Video en Reversa](code-samples/reverse-video-playback.md)
* [Todos los Ejemplos de Código](code-samples/index.md)

### Despliegue

* [Guía de Despliegue](deployment.md)

## Recursos para Desarrolladores

* [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [Referencia de API](https://api.visioforge.org/dotnet/api/index.html)
* [Registro de Cambios](../changelog.md)
* [Contrato de Licencia de Usuario Final](../../eula.md)
* [Información de Licenciamiento](../../../licensing.md)
