---
title: Lecteur vidéo C# WinForms et WPF — Positionnement et volume
description: Créez un lecteur vidéo C# pour les apps bureau WinForms ou WPF avec VisioForge Media Player SDK — positionnement, volume, vitesse et sous-titres.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - Editing
  - RTSP
  - MPEG-DASH
  - MP4
  - MKV
  - WebM
  - AVI
  - MOV
  - WMV
  - H.265
  - C#
  - NuGet
primary_api_classes:
  - MediaPlayerCoreX
  - VideoView
  - UniversalSourceSettings
  - AudioRendererSettings
  - ErrorsEventArgs

---

# Construire un lecteur vidéo C# pour WinForms et WPF

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce guide vous montre comment construire un lecteur vidéo complet pour **applications bureau Windows** en C# en utilisant [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) avec le moteur de haut niveau `MediaPlayerCoreX`. Vous mettrez en place la lecture de fichiers, le positionnement dans la timeline, la pause/reprise, le volume et la vitesse de lecture face à un contrôle `VideoView` WinForms ou WPF.

!!! info "Choisissez votre approche"
    Cette page concerne les **applications bureau Windows** (WinForms ou WPF) utilisant Media Player SDK .Net.

    - **Bureau multiplateforme (inclut Linux)** → [Guide du lecteur Avalonia](avalonia-player.md)
    - **Mobile + bureau à partir d'une seule base de code (iOS, Android, macOS, Windows)** → [Guide du lecteur .NET MAUI](maui-player.md)
    - **Android uniquement** → [Guide du lecteur Android](android-player.md)
    - **Visual Basic .NET** → [Guide du lecteur vidéo VB.NET](video-player-vb-net.md)
    - **Basé sur un pipeline (graphe de blocs avec rendus explicites)** → [Lecteur Media Blocks SDK](../../mediablocks/GettingStarted/player.md)

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Approche MediaPlayerCoreX

`MediaPlayerCoreX` est le moyen le plus simple de construire un lecteur vidéo en C# avec un contrôle complet de la lecture.

### Paquets NuGet requis

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

### Implémentation complète du lecteur vidéo C#

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

public partial class Form1 : Form
{
    private MediaPlayerCoreX _player;
    private System.Timers.Timer _timer;
    private bool _timerFlag;

    private async void Form1_Load(object sender, EventArgs e)
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += Timer_Elapsed;

        // Créer le moteur du lecteur avec le contrôle VideoView
        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += Player_OnError;
        _player.OnStop += Player_OnStop;

        // Remplir la liste des périphériques de sortie audio
        foreach (var device in await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound))
        {
            cbAudioOutput.Items.Add(device.Name);
        }

        if (cbAudioOutput.Items.Count > 0)
            cbAudioOutput.SelectedIndex = 0;
    }
```

### Ouvrir et lire un fichier vidéo

```csharp
    private void btOpenFile_Click(object sender, EventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.avi;*.mkv;*.wmv;*.webm;*.mov;*.ts" +
                     "|Audio Files|*.mp3;*.aac;*.wav;*.wma|All Files|*.*"
        };

        if (ofd.ShowDialog() == DialogResult.OK)
            edFilename.Text = ofd.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        // Définir le périphérique de sortie audio
        var devices = await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound);
        var audioDevice = devices.First(x => x.Name == cbAudioOutput.Text);
        _player.Audio_OutputDevice = new AudioRendererSettings(audioDevice);

        // Ouvrir le fichier multimédia
        var source = await UniversalSourceSettings.CreateAsync(
            new Uri(edFilename.Text));
        await _player.OpenAsync(source);

        // Démarrer la lecture
        await _player.PlayAsync();

        _timer.Start();
    }
```

### Pause, reprise et arrêt

```csharp
    private async void btPause_Click(object sender, EventArgs e)
    {
        await _player.PauseAsync();
    }

    private async void btResume_Click(object sender, EventArgs e)
    {
        await _player.ResumeAsync();
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        _timer.Stop();

        if (_player != null)
        {
            await _player.StopAsync();
        }

        tbTimeline.Value = 0;
    }
```

### Positionnement dans la timeline

```csharp
    private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        _timerFlag = true;

        if (_player == null) return;

        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();

        Invoke(() =>
        {
            tbTimeline.Maximum = (int)duration.TotalSeconds;
            lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

            if (tbTimeline.Maximum >= position.TotalSeconds)
                tbTimeline.Value = (int)position.TotalSeconds;
        });

        _timerFlag = false;
    }

    private async void tbTimeline_Scroll(object sender, EventArgs e)
    {
        if (!_timerFlag && _player != null)
        {
            await _player.Position_SetAsync(
                TimeSpan.FromSeconds(tbTimeline.Value));
        }
    }
```

### Contrôle du volume et de la vitesse

```csharp
    private void tbVolume_Scroll(object sender, EventArgs e)
    {
        if (_player != null)
            _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;
    }

    private async void tbSpeed_Scroll(object sender, EventArgs e)
    {
        await _player.Rate_SetAsync(tbSpeed.Value / 10.0);
    }
```

### Gestion des erreurs et nettoyage

```csharp
    private void Player_OnError(object sender, ErrorsEventArgs e)
    {
        Invoke(() => edLog.Text += e.Message + Environment.NewLine);
    }

    private void Player_OnStop(object sender, StopEventArgs e)
    {
        Invoke(() => tbTimeline.Value = 0);
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _timer.Stop();

        if (_player != null)
        {
            _player.OnError -= Player_OnError;
            _player.OnStop -= Player_OnStop;
            await _player.DisposeAsync();
        }

        VisioForgeX.DestroySDK();
    }
}
```

## Alternative : pipeline Media Blocks SDK

Si vous avez besoin de la flexibilité supplémentaire d'un graphe de blocs (traitement personnalisé, plusieurs puits, superpositions), le [Guide du lecteur Media Blocks SDK](../../mediablocks/GettingStarted/player.md) construit un pipeline équivalent avec `UniversalSourceBlock` + `VideoRendererBlock` + `AudioRendererBlock`. Le code ci-dessus utilise Media Player SDK parce qu'il est conçu pour ce scénario — une seule instance de `MediaPlayerCoreX` remplace tout le câblage des blocs.

## Formats pris en charge

| Catégorie | Formats |
|----------|---------|
| Vidéo | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Audio | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Streaming | RTSP, HTTP, HLS, MPEG-DASH |

## Applications d'exemple

- [Simple Player Demo WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo)
- [Media Player Code Snippet (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/media-player)
- [WinForms Main Demo (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Main%20Demo)

## Foire aux questions

### Quels formats vidéo le lecteur vidéo .NET prend-il en charge ?

Le SDK prend en charge tous les principaux formats vidéo, notamment MP4, AVI, MKV, WMV, WebM, MOV, TS et FLV. La prise en charge des codecs inclut H.264, H.265/HEVC, VP8, VP9, AV1 et MPEG-2 via le moteur intégré basé sur GStreamer. Les formats audio tels que MP3, AAC, WAV, FLAC et OGG sont également pris en charge pour la lecture.

### Puis-je lire des flux RTSP ou de la vidéo réseau dans mon application C# ?

Oui. La méthode `UniversalSourceSettings.CreateAsync` accepte des URI pour les flux RTSP, HTTP, HLS et MPEG-DASH. Passez l'URL du flux comme objet `Uri`, exactement comme vous le feriez pour un chemin de fichier local. Pour les sources RTSP qui nécessitent une authentification, incluez les identifiants directement dans l'URI (par exemple `rtsp://user:password@host:554/stream`).

### Comment contrôler la vitesse de lecture dans le lecteur vidéo ?

Appelez `Rate_SetAsync(double rate)` sur l'instance du lecteur. Une vitesse de 1.0 est la vitesse normale, 2.0 est deux fois plus rapide et 0.5 est la moitié de la vitesse. La plage prise en charge dépend du format multimédia, mais la plupart des fichiers prennent en charge des vitesses entre 0.25x et 4.0x. La vitesse peut être changée pendant la lecture sans arrêter la vidéo.

### Le SDK prend-il en charge le rendu des sous-titres ?

Oui. Le SDK peut rendre les sous-titres intégrés depuis des conteneurs MKV et MP4 ainsi que les fichiers de sous-titres externes SRT et ASS. Les pistes de sous-titres sont détectées automatiquement lorsqu'un fichier est ouvert, et vous pouvez sélectionner la piste à afficher via l'API du lecteur.

### Puis-je construire un lecteur vidéo multiplateforme avec ce SDK ?

Oui. Le SDK fonctionne sur Windows, macOS, Linux, Android et iOS. Pour des applications bureau multiplateformes, utilisez le [framework UI Avalonia](avalonia-player.md) avec la même API `MediaPlayerCoreX`. Le moteur de lecture principal est identique sur toutes les plateformes — seule la surface de rendu vidéo et les paquets NuGet diffèrent.

## Voir aussi

- [Construire un lecteur vidéo en VB.NET](video-player-vb-net.md) — même tutoriel avec la syntaxe VB.NET
- [Lecteur multiplateforme Avalonia](avalonia-player.md) — créez un lecteur vidéo pour Windows, macOS et Linux avec Avalonia UI
- [Guide du lecteur .NET MAUI](maui-player.md) — une seule base de code C# pour iOS, Android, macOS et Windows
- [Guide du lecteur Android](android-player.md) — configuration et déploiement du lecteur spécifiques à Android
- [Mode boucle et plage de position](loop-and-position-range.md) — configurez la lecture en boucle, la répétition A-B et la lecture par segments
- [Lecteur Media Blocks SDK](../../mediablocks/GettingStarted/player.md) — alternative basée sur un pipeline avec rendus explicites
- [Page produit Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net)
