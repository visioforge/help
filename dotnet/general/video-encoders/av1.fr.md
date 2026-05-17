---
title: Encodage vidéo AV1 en C# .NET — guide de configuration
description: Configurez les encodeurs AV1 pour la capture vidéo, l'édition et les pipelines multiplateformes. Compression de nouvelle génération avec exemples VisioForge.
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
  - Conversion
  - AV1
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoEditCoreX
  - AMFAV1EncoderSettings
  - NVENCAV1EncoderSettings
  - QSVAV1EncoderSettings

---

# Encodeurs AV1

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

VisioForge prend en charge plusieurs implémentations d'encodeurs AV1, chacune avec ses propres fonctionnalités et capacités. Ce document couvre les encodeurs disponibles et leurs options de configuration.

Actuellement, les encodeurs AV1 sont pris en charge dans les moteurs multiplateformes : `VideoCaptureCoreX`, `VideoEditCoreX` et `Media Blocks SDK`.

## Encodeurs disponibles

1. [Encodeur AMD AMF AV1 (AMF)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AMFAV1EncoderSettings.html)
2. [Encodeur NVIDIA NVENC AV1 (NVENC)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.NVENCAV1EncoderSettings.html)
3. [Encodeur Intel QuickSync AV1 (QSV)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.QSVAV1EncoderSettings.html)
4. [Encodeur AOM AV1](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AOMAV1EncoderSettings.html)
5. [Encodeur RAV1E](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.RAV1EEncoderSettings.html)

Vous pouvez utiliser l'encodeur AV1 avec une [sortie WebM](../output-formats/webm.md) ou pour le streaming réseau.

## Encodeur AMD AMF AV1

L'encodeur AMD AMF AV1 fournit un encodage accéléré par le matériel utilisant les cartes graphiques AMD.

### Fonctionnalités

- Plusieurs préréglages de qualité
- Modes de contrôle de débit variable
- Contrôle de la taille de GOP
- Contrôle du QP (paramètre de quantification)
- Prise en charge de Smart Access Video

### Modes de contrôle de débit

- `Default` : dépend de l'usage
- `CQP` : QP constant
- `LCVBR` : VBR contraint par latence
- `VBR` : VBR contraint par crête
- `CBR` : débit constant

### Exemple d'utilisation

```csharp
var encoderSettings = new AMFAV1EncoderSettings
{
    Bitrate = 3000,                              // 3 Mbps
    GOPSize = 30,                                // Taille de GOP de 30 images
    Preset = AMFAV1EncoderPreset.Quality,        // Préréglage qualité
    RateControl = AMFAV1RateControlMode.VBR,     // Mode débit variable
    Usage = AMFAV1EncoderUsage.Transcoding,      // Usage transcodage
    MaxBitrate = 5000,                           // Débit max 5 Mbps
    QpI = 26,                                    // QP des I-frames
    QpP = 26,                                    // QP des P-frames
    RefFrames = 1,                               // Nombre d'images de référence
    SmartAccessVideo = false                     // Smart Access Video désactivé
};
```

## Encodeur NVIDIA NVENC AV1

L'encodeur NVENC AV1 de NVIDIA fournit un encodage accéléré par le matériel utilisant les GPU NVIDIA.

### Fonctionnalités

- Plusieurs préréglages d'encodage
- Prise en charge des B-frames adaptatifs
- AQ temporelle (quantification adaptative)
- Contrôle de tampon VBV (Video Buffering Verifier)
- Prise en charge de l'AQ spatiale

### Modes de contrôle de débit

- `Default` : mode par défaut
- `ConstQP` : paramètre de quantification constant
- `CBR` : débit constant
- `VBR` : débit variable
- `CBR_LD_HQ` : CBR basse latence, haute qualité
- `CBR_HQ` : CBR, haute qualité (plus lent)
- `VBR_HQ` : VBR, haute qualité (plus lent)

### Exemple d'utilisation

```csharp
var encoderSettings = new NVENCAV1EncoderSettings
{
    Bitrate = 3000,                          // 3 Mbps
    Preset = NVENCPreset.HighQuality,        // Préréglage haute qualité
    RateControl = NVENCRateControl.VBR,      // Mode débit variable
    GOPSize = 75,                            // Taille de GOP de 75 images
    MaxBitrate = 5000,                       // Débit max 5 Mbps
    BFrames = 2,                             // 2 B-frames entre I et P
    RCLookahead = 8,                         // Look-ahead de 8 images
    TemporalAQ = true,                       // Activer l'AQ temporelle
    Tune = NVENCTune.HighQuality,            // Réglage haute qualité
    VBVBufferSize = 6000                     // Tampon VBV de 6000k
};
```

## Encodeur Intel QuickSync AV1

L'encodeur QuickSync AV1 d'Intel fournit un encodage accéléré par le matériel utilisant les GPU Intel.

### Fonctionnalités

- Prise en charge du mode basse latence
- Target usage configurable
- Contrôle des images de référence
- Paramètres flexibles de taille de GOP

### Modes de contrôle de débit

- `CBR` : débit constant
- `VBR` : débit variable
- `CQP` : quantificateur constant

### Exemple d'utilisation

```csharp
var encoderSettings = new QSVAV1EncoderSettings
{
    Bitrate = 2000,                              // 2 Mbps
    LowLatency = false,                          // Mode latence standard
    TargetUsage = 4,                             // Qualité/vitesse équilibrées
    GOPSize = 30,                                // Taille de GOP de 30 images
    MaxBitrate = 4000,                           // Débit max 4 Mbps
    QPI = 26,                                    // QP des I-frames
    QPP = 28,                                    // QP des P-frames
    RateControl = QSVAV1EncRateControl.VBR,      // Mode débit variable
    RefFrames = 1                                // Nombre d'images de référence
};
```

## Encodeur AOM AV1

L'encodeur AV1 de l'Alliance for Open Media (AOM) est une implémentation logicielle de référence.

### Fonctionnalités

- Paramètres de contrôle de tampon
- Optimisation de l'utilisation CPU
- Prise en charge de l'abandon d'images
- Capacités multithreading
- Prise en charge de la super-résolution

### Modes de contrôle de débit

- `VBR` : mode débit variable
- `CBR` : mode débit constant
- `CQ` : mode qualité contrainte
- `Q` : mode qualité constante

### Exemple d'utilisation

```csharp
var encoderSettings = new AOMAV1EncoderSettings
{
    BufferInitialSize = TimeSpan.FromMilliseconds(4000),
    BufferOptimalSize = TimeSpan.FromMilliseconds(5000),
    BufferSize = TimeSpan.FromMilliseconds(6000),
    CPUUsed = 4,                                   // Niveau d'utilisation CPU
    DropFrame = 0,                                 // Désactiver l'abandon d'images
    RateControl = AOMAV1EncoderEndUsageMode.VBR,   // Mode débit variable
    TargetBitrate = 256,                           // 256 Kbps
    Threads = 0,                                   // Nombre de threads automatique
    UseRowMT = true,                               // Activer le multithreading par ligne
    SuperResMode = AOMAV1SuperResolutionMode.None  // Pas de super-résolution
};
```

## Encodeur RAV1E

RAV1E est un encodeur AV1 rapide et sûr écrit en Rust.

### Fonctionnalités

- Contrôle du préréglage de vitesse
- Paramètres de quantificateur
- Contrôle de l'intervalle d'images-clés
- Mode basse latence
- Réglage psychovisuel

### Exemple d'utilisation

```csharp
var encoderSettings = new RAV1EEncoderSettings
{
    Bitrate = 3000,                               // 3 Mbps
    LowLatency = false,                           // Mode latence standard
    MaxKeyFrameInterval = 240,                    // Intervalle maximum entre images-clés
    MinKeyFrameInterval = 12,                     // Intervalle minimum entre images-clés
    MinQuantizer = 0,                             // Valeur minimale du quantificateur
    Quantizer = 100,                              // Valeur de base du quantificateur
    SpeedPreset = 6,                              // Préréglage de vitesse (0-10)
    Tune = RAV1EEncoderTune.Psychovisual          // Réglage psychovisuel
};
```

## Notes d'utilisation générales

1. Tous les encodeurs implémentent l'interface `IAV1EncoderSettings`, offrant un moyen cohérent de créer des blocs d'encodeur.
2. Chaque encodeur a son propre ensemble d'optimisations et de compromis.
3. Les encodeurs matériels (AMF, NVENC, QSV) offrent généralement de meilleures performances mais peuvent avoir des exigences matérielles spécifiques.
4. Les encodeurs logiciels (AOM, RAV1E) offrent plus de flexibilité mais peuvent nécessiter plus de ressources CPU.

## Recommandations

- Pour les GPU AMD : utilisez l'encodeur AMF
- Pour les GPU NVIDIA : utilisez l'encodeur NVENC
- Pour les GPU Intel : utilisez l'encodeur QSV
- Pour la qualité maximale : utilisez l'encodeur AOM
- Pour un encodage efficace en CPU : utilisez l'encodeur RAV1E

## Bonnes pratiques

1. Vérifiez toujours la disponibilité de l'encodeur avant de l'utiliser
2. Définissez des débits appropriés à votre résolution et fréquence d'images cibles
3. Utilisez des tailles de GOP appropriées au type de contenu
4. Tenez compte du compromis entre qualité et vitesse d'encodage
5. Testez différents modes de contrôle de débit pour trouver le meilleur ajustement pour votre cas d'usage
