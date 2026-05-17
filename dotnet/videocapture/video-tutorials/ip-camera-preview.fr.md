---
title: Aperçu en direct d'une caméra IP en C# .NET — Tutoriel RTSP
description: Aperçu temps réel d'une caméra IP en applications .NET — tutoriel pas à pas et exemples C# complets pour WinForms, WPF, Console.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IPCameraSourceSettings
  - IPSourceEngine
  - VideoCaptureMode
  - IVideoView

---

# Guide d'implémentation de l'aperçu d'une caméra IP

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutoriel vidéo

Ce tutoriel montre comment configurer la fonctionnalité d'aperçu de caméra IP dans vos applications .NET :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/9n44ChQJT7s?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview)

## Redistribuables requis

Avant de commencer, assurez-vous d'avoir installé les paquets suivants :

- Redistribuables de capture vidéo [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuables LAV [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Exemple d'implémentation

Vous trouverez ci-dessous un exemple WinForms complet montrant comment intégrer la fonctionnalité d'aperçu de caméra IP :

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace ip_camera_preview
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Plusieurs moteurs sont disponibles. Nous utiliserons LAV, le plus compatible. Pour une lecture RTSP en faible latence, utilisez le moteur RTSP Low Latency.
            videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
            {
                URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
                Type = IPSourceEngine.Auto_LAV
            };

            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Mode = VideoCaptureMode.IPPreview;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();
        }
    }
}
```

## Détails clés d'implémentation

### Configuration de la source de caméra IP

Le code montre comment configurer la source de caméra IP avec l'URL et le type de moteur appropriés :

```csharp
videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
{
    URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
    Type = IPSourceEngine.Auto_LAV
};
```

### Gestion des paramètres audio

Pour les applications d'aperçu simples, vous pouvez désactiver la lecture et l'enregistrement audio :

```csharp
videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
```

### Définition du mode de capture

Le mode correct pour l'aperçu de caméra IP est :

```csharp
videoCapture1.Mode = VideoCaptureMode.IPPreview;
```

## Options avancées

Pour les applications de production, envisagez d'implémenter :

- Gestion des erreurs et logique de réessai de connexion
- Retour visuel dans l'interface pendant les tentatives de connexion
- Gestion de l'authentification de la caméra
- Contrôle de la fréquence d'images et de la résolution

## Guides associés

- [Configuration de source de caméra RTSP](../video-sources/ip-cameras/rtsp.md) — transport UDP/TCP, réglage basse latence, sélection du moteur
- [Intégration de caméras IP ONVIF](../video-sources/ip-cameras/onvif.md) — auto-découverte, sélection de profil, contrôle PTZ
- [Enregistrer le flux RTSP en MP4](ip-camera-capture-mp4.md) — capturer vers un fichier le flux visualisé

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour explorer plus d'exemples de code. Besoin de l'URL RTSP de votre caméra ? Consultez notre [répertoire des marques de caméras IP](../../camera-brands/index.md).
