---
title: Enregistrement d'écran en C# .NET — MP4, GPU, multi-écrans
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
  - MP4Output
  - ScreenCaptureSourceSettings
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCore
  - AudioCaptureSource

---

# Capture d'écran vers un fichier MP4

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fPJEoOz6lIM?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

## Redistribuables requis

- Redistribuables de capture vidéo [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuables MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## API héritée — Video Capture SDK

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

## API moderne — Video Capture SDK X

L'API moderne multiplateforme utilise `VideoCaptureCoreX` avec la capture d'écran Direct3D 11 et Windows Graphics Capture (WGC). Cette application console enregistre tout l'écran en MP4 avec l'audio système optionnel.

### Paquets NuGet requis

```bash
dotnet add package VisioForge.DotNet.Core.TRIAL
dotnet add package VisioForge.DotNet.VideoCapture.TRIAL
```

Ajoutez le [paquet redistribuable](../../deployment-x/index.md) pour votre plateforme (par exemple, `VisioForge.DotNet.Redist.Base.Windows.x64`).

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
            // Configurer la capture d'écran Direct3D 11 avec WGC
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

Sous Windows, le SDK sélectionne automatiquement WGC lorsqu'il est disponible, en se rabattant sur DXGI Desktop Duplication sur les systèmes plus anciens. Vous pouvez forcer une API spécifique :

```csharp
var screenSource = new ScreenCaptureD3D11SourceSettings
{
    API = D3D11ScreenCaptureAPI.DXGI  // Forcer Desktop Duplication au lieu de WGC
};
```

## Comment ça fonctionne — API héritée

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

### Configuration audio

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

### Capture d'une région

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

### Enregistrement multi-écrans

Sélectionnez l'écran à enregistrer à l'aide de la propriété `DisplayIndex`. L'écran principal porte l'index `0`, le secondaire `1`, et ainsi de suite :

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    DisplayIndex = 1  // Enregistrer l'écran secondaire
};
```

### Paramètres de qualité d'enregistrement

Personnalisez la sortie MP4 en configurant l'encodeur H.264. Le débit binaire par défaut est de 3500 kbps, ce qui produit environ 25 Mo par minute en 1080p :

```csharp
var mp4Output = new MP4Output();
mp4Output.Video.Bitrate = 5000;                          // Qualité supérieure (kbps)
mp4Output.Video.Profile = H264Profile.ProfileMain;       // Meilleure compression que Baseline
mp4Output.Video.RateControl = H264RateControl.VBR;       // Débit variable
videoCapture1.Output_Format = mp4Output;
```

Pour l'encodage accéléré par GPU, consultez la section [Encodage accéléré par GPU](#encodage-accelere-par-gpu) ci-dessus.

### Options du curseur de la souris

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

## Questions fréquentes

### Quelle licence me faut-il pour une application de capture d'écran en C# ?

Video Capture SDK .Net nécessite une licence pour le développement et la distribution. Une licence Développeur supprime le filigrane d'évaluation et débloque toutes les fonctionnalités pendant le développement. Une licence Release est nécessaire lors de la distribution de votre application aux utilisateurs finaux. Le SDK est disponible en édition Premium, qui inclut tous les modes de capture, les sorties MP4/AVI/WMV et les moteurs DirectShow et GStreamer. Vous pouvez évaluer le SDK sans licence — la capture d'écran fonctionne entièrement mais inclut un filigrane superposé. Visitez la [page produit](https://www.visioforge.com/video-capture-sdk-net) pour les tarifs et options de licence.

### Comment contrôler la qualité d'enregistrement et la taille du fichier pour la capture d'écran MP4 ?

Configurez le débit binaire vidéo de `MP4Output` pour équilibrer qualité et taille de fichier. La valeur par défaut est de 3500 kbps, qui fonctionne bien pour la plupart des enregistrements d'écran. Baissez le débit à 1500–2000 kbps pour des fichiers plus petits, ou augmentez à 5000–8000 kbps pour des captures de haute qualité. Utilisez `H264Profile.ProfileMain` ou `ProfileHigh` au lieu de `ProfileBaseline` pour une meilleure compression à qualité égale. La fréquence d'images influence également la taille du fichier — 15 FPS suffisent pour les présentations, tandis que 30 FPS convient mieux aux démos logicielles. Un enregistrement 1080p à 3500 kbps produit environ 25 Mo par minute.

### Puis-je capturer l'audio système (son du bureau) au lieu de l'entrée microphone ?

Oui. Appelez `Audio_CaptureDevices()` pour énumérer tous les périphériques audio disponibles, puis sélectionnez un périphérique loopback ou audio système (comme « Stereo Mix » ou « What U Hear ») par son nom. Définissez `Audio_RecordAudio = true` et `Audio_PlayAudio = false`. Les noms des périphériques loopback disponibles dépendent de votre matériel audio et de vos pilotes. Vous pouvez enregistrer simultanément le microphone et l'audio système en configurant des sources audio supplémentaires.

### Comment enregistrer un écran spécifique dans une configuration multi-écrans ?

Définissez la propriété `DisplayIndex` sur `ScreenCaptureSourceSettings` — utilisez `0` pour l'écran principal, `1` pour le secondaire, et ainsi de suite. Combinez avec `FullScreen = true` pour capturer tout l'écran sélectionné. Vous pouvez également définir `FullScreen = false` avec les coordonnées `Left/Top/Right/Bottom` pour capturer une région spécifique sur l'écran choisi.

### Quelle fréquence d'images dois-je utiliser pour l'enregistrement d'écran ?

La fréquence d'images par défaut est de 10 FPS. Utilisez 15 FPS pour les présentations, les diapositives et le contenu majoritairement statique. Utilisez 25–30 FPS pour les tutoriels logiciels et les démonstrations d'interface où le mouvement fluide de la souris importe. Utilisez 60 FPS pour l'enregistrement de jeux ou le contenu très animé. Des fréquences d'images plus élevées augmentent proportionnellement la taille du fichier et l'utilisation du CPU. Définissez la fréquence d'images via `ScreenCaptureSourceSettings.FrameRate`.

## Voir aussi

- [Capture d'écran en AVI](screen-capture-avi.md) — enregistrer l'écran au format AVI avec vidéo non compressée ou MJPEG
- [Capture d'écran en WMV](screen-capture-wmv.md) — enregistrer l'écran au format Windows Media
- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — application d'enregistrement d'écran en Visual Basic .NET
- [Configuration de source écran](../video-sources/screen.md) — référence complète pour la région de capture, le multi-écrans, le curseur et la fréquence d'images
- [Enregistrer la vidéo de webcam](../guides/save-webcam-video.md) — capture de webcam en MP4 avec configuration audio
- [Exemples de code](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — extraits de code supplémentaires de capture d'écran sur GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
