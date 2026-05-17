---
title: URL RTSP de caméras IP — Annuaire 62 marques en C# .NET
description: Annuaire d'URL RTSP pour 62 marques de caméras IP. Connectez Hikvision, Dahua, Axis, Uniview, EZVIZ, Arlo et plus avec VisioForge .NET.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureCore
  - RTSPSourceSettings
  - IVideoView
  - IPCameraSourceSettings

---

# Guide de connexion aux caméras IP par marque

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Se connecter à des caméras IP en C# .NET est simple lorsque vous connaissez le modèle d'URL RTSP correct pour votre marque de caméra. Chaque fabricant utilise des formats d'URL, des ports et des méthodes d'authentification légèrement différents.

Cet annuaire fournit des **modèles d'URL RTSP par marque**, des exemples de code de connexion utilisant le SDK VisioForge et des conseils de dépannage pour les fabricants de caméras IP les plus populaires.

## Fonctionnement des connexions RTSP de caméras

La plupart des caméras IP modernes exposent leurs flux vidéo via le **protocole RTSP (Real-Time Streaming Protocol)** sur le port 554. Le flux général de connexion est le suivant :

1. Déterminer l'adresse IP de votre caméra (via la découverte ONVIF, la table de baux DHCP ou l'utilitaire du fabricant)
2. Construire l'URL RTSP en utilisant le modèle propre à la marque
3. S'authentifier avec les identifiants de la caméra
4. Se connecter et effectuer le rendu du flux vidéo

### Code de démarrage rapide { #quick-start-code }

Connectez-vous à n'importe quelle caméra RTSP en utilisant l'une des trois approches du SDK VisioForge :

=== "VideoCaptureCoreX"

    ```csharp
    // Initialiser le SDK (à appeler une seule fois au démarrage de l'application)
    await VisioForgeX.InitSDKAsync();

    var videoCapture = new VideoCaptureCoreX(VideoView1);

    // Créer la source RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream1"),
        "admin",
        "password",
        true); // capturer l'audio

    videoCapture.Video_Source = rtsp;

    await videoCapture.StartAsync();
    ```

=== "VideoCaptureCore"

    ```csharp
    var videoCapture = new VideoCaptureCore(VideoView1 as IVideoView);

    videoCapture.IP_Camera_Source = new IPCameraSourceSettings()
    {
        URL = new Uri("rtsp://admin:password@192.168.1.100:554/stream1"),
        Type = IPSourceEngine.Auto_LAV
    };

    videoCapture.Audio_PlayAudio = true;
    videoCapture.Audio_RecordAudio = false;
    videoCapture.Mode = VideoCaptureMode.IPPreview;

    await videoCapture.StartAsync();
    ```

=== "Media Blocks"

    ```csharp
    var pipeline = new MediaBlocksPipeline();

    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream1"),
        "admin",
        "password",
        audioEnabled: true);

    rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;

    var rtspSource = new RTSPSourceBlock(rtspSettings);
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    var audioRenderer = new AudioRendererBlock();

    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);

    await pipeline.StartAsync();
    ```

Remplacez l'URL RTSP par le modèle propre à votre marque, indiqué dans les pages ci-dessous.

### Quel SDK choisir ?

| SDK | Idéal pour | Plateformes |
|-----|----------|-----------|
| **VideoCaptureCoreX** | Nouveaux projets multiplateformes, .NET moderne | Windows, macOS, Linux, Android, iOS |
| **VideoCaptureCore** | Projets Windows uniquement, .NET Framework historique | Windows |
| **Media Blocks** | Pipelines avancés, chaînes de traitement personnalisées | Windows, macOS, Linux, Android, iOS |

**VideoCaptureCoreX** est recommandé pour la plupart des nouveaux projets. Utilisez **Media Blocks** lorsque vous devez construire des pipelines de traitement personnalisés avec plusieurs sources, filtres ou sorties.

## Marques de caméras

### Marques en vedette (guides complets)

| Marque | Siège social | Segment de marché | Guide |
|-------|-------------|----------------|-------|
| **Hikvision** | Hangzhou, Chine | Entreprise / Grand public | [Guide de connexion](hikvision.md) |
| **Dahua** | Hangzhou, Chine | Entreprise / Grand public | [Guide de connexion](dahua.md) |
| **Axis** | Lund, Suède | Entreprise / Professionnel | [Guide de connexion](axis.md) |
| **Reolink** | Hong Kong | Grand public / Prosumer | [Guide de connexion](reolink.md) |
| **Amcrest** | Houston, États-Unis | Grand public / PME | [Guide de connexion](amcrest.md) |
| **Samsung/Hanwha** | Grasbrunn, Allemagne / Séoul, Corée du Sud | Entreprise / Professionnel | [Guide de connexion](samsung.md) |
| **Bosch** | Grasbrunn, Allemagne | Entreprise / Infrastructures critiques | [Guide de connexion](bosch.md) |
| **Ubiquiti** | New York, États-Unis | Prosumer / PME | [Guide de connexion](ubiquiti.md) |
| **Foscam** | Shenzhen, Chine | Grand public / PME | [Guide de connexion](foscam.md) |
| **TP-Link** | Shenzhen, Chine | Grand public / PME | [Guide de connexion](tp-link.md) |
| **Vivotek** | Nouveau Taipei, Taïwan | Entreprise / Professionnel | [Guide de connexion](vivotek.md) |
| **Panasonic/i-PRO** | Tokyo, Japon | Entreprise / Gouvernement | [Guide de connexion](panasonic.md) |
| **Sony** | Tokyo, Japon | Entreprise (arrêté en 2020) | [Guide de connexion](sony.md) |
| **Lorex** | Markham, Canada | Grand public / Prosumer | [Guide de connexion](lorex.md) |
| **D-Link** | Taipei, Taïwan | Grand public / PME | [Guide de connexion](dlink.md) |
| **Honeywell** | Charlotte, États-Unis | Entreprise / Commercial | [Guide de connexion](honeywell.md) |
| **Pelco** | Fresno, États-Unis (Motorola Solutions) | Entreprise / Gouvernement | [Guide de connexion](pelco.md) |
| **Cisco** | San Jose, États-Unis | Entreprise / Grand public-PME (historique) | [Guide de connexion](cisco.md) |
| **Grandstream** | Boston, États-Unis | PME / Professionnel | [Guide de connexion](grandstream.md) |
| **Swann** | Melbourne, Australie | Grand public / Prosumer | [Guide de connexion](swann.md) |
| **GeoVision** | Taipei, Taïwan | Entreprise / Professionnel | [Guide de connexion](geovision.md) |
| **ACTi** | Taipei, Taïwan | Professionnel / Entreprise | [Guide de connexion](acti.md) |
| **Canon** | Tokyo, Japon | Professionnel / Entreprise | [Guide de connexion](canon.md) |
| **FLIR (Teledyne)** | Wilsonville, États-Unis | Entreprise / Thermique | [Guide de connexion](flir.md) |
| **Milesight** | Xiamen, Chine | Professionnel / PME | [Guide de connexion](milesight.md) |
| **INSTAR** | Hanau, Allemagne | Grand public / Maison connectée | [Guide de connexion](instar.md) |
| **Zmodo** | Shenzhen, Chine | Grand public / Économique | [Guide de connexion](zmodo.md) |
| **Arecont Vision** | Glendale, États-Unis (Costar Group) | Professionnel / Entreprise | [Guide de connexion](arecont.md) |
| **JVC** | Yokohama, Japon | Professionnel (arrêté vers 2015) | [Guide de connexion](jvc.md) |
| **Toshiba** | Tokyo, Japon | Entreprise (arrêté) | [Guide de connexion](toshiba.md) |
| **LG** | Séoul, Corée du Sud | Entreprise (arrêté) | [Guide de connexion](lg.md) |
| **Linksys** | Irvine, États-Unis | Grand public (arrêté vers 2014) | [Guide de connexion](linksys.md) |
| **LTS** | City of Industry, États-Unis | Professionnel (OEM Hikvision) | [Guide de connexion](lts.md) |
| **Q-See** | Anaheim, États-Unis | Grand public (disparu vers 2020) | [Guide de connexion](q-see.md) |
| **Speco Technologies** | Amityville, États-Unis | Professionnel | [Guide de connexion](speco.md) |
| **EverFocus** | Nouveau Taipei, Taïwan | Professionnel | [Guide de connexion](everfocus.md) |
| **ABUS** | Wetter, Allemagne | Grand public / Professionnel | [Guide de connexion](abus.md) |
| **Basler** | Ahrensburg, Allemagne | Vision industrielle / Industriel | [Guide de connexion](basler.md) |
| **Mobotix** | Langmeil, Allemagne (Konica Minolta) | Industriel / Infrastructures critiques | [Guide de connexion](mobotix.md) |
| **Avigilon** | Vancouver, Canada (Motorola Solutions) | Entreprise / Infrastructures critiques | [Guide de connexion](avigilon.md) |
| **AVTech** | Taipei, Taïwan | Commercial / Industriel | [Guide de connexion](avtech.md) |
| **LILIN** | Nouveau Taipei, Taïwan | Professionnel / Entreprise | [Guide de connexion](lilin.md) |
| **Zavio** | Hsinchu, Taïwan | Professionnel / PME | [Guide de connexion](zavio.md) |
| **CP Plus** | Delhi, Inde | Entreprise / Commercial | [Guide de connexion](cp-plus.md) |
| **Sanyo** | Osaka, Japon (aujourd'hui Panasonic) | Professionnel (arrêté) | [Guide de connexion](sanyo.md) |
| **BrickCom** | Taipei, Taïwan | Professionnel / Industriel | [Guide de connexion](brickcom.md) |
| **Edimax** | Taipei, Taïwan | Grand public / PME | [Guide de connexion](edimax.md) |
| **Uniview (UNV)** | Hangzhou, Chine | Entreprise / Gouvernement | [Guide de connexion](uniview.md) |
| **Hanwha Vision** | Séoul, Corée du Sud | Entreprise / Professionnel | [Guide de connexion](hanwha.md) |
| **Tiandy** | Tianjin, Chine | Entreprise / PME | [Guide de connexion](tiandy.md) |
| **EZVIZ** | Hangzhou, Chine (Hikvision) | Grand public / Maison connectée | [Guide de connexion](ezviz.md) |
| **Wisenet** | Séoul, Corée du Sud (Hanwha Vision) | Entreprise / Professionnel | [Guide de connexion](wisenet.md) |
| **Annke** | Hong Kong | Grand public / Prosumer | [Guide de connexion](annke.md) |
| **Imou** | Hangzhou, Chine (Dahua) | Grand public / Maison connectée | [Guide de connexion](imou.md) |
| **Wyze** | Kirkland, États-Unis | Grand public (RTSP limité) | [Guide de connexion](wyze.md) |
| **Aqara** | Shenzhen, Chine | Maison connectée / HomeKit | [Guide de connexion](aqara.md) |
| **Verkada** | San Mateo, États-Unis | Entreprise / Géré dans le cloud | [Guide de connexion](verkada.md) |
| **Rhombus** | Sacramento, États-Unis | Entreprise / Géré dans le cloud | [Guide de connexion](rhombus.md) |
| **Arlo** | Carlsbad, États-Unis | Grand public (pas de RTSP) | [Guide de connexion](arlo.md) |
| **Eufy Security** | Changsha, Chine (Anker) | Grand public / Maison connectée | [Guide de connexion](eufy.md) |
| **Tenda** | Shenzhen, Chine | Grand public / Économique | [Guide de connexion](tenda.md) |
| **Mercusys** | Shenzhen, Chine (TP-Link) | Grand public / Économique | [Guide de connexion](mercusys.md) |

### Modèles d'URL RTSP courants par marque

Pour référence rapide, voici les principaux modèles d'URL RTSP pour les marques de caméras populaires :

| Marque | Modèle d'URL RTSP principal | Port par défaut |
|-------|--------------------------|-------------|
| Hikvision | `rtsp://IP:554/Streaming/Channels/101` | 554 |
| Dahua | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Axis | `rtsp://IP:554/axis-media/media.amp` | 554 |
| Foscam | `rtsp://IP:88/videoMain` | 88 |
| TP-Link (Tapo) | `rtsp://IP:554/stream1` | 554 |
| Amcrest | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Reolink | `rtsp://IP:554/h264Preview_01_main` | 554 |
| Ubiquiti | `rtsp://IP:7447/STREAM_TOKEN` | 7447 |
| Samsung/Hanwha | `rtsp://IP:554/profile2/media.smp` | 554 |
| Bosch | `rtsp://IP:554/video?inst=1` | 554 |
| Vivotek | `rtsp://IP:554/live.sdp` | 554 |
| Panasonic/i-PRO | `rtsp://IP:554/MediaInput/h264` | 554 |
| Sony | `rtsp://IP:554/media/video1` | 554 |
| Lorex | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| D-Link | `rtsp://IP:554/live1.sdp` | 554 |
| Honeywell | `rtsp://IP:554/h264` | 554 |
| Pelco | `rtsp://IP:554//stream1` | 554 |
| Cisco | `rtsp://IP:554/img/media.sav` | 554 |
| Grandstream | `rtsp://IP:554/live/ch00_0` | 554 |
| Swann | `rtsp://IP:554/live/h264` | 554 |
| GeoVision | `rtsp://IP:8554//CH001.sdp` | 8554 |
| ACTi | `rtsp://IP:7070//stream1` | 7070 |
| Canon | `rtsp://IP:554/cam1/h264` | 554 |
| FLIR (Teledyne) | `rtsp://IP:554/ch0` | 554 |
| Milesight | `rtsp://IP:554//main` | 554 |
| INSTAR | `rtsp://IP:554//11` | 554 |
| Zmodo | `rtsp://IP:10554//tcp/av0_0` | 10554 |
| Arecont Vision | `rtsp://IP:554/h264.sdp` | 554 |
| JVC | `rtsp://IP:554/PSIA/Streaming/channels/0` | 554 |
| Toshiba | `rtsp://IP:554/live.sdp` | 554 |
| LG | `rtsp://IP:554/video1+audio1` | 554 |
| Linksys | `rtsp://IP:554/img/media.sav` | 554 |
| LTS | `rtsp://IP:554//Streaming/Channels/1` | 554 |
| Q-See | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | 554 |
| Speco | `rtsp://IP:554/1/stream1` | 554 |
| EverFocus | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | 554 |
| ABUS | `rtsp://IP:554/video.mp4` | 554 |
| Basler | `rtsp://IP:554/h264` | 554 |
| Mobotix | `rtsp://IP:554/mobotix.h264` | 554 |
| Avigilon | `rtsp://IP:554/defaultPrimary?streamType=u` | 554 |
| AVTech | `rtsp://IP:554/live/h264` | 554 |
| LILIN | `rtsp://IP:554/rtsph2641080p` | 554 |
| Zavio | `rtsp://IP:554/video.mp4` | 554 |
| CP Plus | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | 554 |
| Sanyo | `rtsp://IP:554/VideoInput/1/h264/1` | 554 |
| BrickCom | `rtsp://IP:554/channel1` | 554 |
| Edimax | `rtsp://IP:554/ipcam_h264.sdp` | 554 |
| Uniview (UNV) | `rtsp://IP:554/media/video1` | 554 |
| Hanwha Vision | `rtsp://IP:554/profile2/media.smp` | 554 |
| Tiandy | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| EZVIZ | `rtsp://IP:554/h264/ch1/main/av_stream` | 554 |
| Wisenet | `rtsp://IP:554/profile2/media.smp` | 554 |
| Annke | `rtsp://IP:554/Streaming/Channels/101` | 554 |
| Imou | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Wyze | `rtsp://IP:8554/live` | 8554 |
| Aqara | `rtsp://IP:554/live/ch00_1` | 554 |
| Verkada | N/A (cloud uniquement) | N/A |
| Rhombus | `rtsp://IP:554/live` (si activé) | 554 |
| Arlo | N/A (pas de RTSP) | N/A |
| Eufy Security | `rtsp://IP:554/live0` | 554 |
| Tenda | `rtsp://IP:554/stream1` | 554 |
| Mercusys | `rtsp://IP:554/stream1` | 554 |

## Découverte ONVIF

La plupart des caméras IP modernes prennent en charge **ONVIF (Open Network Video Interface Forum)**, qui permet la découverte automatique des caméras sur votre réseau. Le SDK VisioForge prend en charge la découverte ONVIF — consultez notre [guide d'intégration ONVIF](../mediablocks/Sources/index.md) pour plus de détails.

## Prise en main { #get-started }

### Installation via NuGet

=== "Multiplateforme (recommandé)"

    ```bash
    dotnet add package VisioForge.CrossPlatform.Core
    ```

=== "Windows uniquement"

    ```bash
    dotnet add package VisioForge.DotNet.Core
    dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
    ```

### Projets d'exemple

Exemples fonctionnels complets pour l'intégration de caméras IP :

- [Aperçu de caméra IP (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview) — Vue caméra en direct
- [Enregistrement de caméra IP vers MP4](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-capture-mp4) — Enregistrer des flux vers un fichier
- [Tous les exemples du SDK .NET](https://github.com/visioforge/.Net-SDK-s-samples) — Dépôt complet d'exemples

## Ressources connexes

- [Documentation du bloc source RTSP](../mediablocks/Sources/index.md)
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Enregistrement de caméra IP vers MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md)
- [Création d'applications de caméra avec Media Blocks](../mediablocks/GettingStarted/camera.md)
- [Guide d'énumération de périphériques](../mediablocks/GettingStarted/device-enum.md)
