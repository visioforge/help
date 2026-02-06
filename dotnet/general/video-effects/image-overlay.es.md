---
title: Superposiciones de Imagen en Flujos de Video
description: Superponga imágenes, GIFs animados y PNGs transparentes en flujos de video en .NET para marcas de agua, logotipos y mejoras visuales con transparencia.
---

# Superposición de imagen

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [MediaPlayerCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Introducción

Este ejemplo demuestra cómo superponer una imagen en un flujo de video.

Se soportan imágenes JPG, PNG, BMP y GIF.

## Código de ejemplo

Superposición de imagen más simple con imagen agregada desde un archivo con posición personalizada:

```csharp
 var effect = new VideoEffectImageLogo(true, "imageoverlay");
 effect.Filename = @"logo.png";
 effect.Left = 100;
 effect.Top = 100;

 VideoCapture1.Video_Effects_Add(effect);
```

### Superposición de imagen transparente

El SDK soporta completamente la transparencia en imágenes PNG. Si desea establecer un nivel de transparencia personalizado, puede usar la propiedad `TransparencyLevel` con un rango (0..255).

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.Filename = @"logo.jpg";

effect.TransparencyLevel = 50;

VideoCapture1.Video_Effects_Add(effect);
```

### Superposición de GIF animado

Puede superponer una imagen GIF animada en un flujo de video. El SDK reproducirá la animación GIF en la superposición.

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.Filename = @"animated.gif";

effect.Animated = true;
effect.AnimationEnabled = true;

VideoCapture1.Video_Effects_Add(effect);
```

### Superposición de imagen desde `System.Drawing.Bitmap`

Puede superponer una imagen desde un objeto `System.Drawing.Bitmap`.

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.MemoryBitmap = new Bitmap("logo.jpg");
VideoCapture1.Video_Effects_Add(effect);
```

### Superposición de imagen desde array de bytes RGB/RGBA

Puede superponer una imagen desde datos RGB/RGBA.

```csharp
// agregar logotipo de imagen
var effect = new VideoEffectImageLogo(true, "imageoverlay");

// cargar imagen desde archivo JPG
var bitmap = new Bitmap("logo.jpg");

// bloquear datos de bitmap y guardar en datos de bytes (IntPtr)
var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
var pixels = Marshal.AllocCoTaskMem(bitmapData.Stride * bitmapData.Height);
NativeAPI.CopyMemory(pixels, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height);
bitmap.UnlockBits(bitmapData);

// establecer datos al efecto
effect.Bitmap = pixels;

// establecer propiedades del bitmap
effect.BitmapWidth = bitmap.Width;
effect.BitmapHeight = bitmap.Height;
effect.BitmapDepth = 3; // RGB24

// liberar bitmap
bitmap.Dispose();

// agregar efecto
VideoCapture1.Video_Effects_Add(effect);
```

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.