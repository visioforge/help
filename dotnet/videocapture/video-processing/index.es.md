---
title: Procesamiento de Video en Video Capture SDK
description: Guía de procesamiento de video incluyendo redimensionado, recorte, efectos y transformaciones con Video Capture SDK .Net.
---

# Procesamiento de Video - Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre las capacidades de procesamiento de video disponibles en Video Capture SDK .Net, incluyendo transformaciones, efectos y filtros.

## Capacidades de Procesamiento

### Transformaciones Básicas

- **Redimensionar** - Cambiar resolución de video
- **Recortar** - Eliminar bordes del video
- **Rotar** - Rotar 90°, 180° o 270°
- **Voltear** - Voltear horizontal o verticalmente

### Ajustes de Color

- Brillo
- Contraste
- Saturación
- Tono
- Gamma

### Efectos

- Superposición de texto
- Superposición de imagen/logo
- Marca de tiempo
- Efectos de desenfoque

## Guías Disponibles

### Redimensionar y Recortar

Aprenda cómo cambiar el tamaño y recortar el video.

[Ver guía →](resize-crop.md)

### Efectos de Video

Aplicar efectos visuales al video.

[Ver guía →](video-effects.md)

### Mezcla de Video

Combinar múltiples fuentes de video.

[Ver guía →](video-mixing.md)

## Ejemplo: Redimensionar Video

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Redimensionar a 1280x720
var resizeBlock = new VideoResizeBlock(new VideoResizeSettings 
{ 
    Width = 1280, 
    Height = 720 
});

pipeline.Connect(videoSource.Output, resizeBlock.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(resizeBlock.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Ejemplo: Ajustes de Color

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Ajustar brillo y contraste
var balanceBlock = new VideoBalanceBlock(new VideoBalanceSettings 
{ 
    Brightness = 0.1,   // +10% brillo
    Contrast = 1.2,     // +20% contraste
    Saturation = 1.1    // +10% saturación
});

pipeline.Connect(videoSource.Output, balanceBlock.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(balanceBlock.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Ejemplo: Superposición de Texto

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Añadir texto sobre el video
var textOverlay = new TextOverlayBlock(new TextOverlaySettings 
{ 
    Text = "Mi Video",
    FontFamily = "Arial",
    FontSize = 32,
    FontColor = SKColors.White,
    X = 10,
    Y = 10
});

pipeline.Connect(videoSource.Output, textOverlay.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(textOverlay.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Cadena de Procesamiento

Puede encadenar múltiples bloques de procesamiento:

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// 1. Redimensionar
var resize = new VideoResizeBlock(new VideoResizeSettings { Width = 1280, Height = 720 });

// 2. Ajustar colores
var balance = new VideoBalanceBlock(new VideoBalanceSettings { Brightness = 0.1 });

// 3. Añadir logo
var imageOverlay = new ImageOverlayBlock(new ImageOverlaySettings 
{ 
    ImagePath = "logo.png",
    X = 10,
    Y = 10,
    Alpha = 0.8
});

// Conectar en cadena
pipeline.Connect(videoSource.Output, resize.Input);
pipeline.Connect(resize.Output, balance.Input);
pipeline.Connect(balance.Output, imageOverlay.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageOverlay.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Consideraciones de Rendimiento

1. **Orden de operaciones**: Redimensione antes de aplicar efectos costosos
2. **Aceleración GPU**: Use bloques acelerados por GPU cuando estén disponibles
3. **Resolución**: Procesar video de menor resolución es más rápido
