---
title: Créer un serveur RTSP en C# .NET — H.264/HEVC + encodage GPU
description: Serveur RTSP avec encodage H.264/AAC, accélération GPU (NVENC, QSV, AMF), authentification et réglage de la latence. Exemples multiplateformes.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Encoding
  - Editing
  - IP Camera
  - RTSP
  - ONVIF
  - UDP
  - WebRTC
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoEditCoreX
  - RTSPServerOutput
  - RTSPServerSettings
  - RTSPServerBlock

---

# Maîtriser la diffusion RTSP avec les SDK VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Le moteur `VideoCaptureCoreX` et le Media Blocks SDK fonctionnent sur **Windows, macOS, Linux, Android et iOS** via GStreamer. Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) pour les détails des codecs et de l'accélération matérielle, et le [guide de déploiement Linux](../../deployment-x/Ubuntu.md) pour la configuration sur Ubuntu / NVIDIA Jetson / Raspberry Pi.

!!! tip "Agents de code IA : utilisez le serveur MCP VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de code IA ?
    Connectez-vous au [serveur MCP VisioForge](../mcp-server-usage.md) public
    à `https://mcp.visioforge.com/mcp` pour des recherches API structurées, des
    exemples de code exécutables et des guides de déploiement — plus précis que de
    parcourir `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction au RTSP

Le protocole RTSP (Real-Time Streaming Protocol) est un protocole de contrôle réseau conçu pour les systèmes de divertissement et de communication afin de contrôler des serveurs de streaming multimédia. Il agit comme une « télécommande réseau », permettant aux utilisateurs de lire, mettre en pause et arrêter les flux multimédias. Les SDK VisioForge exploitent la puissance du RTSP pour fournir des capacités robustes de diffusion vidéo et audio.

Nos SDK intègrent le RTSP avec des codecs standards de l'industrie comme **H.264 (AVC)** pour la vidéo et **Advanced Audio Coding (AAC)** pour l'audio. H.264 offre une excellente qualité vidéo à des débits binaires relativement faibles, ce qui le rend idéal pour la diffusion dans diverses conditions réseau. AAC fournit une compression audio efficace et haute fidélité. Cette combinaison puissante garantit une diffusion audiovisuelle fiable et haute définition adaptée à des applications exigeantes telles que :

*   **Sécurité et vidéosurveillance :** fournir des flux vidéo clairs en temps réel à partir de caméras IP.
*   **Diffusion en direct :** diffuser des événements, webinaires ou spectacles à un large public.
*   **Visioconférence :** permettre une communication fluide et de haute qualité.
*   **Supervision à distance :** observer des processus industriels ou des environnements à distance.

Ce guide explore en détail l'implémentation de la diffusion RTSP avec les SDK VisioForge, couvrant à la fois les approches multiplateformes modernes et les méthodes héritées spécifiques à Windows.

## Sortie RTSP multiplateforme (recommandé)

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Les SDK VisioForge modernes (versions `CoreX` et Media Blocks) fournissent une implémentation flexible et puissante de serveur RTSP multiplateforme bâtie sur le framework robuste GStreamer. Cette approche offre un meilleur contrôle, une prise en charge plus large des codecs et la compatibilité avec Windows, Linux, macOS et d'autres plateformes.

### Composant central : `RTSPServerOutput`

La classe `RTSPServerOutput` est le point central de configuration pour établir un flux RTSP dans les Video Capture SDK ou Video Edit (versions `CoreX`). Elle agit comme un pont entre votre pipeline de capture/édition et la logique sous-jacente du serveur RTSP.

**Principales responsabilités :**

*   **Implémentation d'interfaces :** implémente `IVideoEditXBaseOutput` et `IVideoCaptureXBaseOutput`, permettant une intégration transparente comme format de sortie aussi bien dans les scénarios d'édition que de capture.
*   **Gestion des paramètres :** contient l'objet `RTSPServerSettings`, qui rassemble tous les paramètres de configuration détaillés de l'instance de serveur.
*   **Spécification du codec :** définit les encodeurs vidéo et audio utilisés pour compresser le média avant la diffusion.

**Encodeurs pris en charge :**

VisioForge donne accès à un large éventail d'encodeurs, permettant l'optimisation selon les capacités matérielles et les plateformes cibles :

*   **Encodeurs vidéo :**
    *   **Accélérés matériellement (recommandés pour les performances) :**
        *   `NVENC` (NVIDIA) : exploite le matériel d'encodage dédié sur les GPU NVIDIA.
        *   `QSV` (Intel Quick Sync Video) : utilise les capacités GPU intégrées des processeurs Intel.
        *   `AMF` (AMD Advanced Media Framework) : utilise le matériel d'encodage des GPU/APU AMD.
    *   **Logiciels (indépendants de la plateforme, utilisation CPU plus élevée) :**
        *   `OpenH264` : encodeur logiciel H.264 largement compatible.
        *   `VP8` / `VP9` : codecs vidéo libres de droits développés par Google, offrant une bonne compression (souvent utilisés avec WebRTC, mais disponibles ici).
    *   **Spécifiques à une plateforme :**
        *   `MF HEVC` (Media Foundation HEVC) : encodeur H.265/HEVC spécifique à Windows pour une compression à plus haute efficacité.
*   **Encodeurs audio :**
    *   **Variantes AAC :**
        *   `VO-AAC` : encodeur AAC polyvalent et multiplateforme.
        *   `AVENC AAC` : utilise l'encodeur AAC de FFmpeg.
        *   `MF AAC` : encodeur AAC Windows Media Foundation.
    *   **Autres formats :**
        *   `MP3` : largement compatible mais moins efficace que l'AAC.
        *   `OPUS` : excellent codec à faible latence, idéal pour les applications interactives.

### Configuration du flux : `RTSPServerSettings`

Cette classe encapsule tous les paramètres nécessaires pour définir le comportement et les propriétés de votre serveur RTSP.

**Propriétés détaillées :**

*   **Configuration réseau :**
    *   `Port` (int) : port TCP sur lequel le serveur écoute les connexions RTSP entrantes. La valeur par défaut est `8554`, alternative courante au port standard (souvent restreint) 554. Assurez-vous que ce port est ouvert dans les pare-feu.
    *   `Address` (string) : adresse IP à laquelle le serveur se lie.
        *   `"127.0.0.1"` (par défaut) : n'écoute que les connexions de la machine locale.
        *   `"0.0.0.0"` : écoute sur toutes les interfaces réseau disponibles (à utiliser pour un accès public).
        *   IP spécifique (par exemple, `"192.168.1.100"`) : se lie uniquement à cette interface réseau précise.
    *   `Point` (string) : composant de chemin de l'URL RTSP (par exemple, `/live`, `/stream1`). Les clients se connectent à `rtsp://<Address>:<Port><Point>`. Valeur par défaut : `"/live"`.
*   **Configuration du flux :**
    *   `VideoEncoder` (IVideoEncoder) : instance `IVideoEncoder` — généralement un objet de paramètres d'encodeur (par exemple, `OpenH264EncoderSettings`) ou le résultat de `H264EncoderBlock.GetDefaultSettings()`. Définit le codec, le débit binaire (kbit/s), la qualité, etc.
    *   `AudioEncoder` (IAudioEncoder) : instance `IAudioEncoder` (par exemple, `VOAACEncoderSettings`). Définit les paramètres du codec audio.
    *   `Latency` (TimeSpan) : contrôle le délai de mise en tampon introduit par le serveur pour lisser la gigue du réseau. Valeur par défaut : 250 millisecondes. Des valeurs plus élevées augmentent la stabilité mais aussi le délai.
*   **Authentification :**
    *   `Username` (string) : si défini, les clients doivent fournir ce nom d'utilisateur pour l'authentification basique.
    *   `Password` (string) : si défini, les clients doivent fournir ce mot de passe avec le nom d'utilisateur.
*   **Identité du serveur :**
    *   `Name` (string) : nom convivial pour le serveur, parfois affiché par les applications clientes.
    *   `Description` (string) : description plus détaillée du contenu du flux ou de la finalité du serveur.
*   **Propriété de commodité :**
    *   `URL` (string, lecture seule) : compose automatiquement l'URL complète de connexion RTSP à partir d'`Address`, `Port` et `Point`. Avec les valeurs par défaut de `RTSPServerSettings` (`Port = 8554`), l'URL composée est `rtsp://127.0.0.1:8554/live`.

### Le moteur : `RTSPServerBlock` (Media Blocks SDK)

Avec le Media Blocks SDK, `RTSPServerBlock` représente l'élément réel basé sur GStreamer qui effectue la diffusion.

**Fonctionnalités :**

*   **Puits multimédia :** agit comme point terminal (puits) dans un pipeline multimédia, recevant les données vidéo et audio encodées.
*   **Pads d'entrée :** fournit des pads distincts `VideoInput` et `AudioInput` pour connecter les sources/encodeurs vidéo et audio en amont.
*   **Intégration GStreamer :** gère le `rtspserver` GStreamer sous-jacent et les éléments associés nécessaires à la gestion des connexions clientes et à la diffusion des paquets RTP.
*   **Vérification de disponibilité :** la méthode statique `IsAvailable()` permet de vérifier si les plugins GStreamer nécessaires à la diffusion RTSP sont présents sur le système.
*   **Gestion des ressources :** implémente `IDisposable` pour le nettoyage correct des sockets réseau et des ressources GStreamer lorsque le bloc n'est plus nécessaire.

### Exemples d'utilisation pratiques

#### Exemple 1 : configuration de base du serveur (VideoCaptureCoreX / VideoEditCoreX)

```csharp
// 1. Choisir et configurer les encodeurs

// Utiliser l'accélération matérielle si disponible, sinon se rabattre sur le logiciel
var videoEncoder = H264EncoderBlock.GetDefaultSettings();

var audioEncoder = new VOAACEncoderSettings(); // AAC multiplateforme fiable

// 2. Configurer les paramètres réseau du serveur
var settings = new RTSPServerSettings(videoEncoder, audioEncoder)
{
    Port = 8554,
    Address = "0.0.0.0",  // Accessible depuis d'autres machines du réseau
    Point = "/livefeed"
};

// 3. Créer l'objet de sortie
var rtspOutput = new RTSPServerOutput(settings);

// 4. Intégrer au moteur SDK
// Pour VideoCaptureCoreX :
// videoCapture est une instance initialisée de VideoCaptureCoreX
videoCapture.Outputs_Add(rtspOutput); 

// Pour VideoEditCoreX :
// videoEdit est une instance initialisée de VideoEditCoreX
// videoEdit.Output_Format = rtspOutput; // À définir avant de démarrer l'édition/lecture
```

#### Exemple 2 : pipeline Media Blocks

```csharp
// Supposons que « pipeline » est un MediaBlocksPipeline initialisé
// Supposons que « videoSource » et « audioSource » fournissent des flux multimédias non encodés

// 1. Créer les paramètres des encodeurs vidéo et audio
var videoEncoder = H264EncoderBlock.GetDefaultSettings();

var audioEncoder = new VOAACEncoderSettings();

// 2. Créer les paramètres du serveur RTSP avec une URL spécifique
var serverUri = new Uri("rtsp://192.168.1.50:8554/cam1"); 
var rtspSettings = new RTSPServerSettings(serverUri, videoEncoder, audioEncoder)
{
    Description = "Camera Feed 1 - Warehouse"
};

// 3. Créer le bloc serveur RTSP
if (!RTSPServerBlock.IsAvailable())
{
    Console.WriteLine("RTSP Server components not available. Check GStreamer installation.");
    return; 
}
var rtspSink = new RTSPServerBlock(rtspSettings);

// Connecter directement la source au bloc serveur RTSP, qui utilisera ses propres encodeurs
pipeline.Connect(videoSource.Output, rtspSink.VideoInput); // Connexion directe à l'entrée vidéo du bloc serveur RTSP
pipeline.Connect(audioSource.Output, rtspSink.AudioInput); // Connexion directe à l'entrée audio du bloc serveur RTSP

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Exemple 3 : configuration avancée avec authentification

```csharp
// En utilisant les paramètres de l'Exemple 1...
var secureSettings = new RTSPServerSettings(videoEncoder, audioEncoder)
{
    Port = 8555, // Utiliser un port différent
    Address = "192.168.1.100", // Se lier à une adresse IP interne spécifique
    Point = "/secure",
    Username = "viewer",
    Password = "VerySecretPassword!",
    Latency = TimeSpan.FromMilliseconds(400), // Latence légèrement plus élevée
    Name = "SecureStream",
    Description = "Authorized access only"
};

var secureRtspOutput = new RTSPServerOutput(secureSettings);

// Ajouter à VideoCaptureCoreX ou définir pour VideoEditCoreX comme précédemment
// videoCapture.Outputs_Add(secureRtspOutput); 
```

### Bonnes pratiques de diffusion

1.  **Stratégie de sélection de l'encodeur :**
    *   **Privilégier le matériel :** préférez toujours les encodeurs matériels (NVENC, QSV, AMF) quand ils sont disponibles sur le système cible. Ils réduisent considérablement la charge CPU, autorisant des résolutions plus élevées, des fréquences d'images supérieures ou davantage de flux simultanés.
    *   **Repli logiciel :** utilisez `OpenH264` comme repli logiciel fiable pour une large compatibilité lorsque l'accélération matérielle n'est pas présente ou adaptée.
    *   **Choix du codec :** H.264 reste le codec le plus largement compatible avec les clients RTSP. HEVC offre une meilleure compression mais la prise en charge cliente peut être moins universelle.
2.  **Réglage de la latence :**
    *   **Interactivité ou stabilité :** une faible latence (par exemple, 100 à 200 ms) est cruciale pour des applications comme la visioconférence mais rend le flux plus sensible aux à-coups du réseau.
    *   **Diffusion/surveillance :** une latence plus élevée (par exemple, 500 ms à 1 000 ms et plus) fournit des tampons plus importants, améliorant la résilience du flux sur des réseaux instables (comme le Wi-Fi ou Internet) au prix d'un délai accru. Commencez avec la valeur par défaut (250 ms) et ajustez selon la qualité observée et les besoins.
3.  **Configuration réseau :**
    *   **Sécurité d'abord :** implémentez l'authentification `Username` et `Password` pour tout flux non destiné à un accès public anonyme.
    *   **Adresse de liaison :** utilisez `"0.0.0.0"` avec prudence. Pour une sécurité accrue, liez-vous explicitement à l'interface réseau (`Address`) destinée aux connexions client si possible.
    *   **Règles de pare-feu :** configurez méticuleusement les pare-feu système et réseau pour autoriser les connexions TCP entrantes sur le `Port` RTSP choisi. N'oubliez pas non plus que RTP/RTCP (utilisés pour les données multimédias proprement dites) utilisent souvent des ports UDP dynamiques ; les pare-feu peuvent nécessiter des modules d'aide (comme `nf_conntrack_rtsp` sur Linux) ou l'ouverture de larges plages de ports UDP (moins sûr).
4.  **Gestion des ressources :**
    *   **Pattern Dispose :** les instances de serveur RTSP détiennent des ressources réseau (sockets) et potentiellement des pipelines GStreamer complexes. *Veillez toujours* à les libérer correctement avec des instructions `using` ou des appels explicites à `.Dispose()` dans des blocs `finally` afin d'éviter les fuites de ressources.
    *   **Arrêt en douceur :** lors de l'arrêt du processus de capture ou d'édition, assurez-vous que la sortie est correctement retirée ou que le pipeline est arrêté proprement afin que le serveur RTSP puisse s'arrêter en douceur.

### Considérations de performance

Optimiser la diffusion RTSP consiste à équilibrer qualité, latence et utilisation des ressources :

1.  **Impact de l'encodeur :** c'est souvent le facteur le plus déterminant.
    *   **Matériel :** utilisation CPU nettement plus faible, débit potentiel plus élevé. Nécessite un matériel et des pilotes compatibles.
    *   **Logiciel :** charge CPU élevée, particulièrement à hautes résolutions/fréquences d'images. Limite le nombre de flux simultanés sur une seule machine, mais fonctionne partout.
2.  **Latence et bande passante :** des réglages de faible latence peuvent parfois entraîner une consommation de bande passante en pic plus élevée, le système ayant moins de temps pour lisser la transmission des données.
3.  **Surveillance des ressources :**
    *   **CPU :** surveillez attentivement l'utilisation du CPU, surtout avec les encodeurs logiciels. Une surcharge entraîne des pertes d'images et des saccades.
    *   **Mémoire :** surveillez l'utilisation de la RAM, surtout en cas de plusieurs flux ou de pipelines Media Blocks complexes.
    *   **Réseau :** assurez-vous que l'interface réseau du serveur dispose d'une bande passante suffisante pour le débit binaire configuré, la résolution et le nombre de clients connectés. Calculez la bande passante requise : (débit vidéo + débit audio) × nombre de clients.

## Sortie RTSP Windows uniquement (héritée)

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

L'implémentation comporte plusieurs mécanismes de gestion d'erreurs :

Les versions plus anciennes du SDK (`VideoCaptureCore`, `VideoEditCore`) incluaient un mécanisme de sortie RTSP plus simple, spécifique à Windows. Bien que fonctionnel, il offre moins de flexibilité et de prise en charge des codecs que la classe multiplateforme `RTSPServerOutput`. **Il est généralement recommandé d'utiliser l'approche `CoreX` / Media Blocks pour les nouveaux projets.**

### Fonctionnement

Cette méthode s'appuie sur des composants Windows intégrés ou des filtres spécifiques fournis. La configuration se fait directement via des propriétés de l'objet `VideoCaptureCore` ou `VideoEditCore`.

### Code de configuration

```csharp
using VisioForge.Core.Types.Output;           // MP4Output, NetworkStreamingFormat, H264Profile
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.VideoCapture;           // VideoCaptureCore

// On suppose que VideoCapture1 est une instance de VideoCaptureCore déjà liée à une VideoView.

// 1. Activer la diffusion réseau pour le composant
VideoCapture1.Network_Streaming_Enabled = true;

// 2. Activer la diffusion audio (facultatif)
VideoCapture1.Network_Streaming_Audio_Enabled = true;

// 3. Sélectionner le format RTSP.
//    RTSP_H264_AAC_SW = H.264 logiciel + AAC (RTSP Windows basé sur DirectShow).
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.RTSP_H264_AAC_SW;

// 4. Configurer les paramètres de l'encodeur. MP4Output contient les paramètres H.264 et AAC
//    même si le flux est envoyé via RTSP plutôt qu'écrit dans un fichier.
var mp4Output = new MP4Output();

//    Les paramètres H.264 vivent dans MP4Output.Video (MP4OutputH264Settings).
mp4Output.Video = new MP4OutputH264Settings
{
    Bitrate = 2000,                // kbps
    Profile = H264Profile.ProfileMain,
    Level = H264Level.Level4,
    RateControl = H264RateControl.VBR
};

//    Les paramètres AAC vivent dans MP4Output.Audio_AAC (M4AOutput — déjà initialisé dans le constructeur).
mp4Output.Audio_AAC.Bitrate = 128;      // kbps
mp4Output.Audio_AAC.Object = AACObject.Low;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4;

// 5. Assigner le conteneur de paramètres à la sortie de diffusion réseau
VideoCapture1.Network_Streaming_Output = mp4Output;

// 6. Définir l'URL RTSP que les clients utiliseront.
//    Le serveur écoute sur le port spécifié dans l'URL (5554 ici).
VideoCapture1.Network_Streaming_URL = "rtsp://localhost:5554/vfstream";
//    Utilisez l'IP réelle de la machine au lieu de localhost pour un accès externe.

// 7. Démarrer comme d'habitude (OnError se déclenche si le port est occupé ou les encodeurs indisponibles).
await VideoCapture1.StartAsync();
```

**Note :** cette méthode héritée s'appuie souvent sur des filtres DirectShow ou des transformations Media Foundation disponibles sur le système Windows en question, ce qui la rend moins prévisible et portable que la solution multiplateforme basée sur GStreamer.

## Voir aussi

* [Reconnexion RTSP et bascule de repli](../network-sources/reconnection-and-fallback.md) — gérez les déconnexions de caméra avec les événements de reconnexion, `DisconnectEventInterval` et le `FallbackSwitch` déclaratif (repli image / texte / média) dans tous les SDK VisioForge
* [Configuration de la source caméra RTSP en C#](../../videocapture/video-sources/ip-cameras/rtsp.md) — référence `IPCameraSourceSettings` et `RTSPSourceSettings` avec transport UDP/TCP et réglage des tampons
* [Intégration de caméras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) — WS-Discovery, sélection de profil et contrôle PTZ pour les caméras basées sur le standard
* [Tutoriel d'aperçu en direct caméra IP](../../videocapture/video-tutorials/ip-camera-preview.md) — guide vidéo pour une implémentation minimale d'aperçu
* [Enregistrer un flux RTSP au format MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md) — capturer n'importe quelle caméra IP vers un fichier MP4 en .NET
* [Lecteur RTSP Media Blocks](../../mediablocks/Guides/rtsp-player-csharp.md) — lecture RTSP basée sur le pipeline avec le Media Blocks SDK
* [Bloc de sortie serveur RTSP](../../mediablocks/RTSPServer/index.md) — diffusez votre propre flux vidéo et audio en RTSP
* [Grille RTSP multi-caméras (mur NVR)](../../mediablocks/Guides/multi-camera-rtsp-grid.md) — créez un mur d'aperçu en direct 4×4 avec WPF ou MAUI, avec démarrage synchronisé
* [Enregistrement pré-événement avec caméra IP](../../mediablocks/Guides/pre-event-recording.md) — enregistrez des flux RTSP avec tampon circulaire et déclencheurs de détection de mouvement
* [Enregistrer le flux RTSP original](../../mediablocks/Guides/rtsp-save-original-stream.md) — sauvegardez la vidéo RTSP sans réencodage
* [Lecteur de codes-barres et QR codes](../../mediablocks/Guides/barcode-qr-reader-guide.md) — analysez des codes-barres depuis les flux de caméras IP RTSP en temps réel

---
Pour des exemples plus détaillés et des cas d'usage avancés, explorez les exemples de code fournis dans notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples). Pour les URLs RTSP spécifiques à certaines marques, consultez notre [répertoire des marques de caméras IP](../../camera-brands/index.md).
