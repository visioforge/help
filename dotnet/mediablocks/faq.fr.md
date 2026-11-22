---
title: FAQ Media Blocks SDK .NET — images, sources push, RTSP
description: Réponses sur Media Blocks SDK .NET — envoyer des images H.264, obtenir des bitmaps sans affichage, détection de mouvement, latence et serveurs RTSP.
sidebar_label: FAQ
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - RTSP
  - H.264
  - Troubleshooting
primary_api_classes:
  - MediaBlocksPipeline
  - VideoSampleGrabberBlock
  - H264PushSourceBlock
  - CVMotionCellsBlock
  - RTSPSourceSettings
  - RTSPServerBlock
  - VirtualCameraSinkBlock
  - LiveVideoCompositor

---

# FAQ de Media Blocks SDK .NET

Réponses aux questions que les développeurs posent le plus souvent sur les pipelines de Media Blocks SDK .NET. Les erreurs de déploiement, comme l'absence de `libgstreamer-1.0` sous Windows, sont regroupées dans [Dépannage](../general/troubleshooting.md).

## Entrée et sortie d'images

### Comment lire des images H.264 que mon application reçoit par un socket ?

Envoyez-les dans le pipeline avec `H264PushSourceBlock` et connectez-le à un décodeur et à un moteur de rendu. Le bloc accepte les données Annex B (flux d'octets) et AVC (préfixées par leur longueur), et recale les horodatages que vous passez à `PushData()`. Commencez l'envoi par une image clé. Le bloc, ses propriétés et un pipeline complet sont décrits dans [Bloc source push H264](Sources/index.md#bloc-source-push-h264). Pour les autres formats, `PushSourceBlock` avec `PushDataSourceSettings` accepte toutes les données que vous décrivez avec des caps GStreamer.

Implémentez `OnError` sur le pipeline pendant que vous le construisez : un pad impossible à connecter ou un décodeur qui rejette les données y sont signalés.

### Comment obtenir les images de plusieurs caméras sous forme de bitmaps sans les afficher ?

Remplacez le `VideoRendererBlock` par un `VideoSampleGrabberBlock` ; aucun `VideoView` n'est nécessaire :

```csharp
var grabber = new VideoSampleGrabberBlock(VideoFormatX.BGRA, addNullRenderer: true);
grabber.OnVideoFrameBuffer += Grabber_OnVideoFrameBuffer;

pipeline.Connect(rtspSource.VideoOutput, grabber.Input);
```

`addNullRenderer: true` termine la branche pour que le pipeline fonctionne sans affichage. Utilisez un pipeline, ou une branche, par caméra. Choisissez le format de pixel attendu par votre type de bitmap : SkiaSharp et `Bgra32` de WPF utilisent BGRA, et les bitmaps 32 bits de `System.Drawing` sont aussi en BGRA en mémoire. Traitez ou copiez l'image dans le gestionnaire, et libérez chaque bitmap que vous en créez — une mémoire qui augmente avec chaque caméra ajoutée signifie généralement que les images sont conservées ou mises en file plus vite qu'elles ne sont traitées. Voir [Video sample grabber](VideoProcessing/index.md#video-sample-grabber).

## Analyse vidéo

### Comment détecter le mouvement dans un pipeline Media Blocks ?

Utilisez `CVMotionCellsBlock`. Il a besoin du redistribuable OpenCV de votre plateforme, par exemple le paquet NuGet `VisioForge.CrossPlatform.OpenCV.Windows.x64`, et sa sortie doit être connectée — à un moteur de rendu, ou à un `NullRendererBlock` si vous n'affichez pas la vidéo :

```csharp
var motion = new CVMotionCellsBlock(new CVMotionCellsSettings());
motion.MotionDetected += Motion_MotionDetected;

pipeline.Connect(source.VideoOutput, motion.Input);
pipeline.Connect(motion.Output, videoRenderer.Input);
```

`CVMotionCellsBlock.IsAvailable()` indique si le plugin OpenCV a été trouvé. Les paramètres et les événements sont décrits dans [Bloc CV Motion Cells](OpenCV/index.md#bloc-cv-motion-cells).

## RTSP

### Quelle latence définir pour une source RTSP ?

`RTSPSourceSettings.Latency` est le tampon de gigue de la source RTSP ; sa valeur par défaut est 500 ms. Une valeur inférieure à une image — 20 ms pour une caméra à 25 i/s, dont les images sont espacées de 40 ms — ne peut pas fonctionner. Pour une faible latence, partez de 100 à 200 ms et descendez uniquement tant que l'image reste fluide ; `LowLatencyMode = true` désactive la mise en tampon supplémentaire et fixe la latence à 150 ms, sauf si vous l'avez déjà modifiée.

Ne connectez la branche audio que lorsque la caméra envoie réellement de l'audio : `RTSPSourceSettings.CreateAsync` lit les informations du flux par défaut, et `IsAudioAvailable()` vous indique alors s'il faut ajouter le moteur de rendu ou l'encodeur audio. Une branche audio qui attend un flux qui n'arrive jamais est une cause fréquente de pipelines qui ne démarrent pas. Pour une visionneuse complète, consultez [Visionneuse RTSP](Guides/rtsp-player-csharp.md).

### Peut-on diffuser un fichier vidéo en RTSP, en boucle ?

Oui. Construisez un pipeline avec une source fichier, des encodeurs et un `RTSPServerBlock` ; les clients se connectent alors à l'URL du serveur. Définissez la propriété `Loop` du pipeline pour relancer le fichier à la fin ; elle fonctionne avec les sources dans lesquelles on peut se déplacer, comme les fichiers. Voir [Bloc serveur RTSP](RTSPServer/index.md).

## Sortie vers d'autres applications

### Comment envoyer la sortie du pipeline, ou le Live Video Compositor, vers une caméra virtuelle ?

Connectez la vidéo (et éventuellement l'audio) à un `VirtualCameraSinkBlock`. Zoom, Teams, OBS et les navigateurs la voient alors comme une webcam. Les filtres Virtual Camera et Virtual Audio Card doivent d'abord être enregistrés sur l'ordinateur ; le bloc peut les vérifier et les enregistrer. Les détails se trouvent dans [Puits caméra virtuelle](Sinks/index.md#virtual-camera-sink), et la [Virtual Camera Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Virtual%20Camera%20Demo) montre l'ensemble du flux.

Avec le Live Video Compositor, encapsulez le bloc dans une sortie et ajoutez-la au compositeur :

```csharp
var vcBlock = new VirtualCameraSinkBlock(new VirtualCameraSinkSettings());
var output = new LVCVideoAudioOutput("Virtual Camera", compositor, vcBlock, autostart: true);
await compositor.Output_AddAsync(output);
```

La [Live Video Compositor Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Video%20Compositor%20Demo) la propose parmi ses sorties. Sans clé de licence Virtual Camera SDK, la caméra virtuelle affiche une bande TRIAL.
