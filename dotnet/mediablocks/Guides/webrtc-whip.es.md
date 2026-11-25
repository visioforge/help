---
title: Pantalla o cámara a un navegador por WebRTC en C# .NET
description: Transmita la pantalla, una ventana o una cámara desde una app C# a navegadores por WebRTC sin servidor de medios, y publíquela en MediaMTX o Janus con WHIP.
sidebar_label: Pantalla y cámara al navegador (WebRTC, WHIP)
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

---

# Transmitir la pantalla o una cámara a navegadores web por WebRTC y publicar con WHIP en C# / .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Ejemplos de demostración"
    La parte del navegador de esta guía sigue el [WebRTC Browser Streamer Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20Browser%20Streamer%20Demo): una cámara y un micrófono, una vista previa local y una página de visualización servida por la aplicación. La parte de WHIP sigue el [WebRTC WHIP Streamer Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20WHIP%20Streamer%20Demo) y su [versión Avalonia](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Avalonia/WebRTC%20WHIP%20Streamer) para Windows, macOS y Linux.

    La referencia de los bloques está en [Bloques sink → WebRTC](../Sinks/index.md#webrtc) y [Bloques sink → WHIP](../Sinks/index.md#whip).

## Qué va a construir

Dos formas de mostrar en directo la pantalla, una ventana de aplicación o una cámara a personas que solo tienen un navegador web, ambas con una latencia inferior a un segundo:

1. **Directamente a los navegadores, sin servidor.** `WebRTCSinkBlock` convierte su aplicación .NET en un servidor WebRTC: sirve la página de visualización, ejecuta la señalización y envía a cada espectador su propio flujo WebRTC. No hay que instalar nada junto a la aplicación.
2. **A través de un servidor de medios, con WHIP.** `WHIPSinkBlock` publica un flujo en un servidor de medios WebRTC (MediaMTX, Janus, Cloudflare Stream) mediante WHIP (WebRTC-HTTP Ingestion Protocol). El servidor lo entrega después a cualquier número de espectadores, también en Internet.

![Una ventana de aplicación compartida desde una app C# con WebRTCSinkBlock y reproducida en un navegador; el reloj de la ventana va unos 0,15 segundos por detrás del reloj del propio navegador](webrtc-whip-browser.webp)

```mermaid
graph LR;
    src["Pantalla, ventana o cámara<br/>+ micrófono"] --> app;
    subgraph app ["Su aplicación .NET"]
        WebRTCSinkBlock;
        WHIPSinkBlock;
    end
    WebRTCSinkBlock -- "WebRTC, VP8 + Opus" --> b1["Navegadores de su red"];
    WHIPSinkBlock -- "WHIP: H.264 + Opus" --> server["Servidor de medios<br/>MediaMTX, Janus"];
    server -- "WebRTC (WHEP)" --> b2["Navegadores en cualquier lugar"];
```

Cuál elegir:

| | `WebRTCSinkBlock` | `WHIPSinkBlock` |
| --- | --- | --- |
| Software adicional | ninguno | un servidor de medios WebRTC |
| Codificación | VP8 y Opus, un codificador por espectador, con control de congestión por espectador | H.264 y Opus, codificados una vez en su pipeline |
| CPU por cada espectador adicional | un codificador VP8 más | ninguna: el servidor reenvía el flujo |
| Página de visualización | servida por el bloque, unas 100 líneas de JavaScript | el reproductor propio del servidor o cualquier reproductor WHEP |
| Plataformas | Windows, macOS, Linux, iOS | Windows, macOS, Linux, iOS |
| Uso típico | compartir la pantalla y ver cámaras con pocos espectadores en una red local | publicar para muchos espectadores o a través de Internet |

## Requisitos previos

- Windows, macOS, Linux o iOS.
- .NET 10 (.NET 8 o posterior en Linux).
- Media Blocks SDK .NET versión 2026.11.21 o posterior. Esa versión incluye la corrección de la captura de pantalla hacia `WebRTCSinkBlock` en la que se basa esta guía; con 2026.11.20, `WebRTCSinkBlock` reproduce cámaras, pero el espectador de una captura de pantalla Direct3D 11 nunca recibe imagen. En iOS, los dos bloques necesitan que el SDK y `VisioForge.CrossPlatform.Core.iOS` sean 2026.11.23 o posterior.
- Para WHIP, un servidor de medios WebRTC. Los ejemplos usan [MediaMTX](https://github.com/bluenviron/mediamtx), cuya configuración predeterminada acepta WHIP en el puerto 8889.

Paquetes NuGet:

- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks/): el SDK
- Windows: [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) y [VisioForge.CrossPlatform.Core.Windows.Adds.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Adds.x64/): el runtime de GStreamer y los plugins adicionales, entre ellos `rswebrtc` (ambos bloques)
- macOS: [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS/) y [VisioForge.CrossPlatform.Core.macOS.Adds](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS.Adds/)
- Linux: `VisioForge.CrossPlatform.Core.Linux.x64` y `VisioForge.CrossPlatform.Core.Linux.Adds.x64` (`.arm64` para ARM), además del GStreamer de la distribución con `gstreamer1.0-plugins-bad` y `gstreamer1.0-nice`
- iOS: [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS/), que incluye `rswebrtc`

Sin el paquete `Adds`, `WebRTCSinkBlock.IsAvailable()` y `WHIPSinkBlock.IsAvailable()` devuelven `false`; en iOS, ambos vienen con `VisioForge.CrossPlatform.Core.iOS`. Para los detalles de despliegue, consulte la [Guía de despliegue](../../deployment-x/index.md).

## Parte 1: directamente a los navegadores con WebRTCSinkBlock

### Cómo se conecta un espectador

`WebRTCSinkBlock` ejecuta dos servidores dentro de su proceso: un servidor web para la página de visualización y un servidor de señalización WebSocket. La página pide el flujo al servidor de señalización, el bloque envía una oferta WebRTC y el navegador responde. A partir de ahí el video viaja por la propia conexión WebRTC del espectador.

```mermaid
sequenceDiagram
    participant Browser as Navegador
    participant Web as Servidor web (WebRTCSinkBlock)
    participant Sig as Servidor de señalización (WebRTCSinkBlock)
    Browser->>Web: GET /?signalling=8443
    Web-->>Browser: index.html
    Browser->>Sig: WebSocket: registro como listener, lista de productores
    Browser->>Sig: inicio de sesión con el productor
    Sig-->>Browser: oferta SDP (VP8, Opus)
    Browser->>Sig: respuesta SDP, candidatos ICE
    Sig-->>Browser: candidatos ICE
    Note over Browser,Sig: se genera ConsumerAdded
    Note over Browser,Sig: la pantalla o la cámara se reproduce por la propia conexión WebRTC del espectador
```

Cada espectador obtiene su propio codificador y su propio control de congestión, de modo que un espectador con una conexión Wi-Fi lenta recibe una tasa de bits menor sin reducir la calidad para los demás.

### El programa completo

El siguiente programa de consola envía la pantalla principal (o, con `camera` en la línea de comandos, la primera cámara) y el primer micrófono a cada navegador que abra `http://localhost:8090/?signalling=8443`.

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

// 127.0.0.1 sirve solo a este equipo; "0.0.0.0" sirve también a espectadores de otros equipos.
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

    // La pantalla principal de forma predeterminada, la primera cámara con "camera" en la línea de comandos.
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

    // Su propio servidor de señalización, más un servidor web para la página de visualización en wwwroot.
    var webrtc = new WebRTCSinkBlock(new WebRTCSinkSettings
    {
        SignallingServerHost = Host,
        SignallingServerPort = SignallingPort,
        WebServerDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
        WebServerAddress = $"http://{Host}:{WebPort}/"
    });
    webrtc.ConsumerAdded += (s, e) => Console.WriteLine($"Viewer connected: {e.PeerId}");
    webrtc.ConsumerRemoved += (s, e) => Console.WriteLine($"Viewer left: {e.PeerId}");

    // Video sin comprimir: el sink lo codifica en VP8 para cada espectador.
    pipeline.Connect(video.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Video));

    // El primer micrófono, si lo hay: codificado en Opus para cada espectador.
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

Copie la página de visualización a la carpeta de salida con la compilación; añada esto al `.csproj`:

```xml
<ItemGroup>
  <Content Include="wwwroot\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

Qué hace cada parte:

- **`ScreenSourceBlock` con `ScreenCaptureD3D11SourceSettings`** captura el monitor principal mediante Direct3D 11 a su resolución nativa. `FrameRate` es 10 de forma predeterminada; con 30, el movimiento del ratón y el desplazamiento se ven fluidos. `MonitorIndex` elige otro monitor.
- **`SystemVideoSourceBlock`** abre una cámara. `new VideoCaptureDeviceSourceSettings(device)` elige su formato HD cuando lo tiene; establezca `Format` para elegir otro.
- **`WebRTCSinkBlock.CreateNewInput`** crea una entrada por llamada. El bloque codifica el video y el audio sin comprimir para cada espectador, el video en VP8 y el audio en Opus. El video ya codificado en VP8 (con `VPXEncoderBlock`) se envía tal cual, sin un codificador por espectador.
- **`WebServerDirectory`** activa el servidor web del bloque y lo apunta a la carpeta con `index.html`. Déjelo en `null` para servir la página usted mismo (consulte [Servir la página desde su propio servidor web](#servir-la-pagina-desde-su-propio-servidor-web)).
- **`SignallingServerHost`** y el host de **`WebServerAddress`** deciden quién puede conectarse: `127.0.0.1` acepta solo este equipo, `0.0.0.0` todas las interfaces de red.
- **`ConsumerAdded` / `ConsumerRemoved`** informan de cada espectador que se conecta y se va. Se generan en un hilo de GStreamer: en una aplicación con interfaz, páselos al hilo de la interfaz, como hace la demo con `Dispatcher.Invoke`. `ConsumerRemoved` no se genera para los espectadores que siguen conectados cuando se detiene el pipeline; sus sesiones terminan con él.
- **`try` / `finally`** informa de lo que ha fallado y siempre detiene y libera el pipeline y llama a `VisioForgeX.DestroySDK()`. Cuando `StartAsync` devuelve `false`, el motivo está en los mensajes de `OnError`; un puerto ya en uso es el caso habitual.

### La página de visualización

La página habla el protocolo de señalización del elemento `webrtcsink` de GStreamer: mensajes JSON sobre un WebSocket. Guárdela como `wwwroot/index.html`. Es la página del WebRTC Browser Streamer Demo: reproduce el primer flujo que anuncia el servidor, reintenta cuando la aplicación aún no está en marcha y arranca silenciada para que el navegador la reproduzca sin un clic.

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
        // Puerto de señalización de WebRTCSinkBlock: ?signalling=8443 en la URL de la página.
        const port = new URLSearchParams(location.search).get('signalling') || '8443';
        const video = document.getElementById('video');
        const status = document.getElementById('status');
        let ws, pc, sessionId;

        document.getElementById('unmute').onclick = () => { video.muted = false; };

        function send(msg) { ws.send(JSON.stringify(msg)); }

        function connect() {
            // Una página https:// necesita wss://: establezca SignallingServerCertificate y SignallingServerKey.
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
                // Replique aquí los StunServer y TurnServers del sink cuando se le asignen.
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

        // Un error mientras se reproduce un flujo (un mensaje obsoleto, un candidato rechazado) no lo toca;
        // en otro caso se termina la sesión y se vuelve a pedir la lista de productores.
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

        // Vuelve a pedir la lista de productores tras una pausa, para que una sesión que falla no reintente sin descanso.
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

El intercambio, paso a paso:

1. **Registrarse como listener.** La página abre un WebSocket al puerto de señalización y envía `setPeerStatus` con el rol `listener`, y después pide la lista de productores (`list`). Su aplicación es el productor.
2. **Iniciar una sesión.** La página envía `startSession` con el ID del productor; un productor que aparece más tarde se anuncia con `peerStatusChanged`, así que la página puede abrirse antes de que arranque la aplicación.
3. **Responder a la oferta.** El bloque envía la oferta SDP en un mensaje `peer`; la página crea un `RTCPeerConnection`, responde, y ambos lados intercambian candidatos ICE en otros mensajes `peer`.
4. **Reproducir.** `ontrack` entrega el flujo al elemento `<video>`. El estado muestra `Playing`; **Unmute** activa el sonido, porque los navegadores reproducen un video silenciado sin un clic y uno con sonido solo después de él.
5. **Recuperarse.** Cuando la conexión falla o la aplicación se detiene, la página se reinicia y vuelve a pedir la lista, de modo que se reconecta sola cuando el flujo vuelve.

### Compartir una ventana en lugar de toda la pantalla

Establezca `WindowHandle` para capturar una sola ventana de aplicación mediante Windows Graphics Capture. La ventana se captura aunque otras ventanas la cubran, y el resto del escritorio sigue siendo privado:

```csharp
var screen = new ScreenCaptureD3D11SourceSettings
{
    FrameRate = new VideoFrameRate(30),
    WindowHandle = windowHandle // HWND de la ventana que se comparte, por ejemplo Process.MainWindowHandle
};

var video = new ScreenSourceBlock(screen);
```

El flujo toma el tamaño de la ventana al iniciarse la captura. Una ventana minimizada no da fotogramas hasta que se restaura.

### Servir la página desde su propio servidor web

El servidor web del bloque es opcional. Con `WebServerDirectory` en `null`, solo se ejecuta el servidor de señalización; ponga `index.html` en su propio sitio (ASP.NET Core, IIS, nginx) y pase el puerto de señalización en la cadena de consulta. La página se conecta a `ws://<host de la página>:<puerto>`, o a `wss://` cuando la propia página está en `https://`, por lo que el servidor de señalización debe ser accesible con el mismo nombre de host que la página.

### Una página de visualización en un sitio HTTPS

Un navegador bloquea una conexión `ws://` desde una página `https://`. Entregue al servidor de señalización el certificado y la clave privada de su sitio como archivos PEM y hablará `wss://`:

```csharp
var webrtcSink = new WebRTCSinkBlock(new WebRTCSinkSettings
{
    SignallingServerHost = "0.0.0.0",
    SignallingServerPort = 8443,
    SignallingServerCertificate = @"C:\certs\fullchain.pem",
    SignallingServerKey = @"C:\certs\privkey.pem"
});
```

El certificado debe ser válido para el nombre de host de la dirección de la página, y la clave no debe estar cifrada. `Build` falla, con un mensaje `OnError`, si solo se establece uno de los dos, si no se puede leer un archivo, si la clave no corresponde al certificado (se comprueba en .NET 5 y posteriores) o si también se establece `WebServerDirectory`. El servidor web del propio bloque sirve solo `http://`, así que ponga una página `https://` en su propio servidor web (IIS, nginx, ASP.NET Core) y deje `WebServerDirectory` en `null`.

### Espectadores en otros equipos

1. Establezca `Host` en `"0.0.0.0"` para que ambos servidores escuchen en todas las interfaces de red.
2. Permita el puerto web y el puerto de señalización (aquí 8090 y 8443) en el Firewall de Windows Defender, y la propia aplicación para los medios WebRTC, que usan puertos UDP elegidos en tiempo de ejecución.
3. Abra `http://<dirección de este equipo>:8090/?signalling=8443` en el otro equipo.

Recibir WebRTC no requiere HTTPS, así que `http://` funciona en una red local: abrimos la página de la demo con la dirección de red del equipo, que el navegador considera no segura, y la cámara se reprodujo. `WebRTCSinkSettings` no tiene ajuste de autenticación: cualquiera que alcance los dos puertos puede ver el flujo. Mantenga `127.0.0.1` cuando el flujo no deba salir del equipo, y use WHIP con un servidor de medios que compruebe tokens cuando deba llegar a Internet.

## Parte 2: publicar en un servidor de medios con WHIPSinkBlock

### El pipeline

WHIP admite video H.264 y audio Opus ya codificados. El pipeline codifica una vez; el servidor de medios reenvía el flujo a cada espectador.

```mermaid
graph LR;
    ScreenSourceBlock -- "video sin comprimir" --> H264EncoderBlock;
    SystemAudioSourceBlock -- "audio sin comprimir" --> OPUSEncoderBlock;
    H264EncoderBlock -- H.264 --> WHIPSinkBlock;
    OPUSEncoderBlock -- Opus --> WHIPSinkBlock;
    WHIPSinkBlock -- "WHIP (HTTP + WebRTC)" --> MediaMTX;
```

### El programa completo

El programa de consola publica la pantalla principal y el primer micrófono en `http://<servidor>:8889/screen/whip`, el punto de conexión WHIP de MediaMTX para un flujo llamado `screen`. Pase otro punto de conexión en la línea de comandos. La pantalla se captura con `ScreenCaptureD3D11SourceSettings`, así que este programa se compila para Windows; en macOS use `ScreenCaptureMacOSSourceSettings`, en Linux `ScreenCaptureXDisplaySourceSettings`, o una cámara, como hace la demo de Avalonia.

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

// El punto de conexión WHIP del servidor de medios; MediaMTX usa http://<servidor>:8889/<nombre del flujo>/whip.
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

    // WHIP lleva video H.264 y audio Opus: se codifica una vez aquí y el servidor atiende a cada espectador.
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

Qué hace cada parte:

- **`H264EncoderBlock` con `OpenH264EncoderSettings`** codifica por software, en cualquier equipo Windows, con el perfil Constrained Baseline que decodifican todos los navegadores. En un equipo con GPU NVIDIA, AMD o Intel, los ajustes de un codificador por hardware (`NVENCH264EncoderSettings`, `AMFH264EncoderSettings`, `QSVH264EncoderSettings`) liberan la CPU de la codificación.
- **`OPUSEncoderBlock`** codifica el micrófono en Opus, el códec de audio de WebRTC.
- **`WHIPSinkBlock.CreateNewInput`** crea la entrada de video y la de audio. El bloque añade él mismo el empaquetado RTP, así que los codificadores se conectan directamente a él. El audio es opcional: sin micrófono, el flujo es solo de video.
- **`WHIPSinkSettings.Location`** es el punto de conexión WHIP. En `StartAsync` el bloque envía allí su oferta SDP, el servidor responde y los medios viajan por WebRTC. Al detenerse, el bloque elimina la sesión en el servidor.

### Ver el flujo

MediaMTX sirve su propio reproductor para cada flujo en `http://<servidor>:8889/<nombre del flujo>/`; aquí, `http://<servidor>:8889/screen/`. Cualquier reproductor WHEP puede leerlo también desde `http://<servidor>:8889/screen/whep`, incluido el propio [`WHEPSourceBlock`](../Sources/index.md#bloque-fuente-whep) del SDK.

![Una ventana de aplicación publicada en MediaMTX con WHIPSinkBlock, reproducida en el reproductor web de MediaMTX](webrtc-whip-mediamtx.webp)

### Autenticación, STUN y TURN

```csharp
var whip = new WHIPSinkBlock(new WHIPSinkSettings
{
    Location = "https://media.example.com/live/whip",
    AuthToken = "secret-token",                     // se envía como "Authorization: Bearer secret-token"
    StunServer = "stun://stun.l.google.com:19302",
    TurnServer = "turn://user:password@turn.example.com:3478"
});
```

- **`AuthToken`**: el token de portador de los servicios que lo exigen (Cloudflare Stream, un servidor Janus o MediaMTX con autenticación).
- **`StunServer` / `TurnServer`**: la parte del publicador en el atravesamiento de NAT. Si no se establece, el bloque no usa ningún servidor STUN, lo que basta cuando la aplicación y el servidor están en la misma red. `UseLinkHeaders` (activado de forma predeterminada) toma además los servidores ICE que el servidor WHIP anuncia en sus cabeceras `Link`.
- **`IceTransportPolicy = WHIPIceTransportPolicy.Relay`** envía todo a través del servidor TURN, para redes que bloquean el UDP directo.
- **`Timeout`** (15 segundos de forma predeterminada) limita cada solicitud HTTP de WHIP, incluido el `DELETE` de la sesión al detenerse.

## Ambos desde una sola captura

Una captura puede alimentar ambas salidas a la vez: los espectadores de la red local reciben el flujo directamente de la aplicación, y el servidor de medios recibe el flujo WHIP para todos los demás. `TeeBlock` divide el video:

```csharp
var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);
pipeline.Connect(screen.Output, tee.Input);

// Espectadores locales: video sin comprimir, codificado en VP8 por espectador.
pipeline.Connect(tee.Outputs[0], webrtc.CreateNewInput(MediaBlockPadMediaType.Video));

// El servidor de medios: H.264, codificado una vez.
pipeline.Connect(tee.Outputs[1], h264.Input);
pipeline.Connect(h264.Output, whip.CreateNewInput(MediaBlockPadMediaType.Video));
```

`TeeBlock` está en `VisioForge.Core.MediaBlocks.Special`. Las dos salidas arrancan y se detienen juntas: un servidor WHIP inaccesible hace fallar el inicio de todo el pipeline. Póngalas en dos pipelines cuando el flujo local deba seguir funcionando sin el servidor.

## Límites que conviene conocer antes de publicar

### Plataformas

`WebRTCSinkBlock` funciona en Windows, macOS y Linux con el paquete `Adds` de la plataforma, y en iOS con `VisioForge.CrossPlatform.Core.iOS` 2026.11.23 o posterior: un `Core.iOS` anterior trae el plugin sin sus correcciones, así que el sink arranca, pero `StopAsync` puede quedarse bloqueado cuando un espectador se va durante su negociación. `WHIPSinkBlock` funciona en las mismas plataformas con los mismos paquetes. En Linux ambos necesitan además `gstreamer1.0-plugins-bad` y `gstreamer1.0-nice` de la distribución. Compruebe `IsAvailable()` al iniciar, como hacen ambos programas.

### Códecs

`WebRTCSinkBlock` envía video VP8 y audio Opus, que reproducen todos los navegadores actuales. Admite video y audio sin comprimir, o VP8 y Opus ya codificados; no admite H.264. `WHIPSinkBlock` admite solo H.264 y Opus.

### Latencia

En nuestra prueba en un ordenador con Windows, un cambio en una imagen de 320×240 a 30 fotogramas por segundo llegó al fotograma decodificado del espectador entre 55 y 60 ms después de salir de la fuente (mediana de 58 ms), antes del retardo de reproducción del espectador. Sumando la captura de una ventana y la reproducción y presentación en el navegador, la captura de pantalla del principio de esta guía muestra unos 0,15 segundos.

### CPU y resolución

`WebRTCSinkBlock` ejecuta un codificador VP8 por software para cada espectador. En nuestras pruebas en un equipo Windows, una cámara 1080p y una ventana de 1902×1131 se reprodujeron a 30 fotogramas por segundo, mientras que un monitor completo de 3840×2160 alcanzó unos 20 fotogramas por segundo en el navegador. Para una pantalla 4K, reduzca el video antes del sink con `VideoResizeBlock` (espacio de nombres `VisioForge.Core.MediaBlocks.VideoProcessing`):

```csharp
var resize = new VideoResizeBlock(1920, 1080);
pipeline.Connect(screen.Output, resize.Input);
pipeline.Connect(resize.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Video));
```

Para más de unos pocos espectadores, publique una vez con WHIP y deje que el servidor de medios los atienda.

### Tasa de bits

`StartBitrate` (2 Mbit/s) es el punto de partida de cada espectador; el control de congestión (`CongestionControl`, Google Congestion Control de forma predeterminada) la sube hasta `MaxBitrate` (8 Mbit/s) mientras la red lo permite y la baja cuando no. Suba `MaxBitrate` para obtener texto nítido en una pantalla grande; establezca `CongestionControl = WebRTCSinkCongestionControl.Disabled` para mantener a cada espectador en `StartBitrate`.

### Puertos

Los valores predeterminados son 8443 para la señalización y 8080 para el servidor web. Cuando un puerto está ocupado, `StartAsync` devuelve `false` y `OnError` indica qué puerto. Los programas de esta guía usan 8090 para la página.

### NAT, STUN y TURN

En una red local no hace falta un servidor STUN. Para espectadores detrás de otras redes, establezca `StunServer` y `TurnServers` en `WebRTCSinkSettings` (`StunServer` es `stun://stun.l.google.com:19302` de forma predeterminada) y los mismos servidores en la lista `iceServers` de la página. A través de Internet, WHIP hacia un servidor de medios con dirección pública suele ser la vía más sencilla.

## Solución de problemas

| Síntoma | Causa y solución |
| --- | --- |
| `IsAvailable()` devuelve `false` | El paquete `Adds` de la plataforma no está referenciado (en iOS, falta `VisioForge.CrossPlatform.Core.iOS` o el SDK es anterior a 2026.11.23), o en Linux faltan `gstreamer1.0-plugins-bad` y `gstreamer1.0-nice`. |
| `StartAsync` devuelve `false` y `OnError` indica que el servidor web no puede escuchar | Otro programa usa el puerto web. Elija otro puerto en `WebServerAddress` o detenga el otro programa. |
| La página muestra `Disconnected, retrying...` | El servidor de señalización no es accesible: la aplicación no está en marcha, el puerto `?signalling=` es incorrecto o el firewall lo bloquea. |
| La página muestra `Waiting for the stream...` | La página llegó al servidor de señalización, pero el pipeline aún no se reproduce. Compruebe `StartAsync` y los mensajes de `OnError`. |
| La página muestra `Playing` pero no hay sonido | El video arranca silenciado. Pulse **Unmute**. |
| Un espectador de otro equipo se queda en `Connection lost` | Los medios WebRTC están bloqueados: permita la aplicación en el firewall o configure STUN/TURN en ambos lados. |
| Una captura de pantalla Direct3D 11 nunca se reproduce en el navegador y la página se reconecta sin parar | Media Blocks SDK .NET 2026.11.20 o anterior. Actualice a 2026.11.21 o posterior. |
| La pantalla se reproduce con pocos fotogramas por segundo | Una pantalla 4K se codifica por software para cada espectador. Redúzcala a 1080p con `VideoResizeBlock` o publique con WHIP. |
| WHIP: `StartAsync` devuelve `false` u `OnError` informa de un error HTTP | El punto de conexión es incorrecto, el servidor no funciona o rechazó el token. Abra en un navegador la página del reproductor de la misma URL para comprobar el servidor. |
| WHIP: el flujo llega al servidor pero queda en negro o `ICE failed` | El UDP entre la aplicación y el servidor está bloqueado. Permita el puerto UDP WebRTC del servidor (8189 en MediaMTX) o configure un servidor TURN con `IceTransportPolicy = Relay`. |

## Preguntas frecuentes

### ¿Necesito un servidor de medios para mostrar mi pantalla en un navegador?

No. `WebRTCSinkBlock` sirve la página, ejecuta la señalización y transmite a cada navegador desde su proceso. Un servidor de medios merece la pena cuando hay muchos espectadores o están en Internet; entonces publique en él con `WHIPSinkBlock`.

### ¿Por qué VP8 para los navegadores y H.264 para WHIP?

`WebRTCSinkBlock` codifica por espectador y usa VP8, que decodifican todos los navegadores y que el bloque puede adaptar al ancho de banda de cada espectador. Los servidores WHIP como MediaMTX reenvían un flujo a todos los espectadores sin recodificar, por lo que el flujo debe estar ya en un códec que acepten; H.264 es el que admiten todos los servidores y navegadores.

### ¿A cuántos espectadores puede atender WebRTCSinkBlock?

Cada espectador cuesta un codificador VP8. Un flujo 1080p atiende a unos pocos espectadores en una CPU de escritorio; en 4K, el primer espectador ya es el límite. Para más espectadores, publique una vez con WHIP.

### ¿Puedo añadir audio?

Sí. Conecte una fuente de audio sin comprimir a `CreateNewInput(MediaBlockPadMediaType.Audio)` de `WebRTCSinkBlock`, que codifica Opus por espectador, o la salida de un `OPUSEncoderBlock` a la entrada de audio de `WHIPSinkBlock`.

### ¿Funciona en macOS y Linux?

Sí, los dos, y también en iOS. Para servir cámaras sin una página de visor propia, use [`WHEPServerSinkBlock`](rtsp-cameras-in-browser-whep.md).

### ¿Puede un navegador reproducir el flujo WHIP sin la página del servidor de medios?

Sí. El servidor expone el flujo por WHEP, y una página con unas 20 líneas de JavaScript lo reproduce: el reproductor de [Cámaras IP RTSP en el navegador con WHEP](rtsp-cameras-in-browser-whep.md#el-lado-del-navegador-un-reproductor-whep-minimo) funciona con la URL `/whep` de MediaMTX.

## Véase también

- [Bloques sink → WebRTC](../Sinks/index.md#webrtc): referencia de `WebRTCSinkBlock` y `WebRTCSinkSettings`
- [Bloques sink → WHIP](../Sinks/index.md#whip): referencia de `WHIPSinkBlock` y `WHIPSinkSettings`
- [Cámaras IP RTSP en el navegador con WHEP](rtsp-cameras-in-browser-whep.md): cámaras IP en una página web sin recodificar
- [Bloque Fuente de Pantalla](../Sources/index.md#bloque-fuente-de-pantalla): ajustes de captura de pantalla, ventana y monitor
- [Grabación previa al evento](pre-event-recording.md): conserve los segundos anteriores a un evento mientras transmite
