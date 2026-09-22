---
title: Déploiement Windows du SDK multiplateforme VisioForge
description: Déploiement du SDK VisioForge .NET pour Windows avec paquets NuGet, dépendances et configuration des architectures x86/x64 pour applications multimédias.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - USB3 Vision / GigE
  - NuGet
primary_api_classes:
  - AWSS3SinkBlock
  - CVDewarpBlock
  - CVDilateBlock
  - CVErodeBlock

---

# Guide d'installation et de déploiement Windows pour le SDK multiplateforme VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à l'installation et au déploiement du SDK VisioForge

La suite SDK VisioForge offre de puissantes capacités multimédias pour vos applications .NET, prenant en charge la capture vidéo, l'édition, la lecture et le traitement multimédia avancé sur plusieurs plateformes. Ce guide complet couvre à la fois l'installation et le déploiement pour les applications Windows.

## Installation

Les SDK sont accessibles sous deux formes : un fichier d'installation et des paquets NuGet. Le fichier d'installation offre un processus d'installation simple, garantissant que tous les composants nécessaires sont correctement configurés. Les paquets NuGet, quant à eux, proposent une approche flexible et modulaire pour intégrer les SDK dans vos projets, permettant des mises à jour faciles et une gestion des dépendances. Nous recommandons vivement l'utilisation des paquets NuGet en raison de leur commodité et de leur efficacité dans la gestion des dépendances et des mises à jour des projets.

Lors de la création de votre application, vous avez la possibilité de produire des versions x86 et x64. Cela permet à votre application de fonctionner sur une plus large gamme de systèmes, en accommodant différentes architectures matérielles. Il est toutefois important de noter que le fichier d'installation est exclusivement disponible pour l'architecture x64. Cela signifie que, bien que vous puissiez développer et compiler des builds x86, le processus initial d'installation et de configuration nécessitera un système x64.

### IDE

Pour le développement, vous pouvez utiliser des environnements de développement intégrés (IDE) puissants comme JetBrains Rider ou Visual Studio. Ces deux IDE offrent des outils et des fonctionnalités robustes pour rationaliser le processus de développement sous Windows. Pour garantir une configuration fluide, consultez les guides d'installation correspondants. La [page d'installation de Rider](../install/rider.md) fournit des instructions détaillées pour configurer JetBrains Rider, tandis que la [page d'installation de Visual Studio](../install/visual-studio.md) propose des conseils complets sur l'installation et la configuration de Visual Studio. Ces ressources vous aideront à démarrer rapidement et efficacement, en tirant parti des pleines capacités de ces environnements de développement.

## Distribution et gestion des paquets

Les composants du SDK VisioForge pour Windows sont distribués sous forme de paquets NuGet, ce qui rend l'intégration simple avec les environnements de développement .NET modernes. Vous pouvez ajouter ces paquets à votre projet à l'aide de l'un des outils suivants :

- Gestionnaire de paquets de Visual Studio
- Gestionnaire NuGet de JetBrains Rider
- Visual Studio Code avec les extensions NuGet
- Intégration directe en ligne de commande à l'aide de la CLI .NET

## Paquets de base requis

Chaque application Windows créée avec le SDK VisioForge requiert le paquet de base approprié selon l'architecture cible de votre application. Ces paquets contiennent les composants essentiels au fonctionnement du SDK.

### Paquets de plateforme principaux

- [VisioForge.CrossPlatform.Core.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x86) — pour les applications Windows 32 bits
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64) — pour les applications Windows 64 bits

> **Note** : pour les applications ciblant plusieurs architectures, vous devez inclure les deux paquets et implémenter une logique de sélection appropriée à l'exécution.

## Paquets de composants optionnels

Selon les exigences de votre application, vous pouvez avoir besoin d'inclure des paquets supplémentaires pour des fonctionnalités spécialisées. Ces composants optionnels étendent les capacités du SDK dans divers domaines.

### Intel Quick Sync Video (QSV)

Les encodeurs et décodeurs matériels QSV (`QSVH264EncoderSettings`, `QSVHEVCEncoderSettings`, `QSVAV1EncoderSettings` et les autres réglages `QSV*`) sont livrés dans un paquet séparé. Sans lui, la création d'un encodeur QSV échoue avec `Unable to create H264 encoder 'qsvh264enc'` :

- [VisioForge.CrossPlatform.Core.Windows.Intel.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Intel.x86) — Intel QSV pour les applications 32 bits
- [VisioForge.CrossPlatform.Core.Windows.Intel.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Intel.x64) — Intel QSV pour les applications 64 bits

### Traitement multimédia FFMPEG (recommandé)

Ces paquets fournissent une prise en charge complète des codecs pour une large gamme de formats multimédias grâce à l'intégration de la bibliothèque FFMPEG :

- [VisioForge.CrossPlatform.Libav.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86) — prise en charge FFMPEG 32 bits
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64) — prise en charge FFMPEG 64 bits

Pour les applications avec des contraintes de taille, des versions compressées de ces paquets utilisant la compression UPX sont disponibles :

- [VisioForge.CrossPlatform.Libav.Windows.x86.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86.UPX) — prise en charge FFMPEG 32 bits compressée
- [VisioForge.CrossPlatform.Libav.Windows.x64.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64.UPX) — prise en charge FFMPEG 64 bits compressée

### Intégration cloud — Amazon Web Services

Pour les applications nécessitant une intégration de stockage cloud avec AWS S3 :

- [VisioForge.CrossPlatform.AWS.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x86) — prise en charge AWS 32 bits
- [VisioForge.CrossPlatform.AWS.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x64) — prise en charge AWS 64 bits

Lorsque ces paquets sont utilisés, le Media Block suivant devient disponible :

- `AWSS3SinkBlock` — pour le stockage de fichiers multimédias dans des buckets S3

### Vision par ordinateur avec OpenCV

Pour les applications nécessitant des capacités avancées de traitement d'image et de vision par ordinateur :

- [VisioForge.CrossPlatform.OpenCV.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.Windows.x86) — prise en charge OpenCV 32 bits
- [VisioForge.CrossPlatform.OpenCV.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.Windows.x64) — prise en charge OpenCV 64 bits

L'intégration OpenCV donne accès à des Media Blocks dans l'espace de noms `VisioForge.Core.MediaBlocks.OpenCV`, notamment :

- Transformation d'image : `CVDewarpBlock`, `CVDilateBlock`, `CVErodeBlock`
- Détection de contours et de caractéristiques : `CVEdgeDetectBlock`, `CVLaplaceBlock`, `CVSobelBlock`
- Traitement de visages : `CVFaceBlurBlock`, `CVFaceDetectBlock`
- Détection de mouvement : `CVMotionCellsBlock`
- Reconnaissance d'objets : `CVTemplateMatchBlock`, `CVHandDetectBlock`
- Amélioration d'image : `CVEqualizeHistogramBlock`, `CVSmoothBlock`
- Suivi et superposition : `CVTrackerBlock`, `CVTextOverlayBlock`

## Paquets de prise en charge de matériel spécialisé

Le SDK VisioForge fournit l'intégration avec des systèmes de caméras professionnelles et du matériel spécialisé. Incluez le paquet approprié lorsque vous travaillez avec des types d'appareils spécifiques.

### Caméras Allied Vision

Pour l'intégration avec du matériel de caméras professionnelles Allied Vision :

- [VisioForge.CrossPlatform.AlliedVision.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AlliedVision.Windows.x64)

### Caméras Basler

Pour les applications utilisant des caméras industrielles Basler :

- [VisioForge.CrossPlatform.Basler.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Basler.Windows.x64)

### Caméras Teledyne/FLIR (SDK Spinnaker)

Pour l'imagerie thermique et les caméras FLIR spécialisées :

- [VisioForge.CrossPlatform.Spinnaker.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Spinnaker.Windows.x64)

### Prise en charge du protocole GenICam (GigE/USB3 Vision)

Pour les caméras utilisant le protocole standardisé GenICam :

- [VisioForge.CrossPlatform.GenICam.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.GenICam.Windows.x64)

## Bonnes pratiques de déploiement

Lors du déploiement d'applications basées sur VisioForge pour Windows, prenez en compte ces recommandations :

1. Choisissez les paquets d'architecture appropriés (x86 ou x64) en fonction de votre plateforme cible
2. Incluez les paquets FFMPEG pour une prise en charge complète des formats multimédias
3. N'incluez les paquets de matériel spécialisé que lorsque cela est nécessaire afin de minimiser la taille de déploiement
4. Pour les applications sensibles à la sécurité, envisagez d'utiliser les versions compressées UPX pour obfusquer les bibliothèques natives
5. Testez toujours votre déploiement sur un système propre pour vous assurer que toutes les dépendances sont correctement résolues

## Le cache partagé des plugins natifs

Au démarrage, le SDK ne charge pas ses bibliothèques GStreamer directement depuis le dossier de sortie de
votre application. Il recopie le sous-dossier `x64`, `x86` ou `arm64` de ce dossier vers un emplacement
partagé et charge depuis celui-ci :

```text
%PUBLIC%\.gstreamer\<version-du-sdk>-<jeu-natif>\<architecture>
```

GStreamer tient un registre de plugins qui note chaque plugin par son **chemin absolu**. Un dossier
canonique unique permet donc à toutes les applications VisioForge de la machine de partager un même
registre déjà chaud : seul le premier démarrage après une mise à jour du SDK paie le coût de l'analyse de
l'ensemble des plugins, et tout démarrage ultérieur de n'importe quelle application le réutilise. Ce
dossier reste par ailleurs accessible en écriture, ce que le répertoire d'une application installée sous
`Program Files` n'est pas.

Le dossier est déterminé par la version du SDK **et** par une empreinte des redistribuables core et Libav que
l'application déploie réellement - les paquets `VisioForge.CrossPlatform.*` sont versionnés indépendamment du
SDK managé - si bien que des applications portant des redistribuables différents ne s'écrasent plus mutuellement
leurs bibliothèques à chaque démarrage. Deux déploiements ne différant que par un paquet qu'aucune des deux
empreintes ne couvre (OpenCV, AWS, celui d'un fabricant de caméras) partagent bien un dossier ; les noms de
fichiers de ces paquets sont disjoints du reste, l'effet est donc que quelques fichiers sont recopiés, non
qu'une mauvaise bibliothèque soit chargée. Un cache créé par
le SDK et qui n'a pas été démarré depuis 30 jours est supprimé ; un dossier laissé par un SDK antérieur
utilisant l'ancienne disposition partagée est laissé intact, car les applications construites avec cette
version y chargent toujours.

Deux conséquences à connaître lorsque vous testez un déploiement :

- Le cache accumule les bibliothèques de toutes les applications de cette version du SDK sur la machine. Un
  poste de développement ayant exécuté un jeu de paquets plus complet peut donc satisfaire une dépendance
  que votre application ne distribue pas réellement — et cette même application échoue ensuite sur une
  machine vierge.
- Supprimer `%PUBLIC%\.gstreamer` est sans danger à tout moment ; le démarrage suivant le reconstruit.

Pour écarter le cache partagé et charger exactement ce que votre application a déployé, faites pointer
`VisioForgeX.CacheFolder` vers votre propre dossier natif **avant** d'appeler `InitSDK` :

```csharp
VisioForgeX.CacheFolder = Path.Combine(AppContext.BaseDirectory, "x64");
VisioForgeX.InitSDK();
```

C'est le réglage à retenir pour vérifier qu'un redistribuable est complet, et pour une application qui ne
doit rien écrire en dehors de son propre répertoire. Le coût est que l'application ne partage plus le
registre de la machine et analyse elle-même l'ensemble des plugins à chaque premier démarrage.

Les applications MAUI, WinUI 3 et Unity chargent toujours sur place et n'utilisent jamais le cache partagé.

## Dépannage des problèmes courants

### Problèmes de déploiement

Si vous rencontrez des problèmes après le déploiement :

1. Vérifiez que tous les paquets NuGet requis sont correctement inclus
2. Vérifiez que l'architecture (x86/x64) correspond à la cible de votre application
3. Assurez-vous que les bibliothèques natives sont extraites aux emplacements corrects
4. Examinez les paramètres de sécurité et de permissions Windows susceptibles de restreindre les fonctionnalités multimédias

### Problème de build des fichiers RESX WinForms

Vous pouvez parfois obtenir l'erreur suivante :

`Error MSB3821: Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file. Remove the mark of the web if you want to process these files.`

L'erreur MSB3821 se produit lorsque Visual Studio ou MSBuild ne peut pas traiter un fichier de ressource `.resx` parce qu'il est marqué comme non fiable. Cela se produit quand le fichier porte la « Mark of the Web » (MOTW), une fonctionnalité de sécurité qui marque les fichiers téléchargés depuis Internet ou reçus de sources non fiables. La MOTW place le fichier dans la zone de sécurité Internet ou Restreinte, empêchant son traitement durant un build.

#### Comment corriger ce problème

Pour résoudre cette erreur, vous devez retirer la MOTW du fichier concerné :

##### Débloquer le fichier manuellement

- Cliquez avec le bouton droit sur Form1.resx dans l'Explorateur de fichiers.
- Sélectionnez Propriétés.
- Dans l'onglet Général, recherchez un bouton ou une case Débloquer en bas.
- Cliquez sur Débloquer, puis sur OK.

##### Débloquer via PowerShell (pour plusieurs fichiers)

- Ouvrez PowerShell.
- Naviguez jusqu'au répertoire de votre projet.
- Exécutez la commande : Get-ChildItem -Path . -Recurse | Unblock-File

##### Débloquer l'archive ZIP avant l'extraction

- Si vous avez téléchargé le projet sous forme de fichier ZIP, cliquez avec le bouton droit sur le fichier ZIP.
- Sélectionnez Propriétés.
- Cliquez sur Débloquer, puis extrayez les fichiers.

En débloquant le fichier, vous supprimez la MOTW, ce qui permet à Visual Studio de le traiter normalement durant le build.

Pour toute assistance supplémentaire, visitez le [site de support VisioForge](https://support.visioforge.com/) ou consultez la [documentation de l'API](https://api.visioforge.org/dotnet/api/index.html).

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
