---
title: Enregistrer une webcam en AVI en C# .NET — tutoriel
description: Implémentez l'enregistrement de webcam vers un fichier AVI en .NET avec un tutoriel pas à pas, exemple C# complet, mode asynchrone et sélection du périphérique.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - Screen Capture
  - AVI
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoView
  - IVideoView
  - VideoCaptureSource
  - AudioCaptureSource

---

# Enregistrement d'une webcam vers un fichier AVI dans des applications .NET C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel montre comment capturer la vidéo d'une webcam et l'enregistrer directement au format AVI dans vos applications .NET. L'implémentation utilise des techniques de programmation asynchrone pour une interface réactive et propose une architecture propre adaptée au développement logiciel professionnel.

## Tutoriel vidéo

Regardez notre tutoriel vidéo détaillé couvrant le processus d'implémentation :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/8yFKz1QOJbk?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Accédez au code source complet depuis notre dépôt GitHub officiel :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-avi)

## Dépendances requises

Avant d'implémenter cette solution, assurez-vous d'avoir installé les dépendances nécessaires :

- Redistribuables de capture vidéo :
  - [version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Détails d'implémentation

### Prérequis

Avant de commencer, assurez-vous d'avoir :

- Visual Studio avec les outils de développement .NET
- Une webcam connectée à votre machine de développement
- Une compréhension de base des applications Windows Forms

### Implémentation du code

L'exemple suivant montre une implémentation complète de la capture webcam vers un fichier AVI :

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Importer les espaces de noms du SDK VisioForge
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_avi
{
    public partial class Form1 : Form
    {
        // Déclarer l'objet VideoCaptureCore qui gérera toutes les opérations de capture
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            // Initialisation standard de Windows Forms
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser l'objet VideoCaptureCore et le lier au contrôle VideoView
            // VideoView1 est un contrôle qui affiche l'aperçu vidéo
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Définir le périphérique de capture vidéo — utiliser la première caméra disponible
            // videoCapture1.Video_CaptureDevices() retourne une liste des périphériques vidéo disponibles
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Définir le périphérique de capture audio — utiliser le premier microphone disponible
            // videoCapture1.Audio_CaptureDevices() retourne une liste des périphériques audio disponibles
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Définir le nom du fichier de sortie à « output.avi » dans le dossier Vidéos de l'utilisateur
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");

            // Configurer le format de sortie en AVI
            // Par défaut, cela utilise le codec MJPEG pour la vidéo et PCM pour l'audio
            videoCapture1.Output_Format = new AVIOutput();
            
            // Définir le mode sur VideoCapture (les autres modes incluent ScreenCapture, AudioCapture, etc.)
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Démarrer le processus de capture de manière asynchrone
            // Utiliser le motif async/await pour une interface non bloquante
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter le processus de capture de manière asynchrone
            await videoCapture1.StopAsync();
        }
    }
}
```

### Points clés d'implémentation

#### Sélection du périphérique

L'exemple sélectionne automatiquement les premiers périphériques vidéo et audio disponibles. Dans une application de production, vous voudrez peut-être présenter aux utilisateurs une liste de périphériques disponibles à sélectionner.

#### Configuration de la sortie de fichier

L'exemple enregistre la sortie dans un fichier AVI du dossier Vidéos de l'utilisateur. Vous pouvez personnaliser le chemin et le nom de fichier selon les besoins de votre application.

#### Opération asynchrone

L'implémentation utilise le motif async/await pour empêcher le gel de l'interface pendant les opérations de capture, ce qui est essentiel pour maintenir une expérience d'application réactive.

## Options de personnalisation avancée

Le SDK fournit de nombreuses options de personnalisation, notamment :

- Paramètres de résolution vidéo et de fréquence d'images
- Configuration de la qualité audio
- Sélection de codecs pour différents besoins de sortie
- Effets et transformations vidéo

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et explorer d'autres scénarios d'implémentation.
