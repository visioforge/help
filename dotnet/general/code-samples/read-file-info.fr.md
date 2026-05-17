---
title: Infos d'un fichier vidéo en C# .NET — durée, codec, flux
description: MediaInfoReader retourne les détails des flux vidéo/audio, pistes de sous-titres, FOURCC et balises ID3. Analyse multiplateforme avec VisioForge.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - Metadata
  - MP4
  - C#
primary_api_classes:
  - MediaInfoReader

---

# Lire les informations d'un fichier multimédia en C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

L'accès aux informations détaillées intégrées dans les fichiers multimédias est essentiel pour développer des applications sophistiquées telles que des lecteurs multimédias, des éditeurs vidéo, des systèmes de gestion de contenu et des outils d'analyse de fichiers. Comprendre des propriétés telles que les codecs, la résolution, la fréquence d'images, le débit binaire, la durée et les balises intégrées permet aux développeurs de créer des logiciels plus intelligents et conviviaux.

Ce guide montre comment lire des informations complètes à partir de fichiers vidéo et audio en C# avec la classe `MediaInfoReader`. Les techniques présentées sont applicables à divers projets .NET et fournissent une base pour gérer les fichiers multimédias par programmation.

## Pourquoi extraire les informations des fichiers multimédias ?

Les informations des fichiers multimédias servent à plusieurs fins dans le développement d'applications :

- **Expérience utilisateur** : afficher les détails techniques aux utilisateurs dans les lecteurs multimédias
- **Vérifications de compatibilité** : vérifier si les fichiers répondent aux spécifications requises
- **Traitement automatisé** : configurer les paramètres d'encodage en fonction des propriétés de la source
- **Organisation du contenu** : cataloguer les bibliothèques multimédias avec des métadonnées précises
- **Évaluation de la qualité** : évaluer les fichiers multimédias pour détecter d'éventuels problèmes

## Guide d'implémentation

Explorons le processus d'extraction des informations de fichiers multimédias étape par étape. Les exemples supposent une application WinForms avec un contrôle `TextBox` nommé `mmInfo` pour afficher les informations extraites.

### Étape 1 : initialiser le lecteur d'informations multimédias

La première étape consiste à créer une instance de la classe `MediaInfoReader` :

```csharp
// Importer l'espace de noms nécessaire
using VisioForge.Core.MediaInfo; // Espace de noms pour MediaInfoReader
using VisioForge.Core.Helpers;  // Espace de noms pour TagLibHelper (optionnel)

// Créer une instance de MediaInfoReader
var infoReader = new MediaInfoReader();
```

Cette initialisation prépare le lecteur à traiter les fichiers multimédias.

### Étape 2 : vérifier la lisibilité du fichier (optionnel)

Avant de plonger dans une analyse détaillée, il est souvent utile de vérifier si le fichier est pris en charge :

```csharp
// Définir les variables pour contenir les informations d'erreur potentielles
FilePlaybackError errorCode;
string errorText;

// Spécifier le chemin du fichier multimédia
string filename = @"C:\path\to\your\mediafile.mp4"; // Remplacer par le chemin de votre fichier réel

// Vérifier si le fichier est lisible
if (MediaInfoReader.IsFilePlayable(filename, out errorCode, out errorText))
{
    // Afficher un message de succès
    mmInfo.Text += "Status: This file appears to be playable." + Environment.NewLine;
}
else
{
    // Afficher un message d'erreur incluant le code d'erreur et la description
    mmInfo.Text += $"Status: This file might not be playable. Error: {errorCode} - {errorText}" + Environment.NewLine;
}

mmInfo.Text += "------------------------------------" + Environment.NewLine;
```

Cette vérification fournit un retour anticipé sur l'intégrité et la compatibilité du fichier.

### Étape 3 : extraire les informations détaillées des flux

Nous pouvons maintenant extraire les métadonnées riches du fichier :

```csharp
try
{
    // Affecter le nom de fichier au lecteur
    infoReader.Filename = filename;

    // Lire les informations du fichier (true pour une analyse complète)
    infoReader.ReadFileInfo(true);

    // Traiter les flux vidéo
    mmInfo.Text += $"Found {infoReader.VideoStreams.Count} video stream(s)." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.VideoStreams.Count; i++)
    {
        var stream = infoReader.VideoStreams[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Video Stream #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Codec: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Duration: {stream.Duration}" + Environment.NewLine;
        mmInfo.Text += $"  Dimensions: {stream.Width}x{stream.Height}" + Environment.NewLine;
        mmInfo.Text += $"  FOURCC: {stream.FourCC}" + Environment.NewLine;
        
        if (stream.AspectRatio != null && stream.AspectRatio.Item1 > 0 && stream.AspectRatio.Item2 > 0)
        {
             mmInfo.Text += $"  Aspect Ratio: {stream.AspectRatio.Item1}:{stream.AspectRatio.Item2}" + Environment.NewLine;
        }
        
        // VideoFrameRate est un struct (numérateur/dénominateur) et n'implémente pas IFormattable — formater via .Value (double).
        mmInfo.Text += $"  Frame Rate: {stream.FrameRate.Value:F2} fps" + Environment.NewLine;
        mmInfo.Text += $"  Bitrate: {stream.Bitrate / 1000.0:F0} kbps" + Environment.NewLine;
        mmInfo.Text += $"  Frames Count: {stream.FramesCount}" + Environment.NewLine;
    }

    // Traiter les flux audio
    mmInfo.Text += Environment.NewLine;
    mmInfo.Text += $"Found {infoReader.AudioStreams.Count} audio stream(s)." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.AudioStreams.Count; i++)
    {
        var stream = infoReader.AudioStreams[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Audio Stream #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Codec: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Codec Info: {stream.CodecInfo}" + Environment.NewLine;
        mmInfo.Text += $"  Duration: {stream.Duration}" + Environment.NewLine;
        mmInfo.Text += $"  Bitrate: {stream.Bitrate / 1000.0:F0} kbps" + Environment.NewLine;
        mmInfo.Text += $"  Channels: {stream.Channels}" + Environment.NewLine;
        mmInfo.Text += $"  Sample Rate: {stream.SampleRate} Hz" + Environment.NewLine;
        mmInfo.Text += $"  Bits Per Sample (BPS): {stream.BPS}" + Environment.NewLine;
        mmInfo.Text += $"  Language: {stream.Language}" + Environment.NewLine;
    }

    // Traiter les flux de sous-titres
    mmInfo.Text += Environment.NewLine;
    mmInfo.Text += $"Found {infoReader.Subtitles.Count} subtitle stream(s)." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.Subtitles.Count; i++)
    {
        var stream = infoReader.Subtitles[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Subtitle Stream #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Codec/Format: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Name: {stream.Name}" + Environment.NewLine;
        mmInfo.Text += $"  Language: {stream.Language}" + Environment.NewLine;
    }
}
catch (Exception ex)
{
    // Gérer les erreurs potentielles lors de la lecture du fichier
    mmInfo.Text += $"{Environment.NewLine}Error reading file info: {ex.Message}{Environment.NewLine}";
}
finally
{
    // Important : libérer le lecteur pour libérer les handles de fichier et les ressources
    infoReader.Dispose();
}
```

Le code itère à travers chaque collection (`VideoStreams`, `AudioStreams` et `Subtitles`), en extrayant et en affichant les informations pertinentes pour chaque flux trouvé.

### Étape 4 : extraire les balises de métadonnées

Au-delà des informations techniques des flux, les fichiers multimédias contiennent souvent des balises de métadonnées :

```csharp
// Lire les balises de métadonnées
mmInfo.Text += Environment.NewLine + "--- Metadata Tags ---" + Environment.NewLine;
try
{
    // Utiliser TagLibHelper pour lire les balises du fichier
    var tags = TagLibHelper.ReadTags(filename);

    // Vérifier si les balises ont été lues avec succès
    if (tags != null)
    {
        mmInfo.Text += $"Title: {tags.Title}" + Environment.NewLine;
        mmInfo.Text += $"Artist(s): {string.Join(", ", tags.Performers ?? new string[0])}" + Environment.NewLine;
        mmInfo.Text += $"Album: {tags.Album}" + Environment.NewLine;
        mmInfo.Text += $"Year: {tags.Year}" + Environment.NewLine;
        mmInfo.Text += $"Genre: {string.Join(", ", tags.Genres ?? new string[0])}" + Environment.NewLine;
        mmInfo.Text += $"Comment: {tags.Comment}" + Environment.NewLine;
    }
    else
    {
        mmInfo.Text += "No standard metadata tags found or readable." + Environment.NewLine;
    }
}
catch (Exception ex)
{
    // Gérer les erreurs lors de la lecture des balises
    mmInfo.Text += $"Error reading tags: {ex.Message}" + Environment.NewLine;
}
```

## Bonnes pratiques pour l'analyse de fichiers multimédias

Lorsque vous implémentez l'analyse de fichiers multimédias dans vos applications, prenez en compte ces bonnes pratiques :

### Gestion des erreurs

Encapsulez toujours les opérations sur les fichiers dans des blocs try-catch appropriés. Les fichiers multimédias peuvent être corrompus, inaccessibles ou dans des formats inattendus, ce qui peut provoquer des exceptions.

```csharp
try {
    // Opérations sur les fichiers multimédias
}
catch (Exception ex) {
    // Journaliser l'erreur et fournir un retour à l'utilisateur
}
```

### Gestion des ressources

Libérez correctement les objets qui accèdent aux ressources de fichier pour éviter les problèmes de verrouillage de fichier :

```csharp
using (var infoReader = new MediaInfoReader())
{
    // Utiliser le lecteur
}
// Ou manuellement dans un bloc finally
try {
    // Opérations
}
finally {
    infoReader.Dispose();
}
```

### Considérations de performance

Pour les grandes bibliothèques multimédias, envisagez de :

1. Implémenter des mécanismes de cache pour les analyses répétées
2. Utiliser des threads d'arrière-plan pour le traitement afin de garder l'UI réactive
3. Limiter la profondeur de l'analyse pour des balayages rapides initiaux

## Composants requis

Pour une implémentation réussie, assurez-vous que votre projet inclut les dépendances nécessaires telles que spécifiées dans la documentation du SDK.

## Conclusion

L'extraction d'informations à partir de fichiers multimédias est une capacité puissante pour les développeurs qui construisent des applications travaillant avec du contenu audio et vidéo. Avec les techniques décrites dans ce guide, vous pouvez accéder à des propriétés techniques détaillées et à des balises de métadonnées pour enrichir les fonctionnalités de votre application.

La classe `MediaInfoReader` fournit un moyen pratique et efficace d'extraire les métadonnées nécessaires, ce qui vous permet de construire des fonctionnalités de gestion multimédia plus sophistiquées dans vos applications C#.

Pour des scénarios plus avancés, explorez les capacités complètes du SDK et consultez la documentation détaillée. Vous pouvez trouver des exemples de code supplémentaires sur GitHub pour étendre davantage vos capacités de traitement de fichiers multimédias.
