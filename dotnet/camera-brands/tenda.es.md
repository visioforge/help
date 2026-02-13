---
title: Cómo Conectar una Cámara IP Tenda en C# .NET
description: Conecte cámaras Tenda en C# .NET con patrones de URL RTSP y ejemplos de código para cámaras de seguridad y modelos panorámica/inclinación de las series CP, CT e IT.
---

# Cómo Conectar una Cámara IP Tenda en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Tenda Technology** es un fabricante chino de equipos de red con sede en Shenzhen, China. Fundada en 1999, Tenda es conocida principalmente por routers y equipos de red pero se ha expandido al mercado de cámaras de seguridad con una creciente línea de cámaras IP asequibles dirigidas a los segmentos de consumo y pequeñas empresas. Las cámaras Tenda están ganando tracción en mercados emergentes de Asia, Sudamérica y África.

**Datos clave:**

- **Líneas de productos:** CP (panorámica/inclinación), CT (bala/torreta exterior), IT (interior)
- **Soporte de protocolos:** RTSP, ONVIF (modelos selectos), HTTP
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (varía según el modelo)
- **Soporte ONVIF:** Sí (modelos más recientes)
- **Códecs de vídeo:** H.264, H.265 (modelos selectos)
- **Aplicación complementaria:** Aplicación Tenda Security (para configuración y visualización remota)

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Tenda utilizan una URL RTSP basada en número de flujo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/stream1
```

| Flujo | Patrón de URL | Descripción |
|-------|---------------|-------------|
| Flujo principal | `rtsp://IP:554/stream1` | Resolución completa |
| Subflujo | `rtsp://IP:554/stream2` | Menor resolución |

### Modelos de Cámaras

| Modelo | Tipo | Resolución | URL de Flujo Principal | Audio |
|--------|------|-----------|----------------------|-------|
| CP3 (PT interior) | PTZ Interior | 1920x1080 | `rtsp://IP:554/stream1` | Sí |
| CP6 (PT 2K) | PTZ Interior | 2304x1296 | `rtsp://IP:554/stream1` | Sí |
| CP7 (PT 4MP) | PTZ Interior | 2560x1440 | `rtsp://IP:554/stream1` | Sí |
| CT3 (bala exterior) | Exterior | 1920x1080 | `rtsp://IP:554/stream1` | Sí |
| CT6 (exterior 2K) | Exterior | 2304x1296 | `rtsp://IP:554/stream1` | Sí |
| CT7 (exterior 4MP) | Exterior | 2560x1440 | `rtsp://IP:554/stream1` | Sí |
| IT6 (interior) | Interior | 1920x1080 | `rtsp://IP:554/stream1` | Sí |
| IT7 (interior 2K) | Interior | 2304x1296 | `rtsp://IP:554/stream1` | Sí |

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/stream1` | Flujo principal (recomendado) |
| `rtsp://IP:554/stream2` | Subflujo |
| `rtsp://IP:554/live/ch0` | Formato alternativo (algunos modelos) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Compatible con Dahua (algún firmware OEM) |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Tenda con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Tenda CP7 (4MP pan/tilt), main stream
var uri = new Uri("rtsp://192.168.1.90:554/stream1");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `/stream2` en lugar de `/stream1`.

## URLs de Captura

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación básica |

## Solución de Problemas

### La cámara requiere configuración previa con la aplicación

Las cámaras Tenda deben configurarse inicialmente a través de la aplicación Tenda Security. La cámara necesita credenciales WiFi y configuración de cuenta antes de que RTSP sea accesible. Después de la configuración, puede conectarse directamente vía RTSP en la red local.

### Múltiples formatos de URL

Algunas cámaras Tenda usan diferentes bases de firmware. Si `/stream1` no funciona, pruebe:

1. `rtsp://IP:554/live/ch0` (formato alternativo)
2. `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` (compatible con Dahua)
3. Use el descubrimiento ONVIF para recuperar la URL correcta automáticamente

### Encontrar la IP de la cámara

Las cámaras WiFi Tenda obtienen su IP vía DHCP. Encuéntrela en:

1. La aplicación Tenda Security (Información del dispositivo)
2. La lista de clientes DHCP de su router
3. Descubrimiento ONVIF (si es compatible)

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Tenda?**

La mayoría de las cámaras Tenda usan `rtsp://admin:password@CAMERA_IP:554/stream1` para el flujo principal y `/stream2` para el subflujo. Algunos modelos usan rutas de URL alternativas.

**¿Las cámaras Tenda soportan ONVIF?**

Los modelos más recientes de cámaras Tenda soportan ONVIF para descubrimiento y transmisión estandarizados. Los modelos más antiguos o económicos pueden no soportarlo. Verifique las especificaciones de su cámara en la aplicación Tenda Security.

**¿Las cámaras Tenda son buenas para integración en desarrollo?**

Las cámaras Tenda ofrecen precios competitivos y soporte RTSP estándar, haciéndolas adecuadas para desarrollo y prototipado. Para implementaciones en producción que requieran compatibilidad garantizada con RTSP/ONVIF, considere marcas de vigilancia establecidas como [Hikvision](hikvision.md), [Dahua](dahua.md) o [Reolink](reolink.md).

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión TP-Link](tp-link.md) — Segmento de consumo similar
- [Guía de Conexión Mercusys](mercusys.md) — Alternativa de cámaras económicas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
