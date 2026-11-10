---
title: Enregistrer une webcam en MP4 en C# WPF (H.264, AAC)
description: Une fenêtre WPF en XAML qui affiche la webcam et l'enregistre en MP4, H.264 sur GPU et AAC, avec Video Capture SDK .NET. Code C# et vidéo.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WPF
  - Capture
  - Webcam
  - MP4
  - H.264
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4HWOutput
  - VideoView
video:
  id: NNqmz2vJyQc
  upload_date: 2026-11-10T16:30:06Z
  duration: PT9M

---

# Capture vidéo de webcam en MP4 dans une application WPF

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la capture de webcam en WPF

Ce tutoriel enregistre une webcam et un micro dans un fichier MP4 depuis une application WPF. La fenêtre est décrite en XAML, l'aperçu en direct s'affiche dans un `VideoView`, et le moteur `VideoCaptureCore` encode en H.264 sur le GPU lorsque le PC dispose d'un encodeur matériel, avec un son AAC.

C'est la même application que dans le [tutoriel webcam vers MP4 en WinForms](video-capture-webcam-mp4.md), construite à la manière de WPF :

- La fenêtre, ses listes et ses boutons sont écrits en XAML
- Les gestionnaires des listes déroulantes lisent `SelectedItem`, car dans `SelectionChanged` la propriété `Text` contient encore l'élément précédent
- Fermer la fenêtre pendant un enregistrement finalise le fichier MP4 avant que la fenêtre ne disparaisse

## Tutoriel vidéo d'implémentation

La vidéo suivante construit l'application pas à pas :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/NNqmz2vJyQc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet est disponible dans notre dépôt GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-mp4-wpf)

## Composants redistribuables requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et les paquets redistribuables ci-dessous. Le `VideoView` WPF est fourni dans le paquet principal : aucun paquet d'interface séparé n'est nécessaire.

### Composants de capture vidéo

- Pour les applications 32 bits : [Redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Pour les applications 64 bits : [Redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Composants d'encodage MP4

- Pour les applications 32 bits : [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- Pour les applications 64 bits : [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## La fenêtre en XAML

Le préfixe `vf` associe l'espace de noms `VisioForge.Core.UI.WPF` de l'assembly `VisioForge.Core`. La colonne de gauche contient les listes et les boutons, celle de droite le `VideoView` où s'affiche l'aperçu en direct. Chaque gestionnaire nommé ici est écrit plus bas, dans `MainWindow.xaml.cs`.

```xml
<Window x:Class="WebcamMp4Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Webcam to MP4" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera" />
            <ComboBox x:Name="cbCamera" SelectionChanged="cbCamera_SelectionChanged" />
            <Label Content="Format" />
            <ComboBox x:Name="cbFormat" SelectionChanged="cbFormat_SelectionChanged" />
            <Label Content="Frame rate" />
            <ComboBox x:Name="cbFrameRate" />
            <Label Content="Microphone" />
            <ComboBox x:Name="cbMicrophone" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

## Exemple d'implémentation avec code C#

Le code de la fenêtre. Le projet cible .NET 10 avec les usings implicites ; un projet WPF n'y inclut pas `System.IO`, qui est donc ajouté pour `Path`.

```csharp
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamMp4Wpf;

public partial class MainWindow : Window
{
    private VideoCaptureCore videoCapture1;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Afficher les erreurs du SDK dans la barre de titre
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
    }

    private void cbCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == (string)cbCamera.SelectedItem);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cbFrameRate.Items.Clear();
        if (cbFormat.SelectedItem is null) return; // la liste vient d'être vidée

        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == (string)cbCamera.SelectedItem);
        var format = device.VideoFormats.First(f => f.Name == (string)cbFormat.SelectedItem);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        Title = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam to MP4";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource((string)cbCamera.SelectedItem);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = (string)cbFormat.SelectedItem;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // sans micro : enregistrer la vidéo seule
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // enregistrer le micro sans le lire

        var mp4 = new MP4HWOutput();

        // NVIDIA, puis Intel, puis AMD, puis l'encodeur du CPU
        var gpu = VideoCaptureCore.HWEncodersAvailable();
        mp4.Video.Codec =
            gpu.NVENC_H264 ? MFVideoEncoder.NVENC_H264 :
            gpu.QSV_H264 ? MFVideoEncoder.QSV_H264 :
            gpu.AMD_H264 ? MFVideoEncoder.AMD_H264 :
            MFVideoEncoder.MS_H264;

        mp4.Video.AvgBitrate = 6000; // kbps
        mp4.Video.MaxBitrate = 9000;
        mp4.Video.RateControl = MFCommonRateControlMode.PeakConstrainedVBR;
        mp4.Video.Profile = MFH264Profile.Main;
        mp4.Audio.Bitrate = 192; // AAC, kbps

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
        videoCapture1.Output_Format = mp4;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        videoCapture1.Video_Effects_Enabled = true;
        videoCapture1.Video_Effects_Clear();
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "text")
        {
            Text = "Hello World!", Left = 50, Top = 50, FontColor = System.Drawing.Color.Red
        });
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime, DateTimeMask = "HH:mm:ss", Text = "",
            Left = 50, Top = 110
        });

        btStart.IsEnabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.IsEnabled = started;
        btStart.IsEnabled = !started;
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
    }

    private async void Window_Closing(object sender, CancelEventArgs e)
    {
        if (btStart.IsEnabled) { videoCapture1.Dispose(); return; }

        e.Cancel = true; // enregistrement en cours : finaliser d'abord le MP4
        if (!btStop.IsEnabled) return; // démarrage ou arrêt déjà en cours
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // pas Close() : WPF lève une exception dans Closing
    }
}
```

## Fonctionnalités clés de l'implémentation

1. **Fenêtre en XAML** — Une `Grid` à deux colonnes : les listes et les boutons à gauche, le `VideoView` avec l'aperçu en direct à droite
2. **Sélection du périphérique et du format** — La caméra, son format, sa fréquence d'images et le micro sont choisis dans des listes remplies à partir des périphériques eux-mêmes ; les gestionnaires lisent `SelectedItem`
3. **Encodage matériel avec repli sur le CPU** — `MP4HWOutput` encode en H.264 sur un GPU NVIDIA, Intel ou AMD ; `HWEncodersAvailable` indique les encodeurs présents sur le PC, et `MS_H264` s'exécute sur le CPU s'il n'y en a aucun
4. **Paramètres de l'encodeur** — Une moyenne de 6000 kbps, des pointes jusqu'à 9000 kbps, le profil Main et l'audio AAC à 192 kbps
5. **Opération asynchrone** — `StartAsync` et `StopAsync` gardent l'interface réactive, et la fermeture de la fenêtre pendant un enregistrement attend `StopAsync` avant de fermer

## Remarques sur WPF

- `FontColor` de `VideoEffectTextLogo` est un `System.Drawing.Color`, pas un `Color` WPF : il est donc écrit en entier
- Le gestionnaire `OnError` modifie le titre de la fenêtre via `Dispatcher.InvokeAsync` : il s'exécute ainsi sur le thread de l'interface, quel que soit le thread qui déclenche l'événement
- `Format_UseBest` est activé par défaut et choisit la résolution la plus élevée jusqu'à 1080p, en privilégiant MJPG à résolution égale ; désactivez-le pour enregistrer le format choisi dans la liste
- Sans micro dans la liste, `Audio_RecordAudio` passe à `false` et le MP4 ne contient que la vidéo
- Lorsque le moteur s'est déjà arrêté (après une erreur), `StopAsync` se termine aussitôt et le code qui suit l'`await` s'exécute encore dans `Closing`, où WPF n'autorise pas `Close()` ; la fenêtre se referme donc via `Dispatcher.InvokeAsync`

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons une webcam dans un fichier MP4 depuis une application C# WPF avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une fenêtre WPF avec l'aperçu de la caméra en direct, décrite en XAML. Elle enregistre en MP4, avec du H.264 encodé sur le GPU quand le PC en a un, et de l'audio AAC. La caméra, son format et le micro se choisissent dans des listes, et le code de la fenêtre tient en une centaine de lignes de C#. Voici l'application terminée, qui enregistre depuis une Logitech C920.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail de développement .NET Desktop. Le projet cible .NET 8 ou version ultérieure. La même application en WinForms a sa propre vidéo.

### Créer un projet WPF

Dans Visual Studio, ouvrez File, New, Project. Recherchez WPF et choisissez WPF Application pour C#. Nommez le projet WebcamMp4Wpf. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre MainWindow.xaml, avec le concepteur en haut et le XAML en bas.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet : le SDK lui-même, le redist de capture vidéo et le redist MP4. Ce sont les paquets x64. Une application x86 utilise plutôt les redists x86. Vous pouvez aussi installer ces trois paquets depuis le gestionnaire de paquets NuGet. Le VideoView WPF est livré dans le paquet principal, il n'en faut donc pas d'autre.

### Construire la fenêtre en XAML

En WPF, la fenêtre s'écrit en XAML. D'abord la fenêtre elle-même, avec une grille à deux colonnes et un VideoView, et le concepteur au-dessus la dessine aussitôt. Le VideoView se trouve aussi dans la Toolbox, sous VisioForge. Associez l'espace de noms VisioForge.Core.UI.WPF à un préfixe, ici vf. Ensuite, un VideoView va dans la colonne de droite d'une grille : c'est là que s'affiche l'aperçu en direct. La fenêtre nomme aussi ses gestionnaires Loaded et Closing. Ensuite, les étiquettes et les listes déroulantes vont dans la colonne de gauche. La colonne de gauche est un StackPanel avec une étiquette et une liste déroulante pour la caméra, le format, la fréquence d'images et le micro. Les listes de la caméra et du format nomment un gestionnaire SelectionChanged. Enfin, les boutons Start et Stop. Chaque bouton nomme un gestionnaire Click. Stop reste désactivé jusqu'au début d'un enregistrement. Chaque gestionnaire nommé dans le XAML va dans MainWindow.xaml.cs, et les chapitres suivants les écrivent.

### Créer le moteur de capture

Dans le gestionnaire Loaded de la fenêtre, créez le moteur VideoCaptureCore et passez-lui le VideoView, où s'affiche l'aperçu en direct.

### Énumérer les caméras

Video_CaptureDevices renvoie toutes les caméras de la machine. Nous ajoutons leurs noms à la liste et sélectionnons la première. Quand la caméra change, son gestionnaire remplit la liste avec les formats de cette caméra. Lisez ici SelectedItem, pas Text : dans SelectionChanged, le Text d'une liste déroulante WPF contient encore l'élément précédent. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images. Vider la liste des formats déclenche SelectionChanged sans aucune sélection, le gestionnaire sort donc tout de suite. Une caméra indique son nom, des formats comme 1920x1080 MJPG ou 1280x720 YUY2, et les fréquences d'images de chaque format. MJPG est compressé dans la caméra, donc les hautes résolutions passent par USB. YUY2 n'est pas compressé, donc USB 2 ne transporte que des résolutions plus basses.

### Choisir le format de la caméra

Sélectionnez la caméra par son nom. Désactivez ensuite Format_UseBest et passez le format et la fréquence d'images choisis dans les listes. Format_UseBest est activé par défaut. Il choisit la plus haute résolution jusqu'à 1080p que propose la caméra et, à résolution égale, préfère MJPG. Pour un format fixe, mettez-le à false et indiquez vous-même le format et la fréquence d'images.

### Sélectionner le micro

Le microphone fonctionne de la même façon. Audio_CaptureDevices les énumère, et nous utilisons celui choisi dans la liste. Audio_PlayAudio à false enregistre le micro sans le restituer.

### MP4HWOutput : tout GPU, repli sur le CPU

Pour la sortie, créez un MP4HWOutput, écrivez output.mp4 dans le dossier Videos et passez le moteur en mode capture vidéo, pour qu'il enregistre au lieu de seulement afficher l'aperçu. Un projet WPF laisse System.IO hors de ses usings implicites : ajoutez-le pour Path. MP4HWOutput encode sur le GPU : NVENC sur NVIDIA, Quick Sync sur Intel et AMF sur AMD, en H.264 ou HEVC. Sans encodeur matériel, l'encodeur Windows sur le CPU prend le relais. Le code et le fichier sont les mêmes sur tous les PC. HWEncodersAvailable indique les encodeurs disponibles sur ce PC. Prenez d'abord NVIDIA, puis Intel, puis AMD, et sinon l'encodeur CPU.

### Régler l'encodeur H.264

Réglez ensuite l'encodeur : six mille kilobits par seconde en moyenne, des pointes jusqu'à neuf mille, le profil Main et l'audio AAC à cent quatre-vingt-douze. À titre indicatif : de 2500 à 4000 kilobits pour le 720p, de 5000 à 8000 pour le 1080p et de 15000 à 25000 pour la 4K. Les deux débits se règlent en kilobits par seconde.

### Ajouter une superposition de texte

Les effets vidéo ajoutent une superposition de texte : un Hello World fixe et une horloge qui affiche l'heure actuelle sur chaque image. La couleur de la police est une couleur System.Drawing, pas une couleur WPF : écrivez-la en entier.

### Démarrer et arrêter

Start et Stop sont asynchrones. StartAsync rend la main une fois la capture lancée, et StopAsync finalise le fichier MP4. Si la fenêtre se ferme pendant un enregistrement, annulez la fermeture, attendez StopAsync avec await, puis refermez. Le fichier est complet, et le moteur est libéré au second passage. Lancez l'application depuis Visual Studio. La liste des formats vient directement de la caméra. Start lance l'enregistrement, avec l'horloge en superposition qui tourne. Stop finalise output.mp4. Le fichier arrive dans Videos : 1920x1080, vidéo H.264 et audio AAC.

### Erreurs fréquentes

Quelques erreurs courantes. Un paquet redist manquant : le moteur ne démarre pas. Une caméra déjà ouverte dans une autre application. Des paquets x86 et x64 mélangés dans un même projet. Et libérer le moteur en pleine capture : attendez d'abord StopAsync. Les erreurs du moteur arrivent dans l'événement OnError.

### Étapes suivantes

Il existe aussi des guides sur les caméras IP, l'enregistrement d'écran, les captures d'images et le streaming réseau. Le code complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Webcam vers MP4 en WinForms](video-capture-webcam-mp4.md) — la même application sous forme de projet Windows Forms
- [Enregistrement pré-événement](../guides/pre-event-recording.md) — enregistrement avec tampon circulaire et déclencheurs de détection de mouvement pour webcams et caméras IP
- [Aperçu vidéo de caméra](camera-video-preview.md) — afficher la vidéo de caméra en direct avec capture d'images

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et d'exemples d'implémentation.
