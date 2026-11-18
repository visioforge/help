---
title: Audio bidirectionnel caméra IP en C# WPF — Exemple
description: "Parlez via le haut-parleur d'une caméra IP depuis une app C# WPF : aperçu RTSP avec le son de la caméra et le micro du PC via le backchannel ONVIF."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WPF
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - Audio
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - AudioRendererSettings
  - DeviceEnumerator
  - VisioForgeX

---

# Audio bidirectionnel avec une caméra IP en C# WPF

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation

Ce tutoriel construit un petit interphone dans une application C# WPF avec le moteur multiplateforme `VideoCaptureCoreX` :

- l'URL RTSP de la caméra, son identifiant et son mot de passe sont saisis dans la fenêtre ;
- un `VideoView` affiche la caméra en direct, et le micro de la caméra est joué sur les haut-parleurs du PC choisis dans la fenêtre ;
- le micro du PC choisi dans la fenêtre est joué sur le haut-parleur de la caméra, envoyé via le backchannel audio RTSP d'ONVIF ;
- Start et Stop ouvrent et ferment les deux sens à la fois.

Cela convient aux portiers vidéo, aux postes d'interphone et aux caméras dotées d'un haut-parleur ou d'une sortie audio. Le fonctionnement du backchannel, la version Media Blocks et le volume du haut-parleur via ONVIF sont décrits sur la page [audio bidirectionnel](../video-sources/ip-cameras/two-way-audio.md).

!!! note "Version du SDK"
    `Audio_BackchannelSource` figure dans les versions du SDK postérieures à 2026.11.16.

## Dépôt du code source

Le projet complet est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-two-way-audio-wpf)

## Paquets nécessaires

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — le SDK ; le `VideoView` WPF en fait partie
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — le runtime GStreamer sur lequel fonctionne le moteur X, avec la source RTSP, les décodeurs et l'encodeur G.711 qu'utilise le backchannel
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — le décodeur AAC, pour les caméras qui envoient leur son en AAC

Ce sont les paquets x64, le projet indique donc `<PlatformTarget>x64</PlatformTarget>` ; une application 32 bits prend les paquets `.x86`. La liste complète se trouve dans [Déploiement sous Windows](../../deployment-x/Windows.md).

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

## Exemple d'implémentation en C#

### La fenêtre (MainWindow.xaml)

Les réglages sont à gauche, la caméra à droite. `PasswordBox` affiche des points. Start reste désactivé tant que le SDK n'est pas chargé, et Stop tant que la caméra n'a pas démarré.

```xml
<Window x:Class="IpCameraTwoWayAudioWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="IP Camera Two-Way Audio" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera URL" />
            <TextBox x:Name="edURL" />
            <Label Content="Login" Margin="0,10,0,0" />
            <TextBox x:Name="edLogin" />
            <Label Content="Password" Margin="0,10,0,0" />
            <PasswordBox x:Name="edPassword" />
            <Label Content="Microphone (to the camera speaker)" Margin="0,10,0,0" />
            <ComboBox x:Name="cbMicrophone" />
            <Label Content="Speakers (the camera's sound)" Margin="0,10,0,0" />
            <ComboBox x:Name="cbSpeakers" />

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

### Le code (MainWindow.xaml.cs)

```csharp
using System.ComponentModel;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace IpCameraTwoWayAudioWpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private AudioCaptureDeviceInfo[] microphones;

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

        microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
        foreach (var device in microphones)
            cbMicrophone.Items.Add(device.DisplayName);

        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
        foreach (var device in speakers)
            cbSpeakers.Items.Add(device.DisplayName);

        // Présélectionne la sortie par défaut du système
        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = Math.Max(0, Array.FindIndex(speakers, device => device.IsDefault));

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Title = "Enter the camera URL";
            return;
        }

        if (cbMicrophone.SelectedIndex < 0)
        {
            Title = "No microphone";
            return;
        }

        Title = "Connecting...";
        btStart.IsEnabled = false;

        var started = false;
        try
        {
            // La vidéo de la caméra et son propre micro, lus sur les haut-parleurs choisis
            videoCapture1.Video_Source = await RTSPSourceSettings.CreateAsync(url, edLogin.Text, edPassword.Password, audioEnabled: true);
            videoCapture1.Audio_OutputDevice = cbSpeakers.SelectedIndex < 0 ? null : new AudioRendererSettings(speakers[cbSpeakers.SelectedIndex]);
            videoCapture1.Audio_Play = true;

            // Le micro du PC, envoyé au haut-parleur de la caméra via le backchannel audio ONVIF
            videoCapture1.Audio_BackchannelSource = microphones[cbMicrophone.SelectedIndex].CreateSourceSettingsVC();

            started = await videoCapture1.StartAsync();
        }
        catch (Exception ex)
        {
            Title = ex.Message;
        }

        // Une erreur du SDK publiée pendant le démarrage reste dans le titre
        if (Title == "Connecting...") Title = started ? "Talking to the camera" : "The camera did not start";
        btStop.IsEnabled = started;
        btStart.IsEnabled = !started;
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        Title = "IP Camera Two-Way Audio";
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

        e.Cancel = true; // chargement ou conversation en cours : arrêter d'abord
        if (!btStop.IsEnabled) return; // déjà en chargement, en démarrage ou en arrêt
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // pas Close() : WPF lève une exception dans Closing
    }
}
```

## Points clés de l'implémentation

1. **Initialisation du SDK** - `VisioForgeX.InitSDKAsync` s'exécute une fois, au chargement de la fenêtre. Le premier lancement sur un PC est plus long, c'est pourquoi Start n'est activé qu'après son retour. `VisioForgeX.DestroySDK` le libère à la fermeture de la fenêtre
2. **La caméra** - `RTSPSourceSettings.CreateAsync` reçoit l'URL, l'identifiant et le mot de passe comme arguments séparés, l'URL ne les contient donc pas. Avec `audioEnabled: true`, le son de la caméra est utilisé quand son flux en a un, et `Audio_Play` le joue sur les haut-parleurs indiqués dans `Audio_OutputDevice`, puisque `Audio_Source` reste vide. Sans `Audio_OutputDevice`, le son n'est pas joué. L'[annuaire des marques de caméras IP](../../camera-brands/index.md) donne les chemins RTSP de nombreuses caméras
3. **Le backchannel** - `Audio_BackchannelSource` accepte les mêmes sources audio que `Audio_Source` ; `CreateSourceSettingsVC` transforme le micro choisi en l'une d'elles. Au démarrage, le SDK demande à la caméra son backchannel audio ONVIF et lui envoie le micro en G.711 mu-law à 8000 Hz, le codec qu'ONVIF impose à toute caméra dotée d'une sortie audio. Le son quitte le PC tel qu'il est capturé, sans y ajouter le tampon de réception RTSP
4. **Le démarrage** - une caméra qui ne répond pas met du temps à échouer, c'est pourquoi le titre affiche « Connecting... » et Start reste désactivé jusqu'au retour de `StartAsync`. Un message `OnError` publié pendant le démarrage reste dans le titre
5. **Arrêt et fermeture** - `StopAsync` ferme les deux sens. Fermer la fenêtre pendant le fonctionnement annule la fermeture, attend `StopAsync`, puis referme ; pendant le chargement du SDK, ou un démarrage ou un arrêt en cours, la fermeture est annulée : refermez la fenêtre une fois l'opération terminée

## Dépannage

- **La caméra refuse la connexion** - le chemin ne correspond pas à la marque de la caméra, ou l'identifiant ou le mot de passe est incorrect. Plusieurs mots de passe erronés d'affilée font que beaucoup de caméras verrouillent le compte pendant un moment
- **Aucune image, avec la bonne URL et le bon mot de passe** - demander le backchannel ajoute l'en-tête ONVIF `Require: www.onvif.org/ver20/backchannel` à la requête DESCRIBE et au SETUP du backchannel, et une caméra sans backchannel peut refuser l'une ou l'autre (elle répond 551 Option Not Supported) : une erreur remplace alors le titre, même si le démarrage a déjà indiqué un succès. Laissez `Audio_BackchannelSource` vide et redémarrez : si l'image arrive, la caméra n'a pas de backchannel
- **L'image arrive, mais le haut-parleur de la caméra reste muet** - la caméra n'offre pas de backchannel, ou pas en G.711 mu-law : le SDK indique dans le titre de la fenêtre les codecs que propose la caméra. Vérifiez que l'audio bidirectionnel est activé dans l'interface web de la caméra, qu'aucune application du fabricant ni aucun NVR n'occupe son canal de conversation, et augmentez le volume du haut-parleur (voir [Volume du haut-parleur via ONVIF](../video-sources/ip-cameras/two-way-audio.md#volume-du-haut-parleur-via-onvif))
- **Vous ne vous entendez pas dans le micro de la caméra** - c'est normal. Les caméras dotées d'un haut-parleur l'annulent dans leur micro ; vérifiez donc le haut-parleur à l'oreille, près de la caméra
- **La personne près de la caméra entend un écho** - le PC joue le son de la caméra sur ses haut-parleurs pendant que son micro envoie vers la caméra : le micro du PC capte de nouveau le son de la caméra et le lui renvoie. Utilisez un casque avec micro sur le PC, ou mettez `Audio_Play` à `false`

## Guides associés

- [Audio bidirectionnel avec les caméras IP](../video-sources/ip-cameras/two-way-audio.md) — fonctionnement du backchannel, Media Blocks, volume du haut-parleur via ONVIF
- [Enregistrer une caméra IP en MP4](ip-camera-capture-mp4.md) — la même connexion à la caméra, enregistrée dans un fichier
- [Configuration de la source RTSP](../video-sources/ip-cameras/rtsp.md) — transport UDP/TCP, faible latence
- [Intégration des caméras IP ONVIF](../video-sources/ip-cameras/onvif.md) — découverte et choix du profil

---
Consultez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour plus d'exemples de code. Besoin de l'URL RTSP de votre caméra ? Consultez notre [annuaire des marques de caméras IP](../../camera-brands/index.md).
