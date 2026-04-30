---
title: Cámara IP Zmodo RTSP: conexión y streaming en C# .NET
description: Conecte cámaras Zmodo en C# .NET con patrones de URL RTSP y ejemplos de código para modelos ZH Wi-Fi, ZP PoE, CM legacy y DVR/NVR.
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
  - H.264
  - MJPEG
  - C#

---

# Cómo conectar una cámara IP Zmodo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Zmodo Technology** es una marca de cámaras de seguridad de consumo con sede en Shenzhen, China. Zmodo es conocida por sus cámaras IP Wi-Fi y cableadas asequibles, sistemas DVR/NVR y productos de seguridad para el hogar inteligente. La marca se dirige al mercado de consumo económico y está ampliamente disponible a través de minoristas en línea.

**Datos clave:**

- **Líneas de productos:** ZH-IXx (cámaras Wi-Fi), ZP-IBH/IBI (cámaras PoE), CM-I (cámaras IP legacy), ZMD-ISV (sistemas DVR), Greet (timbre inteligente)
- **Soporte de protocolos:** RTSP, HTTP/MJPEG (legacy), Zmodo Zink (propietario), ONVIF (limitado, algunos modelos ZP)
- **Puerto RTSP predeterminado:** 10554 (cámaras Wi-Fi), 554 (modelos estándar/DVR)
- **Credenciales predeterminadas:** admin / admin o admin / (contraseña vacía)
- **Soporte ONVIF:** Limitado (solo algunos modelos más nuevos de la serie ZP PoE)
- **Códecs de vídeo:** H.264, MPEG-4 (DVR legacy)

!!! warning "Cámaras Zmodo Zink"
    Las cámaras Zmodo que usan el protocolo propietario **Zink** **no** soportan RTSP en absoluto. Estas cámaras solo pueden accederse a través de la app Zmodo. Verifique las especificaciones de su cámara antes de intentar conexiones RTSP.

## Patrones de URL RTSP

Las cámaras Zmodo usan diferentes puertos RTSP y formatos de URL dependiendo de la línea de productos.

### Cámaras Wi-Fi (Serie ZH) -- Puerto 10554

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:10554//tcp/av0_0
```

!!! warning "Puerto no estándar 10554"
    Las cámaras Wi-Fi Zmodo (serie ZH) usan el **puerto 10554**, no el estándar 554. Este es el problema de conexión más común con las cámaras Zmodo.

| Flujo | URL RTSP | Notas |
|-------|----------|-------|
| Flujo principal | `rtsp://IP:10554//tcp/av0_0` | Resolución completa |
| Subflujo | `rtsp://IP:10554//tcp/av0_1` | Resolución más baja |

### URLs Específicas por Modelo (Wi-Fi / PoE)

| Modelo | URL RTSP | Tipo |
|--------|----------|------|
| ZH-IXA15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi interior |
| ZH-IXB15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi interior |
| ZH-IXC15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi interior |
| ZH-IXD15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi interior |
| ZH-IBH13-W | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi bala |
| ZP-IBH13-P | `rtsp://IP:10554//tcp/av0_0` | PoE bala |
| ZP-IBI13-W | `rtsp://IP:10554//tcp/av0_0` | PoE interior |

### Cámaras H.264 Estándar -- Puerto 554

Algunas cámaras Zmodo usan el puerto RTSP estándar:

| Flujo | URL RTSP | Notas |
|-------|----------|-------|
| H.264 directo | `rtsp://IP:554/h264` | Puerto estándar |
| Flujo de canal | `rtsp://IP:554/VideoInput/1/h264/1` | Basado en canal |
| Número de canal | `rtsp://IP:554/[CHANNEL]` | Canal directo |

### Serie Legacy CM-I

| Modelo | URL RTSP | URL Alternativa | Notas |
|--------|----------|-----------------|-------|
| CM-I11123BK | `rtsp://IP:554/VideoInput/1/h264/1` | `http://IP/videostream.asf` | Alternativa HTTP |
| CM-I12316GY | `rtsp://IP:554/VideoInput/1/h264/1` | `http://IP/videostream.asf` | Alternativa HTTP |

### Sistemas DVR/NVR

| Modelo | URL RTSP | Notas |
|--------|----------|-------|
| ZMD-ISV-BFS23NM | `rtsp://IP:554/VideoInput/1/h264/1` | Canal 1 |
| DVR (MPEG-4) | `rtsp://IP:554/mpeg4` | Formato legacy |
| DVR (auth en URL) | `rtsp://IP:554/0/USERNAME:PASSWORD/main` | Auth en la ruta |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Zmodo con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Zmodo ZH-series Wi-Fi camera, main stream -- note port 10554!
var uri = new Uri("rtsp://192.168.1.60:10554//tcp/av0_0");
var username = "admin";
var password = "admin";
```

Para acceder al subflujo, use `//tcp/av0_1` en lugar de `//tcp/av0_0`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura | `http://IP/snapshot.cgi?user=USER&pwd=PASS` | Modelos estándar |
| Captura (cámara) | `http://IP/snapshot.cgi?camera=1` | Selección de cámara |
| Captura DVR | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Sistemas DVR |
| Flujo ASF | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=64&rate=0` | Legacy CM-I |
| Flujo MJPEG | `http://IP/videostream.cgi?rate=11` | Modelos legacy |

## Solución de Problemas

### Debe usar puerto 10554 para cámaras Wi-Fi

El problema de conexión más común con Zmodo es usar el puerto 554 cuando la cámara requiere el **puerto 10554**. Todas las cámaras Wi-Fi de la serie ZH y muchas cámaras PoE de la serie ZP usan el puerto 10554. Si su conexión expira en el puerto 554, cambie al 10554.

### Transporte TCP en la ruta de URL

La ruta `//tcp/av0_0` especifica explícitamente el transporte TCP. Esto está integrado en el formato de URL de Zmodo y no es opcional. No elimine el prefijo `//tcp/` de la ruta.

### Se requiere la app Zmodo para configuración inicial

Algunas cámaras Zmodo requieren la app móvil Zmodo para la configuración Wi-Fi inicial y activación. El acceso RTSP puede no estar disponible hasta que la cámara haya sido configurada a través de la app al menos una vez. Complete la configuración inicial antes de intentar conexiones RTSP.

### Las cámaras con protocolo Zink no soportan RTSP

Las cámaras Zmodo que usan el protocolo propietario **Zink** están diseñadas exclusivamente para el ecosistema Zmodo y no soportan RTSP, ONVIF ni ningún protocolo de transmisión de terceros. Verifique las especificaciones de la cámara o el empaque para la marca "Zink". Si su cámara usa Zink, no puede accederse vía RTSP.

### Las cámaras legacy CM-I usan transmisión HTTP

Las cámaras más antiguas de la serie CM-I pueden tener soporte RTSP limitado o poco confiable. Si RTSP falla en un modelo CM-I, recurra a las URLs de transmisión HTTP ASF o MJPEG: `http://IP/videostream.asf?user=USER&pwd=PASS`.

### Formato de autenticación DVR

Algunos DVR Zmodo incrustan credenciales en la ruta RTSP en lugar de usar autenticación RTSP estándar: `rtsp://IP:554/0/USERNAME:PASSWORD/main`. Si la autenticación estándar falla, pruebe este formato de URL.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Wi-Fi Zmodo?**

Para las cámaras Wi-Fi de la serie ZH, la URL es `rtsp://admin:admin@CAMERA_IP:10554//tcp/av0_0`. Note el puerto no estándar 10554 y el prefijo `//tcp/` en la ruta.

**¿Por qué mi cámara Zmodo usa el puerto 10554 en lugar del 554?**

Zmodo eligió el puerto 10554 para su línea de cámaras Wi-Fi. Este es un puerto fijo en el firmware de la cámara. Algunas cámaras Zmodo estándar (no Wi-Fi) y sistemas DVR usan el puerto estándar 554.

**¿Todas las cámaras Zmodo soportan RTSP?**

No. Las cámaras Zmodo que usan el protocolo propietario Zink no soportan RTSP. Estas cámaras están limitadas a la app Zmodo y servicio en la nube. La mayoría de las cámaras de las series ZH, ZP y CM-I sí soportan RTSP.

**¿Zmodo soporta ONVIF?**

El soporte ONVIF en las cámaras Zmodo es limitado. Algunos modelos más nuevos de la serie ZP PoE incluyen soporte ONVIF, pero la mayoría de los modelos Wi-Fi de consumo (serie ZH) no lo tienen. Verifique las especificaciones de su modelo específico para compatibilidad con ONVIF.

**¿Cuál es la diferencia entre av0_0 y av0_1?**

En la URL RTSP de Zmodo, `av0_0` es el flujo principal (mayor calidad) y `av0_1` es el subflujo (menor resolución). Use `av0_1` cuando necesite menor consumo de ancho de banda para visualización remota.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Foscam](foscam.md) — Cámaras IP de consumo económicas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
