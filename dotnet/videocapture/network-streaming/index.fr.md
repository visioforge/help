---
title: Streaming réseau en C# .NET — RTSP, RTMP, NDI, SRT
description: Diffusez de la vidéo en direct via RTSP, RTMP, NDI, HLS et SRT depuis VisioForge Video Capture. Protocoles, encodeurs et exemples C#.
sidebar_label: Streaming réseau
order: 5
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
video:
  id: _mkmAxqQesw
  upload_date: 2026-11-17T11:35:00Z
  duration: PT10M8S

---

# Streaming réseau dans les applications .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au streaming réseau

Le streaming réseau est devenu un composant fondamental de l'infrastructure moderne de communication numérique, permettant la transmission en temps réel de données audio et vidéo sur des environnements réseau variés. Avec l'expansion continue des capacités de bande passante et l'évolution des attentes des utilisateurs en matière de diffusion de contenu, les développeurs ont besoin d'outils robustes pour implémenter des solutions de streaming qui équilibrent performance, qualité et fiabilité.

Le VisioForge Video Capture SDK pour .NET offre une prise en charge complète de plusieurs protocoles de streaming, offrant aux développeurs un kit d'outils polyvalent pour intégrer des capacités de streaming sophistiquées dans leurs applications. Que vous construisiez des plateformes de diffusion en direct, des outils de visioconférence, des systèmes de surveillance ou des réseaux de diffusion de contenu, ce SDK fournit la base pour implémenter des solutions de streaming professionnelles.

## Tutoriel vidéo — diffuser une webcam en RTMP en C# WPF

Ce tutoriel construit une application WPF sur le moteur `VideoCaptureCoreX` qui affiche l'aperçu en direct d'une webcam et, en même temps, diffuse la caméra et le microphone en RTMP :

- la vidéo est en H.264, encodée sur le GPU quand le PC a un encodeur matériel (NVIDIA NVENC, AMD AMF, Intel Quick Sync), et avec OpenH264 sur le CPU sinon ; l'audio est en AAC ;
- le flux part vers n'importe quel serveur RTMP : MediaMTX, YouTube Live ou le point d'ingestion d'un CDN ;
- dans la vidéo, il part vers un serveur MediaMTX du réseau local, et un navigateur le relit depuis ce serveur.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/_mkmAxqQesw?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### Code source

Le projet complet est sur GitHub : [network-streaming-rtmp-wpf](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/network-streaming-rtmp-wpf).

### Paquets requis

Le SDK, CrossPlatform Core (le runtime GStreamer sur lequel tourne le moteur X, avec le sink RTMP et les encodeurs H.264) et CrossPlatform Libav (l'encodeur AAC) :

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

Ce sont les paquets x64 : réglez donc `PlatformTarget` sur `x64` dans le fichier projet. Une application 32 bits prend les paquets x86.

### La fenêtre en XAML

Une grille à deux colonnes : à gauche les listes des caméras et des microphones, l'URL RTMP et les boutons Start et Stop ; à droite le `VideoView` qui dessine l'aperçu. Les deux boutons démarrent désactivés : Start jusqu'au chargement du SDK, Stop jusqu'à ce qu'un flux soit en cours.

```xml
<Window x:Class="WebcamRtmpWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Webcam to RTMP" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera" />
            <ComboBox x:Name="cbCamera" />
            <Label Content="Microphone" Margin="0,10,0,0" />
            <ComboBox x:Name="cbMicrophone" />
            <Label Content="RTMP URL" Margin="0,10,0,0" />
            <TextBox x:Name="edURL" Text="rtmp://localhost:1935/webcam" />

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

### Le code

```csharp
using System.ComponentModel;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace WebcamRtmpWpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private VideoCaptureDeviceInfo[] cameras;

    private AudioCaptureDeviceInfo[] microphones;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        foreach (var device in cameras)
            cbCamera.Items.Add(device.DisplayName);

        microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
        foreach (var device in microphones)
            cbMicrophone.Items.Add(device.DisplayName);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        if (cbCamera.SelectedIndex < 0)
        {
            Title = "No camera";
            return;
        }

        if (string.IsNullOrWhiteSpace(edURL.Text))
        {
            Title = "Enter the RTMP URL";
            return;
        }

        Title = "Webcam to RTMP";

        // 1080p at 25 fps or more when the camera has it, otherwise 720p, otherwise another format
        videoCapture1.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[cbCamera.SelectedIndex]);

        var hasMic = cbMicrophone.SelectedIndex >= 0;
        videoCapture1.Audio_Source = hasMic ? microphones[cbMicrophone.SelectedIndex].CreateSourceSettingsVC() : null;
        videoCapture1.Audio_Record = hasMic; // no microphone: stream video only
        videoCapture1.Audio_Play = false; // stream the microphone without playing it back

        // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
        videoCapture1.Outputs_Clear();
        videoCapture1.Outputs_Add(new RTMPOutput(edURL.Text.Trim()));

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

        e.Cancel = true; // still loading or streaming: stop the stream first
        if (!btStop.IsEnabled) return; // already loading, starting or stopping
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // not Close(): WPF throws inside Closing
    }
}
```

- `VideoCaptureDeviceSourceSettings` prend du 1080p à 25 images par seconde ou plus quand la caméra le permet, sinon du 720p, sinon un autre format.
- Une URL vide est arrêtée dans le gestionnaire Start : le sink RTMP reviendrait à son propre serveur par défaut au lieu d'échouer.
- Une sortie reste dans le moteur après un flux : `Outputs_Clear` s'exécute donc avant chaque nouveau flux.
- Les propriétés `Video` et `Audio` de `RTMPOutput` acceptent d'autres réglages d'encodeur, par exemple un débit fixe.
- Les erreurs du moteur arrivent dans l'événement `OnError` ; l'application les affiche dans la barre de titre, et c'est là qu'apparaît un serveur injoignable.

### L'URL RTMP

- **MediaMTX** : `rtmp://<serveur>:1935/<nom>`. Le chemin après le port est le nom du flux, et ce même chemin se relit en WebRTC (`http://<serveur>:8889/<nom>`, le lecteur montré dans la vidéo), HLS et RTSP.
- **YouTube Live** : l'adresse d'ingestion suivie de votre clé de flux.
- **rtmps://** se connecte en TLS.

D'autres destinations prennent d'autres sorties, et le reste du code ne change pas : `SRTOutput` prend une URL comme `RTMPOutput`, `YouTubeOutput` et `FacebookLiveOutput` prennent la clé de flux, et `RTSPServerOutput` et `HLSOutput` prennent un objet de réglages (`RTSPServerSettings`, `HLSSinkSettings`).

## Démarrage rapide — streaming RTMP avec VideoCaptureCoreX

Le même flux sans fenêtre : sans `VideoView`, il n'y a pas d'aperçu, et le moteur est créé sans lui. Les appels au moteur sont les mêmes sur toutes les plateformes où tourne le moteur X ; les paquets changent selon la plateforme, et sous Android et iOS l'application demande l'autorisation d'accès à la caméra et au microphone avant de lister les périphériques.

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// Initialisez le SDK une fois par processus.
await VisioForgeX.InitSDKAsync();

// Sans VideoView : pas d'aperçu. Les erreurs arrivent dans OnError.
var videoCapture = new VideoCaptureCoreX();
videoCapture.OnError += (_, e) => Console.WriteLine(e.Message);

// La première caméra et le premier microphone ; vidéo seule s'il n'y a pas de microphone.
var camera = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault();
var microphone = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault();
if (camera == null) return;

videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(camera);
videoCapture.Audio_Source = microphone?.CreateSourceSettingsVC();
videoCapture.Audio_Record = microphone != null;
videoCapture.Audio_Play = false; // ne pas rejouer le microphone

// L'URL d'ingestion complète, avec la clé de flux.
videoCapture.Outputs_Add(new RTMPOutput("rtmp://live.example.com/app/<stream-key>"));
await videoCapture.StartAsync();

// ... et quand le flux doit se terminer :
await videoCapture.StopAsync();
videoCapture.Dispose();
VisioForgeX.DestroySDK();
```

Pour la configuration par protocole (réglage de l'encodeur, tailles de tampon, authentification, débits adaptatifs, etc.), consultez les pages dédiées sous [Protocoles disponibles](#available-protocols) ci-dessous.

## Protocoles principaux de streaming réseau

### RTSP (Real-Time Streaming Protocol)

RTSP est un protocole de niveau application conçu pour contrôler la diffusion de données possédant des propriétés temps réel. Il sert de « télécommande réseau » pour les serveurs multimédias, établissant et contrôlant les sessions multimédias entre points d'extrémité.

#### Fonctionnalités clés de RTSP :

- **Contrôle de session** : permet aux clients d'établir et de gérer des sessions multimédias avec les serveurs
- **Indépendance du transport** : fonctionne avec divers protocoles de transport notamment UDP, TCP et UDP multicast
- **Prise en charge des commandes** : implémente des commandes comme PLAY, PAUSE et RECORD pour un contrôle granulaire de la session
- **Évolutivité** : prend en charge la diffusion unicast et multicast pour une utilisation efficace de la bande passante

### RTMP (Real-Time Messaging Protocol)

Développé à l'origine par Macromedia pour diffuser du contenu Flash, RTMP a évolué pour devenir l'un des protocoles les plus largement utilisés pour la diffusion en direct. Il maintient des connexions persistantes entre le client et le serveur, facilitant une transmission à faible latence cruciale pour les applications interactives.

#### Fonctionnalités clés de RTMP :

- **Faible latence** : offre généralement une latence inférieure à la seconde, le rendant adapté au streaming interactif
- **Distribution fiable** : utilise TCP comme couche de transport pour une distribution fiable des paquets
- **Protection du contenu** : prend en charge le chiffrement pour une diffusion sécurisée du contenu
- **Large prise en charge** : compatible avec de nombreux CDN et plateformes de streaming

### NDI (Network Device Interface)

NDI représente une avancée significative dans les flux de travail de production vidéo professionnels, permettant une transmission vidéo haute qualité et faible latence sur des réseaux IP standards. Développé par NewTek, NDI a été largement adopté dans les environnements de diffusion et de production.

#### Fonctionnalités clés de NDI :

- **Flux de travail basé sur IP** : tire parti de l'infrastructure réseau existante sans matériel spécialisé
- **Communication bidirectionnelle** : prend en charge les métadonnées et les données de contrôle aux côtés de l'audio/vidéo
- **Mécanisme de découverte** : découverte automatique des périphériques et des sources sur les réseaux locaux
- **Encodage haute qualité** : maintient la qualité visuelle tout en optimisant pour les conditions du réseau

### HLS (HTTP Live Streaming)

Développé par Apple, HLS est devenu l'un des protocoles de streaming les plus largement pris en charge. Il segmente le contenu en petits téléchargements de fichiers HTTP, permettant un streaming à débit adaptatif qui ajuste la qualité en fonction de la bande passante disponible du spectateur.

#### Fonctionnalités clés de HLS :

- **Débit adaptatif** : ajuste dynamiquement la qualité du flux selon les conditions du réseau
- **Large compatibilité** : pris en charge sur la plupart des navigateurs et appareils modernes
- **Distribution HTTP** : utilise des serveurs web standard et des CDN pour une distribution efficace du contenu
- **Protection du contenu** : prend en charge le chiffrement et l'intégration DRM

### SRT (Secure Reliable Transport)

SRT est un protocole open source optimisé pour offrir une vidéo haute qualité et faible latence sur des réseaux imprévisibles. Il combine la fiabilité de TCP avec la vitesse d'UDP tout en ajoutant des fonctionnalités de sécurité.

#### Fonctionnalités clés de SRT :

- **Récupération de perte de paquets** : implémente des mécanismes de retransmission dynamique
- **Chiffrement** : chiffrement AES intégré pour une transmission sécurisée
- **Surveillance de la santé du réseau** : évalue en continu la qualité de la connexion
- **Contrôle de la latence** : paramètres de latence configurables pour équilibrer fiabilité et délai

## Protocoles supplémentaires pris en charge

### HTTP MJPEG (Motion JPEG)

MJPEG sur HTTP transmet une séquence d'images JPEG, fournissant une solution de streaming simple mais efficace. Bien que moins efficace en bande passante que les codecs modernes, sa simplicité et sa compatibilité le rendent adapté à certaines applications.

### UDP (User Datagram Protocol)

Le streaming UDP privilégie la vitesse à la fiabilité, le rendant idéal pour les applications temps réel où une perte occasionnelle de paquets est préférable à une latence accrue. Le SDK fournit des paramètres de tampon configurables pour aider à optimiser le streaming UDP selon les conditions réseau spécifiques.

### WMV (Windows Media Video)

Le SDK prend en charge le streaming au format WMV de Microsoft, qui reste pertinent pour certaines applications centrées sur Windows et l'intégration de systèmes hérités.

### Streaming spécifique à la plateforme

Le SDK s'intègre également avec des plateformes de streaming populaires, notamment :

- **YouTube Live** : streaming direct vers les chaînes YouTube
- **Facebook Live** : diffusion Facebook Live intégrée
- **AWS S3** : distribution multimédia basée sur le cloud via Amazon Web Services

## Considérations d'implémentation

Lors de l'implémentation du streaming réseau avec le SDK, les développeurs doivent considérer :

1. **Besoins en bande passante** : estimez et testez la bande passante requise pour votre qualité et fréquence d'images cibles
2. **Résilience réseau** : implémentez une gestion d'erreurs et une logique de reconnexion appropriées
3. **Qualité vs latence** : équilibrez la qualité visuelle face aux exigences de latence
4. **Compatibilité multiplateforme** : sélectionnez les protocoles selon vos plateformes cibles
5. **Besoins de sécurité** : implémentez le chiffrement et l'authentification où nécessaire

## Conclusion

Le VisioForge Video Capture SDK pour .NET offre une prise en charge complète des protocoles de streaming contemporains, permettant aux développeurs d'implémenter des solutions de streaming sophistiquées sans gérer directement la complexité des implémentations de protocoles. Des diffusions en direct à faible latence à la livraison sécurisée de contenu, les capacités de streaming du SDK répondent à des cas d'usage variés dans toutes les industries.

En tirant parti de ces capacités, les développeurs peuvent se concentrer sur la création d'expériences convaincantes tout en s'appuyant sur le SDK pour gérer les défis techniques d'un streaming réseau fiable. Que vous développiez des applications pour le divertissement, l'éducation, la sécurité ou la production vidéo professionnelle, le SDK fournit une base solide pour vos besoins de streaming.

## Protocoles disponibles { #available-protocols }

* [Adobe Flash Server](../../general/network-streaming/adobe-flash.md)
* [AWS S3](../../general/network-streaming/aws-s3.md)
* [Facebook](../../general/network-streaming/facebook.md)
* [HLS](../../general/network-streaming/hls-streaming.md)
* [HTTP MJPEG](../../general/network-streaming/http-mjpeg.md)
* [NDI](../../general/network-streaming/ndi.md)
* [RTMP](../../general/network-streaming/rtmp.md)
* [RTSP](../../general/network-streaming/rtsp.md)
* [SRT](../../general/network-streaming/srt.md)
* [UDP](../../general/network-streaming/udp.md)
* [WMV](../../general/network-streaming/wmv.md)
* [YouTube](../../general/network-streaming/youtube.md)


## Transcription de la vidéo

### Ce que nous construisons

Affichons l'aperçu d'une webcam et diffusons-la sur le réseau en RTMP, dans une application C# WPF avec le VisioForge Video Capture SDK. Voici ce que nous construisons. Une application WPF qui affiche l'aperçu en direct de la webcam et, en même temps, diffuse la caméra et le microphone en RTMP. La vidéo est en H.264, encodée sur le GPU quand le PC a un encodeur matériel, et l'audio en AAC. Le flux part vers n'importe quel serveur RTMP : MediaMTX, YouTube Live ou un CDN. Voici l'application terminée, à gauche, qui diffuse vers un serveur MediaMTX du réseau local. À droite, un navigateur relit le même flux depuis ce serveur. Pourquoi RTMP ? C'est le protocole d'ingestion de YouTube Live, de Twitch et de la plupart des CDN. Un serveur multimédia le reçoit et le redistribue en HLS, WebRTC ou RTSP, si bien que les spectateurs n'ont besoin de rien de particulier. Et il transporte du H.264 et de l'AAC, que tout lecteur sait décoder. SRT, RTSP, HLS et WHIP sont des sorties du même moteur, ajoutées de la même façon.

### Prérequis

Il vous faut Windows 10 ou 11 et Visual Studio 2022 ou 2026 avec la charge de travail de développement .NET Desktop. Le projet cible .NET 8 ou une version ultérieure. Et une webcam, ainsi qu'un serveur RTMP vers lequel diffuser. Ici, c'est MediaMTX, un serveur multimédia gratuit et open source : un seul exécutable qui reçoit le RTMP sur le port 1935.

### Créer un projet WPF

Dans Visual Studio, créez un nouveau projet. Cherchez WPF et choisissez WPF Application pour C#. Nommez le projet WebcamRtmpWpf. Choisissez .NET 10 et cliquez sur Create.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet. Le Video Capture SDK lui-même. CrossPlatform Core, le runtime GStreamer sur lequel tourne le moteur X, avec le sink RTMP et les encodeurs H.264. Et CrossPlatform Libav, qui contient l'encodeur AAC. Ou utilisez le gestionnaire de paquets NuGet : clic droit sur le projet, Manage NuGet Packages, puis installez VisioForge.DotNet.VideoCapture depuis l'onglet Browse. Installez ensuite les paquets Core et Libav pour Windows x64. L'onglet Installed affiche les trois. Ce sont les paquets x64 : réglez donc PlatformTarget sur x64 dans le fichier projet. Une application 32 bits prend les paquets x86.

### Construire la fenêtre en XAML

La fenêtre s'écrit en XAML. D'abord la fenêtre elle-même, avec une grille à deux colonnes et un VideoView, et le concepteur la dessine aussitôt. Associez l'espace de noms VisioForge.Core.UI.WPF à un préfixe, ici vf. Le VideoView va dans la colonne de droite : c'est là que s'affiche l'aperçu en direct. La fenêtre nomme aussi ses gestionnaires Loaded et Closing. Ensuite, les listes des caméras et des microphones, et une zone de texte pour l'adresse RTMP. La colonne de gauche est un StackPanel : une liste déroulante pour les caméras, une pour les microphones et une zone de texte pour l'URL RTMP. Sa valeur par défaut pointe vers un serveur MediaMTX sur cet ordinateur. Enfin, les boutons Start et Stop. Chaque bouton nomme un gestionnaire Click. Tous deux démarrent désactivés : Start jusqu'au chargement du SDK, et Stop jusqu'à ce qu'un flux soit en cours. Chaque gestionnaire nommé par le XAML va dans MainWindow.xaml.cs, et les chapitres suivants les écrivent.

### Initialiser le SDK

MainWindow.xaml.cs commence par ces using : le cœur du SDK, les types de sortie et de source X, et le moteur VideoCaptureCoreX. Au chargement de la fenêtre, attendez VisioForgeX.InitSDKAsync une fois. Créez ensuite le moteur VideoCaptureCoreX en lui passant le VideoView, où l'aperçu est dessiné. Les erreurs du moteur arrivent dans l'événement OnError ; ici, elles vont dans la barre de titre, et c'est aussi là qu'apparaît un serveur injoignable.

### Lister les périphériques

DeviceEnumerator.Shared.VideoSourcesAsync liste les caméras. Gardez le tableau : le gestionnaire Start y prend le périphérique sélectionné, et la liste déroulante affiche le nom de chacun. AudioSourcesAsync liste les microphones de la même façon. La première caméra et le premier microphone sont sélectionnés, et Start n'est activé que maintenant, une fois le SDK chargé.

### Choisir la caméra et le microphone

Le gestionnaire Start vérifie d'abord qu'il y a une caméra et une adresse RTMP. Une adresse vide n'échouerait pas : le sink reviendrait à son propre serveur par défaut, d'où l'arrêt ici. La source vidéo est un VideoCaptureDeviceSourceSettings pour la caméra sélectionnée. Par défaut, il prend du 1080p à 25 images par seconde ou plus quand la caméra le permet, et du 720p sinon. Le microphone devient la source audio via CreateSourceSettingsVC, et Audio_Record l'ajoute au flux. Audio_Play vaut false : l'application ne rejoue pas le microphone dans les haut-parleurs.

### Diffuser en RTMP

Le flux lui-même tient en une ligne : un RTMPOutput avec l'URL de la zone de texte, ajouté au moteur. L'aperçu continue dans le VideoView pendant que le flux part. Une sortie reste dans le moteur après un flux : Outputs_Clear s'exécute donc avant chaque nouveau flux. Sans réglages d'encodeur, RTMPOutput prend l'encodeur H.264 du GPU : NVIDIA NVENC, AMD AMF ou Intel Quick Sync, et OpenH264 sur le CPU quand le PC n'en a aucun. L'audio est en AAC. Ses propriétés Video et Audio acceptent d'autres réglages d'encodeur, par exemple un débit fixe. L'URL désigne le serveur et le flux. Sur MediaMTX, le chemin après le port est le nom du flux, et ce même chemin se relit en HLS, WebRTC et RTSP. Pour YouTube Live, c'est l'adresse d'ingestion suivie de votre clé de flux. Et rtmps, avec un s, se connecte en TLS.

### Démarrer et arrêter

StartAsync indique si la capture a démarré, et les boutons suivent. StopAsync arrête le flux et l'aperçu. Si la fenêtre se ferme pendant la diffusion, annulez la fermeture, attendez StopAsync, puis refermez. Lors de ce second passage, le moteur est libéré, ainsi que le SDK.

### Lancer l'application

Lancez l'application depuis Visual Studio. Une fois le SDK chargé, les listes des caméras et des microphones se remplissent et Start est activé. Saisissez l'adresse du serveur MediaMTX du réseau local, avec webcam comme nom de flux. Cliquez sur Start. L'aperçu démarre, et la même image part vers le serveur. Passons maintenant de l'autre côté. MediaMTX sert chaque flux dans son propre lecteur web, sur le port 8889 avec le même chemin. Le navigateur à droite relit le flux depuis le serveur, avec un léger retard sur l'aperçu. Fermez le lecteur, cliquez sur Stop et fermez la fenêtre.

### Erreurs fréquentes

Quelques erreurs fréquentes. Oublier le paquet Libav, qui contient l'encodeur AAC. Mélanger les paquets x86 et x64, ou lancer une application x86 avec les paquets x64. Un pare-feu qui bloque le port 1935 vers le serveur. Et une adresse YouTube sans la clé de flux à la fin. Quel que soit le problème, le moteur le signale dans l'événement OnError.

### Étapes suivantes

Il existe aussi des guides pour les sorties SRT, RTSP, HLS et WebRTC, pour enregistrer un MP4 pendant la diffusion, pour diffuser une caméra IP ou l'écran, et pour YouTube Live et Facebook Live. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci de nous avoir regardés.

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
