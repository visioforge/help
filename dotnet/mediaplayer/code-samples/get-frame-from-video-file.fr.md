---
title: Extraire des images vidéo en C# .NET — Bitmap, Byte[]
description: Obtenez des images vidéo sous forme de Bitmap, SKBitmap ou tableaux d'octets avec VisioForge Media Player SDK .NET. Positionnez à tout horodatage.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCore
  - VideoEditCore
  - Windows
  - Playback
  - Editing
  - MP4
  - C#
primary_api_classes:
  - MediaInfoReaderX
  - VideoEditCore
  - MediaPlayerCore
  - ExtractFrameWithVideoEditCore
  - ExtractFrameWithMediaPlayerCore

video:
  id: u0rO3xSzO5w
  upload_date: 2026-11-24T06:45:00Z
  duration: PT58S
---

# Extraction d'images vidéo depuis des fichiers vidéo en .NET

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

L'extraction d'images vidéo est une exigence courante dans de nombreuses applications multimédias. Que vous construisiez un outil d'édition vidéo, créiez des miniatures ou effectuiez une analyse vidéo, extraire des images spécifiques de fichiers vidéo est une capacité essentielle. Ce guide explique les différentes approches pour capturer des images depuis des fichiers vidéo dans des applications .NET.

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Pourquoi extraire des images vidéo ?

Il existe de nombreux cas d'usage pour l'extraction d'images vidéo :

- Création d'images miniatures pour les galeries vidéo
- Extraction d'images-clés pour l'analyse vidéo
- Génération d'images d'aperçu à des horodatages spécifiques
- Construction d'outils d'édition vidéo avec une précision image par image
- Création de séquences en accéléré à partir de séquences vidéo
- Capture d'images fixes depuis des enregistrements vidéo

## Comprendre l'extraction d'images vidéo

Les fichiers vidéo contiennent des séquences d'images affichées à des intervalles spécifiques pour créer l'illusion du mouvement. Lors de l'extraction d'une image, vous capturez essentiellement une seule image à un horodatage spécifique dans la vidéo. Ce processus implique :

1. Ouvrir le fichier vidéo
2. Se positionner à l'horodatage spécifique
3. Décoder les données de l'image
4. La convertir en format d'image

## Méthodes d'extraction d'images en .NET

Il existe plusieurs approches pour extraire des images de fichiers vidéo en .NET, selon vos exigences et votre environnement.

### Utilisation des composants SDK spécifiques à Windows

Pour les applications Windows uniquement, les composants SDK classiques offrent des méthodes simples pour l'extraction d'images :

```csharp
// Utilisation de VideoEditCore pour l'extraction d'image
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.MediaPlayer;

public void ExtractFrameWithVideoEditCore()
{
    var videoEdit = new VideoEditCore();
    var bitmap = videoEdit.Helpful_GetFrameFromFile(
        "C:\\Videos\\sample.mp4",
        TimeSpan.FromSeconds(5),
        oldWay: false,
        MediaPlayerSourceMode.LAV);
    bitmap.Save("C:\\Output\\frame.png");
}

// Utilisation de MediaPlayerCore pour l'extraction d'image
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.MediaPlayer;

public void ExtractFrameWithMediaPlayerCore()
{
    var mediaPlayer = new MediaPlayerCore();
    var bitmap = mediaPlayer.Helpful_GetFrameFromFile(
        "C:\\Videos\\sample.mp4",
        TimeSpan.FromSeconds(10),
        oldWay: false,
        MediaPlayerSourceMode.LAV);
    bitmap.Save("C:\\Output\\frame.png");
}
```

La méthode `Helpful_GetFrameFromFile` simplifie le processus en gérant l'ouverture du fichier, le positionnement et le décodage de l'image en un seul appel. Les quatre arguments sont obligatoires : le chemin du fichier, l'horodatage de positionnement, un flag `oldWay` (passez `false` pour le chemin de décodeur moderne) et un sélecteur de moteur `MediaPlayerSourceMode` (`LAV`, `FFMPEG` ou `File_DS`).

### Solutions multiplateformes avec le moteur X

Les applications .NET modernes doivent souvent fonctionner sur plusieurs plateformes. Le moteur X fournit des capacités multiplateformes pour l'extraction d'images vidéo :

#### Extraction d'images sous forme de System.Drawing.Bitmap

L'approche la plus courante consiste à extraire les images sous forme d'objets `System.Drawing.Bitmap` :

```csharp
using VisioForge.Core.MediaInfoReaderX;

public void ExtractFrameAsBitmap()
{
    // Extraire l'image au début de la vidéo (TimeSpan.Zero)
    var bitmap = MediaInfoReaderX.GetFileSnapshotBitmap("C:\\Videos\\sample.mp4", TimeSpan.Zero);
    
    // Extraire une image à 30 secondes dans la vidéo
    var frame30sec = MediaInfoReaderX.GetFileSnapshotBitmap("C:\\Videos\\sample.mp4", TimeSpan.FromSeconds(30));
    
    // Enregistrer l'image extraite
    bitmap.Save("C:\\Output\\first-frame.png");
    frame30sec.Save("C:\\Output\\frame-30sec.png");
}
```

#### Extraction d'images sous forme de bitmaps SkiaSharp

Pour les applications utilisant SkiaSharp pour le traitement graphique, vous pouvez extraire les images directement sous forme d'objets `SKBitmap` :

```csharp
using VisioForge.Core.MediaInfoReaderX;
using SkiaSharp;

public void ExtractFrameAsSkiaBitmap()
{
    // Extraire l'image à 15 secondes dans la vidéo
    var skBitmap = MediaInfoReaderX.GetFileSnapshotSKBitmap("C:\\Videos\\sample.mp4", TimeSpan.FromSeconds(15));
    
    // Travailler avec le SKBitmap
    using (var image = SKImage.FromBitmap(skBitmap))
    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
    using (var stream = File.OpenWrite("C:\\Output\\frame-skia.png"))
    {
        data.SaveTo(stream);
    }
}
```

#### Enregistrer une image par minute en PNG

Une application console complète : elle enregistre neuf images en fichiers PNG, une par minute de 0:30 à 8:30. `GetFileSnapshotSKBitmap` renvoie `null` quand il n'y a pas d'image à une position, par exemple après la fin du fichier, et cette position est ignorée. `SKBitmap.Encode` écrit le PNG. Sur un film H.264 de 10 minutes en 1280x720, les neuf images ont pris environ trois secondes.

```csharp
using System;
using System.IO;
using SkiaSharp;
using VisioForge.Core;
using VisioForge.Core.MediaInfoReaderX;

await VisioForgeX.InitSDKAsync();
for (var sec = 30; sec < 540; sec += 60)
{
    using var frame = MediaInfoReaderX
        .GetFileSnapshotSKBitmap("movie.mp4",
            TimeSpan.FromSeconds(sec));
    if (frame == null) continue;
    using var png = frame.Encode(
        SKEncodedImageFormat.Png, 100);
    File.WriteAllBytes($"frame-{sec}.png",
        png.ToArray());
}
VisioForgeX.DestroySDK();
```

Le projet complet est [extract-frames-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/_VideoTutorials/extract-frames-short-console), une application console Windows x64 qui référence les paquets natifs du SDK pour cette plateforme, `VisioForge.CrossPlatform.Core.Windows.x64` et `VisioForge.CrossPlatform.Libav.Windows.x64`. Les noms de fichier sont relatifs au répertoire de travail : placez donc `movie.mp4` dans le dossier depuis lequel vous lancez l'application.

La vidéo d'une minute écrit et exécute cette application.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/u0rO3xSzO5w?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

#### Travail avec des données RGB brutes

Pour des scénarios plus avancés ou lorsque vous avez besoin d'une manipulation directe des pixels, vous pouvez extraire les images sous forme de tableaux d'octets RGB :

```csharp
using VisioForge.Core.MediaInfoReaderX;

public void ExtractFrameAsRGBArray()
{
    // GetFileSnapshotRGB retourne Tuple<byte[], int, int> : pixels + largeur + hauteur.
    var snapshot = MediaInfoReaderX.GetFileSnapshotRGB("C:\\Videos\\sample.mp4", TimeSpan.FromSeconds(20));
    byte[] rgbData = snapshot.Item1;
    int width = snapshot.Item2;
    int height = snapshot.Item3;

    // Traiter les données RGB — le tampon contient les valeurs R, G, B pour chaque pixel ; largeur et
    // hauteur viennent directement du tuple, donc aucun appel séparé de métadonnées n'est nécessaire.
}
```

## Bonnes pratiques pour l'extraction d'images vidéo

Lors de l'implémentation de l'extraction d'images vidéo dans vos applications, considérez ces bonnes pratiques :

### Considérations de performance

- L'extraction d'images peut être gourmande en CPU, en particulier pour les vidéos haute résolution
- Envisagez d'implémenter des mécanismes de cache pour les images fréquemment consultées
- Pour l'extraction par lots, implémentez un traitement parallèle lorsque c'est approprié

```csharp
// Exemple d'extraction d'images en parallèle
public void ExtractMultipleFramesInParallel(string videoPath, TimeSpan[] timestamps)
{
    Parallel.ForEach(timestamps, timestamp => {
        var bitmap = MediaInfoReaderX.GetFileSnapshotBitmap(videoPath, timestamp);
        bitmap.Save($"C:\\Output\\frame-{timestamp.TotalSeconds}.png");
    });
}
```

### Gestion des erreurs

Implémentez toujours une gestion d'erreur appropriée lors du travail avec des fichiers vidéo :

```csharp
public Bitmap SafeExtractFrame(string videoPath, TimeSpan position)
{
    try
    {
        return MediaInfoReaderX.GetFileSnapshotBitmap(videoPath, position);
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("Video file not found");
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("Invalid position in video");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error extracting frame: {ex.Message}");
    }
    
    return null;
}
```

### Gestion de la mémoire

Une gestion correcte de la mémoire est cruciale, en particulier lors du travail avec de grands fichiers vidéo :

```csharp
public void ExtractFrameWithProperDisposal()
{
    Bitmap bitmap = null;
    try
    {
        bitmap = MediaInfoReaderX.GetFileSnapshotBitmap("C:\\Videos\\sample.mp4", TimeSpan.FromSeconds(5));
        // Traiter le bitmap...
    }
    finally
    {
        bitmap?.Dispose();
    }
}
```

## Applications courantes

L'extraction d'images est utilisée dans diverses applications multimédias :

- **Lecteurs vidéo** : génération de miniatures d'aperçu
- **Bibliothèques multimédias** : création de miniatures vidéo pour les vues en galerie
- **Analyse vidéo** : extraction d'images pour le traitement de vision par ordinateur
- **Gestion de contenu** : création d'images d'aperçu pour les ressources vidéo
- **Édition vidéo** : fourniture d'une référence visuelle pour l'édition de timeline

## Conclusion

L'extraction d'images depuis des fichiers vidéo est une capacité puissante pour les développeurs .NET travaillant avec du contenu multimédia. Que vous construisiez des applications spécifiques à Windows ou des solutions multiplateformes, les méthodes décrites dans ce guide fournissent des moyens efficaces de capturer et de travailler avec des images vidéo.

En comprenant les différentes approches et en suivant les bonnes pratiques, vous pouvez implémenter une fonctionnalité robuste d'extraction d'images dans vos applications .NET.

## Voir aussi

Une fois que vous savez extraire une image à n'importe quel horodatage, vous pouvez indexer et rechercher tout le fichier par son contenu :

- [Recherche vidéo sémantique](../../general/ai/semantic-video-search.md) — retrouvez un moment en le décrivant en texte simple, puis positionnez-vous à cet horodatage pour extraire l'image.
- [Détection à vocabulaire ouvert](../../general/ai/open-vocabulary-detection.md) — détectez tout objet que vous pouvez nommer dans une invite textuelle, sans données d'entraînement.

---
Pour plus d'exemples de code, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
