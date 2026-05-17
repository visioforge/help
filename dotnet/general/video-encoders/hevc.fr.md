---
title: Encodage HEVC matériel avec GPU AMD, NVIDIA et Intel
description: Implémentez l'encodage HEVC (H.265) accéléré par matériel avec GPU AMD, NVIDIA et Intel pour une compression vidéo efficace en .NET.
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
  - Webcam
  - H.265
  - C#
primary_api_classes:
  - AMFHEVCEncoderSettings
  - NVENCHEVCEncoderSettings
  - QSVHEVCEncoderSettings
  - IHEVCEncoderSettings
  - SVTHEVCEncoderSettings

---

# Encodage HEVC matériel dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Ce guide explore les options d'encodage HEVC (H.265) accéléré par le matériel disponibles dans les SDK VisioForge .NET. Nous aborderons les détails d'implémentation pour les encodeurs GPU AMD, NVIDIA et Intel, vous aidant à choisir la bonne solution pour vos besoins de traitement vidéo.

Pour les formats de sortie spécifiques à Windows, consultez notre [documentation de sortie MP4](../output-formats/mp4.md).

## Vue d'ensemble des encodeurs HEVC matériels

Les GPU modernes offrent des capacités d'encodage matériel puissantes qui surpassent significativement les solutions logicielles. Les SDK VisioForge prennent en charge trois grands encodeurs HEVC matériels :

- **AMD AMF** — pour les GPU AMD Radeon
- **NVIDIA NVENC** — pour les GPU NVIDIA GeForce et professionnels
- **Intel QuickSync** — pour les CPU Intel avec graphiques intégrés

Chaque encodeur fournit des fonctionnalités et des options d'optimisation uniques. Explorons leurs capacités et leurs détails d'implémentation.

## Encodeur AMD AMF HEVC

L'Advanced Media Framework (AMF) d'AMD fournit un encodage HEVC accéléré par le matériel sur les GPU Radeon compatibles. Il équilibre vitesse d'encodage, qualité et efficacité pour divers scénarios.

### Fonctionnalités et paramètres clés

- **Méthodes de contrôle de débit** :
  - `CQP` (QP constant) pour des paramètres de qualité fixe
  - `LCVBR` (VBR contraint par latence) pour le streaming
  - `VBR` (débit variable) pour l'encodage hors-ligne
  - `CBR` (débit constant) pour une utilisation fiable de la bande passante

- **Profils d'usage** :
  - Transcoding (qualité maximale)
  - Ultra Low Latency (pour applications en temps réel)
  - Low Latency (pour streaming interactif)
  - Web Camera (optimisé pour sources webcam)

- **Préréglages de qualité** : équilibre entre vitesse d'encodage et qualité de sortie

### Exemple d'implémentation

```csharp
var encoder = new AMFHEVCEncoderSettings
{
    Bitrate = 3000, // Débit cible 3 Mbps
    MaxBitrate = 5000, // Débit de crête 5 Mbps
    RateControl = AMFHEVCEncoderRateControl.CBR,
    
    // Optimisation qualité
    Preset = AMFHEVCEncoderPreset.Quality,
    Usage = AMFHEVCEncoderUsage.Transcoding,
    
    // Paramètres GOP et image
    GOPSize = 30, // Intervalle entre images-clés
    QP_I = 22, // Paramètre de quantification I-frame
    QP_P = 22, // Paramètre de quantification P-frame
    
    RefFrames = 1 // Nombre d'images de référence
};
```

## Encodeur NVIDIA NVENC HEVC

La technologie NVENC de NVIDIA fournit du matériel d'encodage dédié sur les GPU GeForce et professionnels, offrant d'excellentes performances et qualité à divers débits.

### Capacités clés

- **Prise en charge de plusieurs profils** :
  - Main (8 bits)
  - Main10 (10 bits HDR)
  - Main444 (haute précision de couleur)
  - Options de profondeur de bit étendue (12 bits)

- **Fonctionnalités d'encodage avancées** :
  - Prise en charge des B-frames avec placement adaptatif
  - Quantification adaptative temporelle
  - Prédiction pondérée
  - Contrôle de débit look-ahead

- **Préréglages de performance** : du axé qualité au ultra-rapide

### Exemple d'implémentation

```csharp
var encoder = new NVENCHEVCEncoderSettings
{
    // Configuration du débit
    Bitrate = 3000, // Cible 3 Mbps
    MaxBitrate = 5000, // Maximum 5 Mbps
    
    // Paramètres de profil
    Profile = NVENCHEVCProfile.Main,
    Level = NVENCHEVCLevel.Level5_1,
    
    // Options d'amélioration de la qualité
    BFrames = 2, // Nombre de B-frames
    BAdaptive = true, // Placement adaptatif des B-frames
    TemporalAQ = true, // Quantification adaptative temporelle
    WeightedPrediction = true, // Améliore la qualité pour les fondus
    RCLookahead = 20, // Images à analyser pour le contrôle de débit
    
    // Paramètres de tampon
    VBVBufferSize = 0 // Utiliser la taille de tampon par défaut
};
```

## Encodeur Intel QuickSync HEVC

Intel QuickSync exploite le GPU intégré présent dans les processeurs Intel modernes pour un encodage matériel efficace, le rendant accessible sans carte graphique dédiée.

### Caractéristiques clés

- **Options polyvalentes de contrôle de débit** :
  - `CBR` (débit constant)
  - `VBR` (débit variable)
  - `CQP` (quantificateur constant)
  - `ICQ` (qualité constante intelligente)
  - `VCM` (mode visioconférence)
  - `QVBR` (VBR défini par la qualité)

- **Paramètres d'optimisation** :
  - Paramètre Target Usage (équilibre qualité vs vitesse)
  - Mode basse latence pour le streaming
  - Contrôles de conformité HDR
  - Options d'insertion de sous-titres codés

- **Prise en charge des profils** :
  - Main (8 bits)
  - Main10 (10 bits HDR)

### Exemple d'implémentation

```csharp
var encoder = new QSVHEVCEncoderSettings
{
    // Paramètres de débit
    Bitrate = 3000, // Cible 3 Mbps
    MaxBitrate = 5000, // Crête 5 Mbps
    RateControl = QSVHEVCEncRateControl.VBR,
    
    // Réglage de la qualité
    TargetUsage = 4, // 1 = meilleure qualité, 7 = encodage le plus rapide
    
    // Structure du flux
    GOPSize = 30, // Intervalle entre images-clés
    RefFrames = 2, // Images de référence
    
    // Configuration des fonctionnalités
    Profile = QSVHEVCEncProfile.Main,
    LowLatency = false, // Activer pour le streaming
    
    // Options avancées
    CCInsertMode = QSVHEVCEncSEIInsertMode.Insert,
    DisableHRDConformance = false
};
```

## Préréglages de qualité pour configuration simplifiée

Tous les encodeurs prennent en charge des préréglages de qualité standardisés via l'énumération `VideoQuality`, offrant une approche de configuration simplifiée :

- **Low** : cible 1 Mbps, max 2 Mbps (pour streaming basique)
- **Normal** : cible 3 Mbps, max 5 Mbps (pour contenu standard)
- **High** : cible 6 Mbps, max 10 Mbps (pour contenu détaillé)
- **Very High** : cible 15 Mbps, max 25 Mbps (pour qualité premium)

### Utilisation des préréglages de qualité

```csharp
// Pour AMD AMF
var amfEncoder = new AMFHEVCEncoderSettings(VideoQuality.High);

// Pour NVIDIA NVENC
var nvencEncoder = new NVENCHEVCEncoderSettings(VideoQuality.High);

// Pour Intel QuickSync
var qsvEncoder = new QSVHEVCEncoderSettings(VideoQuality.High);
```

## Détection matérielle et stratégie de repli

Une implémentation robuste doit vérifier la disponibilité des encodeurs et implémenter des solutions de repli appropriées :

```csharp
// Créer l'encodeur le plus approprié pour le système actuel
IHEVCEncoderSettings GetOptimalHEVCEncoder()
{
    if (AMFHEVCEncoderSettings.IsAvailable())
    {
        return new AMFHEVCEncoderSettings(VideoQuality.High);
    }
    else if (NVENCHEVCEncoderSettings.IsAvailable())
    {
        return new NVENCHEVCEncoderSettings(VideoQuality.High);
    }
    else if (QSVHEVCEncoderSettings.IsAvailable())
    {
        return new QSVHEVCEncoderSettings(VideoQuality.High);
    }
    else
    {
#if NET_LINUX
        // Solution de repli vers l'encodeur logiciel SVT-HEVC sur Linux (Linux uniquement — voir SVTHEVCEncoderSettings).
        return new SVTHEVCEncoderSettings();
#else
        // Le moteur X NE livre PAS de solution de repli IHEVCEncoderSettings typée pour Windows ou Apple.
        // (Sur les plateformes Apple, VideoToolbox HEVC n'est accessible que via un câblage GStreamer de bas niveau —
        // aucune classe `AppleMedia*HEVC*Settings` n'existe ; seul AppleMediaH264EncoderSettings est fourni.)
        // Laissez l'appelant décider : conserver une sortie H.264, installer un pilote GPU activant QSV/NVENC/AMF, ou
        // utiliser le pipeline FFMPEG-EXE du moteur classique.
        throw new NotSupportedException("No HEVC encoder available on this platform. Install a GPU driver or switch to H.264.");
#endif
    }
}
```

## Bonnes pratiques pour l'encodage HEVC

### 1. Sélection de l'encodeur

- **GPU AMD** : meilleur pour les applications où vous savez que les utilisateurs disposent de matériel AMD
- **GPU NVIDIA** : fournit une qualité constante à travers les générations, idéal pour les applications professionnelles
- **Intel QuickSync** : excellente option universelle lorsqu'un GPU dédié n'est pas garanti

### 2. Sélection du contrôle de débit

- **Streaming** : utilisez CBR pour une utilisation cohérente de la bande passante
- **Contenu VoD** : VBR offre une meilleure qualité à taille de fichier équivalente
- **Archivage** : CQP garantit une qualité constante quelle que soit la complexité du contenu

### 3. Optimisation des performances

- Réduisez le nombre d'images de référence pour un encodage plus rapide
- Ajustez la taille de GOP selon le type de contenu (plus petite pour beaucoup de mouvement, plus grande pour scènes statiques)
- Envisagez de désactiver les B-frames pour les applications à latence ultra-faible

### 4. Amélioration de la qualité

- Activez les fonctionnalités de quantification adaptative pour le contenu de complexité variable
- Utilisez la prédiction pondérée pour le contenu avec fondus ou transitions progressives
- Implémentez le look-ahead lorsque la qualité d'encodage est plus importante que la latence

## Dépannage courant

1. **Indisponibilité de l'encodeur** : assurez-vous que les pilotes GPU sont à jour
2. **Qualité inférieure aux attentes** : vérifiez si les préréglages de qualité correspondent à votre type de contenu
3. **Problèmes de performance** : surveillez l'utilisation du GPU et ajustez les paramètres en conséquence
4. **Problèmes de compatibilité** : vérifiez que les appareils cibles prennent en charge le profil HEVC sélectionné

## Conclusion

L'encodage HEVC accéléré par le matériel offre des avantages de performance significatifs pour les applications .NET de traitement vidéo. En tirant parti d'AMD AMF, NVIDIA NVENC ou Intel QuickSync via les SDK VisioForge, vous pouvez atteindre un équilibre optimal entre qualité, vitesse et efficacité.

Choisissez le bon encodeur et les bons paramètres en fonction de vos exigences spécifiques, votre public cible et votre type de contenu pour offrir la meilleure expérience possible dans vos applications.

Commencez par détecter les encodeurs matériels disponibles, implémentez des paramètres de qualité appropriés et testez sur divers types de contenu pour garantir des résultats optimaux.
