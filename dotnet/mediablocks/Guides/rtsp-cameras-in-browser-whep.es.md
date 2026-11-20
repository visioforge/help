---
title: Cámaras IP RTSP en el navegador con WHEP en C# .NET
description: Muestre cámaras IP RTSP en un navegador por WebRTC (WHEP) desde una app C#, con latencia inferior a un segundo, sin recodificar ni servidor de medios.
sidebar_label: Cámaras RTSP en el navegador (WHEP)
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

# Mostrar cámaras IP RTSP en un navegador web con WHEP en C# / .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Ejemplo de demostración"
    El código de esta guía procede del RTSP WHEP Camera Grid Demo (WPF) del [repositorio de ejemplos](https://github.com/visioforge/.Net-SDK-s-samples): añade cámaras por URL RTSP, sirve una página con una cuadrícula de todas las cámaras y reenvía las solicitudes WHEP de cada cámara a su propio servidor.

    La referencia del bloque está en [Bloques sink → Servidor WHEP](../Sinks/index.md#servidor-whep).

## Qué va a construir

Un navegador no puede abrir una URL RTSP. Las formas habituales de llevar una cámara IP a una página web son transcodificarla a HLS (varios segundos de retardo y un decodificador más un codificador por cámara) o ejecutar un servidor de medios aparte junto a su aplicación. Esta guía sigue una tercera vía: su propia aplicación .NET se convierte en un servidor WebRTC y el H.264 de la cámara llega al navegador exactamente como lo envió la cámara.

El pipeline de una cámara son dos bloques:

```mermaid
graph LR;
    RTSPRAWSourceBlock -- H.264 --> WHEPServerSinkBlock;
```

- `RTSPRAWSourceBlock` se conecta a la cámara y entrega su H.264 codificado sin decodificarlo.
- `WHEPServerSinkBlock` ejecuta un servidor WHEP (WebRTC-HTTP Egress Protocol). Un navegador le envía una oferta WebRTC por HTTP, recibe una respuesta y obtiene el vídeo por su propia sesión WebRTC.

Lo que se gana:

- **Latencia inferior a un segundo.** El navegador reproduce el flujo por WebRTC, no mediante un protocolo basado en segmentos.
- **Casi nada de CPU por cámara, con cualquier número de espectadores.** No se decodifica ni se codifica nada: el bloque envía el flujo de la cámara a cada espectador tal como llega. Cada espectador sigue costando un ancho de banda de red igual a la tasa de bits de la cámara.
- **Sin servidor de medios.** El servidor WHEP se ejecuta dentro de su proceso; el lado del navegador son unas 20 líneas de JavaScript sin ninguna biblioteca de reproductor.

WHEP es un pequeño protocolo HTTP sobre WebRTC: un `POST` con la oferta SDP, un `201 Created` con la respuesta SDP y un encabezado `Location` para la sesión, y un `DELETE` sobre ese `Location` cuando el espectador se va. Cualquier reproductor WHEP puede usar el endpoint, no solo la página de esta guía.

## Requisitos previos

- Windows x64. `WHEPServerSinkBlock` usa el plugin `rswebrtc` de GStreamer, que se distribuye en el paquete de runtime `Adds` de Windows.
- .NET 10 (la demo apunta a `net10.0-windows`).
- Una cámara IP con un flujo RTSP H.264.
- Media Blocks SDK .NET versión 2026.11.18 o posterior, la versión que añade `WHEPServerSinkBlock`.

Paquetes NuGet:

- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks/) — el SDK
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — runtime de GStreamer para Windows x64
- [VisioForge.CrossPlatform.Core.Windows.Adds.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Adds.x64/) — los plugins adicionales, entre ellos `rswebrtc`

Sin el paquete `Adds`, `WHEPServerSinkBlock.IsAvailable()` devuelve `false`. Para los detalles de despliegue consulte la [Guía de despliegue](../../deployment-x/index.md).

La página del navegador es un archivo que sirve la aplicación, así que cópiela a la carpeta de salida en el `.csproj`, como hace la demo:

```xml
<ItemGroup>
  <Content Include="wwwroot\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

## Lo primero que hay que saber: el mismo origen

El servidor WHEP no envía encabezados CORS. Por eso un navegador acepta sus respuestas solo en una página servida desde la misma dirección y el mismo puerto que el endpoint. Abrir `index.html` desde el disco, o servirlo desde otro puerto, falla en el `fetch` de la oferta.

La demo lo resuelve de la forma sencilla: la aplicación ejecuta un pequeño servidor HTTP para la página y reenvía cada solicitud bajo `/whep/` al servidor WHEP, que a su vez escucha solo en loopback. El navegador habla con un único origen; el servidor WHEP nunca queda expuesto directamente.

```mermaid
sequenceDiagram
    participant Browser as Navegador
    participant Page as Servidor de la página (su app)
    participant WHEPServerSinkBlock
    Browser->>Page: GET /
    Page-->>Browser: index.html
    Browser->>Page: POST /whep/endpoint (oferta SDP)
    Page->>WHEPServerSinkBlock: POST /whep/endpoint
    WHEPServerSinkBlock-->>Page: 201 Created, respuesta SDP, Location
    Page-->>Browser: 201 Created, respuesta SDP, Location
    WHEPServerSinkBlock-)Browser: H.264 de la cámara por WebRTC
    Browser->>Page: DELETE Location (página cerrada)
    Page->>WHEPServerSinkBlock: DELETE Location
```

Solo el intercambio HTTP pasa por el servidor de la página. El vídeo viaja por la conexión WebRTC que establecen la oferta y la respuesta.

## Una cámara: el servidor completo

El programa de consola siguiente es el código de la demo para una sola cámara. Construye el pipeline, sirve `wwwroot/index.html` en `http://localhost:8080/` y reenvía `/whep/...` al servidor WHEP en `127.0.0.1:9100`.

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

    // H.264 codificado directamente de la cámara, sin audio: los navegadores aceptan Opus, no el AAC de la cámara.
    var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(new Uri("rtsp://192.168.1.64:554/stream1"), "admin", "password", audioEnabled: false);
    var rtsp = new RTSPRAWSourceBlock(rtspSettings);

    // El servidor WHEP escucha solo en loopback; los navegadores llegan a él a través del servidor de la página de abajo.
    var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings { ServerAddress = WhepServer });
    whep.ConsumerAdded += (s, e) => Console.WriteLine($"Viewer connected: {e.PeerId}");
    whep.ConsumerRemoved += (s, e) => Console.WriteLine($"Viewer left: {e.PeerId}");

    pipeline.Connect(rtsp.VideoOutput, whep.Input);

    // La página y las solicitudes WHEP comparten un origen: whepserversink no envía encabezados CORS.
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
    // CreateAsync lanza una excepción si no puede leer la cámara; HttpListener.Start, si el puerto de la página está ocupado.
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
            // La oferta y el DELETE de la sesión van al servidor WHEP sin cambios.
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
                // La URL de la sesión que el reproductor elimina al irse.
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

Qué hace cada parte:

- **`RTSPRAWSourceSettings.CreateAsync`** se conecta a la cámara y lee la descripción de su flujo antes de construir el pipeline. `audioEnabled: false` deja fuera el audio de la cámara: el servidor WHEP tiene una sola entrada de vídeo y los navegadores aceptan Opus por WebRTC, no el AAC o G.711 que envía una cámara. `CreateAsync` lanza `InvalidOperationException` cuando no puede leer el flujo: la cámara está desconectada, la URL es incorrecta o se rechazan las credenciales.
- **`RTSPRAWSourceBlock.VideoOutput`** lleva el H.264 codificado de la cámara. Conectarlo directamente a **`WHEPServerSinkBlock.Input`** es toda la ruta de medios: ni decodificador ni codificador.
- **`WHEPServerSinkSettings.ServerAddress`** es la dirección en la que escucha el servidor WHEP. `127.0.0.1` lo deja accesible solo desde esta máquina, que es lo que conviene cuando el servidor de la página le reenvía las solicitudes. La propiedad `EndpointUrl` del bloque devuelve la URL a la que envían los reproductores; aquí, `http://127.0.0.1:9100/whep/endpoint`.
- **`ConsumerAdded` / `ConsumerRemoved`** informan de cada espectador que se conecta y se va, con su ID de sesión en `PeerId`. Se generan en un hilo de GStreamer: en una aplicación con interfaz, páselos al hilo de la interfaz sin bloquearlo; la demo usa `Dispatcher.BeginInvoke`. `ConsumerRemoved` no se genera para los espectadores aún conectados cuando se detiene el pipeline; sus sesiones terminan con él.
- **`try` / `finally`** informa de lo que falló (`CreateAsync` con la cámara, `HttpListener.Start` con un puerto ocupado) y el `finally` cierra el servidor de la página, detiene y libera el pipeline y llama a `VisioForgeX.DestroySDK()`, tanto si el inicio tuvo éxito como si falló.
- **El servidor de la página** usa `HttpListener`, que sirve `http://localhost:<puerto>/` sin derechos de administrador. Responde a `/` con la página, reenvía `/whep/...` con el mismo método, cuerpo y `Content-Type`, y devuelve al navegador el código de estado, el cuerpo de la respuesta y el encabezado `Location`.

El servidor de la página se inicia antes que el pipeline, de modo que la página está disponible en cuanto lo está el flujo.

## El lado del navegador: un reproductor WHEP mínimo

Coloque este archivo en `wwwroot/index.html`. La función `play` es el reproductor de la demo con un añadido, la lista `iceServers` que se pasa a `RTCPeerConnection` (consulte [NAT, STUN y TURN](#nat-stun-y-turn)):

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
    // Servidores STUN/TURN del lado del espectador; vacío basta en una red local.
    const iceServers = [];

    // Un reproductor WHEP mínimo: crear la oferta, enviarla con POST al endpoint, aplicar la respuesta.
    async function play(endpoint, video, caption) {
        const pc = new RTCPeerConnection({ iceServers });
        pc.addTransceiver('video', { direction: 'recvonly' });
        pc.ontrack = e => video.srcObject = e.streams[0] || new MediaStream([e.track]);

        await pc.setLocalDescription(await pc.createOffer());
        // Enviar la oferta con todos los candidatos ICE dentro.
        await new Promise(resolve => {
            if (pc.iceGatheringState === 'complete') resolve();
            else pc.onicegatheringstatechange = () => pc.iceGatheringState === 'complete' && resolve();
        });

        const response = await fetch(endpoint, { method: 'POST', headers: { 'Content-Type': 'application/sdp' }, body: pc.localDescription.sdp });
        if (response.status !== 201) throw new Error(`${endpoint}: HTTP ${response.status}`);
        await pc.setRemoteDescription({ type: 'answer', sdp: await response.text() });

        // Terminar la sesión en el servidor cuando se cierra la página.
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

Paso a paso:

1. **Un transceptor de vídeo solo de recepción.** La página solo mira, así que ofrece recibir una pista de vídeo y no envía nada.
2. **Esperar a que termine la recopilación ICE** antes de enviar la oferta. Así todos los candidatos de red viajan dentro del único `POST` y el reproductor nunca necesita las solicitudes `PATCH` de WHEP para trickle ICE.
3. **Enviar con `POST` la oferta SDP** con `Content-Type: application/sdp`. El servidor responde `201 Created` con la respuesta SDP en el cuerpo.
4. **Aplicar la respuesta.** WebRTC se conecta y `ontrack` entrega la pista de la cámara al elemento `<video>`.
5. **Eliminar la sesión en `pagehide`.** El encabezado `Location` identifica la sesión; `keepalive: true` permite que el `DELETE` salga mientras la página se descarga. El servidor genera entonces `ConsumerRemoved`.

El elemento `<video>` está en `muted`, lo que permite al navegador iniciar la reproducción sin un clic, y en `playsinline` para los navegadores móviles. El pie muestra el estado de la conexión (`connecting`, `connected`, `failed`), que es lo primero que hay que mirar cuando un recuadro se queda en negro.

## Escalar a varias cámaras

Un `WHEPServerSinkBlock` sirve una cámara. Para varias cámaras, la demo ejecuta un bloque por cámara, cada uno en su propio puerto, y da a cada cámara su propia ruta en el servidor de la página.

La parte del pipeline, tomada del manejador de inicio de la demo: un par `RTSPRAWSourceBlock` → `WHEPServerSinkBlock` por cámara, con puertos WHEP a partir de 9100:

```csharp
for (var i = 0; i < _cameras.Count; i++)
{
    var camera = _cameras[i];

    // H.264 codificado directamente de la cámara, sin audio: los navegadores aceptan Opus, no el AAC de la cámara.
    var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(camera.Url, camera.Login, camera.Password, audioEnabled: false);
    var rtsp = new RTSPRAWSourceBlock(rtspSettings);

    // Un servidor WHEP por cámara; solo esta app habla con él, los navegadores pasan por el servidor de la página de abajo.
    var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings { ServerAddress = $"http://127.0.0.1:{FirstWhepPort + i}" });
    whep.ConsumerAdded += (s, args) => UpdateViewers(+1);
    whep.ConsumerRemoved += (s, args) => UpdateViewers(-1);

    _pipeline.Connect(rtsp.VideoOutput, whep.Input);
}
```

La parte del enrutamiento: el servidor de la página reenvía `/cam/N/whep/...` al servidor de la cámara N y antepone el mismo prefijo `/cam/N` al `Location` que devuelve, para que el `DELETE` del navegador también llegue a la cámara correcta:

```csharp
var path = request.Url?.AbsolutePath ?? string.Empty;
var parts = path.Split('/', 4, StringSplitOptions.RemoveEmptyEntries);

if (parts.Length == 4 && parts[0] == "cam" && int.TryParse(parts[1], out var camera) && camera >= 0 && camera < _cameras.Count)
{
    // /cam/N/whep/... se reenvía al servidor WHEP de la cámara N.
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
        // La URL de la sesión que el reproductor elimina al irse, bajo el mismo prefijo /cam/N.
        response.Headers["Location"] = $"/cam/{camera}{reply.Headers.Location.OriginalString}";
    }

    response.StatusCode = (int)reply.StatusCode;
    await WriteAsync(response, reply.Content.Headers.ContentType?.ToString(), await reply.Content.ReadAsByteArrayAsync());
}
```

El servidor de la página de la demo también responde a `/cameras` con el número de cámaras, y la página de la cuadrícula crea un recuadro por cámara con su función `play`: la de arriba, sin el añadido de `iceServers`:

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

Con la regla CSS `#grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(420px, 1fr)); }` los recuadros se reparten según el ancho de la ventana.

Dos notas de diseño para un NVR real:

- **Un pipeline o uno por cámara.** La demo pone todos los pares de cámaras en un solo `MediaBlocksPipeline`, de modo que se inician y se detienen juntos, y una cámara que no se puede leer hace que `CreateAsync` lance una excepción antes del inicio. Para reiniciar una cámara sin tocar las demás, dé a cada par su propio pipeline: es el mismo patrón que usa la guía [Cuadrícula RTSP multicámara](multi-camera-rtsp-grid.md) para la representación local.
- **El presupuesto es el ancho de banda, no la CPU.** Cada espectador de cada recuadro recibe la tasa de bits completa de la cámara. Para una cuadrícula, apunte la fuente al subflujo H.264 de la cámara y reserve el flujo principal para las vistas de una sola cámara y para [grabar sin recodificar](rtsp-save-original-stream.md).

## Límites que conviene conocer antes de publicar

### Códec: solo H.264

La entrada del bloque acepta solo H.264 codificado. El vídeo sin comprimir, o cualquier otro códec, no se conecta y el pipeline no se inicia: el bloque nunca codifica para los espectadores. Un flujo H.265 de la cámara se rechaza por el mismo motivo; de todos modos, la mayoría de los navegadores no pueden reproducir H.265 por WebRTC. Cambie la cámara, o su subflujo, a H.264.

### Perfiles H.264 y navegadores

El flujo de la cámara llega al navegador intacto, así que el decodificador WebRTC del navegador debe aceptar el perfil de la cámara. Los navegadores basados en Chromium (Chrome, Edge) reprodujeron el H.264 perfil Main a 1080p de una cámara en nuestras pruebas. Un navegador cuyo decodificador WebRTC acepte solo constrained baseline no muestra imagen para una cámara con perfil Main o High. Cuando un espectador ve un recuadro vacío y otro no, abra la página en Chrome o Edge, o cambie el flujo de la cámara al perfil Baseline si la cámara lo ofrece.

El propio `WHEPSourceBlock` del SDK ofrece H.264 solo como constrained baseline con el `profile-level-id` exacto `42e016`, que los flujos de las cámaras no cumplen, por lo que no obtiene imagen de este bloque. Use un navegador o un reproductor que envíe `level-asymmetry-allowed=1`, como hacen los navegadores.

### Sin audio

El bloque tiene una entrada, para vídeo. Cree la fuente RTSP con `audioEnabled: false`.

### Dirección de escucha

`ServerAddress` acepta solo direcciones `http://` y su valor predeterminado es `http://127.0.0.1:9090`, que acepta reproductores solo en esta máquina; `http://0.0.0.0:<puerto>` acepta reproductores de otras máquinas. Para un servidor que escucha en `0.0.0.0`, `EndpointUrl` indica la dirección de loopback; los reproductores de otras máquinas usan en su lugar la dirección de red de esta máquina.

Con el patrón de servidor de página de esta guía, mantenga el servidor WHEP en `127.0.0.1` y exponga solo el servidor de la página. El servidor de la página de la demo escucha en `localhost`; para mostrar las cámaras en otros equipos, coloque delante un proxy inverso (nginx, IIS, Caddy), reenvíe las mismas rutas y establezca el encabezado `Host` en `localhost:<puerto>`, porque el servidor de la página no responde a ningún otro nombre de host.

### HTTPS

Recibir WebRTC no requiere HTTPS, así que `http://` simple funciona para los espectadores de la red local. El propio servidor WHEP habla solo `http://`; si su despliegue necesita HTTPS, termínelo en el servidor o proxy inverso que sirve la página.

### NAT, STUN y TURN

Cada lado de una conexión WebRTC recopila sus propios candidatos de red, así que STUN y TURN se configuran en cada lado por separado:

- **El lado del servidor:** `StunServer` (`stun://hostname:port`) y `TurnServer` (`turn(s)://username:password@host:port`) en `WHEPServerSinkSettings`. Ambos están sin definir de forma predeterminada, lo que significa ningún servidor STUN; basta en una red local.
- **El lado del espectador:** la lista `iceServers` que la página pasa a `RTCPeerConnection`. En la página anterior está vacía.

El señalizador también anuncia el servidor STUN del servidor en un encabezado `Link` de su respuesta, pero el servidor de la página de esta guía no reenvía ese encabezado y el reproductor no lo lee, así que la página define su propia lista `iceServers`.

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

### Autenticación

`WHEPServerSinkSettings` no tiene ningún ajuste de autenticación: cualquiera que pueda llegar a la dirección del servidor WHEP puede ver el flujo. Manténgalo en `127.0.0.1` y coloque su control de acceso (inicio de sesión, cookie de sesión, comprobación de token) en el servidor de la página que reenvía las solicitudes, antes del reenvío.

### Errores que informa el bloque

La construcción del bloque falla, con un mensaje a través del evento `OnError` del pipeline, cuando `ServerAddress` no es una dirección `http://` con un puerto entre 1 y 65535 o cuando su puerto ya está en uso. Asigne a cada cámara su propio puerto libre.

### Plataformas

Windows x64, con el paquete `VisioForge.CrossPlatform.Core.Windows.Adds.x64`. Compruebe `WHEPServerSinkBlock.IsAvailable()` al iniciar, como hace la demo.

## Solución de problemas

| Síntoma | Causa y solución |
| --- | --- |
| `WHEPServerSinkBlock.IsAvailable()` devuelve `false` | El paquete `VisioForge.CrossPlatform.Core.Windows.Adds.x64` no está referenciado. |
| `CreateAsync` lanza `InvalidOperationException` ("Unable to read RTSP stream info") | La cámara no es accesible, o la URL o las credenciales son incorrectas. Abra la misma URL en otro cliente RTSP para comprobarla. |
| `StartAsync` devuelve `false` | Lea los mensajes de `OnError`: un puerto ya en uso, un `ServerAddress` no válido o un flujo de cámara que no es H.264 (H.265, MJPEG). |
| La página muestra un error de `fetch` como `Failed to fetch` | La página y el endpoint están en orígenes distintos. Sirva la página desde el servidor que reenvía `/whep/`, nunca desde el disco ni desde otro puerto. |
| La página muestra `HTTP 404` o `HTTP 502` | El servidor de la página no reenvió la ruta, o el servidor WHEP no se está ejecutando en el puerto de reenvío. |
| El pie se queda en `connecting` y luego pasa a `failed` | El navegador no puede llegar a la máquina de la aplicación por WebRTC. Revise el firewall de esa máquina; para espectadores fuera de la red local configure STUN/TURN en ambos lados: `StunServer` y `TurnServer` para el servidor, `iceServers` en la página. |
| El pie indica `connected` y el recuadro sigue en negro | El decodificador del navegador no acepta el perfil H.264 de la cámara. Pruebe Chrome o Edge, o el perfil Baseline de la cámara. |
| No llega `ConsumerRemoved` para un espectador que ya no está | La página envía su `DELETE` en `pagehide`; un navegador que se bloquea o pierde la red no lo envía. Los espectadores aún conectados cuando se detiene el pipeline no generan ningún evento. |
| `HttpListener.Start` lanza `HttpListenerException` | El puerto de la página está ocupado, o el prefijo no es `localhost` y requiere derechos de administrador. Elija otro puerto o mantenga `localhost`. |

## ¿Servidor WHEP, WebRTC, WHIP o HLS?

- **`WHEPServerSinkBlock`** (esta guía): la fuente ya es H.264 (una cámara IP) y los espectadores son navegadores de su red. Passthrough, sin codificador, sin servidor de medios, solo vídeo.
- **[`WebRTCSinkBlock`](../Sinks/index.md#webrtc)**: el vídeo está sin comprimir o se procesa en el pipeline (una webcam, la pantalla, superposiciones, analítica), o necesita audio. Codifica para cada espectador (vídeo VP8, audio Opus) con control de congestión por espectador, a costa de un codificador por espectador, y usa el protocolo de señalización WebSocket de `webrtcsink` en lugar de WHEP.
- **[`WHIPSinkBlock`](../Sinks/index.md#whip)**: ya tiene un servidor de medios WebRTC (MediaMTX, Janus, Cloudflare Stream) y quiere que la aplicación le envíe un flujo; el servidor se encarga entonces de los espectadores. Requiere vídeo H.264 y audio Opus; funciona en Windows, macOS y Linux.
- **[HLS](../Sinks/index.md#hls)**: espectadores en internet a través de una CDN o de un reproductor `<video>` simple, cuando un retardo de varios segundos es aceptable. Consulte también [streaming HLS](../../general/network-streaming/hls-streaming.md) y la guía del [reproductor HLS para ASP.NET Web Forms](aspnet-webforms-hls-player.md).

## Preguntas frecuentes

### ¿Puede un navegador reproducir un flujo RTSP directamente?

No. Los navegadores no implementan RTSP. El flujo tiene que llegar a la página por un protocolo que el navegador admita: WebRTC, como en esta guía, o HLS. Con `WHEPServerSinkBlock` el H.264 de la cámara viaja por WebRTC sin convertirse.

### ¿La aplicación recodifica el vídeo?

No. `RTSPRAWSourceBlock` entrega el H.264 codificado de la cámara y `WHEPServerSinkBlock` lo envía a cada espectador tal como llega. Por eso una cámara cuesta casi nada de CPU, la vean cuantos espectadores la vean, y también por eso la cámara debe enviar H.264 en un perfil que el navegador pueda decodificar.

### ¿Necesito un servidor de medios como MediaMTX o Janus?

No. El servidor WHEP se ejecuta dentro de su proceso .NET. Solo necesita un servidor de medios si quiere que él gestione a los espectadores, por ejemplo en internet; en ese caso, envíele el flujo con `WHIPSinkBlock`.

### ¿Por qué no hay audio?

El bloque tiene una única entrada de vídeo. Los navegadores reciben Opus por WebRTC y las cámaras IP envían AAC o G.711, así que el audio de la cámara no puede pasarse tal cual. Cree la fuente con `audioEnabled: false`. Cuando necesite audio en el navegador, use `WebRTCSinkBlock`, que lo codifica a Opus.

### ¿Puedo reproducir el flujo con algo que no sea un navegador?

Sí, cualquier reproductor WHEP puede enviar una oferta al endpoint. La excepción es el propio `WHEPSourceBlock` del SDK, que ofrece solo constrained baseline `42e016` y no obtiene imagen de un flujo de cámara.

### ¿Cuántas cámaras y espectadores puede servir una aplicación?

No hay codificación por espectador, así que el límite es el ancho de banda de red: cada espectador de cada cámara recibe la tasa de bits completa de esa cámara. Use los subflujos de las cámaras para las vistas en cuadrícula.

## Véase también

- [Bloques sink → Servidor WHEP](../Sinks/index.md#servidor-whep): referencia de `WHEPServerSinkBlock` y `WHEPServerSinkSettings`
- [Cuadrícula RTSP multicámara](multi-camera-rtsp-grid.md): una pared de cámaras representada en una aplicación de escritorio en lugar de un navegador
- [Guardar un flujo RTSP sin recodificar](rtsp-save-original-stream.md): grabe el mismo flujo H.264 en MP4 junto a la vista en directo
- [Fuente de cámara RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md): URL RTSP, transporte y ajustes de la cámara
- [Integración de cámaras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md): descubra cámaras y encuentre las URL de sus subflujos
- [Reconexión RTSP y fallback switch](../../general/network-sources/reconnection-and-fallback.md): mantenga en marcha el pipeline de una cámara a pesar de las caídas
