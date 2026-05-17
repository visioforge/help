---
title: SDK .NET VisioForge — journal des modifications et versions
description: Historique des versions pour Video Capture, Media Player, Video Edit et Media Blocks SDK. Nouvelles fonctionnalités, corrections, changements d'API.
hide_table_of_contents: true
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - Capture
  - Playback
  - Streaming
  - Editing
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureCoreX
  - VideoView
  - MediaPlayerCoreX
  - DeviceEnumerator

---

# Journal des modifications

Modifications et mises à jour pour tous les SDK .Net.

## 2026.5.1

* [Core] Changement incompatible : les API de licence n'acceptent désormais que des octets bruts de certificat. Suppression des setters de certificat basés sur le chemin de fichier et sur les flux dans les licences partagées, les wrappers publics du SDK, les wrappers Windows hérités, les tests et la documentation de licence. Les applications doivent charger les fichiers `.vflicense` en mémoire et appeler `SetLicenseCertificateAsync(byte[])`.

## 2026.3.11

* [Core] Énumération des périphériques : les appareils Blackmagic ATEM et Web Presenter apparaissent désormais dans les listes habituelles de périphériques vidéo/audio au lieu d'être filtrés comme du matériel Decklink. Ces appareils utilisent des pilotes USB/UVC standard, et non le SDK Decklink. S'applique aux chemins d'énumération DirectShow et GStreamer.

## 2026.2.16

* [Media Blocks SDK .Net] Ajout de PreEventRecordingBlock pour l'enregistrement vidéo en tampon circulaire (pré-événement) avec durée de tampon configurable, vidage tenant compte des images-clés et arrêt automatique post-événement
* [Video Capture SDK .Net] VideoCaptureCoreX : ajout d'une API d'enregistrement pré-événement avec TriggerPreEventRecording, ExtendPreEventRecording, StopPreEventRecording et des méthodes de requête d'état
* [Video Capture SDK .Net] VideoCaptureCoreX : ajout de PreEventRecordingOutput pour configurer l'enregistrement en tampon circulaire avec prise en charge des conteneurs MP4, MPEG-TS et MKV

## 2026.2.12

* [Media Blocks SDK .Net] Ajout de l'événement OnNetworkSourceDisconnect pour détecter les déconnexions des sources réseau (RTSP, HTTP, SRT, NDI, RTMP, etc.) avec des informations d'erreur détaillées et l'URI de la source

## 2026.2.11

* [Media Player SDK .Net], [Media Blocks SDK .Net] Correction du routage du pipeline d'effets audio

## 2026.2.10

* [Core] Ajout de la prise en charge des chemins réseau UNC (SMB/Samba) pour les sources fichier dans tous les moteurs X, corrigeant les échecs de File.Exists() sur les partages réseau
* [Media Blocks SDK .Net], [Video Capture SDK .Net] Ajout de la prise en charge de la mise en évidence des clics de souris pour la capture d'écran avec abonnement/désabonnement automatique, entrée manuelle des clics et mise à jour des paramètres en temps réel

## 2026.2.8

* [Media Blocks SDK .Net] Ajout d'OverlayManagerImageSequence : superposition de séquence d'images avec durées par image, mise en boucle, animation position/taille, effets de fondu et prise en charge de l'easing
* [Media Blocks SDK .Net] Ajout de la classe de données ImageSequenceItem pour définir les images d'une séquence
* [Media Blocks SDK .Net] OverlayManagerBlock : ajout de méthodes de commodité pour les superpositions de séquences d'images (Video_Overlay_AddImageSequence, UpdateImageSequencePosition, AnimateImageSequence, ImageSequenceFadeIn/Out)
* [Core] Extraction d'OverlayManagerEasingHelper : fonctions d'easing partagées pour tous les types d'animation de superposition (Image, Fade, Pan, Squeezeback, ImageSequence)

## 2026.2.4

* [Media Blocks SDK .Net] Ajout de H264PushSourceBlock pour pousser des données H.264 encodées brutes dans un pipeline de décodage, avec conversion AVC vers byte-stream automatique et rebasement des PTS
* [Core] Ajout de RtspDescribeClient : client RTSP DESCRIBE multiplateforme léger pour la découverte rapide de flux (~100-200 ms), avec analyse SDP et prise en charge de l'authentification Basic/Digest
* [Core] RTSPSourceSettings : ajout d'un chemin de découverte RTSP rapide utilisant RtspDescribeClient
* [Core] RTSPRAWSourceSettings : ajout d'un chemin de découverte RTSP rapide utilisant RtspDescribeClient
* [Core] UniversalSourceSettings : ajout d'un chemin de découverte RTSP rapide pour les URI rtsp:// et rtsps://

## 2026.2.2

* [Core] VideoCaptureDeviceInfo : extension du remplissage du chemin de périphérique Windows pour prendre en charge les périphériques Media Foundation (MF) en plus de KS
* [Core] VideoCaptureDeviceInfo : correction d'un bug préexistant où la validation du chemin de périphérique V4L2 vérifiait la mauvaise variable
* [Core] VideoCaptureDeviceSourceSettings : ajout des méthodes statiques FindByDevicePath() pour restaurer des profils de caméra enregistrés par chemin de périphérique
* [Core] DeviceEnumerator : ajout de la méthode FindVideoSourceByDevicePathAsync() pour rechercher des périphériques par chemin
* [Media Blocks SDK .Net] Ajout de la prise en charge de la sortie sink MPEG-TS RIST (Reliable Internet Stream Transport)
* [Media Blocks SDK .Net] Ajout de la prise en charge de la sortie WebRTC WHIP (WebRTC-HTTP Ingestion Protocol)
* [Video Capture SDK .Net] Ajout d'une sortie de streaming WebRTC WHIP
* [Video Capture SDK .Net] Ajout d'une sortie de streaming RIST

## 2026.1.16

* [Media Blocks SDK .Net] BridgeVideoSourceSettings : ajout de la propriété DoTimestamp pour activer la génération d'horodatages frais dans des scénarios inter-pipelines

## 2026.1.15

* [Media Blocks SDK .Net] DecklinkVideoSinkSettings : le paramètre Mode du constructeur est désormais obligatoire pour éviter les problèmes de désaccord de fréquence d'images (il valait Unknown par défaut, ce qui produisait une sortie inattendue à 23,98 fps)
* [Media Blocks SDK .Net] DecklinkVideoSinkSettings : les propriétés DeviceNumber et Mode sont désormais en lecture seule pour garantir l'immuabilité
* [Core] DecklinkVideoOutputDialog (WPF) : ajout d'un sélecteur de Video Mode pour configurer la fréquence d'images de sortie

## 2026.1.12

* [Core] WPF VideoView : correction d'un crash (System.ExecutionEngineException) lors de la minimisation de la fenêtre pendant la lecture d'une superposition vidéo

## 2026.1.11

* [Media Blocks SDK .Net] RTSPSourceBlock : correction du gel vidéo lorsque la capture audio est désactivée pour des caméras à plusieurs flux audio
* [Media Blocks SDK .Net] RTSPRAWSourceBlock : ajout d'une gestion par fakesink pour les flux audio désactivés afin d'éviter les blocages du pipeline
* [Core] MediaInfoReaderCore : ajout de journalisation pour les flux audio, vidéo et RTP découverts
* [Core] MediaInfoReaderCore : correction d'une blocksize excessive (5 Mo) définie pour les sources RTSP, améliorant la vitesse de découverte

## 2026.1.10

* [Video Capture SDK .Net] DSFFMPEGEXEPipeOutput : correction du décalage de l'aperçu lors de la capture vidéo grâce à un traitement de pipe et de file d'attente optimisé
* [Video Capture SDK .Net] Sortie FFMPEG EXE : ajout d'optimisations d'encodage en temps réel pour VP8/VP9, correction du mode qualité MJPEG, amélioration des valeurs par défaut de H264MFSettings

## 2026.1.6

* [Video Capture SDK .Net] VideoCaptureCoreX : correction d'un problème de résolution de capture vidéo lorsque ResizeVideoEffect est appliqué

## 2025.12.12

* [Media Blocks SDK .Net] Ajout de PitchBlock pour le décalage de hauteur audio avec contrôle au demi-ton (plage de -12 à +12)
* [Media Player SDK X .Net] CDGSource : ajout de la prise en charge du décalage de hauteur via l'option EnablePitchShifting et le contrôle en temps réel PitchSemitones
* [Media Player SDK X .Net / Media Blocks SDK .Net] CDGSourceSettings : ajout de la prise en charge des archives ZIP pour les fichiers karaoké (paires MP3+CDG dans un ZIP)

## 2025.12.9

* Ajout de la prise en charge d'Uno Platform pour les plateformes de bureau et mobiles

## 2025.11.4

* Prise en charge de .Net 10 pour tous les SDK

## 2025.11.3

* Mise à jour de WPF VideoView : ajout des propriétés RotationAngle, RotateCrop et RotationStretch pour prendre en charge le rendu vidéo pivoté

## 2025.11.1

* [Media Blocks SDK .Net] Ajout de la prise en charge des groupes de superpositions synchronisés pour OverlayManagerBlock

## 2025.10.10

* [**SDK Windows**] Mise à jour de VideoEffectRotate avec une option sans rognage

## 2025.10.6

### 🚀 Fonctionnalité majeure : streaming RTSP à très faible latence

* **[Media Blocks SDK .Net]** Mode faible latence révolutionnaire pour les sources RTSP atteignant une **latence totale de 60 à 120 ms** (amélioration de 10 à 14× par rapport aux 1 à 2 secondes par défaut)
  * Ajout de la propriété `RTSPSourceSettings.LowLatencyMode` pour activer le streaming optimisé en une seule ligne
  * Optimisation automatique du pipeline : source RTSP (80 ms), tampons de file d'attente (10-20 ms) et contrôle de synchronisation du moteur de rendu
  * Intégration GStreamer : `latency=80ms`, `buffer-mode=0`, file d'attente `max-size-buffers=2` avec `leaky=downstream`
  * Idéal pour la surveillance en temps réel, les systèmes de sécurité, le monitoring en direct et les applications vidéo interactives

* **[Media Blocks SDK .Net]** RTSPSourceBlock amélioré avec une configuration complète de faible latence
  * Ajout de l'enum `RTSPBufferMode` avec 5 modes (None, Auto, Slave, Buffer, Synced) pour un contrôle fin du tampon de jitter
  * Ajout de l'enum `RTSPNTPTimeSource` (NTP, RunningTime, Clock) pour la synchronisation d'horodatage NTP dans les scénarios multi-caméras
  * Nouvelles propriétés : `LowLatencyMode`, `BufferMode`, `DropOnLatency`, `NTPSync`, `NTPTimeSource`
  * `QueueElement` optimisé avec configuration automatique en faible latence (2 images max, mode leaky downstream)

* **[Video Capture SDK X .Net]** Prise en charge complète du mode faible latence pour les sources RTSP
  * Compatible avec le moteur `VideoCaptureCoreX` sur toutes les plateformes
  * Même API simple : `RTSPSourceSettings.LowLatencyMode = true`
  * Fonctionne parfaitement avec la démo IP Capture et la démo RTSP MultiView

* **[Prise en charge multiplateforme]** Le streaming RTSP à faible latence est désormais disponible sur toutes les plateformes :
  * Windows (WPF, WinForms, Console, Blazor)
  * macOS (MAUI, Console)
  * Linux (Console, WPF avec Mono)
  * Android (MAUI, natif)
  * iOS (MAUI)

* **[Applications de démonstration]** 6 démos mises à jour avec des contrôles d'interface pour le mode faible latence :
  * Media Blocks SDK : RTSP Preview Demo (WPF), RTSP MultiView Demo (WinForms), MAUI RTSPViewer, Android RTSP Client
  * Video Capture SDK X : IP Capture (WPF), RTSP MultiView Demo (WinForms)
  * Toutes les démos incluent des cases à cocher faciles à utiliser ou la faible latence activée par défaut pour une expérience utilisateur optimale

* **[Documentation]** Guides et ressources complets :
  * `RTSP_LOW_LATENCY.md` — Guide d'utilisation complet du Media Blocks SDK avec exemples de code
  * `PIPELINE_LOW_LATENCY.md` — Analyse approfondie des composants du pipeline et techniques d'optimisation de la latence
  * `GSTREAMER_RTSP_EXAMPLES.md` — 7 exemples de pipelines GStreamer en ligne de commande pour tester
  * Documentation HELP officielle mise à jour avec une section faible latence et bonnes pratiques
  * Scripts de test GStreamer : Bash (Linux/macOS), Batch (Windows), PowerShell (Windows, recommandé)

* **[Tests]** Couverture de tests complète et validation :
  * 12 nouveaux tests unitaires pour la configuration faible latence de RTSPSourceSettings
  * Validé sur de véritables caméras IP sur toutes les plateformes
  * Tests de performance : Windows (85 ms), macOS (95 ms), Linux (80 ms), Android (110 ms), iOS (100 ms)

* **[Compatibilité ascendante]** Implémentation 100 % rétrocompatible :
  * Comportement par défaut inchangé — le code existant fonctionne sans modification
  * Le mode faible latence est opt-in via une propriété explicite
  * Aucun impact sur les performances lorsque le mode faible latence n'est pas utilisé
  * L'optimisation des files d'attente n'est appliquée que lorsque `LowLatencyMode=true`

## 2025.10.3

* [Media Blocks SDK .Net] Ajout de la prise en charge du sink DASH (Dynamic Adaptive Streaming over HTTP) avec les classes DASHSinkBlock et DASHOutput
* [Media Blocks SDK .Net] Ajout d'UniversalSourceBlockV2 avec une utilisation mémoire et des performances améliorées

## 2025.9.5

* [Video Fingerprinting SDK] Prise en charge améliorée des vidéos retournées

## 2025.9.3

* [Media Blocks SDK .Net] Ajout de la prise en charge des codes-barres DataMatrix via le bloc DataMatrixDecoderBlock

## 2025.9.1

* [Video Fingerprinting SDK] Prise en charge améliorée des vidéos retournées

## 2025.8.9

* [Video Capture SDK .Net] VideoCaptureCoreX : résolution du problème avec l'appel Snapshot_GetSK sur Android (mauvais espace colorimétrique)

## 2025.8.6

* [Moteurs X] Mise à jour du bloc source RTSP RAW. Ajout des propriétés WaitForKeyframe et SyncAudioWithKeyframe. Le bloc peut attendre les images-clés car certaines caméras peuvent ne pas les envoyer comme premières images.

## 2025.8.4

* [Moteurs X] Ajout de la prise en charge des sources NDI dans Live Video Compositor

## 2025.8.2

* [TOUS] Nouveau code du gestionnaire ONVIF dans VisioForge.Core.ONVIFX. Implémentation complète de divers services ONVIF, y compris la gestion d'appareils, Media v1/v2, PTZ, événements, imagerie, analytique, enregistrement et services de relecture.

## 2025.8.1

* [Media Player SDK] Ajout de la propriété PauseOnStop à MediaPlayerCoreX

## 2025.6.30

* [Moteurs X] Ajout de la prise en charge des GIF animés aux classes `ImageVideoSourceBlock`/`ImageVideoSourceSettings`
* [Moteurs X] Résolution des problèmes de démarrage retardé de fichier dans Live Video Compositor
* [Moteurs X] Mise à jour de l'API du mélangeur vidéo pour utiliser des GUID au lieu d'index entiers pour les sources vidéo

## 2025.6.27

* [Video Capture SDK] Résolution du problème avec le moteur RTSP faible latence sur certaines caméras

## 2025.6.5

* [Moteurs X] Résolution du problème de lecture des sources NDI sans flux audio

## 2025.6.3

* [Moteurs X] Mise à jour de la prise en charge des sources GenICam pour les caméras USB Vision. Ajout de la prise en charge des sources GenTL.

## 2025.6.2

* [Moteurs X] Ajout de la prise en charge du désentrelacement pour les sources vidéo Decklink entrelacées

## 2025.6.1

* [Live Video Compositor] Résolution du problème avec les sources de fichiers mises en pause au démarrage et reprises avec une erreur

## 2025.5.1

* [TOUS] Mise à jour des paquets de dépendances NuGet vers les dernières versions
* [Moteurs X] Résolution du problème de streaming RTMP vers un serveur personnalisé

## 2025.4.8

* [TOUS] Ajout de l'API Absolute Move à la classe `ONVIFDeviceX`. Vous pouvez utiliser cette API pour déplacer la caméra ONVIF vers la position absolue spécifiée.

## 2025.2.24

* [Moteur X] Par défaut, l'énumération des périphériques Media Foundation est désactivée. Vous pouvez l'activer en utilisant la propriété `DeviceEnumerator.Shared.IsEnumerateMediaFoundationDevices`.

## 2025.2.18

* [Media Player SDK.Net] Ajout de la prise en charge de la boucle pour le moteur multiplateforme.
* [TOUS] Mise à jour de la sortie du moteur RTSP-X, correction d'un problème de crash avec la sortie RTSP et les reconnexions fréquentes du lecteur VLC
* [Moteurs X] Modification de la prise en charge du détecteur de visages pour utiliser l'interface IFaceDetector
* [Live Video Compositor] Correction de problèmes d'enregistrement avec une vue vidéo personnalisée attachée à une entrée vidéo
  
## 2025.2.9

* [Moteurs X] Mise à jour de la vitesse de connexion NDI

## 2025.2.4

* [Moteurs X] Ajout de RTSP Server Media Block et de RTSPServerOutput au Video Capture SDK. Vous pouvez utiliser RTSPServerBlock pour créer un serveur RTSP et y diffuser des flux audio et vidéo.

## 2025.2.1

* [Moteurs X] Ajout de la prise en charge des encodeurs NVENC et AMF AV1

## 2025.1.25

* [Windows] Résolution du problème HTTPS avec les certificats SSL non chargés

## 2025.1.22

* [Windows] Résolution du problème de sources ONVIF manquantes lors de l'énumération sur un PC avec plusieurs interfaces réseau
* [Media Blocks SDK .Net] Ajout de l'événement `OnEOS` à la classe `MediaBlockPad`. Vous pouvez utiliser cet événement pour obtenir l'événement EOS (fin de flux) du media block. Cela peut être utile si vous avez plusieurs sources de fichiers de durées différentes et que vous devez arrêter le pipeline lorsque la première source se termine.
* [Media Blocks SDK .Net] Ajout de la méthode `SendEOS` à la classe `MediaBlocksPipeline`. Vous pouvez utiliser cette méthode pour envoyer l'événement EOS (fin de flux) au pipeline.
  
## 2025.1.18

* [NuGet] Les paquets `VisioForge.Core.UI.Apple`, `VisioForge.Core.UI.Android` et `VisioForge.Core.UI.WinUI` sont fusionnés dans le paquet `VisioForge.DotNet.Core`. Tous les espaces de noms sont identiques.
* [Media Blocks SDK .Net] Ajout de la propriété `ZOrder` aux classes `LVCVideoInput` et `LVCVideoAudioInput`. Vous pouvez utiliser cette propriété pour définir l'ordre Z de l'entrée vidéo.

## 2025.1.14

* [NuGet] Les paquets `VisioForge.Core.UI.WPF` et `VisioForge.Core.UI.WinForms` sont fusionnés dans le paquet `VisioForge.DotNet.Core`. Dans les projets WPF, vous devez mettre à jour le code XAML si les noms d'assemblage sont utilisés. Tous les espaces de noms sont identiques.

## 2025.1.11

* [Video Capture SDK .Net] Résolution d'un problème de l'encodeur QSV H264 FFMPEG lié à des symboles erronés dans les paramètres

## 2025.1.7

* [Multiplateforme] Ajout de la prise en charge des sources `libcamera` pour Linux/Raspberry Pi.

## 2025.1.5

* [Multiplateforme] Amélioration de la lecture image précédente dans Media Player SDK .Net (moteur multiplateforme)

## 2025.1.4

* [Multiplateforme] Résolution du problème d'initialisation du plugin AMD AMF

## 2025.1.1

* [Multiplateforme] Résolution d'une fuite mémoire dans `OverlayManagerImage`

## 2025.1.0

* [Multiplateforme] Mise à jour du moteur Live Video Compositor. Amélioration de la prise en charge Decklink pour l'entrée et la sortie. Amélioration des performances. Les nouvelles classes du moteur se trouvent dans l'espace de noms `VisioForge.Core.LiveVideoCompositorV2`.

## 2025.0.29

* [Multiplateforme] Le moteur de rendu vidéo par défaut sur Windows a été changé en DirectX 11

## 2025.0.17

* [Media Blocks SDK .Net] Ajout de la prise en charge des sources libCamera (utilisable sur Raspberry Pi)

## 2025.0.16

* [Media Blocks SDK .Net] Résolution du problème d'ajout de plusieurs AudioRendererBlocks au pipeline

## 2025.0.14

* [Media Blocks SDK .Net] Ajout de la classe « PushJPEGSourceSettings » pour configurer la source JPEG du « PushSourceBlock ». Vous pouvez utiliser cette classe pour définir les paramètres de la source JPEG du « PushSourceBlock ». Un exemple « video-from-images » a également été ajouté.

## 2025.0.7

* [TOUS] Résolution des problèmes de capture de fenêtre dans les SDK multiplateformes
* [Media Blocks SDK .Net] Ajout de l'exemple Bridge Source Switch

## 2025.0.5

* [iOS] Résolution des problèmes de vitesse de lecture pour certains fichiers vidéo
* [iOS] Ajout de la prise en charge du simulateur iOS pour tous les SDK. La source caméra n'est pas prise en charge dans le simulateur.

## 2025.0.3

* [MacOS] Résolution du problème de stride incorrect pour les vidéos de caméras verticales sur MacOS
* [Video Capture SDK .Net] Résolution du problème de couleur d'arrière-plan pour la superposition de texte défilant

## 2025.0

* [TOUS] Prise en charge de .Net 9
* [Media Blocks SDK .Net] Ajout d'`AVIOutputBlock` pour enregistrer des flux vidéo et audio au format AVI
* [Media Blocks SDK .Net] Le constructeur de `TeeBlock` accepte désormais le type de média en paramètre
* [Video Capture SDK .Net] Ajout des méthodes `Video_CaptureDevice_SetDefault` et `Audio_CaptureDevice_SetDefault` à la classe `VideoCaptureCore`. Vous pouvez utiliser ces méthodes pour définir les périphériques de capture vidéo et audio par défaut
* [Multiplateforme] Amélioration des performances de rendu vidéo `Metal` sur les appareils Apple
* [Tous] Amélioration des performances des opérations de traitement vidéo courantes dans les SDK classiques Windows
* [CV] Ajout de détecteurs de visages DNN pour `Media Blocks SDK .Net` et `Video Capture SDK .Net`
* [Mobile] Amélioration de la compatibilité AOT pour iOS et Android
* [WinUI] Amélioration des performances du rendu vidéo `WinUI`
* [Media Blocks SDK .Net] Ajout des méthodes `GetLastFrameAsSKBitmap` et `GetLastFrameAsBitmap` à `VideoSampleGrabberBlock` pour obtenir la dernière image sous forme de `SkiaSharp.SKBitmap` ou de `System.Drawing.Bitmap`
* [Video Capture SDK .Net] `VideoCaptureCore` : ajout de la propriété `AddFakeAudioSource` à `FFMPEGEXEOutput`. La propriété `Network_Streaming_Audio_Enabled` de `VideoCaptureCore` doit être définie sur false pour utiliser cet audio factice.
* [TOUS] Amélioration des performances de VideoView WinUI (et MAUI sur Windows)
* [Video Capture SDK .Net] `VideoCaptureCore` : ajout de l'API `PIP_Video_CaptureDevice_CameraControl_` pour contrôler les paramètres de caméra en mode image dans l'image
* [Moteurs X] Ajout de la prise en charge des en-têtes pour les sources HTTP créées à l'aide de la classe `HTTPSourceSettings`
* [Moteurs X] Mise à jour des exemples Avalonia avec des projets pour macOS, Linux et Windows
* [Moteurs X] Ajout de paquets NuGet redist pour macOS et MacCatalyst (y compris MAUI)
* [Video Capture SDK .Net] `VideoCaptureCore` : ajout de la prise en charge du chemin de périphérique pour l'API `PIP_Video_CaptureDevice_CameraControl`
* [Video Capture SDK .Net] `VideoCaptureCore` : ajout de la propriété `FFMPEG_MaxLoadTimeout` pour les sources caméra IP. Elle vous permet de définir le délai maximal d'attente pour que la source FFMPEG charge le flux
* [Moteurs X] Mise à jour de la prise en charge Linux pour les périphériques audio `ALSA`, `PulseAudio` et `PipeWire`
* [Moteurs X] Mise à jour de la prise en charge Linux pour les périphériques `V4L2`
* [Moteurs X] Les exemples Avalonia ont été modifiés pour adopter une structure moderne à projet unique
* [Moteurs X] Résolution du problème de crash de `MAUI` sur Windows après la mise à jour de `SkiaSharp`
* [Moteurs X] Résolution du problème de crash de `TextureView` sur Android dans les applications `MAUI`
* [Moteurs X] Résolution du problème de lecture pour les sources http utilisant `UniversalSourceBlock`
* [Moteurs X] Ajout de l'exemple Mobile Streamer pour Android
* [Moteurs X] Ajout de la prise en charge d'`OverlayManagerBlock` pour Android (désormais disponible pour toutes les plateformes)
* [Video Capture SDK .Net] `VideoCaptureCoreX` : ajout des propriétés `CustomVideoProcessor`/`CustomAudioProcessor` pour tous les formats de sortie. Vous pouvez utiliser ces propriétés pour définir des blocs personnalisés de traitement vidéo/audio pour le format de sortie.
* [Media Blocks SDK .Net] Ajout de `KeyFrameDetectorBlock` pour détecter les images-clés dans les flux vidéo (H264, H265, VP8, VP9, AV1, etc.)
* [Media Blocks SDK .Net] Correction d'un problème de licence pour la classe `LiveVideoCompositor`
  
## 15.10.0

* [Windows] Mise à jour de l'API de capture de fenêtre pour ne capturer que la fenêtre parente spécifiée par défaut. Ajout de la méthode `UpdateHotkey` à la classe `WindowCaptureForm` pour mettre à jour le raccourci clavier du formulaire de capture de fenêtre.
* [Moteurs X] Meilleure compatibilité AOT pour les paramètres MAUI par défaut sur iOS.
* [Media Blocks SDK .Net] Ajout de `DNNFaceDetectorBlock` pour détecter les visages et les flouter ou pixéliser à l'aide d'OpenCV et de modèles DNN.
* [Media Blocks SDK .Net] Ajout de `MKVOutputBlock` pour enregistrer des flux vidéo et audio au format MKV.
* [Moteurs X] Meilleure prise en charge du changement dynamique de taille de la source vidéo dans les applications MAUI.
* [Moteurs X] Résolution d'un problème avec deux VU-mètres ou plus dans le même pipeline.
* [Moteurs X] Résolution du problème de volume/muet avec le mélangeur audio du moteur Live Video Compositor.
* [Moteurs X] La source `Spinnaker` pour les caméras `FLIR`/`Teledyne` est incluse dans le paquet principal et ne nécessite plus de plugin supplémentaire.
* [Video Capture SDK .Net] Résolution du problème avec l'API `SeparateCapture` lorsqu'aucune `VideoView` n'était utilisée.
* [Moteurs X] Le constructeur `MediaBlocksPipeline` n'a plus le paramètre `live`. Pour des pipelines plus personnalisables, les moteurs de rendu vidéo et audio ont obtenu la propriété `IsSync` (`true` par défaut).
* [Moteurs X] Résolution du crash de `VideoViewTX` dans les applications MAUI Android.
* [Moteurs X] L'interface `IVideoEncoder` a été ajoutée à la classe `MPEG2VideoEncoder`. Cela permet d'utiliser `MPEG2VideoEncoder` avec `MPEGTSOutput`, `AVIOutput` et d'autres classes de sortie.
* [Moteurs X] Résolution du problème de capture de fenêtre utilisant la classe `ScreenCaptureD3D11SourceSettings`. Si le rectangle était incorrect ou non spécifié, cela provoquait une erreur.
* [Moteurs X] Le moteur de rendu `Metal` a été ajouté au SDK pour les appareils Apple et est utilisé par défaut pour iOS et MAUI.
* [Media Blocks SDK .Net] Ajout de l'exemple MAUI Screen Capture.
* [Video Capture SDK .Net] VideoCaptureCore : ajout de la propriété `VLC_CustomDefaultFrameRate` à `IPCameraSourceSettings` pour définir une fréquence d'images personnalisée pour la source caméra IP VLC si la source ne fournit pas la bonne fréquence d'images.
* [Media Blocks SDK .Net] `RTSPSourceBlock` : si la source RTSP possède de l'audio mais que vous avez désactivé le flux audio dans `RTSPSourceSettings`, le SDK ajoutera automatiquement un null renderer pour éviter les avertissements.
* [TOUS] Résolution du problème avec l'appel `VideoFrameX.ToBitmap()` (mauvais espace colorimétrique)
* [Windows] Mise à jour de la prise en charge KLV dans la sortie MPEG-TS
* [Windows] Résolution du problème de sérialisation de MediaPlayerCore
* [TOUS] La classe de paramètres du moteur de rendu vidéo ne contient plus de couleur d'arrière-plan. Utilisez à la place la propriété de couleur d'arrière-plan de VideoView.
* [Moteurs X] Mise à jour des bibliothèques GStreamer
* [Moteurs X] Résolution des problèmes de rendu vidéo sur Android et iOS
* [Moteurs X] Correction du crash iOS lors de l'utilisation de VideoViewGL
* [Moteurs X] Ajout de l'encodeur AAC par défaut pour iOS
* [Moteurs X] Mise à jour de la source caméra iOS pour la prise en charge de haute fréquence d'images
* [Windows] Mise à jour de la source VLC — amélioration de la vitesse de chargement de fichier
* [Media Blocks SDK .Net] : ajout de `UniversalDemuxBlock` qui permet de démultiplexer les flux vidéo et audio depuis un fichier aux formats MP4, MKV, AVI, MOV, TS, VOB, FLV, OGG et WebM
* [Windows] Résolution des problèmes de stabilité FFMPEG
* [Moteurs X] Résolution du problème de source audio loopback utilisant VideoCaptureCoreX et la capture audio vers fichier
* [Moteurs X] Ajout de la prise en charge des sources et puits SRT dans Media Blocks SDK .Net et Video Capture SDK .Net
* [Video Capture SDK .Net] VideoCaptureCore : la méthode `IP_Camera_ONVIF_ListSourcesAsyncEx` a obtenu une version surchargée avec un callback pour une interface utilisateur plus réactive
* [Moteurs X] Mise à jour de la compatibilité avec la source RTSP
* [Moteurs X] `Changement d'API incompatible`. À partir de cette mise à jour, le SDK utilise les implémentations de l'interface `IAudioRendererSettings` pour la configuration de la sortie audio. La sortie WASAPI a obtenu ses classes de configuration personnalisées. Les propriétés Output_AudioDevice de type `VideoCaptureCoreX`/`MediaPlayerCoreX` sont passées à `IAudioRendererSettings`. Vous pouvez créer une instance de la classe `AudioRendererSettings` à partir d'`AudioOutputDeviceInfo` en utilisant le constructeur par défaut.
* [Moteurs X] Résolution du problème de sources Media Foundation manquantes lors de l'énumération des périphériques
* [Moteurs X] Résolution de problèmes de connexion audio avec la source RTSP dans certaines situations
* [Moteurs X] Ajout de la démo RTSP Preview à Media Blocks SDK .Net
* [Windows] Sorties et source FFMPEG mises à jour vers FFMPEG v7.0.
* [Moteurs X] Correction de rares crashes dans la source RTSP lorsque les informations de la caméra ne sont pas disponibles pour une raison quelconque (problème réseau)
* [Moteurs X] Résolution d'un problème avec l'utilisation du moteur de rendu audio `WASAPI/WASAPI2`
* [Moteurs X] Résolution d'un problème avec la source audio loopback sur Windows
* [Moteurs X] Amélioration des performances et de la stabilité du rendu vidéo iOS
* [Moteurs X] Ajout de la sortie AWS S3 Sink pour Media Blocks SDK .Net
* [Moteurs X] Ajout de la prise en charge des caméras Allied Vision USB3/GigE dans Media Blocks SDK .Net et Video Capture SDK .Net

## 15.9

* [Moteurs X] Résolution du mauvais rapport d'aspect avec l'effet/bloc de redimensionnement vidéo
* [Moteurs X] Mise à jour de la redist GStreamer
* [Moteurs X] Ajout de la prise en charge des caméras Basler USB3/GigE dans Media Blocks SDK .Net et Video Capture SDK .Net
* [Video Edit SDK .Net] VideoEditCoreX : la classe TextOverlay a été modifiée pour utiliser des paramètres de police basés sur SkiaSharp. De plus, vous pouvez définir le nom de fichier de police personnalisé ou configurer tous les paramètres de rendu via un SKPaint personnalisé.
* [Windows] Ajout de la prise en charge des flux dans `MediaInfoReader`. Vous pouvez obtenir les informations d'un fichier vidéo/audio depuis un flux (base de données, réseau, mémoire, etc.).
* [Moteurs X] Mise à jour du moteur Live Video Compositor, qui améliore la prise en charge des sources de fichiers
* [Video Capture SDK .Net] Ajout d'un détecteur de caméra couverte dans la `Computer Vision Demo` et le paquet `VisioForge.Core.CV`
* [Moteurs X] Ajout d'une API pour obtenir des instantanés depuis des fichiers vidéo via MediaInfoReaderX : GetFileSnapshotBitmap, GetFileSnapshotSKBitmap, GetFileSnapshotRGB
* [Moteurs X] Prise en charge iOS dans les exemples MAUI
* [Moteurs X] Résolution d'une fuite mémoire pour les sources RTSP
* [Media Player SDK .Net] MediaPlayerCore : ajout de la prise en charge des flux de données dans les fichiers vidéo via le moteur de source FFMPEG. Ajoutez l'événement OnDataFrameBuffer pour obtenir les trames de données (KLV ou autres) depuis le fichier vidéo.
* [Video Capture SDK .Net] VideoCaptureCore : ajout de la prise en charge des flux de données dans les fichiers vidéo via le moteur de source FFMPEG d'IP Capture. Ajoutez l'événement OnDataFrameBuffer pour obtenir les trames de données (KLV ou autres) depuis le flux réseau UDP MPEG-TS ou autre source prise en charge.
* [Video Capture SDK .Net] VideoCaptureCore : ajout de la propriété FFMPEG_CustomOptions à la classe IPCameraSourceSettings. Cette propriété vous permet de définir des options FFMPEG personnalisées pour la source caméra IP
* [Windows] Correction du problème de gel avec la source FFMPEG lorsqu'une connexion réseau est perdue
* [Media Blocks SDK .Net] Ajout de la démo RTSP MultiView en synchronisation
* [Moteurs X] Ajout de la prise en charge des caméras FLIR/Teledyne (USB3Vision/GigE) via le SDK Spinnaker
* [Video Edit SDK .Net] VideoEditCoreX : ajout de la prise en charge de l'utilisation de `Stream` .Net comme source d'entrée
* L'interface IAsyncDisposable a été ajoutée à toutes les classes principales du SDK. L'appel `DisposeAsync` doit être utilisé pour libérer les objets principaux via des méthodes asynchrones.  
* [Video Capture SDK .Net] VideoCaptureCoreX : résolution des problèmes de capture vidéo Android (parfois démarrée une seule fois)
* [Media Blocks SDK .Net] Ajout de l'exemple de streaming HLS
* [Video Capture SDK .Net] VideoCaptureCore : résolution d'un crash si `multiscreen` est activé et que les écrans sont ajoutés sous forme de handles de fenêtre (WinForms)
* [Moteurs X] Amélioration de la vitesse de rendu vidéo MAUI
* [Moteurs X] Résolution de problèmes de lecture multimédia (décodage) dans MAUI Android
* [Moteurs X] Résolution d'un problème avec les sources webcam H264 (parfois non connectées)
* [Moteurs X] Résolution d'un problème de lecture de flux audio dans le moteur Live VideoCompositor
* [Media Blocks SDK .Net] Résolution d'un problème de mauvaise qualité audio lors du mixage avec le moteur Live Video Compositor
* [Media Blocks SDK .Net] Ajout de la sortie Decklink et de la source de fichier dans l'exemple Live Video Compositor
* [Media Player SDK .Net] MediaPlayerCore : ajout de la prise en charge des fichiers MPEG-TS en croissance pour le moteur VLC. Vous pouvez lire des fichiers MPEG-TS en croissance pendant leur enregistrement
  
## 15.8

* [Moteurs X] [Changement d'API incompatible] DeviceEnumerator ne peut désormais être utilisé que via la propriété `DeviceEnumerator.Shared`. Un seul énumérateur par application est requis. Les objets DeviceEnumerator utilisés par l'API ont été supprimés
* [Moteurs X] [Changement d'API incompatible] Android Activity n'est plus requis pour créer des moteurs SDK
* [Moteurs X] [Changement d'API incompatible] Les moteurs X requièrent des étapes supplémentaires d'initialisation et de désinitialisation. Pour initialiser le SDK, utilisez l'appel `VisioForge.Core.VisioForgeX.InitSDK()`. Pour désinitialiser le SDK, utilisez l'appel `VisioForge.Core.VisioForgeX.DestroySDK()`. Vous devez initialiser le SDK avant toute utilisation d'une classe du SDK et le désinitialiser avant la sortie de l'application.
* [Windows] Amélioration des performances de rendu vidéo MAUI sur Windows
* [Windows] Ajout d'une mise en évidence de la souris pour les sources de capture d'écran
* [Windows] Résolution d'un problème d'appel CallbackOnCollectedDelegate avec la classe BasicWindow
* [Avalonia] Résolution d'un problème avec le redimensionnement de VideoView Avalonia
* [Moteurs X] Ajout des propriétés StartPosition et StopPosition à UniversalSourceSettings. Vous pouvez utiliser ces propriétés pour définir les positions de début et de fin de la source fichier.
* [TOUS] Résolution du problème avec les mots de passe contenant des caractères spéciaux utilisés pour les sources RTSP
* [TOUS] Résolution du rare problème de retournement vidéo avec le moteur Virtual Camera SDK
* [TOUS] Le filtre VisioForge MJPEG Decoder a été supprimé des paquets NuGet du SDK. Vous pouvez l'ajouter de manière optionnelle à votre projet par copie de fichier ou déploiement par enregistrement COM.
* [Moteurs X] Correction d'une fuite mémoire dans OverlayManager
* [Media Blocks SDK .Net] Résolution du problème avec VideoSampleGrabberBlock, option SetLastFrame
* [Video Capture SDK .Net] VideoCaptureCoreX : les sources audio WASAPI et WASAPI2 peuvent désormais être utilisées avec le moteur VideoCaptureCoreX
* [Moteurs X] DeviceEnumerator a obtenu des événements pour notifier l'ajout/la suppression de périphériques : OnVideoSourceAdded, OnVideoSourceRemoved, OnAudioSourceAdded, OnAudioSourceRemoved, OnAudioSinkAdded, OnAudioSinkRemoved
* [Moteurs X] Ajout de la prise en charge d'un gestionnaire d'erreur personnalisé pour les moteurs MediaBlocks, VideoCaptureCoreX et MediaPlayerCoreX. Utilisez l'interface IMediaBlocksPipelineCustomErrorHandler et la méthode SetCustomErrorHandler pour définir un gestionnaire d'erreur personnalisé.
* [Video Capture SDK .Net] VideoCaptureCoreX : résolution du problème d'erreur d'index de périphérique incorrect pour les sources vidéo KS (Windows)
* [Video Capture SDK .Net] VideoCaptureCore : ajout de la propriété Virtual_Camera_Output_AlternativeAudioFilterName pour définir un filtre audio personnalisé pour la sortie Virtual Camera SDK
* [Video Edit SDK .Net] VideoEditCore : ajout de la propriété Virtual_Camera_Output_AlternativeAudioFilterName pour définir un filtre audio personnalisé pour la sortie Virtual Camera SDK
* [Media Player SDK .Net] MediaPlayerCore : ajout de la propriété Virtual_Camera_Output_AlternativeAudioFilterName pour définir un filtre audio personnalisé pour la sortie Virtual Camera SDK
* [Video Capture SDK .Net] VideoCaptureCoreX : ajout de la prise en charge du streaming NDI et application d'exemple.
* [Media Blocks SDK .Net] Ajout du bloc BufferSink pour obtenir des trames vidéo/audio du pipeline
* [Media Blocks SDK .Net] Ajout de la classe CustomMediaBlock pour créer des media blocks personnalisés pour tout élément GStreamer
* [Media Blocks SDK .Net] Ajout de la méthode UpdateChannel pour mettre à jour le canal de la source ou du puits Bridge
* [Media Player SDK .Net] MediaPlayerCore : mise à jour de l'effet Tempo.
* [Moteurs X] Mise à jour de l'énumérateur de périphériques. Suppression du dialogue indésirable du pare-feu lors de la liste des sources NDI.
* [Moteurs X] Correction d'un problème avec le mélangeur vidéo lors de l'ajout/suppression de sources vidéo.
* [Media Blocks SDK .Net] Ajout des blocs VideoCropBlock et VideoAspectRatioCropBlock pour rogner les trames vidéo.
* [Media Blocks SDK .Net] Résolution du problème de mauvaise fréquence d'images avec VideoRateBlock.
* [Tous] Résolution d'un problème avec l'effet audio Tempo.
* [Video Capture SDK .Net] VideoCaptureCore : ajout de la prise en charge du moteur de rendu audio WASAPI pour le moteur VideoCaptureCore.

## 15.7

* [TOUS] Prise en charge de .Net 8
* [Video Capture SDK .Net] VideoCaptureCore : correction du problème de l'événement OnNetworkSourceDisconnect appelé deux fois.
* [Moteurs X] Ajout de l'encodeur vidéo MPEG-2.
* [Moteurs X] Ajout de l'encodeur audio MP2.
* [Moteurs X] Résolution des problèmes d'énumération Decklink.
* [Moteurs X] Les paramètres par défaut VP8/VP9 sont passés à l'enregistrement en direct.
* [Moteurs X] Ajout de la prise en charge de l'encodeur vidéo DNxHD.
* [Video Capture SDK .Net] VideoCaptureCoreX : correction d'un problème avec le réglage du format de la source audio (régression).
* [Video Capture SDK .Net] VideoCaptureCoreX : résolution d'un problème de rendu natif WPF avec une fenêtre pop-up.
* [Tous] Prise en charge d'Avalonia 11.0.5.
* [Video Capture SDK .Net] VideoCaptureCoreX : résolution de problèmes de licence.
* [Video Capture SDK .Net] VideoCaptureCore : la méthode Start/StartAsync renvoie false si le périphérique de capture vidéo est déjà utilisé par une autre application.
* [Tous] Mise à jour de la source VLC (libVLC 3.0.19).
* [Tous] Mise à jour des sources et encodeurs FFMPEG. Résolution du problème de dépendances MSVC manquantes.
* [Video Capture SDK] Mise à jour du moteur ONVIF.
* [SDK multiplateformes] Mise à jour de la source Decklink. Résolution du problème de nom de périphérique incorrect.
* [Tous] Mises à jour de sécurité SkiaSharp.
* [SDK multiplateformes] Mise à jour d'Overlay Manager. Ajout de la classe OverlayManagerDateTime pour dessiner la date/heure actuelle et du texte personnalisé.
* [SDK multiplateformes] Mise à jour d'OverlayManagerImage. Résolution du problème d'utilisation de System.Drawing.Bitmap.
* [TOUS] VideoCaptureCore : résolution du rare crash avec WinUI VideoView
* [Video Capture SDK .Net] VideoCaptureCore : mise à jour de la sortie FFMPEG.exe. Amélioration de la prise en charge des encodeurs x264 et x265 des builds FFMPEG personnalisés.

## 15.6

* [Video Capture SDK .Net] VideoCaptureCore : amélioration des performances de rognage vidéo sur les CPU modernes
* [TOUS] VideoCaptureCore, MediaPlayerCore, VideoEditCore : ajout de la méthode statique CreateAsync qui peut être utilisée à la place du constructeur pour créer des moteurs sans décalage d'interface.
* [Video Capture SDK .Net] VideoCaptureCore : résolution de problèmes avec le rognage vidéo.
* [Video Capture SDK .Net] VideoCaptureCoreX : ajout de l'API des superpositions vidéo. La démo Overlay Manager montre comment l'utiliser.
* [Video Capture SDK .Net] Amélioration de la détection des encodeurs matériels. Si vous avez plusieurs GPU, parfois seul le GPU principal peut être utilisé pour l'encodage vidéo.
* [SDK multiplateformes] Mise à jour d'Avalonia VideoView. Résolution du problème de recréation de VideoView.
* [Media Player SDK .Net] MediaPlayerCoreX : résolution du problème de démarrage avec la version Android du moteur MediaPlayerCoreX.
* [Media Player SDK .Net] MediaPlayerCore : la propriété Video_Stream_Index a été remplacée par les méthodes Video_Stream_Select/Video_Stream_SelectAsync.
* [Media Player SDK .Net] MediaPlayerCoreX : ajout de la méthode Video_Stream_Select.
* [Video Capture SDK .Net] VideoCaptureCore : la propriété Network_Streaming_WMV_Maximum_Clients est déplacée vers la classe WMVOutput. Vous pouvez définir le nombre maximal de clients pour la sortie réseau WMV.
* [Tous] Mise à jour du rendu WPF. Amélioration des performances pour les vidéos 4K et 8K.
* [Video Capture SDK .Net] VideoCaptureCoreX : résolution d'un problème avec plusieurs sorties utilisées.
* [Video Capture SDK .Net] VideoCaptureCoreX : résolution d'un problème avec l'événement OnAudioFrameBuffer.
* [Video Capture SDK .Net] Modification de la source Decklink pour améliorer la vitesse de démarrage. La méthode Decklink_CaptureDevices a été remplacée par Decklink_CaptureDevicesAsync asynchrone.
* [Media Player SDK .Net] MediaPlayerCoreX : ajout des propriétés Custom_Video_Outputs/Custom_Audio_Outputs pour définir des moteurs de rendu vidéo/audio personnalisés
* [Media Player SDK .Net] MediaPlayerCoreX : ajout de la démo Decklink Output Player (WPF)
* [Video Edit SDK .Net] Ajout de la démo Multiple Audio Tracks (WPF)
* [Video Edit SDK .Net] Mise à jour de la sortie MP4 pour plusieurs pistes audio
* [SDK multiplateformes] Mise à jour de l'énumérateur de périphériques
* [Video Capture SDK .Net] Résolution d'un problème avec le VU-mètre dans le moteur multiplateforme
* [SDK multiplateformes] Résolution d'un problème avec le VU-mètre (événement non déclenché)
* [Media Player SDK .Net] Mise à jour de la lecture mémoire
* [TOUS] Ajout de la prise en charge de l'interface IAsyncDisposable pour les classes principales multiplateformes. À utiliser pour libérer les objets principaux dans des méthodes asynchrones.
* [Video Capture SDK .Net] Ajout de la prise en charge de madVR pour le multiscreen
* [Video Capture SDK .Net] Résolution d'un problème d'énumération NDI dans le moteur VideoCaptureCore
* [Media Player SDK .Net] Ajout de la démo madVR
* [Video Capture SDK .Net] Ajout de la démo madVR
* [TOUS] Résolution de problèmes madVR dans tous les SDK
* [Media Blocks SDK .Net] Ajout de la démo NDI Source
* [Video Capture SDK .Net] Ajout de la prise en charge NDI pour le moteur multiplateforme
* [TOUS] Résolution du problème « image introuvable » avec le paquet NuGet WinUI
* [Media Blocks SDK .Net / Media Player SDK .Net (multiplateforme)] Ajout de la démo MP3+CDG Karaoke Player
* [Media Blocks SDK .Net] Ajout de CDGSourceBlock pour la lecture de fichiers karaoké MP3+CDG
* [TOUS] Amélioration de la prise en charge madVR
* WinUI VideoView mis à jour pour corriger des problèmes lors de la lecture de fichiers audio
* [Video Capture SDK .Net] Amélioration de la prise en charge des sources VNC pour le moteur VideoCaptureCoreX.
* [Video Capture SDK .Net] Ajout de la prise en charge des sources VNC pour le moteur VideoCaptureCoreX. Vous pouvez utiliser la classe VNCSourceSettings pour configurer Video_Source.
* [Media Blocks SDK .Net] Ajout de la prise en charge des sources VNC. Vous pouvez utiliser la classe VNCSourceBlock comme bloc de source vidéo.
* [Video Capture SDK .Net] La propriété Video_Resize a été modifiée pour utiliser le type IVideoResizeSettings. Vous pouvez utiliser la classe VideoResizeSettings pour effectuer un redimensionnement classique comme avant, ou utiliser MaxineUpscaleSettings/MaxineSuperResSettings pour effectuer un redimensionnement IA sur un GPU Nvidia via le SDK Nvidia Maxine (SDK ou modèles du SDK requis pour le déploiement).
* [TOUS] Résolution de problèmes avec la détection de sources NDI sur le réseau local
* [TOUS] Ajout de la classe KLVParser pour lire et décoder des données depuis des fichiers binaires KLV.
* [TOUS] Ajout du bloc KLVFileSink. Vous pouvez exporter des données KLV depuis des fichiers MPEG-TS.
* [Media Blocks SDK .Net] Ajout de la démo KLV.
* [Video Capture SDK .Net] Ajout d'un streamer réseau MJPEG.
* [TOUS] Ajout de la prise en charge de WASAPI 2.
* [Media Blocks SDK .Net] Mise à jour de l'API des effets vidéo. Ajout du media block Grayscale.
* [Media Blocks SDK .Net] Ajout de l'API Live Video Compositor et exemple.
* [TOUS] Mise à jour du contrôle VideoView Avalonia. Résolution de problèmes de lecture vidéo sur Windows avec des écrans HighDPI.
* [Video Capture SDK .Net] Ajout de la propriété CustomVideoFrameRate à MFOutput. Vous pouvez définir une fréquence d'images personnalisée si votre source fournit une fréquence d'images incorrecte (caméra IP, par exemple).
* [Video Capture SDK .Net] Mise à jour de l'encodeur NVENC. Résolution du problème avec la capture vidéo haute définition.
* [Video Capture SDK .Net] Résolution d'un problème de réglage TV sur les appareils Avermedia
* [Media Blocks SDK .Net] Ajout de blocs OpenCV : CVDewarp, CVDilate, CVEdgeDetect, CVEqualizeHistogram, CVErode, CVFaceBlur, CVFaceDetect, CVHandDetect, CVLaplace, CVMotionCells, CVSmooth, CVSobel, CVTemplateMatch, CVTextOverlay, CVTracker
* [CV] Résolution du problème avec les coordonnées de visage incorrectes.
* [CV, Media Blocks SDK .Net] Ajout du bloc Face Detector.
* [Media Blocks SDK .Net] Ajout de l'encodeur vidéo AV1 rav1e.
* [Media Blocks SDK .Net] Ajout de l'encodeur vidéo GIF.
* [Media Blocks SDK .Net] Ajout des blocs NDI Sink et NDI source.
* [TOUS] Résolution de problèmes de détection du SDK NDI.
* [Media Blocks SDK .Net] Mise à jour de l'encodeur Speex.
* [Media Blocks SDK .Net] Mise à jour du bloc Video Mixer.
* [TOUS] Ajout de méthodes Save/Load pour le format de sortie afin de sérialiser en JSON.
* [Media Blocks SDK .Net] Ajout du bloc puits MJPEG HTTP Live streaming.
* [TOUS] Résolution de la régression MP4 HW QSV H264.
* [TOUS] Mises à jour de stabilité de VideoView WinForms et WPF.
* [Media Player SDK .Net] Suppression de la propriété héritée FilenamesOrURL. Veuillez utiliser l'API `Playlist` à la place.
* [Media Blocks SDK .Net] Ajout de la fonctionnalité de fondu d'entrée/sortie pour le bloc de superposition d'image.
* [TOUS] Mise à jour de la télémétrie
* [TOUS] Les SDK ont été mis à jour pour utiliser `ObservableCollection` au lieu de `List` dans l'API publique.
* [TOUS] Mise à jour de la sortie MP4 HW. Amélioration des performances NVENC.
* [Media Blocks SDK .Net] Ajout de l'exemple Video Compositor.
* [Media Blocks SDK .Net] Ajout des blocs YouTubeSink et FacebookLiveSink avec des configurations personnalisées YouTube/Facebook. `RTMPSink` peut diffuser vers YouTube/Facebook de la même manière qu'auparavant.
* [Media Blocks SDK .Net] Ajout du bloc mélangeur vidéo SqueezeBack.
* [TOUS] Mise à jour du logo texte défilant. Ajout de la méthode Preload pour rendre une superposition de texte avant la lecture.
* [TOUS] Mise à jour du logo texte défilant (performances)
* [Media Blocks SDK .Net] Mise à jour des blocs puits Decklink
* [TOUS] Résolution des crashes avec un logo texte ayant une résolution personnalisée
* [Media Blocks SDK .Net] Ajout de la prise en charge des encodeurs Intel QuickSync H264, HEVC, VP9 et MJPEG.
* [Video Edit SDK .Net] Ajout de la méthode FastEdit_ExtractAudioStreamAsync pour extraire le flux audio depuis un fichier vidéo.
* [Video Edit SDK .Net] Ajout de l'exemple WinForms « Audio Extractor ».
* [Media Blocks SDK .Net] Mise à jour de MP4SinkBlock. Le puits peut découper les fichiers de sortie par durée, taille de fichier ou timecode. Utilisez MP4SplitSinkSettings au lieu de MP4SinkSettings pour configurer.
* [Video Capture SDK .Net] Ajout de l'événement OnMJPEGLowLatencyRAWFrame déclenché lorsque le moteur MJPEG faible latence a reçu une trame RAW d'une caméra.
* [Media Blocks SDK .Net] Ajout de VideoEffectsBlock pour utiliser les effets vidéo, disponible dans les SDK Windows
* [Media Blocks SDK .Net] Mise à jour de la source Decklink
* [Media Blocks SDK .Net] Ajout de la démo Decklink (WPF)
* [TOUS] Résolution du crash de l'effet vidéo DeinterlaceBlend
* [TOUS] Les bibliothèques tierces utilisées ont été déplacées vers l'assembly/paquet NuGet VisioForge.Libs.External
* [TOUS] Ajout du Nvidia Maxine Video Effects SDK (BETA) et application d'exemple pour Media Player SDK .Net et Video Capture SDK .Net
* [Video Capture SDK .Net] Ajout de l'API Decklink_Input_GetVideoFramesCount/Decklink_Input_GetVideoFramesCountAsync pour obtenir le nombre total d'images et d'images perdues pour la source Decklink
* [TOUS] Mise à jour des encodeurs matériels VisioForge

## 15.5

* Prise en charge de .Net 7
* Ajout de la prise en charge de l'événement NetworkDisconnect au moteur de caméra IP MJPEG faible latence
* Ajout de la prise en charge Linux pour les démos basées sur VideoEditCoreX
* Ajout de l'événement OnRTSPLowLatencyRAWFrame pour obtenir des trames RAW depuis un flux RTSP, via le moteur RTSP faible latence
* Ajout de la propriété AutoTransitions au moteur VideoEditCoreX
* Les types System.Drawing.Rectangle et System.Drawing.Size sont remplacés par VisioForge.Types.Rectangle et VisioForge.Types.Size dans toutes les API multiplateformes
* Exemples MAUI (BETA) ajoutés
* Amélioration de la compatibilité avec Snap Camera pour l'encodage MP4 HW
* Licence en ligne mise à jour
* Ajout de la démo Camera Light
* Ajout de la prise en charge des segments dans Media Player SDK .Net (moteur multiplateforme)
* Ajout de l'API Playlist dans Media Player SDK .Net (moteur Windows uniquement)
* Résolution de problèmes avec l'appel « rtsp_source_create_audio_resampler » dans le moteur RTSP faible latence du Video Capture SDK .Net (moteur Windows uniquement)
* Ajout de la prise en charge de plusieurs sorties Decklink dans Video Capture SDK .Net et Video Edit SDK .Net (moteur Windows uniquement)
* Résolution de problèmes avec le moteur de lecture inversée dans Media Player SDK .Net (moteur Windows uniquement)
* ONVIFControl et autres API liées à ONVIF disponibles pour toutes les plateformes
* Changement d'API incompatible : la fréquence d'images est passée de double à VideoFrameRate dans toutes les API
* Ajout du décodage matériel GPU pour le moteur VLC
* Résolution d'un problème avec les applications WPF HighDPI utilisant EVR
* Résolution d'un problème avec la méthode MediaPlayerCore.Video_Renderer_SetCustomWindowHandle
* Ajout de la lecture image précédente dans Media Player SDK .Net (moteur multiplateforme)
* Ajout de la démo WPF Screen Capture à Media Blocks SDK .Net

## 15.4

* Résolution d'un problème avec la propriété Play_PauseAtFirstFrame ignorée
* Mise à jour de la prise en charge HighDPI dans les exemples WinForms
* Résolution d'un problème de prise en charge HighDPI pour le moteur de rendu vidéo Direct2D
* Ajout d'API supplémentaires à la classe ONVIFControl : GetDeviceCapabilities, GetMediaEndpoints
* Résolution d'un problème de réencodage forcé lors de la jonction de fichiers FFMPEG sans réencodage
* Mise à jour de Sentry
* Ajout de paramètres d'interpolation vidéo pour les effets vidéo Zoom et Pan
* Ajout de la prise en charge du framework UI GtkSharp pour le rendu vidéo
* L'API FastEdit a été modifiée pour devenir asynchrone
* Résolution du problème de retournement d'écran avec la propriété Video_Effects_AllowMultipleStreams du noyau Video Capture SDK .Net
* Mise à jour de la démo RTSP MultiView (ajout du décodage GPU, ajout de l'accès au flux RAW)
* Ajout de l'événement OnLoop dans Media Player SDK .Net
* Ajout de la fonctionnalité Loop dans Media Blocks SDK .Net
* VideoView Avalonia a été rétrogradé en 0.10.12 en raison de problèmes d'Avalonia UI avec NativeControl
* Ajout de la démo File Encryptor pour Video Edit SDK .Net

## 15.3

* Amélioration du temps de démarrage de l'application pour les PC avec des cartes Decklink
* Prise en charge du SDK NDI v5
* Résolution d'un problème avec la sortie MKV héritée (mauvaise exception de cast).
* Optimisations de performances des effets Zoom et Pan
* Ajout de l'API de base Media Blocks (en cours)
* Ajout du streaming réseau HLS à Video Edit SDK .Net
* Ajout de la propriété Rotate à WPF VideoView. Vous pouvez faire pivoter la vidéo de 90, 180 ou 270 degrés. Vous pouvez également utiliser la méthode GetImageLayer() pour obtenir le calque Image et appliquer des transformations personnalisées
* Changement d'API — FilterHelpers renommé en FilterDialogHelper
* Les assemblies VisioForge.Types et VisioForge.MediaFramework fusionnés dans VisioForge.Core
* Les classes UI sont déplacées dans les assemblies VisioForge.Core.UI.* et dans des paquets NuGet indépendants
* VisioForge.Types renommé en VisioForge.Core.Types
* VisioForge.Core ne dépend plus du framework Windows Forms

## 15.2

* Ajout des propriétés HorizontalAlignment et VerticalAlignment aux logos texte et image
* Mise à jour de la prise en charge ONVIF, résolution d'un problème avec un nom d'utilisateur et un mot de passe spécifiés dans l'URL mais pas dans les paramètres de la source
* Résolution d'un problème avec le dialogue de sortie FFMPEG.exe
* Résolution d'un problème avec la capture séparée dans des applications service
* SDK migré de NewtonsoftJson vers System.Text.Json
* Mise à jour de la sortie DirectCapture pour les caméras IP
* Optimisations de performances du traitement vidéo
* Le type de propriété IPCameraSourceSettings.URL est passé de string à `System.Uri`
* Ajout de la sortie DirectCapture ASF pour les caméras IP

## 15.1

* Messages de débogage Sentry désactivés dans la console
* Ajout du streaming Icecast
* Le type de propriété VideoStreamInfo.FrameRate est passé à VideoFrameRate (avec numérateur et dénominateur) au lieu de double
* Mise à jour de WPF VideoView, résolution du problème de lecture de flux caméra IP
* Changement d'API incompatible : les assemblies `VisioForge.Controls`, `VisioForge.Controls.UI`, `VisioForge.Controls.UI.Dialogs` et `VisioForge.Tools` sont fusionnés dans l'assembly `VisioForge.Core`
* L'API d'effets audio utilise désormais un nom de chaîne au lieu d'un index
* Ajout de la prise en charge Android dans Media Player SDK .Net
* Ajout d'un nouveau moteur multiplateforme basé sur GStreamer pour prendre en charge Windows et d'autres plateformes dans le cycle de développement v15

## 15.0

* Ajout de la propriété StatusOverlay à la classe VideoCapture. Assignez l'objet `TextStatusOverlay` à cette propriété pour ajouter une superposition de texte de statut, par exemple pour afficher « Connexion en cours… » pendant la connexion à une caméra IP.
* Le moteur de caméra IP RTSP Live555 a été supprimé. Veuillez utiliser les moteurs RTSP faible latence ou FFMPEG.
* Résolution d'un problème possible avec SDK_Version.
* Ajout de l'API Settings_Load. Vous pouvez charger le fichier de paramètres enregistré par Settings_JSON. Assurez-vous que les noms de périphériques sont corrects.
* Résolution d'un problème avec une exception si la capture séparée démarrait avant l'appel à Start/StartAsync.
* Prise en charge RTP pour le moteur de source VLC.
* Changement d'API incompatible : la propriété SDK_State a été supprimée. Il n'y a plus de versions TRIAL et FULL du SDK.
* Changement d'API incompatible : DirectShow_Filters_Show_Dialog, DirectShow_Filters_Has_Dialog, Audio_Codec_HasDialog, Audio_Codec_ShowDialog, Video_Codec_HasDialog, Video_Codec_ShowDialog, Filter_Supported_LAV, Filter_Exists_MatroskaMuxer, Filter_Exists_OGGMuxer, Filter_Exists_VorbisEncoder, Filter_Supported_EVR, Filter_Supported_VMR9 et Filter_Supported_NVENC ont été déplacés vers la classe VisioForge.Tools.FilterHelpers.
* Les classes `VFAudioStreamInfo`/`VFVideoStreamInfo` utilisent `Timespan` pour la durée.
* Les types Decklink de l'assembly VisioForge.Types sont déplacés vers l'espace de noms VisioForge.Types.Decklink.
* Télémétrie mise à jour.
* Chargeur de redist personnalisé mis à jour.
* Mise à jour NDI.
* Changement d'API incompatible : la propriété `Status` a été renommée en `State`. Le type de la propriété est `PlaybackState` dans tous les SDK.
* Changement d'API incompatible : les contrôles UI sont scindés en Core (VideoCaptureCore, MediaPlayerCore, VideoEditCore) et VideoView.
* Changement d'API incompatible : les propriétés Video_CaptureDevice... sont fusionnées dans la propriété Video_CaptureDevice de type VideoCaptureSource.
* Changement d'API incompatible : les propriétés Audio_CaptureDevice... sont fusionnées dans la propriété Audio_CaptureDevice de type AudioCaptureSource.
* Changement d'API incompatible : dans Media Player SDK, les propriétés API `Source_Stream` ont été fusionnées dans la propriété `Source_MemoryStream` de type `MemoryStreamSource`
* Mise à jour de la lecture DVD
* Mise à jour de la source FFMPEG
* Changement d'API incompatible : les types Media Player SDK sont déplacés de l'espace de noms VisioForge.Types vers VisioForge.Types.MediaPlayer
* Changement d'API incompatible : les types Video Capture SDK sont déplacés de l'espace de noms VisioForge.Types vers VisioForge.Types.VideoCapture
* Changement d'API incompatible : les types Video Edit SDK sont déplacés de l'espace de noms VisioForge.Types vers « VisioForge.Types.VideoEdit »
* Changement d'API incompatible : les types Output sont déplacés de l'espace de noms VisioForge.Types vers VisioForge.Types.Output
* Changement d'API incompatible : les types Video Effects sont déplacés de l'espace de noms VisioForge.Types vers VisioForge.Types.VideoEffects
* Changement d'API incompatible : les types Audio Effects sont déplacés de l'espace de noms VisioForge.Types vers VisioForge.Types.AudioEffects
* Changement d'API incompatible : les types Event sont déplacés de l'espace de noms VisioForge.Types vers VisioForge.Types.Events
* Ajout de la méthode Video_Renderer_SetCustomWindowHandle pour définir un moteur de rendu vidéo personnalisé via un handle HWND de fenêtre/contrôle Win32

## 14.4

* Prise en charge de Windows 11
* Mise à jour de la télémétrie
* Résolution de problèmes avec Picture-in-Picture en mode 2x2
* Résolution de problèmes avec la source MJPEG faible latence sous .Net 5/.Net 6/.Net Core 3.1
* Résolution d'un problème avec le streaming réseau UDP pour la source Decklink
* VFMP4v11Output renommé en VFMP4HWOutput
* Ajout de la prise en charge de l'encodeur Microsoft H265
* Ajout de la prise en charge de l'encodeur Intel QuickSync H265
* Ajout des événements OnDecklinkInputDisconnected/OnDecklinkInputReconnected
* Mise à jour de la sortie Decklink
* Résolution de problèmes avec la capture séparée pour les sorties MP4 HW, MOV, MPEG-TS et MKVv2
* Ajout de la propriété Video_CaptureDevice_CustomPinName. Vous pouvez utiliser cette propriété pour définir un nom de broche de sortie personnalisé pour un périphérique de capture vidéo ayant plusieurs broches vidéo de sortie
* Mise à jour de la configuration des redists personnalisés
* Mise à jour du moteur caméra IP RTSP faible latence

## 14.3

* Résolution d'un problème de création du filtre Video Resize pour les redists NuGet
* Mise à jour de la télémétrie
* Mise à jour de la sortie VFDirectCaptureMP4Output
* Prise en charge de .Net 6 (preview)
* Suppression de Nvidia CUDA. NVENC est une alternative moderne et est disponible pour l'encodage H264/HEVC.
* Mise à jour du moteur caméra IP MJPEG faible latence
* Mise à jour du listing de sources NDI
* Amélioration de la prise en charge ONVIF
* Ajout de la prise en charge de .Net Core 3.1 pour le moteur de source RTSP faible latence
* Résolution de problèmes avec Picture-in-Picture en mode 2x2
* Scission du projet et des solutions en fichiers indépendants pour .Net Framework 4.7.2, .Net Core 3.1, .Net 5 et .Net 6

## 14.2

* Résolution d'un problème avec la capture de flux audio lorsque la sortie Virtual Camera SDK était activée
* VFMP4v8v10Output a été remplacé par VFMP4Output
* La méthode « CanStart » a été ajoutée aux éléments Video_CaptureDevices. La méthode renvoie true si le périphérique peut démarrer et n'est pas utilisé exclusivement dans une autre application
* Ajout de l'API async/await à ONVIFControl
* Résolution d'un problème de traitement incorrect de ColorKey dans l'effet vidéo Text Overlay
* Ajout de la prise en charge d'une fréquence d'images forcée pour la source caméra IP RTSP faible latence
* Mise à jour des encodeurs AMD MP4v11
* Résolution du problème d'horodatage qui survenait lors de la pause/reprise d'une capture séparée MP4v11
* Mise à jour du streaming réseau FFMPEG.exe
* Sortie FFMPEG mise à jour vers la dernière version de FFMPEG
* La redist VC++ n'est plus requise pour l'installation. La liaison VC++ est passée en statique (sauf pour la sortie XIPH optionnelle)
* De nombreux filtres DirectShow de base ont été déplacés dans le module VisioForge_BaseFilters

## 14.1

* Ajout du contrôle WPF VideoView. Vous pouvez pousser des trames vidéo depuis l'événement OnVideoFrameBuffer vers le contrôle pour les rendre
* Valeur de transparence par défaut correcte pour un logo texte
* Prise en charge ONVIF ajoutée aux builds .Net 5 / .Net Core 3.1
* Ajout de la méthode IP_Camera_ONVIF_ListSourcesAsync pour découvrir les caméras ONVIF sur le réseau local
* (CHANGEMENT D'API INCOMPATIBLE) Modification de l'API du périphérique de capture vidéo pour l'énumération des fréquences d'images afin de prendre en charge les caméras 4K modernes
* Mise à jour du MJPEG Decoder (amélioration des performances)
* Suppression des encodeurs MP4 v8 hérités
* Prise en charge d'INotifyPropertyChanged dans les wrappers WinForms/WPF pour fournir un support d'application MVVM
* Résolution d'un problème de streaming RTMPS vers Facebook
* Source caméra IP ajoutée à la démo TimeShift
* Ajout de la prise en charge de la sortie séparée pour MOV
* Ajout du drapeau FFMPEG fast-start pour la sortie MP4v11 qui utilise le muxer FFMPEG MP4
* Ajout du décodage GPU pour la source caméra IP dans les applications de démonstration
* Ajout de la propriété CustomRedist_DisableDialog pour désactiver le dialogue de message de redist
* Suppression des assemblies et démos Kinect. Veuillez nous contacter si vous avez encore besoin de paquets Kinect
* Le profil par défaut MP4v10 a été modifié en Baseline / 5.0 pour une meilleure compatibilité navigateur

## 14.0

* Prise en charge de .Net 5.0
* Résolution d'un problème avec des sources Decklink non visibles dans la version NuGet du SDK
* Résolution d'un problème avec le notificateur d'ajout/suppression de périphériques
* Ajout d'une source NDI alternative dans Video Capture SDK .Net
* Ajout du streaming NDI (serveur) dans Video Capture SDK .Net
* Résolution d'un problème d'utilisation de Separate Capture pour le déploiement NuGet
* Résolution d'un problème avec les logos texte/image fusionnés
* Mise à jour du notificateur de périphériques
* Ajout de la classe CameraPowerLineControl pour contrôler l'option de fréquence de ligne d'alimentation des webcams
* Les effets audio hérités ont été supprimés.
* Suppression de HTTP_FFMPEG, RTSP_UDP_FFMPEG, RTSP_TCP_FFMPEG et RTSP_HTTP_FFMPEG de l'énumération VFIPSource. Vous pouvez utiliser la valeur Auto_FFMPEG
* Mise à jour du serveur HLS. Rapport d'erreur correct sur le port utilisé
* Ajout du streaming NDI (serveur) dans Video Edit SDK .Net
* Ajout du streaming NDI (serveur) dans Media Player SDK .Net
* Ajout de la méthode IP_Camera_CheckAvailable dans Video Capture SDK .Net
* Mise à jour du filtre FFMPEG Source, davantage de codecs pris en charge et ajout du décodage GPU

## 12.1

* Migration vers .Net 4.6
* Ajout de la propriété Debug_DisableMessageDialogs pour désactiver la boîte de dialogue d'erreur si l'événement OnError n'est pas implémenté.
* Correction d'un problème de redimensionnement en pause pour les contrôles WPF.
* Mise à jour du moteur ONVIF dans Video Capture SDK .Net
* Mise à jour de la source What You Hear dans Video Capture SDK .Net
* Ajout des événements OnPause/OnResume
* Mise à jour de la démo YouTube dans Media Player SDK .Net
* Amélioration de la prise en charge des webcams avec encodeur H264 intégré dans Video Capture SDK .Net
* Mise à jour de la source VLC
* Suppression d'un avertissement indésirable dans la sortie MP4 v11
* Un installateur unique pour les versions TRIAL et FULL
* Mêmes paquets NuGet pour les versions TRIAL et FULL
* Le paquet NuGet .Net Core est fusionné avec le paquet .Net Framework
* Ajout des redists NuGet. Le déploiement n'a jamais été aussi simple !

## 12.0

* API Async / await pour tous les SDK
* Changement d'API incompatible : toutes les API liées au temps utilisent désormais TimeSpan au lieu de long (millisecondes)
* Lecteur/écrivain de tags — chargement correct du logo pour certains formats vidéo
* Suppression des effets vidéo DirectX 9 hérités
* Correction d'un problème de progression de conversion audio dans Video Edit SDK .Net
* Amélioration de la compatibilité .Net Core
* Sortie Virtual Camera SDK ajoutée à Media Player SDK .Net (en tant que l'un des moteurs de rendu vidéo)
* Prise en charge des appareils NewTek NDI ajoutée à Video Capture SDK .Net en tant que nouveau moteur pour les caméras IP
* Ajout des propriétés Video_Effects_MergeImageLogos et Video_Effects_MergeTextLogos. Si vous avez trois logos ou plus, vous pouvez définir ces propriétés sur true pour optimiser les performances des effets vidéo
* Ajout d'une option de type de playlist pour le streaming réseau HLS
* Ajout d'un serveur HTTP léger intégré pour le streaming réseau HLS
* Ajout de la prise en charge de la vidéo VR 360° dans Media Player SDK .Net
* Amélioration du traitement vidéo DirectX 11
* Ajout de la prise en charge MPEG-TS AAC sans vidéo pour la sortie MPEG-TS
* Amélioration de la source audio What You Hear
* Plusieurs nouvelles applications de démonstration
* Amélioration de la sortie MP4 v11
* La capture séparée pour MP4 v11 peut découper les fichiers sans perte d'images
* De nombreuses corrections de bugs mineures
* Les assemblies .Net Core mis à jour vers .Net Core 3.1 LTS
* Mise à jour du dépôt de démos sur GitHub

## 11.4

* Ajout de l'application de démonstration ASP.Net MVC de conversion vidéo à Video Edit SDK .Net
* Implémentation OSD alternative pour gérer les changements de Windows 10
* Mise à jour des effets vidéo GPU
* Mise à jour de la source mémoire dans Media Player SDK .Net
* Mise à jour de l'API OSD
* Résolution de problèmes de chiffrement vidéo utilisant des clés binaires
* Mise à jour des démos de capture d'écran pour Video Capture SDK .Net, ajout de la sélection de fenêtre à capturer. Vous pouvez capturer toute fenêtre, y compris les fenêtres en arrière-plan
* Ajout de l'effet Mosaic dans la démo Computer Vision de Video Capture SDK .Net
* Ajout de la démo Multiple IP Cameras (WPF) dans Video Capture SDK .Net
* Ajout d'une option personnalisée de redimensionnement vidéo pour la sortie MP4v11
* Redists module de fusion (MSM) ajoutés à tous les SDK
* Mise à jour de la sortie FFMPEG.exe utilisant des pipes au lieu de périphériques virtuels
* Résolution d'un problème avec l'option de résolution de sortie personnalisée PIP dans Video Capture SDK .Net
* Résolution d'un problème de verrouillage de fichier utilisant le moteur LAV dans Media Player SDK .Net
* Ajout du traitement vidéo GPU basé sur DirectX 11

## 11.3

* Résolution d'un problème avec la connexion du moteur de rendu audio si la sortie Virtual Camera SDK est activée dans Video Capture SDK
* Amélioration de la prise en charge des sous-titres avec chargement automatique dans Media Player SDK .Net
* Mise à jour des effets audio fade-in/fade-out
* Ajout de la prise en charge des fichiers MIDI et KAR dans Media Player SDK .Net
* Ajout de la prise en charge des fichiers karaoké CDG (et nouvelle application de démonstration) dans Media Player SDK .Net
* Ajout de la lecture asynchrone dans Media Player SDK .Net
* Mise à jour du sérialiseur JSON intégré
* Ajout du décodage GPU optionnel dans Media Player SDK .Net. Moteurs de décodage disponibles : DXVA2, Direct3D 11, nVidia CUVID, Intel QuickSync
* Ajout de la prise en charge de .Net Core 3.0, y compris les applications de démonstration WinForms et WPF (Windows uniquement)

## 11.2

* Ajout de la propriété Loop à Video Edit SDK .Net
* Mise à jour de l'amélioration audio
* Mise à jour de la source RTSP faible latence
* Résolution d'un problème de rognage pour la source Decklink
* Ajout d'une propriété pour utiliser TCP ou UDP dans le moteur RTSP faible latence
* Déploiement sans enregistrement COM et droits administrateur pour Video Edit SDK et Media Player SDK (BETA)
* Mise à jour du mélangeur vidéo avec des performances améliorées
* Ajout d'un extrait de code de lecture YouTube
* Ajout d'une méthode pour déplacer l'OSD

## 11.1

* Correction d'un problème de recherche avec certains fichiers MP4 dans Video Edit SDK
* Correction d'un problème d'étirement/letterbox dans la version WPF de tous les SDK
* Correction d'un problème avec un égaliseur sur une fréquence d'échantillonnage de 16000 ou moins
* Correction d'un problème avec le sample grabber pour la source DirectShow dans Media Player SDK
* Correction de la lecture des fichiers chiffrés dans Media Player SDK
* Ajout de la classe DVDInfoReader pour lire les informations sur les fichiers DVD
* Résolution d'un problème de nom de fichier incorrect dans l'événement OnSeparateCaptureStopped
* Amélioration de la qualité de détection de codes-barres pour les images pivotées
* La version minimale de .Net Framework est désormais .Net 4.5
* Amélioration de la lecture YouTube dans Media Player SDK. Ajout de l'événement OnYouTubeVideoPlayback pour sélectionner la qualité vidéo pour la lecture
* Ajout de la propriété `Play_PauseAtFirstFrame` à Media Player SDK .Net. Si true, la lecture sera mise en pause sur la première image
* Prise en charge de plusieurs écrans dans la démo Screen Capture de Video Capture SDK .Net
* Résolution d'un problème de lecture de flux réseau dans les applications WPF de Media Player SDK .Net
* Ajout de la lecture de flux MJPEG HTTP faible latence (caméras IP ou autres sources) dans Video Capture SDK .Net
* Ajout du filtre DirectShow Fake Audio Source, qui produit un signal de tonalité
* Mise à jour de la démo Computer Vision dans Video Capture SDK .Net
* Ajout de la méthode Frame_GetCurrentFromRenderer à tous les SDK. À l'aide de cette méthode, vous pouvez obtenir la trame vidéo actuellement rendue directement depuis le moteur de rendu vidéo.
* Ajout de la lecture de source RTSP faible latence dans Video Capture SDK .Net

## 11.0

* Correction d'un bug avec la sortie MP4 v11, paramètres GOP personnalisés
* Mise à jour du MJPEG Decoder
* Correction d'un bug avec la sortie MP4 v11. Ajout du support complet de Windows 7
* L'événement OnStop de Video Edit SDK renvoie un statut réussi
* Mise à jour de la démo principale de Video Capture SDK — le multiscreen peut automatiquement utiliser les écrans externes connectés
* Mise à jour de la démo principale de Media Player SDK — le multiscreen peut automatiquement utiliser les écrans externes connectés
* Ajout du fade-in / fade-out pour le logo texte
* Mise à jour de la sortie Decklink
* Video Edit SDK peut couper rapidement des fichiers depuis des sources réseau (HTTP/HTTPS)
* Ajout de la démo Computer Vision, avec un compteur de voitures/piétons et un détecteur/traqueur de visage/yeux/nez/bouche
* Mise à jour de la sortie MP4 pour utiliser un muxer alternatif qui fournit une fréquence d'images constante
* Ajout de la sortie MPEG-TS
* Ajout de la sortie MOV
