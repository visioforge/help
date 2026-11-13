---
title: Capture d'écran vers MP4 en C# — tutoriel, GPU, multi-écrans
description: Enregistrez plein écran, région ou multi-écrans en MP4. Encodage GPU (NVENC/QSV/AMF), audio loopback, surbrillance du curseur, exemples C# legacy et modernes.
sidebar_label: Capture d'écran en MP4
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - WPF
  - GStreamer
  - Capture
  - Encoding
  - Webcam
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - H.265
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - MP4Output
  - ScreenCaptureSourceSettings
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCore
  - AudioCaptureSource
  - LoopbackAudioCaptureDeviceSourceSettings
  - VideoView
video:
  id: 17eaRlDqM_I
  upload_date: 2026-11-11T19:35:01Z
  duration: PT8M51S

---

# Capture d'écran vers un fichier MP4

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce tutoriel enregistre l'écran et le son du système dans un fichier MP4 depuis une application WPF. Le moteur `VideoCaptureCoreX` capture l'écran principal via Direct3D 11, affiche un aperçu en direct dans un `VideoView` et encode le H.264 sur le GPU lorsque le PC dispose d'un encodeur matériel, avec un audio AAC. Le son joué sur les haut-parleurs est enregistré via WASAPI loopback.

Le moteur classique `VideoCaptureCore` enregistre aussi l'écran ; sa version de l'application se trouve [plus bas sur cette page](#deuxieme-option-videocapturecore).

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/17eaRlDqM_I?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/screen-capture-mp4-wpf)

## Paquets NuGet requis

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — le SDK ; le `VideoView` WPF y est inclus
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — le runtime GStreamer sur lequel fonctionne le moteur X
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — l'encodeur audio AAC

Ce sont les paquets x64 : le projet définit donc `<PlatformTarget>x64</PlatformTarget>` ; une application 32 bits utilise les paquets `.x86`. Consultez [Déploiement sous Windows](../../deployment-x/Windows.md) pour la liste complète.

## La fenêtre en XAML

Le préfixe `vf` associe l'espace de noms `VisioForge.Core.UI.WPF` de l'assembly `VisioForge.Core`. La colonne de gauche contient la liste des haut-parleurs, la case du son et les boutons ; la colonne de droite, le `VideoView` où l'aperçu en direct est affiché. Start reste désactivé tant que le SDK n'est pas chargé.

```xml
<Window x:Class="ScreenCaptureMp4Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Screen to MP4" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Speakers" />
            <ComboBox x:Name="cbSpeakers" />
            <CheckBox x:Name="cbRecordSound" Content="Record system sound" IsChecked="True"
                      Margin="0,10,0,0" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" IsEnabled="False"
                        Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

## Le code de la fenêtre

```csharp
using System.ComponentModel;
using System.IO;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace ScreenCaptureMp4Wpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private AudioOutputDeviceInfo[] speakers;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Affiche les erreurs du SDK dans la barre de titre
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        // Le son de ces sorties est enregistré via WASAPI loopback
        speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
        foreach (var device in speakers)
            cbSpeakers.Items.Add(device.Name);

        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = 0;

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        // Tout l'écran principal, avec le curseur de la souris
        videoCapture1.Video_Source = new ScreenCaptureD3D11SourceSettings
        {
            FrameRate = new VideoFrameRate(30)
        };

        var recordSound = cbRecordSound.IsChecked == true && cbSpeakers.SelectedIndex >= 0;
        videoCapture1.Audio_Source = recordSound
            ? new LoopbackAudioCaptureDeviceSourceSettings(speakers[cbSpeakers.SelectedIndex])
            : null;
        videoCapture1.Audio_Record = recordSound;
        videoCapture1.Audio_Play = false; // ne jamais rejouer le son capturé dans les haut-parleurs

        // H.264 sur l'encodeur GPU s'il existe, OpenH264 sinon ; audio AAC
        videoCapture1.Outputs_Clear();
        videoCapture1.Outputs_Add(new MP4Output(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"screen_{DateTime.Now:yyyyMMdd_HHmmss}.mp4")));

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
        if (btStart.IsEnabled)
        {
            videoCapture1.Dispose();
            VisioForgeX.DestroySDK();
            return;
        }

        e.Cancel = true; // encore en chargement ou en enregistrement : finaliser d'abord le MP4
        if (!btStop.IsEnabled) return; // déjà en chargement, en démarrage ou en arrêt
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // pas Close() : WPF lève une exception dans Closing
    }
}
```

## Fonctionnement

1. **Initialisation du SDK** — `VisioForgeX.InitSDKAsync` s'exécute une fois, au chargement de la fenêtre. Le premier lancement sur un PC est plus long, c'est pourquoi Start n'est activé qu'une fois l'appel terminé. `VisioForgeX.DestroySDK` le libère à la fermeture de la fenêtre.
2. **Source écran** — `ScreenCaptureD3D11SourceSettings` capture par défaut tout l'écran principal avec le curseur de la souris. Il utilise DXGI Desktop Duplication, qui démarre immédiatement même sur un écran immobile, et se rabat sur Windows Graphics Capture lorsque Desktop Duplication n'est pas pris en charge. La fréquence par défaut est de 10 i/s, c'est pourquoi le code fixe 30.
3. **Son du système** — `AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)` liste les haut-parleurs et les casques, et `LoopbackAudioCaptureDeviceSourceSettings` enregistre ce qui est joué sur celui choisi. `Audio_Play` vaut `false`, le son enregistré n'est donc pas rejoué.
4. **Sortie MP4** — `MP4Output` sans paramètres d'encodeur choisit NVIDIA NVENC, puis AMD AMF, puis Intel Quick Sync, et OpenH264 sur le CPU si aucun n'est présent. L'audio est en AAC. Une sortie reste dans le moteur après un enregistrement, c'est pourquoi `Outputs_Clear` s'exécute avant chaque nouvel enregistrement.
5. **Démarrer, arrêter et fermer** — `StartAsync` indique si la capture a démarré, et `StopAsync` finalise le fichier MP4. Fermer la fenêtre pendant un enregistrement annule la fermeture, attend `StopAsync` et referme la fenêtre.

## Remarques

- Un projet WPF exclut `System.IO` de ses usings implicites, le fichier l'ajoute donc pour `Path`.
- L'aperçu montre l'écran enregistré : la fenêtre de l'application apparaît donc en elle-même, dans l'aperçu et dans le fichier. Sur un PC à deux écrans, déplacez la fenêtre sur celui qui n'est pas enregistré ; avec un seul écran, réduisez-la après Start et restaurez-la pour arrêter.
- Avec **Record system sound** décochée, ou sans haut-parleurs dans la liste, le MP4 ne contient que la vidéo.
- `MP4Output` choisit un encodeur GPU parce que son élément GStreamer existe, ce qui ne prouve pas qu'il puisse encoder à ce moment-là. `H264EncoderBlock.CanEncodeAsync` encode quelques images de test pour le vérifier ; pour rester sur le CPU, passez `new OpenH264EncoderSettings()` au constructeur de `MP4Output`.

## Deuxième option : VideoCaptureCore

Le moteur Windows classique, `VideoCaptureCore`, enregistre l'écran en MP4 dans une application WinForms, avec ou sans microphone.

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

### Redistribuables requis

- Redistribuables de capture vidéo [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuables MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

### Exemple de code

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_mp4
{
    public partial class Form1 : Form
    {
        // Composant VideoCapture principal qui gère toutes les opérations d'enregistrement
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Démarre l'enregistrement d'écran avec l'audio du périphérique par défaut
        /// </summary>
        private async void btStartWithAudio_Click(object sender, EventArgs e)
        {
            // Configurer la capture d'écran pour enregistrer tout l'écran
            // ScreenCaptureSourceSettings permet un contrôle fin de la région de capture et des paramètres
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true  // Capturer tout l'écran plutôt qu'une région spécifique
            };

            // Configurer la capture audio en sélectionnant le premier périphérique d'entrée audio disponible
            // Audio_CaptureDevices() retourne tous les microphones et entrées audio connectés
            // Nous sélectionnons le premier périphérique (index 0) dans la collection
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(
                videoCapture1.Audio_CaptureDevices()[0].Name);

            // Désactiver le monitoring/la lecture audio pendant l'enregistrement pour éviter le retour
            // Cela signifie que nous n'entendrons pas l'audio capturé pendant l'enregistrement
            videoCapture1.Audio_PlayAudio = false;

            // Activer l'enregistrement audio pour inclure le son dans le fichier de sortie
            videoCapture1.Audio_RecordAudio = true;

            // Définir l'emplacement du fichier de sortie vers le dossier Vidéos de l'utilisateur
            // Environment.GetFolderPath garantit que le chemin fonctionne sur différents systèmes Windows
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Utiliser le format conteneur MP4 avec les codecs vidéo H.264 et audio AAC (format standard)
            // MP4Output peut être configuré davantage avec des paramètres d'encodage personnalisés si nécessaire
            videoCapture1.Output_Format = new MP4Output();
            
            // Définir le mode de capture sur enregistrement d'écran
            // Les autres modes incluent la capture caméra, le traitement de fichier vidéo, etc.
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Démarrer le processus de capture de manière asynchrone
            // Utiliser le motif async/await pour éviter le gel de l'interface pendant l'opération
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Démarre l'enregistrement d'écran sans audio (vidéo uniquement)
        /// </summary>
        private async void btStartWithoutAudio_Click(object sender, EventArgs e)
        {
            // Configurer la capture d'écran pour un enregistrement plein écran
            // Mêmes ScreenCaptureSourceSettings que dans l'enregistrement avec audio
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true 
            };

            // Désactiver à la fois la lecture et l'enregistrement audio en une seule ligne
            // Cela crée un fichier MP4 vidéo seule sans piste audio
            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;

            // Définir le chemin du fichier de sortie vers le dossier Vidéos de l'utilisateur avec l'extension MP4
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Configurer la sortie comme MP4 (codec vidéo H.264)
            videoCapture1.Output_Format = new MP4Output();
            
            // Définir le mode sur capture d'écran
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Démarrer l'enregistrement d'écran de manière asynchrone
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Arrête le processus d'enregistrement en cours en toute sécurité
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter l'enregistrement de manière asynchrone
            // Cela finalise correctement le fichier MP4 et libère les ressources
            // Utiliser async garantit que l'interface reste réactive pendant la finalisation du fichier
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Initialise le composant VideoCapture au chargement du formulaire
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser le composant de capture vidéo et le connecter à un contrôle d'aperçu vidéo
            // VideoView1 doit être un contrôle de votre formulaire qui implémente l'interface IVideoView
            // Cela permet l'aperçu en direct de la capture lorsque souhaité
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

### Fonctionnement — VideoCaptureCore

Cette application Windows Forms montre la fonctionnalité de capture d'écran avec et sans audio à l'aide de VisioForge Video Capture SDK :

1. **Configuration** : L'objet `VideoCaptureCore` est initialisé lors de l'événement de chargement du formulaire, en le connectant à un composant de vue vidéo.

2. **Capture avec audio** :
   - Configure la capture d'écran en mode plein écran
   - Sélectionne le premier périphérique audio disponible pour l'enregistrement
   - Désactive la lecture audio mais active l'enregistrement audio
   - Définit le fichier de sortie au format MP4 dans le dossier Vidéos de l'utilisateur
   - Utilise une méthode asynchrone pour démarrer la capture

3. **Capture sans audio** :
   - Similaire à ci-dessus mais désactive à la fois la lecture et l'enregistrement audio
   - Utilise le même format de sortie MP4 et le même mode de capture

4. **Arrêt de la capture** :
   - Fournit une méthode simple d'arrêt qui stoppe l'enregistrement de manière asynchrone

L'application montre comment configurer différents scénarios de capture avec un minimum de code en utilisant l'interface fluide du SDK et les motifs asynchrones.

#### Configuration audio

L'exemple de code ci-dessus capture l'audio du microphone en sélectionnant le premier périphérique disponible. Vous pouvez sélectionner un périphérique audio spécifique par son nom, y compris les périphériques audio système (loopback) pour capturer le son du bureau :

```csharp
// Sélectionner un périphérique loopback (par ex., « Stereo Mix ») pour la capture audio système
var devices = videoCapture1.Audio_CaptureDevices();
var loopbackDevice = devices.FirstOrDefault(d => d.Name.Contains("Stereo Mix"));

if (loopbackDevice != null)
{
    videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(loopbackDevice.Name);
}
else
{
    // Solution de repli : utiliser le premier périphérique audio disponible
    videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(devices[0].Name);
}

// Activer l'enregistrement, désactiver la lecture pour éviter le retour
videoCapture1.Audio_RecordAudio = true;
videoCapture1.Audio_PlayAudio = false;
```

Pour créer un enregistrement silencieux sans piste audio, désactivez les deux propriétés audio :

```csharp
videoCapture1.Audio_PlayAudio = false;
videoCapture1.Audio_RecordAudio = false;
```

#### Capture d'une région

Au lieu d'enregistrer tout l'écran, capturez une zone rectangulaire spécifique en définissant `FullScreen = false` et en fournissant des coordonnées en pixels :

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = false,
    Left = 100,
    Top = 100,
    Right = 1380,
    Bottom = 820
};
```

Les coordonnées définissent le rectangle de capture en pixels d'écran. C'est utile pour enregistrer une fenêtre d'application spécifique ou une portion du bureau.

#### Enregistrement multi-écrans

Sélectionnez l'écran à enregistrer à l'aide de la propriété `DisplayIndex`. L'écran principal porte l'index `0`, le secondaire `1`, et ainsi de suite :

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    DisplayIndex = 1  // Enregistrer l'écran secondaire
};
```

#### Paramètres de qualité d'enregistrement

Personnalisez la sortie MP4 en configurant l'encodeur H.264. Le débit binaire par défaut est de 3500 kbps, ce qui produit environ 25 Mo par minute en 1080p :

```csharp
var mp4Output = new MP4Output();
mp4Output.Video.Bitrate = 5000;                          // Qualité supérieure (kbps)
mp4Output.Video.Profile = H264Profile.ProfileMain;       // Meilleure compression que Baseline
mp4Output.Video.RateControl = H264RateControl.VBR;       // Débit variable
videoCapture1.Output_Format = mp4Output;
```

#### Options du curseur de la souris

Incluez le curseur de la souris dans l'enregistrement et ajoutez éventuellement un effet de surbrillance pour les captures d'écran de type tutoriel :

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    GrabMouseCursor = true,
    MouseHighlight = true,
    MouseHighlightColor = System.Drawing.Color.Yellow,
    MouseHighlightRadius = 40,
    MouseHighlightOpacity = 0.4
};
```

La surbrillance dessine un cercle translucide autour du curseur, ce qui facilite le suivi des mouvements de la souris par les spectateurs.

## Application console — VideoCaptureCoreX { #api-moderne-video-capture-sdk-x }

Le moteur multiplateforme `VideoCaptureCoreX` fonctionne aussi sans fenêtre, avec la capture d'écran Direct3D 11 (Desktop Duplication). Cette application console enregistre tout l'écran en MP4 avec l'audio système optionnel.

Elle utilise les mêmes paquets que l'application WPF : consultez [Paquets NuGet requis](#paquets-nuget-requis).

Les mêmes paquets sont utilisés pour l’essai de 30 jours et les applications sous licence. Consultez la [page de licence VisioForge](https://www.visioforge.com/licensing) pour les conditions d’essai et l’activation avec `.vflicense`.

### Exemple complet

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Configurer la capture d'écran Direct3D 11 (Desktop Duplication par défaut)
            var screenSource = new ScreenCaptureD3D11SourceSettings
            {
                FrameRate = new VideoFrameRate(25, 1),
                CaptureCursor = true,
                MonitorIndex = 0  // Écran principal (-1 sélectionne aussi le principal)
            };

            videoCapture.Video_Source = screenSource;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = false;

            // Configurer la sortie MP4 (H.264 + AAC, encodeurs auto-sélectionnés)
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"screen_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");

            var mp4Output = new MP4Output(outputPath);
            videoCapture.Outputs_Add(mp4Output, autostart: true);

            // Démarrer l'enregistrement
            await videoCapture.StartAsync();
            Console.WriteLine($"Recording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            // Arrêter et enregistrer
            await videoCapture.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Ajout de l'audio système (loopback)

Pour inclure l'audio du bureau dans l'enregistrement, ajoutez la capture loopback WASAPI2 :

```csharp
// Énumérer les périphériques de sortie WASAPI2 pour la capture loopback
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
    AudioOutputDeviceAPI.WASAPI2);

if (audioOutputs.Length > 0)
{
    var loopbackSource = new LoopbackAudioCaptureDeviceSourceSettings(audioOutputs[0]);
    videoCapture.Audio_Source = loopbackSource;
    videoCapture.Audio_Record = true;
}
```

Pour l'audio du microphone à la place, utilisez `DeviceEnumerator.Shared.AudioSourcesAsync()` — consultez le [guide de capture audio](../audio-capture/index.md) pour des exemples complets.

## Encodage accéléré par GPU

L'encodage accéléré par matériel décharge la compression H.264/HEVC sur votre GPU, réduisant significativement l'utilisation du CPU lors d'enregistrements à haute résolution ou à haut taux d'images.

```csharp
// NVIDIA NVENC H.264
var mp4Output = new MP4Output(
    outputPath,
    new NVENCH264EncoderSettings());

// Intel Quick Sync Video H.264
var mp4Output = new MP4Output(
    outputPath,
    new QSVH264EncoderSettings());

// AMD AMF H.264
var mp4Output = new MP4Output(
    outputPath,
    new AMFH264EncoderSettings());
```

Les encodeurs HEVC (H.265) sont également disponibles pour une meilleure compression à qualité égale : `NVENCHEVCEncoderSettings`, `QSVHEVCEncoderSettings`, `AMFHEVCEncoderSettings`.

!!! note "Note sur le moteur : `MP4Output` et le moteur X"

    La classe `MP4Output` présentée dans cette section fait référence au
    `MP4Output` du **moteur X** (`VisioForge.Core.Types.X.Output.MP4Output`),
    qui accepte des types `*EncoderSettings` multiplateformes
    (`NVENCH264EncoderSettings`, `OpenH264EncoderSettings`, etc.).
    Lorsque l'encodage matériel n'est pas disponible sur le moteur X et que
    vous utilisez le constructeur sans paramètres `new MP4Output(filename)`,
    le SDK sélectionne automatiquement un encodeur logiciel disponible. Le
    moteur classique DirectShow réservé à Windows possède son propre
    `VisioForge.Core.Types.Output.MP4Output` avec une structure de paramètres
    différente — ne les mélangez pas.

## Capture d'écran avec le Media Blocks SDK

Le Media Blocks SDK utilise une approche par pipeline où vous connectez des blocs de source, de traitement et de sortie. Cela donne un contrôle total sur le flux de données et permet de diviser le flux vidéo vers plusieurs sorties simultanément.

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

class Program
{
    static async Task Main(string[] args)
    {
        await VisioForgeX.InitSDKAsync();

        var pipeline = new MediaBlocksPipeline();

        try
        {
            // Source de capture d'écran
            var screenSettings = new ScreenCaptureD3D11SourceSettings
            {
                FrameRate = new VideoFrameRate(25, 1),
                CaptureCursor = true
            };
            var screenSource = new ScreenSourceBlock(screenSettings);

            // Sortie MP4
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"screen_mb_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            var mp4Sink = new MP4OutputBlock(outputPath);

            // Connecter la source à la sortie
            pipeline.Connect(screenSource.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

            // Démarrer le pipeline
            await pipeline.StartAsync();
            Console.WriteLine($"Recording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            await pipeline.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            await pipeline.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Capture d'écran multiplateforme

Le SDK prend en charge la capture d'écran sur Windows, macOS et Linux avec des paramètres de source spécifiques à chaque plateforme :

| Plateforme | Méthode de capture | Classe de paramètres | Prérequis |
|----------|---------------|----------------|--------------|
| Windows | Direct3D 11 / WGC | `ScreenCaptureD3D11SourceSettings` | Windows 8+ (WGC : Windows 10 v1803+) |
| macOS | AVFoundation | `ScreenCaptureMacOSSourceSettings` | macOS 10.15+ (autorisation d'enregistrement d'écran) |
| Linux | X11 / XDisplay | `ScreenCaptureXDisplaySourceSettings` | Serveur X11 |

Sous Windows, le SDK utilise DXGI Desktop Duplication par défaut : la capture démarre immédiatement, même sur un écran immobile, et se rabat sur WGC lorsque Desktop Duplication n'est pas pris en charge. La capture de fenêtre utilise toujours WGC. Vous pouvez aussi choisir WGC pour un écran ; lui aussi démarre immédiatement sur un écran immobile :

```csharp
var screenSource = new ScreenCaptureD3D11SourceSettings
{
    API = D3D11ScreenCaptureAPI.WGC  // Windows Graphics Capture au lieu de Desktop Duplication
};
```

Pour Android et iOS, ainsi qu'un enregistreur .NET MAUI qui fonctionne sous Windows, Mac Catalyst, Android et iOS, consultez [Enregistrement d'écran multiplateforme en C# et .NET](https://www.visioforge.com/fr/solutions/screen-recording-csharp).

## Questions fréquentes

### Quelle licence me faut-il pour une application de capture d'écran en C# ?

Video Capture SDK .NET nécessite une licence pour le développement et la distribution. Une licence Développeur supprime le filigrane d'évaluation et débloque toutes les fonctionnalités pendant le développement. Une licence Release est nécessaire lors de la distribution de votre application aux utilisateurs finaux. Le SDK est disponible en édition Premium, qui inclut tous les modes de capture, les sorties MP4/AVI/WMV et les moteurs DirectShow et GStreamer. Vous pouvez évaluer le SDK sans licence — la capture d'écran fonctionne entièrement mais inclut un filigrane superposé. Visitez la [page produit](https://www.visioforge.com/video-capture-sdk-net) pour les tarifs et options de licence.

### Comment contrôler la qualité d'enregistrement et la taille du fichier pour la capture d'écran MP4 ?

Configurez le débit binaire vidéo de `MP4Output` pour équilibrer qualité et taille de fichier. La valeur par défaut est de 3500 kbps, qui fonctionne bien pour la plupart des enregistrements d'écran. Baissez le débit à 1500–2000 kbps pour des fichiers plus petits, ou augmentez à 5000–8000 kbps pour des captures de haute qualité. Utilisez `H264Profile.ProfileMain` ou `ProfileHigh` au lieu de `ProfileBaseline` pour une meilleure compression à qualité égale. La fréquence d'images influence également la taille du fichier — 15 FPS suffisent pour les présentations, tandis que 30 FPS convient mieux aux démos logicielles. Un enregistrement 1080p à 3500 kbps produit environ 25 Mo par minute.

### Puis-je capturer l'audio système (son du bureau) au lieu de l'entrée microphone ?

Oui. Avec `VideoCaptureCoreX`, passez un haut-parleur de `DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)` à `LoopbackAudioCaptureDeviceSourceSettings`, comme le fait l'application WPF ci-dessus : il enregistre ce qui est joué sur ce périphérique, sans pilote particulier. Avec `VideoCaptureCore`, appelez `Audio_CaptureDevices()` pour énumérer tous les périphériques audio disponibles, puis sélectionnez un périphérique loopback ou audio système (comme « Stereo Mix » ou « What U Hear ») par son nom. Définissez `Audio_RecordAudio = true` et `Audio_PlayAudio = false`. Les noms des périphériques loopback disponibles dépendent de votre matériel audio et de vos pilotes. Vous pouvez enregistrer simultanément le microphone et l'audio système en configurant des sources audio supplémentaires.

### Comment enregistrer un écran spécifique dans une configuration multi-écrans ?

Avec `VideoCaptureCoreX`, définissez `MonitorIndex` sur `ScreenCaptureD3D11SourceSettings` (`0` est le premier écran, `-1` l'écran principal) et donnez à `Rectangle` la taille de cet écran. Avec `VideoCaptureCore`, définissez la propriété `DisplayIndex` sur `ScreenCaptureSourceSettings` — utilisez `0` pour l'écran principal, `1` pour le secondaire, et ainsi de suite. Combinez avec `FullScreen = true` pour capturer tout l'écran sélectionné. Vous pouvez également définir `FullScreen = false` avec les coordonnées `Left/Top/Right/Bottom` pour capturer une région spécifique sur l'écran choisi.

### Quelle fréquence d'images dois-je utiliser pour l'enregistrement d'écran ?

La fréquence d'images par défaut est de 10 FPS. Utilisez 15 FPS pour les présentations, les diapositives et le contenu majoritairement statique. Utilisez 25–30 FPS pour les tutoriels logiciels et les démonstrations d'interface où le mouvement fluide de la souris importe. Utilisez 60 FPS pour l'enregistrement de jeux ou le contenu très animé. Des fréquences d'images plus élevées augmentent proportionnellement la taille du fichier et l'utilisation du CPU. Définissez la fréquence d'images via `ScreenCaptureD3D11SourceSettings.FrameRate` avec `VideoCaptureCoreX`, ou `ScreenCaptureSourceSettings.FrameRate` avec `VideoCaptureCore`.

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons l'écran et le son du système dans un fichier MP4 depuis une application C# WPF avec le VisioForge Video Capture SDK. Voici ce que nous construisons. L'application enregistre tout l'écran principal, avec le curseur de la souris, et le son joué sur les haut-parleurs. Elle écrit du MP4, avec du H.264 encodé sur le GPU lorsque le PC dispose d'un encodeur matériel, et un audio AAC. Un VideoView affiche un aperçu en direct, et la fenêtre est décrite en XAML. Le code de la fenêtre tient en une quatre-vingt-dizaine de lignes de C#. Voici l'application terminée, en train d'enregistrer. L'aperçu montre l'écran, et l'écran contient l'aperçu : l'image se répète comme entre deux miroirs face à face.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet cible .NET 8 ou une version ultérieure. Le guide écrit présente aussi l'enregistrement d'écran avec le moteur classique VideoCaptureCore.

### Créer un projet WPF

Dans Visual Studio, créez un nouveau projet. Recherchez WPF et choisissez WPF Application pour C#. Nommez le projet ScreenCaptureMp4Wpf. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre MainWindow.xaml, avec le concepteur en haut et le XAML en bas.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet. Le Video Capture SDK lui-même. CrossPlatform Core, le runtime GStreamer sur lequel fonctionne le moteur X. Et CrossPlatform Libav, qui contient l'encodeur AAC. Ou utilisez le NuGet Package Manager : clic droit sur le projet, Manage NuGet Packages, puis installez VisioForge.DotNet.VideoCapture depuis l'onglet Browse. Installez ensuite les paquets Core et Libav pour Windows x64. L'onglet Installed affiche les trois. Ce sont les paquets x64 : définissez donc PlatformTarget sur x64 dans le fichier de projet. Une application 32 bits utilise plutôt les paquets x86.

### Construire la fenêtre en XAML

La fenêtre s'écrit en XAML. D'abord la fenêtre elle-même, avec une grille à deux colonnes et un VideoView, et le concepteur la dessine aussitôt. Associez l'espace de noms VisioForge.Core.UI.WPF à un préfixe, ici vf. Le VideoView va dans la colonne de droite : c'est là que l'aperçu en direct est dessiné. La fenêtre nomme aussi ses gestionnaires Loaded et Closing. Ensuite, la liste des haut-parleurs et une case à cocher pour le son. La colonne de gauche est un StackPanel avec une étiquette, une liste déroulante pour les haut-parleurs et une case qui active ou désactive le son. Elle est cochée au départ. Enfin, les boutons Start et Stop. Chaque bouton nomme un gestionnaire Click. Les deux sont désactivés au départ : Start jusqu'au chargement du SDK, et Stop tant qu'aucun enregistrement n'est en cours. Chaque gestionnaire nommé par le XAML va dans MainWindow.xaml.cs, et les chapitres suivants les écrivent.

### Initialiser le SDK

MainWindow.xaml.cs commence par ces usings. Un projet WPF exclut System.IO de ses usings implicites : ajoutez-le pour Path. Au chargement de la fenêtre, attendez VisioForgeX.InitSDKAsync une seule fois. Créez ensuite le moteur VideoCaptureCoreX et passez-lui le VideoView, où l'aperçu est dessiné. Les erreurs du moteur arrivent dans l'événement OnError ; ici, elles vont dans la barre de titre.

### Lister les haut-parleurs

AudioOutputsAsync avec WASAPI2 liste les haut-parleurs et les casques. Leurs noms remplissent la liste, et le premier est sélectionné. Start n'est activé que maintenant, une fois le SDK chargé. WASAPI loopback enregistre tout ce qui est joué sur un périphérique de sortie. Il ne faut ni Stereo Mix ni câble virtuel. Choisissez le périphérique sur lequel le son est réellement joué, car loopback n'enregistre que celui-là.

### Capturer l'écran

Dans le gestionnaire de Start, définissez la source vidéo sur ScreenCaptureD3D11SourceSettings. Par défaut, elle capture tout l'écran principal avec le curseur de la souris. La fréquence par défaut est de dix images par seconde, réglez-la donc à trente. La source utilise Windows Graphics Capture sous Windows 10 version 1803 et ultérieures, et DXGI Desktop Duplication sur les systèmes plus anciens. MonitorIndex choisit un autre écran, et CaptureCursor à false laisse le curseur de côté.

### Enregistrer le son du système

Lorsque la case est cochée et qu'un périphérique est sélectionné, la source audio est LoopbackAudioCaptureDeviceSourceSettings pour ce périphérique, et Audio_Record est activé. Sinon, il n'y a pas de source audio, et le MP4 ne contient que la vidéo. Audio_Play vaut false : le son enregistré n'est donc pas rejoué dans les haut-parleurs.

### Écrire le MP4

Ajoutez un MP4Output qui écrit dans le dossier Vidéos, avec la date et l'heure dans le nom du fichier, pour qu'un deuxième enregistrement n'écrase jamais le premier. Une sortie reste dans le moteur après un enregistrement, c'est pourquoi Outputs_Clear s'exécute avant chaque nouvel enregistrement. Sans paramètres d'encodeur, MP4Output choisit NVIDIA NVENC, puis AMD AMF, puis Intel Quick Sync, et OpenH264 sur le CPU si le PC n'en a aucun. L'audio est en AAC. H264EncoderBlock.CanEncodeAsync encode quelques images de test lorsque vous devez savoir si un encodeur GPU fonctionne vraiment.

### Démarrer et arrêter

StartAsync indique si la capture a démarré, et les boutons suivent. StopAsync finalise le fichier MP4. Si la fenêtre se ferme pendant un enregistrement, annulez la fermeture, attendez StopAsync et refermez. Lors de ce second passage, le moteur et le SDK sont libérés : le fichier est toujours complet.

### Lancer l'application

Lancez l'application depuis Visual Studio. Une fois le SDK chargé, la liste des haut-parleurs se remplit et Start est activé. Cliquez sur Start. L'aperçu montre l'écran avec l'application à l'intérieur, encore et encore. Le son d'alarme joué pendant ce temps entre dans le fichier par loopback. Stop finalise le MP4. Fermez ensuite la fenêtre. Le fichier arrive dans Vidéos : en 4K à trente images par seconde, avec le son du système.

### Erreurs fréquentes

Quelques erreurs fréquentes. Oublier le paquet Libav, qui contient l'encodeur AAC. Mélanger des paquets x86 et x64, ou lancer une application x86 avec les paquets x64. Oublier que chaque fenêtre de l'écran enregistré finit dans la vidéo, y compris celle de l'application : sur un PC à deux écrans, déplacez-la sur celui qui n'est pas enregistré. Et le loopback sur un périphérique où le son n'est pas joué. Les erreurs du moteur arrivent dans l'événement OnError.

### Étapes suivantes

Des guides couvrent aussi l'enregistrement d'une région ou d'une seule fenêtre, l'ajout d'un microphone, l'enregistrement d'une webcam et la diffusion réseau. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci de votre attention.

## Voir aussi

- [Capture d'écran en AVI](screen-capture-avi.md) — enregistrer l'écran au format AVI avec vidéo non compressée ou MJPEG
- [Capture d'écran en WMV](screen-capture-wmv.md) — enregistrer l'écran au format Windows Media
- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — application d'enregistrement d'écran en Visual Basic .NET
- [Configuration de source écran](../video-sources/screen.md) — référence complète pour la région de capture, le multi-écrans, le curseur et la fréquence d'images
- [Enregistrer la vidéo de webcam](../guides/save-webcam-video.md) — capture de webcam en MP4 avec configuration audio
- [Exemples de code](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — extraits de code supplémentaires de capture d'écran sur GitHub
- [Enregistrement d'écran multiplateforme en C#](https://www.visioforge.com/fr/solutions/screen-recording-csharp) — API de capture par plateforme, un enregistreur .NET MAUI et la version d'essai
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
