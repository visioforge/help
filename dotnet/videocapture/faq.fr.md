---
title: FAQ Video Capture SDK .NET — caméras IP, enregistrement
description: Réponses sur Video Capture SDK .NET — identifiants et reconnexion RTSP, enregistrer sans arrêter l'aperçu, captures d'image, périphériques, superpositions.
sidebar_label: FAQ
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCoreX
  - Windows
  - Capture
  - IP Camera
  - RTSP
  - Screen Capture
  - Webcam
  - Troubleshooting
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureCoreX
  - IPCameraSourceSettings
  - RTSPSourceSettings
  - TextStatusOverlay

---

# FAQ de Video Capture SDK .NET

Réponses aux questions que les développeurs posent le plus souvent sur Video Capture SDK .NET. Les deux moteurs sont couverts : `VideoCaptureCore` (Windows, DirectShow) et `VideoCaptureCoreX` (multiplateforme). Les erreurs de déploiement comme `Unable to create filter` sont regroupées dans [Dépannage](../general/troubleshooting.md).

## Caméras IP

### Comment se connecter à une caméra IP qui demande un identifiant et un mot de passe ?

Passez les identifiants dans les paramètres de la source. Avec `VideoCaptureCore` :

```csharp
VideoCapture1.IP_Camera_Source = new IPCameraSourceSettings
{
    URL = new Uri("rtsp://192.168.1.10:554/stream1"),
    Login = "admin",
    Password = "password",
    Type = IPSourceEngine.Auto_LAV
};

VideoCapture1.Mode = VideoCaptureMode.IPPreview;
await VideoCapture1.StartAsync();
```

Avec `VideoCaptureCoreX` ou Media Blocks SDK .NET, `RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled)` les reçoit en arguments. La plupart des caméras acceptent aussi la forme `rtsp://login:password@host:port/path`, mais des propriétés séparées évitent les problèmes de caractères spéciaux dans le mot de passe.

L'URL doit être l'adresse du flux vidéo lui-même, et non celle de la page web de la caméra. Le chemin diffère d'un fabricant à l'autre : consultez le manuel de la caméra, les [guides par marque de caméra](../camera-brands/index.md), ou découvrez le flux avec [ONVIF](video-sources/ip-cameras/onvif.md). Toute caméra compatible RTSP, ONVIF ou HTTP MJPEG peut être utilisée, qu'elle soit connectée en Wi-Fi ou par câble. Un test rapide : si VLC ouvre l'URL, le SDK peut l'ouvrir aussi. Avec `VideoCaptureCore`, une URL qui ne s'ouvre pas est signalée par un message commençant par `Unable to set URL to network FFMPEG source filter`, `Unable to set URL to LAV Splitter Source filter` ou `Unable to set URL to network VLC source filter`, selon le moteur.

### Comment se reconnecter quand la caméra se déconnecte ou n'est pas encore en ligne ?

`VideoCaptureCore` déclenche `OnNetworkSourceDisconnect` lorsqu'un flux établi est perdu, et `OnError` lorsque la première connexion échoue. Dans les deux cas, arrêtez le moteur et redémarrez-le, généralement après un délai. Pour éviter une tentative de connexion complète, testez d'abord la caméra :

```csharp
var uri = new Uri("rtsp://192.168.1.10/stream1");
bool online = VideoCapture1.IP_Camera_CheckAvailable(
    uri, 554, System.Net.Sockets.ProtocolType.Tcp, TimeSpan.FromSeconds(5));
```

Passez le port réel : lorsque l'URL n'en contient pas, `Uri.Port` renvoie le port par défaut du schéma, ou -1 pour `rtsp://`, que .NET ne connaît pas. Le port RTSP par défaut est 554. Le délai couvre à la fois la résolution du nom d'hôte et la connexion TCP. Seul TCP est pris en charge : une vérification UDP renvoie `false`. La vérification signale une connexion refusée via `OnError` ; exécutez-la donc dans une tâche en arrière-plan et non dans le gestionnaire `OnError`. L'ensemble des options de reconnexion, y compris la bascule de secours automatique des moteurs X, est décrit dans [Reconnexion et secours RTSP](../general/network-sources/reconnection-and-fallback.md).

### Le moteur RTSP à faible latence se connecte mais n'affiche rien

`IPSourceEngine.RTSP_LowLatency` a besoin que la caméra indique sa fréquence d'images. Si une caméra ne le fait pas, définissez-la vous-même :

```csharp
VideoCapture1.IP_Camera_Source.Type = IPSourceEngine.RTSP_LowLatency;
VideoCapture1.IP_Camera_Source.ForcedFramerate = VideoFrameRate.FPS_25;
```

Essayez aussi UDP (`RTSP_LowLatency_UseUDP = true`) et vérifiez que le redistribuable MP4 est déployé : la source à faible latence en fait partie.

### Comment afficher « Connexion… » jusqu'à l'arrivée de la première image ?

Les superpositions et les effets sont dessinés sur les images vidéo ; ils ne peuvent donc pas apparaître avant la première. Utilisez plutôt la superposition d'état du `VideoView` ; elle est dessinée tant que le moteur est démarré mais que la source n'est pas encore connectée :

```csharp
VideoView1.StatusOverlay = new TextStatusOverlay
{
    TextConnecting = "Connecting to the camera..."
};
```

`TextStatusOverlay` possède aussi les propriétés `Font`, `Brush` et des propriétés d'alignement. `ImageStatusOverlay` et `GIFStatusOverlay` affichent une image ou une animation au lieu d'un texte.

### Comment enregistrer de nombreuses caméras IP sans forte charge CPU ?

Le CPU est consommé par le décodage, le réencodage et l'affichage de chaque flux :

- Enregistrez le flux d'origine sans réencodage lorsque le format le permet — voir [Enregistrer le flux RTSP d'origine](../mediablocks/Guides/rtsp-save-original-stream.md).
- Lorsque vous devez réencoder, utilisez un encodeur GPU (NVENC, Intel Quick Sync, AMD AMF) — voir [Encodeurs H.264](../general/video-encoders/h264.md).
- Désactivez le rendu des caméras que personne ne regarde : `VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.None;`.

## Enregistrement

### Comment démarrer et arrêter l'enregistrement sans arrêter l'aperçu ?

Utilisez la capture séparée : définissez `SeparateCapture_Enabled = true`, choisissez un mode de capture (`VideoCaptureMode.VideoCapture`, `IPCapture`, `ScreenCapture`, etc., et non un mode aperçu) et définissez `Output_Format`. Avec `SeparateCapture_AutostartCapture` désactivé (la valeur par défaut), `StartAsync` n'exécute alors que l'aperçu, sauf avec les sorties DirectCapture du périphérique, qui enregistrent dès le départ ; appelez `SeparateCapture_StartAsync(filename)` et `SeparateCapture_StopAsync()` autant de fois que nécessaire. L'aperçu continue et chaque enregistrement est un fichier distinct. Les détails et l'équivalent pour le moteur X se trouvent dans [Capture séparée](video-capture/separate-capture.md).

Les effets vidéo, comme une superposition de texte, sont appliqués avant la séparation du flux ; ils sont donc enregistrés. Pour une superposition visible uniquement dans l'aperçu, utilisez l'API d'affichage à l'écran (OSD).

### Peut-on lire un fichier pendant qu'il est encore en cours d'enregistrement ?

Pas un fichier MP4 : son index est écrit à la fin de l'enregistrement, le fichier ne peut donc pas être ouvert avant. Enregistrez plutôt en MPEG-TS (`MPEGTSOutput`, avec les mêmes codecs H.264 et AAC). Un fichier MPEG-TS peut être lu pendant qu'il grossit. Une autre option consiste à enregistrer par segments et à lire les segments terminés.

### Comment enregistrer le son de plusieurs périphériques audio dans un seul fichier ?

Sélectionnez le périphérique principal comme d'habitude et ajoutez les autres avant de démarrer :

```csharp
VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource("Microphone (USB Audio)");
VideoCapture1.Additional_Audio_CaptureDevice_Add("Line In (Realtek Audio)");
VideoCapture1.Additional_Audio_CaptureDevice_MixChannels = true;
VideoCapture1.Audio_RecordAudio = true;
```

Avec `Additional_Audio_CaptureDevice_MixChannels = true`, le périphérique principal et le premier périphérique supplémentaire sont mixés en une seule piste audio ; sans cette propriété, chaque périphérique est écrit comme un flux audio distinct, que tous les lecteurs n'affichent pas. Avec `VideoCaptureCoreX`, mixez les sources dans un pipeline Media Blocks avec un `AudioMixerBlock` — voir [Traitement audio dans Media Blocks](../mediablocks/AudioProcessing/index.md). Un microphone n'est pas exclusif : le même périphérique peut alimenter plusieurs enregistrements ou applications à la fois.

## Périphériques

### Comment voir une caméra branchée après le démarrage de l'application ?

Les listes de périphériques sont lues à la création du moteur. Appelez `RefreshLists` pour les relire, par exemple depuis les événements `OnDeviceAdded` et `OnDeviceRemoved` :

```csharp
VideoCapture1.OnDeviceAdded += (s, e) =>
{
    VideoCapture1.RefreshLists(true, false, false);
    var cameras = VideoCapture1.Video_CaptureDevices();
    var microphones = VideoCapture1.Audio_CaptureDevices();
};
```

### « Unable to run filter graph », erreur 80004005

Le graphe DirectShow de `VideoCaptureCore` n'a pas pu démarrer. Une cause fréquente est le moteur de rendu vidéo : dans une machine virtuelle, via le Bureau à distance ou avec un ancien pilote d'affichage, sélectionnez-en un autre (`VideoRendererMode.EVR`, `VMR9` ou `VideoRenderer`) dans `Video_Renderer.VideoRenderer`. Vérifiez que le périphérique fonctionne dans la Main Demo et qu'aucune autre application ne l'occupe. Si l'échec persiste, [envoyez les journaux](../general/sendlogs.md).

## Captures d'image

### Comment prendre une capture de la vidéo en direct ?

Avec `VideoCaptureCore`, appelez `Frame_GetCurrent()` pour obtenir un `Bitmap`, ou `Frame_SaveAsync(filename, ImageFormat.Jpeg)` pour écrire un fichier. Pour traiter chaque image, utilisez l'événement `OnVideoFrameBitmap` ou `OnVideoFrameBuffer`.

Avec `VideoCaptureCoreX`, activez le capteur d'instantanés **avant** `StartAsync` — il est désactivé par défaut pour économiser les ressources :

```csharp
core.Snapshot_Grabber_Enabled = true;
await core.StartAsync();

// plus tard
await core.Snapshot_SaveAsync("snapshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, 90);
```

## Superpositions et capture d'écran

### Comment afficher une date et une heure qui ne sont pas celles de l'horloge système ?

Générez le texte vous-même. Avec les moteurs X, `OverlayManagerText.TextProvider` est appelé pour chaque image avec l'horodatage de l'image ; vous pouvez donc renvoyer n'importe quelle heure calculée — voir [Texte qui change à chaque image](../general/video-effects/text-overlay.md#texte-qui-change-a-chaque-image). Avec `VideoCaptureCore`, conservez un `VideoEffectTextLogo`, modifiez son `Text` depuis un minuteur et appelez `Update()` :

```csharp
textLogo.Text = myClock.ToString("yyyy-MM-dd HH:mm:ss");
textLogo.Update();
```

Ne modifiez que les propriétés qui changent réellement ; reconstruire tout l'effet à chaque tic ajoute du retard.

### La capture d'écran n'enregistre qu'une partie de l'écran sur un moniteur 4K ou mis à l'échelle

Cela se produit lorsque la mise à l'échelle de l'affichage Windows dépasse 100 % et que l'application ne gère pas le DPI : Windows lui fournit alors des coordonnées mises à l'échelle, alors que la capture travaille en pixels physiques. Rendez l'application compatible DPI par moniteur (un paramètre `app.manifest` pour WPF, `Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)` pour WinForms). Vous pouvez aussi définir `FullScreen = false` et passer le rectangle exact de l'écran en pixels physiques avec `Left`, `Top`, `Right` et `Bottom` de `ScreenCaptureSourceSettings`.

## Sortie vers d'autres applications

### Comment envoyer la vidéo capturée vers une caméra virtuelle ?

Définissez `VideoCapture1.Virtual_Camera_Output_Enabled = true` avant `StartAsync`. Les autres applications (Zoom, Teams, OBS, navigateurs) voient alors la vidéo, avec tous les effets et superpositions, comme la caméra virtuelle VisioForge. La caméra virtuelle elle-même provient de [Virtual Camera SDK](../../directshow/virtual-camera-sdk/index.md), qui doit être installé ou déployé avec ses filtres enregistrés. Un exemple fonctionnel : [virtual-camera-streaming](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/virtual-camera-streaming).

### Comment diffuser vers un serveur RTMP qui exige une authentification ?

Avec `VideoCaptureCore`, utilisez le format de diffusion `RTMP_FFMPEG_EXE` et remplissez la sortie FFmpeg avec le profil FLV, requis par RTMP :

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
ffmpegOutput.FillDefaults(DefaultsProfile.FLV_H264_AAC, true);
```

Les identifiants font partie de l'URL fournie par votre serveur de diffusion : généralement une clé de flux dans le chemin, ou `rtmp://login:password@host/app/stream` pour les serveurs à authentification par identifiant. D'autres options, y compris pour les moteurs X, se trouvent dans [Diffusion RTMP](../general/network-streaming/rtmp.md).
