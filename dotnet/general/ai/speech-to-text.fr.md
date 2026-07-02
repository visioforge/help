---
title: SDK reconnaissance vocale .NET — SpeechToTextBlock
description: SDK de reconnaissance vocale .NET — transcrivez de l'audio en direct ou en fichier en local avec Whisper ASR et Silero VAD, plus sous-titres SRT/VTT.
sidebar_label: Reconnaissance vocale
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

# Reconnaissance vocale et sous-titres en direct — SpeechToTextBlock

`SpeechToTextBlock` est un bloc média audio uniquement issu de `VisioForge.DotNet.Core.AI.Whisper`. Il
capte le flux audio, segmente la parole avec Silero VAD, la transcrit avec Whisper (Whisper.net /
GGML), et déclenche `OnSpeechRecognized`. L'audio traverse le bloc sans modification. Le bloc
implémente `IAudioProcessingBlock`, il peut donc être inséré dans un pipeline manuel ou enregistré
directement sur `VideoCaptureCoreX`/`MediaPlayerCoreX`.

```csharp
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.Types.X.AI;
```

## Configuration de base du bloc

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

`SpeechToTextTask.Transcribe` conserve la langue source. `SpeechToTextTask.Translate` traduit la
parole source prise en charge vers un texte en anglais.

## Paramètres clés

`SpeechToTextSettings(whisperModelPath)`. Contrairement aux paramètres IA de vision, ce type ne
dérive **pas** de `OnnxInferenceSettings` — Whisper s'exécute via Whisper.net (whisper.cpp / GGML), pas
via ONNX Runtime, donc les réglages ONNX de taille d'entrée/normalisation ne s'appliquent pas.

| Propriété | Par défaut | Description |
| --- | --- | --- |
| `WhisperModelPath` | — | Chemin absolu vers le fichier de modèle GGML de Whisper (`ggml-*.bin`). Obligatoire. |
| `ModelSize` | `WhisperModelSize.Base` | Étiquette informative pour la variante de modèle située à `WhisperModelPath` (voir ci-dessous). |
| `Language` | `"auto"` | Code ISO 639-1 (`"en"`, `"es"`, `"fr"`, ...), ou `"auto"` pour laisser Whisper le détecter. |
| `Task` | `Transcribe` | `Transcribe` (langue source) ou `Translate` (vers l'anglais). |
| `Provider` | `Auto` | Seuls `CPU` et `CUDA` sont pertinents pour le backend GGML (pas de DirectML) ; `Auto` choisit CUDA s'il est présent, sinon CPU. |
| `DeviceId` | `0` | Identifiant du périphérique matériel lorsqu'un fournisseur GPU est sélectionné. |
| `Threads` | `0` | Threads CPU utilisés par Whisper. `0` laisse Whisper.net choisir en fonction du nombre de processeurs disponibles. |
| `EnableVad` | `true` | Segmente la parole avec Silero VAD avant la transcription. Lorsque `false`, l'audio est transcrit par fenêtres fixes, ce qui a tendance à générer du texte halluciné pendant les silences. |
| `Vad` | nouveau `SileroVadSettings` | Paramètres VAD utilisés lorsque `EnableVad` vaut `true`. |
| `FixedWindowSeconds` | `5` | Durée de la fenêtre de transcription fixe lorsque `EnableVad` vaut `false`. Bornée entre 1 et 30 s. |
| `EmitInterim` | `false` | Réservé pour une future capacité d'hypothèses intermédiaires ; n'a actuellement aucun effet — seuls les segments finaux sont émis. |
| `OutputSrtPath` | `null` | Chemin annexe `.srt` optionnel que le bloc écrit à mesure que les segments finaux sont reconnus. |
| `OutputVttPath` | `null` | Chemin annexe `.vtt` (WebVTT) optionnel que le bloc écrit à mesure que les segments finaux sont reconnus. |

## Paramètres VAD

Lorsque `EnableVad` vaut `true`, `SpeechToTextSettings.Vad` contrôle la segmentation de la parole par
Silero. Silero VAD est un modèle ONNX minuscule (~2 Mo, MIT) qui classe de courtes fenêtres audio
comme parole ou non-parole, utilisé comme préfiltre temps réel afin que le modèle Whisper (bien plus
lourd) ne s'exécute que sur la parole réelle.

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

| Propriété | Par défaut | Description |
| --- | --- | --- |
| `ModelPath` | — | Chemin absolu vers `silero_vad.onnx`. |
| `SpeechThreshold` | `0.5` | Seuil de probabilité de parole (0..1). Augmentez-le dans les environnements bruyants pour réduire les faux déclenchements. |
| `MinSilenceMs` | `100` | Silence final minimal, en ms, qui termine un segment de parole. |
| `MinSpeechMs` | `250` | Durée minimale d'une séquence de parole, en ms, pour être émise (élimine les blips parasites). |
| `SpeechPadMs` | `30` | Marge d'attaque, en ms, ajoutée au début de chaque segment détecté. |
| `MaxSpeechMs` | `15000` | Longueur maximale de segment, en ms, avant que le segmenteur ne coupe de force une séquence en cours. |
| `Provider` | `CPU` | Fournisseur d'exécution pour la session VAD — le modèle est minuscule (~1 ms/fenêtre sur CPU), donc le GPU ajoute de la latence sans bénéfice. |
| `DeviceId` | `0` | Identifiant du périphérique matériel lorsqu'un fournisseur GPU est sélectionné. |

Les poids GGML de Whisper et le modèle Silero VAD sont téléchargés à l'exécution ; aucun des deux
n'est fourni dans les paquets NuGet du SDK.

### Tailles de modèle Whisper

`WhisperModelSize` est informatif — il nomme des fichiers de poids GGML de Whisper bien connus afin
qu'une application puisse en choisir un à télécharger. Le fichier réellement chargé est toujours
`SpeechToTextSettings.WhisperModelPath`.

| Valeur | Taille approx. | Remarques |
| --- | --- | --- |
| `Tiny` | ~75 Mo | Le plus rapide, précision la plus faible. Adapté à la transcription CPU en temps réel. |
| `Base` (par défaut) | ~142 Mo | Un bon choix par défaut pour le temps réel sur CPU. |
| `Small` | ~466 Mo | Nettement plus précis ; temps réel avec un GPU ou un CPU rapide. |
| `Medium` | ~1,5 Go | Haute précision ; nécessite généralement un GPU pour le temps réel. |
| `LargeV3` | ~3 Go | Précision maximale ; GPU fortement recommandé. |
| `LargeV3Turbo` | ~1,6 Go | Précision proche des modèles volumineux pour une fraction du coût. |
| `TinyQuantized` | ~31 Mo | `Tiny`, quantifié Q5_1. |
| `BaseQuantized` | ~57 Mo | `Base`, quantifié Q5_1. |
| `SmallQuantized` | ~181 Mo | `Small`, quantifié Q5_1. |
| `MediumQuantized` | ~514 Mo | `Medium`, quantifié Q5_0. |
| `LargeV3TurboQuantized` | ~547 Mo | `LargeV3Turbo`, quantifié Q5_0. Compromis précision/vitesse quantifié recommandé. |

Les variantes anglais uniquement (`*.en`) ne sont pas énumérées ; fournissez leur chemin directement.
Les poids sont sous licence MIT.

## Segments reconnus

Chaque `SpeechSegment` dans `SpeechRecognizedEventArgs.Segments` :

| Propriété | Description |
| --- | --- |
| `Text` | Le texte reconnu pour le segment. |
| `StartTime` / `EndTime` | `TimeSpan`, relatif au début du flux — prêt à l'emploi pour SRT/VTT ou une planification à l'écran. |
| `IsFinal` | Réservé pour distinguer les hypothèses intermédiaires des hypothèses finales une fois les résultats intermédiaires disponibles ; les segments sont actuellement toujours finaux (`true`). |
| `Language` | Code ISO 639-1 détecté/utilisé, ou `null` si inconnu. |
| `Confidence` | Confiance moyenne des tokens (0..1), ou `0` lorsque le modèle ne rapporte pas de probabilités de tokens. |

## Assistants de sous-titrage

### SubtitleWriter — fichiers SRT/VTT

`SubtitleWriter` écrit les segments de parole reconnus dans un fichier annexe SubRip (`.srt`) ou
WebVTT (`.vtt`), en ajoutant un repère (cue) par segment final. Il est thread-safe ; les segments
intermédiaires (non finaux) sont ignorés.

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

Si vous n'avez besoin que de fichiers, il est plus simple de définir `OutputSrtPath` et/ou
`OutputVttPath` sur `SpeechToTextSettings` — le bloc crée et pilote en interne sa/ses propre(s)
instance(s) de `SubtitleWriter` à mesure que les segments finaux sont reconnus, et les libère pour
vous.

### SubtitleRenderer — sous-titres à l'écran

`SubtitleRenderer` pilote une seule superposition de texte sur un `OverlayManagerBlock` : il affiche
le dernier sous-titre et le masque automatiquement après la durée d'affichage du segment.

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

// ... plus tard, lors de la mise hors service :
subtitleRenderer.Dispose(); // supprime la superposition et arrête le minuteur de masquage automatique
```

Connectez `SubtitleRenderer.OnSpeechRecognized` directement comme gestionnaire d'événements du bloc.
Il borne le temps d'affichage à l'écran dans `[MinDisplay, MaxDisplay]` en fonction de la durée du
segment et appelle `OverlayManagerBlock.Video_Overlay_Update` quel que soit le thread qui
l'invoque — orchestrez l'appel via le dispatcher si votre framework d'interface utilisateur l'exige.
Valeurs par défaut de `SubtitleStyle` : `FontName = "Arial"`, `FontSize = 32`, `Color = White`,
`X = 50`, `Y = 50`, `MinDisplay = 1.5 s`, `MaxDisplay = 6 s`.

!!! note "Pas encore de démo livrée"
    `SubtitleRenderer` existe dans le SDK et est documenté à partir de son code source, mais aucune
    démo groupée ne l'utilise actuellement — les démos Live Subtitles X mettent à jour directement une
    étiquette d'interface utilisateur depuis `OnSpeechRecognized`. Utilisez `SubtitleWriter` ou
    `OutputSrtPath`/`OutputVttPath` si vous avez seulement besoin d'un fichier de sous-titres.

## Pipeline Media Blocks manuel

Placez `SpeechToTextBlock` dans la chaîne audio avant le moteur de rendu ou la sortie audio :

```csharp
var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

pipeline.Connect(audioSource.Output, stt.Input);
pipeline.Connect(stt.Output, audioRenderer.Input);
```

## Transcription en direct du microphone avec VideoCaptureCoreX

Pour la capture, une source audio est requise. Si vous souhaitez une analyse sans écoute ni
enregistrement audio, terminez la chaîne audio par un moteur de rendu nul non synchronisé :

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

core.Audio_Processing_AddBlock(stt); // avant StartAsync
await core.StartAsync();
```

`Audio_OutputBlock` construit et termine la chaîne audio sans activer la lecture haut-parleur ni
l'enregistrement de fichier. Le moteur possède le bloc de sortie assigné après le démarrage.

## Transcription de fichier avec MediaPlayerCoreX

Pour la lecture, `Audio_Play` doit valoir `true` pour que la chaîne audio soit construite. Un moteur
de rendu nul non synchronisé permet à la source de s'exécuter sans sortie haut-parleur en temps réel :

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

player.Audio_Processing_AddBlock(stt); // avant OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Consultez [Utiliser les blocs IA avec VideoCaptureCoreX et MediaPlayerCoreX](x-engines.md) pour l'API
complète `Audio_Processing_*`/`Audio_OutputBlock` et les règles de cycle de vie.

## Threading, cadencement et durée de vie

VAD et Whisper s'exécutent **de manière synchrone sur le thread de streaming GStreamer** : l'audio est
segmenté et transcrit sur place, et le segment final est vidé en fin de flux. Rien n'est perdu — la
totalité de l'entrée est transcrite sans perte, et la position du pipeline suit le front de
transcription (utile pour une barre de progression). Comme la transcription s'exécute en ligne, la
source est cadencée sur Whisper : si Whisper est plus lent que le temps réel, l'amont (y compris un
périphérique de capture en direct) est ralenti plutôt que de perdre de l'audio. En pratique, Whisper
Base s'exécute bien au-delà du temps réel, il n'est donc pas le goulot d'étranglement pour une source
typique.

`OnSpeechRecognized` est déclenché sur ce même thread de streaming — ne touchez jamais directement à
l'interface utilisateur depuis le gestionnaire ; orchestrez l'appel vers le dispatcher d'interface
utilisateur ou le thread principal.

Après le démarrage d'une session de capture ou de lecture, le moteur possède les instances connectées
de `SpeechToTextBlock` et les libère à l'arrêt de la session. Créez un nouveau bloc pour la session
suivante.

## Cas d'usage

- **Sous-titrage en direct** — sous-titres en temps réel pour un flux en direct, un webinaire ou une
  superposition d'accessibilité.
- **Transcription de réunions et d'appels** — transcrire un flux microphone en parallèle d'une capture
  `VideoCaptureCoreX`.
- **Indexation et recherche de médias** — transcrire par lots des fichiers vidéo/audio enregistrés pour
  rendre leur contenu consultable.
- **Création de sous-titres** — générer des fichiers annexes `.srt`/`.vtt` à partir d'une vidéo source
  sans service de transcription tiers.
- **Sous-titres de traduction** — définir `Task = SpeechToTextTask.Translate` pour sous-titrer en
  anglais une parole non anglaise.

## Dépannage

| Symptôme | Cause probable | Solution |
| --- | --- | --- |
| Aucun segment n'est jamais reconnu | `WhisperModelPath` invalide, ou chaîne audio non construite | Vérifiez que le fichier de modèle GGML existe à ce chemin ; pour la capture/lecture, vérifiez que la chaîne audio est active (voir `Audio_OutputBlock` ci-dessous). |
| Whisper « hallucine » du texte pendant les silences | `EnableVad` vaut `false` | Activez VAD (`EnableVad = true`, la valeur par défaut) afin que Whisper ne s'exécute que sur la parole détectée, et non sur des fenêtres fixes potentiellement silencieuses. |
| La transcription accuse un retard sur l'audio en direct | Whisper est plus lent que le temps réel sur le matériel/la taille de modèle actuels | Choisissez un `WhisperModelSize`/fichier de modèle plus petit, ou utilisez `Provider = CUDA` sur une machine avec un GPU NVIDIA. |
| Les segments sont coupés en plein milieu d'une phrase | `SileroVadSettings.MaxSpeechMs` coupe de force un énoncé continu de longue durée | Il s'agit d'une borne délibérée (15 s par défaut) afin qu'une transcription en cours ne puisse pas croître sans limite ; augmentez `MaxSpeechMs` si votre scénario nécessite des segments ininterrompus plus longs et peut tolérer une borne plus élevée. |
| Aucun audio n'atteint le bloc sur `VideoCaptureCoreX`/`MediaPlayerCoreX` | Chaîne audio non construite (`Audio_Source`/`Audio_Play` manquant) | Consultez [Transcription en direct du microphone avec VideoCaptureCoreX](#transcription-en-direct-du-microphone-avec-videocapturecorex) et [Transcription de fichier avec MediaPlayerCoreX](#transcription-de-fichier-avec-mediaplayercorex) ci-dessus. |
| Le fichier `.srt`/`.vtt` est vide | Segments jamais finalisés, ou chemin incorrect | Vérifiez que `OutputSrtPath`/`OutputVttPath` pointent vers un chemin accessible en écriture et que de la parole a bien été détectée ; seuls les segments finaux sont écrits. |

## Foire aux questions

### SpeechToTextBlock nécessite-t-il une connexion Internet ?

Non — la transcription s'exécute entièrement sur l'appareil via Whisper.net/GGML ; les fichiers de
modèle sont téléchargés une seule fois par votre application (ou fournis avec elle), et non appelés
par requête sur le réseau.

### Quelles langues sont prises en charge ?

Whisper est multilingue — définissez `Language` avec un code ISO 639-1, ou laissez `"auto"` pour que
Whisper détecte automatiquement la langue parlée.

### Puis-je traduire la parole en sous-titres anglais au lieu de transcrire la langue source ?

Oui — définissez `SpeechToTextSettings.Task = SpeechToTextTask.Translate`.

### Comment obtenir des sous-titres en direct à l'écran plutôt qu'un simple événement ?

Connectez `SubtitleRenderer.OnSpeechRecognized` comme gestionnaire d'événements du bloc, avec un
`OverlayManagerBlock` — voir [SubtitleRenderer — sous-titres à l'écran](#subtitlerenderer-sous-titres-a-lecran).
Si vous avez seulement besoin de fichiers SRT/VTT, définissez plutôt
`OutputSrtPath`/`OutputVttPath`.

### Quelle taille de modèle Whisper dois-je utiliser ?

Commencez avec `Base` (la valeur par défaut) pour la transcription CPU en temps réel. Passez à
`Small`/`Medium`/`LargeV3` (ou leurs variantes quantifiées) pour une précision supérieure si vous
disposez d'un GPU ou pouvez tolérer un traitement plus lent que le temps réel ; voir le tableau des
[Tailles de modèle Whisper](#tailles-de-modele-whisper) pour le compromis complet.

## Démos

- **[Live Subtitles Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Subtitles%20Demo)** — démo WPF de pipeline Media Blocks.
- **[Live Subtitles MB](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/Live%20Subtitles%20MB)** — la même démo Media Blocks pour MAUI.
- **[Live Subtitles](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/Live%20Subtitles)** — démo console sans interface (télécharge les modèles au premier lancement).

Des démos dédiées de sous-titrage en direct `VideoCaptureCoreX`/`MediaPlayerCoreX` (`Capture Live
Subtitles X`, `Capture Live Subtitles X WPF`, `Player Live Subtitles X`, `Player Live Subtitles X
WPF`) figurent dans l'ensemble de démos du SDK et seront liées ici une fois publiées dans le dépôt
d'échantillons public.
