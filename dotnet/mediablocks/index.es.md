---
title: Pipeline Multimedia Modular - Transcodificación y Captura C#
description: Pipelines multimedia en C# con VisioForge Media Blocks SDK. Conecte bloques de fuente, procesamiento y salida para transcodificación, captura y streaming.
sidebar_label: Media Blocks SDK .NET
tags:
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Streaming

---

# Media Blocks SDK para C# .NET — API de Pipeline Multimedia Modular

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Media Blocks SDK para .NET es un framework multimedia basado en pipelines que te permite construir flujos de trabajo personalizados de procesamiento de video y audio en C#. En lugar de usar una API fija con comportamiento predefinido, creas un pipeline conectando bloques tipados — fuentes, codificadores, efectos, renderizadores y sinks — para construir exactamente la cadena de procesamiento que tu aplicación necesita.

El SDK se ejecuta en Windows, macOS, Linux, Android e iOS. Cubre casos de uso que los SDKs de nivel superior Video Capture SDK y Media Player SDK no pueden cubrir: composición multi-fuente, pipelines de transcodificación personalizados, codificación simultánea a múltiples formatos, cadenas de efectos de video en tiempo real e integración con hardware como Blackmagic Decklink o cámaras industriales.

## Cuándo Usar Media Blocks

Media Blocks SDK es la elección correcta cuando necesitas control total sobre el pipeline multimedia. Úsalo en lugar de (o junto con) los otros SDKs de VisioForge cuando:

| Escenario | SDK Recomendado |
| --------- | --------------- |
| Grabación simple de webcam a MP4 | [Video Capture SDK](../videocapture/index.md) |
| Reproducir un archivo de video con controles estándar | [Media Player SDK](../mediaplayer/index.md) |
| Pipeline personalizado: fuente → efectos → codificar → múltiples salidas | **Media Blocks SDK** |
| Composición de video en vivo desde múltiples fuentes | **Media Blocks SDK** |
| Transcodificación / conversión de formato con procesamiento personalizado | **Media Blocks SDK** |
| Grabación RTSP con post-procesamiento (overlay, redimensionar, recodificar) | **Media Blocks SDK** |
| App de medios multiplataforma Avalonia o MAUI | **Media Blocks SDK** |
| Integración con Decklink, GenICam o hardware NVIDIA | **Media Blocks SDK** |

## Casos de Uso Comunes

### Transcodificación y Conversión de Formato de Video

Convierte archivos de video entre formatos (por ejemplo, AVI a MP4, MKV a WebM) con control total sobre codecs, resolución, bitrate y procesamiento. Encadena bloques de redimensionado, desentrelazado o corrección de color entre la fuente y el codificador.

### Captura de Cámara Personalizada con Pipeline de Procesamiento

Construye flujos de trabajo de captura de cámara que van más allá de la grabación simple. Inserta efectos en tiempo real, superposiciones de texto o bloques de visión por computadora entre la fuente de cámara y el sink de archivo. Envía a múltiples destinos simultáneamente — ventana de previsualización, archivo y stream de red.

Ver: [Tutorial de Aplicación de Visor de Cámara](GettingStarted/camera.md)

### Composición y Mezcla de Video en Vivo

Combina múltiples fuentes de video en una sola salida con el [Compositor de Video en Vivo](LiveVideoCompositor/index.md). Posiciona, escala y superpone feeds de video para producción multi-cámara, picture-in-picture o layouts de cuadrícula de vigilancia.

### Grabación de Stream RTSP con Post-Procesamiento

Captura streams RTSP de cámaras IP y aplica procesamiento antes de guardar — redimensiona a menor resolución, agrega superposiciones de marca de tiempo, recodifica con diferentes configuraciones de calidad o divide en segmentos.

Ver: [Guía de Guardado de Stream RTSP](Guides/rtsp-save-original-stream.md) | [Captura ONVIF con Post-Procesamiento](Guides/onvif-capture-with-postprocessing.md)

### Superposición de Texto e Imagen / Marca de Agua

Agrega superposiciones de texto, imágenes o SVG a video en vivo o archivos grabados usando el [Bloque de Gestión de Overlay](VideoProcessing/OverlayManagerBlock.md). Útil para marcas de agua, inserción de marca de tiempo, branding y visualización en pantalla.

### Lectura de Códigos de Barras y QR desde Video

Procesa feeds de cámara en vivo o archivos de video para detectar y decodificar códigos de barras y códigos QR en tiempo real.

Ver: [Guía de Lector de Códigos de Barras y QR](Guides/barcode-qr-reader-guide.md)

### Grabación Pre-Evento

Implementa grabación con buffer circular que captura video continuamente y escribe clips de eventos (incluyendo metraje anterior al disparador) en disco.

Ver: [Guía de Grabación Pre-Evento](Guides/pre-event-recording.md)

## Soporte de Plataformas

| Plataforma | Frameworks de UI | Notas |
| ---------- | ---------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Consola | Conjunto completo de características incluyendo puentes DirectShow |
| macOS | MAUI, Avalonia, Consola | Acceso a cámara AVFoundation |
| Linux x64 | Avalonia, Consola | Cámara V4L2, procesamiento basado en GStreamer |
| Android | MAUI | Via integración MAUI |
| iOS | MAUI | Via integración MAUI |

## Componentes Principales del SDK

### Fuentes

Los [Bloques de Fuente](Sources/index.md) ingestan medios desde cámaras, archivos, streams de red, generadores virtuales y hardware de captura.

### Procesamiento de Video

- [Codificadores de Video](VideoEncoders/index.md) — H.264, H.265/HEVC, VP8, VP9, AV1 con aceleración GPU (NVENC, AMF, Quick Sync)
- [Decodificadores de Video](VideoDecoders/index.md) — Decodificación por hardware y software para todos los codecs principales
- [Procesamiento de Video](VideoProcessing/index.md) — Redimensionar, recortar, rotar, corrección de color, desentrelazar, efectos
- [Renderizado de Video](VideoRendering/index.md) — Mostrar video en controles WinForms, WPF, MAUI y Avalonia
- [Compositor de Video en Vivo](LiveVideoCompositor/index.md) — Mezcla y composición multi-fuente

### Procesamiento de Audio

- [Codificadores de Audio](AudioEncoders/index.md) — Codificación AAC, MP3, Vorbis, Opus, FLAC
- [Procesamiento de Audio](AudioProcessing/index.md) — Filtros, efectos, conversión de frecuencia de muestreo, mezcla de canales
- [Renderizado de Audio](AudioRendering/index.md) — Reproducción en dispositivos de audio del sistema
- [Visualizadores de Audio](AudioVisualizers/index.md) — Bloques de visualización de forma de onda y espectro

### Salida y Conectividad

- [Sinks](Sinks/index.md) — Escribe a archivos MP4, WebM, AVI, MKV, TS y streams de red
- [Bloques de Salida](Outputs/index.md) — Configuraciones de salida de alto nivel
- [Puentes](Bridge/index.md) — Conecta segmentos de pipeline y sincroniza bloques
- [Demuxers](Demuxers/index.md) y [Parsers](Parsers/index.md) — Demultiplexión y análisis de streams

### Hardware y Plataformas Específicas

- [NVIDIA](Nvidia/index.md) — Bloques de aceleración por hardware NVENC/NVDEC
- [Blackmagic Decklink](Decklink/index.md) — Hardware profesional de captura y reproducción
- [OpenCV](OpenCV/index.md) — Integración de visión por computadora
- [OpenGL](OpenGL/index.md) — Procesamiento de video basado en GPU
- [AWS](AWS/index.md) — Bloques de integración con la nube
- [Servidor RTSP](RTSPServer/index.md) — Sirve video como stream RTSP

## Comenzar

Listo para construir tu primer pipeline? La Guía de Inicio Rápido para Desarrolladores cubre la instalación, conceptos principales, arquitectura de pipeline y ejemplos de código paso a paso:

[Guía de Inicio Rápido para Desarrolladores](GettingStarted/index.md)

Tutoriales adicionales de inicio:

- [Implementación Completa de Pipeline](GettingStarted/pipeline.md)
- [Desarrollo de Reproductor de Medios](GettingStarted/player.md)
- [Aplicación de Visor de Cámara](GettingStarted/camera.md)
- [Enumeración de Dispositivos](GettingStarted/device-enum.md)

## Guías

- [Guardar Stream RTSP en Archivo](Guides/rtsp-save-original-stream.md)
- [Visor de Stream RTSP y Reproductor de Cámaras IP](Guides/rtsp-player-csharp.md)
- [Captura ONVIF con Post-Procesamiento](Guides/onvif-capture-with-postprocessing.md)
- [Lector de Códigos de Barras y QR](Guides/barcode-qr-reader-guide.md)
- [Grabación Pre-Evento](Guides/pre-event-recording.md)
- [Etiquetas de Metadatos de Audio](Guides/audio-metadata-tags.md)
- [Efectos de Video Personalizados y Shaders OpenGL](Guides/custom-video-effects-csharp.md)

## Recursos para Desarrolladores

- [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)
- [Guía de Despliegue](../deployment-x/index.md)
- [Referencia de API](https://api.visioforge.org/dotnet/api/index.html)
- [Registro de Cambios](../changelog.md)
- [Contrato de Licencia de Usuario Final](../../eula.md)
- [Información de Licenciamiento](../../../licensing.md)
