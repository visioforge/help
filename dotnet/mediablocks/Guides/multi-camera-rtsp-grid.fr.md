---
title: Grille RTSP multi-caméras en C# .NET pour murs NVR 4x4
description: Construisez un mur de caméras RTSP 4×4 façon NVR en C#/.NET avec le VisioForge Media Blocks SDK. Exemples WPF et MAUI, démarrage synchronisé, faible latence.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Android
  - iOS
  - WPF
  - MAUI
  - GStreamer
  - Streaming
  - Decoding
  - Mixing
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - RTSPPlayEngine
  - VideoRendererBlock
  - RTSPSourceSettings
  - IVideoView

---

# Grille RTSP multi-caméras — mur NVR 4×4 en C# / .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Le Media Blocks SDK fonctionne sous **Windows, macOS, Linux, Android et iOS** via GStreamer — les exemples WPF et MAUI ci-dessous couvrent l'intégralité du scénario multiplateforme. Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) pour les détails sur les codecs et l'accélération matérielle, ainsi que le [guide de déploiement Linux](../../deployment-x/Ubuntu.md) pour Ubuntu / NVIDIA Jetson / Raspberry Pi.

Ce guide montre comment construire une grille d'aperçu en direct 4×4 (16 caméras RTSP simultanées) avec le VisioForge Media Blocks SDK — la disposition classique d'un NVR / mur vidéo / tableau de vidéosurveillance. Vous obtiendrez une classe d'aide réutilisable `RTSPPlayEngine` ainsi que des exemples XAML + code-behind complets pour WPF et MAUI, y compris le schéma de démarrage synchronisé qui maintient les 16 flux alignés à l'image près à l'écran.

## Architecture — un pipeline par caméra

Le Media Blocks SDK prend en charge deux manières d'afficher plusieurs vidéos simultanément, et il est important de choisir la bonne :

- **Un pipeline par caméra, un `VideoRendererBlock` par cellule** (ce guide). Chaque caméra dispose de son propre `MediaBlocksPipeline` + `RTSPSourceBlock` + `VideoRendererBlock`, et chaque `VideoRendererBlock` dessine dans son propre `VideoView` de l'interface. C'est ce dont a besoin un mur NVR — 16 flux indépendants, chacun redimensionnable et redémarrable individuellement.
- **Un seul pipeline avec `VideoMixerBlock`** composant toutes les sources en une seule image de sortie. Utile lorsque vous voulez une seule vidéo fusionnée (diffuser tout le mur en un seul flux RTMP, enregistrer en un seul MP4). Ce n'est pas ce qu'il faut pour une grille d'aperçu interactive — vous perdez le contrôle indépendant.

Ce guide utilise le premier schéma. La topologie pour 16 caméras :

```text
┌───────────────────────┐     ┌──────────────────────┐
│  RTSPSourceBlock #0   │ ──► │ VideoRendererBlock #0 │ ──► videoView[0,0]
└───────────────────────┘     └──────────────────────┘

┌───────────────────────┐     ┌──────────────────────┐
│  RTSPSourceBlock #1   │ ──► │ VideoRendererBlock #1 │ ──► videoView[0,1]
└───────────────────────┘     └──────────────────────┘

                           ... ×16 pipelines indépendants ...
```

## Paquets NuGet requis

**WPF (Windows x64)** :

- [VisioForge.DotNet.Core.UI.WPF](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.WPF/) — contrôle `VideoView` WPF
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — runtime GStreamer pour Windows x64

**MAUI (Windows / Android / iOS / macCatalyst)** :

- [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI/) — contrôle `VideoView` MAUI
- Par plateforme : [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/), [VisioForge.CrossPlatform.Core.Android](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Android/), [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS/), [VisioForge.CrossPlatform.Core.macCatalyst](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macCatalyst/)

## La classe d'aide réutilisable `RTSPPlayEngine`

Les exemples WPF et MAUI utilisent tous deux la même classe encapsulant. Elle prend un `RTSPSourceSettings` déjà configuré ainsi qu'un `IVideoView`, construit un graphe de lecture pipeline unique, et expose des méthodes de cycle de vie asynchrones et un événement `OnError`.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Sources;

public class RTSPPlayEngine : IAsyncDisposable
{
    private MediaBlocksPipeline _pipeline;
    private VideoRendererBlock _videoRenderer;
    private AudioRendererBlock _audioRenderer;
    private RTSPSourceBlock _source;
    private bool _disposed;

    public event EventHandler<ErrorsEventArgs> OnError;

    public RTSPPlayEngine(RTSPSourceSettings rtspSettings, IVideoView videoView)
    {
        _pipeline = new MediaBlocksPipeline();
        _pipeline.OnError += (s, e) => OnError?.Invoke(this, e);

        _source = new RTSPSourceBlock(rtspSettings);
        _videoRenderer = new VideoRendererBlock(_pipeline, videoView) { IsSync = false };
        _pipeline.Connect(_source.VideoOutput, _videoRenderer.Input);

        if (rtspSettings.AudioEnabled)
        {
            _audioRenderer = new AudioRendererBlock() { IsSync = false };
            _pipeline.Connect(_source.AudioOutput, _audioRenderer.Input);
        }
    }

    // Démarrer le pipeline en état pausé (utilisé pour le démarrage synchronisé)
    public Task<bool> PreloadAsync() => _pipeline.StartAsync(true);

    // Démarrer la lecture immédiatement (utilisation non synchronisée)
    public Task<bool> StartAsync() => _pipeline.StartAsync();

    // Reprendre un pipeline préchargé
    public Task ResumeAsync() => _pipeline.ResumeAsync();

    public Task<bool> StopAsync() => _pipeline.StopAsync(true);

    public bool IsPaused() => _pipeline.State == PlaybackState.Pause;
    public bool IsStarted() => _pipeline.State == PlaybackState.Play;

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_pipeline != null)
        {
            await _pipeline.DisposeAsync();
            _pipeline = null;
        }

        _videoRenderer?.Dispose();
        _audioRenderer?.Dispose();
        _source?.Dispose();
    }
}
```

Les deux points de conception clés :

- **`IsSync = false`** sur les deux moteurs de rendu. Pour la vidéosurveillance en direct, vous voulez un comportement d'abandon des images en retard, pas le lipsync basé sur l'horloge par défaut (qui calerait toute la cellule si un paquet est en retard).
- **`PreloadAsync` vs `StartAsync`**. `PreloadAsync` appelle `pipeline.StartAsync(true)` qui précharge le pipeline en état Paused. Combiné avec un appel ultérieur à `ResumeAsync`, cela permet de lancer les 16 caméras, d'attendre que chacune soit sur sa première image, puis de toutes les faire jouer simultanément — pas de décalage sur le mur.

## Exemple WPF — grille 4×4

### XAML

```xml
<Window x:Class="MultiCameraWall.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:wpf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="RTSP 4×4 Wall"
        Width="1600" Height="900"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <DockPanel>
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal" Margin="10">
            <Button x:Name="btStart" Content="Start all" Width="80" Click="btStart_Click" />
            <Button x:Name="btStop"  Content="Stop all"  Width="80" Click="btStop_Click" Margin="10,0,0,0" />
        </StackPanel>

        <UniformGrid Rows="4" Columns="4">
            <wpf:VideoView x:Name="videoView00" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView01" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView02" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView03" Background="Black" Margin="1" />

            <wpf:VideoView x:Name="videoView10" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView11" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView12" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView13" Background="Black" Margin="1" />

            <wpf:VideoView x:Name="videoView20" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView21" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView22" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView23" Background="Black" Margin="1" />

            <wpf:VideoView x:Name="videoView30" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView31" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView32" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView33" Background="Black" Margin="1" />
        </UniformGrid>
    </DockPanel>
</Window>
```

`UniformGrid` est la primitive WPF la plus propre pour une grille régulière — pas de définitions de lignes/colonnes, il répartit automatiquement les enfants dans une disposition 4×4.

### Code-behind

```csharp
using System;
using System.Threading.Tasks;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;

public partial class MainWindow : Window
{
    private const int GridSize = 4;
    private readonly RTSPPlayEngine[] _engines = new RTSPPlayEngine[GridSize * GridSize];
    private readonly IVideoView[] _views;

    // 16 URL RTSP — remplacez par les vôtres. Les URL de services ONVIF fonctionnent aussi ;
    // RTSPSourceSettings.CreateAsync les résoudra en RTSP en interne.
    private static readonly string[] Urls =
    {
        "rtsp://192.168.1.101:554/stream1", "rtsp://192.168.1.102:554/stream1",
        "rtsp://192.168.1.103:554/stream1", "rtsp://192.168.1.104:554/stream1",
        "rtsp://192.168.1.105:554/stream1", "rtsp://192.168.1.106:554/stream1",
        "rtsp://192.168.1.107:554/stream1", "rtsp://192.168.1.108:554/stream1",
        "rtsp://192.168.1.109:554/stream1", "rtsp://192.168.1.110:554/stream1",
        "rtsp://192.168.1.111:554/stream1", "rtsp://192.168.1.112:554/stream1",
        "rtsp://192.168.1.113:554/stream1", "rtsp://192.168.1.114:554/stream1",
        "rtsp://192.168.1.115:554/stream1", "rtsp://192.168.1.116:554/stream1",
    };

    public MainWindow()
    {
        InitializeComponent();
        _views = new IVideoView[]
        {
            videoView00, videoView01, videoView02, videoView03,
            videoView10, videoView11, videoView12, videoView13,
            videoView20, videoView21, videoView22, videoView23,
            videoView30, videoView31, videoView32, videoView33,
        };
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        await DestroyAllAsync();

        // 1. Construire les sources et les engines en parallèle.
        var createTasks = new Task[GridSize * GridSize];
        for (int i = 0; i < _engines.Length; i++)
        {
            int idx = i;
            createTasks[i] = Task.Run(async () =>
            {
                var settings = await RTSPSourceSettings.CreateAsync(
                    new Uri(Urls[idx]), login: "admin", password: "admin123",
                    audioEnabled: false);

                settings.LowLatencyMode = true;          // minimiser le tampon de gigue
                settings.UseGPUDecoder  = true;          // décharger H.264 vers le GPU

                var engine = new RTSPPlayEngine(settings, _views[idx]);
                engine.OnError += (s, err) =>
                    Dispatcher.Invoke(() =>
                        System.Diagnostics.Debug.WriteLine($"cam[{idx}]: {err.Message}"));
                _engines[idx] = engine;
            });
        }
        await Task.WhenAll(createTasks);

        // 2. Précharger chaque pipeline en état Paused.
        await Task.WhenAll(Array.ConvertAll(_engines, en => en.PreloadAsync()));

        // 3. Attendre que tous les pipelines signalent Paused.
        for (int tries = 0; tries < 100; tries++)   // 100 × 50 ms = 5 s max
        {
            bool allPaused = true;
            foreach (var en in _engines)
                if (!en.IsPaused()) { allPaused = false; break; }
            if (allPaused) break;
            await Task.Delay(50);
        }

        // 4. Reprendre tous simultanément — démarrage aligné à l'image sur les 16 cellules.
        foreach (var en in _engines)
            await en.ResumeAsync().ConfigureAwait(false);
    }

    private async void btStop_Click(object sender, RoutedEventArgs e) => await DestroyAllAsync();

    private async Task DestroyAllAsync()
    {
        for (int i = 0; i < _engines.Length; i++)
        {
            if (_engines[i] != null)
            {
                await _engines[i].DisposeAsync();
                _engines[i] = null;
            }
        }
    }

    private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        await DestroyAllAsync();
        VisioForgeX.DestroySDK();
    }
}
```

## Exemple MAUI — grille 4×4

La version MAUI utilise la même classe `RTSPPlayEngine` sans modification — les seules différences sont l'espace de noms XAML, le pont `videoView.GetVideoView()` pour obtenir un `IVideoView` à partir d'un `VideoView` MAUI, et l'enregistrement du gestionnaire dans `MauiProgram.cs`.

### MauiProgram.cs

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}
```

### MainPage.xaml

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
             x:Class="MultiCameraWall.MainPage">
    <Grid RowDefinitions="Auto,*">
        <HorizontalStackLayout Grid.Row="0" Padding="10" Spacing="10">
            <Button x:Name="btStart" Text="Start all" Clicked="OnStartClicked" />
            <Button x:Name="btStop"  Text="Stop all"  Clicked="OnStopClicked"  />
        </HorizontalStackLayout>

        <Grid Grid.Row="1"
              RowDefinitions="*,*,*,*"
              ColumnDefinitions="*,*,*,*"
              RowSpacing="1" ColumnSpacing="1" BackgroundColor="Black">

            <vf:VideoView x:Name="cam00" Grid.Row="0" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam01" Grid.Row="0" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam02" Grid.Row="0" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam03" Grid.Row="0" Grid.Column="3" BackgroundColor="Black" />

            <vf:VideoView x:Name="cam10" Grid.Row="1" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam11" Grid.Row="1" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam12" Grid.Row="1" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam13" Grid.Row="1" Grid.Column="3" BackgroundColor="Black" />

            <vf:VideoView x:Name="cam20" Grid.Row="2" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam21" Grid.Row="2" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam22" Grid.Row="2" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam23" Grid.Row="2" Grid.Column="3" BackgroundColor="Black" />

            <vf:VideoView x:Name="cam30" Grid.Row="3" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam31" Grid.Row="3" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam32" Grid.Row="3" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam33" Grid.Row="3" Grid.Column="3" BackgroundColor="Black" />
        </Grid>
    </Grid>
</ContentPage>
```

### MainPage.xaml.cs

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

public partial class MainPage : ContentPage
{
    private const int GridSize = 4;
    private readonly RTSPPlayEngine[] _engines = new RTSPPlayEngine[GridSize * GridSize];
    private IVideoView[] _views;

    private static readonly string[] Urls =
    {
        "rtsp://192.168.1.101:554/stream1", /* ...15 de plus... */
    };

    public MainPage()
    {
        InitializeComponent();
        _views = new IVideoView[]
        {
            cam00.GetVideoView(), cam01.GetVideoView(), cam02.GetVideoView(), cam03.GetVideoView(),
            cam10.GetVideoView(), cam11.GetVideoView(), cam12.GetVideoView(), cam13.GetVideoView(),
            cam20.GetVideoView(), cam21.GetVideoView(), cam22.GetVideoView(), cam23.GetVideoView(),
            cam30.GetVideoView(), cam31.GetVideoView(), cam32.GetVideoView(), cam33.GetVideoView(),
        };

        VisioForgeX.InitSDK();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        await DestroyAllAsync();

        var createTasks = new Task[GridSize * GridSize];
        for (int i = 0; i < _engines.Length; i++)
        {
            int idx = i;
            createTasks[i] = Task.Run(async () =>
            {
                var settings = await RTSPSourceSettings.CreateAsync(
                    new Uri(Urls[idx]), "admin", "admin123", audioEnabled: false);

                settings.LowLatencyMode = true;
                settings.UseGPUDecoder  = true;

                _engines[idx] = new RTSPPlayEngine(settings, _views[idx]);
            });
        }
        await Task.WhenAll(createTasks);

        await Task.WhenAll(Array.ConvertAll(_engines, en => en.PreloadAsync()));

        for (int tries = 0; tries < 100; tries++)
        {
            bool allPaused = true;
            foreach (var en in _engines)
                if (!en.IsPaused()) { allPaused = false; break; }
            if (allPaused) break;
            await Task.Delay(50);
        }

        foreach (var en in _engines)
            await en.ResumeAsync().ConfigureAwait(false);
    }

    private async void OnStopClicked(object sender, EventArgs e) => await DestroyAllAsync();

    private async Task DestroyAllAsync()
    {
        for (int i = 0; i < _engines.Length; i++)
        {
            if (_engines[i] != null)
            {
                await _engines[i].DisposeAsync();
                _engines[i] = null;
            }
        }
    }
}
```

### Manifeste Android

Ajoutez à `Platforms/Android/AndroidManifest.xml` :

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

### iOS / MacCatalyst

Ajoutez au `.csproj` pour que les dépendances GStreamer se chargent correctement sous l'interpréteur Mono :

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-ios')) Or $(TargetFramework.Contains('-maccatalyst'))">
    <UseInterpreter>true</UseInterpreter>
</PropertyGroup>
```

## Démarrage synchronisé — pourquoi Preload + Resume

Si vous appelez simplement `StartAsync()` sur 16 pipelines en boucle, chacun commencera la lecture dès l'arrivée de sa première image-clé — et c'est un instant horloge mural différent par caméra, dépendant de la latence du handshake RTSP et de la cadence des images-clés. L'œil détecte immédiatement le décalage sur une vue de mur.

Le schéma Preload + Resume résout ce problème :

1. `PreloadAsync()` sur chaque pipeline → chaque pipeline entre en **Paused** à la première image.
2. Sondez jusqu'à ce que `IsPaused()` soit true sur les 16.
3. `ResumeAsync()` sur chaque pipeline en succession rapide → toutes les cellules se débloquent dans la même image.

Vous n'avez pas besoin de ce schéma si les caméras montrent des scènes sans rapport (16 pièces différentes). Utilisez-le lorsque la continuité visuelle entre cellules importe (même pièce sous plusieurs angles, replay synchronisé dans le temps, etc.).

Pour ignorer la synchro : remplacez le bloc preload/resume par une seule boucle appelant `await engine.StartAsync()` sur chaque engine, en série ou via `Task.WhenAll`.

## Réglages de performance

Pour un mur réactif de 16 caméras, réglez chaque `RTSPSourceSettings` :

- **`LowLatencyMode = true`** — définit en interne `buffer-mode=None` + `drop-on-latency=true`. Réduit le tampon de gigue de ~1 s à ~200 ms.
- **`UseGPUDecoder = true`** — décodage matériel H.264 / H.265. Sans cela, un mur 16 flux 1080p saturera le CPU sur la plupart des portables.
- **`AudioEnabled = false`** — sur les 16. Personne ne veut 16 flux audio superposés.
- **`VideoRendererBlock.IsSync = false`** — abandon des images en retard. L'encapsulant ci-dessus le définit déjà.
- **Résolution** : demandez à chaque caméra son **sous-flux** (typiquement 720p ou 480p) via le profil ONVIF, pas le flux principal 4K. 16 × 4K est un problème de bande passante avant d'être un problème de rendu.

Sur un poste de bureau de gamme moyenne (CPU 8 cœurs + GPU intégré), un mur 4×4 720p H.264 se situe à environ 15-25 % de CPU et ≤ 200 Mo de RAM avec ces réglages.

## Gestion des erreurs

Le `RTSPPlayEngine` transmet les erreurs du pipeline via son événement `OnError`. Dans une vue de mur, la bonne réponse est **journaliser et laisser les 15 autres tourner** — ne démontez jamais toute la grille pour une seule panne caméra.

```csharp
engine.OnError += (s, err) =>
{
    // Journal par caméra. Repassez sur le thread UI si vous mettez à jour l'UI ici.
    Dispatcher.Invoke(() => LogLine($"cam[{idx}] error: {err.Message}"));
};
```

Pour un NVR de production, vous ajouteriez : horodatages, filtrage par gravité et une superposition « caméra déconnectée » sur le `VideoView` concerné (voir la section suivante).

## Reconnexion — Fallback Switch

`RTSPSourceSettings` expose une propriété `FallbackSwitch` : lorsque le flux RTSP échoue, le pipeline bascule automatiquement vers une image statique, une carte texte ou un fichier média de repli sans se démonter. Cela signifie que la cellule continue d'afficher *quelque chose* (comme un panneau « caméra hors ligne ») au lieu de figer sur la dernière image valable.

```csharp
settings.FallbackSwitch = new FallbackSwitchSettings
{
    // Paramètres d'image/de texte — voir la doc FallbackSwitch pour les options.
};
```

Pour l'API complète de `FallbackSwitch` (types texte / image / média de remplacement, délais d'expiration réglables, `ManualUnblock`, télémétrie au niveau pipeline via `OnNetworkSourceDisconnect`), consultez le [guide de reconnexion RTSP et solution de repli](../../general/network-sources/reconnection-and-fallback.md). Pour un mur multi-caméras, l'activer sur chaque engine est une mise à niveau d'une ligne vers la résilience en production.

## Documentation associée

- [Configuration de source caméra RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — référence `RTSPSourceSettings`, transport UDP/TCP, réglage du tampon
- [Intégration caméra IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) — WS-Discovery et sélection de profil pour trouver les URL de sous-flux pour votre mur
- [Lecteur RTSP mono-caméra](rtsp-player-csharp.md) — la version mono-flux de ce guide
- [Enregistrer le flux RTSP original](rtsp-save-original-stream.md) — enregistrez n'importe lequel/tous les 16 flux sur disque sans réencodage
- [Approfondissement du protocole RTSP](../../general/network-streaming/rtsp.md) — fonctionnement interne de RTSP
- [Bloc de sortie serveur RTSP](../RTSPServer/index.md) — diffusez votre propre mur composite comme une seule sortie RTSP

## Exemples de projets GitHub

- [Démo RTSP MultiViewSync (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20MultiViewSync%20Demo) — démo de synchro 3 caméras sur laquelle ce guide est basé
- [Démo RTSP MultiView (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) — équivalent WinForms 3×3 (sans synchro)

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour davantage d'exemples de code Media Blocks SDK. Besoin de l'URL RTSP de votre caméra ? Parcourez notre [répertoire de marques de caméras IP](../../camera-brands/index.md) couvrant plus de 60 fabricants.
