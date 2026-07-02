---
title: SDK de voz a texto para .NET — SpeechToTextBlock (Whisper)
description: SDK de voz a texto para .NET — transcribe audio en vivo o de archivo en el dispositivo con Whisper ASR y Silero VAD, con SRT/VTT y subtítulos en vivo.
sidebar_label: Voz a Texto
tags:
  - .NET
  - AI
  - Whisper
  - Speech-to-Text
  - Speech-to-Text SDK
  - Speech Recognition
  - Live Subtitles
  - Transcription
  - VideoCaptureCoreX
  - MediaPlayerCoreX
primary_api_classes:
  - SpeechToTextBlock
  - SpeechToTextSettings
  - SileroVadSettings
  - SpeechSegment
  - SubtitleWriter
  - SubtitleRenderer
  - SubtitleStyle
---

# Voz a texto y subtítulos en vivo — SpeechToTextBlock

`SpeechToTextBlock` es un Media Block exclusivamente de audio de `VisioForge.DotNet.Core.AI.Whisper`.
Intercepta el flujo de audio, segmenta la voz con Silero VAD, la transcribe con Whisper (Whisper.net /
GGML) y dispara `OnSpeechRecognized`. El audio pasa sin cambios. El bloque implementa
`IAudioProcessingBlock`, por lo que puede insertarse en un pipeline manual o registrarse directamente
en `VideoCaptureCoreX`/`MediaPlayerCoreX`.

```csharp
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.Types.X.AI;
```

## Configuración básica del bloque

```csharp
var settings = new SpeechToTextSettings(whisperModelPath)
{
    Language = "auto",
    Task = SpeechToTextTask.Transcribe,
    EnableVad = true,
    EmitInterim = false,
};

settings.Vad.ModelPath = sileroVadModelPath;

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += (sender, e) =>
{
    foreach (var segment in e.Segments)
    {
        Console.WriteLine($"{segment.StartTime:c} - {segment.EndTime:c}: {segment.Text}");
    }
};
```

`SpeechToTextTask.Transcribe` conserva el idioma de origen. `SpeechToTextTask.Translate` traduce la voz
de origen admitida a texto en inglés.

## Configuración clave

`SpeechToTextSettings(whisperModelPath)`. A diferencia de la configuración de IA de visión, este tipo
**no** deriva de `OnnxInferenceSettings` — Whisper se ejecuta a través de Whisper.net (whisper.cpp /
GGML), no de ONNX Runtime, por lo que los parámetros específicos de ONNX (tamaño de entrada,
normalización) no aplican.

| Propiedad | Valor predeterminado | Descripción |
| --- | --- | --- |
| `WhisperModelPath` | — | Ruta absoluta al archivo del modelo Whisper GGML (`ggml-*.bin`). Obligatorio. |
| `ModelSize` | `WhisperModelSize.Base` | Etiqueta informativa de la variante de modelo en `WhisperModelPath` (ver abajo). |
| `Language` | `"auto"` | Código ISO 639-1 (`"en"`, `"es"`, `"fr"`, ...), o `"auto"` para que Whisper lo detecte. |
| `Task` | `Transcribe` | `Transcribe` (idioma de origen) o `Translate` (a inglés). |
| `Provider` | `Auto` | Solo `CPU` y `CUDA` tienen sentido para el backend GGML (sin DirectML); `Auto` elige CUDA si está presente, de lo contrario CPU. |
| `DeviceId` | `0` | ID del dispositivo de hardware cuando se selecciona un proveedor de GPU. |
| `Threads` | `0` | Hilos de CPU que usa Whisper. `0` deja que Whisper.net elija según el número de procesadores disponibles. |
| `EnableVad` | `true` | Segmenta la voz con Silero VAD antes de la transcripción. Cuando es `false`, el audio se transcribe en ventanas fijas, lo que tiende a alucinar texto durante el silencio. |
| `Vad` | nuevo `SileroVadSettings` | Configuración de VAD usada cuando `EnableVad` es `true`. |
| `FixedWindowSeconds` | `5` | Longitud de la ventana de transcripción fija cuando `EnableVad` es `false`. Limitado entre 1 y 30 s. |
| `EmitInterim` | `false` | Reservado para una futura capacidad de hipótesis provisional; actualmente no tiene efecto — solo se emiten segmentos finales. |
| `OutputSrtPath` | `null` | Ruta opcional del archivo lateral `.srt` que el bloque escribe a medida que se reconocen los segmentos finales. |
| `OutputVttPath` | `null` | Ruta opcional del archivo lateral `.vtt` (WebVTT) que el bloque escribe a medida que se reconocen los segmentos finales. |

## Configuración de VAD

Cuando `EnableVad` es `true`, `SpeechToTextSettings.Vad` controla la segmentación de voz de Silero. Silero
VAD es un modelo ONNX diminuto (~2 MB, MIT) que clasifica ventanas cortas de audio como voz o no-voz,
usado como filtro previo en tiempo real para que el modelo Whisper (mucho más pesado) solo se ejecute
sobre voz real.

```csharp
settings.Vad = new SileroVadSettings
{
    ModelPath = sileroVadModelPath,
    SpeechThreshold = 0.5f,
    MinSilenceMs = 100,
    MinSpeechMs = 250,
    SpeechPadMs = 30,
    MaxSpeechMs = 15000,
    Provider = OnnxExecutionProvider.CPU,
};
```

| Propiedad | Valor predeterminado | Descripción |
| --- | --- | --- |
| `ModelPath` | — | Ruta absoluta a `silero_vad.onnx`. |
| `SpeechThreshold` | `0.5` | Umbral de probabilidad de voz (0..1). Súbalo en entornos ruidosos para reducir falsos disparos. |
| `MinSilenceMs` | `100` | Silencio final mínimo, en ms, que termina un segmento de voz. |
| `MinSpeechMs` | `250` | Duración mínima de una racha de voz, en ms, para ser emitida (descarta ruidos espurios). |
| `SpeechPadMs` | `30` | Relleno de inicio, en ms, añadido antes de cada segmento detectado. |
| `MaxSpeechMs` | `15000` | Longitud máxima de segmento, en ms, antes de que el segmentador corte forzosamente una racha continua. |
| `Provider` | `CPU` | Proveedor de ejecución para la sesión de VAD — el modelo es diminuto (~1 ms/ventana en CPU), por lo que la GPU añade latencia sin beneficio. |
| `DeviceId` | `0` | ID del dispositivo de hardware cuando se selecciona un proveedor de GPU. |

Los pesos GGML de Whisper y el modelo Silero VAD se descargan en tiempo de ejecución; ninguno se
distribuye en los paquetes NuGet del SDK.

### Tamaños de modelo de Whisper

`WhisperModelSize` es informativo — nombra archivos de pesos GGML de Whisper conocidos para que una
aplicación pueda elegir cuál descargar. El archivo realmente cargado siempre es
`SpeechToTextSettings.WhisperModelPath`.

| Valor | Tamaño aprox. | Notas |
| --- | --- | --- |
| `Tiny` | ~75 MB | El más rápido, menor precisión. Bueno para transcripción en tiempo real en CPU. |
| `Base` (predeterminado) | ~142 MB | Un buen valor predeterminado en tiempo real para CPU. |
| `Small` | ~466 MB | Notablemente más preciso; tiempo real con GPU o una CPU rápida. |
| `Medium` | ~1.5 GB | Alta precisión; normalmente necesita GPU para tiempo real. |
| `LargeV3` | ~3 GB | Máxima precisión; se recomienda encarecidamente una GPU. |
| `LargeV3Turbo` | ~1.6 GB | Precisión cercana a la de los modelos grandes a una fracción del costo. |
| `TinyQuantized` | ~31 MB | `Tiny`, cuantizado con Q5_1. |
| `BaseQuantized` | ~57 MB | `Base`, cuantizado con Q5_1. |
| `SmallQuantized` | ~181 MB | `Small`, cuantizado con Q5_1. |
| `MediumQuantized` | ~514 MB | `Medium`, cuantizado con Q5_0. |
| `LargeV3TurboQuantized` | ~547 MB | `LargeV3Turbo`, cuantizado con Q5_0. Equilibrio recomendado entre precisión y velocidad cuantizadas. |

Las variantes solo en inglés (`*.en`) no están enumeradas; indique su ruta directamente. Los pesos
tienen licencia MIT.

## Segmentos reconocidos

Cada `SpeechSegment` en `SpeechRecognizedEventArgs.Segments`:

| Propiedad | Descripción |
| --- | --- |
| `Text` | El texto reconocido para el segmento. |
| `StartTime` / `EndTime` | `TimeSpan`, relativo al inicio del flujo — listo para usar en SRT/VTT o programación en pantalla. |
| `IsFinal` | Reservado para distinguir hipótesis provisionales de finales una vez que existan resultados provisionales; los segmentos actualmente siempre son finales (`true`). |
| `Language` | Código ISO 639-1 detectado/usado, o `null` si se desconoce. |
| `Confidence` | Confianza promedio de los tokens (0..1), o `0` cuando el modelo no reporta probabilidades de token. |

## Ayudantes de subtítulos

### SubtitleWriter — archivos SRT/VTT

`SubtitleWriter` escribe los segmentos de voz reconocidos en un archivo lateral SubRip (`.srt`) o
WebVTT (`.vtt`), añadiendo una entrada por cada segmento final. Es seguro para subprocesos (thread-safe);
los segmentos provisionales (no finales) se ignoran.

```csharp
using VisioForge.Core.AI.Whisper.Subtitles;

using var writer = new SubtitleWriter("captions.srt", SubtitleFormat.Srt);
stt.OnSpeechRecognized += (sender, e) =>
{
    foreach (var segment in e.Segments)
    {
        writer.Add(segment);
    }
};
```

Si solo necesita archivos, es más sencillo establecer `OutputSrtPath` y/o `OutputVttPath` en
`SpeechToTextSettings` — el bloque crea y gestiona internamente su(s) propia(s) instancia(s) de
`SubtitleWriter` a medida que se reconocen los segmentos finales, y las descarta por usted.

### SubtitleRenderer — subtítulos en pantalla

`SubtitleRenderer` controla una única superposición de texto en un `OverlayManagerBlock`: muestra el
último subtítulo y lo oculta automáticamente tras la duración de visualización del segmento.

```csharp
using SkiaSharp;
using VisioForge.Core.AI.Whisper.Subtitles;

var style = new SubtitleStyle
{
    FontName = "Arial",
    FontSize = 32,
    Color = SKColors.White,
    X = 50,
    Y = 50,
    MinDisplay = TimeSpan.FromSeconds(1.5),
    MaxDisplay = TimeSpan.FromSeconds(6),
};

var subtitleRenderer = new SubtitleRenderer(overlayManagerBlock, style);
stt.OnSpeechRecognized += subtitleRenderer.OnSpeechRecognized;

// ... más tarde, al finalizar:
subtitleRenderer.Dispose(); // elimina la superposición y detiene el temporizador de ocultamiento automático
```

Conecte `SubtitleRenderer.OnSpeechRecognized` directamente como el manejador de eventos del bloque.
Limita el tiempo en pantalla dentro de `[MinDisplay, MaxDisplay]` según la duración del segmento y
llama a `OverlayManagerBlock.Video_Overlay_Update` desde cualquier hilo que lo invoque — despache la
llamada si su framework de UI lo requiere. Valores predeterminados de `SubtitleStyle`:
`FontName = "Arial"`, `FontSize = 32`, `Color = White`, `X = 50`, `Y = 50`, `MinDisplay = 1.5 s`,
`MaxDisplay = 6 s`.

!!! note "Aún sin demo incluida"
    `SubtitleRenderer` existe en el SDK y está documentado a partir de su código fuente, pero
    actualmente ninguna demo incluida lo utiliza — las demos de Live Subtitles X actualizan una
    etiqueta de UI directamente desde `OnSpeechRecognized` en su lugar. Use `SubtitleWriter` o
    `OutputSrtPath`/`OutputVttPath` si solo necesita un archivo de subtítulos.

## Pipeline manual de Media Blocks

Coloque `SpeechToTextBlock` en la cadena de audio antes del renderizador o la salida de audio:

```csharp
var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

pipeline.Connect(audioSource.Output, stt.Input);
pipeline.Connect(stt.Output, audioRenderer.Input);
```

## Transcripción de micrófono en vivo con VideoCaptureCoreX

Para captura, se requiere una fuente de audio. Si desea análisis sin monitoreo de altavoz ni
grabación, termine la cadena de audio con un renderizador nulo no sincronizado:

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

core.Audio_Processing_AddBlock(stt); // antes de StartAsync
await core.StartAsync();
```

`Audio_OutputBlock` construye y termina la cadena de audio sin habilitar la reproducción por altavoz
ni la grabación en archivo. El motor asume la propiedad del bloque de salida asignado tras el inicio.

## Transcripción de archivo con MediaPlayerCoreX

Para reproducción, `Audio_Play` debe ser `true` para que se construya la cadena de audio. Un
renderizador nulo no sincronizado permite que la fuente se ejecute sin salida real de altavoz:

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

player.Audio_Processing_AddBlock(stt); // antes de OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Consulte [Uso de bloques de IA con VideoCaptureCoreX y MediaPlayerCoreX](x-engines.md) para la API
completa de `Audio_Processing_*`/`Audio_OutputBlock` y las reglas de ciclo de vida.

## Subprocesamiento, ritmo y tiempo de vida

VAD y Whisper se ejecutan **de forma síncrona en el hilo de streaming de GStreamer**: el audio se
segmenta y transcribe en el lugar, y el segmento final se vuelca al final del flujo. Nada se descarta
— toda la entrada se transcribe sin pérdidas, y la posición del pipeline sigue el frente de
transcripción (útil para una barra de progreso). Como la transcripción se ejecuta en línea, la fuente
se ajusta al ritmo de Whisper: si Whisper es más lento que el tiempo real, la fuente ascendente
(incluido un dispositivo de captura en vivo) se limita en lugar de perder audio. En la práctica, Whisper
Base funciona muy por encima del tiempo real, por lo que no es el cuello de botella para una fuente
típica.

`OnSpeechRecognized` se dispara en ese mismo hilo de streaming — nunca toque la UI directamente desde
el manejador; despache la llamada al hilo/dispatcher de la UI.

Después de que una sesión de captura o reproducción se inicia, el motor asume la propiedad de las
instancias conectadas de `SpeechToTextBlock` y las descarta cuando la sesión se detiene. Cree un
bloque nuevo para la siguiente sesión.

## Casos de uso

- **Subtitulado en vivo** — subtítulos en tiempo real para una transmisión en vivo, webinar o
  superposición de accesibilidad.
- **Transcripción de reuniones y llamadas** — transcribe una fuente de micrófono junto con la captura
  de `VideoCaptureCoreX`.
- **Indexación y búsqueda de medios** — transcribe por lotes archivos de video/audio grabados para
  hacer su contenido buscable.
- **Autoría de subtítulos** — genera archivos laterales `.srt`/`.vtt` a partir del video de origen sin
  un servicio de transcripción de terceros.
- **Subtítulos de traducción** — establezca `Task = SpeechToTextTask.Translate` para subtitular voz que
  no está en inglés, en inglés.

## Solución de problemas

| Síntoma | Causa probable | Solución |
| --- | --- | --- |
| Nunca se reconoce ningún segmento | `WhisperModelPath` no es válido, o la cadena de audio no está construida | Confirme que el archivo del modelo GGML existe en esa ruta; para captura/reproducción, confirme que la cadena de audio está activa (ver `Audio_OutputBlock` abajo). |
| Whisper "alucina" texto durante el silencio | `EnableVad` es `false` | Habilite VAD (`EnableVad = true`, el valor predeterminado) para que Whisper solo se ejecute sobre voz detectada, no sobre ventanas fijas que pueden estar en silencio. |
| La transcripción se retrasa respecto al audio en vivo | Whisper es más lento que el tiempo real en el hardware/tamaño de modelo actual | Elija un `WhisperModelSize`/archivo de modelo más pequeño, o use `Provider = CUDA` en una máquina con GPU NVIDIA. |
| Los segmentos se cortan a mitad de frase | `SileroVadSettings.MaxSpeechMs` corta forzosamente una locución continua larga | Este es un límite deliberado (15 s por defecto) para que una transcripción en curso no pueda crecer sin límite; aumente `MaxSpeechMs` si su escenario necesita segmentos ininterrumpidos más largos y puede tolerar el límite mayor. |
| No llega audio al bloque en `VideoCaptureCoreX`/`MediaPlayerCoreX` | Cadena de audio no construida (falta `Audio_Source`/`Audio_Play`) | Consulte [Transcripción de micrófono en vivo con VideoCaptureCoreX](#transcripcion-de-microfono-en-vivo-con-videocapturecorex) y [Transcripción de archivo con MediaPlayerCoreX](#transcripcion-de-archivo-con-mediaplayercorex) arriba. |
| El archivo `.srt`/`.vtt` está vacío | Los segmentos nunca se finalizaron, o la ruta es incorrecta | Confirme que `OutputSrtPath`/`OutputVttPath` apuntan a una ruta con permisos de escritura y que realmente se detectó voz; solo se escriben los segmentos finales. |

## Preguntas frecuentes

### ¿SpeechToTextBlock requiere conexión a internet?

No — la transcripción se ejecuta completamente en el dispositivo mediante Whisper.net/GGML; los
archivos del modelo se descargan una vez desde su aplicación (o se incluyen), y no se llaman por
solicitud a través de la red.

### ¿Qué idiomas admite?

Whisper es multilingüe — establezca `Language` con un código ISO 639-1, o déjelo en `"auto"` para que
Whisper detecte el idioma hablado automáticamente.

### ¿Puedo traducir la voz a subtítulos en inglés en lugar de transcribir el idioma de origen?

Sí — establezca `SpeechToTextSettings.Task = SpeechToTextTask.Translate`.

### ¿Cómo obtengo subtítulos en pantalla en vivo en lugar de solo un evento?

Conecte `SubtitleRenderer.OnSpeechRecognized` como el manejador de eventos del bloque contra un
`OverlayManagerBlock` — consulte [SubtitleRenderer — subtítulos en pantalla](#subtitlerenderer-subtitulos-en-pantalla).
Si solo necesita archivos SRT/VTT, establezca `OutputSrtPath`/`OutputVttPath` en su lugar.

### ¿Qué tamaño de modelo Whisper debería usar?

Comience con `Base` (el predeterminado) para transcripción en tiempo real en CPU. Pase a
`Small`/`Medium`/`LargeV3` (o sus variantes cuantizadas) para mayor precisión si tiene una GPU o
puede tolerar un procesamiento más lento que el tiempo real; consulte la tabla de
[Tamaños de modelo de Whisper](#tamanos-de-modelo-de-whisper) para ver el equilibrio completo.

## Demos

- **[Live Subtitles Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Subtitles%20Demo)** — demo de pipeline de Media Blocks para WPF.
- **[Live Subtitles MB](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/Live%20Subtitles%20MB)** — la misma demo de Media Blocks para MAUI.
- **[Live Subtitles](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/Live%20Subtitles)** — demo de consola sin interfaz (descarga los modelos en la primera ejecución).

Las demos dedicadas de subtítulos en vivo de `VideoCaptureCoreX`/`MediaPlayerCoreX`
(`Capture Live Subtitles X`, `Capture Live Subtitles X WPF`, `Player Live Subtitles X`,
`Player Live Subtitles X WPF`) están en el conjunto de demos del SDK y se enlazarán aquí una vez
publicadas en el repositorio público de ejemplos.
