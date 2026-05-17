---
title: Capture caméscope MPEG-2 avec encodeur matériel en C# .NET
description: Enregistrez la vidéo d'un caméscope au format MPEG-2 en .NET avec le VisioForge Video Capture SDK. Configuration de l'encodeur matériel et exemples C#.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Encoding
  - MPEG-2
  - C#
  - NuGet
primary_api_classes:
  - DirectCaptureMPEGOutput
  - VideoCaptureCore
  - VideoCaptureMode
  - PlaybackState

---

# Capturer la vidéo d'un caméscope au format MPEG-2

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction

Le MPEG-2 reste une norme d'encodage vidéo fiable, largement utilisée dans les flux de travail vidéo professionnels. Ce guide explique comment implémenter la capture d'un caméscope vers MPEG-2 dans vos applications .NET.

Le MPEG-2 (Moving Picture Experts Group 2) a été établi en 1995 comme norme industrielle pour l'encodage vidéo et audio numérique. Malgré l'apparition de formats plus récents, le MPEG-2 continue d'être apprécié pour son équilibre optimal entre efficacité de compression et qualité vidéo, ce qui le rend particulièrement adapté aux applications nécessitant une capture vidéo haute fidélité depuis des caméscopes.

## Pourquoi utiliser MPEG-2 pour la capture caméscope ?

Le MPEG-2 offre plusieurs avantages pour les développeurs qui implémentent la capture caméscope :

- **Compatibilité étendue** avec les logiciels d'édition vidéo et les appareils de lecture
- **Compression efficace** qui préserve la qualité visuelle pour des tailles de fichier raisonnables
- **Gestion robuste** du contenu vidéo entrelacé (courant dans les caméscopes)
- **Norme industrielle** qui garantit un support et une compatibilité à long terme
- **Exigences de traitement plus faibles** par rapport aux codecs modernes plus complexes

## Guide d'implémentation

### Dépendances requises

Avant d'implémenter la capture caméscope vers MPEG-2, assurez-vous que votre projet inclut :

- Video Capture SDK .NET (composant VideoCaptureCore)
- Redistribuables de capture vidéo :
  - [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

Installez ces paquets avec le gestionnaire de paquets NuGet :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

### Implémentation de base

Le code suivant montre comment configurer et exécuter une capture caméscope vers MPEG-2 basique :

```cs
// Initialiser le composant de capture vidéo. VideoCaptureCore nécessite un IVideoView
// (contrôle VideoView WinForms, VideoView WPF, etc.) pour héberger la fenêtre d'aperçu.
using var videoCapture = new VideoCaptureCore(VideoView1 as IVideoView);

// Configurer le format de sortie MPEG-2
videoCapture.Output_Format = new DirectCaptureMPEGOutput();

// Spécifier le mode de capture
videoCapture.Mode = VideoCaptureMode.VideoCapture;

// Définir le chemin du fichier de sortie
videoCapture.Output_Filename = "captured_video.mpg";

// Démarrer le processus de capture (de manière asynchrone)
await videoCapture.StartAsync();

// ... Code supplémentaire pour gérer le processus de capture

// Arrêter la capture une fois terminée
await videoCapture.StopAsync();
```

### Sélection des périphériques d'entrée

Pour que votre application capture depuis le caméscope correct :

```cs
// Lister les périphériques d'entrée vidéo disponibles (Video_CaptureDevices est une méthode, pas une propriété)
foreach (var device in videoCapture.Video_CaptureDevices())
{
    Console.WriteLine($"Device: {device.Name}");
}

// Sélectionner un caméscope et un format spécifiques
videoCapture.Video_CaptureDevice = ...
```

## Fonctionnalités avancées

### Configuration audio

Configurez les paramètres audio pour des résultats optimaux :

```cs
// Lister les périphériques audio disponibles (Audio_CaptureDevices est une méthode, pas une propriété)
foreach (var device in videoCapture.Audio_CaptureDevices())
{
    Console.WriteLine($"Audio device: {device.Name}");
}

// Sélectionner un périphérique et un format audio spécifiques
videoCapture.Audio_CaptureDevice = ...
```

### Gestion des événements

Surveillez et réagissez aux événements :

```cs
// S'abonner aux événements de changement d'état
videoCapture.OnError += (sender, args) => 
{
    Console.WriteLine($"Error: {args.Message}");
};
```

### Gestion de la mémoire

Assurez un nettoyage correct des ressources :

```cs
// Implémenter une libération correcte. State() est une méthode qui retourne PlaybackState
// (Free | Play | Pause) — il n'existe pas d'enum VideoCaptureState.
public async Task StopAndDisposeCapture()
{
    if (videoCapture != null)
    {
        if (videoCapture.State() == PlaybackState.Play)
        {
            await videoCapture.StopAsync();
        }

        videoCapture.Dispose();
    }
}
```

## Dépannage

Si vous rencontrez des problèmes avec votre capture caméscope MPEG-2 :

1. **Vérifiez la compatibilité du périphérique** — Assurez-vous que votre caméscope est correctement reconnu
2. **Vérifiez l'installation des pilotes** — Mettez à jour vers les derniers pilotes de périphérique
3. **Surveillez les ressources système** — La capture peut consommer beaucoup de ressources
4. **Inspectez la qualité de la connexion** — Des problèmes USB ou FireWire peuvent affecter la stabilité
5. **Testez avec différentes résolutions** — Les résolutions plus basses peuvent offrir de meilleures performances

## Conclusion

L'implémentation de la capture MPEG-2 depuis des caméscopes fournit une solution fiable pour les applications nécessitant une capture vidéo de haute qualité avec une large compatibilité. En suivant les techniques exposées dans ce guide, les développeurs peuvent créer une fonctionnalité de capture vidéo robuste qui conserve l'équilibre entre qualité et efficacité pour lequel le MPEG-2 est reconnu.

Pour des scénarios d'utilisation plus avancés et des exemples détaillés, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) contenant des exemples de code supplémentaires et des guides d'implémentation.
