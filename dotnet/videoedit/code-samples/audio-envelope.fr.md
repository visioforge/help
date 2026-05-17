---
title: Effets d'enveloppe de volume audio pour l'édition vidéo .NET
description: Mettez en œuvre des effets d'enveloppe de volume audio avec VisioForge Video Edit SDK .NET. Fondu d'entrée, de sortie et contrôle par image-clé.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - Effects
  - C#
  - NuGet
primary_api_classes:
  - AudioVolumeEnvelopeEffect
  - AudioSource
  - AudioTrackEffect

---

# Mise en œuvre d'effets d'enveloppe de volume audio en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

Les enveloppes de volume audio sont des outils essentiels pour la production vidéo professionnelle, permettant aux développeurs de contrôler avec précision les niveaux audio sur toute une timeline. Ce tutoriel illustre comment mettre en œuvre ces effets dans vos applications .NET.

## Qu'est-ce qu'une enveloppe de volume audio ?

Une enveloppe de volume audio vous permet d'ajuster les niveaux de volume de votre piste audio. Plutôt que d'ajuster manuellement le volume tout au long du processus d'édition, les enveloppes fournissent une méthode programmatique pour définir des niveaux de volume cohérents. Ceci est particulièrement utile lorsque vous travaillez avec plusieurs pistes audio qui doivent maintenir des relations de volume spécifiques.

## Vue d'ensemble de l'implémentation

Le processus d'implémentation comporte trois étapes clés :

1. Créer une source audio à partir de votre fichier
2. Créer l'effet d'enveloppe de volume avec le niveau souhaité
3. Ajouter l'audio avec l'effet à votre timeline

Chaque étape nécessite des composants de code spécifiques que nous explorerons en détail ci-dessous.

## Comprendre la classe AudioVolumeEnvelopeEffect

La classe `AudioVolumeEnvelopeEffect` est le composant principal pour mettre en œuvre le contrôle du volume :

```cs
public class AudioVolumeEnvelopeEffect : AudioTrackEffect
{
    /// <summary>
    /// Gets or sets level (in percents), range is [0-100].
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioVolumeEnvelopeEffect class. 
    /// </summary>
    /// <param name="level">
    /// Level (in percents), range is [0-100].
    /// </param>
    public AudioVolumeEnvelopeEffect(int level) 
    {
        Level = level;
    }
}
```

Comme vous pouvez le voir, cette classe :

- Hérite de `AudioTrackEffect`
- Possède une propriété `Level` qui accepte des valeurs de 0 à 100 (représentant le pourcentage de volume)
- Fournit un constructeur pour définir le niveau initial

## Étapes d'implémentation détaillées

### 1. Création de votre source audio

La première étape consiste à initialiser un objet source audio qui référence votre fichier audio. Cet objet sert de base pour appliquer les effets.

```cs
var audioFile = new AudioSource(file, segments, null);
```

Dans ce code :

- `file` est le chemin vers votre fichier audio
- `segments` définit des segments temporels si vous n'utilisez que des portions de l'audio
- Le paramètre final peut contenir des options supplémentaires (null dans cet exemple basique)

### 2. Configuration de l'effet d'enveloppe de volume

Ensuite, créez et configurez l'effet d'enveloppe de volume en spécifiant le niveau de volume souhaité :

```cs
var envelope = new AudioVolumeEnvelopeEffect(70);
```

Ceci crée un effet d'enveloppe de volume réglé à 70 %. Le paramètre accepte des valeurs de 0 à 100 :

- 0 = silence complet
- 50 = demi-volume
- 100 = volume complet

Vous pouvez également ajuster le niveau après la création :

```cs
var envelope = new AudioVolumeEnvelopeEffect(50);
envelope.Level = 75; // Changé à 75 % de volume
```

### 3. Ajout de l'audio avec effet d'enveloppe à la timeline

L'étape finale consiste à ajouter votre source audio avec l'effet d'enveloppe appliqué à la timeline de votre projet :

```cs
VideoEdit1.Input_AddAudioFile(
    audioFile,                        // Votre source audio configurée
    TimeSpan.FromMilliseconds(0),     // Position de départ sur la timeline
    0,                                // Index de la piste
    new []{ envelope }                // Tableau d'effets à appliquer
);
```

Ceci positionne votre audio au début de la timeline (0 ms) et applique l'effet d'enveloppe que nous avons configuré précédemment.

## Cas d'usage courants

### Normalisation des niveaux audio

Lorsque vous travaillez avec de l'audio provenant de sources différentes, la normalisation garantit des niveaux de volume cohérents :

```cs
// Audio principal d'interview au volume complet
// AudioSource ctor #1 : (filename, startTime?, stopTime?, fileToSync?, streamNumber, rate)
var interviewAudio = new AudioSource("interview.mp3", (TimeSpan?)null, (TimeSpan?)null);
VideoEdit1.Input_AddAudioFile(interviewAudio, TimeSpan.Zero, 0, null);

// Musique d'arrière-plan à 30 % de volume pour ne pas couvrir la parole
var backgroundMusic = new AudioSource("background.mp3", (TimeSpan?)null, (TimeSpan?)null);
var musicEnvelope = new AudioVolumeEnvelopeEffect(30);
VideoEdit1.Input_AddAudioFile(backgroundMusic, TimeSpan.Zero, 1, new[] { musicEnvelope });
```

### Mise en sourdine de sections spécifiques

Si vous devez mettre en sourdine des sections audio dans votre timeline, vous pouvez créer et appliquer différents effets d'enveloppe :

```cs
// Créer des sources audio pour différents segments avec AudioSource(filename, FileSegment[], fileToSync, streamNumber, rate)
var segment1 = new AudioSource("audio.mp3",
    new[] { new FileSegment(TimeSpan.FromMilliseconds(0),     TimeSpan.FromMilliseconds(10000)) }, null); // 0-10s
var segment2 = new AudioSource("audio.mp3",
    new[] { new FileSegment(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(15000)) }, null); // 10-15s
var segment3 = new AudioSource("audio.mp3",
    new[] { new FileSegment(TimeSpan.FromMilliseconds(15000), TimeSpan.FromMilliseconds(30000)) }, null); // 15-30s

// Appliquer différents niveaux de volume
VideoEdit1.Input_AddAudioFile(segment1, TimeSpan.Zero, 0, new[] { new AudioVolumeEnvelopeEffect(100) });
// Mettre en sourdine le segment intermédiaire
VideoEdit1.Input_AddAudioFile(segment2, TimeSpan.FromMilliseconds(10000), 0, new[] { new AudioVolumeEnvelopeEffect(0) });
VideoEdit1.Input_AddAudioFile(segment3, TimeSpan.FromMilliseconds(15000), 0, new[] { new AudioVolumeEnvelopeEffect(100) });
```

## Dépendances requises

Pour mettre en œuvre des effets d'enveloppe audio, vous aurez besoin de :

- Paquets redistribuables Video Edit SDK .NET :
  - [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Vous pouvez installer ces paquets via le gestionnaire de paquets NuGet :

```nuget
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
```

Pour plus d'informations sur le déploiement de ces dépendances sur les systèmes des utilisateurs, consultez notre [documentation de déploiement](../deployment.md).

## Considérations de performance

Lors de la mise en œuvre d'effets de volume audio, tenez compte de ces conseils de performance :

- Appliquez les effets d'enveloppe pendant la phase d'édition/rendu plutôt qu'à l'exécution
- Lorsque vous travaillez avec plusieurs pistes, tenez compte de l'effet cumulé de tout le traitement audio
- Testez sur votre matériel cible pour garantir une lecture fluide

## Dépannage des problèmes courants

Si vous rencontrez des problèmes avec votre implémentation d'enveloppe audio :

- Vérifiez que les chemins et formats des fichiers audio sont pris en charge
- Vérifiez que les pourcentages de volume se situent dans la plage 0-100
- Assurez-vous que l'effet audio est correctement ajouté au tableau d'effets
- Vérifiez que le positionnement sur la timeline ne crée pas de conflits entre les segments audio

## Conclusion

Les effets d'enveloppe de volume audio fournissent un contrôle essentiel sur l'expérience audio de votre application. En suivant ce guide, vous avez appris à mettre en œuvre le contrôle du volume dans vos projets d'édition vidéo .NET, en équilibrant différentes sources audio pour des résultats professionnels.

---
Pour davantage d'exemples de code et des techniques avancées, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).