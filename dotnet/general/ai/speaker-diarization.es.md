---
title: Diarización de hablantes .NET — SpeakerDiarizationBlock
description: Diarización de hablantes en el dispositivo para .NET — «quién habló y cuándo» en audio grabado con pyannote, embeddings WeSpeaker y transcripciones diarizadas.
sidebar_label: Diarización de hablantes
tags:
  - .NET
  - AI
  - Speaker Diarization
  - Diarization SDK
  - Who Spoke When
  - Speaker Segmentation
  - Speaker Embedding
  - VideoCaptureCoreX
  - MediaPlayerCoreX
primary_api_classes:
  - SpeakerDiarizationBlock
  - SpeakerDiarizationSettings
  - SpeakerSegment
  - SpeakerSegmentEventArgs
  - DiarizedTranscriptBuilder
  - DiarizedTranscriptSegment
---

# Diarización de hablantes — SpeakerDiarizationBlock

`SpeakerDiarizationBlock` es un Media Block solo de audio que reside en el espacio de nombres
`VisioForge.Core.MediaBlocks.AI` (ensamblado `VisioForge.Core.AI`, paquete NuGet `VisioForge.DotNet.Core.AI`).
Responde a "quién habló y cuándo": intercepta el flujo de audio, encuentra turnos de habla con un modelo ONNX
de segmentación pyannote, convierte cada turno en una huella de voz con un modelo ONNX de embeddings
WeSpeaker/3D-Speaker y — una vez que la grabación ha terminado — agrupa las huellas de voz en hablantes y
publica la línea de tiempo. El audio atraviesa el bloque: la interceptación fuerza un formato de muestra de
coma flotante de 32 bits intercalado, pero la frecuencia de muestreo y el número de canales se dejan sin
modificar. El bloque implementa `IAudioProcessingBlock`, por lo que puede insertarse en una canalización
manual o registrarse directamente en `VideoCaptureCoreX`/`MediaPlayerCoreX`.

La implementación sigue la canalización de diarización de hablantes offline de referencia de
[sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx), que usa estos mismos dos modelos.

```csharp
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.Types.X.AI;
```

## La diarización es offline — la respuesta llega al final del flujo

**El bloque no informa de nada mientras el audio sigue reproduciéndose.** No es una limitación de la
implementación, es lo que el problema permite:

- El modelo de segmentación separa las voces **dentro de una única ventana de 10 segundos** y las numera
  localmente. El "hablante 1" de la ventana 5 y el "hablante 1" de la ventana 6 no tienen relación alguna — el
  modelo nunca afirma lo contrario.
- Lo único que puede ligar una voz oída en el minuto 1 con esa misma voz en el minuto 40 es **agrupar juntas
  todas las huellas de voz de la grabación**, y eso no se puede hacer hasta que existan todas ellas.

Así que el bloque analiza continuamente a medida que fluye el audio (ese trabajo es real, y por eso la
respuesta aparece casi de inmediato tras la última muestra), pero genera `OnSpeakerSegment` — una vez por
turno, en orden de tiempo de inicio — solo después del fin de flujo (end-of-stream). **Es adecuado para
archivos y flujos finitos.** Una fuente en vivo interminable nunca alcanza el momento en el que existe una
respuesta.

## Configuración básica del bloque

```csharp
var settings = new SpeakerDiarizationSettings(segmentationModelPath, embeddingModelPath)
{
    Provider = OnnxExecutionProvider.CPU,
};

var diarization = new SpeakerDiarizationBlock(settings);

// Se genera para cada turno, en orden, después de que el flujo termine.
diarization.OnSpeakerSegment += (sender, e) =>
{
    var turn = e.Segment;
    Console.WriteLine($"speaker_{turn.SpeakerId:D2}: {turn.Start:c} - {turn.End:c} (conf {turn.Confidence:F2})");
};
```

La misma línea de tiempo está disponible de una pieza mediante `GetTimeline()`. Compruebe antes
`IsTimelineComplete` — una línea de tiempo vacía significa "nunca llegamos a averiguar quién habló", no "nadie
habló":

```csharp
if (!diarization.IsTimelineComplete)
{
    // La ejecución nunca llegó al paso de agrupamiento: la canalización se detuvo antes del fin de flujo,
    // o el trabajador de análisis falló. La línea de tiempo está vacía, y ese vacío no es una respuesta.
    Console.WriteLine("Sin resultado de diarización — la grabación no se escuchó hasta el final.");
    return;
}

Console.WriteLine($"{diarization.SpeakerCount} hablante(s).");

if (diarization.UnattributedTurns > 0)
{
    // Habla que los modelos encontraron pero no pudieron convertir en huella de voz: esos turnos no llevan
    // identificador de hablante y nunca llegan a la línea de tiempo, aunque su audio SÍ se analizó.
    Console.WriteLine($"{diarization.UnattributedTurns} turno(s) no se pudieron atribuir a un hablante.");
}

foreach (var turn in diarization.GetTimeline())
{
    Console.WriteLine(turn); // speaker_00 [00:00:01.583 --> 00:00:03.405] (conf=0.98)
}
```

## Modelos — descarga en tiempo de ejecución

Se requieren dos modelos ONNX, ambos descargados en tiempo de ejecución (ninguno se incluye en los
paquetes NuGet del SDK). Ambos los exporta el proyecto
[sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx):

| Modelo | Función | Licencia | Fuente |
| --- | --- | --- | --- |
| pyannote `segmentation-3.0` | Segmentación de turnos de habla locales (ventana de 10 s, hasta 3 hablantes locales, actividad de conjunto potencia) | MIT | Extraiga `model.onnx` del recurso de versión [`sherpa-onnx-pyannote-segmentation-3-0.tar.bz2`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-segmentation-models/sherpa-onnx-pyannote-segmentation-3-0.tar.bz2). |
| WeSpeaker `voxceleb_resnet34_LM` (inglés, **predeterminado**) | Embedding de hablante (entra fbank de 80 bandas, sale huella de voz de 256 dimensiones) | Apache-2.0 | Recurso de versión [`wespeaker_en_voxceleb_resnet34_LM.onnx`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-recongition-models/wespeaker_en_voxceleb_resnet34_LM.onnx). |
| 3D-Speaker `eres2netv2_sv_zh-cn_16k-common` (**multilingüe**) | Embedding de hablante (entra fbank de 80 bandas, sale huella de voz de 192 dimensiones); separa mucho mejor a los hablantes en habla no inglesa | Apache-2.0 | Recurso de versión [`3dspeaker_speech_eres2netv2_sv_zh-cn_16k-common.onnx`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-recongition-models/3dspeaker_speech_eres2netv2_sv_zh-cn_16k-common.onnx). |

El modelo de segmentación es **independiente del idioma** — solo el modelo de embeddings debe coincidir
con su idioma. Los valores predeterminados `SegmentationModelUrl` / `EmbeddingModelUrl` de
`SpeakerDiarizationSettings` apuntan a los recursos de versión de pyannote y WeSpeaker en inglés. Son
informativos — el SDK nunca descarga nada por su cuenta; los archivos que realmente se cargan son
`SegmentationModelPath` y `EmbeddingModelPath`. Se puede sustituir cualquier modelo de embeddings
WeSpeaker o 3D-Speaker con fbank de 80 bandas cuyos pesos sean de uso comercial; el front-end de
características fbank se configura solo (escalado de muestras y normalización de características) a partir
de los metadatos ONNX del modelo, así que no hace falta ajuste manual al cambiarlo.

## Diarización multilingüe

El embedding WeSpeaker predeterminado está entrenado en inglés (VoxCeleb) e infradetecta hablantes en
otros idiomas. Para contenido no inglés o multilingüe, use el modelo de embeddings **3D-Speaker
ERes2NetV2** multilingüe (Apache-2.0): apunte `EmbeddingModelPath` a su archivo ONNX y establezca
`EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual`. El modelo de segmentación no cambia.

```csharp
var settings = new SpeakerDiarizationSettings(segmentationModelPath, eres2netv2ModelPath)
{
    EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual,

    // Reajusta el umbral para este embedding — véase más abajo. El valor por defecto (0.5) lo sobresegmenta.
    ClusterThreshold = 0.8f,
    Provider = OnnxExecutionProvider.CPU,
};

var diarization = new SpeakerDiarizationBlock(settings);
```

!!! warning "Reajusta `ClusterThreshold` al cambiar de modelo de embedding"

    Cada modelo de embedding tiene su propia geometría del coseno, y el `ClusterThreshold` por defecto de
    `0.5` es la distancia correcta para **WeSpeaker**. Las huellas de voz de ERes2NetV2 están más separadas
    entre sí, de modo que esa misma distancia divide a un único hablante en varios: en un clip de referencia
    con cuatro hablantes produce seis. Un valor en torno a **`0.8`** es el punto de partida para ERes2NetV2.
    Si conoces el número de hablantes, fijar `NumSpeakers` evita la cuestión por completo.

`EmbeddingModel` es una pista declarativa: selecciona la URL de descarga recomendada, mientras que el archivo
que realmente se carga es siempre `EmbeddingModelPath`. Como el front-end lee la configuración de
características de los metadatos ONNX del modelo, cambiar entre ambos modelos no requiere ningún otro cambio
de código.

Las URL de descarga también se exponen como constantes:
`SpeakerDiarizationSettings.WeSpeakerEnglishModelUrl` y
`SpeakerDiarizationSettings.ERes2NetV2MultilingualModelUrl`.

## Ajustes principales

`SpeakerDiarizationSettings(segmentationModelPath, embeddingModelPath)`. A diferencia de los ajustes de
IA de visión, este tipo **no** deriva de `OnnxInferenceSettings` — aquellos describen el preprocesado de
fotogramas de vídeo, que no se aplica a un par de modelos de audio.

Los valores predeterminados son los de la propia implementación de referencia y son un buen punto de partida
para contenido arbitrario.

| Propiedad | Predeterminado | Descripción |
| --- | --- | --- |
| `SegmentationModelPath` | — | Ruta absoluta al modelo ONNX pyannote `segmentation-3.0`. Obligatorio. |
| `EmbeddingModelPath` | — | Ruta absoluta al modelo ONNX de embeddings de hablante (por ejemplo WeSpeaker `voxceleb_resnet34_LM` o 3D-Speaker `eres2netv2`). Obligatorio. |
| `EmbeddingModel` | `WeSpeakerEnglish` | Para qué modelo de embeddings están configurados los ajustes (`WeSpeakerEnglish` o `ERes2NetV2Multilingual`). Una pista declarativa que elige la URL predeterminada; el archivo cargado es siempre `EmbeddingModelPath`, y la extracción de características se autoconfigura desde sus metadatos ONNX. |
| `SegmentationModelUrl` | versión pyannote de sherpa-onnx | URL de descarga informativa del modelo de segmentación. |
| `EmbeddingModelUrl` | versión WeSpeaker de sherpa-onnx | URL de descarga informativa del modelo de embeddings. |
| `NumSpeakers` | `-1` | El número **exacto** de hablantes, cuando lo conoce. Un valor positivo fija el resultado a ese número de hablantes y `ClusterThreshold` se ignora. `-1` significa "dedúcelo de las voces" — el ajuste correcto siempre que el número no sea seguro. |
| `ClusterThreshold` | `0.5` | La **DISTANCIA** del coseno a partir de la cual dos huellas de voz dejan de ser la misma persona. Fíjese en la dirección: esto es una distancia (0 = voces idénticas), **no** una similitud — un valor **menor** es más estricto y produce **más** hablantes. Se ignora cuando `NumSpeakers` es positivo. |
| `MinDurationOn` | `0.3` | El turno más corto, en segundos, que se informa. Los más cortos se descartan como ruido. |
| `MinDurationOff` | `0.5` | La pausa más larga, en segundos, que sigue contando como el mismo turno. Dos turnos de un mismo hablante más próximos que esto se unen, de modo que un hablante que hace una pausa para respirar sigue siendo un solo turno. |
| `Provider` | `Auto` | Proveedor de ejecución para ambas sesiones ONNX. `Auto` usa el proveedor más rápido presente en el binario nativo de ONNX Runtime cargado y recurre a la CPU. |
| `DeviceId` | `0` | Identificador de dispositivo de hardware cuando se selecciona un proveedor de GPU. |

## Turnos de hablante

Cada `SpeakerSegment` (de `SpeakerSegmentEventArgs.Segment` y `GetTimeline()`):

| Propiedad | Descripción |
| --- | --- |
| `SpeakerId` | Identificador de hablante estable y de base cero para esta ejecución. |
| `Start` / `End` | `TimeSpan` sobre la línea de tiempo del medio. |
| `Duration` | `End - Start`. |
| `Confidence` | Con qué fuerza coincidieron las ventanas de análisis solapadas en este hablante para este turno (0..1). `1` significa que todas las ventanas que vieron ese instante nombraron al mismo hablante. |

Los identificadores de hablante son estables **dentro de una ejecución**, no entre ejecuciones
distintas de la canalización (en esta versión no hay reidentificación de hablantes entre ejecuciones).

## Transcripción diarizada — "quién dijo qué"

`DiarizedTranscriptBuilder` combina una transcripción de reconocimiento de voz con la línea de tiempo de
diarización, etiquetando cada segmento de transcripción con el hablante que más se solapa con él (los
empates van al identificador de hablante menor). Es procesamiento de datos puro — sin dependencia de
modelo ni de medios — por lo que empareja un [`SpeechToTextBlock`](speech-to-text.md) con un
`SpeakerDiarizationBlock`. Constrúyala después del fin de flujo, cuando la línea de tiempo de diarización ya
existe.

```csharp
using VisioForge.Core.Types.X.AI;

// transcript: tuplas (start, end, text) de SpeechToTextBlock.OnSpeechRecognized
// timeline: diarization.GetTimeline()
var labeled = DiarizedTranscriptBuilder.Build(transcript, timeline);
foreach (DiarizedTranscriptSegment seg in labeled)
{
    // "[00:00:01.000 --> 00:00:03.000] speaker_00: A pencil with black lead writes best."
    Console.WriteLine(seg);
}
```

Cada `DiarizedTranscriptSegment` lleva `Start`, `End`, `Text` y el `SpeakerId` atribuido (`-1` cuando
ningún turno de diarización se solapa con el segmento — por ejemplo, un silencio).

## Canalización manual de Media Blocks

Coloque `SpeakerDiarizationBlock` en la cadena de audio antes del renderizador o la salida de audio:

```csharp
var diarization = new SpeakerDiarizationBlock(settings);
diarization.OnSpeakerSegment += Diarization_OnSpeakerSegment;

pipeline.Connect(audioSource.Output, diarization.Input);
pipeline.Connect(diarization.Output, audioRenderer.Input);
```

## Diarización de archivos con MediaPlayerCoreX

Para la reproducción, `Audio_Play` debe ser `true` para que se construya la cadena de audio:

```csharp
player.Audio_Play = true;

var diarization = new SpeakerDiarizationBlock(settings);
diarization.OnSpeakerSegment += Diarization_OnSpeakerSegment;

player.Audio_Processing_AddBlock(diarization); // antes de OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Deje que el archivo se reproduzca hasta el final: detener la reproducción antes de tiempo se salta el paso de
agrupamiento y deja la línea de tiempo vacía. La diarización solo lee el audio, así que para indexar un archivo
tan rápido como permita el hardware en lugar de en tiempo real, asigne al reproductor un sumidero de audio no
sincronizado — `player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };` —
para que el audio no lo acompase el reloj. (`Video_Renderer_IsSync = false` solo desacompasa un renderizador de
vídeo, por lo que no tiene efecto en un archivo solo de audio.)

Consulte [Uso de bloques de IA con VideoCaptureCoreX y MediaPlayerCoreX](x-engines.md) para la API
`Audio_Processing_*` completa y las reglas de ciclo de vida. El motor es dueño del bloque conectado tras
el inicio y lo libera cuando la sesión se detiene — cree un bloque nuevo para la siguiente sesión.

## Cómo funciona

Entender estas tres etapas explica todos los ajustes anteriores, y por qué el resultado no puede llegar antes.

1. **Segmentación, a medida que fluye el audio.** El audio se remuestrea a mono 16 kHz y se corta en ventanas
   de 10 segundos que avanzan de 1 segundo en 1 segundo — de modo que las ventanas se solapan un 90 % y cada
   instante de audio lo ven unas diez de ellas. El modelo pyannote etiqueta, para cada uno de sus ~59
   fotogramas de salida por segundo, cuáles de hasta tres hablantes *locales a la ventana* están hablando.
2. **Huellas de voz, a medida que fluye el audio.** Para cada ventana y cada hablante activo en ella, se
   recoge el audio de ese hablante — **menos todos los fotogramas en los que también habla otra persona** — y
   se convierte en una huella de voz. El habla solapada se excluye deliberadamente: una huella de voz tomada
   de dos voces a la vez queda entre ambas y se parece a las dos, lo que suelda dos hablantes en uno. Los
   turnos con menos de ~0,17 s de habla limpia se omiten por la misma razón.
3. **Hablantes y línea de tiempo, al final del flujo.** Todas las huellas de voz de la grabación se agrupan
   juntas (aglomerativo, enlace completo, distancia del coseno — `ClusterThreshold` / `NumSpeakers`). Los
   números de hablante locales a cada ventana se reescriben como números globales, todas las ventanas
   solapadas se disponen sobre una misma rejilla de fotogramas y **cada fotograma se decide por votación
   entre las ~10 ventanas que lo cubren**: cuántas personas hablan es la media que informan esas ventanas, y
   quiénes son es lo más votado. Una ventana que etiqueta mal un instante queda en minoría frente a sus
   vecinas. Por último, los fotogramas se convierten en turnos, las pausas cortas se puentean
   (`MinDurationOff`) y los fragmentos se descartan (`MinDurationOn`).

## Subprocesos, rendimiento y ciclo de vida

El análisis se ejecuta en un **trabajador en segundo plano dedicado**, no en el subproceso de streaming:
la interceptación remuestrea el audio a mono 16 kHz y lo empuja a una **cola no acotada**, y el trabajador lo
consume en ventanas solapadas. Como el análisis está fuera del subproceso de streaming, el audio nunca se
acompasa a los modelos: una fuente más rápida que ellos (un archivo descodificado a toda velocidad, o una
fuente en vivo con un proveedor de ejecución lento) simplemente hace que el trabajador se quede atrás.

**El trabajo pendiente nunca se descarta.** En su lugar la cola crece — en mono 16 kHz cuesta 64 KB por
segundo almacenado — y se vacía en cuanto el trabajador se pone al día. Si la máquina es permanentemente más
lenta que su fuente, el retraso crece sin parar, y esa es la señal honesta: la respuesta es hardware más
rápido o un proveedor de ejecución en GPU, nunca analizar solo una fracción del audio. Al final del flujo, la
canalización espera a que el trabajador vacíe el trabajo pendiente y publique la línea de tiempo antes de
desmontarse.

`OnSpeakerSegment` se genera en el subproceso del trabajador — nunca toque la interfaz de usuario
directamente desde el controlador; delegue al dispatcher de la interfaz o al hilo principal.

La memoria escala con la grabación: las huellas de voz se conservan hasta el final (son la entrada del
agrupamiento), y el agrupamiento construye una matriz de distancias por pares sobre ellas.

## Integridad del resultado

| Miembro | Descripción |
| --- | --- |
| `IsTimelineComplete` | `true` cuando la ejecución produjo realmente una respuesta: el flujo alcanzó el fin de flujo, el trabajador se vació y los hablantes se agruparon y se publicaron. **Compruébelo antes de leer `GetTimeline()`** — cuando es `false` la línea de tiempo está VACÍA, y ese vacío significa "nunca llegamos a averiguar quién habló", no "nadie habló". |
| `SpeakerCount` | Cuántos hablantes distintos se encontraron. `0` hasta que se publica la línea de tiempo. |
| `UnattributedTurns` | Turnos de habla que el modelo de segmentación **encontró** pero que el modelo de embeddings no pudo convertir en huella de voz. Su audio *sí* se analizó, pero no llevan identificador de hablante y están ausentes de la línea de tiempo. |
| `ActiveProvider` | El proveedor de ejecución que realmente utilizaron las sesiones ONNX. Sigue siendo válido tras la ejecución, así que puede registrarse al final. |

**Una línea de tiempo es plenamente fiable cuando `IsTimelineComplete` es `true` Y `UnattributedTurns` es `0`.**

## Casos de uso

- **Transcripción de reuniones y entrevistas** — etiquete una transcripción diarizada con turnos por hablante.
- **Analítica de centros de llamadas** — separe el habla del agente y del cliente para métricas de tiempo de conversación y de turnos.
- **Indexación de medios** — indexe vídeo/audio grabado por hablante para búsqueda y navegación.
- **Herramientas de pódcast y radiodifusión** — genere notas del programa con etiquetas de hablante mediante reconocimiento de voz.

## Solución de problemas

| Síntoma | Causa probable | Solución |
| --- | --- | --- |
| Nunca se generan turnos, e `IsTimelineComplete` es `false` | El flujo nunca terminó — la canalización se detuvo antes de tiempo, o la fuente es un flujo en vivo interminable | Deje que la fuente alcance el fin de flujo. La diarización necesita la grabación completa; no hay ninguna respuesta parcial que dar. |
| No se generan turnos aunque el archivo se reprodujo hasta el final | Una ruta de modelo no es válida, o la cadena de audio no se construye | Confirme que ambos archivos ONNX existen en sus rutas; para la reproducción confirme `Audio_Play = true`. |
| Se detectan demasiados hablantes | `ClusterThreshold` demasiado pequeño para este audio/modelo | **Suba** `ClusterThreshold` (es una distancia — un valor mayor fusiona más), o fije `NumSpeakers` al número conocido. |
| Los hablantes se fusionan en uno | `ClusterThreshold` demasiado grande | **Baje** `ClusterThreshold` (una distancia menor divide con más facilidad), o fije `NumSpeakers`. |
| El análisis se retrasa respecto a una fuente de archivo rápida, y la memoria crece | Las ventanas solapadas exigen mucho cómputo, así que el trabajador se queda atrás y el trabajo pendiente (no acotado) crece — no se pierde audio, pero se almacena | Use `Provider = OnnxExecutionProvider.CUDA` / `DirectML` en una GPU. Cada segundo almacenado cuesta 64 KB. |
| Falta habla aunque `IsTimelineComplete` sea `true` | El modelo de embeddings no pudo obtener la huella de voz de algunos turnos detectados, así que se quedaron sin identificador de hablante | Compruebe `UnattributedTurns`; busque en el registro el error de `SpeakerEmbeddingEngine` para conocer la causa (normalmente falta de memoria en la GPU o un cambio de proveedor fallido). |

## Preguntas frecuentes

### ¿Por qué no obtengo ningún turno de hablante hasta que el archivo termina?

Porque hasta entonces no existe tal cosa como un "hablante 2" del que informar. El modelo de segmentación
separa las voces solo dentro de una única ventana de 10 segundos y las numera localmente; ligar una voz del
minuto 1 con esa misma voz del minuto 40 exige agrupar de una sola vez todas las huellas de voz de la
grabación. El bloque hace todo el trabajo que puede a medida que fluye el audio, y publica en el momento en
que entra la última muestra.

### ¿Puedo diarizar un flujo en vivo interminable?

No. Un flujo en vivo no tiene fin de flujo, así que el paso de agrupamiento nunca se ejecuta. Diarice
grabaciones finitas — o segmente un flujo en vivo en fragmentos finitos y diarice cada uno (los
identificadores de hablante no son comparables entre fragmentos).

### ¿SpeakerDiarizationBlock requiere conexión a internet?

No — la diarización se ejecuta totalmente en el dispositivo mediante ONNX Runtime. Su aplicación
descarga los dos archivos de modelo una vez (o los incluye); no se llama nada por solicitud a través de
la red.

### ¿Puedo combinarlo con reconocimiento de voz para obtener una transcripción etiquetada por hablante?

Sí — ejecute un [`SpeechToTextBlock`](speech-to-text.md) y un `SpeakerDiarizationBlock` sobre el mismo
audio, y luego combine la transcripción con `diarization.GetTimeline()` mediante
`DiarizedTranscriptBuilder.Build` después del fin de flujo.

### ¿Son los mismos los identificadores de hablante en dos ejecuciones distintas?

No — los identificadores son estables solo dentro de una única ejecución. Esta versión no reidentifica a
un hablante entre ejecuciones.

### ¿Con qué idiomas funciona?

El modelo de segmentación es independiente del idioma, así que funciona con cualquier idioma sin más. El
modelo de embeddings WeSpeaker predeterminado está entrenado en inglés (VoxCeleb) e infradetecta
hablantes en otros idiomas; para audio no inglés o multilingüe, cambie `EmbeddingModelPath` al modelo
multilingüe 3D-Speaker ERes2NetV2 (Apache-2.0) y establezca `EmbeddingModel =
SpeakerEmbeddingModel.ERes2NetV2Multilingual` (véase [Diarización multilingüe](#diarizacion-multilingue)).
El front-end adapta automáticamente su extracción de características al modelo elegido a partir de los
metadatos ONNX.
