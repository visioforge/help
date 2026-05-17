---
title: Configurer IIS Smooth Streaming pour applications .NET
description: Configurez Microsoft IIS Smooth Streaming en .NET avec débit adaptatif, compatibilité mobile et dépannage pour une diffusion vidéo de qualité.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Streaming
  - Editing
  - HLS
  - MP4
  - C#
  - JavaScript
  - NuGet

---

# Guide complet de l'implémentation d'IIS Smooth Streaming

IIS Smooth Streaming est l'implémentation Microsoft de la technologie de streaming adaptatif qui ajuste dynamiquement la qualité vidéo en fonction des conditions réseau et des capacités CPU. Ce guide fournit des instructions détaillées sur la configuration et l'implémentation d'IIS Smooth Streaming à l'aide des SDK VisioForge.

## SDK VisioForge compatibles

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button } 

## Présentation d'IIS Smooth Streaming

IIS Smooth Streaming offre plusieurs avantages clés pour les développeurs et les utilisateurs finaux :

- **Streaming à débit adaptatif** : ajuste automatiquement la qualité vidéo en fonction de la bande passante disponible
- **Mise en tampon réduite** : minimise les interruptions de lecture pendant les fluctuations réseau
- **Large compatibilité de périphériques** : fonctionne sur ordinateurs de bureau, périphériques mobiles, téléviseurs intelligents, etc.
- **Diffusion évolutive** : gère efficacement un grand nombre de spectateurs simultanés

Cette technologie est particulièrement précieuse pour les applications nécessitant une diffusion vidéo de haute qualité dans des conditions réseau variées, telles que les événements en direct, les plateformes éducatives et les applications riches en médias.

## Prérequis

Avant d'implémenter IIS Smooth Streaming avec les SDK VisioForge, assurez-vous d'avoir :

1. Windows Server avec IIS installé
2. Un accès administratif au serveur
3. Le SDK VisioForge approprié (Video Capture SDK .Net ou Video Edit SDK .Net)
4. Une compréhension de base du développement .NET

## Configuration IIS étape par étape

### Installation des composants requis

1. Installez [Web Platform Installer](https://www.microsoft.com/web/downloads/platform.aspx) sur votre serveur.
2. Via Web Platform Installer, recherchez et installez IIS Media Services.

![Installation d'IIS Media Services](https://www.visioforge.com/wp-content/uploads/2021/02/iis1.jpg)

Ce paquet de composants inclut tous les modules nécessaires à la fonctionnalité Smooth Streaming, y compris le service Live Smooth Streaming Publishing.

### Configuration d'IIS Manager

1. Ouvrez IIS Manager sur votre serveur via le menu Démarrer ou en exécutant `inetmgr` dans la boîte de dialogue Exécuter.

![Ouverture d'IIS Manager](https://www.visioforge.com/wp-content/uploads/2021/02/iis2.jpg)

2. Dans le volet de navigation à gauche, localisez et développez le nom de votre serveur, puis sélectionnez le site sur lequel vous souhaitez activer Smooth Streaming.

### Création d'un point de publication

1. Dans le site sélectionné, recherchez et ouvrez la fonctionnalité « Live Smooth Streaming Publishing Points ».
2. Cliquez sur « Add » pour créer un nouveau point de publication.

![Ajout d'un point de publication](https://www.visioforge.com/wp-content/uploads/2021/02/iis3.jpg)

3. Configurez les paramètres de base de votre point de publication :
   - **Name** : fournissez un nom descriptif pour votre point de publication (par ex. « MainStream »)
   - **Path** : spécifiez le chemin de fichier où le contenu Smooth Streaming sera stocké

![Configuration du nom du point de publication](https://www.visioforge.com/wp-content/uploads/2021/02/iis4.jpg)

4. Configurez les paramètres supplémentaires en activant la case « Allow clients to connect to this publishing point ». Cela garantit que les clients peuvent se connecter et recevoir le contenu diffusé.

![Paramètres supplémentaires du point de publication](https://www.visioforge.com/wp-content/uploads/2021/02/iis5.jpg)

### Activation de la prise en charge des périphériques mobiles

Pour vous assurer que votre contenu Smooth Streaming est accessible sur les périphériques mobiles :

1. Dans la configuration du point de publication, naviguez vers l'onglet « Mobile Devices ».
2. Activez la case « Allow playback on mobile devices ».

![Configuration des périphériques mobiles](https://www.visioforge.com/wp-content/uploads/2021/02/iis6.jpg)

Ce paramètre génère les formats et manifestes nécessaires à la lecture mobile, élargissant significativement la portée de votre contenu.

### Configuration du lecteur

Pour fournir aux spectateurs un moyen de regarder votre contenu Smooth Streaming :

1. Téléchargez le contrôle Silverlight Smooth Streaming Player fourni par Microsoft.
2. Extrayez les fichiers téléchargés et localisez le fichier `.xap`.
3. Copiez ce fichier `.xap` dans le répertoire de votre site web.
4. Copiez le fichier HTML inclus dans le même répertoire et renommez-le en `index.html`.
5. Ouvrez `index.html` dans un éditeur de texte et remplacez la section « initparams » par la configuration suivante :

```html
<param name="initParams" value="selectedCaptionStreamsCount=0,
autoplay=true,
muted=false,
displayCCButton=false,
mediaLoadTimeout=60000,
stretchMode=none,
poster=,
enableGPUAcceleration=true,
startupBitrate=400000,
disableDynamicHeader=false,
backwardBufferLength=0,
initialEntryStartPosition=0,
forwardBufferLength=10000,
sourceType=livetv,
adaptivestreamingplugin.smoothstreaming=true,
adaptivestreamingplugin.LiveSmoothStreaming=true,
mediaurl=http://localhost/mainstream.isml/manifest" />
```

Cette configuration initialise le lecteur Silverlight avec des paramètres optimaux pour la lecture Smooth Streaming. Le paramètre `mediaurl` doit pointer vers le manifeste de votre point de publication.

### Démarrage du point de publication

1. Revenez à IIS Manager et sélectionnez votre point de publication configuré.
2. Cliquez sur l'action « Start » dans le panneau de droite.

Le point de publication sera désormais actif et prêt à recevoir le contenu de votre application.

## Implémentation de Smooth Streaming dans des applications SDK VisioForge

### Configuration de base

Configurez une instance `VideoCaptureCore` (ou `VideoEditCore`) existante pour pousser vers votre point de publication IIS à l'aide de `NetworkStreamingFormat.SSF_H264_AAC_SW` (Smooth Streaming, H.264 + AAC, encodage logiciel) :

```csharp
using VisioForge.Core.Types.Output;
using VisioForge.Core.VideoCapture;

// Suppose que VideoCapture1 est un VideoCaptureCore déjà lié à un VideoView.
VideoCapture1.Network_Streaming_Enabled   = true;
VideoCapture1.Network_Streaming_Audio_Enabled = true;

// Choisir le format Smooth Streaming. Utiliser SSF_FFMPEG_EXE si vous préférez l'encodage basé sur FFMPEG.
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.SSF_H264_AAC_SW;

// URL du point de publication IIS créé ci-dessus.
VideoCapture1.Network_Streaming_URL = "http://localhost/mainstream.isml";

// Paramètres H.264 + AAC via MP4Output (Smooth Streaming multiplexe H.264/AAC dans le conteneur ISML).
var streamOutput = new MP4Output();
streamOutput.Video = new MP4OutputH264Settings
{
    Bitrate     = 2500,              // kbps — débit cible
    Profile     = H264Profile.ProfileMain,
    Level       = H264Level.Level4,
    RateControl = H264RateControl.VBR
};
streamOutput.Audio_AAC.Bitrate = 128;
streamOutput.Audio_AAC.Object  = AACObject.Low;

VideoCapture1.Network_Streaming_Output = streamOutput;

await VideoCapture1.StartAsync();
```

![Configuration de Smooth Streaming dans la démo SDK](https://www.visioforge.com/wp-content/uploads/2021/02/iis7.jpg)

### Vérification de la connexion

Une fois votre application configurée :

1. Vérifiez le statut de la connexion dans votre application. Vous devriez voir une confirmation que le SDK s'est connecté avec succès à IIS.

![Connexion IIS réussie](https://www.visioforge.com/wp-content/uploads/2021/02/iis8.jpg)

2. Ouvrez un navigateur web et naviguez vers `http://localhost` (ou l'adresse de votre serveur).
3. Le lecteur Silverlight devrait se charger et commencer à lire votre flux.

![Lecture du flux dans le navigateur](https://www.visioforge.com/wp-content/uploads/2021/02/iis10.jpg)

### Streaming HTML5 pour les périphériques iOS

Pour une compatibilité plus large des périphériques, en particulier les périphériques iOS qui ne prennent pas en charge Silverlight, créez un lecteur HTML5 :

1. Créez un nouveau fichier HTML dans le répertoire de votre site web.
2. Incluez le code suivant dans le fichier :

```html
<!DOCTYPE html>
<html>
<head>
    <title>Smooth Streaming HTML5 Player</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .player-container { max-width: 800px; margin: 0 auto; }
        video { width: 100%; height: auto; }
    </style>
</head>
<body>
    <div class="player-container">
        <h1>HTML5 Smooth Streaming Player</h1>
        <video id="videoPlayer" controls autoplay>
            <source src="http://localhost/mainstream.isml/manifest(format=m3u8-aapl)" type="application/x-mpegURL">
            Your browser does not support HTML5 video.
        </video>
    </div>
    
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            var video = document.getElementById('videoPlayer');
            video.addEventListener('error', function(e) {
                console.error('Video playback error:', e);
            });
        });
    </script>
</body>
</html>
```

Ce lecteur HTML5 utilise le format HLS (HTTP Live Streaming), qui est généré automatiquement par IIS Media Services lorsque vous activez la prise en charge des périphériques mobiles.

## Redistribuables requis

Pour assurer le bon fonctionnement de votre application avec IIS Smooth Streaming, incluez les redistribuables suivants :

- Redistribuables du SDK pour votre SDK VisioForge spécifique
- Redistribuables MP4 :
  - Pour les architectures x86 : [VisioForge.DotNet.Core.Redist.MP4.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  - Pour les architectures x64 : [VisioForge.DotNet.Core.Redist.MP4.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

Vous pouvez ajouter ces paquets via NuGet Package Manager dans Visual Studio ou en ligne de commande :

```
Install-Package VisioForge.DotNet.Core.Redist.MP4.x64
```

## Options de configuration avancées

Pour les environnements de production, envisagez ces configurations supplémentaires :

- **Encodage multi-débits** : configurez votre SDK VisioForge pour encoder à plusieurs débits afin d'optimiser le streaming adaptatif
- **Paramètres de manifeste personnalisés** : modifiez le manifeste Smooth Streaming pour des exigences de lecture spécialisées
- **Authentification** : implémentez une authentification basée sur des jetons pour un streaming sécurisé
- **Chiffrement de contenu** : activez la protection DRM pour les contenus sensibles
- **Équilibrage de charge** : configurez plusieurs points de publication derrière un équilibreur de charge pour les scénarios à fort trafic

## Dépannage des problèmes courants

- **Échecs de connexion** : vérifiez que les paramètres du pare-feu autorisent le trafic sur le port de streaming (généralement 80 ou 443)
- **Saccades de lecture** : vérifiez les ressources du serveur et envisagez d'augmenter les paramètres de mise en tampon
- **Problèmes de compatibilité mobile** : assurez-vous que la génération du format mobile est activée et testez sur plusieurs périphériques
- **Problèmes de qualité** : ajustez les paramètres d'encodage et la configuration de l'échelle de débits

## Conclusion

IIS Smooth Streaming, lorsqu'il est implémenté avec les SDK VisioForge, fournit une solution robuste pour la diffusion vidéo adaptative dans des conditions réseau et des périphériques variés. En suivant ce guide complet, vous pouvez configurer, implémenter et optimiser Smooth Streaming pour vos applications .NET.

Pour des exemples de code et d'implémentation supplémentaires, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

---
*Cette documentation est fournie par VisioForge. Pour un support supplémentaire ou des informations sur nos SDK, veuillez consulter [www.visioforge.com](https://www.visioforge.com).*
