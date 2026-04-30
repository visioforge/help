---
title: Cómo conectar una cámara IP Arecont Vision en C# .NET
description: Conecte cámaras Arecont Vision en C# .NET con patrones de URL RTSP y ejemplos de código para modelos AV Series, MegaDome y SurroundVideo multisensor.
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

# Cómo conectar una cámara IP Arecont Vision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Arecont Vision** (ahora parte de Costar Group) es una empresa estadounidense de cámaras IP fundada originalmente en 2003 y con sede en Glendale, California. Arecont Vision fue pionera en cámaras IP de megapíxeles y es conocida por modelos de alta resolución (hasta 20MP) y cámaras panorámicas multisensor. La empresa fue adquirida por **Costar Group** en 2019, que continúa dando soporte y fabricando productos Arecont Vision.

**Datos clave:**

- **Líneas de productos:** AV Series (megapíxel fija), MegaDome, MegaBall, SurroundVideo (panorámica multisensor), MicroDome
- **Soporte de protocolos:** RTSP, ONVIF (modelos más nuevos), PSIA, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** Varía según el modelo (muchos se envían sin contraseña predeterminada)
- **Soporte ONVIF:** Sí (modelos más nuevos), soporte PSIA en la mayoría de los modelos
- **Códecs de video:** H.264, MJPEG (modelos más antiguos solo MJPEG)

!!! info "Arecont Vision y Costar Group"
    Arecont Vision fue adquirida por Costar Group en 2019. Las cámaras Arecont existentes continúan usando los mismos formatos de URL RTSP y PSIA. Los modelos más nuevos con marca Costar pueden usar firmware actualizado pero mantienen patrones de URL retrocompatibles.

## Patrones de URL RTSP

### Formatos de URL Estándar

Las cámaras Arecont Vision soportan múltiples patrones de URL RTSP dependiendo de la generación del modelo y el protocolo configurado:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264.sdp
```

| Patrón de URL | Protocolo | Descripción |
|---------------|-----------|-------------|
| `rtsp://IP:554/h264.sdp` | H.264 | Flujo H.264 estándar (mayoría de modelos actuales) |
| `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA | Flujo H.264 basado en PSIA |
| `rtsp://IP:554/cam1/mpeg4` | MPEG-4 | Flujo MPEG-4 legacy (modelos más antiguos) |

### Flujo H.264 con Parámetros ROI

Las cámaras Arecont soportan parámetros opcionales de Región de Interés (ROI) para personalizar la salida del flujo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264.sdp?res=half&x0=0&y0=0&x1=1600&y1=1200&quality=15&doublescan=0
```

| Parámetro | Valores | Descripción |
|-----------|---------|-------------|
| `res` | `full`, `half` | Resolución del flujo (completa o mitad de la resolución del sensor) |
| `x0`, `y0` | 0 - max | Esquina superior izquierda de la región de interés |
| `x1`, `y1` | 0 - max | Esquina inferior derecha de la región de interés |
| `quality` | 1 - 21 | Factor de calidad JPEG/H.264 (menor = mayor calidad) |
| `doublescan` | 0, 1 | Habilitar modo de doble escaneo para mejor calidad de imagen |

!!! tip "Los Parámetros ROI Son Opcionales"
    Los parámetros ROI (`res`, `x0`, `y0`, `x1`, `y1`, `quality`, `doublescan`) son opcionales y pueden omitirse completamente para transmisión a cuadro completo. Use `rtsp://IP:554/h264.sdp` sin parámetros para la conexión más simple.

### Modelos de Cámaras

| Serie de Modelo | Resolución | URL de Flujo Principal | Códec |
|-----------------|------------|------------------------|-------|
| AV Series (megapíxel genérico) | Varía | `rtsp://IP:554/h264.sdp` | H.264 |
| AV2100 (2MP) | 1600x1200 | `rtsp://IP:554/cam1/mpeg4` | MPEG-4 |
| AV5115/AV5125 | 2592x1944 | `rtsp://IP:554/h264.sdp` | H.264 |
| AV8185DN (8MP multisensor) | 6400x1200 | `rtsp://IP:554/h264.sdp` | H.264 |
| AV10005/AV10115 (10MP) | 3648x2752 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| AV20185 (20MP multisensor) | 10240x1536 | `rtsp://IP:554/h264.sdp` | H.264 |
| MegaDome Series | Varía | `rtsp://IP:554/h264.sdp` | H.264 |
| MegaBall Series | Varía | `rtsp://IP:554/h264.sdp` | H.264 |
| MicroDome Series | Varía | `rtsp://IP:554/h264.sdp` | H.264 |

### URLs de Transmisión PSIA

Los modelos que soportan el protocolo PSIA pueden usar el siguiente formato de URL:

| Canal | URL |
|-------|-----|
| Canal 0 (predeterminado) | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` |
| Canal 1 | `rtsp://IP:554/PSIA/Streaming/channels/1?videoCodecType=H.264` |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Arecont Vision con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Arecont Vision AV Series, H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/h264.sdp");
var username = "admin";
var password = "YourPassword";
```

Para transmisión basada en PSIA, use la URL PSIA en su lugar:

```csharp
// Arecont Vision via PSIA protocol
var uri = new Uri("rtsp://192.168.1.90:554/PSIA/Streaming/channels/0?videoCodecType=H.264");
```

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/img.jpg` | Mayoría de modelos, requiere autenticación básica |
| Captura JPEG (alt) | `http://IP/Jpeg/CamImg.jpg` | URL de captura alternativa |
| Captura Configurable | `http://IP/image?res=half&x0=0&y0=0&x1=1600&y1=1200&quality=15&doublescan=0` | Captura con parámetros ROI |
| Flujo MJPEG | `http://IP/mjpeg?res=full&x0=0&y0=0&x1=100%&y1=100%&quality=12&doublescan=0` | Flujo MJPEG continuo |

### URLs de Captura Multisensor (SurroundVideo)

Para cámaras SurroundVideo y otras multisensor, cada sensor tiene su propia URL de captura:

| Sensor | Patrón de URL | Notas |
|--------|---------------|-------|
| Canal 1 | `http://IP/image1?res=half&x1=0&y1=0` | Primer sensor |
| Canal 2 | `http://IP/image2?res=half&x1=0&y1=0` | Segundo sensor |
| Canal 3 | `http://IP/image3?res=half&x1=0&y1=0` | Tercer sensor |
| Canal 4 | `http://IP/image4?res=half&x1=0&y1=0` | Cuarto sensor |

## Solución de Problemas

### Problemas con parámetros ROI

Las cámaras Arecont tienen parámetros únicos de Región de Interés (`res`, `x0`, `y0`, `x1`, `y1`, `quality`, `doublescan`) incrustados en sus URLs. Si encuentra problemas de conexión:

1. Elimine todos los parámetros ROI y use la URL básica: `rtsp://IP:554/h264.sdp`
2. Verifique que la resolución de la cámara soporte las coordenadas ROI solicitadas
3. Use `res=full` para transmisión a resolución completa o `res=half` para ancho de banda reducido

### Solo MJPEG en modelos más antiguos

Algunos modelos Arecont más antiguos (AV1300, AV2100, AV3100) solo soportan codificación MJPEG y no tienen capacidad H.264. Para estas cámaras:

- Use la URL RTSP MPEG-4: `rtsp://IP:554/cam1/mpeg4`
- O use el flujo HTTP MJPEG: `http://IP/mjpeg`

### PSIA vs RTSP directo

Las cámaras Arecont soportan tanto RTSP directo como URLs RTSP basadas en PSIA. Si un formato no funciona:

- Pruebe el formato alternativo (cambie entre `h264.sdp` y `PSIA/Streaming/channels/0`)
- Verifique que la versión del firmware de la cámara soporte el protocolo elegido
- Compruebe que PSIA esté habilitado en la interfaz web de la cámara

### Tiempo de espera de conexión

Las cámaras Arecont pueden tardar más en establecer una sesión RTSP que otras marcas, especialmente para modelos de alta resolución (10MP+):

- Aumente el tiempo de espera de conexión a al menos 10 segundos
- Para modelos multisensor, conéctese a canales individuales en lugar del flujo compuesto para menor latencia

## Preguntas Frecuentes

**¿Qué formato de URL RTSP usa Arecont Vision?**

La URL RTSP principal es `rtsp://IP:554/h264.sdp` para transmisión H.264. La transmisión basada en PSIA usa `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264`. Los modelos más antiguos pueden usar `rtsp://IP:554/cam1/mpeg4` para flujos MPEG-4.

**¿Las cámaras Arecont Vision siguen teniendo soporte después de la adquisición por Costar?**

Sí. Costar Group adquirió Arecont Vision en 2019 y continúa fabricando y dando soporte a los productos de cámaras Arecont Vision. Las cámaras existentes siguen siendo completamente funcionales, y las actualizaciones de firmware están disponibles a través de los canales de soporte de Costar.

**¿Cómo me conecto a una cámara multisensor SurroundVideo?**

Las cámaras SurroundVideo exponen canales de sensores individuales a través de URLs numeradas. Para capturas, use `http://IP/image1`, `http://IP/image2`, etc. Para RTSP, use la URL H.264 estándar con el compuesto panorámico completo, o URLs basadas en canal PSIA para sensores individuales.

**¿Las cámaras Arecont Vision soportan ONVIF?**

Los modelos más nuevos de Arecont Vision soportan ONVIF Profile S. Los modelos más antiguos dependen del protocolo PSIA en su lugar. Verifique las especificaciones de su cámara o la interfaz web para confirmar la disponibilidad de ONVIF.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de GeoVision](geovision.md) — Cámaras de vigilancia profesional
- [Captura ONVIF con Postprocesamiento](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Pipeline de captura ONVIF para Arecont
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
