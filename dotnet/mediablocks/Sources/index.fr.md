---
title: Sources Media Blocks SDK — webcam, fichier, RTSP, écran
description: Connectez toute entrée à un pipeline Media Blocks : webcams, fichiers vidéo, RTSP, RTMP, SRT, NDI, Decklink, écran, virtuelle. API d'énumération C# / .NET.
sidebar_label: Sources
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - VideoRendererBlock
  - MediaBlocksPipeline
  - AudioRendererBlock
  - UniversalSourceBlock
  - DeviceEnumerator

---

# Blocs de sources — VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Les blocs de sources fournissent des données au pipeline et sont généralement les premiers blocs de toute chaîne de traitement multimédia. VisioForge Media Blocks SDK .Net fournit une collection complète de blocs de sources pour diverses entrées, notamment les périphériques matériels, les fichiers, les réseaux et les sources virtuelles.

!!! tip "Agents IA de programmation : utilisez le serveur MCP VisioForge"

    Vous construisez avec **Claude Code**, **Cursor** ou un autre agent IA de programmation ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à `https://mcp.visioforge.com/mcp` pour des recherches d'API structurées, des
    exemples de code exécutables et des guides de déploiement — plus précis qu'un grep
    sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Blocs de sources matérielles

### Source vidéo système { #system-video-source }

`SystemVideoSourceBlock` permet d'accéder aux webcams et autres périphériques de capture vidéo.

#### Informations sur le bloc

Nom : SystemVideoSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Énumération des périphériques disponibles

Utilisez la méthode `DeviceEnumerator.Shared.VideoSourcesAsync()` pour obtenir la liste des périphériques disponibles et leurs spécifications : résolutions disponibles, fréquences d'images et formats vidéo.
Cette méthode renvoie une liste d'objets `VideoCaptureDeviceInfo`. Chaque objet `VideoCaptureDeviceInfo` fournit des informations détaillées sur un périphérique de capture.

#### Exemple de pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// créer la source vidéo
VideoCaptureDeviceSourceSettings videoSourceSettings = null;

// sélectionner le premier périphérique
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
if (device != null)
{
    // sélectionner le premier format (peut-être pas le meilleur, juste pour l'exemple)
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };

        // sélectionner la première fréquence d'images
        videoSourceSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

// créer le bloc source vidéo avec le périphérique et le format sélectionnés
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);

// créer le bloc de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connecter les blocs
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// démarrer le pipeline
await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo de capture vidéo simple (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

#### Remarques

Vous pouvez spécifier une API à utiliser lors de l'énumération des périphériques (consultez la description de l'enum `VideoCaptureDeviceAPI` sous `SystemVideoSourceBlock` pour les valeurs typiques). Les plateformes Android et iOS n'ont qu'une seule API, tandis que Windows et Linux en proposent plusieurs.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Source audio système { #system-audio-source }

`SystemAudioSourceBlock` permet d'accéder aux microphones et autres périphériques de capture audio.

#### Informations sur le bloc

Nom : SystemAudioSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Audio en sortie | audio non compressé | 1 |

#### Énumération des périphériques disponibles

Utilisez l'appel de méthode `DeviceEnumerator.Shared.AudioSourcesAsync()` pour obtenir la liste des périphériques disponibles et leurs spécifications.

Lors de l'énumération des périphériques, vous pouvez obtenir la liste des périphériques disponibles et leurs spécifications. Vous pouvez sélectionner le périphérique et son format pour créer les paramètres de la source.

#### Exemple de pipeline

```mermaid
graph LR;
    SystemAudioSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// créer le bloc source audio
IAudioCaptureDeviceSourceSettings audioSourceSettings = null;

// sélectionner le premier périphérique
var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
if (device != null)
{
    // sélectionner le premier format
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        audioSourceSettings = device.CreateSourceSettings(formatItem.ToFormat());
    }    
}

// créer le bloc source audio avec le périphérique et le format sélectionnés
var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

// créer le bloc de rendu audio
var audioRenderer = new AudioRendererBlock();

// connecter les blocs
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// démarrer le pipeline
await pipeline.StartAsync();
```

#### Capture audio depuis les haut-parleurs (loopback)

Actuellement, la capture audio loopback n'est prise en charge que sur Windows. Utilisez la classe `LoopbackAudioCaptureDeviceSourceSettings` pour créer les paramètres de source dédiés à la capture audio loopback.

WASAPI2 est utilisée comme API par défaut pour la capture audio loopback. Vous pouvez spécifier l'API à utiliser lors de l'énumération des périphériques.

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// créer le bloc source audio
var deviceItem = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2))[0];
if (deviceItem == null)
{
    return;
}

var audioSourceSettings = new LoopbackAudioCaptureDeviceSourceSettings(deviceItem);
var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

// créer le bloc de rendu audio
var audioRenderer = new AudioRendererBlock();

// connecter les blocs
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// démarrer le pipeline
await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo de capture audio](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Audio%20Capture%20Demo)
- [Démo de capture simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

#### Remarques

Vous pouvez spécifier une API à utiliser lors de l'énumération des périphériques. Les plateformes Android et iOS n'ont qu'une seule API, tandis que Windows et Linux en proposent plusieurs.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source Basler

Le bloc source Basler prend en charge les caméras Basler USB3 Vision et GigE.
Le SDK Pylon ou le Runtime doit être installé pour utiliser la source caméra.

#### Informations sur le bloc

Nom : BaslerSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|-----------------|:--------------------:|:-----------:|
| Vidéo en sortie |     Non compressée    |      1      |

#### Exemple de pipeline

```mermaid
graph LR;
    BaslerSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// obtenir les infos de source Basler en énumérant les sources
var sources = await DeviceEnumerator.Shared.BaslerSourcesAsync();
var sourceInfo = sources[0];

// créer la source Basler
var source = new BaslerSourceBlock(new BaslerSourceSettings(sourceInfo));

// créer le moteur de rendu vidéo pour VideoView
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connecter
pipeline.Connect(source.Output, videoRenderer.Input);

// démarrer
await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo source Basler (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Basler%20Source%20Demo)

#### Plateformes

Windows, Linux.

### Bloc source Spinnaker/FLIR

La source Spinnaker/FLIR prend en charge la connexion aux caméras FLIR via le SDK Spinnaker.

Pour utiliser le `SpinnakerSourceBlock`, vous devez d'abord énumérer les caméras Spinnaker disponibles, puis configurer la source à l'aide de `SpinnakerSourceSettings`.

#### Énumérer les périphériques et `SpinnakerCameraInfo`

Utilisez `DeviceEnumerator.Shared.SpinnakerSourcesAsync()` pour obtenir une liste d'objets `SpinnakerCameraInfo`. Chaque `SpinnakerCameraInfo` fournit des détails sur une caméra détectée :

- `Name` (string) : identifiant ou nom unique de la caméra. Souvent un numéro de série ou une combinaison modèle-série.
- `NetworkInterfaceName` (string) : nom de l'interface réseau s'il s'agit d'une caméra GigE.
- `Vendor` (string) : nom du fabricant de la caméra.
- `Model` (string) : nom du modèle de la caméra.
- `SerialNumber` (string) : numéro de série de la caméra.
- `FirmwareVersion` (string) : version du micrologiciel de la caméra.
- `SensorSize` (`Size`) : indique les dimensions du capteur (largeur, hauteur). Vous devrez peut-être appeler une méthode de `SpinnakerCameraInfo` telle que `ReadInfo()` (si disponible, ou impliquée par l'énumération) pour renseigner ce champ.
- `WidthMax` (int) : largeur maximale du capteur.
- `HeightMax` (int) : hauteur maximale du capteur.

Vous sélectionnez un objet `SpinnakerCameraInfo` dans la liste pour initialiser `SpinnakerSourceSettings`.

#### Paramètres

Le `SpinnakerSourceBlock` se configure via `SpinnakerSourceSettings`. Propriétés clés :

- `Name` (string) : nom de la caméra (issu de `SpinnakerCameraInfo.Name`) à utiliser.
- `Region` (`Rect`) : définit la région d'intérêt (ROI) à capturer depuis le capteur de la caméra. Renseignez X, Y, Width, Height.
- `FrameRate` (`VideoFrameRate`) : fréquence d'images souhaitée.
- `PixelFormat` (enum `SpinnakerPixelFormat`) : format de pixel souhaité (par ex. `RGB`, `Mono8`, `BayerRG8`). Par défaut `RGB`.
- `OffsetX` (int) : décalage X pour la ROI sur le capteur (par défaut 0). Souvent implicitement inclus dans `Region.X`.
- `OffsetY` (int) : décalage Y pour la ROI sur le capteur (par défaut 0). Souvent implicitement inclus dans `Region.Y`.
- `ExposureMinimum` (int) : temps d'exposition minimum pour l'algorithme d'exposition automatique (microsecondes, par ex. 10-29999999). Par défaut 0 (auto/valeur par défaut de la caméra).
- `ExposureMaximum` (int) : temps d'exposition maximum pour l'algorithme d'exposition automatique (microsecondes). Par défaut 0 (auto/valeur par défaut de la caméra).
- `ShutterType` (enum `SpinnakerSourceShutterType`) : type d'obturateur (par ex. `Rolling`, `Global`). Par défaut `Rolling`.

Constructeur :
`SpinnakerSourceSettings(string deviceName, Rect region, VideoFrameRate frameRate, SpinnakerPixelFormat pixelFormat = SpinnakerPixelFormat.RGB)`

#### Informations sur le bloc

Nom : SpinnakerSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | divers | un ou plusieurs |

#### Exemple de pipeline

`SpinnakerSourceBlock:Output` &#8594;  `VideoRendererBlock`

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var sources = await DeviceEnumerator.Shared.SpinnakerSourcesAsync();
var sourceSettings = new SpinnakerSourceSettings(sources[0].Name, new VisioForge.Core.Types.Rect(0, 0, 1280, 720), new VideoFrameRate(10)); 

var source = new SpinnakerSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Prérequis

- Le SDK Spinnaker doit être installé.

#### Plateformes

Windows

### Bloc source Allied Vision

Le bloc source Allied Vision permet l'intégration des caméras Allied Vision via le SDK Vimba. Il autorise la capture de flux vidéo depuis ces caméras industrielles.

#### Informations sur le bloc

Nom : AlliedVisionSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Vidéo en sortie | Vidéo non compressée | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    AlliedVisionSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Énumérer les caméras Allied Vision
var alliedVisionCameras = await DeviceEnumerator.Shared.AlliedVisionSourcesAsync();
if (alliedVisionCameras.Count == 0)
{
    Console.WriteLine("Aucune caméra Allied Vision trouvée.");
    return;
}

var cameraInfo = alliedVisionCameras[0]; // Sélectionner la première caméra

// Créer les paramètres de la source Allied Vision
// Width, height, x, y sont facultatifs et dépendent de la définition éventuelle d'une ROI spécifique
// Si null, la résolution par défaut/complète du capteur peut être utilisée. Camera.ReadInfo() doit être appelée.
cameraInfo.ReadInfo(); // S'assurer que les infos de la caméra (Width/Height) sont lues

var alliedVisionSettings = new AlliedVisionSourceSettings(
    cameraInfo,
    width: cameraInfo.Width, // Ou une largeur de ROI spécifique
    height: cameraInfo.Height // Ou une hauteur de ROI spécifique
);

// Configurer éventuellement d'autres paramètres
alliedVisionSettings.ExposureAuto = VmbSrcExposureAutoModes.Continuous;
alliedVisionSettings.Gain = 10; // Exemple de valeur de gain

var alliedVisionSource = new AlliedVisionSourceBlock(alliedVisionSettings);

// Créer le moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant que VideoView1 est votre contrôle d'affichage

// Connecter les blocs
pipeline.Connect(alliedVisionSource.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Prérequis

- Le SDK Allied Vision Vimba doit être installé.

#### Exemples d'applications

- Référez-vous aux exemples illustrant l'intégration de caméras industrielles s'il en existe.

#### Plateformes

Windows, macOS, Linux.

### Bloc source Blackmagic Decklink

Pour plus d'informations sur les sources Decklink, consultez [Decklink](../Decklink/index.md).

## Blocs de sources de fichiers

### Bloc source universel

Une source universelle qui décode les fichiers et flux réseau audio/vidéo et fournit des données non compressées aux blocs connectés.

Le bloc prend en charge MP4, WebM, AVI, TS, MKV, MP3, AAC, M4A et bien d'autres formats. Si le redistribuable FFMPEG est disponible, tous les décodeurs disponibles dans FFMPEG sont également pris en charge.

#### Paramètres

Le `UniversalSourceBlock` se configure via `UniversalSourceSettings`. Il est recommandé de créer les paramètres à l'aide de la méthode de fabrique statique `await UniversalSourceSettings.CreateAsync(...)`.

Propriétés et paramètres clés de `UniversalSourceSettings` :

- **URI/Nom de fichier** :
  - `UniversalSourceSettings.CreateAsync(string filename, bool renderVideo = true, bool renderAudio = true, bool renderSubtitle = false)` : crée les paramètres à partir d'un chemin de fichier local.
  - `UniversalSourceSettings.CreateAsync(System.Uri uri, bool renderVideo = true, bool renderAudio = true, bool renderSubtitle = false)` : crée les paramètres à partir d'un `System.Uri` (peut être une URI de fichier ou une URI réseau telle que HTTP, RTSP — bien que des blocs dédiés soient souvent préférés pour les flux réseau). Pour iOS, un `Foundation.NSUrl` est utilisé.
  - Les booléens `renderVideo`, `renderAudio`, `renderSubtitle` contrôlent les flux traités. La méthode `CreateAsync` peut les actualiser en fonction de la disponibilité réelle des flux dans le fichier/flux multimédia si `ignoreMediaInfoReader` est `false` (par défaut).
- `StartPosition` (`TimeSpan?`) : définit la position de départ de la lecture.
- `StopPosition` (`TimeSpan?`) : définit la position d'arrêt de la lecture.
- `VideoCustomFrameRate` (`VideoFrameRate?`) : si défini, les images vidéo seront supprimées ou dupliquées pour correspondre à cette fréquence d'images personnalisée.
- `UseAdvancedEngine` (bool) : si `true` (par défaut, sauf sur Android où la valeur est `false`), utilise un moteur avancé prenant en charge la sélection de flux.
- `DisableHWDecoders` (bool) : si `true` (par défaut `false`, sauf sur Android où c'est `true`), les décodeurs accélérés matériellement sont désactivés, forçant le décodage logiciel.
- `MPEGTSProgramNumber` (int) : pour les flux MPEG-TS, spécifie le numéro de programme à sélectionner (par défaut -1, ce qui signifie sélection automatique ou premier programme).
- `ReadInfoAsync()` : lit de façon asynchrone les informations du fichier multimédia (`MediaFileInfo`). Cette méthode est appelée en interne par `CreateAsync` sauf si `ignoreMediaInfoReader` vaut true.
- `GetInfo()` : obtient le `MediaFileInfo` mis en cache.

Le `UniversalSourceBlock` lui-même est ensuite instancié avec ces paramètres : `new UniversalSourceBlock(settings)`.
La propriété `Filename` sur l'instance `UniversalSourceBlock` (telle que vue dans des exemples plus anciens) est un raccourci qui crée en interne des `UniversalSourceSettings` basiques. Utiliser `UniversalSourceSettings.CreateAsync` offre davantage de contrôle.

#### Informations sur le bloc

Nom : UniversalSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Audio en sortie | dépend du décodeur | un ou plusieurs |
| Vidéo en sortie | dépend du décodeur | un ou plusieurs |
| Sous-titres en sortie | dépend du décodeur | un ou plusieurs |

#### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->VideoRendererBlock;
    UniversalSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var sourceSettings = await UniversalSourceSettings.CreateAsync("test.mp4");
var fileSource = new UniversalSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);            

await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo de lecteur simple (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Player%20Demo%20WPF)

#### Plateformes

Windows, macOS, Linux, iOS, Android.


### Bloc source de sous-titres

Le bloc source de sous-titres charge les sous-titres depuis un fichier et les renvoie sous forme de flux de sous-titres, qui peut ensuite être superposé à la vidéo ou rendu séparément.

#### Informations sur le bloc

Nom : `SubtitleSourceBlock`.

| Direction du pin | Type de média | Nombre de pins |
|-----------------|:--------------------:|:-----------:|
| Sous-titres en sortie | Données de sous-titres | 1           |

#### Paramètres

Le `SubtitleSourceBlock` se configure via `SubtitleSourceSettings`. Propriétés clés :

- `Filename` (string) : chemin du fichier de sous-titres (par ex. .srt, .ass).

#### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock --> SubtitleOverlayBlock;
    SubtitleSourceBlock --> SubtitleOverlayBlock;
    SubtitleOverlayBlock --> VideoRendererBlock;
    UniversalSourceBlock --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer les paramètres de la source de sous-titres
var subtitleSettings = new SubtitleSourceSettings("path/to/your/subtitles.srt");
var subtitleSource = new SubtitleSourceBlock(subtitleSettings);

// Exemple : superposer des sous-titres sur une vidéo issue d'un UniversalSourceBlock
var fileSource = await UniversalSourceSettings.CreateAsync("path/to/your/video.mp4");
var universalSource = new UniversalSourceBlock(fileSource);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

// Il s'agit d'une superposition conceptuelle. L'implémentation réelle peut nécessiter un bloc de superposition de sous-titres spécifique.
// Par simplicité, supposons qu'un bloc en aval puisse consommer un flux de sous-titres,
// ou que vous le connectiez à un bloc qui restitue les sous-titres sur la vidéo.
// Exemple avec un SubtitleOverlayBlock hypothétique :
// var subtitleOverlay = new SubtitleOverlayBlock(); // En supposant qu'un tel bloc existe
// pipeline.Connect(universalSource.VideoOutput, subtitleOverlay.VideoInput);
// pipeline.Connect(subtitleSource.Output, subtitleOverlay.SubtitleInput);
// pipeline.Connect(subtitleOverlay.Output, videoRenderer.Input);
// pipeline.Connect(universalSource.AudioOutput, audioRenderer.Input);

// Pour un lecteur simple sans superposition explicite montrée ici :
pipeline.Connect(universalSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(universalSource.AudioOutput, audioRenderer.Input);
// L'utilisation des sous-titres issus de subtitleSource.Output dépend du reste de la conception du pipeline.
// Ce bloc fournit principalement le flux de sous-titres.

Console.WriteLine("Source de sous-titres créée. Connectez sa sortie à un bloc compatible tel qu'une superposition ou un moteur de rendu de sous-titres.");

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android (selon les capacités d'analyse des sous-titres).

### Bloc source de flux (Stream)

Le bloc source de flux permet de lire des données multimédias depuis un `System.IO.Stream`. C'est utile pour lire des médias depuis la mémoire, des ressources embarquées ou des fournisseurs de flux personnalisés sans avoir besoin d'un fichier temporaire. Le format des données du flux doit être analysable par le framework multimédia sous-jacent (GStreamer).

#### Informations sur le bloc

Nom : `StreamSourceBlock`.
(Les informations sur les pins sont dynamiques, à la manière de `UniversalSourceBlock`, et dépendent du contenu du flux. Typiquement, il dispose d'une sortie qui se connecte à un démultiplexeur/décodeur tel que `DecodeBinBlock`, ou fournit des pins audio/vidéo décodés s'il inclut des capacités de démultiplexage/décodage.)

| Direction du pin | Type de média | Nombre de pins |
|---------------|:------------------:|:----------:|
| Données en sortie | variable (flux brut) | 1          |
| Vidéo en sortie | dépend du flux | 0 ou 1     |
| Audio en sortie | dépend du flux | 0 ou 1+    |

#### Paramètres

Le `StreamSourceBlock` est typiquement instancié directement avec un `System.IO.Stream`. La classe `StreamSourceSettings` sert d'enveloppe pour fournir ce flux.

- `Stream` (`System.IO.Stream`) : flux d'entrée contenant les données multimédias. Le flux doit être lisible et, si le pipeline requiert la possibilité de se positionner, accessible en seek.

#### Exemple de pipeline

Si `StreamSourceBlock` produit des données brutes qui nécessitent un décodage :

```mermaid
graph LR;
    StreamSourceBlock -- Données du flux --> DecodeBinBlock;
    DecodeBinBlock -- Sortie vidéo --> VideoRendererBlock;
    DecodeBinBlock -- Sortie audio --> AudioRendererBlock;
```

Si `StreamSourceBlock` gère le décodage en interne (cas moins courant pour une source de flux générique) :

```mermaid
graph LR;
    StreamSourceBlock -- Sortie vidéo --> VideoRendererBlock;
    StreamSourceBlock -- Sortie audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Exemple : charger un fichier vidéo dans un MemoryStream
byte[] fileBytes = File.ReadAllBytes("path/to/your/video.mp4");
var memoryStream = new MemoryStream(fileBytes);

// StreamSourceSettings est un conteneur pour le flux.
var streamSettings = new StreamSourceSettings(memoryStream);
// La méthode CreateBlock de StreamSourceSettings renvoie typiquement new StreamSourceBlock(streamSettings.Stream)
var streamSource = streamSettings.CreateBlock() as StreamSourceBlock; 
// Ou, plus directement : var streamSource = new StreamSourceBlock(memoryStream);

// Créer les moteurs de rendu vidéo et audio
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant VideoView1
var audioRenderer = new AudioRendererBlock();

// Connecter les sorties. Couramment, un StreamSourceBlock fournit des données brutes à un DecodeBinBlock.
var decodeBin = new DecodeBinBlock();
pipeline.Connect(streamSource.Output, decodeBin.Input); // En supposant un unique pin 'Output' sur StreamSourceBlock
pipeline.Connect(decodeBin.VideoOutput, videoRenderer.Input);
pipeline.Connect(decodeBin.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();

// Important : assurez-vous que le flux reste ouvert et valide pendant toute la durée de la lecture.
// Libérez le flux lorsque le pipeline est arrêté ou libéré.
// Tenez-en compte par rapport à pipeline.DisposeAsync() ou à un nettoyage similaire.
// memoryStream.Dispose(); // Typiquement après pipeline.StopAsync() et pipeline.DisposeAsync()
```

#### Remarques

Le `StreamSourceBlock` tente de lire depuis le flux fourni. La réussite de la lecture dépend du format des données du flux et de la disponibilité des démultiplexeurs et décodeurs appropriés dans les parties suivantes du pipeline (souvent gérées via `DecodeBinBlock`).

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source de flux avec décodeur

Le `StreamSourceBlockWithDecoder` combine la lecture de flux et le décodage automatique en un seul bloc. Il accepte un `System.IO.Stream` ou un chemin de fichier et démultiplexe et décode automatiquement le contenu — aucun `DecodeBinBlock` supplémentaire n'est nécessaire.

#### Informations sur le bloc

Nom : StreamSourceBlockWithDecoder.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 0 ou 1 |
| Audio en sortie | Audio non compressé | 0 ou 1 |

#### Constructeurs

```csharp
// Depuis un flux .NET
StreamSourceBlockWithDecoder(Stream stream)

// Depuis un chemin de fichier
StreamSourceBlockWithDecoder(string filename)
```

Le bloc connecte en interne un `StreamSourceBlock` à un `DecodeBinBlock`. Les pads `VideoOutput` et `AudioOutput` valent `null` si la source ne contient pas le type de média correspondant.

#### Exemple de pipeline

```mermaid
graph LR;
    StreamSourceBlockWithDecoder -- Vidéo --> VideoRendererBlock;
    StreamSourceBlockWithDecoder -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Depuis un chemin de fichier
var source = new StreamSourceBlockWithDecoder("C:/media/video.mp4");

// Ou depuis un flux :
// var stream = File.OpenRead("C:/media/video.mp4");
// var source = new StreamSourceBlockWithDecoder(stream);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source de séquence d'images

Le `ImageSequenceSourceBlock` génère un flux vidéo à partir d'une séquence d'images fixes stockées dans un dossier. La fréquence d'images est configurable et le bloc prend en charge les modes de flux en direct ou seekable.

#### Informations sur le bloc

Nom : ImageSequenceSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |

#### Paramètres

Le bloc prend une instance de `ImageSequenceSourceSettings` :

Constructeur : `ImageSequenceSourceSettings(string folderPath, string filePattern = null)`

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `FolderPath` | `string` | requis | Dossier contenant les fichiers d'images |
| `FilePattern` | `string` | détection automatique | Motif de nom de fichier (par ex. `image_%05d.jpg`) ; détecté automatiquement si `null` |
| `StartIndex` | `int` | détection automatique | Index de départ de la séquence d'images ; détecté automatiquement à partir du nom du premier fichier |
| `FrameRate` | `VideoFrameRate` | `FPS_25` | Fréquence d'images en sortie |
| `IsLive` | `bool` | `false` | Traiter la source comme un flux en direct (non seekable) |
| `NumBuffers` | `int` | `-1` | Nombre maximal d'images à produire (−1 = illimité) |

Formats d'image pris en charge : `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.tiff`, `.tif`.

#### Exemple de pipeline

```mermaid
graph LR;
    ImageSequenceSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageSequenceSourceSettings("C:/images/sequence/")
{
    FrameRate = VideoFrameRate.FPS_25
};

var imageSource = new ImageSequenceSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

`ImageSequenceSourceBlock.IsAvailable(settings)` renvoie `true` si l'élément GStreamer de séquence d'images est disponible.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source GIF animé

Le `AnimatedGIFSourceBlock` génère un flux vidéo à partir d'un fichier GIF animé. Il prend en charge la fréquence d'images configurable, le bouclage et le mode de flux en direct ou seekable.

#### Informations sur le bloc

Nom : AnimatedGIFSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |

#### Paramètres

Le bloc prend une instance de `ImageVideoSourceSettings` :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Filename` | `string` | requis | Chemin du fichier GIF animé |
| `IsLive` | `bool` | `false` | Traiter la source comme un flux en direct (non seekable) |
| `NumBuffers` | `int` | `-1` | Nombre maximal d'images à produire (−1 = illimité) |
| `AnimationInfiniteLoop` | `bool` | `true` | Boucler indéfiniment l'animation |
| `FrameRate` | `VideoFrameRate` | `FPS_10` | Fréquence d'images en sortie |

#### Exemple de pipeline

```mermaid
graph LR;
    AnimatedGIFSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageVideoSourceSettings("C:/media/animation.gif")
{
    AnimationInfiniteLoop = true,
    FrameRate = VideoFrameRate.FPS_25
};

var gifSource = new AnimatedGIFSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(gifSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

`AnimatedGIFSourceBlock.IsAvailable()` renvoie `true` si l'élément GStreamer de GIF animé est disponible.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source CDG

Le bloc source CDG est conçu pour lire les fichiers CD+G (Compact Disc + Graphics), couramment utilisés pour le karaoké. Il décode à la fois la piste audio et le flux graphique basse résolution.

#### Informations sur le bloc

Nom : CDGSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Audio en sortie  | Audio non compressé | 1          |
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    CDGSourceBlock -- Audio --> AudioRendererBlock;
    CDGSourceBlock -- Vidéo --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer les paramètres de la source CDG
var cdgSettings = new CDGSourceSettings(
    "path/to/your/file.cdg",  // Chemin du fichier graphique CDG
    "path/to/your/file.mp3"   // Chemin du fichier audio correspondant (MP3, WAV, etc.)
);
// Si audioFilename est null ou vide, l'audio sera ignoré.

var cdgSource = new CDGSourceBlock(cdgSettings);

// Créer le moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant que VideoView1 est votre contrôle d'affichage
pipeline.Connect(cdgSource.VideoOutput, videoRenderer.Input);

// Créer le moteur de rendu audio (si l'audio doit être lu)
if (!string.IsNullOrEmpty(cdgSettings.AudioFilename) && cdgSource.AudioOutput != null)
{
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(cdgSource.AudioOutput, audioRenderer.Input);
}

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Remarques

Nécessite à la fois un fichier `.cdg` pour les graphiques et un fichier audio séparé (par ex. MP3, WAV) pour la musique.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

## Blocs de sources réseau

### Bloc source VNC

Le bloc source VNC permet de capturer la vidéo depuis un serveur VNC (Virtual Network Computing) ou RFB (Remote Framebuffer). C'est utile pour diffuser le bureau d'une machine distante.

#### Informations sur le bloc

Nom : `VNCSourceBlock`.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Paramètres

Le `VNCSourceBlock` se configure via `VNCSourceSettings`. Propriétés clés :

- `Host` (string) : nom d'hôte ou adresse IP du serveur VNC.
- `Port` (int) : numéro de port du serveur VNC.
- `Password` (string) : mot de passe pour l'authentification au serveur VNC, si requis.
- `Uri` (string) : alternativement, une URI RFB complète (par ex. "rfb://host:port").
- `Width` (int) : largeur de sortie souhaitée. Le bloc peut se connecter à un serveur VNC qui fournit des dimensions spécifiques.
- `Height` (int) : hauteur de sortie souhaitée.
- `Shared` (bool) : indique si le bureau doit être partagé avec d'autres clients (par défaut `true`).
- `ViewOnly` (bool) : si `true`, aucune entrée (souris/clavier) n'est envoyée au serveur VNC (par défaut `false`).
- `Incremental` (bool) : indique si les mises à jour incrémentielles doivent être utilisées (par défaut `true`).
- `UseCopyrect` (bool) : indique si l'encodage copyrect doit être utilisé (par défaut `false`).
- `RFBVersion` (string) : version du protocole RFB (par défaut "3.3").
- `OffsetX` (int) : décalage X pour la capture d'écran.
- `OffsetY` (int) : décalage Y pour la capture d'écran.

#### Exemple de pipeline

```mermaid
graph LR;
    VNCSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer les paramètres de la source VNC
var vncSettings = new VNCSourceSettings
{
    Host = "your-vnc-server-ip", // ou utilisez Uri
    Port = 5900, // Port VNC standard
    Password = "your-password", // si nécessaire
    // Width = 1920, // Facultatif : largeur souhaitée
    // Height = 1080, // Facultatif : hauteur souhaitée
};

var vncSource = new VNCSourceBlock(vncSettings);

// Créer le moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant que VideoView1 est votre contrôle d'affichage

// Connecter les blocs
pipeline.Connect(vncSource.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux (dépend de la disponibilité du plugin VNC GStreamer sous-jacent).

### Bloc source RTSP

La source RTSP prend en charge la connexion à des caméras IP et autres périphériques compatibles avec le protocole RTSP.

Codecs vidéo pris en charge : H264, HEVC, MJPEG.
Codecs audio pris en charge : AAC, MP3, PCM, G726, G711, et quelques autres si le redistribuable FFMPEG est installé.

#### Informations sur le bloc

Nom : RTSPSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Audio en sortie | dépend du décodeur | un ou plusieurs |
| Vidéo en sortie | dépend du décodeur | un ou plusieurs |
| Sous-titres en sortie | dépend du décodeur | un ou plusieurs |

#### Paramètres

Le `RTSPSourceBlock` se configure via `RTSPSourceSettings`. Propriétés clés :

- `Uri` : URL RTSP du flux.
- `Login` : nom d'utilisateur pour l'authentification RTSP, si requis.
- `Password` : mot de passe pour l'authentification RTSP, si requis.
- `AudioEnabled` : booléen indiquant s'il faut tenter de traiter le flux audio.
- `Latency` : spécifie la durée de mise en tampon du flux entrant (par défaut 1000 ms).
- `AllowedProtocols` : définit les protocoles de transport utilisés pour recevoir le flux. C'est un enum de flags `RTSPSourceProtocol` avec les valeurs :
  - `UDP` : transport des données sur UDP.
  - `UDP_Multicast` : transport des données sur UDP multicast.
  - `TCP` (recommandé) : transport des données sur TCP.
  - `HTTP` : transport des données via un tunnel HTTP.
  - `EnableTLS` : chiffrement TCP et HTTP avec TLS (utilisez `rtsps://` ou `httpsps://` dans l'URI).
- `DoRTCP` : active RTCP (RTP Control Protocol) pour les statistiques et le contrôle du flux (généralement true par défaut).
- `RTPBlockSize` : spécifie la taille des blocs RTP.
- `UDPBufferSize` : taille du tampon pour le transport UDP.
- `CustomVideoDecoder` : permet de spécifier un nom d'élément décodeur vidéo GStreamer personnalisé si le décodeur par défaut ne convient pas.
- `UseGPUDecoder` : si défini à `true`, le SDK tentera d'utiliser un décodeur GPU accéléré matériellement si disponible.
- `CompatibilityMode` : si `true`, le SDK n'essaie pas de lire les informations de la caméra avant de tenter la lecture, ce qui peut être utile pour les flux problématiques.
- `EnableRAWVideoAudioEvents` : si `true`, active les événements pour les données brutes (non décodées) d'échantillons vidéo et audio.

Il est recommandé d'initialiser `RTSPSourceSettings` à l'aide de la méthode de fabrique statique `RTSPSourceSettings.CreateAsync(Uri uri, string login, string password, bool audioEnabled, bool readInfo = true)`. Cette méthode peut également gérer la découverte ONVIF si l'URI pointe vers un service de périphérique ONVIF. Définir `readInfo` à `false` active `CompatibilityMode`.

#### Exemple de pipeline

```mermaid
graph LR;
    RTSPSourceBlock-->VideoRendererBlock;
    RTSPSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Il est recommandé d'utiliser CreateAsync pour initialiser les paramètres
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://login:pwd@192.168.1.64:554/Streaming/Channels/101?transportmode=unicast&profile=Profile_1"),
    "login", 
    "pwd",
    audioEnabled: true);

// Configurer éventuellement des paramètres supplémentaires
// rtspSettings.Latency = TimeSpan.FromMilliseconds(500);
// rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP; // Préférer TCP

var rtspSource = new RTSPSourceBlock(rtspSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);      

await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo d'aperçu RTSP](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo)
- [Démo RTSP MultiViewSync](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20MultiViewSync%20Demo)

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source RTSP brut

Le `RTSPRAWSourceBlock` se connecte à un flux RTSP et fournit directement les données vidéo et audio **encodées** (non décodées). Contrairement à `RTSPSourceBlock`, aucun pipeline de décodage n'est créé, ce qui réduit la charge CPU. Utilisez ce bloc lorsque vous devez enregistrer, retransmettre ou traiter le flux binaire compressé brut.

#### Informations sur le bloc

Nom : RTSPRAWSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo encodée (par ex. H.264) | 1 |
| Audio en sortie | Audio encodé (par ex. AAC) | 1 |

#### Paramètres

Utilisez la méthode de fabrique `RTSPRAWSourceSettings.CreateAsync(uri, login, password, audioEnabled)` pour créer les paramètres. Propriétés clés :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Uri` | `Uri` | — | URL du flux RTSP |
| `AllowedProtocols` | `RTSPSourceProtocol` | Tous | Protocoles de transport (TCP, UDP, UDP_Multicast, HTTP) |
| `Latency` | `int` | — | Mise en tampon en millisecondes |
| `Login` | `string` | — | Nom d'utilisateur pour l'authentification |
| `Password` | `string` | — | Mot de passe pour l'authentification |
| `AudioEnabled` | `bool` | `true` | Activer le pad de sortie audio |
| `WaitForKeyframe` | `bool` | `true` | Attendre une image IDR avant de transmettre la vidéo |
| `SyncAudioWithKeyframe` | `bool` | `true` | Synchroniser la livraison audio sur la première image clé |
| `DoRTCP` | `bool` | — | Activer le protocole de contrôle RTCP |
| `UDPBufferSize` | `int` | — | Taille du tampon de réception UDP |

#### Exemple de pipeline

```mermaid
graph LR;
    RTSPRAWSourceBlock-->MP4SinkBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.64:554/stream"),
    login: "admin",
    password: "password",
    audioEnabled: true);

var rtspRawSource = new RTSPRAWSourceBlock(settings);

// VideoOutput transporte la vidéo encodée (par ex. H.264)
// AudioOutput transporte l'audio encodé (par ex. AAC)
var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(rtspRawSource.VideoOutput, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(rtspRawSource.AudioOutput, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

#### Disponibilité

`RTSPRAWSourceBlock.IsAvailable()` renvoie `true` si les éléments GStreamer RTSP requis sont disponibles.

#### Plateformes

Windows, macOS, Linux.

### Bloc source RTMP

Le bloc source RTMP se connecte à des flux RTMP publiés par des serveurs multimédias (Nginx-RTMP, Wowza, OBS, FFmpeg, etc.). Il fournit des pads vidéo et audio compressés à utiliser avec des blocs de décodage ou d'enregistrement.

Codecs vidéo pris en charge : H.264, H.265, VP8.
Codecs audio pris en charge : AAC, MP3.

#### Informations sur le bloc

Nom : RTMPSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo compressée | 1 |
| Audio en sortie | Audio compressé | 1 |

#### Paramètres

Le `RTMPSourceBlock` se configure via `RTMPSourceSettings`. Utilisez la méthode de fabrique statique pour initialiser les paramètres avec les informations de flux lues depuis le serveur :

```csharp
var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true);
```

Définissez `readInfo: false` (qui active `CompatibilityMode`) pour les flux dont la pré-lecture échoue :

```csharp
var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true,
    readInfo: false);
```

Propriétés clés :

- `Uri` : URI du flux RTMP.
- `AudioEnabled` : indique s'il faut créer un pad de sortie audio (par défaut `true`).
- `CompatibilityMode` : ignore la lecture des informations de flux ; utile pour les flux problématiques.
- `UseGPUDecoder` : tente le décodage accéléré matériellement.
- `ExtraConnectArgs` : paramètres de requête supplémentaires pour la connexion RTMP.

#### Exemple de pipeline

```mermaid
graph LR;
    RTMPSourceBlock-->VideoRendererBlock;
    RTMPSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true);

var rtmpSource = new RTMPSourceBlock(rtmpSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtmpSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtmpSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux.

### Bloc source HTTP

Le bloc source HTTP permet de récupérer des données via les protocoles HTTP/HTTPS.
Il peut être utilisé pour lire des données depuis des caméras IP MJPEG, des fichiers MP4 réseau ou d'autres sources.

#### Informations sur le bloc

Nom : HTTPSourceBlock.

| Direction du pin |  Type de média  | Nombre de pins  |
|---------------|:------------:|:-----------:|
| Sortie        |     Données     |      1      |

#### Exemple de pipeline

Le pipeline d'exemple lit les données depuis une caméra MJPEG et les affiche via VideoView.

```mermaid
graph LR;
    HTTPSourceBlock-->JPEGDecoderBlock;
    JPEGDecoderBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new HTTPSourceSettings(new Uri("http://mjpegcamera:8080"))
{
    UserID = "username",
    UserPassword = "password"
};

var source = new HTTPSourceBlock(settings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var jpegDecoder = new JPEGDecoderBlock();

pipeline.Connect(source.Output, jpegDecoder.Input);
pipeline.Connect(jpegDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux.


### Bloc source HTTP MJPEG

Le bloc source HTTP MJPEG est conçu spécifiquement pour se connecter aux flux vidéo MJPEG (Motion JPEG) sur HTTP/HTTPS et les décoder. C'est un cas courant pour de nombreuses caméras IP.

#### Informations sur le bloc

Nom : HTTPMJPEGSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    HTTPMJPEGSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer les paramètres pour la source HTTP MJPEG
var mjpegSettings = await HTTPMJPEGSourceSettings.CreateAsync(
    new Uri("http://your-mjpeg-camera-url/stream"), // Remplacez par l'URL du flux MJPEG de votre caméra
    "username", // Facultatif : nom d'utilisateur pour l'authentification de la caméra
    "password"  // Facultatif : mot de passe pour l'authentification de la caméra
);

if (mjpegSettings == null)
{
    Console.WriteLine("Échec de l'initialisation des paramètres HTTP MJPEG.");
    return;
}

mjpegSettings.CustomVideoFrameRate = new VideoFrameRate(25); // Facultatif : à définir si la caméra ne renvoie pas la fréquence d'images
mjpegSettings.Latency = TimeSpan.FromMilliseconds(200); // Facultatif : ajuster la latence

var httpMjpegSource = new HTTPMJPEGSourceBlock(mjpegSettings);

// Créer le moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant que VideoView1 est votre contrôle d'affichage

// Connecter les blocs
pipeline.Connect(httpMjpegSource.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Exemples d'applications

- Similaire à la démo source HTTP MJPEG mentionnée sous le bloc source HTTP générique.

#### Plateformes

Windows, macOS, Linux.

### Bloc source NDI

Le bloc source NDI prend en charge la connexion aux sources logicielles NDI et aux périphériques compatibles avec le protocole NDI.

#### Informations sur le bloc

Nom : NDISourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|-----------------|:--------------------:|:-----------:|
| Audio en sortie |     Non compressé     |      1      |
| Vidéo en sortie |     Non compressée    |      1      |

#### Exemple de pipeline

```mermaid
graph LR;
    NDISourceBlock-->VideoRendererBlock;
    NDISourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// obtenir les infos de la source NDI en énumérant les sources
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSourceInfo = ndiSources[0];

// CreateAsync prend (ContextX context, NDISourceInfo info) et doit être awaitée.
var ndiSettings = await NDISourceSettings.CreateAsync(pipeline.GetContext(), ndiSourceInfo);

var ndiSource = new NDISourceBlock(ndiSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);      

await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo source NDI](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/NDI%20Source%20Demo)

#### Plateformes

Windows, macOS, Linux.

### Bloc source NDI X

Le `NDISourceXBlock` est une source NDI étendue qui capture la vidéo et l'audio depuis des sources réseau NDI. Il utilise les mêmes `NDISourceSettings` que `NDISourceBlock` mais s'appuie sur un élément NDI GStreamer alternatif (`ndisrcx`).

#### Informations sur le bloc

Nom : NDISourceXBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |
| Audio en sortie | Audio non compressé | 1 |

#### Paramètres

Le bloc accepte `NDISourceSettings`. Utilisez la fabrique asynchrone statique pour le créer :

```csharp
var ndiSettings = await NDISourceSettings.CreateAsync(context, ndiSourceInfo);
```

Propriétés clés de `NDISourceSettings` :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `SourceName` | `string` | `""` | Nom de la source NDI |
| `URL` | `string` | `""` | URL de la source |
| `ReceiverName` | `string` | `"VF NDI Receiver"` | Nom de l'application réceptrice |
| `Bandwidth` | `int` | `100` | −10 métadonnées uniquement, 10 audio uniquement, 100 qualité maximale |
| `ColorFormat` | `NDIRecvColorFormat` | `UyvyBgra` | Format de pixel pour la vidéo reçue |
| `Timeout` | `TimeSpan` | 5 s | Délai d'attente pour détecter une déconnexion |
| `ConnectTimeout` | `TimeSpan` | 10 s | Délai d'attente pour la connexion initiale |
| `TimestampMode` | `NDITimestampMode` | `Auto` | Mode de synchronisation des horodatages |

#### Exemple de pipeline

```mermaid
graph LR;
    NDISourceXBlock -- Vidéo --> VideoRendererBlock;
    NDISourceXBlock -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSourceInfo = ndiSources[0];

// CreateAsync prend (ContextX context, NDISourceInfo info).
var ndiSettings = await NDISourceSettings.CreateAsync(pipeline.GetContext(), ndiSourceInfo);

var ndiSource = new NDISourceXBlock(ndiSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows.

### Bloc source GenICam

La source GenICam prend en charge la connexion aux caméras GigE et USB3 Vision compatibles avec le protocole GenICam.

#### Informations sur le bloc

Nom : GenICamSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | divers | un ou plusieurs |

#### Exemple de pipeline

```mermaid
graph LR;
    GenICamSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var sourceSettings = new GenICamSourceSettings(cbCamera.Text, new VisioForge.Core.Types.Rect(0, 0, 512, 512), 15, GenICamPixelFormat.Mono8); 
var source = new GenICamSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo source GenICam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/GenICam%20Source%20Demo)

En savoir plus sur la [source GenICam](../../videocapture/video-sources/usb3v-gige-genicam/index.md).

#### Plateformes

Windows, macOS, Linux

### Bloc source SRT (avec décodage)

Le protocole `Secure Reliable Transport (SRT)` est un protocole de streaming vidéo open source conçu pour une livraison sécurisée et à faible latence sur des réseaux imprévisibles, comme l'Internet public. Développé par Haivision, SRT optimise les performances de streaming en s'adaptant dynamiquement aux variations de bande passante et en minimisant les effets de la perte de paquets. Il intègre le chiffrement AES pour une transmission sécurisée du contenu. Principalement utilisé en radiodiffusion et en streaming en ligne, SRT est essentiel pour livrer des flux vidéo de haute qualité dans des applications temps réel, améliorant l'expérience des spectateurs même dans des conditions réseau difficiles. Il prend en charge le streaming point à point et multicast, ce qui le rend polyvalent pour des configurations variées.

Le bloc source SRT fournit des flux vidéo et audio décodés depuis une source SRT.

#### Informations sur le bloc

Nom : SRTSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Non compressée | 0+ |
| Audio en sortie | Non compressé | 0+ |

#### Paramètres

Le `SRTSourceBlock` se configure via `SRTSourceSettings`. Cette classe offre des options complètes pour les connexions SRT :

- `Uri` (string) : URI SRT (par ex. "srt://127.0.0.1:8888" ou "srt://example.com:9000?mode=listener"). Par défaut "srt://127.0.0.1:8888".
- `Mode` (enum `SRTConnectionMode`) : spécifie le mode de connexion SRT. Par défaut `Caller`. Voir les détails de l'enum `SRTConnectionMode` ci-dessous.
- `Passphrase` (string) : mot de passe pour une transmission chiffrée.
- `PbKeyLen` (enum `SRTKeyLength`) : longueur de la clé cryptographique pour le chiffrement AES. Par défaut `NoKey`. Voir les détails de l'enum `SRTKeyLength` ci-dessous.
- `Latency` (`TimeSpan`) : latence de transmission maximale acceptée (côté récepteur pour caller/listener, ou pour les deux en mode rendezvous). Par défaut 125 millisecondes.
- `StreamId` (string) : identifiant de flux pour le contrôle d'accès SRT.
- `LocalAddress` (string) : adresse locale à laquelle se lier en mode `Listener` ou `Rendezvous`. Par défaut `null` (toutes).
- `LocalPort` (uint) : port local auquel se lier en mode `Listener` ou `Rendezvous`. Par défaut 7001.
- `Authentication` (bool) : indique si la connexion doit être authentifiée. Par défaut `true`.
- `AutoReconnect` (bool) : indique si la source doit tenter une reconnexion en cas d'échec. Par défaut `true`.
- `KeepListening` (bool) : si `false` (par défaut), l'élément signale la fin de flux lorsque le client distant se déconnecte (en mode listener). Si `true`, il continue d'attendre une reconnexion.
- `PollTimeout` (`TimeSpan`) : délai de scrutation utilisé lorsqu'un polling SRT est démarré. Par défaut 1000 millisecondes.
- `WaitForConnection` (bool) : si `true` (par défaut), bloque le flux jusqu'à ce qu'un client se connecte (en mode listener).

Les `SRTSourceSettings` peuvent être initialisés via `await SRTSourceSettings.CreateAsync(string uri, bool ignoreMediaInfoReader = false)`. Définir `ignoreMediaInfoReader` à `true` peut s'avérer utile si la lecture des informations média échoue pour un flux en direct.

##### Enum `SRTConnectionMode`

Définit le mode opérationnel d'une connexion SRT :

- `None` (0) : aucun mode de connexion spécifié (à ne pas utiliser directement en général).
- `Caller` (1) : la source initie la connexion vers un listener.
- `Listener` (2) : la source attend une connexion entrante depuis un caller.
- `Rendezvous` (3) : les deux extrémités initient simultanément la connexion l'une vers l'autre, utile pour traverser des pare-feu.

##### Enum `SRTKeyLength`

Définit la longueur de clé pour le chiffrement AES de SRT :

- `NoKey` (0) / `Length0` (0) : aucun chiffrement utilisé.
- `Length16` (16) : clé de chiffrement AES 16 octets (128 bits).
- `Length24` (24) : clé de chiffrement AES 24 octets (192 bits).
- `Length32` (32) : clé de chiffrement AES 32 octets (256 bits).

#### Exemple de pipeline

```mermaid
graph LR;
    SRTSourceBlock-->VideoRendererBlock;
    SRTSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// SRTSourceSettings n'a qu'un constructeur privé ; utilisez la fabrique asynchrone.
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri(edURL.Text));
var source = new SRTSourceBlock(srtSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

pipeline.Connect(source.VideoOutput, videoRenderer.Input);
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Exemples d'applications

- [Démo source SRT](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/SRT%20Source%20Demo)

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source SRT brut

Le protocole `Secure Reliable Transport (SRT)` est un protocole de streaming qui optimise la livraison des données vidéo sur des réseaux imprévisibles, comme Internet. Open source et conçu pour gérer le streaming vidéo et audio haute performance, SRT offre la sécurité par le chiffrement de bout en bout, la fiabilité par la récupération des paquets perdus, et une faible latence adaptée aux diffusions en direct. Il s'adapte aux conditions réseau variables en gérant dynamiquement la bande passante, garantissant des flux de haute qualité même dans des conditions sous-optimales. Largement utilisé dans les applications de diffusion et de streaming, SRT prend en charge l'interopérabilité et est idéal pour la production distante et la distribution de contenu.

La source SRT prend en charge la connexion aux sources SRT et fournit un flux de données. Vous pouvez connecter ce bloc à `DecodeBinBlock` pour décoder le flux.

#### Informations sur le bloc

Nom : SRTRAWSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Données en sortie | Quelconque | un |

#### Paramètres

Le `SRTRAWSourceBlock` se configure via `SRTSourceSettings`. Consultez la description détaillée de `SRTSourceSettings` et de ses enums associés (`SRTConnectionMode`, `SRTKeyLength`) dans la section « Bloc source SRT (avec décodage) » pour toutes les propriétés disponibles et leurs explications.

#### Exemple de pipeline

```mermaid
graph LR;
    SRTRAWSourceBlock-->DecodeBinBlock;
    DecodeBinBlock-->VideoRendererBlock;
    DecodeBinBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// SRTSourceSettings n'a qu'un constructeur privé ; utilisez la fabrique asynchrone.
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri(edURL.Text));
var source = new SRTRAWSourceBlock(srtSettings);
var decodeBin = new DecodeBinBlock();
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

pipeline.Connect(source.Output, decodeBin.Input);
pipeline.Connect(decodeBin.VideoOutput, videoRenderer.Input);
pipeline.Connect(decodeBin.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

## Autres blocs de sources

### Bloc source écran

La source écran permet d'enregistrer la vidéo depuis l'écran. Vous pouvez sélectionner l'affichage (s'il y en a plusieurs), la partie de l'écran à enregistrer et, en option, l'enregistrement du curseur de la souris.

#### Paramètres

Le `ScreenSourceBlock` utilise des classes de paramètres spécifiques à la plateforme. Le choix de la classe de paramètres détermine la technologie de capture d'écran sous-jacente. L'enum `ScreenCaptureSourceType` indique les technologies disponibles :

##### Windows

- `ScreenCaptureDX9SourceSettings` — utilise `DirectX 9` pour l'enregistrement d'écran. (`ScreenCaptureSourceType.DX9`)
- `ScreenCaptureD3D11SourceSettings` — utilise `Direct3D 11` Desktop Duplication pour l'enregistrement d'écran. Permet la capture d'une fenêtre spécifique. (`ScreenCaptureSourceType.D3D11DesktopDuplication`)
- `ScreenCaptureGDISourceSettings` — utilise `GDI` pour l'enregistrement d'écran. (`ScreenCaptureSourceType.GDI`)

##### macOS

`ScreenCaptureMacOSSourceSettings` — utilise `AVFoundation` pour l'enregistrement d'écran. (`ScreenCaptureSourceType.AVFoundation`)

##### Linux

`ScreenCaptureXDisplaySourceSettings` — utilise `X11` (XDisplay) pour l'enregistrement d'écran. (`ScreenCaptureSourceType.XDisplay`)

##### iOS

`IOSScreenSourceSettings` — utilise `AVFoundation` pour l'enregistrement de la fenêtre/application courante. (`ScreenCaptureSourceType.IOSScreen`)

#### Informations sur le bloc

Nom : ScreenSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    ScreenSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

#### Exemple de code

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// créer les paramètres de la source
var screenSourceSettings = new ScreenCaptureDX9SourceSettings() { FrameRate = 15 };

// créer le bloc source
var screenSourceBlock = new ScreenSourceBlock(screenSourceSettings);

// créer le bloc encodeur vidéo et le connecter au bloc source
var h264EncoderBlock = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(screenSourceBlock.Output, h264EncoderBlock.Input);

// créer le bloc puits MP4 et le connecter au bloc encodeur
var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

// exécuter le pipeline
await pipeline.StartAsync();
```

#### [Windows] Capture de fenêtre

Vous pouvez capturer une fenêtre spécifique en utilisant la classe `ScreenCaptureD3D11SourceSettings`.

```csharp
// créer la source Direct3D11
var source = new ScreenCaptureD3D11SourceSettings();

// définir la fréquence d'images
source.FrameRate = new VideoFrameRate(30);

// obtenir le handle de la fenêtre
var wih = new System.Windows.Interop.WindowInteropHelper(this);
source.WindowHandle = wih.Handle;

// créer le bloc source en utilisant les paramètres D3D11 construits ci-dessus
var screenSourceBlock = new ScreenSourceBlock(source);

// le reste du code est identique à ci-dessus
```

#### Exemples d'applications

- [Démo de capture d'écran (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Screen%20Capture)
- [Démo de capture d'écran (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/ScreenCaptureMB)
- [Démo de capture d'écran (iOS)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/iOS/ScreenCapture)

#### Plateformes

Windows, macOS, Linux, iOS.

### Bloc source de bascule avec repli

Le `FallbackSwitchSourceBlock` enveloppe une source vidéo primaire et bascule automatiquement vers un contenu de repli lorsque la source principale échoue ou devient indisponible — idéal pour les pipelines de streaming en direct fiables.

#### Informations sur le bloc

Nom : FallbackSwitchSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |
| Audio en sortie | Audio non compressé | 1 |

#### Types de source principale pris en charge

Le premier argument du constructeur est `IVideoSourceSettings`. Implémentations prises en charge :

- `RTSPSourceSettings` — flux RTSP
- `SRTSourceSettings` — flux SRT
- `NDISourceSettings` — source NDI
- `RTMPSourceSettings` — flux RTMP
- `HTTPSourceSettings` — flux HTTP

#### Paramètres

**`FallbackSwitchSettings`**

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Enabled` | `bool` | `false` | Active la bascule vers le repli |
| `Fallback` | `FallbackSwitchSettingsBase` | `null` | Configuration du contenu de repli |
| `EnableVideo` | `bool` | `true` | Applique le repli à la vidéo |
| `EnableAudio` | `bool` | `true` | Applique le repli à l'audio |
| `MinLatencyMs` | `int` | `0` | Latence minimale avant bascule |
| `FallbackVideoCaps` | `string` | `null` | Remplacement des caps vidéo (par ex. `"video/x-raw,width=1920,height=1080"`) |
| `FallbackAudioCaps` | `string` | `null` | Remplacement des caps audio |

Propriétés **`FallbackSwitchSettingsBase`** (partagées par tous les types de repli) :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `TimeoutMs` | `int` | `5000` | Millisecondes avant bascule vers le repli |
| `RestartTimeoutMs` | `int` | `5000` | Millisecondes avant nouvelle tentative sur la source principale |
| `RetryTimeoutMs` | `int` | `60000` | Millisecondes avant arrêt des tentatives |
| `ImmediateFallback` | `bool` | `false` | Bascule immédiatement vers le repli en cas d'erreur |
| `RestartOnEos` | `bool` | `false` | Redémarre la source principale en fin de flux |
| `ManualUnblock` | `bool` | `false` | Exige un appel manuel à `Unblock()` pour reprendre la source principale |

**Types de contenu de repli** (définissez `Fallback` à l'un de) :

- `StaticTextFallbackSettings` — superposition de texte avec police, couleur et position configurables
- `StaticImageFallbackSettings` — affiche une image statique (options : chemin de fichier, échelle, ratio d'aspect)
- `MediaBlockFallbackSettings` — lit une URI de repli ou un pipeline GStreamer personnalisé

**Méthodes de diagnostic :**

- `GetStatus()` — renvoie l'état courant de la source principale/de repli sous forme de chaîne
- `GetStatistics()` — renvoie un dictionnaire du nombre de tentatives et de statistiques de mise en tampon
- `Unblock()` — débloque manuellement lorsque `ManualUnblock` est activé

#### Exemple de pipeline

```mermaid
graph LR;
    FallbackSwitchSourceBlock -- Vidéo --> VideoRendererBlock;
    FallbackSwitchSourceBlock -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// RTSPSourceSettings a un constructeur privé — construisez-le via la fabrique CreateAsync.
var mainSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://camera.example.com/stream"),
    login: null,
    password: null,
    audioEnabled: true);

var fallbackSettings = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticTextFallbackSettings
    {
        Text = "Flux indisponible",
        BackgroundColor = SKColors.Black,
        TextColor = SKColors.White
    }
};

var source = new FallbackSwitchSourceBlock(mainSettings, fallbackSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source vidéo virtuelle

Le `VirtualVideoSourceBlock` produit des données vidéo de test dans une grande variété de formats vidéo. Le type de données de test est contrôlé par les paramètres.

#### Paramètres

Le `VirtualVideoSourceBlock` se configure via `VirtualVideoSourceSettings`. Propriétés clés :

- `Pattern` (enum `VirtualVideoSourcePattern`) : spécifie le type de mire de test à générer. Voir l'enum `VirtualVideoSourcePattern` ci-dessous pour les motifs disponibles. Par défaut `SMPTE`.
- `Width` (int) : largeur de la vidéo de sortie (par défaut 1280).
- `Height` (int) : hauteur de la vidéo de sortie (par défaut 720).
- `FrameRate` (`VideoFrameRate`) : fréquence d'images de la vidéo de sortie (par défaut 30 fps).
- `Format` (enum `VideoFormatX`) : format de pixel de la vidéo (par défaut `RGB`).
- `ForegroundColor` (`SKColor`) : pour les motifs utilisant une couleur de premier plan (par ex. `SolidColor`), cette propriété la définit (par défaut `SKColors.White`).

Constructeurs :

- `VirtualVideoSourceSettings()` : constructeur par défaut.
- `VirtualVideoSourceSettings(int width, int height, VideoFrameRate frameRate)` : initialise avec les dimensions et la fréquence d'images spécifiées.

##### Enum `VirtualVideoSourcePattern`

Définit la mire de test générée par `VirtualVideoSourceBlock` :

- `SMPTE` (0) : barres de couleur SMPTE 100 %.
- `Snow` (1) : aléatoire (« neige » télévisuelle).
- `Black` (2) : noir 100 %.
- `White` (3) : blanc 100 %.
- `Red` (4), `Green` (5), `Blue` (6) : couleurs unies.
- `Checkers1` (7) à `Checkers8` (10) : motifs en damier avec des carrés de 1, 2, 4 ou 8 pixels.
- `Circular` (11) : motif circulaire.
- `Blink` (12) : motif clignotant.
- `SMPTE75` (13) : barres de couleur SMPTE 75 %.
- `ZonePlate` (14) : zone plate.
- `Gamut` (15) : damier de gamut.
- `ChromaZonePlate` (16) : zone plate chroma.
- `SolidColor` (17) : couleur unie, définie par `ForegroundColor`.
- `Ball` (18) : balle mobile.
- `SMPTE100` (19) : alias pour les barres de couleur SMPTE 100 %.
- `Bar` (20) : motif de barres.
- `Pinwheel` (21) : motif en moulinet.
- `Spokes` (22) : motif en rayons.
- `Gradient` (23) : motif de dégradé.
- `Colors` (24) : motif de couleurs variées.
- `SMPTERP219` (25) : mire de test SMPTE conforme RP 219.

#### Informations sur le bloc

Nom : VirtualVideoSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    VirtualVideoSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
                      
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSourceBlock.Output, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source audio virtuelle

Le `VirtualAudioSourceBlock` produit des données audio de test dans une grande variété de formats audio. Le type de données de test est contrôlé par les paramètres.

#### Paramètres

Le `VirtualAudioSourceBlock` se configure via `VirtualAudioSourceSettings`. Propriétés clés :

- `Wave` (enum `VirtualAudioSourceSettingsWave`) : spécifie le type de forme d'onde audio à générer. Voir l'enum `VirtualAudioSourceSettingsWave` ci-dessous. Par défaut `Sine`.
- `Format` (enum `AudioFormatX`) : format d'échantillon audio (par défaut `S16LE`).
- `SampleRate` (int) : fréquence d'échantillonnage en Hz (par défaut 48000).
- `Channels` (int) : nombre de canaux audio (par défaut 2).
- `Volume` (double) : volume du signal de test (de 0,0 à 1,0, par défaut 0,8).
- `Frequency` (double) : fréquence du signal de test en Hz (par ex. pour une onde sinusoïdale, par défaut 440).
- `IsLive` (bool) : indique si la source est en direct (par défaut `true`).
- `ApplyTickRamp` (bool) : applique une rampe aux échantillons de tick (par défaut `false`).
- `CanActivatePull` (bool) : peut s'activer en mode pull (par défaut `false`).
- `CanActivatePush` (bool) : peut s'activer en mode push (par défaut `true`).
- `MarkerTickPeriod` (uint) : fait du Nième tick un tick marqueur (pour l'onde `Ticks`, 0 = aucun marqueur, par défaut 0).
- `MarkerTickVolume` (double) : volume des ticks marqueurs (par défaut 1,0).
- `SamplesPerBuffer` (int) : nombre d'échantillons dans chaque tampon sortant (par défaut 1024).
- `SinePeriodsPerTick` (uint) : nombre de périodes de l'onde sinusoïdale dans un tick (pour l'onde `Ticks`, par défaut 10).
- `TickInterval` (`TimeSpan`) : distance entre le début du tick courant et le début du suivant (par défaut 1 seconde).
- `TimestampOffset` (`TimeSpan`) : décalage ajouté aux horodatages (par défaut `TimeSpan.Zero`).

Constructeur :

- `VirtualAudioSourceSettings(VirtualAudioSourceSettingsWave wave = VirtualAudioSourceSettingsWave.Ticks, int sampleRate = 48000, int channels = 2, AudioFormatX format = AudioFormatX.S16LE)`

##### Enum `VirtualAudioSourceSettingsWave`

Définit la forme d'onde pour `VirtualAudioSourceBlock` :

- `Sine` (0) : onde sinusoïdale.
- `Square` (1) : onde carrée.
- `Saw` (2) : onde en dents de scie.
- `Triangle` (3) : onde triangulaire.
- `Silence` (4) : silence.
- `WhiteNoise` (5) : bruit blanc uniforme.
- `PinkNoise` (6) : bruit rose.
- `SineTable` (7) : table sinus.
- `Ticks` (8) : ticks périodiques.
- `GaussianNoise` (9) : bruit blanc gaussien.
- `RedNoise` (10) : bruit rouge (brownien).
- `BlueNoise` (11) : bruit bleu.
- `VioletNoise` (12) : bruit violet.

#### Informations sur le bloc

Nom : VirtualAudioSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Audio en sortie | audio non compressé | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    VirtualAudioSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
                      
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSourceBlock.Output, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source démultiplexeur

Le bloc source démultiplexeur sert à démultiplexer les fichiers multimédias locaux en leurs flux élémentaires constitutifs (vidéo, audio, sous-titres). Il permet le rendu sélectif de ces flux.

#### Informations sur le bloc

Nom : DemuxerSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|-----------------|:--------------------:|:-----------:|
| Vidéo en sortie | Dépend du fichier | 0 ou 1      |
| Audio en sortie | Dépend du fichier | 0 ou 1+     |
| Sous-titres en sortie | Dépend du fichier | 0 ou 1+     |

#### Exemple de pipeline

```mermaid
graph LR;
    DemuxerSourceBlock -- Flux vidéo --> VideoRendererBlock;
    DemuxerSourceBlock -- Flux audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer les paramètres ; veillez à awaiter CreateAsync
var demuxerSettings = await DemuxerSourceSettings.CreateAsync(
    "path/to/your/video.mp4", 
    renderVideo: true, 
    renderAudio: true, 
    renderSubtitle: false);

if (demuxerSettings == null)
{
    Console.WriteLine("Échec de l'initialisation des paramètres du démultiplexeur. Vérifiez que le fichier existe et est lisible.");
    return;
}

var demuxerSource = new DemuxerSourceBlock(demuxerSettings);

// Configurer le rendu vidéo si la vidéo est disponible et rendue
if (demuxerSettings.RenderVideo && demuxerSource.VideoOutput != null)
{
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant que VideoView1 est votre contrôle d'affichage
    pipeline.Connect(demuxerSource.VideoOutput, videoRenderer.Input);
}

// Configurer le rendu audio si l'audio est disponible et rendu
if (demuxerSettings.RenderAudio && demuxerSource.AudioOutput != null)
{
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(demuxerSource.AudioOutput, audioRenderer.Input);
}

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Exemples d'applications

- Aucun lien d'application d'exemple spécifique, mais utilisable dans des scénarios de type lecteur.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source d'image vidéo

Le bloc source d'image vidéo génère un flux vidéo à partir d'un fichier d'image statique (par ex. JPG, PNG). Il restitue l'image à plusieurs reprises sous forme d'images vidéo selon la fréquence d'images spécifiée.

#### Informations sur le bloc

Nom : ImageVideoSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    ImageVideoSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer les paramètres de la source d'image vidéo
var imageSourceSettings = new ImageVideoSourceSettings("path/to/your/image.jpg"); // Remplacez par le chemin de votre image
imageSourceSettings.FrameRate = new VideoFrameRate(10); // Produire 10 images par seconde
imageSourceSettings.IsLive = true; // Traiter comme une source en direct (facultatif)
// imageSourceSettings.NumBuffers = 100; // Facultatif : produire seulement 100 images puis s'arrêter

var imageSource = new ImageVideoSourceBlock(imageSourceSettings);

// Créer le moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant que VideoView1 est votre contrôle d'affichage

// Connecter les blocs
pipeline.Connect(imageSource.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Remarques

Ce bloc utilise SkiaSharp pour le décodage d'image ; veillez à ce que les dépendances nécessaires soient satisfaites si vous n'utilisez pas un paquet VisioForge standard qui l'inclut.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source de superposition de texte

Le `TextOverlaySourceBlock` génère un flux vidéo affichant un texte configurable sur un fond uni. Il utilise en interne les éléments GStreamer `videotestsrc` et `textoverlay`. Ce bloc est utile comme source de remplacement ou de repli — par exemple pour afficher « Aucun signal » lorsqu'une caméra principale est indisponible.

#### Informations sur le bloc

Nom : TextOverlaySourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |

#### Paramètres

Le bloc se configure via `StaticTextFallbackSettings` :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Text` | `string` | `"Source Unavailable"` | Texte à afficher |
| `FontFamily` | `string` | `"Arial"` | Nom de la famille de polices |
| `FontSize` | `float` | `24` | Taille de la police en points |
| `FontStyle` | `FontStyle` | `Normal` | Style de la police (Normal, Bold, Italic, etc.) |
| `TextColor` | `SKColor` | Blanc | Couleur du texte |
| `BackgroundColor` | `SKColor` | Noir | Couleur de remplissage du fond |
| `Position` | `SKPoint` | `(0.5, 0.5)` | Position d'ancrage du texte en proportion (0–1) |
| `CenterAlign` | `bool` | `true` | Centrer le texte |

#### Exemple de pipeline

```mermaid
graph LR;
    TextOverlaySourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new StaticTextFallbackSettings
{
    Text = "Aucun signal",
    FontSize = 36,
    TextColor = SKColors.White,
    BackgroundColor = SKColors.Black
};

var textSource = new TextOverlaySourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(textSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source mélangeur vidéo

Le `VideoMixerSourceBlock` compose plusieurs sources vidéo en un seul flux de sortie. Chaque entrée est placée dans un rectangle configurable sur le canevas de sortie. Pendant la lecture, le bloc prend en charge le repositionnement en direct, le fondu d'opacité et la suppression par incrustation par chrominance par entrée.

Le backend du mélangeur est contrôlé par `VideoMixerType` : CPU (par défaut), OpenGL ou Direct3D 11.

#### Informations sur le bloc

Nom : VideoMixerSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |

#### Paramètres (`VideoMixerSourceSettings`)

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Width` | `int` | `1920` | Largeur du canevas de sortie en pixels |
| `Height` | `int` | `1080` | Hauteur du canevas de sortie en pixels |
| `FrameRate` | `VideoFrameRate` | `FPS_30` | Fréquence d'images en sortie |
| `MixerType` | `VideoMixerType` | `CPU` | Backend de rendu (CPU, OpenGL, D3D11) |

Utilisez `settings.Add(source, rect)` ou `settings.Add(source, left, top, width, height)` pour enregistrer chaque source d'entrée et son rectangle englobant sur le canevas.

#### Contrôle à l'exécution

Le bloc implémente `IVideoMixerControl` pour les mises à jour en direct :

- `Input_Move(id, rect, duration, startAlpha, endAlpha)` — anime la position et l'opacité
- `Input_Update(stream)` — met à jour les propriétés du flux
- `Input_UpdateChromaKeySettings(id, settings)` — modifie l'incrustation par chrominance
- `StartFadeIn(id, duration)` / `StartFadeOut(id, duration)` — contrôles de fondu
- `Input_List()` — énumère tous les flux d'entrée

#### Exemple de pipeline

```mermaid
graph LR;
    Source1-->VideoMixerSourceBlock;
    Source2-->VideoMixerSourceBlock;
    VideoMixerSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new VideoMixerSourceSettings(1920, 1080, VideoFrameRate.FPS_30);

// Ajouter une source fichier sur la moitié gauche
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("source1.mp4"));
settings.Add(fileSource, new Rect(0, 0, 960, 1080));

// Ajouter une webcam sur la moitié droite
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);
settings.Add(videoSource, new Rect(960, 0, 960, 1080));

var mixer = new VideoMixerSourceBlock(pipeline, settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixer.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

`VideoMixerSourceBlock.IsAvailable()` renvoie `true` si les éléments compositeurs GStreamer requis sont disponibles.

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source WebView2

Le `WebView2SourceBlock` restitue une surface de navigateur Microsoft Edge WebView2 sous forme de source vidéo en direct. Il est utile pour capturer des pages web, des superpositions HTML, des tableaux de bord ou du contenu interactif sous forme de flux vidéo. Windows uniquement — il nécessite le runtime Microsoft Edge WebView2 et le plugin GStreamer `webview2`.

#### Informations sur le bloc

Nom : WebView2SourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Sortie vidéo | Vidéo non compressée | 1 |

#### Paramètres (WebView2SourceSettings)

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Location` | `string` | `"about:blank"` | URL à afficher. |
| `JavaScript` | `string` | `null` | JavaScript à exécuter lorsque la navigation est terminée. |
| `Adapter` | `int` | `-1` | Index de l'adaptateur DXGI (`-1` pour n'importe quel périphérique). |
| `ProcessingDeadline` | `ulong` | `20000000` | Temps de traitement maximal pour un tampon, en nanosecondes. |
| `UserDataFolder` | `string` | `null` | Chemin absolu vers le dossier de données utilisateur WebView2. |

#### Exemple de pipeline

```mermaid
graph LR;
    WebView2SourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// créer les paramètres de la source (passer l'URL via le constructeur)
var settings = new WebView2SourceSettings("https://www.visioforge.com/");

// créer le bloc source
var webViewSource = new WebView2SourceBlock(settings);

// créer le bloc de rendu vidéo et le connecter au bloc source
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(webViewSource.Output, videoRenderer.Input);

// exécuter le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

`WebView2SourceBlock.IsAvailable()` renvoie `true` si le plugin GStreamer `webview2` est installé.

#### Plateformes

Windows.

## Blocs de sources push

Les blocs de sources push vous permettent d'alimenter directement le pipeline Media Blocks avec des données multimédias (vidéo, audio, images JPEG ou données génériques) depuis le code de votre application. C'est utile lorsque vos médias proviennent d'une source personnalisée, telle qu'un périphérique de capture propriétaire, un flux réseau non pris en charge par les blocs intégrés ou un contenu généré procéduralement.

Le comportement des sources push est généralement contrôlé par des paramètres communs accessibles via l'interface `IPushSourceSettings`, implémentée par des classes de paramètres push spécifiques :

- `IsLive` (bool) : indique si la source est en direct. Les valeurs par défaut varient selon le type (par ex. `true` pour audio/vidéo).
- `DoTimestamp` (bool) : si `true`, le bloc tente de générer des horodatages pour les données poussées.
- `StreamType` (enum `PushSourceStreamType` : `Stream` ou `SeekableStream`) : définit les caractéristiques du flux.
- `PushFormat` (enum `PushSourceFormat` : `Bytes`, `Time`, `Default`, `Automatic`) : contrôle la manière dont les données sont poussées (par ex. en fonction du nombre d'octets ou du temps).
- `BlockPushData` (bool) : si `true`, l'opération de push se bloque jusqu'à ce que les données soient consommées par le pipeline.

Le type spécifique de source push est déterminé par l'enum `PushSourceType` : `Video`, `Audio`, `Data`, `JPEG`.

### Bloc source push vidéo

Permet de pousser des images vidéo brutes dans le pipeline.

#### Informations sur le bloc

Nom : PushVideoSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Paramètres

Configuré via `PushVideoSourceSettings` :

- `Width` (int) : largeur des images vidéo.
- `Height` (int) : hauteur des images vidéo.
- `FrameRate` (`VideoFrameRate`) : fréquence d'images de la vidéo.
- `Format` (enum `VideoFormatX`) : format de pixel des images vidéo (par ex. `RGB`, `NV12`, `I420`).
- Hérite des paramètres push communs tels que `IsLive` (par défaut `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructeur : `PushVideoSourceSettings(int width, int height, VideoFrameRate frameRate, VideoFormatX format = VideoFormatX.RGB)`

#### Exemple de pipeline

```mermaid
graph LR;
    PushVideoSourceBlock-->VideoEncoderBlock-->MP4SinkBlock;
    PushVideoSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer la source push vidéo
var videoPushSettings = new PushVideoSourceSettings(
    width: 640, 
    height: 480, 
    frameRate: new VideoFrameRate(30), 
    format: VideoFormatX.RGB);
// videoPushSettings.IsLive = true; // Par défaut

var videoPushSource = new PushVideoSourceBlock(videoPushSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoPushSource.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Pousser des images depuis votre application :
// var frame = new VideoFrameX(...);
// videoPushSource.PushFrame(frame);
// videoPushSource.ClearQueue(); // Vider les images mises en tampon si nécessaire
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source push audio

Permet de pousser des échantillons audio bruts dans le pipeline.

#### Informations sur le bloc

Nom : PushAudioSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Audio en sortie  | Audio non compressé | 1          |

#### Paramètres

Configuré via `PushAudioSourceSettings` :

- `SampleRate` (int) : fréquence d'échantillonnage de l'audio (par ex. 44100, 48000).
- `Channels` (int) : nombre de canaux audio (par ex. 1 pour mono, 2 pour stéréo).
- `Format` (enum `AudioFormatX`) : format des échantillons audio (par ex. `S16LE` pour du PCM 16 bits signé little-endian).
- Hérite des paramètres push communs tels que `IsLive` (par défaut `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructeur : `PushAudioSourceSettings(bool isLive = true, int sampleRate = 48000, int channels = 2, AudioFormatX format = AudioFormatX.S16LE)`

#### Exemple de pipeline

```mermaid
graph LR;
    PushAudioSourceBlock-->AudioEncoderBlock-->MP4SinkBlock;
    PushAudioSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer la source push audio
var audioPushSettings = new PushAudioSourceSettings(
    isLive: true, 
    sampleRate: 44100, 
    channels: 2, 
    format: AudioFormatX.S16LE);

var audioPushSource = new PushAudioSourceBlock(audioPushSettings);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioPushSource.Output, audioRenderer.Input);

await pipeline.StartAsync();

// Pousser des données audio depuis votre application :
// var frame = new AudioFrame(...);
// audioPushSource.PushData(frame);
// Ou des octets bruts : audioPushSource.PushData(audioData, audioData.Length);
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source push de données

Permet de pousser des données binaires génériques dans le pipeline. L'interprétation de ces données dépend des `Caps` (capacités) spécifiées.

#### Informations sur le bloc

Nom : `PushSourceBlock` (configuré pour les données).

| Direction du pin | Type de média | Nombre de pins |
|---------------|:----------:|:----------:|
| Données en sortie   | Personnalisé     | 1          |

#### Paramètres

Configuré via `PushDataSourceSettings` :

- `Caps` (`Gst.Caps`) : chaîne de capacités GStreamer décrivant le format des données (par ex. "video/x-h264, stream-format=byte-stream"). Ceci est essentiel pour que les blocs en aval comprennent les données.
- `PadMediaType` (enum `MediaBlockPadMediaType`) : spécifie le type du pad de sortie (par ex. `Video`, `Audio`, `Data`, `Auto`).
- Hérite des paramètres push communs tels que `IsLive`, `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

#### Exemple de pipeline

```mermaid
graph LR;
    PushDataSourceBlock-->ParserOrDecoder-->Renderer;
```

Exemple : pousser un flux binaire H.264 Annex B

```mermaid
graph LR;
    PushDataSourceBlock-->H264ParseBlock-->H264DecoderBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer la source push de données pour un flux binaire H.264
var dataPushSettings = new PushDataSourceSettings();
dataPushSettings.Caps = new Gst.Caps("video/x-h264, stream-format=(string)byte-stream");
dataPushSettings.PadMediaType = MediaBlockPadMediaType.Video;
// dataPushSettings.IsLive = true; // À définir si en direct

var dataPushSource = new PushSourceBlock(dataPushSettings);

// Exemple : décoder et restituer un flux H.264
var h264Parser = new H264ParseBlock();
var h264Decoder = new H264DecoderBlock(); // Décodeur H.264 (sélectionne automatiquement un IH264DecoderSettings concret)
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(dataPushSource.Output, h264Parser.Input);
pipeline.Connect(h264Parser.Output, h264Decoder.Input);
pipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();

// Dans un thread ou une tâche séparée, pousser des NALU H.264 :
// byte[] naluData = ... ; // Vos données de NALU H.264
// dataPushSource.PushFrame(naluData);
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source push JPEG

Permet de pousser des images JPEG individuelles, qui sont ensuite produites sous forme de flux vidéo.

#### Informations sur le bloc

Nom : `PushSourceBlock` (configuré pour le JPEG).

| Direction du pin | Type de média | Nombre de pins |
|---------------|:--------------------:|:----------:|
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Paramètres

Configuré via `PushJPEGSourceSettings` :

- `Width` (int) : largeur des images JPEG décodées.
- `Height` (int) : hauteur des images JPEG décodées.
- `FrameRate` (`VideoFrameRate`) : fréquence d'images à laquelle les images JPEG sont présentées sous forme de flux vidéo.
- Hérite des paramètres push communs tels que `IsLive` (par défaut `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructeur : `PushJPEGSourceSettings(int width, int height, VideoFrameRate frameRate)`

#### Exemple de pipeline

```mermaid
graph LR;
    PushJPEGSourceBlock-->VideoRendererBlock;
    PushJPEGSourceBlock-->VideoEncoderBlock-->MP4SinkBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer la source push JPEG
var jpegPushSettings = new PushJPEGSourceSettings(
    width: 1280, 
    height: 720, 
    frameRate: new VideoFrameRate(10)); // Présenter les JPEG sous forme de vidéo à 10 FPS

var jpegPushSource = new PushSourceBlock(jpegPushSettings);

// Exemple : restituer le flux vidéo issu des JPEG
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(jpegPushSource.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();

// Dans un thread ou une tâche séparée, pousser les données d'image JPEG :
// byte[] jpegImageData = File.ReadAllBytes("image.jpg");
// jpegPushSource.PushFrame(jpegImageData); 
// Appelez PushFrame pour chaque nouvelle image JPEG.
```

#### Plateformes

Windows, macOS, Linux, iOS, Android.

### Bloc source push H264

Le bloc source push H264 permet de pousser des données encodées H.264 brutes directement dans un pipeline Media Blocks pour décodage et rendu. Il enveloppe un élément GStreamer `appsrc` configuré pour une entrée en byte-stream H.264.

Ce bloc est conçu pour les scénarios où vous recevez des données H.264 encodées brutes depuis une source externe (par exemple un callback RTSP, un transport réseau personnalisé ou un lecteur de fichier) et devez les décoder et les afficher. Il gère automatiquement :

- **Conversion AVC vers byte-stream** : RTSP et de nombreuses autres sources livrent l'H.264 au format AVC (unités NAL préfixées par une longueur de 4 octets). Le bloc convertit automatiquement ce format vers le format Annex B byte-stream (codes de démarrage) attendu par l'analyseur H.264 de GStreamer.
- **Rebasement des PTS** : les horodatages de présentation issus de sources externes (par ex. caméras RTSP) sont sur la base de temps de la source, qui peut être loin de zéro. Le bloc rebase tous les horodatages par rapport à la première image, afin que l'horloge du pipeline démarre près de zéro, évitant ainsi les délais d'affichage.

#### Informations sur le bloc

Nom : H264PushSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | H.264 encodé (byte-stream) | 1 |

#### Propriétés

- `DoTimestamp` (bool) : si `true`, l'appsrc génère automatiquement les horodatages en utilisant l'horloge du pipeline (latence minimale, mais casse la synchronisation A/V). Si `false` (par défaut), le PTS fourni à `PushData()` est utilisé après rebasement. Doit être défini avant le démarrage du pipeline.

#### Méthodes

- `PushData(byte[] data, int length, TimeSpan timestamp)` : pousse des données H.264 encodées avec un horodatage de présentation. Gère automatiquement la conversion AVC vers byte-stream et le rebasement des PTS. Renvoie un statut `FlowReturn`.
- `PushData(byte[] data, int length)` : pousse des données H.264 sans horodatage (utilise `TimeSpan.Zero`).
- `SendEOS()` : signale la fin de flux au pipeline.

#### Exemple de pipeline

Le pipeline typique connecte H264PushSourceBlock via un analyseur et un décodeur jusqu'à un moteur de rendu vidéo :

```mermaid
graph LR;
    H264PushSourceBlock-->H264ParseBlock;
    H264ParseBlock-->H264DecoderBlock;
    H264DecoderBlock-->VideoRendererBlock;
```

#### Architecture à deux pipelines avec callback RTSP

Un cas d'usage courant est le motif à deux pipelines : un pipeline capture le H.264 brut depuis une caméra RTSP, et un second pipeline le décode et l'affiche. Cela vous donne accès aux octets encodés bruts entre la capture et le décodage.

```mermaid
graph LR;
    subgraph "Pipeline 1 — Grabber"
        RTSPRAWSourceBlock-->BufferSinkBlock;
    end
    subgraph "Pipeline 2 — Lecteur"
        H264PushSourceBlock-->H264ParseBlock;
        H264ParseBlock-->H264DecoderBlock;
        H264DecoderBlock-->VideoRendererBlock;
    end
    BufferSinkBlock-.Callback PushData.->H264PushSourceBlock;
```

#### Exemple de code

```csharp
// === Pipeline 2 : Lecteur (décodage et rendu) ===
var playerPipeline = new MediaBlocksPipeline();

// Créer la source push H264 — c'est là que les données H.264 brutes seront injectées
var h264Source = new H264PushSourceBlock();

// Créer l'analyseur, le décodeur et le moteur de rendu
var h264Parser = new H264ParseBlock();
var h264Decoder = new H264DecoderBlock(new FFMPEGH264DecoderSettings());
var videoRenderer = new VideoRendererBlock(playerPipeline, VideoView1);

// Connecter : source push -> analyseur -> décodeur -> moteur de rendu
playerPipeline.Connect(h264Source.Output, h264Parser.Input);
playerPipeline.Connect(h264Parser.Output, h264Decoder.Input);
playerPipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Précharger le pipeline lecteur (crée les éléments mais reste en état PAUSED)
await playerPipeline.StartAsync(onlyPreload: true);

// === Pipeline 1 : Grabber (capture RTSP) ===
var grabberPipeline = new MediaBlocksPipeline();

var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://admin:password@192.168.1.64:554/stream"),
    "admin", "password");
rtspSettings.Latency = 50;

var rtspSource = new RTSPRAWSourceBlock(rtspSettings);
var bufferSink = new BufferSinkBlock();

// Câbler le callback pour pousser les données H.264 brutes dans le pipeline lecteur
bufferSink.OnDataFrameBuffer += (sender, args) =>
{
    // Copier les données H.264 brutes depuis la mémoire non managée
    var data = new byte[args.Size];
    System.Runtime.InteropServices.Marshal.Copy(args.Data, data, 0, args.Size);

    // Pousser dans le pipeline lecteur (la conversion AVC vers byte-stream est automatique)
    h264Source.PushData(data, args.Size, args.Timestamp);
};

grabberPipeline.Connect(rtspSource.Output, bufferSink.Input);

// Démarrer le grabber (établit la connexion RTSP et la livraison des images H.264)
await grabberPipeline.StartAsync();

// Attendre l'arrivée des données initiales, puis reprendre le lecteur
await Task.Delay(1000);
await playerPipeline.ResumeAsync();
```

#### Séquence de démarrage du pipeline

L'ordre de démarrage est crucial pour le streaming en direct avec deux pipelines :

1. **Créer et connecter** les blocs du pipeline lecteur.
2. **Précharger** le pipeline lecteur (`StartAsync(onlyPreload: true)`) — transition vers PAUSED, création des éléments GStreamer et liaison des pads.
3. **Créer et démarrer** le pipeline grabber — la connexion RTSP est établie, les images H.264 arrivent via le callback.
4. **Pousser les données** dans le pipeline lecteur en pause — `h264parse` accumule les jeux de paramètres SPS/PPS.
5. **Reprendre** le pipeline lecteur (`ResumeAsync()`) — transition vers PLAYING, le décodeur commence à produire des images.

#### Format AVC vs byte-stream

Les sources RTSP livrent typiquement le H.264 au format AVC où chaque unité NAL est préfixée d'une longueur de 4 octets en big-endian :

```text
AVC:         [00 00 00 17][NAL data...][00 00 1C CC][NAL data...]
              length=23                 length=7372
```

L'élément `h264parse` de GStreamer attend le format Annex B byte-stream avec des codes de démarrage :

```text
Byte-stream: [00 00 00 01][NAL data...][00 00 00 01][NAL data...]
              start code               start code
```

La méthode `PushData()` convertit automatiquement AVC vers byte-stream sur place (les deux utilisent des préfixes de 4 octets, donc aucune réallocation de tampon n'est nécessaire). Les données déjà au format byte-stream sont détectées et transmises sans modification.

#### Exemples d'applications

- [Démo callback caméra H264 RTSP brut (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20RAW%20Camera%20H264%20Callback%20Demo)

#### Plateformes

Windows, macOS, Linux.

## Blocs de sources des plateformes Apple

### Bloc source vidéo iOS

`iOSVideoSourceBlock` fournit la capture vidéo depuis la caméra du périphérique sur les plateformes iOS. Disponible uniquement sur iOS (non sur macOS Catalyst).

#### Informations sur le bloc

Nom : IOSVideoSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:------------------:|:----------:|
| Vidéo en sortie  | Vidéo non compressée | 1          |

#### Énumération des périphériques disponibles

Utilisez `DeviceEnumerator.Shared.VideoSourcesAsync()` pour obtenir la liste des périphériques vidéo disponibles sur iOS. Chaque périphérique est représenté par un objet `VideoCaptureDeviceInfo`.

#### Exemple de pipeline

```mermaid
graph LR;
    IOSVideoSourceBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// sélectionner le premier périphérique vidéo disponible
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
VideoCaptureDeviceSourceSettings videoSourceSettings = null;
if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };
        videoSourceSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

// créer le bloc source vidéo iOS
var videoSource = new IOSVideoSourceBlock(videoSourceSettings);

// créer le bloc de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connecter les blocs
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// démarrer le pipeline
await pipeline.StartAsync();
```

#### Plateformes

iOS (non disponible sur macOS Catalyst)

---

### Bloc source audio macOS

`OSXAudioSourceBlock` fournit la capture audio depuis les périphériques d'entrée sur les plateformes macOS.

#### Informations sur le bloc

Nom : OSXAudioSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
|---------------|:------------------:|:----------:|
| Audio en sortie  | Audio non compressé | 1          |

#### Énumération des périphériques disponibles

Utilisez `DeviceEnumerator.Shared.AudioSourcesAsync()` pour obtenir la liste des périphériques audio disponibles sur macOS. Chaque périphérique est représenté par un objet `AudioCaptureDeviceInfo`.

#### Exemple de pipeline

```mermaid
graph LR;
    OSXAudioSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
// créer le pipeline
var pipeline = new MediaBlocksPipeline();

// sélectionner le premier périphérique audio disponible
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var device = devices.Length > 0 ? devices[0] : null;
OSXAudioSourceSettings audioSourceSettings = null;
if (device != null)
{
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        audioSourceSettings = new OSXAudioSourceSettings(device.DeviceID, formatItem);
    }
}

// créer le bloc source audio macOS
var audioSource = new OSXAudioSourceBlock(audioSourceSettings);

// créer le bloc de rendu audio
var audioRenderer = new AudioRendererBlock();

// connecter les blocs
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// démarrer le pipeline
await pipeline.StartAsync();
```

#### Plateformes

macOS (non disponible sur iOS)

---

### Bloc source Blu-Ray

Le `BluRaySourceBlock` lit des flux vidéo, audio et sous-titres optionnels depuis un disque Blu-Ray ou un dossier d'image disque Blu-Ray à l'aide de l'élément `bluray` de GStreamer.

#### Informations sur le bloc

Nom : BluRaySourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 0 ou 1 |
| Audio en sortie | Audio non compressé | 0 ou 1 |
| Sous-titres en sortie | Sous-titres | 0 ou 1 |

#### Paramètres

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Location` | `string` | requis | Chemin du périphérique Blu-Ray ou du dossier d'image disque |
| `Title` | `int` | `-1` | Titre à lire (−1 = titre principal/le plus long) |
| `RenderVideo` | `bool` | `true` | Active le pad de sortie vidéo |
| `RenderAudio` | `bool` | `true` | Active le pad de sortie audio |
| `RenderSubtitle` | `bool` | `false` | Active le pad de sortie sous-titres |
| `AudioStream` | `int` | `-1` | Index du flux audio (−1 = par défaut) |
| `SubtitleStream` | `int` | `-1` | Index du flux de sous-titres (−1 = par défaut) |

#### Exemple de pipeline

```mermaid
graph LR;
    BluRaySourceBlock -- Vidéo --> VideoRendererBlock;
    BluRaySourceBlock -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new BluRaySourceSettings("/dev/sr0")
{
    Title = -1,
    RenderVideo = true,
    RenderAudio = true
};

var source = new BluRaySourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = BluRaySourceBlock.IsAvailable();
```

#### Plateformes

Windows, macOS, Linux.

---

### Bloc source DVB

Le `DVBSourceBlock` capture un flux de télévision numérique en direct depuis un adaptateur DVB (DVB-T, DVB-C, DVB-S, DVB-S2 ou ISDB-T). Le bloc accorde le canal spécifié et produit un flux MPEG-TS brut.

#### Informations sur le bloc

Nom : DVBSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Flux MPEG-TS | 1 |

#### Paramètres

`DVBSourceSettings` contient de nombreux paramètres de syntonisation. Propriétés principales :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `AdapterID` | `int` | `0` | Index du périphérique adaptateur DVB |
| `FrontendID` | `int` | `0` | Index du périphérique frontal |
| `DeliverySystem` | `DVBDeliverySystem` | Undefined | DVB-T / DVB-C / DVB-S / DVB-S2 / ISDB-T |
| `Frequency` | `uint` | `0` | Fréquence centrale en Hz (ou kHz pour le satellite) |
| `Pids` | `string` | `"8192"` | Liste de filtres PID (8192 = transport stream complet) |
| `Modulation` | `DVBModulation` | QAM_16 | Schéma de modulation |
| `SymbolRate` | `uint` | `0` | Débit de symboles en kBd (satellite) |
| `BandwidthHZ` | `uint` | `0` | Largeur de bande du canal en Hz (DVB-T) |

D'autres propriétés couvrent l'intervalle de garde, le mode de transmission, la hiérarchie, les taux de code, la configuration LNB, DiSEqC, les paramètres de couches ISDB-T et les délais de syntonisation.

Événements déclenchés pendant la syntonisation : `TuningStart`, `TuningDone`, `TuningFail`.

#### Exemple de pipeline

```mermaid
graph LR;
    DVBSourceBlock-->DecodeBinBlock;
    DecodeBinBlock -- Vidéo --> VideoRendererBlock;
    DecodeBinBlock -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new DVBSourceSettings
{
    AdapterID = 0,
    DeliverySystem = DVBDeliverySystem.DVBT,
    Frequency = 506000000,
    BandwidthHZ = 8000000
};

var dvb = new DVBSourceBlock(settings);
dvb.TuningDone += (s, e) => Console.WriteLine("Syntonisation terminée");

// Décoder le flux MPEG-TS
var decoder = new DecodeBinBlock();
pipeline.Connect(dvb.VideoOutput, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = DVBSourceBlock.IsAvailable();
```

#### Plateformes

Windows, Linux.

---

### Bloc source PulseAudio

Le `PulseAudioSourceBlock` capture l'audio depuis un périphérique PulseAudio sous Linux. Il implémente `IAudioVolumeMute` pour le contrôle du volume et de la sourdine en temps réel.

#### Informations sur le bloc

Nom : PulseAudioSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Audio en sortie | Audio non compressé | 1 |

#### Paramètres

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Device` | `AudioCaptureDeviceInfo` | — | Périphérique de capture (issu de l'énumérateur de périphériques) |
| `Format` | `AudioCaptureDeviceFormat` | — | Format audio (fréquence d'échantillonnage, canaux, profondeur de bit) |
| `DeviceName` | `string` | `null` | Nom du périphérique PulseAudio (null = défaut système) |
| `ClientName` | `string` | `"VisioForge"` | Nom affiché dans les applications de contrôle audio |
| `ProvideClock` | `bool` | `true` | Fournit une horloge pour le pipeline |

#### Exemple de pipeline

```mermaid
graph LR;
    PulseAudioSourceBlock-->AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Énumérer les périphériques PulseAudio
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var device = devices[0];
var format = device.Formats[0];

var settings = new PulseAudioSourceSettings(device, format);
var audioSource = new PulseAudioSourceBlock(settings);

// Facultatif : contrôler le volume à l'exécution
audioSource.SetVolume(0.8);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSource.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = PulseAudioSourceBlock.IsAvailable();
```

#### Plateformes

Linux.

---

### Bloc source Raspberry Pi

Le `RaspberryPiSourceBlock` capture la vidéo depuis le module caméra du Raspberry Pi en utilisant l'élément GStreamer `rpicamsrc`. Il produit de la vidéo encodée H.264 et prend en charge les contrôles de caméra dont la rotation, le retournement et les ajustements d'image.

#### Informations sur le bloc

Nom : RaspberryPiSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo H.264 | 1 |

#### Paramètres

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `CameraNumber` | `int` | `0` | Index de caméra pour les configurations multi-caméras |
| `Width` | `uint` | `1920` | Largeur de capture en pixels |
| `Height` | `uint` | `1080` | Hauteur de capture en pixels |
| `FrameRate` | `VideoFrameRate` | FPS_30 | Fréquence d'images de capture |
| `Bitrate` | `uint` | `17000000` | Débit binaire H.264 en bps |
| `Rotation` | `int` | `0` | Rotation en degrés : 0, 90, 180, 270 |
| `HorizontalFlip` | `bool` | `false` | Retourner l'image horizontalement |
| `VerticalFlip` | `bool` | `false` | Retourner l'image verticalement |
| `Brightness` | `int` | `50` | Luminosité : 0–100 |
| `Contrast` | `int` | `0` | Contraste : −100 à 100 |
| `Saturation` | `int` | `0` | Saturation : −100 à 100 |
| `ISO` | `uint` | `0` | Sensibilité ISO (0 = auto) |
| `ExposureMode` | `string` | `"auto"` | Mode d'exposition (auto, night, sports, etc.) |
| `AWBMode` | `string` | `"auto"` | Mode de balance des blancs automatique |

#### Exemple de pipeline

```mermaid
graph LR;
    RaspberryPiSourceBlock-->H264DecoderBlock;
    H264DecoderBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new RaspberryPiSourceSettings
{
    Width = 1920,
    Height = 1080,
    FrameRate = VideoFrameRate.FPS_30,
    Rotation = 0,
    Bitrate = 17000000
};

var camera = new RaspberryPiSourceBlock(settings);

// Décoder la sortie H.264 avant le rendu
var decoder = new H264DecoderBlock();
pipeline.Connect(camera.Output, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = RaspberryPiSourceBlock.IsAvailable();
```

#### Plateformes

Linux (Raspberry Pi).

---

### Bloc source GStreamer PlayBin

Le `PlayBinSourceBlock` lit les médias en utilisant l'élément `playbin` intégré de GStreamer. Il accepte les mêmes `UniversalSourceSettings` que `UniversalSourceBlock` mais délègue l'intégralité du pipeline de décodage au moteur de lecture haut niveau de GStreamer.

#### Informations sur le bloc

Nom : PlayBinSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 0 ou 1 |
| Audio en sortie | Audio non compressé | 0 ou 1 |
| Sous-titres en sortie | Sous-titres | 0 ou 1 |

#### Paramètres

Utilise `UniversalSourceSettings`. Propriétés clés :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `URI` | `Uri` | requis | URI de la source multimédia |
| `RenderVideo` | `bool` | `true` | Active le pad de sortie vidéo |
| `RenderAudio` | `bool` | `true` | Active le pad de sortie audio |
| `RenderSubtitle` | `bool` | `false` | Active le pad de sortie sous-titres |
| `StartPosition` | `TimeSpan?` | `null` | Position de début de lecture |
| `StopPosition` | `TimeSpan?` | `null` | Position d'arrêt de lecture |
| `VideoCustomFrameRate` | `VideoFrameRate?` | `null` | Remplace la fréquence d'images en sortie |

#### Exemple de pipeline

```mermaid
graph LR;
    PlayBinSourceBlock -- Vidéo --> VideoRendererBlock;
    PlayBinSourceBlock -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await UniversalSourceSettings.CreateAsync(new Uri("file:///video.mp4"));
var source = new PlayBinSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, Android, iOS.

---

### Bloc source fichier basique

Le `BasicFileSourceBlock` lit des données binaires brutes depuis un fichier et les expose sous forme d'un unique pad de sortie. Une étape facultative de détection de type (« type-find ») peut détecter automatiquement le format du flux afin qu'il puisse être connecté à un démultiplexeur ou à un décodeur.

#### Informations sur le bloc

Nom : BasicFileSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Sortie | Auto (données brutes) | 1 |

#### Paramètres

| Paramètre | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `filename` | `string` | — | Chemin du fichier source |
| `addTypeFind` | `bool` | `false` | Ajoute un élément type-find pour la détection automatique du format |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock-->DecodeBinBlock;
    DecodeBinBlock -- Vidéo --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Lire le fichier avec détection automatique du type
var fileSource = new BasicFileSourceBlock("video.ts", addTypeFind: true);

var decoder = new DecodeBinBlock();
pipeline.Connect(fileSource.Output, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = BasicFileSourceBlock.IsAvailable();
```

#### Plateformes

Windows, macOS, Linux, Android, iOS.

---

### Bloc source mélangeur personnalisé

Le `CustomMixerSourceBlock` est une source basée sur le push qui accepte des images vidéo et audio fournies en externe via des files d'attente managées, les exposant sous forme de pads de sortie en direct. Il est conçu pour les pipelines de mixage et de composition personnalisés où les images proviennent du code de l'application.

#### Informations sur le bloc

Nom : CustomMixerSourceBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |
| Audio en sortie | Audio non compressé | 1 |

#### Paramètres

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Width` | `int` | `1920` | Largeur vidéo en pixels |
| `Height` | `int` | `1080` | Hauteur vidéo en pixels |
| `VideoFormat` | `VideoFormatX` | BGRA | Format de pixel vidéo |
| `FrameRate` | `VideoFrameRate` | 30 fps | Fréquence d'images en sortie |
| `AudioFormat` | `AudioFormatX` | S16LE | Format d'échantillon audio |
| `AudioSampleRate` | `int` | `48000` | Fréquence d'échantillonnage audio en Hz |
| `AudioChannels` | `int` | `2` | Nombre de canaux audio |
| `IsLive` | `bool` | `true` | Marquer la source comme live |
| `DoTimestamp` | `bool` | `true` | Horodater automatiquement les tampons poussés |
| `EnableThreadedPush` | `bool` | `true` | Pousser les images depuis un thread dédié |
| `ManagedQueueMaxBuffers` | `int` | `4` | Nombre maximal d'images en file d'attente avant abandon |

#### Événements

- `QueueDropped` — déclenché lorsque la file d'attente managée abandonne un tampon en raison de la backpressure.

#### Exemple de pipeline

```mermaid
graph LR;
    CustomMixerSourceBlock -- Vidéo --> VideoRendererBlock;
    CustomMixerSourceBlock -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new CustomMixerSourceSettings
{
    Width = 1280,
    Height = 720,
    FrameRate = VideoFrameRate.FPS_30,
    AudioSampleRate = 48000,
    AudioChannels = 2
};

var mixerSource = new CustomMixerSourceBlock(settings);
mixerSource.QueueDropped += (s, e) => Console.WriteLine("Tampon abandonné");

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixerSource.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = CustomMixerSourceBlock.IsAvailable();
```

#### Plateformes

Windows, macOS, Linux, Android, iOS.

---

### Bloc source image vidéo Cairo

Le `ImageVideoSourceCairoBlock` restitue une image statique ou animée (y compris des GIF animés) sous forme de flux vidéo continu via la bibliothèque graphique 2D Cairo. Contrairement à la source d'image standard, la taille de l'image de sortie est spécifiée explicitement via un paramètre de constructeur `Size`.

#### Informations sur le bloc

Nom : ImageVideoSourceCairoBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 1 |

#### Paramètres du constructeur

| Paramètre | Type | Description |
| --- | --- | --- |
| `pipeline` | `MediaBlocksPipeline` | Pipeline propriétaire |
| `settings` | `ImageVideoSourceSettings` | Paramètres de la source d'image |
| `videoSize` | `Size` | Dimensions de l'image de sortie en pixels |

#### Paramètres (ImageVideoSourceSettings)

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Filename` | `string` | requis | Chemin du fichier image |
| `FrameRate` | `VideoFrameRate` | 10 fps | Fréquence d'images en sortie |
| `IsLive` | `bool` | `false` | Marque la source comme live |
| `NumBuffers` | `int` | `-1` | Nombre d'images à produire (−1 = illimité) |
| `AnimationInfiniteLoop` | `bool` | `true` | Boucler indéfiniment les images animées |

#### Exemple de pipeline

```mermaid
graph LR;
    ImageVideoSourceCairoBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageVideoSourceSettings("image.png")
{
    FrameRate = VideoFrameRate.FPS_30,
    IsLive = true
};

var imageSource = new ImageVideoSourceCairoBlock(pipeline, settings, new Size(1920, 1080));

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux.

---

### Bloc source universel Mini

Le `UniversalSourceBlockMini` est une variante légère de `UniversalSourceBlock` utilisant les mêmes `UniversalSourceSettings`. Il s'appuie sur un pipeline interne simplifié, adapté aux environnements à faibles ressources ou aux scénarios où la surcharge du moteur de décodage complet n'est pas souhaitable.

#### Informations sur le bloc

Nom : UniversalSourceBlockMini.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 0 ou 1 |
| Audio en sortie | Audio non compressé | 0 ou 1 |
| Sous-titres en sortie | Sous-titres | 0 ou 1 |

#### Paramètres

Utilise `UniversalSourceSettings` — la même classe que `UniversalSourceBlock`.

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `URI` | `Uri` | requis | URI de la source multimédia |
| `RenderVideo` | `bool` | `true` | Active la sortie vidéo |
| `RenderAudio` | `bool` | `true` | Active la sortie audio |
| `RenderSubtitle` | `bool` | `false` | Active la sortie sous-titres |
| `StartPosition` | `TimeSpan?` | `null` | Position de début de lecture |
| `StopPosition` | `TimeSpan?` | `null` | Position d'arrêt de lecture |

#### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlockMini -- Vidéo --> VideoRendererBlock;
    UniversalSourceBlockMini -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await UniversalSourceSettings.CreateAsync(new Uri("file:///video.mp4"));
var source = new UniversalSourceBlockMini(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

```csharp
bool available = UniversalSourceBlockMini.IsAvailable();
```

#### Plateformes

Windows, macOS, Linux, Android, iOS.

---

### Bloc source universel V2

Le `UniversalSourceBlockV2` est une source multimédia multi-flux avancée qui expose des listes de pads de sortie distinctes pour les flux vidéo, audio, sous-titres et métadonnées. Il convient aux fichiers multimédias complexes contenant plusieurs pistes et aux scénarios nécessitant un contrôle du décodeur par flux.

#### Informations sur le bloc

Nom : UniversalSourceBlockV2.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en sortie | Vidéo non compressée | 0…N |
| Audio en sortie | Audio non compressé | 0…N |
| Sous-titres en sortie | Sous-titres | 0…N |
| Métadonnées en sortie | Métadonnées | 0…N |

#### Paramètres (UniversalSourceSettingsV2)

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `URI` | `Uri` | requis | URI de la source multimédia |
| `RenderVideo` | `bool` | `true` | Active les pads de sortie vidéo |
| `RenderAudio` | `bool` | `true` | Active les pads de sortie audio |
| `RenderSubtitle` | `bool` | `false` | Active les pads de sortie sous-titres |
| `StartPosition` | `TimeSpan?` | `null` | Position de début de lecture |
| `StopPosition` | `TimeSpan?` | `null` | Position d'arrêt de lecture |
| `VideoCustomFrameRate` | `VideoFrameRate?` | `null` | Remplace la fréquence d'images en sortie |

#### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlockV2 -- Vidéo --> VideoRendererBlock;
    UniversalSourceBlockV2 -- Audio --> AudioRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new UniversalSourceSettingsV2(new Uri("file:///multi_track.mkv"))
{
    RenderVideo = true,
    RenderAudio = true
};

var source = new UniversalSourceBlockV2(settings);

// Accéder aux premiers flux vidéo et audio
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plateformes

Windows, macOS, Linux, Android, iOS.
