---
title: Cámaras IP CP Plus: guía de conexión RTSP en C# .NET
description: Conecte cámaras CP Plus en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series UNC, NC, RNP, Guard+ y Cosmic.
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

# Cómo conectar una cámara IP CP Plus en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**CP Plus** (Aditya Infotech Ltd.) es la marca #1 de cámaras de seguridad de India y uno de los mayores fabricantes de vigilancia del mundo, con sede en Delhi, India. Las cámaras CP Plus son principalmente productos **OEM de Dahua**, lo que significa que la mayoría de modelos ejecutan firmware Dahua y comparten patrones de URL RTSP idénticos. Algunos modelos usan chipsets alternativos con diferentes formatos de URL. CP Plus domina los mercados de India, Medio Oriente y el Sudeste Asiático con una gama completa de cámaras IP, NVRs y sistemas analógicos.

**Datos clave:**

- **Líneas de productos:** UNC (cámaras IP), RNP (NVRs), VAC (analógico), Guard+ (inalámbrico), E series (nivel de entrada), Cosmic (económico)
- **Base OEM:** Dahua (la mayoría de modelos usan firmware y patrones de URL de Dahua)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (mayoría de modelos)
- **Códecs de video:** H.264, H.265 (modelos más nuevos)
- **Mercado dominante:** India (#1), Medio Oriente, Sudeste Asiático

!!! info "CP Plus = OEM de Dahua"
    Las cámaras CP Plus son principalmente productos OEM de Dahua y usan los mismos patrones de URL RTSP. Consulte nuestra [guía de conexión de Dahua](dahua.md) para detalles adicionales. Algunos modelos CP Plus usan el formato de URL `/VideoInput/` en su lugar.

## Patrones de URL RTSP

### Formato de URL Estándar (estilo Dahua)

La mayoría de cámaras CP Plus usan el patrón de URL `cam/realmonitor` de Dahua:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=1
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de cámara (1 para cámaras independientes) |
| `subtype` | 0 | Flujo principal (resolución más alta) |
| `subtype` | 1 | Subflujo (resolución más baja, menos ancho de banda) |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|------------------------|-------|
| CP-UNC-DP10L2C | IP domo | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | URL estilo Dahua |
| CP-UNC-TY20FL2C | IP torreta | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | URL estilo Dahua |
| CP-NC9W-K | Cámara de red | `rtsp://IP:554/VideoInput/1/mpeg4/1` | Formato VideoInput |
| B series | Básica | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | URL estilo Dahua |

### URLs de Canal NVR

Para NVRs CP Plus (CP-RNP-36D, CN-RNP-36D, etc.):

| Canal | Flujo Principal | Subflujo |
|-------|-----------------|----------|
| Cámara 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Cámara 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Cámara N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

### Formatos de URL Alternativos

Algunos modelos CP Plus usan diferentes patrones de URL dependiendo del firmware y chipset:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Estilo Dahua estándar (mayoría de modelos) |
| `rtsp://IP:554//cam/realmonitor` | Alternativa estilo Dahua (doble barra) |
| `rtsp://IP:554/VideoInput/1/mpeg4/1` | Formato VideoInput (CP-NC9W-K, CP-UNC-DP10L2C en algún firmware) |
| `rtsp://IP:554//cam/realmonitor?channel=1&subtype=00&authbasic=AUTH` | Con autenticación codificada en base64 |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara CP Plus con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// CP Plus CP-UNC-DP10L2C, main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `subtype=1` en lugar de `subtype=0`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi?1` | Requiere autenticación básica (CP-UNC-TY20FL2C) |
| Captura JPEG (legacy) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Auth basada en URL para modelos más antiguos |
| Imagen JPEG | `http://IP/cgi-bin/jpg/image.cgi` | Endpoint JPEG alternativo |
| Flujo MJPEG | `http://IP/api/mjpegvideo.cgi?InputNumber=1&StreamNumber=CHANNEL` | Flujo MJPEG continuo |
| Flujo HTTP Directo | `http://IP:8008/` | Algunos modelos NVR (CN-RNP-36D, CP-RNP-36D) |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras CP Plus se envían con credenciales predeterminadas (`admin` / `admin`). Si ha cambiado la contraseña a través de la interfaz web o la app móvil, asegúrese de que su URL RTSP use las credenciales actualizadas.

1. Acceda a la cámara en `http://CAMERA_IP` en un navegador
2. Inicie sesión con sus credenciales
3. Verifique que RTSP esté habilitado bajo **Setup > Network > Port**
4. Use esas credenciales en su URL RTSP

### La URL estilo Dahua no funciona

Algunos modelos CP Plus (particularmente la serie CP-NC) usan el formato de URL `/VideoInput/` en lugar del patrón `cam/realmonitor` de Dahua. Pruebe:

```
rtsp://admin:password@IP:554/VideoInput/1/mpeg4/1
```

### Puerto 554 vs puerto personalizado

Verifique la configuración del puerto RTSP en:

- Interfaz web: **Setup > Network > Port > RTSP Port**
- El predeterminado es 554

### Confusión de tipo de flujo

- `subtype=0` = Flujo principal (resolución completa, mayor ancho de banda)
- `subtype=1` = Subflujo (resolución reducida, menor ancho de banda)

### Flujo HTTP directo en puerto 8008

Algunos modelos NVR CP Plus (CP-RNP-36D, CN-RNP-36D) exponen un flujo HTTP directo en el puerto 8008. Pruebe acceder a `http://CAMERA_IP:8008/` si el RTSP estándar no está disponible.

## Preguntas Frecuentes

**¿Las cámaras CP Plus son iguales que Dahua?**

La mayoría de cámaras CP Plus son fabricadas por Dahua y usan firmware Dahua. El formato de URL RTSP (`cam/realmonitor?channel=1&subtype=0`) es idéntico para la mayoría de modelos. Sin embargo, algunos modelos CP Plus usan chipsets diferentes con el formato de URL `/VideoInput/`. Cualquier código escrito para cámaras Dahua generalmente funciona con CP Plus y viceversa.

**¿Cuál es la URL RTSP predeterminada para las cámaras CP Plus?**

La URL es `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=1` para el subflujo. Reemplace `subtype=1` con `subtype=0` para el flujo principal. Para configuraciones NVR, cambie `channel=1` al número de canal apropiado.

**¿Las cámaras CP Plus soportan ONVIF?**

Sí. La mayoría de cámaras IP CP Plus actuales soportan ONVIF, lo que proporciona una forma estandarizada de descubrir y conectarse a cámaras independientemente del formato de URL específico.

**¿Qué pasa si mi cámara CP Plus usa un formato de URL diferente?**

Algunos modelos CP Plus (especialmente la serie CP-NC) usan `rtsp://IP:554/VideoInput/1/mpeg4/1` en lugar de la URL estilo Dahua. Si la URL estándar no funciona, pruebe el formato VideoInput. También puede usar el descubrimiento ONVIF para detectar automáticamente la URL correcta.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Dahua](dahua.md) — Mismo formato de URL para la mayoría de modelos
- [Guía de Conexión de Amcrest](amcrest.md) — Otro OEM de Dahua
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
