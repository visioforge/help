---
title: AVTech: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecte cámaras AVTech en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series AVM, AVN, AVC y AVI.
---

# Cómo conectar una cámara IP AVTech en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**AVTech** (AVTech Corporation) es un fabricante taiwanés de equipos de vigilancia con sede en Taipéi, Taiwán, fundado en 1996. AVTech es uno de los mayores fabricantes de DVR/NVR a nivel global, con una fuerte presencia en los mercados de Asia-Pacífico, Medio Oriente y América Latina. La empresa produce una amplia gama de cámaras IP, DVRs, NVRs y la plataforma de visualización móvil EagleEyes. AVTech es conocido por ofrecer soluciones de vigilancia económicas con amplia compatibilidad de modelos.

**Datos clave:**

- **Líneas de productos:** AVM (cámaras IP), AVN (cámaras de red), AVC (DVRs), AVI (cámaras especiales), EagleEyes (app móvil)
- **Soporte de protocolos:** RTSP, ONVIF (modelos más nuevos), HTTP/CGI, MJPEG
- **Puerto RTSP predeterminado:** 554 (algunos modelos usan puerto 88)
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (modelos más nuevos)
- **Códecs de video:** H.264, MPEG-4, MJPEG
- **Acceso de invitado:** Muchos modelos permiten capturas JPEG sin autenticación vía CGI de invitado

!!! note "Algunos modelos AVTech usan puerto 88"
    Algunos modelos AVTech más nuevos usan el puerto 88 en lugar del 554 para RTSP. Si el puerto 554 no funciona, pruebe el puerto 88 con el patrón de URL `rtsp://IP:88//live/h264_ulaw/VGA`.

!!! warning "Seguridad del acceso de invitado"
    Muchas cámaras AVTech exponen un endpoint CGI de invitado (`/cgi-bin/guest/Video.cgi`) que permite acceso a capturas sin autenticación. Asegúrese de que la configuración de acceso de invitado de su cámara esté configurada de forma segura.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras AVTech usan el patrón de URL basado en ruta `/live/`:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live/h264
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `/live/h264` | Flujo H.264 | Flujo de video H.264 principal |
| `/live/mpeg4` | Flujo MPEG-4 | Flujo de video MPEG-4 legacy |
| `/live/h264/ch[N]` | Canal N | Flujo específico de canal para DVRs/NVRs |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|------------------------|-------|
| AVM217 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM328 | IP domo | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM357 | IP domo | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM457 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM459 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM552 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM561 | IP domo | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVM571 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN211 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN252 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN257 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN304 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN314 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN362 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN801 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN812 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVN813 | Cámara de red | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVI201 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |
| AVI203 | Cámara IP | `rtsp://IP:554/live/h264` | Flujo principal H.264 |

### URLs de Canal DVR/NVR

Para DVRs y NVRs AVTech (serie AVC y otros):

| Canal | Flujo Principal (H.264) | Flujo Principal (MPEG-4) |
|-------|--------------------------|--------------------------|
| Canal 1 | `rtsp://IP:554/live/h264/ch1` | `rtsp://IP:554/live/mpeg4/ch1` |
| Canal 2 | `rtsp://IP:554/live/h264/ch2` | `rtsp://IP:554/live/mpeg4/ch2` |
| Canal 3 | `rtsp://IP:554/live/h264/ch3` | `rtsp://IP:554/live/mpeg4/ch3` |
| Canal N | `rtsp://IP:554/live/h264/chN` | `rtsp://IP:554/live/mpeg4/chN` |

### Formatos de URL Alternativos

Algunos modelos AVTech, particularmente los más nuevos, usan el puerto 88 y diferentes formatos de ruta:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/live/h264` | H.264 estándar (recomendado) |
| `rtsp://IP:554/live/mpeg4` | Flujo MPEG-4 |
| `rtsp://IP//live/h264` | Sin puerto explícito (algunos modelos) |
| `rtsp://IP:88//live/h264_ulaw/VGA` | Puerto 88, con audio, resolución VGA |
| `rtsp://IP:88//live/video_audio/profile1` | Puerto 88 con selección de perfil |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara AVTech con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// AVTech AVM552, H.264 main stream
var uri = new Uri("rtsp://192.168.1.80:554/live/h264");
var username = "admin";
var password = "YourPassword";
```

Para acceder al flujo MPEG-4, use `/live/mpeg4` en lugar de `/live/h264`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG (invitado) | `http://IP/cgi-bin/guest/Video.cgi?media=JPEG` | Sin autenticación requerida (si el acceso de invitado está habilitado) |
| Captura JPEG (canal) | `http://IP/cgi-bin/guest/Video.cgi?media=JPEG&channel=CHANNEL` | Captura específica de canal |
| Flujo en Vivo MJPEG | `http://IP/live/mjpeg` | Flujo MJPEG continuo |

## Solución de Problemas

### Error "401 Unauthorized"

Si recibe un error de autenticación:

1. Verifique sus credenciales - el valor predeterminado es admin / admin
2. Acceda a la cámara en `http://CAMERA_IP` en un navegador para confirmar que el inicio de sesión funciona
3. Asegúrese de que RTSP esté habilitado en la configuración de red de la cámara
4. Intente incluir credenciales en la URL: `rtsp://admin:password@IP:554/live/h264`

### Puerto 554 vs puerto 88

Algunos modelos AVTech más nuevos usan el puerto 88 en lugar del puerto RTSP estándar 554. Si no puede conectarse en el puerto 554:

1. Pruebe el puerto 88: `rtsp://IP:88//live/h264_ulaw/VGA`
2. Note la doble barra (`//`) en algunos patrones de URL del puerto 88
3. Verifique la interfaz web de la cámara bajo configuración de red para el puerto RTSP configurado

### MPEG-4 vs H.264

Los modelos AVTech más antiguos pueden solo soportar MPEG-4. Si la URL de flujo H.264 no funciona:

- Pruebe `rtsp://IP:554/live/mpeg4` en su lugar
- Verifique la configuración de codificación de la cámara en la interfaz web
- Los modelos más nuevos soportan H.264; los modelos más antiguos pueden ser solo MPEG-4

### Doble barra en la URL

Algunos patrones de URL de AVTech incluyen una doble barra (`//`) después de la IP o puerto. Esto es intencional y requerido por ciertas versiones de firmware. Si una URL con una sola barra no funciona, pruebe la variante con doble barra.

### App móvil EagleEyes

La app EagleEyes es la plataforma de visualización móvil de AVTech. El acceso RTSP funciona independientemente de EagleEyes y no requiere que la app esté configurada.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras AVTech?**

La URL es `rtsp://admin:password@CAMERA_IP:554/live/h264` para el flujo principal H.264. Para DVRs/NVRs, agregue el número de canal: `rtsp://IP:554/live/h264/ch1` para el canal 1.

**¿Las cámaras AVTech soportan ONVIF?**

Los modelos AVTech más nuevos soportan ONVIF. Los modelos más antiguos pueden no tener soporte ONVIF y dependen de protocolos propietarios y RTSP para integración.

**¿Cuál es la diferencia entre las series AVM y AVN?**

La serie AVM son cámaras IP diseñadas para conexión directa a red, mientras que la serie AVN son cámaras de red que pueden incluir funciones adicionales como Wi-Fi integrado o audio. Ambas series usan el mismo formato de URL RTSP.

**¿Puedo acceder a capturas de AVTech sin autenticación?**

Muchas cámaras AVTech tienen un endpoint CGI de invitado (`/cgi-bin/guest/Video.cgi?media=JPEG`) que permite acceso a capturas JPEG sin autenticación. Esto es una preocupación de seguridad si su cámara es accesible en la red. Verifique la configuración de acceso de invitado de su cámara y deshabilite el acceso de invitado si no es necesario.

**¿Por qué algunas URLs de AVTech usan el puerto 88?**

Algunas versiones de firmware AVTech más nuevas tienen como predeterminado el puerto 88 para RTSP en lugar del puerto estándar 554. Si no puede conectarse en el puerto 554, pruebe el puerto 88. La configuración del puerto típicamente puede verificarse y cambiarse en la interfaz web de la cámara bajo configuración de red.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de LILIN](lilin.md) — Cámaras industriales taiwanesas
- [Guía de Conexión de BrickCom](brickcom.md) — Cámaras industriales taiwanesas
- [Integración de Cámara IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Descubrimiento de dispositivos ONVIF AVTech
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
