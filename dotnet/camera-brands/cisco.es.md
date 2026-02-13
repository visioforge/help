---
title: Cómo Conectar una Cámara IP Cisco en C# .NET
description: Conecta cámaras Cisco CIVS, PVC, VC y Linksys WVC en C# .NET con patrones de URL RTSP y ejemplos de código para modelos empresariales y PYME.
---

# Cómo Conectar una Cámara IP Cisco en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Cisco Systems** es la empresa de redes más grande del mundo, con sede en San José, California, EE.UU. Cisco produjo cámaras IP bajo las marcas **Cisco** y **Linksys**. Las principales líneas de cámaras fueron **Cisco Video Surveillance (CIVS/VC/PVC)** para despliegues empresariales y la antigua serie **Linksys (WVC)** para mercados de consumo y pequeñas empresas. Cisco vendió su negocio de videovigilancia a Verkada y descontinuó la mayoría de productos de cámaras, pero muchas unidades permanecen desplegadas en el campo. La línea **Meraki MV** (gestionada en la nube) es el único producto de cámaras Cisco actual.

**Datos clave:**

- **Líneas de producto:** CIVS (videovigilancia empresarial), PVC (pequeña empresa), VC (cámara de video), WVC (cámara de video inalámbrica, antigua Linksys), WCS (servidor de cámara inalámbrica), Meraki MV (gestionada en la nube, actual)
- **Soporte de protocolos:** RTSP, HTTP/CGI, MJPEG, ASF (modelos WVC); Meraki MV es solo en la nube
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (varía según modelo)
- **Soporte ONVIF:** Limitado (solo algunos modelos CIVS más nuevos; la serie WVC no soporta ONVIF)
- **Códecs de video:** H.264, MPEG-4 (serie WVC), MJPEG

!!! warning "Cámaras Meraki MV"
    Las cámaras Cisco Meraki MV usan acceso solo en la nube y **no** soportan streaming RTSP directo. No pueden conectarse mediante URLs RTSP locales. La información en esta página aplica a las líneas de productos de cámaras Cisco y Linksys anteriores.

## Patrones de URL RTSP

### Cámaras Empresariales (Series CIVS / VC / PVC)

La mayoría de las cámaras IP empresariales Cisco usan la ruta `/img/media.sav`:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo principal | `rtsp://IP:554/img/media.sav` | La mayoría de cámaras CIVS y WVC |
| Live SDP | `rtsp://IP:554/live.sdp` | VC240, PVC300, serie VC220 |
| Video SAV | `rtsp://IP:554/img/video.sav` | PVC2300 |
| Código de acceso | `rtsp://IP:554/[ACCESS_CODE]` | Código configurado en la interfaz web de la cámara |
| Flujo raíz | `rtsp://IP:554/` | Alternativa para PVC2300 y otros |

### URLs Específicas por Modelo

| Modelo | URL RTSP | Tipo |
|-------|----------|------|
| CIVS 2500 / 2521 / 2531 | `rtsp://IP:554/img/media.sav` | Domo/bullet empresarial |
| PVC300 | `rtsp://IP:554/live.sdp` | PTZ pequeña empresa |
| PVC2300 | `rtsp://IP:554/img/video.sav` | Caja pequeña empresa |
| PVC2300 (alt) | `rtsp://IP:554/` | Alternativa |
| VC220 | `rtsp://IP:554/live.sdp` | Cámara domo |
| VC240 | `rtsp://IP:554/live.sdp` | Cámara domo |
| WVC80N | `rtsp://IP:554/img/media.sav` | Inalámbrica Linksys |
| WVC210 | `rtsp://IP:554/img/media.sav` | Inalámbrica Linksys |
| WVC54GCA | `rtsp://IP:554/img/media.sav` | Inalámbrica Linksys |
| WCS-1130 | `rtsp://IP:554/play1.sdp` | Servidor de cámara inalámbrica |

!!! info "Extensión de archivo inusual"
    Las cámaras Cisco y Linksys usan la ruta `/img/media.sav`, que tiene una extensión de archivo `.sav` inusual. Este no es un formato de medios estándar -- es un endpoint RTSP específico de Cisco. No lo confunda con una URL de descarga de archivo.

### Serie WVC (Antigua Linksys)

La serie Linksys WVC (Wireless Video Camera) fue una línea popular de cámaras de consumo antes de que Cisco la rebautizara y eventualmente la descontinuara:

| Modelo | URL RTSP | Resolución | Notas |
|-------|----------|------------|-------|
| WVC54GCA | `rtsp://IP:554/img/media.sav` | 640x480 | Wi-Fi, MPEG-4 |
| WVC80N | `rtsp://IP:554/img/media.sav` | 640x480 | Wireless-N |
| WVC210 | `rtsp://IP:554/img/media.sav` | 640x480 | Wireless-G PTZ |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Cisco con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Cisco enterprise camera (CIVS/WVC), main stream
var uri = new Uri("rtsp://192.168.1.50:554/img/media.sav");
var username = "admin";
var password = "YourPassword";
```

Para cámaras VC240 o PVC300, use `/live.sdp` en lugar de `/img/media.sav`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/img/snapshot.cgi?size=3` | La mayoría de cámaras Cisco (tamaño: 1=QQVGA, 2=QVGA, 3=VGA) |
| Captura JPEG (VGA) | `http://IP/img/snapshot.cgi?img=vga` | Resolución con nombre |
| Flujo MJPEG | `http://IP/img/video.mjpeg` | Flujo MJPEG continuo |
| MJPEG (alt) | `http://IP/img/mjpeg.jpg` | Endpoint MJPEG alternativo |
| Flujo ASF | `http://IP/img/video.asf` | Flujo ASF de la serie WVC |
| Captura PVC300 | `http://IP/cgi-bin/viewer/snapshot.jpg?resolution=640x480` | Parámetro de resolución requerido |

## Solución de Problemas

### `/img/media.sav` no responde

La extensión `.sav` es un endpoint RTSP específico de Cisco. Si la URL no funciona:

1. Verifique la dirección IP de la cámara y que el puerto 554 esté abierto
2. Confirme que RTSP está habilitado en la interfaz web de la cámara
3. Algunos modelos requieren que se configure un código de acceso antes de que funcione el acceso RTSP -- verifique la configuración de streaming de la cámara
4. Intente la URL alternativa `rtsp://IP:554/` si la ruta específica no responde

### La serie WVC devuelve solo MPEG-4

Las cámaras Linksys WVC (WVC54GCA, WVC80N, WVC210) soportan MPEG-4 pero no H.264. El SDK de VisioForge maneja flujos MPEG-4 automáticamente. Si ve artefactos, asegúrese de que no está forzando la decodificación H.264.

### Autenticación por código de acceso

Algunas cámaras Cisco usan un código de acceso en lugar de usuario/contraseña tradicional para RTSP. El código de acceso se configura en la interfaz web de la cámara en la configuración de streaming y se agrega a la URL:

```
rtsp://IP:554/[YOUR_ACCESS_CODE]
```

### Flujos HTTP para cámaras antiguas

Las cámaras Cisco/Linksys antiguas pueden funcionar mejor con flujos ASF o MJPEG basados en HTTP que con RTSP. Use la URL ASF (`http://IP/img/video.asf`) como alternativa si RTSP no es confiable.

### Cámaras Meraki MV no accesibles

Las cámaras Meraki MV son gestionadas solo en la nube y no soportan acceso RTSP local. No hay URL RTSP local disponible para estas cámaras. El video solo puede accederse a través de la interfaz en la nube del Meraki Dashboard.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Cisco?**

Para la mayoría de cámaras IP Cisco y Linksys, use `rtsp://admin:contraseña@IP_CAMARA:554/img/media.sav`. Para cámaras de la serie VC/PVC, intente `rtsp://admin:contraseña@IP_CAMARA:554/live.sdp` en su lugar.

**¿Las cámaras Cisco soportan ONVIF?**

Solo algunas cámaras empresariales de la serie CIVS más nuevas tienen soporte ONVIF limitado. Las cámaras de consumo Linksys WVC y las cámaras Meraki MV no soportan ONVIF.

**¿Cisco aún fabrica cámaras?**

Cisco vendió su negocio de videovigilancia (línea CIVS) a Verkada y descontinuó las cámaras Linksys WVC. El único producto de cámaras Cisco actual es la serie Meraki MV, que es gestionada en la nube y no soporta RTSP. Sin embargo, muchas cámaras Cisco antiguas permanecen desplegadas y operativas.

**¿Qué códecs usan las cámaras Cisco?**

Las cámaras empresariales CIVS más nuevas soportan H.264. La serie Linksys WVC usa principalmente MPEG-4 y MJPEG. El códec depende del modelo y la versión del firmware.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Linksys](linksys.md) — Mismos patrones de URL, subsidiaria de Cisco
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
