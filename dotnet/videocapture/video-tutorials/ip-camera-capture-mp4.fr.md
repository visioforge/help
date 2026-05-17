---
title: Enregistrer une caméra IP RTSP en MP4 en C# .NET — Exemple
description: Enregistrez des flux RTSP de caméras IP vers des fichiers MP4 en C# / .NET. Authentification, découverte ONVIF, reconnexion, encodage GPU. Exemple complet.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - MP4Output
  - DeviceEnumerator
  - AudioRendererSettings

---

# Capturer les flux de caméras IP vers des fichiers MP4 en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à l'enregistrement de caméras IP

Les caméras IP offrent de puissantes capacités de surveillance et de monitoring via des connexions réseau. Ce guide montre comment exploiter ces appareils dans vos applications .NET pour capturer et enregistrer des flux vidéo vers des fichiers MP4. En suivant des pratiques modernes de programmation C#, vous apprendrez à établir des connexions vers des caméras IP, à configurer les flux vidéo et à enregistrer des captures de haute qualité pour diverses applications : systèmes de sécurité, solutions de monitoring et outils d'archivage vidéo.

## Tutoriel vidéo d'implémentation

La vidéo suivante illustre le processus d'implémentation complet :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/qX3AiGyWbO8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-capture-mp4)

## Guide d'implémentation pas à pas

Ce guide montre comment établir des connexions vers des caméras IP, diffuser leur contenu vidéo et l'encoder directement vers des fichiers MP4 à l'aide du composant VideoCaptureCoreX. L'implémentation prend en charge plusieurs protocoles de caméra, dont RTSP, HTTP et ONVIF.

### Implémentation du code

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

using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core;

namespace ip_camera_capture_mp4
{
    public partial class Form1 : Form
    {
        private VideoCaptureCoreX videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCoreX(VideoView1);

            // Source de caméra RTSP
            var rtsp = await RTSPSourceSettings.CreateAsync(new Uri(edURL.Text), edLogin.Text, edPassword.Text, cbAudioStream.Checked);
            videoCapture1.Video_Source = rtsp;

            // Périphérique de sortie audio
            var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound))[0];
            videoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

            // Configurer la sortie MP4
            var mp4Output = new MP4Output(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4"));
            videoCapture1.Outputs_Add(mp4Output);

            // Démarrer
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();

            await videoCapture1.DisposeAsync();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await VisioForgeX.InitSDKAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Détails d'implémentation et bonnes pratiques

### Initialisation du SDK

Avant de travailler avec des caméras IP, une initialisation correcte du SDK est cruciale pour s'assurer que tous les composants sont chargés et prêts :

```csharp
private async void Form1_Load(object sender, EventArgs e)
{
    await VisioForgeX.InitSDKAsync();
}
```

Initialisez toujours le SDK au démarrage de l'application pour vous assurer que tous les composants requis sont correctement chargés avant de tenter une connexion caméra.

### Configuration de la connexion caméra

L'exemple utilise RTSP (Real-Time Streaming Protocol), l'un des protocoles les plus courants pour le streaming de caméras IP :

```csharp
// Source de caméra RTSP
var rtsp = await RTSPSourceSettings.CreateAsync(new Uri(edURL.Text), edLogin.Text, edPassword.Text, cbAudioStream.Checked);
videoCapture1.Video_Source = rtsp;
```

Lors de la connexion à des caméras IP, prenez en compte ces paramètres importants :

- **URL de la caméra** — L'URL RTSP complète de votre caméra (par exemple, `rtsp://192.168.1.100:554/stream1`)
- **Authentification** — De nombreuses caméras requièrent un nom d'utilisateur et un mot de passe
- **Flux audio** — Choisissez d'inclure ou non l'audio de la caméra
- **Délai de connexion** — Pour les applications de production, implémentez une gestion appropriée du délai d'attente

### Configuration de la sortie MP4

MP4 est un format de conteneur idéal pour les enregistrements vidéo en raison de son excellente compatibilité et de sa compression :

```csharp
// Configurer la sortie MP4
var mp4Output = new MP4Output(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4"));
videoCapture1.Outputs_Add(mp4Output);
```

Lors de la configuration de la sortie MP4, considérez ces options :

- **Nommage des fichiers** — Implémentez un nommage dynamique basé sur la date/heure pour des enregistrements organisés
- **Emplacement de stockage** — Choisissez des chemins de stockage appropriés avec suffisamment d'espace disque
- **Qualité vidéo** — Configurez le débit binaire, la fréquence d'images et la résolution selon vos besoins
- **Métadonnées** — Ajoutez des métadonnées pertinentes aux enregistrements pour faciliter la classification et la recherche

### Gestion des ressources

Une gestion appropriée des ressources est essentielle lorsqu'on travaille avec des flux vidéo, afin d'éviter les fuites de mémoire et d'assurer des performances stables :

```csharp
private async void btStop_Click(object sender, EventArgs e)
{
    await videoCapture1.StopAsync();
    await videoCapture1.DisposeAsync();
}

private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    VisioForgeX.DestroySDK();
}
```

Implémentez toujours un nettoyage correct des ressources, en particulier :

- Arrêtez les flux actifs avant la fermeture de l'application
- Libérez les ressources de capture lorsqu'elles ne sont plus nécessaires
- Libérez les ressources du SDK à la fermeture de votre application

### Considérations avancées pour l'implémentation

Pour des applications de niveau production, envisagez ces fonctionnalités supplémentaires :

1. **Gestion des erreurs** — Implémentez une gestion complète des erreurs pour les déconnexions réseau, les échecs d'authentification et les problèmes de stockage
2. **Monitoring** — Ajoutez une supervision d'état pour suivre la santé du flux et l'état d'enregistrement
3. **Logique de reconnexion** — Implémentez une reconnexion automatique en cas d'interruptions réseau
4. **Prise en charge multi-caméras** — Étendez l'implémentation pour gérer plusieurs flux de caméras simultanément
5. **Planification d'enregistrement** — Ajoutez des fonctions d'enregistrement programmé pour les applications de surveillance

## Dépendances requises

Pour que cette implémentation fonctionne correctement, les paquets suivants doivent être inclus dans votre projet :

- Redistribuables de capture vidéo [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuables LAV [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)
- Redistribuables MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Résolution des problèmes courants

Lors de l'implémentation de la capture de caméra IP, préparez-vous à traiter ces défis fréquents :

1. **Échecs de connexion** — Vérifiez la connectivité réseau, les identifiants de la caméra et la configuration du pare-feu
2. **Performance du flux** — Équilibrez les paramètres de qualité avec la bande passante disponible et la puissance de traitement
3. **Erreurs de fichier de sortie** — Assurez-vous d'un espace disque adéquat et de permissions d'écriture appropriées
4. **Fuites de ressources** — Surveillez l'utilisation de la mémoire pendant les longues sessions d'enregistrement
5. **Compatibilité des caméras** — Différents modèles de caméras peuvent nécessiter des ajustements de configuration spécifiques

## Guides associés

- [Tutoriel d'aperçu en direct de caméra IP](ip-camera-preview.md) — commencez par l'aperçu avant d'ajouter l'enregistrement
- [Configuration de source de caméra RTSP](../video-sources/ip-cameras/rtsp.md) — transport UDP/TCP, réglage basse latence, sélection du moteur
- [Intégration de caméras IP ONVIF](../video-sources/ip-cameras/onvif.md) — auto-découverte et sélection de profil pour caméras IP
- [Enregistrer le flux RTSP original sans réencodage](../../mediablocks/Guides/rtsp-save-original-stream.md) — alternative Media Blocks préservant le débit source

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code. Besoin de l'URL RTSP de votre caméra ? Consultez notre [répertoire des marques de caméras IP](../../camera-brands/index.md).
