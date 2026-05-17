---
title: Capture d'écran en WMV avec codec Windows Media en C# .NET
description: Enregistrez l'écran au format WMV en C# avec les codecs Windows Media. Quand choisir WMV, configuration de codec et exemples C# complets avec VisioForge SDK.
sidebar_label: Capture d'écran en WMV
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - Screen Capture
  - MP4
  - AVI
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - WMVOutput
  - VideoCaptureCoreX
  - VideoView
  - IVideoView

---

# Implémenter l'enregistrement d'écran en WMV dans des applications C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutoriel vidéo pas à pas

Regardez notre tutoriel vidéo détaillé qui présente chaque étape du processus d'implémentation :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/8JYSDw2JeAo?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Accédez au code source complet de ce tutoriel sur notre dépôt GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-wmv)

## Dépendances requises

Avant de commencer, assurez-vous d'avoir installé les paquets redistribuables nécessaires :

- Redistribuables de capture vidéo :
  - [paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Quand utiliser WMV pour l'enregistrement d'écran

WMV (Windows Media Video) utilise les codecs Windows Media de Microsoft et le conteneur ASF. Il reste utile dans des scénarios spécifiques centrés sur Windows :

- **Intégration native Windows** — Les fichiers WMV se lisent dans Windows Media Player sans codecs supplémentaires et s'intègrent à Windows Movie Maker et à d'autres outils Microsoft
- **Streaming ASF** — Le conteneur ASF prend en charge la diffusion en direct via Windows Media Services, utile pour la diffusion sur intranet
- **Fichiers plus petits qu'AVI** — La compression WMV est plus efficace que MJPEG, bien que moins efficace que H.264 MP4
- **Environnements d'entreprise anciens** — De nombreux environnements d'entreprise standardisent les formats Windows Media pour la distribution interne de vidéos

**Compromis :** WMV est un format réservé à Windows avec une prise en charge limitée sur macOS et Linux. Pour la compatibilité multiplateforme, [MP4 est le format recommandé](screen-capture-mp4.md).

## API moderne — Video Capture SDK X

L'API moderne multiplateforme utilise `VideoCaptureCoreX`. Pour le modèle complet d'application console avec configuration de capture d'écran, configuration audio et cycle de vie d'enregistrement, consultez le guide [Capture d'écran en MP4](screen-capture-mp4.md#api-moderne-video-capture-sdk-x). Pour produire du WMV au lieu de MP4, remplacez la configuration de sortie :

```csharp
// Sortie WMV (codecs Windows Media Video + WMA audio)
var wmvOutput = new WMVOutput(outputPath);
videoCapture.Outputs_Add(wmvOutput, autostart: true);
```

La capture de région, l'enregistrement multi-écrans, l'audio, la mise en surbrillance du curseur et les options d'encodage GPU sont traités dans le [guide MP4](screen-capture-mp4.md) — toutes les fonctionnalités de configuration de la source fonctionnent à l'identique avec la sortie WMV.

## API héritée — Video Capture SDK

### Exemple de code

L'extrait de code suivant montre comment créer une application basique d'enregistrement d'écran qui capture votre écran dans un fichier WMV :

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
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_wmv
{
    public partial class Form1 : Form
    {
        // Déclarer l'objet VideoCaptureCore principal qui gérera l'enregistrement de l'écran
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            // Initialiser les composants du formulaire (boutons, panneaux, etc.)
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialiser l'instance VideoCaptureCore et l'associer au contrôle VideoView
            // VideoView1 est un contrôle d'interface où l'aperçu de la capture d'écran sera affiché
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurer la capture d'écran pour enregistrer l'écran complet
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { FullScreen = true };
            
            // Désactiver l'enregistrement et la lecture audio
            videoCapture1.Audio_RecordAudio = videoCapture1.Audio_PlayAudio = false;
            
            // Définir le chemin du fichier de sortie vers le dossier Vidéos de l'utilisateur
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
            
            // Définir le format de sortie comme WMV avec les paramètres par défaut
            videoCapture1.Output_Format = new WMVOutput();
            
            // Définir le mode de capture sur capture d'écran
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;
            
            // Démarrer le processus de capture d'écran de manière asynchrone
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Arrêter le processus de capture d'écran de manière asynchrone
            await videoCapture1.StopAsync();
        }
    }
}
```

## Questions fréquentes

### Quand dois-je utiliser WMV plutôt que MP4 pour l'enregistrement d'écran ?

Choisissez WMV lorsque votre public cible utilise exclusivement Windows et que vous avez besoin d'une lecture native sans installation de codecs supplémentaires, lorsque vous distribuez des vidéos via Windows Media Services ou SharePoint, ou lorsque vous travaillez dans des environnements d'entreprise qui standardisent les formats Windows Media. Pour la distribution multiplateforme ou la publication web, MP4 avec H.264 est le meilleur choix — il offre des fichiers plus petits, une prise en charge plus large des appareils et une meilleure compression.

## Voir aussi

- [Capture d'écran en MP4](screen-capture-mp4.md) — format recommandé avec couverture complète des fonctionnalités (région, multi-écrans, audio, encodage GPU, multiplateforme)
- [Capture d'écran en AVI](screen-capture-avi.md) — format AVI avec MJPEG pour l'édition image par image
- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — enregistrement d'écran en Visual Basic .NET
- [Configuration de source écran](../video-sources/screen.md) — référence complète des paramètres de capture
- [Exemples de code](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — extraits de code supplémentaires de capture d'écran sur GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
