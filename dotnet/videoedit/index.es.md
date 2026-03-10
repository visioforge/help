---
title: Edición de Video - Línea de Tiempo y Transiciones en C# .NET
description: Edición de video en C# con Video Edit SDK — línea de tiempo, transiciones, superposiciones, conversión de formato y codificación acelerada por hardware.
sidebar_label: Video Edit SDK .NET

---

# Video Edit SDK para C# .NET — API de Edición de Video por Línea de Tiempo

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Video Edit SDK para .NET es una biblioteca de edición de video en C# que te permite construir aplicaciones de edición de video basadas en línea de tiempo. Agrega archivos de video y audio a una línea de tiempo, recorta segmentos, aplica transiciones y efectos, superpone texto e imágenes, y renderiza el resultado a MP4, AVI, MKV, WebM u otros formatos — todo desde tu código .NET.

El SDK proporciona dos motores: **VideoEditCore** (solo Windows, basado en DirectShow) y **VideoEditCoreX** (multiplataforma, se ejecuta en Windows, macOS, Linux, Android e iOS). Ambos motores comparten el mismo modelo de línea de tiempo — agrega fuentes, configura la salida e inicia la edición.

## Inicio Rápido

### 1. Instalar Paquete NuGet

```bash
dotnet add package VisioForge.DotNet.VideoEditX
```

Para dependencias específicas de plataforma y configuración de frameworks de UI, consulta la [Guía de Instalación](../install/index.md).

### 2. Ejemplo Mínimo de Edición de Video

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// Inicializar SDK
VisioForgeX.InitSDK();

// Crear editor con vista previa de video
var editor = new VideoEditCoreX(videoView);

// Establecer resolución de salida y velocidad de cuadros
editor.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
editor.Output_VideoFrameRate = new VideoFrameRate(30);

// Agregar archivos de video a la línea de tiempo
editor.Input_AddAudioVideoFile("intro.mp4", null, null, null);
editor.Input_AddAudioVideoFile("main.mp4", null, null, null);

// Establecer formato de salida
editor.Output_Format = new MP4Output("output.mp4");

// Iniciar edición
editor.Start();

// ... cuando termine:
editor.Stop();
editor.Dispose();
VisioForgeX.DestroySDK();
```

Para la guía de implementación completa con manejo de eventos, fuentes de audio, fuentes de imagen y configuración avanzada de línea de tiempo, consulta la [Guía de Inicio Rápido](getting-started.md).

## Casos de Uso Comunes

### Combinar Múltiples Archivos de Video

Fusiona múltiples clips de video en un solo archivo de salida. Agrega archivos a la línea de tiempo en secuencia, establece el formato de salida y renderiza. Soporta mezclar diferentes formatos de origen — combina archivos MP4, AVI y MOV en una sola salida.

Ver: [Crear Videos desde Múltiples Fuentes](code-samples/output-file-from-multiple-sources.md)

### Recortar y Cortar Segmentos de Video

Extrae rangos de tiempo específicos de archivos de video estableciendo tiempos de inicio y fin en cada fuente. Combina múltiples segmentos del mismo archivo o diferentes archivos en una edición final.

Ver: [Trabajar con Segmentos de Video](code-samples/several-segments.md)

### Agregar Superposiciones de Texto e Imagen

Inserta títulos de texto, subtítulos, logos y marcas de agua sobre el contenido de video. Posiciona, escala y temporiza las superposiciones en la línea de tiempo.

Ver: [Implementación de Superposición de Texto](code-samples/add-text-overlay.md) | [Agregar Superposiciones de Imagen](code-samples/add-image-overlay.md)

### Aplicar Transiciones entre Clips

Agrega disoluciones cruzadas, barridos, deslizamientos, fundidos y más de 100 transiciones estándar SMPTE entre segmentos de video. Controla la duración de la transición, estilo de borde y dirección.

Ver: [Efectos de Transición entre Fragmentos de Video](code-samples/transition-video.md) | [Referencia de Transiciones](transitions.md)

### Crear Presentación desde Imágenes

Construye presentaciones de video desde imágenes JPG, PNG, BMP y GIF con duración de visualización configurable por imagen, transiciones entre diapositivas y música de fondo.

Ver: [Generar Videos desde Secuencias de Imágenes](code-samples/video-images-console.md)

### Agregar Música de Fondo y Mezcla de Audio

Mezcla múltiples pistas de audio con contenido de video. Controla el volumen por pista, aplica efectos de envolvente de audio para fade-in/fade-out, y sincroniza audio con video.

Ver: [Efectos de Envolvente de Volumen de Audio](code-samples/audio-envelope.md) | [Control de Volumen Personalizado](code-samples/volume-for-track.md)

### Composición Picture-in-Picture

Superpone múltiples fuentes de video con control de posición y tamaño para layouts picture-in-picture, videos de reacción o composiciones multi-cámara.

Ver: [Efectos Picture-In-Picture](code-samples/picture-in-picture.md)

## Formatos Soportados

| Categoría | Formatos |
| --------- | -------- |
| Contenedores de Video | MP4, AVI, MOV, WMV, MKV, WebM, TS, FLV |
| Codecs de Video | H.264, H.265/HEVC, VP9, AV1, MPEG-2 |
| Formatos de Audio | AAC, MP3, WMA, OPUS, Vorbis, FLAC, WAV |
| Formatos de Imagen | JPG, PNG, BMP, GIF |

## Soporte de Plataformas

| Plataforma | Frameworks de UI | Motor | Notas |
| ---------- | ---------------- | ----- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Consola | VideoEditCore, VideoEditCoreX | Conjunto completo de características incluyendo puentes DirectShow |
| macOS | MAUI, Avalonia, Consola | VideoEditCoreX | Intel y Apple Silicon |
| Linux x64 | Avalonia, Consola | VideoEditCoreX | Ubuntu, Debian, CentOS |
| Android | MAUI | VideoEditCoreX | Via integración MAUI |
| iOS | MAUI | VideoEditCoreX | Via integración MAUI |

## Documentación para Desarrolladores

### Guías

* [Guía de Inicio Rápido](getting-started.md) — Tutorial completo de implementación con ambos motores
* [Ejemplos de Código](code-samples/index.md) — Ejemplos listos para usar de superposiciones, transiciones, audio y composición
* [Guía de Despliegue](deployment.md) — Paquetes NuGet, instaladores e instalación manual
* [Referencia de Transiciones](transitions.md) — Más de 100 códigos de transición SMPTE y propiedades

### iOS

* [Editor de Video iOS](code-samples/ios-video-editor.md) — Construir aplicaciones de edición de video para iPhone e iPad

## Recursos para Desarrolladores

* [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples)
* [Referencia de API](https://api.visioforge.org/dotnet/api/index.html)
* [Registro de Cambios](../changelog.md)
* [Contrato de Licencia de Usuario Final](../../eula.md)
* [Información de Licenciamiento](../../../licensing.md)
