---
title: Dépannage des SDK .NET — erreurs courantes et solutions
description: Corrigez les erreurs courantes des SDK .NET VisioForge — filtres et DLL manquants, MF Mux, REGDB_E_CLASSNOTREG, framework cible, MSB3821 et NVENC.
sidebar_label: Dépannage
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - Deployment
  - NuGet
  - Troubleshooting
primary_api_classes:
  - VideoCaptureCore
  - MediaPlayerCore
  - VideoEditCore
  - VideoView

---

# Corriger les erreurs courantes des SDK .NET

Voici les erreurs que les développeurs rencontrent le plus souvent lors de la première compilation ou du premier déploiement d'une application avec Video Capture SDK .NET, Media Player SDK .NET, Video Edit SDK .NET ou Media Blocks SDK .NET. La plupart sont des problèmes de déploiement : un composant natif dont l'application a besoin n'est pas à côté d'elle ou n'est pas enregistré.

Si aucune des réponses ci-dessous ne vous aide, implémentez l'événement `OnError` de l'objet moteur, reproduisez le problème et [envoyez les journaux](sendlogs.md) au support.

## Erreurs de déploiement

### « Unable to create filter » ou « Unable to add … filter »

Les messages comme `Unable to create filter: VisioForge Resize`, `Unable to add Video Effects filter`, `Unable to create LAV Splitter Source filter` ou `Unable to add filter: … REGDB_E_CLASSNOTREG (0x80040154)` proviennent des moteurs Windows (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`). Ils signifient qu'un filtre DirectShow nécessaire au moteur n'est pas disponible sur cet ordinateur.

La solution consiste à déployer les redistribuables :

1. **NuGet (recommandé) :** ajoutez les paquets `VisioForge.DotNet.Core.Redist.*` dont vos fonctions ont besoin : le paquet de base, le paquet de votre SDK et les paquets de fonctions (MP4, FFMPEG, LAV, VLC, etc.). Leurs fichiers sont copiés dans le dossier de sortie à la compilation et chargés depuis cet emplacement sans enregistrement.
2. **Programme d'installation ou déploiement manuel :** exécutez le programme d'installation des redistribuables, ou copiez et enregistrez les filtres comme décrit dans le guide de déploiement.

L'architecture du paquet doit correspondre à celle du processus : un processus x64 ne peut pas charger de filtres x86. Si l'application fonctionne sur le poste de développement mais pas sur un ordinateur vierge, comparez les dossiers de sortie : la cause habituelle est un paquet installé uniquement sur le poste de développement.

Les listes complètes de paquets se trouvent dans les guides de déploiement : [Video Capture SDK](../videocapture/deployment.md), [Media Player SDK](../mediaplayer/deployment.md), [Video Edit SDK](../videoedit/deployment.md).

### « Unable to create VisioForge MF Mux filter »

Les sorties Media Foundation de `VideoCaptureCore` — `MP4HWOutput`, `MPEGTSOutput` et les autres sorties qui utilisent les encodeurs Microsoft ou GPU — écrivent le fichier via le filtre VisioForge MF Mux, fourni avec le redistribuable MP4. Ajoutez le paquet `VisioForge.DotNet.Core.Redist.MP4.x64` (ou `.x86`, `.arm64` selon votre plateforme). Le même paquet est nécessaire pour la source RTSP à faible latence.

### DllNotFoundException: Unable to load DLL 'VisioForge_MFP64.dll'

`VisioForge_MFP64.dll` (et `VisioForge_MFP.dll` pour x86) est une bibliothèque native du SDK. Elle est fournie avec les paquets `VisioForge.DotNet.Core.Redist.Base` pour les moteurs Windows et avec les paquets `VisioForge.CrossPlatform.Core.Windows` pour les moteurs X et Media Blocks SDK .NET. L'exception signifie qu'elle n'est pas dans le dossier de l'application ou que son architecture ne correspond pas à celle du processus :

- Ajoutez le paquet redistribuable (par exemple `VisioForge.DotNet.Core.Redist.Base.x64`) au projet de l'application lui-même, et pas seulement à une bibliothèque de classes qu'il référence.
- Compilez pour une plateforme explicite (`x64` ou `x86`) plutôt qu'AnyCPU lorsque vous déployez un seul jeu de redistribuables.
- Si les paquets sont référencés et que le fichier manque toujours, retirez-les et ajoutez-les de nouveau (ou supprimez les dossiers `bin` et `obj`) pour que l'étape de copie s'exécute à nouveau.

### libgstreamer-1.0 introuvable ou VideoCaptureCore introuvable

Les moteurs Windows (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) et les contrôles d'interface Windows n'existent que dans la version Windows du SDK. Un projet dont le framework cible n'a pas le suffixe `-windows` (par exemple `net8.0`) reçoit la version multiplateforme : le compilateur ne trouve pas les types Windows, et un moteur X ou un pipeline Media Blocks cherche au démarrage les bibliothèques GStreamer de Linux (`libgstreamer-1.0.so`).

Utilisez un framework cible Windows, par exemple :

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

Vérifiez aussi que l'application s'exécute sur la même plateforme (x64 ou x86) que les paquets redistribuables que vous avez ajoutés.

### Erreur MSB3821 sur les fichiers .resx d'un exemple téléchargé

`Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file` signifie que Windows a marqué comme non fiables les fichiers d'un ZIP téléchargé. Débloquez l'archive avant de l'extraire, ou clonez le [dépôt d'exemples](https://github.com/visioforge/.Net-SDK-s-samples) avec git. La marche à suivre se trouve dans [Problème de build des fichiers RESX WinForms](../deployment-x/Windows.md#probleme-de-build-des-fichiers-resx-winforms).

## Mise à niveau depuis des versions antérieures

### Les anciens noms de classes comme VFMP4v8v10Output ou le contrôle VideoCapture sont introuvables

Les anciens noms des classes de sortie correspondent à l'API actuelle : `VFMP4v8v10Output` (renommée `VFMP4Output` en version 14.2) s'appelle désormais `MP4Output`. La version 15 a scindé les contrôles d'interface uniques `VideoCapture`, `MediaPlayer` et `VideoEdit` en un contrôle `VideoView`, qui affiche la vidéo, et une classe moteur (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) qui conserve l'API de propriétés et de méthodes habituelle. Créez le moteur dans le code et passez-lui le `VideoView`, comme le font les [exemples](https://github.com/visioforge/.Net-SDK-s-samples). Pour la version actuelle, consultez le [guide de migration](../migration-from-v15.md).

## Licences

### Le texte « UNREGISTERED VERSION » s'affiche sur la vidéo

Cette superposition est dessinée par la version d'essai de 30 jours. Sa deuxième ligne indique l'édition requise par les fonctionnalités utilisées (STANDARD, PROFESSIONAL ou PREMIUM) et la dernière est la page d'achat, visioforge.com/buy ; les pipelines Media Blocks et `VideoEditCoreX` affichent seulement UNREGISTERED et l'adresse. Une licence achetée est fournie sous la forme d'un fichier de certificat `.vflicense` : chargez-le dans chaque instance du moteur avec `SetLicenseCertificateAsync` avant de la démarrer. Un exemple se trouve dans [Video Capture SDK .NET](../videocapture/index.md). Si la superposition d'essai apparaît mais que rien n'est lu, le message n'est pas la cause : consultez l'événement `OnError` pour connaître l'erreur réelle.

## Encodage matériel

### L'encodeur NVIDIA NVENC fonctionne sur un PC mais pas sur un autre

Lorsque NVENC échoue sur un ordinateur (un message `Unable to activate encoder`, aucun fichier de sortie) alors qu'il fonctionne sur un autre, vérifiez les points suivants :

- Installez le pilote le plus récent depuis le site web de NVIDIA ; les pilotes installés par Windows Update peuvent être plus anciens que la version de NVENC utilisée par le SDK.
- Les cartes GeForce limitent le nombre de sessions d'encodage simultanées. Fermez les autres applications qui encodent (OBS, navigateurs qui partagent l'écran, enregistreurs) et réessayez.
- Les GPU anciens ne prennent pas en charge tous les codecs, par exemple HEVC. Essayez d'abord H.264.
- Testez la même configuration avec l'application de démonstration du SDK. Si la démonstration échoue aussi, [envoyez les journaux](sendlogs.md).

Utilisez un autre encodeur (Intel Quick Sync, AMD AMF ou un encodeur logiciel) comme solution de repli sur les ordinateurs sans NVENC fonctionnel. Les options d'encodeur se trouvent dans [Encodeurs H.264](video-encoders/h264.md).
