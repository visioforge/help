---
title: Cámara IP GeoVision: Conexión RTSP con C# .NET y SDK
description: Conecta cámaras GeoVision en C# .NET con patrones de URL RTSP y ejemplos de código para modelos GV-BL, GV-FD, GV-VD, GV-FE y GV-DVR.
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
  - IP Camera
  - RTSP
  - ONVIF
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP GeoVision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**GeoVision** (GeoVision Inc.) es un fabricante taiwanés de cámaras IP, grabadores de video en red y software de gestión de video, con sede en Taipei, Taiwán. GeoVision es una marca bien establecida en el mercado de vigilancia empresarial y profesional, conocida por sus cámaras IP de la serie GV y la plataforma VMS de GeoVision.

**Datos clave:**

- **Líneas de producto:** GV-BL (bullet), GV-FD (domo fijo), GV-VD (domo antivandálico), GV-FE (ojo de pez), GV-CB (cubo), GV-CA (cámara), GV-DVR (grabador de video digital), GV-NVR
- **Soporte de protocolos:** RTSP, ONVIF, PSIA, HTTP/CGI
- **Puerto RTSP predeterminado:** 8554 (cámaras IP), 554 (DVR/Server)
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (modelos actuales)
- **Códecs de video:** H.264, H.265 (modelos actuales), MPEG-4 (antiguos)

!!! warning "Puerto RTSP no estándar"
    Las cámaras IP GeoVision usan el **puerto 8554** por defecto, no el estándar 554. Asegúrese de especificar el puerto correcto al construir su URL RTSP. El software GeoVision DVR/Server usa el puerto estándar 554.

## Patrones de URL RTSP

### Formato Estándar de Cámara IP

Las cámaras IP GeoVision usan un patrón de URL SDP basado en canal en el puerto 8554:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:8554//CH001.sdp
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| Puerto | 8554 | Predeterminado para cámaras IP GeoVision |
| `CH001` | CH001, CH002... | Número de canal (3 dígitos con ceros) |
| `.sdp` | Requerido | Sufijo descriptor de sesión SDP |

!!! note "Doble barra"
    Algunos modelos GeoVision requieren una doble barra (`//`) antes del identificador de canal. Si una barra simple no funciona, intente `//CH001.sdp`.

### Flujos de Cámara IP

| Flujo | URL | Notas |
|--------|-----|-------|
| Flujo principal | `rtsp://IP:8554//CH001.sdp` | Resolución completa, puerto 8554 |
| Subflujo | `rtsp://IP:8554//CH002.sdp` | Resolución menor |

### DVR / GeoVision Server

El DVR de GeoVision y el software GV-Server usan el puerto 554:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:554/CH001.sdp
```

| Canal | URL de Flujo Principal | Notas |
|---------|----------------|-------|
| Canal 1 | `rtsp://IP:554/CH001.sdp` | Puerto 554 en DVR/Server |
| Canal 2 | `rtsp://IP:554/CH002.sdp` | Puerto 554 en DVR/Server |
| Canal N | `rtsp://IP:554/CH00N.sdp` | Rellenar con ceros hasta 3 dígitos |

### Streaming PSIA

GeoVision también soporta URLs RTSP compatibles con PSIA:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:554/PSIA/Streaming/channels/1?videoCodecType=MPEG4
```

### Tabla Resumen de URLs

| Tipo de Dispositivo | URL de Flujo Principal | Puerto Predeterminado | Notas |
|-------------|----------------|--------------|-------|
| GV-BL (bullet) | `rtsp://IP:8554//CH001.sdp` | 8554 | Cámara IP estándar |
| GV-FD (domo fijo) | `rtsp://IP:8554//CH001.sdp` | 8554 | Cámara IP estándar |
| GV-VD (domo antivandálico) | `rtsp://IP:8554//CH001.sdp` | 8554 | Cámara IP estándar |
| GV-FE (ojo de pez) | `rtsp://IP:8554//CH001.sdp` | 8554 | Cámara IP estándar |
| GV-CB (cubo) | `rtsp://IP:8554//CH001.sdp` | 8554 | Cámara IP estándar |
| GV-DVR | `rtsp://IP:554/CH001.sdp` | 554 | Software DVR |
| GV-NVR | `rtsp://IP:554/CH001.sdp` | 554 | Software NVR |
| Flujo PSIA | `rtsp://IP:554/PSIA/Streaming/channels/1` | 554 | Compatible con PSIA |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara GeoVision con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// GeoVision GV-BL2702, main stream (note port 8554)
var uri = new Uri("rtsp://192.168.1.90:8554//CH001.sdp");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `CH002.sdp` en lugar de `CH001.sdp`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación básica |
| Captura JPEG (alt) | `http://IP/GetImage.cgi` | Algunos modelos |
| Captura JPEG (alt) | `http://IP/cgi-bin/getimage` | Algunos modelos |
| Captura JPEG (visor) | `http://IP/cgi-bin/viewer/video.jpg` | Interfaz del visor web |
| Imagen estática (alt) | `http://IP/cgi-bin/jpg/image.cgi` | Algunos modelos |
| Captura antigua | `http://IP/cam1.jpg` | Firmware 6.0-8.x, canal 1 |
| Captura antigua (canal N) | `http://IP/camN.jpg` | Firmware 6.0-8.x, canal N |

## Solución de Problemas

### Puerto incorrecto -- 554 vs 8554

El problema de conexión más común con cámaras GeoVision es usar el puerto incorrecto:

- **Cámaras IP** (GV-BL, GV-FD, GV-VD, GV-FE, GV-CB): Use el **puerto 8554**
- **Software DVR / GV-Server**: Use el **puerto 554**

Si obtiene un tiempo de espera de conexión, verifique que está usando el puerto correcto para su tipo de dispositivo.

### Doble barra en la ruta de URL

Algunos modelos de cámaras IP GeoVision requieren una doble barra antes del identificador de canal (`//CH001.sdp`). Si una barra simple (`/CH001.sdp`) devuelve un error, agregue la barra extra.

### Formato de numeración de canales

GeoVision usa números de canal de tres dígitos rellenados con ceros: `CH001`, `CH002`, `CH003`, etc. Usar `CH1` en lugar de `CH001` no funcionará.

### Diferencias en versiones de firmware

Las versiones de firmware más antiguas de GeoVision (6.x-8.x) pueden usar diferentes formatos de URL de captura. Si la URL de captura basada en CGI no funciona, intente el formato antiguo (`http://IP/cam1.jpg`).

## Preguntas Frecuentes

**¿Qué puerto usa GeoVision para RTSP?**

Las cámaras IP GeoVision usan el **puerto 8554** por defecto, que difiere del puerto estándar de la industria 554. El software GeoVision DVR y GV-Server usa el puerto estándar 554.

**¿Cuál es la URL RTSP predeterminada para cámaras IP GeoVision?**

La URL es `rtsp://admin:contraseña@IP_CAMARA:8554//CH001.sdp` para el flujo principal. Use `CH002.sdp` para el subflujo. Note la doble barra antes de `CH001` y el puerto 8554.

**¿Las cámaras GeoVision soportan ONVIF?**

Sí. Todos los modelos actuales de cámaras IP GeoVision soportan ONVIF Profile S. El descubrimiento ONVIF puede usarse como alternativa a la configuración manual de URL RTSP.

**¿Puedo conectarme a un DVR GeoVision y una cámara IP al mismo tiempo?**

Sí. Conéctese al DVR en el puerto 554 y a las cámaras IP individuales en el puerto 8554. Cada dispositivo tiene su propia dirección IP y endpoint RTSP.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Vivotek](vivotek.md) — Cámaras empresariales taiwanesas
- [Guía de Conexión de ACTi](acti.md) — Cámaras profesionales taiwanesas
- [Guía de Integración de Cámara RTSP](../videocapture/video-sources/ip-cameras/rtsp.md) — Configuración de flujo RTSP GeoVision
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
