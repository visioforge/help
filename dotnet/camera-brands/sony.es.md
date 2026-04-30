---
title: Sony SNC: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecta cámaras IP Sony SNC en C# .NET con patrones de URL RTSP y ejemplos de código para modelos CH, DH, EB, CX e IPELA antiguos.
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

# Cómo Conectar una Cámara IP Sony en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Sony** (Sony Corporation, División de Sistemas de Seguridad) fue un importante fabricante de cámaras IP de vigilancia profesional bajo la marca **IPELA** y posteriormente la línea de productos **SNC** (Sony Network Camera). Sony salió del mercado de cámaras de seguridad en 2020, vendiendo su negocio de seguridad a **Bosch**. Sin embargo, una gran base instalada de cámaras Sony permanece en uso en todo el mundo, particularmente en instalaciones empresariales y gubernamentales.

**Datos clave:**

- **Líneas de producto:** SNC-CH (caja, H.264), SNC-DH (domo, H.264), SNC-EB/ER (serie E), SNC-CX (compacta), SNC-VB/VM/WR/XM (última generación antes de la salida), SNC-DF/RX/RZ/CS (IPELA antigua), SNT (codificadores de video)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI, propietario de Sony (DEPA)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (debe cambiarse durante la configuración)
- **Soporte ONVIF:** Sí (SNC-CH/DH y modelos más nuevos)
- **Códecs de vídeo:** H.264, H.265 (modelos tardíos), MPEG-4 (antiguos), MJPEG
- **Estado:** Sony salió del mercado de cámaras de seguridad en 2020

!!! warning "Fin de vida"
    Sony salió del mercado de cámaras de seguridad en 2020. Aunque las cámaras existentes continúan funcionando, no se publican nuevas actualizaciones de firmware ni nuevos modelos. Las URLs RTSP documentadas aquí siguen siendo válidas para las instalaciones existentes.

## Patrones de URL RTSP

### Modelos de Última Generación (SNC-CH/DH/EB/ER/CX/VB/VM/WR/XM)

| Flujo | URL RTSP | Códec | Notas |
|--------|----------|-------|-------|
| Video 1 (principal) | `rtsp://IP:554/media/video1` | H.264 | Flujo principal |
| Video 2 (sub) | `rtsp://IP:554/media/video2` | H.264 | Subflujo |
| Perfil ONVIF | `rtsp://IP//profile` | H.264 | Basado en ONVIF (note la doble barra) |
| Directo | `rtsp://IP//media/video1` | H.264 | Alternativo (doble barra) |

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Resolución | Notas |
|-------------|----------|------------|-------|
| SNC-CH110 | `rtsp://IP/media/video1` | 1280x1024 | Cámara de caja |
| SNC-CH120/CH140 | `rtsp://IP/media/video1` | 1280x1024 / 1920x1080 | Cámara de caja |
| SNC-CH160/CH180 | `rtsp://IP/media/video1` | 1920x1080 | Cámara de caja |
| SNC-CH210/CH260/CH280 | `rtsp://IP/media/video1` | 1920x1080 / 2MP | Cámara de caja |
| SNC-DH110/DH120/DH140 | `rtsp://IP/media/video1` | Hasta 1080p | Domo fijo |
| SNC-DH160/DH180 | `rtsp://IP/media/video1` | 1920x1080 | Domo fijo |
| SNC-DH210/DH260 | `rtsp://IP/media/video1` | 1920x1080 | Domo fijo |
| SNC-EB600B | `rtsp://IP/media/video1` | 1080p | Serie E |
| SNC-CX600W | `rtsp://IP:554//media/video1` | 1080p | Compacta |
| SNC-VB630/WR630/XM632 | `rtsp://IP//profile` | 1080p+ | Última generación |
| SNC-DM110 | `rtsp://IP:554//media/video1` | 720p | Mini domo |

### Modelos IPELA Anteriores (SNC-RX/RZ/DF/CS/EP)

Las cámaras Sony IPELA más antiguas típicamente no soportan RTSP y usan streaming basado en HTTP:

| Serie de Modelo | URL | Notas |
|-------------|-----|-------|
| SNC-RX530/RX550 | `http://IP/jpeg/vga.jpg` | Captura JPEG |
| SNC-RZ25/RZ30/RZ50 | `http://IP/oneshotimage.jpg` | Captura JPEG |
| SNC-DF40/DF50/DF70/DF80 | `http://IP/image` | Captura JPEG |
| SNC-CS11/CS3P/CS50P | `http://IP/oneshotimage.jpg` | Captura JPEG |
| SNC-EP520/EP580 | `http://IP/jpeg/vga.jpg` | Captura JPEG |
| SNC-M1/M3 | `http://IP/image` | MPEG-4 muy antiguo |

### URLs de Codificador de Video

| Codificador | URL | Notas |
|---------|-----|-------|
| SNT-EX101/EX104 | `http://IP/oneshotimage.jpg` | Captura por canal |
| SNT-EX104 (canal) | `http://IP/CH1/oneshotimage.jpg` | Específico por canal |
| SNT-V704 | `http://IP/CH1/oneshotimage.jpg` | Codificador de 4 canales |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Sony con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Sony SNC camera, main stream
var uri = new Uri("rtsp://192.168.1.55:554/media/video1");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `/media/video2` en su lugar.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/oneshotimage.jpg` | La mayoría de modelos SNC |
| JPEG (VGA) | `http://IP/jpeg/vga.jpg` | Resolución VGA |
| JPEG (QVGA) | `http://IP/jpeg/qvga.jpg` | Resolución QVGA |
| Flujo MJPEG | `http://IP/img/mjpeg.cgi` | MJPEG continuo |
| MJPEG (alt) | `http://IP/mjpeg` | Ruta MJPEG alternativa |
| H.264 por HTTP | `http://IP/h264` | Flujo H.264 vía HTTP |
| Imagen | `http://IP/image` | Captura genérica |
| Captura de canal | `http://IP/oneshotimage1` | Específica por canal |

## Solución de Problemas

### Doble barra en URLs

Algunos modelos Sony usan una doble barra antes de la ruta en URLs RTSP:

- `rtsp://IP//media/video1` (doble barra)
- `rtsp://IP:554/media/video1` (barra simple con puerto)

Ambos formatos generalmente funcionan, pero intente la variante con doble barra si la URL estándar falla.

### ONVIF vs RTSP directo

Las cámaras Sony soportan tanto RTSP directo como conexiones basadas en ONVIF:

- RTSP directo: `rtsp://IP:554/media/video1` (recomendado)
- ONVIF: `rtsp://IP//profile` (URL descubierta por ONVIF)

### Cámaras antiguas sin RTSP

Las cámaras Sony IPELA más antiguas (series SNC-RX, SNC-RZ, SNC-DF, SNC-CS, SNC-M) frecuentemente no soportan RTSP y solo ofrecen HTTP JPEG/MJPEG. Para estas cámaras, use la integración de capturas HTTP.

### Sony salió del mercado

Sony vendió su negocio de cámaras de seguridad en 2020. Las cámaras existentes continúan funcionando pero no reciben nuevas actualizaciones de firmware. Planifique una eventual migración al desplegar nuevas integraciones.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Sony SNC?**

Para las cámaras Sony SNC actuales, use `rtsp://admin:contraseña@IP_CAMARA:554/media/video1` para el flujo principal y `media/video2` para el subflujo.

**¿Sony todavía fabrica cámaras IP?**

No. Sony salió del mercado de cámaras de seguridad en 2020. Las cámaras Sony SNC existentes permanecen en uso y sus flujos RTSP continúan funcionando, pero no se publican nuevos modelos ni actualizaciones de firmware.

**¿Las cámaras Sony soportan ONVIF?**

Sí. Las series Sony SNC-CH, SNC-DH y posteriores soportan ONVIF Profile S. Use `rtsp://IP//profile` para conexiones basadas en ONVIF.

**¿Qué pasa con las cámaras Sony IPELA?**

IPELA fue la marca de cámaras anterior de Sony. Muchos modelos IPELA (series SNC-RX, SNC-RZ, SNC-DF) solo soportan HTTP JPEG/MJPEG, no RTSP. Los modelos IPELA posteriores (series SNC-CH/DH) sí soportan RTSP vía `media/video1`.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Canon](canon.md) — Cámaras empresariales japonesas
- [Guía de Conexión de Axis](axis.md) — Par en vigilancia empresarial
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
