---
title: Ejemplos de Código de Media Player SDK
description: Ejemplos prácticos de Media Player SDK .Net: reproducción, streaming, efectos de video, control de audio y más funciones avanzadas.
---

# Ejemplos de Código - Media Player SDK

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección contiene ejemplos de código prácticos para diferentes escenarios de reproducción multimedia.

## Ejemplos Disponibles

### Reproducción Básica

- Reproducción de archivos locales
- Reproducción de URLs
- Control de volumen y balance

### Streaming

- Reproducción de flujos RTSP
- Reproducción de HLS/DASH
- Manejo de autenticación

### Efectos y Procesamiento

- Ajustes de video (brillo, contraste)
- Captura de fotogramas
- Superposición de texto

## Ejemplo: Reproductor Básico

```csharp
public class SimplePlayer
{
    private MediaPlayerCore _player;
    
    public SimplePlayer(VideoView videoView)
    {
        _player = new MediaPlayerCore();
        _player.Video_Renderer.VideoView = videoView;
        
        // Configurar eventos
        _player.OnStop += OnPlaybackStopped;
        _player.OnError += OnPlaybackError;
    }
    
    public async Task PlayFileAsync(string filePath)
    {
        _player.Source_Filename = filePath;
        await _player.PlayAsync();
    }
    
    public async Task StopAsync()
    {
        await _player.StopAsync();
    }
    
    private void OnPlaybackStopped(object sender, EventArgs e)
    {
        Console.WriteLine("Reproducción finalizada");
    }
    
    private void OnPlaybackError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}
```

## Ejemplo: Reproductor con Controles

```csharp
public class AdvancedPlayer
{
    private MediaPlayerCore _player;
    private bool _isPlaying;
    
    public AdvancedPlayer(VideoView videoView)
    {
        _player = new MediaPlayerCore();
        _player.Video_Renderer.VideoView = videoView;
    }
    
    public async Task PlayPauseAsync()
    {
        if (_isPlaying)
        {
            await _player.PauseAsync();
            _isPlaying = false;
        }
        else
        {
            await _player.ResumeAsync();
            _isPlaying = true;
        }
    }
    
    public async Task SeekAsync(TimeSpan position)
    {
        await _player.Position_SetAsync(position);
    }
    
    public void SetVolume(double volume)
    {
        // volume: 0.0 a 1.0
        _player.Audio_Volume_Set(volume);
    }
    
    public TimeSpan GetDuration()
    {
        return _player.Duration();
    }
    
    public TimeSpan GetPosition()
    {
        return _player.Position_Get();
    }
}
```

## Ejemplo: Captura de Fotogramas

```csharp
public async Task CaptureFrameAsync(string outputPath)
{
    // Obtener el fotograma actual
    var frame = await _player.Frame_GetCurrentAsync();
    
    if (frame != null)
    {
        // Guardar como PNG
        frame.Save(outputPath);
        Console.WriteLine($"Fotograma guardado en: {outputPath}");
    }
}
```

## Ejemplo: Reproducción de RTSP

```csharp
public async Task PlayRTSPAsync(string url, string username, string password)
{
    _player.Source_Type = SourceType.RTSP;
    _player.Source_Filename = url;
    
    if (!string.IsNullOrEmpty(username))
    {
        _player.Source_RTSP_Login = username;
        _player.Source_RTSP_Password = password;
    }
    
    // Configurar latencia para streaming en vivo
    _player.Source_RTSP_Latency = 200; // milisegundos
    
    await _player.PlayAsync();
}
```

## Ejemplo: Lista de Reproducción

```csharp
public class PlaylistPlayer
{
    private MediaPlayerCore _player;
    private List<string> _playlist = new List<string>();
    private int _currentIndex = 0;
    
    public PlaylistPlayer(VideoView videoView)
    {
        _player = new MediaPlayerCore();
        _player.Video_Renderer.VideoView = videoView;
        _player.OnStop += OnTrackFinished;
    }
    
    public void AddToPlaylist(string filePath)
    {
        _playlist.Add(filePath);
    }
    
    public async Task PlayAsync()
    {
        if (_playlist.Count > 0)
        {
            await PlayTrackAsync(_currentIndex);
        }
    }
    
    public async Task NextAsync()
    {
        _currentIndex = (_currentIndex + 1) % _playlist.Count;
        await PlayTrackAsync(_currentIndex);
    }
    
    public async Task PreviousAsync()
    {
        _currentIndex = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
        await PlayTrackAsync(_currentIndex);
    }
    
    private async Task PlayTrackAsync(int index)
    {
        await _player.StopAsync();
        _player.Source_Filename = _playlist[index];
        await _player.PlayAsync();
    }
    
    private async void OnTrackFinished(object sender, EventArgs e)
    {
        // Reproducir siguiente pista automáticamente
        await NextAsync();
    }
}
```
