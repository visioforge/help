---
title: Vivotek - URL RTSP y Conexión con Cámaras IP en C# .NET
description: Conecta cámaras IP Vivotek en C# .NET con patrones de URL RTSP y ejemplos de código para modelos FD, IP, SD, FE ojo de pez y servidores de video.
---

# Cómo Conectar una Cámara IP Vivotek en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Vivotek Inc.** es un fabricante taiwanés de soluciones de vigilancia en red con sede en New Taipei City. Fundada en 2000, Vivotek es una de las marcas líderes de cámaras IP del mundo, ampliamente desplegada en entornos empresariales, comerciales, de transporte y vigilancia urbana. Vivotek es conocida por su amplia gama de factores de forma incluyendo ojo de pez, panorámicas, domos de alta velocidad y cámaras especializadas.

**Datos clave:**

- **Líneas de producto:** FD (domo fijo), IP (caja/bullet), IB (bullet), SD (domo de alta velocidad), FE (ojo de pez), MD (domo móvil), CC (compacta), VS (servidores de video/codificadores)
- **Soporte de protocolos:** RTSP, ONVIF (Perfil S/G/T), HTTP/CGI, MJPEG
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** root / (vacío o configurado durante la instalación); modelos antiguos: root / root
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de video:** H.264, H.265, MJPEG

## Patrones de URL RTSP

### Modelos Actuales

Todas las cámaras Vivotek actuales usan el patrón de URL `live.sdp` para streaming RTSP:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo 1 (principal) | `rtsp://IP:554/live.sdp` | Flujo principal, H.264/H.265 |
| Flujo 2 (sub) | `rtsp://IP:554/live2.sdp` | Subflujo |
| Flujo 3 | `rtsp://IP:554/live3.sdp` | Tercer flujo (si es compatible) |
| Flujo 4 | `rtsp://IP:554/live4.sdp` | Cuarto flujo (algunos modelos) |

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Factor de Forma |
|-------------|----------|-------------|
| FD81xx (domo fijo) | `rtsp://IP:554/live.sdp` | Domo fijo |
| FD83xx (domo fijo) | `rtsp://IP:554/live.sdp` | Domo fijo |
| FD8134/FD8136 | `rtsp://IP:554/live.sdp` | Mini domo |
| FD8161/FD8162/FD8166 | `rtsp://IP:554/live.sdp` | Domo fijo |
| FD8335H | `rtsp://IP:554/live.sdp` | Domo fijo |
| FD8361/FD8362E/FD8372 | `rtsp://IP:554/live.sdp` | Domo fijo |
| FE8171V/FE8172V/FE8174 | `rtsp://IP:554/live.sdp` | Ojo de pez |
| IP7130/IP7131/IP7132 | `rtsp://IP:554/live.sdp` | Cámara de caja |
| IP7160/IP7161 | `rtsp://IP:554/live.sdp` | Cámara de caja |
| IP7330/IP7361 | `rtsp://IP:554/live.sdp` | Bullet |
| IP8130/IP8133/IP8152 | `rtsp://IP:554/live.sdp` | Cámara de caja |
| IP8331/IP8332/IP8335H | `rtsp://IP:554/live.sdp` | Cámara de caja |
| IP8362/IP8364 | `rtsp://IP:554/live.sdp` | Cámara de caja |
| SD8362E | `rtsp://IP:554/live.sdp` | Domo de alta velocidad |
| CC8130 | `rtsp://IP:554/live.sdp` | Compacta |
| MD7560/MD8562 | `rtsp://IP:554/live.sdp` | Domo móvil |

### Modelos Anteriores

Los modelos Vivotek más antiguos (series IP3xxx, IP6xxx, PT3xxx, PZ6xxx) usaban streaming solo por HTTP:

| Serie de Modelo | URL | Notas |
|-------------|-----|-------|
| IP3121/IP3122/IP3133/IP3135 | `http://IP/cgi-bin/video.jpg?size=2` | Solo JPEG |
| IP6127 | `http://IP/cgi-bin/video.jpg?size=2` | Solo JPEG |
| PT3112/PT3122 | `http://IP/cgi-bin/video.jpg?size=2` | Giro/inclinación, JPEG |
| PZ6114/PZ6122 | `http://IP/cgi-bin/video.jpg?size=2` | Giro/zoom, JPEG |

### URLs de Servidor de Video

Los servidores de video Vivotek codifican señales de cámaras analógicas para streaming IP:

| Modelo | URL RTSP | Notas |
|-------|----------|-------|
| VS2403 | `rtsp://IP:554/live.sdp` | Servidor de video, multicanal |
| VS3100P | `http://IP/cgi-bin/video.jpg?size=2` | Codificador antiguo |
| VS7100 | `rtsp://IP:554/live.sdp` | Servidor de video |
| VS8102 | `rtsp://IP:554/live.sdp` | Servidor de video |
| VS8401 | `rtsp://IP:554/live.sdp` | Servidor de 4 canales |
| VS8801 | `rtsp://IP:554/live.sdp` | Servidor de 8 canales |

### URLs de NVR

| Modelo | URL RTSP | Notas |
|-------|----------|-------|
| NR8x01 NVR | `rtsp://IP:554/live.sdp` | A través del NVR |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Vivotek con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Vivotek camera, main stream
var uri = new Uri("rtsp://192.168.1.50:554/live.sdp");
var username = "root";
var password = "YourPassword";
```

Para acceder al subflujo, use `/live2.sdp` en su lugar.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/cgi-bin/viewer/video.jpg?resolution=640x480` | Modelos actuales |
| Captura JPEG (canal) | `http://IP/cgi-bin/viewer/video.jpg?channel=1&resolution=640x480` | Multicanal |
| Flujo MJPEG | `http://IP/video.mjpg` | MJPEG continuo |
| Flujo MJPEG (alt) | `http://IP/video2.mjpg` | Segundo flujo |
| MJPEG (parámetros) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | Con parámetros de calidad/fps |
| Captura antigua | `http://IP/cgi-bin/video.jpg` | Modelos antiguos |
| Captura antigua (tamaño) | `http://IP/cgi-bin/video.jpg?size=2` | Modelos antiguos, VGA |
| Captura CGI | `http://IP/snapshot.cgi` | Algunos modelos |

## Solución de Problemas

### Patrón de URL consistente

A diferencia de muchas marcas, Vivotek usa el mismo patrón de URL RTSP `live.sdp` en prácticamente todos sus modelos con capacidad RTSP. Si `rtsp://IP:554/live.sdp` no funciona, intente:

- `rtsp://IP:554/live2.sdp` (subflujo)
- `rtsp://IP:554/live3.sdp` (tercer flujo)

### Credenciales predeterminadas

- **Modelos actuales:** `root` con contraseña configurada durante la instalación inicial
- **Modelos antiguos:** `root` / (contraseña vacía) o `root` / `root`
- Algunos modelos requieren configuración a través de la interfaz web antes de que RTSP sea accesible

### Puertos no estándar en algunos modelos

Algunas cámaras Vivotek pueden usar puertos RTSP no estándar (ej., 1025, 1032) si están configurados. Verifique la interfaz web de la cámara en Red > Configuración RTSP si el puerto 554 no responde.

### Cámaras antiguas solo HTTP

Las cámaras Vivotek muy antiguas (series IP31xx, IP61xx, PT31xx, PZ61xx) solo soportan flujos HTTP JPEG y MJPEG, no RTSP. Estas cámaras no pueden usar la fuente RTSP -- use la integración de captura HTTP o MJPEG en su lugar.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Vivotek?**

La URL estándar es `rtsp://root:contraseña@IP_CAMARA:554/live.sdp` para el flujo principal. Use `live2.sdp` para el subflujo y `live3.sdp` para el tercer flujo. Este patrón funciona en prácticamente todos los modelos Vivotek con capacidad RTSP.

**¿Las cámaras Vivotek soportan H.265?**

Sí. Las cámaras Vivotek actuales soportan H.265 (HEVC). Use la misma URL `live.sdp` -- el códec se configura en la interfaz web de la cámara, no en la URL.

**¿Cuál es la diferencia entre live.sdp y live2.sdp?**

`live.sdp` es el flujo principal (mayor calidad), `live2.sdp` es típicamente un subflujo de menor resolución para visualización con ancho de banda limitado, y `live3.sdp` es un tercer flujo frecuentemente usado para visualización móvil.

**¿Los servidores de video Vivotek soportan RTSP?**

Sí. Los servidores de video Vivotek actuales (VS2403, VS7100, VS8102, VS8401, VS8801) soportan RTSP usando el mismo patrón de URL `live.sdp` que las cámaras. Los servidores antiguos (VS3100P) solo soportan HTTP JPEG.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de GeoVision](geovision.md) — Cámaras empresariales taiwanesas
- [Guía de Conexión de ACTi](acti.md) — Cámaras profesionales taiwanesas
- [Captura de Cámara IP a MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md) — Grabar flujos Vivotek a archivo
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
