---
title: Mercusys - URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecte cámaras Mercusys en C# .NET con patrones de URL RTSP y ejemplos de código para cámaras de seguridad interiores y exteriores de las series MC y MB.
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
  - C#

---

# Cómo Conectar una Cámara IP Mercusys en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Mercusys** es una marca de redes y hogar inteligente propiedad de **TP-Link**. Mercusys se dirige al segmento de presupuesto consciente con routers asequibles, sistemas mesh y cámaras de seguridad. Las cámaras Mercusys comparten similitudes de diseño y firmware con las cámaras TP-Link Tapo, ofreciendo soporte RTSP estándar a precios más bajos.

**Datos clave:**

- **Líneas de productos:** MC (cámaras interiores), MB (cámaras exteriores)
- **Soporte de protocolos:** RTSP, ONVIF (modelos selectos), HTTP
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** Se establecen durante la configuración en la aplicación (sin valores de fábrica)
- **Soporte ONVIF:** Sí (modelos más recientes, debe habilitarse)
- **Códecs de vídeo:** H.264
- **Empresa matriz:** TP-Link
- **Aplicación complementaria:** Aplicación Mercusys Security

!!! info "Mercusys y TP-Link Tapo"
    Las cámaras Mercusys comparten arquitectura de firmware con las cámaras TP-Link Tapo. El formato de URL RTSP (`/stream1`, `/stream2`) es similar. Si está familiarizado con la integración de Tapo, el mismo enfoque funciona con Mercusys. Consulte nuestra [guía de conexión TP-Link](tp-link.md) para detalles adicionales.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Mercusys utilizan una URL RTSP basada en número de flujo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/stream1
```

| Flujo | Patrón de URL | Descripción |
|-------|---------------|-------------|
| Flujo principal | `rtsp://IP:554/stream1` | Resolución completa |
| Subflujo | `rtsp://IP:554/stream2` | Menor resolución, menos ancho de banda |

### Modelos de Cámaras

| Modelo | Tipo | Resolución | URL de Flujo Principal | Audio |
|--------|------|-----------|----------------------|-------|
| MC50 (PT interior) | Panorámica/inclinación interior | 1920x1080 | `rtsp://IP:554/stream1` | Sí |
| MC60 (PT interior 2K) | Panorámica/inclinación interior | 2304x1296 | `rtsp://IP:554/stream1` | Sí |
| MC70 (PT interior 4MP) | Panorámica/inclinación interior | 2560x1440 | `rtsp://IP:554/stream1` | Sí |
| MB50 (bala exterior) | Exterior | 1920x1080 | `rtsp://IP:554/stream1` | Sí |
| MB60 (exterior 2K) | Exterior | 2304x1296 | `rtsp://IP:554/stream1` | Sí |
| MB70 (exterior 4MP) | Exterior | 2560x1440 | `rtsp://IP:554/stream1` | Sí |

### Habilitación de RTSP / ONVIF

Es posible que RTSP y ONVIF deban habilitarse en la configuración de la cámara:

1. Abra la aplicación **Mercusys Security**
2. Seleccione su cámara → **Configuración**
3. Navegue a **Configuración avanzada**
4. Habilite **RTSP** y/o **ONVIF**
5. Establezca un nombre de usuario y contraseña para el acceso RTSP

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Mercusys con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Mercusys MC70 (4MP indoor pan/tilt), main stream
var uri = new Uri("rtsp://192.168.1.90:554/stream1");
var username = "rtsp_user"; // set in Mercusys Security app
var password = "rtsp_pass";
```

Para acceso al subflujo, use `/stream2` en lugar de `/stream1`.

## URLs de Captura

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación básica |

## Solución de Problemas

### RTSP no accesible

Las cámaras Mercusys requieren configuración inicial a través de la aplicación Mercusys Security. Es posible que RTSP también deba habilitarse explícitamente en la configuración avanzada. Después de habilitar RTSP, las credenciales establecidas en la aplicación deben usarse para la autenticación RTSP.

### Descubrimiento de IP de la cámara

Encuentre la dirección IP de su cámara Mercusys en:

1. La aplicación Mercusys Security → Información del dispositivo
2. La lista de clientes DHCP de su router
3. Descubrimiento ONVIF (si está habilitado)

### Similar a TP-Link Tapo

Si la solución de problemas estándar de Mercusys no resuelve su problema, consulte nuestra [guía TP-Link Tapo](tp-link.md) para pasos adicionales de solución de problemas, ya que el firmware es similar.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Mercusys?**

Las cámaras Mercusys usan `rtsp://username:password@CAMERA_IP:554/stream1` para el flujo principal y `/stream2` para el subflujo. El nombre de usuario y contraseña se establecen durante la habilitación de RTSP en la aplicación Mercusys Security.

**¿Mercusys es lo mismo que TP-Link?**

Mercusys es una marca propiedad de TP-Link que se dirige al segmento de presupuesto. Las cámaras Mercusys comparten arquitectura de firmware con las cámaras TP-Link Tapo y usan formatos de URL RTSP similares.

**¿Las cámaras Mercusys soportan ONVIF?**

Los modelos más recientes de cámaras Mercusys soportan ONVIF, pero debe habilitarse a través de la aplicación Mercusys Security. Los modelos más antiguos pueden no incluir soporte ONVIF.

**¿Cómo se comparan las cámaras Mercusys con TP-Link Tapo?**

Las cámaras Mercusys se posicionan como una alternativa más asequible a Tapo. Comparten firmware y patrones de URL RTSP similares. Las cámaras Tapo generalmente tienen más opciones de modelos y mayor soporte de la comunidad.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión TP-Link](tp-link.md) — Empresa matriz, firmware similar
- [Guía de Conexión Tenda](tenda.md) — Alternativa de cámaras económicas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
