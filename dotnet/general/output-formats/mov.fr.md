---
title: Encodage et sortie de fichier MOV dans les apps vidéo .NET
description: Générez des fichiers MOV en .NET avec encodage accéléré matériellement, prise en charge multiplateforme et options audio/vidéo professionnelles.
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
  - Encoding
  - Editing
  - MP4
  - MOV
  - H.264
  - H.265
  - AAC
  - MP3
  - C#
primary_api_classes:
  - MOVOutput
  - NVENCH264EncoderSettings
  - VOAACEncoderSettings
  - VideoCaptureCoreX
  - VideoEditCoreX

---

# Sortie de fichier MOV pour les applications vidéo .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

## Introduction à la sortie MOV dans VisioForge

Le format de conteneur MOV est largement utilisé pour le stockage vidéo dans les environnements professionnels et les écosystèmes Apple. Les SDK .NET de VisioForge offrent une prise en charge multiplateforme robuste pour générer des fichiers MOV avec des options d'encodage personnalisables. La classe `MOVOutput` sert d'interface principale pour configurer et générer ces fichiers sur les environnements Windows, macOS et Linux.

Les fichiers MOV créés avec les SDK VisioForge peuvent tirer parti de l'accélération matérielle via des encodeurs NVIDIA, Intel et AMD, ce qui les rend idéaux pour les applications critiques en termes de performance. Ce guide vous accompagne à travers les étapes essentielles pour implémenter la sortie MOV dans les applications vidéo .NET.

### Quand utiliser le format MOV

MOV est particulièrement bien adapté à :

- Les flux de travail d'édition vidéo
- Les projets nécessitant la compatibilité avec l'écosystème Apple
- Les pipelines de production vidéo professionnels
- Les applications nécessitant la préservation des métadonnées
- L'archivage haute qualité

## Premiers pas avec la sortie MOV

La classe `MOVOutput` ([référence d'API](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MOVOutput.html)) fournit la base pour la génération de fichiers MOV avec les SDK VisioForge. Elle encapsule la configuration des encodeurs vidéo et audio, des paramètres de traitement et des paramètres de puits.

### Implémentation de base

La création d'une sortie MOV nécessite un code minimal :

```csharp
// Créer une sortie MOV ciblant le nom de fichier spécifié
var movOutput = new MOVOutput("output.mov");
```

Cette implémentation simple effectue automatiquement les opérations suivantes :

- Sélectionne l'encodeur NVENC H264 s'il est disponible (se replie sur OpenH264)
- Choisit l'encodeur AAC approprié pour votre plateforme (MF AAC sur Windows, VO-AAC ailleurs)
- Configure les paramètres du conteneur MOV pour une large compatibilité

### Comportement de configuration par défaut

La configuration par défaut offre des performances et une compatibilité équilibrées sur l'ensemble des plateformes. Cependant, pour des cas d'usage spécialisés, vous devrez probablement personnaliser les paramètres d'encodeur, ce que nous aborderons dans les sections suivantes.

## Options d'encodeur vidéo pour les fichiers MOV

La sortie MOV prend en charge une variété d'encodeurs vidéo pour répondre à différentes exigences de performance, de qualité et de compatibilité. Le choix de l'encodeur a un impact significatif sur la vitesse de traitement, la consommation de ressources et la qualité de sortie.

### Encodeurs vidéo pris en charge

La sortie MOV prend en charge ces encodeurs vidéo. Pour des options de configuration détaillées, consultez la [documentation de l'encodeur H.264](../video-encoders/h264.md) et la [documentation de l'encodeur HEVC](../video-encoders/hevc.md) :

| Encodeur | Technologie | Plateforme | Idéal pour |
|---------|------------|----------|----------|
| OpenH264 | Logiciel | Multiplateforme | Compatibilité |
| NVENC H264 | GPU NVIDIA | Multiplateforme | Performance |
| QSV H264 | GPU Intel | Multiplateforme | Efficacité |
| AMF H264 | GPU AMD | Multiplateforme | Performance |
| MF HEVC | Logiciel | Windows uniquement | Qualité |
| NVENC HEVC | GPU NVIDIA | Multiplateforme | Qualité/Performance |
| QSV HEVC | GPU Intel | Multiplateforme | Efficacité |
| AMF H265 | GPU AMD | Multiplateforme | Qualité/Performance |

### Configuration des encodeurs vidéo

Définissez un encodeur vidéo spécifique avec un code comme celui-ci :

```csharp
// Pour l'encodage accéléré matériellement NVIDIA. Bitrate est en Kbit/s sur les encodeurs de l'espace de noms X.
movOutput.Video = new NVENCH264EncoderSettings() {
    Bitrate = 5000,  // Kbit/s — 5 Mbps
};

// Pour l'encodage logiciel avec OpenH264. RateControl est OpenH264RCMode (pas RateControlMode).
movOutput.Video = new OpenH264EncoderSettings() {
    RateControl = OpenH264RCMode.Bitrate,
    Bitrate = 2500,  // Kbit/s — 2.5 Mbps
};
```

### Stratégie de sélection d'encodeur

Lors de l'implémentation de la sortie MOV, prenez en compte ces facteurs pour la sélection d'encodeur :

1. **Disponibilité matérielle** — Vérifiez si l'accélération GPU est disponible
2. **Exigences de qualité** — HEVC offre une meilleure qualité à des débits inférieurs
3. **Vitesse de traitement** — Les encodeurs matériels offrent des avantages de vitesse significatifs
4. **Compatibilité de plateforme** — Certains encodeurs sont spécifiques à Windows

Une approche à plusieurs niveaux fonctionne souvent mieux, en vérifiant l'encodeur disponible le plus rapide et en se repliant au besoin :

```csharp
// Essayer NVIDIA, puis Intel, puis l'encodage logiciel
if (NVENCH264EncoderSettings.IsAvailable())
{
    movOutput.Video = new NVENCH264EncoderSettings();
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    movOutput.Video = new QSVH264EncoderSettings();
}
else
{
    movOutput.Video = new OpenH264EncoderSettings();
}
```

## Options d'encodeur audio

La qualité audio est essentielle pour la plupart des applications vidéo. Le SDK fournit plusieurs encodeurs audio optimisés pour différents cas d'usage.

### Encodeurs audio pris en charge

Pour les détails de configuration des codecs audio, consultez la [documentation de l'encodeur AAC](../audio-encoders/aac.md) et la [documentation de l'encodeur MP3](../audio-encoders/mp3.md) :

| Encodeur | Type | Plateforme | Qualité | Cas d'usage |
|---------|------|----------|---------|----------|
| MP3 | Logiciel | Multiplateforme | Bonne | Distribution Web |
| VO-AAC | Logiciel | Multiplateforme | Excellente | Usage professionnel |
| AVENC AAC | Logiciel | Multiplateforme | Très bonne | Usage général |
| MF AAC | Accéléré matériellement | Windows uniquement | Excellente | Applications Windows |

### Configuration de l'encodeur audio

L'implémentation de l'encodage audio nécessite un code minimal :

```csharp
// Configuration MP3. Bitrate en Kbit/s (8/16/.../320). MP3EncoderSettings expose
// uniquement Bitrate et ForceMono — le nombre de canaux suit la source en amont.
movOutput.Audio = new MP3EncoderSettings() {
    Bitrate = 320,        // Kbit/s — 320 kbps haute qualité
    ForceMono = false,    // Laisser à false (par défaut) pour conserver le stéréo de la source
};

// Ou AAC pour une meilleure qualité (Windows)
movOutput.Audio = new MFAACEncoderSettings() {
    Bitrate = 192,        // Kbit/s — 192 kbps
};

// Implémentation AAC multiplateforme. La fréquence d'échantillonnage suit la source.
movOutput.Audio = new VOAACEncoderSettings() {
    Bitrate = 192,
};
```

### Considérations audio spécifiques à la plateforme

Pour gérer élégamment les différences de plateforme, utilisez la compilation conditionnelle :

```csharp
// Sélectionner l'encodeur approprié en fonction de la plateforme
#if NET_WINDOWS
    movOutput.Audio = new MFAACEncoderSettings();
#else
    movOutput.Audio = new VOAACEncoderSettings();
#endif
```

## Personnalisation avancée de la sortie MOV

Au-delà de la configuration de base, les SDK VisioForge permettent une personnalisation puissante de la sortie MOV via des blocs de traitement multimédia et des paramètres de puits.

### Pipeline de traitement personnalisé

Pour des besoins de traitement vidéo spécialisés, le SDK fournit une intégration de blocs multimédias :

```csharp
// Ajouter un traitement vidéo personnalisé
movOutput.CustomVideoProcessor = new SomeMediaBlock();

// Ajouter un traitement audio personnalisé
movOutput.CustomAudioProcessor = new SomeMediaBlock();
```

### Configuration du puits MOV

Affinez les paramètres du conteneur MOV pour des exigences spécialisées :

```csharp
// Configurer les paramètres du puits
movOutput.Sink.Filename = "new_output.mov";
```

### Détection dynamique d'encodeur

Votre application peut sélectionner intelligemment les encodeurs en fonction des capacités du système :

```csharp
// Obtenir les encodeurs vidéo disponibles
var videoEncoders = movOutput.GetVideoEncoders();

// Obtenir les encodeurs audio disponibles
var audioEncoders = movOutput.GetAudioEncoders();

// Afficher les options disponibles aux utilisateurs ou auto-sélectionner
foreach (var encoder in videoEncoders)
{
    Console.WriteLine($"Available encoder: {encoder.Name}");
}
```

## Intégration avec les composants principaux du SDK VisioForge

La sortie MOV s'intègre parfaitement avec les composants principaux du SDK pour la capture, l'édition et le traitement vidéo.

### Intégration de la capture vidéo

Ajoutez la sortie MOV à un flux de capture :

```csharp
// Créer et configurer le core de capture
var core = new VideoCaptureCoreX();

// Ajouter des périphériques de capture
// ..
// Ajouter la sortie MOV configurée
core.Outputs_Add(movOutput, true);

// Démarrer la capture
await core.StartAsync();
```

### Intégration du Video Edit SDK

Intégrez la sortie MOV dans l'édition vidéo :

```csharp
// Créer le core d'édition et configurer le projet
var core = new VideoEditCoreX();

// Ajouter le fichier d'entrée
// ...

// Définir MOV comme format de sortie
core.Output_Format = movOutput;

// Traiter la vidéo
await core.StartAsync();
```

### Implémentation Media Blocks SDK

Pour un contrôle direct du pipeline multimédia :

```csharp
// Créer les blocs d'encodeur (et non les paramètres bruts — le pipeline consomme des blocs).
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 192 });
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings { Bitrate = 5000 });

// Configurer le puits MOV — MOV et MP4 sont des multiplexeurs différents (qtmux vs mp4mux), utilisez donc MOVSinkBlock.
var movSinkSettings = new MOVSinkSettings("output.mov");
var movSink = new MOVSinkBlock(movSinkSettings);

// Câbler les encodeurs au puits via des entrées dynamiques.
pipeline.Connect(h264Encoder.Output, movSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(aacEncoder.Output, movSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Notes de compatibilité des plateformes

Bien que l'implémentation MOV de VisioForge soit multiplateforme, certaines fonctionnalités sont spécifiques à une plateforme :

### Fonctionnalités spécifiques à Windows

- L'encodeur vidéo MF HEVC fournit un encodage optimisé sur Windows
- L'encodeur audio MF AAC offre une accélération matérielle sur les systèmes compatibles

### Fonctionnalités multiplateformes

- Les encodeurs OpenH264, NVENC, QSV et AMF fonctionnent sur l'ensemble des systèmes d'exploitation
- VO-AAC et AVENC AAC fournissent un encodage audio cohérent partout

## Conclusion

La capacité de sortie MOV des SDK VisioForge .NET fournit une solution puissante et flexible pour créer des fichiers vidéo de haute qualité. En tirant parti de l'accélération matérielle lorsqu'elle est disponible et en se repliant sur des implémentations logicielles optimisées au besoin, le SDK garantit d'excellentes performances sur l'ensemble des plateformes.

Pour plus d'informations, consultez la [documentation de l'API VisioForge](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MOVOutput.html) ou explorez d'autres formats de sortie dans notre documentation.
