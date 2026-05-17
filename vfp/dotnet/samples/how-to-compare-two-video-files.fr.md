---
title: Comparer des fichiers vidéo avec l'empreinte vidéo en .NET
description: Comparez deux fichiers vidéo pour la similarité avec le VisioForge Video Fingerprinting SDK en C# — analyse d'images, signatures, exemples de code.
tags:
  - Video Fingerprinting SDK
  - .NET
  - DirectShow
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

# Techniques et méthodes de comparaison de fichiers vidéo

## Introduction à l'empreinte vidéo

Le Video Fingerprinting SDK fournit des outils puissants pour comparer avec précision les fichiers vidéo à l'aide d'une technologie d'empreinte avancée. Cette approche analyse les images vidéo et les échantillons audio pour générer des signatures uniques qui représentent le contenu. Ces signatures peuvent ensuite être comparées pour déterminer la similarité entre différents fichiers vidéo.

## Comprendre le processus de comparaison

L'empreinte vidéo fonctionne en extrayant les caractéristiques distinctives des images vidéo et des échantillons audio, créant ainsi une représentation compacte pouvant être stockée et comparée efficacement. Cette technique est particulièrement utile pour :

- Détecter du contenu en doublon ou similaire
- Identifier des versions modifiées de vidéos
- Vérifier et authentifier du contenu
- Protéger les droits d'auteur et détecter les infractions

## Implémentation de la comparaison vidéo en .NET

### Création d'empreintes pour la première vidéo

La première étape consiste à générer une empreinte pour votre fichier vidéo initial. Le code suivant montre comment créer une source à l'aide du moteur DirectShow et limiter l'analyse aux 5 premières secondes :

```csharp
// VFPFingerprintSource n'a qu'un constructeur (string filename) — il n'existe pas
// de surcharge avec sélection de moteur. Choisissez la fenêtre d'analyse via StartTime/StopTime.
var source1 = new VFPFingerprintSource(File1);
source1.StopTime = TimeSpan.FromMilliseconds(5000);

// VFPAnalyzer.GetComparingFingerprintForVideoFile n'a qu'une variante Async.
var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1, ErrorCallback);
```

### Génération d'empreintes pour la seconde vidéo

De même, nous devons créer une empreinte pour le second fichier vidéo pour permettre la comparaison :

```csharp
// Même constructeur à argument unique + getter async pour la seconde vidéo.
var source2 = new VFPFingerprintSource(File2);
source2.StopTime = TimeSpan.FromMilliseconds(5000);

var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2, ErrorCallback);
```

### Comparaison des empreintes vidéo

Une fois les deux empreintes générées, vous pouvez les comparer pour déterminer la similarité entre les vidéos :

```csharp
// comparer la première et la seconde empreinte
var res = VFPCompare.Compare(fp1, fp2, 500);

// vérifier le résultat
if (res < 300)
{
    Console.WriteLine("Les fichiers d'entrée sont similaires.");
}
else
{
    Console.WriteLine("Les fichiers d'entrée sont différents.");
}
```

Le résultat de la comparaison est une valeur numérique représentant la différence entre les vidéos. Les valeurs plus basses indiquent une plus grande similarité.

## Optimisation du processus de comparaison

### Stockage des empreintes pour une utilisation répétée

Pour améliorer l'efficacité, vous pouvez enregistrer les empreintes dans des fichiers binaires pour une utilisation future sans avoir besoin de réanalyser les vidéos :

```csharp
VFPFingerPrint fp1 = ...;
fp1.Save(filename);
```

### Exigences de stockage et intégration de base de données

Les fichiers d'empreinte sont compacts, nécessitant environ 250 Ko d'espace disque par minute de vidéo. Pour les applications qui doivent stocker et comparer de nombreuses empreintes, l'intégration MongoDB est disponible via les extensions du SDK.

## Applications avancées

La technologie d'empreinte vidéo offre de nombreuses applications pratiques :

- Systèmes d'identification de contenu
- Surveillance automatisée des droits d'auteur
- Gestion d'actifs médias
- Déduplication vidéo dans de grandes archives
- Surveillance et vérification de diffusion

## Ressources supplémentaires

[Page produit](https://www.visioforge.com/video-fingerprinting-sdk)
