---
title: SDK vidéo Delphi et ActiveX — Lecture, capture, édition
description: Composants VCL et contrôles ActiveX pour la lecture vidéo, la capture, l'enregistrement d'écran et l'édition vidéo en Delphi et C++ Builder.
sidebar_label: All-in-One Media Framework (Delphi/ActiveX)
order: 18
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Streaming
  - Editing
primary_api_classes:
  - TVFMediaPlayer
  - TVFVideoCapture
  - TVFVideoEdit

---

# All-in-One Media Framework

Un ensemble de bibliothèques Delphi/ActiveX pour le traitement vidéo, la lecture et la capture, appelé All-in-One Media Framework. Ces bibliothèques aident les développeurs à créer des applications professionnelles d'édition, de lecture et de capture vidéo avec un minimum d'effort et un maximum de performance.

Le framework fournit une solution complète pour la gestion des médias dans les applications Delphi, offrant des capacités de traitement vidéo haute performance qui nécessiteraient autrement une programmation bas niveau étendue. Les développeurs peuvent implémenter des flux de travail vidéo complexes grâce à une architecture simple basée sur des composants.

Vous trouverez ici la documentation des bibliothèques suivantes :

## Bibliothèques

- [TVFMediaPlayer](mediaplayer/index.md) — Composant lecteur multimédia complet avec prise en charge des listes de lecture, positionnement précis à l'image et contrôles de lecture avancés
- [TVFVideoCapture](videocapture/index.md) — Puissant composant de capture vidéo prenant en charge les webcams, cartes de capture, caméras IP et enregistrement d'écran
- [TVFVideoEdit](videoedit/index.md) — Composant d'édition vidéo professionnel avec prise en charge de la timeline, transitions, filtres et export vers de nombreux formats

## Exemples d'implémentation

Le framework inclut de nombreux exemples montrant comment implémenter des tâches multimédias courantes :

- Lecteurs vidéo avec contrôles et visualisations personnalisés
- Applications d'enregistrement multi-caméras
- Logiciels d'édition vidéo avec prise en charge de la timeline
- Utilitaires de conversion de format
- Applications de streaming multimédia

## Informations générales

Les paquets ActiveX peuvent être utilisés dans de nombreux langages de programmation et environnements de développement, notamment Visual C++, Visual Basic et C++ Builder. Ces composants étendent les capacités de vos logiciels, accélèrent le développement et améliorent les performances. Grâce à l'intégration ActiveX, vous pouvez incorporer des composants logiciels existants dans vos projets pour gagner en efficacité et en fonctionnalités.

Notre framework est compatible avec toutes les versions de Delphi, de Delphi 6 à Delphi 11 et au-delà, ce qui le rend adapté aux projets existants comme au nouveau développement. Les composants conservent une API cohérente entre les différentes versions de Delphi, ce qui simplifie la migration entre versions de l'IDE.

## Spécifications techniques

- **Formats multimédias pris en charge** : MP4, AVI, MOV, MKV, MPEG, WMV et bien d'autres
- **Prise en charge audio** : AAC, MP3, PCM, WMA et d'autres codecs audio populaires
- **Codecs vidéo** : H.264, H.265/HEVC, MPEG-4, VP9, AV1 et plus encore
- **Sources de capture** : webcams, cartes de capture HDMI, caméras IP, capture d'écran
- **Accélération matérielle** : NVIDIA NVENC, Intel Quick Sync, AMD AMF

## Limitations de la prise en charge x64

Avec Delphi XE2 et versions ultérieures, vous pouvez développer des applications 64 bits. Notre framework prend pleinement en charge ces applications 64 bits, vous permettant de tirer parti de la puissance de calcul moderne et de répondre à des besoins mémoire plus importants. La prise en charge 64 bits permet de traiter des vidéos en plus haute résolution et des opérations d'édition plus complexes, qui seraient impossibles en environnement 32 bits.

Microsoft Visual Basic 6 ne prend pas en charge les applications 64 bits. Si vous utilisez Visual Basic 6, vous devrez utiliser la version 32 bits de notre framework, en raison des limitations inhérentes à VB6. Bien que les applications 32 bits puissent accéder jusqu'à 4 Go de mémoire avec une configuration appropriée, pour les applications vidéo exigeantes nous recommandons d'utiliser Delphi ou un autre environnement de développement prenant en charge le 64 bits.

## Bonnes pratiques de développement

Lors de l'intégration du framework dans vos applications, tenez compte de ces bonnes pratiques :

- Initialisez les composants au moment de la conception lorsque c'est possible, pour une meilleure intégration avec l'IDE
- Utilisez l'accélération matérielle pour les opérations exigeantes comme l'encodage et le décodage
- Mettez en place une gestion d'erreurs appropriée pour les opérations multimédias
- Tenez compte de la gestion mémoire pour les fichiers multimédias volumineux
- Testez avec différentes sources multimédias pour garantir la compatibilité

---

Pour plus d'informations sur le framework, consultez la page produit [All-in-One Media Framework (Delphi/ActiveX)](https://www.visioforge.com/all-in-one-media-framework).
