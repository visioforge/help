---
title: Cómo conectar una cámara IP ABUS en C# .NET
description: Conecte cámaras ABUS en C# .NET con patrones de URL RTSP y ejemplos de código para modelos TVIP, CASA, Digi-Lan y series TV.
---

# Cómo conectar una cámara IP ABUS en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**ABUS** (August Bremicker Soehne KG) es una empresa de seguridad alemana con sede en Wetter, Alemania. Fundada en 1924, ABUS es uno de los mayores fabricantes de productos de seguridad de Europa, conocido por cerraduras, sistemas de alarma y videovigilancia. La serie **TVIP** de cámaras IP está ampliamente desplegada en Europa, particularmente en Alemania, Austria y los países del Benelux.

**Datos clave:**

- **Líneas de productos:** TVIP (cámaras IP), CASA (consumo), TV (IP analógico legacy), Digi-Lan (IP más antiguo)
- **Numeración de modelos TVIP:** TVIP1xxxx (consumo), TVIP2xxxx (2MP), TVIP4xxxx (4MP), TVIP5xxxx (5MP), TVIP6xxxx/7xxxx (especial)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI, MJPEG
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (algunos modelos: root / pass)
- **Soporte ONVIF:** Sí (TVIP2xxxx y posteriores)
- **Códecs de video:** H.264 (TVIP2xxxx y posteriores), MJPEG (modelos más antiguos)

!!! info "Numeración de Modelos ABUS TVIP"
    El número de modelo TVIP indica el nivel de resolución: **1xxxx** = básico/consumo, **2xxxx** = 2MP (1080p), **4xxxx** = 4MP, **5xxxx** = 5MP, **6xxxx/7xxxx** = modelos especiales. Esto ayuda a identificar qué formatos de URL y códecs soporta una cámara.

## Patrones de URL RTSP

### Formatos de URL Principales

Las cámaras ABUS soportan múltiples formatos de URL RTSP dependiendo de la generación del modelo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video.mp4
```

| Patrón de URL | Descripción |
|---------------|-------------|
| `rtsp://IP:554/video.mp4` | Flujo MP4 (recomendado para modelos H.264) |
| `rtsp://IP:554/live.sdp` | Flujo SDP en vivo (modelos de consumo y legacy) |
| `rtsp://IP:554/video.h264` | Flujo H.264 directo |
| `rtsp://IP:554/VideoInput/CHANNEL/h264/1` | Formato VideoInput (modelos 4MP) |

### Serie TVIP1xxxx (Consumo)

| Modelo | URL de Flujo Principal | Notas |
|--------|------------------------|-------|
| TVIP10000 | `rtsp://IP:554/live.sdp` | Solo MJPEG |
| TVIP10500 | `rtsp://IP:554/live.sdp` | Solo MJPEG |
| TVIP10550 | `rtsp://IP:554/live.sdp` | Solo MJPEG |
| TVIP11000 | `rtsp://IP:554/live.sdp` | MJPEG/H.264 |

!!! warning "Modelos Solo MJPEG"
    Algunos modelos TVIP1xxxx más antiguos (TVIP10000, TVIP10500, TVIP10550) solo soportan codificación MJPEG sin H.264. Use las URLs de flujo MJPEG HTTP listadas en la sección de Captura y MJPEG a continuación para estos modelos.

### Serie TVIP2xxxx (2MP)

| Modelo | URL de Flujo Principal | URL Alternativa |
|--------|------------------------|-----------------|
| TVIP20000 | `rtsp://IP:554/video.mp4` | - |
| TVIP20550 | `rtsp://IP:554/video.mp4` | - |
| TVIP21550 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/live.sdp` |
| TVIP22500 | `rtsp://IP:554/video.h264` | - |

### Serie TVIP4xxxx (4MP)

| Modelo | URL de Flujo Principal | URL Alternativa |
|--------|------------------------|-----------------|
| TVIP41500 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/VideoInput/1/h264/1` |
| TVIP41550 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/VideoInput/1/h264/1` |

### Serie TVIP5xxxx (5MP)

| Modelo | URL de Flujo Principal |
|--------|------------------------|
| TVIP51550 | `rtsp://IP:554/video.mp4` |

### Serie TVIP6xxxx / TVIP7xxxx (Especial)

| Modelo | URL de Flujo Principal |
|--------|------------------------|
| TVIP61500 | `rtsp://IP:554/video.mp4` |
| TVIP71550 | `rtsp://IP:554/video.mp4` |

### Serie CASA (Consumo)

| Modelo | URL de Flujo Principal |
|--------|------------------------|
| CASA20550 | `rtsp://IP:554/live.sdp` |

### Serie Legacy TV / Digi-Lan

| Modelo | URL de Flujo Principal |
|--------|------------------------|
| Digi-Lan TV7220 | `rtsp://IP:554/live.sdp` |
| TV7240-LAN | `rtsp://IP:554/live.sdp` |
| TV32500 | `rtsp://IP:554/video.mp4` |

!!! tip "Qué Formato de URL Probar Primero"
    Para cámaras ABUS, pruebe primero `video.mp4` para transmisión H.264, luego `live.sdp` como alternativa. Para modelos TVIP1xxxx más antiguos, `live.sdp` es típicamente la única opción RTSP. El formato `VideoInput` es específico de modelos TVIP4xxxx.

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara ABUS con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// ABUS TVIP41550, main stream
var uri = new Uri("rtsp://192.168.1.90:554/video.mp4");
var username = "admin";
var password = "admin";
```

Para modelos que usan el formato `VideoInput`, use:

```csharp
// ABUS TVIP41500, VideoInput format
var uri = new Uri("rtsp://192.168.1.90:554/VideoInput/1/h264/1");
```

## URLs de Captura y MJPEG

### Capturas JPEG

| Tipo | Patrón de URL | Modelos Compatibles |
|------|---------------|---------------------|
| Captura estándar | `http://IP/jpg/image.jpg` | TVIP10500, TVIP10550, TVIP11000, TVIP20000, TVIP21550, TVIP51550 |
| Captura alta resolución | `http://IP/jpg/image.jpg?size=3` | TVIP10001, TVIP21050, TVIP71550 |
| Visor CGI | `http://IP/cgi-bin/viewer/video.jpg?channel=CH&resolution=WxH` | CASA20550, TVIP41550, TVIP51550 |
| Captura CGI simple | `http://IP/cgi-bin/video.jpg` | Digi-Lan, modelos TV |
| CGI alternativo | `http://IP/cgi-bin/jpg/image` | TVIP20050 |
| Imagen de perfil | `http://IP/cgi-bin/view/image?pro_CHANNEL` | TVIP20000, TVIP21500 |
| JPEG pull | `http://IP/jpeg/pull` | TVIP62000 |

### Flujos MJPEG

| Tipo | Patrón de URL | Modelos Compatibles |
|------|---------------|---------------------|
| MJPEG estándar | `http://IP/video.mjpg` | TVIP10000, TVIP11000, TVIP21500, TVIP21550, TVIP51550, TVIP71501 |
| MJPEG con parámetros | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | TVIP31550, TVIP21501, TVIP51550, TVIP71550 |

!!! note "Parámetros MJPEG"
    El parámetro `q` controla la calidad JPEG (1-100), `fps` establece la tasa de cuadros, e `id` es un identificador de sesión. Ajuste estos valores según sus requisitos de ancho de banda y calidad.

## Solución de Problemas

### Múltiples formatos de URL funcionan en la misma cámara

Muchas cámaras ABUS responden a varios formatos de URL RTSP y HTTP diferentes. Esto es por diseño. Para los mejores resultados:

1. Pruebe primero `rtsp://IP:554/video.mp4` para transmisión H.264
2. Recurra a `rtsp://IP:554/live.sdp` si `video.mp4` no funciona
3. Use `http://IP/video.mjpg` para transmisión MJPEG como último recurso

### Los modelos TVIP1xxxx más antiguos no tienen H.264

Algunas cámaras TVIP1xxxx de primera generación (TVIP10000, TVIP10500, TVIP10550) solo soportan codificación MJPEG. Estas cámaras no responderán a URLs RTSP `video.mp4` o `video.h264`. Use el flujo MJPEG HTTP (`http://IP/video.mjpg`) o la URL RTSP `live.sdp` en su lugar.

### Las credenciales predeterminadas varían según el modelo

La mayoría de cámaras ABUS usan `admin` / `admin` como credenciales predeterminadas. Sin embargo, algunos modelos tienen como predeterminado `root` / `pass`. Si la autenticación falla con un conjunto, pruebe el otro. Consulte la documentación de la cámara para las credenciales predeterminadas específicas.

### Decodificación del número de modelo TVIP

Si no está seguro de qué formato de URL usar, el número de modelo TVIP proporciona orientación:

- **TVIP1xxxx:** Comience con `live.sdp`, puede ser solo MJPEG
- **TVIP2xxxx:** Comience con `video.mp4`, la mayoría soportan H.264
- **TVIP4xxxx:** Comience con `video.mp4`, también pruebe `VideoInput/1/h264/1`
- **TVIP5xxxx+:** Comience con `video.mp4`

### Parámetro de resolución de captura

Para la URL `jpg/image.jpg?size=N`, el parámetro `size` controla la resolución:

- `size=1` = Resolución más baja
- `size=2` = Resolución media
- `size=3` = Resolución más alta

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras ABUS?**

Para la mayoría de cámaras ABUS actuales (TVIP2xxxx y posteriores), la URL predeterminada es `rtsp://admin:admin@CAMERA_IP:554/video.mp4`. Para modelos de consumo más antiguos (TVIP1xxxx), pruebe `rtsp://admin:admin@CAMERA_IP:554/live.sdp` en su lugar.

**¿Las cámaras ABUS soportan ONVIF?**

Sí. Las cámaras ABUS de la generación TVIP2xxxx en adelante soportan ONVIF, lo que proporciona descubrimiento y transmisión estandarizados de cámaras. Los modelos TVIP1xxxx más antiguos pueden no soportar ONVIF.

**¿Puedo usar transmisión MJPEG con cámaras ABUS?**

Sí. La mayoría de cámaras ABUS soportan transmisión MJPEG vía `http://CAMERA_IP/video.mjpg`. Esto es particularmente útil para modelos TVIP1xxxx más antiguos que no soportan codificación H.264. MJPEG usa más ancho de banda que H.264 pero es compatible con una gama más amplia de software.

**¿Qué significan los números de modelo ABUS TVIP?**

El número de cinco dígitos después de "TVIP" indica el nivel de resolución de la cámara: 1xxxx = básico/consumo, 2xxxx = 2MP (1080p), 4xxxx = 4MP, 5xxxx = 5MP, y 6xxxx/7xxxx = modelos especiales. Los números más altos generalmente indican hardware más nuevo con soporte más amplio de protocolos y códecs.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de INSTAR](instar.md) — Cámaras alemanas de consumo / hogar inteligente
- [Integración de Cámara IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Configuración de dispositivo ONVIF ABUS
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
