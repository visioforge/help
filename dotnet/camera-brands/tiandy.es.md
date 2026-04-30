---
title: Cámara IP Tiandy en C# .NET — RTSP, ONVIF Configuración
description: Conecte cámaras IP Tiandy (TC-C, TC-NC, TC-A, TC-R NVR) en C# / .NET vía RTSP y ONVIF. URLs de stream, credenciales, configuración H.265. Ejemplo de código.
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

# Cómo Conectar una Cámara IP Tiandy en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Tiandy Technologies** (Tiandy Technologies Co., Ltd.) es un fabricante chino de videovigilancia con sede en Tianjin, China. Fundada en 1994, Tiandy es uno de los mayores fabricantes de equipos de seguridad de China y se ha expandido rápidamente en mercados internacionales de Asia, Medio Oriente, África y América Latina. Tiandy se especializa en cámaras IP con inteligencia artificial, NVRs y soluciones integradas de gestión de vídeo.

**Datos clave:**

- **Líneas de productos:** TC-C (cámaras IP actuales), TC-NC (IP heredada), TC-A (analítica IA), TC-R (NVRs), TC-NR (grabadores de red)
- **Soporte de protocolos:** RTSP, ONVIF Profile S/T, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / 1111 (modelos antiguos) o admin / admin123 (varía según la región)
- **Soporte ONVIF:** Sí (modelos actuales)
- **Códecs de vídeo:** H.264, H.265 (SuperH.265), MJPEG
- **Características de IA:** Smart H.265+, detección facial, protección perimetral, conteo de personas

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Tiandy utilizan una estructura de URL RTSP basada en canal y flujo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de cámara (1 para cámaras independientes) |
| `subtype` | 0 | Flujo principal (mayor resolución) |
| `subtype` | 1 | Subflujo (menor resolución) |

!!! info "Formato de URL Compatible con Dahua"
    Muchas cámaras Tiandy utilizan el mismo formato de URL RTSP `cam/realmonitor` que las cámaras Dahua. Si está familiarizado con la integración de Dahua, los mismos patrones de URL pueden funcionar con Tiandy. Consulte nuestra [guía de conexión Dahua](dahua.md) para más detalles.

### Formatos de URL Alternativos

| Patrón de URL | Descripción |
|---------------|-------------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Compatible con Dahua (muchos modelos) |
| `rtsp://IP:554/live/ch0` | Flujo principal (formato heredado) |
| `rtsp://IP:554/live/ch1` | Subflujo (formato heredado) |
| `rtsp://IP:554/media/video1` | Compatible con Uniview (algunos modelos) |
| `rtsp://IP:554/Streaming/Channels/101` | Compatible con Hikvision (algunos modelos OEM) |
| `rtsp://IP:554/h264` | Ruta simple de flujo H.264 |

### Modelos de Cámaras

| Serie de Modelo | Resolución | URL de Flujo Principal | Audio |
|----------------|-----------|----------------------|-------|
| TC-C32JN (bala 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C34JN (bala 4MP) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C35JN (bala 5MP) | 2592x1944 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C38JN (bala 4K) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| TC-C32DN (domo 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C34DN (domo 4MP) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| TC-C32EP (torreta 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| TC-C34EP (torreta 4MP) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| TC-A32E2T (IA 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| TC-C32WP (WiFi 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |

### URLs de Canales del NVR

Para NVRs Tiandy (TC-R3100, TC-R3200, series TC-NR):

| Canal | Flujo Principal | Subflujo |
|-------|----------------|----------|
| Cámara 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Cámara 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Cámara N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Tiandy con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Tiandy TC-C34JN (4MP bullet), main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `subtype=1` en lugar de `subtype=0`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requiere autenticación digest |
| Flujo MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | MJPEG continuo |

## Solución de Problemas

### Múltiples formatos de URL

Las cámaras Tiandy pueden usar diferentes formatos de URL RTSP dependiendo de la versión de firmware y el modelo. Si un formato no funciona, pruebe las alternativas en este orden:

1. `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` (compatible con Dahua, más común)
2. `rtsp://IP:554/live/ch0` (formato Tiandy heredado)
3. `rtsp://IP:554/h264` (ruta simple)

### Las credenciales predeterminadas varían

Las contraseñas predeterminadas de Tiandy difieren según el modelo y la región. Los valores predeterminados comunes incluyen:

- `admin` / `1111`
- `admin` / `admin123`
- `admin` / `123456`

Si ninguno de estos funciona, la cámara puede requerir activación inicial a través de la interfaz web o la utilidad EasyLive de Tiandy.

### Códec SuperH.265

SuperH.265 de Tiandy es una optimización propietaria que produce flujos H.265/HEVC estándar. No se requiere ningún decodificador especial. El VisioForge SDK maneja los flujos H.265 de forma nativa.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Tiandy?**

La mayoría de las cámaras Tiandy usan `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` para el flujo principal, que es el mismo formato que las cámaras Dahua. Algunos modelos más antiguos usan `rtsp://IP:554/live/ch0` en su lugar.

**¿Las cámaras Tiandy son OEMs de Dahua?**

No. Tiandy es un fabricante independiente con su propio hardware y firmware. Sin embargo, algunos firmware de Tiandy usan el mismo formato de URL RTSP que Dahua (`cam/realmonitor`), lo cual es común entre varios fabricantes chinos de vigilancia.

**¿Las cámaras Tiandy soportan ONVIF?**

Sí. Los modelos actuales de Tiandy soportan ONVIF Profile S y Profile T. ONVIF debe habilitarse en la interfaz web de la cámara en la configuración de red. Algunos modelos requieren crear una cuenta de usuario ONVIF separada.

**¿Qué serie de cámaras Tiandy debería elegir?**

**TC-C** es la serie mainstream actual. El número después de "TC-C3" indica la resolución: **2** = 2MP, **4** = 4MP, **5** = 5MP, **8** = 4K. Las letras del sufijo indican el factor de forma: **JN** = bala, **DN** = domo, **EP** = torreta/eyeball, **WP** = WiFi.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Dahua](dahua.md) — Formato de URL similar
- [Guía de Conexión Uniview](uniview.md) — Otra importante marca china de vigilancia
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
