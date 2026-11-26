---
title: Videollamada WebRTC bidireccional con navegador en C# .NET
description: Cree una videollamada 1:1 entre una aplicación C# y un navegador por WebRTC, con cancelación de eco, silencio y cámara apagada, sin servidor de medios.
sidebar_label: Videollamada con un navegador (WebRTC)
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - Streaming
  - Webcam
  - WebRTC
  - VP8
  - Opus
  - C#
  - JavaScript
  - NuGet
primary_api_classes:
  - WebRTCSourceBlock
  - WebRTCSourceSettings
  - WebRTCSinkBlock
  - WebRTCSinkSettings
  - EchoCancellationBlock
  - MediaBlocksPipeline

---

# Videollamada WebRTC bidireccional entre una aplicación C# y un navegador

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Ejemplo de demostración"
    Esta guía sigue la [WebRTC Video Call Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20Video%20Call%20Demo): una cámara y un micrófono con cancelación de eco, silencio y cámara apagada, el video y la voz de quien llama, y una página de llamada que sirve la propia aplicación.

    La referencia de los bloques está en [Bloques fuente → WebRTC](../Sources/index.md#bloque-fuente-webrtc), [Bloques sink → WebRTC](../Sinks/index.md#webrtc) y [Procesamiento de audio → Cancelación de eco](../AudioProcessing/index.md#cancelacion-de-eco).

## Qué va a construir

Una videollamada entre una aplicación .NET de escritorio y una persona en un navegador web: un agente de soporte y un cliente, un quiosco y un operador, un instrumento de laboratorio y un experto remoto. Los dos lados se ven y se oyen con una latencia inferior a un segundo, y no hay nada entre ellos: ni servidor de medios, ni servicio en la nube, ni plugin de navegador.

```mermaid
graph LR;
    subgraph app ["Su aplicación .NET"]
        cam["Cámara + micrófono"] --> EchoCancellationBlock;
        EchoCancellationBlock --> WebRTCSinkBlock;
        WebRTCSourceBlock --> renderers["VideoView + altavoces"];
    end
    WebRTCSinkBlock -- "WebRTC: su video y su voz" --> browser["Navegador<br/>página de llamada"];
    browser -- "WebRTC: el video y la voz de quien llama" --> WebRTCSourceBlock;
```

Tres bloques hacen el trabajo:

| Bloque | Papel en la llamada |
| --- | --- |
| `WebRTCSinkBlock` | Envía su cámara y su micrófono al navegador. Ejecuta el servidor de señalización y sirve la página de llamada. |
| `WebRTCSourceBlock` | Recibe la cámara y el micrófono del navegador, decodificados, para un `VideoRendererBlock` y los altavoces. |
| `EchoCancellationBlock` | Elimina la voz de quien llama que reproducen sus altavoces y que su micrófono vuelve a captar, para que quien llama no oiga eco. También suprime el ruido y ajusta el nivel del micrófono. |

## Cuándo usarlo

- **Una aplicación y una persona que llama a la vez**, en la misma red o a través de internet con un servidor TURN.
- Quiere la llamada dentro de su propia aplicación, junto a sus controles, grabaciones o mediciones, y que quien llama no necesite más que un enlace.
- No quiere ejecutar ni pagar un servidor de medios.

Para un flujo hacia muchos espectadores use solo el `WebRTCSinkBlock` ([Pantalla y cámara al navegador](webrtc-whip.md)), y para publicar a través de un servidor de medios use WHIP y WHEP ([Reproductor WebRTC (WHEP)](webrtc-whep-player.md)).

## Requisitos previos

- Windows x64 (la plataforma en la que se ejecutan la demo y las pruebas), .NET 10.
- Media Blocks SDK .NET 2026.11.26 o posterior, con los paquetes `VisioForge.CrossPlatform.Core.Windows.x64` y `VisioForge.CrossPlatform.Core.Windows.Adds.x64`. El paquete `.Adds` incluye los plugins de WebRTC y de cancelación de eco.
- Una cámara y un micrófono, y Chrome, Edge, Firefox o Safari para quien llama.

Compruebe `WebRTCSinkBlock.IsAvailable()`, `WebRTCSourceBlock.IsAvailable()` y `EchoCancellationBlock.IsAvailable()` antes de construir el pipeline: cada uno devuelve `false` cuando falta el paquete `.Adds`.

## Cómo se encuentran los dos lados

El navegador y la aplicación se encuentran en el servidor de señalización que ejecuta el `WebRTCSinkBlock` (WebSocket, puerto 8443 por defecto):

1. El `WebRTCSinkBlock` se registra como **productor**: cualquiera en el servidor puede pedir su flujo.
2. El `WebRTCSourceBlock`, sin `ProducerPeerId` ni `ConnectToFirstProducer`, se registra como consumidor que **espera una llamada**.
3. La página de llamada se registra como productor y como oyente. Pide el flujo de la aplicación (una sesión WebRTC) y llama al `WebRTCSourceBlock` que espera con su propia cámara y su micrófono (una segunda sesión).

Cada sentido es su propia sesión WebRTC, así que los dos pueden usar códecs y tasas de bits distintos, y el sink adapta su tasa de bits a la red de quien llama.

## El pipeline

```csharp
var pipeline = new MediaBlocksPipeline();

// Su lado: cámara y micrófono hacia el navegador. El sink sirve la página de llamada desde wwwroot.
var sink = new WebRTCSinkBlock(new WebRTCSinkSettings
{
    SignallingServerHost = "127.0.0.1",
    SignallingServerPort = 8443,
    WebServerDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
    WebServerAddress = "http://127.0.0.1:8080/",
});

var camera = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(videoDevice));
var microphone = new SystemAudioSourceBlock(audioDevice.CreateSourceSettings());
var echoCancellation = new EchoCancellationBlock(new EchoCancellationSettings());

pipeline.Connect(camera.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(microphone.Output, echoCancellation.Input);
pipeline.Connect(echoCancellation.Output, sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// El lado de quien llama: los renderizadores esperan la llamada. La voz de quien llama pasa por el
// cancelador de eco de camino a los altavoces, para que el cancelador sepa qué quitar del micrófono.
var remoteVideo = new VideoRendererBlock(pipeline, RemoteView) { IsSync = false };
pipeline.AddBlock(remoteVideo);
pipeline.Connect(echoCancellation.PlaybackOutput, new AudioRendererBlock().Input);

await pipeline.StartAsync();

// La fuente se une al servidor de señalización del sink cuando este funciona, y espera a que la página la llame.
var source = new WebRTCSourceBlock(new WebRTCSourceSettings { SignallingServer = "ws://127.0.0.1:8443" });
source.VideoOutput.OnDemandConnect = true;
source.AudioOutput.OnDemandConnect = true;

pipeline.AddBlockLive(source);
pipeline.ConnectLive(source.VideoOutput, remoteVideo.Input);
pipeline.ConnectLive(source.AudioOutput, echoCancellation.PlaybackInput);
pipeline.SyncBlockStateLive(source);
```

Por qué la fuente se añade después de `StartAsync`: su cliente de señalización se conecta en cuanto la fuente arranca, y el servidor del sink solo escucha cuando el pipeline está en marcha. `OnDemandConnect = true` mantiene abiertas las salidas de la fuente para `ConnectLive`.

`WebRTCSinkBlock.ConsumerAdded` y `ConsumerRemoved` indican cuándo quien llama se une y se va. Cuando cuelga y vuelve a llamar, el mismo `WebRTCSourceBlock` acepta la nueva llamada por las mismas salidas; no hace falta reconstruir nada.

## La página de llamada

El `wwwroot/index.html` de la demo es un único archivo de JavaScript sin bibliotecas. Las partes importantes:

```javascript
// Registrarse como productor (para llamar a la aplicación) y como oyente (para ver cuándo llega).
// callPage marca las páginas de llamada: los manejadores de 'list' y 'peerStatusChanged' omiten los
// productores cuyo meta lo lleva, así dos páginas abiertas nunca se toman por la aplicación.
send({ type: 'setPeerStatus', roles: ['producer', 'listener'], meta: { callPage: true } });
send({ type: 'list' });          // el sink de la aplicación: recibir su flujo
send({ type: 'listConsumers' }); // la fuente de la aplicación que espera: llamarla

// list → startSession con el ID de par del sink: el sink envía una oferta y la página responde.
// listConsumers → startSession con el ID de par de la fuente: el servidor responde con
// startSession, y la página ofrece su cámara y su micrófono.
```

La página abre un `RTCPeerConnection` por sesión y dirige cada mensaje `peer` según su `sessionId`. Copie el archivo tal cual o úselo como referencia para su propia página.

## Silencio y cámara apagada

- **Silencio**: ponga un `VolumeBlock` antes del cancelador de eco y asigne `Mute = true`. La llamada conserva su pista de audio; quien llama oye silencio.
- **Cámara apagada**: ponga un `VideoBalanceBlock` después de la cámara y asigne `Settings.Contrast = 0` y `Settings.Brightness = -1`. Quien llama ve una imagen negra y la pista de video se mantiene, así que volver a encender la cámara no requiere una nueva negociación.

Ambos surten efecto durante la llamada. En el navegador, `track.enabled = false` hace lo mismo del lado de quien llama.

## Límites

- **HTTPS fuera de localhost.** Los navegadores dan a una página la cámara y el micrófono solo en `https://` o en `localhost`/`127.0.0.1`, y el servidor web integrado del sink es solo `http://`. Para quien llama desde otro equipo, aloje la página de llamada en su propio sitio `https://`, deje `WebServerDirectory` vacío y dé al sink `SignallingServerCertificate` y `SignallingServerKey` para que la página llegue al servidor de señalización por `wss://`. Asigne `SignallingServerHost = "0.0.0.0"` para que la página pueda conectarse, y, como el servidor solo acepta entonces `wss://`, asigne `WebRTCSourceSettings.SignallingServer = "wss://127.0.0.1:<puerto>"` (la fuente no comprueba el certificado del servidor, así que el nombre de host de su sitio no importa aquí).
- **TURN a través de internet.** En una misma red local los dos lados se conectan directamente. Detrás de NAT o de un cortafuegos, ejecute un servidor TURN (por ejemplo coturn) y configúrelo en `WebRTCSinkSettings.TurnServers` y `WebRTCSourceSettings.TurnServers` (`turn://usuario:contraseña@host:3478`) y en los `iceServers` de la página (`{ urls: 'turn:host:3478', username, credential }`). STUN por sí solo (`StunServer`, por defecto el servidor de Google) no basta para un NAT simétrico.
- **Una llamada a la vez.** El `WebRTCSourceBlock` acepta una llamada a la vez: una llamada 1:1, no una conferencia. Una segunda persona que llama durante una llamada recibe la imagen de la aplicación, pero no se muestra, ni siquiera cuando la primera cuelga.
- **La cancelación de eco necesita los dos caminos.** Con `PlaybackInput` sin conectar, el cancelador no tiene con qué comparar y no elimina nada. Ambos caminos funcionan a 48 kHz; el cancelador remuestrea.
- **Windows es la plataforma probada.** Los mismos plugins se incluyen en `VisioForge.CrossPlatform.Core.macOS.Adds`; en otras plataformas compruebe `IsAvailable()`.

## Preguntas frecuentes

### ¿Necesito un servidor de medios o un servicio en la nube para una llamada WebRTC desde .NET?

No. El `WebRTCSinkBlock` ejecuta el servidor de señalización dentro de su aplicación y sirve la página de llamada, y los medios van directamente entre la aplicación y el navegador. Solo hace falta un servidor TURN cuando los dos lados no pueden alcanzarse directamente.

### ¿Cómo llama el navegador a la aplicación .NET?

La página de llamada se registra en el servidor de señalización de la aplicación como productor e inicia una sesión con el `WebRTCSourceBlock` que espera. El servidor indica a la página que envíe su oferta, y la fuente responde. Una segunda sesión en el otro sentido lleva la cámara y el micrófono de la aplicación.

### ¿Por qué quien llama oye eco y cómo se elimina?

La voz de quien llama sale por sus altavoces y vuelve a entrar en su micrófono. El `EchoCancellationBlock` escucha lo que reproducen los altavoces (su camino de reproducción) y lo resta del micrófono (su camino principal). Conecte ambos caminos; con auriculares el eco no se produce.

### ¿Qué códecs se usan?

El navegador y los bloques los negocian en cada sesión: VP8, H.264, VP9, H.265 o AV1 para el video, el que admitan ambos lados, y Opus para el audio. El video que entrega la fuente está decodificado, con el tamaño y la velocidad de fotogramas que envía el navegador.

### ¿Puede la llamada funcionar a través de internet?

Sí, con la página de llamada en un sitio `https://`, el servidor de señalización en `wss://` y un servidor TURN para quien llama detrás de NAT. Consulte [Límites](#limites).

## Véase también

- [Pantalla y cámara al navegador (WebRTC, WHIP)](webrtc-whip.md): un flujo hacia muchos navegadores
- [Reproductor WebRTC (WHEP)](webrtc-whep-player.md): reproducir un flujo de un servidor de medios WebRTC
- [Bloques fuente → WebRTC](../Sources/index.md#bloque-fuente-webrtc)
- [Procesamiento de audio → Cancelación de eco](../AudioProcessing/index.md#cancelacion-de-eco)
