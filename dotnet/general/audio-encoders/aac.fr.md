---
title: Encodage audio AAC avec conteneur M4A en C# .NET VisioForge
description: Utilisez les backends avenc_aac, voaacenc et Media Foundation avec détection à l'exécution. Débit 32-320 kbps, surround 5.1, conteneurs M4A/MP4.
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
  - Recording
  - Encoding
  - Editing
  - MP4
  - TS
  - MPEG-2
  - AAC
  - C#
primary_api_classes:
  - M4AOutput
  - AACObject
  - AVENCAACEncoderSettings
  - VOAACEncoderSettings
  - AACOutput

---

# Encodeur AAC et sortie M4A

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le SDK VisioForge fournit plusieurs implémentations d'encodeurs AAC, chacune avec des caractéristiques et des cas d'usage spécifiques.

## Qu'est-ce que la sortie M4A ?

M4A est un format de fichier utilisé pour stocker des données audio encodées avec le codec Advanced Audio Coding (AAC). Les SDK VisioForge .Net offrent une prise en charge robuste pour la création de fichiers audio M4A de haute qualité via leur classe dédiée M4AOutput. Ce format est largement utilisé pour la distribution audio numérique grâce à son excellente efficacité de compression et à sa qualité sonore.

## Sortie M4A (AAC) multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Les SDK avec capacités multiplateformes (VideoCaptureCoreX, VideoEditCoreX, MediaBlocksPipeline) permettent d'utiliser plusieurs implémentations d'encodeurs AAC via `M4AOutput`. Ce guide se concentre sur trois approches principales utilisant des objets de paramètres dédiés :

1. [Encodeur AVENC AAC](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.AVENCAACEncoderSettings.html) - Un encodeur multiplateforme riche en fonctionnalités.
2. [Encodeur VO-AAC](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.VOAACEncoderSettings.html) - Un encodeur multiplateforme simplifié.
3. Encodeur AAC Media Foundation - Un encodeur système spécifique à Windows, accessible sur les plateformes Windows via `MFAACEncoderSettings`.

### Encodeur AVENC AAC

L'encodeur AVENC AAC offre les options de configuration les plus complètes pour l'encodage audio. Il fournit des paramètres avancés pour le codage stéréo, la prédiction et le modelage du bruit.

#### Caractéristiques principales

- Stratégies de codeur multiples
- Codage stéréo configurable
- Techniques avancées de bruit et de prédiction

#### Stratégies de codeur

L'encodeur AVENC AAC prend en charge trois stratégies de codeur :

- `ANMR` : Méthode avancée de modélisation et de réduction du bruit
- `TwoLoop` : Méthode de recherche à deux boucles pour l'optimisation
- `Fast` : Algorithme de recherche rapide par défaut (recommandé pour la plupart des cas d'usage)

#### Exemple de configuration

```csharp
var aacSettings = new AVENCAACEncoderSettings
{
    Coder = AVENCAACEncoderCoder.Fast,
    Bitrate = 192,
    IntensityStereo = true,
    ForceMS = AVENCAACTrilian.Auto,
    TNS = true
};
```

#### Paramètres pris en charge

- **Débits** : 0, 32, 64, 96, 128, 160, 192, 224, 256, 320 kbps
- **Fréquences d'échantillonnage** : 7350 à 96000 Hz
- **Canaux** : 1 à 6 canaux

### Encodeur VO-AAC

!!! warning "Obsolète"
    `VOAACEncoderSettings` est marqué `[Obsolete]` dans les versions récentes du SDK — l'élément `voaacenc` sous-jacent échoue dans la négociation des caps avec `mpegtsmux` sur iOS et n'est plus recommandé. Préférez `AVENCAACEncoderSettings` pour le nouveau code ; `MFAACEncoderSettings` reste un bon choix sur Windows.

L'encodeur VO-AAC est un encodeur plus simplifié avec des options de configuration plus simples.

#### Caractéristiques principales

- Configuration simplifiée
- Contrôles directs du débit et de la fréquence d'échantillonnage
- Limité à l'audio stéréo

#### Exemple de configuration

```csharp
var aacSettings = new VOAACEncoderSettings
{
    Bitrate = 128
};
```

#### Paramètres pris en charge

- **Débits** : 32, 64, 96, 128, 160, 192, 224, 256, 320 kbps
- **Fréquences d'échantillonnage** : 8000 à 96000 Hz
- **Canaux** : 1-2 canaux

### Encodeur AAC Media Foundation (Windows uniquement)

Cet encodeur est spécifique aux plateformes Windows et offre une solution d'encodage limitée mais optimisée pour les performances.

#### Caractéristiques principales

- Implémentation spécifique à Windows
- Options de débit prédéfinies
- Prise en charge limitée des fréquences d'échantillonnage

#### Paramètres pris en charge

- **Débits** : 0 (Auto), 96, 128, 160, 192, 576, 768, 960, 1152 kbps
- **Fréquences d'échantillonnage** : 44100, 48000 Hz
- **Canaux** : 1, 2, 6 canaux

### Disponibilité et sélection de l'encodeur

Chaque encodeur fournit une méthode statique `IsAvailable()` pour vérifier s'il peut être utilisé dans l'environnement courant. C'est utile pour les vérifications de compatibilité à l'exécution.

```csharp
if (AVENCAACEncoderSettings.IsAvailable())
{
    // Utiliser l'encodeur AVENC AAC
}
else if (VOAACEncoderSettings.IsAvailable())
{
    // Solution de repli vers l'encodeur VO-AAC
}
```

### Prise en main de M4AOutput

L'implémentation multiplateforme utilise la classe [M4AOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.M4AOutput.html) comme base pour la création de fichiers M4A. Pour commencer à utiliser cette fonctionnalité, initialisez la classe avec le nom de fichier de sortie souhaité :

```csharp
var output = new M4AOutput("output.m4a");
```

### Basculement entre encodeurs

La sélection par défaut de l'encodeur dépend de la plateforme :

- Environnements Windows : MF AAC
- Autres plateformes : VO-AAC

Vous pouvez remplacer cette sélection par défaut en définissant explicitement la propriété `Audio` :

```csharp
// Pour l'encodeur VO-AAC
output.Audio = new VOAACEncoderSettings();

// Pour l'encodeur AVENC AAC
output.Audio = new AVENCAACEncoderSettings();

// Pour l'encodeur MF AAC (Windows uniquement)
#if NET_WINDOWS
output.Audio = new MFAACEncoderSettings();
#endif
```

### Configuration des paramètres du puits MP4

Comme les fichiers M4A reposent sur le format de conteneur MP4, vous pouvez ajuster divers paramètres de sortie via la propriété `Sink` :

```csharp
// Modifier le nom de fichier de sortie
output.Sink.Filename = "new_output.m4a";
```

### Traitement audio avancé

Pour les flux de travail nécessitant un traitement audio spécialisé, la classe M4AOutput prend en charge des processeurs audio personnalisés :

```csharp
// Implémentez votre logique de traitement audio personnalisée
output.CustomAudioProcessor = new MyCustomAudioProcessor(); 
```

### Méthodes clés pour la gestion des fichiers

La classe M4AOutput fournit plusieurs méthodes pour gérer les fichiers et récupérer des informations sur l'encodeur :

```csharp
// Obtenir le nom de fichier de sortie actuel
string currentFile = output.GetFilename();

// Mettre à jour le nom de fichier de sortie
output.SetFilename("updated_file.m4a");

// Récupérer les encodeurs audio disponibles
var audioEncoders = output.GetAudioEncoders();
```

### Utilisation de la sortie M4A dans différents SDK

Chaque SDK VisioForge a une approche légèrement différente pour implémenter la sortie M4A :

#### Avec Video Capture SDK

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(output, true);
```

#### Avec Video Edit SDK

```csharp
var core = new VideoEditCoreX();
core.Output_Format = output;
```

#### Avec Media Blocks SDK

```csharp
var aac = new VOAACEncoderSettings();
var sinkSettings = new MP4SinkSettings("output.m4a");
var m4aOutput = new M4AOutputBlock(sinkSettings, aac);
```

### Considérations sur le contrôle de débit

1. **Encodeur AVENC AAC** :
   - Contrôle de débit le plus flexible
   - Prend en charge le débit constant (CBR)
   - Plusieurs stratégies d'encodage affectent la qualité et les performances

2. **Encodeur VO-AAC** :
   - Contrôle simple de débit constant
   - Recommandé pour les besoins d'encodage simples
   - Configuration avancée limitée

3. **Encodeur Media Foundation** :
   - Limité à des débits prédéfinis
   - Bon pour un encodage rapide basé sur Windows
   - Option de débit automatique disponible

### Recommandations

- Pour l'encodage audio avancé avec un contrôle maximal, utilisez l'encodeur AVENC AAC
- Pour un encodage simple et multiplateforme, utilisez l'encodeur VO-AAC
- Pour un encodage optimisé spécifique à Windows, utilisez l'encodeur Media Foundation

### Considérations de performance et de qualité

- **Débit vs qualité vs taille de fichier** : Des débits plus élevés produisent généralement une meilleure qualité audio mais aboutissent également à des fichiers plus volumineux. Expérimentez avec différents débits pour trouver l'équilibre optimal pour votre contenu spécifique et vos besoins de distribution.
- **Correspondance de la fréquence d'échantillonnage** : Essayez toujours de choisir des fréquences d'échantillonnage qui correspondent à votre audio source. Cela évite un rééchantillonnage inutile, qui peut potentiellement dégrader la qualité audio.
- **Caractéristiques des encodeurs** :
  - `Encodeur AVENC AAC` : Offre les options de configuration les plus étendues, permettant un contrôle fin de la qualité et des performances. Idéal pour les cas d'usage avancés.
  - `Encodeur VO-AAC` : Offre un bon équilibre entre simplicité, compatibilité multiplateforme et qualité. Un choix solide pour de nombreux scénarios courants.
  - `Encodeur AAC Media Foundation` : Tire parti des capacités de traitement audio intégrées à Windows. Il peut être efficace sur Windows mais offre moins de flexibilité de configuration qu'AVENC.
- **Configuration des canaux (mono vs stéréo)** :
  - Pour le contenu vocal uniquement, l'utilisation de l'encodage mono (1 canal) peut réduire significativement la taille du fichier sans perte notable de qualité pour la parole. Vérifiez si les paramètres de l'encodeur choisi (par ex. `AVENCAACEncoderSettings.Channels`) autorisent une configuration explicite des canaux.
  - Pour la musique et les environnements audio riches, la stéréo (2 canaux) est généralement préférée.
- **Plages de débit selon le contenu** : Si plus élevé est souvent meilleur, le « meilleur » débit dépend du contenu audio :
  - *Parole/voix :* 64-96 kbps peut suffire.
  - *Musique générale :* 128-192 kbps est une cible courante pour une bonne qualité.
  - *Audio haute fidélité :* 256-320 kbps ou plus peut être utilisé lorsque la qualité immaculée est critique.
    Ces valeurs sont indicatives ; testez toujours avec votre audio spécifique.
- **Audience cible et plateforme** : Tenez compte de qui écoutera et sur quels appareils. Par exemple, si l'audio est principalement destiné au streaming web vers des appareils mobiles, des débits extrêmement élevés pourraient entraîner des problèmes de mise en mémoire tampon ou une consommation de données inutile. Adaptez votre choix d'encodeur et vos paramètres en conséquence.

### Exemple de code

- Consultez le guide [sortie MP4](../output-formats/mp4.md) pour des exemples de code.
- Consultez le [bloc d'encodeur AAC](../../mediablocks/AudioEncoders/index.md) pour des exemples de code.

## Sortie AAC Windows uniquement

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

[M4AOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.M4AOutput.html) est la classe principale pour configurer les paramètres de sortie M4A (AAC). Elle implémente à la fois les interfaces `IVideoEditBaseOutput` et `IVideoCaptureBaseOutput`.

### Propriétés

| Propriété | Type | Description | Valeur par défaut |
|----------|------|-------------|---------------|
| Version | AACVersion | Spécifie la version AAC (MPEG-2 ou MPEG-4) | MPEG4 |
| Object | AACObject | Définit le type d'objet AAC | Low |
| Output | AACOutput | Définit le mode de sortie AAC | RAW |
| Bitrate | int | Spécifie le débit AAC en kbps | 128 |

### Méthodes

#### `GetInternalTypeVC()`

- Retour : `VideoCaptureOutputFormat.M4A`
- Objectif : Obtient le format de sortie interne pour la capture vidéo

#### `GetInternalTypeVE()`

- Retour : `VideoEditOutputFormat.M4A`
- Objectif : Obtient le format de sortie interne pour l'édition vidéo

#### `Save()`

- Retour : Représentation JSON de l'objet M4AOutput sous forme de chaîne
- Objectif : Sérialise la configuration actuelle en JSON

#### `Load(string json)`

- Paramètres : Chaîne JSON contenant la configuration M4AOutput
- Retour : Nouvelle instance M4AOutput
- Objectif : Crée une nouvelle instance M4AOutput à partir d'une configuration JSON

### Énumérations associées

#### AACVersion

Définit la version AAC à utiliser :

| Valeur | Description |
|-------|-------------|
| MPEG4 | AAC MPEG-4 (par défaut) |
| MPEG2 | AAC MPEG-2 |

#### AACObject

Spécifie le type d'objet de flux de l'encodeur AAC :

| Valeur | Description |
|-------|-------------|
| Undefined | Ne pas utiliser |
| Main | Profil principal |
| Low | Profil Low Complexity (par défaut) |
| SSR | Profil Scalable Sample Rate |
| LTP | Profil Long Term Prediction |

#### AACOutput

Détermine le type de sortie du flux de l'encodeur AAC :

| Valeur | Description |
|-------|-------------|
| RAW | Flux AAC brut (par défaut) |
| ADTS | Format Audio Data Transport Stream |

### Exemple d'utilisation

```csharp
// Créer une nouvelle configuration de sortie M4A
var core = new VideoCaptureCore();
core.Mode = VideoCaptureMode.VideoCapture;
core.Output_Filename = "output.m4a";

var output = new M4AOutput
{
    Bitrate = 192,
    Version = AACVersion.MPEG4,
    Object = AACObject.Low,
    Output = AACOutput.ADTS
};

core.Output_Format = output; // core est une instance de VideoCaptureCore ou VideoEditCore
```

### Choisir le bon débit

Le débit optimal dépend du type de contenu et des exigences de qualité :

- **64-96 kbps** : Convient aux enregistrements vocaux et au contenu parlé
- **128-192 kbps** : Recommandé pour la musique générale et le contenu audio
- **256-320 kbps** : Idéal pour la musique haute fidélité où la qualité est primordiale

### Choix du profil approprié

- Utilisez `AACObject.Low` pour la plupart des applications car il offre un excellent équilibre entre qualité et efficacité d'encodage
- Réservez `AACObject.Main` aux cas d'usage spécialisés nécessitant une qualité maximale
- Évitez `AACObject.Undefined` car ce n'est pas une option d'encodage valide

### Choix du format de conteneur

- `AACOutput.ADTS` offre une meilleure compatibilité avec divers lecteurs et appareils
- `AACOutput.RAW` est préférable lorsque le flux AAC sera intégré dans un autre format de conteneur
