---
title: Caméras IP RTSP dans le navigateur avec WHEP en C# .NET
description: Affichez des caméras IP RTSP dans un navigateur en WebRTC (WHEP) depuis une app C#, avec une latence sous la seconde, sans réencodage ni serveur média.
sidebar_label: Caméras RTSP dans le navigateur (WHEP)
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - Streaming
  - IP Camera
  - RTSP
  - WebRTC
  - H.264
  - C#
  - JavaScript
  - NuGet
primary_api_classes:
  - WHEPServerSinkBlock
  - WHEPServerSinkSettings
  - RTSPRAWSourceBlock
  - RTSPRAWSourceSettings
  - MediaBlocksPipeline

---

# Afficher des caméras IP RTSP dans un navigateur web avec WHEP en C# / .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Exemple de démonstration"
    Le code de ce guide provient du RTSP WHEP Camera Grid Demo (WPF) du [dépôt d'exemples](https://github.com/visioforge/.Net-SDK-s-samples) : il ajoute des caméras par URL RTSP, sert une page avec une grille de toutes les caméras et transfère les requêtes WHEP de chaque caméra vers son propre serveur.

    La référence du bloc se trouve dans [Blocs sink → Serveur WHEP](../Sinks/index.md#serveur-whep).

## Ce que vous allez construire

Un navigateur ne sait pas ouvrir une URL RTSP. Les moyens habituels d'afficher une caméra IP sur une page web sont de la transcoder en HLS (plusieurs secondes de retard, et un décodeur plus un encodeur par caméra) ou de faire tourner un serveur média séparé à côté de votre application. Ce guide suit une troisième voie : votre application .NET devient elle-même un serveur WebRTC, et le H.264 de la caméra arrive au navigateur exactement tel que la caméra l'a envoyé.

Le pipeline d'une caméra tient en deux blocs :

```mermaid
graph LR;
    RTSPRAWSourceBlock -- H.264 --> WHEPServerSinkBlock;
```

- `RTSPRAWSourceBlock` se connecte à la caméra et fournit son H.264 encodé sans le décoder.
- `WHEPServerSinkBlock` exécute un serveur WHEP (WebRTC-HTTP Egress Protocol). Un navigateur lui envoie une offre WebRTC en HTTP, reçoit une réponse et obtient la vidéo par sa propre session WebRTC.

Ce que vous y gagnez :

- **Une latence sous la seconde.** Le navigateur lit le flux en WebRTC, et non par un protocole à segments.
- **Presque aucun CPU par caméra, quel que soit le nombre de spectateurs.** Rien n'est décodé ni encodé : le bloc envoie le flux de la caméra à chaque spectateur tel qu'il arrive. Chaque spectateur consomme toujours une bande passante réseau égale au débit de la caméra.
- **Pas de serveur média.** Le serveur WHEP tourne dans votre processus ; côté navigateur, une vingtaine de lignes de JavaScript suffisent, sans bibliothèque de lecteur.

WHEP est un petit protocole HTTP au-dessus de WebRTC : un `POST` avec l'offre SDP, un `201 Created` avec la réponse SDP et un en-tête `Location` pour la session, puis un `DELETE` sur ce `Location` quand le spectateur s'en va. N'importe quel lecteur WHEP peut utiliser le endpoint, pas seulement la page de ce guide.

## Prérequis

- Windows x64. `WHEPServerSinkBlock` utilise le plugin GStreamer `rswebrtc`, livré dans le paquet de runtime `Adds` pour Windows.
- .NET 10 (la démo cible `net10.0-windows`).
- Une caméra IP avec un flux RTSP H.264.
- Media Blocks SDK .NET version 2026.11.18 ou ultérieure, la version qui ajoute `WHEPServerSinkBlock`.

Paquets NuGet :

- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks/) — le SDK
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — runtime GStreamer pour Windows x64
- [VisioForge.CrossPlatform.Core.Windows.Adds.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Adds.x64/) — les plugins supplémentaires, dont `rswebrtc`

Sans le paquet `Adds`, `WHEPServerSinkBlock.IsAvailable()` renvoie `false`. Pour les détails de déploiement, consultez le [Guide de déploiement](../../deployment-x/index.md).

La page du navigateur est un fichier servi par l'application : copiez-la dans le dossier de sortie depuis le `.csproj`, comme le fait la démo :

```xml
<ItemGroup>
  <Content Include="wwwroot\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

## La première chose à savoir : la même origine

Le serveur WHEP n'envoie pas d'en-têtes CORS. Un navigateur n'accepte donc ses réponses que sur une page servie depuis la même adresse et le même port que le endpoint. Ouvrir `index.html` depuis le disque, ou le servir depuis un autre port, échoue au `fetch` de l'offre.

La démo règle cela de la façon la plus simple : l'application fait tourner un petit serveur HTTP pour la page et transfère chaque requête sous `/whep/` vers le serveur WHEP, qui lui-même n'écoute qu'en loopback. Le navigateur ne parle qu'à une seule origine ; le serveur WHEP n'est jamais exposé directement.

```mermaid
sequenceDiagram
    participant Browser as Navigateur
    participant Page as Serveur de la page (votre app)
    participant WHEPServerSinkBlock
    Browser->>Page: GET /
    Page-->>Browser: index.html
    Browser->>Page: POST /whep/endpoint (offre SDP)
    Page->>WHEPServerSinkBlock: POST /whep/endpoint
    WHEPServerSinkBlock-->>Page: 201 Created, réponse SDP, Location
    Page-->>Browser: 201 Created, réponse SDP, Location
    WHEPServerSinkBlock-)Browser: H.264 de la caméra en WebRTC
    Browser->>Page: DELETE Location (page fermée)
    Page->>WHEPServerSinkBlock: DELETE Location
```

Seul l'échange HTTP passe par le serveur de la page. La vidéo circule par la connexion WebRTC que l'offre et la réponse établissent.

## Une caméra : le serveur complet

Le programme console ci-dessous reprend le code de la démo pour une seule caméra. Il construit le pipeline, sert `wwwroot/index.html` sur `http://localhost:8080/` et transfère `/whep/...` vers le serveur WHEP sur `127.0.0.1:9100`.

```csharp
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

const string PageUrl = "http://localhost:8080/";
const string WhepServer = "http://127.0.0.1:9100";

await VisioForgeX.InitSDKAsync();

using var http = new HttpClient();
MediaBlocksPipeline? pipeline = null;
HttpListener? pageServer = null;

try
{
    if (!WHEPServerSinkBlock.IsAvailable())
    {
        Console.WriteLine("WHEPServerSinkBlock is not available: the VisioForge.CrossPlatform.Core.Windows.Adds.x64 package is missing.");
        return;
    }

    pipeline = new MediaBlocksPipeline();
    pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

    // H.264 encodé directement depuis la caméra, sans audio : les navigateurs acceptent l'Opus, pas l'AAC de la caméra.
    var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(new Uri("rtsp://192.168.1.64:554/stream1"), "admin", "password", audioEnabled: false);
    var rtsp = new RTSPRAWSourceBlock(rtspSettings);

    // Le serveur WHEP n'écoute qu'en loopback ; les navigateurs l'atteignent par le serveur de la page ci-dessous.
    var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings { ServerAddress = WhepServer });
    whep.ConsumerAdded += (s, e) => Console.WriteLine($"Viewer connected: {e.PeerId}");
    whep.ConsumerRemoved += (s, e) => Console.WriteLine($"Viewer left: {e.PeerId}");

    pipeline.Connect(rtsp.VideoOutput, whep.Input);

    // La page et les requêtes WHEP partagent une origine : whepserversink n'envoie pas d'en-têtes CORS.
    pageServer = new HttpListener();
    pageServer.Prefixes.Add(PageUrl);
    pageServer.Start();
    _ = ServePageAsync(pageServer);

    if (await pipeline.StartAsync())
    {
        Console.WriteLine($"Open {PageUrl} in a browser. Press Enter to stop.");
        Console.ReadLine();
    }
}
catch (Exception ex)
{
    // CreateAsync lève une exception s'il ne peut pas lire la caméra ; HttpListener.Start, si le port de la page est occupé.
    Console.WriteLine(ex.Message);
}
finally
{
    pageServer?.Close();

    if (pipeline != null)
    {
        await pipeline.StopAsync();
        await pipeline.DisposeAsync();
    }

    VisioForgeX.DestroySDK();
}

async Task ServePageAsync(HttpListener server)
{
    while (server.IsListening)
    {
        HttpListenerContext context;
        try
        {
            context = await server.GetContextAsync();
        }
        catch (Exception ex) when (ex is HttpListenerException || ex is ObjectDisposedException)
        {
            return;
        }

        _ = HandleRequestAsync(context);
    }
}

async Task HandleRequestAsync(HttpListenerContext context)
{
    var request = context.Request;
    var response = context.Response;

    try
    {
        var path = request.Url?.AbsolutePath ?? string.Empty;

        if (path == "/")
        {
            await WriteAsync(response, "text/html", File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html")));
        }
        else if (path.StartsWith("/whep/", StringComparison.Ordinal))
        {
            // L'offre et le DELETE de la session partent vers le serveur WHEP sans modification.
            using var forward = new HttpRequestMessage(new HttpMethod(request.HttpMethod), WhepServer + path);
            if (request.HasEntityBody)
            {
                using var body = new MemoryStream();
                await request.InputStream.CopyToAsync(body);
                forward.Content = new ByteArrayContent(body.ToArray());
                forward.Content.Headers.TryAddWithoutValidation("Content-Type", request.ContentType);
            }

            using var reply = await http.SendAsync(forward);
            if (reply.Headers.Location != null)
            {
                // L'URL de la session que le lecteur supprime en partant.
                response.Headers["Location"] = reply.Headers.Location.OriginalString;
            }

            response.StatusCode = (int)reply.StatusCode;
            await WriteAsync(response, reply.Content.Headers.ContentType?.ToString(), await reply.Content.ReadAsByteArrayAsync());
        }
        else
        {
            response.StatusCode = 404;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        response.StatusCode = 502;
    }
    finally
    {
        response.Close();
    }
}

static async Task WriteAsync(HttpListenerResponse response, string? contentType, byte[] body)
{
    if (contentType != null)
    {
        response.ContentType = contentType;
    }

    response.ContentLength64 = body.Length;
    await response.OutputStream.WriteAsync(body);
}
```

Rôle de chaque partie :

- **`RTSPRAWSourceSettings.CreateAsync`** se connecte à la caméra et lit la description de son flux avant la construction du pipeline. `audioEnabled: false` laisse de côté l'audio de la caméra : le serveur WHEP n'a qu'une entrée vidéo, et les navigateurs acceptent l'Opus en WebRTC, pas l'AAC ou le G.711 qu'envoie une caméra. `CreateAsync` lève `InvalidOperationException` lorsqu'il ne peut pas lire le flux : caméra hors ligne, URL incorrecte ou identifiants refusés.
- **`RTSPRAWSourceBlock.VideoOutput`** porte le H.264 encodé de la caméra. Le relier directement à **`WHEPServerSinkBlock.Input`** constitue tout le chemin média : ni décodeur, ni encodeur.
- **`WHEPServerSinkSettings.ServerAddress`** est l'adresse d'écoute du serveur WHEP. `127.0.0.1` ne le rend accessible que depuis cette machine, ce qui convient quand le serveur de la page lui transfère les requêtes. La propriété `EndpointUrl` du bloc renvoie l'URL à laquelle les lecteurs envoient leur offre, ici `http://127.0.0.1:9100/whep/endpoint`.
- **`ConsumerAdded` / `ConsumerRemoved`** signalent chaque spectateur qui se connecte et s'en va, avec son identifiant de session dans `PeerId`. Ils sont déclenchés sur un thread GStreamer : dans une application avec interface, passez au thread de l'interface sans le bloquer ; la démo utilise `Dispatcher.BeginInvoke`. `ConsumerRemoved` n'est pas déclenché pour les spectateurs encore connectés à l'arrêt du pipeline ; leurs sessions se terminent avec lui.
- **`try` / `finally`** signale ce qui a échoué (`CreateAsync` sur la caméra, `HttpListener.Start` sur un port occupé) et le `finally` ferme le serveur de la page, arrête et libère le pipeline, puis appelle `VisioForgeX.DestroySDK()`, que le démarrage ait réussi ou échoué.
- **Le serveur de la page** utilise `HttpListener`, qui sert `http://localhost:<port>/` sans droits d'administrateur. Il répond à `/` avec la page, transfère `/whep/...` avec la même méthode, le même corps et le même `Content-Type`, et renvoie au navigateur le code d'état, le corps de la réponse et l'en-tête `Location`.

Le serveur de la page démarre avant le pipeline, si bien que la page est accessible dès que le flux l'est.

## Côté navigateur : un lecteur WHEP minimal

Placez ce fichier dans `wwwroot/index.html`. La fonction `play` est le lecteur de la démo avec un ajout, la liste `iceServers` passée à `RTCPeerConnection` (voir [NAT, STUN et TURN](#nat-stun-et-turn)) :

```html
<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>Camera</title>
    <style>
        body { margin: 0; background: #111; color: #ddd; font-family: sans-serif; }
        figure { margin: 0; position: relative; background: #000; }
        video { width: 100%; aspect-ratio: 16 / 9; display: block; }
        figcaption { position: absolute; left: 8px; top: 6px; font-size: 13px; text-shadow: 0 0 3px #000; }
    </style>
</head>
<body>
<figure>
    <video id="video" autoplay muted playsinline></video>
    <figcaption id="caption" data-name="Camera">Camera</figcaption>
</figure>
<script>
    // Serveurs STUN/TURN côté spectateur ; une liste vide suffit sur un réseau local.
    const iceServers = [];

    // Un lecteur WHEP minimal : créer l'offre, l'envoyer en POST au endpoint, appliquer la réponse.
    async function play(endpoint, video, caption) {
        const pc = new RTCPeerConnection({ iceServers });
        pc.addTransceiver('video', { direction: 'recvonly' });
        pc.ontrack = e => video.srcObject = e.streams[0] || new MediaStream([e.track]);

        await pc.setLocalDescription(await pc.createOffer());
        // Envoyer l'offre avec tous les candidats ICE à l'intérieur.
        await new Promise(resolve => {
            if (pc.iceGatheringState === 'complete') resolve();
            else pc.onicegatheringstatechange = () => pc.iceGatheringState === 'complete' && resolve();
        });

        const response = await fetch(endpoint, { method: 'POST', headers: { 'Content-Type': 'application/sdp' }, body: pc.localDescription.sdp });
        if (response.status !== 201) throw new Error(`${endpoint}: HTTP ${response.status}`);
        await pc.setRemoteDescription({ type: 'answer', sdp: await response.text() });

        // Terminer la session sur le serveur à la fermeture de la page.
        const session = response.headers.get('Location');
        addEventListener('pagehide', () => fetch(session, { method: 'DELETE', keepalive: true }));
        pc.onconnectionstatechange = () => caption.textContent = `${caption.dataset.name} - ${pc.connectionState}`;
    }

    const caption = document.getElementById('caption');
    play('/whep/endpoint', document.getElementById('video'), caption).catch(e => caption.textContent = e.message);
</script>
</body>
</html>
```

Étape par étape :

1. **Un transceiver vidéo en réception seule.** La page ne fait que regarder : elle propose de recevoir une piste vidéo et n'envoie rien.
2. **Attendre la fin de la collecte ICE** avant d'envoyer l'offre. Tous les candidats réseau voyagent alors dans l'unique `POST`, et le lecteur n'a jamais besoin des requêtes `PATCH` de WHEP pour le trickle ICE.
3. **Envoyer l'offre SDP en `POST`** avec `Content-Type: application/sdp`. Le serveur répond `201 Created` avec la réponse SDP dans le corps.
4. **Appliquer la réponse.** WebRTC se connecte et `ontrack` remet la piste de la caméra à l'élément `<video>`.
5. **Supprimer la session sur `pagehide`.** L'en-tête `Location` désigne la session ; `keepalive: true` laisse partir le `DELETE` pendant le déchargement de la page. Le serveur déclenche alors `ConsumerRemoved`.

L'élément `<video>` est `muted`, ce qui permet au navigateur de lancer la lecture sans clic, et `playsinline` pour les navigateurs mobiles. La légende affiche l'état de la connexion (`connecting`, `connected`, `failed`), la première chose à regarder quand une vignette reste noire.

## Passer à plusieurs caméras

Un `WHEPServerSinkBlock` sert une caméra. Pour plusieurs caméras, la démo exécute un bloc par caméra, chacun sur son propre port, et donne à chaque caméra son propre chemin sur le serveur de la page.

La partie pipeline, tirée du gestionnaire de démarrage de la démo : une paire `RTSPRAWSourceBlock` → `WHEPServerSinkBlock` par caméra, avec des ports WHEP comptés à partir de 9100 :

```csharp
for (var i = 0; i < _cameras.Count; i++)
{
    var camera = _cameras[i];

    // H.264 encodé directement depuis la caméra, sans audio : les navigateurs acceptent l'Opus, pas l'AAC de la caméra.
    var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(camera.Url, camera.Login, camera.Password, audioEnabled: false);
    var rtsp = new RTSPRAWSourceBlock(rtspSettings);

    // Un serveur WHEP par caméra ; seule cette app lui parle, les navigateurs passent par le serveur de la page ci-dessous.
    var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings { ServerAddress = $"http://127.0.0.1:{FirstWhepPort + i}" });
    whep.ConsumerAdded += (s, args) => UpdateViewers(+1);
    whep.ConsumerRemoved += (s, args) => UpdateViewers(-1);

    _pipeline.Connect(rtsp.VideoOutput, whep.Input);
}
```

La partie routage : le serveur de la page transfère `/cam/N/whep/...` vers le serveur de la caméra N et place le même préfixe `/cam/N` devant le `Location` qu'il renvoie, afin que le `DELETE` du navigateur atteigne lui aussi la bonne caméra :

```csharp
var path = request.Url?.AbsolutePath ?? string.Empty;
var parts = path.Split('/', 4, StringSplitOptions.RemoveEmptyEntries);

if (parts.Length == 4 && parts[0] == "cam" && int.TryParse(parts[1], out var camera) && camera >= 0 && camera < _cameras.Count)
{
    // /cam/N/whep/... est transféré vers le serveur WHEP de la caméra N.
    using var forward = new HttpRequestMessage(new HttpMethod(request.HttpMethod), $"http://127.0.0.1:{FirstWhepPort + camera}/{parts[2]}/{parts[3]}");
    if (request.HasEntityBody)
    {
        using var body = new MemoryStream();
        await request.InputStream.CopyToAsync(body);
        forward.Content = new ByteArrayContent(body.ToArray());
        forward.Content.Headers.TryAddWithoutValidation("Content-Type", request.ContentType);
    }

    using var reply = await _http.SendAsync(forward);
    if (reply.Headers.Location != null)
    {
        // L'URL de la session que le lecteur supprime en partant, sous le même préfixe /cam/N.
        response.Headers["Location"] = $"/cam/{camera}{reply.Headers.Location.OriginalString}";
    }

    response.StatusCode = (int)reply.StatusCode;
    await WriteAsync(response, reply.Content.Headers.ContentType?.ToString(), await reply.Content.ReadAsByteArrayAsync());
}
```

Le serveur de la page de la démo répond aussi à `/cameras` avec le nombre de caméras, et la page de la grille crée une vignette par caméra avec sa fonction `play` : celle ci-dessus, sans l'ajout de `iceServers` :

```javascript
(async () => {
    const count = await (await fetch('/cameras')).json();
    for (let i = 0; i < count; i++) {
        const figure = document.createElement('figure');
        const video = Object.assign(document.createElement('video'), { autoplay: true, muted: true, playsInline: true });
        const caption = document.createElement('figcaption');
        caption.dataset.name = caption.textContent = `Camera ${i}`;
        figure.append(video, caption);
        document.getElementById('grid').append(figure);
        play(`/cam/${i}/whep/endpoint`, video, caption).catch(e => caption.textContent = e.message);
    }
})();
```

Avec la règle CSS `#grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(420px, 1fr)); }`, les vignettes se répartissent selon la largeur de la fenêtre.

Deux remarques de conception pour un vrai NVR :

- **Un pipeline ou un par caméra.** La démo place toutes les paires de caméras dans un seul `MediaBlocksPipeline` : elles démarrent et s'arrêtent ensemble, et une caméra illisible fait lever une exception à `CreateAsync` avant le démarrage. Pour redémarrer une caméra sans toucher aux autres, donnez à chaque paire son propre pipeline : c'est le schéma que le guide [Grille RTSP multi-caméras](multi-camera-rtsp-grid.md) utilise pour le rendu local.
- **Le budget, c'est la bande passante, pas le CPU.** Chaque spectateur de chaque vignette reçoit le débit complet de la caméra. Pour une grille, pointez la source vers le sous-flux H.264 de la caméra et gardez le flux principal pour les vues d'une seule caméra et pour [l'enregistrement sans réencodage](rtsp-save-original-stream.md).

## Limites à connaître avant la mise en production

### Codec : H.264 uniquement

L'entrée du bloc n'accepte que du H.264 encodé. La vidéo brute, ou tout autre codec, ne se connecte pas et le pipeline ne démarre pas : le bloc n'encode jamais pour les spectateurs. Un flux H.265 de la caméra est refusé pour la même raison ; de toute façon, la plupart des navigateurs ne savent pas lire le H.265 en WebRTC. Passez la caméra, ou son sous-flux, en H.264.

### Profils H.264 et navigateurs

Le flux de la caméra arrive intact au navigateur : le décodeur WebRTC du navigateur doit donc accepter le profil de la caméra. Les navigateurs basés sur Chromium (Chrome, Edge) ont lu le H.264 profil Main en 1080p d'une caméra lors de nos tests. Un navigateur dont le décodeur WebRTC n'accepte que le constrained baseline n'affiche aucune image pour une caméra en profil Main ou High. Quand un spectateur voit une vignette vide et pas un autre, ouvrez la page dans Chrome ou Edge, ou passez le flux de la caméra en profil Baseline si la caméra le propose.

Le `WHEPSourceBlock` du SDK lui-même ne propose le H.264 qu'en constrained baseline avec le `profile-level-id` exact `42e016`, auquel les flux des caméras ne correspondent pas : il n'obtient donc aucune image de ce bloc. Utilisez un navigateur, ou un lecteur qui envoie `level-asymmetry-allowed=1` comme le font les navigateurs.

### Pas d'audio

Le bloc a une seule entrée, pour la vidéo. Créez la source RTSP avec `audioEnabled: false`.

### Adresse d'écoute

`ServerAddress` n'accepte que des adresses `http://` et vaut par défaut `http://127.0.0.1:9090`, qui n'accepte que les lecteurs de cette machine ; `http://0.0.0.0:<port>` accepte les lecteurs d'autres machines. Pour un serveur qui écoute sur `0.0.0.0`, `EndpointUrl` indique l'adresse de loopback ; les lecteurs des autres machines utilisent plutôt l'adresse réseau de cette machine.

Avec le schéma de serveur de page de ce guide, gardez le serveur WHEP sur `127.0.0.1` et n'exposez que le serveur de la page. Le serveur de la page de la démo écoute sur `localhost` ; pour afficher les caméras sur d'autres ordinateurs, placez devant lui un proxy inverse (nginx, IIS, Caddy), transférez les mêmes chemins et fixez l'en-tête `Host` à `localhost:<port>`, car le serveur de la page ne répond à aucun autre nom d'hôte.

### HTTPS

Recevoir du WebRTC ne nécessite pas HTTPS : un simple `http://` fonctionne pour les spectateurs du réseau local. Le serveur WHEP lui-même ne parle que `http://` ; si votre déploiement exige HTTPS, terminez-le dans le serveur ou le proxy inverse qui sert la page.

### NAT, STUN et TURN

Chaque côté d'une connexion WebRTC collecte ses propres candidats réseau : STUN et TURN se règlent donc séparément de chaque côté.

- **Côté serveur :** `StunServer` (`stun://hostname:port`) et `TurnServer` (`turn(s)://username:password@host:port`) dans `WHEPServerSinkSettings`. Tous deux sont vides par défaut, ce qui signifie aucun serveur STUN ; cela suffit sur un réseau local.
- **Côté spectateur :** la liste `iceServers` que la page passe à `RTCPeerConnection`. Elle est vide dans la page ci-dessus.

Le signaleur annonce aussi le serveur STUN du serveur dans un en-tête `Link` de sa réponse, mais le serveur de la page de ce guide ne transfère pas cet en-tête et le lecteur ne le lit pas : la page définit donc sa propre liste `iceServers`.

```csharp
var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings
{
    ServerAddress = "http://127.0.0.1:9100",
    StunServer = "stun://stun.l.google.com:19302",
    TurnServer = "turn://user:password@turn.example.com:3478"
});
```

```javascript
const iceServers = [
    { urls: 'stun:stun.l.google.com:19302' },
    { urls: 'turn:turn.example.com:3478', username: 'user', credential: 'password' }
];
```

### Authentification

`WHEPServerSinkSettings` n'a aucun réglage d'authentification : quiconque peut atteindre l'adresse du serveur WHEP peut regarder. Gardez-le sur `127.0.0.1` et placez votre contrôle d'accès (connexion, cookie de session, vérification de jeton) dans le serveur de la page qui transfère les requêtes, avant le transfert.

### Erreurs signalées par le bloc

La construction du bloc échoue, avec un message via l'événement `OnError` du pipeline, lorsque `ServerAddress` n'est pas une adresse `http://` avec un port entre 1 et 65535 ou lorsque son port est déjà utilisé. Donnez à chaque caméra son propre port libre.

### Plateformes

Windows x64, avec le paquet `VisioForge.CrossPlatform.Core.Windows.Adds.x64`. Vérifiez `WHEPServerSinkBlock.IsAvailable()` au démarrage, comme le fait la démo.

## Dépannage

| Symptôme | Cause et solution |
| --- | --- |
| `WHEPServerSinkBlock.IsAvailable()` renvoie `false` | Le paquet `VisioForge.CrossPlatform.Core.Windows.Adds.x64` n'est pas référencé. |
| `CreateAsync` lève `InvalidOperationException` (« Unable to read RTSP stream info ») | La caméra est injoignable, ou l'URL ou les identifiants sont faux. Ouvrez la même URL dans un autre client RTSP pour la vérifier. |
| `StartAsync` renvoie `false` | Lisez les messages `OnError` : un port déjà utilisé, un `ServerAddress` invalide ou un flux de caméra qui n'est pas en H.264 (H.265, MJPEG). |
| La page affiche une erreur de `fetch` comme `Failed to fetch` | La page et le endpoint sont sur des origines différentes. Servez la page depuis le serveur qui transfère `/whep/`, jamais depuis le disque ni depuis un autre port. |
| La page affiche `HTTP 404` ou `HTTP 502` | Le serveur de la page n'a pas transféré le chemin, ou le serveur WHEP ne tourne pas sur le port de transfert. |
| La légende reste sur `connecting`, puis passe à `failed` | Le navigateur ne peut pas joindre la machine de l'application en WebRTC. Vérifiez le pare-feu de cette machine ; pour des spectateurs hors du réseau local, réglez STUN/TURN des deux côtés : `StunServer` et `TurnServer` pour le serveur, `iceServers` dans la page. |
| La légende indique `connected`, la vignette reste noire | Le décodeur du navigateur n'accepte pas le profil H.264 de la caméra. Essayez Chrome ou Edge, ou le profil Baseline de la caméra. |
| Pas de `ConsumerRemoved` pour un spectateur parti | La page envoie son `DELETE` sur `pagehide` ; un navigateur qui plante ou perd le réseau ne l'envoie pas. Les spectateurs encore connectés à l'arrêt du pipeline ne déclenchent aucun événement. |
| `HttpListener.Start` lève `HttpListenerException` | Le port de la page est occupé, ou le préfixe n'est pas `localhost` et exige des droits d'administrateur. Choisissez un autre port ou gardez `localhost`. |

## Serveur WHEP, WebRTC, WHIP ou HLS ?

- **`WHEPServerSinkBlock`** (ce guide) : la source est déjà en H.264 (une caméra IP) et les spectateurs sont des navigateurs de votre réseau. Passthrough, pas d'encodeur, pas de serveur média, vidéo seule.
- **[`WebRTCSinkBlock`](../Sinks/index.md#webrtc)** : la vidéo est brute ou traitée dans le pipeline (une webcam, l'écran, des incrustations, de l'analyse), ou vous avez besoin de l'audio. Il encode pour chaque spectateur (vidéo VP8, audio Opus) avec un contrôle de congestion par spectateur, au prix d'un encodeur par spectateur, et utilise le protocole de signalisation WebSocket de `webrtcsink` au lieu de WHEP.
- **[`WHIPSinkBlock`](../Sinks/index.md#whip)** : vous faites déjà tourner un serveur média WebRTC (MediaMTX, Janus, Cloudflare Stream) et voulez que l'application lui pousse un flux ; le serveur gère alors les spectateurs. Exige de la vidéo H.264 et de l'audio Opus ; fonctionne sous Windows, macOS et Linux.
- **[HLS](../Sinks/index.md#hls)** : des spectateurs sur Internet via un CDN ou un simple lecteur `<video>`, quand un retard de plusieurs secondes est acceptable. Voir aussi [le streaming HLS](../../general/network-streaming/hls-streaming.md) et le guide du [lecteur HLS pour ASP.NET Web Forms](aspnet-webforms-hls-player.md).

## Foire aux questions

### Un navigateur peut-il lire un flux RTSP directement ?

Non. Les navigateurs n'implémentent pas RTSP. Le flux doit arriver à la page par un protocole que le navigateur prend en charge : WebRTC, comme dans ce guide, ou HLS. Avec `WHEPServerSinkBlock`, le H.264 de la caméra est transporté en WebRTC sans conversion.

### L'application réencode-t-elle la vidéo ?

Non. `RTSPRAWSourceBlock` fournit le H.264 encodé de la caméra et `WHEPServerSinkBlock` l'envoie à chaque spectateur tel qu'il arrive. C'est pourquoi une caméra ne coûte presque aucun CPU quel que soit le nombre de spectateurs, et aussi pourquoi la caméra doit envoyer du H.264 dans un profil que le navigateur sait décoder.

### Ai-je besoin d'un serveur média comme MediaMTX ou Janus ?

Non. Le serveur WHEP tourne dans votre processus .NET. Un serveur média n'est utile que si vous voulez qu'il gère les spectateurs, par exemple sur Internet ; poussez-lui alors le flux avec `WHIPSinkBlock`.

### Pourquoi n'y a-t-il pas d'audio ?

Le bloc n'a qu'une entrée vidéo. Les navigateurs reçoivent de l'Opus en WebRTC, et les caméras IP envoient de l'AAC ou du G.711 : l'audio de la caméra ne peut donc pas être transmis tel quel. Créez la source avec `audioEnabled: false`. Si vous avez besoin de l'audio dans le navigateur, utilisez `WebRTCSinkBlock`, qui l'encode en Opus.

### Puis-je lire le flux avec autre chose qu'un navigateur ?

Oui, n'importe quel lecteur WHEP peut envoyer une offre au endpoint. L'exception est le `WHEPSourceBlock` du SDK, qui ne propose que le constrained baseline `42e016` et n'obtient aucune image d'un flux de caméra.

### Combien de caméras et de spectateurs une application peut-elle servir ?

Il n'y a pas d'encodage par spectateur : la limite est la bande passante réseau, chaque spectateur de chaque caméra recevant le débit complet de cette caméra. Utilisez les sous-flux des caméras pour les vues en grille.

## Voir aussi

- [Blocs sink → Serveur WHEP](../Sinks/index.md#serveur-whep) : référence de `WHEPServerSinkBlock` et `WHEPServerSinkSettings`
- [Grille RTSP multi-caméras](multi-camera-rtsp-grid.md) : un mur de caméras affiché dans une application de bureau plutôt que dans un navigateur
- [Enregistrer un flux RTSP sans réencodage](rtsp-save-original-stream.md) : enregistrez le même flux H.264 en MP4 en parallèle de la vue en direct
- [Source caméra RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) : URL RTSP, transport et réglages de la caméra
- [Intégration des caméras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) : découvrez les caméras et trouvez les URL de leurs sous-flux
- [Reconnexion RTSP et fallback switch](../../general/network-sources/reconnection-and-fallback.md) : maintenez le pipeline d'une caméra malgré les coupures
