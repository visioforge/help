---
title: Cómo conectar una cámara IP Speco Technologies en C# .NET
description: Conecte cámaras y DVRs Speco Technologies en C# .NET con patrones de URL RTSP y ejemplos de código para modelos O Series, VIP, LS, ZIP, SIP y DVR.
---

# Cómo conectar una cámara IP Speco Technologies en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Speco Technologies** es una empresa estadounidense de vigilancia profesional con sede en Amityville, Nueva York. Fundada en 1969, Speco fabrica cámaras IP, cámaras analógicas, DVRs, NVRs y equipos de control de acceso para el mercado de integradores de seguridad profesional. Los productos Speco se venden a través de distribuidores autorizados e integradores de seguridad en lugar de canales directos al consumidor, lo que los convierte en una opción común en instalaciones comerciales.

**Datos clave:**

- **Líneas de productos:** O Series (cámaras IP: O2B, O2D, OINT), VIP Series (cámaras IP), ZIP Series, SIP Series, LS Series, líneas DVR (TH/TL, RS, PCPRO)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (todas las cámaras IP actuales)
- **Códecs de video:** H.264 (todos los modelos actuales), MPEG-4 (modelos más antiguos)

!!! info "Múltiples Líneas de Productos"
    Speco Technologies tiene muchas líneas de productos distintas, cada una con diferentes formatos de URL RTSP. Identifique su serie de modelo exacta (O, VIP, LS, ZIP, SIP o tipo de DVR) antes de configurar la URL del flujo. El flujo raíz `rtsp://IP:554/` funciona en muchos dispositivos Speco como prueba rápida.

## Patrones de URL RTSP

### Cámaras IP O Series

La O Series es la línea actual de cámaras IP de Speco, incluyendo modelos bala (O2B), domo (O2D) e intensificador (OINT):

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/
```

| Modelo | Resolución | URL de Flujo Principal | Notas |
|--------|------------|------------------------|-------|
| O2B2 (bala) | 1080p | `rtsp://IP:554/` | Flujo raíz |
| O2D4 (domo) | 1080p | `rtsp://IP:554/` | Flujo raíz |
| OINT56B1G (intensificador) | 1080p | `rtsp://IP:554/mpeg4` | Flujo MPEG-4 |
| OINT56B1G (intensificador) | 1080p | `rtsp://IP:554/` | Flujo raíz (H.264) |

### Cámaras IP VIP Series

La VIP Series usa un formato de ruta de flujo numerado:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/1/stream1
```

| Modelo | Resolución | URL de Flujo Principal | Notas |
|--------|------------|------------------------|-------|
| VIP2B1M (bala) | 1080p | `rtsp://IP:554/1/stream1` | Stream 1 (principal) |
| VIP2C1N (cubo) | 1080p | `rtsp://IP:554/1/stream1` | Stream 1 (principal) |

### LS Series

La LS Series usa una ruta H.264 basada en canal y también soporta un formato de credenciales en URL:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/cam1/h264` | Flujo H.264 canal 1 |
| `rtsp://IP:554/cam2/h264` | Flujo H.264 canal 2 |
| `rtsp://IP:554/cam[N]/h264` | Flujo H.264 canal N |
| `rtsp://IP:554//user=admin_password=tlJwpbo6_channel=1_stream=0.sdp` | Formato de credenciales en URL |

!!! warning "Formato de Credenciales de LS Series"
    La LS Series soporta un formato inusual de credenciales en URL donde el nombre de usuario y contraseña están incrustados directamente en la ruta. La contraseña en este formato es específica del dispositivo y puede diferir de la contraseña de la interfaz web. Verifique la página de configuración RTSP del dispositivo para el valor correcto.

### ZIP Series

La ZIP Series usa un formato de transmisión basado en perfil:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//stream0/Channel=0;Profile=0
```

| Modelo | URL de Flujo Principal | Notas |
|--------|------------------------|-------|
| ZIP2B (bala) | `rtsp://IP:554//stream0/Channel=0;Profile=0` | Perfil 0 (principal) |

### Modelos DVR

Los DVRs Speco usan varios formatos de URL dependiendo de la línea de DVR:

| Serie DVR | Patrón de URL | Notas |
|-----------|---------------|-------|
| DVR4WM | `rtsp://IP:554/` | Flujo raíz |
| RS Series | `rtsp://IP:554/Live/Channel=1` | Formato de canal en vivo |
| RS Series | `rtsp://IP:554/Live/Channel=2` | Canal 2 |
| DVRs generales | `rtsp://IP:554/` | Flujo raíz (alternativa) |

### URLs de Canal DVR (RS Series)

Para DVRs Speco RS Series:

| Canal | URL de Flujo Principal |
|-------|------------------------|
| Cámara 1 | `rtsp://IP:554/Live/Channel=1` |
| Cámara 2 | `rtsp://IP:554/Live/Channel=2` |
| Cámara N | `rtsp://IP:554/Live/Channel=N` |

### Resumen de Todos los Formatos de URL

| Patrón de URL | Línea de Producto | Notas |
|---------------|-------------------|-------|
| `rtsp://IP:554/` | O Series, DVRs, general | Flujo raíz (funciona en muchos dispositivos) |
| `rtsp://IP:554/mpeg4` | O Series (más antiguo) | Flujo MPEG-4 |
| `rtsp://IP:554/1/stream1` | VIP Series | Formato de flujo numerado |
| `rtsp://IP:554/cam[N]/h264` | LS Series | H.264 basado en canal |
| `rtsp://IP:554//stream0/Channel=0;Profile=0` | ZIP Series | Formato basado en perfil |
| `rtsp://IP:554/Live/Channel=N` | RS DVR | Formato de canal en vivo |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Speco con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Speco O2D4 dome camera, root stream
var uri = new Uri("rtsp://192.168.1.64:554/");
var username = "admin";
var password = "admin";
```

Para cámaras VIP Series, use la ruta `/1/stream1` en su lugar:

```csharp
// Speco VIP2B1M bullet camera, main stream
var uri = new Uri("rtsp://192.168.1.64:554/1/stream1");
var username = "admin";
var password = "admin";
```

## URLs de Captura y MJPEG

### Capturas de Cámaras IP

| Tipo | Patrón de URL | Modelos | Notas |
|------|---------------|---------|-------|
| Imagen Fija | `http://IP/stillimg.jpg` | O2B2, O2D4, OINT56B1G | Captura JPEG básica |
| Imagen Fija (puerto 554) | `http://IP:554/stillimg.jpg` | O2B2, O2D4, OINT56B1G | Puerto alternativo |
| Captura del Codificador | `http://IP/cgi-bin/encoder?USER=user&PWD=pass&SNAPSHOT` | IP-SD10X, SIP Series | Basada en CGI con credenciales |
| Flujo del Sistema | `http://IP/cgi-bin/cmd/system?GET_STREAM&USER=user&PWD=pass` | Varias cámaras IP | Formato de comando del sistema |

### Capturas de DVR

| Tipo | Patrón de URL | Serie DVR | Notas |
|------|---------------|-----------|-------|
| Imagen Completa | `http://IP/images1full` | Varios DVRs | Reemplace `1` con número de canal |
| Imagen SIF | `http://IP/images1sif` | Varios DVRs | Resolución más baja, reemplace `1` con canal |
| Obtener Imagen | `http://IP/getimage?camera=1&fmt=full` | Varios DVRs | Reemplace `1` con número de canal |
| Captura Móvil | `http://IP/mobile/channel1.jpg` | PCPRO DVR | Optimizada para móvil, reemplace `1` con canal |
| Flujo TH/TL | `http://IP/ivop.get?action=live&THREAD_ID=` | TH/TL DVR | Flujo en vivo vía HTTP |

## Solución de Problemas

### Formatos de URL inconsistentes entre líneas de productos

Speco Technologies tiene muchas líneas de productos diferentes, cada una con su propio formato de URL RTSP. Si un patrón de URL no funciona, pruebe primero el flujo raíz `rtsp://IP:554/` como prueba base. Luego pruebe el formato específico para su línea de productos como se lista en las tablas anteriores.

### Limitaciones del flujo raíz

El flujo raíz (`rtsp://IP:554/`) funciona en muchos dispositivos Speco para el flujo principal pero no es confiable para acceder a subflujos o canales específicos en dispositivos multicanal. Use el formato de URL específico de la línea de productos para control completo sobre la selección de flujo.

### Formato de credenciales en URL de LS Series

La LS Series usa un formato de URL inusual donde las credenciales están incrustadas en la ruta (`/user=admin_password=VALUE_channel=1_stream=0.sdp`). La contraseña en este formato puede ser un valor generado por el dispositivo que difiere de la contraseña de la interfaz web. Verifique la página de **Configuración RTSP** del dispositivo en la interfaz web para la cadena de credenciales correcta.

### Descubrimiento de red

Speco proporciona una herramienta DDNS y utilidad de descubrimiento de dispositivos para encontrar cámaras en la red. Descargue la herramienta DDNS de Speco desde el sitio web de Speco Technologies para localizar dispositivos que no responden en las direcciones IP esperadas.

### Credenciales predeterminadas

Los dispositivos Speco típicamente se envían con credenciales predeterminadas de `admin` / `admin`. Si estas no funcionan, la contraseña puede haber sido cambiada durante la instalación por el integrador de seguridad.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Speco?**

Para la mayoría de cámaras IP Speco, pruebe primero el flujo raíz: `rtsp://admin:admin@CAMERA_IP:554/`. Para cámaras VIP Series use `rtsp://IP:554/1/stream1`, y para cámaras LS Series use `rtsp://IP:554/cam1/h264`. La URL correcta depende de su línea de productos específica.

**¿Las cámaras Speco soportan ONVIF?**

Sí. Todas las cámaras IP Speco actuales soportan ONVIF. El descubrimiento y transmisión ONVIF es la forma más confiable de conectarse a cámaras Speco si no está seguro del formato de URL RTSP exacto para su modelo.

**¿Por qué hay tantos formatos de URL diferentes para las cámaras Speco?**

Speco Technologies ha estado fabricando equipos de vigilancia desde 1969 y ha adquirido o desarrollado múltiples líneas de productos a lo largo de las décadas. Cada línea de productos (O Series, VIP, LS, ZIP, SIP, líneas DVR) puede usar diferente firmware y arquitecturas de transmisión, resultando en diferentes formatos de URL. Siempre identifique su serie de modelo exacta antes de configurar la conexión.

**¿Cómo encuentro mi cámara Speco en la red?**

Use la herramienta DDNS de Speco o la utilidad de descubrimiento de dispositivos, disponible en el sitio web de Speco Technologies. Alternativamente, use el descubrimiento ONVIF a través del VisioForge SDK o una herramienta de escaneo de red para localizar la dirección IP de la cámara en su red local.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de EverFocus](everfocus.md) — Cámaras de vigilancia profesional
- [Guardar Flujo RTSP Original](../mediablocks/Guides/rtsp-save-original-stream.md) — Grabar flujos Speco sin recodificación
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
