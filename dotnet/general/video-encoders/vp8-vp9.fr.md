---
title: Encodage vidéo VP8 et VP9 — configuration et réglage
description: Configurez les encodeurs vidéo VP8 et VP9 pour des performances optimales de streaming, d'enregistrement et de traitement dans VisioForge .NET.
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
  - WebM
  - VP8
  - VP9
  - C#
primary_api_classes:
  - VP9EncoderSettings
  - VP8EncoderSettings
  - WebMOutput
  - QSVVP9EncoderSettings
  - WebMVideoEncoder

---

# Encodeurs VP8 et VP9 dans VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce guide vous montre comment implémenter l'encodage vidéo VP8 et VP9 dans les SDK VisioForge .NET. Vous découvrirez les options d'encodeur disponibles et comment les optimiser pour les besoins spécifiques de votre application.

## Vue d'ensemble des options d'encodeur

Le SDK VisioForge fournit plusieurs implémentations d'encodeur en fonction de vos exigences de plateforme :

### Encodeurs pour plateforme Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

- Encodeurs logiciels VP8 et VP9 configurés via la classe [WebMOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WebMOutput.html)

### Options multiplateformes du moteur X

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

- Encodeur logiciel VP8 via [VP8EncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.VP8EncoderSettings.html)
- Encodeur logiciel VP9 via [VP9EncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.VP9EncoderSettings.html)
- Encodeur VP9 GPU Intel accéléré par le matériel via [QSVVP9EncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.QSVVP9EncoderSettings.html) pour les GPU intégrés

## Stratégies de contrôle de débit

Tous les encodeurs VP8 et VP9 prennent en charge différents modes de contrôle de débit pour correspondre aux exigences de votre application :

### Débit constant (CBR)

CBR maintient un débit constant tout au long du processus d'encodage, ce qui le rend idéal pour :

- Applications de streaming en direct
- Scénarios avec limitations de bande passante
- Communication vidéo en temps réel

**Exemples d'implémentation :**

Avec `WebMOutput` (Windows) :

```csharp
var webmOutput = new WebMOutput();
webmOutput.Video_EndUsage = VP8EndUsageMode.CBR;
webmOutput.Video_Encoder = WebMVideoEncoder.VP8;
webmOutput.Video_Bitrate = 2000;  // 2 Mbps
```

Avec `VP8EncoderSettings` :

```csharp
var vp8 = new VP8EncoderSettings();
vp8.RateControl = VPXRateControl.CBR;
vp8.TargetBitrate = 2000;  // 2 Mbps
```

Avec `VP9EncoderSettings` :

```csharp
var vp9 = new VP9EncoderSettings();
vp9.RateControl = VPXRateControl.CBR;
vp9.TargetBitrate = 2000;  // 2 Mbps
```

Avec l'encodeur GPU Intel :

```csharp
var vp9qsv = new QSVVP9EncoderSettings();
vp9qsv.RateControl = QSVVP9EncRateControl.CBR;
vp9qsv.Bitrate = 2000;  // 2 Mbps
```

### Débit variable (VBR)

VBR ajuste dynamiquement le débit selon la complexité du contenu, optimal pour :

- Encodage vidéo non en direct
- Scénarios privilégiant la qualité visuelle sur la taille du fichier
- Contenu avec complexité visuelle variable

**Exemples d'implémentation :**

Avec `WebMOutput` (Windows) :

```csharp
var webmOutput = new WebMOutput();
webmOutput.Video_EndUsage = VP8EndUsageMode.VBR;
webmOutput.Video_Encoder = WebMVideoEncoder.VP8;
webmOutput.Video_Bitrate = 3000;  // Cible 3 Mbps
```

Avec `VP8EncoderSettings` :

```csharp
var vp8 = new VP8EncoderSettings();
vp8.RateControl = VPXRateControl.VBR;
vp8.TargetBitrate = 3000;
```

Avec `VP9EncoderSettings` :

```csharp
var vp9 = new VP9EncoderSettings();
vp9.RateControl = VPXRateControl.VBR;
vp9.TargetBitrate = 3000;
```

Avec l'encodeur GPU Intel :

```csharp
var vp9qsv = new QSVVP9EncoderSettings();
vp9qsv.RateControl = QSVVP9EncRateControl.VBR;
vp9qsv.Bitrate = 3000;
```

## Modes d'encodage orientés qualité

Ces modes privilégient une qualité visuelle constante par rapport à des cibles de débit spécifiques :

### Mode qualité constante (CQ)

Disponible pour les encodeurs logiciels VP8 et VP9 :

```csharp
var vp8 = new VP8EncoderSettings();
vp8.RateControl = VPXRateControl.CQ;
vp8.CQLevel = 20;  // Niveau de qualité (0-63, valeurs plus basses = meilleure qualité)
```

```csharp
var vp9 = new VP9EncoderSettings();
vp9.RateControl = VPXRateControl.CQ;
vp9.CQLevel = 20;
```

### Modes qualité Intel QSV

L'encodeur matériel d'Intel prend en charge deux modes orientés qualité :

**Qualité constante intelligente (ICQ) :**

```csharp
var vp9qsv = new QSVVP9EncoderSettings();
vp9qsv.RateControl = QSVVP9EncRateControl.ICQ;
vp9qsv.ICQQuality = 25;  // 20-27 recommandé pour qualité équilibrée
```

**Paramètre de quantification constant (CQP) :**

```csharp
var vp9qsv = new QSVVP9EncoderSettings();
vp9qsv.RateControl = QSVVP9EncRateControl.CQP;
vp9qsv.QPI = 26;  // QP des I-frames
vp9qsv.QPP = 28;  // QP des P-frames
```

## Optimisation des performances VP9

Les encodeurs VP9 offrent des fonctionnalités supplémentaires pour des performances accrues :

### Quantification adaptative

Améliore la qualité visuelle en allouant plus de bits aux zones complexes :

```csharp
var vp9 = new VP9EncoderSettings();
vp9.AQMode = VPXAdaptiveQuantizationMode.Variance;  // Activer l'AQ basée sur la variance
```

### Traitement parallèle

Accélère l'encodage par le multithreading et le traitement en tuiles :

```csharp
var vp9 = new VP9EncoderSettings();
vp9.FrameParallelDecoding = true;  // Activer le traitement parallèle d'images
vp9.RowMultithread = true;         // Activer le multithreading par ligne
vp9.TileColumns = 6;               // Définir le nombre de colonnes de tuiles (log2)
vp9.TileRows = 0;                  // Définir le nombre de lignes de tuiles (log2)
```

## Paramètres de résilience aux erreurs

VP8 et VP9 prennent tous deux en charge la résilience aux erreurs pour un streaming robuste sur des réseaux peu fiables :

En utilisant `WebMOutput` (Windows) :

```csharp
var webmOutput = new WebMOutput();
webmOutput.Video_ErrorResilient = true;  // Activer la résilience aux erreurs
```

En utilisant les encodeurs logiciels :

```csharp
var vpx = new VP8EncoderSettings();  // ou VP9EncoderSettings
vpx.ErrorResilient = VPXErrorResilientFlags.Default | VPXErrorResilientFlags.Partitions;
```

## Options de réglage des performances

Optimisez les performances d'encodage avec ces paramètres :

```csharp
var vpx = new VP8EncoderSettings();  // ou VP9EncoderSettings
vpx.CPUUsed = 0;           // Plage : -16 à 16, valeurs plus élevées favorisent la vitesse sur la qualité
vpx.NumOfThreads = 4;      // Spécifier le nombre de threads d'encodage
vpx.TokenPartitions = VPXTokenPartitions.Eight;  // Activer le traitement parallèle de jetons
```

## Bonnes pratiques pour l'encodage VP8/VP9

### Sélection du contrôle de débit

Choisissez le mode de contrôle de débit approprié selon votre application :

- **CBR** pour le streaming en direct et la communication en temps réel
- **VBR** pour l'encodage hors-ligne où la qualité est la priorité
- **Modes basés sur la qualité** (CQ, ICQ, CQP) pour la meilleure qualité possible quelle que soit le débit

### Optimisation des performances

- Ajustez `CPUUsed` pour équilibrer qualité et vitesse d'encodage
- Activez le multithreading pour un encodage plus rapide sur les systèmes multi-cœurs
- Utilisez le parallélisme en tuiles dans VP9 pour une meilleure utilisation du matériel

### Récupération d'erreur

- Activez la résilience aux erreurs lors du streaming sur des réseaux peu fiables
- Configurez le partitionnement de jetons pour une meilleure récupération d'erreur
- Tenez compte des limites de réordonnancement d'images pour les applications à basse latence

### Optimisation de la qualité

- Utilisez la quantification adaptative dans VP9 pour une meilleure distribution de la qualité
- Envisagez l'encodage à deux passes pour les scénarios d'encodage hors-ligne
- Ajustez les paramètres de quantificateur selon le type de contenu et la qualité cible

En suivant ce guide, vous pourrez implémenter et configurer efficacement les encodeurs VP8 et VP9 dans vos applications VisioForge .NET pour des performances et une qualité optimales.
