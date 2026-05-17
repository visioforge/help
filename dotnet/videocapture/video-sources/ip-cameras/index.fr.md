---
title: Intégration de caméras IP en C# .NET — Guides RTSP/ONVIF
description: Connectez des caméras IP via RTSP et ONVIF en C# / .NET. Authentification, multi-flux, découverte, reconnexion. Exemples complets pour les grandes marques.
sidebar_label: Caméras IP et sources réseau
order: 19
tags:
  - Video Capture SDK
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
  - IPCameraSourceSettings
  - RTSPSourceSettings
  - IPSourceEngine

---

# Guide complet de l'intégration des caméras IP et des sources réseau

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! info "Prise en charge multiplateforme"
    Le moteur `VideoCaptureCoreX` fonctionne sous **Windows, macOS, Linux, Android et iOS** via GStreamer. Le moteur classique `VideoCaptureCore` est réservé à Windows. Consultez la [matrice de prise en charge des plateformes](../../../platform-matrix.md) pour les détails sur les codecs et l'accélération matérielle, ainsi que le [guide de déploiement Linux](../../../deployment-x/Ubuntu.md) pour la configuration sur Ubuntu / NVIDIA Jetson / Raspberry Pi.

## Prise en charge des plateformes

| Type de source | `VideoCaptureCore` (Windows) | `VideoCaptureCoreX` (multiplateforme) |
|-------------|:---:|:---:|
| RTSP         | ✅ | ✅ Windows · macOS · Linux · Android · iOS |
| HTTP MJPEG   | ✅ | ✅ Windows · macOS · Linux · Android · iOS |
| ONVIF        | ✅ | ✅ via `RTSPSourceSettings.CreateAsync` |
| NDI          | ✅ | ✅ nécessite le runtime NDI par plateforme |
| SRT          | ✅ | ✅ Windows · macOS · Linux · Android · iOS |
| VNC          | ⛔ | ✅ |
| UDP          | ✅ | ✅ |

Pour les applications multiplateformes MAUI (base de code unique → Windows · macOS · iOS · Android), consultez le [guide de la grille RTSP multi-caméras 4×4](../../../mediablocks/Guides/multi-camera-rtsp-grid.md). Pour gérer les coupures de caméras, la logique de reconnexion et le `FallbackSwitch` automatique (texte / image / flux alternatif) sur tous les SDK, consultez le [guide de reconnexion RTSP et de solution de repli](../../../general/network-sources/reconnection-and-fallback.md).

## Introduction aux sources vidéo réseau

Les applications vidéo modernes nécessitent souvent l'intégration de diverses sources vidéo réseau. Le Video Capture SDK pour .NET offre une prise en charge robuste de différents types de caméras IP et de flux vidéo réseau, permettant aux développeurs d'incorporer facilement de la vidéo réseau en direct dans leurs applications .NET.

Ce guide complet couvre toutes les sources réseau prises en charge et fournit des exemples d'implémentation clairs pour les frameworks VideoCaptureCore et VideoCaptureCoreX.

## Types de caméras IP et de sources réseau pris en charge

Le SDK offre une compatibilité étendue avec diverses sources vidéo réseau, notamment :

* [Caméras compatibles ONVIF](onvif.md) — Standard industriel pour les produits de sécurité basés sur IP
* [Caméras RTSP](rtsp.md) — Caméras utilisant le protocole Real-Time Streaming Protocol
* Caméras HTTP MJPEG — Streaming Motion JPEG sur HTTP
* Caméras et flux UDP — Flux basés sur le protocole User Datagram Protocol
* [Caméras NDI](ndi.md) — Caméras utilisant la technologie Network Device Interface
* Serveurs et caméras SRT — Protocole Secure Reliable Transport
* Serveurs VNC — Virtual Network Computing pour la capture d'écran
* Flux RTMP — Sources Real-Time Messaging Protocol
* Flux HLS — Sources HTTP Live Streaming
* Sources vidéo HTTP — Divers flux vidéo basés sur HTTP

Chaque protocole offre des avantages spécifiques selon les exigences de votre application, des besoins de faible latence à la transmission vidéo de haute qualité.

Vous cherchez les URL RTSP d'une marque de caméra spécifique ? Parcourez notre [répertoire des marques de caméras IP](../../../camera-brands/index.md) pour des guides de connexion couvrant plus de 60 fabricants, dont Hikvision, Dahua, Axis, Reolink et bien d'autres.

## Implémentation de source universelle pour les protocoles réseau

Notre SDK propose une approche universelle pour la prise en charge de la plupart des sources vidéo réseau, notamment RTSP, RTMP, HTTP, etc. Cette flexibilité permet aux développeurs de se concentrer sur la logique applicative plutôt que sur les détails d'implémentation propres à chaque protocole.

=== "VideoCaptureCore"

    
    ### Implémentation de la source universelle dans VideoCaptureCore
    
    Pour les applications VideoCaptureCore, vous pouvez utiliser la classe IPCameraSourceSettings pour définir votre source vidéo réseau :
    
    ```cs
    // Créer et configurer la source réseau
    VideoCapture1.IP_Camera_Source = new IPCameraSourceSettings
    {
        URL = new Uri("rtsp://192.168.1.100:554/stream1"), // L'URL du flux (Uri, pas string)
        Login = "admin", // Identifiants d'authentification optionnels
        Password = "password123",
        AudioCapture = true, // Mettre à true pour inclure l'audio depuis la source
        Type = IPSourceEngine.Auto_VLC // Le moteur de traitement à utiliser
    };
    ```
    
    #### Types de moteurs disponibles
    
    Le SDK prend en charge plusieurs moteurs sous-jacents pour traiter les flux réseau, offrant de la flexibilité pour différents scénarios :
    
    * `Auto_VLC` — Utilise le moteur VLC, offrant une large prise en charge de protocoles et de compatibilité
    * `Auto_FFMPEG` — Utilise le moteur FFMPEG, offrant une prise en charge étendue de formats et de personnalisation
    * `Auto_LAV` — Utilise le moteur LAV, optimisé pour les environnements Windows
    
    ### Personnalisation des paramètres FFMPEG pour utilisateurs avancés
    
    Le SDK permet un contrôle précis des paramètres FFMPEG lorsque vous utilisez le moteur FFMPEG. Cela fournit aux utilisateurs avancés de nombreuses options de personnalisation :
    
    ```cs
    // Configurer des paramètres FFMPEG personnalisés
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("rtsp_transport", "tcp"); // Forcer le transport TCP
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("timeout", "3000000"); // Définir le timeout en microsecondes
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("buffer_size", "1000000"); // Ajuster la taille du tampon
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("max_delay", "500000"); // Définir le délai maximum autorisé
    ```
    
    Ces paramètres sont passés directement à la fonction avformat_open_input de FFMPEG, offrant des options de personnalisation poussées pour les performances de streaming réseau.
    

=== "VideoCaptureCoreX"

    
    ### Implémentation de la source universelle dans VideoCaptureCoreX
    
    Pour les applications VideoCaptureCoreX, l'approche utilise les patterns asynchrones modernes :
    
    ```cs
    // Préparer l'URL source avec authentification si nécessaire
    var uri = new Uri("rtsp://192.168.1.100:554/stream1");
    if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
    {
        uri = new UriBuilder(uri) { UserName = login, Password = password }.Uri;
    }
    
    // Créer la source universelle avec les paramètres souhaités
    var source = await UniversalSourceSettings.CreateAsync(
        uri,
        renderAudio: true); // Inclure le flux audio
    
    // Appliquer la source à l'objet de capture
    VideoCapture1.Video_Source = source;
    ```
    
    L'approche VideoCaptureCoreX offre un modèle asynchrone moderne basé sur les tâches, ce qui la rend idéale pour des applications à interface utilisateur réactive.
    


## Implémentation MJPEG à faible latence

Pour les applications nécessitant une latence minimale, comme la surveillance ou les systèmes de contrôle en temps réel, le SDK propose une implémentation MJPEG spécialisée à faible latence, avec une latence typique inférieure à 100 ms.

=== "VideoCaptureCore"

    
    ### Configuration MJPEG à faible latence dans VideoCaptureCore
    
    ```cs
    // Créer l'objet de paramètres
    var settings = new IPCameraSourceSettings
    {
        URL = new Uri("http://192.168.1.100/video.mjpg"),
        Login = "admin",
        Password = "pass123",
        // Utiliser le moteur MJPEG dédié à faible latence
        Type = IPSourceEngine.HTTP_MJPEG_LowLatency
    };
    
    // Appliquer les paramètres à l'objet VideoCaptureCore
    VideoCapture1.IP_Camera_Source = settings;
    ```
    
    Ce mode spécialisé contourne les traitements inutiles pour minimiser la latence, ce qui le rend idéal pour les applications sensibles au temps.
    

=== "VideoCaptureCoreX"

    
    ### Configuration MJPEG à faible latence dans VideoCaptureCoreX
    
    ```cs
    // Créer les paramètres de source HTTP MJPEG spécialisés
    var mjpeg = await HTTPMJPEGSourceSettings.CreateAsync(
        new Uri("http://192.168.1.100/video.mjpg"),
        "admin", // Nom d'utilisateur
        "pass123"
    );
    
    // Appliquer les paramètres à l'objet VideoCaptureCoreX
    VideoCapture1.Video_Source = mjpeg;
    ```
    
    L'implémentation MJPEG à faible latence est particulièrement utile pour les systèmes de surveillance, le monitoring à distance et les applications industrielles où la réduction du délai est critique.
    


## Implémentation Secure Reliable Transport (SRT)

SRT est un protocole moderne conçu pour le streaming vidéo fiable sur des réseaux imprévisibles. Il est particulièrement utile pour préserver la qualité dans des conditions réseau difficiles.

=== "VideoCaptureCoreX"

    
    ### Implémentation de la source SRT dans VideoCaptureCoreX
    
    ```cs
    // Créer les paramètres de source SRT avec l'URL du serveur — CreateAsync attend un System.Uri, pas une chaîne.
    var srt = await SRTSourceSettings.CreateAsync(new Uri("srt://streaming-server.example.com:7001"));
    
    // Appliquer la source SRT à l'objet de capture
    VideoCapture1.Video_Source = srt;
    ```
    
    SRT offre des avantages significatifs pour un streaming fiable sur des réseaux difficiles, avec sécurité intégrée, correction d'erreurs et contrôle de la congestion.
    


## Gestion des déconnexions réseau

Toute implémentation robuste de source réseau doit gérer les interruptions de connexion avec élégance. Le SDK fournit des mécanismes intégrés pour détecter et répondre aux déconnexions réseau.

=== "VideoCaptureCore"

    
    ### Implémentation de la gestion des déconnexions réseau dans VideoCaptureCore
    
    ```cs
    // DisconnectEventInterval se trouve sur IPCameraSourceSettings (accessible via VideoCapture1.IP_Camera_Source),
    // pas sur VideoCaptureCore lui-même. Configurez-le avant de démarrer l'aperçu/la capture.
    VideoCapture1.IP_Camera_Source.DisconnectEventInterval = TimeSpan.FromSeconds(5); // Vérifier toutes les 5 secondes
    
    // L'événement de déconnexion EST sur VideoCaptureCore
    VideoCapture1.OnNetworkSourceDisconnect += VideoCapture1_OnNetworkSourceDisconnect;
    
    // Implémenter le gestionnaire d'événement de déconnexion
    private void VideoCapture1_OnNetworkSourceDisconnect(object sender, EventArgs e)
    {
        Invoke((Action)(
            async () =>
            {
                await VideoCapture1.StopAsync();
                
                // Notifier l'utilisateur
                MessageBox.Show(this, "Network source disconnected!");
            }));
    }
    ```
    
    Une bonne gestion des déconnexions améliore la fiabilité de l'application et l'expérience utilisateur lors des fluctuations du réseau.
    


## Implémentation de la source VNC

Virtual Network Computing (VNC) permet de capturer les écrans de bureau distants comme sources vidéo, ce qui est utile pour les applications d'enregistrement d'écran et d'assistance à distance.

=== "VideoCaptureCoreX"

    
    ### Implémentation de la source VNC dans VideoCaptureCoreX
    
    ```cs
    // Créer l'objet de paramètres de source VNC
    var vncSettings = new VNCSourceSettings();
    
    // Configurer en utilisant l'hôte et le port
    vncSettings.Host = "remote-server.example.com";
    vncSettings.Port = 5900; // Port VNC par défaut
    
    // Ou configurer en utilisant le format URI
    // vncSettings.Uri = "vnc://remote-server.example.com:5900";
    
    // Définir les identifiants d'authentification
    vncSettings.Password = "secure-password";
    
    // Optionnel : configurer le comportement avancé du VNC
    vncSettings.Shared = true;        // Partager le bureau avec d'autres clients
    vncSettings.ViewOnly = true;      // Lecture seule, n'envoyer aucun événement d'entrée
    vncSettings.Incremental = true;   // Utiliser les mises à jour incrémentales du framebuffer
    vncSettings.RFBVersion = "3.8";   // Version du protocole RFB
    
    // Appliquer les paramètres à l'objet de capture
    VideoCapture1.Video_Source = vncSettings;
    ```
    
    L'implémentation de la source VNC offre une solution complète pour la capture de bureau distant, avec des paramètres de qualité et de performance personnalisables.
    
    * [Exemple complet de source VNC (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/VNC%20Source%20Demo)
    


## Bonnes pratiques pour les sources réseau

Pour des performances optimales lorsque vous travaillez avec des sources vidéo réseau :

1. **Gestion du tampon** : ajustez la mise en tampon selon la stabilité de la source et les exigences de latence
2. **Gestion des erreurs** : implémentez une gestion exhaustive des interruptions réseau
3. **Authentification** : utilisez toujours un stockage sécurisé des identifiants pour l'authentification de la caméra
4. **Mise en pool des connexions** : réutilisez les connexions lors de l'accès à plusieurs flux du même appareil
5. **Considération de la bande passante** : surveillez et gérez la consommation de bande passante, en particulier avec plusieurs sources

## Conclusion

Le Video Capture SDK pour .NET offre une prise en charge complète des caméras IP et des sources réseau, permettant aux développeurs de créer des applications vidéo sophistiquées. Avec la prise en charge de multiples protocoles et des options de configuration flexibles, il s'adapte à un large éventail de cas d'usage, de la vidéosurveillance au streaming multimédia.

Pour des exemples d'implémentation supplémentaires et des scénarios d'utilisation avancés, explorez notre dépôt GitHub contenant des exemples de code complets.

## Voir aussi

* [Enregistrement pré-événement](../../guides/pre-event-recording.md) — enregistrement par tampon circulaire avec détection de mouvement pour caméras IP et webcams
* [Capture de caméra IP vers MP4](../../video-tutorials/ip-camera-capture-mp4.md) — enregistrer la vidéo d'une caméra IP dans un fichier MP4
* [Lecteur de codes-barres et QR code](../../../mediablocks/Guides/barcode-qr-reader-guide.md) — scanner des codes-barres et QR codes depuis des flux vidéo de caméras IP

---

Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code plus complets et des applications de démonstration.
