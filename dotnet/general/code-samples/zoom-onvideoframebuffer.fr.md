---
title: Effets de zoom et panoramique vidéo en C# .NET VisioForge
description: Effets de zoom et de panoramique en C# .NET avec VideoEffectZoom et VideoEffectPan — point focal ajustable pour capture, lecture et édition.
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

# Implémenter des effets de zoom en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Le zoom et le panoramique sont des effets vidéo intégrés aux moteurs classiques (Windows / DirectShow) de VisioForge — `VideoCaptureCore`, `MediaPlayerCore` et `VideoEditCore`. Les classes `VideoEffectZoom` et `VideoEffectPan` gèrent la mise à l'échelle, le centrage et les ajustements à l'exécution sans toucher directement au tampon d'image. C'est la voie recommandée lorsque vous voulez du zoom.

Vous n'avez besoin de descendre au niveau de `OnVideoFrameBuffer` que lorsque `VideoEffectZoom` ne peut pas exprimer ce dont vous avez besoin — par exemple, une déformation non affine personnalisée ou l'intégration avec une bibliothèque d'images externe.

## Appliquer VideoEffectZoom (recommandé)

`VideoEffectZoom` s'ajoute au pipeline une seule fois et peut être ajusté pendant la lecture vidéo. Il met à l'échelle chaque image automatiquement — pas de code C# par image.

```cs
using VisioForge.Core.Types.VideoEffects;

var zoomEffect = new VideoEffectZoom(
    zoomX: 2.0,    // 2.0 = zoom horizontal 200 % (1.0 = pas de zoom)
    zoomY: 2.0,    // 2.0 = zoom vertical 200 % — gardez égal à zoomX pour une mise à l'échelle uniforme
    shiftX: 0,     // Décalage en pixels depuis le centre ; positif déplace vers la droite
    shiftY: 0,     // Décalage en pixels depuis le centre ; positif déplace vers le bas
    enabled: true,
    name: "Zoom");

// VideoCapture1 est une instance de VideoCaptureCore (même API sur MediaPlayerCore / VideoEditCore).
VideoCapture1.Video_Effects_Enabled = true;
VideoCapture1.Video_Effects_Add(zoomEffect);
```

### Ajuster le zoom à l'exécution

Conservez une référence à l'effet et modifiez ses propriétés pendant que le pipeline tourne — le SDK récupère les nouvelles valeurs à la prochaine image :

```cs
zoomEffect.ZoomX = 3.0;
zoomEffect.ZoomY = 3.0;
zoomEffect.ShiftX = 200;   // Mettre la mise au point 200 px à droite du centre
zoomEffect.ShiftY = -100;  // Et 100 px vers le haut
```

Bascule sans suppression :

```cs
zoomEffect.Enabled = false;   // Contournement à la volée
zoomEffect.Enabled = true;    // Réactiver plus tard
```

### Qualité d'interpolation

`InterpolationMode` est par défaut `VideoInterpolationMode.Bilinear`. Pour des résultats plus nets à des facteurs de zoom élevés, choisissez un mode de qualité supérieure ; pour minimiser le CPU, utilisez `NearestNeighbor`.

```cs
zoomEffect.InterpolationMode = VideoInterpolationMode.Bicubic;
```

## Combiner avec VideoEffectPan

Si vous voulez une animation de panoramique fluide sur une source plus grande que la sortie (par exemple, un zoom lent « Ken Burns » sur une image fixe), combinez `VideoEffectZoom` avec `VideoEffectPan` du même espace de noms. Pilotez les deux depuis un minuteur ou une courbe d'animation.

## Descendre au niveau de OnVideoFrameBuffer

N'implémentez un zoom personnalisé à la main que lorsque `VideoEffectZoom` ne peut pas faire ce dont vous avez besoin — par exemple, une déformation non affine, un agrandissement par pixel autour du curseur, ou l'intégration avec une bibliothèque d'imagerie tierce. Vous obtenez les octets bruts de l'image, vous les transformez sur place (ou dans `e.Frame.Data`), et vous définissez `e.UpdateData = true` pour que les pixels modifiés circulent en aval.

```cs
using System.Runtime.InteropServices;

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Frame.Data     — IntPtr vers le tampon de pixels
    // e.Frame.DataSize — taille du tampon en octets
    // e.Frame.Info.Width / Info.Height / Info.Stride — dimensions de l'image (RAWBaseVideoInfo)
    // e.Frame.Timestamp — TimeSpan par image

    // 1. Lire/copier les octets dans votre propre tampon temporaire :
    byte[] scratch = new byte[e.Frame.DataSize];
    Marshal.Copy(e.Frame.Data, scratch, 0, e.Frame.DataSize);

    // 2. Appliquer la transformation personnalisée dont vous avez besoin sur les octets de `scratch`
    //    (rééchantillonnage, déformation, composition, etc.). Gardez la taille de sortie == entrée
    //    car le SDK ne négociera pas une nouvelle résolution en cours de pipeline.

    // 3. Réécrire le résultat dans le tampon d'origine :
    Marshal.Copy(scratch, 0, e.Frame.Data, e.Frame.DataSize);

    // 4. Indiquer au pipeline que nous avons modifié les pixels.
    e.UpdateData = true;
}
```

### Note sur les moteurs X

Sur les moteurs X multiplateformes (`VideoCaptureCoreX`, `MediaPlayerCoreX`), le tampon arrive dans `VideoFrameXBufferEventArgs`. Les dimensions plates résident directement sur `e.Frame.Width` / `Height` / `Stride`, et les images sont généralement en BGRA. Pour des calculs de pixels lourds, enveloppez le tampon dans `SKPixmap` (SkiaSharp est déjà une dépendance transitive des moteurs X).

## Considérations de performance

- Préférez `VideoEffectZoom` au chemin par tampon d'image — le scaleur natif est plus rapide et thread-safe.
- Réutilisez les tampons temporaires plutôt que d'en allouer un par image.
- Gardez la résolution de sortie égale à la résolution d'entrée depuis le gestionnaire — le pipeline ne renégocie pas les caps en cours de flux.
- Déléguez les travaux lourds de CV / IA à un thread worker ; retournez rapidement du gestionnaire d'événements pour éviter la contre-pression.

## Conclusion

Pour pratiquement toutes les applications, `VideoEffectZoom` (éventuellement combiné avec `VideoEffectPan`) est le bon outil — c'est une ligne de configuration, ajustable à l'exécution et implémenté en code natif. `OnVideoFrameBuffer` reste disponible pour les cas où vous avez véritablement besoin de posséder les octets.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.
