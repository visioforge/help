---
title: Cómo Conectar una Cámara IP TP-Link en C# .NET
description: Conecta cámaras TP-Link y cámaras Tapo en C# .NET con patrones de URL RTSP y ejemplos de código para modelos TL-SC, NC y Tapo C.
---

# Cómo Conectar una Cámara IP TP-Link en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**TP-Link** es un fabricante global de equipos de red con sede en Shenzhen, China. Aunque es conocido principalmente por routers y equipos de red, TP-Link produce cámaras IP tanto bajo la marca **TP-Link** (serie TL-SC, ahora descontinuada) como bajo la marca de hogar inteligente **Tapo** (serie Tapo C, actualmente activa). La línea Tapo se ha convertido en una de las marcas de cámaras de consumo más vendidas a nivel mundial gracias a sus precios agresivos y configuración basada en aplicación.

**Datos clave:**

- **Líneas de producto:** Serie TL-SC (antigua, descontinuada), serie NC (cámaras en la nube, descontinuada), serie Tapo C (cámaras inteligentes para el hogar actuales)
- **Soporte de protocolos:** RTSP, HTTP/MJPEG, ONVIF (modelos Tapo con actualización de firmware), protocolo propietario en la nube
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** Varía según la generación (ver abajo)
- **Soporte ONVIF:** Serie Tapo C (requiere habilitación en la app Tapo); la serie TL-SC no tiene ONVIF
- **Códecs de video:** H.264 (todos los modelos), H.265 (Tapo C320WS y posteriores)

### Credenciales por Línea de Producto

| Línea de Producto | Usuario Predeterminado | Contraseña Predeterminada | Notas |
|-------------|-----------------|-----------------|-------|
| Serie TL-SC | admin | admin | Antigua, fija |
| Serie NC | admin | admin | Gestionada en la nube |
| Serie Tapo C | (configurado en la app) | (configurado en la app) | Debe crear credenciales RTSP en la app Tapo |

!!! info "Credenciales de cámaras Tapo"
    Las cámaras Tapo requieren que cree una **cuenta de cámara** separada en la app Tapo (Configuración Avanzada > Cuenta de Cámara) antes de que funcione el acceso RTSP. Este usuario/contraseña es diferente de su cuenta en la nube de TP-Link.

## Patrones de URL RTSP

### Serie Tapo C (Modelos Actuales)

La línea de cámaras Tapo utiliza un formato de URL RTSP sencillo:

| Modelo | URL RTSP | Flujo | Audio |
|-------|----------|--------|-------|
| Tapo C100 (interior) | `rtsp://IP:554/stream1` | Principal (1080p) | Sí |
| Tapo C100 (interior) | `rtsp://IP:554/stream2` | Sub (360p) | Sí |
| Tapo C110 (interior 3MP) | `rtsp://IP:554/stream1` | Principal (2304x1296) | Sí |
| Tapo C110 (interior 3MP) | `rtsp://IP:554/stream2` | Sub | Sí |
| Tapo C200 (giro/inclinación) | `rtsp://IP:554/stream1` | Principal (1080p) | Sí |
| Tapo C200 (giro/inclinación) | `rtsp://IP:554/stream2` | Sub (360p) | Sí |
| Tapo C210 (giro/inclinación 3MP) | `rtsp://IP:554/stream1` | Principal (2304x1296) | Sí |
| Tapo C310 (exterior) | `rtsp://IP:554/stream1` | Principal (2048x1296) | Sí |
| Tapo C320WS (exterior 2K) | `rtsp://IP:554/stream1` | Principal (2560x1440) | Sí |
| Tapo C500 (exterior PTZ) | `rtsp://IP:554/stream1` | Principal (1080p) | Sí |
| Tapo C520WS (exterior 2K PTZ) | `rtsp://IP:554/stream1` | Principal (2560x1440) | Sí |

### Serie TL-SC (Modelos Anteriores)

La serie TL-SC descontinuada utilizaba diferentes formatos de URL dependiendo del modelo:

| Modelo | URL RTSP | Códec | Audio |
|-------|----------|-------|-------|
| TL-SC3130 | `rtsp://IP:554/video.mp4` | MPEG-4 | Sí |
| TL-SC3130G | `rtsp://IP:554/video.mp4` | MPEG-4 | Sí |
| TL-SC3171 | `rtsp://IP:554/video.mp4` | MPEG-4 | Sí |
| TL-SC3171G | `rtsp://IP:554/video.mp4` | MPEG-4 | Sí |
| TL-SC3230 | `rtsp://IP:554/video.h264` | H.264 | Sí |
| TL-SC3230N | `rtsp://IP:554/video.h264` | H.264 | Sí |
| TL-SC3430 | `rtsp://IP:554/video.h264` | H.264 | Sí |
| TL-SC3430N | `rtsp://IP:554/video.h264` | H.264 | Sí |
| TL-SC4171G | `rtsp://IP:554/video.mp4` | MPEG-4 | Sí |

### Formatos de URL Alternativos TL-SC

| Patrón de URL | Códec | Notas |
|-------------|-------|-------|
| `rtsp://IP:554/video.mp4` | MPEG-4 | Principal para modelos SC3xxx |
| `rtsp://IP:554/video.h264` | H.264 | Principal para modelos SC más nuevos |
| `rtsp://IP:554/video.mjpg` | MJPEG | Menor calidad, mayor compatibilidad |
| `rtsp://IP:554/video.pro2` | MPEG-4 | Perfil alternativo |
| `rtsp://IP:554/live.sdp` | H.264 | Flujo basado en SDP |
| `rtsp://IP:554/cam1/h264` | H.264 | Formato basado en canal |
| `rtsp://IP:554/media.amp` | Auto | Firmware compatible con Axis |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara TP-Link con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// TP-Link Tapo C200, main stream
var uri = new Uri("rtsp://192.168.1.100:554/stream1");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `/stream2` en su lugar.

## URLs de Captura y MJPEG

### Serie Tapo C

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura | `http://IP/snapshot.jpg` | Puede requerir autenticación |

### Serie TL-SC

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/jpg/image.jpg` | Captura básica |
| Captura con tamaño | `http://IP/jpg/image.jpg?size=3` | Tamaño predefinido |
| Captura CGI | `http://IP/cgi-bin/jpg/image` | Basada en CGI |
| Flujo MJPEG | `http://IP/video.mjpg` | MJPEG continuo |
| MJPEG (calidad) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | Control de calidad/FPS |
| Video CGI | `http://IP/video.cgi?resolution=VGA` | Resolución específica |
| Net Video CGI | `http://IP/cgi-bin/net_video.cgi?channel=1` | Basado en canal |
| Compatible con Axis | `http://IP/axis-cgi/mjpg/video.cgi` | API Axis emulada |

## Solución de Problemas

### Cámara Tapo: "Conexión rechazada" o "No autorizado"

El problema más común con las cámaras Tapo es no configurar las credenciales RTSP:

1. Abra la **app Tapo** en su teléfono
2. Vaya a la configuración de su cámara
3. Navegue a **Configuración Avanzada > Cuenta de Cámara**
4. Cree un nombre de usuario y contraseña
5. Use estas credenciales (no su cuenta de TP-Link) en las URLs RTSP

### Cámara Tapo: ONVIF no funciona

ONVIF está deshabilitado por defecto en las cámaras Tapo. Para habilitarlo:

1. Abra la app Tapo
2. Vaya a configuración de cámara > Configuración Avanzada
3. Habilite el interruptor de **ONVIF**
4. La cámara se reiniciará

### Modelos TL-SC: URL de códec incorrecta

Las cámaras TL-SC son específicas del códec en sus URLs:

- **Serie SC3130/3171:** Use `/video.mp4` (MPEG-4)
- **Serie SC3230/3430:** Use `/video.h264` (H.264)
- Usar el códec incorrecto en la ruta de la URL resultará en que no haya flujo

### Stream2 en cámaras Tapo muestra baja resolución

Esto es por diseño. `stream2` es el subflujo destinado a menor ancho de banda. Use `stream1` para resolución completa. Puede ajustar la resolución del subflujo en la app Tapo en la configuración de la cámara.

### Modelos TL-SC: videostream.asf no funciona

El formato de URL `videostream.asf` requiere credenciales incrustadas en la URL:
`http://IP/videostream.asf?user=admin&pwd=admin&resolution=64&rate=0`

Los valores del parámetro `resolution`: 32 = 320x240, 64 = 640x480.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Tapo?**

La URL es `rtsp://usuario:contraseña@IP_CAMARA:554/stream1` para el flujo principal y `stream2` para el subflujo. Primero debe crear credenciales RTSP en la app Tapo en Configuración Avanzada > Cuenta de Cámara.

**¿Puedo usar cámaras Tapo sin el servicio en la nube de Tapo?**

Sí. Una vez que configure las credenciales RTSP a través de la app Tapo, puede acceder al flujo RTSP de la cámara directamente a través de su red local sin ninguna dependencia de la nube. La app Tapo solo es necesaria para la configuración inicial y la configuración de credenciales.

**¿Cuál es la diferencia entre las cámaras TL-SC y Tapo?**

La serie TL-SC fue la línea de cámaras IP más antigua de TP-Link (descontinuada) con gestión tradicional basada en web. Tapo es la marca actual de cámaras inteligentes para el hogar con configuración basada en aplicación. Ambas soportan RTSP pero usan diferentes patrones de URL y métodos de autenticación.

**¿Las cámaras Tapo soportan H.265?**

Algunos modelos como la Tapo C320WS y C520WS soportan codificación H.265. La mayoría de las cámaras Tapo usan H.264. Consulte las especificaciones de su modelo específico para soporte de H.265.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Reolink](reolink.md) — Alternativa de consumo con RTSP
- [Guía de Conexión de Mercusys](mercusys.md) — Submarca de TP-Link, mismo firmware
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
