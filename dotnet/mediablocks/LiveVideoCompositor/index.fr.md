---
title: Compositeur de vidéo en direct — mixage temps réel C# .NET
description: Mixez plusieurs sources vidéo et audio en direct en temps réel avec VisioForge Media Blocks. Bascule dynamique pour streaming et enregistrement.
sidebar_label: Compositeur de vidéo en direct
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
  - LiveVideoCompositor
  - LVCVideoInput
  - MediaBlock
  - LVCAudioInput
  - LVCAudioOutput

---

# Compositeur de vidéo en direct

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le compositeur de vidéo en direct fait partie du [VisioForge Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) et permet d'ajouter et de retirer des sources et des sorties d'un pipeline en temps réel.

Vous pouvez ainsi créer des applications qui prennent simultanément en charge plusieurs sources vidéo et audio.

Par exemple, le LVC vous permet de démarrer la diffusion vers YouTube au moment précis voulu tout en enregistrant simultanément la vidéo sur disque.
Avec le LVC, vous pouvez créer une application similaire à OBS Studio.

Chaque source et chaque sortie possède un identifiant unique utilisable pour les ajouter et les retirer en temps réel.

Chaque source et chaque sortie dispose de son propre pipeline indépendant, qu'il est possible de démarrer et d'arrêter.

## Fonctionnalités

- Prend en charge plusieurs sources vidéo et audio
- Prend en charge plusieurs sorties vidéo et audio
- Définition de la position et de la taille des sources vidéo
- Définition de la transparence des sources vidéo
- Définition du volume des sources audio

## Classe LiveVideoCompositor

La classe `LiveVideoCompositor` est la classe principale qui permet l'ajout et la suppression de sources et de sorties en direct dans le pipeline. Lors de sa création, il faut spécifier la résolution et la fréquence d'images à utiliser. Toutes les sources ayant une fréquence d'images différente seront automatiquement converties vers la fréquence spécifiée à la création du LVC.

`LiveVideoCompositorSettings` permet de définir les paramètres vidéo et audio. Propriétés clés :

- `MixerType` : spécifie le type de mélangeur vidéo (par ex. `LVCMixerType.OpenGL`, `LVCMixerType.D3D11` (Windows uniquement) ou `LVCMixerType.CPU`).
- `AudioEnabled` : booléen indiquant si le flux audio est activé.
- `VideoWidth`, `VideoHeight`, `VideoFrameRate` : définissent la résolution et la fréquence d'images de sortie.
- `AudioFormat`, `AudioSampleRate`, `AudioChannels` : définissent les paramètres audio de sortie.
- `VideoView` : `IVideoView` facultatif pour effectuer le rendu de la sortie vidéo directement.
- `AudioOutput` : `AudioRendererBlock` facultatif pour effectuer le rendu de la sortie audio directement.

Il est également nécessaire de définir le nombre maximal de sources et de sorties lors de la conception de votre application, bien que ce ne soit pas un paramètre direct de `LiveVideoCompositorSettings`.

### Exemple de code

1. Créer une nouvelle instance de la classe `LiveVideoCompositor`.

```csharp
var settings = new LiveVideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_25);
// Configurer facultativement d'autres paramètres comme MixerType, AudioEnabled, etc.
// settings.MixerType = LVCMixerType.OpenGL;
// settings.AudioEnabled = true;
var compositor = new LiveVideoCompositor(settings);
```

2. Ajouter des sources et des sorties vidéo et audio (voir ci-dessous)
3. Démarrer le pipeline.

```csharp
await compositor.StartAsync();
```

## Entrée vidéo LVC

La classe `LVCVideoInput` sert à ajouter des sources vidéo au pipeline LVC. Elle permet de définir les paramètres vidéo et le rectangle de la source vidéo.

Vous pouvez utiliser tout bloc disposant d'un pad de sortie vidéo. Par exemple, vous pouvez utiliser `VirtualVideoSourceBlock` pour créer une source vidéo virtuelle ou `SystemVideoSourceBlock` pour capturer la vidéo depuis la webcam.

Propriétés clés de `LVCVideoInput` :

- `Rectangle` : définit la position et la taille de la source vidéo dans la sortie du compositeur.
- `ZOrder` : détermine l'ordre d'empilement des sources vidéo qui se chevauchent.
- `ResizePolicy` : spécifie comment redimensionner la source vidéo si son rapport d'aspect diffère du rectangle cible (`LVCResizePolicy.Stretch`, `LVCResizePolicy.Letterbox`, `LVCResizePolicy.LetterboxToFill`).
- `VideoView` : `IVideoView` facultatif pour prévisualiser cette source d'entrée spécifique.

### Utilisation

Lors de la création d'un objet `LVCVideoInput`, vous devez spécifier le `MediaBlock` à utiliser comme source des données vidéo, ainsi qu'un `VideoFrameInfoX` décrivant la vidéo, un `Rect` pour son placement et indiquer s'il doit `autostart`.

### Exemple de code

#### Source vidéo virtuelle

L'exemple de code ci-dessous montre comment créer un objet `LVCVideoInput` avec un `VirtualVideoSourceBlock` comme source vidéo.

```csharp
var rect = new Rect(0, 0, 640, 480);

var name = "Video source [Virtual]";
var settings = new VirtualVideoSourceSettings();
var info = new VideoFrameInfoX(settings.Width, settings.Height, settings.FrameRate);
var src = new LVCVideoInput(name, _compositor, new VirtualVideoSourceBlock(settings), info, rect, true);
// Définir facultativement ZOrder ou ResizePolicy
// src.ZOrder = 1;
// src.ResizePolicy = LVCResizePolicy.Letterbox;
if (await _compositor.Input_AddAsync(src))
{
    // ajouté avec succès
}
else
{
    src.Dispose();
}
```

#### Source écran

Sur les plateformes de bureau, on peut capturer l'écran. L'exemple de code ci-dessous montre comment créer un objet `LVCVideoInput` avec un `ScreenSourceBlock` comme source vidéo.

```csharp
var settings = new ScreenCaptureDX9SourceSettings();
settings.CaptureCursor = true;
settings.Monitor = 0;
settings.FrameRate = new VideoFrameRate(30);
settings.Rectangle = new Rect(0, 0, 1920, 1080); // (left, top, right, bottom) — 1920x1080 plein

var rect = new Rect(0, 0, 640, 480);
var name = $"Screen source";
var info = new VideoFrameInfoX(settings.Rectangle.Width, settings.Rectangle.Height, settings.FrameRate);
var src = new LVCVideoInput(name, _compositor, new ScreenSourceBlock(settings), info, rect, true);
// Définir facultativement ZOrder ou ResizePolicy
// src.ZOrder = 0;
// src.ResizePolicy = LVCResizePolicy.Stretch;
if (await _compositor.Input_AddAsync(src))
{
    // ajouté avec succès
}
else
{ 
    src.Dispose(); 
}
```

#### Source vidéo système (webcam)

L'exemple de code ci-dessous montre comment créer un objet `LVCVideoInput` avec un `SystemVideoSourceBlock` comme source vidéo.

Nous utilisons la classe `DeviceEnumerator` pour obtenir les périphériques de source vidéo. Le premier périphérique vidéo sera utilisé comme source vidéo. Le premier format vidéo du périphérique sera utilisé comme format vidéo.

```csharp
VideoCaptureDeviceSourceSettings settings = null;

var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        settings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };

        settings.Format.FrameRate = dlg.FrameRate;
    }
}

if (settings == null)
{
    MessageBox.Show(this, "Unable to configure video capture device.");
    return;
}

var name = $"Camera source [{device.Name}]";
var rect = new Rect(0, 0, 1280, 720);
var videoInfo = new VideoFrameInfoX(settings.Format.Width, settings.Format.Height, settings.Format.FrameRate);
var src = new LVCVideoInput(name, _compositor, new SystemVideoSourceBlock(settings), videoInfo, rect, true);
// Définir facultativement ZOrder ou ResizePolicy
// src.ZOrder = 2;
// src.ResizePolicy = LVCResizePolicy.LetterboxToFill;

if (await _compositor.Input_AddAsync(src))
{
    // ajouté avec succès
}
else
{
    src.Dispose();
}
```

## Entrée audio LVC

La classe `LVCAudioInput` sert à ajouter des sources audio au pipeline LVC. Elle permet de définir les paramètres audio et le volume de la source audio.

Vous pouvez utiliser tout bloc disposant d'un pad de sortie audio. Par exemple, vous pouvez utiliser `VirtualAudioSourceBlock` pour créer une source audio virtuelle ou `SystemAudioSourceBlock` pour capturer l'audio depuis le microphone.

### Utilisation

Lors de la création d'un objet `LVCAudioInput`, vous devez spécifier le `MediaBlock` à utiliser comme source des données audio, ainsi qu'un `AudioInfoX` (qui nécessite le format, les canaux et la fréquence d'échantillonnage) et indiquer s'il doit `autostart`.

### Exemple de code

#### Source audio virtuelle

L'exemple de code ci-dessous montre comment créer un objet `LVCAudioInput` avec un `VirtualAudioSourceBlock` comme source audio.

```csharp
var name = "Audio source [Virtual]";
var settings = new VirtualAudioSourceSettings();
var info = new AudioInfoX(settings.Format, settings.SampleRate, settings.Channels);
var src = new LVCAudioInput(name, _compositor, new VirtualAudioSourceBlock(settings), info, true);            
if (await _compositor.Input_AddAsync(src))
{
    // ajouté avec succès
}
else
{
    src.Dispose();
}
```

#### Source audio système (DirectSound sous Windows)

L'exemple de code ci-dessous montre comment créer un objet `LVCAudioInput` avec un `SystemAudioSourceBlock` comme source audio.

Nous utilisons la classe `DeviceEnumerator` pour obtenir les périphériques audio. Le premier périphérique audio sert de source audio. Le premier format audio du périphérique sert de format audio.

```csharp
DSAudioCaptureDeviceSourceSettings settings = null;
AudioCaptureDeviceFormat deviceFormat = null;

var device = (await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound))[0];
if (device != null)
{
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        deviceFormat = formatItem.ToFormat();
        settings = new DSAudioCaptureDeviceSourceSettings(device, deviceFormat);
    }
}    

if (settings == null)
{
    MessageBox.Show(this, "Unable to configure audio capture device.");
    return;
}

var name = $"Audio source [{device.Name}]";
var info = new AudioInfoX(deviceFormat.Format, deviceFormat.SampleRate, deviceFormat.Channels);
var src = new LVCAudioInput(name, _compositor, new SystemAudioSourceBlock(settings), info, true);
if (await _compositor.Input_AddAsync(src))
{
    // ajouté avec succès
}
else
{
    src.Dispose();
}
```

## Sortie vidéo LVC

La classe `LVCVideoOutput` sert à ajouter des sorties vidéo au pipeline LVC. Vous pouvez démarrer et arrêter le pipeline de sortie indépendamment du pipeline principal.

### Utilisation

Lors de la création d'un objet `LVCVideoOutput`, vous devez spécifier le `MediaBlock` à utiliser comme sortie des données vidéo, son `name`, une référence au `LiveVideoCompositor` et indiquer s'il doit `autostart` avec le pipeline principal. Un `MediaBlock` de traitement facultatif peut également être fourni. Cet élément sert généralement à enregistrer la vidéo dans un fichier ou à la diffuser (sans audio).

Pour les sorties vidéo+audio, utilisez la classe `LVCVideoAudioOutput`.

Vous pouvez utiliser SuperMediaBlock pour réaliser un pipeline de bloc personnalisé pour la sortie vidéo. Par exemple, vous pouvez ajouter un encodeur vidéo, un multiplexeur et un écrivain de fichier pour enregistrer la vidéo dans un fichier.

## Sortie audio LVC

La classe `LVCAudioOutput` sert à ajouter des sorties audio au pipeline LVC. Vous pouvez démarrer et arrêter le pipeline de sortie indépendamment du pipeline principal.

### Utilisation

Lors de la création d'un objet `LVCAudioOutput`, vous devez spécifier le `MediaBlock` à utiliser comme sortie des données audio, son `name`, une référence au `LiveVideoCompositor` et indiquer s'il doit `autostart`.

### Exemple de code

#### Ajouter un moteur de rendu audio

Ajoute un moteur de rendu audio au pipeline LVC. Vous devez créer un objet `AudioRendererBlock` puis créer un objet `LVCAudioOutput`. Enfin, ajoutez la sortie au compositeur.

Le premier périphérique sert de sortie audio.

```csharp
var audioRenderer = new AudioRendererBlock((await DeviceEnumerator.Shared.AudioOutputsAsync())[0]); 
var audioRendererOutput = new LVCAudioOutput("Audio renderer", _compositor, audioRenderer, true);
await _compositor.Output_AddAsync(audioRendererOutput, true);
```

#### Ajouter une sortie MP3

Ajoute une sortie MP3 au pipeline LVC. Vous devez créer un objet `MP3OutputBlock` puis créer un objet `LVCAudioOutput`. Enfin, ajoutez la sortie au compositeur.

```csharp
var mp3Output = new MP3OutputBlock(outputFile, new MP3EncoderSettings());
var output = new LVCAudioOutput(outputFile, _compositor, mp3Output, false);

if (await _compositor.Output_AddAsync(output))
{
    // ajouté avec succès
}
else
{
    output.Dispose();
}
```

## Sortie vidéo/audio LVC

La classe `LVCVideoAudioOutput` sert à ajouter des sorties vidéo+audio au pipeline LVC. Vous pouvez démarrer et arrêter le pipeline de sortie indépendamment du pipeline principal.

### Utilisation

Lors de la création d'un objet `LVCVideoAudioOutput`, vous devez spécifier le `MediaBlock` à utiliser comme sortie des données vidéo+audio, son `name`, une référence au `LiveVideoCompositor` et indiquer s'il doit `autostart`. Des `MediaBlock` de traitement facultatifs pour la vidéo et l'audio peuvent également être fournis.

### Exemple de code

#### Ajouter une sortie MP4

```csharp
var mp4Output = new MP4OutputBlock(new MP4SinkSettings("output.mp4"), new OpenH264EncoderSettings(), new MFAACEncoderSettings());

var output = new LVCVideoAudioOutput(outputFile, _compositor, mp4Output, false); 

if (await _compositor.Output_AddAsync(output))
{
    // ajouté avec succès
}
else
{
    output.Dispose();
}
```

#### Ajouter une sortie WebM

```csharp
var webmOutput = new WebMOutputBlock(new WebMSinkSettings("output.webm"), new VP8EncoderSettings(), new VorbisEncoderSettings());
var output = new LVCVideoAudioOutput(outputFile, _compositor, webmOutput, false);

if (await _compositor.Output_AddAsync(output))
{
   // ajouté avec succès
}
else
{
    output.Dispose();
}
```

## Sortie Video View LVC

La classe `LVCVideoViewOutput` sert à ajouter un affichage vidéo au pipeline LVC. Vous pouvez l'utiliser pour afficher la vidéo à l'écran.

### Utilisation

Lors de la création d'un objet `LVCVideoViewOutput`, vous devez spécifier le contrôle `IVideoView` à utiliser, son `name`, une référence au `LiveVideoCompositor` et indiquer s'il doit `autostart`. Un `MediaBlock` de traitement facultatif peut également être fourni.

### Exemple de code

```csharp
var name = "[VideoView] Preview";
var videoRendererOutput = new LVCVideoViewOutput(name, _compositor, VideoView1, true);
await _compositor.Output_AddAsync(videoRendererOutput);
```

VideoView1 est un objet `VideoView` utilisé pour afficher la vidéo. Chaque plateforme/framework d'interface possède sa propre implémentation de `VideoView`.

Vous pouvez ajouter plusieurs objets `LVCVideoViewOutput` au pipeline LVC pour afficher la vidéo sur différents écrans.

## Surveillance de la fréquence d'images de rendu

Lorsque le compositeur compose de nombreuses entrées simultanément, il peut prendre du retard par rapport à la `VideoFrameRate` configurée si le CPU ou le GPU manque de marge. Abonnez-vous à `OnRenderStatistics` pour lire la fréquence d'images réelle en sortie et détecter rapidement les pertes :

```csharp
using System.Diagnostics;

compositor.OnRenderStatistics += (sender, e) =>
{
    // Se déclenche environ deux fois par seconde sur un thread du pool de threads.
    Debug.WriteLine($"FPS: {e.ActualFps:F1} / configured {e.ConfiguredFps:F1}, total frames: {e.FramesDelivered}");
};
```

`RenderStatisticsEventArgs` expose :

- `ActualFps` – fréquence d'images de sortie mesurée sur la dernière fenêtre d'échantillonnage.
- `ConfiguredFps` – fréquence d'images demandée via `LiveVideoCompositorSettings.VideoFrameRate`.
- `FramesDelivered` – compteur monotone d'images produites depuis le démarrage du compositeur.
- `LastFrameTimestamp` – PTS de l'image la plus récente (`TimeSpan.Zero` s'il n'y en a pas encore).

Si `ActualFps` reste sensiblement inférieur à `ConfiguredFps` pendant plusieurs ticks consécutifs, le pipeline ne suit pas — causes typiques : trop d'entrées, résolutions sources surdimensionnées, mélangeur purement logiciel sur un CPU fortement chargé, ou encodeur aval lent. Basculer `LVCMixerType` sur `OpenGL` ou `D3D11`, réduire le nombre d'entrées ou abaisser la résolution source restaure habituellement la fréquence configurée.

Repassez sur le thread d'interface utilisateur avant de mettre à jour les contrôles, l'événement étant déclenché sur un thread du pool de threads.

## V1 et V2

Le SDK fournit deux implémentations sous des espaces de noms différents :

- `VisioForge.Core.LiveVideoCompositorV2` – actuelle, recommandée.
- `VisioForge.Core.LiveVideoCompositor` – implémentation originale, **dépréciée** et prévue pour être supprimée dans une version future.

Les deux espaces de noms exposent une classe `LiveVideoCompositor` portant le même nom ainsi que les mêmes helpers `LVC*`. Migrer un fichier se résume généralement à un échange d'instruction `using` d'une seule ligne :

```csharp
// Avant
using VisioForge.Core.LiveVideoCompositor;

// Après
using VisioForge.Core.LiveVideoCompositorV2;
```

Après le changement du `using`, `new LiveVideoCompositor(...)` se résout vers la classe V2 et les avertissements d'obsolescence disparaissent. L'événement `OnRenderStatistics` n'est disponible que sur V2.

---

[Application d'exemple sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Video%20Compositor%20Demo)
