---
title: Guía de Conexión RTSP para Cámaras Foscam en C# .NET
description: Conecta cámaras Foscam en C# .NET con patrones de URL RTSP y HTTP, acceso API CGI y ejemplos de código para modelos FI, C1, C2, R2 y R4.
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
  - H.264
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Foscam en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Foscam** (Shenzhen Foscam Intelligent Technology Co., Ltd.) es un fabricante chino especializado en cámaras IP para consumidores y pequeñas empresas. Fundada en 2007 y con sede en Shenzhen, China, Foscam ganó popularidad por sus cámaras Wi-Fi asequibles y fue una de las primeras marcas en llevar cámaras IP de bajo costo al mercado de consumo.

**Datos clave:**

- **Líneas de producto:** Serie FI (legacy pan/tilt), C1/C2 (interior HD), R2/R4 (interior pan/tilt), SD (exterior), serie G (batería), serie VZ (timbre)
- **Soporte de protocolos:** RTSP, HTTP/CGI, ONVIF (modelos más recientes), P2P
- **Puerto RTSP predeterminado:** 88 (no 554 -- esto es único de Foscam)
- **Puerto HTTP predeterminado:** 88
- **Credenciales predeterminadas:** admin / (contraseña en blanco en modelos más antiguos); admin / (establecido durante configuración en modelos más nuevos)
- **Soporte ONVIF:** Parcial (solo modelos HD más recientes, ej., C1, C2, R2, R4)
- **Códecs de video:** H.264 (modelos HD), MJPEG (modelos legacy)

## Patrones de URL RTSP

Las cámaras Foscam usan un puerto no estándar (88) y nombres de ruta de flujo simples.

### Formato de URL

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:88/videoMain
```

!!! warning "Puerto no estándar"
    Las cámaras Foscam típicamente usan el **puerto 88** tanto para RTSP como para HTTP, no el puerto estándar 554. Este es el problema de conexión más común.

### Modelos HD (H.264)

| Serie de Modelo | URL RTSP | Flujo | Audio |
|-------------|----------|--------|-------|
| C1 / C1 Lite (interior) | `rtsp://IP:88/videoMain` | Principal (720p) | Sí |
| C1 / C1 Lite (interior) | `rtsp://IP:88/videoSub` | Sub (VGA) | Sí |
| C2 (interior 1080p) | `rtsp://IP:88/videoMain` | Principal (1080p) | Sí |
| C2 (interior 1080p) | `rtsp://IP:88/videoSub` | Sub (VGA) | Sí |
| R2 (pan/tilt 1080p) | `rtsp://IP:88/videoMain` | Principal (1080p) | Sí |
| R4 (pan/tilt 1440p) | `rtsp://IP:88/videoMain` | Principal (2560x1440) | Sí |
| FI9821W V2 (pan/tilt) | `rtsp://IP:88/videoMain` | Principal (720p) | Sí |
| FI9826W (pan/tilt/zoom) | `rtsp://IP:88/videoMain` | Principal (960p) | Sí |
| FI9828P (PTZ exterior) | `rtsp://IP:88/videoMain` | Principal (960p) | Sí |
| FI9900P (bala exterior) | `rtsp://IP:88/videoMain` | Principal (1080p) | Sí |
| SD2 (pan/tilt exterior) | `rtsp://IP:88/videoMain` | Principal (1080p) | Sí |

### Modelos Legacy (Solo MJPEG)

Los modelos Foscam más antiguos (FI8904W, FI8910W, FI8918W, FI8919W) no soportan RTSP. Usan solo transmisión HTTP:

| Modelo | URL HTTP | Tipo | Audio |
|-------|----------|------|-------|
| FI8904W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flujo ASF | Sí |
| FI8910W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flujo ASF | Sí |
| FI8918W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flujo ASF | Sí |
| FI8919W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flujo ASF | Sí |
| FI8904W | `http://IP:88/videostream.cgi?user=USER&pwd=PASS&resolution=32` | MJPEG | No |

### Puertos RTSP Alternativos

Algunos modelos Foscam pueden configurarse con puertos alternativos:

| Patrón de URL | Puerto | Notas |
|-------------|------|-------|
| `rtsp://IP:88/videoMain` | 88 | Predeterminado para la mayoría de modelos |
| `rtsp://IP:554/videoMain` | 554 | Si se reconfigura en ajustes |
| `rtsp://IP:554/cam1/mpeg4` | 554 | Algunas variantes OEM |
| `rtsp://IP:554/live1.sdp` | 554 | Firmware compatible con DCS |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Foscam con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Foscam R2, main stream -- note port 88, not 554!
var uri = new Uri("rtsp://192.168.1.30:88/videoMain");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, usa `/videoSub` en su lugar.

## URLs de Captura y MJPEG

Foscam proporciona una API CGI para capturas y control:

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura CGI (HD) | `http://IP:88/cgi-bin/CGIProxy.fcgi?cmd=snapPicture2&usr=USER&pwd=PASS` | Modelos HD |
| Captura Legacy | `http://IP:88/snapshot.cgi?user=USER&pwd=PASS` | Modelos legacy |
| Captura (conteo) | `http://IP:88/snapshot.cgi?user=USER&pwd=PASS&count=0` | Fotograma único |
| Flujo MJPEG (legacy) | `http://IP:88/videostream.cgi?user=USER&pwd=PASS&resolution=32` | MJPEG VGA |
| Flujo ASF (legacy) | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Contenedor ASF |
| Video CGI | `http://IP:88/video.cgi?resolution=VGA` | Video directo |

## Solución de Problemas

### Puerto incorrecto -- debe usar 88, no 554

El problema de conexión Foscam más común es usar el puerto 554. Las cámaras Foscam usan por defecto el **puerto 88** para todos los servicios (RTSP, HTTP y CGI). Si tu conexión se agota, verifica el número de puerto primero.

### Modelos Legacy vs HD

Foscam tiene dos generaciones de producto fundamentalmente diferentes:

- **Legacy (FI89xx):** Solo MJPEG, transmisión HTTP vía `videostream.asf` o `videostream.cgi`, sin RTSP
- **HD (C1, C2, R2, R4, FI99xx):** H.264, RTSP vía `videoMain`/`videoSub`, soporte ONVIF

Si `rtsp://IP:88/videoMain` no funciona, tu cámara es probablemente un modelo legacy -- usa las URLs de transmisión HTTP en su lugar.

### Contraseña en blanco/vacía

Las cámaras Foscam más antiguas se envían con contraseña en blanco (usuario: `admin`, contraseña: cadena vacía). El firmware más reciente requiere establecer una contraseña durante la configuración inicial. Si la autenticación falla con una contraseña, prueba una contraseña vacía para modelos legacy.

### Inestabilidad de conexión Wi-Fi

Las cámaras Foscam Wi-Fi pueden experimentar cortes en la transmisión. Recomendaciones:

- Usa modo de transporte TCP para mayor fiabilidad
- Posiciona la cámara más cerca del router Wi-Fi
- Usa Wi-Fi 2.4GHz (mejor alcance) en lugar de 5GHz
- Reduce la resolución del flujo al subflujo: `rtsp://IP:88/videoSub`

### ONVIF no disponible

ONVIF solo es soportado en modelos HD más recientes (C1, C2, R2, R4, FI99xx). Las cámaras legacy FI89xx no soportan ONVIF. Para modelos legacy, usa URLs HTTP/RTSP directas en su lugar.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Foscam?**

Para modelos HD, la URL es `rtsp://admin:password@IP_CAMARA:88/videoMain`. Nota el puerto no estándar 88 (no 554). Para modelos legacy (serie FI89xx), usa HTTP: `http://IP_CAMARA:88/videostream.asf?user=admin&pwd=password`.

**¿Por qué Foscam usa el puerto 88 en lugar del estándar 554?**

Foscam eligió el puerto 88 como predeterminado para todos los servicios de la cámara para evitar conflictos con otros dispositivos de red. Puedes cambiar esto en la interfaz web de la cámara en Configuración > Red > Puerto, pero el predeterminado es 88.

**¿Puedo cambiar el puerto RTSP de Foscam a 554?**

Sí. Accede a la interfaz web de la cámara en `http://IP_CAMARA:88`, ve a Configuración > Red > Puerto, y cambia el puerto RTSP a 554. Después de guardar y reiniciar, puedes usar el puerto estándar 554 en tus URLs RTSP.

**¿Foscam soporta control pan/tilt/zoom a través del SDK?**

Los modelos PTZ de Foscam (R2, R4, FI9821, FI9826) soportan pan/tilt a través de su API CGI y ONVIF (modelos HD). Puedes enviar comandos PTZ a través de ONVIF usando las funciones de control PTZ del VisioForge SDK.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión TP-Link](tp-link.md) — Cámaras de consumo con RTSP
- [Guía de Conexión D-Link](dlink.md) — Par del segmento consumidor
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
