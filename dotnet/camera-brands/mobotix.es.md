---
title: Mobotix: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecte cámaras MOBOTIX en C# .NET con patrones de URL RTSP para series classic Mx y MOVE. Incluye opciones de flujos MxPEG, MJPEG y H.264.
---

# Cómo conectar una cámara IP Mobotix en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**MOBOTIX** (MOBOTIX AG) es un fabricante alemán de cámaras IP con sede en Langmeil, Alemania, fundado en 1999. MOBOTIX fue pionero en el concepto de sistemas de video IP descentralizados donde el procesamiento inteligente, la grabación y las analíticas ocurren directamente dentro de la cámara en lugar de en un servidor central. La empresa fue adquirida por **Konica Minolta** en 2016. Las cámaras MOBOTIX son conocidas por su construcción robusta, larga vida operativa y su idoneidad para entornos industriales, exteriores y de infraestructura crítica.

**Datos clave:**

- **Líneas de productos:** M-series (exterior), D-series (domo), S-series (hemisférica), Q-series (panorámica), T-series (estación de puerta), MOVE (línea ONVIF más nueva)
- **Soporte de protocolos:** RTSP, HTTP/CGI, MxPEG (propietario); ONVIF (solo serie MOVE)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / meinsm
- **Soporte ONVIF:** Solo serie MOVE (las cámaras classic Mx no soportan ONVIF)
- **Códecs de video:** MxPEG (propietario), MJPEG, H.264 (modelos más nuevos)
- **Arquitectura:** Descentralizada, grabación y procesamiento dentro de la cámara

!!! warning "Serie Classic vs MOVE"
    Las cámaras Mobotix clásicas (series M/D/S/Q) usan principalmente el códec propietario MxPEG y no soportan ONVIF. Para ONVIF y RTSP estándar H.264/H.265, use la serie más nueva MOBOTIX MOVE.

!!! note "Acerca de MxPEG"
    MxPEG es un códec de video propietario desarrollado por MOBOTIX para uso eficiente del ancho de banda con su arquitectura descentralizada. Si su aplicación no puede decodificar MxPEG nativamente, use el flujo alternativo MJPEG vía HTTP (`/cgi-bin/faststream.jpg`) o configure la cámara para producir MJPEG o H.264 estándar donde sea compatible. El VisioForge SDK puede conectarse a cámaras MOBOTIX usando el flujo HTTP MJPEG o el flujo RTSP H.264 en modelos compatibles.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras MOBOTIX usan URLs RTSP basadas en ruta con la marca:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/mobotix.h264
```

| Flujo | Patrón de URL | Descripción |
|-------|---------------|-------------|
| Flujo principal H.264 | `rtsp://IP:554/mobotix.h264` | Flujo H.264 principal (modelos más nuevos) |
| Flujo MJPEG | `rtsp://IP:554/mobotix.mjpeg` | MJPEG sobre RTSP |

### Series de Cámaras y URLs

| Serie | Tipo | URL Recomendada | Códec |
|-------|------|-----------------|-------|
| MOVE Bullet | IP bala | `rtsp://IP:554/mobotix.h264` | H.264 |
| MOVE Dome | IP domo | `rtsp://IP:554/mobotix.h264` | H.264 |
| MOVE Vandal Dome | IP antivandálica | `rtsp://IP:554/mobotix.h264` | H.264 |
| M-series (M73, M16) | Exterior | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| D-series (D16, D26) | Domo | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| S-series (S16, S26) | Hemisférica | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| Q-series (Q26) | Panorámica 360 | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| T-series (T26) | Estación de puerta | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |

### URLs ONVIF de la Serie MOVE

Las cámaras MOBOTIX MOVE soportan ONVIF estándar y proporcionan flujos RTSP convencionales:

| Flujo | Patrón de URL | Notas |
|-------|---------------|-------|
| Flujo principal | `rtsp://IP:554/mobotix.h264` | Flujo principal H.264 |
| Subflujo | `rtsp://IP:554/mobotix.mjpeg` | Flujo secundario MJPEG |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara MOBOTIX con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// MOBOTIX MOVE or classic Mx camera, H.264 stream
var uri = new Uri("rtsp://192.168.1.90:554/mobotix.h264");
var username = "admin";
var password = "meinsm";
```

Para cámaras Mx clásicas que solo soportan MxPEG, use la URL de flujo HTTP MJPEG en su lugar (ver abajo).

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| MJPEG Resolución Completa | `http://IP/cgi-bin/faststream.jpg?stream=full` | MJPEG continuo a resolución completa |
| Flujo MxPEG | `http://IP/cgi-bin/faststream.jpg?stream=MxPEG&needlength&fps=6` | MxPEG propietario a 6 fps |
| MJPEG con FPS Controlado | `http://IP/control/faststream.jpg?stream=full&fps=10` | MJPEG limitado a 10 fps |
| Captura Actual | `http://IP/record/current.jpg` | Captura JPEG individual |

## Solución de Problemas

### Cámara Mx clásica no conecta vía RTSP

Las cámaras MOBOTIX clásicas (series M, D, S, Q, T) usan principalmente el códec propietario MxPEG. Si el flujo RTSP falla:

1. Pruebe la URL RTSP MJPEG: `rtsp://IP:554/mobotix.mjpeg`
2. Si RTSP no está disponible, use el flujo HTTP MJPEG: `http://IP/cgi-bin/faststream.jpg?stream=full`
3. Verifique que RTSP esté habilitado en la interfaz web de la cámara bajo **Admin Menu > Network Setup > RTSP Server**

### Error "401 Unauthorized"

Las cámaras MOBOTIX usan las credenciales predeterminadas `admin` / `meinsm`. Si la autenticación falla:

1. Acceda a la interfaz web de la cámara en `http://CAMERA_IP`
2. Inicie sesión con las credenciales predeterminadas o configuradas
3. Verifique que la cuenta de usuario tenga permisos de acceso a transmisión
4. Use las credenciales correctas en su URL RTSP

### El flujo MxPEG no decodifica

MxPEG es un códec propietario que los reproductores y bibliotecas de medios estándar pueden no soportar. Alternativas:

- Use el flujo alternativo MJPEG vía `http://IP/cgi-bin/faststream.jpg?stream=full`
- Configure la cámara para producir H.264 si el modelo y firmware lo soportan
- Para cámaras de la serie MOVE, el RTSP H.264 es compatible nativamente

### El descubrimiento ONVIF no encuentra la cámara

Solo las cámaras de la serie MOBOTIX MOVE soportan ONVIF. Las cámaras Mx clásicas (series M, D, S, Q, T) no implementan el protocolo ONVIF. Para cámaras clásicas, conéctese directamente usando las URLs RTSP o HTTP listadas anteriormente.

### Baja tasa de cuadros en flujos MJPEG

Las cámaras MOBOTIX clásicas pueden tener como predeterminado tasas de cuadros bajas para conservar ancho de banda. Para ajustar:

1. Abra la interfaz web de la cámara
2. Navegue a **Admin Menu > Image Control > Frame Rate**
3. Aumente la tasa de cuadros máxima
4. Para flujos HTTP, especifique los fps deseados en la URL: `http://IP/control/faststream.jpg?stream=full&fps=15`

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras MOBOTIX?**

Para cámaras MOBOTIX MOVE más nuevas, la URL predeterminada es `rtsp://admin:meinsm@CAMERA_IP:554/mobotix.h264`. Para cámaras Mx clásicas, use `rtsp://admin:meinsm@CAMERA_IP:554/mobotix.mjpeg` o el flujo HTTP MJPEG en `http://CAMERA_IP/cgi-bin/faststream.jpg?stream=full`.

**¿Qué es MxPEG y lo necesito?**

MxPEG es un códec de video propietario desarrollado por MOBOTIX para transmisión eficiente en ancho de banda en su arquitectura de cámaras descentralizada. No necesita soporte MxPEG para usar cámaras MOBOTIX con VisioForge SDK. En su lugar, use el flujo HTTP MJPEG estándar o el flujo RTSP H.264 (en modelos compatibles) como se describe en esta página.

**¿Las cámaras MOBOTIX soportan ONVIF?**

Solo la serie MOBOTIX MOVE soporta ONVIF. Las cámaras MOBOTIX clásicas (series M, D, S, Q, T) usan una interfaz web propietaria y no soportan descubrimiento ni perfiles ONVIF.

**¿Cuál es la diferencia entre las cámaras MOBOTIX classic y MOVE?**

Las cámaras MOBOTIX clásicas (series M, D, S, Q, T) usan una arquitectura descentralizada con grabación dentro de la cámara y el códec propietario MxPEG. Las cámaras de la serie MOVE son la línea de productos más nueva de MOBOTIX que sigue protocolos estándar de la industria incluyendo ONVIF, H.264 y H.265, haciéndolas más fáciles de integrar con VMS de terceros y soluciones SDK.

**¿Puedo conectarme a cámaras MOBOTIX sin ONVIF?**

Sí. Todas las cámaras MOBOTIX soportan conexiones RTSP o HTTP directas usando las URLs listadas en esta página. ONVIF no es necesario para transmisión básica de video.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Basler](basler.md) — Cámaras industriales / visión artificial
- [Guía de Conexión de FLIR](flir.md) — Imágenes industriales y térmicas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
