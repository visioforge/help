---
title: Lire une vidéo depuis un flux ou tableau d'octets en C#
description: Lisez une vidéo depuis un Stream .NET ou un tableau d'octets en C# WPF avec Media Player SDK .NET — MemoryStreamSource, ManagedIStream, Memory_DS.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCore
  - Windows
  - DirectShow
  - Playback
  - WPF
  - C#
  - NuGet
primary_api_classes:
  - MemoryStreamSource
  - MediaPlayerSourceMode
  - ManagedIStream
  - MediaPlayerCore
video:
  id: zSu_50b7IVQ
  upload_date: 2026-11-12T09:06:24Z
  duration: PT11M32S

---

# Lire une vidéo depuis la mémoire en C# WPF

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce tutoriel crée un lecteur WPF qui lit une vidéo depuis un `Stream` .NET au lieu d'un nom de fichier, avec [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) et son moteur `MediaPlayerCore`. L'application lit le fichier de deux façons : comme un `FileStream`, ou comme un tableau d'octets lu en entier en mémoire et enveloppé dans un `MemoryStream`. Le même code lit tout flux lisible et positionnable : un téléchargement copié en mémoire, un blob de base de données ou un fichier déchiffré en mémoire.

`MediaPlayerCore` est le moteur DirectShow du SDK : l'application fonctionne donc uniquement sous Windows.

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/zSu_50b7IVQ?controls=1" title="Lire une vidéo depuis la mémoire en C# WPF - tutoriel YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_VideoTutorials/memory-playback-wpf)

Le film de la vidéo est [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, sous licence [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

La vidéo crée le projet en .NET 10, qui nécessite Visual Studio 2026 ; dans Visual Studio 2022, choisissez .NET 8 ou .NET 9.

## Paquets NuGet requis

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) — le SDK avec `MediaPlayerCore` et le `VideoView` WPF
- [VisioForge.DotNet.Core.Redist.MediaPlayer.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x64/) — les filtres DirectShow du SDK, dont la source qui lit le flux
- [VisioForge.DotNet.Core.Redist.LAV.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/) — le séparateur et les décodeurs LAV, qui trouvent les pistes vidéo et audio dans le flux et les décodent

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.12]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.MediaPlayer.x64" Version="[2026.11.12]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.LAV.x64" Version="[2026.11.12]" />
```

Ce sont les paquets x64 : le projet définit donc `<PlatformTarget>x64</PlatformTarget>` ; une application 32 bits prend les paquets `.x86`. Consultez le [guide de déploiement](../deployment.md) pour la liste complète.

## La fenêtre en XAML

Le préfixe `vf` associe l'espace de noms `VisioForge.Core.UI.WPF` de l'assembly `VisioForge.Core`. Le `VideoView` occupe la colonne de gauche, avec la timeline et les boutons de lecture en dessous ; la colonne de droite contient le nom du fichier, deux boutons radio qui choisissent la façon de lire le fichier et le journal des erreurs. Play est activé dès le départ : `MediaPlayerCore` n'a pas besoin d'initialiser le SDK.

```xml
<Window x:Class="MemoryPlaybackWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Memory Playback" Height="600" Width="1000"
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
            <Button x:Name="btPlay" Content="Play" Width="90" Click="btPlay_Click" />
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

            <Label Content="Read the file as" Margin="0,10,0,0" />
            <RadioButton x:Name="rbStream" Content="Stream (FileStream)" IsChecked="True" />
            <RadioButton x:Name="rbByteArray" Content="Byte array (MemoryStream)" Margin="0,5,0,0" />

            <Label Content="Log" Margin="0,10,0,0" />
            <TextBox x:Name="edLog" Height="150" IsReadOnly="True" TextWrapping="Wrap"
                     VerticalScrollBarVisibility="Auto" />
        </StackPanel>
    </Grid>
</Window>
```

## Le code de la fenêtre

`MainWindow.xaml.cs`. Le projet active `ImplicitUsings` : `System`, `System.Threading.Tasks` et les autres espaces de noms courants ne figurent donc pas dans la liste.

```cs
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Win32;
using VisioForge.Core;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;
using VisioForge.Core.Types.MediaPlayer;

namespace MemoryPlaybackWpf;

public partial class MainWindow : Window
{
    private MediaPlayerCore _player;

    private Stream _source; // le lecteur le lit pendant la lecture : fermé seulement après l'arrêt

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(500) };

    private bool _timerMovesSlider; // true tant que le minuteur, et non l'utilisateur, déplace la timeline

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _player = new MediaPlayerCore(VideoView1);
        _player.OnError += (_, args) => Dispatcher.InvokeAsync(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Déclenché par StopAsync et à la fin du fichier ; traité seulement pour la fin du fichier
        // (Stop activé), pas pour un arrêt tardif du fichier précédent (State n'est plus Free)
        _player.OnStop += (_, _) => Dispatcher.InvokeAsync(() =>
        {
            if (btStop.IsEnabled && _player.State() == PlaybackState.Free) Stopped();
        });

        _timer.Tick += Timer_Tick;
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

        try
        {
            _source = rbByteArray.IsChecked == true
                ? new MemoryStream(await File.ReadAllBytesAsync(edFilename.Text)) // le fichier entier en mémoire
                : new FileStream(edFilename.Text, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            edLog.AppendText(ex.Message + Environment.NewLine);
            ShowPlaying(false);
            return;
        }

        // Un flux .NET positionnable, enveloppé dans l'IStream COM que lit le lecteur ; le
        // séparateur LAV y trouve la vidéo et l'audio
        _player.Source_MemoryStream = new MemoryStreamSource(new ManagedIStream(_source), _source.Length);
        _player.Source_Mode = MediaPlayerSourceMode.Memory_DS;

        if (!await _player.PlayAsync() || _player.State() == PlaybackState.Free)
        {
            Stopped(); // pas de lecture (la raison est dans le journal), ou déjà terminée
            return;
        }

        slTimeline.Maximum = (await _player.Duration_TimeAsync()).TotalSeconds;
        ShowPlaying(true);
        _timer.Start();
    }

    private async void btPause_Click(object sender, RoutedEventArgs e)
    {
        btPause.IsEnabled = false;
        var paused = await _player.PauseAsync();
        if (!btStop.IsEnabled) return; // arrêté entre-temps
        btResume.IsEnabled = paused;
        btPause.IsEnabled = !paused;
    }

    private async void btResume_Click(object sender, RoutedEventArgs e)
    {
        btResume.IsEnabled = false;
        var resumed = await _player.ResumeAsync();
        if (!btStop.IsEnabled) return; // arrêté entre-temps
        btPause.IsEnabled = resumed;
        btResume.IsEnabled = !resumed;
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
        btPause.IsEnabled = btStop.IsEnabled = playing;
        btResume.IsEnabled = false;
    }

    private void Stopped()
    {
        _timer.Stop();
        ShowPlaying(false);

        _source?.Dispose(); // le lecteur l'a libéré
        _source = null;

        lbTime.Text = "00:00:00 / 00:00:00";
        _timerMovesSlider = true;
        slTimeline.Value = 0;
        _timerMovesSlider = false;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_Get_TimeAsync();
        if (!_timer.IsEnabled || slTimeline.IsMouseCaptureWithin) return; // arrêté, ou l'utilisateur fait glisser

        var duration = TimeSpan.FromSeconds(slTimeline.Maximum); // lue une fois, après PlayAsync
        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        _timerMovesSlider = true;
        slTimeline.Value = position.TotalSeconds;
        _timerMovesSlider = false;
    }

    // Un clic sur la piste se déplace aussitôt ; un glissement se déplace une fois, à la fin
    private async void slTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_timerMovesSlider && !slTimeline.IsMouseCaptureWithin) await SeekAsync();
    }

    private async void slTimeline_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (!e.Canceled && e.HorizontalChange != 0) await SeekAsync(); // pas un appui sans mouvement
    }

    private async Task SeekAsync()
    {
        if (btStop.IsEnabled) await _player.Position_Set_TimeAsync(TimeSpan.FromSeconds(slTimeline.Value));
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        _timer.Stop();
        _player.Dispose(); // arrête la lecture ; Stopped() ferme ensuite le flux, ou la sortie de l'application
    }
}
```

## Fonctionnement

1. **La source** — avec *Byte array* coché, `File.ReadAllBytesAsync` lit le fichier entier en mémoire et un `MemoryStream` enveloppe le tableau ; sinon, un `FileStream` ouvert avec `FileAccess.Read` lit le fichier au fur et à mesure des demandes du lecteur. Un fichier absent ou verrouillé va dans le journal, et la fenêtre reste arrêtée.
2. **Lecture depuis la mémoire** — `ManagedIStream` (espace de noms `VisioForge.Core`) enveloppe le flux .NET dans l'`IStream` COM que lit le moteur DirectShow. `MemoryStreamSource` le reçoit avec la longueur du flux, et `Source_Mode = MediaPlayerSourceMode.Memory_DS` fait lire le lecteur depuis celui-ci. Avec le paquet LAV installé, le séparateur LAV trouve les pistes vidéo et audio dans le flux : le constructeur à deux arguments suffit.
3. **Lecture** — `PlayAsync` renvoie `false` quand le flux ne peut pas être lu, et la raison arrive par `OnError`. Si `State()` renvoie `PlaybackState.Free`, la lecture n'a pas démarré, ou un fichier très court est déjà terminé. Sinon, `Duration_TimeAsync` est lue une fois, pour l'échelle de la timeline.
4. **La durée de vie du flux** — le lecteur lit le flux et s'y positionne tant qu'il joue : `Stopped()` est donc le seul endroit qui le libère, après `StopAsync` ou quand `OnStop` signale la fin du fichier. Un bloc `using` autour de `PlayAsync` fermerait le flux pendant la lecture de la vidéo.
5. **Timeline** — un `DispatcherTimer` lit `Position_Get_TimeAsync` deux fois par seconde sur le thread de l'interface. L'indicateur `_timerMovesSlider` distingue les mouvements du minuteur de ceux de l'utilisateur : le minuteur ne déplace jamais la lecture. Un clic sur la piste se positionne aussitôt ; un glissement se positionne une fois, dans `Thumb.DragCompleted`. `Position_Set_TimeAsync` se positionne dans le flux.
6. **Arrêt et fermeture** — `OnStop` se déclenche après `StopAsync` et à la fin du fichier, sur un autre thread : le gestionnaire revient donc sur le thread de l'interface avec `Dispatcher.InvokeAsync`, comme `OnError`. Il n'agit que tant que Stop est activé et que `State()` vaut `PlaybackState.Free`. La fermeture de la fenêtre appelle `Dispose`, qui arrête la lecture et libère le lecteur.

## Remarques

- Le flux doit être lisible et positionnable, et sa `Length` doit être connue : le lecteur s'y positionne pour trouver les pistes et suivre la timeline. Un flux réseau ou de décompression non positionnable est d'abord copié dans un `MemoryStream`.
- Un tableau d'octets garde tout le fichier en mémoire. Pour un gros fichier, un `FileStream` ne lit que ce dont le lecteur a besoin.
- Sans le paquet LAV, le lecteur ne peut pas savoir ce que contient le flux : utilisez le constructeur `MemoryStreamSource(IStream stream, bool? videoPresent, bool? audioPresent, long size)` et réglez les deux indicateurs selon ce que le flux contient réellement.
- `ManagedIStream` lit à travers n'importe quel `Stream` : une classe de flux personnalisée — un flux qui déchiffre, par exemple — se lit de la même façon, à condition d'être lisible et positionnable.

## Applications d'exemple

- [Tutoriel de lecture depuis la mémoire WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_VideoTutorials/memory-playback-wpf) — le code de cette page et de la vidéo
- [Démo Memory Stream WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Memory%20Stream)
- [Démo Encrypted Memory Playback WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Encrypted%20Memory%20Playback%20Demo)

## Foire aux questions

### Puis-je lire une vidéo depuis un tableau d'octets en C# ?

Oui. Enveloppez le tableau dans un `MemoryStream`, celui-ci dans `ManagedIStream`, et passez-le à `MemoryStreamSource` avec la longueur du tableau. Réglez `Source_Mode` sur `MediaPlayerSourceMode.Memory_DS` et appelez `PlayAsync`. Gardez le `MemoryStream` ouvert jusqu'à l'arrêt de la lecture.

### Pourquoi la lecture s'arrête-t-elle ou échoue-t-elle juste après le démarrage ?

Le plus souvent, le flux est libéré trop tôt, par exemple par un bloc `using` autour de `PlayAsync`, ou le flux n'est pas positionnable. Gardez le flux dans un champ et libérez-le après `StopAsync` ou à la fin du fichier.

### Puis-je lire un flux téléchargé depuis le réseau ?

Copiez-le d'abord dans un `MemoryStream`. Un flux réseau n'est pas positionnable et n'a pas de longueur, et le lecteur a besoin des deux.

### Quels formats se lisent depuis la mémoire ?

Ceux que le séparateur et les décodeurs LAV lisent depuis un fichier : MP4, MKV, AVI, MOV, WMV, WebM, MPEG-TS et d'autres. Le flux contient les mêmes octets que le fichier.

## Transcription de la vidéo

### Ce que nous construisons

Lisons une vidéo depuis la mémoire dans une application C# WPF, avec le VisioForge Media Player SDK. Voici ce que nous construisons. Un lecteur WPF qui lit un flux .NET au lieu d'un nom de fichier. Soit un FileStream, soit le fichier entier lu dans un tableau d'octets et enveloppé dans un MemoryStream. Il a Play, Pause, Resume et Stop, et une timeline pour se déplacer. Le même code lit un flux obtenu n'importe où ailleurs : un téléchargement, une base de données ou un fichier que vous avez déchiffré en mémoire. Voici le lecteur terminé. Le film est Big Buck Bunny, de la Blender Foundation, sous licence Creative Commons Attribution.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet est une application WPF sur .NET 10. Et un fichier vidéo à lire. Le lecteur ici est MediaPlayerCore, le moteur DirectShow du SDK, donc cette application fonctionne sous Windows.

### Créer un projet WPF

Dans Visual Studio, créez un nouveau projet. Recherchez WPF, et choisissez WPF Application pour C#. Nommez le projet MemoryPlaybackWpf. Choisissez .NET 10 et cliquez sur Create.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet. VisioForge.DotNet.Core contient MediaPlayerCore. Redist MediaPlayer contient les filtres DirectShow du SDK, dont la source qui lit le flux. Et Redist LAV contient le séparateur et les décodeurs LAV, qui trouvent la vidéo et l'audio dans le flux et les décodent. Ou utilisez le gestionnaire de paquets NuGet : clic droit sur le projet, Manage NuGet Packages, puis installez VisioForge.DotNet.Core depuis l'onglet Browse. Installez ensuite les redists MediaPlayer et LAV pour x64. L'onglet Installed liste les trois. Ce sont les paquets x64, donc réglez PlatformTarget sur x64 dans le fichier projet. Une application 32 bits prend plutôt les paquets x86.

### Construire la fenêtre en XAML

La fenêtre est écrite en XAML. D'abord la fenêtre elle-même, avec une grille et un VideoView, et le concepteur la dessine aussitôt. Associez l'espace de noms VisioForge.Core.UI.WPF à un préfixe, ici vf. La grille a deux colonnes, la vidéo et une colonne de réglages de 260 pixels, et trois lignes : la vidéo, la timeline et les boutons. Le VideoView remplit la première cellule. La fenêtre nomme aussi ses gestionnaires Loaded et Closing. Sous la vidéo vient la timeline. Un curseur pour la position, avec le temps à sa droite. IsMoveToPointEnabled fait qu'un clic sur la piste saute directement à ce point. Le curseur nomme deux gestionnaires : ValueChanged, et DragCompleted de son thumb. Puis les boutons de lecture. Play, Pause, Resume et Stop, chacun avec un gestionnaire Click. Play est activé dès le départ, car ce moteur n'a pas besoin d'initialiser le SDK. Les trois autres sont désactivés au départ, jusqu'à ce qu'un fichier soit lu. Enfin, la colonne de réglages à droite. Elle contient le nom du fichier avec un bouton de parcours, et deux boutons radio qui choisissent comment l'application lit le fichier : comme un FileStream, ou comme un tableau d'octets dans un MemoryStream. Stream est coché au départ. Puis une zone de texte en lecture seule pour le journal des erreurs. Chaque gestionnaire nommé par le XAML va dans MainWindow.xaml.cs, et les chapitres suivants les écrivent.

### Créer le lecteur

MainWindow.xaml.cs commence par ces using : System.IO pour les flux, des espaces de noms WPF pour le thumb du curseur et le minuteur, Microsoft.Win32 pour la boîte de dialogue de fichier, et les espaces de noms VisioForge. La fenêtre garde le lecteur, le flux qu'il lit, un minuteur pour la timeline, et un indicateur. Au chargement de la fenêtre, créez MediaPlayerCore et passez-lui le VideoView, où la vidéo est dessinée. Les erreurs arrivent dans l'événement OnError sur un autre thread, donc le gestionnaire revient sur le thread de l'interface et les ajoute au journal. OnStop se déclenche après StopAsync, et quand le fichier arrive à sa fin. Le gestionnaire n'agit que tant que Stop est activé et que State vaut Free, c'est-à-dire à la fin du fichier. Ensuite Window_Loaded branche le gestionnaire Tick du minuteur. Le bouton de parcours affiche un OpenFileDialog filtré sur les fichiers vidéo, et met le chemin choisi dans la zone du nom de fichier.

### Lire depuis la mémoire

Le gestionnaire de Play désactive d'abord les boutons et vide le journal. Puis il ouvre la source. Avec Byte array coché, ReadAllBytesAsync lit le fichier entier en mémoire, et un MemoryStream enveloppe le tableau. Sinon, un FileStream lit le fichier au fur et à mesure des besoins du lecteur. Un fichier absent ou verrouillé va dans le journal, et la fenêtre reste à l'arrêt. Voici la partie qui lit depuis la mémoire. ManagedIStream enveloppe le flux .NET dans l'IStream COM que lit le moteur DirectShow. MemoryStreamSource le prend avec la longueur du flux, et Source_Mode réglé sur Memory_DS fait lire le lecteur à partir de lui. Le flux doit permettre la lecture et le déplacement. Le séparateur LAV y trouve les pistes vidéo et audio, vous n'avez donc pas à dire au lecteur ce que contient le flux. PlayAsync renvoie false quand le flux ne peut pas être lu, et la raison arrive dans OnError. State Free signifie que la lecture n'a pas démarré, ou qu'un fichier très court est déjà terminé, et la fenêtre revient à l'arrêt. Sinon, lisez la durée une fois, pour l'échelle de la timeline, passez les boutons en mode lecture et démarrez le minuteur.

### Pause, reprise et arrêt

Pause se désactive, attend PauseAsync et active Resume une fois le lecteur en pause. Si la lecture s'est arrêtée entre-temps, il ne touche pas aux boutons. Resume fait de même, avec ResumeAsync. Stop arrête le minuteur, désactive les boutons pendant qu'il travaille, attend StopAsync et réinitialise la fenêtre. Deux petites méthodes règlent les boutons. ShowBusy désactive les quatre. ShowPlaying active soit Play, soit Pause et Stop. Stopped remet la fenêtre en place, et c'est le seul endroit qui ferme le flux. Le lecteur lit le flux tant qu'il joue, donc libérez-le seulement une fois le lecteur arrêté, jamais juste après PlayAsync. Puis l'étiquette de temps et la timeline reviennent à zéro.

### La timeline

Le minuteur se déclenche deux fois par seconde sur le thread de l'interface. Chaque tick lit Position_Get_TimeAsync. Tant que l'utilisateur tient le curseur, le tick n'y touche pas. Sinon, il met à jour l'étiquette de temps et déplace le curseur, avec l'indicateur levé, pour que ce mouvement ne provoque pas de déplacement dans la vidéo. Un clic sur la piste se déplace aussitôt, dans ValueChanged. Un glissement se déplace une seule fois, au relâchement du thumb, dans DragCompleted. Les deux appellent Position_Set_TimeAsync pendant la lecture d'un fichier. Le déplacement fonctionne aussi sur un flux en mémoire, car le flux le permet.

### Fermer la fenêtre

Quand la fenêtre se ferme, Dispose arrête la lecture et libère le lecteur. Le flux est fermé par Stopped, ou avec l'application.

### Lancer l'application

Lancez l'application depuis Visual Studio. Cliquez sur le bouton de parcours et choisissez le fichier vidéo. Stream est coché, donc Play lit le fichier via un FileStream. L'étiquette de temps affiche la position et la durée. Un clic sur la timeline se déplace dans le flux. Pause fige l'image, Resume reprend au même point, et Stop réinitialise la fenêtre. Cochez maintenant Byte array et cliquez sur Play. L'application lit d'abord le fichier entier en mémoire, et le lecteur le joue depuis le MemoryStream, sans accès au fichier pendant la lecture. Arrêtez, puis fermez la fenêtre.

### Erreurs fréquentes

Quelques erreurs fréquentes. Libérer le flux pendant que le lecteur le lit encore, par exemple avec un bloc using autour de PlayAsync : fermez-le seulement après l'arrêt. Un flux qui ne permet pas le déplacement, comme un flux réseau : le lecteur doit se déplacer et connaître la longueur, alors copiez-le d'abord dans un MemoryStream. Oublier le paquet LAV : sans lui, le lecteur ne sait pas ce que contient le flux. Un tableau d'octets pour un fichier énorme, qui occupe autant de mémoire : préférez-y un FileStream. Et mélanger les paquets x86 et x64.

### Étapes suivantes

Il existe aussi des guides pour un lecteur vidéo complet avec volume et vitesse, pour lire seulement un fragment d'un fichier, pour afficher la première image d'une vidéo, et pour déployer les redists avec votre application. Le code source complet est sur GitHub, et la documentation est sur visioforge.com/help. Merci de votre attention.

## Voir aussi

- [Créer un lecteur vidéo en C#](../guides/video-player-csharp.md) — un lecteur complet avec volume, vitesse et sortie audio
- [Lire un fragment de fichier](play-fragment-file.md) — lire seulement une plage d'un fichier multimédia
- [Afficher la première image d'une vidéo](show-first-frame.md) — afficher la première image d'une vidéo en aperçu
- [Guide de déploiement](../deployment.md) — les paquets NuGet et les installateurs dont l'application a besoin
- [Page produit Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net)
