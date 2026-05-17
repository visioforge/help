---
title: Effets vidéo personnalisés temps réel en C# .NET VisioForge
description: Effets vidéo personnalisés en C# avec OnVideoFrameBitmap et OnVideoFrameBuffer pour traitement temps réel et superpositions.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoFrameBitmapEventArgs
  - VideoFrameBufferEventArgs

---

# Créer des effets vidéo personnalisés en temps réel dans des applications C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au traitement des images vidéo

Lors du développement d'applications vidéo, vous devez souvent appliquer des effets personnalisés ou des superpositions aux flux vidéo en temps réel. Le SDK .NET fournit deux événements puissants à cette fin : `OnVideoFrameBitmap` et `OnVideoFrameBuffer`. Ces événements vous donnent un accès direct à chaque image vidéo, vous permettant de modifier les pixels avant qu'ils ne soient rendus ou encodés.

## Méthodes d'implémentation

Il existe deux approches principales pour implémenter des effets vidéo personnalisés :

1. **Avec OnVideoFrameBitmap** : traite les images comme des objets Bitmap avec GDI+ — plus simple à utiliser mais avec des performances modérées
2. **Avec OnVideoFrameBuffer** : manipule directement le tampon d'image RGB24 brut — offre de meilleures performances mais nécessite plus de code bas niveau

## Exemples de code pour effets vidéo personnalisés

### Implémentation de superposition de texte

L'ajout de superpositions de texte à la vidéo est utile pour le tatouage numérique, l'affichage d'informations ou la création de sous-titres. Cet exemple montre comment ajouter un texte simple à vos images vidéo :

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);

    grf.DrawString("Hello!", new Font(FontFamily.GenericSansSerif, 20), new SolidBrush(Color.White), 20, 20);
    grf.Dispose();

    e.UpdateData = true;
}
```

### Implémentation d'un effet niveaux de gris

Convertir la vidéo en niveaux de gris est une technique fondamentale de traitement d'image. Cet exemple montre comment accéder aux valeurs individuelles de pixels et les modifier :

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Bitmap bmp = e.Frame;
    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
    byte[] rgbValues = new byte[bytes];
    
    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Appliquer la formule standard de luminance (0.3R + 0.59G + 0.11B) pour une conversion précise en niveaux de gris
    for (int i = 0; i < rgbValues.Length; i += 3)
    {
        int gray = (int)(rgbValues[i] * 0.3 + rgbValues[i + 1] * 0.59 + rgbValues[i + 2] * 0.11);
        rgbValues[i] = (byte)gray;
        rgbValues[i + 1] = (byte)gray;
        rgbValues[i + 2] = (byte)gray;
    }
    
    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
    bmp.UnlockBits(bmpData);
    
    e.UpdateData = true;
}
```

### Implémentation de l'ajustement de luminosité

Cet exemple montre comment ajuster la luminosité des images vidéo — une exigence courante dans les applications de traitement vidéo :

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    float brightness = 1.2f; // Les valeurs > 1 augmentent la luminosité, < 1 la diminuent
    
    Bitmap bmp = e.Frame;
    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
    byte[] rgbValues = new byte[bytes];
    
    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Appliquer l'ajustement de luminosité à chaque canal de couleur
    for (int i = 0; i < rgbValues.Length; i++)
    {
        int newValue = (int)(rgbValues[i] * brightness);
        rgbValues[i] = (byte)Math.Min(255, Math.Max(0, newValue));
    }
    
    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
    bmp.UnlockBits(bmpData);
    
    e.UpdateData = true;
}
```

### Implémentation de superposition d'horodatage

Ajouter des horodatages aux images vidéo est essentiel pour les applications de surveillance et de journalisation. Cet exemple montre comment créer un horodatage d'aspect professionnel avec un arrière-plan semi-transparent :

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);
    
    // Créer un arrière-plan semi-transparent pour une meilleure lisibilité
    Rectangle textBackground = new Rectangle(10, e.Frame.Height - 50, 250, 40);
    grf.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), textBackground);
    
    // Afficher la date et l'heure actuelles
    string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    grf.DrawString(dateTime, new Font(FontFamily.GenericSansSerif, 16), 
                  new SolidBrush(Color.White), 15, e.Frame.Height - 45);
    
    grf.Dispose();
    
    e.UpdateData = true;
}
```

## Conseils d'optimisation des performances

### Travailler avec les données brutes du tampon

Pour les applications hautes performances, le traitement des données brutes du tampon offre des avantages significatifs de vitesse :

```cs
// Exemple de l'événement OnVideoFrameBuffer (pseudo-code)
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Frame.Data est un IntPtr vers le tampon natif (longueur = e.Frame.DataSize).
    // Les dimensions et le stride se trouvent dans e.Frame.Info ; la disposition réelle
    // des pixels est dans e.Frame.Info.Colorspace (RGB24/RGB32/YUY2/NV12/...). Ne pas
    // supposer RGB24.
    int width   = e.Frame.Info.Width;
    int height  = e.Frame.Info.Height;
    int stride  = e.Frame.Info.Stride;
    var colorspace = e.Frame.Info.Colorspace;

    // Traiter e.Frame.Data sur place pour des performances maximales, puis définir
    // e.UpdateData = true afin que le tampon modifié soit propagé en aval.
}
```

### Bonnes pratiques pour le traitement des images

* **Gestion de la mémoire** : libérez toujours les objets Graphics et déverrouillez les données bitmap
* **Considérations de performance** : pour le traitement en temps réel, gardez les opérations légères
* **Traitement du tampon** : nous recommandons fortement de traiter les données BRUTES dans l'événement OnVideoFrameBuffer pour des performances optimales
* **Bibliothèques externes** : envisagez d'utiliser Intel IPP ou d'autres bibliothèques optimisées de traitement d'image pour les opérations complexes

---
## Ressources supplémentaires
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour accéder à plus d'exemples de code et à des exemples de projets complets.
