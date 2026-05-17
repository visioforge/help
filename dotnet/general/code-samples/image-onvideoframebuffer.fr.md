---
title: Traiter les images vidéo en C# .NET — OnVideoFrameBuffer
description: Accédez au tampon brut d'image vidéo en C# / .NET via OnVideoFrameBuffer. Modifiez pixels, dessinez images et appliquez des mélanges.
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
  - Encoding
  - C#
primary_api_classes:
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - VideoCaptureCore
  - VideoFrameBufferEventArgs

---

# Dessiner des images avec OnVideoFrameBuffer en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

L'événement `OnVideoFrameBuffer` donne un accès direct, au niveau du pixel, à chaque image vidéo qui passe dans le pipeline. Les gestionnaires d'événements reçoivent un tampon brut et peuvent inspecter, modifier ou écraser les pixels avant que l'image ne passe à l'étape suivante (aperçu, encodeur, sortie fichier). Dessiner une image sur le frame — pour un tatouage numérique, un logo, une superposition de débogage ou une annotation de vision par ordinateur — est le cas d'usage le plus courant, et celui que ce guide détaille.

!!! tip "Vous cherchez la fonctionnalité de superposition de haut niveau ?"
    Si vous avez seulement besoin de placer une image statique ou animée sur la vidéo (PNG / JPG / GIF / BMP), utilisez l'[effet de superposition d'image](../video-effects/image-overlay.md) dédié — une ligne de code via `Video_Effects_Add(new VideoEffectImageLogo(...))`. Utilisez `OnVideoFrameBuffer` (cette page) lorsque vous avez besoin d'un **contrôle au niveau pixel** : modes de mélange personnalisés, logique par image, annotations de CV, ou intégration avec des bibliothèques d'imagerie tierces.

### Moteurs pris en charge

L'événement `OnVideoFrameBuffer` est exposé sur les deux familles de moteurs :

| Moteur | Type d'arguments d'événement | Format de pixel |
|---|---|---|
| `VideoCaptureCore` (DirectShow, Windows) | `VideoFrameBufferEventArgs` | RGB24 / RGB32 |
| `VideoCaptureCoreX` (GStreamer, multiplateforme) | `VideoFrameXBufferEventArgs` | BGRA (le plus courant) |
| `MediaPlayerCoreX` (GStreamer, multiplateforme) | `VideoFrameXBufferEventArgs` | BGRA (le plus courant) |

Les deux moteurs suivent le même modèle — s'abonner à l'événement, lire `e.Frame.Data` (un `IntPtr`) avec `Width` / `Height` / `Stride`, modifier éventuellement le tampon sur place, et définir `e.UpdateData = true` pour propager les changements en aval.

## Comprendre le processus

Lorsque vous travaillez avec des images vidéo, vous devez :

1. Charger votre image (logo, tatouage, etc.) en mémoire.
2. Convertir l'image en un format de tampon compatible (RGB24/RGB32 pour le moteur classique, BGRA pour les moteurs X).
3. Vous abonner à l'événement `OnVideoFrameBuffer`.
4. Dessiner l'image sur chaque image vidéo au fur et à mesure de son traitement.
5. Définir `e.UpdateData = true` pour que l'image modifiée remplace l'original en aval.

## Exemple VideoCaptureCore (DirectShow)

Détaillons l'implémentation étape par étape :

### Étape 1 : charger votre image

Tout d'abord, chargez le fichier image que vous souhaitez superposer à la vidéo :

```cs
// Chargement du Bitmap depuis un fichier
private Bitmap logoImage = new Bitmap(@"logo24.jpg");
// Vous pouvez aussi utiliser un PNG avec canal alpha pour la transparence
//private Bitmap logoImage = new Bitmap(@"logo32.png");
```

### Étape 2 : préparer les tampons mémoire

Initialisez les pointeurs pour le tampon d'image :

```cs
// Tampon RGB24/RGB32 du logo
private IntPtr logoImageBuffer = IntPtr.Zero;
private int logoImageBufferSize = 0;
```

### Étape 3 : implémenter le gestionnaire de l'événement OnVideoFrameBuffer

L'implémentation complète du gestionnaire d'événements :

```cs
private void VideoCapture1_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
{
    // Créer le tampon du logo s'il n'est pas alloué ou de taille zéro
    if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
    {
        if (logoImageBuffer == IntPtr.Zero)
        {
            if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
            else
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB24(logoImage.Width) * logoImage.Height;
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
        }
        else
        {
            if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;

                Marshal.FreeCoTaskMem(logoImageBuffer);
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
            else
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB24(logoImage.Width) * logoImage.Height;

                Marshal.FreeCoTaskMem(logoImageBuffer);
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
        }

        if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
        {
            BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format32bppArgb);
        }
        else
        {
            BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format24bppRgb);
        }
    }

    // Dessiner l'image — le struct VideoFrame classique conserve Width/Height/Stride dans Frame.Info
    if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
    {
        FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
            e.Frame.Info.Height, 0, 0);
    }
    else
    {
        FastImageProcessing.Draw_RGB24OnRGB24Old(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
            e.Frame.Info.Height, 0, 0);
    }

    e.UpdateData = true;
}
```

## Explication détaillée

### Gestion de la mémoire

Le code gère à la fois les formats d'image 24 bits et 32 bits. Voici ce qui se passe :

1. **Vérification de l'initialisation du tampon** : le code vérifie d'abord si le tampon du logo doit être créé ou recréé.

2. **Détection du format** : il détermine s'il faut utiliser le format RGB24 ou RGB32 en fonction de l'image chargée :
   - RGB24 : couleur standard 24 bits (8 bits chacun pour R, G, B)
   - RGB32 : couleur 32 bits avec canal alpha pour la transparence (8 bits chacun pour R, G, B, A)

3. **Allocation de mémoire** : alloue de la mémoire non managée via `Marshal.AllocCoTaskMem()` pour stocker les données d'image.

4. **Conversion d'image** : convertit le Bitmap en données de pixels brutes dans le tampon alloué via `BitmapHelper.BitmapToIntPtr()`.

### Processus de dessin

Une fois le tampon préparé, le dessin a lieu :

1. **Dessin spécifique au format** : le code sélectionne la méthode de dessin appropriée en fonction du format d'image :
   - `FastImageProcessing.Draw_RGB32OnRGB24()` pour les images 32 bits avec transparence
   - `FastImageProcessing.Draw_RGB24OnRGB24Old()` pour les images standard 24 bits (forme à 8 arguments) ou `Draw_RGB24OnRGB24S()` lorsque les strides source/destination sont connus

2. **Paramètres de position** : les paramètres `0, 0` spécifient où dessiner l'image (coin supérieur gauche dans cet exemple).

3. **Mise à jour de l'image** : définir `e.UpdateData = true` garantit que les données de l'image modifiée sont utilisées pour l'affichage ou le traitement ultérieur.

## Exemple VideoCaptureCoreX / MediaPlayerCoreX (moteurs X)

Sur les moteurs X multiplateformes, la signature de l'événement passe à `VideoFrameXBufferEventArgs` et le tampon d'image arrive typiquement au format **BGRA** (4 octets par pixel). Le même modèle s'applique — s'abonner, inspecter, modifier, marquer les mises à jour. L'exemple ci-dessous utilise SkiaSharp pour envelopper le tampon brut et dessiner un logo PNG par-dessus ; SkiaSharp est déjà une dépendance transitive des moteurs X, donc aucun paquet NuGet supplémentaire n'est nécessaire.

```cs
using SkiaSharp;

// Charger le logo une seule fois (un PNG avec alpha fonctionne bien pour les tatouages numériques)
private SKBitmap _logo = SKBitmap.Decode(@"logo.png");

// S'abonner après avoir construit VideoCaptureCoreX / MediaPlayerCoreX
_videoCapture.OnVideoFrameBuffer += VideoCapture_OnVideoFrameBuffer;

private void VideoCapture_OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
{
    if (e.Frame == null || e.Frame.Data == IntPtr.Zero)
    {
        return;
    }

    // Envelopper le tampon BGRA brut dans un canevas SkiaSharp (sans allocation supplémentaire)
    var info = new SKImageInfo(e.Frame.Width, e.Frame.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

    using (var pixmap = new SKPixmap(info, e.Frame.Data, e.Frame.Stride))
    using (var surface = SKSurface.Create(pixmap))
    {
        var canvas = surface.Canvas;

        // Dessiner le logo en bas à droite avec 16 px de marge
        var x = e.Frame.Width - _logo.Width - 16;
        var y = e.Frame.Height - _logo.Height - 16;
        canvas.DrawBitmap(_logo, x, y);
        canvas.Flush();
    }

    // Propager l'image modifiée en aval
    e.UpdateData = true;
}
```

**Pourquoi BGRA importe.** Les moteurs X demandent BGRA par défaut pour les rappels d'image, car il correspond 1:1 à SkiaSharp, System.Drawing et la plupart des chemins d'interopérabilité conviviaux pour le GPU. Si vous avez besoin d'un autre format (I420, NV12, RGB24), demandez un bloc de conversion de format en amont de votre gestionnaire plutôt que de convertir à chaque image.

**Piles d'imagerie alternatives.** Vous pouvez aussi utiliser `System.Drawing.Bitmap` via `new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, data)` sur Windows, ou des écritures manuelles d'octets via `Marshal.Copy` / `Span<byte>` pour un contrôle maximal. SkiaSharp est l'option recommandée sur macOS / Linux / iOS / Android.

**Parité au niveau du moteur.** Tout ce qui est documenté dans les sections [Gestion de la mémoire](#gestion-de-la-memoire), [Gestion des erreurs](#gestion-des-erreurs) et [Optimisation des performances](#optimisation-des-performances) ci-dessous s'applique également aux moteurs X — l'événement est déclenché sur un thread de traitement, `UpdateData` détermine si le tampon est réutilisé en aval, et le travail lourd doit être délégué pour éviter de perdre des images.

## Bonnes pratiques pour la superposition d'image

Pour des performances optimales lors de la superposition d'images sur les images vidéo :

1. **Gestion de la mémoire** : libérez toujours la mémoire allouée lorsqu'elle n'est plus nécessaire pour éviter les fuites de mémoire.

2. **Réutilisation du tampon** : créez le tampon une fois et réutilisez-le pour les images suivantes plutôt que de le recréer pour chaque image.

3. **Considérations sur la taille des images** : utilisez des images de taille appropriée ; superposer de grandes images peut affecter les performances.

4. **Choix du format** :
   - Utilisez PNG (RGB32) lorsque vous avez besoin de transparence
   - Utilisez JPG (RGB24) lorsque la transparence n'est pas requise (plus efficace)

5. **Calcul de la position** : pour un positionnement dynamique, calculez les coordonnées en fonction des dimensions de l'image. Sur le moteur classique (`VideoFrameBufferEventArgs`), Width/Height se trouvent sur `e.Frame.Info` ; sur les moteurs X (`VideoFrameXBufferEventArgs`), ils sont directement sur `e.Frame`.

   ```cs
   // Moteur classique — Width/Height se trouvent sur e.Frame.Info
   int xPos = e.Frame.Info.Width - logoImage.Width - 10;
   int yPos = e.Frame.Info.Height - logoImage.Height - 10;
   ```

## Gestion des erreurs

Lors de l'implémentation de cette fonctionnalité, envisagez d'ajouter une gestion des erreurs :

```cs
try 
{
    // Votre implémentation existante
}
catch (OutOfMemoryException ex)
{
    // Gérer les échecs d'allocation de mémoire
    Console.WriteLine("Failed to allocate memory: " + ex.Message);
}
catch (Exception ex)
{
    // Gérer les autres exceptions
    Console.WriteLine("Error during frame processing: " + ex.Message);
}
finally 
{
    // Code de nettoyage optionnel
}
```

## Optimisation des performances

Pour les applications hautes performances, envisagez ces optimisations :

1. **Pré-allocation du tampon** : initialisez les tampons au démarrage de l'application plutôt que pendant le traitement vidéo.

2. **Traitement conditionnel** : ne traitez que les images qui ont besoin de la superposition (par exemple, sautez le traitement pour certaines images).

3. **Traitement parallèle** : pour les opérations complexes, envisagez d'utiliser des techniques de traitement parallèle.

## Conclusion

L'événement `OnVideoFrameBuffer` donne un accès direct à chaque image brute, à la fois sur le moteur classique `VideoCaptureCore` (RGB24/RGB32 via `VideoFrameBufferEventArgs`) et sur les moteurs X multiplateformes (`VideoCaptureCoreX` / `MediaPlayerCoreX`, BGRA via `VideoFrameXBufferEventArgs`). C'est le bon outil lorsque vous avez besoin d'un contrôle au niveau pixel — modes de mélange personnalisés, annotations CV par image, ou intégration avec des bibliothèques d'imagerie tierces.

Pour les superpositions d'images statiques ou animées sans écrire un gestionnaire par image, l'[effet de superposition d'image](../video-effects/image-overlay.md) en une ligne est généralement le meilleur choix.

## Documentation associée

- [Effet de superposition d'image](../video-effects/image-overlay.md) — superposition de tatouage / logo déclarative, de haut niveau, sans écrire de rappel.
- [Superposition de texte via OnVideoFrameBuffer](text-onvideoframebuffer.md) — même technique appliquée au texte au lieu des images.
- [Dessiner la vidéo dans un PictureBox](draw-video-picturebox.md) — modèle de rendu WinForms qui s'associe souvent au travail au niveau pixel.

---

Vous cherchez plus d'exemples de code ? Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples et ressources supplémentaires.
