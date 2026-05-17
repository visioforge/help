---
title: Capture ONVIF IP vers MP4 avec effets vidéo en C# .NET SDK
description: Capturez la vidéo de caméras IP ONVIF et appliquez redimensionnement, luminosité et filtres avant l'enregistrement MP4 avec le VisioForge Media Blocks SDK .NET.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Recording
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - AAC
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - RTSPSourceBlock
  - RTSPSourceSettings
  - H264EncoderBlock
  - AACEncoderBlock
  - MP4SinkBlock

---

# Capture MP4 depuis une caméra ONVIF avec post-traitement

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Le Media Blocks SDK fonctionne sous **Windows, macOS, Linux, Android et iOS** via GStreamer. Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) pour les détails sur les codecs et l'accélération matérielle, ainsi que le [guide de déploiement Linux](../../deployment-x/Ubuntu.md) pour Ubuntu / NVIDIA Jetson / Raspberry Pi.

!!!info Exemples de démo
Pour des exemples complets et fonctionnels, consultez :
- [Démo RTSP Preview](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) — Montre l'aperçu caméra ONVIF avec post-traitement
- [Démo IP Capture (Video Capture SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) — Alternative utilisant le Video Capture SDK

Pour une documentation ONVIF complète, consultez le [guide d'intégration ONVIF IP Camera](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Table des matières

- [Capture MP4 depuis une caméra ONVIF avec post-traitement](#capture-mp4-depuis-une-camera-onvif-avec-post-traitement)
  - [Table des matières](#table-des-matieres)
  - [Vue d'ensemble](#vue-densemble)
  - [Quand utiliser le post-traitement](#quand-utiliser-le-post-traitement)
  - [Prérequis](#prerequis)
  - [Configuration de base : découverte et connexion ONVIF](#configuration-de-base-decouverte-et-connexion-onvif)
  - [Exemple 1 : redimensionner la vidéo](#exemple-1-redimensionner-la-video)
  - [Exemple 2 : appliquer des effets vidéo](#exemple-2-appliquer-des-effets-video)
  - [Exemple 3 : flou des visages en temps réel](#exemple-3-flou-des-visages-en-temps-reel)
  - [Exemple 4 : superposition de filigrane et logo](#exemple-4-superposition-de-filigrane-et-logo)
  - [Considérations de performance](#considerations-de-performance)
  - [Bonnes pratiques](#bonnes-pratiques)
  - [Dépannage](#depannage)

## Vue d'ensemble

Ce guide montre comment capturer la vidéo de caméras IP ONVIF tout en appliquant divers effets de post-traitement avant l'encodage vers MP4. Contrairement à l'enregistrement pass-through qui préserve le flux d'origine, le post-traitement exige le décodage de la vidéo, l'application de transformations et le réencodage.

Cette approche est utile lorsque vous devez :
- Redimensionner ou rogner la vidéo
- Appliquer des corrections de luminosité, contraste ou couleur
- Ajouter des filigranes ou logos
- Flouter les visages pour préserver la vie privée
- Appliquer des effets ou filtres artistiques
- Combiner plusieurs étapes de traitement

## Quand utiliser le post-traitement

**Utilisez le post-traitement lorsque :**
- Vous devez redimensionner la vidéo (par ex. 4K vers 1080p)
- Vous voulez appliquer des effets vidéo (luminosité, contraste, etc.)
- Vous devez ajouter des superpositions ou filigranes
- Les exigences de confidentialité imposent le flou des visages
- Vous combinez plusieurs flux caméra
- Vous devez appliquer des algorithmes IA / vision par ordinateur

**Utilisez le pass-through (sans post-traitement) lorsque :**
- Vous voulez préserver la qualité vidéo d'origine
- Vous devez minimiser la consommation CPU
- L'espace de stockage n'est pas un problème
- La durée d'enregistrement est longue (heures/jours)

Pour l'enregistrement pass-through, consultez [Enregistrer un flux RTSP sans réencodage](./rtsp-save-original-stream.md).

## Prérequis

1. **VisioForge Media Blocks SDK .NET** installé
2. **Caméra ONVIF** accessible sur votre réseau
3. **Identifiants caméra valides** (nom d'utilisateur et mot de passe)
4. **Compréhension de base de** :
   - C# async/await
   - bases du protocole ONVIF
   - paramètres d'encodage vidéo

## Configuration de base : découverte et connexion ONVIF

D'abord, découvrez et connectez-vous à votre caméra ONVIF :

```cs
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFX;

// Découvrir les caméras ONVIF
var discovery = new Discovery();
var cts = new CancellationTokenSource();
string cameraUrl = null;

await discovery.Discover(5, (device) =>
{
    if (device.XAdresses?.Any() == true)
    {
        cameraUrl = device.XAdresses.FirstOrDefault();
        Console.WriteLine($"Found camera: {cameraUrl}");
    }
}, cts.Token);

if (string.IsNullOrEmpty(cameraUrl))
{
    Console.WriteLine("No ONVIF cameras found");
    return;
}

// Se connecter à la caméra
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(cameraUrl, "admin", "password");

if (!connected)
{
    Console.WriteLine("Failed to connect to camera");
    return;
}

// Obtenir l'URL du flux RTSP
var profiles = await onvifClient.GetProfilesAsync();
var streamUri = await onvifClient.GetStreamUriAsync(profiles[0]);
string rtspUrl = streamUri.Uri;

Console.WriteLine($"RTSP URL: {rtspUrl}");
```

## Exemple 1 : redimensionner la vidéo

Redimensionnez la vidéo de la caméra ONVIF avant l'enregistrement vers MP4. L'API Media Blocks prend
des objets de paramètres, pas des propriétés fluides — `RTSPSourceBlock` se construit à partir
d'un `RTSPSourceSettings`, l'encodeur à partir de son objet de paramètres, le puits MP4 distribue
des pads d'entrée via `CreateNewInput(...)`, et les liens sont déclarés sur le pipeline.

```cs
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types;

// Créer le pipeline
var pipeline = new MediaBlocksPipeline();

// Source RTSP depuis la caméra ONVIF. Les identifiants et la latence se trouvent sur l'objet
// settings ; utilisez la fabrique async pour que les paramètres découvrent les infos de codec en amont.
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Bloc de redimensionnement vidéo — réduire à 1280x720. Le ctor (width, height) est un
// raccourci pour `new VideoResizeBlock(new ResizeVideoEffect(w, h))`.
var videoResize = new VideoResizeBlock(1280, 720);

// Encodeur H.264. Choisissez une classe de paramètres concrète — Bitrate est en Kbit/s (2000 = 2 Mbps).
// OpenH264EncoderSettings fonctionne sur toute plateforme ; passez à NVENC / QSV / AMF / MFH264 pour l'accélération GPU.
var h264Settings = new OpenH264EncoderSettings { Bitrate = 2000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// Encodeur audio AAC (Bitrate en Kbit/s — 128 = 128 kbps).
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Puits MP4 — le ctor avec chemin de fichier est le moyen le plus court d'obtenir un écrivain MP4 valide.
var mp4Sink = new MP4SinkBlock("output_resized.mp4");

// Ajouter chaque bloc au pipeline avant de câbler les liens
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(videoResize);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Câbler le chemin vidéo (vidéo RTSP → resize → H.264 → pad vidéo du puits MP4)
pipeline.Connect(rtspSource.VideoOutput, videoResize.Input);
pipeline.Connect(videoResize.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Câbler le chemin audio (audio RTSP → AAC → pad audio du puits MP4)
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Démarrer l'enregistrement
await pipeline.StartAsync();

Console.WriteLine("Recording with resize... Press Enter to stop.");
Console.ReadLine();

// Arrêter et nettoyer
await pipeline.StopAsync();
await pipeline.DisposeAsync();

Console.WriteLine("Recording complete: output_resized.mp4");
```

## Exemple 2 : appliquer des effets vidéo

Appliquez des ajustements de luminosité, contraste, teinte et saturation. Les blocs de traitement
vidéo prennent un objet de paramètres dans le ctor ; les paramètres portent les boutons de réglage.

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

// Créer le pipeline
var pipeline = new MediaBlocksPipeline();

// Source RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Bloc de balance vidéo — les boutons sont sur l'objet de paramètres.
// Brightness : -1.0..1.0 (0.2 = légèrement plus lumineux)
// Contrast :    0.0..2.0 (1.15 = +15 % contraste)
// Saturation :  0.0..2.0 (1.3  = +30 % saturation)
// Hue :        -1.0..1.0 (0.0  = pas de décalage)
var balanceSettings = new VideoBalanceVideoEffect
{
    Brightness = 0.2,
    Contrast   = 1.15,
    Saturation = 1.3,
    Hue        = 0.0,
};
var videoBalance = new VideoBalanceBlock(balanceSettings);

// Le bloc d'effets de couleur prend un préréglage directement dans le ctor
var colorEffects = new ColorEffectsBlock(ColorEffectsPreset.Sepia);

// Encodeur H.264 (3 Mbps)
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// Audio AAC
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Sortie MP4
var mp4Sink = new MP4SinkBlock("output_enhanced.mp4");

// Ajouter le tout au pipeline puis câbler
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(videoBalance);
pipeline.AddBlock(colorEffects);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Chaîne vidéo : RTSP → balance → color-effects → H.264 → MP4
pipeline.Connect(rtspSource.VideoOutput, videoBalance.Input);
pipeline.Connect(videoBalance.Output, colorEffects.Input);
pipeline.Connect(colorEffects.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Chaîne audio
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Démarrer
await pipeline.StartAsync();

Console.WriteLine("Recording with enhancements...");
await Task.Delay(TimeSpan.FromMinutes(5)); // Enregistrer pendant 5 minutes

await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## Exemple 3 : flou des visages en temps réel

Appliquez la détection et le flou des visages pour la protection de la vie privée :

```cs
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;

// Créer le pipeline
var pipeline = new MediaBlocksPipeline();

// Source RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// CVFaceBlurBlock — détection et flou automatique des visages via OpenCV
var faceBlurSettings = new CVFaceBlurSettings
{
    ScaleFactor      = 1.25,
    MinNeighbors     = 3,
    MinSize          = new Size(30, 30),
    MainCascadeFile  = "haarcascade_frontalface_default.xml",
};
var faceBlur = new CVFaceBlurBlock(faceBlurSettings);

// Encodeur H.264
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// Audio AAC
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Sortie MP4
var mp4Sink = new MP4SinkBlock("output_face_blur.mp4");

// Ajouter + câbler
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(faceBlur);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

pipeline.Connect(rtspSource.VideoOutput, faceBlur.Input);
pipeline.Connect(faceBlur.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Démarrer
await pipeline.StartAsync();
```

## Exemple 4 : superposition de filigrane et logo

Ajoutez un logo en filigrane et une superposition d'horodatage. `ImageOverlayBlock` prend soit un
nom de fichier, soit un `ImageOverlaySettings` ; position/opacité sont sur les paramètres.
`TextOverlayBlock` prend toujours un `TextOverlaySettings`.

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;
using SkiaSharp;

// Créer le pipeline
var pipeline = new MediaBlocksPipeline();

// Source RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Logo / filigrane : le ctor avec chemin de fichier charge l'image. Les boutons de position
// sont sur l'objet de paramètres ; la transparence est Alpha (0..1, pas Opacity).
var logoOverlay = new ImageOverlayBlock(new ImageOverlaySettings("watermark.png")
{
    X     = 10,   // 10 px depuis la gauche
    Y     = 10,   // 10 px depuis le haut
    Alpha = 0.7,  // 0..1
});

// Superposition de texte statique. TextOverlaySettings porte Text, position, Color (SKColor)
// et boutons de police — voir la référence OverlayManagerText pour toutes les options.
var textOverlay = new TextOverlayBlock(new TextOverlaySettings("Camera 1")
{
    Color = SKColors.White,
});

// Encodeur H.264
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// Audio AAC
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Sortie MP4
var mp4Sink = new MP4SinkBlock("output_watermarked.mp4");

// Ajouter + câbler
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(logoOverlay);
pipeline.AddBlock(textOverlay);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Chaîne vidéo : RTSP → logo → texte → H.264 → MP4
pipeline.Connect(rtspSource.VideoOutput, logoOverlay.Input);
pipeline.Connect(logoOverlay.Output, textOverlay.Input);
pipeline.Connect(textOverlay.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Chaîne audio
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Démarrer
await pipeline.StartAsync();
```

## Considérations de performance

1. **Consommation CPU** : le traitement vidéo est gourmand en CPU. Chaque effet ajoute du surcoût :
   - Redimensionnement simple : ~10-20 % CPU par flux
   - Correction colorimétrique : ~5-15 % CPU
   - Détection de visages : ~30-50 % CPU (dépend de la résolution)
   - Effets multiples : consommation CPU cumulative

2. **Accélération GPU** : utilisez les encodeurs matériels lorsque disponibles.
   `H264EncoderBlock.GetDefaultSettings()` préfère déjà NVENC / QSV / AMF si la
   plateforme le permet, mais vous pouvez forcer un backend spécifique :

   ```cs
   // Encodeur NVIDIA NVENC H.264 (Bitrate en Kbit/s — 4000 = 4 Mbps)
   var nvencSettings = new NVENCH264EncoderSettings { Bitrate = 4000 };
   var h264Encoder   = new H264EncoderBlock(nvencSettings);
   ```

3. **Gestion des erreurs** : abonnez-vous à `OnError` pour être informé des défaillances du pipeline :

   ```cs
   pipeline.OnError += (sender, e) =>
   {
       Console.WriteLine($"Pipeline error: {e.Message}");
   };
   ```

4. **Équilibre des paramètres d'encodage** :
   - **Qualité** : débit plus élevé, préréglage plus lent = meilleure qualité, plus de CPU
   - **Performance** : débit plus bas, préréglage plus rapide = qualité moindre, moins de CPU
   - **Taille de fichier** : le débit affecte directement la taille du fichier

## Bonnes pratiques

1. **Testez d'abord les performances** :
   - Commencez par un pipeline simple
   - Ajoutez les effets un par un
   - Surveillez la consommation CPU/mémoire
   - Ajustez les paramètres en fonction du matériel

2. **Choisissez des débits appropriés** (toutes les valeurs en Kbit/s) :
   - 720p : 1000-2000
   - 1080p : 2000-4000
   - 4K : 8000-15000

3. **Libérez les ressources** :

   ```cs
   try
   {
       await pipeline.StartAsync();
       // ... enregistrement ...
   }
   finally
   {
       await pipeline.StopAsync();
       await pipeline.DisposeAsync();
       onvifClient?.Dispose();
   }
   ```

4. **Observez les événements du cycle de vie du pipeline** :

   ```cs
   pipeline.OnStart  += (s, e) => Console.WriteLine("Pipeline started");
   pipeline.OnStop   += (s, e) => Console.WriteLine("Pipeline stopped");
   pipeline.OnPause  += (s, e) => Console.WriteLine("Pipeline paused");
   pipeline.OnResume += (s, e) => Console.WriteLine("Pipeline resumed");
   ```

## Dépannage

**Consommation CPU élevée :**
- Réduisez la résolution vidéo
- Préréglage d'encodeur plus bas (encodage plus rapide)
- Supprimez les effets inutiles
- Utilisez l'accélération GPU si disponible

**Images abandonnées :**
- Vérifiez si le CPU est saturé
- Réduisez la fréquence d'images
- Abaissez le débit
- Simplifiez le pipeline de traitement

**Qualité vidéo médiocre :**
- Augmentez le débit
- Utilisez un préréglage d'encodeur plus lent
- Vérifiez la qualité de la vidéo source
- Vérifiez la bande passante réseau pour RTSP

**Fuites mémoire :**
- Assurez-vous de libérer correctement les blocs
- Vérifiez l'absence de références circulaires
- Surveillez les enregistrements longue durée

**Effets non appliqués :**
- Vérifiez les connexions des blocs via `pipeline.Connect(outPad, inPad)`
- Vérifiez que les paramètres d'effet sont valides
- Assurez-vous que chaque bloc est enregistré via `pipeline.AddBlock(...)` avant `StartAsync`
- Revoyez l'ordre du pipeline (chaîne d'effets)

---
Pour un enregistrement plus simple sans post-traitement, consultez [Enregistrer un flux RTSP sans réencodage](./rtsp-save-original-stream.md).
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples complets et fonctionnels.
