---
title: Software de Terceros con Video Capture SDK
description: Integra herramientas de streaming OBS, FFMPEG y VLC con Video Capture SDK DirectShow para aplicaciones WinForms, WPF y Consola en .NET.
---

# Integrar Software de Terceros con Video Capture SDK

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción General

El Video Capture SDK .NET proporciona capacidades robustas para integrar con varias aplicaciones de software de terceros. Esta integración expande la funcionalidad de tus aplicaciones y permite mayor flexibilidad en flujos de trabajo de procesamiento de video.

## Cómo Funciona la Integración

El SDK usa Virtual Camera SDK como puente entre nuestro Video Capture SDK y aplicaciones de terceros. Este puente crea un dispositivo de cámara virtual que puede ser detectado y usado por cualquier aplicación compatible con DirectShow en tu entorno de desarrollo.

### Puente de Video

La tecnología de cámara virtual permite que los flujos de video capturados pasen sin problemas a aplicaciones externas sin pérdida de calidad o impacto significativo en el rendimiento.

### Puente de Audio

Además del video, también se proporciona un puente de audio, habilitando integración audiovisual completa con software externo.

## Aplicaciones Compatibles

La cámara virtual funciona con numerosas aplicaciones compatibles con DirectShow, incluyendo:

- OBS (Open Broadcaster Software)
- FFMPEG
- VLC Media Player
- Zoom, Teams y otro software de conferencias
- Aplicaciones DirectShow personalizadas

## Tutoriales Detallados

Nuestros tutoriales paso a paso te guían a través del proceso de integración con aplicaciones populares:

- [Integración de Streaming FFMPEG](ffmpeg-streaming.md) - Aprende cómo usar FFMPEG con el SDK para poderosas capacidades de streaming
- [Configuración de Streaming OBS](obs-streaming.md) - Guía detallada para integrar con Open Broadcaster Software
  
## Recursos de Desarrollo

Proporcionamos documentación extensa y ejemplos para ayudarte a implementar estas integraciones en tus proyectos de software. La integración funciona en todas las plataformas soportadas:

- Aplicaciones WinForms
- Aplicaciones WPF (Windows Presentation Foundation)
- Aplicaciones de Consola

---

Para ejemplos de implementación adicionales y muestras de código, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
