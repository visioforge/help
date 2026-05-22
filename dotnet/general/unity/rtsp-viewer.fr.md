---
title: Streaming de caméra RTSP dans Unity avec Media Blocks SDK
description: Affichez un flux de caméra RTSP en direct dans Unity 6 avec le VisioForge Media Blocks SDK .NET — RTSPSourceBlock et BufferSinkBlock rendus dans un RawImage.
sidebar_label: Afficher une caméra RTSP
order: 52
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - RTSP
  - IP Camera
  - Streaming
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - RTSPSourceBlock
  - BufferSinkBlock
  - AudioRendererBlock
---

# Afficher une caméra RTSP dans Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

La scène **`RTSPViewer`** affiche un flux de caméra RTSP / IP en direct avec le **Media Blocks SDK
.NET**, rendu dans un `RawImage` Unity. Cet article suppose que vous avez importé le paquet Unity et
appliqué les deux réglages de projet requis — consultez d'abord [Utiliser VisioForge dans Unity](index.md).

## Lancer l'exemple

1. Dans la fenêtre **Project**, ouvrez `Assets/Scenes/RTSPViewer.unity` (double-cliquez dessus).
2. Dans la **Hierarchy**, sélectionnez le GameObject **RawImage**. Le composant `RTSPViewerPlayer`
   y est attaché.
3. Dans l'**Inspector**, définissez **Rtsp Url** (et **Login** / **Password** si la caméra requiert
   une authentification).
4. Appuyez sur **▶ Play** — le flux s'affiche dans la vue Game.

![Hierarchy de la scène RTSPViewer : Canvas avec RawImage, EventSystem, Main Camera](unity-rtspviewer-hierarchy.webp)

![Composant RTSPViewerPlayer dans l'Inspector avec les champs Rtsp Url, Login et Password](unity-rtspviewer-inspector.webp)

## Champs de l'Inspector

| Champ | Valeur par défaut | Description |
|---|---|---|
| **Rtsp Url** | `rtsp://192.168.1.10:554/stream` | URL RTSP de la caméra/du flux. |
| **Login** | *(vide)* | Nom d'utilisateur RTSP — laissez vide si le flux ne nécessite pas d'authentification. |
| **Password** | *(vide)* | Mot de passe RTSP. |
| **Auto Play On Start** | `true` | Se connecter automatiquement dans `Start()`. |
| **Render Audio** | `true` | Diffuser l'audio via le périphérique par défaut du système. |
| **Aspect Mode** | `Letterbox` | Manière d'adapter la vidéo au `RawImage` : `Stretch`, `Letterbox` ou `Crop`. |

## Le pipeline

`RTSPViewerPlayer` construit ce pipeline :

```mermaid
graph LR;
    src[RTSPSourceBlock] -->|vidéo| sink["BufferSinkBlock (RGBA)"];
    sink --> view["VisioForgeVideoView (Texture2D)"];
    src -->|audio, optionnel| audio[AudioRendererBlock];
```

Le cœur de `PlayAsync` :

```csharp
_pipeline = new MediaBlocksPipeline();

// readInfo:false ignore le pré-sondage du média (il peut échouer sous le runtime Unity, et
// sonder un flux en direct ajoute de la latence de connexion) ; le codec est négocié au démarrage de la lecture.
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), login ?? string.Empty, password ?? string.Empty,
    audioEnabled: _renderAudio, readInfo: false);

_source = new RTSPSourceBlock(settings);

_videoSink = new BufferSinkBlock(VideoFormatX.RGBA);
_videoSink.OnVideoFrameBuffer += _videoView.OnFrameBuffer;
_pipeline.Connect(_source.VideoOutput, _videoSink.Input);

if (_renderAudio && _source.AudioOutput != null)
{
    _audioRenderer = new AudioRendererBlock();
    _pipeline.Connect(_source.AudioOutput, _audioRenderer.Input);
}

await _pipeline.StartAsync();
```

## L'utiliser dans votre propre scène

Ajoutez un **Canvas → Raw Image** (*GameObject → UI → Raw Image*), sélectionnez-le, **Add Component →**
`RTSPViewerPlayer`, définissez **Rtsp Url**, puis appuyez sur **▶ Play**. La disposition du
`RawImage`, la gestion de l'aspect et le retournement vertical sont pris en charge par le
`VisioForgeVideoView` fourni. Pour la lecture de fichiers locaux plutôt que le RTSP, utilisez
`MediaBlocksPlayer` (voir [Lire un fichier multimédia](simple-player.md)).

## Foire aux questions

### Comment fournir les identifiants de la caméra ?

Renseignez les champs **Login** et **Password**. Laissez-les vides pour les flux qui ne nécessitent
aucune authentification ; les identifiants sont envoyés à la caméra, ils ne sont pas intégrés à
l'URL.

### Quel format d'URL dois-je utiliser ?

La forme standard `rtsp://host:port/path` exposée par votre caméra, par exemple
`rtsp://192.168.1.21:554/Streaming/Channels/101` (Hikvision) ou
`rtsp://192.168.1.22:554/cam/realmonitor?channel=1&subtype=0` (Dahua). Consultez le manuel de votre
caméra pour connaître son chemin de flux exact.

### Que se passe-t-il si la caméra n'a pas d'audio ?

Cela fonctionne en vidéo seule. La branche audio n'est connectée que lorsque le flux transporte
effectivement de l'audio ; une caméra sans audio ne nécessite donc aucune modification.

### Puis-je afficher plusieurs caméras à la fois ?

Oui. Ajoutez un `RawImage` avec son propre `RTSPViewerPlayer` pour chaque caméra ; chacun construit
un pipeline indépendant.

## Voir aussi

- [Utiliser VisioForge dans Unity](index.md) — présentation du paquet, configuration et fonctionnement du rendu
- [Lire un fichier multimédia dans Unity](simple-player.md) — l'exemple de lecture de fichier
- [Guide du streaming RTSP](../network-streaming/rtsp.md) — le RTSP à travers les SDK VisioForge .NET
- [Répertoire des marques de caméras IP](../../camera-brands/index.md) — URLs et réglages de caméras testés
- [Lecteur RTSP Media Blocks en C#](../../mediablocks/Guides/rtsp-player-csharp.md) — un exemple RTSP hors Unity
