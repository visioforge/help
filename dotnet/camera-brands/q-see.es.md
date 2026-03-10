---
title: Cómo conectar una cámara IP y DVR Q-See en C# .NET
description: Conecte cámaras y DVRs Q-See en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series QC, QCN y QS.
---

# Cómo conectar una cámara IP y DVR Q-See en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Q-See** fue una marca estadounidense de vigilancia de consumo con sede en Anaheim, California. Los DVRs y cámaras IP Q-See eran sistemas de vigilancia económicos populares vendidos a través de los principales minoristas de EE.UU., incluyendo Costco y Amazon. La empresa dejó de operar esencialmente en 2020, pero un gran número de sistemas Q-See permanecen desplegados en hogares y negocios. Los productos Q-See usaban una mezcla de **cámaras OEM de Dahua** y componentes de otros fabricantes chinos, lo que significa que la mayoría de dispositivos Q-See siguen las convenciones de URL RTSP de Dahua.

**Datos clave:**

- **Líneas de productos:** Serie QC (DVRs), Serie QCN (cámaras IP), Serie QS (kits DVR)
- **Soporte de protocolos:** RTSP, HTTP/CGI, ONVIF (algunos modelos de cámaras IP)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin o admin / 123456
- **Soporte ONVIF:** Algunos modelos de cámaras IP (serie QCN)
- **Códecs de vídeo:** H.264 (mayoría de modelos), MPEG-4 (DVRs más antiguos)
- **Base OEM:** Mezcla de Dahua y otros fabricantes

!!! warning "Q-See Ha Dejado de Operar"
    Q-See cesó operaciones alrededor de 2020. No hay actualizaciones de firmware, soporte técnico ni servicios en la nube disponibles. Si está integrando hardware Q-See, trátelo como equipo compatible con Dahua y pruebe primero los patrones de URL de Dahua. Consulte nuestra [guía de conexión de Dahua](dahua.md) para detalles adicionales.

## Patrones de URL RTSP

### Formato de URL Estándar (Basado en Dahua)

La mayoría de dispositivos Q-See usan el patrón de URL `cam/realmonitor` de Dahua:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de cámara (1 para cámaras independientes, 1-N para canales DVR) |
| `subtype` | 0 | Flujo principal (resolución más alta) |
| `subtype` | 1 | Subflujo (resolución más baja, menos ancho de banda) |

### Modelos DVR (Serie QC, Serie QS)

| Modelo / Serie | URL de Flujo Principal | Notas |
|----------------|------------------------|-------|
| QC-804 (DVR 4 canales) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Formato Dahua, cambie `channel` para cada entrada |
| QS408 / QS411 (kits DVR) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Formato Dahua |
| DVRs varios | `rtsp://IP:554/` | Flujo raíz (alternativa) |
| DVRs varios | `rtsp://IP:554/live.sdp` | Flujo SDP en vivo |
| DVRs varios | `rtsp://IP:554/ch0_unicast_firststream` | Primer flujo unicast |

### Modelos de Cámara IP (Serie QCN)

| Modelo | Resolución | URL | Notas |
|--------|------------|-----|-------|
| QCN7001B | 1080p | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Formato Dahua (recomendado) |
| QCN7001B | 1080p | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | Formato PSIA |
| QCN7001B | 1080p | `rtsp://IP:554/VideoInput/1/h264/1` | VideoInput H.264 |
| QCN7001B | 1080p | `rtsp://IP:554/VideoInput/1/mpeg4/1` | VideoInput MPEG-4 |
| QCN7005B | 1080p | `rtsp://IP:554/` | Flujo raíz |

### Formatos de URL Alternativos

Algunos dispositivos Q-See soportan patrones de URL adicionales:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Formato estándar Dahua (recomendado) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Subflujo (menor ancho de banda) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=00&authbasic=BASE64` | Con credenciales codificadas en base64 |
| `rtsp://IP:554/` | Flujo raíz (alternativa para muchos modelos) |
| `rtsp://IP:554/live.sdp` | Formato SDP en vivo |
| `rtsp://IP:554/ch0_unicast_firststream` | Primer flujo unicast |

!!! note "Autenticación Base64"
    El parámetro `authbasic=` usado en algunas URLs de Q-See toma credenciales codificadas en base64 en el formato `username:password`. Por ejemplo, `admin:admin` se codifica como `YWRtaW46YWRtaW4=`.

### URLs de Canal DVR

Para DVRs Q-See multicanal (QC-804, QS408, QS411, etc.):

| Canal | Flujo Principal | Subflujo |
|-------|-----------------|----------|
| Cámara 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Cámara 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Cámara N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Q-See con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Q-See QC-804 DVR, channel 1 main stream
var uri = new Uri("rtsp://192.168.1.100:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "admin";
```

Para acceder al subflujo, use `subtype=1` en lugar de `subtype=0`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura CGI | `http://IP/cgi-bin/snapshot.cgi?chn=1&u=USER&p=PASS` | Captura basada en canal con credenciales |
| Captura basada en Login | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Captura con parámetros de login |
| Imagen Fija | `http://IP/stillimg1.jpg` | Reemplace `1` con número de canal |
| Imagen de Flujo | `http://IP/images/stream_1.jpg` | Reemplace `1` con número de canal |
| Flujo Rápido (Serie QS) | `http://IP/control/faststream.jpg?stream=MxPEG&needlength&fps=6` | Flujo rápido continuo |

## Solución de Problemas

### Sin actualizaciones de firmware ni soporte

Q-See cesó operaciones alrededor de 2020. No hay actualizaciones de firmware, no hay soporte técnico y no hay piezas de repuesto disponibles. Si su dispositivo Q-See tiene una vulnerabilidad de seguridad o error, no puede ser parcheado. Considere actualizar a una marca de cámaras actualmente soportada.

### Pruebe primero los patrones de URL de Dahua

La mayoría de DVRs Q-See y muchas cámaras IP usan firmware Dahua internamente. Si las URLs específicas de Q-See listadas anteriormente no funcionan, pruebe el formato estándar `cam/realmonitor` de Dahua. Consulte nuestra [guía de conexión de Dahua](dahua.md) para el conjunto completo de patrones de URL de Dahua.

### Parámetro de autenticación Base64

Algunos dispositivos Q-See usan un parámetro `authbasic=` en la URL RTSP en lugar de incrustar credenciales en la URI. Codifique `username:password` como base64:

- `admin:admin` = `YWRtaW46YWRtaW4=`
- `admin:123456` = `YWRtaW46MTIzNDU2`

### Reenvío de puertos para acceso remoto

Los DVRs Q-See típicamente requieren reenvío manual de puertos para acceso RTSP remoto. Reenvíe el puerto **554** (RTSP) y opcionalmente el puerto **80** u **8080** (HTTP) en su router a la dirección IP local del DVR.

### Credenciales predeterminadas

Los dispositivos Q-See comúnmente se envían con uno de estos pares de credenciales:

- `admin` / `admin`
- `admin` / `123456`

Si ninguno funciona, la contraseña puede haber sido cambiada por el propietario o instalador anterior.

## Preguntas Frecuentes

**¿Qué formato de URL RTSP usan las cámaras Q-See?**

La mayoría de dispositivos Q-See usan el formato `cam/realmonitor` de Dahua: `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0`. Esto se debe a que las cámaras y DVRs Q-See eran principalmente versiones OEM de hardware Dahua. Use `channel=1` para cámaras independientes o el número de canal apropiado para entradas DVR.

**¿Las cámaras Q-See siguen teniendo soporte?**

No. Q-See cesó operaciones alrededor de 2020. No hay actualizaciones de firmware, servicios en la nube ni soporte técnico disponible. El hardware sigue funcionando, pero no habrá futuros parches ni mejoras. Muchos usuarios han migrado a otras marcas como Amcrest o Reolink que usan protocolos similares basados en Dahua.

**¿Puedo usar cámaras Q-See con ONVIF?**

Algunas cámaras IP Q-See (serie QCN) soportan ONVIF, pero la mayoría de DVRs Q-See no lo hacen. Si el descubrimiento ONVIF falla, use los patrones de URL RTSP directos listados anteriormente.

**¿Cuál es la contraseña predeterminada para las cámaras Q-See?**

Las credenciales predeterminadas son típicamente `admin` / `admin` o `admin` / `123456`. Dado que Q-See ya no está disponible, no hay una herramienta oficial de restablecimiento de contraseña. Un restablecimiento de fábrica (generalmente un botón de orificio en el dispositivo) restaurará las credenciales predeterminadas en la mayoría de los modelos.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Dahua](dahua.md) — Mismo formato de URL para la mayoría de dispositivos Q-See
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
