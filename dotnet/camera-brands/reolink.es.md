---
title: Reolink: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecta cámaras Reolink en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series RLC, E1, Argus, CX y Duo.
---

# Cómo Conectar una Cámara IP Reolink en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Reolink** (Reolink Digital Technology Co., Ltd.) es un fabricante de cámaras IP para consumidores y prosumers con sede en Hong Kong. Fundada en 2009, Reolink ha crecido rápidamente a través de ventas directas al consumidor en Amazon y su propio sitio web, ofreciendo cámaras con precios competitivos y acceso RTSP directo. Reolink es notable por su documentación clara de URLs RTSP y fácil integración con software de terceros.

**Datos clave:**

- **Líneas de producto:** Serie RLC (cableadas PoE), serie RLN (NVRs), serie E1 (Wi-Fi pan/tilt), serie Argus (batería/solar), serie CX (visión nocturna ColorX), serie Duo (doble lente), TrackMix (seguimiento automático)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/HTTPS, protocolo propietario Reolink
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (contraseña en blanco o establecida durante configuración)
- **Soporte ONVIF:** Sí (la mayoría de los modelos actuales, puede requerir habilitación en la configuración de la cámara)
- **Códecs de vídeo:** H.264 (todos los modelos), H.265 (la mayoría de los modelos actuales)

## Patrones de URL RTSP

Reolink usa un patrón de URL consistente en la mayoría de los modelos. La principal diferencia es entre cámaras y NVRs (que usan números de canal).

### URLs RTSP de Cámaras

| Flujo | URL RTSP | Resolución | Notas |
|--------|----------|------------|-------|
| Principal (clear) | `rtsp://IP:554/h264Preview_01_main` | Completa (hasta 4K/8MP) | Flujo principal H.264 |
| Sub (fluent) | `rtsp://IP:554/h264Preview_01_sub` | Reducida (640x360) | Menor ancho de banda |

!!! info "Flujos H.265"
    Para cámaras con H.265 habilitado, la URL permanece igual (`h264Preview_01_main`). El `h264` en la ruta de URL no es específico del códec -- funciona tanto para flujos H.264 como H.265.

### URLs de Canales NVR

Para NVRs Reolink (RLN8-410, RLN16-410, RLN36, etc.), agrega el número de canal:

| Canal | URL Flujo Principal | URL Subflujo |
|---------|----------------|----------------|
| Canal 1 | `rtsp://IP:554/h264Preview_01_main` | `rtsp://IP:554/h264Preview_01_sub` |
| Canal 2 | `rtsp://IP:554/h264Preview_02_main` | `rtsp://IP:554/h264Preview_02_sub` |
| Canal 3 | `rtsp://IP:554/h264Preview_03_main` | `rtsp://IP:554/h264Preview_03_sub` |
| Canal N | `rtsp://IP:554/h264Preview_0N_main` | `rtsp://IP:554/h264Preview_0N_sub` |

### Modelos y Resoluciones

| Modelo | Resolución | Códec | Wi-Fi | RTSP |
|-------|-----------|-------|-------|------|
| RLC-410 | 2560x1440 (4MP) | H.264/H.265 | No (PoE) | Sí |
| RLC-510A | 2560x1920 (5MP) | H.264/H.265 | No (PoE) | Sí |
| RLC-520A | 2560x1920 (5MP) | H.264/H.265 | No (PoE) | Sí |
| RLC-810A | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Sí |
| RLC-811A | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Sí |
| RLC-820A | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Sí |
| RLC-1212A | 4512x2512 (12MP) | H.264/H.265 | No (PoE) | Sí |
| E1 Zoom | 2560x1920 (5MP) | H.264/H.265 | Sí | Sí |
| E1 Pro | 2560x1440 (4MP) | H.264 | Sí | Sí |
| E1 Outdoor | 2560x1920 (5MP) | H.264/H.265 | Sí | Sí |
| CX410 | 2560x1440 (4MP) | H.264/H.265 | No (PoE) | Sí |
| CX810 | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Sí |
| TrackMix PoE | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Sí |
| Duo 2 PoE | 4608x1728 (8MP) | H.264/H.265 | No (PoE) | Sí |
| Argus 3 Pro | 2560x1440 (4MP) | H.264 | Sí (batería) | Sí |

!!! warning "Cámaras Argus con batería"
    Las cámaras Argus alimentadas por batería soportan RTSP pero agotan la batería rápidamente cuando transmiten continuamente. Usa RTSP solo para pruebas o grabación activada por eventos, no para monitoreo 24/7.

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Reolink con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Reolink RLC-810A, main stream
var uri = new Uri("rtsp://192.168.1.88:554/h264Preview_01_main");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, usa `h264Preview_01_sub` en lugar de `h264Preview_01_main`. Para canales NVR, cambia el número de canal (ej., `h264Preview_03_main` para el canal 3).

## URLs de Captura

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/cgi-bin/api.cgi?cmd=Snap&channel=0&rs=abc123&user=USER&password=PASS` | Captura basada en API |

## Solución de Problemas

### RTSP no funciona -- "Connection refused"

Puede ser necesario habilitar RTSP en algunos modelos Reolink:

1. Abre la **app Reolink** o la interfaz web
2. Ve a **Configuración > Red > Avanzado > Configuración de Puertos**
3. Asegúrate de que el **puerto RTSP** esté habilitado y configurado en 554
4. Algunas versiones de firmware más antiguas tienen RTSP deshabilitado por defecto

### El flujo H.265 no se decodifica

Si tu cámara Reolink está configurada para H.265 y el flujo no se decodifica:

- El SDK soporta H.265 de forma nativa, pero asegúrate de estar usando una versión reciente del SDK
- Intenta cambiar la cámara a H.264 en **Configuración > Pantalla > Codificación** como solución alternativa
- La ruta de la URL RTSP (`h264Preview`) permanece igual independientemente del códec real

### El subflujo muestra baja calidad

El subflujo (`h264Preview_01_sub`) es intencionalmente de menor resolución (típicamente 640x360) para reducir el ancho de banda. Usa `h264Preview_01_main` para resolución completa. Puedes ajustar la calidad del subflujo en la app Reolink en la configuración de Pantalla.

### Numeración de canales NVR

Los canales del NVR Reolink están indexados desde 1 con formato de dos dígitos con ceros: `01`, `02`, `03`... `16`. El canal 0 no existe.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Reolink?**

La URL es `rtsp://admin:password@IP_CAMARA:554/h264Preview_01_main` para el flujo principal y `h264Preview_01_sub` para el subflujo. La contraseña es la que estableciste durante la configuración de la cámara.

**¿Cambia la URL RTSP al usar H.265?**

No. La ruta de URL `h264Preview_01_main` se usa tanto para flujos H.264 como H.265. El `h264` en la ruta es una convención de nomenclatura legacy, no un selector de códec.

**¿Puedo acceder a cámaras Reolink remotamente por RTSP?**

RTSP está diseñado para acceso en red local. Para acceso remoto, necesitarías configurar reenvío de puertos en tu router (reenviar el puerto 554 a la IP de la cámara) o usar una VPN. El acceso nube/P2P de Reolink usa un protocolo propietario, no RTSP.

**¿Las cámaras Reolink Duo tienen flujos RTSP separados para cada lente?**

Sí. Las cámaras Reolink Duo exponen la lente gran angular en el canal estándar y pueden proporcionar flujos adicionales. Consulta la documentación de tu modelo específico para el acceso al flujo de doble lente.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Amcrest](amcrest.md) — Alternativa de consumo con RTSP
- [Guía de Conexión TP-Link](tp-link.md) — Cámaras económicas con RTSP nativo
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
