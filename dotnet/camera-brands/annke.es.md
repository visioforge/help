---
title: Cámara IP Annke - URLs RTSP y Ejemplos de Código C# .NET
description: Conecte cámaras Annke en C# .NET con patrones de URL RTSP y ejemplos de código para modelos C500, C800, CZ400, NC400 y NVR.
---

# Cómo Conectar una Cámara IP Annke en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Annke** (Annke Innovation Co., Ltd.) es una marca de cámaras de seguridad para consumidores y prosumidores con sede en Hong Kong, que vende principalmente a través de Amazon y canales directos al consumidor. Las cámaras Annke se fabrican utilizando hardware OEM de **Hikvision**, y la mayoría de los modelos usan firmware y patrones de URL RTSP compatibles con Hikvision. Annke ofrece precios competitivos en cámaras PoE, NVRs y kits completos de vigilancia.

**Datos clave:**

- **Líneas de productos:** Serie C (cámaras IP), Serie CZ (PTZ), Serie NC (NVRs), Serie I (torreta/domo)
- **Soporte de protocolos:** RTSP, ONVIF Profile S, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (se establece durante la configuración inicial; algunos modelos: admin / admin)
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de vídeo:** H.264, H.265 (4MP y superior)
- **Base OEM:** Hikvision (la mayoría de los modelos usan firmware compatible con Hikvision)

!!! info "Annke Usa Firmware de Hikvision"
    La mayoría de las cámaras Annke usan firmware OEM de Hikvision. El formato de URL RTSP (`/Streaming/Channels/`) es idéntico al de Hikvision. Consulte nuestra [guía de conexión Hikvision](hikvision.md) para detalles adicionales y solución de problemas.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Annke utilizan el patrón de URL `Streaming/Channels` de Hikvision:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/Streaming/Channels/[CHANNEL_ID]
```

| ID de Canal | Flujo | Descripción |
|------------|-------|-------------|
| 101 | Flujo principal | Resolución completa |
| 102 | Subflujo | Menor resolución |
| 103 | Tercer flujo | Optimizado para móvil (si es compatible) |

### Modelos de Cámaras

| Modelo | Resolución | URL de Flujo Principal | Audio |
|--------|-----------|----------------------|-------|
| C500 (bala 5MP) | 2592x1944 | `rtsp://IP:554/Streaming/Channels/101` | Sí |
| C800 (bala 4K) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Sí |
| C1200 (bala 12MP) | 4000x3000 | `rtsp://IP:554/Streaming/Channels/101` | Sí |
| CZ400 (PTZ 4MP) | 2560x1440 | `rtsp://IP:554/Streaming/Channels/101` | Sí |
| I91BN (torreta 4K) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Sí |
| I91BM (domo 4K) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Sí |
| NC400 (NVR 4ch) | N/A | Consulte la sección NVR | N/A |
| N48PAW (NVR PoE 8ch) | N/A | Consulte la sección NVR | N/A |

### URLs de Canales del NVR

Para NVRs Annke (NC400, N48PAW, N46PCK, etc.):

| Canal | Flujo Principal | Subflujo |
|-------|----------------|----------|
| Cámara 1 | `rtsp://IP:554/Streaming/Channels/101` | `rtsp://IP:554/Streaming/Channels/102` |
| Cámara 2 | `rtsp://IP:554/Streaming/Channels/201` | `rtsp://IP:554/Streaming/Channels/202` |
| Cámara N | `rtsp://IP:554/Streaming/Channels/N01` | `rtsp://IP:554/Streaming/Channels/N02` |

### Formatos de URL Alternativos

Algunos modelos Annke (especialmente variantes OEM no Hikvision) usan patrones de URL diferentes:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/Streaming/Channels/101` | Estilo Hikvision (la mayoría de modelos) |
| `rtsp://IP:554/h264/ch1/main/av_stream` | Firmware Hikvision antiguo |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Estilo Dahua (algunos modelos antiguos) |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Annke con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Annke C800 (4K bullet), main stream
var uri = new Uri("rtsp://192.168.1.90:554/Streaming/Channels/101");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `/Streaming/Channels/102` en su lugar.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/ISAPI/Streaming/channels/101/picture` | Requiere autenticación digest |
| Flujo MJPEG | `http://IP/ISAPI/Streaming/channels/102/httpPreview` | Subflujo como MJPEG |
| Captura Heredada | `http://IP/Streaming/channels/1/picture` | Firmware antiguo |

## Solución de Problemas

### La cámara requiere activación

Las cámaras Annke con firmware más reciente requieren activación inicial (configuración de contraseña) antes de que funcione el acceso RTSP. Use la interfaz web de la cámara en `http://CAMERA_IP` o la herramienta de descubrimiento compatible con SADP de Annke.

### El formato de URL de Hikvision no funciona

Algunos modelos Annke usan firmware OEM diferente. Si `/Streaming/Channels/101` no funciona, pruebe:

1. `/h264/ch1/main/av_stream` (firmware Hikvision antiguo)
2. `/cam/realmonitor?channel=1&subtype=0` (estilo Dahua)
3. Use el descubrimiento ONVIF para recuperar automáticamente la URL de flujo correcta

### Problemas con flujo H.265

Las cámaras Annke 4K (C800, I91BN) tienen H.265 como codificación predeterminada. Si la reproducción falla, cambie la cámara a H.264 en la interfaz web o instale el redistribuible del decodificador HEVC.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Annke?**

La mayoría de las cámaras Annke usan `rtsp://admin:password@CAMERA_IP:554/Streaming/Channels/101` para el flujo principal. Use el canal `102` para el subflujo. Este es el mismo formato que las cámaras Hikvision.

**¿Las cámaras Annke son OEMs de Hikvision?**

La mayoría de las cámaras Annke usan hardware y firmware OEM de Hikvision. El formato de URL RTSP, la interfaz web y la API son normalmente idénticos a Hikvision. Algunos modelos Annke pueden usar bases OEM diferentes.

**¿Las cámaras Annke soportan ONVIF?**

Sí. Todas las cámaras Annke actuales soportan ONVIF Profile S, proporcionando descubrimiento y acceso a flujos estandarizados.

**¿Puedo mezclar cámaras Annke con NVRs Hikvision?**

Sí. Dado que las cámaras Annke usan protocolos compatibles con Hikvision, funcionan nativamente con NVRs Hikvision y viceversa. También puede mezclar cámaras Annke en cualquier NVR o VMS compatible con ONVIF.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Hikvision](hikvision.md) — Mismo formato de URL (base OEM)
- [Guía de Conexión LTS](lts.md) — Otro OEM de Hikvision
- [Guía de Conexión Dahua](dahua.md) — Ecosistema OEM alternativo
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
