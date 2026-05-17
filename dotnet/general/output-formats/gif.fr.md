---
title: Encodage de vidéo en GIF animé dans les applications .NET
description: Créez des animations GIF à partir de vidéos en .NET avec contrôle de la fréquence d'images, paramètres de résolution et optimisation multiplateforme.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Encoding
  - Editing
  - GIF
  - C#
primary_api_classes:
  - AnimatedGIFOutput
  - GIFEncoderSettings
  - VideoCaptureCoreX
  - VideoEditCoreX
  - VideoCaptureCore

---

# Encodeur GIF

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

L'encodeur GIF est un composant du SDK VisioForge qui permet l'encodage vidéo au format GIF. Ce document fournit des informations détaillées sur les paramètres de l'encodeur GIF et les lignes directrices de son implémentation.

## Sortie GIF multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Les paramètres de l'encodeur GIF sont gérés par la classe `GIFEncoderSettings`, qui fournit des options de configuration pour contrôler le processus d'encodage.

### Propriétés

1. **Repeat**
   - Type : `uint`
   - Description : contrôle le nombre de fois que l'animation GIF sera répétée
   - Valeurs :
     - `-1` : boucle infinie
     - `0..n` : nombre fini de répétitions

2. **Speed**
   - Type : `int`
   - Description : contrôle la vitesse d'encodage
   - Plage : 1 à 30 (les valeurs supérieures aboutissent à un encodage plus rapide)
   - Valeur par défaut : 10

## Guide d'implémentation

### Utilisation de base

Voici un exemple de base de configuration et d'utilisation de l'encodeur GIF :

```csharp
using VisioForge.Core.Types.X.VideoEncoders;

// Créer et configurer les paramètres de l'encodeur GIF
var settings = new GIFEncoderSettings
{
    Repeat = 0,      // Lecture unique
    Speed = 15       // Régler la vitesse d'encodage à 15
};
```

### Configuration avancée

Pour un encodage GIF plus contrôlé, vous pouvez ajuster les paramètres selon vos besoins spécifiques :

```csharp
// Configurer un GIF en boucle infinie avec vitesse d'encodage maximale
var settings = new GIFEncoderSettings
{
    Repeat = uint.MaxValue,  // Boucle infinie
    Speed = 30              // Vitesse d'encodage maximale
};

// Configurer pour une qualité optimale
var qualitySettings = new GIFEncoderSettings
{
    Repeat = 1,    // Lecture deux fois
    Speed = 1      // Vitesse d'encodage la plus lente pour la meilleure qualité
};
```

## Bonnes pratiques

1. **Choix de la vitesse**
   - Pour la meilleure qualité, utilisez de faibles valeurs de vitesse (1-5)
   - Pour un équilibre entre qualité et performance, utilisez des valeurs moyennes (6-15)
   - Pour l'encodage le plus rapide, utilisez des valeurs élevées (16-30)

2. **Considérations mémoire**
   - Les valeurs de vitesse élevées consomment davantage de mémoire pendant l'encodage
   - Pour les vidéos volumineuses, envisagez d'utiliser des valeurs de vitesse plus faibles pour gérer l'utilisation mémoire

3. **Configuration de la boucle** (`Repeat` est un `uint`)
   - Utilisez `Repeat = uint.MaxValue` pour les boucles infinies (la docstring indique « -1 = loop forever » mais comme le type de la propriété est `uint`, `-1` ne compilera pas)
   - Définissez des comptes de répétition spécifiques pour les GIF de style présentation
   - Utilisez `Repeat = 0` pour les GIF à lecture unique

## Optimisation des performances

Lors de l'encodage de vidéos au format GIF, envisagez ces stratégies d'optimisation :

```csharp
// Optimiser pour la diffusion web
var webOptimizedSettings = new GIFEncoderSettings
{
    Repeat = uint.MaxValue,  // Boucle infinie pour la lecture web
    Speed = 20              // Encodage rapide pour le contenu web
};

// Optimiser pour la qualité
var qualityOptimizedSettings = new GIFEncoderSettings
{
    Repeat = 1,    // Une seule répétition
    Speed = 3      // Encodage plus lent pour une meilleure qualité
};
```

### Exemple d'implémentation

Voici un exemple complet montrant comment configurer la sortie GIF :

Construisez d'abord le `GIFOutput` (moteurs X) avec les paramètres de l'encodeur :

```csharp
var gifOutput = new GIFOutput("output.gif");
gifOutput.Settings = new GIFEncoderSettings
{
    Repeat = uint.MaxValue,  // Boucle infinie
    Speed = 4,
};
```

Ajoutez la sortie GIF à l'instance core Video Capture SDK :

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(gifOutput, true);
```

Ou attribuez-la comme `Output_Format` d'une timeline `VideoEditCoreX` :

```csharp
var editor = new VideoEditCoreX();
editor.Input_AddVideoFile("input.mp4");
editor.Output_Format = gifOutput;
editor.Start();
```

!!! note "La sortie GIF est uniquement vidéo"

    `GIFOutput` implémente à la fois `IVideoCaptureXBaseOutput` et
    `IVideoEditXBaseOutput`, donc elle fonctionne avec `VideoCaptureCoreX` et
    `VideoEditCoreX`. Il n'y a pas de multiplexeur — `gifenc` écrit directement
    le fichier `.gif` final. Les pistes audio de la timeline sont supprimées, et
    `SmartRender = true` est incompatible avec la sortie GIF (aucun clip source
    n'est déjà en `image/gif`). Pour les pipelines qui nécessitent un contrôle
    plus fin sur le placement de l'encodeur, l'extrait Media Blocks
    `GIFEncoderBlock` ci-dessous reste disponible.

Créez une instance de sortie GIF Media Blocks :

```csharp
var gifSettings = new GIFEncoderSettings();
var gifEncoderBlock = new GIFEncoderBlock(gifSettings, "output.gif");
```

## Sortie GIF uniquement Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La classe `AnimatedGIFOutput` est une classe de configuration spécialisée dans l'espace de noms `VisioForge.Core.Types.Output` qui gère les paramètres pour la génération de fichiers GIF animés. Cette classe est conçue pour fonctionner aussi bien avec les opérations de capture vidéo que d'édition vidéo, en implémentant à la fois les interfaces `IVideoEditBaseOutput` et `IVideoCaptureBaseOutput`.

L'objectif principal de cette classe est de fournir un conteneur de configuration pour contrôler la manière dont le contenu vidéo est converti au format GIF animé. Elle permet aux utilisateurs de spécifier des paramètres clés tels que la fréquence d'images et les dimensions de sortie, qui sont cruciaux pour créer des GIF animés optimisés à partir de sources vidéo.

### Propriétés

#### ForcedVideoHeight

- Type : `int`
- Objectif : spécifie une hauteur forcée pour le GIF de sortie
- Utilisation : définissez cette propriété lorsque vous avez besoin de redimensionner le GIF de sortie à une hauteur spécifique, indépendamment des dimensions de la vidéo d'entrée
- Exemple : `gifOutput.ForcedVideoHeight = 480;`

#### ForcedVideoWidth

- Type : `int`
- Objectif : spécifie une largeur forcée pour le GIF de sortie
- Utilisation : définissez cette propriété lorsque vous avez besoin de redimensionner le GIF de sortie à une largeur spécifique, indépendamment des dimensions de la vidéo d'entrée
- Exemple : `gifOutput.ForcedVideoWidth = 640;`

#### FrameRate

- Type : `VideoFrameRate`
- Valeur par défaut : 2 images par seconde
- Objectif : contrôle le nombre d'images par seconde que le GIF de sortie contiendra
- Remarque : la valeur par défaut de 2 fps est choisie pour équilibrer la taille du fichier et la fluidité de l'animation dans une utilisation GIF typique

### Constructeur

```csharp
public AnimatedGIFOutput()
```

Le constructeur initialise une nouvelle instance avec les paramètres par défaut :

- Définit la fréquence d'images à 2 fps via `new VideoFrameRate(2)`
- Toutes les autres propriétés sont initialisées à leurs valeurs par défaut

### Méthodes de sérialisation

#### Save()

- Retourne : `string`
- Objectif : sérialise la configuration actuelle au format JSON
- Utilisation : utilisez cette méthode lorsque vous devez enregistrer ou transférer la configuration
- Exemple :
  
```csharp
var gifOutput = new AnimatedGIFOutput();
gifOutput.ForcedVideoWidth = 800;
string jsonConfig = gifOutput.Save();
```

#### Load(string json)

- Paramètres : `json` — chaîne JSON contenant la configuration sérialisée
- Retourne : `AnimatedGIFOutput`
- Objectif : crée une nouvelle instance à partir d'une chaîne de configuration JSON
- Utilisation : utilisez cette méthode pour restaurer une configuration précédemment enregistrée
- Exemple :
  
```csharp
string jsonConfig = "..."; // Votre configuration JSON enregistrée
var gifOutput = AnimatedGIFOutput.Load(jsonConfig);
```

### Bonnes pratiques et lignes directrices d'utilisation

1. Considérations sur la fréquence d'images
   - La valeur par défaut de 2 fps convient à la plupart des animations basiques
   - Augmentez la fréquence d'images pour des animations plus fluides, mais soyez conscient des implications sur la taille du fichier
   - Envisagez des fréquences d'images plus élevées (par ex. 10-15 fps) pour des mouvements complexes

2. Paramètres de résolution
   - Ne définissez ForcedVideoWidth/Height que lorsque vous devez spécifiquement redimensionner
   - Maintenez le rapport hauteur/largeur en définissant la largeur et la hauteur proportionnellement
   - Tenez compte des limitations de la plateforme cible lors du choix des dimensions

3. Optimisation des performances
   - Des fréquences d'images plus faibles aboutissent à des fichiers plus petits
   - Tenez compte de l'équilibre entre qualité et taille de fichier en fonction de votre cas d'usage
   - Testez différentes configurations pour trouver les paramètres optimaux pour vos besoins

### Exemple d'utilisation

Voici un exemple complet de configuration et d'utilisation de la classe AnimatedGIFOutput :

```csharp
// Créer une nouvelle instance avec les paramètres par défaut
var gifOutput = new AnimatedGIFOutput();

// Configurer les paramètres de sortie
gifOutput.ForcedVideoWidth = 800;
gifOutput.ForcedVideoHeight = 600;
gifOutput.FrameRate = new VideoFrameRate(5); // 5 fps

// Appliquer les paramètres au core
core.Output_Format = gifOutput; // core est une instance de VideoCaptureCore ou VideoEditCore
core.Output_Filename = "output.gif";
```

### Scénarios courants et solutions

#### Création de GIF optimisés pour le web

```csharp
var webGifOutput = new AnimatedGIFOutput
{
    ForcedVideoWidth = 480,
    ForcedVideoHeight = 270,
    FrameRate = new VideoFrameRate(5)
};
```

#### Paramètres d'animation haute qualité
  
```csharp
var highQualityGif = new AnimatedGIFOutput
{
    FrameRate = new VideoFrameRate(15)
};
```
