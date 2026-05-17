---
title: Encodage audio FLAC sans perte et enregistrement en C# .NET
description: Enregistrement sans perte avec niveaux de qualité 0-9, fréquences jusqu'à 655 kHz, surround 8 canaux et optimisation LPC. Exemples C# avec VisioForge SDK.
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
  - Streaming
  - Encoding
  - Editing
  - FLAC
  - C#
primary_api_classes:
  - FLACOutput
  - FLACEncoderSettings
  - VideoCaptureCoreX
  - VideoEditCoreX
  - CustomAudioProcessor

---

# Encodeur et sortie FLAC

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

L'encodeur FLAC (Free Lossless Audio Codec) fournit une compression audio sans perte de haute qualité tout en préservant la qualité audio d'origine.

## Sortie FLAC multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Fonctionnalités

L'encodeur FLAC prend en charge une large gamme de configurations audio :

- Fréquences d'échantillonnage de 1 Hz à 655 350 Hz
- Jusqu'à 8 canaux audio (mono à surround 7.1)
- Compression sans perte avec paramètres de qualité ajustables
- Prise en charge de la sortie diffusable en continu
- Tailles de bloc et paramètres de compression configurables

### Paramètres de qualité

L'encodeur fournit un paramètre de qualité allant de 0 à 9 :

- 0 : Compression la plus rapide (utilisation CPU la plus faible)
- 1-7 : Paramètres de compression équilibrés
- 8 : Compression la plus élevée (utilisation CPU plus importante)
- 9 : Compression extrême (extrêmement gourmande en CPU)

Le paramètre de qualité par défaut est 5, ce qui offre un bon équilibre entre taux de compression et vitesse de traitement.

### Paramètres de base

La classe multiplateforme [FLACEncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.FLACEncoderSettings.html) offre des options de configuration avancées :

```csharp
// Créer les paramètres de l'encodeur FLAC avec la qualité par défaut
var flacSettings = new FLACEncoderSettings
{
    // Niveau de compression par défaut
    Quality = 5,        

    // Taille de bloc audio en échantillons
    BlockSize = 4608,              

    // Activer la prise en charge du streaming
    StreamableSubset = true,    

    // Activer le traitement stéréo
    MidSideStereo = true          
};
```

### Paramètres de compression avancés

```csharp
// Créer les paramètres de l'encodeur FLAC avec configuration avancée
var advancedSettings = new FLACEncoderSettings
{
    // Paramètres de prédiction linéaire
    // Ordre LPC maximal pour la prédiction
    MaxLPCOrder = 8,               
    // Précision automatique pour les coefficients
    QlpCoeffPrecision = 0,        
    
    // Paramètres de codage du résidu
    MinResidualPartitionOrder = 3,
    MaxResidualPartitionOrder = 3,
    
    // Paramètres d'optimisation de la recherche
    // Désactiver la recherche coûteuse de coefficients
    ExhaustiveModelSearch = false, 
    // Désactiver la recherche de précision
    QlpCoeffPrecSearch = false,    
    // Désactiver la recherche de codes d'échappement
    EscapeCoding = false          
};
```

### Exemple de code

Ajouter la sortie FLAC à l'instance du noyau Video Capture SDK :

```csharp
// Créer une instance du noyau Video Capture SDK
var core = new VideoCaptureCoreX();

// Créer une instance de sortie FLAC
var flacOutput = new FLACOutput("output.flac");

// Définir la qualité de l'encodeur FLAC
flacOutput.Audio.Quality = 5;

// Ajouter la sortie FLAC
core.Outputs_Add(flacOutput, true);
```

Définir le format de sortie pour l'instance du noyau Video Edit SDK :

```csharp
// Créer une instance du noyau Video Edit SDK
 var core = new VideoEditCoreX();

// Créer une instance de sortie FLAC
 var flacOutput = new FLACOutput("output.flac");

 // Définir la qualité 
 flacOutput.Audio.Quality = 5;

 // Définir le format de sortie
 core.Output_Format = flacOutput;
```

Créer une instance de sortie FLAC Media Blocks :

```csharp
// Créer une instance des paramètres de l'encodeur FLAC
var flacSettings = new FLACEncoderSettings();

// Créer une instance de sortie FLAC
var flacOutput = new FLACOutputBlock("output.flac", flacSettings);
```

### Classe FLACOutput

La classe `FLACOutput` fournit la fonctionnalité de configuration de la sortie FLAC (Free Lossless Audio Codec) dans les SDK VisioForge.

```csharp
// Créer une nouvelle instance de sortie FLAC
var flacOutput = new FLACOutput("output.flac");

// Configurer les paramètres de l'encodeur FLAC (Quality : 0 le plus rapide .. 8 le plus élevé, 9 extrême)
flacOutput.Audio.Quality = 5;
```

#### Nom de fichier

- Définissez le nom de fichier de sortie pendant l'initialisation ou via la propriété `Filename`
- Ou appelez les méthodes `GetFilename()` / `SetFilename(string)` (équivalentes — elles sous-tendent la propriété `Filename`)

```csharp
// Définir pendant l'initialisation
var flacOutput = new FLACOutput("audio_output.flac");

// Ou via la propriété
flacOutput.Filename = "new_output.flac";

// Ou via les accesseurs méthode
flacOutput.SetFilename("final.flac");
string currentPath = flacOutput.GetFilename();  // "final.flac"
```

#### Paramètres audio

La propriété `Audio` donne accès aux paramètres d'encodage spécifiques à FLAC via la classe `FLACEncoderSettings` :

```csharp
flacOutput.Audio = new FLACEncoderSettings();
// Configurer ici des paramètres d'encodage FLAC spécifiques
```

#### Traitement audio personnalisé

Vous pouvez définir un processeur audio personnalisé via la propriété `CustomAudioProcessor` :

```csharp
flacOutput.CustomAudioProcessor = new CustomMediaBlock();
```

#### Notes d'implémentation

- La classe implémente plusieurs interfaces :
  - `IVideoEditXBaseOutput`
  - `IVideoCaptureXBaseOutput`
  - `IOutputAudioProcessor`
  
- Seul l'encodage audio FLAC est pris en charge (pas de capacités d'encodage vidéo)
- Les paramètres par défaut de l'encodeur FLAC sont créés automatiquement à l'initialisation

Le Media Blocks SDK contient un [bloc d'encodeur FLAC](../../mediablocks/AudioEncoders/index.md) dédié.

### Considérations de performance

Lors de la configuration de l'encodeur FLAC, tenez compte de ces facteurs de performance :

1. Des réglages de qualité plus élevés (7-9) augmenteront significativement l'utilisation CPU
2. L'option `ExhaustiveModelSearch` peut considérablement impacter la vitesse d'encodage
3. Des tailles de bloc plus grandes peuvent améliorer la compression mais augmenter l'utilisation mémoire
4. `StreamableSubset` devrait rester activé sauf besoins spécifiques

### Compatibilité

L'encodeur prend en charge les configurations suivantes :

- Canaux audio : 1 à 8 canaux
- Fréquences d'échantillonnage : 1 Hz à 655 350 Hz
- Débit : Variable (compression sans perte)

### Gestion des erreurs

Vérifiez toujours la disponibilité de l'encodeur avant utilisation :

```csharp
if (!FLACEncoderSettings.IsAvailable())
{
    // Gérer le scénario d'encodeur indisponible
    Console.WriteLine("FLAC encoder is not available on this system");
    return;
}
```

### Bonnes pratiques

1. Commencez avec le réglage de qualité par défaut (5) et ajustez selon vos besoins
2. Activez `MidSideStereo` pour le contenu stéréo afin d'améliorer la compression
3. Utilisez `SeekPoints` pour les fichiers audio plus longs afin d'activer le positionnement rapide
4. Conservez `StreamableSubset` activé sauf besoins spécifiques
5. Évitez d'utiliser `ExhaustiveModelSearch` sauf si le taux de compression est critique

## Sortie FLAC Windows uniquement

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La classe [FLACOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.FLACOutput.html) fournit des paramètres spécifiques à Windows pour l'encodeur FLAC. Cette classe implémente à la fois les interfaces `IVideoEditBaseOutput` et `IVideoCaptureBaseOutput`, ce qui la rend adaptée aux scénarios d'édition et de capture vidéo.

### Propriétés

#### Niveau de compression

- **Propriété** : `Level`
- **Type** : `int`
- **Plage** : 0-8
- **Par défaut** : 5
- **Description** : Contrôle le niveau de compression, où 0 offre la compression la plus rapide et 8 la compression la plus élevée.

#### Taille de bloc

- **Propriété** : `BlockSize`
- **Type** : `int`
- **Par défaut** : 4608
- **Valeurs valides** : Pour les flux subset, doit être l'une des suivantes :
  - 192, 256, 512, 576, 1024, 1152, 2048, 2304, 4096, 4608
  - 8192, 16384 (uniquement si la fréquence d'échantillonnage > 48 kHz)
- **Description** : Spécifie la taille de bloc en échantillons. L'encodeur utilise la même taille de bloc pour l'ensemble du flux.

#### Ordre LPC

- **Propriété** : `LPCOrder`
- **Type** : `int`
- **Par défaut** : 8
- **Contraintes** :
  - Doit être ≤ 32
  - Pour les flux subset à ≤ 48 kHz, doit être ≤ 12
- **Description** : Spécifie l'ordre maximal de la prédiction linéaire (LPC). La définir à 0 désactive la prédiction linéaire générique et utilise uniquement des prédicteurs fixes, ce qui est plus rapide mais entraîne généralement des fichiers 5 à 10 % plus volumineux.

#### Options de codage mid-side

##### Codage mid-side

- **Propriété** : `MidSideCoding`
- **Type** : `bool`
- **Par défaut** : `false`
- **Description** : Active le codage mid-side pour les flux stéréo. Cela augmente généralement la compression de quelques pour cent en encodant à la fois les versions paire stéréo et mid-side de chaque bloc et en sélectionnant la trame résultante la plus petite.

##### Codage mid-side adaptatif

- **Propriété** : `AdaptiveMidSideCoding`
- **Type** : `bool`
- **Par défaut** : `false`
- **Description** : Active le codage mid-side adaptatif pour les flux stéréo. Cela fournit un encodage plus rapide que le codage mid-side complet mais avec une compression légèrement inférieure en basculant adaptativement entre le codage indépendant et mid-side.

#### Paramètres Rice

##### Rice minimum

- **Propriété** : `RiceMin`
- **Type** : `int`
- **Par défaut** : 3
- **Description** : Définit l'ordre minimum de partition du résidu. Fonctionne conjointement avec RiceMax pour contrôler comment le signal résiduel est partitionné.

##### Rice maximum

- **Propriété** : `RiceMax`
- **Type** : `int`
- **Par défaut** : 3
- **Description** : Définit l'ordre maximum de partition du résidu. Le résidu est partitionné en 2^min à 2^max morceaux, chacun avec son propre paramètre Rice. Les paramètres optimaux dépendent généralement de la taille du bloc, avec les meilleurs résultats lorsque blocksize/(2^n)=128.

#### Options avancées

##### Recherche exhaustive du modèle

- **Propriété** : `ExhaustiveModelSearch`
- **Type** : `bool`
- **Par défaut** : `false`
- **Description** : Active la recherche exhaustive du modèle pour un encodage optimal. Lorsque activé, l'encodeur génère des sous-trames pour chaque ordre et utilise la plus petite, améliorant potentiellement la compression d'environ 0,5 % au prix d'un temps d'encodage considérablement accru.

### Méthodes

#### Constructeur

```csharp
public FLACOutput()
```

Initialise une nouvelle instance avec les valeurs par défaut :

- Level = 5
- RiceMin = 3
- RiceMax = 3
- LPCOrder = 8
- BlockSize = 4608

### Sérialisation

#### Save()

```csharp
public string Save()
```

Sérialise les paramètres en chaîne JSON.

#### Load(string json)

```csharp
public static FLACOutput Load(string json)
```

Crée une nouvelle instance FLACOutput à partir d'une chaîne JSON.

### Exemple d'utilisation

```csharp
var flacSettings = new FLACOutput
{
    Level = 8,                   // Compression maximale
    BlockSize = 4608,            // Taille de bloc par défaut
    MidSideCoding = true,        // Activer le codage mid-side pour une meilleure compression
    ExhaustiveModelSearch = true // Activer la recherche exhaustive pour la meilleure compression
};

core.Output_Format = flacSettings; // Core est VideoCaptureCore ou VideoEditCore
```

### Bonnes pratiques

#### Sélection du niveau de compression

- Utilisez le niveau 0-3 pour un encodage plus rapide avec compression modérée
- Utilisez le niveau 4-6 pour un équilibre compression/vitesse
- Utilisez le niveau 7-8 pour une compression maximale indépendamment de la vitesse

#### Considérations sur la taille de bloc

- Des tailles de bloc plus grandes offrent généralement une meilleure compression
- Tenez-vous-en aux valeurs standard (4608, 4096, etc.) pour une compatibilité maximale
- Tenez compte des contraintes mémoire lors du choix de la taille de bloc

#### Codage mid-side

- Activez-le pour le contenu stéréo lorsque la compression est prioritaire
- Utilisez le mode adaptatif lorsque la vitesse d'encodage est importante
- Désactivez-le pour le contenu mono car il n'a aucun effet

#### Paramètres Rice

- Les valeurs par défaut (3,3) conviennent à la plupart des cas d'usage
- Augmentez-les pour une compression potentiellement meilleure au prix de la vitesse d'encodage
- Les valeurs au-delà de 6 apportent rarement des bénéfices significatifs
