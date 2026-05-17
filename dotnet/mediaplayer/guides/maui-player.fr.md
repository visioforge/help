---
title: Lecteur vidéo .NET MAUI en C# — iOS, Android, Windows
description: Créez un lecteur vidéo .NET MAUI en C# qui fonctionne sur iOS, Android, macOS et Windows depuis une seule base de code avec le contrôle VideoView.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Playback
  - Streaming
  - MPEG-DASH
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - VideoView
  - MediaPlayerCoreX
  - UniversalSourceSettings
  - IVideoView
  - ValueChangedEventArgs

---

# Construire un lecteur vidéo .NET MAUI en C#

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

.NET MAUI vous permet de livrer une seule base de code C# pour **iOS, Android, macOS et Windows**. Ce guide met en place le contrôle VisioForge `VideoView` avec `MediaPlayerCoreX` pour lire des fichiers locaux et des flux réseau — un starter ciblé qui reflète la [démo Simple Player MAUI](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI/SimplePlayer).

> **Vous choisissez un framework ?**
> Avalonia (bureau d'abord, inclut Linux) → [Guide du lecteur Avalonia](avalonia-player.md).
> WinForms / WPF (bureau Windows) → [Construire un lecteur vidéo en C#](video-player-csharp.md).
> Android uniquement (sans MAUI) → [Guide du lecteur Android](android-player.md).

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Prérequis

- SDK .NET 8 (ou plus récent) avec la charge de travail MAUI : `dotnet workload install maui`
- Chaînes d'outils des plateformes pour les cibles que vous livrez (Xcode pour iOS/macCatalyst, Android SDK pour Android)
- Consultez le [Guide d'installation MAUI](../../install/maui.md) pour les détails complets de configuration des plateformes

## Paquets NuGet

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.2.19" />

<!-- Redists de plateforme — incluez uniquement les cibles que vous construisez -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0"
                  Condition="$(TargetFramework.Contains('windows'))" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0"
                  Condition="$(TargetFramework.Contains('windows'))" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1"
                  Condition="$(TargetFramework.Contains('maccatalyst'))" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33"
                  Condition="$(TargetFramework.Contains('android'))" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16"
                  Condition="$(TargetFramework.Contains('ios'))" />
```

## MauiProgram.cs

Enregistrez les handlers VisioForge afin que MAUI puisse résoudre le contrôle `VideoView` :

```csharp
using VisioForge.Core.UI.MAUI;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseSkiaSharp()
        .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());

    return builder.Build();
}
```

## Disposition XAML

Déposez un `VideoView` dans votre page à côté d'un curseur de positionnement et de boutons de lecture :

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
             x:Class="MauiPlayerDemo.MainPage">

    <Grid RowDefinitions="*,Auto" RowSpacing="0">
        <my:VideoView Grid.Row="0" x:Name="videoView"
                      HorizontalOptions="FillAndExpand"
                      VerticalOptions="FillAndExpand"
                      Background="Black" />

        <VerticalStackLayout Grid.Row="1" Spacing="4" Padding="12,8">
            <Slider x:Name="slSeeking" ValueChanged="slSeeking_ValueChanged" />

            <Grid ColumnDefinitions="*,*,*" ColumnSpacing="8">
                <Button Grid.Column="0" x:Name="btOpen"
                        Text="OPEN" Clicked="btOpen_Clicked" />
                <Button Grid.Column="1" x:Name="btPlayPause"
                        Text="PLAY" Clicked="btPlayPause_Clicked" />
                <Button Grid.Column="2" x:Name="btStop"
                        Text="STOP" Clicked="btStop_Clicked" />
            </Grid>

            <Slider x:Name="slVolume" Minimum="0" Maximum="100" Value="50"
                    ValueChanged="slVolume_ValueChanged" />
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

## Code-Behind : configuration du lecteur

`VideoView.GetVideoView()` retourne un pont `IVideoView` que `MediaPlayerCoreX` consomme à l'identique sur chaque cible MAUI (WinUI, macCatalyst, AndroidX, UIKit) :

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.MAUI;

public partial class MainPage : ContentPage
{
    private MediaPlayerCoreX _player;
    private System.Timers.Timer _positionTimer = new(500);
    private string _filename;
    private volatile bool _isTimerUpdate;

    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;
        _positionTimer.Elapsed += PositionTimer_Elapsed;
    }

    private async void MainPage_Loaded(object sender, EventArgs e)
    {
        IVideoView vv = videoView.GetVideoView();
        _player = new MediaPlayerCoreX(vv);

        _player.OnError += (_, args) => Debug.WriteLine(args.Message);
        _player.OnStop  += Player_OnStop;

        // iOS n'énumère pas les périphériques de sortie audio — ignorer le sélecteur ici.
#if !__IOS__ || __MACCATALYST__
        var outputs = await _player.Audio_OutputDevicesAsync();
        if (outputs.Length > 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(outputs[0]);
#endif

        Window.Destroying += async (_, _) =>
        {
            if (_player != null)
            {
                await _player.StopAsync();
                await _player.DisposeAsync();
                _player = null;
            }
            VisioForgeX.DestroySDK();
        };
    }
}
```

## Ouvrir un fichier avec `FilePicker`

```csharp
private async void btOpen_Clicked(object sender, EventArgs e)
{
    var result = await FilePicker.Default.PickAsync();
    if (result == null) return;

    _filename = result.FullPath;

    var source = await UniversalSourceSettings.CreateAsync(new Uri(_filename));
    await _player.OpenAsync(source);
    await _player.PlayAsync();

    _positionTimer.Start();
    btPlayPause.Text = "PAUSE";
}
```

Vous pouvez passer tout URI pris en charge (HTTP, HLS, RTSP) à `UniversalSourceSettings.CreateAsync` — le même chemin de code lit des flux réseau sur mobile.

## Lire / Pause / Arrêter

`MediaPlayerCoreX.State` expose le `PlaybackState` actuel afin qu'un seul bouton puisse couvrir le cycle de vie complet :

```csharp
private async void btPlayPause_Clicked(object sender, EventArgs e)
{
    if (_player == null || string.IsNullOrEmpty(_filename)) return;

    switch (_player.State)
    {
        case PlaybackState.Play:
            await _player.PauseAsync();
            btPlayPause.Text = "PLAY";
            break;
        case PlaybackState.Pause:
            await _player.ResumeAsync();
            btPlayPause.Text = "PAUSE";
            break;
        case PlaybackState.Free:
            var source = await UniversalSourceSettings.CreateAsync(new Uri(_filename));
            await _player.OpenAsync(source);
            await _player.PlayAsync();
            _positionTimer.Start();
            btPlayPause.Text = "PAUSE";
            break;
    }
}

private async void btStop_Clicked(object sender, EventArgs e)
{
    _positionTimer.Stop();
    if (_player != null) await _player.StopAsync();
    btPlayPause.Text = "PLAY";
}
```

## Positionnement et volume

Le curseur de positionnement et le curseur de volume passent tous deux par le flag du timer afin que les mises à jour de position pilotées par le timer ne combattent pas les glissements de l'utilisateur :

```csharp
private async void PositionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
{
    if (_player == null) return;

    var position = await _player.Position_GetAsync();
    var duration = await _player.DurationAsync();

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
        _isTimerUpdate = true;
        slSeeking.Maximum = duration.TotalMilliseconds;
        slSeeking.Value   = Math.Min(position.TotalMilliseconds, slSeeking.Maximum);
        _isTimerUpdate = false;
    });
}

private async void slSeeking_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (!_isTimerUpdate && _player != null)
        await _player.Position_SetAsync(TimeSpan.FromMilliseconds(e.NewValue));
}

private void slVolume_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (_player != null)
        _player.Audio_OutputDevice_Volume = e.NewValue / 100.0;
}

private void Player_OnStop(object sender, StopEventArgs e)
    => MainThread.BeginInvokeOnMainThread(() =>
    {
        btPlayPause.Text = "PLAY";
        slSeeking.Value  = 0;
    });
```

## Notes par plateforme

- **iOS** — ajoutez les exceptions [App Transport Security](../../install/maui.md) pour les flux HTTP (non-HTTPS) et les descriptions d'utilisation du microphone/photothèque dont vous avez besoin. `_player.Audio_OutputDevicesAsync()` n'est pas disponible sur iOS — le moteur de rendu sélectionne automatiquement la sortie active.
- **Android** — déclarez la permission `INTERNET` pour les flux réseau ; pour la lecture de fichiers locaux sur API 33+, vous avez besoin de `READ_MEDIA_VIDEO` ou d'un URI `FilePicker` accordé par l'utilisateur.
- **macCatalyst** — utilisez `VisioForge.CrossPlatform.Core.macCatalyst` ; le décodage matériel passe par `VideoToolbox`.
- **Windows (WinUI)** — utilisez les redists `Windows.x64` + `Libav.Windows.x64` ; le décodage matériel passe par `d3d11va` / `nvdec` / `qsv` selon le GPU.

Le câblage complet spécifique aux plateformes (entitlements, manifests, provisioning) se trouve dans le [Guide d'installation MAUI](../../install/maui.md).

## Formats pris en charge

| Catégorie | Formats |
|----------|---------|
| Conteneurs | MP4, MOV, MKV, WebM, AVI, TS, FLV |
| Codecs vidéo | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Codecs audio | AAC, MP3, Opus, Vorbis, FLAC, AC-3 |
| Streaming | HTTP, HLS, RTSP, MPEG-DASH |

## Applications d'exemple

- [MAUI SimplePlayer (Media Player SDK X)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI/SimplePlayer) — la source de travail complète derrière ce tutoriel
- [MAUI SimplePlayer (Media Blocks SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/SimplePlayer) — la même app construite sur l'API pipeline, si vous avez besoin de traitement personnalisé

## Voir aussi

- [Lecteur multiplateforme Avalonia](avalonia-player.md) — bureau d'abord multiplateforme (inclut Linux)
- [Construire un lecteur vidéo en C#](video-player-csharp.md) — WinForms/WPF sur Windows
- [Guide du lecteur Android](android-player.md) — configuration et déploiement spécifiques à Android
- [Lecteur Media Blocks Pipeline](../../mediablocks/GettingStarted/player.md) — alternative basée sur des blocs avec rendus vidéo/audio explicites
- [Guide d'installation MAUI](../../install/maui.md) — entitlements de plateforme, charges de travail et packaging redist
