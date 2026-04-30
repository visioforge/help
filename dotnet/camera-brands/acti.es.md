---
title: Cámara IP ACTi - URL RTSP y Guía de Conexión C# .NET
description: Conecta cámaras IP ACTi en C# .NET con patrones de URL RTSP y ejemplos de código para series A, B, D, E y modelos antiguos ACM, KCM, TCM.
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
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - H.265
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP ACTi en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**ACTi Corporation** es un fabricante taiwanés de cámaras de vigilancia IP y soluciones de gestión de video. Con sede en Taipéi, Taiwán, ACTi se dirige a mercados profesionales y empresariales con una amplia gama de cámaras fijas, domo, bullet y PTZ. ACTi es conocida por sus cámaras actuales de las series A/B/D/E y las líneas de productos antiguas ACM, KCM y TCM.

**Datos clave:**

- **Líneas de producto:** Serie A (caja), serie B (bullet/zoom), serie D (domo), serie E (domo hemisférico), KCM (domo antigua), ACM (caja/domo antigua), TCM (caja antigua)
- **Soporte de protocolos:** RTSP, ONVIF (series actuales A/B/D/E), HTTP/CGI
- **Puerto RTSP predeterminado:** 7070 (la mayoría de modelos), 554 (algunos modelos antiguos)
- **Credenciales predeterminadas:** Admin / 123456 (modelos actuales), admin / admin (antiguos)
- **Soporte ONVIF:** Sí (series actuales A/B/D/E)
- **Códecs de video:** H.264, H.265 (serie E), MJPEG

!!! warning "Puerto no estándar"
    Las cámaras ACTi usan el **puerto 7070** por defecto para RTSP, no el puerto estándar 554. Este es el problema de conexión más común al integrar cámaras ACTi.

## Patrones de URL RTSP

### Modelos Actuales (Series A/B/D/E)

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo principal | `rtsp://IP:7070//stream1` | Flujo primario (note la doble barra) |
| Flujo raíz | `rtsp://IP:7070/` | Alternativa |
| H.264 directo | `rtsp://IP:7070/h264` | Selección explícita de códec |
| Flujo ONVIF | `rtsp://IP:7070//onvif-stream1` | Variante ONVIF |

!!! info "Doble barra antes de stream1"
    Las cámaras ACTi usan una **doble barra** antes de `stream1` en sus URLs RTSP: `rtsp://IP:7070//stream1`. Esto es intencional y necesario para la mayoría de modelos actuales.

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Tipo | Notas |
|-------------|----------|------|-------|
| D11, D21, D31, D32 | `rtsp://IP:7070//stream1` | Domo | Actual |
| D42, D51, D52, D55, D72 | `rtsp://IP:7070//stream1` | Domo | Actual |
| E12, E32, E33, E43, E46 | `rtsp://IP:7070//stream1` | Hemisférico | Actual, compatible con H.265 |
| E51, E52, E63, E65, E73 | `rtsp://IP:7070//stream1` | Hemisférico | Actual, compatible con H.265 |
| E82, E84, E96 | `rtsp://IP:7070//stream1` | Hemisférico | Actual, compatible con H.265 |
| B53, B87, B95 | `rtsp://IP:7070//stream1` | Bullet/Zoom | Actual |
| Serie A (caja) | `rtsp://IP:7070//stream1` | Caja | Actual |

### Modelos Anteriores

Las cámaras ACTi antiguas pueden usar el puerto 554 o 7070 dependiendo del modelo y la versión del firmware:

| Serie de Modelo | URL RTSP | Tipo | Notas |
|-------------|----------|------|-------|
| ACM-1011 | `rtsp://IP:554/` o `rtsp://IP:7070/` | Caja | Antigua |
| ACM-3401 | `rtsp://IP:554/` o `rtsp://IP:7070/` | Domo | Antigua |
| ACM-5601 | `rtsp://IP:554/` o `rtsp://IP:7070/` | Caja | Antigua |
| ACM-7411 | `rtsp://IP:554/` o `rtsp://IP:7070/` | Domo | Antigua |
| KCM-3311 | `rtsp://IP:7070/` | Domo | Antigua |
| KCM-5611 | `rtsp://IP:7070/` | Domo | Antigua |
| KCM-7211 | `rtsp://IP:7070/` | Domo | Antigua |
| TCM-1231 | `rtsp://IP:7070/` | Caja | Antigua |
| TCM-3511 | `rtsp://IP:7070/` | Caja | Antigua |
| TCM-5111 | `rtsp://IP:7070/` | Caja | Antigua |
| TCM-5311 | `rtsp://IP:7070/` | Caja | Antigua |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara ACTi con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// ACTi D/E series camera, main stream -- note port 7070, not 554!
var uri = new Uri("rtsp://192.168.1.50:7070//stream1");
var username = "Admin";
var password = "123456";
```

Para modelos ACM antiguos que usan el puerto 554, cambie el puerto correspondiente. Para un flujo raíz más simple, use `rtsp://IP:7070/` como URL.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura CGI | `http://IP/cgi-bin/encoder?USER=USERNAME&PWD=PASSWORD&SNAPSHOT` | Captura autenticada |
| Streaming HTTP | `http://IP/cgi-bin/cmd/system?GET_STREAM&USER=USERNAME&PWD=PASSWORD` | Flujo continuo |
| Imagen JPEG | `http://IP/jpg/image.jpg` | JPEG directo |
| JPEG (alt) | `http://IP/now.jpg` | Ruta de captura alternativa |

## Solución de Problemas

### Puerto 7070, no 554

El problema de conexión más común con ACTi es usar el puerto estándar 554. Las cámaras ACTi usan por defecto el **puerto 7070** para RTSP. Si su conexión agota el tiempo de espera o es rechazada, verifique que está usando el puerto correcto.

- Correcto: `rtsp://IP:7070//stream1`
- Probablemente incorrecto: `rtsp://IP:554//stream1` (a menos que use un modelo ACM antiguo)

### Doble barra antes de stream1

Las cámaras ACTi de generación actual usan una **doble barra** antes de `stream1`:

- Correcto: `rtsp://IP:7070//stream1`
- Puede no funcionar: `rtsp://IP:7070/stream1`

### Las credenciales predeterminadas difieren según la generación

- **Modelos actuales (series A/B/D/E):** Usuario `Admin` (A mayúscula), contraseña `123456`
- **Modelos antiguos (ACM/KCM/TCM):** Usuario `admin` (minúscula), contraseña `admin`

Siempre cambie las credenciales predeterminadas antes de desplegar cámaras en una red de producción.

### Modelos ACM antiguos y puerto 554

Algunas cámaras ACM-series más antiguas (ACM-1011, ACM-3401, ACM-5601, ACM-7411) pueden usar el puerto 554 en lugar de 7070. Si el puerto 7070 falla en un modelo antiguo, intente el puerto 554 con la URL raíz `rtsp://IP:554/`.

### Disponibilidad de ONVIF

ONVIF solo es soportado en cámaras de generación actual (series A, B, D y E). Las cámaras antiguas ACM, KCM y TCM no soportan ONVIF. Para modelos antiguos, use URLs RTSP o HTTP directas.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras ACTi?**

Para cámaras ACTi actuales (series A/B/D/E), use `rtsp://Admin:123456@IP_CAMARA:7070//stream1`. Note el puerto no estándar 7070 y la doble barra antes de `stream1`. Para modelos antiguos, intente `rtsp://admin:admin@IP_CAMARA:7070/` o `rtsp://admin:admin@IP_CAMARA:554/`.

**¿Por qué ACTi usa el puerto 7070 en lugar de 554?**

ACTi eligió el puerto 7070 como su puerto RTSP predeterminado. Esto puede cambiarse en la interfaz web de la cámara, pero el predeterminado de fábrica es 7070 para la mayoría de modelos. Algunas cámaras ACM-series antiguas usan por defecto el puerto 554.

**¿ACTi soporta H.265?**

Las cámaras de la serie E actual (modelos domo hemisférico) soportan codificación H.265. Otras series actuales (A, B, D) usan principalmente H.264. Los modelos antiguos (ACM, KCM, TCM) soportan solo H.264 y MJPEG.

**¿Cuál es la diferencia entre las series de productos ACTi?**

ACTi organiza las cámaras por letra: **A** = cámaras de caja, **B** = cámaras bullet y zoom, **D** = cámaras domo, **E** = cámaras domo hemisféricas. Las líneas de productos antiguas incluyen ACM (caja/domo), KCM (domo) y TCM (caja).

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Vivotek](vivotek.md) — Cámaras empresariales taiwanesas
- [Guía de Conexión de GeoVision](geovision.md) — Cámaras profesionales taiwanesas
- [Integración de Cámara IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Configuración de dispositivo ONVIF ACTi
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
