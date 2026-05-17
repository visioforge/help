---
title: Reconnexion RTSP et Fallback Switch en C# .NET — tous SDK
description: Gérez les déconnexions de caméras IP en C# / .NET avec événements de reconnexion, DisconnectEventInterval et FallbackSwitch dans tous les SDK VisioForge.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - C#
primary_api_classes:
  - FallbackSwitchSettings
  - RTSPSourceSettings
  - MediaPlayerCore
  - VideoCaptureCoreX
  - MediaPlayerCoreX

---

# Reconnexion RTSP et Fallback Switch en C# / .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Les événements de reconnexion fonctionnent sur tous les SDK. Le `FallbackSwitch` déclaratif repose sur GStreamer et fonctionne donc sur **Windows, macOS, Linux, Android et iOS** dans les moteurs `X` modernes et Media Blocks — il n'est pas disponible sur les moteurs classiques DirectShow uniquement Windows. Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) et le [guide de déploiement Linux](../../deployment-x/Ubuntu.md).

Les caméras IP perdent leurs connexions — coupures réseau, redémarrages d'alimentation, redémarrages de routeur, manque de keyframes. Les SDK VisioForge vous offrent deux manières de gérer cela :

- **Réactive** — votre code s'abonne à un événement de déconnexion, puis arrête et redémarre la source.
- **Déclarative** — vous configurez un `FallbackSwitch` sur la source, le pipeline bascule automatiquement sur une image de remplacement / une carte de texte / un flux alternatif, et votre code ne voit jamais la coupure.

Choisissez en fonction du SDK que vous utilisez et de l'expérience utilisateur souhaitée. Ce guide couvre les deux approches dans tous les SDK de la famille.

## Quelle approche est disponible où

| SDK | Réactive (détection + redémarrage) | Déclarative (FallbackSwitch) |
|---|:---:|:---:|
| VideoCaptureCore (classique, Windows) | ✅ `OnNetworkSourceDisconnect` + `DisconnectEventInterval` | ⛔ |
| MediaPlayerCore (classique, Windows) | ✅ `OnNetworkSourceStop` (FFMPEG) / `OnError` | ⛔ |
| VideoCaptureCoreX (multiplateforme) | ✅ pipeline `OnNetworkSourceDisconnect` / `OnError` | ✅ via `RTSPSourceSettings.FallbackSwitch` |
| MediaPlayerCoreX (multiplateforme) | ✅ pipeline `OnNetworkSourceDisconnect` / `OnError` | ✅ |
| Media Blocks SDK | ✅ événement de pipeline | ✅ `FallbackSwitchSourceBlock` |

Règle empirique : sur les moteurs classiques Windows, vous n'avez que la réactive. Sur n'importe quel moteur moderne (multiplateforme), préférez la déclarative pour l'UX, ajoutez la réactive en complément pour la télémétrie.

## Réactive — VideoCaptureCore (classique)

`IPCameraSourceSettings.DisconnectEventInterval` définit la durée pendant laquelle le SDK attend sans images entrantes avant de déclencher l'événement de déconnexion. La valeur par défaut est de 10 secondes ; abaissez-la à 3-5 secondes pour la vidéosurveillance.

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.VideoCapture;

videoCapture1.IP_Camera_Source = new IPCameraSourceSettings
{
    URL = new Uri("rtsp://192.168.1.100:554/stream1"),
    Login = "admin",
    Password = "admin123",
    Type = IPSourceEngine.Auto_LAV,
    DisconnectEventInterval = TimeSpan.FromSeconds(5)
};

videoCapture1.OnNetworkSourceDisconnect += async (s, e) =>
{
    // Backoff exponentiel : 1 s, 2 s, 4 s, 8 s... plafonné à 30 s.
    int attempt = 0;
    int delayMs = 1000;
    while (attempt < 10)
    {
        await videoCapture1.StopAsync();
        await Task.Delay(delayMs);
        if (await videoCapture1.StartAsync()) break;
        delayMs = Math.Min(delayMs * 2, 30_000);
        attempt++;
    }
};

videoCapture1.Mode = VideoCaptureMode.IPPreview;
await videoCapture1.StartAsync();
```

`OnNetworkSourceDisconnect` se déclenche depuis un thread de minuterie — basculez vers le thread d'interface utilisateur (`Invoke` / `Dispatcher.Invoke`) avant de toucher aux contrôles.

## Réactive — MediaPlayerCore (classique)

Le `MediaPlayerCore` classique expose `OnNetworkSourceStop` sur le moteur FFMPEG ; pour les autres moteurs et toute défaillance générique, abonnez-vous à `OnError`.

```csharp
using VisioForge.Core.MediaPlayer;

mediaPlayer1.OnNetworkSourceStop += async (s, e) =>
{
    await mediaPlayer1.StopAsync();
    await Task.Delay(2000);
    await mediaPlayer1.StartAsync();
};

mediaPlayer1.OnError += (s, e) =>
{
    // Journaliser mais ne pas démanteler — laissez la boucle de reprise ci-dessus gérer les vraies coupures.
    Debug.WriteLine($"Erreur du lecteur : {e.Message}");
};
```

## Réactive — pipelines Media Blocks

`MediaBlocksPipeline` expose `OnNetworkSourceDisconnect` avec un `NetworkSourceDisconnectEventArgs` riche — utile lorsque plusieurs sources RTSP partagent une seule application et que vous devez savoir *laquelle* a coupé.

```csharp
using VisioForge.Core.Types.Events;

pipeline.OnNetworkSourceDisconnect += (s, e) =>
{
    // e.SourceElementName — l'élément GStreamer qui a échoué
    // e.ErrorMessage       — courte description
    // e.DebugInfo          — sortie de débogage GStreamer (peut être null)
    // e.Uri                — l'URL RTSP en échec (peut être null)
    // e.Timestamp          — moment où le SDK a détecté l'échec
    Log($"[{e.Timestamp:HH:mm:ss}] {e.SourceElementName} a coupé : {e.ErrorMessage}");
};

pipeline.OnError += (s, e) =>
{
    Log($"Erreur de pipeline : {e.Message}");
};
```

## Réactive — moteurs X (VideoCaptureCoreX / MediaPlayerCoreX)

`VideoCaptureCoreX` et `MediaPlayerCoreX` **n'exposent pas** publiquement leur `MediaBlocksPipeline` interne. Pour réagir aux déconnexions sur ces moteurs :

1. Abonnez-vous à `OnError` sur le moteur — se déclenche pour toutes les erreurs au niveau du pipeline, y compris les sources RTSP perdues.
2. Utilisez le `FallbackSwitch` déclaratif (ci-dessous) pour maintenir l'interface utilisateur vivante pendant les coupures transitoires.
3. Si vous avez besoin du `NetworkSourceDisconnectEventArgs` granulaire par source, construisez le pipeline de capture directement avec `MediaBlocksPipeline` + `RTSPSourceBlock` et utilisez le modèle Media Blocks ci-dessus.

```csharp
videoCaptureCoreX.OnError += (s, e) =>
{
    // e.Message porte l'erreur au niveau du moteur ; pour les coupures RTSP, le texte inclut
    // le nom de l'élément source. Associez ce gestionnaire à une boucle de reprise ou un FallbackSwitch.
    Log($"Erreur VideoCaptureCoreX : {e.Message}");
};
```

## Déclarative — vue d'ensemble du FallbackSwitch

Le `FallbackSwitch` enveloppe une source en direct avec un basculement automatique. Lorsque la source principale arrête de produire des images pendant plus longtemps que `TimeoutMs`, le pipeline bascule sur un repli préconfiguré (texte, image ou média alternatif) et maintient le rendu de votre `VideoView`. Lorsque la source principale se rétablit, le pipeline rebascule.

### Le conteneur `FallbackSwitchSettings`

```csharp
public class FallbackSwitchSettings
{
    public bool Enabled { get; set; } = false;
    public FallbackSwitchSettingsBase Fallback { get; set; }
    public bool EnableVideo { get; set; } = true;
    public bool EnableAudio { get; set; } = true;
    public int MinLatencyMs { get; set; } = 0;
    public string FallbackVideoCaps { get; set; }   // par ex. "video/x-raw,width=1920,height=1080"
    public string FallbackAudioCaps { get; set; }   // par ex. "audio/x-raw,rate=48000,channels=2"
}
```

### Paramètres de base (partagés par tous les types de repli)

```csharp
public abstract class FallbackSwitchSettingsBase
{
    public int  TimeoutMs        { get; set; } = 5000;   // silence avant que le repli ne s'active
    public int  RestartTimeoutMs { get; set; } = 5000;   // attente entre les tentatives de redémarrage de la source principale
    public int  RetryTimeoutMs   { get; set; } = 60000;  // fenêtre d'abandon après plusieurs échecs
    public bool ImmediateFallback{ get; set; } = false;  // afficher le repli dès la première coupure d'image
    public bool RestartOnEos     { get; set; } = false;  // traiter l'EOS comme un échec (pour les boucles)
    public bool ManualUnblock    { get; set; } = false;  // exiger que l'application appelle Unblock() lors de la reprise
}
```

### Trois types de repli

1. **`StaticTextFallbackSettings`** — rendre une carte de texte « Caméra hors ligne ».
2. **`StaticImageFallbackSettings`** — afficher un logo de marque, le dernier instantané ou une diapositive « reconnexion en cours ».
3. **`MediaBlockFallbackSettings`** — lire un flux en direct alternatif ou un fichier en boucle.

## Repli — texte statique

```csharp
using SkiaSharp;
using VisioForge.Core.Types.X.Sources;

var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticTextFallbackSettings
    {
        Text            = "Caméra hors ligne — reconnexion...",
        FontFamily      = "Arial",
        FontSize        = 32f,
        TextColor       = SKColors.White,
        BackgroundColor = SKColors.Black,
        Position        = new SKPoint(0.5f, 0.5f),   // centre
        TimeoutMs       = 3000,
        RestartTimeoutMs= 5000,
    },
};
```

## Repli — image statique

```csharp
var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticImageFallbackSettings
    {
        ImagePath           = @"C:\assets\camera-offline.png",
        ScaleToFit          = true,
        MaintainAspectRatio = true,
        BackgroundColor     = SKColors.Black,
        TimeoutMs           = 3000,
    },
};
```

## Repli — source de média alternative

```csharp
var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new MediaBlockFallbackSettings
    {
        FallbackUri      = "file:///C:/assets/standby-loop.mp4",
        IsLive           = false,
        RestartOnEos     = true,      // redémarrer la boucle à la fin du fichier
        TimeoutMs        = 3000,
    },
};
```

`FallbackUri` accepte toute URI que GStreamer peut lire — `file://`, un autre `rtsp://`, HLS, HTTP. `CustomPipeline` permet d'injecter une ligne de lancement GStreamer sur mesure pour les scénarios avancés (par ex. `videotestsrc pattern=snow`).

## Utilisation du FallbackSwitch — haut niveau (`RTSPSourceSettings.FallbackSwitch`)

Le chemin le plus simple : attachez directement l'objet de paramètres à `RTSPSourceSettings` et passez-le à `VideoCaptureCoreX` / `MediaPlayerCoreX` comme d'habitude. Aucune plomberie de pipeline supplémentaire.

```csharp
var rtsp = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.100:554/stream1"),
    "admin", "admin123", audioEnabled: false);

rtsp.LowLatencyMode = true;
rtsp.FallbackSwitch = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticImageFallbackSettings
    {
        ImagePath = "offline.png",
        TimeoutMs = 3000,
    },
};

_videoCapture.Video_Source = rtsp;
await _videoCapture.StartAsync();
```

## Utilisation du FallbackSwitch — bas niveau (`FallbackSwitchSourceBlock`)

Dans le Media Blocks SDK, vous câblez le repli directement au niveau du pipeline via `FallbackSwitchSourceBlock`. Cela fonctionne avec *n'importe quelle* source — RTSP, HTTP MJPEG, fichier, périphérique — pas seulement RTSP.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;

var fallbackSwitch = new FallbackSwitchSourceBlock(
    mainSourceSettings: rtspSettings,                          // IVideoSourceSettings
    fallbackSettings:   new FallbackSwitchSettings
    {
        Enabled = true,
        Fallback = new StaticTextFallbackSettings { Text = "HORS LIGNE" },
    });

// Télémétrie — fonctionne en parallèle de l'UI déclarative
pipeline.OnNetworkSourceDisconnect += (s, e) =>
    metrics.RecordDisconnect(e.SourceElementName, e.Uri, e.Timestamp);

// FallbackSwitchSourceBlock expose des pads VideoOutput / AudioOutput distincts — aucun Output unique.
pipeline.Connect(fallbackSwitch.VideoOutput, renderer.Input);
await pipeline.StartAsync();

// Inspecter l'état d'exécution :
string status = fallbackSwitch.GetStatus();
var stats = fallbackSwitch.GetStatistics();

// Si ManualUnblock est défini, appelez Unblock() lorsque vous souhaitez que la lecture reprenne sur la source principale.
// fallbackSwitch.Unblock();
```

## Modèles courants

**Backoff exponentiel** — dans la boucle réactive, doublez le délai à chaque reconnexion ratée, plafonnez à 30 s. Cela vous empêche de saturer le cache ARP d'une caméra morte.

**UX déclarative + télémétrie réactive** — laissez `FallbackSwitch` maintenir l'écran vivant, et utilisez `pipeline.OnNetworkSourceDisconnect` pour alimenter votre tableau de bord de monitoring / alerte Slack / journal NVR. Les deux approches ne s'excluent pas l'une l'autre.

**Mur multi-caméras** — ne démantelez jamais toute la grille sur une seule panne. Consultez le [guide de la grille RTSP multi-caméras](../../mediablocks/Guides/multi-camera-rtsp-grid.md) pour le modèle un-pipeline-par-caméra ; attachez un `FallbackSwitch` à chaque moteur indépendamment.

**Note multiplateforme** — `FallbackSwitch` dépend de l'élément GStreamer `fallbackswitch`, qui est livré avec le redist X. Les `VideoCaptureCore` / `MediaPlayerCore` classiques uniquement Windows ne l'ont pas — utilisez l'approche réactive là-bas.

## Dépannage

**Le repli ne s'active jamais** — `Enabled = true` ? `TimeoutMs` plus bas que la coupure que vous voulez attraper ? Les premières images doivent réussir avant que le compteur de timeout ne démarre — une caméra qui échoue à la première poignée de main est une erreur de démarrage de pipeline, pas un timeout.

**Le repli s'active mais ne se désactive jamais** — la source principale ne se rétablit pas. Vérifiez `RestartTimeoutMs` (trop élevé retarde la prochaine reprise) et `RetryTimeoutMs` (après cette fenêtre, le bloc cesse d'essayer). Avec `ManualUnblock = true`, vous devez appeler `fallbackSwitch.Unblock()` vous-même.

**Le repli image affiche du noir** — incompatibilité des caps codec entre le pipeline principal et le décodeur de repli. Définissez `FallbackVideoCaps` explicitement, par ex. `"video/x-raw,width=1920,height=1080,format=RGB"`, en correspondance avec le format attendu par le moteur de rendu.

**Repli texte — police / position incorrecte** — `FontFamily` doit exister sur la plateforme cible (Arial est sur Windows et macOS ; préférez `DejaVuSans` sur Linux). `Position` est une fraction de 0,0 à 1,0 de l'image vidéo.

**`OnNetworkSourceDisconnect` se déclenche à répétition** — le bloc source réessaie et échoue en succession rapide. Augmentez `RestartTimeoutMs` à 10-15 s sur un réseau réputé instable, ou enveloppez la journalisation avec un debounce.

**Exceptions de thread UI** — `OnNetworkSourceDisconnect` se déclenche depuis le thread du bus GStreamer. Utilisez `Dispatcher.Invoke` (WPF) / `Control.Invoke` (WinForms) / `MainThread.BeginInvokeOnMainThread` (MAUI) avant de toucher aux contrôles.

## Documentation associée

- [RTSP en profondeur](../network-streaming/rtsp.md) — fonctionnement de RTSP, options de transport et architecture de streaming
- [Configuration de source de caméra RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — référence `IPCameraSourceSettings` / `RTSPSourceSettings`
- [Intégration de caméra IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) — découverte + PTZ ; associez la redécouverte ONVIF à FallbackSwitch pour les caméras difficiles à atteindre
- [Lecteur RTSP Media Blocks](../../mediablocks/Guides/rtsp-player-csharp.md) — pipeline mono-caméra
- [Grille RTSP multi-caméras (mur NVR)](../../mediablocks/Guides/multi-camera-rtsp-grid.md) — mur 4×4 avec repli par cellule
- [Enregistrer un flux RTSP sans réencodage](../../mediablocks/Guides/rtsp-save-original-stream.md) — archiver en parallèle de l'aperçu en direct
- [Sortie serveur RTSP](../../mediablocks/RTSPServer/index.md) — résilience côté serveur pour votre propre flux
- [Matrice de prise en charge des plateformes](../../platform-matrix.md) — détails sur les codecs et l'accélération matérielle selon les plateformes

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code incluant l'aperçu RTSP, l'enregistrement et les démos MultiView.
