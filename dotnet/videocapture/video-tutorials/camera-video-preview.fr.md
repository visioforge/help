---
title: Aperçu webcam et capture d'images en C# .NET — WinForms/WPF
description: Affichez le flux webcam en direct et capturez des images individuelles avec VisioForge Video Capture SDK. Code C# fonctionnel pour WinForms, WPF et console.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoView
  - IVideoView
  - VideoCaptureSource
  - AudioCaptureSource

---

# Implémenter l'aperçu vidéo webcam avec capture d'images en C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel montre comment créer une application professionnelle d'aperçu vidéo webcam avec la capacité de capturer des images individuelles. Cette fonctionnalité est essentielle pour développer des applications nécessitant une analyse d'image, des capacités d'instantané ou des interfaces de caméra personnalisées.

## Tutoriel vidéo étape par étape

Regardez notre vidéo détaillée qui couvre tous les aspects de l'intégration webcam :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/kxC6JrJddek?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Prise en main

Avant de plonger dans le code, vous devez configurer votre environnement de développement avec les dépendances nécessaires. Le code source complet est disponible sur GitHub pour référence :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-preview-webcam-frame-capture)

## Guide d'implémentation

### Dépendances requises

Tout d'abord, assurez-vous d'avoir les paquets NuGet suivants installés :

- Redistribuables Video Capture [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Exemple de code source

L'implémentation C# suivante montre comment :

1. Initialiser un composant de capture vidéo
2. Se connecter à un périphérique webcam
3. Afficher un aperçu vidéo en temps réel
4. Capturer et sauvegarder des images individuelles en JPEG

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Références SDK VisioForge pour la fonctionnalité de capture vidéo
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_preview_webcam_frame_capture
{
    public partial class Form1 : Form
    {
        // Composant principal de capture vidéo pour gérer l'entrée caméra
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        // Initialisation du formulaire — crée une nouvelle instance de VideoCaptureCore liée à notre vue vidéo
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser le composant de capture vidéo avec le contrôle VideoView
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        // Gestionnaire de clic du bouton Start — configure et démarre la capture vidéo
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurer le périphérique de capture vidéo par défaut (première caméra disponible)
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configurer le périphérique de capture audio par défaut (premier microphone disponible)
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Définir le mode sur VideoPreview (affichage uniquement, sans enregistrement)
            videoCapture1.Mode = VideoCaptureMode.VideoPreview;

            // Démarrer l'aperçu vidéo de manière asynchrone
            await videoCapture1.StartAsync();
        }

        // Gestionnaire de clic du bouton Stop — arrête la capture vidéo
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter l'aperçu vidéo de manière asynchrone
            await videoCapture1.StopAsync();
        }

        // Gestionnaire de clic du bouton Save Frame — capture et sauvegarde l'image actuelle en JPEG
        private async void btSaveFrame_Click(object sender, EventArgs e)
        {
            // Sauvegarder l'image actuelle en JPEG dans le dossier Images de l'utilisateur
            // Le paramètre de qualité (85) spécifie le niveau de compression JPEG (0-100)
            await videoCapture1.Frame_SaveAsync(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "frame.jpg"),
                ImageFormat.Jpeg,
                85);
        }
    }
}
```

## Décomposition du code

### Initialisation

L'application commence par créer une nouvelle instance de `VideoCaptureCore` au chargement du formulaire, qui sert d'interface principale pour interagir avec les périphériques webcam.

### Sélection du périphérique

Lorsque l'utilisateur clique sur le bouton Start, l'application sélectionne automatiquement les premiers périphériques de capture vidéo et audio disponibles. Dans un environnement de production, vous voudrez peut-être proposer un menu déroulant de sélection pour les utilisateurs disposant de plusieurs caméras.

### Mode aperçu

L'application est configurée en mode `VideoPreview`, qui active l'affichage en temps réel sans enregistrement du flux sur disque. C'est idéal pour les applications qui n'ont besoin que d'afficher la sortie de la caméra.

### Capture d'image

La fonctionnalité de capture d'image montre comment sauvegarder l'image vidéo actuelle en JPEG avec un niveau de qualité spécifié. L'image est sauvegardée par défaut dans le dossier Images de l'utilisateur.

## Applications avancées

Ce code peut être étendu pour prendre en charge diverses applications du monde réel :

- Numérisation de documents
- Applications de photomaton
- Visioconférence
- Vision par ordinateur et traitement d'image
- Systèmes de sécurité et de vidéosurveillance

## Ressources supplémentaires

- [Enregistrement pré-événement](../guides/pre-event-recording.md) — mettre en tampon la vidéo de la caméra et sauvegarder des clips à la détection de mouvement ou sur déclencheur manuel
- [Capture webcam vers MP4](video-capture-webcam-mp4.md) — enregistrer la vidéo webcam dans un fichier MP4

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour explorer d'autres exemples de code et techniques d'implémentation avancées.
