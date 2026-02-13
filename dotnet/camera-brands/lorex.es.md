---
title: Cómo Conectar una Cámara IP Lorex en C# .NET
description: Conecta cámaras de seguridad y NVRs Lorex en C# .NET con patrones de URL RTSP y ejemplos de código para modelos LNB, LNE, LNZ y DVR Lorex.
---

# Cómo Conectar una Cámara IP Lorex en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Lorex Technology** (subsidiaria de Dahua Technology a través de FLIR/Lorex) es una importante marca de cámaras de seguridad para consumidores y prosumidores en América del Norte. Las cámaras Lorex son fabricadas principalmente por **Dahua Technology** y se venden bajo la marca Lorex a través de canales minoristas incluyendo Amazon, Costco y Best Buy. Lorex es una de las marcas de cámaras de seguridad más vendidas en Estados Unidos y Canadá.

**Datos clave:**

- **Líneas de producto:** LNB (bullet IP), LNE (domo/torreta IP), LNZ (PTZ IP), LNC (consumo Wi-Fi), IPSC (antigua), serie L (antigua)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (configurado durante la instalación del NVR/cámara); algunos modelos antiguos: admin / admin
- **Soporte ONVIF:** Sí (la mayoría de modelos actuales)
- **Códecs de video:** H.264, H.265 (modelos más nuevos)
- **Base OEM:** Dahua Technology (algunos modelos usan firmware Hikvision)

!!! info "Lorex usa múltiples fuentes OEM"
    La mayoría de las cámaras IP Lorex son fabricadas por Dahua y usan el formato de URL RTSP de Dahua. Sin embargo, algunos modelos Lorex (particularmente LNB2153 y MCNB2153) usan firmware basado en Hikvision con URLs `/Streaming/Channels/`. Verifique ambos formatos de URL si uno no funciona.

## Patrones de URL RTSP

### Modelos Basados en Dahua (La Mayoría de Cámaras IP Lorex)

La mayoría de las cámaras IP Lorex usan el formato de URL de Dahua:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo principal | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Resolución completa |
| Subflujo | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Resolución menor |

### Modelos Basados en Hikvision

Algunos modelos Lorex usan firmware Hikvision:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo principal | `rtsp://IP:554//Streaming/Channels/1` | Resolución completa (note la doble barra) |
| Subflujo | `rtsp://IP:554//Streaming/Channels/2` | Resolución menor |
| H.264 directo | `rtsp://IP:554/ch0_0.h264` | Flujo H.264 directo |

### URLs Específicas por Modelo

| Modelo | URL RTSP | Base OEM | Notas |
|-------|----------|----------|-------|
| LNB2153 | `rtsp://IP:554//Streaming/Channels/1` | Hikvision | Bullet 1080p |
| LNB2184 | `rtsp://IP:554/video.mp4` | Dahua | Bullet 4MP |
| LNE1001 | `rtsp://IP:554/` | Dahua | Domo 1080p |
| LNE3003 | `rtsp://IP:554/video.mp4` | Dahua | Domo 2K |
| LNZ4001 | `rtsp://IP:554/video.mp4` | Dahua | PTZ |
| MCNB2153 | `rtsp://IP:554//Streaming/Channels/1` | Hikvision | Bullet 1080p |

### Formatos de URL Alternativos

Algunas cámaras Lorex también responden a estas URLs:

| Patrón de URL | Notas |
|-------------|-------|
| `rtsp://IP:554/` | Ruta raíz (algunos modelos) |
| `rtsp://IP:554/video.mp4` | Flujo de video |
| `rtsp://IP:554/ch0_0.h264` | H.264 directo |

### Modelos Anteriores

| Modelo | URL | Notas |
|-------|-----|-------|
| Serie IPSC | `rtsp://IP:554/` | Cámaras IP antiguas |
| L23WD | `rtsp://IP:554/` | Inalámbrica antigua |
| IP1240 | `http://IP/GetData.cgi` | Solo HTTP |
| LNC104/116/204 | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Cámaras Wi-Fi, solo HTTP |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Lorex con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido).

### Modelos Basados en Dahua (La Mayoría de Cámaras Lorex)

```csharp
// Lorex camera (Dahua-based), main stream
var uri = new Uri("rtsp://192.168.1.65:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

### Modelos Lorex Basados en Hikvision

```csharp
// Lorex LNB2153 (Hikvision-based), main stream
var uri = new Uri("rtsp://192.168.1.65:554//Streaming/Channels/1");
var username = "admin";
var password = "YourPassword";
```

Consulte la [guía de identificación OEM](#determinar-su-base-oem) en Solución de Problemas para determinar qué formato de URL usa su cámara Lorex.

## URLs de Captura

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/jpg/image.jpg` | La mayoría de cámaras IP Lorex |
| Captura (auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Cámaras Wi-Fi de consumo |
| Captura (cuenta) | `http://IP/snapshot.jpg?account=USER&password=PASS` | Autenticación alternativa |
| GetData | `http://IP/GetData.cgi` | Modelos antiguos |
| Flujo MJPEG | `http://IP/video.mjpg` | MJPEG continuo |

## Solución de Problemas

### Determinar su base OEM

Las cámaras Lorex usan firmware de diferentes fabricantes. Para determinar qué formato de URL usar:

1. Intente primero el **formato Dahua**: `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0`
2. Si eso falla, intente el **formato Hikvision**: `rtsp://IP:554//Streaming/Channels/1`
3. Verifique la interfaz web de la cámara -- las cámaras basadas en Dahua tienen una interfaz web azul/blanca, mientras que las basadas en Hikvision tienen una interfaz gris oscuro/negra

### Acceso por NVR vs cámara directa

- Al conectarse a través de un NVR Lorex, use `channel=N` en el formato de URL de Dahua para seleccionar la cámara
- Al conectarse directamente a una cámara IP, siempre use `channel=1`

### Cámaras Lorex Wi-Fi de consumo (serie LNC)

La serie LNC (LNC104, LNC116, LNC204) son cámaras Wi-Fi de consumo que típicamente no soportan RTSP. Proporcionan URLs de captura HTTP solamente y están diseñadas principalmente para uso con la app Lorex.

### Puerto 9000

Algunas cámaras Lorex muy antiguas usaban el puerto 9000 para streaming en lugar del 554. Si el puerto estándar 554 no funciona en un modelo antiguo, intente: `rtsp://IP:9000/`

## Preguntas Frecuentes

**¿Las cámaras Lorex son iguales a las Dahua?**

La mayoría de las cámaras IP Lorex son fabricadas por Dahua y usan firmware idéntico. El formato de URL RTSP (`cam/realmonitor?channel=1&subtype=0`) es el mismo. Sin embargo, algunos modelos Lorex usan firmware Hikvision. Consulte nuestra [guía de conexión de Dahua](dahua.md) para detalles adicionales.

**¿Cuál es la URL RTSP predeterminada para cámaras Lorex?**

Intente `rtsp://admin:contraseña@IP_CAMARA:554/cam/realmonitor?channel=1&subtype=0` primero (basado en Dahua). Si eso falla, intente `rtsp://admin:contraseña@IP_CAMARA:554//Streaming/Channels/1` (basado en Hikvision).

**¿Puedo usar cámaras Lorex sin el NVR Lorex?**

Sí. Las cámaras IP Lorex con soporte RTSP pueden conectarse directamente usando sus direcciones IP individuales. No necesita el NVR Lorex para la integración con software de terceros.

**¿Las cámaras Lorex soportan ONVIF?**

La mayoría de las cámaras IP Lorex actuales soportan ONVIF. Las cámaras Wi-Fi de consumo (serie LNC) generalmente no lo soportan.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Dahua](dahua.md) — Mismo formato de URL para la mayoría de modelos
- [Guía de Conexión de Amcrest](amcrest.md) — Otro OEM de Dahua
- [Guía de Conexión de Swann](swann.md) — Par en segmento consumo/prosumidor
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
