---
title: Recherche de fragment vidéo par empreinte en C# .NET
description: Trouvez des fragments vidéo dans des fichiers plus grands avec le VisioForge Video Fingerprinting SDK en C# .NET — implémentation pas à pas et exemples.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - C#
primary_api_classes:
  - VFPFingerprintSource
  - VFSimplePlayerEngine
  - VFPAnalyzer

video:
  id: MM3IJxIxwww
  upload_date: 2026-11-24T09:52:31Z
  duration: PT1M8S
---

# Recherche de fragments vidéo dans un contenu vidéo plus volumineux

## Introduction

La technologie d'empreinte vidéo permet aux développeurs d'identifier et de localiser des segments vidéo spécifiques dans des fichiers vidéo plus grands. Ce guide démontre le processus d'implémentation à l'aide d'un puissant Video Fingerprinting SDK fonctionnant avec divers formats vidéo et niveaux de qualité.

Les principaux exemples de ce tutoriel utilisent l'implémentation de l'API .NET, mais une fonctionnalité équivalente est disponible via l'API C++ pour les développeurs préférant les solutions en code natif.

## Processus d'implémentation

### Étape 1 : analyser le fichier fragment

Tout d'abord, nous devons extraire une empreinte du plus petit fragment vidéo que nous voulons localiser dans la vidéo plus grande. Ce processus implique d'analyser les caractéristiques uniques de la vidéo et de générer une signature numérique.

```csharp
// VFPFingerprintSource n'a qu'un constructeur (string filename) — il n'existe pas
// de surcharge avec sélection de moteur. Limitez la fenêtre d'analyse via StartTime/StopTime.
var fragmentSrc = new VFPFingerprintSource(ShortFile);
fragmentSrc.StopTime = TimeSpan.FromMilliseconds(5000);

// VFPAnalyzer.GetSearchFingerprintForVideoFile n'a qu'une variante Async.
var fragment = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(fragmentSrc, ErrorCallback);
```

Dans ce bloc de code, nous :

- Créons une source d'empreinte pointant vers le fichier fragment court
- Définissons une durée limite de 5 secondes pour l'analyse
- Générons une empreinte de recherche à l'aide de l'analyseur

### Étape 2 : analyser la vidéo cible

Ensuite, nous devons extraire une empreinte de la vidéo plus grande dans laquelle nous chercherons notre fragment. Le processus est similaire, mais sans limitations de temps.

```csharp
// Même constructeur à argument unique + getter async pour le fichier plus long.
var mainSrc = new VFPFingerprintSource(LongFile);

var main = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(mainSrc, ErrorCallback);
```

### Étape 3 : configurer la gestion des erreurs

Pour maintenir une gestion d'erreurs robuste, nous implémentons une fonction de callback qui capture et affiche toutes les erreurs rencontrées pendant le processus d'empreinte.

```csharp
private static void ErrorCallback(string error)
{
    Console.WriteLine(error);
}
```

### Étape 4 : effectuer l'opération de recherche

Avec les deux empreintes prêtes, nous pouvons maintenant rechercher le fragment dans le fichier vidéo plus grand.

```csharp
// définir le niveau de différence maximum
var maxDifference = 500;

// rechercher un fragment vidéo dans une autre vidéo à l'aide des empreintes
var res = VFPSearch.Search(fragment, 0, main, 0, out var difference, maxDifference);

// vérifier le résultat
if (res != int.MaxValue)
{
    TimeSpan ts = new TimeSpan(res * TimeSpan.TicksPerSecond);
    Console.WriteLine($"Fichier fragment détecté à {ts:g}, niveau de différence : {difference}");
}
else
{
    Console.WriteLine("Fichier fragment introuvable.");
}
```

Dans ce code :

- Nous définissons un niveau de tolérance pour les différences entre empreintes
- Effectuons l'opération de recherche entre notre fragment et la vidéo principale
- Vérifions si une position correspondante a été trouvée (`Search` renvoie `int.MaxValue` lorsque le fragment est introuvable ; 0 signifie une correspondance au tout début de la vidéo)
- Convertissons le résultat en horodatage et l'affichons avec la valeur de différence

### Trouver chaque correspondance en quelques lignes

Une application console complète : elle trouve chaque endroit où `clip.mp4` apparaît dans `movie.mp4` et affiche le début de chaque correspondance. `VFPAnalyzer.SearchAsync` reçoit les deux empreintes, la durée de l'extrait, la limite de différence (ici, une correspondance doit rester sous 500) et `true` pour renvoyer toutes les correspondances et pas seulement la meilleure, et renvoie le début de chaque correspondance sous forme de `List<TimeSpan>`. `GetSearchFingerprintForVideoFileAsync` renvoie `null` lorsqu'un fichier ne peut pas être analysé et affiche la raison via le callback ; un fichier manquant fait lever `FileNotFoundException` au constructeur de `VFPFingerprintSource`. Un extrait de 10 secondes en 480p a été trouvé aux cinq endroits de 2 heures 18 minutes de vidéo 720p, en neuf minutes environ.

```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.VideoFingerPrinting;
using static VisioForge.Core.VideoFingerPrinting.VFPAnalyzer;

await VisioForgeX.InitSDKAsync();
SetLicenseKey("TRIAL");
var clip = await
    GetSearchFingerprintForVideoFileAsync(
        new VFPFingerprintSource("clip.mp4"),
        Console.WriteLine);
var movie = await
    GetSearchFingerprintForVideoFileAsync(
        new VFPFingerprintSource("movie.mp4"),
        Console.WriteLine);
if (clip != null && movie != null)
{
    var found = await SearchAsync(
        clip, movie, clip.Duration, 500, true);
    foreach (var at in found)
        Console.WriteLine(
            $"Found at {at:hh\\:mm\\:ss}");
    if (found.Count == 0)
        Console.WriteLine("Not found");
}
VisioForgeX.DestroySDK();
```

Le projet complet est [find-clip-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/_VideoTutorials/find-clip-short-console), une application console Windows x64 qui référence `VisioForge.DotNet.Core.Redist.VideoFingerprinting` et les paquets natifs du SDK pour cette plateforme, `VisioForge.CrossPlatform.Core.Windows.x64` et `VisioForge.CrossPlatform.Libav.Windows.x64`. Les noms de fichier sont relatifs au répertoire de travail : placez donc les deux fichiers dans le dossier depuis lequel vous lancez l'application. Sans clé de licence, le SDK fonctionne en mode d'essai et affiche une fois le message « Unregistered version » ; cliquez sur OK et la recherche continue.

La vidéo d'une minute écrit et exécute cette application.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/MM3IJxIxwww?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Considérations de performance

La technologie d'empreinte utilise des algorithmes sophistiqués qui équilibrent précision et performance. Pour des résultats optimaux :

- Envisagez d'ajuster le niveau de différence maximum selon vos exigences spécifiques
- Traitez les vidéos à leur résolution native lorsque possible
- Pour les très gros fichiers, envisagez de diviser la recherche en blocs gérables

## Ressources supplémentaires

Pour la documentation complète, les exemples d'implémentation dans d'autres langages et les informations de licence, consultez la [page produit](https://www.visioforge.com/video-fingerprinting-sdk).
