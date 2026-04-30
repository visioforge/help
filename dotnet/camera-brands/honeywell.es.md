---
title: Honeywell - URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecta cámaras Honeywell Performance Series y equIP en C# .NET con patrones de URL RTSP y ejemplos de código para modelos HD, HDZ, HBD, HBW y PSIA.
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
  - H.265
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Honeywell en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Honeywell Commercial Security** (parte de Honeywell Building Technologies) es un importante fabricante de equipos empresariales de videovigilancia. Las cámaras Honeywell se despliegan ampliamente en edificios comerciales, infraestructura crítica, instalaciones gubernamentales y sistemas de transporte en todo el mundo. Honeywell adquirió varias marcas de cámaras a lo largo de los años incluyendo **Samsung Techwin** (brevemente) y comercializa cámaras bajo las líneas de productos **Performance Series**, **30 Series** y **60 Series**.

**Datos clave:**

- **Líneas de producto:** Performance Series (equIP, serie H), 30 Series (HC30W, HC35W), 60 Series (HC60W), HDZ/HD (equIP antigua), HBD/HBW (bullet/domo), IPCAM (consumo)
- **Soporte de protocolos:** RTSP, ONVIF (Perfil S/G/T), PSIA, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / 1234 (Performance Series); admin / admin (modelos antiguos); varía según modelo y firmware
- **Soporte ONVIF:** Sí (todos los modelos actuales Performance Series y 30/60 Series)
- **Códecs de video:** H.264, H.265 (modelos actuales), MPEG-4 (antiguos)

## Patrones de URL RTSP

### Modelos Actuales (Performance Series, 30/60 Series)

| Flujo | URL RTSP | Códec | Notas |
|--------|----------|-------|-------|
| Flujo principal (H.264) | `rtsp://IP:554/h264` | H.264 | Flujo primario |
| Flujo principal (H.265) | `rtsp://IP:554/h265` | H.265 | Solo modelos actuales |
| Flujo de canal (H.264) | `rtsp://IP:554/cam1/h264` | H.264 | Específico por canal |
| Flujo de canal (MPEG-4) | `rtsp://IP:554/cam1/mpeg4` | MPEG-4 | Alternativa antigua |
| Flujo PSIA | `rtsp://IP:554/PSIA/Streaming/channels/1` | H.264 | Compatible con PSIA |

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Resolución | Notas |
|-------------|----------|------------|-------|
| HC30W/HC35W (30 Series) | `rtsp://IP:554/h264` | Hasta 5MP | Wi-Fi actual |
| HC60W (60 Series) | `rtsp://IP:554/h264` | Hasta 4K | Cableada actual |
| HD45IP | `rtsp://IP:554/h264` | 1080p | Domo equIP |
| HD54IP | `rtsp://IP:554/h264` | 1080p | Caja equIP |
| HD55IPX | `rtsp://IP:554/h264` | 1080p+ | Caja equIP |
| HDZ20HDEX/HDZ20HDX | `rtsp://IP:554/h264` | 1080p | PTZ equIP |
| HD4MDIP | `rtsp://IP:554/cam1/mpeg4` | 720p | Multicanal |
| HDM3DIP | `rtsp://IP:554/cam1/mpeg4` | 720p | Mini domo |
| Serie HBD/HBW | `rtsp://IP:554/h264` | Hasta 4MP | Bullet/domo |

### Streaming PSIA

Las cámaras Honeywell que soportan **PSIA (Physical Security Interoperability Alliance)** usan un formato de URL diferente:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Canal 1 | `rtsp://IP:554/PSIA/Streaming/channels/1` | Primer canal |
| Canal 2 | `rtsp://IP:554/PSIA/Streaming/channels/2` | Segundo canal |

### Modelos Anteriores (Solo HTTP)

Las cámaras de consumo Honeywell más antiguas (serie IPCAM) usan HTTP:

| Modelo | URL | Notas |
|-------|-----|-------|
| IPCAM / IPCAM-PT | `http://IP/img/snapshot.cgi?size=3` | Captura JPEG |
| IPCAM-PT | `http://IP/img/video.mjpeg` | Flujo MJPEG |
| IPCAM-PT | `http://IP/img/video.asf` | Flujo ASF (audio) |
| IPCAM-OD / IPCAM-W12 | `http://IP/img/video.mjpeg` | Flujo MJPEG |
| IPCAM-OD / IPCAM-W12 | `http://IP/img/video.asf` | Flujo ASF (audio) |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Honeywell con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Honeywell Performance Series camera, main stream
var uri = new Uri("rtsp://192.168.1.75:554/h264");
var username = "admin";
var password = "YourPassword";
```

Para flujos de canal PSIA, use `/PSIA/Streaming/channels/1` en su lugar. Para modelos multicanal, use el formato `/cam1/h264`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/img/snapshot.cgi?size=3` | La mayoría de modelos |
| Flujo MJPEG | `http://IP/img/video.mjpeg` | MJPEG continuo |
| Flujo ASF | `http://IP/img/video.asf` | ASF con audio |
| Captura HREP | `http://IP/cgi-bin/webra_fcgi.fcgi?api=get_jpeg_raw&chno=1` | Captura de canal del NVR |

## Solución de Problemas

### Formato de URL RTSP

Las cámaras Honeywell usan un formato de URL RTSP simple comparado con otras marcas:

- Principal: `rtsp://IP:554/h264` (sin rutas complejas)
- Multicanal: `rtsp://IP:554/cam1/h264` (número de canal en la ruta)
- PSIA: `rtsp://IP:554/PSIA/Streaming/channels/1` (estándar PSIA)

Si `/h264` no funciona, intente `/cam1/h264` o la URL PSIA.

### Las credenciales predeterminadas varían

Honeywell ha usado diferentes credenciales predeterminadas según las líneas de producto:

- **Performance Series:** admin / 1234 (debe cambiarse en el primer inicio)
- **30/60 Series:** Configurado durante la instalación inicial (sin predeterminado)
- **equIP antigua:** admin / admin
- **Serie IPCAM:** admin / (vacío) o admin / admin

### PSIA vs ONVIF

Las cámaras Honeywell soportan tanto PSIA como ONVIF:

- **ONVIF** es recomendado para nuevas integraciones (mayor compatibilidad)
- **PSIA** es el estándar de interoperabilidad antiguo de Honeywell, aún soportado en la mayoría de modelos
- Ambos proporcionan los mismos flujos de video a través de diferentes mecanismos de descubrimiento y configuración

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Honeywell?**

Para la mayoría de cámaras Honeywell actuales, use `rtsp://admin:contraseña@IP_CAMARA:554/h264`. Para modelos multicanal, use `rtsp://admin:contraseña@IP_CAMARA:554/cam1/h264`. Las cámaras compatibles con PSIA también responden a `/PSIA/Streaming/channels/1`.

**¿Las cámaras Honeywell soportan ONVIF?**

Sí. Todas las cámaras actuales Honeywell Performance Series, 30 Series y 60 Series soportan ONVIF Profile S (streaming), Profile G (grabación) y Profile T (streaming avanzado). Los modelos equIP antiguos pueden solo soportar ONVIF Profile S.

**¿Qué es PSIA en cámaras Honeywell?**

PSIA (Physical Security Interoperability Alliance) es una alternativa a ONVIF para la interoperabilidad de dispositivos. Honeywell ha soportado históricamente PSIA junto con ONVIF. Los flujos PSIA usan el formato de URL `rtsp://IP:554/PSIA/Streaming/channels/1`.

**¿Los modelos Honeywell IPCAM aún tienen soporte?**

La serie de consumo IPCAM está descontinuada. Estas cámaras solo soportan HTTP MJPEG/JPEG y no tienen RTSP. Para modelos IPCAM, use la URL de captura HTTP `http://IP/img/snapshot.cgi?size=3` o flujo MJPEG `http://IP/img/video.mjpeg`.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Bosch](bosch.md) — Par en segmento empresarial/comercial
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
