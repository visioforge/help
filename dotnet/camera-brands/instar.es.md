---
title: Cámaras IP INSTAR - URLs RTSP y conexión con C# .NET
description: Conecte cámaras IP INSTAR en C# .NET con patrones de URL RTSP y ejemplos de código para modelos HD y legacy IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx.
---

# Cómo conectar una cámara IP INSTAR en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**INSTAR** (INSTAR Deutschland GmbH) es un fabricante alemán de cámaras IP con sede en Hanau, Alemania. INSTAR se especializa en cámaras IP asequibles para interiores y exteriores para el mercado de consumo y pequeñas empresas, con una fuerte presencia en Europa, particularmente en Alemania. Las cámaras INSTAR son conocidas por sus opciones de almacenamiento local, integración MQTT con hogares inteligentes (Home Assistant, ioBroker, Node-RED) y configuración sencilla.

**Datos clave:**

- **Líneas de productos:** IN-2xxx/3xxx/4xxx (legacy VGA), IN-5xxx (720p), IN-6xxx (720p HD), IN-7xxx (1080p Full HD), IN-8xxx (actual 1080p+), IN-9xxx (actual 4K/WQHD)
- **Soporte de protocolos:** RTSP, HTTP, ONVIF (IN-6xxx y posteriores), MQTT
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / instar (varía según el modelo)
- **Soporte ONVIF:** Sí (series IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx)
- **Códecs de video:** H.265 (IN-9xxx), H.264 (IN-6xxx/7xxx/8xxx), MPEG-4 (IN-5xxx), MJPEG (legacy IN-2xxx/3xxx/4xxx)

## Patrones de URL RTSP

Las cámaras INSTAR usan un formato de URL distintivo con una **doble barra diagonal** antes del número de flujo.

### Formato de URL (Modelos HD)

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//11
```

!!! warning "Doble barra diagonal"
    Las cámaras HD INSTAR usan una **doble barra diagonal** (`//`) antes del número de flujo. Usar una sola barra resultará en un fallo de conexión.

### Modelos HD (IN-6xxx / IN-7xxx / IN-8xxx / IN-9xxx)

| Flujo | URL RTSP | Resolución | Notas |
|-------|----------|------------|-------|
| Flujo principal | `rtsp://IP:554//11` | Resolución completa | H.264 / H.265 |
| Subflujo | `rtsp://IP:554//12` | Resolución más baja | Optimizado para ancho de banda |
| Tercer flujo | `rtsp://IP:554//13` | Optimizado para móvil | Resolución más baja |

### URLs Específicas por Modelo

| Modelo | URL RTSP | Resolución | Tipo |
|--------|----------|------------|------|
| IN-6012 HD | `rtsp://IP:554//11` | 720p | Interior pan/tilt |
| IN-6014 HD | `rtsp://IP:554//11` | 720p | Interior |
| IN-7011 HD | `rtsp://IP:554//11` | 1080p | Interior pan/tilt |
| IN-8015 Full HD | `rtsp://IP:554//11` | 1080p | Interior/Exterior |
| IN-9008 Full HD | `rtsp://IP:554//11` | 1080p+ | Exterior PoE |
| IN-9020 Full HD | `rtsp://IP:554//11` | WQHD/4K | Exterior PoE |

### Modelos 720p Anteriores (IN-5xxx -- MPEG-4)

Las cámaras IN-5xxx usan una ruta RTSP diferente con codificación MPEG-4:

| Modelo | URL RTSP | Resolución | Notas |
|--------|----------|------------|-------|
| IN-5905 HD | `rtsp://IP:554/MediaInput/mpeg4` | 720p | Exterior |
| IN-5907 HD | `rtsp://IP:554/MediaInput/mpeg4` | 720p | Exterior |

### Modelos Anteriores (IN-2xxx / IN-3xxx / IN-4xxx -- Solo HTTP)

Las cámaras INSTAR legacy de resolución VGA no soportan RTSP. Solo usan transmisión basada en HTTP:

| Serie de Modelo | URL HTTP | Tipo | Notas |
|-----------------|----------|------|-------|
| IN-2xxx/3xxx/4xxx | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=32&rate=0` | Flujo ASF | Resolución VGA |
| IN-2xxx/3xxx/4xxx | `http://IP/videostream.cgi?rate=11` | MJPEG | Sin audio |
| IN-2xxx/3xxx/4xxx | `http://IP//iphone/11?USER:PASS&` | Flujo móvil | Compatible con iPhone |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara INSTAR con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// INSTAR IN-8015 Full HD, main stream -- note the double forward slash!
var uri = new Uri("rtsp://192.168.1.50:554//11");
var username = "admin";
var password = "instar";
```

Para acceder al subflujo, use `//12` en lugar de `//11`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura (HD) | `http://IP/tmpfs/auto.jpg` | IN-6xxx/7xxx/8xxx/9xxx |
| Captura (HD, auth) | `http://IP/snap.jpg?usr=USER&pwd=PASS` | Con credenciales |
| Captura (legacy) | `http://IP/snapshot.cgi` | IN-2xxx/3xxx/4xxx |
| Captura (legacy, auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Legacy con credenciales |
| Flujo ASF (legacy) | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=32&rate=0` | VGA ASF |
| Flujo MJPEG (legacy) | `http://IP/videostream.cgi?rate=11` | MJPEG legacy |

## Solución de Problemas

### Se requiere doble barra diagonal

El problema de conexión más común con INSTAR es olvidar la **doble barra diagonal** antes del número de flujo. La URL correcta es `rtsp://IP:554//11` (dos barras), no `rtsp://IP:554/11` (una barra).

### Las cámaras legacy no tienen soporte RTSP

Las cámaras IN-2xxx, IN-3xxx e IN-4xxx son solo HTTP. No soportan RTSP en absoluto. Use las URLs de transmisión ASF o MJPEG por HTTP para estos modelos.

### IN-5xxx usa una ruta RTSP diferente

Las cámaras IN-5xxx usan `rtsp://IP:554/MediaInput/mpeg4` en lugar de la ruta `//11` usada por los modelos HD más nuevos. Si la URL `//11` falla en una cámara INSTAR de 720p, verifique si su modelo es de la serie IN-5xxx.

### Integración MQTT y hogar inteligente

Las cámaras INSTAR soportan MQTT para integración con Home Assistant, ioBroker y Node-RED. MQTT se usa para control de cámara y notificaciones de eventos, no para transmisión de video. Para integración de video con plataformas de hogar inteligente, use la URL RTSP.

### Disponibilidad de PoE

Los modelos de exterior IN-8xxx e IN-9xxx soportan Power over Ethernet (PoE), permitiendo un solo cable para energía y datos. Los modelos de interior típicamente requieren un adaptador de energía separado.

### Las credenciales varían según el modelo

Aunque las credenciales predeterminadas comunes son admin / instar, algunos modelos pueden usar valores predeterminados diferentes. Consulte la documentación o etiqueta de la cámara para las credenciales de fábrica. Las cámaras INSTAR típicamente requieren cambiar la contraseña predeterminada durante la configuración inicial.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras INSTAR?**

Para los modelos HD actuales (IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx), la URL es `rtsp://admin:instar@CAMERA_IP:554//11`. Note la doble barra diagonal antes de `11`. Para modelos IN-5xxx, use `rtsp://admin:instar@CAMERA_IP:554/MediaInput/mpeg4`.

**¿Todas las cámaras INSTAR soportan RTSP?**

No. Los modelos legacy (IN-2xxx, IN-3xxx, IN-4xxx) son cámaras de resolución VGA que solo soportan transmisión basada en HTTP en formato ASF o MJPEG. Todas las cámaras IN-5xxx y posteriores soportan RTSP.

**¿Cuál es la diferencia entre los flujos //11, //12 y //13?**

El flujo `//11` es el principal (mayor calidad), `//12` es un subflujo de menor resolución adecuado para visualización remota con ancho de banda limitado, y `//13` es un tercer flujo optimizado para móvil con la resolución más baja.

**¿Las cámaras INSTAR soportan ONVIF?**

Sí. ONVIF es compatible con las cámaras de las series IN-6xxx, IN-7xxx, IN-8xxx e IN-9xxx. Los modelos legacy no soportan ONVIF. Puede usar las funciones ONVIF del VisioForge SDK para descubrimiento de cámaras y control PTZ en los modelos compatibles.

**¿Puedo integrar cámaras INSTAR con Home Assistant?**

Sí. Las cámaras INSTAR soportan MQTT, lo que facilita su integración con Home Assistant, ioBroker y Node-RED para automatización y acciones basadas en eventos. Para transmisión de video en Home Assistant, use la URL RTSP en una integración de cámara genérica.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de ABUS](abus.md) — Cámaras alemanas de consumo / hogar inteligente
- [Guardar Flujo RTSP Original](../mediablocks/Guides/rtsp-save-original-stream.md) — Grabar flujos INSTAR sin recodificación
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
