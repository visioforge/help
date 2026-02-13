---
title: Cómo Conectar una Cámara Verkada en C# .NET - Restricciones de Gestión en la Nube
description: Opciones de integración de cámaras Verkada en C# .NET. Comprenda la arquitectura gestionada en la nube, limitaciones RTSP y enfoques alternativos para cámaras Verkada.
meta:
  - name: robots
    content: "noindex, follow"
---

# Cómo Conectar una Cámara Verkada en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Verkada** es una empresa estadounidense de cámaras de seguridad gestionadas en la nube con sede en San Mateo, California. Fundada en 2016, Verkada ofrece cámaras de grado empresarial con una arquitectura completamente gestionada en la nube. A diferencia de las cámaras IP tradicionales, las cámaras Verkada se gestionan exclusivamente a través de la plataforma en la nube Verkada Command — no hay flujos RTSP locales, soporte ONVIF ni acceso directo de red a las cámaras.

**Datos clave:**

- **Líneas de productos:** CD (mini domo), CB (bala), CE (domo exterior), CF (ojo de pez), CM (multi-sensor), CP (PTZ)
- **Arquitectura:** Gestionada en la nube — todo el procesamiento y acceso de vídeo pasa por la plataforma Verkada Command
- **Soporte RTSP:** No
- **Soporte ONVIF:** No
- **Acceso a red local:** Sin acceso directo — las cámaras se comunican solo con la nube de Verkada
- **Códecs de vídeo:** H.264, H.265 (gestionados por la plataforma en la nube)
- **Acceso API:** API de Verkada (basada en la nube, requiere suscripción empresarial)

!!! warning "Sin RTSP ni Transmisión Local"
    Las cámaras Verkada **no** soportan RTSP, ONVIF ni ningún protocolo de transmisión local estándar. Son dispositivos gestionados en la nube que solo pueden accederse a través de la plataforma Command o la API de Verkada. La integración directa usando la fuente RTSP del VisioForge SDK no es posible con cámaras Verkada.

## Por Qué Verkada No Tiene RTSP

La arquitectura de Verkada es fundamentalmente diferente de las cámaras IP tradicionales:

1. **Diseño orientado a la nube:** El vídeo se procesa en la cámara y se transmite a la nube de Verkada
2. **Sin puertos de red local:** Las cámaras no exponen el puerto 554 ni ningún endpoint RTSP
3. **Acceso gestionado:** Todo el acceso al vídeo pasa por Verkada Command (web/móvil)
4. **Seguridad de confianza cero:** Sin conexiones directas de cámara a cliente en la LAN

Esta arquitectura proporciona implementación simplificada y gestión centralizada pero elimina la integración directa con SDK.

## Opciones de Integración

### Opción 1: API de Verkada (Basada en la Nube)

Verkada ofrece una API REST para clientes empresariales que proporciona:

- Listado y estado de cámaras
- Exportación/descarga de vídeo (clips)
- Recuperación de miniaturas/capturas
- Datos de eventos y alertas

La API **no** proporciona flujos RTSP en vivo ni vídeo en tiempo real. Está diseñada para recuperación de clips y acceso a metadatos.

### Opción 2: Salida HDMI (Modelos Selectos)

Algunos modelos Verkada incluyen un puerto de salida HDMI para visualización local. Puede capturar esta salida usando una tarjeta capturadora HDMI:

```csharp
// Capture HDMI output from Verkada camera via USB capture card
var pipeline = new MediaBlocksPipeline();

// Use system video source (HDMI capture card appears as webcam)
var captureDevice = new SystemVideoSourceBlock(captureDeviceSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(captureDevice.Output, videoRenderer.Input);
await pipeline.StartAsync();
```

Este enfoque proporciona vídeo local en tiempo real pero requiere conectividad HDMI física y una tarjeta capturadora.

### Opción 3: Cámaras Alternativas con RTSP

Si necesita integración RTSP directa con cámaras de grado empresarial, considere estas alternativas:

| Alternativa | Segmento de Mercado | RTSP | ONVIF | Guía |
|------------|---------------------|------|-------|------|
| Axis | Empresarial | Sí | Sí | [Guía de Conexión](axis.md) |
| Bosch | Empresarial | Sí | Sí | [Guía de Conexión](bosch.md) |
| Hanwha Vision | Empresarial | Sí | Sí | [Guía de Conexión](hanwha.md) |
| Avigilon | Empresarial | Sí | Sí | [Guía de Conexión](avigilon.md) |
| Hikvision | Empresarial | Sí | Sí | [Guía de Conexión](hikvision.md) |

## Preguntas Frecuentes

**¿Puedo conectar cámaras Verkada con RTSP?**

No. Las cámaras Verkada no soportan RTSP, ONVIF ni ningún protocolo de transmisión local. Son dispositivos gestionados en la nube accesibles solo a través de la plataforma Command o la API de Verkada.

**¿Verkada tiene una API para acceso a vídeo?**

Sí, pero la API de Verkada proporciona exportación de clips y recuperación de capturas, no transmisión de vídeo en vivo. El vídeo en tiempo real solo está disponible a través de la interfaz web Verkada Command o la aplicación móvil. El acceso a la API empresarial requiere una suscripción a Verkada.

**¿Puedo usar VisioForge SDK con cámaras Verkada?**

No directamente vía RTSP. La única opción de integración local es capturar la salida HDMI de modelos Verkada selectos usando una tarjeta capturadora con la fuente de vídeo del sistema del VisioForge SDK. Para integración basada en la nube, necesitaría usar la API de Verkada por separado.

**¿Qué cámaras empresariales soportan RTSP?**

Para cámaras empresariales con soporte completo de RTSP y ONVIF, consulte nuestras guías para [Axis](axis.md), [Bosch](bosch.md), [Hanwha Vision](hanwha.md), [Avigilon](avigilon.md) e [Hikvision](hikvision.md).

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Axis](axis.md) — Alternativa empresarial con RTSP
- [Guía de Conexión Bosch](bosch.md) — Alternativa empresarial con RTSP
- [Guía de Conexión Rhombus](rhombus.md) — Otra plataforma gestionada en la nube
- [Instalación del SDK y Ejemplos](index.md#comenzar)
