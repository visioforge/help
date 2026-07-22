---
title: Diarisation des locuteurs .NET — SpeakerDiarizationBlock
description: Diarisation des locuteurs sur l'appareil pour .NET — « qui a parlé et quand » avec pyannote, embeddings WeSpeaker et transcriptions diarisées.
sidebar_label: Diarisation des locuteurs
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

# Diarisation des locuteurs — SpeakerDiarizationBlock

`SpeakerDiarizationBlock` est un Media Block audio uniquement qui réside dans l'espace de noms
`VisioForge.Core.MediaBlocks.AI` (assembly `VisioForge.Core.AI`, package NuGet `VisioForge.DotNet.Core.AI`).
Il répond à « qui a parlé et quand » : il intercepte le flux audio, détecte les tours de parole avec un modèle
ONNX de segmentation pyannote, transforme chaque tour en empreinte vocale avec un modèle ONNX d'embeddings
WeSpeaker/3D-Speaker et — une fois l'enregistrement terminé — regroupe les empreintes vocales en locuteurs
et publie la chronologie. L'audio traverse le bloc : l'interception force un format d'échantillon en virgule
flottante 32 bits entrelacé, mais la fréquence d'échantillonnage et le nombre de canaux restent inchangés.
Le bloc implémente `IAudioProcessingBlock`, il peut donc être inséré dans un pipeline manuel ou enregistré
directement sur `VideoCaptureCoreX`/`MediaPlayerCoreX`.

L'implémentation suit le pipeline de diarisation des locuteurs hors ligne de référence
[sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx), qui utilise ces deux mêmes modèles.

```csharp
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.Types.X.AI;
```

## La diarisation est hors ligne — la réponse arrive en fin de flux

**Le bloc ne rapporte rien tant que l'audio est encore en cours de lecture.** Ce n'est pas une limitation de
l'implémentation, c'est ce que le problème autorise :

- Le modèle de segmentation sépare les voix **à l'intérieur d'une seule fenêtre de 10 secondes** et les numérote
  localement. Le « locuteur 1 » de la fenêtre 5 et le « locuteur 1 » de la fenêtre 6 n'ont aucun rapport — le
  modèle ne prétend jamais le contraire.
- La seule chose qui puisse relier une voix entendue à la minute 1 à la même voix à la minute 40, c'est de
  **regrouper ensemble toutes les empreintes vocales de l'enregistrement**, et cela ne peut pas être fait tant
  que toutes les empreintes vocales n'existent pas.

Le bloc analyse donc en continu à mesure que l'audio s'écoule (ce travail est réel, et c'est pourquoi la réponse
apparaît presque immédiatement après le dernier échantillon), mais il ne déclenche `OnSpeakerSegment` — une fois
par tour, dans l'ordre des temps de début — qu'après la fin de flux. **Il convient aux fichiers et aux flux
finis.** Une source en direct sans fin n'atteint jamais le moment où une réponse existe.

## Configuration de base du bloc

```csharp
var settings = new SpeakerDiarizationSettings(segmentationModelPath, embeddingModelPath)
{
    Provider = OnnxExecutionProvider.CPU,
};

var diarization = new SpeakerDiarizationBlock(settings);

// Déclenché pour chaque tour, dans l'ordre, après la fin du flux.
diarization.OnSpeakerSegment += (sender, e) =>
{
    var turn = e.Segment;
    Console.WriteLine($"speaker_{turn.SpeakerId:D2}: {turn.Start:c} - {turn.End:c} (conf {turn.Confidence:F2})");
};
```

La même chronologie est disponible d'un seul tenant via `GetTimeline()`. Vérifiez d'abord
`IsTimelineComplete` — une chronologie vide signifie « nous n'avons jamais pu déterminer qui a parlé », et non
« personne n'a parlé » :

```csharp
if (!diarization.IsTimelineComplete)
{
    // L'exécution n'a jamais atteint l'étape de regroupement : le pipeline a été arrêté avant la fin de flux,
    // ou le worker d'analyse a échoué. La chronologie est vide, et ce vide n'est pas une réponse.
    Console.WriteLine("Aucun résultat de diarisation — l'enregistrement n'a pas été entendu jusqu'au bout.");
    return;
}

Console.WriteLine($"{diarization.SpeakerCount} locuteur(s).");

if (diarization.UnattributedTurns > 0)
{
    // De la parole que les modèles ont trouvée mais n'ont pas pu transformer en empreinte vocale : ces tours
    // ne portent aucun identifiant de locuteur et n'atteignent jamais la chronologie, bien que leur audio AIT été analysé.
    Console.WriteLine($"{diarization.UnattributedTurns} tour(s) n'ont pas pu être attribués à un locuteur.");
}

foreach (var turn in diarization.GetTimeline())
{
    Console.WriteLine(turn); // speaker_00 [00:00:01.583 --> 00:00:03.405] (conf=0.98)
}
```

## Modèles — téléchargement à l'exécution

Deux modèles ONNX sont requis, tous deux téléchargés à l'exécution (aucun n'est fourni dans les paquets
NuGet du SDK). Les deux sont exportés par le projet
[sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx) :

| Modèle | Rôle | Licence | Source |
| --- | --- | --- | --- |
| pyannote `segmentation-3.0` | Segmentation des tours de parole locaux (fenêtre de 10 s, jusqu'à 3 locuteurs locaux, activité en ensemble des parties) | MIT | Extrayez `model.onnx` de la ressource de version [`sherpa-onnx-pyannote-segmentation-3-0.tar.bz2`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-segmentation-models/sherpa-onnx-pyannote-segmentation-3-0.tar.bz2). |
| WeSpeaker `voxceleb_resnet34_LM` (anglais, **par défaut**) | Embedding de locuteur (fbank de 80 bandes en entrée, empreinte vocale de 256 dimensions en sortie) | Apache-2.0 | Ressource de version [`wespeaker_en_voxceleb_resnet34_LM.onnx`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-recongition-models/wespeaker_en_voxceleb_resnet34_LM.onnx). |
| 3D-Speaker `eres2netv2_sv_zh-cn_16k-common` (**multilingue**) | Embedding de locuteur (fbank de 80 bandes en entrée, empreinte vocale de 192 dimensions en sortie) ; sépare bien mieux les locuteurs sur de la parole non anglophone | Apache-2.0 | Ressource de version [`3dspeaker_speech_eres2netv2_sv_zh-cn_16k-common.onnx`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-recongition-models/3dspeaker_speech_eres2netv2_sv_zh-cn_16k-common.onnx). |

Le modèle de segmentation est **indépendant de la langue** — seul le modèle d'embeddings doit
correspondre à votre langue. Les valeurs par défaut `SegmentationModelUrl` / `EmbeddingModelUrl` de
`SpeakerDiarizationSettings` pointent vers les ressources de version pyannote et WeSpeaker anglais. Elles
sont informatives — le SDK ne télécharge jamais rien de lui-même ; les fichiers réellement chargés sont
`SegmentationModelPath` et `EmbeddingModelPath`. Tout modèle d'embeddings WeSpeaker ou 3D-Speaker à fbank
de 80 bandes dont les poids sont utilisables commercialement peut être substitué ; le front-end de
caractéristiques fbank se configure lui-même (mise à l'échelle des échantillons et normalisation des
caractéristiques) à partir des métadonnées ONNX du modèle, donc aucun réglage manuel n'est nécessaire
lors du changement.

## Diarisation multilingue

L'embedding WeSpeaker par défaut est entraîné en anglais (VoxCeleb) et sous-détecte les locuteurs dans
les autres langues. Pour un contenu non anglophone ou multilingue, utilisez le modèle d'embeddings
**3D-Speaker ERes2NetV2** multilingue (Apache-2.0) : faites pointer `EmbeddingModelPath` vers son fichier
ONNX et définissez `EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual`. Le modèle de
segmentation ne change pas.

```csharp
var settings = new SpeakerDiarizationSettings(segmentationModelPath, eres2netv2ModelPath)
{
    EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual,

    // Réajustez le seuil pour cet embedding — voir ci-dessous. La valeur par défaut (0.5) le sursegmente.
    ClusterThreshold = 0.8f,
    Provider = OnnxExecutionProvider.CPU,
};

var diarization = new SpeakerDiarizationBlock(settings);
```

!!! warning "Réajustez `ClusterThreshold` lorsque vous changez de modèle d'embedding"

    Chaque modèle d'embedding a sa propre géométrie cosinus, et le `ClusterThreshold` par défaut de `0.5`
    est la bonne distance pour **WeSpeaker**. Les empreintes vocales d'ERes2NetV2 sont plus éloignées les
    unes des autres : à cette même distance, un seul locuteur se scinde en plusieurs — sur un clip de
    référence à quatre locuteurs, on en obtient six. Une valeur autour de **`0.8`** est le point de départ
    pour ERes2NetV2. Si vous connaissez le nombre de locuteurs, renseigner `NumSpeakers` élimine la question.

`EmbeddingModel` est une indication déclarative : elle sélectionne l'URL de téléchargement recommandée,
tandis que le fichier réellement chargé est toujours `EmbeddingModelPath`. Comme le front-end lit la
configuration des caractéristiques depuis les métadonnées ONNX du modèle, passer d'un modèle à l'autre ne
nécessite aucune autre modification de code.

Les URL de téléchargement sont aussi exposées comme constantes :
`SpeakerDiarizationSettings.WeSpeakerEnglishModelUrl` et
`SpeakerDiarizationSettings.ERes2NetV2MultilingualModelUrl`.

## Paramètres clés

`SpeakerDiarizationSettings(segmentationModelPath, embeddingModelPath)`. Contrairement aux paramètres
d'IA de vision, ce type ne dérive **pas** de `OnnxInferenceSettings` — ceux-ci décrivent le
prétraitement des images vidéo, ce qui ne s'applique pas à une paire de modèles audio.

Les valeurs par défaut sont celles de l'implémentation de référence et constituent un bon point de départ
pour un contenu quelconque.

| Propriété | Défaut | Description |
| --- | --- | --- |
| `SegmentationModelPath` | — | Chemin absolu vers le modèle ONNX pyannote `segmentation-3.0`. Obligatoire. |
| `EmbeddingModelPath` | — | Chemin absolu vers le modèle ONNX d'embeddings de locuteur (par exemple WeSpeaker `voxceleb_resnet34_LM` ou 3D-Speaker `eres2netv2`). Obligatoire. |
| `EmbeddingModel` | `WeSpeakerEnglish` | Pour quel modèle d'embeddings les paramètres sont configurés (`WeSpeakerEnglish` ou `ERes2NetV2Multilingual`). Une indication déclarative qui choisit l'URL par défaut ; le fichier chargé est toujours `EmbeddingModelPath`, et l'extraction des caractéristiques s'autoconfigure depuis ses métadonnées ONNX. |
| `SegmentationModelUrl` | version pyannote de sherpa-onnx | URL de téléchargement informative du modèle de segmentation. |
| `EmbeddingModelUrl` | version WeSpeaker de sherpa-onnx | URL de téléchargement informative du modèle d'embeddings. |
| `NumSpeakers` | `-1` | Le nombre **exact** de locuteurs, lorsque vous le connaissez. Une valeur positive fige le résultat sur ce nombre de locuteurs et `ClusterThreshold` est ignoré. `-1` signifie « déduis-le des voix » — le bon réglage dès que le décompte n'est pas certain. |
| `ClusterThreshold` | `0.5` | La **DISTANCE** cosinus à partir de laquelle deux empreintes vocales cessent d'être la même personne. Attention au sens : il s'agit d'une distance (0 = voix identiques), et **non** d'une similarité — une valeur **plus petite** est plus stricte et donne **plus** de locuteurs. Ignoré lorsque `NumSpeakers` est positif. |
| `MinDurationOn` | `0.3` | Le tour le plus court, en secondes, qui soit rapporté. Les plus courts sont écartés comme du bruit. |
| `MinDurationOff` | `0.5` | La pause la plus longue, en secondes, qui compte encore comme le même tour. Deux tours d'un même locuteur plus proches que cela sont fusionnés, de sorte qu'un locuteur qui reprend son souffle reste un seul tour. |
| `Provider` | `Auto` | Fournisseur d'exécution pour les deux sessions ONNX. `Auto` utilise le fournisseur le plus rapide présent dans le binaire natif ONNX Runtime chargé et se rabat sur le CPU. |
| `DeviceId` | `0` | Identifiant du périphérique matériel lorsqu'un fournisseur GPU est sélectionné. |

## Tours de locuteur

Chaque `SpeakerSegment` (issu de `SpeakerSegmentEventArgs.Segment` et de `GetTimeline()`) :

| Propriété | Description |
| --- | --- |
| `SpeakerId` | Identifiant de locuteur stable, à base zéro, pour cette exécution. |
| `Start` / `End` | `TimeSpan` sur la chronologie du média. |
| `Duration` | `End - Start`. |
| `Confidence` | À quel point les fenêtres d'analyse qui se recouvrent se sont accordées sur ce locuteur pour ce tour (0..1). `1` signifie que toutes les fenêtres ayant vu cet instant ont nommé le même locuteur. |

Les identifiants de locuteur sont stables **au sein d'une exécution**, pas d'une exécution de pipeline à
l'autre (cette version ne réidentifie pas un locuteur entre exécutions).

## Transcription diarisée — « qui a dit quoi »

`DiarizedTranscriptBuilder` fusionne une transcription de reconnaissance vocale avec la chronologie de
diarisation, en étiquetant chaque segment de transcription avec le locuteur qui le recouvre le plus (les
égalités reviennent à l'identifiant de locuteur le plus bas). C'est du traitement de données pur — sans
dépendance de modèle ni de média — il associe donc un [`SpeechToTextBlock`](speech-to-text.md) à un
`SpeakerDiarizationBlock`. Construisez-la après la fin de flux, lorsque la chronologie de diarisation existe.

```csharp
using VisioForge.Core.Types.X.AI;

// transcript : tuples (start, end, text) issus de SpeechToTextBlock.OnSpeechRecognized
// timeline : diarization.GetTimeline()
var labeled = DiarizedTranscriptBuilder.Build(transcript, timeline);
foreach (DiarizedTranscriptSegment seg in labeled)
{
    // "[00:00:01.000 --> 00:00:03.000] speaker_00: A pencil with black lead writes best."
    Console.WriteLine(seg);
}
```

Chaque `DiarizedTranscriptSegment` porte `Start`, `End`, `Text` et le `SpeakerId` attribué (`-1`
lorsqu'aucun tour de diarisation ne recouvre le segment — par exemple un silence).

## Pipeline manuel Media Blocks

Placez `SpeakerDiarizationBlock` dans la chaîne audio avant le rendu ou la sortie audio :

```csharp
var diarization = new SpeakerDiarizationBlock(settings);
diarization.OnSpeakerSegment += Diarization_OnSpeakerSegment;

pipeline.Connect(audioSource.Output, diarization.Input);
pipeline.Connect(diarization.Output, audioRenderer.Input);
```

## Diarisation de fichier avec MediaPlayerCoreX

Pour la lecture, `Audio_Play` doit valoir `true` pour que la chaîne audio soit construite :

```csharp
player.Audio_Play = true;

var diarization = new SpeakerDiarizationBlock(settings);
diarization.OnSpeakerSegment += Diarization_OnSpeakerSegment;

player.Audio_Processing_AddBlock(diarization); // avant OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Laissez le fichier se lire jusqu'à sa fin : arrêter la lecture prématurément saute l'étape de regroupement
et laisse la chronologie vide. La diarisation ne lit que l'audio, donc pour indexer un fichier aussi vite que
le matériel le permet plutôt qu'en temps réel, donnez au lecteur un puits audio non synchronisé — `player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };` —
afin que l'audio ne soit pas cadencé par l'horloge. (`Video_Renderer_IsSync = false` ne désynchronise qu'un
rendu vidéo, il n'a donc aucun effet sur un fichier uniquement audio.)

Voir [Utilisation des blocs d'IA avec VideoCaptureCoreX et MediaPlayerCoreX](x-engines.md) pour l'API
`Audio_Processing_*` complète et les règles de cycle de vie. Le moteur possède le bloc câblé après le
démarrage et le libère à l'arrêt de la session — créez un nouveau bloc pour la session suivante.

## Fonctionnement

Comprendre ces trois étapes explique chacun des paramètres ci-dessus, et pourquoi le résultat ne peut pas
arriver plus tôt.

1. **Segmentation, à mesure que l'audio s'écoule.** L'audio est rééchantillonné en mono 16 kHz et découpé en
   fenêtres de 10 secondes qui avancent d'une seconde à la fois — les fenêtres se recouvrent donc à 90 %, et
   chaque instant d'audio est vu par une dizaine d'entre elles. Le modèle pyannote indique, pour chacune de ses
   ~59 trames de sortie par seconde, lesquels des trois locuteurs *locaux à la fenêtre* au maximum sont en train
   de parler.
2. **Empreintes vocales, à mesure que l'audio s'écoule.** Pour chaque fenêtre et chaque locuteur qui y est actif,
   l'audio de ce locuteur est collecté — **moins chaque trame où quelqu'un d'autre parle aussi** — et transformé
   en empreinte vocale. La parole superposée est exclue délibérément : une empreinte vocale prise sur deux voix à
   la fois se situe entre les deux et ressemble aux deux, ce qui soude deux locuteurs en un seul. Les tours
   comptant moins de ~0,17 s de parole propre sont écartés pour la même raison.
3. **Locuteurs et chronologie, en fin de flux.** Toutes les empreintes vocales de l'enregistrement sont regroupées
   ensemble (regroupement agglomératif, saut complet, distance cosinus — `ClusterThreshold` / `NumSpeakers`). Les
   numéros de locuteur locaux aux fenêtres sont réécrits en numéros globaux, toutes les fenêtres qui se recouvrent
   sont posées sur une même grille de trames, et **chaque trame est décidée par un vote entre les ~10 fenêtres qui
   la couvrent** : combien de personnes parlent est la moyenne de ce que ces fenêtres rapportent, et qui elles sont
   est le plus voté. Une fenêtre qui étiquette mal un instant est mise en minorité par ses voisines. Enfin, les
   trames deviennent des tours, les courtes pauses sont comblées (`MinDurationOff`) et les fragments sont écartés
   (`MinDurationOn`).

## Threads, performances et cycle de vie

L'analyse s'exécute sur un **worker d'arrière-plan dédié**, pas sur le thread de streaming : l'interception
rééchantillonne l'audio en mono 16 kHz et le pousse dans une **file non bornée**, et le worker le consomme
par fenêtres qui se recouvrent. Comme l'analyse est hors du thread de streaming, l'audio n'est jamais cadencé
sur les modèles : une source qui va plus vite qu'eux (un fichier décodé à pleine vitesse, ou une source en
direct sur un fournisseur d'exécution lent) fait simplement prendre du retard au worker.

**Un arriéré n'est jamais abandonné.** La file grandit à la place — en mono 16 kHz cela coûte 64 Ko par seconde
mise en tampon — et elle se vide dès que le worker rattrape son retard. Si une machine est durablement plus lente
que sa source, l'arriéré ne cesse de croître, et c'est le signal honnête : la réponse est un matériel plus rapide
ou un fournisseur d'exécution GPU, jamais l'analyse d'une fraction de l'audio. En fin de flux, le pipeline attend
que le worker vide l'arriéré et publie la chronologie avant de se démonter.

`OnSpeakerSegment` est déclenché sur le thread du worker — ne touchez jamais l'interface utilisateur
directement depuis le gestionnaire ; déléguez au dispatcher de l'interface ou au thread principal.

La mémoire croît avec l'enregistrement : les empreintes vocales sont conservées jusqu'à la fin (elles sont
l'entrée du regroupement), et le regroupement construit une matrice de distances deux à deux sur celles-ci.

## Complétude du résultat

| Membre | Description |
| --- | --- |
| `IsTimelineComplete` | `true` lorsque l'exécution a réellement produit une réponse : le flux a atteint la fin de flux, le worker a vidé son arriéré, et les locuteurs ont été regroupés et publiés. **Vérifiez-le avant de lire `GetTimeline()`** — lorsqu'il vaut `false`, la chronologie est VIDE, et ce vide signifie « nous n'avons jamais pu déterminer qui a parlé », et non « personne n'a parlé ». |
| `SpeakerCount` | Combien de locuteurs distincts ont été trouvés. `0` tant que la chronologie n'est pas publiée. |
| `UnattributedTurns` | Tours de parole que le modèle de segmentation a **trouvés** mais que le modèle d'embeddings n'a pas pu transformer en empreinte vocale. Leur audio *a bien* été analysé, mais ils ne portent aucun identifiant de locuteur et sont absents de la chronologie. |
| `ActiveProvider` | Le fournisseur d'exécution réellement engagé par les sessions ONNX. Reste valide après l'exécution, il peut donc être journalisé à la fin. |

**Une chronologie n'est pleinement digne de confiance que lorsque `IsTimelineComplete` vaut `true` ET que
`UnattributedTurns` vaut `0`.**

## Cas d'usage

- **Transcription de réunions et d'entretiens** — étiquetez une transcription diarisée avec les tours par locuteur.
- **Analytique de centres d'appels** — séparez la parole de l'agent et du client pour des métriques de temps de parole et d'alternance.
- **Indexation de médias** — indexez la vidéo/l'audio enregistrés par locuteur pour la recherche et la navigation.
- **Outils de podcast et de diffusion** — générez des notes d'émission étiquetées par locuteur avec la reconnaissance vocale.

## Dépannage

| Symptôme | Cause probable | Solution |
| --- | --- | --- |
| Aucun tour n'est jamais déclenché, et `IsTimelineComplete` vaut `false` | Le flux ne s'est jamais terminé — le pipeline a été arrêté prématurément, ou la source est un flux en direct sans fin | Laissez la source atteindre la fin de flux. La diarisation a besoin de l'enregistrement complet ; il n'y a pas de réponse partielle à donner. |
| Aucun tour n'est déclenché bien que le fichier ait été lu jusqu'au bout | Un chemin de modèle est invalide, ou la chaîne audio n'est pas construite | Vérifiez que les deux fichiers ONNX existent à leurs chemins ; pour la lecture, vérifiez `Audio_Play = true`. |
| Trop de locuteurs détectés | `ClusterThreshold` trop petit pour cet audio/ce modèle | **Augmentez** `ClusterThreshold` (c'est une distance — une valeur plus grande fusionne davantage), ou fixez `NumSpeakers` au nombre connu. |
| Locuteurs fusionnés en un seul | `ClusterThreshold` trop grand | **Diminuez** `ClusterThreshold` (une distance plus petite scinde plus facilement), ou fixez `NumSpeakers`. |
| L'analyse est en retard sur une source de fichier rapide, et la mémoire croît | Les fenêtres qui se recouvrent sont coûteuses en calcul : le worker prend du retard et l'arriéré (non borné) grossit — aucun audio n'est perdu, mais il est mis en tampon | Utilisez `Provider = OnnxExecutionProvider.CUDA`/`DirectML` sur un GPU. Une seconde mise en tampon coûte 64 Ko. |
| De la parole manque alors que `IsTimelineComplete` vaut `true` | Le modèle d'embeddings n'a pas pu calculer l'empreinte vocale de certains tours détectés, qui sont donc restés sans identifiant de locuteur | Vérifiez `UnattributedTurns` ; cherchez l'erreur `SpeakerEmbeddingEngine` dans le journal pour la cause (typiquement une mémoire GPU insuffisante ou un repli de fournisseur). |

## Foire aux questions

### Pourquoi n'obtenez-vous aucun tour de locuteur avant la fin du fichier ?

Parce que jusque-là, il n'existe pas de « locuteur 2 » à rapporter. Le modèle de segmentation ne sépare les voix
qu'à l'intérieur d'une seule fenêtre de 10 secondes et les numérote localement ; relier une voix à la minute 1 à
la même voix à la minute 40 exige de regrouper d'un coup toutes les empreintes vocales de l'enregistrement. Le bloc
fait tout le travail qu'il peut à mesure que l'audio s'écoule, et publie dès que le dernier échantillon est arrivé.

### Puis-je diariser un flux en direct sans fin ?

Non. Un flux en direct n'a pas de fin de flux, donc l'étape de regroupement ne s'exécute jamais. Diarisez des
enregistrements finis — ou découpez un flux en direct en morceaux finis et diarisez chacun d'eux (les identifiants
de locuteur ne sont pas comparables d'un morceau à l'autre).

### SpeakerDiarizationBlock nécessite-t-il une connexion Internet ?

Non — la diarisation s'exécute entièrement sur l'appareil via ONNX Runtime. Votre application télécharge
les deux fichiers de modèle une seule fois (ou les intègre) ; rien n'est appelé par requête sur le
réseau.

### Puis-je le combiner avec la reconnaissance vocale pour obtenir une transcription étiquetée par locuteur ?

Oui — exécutez un [`SpeechToTextBlock`](speech-to-text.md) et un `SpeakerDiarizationBlock` sur le même
audio, puis fusionnez la transcription avec `diarization.GetTimeline()` à l'aide de
`DiarizedTranscriptBuilder.Build` après la fin de flux.

### Les identifiants de locuteur sont-ils les mêmes d'une exécution à l'autre ?

Non — les identifiants ne sont stables qu'au sein d'une seule exécution. Cette version ne réidentifie pas
un locuteur entre exécutions.

### Avec quelles langues fonctionne-t-il ?

Le modèle de segmentation est indépendant de la langue, il fonctionne donc d'emblée avec n'importe quelle
langue. Le modèle d'embeddings WeSpeaker par défaut est entraîné en anglais (VoxCeleb) et sous-détecte
les locuteurs dans les autres langues ; pour de l'audio non anglophone ou multilingue, changez
`EmbeddingModelPath` pour le modèle multilingue 3D-Speaker ERes2NetV2 (Apache-2.0) et définissez
`EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual` (voir
[Diarisation multilingue](#diarisation-multilingue)). Le front-end adapte automatiquement son extraction
de caractéristiques au modèle choisi à partir des métadonnées ONNX.
