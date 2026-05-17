---
title: Ejemplos de código C# para edición de video con .NET SDK
description: Ejemplos prácticos de Video Edit SDK .Net — superposiciones de texto/imagen, efectos, transiciones y manipulación de audio para desarrolladores .NET.
sidebar_label: Ejemplos de código
tags:
  - Video Edit SDK
  - .NET

---

# Ejemplos de código y tutoriales de edición de video .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta página recopila recetas en C# listas para usar para los escenarios de edición más comunes con Video Edit SDK .Net. Cada fragmento está verificado contra el código fuente del SDK y las demos bajo `_SOURCE/_DEMOS/Video Edit SDK/`. Las recetas siguientes utilizan el motor `VideoEditCore` (solo Windows). El código multiplataforma basado en `VideoEditCoreX` sigue una forma similar con tipos de fuente y de efecto específicos del motor.

## Recetas disponibles

### Efectos visuales y superposiciones

- [**Añadir superposiciones de imagen al video**](add-image-overlay.md) — Aprende a superponer imágenes sobre tu contenido de video
- [**Implementación de superposición de texto**](add-text-overlay.md) — Técnicas para añadir y formatear superposiciones de texto sobre videos
- [**Efectos Picture-In-Picture**](picture-in-picture.md) — Crea efectos PiP profesionales en tus aplicaciones de video

### Manipulación de audio

- [**Efectos de envolvente de volumen de audio**](audio-envelope.md) — Controla cambios de volumen en el tiempo
- [**Múltiples flujos de audio en archivos AVI**](multiple-audio-streams-avi.md) — Trabajar con varias pistas de audio
- [**Control de volumen personalizado para pistas de audio**](volume-for-track.md) — Técnicas precisas de gestión de niveles de audio

### Composición de video

- [**Crear videos desde múltiples fuentes**](output-file-from-multiple-sources.md) — Combina varios archivos de entrada en una sola salida
- [**Trabajar con segmentos de video**](several-segments.md) — Extrae y usa varios segmentos del mismo archivo fuente
- [**Efectos de transición entre fragmentos de video**](transition-video.md) — Implementación de transiciones suaves entre clips
- [**Generar videos a partir de secuencias de imágenes**](video-images-console.md) — Ejemplo de aplicación de consola para conversión imagen-a-video
- [**Integración de video con múltiples flujos de audio**](adding-video-file-with-multiple-audio-streams.md) — Trabajar con combinaciones complejas de audio-video

## iOS

- [Editor de video iOS](ios-video-editor.md) — Creación de aplicaciones de edición de video para iPhone y iPad

## Receta — Unir varios archivos fuente en un solo MP4

`VideoSource` acepta un nombre de archivo más tiempos de inicio/fin opcionales. Para recortar una sección de una fuente, pasa los desplazamientos de inicio y fin; para usar el archivo completo, pasa `null`. Cada llamada a `Input_AddVideoFile` añade a la línea de tiempo.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

public async Task JoinClipsAsync(string output)
{
    var videoEdit = new VideoEditCore();

    // Contenedor de salida.
    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    // Primer clip — los primeros 10 segundos.
    var clip1 = new VideoSource(
        @"intro.mp4",
        TimeSpan.Zero,
        TimeSpan.FromSeconds(10),
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip1);

    // Segundo clip — archivo completo añadido a la línea de tiempo.
    var clip2 = new VideoSource(
        @"content.mp4",
        null,
        null,
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip2);

    // Tercer clip — archivo completo añadido.
    var clip3 = new VideoSource(
        @"outro.mp4",
        null,
        null,
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip3);

    // Lanzar el motor.
    await videoEdit.StartAsync();
}
```

## Receta — Añadir un logo de imagen en superposición

`VideoEffectImageLogo` es el efecto de superposición de imagen heredado. Créalo con un nombre de efecto único (el segundo argumento del constructor), asigna el archivo de imagen mediante `Filename` y añádelo al motor. La posición se controla con `Left`/`Top` (en píxeles) cuando no se usa alineación automática.

```csharp
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEffects;

public async Task AddLogoAsync(string source, string output)
{
    var videoEdit = new VideoEditCore();

    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    var video = new VideoSource(source, null, null, VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(video);

    // Superposición de logo de imagen (se recomienda PNG con alfa para logos transparentes).
    var logo = new VideoEffectImageLogo(enabled: true, name: "logo1")
    {
        Filename = "logo.png",
        Left = 10,
        Top = 10
    };
    videoEdit.Video_Effects_Add(logo);

    await videoEdit.StartAsync();
}
```

## Receta — Reemplazar el audio fuente por una pista musical

Añade el archivo de video como fuente solo-video (mediante las sobrecargas de `Input_AddVideoFile` con `targetStreamIndex` si es necesario), y luego añade un `AudioSource` aparte para la pista musical. El constructor `AudioSource` acepta un nombre de archivo más desplazamientos de inicio/fin opcionales.

```csharp
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

public async Task ReplaceAudioAsync(string source, string music, string output)
{
    var videoEdit = new VideoEditCore();

    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    // Video fuente (el motor ignora el audio original cuando se añade un AudioSource
    // separado; consulta las sobrecargas de Input_AddAudioFile para controlar la mezcla).
    var video = new VideoSource(source, null, null, VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(video);

    // Música de fondo — archivo completo.
    var music_src = new AudioSource(music);
    await videoEdit.Input_AddAudioFileAsync(music_src);

    await videoEdit.StartAsync();
}
```

## Recursos adicionales

Encuentra muestras y recursos más extensos en nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples), donde actualizamos regularmente nuestra colección con nuevos ejemplos y técnicas de implementación para desarrolladores .NET.
