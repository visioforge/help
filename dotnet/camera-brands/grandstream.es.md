---
title: Cómo Conectar una Cámara IP Grandstream en C# .NET
description: Conecta cámaras Grandstream GXV y GSC en C# .NET con patrones de URL RTSP y ejemplos de código para GXV3500, GXV3610, GSC3610 y otros modelos.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Grandstream en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Grandstream Networks** es una empresa estadounidense con sede en Boston, Massachusetts, EE.UU., conocida por teléfonos VoIP y productos de vigilancia IP. Grandstream ofrece cámaras IP y codificadores de video bajo las líneas de productos **GXV** (antigua) y **GSC** (generación actual), dirigidas a mercados PYME y profesionales. Sus cámaras se despliegan frecuentemente junto con los sistemas Grandstream VoIP y UCM (Unified Communications Manager).

**Datos clave:**

- **Líneas de producto:** GXV (cámaras IP y codificadores de video, antigua), GSC (cámaras inteligentes de generación actual)
- **Soporte de protocolos:** RTSP, ONVIF (GXV36xx y serie GSC más nueva), HTTP/CGI, SIP (videollamada)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (GXV36xx y modelos GSC más nuevos)
- **Códecs de video:** H.264, H.265 (modelos GSC actuales), MPEG-4 (modelos GXV antiguos)

## Patrones de URL RTSP

### Cámaras GSC-Series Actuales

Las cámaras Grandstream GSC de generación actual usan un formato de URL basado en canal:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo primario | `rtsp://IP:554/live/ch00_0` | Flujo principal, canal 0 |
| Flujo secundario | `rtsp://IP:554/live/ch00_1` | Subflujo |

### Serie GXV (Antigua)

Las cámaras GXV más antiguas soportan múltiples formatos de URL dependiendo del modelo:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo primario | `rtsp://IP:554//0` | Flujo principal (canal 0) |
| Flujo secundario | `rtsp://IP:554//4` | Subflujo (canal 4) |
| H.264 SDP | `rtsp://IP:554/ipcam_h264.sdp` | Acceso basado en archivo SDP |
| Live H.264 | `rtsp://IP:554/live/h264` | Flujo con nombre |
| Basado en canal | `rtsp://IP:554/[CHANNEL]` | Número de canal directo |
| Flujo con auth | `rtsp://IP:554//0/888888:888888/main` | Con credenciales incrustadas |
| MPEG-4 (antiguo) | `rtsp://IP:554/cam1/mpeg4?user=USER&pwd=PASS` | Flujo MPEG-4 antiguo |

!!! info "Numeración de canales inusual"
    Grandstream usa un esquema de numeración de canales no estándar. Para cámaras de un solo canal, el canal **0** es el flujo primario y el canal **4** es el flujo secundario. Esto difiere de la mayoría de otras marcas que usan numeración secuencial.

### URLs Específicas por Modelo

| Modelo | Flujo Primario | Flujo Secundario | Tipo |
|-------|---------------|-----------------|------|
| GXV3500 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Codificador de video |
| GXV3504 (canal 1) | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Codificador de 4 canales |
| GXV3504 (canal 2) | `rtsp://IP:554/1` | `rtsp://IP:554/5` | Codificador de 4 canales |
| GXV3504 (canal 3) | `rtsp://IP:554/2` | `rtsp://IP:554/6` | Codificador de 4 canales |
| GXV3504 (canal 4) | `rtsp://IP:554/3` | `rtsp://IP:554/7` | Codificador de 4 canales |
| GXV3601 / GXV3611 | `rtsp://IP:554//4` | -- | Cámara domo |
| GXV3601 (alt) | `rtsp://IP:554/ipcam_h264.sdp` | -- | Basado en SDP |
| GXV3610 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Domo HD |
| GXV3651 / GXV3661 / GXV3662 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Cámaras FHD |
| GXV3672 | `rtsp://IP:554//0` | `rtsp://IP:554/live/ch00_0` | Exterior HD/FHD |
| GSC3610 / GSC3615 | `rtsp://IP:554/live/ch00_0` | `rtsp://IP:554/live/ch00_1` | Domo actual |
| GSC3620 | `rtsp://IP:554/live/ch00_0` | `rtsp://IP:554/live/ch00_1` | Exterior actual |

### Mapa de Canales del Codificador Multicanal (GXV3504)

El GXV3504 es un codificador de video de 4 canales con la siguiente numeración de canales:

| Entrada | Canal Primario | Canal Secundario |
|-------|----------------|-------------------|
| Entrada 1 | 0 | 4 |
| Entrada 2 | 1 | 5 |
| Entrada 3 | 2 | 6 |
| Entrada 4 | 3 | 7 |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Grandstream con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Grandstream GSC-series camera, main stream
var uri = new Uri("rtsp://192.168.1.60:554/live/ch00_0");
var username = "admin";
var password = "YourPassword";
```

Para modelos GXV antiguos, use `rtsp://IP:554//0` para el flujo primario o `rtsp://IP:554//4` para el flujo secundario.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/snapshot/view0.jpg` | Captura del canal 0 |
| Captura JPEG (canal 1) | `http://IP/snapshot/view1.jpg` | Canal 1 (multicanal) |
| Flujo HTTP | `http://IP/goform/stream?cmd=get&channel=0` | Flujo HTTP basado en canal |

## Solución de Problemas

### Confusión en la numeración de canales

La numeración de canales de Grandstream es poco convencional:

- **Cámaras de un solo canal:** Canal 0 = flujo primario, Canal 4 = flujo secundario
- **GXV3504 (codificador de 4 canales):** Canales 0-3 son flujos primarios para entradas 1-4; Canales 4-7 son flujos secundarios para entradas 1-4

Si obtiene un flujo en blanco o un error, verifique que está usando el número de canal correcto para la calidad de flujo deseada.

### Credencial predeterminada de fábrica `888888`

Algunos modelos GXV más antiguos de Grandstream usan `888888` como contraseña predeterminada (o incrustada en la URL RTSP como `888888:888888`). Si `admin` / `admin` no funciona, intente `888888` como contraseña.

### RTSP no habilitado

En algunos modelos GXV más antiguos, el streaming RTSP debe habilitarse explícitamente en la interfaz web de la cámara. Navegue a la página de configuración de streaming o medios y confirme que RTSP está activado y configurado en el puerto 554.

### Múltiples formatos de URL por modelo

Muchas cámaras GXV soportan varios formatos de URL RTSP simultáneamente. Si un formato no funciona, intente las alternativas:

1. `rtsp://IP:554//0` (número de canal con doble barra)
2. `rtsp://IP:554/live/ch00_0` (canal con nombre)
3. `rtsp://IP:554/ipcam_h264.sdp` (archivo SDP)
4. `rtsp://IP:554/live/h264` (flujo con nombre)

### Compatibilidad de códecs

Las cámaras GSC-series actuales soportan H.265 y H.264. Los modelos GXV antiguos pueden usar MPEG-4 por defecto. Si experimenta problemas de decodificación con un modelo antiguo, verifique la interfaz web de la cámara y cambie el códec a H.264 si está disponible.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Grandstream?**

Para cámaras GSC-series actuales, use `rtsp://admin:contraseña@IP_CAMARA:554/live/ch00_0`. Para cámaras GXV-series más antiguas, intente `rtsp://admin:contraseña@IP_CAMARA:554//0` para el flujo primario.

**¿Las cámaras Grandstream soportan ONVIF?**

Sí, la serie GXV36xx y las cámaras GSC-series actuales soportan ONVIF. Los modelos GXV35xx más antiguos y los codificadores de video generalmente no soportan ONVIF.

**¿Cuál es la diferencia entre canal 0 y canal 4?**

En cámaras Grandstream de un solo canal, el canal 0 es el flujo primario (alta calidad) y el canal 4 es el flujo secundario (menor calidad). Esta es una convención específica de Grandstream que difiere de la mayoría de otras marcas de cámaras.

**¿Puedo usar cámaras Grandstream con un sistema UCM?**

Sí. Las cámaras Grandstream se integran nativamente con los sistemas Grandstream UCM (Unified Communications Manager). Sin embargo, el acceso RTSP funciona independientemente del UCM y puede usarse con cualquier software de terceros incluyendo los SDKs de VisioForge.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Milesight](milesight.md) — Segmento de cámaras PYME/profesional
- [Integración de Cámara IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Configuración de dispositivo ONVIF Grandstream
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
