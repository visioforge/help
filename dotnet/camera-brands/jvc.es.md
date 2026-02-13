---
title: Cómo conectar una cámara IP JVC en C# .NET
description: Conecte cámaras JVC en C# .NET con patrones de URL RTSP y ejemplos de código para cámaras de red de las series VN-H, VN-T, VN-C y VN-X.
---

# Cómo conectar una cámara IP JVC en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**JVC** (JVCKENWOOD Corporation) es una empresa de electrónica japonesa con sede en Yokohama, Japón. La División de Sistemas Profesionales de JVC produjo las cámaras IP de la serie VN para aplicaciones de vigilancia. JVC salió del mercado de cámaras IP independientes alrededor de 2015, pero muchas cámaras de la serie VN permanecen en servicio activo en instalaciones empresariales y gubernamentales. Estas cámaras son conocidas por su robusto soporte del protocolo PSIA y rendimiento confiable.

**Datos clave:**

- **Líneas de productos:** VN-H Series (VN-H37, VN-H137, VN-H237, VN-H657), VN-T Series (VN-T216U), VN-X Series (VN-X35U, VN-X235U), VN-C Series (VN-C20U)
- **Soporte de protocolos:** RTSP, ONVIF (series VN-H/VN-T), PSIA, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / jvc (varía según el modelo)
- **Soporte ONVIF:** Sí (series VN-H y VN-T)
- **Códecs de video:** H.264 (series VN-H/VN-T), MPEG-4 (modelos VN-C más antiguos)

!!! warning "Línea de Productos Descontinuada"
    JVC salió del mercado de cámaras IP alrededor de 2015. Aunque las cámaras de la serie VN siguen ampliamente desplegadas, las actualizaciones de firmware ya no están disponibles. Considere la segmentación de red y reglas de firewall para proteger estas cámaras, ya que no recibirán parches de seguridad.

## Patrones de URL RTSP

### Formatos de URL Estándar

Las cámaras JVC soportan múltiples patrones de URL RTSP dependiendo de la serie del modelo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/PSIA/Streaming/channels/0?videoCodecType=H.264
```

| Patrón de URL | Protocolo | Descripción |
|---------------|-----------|-------------|
| `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA | Flujo H.264 principal (mayoría de modelos VN-H) |
| `rtsp://IP:554/PSIA/Streaming/channels/CHANNEL` | PSIA | Flujo PSIA por número de canal |
| `rtsp://IP:554/video.h264` | H.264 | Flujo H.264 directo (serie VN general) |
| `rtsp://IP:554/1/stream1` | H.264 | URL de flujo alternativa (VN-T216U) |
| `rtsp://IP:554//livestream` | H.264 | URL de flujo en vivo (VN-H57) |

!!! note "Numeración de Canales PSIA"
    Las cámaras JVC usan numeración de canales basada en cero para URLs PSIA. El canal 0 es el primer (y generalmente único) canal de video. Esto difiere de la mayoría de otras marcas que comienzan la numeración de canales en 1.

### URLs de Canal PSIA

| Canal | URL | Descripción |
|-------|-----|-------------|
| Canal 0 (principal) | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | Primer canal de video (flujo principal) |
| Canal 1 | `rtsp://IP:554/PSIA/Streaming/channels/1?videoCodecType=H.264` | Segundo canal de video (subflujo, si disponible) |

### Modelos de Cámaras

| Serie de Modelo | Resolución | URL de Flujo Principal | Códec |
|-----------------|------------|------------------------|-------|
| VN-H37 (domo HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H137 (bala HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H237 (domo HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H657 (PTZ HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-T216U (caja HD) | 1920x1080 | `rtsp://IP:554/1/stream1` | H.264 |
| VN-X35U (cámara de red) | 1280x960 | `rtsp://IP:554/video.h264` | H.264 |
| VN-X235U (cámara de red) | 1920x1080 | `rtsp://IP:554/video.h264` | H.264 |
| VN-C20U (red legacy) | 640x480 | N/A (solo captura HTTP) | MJPEG |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara JVC con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// JVC VN-H Series, PSIA H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/PSIA/Streaming/channels/0?videoCodecType=H.264");
var username = "admin";
var password = "jvc";
```

Para cámaras de la serie VN-T usando el formato de URL alternativo:

```csharp
// JVC VN-T216U, alternative stream URL
var uri = new Uri("rtsp://192.168.1.90:554/1/stream1");
```

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/video.jpg` | Captura estándar (mayoría de modelos) |
| Captura Java Applet | `http://IP/java.jpg` | Captura basada en Java (legacy) |
| Captura API | `http://IP/api/video?encode=jpeg` | Captura JPEG basada en API (serie VN-X) |
| Flujo MJPEG | `http://IP/api/video?encode=jpeg&framerate=15&boundary=on` | MJPEG continuo vía API |

### URLs de Captura Específicas por Modelo

| Serie de Modelo | URL de Captura | Notas |
|-----------------|----------------|-------|
| VN-H Series | `http://IP/cgi-bin/video.jpg` | Captura basada en CGI |
| VN-T Series | `http://IP/cgi-bin/video.jpg` | Captura basada en CGI |
| VN-C Series (VN-C20U) | `http://IP/cgi-bin/video.jpg` | Captura basada en CGI |
| VN-X Series (VN-X35U/X235U) | `http://IP/api/video?encode=jpeg` | Captura basada en API |

## Solución de Problemas

### La numeración de canales PSIA comienza en 0

A diferencia de la mayoría de marcas de cámaras donde la numeración de canales comienza en 1, JVC usa numeración de canales PSIA **basada en cero**. Si está portando código de otra marca:

- Canal 0 = Primer canal de video (equivalente al Canal 1 en otras marcas)
- Canal 1 = Segundo canal de video (subflujo o sensor secundario)

### Las credenciales predeterminadas no funcionan

Las cámaras JVC se envían con diferentes credenciales predeterminadas dependiendo del modelo y versión de firmware:

1. Pruebe `admin` / `jvc` (más común)
2. Pruebe `admin` / `admin`
3. Intente acceder a la interfaz web en `http://CAMERA_IP` para restablecer o verificar credenciales
4. Algunos modelos se envían sin contraseña predeterminada - acceda primero a la interfaz web para establecer una

### Actualizaciones de firmware no disponibles

Dado que JVC descontinuó su línea de cámaras IP alrededor de 2015, las actualizaciones de firmware ya no están disponibles. Para mitigar riesgos de seguridad:

- Coloque las cámaras en una VLAN o segmento de red aislado
- Use reglas de firewall para restringir el acceso a los puertos de la cámara
- Deshabilite UPnP y cualquier función de conectividad en la nube
- Considere reemplazar las cámaras al final de su vida útil con modelos actualmente soportados

### Acceso solo HTTP para la serie VN-C

Las cámaras más antiguas de la serie VN-C (como VN-C20U) no soportan transmisión RTSP y solo proporcionan acceso MJPEG basado en HTTP. Use las URLs de captura HTTP o flujo MJPEG para estos modelos en lugar de RTSP.

### Múltiples formatos de URL en la serie VN-T

La VN-T216U soporta múltiples formatos de URL RTSP. Si uno no funciona, pruebe alternativas:

1. `rtsp://IP:554/1/stream1` (preferido)
2. `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` (PSIA)
3. `rtsp://IP:554/video.h264` (H.264 directo)

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras JVC?**

Para la mayoría de cámaras de la serie VN-H, la URL RTSP principal es `rtsp://admin:jvc@CAMERA_IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264`. La serie VN-T usa `rtsp://IP:554/1/stream1` como alternativa. Los modelos de la serie VN-X usan `rtsp://IP:554/video.h264`.

**¿Las cámaras IP JVC siguen teniendo soporte?**

JVC salió del mercado de cámaras IP independientes alrededor de 2015. Las cámaras siguen siendo funcionales pero ya no reciben actualizaciones de firmware ni soporte oficial. Muchas cámaras de la serie VN siguen activamente desplegadas en sistemas de vigilancia en todo el mundo.

**¿Las cámaras JVC soportan ONVIF?**

Las cámaras de las series VN-H y VN-T soportan ONVIF Profile S. Los modelos más antiguos VN-C y algunos VN-X no soportan ONVIF y dependen de interfaces PSIA o CGI propietarias.

**¿Por qué la numeración de canales PSIA comienza en 0?**

JVC implementa numeración de canales PSIA basada en cero, lo que significa que el primer canal de video es el canal 0 en lugar del canal 1. Esto es específico de la implementación PSIA de JVC y difiere de la mayoría de otros fabricantes de cámaras. Al migrar desde otra marca, ajuste sus números de canal en consecuencia.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Sony](sony.md) — Cámaras empresariales japonesas
- [Guía de Conexión de Canon](canon.md) — Cámaras profesionales japonesas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
