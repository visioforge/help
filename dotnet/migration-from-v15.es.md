---
title: Guía de Migración — VisioForge .NET SDK v15 a v2026
description: Guía paso a paso para actualizar VisioForge .NET SDK v15 a v2026, cubriendo DirectShow y migración a X-engines multiplataforma.
sidebar_label: Migración desde v15
order: 25
---

# Guía de Migración: v15 a v2026

Esta guía le ayuda a actualizar su aplicación de VisioForge .NET SDK v15.x a v2026.x. El SDK v2026 contiene **dos tipos de motores**, y puede elegir la ruta de migración que mejor se adapte a sus necesidades:

- **Ruta A: Permanecer en DirectShow** — Cambios mínimos en el código, misma arquitectura solo para Windows. Ideal para aplicaciones en producción que necesitan una actualización rápida y de bajo riesgo.
- **Ruta B: Migrar a X-engines** — Cambios significativos en la API, pero obtiene soporte multiplataforma, códecs modernos y nuevas características. Ideal para aplicaciones que planean una actualización importante.

> **Importante:** Las clases legacy de DirectShow (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) **siguen siendo totalmente compatibles y se actualizan activamente** en v2026. No necesita migrar a X-engines de inmediato. Recomendamos la Ruta A primero, y luego migrar a X-engines cuando su agenda lo permita.

## Cambios en el framework objetivo

La siguiente tabla muestra los frameworks objetivo compatibles en v2026:

| Framework | Motores DirectShow | X-engines |
|-----------|-------------------|-----------|
| .NET Framework 4.6.1+ | Compatible | Compatible |
| .NET Core 3.1 | Compatible | Compatible |
| .NET 5.0 / 6.0 / 7.0 | Compatible | Compatible |
| .NET 8.0 | Compatible | Compatible |
| .NET 9.0 | Compatible | Compatible |
| .NET 10.0 | Compatible | Compatible |

> **Nota:** Las clases DirectShow requieren un TFM de Windows (por ejemplo, `net10.0-windows`) o .NET Framework. Para X-engines, los TFMs específicos de plataforma controlan el objetivo: `net10.0-windows` para Windows, `net10.0-macos` para macOS, `net10.0-ios` para iOS, `net10.0-android` para Android. `net10.0` sin sufijo de plataforma es solo para Linux.

---

## Ruta A: DirectShow v15 → v2026 DirectShow (Impacto Mínimo)

Esta ruta mantiene su código existente basado en DirectShow con cambios mínimos.

### Cambios en paquetes NuGet

| Paquete v15 | Paquete v2026 |
|-------------|---------------|
| `VisioForge.DotNet.Core` | `VisioForge.DotNet.Core` (mismo nombre, nueva versión) |

También están disponibles paquetes específicos por producto:

- `VisioForge.DotNet.VideoCapture`
- `VisioForge.DotNet.VideoEdit`
- `VisioForge.DotNet.MediaPlayer`

### Cambios en espacios de nombres

**No se requieren cambios en los espacios de nombres.** Todos los espacios de nombres de DirectShow permanecen iguales:

| Espacio de nombres | Estado |
|-----------|--------|
| `VisioForge.Core.VideoCapture` | Sin cambios |
| `VisioForge.Core.VideoEdit` | Sin cambios |
| `VisioForge.Core.MediaPlayer` | Sin cambios |
| `VisioForge.Core.Types` | Sin cambios |
| `VisioForge.Core.Types.VideoCapture` | Sin cambios |
| `VisioForge.Core.Types.Output` | Sin cambios |
| `VisioForge.Core.Types.VideoEffects` | Sin cambios |
| `VisioForge.Core.Types.AudioEffects` | Sin cambios |
| `VisioForge.Core.Types.Events` | Sin cambios |
| `VisioForge.Core.Types.MediaPlayer` | Sin cambios |
| `VisioForge.Core.Types.VideoEdit` | Sin cambios |

### Cambios en la API

La API de DirectShow permanece prácticamente sin cambios de v15 a v2026:

- **No se requiere inicialización del SDK** (igual que en v15)
- Mismos nombres de clases: `VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`
- Mismos patrones de creación: `new VideoCaptureCore(videoView)`, `new MediaPlayerCore(videoView)`, `new VideoEditCore(videoView)`
- Misma enumeración de dispositivos: `VideoCapture1.Video_CaptureDevices()`, `.Audio_CaptureDevices()`
- Misma configuración de salida: `VideoCapture1.Output_Format = mp4Output;`
- Misma configuración de modo: `VideoCapture1.Mode = VideoCaptureMode.VideoCapture;`
- Mismos nombres de eventos: `OnError`, `OnStop`, etc.

### Solución de problemas

| Problema | Solución |
|---------|----------|
| Espacio de nombres `VisioForge.Core.Types` no encontrado | Verifique que el paquete NuGet correcto esté instalado (`VisioForge.DotNet.Core` o paquete específico del producto) |
| Tipos o clases faltantes | Para .NET 8+, asegúrese de que su TFM incluya `-windows` (por ejemplo, `net8.0-windows`, no `net8.0`). Para .NET Framework, use `net461` o `net472`. |
| `VideoView` de WPF no encontrado | El proyecto debe apuntar a un TFM de Windows (por ejemplo, `net8.0-windows`) con `<UseWPF>true</UseWPF>` |

---

## Ruta B: Migrar a X-Engines (Multiplataforma)

Esta ruta migra de las clases DirectShow a las nuevas clases X-engine multiplataforma. Es un cambio más significativo pero proporciona grandes beneficios.

### ¿Por qué migrar a X-engines?

- **Multiplataforma**: Windows, macOS, Linux, iOS, Android
- **Códecs modernos**: AV1, HEVC con codificación por hardware (NVIDIA NVENC, AMD AMF, Intel QSV)
- **Múltiples salidas simultáneas**: Grabar en MP4 + transmitir por RTMP al mismo tiempo
- **Nuevos protocolos de streaming**: SRT, RIST, WebRTC WHIP, HLS, DASH
- **API async-first**: Soporte completo de async/await en toda la API
- **Pipeline MediaBlocks**: Construya pipelines de procesamiento de medios personalizados con bloques componibles

### Paquetes NuGet

Se necesitan paquetes adicionales para X-engines:

```xml
<!-- SDK principal -->
<PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />

<!-- Runtime de plataforma (requerido para X-engines) -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.*" />

<!-- Paquete de UI (elija uno según su framework de UI) -->
<PackageReference Include="VisioForge.DotNet.Core.UI.WPF" Version="2026.*" />
<!-- O -->
<PackageReference Include="VisioForge.DotNet.Core.UI.WinForms" Version="2026.*" />
<!-- O -->
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
<!-- O -->
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
```

### Inicialización del SDK (requerida para X-engines)

Los X-engines requieren inicialización explícita al inicio de la aplicación y limpieza al cerrar. **Esto no es necesario para las clases legacy de DirectShow.**

```csharp
// Al inicio de la aplicación (por ejemplo, Window_Loaded, OnStartup)
await VisioForgeX.InitSDKAsync();

// Al cerrar la aplicación (por ejemplo, Window_Closing, OnExit)
VisioForgeX.DestroySDK();
```

> **Advertencia:** Si el SDK no se desinicializa correctamente, la aplicación puede bloquearse al salir.

### Migración de espacios de nombres

| Espacio de nombres v15 (DirectShow) | Espacio de nombres v2026 (X-Engine) |
|---|---|
| `VisioForge.Core.VideoCapture` | `VisioForge.Core.VideoCaptureX` |
| `VisioForge.Core.VideoEdit` | `VisioForge.Core.VideoEditX` |
| `VisioForge.Core.MediaPlayer` | `VisioForge.Core.MediaPlayerX` |
| `VisioForge.Core.Types.VideoCapture` | `VisioForge.Core.Types.X.Sources` + `VisioForge.Core.Types.X.VideoCapture` |
| `VisioForge.Core.Types.Output` | `VisioForge.Core.Types.X.Output` |
| `VisioForge.Core.Types.VideoEffects` | `VisioForge.Core.Types.X.VideoEffects` |
| `VisioForge.Core.Types.AudioEffects` | `VisioForge.Core.Types.X.AudioEffects` |
| `VisioForge.Core.Types.Events` | `VisioForge.Core.Types.Events` (sin cambios) |
| `VisioForge.Core.Types.MediaPlayer` | `VisioForge.Core.Types.X.Sources` |
| `VisioForge.Core.Types.VideoEdit` | `VisioForge.Core.Types.X.VideoEdit` |
| — (nuevo) | `VisioForge.Core.Types.X.AudioRenderers` |
| — (nuevo) | `VisioForge.Core.Types.X.VideoEncoders` |
| — (nuevo) | `VisioForge.Core.Types.X.AudioEncoders` |
| — (nuevo) | `VisioForge.Core.Types.X.Sinks` |
| — (nuevo) | `VisioForge.Core.MediaBlocks` |

### Migración de clases

| v15 (DirectShow) | v2026 (X-Engine) | Notas |
|---|---|---|
| `VideoCaptureCore` | `VideoCaptureCoreX` | Constructor directo en lugar de factory |
| `VideoEditCore` | `VideoEditCoreX` | |
| `MediaPlayerCore` | `MediaPlayerCoreX` | |
| `VideoCaptureSource` | `VideoCaptureDeviceSourceSettings` | Fuertemente tipado |
| `AudioCaptureSource` | vía `device.CreateSourceSettingsVC()` | Creación basada en dispositivo |
| `MP4Output` | `MP4Output` | Mismo nombre, diferente espacio de nombres |
| `MP4HWOutput` | `MP4Output` con codificador HW | Ver sección de codificadores |
| `AVIOutput` | `AVIOutput` | Mismo nombre, diferente espacio de nombres |
| `WMVOutput` | `WMVOutput` | Mismo nombre, diferente espacio de nombres |
| `MOVOutput` | `MOVOutput` | Mismo nombre, diferente espacio de nombres |
| `MPEGTSOutput` | `MPEGTSOutput` | Mismo nombre, diferente espacio de nombres |
| `WebMOutput` | `WebMOutput` | Mismo nombre, diferente espacio de nombres |
| `VideoCaptureMode` | No necesario | La vista previa es el modo predeterminado; agregue salidas para grabar |

---

### Video Capture: Migración lado a lado

#### Creación del motor

```csharp
// v15 (DirectShow)
using VisioForge.Core.VideoCapture;

VideoCaptureCore VideoCapture1;
VideoCapture1 = await VideoCaptureCore.CreateAsync(VideoView1 as IVideoView);

// v2026 (X-Engine)
using VisioForge.Core.VideoCaptureX;

VideoCaptureCoreX VideoCapture1;
await VisioForgeX.InitSDKAsync();  // Una sola vez al inicio de la aplicación
VideoCapture1 = new VideoCaptureCoreX(VideoView1 as IVideoView);
```

#### Enumeración de dispositivos

```csharp
// v15 (DirectShow) — síncrono, método de instancia
foreach (var device in VideoCapture1.Video_CaptureDevices())
{
    cbVideoInputDevice.Items.Add(device.Name);
}

foreach (var device in VideoCapture1.Audio_CaptureDevices())
{
    cbAudioInputDevice.Items.Add(device.Name);
}

foreach (string device in VideoCapture1.Audio_OutputDevices())
{
    cbAudioOutputDevice.Items.Add(device);
}
```

```csharp
// v2026 (X-Engine) — asíncrono, singleton compartido
using VisioForge.Core.MediaBlocks;

// Opción 1: Enumeración única
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
foreach (var device in videoDevices)
{
    cbVideoInputDevice.Items.Add(device.DisplayName);  // Nota: DisplayName, no Name
}

var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
foreach (var device in audioDevices)
{
    cbAudioInputDevice.Items.Add(device.DisplayName);
}

var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
foreach (var device in audioOutputs)
{
    cbAudioOutputDevice.Items.Add(device.DisplayName);
}

// Opción 2: Monitoreo de conexión en caliente (nueva característica)
DeviceEnumerator.Shared.OnVideoSourceAdded += (sender, device) =>
{
    cbVideoInputDevice.Items.Add(device.DisplayName);
};
await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();
```

#### Configuración de fuente de video

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbVideoInputDevice.Text);
VideoCapture1.Video_CaptureDevice.Format = cbVideoInputFormat.Text;
VideoCapture1.Video_CaptureDevice.Format_UseBest = cbUseBestFormat.IsChecked == true;
VideoCapture1.Video_CaptureDevice.FrameRate = new VideoFrameRate(30.0);
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.Types.X.Sources;

var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var deviceItem = devices.FirstOrDefault(d => d.DisplayName == cbVideoInputDevice.Text);

var videoSourceSettings = new VideoCaptureDeviceSourceSettings(deviceItem);

// Establecer formato (tipado, no basado en cadenas)
var formatItem = deviceItem.VideoFormats
    .FirstOrDefault(f => f.Name == cbVideoInputFormat.Text);
if (formatItem != null)
{
    videoSourceSettings.Format = formatItem.ToFormat();
    videoSourceSettings.Format.FrameRate = new VideoFrameRate(30.0);
}

VideoCapture1.Video_Source = videoSourceSettings;
```

#### Configuración de fuente de audio

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource(cbAudioInputDevice.Text);
VideoCapture1.Audio_CaptureDevice.Format = cbAudioInputFormat.Text;
VideoCapture1.Audio_CaptureDevice.Format_UseBest = cbUseBestFormat.IsChecked == true;
VideoCapture1.Audio_OutputDevice = cbAudioOutputDevice.Text;
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.AudioRenderers;

var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var deviceItem = audioDevices.FirstOrDefault(d => d.DisplayName == cbAudioInputDevice.Text);

var formatItem = deviceItem.Formats
    .FirstOrDefault(f => f.Name == cbAudioInputFormat.Text);
IVideoCaptureBaseAudioSourceSettings audioSource = deviceItem.CreateSourceSettingsVC(formatItem?.ToFormat());
VideoCapture1.Audio_Source = audioSource;

// Dispositivo de salida de audio
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var outputDevice = audioOutputs.FirstOrDefault(d => d.DisplayName == cbAudioOutputDevice.Text);
VideoCapture1.Audio_OutputDevice = new AudioRendererSettings(outputDevice);
```

#### Salida y grabación

```csharp
// v15 (DirectShow) — salida única
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Mode = VideoCaptureMode.VideoCapture;
VideoCapture1.Output_Filename = "output.mp4";

var mp4Output = new MP4Output();
// configurar mp4Output...
VideoCapture1.Output_Format = mp4Output;

await VideoCapture1.StartAsync();
```

```csharp
// v2026 (X-Engine) — múltiples salidas simultáneas
using VisioForge.Core.Types.X.Output;

// No se necesita propiedad Mode — la vista previa es el modo predeterminado
// Agregar salida(s) para grabación
VideoCapture1.Outputs_Add(new MP4Output("output.mp4"), false);

// Puede agregar múltiples salidas simultáneamente:
// VideoCapture1.Outputs_Add(new WebMOutput("output.webm"), false);

await VideoCapture1.StartAsync();
```

#### Solo vista previa (sin grabación)

```csharp
// v15 (DirectShow)
VideoCapture1.Mode = VideoCaptureMode.VideoPreview;
await VideoCapture1.StartAsync();

// v2026 (X-Engine) — simplemente no agregue ninguna salida
await VideoCapture1.StartAsync();
```

#### Codificación acelerada por hardware

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.Output;

var mp4Output = new MP4HWOutput();
VideoCapture1.Output_Format = mp4Output;
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;

var mp4Output = new MP4Output("output.mp4");
mp4Output.Video = new NVENCH264EncoderSettings();  // NVIDIA
// o: mp4Output.Video = new QSVH264EncoderSettings();  // Intel
// o: mp4Output.Video = new AMFH264EncoderSettings();  // AMD
VideoCapture1.Outputs_Add(mp4Output, false);
```

#### Control de volumen

```csharp
// v15 (DirectShow)
VideoCapture1.Audio_OutputDevice_Volume_Set(70);
VideoCapture1.Audio_OutputDevice_Balance_Set(0);

// v2026 (X-Engine)
VideoCapture1.Audio_OutputDevice_Volume = 0.7f;  // 0.0 a 1.0
```

#### Limpieza

```csharp
// v15 (DirectShow)
VideoCapture1.Dispose();

// v2026 (X-Engine)
await VideoCapture1.DisposeAsync();
VisioForgeX.DestroySDK();  // Al cerrar la aplicación
```

---

### Media Player: Migración lado a lado

#### Creación del motor y reproducción

```csharp
// v15 (DirectShow)
using VisioForge.Core.MediaPlayer;

MediaPlayerCore _player;
_player = new MediaPlayerCore(VideoView1 as IVideoView);
_player.Audio_OutputDevice = cbAudioOutput.Text;
_player.Playlist_Clear();
_player.Playlist_Add("video.mp4");
await _player.PlayAsync();
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();  // Una sola vez al inicio de la aplicación

MediaPlayerCoreX _player;
_player = new MediaPlayerCoreX(VideoView1);

var source = await UniversalSourceSettingsV2.CreateAsync(new Uri("video.mp4"));
await _player.OpenAsync(source);
await _player.PlayAsync();
```

#### Diferencias clave

| Característica | v15 (DirectShow) | v2026 (X-Engine) |
|---------|-------------------|-------------------|
| Configuración de fuente | `Playlist_Add("file.mp4")` | `OpenAsync(UniversalSourceSettingsV2)` |
| Posición | `_player.Position` (propiedad) | `await _player.Position_GetAsync()` |
| Duración | `_player.Duration` (propiedad) | `await _player.DurationAsync()` |
| Versión | `_player.SDK_Version()` (instancia) | `MediaPlayerCoreX.SDK_Version` (estático) |
| Bucle | `_player.Looping` | `_player.Loop` |

---

### Video Edit: Migración lado a lado

#### Creación del motor

```csharp
// v15 (DirectShow)
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

VideoEditCore VideoEdit1;
VideoEdit1 = new VideoEditCore(VideoView1 as IVideoView);
VideoEdit1.Input_AddVideoFile("input.mp4");
VideoEdit1.Output_Filename = "output.mp4";
VideoEdit1.Output_Format = new MP4Output();
await VideoEdit1.StartAsync();
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.VideoEditX;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;

await VisioForgeX.InitSDKAsync();  // Una sola vez al inicio de la aplicación

VideoEditCoreX VideoEdit1;
VideoEdit1 = new VideoEditCoreX(VideoView1 as IVideoView);
VideoEdit1.Input_AddVideoFile("input.mp4");
VideoEdit1.Output_Format = new MP4Output("output.mp4");
await VideoEdit1.StartAsync();
```

#### Diferencias clave

| Característica | v15 (DirectShow) | v2026 (X-Engine) |
|---------|-------------------|-------------------|
| Espacio de nombres | `VisioForge.Core.VideoEdit` | `VisioForge.Core.VideoEditX` |
| Tipos de salida | `VisioForge.Core.Types.Output` | `VisioForge.Core.Types.X.Output` |
| Nombre de archivo de salida | Propiedad separada `Output_Filename` | Incluido en el constructor: `new MP4Output("ruta")` |
| Efectos | `VisioForge.Core.Types.VideoEffects` | `VisioForge.Core.Types.X.VideoEffects` |
| Tamaño de video | `VisioForge.Core.Types.Size` | `VisioForge.Core.Types.Size` (sin cambios) |

---

### Eventos

La mayoría de los eventos mantienen los mismos nombres en ambos motores:

| Evento | v15 | v2026 X-Engine | Notas |
|-------|-----|----------------|-------|
| `OnError` | Sí | Sí | `ErrorsEventArgs` (igual) |
| `OnStop` | Sí | Sí | |
| `OnStart` | Sí | Sí | |
| `OnAudioVUMeter` | — | Sí | Nuevo en X-engine |
| `OnOutputStarted` | — | Sí | Nuevo — eventos por salida |
| `OnOutputStopped` | — | Sí | Nuevo — eventos por salida |
| `OnBarcodeDetected` | — | Sí | Nuevo |
| `OnFaceDetected` | — | Sí | Nuevo |
| `OnMotionDetection` | Sí | Sí | |
| `OnVideoFrameBuffer` | Sí | Sí | |

---

## Preguntas Frecuentes

### "El espacio de nombres VisioForge.Core.Types ya no está disponible"

Esto depende de qué motor esté utilizando:

- **Clases DirectShow**: `VisioForge.Core.Types` todavía existe. Asegúrese de tener el paquete NuGet correcto instalado. Para .NET 8+, asegúrese de que su TFM incluya `-windows`.
- **Clases X-engine**: Use los sub-espacios de nombres `VisioForge.Core.Types.X.*` (por ejemplo, `VisioForge.Core.Types.X.Output`, `VisioForge.Core.Types.X.Sources`).

### "¿Puedo seguir usando las clases DirectShow (VideoCaptureCore, etc.)?"

Sí. Las clases DirectShow son totalmente compatibles y se actualizan activamente en v2026. Puede actualizar el paquete NuGet y mantener su código existente con cambios mínimos.

### "MP4HWOutput no se encuentra"

En X-engines, `MP4HWOutput` se reemplaza por `MP4Output` con un codificador de hardware específico:

```csharp
var mp4 = new MP4Output("output.mp4");
mp4.Video = new NVENCH264EncoderSettings();  // o QSVH264, AMFH264, etc.
```

### "VideoCaptureMode no se encuentra"

Los X-engines no usan una propiedad `Mode`. La vista previa es el comportamiento predeterminado. Para grabar, agregue salidas con `Outputs_Add()`.

### "El método Audio_CaptureDevices() no se encuentra"

En X-engines, la enumeración de dispositivos usa un singleton asíncrono compartido:

```csharp
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
```

### "¿Cuál es la estrategia de migración recomendada para aplicaciones en producción?"

1. **Primero**: Actualice a v2026 manteniendo las clases DirectShow (Ruta A) — bajo riesgo, cambios mínimos en el código
2. **Pruebe**: Verifique que su aplicación funcione correctamente con la nueva versión del paquete
3. **Luego**: Cuando esté listo, migre a X-engines (Ruta B) — módulo por módulo si es necesario
4. **Ambos motores pueden coexistir** en la misma aplicación durante el período de transición

---

Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
