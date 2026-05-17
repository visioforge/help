---
title: Enregistrer la vidéo de la webcam en C# .NET — guide complet
description: Enregistrez la vidéo de la webcam en MP4 et WebM en C# avec VisioForge Video Capture SDK. Encodage GPU, énumération de périphériques, multiplateforme.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Webcam
  - IP Camera
  - Screen Capture
  - MP4
  - WebM
  - H.264
  - H.265
  - VP8
  - VP9
  - AV1
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - DeviceEnumerator
  - VideoCaptureDeviceSourceSettings
  - MP4Output
  - WebMOutput

---

# Enregistrer la vidéo de la webcam avec .Net : guide complet pour capturer la webcam

Capturer et enregistrer la vidéo des webcams est une exigence courante dans de nombreuses applications modernes, des outils de visioconférence aux systèmes de surveillance. Que vous ayez besoin d'enregistrer la vidéo de la webcam, d'afficher le flux de la webcam ou de capturer des images, implémenter une fonctionnalité de webcam fiable en .NET (DotNet) peut être un défi. Ce tutoriel fournit un guide simple étape par étape pour enregistrer la vidéo de la webcam à l'aide de C# (C Sharp) avec un minimum de code.

## Fonctionnalités clés de Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) est une bibliothèque puissante conçue spécifiquement pour les développeurs .NET qui doivent implémenter la fonctionnalité de capture de webcam dans leurs applications. Que vous souhaitiez enregistrer la vidéo de la webcam, sauvegarder les images de la webcam sous forme d'images ou afficher le flux de la webcam dans votre application, ce SDK répond à vos besoins. Parmi ses caractéristiques remarquables :

- Intégration transparente avec les applications .NET
- Prise en charge de plusieurs périphériques de capture (webcams USB, caméras IP, cartes de capture)
- Enregistrement et traitement vidéo et audio haute performance
- Code simple pour obtenir et afficher les flux de la webcam
- Création et enregistrement de fichiers vidéo dans divers formats
- Encodage accéléré par GPU pour des performances optimales
- Compatibilité multiplateforme

## Formats de sortie : MP4 et WebM en détail

### Format MP4

MP4 est l'un des formats de conteneurs vidéo les plus largement pris en charge, ce qui en fait un excellent choix pour les applications où la compatibilité est une priorité. Pour les options de configuration détaillées, consultez la [documentation du format MP4](../../general/output-formats/mp4.md).

Codecs pris en charge pour MP4 :

- **H.264 (AVC) :** la norme de l'industrie pour la compression vidéo, offrant une excellente qualité et une large compatibilité. Consultez la [documentation de l'encodeur H.264](../../general/video-encoders/h264.md).
- **H.265 (HEVC) :** codec de nouvelle génération offrant jusqu'à 50 % de meilleure compression que H.264 tout en conservant la même qualité. Consultez la [documentation de l'encodeur HEVC](../../general/video-encoders/hevc.md).
- **AAC :** Advanced Audio Coding, la norme de l'industrie pour la compression audio numérique avec perte.

### Format WebM

WebM est un format de fichier multimédia ouvert et libre de redevances conçu pour le web. Pour les options de configuration détaillées, consultez la [documentation du format WebM](../../general/output-formats/webm.md).

Codecs pris en charge pour WebM :

- **VP8 :** codec vidéo open source développé par Google, principalement utilisé avec le format WebM.
- **VP9 :** successeur de VP8, offrant une efficacité de compression nettement améliorée.
- **AV1 :** nouveau codec vidéo open source avec une compression et une qualité supérieures, en particulier à des débits binaires plus faibles.
- **Vorbis :** format de compression audio gratuit et open source offrant une bonne qualité à des débits binaires plus faibles.

Chaque codec peut être affiné avec divers paramètres pour atteindre l'équilibre optimal entre qualité et taille de fichier selon les exigences spécifiques de votre application.

## Accélération GPU pour un encodage efficace

L'une des caractéristiques remarquables de Video Capture SDK .Net est sa prise en charge robuste de l'encodage vidéo accéléré par GPU, qui offre plusieurs avantages significatifs :

### Avantages de l'accélération GPU

- **Utilisation CPU considérablement réduite :** libérez les ressources CPU pour d'autres tâches applicatives
- **Vitesses d'encodage plus rapides :** jusqu'à 10 fois plus rapide que l'encodage CPU uniquement
- **Encodage haute résolution en temps réel :** activez la capture vidéo 4K avec un impact système minimal
- **Consommation d'énergie réduite :** particulièrement important pour les applications mobiles et portables
- **Qualité supérieure au même débit binaire :** certains encodeurs GPU offrent une meilleure qualité par bit que l'encodage CPU

### Technologies GPU prises en charge

Video Capture SDK .Net exploite plusieurs technologies d'accélération GPU :

- **NVIDIA NVENC :** encodage accéléré par matériel sur les GPU NVIDIA
- **AMD AMF/VCE :** accélération sur les cartes graphiques AMD
- **Intel Quick Sync Video :** encodage matériel sur les graphismes intégrés Intel

Le SDK détecte automatiquement le matériel disponible et sélectionne le chemin d'encodage optimal en fonction des capacités de votre système, avec une solution de repli vers l'encodage logiciel si nécessaire.

## Exemple d'implémentation (code C# pour capturer la vidéo de la webcam)

Parcourons un tutoriel simple sur la façon d'enregistrer la vidéo de la webcam en utilisant C#. Implémenter la capture de webcam avec Video Capture SDK .Net est simple. Voici un exemple complet montrant comment obtenir le flux de votre webcam, l'afficher dans votre application et l'enregistrer dans un fichier MP4 avec encodage H.264 :

```csharp
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.AudioRenderers;

namespace video_capture_webcam_mp4
{
    public partial class Form1 : Form
    {
        // Objet Video Capture principal
        private VideoCaptureCoreX videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Créer une instance de VideoCaptureCoreX et définir VideoView pour le rendu vidéo
            videoCapture1 = new VideoCaptureCoreX(VideoView1);

            // Énumérer les sources vidéo et audio
            var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();
            var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();

            // Définir la source vidéo par défaut
            videoCapture1.Video_Source = new VideoCaptureDeviceSourceSettings(videoSources[0]);

            // Définir la source audio par défaut
            videoCapture1.Audio_Source = audioSources[0].CreateSourceSettingsVC();

            // Définir le puits audio par défaut
            var audioRenderers = await DeviceEnumerator.Shared.AudioOutputsAsync();
            videoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioRenderers[0]);
            videoCapture1.Audio_Play = true;

            // Configurer la sortie MP4. Les encodeurs vidéo et audio par défaut seront utilisés. 
            // Les encodeurs GPU seront utilisés s'ils sont disponibles.
            var mp4Output = new MP4Output(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4"));
            videoCapture1.Outputs_Add(mp4Output);

            videoCapture1.Audio_Record = true;

            // Démarrer la capture
            await videoCapture1.StartAsync();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Nous devons initialiser le moteur au démarrage
            Text += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
            this.Enabled = false;
            await VisioForgeX.InitSDKAsync();
            this.Enabled = true;
            Text = Text.Replace("[FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter la capture
            await videoCapture1.StopAsync();

            // Libérer les ressources
            await videoCapture1.DisposeAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Libérer le SDK
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Exemples de code supplémentaires

### Sortie WebM avec VP9

Pour une sortie WebM avec encodage VP9, modifiez simplement les paramètres de l'encodeur :

```csharp
using VisioForge.Core.Types.X.Output;

// Configurer la sortie WebM avec le codec VP9
var webmOutput = new WebMOutput(Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
    "output.webm"));
videoCapture1.Outputs_Add(webmOutput);
```

### Activer le capteur d'instantanés

Voici un exemple simple de la façon d'enregistrer une seule image depuis la webcam. Activez le capteur d'échantillons vidéo :

```csharp
// Activer le capteur d'instantanés avant de démarrer la capture
videoCapture1.Snapshot_Grabber_Enabled = true;
await videoCapture1.StartAsync();
```

### Enregistrer un instantané

Obtenir et enregistrer une seule image depuis la webcam :

```csharp
using SkiaSharp;

// Enregistrer l'image actuelle au format JPEG
await videoCapture1.Snapshot_SaveAsync(
    "snapshot.jpg", 
    SKEncodedImageFormat.Jpeg, 
    85);

// Ou enregistrer au format PNG
await videoCapture1.Snapshot_SaveAsync(
    "snapshot.png", 
    SKEncodedImageFormat.Png);
```

## Dépendances natives

Video Capture SDK .Net s'appuie sur des bibliothèques natives pour accéder aux périphériques webcam et effectuer le traitement vidéo et audio. Ces dépendances natives sont fournies avec le SDK et sont automatiquement déployées avec votre application, garantissant une intégration et une compatibilité transparentes entre différents systèmes.

### Référence du paquet

Paquet principal du SDK :

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
```

### Paquets redistribuables spécifiques aux plateformes

Windows x64 :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

Pour les autres plateformes :

```xml
<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

## Compatibilité multiplateforme

Video Capture SDK .Net est conçu en pensant à la compatibilité multiplateforme, ce qui en fait un choix idéal pour les développeurs travaillant sur des applications devant fonctionner sur plusieurs systèmes d'exploitation.

### Compatibilité MAUI

Pour les développeurs travaillant avec .NET MAUI (Multi-platform App UI), Video Capture SDK .Net offre :

- Compatibilité complète avec les applications MAUI
- API cohérente sur toutes les plateformes prises en charge
- Optimisations spécifiques à chaque plateforme tout en conservant une base de code unifiée
- Exemples de projets MAUI démontrant l'implémentation sur différentes plateformes

Cette capacité multiplateforme permet aux développeurs d'écrire le code une seule fois et de le déployer sur Windows, macOS et plateformes mobiles via MAUI, réduisant significativement le temps de développement et les coûts de maintenance.

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) fournit une solution complète pour ajouter des capacités de capture vidéo webcam à vos applications DotNet. Que vous deviez enregistrer des images de webcam, sauvegarder des images de webcam ou simplement afficher le flux de la webcam dans votre application, cette bibliothèque rend le processus simple avec seulement quelques lignes de code C#.

Avec la prise en charge de formats standard de l'industrie comme MP4 et WebM, de codecs modernes incluant H.264/H.265 et VP8/VP9/AV1, et une puissante accélération GPU, elle offre les performances et la flexibilité nécessaires pour les applications de capture vidéo les plus exigeantes. La capacité de créer et d'enregistrer efficacement des fichiers vidéo rend cette bibliothèque idéale pour toute application devant enregistrer du contenu webcam.

La compatibilité multiplateforme du SDK, étendue à macOS et aux applications MAUI, garantit que votre solution de capture webcam fonctionne de manière cohérente sur différents systèmes d'exploitation. Que vous construisiez un outil de visioconférence, une application de surveillance ou tout autre logiciel nécessitant une fonctionnalité de webcam, Video Capture SDK .Net offre les outils dont vous avez besoin pour implémenter rapidement ces fonctionnalités.

Pour commencer, il suffit de suivre le tutoriel étape par étape et les exemples de code fournis ci-dessus. Pour des cas d'utilisation plus avancés et une documentation détaillée sur la façon d'enregistrer la vidéo de la webcam en utilisant .NET, visitez notre site Web ou consultez la documentation du SDK.

## Foire aux questions

### Quel format dois-je utiliser pour enregistrer la vidéo de la webcam — MP4 ou WebM ?

MP4 avec encodage H.264 est le meilleur choix pour la plupart des applications car il offre une large compatibilité avec les appareils et les lecteurs, une compression efficace et un encodage accéléré par matériel sur la plupart des GPU. Choisissez WebM avec VP9 ou AV1 si vous avez besoin d'un format libre de redevances pour la diffusion web ou la lecture dans le navigateur. Les deux formats prennent en charge un enregistrement audio de haute qualité aux côtés de la vidéo.

### Comment définir la résolution et la fréquence d'images pour l'enregistrement webcam ?

Utilisez la classe `VideoCaptureDeviceSourceSettings` pour sélectionner une résolution et une fréquence d'images spécifiques parmi les formats pris en charge par votre caméra. Après avoir créé l'objet de paramètres de source, appelez `GetFormats()` pour énumérer les modes disponibles, puis assignez votre format préféré avant de démarrer la capture. Le SDK négocie automatiquement la correspondance la plus proche si le format exact n'est pas disponible.

### Puis-je utiliser l'accélération GPU pour enregistrer la vidéo de la webcam en C# ?

Oui. Video Capture SDK .Net détecte automatiquement le matériel GPU disponible — NVIDIA NVENC, AMD AMF/VCE et Intel Quick Sync Video — et sélectionne l'encodage accéléré par matériel lorsque vous créez un `MP4Output` ou un `WebMOutput`. Aucune configuration supplémentaire n'est requise. Si aucun GPU compatible n'est trouvé, le SDK bascule de manière transparente vers un encodage logiciel optimisé.

### Comment enregistrer la vidéo de la webcam avec l'audio ?

Définissez la propriété `Audio_Source` sur un microphone ou un autre périphérique d'entrée audio, puis activez l'enregistrement avec `Audio_Record = true`. Pour surveiller l'audio pendant la capture, définissez `Audio_Play = true` et assignez un `Audio_OutputDevice`. Lors de l'enregistrement au format MP4, l'audio est encodé en AAC par défaut ; WebM utilise Vorbis.

### L'enregistrement webcam fonctionne-t-il sur macOS et Linux ?

Oui. Le SDK est entièrement multiplateforme. Ajoutez les paquets NuGet redistribuables spécifiques à chaque plateforme (macOS, Linux x64) aux côtés du paquet principal `VisioForge.DotNet.VideoCapture`. Le même code C# fonctionne sur Windows, macOS et Linux, et le SDK prend également en charge .NET MAUI pour le déploiement mobile et multiplateforme sur ordinateur de bureau.

## Prise en charge de VB.NET

Vous cherchez l'enregistrement webcam en VB.NET ? Consultez notre guide dédié : [Enregistrer la vidéo de la webcam en VB.NET](record-webcam-vb-net.md).

## Voir aussi

- [Enregistrer la vidéo de la webcam en VB.NET](record-webcam-vb-net.md) — même fonctionnalité avec des exemples de code VB.NET
- [Capture d'écran vers MP4](../video-tutorials/screen-capture-mp4.md) — enregistrer l'écran du bureau au lieu de la webcam
- [Tutoriel webcam vers MP4](../video-tutorials/video-capture-webcam-mp4.md) — guide pas à pas d'enregistrement MP4
- [Capture caméra IP vers MP4](../video-tutorials/ip-camera-capture-mp4.md) — enregistrer depuis des caméras réseau au lieu de webcams locales
- [Lecteur de codes-barres et QR codes](../../mediablocks/Guides/barcode-qr-reader-guide.md) — combiner la capture webcam avec la détection de codes-barres
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
