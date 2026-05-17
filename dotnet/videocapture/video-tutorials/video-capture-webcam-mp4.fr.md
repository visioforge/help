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
  - MP4Output
  - VideoView

---

# Implémentation de capture vidéo de webcam en MP4 en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la capture de webcam dans les applications .NET

Créer des applications qui enregistrent la vidéo depuis des webcams vers des fichiers MP4 est un besoin courant dans de nombreux projets logiciels. Ce tutoriel propose une approche orientée développeurs pour implémenter cette fonctionnalité à l'aide de techniques modernes .NET et de Video Capture SDK.

Lors de l'implémentation des capacités d'enregistrement de webcam, les développeurs doivent prendre en compte :

- La sélection des périphériques d'entrée vidéo et audio appropriés
- La configuration des paramètres de compression vidéo et audio
- La gestion du cycle de vie de la capture (initialisation, démarrage, arrêt)
- La gestion de la création et de l'organisation du fichier de sortie

## Tutoriel vidéo d'implémentation

La vidéo suivante illustre le processus complet de mise en place d'une application de capture de webcam :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/TunCZ_2bNr8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Pour les développeurs qui préfèrent examiner l'implémentation complète, tout le code source est disponible dans notre dépôt GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-mp4)

## Composants redistribuables requis

Avant d'implémenter la solution, assurez-vous d'avoir installé les paquets redistribuables nécessaires :

### Composants de capture vidéo

- Pour les applications 32 bits : [Redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Pour les applications 64 bits : [Redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Composants d'encodage MP4

- Pour les applications 32 bits : [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- Pour les applications 64 bits : [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Exemple d'implémentation avec code C#

L'implémentation C# suivante montre comment créer une application Windows Forms qui capture la vidéo d'une webcam et l'enregistre dans un fichier MP4. Le code inclut une initialisation, une configuration et une gestion des ressources appropriées.

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

// Importer les bibliothèques VisioForge pour la fonctionnalité de capture vidéo
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_mp4
{
    public partial class Form1 : Form
    {
        // L'objet de capture vidéo principal qui contrôle le processus de capture
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gestionnaire de clic du bouton Démarrer — configure et lance la capture vidéo
        /// </summary>
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Sélectionner le premier périphérique vidéo disponible (webcam) du système
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Sélectionner le premier périphérique audio disponible (microphone) du système
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Définir le chemin du fichier de sortie vers le dossier Vidéos de l'utilisateur avec « output.mp4 » comme nom
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            
            // Configurer le format de sortie en MP4 avec les paramètres par défaut (vidéo H.264, audio AAC)
            videoCapture1.Output_Format = new MP4Output();
            
            // Définir le mode sur VideoCapture pour capturer à la fois la vidéo et l'audio
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Démarrer le processus de capture de manière asynchrone
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Gestionnaire de chargement du formulaire — initialise le composant de capture vidéo
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser l'objet VideoCaptureCore en le connectant au contrôle VideoView du formulaire
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        /// <summary>
        /// Gestionnaire de clic du bouton Arrêter — arrête la capture en cours
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter le processus de capture de manière asynchrone et finaliser le fichier de sortie
            await videoCapture1.StopAsync();
        }
    }
}
```

## Fonctionnalités clés de l'implémentation

Cette implémentation offre plusieurs capacités importantes pour les développeurs :

1. **Sélection automatique des périphériques** — Le code sélectionne automatiquement les premiers périphériques vidéo et audio disponibles
2. **Format de sortie standard** — Configure la sortie MP4 avec les codecs standard de l'industrie : vidéo H.264 et audio AAC
3. **Opération asynchrone** — Utilise le motif async/await pour une interface non bloquante pendant les opérations de capture
4. **Intégration simple** — Facile à intégrer dans des applications Windows Forms existantes

## Options de configuration avancée

Bien que l'exemple présente une implémentation de base, les développeurs peuvent étendre la solution avec des fonctionnalités supplémentaires :

- Paramètres personnalisés de résolution vidéo et de fréquence d'images
- Ajustements de débit binaire pour le contrôle de la qualité
- Effets et transformations vidéo à la volée
- Prise en charge de plusieurs pistes audio

## Voir aussi

- [Enregistrement pré-événement](../guides/pre-event-recording.md) — enregistrement avec tampon circulaire et déclencheurs de détection de mouvement pour webcams et caméras IP
- [Aperçu vidéo de caméra](camera-video-preview.md) — afficher la vidéo de caméra en direct avec capture d'images

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et d'exemples d'implémentation.
