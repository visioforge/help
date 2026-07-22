---
title: Detección de eventos de audio en .NET — YAMNet AudioSet
description: Detecte sonidos reales — sirena, ladrido, rotura de cristal, alarma, música, voz — en audio en vivo o de archivo con YAMNet de AudioSet, en el dispositivo.
sidebar_label: Detección de eventos de audio
tags:
  - .NET
  - AI
  - YAMNet
  - AudioSet
  - Audio Event Detection
  - Sound Classification
  - Sound Recognition
  - Surveillance
  - VideoCaptureCoreX
  - MediaPlayerCoreX
primary_api_classes:
  - AudioEventDetectorBlock
  - AudioEventDetectorSettings
  - AudioEventArgs
  - AudioEventScore
  - AudioScoresEventArgs
---

# Detección de eventos de audio — AudioEventDetectorBlock

`AudioEventDetectorBlock` es un Media Block solo de audio de `VisioForge.DotNet.Core.AI`. Interviene el
flujo de audio, lo remuestrea a 16&nbsp;kHz mono y ejecuta un clasificador ONNX **YAMNet** (Google,
AudioSet) sobre ventanas cortas para reconocer sonidos del mundo real — sirena, ladrido de perro,
rotura de cristal, alarma, música, voz y cientos más (521 clases de AudioSet). El audio pasa sin
cambios. El bloque implementa `IAudioProcessingBlock`, por lo que puede insertarse en un pipeline
manual o registrarse en `VideoCaptureCoreX`/`MediaPlayerCoreX`. Es un complemento natural de las
funciones de análisis de objetos y PTZ para vigilancia y monitorización.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.AI;
using VisioForge.Core.Types.X.Sources;
```

## Configuración básica del bloque

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    Threshold = 0.5f,        // confianza que una clase debe alcanzar para activarse
    SmoothingWindows = 3,    // media móvil que suprime picos de una sola ventana
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += (sender, e) =>
{
    // Se genera en un hilo de trabajo en segundo plano — sincronice con el hilo de UI antes de tocar la UI.
    Console.WriteLine($"{e.Label} ({e.Confidence:F2}) [{e.Start:mm\\:ss} - {e.End:mm\\:ss}]");
};
```

Conéctelo entre una fuente de audio y un sumidero (o cualquier bloque de audio posterior):

```csharp
var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(mediaFile, renderVideo: false, renderAudio: true));

// Sin sincronizar: un archivo sin conexión se analiza tan rápido como el modelo lo permite.
// Mantenga IsSync = true (el valor predeterminado) para una fuente en vivo que también quiera monitorizar.
var sink = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };

pipeline.Connect(source.AudioOutput, detector.Input);
pipeline.Connect(detector.Output, sink.Input);

await pipeline.StartAsync();
```

## Uso con VideoCaptureCoreX y MediaPlayerCoreX

`AudioEventDetectorBlock` implementa `IAudioProcessingBlock`, por lo que, en lugar de construir un
pipeline manual, puede registrarlo en un motor X. El bloque **debe añadirse antes de que la sesión
comience**: la lista de bloques de procesamiento se consume mientras se construye el pipeline, y un
bloque añadido después se ignora.

Para la captura, termine la cadena de audio con un renderizador nulo no sincronizado si desea el
análisis sin monitorización por altavoz ni grabación:

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += Detector_OnAudioEvent;

core.Audio_Processing_AddBlock(detector); // antes de StartAsync
await core.StartAsync();
```

Para la reproducción, `Audio_Play` debe ser `true` para que se construya la cadena de audio:

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += Detector_OnAudioEvent;

player.Audio_Processing_AddBlock(detector); // antes de OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Consulte [Uso de bloques de IA con VideoCaptureCoreX y MediaPlayerCoreX](x-engines.md) para conocer la
API completa de `Audio_Processing_*`/`Audio_OutputBlock` y las reglas de ciclo de vida.

## Cómo se generan los eventos

El bloque clasifica cada ventana de `WindowSeconds`, promedia las puntuaciones por trama del modelo
y las suaviza con una media móvil sobre `SmoothingWindows` ventanas. Una **histéresis** por clase
(disparador Schmitt) colapsa un sonido continuo en un único evento: una clase entra en `Threshold` y
sale en `Threshold * ReleaseRatio` (0.7 por defecto). `OnAudioEvent` se genera **una vez por sonido**,
cuando termina (cae por debajo del umbral de liberación) o al final del flujo, llevando el intervalo
completo `Start`..`End` y la `Confidence` máxima. Así, una sirena continua produce un solo evento con
duración, no una avalancha de duplicados.

`Start` y `End` se miden sobre el audio que el bloque ha analizado realmente, contando desde cero
cuando arranca el pipeline: no son posiciones del contenido multimedia. En una reproducción o
captura directa desde el principio ambos coinciden. Se separan en cuanto se omite o se repite audio
por debajo del bloque: posicionarse en otro punto de un `MediaPlayerCoreX` no reinicia este reloj, así
que tras un reposicionamiento los tiempos informados ya no se corresponden con las posiciones del
archivo, y un hueco en una fuente en vivo deja todos los tiempos posteriores adelantados por la
duración que falta.

La inferencia se ejecuta en un hilo de trabajo dedicado alimentado por una cola de muestras **no
acotada**, de modo que el hilo de streaming nunca se bloquea y **la cola nunca descarta muestras para
limitar su propio crecimiento**: una fuente más rápida que el trabajador (una fuente en vivo con un
proveedor lento, o un archivo decodificado a máxima velocidad por un renderizador no sincronizado) solo
hace crecer el retraso acumulado — 64&nbsp;KB por segundo encolado en mono de 16&nbsp;kHz — que se
drena en cuanto el trabajador se pone al día. Dos salidas previstas detienen el análisis en lugar de
descartar en silencio una parte de él; ambas se describen en las notas siguientes. YAMNet es diminuto:
en la CPU, unos 0.6&nbsp;ms para la ventana predeterminada de 0.975&nbsp;s y 1.8&nbsp;ms para el máximo
de 5&nbsp;s (el coste sigue al número de tramas de la ventana), más de 1000× tiempo real en ambos
casos, por lo que el proveedor de CPU suele ser suficiente.

## Ajustes clave

`AudioEventDetectorSettings(yamnetModelPath)`. A diferencia de los ajustes de IA de visión, este tipo
**no** deriva de `OnnxInferenceSettings` — YAMNet consume una forma de onda de audio, no una imagen.
Cada propiedad de abajo se lee **una sola vez, cuando se construye el bloque** durante `StartAsync`:
defínalas antes de iniciar el pipeline; modificar el objeto de ajustes después no tiene efecto.

| Propiedad | Predeterminado | Descripción |
| --- | --- | --- |
| `ModelPath` | — | Ruta absoluta al modelo ONNX de YAMNet. Requerido. |
| `Threshold` | 0.5 | Confianza que una clase debe alcanzar para iniciar un evento, limitada a 0.01–1 (un umbral de 0 nunca podría liberarse y bloquearía todas las clases). |
| `ReleaseRatio` | 0.7 | Un evento activo termina por debajo de `Threshold * ReleaseRatio` (histéresis), limitado a 0.01–1 por la misma razón. |
| `SmoothingWindows` | 3 | Ventanas sobre las que se promedian las puntuaciones (1 desactiva el suavizado). |
| `WindowSeconds` | 0.975 | Longitud de la ventana de inferencia: un parche YAMNet de 0.96&nbsp;s más un pequeño margen; limitada a 0.96–5&nbsp;s. |
| `ClassFilter` | null | Lista opcional de nombres de clase de AudioSet (p. ej. `"Siren"`, `"Dog"`). |
| `Provider` | `Auto` | Proveedor de ejecución ONNX (la CPU suele bastar). |
| `EnableScoresEvent` | false | Generar `OnScores` (top-K) para cada ventana, para un medidor en vivo. |
| `TopK` | 5 | Número de clases reportadas en el evento de puntuaciones. |

## Filtrar a sonidos específicos

Pase una lista de permitidos `ClassFilter` para reportar solo los sonidos que le interesan — por
ejemplo, un vigilante de alarmas que ignora todo lo demás:

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    ClassFilter = new[] { "Siren", "Emergency vehicle", "Police car (siren)", "Alarm", "Glass", "Gunshot, gunfire" },
};
```

Los nombres de clase son los nombres para mostrar de AudioSet y deben coincidir exactamente con uno de
ellos (no se distinguen mayúsculas, pero una errata, un singular/plural distinto o una abreviatura sí
importan). Los nombres desconocidos se ignoran con una advertencia **siempre que al menos un nombre se
resuelva**. Un filtro no vacío en el que **ningún** nombre se resuelva hace fallar la construcción del
bloque — `StartAsync` devuelve `false` y se reporta un error que nombra las entradas no resueltas — en
lugar de recurrir silenciosamente a detectar las 521 clases.

## Medidor de puntuaciones en vivo

Active `OnScores` para recibir las puntuaciones top-K de cada ventana procesada (útil para un medidor
de nivel en vivo o una vista de depuración):

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    EnableScoresEvent = true, // se lee al iniciar el pipeline — defínalo antes de StartAsync
    TopK = 5,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnScores += (sender, e) =>
{
    foreach (var s in e.Scores)
    {
        Console.WriteLine($"  {s.Label}: {s.Confidence:F2}");
    }
};
```

## Modelo

El bloque usa **YAMNet**, el clasificador de audio MobileNet-v1 de Google entrenado con
[AudioSet](https://research.google.com/audioset/), que predice 521 clases de la ontología de AudioSet.
El modelo consume una forma de onda mono de 16&nbsp;kHz (el bloque se encarga del remuestreo y la
mezcla a mono) y devuelve puntuaciones de clase por trama. Los pesos son Apache-2.0. Su aplicación
proporciona el archivo del modelo y apunta `ModelPath` a él: los paquetes NuGet del SDK no incluyen ni
los pesos ni un descargador, y el bloque nunca descarga nada por sí mismo. La demo de abajo tiene un
botón **Download** que obtiene el modelo una vez y lo almacena en caché; no se descarga nada mientras no
haga clic en él.

**Para producción, convierta usted mismo el modelo oficial TF-Hub de Google a ONNX** en lugar de
depender de una exportación ONNX de terceros. La conversión es solo un cambio de formato y no altera la
licencia Apache-2.0 de los pesos:

```bash
pip install tensorflow tensorflow-hub tf2onnx onnx
python -c "import tensorflow_hub as hub, tensorflow as tf; m=hub.load('https://tfhub.dev/google/yamnet/1'); tf.saved_model.save(m, 'yamnet_tf')"
python -m tf2onnx.convert --saved-model yamnet_tf --output yamnet.onnx --opset 13
```

Si usa una exportación ONNX de la comunidad, fíjela a una revisión inmutable y verifique su SHA-256 — la
demo WPF [Audio Event Detection Demo](#demos) descarga una exportación fijada de los pesos Apache-2.0 y
verifica el hash tras la descarga.

> El reconocimiento de eventos de sonido usa la ontología de AudioSet y el modelo YAMNet de Google,
> publicado bajo la licencia Apache-2.0.

## Demos

La demo `Audio Event Detection Demo` (Media Blocks para WPF — descarga del modelo con verificación
SHA-256, filtro de clases, registro de eventos en vivo) está en el conjunto de demos del SDK y se
enlazará aquí una vez publicada en el repositorio público de ejemplos.

## Notas

- `OnAudioEvent` y `OnScores` se generan en el hilo de trabajo de inferencia; sincronice con el hilo de
  UI antes de actualizar la UI. Un manejador que lance una excepción se captura y registra, nunca se
  propaga al hilo de trabajo.
- Al final del flujo, la ventana final y los eventos aún activos se vacían antes del desmontaje.
- En funcionamiento normal, el bloque nunca descarta audio, ni de la ruta de *paso directo* ni de la de
  análisis. La cola de análisis interna no está acotada: una fuente que supera al hilo de trabajo hace
  crecer el retraso acumulado en lugar de descartar muestras, y el audio sigue pasando aguas abajo sin
  cambios.
- Dos salidas previstas terminan el análisis en vez de perder audio en silencio, y ninguna afecta nunca
  a la ruta de *paso directo*: si el retraso acumulado ya no cabe en memoria, el bloque reporta un error
  una vez y deja de analizar ese flujo (use un proveedor de ejecución de GPU o hardware más rápido); y
  una parada explícita abandona el retraso acumulado que el hilo de trabajo no ha alcanzado — solo un
  final de flujo natural lo drena por completo.
