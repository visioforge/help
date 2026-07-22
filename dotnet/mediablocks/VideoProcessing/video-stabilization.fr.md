---
title: Bloc de stabilisation vidéo en temps réel en C# .NET
description: Supprimez les tremblements de caméra sur une vidéo en direct ou enregistrée en C# avec VideoStabilizationBlock (deshake OpenCV) du Media Blocks SDK.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - GStreamer
  - OpenCV
  - Effects
  - Playback
  - C#
primary_api_classes:
  - VideoStabilizationBlock
  - VideoStabilizationSettings
  - UniversalSourceBlock
  - VideoRendererBlock
  - MediaBlocksPipeline

---

# Bloc de stabilisation vidéo

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation

Le `VideoStabilizationBlock` supprime les tremblements de caméra d'un flux vidéo en direct ou enregistré en temps réel. Il estime le mouvement global entre les images (translation et rotation) à l'aide d'un flux optique épars, lisse la trajectoire de caméra obtenue avec une fenêtre causale de moyenne mobile, puis déforme chaque image pour la ramener sur la trajectoire lissée. Un léger zoom central (rapport de recadrage) masque les bords exposés par la compensation.

Le bloc s'appuie sur l'élément GStreamer `vfdeshake` d'OpenCV et nécessite donc le redistribuable OpenCV du SDK. Il est actuellement disponible sous Windows.

La latence est nulle : seules les images passées sont utilisées pour le lissage, de sorte que le bloc peut être utilisé dans des pipelines en direct.

## Fonctionnalités clés

- Estimation du mouvement global (translation + rotation) via `goodFeaturesToTrack` + flux optique pyramidal de Lucas-Kanade et un ajustement affine partiel par RANSAC.
- Lissage causal de la trajectoire avec un rayon de fenêtre configurable, sans latence d'image ajoutée.
- Zoom central automatique (`CropRatio`) pour garder les bords masqués.
- Limites de correction par image (`MaxShift`, `MaxAngle`) comme limiteurs de sécurité.
- Réglage en direct : chaque propriété peut être modifiée pendant l'exécution du pipeline.
- Mode passthrough (`Enabled = false`) pour une comparaison instantanée avant/après.

## Paramètres

`VideoStabilizationSettings` correspond un à un aux propriétés de l'élément :

| Propriété | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `Enabled` | `bool` | `true` | Lorsqu'il vaut `false`, le bloc laisse passer les images sans modification. |
| `SmoothingRadius` | `int` | `15` | Nombre d'images passées moyennées lors du lissage de la trajectoire de caméra. Plus grand = plus stable, réaction plus lente. Plage `1` - `1000`. |
| `CropRatio` | `double` | `0.9` | Fraction de l'image conservée visible. L'image est agrandie de `1 / CropRatio` autour de son centre pour masquer les bords exposés. `1.0` désactive le zoom. Plage `0.5` - `1.0`. |
| `MaxShift` | `int` | `100` | Correction de translation maximale par image en pixels (limiteur de sécurité). Minimum `0`. |
| `MaxAngle` | `double` | `15` | Correction de rotation maximale par image en degrés (limiteur de sécurité). Plage `0` - `90`. |

Toutes les propriétés **sont bornées silencieusement** à leur plage lors de l'affectation : une valeur hors plage est corrigée et non rejetée. Relisez la propriété si vous devez savoir quelle valeur a été appliquée.

## Disponibilité

`IsAvailable()` recherche l'élément `vfdeshake` dans le registre GStreamer : appelez-le donc **après** l'initialisation du SDK :

```csharp
await VisioForgeX.InitSDKAsync();

if (!VideoStabilizationBlock.IsAvailable())
{
    // Ajoutez le package NuGet VisioForge.CrossPlatform.OpenCV.Windows.x64.
}
```

## Exemple de code - prévisualiser un fichier stabilisé

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sources;

// Initialisez le SDK une seule fois au démarrage de l'application avant de construire le moindre pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings
{
    Enabled = true,
    SmoothingRadius = 20,
    CropRatio = 0.9,
});

// VideoView1 est un contrôle VideoView de VisioForge sur votre formulaire/fenêtre.
var renderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, renderer.Input);

await pipeline.StartAsync();
```

## Exemple de code - stabiliser et enregistrer en MP4

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Initialisez le SDK une seule fois au démarrage de l'application, avant de construire un pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings { CropRatio = 0.9 });

var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
var mp4 = new MP4SinkBlock(new MP4SinkSettings(@"C:\Videos\stabilized.mp4"));

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, h264.Input);
pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// Le fichier est lu jusqu'à la fin et le pipeline finalise le MP4 sur EOS : utilisez OnStop pour le savoir.
// Pour arrêter l'enregistrement PLUS TÔT, appelez StopAsync() : cela envoie EOS afin que le multiplexeur écrive l'atome moov.
// Tuer le processus laisse au contraire un fichier illisible.
// await pipeline.StopAsync();
```

## Exemple de code - prévisualiser et enregistrer la sortie stabilisée

Pour visualiser la vidéo stabilisée et l'enregistrer en même temps, dérivez la sortie du stabilisateur avec un `TeeBlock` : une branche alimente le moteur de rendu, l'autre l'encodeur H264 et le puits MP4.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Initialisez le SDK une seule fois au démarrage de l'application, avant de construire un pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings { CropRatio = 0.9 });

var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);
var renderer = new VideoRendererBlock(pipeline, VideoView1);
var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
var mp4 = new MP4SinkBlock(new MP4SinkSettings(@"C:\Videos\stabilized.mp4"));

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, tee.Input);
pipeline.Connect(tee.Outputs[0], renderer.Input);
pipeline.Connect(tee.Outputs[1], h264.Input);
pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// Le fichier est lu jusqu'à la fin et le pipeline finalise le MP4 sur EOS : utilisez OnStop pour le savoir.
// Pour arrêter l'enregistrement PLUS TÔT, appelez StopAsync() : cela envoie EOS afin que le multiplexeur écrive l'atome moov.
// Tuer le processus laisse au contraire un fichier illisible.
// await pipeline.StopAsync();
```

## Utilisation dans les moteurs X

`VideoStabilizationBlock` implémente `IVideoProcessingBlock` : il peut donc être inséré directement dans les moteurs X de haut niveau - `VideoCaptureCoreX` et `MediaPlayerCoreX` - via `Video_Processing_AddBlock()`. Le moteur raccorde le bloc dans son chemin vidéo pour vous, ce qui vous permet de stabiliser une **caméra en direct** et pas seulement un fichier.

Ajoutez le bloc **avant que le moteur ne construise son pipeline**. Pour `VideoCaptureCoreX`, cela signifie avant `StartAsync()` ; pour `MediaPlayerCoreX`, avant `OpenAsync()`, car le lecteur construit son graphe pendant Open : un bloc ajouté ensuite est ignoré (il ne produit qu'un avertissement dans le journal).

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// Initialisez le SDK une seule fois au démarrage de l'application avant de créer le moindre moteur.
await VisioForgeX.InitSDKAsync();

if (!VideoStabilizationBlock.IsAvailable())
{
    // Ajoutez le paquet NuGet VisioForge.CrossPlatform.OpenCV.Windows.x64.
    return;
}

// Énumérez les caméras.
var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
if (cameras.Length == 0)
{
    return;
}

// VideoView1 est un contrôle VideoView de VisioForge sur votre formulaire/fenêtre.
var core = new VideoCaptureCoreX(VideoView1);

// Ce constructeur choisit un format HD (ou le meilleur disponible) et la fréquence d'images du périphérique.
core.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[0]);

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings
{
    SmoothingRadius = 20,
    CropRatio = 0.9,
});

core.Video_Processing_AddBlock(stabilizer);

// StartAsync renvoie false si un bloc n'a pas pu être construit ; elle ne lève pas d'exception.
if (!await core.StartAsync())
{
    // Traitez l'échec : le redistribuable OpenCV ou la caméra peuvent être indisponibles.
}
```

`ApplySettings()` fonctionne exactement de la même manière dans un moteur : modifiez les valeurs de `stabilizer.Settings` et appelez `stabilizer.ApplySettings()` pour réajuster l'effet pendant que la caméra continue de tourner.

!!! warning "Le moteur est propriétaire du bloc"

    Dès que vous passez un bloc à `Video_Processing_AddBlock()`, le moteur en devient propriétaire : il le construit au démarrage et **le libère à l'arrêt de la capture ou de la lecture**. Ne le libérez pas vous-même et ne réutilisez pas la même instance pour une seconde session : créez un nouveau bloc à chaque démarrage. `ApplySettings()` peut être appelée à tout moment (elle ne fait rien une fois le bloc libéré).

## Réglage en direct

L'élément `vfdeshake` relit ses propriétés à chaque image. Modifiez n'importe quelle valeur de l'objet `Settings` et appelez `ApplySettings()` pour réajuster l'effet sans reconstruire le pipeline :

```csharp
stabilizer.Settings.Enabled = false;      // bascule avant/après instantanée
stabilizer.Settings.SmoothingRadius = 30; // plus stable
stabilizer.Settings.CropRatio = 0.85;     // zoome un peu plus
stabilizer.ApplySettings();
```

## Conseils

- Augmentez `SmoothingRadius` pour des résultats plus stables sur des tremblements continus ; réduisez-le si la stabilisation réagit trop lentement aux mouvements de caméra intentionnels.
- Réduisez `CropRatio` (par exemple `0.85`) lorsque le tremblement est important, afin que la compensation plus large reste masquée derrière le zoom. Des valeurs plus élevées conservent une plus grande partie de l'image mais peuvent exposer les bords.
- L'estimation du mouvement global fonctionne mieux lorsque la scène est dominée par le mouvement de la caméra. Les scènes comportant de grands objets en mouvement au premier plan sont plus difficiles à stabiliser.

## Démos

- **[Démo de stabilisation vidéo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Video%20Stabilization%20Demo)** - pipeline Media Blocks : ouvrez un fichier, prévisualisez la lecture stabilisée, enregistrez-la éventuellement en MP4 et ajustez les paramètres en direct.
- **[Capture avec stabilisation vidéo X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Video%20Stabilization%20Capture%20X)** - stabilisation d'une caméra en direct avec `VideoCaptureCoreX` et `Video_Processing_AddBlock()`.
- **[Démo de stabilisation de caméra](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Video%20Stabilization%20Camera%20Demo)** - la même caméra en direct, mais construite à la main comme un `MediaBlocksPipeline` : source caméra -> stabilisateur -> moteur de rendu, avec réglage en direct.
