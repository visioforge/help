---
title: Dessiner du texte sur les images vidéo en C# .NET VisioForge
description: Dessinez du texte dynamique sur les images vidéo en C# / .NET avec l'événement OnVideoFrameBuffer. Horodatages, données de capteur, polices personnalisées.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - C#
primary_api_classes:
  - VideoEffectTextLogo
  - VideoFrameBufferEventArgs
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - MediaPlayerCoreX

---

# Créer des superpositions de texte personnalisées avec OnVideoFrameBuffer en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

L'événement `OnVideoFrameBuffer` donne un accès direct, au niveau du pixel, à chaque image vidéo qui passe dans le pipeline. Dessiner du texte sur l'image — horodatages, valeurs de capteurs, télémétrie de débogage ou image de marque personnalisée — est l'un des usages les plus courants. Ce guide montre comment rendre du texte sur des images brutes avec un contrôle total de la police, de la couleur, de la position et de la logique par image.

!!! tip "Vous cherchez la fonctionnalité de superposition de texte de haut niveau ?"
    Si vous avez seulement besoin d'une superposition de texte statique, animée ou pilotée par horloge avec un positionnement standard, utilisez l'[effet de superposition de texte](../video-effects/text-overlay.md) dédié — une ligne de code via `Video_Effects_Add(new VideoEffectTextLogo(...))`. Utilisez `OnVideoFrameBuffer` (cette page) lorsque vous avez besoin d'un **contrôle au niveau pixel** : polices personnalisées, mise en page avancée, contenu dynamique par image, ou intégration avec des bibliothèques de texte/graphiques tierces.

### Moteurs pris en charge

L'événement `OnVideoFrameBuffer` est exposé sur les deux familles de moteurs :

| Moteur | Type d'arguments d'événement | Format de pixel |
|---|---|---|
| `VideoCaptureCore` (DirectShow, Windows) | `VideoFrameBufferEventArgs` | RGB24 / RGB32 |
| `VideoCaptureCoreX` (GStreamer, multiplateforme) | `VideoFrameXBufferEventArgs` | BGRA (le plus courant) |
| `MediaPlayerCoreX` (GStreamer, multiplateforme) | `VideoFrameXBufferEventArgs` | BGRA (le plus courant) |

Les deux moteurs suivent le même modèle — s'abonner à l'événement, lire `e.Frame.Data` (un `IntPtr`) avec `Width` / `Height` / `Stride`, rendre dans le tampon sur place, et définir `e.UpdateData = true` pour propager les changements en aval.

## Comprendre l'événement OnVideoFrameBuffer

L'événement OnVideoFrameBuffer est un point d'accroche puissant qui donne aux développeurs un accès direct au tampon d'image vidéo pendant le traitement. Cet événement se déclenche pour chaque image vidéo, offrant une opportunité de modifier les données de l'image avant son affichage ou son encodage.

Les principaux avantages de l'utilisation de OnVideoFrameBuffer pour les superpositions de texte sont :

- **Accès au niveau de l'image** : modifier les images individuelles avec une précision au pixel près
- **Contenu dynamique** : mettre à jour le texte en fonction de données en temps réel ou d'horodatages
- **Style personnalisé** : appliquer des polices, des couleurs et des effets personnalisés au-delà de ce que proposent les API intégrées
- **Optimisations de performance** : implémenter des techniques de rendu efficaces pour des applications hautes performances

## Aperçu de l'implémentation

La technique présentée ici utilise les composants suivants :

1. Un gestionnaire d'événements pour OnVideoFrameBuffer qui traite chaque image vidéo
2. Un objet VideoEffectTextLogo pour définir les propriétés du texte
3. L'API FastImageProcessing pour rendre le texte sur le tampon d'image

Cette approche est particulièrement utile lorsque vous avez besoin de :

- Afficher des données dynamiques comme des horodatages, des métadonnées ou des valeurs de capteurs
- Créer des effets de texte animés
- Positionner le texte avec une précision au pixel près
- Appliquer un style personnalisé non disponible via les API standard

## Implémentation d'un exemple de code

L'exemple C# suivant montre comment implémenter un système de superposition de texte basique avec l'événement OnVideoFrameBuffer :

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logoInitiated)
    {
        logoInitiated = true;

        InitTextLogo();
    }

    // AddTextLogo(context, pixels, pixels32bit, pixels32tmp, frameWidth, frameHeight,
    //             ref textLogo, timeStamp, frameNumber)
    // Passer pixels32bit: false + pixels32tmp: IntPtr.Zero pour les images RGB24.
    FastImageProcessing.AddTextLogo(
        context: null,
        pixels: e.Frame.Data,
        pixels32bit: false,
        pixels32tmp: IntPtr.Zero,
        frameWidth: e.Frame.Info.Width,
        frameHeight: e.Frame.Info.Height,
        textLogo: ref textLogo,
        timeStamp: e.Frame.Timestamp,
        frameNumber: 0);
}

private bool logoInitiated = false;

private VideoEffectTextLogo textLogo = null;

private void InitTextLogo()
{
    textLogo = new VideoEffectTextLogo(true);
    textLogo.Text = "Hello world!";
    textLogo.Left = 50;
    textLogo.Top = 50;
}
```

## Explication détaillée du code

Décomposons les composants clés de cette implémentation :

### Le gestionnaire d'événements

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
```

Cette méthode est déclenchée pour chaque image vidéo. Le VideoFrameBufferEventArgs fournit l'accès à :

- Données de l'image (tampon de pixels)
- Dimensions de l'image (largeur et hauteur)
- Informations d'horodatage

### Logique d'initialisation

```cs
if (!logoInitiated)
{
    logoInitiated = true;
    InitTextLogo();
}
```

Ce code garantit que le logo de texte n'est initialisé qu'une seule fois, ce qui évite la création inutile d'objets pour chaque image. Ce modèle est important pour les performances lors du traitement de vidéo à hautes fréquences d'images.

### Configuration du logo de texte

```cs
private void InitTextLogo()
{
    textLogo = new VideoEffectTextLogo(true);
    textLogo.Text = "Hello world!";
    textLogo.Left = 50;
    textLogo.Top = 50;
}
```

La classe VideoEffectTextLogo est utilisée pour définir les propriétés de la superposition de texte :

- Le contenu du texte (« Hello world! »)
- Coordonnées de position (50 pixels depuis la gauche et le haut)

### Rendu de la superposition de texte

```cs
FastImageProcessing.AddTextLogo(
    context: null,
    pixels: e.Frame.Data,
    pixels32bit: false,        // true lorsque le moteur fournit du RGB32
    pixels32tmp: IntPtr.Zero,  // tampon temporaire optionnel ; IntPtr.Zero laisse l'utilitaire allouer à la demande
    frameWidth:  e.Frame.Info.Width,
    frameHeight: e.Frame.Info.Height,
    textLogo: ref textLogo,
    timeStamp: e.Frame.Timestamp,
    frameNumber: 0);
```

La signature à 9 arguments reflète exactement `FastImageProcessing.AddTextLogo`. La largeur/hauteur résident dans `e.Frame.Info` sur la structure classique `VideoFrame` ; l'horodatage réside dans `e.Frame.Timestamp`. Passez `pixels32bit: true` lorsque votre source est en RGB32.

## Options de personnalisation avancées

Bien que l'exemple de base montre une simple superposition de texte statique, la classe VideoEffectTextLogo prend en charge de nombreuses options de personnalisation :

### Mise en forme du texte

```cs
// Font est un System.Drawing.Font complet — toute combinaison police + style fonctionne.
textLogo.Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold);
textLogo.FontColor = System.Drawing.Color.White;
textLogo.TransparencyLevel = 200;   // 0 (totalement transparent) - 255 (opaque)
```

### Arrière-plan et bordures

```cs
textLogo.BackgroundTransparent = false;
textLogo.BackgroundColor = System.Drawing.Color.Black;

// L'anneau de bordure se configure via BorderMode + couleurs et tailles par anneau (interne/externe).
// Valeurs de TextEffectMode : None, Inner, Outer, InnerAndOuter, Embossed, Outline, FilledOutline, Halo.
textLogo.BorderMode = TextEffectMode.InnerAndOuter;
textLogo.BorderInnerColor = System.Drawing.Color.Yellow;
textLogo.BorderInnerSize = 2;
textLogo.BorderOuterColor = System.Drawing.Color.Black;
textLogo.BorderOuterSize = 1;
```

### Animation et contenu dynamique

Pour du contenu dynamique qui change à chaque image :

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logoInitiated)
    {
        logoInitiated = true;
        InitTextLogo();
    }

    // L'horodatage réside dans e.Frame.Timestamp (TimeSpan)
    textLogo.Text = $"Timestamp: {e.Frame.Timestamp:hh\\:mm\\:ss\\.fff}";

    // Animer la position
    textLogo.Left = 50 + (int)(Math.Sin(e.Frame.Timestamp.TotalSeconds) * 50);

    FastImageProcessing.AddTextLogo(
        context: null,
        pixels: e.Frame.Data,
        pixels32bit: false,
        pixels32tmp: IntPtr.Zero,
        frameWidth:  e.Frame.Info.Width,
        frameHeight: e.Frame.Info.Height,
        textLogo: ref textLogo,
        timeStamp: e.Frame.Timestamp,
        frameNumber: 0);
}
```

## Considérations de performance

Lors de l'implémentation de superpositions de texte personnalisées, prenez en compte ces bonnes pratiques de performance :

1. **Initialisez les objets une seule fois** : créez l'objet VideoEffectTextLogo une seule fois, pas par image
2. **Minimisez les changements de texte** : mettez à jour le contenu du texte uniquement lorsque c'est nécessaire
3. **Utilisez des polices efficaces** : les polices simples se rendent plus vite que les complexes
4. **Tenez compte de la résolution** : les vidéos de plus haute résolution nécessitent plus de puissance de traitement
5. **Testez sur le matériel cible** : assurez-vous que votre implémentation fonctionne bien sur les systèmes de production

## Plusieurs éléments de texte

Pour afficher plusieurs éléments de texte sur la même image :

```cs
private VideoEffectTextLogo titleLogo = null;
private VideoEffectTextLogo timestampLogo = null;

private void InitTextLogos()
{
    titleLogo = new VideoEffectTextLogo(true);
    titleLogo.Text = "Camera Feed";
    titleLogo.Left = 50;
    titleLogo.Top = 50;
    
    timestampLogo = new VideoEffectTextLogo(true);
    timestampLogo.Left = 50;
    timestampLogo.Top = 100;
}

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logosInitiated)
    {
        logosInitiated = true;
        InitTextLogos();
    }
    
    // Mettre à jour le contenu dynamique
    timestampLogo.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

    // Rendre les deux éléments de texte
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, false, IntPtr.Zero,
        e.Frame.Info.Width, e.Frame.Info.Height, ref titleLogo, e.Frame.Timestamp, 0);
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, false, IntPtr.Zero,
        e.Frame.Info.Width, e.Frame.Info.Height, ref timestampLogo, e.Frame.Timestamp, 0);
}
```

## Exemple VideoCaptureCoreX / MediaPlayerCoreX (moteurs X)

Sur les moteurs X multiplateformes, la signature de l'événement est `VideoFrameXBufferEventArgs` et le tampon d'image arrive généralement au format **BGRA** (4 octets par pixel). L'exemple ci-dessous utilise SkiaSharp pour envelopper le tampon brut et y dessiner du texte ; SkiaSharp est une dépendance transitive des moteurs X, donc aucun paquet NuGet supplémentaire n'est nécessaire.

```cs
using SkiaSharp;

// Créer les paints une fois, les réutiliser entre les images
private SKPaint _textPaint = new SKPaint
{
    Color = SKColors.White,
    TextSize = 32,
    IsAntialias = true,
    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
};

private SKPaint _shadowPaint = new SKPaint
{
    Color = SKColors.Black.WithAlpha(160),
    TextSize = 32,
    IsAntialias = true,
    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
};

// S'abonner après avoir construit VideoCaptureCoreX / MediaPlayerCoreX
_videoCapture.OnVideoFrameBuffer += VideoCapture_OnVideoFrameBuffer;

private void VideoCapture_OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
{
    if (e.Frame == null || e.Frame.Data == IntPtr.Zero)
    {
        return;
    }

    // Envelopper le tampon BGRA brut dans une surface SkiaSharp (sans allocation supplémentaire)
    var info = new SKImageInfo(e.Frame.Width, e.Frame.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

    using (var pixmap = new SKPixmap(info, e.Frame.Data, e.Frame.Stride))
    using (var surface = SKSurface.Create(pixmap))
    {
        var canvas = surface.Canvas;

        // Contenu dynamique construit par image
        var timestamp = e.Frame.Timestamp.ToString(@"hh\:mm\:ss\.fff");
        var line = $"REC  {timestamp}";

        // Dessiner d'abord l'ombre, puis le texte principal pour la lisibilité sur n'importe quel arrière-plan
        canvas.DrawText(line, 18, 42, _shadowPaint);
        canvas.DrawText(line, 16, 40, _textPaint);
        canvas.Flush();
    }

    // Propager l'image modifiée en aval
    e.UpdateData = true;
}
```

**Pourquoi BGRA importe.** Les moteurs X demandent BGRA par défaut pour les rappels d'image, car il correspond 1:1 à SkiaSharp, System.Drawing et la plupart des chemins d'interopérabilité conviviaux pour le GPU. Si vous avez besoin d'un autre format, demandez un bloc de conversion de format en amont plutôt que de convertir à chaque image.

**Mesurer et positionner le texte.** Utilisez `_textPaint.MeasureText(line)` pour calculer la largeur pour un alignement à droite ou centré. SkiaSharp expose aussi `SKFontMetrics` via `_textPaint.FontMetrics` pour la ligne de base / ascent / descent afin de positionner le texte avec précision contre les bords de l'image.

**Piles d'imagerie alternatives.** Vous pouvez aussi utiliser `System.Drawing.Graphics` enveloppant un `Bitmap` construit sur le tampon brut sur Windows, ou des écritures d'octets directes avec `Marshal.Copy` / `Span<byte>` pour un contrôle total. SkiaSharp est l'option recommandée sur macOS / Linux / iOS / Android.

**Parité au niveau du moteur.** Tout ce qui figure dans [Considérations de performance](#considerations-de-performance) et [Plusieurs éléments de texte](#plusieurs-elements-de-texte) s'applique également aux moteurs X — l'événement est déclenché sur un thread de traitement, `UpdateData` propage les changements et le travail lourd doit être délégué pour éviter de perdre des images.

## Composants requis

Pour implémenter cette solution, vous aurez besoin de :

- Paquet redistribuable du SDK installé dans votre application (NuGet sur les moteurs X, programme d'installation sur `VideoCaptureCore`).
- Référence au SDK approprié (Video Capture SDK, Video Edit SDK ou Media Player SDK — X ou classique).
- Pour l'exemple sur le moteur X : une référence transitive à SkiaSharp (déjà tirée par le SDK) ou votre propre bibliothèque de rendu de texte.

## Conclusion

L'événement `OnVideoFrameBuffer` donne un accès direct à chaque image brute, à la fois sur le moteur classique `VideoCaptureCore` (RGB24/RGB32 via `VideoFrameBufferEventArgs` + `FastImageProcessing`) et sur les moteurs X multiplateformes (`VideoCaptureCoreX` / `MediaPlayerCoreX`, BGRA via `VideoFrameXBufferEventArgs` + SkiaSharp). C'est le bon outil lorsque vous avez besoin d'un rendu de texte au niveau pixel — polices personnalisées, contenu dynamique par image, anticrénelage que vous contrôlez, ou intégration avec des bibliothèques de texte/graphiques tierces.

Pour les superpositions de texte statiques ou pilotées par horloge sans écrire un gestionnaire par image, l'[effet de superposition de texte](../video-effects/text-overlay.md) en une ligne est généralement le meilleur choix.

## Documentation associée

- [Effet de superposition de texte](../video-effects/text-overlay.md) — superposition de texte déclarative, de haut niveau, sans écrire de rappel.
- [Dessin d'image via OnVideoFrameBuffer](image-onvideoframebuffer.md) — même technique appliquée aux images au lieu du texte.
- [Dessiner la vidéo dans un PictureBox](draw-video-picturebox.md) — modèle de rendu WinForms qui s'associe souvent au travail au niveau pixel.

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.
