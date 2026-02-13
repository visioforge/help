---
title: Cómo Conectar una Cámara IP Panasonic (i-PRO) en C# .NET
description: Conecta cámaras Panasonic i-PRO y cámaras antiguas WV/BL/BB en C# .NET con patrones de URL RTSP y ejemplos de código para modelos actuales y anteriores.
---

# Cómo Conectar una Cámara IP Panasonic (i-PRO) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Panasonic i-PRO** (anteriormente Panasonic Security Systems, ahora operando como i-PRO Co., Ltd.) es un fabricante japonés de equipos profesionales de videovigilancia. Originalmente parte de Panasonic Corporation, la división de seguridad se separó como **i-PRO** en 2019. Las cámaras Panasonic/i-PRO se despliegan ampliamente en entornos empresariales, gubernamentales, de transporte y comerciales en todo el mundo.

**Datos clave:**

- **Líneas de producto:** WV-S (serie S actual), WV-X (serie X con IA), WV-SF/SC/SP/SW (generación intermedia), WV-NP/NS/NW (profesional antigua), BL (consumo/PYME), BB/KX-HCM (consumo antigua)
- **Soporte de protocolos:** RTSP, ONVIF (Perfil S/G/T), HTTP/CGI, MJPEG, propietario de Panasonic
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / 12345 (modelos actuales requieren cambio de contraseña en el primer inicio); modelos antiguos BB/BL a menudo no tenían contraseña predeterminada
- **Soporte ONVIF:** Sí (todos los modelos actuales WV-S/WV-X)
- **Códecs de video:** H.264, H.265 (modelos actuales), MPEG-4 (antiguos), MJPEG

## Patrones de URL RTSP

### Modelos Actuales (Series WV-S/WV-X, i-PRO)

Las cámaras Panasonic i-PRO actuales usan el formato de URL `MediaInput`:

| Flujo | URL RTSP | Códec | Notas |
|--------|----------|-------|-------|
| Flujo H.264 | `rtsp://IP:554/MediaInput/h264` | H.264 | Flujo RTSP principal |
| Flujo H.265 | `rtsp://IP:554/MediaInput/h265` | H.265 | Solo modelos actuales |
| Flujo MPEG-4 | `rtsp://IP:554/MediaInput/mpeg4` | MPEG-4 | Alternativa antigua |
| Flujo ONVIF | `rtsp://IP//ONVIF/MediaInput` | H.264 | Compatible con ONVIF (note la doble barra) |

!!! warning "Doble barra en URLs ONVIF"
    Las URLs ONVIF de Panasonic usan una doble barra antes de `ONVIF`: `rtsp://IP//ONVIF/MediaInput`. Esto es intencional y necesario para conexiones basadas en ONVIF.

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Generación |
|-------------|----------|------------|
| WV-S1131/S1132 | `rtsp://IP:554/MediaInput/h264` | Actual (i-PRO) |
| WV-S2131L/S2231L | `rtsp://IP:554/MediaInput/h264` | Actual (i-PRO) |
| WV-X1551L/X2251L | `rtsp://IP:554/MediaInput/h264` | Serie actual con IA |
| WV-SF132/SF135/SF138 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia |
| WV-SF332/SF335/SF346 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia |
| WV-SC384/SC385/SC386 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia |
| WV-SP105/SP306/SP508 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia |
| WV-SW115/SW155/SW175 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia exterior |
| WV-SW316/SW352/SW355 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia exterior |
| WV-SW395/SW396/SW458 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia exterior |
| WV-SW558/SW559/SW598 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia exterior |
| WV-ST162/ST165 | `rtsp://IP:554/MediaInput/h264` | Generación intermedia PTZ |
| WV-NP240/NP244/NP304 | `rtsp://IP:554/MediaInput/mpeg4` | Profesional antigua |
| WV-NP502/NP1000/NP1004 | `rtsp://IP:554/MediaInput/mpeg4` | Profesional antigua |
| WV-NS202/NS324/NS954 | `rtsp://IP:554/MediaInput/mpeg4` | PTZ antigua |
| WV-NW484/NW502/NW960/NW964 | `rtsp://IP:554/MediaInput/mpeg4` | Exterior antigua |

### Modelos de Consumo Anteriores (Series BB/BL/KX)

Las cámaras de consumo Panasonic más antiguas usaban diferentes patrones de URL:

| Serie de Modelo | URL RTSP | Códec | Notas |
|-------------|----------|-------|-------|
| BL-C210/C230 | `rtsp://IP:554/MediaInput/h264` | H.264 | Modelos de consumo tardíos |
| BL-C210/C230 | `rtsp://IP:554/MediaInput/mpeg4` | MPEG-4 | Alternativa MPEG-4 |
| BL-VP101/VP104 | `rtsp://IP:554/MediaInput/h264` | H.264 | Compacta |
| BB-HCM531A/735 | `rtsp://IP/nphMpeg4/g726-640x48` | MPEG-4 | Formato muy antiguo |
| BB/BL/KX (solo HTTP) | `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` | MJPEG | Flujo MJPEG por HTTP |

!!! info "Cámaras antiguas BB/BL"
    Muchas cámaras antiguas de las series BB y BL de Panasonic no soportan RTSP en absoluto. Solo proporcionan flujos HTTP MJPEG y capturas JPEG. Las cámaras i-PRO actuales soportan completamente RTSP.

### URLs de Codificador/DVR

| Dispositivo | URL RTSP | Notas |
|--------|----------|-------|
| Codificador WJ-GXE500 | `http://IP/cgi-bin/camera` | MJPEG por HTTP |
| DVR WJ-HD220 | `http://IP/cgi-bin/jpeg` | Captura desde DVR |
| NVR WJ-ND400 | `http://IP/cgi-bin/jpeg` | Captura desde NVR |
| NVR WJ-NV200 | `http://IP/cgi-bin/checkimage.cgi?UID=USER&CAM=CHANNEL` | Captura de canal |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Panasonic con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Panasonic i-PRO camera, H.264 stream
var uri = new Uri("rtsp://192.168.1.80:554/MediaInput/h264");
var username = "admin";
var password = "YourPassword";
```

Para H.265, use `/MediaInput/h265` en su lugar.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Flujo MJPEG (actual) | `http://IP/cgi-bin/mjpeg?stream=1` | Modelos actuales WV-S/WV-X |
| Captura JPEG | `http://IP/cgi-bin/camera` | Modelos actuales |
| Captura (tamaño) | `http://IP/SnapshotJPEG?Resolution=640x480` | Modelos de generación intermedia |
| Captura (calidad) | `http://IP/SnapShotJPEG?Resolution=320x240&Quality=Motion` | Modelos antiguos |
| Flujo MJPEG (antiguo) | `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` | Modelos BB/BL/KX |
| Server push | `http://IP/cgi-bin/nphContinuousServerPush` | Push continuo de JPEG |

## Solución de Problemas

### Confusión de nombre de marca

La marca de cámaras de seguridad Panasonic ha evolucionado:

- **Panasonic** (antes de 2019): Marca completa Panasonic
- **i-PRO** (2019-presente): Separada de Panasonic como i-PRO Co., Ltd.
- Los productos actuales llevan la marca **i-PRO** pero muchos usuarios aún buscan "cámara Panasonic"

Todos usan patrones de URL RTSP compatibles dentro de su generación.

### MediaInput/h264 vs ONVIF/MediaInput

- Use `rtsp://IP:554/MediaInput/h264` para conexiones RTSP directas (recomendado)
- Use `rtsp://IP//ONVIF/MediaInput` para conexiones compatibles con ONVIF (note la doble barra)
- Ambos proporcionan el mismo flujo de video pero usan diferentes mecanismos de autenticación

### Cámaras antiguas sin RTSP

Muchas cámaras antiguas de la serie BB y BL de Panasonic (particularmente BL-C1, BL-C10, BL-C30, BL-C101, BL-C111, BL-C131 y anteriores) no soportan RTSP. Estas cámaras solo proporcionan:

- HTTP MJPEG: `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard`
- Captura HTTP: `http://IP/SnapshotJPEG?Resolution=320x240`

### MPEG-4 vs H.264

Las cámaras antiguas de las series WV-NP/NS/NW pueden solo soportar MPEG-4 sobre RTSP. Intente `MediaInput/mpeg4` si `MediaInput/h264` falla en modelos antiguos.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Panasonic/i-PRO?**

Para cámaras i-PRO actuales, use `rtsp://admin:contraseña@IP_CAMARA:554/MediaInput/h264`. Para conexiones ONVIF, use `rtsp://IP_CAMARA//ONVIF/MediaInput`. Los modelos antiguos pueden necesitar `MediaInput/mpeg4`.

**¿Panasonic es lo mismo que i-PRO?**

Sí. La división de cámaras de seguridad de Panasonic se separó como i-PRO Co., Ltd. en 2019. Las cámaras actuales llevan la marca i-PRO, pero usan los mismos patrones de URL RTSP que las cámaras Panasonic WV-series de última generación.

**¿Las cámaras Panasonic soportan H.265?**

Las cámaras i-PRO actuales (series WV-S y WV-X) soportan H.265. Use `rtsp://IP:554/MediaInput/h265` para el flujo H.265. Los modelos de generación intermedia y anteriores solo soportan H.264 y MPEG-4.

**¿Puedo conectarme a cámaras antiguas Panasonic BB/BL?**

Muchas cámaras antiguas de las series BB y BL no soportan RTSP y solo proporcionan flujos HTTP MJPEG. Use la URL HTTP MJPEG `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` con una fuente HTTP en lugar de RTSP.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Sanyo](sanyo.md) — Adquirida por Panasonic, línea de productos predecesora
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
