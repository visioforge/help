---
title: Implementar Efectos de Zoom en Aplicaciones .NET de Video
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

# Implementar Efectos de Zoom en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Zoom y paneo son efectos de video integrados en los motores clásicos (Windows / DirectShow) de VisioForge — `VideoCaptureCore`, `MediaPlayerCore` y `VideoEditCore`. Las clases `VideoEffectZoom` y `VideoEffectPan` manejan escalado, centrado y ajustes en tiempo real sin tocar directamente el búfer de frames. Esta es la ruta recomendada cuando quieres zoom.

Solo necesitas bajar a `OnVideoFrameBuffer` cuando `VideoEffectZoom` no pueda expresar lo que necesitas — por ejemplo, una deformación no-afín personalizada o integración con una biblioteca de imágenes externa.

## Aplicando VideoEffectZoom (recomendado)

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

Mantén una referencia al efecto y modifica sus propiedades mientras el pipeline está corriendo — el SDK recoge los nuevos valores en el siguiente frame:

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

`InterpolationMode` por defecto es `VideoInterpolationMode.Bilinear`. Para resultados más nítidos con factores de zoom altos, establece un modo de mayor calidad; para el menor CPU usa `NearestNeighbor`.

```cs
zoomEffect.InterpolationMode = VideoInterpolationMode.Bicubic;
```

## Emparejando con VideoEffectPan

Si quieres una animación suave de paneo sobre una fuente más grande que la salida (por ejemplo, un "Ken Burns" con zoom lento sobre una imagen fija), combina `VideoEffectZoom` con `VideoEffectPan` del mismo espacio de nombres. Dirige ambos desde un timer o una curva de animación.

## Bajar a OnVideoFrameBuffer

Implementa un zoom personalizado a mano solo cuando `VideoEffectZoom` no pueda hacer lo que necesitas — por ejemplo, una deformación no-afín, magnificación por-píxel alrededor del cursor, o integración con una biblioteca de imágenes de terceros. Obtienes los bytes del frame, los transformas en sitio (o dentro de `e.Frame.Data`), y estableces `e.UpdateData = true` para que los píxeles modificados fluyan río abajo.

```cs
using System.Runtime.InteropServices;

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Frame.Data     — IntPtr al búfer de píxeles
    // e.Frame.DataSize — tamaño del búfer en bytes
    // e.Frame.Info.Width / Info.Height / Info.Stride — dimensiones (RAWBaseVideoInfo)
    // e.Frame.Timestamp — TimeSpan por frame

    // 1. Lee/copia los bytes a tu propio búfer de scratch:
    byte[] scratch = new byte[e.Frame.DataSize];
    Marshal.Copy(e.Frame.Data, scratch, 0, e.Frame.DataSize);

    // 2. Aplica cualquier transformación personalizada sobre los bytes de `scratch`
    //    (resamplear, deformar, componer, etc.). Mantén el tamaño de salida == entrada
    //    porque el SDK no negociará una nueva resolución a mitad de pipeline.

    // 3. Escribe el resultado de vuelta al búfer original:
    Marshal.Copy(scratch, 0, e.Frame.Data, e.Frame.DataSize);

    // 4. Avisa al pipeline que mutamos los píxeles.
    e.UpdateData = true;
}
```

### Nota sobre los motores X

En los motores X multiplataforma (`VideoCaptureCoreX`, `MediaPlayerCoreX`) el búfer llega en `VideoFrameXBufferEventArgs`. Las dimensiones planas viven directamente en `e.Frame.Width` / `Height` / `Stride`, y los frames típicamente son BGRA. Para matemáticas pesadas de píxeles, envuelve el búfer en `SKPixmap` (SkiaSharp ya es una dependencia transitiva de los motores X).

## Consideraciones de Rendimiento

- Prefiere `VideoEffectZoom` sobre la ruta del frame-buffer — el escalador nativo es más rápido y thread-safe.
- Reutiliza búferes de scratch en lugar de asignar por frame.
- Mantén la resolución de salida igual a la de entrada desde el handler — el pipeline no renegocia caps en medio del stream.
- Descarga trabajo pesado de CV / IA a un hilo worker; retorna rápido del handler para evitar back-pressure.

## Conclusión

Para prácticamente todas las aplicaciones, `VideoEffectZoom` (opcionalmente emparejado con `VideoEffectPan`) es la herramienta correcta — es una línea de setup, ajustable en tiempo de ejecución, e implementada en código nativo. `OnVideoFrameBuffer` permanece disponible para los casos en que genuinamente necesitas poseer los bytes.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para más ejemplos de código.
