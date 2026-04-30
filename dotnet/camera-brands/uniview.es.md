---
title: Cómo Conectar una Cámara IP Uniview (UNV) en C# .NET
description: Conecte cámaras Uniview en C# .NET con patrones de URL RTSP y ejemplos de código para las series IPC-B, IPC-T, IPC-D, IPC-E y modelos NVR.
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
  - H.265
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Uniview (UNV) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Uniview** (Zhejiang Uniview Technologies Co., Ltd.), también conocida como **UNV**, es el tercer mayor fabricante mundial de videovigilancia por cuota de mercado, detrás de Hikvision y Dahua. Fundada en 2005 y con sede en Hangzhou, China, Uniview fue pionera en videovigilancia IP en China y ofrece una gama completa de cámaras IP, NVRs, software VMS y productos de control de acceso para mercados empresariales y gubernamentales.

**Datos clave:**

- **Líneas de productos:** IPC-B (bala), IPC-T (torreta), IPC-D (domo), IPC-E (eyeball), IPC-P (PTZ), NVR30x/50x (NVRs)
- **Soporte de protocolos:** RTSP, ONVIF Profile S/G/T, HTTP/CGI, SDK (EZStation)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / 123456 (debe cambiarse en el primer inicio de sesión con firmware más reciente)
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de vídeo:** H.264, H.265 (U-Code Smart Codec), MJPEG
- **Posición en el mercado:** #3 a nivel mundial en videovigilancia

!!! info "Uniview vs Marca UNV"
    Uniview comercializa bajo ambos nombres de marca **Uniview** y **UNV** dependiendo de la región. Los patrones de URL RTSP y el firmware son idénticos independientemente de la marca. Algunos socios OEM remarcan el hardware Uniview bajo sus propias etiquetas.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Uniview utilizan una estructura de URL basada en perfil de medios:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/media/video[STREAM]
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `video1` | Flujo principal | Mayor resolución (4K/5MP/4MP/2MP) |
| `video2` | Subflujo | Menor resolución, ancho de banda reducido |
| `video3` | Tercer flujo | Optimizado para móvil (si es compatible) |

### Formatos de URL Alternativos

Las cámaras Uniview soportan múltiples patrones de URL RTSP:

| Patrón de URL | Descripción |
|---------------|-------------|
| `rtsp://IP:554/media/video1` | Flujo principal (recomendado) |
| `rtsp://IP:554/media/video2` | Subflujo |
| `rtsp://IP:554/media/video3` | Tercer flujo |
| `rtsp://IP:554/unicast/c1/s0/live` | Flujo principal unicast (alternativo) |
| `rtsp://IP:554/unicast/c1/s1/live` | Subflujo unicast (alternativo) |
| `rtsp://IP:554/live/ch00_0` | Formato heredado (firmware antiguo) |
| `rtsp://IP:554/live/ch00_1` | Subflujo heredado |

### Modelos de Cámaras IP

| Serie de Modelo | Resolución | URL de Flujo Principal | Audio |
|----------------|-----------|----------------------|-------|
| IPC-B112-PF28 (bala 2MP) | 1920x1080 | `rtsp://IP:554/media/video1` | No |
| IPC-B314-APKZ (bala 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Sí |
| IPC-B315-APKZ (bala 5MP) | 2880x1620 | `rtsp://IP:554/media/video1` | Sí |
| IPC-T112-PF28 (torreta 2MP) | 1920x1080 | `rtsp://IP:554/media/video1` | No |
| IPC-T314-APKZ (torreta 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Sí |
| IPC-D312-APKZ (domo 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Sí |
| IPC-D314-APKZ (domo 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Sí |
| IPC-E312-APKZ (eyeball 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Sí |
| IPC-P1E2-I (PTZ 2MP) | 1920x1080 | `rtsp://IP:554/media/video1` | Sí |
| IPC-B182-PF28 (bala 4K) | 3840x2160 | `rtsp://IP:554/media/video1` | Sí |

### URLs de Canales del NVR

Para NVRs Uniview (NVR301, NVR302, NVR304, NVR501, NVR516):

| Canal | Flujo Principal | Subflujo |
|-------|----------------|----------|
| Cámara 1 | `rtsp://IP:554/media/video1` | `rtsp://IP:554/media/video2` |
| Cámara 2 | `rtsp://IP:554/media/video3` | `rtsp://IP:554/media/video4` |
| Cámara 3 | `rtsp://IP:554/media/video5` | `rtsp://IP:554/media/video6` |
| Cámara N | `rtsp://IP:554/media/video[2N-1]` | `rtsp://IP:554/media/video[2N]` |

!!! tip "Numeración de Canales del NVR"
    En los NVRs Uniview, el número de flujo de vídeo codifica tanto el canal como el tipo de flujo. Cada canal utiliza dos números consecutivos: impar para flujo principal, par para subflujo. Cámara 1 = video1/video2, Cámara 2 = video3/video4, y así sucesivamente.

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Uniview con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Uniview IPC-B314-APKZ, main stream
var uri = new Uri("rtsp://192.168.1.90:554/media/video1");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `/media/video2` en lugar de `/media/video1`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación digest |
| Captura ONVIF | `http://IP/onvif-http/snapshot?channel=1` | Captura HTTP ONVIF |

## Solución de Problemas

### La contraseña predeterminada debe cambiarse

Las cámaras Uniview con firmware actual requieren que la contraseña predeterminada (`123456`) se cambie durante la configuración inicial. Si aún no ha configurado la cámara:

1. Acceda a la cámara en `http://CAMERA_IP` desde un navegador
2. Complete el asistente de activación
3. Establezca una contraseña segura
4. Use esas credenciales en su URL RTSP

### Formato de URL "unicast" vs "media"

Si `/media/video1` no funciona en su cámara, pruebe el formato unicast: `rtsp://IP:554/unicast/c1/s0/live`. Las versiones de firmware más antiguas de Uniview pueden soportar solo la ruta unicast. El firmware más reciente soporta ambos formatos.

### El flujo H.265 no se reproduce

El códec inteligente U-Code de Uniview produce flujos H.265/HEVC estándar. Si la reproducción H.265 falla:

1. Instale el redistribuible del decodificador HEVC
2. O cambie la cámara a codificación H.264 en la interfaz web: **Setup > Video > Video**
3. Use `rtspSettings.UseGPUDecoder = true` para decodificación H.265 acelerada por hardware

### Problemas de descubrimiento ONVIF

ONVIF está habilitado por defecto en las cámaras Uniview pero puede requerir una contraseña ONVIF separada. Verifique **Setup > Network > ONVIF** en la interfaz web y asegúrese de que la cuenta de usuario ONVIF esté configurada.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Uniview?**

La URL estándar es `rtsp://admin:password@CAMERA_IP:554/media/video1` para el flujo principal. Use `/media/video2` para el subflujo. Algunos modelos más antiguos usan `rtsp://IP:554/unicast/c1/s0/live` en su lugar.

**¿Uniview es lo mismo que UNV?**

Sí. Uniview y UNV son la misma empresa (Zhejiang Uniview Technologies). La marca varía según la región. Todas las cámaras usan firmware, formatos de URL RTSP e interfaces web idénticos independientemente de si llevan la etiqueta Uniview o UNV.

**¿Las cámaras Uniview soportan ONVIF?**

Sí. Todas las cámaras Uniview actuales soportan ONVIF Profile S y Profile T. ONVIF permite el descubrimiento automático de cámaras y acceso estandarizado a flujos sin usar URLs RTSP específicas de la marca.

**¿Cómo accedo a múltiples canales en un NVR Uniview?**

Los NVRs Uniview usan números de flujo de vídeo secuenciales: Cámara 1 = video1 (principal) / video2 (sub), Cámara 2 = video3 (principal) / video4 (sub), y así sucesivamente. La fórmula es: flujo principal = video[2N-1], subflujo = video[2N] donde N es el número de canal de la cámara.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Hikvision](hikvision.md) — Líder mundial del mercado, formato de URL diferente
- [Guía de Conexión Dahua](dahua.md) — Otra importante marca china de vigilancia
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
