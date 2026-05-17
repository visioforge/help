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
if (res > 0)
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
- Vérifions si une position correspondante a été trouvée (valeur de résultat positive)
- Convertissons le résultat en horodatage et l'affichons avec la valeur de différence

## Considérations de performance

La technologie d'empreinte utilise des algorithmes sophistiqués qui équilibrent précision et performance. Pour des résultats optimaux :

- Envisagez d'ajuster le niveau de différence maximum selon vos exigences spécifiques
- Traitez les vidéos à leur résolution native lorsque possible
- Pour les très gros fichiers, envisagez de diviser la recherche en blocs gérables

## Ressources supplémentaires

Pour la documentation complète, les exemples d'implémentation dans d'autres langages et les informations de licence, consultez la [page produit](https://www.visioforge.com/video-fingerprinting-sdk).
