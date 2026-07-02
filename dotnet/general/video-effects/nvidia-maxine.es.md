---
title: "Mejora de video con IA NVIDIA Maxine en C# .NET (RTX)"
description: Mejore el video con la IA NVIDIA Maxine en C# .NET — superresolución, escalado, reducción de ruido y reducción de artefactos en GPU RTX.
sidebar_label: NVIDIA Maxine
tags:
  - Video Capture SDK
  - Media Player SDK
  - .NET
  - NVIDIA Maxine
  - AI
  - Super Resolution
  - Windows
primary_api_classes:
  - MaxineDenoiseVideoEffect
  - MaxineArtifactReductionVideoEffect
  - MaxineSuperResSettings
  - MaxineUpscaleSettings
---

# Mejora de video con IA NVIDIA Maxine en C# .NET

NVIDIA Maxine aporta mejora de video con IA acelerada por GPU a los motores clásicos de VisioForge
(`VideoCaptureCore` y `MediaPlayerCore`). Los efectos se ejecutan sobre el NVIDIA Maxine SDK y requieren una
**GPU NVIDIA RTX** (núcleos Tensor). Son solo para Windows.

| Efecto | Tipo de API | Se aplica mediante |
| --- | --- | --- |
| Reducción de ruido | `MaxineDenoiseVideoEffect` | `Video_Effects_Add` |
| Reducción de artefactos | `MaxineArtifactReductionVideoEffect` | `Video_Effects_Add` |
| Superresolución | `MaxineSuperResSettings` | `Video_Resize` |
| Escalado | `MaxineUpscaleSettings` | `Video_Resize` |

## Requisitos

- GPU NVIDIA RTX (GeForce RTX 2060 o superior) con controladores actuales.
- El NVIDIA Maxine SDK con su **directorio de modelos** en disco — la mayoría de los constructores reciben la ruta a él.
- Windows 10/11. Estos efectos se dirigen a los motores `VideoCaptureCore` / `MediaPlayerCore` basados en DirectShow.

Las clases de efecto de reducción de ruido/artefactos residen en `VisioForge.Core.Types.VideoEffects` (reducción de ruido en
`VisioForge.Core.Types.VideoEffects.NvidiaMaxine`); los ajustes de superresolución y escalado residen en
`VisioForge.Core.Types`.

!!! note "Habilite el pipeline de efectos"
    Los efectos de filtro añadidos con `Video_Effects_Add` solo se ejecutan cuando el pipeline de efectos está habilitado. Establezca
    `Video_Effects_Enabled = true` una vez antes de añadir efectos — está en `false` de forma predeterminada.

## Reducción de ruido

Elimina el ruido de cámara/sensor preservando el detalle. `Strength` va de 0.0 a 1.0 (predeterminado 0.7).

```csharp
using VisioForge.Core.Types.VideoEffects.NvidiaMaxine;

VideoCapture1.Video_Effects_Enabled = true; // los efectos están desactivados por defecto

var denoise = new MaxineDenoiseVideoEffect(modelsDir, strength: 0.7f);
VideoCapture1.Video_Effects_Add(denoise);
```

## Reducción de artefactos

Elimina artefactos de compresión (bloques, ringing, banding). Elija el modo según la tasa de bits de la fuente.

```csharp
using VisioForge.Core.Types.VideoEffects;

VideoCapture1.Video_Effects_Enabled = true;

var artifactReduction = new MaxineArtifactReductionVideoEffect(
    modelsDir,
    mode: MaxineArtifactReductionEffectMode.LowBitrate);
VideoCapture1.Video_Effects_Add(artifactReduction);
```

| `MaxineArtifactReductionEffectMode` | Úselo para |
| --- | --- |
| `HighBitrate` | Fuentes de 10+ Mbps; más suave, preserva gradientes y detalle fino. |
| `LowBitrate` | Por debajo de ~5 Mbps; eliminación agresiva de artefactos fuertes (predeterminado). |

## Superresolución

Escalado con IA a una altura objetivo. Asigne los ajustes a `Video_Resize` y habilite resize/crop. El ancho se
calcula para preservar la relación de aspecto.

```csharp
using VisioForge.Core.Types;

VideoCapture1.Video_Resize = new MaxineSuperResSettings(modelsDir, height: 2160)
{
    Mode = MaxineSuperResolutionEffectMode.HQSource,
};
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

| `MaxineSuperResolutionEffectMode` | Úselo para |
| --- | --- |
| `HQSource` | Fuentes de alta calidad/alta tasa de bits; prioriza la eliminación de artefactos (predeterminado). |
| `LQSource` | Fuentes muy comprimidas; prioriza la mejora del detalle. |

## Escalado

Un escalador más ligero (frente a la superresolución) con una `Strength` ajustable (0.0–1.0, predeterminado 0.4).

```csharp
using VisioForge.Core.Types;

VideoCapture1.Video_Resize = new MaxineUpscaleSettings(modelsDir, height: 1080, strength: 0.4f);
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

## Motor Media Player

Los mismos efectos se aplican al motor `MediaPlayerCore` para la mejora con IA durante la reproducción. Establezca
`MediaPlayer1.Video_Effects_Enabled = true`, luego use `MediaPlayer1.Video_Effects_Add(...)` para
reducción de ruido/artefactos, y asigne `MediaPlayer1.Video_Resize` para superresolución/escalado. El reproductor no
tiene un indicador `Video_ResizeOrCrop_Enabled` — asigne `Video_Resize` antes de iniciar la reproducción; se aplica
cuando se construye el grafo de reproducción (cambiarlo durante la reproducción activa solo surte efecto en el siguiente inicio).

## Demos

- **Nvidia Maxine Demo** (Video Capture, WPF) — [Nvidia Maxine Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Nvidia%20Maxine%20Demo).
- **Nvidia Maxine Player** (Media Player, WPF) — [Nvidia Maxine Player](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Nvidia%20Maxine%20Player).

## Véase también

- [Referencia de Efectos](effects-reference.md) — catálogo completo de efectos de CPU, GPU e IA.
- [Añadir Efectos](add.md)
