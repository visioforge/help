---
title: Sanyo: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecte cámaras Sanyo en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series VCC, VDC y VCC-HD.
---

# Cómo Conectar una Cámara IP Sanyo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Sanyo** (Sanyo Electric Co., Ltd.) fue un fabricante japonés de electrónica con sede en Osaka, Japón. La división de cámaras de seguridad de Sanyo produjo las reconocidas líneas de cámaras VCC y VDC para instalaciones de vigilancia profesional. Entre 2009 y 2011, Panasonic adquirió Sanyo Electric, y la tecnología de cámaras se integró en la línea de productos i-PRO de Panasonic. Aunque las cámaras Sanyo ya no se fabrican, muchas unidades permanecen desplegadas en instalaciones heredadas en todo el mundo.

**Datos clave:**

- **Líneas de productos:** VCC (cámaras de caja), VDC (cámaras domo), VCC-HD (serie HD)
- **Estado:** Descontinuado (adquirido por Panasonic 2009-2011)
- **Soporte de protocolos:** RTSP, HTTP/CGI, ONVIF limitado (firmware más reciente)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Limitado (solo firmware antiguo)
- **Códecs de vídeo:** H.264, MJPEG
- **Sucesor:** Panasonic i-PRO

!!! warning "Las Cámaras Sanyo Están Descontinuadas"
    Las cámaras de seguridad Sanyo están descontinuadas. Sanyo Electric fue adquirida por Panasonic, y la tecnología de cámaras se integró en la línea de productos i-PRO de Panasonic. Consulte nuestra [guía de conexión Panasonic/i-PRO](panasonic.md) para productos actuales.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Sanyo utilizan la ruta RTSP `VideoInput`:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/VideoInput/1/h264/1
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `VideoInput` | 1, 2, 3... | Canal de cámara (1 para cámaras independientes) |
| `h264` | h264 | Códec de vídeo H.264 |
| `1` final | 1 | Índice de flujo |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|----------------------|-------|
| VCC-HD2300P | Cámara de caja HD | `rtsp://IP:554/VideoInput/1/h264/1` | Flujo principal H.264 |
| VCC-HD series | Cámaras HD | `rtsp://IP:554/VideoInput/1/h264/1` | Flujo principal H.264 |
| VCC-9574N | Cámara de caja | `rtsp://IP:554/VideoInput/1/h264/1` | Flujo principal H.264 |
| VCC-P9574N | Cámara PTZ | `rtsp://IP:554/VideoInput/1/h264/1` | Flujo principal H.264 |
| VDC series | Cámaras domo | `rtsp://IP:554/VideoInput/1/h264/1` | Flujo principal H.264 |

### URLs de Canales del DVR

Para sistemas DVR Sanyo con múltiples canales:

| Canal | URL de Flujo Principal |
|-------|----------------------|
| Cámara 1 | `rtsp://IP:554/VideoInput/1/h264/1` |
| Cámara 2 | `rtsp://IP:554/VideoInput/2/h264/1` |
| Cámara N | `rtsp://IP:554/VideoInput/N/h264/1` |

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/VideoInput/1/h264/1` | Flujo H.264 estándar (recomendado) |
| `rtsp://IP:554/VideoInput/CHANNEL/h264/1` | Acceso DVR multicanal |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Sanyo con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Sanyo VCC-HD2300P, main stream
var uri = new Uri("rtsp://192.168.1.90:554/VideoInput/1/h264/1");
var username = "admin";
var password = "YourPassword";
```

Para acceso DVR multicanal, reemplace `VideoInput/1` con el número de canal apropiado.

## URLs de Captura y MJPEG

!!! note "Endpoint liveimg.cgi de Sanyo"
    Las cámaras Sanyo utilizan un endpoint `/liveimg.cgi` distintivo para capturas HTTP y flujos MJPEG. El parámetro `serverpush=1` habilita la transmisión MJPEG continua.

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura en Vivo | `http://IP/liveimg.cgi` | Fotograma JPEG único |
| Flujo MJPEG | `http://IP/liveimg.cgi?serverpush=1` | MJPEG continuo por server-push |
| MJPEG con Canal | `http://IP/liveimg.cgi?serverpush=1&jpeg=1&stream=CHANNEL` | Flujo MJPEG específico por canal |
| Captura de Canal (DVR) | `http://IP/liveimg.cgi?ch=CHANNEL` | Captura específica por canal para DVR |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras Sanyo utilizan autenticación básica por defecto. Asegúrese de proporcionar las credenciales correctas:

1. Acceda a la cámara en `http://CAMERA_IP` desde un navegador
2. Inicie sesión con sus credenciales (predeterminadas: admin / admin)
3. Verifique que el servicio RTSP esté habilitado en la configuración de red
4. Use esas credenciales en su URL RTSP

### Flujo H.264 no disponible

Los modelos Sanyo más antiguos pueden soportar solo MJPEG. Si la URL H.264 no funciona, intente usar el flujo HTTP MJPEG en su lugar:

```
http://CAMERA_IP/liveimg.cgi?serverpush=1
```

### Firmware y compatibilidad

Dado que las cámaras Sanyo están descontinuadas, las actualizaciones de firmware ya no están disponibles. Si encuentra problemas de compatibilidad:

- Asegúrese de que el firmware de la cámara sea la última versión disponible
- Intente usar el descubrimiento ONVIF si la conexión directa por URL falla
- Considere migrar a cámaras Panasonic i-PRO, que heredan la tecnología de Sanyo

### El server-push MJPEG no funciona

El parámetro `serverpush=1` requiere que el servidor HTTP de la cámara soporte codificación de transferencia fragmentada. Algunas versiones de firmware más antiguas pueden no soportar esto de manera confiable. Intente el endpoint de captura única (`/liveimg.cgi` sin parámetros) y consulte a la frecuencia de fotogramas deseada.

## Preguntas Frecuentes

**¿Las cámaras Sanyo todavía tienen soporte?**

Las cámaras de seguridad Sanyo están descontinuadas. Sanyo Electric fue completamente adquirida por Panasonic, y la tecnología de cámaras de vigilancia se fusionó en la línea de productos i-PRO de Panasonic. No hay nuevas actualizaciones de firmware ni soporte disponibles para cámaras de marca Sanyo.

**¿Cuál es la URL RTSP predeterminada para cámaras Sanyo?**

La URL es `rtsp://admin:password@CAMERA_IP:554/VideoInput/1/h264/1` para el flujo principal H.264. Para configuraciones DVR, reemplace `VideoInput/1` con el número de canal apropiado (por ejemplo, `VideoInput/2` para el canal 2).

**¿Las cámaras Sanyo soportan ONVIF?**

Solo algunas cámaras Sanyo con versiones de firmware más recientes tienen soporte ONVIF limitado. La mayoría de los modelos más antiguos no soportan ONVIF y requieren configuración directa de URL RTSP.

**¿Qué debería usar en lugar de cámaras Sanyo?**

La línea de productos i-PRO de Panasonic es la sucesora directa de la división de cámaras de seguridad de Sanyo. Las cámaras i-PRO utilizan rutas RTSP VideoInput similares y ofrecen características modernas como H.265, analítica avanzada y soporte ONVIF completo. Consulte nuestra [guía de conexión Panasonic/i-PRO](panasonic.md).

**¿Cómo obtengo capturas de una cámara Sanyo?**

Use el endpoint HTTP `/liveimg.cgi`: `http://CAMERA_IP/liveimg.cgi` devuelve un fotograma JPEG único. Agregue `?serverpush=1` para un flujo MJPEG continuo, o `?ch=CHANNEL` para un canal DVR específico.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Panasonic/i-PRO](panasonic.md) — Línea de productos sucesora
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
