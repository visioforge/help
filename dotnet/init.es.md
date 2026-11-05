---
title: Inicialización del SDK .NET para video multiplataforma
description: Inicializar y desinicializar SDKs .NET para captura, edición y reproducción de video con DirectShow y motores X multiplataforma.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing

---

# Inicialización

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tipos de motores del SDK

Todos los SDKs contienen motores basados en DirectShow solo para Windows y motores X multiplataforma.

### Motores solo para Windows

- VideoCaptureCore
- VideoEditCore
- MediaPlayerCore

### Motores X

- VideoCaptureCoreX
- VideoEditCoreX
- MediaPlayerCoreX
- MediaBlocksPipeline

Los motores X requieren pasos adicionales de inicialización y desinicialización.

## Inicialización y desinicialización del SDK para motores X

Todos los motores X (`VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX`, `MediaBlocksPipeline`) utilizan un único paso de inicialización compartido. Debe inicializar el SDK antes de usar cualquier clase del SDK y desinicializar el SDK antes de que la aplicación termine. Los núcleos DirectShow solo de Windows (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) no necesitan ningún paso de inicialización.

### Llamada a InitSDKAsync

Lo recomendado es la forma asíncrona, para que el arranque en frío de GStreamer (resolución de bibliotecas nativas, escaneo del registro de plugins) no congele el hilo que llama:

```csharp
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

La forma bloqueante es equivalente y puede llamarse desde cualquier hilo. Úsala directamente cuando la inicialización deba ejecutarse en un hilo específico (ver la nota a continuación):

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

`InitSDKAsync` traslada `InitSDK` a un hilo del grupo de subprocesos mediante `Task.Run`. Los manejadores de excepciones no controladas de GStreamer / GLib se enlazan al hilo que primero desencadena la inicialización, así que si su aplicación depende de que la inicialización se ejecute en un hilo concreto (por ejemplo, por estado GLib estático de subproceso), llame a `InitSDK()` directamente en ese hilo.

Llame también a `InitSDK` antes de cualquier comprobación de disponibilidad. Hasta que se ejecute, un `IsAvailable()` que busca un elemento de GStreamer lanza una excepción que le pide llamar a `InitSDK`; no devuelve `false`. Algunos constructores que eligen un codificador o decodificador predeterminado también lanzan una excepción: en todas las plataformas, `new HEVCEncoderBlock()`, `new UniversalDecoderBlock(...)` y los constructores sin parámetros de `H264DecoderBlock`, `HEVCDecoderBlock`, `VP8DecoderBlock` y `VP9DecoderBlock`; en Windows y Linux, `new H264EncoderBlock()`.

### Desinicialización

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

`DestroySDK` no tiene variante asíncrona y puede llamarse desde cualquier hilo.

Si el SDK no se desinicializa correctamente, la aplicación puede experimentar un bloqueo al salir debido a la incapacidad de finalizar uno de sus hilos. Este problema surge porque el SDK continúa operando, impidiendo que la aplicación se cierre correctamente. Para asegurar una salida limpia, es crucial desinicializar el SDK apropiadamente según el framework de UI que esté usando.

Para aplicaciones desarrolladas usando diferentes frameworks de UI, puede desinicializar el SDK en el evento `FormClosing` u otro manejador de eventos relevante. Este enfoque asegura que el SDK se destruya correctamente antes de que la aplicación se cierre, permitiendo que todos los hilos terminen correctamente.

Además, el SDK puede destruirse desde cualquier hilo, proporcionando flexibilidad en cómo maneja el proceso de desinicialización. Como `DestroySDK` es síncrono, si quiere mantener la interfaz de usuario responsiva llámelo en un hilo en segundo plano (por ejemplo mediante `Task.Run`) — no existe una sobrecarga asíncrona de `DestroySDK`.

Implementar estas prácticas asegura que su aplicación salga sin problemas sin bloquearse, proporcionando una experiencia fluida para los usuarios. Gestionar correctamente la desinicialización del SDK es crucial para mantener la estabilidad y rendimiento de su aplicación.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.