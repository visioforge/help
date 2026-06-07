---
title: Streaming de caméras RTSP en C# .NET — modes UDP et TCP
description: Connectez-vous à des flux de caméras RTSP avec VisioForge Video Capture SDK. Faible latence, options de transport UDP/TCP et exemples de code C#.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - IP Camera
  - NDI Source
  - RTSP
  - ONVIF
  - NDI
  - UDP
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureCore
  - RTSPSourceSettings
  - IPCameraSourceSettings
  - IPSourceEngine

---

# Intégration de flux de caméras RTSP dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! info "Prise en charge multiplateforme"
    Le moteur `VideoCaptureCoreX` fonctionne sous **Windows, macOS, Linux, Android et iOS** via GStreamer ; le moteur classique `VideoCaptureCore` est réservé à Windows. Consultez la [matrice de prise en charge des plateformes](../../../platform-matrix.md) pour les détails sur les codecs et l'accélération matérielle, ainsi que le [guide de déploiement Linux](../../../deployment-x/Ubuntu.md) pour la configuration sur Ubuntu / NVIDIA Jetson / Raspberry Pi.

## Configuration de sources standard de caméras RTSP

L'implémentation de flux de caméras RTSP dans vos applications .NET offre un accès flexible aux caméras réseau et aux flux vidéo. Cette capacité puissante permet la surveillance en temps réel, les fonctions de vidéosurveillance et le traitement vidéo directement au sein de votre application.

=== "VideoCaptureCore"

    Pour des options de connexion supplémentaires et des protocoles alternatifs, consultez notre documentation détaillée sur les [caméras IP](index.md) qui couvre un large éventail d'approches d'intégration de caméras.

=== "VideoCaptureCoreX"

    
    ```cs
    // Créer l'objet de paramètres de source RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(new Uri("rtsp://192.168.1.1:554/live"), "login", "password", true /*capturer l'audio ?*/);
    
    // Affecter la source à l'objet VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    


## Optimisation pour le streaming RTSP à faible latence

La faible latence est critique pour de nombreuses applications temps réel, notamment la surveillance, les systèmes interactifs et la diffusion en direct. Notre SDK fournit des configurations spécialisées pour minimiser le délai entre la capture et l'affichage.

=== "VideoCaptureCore"

    
    Notre SDK inclut un mode dédié, spécifiquement conçu pour le streaming RTSP à faible latence. Lorsqu'il est correctement configuré, ce mode atteint généralement une latence inférieure à 250 millisecondes, ce qui le rend idéal pour les applications sensibles au temps.
    
    ```cs
    // Créer l'objet de paramètres de source.
    var settings = new IPCameraSourceSettings();
    
    // Configurer l'adresse IP, le nom d'utilisateur, le mot de passe, etc.
    // ...
    
    // Définir le mode RTSP LowLatency.
    settings.Type = IPSourceEngine.RTSP_LowLatency;
    
    // Définir le mode UDP ou TCP.
    settings.RTSP_LowLatency_UseUDP = false; // true pour utiliser UDP, false pour utiliser TCP
    
    // Affecter la source à l'objet VideoCaptureCore.
    VideoCapture1.IP_Camera_Source = settings;
    ```
    

=== "VideoCaptureCoreX"

    
    **NOUVEAU : mode à latence ultra-faible (60-120 ms)**
    
    VideoCaptureCoreX inclut désormais un mode dédié à faible latence qui atteint une latence totale de 60-120 ms — jusqu'à 10 fois plus rapide que le mode standard. Parfait pour la vidéosurveillance en temps réel, le monitoring interactif et les applications de sécurité.
    
    ```cs
    // Créer les paramètres de source RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream"), 
        "admin", 
        "password", 
        true); // activer l'audio
    
    // Activer le mode faible latence — optimise pour un délai minimal (60-120 ms)
    rtsp.LowLatencyMode = true;
    
    // Affecter la source à VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    
    **Fonctionnement :**
    - Définit le tampon anti-gigue RTSP à 80 ms (au lieu de 1000 ms par défaut)
    - Optimise la mise en tampon de la file d'attente interne (max 2 images)
    - Désactive le réordonnancement de paquets pour un délai minimal
    - Compromis : optimise la vitesse au détriment de la stabilité
    
    **Quand l'utiliser :**
    - ✓ Vidéosurveillance et monitoring en temps réel
    - ✓ Systèmes de sécurité en direct
    - ✓ Applications vidéo interactives
    - ✓ Systèmes de contrôle à distance
    - ✗ Enregistrement sur des réseaux instables (utilisez le mode par défaut)
    - ✗ Capture archivistique de longue durée (utilisez le mode par défaut)
    
    **Configuration avancée :**
    
    Pour un contrôle fin, vous pouvez configurer manuellement les paramètres de latence :
    
    ```cs
    var rtsp = await RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled);
    
    // Configuration manuelle de la faible latence
    rtsp.Latency = TimeSpan.FromMilliseconds(50);  // Taille de tampon personnalisée
    rtsp.BufferMode = RTSPBufferMode.None;          // Pas de mise en tampon anti-gigue
    rtsp.DropOnLatency = false;                     // Ne pas perdre d'images
    ```
    


## Exemples d'implémentation et applications de référence

Ces projets d'exemple illustrent des modèles d'implémentation RTSP pratiques et peuvent servir de point de départ pour votre propre développement. L'examen de ces exemples vous aidera à comprendre les bonnes pratiques d'intégration RTSP.

=== "VideoCaptureCore"

    
    - [Exemple principal du SDK (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [Exemple RTSP et autres sources (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/IP_Capture)
    - [Exemple principal du SDK (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)

=== "VideoCaptureCoreX"

    - [Exemple de source RTSP (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture)


## Résolution des problèmes de connexion RTSP

Lorsque vous travaillez avec des caméras RTSP, vous pouvez rencontrer des problèmes de connectivité liés à la configuration réseau, aux paramètres du pare-feu ou à l'authentification. Voici les facteurs clés à considérer :

- Vérifiez la connectivité réseau entre votre application et la caméra
- Assurez-vous que les identifiants d'authentification corrects sont fournis
- Vérifiez si des pare-feu bloquent les ports requis (généralement 554 pour RTSP)
- Envisagez d'utiliser TCP plutôt qu'UDP si vous rencontrez des pertes de paquets
- Testez les flux de la caméra avec VLC ou un outil similaire pour isoler les problèmes propres à l'application

Besoin de l'URL RTSP de votre caméra ? Parcourez notre [répertoire des marques de caméras IP](../../../camera-brands/index.md) pour des URL RTSP et des exemples de connexion par marque.

## Documentation associée

- [Plongée approfondie dans le protocole RTSP](../../../general/network-streaming/rtsp.md) — fonctionnement du RTSP, options de transport et architecture de streaming
- [Intégration des caméras IP ONVIF](onvif.md) — WS-Discovery, gestion des profils et contrôle PTZ
- [Intégration de sources NDI](ndi.md) — alternative vidéo-sur-IP professionnelle pour studio et diffusion
- [Tutoriel d'aperçu en direct d'une caméra IP](../../video-tutorials/ip-camera-preview.md) — pas-à-pas vidéo avec un exemple C# minimal
- [Enregistrer un flux RTSP vers MP4](../../video-tutorials/ip-camera-capture-mp4.md) — capturer n'importe quelle caméra IP dans un fichier
- [Lecteur RTSP Media Blocks](../../../mediablocks/Guides/rtsp-player-csharp.md) — alternative basée sur pipeline dans Media Blocks SDK
- [Grille RTSP multi-caméras (mur NVR)](../../../mediablocks/Guides/multi-camera-rtsp-grid.md) — mur d'aperçu en direct 4×4 pour WPF et MAUI
- [Reconnexion RTSP et basculement de repli](../../../general/network-sources/reconnection-and-fallback.md) — événements de déconnexion et `FallbackSwitch` image/texte/média sur tous les SDK
