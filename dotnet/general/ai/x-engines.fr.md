---
title: Blocs IA avec VideoCaptureCoreX et MediaPlayerCoreX
description: Insérez des blocs IA (OCR, détection d'objets, reconnaissance faciale, ANPR, arrière-plan, transcription) dans VideoCaptureCoreX/MediaPlayerCoreX.
sidebar_label: Moteurs X
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

# Blocs IA avec VideoCaptureCoreX et MediaPlayerCoreX

`VideoCaptureCoreX` et `MediaPlayerCoreX` peuvent héberger des blocs multimédias fournis par
l'utilisateur au sein de leurs pipelines de haut niveau. Utilisez cette approche lorsque vous
souhaitez profiter du confort des moteurs X et n'avez besoin que d'ajouter du traitement IA à la
chaîne vidéo ou audio — sans avoir à construire manuellement une topologie `MediaBlocksPipeline`.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.AI;
```

## API publique

Les deux moteurs exposent la même API de blocs de traitement :

| API | Objectif |
| --- | --- |
| `Video_Processing_AddBlock(IVideoProcessingBlock block)` | Ajoute un bloc de traitement vidéo pour la prochaine session. |
| `Video_Processing_RemoveBlock(IVideoProcessingBlock block)` | Supprime un bloc vidéo enregistré avant le démarrage de la session. |
| `Video_Processing_Clear()` | Efface les blocs vidéo enregistrés avant le démarrage de la session. |
| `Video_Processing_Blocks` | Instantané des blocs vidéo enregistrés. |
| `Audio_Processing_AddBlock(IAudioProcessingBlock block)` | Ajoute un bloc de traitement audio pour la prochaine session. |
| `Audio_Processing_RemoveBlock(IAudioProcessingBlock block)` | Supprime un bloc audio enregistré avant le démarrage de la session. |
| `Audio_Processing_Clear()` | Efface les blocs audio enregistrés avant le démarrage de la session. |
| `Audio_Processing_Blocks` | Instantané des blocs audio enregistrés. |
| `Audio_OutputBlock` | Remplace le puits audio par défaut par un `MediaBlock` personnalisé, généralement un `NullRendererBlock` non synchronisé pour la transcription vocale. |

Chaque bloc IA vidéo (`OcrBlock`, `YOLOObjectDetectorBlock`, `ObjectAnalyticsBlock`,
`FaceRecognitionBlock`, `LicensePlateRecognizerBlock`, `BackgroundRemovalBlock`,
`OnnxInferenceBlock`) implémente `IVideoProcessingBlock`. `SpeechToTextBlock` implémente
`IAudioProcessingBlock`.

## Règles de cycle de vie

Enregistrez les blocs avant que le moteur ne construise son pipeline : avant `StartAsync` sur
`VideoCaptureCoreX`, avant `OpenAsync`/`PlayAsync` sur `MediaPlayerCoreX`. Ajouter, supprimer ou
effacer des blocs de traitement après le démarrage de la construction du pipeline est ignoré et
consigné comme un `Warning` :

- `VideoCaptureCoreX` : *« Cannot add a processing block while capture is running. Add it before
  Start(). »*
- `MediaPlayerCoreX` : *« Cannot add a processing block while playback is running. Add it before
  Play(). »*

(Le texte du journal utilise les noms classiques `Start()`/`Play()` bien que l'API publique que
vous appelez soit `StartAsync()`/`PlayAsync()` — les chaînes d'avertissement datent d'avant les
wrappers asynchrones et sont citées ici verbatim afin que vous puissiez les rechercher par grep.)

Si l'un de ces messages apparaît dans le journal, l'appel au bloc a eu lieu après le démarrage (ou
le début du démarrage) de la session — déplacez-le plus tôt.

Une fois la session démarrée, le moteur devient propriétaire des blocs de traitement câblés et les
libère à l'arrêt de la session. Créez une nouvelle instance de bloc avant chaque nouvelle session ;
les événements de bloc sont déclenchés sur les threads du pipeline ou des workers de bloc — gardez
les gestionnaires non bloquants et transférez les modifications d'interface utilisateur vers le
dispatcher d'interface ou le thread principal. Si une chaîne est inactive, ses blocs enregistrés ne
s'exécutent pas — par exemple, un bloc de traitement audio nécessite une source audio et une chaîne
audio construite.

## Ordre d'insertion dans le pipeline

- **`VideoCaptureCoreX`** : les blocs IA vidéo sont insérés après les effets vidéo et avant le
  capteur d'échantillons, la superposition, le tee, le moteur de rendu et les sorties — de sorte que
  les superpositions dessinées par le bloc IA atteignent à la fois l'aperçu et chaque sortie
  d'enregistrement/diffusion.
- **`MediaPlayerCoreX`** : les blocs IA vidéo sont insérés après les effets vidéo et avant le
  capteur d'échantillons, le moteur de rendu et les sorties vidéo personnalisées.

## IA vidéo avec VideoCaptureCoreX

```csharp
var detector = new YOLOObjectDetectorBlock(new YoloDetectorSettings(modelPath)
{
    Model = ObjectDetectorModel.YOLOv8,
    DrawDetections = true,
});

detector.OnObjectsDetected += (sender, e) =>
{
    // Déclenché depuis le thread de streaming.
    Console.WriteLine($"Detected {e.Objects.Length} objects.");
};

core.Video_Processing_AddBlock(detector); // avant StartAsync
await core.StartAsync();
```

Le même schéma s'applique à `OcrBlock`, `ObjectAnalyticsBlock`, `FaceRecognitionBlock`,
`LicensePlateRecognizerBlock`, `BackgroundRemovalBlock` et `OnnxInferenceBlock` — consultez la page
dédiée à chaque bloc pour ses paramètres et le contenu de ses événements.

## Transcription vocale avec VideoCaptureCoreX

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var sttSettings = new SpeechToTextSettings(whisperModelPath)
{
    EnableVad = false, // ou conservez EnableVad = true et définissez sttSettings.Vad.ModelPath = sileroVadModelPath;
};
var stt = new SpeechToTextBlock(sttSettings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

core.Audio_Processing_AddBlock(stt); // avant StartAsync
await core.StartAsync();
```

`EnableVad` est à `true` par défaut, ce qui nécessite `SileroVadSettings.ModelPath` — la
construction du pipeline échoue si le VAD est activé sans chemin de modèle défini. Désactivez le
VAD ou définissez `sttSettings.Vad.ModelPath` avant d'enregistrer le bloc.

Pour la capture, une source audio est requise. `Audio_OutputBlock` peut activer et terminer la
chaîne audio sans `Audio_Play`, `Audio_Record`, la surveillance par haut-parleur, ni une sortie
d'enregistrement.

## IA vidéo avec MediaPlayerCoreX

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

player.Video_Processing_AddBlock(ocr); // avant OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

## Transcription vocale avec MediaPlayerCoreX

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var sttSettings = new SpeechToTextSettings(whisperModelPath)
{
    EnableVad = false, // ou conservez EnableVad = true et définissez sttSettings.Vad.ModelPath = sileroVadModelPath;
};
var stt = new SpeechToTextBlock(sttSettings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

player.Audio_Processing_AddBlock(stt); // avant OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Pour la lecture, `Audio_Play` doit être `true` pour que la chaîne audio soit construite. Remplacer
le puits par un moteur de rendu nul non synchronisé évite la sortie sur haut-parleur et ne cale pas
la transcription sur le rythme de la lecture en temps réel.

## Dépannage

| Symptôme | Cause probable | Correction |
| --- | --- | --- |
| Les événements du bloc ne se déclenchent jamais, aucun avertissement consigné | Bloc enregistré après le démarrage de la session | Enregistrez-le avec `Video_Processing_AddBlock`/`Audio_Processing_AddBlock` avant `StartAsync` (`VideoCaptureCoreX`) ou avant `OpenAsync`/`PlayAsync` (`MediaPlayerCoreX`). |
| Le journal affiche *« Cannot add a processing block while capture/playback is running... »* | Même cause — l'appel a eu lieu après le début de la construction du pipeline | Déplacez l'appel `Video_Processing_AddBlock`/`Audio_Processing_AddBlock` plus tôt dans votre séquence de démarrage. |
| `SpeechToTextBlock` ne reçoit jamais d'audio sur `VideoCaptureCoreX` | Aucun `Audio_Source` configuré | Un bloc de traitement audio nécessite une chaîne audio construite — définissez `Audio_Source` (et, si vous ne voulez pas de sortie sur haut-parleur, `Audio_OutputBlock` vers un `NullRendererBlock` non synchronisé). |
| `SpeechToTextBlock` ne reçoit jamais d'audio sur `MediaPlayerCoreX` | `Audio_Play` laissé à sa valeur par défaut | Définissez `player.Audio_Play = true` avant `OpenAsync` ; la chaîne audio n'est sinon pas construite, quels que soient les blocs audio enregistrés. |
| La superposition du bloc IA n'apparaît pas dans la sortie enregistrée/diffusée, seulement dans l'aperçu | Bloc enregistré sur la mauvaise chaîne, ou inséré après le point où le moteur construit les sorties | Les blocs IA vidéo sont insérés avant le capteur d'échantillons/la superposition/le tee/le moteur de rendu/les sorties — voir [Ordre d'insertion dans le pipeline](#ordre-dinsertion-dans-le-pipeline) — il s'agit donc généralement d'un problème de timing d'enregistrement, pas de topologie. |
| Une instance de bloc réutilisée pour un second `StartAsync`/`PlayAsync` lève une exception ou se comporte étrangement | Le moteur a déjà libéré l'instance de bloc de la session précédente | Créez une nouvelle instance de bloc (et réabonnez-vous à ses événements) pour chaque nouvelle session de capture/lecture. |

## Foire aux questions

### Dois-je reconstruire un MediaBlocksPipeline manuel pour utiliser des blocs IA avec VideoCaptureCoreX/MediaPlayerCoreX ?

Non — c'est tout l'intérêt de cette intégration. `Video_Processing_AddBlock`/
`Audio_Processing_AddBlock` insèrent les mêmes instances de blocs IA dans le pipeline interne
existant du moteur ; vous ne construisez ni ne gérez vous-même de `MediaBlocksPipeline`.

### Puis-je ajouter ou supprimer des blocs IA pendant que la capture/lecture est en cours ?

Non — les ajouts/suppressions/effacements sont ignorés (et consignés comme un avertissement) une
fois la construction du pipeline démarrée. Enregistrez tout ce dont vous avez besoin avant
`StartAsync` / `OpenAsync`+`PlayAsync`, et démarrez une nouvelle session avec de nouvelles
instances de bloc si l'ensemble de blocs doit changer.

### Les blocs IA vidéo ralentissent-ils les sorties d'enregistrement ou de diffusion ?

Ils ajoutent leur propre coût d'inférence à la chaîne vidéo, puisqu'ils s'exécutent en ligne avant
le moteur de rendu et les sorties. Utilisez `FramesToSkip` (disponible dans les paramètres de
chaque bloc IA vidéo) et, lorsque c'est possible, un fournisseur d'exécution GPU pour maîtriser ce
coût.

### Puis-je combiner plusieurs blocs IA — par exemple la détection d'objets et la transcription vocale — dans une même session ?

Oui — enregistrez un ou plusieurs `IVideoProcessingBlock` avec `Video_Processing_AddBlock` et,
séparément, un `IAudioProcessingBlock` (comme `SpeechToTextBlock`) avec
`Audio_Processing_AddBlock`, le tout avant le démarrage de la session. Les chaînes de traitement
vidéo et audio sont indépendantes.

## Démos

Chaque bloc couvert sur cette page dispose également de démos de pipeline Media Blocks dédiées —
consultez la section **Démos** de la page propre à chaque bloc ([OCR](ocr.md#demos),
[Détection d'objets](object-detection.md#demos),
[Analytique d'objets](object-analytics.md#demos),
[Reconnaissance faciale](face-recognition.md#demos),
[Reconnaissance de plaques d'immatriculation](license-plate-recognition.md#demos),
[Suppression d'arrière-plan](background-removal.md#demo),
[Transcription vocale](speech-to-text.md#demos)).

L'ensemble de démos du SDK propose en outre 16 démos construites directement sur
`VideoCaptureCoreX` / `MediaPlayerCoreX` avec l'API présentée sur cette page — Détection d'objets,
OCR, Reconnaissance faciale et Sous-titres en direct, chacune pour WPF et MAUI, sur les moteurs de
capture et de lecture (`Capture Object Detection X`, `Capture Object Detection X WPF`,
`Capture OCR X`, `Capture OCR X WPF`, `Capture Face Recognition X`, `Capture Face Recognition X WPF`,
`Capture Live Subtitles X`, `Capture Live Subtitles X WPF`, `Player Object Detection X`,
`Player Object Detection X WPF`, `Player OCR X`, `Player OCR X WPF`, `Player Face Recognition X`,
`Player Face Recognition X WPF`, `Player Live Subtitles X`, `Player Live Subtitles X WPF`). Les
liens GitHub seront ajoutés ici une fois ces démos publiées dans le dépôt d'exemples public.
