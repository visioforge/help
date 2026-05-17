---
title: Sortie vidéo WebM avec codecs VP8, VP9, AV1 en .NET
description: Créez des vidéos WebM en .NET avec VP8, VP9 et AV1 pour un streaming vidéo prêt pour le Web et la diffusion de contenu HTML5.
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
  - Recording
  - Encoding
  - Editing
  - WebM
  - VP8
  - VP9
  - AV1
  - Opus
  - Vorbis
  - C#
  - NuGet
primary_api_classes:
  - WebMOutput
  - VideoCaptureCore
  - VideoEditCore
  - VideoCaptureCoreX
  - VideoEditCoreX

---

# Sortie vidéo WebM dans les SDK VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Qu'est-ce que WebM ?

WebM est un format de fichier multimédia open source et libre de redevances, optimisé pour la diffusion Web. Conçu pour offrir un streaming vidéo efficace avec des exigences de traitement minimales, WebM est devenu un standard pour le contenu vidéo HTML5. Le format prend en charge les codecs modernes, dont VP8 et VP9 pour la compression vidéo, ainsi que Vorbis et Opus pour l'encodage audio.

Les principaux avantages de WebM incluent :

- **Performances optimisées pour le Web** avec des temps de chargement rapides
- **Large prise en charge des navigateurs** sur les principales plateformes
- **Vidéo de haute qualité** avec des tailles de fichier réduites
- **Licences open source** sans coûts de redevances
- **Capacités de streaming efficaces** pour les applications multimédias

## Implémentation Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Sur les plateformes Windows, l'implémentation de VisioForge tire parti de la classe [WebMOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WebMOutput.html) de l'espace de noms `VisioForge.Core.Types.X.Output`.

### Configuration de base

Pour implémenter rapidement la sortie WebM dans votre application Windows :

```csharp
using VisioForge.Core.Types.Output;

// Initialiser les paramètres de sortie WebM
var webmOutput = new WebMOutput();

// Configurer les paramètres essentiels
webmOutput.Video_Mode = VP8QualityMode.Realtime;
webmOutput.Video_EndUsage = VP8EndUsageMode.VBR;
webmOutput.Video_Encoder = WebMVideoEncoder.VP8;
webmOutput.Video_Bitrate = 2000;
webmOutput.Audio_Quality = 80;

// Appliquer à votre instance principale
var core = new VideoCaptureCore(); // ou VideoEditCore
core.Output_Format = webmOutput;
core.Output_Filename = "output.webm";
```

### Paramètres de qualité vidéo

L'ajustement précis de la qualité vidéo WebM implique d'équilibrer plusieurs paramètres :

```csharp
var webmOutput = new WebMOutput();

// Paramètres de qualité
webmOutput.Video_MinQuantizer = 4;    // Valeurs inférieures = qualité supérieure (plage : 0-63)
webmOutput.Video_MaxQuantizer = 48;   // Limite supérieure de qualité (plage : 0-63)
webmOutput.Video_Bitrate = 2000;      // Débit cible en kbps

// Encoder avec plusieurs threads pour de meilleures performances
webmOutput.Video_ThreadCount = 4;     // Ajuster en fonction des cœurs CPU disponibles
```

### Contrôle des keyframes

Une configuration correcte des keyframes est cruciale pour un streaming et un positionnement efficaces :

```csharp
// Paramètres de keyframe
webmOutput.Video_Keyframe_MinInterval = 30;   // Nombre minimum d'images entre keyframes
webmOutput.Video_Keyframe_MaxInterval = 300;  // Nombre maximum d'images entre keyframes
webmOutput.Video_Keyframe_Mode = VP8KeyframeMode.Auto;
```

### Optimisation des performances

Équilibrez la vitesse d'encodage et la qualité avec ces paramètres :

```csharp
// Paramètres de performance
webmOutput.Video_CPUUsed = 0;           // Plage : -16 à 16 (plus élevé = encodage plus rapide, qualité inférieure)
webmOutput.Video_LagInFrames = 25;      // Tampon d'anticipation d'images (plus élevé = meilleure qualité)
webmOutput.Video_ErrorResilient = true; // Activer pour les applications de streaming
```

### Gestion du tampon

Pour les applications de streaming, une configuration de tampon appropriée améliore la stabilité de la lecture :

```csharp
// Paramètres de tampon
webmOutput.Video_Decoder_Buffer_Size = 6000;        // Taille du tampon en millisecondes
webmOutput.Video_Decoder_Buffer_InitialSize = 4000; // Niveau de remplissage initial du tampon
webmOutput.Video_Decoder_Buffer_OptimalSize = 5000; // Niveau de tampon cible

// Réglage fin du contrôle de débit
webmOutput.Video_UndershootPct = 50;  // Permet au débit de descendre sous la cible
webmOutput.Video_OvershootPct = 50;   // Permet au débit de dépasser temporairement la cible
```

## Implémentation multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Pour les applications multiplateformes, VisioForge fournit la classe [WebMOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WebMOutput.html) de l'espace de noms `VisioForge.Core.Types.X.Output`, offrant une flexibilité accrue des codecs.

### Configuration de base

```csharp
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types.X.AudioEncoders;

// Créer la sortie WebM
var webmOutput = new WebMOutput("output.webm");

// Configurer l'encodeur vidéo (VP8)
webmOutput.Video = new VP8EncoderSettings();

// Configurer l'encodeur audio (Vorbis)
webmOutput.Audio = new VorbisEncoderSettings();
```

### Intégration avec Video Capture SDK

```csharp
// Ajouter la sortie WebM à Video Capture SDK
var core = new VideoCaptureCoreX();
core.Outputs_Add(webmOutput, true);
```

### Intégration avec Video Edit SDK

```csharp
// Définir WebM comme format de sortie pour Video Edit SDK
var core = new VideoEditCoreX();
core.Output_Format = webmOutput;
```

### Intégration avec Media Blocks SDK

```csharp
// Créer les encodeurs
var vorbis = new VorbisEncoderSettings();
var vp9 = new VP9EncoderSettings();

// Configurer le bloc de sortie WebM
var webmSettings = new WebMSinkSettings("output.webm");
var webmOutput = new WebMOutputBlock(webmSettings, vp9, vorbis);

// Ajouter à votre pipeline
// pipeline.AddBlock(webmOutput);
```

## Guide de sélection de codec

### Codecs vidéo

Les SDK VisioForge prennent en charge plusieurs codecs vidéo pour WebM :

1. **VP8**
   - Vitesse d'encodage plus rapide
   - Exigences de calcul plus faibles
   - Compatibilité plus large avec les anciens navigateurs
   - Bonne qualité pour la vidéo standard

2. **VP9**
   - Meilleure efficacité de compression (fichiers 30 à 50 % plus petits par rapport à VP8)
   - Qualité supérieure au même débit
   - Performances d'encodage plus lentes
   - Idéal pour le contenu haute résolution

3. **AV1**
   - Codec de nouvelle génération avec une compression supérieure
   - Qualité la plus élevée par bit
   - Complexité d'encodage nettement plus élevée
   - Idéal pour les situations où le temps d'encodage n'est pas critique

Pour les paramètres spécifiques aux codecs, consultez nos pages de documentation dédiées :

- [Configuration VP8/VP9](../video-encoders/vp8-vp9.md)
- [Configuration AV1](../video-encoders/av1.md)

### Codecs audio

Deux principales options de codec audio sont disponibles :

1. **Vorbis**
   - Codec établi offrant une bonne qualité globale
   - Compatible avec tous les navigateurs prenant en charge WebM
   - Choix par défaut pour la plupart des applications

2. **Opus**
   - Qualité audio supérieure, en particulier à faible débit
   - Meilleur pour le contenu vocal et la musique
   - Latence plus faible pour les applications de streaming
   - Plus efficace pour les scénarios à bande passante limitée

Pour les paramètres audio détaillés, consultez :

- [Configuration Vorbis](../audio-encoders/vorbis.md)
- [Configuration Opus](../audio-encoders/opus.md)

## Stratégies d'optimisation

### Pour la qualité vidéo

Pour atteindre la qualité vidéo la plus élevée possible :

- Utilisez VP9 ou AV1 pour l'encodage vidéo
- Définissez des valeurs de quantificateur plus basses (qualité supérieure)
- Augmentez `LagInFrames` pour une meilleure analyse anticipée
- Utilisez l'encodage en deux passes pour le traitement vidéo hors ligne
- Définissez des débits plus élevés pour le contenu visuel complexe

```csharp
// Configuration VP9 axée sur la qualité (les propriétés résident sur la base partagée VPXEncoderSettings).
var vp9 = new VP9EncoderSettings
{
    TargetBitrate = 3000,                    // kbps — débit plus élevé pour une meilleure qualité
    CPUUsed       = 0,                       // 0 = plus lent/meilleure qualité ; augmenter vers 5 pour la vitesse
    Deadline      = 0,                       // 0 = meilleur, 1 = temps réel (microsecondes par image)
    RateControl   = VPXRateControl.VBR,      // débit variable pour le travail axé sur la qualité
    MultipassMode = VPXMultipassMode.OnePass // passer à FirstPass/LastPass pour les flux en 2 passes
};
```

### Pour les applications en temps réel

Lorsque la faible latence est essentielle :

- Choisissez VP8 pour un encodage plus rapide
- Utilisez l'encodage en une passe
- Définissez `CPUUsed` à des valeurs plus élevées
- Utilisez des tampons d'anticipation d'images plus petits
- Configurez des intervalles de keyframe plus courts

```csharp
// Configuration VP8 à faible latence
var vp8 = new VP8EncoderSettings
{
    RateControl    = VPXRateControl.CBR,               // débit constant pour un streaming prévisible
    CPUUsed        = 8,                                // 0..16 sur VP8 — plus élevé = plus rapide/qualité moindre
    Deadline       = 1,                                // 1 = temps réel (microsecondes par image ; 0 = meilleure qualité)
    ErrorResilient = VPXErrorResilientFlags.Default    // activer les drapeaux de résilience pour la récupération des pertes de paquets
};
```

### Pour l'efficacité de la taille de fichier

Pour minimiser les besoins de stockage :

- Utilisez VP9 ou AV1 pour une compression maximale
- Activez l'encodage en deux passes
- Définissez des débits cibles appropriés
- Utilisez l'encodage à débit variable (VBR)
- Évitez les keyframes inutiles

```csharp
// Configuration optimisée pour le stockage utilisant l'encodeur de référence AOM AV1.
// Autres variantes AV1 : SVTAV1EncoderSettings, NVENCAV1EncoderSettings, AMFAV1EncoderSettings, QSVAV1EncoderSettings.
var av1 = new AOMAV1EncoderSettings
{
    RateControl = AOMAV1EncoderEndUsageMode.VBR,  // débit variable pour l'efficacité
    CPUUsed     = 2                               // équilibre entre vitesse et compression (0 = plus lent/meilleur)
};
```

## Dépendances

Pour implémenter la sortie WebM, ajoutez les paquets NuGet appropriés à votre projet :

- Pour les plateformes x86 : [VisioForge.DotNet.Core.Redist.WebM.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86)
- Pour les plateformes x64 : [VisioForge.DotNet.Core.Redist.WebM.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x64)

## Ressources d'apprentissage

Pour des exemples d'implémentation supplémentaires et des scénarios plus avancés, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) contenant des exemples de code pour tous les SDK VisioForge.
