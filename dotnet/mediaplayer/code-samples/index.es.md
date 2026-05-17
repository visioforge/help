---
title: Ejemplos C# de Media Player SDK .NET — playlist y streaming
description: Implementa reproducción, extracción de fotogramas, listas de reproducción y streaming con VisioForge Media Player SDK .NET. WinForms, WPF y Consola.
sidebar_label: Ejemplos de código
tags:
  - Media Player SDK
  - .NET
  - Windows
  - Playback
  - Streaming

---

# Ejemplos de implementación de Media Player SDK .NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta página recopila recetas en C# listas para usar para los escenarios de reproducción más comunes con Media Player SDK .Net. Cada fragmento está verificado contra el código fuente del SDK y las demos bajo `_SOURCE/_DEMOS/Media Player SDK/`. En los ejemplos siguientes se utiliza el motor `MediaPlayerCore` (solo Windows); el código multiplataforma basado en `MediaPlayerCoreX` sigue la misma estructura general con ajustes de fuente específicos del motor.

## Recetas disponibles

### Reproducción básica

- Abrir un archivo local e iniciar la reproducción
- Pausar, reanudar y detener un reproductor en marcha
- Ajustar el volumen de salida

### Streaming

- Reproducir un flujo de red (HTTP / RTSP) por URL
- Cambiar el motor de fuente al backend FFmpeg para flujos en directo

### Efectos y procesamiento

- Capturar el fotograma actual como `Bitmap`
- Saltar a una posición concreta y leer el tiempo de reproducción

## Receta — Reproductor simple

La siguiente clase encapsula `MediaPlayerCore` con llamadas start/stop. El constructor recibe una instancia `IVideoView` (`VideoView` para WPF, `VideoViewWinForms` para WinForms, etc.) que proporciona tu capa de UI.

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

        // Suscripción a eventos.
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
        Console.WriteLine("Reproducción finalizada.");
    }

    private void OnPlaybackError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}
```

## Receta — Reproductor con controles de pausa y volumen

`MediaPlayerCore` expone `PauseAsync`/`ResumeAsync` para el control de la reproducción en curso y una API de volumen por dispositivo de salida (`Audio_OutputDevice_Volume_Set`) que acepta un entero en el rango 0–100.

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
        // volume: 0 (silencio) … 100 (máximo) para el primer dispositivo de salida de audio.
        _player.Audio_OutputDevice_Volume_Set(0, volume);
    }

    public Task<TimeSpan> GetDurationAsync() => _player.Duration_TimeAsync();

    public Task<TimeSpan> GetPositionAsync() => _player.Position_Get_TimeAsync();
}
```

## Receta — Capturar el fotograma actual

`Frame_GetCurrent()` devuelve el fotograma más recientemente renderizado como `System.Drawing.Bitmap`. Guárdalo mediante la API `Bitmap.Save` o utiliza `Frame_Save` para escritura en disco integrada con `ImageFormat`.

```csharp
using System.Drawing.Imaging;
using System.Threading.Tasks;

public Task<bool> SaveCurrentFrameAsync(MediaPlayerCore player, string outputPath)
{
    // Helper integrado: codifica el fotograma actual a PNG (o cualquier ImageFormat).
    return player.Frame_SaveAsync(outputPath, ImageFormat.Png);
}
```

## Receta — Reproducir un flujo de red

Para consumir URLs RTSP, RTMP, HTTP, UDP o TCP a través del backend FFmpeg, cambia `Source_Mode` a `MediaPlayerSourceMode.FFMPEG` y añade la URL a la lista de reproducción como cualquier otra fuente.

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

## Receta — Navegación por la lista de reproducción

La API de lista de reproducción integrada lleva el índice actual por ti. `Playlist_PlayNext` avanza al elemento siguiente; `OnStop` se dispara al final natural del flujo, donde puedes encadenar la siguiente pista.

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
        // Avance automático a la siguiente entrada de la lista al EOS natural.
        if (e.Successful)
        {
            await _player.Playlist_PlayNextAsync();
        }
    }
}
```

## Ejemplos de implementación destacados

### Ejemplos de procesamiento de video

- [¿Cómo obtener un fotograma específico de un archivo de video?](get-frame-from-video-file.md)
- [¿Cómo reproducir un fragmento del archivo fuente?](play-fragment-file.md)
- [¿Cómo mostrar el primer fotograma?](show-first-frame.md)

### Ejemplos avanzados de reproducción

- [Implementación de reproducción desde memoria](memory-playback.md)
- [Integración de la API de listas de reproducción](playlist-api.md)
- [Reproducción de fotograma anterior y reproducción inversa de video](reverse-video-playback.md)

---

## Recursos adicionales

Para una colección más extensa de ejemplos de código y escenarios de implementación, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
