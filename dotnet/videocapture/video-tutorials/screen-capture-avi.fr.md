---
title: Capture d'écran en AVI avec encodage MJPEG en C# .NET
description: Enregistrez l'écran au format AVI en C# avec vidéo MJPEG ou non compressée. Quand choisir AVI plutôt que MP4, options de codec et exemples complets.
sidebar_label: Capture d'écran en AVI
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - MJPEG
  - C#
  - NuGet
primary_api_classes:
  - AVIOutput
  - VideoCaptureCore
  - VideoCaptureCoreX
  - MJPEGEncoderSettings
  - VideoView

---

# Guide d'implémentation de capture d'écran en AVI en C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutoriel vidéo pas à pas

Regardez notre tutoriel détaillé qui présente le processus d'implémentation :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/AUT8oVPinUs?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Accédez au code source complet de ce tutoriel :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-avi)

## Quand utiliser AVI pour l'enregistrement d'écran

AVI (Audio Video Interleave) est un format de conteneur ancien qui reste utile dans des scénarios spécifiques :

- **Flux de travail d'édition basés sur DirectShow** — Les fichiers AVI s'intègrent parfaitement avec les filtres DirectShow et les outils d'édition vidéo plus anciens
- **Codec MJPEG** — Chaque image est compressée indépendamment, ce qui rend AVI idéal pour l'édition image par image où vous avez besoin d'un accès aléatoire à n'importe quelle image sans décoder les précédentes
- **Compatibilité maximale** — AVI est pris en charge par pratiquement tous les lecteurs et éditeurs vidéo sous Windows, y compris les applications anciennes
- **Structure de codec simple** — Contrairement au format complexe basé sur les atomes de MP4, AVI a une structure directe plus facile à récupérer à partir d'enregistrements incomplets

**Compromis :** Les fichiers AVI avec MJPEG sont nettement plus volumineux que les fichiers MP4 avec H.264. Un enregistrement 1080p à 25 FPS produit environ 150–200 Mo par minute avec MJPEG, contre environ 25 Mo par minute avec MP4 H.264.

Pour la plupart des cas d'usage d'enregistrement d'écran, [MP4 est le format recommandé](screen-capture-mp4.md). Utilisez AVI lorsque vous avez spécifiquement besoin de l'indépendance des images MJPEG, de la compatibilité DirectShow ou d'une capture non compressée.

## API moderne — Video Capture SDK X

L'API moderne multiplateforme utilise `VideoCaptureCoreX`. Pour le modèle complet d'application console avec configuration de capture d'écran, configuration audio et cycle de vie d'enregistrement, consultez le guide [Capture d'écran en MP4](screen-capture-mp4.md#api-moderne-video-capture-sdk-x). Pour produire de l'AVI au lieu de MP4, remplacez la configuration de sortie :

```csharp
// AVI avec codecs par défaut (vidéo OpenH264 + audio MP3)
var aviOutput = new AVIOutput(outputPath);
videoCapture.Outputs_Add(aviOutput, autostart: true);
```

### Options de codec AVI

Choisissez l'encodeur vidéo selon votre flux de travail :

```csharp
// MJPEG — indépendant par image, fichiers plus volumineux, idéal pour l'édition
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new MJPEGEncoderSettings();

// H.264 dans un conteneur AVI — fichiers plus petits, moins adapté à l'édition
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new OpenH264EncoderSettings();
```

La capture de région, l'enregistrement multi-écrans, l'audio, la mise en surbrillance du curseur et les options d'encodage GPU sont traités dans le [guide MP4](screen-capture-mp4.md) — toutes les fonctionnalités de configuration de la source fonctionnent à l'identique avec la sortie AVI.

## API héritée — Video Capture SDK

Cet exemple WPF montre la capture d'écran en AVI à l'aide de l'API héritée `VideoCaptureCore`. Ajoutez un contrôle `VideoView` nommé `VideoView1` à votre fenêtre XAML.

### Exemple de code

```csharp
using System;
using System.IO;
using System.Windows;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_avi
{
    public partial class MainWindow : Window
    {
        private VideoCaptureCore videoCapture1;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            // Capturer tout l'écran
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
            {
                FullScreen = true
            };

            // Vidéo seulement — sans audio
            videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Audio_PlayAudio = false;

            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                "output.avi");

            // Sortie AVI avec vidéo MJPEG et audio PCM
            videoCapture1.Output_Format = new AVIOutput();
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            await videoCapture1.StopAsync();
        }
    }
}
```

### Dépendances requises

Installez les paquets NuGet suivants :

- Composants redistribuables de capture vidéo :
  - [version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Questions fréquentes

### Pourquoi mon fichier d'enregistrement d'écran AVI est-il si volumineux ?

MJPEG compresse chaque image indépendamment, sacrifiant la taille du fichier pour faciliter l'édition (voir la comparaison de tailles ci-dessus). Pour réduire la taille du fichier : baissez la fréquence d'images (10–15 FPS suffit pour des présentations), capturez une zone plus petite plutôt que tout l'écran, ou passez à la [sortie MP4](screen-capture-mp4.md) qui utilise la compression inter-images H.264 et produit des fichiers environ 6 à 8 fois plus petits.

### Quand dois-je utiliser AVI plutôt que MP4 pour l'enregistrement d'écran ?

Choisissez AVI lorsque vous avez besoin d'un accès indépendant aux images pour l'édition vidéo (MJPEG permet de naviguer vers n'importe quelle image sans décoder les précédentes), lorsque vous travaillez avec des pipelines basés sur DirectShow anciens, ou lorsque vous avez besoin d'une compatibilité maximale avec les anciens outils vidéo Windows. Dans tous les autres cas, MP4 avec H.264 offre une meilleure compression, des fichiers plus petits et une prise en charge multiplateforme plus large.

## Voir aussi

- [Capture d'écran en MP4](screen-capture-mp4.md) — format recommandé avec couverture complète des fonctionnalités (région, multi-écrans, audio, encodage GPU, multiplateforme)
- [Capture d'écran en WMV](screen-capture-wmv.md) — alternative au format Windows Media
- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — enregistrement d'écran en Visual Basic .NET
- [Configuration de source écran](../video-sources/screen.md) — référence complète des paramètres de capture
- [Exemples de code](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — extraits de code supplémentaires de capture d'écran sur GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
