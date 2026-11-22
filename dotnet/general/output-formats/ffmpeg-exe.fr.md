---
title: Configuration de sortie FFMPEG.exe dans les SDK vidéo .NET
description: Configurez la sortie FFMPEG.exe en .NET pour capture et édition vidéo avec accélération matérielle, codecs personnalisés et options pro.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Encoding
  - Editing
  - MP4
  - WebM
  - AVI
  - MOV
  - TS
  - H.264
  - H.265
  - VP9
  - Opus
  - Webcam
  - WinForms
  - ProRes
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEditCore
  - FFMPEGEXEOutput
  - H264MFSettings
  - H264NVENCSettings
  - HEVCMFSettings
  - BasicVideoSettings
  - BasicAudioSettings
  - VideoCaptureSource
video:
  id: bNn7p2H6i3M
  upload_date: 2026-11-17T06:59:13Z
  duration: PT13M6S

---

# Intégration de FFMPEG.exe avec les SDK VisioForge .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

!!! note "Édition"
    La sortie FFmpeg.exe nécessite l'édition Premium dans `VideoCaptureCore` comme dans `VideoEditCore`. Avec une licence Standard ou Professional, la tâche s'exécute en mode d'essai, avec le filigrane d'essai, uniquement pendant les 30 jours d'essai ; ensuite, `Start` renvoie `false`.

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/bNn7p2H6i3M?controls=1" title="Enregistrer une webcam en MP4 et WebM avec FFmpeg en C# - tutoriel YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-ffmpeg-winforms)

La vidéo crée le projet en .NET 10, qui nécessite Visual Studio 2026 ; dans Visual Studio 2022, choisissez .NET 8 ou .NET 9.

## Introduction à la sortie FFMPEG en .NET

Ce guide fournit des instructions détaillées pour implémenter une sortie FFMPEG.exe dans des applications Windows à l'aide des SDK .NET de VisioForge. L'intégration fonctionne aussi bien avec [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) qu'avec [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net), en utilisant les moteurs `VideoCaptureCore` et `VideoEditCore`.

FFMPEG fonctionne comme un puissant framework multimédia qui permet aux développeurs de produire une sortie dans une grande variété de formats vidéo et audio. Sa flexibilité provient d'une prise en charge étendue des codecs et d'un contrôle granulaire sur les paramètres d'encodage pour les flux vidéo comme audio.

## Pourquoi utiliser FFMPEG avec les SDK VisioForge ?

L'intégration de FFMPEG dans vos applications propulsées par VisioForge procure plusieurs avantages techniques :

- **Polyvalence des formats** : prise en charge de pratiquement tous les formats de conteneur modernes
- **Flexibilité des codecs** : accès aux codecs open source comme propriétaires
- **Optimisation des performances** : options pour l'accélération CPU et GPU
- **Profondeur de personnalisation** : contrôle fin des paramètres d'encodage

## Fonctionnalités et capacités clés

### Formats de sortie pris en charge

FFMPEG prend en charge de nombreux formats de conteneur, parmi lesquels :

- MP4 (MPEG-4 Part 14)
- WebM (VP8/VP9 avec Vorbis/Opus)
- MKV (Matroska)
- AVI (Audio Video Interleave)
- MOV (QuickTime)
- WMV (Windows Media Video)
- FLV (Flash Video)
- TS (MPEG Transport Stream)

### Options d'accélération matérielle

L'encodage vidéo moderne profite des technologies d'accélération matérielle qui améliorent significativement la vitesse et l'efficacité d'encodage :

- **Intel QuickSync** : exploite les graphiques intégrés Intel pour l'encodage H.264 et HEVC
- **NVIDIA NVENC** : utilise les GPU NVIDIA pour un encodage accéléré (nécessite une carte graphique NVIDIA compatible)
- **AMD AMF/VCE** : utilise les processeurs graphiques AMD pour l'accélération de l'encodage

### Prise en charge des codecs vidéo

L'intégration offre l'accès à plusieurs codecs vidéo avec des paramètres personnalisables :

- **H.264/AVC** : standard industriel avec un excellent rapport qualité/taille
- **H.265/HEVC** : codec à plus haute efficacité pour le contenu 4K et au-delà
- **VP9** : codec vidéo ouvert de Google utilisé dans WebM
- **MPEG-2** : codec historique pour la compatibilité DVD et diffusion
- **ProRes** : codec professionnel pour les workflows d'édition

## Processus d'implémentation

### 1. Configuration de votre environnement de développement

Avant d'implémenter la sortie FFMPEG, assurez-vous que votre environnement de développement est correctement configuré :

1. Créez un nouveau projet .NET ou ouvrez-en un existant
2. Installez les paquets NuGet du SDK VisioForge appropriés
3. Ajoutez les paquets de dépendance FFMPEG (détaillés dans la section Dépendances)
4. Importez les espaces de noms nécessaires dans votre code :

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.Types.Output;     // FFMPEGEXEOutput
using VisioForge.Core.Types.FFMPEGEXE;  // H264MFSettings, H264NVENCSettings, H264QSVSettings, HEVCQSVSettings, OutputMuxer, BasicAudioSettings, ...
```

### 2. Initialisation de la sortie FFMPEG

Commencez par créer une instance de `FFMPEGEXEOutput` pour gérer votre configuration de sortie :

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
```

Cet objet servira de conteneur pour tous vos paramètres et préférences d'encodage.

### 3. Configuration du format de conteneur de sortie

Définissez le format de conteneur de sortie souhaité à l'aide de la propriété `OutputMuxer` :

```csharp
ffmpegOutput.OutputMuxer = OutputMuxer.MP4;
```

Les autres options de conteneur courantes incluent :

- `OutputMuxer.Matroska` — pour le conteneur Matroska (.mkv)
- `OutputMuxer.WebM` — pour le format WebM
- `OutputMuxer.AVI` — pour le format AVI
- `OutputMuxer.MOV` — pour le conteneur QuickTime

### 4. Configuration de l'encodeur vidéo

FFMPEG propose plusieurs options d'encodeur vidéo. Sélectionnez et configurez l'encodeur approprié en fonction de vos besoins et du matériel disponible :

#### Encodage H.264 standard basé sur le CPU

```csharp
var videoEncoder = new H264MFSettings
{
    Bitrate     = 5000000, // 5 Mbps
    RateControl = H264MFRateControl.CBR,
};
ffmpegOutput.Video = videoEncoder;
```

#### Encodage NVIDIA accéléré matériellement

```csharp
var nvidiaEncoder = new H264NVENCSettings
{
    Bitrate = 8000000,        // 8 Mbps
};
ffmpegOutput.Video = nvidiaEncoder;
```

#### Encodage Intel QuickSync accéléré matériellement

```csharp
var intelEncoder = new H264QSVSettings
{
    Bitrate = 6000000
};
ffmpegOutput.Video = intelEncoder;
```

#### Encodage HEVC/H.265 pour une efficacité supérieure

```csharp
var hevcEncoder = new HEVCQSVSettings
{
    Bitrate = 3000000,  
};
ffmpegOutput.Video = hevcEncoder;
```

### 5. Configuration de l'encodeur audio

Configurez vos paramètres d'encodage audio en fonction des exigences de qualité et de la compatibilité avec la plateforme cible :

```csharp
var audioEncoder = new BasicAudioSettings
{
    Bitrate = 192000,    // 192 kbps
    Channels = 2,        // Stéréo
    SampleRate = 48000,  // 48 kHz — standard professionnel
    Encoder = AudioEncoder.AAC,
    Mode = AudioMode.CBR
};

ffmpegOutput.Audio = audioEncoder;
```

### 6. Configuration finale et exécution

Appliquez tous les paramètres et démarrez le processus d'encodage :

```csharp
// Appliquer les paramètres de format
core.Output_Format = ffmpegOutput;

// Définir le mode de fonctionnement
core.Mode = VideoCaptureMode.VideoCapture;  // Pour Video Capture SDK
// core.Mode = VideoEditMode.Convert;       // Pour Video Edit SDK

// Définir le chemin de sortie
core.Output_Filename = "output.mp4";

// Lancer le traitement
await core.StartAsync();
```

## Dépendances requises

Installez les paquets NuGet suivants en fonction de votre architecture cible pour assurer un fonctionnement correct :

### Dépendances Video Capture SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x64
```

Pour les cibles x86 :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x86
```

### Dépendances Video Edit SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x64
```

Pour les cibles x86 :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x86
```

## Dépannage et optimisation

### Problèmes courants et solutions

- **Erreurs « codec introuvable »** : un encodeur absent de l'exécutable FFmpeg fait échouer `Start` / `StartAsync`, et `OnError` indique l'encodeur et l'exécutable. Le `vfffmpeg64.exe` fourni n'a ni libx264 ni libx265 : utilisez `H264MFSettings`, `OpenH264Settings` ou un encodeur matériel, ou indiquez dans `ExecutableFilename` une version de FFmpeg qui les contient. `ffmpeg -encoders` liste ce que contient une version
- **Échecs d'accélération matérielle** : un encodeur QSV, NVENC ou AMF sur un PC sans ce GPU démarre, puis FFmpeg s'arrête ; `OnError` le signale avec la dernière ligne d'erreur de FFmpeg. Vérifiez la compatibilité du GPU et la version des pilotes
- **Problèmes de performance** : ajustez le nombre de threads et le preset d'encodage selon les ressources CPU disponibles
- **Problèmes de qualité de sortie** : affinez le débit, le profil et les paramètres d'encodage

### Conseils d'optimisation des performances

- Utilisez l'accélération matérielle lorsqu'elle est disponible
- Choisissez les presets appropriés en fonction de vos exigences de qualité/vitesse
- Définissez des débits raisonnables selon le type de contenu et la résolution
- Envisagez l'encodage en deux passes pour les scénarios non temps réel exigeant la plus haute qualité

## Code du tutoriel : webcam vers MP4 et WebM

Le `Form1.cs` complet de la vidéo. Le formulaire contient un `VideoView`, des listes déroulantes pour la caméra, le format, la fréquence d'images, le micro et la sortie, et les boutons Start et Stop ; Stop est désactivé au départ. Le fichier du concepteur se trouve dans le code source sur GitHub. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, le même code se compile dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>` ; dans `Program.cs`, remplacez `ApplicationConfiguration.Initialize()`, que .NET Framework ne génère pas, par `Application.EnableVisualStyles()` et `Application.SetCompatibleTextRenderingDefault(false)`, et ajoutez `<dpiAware>true</dpiAware>` dans un `app.manifest` pour garder la prise en charge du DPI système qu'active .NET 10.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.FFMPEGEXE;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamFfmpeg;

public partial class Form1 : Form
{
    private const string Mp4H264 = "MP4 - H.264 + AAC";
    private const string Mp4Hevc = "MP4 - HEVC + AAC";
    private const string WebmVp9 = "WebM - VP9 + Opus";

    private VideoCaptureCore videoCapture1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        cbOutput.Items.Add(Mp4H264);
        cbOutput.Items.Add(WebmVp9);

        // HEVC needs a hardware encoder: an Intel, NVIDIA or AMD GPU
        var hw = VideoCaptureCore.HWEncodersAvailable();
        if (hw.QSV_H265 || hw.NVENC_H265 || hw.AMD_H265)
            cbOutput.Items.Add(Mp4Hevc);

        cbOutput.SelectedIndex = 0;
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

        Text = "Webcam to MP4 and WebM with FFmpeg";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // no microphone: record video only
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // record the microphone without playing it back

        var ffmpeg = new FFMPEGEXEOutput();
        string extension;
        switch (cbOutput.Text)
        {
            case Mp4Hevc:
                ffmpeg.OutputMuxer = OutputMuxer.MP4;
                ffmpeg.Video = new HEVCMFSettings
                {
                    ForceHWEncoding = true, // the GPU's encoder
                    RateControl = H264MFRateControl.CBR,
                    Bitrate = 6_000_000,
                };
                ffmpeg.Audio = Aac();
                extension = ".mp4";
                break;

            case WebmVp9:
                ffmpeg.OutputMuxer = OutputMuxer.WebM;
                ffmpeg.Video = new BasicVideoSettings
                {
                    Encoder = VideoEncoder.VP9,
                    BasicMode = VideoMode.CBR,
                    Bitrate = 8_000_000,
                };
                ffmpeg.Audio = new BasicAudioSettings
                {
                    Encoder = AudioEncoder.OPUS,
                    Mode = AudioMode.CBR,
                    Bitrate = 128_000,
                    SampleRate = 0, // 0: keep the microphone's own
                    Channels = 0,
                };
                extension = ".webm";
                break;

            default:
                ffmpeg.OutputMuxer = OutputMuxer.MP4;
                ffmpeg.Video = new H264MFSettings
                {
                    RateControl = H264MFRateControl.CBR,
                    Bitrate = 8_000_000,
                };
                ffmpeg.Audio = Aac();
                extension = ".mp4";
                break;
        }

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output" + extension);
        videoCapture1.Output_Format = ffmpeg;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private static BasicAudioSettings Aac() => new()
    {
        Encoder = AudioEncoder.AAC,
        Mode = AudioMode.CBR,
        Bitrate = 192_000,
        SampleRate = 0, // 0: keep the microphone's own
        Channels = 0,
    };

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Finish the file before the window goes away
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // already starting or stopping

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // close again once this handler has returned
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Ressources supplémentaires

- **[Exemple du tutoriel de sortie FFmpeg (Video Capture SDK, WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-ffmpeg-winforms)** — l'application de la vidéo : une webcam et son micro vers MP4 avec H.264 et AAC ou vers WebM avec VP9 et Opus, et vers MP4 avec HEVC quand le PC dispose d'un encodeur HEVC matériel Intel, NVIDIA ou AMD.

Pour davantage d'exemples de code et d'implémentation, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Pour en savoir plus sur les paramètres et les capacités de FFMPEG, consultez la [documentation officielle de FFMPEG](https://ffmpeg.org/documentation.html).

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons une webcam dans des fichiers MP4 et WebM via FFmpeg, dans une application C# WinForms avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une fenêtre WinForms avec un aperçu en direct de la caméra. Elle enregistre des fichiers MP4 avec de la vidéo H.264 et de l'audio AAC, et des fichiers WebM avec de la vidéo VP9 et de l'audio Opus. Sur un PC doté d'un encodeur GPU pour HEVC, elle enregistre aussi du HEVC en MP4. Les trois viennent d'une seule classe de sortie : FFMPEGEXEOutput. Voici l'application terminée, qui enregistre depuis une Logitech C920.

### Pourquoi FFmpeg

Pourquoi FFmpeg ? Le SDK capture la caméra et le micro, et transmet leurs images à FFmpeg. FFmpeg les encode et écrit le fichier. Une seule classe de sortie couvre donc de nombreux conteneurs et codecs. FFmpeg est fourni dans un package NuGet, il n'y a donc rien d'autre à installer. FFMPEGEXEOutput nécessite l'édition Premium du SDK, et la version d'essai le fait fonctionner pendant trente jours.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Et une webcam avec un micro. VideoCaptureCore est le moteur DirectShow du SDK : cette application ne fonctionne que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet WebcamFfmpeg. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant trois packages NuGet : le SDK lui-même, le redist de capture vidéo avec ses filtres DirectShow, et le redist FFmpeg. Ce dernier place FFmpeg à côté de votre application. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.Core et installez-le. Puis installez le redist de capture vidéo pour x64. Et le redist FFmpeg pour x64. Les deux redist sont ceux pour x64, réglez donc PlatformTarget sur x64 dans le fichier projet. Une application x86 prend à la place les redists x86.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'aperçu en direct. Ajoutez une étiquette et une liste déroulante pour la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour le format de la caméra, la fréquence d'images, le micro et la sortie. Ajoutez les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire, sur les listes de la caméra et du format, et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Les réglages FFmpeg se trouvent dans VisioForge.Core.Types.FFMPEGEXE. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu en direct s'affiche. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles s'affichent dans la barre de titre.

### Lister les périphériques

Video_CaptureDevices renvoie toutes les caméras, et Audio_CaptureDevices tous les micros. Nous listons leurs noms et sélectionnons le premier de chaque liste. Quand la caméra change, son gestionnaire remplit la liste des formats avec ceux de cette caméra, et choisit 1920x1080 MJPG s'il existe. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images, et choisit trente images par seconde.

### Lister les sorties

De retour dans le gestionnaire Load, la liste des sorties reçoit ses entrées. H.264 et WebM fonctionnent sur n'importe quel PC. HEVC a besoin d'un encodeur matériel : HWEncodersAvailable nous indique si un GPU Intel, NVIDIA ou AMD en possède un, et c'est seulement alors que HEVC figure dans la liste. Voici avec quoi le redist FFmpeg encode. H.264 utilise l'encodeur Media Foundation fourni avec Windows. VP9 et Opus sont des codecs ouverts pour WebM, encodés sur le CPU. AAC est l'audio du MP4. HEVC passe par l'encodeur Media Foundation du GPU, c'est pourquoi le code active ForceHWEncoding. Sur un PC qui n'en a pas, HEVC reste hors de la liste.

### Choisir le format de la caméra

Le gestionnaire de Start vérifie d'abord qu'une fréquence d'images est sélectionnée. Sinon, il l'indique dans la barre de titre. Puis il sélectionne la caméra par son nom, désactive Format_UseBest, et transmet le format et la fréquence d'images choisis dans les listes. Format_UseBest est activé par défaut. Il choisit la résolution la plus haute jusqu'à 1080p, puis préfère MJPG et la fréquence d'images la plus rapide. Pour enregistrer en 4K, ou dans le format de votre choix, désactivez-le et réglez Format et FrameRate, comme ici.

### Sélectionner le micro

Sans micro dans la liste, l'application enregistre la vidéo seule. Sinon, elle enregistre celui qui est sélectionné. Audio_PlayAudio est désactivé : le micro n'est pas joué dans les haut-parleurs pendant l'enregistrement.

### FFMPEGEXEOutput

Passons à la sortie. Un FFMPEGEXEOutput prend un muxer, c'est-à-dire le conteneur, des réglages vidéo et des réglages audio. Le cas par défaut est MP4. H264MFSettings choisit l'encodeur H.264, à huit mégabits par seconde constants, et l'audio est en AAC. Le cas HEVC est aussi du MP4. HEVCMFSettings avec ForceHWEncoding utilise l'encodeur du GPU. HEVC demande moins de bits pour la même image, d'où six mégabits par seconde ici. Pour WebM, le muxer est WebM. BasicVideoSettings choisit l'encodeur VP9 à huit mégabits par seconde constants, et BasicAudioSettings choisit Opus à 128 kilobits par seconde. Les deux cas MP4 partagent ce réglage AAC : 192 kilobits par seconde à débit constant. Quelques détails. Ici, Bitrate est en bits par seconde : huit millions signifie huit mégabits. SampleRate et Channels à zéro conservent le format propre du micro. Huit mégabits de vidéo font environ soixante mégaoctets par minute. Et l'extension du fichier doit correspondre au muxer : MP4 ou WebM.

### Démarrer et arrêter

Après le switch, le fichier va dans le dossier Vidéos, avec l'extension choisie par le cas. Le moteur passe en mode capture vidéo, pour enregistrer et pas seulement afficher l'aperçu. StartAsync rend la main dès que la capture tourne. StopAsync arrête la capture, et FFmpeg finit d'écrire le fichier.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait le fichier inachevé. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant l'enregistrement, il annule la fermeture. Puis il arrête l'enregistrement, et ferme de nouveau la fenêtre une fois le gestionnaire terminé. Quand rien n'est enregistré, le gestionnaire libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes de la caméra, du format et du micro se remplissent à partir des périphériques. Avec MP4 et H.264 sélectionnés, Start lance l'enregistrement. La C920 met quelques secondes à envoyer sa première image. Stop termine output.mp4. Choisissez ensuite WebM et enregistrez de nouveau. Stop termine output.webm, puis nous fermons la fenêtre.

### Vérifier les fichiers avec ffprobe

Les deux fichiers arrivent dans le dossier Vidéos. ffprobe montre ce qu'il contient. Le MP4 contient de la vidéo H.264 en 1920x1080 et de l'audio AAC à 48 kilohertz en stéréo. Le WebM contient de la vidéo VP9 en 1920x1080 et de l'audio Opus à 48 kilohertz en stéréo.

### Erreurs fréquentes

Quelques erreurs fréquentes. Le package redist FFmpeg manque, le SDK signale donc que vfffmpeg64.exe est introuvable. HEVC sur un PC sans encodeur GPU pour ce codec : vérifiez d'abord HWEncodersAvailable. Un débit écrit en kilobits : ici, ce sont des bits par seconde. Une licence Standard ou Professional : FFMPEGEXEOutput nécessite l'édition Premium. Et fermer la fenêtre en plein enregistrement : attendez d'abord StopAsync avec await.

### Étapes suivantes

Ensuite, la vidéo webcam vers MP4 enregistre du H.264 avec les encodeurs propres au SDK, sans FFmpeg, et la vidéo webcam vers WMV utilise les encodeurs Windows Media. Il existe aussi des guides pour les caméras IP et l'enregistrement d'écran. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.
