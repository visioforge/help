---
title: Cámara IP LTS - URL RTSP y guía de conexión en C# .NET
description: Conecte cámaras LTS (LT Security) en C# .NET con patrones de URL RTSP y ejemplos de código para series CMIP, CMHR y modelos NVR. LTS usa firmware Hikvision.
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

# Cómo conectar una cámara IP LTS en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**LTS (LT Security Inc.)** es una empresa de seguridad estadounidense con sede en City of Industry, California. Las cámaras LTS son fabricadas por **Hikvision** y usan firmware, protocolos e interfaces web de Hikvision. LTS redistribuye hardware Hikvision con soporte técnico basado en EE.UU. y precios competitivos, lo que la convierte en una opción popular en el mercado de instalación profesional.

Debido a que las cámaras LTS ejecutan firmware Hikvision, usan el mismo formato de URL RTSP, implementación ONVIF y endpoints de API que las cámaras Hikvision. Cualquier código de integración escrito para Hikvision funciona con LTS y viceversa.

**Datos clave:**

- **Líneas de productos:** CMIP (cámaras IP), CMHR (analógico HD-TVI), LTD (DVRs), LTN (NVRs)
- **Soporte de protocolos:** RTSP, ONVIF Profile S/G/T, HTTP/ISAPI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / 123456 (algunos modelos: admin / admin)
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de video:** H.264, H.265 (CMIP4xxx y posteriores)
- **Base OEM:** Hikvision (formato de URL RTSP idéntico)

!!! info "LTS = OEM de Hikvision"
    Las cámaras LTS usan firmware Hikvision y el mismo formato de URL RTSP exacto que las cámaras Hikvision. Cualquier código escrito para cámaras Hikvision funciona con LTS. Consulte nuestra [guía de conexión de Hikvision](hikvision.md) para detalles adicionales.

## Patrones de URL RTSP

### Formato de URL Estándar (Hikvision)

La mayoría de cámaras LTS actuales usan el patrón de URL estándar de Hikvision `Streaming/Channels`:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/Streaming/Channels/[CHANNEL_ID]
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `CHANNEL_ID` | 101 | Canal 1, flujo principal |
| `CHANNEL_ID` | 102 | Canal 1, subflujo |
| `CHANNEL_ID` | 201 | Canal 2, flujo principal (NVR) |
| `CHANNEL_ID` | 202 | Canal 2, subflujo (NVR) |

!!! note "Doble barra en la URL"
    Algunas cámaras LTS/Hikvision usan `//Streaming/Channels/1` (con doble barra diagonal antes de `Streaming`). Tanto las variantes con una barra como con doble barra típicamente funcionan, pero pruebe la versión con doble barra si la URL con una sola barra falla.

### URLs RTSP por Modelo

| Modelo | URL RTSP | Resolución | Notas |
|--------|----------|------------|-------|
| CMIP3122 | `rtsp://IP:554/Streaming/Channels/101` | 3MP | Formato estándar Hikvision |
| CMIP3132-28 | `rtsp://IP:554/Streaming/Channels/101` | 3MP | Formato estándar Hikvision |
| CMIP3432 | `rtsp://IP:554/Streaming/Channels/101` | 4MP | Formato estándar Hikvision |
| CMIP3243 | `rtsp://IP:554/live.h264` | 3MP | Flujo H.264 alternativo |
| CMIP3412-28 | `rtsp://IP:554/live.h264` | 4MP | Flujo H.264 alternativo |
| CMIP8232 | `rtsp://IP:554/live.sdp` | 8MP/4K | Flujo SDP en vivo |
| CMIP8232 (alt) | `rtsp://IP:554/HighResolutionVideo` | 8MP/4K | Flujo de alta resolución |
| CMIP8232 (sub) | `rtsp://IP:554/h264/ch1/sub/` | 8MP/4K | Subflujo H.264 |
| Serie CMIP (baja res) | `rtsp://IP:554/LowResolutionVideo` | Varía | Subflujo de baja resolución |

### Formatos de URL Alternativos

Algunos modelos LTS más antiguos o versiones de firmware específicas soportan estas URLs alternativas:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/Streaming/Channels/101` | Hikvision estándar (recomendado) |
| `rtsp://IP:554//Streaming/Channels/1` | Variante con doble barra |
| `rtsp://IP:554/live.h264` | Flujo H.264 en vivo (CMIP3xxx más antiguo) |
| `rtsp://IP:554/live.sdp` | Flujo SDP en vivo (CMIP8xxx) |
| `rtsp://IP:554/HighResolutionVideo` | Flujo de alta resolución nombrado |
| `rtsp://IP:554/LowResolutionVideo` | Flujo de baja resolución nombrado |
| `rtsp://IP:554/h264/ch1/sub/` | Subflujo H.264 por canal |
| `rtsp://IP:554/cam1/mpeg4?user=USER&pwd=PASS` | MPEG-4 con auth basada en URL |

### URLs de Canal NVR (Serie LTN)

Para NVRs LTS (LTN8704, LTN8708, LTN8716, etc.):

| Canal | Flujo Principal | Subflujo |
|-------|-----------------|----------|
| Cámara 1 | `rtsp://IP:554/Streaming/Channels/101` | `rtsp://IP:554/Streaming/Channels/102` |
| Cámara 2 | `rtsp://IP:554/Streaming/Channels/201` | `rtsp://IP:554/Streaming/Channels/202` |
| Cámara N | `rtsp://IP:554/Streaming/Channels/N01` | `rtsp://IP:554/Streaming/Channels/N02` |

### Resumen de Series de Modelos

| Serie de Modelo | URL RTSP Principal | URLs Alternativas |
|-----------------|--------------------|--------------------|
| CMIP3xxx (3MP) | `/Streaming/Channels/101` | `/live.h264` (algunos modelos) |
| CMIP4xxx (4MP) | `/Streaming/Channels/101` | `/live.h264` (algunos modelos) |
| CMIP8xxx (8MP/4K) | `/Streaming/Channels/101` | `/live.sdp`, `/HighResolutionVideo`, `/h264/ch1/sub/` |
| NVRs LTN | `/Streaming/Channels/N01` | Basado en canal |
| DVRs LTD | `/Streaming/Channels/N01` | Basado en canal |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara LTS con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// LTS CMIP3432, main stream (Hikvision format)
var uri = new Uri("rtsp://192.168.1.80:554/Streaming/Channels/101");
var username = "admin";
var password = "123456";
```

Para acceder al subflujo, use el ID de canal `102` en lugar de `101`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/snapshot.jpg` | Captura estándar |
| Captura 3GP | `http://IP/snapshot_3gp.jpg` | Formato 3GP (optimizado para móvil) |
| Captura de Flujo | `http://IP/stream.jpg` | Captura basada en flujo |
| Captura de Canal DVR | `http://IP/stillimg[CHANNEL].jpg` | Reemplace `[CHANNEL]` con número de canal (DVRs LTD) |
| Captura ISAPI | `http://IP/ISAPI/Streaming/channels/101/picture` | Hikvision ISAPI (requiere auth) |

## Solución de Problemas

### Identificar el formato de URL correcto

Las cámaras LTS abarcan múltiples generaciones con diferentes formatos de URL. Para determinar qué URL usa su cámara:

1. Pruebe primero el formato estándar Hikvision: `rtsp://IP:554/Streaming/Channels/101`
2. Si falla, pruebe la variante con doble barra: `rtsp://IP:554//Streaming/Channels/1`
3. Para modelos CMIP3xxx más antiguos, pruebe `rtsp://IP:554/live.h264`
4. Para modelos CMIP8xxx (4K), pruebe `rtsp://IP:554/live.sdp`

### Credenciales predeterminadas y activación

- Cámaras LTS más antiguas: la contraseña predeterminada es `123456` o `admin`
- Cámaras LTS más nuevas (con firmware Hikvision 5.3+): requieren activación de contraseña en el primer uso, similar a Hikvision
- Si no puede iniciar sesión con credenciales predeterminadas, la cámara puede necesitar activarse vía la Herramienta de Descubrimiento LTS o la Herramienta SADP de Hikvision

### Uso de herramientas Hikvision con cámaras LTS

Dado que las cámaras LTS ejecutan firmware Hikvision, puede usar utilidades de Hikvision para descubrimiento y configuración de red:

- **Herramienta SADP de Hikvision** -- descubre cámaras LTS en la red local y puede activarlas/resetearlas
- **Herramienta de Descubrimiento LTS** -- versión con marca LTS de SADP con funcionalidad idéntica
- **iVMS-4200** -- software VMS gratuito de Hikvision funciona con cámaras LTS

### Error "401 Unauthorized"

1. Verifique que sus credenciales sean correctas (predeterminado: admin / 123456)
2. En firmware más nuevo, asegúrese de que la cámara haya sido activada y esté usando la contraseña establecida durante la activación
3. Verifique si la cámara tiene una política de bloqueo -- demasiados intentos de inicio de sesión fallidos pueden bloquear temporalmente el acceso
4. Algunos modelos requieren autenticación digest en lugar de autenticación básica para RTSP

### Problema de URL con doble barra

La URL `//Streaming/Channels/1` con doble barra diagonal al inicio es un patrón conocido de Hikvision. Algunos clientes HTTP o bibliotecas RTSP pueden normalizar esto a una sola barra. Si su conexión falla:

- Asegúrese de que su cadena de URL preserve la doble barra
- Pruebe ambas variantes `//Streaming/Channels/1` y `/Streaming/Channels/101`

## Preguntas Frecuentes

**¿Las cámaras LTS son iguales que Hikvision?**

Las cámaras LTS son fabricadas por Hikvision y ejecutan firmware Hikvision. El formato de URL RTSP (`/Streaming/Channels/101`), la implementación ONVIF y la interfaz ISAPI son idénticos. Las principales diferencias son la marca, el precio y el soporte técnico basado en EE.UU. de LTS. Cualquier código escrito para cámaras Hikvision funciona con cámaras LTS.

**¿Cuál es la URL RTSP predeterminada para las cámaras LTS?**

Para la mayoría de cámaras LTS actuales, use `rtsp://admin:123456@CAMERA_IP:554/Streaming/Channels/101` para el flujo principal. Use el ID de canal `102` para el subflujo. Los modelos más antiguos pueden usar `/live.h264` o `/live.sdp` en su lugar.

**¿Las cámaras LTS soportan ONVIF?**

Sí. Todas las cámaras IP LTS actuales (serie CMIP) soportan ONVIF Profile S y Profile T. ONVIF puede usarse para descubrimiento y configuración automática junto con URLs RTSP directas.

**¿Cuál es la diferencia entre las series CMIP y CMHR?**

Las cámaras CMIP son cámaras IP (de red) que soportan transmisión RTSP. Las cámaras CMHR son cámaras analógicas HD-TVI que se conectan directamente a DVRs vía cable coaxial y no tienen capacidad RTSP de red. Solo las cámaras de la serie CMIP pueden conectarse vía URLs RTSP en software.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Hikvision](hikvision.md) — Mismo formato de URL (base OEM)
- [Guía de Conexión de Annke](annke.md) — Otro OEM de Hikvision
- [Guía de Integración de Cámara RTSP](../videocapture/video-sources/ip-cameras/rtsp.md) — Configuración de flujo RTSP LTS
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
