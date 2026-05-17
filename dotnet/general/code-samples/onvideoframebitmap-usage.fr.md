---
title: OnVideoFrameBitmap — accès temps réel aux images en .NET
description: Accédez aux images vidéo et modifiez-les en temps réel avec les événements OnVideoFrameBitmap pour la manipulation vidéo avancée dans les applications C#.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoFrameBitmapEventArgs

---

# Guide d'utilisation d'OnVideoFrameBitmap en temps réel

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

L'événement `OnVideoFrameBitmap` est une fonctionnalité puissante des bibliothèques de traitement vidéo .NET qui permet aux développeurs d'accéder aux images vidéo et de les modifier en temps réel. Ce guide explore les applications pratiques, les techniques d'implémentation et les considérations de performance pour la manipulation d'images vidéo au format bitmap dans les applications C#.

## Comprendre les événements OnVideoFrameBitmap

L'événement `OnVideoFrameBitmap` fournit une interface directe pour accéder aux images vidéo au fur et à mesure de leur traitement par le SDK. Cette capacité est essentielle pour les applications qui nécessitent :

- Une analyse vidéo en temps réel
- Une manipulation image par image
- L'implémentation de superpositions dynamiques
- Des effets vidéo personnalisés
- L'intégration de la vision par ordinateur

Lorsque l'événement se déclenche, il fournit une représentation bitmap de l'image vidéo courante, permettant un accès et une manipulation au niveau du pixel avant que l'image ne continue dans le pipeline de traitement.

## Implémentation de base

`OnVideoFrameBitmap` existe sur les moteurs uniquement Windows : `VideoCaptureCore`, `MediaPlayerCore` et `VideoEditCore`. Dans les extraits ci-dessous, `VideoCapture1` est une instance de `VideoCaptureCore` — remplacez par `MediaPlayer1` ou `VideoEdit1` si vous utilisez un autre moteur.

Abonnez-vous avant de démarrer le pipeline pour ne pas manquer la première image :

```csharp
// Type d'événement : EventHandler<VideoFrameBitmapEventArgs>. Déclenché sur un thread worker.
VideoCapture1.OnVideoFrameBitmap += VideoCapture1_OnVideoFrameBitmap;

// Implémenter le gestionnaire d'événements
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // e.Frame — l'image courante sous forme de System.Drawing.Bitmap (possédé par le SDK ; ne pas libérer).
    // Définir e.UpdateData = true si vous modifiez le bitmap sur place.
}
```

## Manipuler les images vidéo

### Exemple simple de superposition Bitmap

L'exemple suivant montre comment superposer une image sur chaque image vidéo :

```csharp
Bitmap bmp = new Bitmap(@"c:\samples\pics\1.jpg");

using (Graphics g = Graphics.FromImage(e.Frame))
{
    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
    e.UpdateData = true;
}

bmp.Dispose();
```

Dans ce code :

1. Nous créons un objet `Bitmap` à partir d'un fichier image
2. Nous utilisons la classe `Graphics` pour dessiner sur le bitmap de l'image
3. Nous définissons `e.UpdateData = true` pour informer le SDK que nous avons modifié l'image
4. Nous libérons correctement nos ressources pour éviter les fuites de mémoire

> **Important :** définissez toujours `e.UpdateData = true` lorsque vous modifiez le bitmap de l'image. Cela signale au SDK d'utiliser votre image modifiée au lieu de l'original.

### Ajouter des superpositions de texte

Les superpositions de texte sont couramment utilisées pour les horodatages, les sous-titres ou les affichages informatifs :

```csharp
using (Graphics g = Graphics.FromImage(e.Frame))
{
    // Créer un arrière-plan semi-transparent pour le texte
    using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
    {
        g.FillRectangle(brush, 10, 10, 200, 30);
    }
    
    // Ajouter une superposition de texte
    using (Font font = new Font("Arial", 12))
    using (SolidBrush textBrush = new SolidBrush(Color.White))
    {
        g.DrawString(DateTime.Now.ToString(), font, textBrush, new PointF(15, 15));
    }
    
    e.UpdateData = true;
}
```

## Considérations de performance

Lorsque vous travaillez avec `OnVideoFrameBitmap`, il est crucial d'optimiser votre code pour la performance. Chaque opération de traitement d'image doit se terminer rapidement pour maintenir une lecture vidéo fluide.

### Gestion des ressources

La gestion correcte des ressources est essentielle :

```csharp
// Approche peu performante
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Bitmap overlay = new Bitmap(@"c:\logo.png");
    Graphics g = Graphics.FromImage(e.Frame);
    g.DrawImage(overlay, 0, 0);
    e.UpdateData = true;
    // Fuite de mémoire ! Graphics et Bitmap non libérés
}

// Approche optimisée
private Bitmap _cachedOverlay;

private void InitializeResources()
{
    _cachedOverlay = new Bitmap(@"c:\logo.png");
}

private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    using (Graphics g = Graphics.FromImage(e.Frame))
    {
        g.DrawImage(_cachedOverlay, 0, 0);
        e.UpdateData = true;
    }
}

private void CleanupResources()
{
    _cachedOverlay?.Dispose();
}
```

### Optimiser le temps de traitement

Pour maintenir une lecture vidéo fluide :

1. **Pré-calculer si possible** : préparez les ressources avant le début du traitement
2. **Mettre en cache les objets fréquemment utilisés** : évitez de créer de nouveaux objets pour chaque image
3. **Ne traiter qu'au besoin** : ajoutez une logique conditionnelle pour sauter des images ou effectuer des opérations moins intensives si nécessaire
4. **Utiliser des opérations de dessin efficaces** : choisissez les méthodes GDI+ appropriées selon vos besoins

```csharp
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Ne traiter qu'une image sur deux
    if (_frameCounter % 2 == 0)
    {
        using (Graphics g = Graphics.FromImage(e.Frame))
        {
            // Votre code de traitement d'image
            e.UpdateData = true;
        }
    }
    _frameCounter++;
}
```

## Techniques avancées de manipulation d'image

### Appliquer des filtres et des effets

Vous pouvez implémenter des filtres personnalisés de traitement d'image :

```csharp
private void ApplyGrayscaleFilter(Bitmap bitmap)
{
    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
    byte[] rgbValues = new byte[bytes];
    
    Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Traiter les données de pixels
    for (int i = 0; i < rgbValues.Length; i += 4)
    {
        byte gray = (byte)(0.299 * rgbValues[i + 2] + 0.587 * rgbValues[i + 1] + 0.114 * rgbValues[i]);
        rgbValues[i] = gray;     // Bleu
        rgbValues[i + 1] = gray; // Vert
        rgbValues[i + 2] = gray; // Rouge
    }
    
    Marshal.Copy(rgbValues, 0, ptr, bytes);
    bitmap.UnlockBits(bmpData);
}
```

## Intégration avec les bibliothèques de vision par ordinateur

L'événement `OnVideoFrameBitmap` peut être combiné avec des bibliothèques populaires de vision par ordinateur :

```csharp
// Exemple utilisant une bibliothèque hypothétique de vision par ordinateur
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Convertir le bitmap au format requis par la bibliothèque CV
    byte[] imageData = ConvertBitmapToByteArray(e.Frame);
    
    // Traiter avec la bibliothèque CV
    var results = _computerVisionProcessor.DetectFaces(imageData, e.Frame.Width, e.Frame.Height);
    
    // Dessiner les résultats sur l'image
    using (Graphics g = Graphics.FromImage(e.Frame))
    {
        foreach (var face in results)
        {
            g.DrawRectangle(new Pen(Color.Yellow, 2), face.X, face.Y, face.Width, face.Height);
        }
        
        e.UpdateData = true;
    }
}
```

## Dépannage des problèmes courants

### Fuites de mémoire

Si vous constatez une augmentation de la mémoire pendant un traitement vidéo prolongé :

1. Assurez-vous que tous les objets `Graphics` sont libérés
2. Libérez correctement tous les objets `Bitmap` temporaires
3. Évitez de capturer de gros objets dans les expressions lambda

### Dégradation des performances

Si le traitement d'image devient lent :

1. Profilez votre gestionnaire d'événements pour identifier les goulots d'étranglement
2. Envisagez de réduire la fréquence de traitement
3. Optimisez les opérations GDI+ ou envisagez DirectX pour les applications critiques en performance

## Intégration SDK

L'événement `OnVideoFrameBitmap` est disponible dans les SDK suivants :

## Dépendances requises

Pour utiliser la fonctionnalité décrite dans ce guide, vous aurez besoin de :

- Paquet de redistribution du SDK
- System.Drawing (inclus dans .NET Framework)
- Prise en charge de Windows GDI+

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code et projets démontrant ces techniques en action.
