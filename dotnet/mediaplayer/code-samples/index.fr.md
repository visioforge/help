---
title: Exemples de code et tutoriels de lecteur vidéo en C# .NET
description: Implémentez la lecture, l'extraction d'images, les listes de lecture et le streaming avec VisioForge Media Player SDK .NET. WinForms, WPF et Console.
sidebar_label: Exemples de code
tags:
  - Media Player SDK
  - .NET
  - Windows
  - Playback
  - Streaming

---

# Exemples d'implémentation .NET Media Player SDK

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Cette page rassemble des recettes C# prêtes à l'emploi pour les scénarios de lecture les plus courants utilisant Media Player SDK .Net. Chaque extrait est vérifié par rapport au code source du SDK et aux démos sous `_SOURCE/_DEMOS/Media Player SDK/`. Le moteur `MediaPlayerCore` (Windows uniquement) est utilisé dans les exemples ci-dessous ; le code multi-plateforme basé sur `MediaPlayerCoreX` suit la même structure générale avec des paramètres de source spécifiques au moteur.

## Recettes disponibles

### Lecture de base

- Ouvrir un fichier local et démarrer la lecture
- Mettre en pause, reprendre et arrêter un lecteur en cours d'exécution
- Régler le volume de sortie

### Streaming

- Lire un flux réseau (HTTP / RTSP) par URL
- Basculer le moteur source vers le backend FFmpeg pour les flux en direct

### Effets et traitement

- Capturer l'image courante sous forme de `Bitmap`
- Aller à une position spécifique et lire le temps de lecture

## Recette — Lecteur simple

La classe suivante encapsule `MediaPlayerCore` avec des appels start/stop. Le constructeur reçoit une instance `IVideoView` (`VideoView` pour WPF, `VideoViewWinForms` pour WinForms, etc.) fournie par votre couche UI.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;

public class SimplePlayer
{
    private readonly MediaPlayerCore _player;

    public SimplePlayer(IVideoView videoView)
    {
        _player = new MediaPlayerCore(videoView);

        // Abonnement aux événements.
        _player.OnStop += OnPlaybackStopped;
        _player.OnError += OnPlaybackError;
    }

    public async Task PlayFileAsync(string filePath)
    {
        _player.Playlist_Clear();
        _player.Playlist_Add(filePath);
        await _player.PlayAsync();
    }

    public Task StopAsync() => _player.StopAsync();

    private void OnPlaybackStopped(object sender, StopEventArgs e)
    {
        Console.WriteLine("Lecture terminée.");
    }

    private void OnPlaybackError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Erreur : {e.Message}");
    }
}
```

## Recette — Lecteur avec contrôles de pause et de volume

`MediaPlayerCore` expose `PauseAsync`/`ResumeAsync` pour le contrôle de la lecture en cours et une API de volume par périphérique de sortie (`Audio_OutputDevice_Volume_Set`) qui accepte un entier dans la plage 0–100.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;

public class ControllablePlayer
{
    private readonly MediaPlayerCore _player;
    private bool _isPaused;

    public ControllablePlayer(IVideoView videoView)
    {
        _player = new MediaPlayerCore(videoView);
    }

    public async Task PlayPauseAsync()
    {
        if (_isPaused)
        {
            await _player.ResumeAsync();
            _isPaused = false;
        }
        else
        {
            await _player.PauseAsync();
            _isPaused = true;
        }
    }

    public Task SeekAsync(TimeSpan position) =>
        _player.Position_Set_TimeAsync(position);

    public void SetVolume(int volume)
    {
        // volume : 0 (muet) … 100 (maximum) pour le premier périphérique de sortie audio.
        _player.Audio_OutputDevice_Volume_Set(0, volume);
    }

    public Task<TimeSpan> GetDurationAsync() => _player.Duration_TimeAsync();

    public Task<TimeSpan> GetPositionAsync() => _player.Position_Get_TimeAsync();
}
```

## Recette — Capturer l'image courante

`Frame_GetCurrent()` retourne l'image la plus récemment rendue sous forme de `System.Drawing.Bitmap`. Sauvegardez-la via l'API `Bitmap.Save` ou utilisez `Frame_Save` pour une écriture disque intégrée avec `ImageFormat`.

```csharp
using System.Drawing.Imaging;
using System.Threading.Tasks;

public Task<bool> SaveCurrentFrameAsync(MediaPlayerCore player, string outputPath)
{
    // Helper intégré : encode l'image courante en PNG (ou tout autre ImageFormat).
    return player.Frame_SaveAsync(outputPath, ImageFormat.Png);
}
```

## Recette — Lire un flux réseau

Pour consommer des URL RTSP, RTMP, HTTP, UDP ou TCP via le backend FFmpeg, basculez `Source_Mode` vers `MediaPlayerSourceMode.FFMPEG` et ajoutez l'URL à la liste de lecture comme n'importe quelle autre source.

```csharp
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.MediaPlayer;

public async Task PlayRtspAsync(MediaPlayerCore player, string url)
{
    player.Source_Mode = MediaPlayerSourceMode.FFMPEG;

    player.Playlist_Clear();
    player.Playlist_Add(url);

    await player.PlayAsync();
}
```

## Recette — Navigation dans la liste de lecture

L'API de liste de lecture intégrée suit l'index courant pour vous. `Playlist_PlayNext` avance vers l'élément suivant ; `OnStop` se déclenche en fin de flux naturelle, où vous pouvez enchaîner la piste suivante.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.Events;

public class PlaylistPlayer
{
    private readonly MediaPlayerCore _player;

    public PlaylistPlayer(IVideoView videoView)
    {
        _player = new MediaPlayerCore(videoView);
        _player.OnStop += OnTrackFinished;
    }

    public void AddToPlaylist(string filePath) => _player.Playlist_Add(filePath);

    public Task PlayAsync() => _player.PlayAsync();

    public Task<bool> NextAsync() => _player.Playlist_PlayNextAsync();

    private async void OnTrackFinished(object sender, StopEventArgs e)
    {
        // Avance automatique vers l'entrée suivante de la liste sur EOS naturel.
        if (e.Successful)
        {
            await _player.Playlist_PlayNextAsync();
        }
    }
}
```

## Exemples d'implémentation phares

### Exemples de traitement vidéo

- [Comment obtenir une image spécifique depuis un fichier vidéo ?](get-frame-from-video-file.md)
- [Comment lire un fragment du fichier source ?](play-fragment-file.md)
- [Comment afficher la première image ?](show-first-frame.md)

### Exemples de lecture avancée

- [Implémentation de la lecture depuis la mémoire](memory-playback.md)
- [Intégration de l'API de liste de lecture](playlist-api.md)
- [Image précédente et lecture vidéo inversée](reverse-video-playback.md)

---

## Ressources supplémentaires

Pour une collection plus étendue d'exemples de code et de scénarios d'implémentation, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
