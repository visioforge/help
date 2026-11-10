---
title: Enregistrer une webcam en MP4 avec encodage H.264 en C# .NET
description: Énumérez les webcams, encodez en H.264/AAC et enregistrez en MP4 avec VisioForge Video Capture SDK. Capture asynchrone avec aperçu WinForms et exemples C#.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
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
  id: fAt1Bp6qYPc
  upload_date: 2026-11-03T13:29:29Z
  duration: PT7M40S

---

# Implémentation de capture vidéo de webcam en MP4 en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la capture de webcam dans les applications .NET

Créer des applications qui enregistrent la vidéo depuis des webcams vers des fichiers MP4 est un besoin courant dans de nombreux projets logiciels. Ce tutoriel propose une approche orientée développeurs pour implémenter cette fonctionnalité à l'aide de techniques modernes .NET et de Video Capture SDK.

Lors de l'implémentation des capacités d'enregistrement de webcam, les développeurs doivent prendre en compte :

- La sélection des périphériques d'entrée vidéo et audio appropriés
- La configuration des paramètres de compression vidéo et audio
- La gestion du cycle de vie de la capture (initialisation, démarrage, arrêt)
- La gestion de la création et de l'organisation du fichier de sortie

Ce tutoriel utilise le moteur VideoCaptureCore, réservé à Windows. [Video Capture SDK .NET](https://www.visioforge.com/fr/video-capture-sdk-net) enregistre aussi les webcams en MP4 sous macOS, Linux, Android et iOS avec VideoCaptureCoreX : consultez [Enregistrer la vidéo d'une webcam en C#](../guides/save-webcam-video.md) et les guides de déploiement [Android](../../deployment-x/Android.md) et [iOS](../../deployment-x/iOS.md).

## Tutoriel vidéo d'implémentation

La vidéo suivante illustre le processus complet de mise en place d'une application de capture de webcam :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fAt1Bp6qYPc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Pour les développeurs qui préfèrent examiner l'implémentation complète, tout le code source est disponible dans notre dépôt GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-mp4-winforms)

## Composants redistribuables requis

Avant d'implémenter la solution, installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et les paquets redistribuables nécessaires :

### Composants de capture vidéo

- Pour les applications 32 bits : [Redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Pour les applications 64 bits : [Redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Composants d'encodage MP4

- Pour les applications 32 bits : [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- Pour les applications 64 bits : [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Exemple d'implémentation avec code C#

L'implémentation C# suivante montre comment créer une application Windows Forms qui capture la vidéo d'une webcam et l'enregistre dans un fichier MP4. Le formulaire contient un `VideoView`, des listes déroulantes pour la caméra, le format, la fréquence d'images et le micro, ainsi que les boutons Start et Stop ; le fichier du concepteur se trouve dans le code source indiqué plus haut. Le projet cible .NET 10 avec les usings implicites.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamMp4;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Afficher les erreurs du SDK dans la barre de titre
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
    }

    private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        var format = device.VideoFormats.First(f => f.Name == cbFormat.Text);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam to MP4";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(cbMicrophone.Text);
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
            Text = "Hello World!", Left = 50, Top = 50, FontColor = Color.Red
        });
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime, DateTimeMask = "HH:mm:ss", Text = "",
            Left = 50, Top = 110
        });

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Finaliser le MP4 avant la fermeture de la fenêtre
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // démarrage ou arrêt déjà en cours

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            Close();
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Fonctionnalités clés de l'implémentation

Cette implémentation offre plusieurs capacités importantes pour les développeurs :

1. **Sélection du périphérique et du format** — La caméra, son format, sa fréquence d'images et le micro sont choisis dans des listes remplies à partir des périphériques eux-mêmes
2. **Encodage matériel avec repli sur le CPU** — `MP4HWOutput` encode en H.264 sur un GPU NVIDIA, Intel ou AMD ; `HWEncodersAvailable` indique les encodeurs présents sur le PC, et `MS_H264` s'exécute sur le CPU s'il n'y en a aucun
3. **Paramètres de l'encodeur** — Une moyenne de 6000 kbps, des pointes jusqu'à 9000 kbps, le profil Main et l'audio AAC à 192 kbps
4. **Superposition de texte** — Un texte fixe et une horloge qui affiche l'heure actuelle, ajoutés comme effets vidéo
5. **Opération asynchrone** — `StartAsync` et `StopAsync` gardent l'interface réactive, et la fermeture du formulaire finalise d'abord le fichier MP4

## Options de configuration avancée

Les développeurs peuvent étendre la solution avec des fonctionnalités supplémentaires :

- HEVC au lieu de H.264 : `NVENC_H265`, `QSV_H265` ou `AMD_H265`
- Autres effets et transformations vidéo à la volée
- Prise en charge de plusieurs pistes audio

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons une webcam dans un fichier MP4 depuis une application C# WinForms avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une fenêtre WinForms avec un aperçu de la caméra en direct. Elle enregistre en MP4, avec du H.264 encodé sur le GPU quand le PC en a un, et de l'audio AAC. La caméra, son format et le microphone se choisissent dans des listes, et l'application complète fait une centaine de lignes de C#. Voici l'application terminée, qui enregistre depuis une Logitech BRIO.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail de développement .NET Desktop. Le projet peut cibler .NET 8 ou version ultérieure, ou .NET Framework 4.7.2. Cette vidéo porte sur WinForms. La version WPF arrive ensuite.

### Créer un projet WinForms

Dans Visual Studio, ouvrez File, New, Project. Recherchez Windows Forms et choisissez Windows Forms App pour C#. Nommez le projet WebcamMp4. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet : le SDK lui-même, le redist de capture vidéo et le redist MP4. Ce sont les paquets x64. Une application x86 utilise plutôt les redists x86. Vous pouvez aussi installer ces trois paquets depuis le gestionnaire de paquets NuGet.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'aperçu en direct. Ajoutez une étiquette et une liste déroulante pour la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour le format de la caméra, la fréquence d'images et le microphone. Ajoutez les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire, les listes déroulantes et les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où s'affiche l'aperçu en direct.

### Énumérer les caméras

Video_CaptureDevices renvoie toutes les caméras de la machine. Nous ajoutons leurs noms à la liste et sélectionnons la première. Quand la caméra change, son gestionnaire remplit la liste avec les formats de cette caméra. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images. Une caméra indique son nom, des formats comme 1920x1080 MJPG ou 1280x720 YUY2, et les fréquences d'images de chaque format. MJPG est compressé dans la caméra, donc les hautes résolutions passent par USB. YUY2 n'est pas compressé, donc USB 2 ne transporte que des résolutions plus basses.

### Choisir le format de la caméra

Sélectionnez la caméra par son nom. Désactivez ensuite Format_UseBest et passez le format et la fréquence d'images choisis dans les listes. Format_UseBest est activé par défaut. Il choisit la plus haute résolution proposée par la caméra, jusqu'à 1080p, et privilégie MJPG. Un format vide y revient également.

### Sélectionner le micro

Le microphone fonctionne de la même façon. Audio_CaptureDevices les énumère, et nous utilisons celui choisi dans la liste.

### MP4HWOutput : tout GPU, repli sur le CPU

Pour la sortie, créez un MP4HWOutput, écrivez output.mp4 dans le dossier Videos et passez le moteur en mode capture vidéo, pour qu'il enregistre au lieu de seulement afficher l'aperçu. MP4HWOutput encode sur le GPU : NVENC sur NVIDIA, Quick Sync sur Intel et AMF sur AMD, en H.264 ou HEVC. Sans encodeur matériel, l'encodeur Windows sur le CPU prend le relais. Le code et le fichier sont les mêmes sur tous les PC. HWEncodersAvailable indique les encodeurs disponibles sur ce PC. Prenez d'abord NVIDIA, puis Intel, puis AMD, et sinon l'encodeur CPU.

### Régler l'encodeur H.264

Réglez ensuite l'encodeur : six mille kilobits par seconde en moyenne, des pointes jusqu'à neuf mille, le profil Main et l'audio AAC à cent quatre-vingt-douze. À titre indicatif : de 2500 à 4000 kilobits pour le 720p, de 5000 à 8000 pour le 1080p et de 15000 à 25000 pour la 4K. Les deux débits se règlent en kilobits par seconde.

### Ajouter une superposition de texte

Les effets vidéo ajoutent une superposition de texte : un Hello World fixe et une horloge qui affiche l'heure actuelle sur chaque image.

### Démarrer et arrêter

Start et Stop sont asynchrones. StartAsync rend la main une fois la capture lancée, et StopAsync finalise le fichier MP4. Lancez l'application depuis Visual Studio. La liste des formats vient directement de la caméra. Start lance l'enregistrement, avec l'horloge en superposition qui tourne. Stop finalise output.mp4. Le fichier arrive dans Videos : 1920x1080, vidéo H.264 et audio AAC.

### Erreurs fréquentes

Quelques erreurs courantes. Un paquet redist manquant : le moteur ne démarre pas. Une caméra déjà ouverte dans une autre application. Des paquets x86 et x64 mélangés dans un même projet. Et libérer le moteur en pleine capture : attendez d'abord StopAsync. Les erreurs du moteur arrivent dans l'événement OnError.

### Étapes suivantes

Ensuite, nous créerons la même application en WPF. Il existe aussi des guides sur les caméras IP, l'enregistrement d'écran, les captures d'images et le streaming réseau. Le code complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Webcam vers MP4 en WPF](video-capture-webcam-mp4-wpf.md) — la même application sous forme de projet WPF, avec la fenêtre en XAML
- [Enregistrement pré-événement](../guides/pre-event-recording.md) — enregistrement avec tampon circulaire et déclencheurs de détection de mouvement pour webcams et caméras IP
- [Aperçu vidéo de caméra](camera-video-preview.md) — afficher la vidéo de caméra en direct avec capture d'images

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et d'exemples d'implémentation.
