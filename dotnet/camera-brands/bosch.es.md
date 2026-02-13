---
title: Cómo Conectar una Cámara IP Bosch en C# .NET
description: Conecta cámaras Bosch Security en C# .NET con patrones de URL RTSP y ejemplos de código para modelos Dinion, Flexidome, Autodome y codificadores VIP.
---

# Cómo Conectar una Cámara IP Bosch en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Bosch Security and Safety Systems** (una división de Robert Bosch GmbH) es un fabricante alemán de equipos de videovigilancia profesional y empresarial. Con sede en Grasbrunn cerca de Múnich, Bosch produce cámaras IP, codificadores, soluciones de grabación y análisis de video principalmente para mercados de infraestructura crítica, transporte y seguridad empresarial.

**Datos clave:**

- **Líneas de producto:** Dinion (bala/caja), Flexidome (domo), Autodome (PTZ), MIC (ruggedizado), NBN/NDN/NTC (red legacy), NWC (compacta), VideoJet/VIP (codificadores)
- **Soporte de protocolos:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Bosch VMS (BVMS), grabación directa iSCSI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** Varía según modelo y versión de firmware; muchos requieren configuración a través de Bosch Configuration Manager
- **Soporte ONVIF:** Sí (todas las cámaras IP actuales)
- **Códecs de video:** H.264, H.265, MJPEG
- **Característica única:** Modo túnel RTSP para traversal de firewall

## Patrones de URL RTSP

Las cámaras Bosch usan varios patrones de URL dependiendo de la generación del modelo. Los más comunes son las rutas `/rtsp_tunnel` y `/video`.

### Modelos Actuales (Firmware Bosch CPP)

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo de video 1 | `rtsp://IP:554/video?inst=1` | Flujo principal |
| Flujo de video 2 | `rtsp://IP:554/video?inst=2` | Subflujo |
| Túnel RTSP | `rtsp://IP:554//rtsp_tunnel` | Compatible con firewall (nota la doble barra) |
| H.264 directo | `rtsp://IP:554/h264` | Flujo H.264 directo |

!!! info "Modo túnel RTSP"
    La URL `//rtsp_tunnel` (con doble barra) es el modo de túnel RTSP propietario de Bosch que funciona mejor a través de firewalls y NAT. Encapsula datos RTP dentro de la conexión TCP RTSP. Usa la URL estándar `/video` para la mayoría de las integraciones.

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Códec | Notas |
|-------------|----------|-------|-------|
| Dinion IP 4000/5000/7000/8000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Actual |
| Flexidome IP 4000/5000/7000/8000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Actual |
| Autodome IP 4000/5000/7000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | PTZ actual |
| MIC IP fusion/starlight/ultra | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Ruggedizado |
| NDC-225-PI | `rtsp://IP:554//rtsp_tunnel` | H.264 | Legacy |
| NDC-255-P | `rtsp://IP:554//rtsp_tunnel` | H.264 | Legacy |
| NDC-265-P | `rtsp://IP:554/h264` | H.264 | Legacy |
| NDN-832v | `rtsp://IP:554//rtsp_tunnel` | H.264 | Domo legacy |
| NTC-255-PI | `rtsp://IP:554/video` | H.264 | Térmico legacy |
| NTC-265-PI | `rtsp://IP:554/h264` | H.264 | Térmico legacy |
| NTI-50022-V3 | `rtsp://IP:554/h264` | H.264 | Bala IP |
| NWC-0455-20P | `rtsp://IP:554/h264` | H.264 | Compacta |

### URLs de Codificadores

Los codificadores de video Bosch (series VideoJet, VIP) permiten conectar cámaras analógicas a redes IP:

| Codificador | URL RTSP | Notas |
|---------|----------|-------|
| VideoJet 10 | `rtsp://IP:554/video?inst=1` | Canal único |
| VIP X1 | `rtsp://IP:554//rtsp_tunnel` | Canal único |
| VIP X1600 | `rtsp://IP:554/video?inst=1` | Multicanal |
| VIP X2 | `rtsp://IP:554/video?inst=1` | Canal doble |

### URLs RTSP de DVR

| Modelo DVR | URL RTSP | Notas |
|-----------|----------|-------|
| DVR 440/480/600 | `rtsp://IP:554/rtsp_tunnel` | Barra simple |
| DVR 440/480/600 | `rtsp://IP:554/video` | Alternativa |
| DVR (canal) | `rtsp://IP:554/cgi-bin/rtspStream/CHANNEL` | Específico por canal |
| DVR (SDP) | `rtsp://IP:554/user=USER&password=PASS&channel=1&stream=0.sdp?` | Basado en SDP |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Bosch con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Bosch Dinion/Flexidome, main stream
var uri = new Uri("rtsp://192.168.1.60:554/video?inst=1");
var username = "service";
var password = "YourPassword";
```

Para acceder al subflujo, usa `?inst=2` en lugar de `?inst=1`. Para modelos Bosch legacy, usa la URL de túnel RTSP `//rtsp_tunnel` (nota la doble barra).

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/snap.jpg` | Captura básica |
| Captura (dimensionada) | `http://IP/snap.jpg?JpegSize=XL` | Tamaños XL, M disponibles |
| Captura (canal) | `http://IP/snap.jpg?JpegCam=CHANNEL` | Codificadores multicanal |
| Captura (auth) | `http://IP/snap.jpg?usr=USER&pwd=PASS` | Autenticación por URL |
| Flujo MJPEG | `http://IP/img/mjpeg.jpg` | MJPEG continuo |
| Imagen | `http://IP/img.jpg` | Fotograma único |
| Imagen (alt) | `http://IP/image.jpg` | Ruta alternativa |

## Solución de Problemas

### Doble barra en la URL rtsp_tunnel

La URL `//rtsp_tunnel` (con doble barra antes de `rtsp_tunnel`) es intencional para cámaras Bosch legacy. Esto no es un error tipográfico:

- Correcto: `rtsp://IP:554//rtsp_tunnel`
- Incorrecto: `rtsp://IP:554/rtsp_tunnel` (puede funcionar en algunos modelos pero no en todos)

### Se requiere Bosch Configuration Manager

Muchas cámaras Bosch requieren configuración inicial a través de la aplicación de escritorio **Bosch Configuration Manager** antes de que el acceso RTSP funcione. La cámara puede no responder a conexiones RTSP hasta que se complete la configuración inicial.

### El nombre de usuario predeterminado varía

- **Modelos actuales:** Usuario `service` con contraseña establecida durante la configuración
- **Modelos legacy:** Pueden usar `admin`, `user` o `service` dependiendo del firmware
- Consulta el Bosch Configuration Manager o la interfaz web de la cámara para la configuración de usuarios

### Parámetro inst

El parámetro `?inst=1` selecciona la instancia del flujo de video:
- `inst=1` = Primer flujo de video (principal)
- `inst=2` = Segundo flujo de video (sub)
- No todos los modelos soportan múltiples instancias

### Selección de canal del codificador

Para codificadores multicanal (VIP X1600, series VideoJet X), usa el parámetro `inst` para seleccionar el canal:
- `rtsp://IP:554/video?inst=1` = Canal 1
- `rtsp://IP:554/video?inst=2` = Canal 2

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Bosch?**

Para cámaras Bosch actuales, la URL es `rtsp://service:password@IP_CAMARA:554/video?inst=1`. Para modelos legacy, prueba `rtsp://IP_CAMARA:554//rtsp_tunnel` o `rtsp://IP_CAMARA:554/h264`.

**¿Qué es el modo túnel RTSP de Bosch?**

El túnel RTSP (`//rtsp_tunnel`) es el modo propietario de Bosch que encapsula datos RTP dentro de la conexión TCP RTSP, facilitando el traversal de firewalls. Es el modo de transmisión predeterminado en muchas cámaras Bosch legacy.

**¿Las cámaras Bosch soportan H.265?**

Las cámaras IP Bosch actuales (plataforma CPP13/CPP14, incluyendo series Dinion/Flexidome 7000/8000) soportan H.265. Las cámaras legacy soportan H.264 y MPEG-4. Consulta la hoja de datos de tu modelo específico para soporte de códecs.

**¿Puedo usar codificadores Bosch para conectar cámaras analógicas?**

Sí. Los codificadores Bosch VideoJet y VIP convierten señales de cámaras analógicas a flujos IP accesibles vía RTSP. Usa el mismo formato de URL (`/video?inst=1` o `//rtsp_tunnel`) que las cámaras IP.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Axis](axis.md) — Par de vigilancia empresarial
- [Guía de Conexión Honeywell](honeywell.md) — Segmento empresarial / comercial
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
