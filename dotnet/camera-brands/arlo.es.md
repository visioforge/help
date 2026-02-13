---
title: Cómo Conectar una Cámara Arlo en C# .NET - Soluciones RTSP
description: Limitaciones RTSP de cámaras Arlo en C# .NET. Sin soporte RTSP nativo. Opciones de solución y recomendaciones de cámaras alternativas para desarrolladores.
meta:
  - name: robots
    content: "noindex, follow"
---

# Cómo Conectar una Cámara Arlo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Arlo Technologies** es una empresa estadounidense de seguridad para el hogar inteligente con sede en Carlsbad, California. Originalmente una marca de Netgear, Arlo se independizó en 2018. Arlo es una de las marcas de cámaras de seguridad inalámbricas más vendidas en Norteamérica y Europa, conocida por cámaras exteriores con batería, timbres y cámaras con reflector.

**Datos clave:**

- **Líneas de productos:** Pro (insignia), Ultra (4K), Essential (valor), Go (celular), Floodlight, Doorbell
- **Arquitectura:** Orientada a la nube con almacenamiento local opcional (Arlo SmartHub/Estación Base)
- **Soporte RTSP:** No (eliminado del SmartHub en 2021)
- **Soporte ONVIF:** No
- **Códecs de vídeo:** H.264, H.265 (modelos selectos)
- **Dependencia de la nube:** Alta — todas las funciones requieren suscripción Arlo Secure
- **Alimentación:** Batería, solar o con cable según el modelo

!!! warning "Sin Soporte RTSP"
    Las cámaras Arlo **no** soportan RTSP. Arlo ofrecía anteriormente acceso RTSP a través del SmartHub (VMB4540/VMB5000) para modelos selectos de cámaras, pero esta función fue **eliminada** en una actualización de firmware de 2021. Actualmente no hay forma de acceder a los flujos de cámaras Arlo vía RTSP.

## Historial de RTSP en Arlo

Arlo tuvo un breve período de soporte RTSP:

| Período | Estado | Detalles |
|---------|--------|----------|
| Antes de 2019 | Sin RTSP | Acceso solo por nube |
| 2019-2021 | RTSP disponible (beta) | Vía SmartHub solo para Ultra/Pro 3/Pro 4 |
| 2021-presente | RTSP eliminado | Actualización de firmware eliminó la funcionalidad RTSP |

La función RTSP estaba disponible en el **Arlo SmartHub (VMB5000)** para:
- Arlo Ultra (VMC5040)
- Arlo Pro 3 (VMC4040P)
- Arlo Pro 4 (VMC4050P)

Nunca estuvo disponible para modelos Arlo Essential, Go o Doorbell.

## Por Qué No Hay Integración Directa

La arquitectura de Arlo impide la integración directa con SDK:

1. **Transmisión obligatoria por nube:** Todo el vídeo se enruta a través de los servidores en la nube de Arlo
2. **Sin acceso a red local:** Las cámaras se comunican con el SmartHub/estación base usando protocolos propietarios
3. **Sin puertos abiertos:** Ni las cámaras ni las estaciones base exponen endpoints de vídeo RTSP o HTTP
4. **Dependencia de suscripción:** El acceso al vídeo requiere un plan activo de Arlo Secure

## Posibles Soluciones

### Opción 1: API de Arlo (No Oficial)

Existen bibliotecas desarrolladas por la comunidad que se conectan con la API en la nube de Arlo para:

- Recuperar imágenes de captura
- Descargar clips grabados
- Activar acciones de la cámara

Estas son no oficiales y pueden dejar de funcionar con actualizaciones del servicio de Arlo. No proporcionan flujos RTSP en tiempo real.

### Opción 2: Salida HDMI del SmartHub

El Arlo SmartHub (VMB5000) tiene una salida HDMI que muestra una cuadrícula de cámaras en vivo. Puede capturar esto con una tarjeta capturadora HDMI:

```csharp
// Capture HDMI output from Arlo SmartHub via USB capture card
var pipeline = new MediaBlocksPipeline();

var captureDevice = new SystemVideoSourceBlock(captureDeviceSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(captureDevice.Output, videoRenderer.Input);
await pipeline.StartAsync();
```

Esto proporciona una vista compuesta de todas las cámaras, no flujos individuales.

## Alternativas Recomendadas

Para desarrolladores que necesitan integración RTSP directa con cámaras, estas cámaras de consumo proporcionan soporte RTSP nativo:

| Alternativa | Tipo | RTSP | Opción Batería | Guía |
|------------|------|------|---------------|------|
| Reolink Argus 3 | Batería exterior | Sí | Sí | [Guía de Conexión](reolink.md) |
| Amcrest | Exterior con cable | Sí | No | [Guía de Conexión](amcrest.md) |
| EZVIZ | Interior/exterior | Sí (requiere habilitación) | Limitado | [Guía de Conexión](ezviz.md) |
| TP-Link Tapo | Interior/exterior | Sí | No | [Guía de Conexión](tp-link.md) |
| Eufy Security | Con cable/batería | Sí (algunos modelos) | Sí | [Guía de Conexión](eufy.md) |

## Preguntas Frecuentes

**¿Las cámaras Arlo soportan RTSP?**

No. Las cámaras Arlo actualmente no soportan RTSP. Una breve beta de RTSP estuvo disponible en el SmartHub (2019-2021) para modelos selectos, pero fue eliminada en una actualización de firmware. No hay forma actual de acceder a flujos de Arlo vía RTSP.

**¿Puedo usar VisioForge SDK con cámaras Arlo?**

No directamente. Las cámaras Arlo no tienen endpoints de transmisión RTSP, ONVIF o local. La única opción de integración es capturar la salida HDMI del SmartHub usando una tarjeta capturadora. Para integración directa con SDK, use cámaras con soporte RTSP nativo.

**¿Arlo traerá de vuelta el RTSP?**

No ha habido ningún anuncio oficial de Arlo sobre restaurar el soporte RTSP. El modelo de negocio de Arlo está basado en suscripciones, y la transmisión local entra en conflicto con este enfoque.

**¿Qué cámaras con batería soportan RTSP?**

Para cámaras con batería con soporte RTSP, considere [Reolink](reolink.md) (serie Argus) o [Eufy Security](eufy.md) (modelos selectos). La mayoría de las cámaras con batería de otras marcas también son solo de nube.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Reolink](reolink.md) — Alternativa de consumo con RTSP
- [Guía de Conexión Eufy Security](eufy.md) — Consumo con RTSP parcial
- [Guía de Conexión Wyze](wyze.md) — Otra marca orientada a la nube con RTSP limitado
- [Instalación del SDK y Ejemplos](index.md#comenzar)
