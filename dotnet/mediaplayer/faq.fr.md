---
title: FAQ Media Player SDK .NET — flux, recherche, vitesse, WPF
description: Réponses sur Media Player SDK .NET — URL réseau, image après une recherche, vitesse et image par image, démarrage synchronisé, WPF et karaoké.
sidebar_label: FAQ
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - Windows
  - Playback
  - RTSP
  - WPF
  - Troubleshooting
primary_api_classes:
  - MediaPlayerCore
  - MediaPlayerCoreX
  - CDGSourceSettings
  - MediaInfoReader
  - MediaInfoReaderX

---

# FAQ de Media Player SDK .NET

Réponses aux questions que les développeurs posent le plus souvent sur Media Player SDK .NET, pour les deux moteurs : `MediaPlayerCore` (Windows, DirectShow) et `MediaPlayerCoreX` (multiplateforme). Les erreurs de déploiement sont regroupées dans [Dépannage](../general/troubleshooting.md).

## Sources réseau

### « Unable to load source file or stream » — un flux réseau ne s'ouvre pas

`MediaPlayerCore` signale ce message, ou `Unable to set URL to network VLC source filter` avec le mode source `HTTP_RTSP_VLC`, lorsque le moteur de la source n'a pas pu ouvrir l'URL. Vérifiez, dans cet ordre :

1. L'URL est l'adresse du flux lui-même (par exemple `rtsp://host:554/path` ou `http://host/stream.m3u8`), et non celle d'une page web ou la seule adresse IP de la caméra.
2. La même URL s'ouvre dans VLC sur le même ordinateur. Sinon, le problème vient de l'URL, du réseau ou d'un pare-feu, pas du lecteur.
3. Les identifiants sont inclus : `rtsp://login:password@host:port/path`.
4. Le port est accessible depuis l'ordinateur qui exécute l'application, et pas seulement depuis le poste de développement.

Pour les caméras IP qui demandent plus qu'une URL, comme la découverte ONVIF ou une connexion à faible latence, consultez la [FAQ de Video Capture SDK](../videocapture/faq.md#cameras-ip).

## Recherche, vitesse et images

### Frame_GetCurrent renvoie l'image précédente juste après une recherche

`Position_Set_Time` rend la main dès que la recherche est demandée, avant que la nouvelle image soit décodée et affichée ; un appel immédiat à `Frame_GetCurrent()` voit donc encore l'ancienne. Prenez plutôt l'image dans l'événement `OnVideoFrameBitmap` après la recherche : positionnez un indicateur lors de la recherche et conservez une copie de `e.Frame` (`new Bitmap(e.Frame)`) la première fois que le gestionnaire s'exécute ensuite ; le bitmap de l'événement lui-même appartient au SDK. Pour lire des images d'un fichier sans le lire, consultez [Obtenir une image d'un fichier vidéo](code-samples/get-frame-from-video-file.md).

### Comment lire plus vite ou plus lentement, image par image ou à l'envers ?

Avec `MediaPlayerCore` :

- `SetSpeedAsync(2.0)` lit à vitesse double ; les valeurs inférieures à 1.0 ralentissent la lecture.
- `NextFrame()` et `PreviousFrame()` mettent la lecture en pause et avancent ou reculent d'une image.
- La lecture continue à l'envers utilise le mode de lecture inverse — voir [Lecture vidéo inversée](code-samples/reverse-video-playback.md).

Avec `MediaPlayerCoreX`, utilisez `Rate_SetAsync(rate)` et `NextFrame()`.

Reculer image par image est coûteux avec les formats qui utilisent des images clés (H.264, HEVC et la plupart des autres) : le décodeur doit revenir à l'image clé précédente et décoder à nouveau vers l'avant. Une recherche avec `Position_Set_Time` vers une position antérieure est la façon générale de revenir en arrière.

### Comment démarrer la lecture à un instant précis, ou plusieurs lecteurs à la fois ?

Préchargez le fichier et démarrez-le plus tard. `PlayAsync(true)` construit et prépare le graphe de lecture sans le démarrer ; `PlayDelayed()` démarre ensuite la lecture presque sans délai :

```csharp
await player1.PlayAsync(true);
await player2.PlayAsync(true);

// au moment voulu
player1.PlayDelayed();
player2.PlayDelayed();
```

C'est la façon de synchroniser la vidéo avec une horloge externe ou une piste audio sous Windows. Media Player SDK .NET prend en charge les applications WinForms, WPF, WinUI 3, MAUI, Avalonia et Uno ; les applications UWP ne sont pas prises en charge.

## WPF

### Peut-on placer des contrôles WPF sur la vidéo, ou la capturer avec un VisualBrush ?

Oui. Le `VideoView` WPF est un élément WPF, et non une fenêtre WinForms hébergée ; il participe donc à la mise en page, aux transformations et à l'ordre Z de WPF lorsqu'il effectue le rendu dans un mode composable. Utilisez le moteur de rendu composable D3D11 pour un rendu GPU, ou le mode logiciel. Seuls les moteurs de rendu à fenêtre native dessinent par-dessus tout — voir [Superposer des contrôles WPF sur la vidéo](../general/code-samples/wpf-controls-over-video.md).

## Fichiers et formats

### Comment lire des fichiers karaoké CDG (MP3+G ou ZIP) avec changement de hauteur ?

`MediaPlayerCoreX` les ouvre avec `CDGSourceSettings`, pour une paire de fichiers CDG et audio ou pour une archive ZIP contenant les deux :

```csharp
var settings = new CDGSourceSettings("song.zip");
settings.EnablePitchShifting = true;
settings.PitchSemitones = -2;
await player.OpenAsync(settings);
await player.PlayAsync();
```

`Rate_SetAsync` modifie aussi le tempo des fichiers karaoké. La [Karaoke Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Karaoke%20Demo) présente un lecteur karaoké complet.

## Décodage matériel

### Quel décodeur matériel le moteur source FFMPEG doit-il utiliser ?

Avec `Source_Mode` défini sur `MediaPlayerSourceMode.FFMPEG`, définissez `Source_FFMPEG_HWDecoder` avant `PlayAsync`. Pour les caméras IP dans Video Capture SDK .NET, le même choix est `FFMPEG_HWDecoder` dans `IPCameraSourceSettings` avec le moteur `Auto_FFMPEG`. `DXVA2Native` n'y est pas disponible.

```csharp
player.Source_Mode = MediaPlayerSourceMode.FFMPEG;
player.Source_FFMPEG_HWDecoder = FFMPEGHWDecoder.Direct3D11;
```

| Valeur | À utiliser quand |
|---|---|
| `Auto` (par défaut) | Vous voulez le comportement des versions précédentes : décodage logiciel. |
| `Software` | La vidéo doit être décodée par le processeur, par exemple pour contourner un problème de pilote. |
| `NvidiaCUDA` | L'ordinateur dispose d'un GPU NVIDIA. |
| `IntelQuickSync` | L'ordinateur dispose d'une carte graphique Intel. |
| `Direct3D11` | Tout GPU. Un bon premier choix si vous ne connaissez pas le fabricant du GPU. |
| `DXVA2CopyBack` | Tout GPU, y compris d'anciens pilotes sans décodage vidéo Direct3D 11. |
| `DXVA2Native` | La plus faible charge processeur, avec EVR ou madVR, sans effets vidéo ni sample grabber. |

`NvidiaCUDA`, `IntelQuickSync`, `Direct3D11` et `DXVA2CopyBack` recopient les images décodées en mémoire système : tous les moteurs de rendu, les effets vidéo et le sample grabber fonctionnent avec eux.

Toutes les valeurs sauf `Auto` sont strictes. Si l'ordinateur ne dispose pas de ce GPU, si le GPU ne peut pas décoder la vidéo (codec, profil, profondeur de bits ou taille d'image), ou si `DXVA2Native` n'a pas de moteur de rendu qui accepte ses surfaces, la lecture ne démarre pas et `OnError` indique le décodeur et la raison. Le SDK ne revient jamais silencieusement au décodage logiciel : gérez `OnError` et choisissez un autre décodeur, ou `Auto`.
