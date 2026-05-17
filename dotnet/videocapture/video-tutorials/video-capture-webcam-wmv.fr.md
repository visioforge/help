---
title: Enregistrer une webcam en WMV avec Windows Media en C# .NET
description: Capturez la vidéo d'une webcam et enregistrez-la au format WMV avec VisioForge Video Capture SDK. Configuration de l'encodeur Windows Media et exemples C#.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Webcam
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - WMVOutput
  - VideoView

---

# Capture vidéo de webcam au format WMV en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutoriel vidéo d'implémentation

Ce tutoriel montre comment créer une application Windows Forms qui capture la vidéo d'une webcam et l'enregistre au format WMV à l'aide de Video Capture SDK .NET. L'exemple ci-dessous fournit une présentation complète avec un code source intégralement commenté.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Bqss-zdalXg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Accès au code source

Accédez au projet complet avec tous les fichiers sources et exemples supplémentaires sur notre dépôt GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-wmv)

## Dépendances requises

Avant d'implémenter le code, assurez-vous d'avoir installé les paquets redistribuables nécessaires via NuGet :

- **Redistribuables de capture vidéo :**
  - [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Étapes d'implémentation

Les sections suivantes décrivent les étapes clés pour implémenter la fonctionnalité de capture de webcam dans votre application .NET.

### Configuration de la structure du projet

Tout d'abord, créez une application Windows Forms et ajoutez les références du SDK via NuGet. Implémentez ensuite le formulaire avec les contrôles nécessaires, y compris une zone d'aperçu vidéo et des boutons démarrage/arrêt.

### Code d'implémentation C#

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_wmv
{
    public partial class Form1 : Form
    {
        // Instance principale du composant VideoCapture
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurer le périphérique de capture vidéo par défaut (webcam)
            // En utilisant le premier périphérique disponible dans le système
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configurer le périphérique de capture audio par défaut (microphone)
            // En utilisant le premier périphérique audio disponible dans le système
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Définir le chemin du fichier de sortie vers le dossier Vidéos de l'utilisateur
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");

            // Configurer le format de sortie en WMV avec les paramètres par défaut
            // WMV est un bon choix pour la compatibilité Windows
            videoCapture1.Output_Format = new WMVOutput();
            
            // Définir le mode de capture sur VideoCapture (capture à la fois la vidéo et l'audio)
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Démarrer le processus de capture de manière asynchrone
            // Cela permet à l'interface de rester réactive pendant la capture
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter le processus de capture de manière asynchrone
            // Cela garantit un nettoyage approprié des ressources
            await videoCapture1.StopAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser le VideoCaptureCore lors du chargement du formulaire
            // Le connecter au contrôle VideoView du formulaire pour l'aperçu
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

### Détails clés d'implémentation

1. **Initialisation du composant** : Le `VideoCaptureCore` est initialisé lorsque le formulaire se charge, en se connectant au contrôle d'aperçu vidéo.

2. **Sélection du périphérique** : Le code sélectionne automatiquement la première webcam et le premier microphone disponibles sur le système.

3. **Configuration de la sortie** : Le format WMV est sélectionné pour sa compatibilité avec les systèmes Windows et sa large prise en charge de plateformes.

4. **Gestion des ressources** : La méthode `StopAsync()` garantit un nettoyage approprié des ressources système lorsque l'enregistrement se termine.

## Options de personnalisation avancée

Le SDK propose des options supplémentaires non présentées dans cet exemple de base :

- Paramètres de résolution et de fréquence d'images de la caméra
- Paramètres de qualité vidéo et de compression
- Filigranes et superpositions personnalisés
- Capacités de capture multi-périphériques

## Conseils de dépannage

- Assurez-vous que votre webcam est correctement connectée et reconnue par Windows
- Vérifiez que les redistribuables corrects sont installés pour votre plateforme cible (x86/x64)
- Vérifiez les autorisations Windows pour l'accès à la caméra sur les systèmes d'exploitation plus récents

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et d'exemples d'implémentation avancée.
