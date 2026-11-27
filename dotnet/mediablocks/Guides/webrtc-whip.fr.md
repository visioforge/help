---
title: Écran ou caméra vers un navigateur en WebRTC en C# .NET
description: Diffusez l'écran, une fenêtre ou une caméra d'une app C# vers des navigateurs en WebRTC sans serveur multimédia, et publiez vers MediaMTX ou Janus en WHIP.
sidebar_label: Écran et caméra vers le navigateur (WebRTC, WHIP)
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - iOS
  - Streaming
  - Screen Capture
  - Webcam
  - WebRTC
  - WHIP
  - VP8
  - H.264
  - Opus
  - C#
  - JavaScript
  - NuGet
primary_api_classes:
  - WebRTCSinkBlock
  - WebRTCSinkSettings
  - WHIPSinkBlock
  - WHIPSinkSettings
  - ScreenSourceBlock
  - SystemVideoSourceBlock
  - MediaBlocksPipeline
video:
  id: Zeltyn_FSMI
  upload_date: 2026-11-26T21:33:48Z
  duration: PT6M11S

---

# Diffuser l'écran ou une caméra vers des navigateurs web en WebRTC et publier avec WHIP en C# / .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Exemples de démonstration"
    La partie navigateur de ce guide suit le [WebRTC Browser Streamer Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20Browser%20Streamer%20Demo) : une caméra et un micro, un aperçu local et une page de visionnage servie par l'application. La partie WHIP suit le [WebRTC WHIP Streamer Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20WHIP%20Streamer%20Demo) et sa [version Avalonia](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Avalonia/WebRTC%20WHIP%20Streamer) pour Windows, macOS et Linux.

    La référence des blocs se trouve dans [Blocs sink → WebRTC](../Sinks/index.md#webrtc) et [Blocs sink → WHIP](../Sinks/index.md#whip).

## Tutoriel vidéo : caméra vers les navigateurs en WebRTC, et WHIP, en C# WinForms

Le tutoriel construit une application WinForms qui envoie une caméra vers les navigateurs avec `WebRTCSinkBlock` (l'application sert la page du lecteur et assure la signalisation, sans serveur multimédia) et compte les spectateurs à mesure qu'ils se connectent et partent. Avec une adresse WHIP, un `TeeBlock` envoie aussi la caméra, encodée une seule fois en H.264, vers MediaMTX avec `WHIPSinkBlock`, et l'exécution montre les deux lecteurs aux 30 images par seconde de la caméra.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Zeltyn_FSMI?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Son code : [webrtc-whip-winforms](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_VideoTutorials/webrtc-whip-winforms) sur GitHub.

### La version courte

Une application console en quelques lignes de C# : elle envoie la première webcam vers un navigateur en WebRTC. `WebRTCSinkBlock` sert la page du lecteur depuis le dossier `wwwroot` fourni avec l'exemple, sur le port 8090, et assure lui-même la signalisation. Lancez-la, ouvrez `http://localhost:8090/?signalling=8443` dans un navigateur du même PC, et appuyez sur Entrée pour l'arrêter. En plus de Media Blocks, elle a besoin du paquet `VisioForge.CrossPlatform.Core.Windows.Adds.x64`, qui apporte le plugin WebRTC.

```csharp
using System.IO;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();
await using var p = new MediaBlocksPipeline();
p.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var cams = await DeviceEnumerator.Shared
    .VideoSourcesAsync();
var cam = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(
        cams[0]));
// Sert la page et assure lui-même la signalisation
var webrtc = new WebRTCSinkBlock(new()
{
    WebServerAddress = "http://127.0.0.1:8090/",
    WebServerDirectory = Path.Combine(
        AppContext.BaseDirectory, "wwwroot")
});
p.Connect(cam.Output, webrtc.CreateNewInput(
    MediaBlockPadMediaType.Video));
if (!await p.StartAsync()) return;
// http://localhost:8090/?signalling=8443
Console.ReadLine();
await p.StopAsync();
```

Son code : [webrtc-browser-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_VideoTutorials/webrtc-browser-short-console) sur GitHub.

## Ce que vous allez construire

Deux façons de montrer en direct l'écran, une fenêtre d'application ou une caméra à des personnes qui n'ont qu'un navigateur web, toutes deux avec une latence inférieure à une seconde :

1. **Directement vers les navigateurs, sans serveur.** `WebRTCSinkBlock` fait de votre application .NET un serveur WebRTC : il sert la page de visionnage, assure la signalisation et envoie à chaque spectateur son propre flux WebRTC. Rien à installer à côté de l'application.
2. **Par un serveur multimédia, avec WHIP.** `WHIPSinkBlock` publie un flux vers un serveur multimédia WebRTC (MediaMTX, Janus, Cloudflare Stream) en WHIP (WebRTC-HTTP Ingestion Protocol). Le serveur le distribue ensuite à autant de spectateurs que nécessaire, y compris sur Internet.

![Une fenêtre d'application partagée depuis une app C# avec WebRTCSinkBlock et lue dans un navigateur ; l'horloge de la fenêtre a environ 0,15 seconde de retard sur l'horloge du navigateur](webrtc-whip-browser.webp)

```mermaid
graph LR;
    src["Écran, fenêtre ou caméra<br/>+ micro"] --> app;
    subgraph app ["Votre application .NET"]
        WebRTCSinkBlock;
        WHIPSinkBlock;
    end
    WebRTCSinkBlock -- "WebRTC, VP8 + Opus" --> b1["Navigateurs de votre réseau"];
    WHIPSinkBlock -- "WHIP : H.264 + Opus" --> server["Serveur multimédia<br/>MediaMTX, Janus"];
    server -- "WebRTC (WHEP)" --> b2["Navigateurs partout"];
```

Lequel choisir :

| | `WebRTCSinkBlock` | `WHIPSinkBlock` |
| --- | --- | --- |
| Logiciel supplémentaire | aucun | un serveur multimédia WebRTC |
| Encodage | VP8 et Opus, un encodeur par spectateur, avec contrôle de congestion par spectateur | H.264 et Opus, encodés une seule fois dans votre pipeline |
| CPU par spectateur supplémentaire | un encodeur VP8 de plus | aucun : le serveur relaie le flux |
| Page de visionnage | servie par le bloc, environ 100 lignes de JavaScript | le lecteur du serveur, ou tout lecteur WHEP |
| Plateformes | Windows, macOS, Linux, iOS | Windows, macOS, Linux, iOS |
| Usage typique | partage d'écran et vues de caméra pour quelques spectateurs sur un réseau local | diffusion à de nombreux spectateurs, ou via Internet |

## Prérequis

- Windows, macOS, Linux ou iOS.
- .NET 10 (.NET 8 ou ultérieur sous Linux).
- Media Blocks SDK .NET version 2026.11.21 ou ultérieure. Cette version apporte la correction de la capture d'écran vers `WebRTCSinkBlock` sur laquelle repose ce guide ; avec 2026.11.20, `WebRTCSinkBlock` lit les caméras, mais le spectateur d'une capture d'écran Direct3D 11 ne reçoit jamais d'image. Sous iOS, les deux blocs nécessitent le SDK et `VisioForge.CrossPlatform.Core.iOS` en 2026.11.23 ou ultérieure.
- Pour WHIP, un serveur multimédia WebRTC. Les exemples utilisent [MediaMTX](https://github.com/bluenviron/mediamtx), dont la configuration par défaut accepte WHIP sur le port 8889.

Paquets NuGet :

- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks/) : le SDK
- Windows : [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) et [VisioForge.CrossPlatform.Core.Windows.Adds.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Adds.x64/) : le runtime GStreamer et les plugins supplémentaires, dont `rswebrtc` (les deux blocs)
- macOS : [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS/) et [VisioForge.CrossPlatform.Core.macOS.Adds](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS.Adds/)
- Linux : `VisioForge.CrossPlatform.Core.Linux.x64` et `VisioForge.CrossPlatform.Core.Linux.Adds.x64` (`.arm64` pour ARM), plus le GStreamer de la distribution avec `gstreamer1.0-plugins-bad` et `gstreamer1.0-nice`
- iOS : [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS/), qui inclut `rswebrtc`

Sans le paquet `Adds`, `WebRTCSinkBlock.IsAvailable()` et `WHIPSinkBlock.IsAvailable()` renvoient `false` ; sous iOS, les deux sont fournis par `VisioForge.CrossPlatform.Core.iOS`. Pour le déploiement, consultez le [Guide de déploiement](../../deployment-x/index.md).

## Partie 1 : directement vers les navigateurs avec WebRTCSinkBlock

### Comment un spectateur se connecte

`WebRTCSinkBlock` exécute deux serveurs dans votre processus : un serveur web pour la page de visionnage et un serveur de signalisation WebSocket. La page demande le flux au serveur de signalisation, le bloc envoie une offre WebRTC et le navigateur répond. Ensuite, la vidéo passe par la propre connexion WebRTC du spectateur.

```mermaid
sequenceDiagram
    participant Browser as Navigateur
    participant Web as Serveur web (WebRTCSinkBlock)
    participant Sig as Serveur de signalisation (WebRTCSinkBlock)
    Browser->>Web: GET /?signalling=8443
    Web-->>Browser: index.html
    Browser->>Sig: WebSocket : inscription comme listener, liste des producteurs
    Browser->>Sig: ouverture d'une session avec le producteur
    Sig-->>Browser: offre SDP (VP8, Opus)
    Browser->>Sig: réponse SDP, candidats ICE
    Sig-->>Browser: candidats ICE
    Note over Browser,Sig: ConsumerAdded est déclenché
    Note over Browser,Sig: l'écran ou la caméra est lu par la propre connexion WebRTC du spectateur
```

Chaque spectateur obtient son propre encodeur et son propre contrôle de congestion : un spectateur sur un Wi-Fi lent reçoit un débit plus faible sans dégrader la qualité des autres.

### Le programme complet

Le programme console ci-dessous envoie l'écran principal (ou, avec `camera` sur la ligne de commande, la première caméra) et le premier micro à chaque navigateur qui ouvre `http://localhost:8090/?signalling=8443`.

<!-- vf-compile -->
```csharp
using System;
using System.IO;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

// 127.0.0.1 ne sert que cet ordinateur ; "0.0.0.0" sert aussi les spectateurs des autres ordinateurs.
const string Host = "127.0.0.1";
const int WebPort = 8090;
const int SignallingPort = 8443;

await VisioForgeX.InitSDKAsync();

MediaBlocksPipeline? pipeline = null;

try
{
    if (!WebRTCSinkBlock.IsAvailable())
    {
        Console.WriteLine("WebRTCSinkBlock is not available: add the VisioForge.CrossPlatform.Core.Windows.Adds.x64 package.");
        return;
    }

    pipeline = new MediaBlocksPipeline();
    pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

    // L'écran principal par défaut, la première caméra avec "camera" sur la ligne de commande.
    MediaBlock video;
    if (args.Length > 0 && args[0] == "camera")
    {
        var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (cameras.Length == 0)
        {
            Console.WriteLine("No camera found.");
            return;
        }

        video = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(cameras[0]));
    }
    else
    {
        video = new ScreenSourceBlock(new ScreenCaptureD3D11SourceSettings { FrameRate = new VideoFrameRate(30) });
    }

    // Son propre serveur de signalisation, plus un serveur web pour la page de visionnage dans wwwroot.
    var webrtc = new WebRTCSinkBlock(new WebRTCSinkSettings
    {
        SignallingServerHost = Host,
        SignallingServerPort = SignallingPort,
        WebServerDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
        WebServerAddress = $"http://{Host}:{WebPort}/"
    });
    webrtc.ConsumerAdded += (s, e) => Console.WriteLine($"Viewer connected: {e.PeerId}");
    webrtc.ConsumerRemoved += (s, e) => Console.WriteLine($"Viewer left: {e.PeerId}");

    // Vidéo brute : le sink l'encode en VP8 pour chaque spectateur.
    pipeline.Connect(video.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Video));

    // Le premier micro, s'il y en a un : encodé en Opus pour chaque spectateur.
    var microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
    if (microphones.Length > 0)
    {
        var microphone = new SystemAudioSourceBlock(microphones[0].CreateSourceSettings());
        pipeline.Connect(microphone.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Audio));
    }

    if (await pipeline.StartAsync())
    {
        Console.WriteLine($"Open http://localhost:{WebPort}/?signalling={SignallingPort} in a browser. Press Enter to stop.");
        Console.ReadLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (pipeline != null)
    {
        await pipeline.StopAsync();
        await pipeline.DisposeAsync();
    }

    VisioForgeX.DestroySDK();
}
```

Copiez la page de visionnage dans le dossier de sortie à la compilation ; ajoutez ceci au `.csproj` :

```xml
<ItemGroup>
  <Content Include="wwwroot\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

Rôle de chaque partie :

- **`ScreenSourceBlock` avec `ScreenCaptureD3D11SourceSettings`** capture le moniteur principal via Direct3D 11 à sa résolution native. `FrameRate` vaut 10 par défaut ; à 30, les mouvements de souris et le défilement sont fluides. `MonitorIndex` choisit un autre moniteur.
- **`SystemVideoSourceBlock`** ouvre une caméra. `new VideoCaptureDeviceSourceSettings(device)` choisit son format HD quand elle en a un ; définissez `Format` pour en choisir un autre.
- **`WebRTCSinkBlock.CreateNewInput`** crée une entrée par appel. Le bloc encode la vidéo et l'audio bruts pour chaque spectateur, la vidéo en VP8 et l'audio en Opus. Une vidéo déjà encodée en VP8 (avec `VPXEncoderBlock`) est envoyée telle quelle, sans encodeur par spectateur.
- **`WebServerDirectory`** active le serveur web du bloc et le fait pointer sur le dossier contenant `index.html`. Laissez-le à `null` pour servir la page vous-même (voir [Servir la page depuis votre propre serveur web](#servir-la-page-depuis-votre-propre-serveur-web)).
- **`SignallingServerHost`** et l'hôte de **`WebServerAddress`** déterminent qui peut se connecter : `127.0.0.1` n'accepte que cet ordinateur, `0.0.0.0` toutes les interfaces réseau.
- **`ConsumerAdded` / `ConsumerRemoved`** signalent chaque spectateur qui se connecte et qui part. Ils sont déclenchés sur un thread GStreamer : dans une application graphique, repassez sur le thread de l'interface, comme le fait la démo avec `Dispatcher.Invoke`. `ConsumerRemoved` n'est pas déclenché pour les spectateurs encore connectés à l'arrêt du pipeline ; leurs sessions se terminent avec lui.
- **`try` / `finally`** signale ce qui a échoué et, dans tous les cas, arrête et libère le pipeline et appelle `VisioForgeX.DestroySDK()`. Quand `StartAsync` renvoie `false`, la raison figure dans les messages `OnError` ; un port déjà utilisé est le cas le plus courant.

### La page de visionnage

La page parle le protocole de signalisation de l'élément GStreamer `webrtcsink` : des messages JSON sur un WebSocket. Enregistrez-la sous `wwwroot/index.html`. C'est la page du WebRTC Browser Streamer Demo : elle lit le premier flux annoncé par le serveur, réessaie tant que l'application n'est pas lancée et démarre en sourdine pour que le navigateur la lise sans clic.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>VisioForge WebRTC Viewer</title>
    <link rel="icon" href="data:,">
    <style>
        body { margin: 0; background: #111; color: #ddd; font-family: system-ui, sans-serif; display: flex; flex-direction: column; align-items: center; }
        video { width: 100%; max-width: 1280px; max-height: 80vh; background: #000; margin-top: 16px; }
        #bar { margin: 12px 16px; display: flex; gap: 12px; align-items: center; }
        button { padding: 6px 14px; }
    </style>
</head>
<body>
    <video id="video" autoplay playsinline muted></video>
    <div id="bar">
        <button id="unmute">Unmute</button>
        <span id="status">Connecting...</span>
    </div>

    <script>
        // Port de signalisation de WebRTCSinkBlock : ?signalling=8443 dans l'URL de la page.
        const port = new URLSearchParams(location.search).get('signalling') || '8443';
        const video = document.getElementById('video');
        const status = document.getElementById('status');
        let ws, pc, sessionId;

        document.getElementById('unmute').onclick = () => { video.muted = false; };

        function send(msg) { ws.send(JSON.stringify(msg)); }

        function connect() {
            // Une page https:// a besoin de wss:// : définissez SignallingServerCertificate et SignallingServerKey.
            ws = new WebSocket(`${location.protocol === 'https:' ? 'wss' : 'ws'}://${location.hostname}:${port}`);
            ws.onopen = () => send({ type: 'setPeerStatus', roles: ['listener'], meta: null });
            ws.onclose = () => { reset(); status.textContent = 'Disconnected, retrying...'; setTimeout(connect, 2000); };
            ws.onmessage = e => onMessage(e.data).catch(err => fail('Error: ' + err));
        }

        function startSession(peerId) {
            if (sessionId === undefined) {
                sessionId = null;
                send({ type: 'startSession', peerId });
            }
        }

        async function onMessage(data) {
            const msg = JSON.parse(data);
            switch (msg.type) {
                case 'welcome':
                    send({ type: 'list' });
                    break;
                case 'list':
                    if (msg.producers.length > 0) startSession(msg.producers[0].id);
                    else status.textContent = 'Waiting for the stream...';
                    break;
                case 'peerStatusChanged':
                    if (msg.roles.includes('producer')) startSession(msg.peerId);
                    break;
                case 'sessionStarted':
                    sessionId = msg.sessionId;
                    break;
                case 'peer':
                    await onPeer(msg);
                    break;
                case 'endSession':
                    reset();
                    status.textContent = 'Stream ended, waiting...';
                    listLater();
                    break;
                case 'error':
                    fail('Error: ' + msg.details);
                    break;
            }
        }

        async function onPeer(msg) {
            if (msg.sdp && msg.sdp.type === 'offer') {
                // Reprenez ici les StunServer et TurnServers du sink lorsqu'ils sont définis.
                if (pc) { pc.close(); video.srcObject = null; }
                pc = new RTCPeerConnection({ iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] });
                const conn = pc;
                pc.onconnectionstatechange = () => {
                    if (conn === pc && conn.connectionState === 'failed') {
                        video.srcObject = null;
                        fail('Connection lost');
                    }
                };
                pc.ontrack = e => { video.srcObject = e.streams[0] || new MediaStream([e.track]); status.textContent = 'Playing'; };
                pc.onicecandidate = e => {
                    if (e.candidate) send({ type: 'peer', sessionId, ice: { candidate: e.candidate.candidate, sdpMLineIndex: e.candidate.sdpMLineIndex } });
                };
                await pc.setRemoteDescription(msg.sdp);
                const answer = await pc.createAnswer();
                await pc.setLocalDescription(answer);
                send({ type: 'peer', sessionId, sdp: { type: 'answer', sdp: answer.sdp } });
            } else if (msg.ice && pc) {
                await pc.addIceCandidate({ candidate: msg.ice.candidate, sdpMLineIndex: msg.ice.sdpMLineIndex });
            }
        }

        // Une erreur pendant la lecture d'un flux (message périmé, candidat refusé) ne l'interrompt pas ;
        // sinon la session est terminée et la liste des producteurs redemandée.
        function fail(text) {
            if (video.srcObject) {
                status.textContent = text;
                return;
            }

            if (sessionId) send({ type: 'endSession', sessionId });
            reset();
            status.textContent = text + ', retrying...';
            listLater();
        }

        // Redemande la liste des producteurs après une pause, pour qu'une session en échec ne réessaie pas en boucle serrée.
        function listLater() {
            const socket = ws;
            setTimeout(() => { if (ws === socket && ws.readyState === WebSocket.OPEN) send({ type: 'list' }); }, 2000);
        }

        function reset() {
            if (pc) pc.close();
            pc = undefined;
            sessionId = undefined;
            video.srcObject = null;
        }

        connect();
    </script>
</body>
</html>
```

L'échange, étape par étape :

1. **S'inscrire comme listener.** La page ouvre un WebSocket vers le port de signalisation, envoie `setPeerStatus` avec le rôle `listener`, puis demande la liste des producteurs (`list`). Votre application est le producteur.
2. **Ouvrir une session.** La page envoie `startSession` avec l'identifiant du producteur ; un producteur qui apparaît plus tard est annoncé par `peerStatusChanged`, la page peut donc être ouverte avant le démarrage de l'application.
3. **Répondre à l'offre.** Le bloc envoie l'offre SDP dans un message `peer` ; la page crée un `RTCPeerConnection`, répond, et les deux côtés échangent leurs candidats ICE dans d'autres messages `peer`.
4. **Lire.** `ontrack` remet le flux à l'élément `<video>`. L'état affiche `Playing` ; **Unmute** active le son, car les navigateurs lisent une vidéo muette sans clic et une vidéo sonore seulement après.
5. **Se rétablir.** Quand la connexion échoue ou que l'application s'arrête, la page se réinitialise et redemande la liste ; elle se reconnecte d'elle-même au retour du flux.

### Partager une fenêtre plutôt que tout l'écran

Définissez `WindowHandle` pour capturer une seule fenêtre d'application via Windows Graphics Capture. La fenêtre est capturée même lorsque d'autres fenêtres la recouvrent, et le reste du bureau reste privé :

```csharp
var screen = new ScreenCaptureD3D11SourceSettings
{
    FrameRate = new VideoFrameRate(30),
    WindowHandle = windowHandle // HWND de la fenêtre à partager, par exemple Process.MainWindowHandle
};

var video = new ScreenSourceBlock(screen);
```

Le flux prend la taille de la fenêtre au démarrage de la capture. Une fenêtre réduite ne fournit aucune image tant qu'elle n'est pas restaurée.

### Servir la page depuis votre propre serveur web

Le serveur web du bloc est facultatif. Avec `WebServerDirectory` laissé à `null`, seul le serveur de signalisation tourne ; placez `index.html` dans votre propre site (ASP.NET Core, IIS, nginx) et passez le port de signalisation dans la chaîne de requête. La page se connecte à `ws://<hôte de la page>:<port>`, ou à `wss://` quand la page elle-même est en `https://` : le serveur de signalisation doit donc être joignable sous le même nom d'hôte que la page.

### Une page de visionnage sur un site HTTPS

Un navigateur bloque une connexion `ws://` depuis une page `https://`. Donnez au serveur de signalisation le certificat et la clé privée de votre site sous forme de fichiers PEM, et il parle `wss://` :

```csharp
var webrtcSink = new WebRTCSinkBlock(new WebRTCSinkSettings
{
    SignallingServerHost = "0.0.0.0",
    SignallingServerPort = 8443,
    SignallingServerCertificate = @"C:\certs\fullchain.pem",
    SignallingServerKey = @"C:\certs\privkey.pem"
});
```

Le certificat doit être valide pour le nom d'hôte de l'adresse de la page, et la clé ne doit pas être chiffrée. `Build` échoue, avec un message `OnError`, si un seul des deux est défini, si un fichier est illisible, si la clé n'est pas celle du certificat (vérifié sous .NET 5 et versions ultérieures) ou si `WebServerDirectory` est également défini. Le serveur web du bloc ne sert que `http://` : placez une page `https://` sur votre propre serveur web (IIS, nginx, ASP.NET Core) et laissez `WebServerDirectory` à `null`.

### Spectateurs sur d'autres ordinateurs

1. Définissez `Host` sur `"0.0.0.0"` pour que les deux serveurs écoutent sur toutes les interfaces réseau.
2. Autorisez le port web et le port de signalisation (ici 8090 et 8443) dans le Pare-feu Windows Defender, ainsi que l'application elle-même pour les médias WebRTC, qui utilisent des ports UDP choisis à l'exécution.
3. Ouvrez `http://<adresse de cet ordinateur>:8090/?signalling=8443` sur l'autre ordinateur.

La réception WebRTC ne demande pas HTTPS, donc `http://` suffit sur un réseau local : nous avons ouvert la page de la démo par l'adresse réseau de l'ordinateur, que le navigateur considère comme non sécurisée, et la caméra s'est affichée. `WebRTCSinkSettings` n'a aucun réglage d'authentification : quiconque atteint les deux ports peut regarder. Gardez `127.0.0.1` quand le flux ne doit pas quitter l'ordinateur, et utilisez WHIP avec un serveur multimédia qui vérifie des jetons quand il doit atteindre Internet.

## Partie 2 : publier vers un serveur multimédia avec WHIPSinkBlock

### Le pipeline

WHIP accepte de la vidéo H.264 et de l'audio Opus déjà encodés. Le pipeline encode une seule fois ; le serveur multimédia relaie le flux à chaque spectateur.

```mermaid
graph LR;
    ScreenSourceBlock -- "vidéo brute" --> H264EncoderBlock;
    SystemAudioSourceBlock -- "audio brut" --> OPUSEncoderBlock;
    H264EncoderBlock -- H.264 --> WHIPSinkBlock;
    OPUSEncoderBlock -- Opus --> WHIPSinkBlock;
    WHIPSinkBlock -- "WHIP (HTTP + WebRTC)" --> MediaMTX;
```

### Le programme complet

Le programme console publie l'écran principal et le premier micro vers `http://<serveur>:8889/screen/whip`, le point de terminaison WHIP de MediaMTX pour un flux nommé `screen`. Passez un autre point de terminaison sur la ligne de commande. L'écran est capturé avec `ScreenCaptureD3D11SourceSettings`, ce programme se compile donc pour Windows ; sous macOS utilisez `ScreenCaptureMacOSSourceSettings`, sous Linux `ScreenCaptureXDisplaySourceSettings`, ou une caméra, comme le fait la démo Avalonia.

<!-- vf-compile -->
```csharp
using System;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Le point de terminaison WHIP du serveur multimédia ; MediaMTX accepte http://<serveur>:8889/<nom du flux>/whip.
var whipUrl = args.Length > 0 ? args[0] : "http://192.168.1.10:8889/screen/whip";

await VisioForgeX.InitSDKAsync();

MediaBlocksPipeline? pipeline = null;

try
{
    if (!WHIPSinkBlock.IsAvailable())
    {
        Console.WriteLine("WHIPSinkBlock is not available: add the VisioForge.CrossPlatform.Core.*.Adds package for this platform.");
        return;
    }

    pipeline = new MediaBlocksPipeline();
    pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

    var screen = new ScreenSourceBlock(new ScreenCaptureD3D11SourceSettings { FrameRate = new VideoFrameRate(30) });

    // WHIP transporte de la vidéo H.264 et de l'audio Opus : encodage unique ici, le serveur sert chaque spectateur.
    var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
    var whip = new WHIPSinkBlock(new WHIPSinkSettings { Location = whipUrl });

    pipeline.Connect(screen.Output, h264.Input);
    pipeline.Connect(h264.Output, whip.CreateNewInput(MediaBlockPadMediaType.Video));

    var microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
    if (microphones.Length > 0)
    {
        var microphone = new SystemAudioSourceBlock(microphones[0].CreateSourceSettings());
        var opus = new OPUSEncoderBlock(new OPUSEncoderSettings());
        pipeline.Connect(microphone.Output, opus.Input);
        pipeline.Connect(opus.Output, whip.CreateNewInput(MediaBlockPadMediaType.Audio));
    }

    if (await pipeline.StartAsync())
    {
        Console.WriteLine($"Publishing to {whipUrl}. Press Enter to stop.");
        Console.ReadLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (pipeline != null)
    {
        await pipeline.StopAsync();
        await pipeline.DisposeAsync();
    }

    VisioForgeX.DestroySDK();
}
```

Rôle de chaque partie :

- **`H264EncoderBlock` avec `OpenH264EncoderSettings`** encode en logiciel, sur tout ordinateur Windows, avec le profil Constrained Baseline que décodent tous les navigateurs. Sur un ordinateur doté d'un GPU NVIDIA, AMD ou Intel, les réglages d'un encodeur matériel (`NVENCH264EncoderSettings`, `AMFH264EncoderSettings`, `QSVH264EncoderSettings`) déchargent le CPU de l'encodage.
- **`OPUSEncoderBlock`** encode le micro en Opus, le codec audio de WebRTC.
- **`WHIPSinkBlock.CreateNewInput`** crée l'entrée vidéo et l'entrée audio. Le bloc ajoute lui-même l'encapsulation RTP, les encodeurs s'y connectent donc directement. L'audio est facultatif : sans micro, le flux est uniquement vidéo.
- **`WHIPSinkSettings.Location`** est le point de terminaison WHIP. Au `StartAsync`, le bloc y envoie son offre SDP, le serveur répond et les médias circulent en WebRTC. À l'arrêt, le bloc supprime la session sur le serveur.

### Regarder le flux

MediaMTX sert son propre lecteur pour chaque flux à `http://<serveur>:8889/<nom du flux>/`, ici `http://<serveur>:8889/screen/`. Tout lecteur WHEP peut aussi le lire depuis `http://<serveur>:8889/screen/whep`, y compris le [`WHEPSourceBlock`](../Sources/index.md#bloc-source-whep) du SDK.

![Une fenêtre d'application publiée vers MediaMTX avec WHIPSinkBlock, lue dans le lecteur web de MediaMTX](webrtc-whip-mediamtx.webp)

### Authentification, STUN et TURN

```csharp
var whip = new WHIPSinkBlock(new WHIPSinkSettings
{
    Location = "https://media.example.com/live/whip",
    AuthToken = "secret-token",                     // envoyé sous la forme "Authorization: Bearer secret-token"
    StunServer = "stun://stun.l.google.com:19302",
    TurnServer = "turn://user:password@turn.example.com:3478"
});
```

- **`AuthToken`** : le jeton bearer des services qui en exigent un (Cloudflare Stream, un serveur Janus ou MediaMTX avec authentification).
- **`StunServer` / `TurnServer`** : la part de l'éditeur dans la traversée de NAT. Non défini, le bloc n'utilise aucun serveur STUN, ce qui suffit quand l'application et le serveur sont sur le même réseau. `UseLinkHeaders` (activé par défaut) reprend aussi les serveurs ICE que le serveur WHIP annonce dans ses en-têtes `Link`.
- **`IceTransportPolicy = WHIPIceTransportPolicy.Relay`** fait tout passer par le serveur TURN, pour les réseaux qui bloquent l'UDP direct.
- **`Timeout`** (15 secondes par défaut) borne chaque requête HTTP WHIP, y compris le `DELETE` de la session à l'arrêt.

## Les deux depuis une seule capture

Une capture peut alimenter les deux sorties à la fois : les spectateurs du réseau local reçoivent le flux directement de l'application, et le serveur multimédia reçoit le flux WHIP pour tous les autres. `TeeBlock` divise la vidéo :

```csharp
var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);
pipeline.Connect(screen.Output, tee.Input);

// Spectateurs locaux : vidéo brute, encodée en VP8 par spectateur.
pipeline.Connect(tee.Outputs[0], webrtc.CreateNewInput(MediaBlockPadMediaType.Video));

// Le serveur multimédia : H.264, encodé une seule fois.
pipeline.Connect(tee.Outputs[1], h264.Input);
pipeline.Connect(h264.Output, whip.CreateNewInput(MediaBlockPadMediaType.Video));
```

`TeeBlock` se trouve dans `VisioForge.Core.MediaBlocks.Special`. Les deux sorties démarrent et s'arrêtent ensemble : un serveur WHIP injoignable fait échouer le démarrage de tout le pipeline. Placez-les dans deux pipelines quand le flux local doit continuer sans le serveur.

## Limites à connaître avant la mise en production

### Plateformes

`WebRTCSinkBlock` fonctionne sous Windows, macOS et Linux avec le paquet `Adds` de la plateforme, et sous iOS avec `VisioForge.CrossPlatform.Core.iOS` 2026.11.23 ou ultérieure : un `Core.iOS` antérieur contient le plugin sans ses corrections, si bien que le sink démarre, mais `StopAsync` peut se bloquer quand un spectateur part pendant sa négociation. `WHIPSinkBlock` fonctionne sur les mêmes plateformes avec les mêmes paquets. Sous Linux, il leur faut aussi `gstreamer1.0-plugins-bad` et `gstreamer1.0-nice` de la distribution. Vérifiez `IsAvailable()` au démarrage, comme le font les deux programmes.

### Codecs

`WebRTCSinkBlock` envoie de la vidéo VP8 et de l'audio Opus, que lisent tous les navigateurs actuels. Il accepte la vidéo et l'audio bruts, ou du VP8 et de l'Opus déjà encodés ; il n'accepte pas le H.264. `WHIPSinkBlock` n'accepte que H.264 et Opus.

### Latence

Dans notre test sur un ordinateur Windows, un changement dans une image 320×240 à 30 images par seconde a atteint l'image décodée du spectateur 55 à 60 ms après avoir quitté la source (médiane de 58 ms), avant le délai de lecture du spectateur. En ajoutant la capture d'une fenêtre, la lecture et l'affichage dans le navigateur, la capture d'écran du début de ce guide montre environ 0,15 seconde.

### CPU et résolution

`WebRTCSinkBlock` exécute un encodeur VP8 logiciel par spectateur. Lors de nos tests sur un ordinateur Windows, une caméra 1080p et une fenêtre de 1902×1131 ont été lues à 30 images par seconde, tandis qu'un moniteur entier de 3840×2160 atteignait environ 20 images par seconde dans le navigateur. Pour un écran 4K, réduisez la vidéo avant le sink avec `VideoResizeBlock` (espace de noms `VisioForge.Core.MediaBlocks.VideoProcessing`) :

```csharp
var resize = new VideoResizeBlock(1920, 1080);
pipeline.Connect(screen.Output, resize.Input);
pipeline.Connect(resize.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Video));
```

Au-delà de quelques spectateurs, publiez une seule fois en WHIP et laissez le serveur multimédia les servir.

### Débit

`StartBitrate` (2 Mbit/s) est le point de départ de chaque spectateur ; le contrôle de congestion (`CongestionControl`, Google Congestion Control par défaut) le monte jusqu'à `MaxBitrate` (8 Mbit/s) tant que le réseau suit, et le baisse sinon. Augmentez `MaxBitrate` pour un texte net sur un grand écran ; définissez `CongestionControl = WebRTCSinkCongestionControl.Disabled` pour maintenir chaque spectateur à `StartBitrate`.

### Ports

Les valeurs par défaut sont 8443 pour la signalisation et 8080 pour le serveur web. Quand un port est occupé, `StartAsync` renvoie `false` et `OnError` indique lequel. Les programmes de ce guide utilisent 8090 pour la page.

### NAT, STUN et TURN

Sur un réseau local, aucun serveur STUN n'est nécessaire. Pour des spectateurs derrière d'autres réseaux, définissez `StunServer` et `TurnServers` dans `WebRTCSinkSettings` (`StunServer` vaut `stun://stun.l.google.com:19302` par défaut) et les mêmes serveurs dans la liste `iceServers` de la page. Via Internet, WHIP vers un serveur multimédia doté d'une adresse publique est généralement la voie la plus simple.

## Dépannage

| Symptôme | Cause et solution |
| --- | --- |
| `IsAvailable()` renvoie `false` | Le paquet `Adds` de la plateforme n'est pas référencé (sous iOS, `VisioForge.CrossPlatform.Core.iOS` manque ou le SDK est antérieur à 2026.11.23), ou sous Linux `gstreamer1.0-plugins-bad` et `gstreamer1.0-nice` manquent. |
| `StartAsync` renvoie `false` et `OnError` indique que le serveur web ne peut pas écouter | Un autre programme utilise le port web. Choisissez un autre port dans `WebServerAddress`, ou arrêtez l'autre programme. |
| La page affiche `Disconnected, retrying...` | Le serveur de signalisation est injoignable : l'application n'est pas lancée, le port `?signalling=` est faux, ou le pare-feu le bloque. |
| La page affiche `Waiting for the stream...` | La page a atteint le serveur de signalisation, mais le pipeline n'est pas encore en lecture. Vérifiez `StartAsync` et les messages `OnError`. |
| La page affiche `Playing` mais il n'y a pas de son | La vidéo démarre en sourdine. Cliquez sur **Unmute**. |
| Un spectateur sur un autre ordinateur reste sur `Connection lost` | Les médias WebRTC sont bloqués : autorisez l'application dans le pare-feu, ou configurez STUN/TURN des deux côtés. |
| Une capture d'écran Direct3D 11 n'est jamais lue dans le navigateur et la page se reconnecte sans cesse | Media Blocks SDK .NET 2026.11.20 ou antérieur. Passez à 2026.11.21 ou ultérieur. |
| L'écran est lu avec peu d'images par seconde | Un écran 4K est encodé en logiciel pour chaque spectateur. Réduisez-le en 1080p avec `VideoResizeBlock`, ou publiez en WHIP. |
| WHIP : `StartAsync` renvoie `false` ou `OnError` signale une erreur HTTP | Le point de terminaison est faux, le serveur est arrêté ou il a refusé le jeton. Ouvrez la page du lecteur de la même URL dans un navigateur pour vérifier le serveur. |
| WHIP : le flux atteint le serveur mais reste noir ou affiche `ICE failed` | L'UDP entre l'application et le serveur est bloqué. Autorisez le port UDP WebRTC du serveur (8189 sur MediaMTX), ou configurez un serveur TURN avec `IceTransportPolicy = Relay`. |

## Questions fréquentes

### Faut-il un serveur multimédia pour afficher mon écran dans un navigateur ?

Non. `WebRTCSinkBlock` sert la page, assure la signalisation et diffuse vers chaque navigateur depuis votre processus. Un serveur multimédia se justifie quand les spectateurs sont nombreux ou sur Internet ; publiez alors vers lui avec `WHIPSinkBlock`.

### Pourquoi VP8 pour les navigateurs et H.264 pour WHIP ?

`WebRTCSinkBlock` encode par spectateur et utilise VP8, que décodent tous les navigateurs et que le bloc peut adapter à la bande passante de chaque spectateur. Les serveurs WHIP comme MediaMTX relaient un seul flux à tous les spectateurs sans réencoder : le flux doit donc déjà être dans un codec qu'ils acceptent ; H.264 est celui qu'acceptent tous les serveurs et navigateurs.

### Combien de spectateurs WebRTCSinkBlock peut-il servir ?

Chaque spectateur coûte un encodeur VP8. Un flux 1080p sert quelques spectateurs sur un CPU de bureau ; en 4K, le premier spectateur est déjà la limite. Pour davantage de spectateurs, publiez une seule fois en WHIP.

### Puis-je ajouter de l'audio ?

Oui. Connectez une source audio brute à `CreateNewInput(MediaBlockPadMediaType.Audio)` de `WebRTCSinkBlock`, qui encode en Opus par spectateur, ou la sortie d'un `OPUSEncoderBlock` à l'entrée audio de `WHIPSinkBlock`.

### Cela fonctionne-t-il sous macOS et Linux ?

Oui, les deux, et aussi sous iOS. Pour servir des caméras sans page de visionnage à vous, utilisez [`WHEPServerSinkBlock`](rtsp-cameras-in-browser-whep.md).

### Un navigateur peut-il lire le flux WHIP sans la page du serveur multimédia ?

Oui. Le serveur expose le flux en WHEP, et une page d'une vingtaine de lignes de JavaScript le lit : le lecteur de [Caméras IP RTSP dans le navigateur avec WHEP](rtsp-cameras-in-browser-whep.md#cote-navigateur-un-lecteur-whep-minimal) fonctionne avec l'URL `/whep` de MediaMTX.

## Voir aussi

- [Blocs sink → WebRTC](../Sinks/index.md#webrtc) : référence de `WebRTCSinkBlock` et `WebRTCSinkSettings`
- [Blocs sink → WHIP](../Sinks/index.md#whip) : référence de `WHIPSinkBlock` et `WHIPSinkSettings`
- [Caméras IP RTSP dans le navigateur avec WHEP](rtsp-cameras-in-browser-whep.md) : des caméras IP dans une page web sans réencodage
- [Bloc source écran](../Sources/index.md#bloc-source-ecran) : réglages de capture d'écran, de fenêtre et de moniteur
- [Enregistrement pré-événement](pre-event-recording.md) : conservez les secondes précédant un événement pendant la diffusion
