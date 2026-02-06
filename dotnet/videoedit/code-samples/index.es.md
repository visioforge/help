---
title: Ejemplos de Código de Video Edit SDK
description: Ejemplos prácticos de Video Edit SDK .Net: superposiciones de texto/imagen, efectos de video, transiciones y manipulación avanzada.
---

# Ejemplos de Código - Video Edit SDK

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección contiene ejemplos de código prácticos para diferentes escenarios de edición de video.

## Ejemplos Disponibles

### Superposiciones

- [Añadir superposición de imagen](add-image-overlay.md) - Añadir un logo o imagen sobre el video
- [Añadir superposición de texto](add-text-overlay.md) - Añadir texto al video

### Manipulación de Audio

- [Envolvente de audio](audio-envelope.md) - Controlar volumen con curvas
- [Volumen por pista](volume-for-track.md) - Ajustar volumen de pistas individuales
- [Múltiples pistas de audio en AVI](multiple-audio-streams-avi.md) - Crear archivos AVI con múltiples pistas

### Efectos y Transiciones

- [Transición entre videos](transition-video.md) - Añadir efectos de transición
- [Picture in Picture](picture-in-picture.md) - Mostrar video dentro de otro
- [Varios segmentos](several-segments.md) - Combinar múltiples segmentos

### Manipulación de Archivos

- [Video con múltiples pistas de audio](adding-video-file-with-multiple-audio-streams.md)
- [Crear archivo de múltiples fuentes](output-file-from-multiple-sources.md)
- [Extraer imágenes de video](video-images-console.md)

### Plataformas Específicas

- [Editor de video iOS](ios-video-editor.md) - Edición de video en dispositivos iOS

## Ejemplo Rápido: Combinar Videos

```csharp
var videoEdit = new VideoEditCore();

// Configurar salida MP4
videoEdit.Output_Filename = "resultado.mp4";
videoEdit.Output_Format = new MP4Output(new H264EncoderSettings(), new AACEncoderSettings());

// Añadir primer video (0-10 segundos)
var video1 = new VideoFileSource("intro.mp4");
video1.StartTime = TimeSpan.Zero;
video1.StopTime = TimeSpan.FromSeconds(10);
videoEdit.Input_AddVideoFile(video1);

// Añadir segundo video (10-30 segundos)
var video2 = new VideoFileSource("contenido.mp4");
video2.StartTime = TimeSpan.FromSeconds(10);
video2.StopTime = TimeSpan.FromSeconds(30);
videoEdit.Input_AddVideoFile(video2);

// Añadir tercero video (30-40 segundos)
var video3 = new VideoFileSource("outro.mp4");
video3.StartTime = TimeSpan.FromSeconds(30);
video3.StopTime = TimeSpan.FromSeconds(40);
videoEdit.Input_AddVideoFile(video3);

// Renderizar
await videoEdit.StartAsync();
```

## Ejemplo: Añadir Logo

```csharp
var videoEdit = new VideoEditCore();

videoEdit.Output_Filename = "video_con_logo.mp4";
videoEdit.Output_Format = new MP4Output();

// Añadir video base
var video = new VideoFileSource("mi_video.mp4");
videoEdit.Input_AddVideoFile(video);

// Añadir superposición de imagen (logo)
var logo = new ImageOverlay("logo.png")
{
    X = 10,
    Y = 10,
    Width = 100,
    Height = 100,
    Opacity = 0.8
};
videoEdit.Video_Effects_Add(logo);

await videoEdit.StartAsync();
```

## Ejemplo: Añadir Música de Fondo

```csharp
var videoEdit = new VideoEditCore();

videoEdit.Output_Filename = "video_con_musica.mp4";
videoEdit.Output_Format = new MP4Output();

// Añadir video (sin audio original)
var video = new VideoFileSource("mi_video.mp4");
video.Audio_StreamIndex = -1; // Ignorar audio original
videoEdit.Input_AddVideoFile(video);

// Añadir música de fondo
var musica = new AudioFileSource("musica.mp3");
musica.StartTime = TimeSpan.Zero;
musica.Volume = 0.5; // 50% de volumen
videoEdit.Input_AddAudioFile(musica);

await videoEdit.StartAsync();
```
