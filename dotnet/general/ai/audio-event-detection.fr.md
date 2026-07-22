---
title: Détection d'événements audio en .NET — YAMNet AudioSet
description: Détectez des sons réels — sirène, aboiement, bris de verre, alarme, musique, parole — dans l'audio en direct ou fichier avec YAMNet AudioSet, sur l'appareil.
sidebar_label: Détection d'événements audio
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

# Détection d'événements audio — AudioEventDetectorBlock

`AudioEventDetectorBlock` est un Media Block audio uniquement de `VisioForge.DotNet.Core.AI`. Il capte
le flux audio, le rééchantillonne en 16&nbsp;kHz mono et exécute un classifieur ONNX **YAMNet**
(Google, AudioSet) sur de courtes fenêtres pour reconnaître des sons du monde réel — sirène, aboiement
de chien, bris de verre, alarme, musique, parole et des centaines d'autres (521 classes AudioSet).
L'audio passe sans modification. Le bloc implémente `IAudioProcessingBlock`, il peut donc être inséré
dans un pipeline manuel ou enregistré sur `VideoCaptureCoreX`/`MediaPlayerCoreX`. C'est un complément
naturel des fonctions d'analyse d'objets et PTZ pour la surveillance et le monitoring.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.AI;
using VisioForge.Core.Types.X.Sources;
```

## Configuration de base du bloc

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    Threshold = 0.5f,        // confiance qu'une classe doit atteindre pour se déclencher
    SmoothingWindows = 3,    // moyenne mobile qui supprime les pics d'une seule fenêtre
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += (sender, e) =>
{
    // Déclenché sur un thread de travail en arrière-plan — passez au thread UI avant de toucher l'UI.
    Console.WriteLine($"{e.Label} ({e.Confidence:F2}) [{e.Start:mm\\:ss} - {e.End:mm\\:ss}]");
};
```

Connectez-le entre une source audio et un puits (ou tout bloc audio en aval) :

```csharp
var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(mediaFile, renderVideo: false, renderAudio: true));

// Non synchronisé : un fichier hors ligne est analysé aussi vite que le modèle le permet.
// Conservez IsSync = true (la valeur par défaut) pour une source en direct que vous souhaitez aussi monitorer.
var sink = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };

pipeline.Connect(source.AudioOutput, detector.Input);
pipeline.Connect(detector.Output, sink.Input);

await pipeline.StartAsync();
```

## Utilisation avec VideoCaptureCoreX et MediaPlayerCoreX

`AudioEventDetectorBlock` implémente `IAudioProcessingBlock` : au lieu de construire un pipeline
manuel, vous pouvez donc l'enregistrer sur un moteur X. Le bloc **doit être ajouté avant le démarrage
de la session** — la liste des blocs de traitement est consommée pendant la construction du pipeline,
et un bloc ajouté plus tard est ignoré.

Pour la capture, terminez la chaîne audio par un moteur de rendu nul non synchronisé si vous souhaitez
l'analyse sans monitoring au haut-parleur ni enregistrement :

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += Detector_OnAudioEvent;

core.Audio_Processing_AddBlock(detector); // avant StartAsync
await core.StartAsync();
```

Pour la lecture, `Audio_Play` doit valoir `true` pour que la chaîne audio soit construite :

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += Detector_OnAudioEvent;

player.Audio_Processing_AddBlock(detector); // avant OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Consultez [Utiliser les blocs d'IA avec VideoCaptureCoreX et MediaPlayerCoreX](x-engines.md) pour l'API
complète `Audio_Processing_*`/`Audio_OutputBlock` et les règles de cycle de vie.

## Comment les événements sont déclenchés

Le bloc classe chaque fenêtre de `WindowSeconds`, moyenne les scores par trame du modèle et les lisse
avec une moyenne mobile sur `SmoothingWindows` fenêtres. Une **hystérésis** par classe (déclencheur de
Schmitt) fusionne un son continu en un seul événement : une classe entre à `Threshold` et sort à
`Threshold * ReleaseRatio` (0.7 par défaut). `OnAudioEvent` se déclenche **une fois par son**, lorsqu'il
se termine (passe sous le seuil de relâchement) ou en fin de flux, portant l'intervalle complet
`Start`..`End` et la `Confidence` maximale. Ainsi, une sirène continue produit un seul événement avec
une durée, pas un flot de doublons.

`Start` et `End` sont mesurés sur l'audio que le bloc a réellement analysé, à partir de zéro au
démarrage du pipeline : ce ne sont pas des positions dans le média. Pour une lecture ou une capture
continue depuis le début, les deux coïncident. Ils divergent dès que de l'audio est sauté ou rejoué
sous le bloc : se positionner ailleurs dans un `MediaPlayerCoreX` ne réinitialise pas cette horloge,
de sorte qu'après un repositionnement les temps rapportés ne correspondent plus aux positions du
fichier, et un trou dans une source en direct laisse tous les temps ultérieurs en avance de la durée
manquante.

L'inférence s'exécute sur un thread de travail dédié alimenté par une file d'échantillons **non
bornée** : le thread de streaming n'est donc jamais bloqué et **la file ne supprime jamais
d'échantillons pour borner sa propre croissance**. Une source plus rapide que le worker (une source en
direct sur un fournisseur lent, ou un fichier décodé à pleine vitesse par un moteur de rendu non
synchronisé) ne fait que faire croître le retard accumulé — 64&nbsp;Ko par seconde mise en file en mono
16&nbsp;kHz — qui se résorbe dès que le worker rattrape. Deux sorties prévues arrêtent l'analyse au
lieu d'en supprimer silencieusement une partie ; toutes deux sont décrites dans les remarques
ci-dessous. YAMNet est minuscule — sur le CPU, environ 0,6&nbsp;ms pour la fenêtre par défaut de
0,975&nbsp;s et 1,8&nbsp;ms pour le maximum de 5&nbsp;s (le coût suit le nombre de trames de la
fenêtre), plus de 1000× le temps réel dans les deux cas ; le fournisseur CPU suffit donc généralement.

## Paramètres clés

`AudioEventDetectorSettings(yamnetModelPath)`. Contrairement aux paramètres d'IA de vision, ce type ne
dérive **pas** de `OnnxInferenceSettings` — YAMNet consomme une forme d'onde audio, pas une image.
Chaque propriété ci-dessous est lue **une seule fois, à la construction du bloc** pendant `StartAsync` :
définissez-les avant de démarrer le pipeline ; modifier l'objet de paramètres ensuite reste sans effet.

| Propriété | Défaut | Description |
| --- | --- | --- |
| `ModelPath` | — | Chemin absolu du modèle ONNX YAMNet. Requis. |
| `Threshold` | 0.5 | Confiance qu'une classe doit atteindre pour démarrer un événement, bornée à 0.01–1 (un seuil de 0 ne pourrait jamais être relâché et verrouillerait toutes les classes). |
| `ReleaseRatio` | 0.7 | Un événement actif se termine sous `Threshold * ReleaseRatio` (hystérésis), borné à 0.01–1 pour la même raison. |
| `SmoothingWindows` | 3 | Fenêtres sur lesquelles les scores sont moyennés (1 désactive le lissage). |
| `WindowSeconds` | 0.975 | Longueur de la fenêtre d'inférence : un patch YAMNet de 0.96&nbsp;s plus une petite marge ; bornée à 0.96–5&nbsp;s. |
| `ClassFilter` | null | Liste d'autorisation optionnelle de noms de classe AudioSet (ex. `"Siren"`, `"Dog"`). |
| `Provider` | `Auto` | Fournisseur d'exécution ONNX (le CPU suffit généralement). |
| `EnableScoresEvent` | false | Déclencher `OnScores` (top-K) pour chaque fenêtre, pour un indicateur en direct. |
| `TopK` | 5 | Nombre de classes rapportées dans l'événement de scores. |

## Filtrer vers des sons spécifiques

Passez une liste d'autorisation `ClassFilter` pour ne rapporter que les sons qui vous intéressent — par
exemple, un veilleur d'alarme qui ignore tout le reste :

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    ClassFilter = new[] { "Siren", "Emergency vehicle", "Police car (siren)", "Alarm", "Glass", "Gunshot, gunfire" },
};
```

Les noms de classe sont les noms d'affichage AudioSet et doivent correspondre exactement à l'un d'eux
(la casse est ignorée, mais une faute de frappe, un singulier/pluriel différent ou une abréviation ne
le sont pas). Les noms inconnus sont ignorés avec un avertissement **tant qu'au moins un nom est
résolu**. Un filtre non vide dont **aucun** nom n'est résolu fait échouer la construction du bloc —
`StartAsync` renvoie `false` et une erreur nommant les entrées non résolues est signalée — au lieu de
se rabattre silencieusement sur la détection des 521 classes.

## Indicateur de scores en direct

Activez `OnScores` pour recevoir les scores top-K de chaque fenêtre traitée (utile pour un indicateur
de niveau en direct ou une vue de débogage) :

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    EnableScoresEvent = true, // lu au démarrage du pipeline — définissez-le avant StartAsync
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

## Modèle

Le bloc utilise **YAMNet**, le classifieur audio MobileNet-v1 de Google entraîné sur
[AudioSet](https://research.google.com/audioset/), qui prédit 521 classes de l'ontologie AudioSet. Le
modèle consomme une forme d'onde mono 16&nbsp;kHz (le bloc gère le rééchantillonnage et le mixage
mono) et renvoie des scores de classe par trame. Les poids sont sous Apache-2.0. Votre application
fournit le fichier de modèle et fait pointer `ModelPath` dessus : les paquets NuGet du SDK ne
contiennent ni les poids ni un téléchargeur, et le bloc ne récupère jamais rien lui-même. La démo
ci-dessous propose un bouton **Download** qui récupère le modèle une fois et le met en cache ; rien
n'est téléchargé tant que vous ne cliquez pas dessus.

**Pour la production, convertissez vous-même le modèle officiel TF-Hub de Google en ONNX** plutôt que de
dépendre d'une exportation ONNX tierce. La conversion n'est qu'un changement de format et ne modifie pas
la licence Apache-2.0 des poids :

```bash
pip install tensorflow tensorflow-hub tf2onnx onnx
python -c "import tensorflow_hub as hub, tensorflow as tf; m=hub.load('https://tfhub.dev/google/yamnet/1'); tf.saved_model.save(m, 'yamnet_tf')"
python -m tf2onnx.convert --saved-model yamnet_tf --output yamnet.onnx --opset 13
```

Si vous utilisez une exportation ONNX communautaire, épinglez-la à une révision immuable et vérifiez son
SHA-256 — la démo WPF [Audio Event Detection Demo](#demos) télécharge une exportation épinglée des poids
Apache-2.0 et vérifie l'empreinte après le téléchargement.

> La reconnaissance d'événements sonores utilise l'ontologie AudioSet et le modèle YAMNet de Google,
> publié sous la licence Apache-2.0.

## Démos

La démo `Audio Event Detection Demo` (Media Blocks pour WPF — téléchargement du modèle avec
vérification SHA-256, filtre de classes, journal d'événements en direct) figure dans l'ensemble de
démos du SDK et sera liée ici une fois publiée dans le dépôt d'échantillons public.

## Remarques

- `OnAudioEvent` et `OnScores` sont déclenchés sur le thread de travail d'inférence ; passez au thread
  UI avant de mettre à jour l'UI. Un gestionnaire qui lève une exception est capturé et journalisé,
  jamais propagé sur le thread de travail.
- En fin de flux, la fenêtre finale et tous les événements encore actifs sont vidés avant le démontage.
- En fonctionnement normal, le bloc ne supprime jamais l'audio, ni du chemin de *passage direct* ni de
  celui d'analyse. La file d'analyse interne n'est pas bornée : une source qui dépasse le thread de
  travail fait croître le retard accumulé au lieu de supprimer des échantillons, et l'audio continue de
  passer en aval sans modification.
- Deux sorties prévues mettent fin à l'analyse plutôt que de perdre de l'audio en silence, et aucune ne
  touche jamais le chemin de *passage direct* : si le retard accumulé ne tient plus en mémoire, le bloc
  signale une erreur une seule fois et cesse d'analyser ce flux (utilisez un fournisseur d'exécution GPU
  ou du matériel plus rapide) ; et un arrêt explicite abandonne le retard accumulé que le thread de
  travail n'a pas atteint — seule une fin de flux naturelle le vide entièrement.
