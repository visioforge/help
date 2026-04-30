---
title: BrickCom: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecte cámaras BrickCom en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series CB, MB, OB, VD, WCB, WOB y MD.
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

# Cómo Conectar una Cámara IP BrickCom en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**BrickCom** (Brickcom Corporation) es un fabricante profesional taiwanés de cámaras IP con sede en Taipéi, Taiwán. Fundada en 2004, BrickCom se dirige a los mercados de seguridad profesional y vigilancia industrial con una amplia gama de formatos que incluyen cámaras tipo bala, domo, cubo y especiales. La marca es conocida por su patrón de URL RTSP basado en canales, sencillo y consistente en todas sus líneas de productos.

**Datos clave:**

- **Líneas de productos:** CB (cubo), MB (mini bala), OB (bala exterior), VD (domo antivandálico), FD (domo fijo), MD (multidireccional), WCB/WOB (inalámbricas)
- **Patrón de URL basado en canales:** `/channel1` para flujo principal, `/channel2` para subflujo
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (la mayoría de modelos)
- **Códecs de vídeo:** H.264, MJPEG
- **URL RTSP principal:** `rtsp://IP:554/channel1`

!!! tip "Numeración de canales"
    BrickCom utiliza URLs simples basadas en canales. Use `/channel1` para el flujo primario (alta calidad) y `/channel2` para el flujo secundario (menor ancho de banda).

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras BrickCom utilizan un patrón de URL RTSP simple basado en canales:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/channel1
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `channel1` | Flujo principal | Flujo primario (mayor resolución) |
| `channel2` | Subflujo | Flujo secundario (menor resolución, menos ancho de banda) |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|----------------------|-------|
| CB-100 (cubo) | Cubo | `rtsp://IP:554/channel1` | Cámara cubo interior |
| MB-300Ap (mini bala) | Mini Bala | `rtsp://IP:554/channel1` | Factor de forma bala compacto |
| OB-100Ap (bala exterior) | Bala Exterior | `rtsp://IP:554/channel1` | Bala resistente a la intemperie |
| OB-300Af (bala exterior) | Bala Exterior | `rtsp://IP:554/channel1` | Bala con enfoque automático |
| VD-130Ae (domo antivandálico) | Domo Antivandálico | `rtsp://IP:554/channel1` | Domo con clasificación IK10 |
| VD-301AF (domo antivandálico) | Domo Antivandálico | `rtsp://IP:554/channel1` | Domo antivandálico con enfoque automático |
| VD-500Af (domo antivandálico) | Domo Antivandálico | `rtsp://IP:554/channel1` | Domo antivandálico 5MP |
| WCB-100Ap (cubo inalámbrico) | Cubo Inalámbrico | `rtsp://IP:554/channel1` | Cámara cubo Wi-Fi |
| WCB-300AP (cubo inalámbrico) | Cubo Inalámbrico | `rtsp://IP:554/channel1` | Cubo Wi-Fi, 3MP |
| WOB-100Ae (bala inalámbrica) | Bala Inalámbrica | `rtsp://IP:554/channel1` | Bala exterior Wi-Fi |
| MD-500AP-360-A1 (multi-domo) | Multidireccional | `rtsp://IP:554/channel1` | Multi-sensor 360 grados |

### Formatos de URL Alternativos

Algunos modelos y versiones de firmware de BrickCom soportan estas URLs RTSP adicionales:

| Patrón de URL | Modelos Soportados | Notas |
|---------------|-------------------|-------|
| `rtsp://IP:554/channel1` | La mayoría de modelos | Estándar (recomendado) |
| `rtsp://IP:554/h264` | Varios | Flujo H.264 directo |
| `rtsp://IP//ONVIF/channel2` | VD-500Af, WCB-100Ap | Subflujo ONVIF |
| `rtsp://IP/stream/bidirect/channel1` | Modelos selectos | Flujo bidireccional con audio |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara BrickCom con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// BrickCom OB-300Af, main stream
var uri = new Uri("rtsp://192.168.1.90:554/channel1");
var username = "admin";
var password = "admin";
```

Para acceso al subflujo, use `/channel2` en lugar de `/channel1`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/snapshot.jpg` | Sin autenticación requerida |
| Captura JPEG (auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Autenticación basada en URL |
| Captura de Canal | `http://IP/snapshot.jpg?user=USER&pwd=PASS&strm=1` | Canal específico con autenticación |
| Captura CGI | `http://IP/cgi-bin/media.cgi?action=getSnapshot` | Captura basada en CGI |
| Flujo HTTP de Canal | `http://IP/channel2` | Subflujo HTTP |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras BrickCom vienen con credenciales predeterminadas de **admin / admin**. Si ha cambiado la contraseña a través de la interfaz web:

1. Acceda a la cámara en `http://CAMERA_IP` desde un navegador
2. Navegue a **Configuration > User Management**
3. Verifique sus credenciales
4. Use esas credenciales en su URL RTSP

### La URL de canal no conecta

Si `rtsp://IP:554/channel1` no funciona, intente la URL H.264 alternativa:

- `rtsp://IP:554/h264` -- flujo H.264 directo sin especificación de canal
- Algunas versiones de firmware más antiguas pueden requerir el formato ONVIF: `rtsp://IP//ONVIF/channel2`

### Problemas de descubrimiento ONVIF

Las cámaras BrickCom soportan ONVIF en la mayoría de modelos. Si el descubrimiento ONVIF falla:

1. Acceda a la interfaz web en `http://CAMERA_IP`
2. Navegue a **Configuration > Network > ONVIF**
3. Asegúrese de que ONVIF esté habilitado
4. Verifique el puerto ONVIF (predeterminado: 80 o 8080)

### Desconexiones en modelos inalámbricos (WCB/WOB)

Las cámaras BrickCom inalámbricas (series WCB y WOB) pueden experimentar desconexiones RTSP intermitentes en redes Wi-Fi congestionadas. Use el subflujo (`/channel2`) para menores requisitos de ancho de banda, o conéctese por Ethernet para máxima confiabilidad.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras BrickCom?**

La URL es `rtsp://admin:admin@CAMERA_IP:554/channel1` para el flujo principal. Use `/channel2` para el subflujo con menor resolución y ancho de banda.

**¿Las cámaras BrickCom soportan ONVIF?**

Sí. La mayoría de los modelos actuales de BrickCom soportan ONVIF. Algunos modelos también exponen una ruta RTSP específica de ONVIF en `rtsp://IP//ONVIF/channel2`.

**¿Cuál es la diferencia entre channel1 y channel2?**

`/channel1` proporciona el flujo primario de alta resolución y `/channel2` proporciona un flujo secundario de menor resolución adecuado para miniaturas, visualización móvil o escenarios con restricciones de ancho de banda.

**¿Puedo acceder a múltiples flujos simultáneamente?**

Sí. Las cámaras BrickCom soportan conexiones concurrentes tanto a `/channel1` como a `/channel2`. El número máximo de conexiones simultáneas depende del modelo específico.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión AVTech](avtech.md) — Cámaras industriales taiwanesas
- [Guía de Conexión LILIN](lilin.md) — Cámaras profesionales taiwanesas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
