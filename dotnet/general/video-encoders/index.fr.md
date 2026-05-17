---
title: Encodeurs vidéo pour VisioForge .NET — H264, HEVC, AV1
description: Vue d'ensemble des encodeurs vidéo avec accélération matérielle, fonctionnalités codec et optimisation des performances pour applications vidéo .NET.
sidebar_label: Encodeurs vidéo

order: 19
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - NVENCH264EncoderSettings
  - NVENCHEVCEncoderSettings
  - AMFH264EncoderSettings
  - AMFHEVCEncoderSettings
  - QSVH264EncoderSettings
  - QSVHEVCEncoderSettings
  - OpenH264EncoderSettings
  - AOMAV1EncoderSettings

---

# Encodeurs vidéo pour VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction aux encodeurs vidéo

Les encodeurs vidéo sont des composants essentiels des applications de traitement multimédia, chargés de compresser les données vidéo tout en maintenant une qualité optimale. Les SDK VisioForge .NET intègrent plusieurs encodeurs avancés pour répondre aux divers besoins de développement sur différentes plateformes et cas d'usage.

Ce guide fournit des informations détaillées sur les capacités de chaque encodeur, ses caractéristiques de performance et ses détails d'implémentation pour aider les développeurs .NET à prendre des décisions éclairées pour leurs applications multimédias.

## Encodage matériel vs logiciel

Lors du développement d'applications de traitement vidéo, le choix entre encodeurs matériels et logiciels impacte significativement les performances de l'application et l'expérience utilisateur.

### Encodeurs accélérés par le matériel

Les encodeurs matériels utilisent des unités de traitement dédiées (GPU ou matériel spécialisé) :

- **Avantages** : utilisation CPU réduite, vitesses d'encodage plus élevées, efficacité énergétique améliorée
- **Cas d'usage** : streaming en temps réel, traitement vidéo en direct, applications mobiles
- **Exemples dans notre SDK** : NVIDIA NVENC, AMD AMF, Intel QuickSync

### Encodeurs logiciels

Les encodeurs logiciels s'exécutent sur le CPU sans matériel spécialisé :

- **Avantages** : compatibilité plus large, plus d'options de contrôle de la qualité, indépendance de plateforme
- **Cas d'usage** : encodage hors-ligne haute qualité, environnements sans matériel compatible
- **Exemples dans notre SDK** : OpenH264, encodeur logiciel MJPEG

## Encodeurs vidéo disponibles

Nos SDK fournissent de nombreuses options d'encodeurs pour répondre aux divers besoins des projets :

### Encodeurs H.264 (AVC)

H.264 reste l'un des codecs vidéo les plus utilisés, offrant une excellente efficacité de compression et une large compatibilité.

#### Caractéristiques clés :

- Prise en charge de plusieurs profils (Baseline, Main, High)
- Contrôles de débit ajustables (CBR, VBR, CQP)
- Configuration des B-frames et des images de référence
- Options d'accélération matérielle des principaux fournisseurs

[En savoir plus sur les encodeurs H.264 →](h264.md)

### Encodeurs HEVC (H.265)

HEVC offre une efficacité de compression supérieure à H.264, permettant une vidéo de meilleure qualité au même débit ou une qualité comparable à des débits inférieurs.

#### Caractéristiques clés :

- Compression environ 50 % meilleure que H.264
- Prise en charge de profondeurs de couleur 8 bits et 10 bits
- Plusieurs options d'accélération matérielle
- Mécanismes avancés de contrôle de débit

[En savoir plus sur les encodeurs HEVC →](hevc.md)

### Encodeur AV1

AV1 représente la nouvelle génération de codecs vidéo, offrant une efficacité de compression supérieure particulièrement adaptée au streaming web.

#### Caractéristiques clés :

- Standard ouvert libre de redevances
- Meilleure compression que HEVC
- Prise en charge croissante par les navigateurs et appareils
- Optimisé pour la diffusion de contenu web

[En savoir plus sur l'encodeur AV1 →](av1.md)

### Encodeurs MJPEG

Motion JPEG fournit une compression JPEG image par image, utile pour des applications spécifiques où l'accès individuel aux images est important.

#### Caractéristiques clés :

- Implémentation simple
- Faible latence d'encodage
- Accès indépendant aux images
- Implémentations matérielles et logicielles

[En savoir plus sur les encodeurs MJPEG →](mjpeg.md)

### Encodeurs VP8 et VP9

Ces codecs ouverts développés par Google offrent des alternatives libres de redevances avec une bonne efficacité de compression.

#### Caractéristiques clés :

- Implémentation open source
- Rapport qualité/débit compétitif
- Large prise en charge par les navigateurs web
- Adapté au format de conteneur WebM

[En savoir plus sur les encodeurs VP8/VP9 →](vp8-vp9.md)

### Encodeur Windows Media Video

L'encodeur WMV offre une compatibilité avec l'écosystème Windows et les applications historiques.

#### Caractéristiques clés :

- Intégration native à Windows
- Options de profils multiples
- Compatible avec le framework Windows Media
- Efficace pour les déploiements centrés sur Windows

[En savoir plus sur l'encodeur WMV →](../output-formats/wmv.md)

## Lignes directrices pour la sélection d'encodeur

La sélection de l'encodeur optimal dépend de plusieurs facteurs :

### Compatibilité avec les plateformes

- **Windows** : tous les encodeurs pris en charge
- **macOS** : encodeurs Apple Media, OpenH264, AV1
- **Linux** : VAAPI, OpenH264, implémentations logicielles

### Exigences matérielles

Lors de l'utilisation d'encodeurs accélérés par le matériel, vérifiez la compatibilité du système :

Les vérifications de disponibilité des encodeurs matériels se trouvent sur les classes de paramètres spécifiques au codec (il n'existe pas de type de base partagé `NVENCEncoderSettings` / `AMFEncoderSettings` / `QSVEncoderSettings` — instanciez `NVENCH264EncoderSettings`, `AMFHEVCEncoderSettings`, `QSVH264EncoderSettings`, etc., selon le codec requis) :

```csharp
// Sonder les encodeurs matériels H.264 par ordre de priorité.
if (NVENCH264EncoderSettings.IsAvailable())
{
    // GPU NVIDIA disponible — utiliser NVENC H.264.
    var settings = new NVENCH264EncoderSettings();
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    // CPU Intel avec Quick Sync disponible.
    var settings = new QSVH264EncoderSettings();
}
else if (AMFH264EncoderSettings.IsAvailable())
{
    // GPU AMD disponible — utiliser AMF H.264.
    var settings = new AMFH264EncoderSettings();
}
else
{
    // Solution de repli vers l'encodeur logiciel (multiplateforme).
    var settings = new OpenH264EncoderSettings();
}
```

### Compromis qualité/performance

Différents encodeurs offrent différents équilibres entre qualité et vitesse d'encodage :

| Type d'encodeur | Qualité | Performances | Utilisation CPU |
|--------------|---------|-------------|-----------|
| NVENC H.264 | Bonne | Excellente | Très faible |
| NVENC HEVC | Très bonne | Très bonnes | Très faible |
| AMF H.264 | Bonne | Très bonnes | Très faible |
| QSV H.264 | Bonne | Excellente | Très faible |
| OpenH264 | Bonne-Excellente | Modérée | Élevée |
| AV1 | Excellente | Médiocre-Modérée | Très élevée |

### Scénarios d'encodage

- **Streaming en direct** : préférez les encodeurs matériels avec contrôle de débit CBR
- **Enregistrement vidéo** : encodeurs matériels avec VBR pour un meilleur équilibre qualité/taille
- **Traitement hors-ligne** : encodeurs orientés qualité avec VBR ou CQP
- **Applications à faible latence** : encodeurs matériels avec préréglages basse latence

## Optimisation des performances

Maximisez l'efficacité de l'encodeur avec ces bonnes pratiques :

1. **Adaptez la résolution de sortie aux exigences du contenu** — évitez le surdimensionnement inutile
2. **Sélectionnez des débits appropriés** — plus n'est pas toujours mieux ; visez votre support de diffusion
3. **Choisissez judicieusement les préréglages d'encodeur** — les préréglages plus rapides utilisent moins de CPU mais peuvent réduire la qualité
4. **Activez la détection de scène** pour une meilleure qualité aux changements de scène
5. **Utilisez l'accélération matérielle** lorsqu'elle est disponible pour les applications en temps réel

## Conclusion

Les SDK VisioForge .NET fournissent un ensemble complet d'encodeurs vidéo pour répondre aux divers besoins sur différentes plateformes et cas d'usage. En comprenant les forces et les configurations de chaque encodeur, les développeurs peuvent créer des applications vidéo haute performance avec une qualité et une efficacité optimales.

Pour les détails de configuration spécifiques aux encodeurs, consultez les pages de documentation dédiées à chaque type d'encodeur, liées tout au long de ce guide.
