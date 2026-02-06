---
title: Dispositivos de Captura de Video
description: Usa webcams, tarjetas de captura y dispositivos de video en Video Capture SDK .Net. Configuración, enumeración y control completo de hardware.
---

# Dispositivos de Captura de Video - Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre el uso de dispositivos de captura de video físicos como webcams USB, cámaras integradas y tarjetas de captura.

## Tipos de Dispositivos

### Webcams USB

Las webcams USB son los dispositivos de captura más comunes. El SDK soporta la mayoría de las webcams compatibles con UVC.

### Cámaras Integradas

Cámaras integradas en laptops y dispositivos móviles.

### Tarjetas de Captura

Dispositivos que capturan video desde fuentes HDMI, SDI u otras interfaces de video.

## Guías Disponibles

### Enumeración y Selección

Aprenda cómo enumerar y seleccionar dispositivos de captura.

[Enumerar y seleccionar dispositivos →](enumerate-and-select.md)

### Control de Cámara (PTZ)

Control Pan-Tilt-Zoom para cámaras compatibles.

[Control de cámara PTZ →](camera-control-ptz.md)

### Crossbar de Video

Configuración de crossbar para tarjetas de captura.

[Crossbar de video →](crossbar.md)

### Ajustes de Video

Configuración de brillo, contraste, saturación y otros ajustes.

[Ajustes de video →](video-adjustments.md)

### Luz de Cámara

Control de la luz LED de la cámara.

[Habilitar luz de cámara →](enable-camera-light.md)

## Ejemplo Básico

```csharp
// Enumerar dispositivos de video
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();

if (devices.Length > 0)
{
    var device = devices[0];
    var format = device.VideoFormats[0];
    
    var settings = new VideoCaptureDeviceSourceSettings(device)
    {
        Format = format.ToFormat()
    };
    settings.Format.FrameRate = format.FrameRateList[0];
    
    var videoSource = new SystemVideoSourceBlock(settings);
    
    // Usar la fuente de video...
}
```

## Formatos de Video

Los dispositivos de captura soportan varios formatos de píxel:

| Formato | Descripción | Uso de CPU |
|---------|-------------|------------|
| YUY2 | Video sin comprimir | Alto |
| NV12 | Video sin comprimir | Alto |
| MJPEG | Video comprimido JPEG | Bajo |
| H.264 | Video comprimido H.264 | Bajo |

## Resoluciones Comunes

- 640x480 (VGA)
- 1280x720 (HD)
- 1920x1080 (Full HD)
- 3840x2160 (4K)

## Consideraciones de Rendimiento

1. **MJPEG vs YUY2**: MJPEG usa menos ancho de banda USB pero requiere decodificación
2. **Resolución vs fps**: Mayor resolución puede limitar la tasa de fotogramas máxima
3. **Múltiples cámaras**: Considere el ancho de banda USB al usar múltiples dispositivos
