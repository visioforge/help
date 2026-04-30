---
title: Cámaras IP Linksys: URLs RTSP y conexión en C# .NET
description: Conecte cámaras Linksys WVC, PVC y LCAB en C# .NET con patrones de URL RTSP, flujos ASF/MJPEG y ejemplos de código para modelos descontinuados de la serie WVC.
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
  - MP4
  - H.264
  - MJPEG
  - C#

---

# Cómo conectar una cámara IP Linksys en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Linksys** es una empresa estadounidense de redes con sede en Irvine, California. Originalmente adquirida por Cisco Systems en 2003, la marca fue vendida a Belkin (subsidiaria de Foxconn) en 2013. Durante los años de propiedad de Cisco, Linksys produjo la popular serie **WVC (Wireless Video Camera)** de cámaras IP para el mercado de consumo y prosumidor. La línea de productos de cámaras fue descontinuada alrededor de 2014, pero muchas unidades permanecen desplegadas y operativas.

Debido a que Linksys era una marca de Cisco, sus cámaras comparten patrones de URL y firmware idénticos con los productos de cámaras de consumo de Cisco. La extensión `.sav` en las URLs RTSP es un formato propietario de endpoint de Cisco/Linksys.

**Datos clave:**

- **Líneas de productos:** Serie WVC (WVC54G, WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210, WVC2300), Serie PVC (PVC2300), Serie LCAB
- **Soporte de protocolos:** RTSP, HTTP/ASF, MJPEG, MMS (Windows Media)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Limitado (solo serie LCAB)
- **Códecs de video:** MPEG-4/ASF (serie WVC), H.264 (modelos más nuevos), MJPEG

!!! warning "Línea de productos descontinuada"
    Las cámaras IP Linksys fueron descontinuadas alrededor de 2014. No hay nuevas actualizaciones de firmware ni soporte oficial disponible. La información en esta página se proporciona para instalaciones legacy. Muchos modelos WVC requieren Internet Explorer con ActiveX para su interfaz web.

!!! info "Linksys = cámaras de consumo Cisco"
    Las cámaras Linksys usan los mismos patrones de URL que las cámaras de consumo Cisco ya que Linksys era una marca de Cisco. Consulte nuestra [guía de conexión de Cisco](cisco.md) para detalles adicionales y modelos de cámaras empresariales Cisco.

## Patrones de URL RTSP

### Formato de URL Estándar

La mayoría de cámaras Linksys usan la ruta RTSP de Cisco `/img/media.sav`:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/img/media.sav
```

!!! note "Extensión `.sav` inusual"
    La extensión `.sav` es un endpoint RTSP propietario de Cisco/Linksys -- no es un formato de archivo multimedia estándar. No lo confunda con una URL de descarga de archivo.

### URLs RTSP por Modelo

| Modelo | URL RTSP | Códec | Notas |
|--------|----------|-------|-------|
| WVC54GCA | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G, 640x480 |
| WVC80N | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-N, 640x480 |
| WVC80N (alt) | `rtsp://IP:554/img/video.sav` | MPEG-4 | Endpoint de video alternativo |
| WVC210 | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G PTZ |
| WVC200 | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G |
| PVC2300 | `rtsp://IP:554/video.mp4` | MPEG-4/H.264 | Cámara de caja para pequeñas empresas |
| LCAB03VLNOD | `rtsp://IP:554//ONVIF/channel2` | H.264 | Cámara exterior con ONVIF |

### Resumen de Familia de Modelos

| Familia de Modelo | Flujo RTSP | HTTP ASF | MJPEG | CGI de Captura |
|-------------------|------------|----------|-------|----------------|
| WVC54G / WVC54GC / WVC54GCA | `/img/media.sav` | Sí | Sí | Sí |
| WVC80N | `/img/media.sav`, `/img/video.sav` | Sí | Sí | -- |
| WVC200 / WVC210 | `/img/media.sav` | Sí | Sí | Sí |
| WVC2300 | `/img/media.sav` | Sí | -- | -- |
| PVC2300 | `/video.mp4` | Sí | -- | -- |
| Serie LCAB | `//ONVIF/channel2` | -- | -- | -- |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Linksys con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Linksys WVC80N, primary RTSP stream
var uri = new Uri("rtsp://192.168.1.60:554/img/media.sav");
var username = "admin";
var password = "admin";
```

Para cámaras PVC2300, use `/video.mp4` en lugar de `/img/media.sav`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Modelos Compatibles |
|------|---------------|---------------------|
| Flujo de Video ASF | `http://IP/img/video.asf` | WVC54G, WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210, WVC11b |
| Flujo MJPEG | `http://IP/img/video.mjpeg` | WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210 |
| Cuadro Único MJPEG | `http://IP/img/mjpeg.jpg` | Mayoría de modelos WVC |
| MJPEG CGI | `http://IP/img/mjpeg.cgi` | Mayoría de modelos WVC |
| MJPEG (mayúsculas) | `http://IP/MJPEG.CGI` | Algunos modelos WVC |
| Captura Alta Resolución | `http://IP/img/snapshot.cgi?size=3` | WVC54GCA, WVC200, WVC210 |
| Captura Media | `http://IP/img/snapshot.cgi?size=2` | WVC54GCA, WVC200, WVC210 |
| Captura VGA | `http://IP/img/snapshot.cgi?img=vga` | WVC54GCA, WVC200, WVC210 |
| ASF Alternativo | `http://IP/videostream.asf` | WVC54GC, WVC80N |
| Flujo MMS | `mms://IP/img/video.asf` | Legacy Windows Media (todos los modelos WVC) |

!!! tip "Flujos HTTP como alternativa a RTSP"
    Muchas cámaras Linksys WVC funcionan de manera más confiable con flujos ASF o MJPEG basados en HTTP que con RTSP. Si la URL RTSP no responde, pruebe el flujo ASF en `http://IP/img/video.asf` como alternativa.

## Solución de Problemas

### El flujo RTSP no conecta

Las cámaras Linksys WVC tienen soporte RTSP limitado. Muchos modelos sirven video principalmente por HTTP usando ASF (Advanced Streaming Format) en lugar de RTSP verdadero:

1. Verifique la dirección IP de la cámara y que el puerto 554 esté abierto
2. Confirme que RTSP esté habilitado en la interfaz web de la cámara
3. Pruebe el flujo HTTP ASF (`http://IP/img/video.asf`) como alternativa
4. Algunos modelos requieren que la interfaz web se acceda primero (vía Internet Explorer con ActiveX) antes de que RTSP esté disponible

### El flujo ASF requiere manejo específico

Los flujos ASF (Advanced Streaming Format) de las cámaras WVC usan un contenedor propietario de Microsoft. El VisioForge SDK maneja flujos ASF automáticamente. Si encuentra problemas:

- Asegúrese de que se está conectando vía HTTP, no RTSP, para URLs ASF
- Los flujos ASF pueden requerir componentes de Windows Media o filtros LAV en algunos sistemas

### Flujos con protocolo MMS

Las URLs del protocolo `mms://` son específicas de Windows Media y solo funcionan con Windows Media Player o decodificadores compatibles. Para aplicaciones modernas, use la URL HTTP ASF (`http://IP/img/video.asf`) en lugar del equivalente MMS.

### La interfaz web requiere Internet Explorer

Muchos modelos WVC requieren Internet Explorer con controles ActiveX para su interfaz de configuración web. Use Internet Explorer o un navegador compatible con ActiveX para acceder a la configuración de la cámara. Los flujos RTSP y HTTP en sí funcionan con cualquier cliente.

### Cámara no descubrible en la red

Las cámaras Linksys no soportan protocolos de descubrimiento modernos (excepto la serie LCAB con ONVIF). Para encontrar la cámara:

1. Verifique la tabla de concesiones DHCP de su router para la dirección IP de la cámara
2. Use la Utilidad de Cámara Linksys (si aún está disponible) para el descubrimiento
3. Pruebe la dirección IP predeterminada asignada por la cámara (consulte el manual del modelo)
4. Use un escáner de red como Advanced IP Scanner

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Linksys?**

Para la mayoría de cámaras Linksys WVC, use `rtsp://admin:admin@CAMERA_IP:554/img/media.sav`. Para la PVC2300, use `rtsp://admin:admin@CAMERA_IP:554/video.mp4` en su lugar. Si RTSP no funciona, pruebe el flujo HTTP ASF en `http://CAMERA_IP/img/video.asf`.

**¿Las cámaras Linksys siguen disponibles para compra?**

No. Linksys descontinuó toda su línea de productos de cámaras IP alrededor de 2014, poco después de que la marca fuera vendida de Cisco a Belkin/Foxconn. No hay actualizaciones de firmware ni soporte oficial disponible. Sin embargo, muchas cámaras WVC y PVC permanecen en uso y funcionales.

**¿Las cámaras Linksys soportan ONVIF?**

Solo las cámaras de la serie LCAB tienen soporte ONVIF. Las series WVC y PVC no soportan ONVIF. Para cámaras WVC, use los patrones de URL RTSP o HTTP directos listados anteriormente.

**¿Las URLs de cámaras Linksys y Cisco son iguales?**

Sí. Las cámaras Linksys fueron producidas durante la propiedad de Cisco sobre la marca y comparten el mismo firmware y patrones de URL que las cámaras de consumo Cisco. La ruta RTSP `/img/media.sav` y la ruta HTTP `/img/video.asf` son idénticas en ambas marcas. Consulte nuestra [guía de conexión de Cisco](cisco.md) para más detalles.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Cisco](cisco.md) — Mismos patrones de URL, empresa matriz
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
