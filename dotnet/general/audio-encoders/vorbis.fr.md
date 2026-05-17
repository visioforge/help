---
title: Guide d'encodage audio Vorbis pour le SDK .NET VisioForge
description: Implémentez l'encodage audio Vorbis en .NET avec optimisation de la qualité, prise en charge multiplateforme et compression efficace pour le streaming.
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
  - WebM
  - OGG
  - Vorbis
  - C#
primary_api_classes:
  - OGGVorbisOutput
  - VorbisEncoderSettings
  - WebMOutput
  - VideoCaptureCore
  - VideoEditCore

---

# Encodage audio Vorbis pour les développeurs .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à Vorbis dans le SDK VisioForge

La suite SDK VisioForge offre de puissantes capacités d'encodage audio Vorbis qui permettent aux développeurs d'implémenter une compression audio de haute qualité dans leurs applications .NET. Vorbis, un codec audio open source, fournit une fidélité audio exceptionnelle avec des taux de compression efficaces, ce qui le rend idéal pour les applications de streaming, le contenu multimédia et l'audio web.

Ce guide vous aidera à naviguer parmi les différentes options d'implémentation Vorbis disponibles dans l'écosystème SDK VisioForge, en fournissant des exemples de code pratiques et des stratégies d'optimisation pour différents cas d'usage.

## Options de l'encodeur Vorbis

Le SDK expose trois API distinctes pour l'encodage Vorbis. Deux sont du domaine Windows classique (`VideoCaptureCore` / `VideoEditCore`) et une est multiplateforme (`VideoCaptureCoreX` / `VideoEditCoreX` / `MediaBlocksPipeline`).

### Options d'implémentation

#### 1. Conteneur WebM avec audio Vorbis (Windows classique)

La classe [`WebMOutput`](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WebMOutput.html) de l'espace de noms `VisioForge.Core.Types.X.Output` encapsule l'audio Vorbis dans le format de conteneur WebM. S'exécute uniquement sur Windows (DirectShow).

#### 2. Sortie OGG Vorbis dédiée (Windows classique)

La classe [`OGGVorbisOutput`](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.OGGVorbisOutput.html) de l'espace de noms `VisioForge.Core.Types.X.Output` fournit un contrôle détaillé sur les modes VBR/débit pour Vorbis en conteneur OGG. Windows uniquement.

#### 3. VorbisEncoderSettings flexible (multiplateforme)

La classe [`VorbisEncoderSettings`](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.VorbisEncoderSettings.html) de l'espace de noms `VisioForge.Core.Types.X.AudioEncoders` pilote l'encodage Vorbis sur les moteurs X (`VideoCaptureCoreX`, `VideoEditCoreX`) et Media Blocks (`VorbisEncoderBlock`). C'est la voie recommandée pour les projets multiplateformes.

### Stratégies de contrôle de débit

Le choix du mode de contrôle de débit approprié est crucial pour équilibrer la qualité audio face aux exigences de taille de fichier. L'encodage Vorbis dans VisioForge prend en charge deux approches principales :

#### Débit variable basé sur la qualité (VBR)

Le VBR basé sur la qualité est l'approche recommandée pour la plupart des applications, car il ajuste dynamiquement le débit pour maintenir une qualité perceptive cohérente tout au long du flux audio.

=== "WebMOutput"

    WebMOutput implémente une approche simplifiée basée sur la qualité avec une échelle facile à comprendre :
    
    ```cs
    // Créer et configurer la sortie WebM avec audio Vorbis de haute qualité
    var webmOutput = new WebMOutput();
    
    // Plage de qualité : 20 (la plus basse) à 100 (la plus haute)
    // Les valeurs 70-80 offrent une excellente qualité pour la plupart des contenus
    webmOutput.Audio_Quality = 80;
    
    // Des valeurs plus élevées produisent une meilleure qualité audio avec des fichiers plus volumineux
    // Des valeurs plus faibles privilégient la taille du fichier sur la fidélité audio
    ```
    
    Considérations clés :
    
    - Le réglage de qualité impacte directement la qualité audio perçue et la taille du fichier
    - Les valeurs autour de 70-80 fonctionnent bien pour la plupart des contenus professionnels
    - Des réglages plus bas (40-60) peuvent convenir aux enregistrements vocaux uniquement

=== "OGGVorbisOutput"

    OGGVorbisOutput offre une sélection plus explicite du mode de qualité :
    
    ```cs
    // Initialiser la sortie OGG Vorbis pour un encodage axé sur la qualité
    var oggOutput = new OGGVorbisOutput();
    
    // Définir le mode d'encodage en VBR basé sur la qualité
    oggOutput.Mode = VorbisMode.Quality;
    
    // Configurer le niveau de qualité (plage : 20-100)
    // 80 : Haute qualité pour la musique et l'audio complexe
    // 60 : Bonne qualité pour un usage général
    // 40 : Qualité acceptable pour les enregistrements vocaux
    oggOutput.Quality = 80;
    ```
    
    Cette implémentation vous donne un contrôle direct sur le compromis qualité/taille, ce qui la rend idéale pour les applications avec des types de contenu variés.

=== "VorbisEncoderSettings"

    VorbisEncoderSettings utilise l'échelle native de qualité Vorbis :
    
    ```cs
    // Créer l'encodeur Vorbis avec contrôle de débit basé sur la qualité
    var vorbisEncoder = new VorbisEncoderSettings();
    
    // Définir le mode de contrôle de débit en VBR basé sur la qualité
    vorbisEncoder.RateControl = VorbisEncoderRateControl.Quality;
    
    // Configurer le niveau de qualité en utilisant l'échelle Vorbis (-1 à 10)
    // -1 : Très faible qualité (~45 kbps)
    // 3 : Bonne qualité (~112 kbps)
    // 5 : Très bonne qualité (~160 kbps) 
    // 8 : Excellente qualité (~224 kbps)
    // 10 : Qualité la plus élevée (~320 kbps)
    vorbisEncoder.Quality = 5;
    ```
    
    L'implémentation VorbisEncoderSettings offre le contrôle de qualité le plus précis, utilisant l'échelle de qualité Vorbis établie que les ingénieurs audio connaissent.


#### Encodage contraint par le débit

Pour les scénarios avec des limitations spécifiques de bande passante ou des tailles de fichier cibles, l'encodage contraint par le débit offre des tailles de sortie plus prévisibles.

=== "WebMOutput"

    WebMOutput ne prend pas en charge le contrôle explicite du débit pour l'audio Vorbis. Les développeurs doivent utiliser le paramètre de qualité à la place et tester pour déterminer les débits résultants.

=== "OGGVorbisOutput"

    OGGVorbisOutput fournit des outils complets de gestion du débit :
    
    ```cs
    // Configurer la sortie OGG avec des contraintes de débit spécifiques
    var oggOutput = new OGGVorbisOutput();
    
    // Activer le mode d'encodage contrôlé par le débit
    oggOutput.Mode = VorbisMode.Bitrate;
    
    // Configurer les paramètres de débit (toutes les valeurs en Kbps)
    oggOutput.MinBitRate = 96;     // Plancher minimum de débit
    oggOutput.AvgBitRate = 160;    // Débit moyen cible
    oggOutput.MaxBitRate = 240;    // Plafond maximum de débit
    
    // Ces paramètres créent un encodage VBR contrôlé qui
    // est en moyenne de 160 Kbps mais peut fluctuer entre les limites
    ```
    
    Cette approche est idéale pour les applications de streaming où la prévision de bande passante est importante.

=== "VorbisEncoderSettings"

    VorbisEncoderSettings offre les options de contrôle du débit les plus détaillées :
    
    ```cs
    // Initialiser l'encodeur Vorbis avec des contraintes de débit
    var vorbisEncoder = new VorbisEncoderSettings();
    
    // Définir le mode de contrôle de débit basé sur le débit
    vorbisEncoder.RateControl = VorbisEncoderRateControl.Bitrate;
    
    // Configurer les paramètres de débit (toutes les valeurs en Kbps ; plage valide 16-240)
    vorbisEncoder.Bitrate = 192;      // Débit moyen cible
    vorbisEncoder.MinBitrate = 128;   // Débit minimum autorisé
    vorbisEncoder.MaxBitrate = 240;   // Débit maximum autorisé
    
    // Ces paramètres sont idéaux pour les applications nécessitant
    // des tailles de fichier prévisibles ou une bande passante de streaming
    ```
    
    Les contrôles flexibles de débit permettent un encodage audio précis adapté aux exigences spécifiques de livraison.


Consultez le [VorbisEncoderBlock](../../mediablocks/AudioEncoders/index.md) et le [OGGSinkBlock](../../mediablocks/Sinks/index.md) pour plus d'informations.

### Bonnes pratiques pour les développeurs

Pour obtenir des résultats optimaux avec l'encodage Vorbis dans vos applications .NET, considérez ces recommandations centrées sur les développeurs :

#### Choisir le bon mode d'encodage

1. **Choix par défaut : VBR basé sur la qualité**
   - Produit une qualité perçue cohérente sur des contenus variés
   - Optimise automatiquement le débit en fonction de la complexité audio
   - Simplifie la configuration avec un seul paramètre de qualité

2. **Quand utiliser le mode contraint par le débit :**
   - Applications de streaming avec limitations de bande passante
   - Environnements à stockage limité avec allocations de taille fixe
   - Réseaux de diffusion de contenu avec exigences de bande passante prévisibles

#### Paramètres recommandés pour les cas d'usage courants

| Type de contenu | Paramètres recommandés |
|-------------|----------------------|
| Musique (haute qualité) | WebM : Audio_Quality = 80<br>OGG : Quality = 80<br>VorbisEncoder : Quality = 6 |
| Enregistrements vocaux | WebM : Audio_Quality = 60<br>OGG : Quality = 60<br>VorbisEncoder : Quality = 3 |
| Contenu mixte | WebM : Audio_Quality = 70<br>OGG : Quality = 70<br>VorbisEncoder : Quality = 4 |
| Audio en streaming | OGG : Mode = Bitrate, AvgBitRate = 128<br>VorbisEncoder : RateControl = Bitrate, Bitrate = 128 |

## Sortie Windows uniquement

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La classe `OGGVorbisOutput` fournit la configuration et la fonctionnalité pour encoder l'audio à l'aide du codec Vorbis.

### Détails de la classe

```csharp
public sealed class OGGVorbisOutput : IVideoEditBaseOutput, IVideoCaptureBaseOutput
```

La classe implémente deux interfaces :

- `IVideoEditBaseOutput` : Permet l'utilisation dans des scénarios d'édition vidéo
- `IVideoCaptureBaseOutput` : Permet l'utilisation dans des scénarios de capture vidéo

### Contrôles de débit

Lorsqu'elle fonctionne en mode Bitrate, ces propriétés contrôlent les contraintes de débit de sortie :

#### AvgBitRate

- Type : `int`
- Valeur par défaut : 128 (Kbps)
- Description : Spécifie le débit moyen cible pour le flux audio encodé. Cette valeur représente le compromis général entre qualité et taille de fichier.

#### MaxBitRate

- Type : `int`
- Valeur par défaut : 192 (Kbps)
- Description : Définit le débit maximum autorisé pendant l'encodage. Utile pour s'assurer que l'audio encodé ne dépasse pas les contraintes de bande passante.

#### MinBitRate

- Type : `int`
- Valeur par défaut : 64 (Kbps)
- Description : Définit le débit minimum autorisé pendant l'encodage. Aide à maintenir un niveau de qualité de référence même pendant les passages audio simples.

### Contrôles de qualité

#### Quality

- Type : `int`
- Valeur par défaut : 80
- Plage valide : 10-100
- Description : Lorsqu'il fonctionne en mode Quality, cette valeur détermine la qualité d'encodage. Des valeurs plus élevées entraînent une meilleure qualité audio mais des tailles de fichier plus importantes.

#### Mode

- Type : `VorbisMode` (enum)
- Valeur par défaut : `VorbisMode.Bitrate`
- Options :
  - `VorbisMode.Quality` : L'encodage se concentre sur le maintien d'un niveau de qualité cohérent
  - `VorbisMode.Bitrate` : L'encodage se concentre sur le maintien des contraintes de débit spécifiées

### Constructeur

```csharp
public OGGVorbisOutput()
```

Initialise une nouvelle instance avec les valeurs par défaut :

- MinBitRate : 64 kbps
- AvgBitRate : 128 kbps
- MaxBitRate : 192 kbps
- Quality : 80
- Mode : VorbisMode.Bitrate

### Méthodes de sérialisation

#### Save()

```csharp
public string Save()
```

Sérialise la configuration actuelle en chaîne JSON, permettant aux paramètres d'être sauvegardés et restaurés ultérieurement.

#### Load(string json)

```csharp
public static OGGVorbisOutput Load(string json)
```

Crée une nouvelle instance avec les paramètres désérialisés à partir de la chaîne JSON fournie.

### Exemples d'utilisation

#### Utilisation basique avec paramètres par défaut

```csharp
var oggOutput = new OGGVorbisOutput();
// Prêt à l'emploi avec les paramètres par défaut (mode Bitrate, 128 kbps en moyenne)
```

#### Encodage basé sur la qualité

```csharp
var oggOutput = new OGGVorbisOutput
{
    Mode = VorbisMode.Quality,
    Quality = 90  // Réglage de haute qualité
};
```

#### Encodage avec débit contraint

```csharp
var oggOutput = new OGGVorbisOutput
{
    Mode = VorbisMode.Bitrate,
    MinBitRate = 96,    // Minimum 96 kbps
    AvgBitRate = 160,   // Cible 160 kbps
    MaxBitRate = 240    // Maximum 240 kbps
};
```

#### Sauvegarde et chargement de la configuration

```csharp
// Sauvegarder la configuration
var oggOutput = new OGGVorbisOutput();
string savedConfig = oggOutput.Save();
```

```csharp
// Charger la configuration
var loadedOutput = OGGVorbisOutput.Load(savedConfig);
```

#### Appliquer les paramètres aux instances du noyau

```csharp
var core = new VideoCaptureCore();
core.Output_Filename = "output.ogg";
core.Output_Format = oggOutput;
```

```csharp
var core = new VideoEditCore();
core.Output_Filename = "output.ogg";
core.Output_Format = oggOutput;
```

## Considérations de performance

Lors de l'implémentation de l'encodage Vorbis dans des environnements de production :

- La qualité d'encodage impacte directement l'utilisation du CPU ; des réglages de qualité plus élevés nécessitent plus de puissance de traitement
- L'implémentation VorbisEncoderSettings offre le meilleur équilibre entre flexibilité et performance
- Des profils préconfigurés peuvent aider à standardiser la qualité de sortie sur différents types de contenu
- Envisagez l'encodage multithread pour les applications de traitement par lots

## Conclusion

L'encodage Vorbis fournit une excellente solution open source pour la compression audio de haute qualité dans les applications .NET. En comprenant les différentes options d'implémentation et stratégies de configuration disponibles dans le SDK VisioForge, les développeurs peuvent équilibrer efficacement la qualité audio, la taille du fichier et les exigences de performance pour leurs cas d'usage spécifiques.

Que vous construisiez une application de streaming, un outil de traitement multimédia ou que vous intégriez des capacités audio dans un écosystème logiciel plus large, les encodeurs Vorbis dans les SDK .NET de VisioForge offrent la flexibilité et la performance nécessaires au traitement audio professionnel.
