---
title: Implementar efectos de zoom en aplicaciones .NET de video
description: Aplique efectos de zoom y paneo en C# / .NET con VideoEffectZoom y VideoEffectPan — punto focal ajustable en tiempo real para captura, reproducción y edición.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoEffectZoom
  - VideoEffectPan

---

# Implementar efectos de zoom en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Zoom y paneo son efectos de video integrados en los motores clásicos (Windows / DirectShow) de VisioForge — `VideoCaptureCore`, `MediaPlayerCore` y `VideoEditCore`. Las clases `VideoEffectZoom` y `VideoEffectPan` manejan escalado, centrado y ajustes en tiempo real sin tocar directamente el búfer de frames. Esta es la ruta recomendada cuando quiere zoom.

Solo necesita bajar a `OnVideoFrameBuffer` cuando `VideoEffectZoom` no pueda expresar lo que necesita — por ejemplo, una deformación no-afín personalizada o integración con una biblioteca de imágenes externa.

## Aplicar VideoEffectZoom (recomendado)

`VideoEffectZoom` se agrega al pipeline una sola vez y puede ajustarse mientras el video se reproduce. Escala cada frame automáticamente — sin código C# por frame.

```cs
using VisioForge.Core.Types.VideoEffects;

var zoomEffect = new VideoEffectZoom(
    zoomX: 2.0,    // 2.0 = zoom horizontal 200% (1.0 = sin zoom)
    zoomY: 2.0,    // 2.0 = zoom vertical 200% — mantén igual a zoomX para escalado uniforme
    shiftX: 0,     // Desplazamiento en píxeles desde el centro; positivo desplaza a la derecha
    shiftY: 0,     // Desplazamiento en píxeles desde el centro; positivo desplaza hacia abajo
    enabled: true,
    name: "Zoom");

// VideoCapture1 es una instancia de VideoCaptureCore (misma API en MediaPlayerCore / VideoEditCore).
VideoCapture1.Video_Effects_Enabled = true;
VideoCapture1.Video_Effects_Add(zoomEffect);
```

### Ajustar zoom en tiempo de ejecución

Mantenga una referencia al efecto y modifique sus propiedades mientras el pipeline está corriendo — el SDK recoge los nuevos valores en el siguiente frame:

```cs
zoomEffect.ZoomX = 3.0;
zoomEffect.ZoomY = 3.0;
zoomEffect.ShiftX = 200;   // Enfoque del paneo 200 px a la derecha del centro
zoomEffect.ShiftY = -100;  // Y 100 px hacia arriba
```

Alternar sin quitar:

```cs
zoomEffect.Enabled = false;   // Bypass al vuelo
zoomEffect.Enabled = true;    // Re-activar luego
```

### Calidad de interpolación

`InterpolationMode` por defecto es `VideoInterpolationMode.Bilinear`. Para resultados más nítidos con factores de zoom altos, establezca un modo de mayor calidad; para el menor CPU use `NearestNeighbor`.

```cs
zoomEffect.InterpolationMode = VideoInterpolationMode.Bicubic;
```

## Emparejar con VideoEffectPan

Si quiere una animación suave de paneo sobre una fuente más grande que la salida (por ejemplo, un "Ken Burns" con zoom lento sobre una imagen fija), combina `VideoEffectZoom` con `VideoEffectPan` del mismo espacio de nombres. Dirige ambos desde un timer o una curva de animación.

## Bajar a OnVideoFrameBuffer

Implemente un zoom personalizado a mano solo cuando `VideoEffectZoom` no pueda hacer lo que necesita — por ejemplo, una deformación no-afín, magnificación por-píxel alrededor del cursor, o integración con una biblioteca de imágenes de terceros. Obtiene los bytes del frame, los transforma en sitio (o dentro de `e.Frame.Data`), y establece `e.UpdateData = true` para que los píxeles modificados fluyan río abajo.

```cs
using System.Runtime.InteropServices;

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Frame.Data     — IntPtr al búfer de píxeles
    // e.Frame.DataSize — tamaño del búfer en bytes
    // e.Frame.Info.Width / Info.Height / Info.Stride — dimensiones (RAWBaseVideoInfo)
    // e.Frame.Timestamp — TimeSpan por frame

    // 1. Leer/copiar los bytes a su propio búfer de scratch:
    byte[] scratch = new byte[e.Frame.DataSize];
    Marshal.Copy(e.Frame.Data, scratch, 0, e.Frame.DataSize);

    // 2. Aplicar cualquier transformación personalizada sobre los bytes de `scratch`
    //    (resamplear, deformar, componer, etc.). Mantener el tamaño de salida == entrada
    //    porque el SDK no negociará una nueva resolución a mitad de pipeline.

    // 3. Escribir el resultado de vuelta al búfer original:
    Marshal.Copy(scratch, 0, e.Frame.Data, e.Frame.DataSize);

    // 4. Avisar al pipeline que mutamos los píxeles.
    e.UpdateData = true;
}
```

### Nota sobre los motores X

En los motores X multiplataforma (`VideoCaptureCoreX`, `MediaPlayerCoreX`) el búfer llega en `VideoFrameXBufferEventArgs`. Las dimensiones planas viven directamente en `e.Frame.Width` / `Height` / `Stride`, y los frames llegan en el formato del grabber del motor — RGBA en `VideoCaptureCoreX`, RGB en `MediaPlayerCoreX` (compruebe `e.Frame.Format` antes de envolver). Para matemáticas pesadas de píxeles, envuelve el búfer en `SKPixmap` (SkiaSharp ya es una dependencia transitiva de los motores X).

## Consideraciones de rendimiento

- Prefiera `VideoEffectZoom` sobre la ruta del frame-buffer — el escalador nativo es más rápido y thread-safe.
- Reutilice búferes de scratch en lugar de asignar por frame.
- Mantenga la resolución de salida igual a la de entrada desde el handler — el pipeline no renegocia caps en medio del stream.
- Descargue trabajo pesado de CV / IA a un hilo worker; retorne rápido del handler para evitar back-pressure.

## Conclusión

Para prácticamente todas las aplicaciones, `VideoEffectZoom` (opcionalmente emparejado con `VideoEffectPan`) es la herramienta correcta — es una línea de setup, ajustable en tiempo de ejecución, e implementada en código nativo. `OnVideoFrameBuffer` permanece disponible para los casos en que genuinamente necesita poseer los bytes.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para más ejemplos de código.
