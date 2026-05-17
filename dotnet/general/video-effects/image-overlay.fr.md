---
title: Superposition d'image sur vidéo en .NET — PNG, GIF et bitmap
description: Superposez images, GIF animés et PNG transparents sur des flux vidéo .NET pour filigranes, logos et améliorations visuelles avec transparence.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCore
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Playback
  - Editing
  - GIF
  - C#
primary_api_classes:
  - VideoEffectImageLogo

---

# Superposition d'image

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [MediaPlayerCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Introduction

Cet exemple montre comment superposer une image sur un flux vidéo.

Les images JPG, PNG, BMP et GIF sont prises en charge.

## Exemple de code

Superposition d'image la plus simple avec une image chargée depuis un fichier à une position personnalisée :

```csharp
 var effect = new VideoEffectImageLogo(true, "imageoverlay");
 effect.Filename = @"logo.png";
 effect.Left = 100;
 effect.Top = 100;

 VideoCapture1.Video_Effects_Add(effect);
```

### Superposition d'image transparente

Le SDK prend entièrement en charge la transparence des images PNG. Si vous souhaitez définir un niveau de transparence personnalisé, vous pouvez utiliser la propriété `TransparencyLevel` avec une plage (0..255).

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.Filename = @"logo.jpg";

effect.TransparencyLevel = 50;

VideoCapture1.Video_Effects_Add(effect);
```

### Superposition de GIF animé

Vous pouvez superposer une image GIF animée sur un flux vidéo. Le SDK jouera l'animation du GIF dans la superposition.

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.Filename = @"animated.gif";

effect.Animated = true;
effect.AnimationEnabled = true;

VideoCapture1.Video_Effects_Add(effect);
```

### Superposition d'image à partir de `System.Drawing.Bitmap`

Vous pouvez superposer une image à partir d'un objet `System.Drawing.Bitmap`.

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.MemoryBitmap = new Bitmap("logo.jpg");
VideoCapture1.Video_Effects_Add(effect);
```

### Superposition d'image à partir d'un tableau d'octets RGB/RGBA

Vous pouvez superposer une image à partir de données RGB/RGBA.

```csharp
// ajouter le logo image
var effect = new VideoEffectImageLogo(true, "imageoverlay");

// charger l'image depuis un fichier JPG
var bitmap = new Bitmap("logo.jpg");

// verrouiller les données du bitmap et enregistrer en tant que données octet (IntPtr)
var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
var pixels = Marshal.AllocCoTaskMem(bitmapData.Stride * bitmapData.Height);
NativeAPI.CopyMemory(pixels, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height);
bitmap.UnlockBits(bitmapData);

// affecter les données à l'effet
effect.Bitmap = pixels;

// définir les propriétés du bitmap
effect.BitmapWidth = bitmap.Width;
effect.BitmapHeight = bitmap.Height;
effect.BitmapDepth = 3; // RGB24

// libérer le bitmap
bitmap.Dispose();

// ajouter l'effet
VideoCapture1.Video_Effects_Add(effect);
```

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.