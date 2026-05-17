---
title: Capture webcam avec superposition de texte en C# .NET
description: Capturez la vidéo d'une webcam et ajoutez des superpositions de texte personnalisées en C# .NET avec exemples de code complets et techniques.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Playback
  - Webcam
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IVideoView
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4Output

---

# Ajouter des superpositions de texte à la capture vidéo d'une webcam en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutoriel vidéo pas à pas

La vidéo suivante montre le processus de capture vidéo depuis une webcam et d'ajout de superpositions de texte à l'aide de C# et .NET :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/D_JPo9A9HMA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Accès au code source

Accédez au code source complet de cette implémentation sur notre dépôt GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-text-overlay)

## Dépendances requises

Pour implémenter cette fonctionnalité dans votre application, vous devrez installer les paquets NuGet suivants :

- **Dépendances de capture vidéo :**
  - [version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
  
- **Dépendances de traitement MP4 :**
  - [version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  - [version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Vue d'ensemble de l'implémentation

Ce tutoriel montre comment :

- Accéder à une webcam connectée et en capturer la vidéo
- Traiter le flux vidéo en temps réel
- Ajouter des superpositions de texte personnalisables avec couleur et positionnement
- Enregistrer la vidéo capturée avec superposition de texte dans un fichier MP4

L'implémentation utilise Windows Forms pour l'interface utilisateur, mais la fonctionnalité principale de capture et de superposition de texte fonctionne avec n'importe quel framework d'interface .NET.

## Implémentation C# complète

L'exemple de code suivant montre une implémentation complète de capture de webcam avec la fonctionnalité de superposition de texte :

```csharp
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;

namespace video_capture_text_overlay
{
    public partial class Form1 : Form
    {
        // Le composant principal de capture vidéo qui gère toute la fonctionnalité de capture
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Démarre la capture vidéo avec superposition de texte
        /// </summary>
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurer le périphérique de capture vidéo — utilise la première caméra disponible
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configurer le périphérique de capture audio — utilise le premier microphone disponible
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Définir le mode de capture sur capture vidéo standard
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;
            
            // Définir le nom du fichier de sortie pour l'enregistrer dans le dossier Vidéos de l'utilisateur
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            
            // Configurer MP4 comme format de sortie — prend également en charge d'autres formats
            videoCapture1.Output_Format = new MP4Output();

            // Ajouter une superposition de texte à la vidéo
            // Étape 1 : Activer les effets vidéo
            videoCapture1.Video_Effects_Enabled = true;
            
            // Étape 2 : Effacer les effets existants
            videoCapture1.Video_Effects_Clear();
            
            // Étape 3 : Créer un nouvel effet de superposition de texte
            var textOverlay = new VideoEffectTextLogo(true) { 
                Text = "Hello World!",  // Le texte à afficher
                Top = 50,               // Position Y (depuis le haut)
                Left = 50,              // Position X (depuis la gauche)
                FontColor = Color.Red   // Couleur du texte
            };
            
            // Étape 4 : Ajouter la superposition de texte à la collection d'effets
            videoCapture1.Video_Effects_Add(textOverlay);

            // Démarrer le processus de capture de manière asynchrone
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Arrête la capture vidéo
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter le processus de capture de manière asynchrone
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Initialise le composant de capture vidéo au chargement du formulaire
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser le VideoCaptureCore avec le contrôle de vue vidéo
            // VideoView1 doit être un contrôle de votre formulaire qui implémente IVideoView
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
            
            // Options d'initialisation supplémentaires :
            // videoCapture1.Audio_PlayAudio = true;  // Lire l'audio pendant la capture
            // videoCapture1.Audio_RecordAudio = true;  // Enregistrer l'audio avec la vidéo
        }
    }
}
```

## Détails clés d'implémentation

### Sélection et configuration du périphérique

Le code sélectionne automatiquement la première caméra et le premier microphone disponibles pour la capture vidéo et audio. Dans un environnement de production, vous voudrez peut-être proposer aux utilisateurs des options pour sélectionner des périphériques spécifiques.

### Propriétés de superposition de texte

L'implémentation de superposition de texte prend en charge plusieurs options de personnalisation :

- **Contenu du texte** : N'importe quelle chaîne peut être affichée sur la vidéo
- **Position** : Spécifiez les coordonnées exactes pour le placement du texte
- **Couleur** : Choisissez n'importe quelle couleur pour l'affichage du texte
- **Taille et style** : Personnalisez la police, la taille et le style (options commentées)

### Opération asynchrone

L'implémentation utilise le motif async/await de C# pour des opérations vidéo non bloquantes, garantissant que votre application reste réactive pendant la capture.

### Sortie de fichier

La vidéo capturée avec superposition de texte est enregistrée sous forme de fichier MP4 dans le dossier Vidéos de l'utilisateur. Le format et l'emplacement de sortie peuvent être personnalisés selon les besoins de votre application.

## Ressources supplémentaires

Pour plus d'exemples et d'extraits de code liés au traitement et à la manipulation vidéo, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
