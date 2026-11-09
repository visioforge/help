---
title: Integrar OBS, FFMPEG y VLC con Video Capture SDK .NET
description: Integre herramientas de streaming OBS, FFMPEG y VLC con Video Capture SDK DirectShow para aplicaciones WinForms, WPF y Consola en .NET.
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - Streaming

order: 4
sidebar_label: 3rd-Party Software Usage
---

# Integrar software de terceros con Video Capture SDK

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

El Video Capture SDK .NET proporciona capacidades robustas para integrar con varias aplicaciones de software de terceros. Esta integración expande la funcionalidad de sus aplicaciones y permite mayor flexibilidad en flujos de trabajo de procesamiento de video.

## Cómo funciona la integración

El SDK usa Virtual Camera SDK como puente entre nuestro Video Capture SDK y aplicaciones de terceros. Este puente crea un dispositivo de cámara virtual que puede ser detectado y usado por cualquier aplicación compatible con DirectShow en su entorno de desarrollo.

Virtual Camera SDK se licencia por separado: sin su clave de licencia, indicada en `Virtual_Camera_Output_LicenseKey`, la cámara virtual muestra una franja TRIAL, y una licencia de Video Capture SDK .NET no la elimina.

### Puente de video

La tecnología de cámara virtual permite que los flujos de video capturados pasen sin problemas a aplicaciones externas sin pérdida de calidad o impacto significativo en el rendimiento.

### Puente de audio

Además del video, también se proporciona un puente de audio, habilitando integración audiovisual completa con software externo.

## Aplicaciones compatibles

La cámara virtual funciona con numerosas aplicaciones compatibles con DirectShow, incluyendo:

- OBS (Open Broadcaster Software)
- FFMPEG
- VLC Media Player
- Zoom y otro software de conferencias compatible con cámaras DirectShow
- Aplicaciones DirectShow personalizadas

## Tutoriales detallados

Nuestros tutoriales paso a paso le guían a través del proceso de integración con aplicaciones populares:

- [Integración de streaming FFMPEG](ffmpeg-streaming.md) - Aprenda cómo usar FFMPEG con el SDK para poderosas capacidades de streaming
- [Configuración de streaming OBS](obs-streaming.md) - Guía detallada para integrar con Open Broadcaster Software
  
## Recursos de desarrollo

Proporcionamos documentación extensa y ejemplos para ayudarle a implementar estas integraciones en sus proyectos de software. La integración funciona en todas las plataformas soportadas:

- Aplicaciones WinForms
- Aplicaciones WPF (Windows Presentation Foundation)
- Aplicaciones de Consola

---

Para ejemplos de implementación adicionales y muestras de código, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
