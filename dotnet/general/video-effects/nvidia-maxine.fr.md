---
title: "Amélioration vidéo par IA NVIDIA Maxine en C# .NET"
description: Améliorez la vidéo avec l'IA NVIDIA Maxine en C# .NET — super-résolution, upscaling, débruitage et réduction d'artefacts sur GPU RTX.
sidebar_label: NVIDIA Maxine
tags:
  - Video Capture SDK
  - Media Player SDK
  - .NET
  - NVIDIA Maxine
  - AI
  - Super Resolution
  - Windows
primary_api_classes:
  - MaxineDenoiseVideoEffect
  - MaxineArtifactReductionVideoEffect
  - MaxineSuperResSettings
  - MaxineUpscaleSettings
---

# Amélioration vidéo par IA NVIDIA Maxine en C# .NET

NVIDIA Maxine apporte une amélioration vidéo par IA accélérée par GPU aux moteurs classiques de VisioForge
(`VideoCaptureCore` et `MediaPlayerCore`). Les effets s'exécutent sur le NVIDIA Maxine SDK et nécessitent un
**GPU NVIDIA RTX** (cœurs Tensor). Ils sont réservés à Windows.

| Effet | Type d'API | Appliqué via |
| --- | --- | --- |
| Débruitage | `MaxineDenoiseVideoEffect` | `Video_Effects_Add` |
| Réduction d'artefacts | `MaxineArtifactReductionVideoEffect` | `Video_Effects_Add` |
| Super-résolution | `MaxineSuperResSettings` | `Video_Resize` |
| Upscaling | `MaxineUpscaleSettings` | `Video_Resize` |

## Prérequis

- GPU NVIDIA RTX (GeForce RTX 2060 ou supérieur) avec des pilotes à jour.
- Le NVIDIA Maxine SDK avec son **répertoire de modèles** sur disque — la plupart des constructeurs prennent le chemin vers celui-ci.
- Windows 10/11. Ces effets ciblent les moteurs `VideoCaptureCore` / `MediaPlayerCore` basés sur DirectShow.

Les classes d'effet de débruitage/artefacts se trouvent dans `VisioForge.Core.Types.VideoEffects` (débruitage dans
`VisioForge.Core.Types.VideoEffects.NvidiaMaxine`) ; les réglages de super-résolution et d'upscaling se trouvent dans
`VisioForge.Core.Types`.

!!! note "Activez le pipeline d'effets"
    Les effets de filtre ajoutés avec `Video_Effects_Add` ne s'exécutent que lorsque le pipeline d'effets est activé. Définissez
    `Video_Effects_Enabled = true` une fois avant d'ajouter des effets — il vaut `false` par défaut.

## Débruitage

Supprime le bruit caméra/capteur tout en préservant le détail. `Strength` va de 0.0 à 1.0 (par défaut 0.7).

```csharp
using VisioForge.Core.Types.VideoEffects.NvidiaMaxine;

VideoCapture1.Video_Effects_Enabled = true; // les effets sont désactivés par défaut

var denoise = new MaxineDenoiseVideoEffect(modelsDir, strength: 0.7f);
VideoCapture1.Video_Effects_Add(denoise);
```

## Réduction d'artefacts

Supprime les artefacts de compression (blocs, ringing, banding). Choisissez le mode selon le débit de la source.

```csharp
using VisioForge.Core.Types.VideoEffects;

VideoCapture1.Video_Effects_Enabled = true;

var artifactReduction = new MaxineArtifactReductionVideoEffect(
    modelsDir,
    mode: MaxineArtifactReductionEffectMode.LowBitrate);
VideoCapture1.Video_Effects_Add(artifactReduction);
```

| `MaxineArtifactReductionEffectMode` | À utiliser pour |
| --- | --- |
| `HighBitrate` | Sources à 10+ Mbps ; plus doux, préserve les dégradés et le détail fin. |
| `LowBitrate` | En dessous d'environ 5 Mbps ; suppression agressive des artefacts marqués (par défaut). |

## Super-résolution

Upscaling par IA vers une hauteur cible. Affectez les réglages à `Video_Resize` et activez le redimensionnement/recadrage. La largeur est
calculée pour préserver le rapport d'aspect.

```csharp
using VisioForge.Core.Types;

VideoCapture1.Video_Resize = new MaxineSuperResSettings(modelsDir, height: 2160)
{
    Mode = MaxineSuperResolutionEffectMode.HQSource,
};
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

| `MaxineSuperResolutionEffectMode` | À utiliser pour |
| --- | --- |
| `HQSource` | Sources de haute qualité/à haut débit ; privilégie la suppression d'artefacts (par défaut). |
| `LQSource` | Sources fortement compressées ; privilégie l'amélioration du détail. |

## Upscaling

Un upscaler plus léger (par rapport à la super-résolution) avec une `Strength` ajustable (0.0–1.0, par défaut 0.4).

```csharp
using VisioForge.Core.Types;

VideoCapture1.Video_Resize = new MaxineUpscaleSettings(modelsDir, height: 1080, strength: 0.4f);
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

## Moteur Media Player

Les mêmes effets s'appliquent au moteur `MediaPlayerCore` pour l'amélioration par IA pendant la lecture. Définissez
`MediaPlayer1.Video_Effects_Enabled = true`, puis utilisez `MediaPlayer1.Video_Effects_Add(...)` pour
le débruitage/les artefacts, et affectez `MediaPlayer1.Video_Resize` pour la super-résolution/l'upscaling. Le lecteur n'a
pas d'indicateur `Video_ResizeOrCrop_Enabled` — affectez `Video_Resize` avant de démarrer la lecture ; il est appliqué
lors de la construction du graphe de lecture (le modifier pendant la lecture active ne prend effet qu'au prochain démarrage).

## Démos

- **Nvidia Maxine Demo** (Video Capture, WPF) — [Nvidia Maxine Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Nvidia%20Maxine%20Demo).
- **Nvidia Maxine Player** (Media Player, WPF) — [Nvidia Maxine Player](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Nvidia%20Maxine%20Player).

## Voir aussi

- [Référence des effets](effects-reference.md) — catalogue complet des effets CPU, GPU et IA.
- [Ajout d'effets](add.md)
