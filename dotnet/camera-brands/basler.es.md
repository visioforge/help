---
title: Basler - URLs RTSP y conexión de cámaras IP en C# .NET
description: Guía para conectar cámaras IP Basler BIP2 en C# .NET usando URLs RTSP. Incluye patrones de URL, ejemplos de código y solución de problemas.
---

# Cómo conectar una cámara IP Basler en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Basler** (Basler AG) es un fabricante alemán de cámaras con sede en Ahrensburg, Alemania, fundado en 1988. Basler es líder mundial en cámaras de visión artificial industrial y también produce cámaras de seguridad IP bajo la línea de productos **BIP2**. Mientras que las cámaras de visión artificial de Basler usan protocolos industriales especializados, la serie BIP2 proporciona conectividad estándar RTSP y ONVIF para aplicaciones de seguridad y vigilancia.

**Datos clave:**

- **Líneas de productos:** ace (visión artificial), dart (compacta), boost (alta velocidad), BIP2 (seguridad IP)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI (serie BIP2); GigE Vision, USB3 Vision (visión artificial)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (serie BIP2)
- **Códecs de video:** H.264, MPEG-4, MJPEG
- **Uso principal:** Visión industrial, automatización de fábricas, inspección de calidad, vigilancia IP

!!! info "Las Cámaras de Visión Artificial Usan Protocolos Diferentes"
    Las cámaras de visión artificial ace, dart y boost de Basler usan protocolos GigE Vision o USB3 Vision, no RTSP. Estas requieren el SDK Pylon de Basler o un framework compatible con GenICam. Las URLs RTSP en esta página se aplican a la línea de cámaras de seguridad IP BIP2 de Basler.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras IP Basler BIP2 usan URLs RTSP simples basadas en ruta:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264
```

| Flujo | Patrón de URL | Descripción |
|-------|---------------|-------------|
| Flujo principal H.264 | `rtsp://IP:554/h264` | Flujo principal, mejor calidad |
| Flujo MPEG-4 | `rtsp://IP:554/mpeg4` | Flujo legacy codificado en MPEG-4 |
| JPEG sobre RTSP | `rtsp://IP:554/jpeg` | Cuadros JPEG sobre RTSP |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Códec |
|--------|------|------------------------|-------|
| BIP2-1280c (720p) | IP bala | `rtsp://IP:554/h264` | H.264 |
| BIP2-1300c (1.3MP) | IP bala | `rtsp://IP:554/h264` | H.264 |
| BIP2-1920c (1080p) | IP bala | `rtsp://IP:554/h264` | H.264 |
| BIP2-1300c-dn | IP día/noche | `rtsp://IP:554/h264` | H.264 |
| BIP2-1920c-dn | IP día/noche | `rtsp://IP:554/h264` | H.264 |

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/h264` | Flujo H.264 (recomendado) |
| `rtsp://IP:554/mpeg4` | Flujo MPEG-4 (legacy) |
| `rtsp://IP:554/jpeg` | JPEG sobre RTSP |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Basler con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Basler BIP2, H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/h264");
var username = "admin";
var password = "admin";
```

Para flujos MPEG-4, reemplace `/h264` con `/mpeg4` en la URL.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Flujo MJPEG | `http://IP/cgi-bin/mjpeg` | Flujo MJPEG continuo |
| Captura JPEG | `http://IP/cgi-bin/jpeg?stream=0` | Captura del canal 0 |
| Captura JPEG (canal) | `http://IP/cgi-bin/jpeg?stream=CHANNEL` | Captura de canal específico |

## Solución de Problemas

### Cámara de visión artificial no conecta vía RTSP

Las cámaras ace, dart y boost de Basler no soportan RTSP. Estas cámaras usan protocolos GigE Vision (Ethernet) o USB3 Vision (USB) y requieren el SDK Pylon de Basler o una biblioteca compatible con GenICam. Solo la serie de cámaras IP BIP2 soporta transmisión RTSP.

### Error "401 Unauthorized"

Las cámaras Basler BIP2 se envían con credenciales predeterminadas `admin` / `admin`. Si las credenciales han sido cambiadas:

1. Acceda a la interfaz web de la cámara en `http://CAMERA_IP`
2. Inicie sesión y verifique o restablezca las credenciales
3. Use las credenciales actualizadas en su URL RTSP

### Sin salida de video en flujo MPEG-4

Algunas versiones de firmware más nuevas de Basler BIP2 pueden tener como predeterminado solo H.264. Si el flujo MPEG-4 no devuelve datos:

1. Abra la interfaz web de la cámara
2. Navegue a la configuración de flujo de video
3. Asegúrese de que la codificación MPEG-4 esté habilitada
4. Alternativamente, use la ruta de flujo `/h264` en su lugar

### El descubrimiento ONVIF no encuentra la cámara

ONVIF es compatible solo con cámaras de la serie BIP2. Asegúrese de que:

- El firmware de la cámara esté actualizado
- ONVIF esté habilitado en la configuración de red de la cámara
- La cámara y el cliente de descubrimiento estén en la misma subred

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Basler?**

Para cámaras IP Basler BIP2, la URL predeterminada es `rtsp://admin:admin@CAMERA_IP:554/h264` para el flujo principal H.264. Reemplace las credenciales si han sido cambiadas de los valores predeterminados.

**¿Puedo usar VisioForge SDK con cámaras de visión artificial Basler?**

La conexión basada en RTSP descrita en esta página se aplica solo a cámaras de seguridad IP Basler BIP2. Las cámaras de visión artificial de Basler (ace, dart, boost) usan protocolos GigE Vision o USB3 Vision y requieren el SDK Pylon de Basler o un framework compatible con GenICam para integración directa.

**¿Las cámaras Basler soportan ONVIF?**

Sí, pero solo la serie de cámaras de seguridad IP BIP2 soporta ONVIF. Las cámaras de visión artificial de Basler usan protocolos industriales (GigE Vision, USB3 Vision) en su lugar.

**¿Qué códecs soportan las cámaras IP Basler?**

Las cámaras Basler BIP2 soportan H.264, MPEG-4 y MJPEG. Se recomienda H.264 para el mejor equilibrio entre calidad y eficiencia de ancho de banda.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Mobotix](mobotix.md) — Cámaras industriales alemanas
- [Guía de Conexión de FLIR](flir.md) — Imágenes industriales y térmicas
- [Construcción de Aplicaciones de Cámara con Media Blocks](../mediablocks/GettingStarted/camera.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
