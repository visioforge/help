---
title: Cómo Conectar una Cámara IP Hanwha Vision en C# .NET
description: Conecte cámaras Hanwha Vision en C# .NET con patrones de URL RTSP y ejemplos de código para las series X, Q, P, L y modelos NVR Wisenet.
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
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Hanwha Vision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Hanwha Vision** (anteriormente Hanwha Techwin, anteriormente Samsung Techwin) es un fabricante surcoreano de videovigilancia y subsidiaria de Hanwha Group. Hanwha adquirió la división de seguridad de Samsung en 2015 y cambió su marca a Hanwha Vision en 2023. Todas las cámaras se comercializan bajo la marca de producto **Wisenet**. Hanwha Vision es uno de los 5 principales fabricantes mundiales de vigilancia con fuerte presencia en mercados empresariales y gubernamentales.

**Datos clave:**

- **Líneas de productos:** Wisenet X (premium), Wisenet P (IA/4K), Wisenet Q (mainstream), Wisenet L (valor), Wisenet T (térmico)
- **Soporte de protocolos:** RTSP, ONVIF Profile S/G/T, HTTP/CGI, SUNAPI (propietario)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (se establece durante la configuración inicial; modelos antiguos: admin / 4321)
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de vídeo:** H.264, H.265 (WiseStream II), MJPEG
- **Marca de producto:** Wisenet (consulte también nuestra [guía Samsung/Hanwha](samsung.md) para URLs heredadas de Samsung Techwin)

!!! info "Hanwha Vision vs Samsung vs Wisenet"
    **Hanwha Vision** es el nombre de la empresa (desde 2023). **Wisenet** es la marca de producto para todas las cámaras y NVRs. **Samsung Techwin** fue el nombre anterior de la empresa (antes de 2015). Nuestra [guía Samsung/Hanwha](samsung.md) cubre modelos heredados de marca Samsung. Esta página cubre los productos actuales de Hanwha Vision / Wisenet.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Hanwha Vision utilizan una estructura de URL RTSP basada en perfiles:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/profile[N]/media.smp
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `profile1` | Profile 1 | Normalmente configurado como flujo principal |
| `profile2` | Profile 2 | Normalmente configurado como subflujo |
| `profile3` | Profile 3 | Tercer flujo (si está configurado) |

### Modelos de Cámaras

| Serie de Modelo | Resolución | URL de Flujo Principal | Audio |
|----------------|-----------|----------------------|-------|
| XNO-6080R (X bala 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Sí |
| XNO-8080R (X bala 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Sí |
| XNO-9080R (X bala 4K) | 3840x2160 | `rtsp://IP:554/profile2/media.smp` | Sí |
| XND-6080 (X domo 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Sí |
| XND-8080RV (X domo 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Sí |
| XNP-6120H (X PTZ 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Sí |
| PNO-A9081R (P bala IA 4K) | 3840x2160 | `rtsp://IP:554/profile2/media.smp` | Sí |
| QNO-8080R (Q bala 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Sí |
| QND-8080R (Q domo 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Sí |
| LNO-6032R (L bala 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | No |

!!! tip "Numeración de Perfiles"
    En la mayoría de las cámaras Hanwha Vision, `profile2` es el flujo principal (mayor calidad), lo cual difiere de la mayoría de otras marcas que usan el perfil/canal 1. Si `profile2` no funciona, pruebe `profile1` o `profile3`. Puede verificar las asignaciones de perfil en la interfaz web de la cámara en **Video Profile**.

### URLs de Canales del NVR

Para NVRs Wisenet (series XRN, QRN, LRN):

| Canal | Flujo Principal | Subflujo |
|-------|----------------|----------|
| Cámara 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | `rtsp://IP:554/profile3/media.smp/trackID=channel1` |
| Cámara 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | `rtsp://IP:554/profile3/media.smp/trackID=channel2` |
| Cámara N | `rtsp://IP:554/profile2/media.smp/trackID=channelN` | `rtsp://IP:554/profile3/media.smp/trackID=channelN` |

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/profile2/media.smp` | Estándar (recomendado) |
| `rtsp://IP:554/profile1/media.smp` | Primer perfil |
| `rtsp://IP:554/onvif-media/media.amp` | Servicio de medios ONVIF |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Algunas variantes OEM |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Hanwha Vision con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Hanwha Vision XNO-8080R (Wisenet X 5MP), main stream
var uri = new Uri("rtsp://192.168.1.90:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `/profile3/media.smp` en lugar de `/profile2/media.smp`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/video.cgi?msubmenu=jpg&action=view&Resolution=1920x1080&Quality=5&Channel=0` | Captura a resolución completa |
| Captura JPEG (simple) | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación digest |
| Flujo MJPEG | `http://IP/cgi-bin/video.cgi?msubmenu=mjpeg&action=view&Channel=0&Stream=0` | MJPEG continuo |

## Solución de Problemas

### Confusión entre profile2 y profile1

Las cámaras Hanwha Vision normalmente asignan `profile2` como el flujo principal (mayor calidad), lo cual difiere de la mayoría de otras marcas que usan el perfil/canal 1. Si no obtiene vídeo o la resolución es baja desde `profile2`, verifique la configuración de perfil en la interfaz web de la cámara en **Video Profile**.

### Se requiere activación de contraseña

Las cámaras Hanwha Vision actuales se envían sin contraseña predeterminada. Debe activar la cámara y establecer una contraseña a través de:

1. Wisenet Installation Wizard (herramienta IP Installer)
2. Navegador web en `http://CAMERA_IP`
3. Aplicación móvil Wisenet

Los modelos más antiguos de Samsung Techwin usaban `admin` / `4321` como predeterminados.

### Códec WiseStream II

WiseStream II es la tecnología de codificación dinámica de Hanwha que ajusta la compresión por región en el fotograma. Produce flujos H.265 o H.264 estándar que son compatibles con cualquier decodificador. No se requiere ningún códec especial.

### SUNAPI vs ONVIF

Las cámaras Hanwha Vision soportan tanto su SUNAPI propietario como el estándar ONVIF. Para la integración con VisioForge SDK, use las URLs RTSP anteriores o el descubrimiento ONVIF. SUNAPI se usa principalmente por el VMS propio de Hanwha (SSM/Wisenet WAVE).

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Hanwha Vision (Wisenet)?**

La URL estándar es `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp` para el flujo principal. Use `profile3` para el subflujo. Los números de perfil pueden personalizarse en la interfaz web de la cámara.

**¿Las cámaras Hanwha Vision y Samsung son iguales?**

Hanwha Vision adquirió la división de cámaras de seguridad de Samsung en 2015 (entonces llamada Samsung Techwin, luego Hanwha Techwin, ahora Hanwha Vision). Las cámaras actuales se venden bajo la marca **Wisenet**. Las cámaras heredadas de marca Samsung pueden usar patrones de URL diferentes -- consulte nuestra [guía Samsung/Hanwha](samsung.md).

**¿Cuál es la diferencia entre las series Wisenet X, P, Q y L?**

**X** = premium empresarial (mejor poca luz, WDR). **P** = con inteligencia artificial (analítica de aprendizaje profundo). **Q** = negocio mainstream (buen equilibrio de características y precio). **L** = valor/nivel de entrada (características básicas, precios competitivos). **T** = imagen térmica.

**¿Las cámaras Hanwha Vision soportan ONVIF?**

Sí. Todas las cámaras Hanwha Vision actuales soportan ONVIF Profile S, G y T. ONVIF proporciona descubrimiento estandarizado, transmisión y control PTZ.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía Heredada Samsung/Hanwha](samsung.md) — Modelos más antiguos de Samsung Techwin
- [Guía de Productos Wisenet](wisenet.md) — Detalles de la familia de productos Wisenet
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
