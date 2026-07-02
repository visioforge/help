---
title: Bloques de IA con VideoCaptureCoreX y MediaPlayerCoreX
description: Inserte Media Blocks de IA — OCR, detección de objetos, reconocimiento facial, ANPR, eliminación de fondo, voz a texto — en VideoCaptureCoreX/MediaPlayerCoreX.
sidebar_label: Motores X
tags:
  - .NET
  - AI
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - Media Blocks
primary_api_classes:
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - IVideoProcessingBlock
  - IAudioProcessingBlock
  - NullRendererBlock
---

# Bloques de IA con VideoCaptureCoreX y MediaPlayerCoreX

`VideoCaptureCoreX` y `MediaPlayerCoreX` pueden alojar Media Blocks proporcionados por el usuario
dentro de sus pipelines de alto nivel. Use esta vía cuando quiera la comodidad de los motores X y
solo necesite añadir procesamiento de IA a la cadena de vídeo o audio — sin necesidad de construir
manualmente una topología de `MediaBlocksPipeline`.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.AI;
```

## API pública

Ambos motores exponen la misma API de bloques de procesamiento:

| API | Propósito |
| --- | --- |
| `Video_Processing_AddBlock(IVideoProcessingBlock block)` | Añade un bloque de procesamiento de vídeo para la siguiente sesión. |
| `Video_Processing_RemoveBlock(IVideoProcessingBlock block)` | Elimina un bloque de vídeo registrado antes de que comience la sesión. |
| `Video_Processing_Clear()` | Elimina todos los bloques de vídeo registrados antes de que comience la sesión. |
| `Video_Processing_Blocks` | Instantánea de los bloques de vídeo registrados. |
| `Audio_Processing_AddBlock(IAudioProcessingBlock block)` | Añade un bloque de procesamiento de audio para la siguiente sesión. |
| `Audio_Processing_RemoveBlock(IAudioProcessingBlock block)` | Elimina un bloque de audio registrado antes de que comience la sesión. |
| `Audio_Processing_Clear()` | Elimina todos los bloques de audio registrados antes de que comience la sesión. |
| `Audio_Processing_Blocks` | Instantánea de los bloques de audio registrados. |
| `Audio_OutputBlock` | Reemplaza el sink de audio predeterminado por un `MediaBlock` personalizado, comúnmente un `NullRendererBlock` sin sincronización para voz a texto. |

Cada bloque de IA de vídeo (`OcrBlock`, `YOLOObjectDetectorBlock`, `ObjectAnalyticsBlock`,
`FaceRecognitionBlock`, `LicensePlateRecognizerBlock`, `BackgroundRemovalBlock`,
`OnnxInferenceBlock`) implementa `IVideoProcessingBlock`. `SpeechToTextBlock` implementa
`IAudioProcessingBlock`.

## Reglas del ciclo de vida

Registre los bloques antes de que el motor construya su pipeline: antes de `StartAsync` en
`VideoCaptureCoreX`, antes de `OpenAsync`/`PlayAsync` en `MediaPlayerCoreX`. Añadir, eliminar o
limpiar bloques de procesamiento después de que haya comenzado la construcción del pipeline se
ignora y se registra como `Warning`:

- `VideoCaptureCoreX`: *"Cannot add a processing block while capture is running. Add it before
  Start()."*
- `MediaPlayerCoreX`: *"Cannot add a processing block while playback is running. Add it before
  Play()."*

(El texto del log usa los nombres clásicos `Start()`/`Play()` aunque la API pública que se invoca es
`StartAsync()`/`PlayAsync()` — las cadenas de advertencia son anteriores a los wrappers asíncronos y
se citan aquí de forma literal para que pueda buscarlas con grep.)

Si ve uno de estos mensajes en el log, la llamada al bloque ocurrió después de que la sesión ya
había comenzado (o estaba comenzando) — muévala más temprano.

Una vez que la sesión comienza, el motor pasa a ser propietario de los bloques de procesamiento
conectados y los libera cuando la sesión se detiene. Cree una nueva instancia de bloque antes de
cada nueva sesión; los eventos de los bloques se generan en los hilos del pipeline o de trabajo del
bloque — mantenga los controladores sin bloqueo y traslade los cambios de la UI al dispatcher de la
UI o al hilo principal. Si una cadena está inactiva, sus bloques registrados no se ejecutan — por
ejemplo, un bloque de procesamiento de audio requiere una fuente de audio y una cadena de audio
construida.

## Orden de inserción en el pipeline

- **`VideoCaptureCoreX`**: los bloques de IA de vídeo se insertan después de los efectos de vídeo y
  antes del sample grabber, el overlay, el tee, el renderer y las salidas — de modo que los
  overlays que dibuja el bloque de IA llegan tanto a la vista previa como a cada salida de
  grabación/streaming.
- **`MediaPlayerCoreX`**: los bloques de IA de vídeo se insertan después de los efectos de vídeo y
  antes del sample grabber, el renderer y las salidas de vídeo personalizadas.

## IA de vídeo en VideoCaptureCoreX

```csharp
var detector = new YOLOObjectDetectorBlock(new YoloDetectorSettings(modelPath)
{
    Model = ObjectDetectorModel.YOLOv8,
    DrawDetections = true,
});

detector.OnObjectsDetected += (sender, e) =>
{
    // Se genera desde el hilo de streaming.
    Console.WriteLine($"Detected {e.Objects.Length} objects.");
};

core.Video_Processing_AddBlock(detector); // antes de StartAsync
await core.StartAsync();
```

El mismo patrón se aplica a `OcrBlock`, `ObjectAnalyticsBlock`, `FaceRecognitionBlock`,
`LicensePlateRecognizerBlock`, `BackgroundRemovalBlock` y `OnnxInferenceBlock` — consulte la página
propia de cada bloque para conocer su configuración y el payload de sus eventos.

## Voz a texto en VideoCaptureCoreX

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var sttSettings = new SpeechToTextSettings(whisperModelPath)
{
    EnableVad = false, // o mantenga EnableVad = true y establezca sttSettings.Vad.ModelPath = sileroVadModelPath;
};
var stt = new SpeechToTextBlock(sttSettings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

core.Audio_Processing_AddBlock(stt); // antes de StartAsync
await core.StartAsync();
```

`EnableVad` es `true` de forma predeterminada, lo que requiere `SileroVadSettings.ModelPath` — la
construcción del pipeline falla si VAD está habilitado pero no se ha establecido ninguna ruta de
modelo. Deshabilite VAD o establezca `sttSettings.Vad.ModelPath` antes de registrar el bloque.

Para la captura, se requiere una fuente de audio. `Audio_OutputBlock` puede activar y terminar la
cadena de audio sin necesidad de `Audio_Play`, `Audio_Record`, monitorización por altavoz ni una
salida de grabación.

## IA de vídeo en MediaPlayerCoreX

```csharp
var ocr = new OcrBlock(new OcrSettings(
    detectionModelPath,
    recognitionModelPath,
    characterDictionaryPath,
    classificationModelPath)
{
    DrawResults = true,
});

ocr.OnTextDetected += Ocr_OnTextDetected;

player.Video_Processing_AddBlock(ocr); // antes de OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

## Voz a texto en MediaPlayerCoreX

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var sttSettings = new SpeechToTextSettings(whisperModelPath)
{
    EnableVad = false, // o mantenga EnableVad = true y establezca sttSettings.Vad.ModelPath = sileroVadModelPath;
};
var stt = new SpeechToTextBlock(sttSettings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

player.Audio_Processing_AddBlock(stt); // antes de OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Para la reproducción, `Audio_Play` debe ser `true` para que se construya la cadena de audio.
Reemplazar el sink por un null renderer sin sincronización evita la salida por altavoz y hace que la
transcripción no siga el ritmo de la reproducción en tiempo real.

## Solución de problemas

| Síntoma | Causa probable | Solución |
| --- | --- | --- |
| Los eventos del bloque nunca se disparan y no se registra ninguna advertencia | El bloque se registró después de que la sesión ya estaba en ejecución | Registre el bloque con `Video_Processing_AddBlock`/`Audio_Processing_AddBlock` antes de `StartAsync` (`VideoCaptureCoreX`) o antes de `OpenAsync`/`PlayAsync` (`MediaPlayerCoreX`). |
| El log muestra *"Cannot add a processing block while capture/playback is running..."* | Igual que el anterior — la llamada ocurrió después de que el pipeline comenzara a construirse | Mueva la llamada a `Video_Processing_AddBlock`/`Audio_Processing_AddBlock` más temprano en su secuencia de inicio. |
| `SpeechToTextBlock` nunca recibe audio en `VideoCaptureCoreX` | No se configuró ningún `Audio_Source` | Un bloque de procesamiento de audio requiere una cadena de audio construida — establezca `Audio_Source` (y, si no desea salida por altavoz, `Audio_OutputBlock` con un `NullRendererBlock` sin sincronización). |
| `SpeechToTextBlock` nunca recibe audio en `MediaPlayerCoreX` | `Audio_Play` se dejó en su valor predeterminado | Establezca `player.Audio_Play = true` antes de `OpenAsync`; de lo contrario la cadena de audio no se construye, sin importar los bloques de audio registrados. |
| El overlay del bloque de IA no aparece en la salida grabada/transmitida, solo en la vista previa | El bloque se registró en la cadena incorrecta, o se insertó después del punto en que el motor construye las salidas | Los bloques de IA de vídeo se insertan antes del sample grabber/overlay/tee/renderer/salidas — vea [Orden de inserción en el pipeline](#orden-de-insercion-en-el-pipeline) — así que normalmente se trata de un problema de temporización de registro, no de topología. |
| La instancia del bloque reutilizada en un segundo `StartAsync`/`PlayAsync` lanza una excepción o se comporta de forma extraña | El motor ya liberó la instancia del bloque de la sesión anterior | Cree una nueva instancia de bloque (y vuelva a suscribirse a sus eventos) para cada nueva sesión de captura/reproducción. |

## Preguntas frecuentes

### ¿Necesito reconstruir un MediaBlocksPipeline manual para usar bloques de IA con VideoCaptureCoreX/MediaPlayerCoreX?

No — ese es precisamente el objetivo de esta integración. `Video_Processing_AddBlock`/
`Audio_Processing_AddBlock` insertan las mismas instancias de bloques de IA en el pipeline interno
ya existente del motor; usted no construye ni gestiona ningún `MediaBlocksPipeline` por su cuenta.

### ¿Puedo añadir o eliminar bloques de IA mientras la captura/reproducción está en ejecución?

No — las adiciones/eliminaciones/limpiezas se ignoran (y se registran como advertencia) una vez que
ha comenzado la construcción del pipeline. Registre todo lo que necesite antes de `StartAsync` /
`OpenAsync`+`PlayAsync`, e inicie una nueva sesión con instancias de bloque nuevas si necesita
cambiar el conjunto de bloques.

### ¿Los bloques de IA de vídeo ralentizan las salidas de grabación o streaming?

Añaden su propio coste de inferencia a la cadena de vídeo, ya que se ejecutan en línea antes del
renderer y las salidas. Use `FramesToSkip` (disponible en la configuración de cada bloque de IA de
vídeo) y, cuando esté disponible, un proveedor de ejecución GPU para mantener ese coste acotado.

### ¿Puedo combinar varios bloques de IA — por ejemplo, detección de objetos y voz a texto — en una sola sesión?

Sí — registre uno o más `IVideoProcessingBlock` con `Video_Processing_AddBlock` y, por separado, un
`IAudioProcessingBlock` (como `SpeechToTextBlock`) con `Audio_Processing_AddBlock`, todos antes de
que comience la sesión. Las cadenas de procesamiento de vídeo y audio son independientes.

## Demos

Cada bloque cubierto en esta página también cuenta con demos específicas del pipeline de Media
Blocks — consulte la sección **Demos** en la página propia de cada bloque
([OCR](ocr.md#demos), [Detección de objetos](object-detection.md#demos),
[Análisis de objetos](object-analytics.md#demos), [Reconocimiento facial](face-recognition.md#demos),
[Reconocimiento de matrículas](license-plate-recognition.md#demos),
[Eliminación de fondo](background-removal.md#demo), [Voz a texto](speech-to-text.md#demos)).

El conjunto de demos del SDK incluye además 16 demos construidas directamente sobre
`VideoCaptureCoreX` / `MediaPlayerCoreX` usando la API de esta página — Detección de Objetos, OCR,
Reconocimiento Facial y Subtítulos en Vivo, cada una tanto para WPF como para MAUI, en los motores
de captura y de reproducción (`Capture Object Detection X`, `Capture Object Detection X WPF`,
`Capture OCR X`, `Capture OCR X WPF`, `Capture Face Recognition X`, `Capture Face Recognition X WPF`,
`Capture Live Subtitles X`, `Capture Live Subtitles X WPF`, `Player Object Detection X`,
`Player Object Detection X WPF`, `Player OCR X`, `Player OCR X WPF`, `Player Face Recognition X`,
`Player Face Recognition X WPF`, `Player Live Subtitles X`, `Player Live Subtitles X WPF`). Los
enlaces de GitHub se añadirán aquí una vez que estas demos se publiquen en el repositorio público de
muestras.
