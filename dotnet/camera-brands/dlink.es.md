---
title: D-Link DCS - URLs RTSP de Cámaras IP y Código en C# .NET
description: Conecta cámaras D-Link DCS en C# .NET con patrones de URL RTSP y ejemplos de código para DCS-930, DCS-2130, DCS-5222 y otros modelos DCS.
---

# Cómo Conectar una Cámara IP D-Link en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**D-Link Corporation** es un fabricante taiwanés de equipos de red con sede en Taipei. D-Link produce cámaras IP bajo la línea de productos **DCS (D-Link Cloud Security)**, dirigida a mercados de consumo y pequeñas empresas. Las cámaras D-Link están ampliamente disponibles a través de canales minoristas y son populares para seguridad del hogar y despliegues en oficinas pequeñas.

**Datos clave:**

- **Líneas de producto:** DCS-930/932/933/934 (consumo Wi-Fi), DCS-2130/2132/2230/2310/2330/2332 (prosumidor), DCS-5020/5222/5615 (PTZ), DCS-6010/6113/6818 (empresarial), DCS-7010/7110/7410 (exterior profesional)
- **Soporte de protocolos:** RTSP, ONVIF (algunos modelos), HTTP/CGI, MJPEG, nube mydlink de D-Link
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (contraseña vacía); algunos modelos: admin / admin
- **Soporte ONVIF:** Solo modelos seleccionados (típicamente DCS-2xxx y superiores)
- **Códecs de video:** H.264, MJPEG, MPEG-4 (antiguos)

## Patrones de URL RTSP

### Modelos Actuales y Recientes

Las cámaras D-Link usan el formato de URL `live.sdp` o `play.sdp`:

| Flujo | URL RTSP | Calidad | Notas |
|--------|----------|---------|-------|
| Flujo principal (H.264) | `rtsp://IP:554/live1.sdp` | Alta | Flujo principal H.264 |
| Subflujo (H.264) | `rtsp://IP:554/live2.sdp` | Media | Segundo flujo |
| Tercer flujo | `rtsp://IP:554/live3.sdp` | Baja | Tercer flujo (algunos modelos) |
| Flujo principal (alt) | `rtsp://IP:554/play1.sdp` | Alta | URL alternativa |
| Subflujo (alt) | `rtsp://IP:554/play2.sdp` | Media | URL alternativa |

### URLs Específicas por Modelo

| Modelo | URL RTSP | Resolución | Tipo |
|-------|----------|------------|------|
| DCS-930L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumo Wi-Fi |
| DCS-932L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumo Wi-Fi IR |
| DCS-933L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumo Wi-Fi |
| DCS-934L | `rtsp://IP:554/play1.sdp` | 1280x720 | Consumo HD |
| DCS-942L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumo IR |
| DCS-2100+ | `rtsp://IP:554/live.sdp` | 640x480 | Antigua |
| DCS-2121 | `rtsp://IP:554/play1.sdp` | 640x480 | Prosumidor |
| DCS-2130 | `rtsp://IP:554//live1.sdp` | 1280x720 | Prosumidor HD |
| DCS-2132L | `rtsp://IP:554//live1.sdp` | 1280x720 | Prosumidor HD |
| DCS-2230 | `rtsp://IP:554//live1.sdp` | 1920x1080 | Prosumidor FHD |
| DCS-2310L | `rtsp://IP:554/live1.sdp` | 1280x720 | Exterior HD |
| DCS-2332L | `rtsp://IP:554//live1.sdp` | 1280x720 | Exterior HD |
| DCS-5020L | `rtsp://IP:554/play1.sdp` | 640x480 | PTZ consumo |
| DCS-5222L | `rtsp://IP:554//live1.sdp` | 1280x720 | PTZ HD |
| DCS-6010L | `rtsp://IP:554/live1.sdp` | 1600x1200 | Panorámica |
| DCS-6113 | `rtsp://IP:554/live1.sdp` | 1920x1080 | Cámara de caja |
| DCS-6818 | `rtsp://IP:554/live3.sdp` | 1920x1080 | Empresarial |
| DCS-7010L | `rtsp://IP:554/live1.sdp` | 1280x720 | Exterior PoE |
| DCS-7110 | `rtsp://IP:554/live1.sdp` | 1280x800 | Exterior HD |
| DCS-7410 | `rtsp://IP:554/live1.sdp` | 1280x720 | Exterior empresarial |

!!! info "Doble barra en algunas URLs"
    Algunos modelos D-Link usan una doble barra antes de la ruta: `rtsp://IP:554//live1.sdp`. Esto es común en los modelos DCS-2130, DCS-2132L, DCS-2230, DCS-2332L y DCS-5222L. Intente tanto la barra simple como la doble si un formato no funciona.

### Modelos Anteriores (Solo HTTP)

Las cámaras D-Link DCS muy antiguas solo soportan HTTP:

| Modelo | URL | Notas |
|-------|-----|-------|
| DCS-900 | `http://IP/cgi-bin/video.jpg` | Solo JPEG |
| DCS-910 | `http://IP/video.cgi` | MJPEG |
| DCS-920 | `http://IP/video.cgi` | MJPEG |
| DCS-2100 | `http://IP/cgi-bin/video.jpg?size=2` | Solo JPEG |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara D-Link con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// D-Link DCS camera, main stream
var uri = new Uri("rtsp://192.168.1.45:554/live1.sdp");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `/live2.sdp` en su lugar.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/image/jpeg.cgi` | La mayoría de modelos DCS actuales |
| Flujo MJPEG | `http://IP/video/mjpg.cgi` | MJPEG continuo |
| MJPEG (alt) | `http://IP/video.cgi` | Modelos antiguos |
| MJPEG (auth) | `http://IP/mjpeg.cgi?user=USER&password=PASS&channel=1` | Con autenticación |
| Captura DMS | `http://IP/dms.jpg` | DCS-2130/2132/2230/2310/2332 |
| Flujo DMS | `http://IP/dms?nowprofileid=2` | Basado en perfil |
| Flujo ipcam | `http://IP/ipcam/stream.cgi?nowprofileid=2` | Algunos modelos |
| JPEG antigua | `http://IP/cgi-bin/video.jpg` | Modelos DCS muy antiguos |

## Solución de Problemas

### Formato de URL live vs play

Las cámaras D-Link usan dos convenciones de nombres de URL:

- **Modelos actuales (DCS-2xxx+):** `live1.sdp`, `live2.sdp`, `live3.sdp`
- **Modelos de consumo (DCS-930/932/933/942):** `play1.sdp`, `play2.sdp`, `play3.sdp`

Si un formato no funciona, intente el otro.

### La contraseña predeterminada está vacía

Muchas cámaras D-Link vienen con `admin` como nombre de usuario y una **contraseña vacía**. Puede necesitar configurar una contraseña a través de la interfaz web o el Asistente de Configuración de D-Link antes de que RTSP funcione correctamente.

### Cámaras en la nube mydlink

Algunas cámaras D-Link más nuevas están diseñadas principalmente para el ecosistema en la nube mydlink y pueden tener soporte RTSP limitado o inexistente. Verifique las especificaciones de la cámara para "RTSP" o soporte de "integración con terceros".

### Configuración de puerto

Las cámaras D-Link usan el puerto 554 por defecto para RTSP. La interfaz HTTP está típicamente en el puerto 80. Ambos pueden cambiarse en la interfaz web de la cámara en Configuración de Red.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras D-Link?**

Para la mayoría de cámaras D-Link DCS, intente `rtsp://admin:contraseña@IP_CAMARA:554/live1.sdp` o `rtsp://admin:contraseña@IP_CAMARA:554/play1.sdp`. El formato `live` es usado por modelos más nuevos, mientras que `play` es usado por modelos de consumo.

**¿Las cámaras D-Link soportan ONVIF?**

Modelos seleccionados soportan ONVIF (típicamente DCS-2xxx y modelos de gama alta). Las cámaras de consumo como la DCS-930L y DCS-932L generalmente no soportan ONVIF.

**¿Cuál es la diferencia entre live1.sdp y play1.sdp?**

Ambos sirven el mismo propósito (flujo de video principal) pero son usados por diferentes generaciones de cámaras D-Link. `live1.sdp` es más común en modelos prosumidor/profesionales más nuevos, mientras que `play1.sdp` es usado en modelos de consumo más antiguos.

**¿Puedo conectarme a cámaras D-Link sin la app mydlink?**

Sí. Las cámaras D-Link con soporte RTSP pueden accederse directamente vía su dirección IP sin el servicio en la nube mydlink. La nube mydlink es opcional para acceso remoto.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Foscam](foscam.md) — Par en cámaras IP de consumo
- [Guía de Conexión de TP-Link](tp-link.md) — Cámaras de consumo con RTSP
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
