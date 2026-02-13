---
title: Cómo Conectar una Cámara IP Samsung (Hanwha) en C# .NET
description: Conecta cámaras Samsung Wisenet y Hanwha Vision en C# .NET con patrones de URL RTSP y ejemplos de código para modelos SNO, SND, XNO, XND y PNO.
---

# Cómo Conectar una Cámara IP Samsung (Hanwha) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Hanwha Vision** (anteriormente Samsung Techwin, luego Hanwha Techwin) es un fabricante surcoreano de equipos de videovigilancia profesional y empresarial. La marca de cámaras de seguridad Samsung fue renombrada a **Wisenet** después de que Hanwha Group adquiriera Samsung Techwin en 2015. Las cámaras Hanwha Vision se despliegan ampliamente en instalaciones empresariales, gubernamentales y de infraestructura crítica en todo el mundo.

**Datos clave:**

- **Líneas de producto:** XNO/XND/XNV (serie X, buque insignia actual), PNO/PND/PNV (serie P, gama media), QNO/QND/QNV (serie Q, económica), SNO/SND/SNV/SNB (serie S, legacy Samsung)
- **Convención de nombres:** Primera letra = serie, N = red, O = exterior, D = domo, V = antivandalismo, B = caja, P = PTZ
- **Soporte de protocolos:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Wisenet WAVE VMS
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (establecido durante configuración inicial); modelos legacy Samsung: admin / 4321
- **Soporte ONVIF:** Sí (todos los modelos actuales y la mayoría de los legacy)
- **Códecs de video:** H.264, H.265, MJPEG

## Patrones de URL RTSP

### Modelos Actuales (Wisenet Series X/P/Q)

Las cámaras Hanwha Vision actuales usan un formato de URL basado en perfiles:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Perfil 1 (principal) | `rtsp://IP:554/profile2/media.smp` | Flujo principal, H.264/H.265 |
| Perfil 2 (sub) | `rtsp://IP:554/profile3/media.smp` | Subflujo |
| ONVIF Perfil 1 | `rtsp://IP:554//onvif/profile1/media.smp` | Compatible con ONVIF (nota la doble barra) |
| ONVIF Perfil 2 | `rtsp://IP:554//onvif/profile2/media.smp` | Subflujo ONVIF |

!!! warning "Doble barra en URLs ONVIF"
    Las URLs ONVIF de Samsung/Hanwha usan una doble barra (`//onvif/`). Esto es intencional y obligatorio. Usar una sola barra fallará.

### Samsung Serie S Legacy

| Serie de Modelo | URL RTSP | Códec |
|-------------|----------|-------|
| SNB-xxxx (caja) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SND-xxxx (domo) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNO-xxxx (exterior) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNV-xxxx (antivandalismo) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNP-xxxx (PTZ) | `rtsp://IP:554/profile2/media.smp` | H.264 |

### Modelos Samsung Más Antiguos (Pre-Wisenet)

Los modelos Samsung más antiguos usaban formatos de URL diferentes:

| Patrón de URL | Modelos | Códec |
|-------------|--------|-------|
| `rtsp://IP:554/mpeg4unicast` | SNB-2000, SNC-1300, SNP-3301/3370 | MPEG-4 |
| `rtsp://IP:554/h264unicast` | SNP-3301/H, SNP-3370/TH | H.264 |
| `rtsp://IP:554/mjpegunicast` | SNP-3301/H, SNP-3370/TH | MJPEG |
| `rtsp://IP:554/H264/media.smp` | SNB-3000, SND-3080, SNV-3080/3081 | H.264 |
| `rtsp://IP:554/MPEG4/media.smp` | SNV-3080/3081 | MPEG-4 |
| `rtsp://IP:554/MJPEG/media.smp` | SNB-3000, SNV-3081, SNV-6084R | MJPEG |
| `rtsp://IP:554/MediaInput/h264` | Varios Samsung | H.264 |

### URLs DVR/NVR

| Dispositivo | URL RTSP | Notas |
|--------|----------|-------|
| SRD-165 DVR | `rtsp://IP:558/` | Puerto no estándar 558 |
| SME DVR | `rtsp://IP:554/mpeg4unicast` | MPEG-4 |
| SMT DVR | `rtsp://IP:554/mpeg4unicast` | MPEG-4 |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Samsung (Hanwha Wisenet) con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Hanwha Wisenet X-series camera, main stream
var uri = new Uri("rtsp://192.168.1.70:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, usa `profile3/media.smp` en lugar de `profile2/media.smp`. Para modelos legacy Samsung serie S, usa la contraseña predeterminada `4321` y la ruta de URL `/mpeg4unicast` o `/H264/media.smp`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/cgi-bin/video.cgi?msubmenu=jpg` | Modelos actuales |
| Flujo MJPEG | `http://IP/cgi-bin/video.cgi?msubmenu=mjpg` | Modelos actuales |
| Captura Legacy | `http://IP/video?submenu=jpg` | Modelos pre-Wisenet |
| MJPEG Legacy | `http://IP/video?submenu=mjpg` | Modelos pre-Wisenet |
| Captura CGI | `http://IP/cgi-bin/webra_fcgi.fcgi?api=get_jpeg_raw&chno=CHANNEL` | Modelos DVR |
| Captura (dimensionada) | `http://IP/snap.jpg?JpegSize=XL` | Algunos firmware OEM Bosch |

## Solución de Problemas

### Diferencias en contraseña predeterminada

- **Modelos Hanwha Vision actuales:** La contraseña debe establecerse durante la configuración inicial a través del navegador web
- **Samsung serie S legacy:** La contraseña predeterminada es `4321`
- **Modelos Samsung muy antiguos:** Algunos usaban `admin` / `admin`

### profile2 vs profile1 en la URL

Las cámaras Samsung/Hanwha usan `profile2/media.smp` para el flujo principal (no `profile1`). Esta es una fuente común de confusión:

- `profile2/media.smp` = Flujo principal (típicamente H.264 a resolución completa)
- `profile3/media.smp` = Subflujo
- Los números de perfil pueden diferir según la configuración de la cámara

### Problema de doble barra ONVIF

El formato de URL ONVIF requiere una doble barra antes de `onvif`:
- Correcto: `rtsp://IP:554//onvif/profile1/media.smp`
- Incorrecto: `rtsp://IP:554/onvif/profile1/media.smp`

### Confusión de nombre de marca

Samsung Techwin fue adquirida por Hanwha en 2015. La marca se ha llamado:
- **Samsung Techwin** (antes de 2015)
- **Hanwha Techwin** (2015-2022)
- **Hanwha Vision** (2022-presente)
- **Wisenet** (nombre de marca del producto, usado en todo momento)

Todos usan los mismos patrones de URL RTSP dentro de su respectiva generación.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Samsung/Hanwha?**

Para modelos Wisenet actuales, la URL es `rtsp://admin:password@IP_CAMARA:554/profile2/media.smp`. Para modelos Samsung legacy, prueba `rtsp://admin:4321@IP_CAMARA:554/mpeg4unicast` o `rtsp://IP_CAMARA:554/H264/media.smp`.

**¿Samsung es lo mismo que Hanwha Vision?**

Sí. La división de cámaras de seguridad de Samsung fue adquirida por Hanwha Group en 2015. La marca del producto es **Wisenet**. Las cámaras Samsung legacy (series SNB, SND, SNO) y las cámaras Hanwha Vision actuales (series XNO, XND, PNO) usan patrones RTSP similares.

**¿Las cámaras Samsung/Hanwha soportan H.265?**

Sí. Las cámaras actuales de las series X y P soportan H.265 (HEVC). Las cámaras legacy de la serie S típicamente solo soportan H.264 y MPEG-4.

**¿Qué VMS funciona con cámaras Hanwha?**

El VMS propio de Hanwha es **Wisenet WAVE**. Sin embargo, todas las cámaras Hanwha soportan RTSP estándar y ONVIF, haciéndolas compatibles con cualquier software de terceros incluyendo aplicaciones del VisioForge SDK.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Hanwha Vision](hanwha.md) — Nombre de marca actual, mismas URLs
- [Guía de Conexión Wisenet](wisenet.md) — Familia de productos Hanwha Vision
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
