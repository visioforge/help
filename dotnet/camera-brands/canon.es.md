---
title: Cómo Conectar una Cámara IP Canon en C# .NET
description: Conecta cámaras IP Canon VB-series en C# .NET con patrones de URL RTSP y ejemplos de código para modelos VB-H, VB-M, VB-S, VB-R y VB-C antiguos.
---

# Cómo Conectar una Cámara IP Canon en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Canon Inc.** es una corporación multinacional japonesa con sede en Tokio. La división de cámaras IP de Canon produce la **serie VB** de cámaras de red dirigidas a mercados profesionales y de vigilancia empresarial. Las cámaras Canon son conocidas por su calidad óptica, aprovechando la experiencia de Canon en fabricación de lentes. Canon ha estado reduciendo su línea de cámaras IP en los últimos años, enfocándose en modelos de gama alta.

**Datos clave:**

- **Líneas de producto:** Serie VB-H (caja/PTZ, actual), serie VB-M (PTZ/compacta), serie VB-S (compacta), serie VB-R (PTZ), serie VB-C (PTZ antigua)
- **Soporte de protocolos:** RTSP, ONVIF (series VB-H y VB-M), HTTP/CGI con ruta propietaria `-wvhttp-01-`
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** root / (específico de la cámara) o admin / admin (varía según modelo)
- **Soporte ONVIF:** Sí (series VB-H y VB-M)
- **Códecs de video:** H.264, H.265 (series VB-H47, VB-H761), MJPEG

## Patrones de URL RTSP

Las cámaras Canon usan streaming basado en perfiles con identificadores de canal y perfil en la ruta de la URL.

### Modelos Actuales (Series VB-H / VB-M / VB-S / VB-R)

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Basado en canal | `rtsp://IP:554/cam1/h264` | Canal 1, H.264 |
| Flujo de perfil | `rtsp://IP:554//stream/profile1=r` | Perfil 1, modo lectura (note la doble barra) |
| Perfil (corto) | `rtsp://IP:554/profile1=r` | Perfil 1, variante más corta |
| Perfil unicast | `rtsp://IP/profile1=u` | Modo unicast, sin puerto |

!!! info "Streaming basado en perfiles"
    Las cámaras Canon usan identificadores de perfil con modos de acceso: `profile1=r` para **lectura** (compatible con multicast) y `profile1=u` para **unicast** (conexión directa). Use `=r` para acceso general y `=u` cuando se conecte directamente sin multicast.

### URLs Específicas por Modelo

| Modelo | URL RTSP | Tipo | Notas |
|-------|----------|------|-------|
| VB-H41 | `rtsp://IP:554//stream/profile1=r` | Caja fija | Perfil con doble barra |
| VB-H43 / VB-H45 | `rtsp://IP:554/cam1/h264` | Caja fija | Basado en canal |
| VB-H47 | `rtsp://IP:554/cam1/h264` | Caja fija | Compatible con H.265 |
| VB-H610D / VB-H610VE | `rtsp://IP:554/cam1/h264` | Domo fijo | Actual |
| VB-H730F | `rtsp://IP:554/cam1/h264` | Domo fijo | Ojo de pez |
| VB-H751LE | `rtsp://IP:554/cam1/h264` | Bullet fija | Exterior |
| VB-H761LVE | `rtsp://IP:554/cam1/h264` | Bullet fija | Compatible con H.265 |
| VB-M40 | `rtsp://IP/profile1=u` | PTZ compacta | Unicast, sin puerto especificado |
| VB-M42 / VB-M44 | `rtsp://IP:554/cam1/h264` | PTZ compacta | Basado en canal |
| VB-M600D | `rtsp://IP/profile1=r` | Domo compacto | Modo lectura |
| VB-M620D / VB-M640V | `rtsp://IP:554/cam1/h264` | Domo compacto | Actual |
| VB-M741LE | `rtsp://IP:554/cam1/h264` | PTZ compacta | Exterior |
| VB-S30D / VB-S31D | `rtsp://IP:554/cam1/h264` | Compacta | Interior |
| VB-S800D / VB-S900F | `rtsp://IP:554/cam1/h264` | Compacta | Interior |
| VB-R11 / VB-R11VE | `rtsp://IP:554/cam1/h264` | Domo PTZ | Actual |
| VB-R12VE | `rtsp://IP:554/cam1/h264` | Domo PTZ | Exterior |

### Modelos Anteriores (Serie VB-C -- Solo HTTP)

Las cámaras VB-C antiguas no soportan RTSP. Usan URLs HTTP propietarias `-wvhttp-01-` de Canon:

| Modelo | URL HTTP | Tipo | Notas |
|-------|----------|------|-------|
| VB-C300 | `http://IP/-wvhttp-01-/GetLiveImage` | Domo PTZ | Solo HTTP |
| VB-C10 | `http://IP/-wvhttp-01-/GetLiveImage` | Compacta | Solo HTTP |
| VB-C50i | `http://IP/-wvhttp-01-/GetLiveImage` | Domo PTZ | Solo HTTP |
| VB-610 | `http://IP/-wvhttp-01-/video.cgi` | Fija | Solo HTTP |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Canon con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Canon VB-H series camera, channel 1
var uri = new Uri("rtsp://192.168.1.70:554/cam1/h264");
var username = "root";
var password = "YourPassword";
```

Para acceso basado en perfiles en modelos VB más antiguos, use `rtsp://IP:554/profile1=r` o `rtsp://IP/profile1=u` dependiendo del modelo.

## URLs de Captura y MJPEG

Canon usa un prefijo de ruta `-wvhttp-01-` distintivo para todo el acceso HTTP de imagen y video:

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Imagen en vivo | `http://IP/-wvhttp-01-/GetLiveImage` | Captura actual |
| Flujo MJPEG | `http://IP/-wvhttp-01-/video.cgi` | MJPEG continuo |
| Captura (tamaño) | `http://IP/-wvhttp-01-/GetOneShot?image_size=WIDTHxHEIGHT` | Resolución personalizada |
| Captura (continua) | `http://IP/-wvhttp-01-/GetOneShot?image_size=WIDTHxHEIGHT&frame_count=0` | Captura continua |

!!! info "Prefijo `-wvhttp-01-` de Canon"
    El prefijo de ruta `-wvhttp-01-` es exclusivo de las cámaras de red Canon. Todas las URLs HTTP de imagen y video usan este prefijo. Esta ruta distintiva puede ayudar a identificar cámaras Canon en una red.

## Solución de Problemas

### RTSP debe habilitarse en la interfaz web

Las cámaras Canon pueden no tener RTSP habilitado por defecto. Acceda a la interfaz web de la cámara y navegue a la configuración de streaming para habilitar RTSP. Sin esto, la cámara solo responderá a solicitudes HTTP.

### La serie VB-C antigua es solo HTTP

La serie VB-C (VB-C300, VB-C10, VB-C50i) y VB-610 no soportan RTSP en absoluto. Use las URLs HTTP `-wvhttp-01-` de Canon para acceso de video desde estos modelos:

- `http://IP/-wvhttp-01-/GetLiveImage` para capturas
- `http://IP/-wvhttp-01-/video.cgi` para streaming MJPEG

### Modos lectura vs unicast de perfil

Las URLs de perfil de Canon usan dos modos de acceso:

- `profile1=r` -- **Modo lectura**: Permite distribución multicast, adecuado para múltiples espectadores
- `profile1=u` -- **Modo unicast**: Conexión directa, un espectador por flujo

Si multicast no está configurado en su red, use `profile1=u` para una conexión unicast directa.

### Doble barra en algunas URLs

Algunos modelos Canon (notablemente VB-H41) requieren una **doble barra** antes de la ruta del flujo:

- VB-H41: `rtsp://IP:554//stream/profile1=r` (doble barra)
- La mayoría: `rtsp://IP:554/cam1/h264` (barra simple)

### Las credenciales predeterminadas varían

Las cámaras Canon no tienen credenciales predeterminadas universales:

- **Modelos actuales:** Frecuentemente `root` con contraseña configurada durante la instalación inicial
- **Modelos antiguos:** Pueden usar `admin` / `admin` o `root` / `camera`
- Verifique la etiqueta de la cámara o la guía de instalación para valores predeterminados específicos del modelo

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Canon?**

Para cámaras Canon VB-H y VB-M actuales, use `rtsp://root:contraseña@IP_CAMARA:554/cam1/h264`. Para modelos más antiguos, intente `rtsp://IP_CAMARA:554/profile1=r` o `rtsp://IP_CAMARA/profile1=u`.

**¿Las cámaras Canon soportan H.265?**

Modelos Canon seleccionados soportan H.265, incluyendo las series VB-H47 y VB-H761. La mayoría de las demás cámaras VB-series usan H.264. Los modelos antiguos VB-C solo soportan MJPEG por HTTP.

**¿Qué es la ruta `-wvhttp-01-` en URLs de Canon?**

El prefijo `-wvhttp-01-` es la ruta HTTP propietaria de Canon usada para todo el acceso web de imagen y video en sus cámaras de red. Se usa para capturas (`GetOneShot`, `GetLiveImage`), streaming MJPEG (`video.cgi`) y control de cámara. Esta ruta es exclusiva de las cámaras Canon.

**¿Puedo conectarme a cámaras Canon VB-C antiguas?**

Las cámaras VB-C antiguas (VB-C300, VB-C10, VB-C50i) son solo HTTP y no soportan RTSP. Puede acceder a su video usando la URL HTTP `http://IP_CAMARA/-wvhttp-01-/GetLiveImage` para capturas o `http://IP_CAMARA/-wvhttp-01-/video.cgi` para streaming MJPEG.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Sony](sony.md) — Cámaras empresariales japonesas
- [Guía de Conexión de Axis](axis.md) — Líder en vigilancia empresarial
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
