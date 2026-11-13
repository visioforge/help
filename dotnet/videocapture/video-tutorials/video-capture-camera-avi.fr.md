---
title: Enregistrer une webcam en AVI en C# WinForms (MJPEG, MP3)
description: "Enregistrez une webcam en AVI depuis une app C# WinForms avec VideoCaptureCore : codec vidéo au choix, audio PCM ou MP3, vérification avec ffprobe."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - AVI
  - MJPEG
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - AVIOutput
  - MP3Output
  - VideoView
video:
  id: f5EXTFQETKA
  upload_date: 2026-11-12T15:20:03Z
  duration: PT10M56S

---

# Enregistrement de webcam vers un fichier AVI dans les applications .NET C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel enregistre une webcam dans un fichier AVI depuis une application C# WinForms avec le moteur VideoCaptureCore. Le codec vidéo est choisi parmi ceux installés sur le PC (MJPEG par défaut), et l'audio est en PCM non compressé ou en MP3.

**Quand choisir AVI plutôt que MP4 :** avec MJPEG, chaque image est une image complète : vous pouvez donc couper et monter sur n'importe quelle image, et lire chaque image séparément pour l'analyse ; l'audio PCM est sans perte. Le prix, c'est la taille : environ vingt mégaoctets par seconde en 1080p. Pour le web, les téléphones et le partage, enregistrez plutôt en [MP4 avec H.264](video-capture-webcam-mp4.md).

Ce tutoriel utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK. Sous macOS, Linux, Android et iOS, VideoCaptureCoreX enregistre aussi en AVI : consultez [Sortie AVI](../../general/output-formats/avi.md) et [Enregistrer la vidéo d'une webcam en C#](../guides/save-webcam-video.md).

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/f5EXTFQETKA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-avi-winforms)

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et le redist de capture vidéo avec ses filtres DirectShow. AVI n'a besoin de rien d'autre :

- Pour les applications 64 bits : [redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Pour les applications 32 bits : [redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Réglez `PlatformTarget` dans le fichier projet selon le redist (`x64` ou `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des listes déroulantes pour la caméra, le format, la fréquence d'images, le micro et le codec vidéo, une case à cocher « MP3 audio » et les boutons Start et Stop ; le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, le même code se compile dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamAvi;

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
        cbMP3.Enabled = cbMicrophone.Items.Count > 0;

        // Les codecs vidéo installés sur ce PC
        foreach (var codec in videoCapture1.Video_Codecs())
            cbVideoCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "MJPEG Compressor";
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
        if (cbFrameRate.SelectedItem is null)
        {
            Text = "No camera, format or frame rate";
            return;
        }

        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        Text = "Webcam to AVI";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // pas de micro : enregistrer la vidéo seule
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // enregistrer le micro sans le relire

        // Audio : MP3, ou PCM non compressé
        var avi = cbMP3.Checked ? new AVIOutput(new MP3Output()) : new AVIOutput();
        avi.Video_Codec = cbVideoCodec.Text;

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");
        videoCapture1.Output_Format = avi;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

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
        // Terminer l'AVI avant la fermeture de la fenêtre
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // démarrage ou arrêt déjà en cours

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // refermer une fois ce gestionnaire terminé
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Fonctionnalités clés de l'implémentation

1. **Choix du périphérique et du format** - La caméra, son format, sa fréquence d'images et le micro sont choisis dans des listes remplies à partir des périphériques eux-mêmes ; sans micro, `Audio_RecordAudio` passe à `false` et l'AVI ne contient que la vidéo
2. **Format de la caméra** - `Format_UseBest` est activé par défaut : il choisit la résolution la plus élevée jusqu'à 1080p, puis préfère MJPG et la fréquence d'images la plus rapide. Pour enregistrer en 4K, ou dans le format de votre choix, désactivez-le et réglez `Format` et `FrameRate`
3. **Codec vidéo** - `Video_Codecs()` renvoie les compresseurs vidéo DirectShow installés sur le PC ; `AVIOutput.Video_Codec` prend l'un de ces noms et vaut `MJPEG Compressor` par défaut, qui stocke chaque image comme une image JPEG. Un codec que vous installez vous-même apparaît dans la même liste
4. **Audio** - Un `AVIOutput` simple écrit du PCM non compressé, 16 bits stéréo à 44,1 kHz, environ dix mégaoctets par minute ; `new AVIOutput(new MP3Output())` écrit du MP3 à 192 kbit/s, environ un mégaoctet et demi par minute
5. **Fonctionnement asynchrone** - `StartAsync` et `StopAsync` gardent l'interface réactive, et la fermeture du formulaire termine d'abord le fichier AVI

## Vérifier le fichier avec ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.avi -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=mjpeg|width=1920|height=1080
# stream|codec_name=pcm_s16le|sample_rate=44100|channels=2
# format|format_name=avi|duration=14.883073|size=316108288
```

Quinze secondes de MJPEG en 1080p pèsent environ trois cents mégaoctets, soit à peu près 170 mégabits par seconde : c'est pourquoi AVI sert au montage et à l'analyse, pas au partage.

## Transcription de la vidéo

### Ce que nous construisons

Capturons une webcam dans un fichier AVI depuis une application C# WinForms, avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une fenêtre WinForms avec un aperçu en direct de la caméra. Elle enregistre un fichier AVI avec n'importe quel codec vidéo installé sur le PC, MJPEG par défaut. L'audio est en PCM non compressé, ou en MP3 si vous cochez une case. La caméra, son format, la fréquence d'images et le micro sont choisis dans des listes. Voici l'application terminée, qui enregistre depuis une Logitech C920.

### AVI ou MP4

Quand choisir AVI plutôt que MP4 ? Avec MJPEG, chaque image est une image complète : vous pouvez couper et monter sur n'importe quelle image, et lire chaque image seule pour l'analyser. L'audio PCM est sans perte. Le prix, c'est la taille : ici, environ vingt mégaoctets par seconde en 1080p. Pour le web, les téléphones et le partage, MP4 avec H.264 est le meilleur choix. C'est la vidéo webcam vers MP4.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Et une webcam avec un micro. VideoCaptureCore est le moteur DirectShow du SDK : cette application ne fonctionne que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet WebcamAvi. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet : le SDK lui-même, et le redist de capture vidéo avec ses filtres DirectShow. AVI n'a besoin de rien d'autre. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.Core et installez-le. Puis installez le redist de capture vidéo pour x64. Le redist est celui pour x64 : réglez donc PlatformTarget sur x64 dans le fichier du projet. Une application x86 prend le redist x86 à la place.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'aperçu en direct. Ajoutez une étiquette et une liste déroulante pour la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour le format de la caméra, la fréquence d'images, le micro et le codec vidéo. Ajoutez une case à cocher pour l'audio MP3, et les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire, sur les listes de la caméra et du format, et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu en direct s'affiche. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles s'affichent dans la barre de titre.

### Lister les périphériques

Video_CaptureDevices renvoie toutes les caméras, et Audio_CaptureDevices tous les micros. Nous listons leurs noms et sélectionnons le premier de chaque liste. Sans micro, la case MP3 est désactivée. Quand la caméra change, son gestionnaire remplit la liste des formats avec ceux de cette caméra, et choisit 1920x1080 MJPG s'il existe. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images, et choisit trente images par seconde.

### Choisir le format de la caméra

Le gestionnaire de Start vérifie d'abord qu'une fréquence d'images et un codec vidéo sont sélectionnés. Sinon, il l'indique dans la barre de titre. Puis il sélectionne la caméra par son nom, désactive Format_UseBest, et transmet le format et la fréquence d'images choisis dans les listes. Format_UseBest est activé par défaut. Il choisit la résolution la plus haute jusqu'à 1080p, puis préfère MJPG et la fréquence d'images la plus rapide. Pour enregistrer en 4K, ou dans le format de votre choix, désactivez-le et réglez Format et FrameRate, comme ici.

### Sélectionner le micro

Sans aucun micro dans la liste, l'application capture uniquement la vidéo. Sinon, elle enregistre celui qui est sélectionné. Audio_PlayAudio est désactivé : le micro n'est pas joué dans les haut-parleurs pendant l'enregistrement.

### Choisir le codec vidéo

De retour dans le gestionnaire Load, Video_Codecs liste les codecs vidéo installés sur ce PC. Nous remplissons la liste des codecs et sélectionnons MJPEG Compressor. Video_Codecs renvoie les compresseurs vidéo DirectShow de ce PC. Celui-ci en a deux : MJPEG Compressor et DV Video Encoder. MJPEG enregistre chaque image comme une image JPEG. DV n'écrit que du DV en définition standard. Un codec que vous installez vous-même apparaît dans la même liste.

### AVIOutput : audio PCM ou MP3

Passons à la sortie. Un AVIOutput simple écrit un audio PCM non compressé. Passez un MP3Output à son constructeur, et l'audio est en MP3. Réglez Video_Codec sur le nom choisi dans la liste, écrivez output.avi dans le dossier Vidéos, et passez le moteur en mode capture vidéo, pour qu'il enregistre au lieu de seulement afficher l'aperçu. Un AVIOutput simple écrit du PCM : stéréo 16 bits à 44,1 kilohertz, sans perte, environ dix mégaoctets par minute. Avec un MP3Output, il écrit du MP3 à 192 kilobits par seconde, environ un mégaoctet et demi par minute. À côté de la vidéo MJPEG, les deux sont petits.

### Démarrer et arrêter

Start et Stop sont asynchrones. StartAsync revient dès que la capture tourne, et StopAsync termine l'écriture du fichier AVI.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait l'AVI inachevé. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant l'enregistrement, il annule la fermeture. Puis il arrête l'enregistrement, et ferme de nouveau la fenêtre une fois le gestionnaire terminé. Quand rien n'est enregistré, le gestionnaire libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes de la caméra, du format et du micro se remplissent à partir des périphériques. La liste des codecs montre les deux codecs de ce PC. MJPEG reste sélectionné. Start lance l'enregistrement. La C920 met quelques secondes à envoyer sa première image. Stop termine output.avi, puis nous fermons la fenêtre.

### Vérifier le fichier avec ffprobe

Le fichier arrive dans Vidéos : environ trois cents mégaoctets pour quinze secondes. ffprobe montre ce qu'il contient. Le conteneur est AVI. La vidéo est en MJPEG, en 1920x1080. L'audio est en PCM, 16 bits, stéréo à 44,1 kilohertz. Cela fait environ 170 mégabits par seconde : voilà pourquoi AVI sert au montage et à l'analyse, pas au partage.

### Erreurs fréquentes

Quelques erreurs fréquentes. Un paquet redist manquant : le moteur ne démarre pas. Une caméra déjà ouverte dans une autre application. Des paquets x86 et x64 mélangés dans un même projet. Fermer la fenêtre en plein enregistrement : attendez d'abord StopAsync. Et AVI là où il faut du MP4 : une heure de MJPEG en 1080p atteint des dizaines de gigaoctets.

### Étapes suivantes

Ensuite, la vidéo webcam vers MP4 enregistre la même caméra dans de petits fichiers H.264. Il existe aussi des guides pour les caméras IP, l'enregistrement d'écran et les captures d'image. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Webcam vers MP4](video-capture-webcam-mp4.md) — la même caméra enregistrée dans de petits fichiers H.264
- [Capture d'écran vers AVI](screen-capture-avi.md) — enregistrer l'écran dans un fichier AVI
- [Aperçu vidéo de la caméra](camera-video-preview.md) — afficher la vidéo de la caméra en direct avec capture d'images

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
