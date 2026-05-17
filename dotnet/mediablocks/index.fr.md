---
title: API de pipeline multimédia modulaire pour C# .NET VisioForge
description: Pipelines multimédias personnalisés en C# avec VisioForge Media Blocks. Connectez source, traitement et sortie pour transcodage, capture et streaming.
sidebar_label: Media Blocks SDK .NET
order: 14
tags:
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Streaming

---

# Media Blocks SDK pour C# .NET — API de pipeline multimédia modulaire

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Media Blocks SDK pour .NET est un framework multimédia basé sur des pipelines qui vous permet de construire des flux de traitement vidéo et audio personnalisés en C#. Au lieu d'utiliser une API fixe avec un comportement prédéfini, vous créez un pipeline en connectant des blocs typés — sources, encodeurs, effets, moteurs de rendu et puits — afin de construire exactement la chaîne de traitement dont votre application a besoin.

Le SDK fonctionne sous Windows, macOS, Linux, Android et iOS. Il prend en charge des cas d'usage que les SDK de plus haut niveau Video Capture SDK et Media Player SDK ne peuvent pas couvrir : composition multi-source, pipelines de transcodage personnalisés, encodage simultané vers plusieurs formats, chaînes d'effets vidéo en temps réel et intégration avec du matériel comme Blackmagic Decklink ou des caméras industrielles.

## Quand utiliser Media Blocks

Media Blocks SDK est le bon choix lorsque vous avez besoin d'un contrôle total sur le pipeline multimédia. Utilisez-le à la place (ou en complément) des autres SDK VisioForge lorsque :

| Scénario | SDK recommandé |
| -------- | --------------- |
| Enregistrement simple de webcam vers MP4 | [Video Capture SDK](../videocapture/index.md) |
| Lire un fichier vidéo avec des contrôles standard | [Media Player SDK](../mediaplayer/index.md) |
| Pipeline personnalisé : source → effets → encodage → sorties multiples | **Media Blocks SDK** |
| Composition vidéo en direct depuis plusieurs sources | **Media Blocks SDK** |
| Transcodage / conversion de format avec traitement personnalisé | **Media Blocks SDK** |
| Enregistrement RTSP avec post-traitement (superposition, redimensionnement, ré-encodage) | **Media Blocks SDK** |
| Application multimédia multiplateforme Avalonia ou MAUI | **Media Blocks SDK** |
| Intégration avec Decklink, GenICam ou matériel NVIDIA | **Media Blocks SDK** |

## Cas d'usage courants

### Transcodage et conversion de format de fichiers vidéo

Convertissez des fichiers vidéo entre formats (par exemple AVI vers MP4, MKV vers WebM) avec un contrôle total sur les codecs, la résolution, le débit binaire et le traitement. Enchaînez des blocs de redimensionnement, de désentrelacement ou de correction colorimétrique entre la source et l'encodeur.

### Capture de caméra personnalisée avec pipeline de traitement

Construisez des flux de capture de caméra qui vont au-delà du simple enregistrement. Insérez des effets en temps réel, des superpositions de texte ou des blocs de vision par ordinateur entre la source caméra et le puits de fichier. Envoyez vers plusieurs destinations simultanément — fenêtre d'aperçu, fichier et flux réseau.

Voir : [Tutoriel d'application de visualisation de caméra](GettingStarted/camera.md)

### Composition et mixage vidéo en direct

Combinez plusieurs sources vidéo en une seule sortie avec le [Compositeur de vidéo en direct](LiveVideoCompositor/index.md). Positionnez, mettez à l'échelle et superposez les flux vidéo pour une production multi-caméra, de l'image dans l'image ou des dispositions de grille de surveillance.

### Enregistrement de flux RTSP avec post-traitement

Capturez les flux RTSP des caméras IP et appliquez un traitement avant la sauvegarde — redimensionnez à une résolution inférieure, ajoutez des superpositions d'horodatage, ré-encodez avec d'autres paramètres de qualité ou découpez en segments.

Voir : [Guide d'enregistrement de flux RTSP](Guides/rtsp-save-original-stream.md) | [Capture ONVIF avec post-traitement](Guides/onvif-capture-with-postprocessing.md)

### Superposition de texte et d'image / filigrane

Ajoutez du texte, des images ou des superpositions SVG à la vidéo en direct ou aux fichiers enregistrés à l'aide du [Bloc de gestion de superposition](VideoProcessing/OverlayManagerBlock.md). Utile pour le filigranage, l'insertion d'horodatage, l'identité visuelle et l'affichage à l'écran.

### Lecture de codes-barres et QR codes depuis la vidéo

Traitez les flux de caméra en direct ou les fichiers vidéo pour détecter et décoder les codes-barres et les QR codes en temps réel.

Voir : [Guide du lecteur de codes-barres et QR codes](Guides/barcode-qr-reader-guide.md)

### Enregistrement pré-événement

Mettez en œuvre un enregistrement à tampon circulaire qui capture la vidéo en continu et écrit sur le disque des clips d'événement (incluant les images antérieures au déclencheur).

Voir : [Guide d'enregistrement pré-événement](Guides/pre-event-recording.md)

## Prise en charge des plateformes

| Plateforme | Frameworks d'interface utilisateur | Notes |
| -------- | ------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Console | Ensemble complet de fonctionnalités, y compris les ponts DirectShow |
| macOS | MAUI, Avalonia, Console | Accès caméra AVFoundation |
| Linux x64 | Avalonia, Console | Caméra V4L2, traitement basé sur GStreamer |
| Android | MAUI | Via l'intégration MAUI |
| iOS | MAUI | Via l'intégration MAUI |

## Composants principaux du SDK

### Sources

Les [Blocs de source](Sources/index.md) ingèrent les médias depuis des caméras, des fichiers, des flux réseau, des générateurs virtuels et du matériel de capture.

### Traitement vidéo

- [Encodeurs vidéo](VideoEncoders/index.md) — H.264, H.265/HEVC, VP8, VP9, AV1 avec accélération GPU (NVENC, AMF, Quick Sync)
- [Décodeurs vidéo](VideoDecoders/index.md) — Décodage matériel et logiciel pour tous les codecs majeurs
- [Traitement vidéo](VideoProcessing/index.md) — Redimensionnement, rognage, rotation, correction colorimétrique, désentrelacement, effets
- [Rendu vidéo](VideoRendering/index.md) — Afficher la vidéo dans des contrôles WinForms, WPF, MAUI et Avalonia
- [Compositeur de vidéo en direct](LiveVideoCompositor/index.md) — Mixage et composition multi-source

### Traitement audio

- [Encodeurs audio](AudioEncoders/index.md) — Encodage AAC, MP3, Vorbis, Opus, FLAC
- [Traitement audio](AudioProcessing/index.md) — Filtres, effets, conversion de fréquence d'échantillonnage, mixage de canaux
- [Rendu audio](AudioRendering/index.md) — Lecture vers les périphériques audio du système
- [Visualiseurs audio](AudioVisualizers/index.md) — Blocs de visualisation de forme d'onde et de spectre

### Sortie et connectivité

- [Puits](Sinks/index.md) — Écriture dans des fichiers MP4, WebM, AVI, MKV, TS et des flux réseau
- [Blocs de sortie](Outputs/index.md) — Configurations de sortie de haut niveau
- [Ponts](Bridge/index.md) — Connectez des segments de pipeline et synchronisez les blocs
- [Démultiplexeurs](Demuxers/index.md) et [Analyseurs](Parsers/index.md) — Démultiplexage et analyse des flux

### Matériel et spécificités de plateforme

- [NVIDIA](Nvidia/index.md) — Blocs d'accélération matérielle NVENC/NVDEC
- [Blackmagic Decklink](Decklink/index.md) — Matériel professionnel de capture et de lecture
- [OpenCV](OpenCV/index.md) — Intégration de la vision par ordinateur
- [OpenGL](OpenGL/index.md) — Traitement vidéo basé sur le GPU
- [AWS](AWS/index.md) — Blocs d'intégration cloud
- [Serveur RTSP](RTSPServer/index.md) — Diffuser la vidéo sous forme de flux RTSP

## Prise en main

Prêt à construire votre premier pipeline ? Le guide de démarrage rapide pour développeurs couvre l'installation, les concepts fondamentaux, l'architecture du pipeline et des exemples de code pas à pas :

[Guide de démarrage rapide pour développeurs](GettingStarted/index.md)

Tutoriels supplémentaires de prise en main :

- [Implémentation complète d'un pipeline](GettingStarted/pipeline.md)
- [Développement d'un lecteur multimédia](GettingStarted/player.md)
- [Application de visualisation de caméra](GettingStarted/camera.md)
- [Énumération des périphériques](GettingStarted/device-enum.md)

## Guides

- [Enregistrer un flux RTSP dans un fichier](Guides/rtsp-save-original-stream.md)
- [Visualisateur de flux RTSP et lecteur de caméras IP](Guides/rtsp-player-csharp.md)
- [Capture ONVIF avec post-traitement](Guides/onvif-capture-with-postprocessing.md)
- [Lecteur de codes-barres et QR codes](Guides/barcode-qr-reader-guide.md)
- [Enregistrement pré-événement](Guides/pre-event-recording.md)
- [Étiquettes de métadonnées audio](Guides/audio-metadata-tags.md)
- [Effets vidéo personnalisés et shaders OpenGL](Guides/custom-video-effects-csharp.md)

## Ressources pour développeurs

- [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)
- [Guide de déploiement](../deployment-x/index.md)
- [Référence de l'API](https://api.visioforge.org/dotnet/api/index.html)
- [Journal des modifications](../changelog.md)
- [Contrat de licence utilisateur final](../../eula.md)
- [Informations de licence](../../../licensing.md)
