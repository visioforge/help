---
title: Compositor de Video en Vivo .Net
description: Componga múltiples fuentes de video y audio en vivo en tiempo real con capacidades dinámicas de agregar/eliminar para aplicaciones de streaming y grabación.
---

# Compositor de Video en Vivo

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El Compositor de Video en Vivo es parte del [VisioForge Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) que le permite agregar y eliminar fuentes y salidas en tiempo real a un pipeline.

Esto le permite crear aplicaciones que manejan simultáneamente múltiples fuentes de video y audio.

Por ejemplo, el LVC le permite comenzar a transmitir a YouTube en el momento justo mientras graba video simultáneamente en disco.
Usando el LVC, puede crear una aplicación similar a OBS Studio.

Cada fuente y salida tiene su identificador único que puede usarse para agregar y eliminar fuentes y salidas en tiempo real.

Cada fuente y salida tiene su propio pipeline independiente que puede iniciarse y detenerse.

## Características

- Soporta múltiples fuentes de video y audio
- Soporta múltiples salidas de video y audio
- Configuración de posición y tamaño de fuentes de video
- Configuración de transparencia de fuentes de video
- Configuración del volumen de fuentes de audio

## Clase LiveVideoCompositor

El `LiveVideoCompositor` es la clase principal que permite agregar y eliminar fuentes y salidas en vivo al pipeline. Al crearlo, es necesario especificar la resolución y tasa de frames a usar. Todas las fuentes con diferente tasa de frames serán automáticamente convertidas a la tasa de frames especificada al crear el LVC.

`LiveVideoCompositorSettings` le permite configurar los parámetros de video y audio. Las propiedades clave incluyen:

- `MixerType`: Especifica el tipo de mezclador de video (ej., `LVCMixerType.OpenGL`, `LVCMixerType.D3D11` (solo Windows), o `LVCMixerType.CPU`).
- `AudioEnabled`: Un booleano que indica si el flujo de audio está habilitado.
- `VideoWidth`, `VideoHeight`, `VideoFrameRate`: Definen la resolución de video de salida y tasa de frames.
- `AudioFormat`, `AudioSampleRate`, `AudioChannels`: Definen los parámetros de audio de salida.
- `VideoView`: Un `IVideoView` opcional para renderizar la salida de video directamente.
- `AudioOutput`: Un `AudioRendererBlock` opcional para renderizar la salida de audio directamente.

También es necesario establecer el número máximo de fuentes y salidas al diseñar su aplicación, aunque esto no es un parámetro directo de `LiveVideoCompositorSettings`.

### Código de ejemplo

1. Cree una nueva instancia de la clase `LiveVideoCompositor`.

```csharp
var settings = new LiveVideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_25);
// Opcionalmente, configure otros ajustes como MixerType, AudioEnabled, etc.
// settings.MixerType = LVCMixerType.OpenGL;
// settings.AudioEnabled = true;
var compositor = new LiveVideoCompositor(settings);
```

2. Agregue fuentes y salidas de video y audio (ver abajo)
3. Inicie el pipeline.

```csharp
await compositor.StartAsync();
```

## Entrada de Video LVC

La clase `LVCVideoInput` se usa para agregar fuentes de video al pipeline LVC. La clase le permite configurar los parámetros de video y el rectángulo de la fuente de video.

Puede usar cualquier bloque que tenga un pad de salida de video. Por ejemplo, puede usar `VirtualVideoSourceBlock` para crear una fuente de video virtual o `SystemVideoSourceBlock` para capturar video desde la webcam.

Las propiedades clave para `LVCVideoInput` incluyen:

- `Rectangle`: Define la posición y tamaño de la fuente de video dentro de la salida del compositor.
- `ZOrder`: Determina el orden de apilamiento de fuentes de video superpuestas.
- `ResizePolicy`: Especifica cómo debe redimensionarse la fuente de video si su relación de aspecto difiere del rectángulo objetivo (`LVCResizePolicy.Stretch`, `LVCResizePolicy.Letterbox`, `LVCResizePolicy.LetterboxToFill`).
- `VideoView`: Un `IVideoView` opcional para previsualizar esta fuente de entrada específica.

### Uso

Al crear un objeto `LVCVideoInput`, debe especificar el `MediaBlock` a usar como fuente de datos de video, junto con `VideoFrameInfoX` describiendo el video, un `Rect` para su ubicación, y si debe `autostart`.

### Código de ejemplo

#### Fuente de video virtual

El código de ejemplo a continuación muestra cómo crear un objeto `LVCVideoInput` con un `VirtualVideoSourceBlock` como fuente de video.

```csharp
var rect = new Rect(0, 0, 640, 480);

var name = "Fuente de video [Virtual]";
var settings = new VirtualVideoSourceSettings();
var info = new VideoFrameInfoX(settings.Width, settings.Height, settings.FrameRate);
var src = new LVCVideoInput(name, _compositor, new VirtualVideoSourceBlock(settings), info, rect, true);
// Opcionalmente, configure ZOrder o ResizePolicy
// src.ZOrder = 1;
// src.ResizePolicy = LVCResizePolicy.Letterbox;
if (await _compositor.Input_AddAsync(src))
{
    // agregado exitosamente
}
else
{
    src.Dispose();
}
```

#### Fuente de pantalla

Para plataformas de escritorio, podemos capturar la pantalla. El código de ejemplo a continuación muestra cómo crear un objeto `LVCVideoInput` con un `ScreenSourceBlock` como fuente de video.

```csharp
var settings = new ScreenCaptureDX9SourceSettings();
settings.CaptureCursor = true;
settings.Monitor = 0;
settings.FrameRate = new VideoFrameRate(30);
settings.Rectangle = new Rectangle(0, 0, 1920, 1080);

var rect = new Rect(0, 0, 640, 480);
var name = $"Fuente de pantalla";
var info = new VideoFrameInfoX(settings.Rectangle.Width, settings.Rectangle.Height, settings.FrameRate);
var src = new LVCVideoInput(name, _compositor, new ScreenSourceBlock(settings), info, rect, true);
// Opcionalmente, configure ZOrder o ResizePolicy
// src.ZOrder = 0;
// src.ResizePolicy = LVCResizePolicy.Stretch;
if (await _compositor.Input_AddAsync(src))
{
    // agregado exitosamente
}
else
{ 
    src.Dispose(); 
}
```

#### Fuente de video del sistema (webcam)

El código de ejemplo a continuación muestra cómo crear un objeto `LVCVideoInput` con un `SystemVideoSourceBlock` como fuente de video.

Usamos la clase `DeviceEnumerator` para obtener los dispositivos de fuente de video. El primer dispositivo de video se usará como fuente de video. El primer formato de video del dispositivo se usará como formato de video.

```csharp
VideoCaptureDeviceSourceSettings settings = null;

var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        settings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };

        settings.Format.FrameRate = dlg.FrameRate;
    }
}

if (settings == null)
{
    MessageBox.Show(this, "No se puede configurar el dispositivo de captura de video.");
    return;
}

var name = $"Fuente de cámara [{device.Name}]";
var rect = new Rect(0, 0, 1280, 720);
var videoInfo = new VideoFrameInfoX(settings.Format.Width, settings.Format.Height, settings.Format.FrameRate);
var src = new LVCVideoInput(name, _compositor, new SystemVideoSourceBlock(settings), videoInfo, rect, true);
// Opcionalmente, configure ZOrder o ResizePolicy
// src.ZOrder = 2;
// src.ResizePolicy = LVCResizePolicy.LetterboxToFill;

if (await _compositor.Input_AddAsync(src))
{
    // agregado exitosamente
}
else
{
    src.Dispose();
}
```

## Entrada de Audio LVC

La clase `LVCAudioInput` se usa para agregar fuentes de audio al pipeline LVC. La clase le permite configurar los parámetros de audio y el volumen de la fuente de audio.

Puede usar cualquier bloque que tenga un pad de salida de audio. Por ejemplo, puede usar `VirtualAudioSourceBlock` para crear una fuente de audio virtual o `SystemAudioSourceBlock` para capturar audio desde el micrófono.

### Uso

Al crear un objeto `LVCAudioInput`, debe especificar el `MediaBlock` a usar como fuente de datos de audio, junto con `AudioInfoX` (que requiere formato, canales y tasa de muestreo) y si debe `autostart`.

### Código de ejemplo

#### Fuente de audio virtual

El código de ejemplo a continuación muestra cómo crear un objeto `LVCAudioInput` con un `VirtualAudioSourceBlock` como fuente de audio.

```csharp
var name = "Fuente de audio [Virtual]";
var settings = new VirtualAudioSourceSettings();
var info = new AudioInfoX(settings.Format, settings.SampleRate, settings.Channels);
var src = new LVCAudioInput(name, _compositor, new VirtualAudioSourceBlock(settings), info, true);            
if (await _compositor.Input_AddAsync(src))
{
    // agregado exitosamente
}
else
{
    src.Dispose();
}
```

#### Fuente de audio del sistema (DirectSound en Windows)

El código de ejemplo a continuación muestra cómo crear un objeto `LVCAudioInput` con un `SystemAudioSourceBlock` como fuente de audio.

Usamos la clase `DeviceEnumerator` para obtener los dispositivos de audio. El primer dispositivo de audio se usa como fuente de audio. El primer formato de audio del dispositivo se usa como formato de audio.

```csharp
DSAudioCaptureDeviceSourceSettings settings = null;
AudioCaptureDeviceFormat deviceFormat = null;

var device = (await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound))[0];
if (device != null)
{
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        deviceFormat = formatItem.ToFormat();
        settings = new DSAudioCaptureDeviceSourceSettings(device, deviceFormat);
    }
}    

if (settings == null)
{
    MessageBox.Show(this, "No se puede configurar el dispositivo de captura de audio.");
    return;
}

var name = $"Fuente de audio [{device.Name}]";
var info = new AudioInfoX(deviceFormat.Format, deviceFormat.SampleRate, deviceFormat.Channels);
var src = new LVCAudioInput(name, _compositor, new SystemAudioSourceBlock(settings), info, true);
if (await _compositor.Input_AddAsync(src))
{
    // agregado exitosamente
}
else
{
    src.Dispose();
}
```

## Salida de Video LVC

La clase `LVCVideoOutput` se usa para agregar salidas de video al pipeline LVC. Puede iniciar y detener el pipeline de salida independientemente del pipeline principal.

### Uso

Al crear un objeto `LVCVideoOutput`, debe especificar el `MediaBlock` a usar como salida de datos de video, su `name`, una referencia al `LiveVideoCompositor`, y si debe `autostart` con el pipeline principal. También se puede proporcionar un `MediaBlock` de procesamiento opcional. Usualmente, este elemento se usa para guardar el video como archivo o transmitirlo (sin audio).

Para salidas de video+audio, use la clase `LVCVideoAudioOutput`.

Puede usar SuperMediaBlock para hacer un pipeline de bloques personalizado para la salida de video. Por ejemplo, puede agregar un codificador de video, un muxer y un escritor de archivos para guardar el video en un archivo.

## Salida de Audio LVC

La clase `LVCAudioOutput` se usa para agregar salidas de audio al pipeline LVC. Puede iniciar y detener el pipeline de salida independientemente del pipeline principal.

### Uso

Al crear un objeto `LVCAudioOutput`, debe especificar el `MediaBlock` a usar como salida de datos de audio, su `name`, una referencia al `LiveVideoCompositor`, y si debe `autostart`.

### Código de ejemplo

#### Agregar un renderizador de audio

Agregue un renderizador de audio al pipeline LVC. Necesita crear un objeto `AudioRendererBlock` y luego crear un objeto `LVCAudioOutput`. Finalmente, agregue la salida al compositor.

El primer dispositivo se usa como salida de audio.

```csharp
var audioRenderer = new AudioRendererBlock((await DeviceEnumerator.Shared.AudioOutputsAsync())[0]); 
var audioRendererOutput = new LVCAudioOutput("Renderizador de audio", _compositor, audioRenderer, true);
await _compositor.Output_AddAsync(audioRendererOutput, true);
```

#### Agregar una salida MP3

Agregue una salida MP3 al pipeline LVC. Necesita crear un objeto `MP3OutputBlock` y luego crear un objeto `LVCAudioOutput`. Finalmente, agregue la salida al compositor.

```csharp
var mp3Output = new MP3OutputBlock(outputFile, new MP3EncoderSettings());
var output = new LVCAudioOutput(outputFile, _compositor, mp3Output, false);

if (await _compositor.Output_AddAsync(output))
{
    // agregado exitosamente
}
else
{
    output.Dispose();
}
```

## Salida de Video/Audio LVC

La clase `LVCVideoAudioOutput` se usa para agregar salidas de video+audio al pipeline LVC. Puede iniciar y detener el pipeline de salida independientemente del pipeline principal.

### Uso

Al crear un objeto `LVCVideoAudioOutput`, debe especificar el `MediaBlock` a usar como salida de datos de video+audio, su `name`, una referencia al `LiveVideoCompositor`, y si debe `autostart`. También se pueden proporcionar `MediaBlock`s de procesamiento opcionales para video y audio.

### Código de ejemplo

#### Agregar una salida MP4

```csharp
var mp4Output = new MP4OutputBlock(new MP4SinkSettings("output.mp4"), new OpenH264EncoderSettings(), new MFAACEncoderSettings());

var output = new LVCVideoAudioOutput(outputFile, _compositor, mp4Output, false); 

if (await _compositor.Output_AddAsync(output))
{
    // agregado exitosamente
}
else
{
    output.Dispose();
}
```

#### Agregar una salida WebM

```csharp
var webmOutput = new WebMOutputBlock(new WebMSinkSettings("output.webm"), new VP8EncoderSettings(), new VorbisEncoderSettings());
var output = new LVCVideoAudioOutput(outputFile, _compositor, webmOutput, false);

if (await _compositor.Output_AddAsync(output))
{
   // agregado exitosamente
}
else
{
    output.Dispose();
}
```

## Salida de Vista de Video LVC

La clase `LVCVideoViewOutput` se usa para agregar vista de video al pipeline LVC. Puede usarla para mostrar el video en pantalla.

### Uso

Al crear un objeto `LVCVideoViewOutput`, debe especificar el control `IVideoView` a usar, su `name`, una referencia al `LiveVideoCompositor`, y si debe `autostart`. También se puede proporcionar un `MediaBlock` de procesamiento opcional.

### Código de ejemplo

```csharp
var name = "[VideoView] Vista previa";
var videoRendererOutput = new LVCVideoViewOutput(name, _compositor, VideoView1, true);
await _compositor.Output_AddAsync(videoRendererOutput);
```

VideoView1 es un objeto `VideoView` que se usa para mostrar el video. Cada plataforma / framework de UI tiene su propia implementación de `VideoView`.

Puede agregar varios objetos `LVCVideoViewOutput` al pipeline LVC para mostrar el video en diferentes pantallas.

---

[Aplicación de ejemplo en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Video%20Compositor%20Demo)
