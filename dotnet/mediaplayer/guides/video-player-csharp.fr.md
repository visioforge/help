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
  - AudioRendererSettings
  - ErrorsEventArgs
video:
  id: YJbUV370TKo
  upload_date: 2026-11-12T05:22:05Z
  duration: PT12M7S

---

# Construire un lecteur vidéo C# pour WinForms et WPF

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce tutoriel construit un lecteur vidéo pour **applications bureau Windows** en C# avec [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) et son moteur `MediaPlayerCoreX` : une fenêtre WPF qui lit un fichier avec une timeline, la pause et la reprise, le volume, la vitesse de lecture, le choix de la sortie audio et un journal d'erreurs. Le même lecteur sous forme de formulaire WinForms se trouve [plus bas sur cette page](#deuxieme-option-winforms).

!!! info "Choisissez votre approche"
    Cette page concerne les **applications bureau Windows** (WinForms ou WPF) utilisant Media Player SDK .NET.

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

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/YJbUV370TKo?controls=1" title="Créer un lecteur vidéo en C# WPF - tutoriel YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/_VideoTutorials/video-player-csharp-wpf)

Le film de la vidéo est [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, sous licence [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

La vidéo crée le projet en .NET 10, qui nécessite Visual Studio 2026 ; dans Visual Studio 2022, choisissez .NET 8 ou .NET 9.

## Paquets NuGet requis

- [VisioForge.DotNet.MediaPlayer](https://www.nuget.org/packages/VisioForge.DotNet.MediaPlayer/) — le SDK ; les contrôles `VideoView` WPF et WinForms y sont inclus
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — le runtime GStreamer sur lequel fonctionne `MediaPlayerCoreX`
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — les codecs FFmpeg (libav) par lesquels de nombreux formats sont décodés

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="[2026.11.12]"/>
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.12]"/>
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.12]"/>
```

Ce sont les paquets x64 : le projet définit donc `<PlatformTarget>x64</PlatformTarget>` ; une application 32 bits utilise les paquets `.x86`. Consultez [Déploiement sous Windows](../../deployment-x/Windows.md) pour la liste complète.

## La fenêtre en XAML

Le préfixe `vf` associe l'espace de noms `VisioForge.Core.UI.WPF` de l'assembly `VisioForge.Core`. Le `VideoView` occupe la colonne de gauche, avec la timeline et les boutons de lecture en dessous ; la colonne de droite contient le nom du fichier, la sortie audio, les curseurs de volume et de vitesse et le journal d'erreurs. Play reste désactivé tant que le SDK n'est pas chargé.

```xml
<Window x:Class="VideoPlayerWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Video Player" Height="600" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="260" />
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <vf:VideoView x:Name="VideoView1" Background="Black" />

        <DockPanel Grid.Row="1" Margin="0,10,0,0">
            <TextBlock x:Name="lbTime" DockPanel.Dock="Right" Text="00:00:00 / 00:00:00"
                       Margin="10,0,0,0" VerticalAlignment="Center" />
            <Slider x:Name="slTimeline" IsMoveToPointEnabled="True"
                    ValueChanged="slTimeline_ValueChanged"
                    Thumb.DragCompleted="slTimeline_DragCompleted" />
        </DockPanel>

        <StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10,0,0">
            <Button x:Name="btPlay" Content="Play" Width="90" IsEnabled="False" Click="btPlay_Click" />
            <Button x:Name="btPause" Content="Pause" Width="90" Margin="10,0,0,0" IsEnabled="False"
                    Click="btPause_Click" />
            <Button x:Name="btResume" Content="Resume" Width="90" Margin="10,0,0,0" IsEnabled="False"
                    Click="btResume_Click" />
            <Button x:Name="btStop" Content="Stop" Width="90" Margin="10,0,0,0" IsEnabled="False"
                    Click="btStop_Click" />
        </StackPanel>

        <StackPanel Grid.Column="1" Grid.RowSpan="3" Margin="10,0,0,0">
            <Label Content="File" />
            <DockPanel>
                <Button x:Name="btOpen" Content="..." DockPanel.Dock="Right" Width="30" Margin="5,0,0,0"
                        Click="btOpen_Click" />
                <TextBox x:Name="edFilename" />
            </DockPanel>

            <Label Content="Audio output" Margin="0,10,0,0" />
            <ComboBox x:Name="cbSpeakers" />

            <Label Content="Volume" Margin="0,10,0,0" />
            <Slider x:Name="slVolume" Maximum="100" Value="80" AutoToolTipPlacement="TopLeft"
                    ValueChanged="slVolume_ValueChanged" />

            <Label Content="Speed" Margin="0,10,0,0" />
            <Slider x:Name="slSpeed" Minimum="0.5" Maximum="2" Value="1" TickFrequency="0.25"
                    IsSnapToTickEnabled="True" TickPlacement="BottomRight"
                    AutoToolTipPlacement="TopLeft" AutoToolTipPrecision="2"
                    ValueChanged="slSpeed_ValueChanged" />

            <Label Content="Log" Margin="0,10,0,0" />
            <TextBox x:Name="edLog" Height="150" IsReadOnly="True" TextWrapping="Wrap"
                     VerticalScrollBarVisibility="Auto" />
        </StackPanel>
    </Grid>
</Window>
```

## Le code de la fenêtre

```csharp
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Win32;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;

namespace VideoPlayerWpf;

public partial class MainWindow : Window
{
    private MediaPlayerCoreX _player;

    private AudioOutputDeviceInfo[] _speakers;

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(500) };

    private bool _timerMovesSlider; // true tant que c'est le minuteur, et non l'utilisateur, qui déplace la timeline

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += (_, args) => Dispatcher.InvokeAsync(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Déclenché par StopAsync et à la fin du fichier ; seule la fin du fichier est traitée
        // (Stop est activé), pas un arrêt tardif du fichier précédent (State n'est plus Free)
        _player.OnStop += (_, _) => Dispatcher.InvokeAsync(() =>
        {
            if (btStop.IsEnabled && _player.State == PlaybackState.Free) Stopped();
        });

        _speakers = await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound);
        foreach (var device in _speakers)
            cbSpeakers.Items.Add(device.Name);

        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = 0;

        _timer.Tick += Timer_Tick;
        btPlay.IsEnabled = true;
    }

    private void btOpen_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Video files|*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.webm;*.ts|All files|*.*"
        };

        if (dialog.ShowDialog() == true) edFilename.Text = dialog.FileName;
    }

    private async void btPlay_Click(object sender, RoutedEventArgs e)
    {
        ShowBusy();
        edLog.Clear();

        if (cbSpeakers.SelectedIndex >= 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(_speakers[cbSpeakers.SelectedIndex]);
        _player.Audio_OutputDevice_Volume = slVolume.Value / 100;

        if (await _player.OpenAsync(edFilename.Text))
        {
            await _player.Rate_SetAsync(slSpeed.Value); // un fichier nouvellement ouvert démarre à 1.0
            await _player.PlayAsync();
        }

        // Free : le fichier ne s'est pas ouvert ou n'a pas été lu (la raison est dans le journal), ou il est déjà terminé
        if (_player.State == PlaybackState.Free)
        {
            Stopped();
            return;
        }

        ShowPlaying(true);
        _timer.Start();
    }

    private async void btPause_Click(object sender, RoutedEventArgs e)
    {
        await _player.PauseAsync();
    }

    private async void btResume_Click(object sender, RoutedEventArgs e)
    {
        await _player.ResumeAsync();
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        ShowBusy();
        await _player.StopAsync();
        Stopped();
    }

    private void ShowBusy()
    {
        btPlay.IsEnabled = btPause.IsEnabled = btResume.IsEnabled = btStop.IsEnabled = false;
    }

    private void ShowPlaying(bool playing)
    {
        btPlay.IsEnabled = !playing;
        btPause.IsEnabled = btResume.IsEnabled = btStop.IsEnabled = playing;
    }

    private void Stopped()
    {
        _timer.Stop();
        ShowPlaying(false);

        lbTime.Text = "00:00:00 / 00:00:00";
        _timerMovesSlider = true;
        slTimeline.Value = 0;
        _timerMovesSlider = false;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();
        if (!_timer.IsEnabled || slTimeline.IsMouseCaptureWithin) return; // arrêté, ou l'utilisateur fait glisser

        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        _timerMovesSlider = true;
        slTimeline.Maximum = duration.TotalSeconds;
        slTimeline.Value = position.TotalSeconds;
        _timerMovesSlider = false;
    }

    // Un clic sur la piste repositionne aussitôt ; un glissement repositionne une fois terminé
    private async void slTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_timerMovesSlider && !slTimeline.IsMouseCaptureWithin) await SeekAsync();
    }

    private async void slTimeline_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (!e.Canceled && e.HorizontalChange != 0) await SeekAsync(); // pas un appui sans déplacement
    }

    private async Task SeekAsync()
    {
        if (btStop.IsEnabled) await _player.Position_SetAsync(TimeSpan.FromSeconds(slTimeline.Value));
    }

    private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_player != null) _player.Audio_OutputDevice_Volume = slVolume.Value / 100;
    }

    private async void slSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (btStop.IsEnabled) await _player.Rate_SetAsync(slSpeed.Value); // sinon, Play l'applique
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (!btPlay.IsEnabled && !btStop.IsEnabled)
        {
            e.Cancel = true; // le SDK est encore en chargement, un fichier s'ouvre ou la lecture s'arrête
            return;
        }

        _timer.Stop();
        _player.Dispose(); // arrête d'abord la lecture
        VisioForgeX.DestroySDK();
    }
}
```

## Fonctionnement

1. **Initialisation du SDK** — `VisioForgeX.InitSDKAsync` s'exécute une fois, au chargement de la fenêtre. Le premier lancement sur un PC est plus long, c'est pourquoi Play n'est activé qu'une fois l'appel terminé. Un seul `MediaPlayerCoreX` sert pour tous les fichiers lus par l'application ; `Dispose` et `VisioForgeX.DestroySDK` les libèrent à la fermeture de la fenêtre.
2. **Sortie audio** — `Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)` liste les haut-parleurs et les casques, et `AudioRendererSettings` envoie le son vers celui choisi. `Audio_OutputDevice_Volume` accepte de 0.0 à 1.0 et peut changer pendant la lecture du fichier.
3. **Ouvrir et lire** — `OpenAsync` accepte un chemin de fichier ou une URL et renvoie `false` lorsque la source ne peut pas être ouverte ; la raison arrive par `OnError`. `PlayAsync` démarre la lecture. La vitesse est définie après `OpenAsync`, car un fichier nouvellement ouvert démarre toujours à 1.0. Ensuite, `State` indique si le fichier est lu : `PlaybackState.Free` signifie qu'il ne s'est pas ouvert ou n'a pas été lu, ou qu'un fichier très court est déjà terminé, et la fenêtre revient à l'état arrêté.
4. **Timeline** — un `DispatcherTimer` lit `Position_GetAsync` et `DurationAsync` deux fois par seconde sur le thread de l'interface. L'indicateur `_timerMovesSlider` distingue les mises à jour du minuteur de celles de l'utilisateur, de sorte que le minuteur ne repositionne jamais. Un clic sur la piste repositionne aussitôt ; un glissement repositionne une seule fois, dans `Thumb.DragCompleted`, et le minuteur ne touche pas au curseur tant que la souris le tient. Le positionnement fonctionne aussi en pause.
5. **Vitesse** — `Rate_SetAsync` définit la vitesse de lecture : 1.0 est la vitesse normale, 2.0 le double, 0.5 la moitié. Ici, le curseur va de 0.5 à 2 par pas de 0.25.
6. **Arrêt, fin du fichier et fermeture** — `OnStop` est déclenché après `StopAsync` et à la fin du fichier ; il arrive sur un autre thread, c'est pourquoi le gestionnaire revient sur le thread de l'interface avec `Dispatcher.InvokeAsync`, comme `OnError`. Play et Stop désactivent les boutons de lecture pendant leur travail, et le bouton Stop réinitialise lui-même la fenêtre après `StopAsync` : le gestionnaire n'agit donc que lorsque Stop est activé et que `State` vaut `PlaybackState.Free`, c'est-à-dire à la fin du fichier, et non pour un `OnStop` tardif du fichier précédent arrivé après le démarrage du suivant. La fermeture de la fenêtre pendant le chargement du SDK, l'ouverture d'un fichier ou l'arrêt de la lecture est annulée ; sinon, `Dispose` arrête la lecture et libère le lecteur.

## Remarques

- Le volume réglé avant Play est conservé et appliqué au démarrage de la lecture.
- Sans aucun périphérique audio dans la liste, le fichier est lu sans son.
- `OpenAsync` accepte aussi une URL `rtsp://`, `http://` ou HLS ; un flux en direct n'a pas de durée : le libellé de temps affiche la position, la timeline reste au début, et le positionnement dépend de la source.

## Deuxième option : WinForms

Le même lecteur dans un formulaire WinForms. Un `TrackBar` ne déclenche `Scroll` que lorsque l'utilisateur le déplace : le minuteur peut donc mettre à jour la timeline sans l'indicateur dont le curseur WPF a besoin, et un `System.Windows.Forms.Timer` s'exécute sur le thread de l'interface. Le `TrackBar` garde la souris tant qu'un bouton reste enfoncé dessus : un clic ou un glissement repositionne donc une seule fois, dans `MouseUp`. Le formulaire contient un `VideoView` de `VisioForge.Core.UI.WinForms` nommé `VideoView1`, les `TrackBar` `tbTimeline`, `tbVolume` (0–100) et `tbSpeed` (5–20, lu comme 0.5–2.0), un `ComboBox` `cbAudioOutput`, les boutons `btOpenFile`, `btStart`, `btPause`, `btResume` et `btStop` (tous sauf `btOpenFile` désactivés à la conception), le libellé `lbTime` et les zones de texte `edFilename` et `edLog` (multiligne, en lecture seule). Reliez `Scroll` sur les trois `TrackBar` et `MouseUp` sur `tbTimeline`, `Click` sur les boutons, et `Load` et `FormClosing` sur le formulaire.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;

public partial class Form1 : Form
{
    private MediaPlayerCoreX _player;

    private AudioOutputDeviceInfo[] _speakers;

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 500 };

    private bool _scrolled; // true quand l'utilisateur a déplacé la timeline avec le bouton de la souris enfoncé

    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += (_, args) => BeginInvoke(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Déclenché par StopAsync et à la fin du fichier ; seule la fin du fichier est traitée
        // (Stop est activé), pas un arrêt tardif du fichier précédent (State n'est plus Free)
        _player.OnStop += (_, _) => BeginInvoke(() =>
        {
            if (btStop.Enabled && _player.State == PlaybackState.Free) Stopped();
        });

        _speakers = await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound);
        foreach (var device in _speakers)
            cbAudioOutput.Items.Add(device.Name);

        if (cbAudioOutput.Items.Count > 0) cbAudioOutput.SelectedIndex = 0;

        _timer.Tick += Timer_Tick;
        btStart.Enabled = true;
    }

    private void btOpenFile_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Video files|*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.webm;*.ts|All files|*.*"
        };

        if (dialog.ShowDialog() == DialogResult.OK) edFilename.Text = dialog.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        ShowBusy();
        edLog.Clear();

        if (cbAudioOutput.SelectedIndex >= 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(_speakers[cbAudioOutput.SelectedIndex]);
        _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;

        if (await _player.OpenAsync(edFilename.Text))
        {
            await _player.Rate_SetAsync(tbSpeed.Value / 10.0); // un fichier nouvellement ouvert démarre à 1.0
            await _player.PlayAsync();
        }

        // Free : le fichier ne s'est pas ouvert ou n'a pas été lu (la raison est dans le journal), ou il est déjà terminé
        if (_player.State == PlaybackState.Free)
        {
            Stopped();
            return;
        }

        ShowPlaying(true);
        _timer.Start();
    }

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
        ShowBusy();
        await _player.StopAsync();
        Stopped();
    }

    private void ShowBusy()
    {
        btStart.Enabled = btPause.Enabled = btResume.Enabled = btStop.Enabled = false;
    }

    private void ShowPlaying(bool playing)
    {
        btStart.Enabled = !playing;
        btPause.Enabled = btResume.Enabled = btStop.Enabled = playing;
    }

    private void Stopped()
    {
        _timer.Stop();
        ShowPlaying(false);

        lbTime.Text = "00:00:00 / 00:00:00";
        tbTimeline.Value = 0;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();
        if (!_timer.Enabled || tbTimeline.Capture) return; // arrêté, ou l'utilisateur fait glisser

        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        // Affecter Value ne déclenche pas Scroll, cela ne repositionne donc pas
        tbTimeline.Maximum = (int)duration.TotalSeconds;
        tbTimeline.Value = Math.Min((int)position.TotalSeconds, tbTimeline.Maximum);
    }

    // Une touche repositionne aussitôt ; un clic ou un glissement repositionne une fois le bouton de la souris relâché
    private async void tbTimeline_Scroll(object sender, EventArgs e)
    {
        if (tbTimeline.Capture) _scrolled = true;
        else await SeekAsync();
    }

    private async void tbTimeline_MouseUp(object sender, MouseEventArgs e)
    {
        if (!_scrolled) return; // un appui sans déplacement ne repositionne pas

        _scrolled = false;
        await SeekAsync();
    }

    private async Task SeekAsync()
    {
        if (btStop.Enabled) await _player.Position_SetAsync(TimeSpan.FromSeconds(tbTimeline.Value));
    }

    private void tbVolume_Scroll(object sender, EventArgs e)
    {
        if (_player != null) _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;
    }

    private async void tbSpeed_Scroll(object sender, EventArgs e)
    {
        if (btStop.Enabled) await _player.Rate_SetAsync(tbSpeed.Value / 10.0); // sinon, Start l'applique
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true; // le SDK est encore en chargement, un fichier s'ouvre ou la lecture s'arrête
            return;
        }

        _timer.Stop();
        _player.Dispose(); // arrête d'abord la lecture
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

Oui. `OpenAsync` accepte une URL RTSP, HTTP, HLS ou MPEG-DASH aussi bien qu'un chemin de fichier : le même code lit donc un flux. Pour les sources RTSP qui nécessitent une authentification, incluez les identifiants directement dans l'URL (par ex. `rtsp://user:password@host:554/stream`).

### Comment contrôler la vitesse de lecture dans le lecteur vidéo ?

Appelez `Rate_SetAsync(double rate)` sur le lecteur après `OpenAsync`. Une vitesse de 1.0 est la vitesse normale, 2.0 est deux fois plus rapide et 0.5 est la moitié de la vitesse. La vitesse peut être changée pendant la lecture sans arrêter la vidéo. Après `StopAsync` ou la fin du fichier, la lecture suivante démarre à 1.0 : définissez donc à nouveau la vitesse après le prochain `OpenAsync`, comme le fait le code ci-dessus.

### Le SDK prend-il en charge le rendu des sous-titres ?

Oui. `Subtitles_Enabled` est activé par défaut, donc `OpenAsync` affiche la piste de sous-titres intégrée d'un fichier MKV ou MP4. Pour charger un fichier externe SRT, ASS ou WebVTT, définissez `Subtitles_ExternalFile` avant `OpenAsync` ; `Subtitles_Settings` règle l'apparence des sous-titres et `Subtitle_Streams` liste les pistes de sous-titres du fichier.

### Puis-je construire un lecteur vidéo multiplateforme avec ce SDK ?

Oui. Le SDK fonctionne sur Windows, macOS, Linux, Android et iOS. Pour des applications bureau multiplateformes, utilisez le [framework UI Avalonia](avalonia-player.md) avec la même API `MediaPlayerCoreX`. Le moteur de lecture principal est identique sur toutes les plateformes — seule la surface de rendu vidéo et les paquets NuGet diffèrent.

## Transcription de la vidéo

### Ce que nous construisons

Créons un lecteur vidéo dans une application C# WPF avec le VisioForge Media Player SDK. Voici ce que nous construisons. Une fenêtre WPF qui lit un fichier vidéo : MP4, MKV, AVI, MOV, WebM et bien d'autres formats. Elle a Play, Pause, Resume et Stop, une timeline sur laquelle vous pouvez cliquer ou faire glisser pour vous déplacer, des curseurs pour le volume et la vitesse de lecture, une liste des sorties audio et un journal des erreurs. Le code de la fenêtre fait environ cent quatre-vingts lignes de C#. Voici le lecteur terminé. Le film est Big Buck Bunny, de la Blender Foundation, sous licence Creative Commons Attribution.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail de développement .NET Desktop. Le projet cible .NET 8 ou une version ultérieure. Et un fichier vidéo à lire. Le guide écrit propose aussi le même lecteur sous forme de formulaire WinForms.

### Créer un projet WPF

Dans Visual Studio, créez un nouveau projet. Recherchez WPF, et choisissez WPF Application pour C#. Nommez le projet VideoPlayerWpf. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre MainWindow.xaml, avec le concepteur en haut et le XAML en dessous.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet. Le Media Player SDK lui-même. CrossPlatform Core, le runtime GStreamer sur lequel tourne le moteur X. Et CrossPlatform Libav, les codecs FFmpeg par lesquels de nombreux formats sont décodés. Ou utilisez le NuGet Package Manager : clic droit sur le projet, Manage NuGet Packages, puis installez VisioForge.DotNet.MediaPlayer depuis l'onglet Browse. Installez ensuite les paquets Core et Libav pour Windows x64. L'onglet Installed affiche les trois. Ce sont les paquets x64, donc réglez PlatformTarget sur x64 dans le fichier du projet. Une application 32 bits prend plutôt les paquets x86.

### Construire la fenêtre en XAML

La fenêtre est écrite en XAML. D'abord la fenêtre elle-même, avec une grille et un VideoView, et le concepteur la dessine aussitôt. Associez l'espace de noms VisioForge.Core.UI.WPF à un préfixe, ici vf. La grille a deux colonnes, la vidéo et une colonne de réglages de 260 pixels, et trois lignes : la vidéo, la timeline et les boutons. Le VideoView remplit la première cellule. La fenêtre nomme aussi ses gestionnaires Loaded et Closing. Sous la vidéo vient la timeline. Un curseur pour la position, avec le temps à sa droite. IsMoveToPointEnabled fait qu'un clic sur la piste saute directement à ce point. Le curseur nomme deux gestionnaires : ValueChanged, et DragCompleted de son pouce. Puis les boutons de lecture. Play, Pause, Resume et Stop, chacun avec un gestionnaire Click. Ils sont tous désactivés au départ : Play jusqu'au chargement du SDK, et les trois autres jusqu'à la lecture d'un fichier. Enfin, la colonne de réglages à droite. Elle contient le nom du fichier avec un bouton de parcours, et une liste déroulante pour la sortie audio. Puis un curseur de volume de 0 à 100, un curseur de vitesse de 0.5 à 2 par pas de 0.25, et une zone de texte en lecture seule pour le journal des erreurs. Chaque gestionnaire nommé par le XAML va dans MainWindow.xaml.cs, et les chapitres suivants les écrivent.

### Initialiser le SDK

MainWindow.xaml.cs commence par ces using : les espaces de noms WPF pour le pouce du curseur et le minuteur, Microsoft.Win32 pour la boîte de dialogue de fichier, et les espaces de noms VisioForge. Au chargement de la fenêtre, attendez VisioForgeX.InitSDKAsync une seule fois. Créez ensuite le moteur MediaPlayerCoreX et passez-lui le VideoView, où la vidéo est dessinée. Les erreurs arrivent dans l'événement OnError sur un autre thread, donc le gestionnaire revient au thread de l'interface et les ajoute au journal. Audio_OutputDevicesAsync avec DirectSound liste les haut-parleurs et les casques. Leurs noms remplissent la liste déroulante, et le premier est sélectionné. Play n'est activé que maintenant, une fois le SDK chargé. Le bouton de parcours affiche un OpenFileDialog filtré sur les fichiers vidéo, et place le chemin choisi dans la zone du nom de fichier.

### Ouvrir et lire

Le gestionnaire de Play désactive d'abord les boutons et vide le journal. AudioRendererSettings envoie le son vers la sortie sélectionnée. Audio_OutputDevice_Volume accepte de 0 à 1, donc la valeur du curseur est divisée par cent. OpenAsync prend le chemin du fichier, ou une URL, et renvoie false quand la source ne peut pas être ouverte. La raison arrive dans OnError. Réglez la vitesse après OpenAsync, car un fichier tout juste ouvert démarre toujours à vitesse normale. Puis PlayAsync lance la lecture. Vérifiez ensuite State. PlaybackState.Free signifie que le fichier ne s'est pas ouvert ou n'a pas été lu, ou qu'un fichier très court est déjà terminé, et la fenêtre revient à l'état arrêté. Sinon, les boutons passent en lecture, et le minuteur de la timeline démarre.

### Pause, reprise et arrêt

Pause et Resume sont un appel chacun : PauseAsync et ResumeAsync. Stop arrête le minuteur, désactive les boutons pendant son travail, attend StopAsync et réinitialise la fenêtre. Deux petites méthodes utilitaires règlent les boutons. ShowBusy désactive les quatre, et ShowPlaying active soit Play, soit les trois autres. Stopped remet la fenêtre en place : les boutons, l'étiquette du temps, et la timeline à zéro. L'indicateur autour de cette dernière ligne signale au gestionnaire de la timeline que ce déplacement ne vient pas de l'utilisateur. OnStop se déclenche après StopAsync, et quand le fichier arrive à sa fin. Le gestionnaire n'agit que tant que Stop est activé et que State vaut Free, c'est-à-dire à la fin du fichier. Un OnStop tardif du fichier précédent est ignoré, car le suivant est déjà en lecture.

### La timeline

La timeline suit un DispatcherTimer qui se déclenche deux fois par seconde sur le thread de l'interface. Son gestionnaire Tick est branché dans Window_Loaded. Chaque déclenchement lit Position_GetAsync et DurationAsync. Tant que l'utilisateur tient le curseur, le déclenchement n'y touche pas. Sinon, il met à jour l'étiquette du temps et déplace le curseur, avec l'indicateur activé, pour que ce déplacement ne provoque pas de saut. Un clic sur la piste saute aussitôt, dans ValueChanged. Un glissement saute une seule fois, au relâchement du pouce, dans DragCompleted. Un appui sur le pouce sans le déplacer ne provoque pas de saut. SeekAsync appelle Position_SetAsync tant qu'un fichier est ouvert. Cela fonctionne aussi en pause.

### Volume et vitesse

Le volume suit son curseur aussitôt, via Audio_OutputDevice_Volume. Rate_SetAsync change la vitesse pendant la lecture : 1 est la vitesse normale, 2 le double, et 0.5 la moitié. Quand rien n'est lu, Play applique la valeur du curseur.

### Fermer la fenêtre

À la fermeture de la fenêtre, Dispose arrête la lecture et libère le lecteur, et DestroySDK libère le SDK. Une fermeture pendant le chargement du SDK, l'ouverture d'un fichier ou l'arrêt de la lecture est annulée, pour que le lecteur ne soit jamais libéré au milieu d'un appel.

### Lancer l'application

Lancez l'application depuis Visual Studio. Une fois le SDK chargé, les sorties audio remplissent la liste et Play est activé. Cliquez sur le bouton de parcours et choisissez le fichier vidéo. Cliquez sur Play. L'étiquette du temps affiche la position et la durée du fichier. Un clic sur la timeline saute directement à ce point. Ou faites glisser le pouce : la lecture saute une seule fois, quand vous le relâchez. Le volume suit son curseur, et à une fois et demie la vitesse le film défile plus vite. Pause fige l'image, et Resume reprend au même point. Stop réinitialise la fenêtre. Puis fermez-la.

### Erreurs fréquentes

Quelques erreurs fréquentes. Oublier le paquet Libav, si bien que de nombreux formats ne se décodent pas. Mélanger des paquets x86 et x64, ou lancer une application x86 avec les paquets x64. Régler la vitesse avant OpenAsync : le nouveau fichier redémarre à vitesse normale. Laisser les déplacements du curseur par le minuteur provoquer des sauts, ce qui saccade la lecture. Et les erreurs arrivent dans l'événement OnError, sur un autre thread.

### Étapes suivantes

Il existe aussi des guides pour lire un fichier en boucle ou seulement un extrait, pour le même lecteur en VB.NET, et pour des lecteurs multiplateformes avec Avalonia, .NET MAUI et Android. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci de votre attention.

## Voir aussi

- [Construire un lecteur vidéo en VB.NET](video-player-vb-net.md) — même tutoriel avec la syntaxe VB.NET
- [Lecteur multiplateforme Avalonia](avalonia-player.md) — créez un lecteur vidéo pour Windows, macOS et Linux avec Avalonia UI
- [Guide du lecteur .NET MAUI](maui-player.md) — une seule base de code C# pour iOS, Android, macOS et Windows
- [Guide du lecteur Android](android-player.md) — configuration et déploiement du lecteur spécifiques à Android
- [Mode boucle et plage de position](loop-and-position-range.md) — configurez la lecture en boucle, la répétition A-B et la lecture par segments
- [Lecteur Media Blocks SDK](../../mediablocks/GettingStarted/player.md) — alternative basée sur un pipeline avec rendus explicites
- [Page produit Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net)
