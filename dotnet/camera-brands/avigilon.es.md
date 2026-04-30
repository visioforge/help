---
title: Cámara IP Avigilon RTSP en C# .NET - Guía completa
description: Conecte cámaras Avigilon en C# .NET con patrones de URL RTSP y ejemplos de código para modelos H5A, H5M, H5 Pro, H5SL y NVR Unity.
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
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.265
  - MJPEG
  - C#

---

# Cómo conectar una cámara IP Avigilon en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Avigilon** (Avigilon Corporation) es un fabricante de cámaras de seguridad empresarial originalmente con sede en Vancouver, Canadá. Fundada en 2004, Avigilon fue adquirida por **Motorola Solutions** en 2018 por aproximadamente $1 mil millones. La empresa es conocida por cámaras de alta resolución (hasta 61MP), analíticas de video con IA incluyendo Detección de Movimiento Inusual (UMD) y Búsqueda por Apariencia, y la tecnología propietaria HDSM (High Definition Stream Management). Las cámaras Avigilon están ampliamente desplegadas en entornos empresariales, gubernamentales y de infraestructura crítica.

**Datos clave:**

- **Líneas de productos:** H5A (bala/domo), H5M (mini domo), H5 Pro (multisensor), H5SL (línea económica), Unity (NVRs)
- **Líneas anteriores:** HD Pro, HD Multisensor, HD Micro Dome, HD PTZ
- **Soporte de protocolos:** RTSP, ONVIF (Profile S, Profile T), HTTP
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (debe cambiarse en la configuración inicial)
- **Soporte ONVIF:** Sí (Profile S, Profile T)
- **Códecs de video:** H.264, H.265, HDSM SmartCodec (basado en H.265)
- **Conocido por:** Analíticas de IA (Detección de Movimiento Inusual, Búsqueda por Apariencia), HDSM SmartCodec

!!! info "Avigilon ahora es parte de Motorola Solutions"
    Avigilon ahora es parte de Motorola Solutions. La línea de cámaras Avigilon continúa bajo la división de Seguridad de Video y Control de Acceso de Motorola Solutions. Consulte también nuestra [guía de Pelco](pelco.md) para otra marca de cámaras de Motorola Solutions.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Avigilon usan el patrón de URL `defaultPrimary` / `defaultSecondary` con un parámetro de tipo de flujo unicast:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/defaultPrimary?streamType=u
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `defaultPrimary` | Flujo principal | Flujo principal (resolución más alta) |
| `defaultSecondary` | Flujo secundario | Subflujo (resolución más baja, menos ancho de banda) |
| `streamType` | `u` | Entrega de flujo unicast |

### Modelos de Cámaras

| Serie de Modelo | Tipo | URL de Flujo Principal | Notas |
|-----------------|------|------------------------|-------|
| H5A Bullet | Bala fija | `rtsp://IP:554/defaultPrimary?streamType=u` | Con IA, hasta 8MP |
| H5A Dome | Domo fijo | `rtsp://IP:554/defaultPrimary?streamType=u` | Con IA, hasta 8MP |
| H5M Mini Dome | Mini domo | `rtsp://IP:554/defaultPrimary?streamType=u` | Factor de forma compacto |
| H5 Pro Multi-sensor | Multisensor | `rtsp://IP:554/defaultPrimary0?streamType=u` | Ver nota multisensor abajo |
| H5SL Bullet | Bala económica | `rtsp://IP:554/defaultPrimary?streamType=u` | Línea económica |
| H5SL Dome | Domo económico | `rtsp://IP:554/defaultPrimary?streamType=u` | Línea económica |
| HD Pro | Legacy alta resolución | `rtsp://IP:554/defaultPrimary?streamType=u` | Hasta 61MP |

### URLs de Cámaras Multisensor

Para cámaras Avigilon H5 Pro y otras multisensor, cada cabezal de sensor tiene su propio índice de flujo:

| Sensor | Flujo Principal | Subflujo |
|--------|-----------------|----------|
| Sensor 1 | `rtsp://IP:554/defaultPrimary0?streamType=u` | `rtsp://IP:554/defaultSecondary0?streamType=u` |
| Sensor 2 | `rtsp://IP:554/defaultPrimary1?streamType=u` | `rtsp://IP:554/defaultSecondary1?streamType=u` |
| Sensor 3 | `rtsp://IP:554/defaultPrimary2?streamType=u` | `rtsp://IP:554/defaultSecondary2?streamType=u` |
| Sensor 4 | `rtsp://IP:554/defaultPrimary3?streamType=u` | `rtsp://IP:554/defaultSecondary3?streamType=u` |

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/defaultPrimary?streamType=u` | Principal estándar (recomendado) |
| `rtsp://IP:554/defaultSecondary?streamType=u` | Flujo secundario / subflujo |
| `rtsp://IP:554/defaultPrimary0?streamType=u` | Flujo principal alternativo (también usado para sensor 1 multisensor) |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Avigilon con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Avigilon H5A dome, primary stream (unicast)
var uri = new Uri("rtsp://192.168.1.100:554/defaultPrimary?streamType=u");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `defaultSecondary` en lugar de `defaultPrimary`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/snapshot.jpg` | Requiere autenticación básica |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras Avigilon requieren que la contraseña predeterminada se cambie durante la configuración inicial. Si no ha configurado la cámara aún:

1. Acceda a la cámara en `http://CAMERA_IP` en un navegador
2. Complete el asistente de configuración inicial y establezca una contraseña fuerte
3. Use esas credenciales en su URL RTSP

### Flujos HDSM SmartCodec

El HDSM SmartCodec de Avigilon está basado en H.265. Asegúrese de que su decodificador soporte H.265 al conectarse a cámaras configuradas para usar HDSM SmartCodec. Si experimenta problemas de decodificación, intente cambiar la cámara a codificación H.264 estándar en la interfaz web de la cámara.

### Parámetro de tipo de flujo

El parámetro `streamType=u` solicita entrega unicast. Si omite este parámetro, la cámara puede predeterminar a multicast, lo cual puede causar problemas en redes no configuradas para enrutamiento multicast.

### Las cámaras multisensor muestran solo una vista

Para modelos multisensor (H5 Pro), cada sensor se accede como un flujo separado. Use `defaultPrimary0`, `defaultPrimary1`, etc. para acceder a cabezales de sensores individuales. Consulte la tabla de URLs multisensor arriba.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Avigilon?**

La URL es `rtsp://admin:password@CAMERA_IP:554/defaultPrimary?streamType=u` para el flujo principal. Use `defaultSecondary` en lugar de `defaultPrimary` para el subflujo de menor resolución.

**¿Las cámaras Avigilon soportan ONVIF?**

Sí. Las cámaras Avigilon soportan ONVIF Profile S y Profile T. ONVIF puede habilitarse a través de la interfaz web de la cámara o el software Avigilon Control Center (ACC).

**¿Qué es HDSM SmartCodec?**

HDSM (High Definition Stream Management) SmartCodec es la tecnología de compresión propietaria de Avigilon basada en H.265. Reduce los requisitos de ancho de banda y almacenamiento codificando inteligentemente diferentes regiones de la imagen a diferentes niveles de calidad mientras mantiene detalle en áreas de interés. Los flujos codificados con HDSM SmartCodec son compatibles con decodificadores H.265 estándar.

**¿Puedo usar cámaras Avigilon sin Avigilon Control Center?**

Sí. Aunque Avigilon Control Center (ACC) es el VMS recomendado, las cámaras exponen flujos RTSP estándar y soportan ONVIF, permitiendo integración con cualquier aplicación compatible con RTSP incluyendo los SDKs de VisioForge.

**¿Cómo accedo a sensores individuales en cámaras multisensor?**

Cada cabezal de sensor en una cámara multisensor (como la H5 Pro) tiene su propia URL de flujo. Use `defaultPrimary0` para el sensor 1, `defaultPrimary1` para el sensor 2, y así sucesivamente. Cada sensor también puede tener un flujo secundario accesible vía `defaultSecondary0`, `defaultSecondary1`, etc.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Pelco](pelco.md) — También de Motorola Solutions, cámaras empresariales
- [Captura ONVIF con Postprocesamiento](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Pipeline de captura ONVIF para Avigilon
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
