---
title: Reconexión RTSP y Fallback Switch en C# .NET — Todos SDKs
description: Gestiona desconexiones de cámaras IP con eventos de reconexión, DisconnectEventInterval y FallbackSwitch en los SDK de VisioForge.
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

# Reconexión RTSP y Fallback Switch en C# / .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Soporte multiplataforma"
    Los eventos de reconexión funcionan en todos los SDK. El `FallbackSwitch` declarativo se basa en GStreamer y funciona en **Windows, macOS, Linux, Android e iOS** en los motores `X` modernos y Media Blocks — no está disponible en los motores clásicos solo-Windows DirectShow. Consulta la [matriz de soporte de plataformas](../../platform-matrix.md) y la [guía de despliegue en Linux](../../deployment-x/Ubuntu.md).

Las cámaras IP pierden conexiones — parpadeos de red, ciclos de energía, reinicios de router, falta de keyframes. Los SDKs VisioForge te dan dos formas de manejarlo:

- **Reactivo** — tu código se suscribe a un evento de desconexión, luego detiene y reinicia la fuente.
- **Declarativo** — configuras un `FallbackSwitch` en la fuente, el pipeline cambia automáticamente a una imagen / tarjeta de texto / stream alternativo, y tu código nunca ve el fallo.

Elige según el SDK que uses y la UX que quieras. Esta guía cubre ambos enfoques en todos los SDK de la familia.

## Qué enfoque está disponible dónde

| SDK | Reactivo (detectar + reiniciar) | Declarativo (FallbackSwitch) |
|---|:---:|:---:|
| VideoCaptureCore (clásico, Windows) | ✅ `OnNetworkSourceDisconnect` + `DisconnectEventInterval` | ⛔ |
| MediaPlayerCore (clásico, Windows) | ✅ `OnNetworkSourceStop` (FFMPEG) / `OnError` | ⛔ |
| VideoCaptureCoreX (multiplataforma) | ✅ pipeline `OnNetworkSourceDisconnect` / `OnError` | ✅ vía `RTSPSourceSettings.FallbackSwitch` |
| MediaPlayerCoreX (multiplataforma) | ✅ pipeline `OnNetworkSourceDisconnect` / `OnError` | ✅ |
| Media Blocks SDK | ✅ evento de pipeline | ✅ `FallbackSwitchSourceBlock` |

Regla general: en los motores clásicos Windows solo tienes reactivo. En cualquier motor moderno (multiplataforma), prefiere declarativo para la UX, y añade reactivo encima para telemetría.

## Reactivo — VideoCaptureCore (clásico)

`IPCameraSourceSettings.DisconnectEventInterval` define cuánto espera el SDK sin frames entrantes antes de disparar el evento de desconexión. El valor por defecto es 10 segundos; bájalo a 3–5 segundos para vigilancia.

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
    // Retroceso exponencial: 1s, 2s, 4s, 8s... limitado a 30s.
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

`OnNetworkSourceDisconnect` se dispara desde un hilo temporizador — marshall al hilo UI (`Invoke` / `Dispatcher.Invoke`) antes de tocar controles.

## Reactivo — MediaPlayerCore (clásico)

El `MediaPlayerCore` clásico expone `OnNetworkSourceStop` en el motor FFMPEG; para otros motores y cualquier fallo genérico, suscríbete a `OnError`.

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
    // Log pero sin teardown — deja que el bucle de reintentos arriba maneje las caídas reales.
    Debug.WriteLine($"Player error: {e.Message}");
};
```

## Reactivo — pipelines Media Blocks

`MediaBlocksPipeline` expone `OnNetworkSourceDisconnect` con `NetworkSourceDisconnectEventArgs` rico — útil cuando múltiples fuentes RTSP comparten una app y necesitas saber *cuál* se cayó.

```csharp
using VisioForge.Core.Types.Events;

pipeline.OnNetworkSourceDisconnect += (s, e) =>
{
    // e.SourceElementName — el elemento GStreamer que falló
    // e.ErrorMessage       — descripción corta
    // e.DebugInfo          — salida de debug de GStreamer (puede ser null)
    // e.Uri                — la URL RTSP que falló (puede ser null)
    // e.Timestamp          — cuándo el SDK detectó el fallo
    Log($"[{e.Timestamp:HH:mm:ss}] {e.SourceElementName} cayó: {e.ErrorMessage}");
};

pipeline.OnError += (s, e) =>
{
    Log($"Error de pipeline: {e.Message}");
};
```

## Reactivo — motores X (VideoCaptureCoreX / MediaPlayerCoreX)

`VideoCaptureCoreX` y `MediaPlayerCoreX` **no** exponen públicamente su `MediaBlocksPipeline` interno. Para reaccionar a desconexiones en estos motores:

1. Suscríbete a `OnError` en el motor — se dispara para todos los errores a nivel de pipeline incluyendo pérdidas de fuentes RTSP.
2. Usa el `FallbackSwitch` declarativo (abajo) para mantener la UI viva durante caídas transitorias.
3. Si necesitas el `NetworkSourceDisconnectEventArgs` granular por fuente, construye el pipeline de captura directamente con `MediaBlocksPipeline` + `RTSPSourceBlock` y usa el patrón Media Blocks de arriba.

```csharp
videoCaptureCoreX.OnError += (s, e) =>
{
    // e.Message lleva el error a nivel de motor; para caídas RTSP el texto incluye
    // el nombre del elemento fuente. Empareja este handler con un bucle de reintentos o FallbackSwitch.
    Log($"VideoCaptureCoreX error: {e.Message}");
};
```

## Declarativo — Visión general de FallbackSwitch

`FallbackSwitch` envuelve una fuente en vivo con failover automático. Cuando la fuente principal deja de producir frames por más de `TimeoutMs`, el pipeline cambia a un fallback preconfigurado (texto, imagen o media alternativo) y mantiene tu `VideoView` renderizando. Cuando la fuente principal se recupera, el pipeline cambia de vuelta.

### El contenedor `FallbackSwitchSettings`

```csharp
public class FallbackSwitchSettings
{
    public bool Enabled { get; set; } = false;
    public FallbackSwitchSettingsBase Fallback { get; set; }
    public bool EnableVideo { get; set; } = true;
    public bool EnableAudio { get; set; } = true;
    public int MinLatencyMs { get; set; } = 0;
    public string FallbackVideoCaps { get; set; }   // ej. "video/x-raw,width=1920,height=1080"
    public string FallbackAudioCaps { get; set; }   // ej. "audio/x-raw,rate=48000,channels=2"
}
```

### Parámetros base (compartidos por todos los tipos de fallback)

```csharp
public abstract class FallbackSwitchSettingsBase
{
    public int  TimeoutMs        { get; set; } = 5000;   // silencio antes de que el fallback se active
    public int  RestartTimeoutMs { get; set; } = 5000;   // espera entre intentos de reinicio de la fuente principal
    public int  RetryTimeoutMs   { get; set; } = 60000;  // ventana de rendición tras fallos repetidos
    public bool ImmediateFallback{ get; set; } = false;  // mostrar fallback desde el primer hueco de frames
    public bool RestartOnEos     { get; set; } = false;  // tratar EOS como fallo (para bucles)
    public bool ManualUnblock    { get; set; } = false;  // requerir que la app llame a Unblock() en recuperación
}
```

### Tres tipos de fallback

1. **`StaticTextFallbackSettings`** — renderiza una tarjeta de texto "Cámara offline".
2. **`StaticImageFallbackSettings`** — muestra un logo de marca, última instantánea o placa "reconectando".
3. **`MediaBlockFallbackSettings`** — reproduce un stream en vivo alternativo o un archivo en bucle.

## Fallback — texto estático

```csharp
using SkiaSharp;
using VisioForge.Core.Types.X.Sources;

var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticTextFallbackSettings
    {
        Text            = "Cámara offline — reconectando...",
        FontFamily      = "Arial",
        FontSize        = 32f,
        TextColor       = SKColors.White,
        BackgroundColor = SKColors.Black,
        Position        = new SKPoint(0.5f, 0.5f),   // centro
        TimeoutMs       = 3000,
        RestartTimeoutMs= 5000,
    },
};
```

## Fallback — imagen estática

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

## Fallback — fuente de media alternativa

```csharp
var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new MediaBlockFallbackSettings
    {
        FallbackUri      = "file:///C:/assets/standby-loop.mp4",
        IsLive           = false,
        RestartOnEos     = true,      // reiniciar el bucle cuando el archivo termine
        TimeoutMs        = 3000,
    },
};
```

`FallbackUri` acepta cualquier URI que GStreamer pueda leer — `file://`, otro `rtsp://`, HLS, HTTP. `CustomPipeline` te permite inyectar una línea de lanzamiento GStreamer personalizada para escenarios avanzados (ej. `videotestsrc pattern=snow`).

## Usando FallbackSwitch — alto nivel (`RTSPSourceSettings.FallbackSwitch`)

La vía más simple: adjunta el objeto de configuración directamente a `RTSPSourceSettings` y pásalo a `VideoCaptureCoreX` / `MediaPlayerCoreX` como siempre. Sin plomería extra de pipeline.

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

## Usando FallbackSwitch — bajo nivel (`FallbackSwitchSourceBlock`)

En el Media Blocks SDK cableas el fallback directamente a nivel de pipeline vía `FallbackSwitchSourceBlock`. Esto funciona con *cualquier* fuente — RTSP, HTTP MJPEG, archivo, dispositivo — no solo RTSP.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;

var fallbackSwitch = new FallbackSwitchSourceBlock(
    mainSourceSettings: rtspSettings,                          // IVideoSourceSettings
    fallbackSettings:   new FallbackSwitchSettings
    {
        Enabled = true,
        Fallback = new StaticTextFallbackSettings { Text = "OFFLINE" },
    });

// Telemetría — funciona junto al UI declarativo
pipeline.OnNetworkSourceDisconnect += (s, e) =>
    metrics.RecordDisconnect(e.SourceElementName, e.Uri, e.Timestamp);

// FallbackSwitchSourceBlock expone pads VideoOutput / AudioOutput separados — no hay un único Output.
pipeline.Connect(fallbackSwitch.VideoOutput, renderer.Input);
await pipeline.StartAsync();

// Inspeccionar estado en runtime:
string status = fallbackSwitch.GetStatus();
var stats = fallbackSwitch.GetStatistics();

// Si ManualUnblock está configurado, llama Unblock() cuando quieras que la reproducción reanude la fuente principal.
// fallbackSwitch.Unblock();
```

## Patrones Comunes

**Retroceso exponencial** — en el bucle reactivo, duplica el retraso en cada reconexión fallida, limita a 30 s. Evita que machaques la caché ARP de una cámara muerta.

**UX declarativa + telemetría reactiva** — deja que `FallbackSwitch` mantenga la pantalla viva, y usa `pipeline.OnNetworkSourceDisconnect` para alimentar tu dashboard de monitoreo / alerta Slack / log NVR. Ningún enfoque excluye al otro.

**Muro multi-cámara** — nunca derribes todo el grid por un fallo. Consulta la [guía de grid RTSP multi-cámara](../../mediablocks/Guides/multi-camera-rtsp-grid.md) para el patrón un-pipeline-por-cámara; adjunta un `FallbackSwitch` a cada motor independientemente.

**Nota multiplataforma** — `FallbackSwitch` depende del elemento GStreamer `fallbackswitch`, que viene con el redist X. Los clásicos solo-Windows `VideoCaptureCore` / `MediaPlayerCore` no lo tienen — usa el enfoque reactivo allí.

## Solución de Problemas

**El fallback nunca se activa** — ¿`Enabled = true`? ¿`TimeoutMs` menor que el gap que quieres capturar? Los primeros frames necesitan tener éxito antes de que el timeout empiece a contar — una cámara que falla en el primer handshake es un error de inicio de pipeline, no un timeout.

**El fallback se activa pero nunca sale** — la fuente principal no se está recuperando. Verifica `RestartTimeoutMs` (muy alto retrasa el siguiente reintento) y `RetryTimeoutMs` (después de esta ventana el bloque deja de reintentar). Con `ManualUnblock = true` debes llamar `fallbackSwitch.Unblock()` tú mismo.

**El fallback de imagen muestra negro** — desajuste de caps de códec entre el pipeline principal y el decodificador de fallback. Establece `FallbackVideoCaps` explícitamente, ej. `"video/x-raw,width=1920,height=1080,format=RGB"`, que coincida con el formato esperado del renderizador.

**Fallback de texto — fuente / posición incorrectas** — `FontFamily` debe existir en la plataforma objetivo (Arial está en Windows y macOS; prefiere `DejaVuSans` en Linux). `Position` son fracciones 0.0–1.0 del cuadro de video.

**`OnNetworkSourceDisconnect` se dispara repetidamente** — el bloque de fuente está reintentando y fallando en rápida sucesión. Sube `RestartTimeoutMs` a 10–15 s en una red reconocidamente inestable, o envuelve el logging con un debounce.

**Excepciones de hilo UI** — `OnNetworkSourceDisconnect` se dispara desde el hilo del bus GStreamer. Usa `Dispatcher.Invoke` (WPF) / `Control.Invoke` (WinForms) / `MainThread.BeginInvokeOnMainThread` (MAUI) antes de tocar controles.

## Documentación Relacionada

- [Inmersión profunda en el protocolo RTSP](../network-streaming/rtsp.md) — cómo funciona RTSP, opciones de transporte y arquitectura de streaming
- [Configuración de fuente de cámara RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — referencia de `IPCameraSourceSettings` / `RTSPSourceSettings`
- [Integración de cámara IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) — descubrimiento + PTZ; combina re-descubrimiento ONVIF con FallbackSwitch para cámaras difíciles
- [Reproductor RTSP de Media Blocks](../../mediablocks/Guides/rtsp-player-csharp.md) — pipeline mono-cámara
- [Grid RTSP multi-cámara (muro NVR)](../../mediablocks/Guides/multi-camera-rtsp-grid.md) — muro 4×4 con fallback por celda
- [Guardar stream RTSP sin re-codificación](../../mediablocks/Guides/rtsp-save-original-stream.md) — archivar junto con la vista previa en vivo
- [Salida servidor RTSP](../../mediablocks/RTSPServer/index.md) — resiliencia del lado servidor para tu propio stream
- [Matriz de soporte de plataformas](../../platform-matrix.md) — detalles de códecs y aceleración por hardware

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código incluyendo demos RTSP de vista previa, grabación y MultiView.
