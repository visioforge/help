---
title: Swann: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecta cámaras Swann en C# .NET con patrones de URL RTSP y ejemplos de código para modelos NHD, SWNHD, DVR/NVR y ADS antiguos.
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
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Swann en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Swann** (Swann Communications) es una marca australiana de seguridad para consumidores con sede en Melbourne, Australia, ahora propiedad de **Infinova**. Swann es una de las marcas de seguridad para consumidores y prosumidores más conocidas, popular por sus sistemas de cámaras DVR/NVR vendidos a través de grandes minoristas. Swann ofrece una gama de cámaras IP independientes, sistemas de cámaras analógicas sobre coaxial (BNC) y grabadores de vídeo en red.

**Datos clave:**

- **Líneas de producto:** NHD (cámaras HD de red actuales), SWNHD (cámaras IP HD), SWPRO (analógica sobre coaxial), sistemas DVR/NVR, ADS (cámaras IP antiguas)
- **Soporte de protocolos:** RTSP, ONVIF (modelos NHD actuales), HTTP/MJPEG (antiguos)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin o admin / (vacío) en modelos antiguos
- **Soporte ONVIF:** Sí (cámaras NHD-series actuales)
- **Códecs de vídeo:** H.264, H.265 (modelos actuales), MPEG-4 (DVRs antiguos)
- **Base OEM:** Muchos NVRs Swann más nuevos son OEM de Hikvision y usan patrones de URL RTSP de Hikvision

!!! info "NVRs Swann y Hikvision"
    Muchos NVRs Swann actuales son fabricados por Hikvision y usan firmware Hikvision. Si la URL RTSP estándar de Swann no funciona en su NVR, intente el formato de URL de Hikvision (`/Streaming/Channels/`). Consulte nuestra [guía de conexión de Hikvision](hikvision.md) para detalles.

## Patrones de URL RTSP

### Cámaras IP NHD-Series Actuales

Las cámaras IP independientes Swann NHD-series (SWNHD-820CAM, SWNHD-830CAM, NHD-866, etc.) usan la siguiente URL:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:554/live/h264
```

### Sistemas NVR (Basados en Hikvision)

La mayoría de NVRs Swann actuales usan rutas RTSP estilo Hikvision:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:554//Streaming/Channels/[ID_CANAL]
```

| Canal | Flujo Principal | Subflujo |
|---------|-------------|------------|
| Cámara 1 | `rtsp://IP:554//Streaming/Channels/1` | `rtsp://IP:554//Streaming/Channels/102` |
| Cámara 2 | `rtsp://IP:554//Streaming/Channels/2` | `rtsp://IP:554//Streaming/Channels/202` |
| Cámara N | `rtsp://IP:554//Streaming/Channels/N` | `rtsp://IP:554//Streaming/Channels/N02` |

!!! note "Numeración de canales"
    Para NVRs basados en Hikvision, el ID del canal del flujo principal coincide con el número de la cámara (1, 2, 3...). El subflujo usa el formato `N02` donde N es el número de la cámara (102, 202, 302...).

### Modelos DVR Anteriores

Los sistemas DVR Swann más antiguos (DVR4-PRO-NET, etc.) y cámaras independientes usan MPEG-4:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:554/mpeg4
```

### Tabla Resumen de URLs

| Modelo / Serie | URL de Flujo Principal | Notas |
|----------------|----------------|-------|
| Cámaras NHD-series (SWNHD-820/830) | `rtsp://IP:554/live/h264` | Cámaras IP independientes |
| IP-3G ConnectCam | `rtsp://IP:554/mpeg4` | Independiente antigua |
| Max-IP-Cam | `rtsp://IP:554/mpeg4` | Independiente antigua |
| NVR actual (canal 1) | `rtsp://IP:554//Streaming/Channels/1` | OEM Hikvision |
| NVR actual (canal 1, sub) | `rtsp://IP:554//Streaming/Channels/102` | OEM Hikvision |
| DVR4-PRO-NET | `rtsp://IP:554/mpeg4` | DVR antiguo |
| Cámaras IP Swann genéricas | `rtsp://IP:554/live/h264` | Intente esta primero |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Swann con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Swann NHD-series camera, main stream
var uri = new Uri("rtsp://192.168.1.90:554/live/h264");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo del NVR, use `/Streaming/Channels/102` en lugar de `/Streaming/Channels/1`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Flujo HTTP (ADS-440 antigua) | `http://IP/videostream.asf?user=USER&pwd=PASS` | Formato ASF, sin RTSP |
| Flujo MJPEG (antiguo) | `http://IP/videostream.cgi?user=USER&pwd=PASS` | Modelos antiguos |
| Captura ONVIF | `http://IP/onvif-http/snapshot` | NHD-series con ONVIF |

!!! warning "Cámaras antiguas solo HTTP"
    La serie ADS-440 y algunos otros modelos Swann antiguos solo soportan streaming HTTP (ASF o MJPEG) y no soportan RTSP en absoluto. Use la URL HTTP directamente para estas cámaras.

## Solución de Problemas

### Identificar el tipo de firmware de su NVR

Muchos NVRs Swann son OEM de Hikvision. Para determinar qué formato de URL usar:

1. Acceda a la interfaz web del NVR en `http://IP_NVR`
2. Verifique la página de inicio de sesión -- los NVRs basados en Hikvision a menudo muestran una interfaz estilo Hikvision
3. Intente primero la URL de Hikvision (`/Streaming/Channels/1`), luego recurra a las URLs de Swann (`/live/h264` o `/mpeg4`)

### "Conexión rechazada" en cámaras antiguas

Las cámaras Swann más antiguas (serie ADS-440, modelos DVR tempranos) pueden no soportar RTSP en absoluto. Estas cámaras usan streaming basado en HTTP solamente. Intente la URL HTTP ASF o MJPEG en lugar de RTSP.

### Las credenciales predeterminadas no funcionan

- Los modelos actuales típicamente vienen con admin / admin pero requieren cambio de contraseña en la primera configuración
- Algunos modelos antiguos usan admin con contraseña vacía
- Siempre complete la configuración inicial a través de la interfaz web de Swann o la app SwannView antes de intentar acceso RTSP

### SwannView vs acceso RTSP local

SwannView (servicio en la nube de Swann) es independiente del acceso RTSP local. No necesita una cuenta SwannView para usar streaming RTSP en su red local. RTSP funciona puramente a través de la conexión de red local.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Swann?**

Para cámaras NHD-series actuales, use `rtsp://admin:contraseña@IP_CAMARA:554/live/h264`. Para NVRs Swann (basados en Hikvision), use `rtsp://admin:contraseña@IP_NVR:554//Streaming/Channels/1` para el flujo principal del canal 1.

**¿Los NVRs Swann son compatibles con URLs RTSP de Hikvision?**

Sí. Muchos NVRs Swann actuales son fabricados por Hikvision y usan firmware idéntico. El formato de URL RTSP de Hikvision (`/Streaming/Channels/`) funciona en estos sistemas. Si la URL estándar de Swann falla, intente el formato Hikvision.

**¿Todas las cámaras Swann soportan RTSP?**

No. Algunos modelos Swann antiguos (particularmente la serie ADS-440) solo soportan streaming basado en HTTP en formato ASF o MJPEG. Todas las cámaras NHD-series y NVRs actuales soportan RTSP.

**¿Las cámaras Swann soportan ONVIF?**

Sí, las cámaras NHD-series actuales soportan ONVIF. Los modelos antiguos (SWPRO, serie ADS) generalmente no soportan ONVIF.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Lorex](lorex.md) — Par en segmento consumo/prosumidor
- [Guía de Conexión de Hikvision](hikvision.md) — NVRs Swann con firmware Hikvision
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
