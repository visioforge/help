---
title: Capteur d'échantillons vidéo — images brutes en C# .NET
description: Extrayez les images vidéo brutes depuis les Video Capture SDK, Media Player et Video Edit avec accès mémoire managée et conversion bitmap en C#.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
  - C#
  - NuGet
primary_api_classes:
  - VideoSampleGrabberBlock
  - VideoFrameXBufferEventArgs
  - VideoFrameBitmapEventArgs
  - VideoFrameSKBitmapEventArgs
  - VideoFrameBufferEventArgs

---

# Utilisation du capteur d'échantillons vidéo

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Obtention des images vidéo brutes sous forme de pointeur de mémoire non managée à l'intérieur de la structure

=== "Moteurs X"

    
    ```csharp
    // S'abonner à l'événement de tampon d'image vidéo
    VideoCapture1.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
    {
        // Traiter l'objet VideoFrameX
        ProcessFrame(e.Frame);
    }
    
    // Exemple de traitement d'une image VideoFrameX — ajustement de la luminosité
    private void ProcessFrame(VideoFrameX frame)
    {
        // Traiter uniquement les formats RGB/BGR/RGBA/BGRA
        if (frame.Format != VideoFormatX.RGB && 
            frame.Format != VideoFormatX.BGR && 
            frame.Format != VideoFormatX.RGBA && 
            frame.Format != VideoFormatX.BGRA)
        {
            return;
        }
        
        // Obtenir les données sous forme de tableau d'octets pour manipulation
        byte[] data = frame.ToArray();
        
        // Déterminer la taille du pixel selon le format
        int pixelSize = (frame.Format == VideoFormatX.RGB || frame.Format == VideoFormatX.BGR) ? 3 : 4;
        
        // Facteur de luminosité (1.2 = 20 % plus lumineux, 0.8 = 20 % plus sombre)
        float brightnessFactor = 1.2f;

        // Précalculer une table de correspondance pour que la boucle interne soit une consultation de table, pas une multiplication
        var lut = new byte[256];
        for (int v = 0; v < 256; v++)
        {
            lut[v] = (byte)Math.Min(255, (int)(v * brightnessFactor));
        }

        // Les lignes sont complétées sur un stride de 4 octets : parcourir ligne par ligne, jamais data.Length
        int stride = frame.Stride;
        for (int y = 0; y < frame.Height; y++)
        {
            int row = y * stride;
            for (int x = 0; x < frame.Width; x++)
            {
                int pixel = row + x * pixelSize;
                data[pixel] = lut[data[pixel]];
                data[pixel + 1] = lut[data[pixel + 1]];
                data[pixel + 2] = lut[data[pixel + 2]];
            }
        }
        
        // Copier les données modifiées dans l'image
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    

=== "Moteurs classiques"

    
    ```csharp
    // S'abonner à l'événement de tampon d'image vidéo
    VideoCapture1.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
    {
        // Traiter la structure VideoFrame
        ProcessFrame(e.Frame);
        
        // Si vous avez modifié l'image et souhaitez mettre à jour le flux vidéo
        e.UpdateData = true;
    }
    
    // Exemple de traitement d'une image VideoFrame — ajustement de la luminosité
    private void ProcessFrame(VideoFrame frame)
    {
        // Traiter uniquement le format RGB pour cet exemple
        if (frame.Info.Colorspace != RAWVideoColorSpace.RGB24)
        {
            return;
        }
        
        // Obtenir les données sous forme de tableau d'octets pour manipulation
        byte[] data = frame.ToArray();
        
        // Facteur de luminosité (1.2 = 20 % plus lumineux, 0.8 = 20 % plus sombre)
        float brightnessFactor = 1.2f;

        // Précalculer une table de correspondance pour que la boucle interne soit une consultation de table, pas une multiplication
        var lut = new byte[256];
        for (int v = 0; v < 256; v++)
        {
            lut[v] = (byte)Math.Min(255, (int)(v * brightnessFactor));
        }

        // Les lignes RGB24 sont complétées sur un stride de 4 octets : parcourir ligne par ligne, jamais data.Length
        int stride = frame.Info.Stride;
        int rowBytes = frame.Info.Width * 3;
        for (int y = 0; y < frame.Info.Height; y++)
        {
            int row = y * stride;
            for (int i = 0; i < rowBytes; i++)
            {
                data[row + i] = lut[data[row + i]];
            }
        }
        
        // Copier les données modifiées dans l'image
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    

=== "Media Blocks SDK"

    
    ```csharp
    // Créer et configurer le bloc de capteur d'échantillons vidéo
    var videoSampleGrabberBlock = new VideoSampleGrabberBlock(VideoFormatX.RGB);
    // Requis lorsque le gestionnaire écrit dans l'image
    videoSampleGrabberBlock.AllowFrameModification = true;
    videoSampleGrabberBlock.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
    {
        // Traiter l'objet VideoFrameX
        ProcessFrame(e.Frame);
    }
    
    // Exemple de traitement d'une image VideoFrameX — ajustement de la luminosité
    private void ProcessFrame(VideoFrameX frame)
    {
        if (frame.Format != VideoFormatX.RGB)
        {
            return;
        }
        
        // Obtenir les données sous forme de tableau d'octets pour manipulation
        byte[] data = frame.ToArray();
        
        // Facteur de luminosité (1.2 = 20 % plus lumineux, 0.8 = 20 % plus sombre)
        float brightnessFactor = 1.2f;

        // Précalculer une table de correspondance pour que la boucle interne soit une consultation de table, pas une multiplication
        var lut = new byte[256];
        for (int v = 0; v < 256; v++)
        {
            lut[v] = (byte)Math.Min(255, (int)(v * brightnessFactor));
        }

        // Les lignes RGB sont complétées sur un stride de 4 octets : parcourir ligne par ligne, jamais data.Length
        int stride = frame.Stride;
        int rowBytes = frame.Width * 3;
        for (int y = 0; y < frame.Height; y++)
        {
            int row = y * stride;
            for (int i = 0; i < rowBytes; i++)
            {
                data[row + i] = lut[data[row + i]];
            }
        }
        
        // Copier les données modifiées dans l'image
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    


!!! note "Préférez l'effet de luminosité intégré pour une simple luminosité"

    La luminosité est un effet vidéo intégré : appelez `VideoEffects_SetBrightness` (valeurs de -1.0 à 1.0) sur les moteurs classiques, ou ajoutez un `VideoBalanceBlock` avec `VideoBalanceVideoEffect.Brightness` défini dans un pipeline Media Blocks. Les deux s'exécutent nativement et sont plus rapides que le traitement par image en code managé. Utilisez le sample grabber pour le traitement personnalisé par image que les effets intégrés ne couvrent pas.

## Travailler avec des images bitmap

Si vous avez besoin de travailler avec des objets Bitmap managés plutôt qu'avec des pointeurs vers de la mémoire brute, vous pouvez utiliser l'événement `OnVideoFrameBitmap` des classes `core` ou du SampleGrabberBlock. Les moteurs X et le SampleGrabberBlock transmettent une copie de l'image, donc les modifications de ce bitmap n'atteignent le flux vidéo qu'avec les moteurs classiques, et seulement si vous définissez `e.UpdateData = true` :

```csharp
// S'abonner à l'événement d'image bitmap
VideoCapture1.OnVideoFrameBitmap += OnVideoFrameBitmap;

private void OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Traiter l'objet Bitmap
    ProcessBitmap(e.Frame);
    
    // Si vous avez modifié le bitmap et souhaitez mettre à jour le flux vidéo
    e.UpdateData = true;
}

// Copie réutilisable de l'image
private Bitmap _source;

// Exemple de traitement d'un Bitmap — ajustement de la luminosité
private void ProcessBitmap(Bitmap bitmap)
{
    // Utilisez les méthodes Bitmap ou Graphics pour manipuler l'image
    // Cet exemple utilise ColorMatrix pour l'ajustement de la luminosité
    
    // GDI+ ne peut pas dessiner un bitmap sur son propre Graphics, on copie donc d'abord l'image dans un tampon réutilisable
    if (_source == null || _source.Width != bitmap.Width || _source.Height != bitmap.Height)
    {
        _source?.Dispose();
        _source = new Bitmap(bitmap.Width, bitmap.Height);
    }

    using (Graphics copy = Graphics.FromImage(_source))
    {
        copy.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
    }

    // Créer un objet graphics à partir du bitmap
    using (Graphics g = Graphics.FromImage(bitmap))
    {
        // Créer une matrice de couleurs pour l'ajustement de la luminosité
        float brightnessFactor = 1.2f; // 1.0 = aucun changement, >1.0 = plus lumineux, <1.0 = plus sombre
        ColorMatrix colorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {brightnessFactor, 0, 0, 0, 0},
            new float[] {0, brightnessFactor, 0, 0, 0},
            new float[] {0, 0, brightnessFactor, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });
        
        // Créer un objet ImageAttributes et définir la matrice de couleurs
        using (ImageAttributes attributes = new ImageAttributes())
        {
            attributes.SetColorMatrix(colorMatrix);
            
            // Dessiner l'image avec l'ajustement de luminosité
            g.DrawImage(_source, 
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                0, 0, bitmap.Width, bitmap.Height,
                GraphicsUnit.Pixel, attributes);
        }
    }
}
```

## Travailler avec SkiaSharp pour des applications multiplateformes

Pour les applications multiplateformes, le VideoSampleGrabberBlock offre la possibilité de travailler avec SkiaSharp, une API graphique 2D haute performance pour .NET. Ceci est particulièrement utile pour les applications ciblant plusieurs plateformes, notamment mobile et web.

### Utilisation de l'événement OnVideoFrameSKBitmap

```csharp
// D'abord, ajoutez le paquet NuGet SkiaSharp à votre projet
// Install-Package SkiaSharp

// Importer les espaces de noms nécessaires
using SkiaSharp;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.Events;

// Créer un VideoSampleGrabberBlock avec le format RGBA ou BGRA
// Note : l'événement OnVideoFrameSKBitmap ne fonctionne qu'avec les formats RGBA ou BGRA
var videoSampleGrabberBlock = new VideoSampleGrabberBlock(VideoFormatX.BGRA);

// Activer la propriété SaveLastFrame si vous souhaitez prendre des instantanés plus tard
videoSampleGrabberBlock.SaveLastFrame = true;

// S'abonner à l'événement bitmap SkiaSharp
videoSampleGrabberBlock.OnVideoFrameSKBitmap += OnVideoFrameSKBitmap;

// Gestionnaire d'événements pour les images bitmap SkiaSharp
private void OnVideoFrameSKBitmap(object sender, VideoFrameSKBitmapEventArgs e)
{
    // Traiter le SKBitmap
    ProcessSKBitmap(e.Frame);
    
    // Note : contrairement à VideoFrameBitmapEventArgs, VideoFrameSKBitmapEventArgs n'a pas
    // de propriété UpdateData car il est conçu uniquement pour la visualisation/analyse d'image
}

// Exemple de traitement d'un SKBitmap — ajustement de la luminosité
private void ProcessSKBitmap(SKBitmap bitmap)
{
    // Créer un nouveau bitmap pour contenir l'image traitée
    using (var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height)))
    {
        var canvas = surface.Canvas;
        
        // Configurer un paint avec un filtre de couleur pour l'ajustement de la luminosité
        using (var paint = new SKPaint())
        {
            // Créer un filtre de luminosité (1.2 = 20 % plus lumineux)
            float brightnessFactor = 1.2f;
            var colorMatrix = new float[]
            {
                brightnessFactor, 0, 0, 0, 0,
                0, brightnessFactor, 0, 0, 0,
                0, 0, brightnessFactor, 0, 0,
                0, 0, 0, 1, 0
            };
            
            paint.ColorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
            
            // Dessiner le bitmap d'origine avec le filtre de luminosité appliqué
            canvas.DrawBitmap(bitmap, 0, 0, paint);
            
            // Si vous avez besoin d'obtenir le résultat comme nouveau SKBitmap :
            var processedImage = surface.Snapshot();
            using (var processedBitmap = SKBitmap.FromImage(processedImage))
            {
                // Utilisez processedBitmap pour d'autres opérations ou l'affichage
                // Par exemple, l'afficher dans une vue SkiaSharp
                // mySkiaView.SKBitmap = processedBitmap.Copy();
            }
        }
    }
}
```

### Prise d'instantanés avec SkiaSharp

```csharp
// Créer une méthode pour capturer et enregistrer un instantané
private void CaptureSnapshot(string filePath)
{
    // Assurez-vous que SaveLastFrame a été activé sur le VideoSampleGrabberBlock
    if (videoSampleGrabberBlock.SaveLastFrame)
    {
        // Obtenir la dernière image en tant que SKBitmap
        using (var bitmap = videoSampleGrabberBlock.GetLastFrameAsSKBitmap())
        {
            if (bitmap != null)
            {
                // Enregistrer le bitmap dans un fichier
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}
```

### Avantages de l'utilisation de SkiaSharp

1. **Compatibilité multiplateforme** : fonctionne sur Windows, macOS, Linux, iOS, Android et WebAssembly
2. **Performances** : fournit un traitement graphique haute performance
3. **API moderne** : offre un ensemble complet de fonctions de dessin, de filtrage et de transformation
4. **Efficacité mémoire** : gestion de la mémoire plus efficace que System.Drawing
5. **Aucune dépendance plateforme** : aucune dépendance vis-à-vis de bibliothèques d'imagerie spécifiques à la plateforme

## Informations sur le traitement des images

Vous pouvez obtenir des images vidéo depuis des sources en direct ou des fichiers à l'aide des événements `OnVideoFrameBuffer` et `OnVideoFrameBitmap`.

L'événement `OnVideoFrameBuffer` est plus rapide et fournit le pointeur de mémoire non managée vers l'image décodée. L'événement `OnVideoFrameBitmap` est plus lent, mais vous obtenez l'image décodée sous forme d'objet de la classe `Bitmap`.

### Comprendre les objets d'image

- **VideoFrameX** (moteurs X) : contient les données d'image, dimensions, format, horodatage et méthodes pour manipuler les données vidéo brutes
- **VideoFrame** (moteurs classiques) : structure similaire mais avec un agencement mémoire différent
- **Propriétés communes** :
  - Width/Height : dimensions de l'image
  - Format/Colorspace : format de pixel (RGB, BGR, RGBA, etc.)
  - Stride : nombre d'octets par ligne d'analyse
  - Timestamp : position de l'image dans la chronologie vidéo
  - Data : pointeur vers la mémoire non managée contenant les données de pixels

### Considérations importantes

1. Le format de pixel de l'image affecte la manière de traiter les données :
   - RGB/BGR : 3 octets par pixel
   - RGBA/BGRA/ARGB : 4 octets par pixel (avec canal alpha)
   - Formats YUV : arrangements de composantes différents

2. Avec les moteurs classiques, définissez `e.UpdateData = true` si vous avez modifié les données de l'image et souhaitez que les modifications soient visibles dans le flux vidéo. Media Blocks SDK et les moteurs X ne lisent pas `UpdateData`.

3. Définissez `AllowFrameModification = true` sur le `VideoSampleGrabberBlock` (Media Blocks SDK) avant le démarrage du pipeline, ou sur le `CustomVideoSampleGrabberEffect` (Media Blocks SDK et moteurs X) avant d'ajouter l'effet. Sans cela, l'image est mappée en lecture seule et vos modifications peuvent atteindre une autre branche d'un `TeeBlock` qui partage la même image.

4. Pour un traitement nécessitant plusieurs images ou des opérations complexes, envisagez d'utiliser un tampon ou une file d'attente pour stocker les images.

5. Lorsque vous utilisez `OnVideoFrameSKBitmap`, sélectionnez RGBA ou BGRA comme format d'image lors de la création du VideoSampleGrabberBlock.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.