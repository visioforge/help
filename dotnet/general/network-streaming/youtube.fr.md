---
title: Diffusion YouTube Live en C# .NET avec sortie RTMP
description: Diffusez vers YouTube Live avec RTMP en .NET avec encodeurs vidéo optimisés, configuration audio et support multiplateforme.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - Editing
  - RTMP
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoEditCoreX
  - YouTubeOutput
  - NVENCH264EncoderSettings
  - MediaBlockPadMediaType

---

# Diffusion YouTube Live avec les SDK VisioForge

## Introduction à l'intégration du streaming YouTube

La fonctionnalité de sortie RTMP YouTube dans les SDK VisioForge permet aux développeurs de créer des applications .NET robustes qui diffusent du contenu vidéo de haute qualité directement vers YouTube. Cette implémentation exploite divers encodeurs vidéo et audio pour optimiser les performances de streaming sur différentes configurations matérielles et plateformes. Ce guide complet fournit des instructions détaillées sur la configuration et le dépannage du streaming YouTube dans vos applications.

## Plateformes SDK prises en charge

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Toutes les principales plateformes SDK VisioForge fournissent des capacités multiplateformes pour le streaming YouTube, assurant une fonctionnalité cohérente sur Windows, macOS et d'autres systèmes d'exploitation.

## Comprendre la classe YouTubeOutput

La classe `YouTubeOutput` sert d'interface principale pour la configuration du streaming YouTube, offrant de larges options de personnalisation, notamment :

- **Sélection et configuration de l'encodeur vidéo** : choisissez parmi plusieurs encodeurs accélérés matériellement et logiciels
- **Sélection et configuration de l'encodeur audio** : configurez les encodeurs audio AAC avec des paramètres personnalisés
- **Traitement vidéo et audio personnalisé** : appliquez des filtres et des transformations avant la diffusion
- **Paramètres de puits spécifiques à YouTube** : affinez les paramètres de diffusion spécifiques aux exigences de YouTube

## Prise en main : processus de configuration de base

### Configuration de la clé de stream

Le fondement de toute implémentation de streaming YouTube commence par votre clé de stream YouTube. Ce jeton d'authentification connecte votre application à votre chaîne YouTube :

```csharp
// Initialiser la sortie YouTube avec votre clé de stream
var youtubeOutput = new YouTubeOutput("your-youtube-stream-key");
```

## Options de configuration de l'encodeur vidéo

### Prise en charge complète des encodeurs vidéo

Le SDK prend en charge plusieurs encodeurs vidéo, chacun optimisé pour différents environnements matériels et exigences de performance :

| Type d'encodeur | Plateforme/Matériel | Caractéristiques de performance |
|--------------|-------------------|----------------------------|
| OpenH264 | Multiplateforme (logiciel) | Intensif en CPU, largement compatible |
| NVENC H264 | GPU NVIDIA | Accéléré matériellement, utilisation CPU réduite |
| QSV H264 | CPU Intel avec Quick Sync | Accéléré matériellement, efficace |
| AMF H264 | GPU AMD | Accéléré matériellement pour matériel AMD |
| HEVC/H265 | Divers (lorsque pris en charge) | Efficacité de compression supérieure |

### Sélection dynamique d'encodeur

Le système sélectionne intelligemment les encodeurs par défaut en fonction de la plateforme (OpenH264 sur la plupart des plateformes, Apple Media H264 sur macOS). Les développeurs peuvent remplacer ces valeurs par défaut pour exploiter des capacités matérielles spécifiques :

```csharp
// Exemple : utiliser l'encodeur NVIDIA NVENC si disponible
if (NVENCH264EncoderSettings.IsAvailable())
{
    youtubeOutput.Video = new NVENCH264EncoderSettings();
}
```

### Configuration des paramètres d'encodage vidéo

Chaque encodeur prend en charge la personnalisation de divers paramètres pour optimiser la qualité et les performances de streaming :

```csharp
var videoSettings = new OpenH264EncoderSettings
{
    Bitrate = 4500,  // Kbit/s — 4,5 Mbps
    GOPSize = 60,    // Keyframe toutes les 2 secondes à 30 fps (nom de propriété réel ; pas de KeyframeInterval)
    // Ajouter d'autres paramètres spécifiques à l'encodeur selon les besoins
};
youtubeOutput.Video = videoSettings;
```

## Configuration de l'encodeur audio

### Encodeurs audio AAC pris en charge

Le SDK prend en charge plusieurs encodeurs audio AAC pour assurer une qualité audio optimale sur différentes plateformes :

- **VO-AAC** : par défaut pour les plateformes non-Windows, fournissant un encodage audio cohérent
- **AVENC AAC** : option multiplateforme alternative avec différentes caractéristiques de performance
- **MF AAC** : encodeur spécifique à Windows exploitant Media Foundation

### Exemple de configuration de l'encodeur audio

```csharp
// Exemple : configurer les paramètres de l'encodeur audio.
// VOAACEncoderSettings expose uniquement Bitrate (Kbit/s) ; la fréquence d'échantillonnage suit la source en amont.
var audioSettings = new VOAACEncoderSettings
{
    Bitrate = 128,  // Kbit/s — 128 kbps
};
youtubeOutput.Audio = audioSettings;
```

## Stratégies d'optimisation spécifiques aux plateformes

### Fonctionnalités spécifiques à Windows

- Exploite les encodeurs Media Foundation (MF) pour des performances optimales sous Windows
- Fournit des capacités d'encodage HEVC/H265 étendues
- Par défaut sur MF AAC pour l'encodage audio, optimisé pour la plateforme Windows

### Considérations d'implémentation macOS

- Utilise automatiquement l'encodeur Apple Media H264 pour des performances natives
- Implémente VO-AAC pour l'encodage audio avec optimisation macOS

### Couche de compatibilité multiplateforme

- Replie sur OpenH264 pour la vidéo sur les plateformes sans optimisations spécifiques
- Utilise VO-AAC pour un encodage audio cohérent dans divers environnements

## Bonnes pratiques pour un streaming optimal

### Sélection d'encodeur sensible au matériel

- Vérifiez toujours la disponibilité de l'encodeur avant d'implémenter des options accélérées matériellement
- Implémentez des mécanismes de repli vers OpenH264 lorsque le matériel spécialisé n'est pas disponible
- Tenez compte des capacités d'encodeur spécifiques aux plateformes lors de la conception d'applications multiplateformes

### Paramètres de flux optimisés pour YouTube

- Respectez les débits recommandés par YouTube pour votre résolution cible
- Implémentez l'intervalle de keyframe standard de 2 secondes (60 images à 30 fps)
- Configurez une fréquence d'échantillonnage audio de 48 kHz pour respecter les spécifications audio de YouTube

### Gestion robuste des erreurs

- Développez une gestion d'erreurs complète pour les problèmes de connexion
- Implémentez une surveillance continue des performances de l'encodeur
- Créez des outils de diagnostic pour évaluer la santé du flux pendant le fonctionnement

## Exemples d'implémentation complets

### Intégration VideoCaptureCoreX/VideoEditCoreX

Cet exemple démontre une implémentation complète de streaming YouTube avec gestion des erreurs pour VideoCaptureCoreX/VideoEditCoreX :

```csharp
try
{
    var youtubeOutput = new YouTubeOutput("your-stream-key");
    
    // Configurer l'encodage vidéo (Bitrate en Kbit/s ; GOPSize contrôle la cadence des keyframes)
    if (NVENCH264EncoderSettings.IsAvailable())
    {
        youtubeOutput.Video = new NVENCH264EncoderSettings
        {
            Bitrate = 4500,
            GOPSize = 60,
        };
    }

    // Configurer l'encodage audio (Bitrate en Kbit/s)
    youtubeOutput.Audio = new MFAACEncoderSettings
    {
        Bitrate = 128,
    };

    // Faire tourner la clé de stream YouTube ou changer la destination du puits en remplaçant l'objet Sink.
    // YouTubeSinkSettings expose uniquement la clé Key du stream (les autres propriétés sont internes).
    // youtubeOutput.Sink = new YouTubeSinkSettings("new-stream-key");
    
    // Ajouter la sortie à l'instance de capture vidéo
    core.Outputs_Add(youtubeOutput, true); // core est une instance de VideoCaptureCoreX

    // Ou définir la sortie pour l'instance d'édition vidéo
    videoEdit.Output_Format = youtubeOutput; // videoEdit est une instance de VideoEditCoreX
}
catch (Exception ex)
{
    // Gérer les erreurs d'initialisation
    Console.WriteLine($"Échec de l'initialisation de la sortie YouTube : {ex.Message}");
}
```

### Implémentation avec le Media Blocks SDK

Pour les développeurs utilisant le Media Blocks SDK, cet exemple montre comment connecter les composants d'encodage au puits YouTube :

```csharp
// Créer le bloc puits YouTube (utilisant RTMP)
var youtubeSinkBlock = new YouTubeSinkBlock(new YouTubeSinkSettings("streaming key"));

// Connecter l'encodeur vidéo au bloc puits
pipeline.Connect(h264Encoder.Output, youtubeSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

// Connecter l'encodeur audio au bloc puits
pipeline.Connect(aacEncoder.Output, youtubeSinkBlock.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Dépannage des problèmes courants

### Problèmes d'initialisation de l'encodeur

- Vérifiez la disponibilité de l'encodeur matériel via les diagnostics système
- Assurez-vous que le système répond à toutes les exigences de votre encodeur choisi
- Confirmez l'installation correcte des pilotes spécifiques au matériel pour l'accélération GPU

### Échecs de connexion au flux

- Validez le format de la clé de stream et son statut d'expiration
- Testez la connectivité réseau aux serveurs de streaming de YouTube
- Vérifiez l'état du service YouTube via les canaux officiels

### Optimisation des performances

- Surveillez l'utilisation des ressources système pendant les sessions de streaming
- Ajustez les débits et paramètres d'encodage en fonction des ressources disponibles
- Envisagez de basculer vers l'accélération matérielle lorsque l'utilisation CPU est excessive

## Ressources et documentation supplémentaires

- [Documentation officielle de YouTube Live Streaming](https://support.google.com/youtube/topic/9257891)
- [Exigences techniques de stream YouTube](https://support.google.com/youtube/answer/2853702)

En exploitant ces options de configuration détaillées et bonnes pratiques, les développeurs peuvent créer des applications de streaming YouTube robustes à l'aide des SDK VisioForge qui diffusent du contenu de haute qualité tout en optimisant l'utilisation des ressources système sur plusieurs plateformes.
