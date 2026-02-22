---
title: Conectar Cámara IP Hikvision en C# .NET - URL RTSP
description: Conecta cámaras Hikvision en C# .NET con patrones de URL RTSP, descubrimiento ONVIF y ejemplos de código completos para modelos DS-2CD, DS-2DE y NVR.
---

# Cómo Conectar una Cámara IP Hikvision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Hikvision** (Hangzhou Hikvision Digital Technology Co., Ltd.) es el mayor fabricante de equipos de videovigilancia del mundo por cuota de mercado. Fundada en 2001 y con sede en Hangzhou, China, Hikvision produce cámaras IP, DVRs, NVRs y software de gestión de video utilizados en mercados empresariales, gubernamentales y de consumo.

**Datos clave:**

- **Líneas de producto:** DS-2CD (cámaras fijas), DS-2DE (cámaras PTZ), DS-76/77/96 (NVRs), DS-7200/7300/7600 (DVRs)
- **Soporte de protocolos:** ONVIF Profile S/G/T, RTSP, HTTP, ISAPI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (establecido durante la configuración inicial; firmware antiguo: admin / 12345)
- **Soporte ONVIF:** Completo -- recomendado para descubrimiento y configuración automática
- **Códecs de video:** H.264, H.265 (Smart Codec), MJPEG

## Patrones de URL RTSP

Las cámaras Hikvision utilizan una estructura de URL basada en canales. Los números de canal codifican tanto el canal de la cámara como el tipo de flujo.

### Formato de URL

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:[PUERTO]/Streaming/Channels/[ID_CANAL]
```

**Codificación del ID de canal:**

- ID de canal = (número_de_cámara * 100) + número_de_flujo
- Flujo 1 = flujo principal, Flujo 2 = subflujo, Flujo 3 = tercer flujo
- Ejemplo: Cámara 1, flujo principal = **101**; Cámara 1, subflujo = **102**

### Cámaras IP (Canal Único)

| Serie de Modelo | URL RTSP | Flujo | Audio |
|-------------|----------|--------|-------|
| DS-2CD2xx2 (2MP fija) | `rtsp://IP:554/Streaming/Channels/101` | Principal (1080p) | Sí |
| DS-2CD2xx2 (2MP fija) | `rtsp://IP:554/Streaming/Channels/102` | Sub (CIF/D1) | Sí |
| DS-2CD2x32 (3MP fija) | `rtsp://IP:554/Streaming/Channels/101` | Principal (2048x1536) | Sí |
| DS-2CD2x32 (3MP fija) | `rtsp://IP:554/Streaming/Channels/102` | Sub | Sí |
| DS-2CD21xx-I (serie value) | `rtsp://IP:554/Streaming/Channels/1` | Principal | Sí |
| DS-2CD21xx-I (serie value) | `rtsp://IP:554/Streaming/Channels/2` | Sub | Sí |
| Serie DS-2DE (PTZ) | `rtsp://IP:554/Streaming/Channels/101` | Principal | Sí |
| DS-2CD6362F (ojo de pez) | `rtsp://IP:554/Streaming/Channels/101` | Principal (3072x2048) | Sí |

### Canales NVR / DVR

Para dispositivos NVR y DVR, cambia el número de cámara en el ID de canal:

| Dispositivo | Canal | URL RTSP | Flujo |
|--------|---------|----------|--------|
| NVR Cámara 1 | 1 | `rtsp://IP:554/Streaming/Channels/101` | Principal |
| NVR Cámara 1 | 1 | `rtsp://IP:554/Streaming/Channels/102` | Sub |
| NVR Cámara 2 | 2 | `rtsp://IP:554/Streaming/Channels/201` | Principal |
| NVR Cámara 2 | 2 | `rtsp://IP:554/Streaming/Channels/202` | Sub |
| NVR Cámara 8 | 8 | `rtsp://IP:554/Streaming/Channels/801` | Principal |
| DVR Canal 1 | 1 | `rtsp://IP:554/Streaming/Channels/101` | Principal |

### Formatos de URL Alternativos

Algunos modelos Hikvision más antiguos y variantes OEM utilizan patrones de URL diferentes:

| Patrón de URL | Notas |
|-------------|-------|
| `rtsp://IP:554/h264/ch1/main/av_stream` | Versiones de firmware más antiguas |
| `rtsp://IP:554/h264/ch1/sub/av_stream` | Firmware antiguo, subflujo |
| `rtsp://IP:554/PSIA/Streaming/channels/101` | Protocolo PSIA (legacy) |
| `rtsp://IP:554/video.h264` | Algunos modelos OEM |
| `rtsp://IP:554/live.sdp` | Algunos modelos más antiguos |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | OEM compatible con Dahua |
| `rtsp://IP:554/mpeg4` | Flujo MPEG4 (legacy) |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Hikvision con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Hikvision DS-2CD2032-I, main stream
var uri = new Uri("rtsp://192.168.1.64:554/Streaming/Channels/101");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, usa `/Streaming/Channels/102` en su lugar.

### Descubrimiento ONVIF

Las cámaras Hikvision tienen excelente soporte ONVIF. Usa ONVIF para descubrir automáticamente cámaras en tu red y obtener sus URIs de flujo sin construir manualmente las URLs RTSP. Consulta la [guía de integración ONVIF](../mediablocks/Sources/index.md) para ejemplos de código de descubrimiento.

## URLs de Captura y MJPEG

Las cámaras Hikvision también proporcionan endpoints HTTP para capturas y flujos MJPEG:

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/ISAPI/Streaming/channels/101/picture` | Requiere autenticación |
| Flujo MJPEG | `http://IP/ISAPI/Streaming/channels/102/httpPreview` | Subflujo como MJPEG |
| Captura Legacy | `http://IP/Streaming/channels/1/picture` | Firmware antiguo |
| Captura CGI | `http://IP/cgi-bin/snapshot.cgi` | Autenticación básica |

## Solución de Problemas

### "Doble barra" en la ruta de URL

Las URLs RTSP de Hikvision usan una ruta que comienza con `/Streaming/Channels/`. Algunas herramientas o código generan `//Streaming/Channels/` (doble barra). Ambas funcionan con cámaras Hikvision, pero usa una sola barra para mayor corrección.

### Conexión rechazada en el puerto 554

- Verifica que RTSP esté habilitado en la interfaz web de la cámara: **Configuración > Red > Configuración Avanzada > RTSP**
- Comprueba que el puerto RTSP no haya sido cambiado del predeterminado (554)
- Asegúrate de que ningún firewall esté bloqueando el puerto entre tu aplicación y la cámara

### Fallos de autenticación

- Las cámaras Hikvision requieren **autenticación digest** por defecto. El VisioForge SDK maneja esto automáticamente.
- En firmware más reciente, las credenciales predeterminadas `admin/12345` están deshabilitadas. Debes establecer una contraseña segura durante la configuración inicial mediante la herramienta Hikvision SADP o la interfaz web.
- Si te conectas a un NVR, usa las credenciales del NVR, no las credenciales individuales de la cámara.

### El flujo H.265 no se reproduce

- Asegúrate de tener instalado el redistributable del decodificador HEVC
- Alternativamente, configura la cámara para usar codificación H.264 en sus ajustes de video
- Usa `rtspSettings.UseGPUDecoder = true` para decodificación H.265 acelerada por hardware

### Alta latencia

- Usa transporte TCP: `rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP`
- Reduce la latencia del buffer: `rtspSettings.Latency = 200` (en milisegundos)
- Cambia al subflujo (canal 102) para menores requisitos de ancho de banda
- Desactiva el audio si no es necesario: `audioEnabled: false`

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Hikvision?**

La URL RTSP estándar para cámaras Hikvision es `rtsp://admin:password@IP_CAMARA:554/Streaming/Channels/101` para el flujo principal. Reemplaza `admin` y `password` con las credenciales de tu cámara, e `IP_CAMARA` con la dirección IP de la cámara. Usa el canal `102` para el subflujo.

**¿Cómo encuentro la dirección IP de mi cámara Hikvision?**

Usa la herramienta Hikvision SADP (Search Active Devices Protocol), que es una utilidad gratuita que descubre todos los dispositivos Hikvision en tu red local. Alternativamente, consulta la lista de clientes DHCP de tu router o usa el descubrimiento de dispositivos ONVIF con el VisioForge SDK.

**¿Puedo conectarme a un NVR Hikvision y ver canales de cámaras individuales?**

Sí. Usa el mismo formato de URL RTSP pero cambia el número de canal. La cámara 1 es el canal 101 (principal) o 102 (sub), la cámara 2 es el canal 201/202, y así sucesivamente. La fórmula es: ID de canal = (número_de_cámara x 100) + número_de_flujo.

**¿El VisioForge SDK soporta H.265+ (Smart Codec) de Hikvision?**

Sí. El SDK soporta decodificación estándar H.265/HEVC. El H.265+ de Hikvision es una optimización de compresión propietaria que produce flujos H.265 estándar, por lo que funciona con cualquier decodificador compatible con H.265.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión LTS](lts.md) — OEM de Hikvision, usa el mismo formato de URL
- [Guía de Conexión EZVIZ](ezviz.md) — Marca de consumo de Hikvision
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
