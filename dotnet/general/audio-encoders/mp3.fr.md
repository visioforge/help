---
title: Encodeur audio MP3 en C# .NET — Paramètres de débit LAME
description: Encodeur LAME avec modes CBR, ABR et VBR par qualité. Joint stereo, optimisation voix, débit 8-320 kbps. Exemples C# avec VisioForge SDK.
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
  - MP3
  - C#
primary_api_classes:
  - MP3Output
  - MP3EncoderSettings
  - MP3EncoderRateControl
  - MP3ChannelsMode
  - MP3EncodingQuality

---

# Maîtriser l'audio MP3 : enregistrer, capturer et éditer en C# et .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le SDK VisioForge permet aux développeurs d'enregistrer, capturer et éditer de l'audio MP3 de manière fluide dans les applications C#. Ce guide explore comment tirer parti de notre SDK .NET robuste pour le traitement audio MP3 de haute qualité. Que vous ayez besoin de capturer des flux multimédias, d'enregistrer des fichiers MP3 ou d'éditer des formes d'onde audio, notre boîte à outils multimédia C# fournit des outils complets utilisant la bibliothèque LAME. Le MP3, un format de compression audio avec perte largement adopté, est idéal pour le streaming audio et le stockage efficace.

Vous pouvez utiliser l'encodeur MP3 pour intégrer des fonctionnalités de capture et d'enregistrement audio dans divers formats de conteneur tels que MP4, AVI et MKV, améliorant ainsi vos projets de capture audio. Notre SDK fonctionne parfaitement avec Visual Studio pour une expérience de développement fluide.

Le SDK contient un encodeur audio MP3 qui peut être utilisé pour encoder des flux audio au format MP3 grâce à la bibliothèque LAME. Le MP3 est un format de compression audio avec perte largement utilisé pour le streaming et le stockage audio.

Vous pouvez utiliser l'encodage MP3 pour encoder l'audio dans des conteneurs MP4, AVI, MKV et d'autres.

## Capture et enregistrement audio MP3 multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La classe [MP3EncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.MP3EncoderSettings.html) offre aux développeurs une approche simplifiée pour configurer l'encodage MP3 dans les projets de capture audio en C#. Cette solution multiplateforme prend en charge divers contrôles de débit et paramètres de qualité, ce qui la rend idéale pour les applications d'enregistrement MP3 en .NET sur différents systèmes d'exploitation.

### Formats et spécifications pris en charge pour l'enregistrement MP3 en C#

- Format d'entrée : S16LE (Signed 16-bit Little Endian)
- Fréquences d'échantillonnage : 8000, 11025, 12000, 16000, 22050, 24000, 32000, 44100, 48000 Hz
- Canaux : Mono (1) ou Stéréo (2)

### Modes de contrôle de débit

L'encodeur prend en charge trois modes de contrôle de débit :

1. **CBR (débit constant)**
   - Débit fixe pendant tout le processus d'encodage
   - Débits pris en charge : 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 Kbit/s
   - Meilleur pour le streaming MP3 et lorsque la cohérence de la taille de fichier est importante

2. **ABR (débit moyen)**
   - Maintient un débit moyen tout en autorisant une certaine variation
   - Plus efficace que CBR tout en maintenant des tailles de fichier prévisibles
   - Utile pour les services de streaming qui ont besoin d'estimations approximatives de la taille du fichier

3. **VBR basé sur la qualité**
   - Débit variable basé sur la complexité du son
   - Paramètre de qualité de 0 (meilleur) à 10
   - Plus efficace pour le stockage et meilleur rapport qualité/taille

### Exemples d'encodage MP3 en C#

Créer des paramètres de base pour l'encodeur MP3 avec CBR.

```csharp
// Créer des paramètres de base de l'encodeur MP3 en utilisant le mode débit constant
var mp3Settings = new MP3EncoderSettings
{
    // Définir sur débit constant - fournit une taille de fichier cohérente et une fiabilité de streaming
    RateControl = MP3EncoderRateControl.CBR,
    // 192 kbps offre une bonne qualité pour la plupart des contenus musicaux tout en maintenant une taille de fichier raisonnable
    Bitrate = 192,
    // La qualité standard offre un bon équilibre entre vitesse d'encodage et qualité de sortie
    EncodingEngineQuality = MP3EncodingQuality.Standard,
    // Conserver les canaux stéréo (false) - mettre à true si vous souhaitez convertir en mono
    ForceMono = false
};
```

Configuration VBR basée sur la qualité pour l'édition MP3 en .NET de haute qualité.

```csharp
// Configurer l'encodeur MP3 avec débit variable pour un rapport qualité/taille optimal
var vbrSettings = new MP3EncoderSettings
{
    // Le VBR basé sur la qualité ajuste dynamiquement le débit en fonction de la complexité audio
    RateControl = MP3EncoderRateControl.Quality,
    // Échelle de qualité : 0 (meilleur) à 10 (pire) - 2.0 offre une excellente qualité avec une taille de fichier raisonnable
    Quality = 2.0f,
    // L'encodage de haute qualité utilise plus de CPU mais produit de meilleurs résultats
    EncodingEngineQuality = MP3EncodingQuality.High
};
```

Ajoutez la sortie MP3 pour capturer de l'audio MP3 en C# avec le Video Capture SDK :

La classe [MP3Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP3Output.html) implémente plusieurs interfaces :

- IVideoEditXBaseOutput
- IVideoCaptureXBaseOutput
- IOutputAudioProcessor

```csharp
// Créer une instance du noyau Video Capture SDK pour l'enregistrement
var core = new VideoCaptureCoreX();

// Initialiser la sortie MP3 avec le nom de fichier cible
var mp3Output = new MP3Output("output.mp3");

// Configurer les paramètres d'encodage audio
mp3Output.Audio.RateControl = MP3EncoderRateControl.CBR;  // Utiliser un débit constant pour un streaming fiable
mp3Output.Audio.Bitrate = 128;  // 128 kbps convient à l'enregistrement audio général

// Ajouter la sortie MP3 au pipeline de capture
core.Outputs_Add(mp3Output, true);
```

Définir le format de sortie pour l'instance du noyau Video Edit SDK :

```csharp
// Initialiser le Video Edit SDK pour traiter les médias existants
var core = new VideoEditCoreX();

// Créer la sortie MP3 avec le nom de fichier cible
var mp3Output = new MP3Output("output.mp3");

// Configurer l'encodage à débit variable pour un meilleur rapport qualité/taille
mp3Output.Audio.RateControl = MP3EncoderRateControl.Quality;
mp3Output.Audio.Quality = 5.0f;  // Réglage de qualité intermédiaire (échelle 0-10) - bon équilibre entre qualité et taille

// Définir comme format de sortie principal pour l'éditeur
core.Output_Format = mp3Output;
```

### Initialisation

Pour créer une nouvelle instance MP3Output, vous devez fournir le nom de fichier de sortie :

```csharp
// Initialiser la sortie MP3 avec le nom de fichier de destination
var mp3Output = new MP3Output("output.mp3");
```

### Paramètres audio

La propriété `Audio` donne accès aux paramètres de l'encodeur MP3 :

```csharp
// Créer un objet de paramètres par défaut de l'encodeur MP3
mp3Output.Audio = new MP3EncoderSettings();
// Une configuration supplémentaire peut être appliquée aux propriétés de mp3Output.Audio
```

### Traitement audio personnalisé

Vous pouvez définir un processeur audio personnalisé via la propriété `CustomAudioProcessor` pour gérer les manipulations de forme d'onde :

```csharp
// Attacher un processeur audio personnalisé pour une manipulation audio avancée
mp3Output.CustomAudioProcessor = new MediaBlock();
// Le MediaBlock peut être configuré pour des effets, du filtrage ou un autre traitement audio
```

### Opérations sur le nom de fichier

Il existe plusieurs façons de travailler avec le nom de fichier de sortie :

```csharp
// Récupérer le nom de fichier de sortie actuel
string currentFile = mp3Output.GetFilename();

// Modifier la destination de sortie
mp3Output.SetFilename("newoutput.mp3");

// Autre façon de définir le nom de fichier via la propriété
mp3Output.Filename = "another.mp3";
```

### Encodeurs audio

La classe MP3Output prend en charge l'encodage MP3 exclusivement. Vous pouvez vérifier les encodeurs disponibles :

```csharp
// Obtenir des informations sur les encodeurs audio disponibles
var audioEncoders = mp3Output.GetAudioEncoders();
// Retourne une liste de tuples contenant les noms des encodeurs et leurs types de paramètres
// Pour MP3Output, cela contiendra une seule entrée pour MP3
```

### Classe MP3OutputBlock

La classe [MP3OutputBlock](../../mediablocks/AudioEncoders/index.md) offre une manière plus flexible de configurer l'encodage MP3.

Créer une instance de sortie MP3 Media Blocks :

```csharp
// Créer des paramètres d'encodeur MP3 avec la configuration souhaitée
var mp3Settings = new MP3EncoderSettings();

// Initialiser le bloc de sortie MP3 avec le fichier de destination et les paramètres
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
```

Vérifier si l'encodage MP3 est disponible.

```cs
// Vérifier si l'encodage MP3 est disponible sur le système courant
if (!MP3EncoderSettings.IsAvailable())
{
   // Gérer le cas où l'encodage MP3 n'est pas disponible
   // Cela peut se produire si LAME ou d'autres bibliothèques requises sont manquantes
}
```

### Niveaux de qualité d'encodage

L'encodeur prend en charge trois préréglages de qualité qui affectent la vitesse d'encodage et l'utilisation du CPU :

- `Fast` : Encodage le plus rapide, utilisation CPU plus faible
- `Standard` : Vitesse et qualité équilibrées (par défaut)
- `High` : Meilleure qualité, utilisation CPU plus élevée

### Scénarios courants

#### Capture musicale de haute qualité en C#

```csharp
// Configurer les paramètres pour un enregistrement musical de haute qualité
var highQualitySettings = new MP3EncoderSettings
{
    // Utiliser un débit variable basé sur la qualité pour une fidélité audio optimale
    RateControl = MP3EncoderRateControl.Quality,
    // Réglage de qualité le plus élevé (0.0f) pour une fidélité audio maximale
    Quality = 0.0f,
    // Utiliser l'algorithme d'encodage haute qualité (plus intensif CPU mais meilleurs résultats)
    EncodingEngineQuality = MP3EncodingQuality.High
};
```

#### Audio en streaming

```csharp
// Configurer les paramètres optimisés pour les applications de streaming audio
var streamingSettings = new MP3EncoderSettings
{
    // Utiliser un débit constant pour des performances de streaming prévisibles
    RateControl = MP3EncoderRateControl.CBR,
    // 128 kbps offre une bonne qualité pour la plupart du contenu tout en étant économe en bande passante
    Bitrate = 128,
    // L'encodage rapide réduit l'utilisation CPU, important pour le streaming en temps réel
    EncodingEngineQuality = MP3EncodingQuality.Fast
};
```

## Sortie MP3 Windows uniquement

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La classe [sortie de fichier MP3](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP3Output.html) offre des options de configuration avancées pour l'encodage MP3 dans des scénarios de capture et d'édition audio vidéo en C#.

### Caractéristiques principales

- Sélection flexible du mode de canal
- Prise en charge de l'encodage VBR et CBR pour un enregistrement MP3 .NET optimal
- Paramètres d'encodage avancés pour les applications audio professionnelles
- Réglages de contrôle de qualité pour des résultats d'édition MP3 parfaits en C#

### Configuration de base

#### CBR_Bitrate

Contrôle le réglage de débit constant (CBR) pour l'encodage MP3.

- Pour MPEG-1 (32, 44.1, 48 kHz) : Valeurs valides 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 kbps
- Pour MPEG-2 (16, 22.05, 24 kHz) : Valeurs valides 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 kbps
- Valeurs par défaut : 128 kbps (MPEG-1) ou 64 kbps (MPEG-2)

#### SampleRate

Spécifie la fréquence d'échantillonnage audio en Hz. Valeurs courantes :

- 44100 Hz (qualité CD, par défaut)
- 48000 Hz (audio professionnel)
- 32000 Hz (diffusion)
- 22050 Hz (qualité inférieure)
- 16000 Hz (voix)

#### ChannelsMode

Détermine comment les canaux audio sont encodés. Les options incluent :

1. StandardStereo : Encodage indépendant des canaux avec allocation dynamique des bits
2. JointStereo : Exploite la corrélation entre canaux à l'aide de l'encodage mid/side
3. DualStereo : Encodage indépendant avec allocation fixe 50/50 des bits (idéal pour double langue)
4. Mono : Sortie sur un seul canal (mixe en mono une entrée stéréo)

### Paramètres de débit variable (VBR)

#### VBR_Mode

Active l'encodage à débit variable lorsque défini à true (par défaut). Le VBR permet à l'encodeur d'ajuster le débit en fonction de la complexité audio.

#### VBR_MinBitrate

Définit le débit minimum autorisé pour l'encodage VBR (par défaut : 96 kbps).

#### VBR_MaxBitrate

Définit le débit maximum autorisé pour l'encodage VBR (par défaut : 192 kbps).

#### VBR_Quality

Contrôle la qualité de l'encodage VBR (0-9) :

- Valeurs plus faibles (0-4) : Meilleure qualité, encodage plus lent
- Valeurs intermédiaires (5-6) : Qualité et vitesse équilibrées
- Valeurs plus élevées (7-9) : Qualité inférieure, encodage plus rapide

### Qualité et performance

#### EncodingQuality

Détermine la qualité algorithmique de l'encodage (0-9) :

- 0-1 : Meilleure qualité, encodage le plus lent
- 2 : Recommandé pour la haute qualité
- 5 : Par défaut, bon équilibre entre vitesse et qualité
- 7 : Encodage rapide avec qualité acceptable
- 9 : Encodage le plus rapide, qualité la plus faible

### Fonctionnalités spéciales

#### ForceMono

Lorsqu'activé, mixe automatiquement l'audio multicanal en mono.

#### VoiceEncodingMode

Mode expérimental optimisé pour le contenu vocal.

#### KeepAllFrequencies

Désactive le filtrage automatique des fréquences, préservant toutes les fréquences au prix d'une efficacité moindre.

#### DisableShortBlocks

Force l'utilisation des blocs longs uniquement, ce qui peut améliorer la qualité à très faible débit mais peut provoquer des artefacts de pré-écho.

### Indicateurs de trame MP3

#### Copyright

Définit le bit de copyright dans les trames MP3.

#### Original

Marque le flux comme contenu original.

#### CRCProtected

Active la détection d'erreur CRC au prix de 16 bits par trame.

#### EnableXingVBRTag

Ajoute des en-têtes d'informations VBR pour une meilleure compatibilité avec les lecteurs.

#### StrictISOCompliance

Impose une conformité stricte au standard ISO MP3.

### Exemples de configurations d'enregistrement et d'édition MP3

Paramètres de base pour les applications de capture MP3 en C#.

```csharp
// Configurer une sortie MP3 basique avec des paramètres standard
var mp3Output = new MP3Output
{
    // 192 kbps offre une bonne qualité pour la plupart des contenus musicaux
    CBR_Bitrate = 192,
    // Fréquence d'échantillonnage de qualité CD
    SampleRate = 44100,
    // Le mode joint stereo offre une meilleure compression pour la plupart du contenu stéréo
    ChannelsMode = MP3ChannelsMode.JointStereo,
};

// Définir comme format de sortie pour la capture ou l'édition
core.Output_Format = mp3Output; // Core est VideoCaptureCore ou VideoEditCore
```

Configuration VBR.

```csharp
// Configurer une sortie MP3 avec débit variable pour un meilleur équilibre qualité/taille
var mp3Output = new MP3Output
{    
    // Activer l'encodage à débit variable
    VBR_Mode = true,
    // Définir un plancher de débit minimum pour assurer une qualité acceptable
    VBR_MinBitrate = 96,
    // Limiter le débit maximum pour contrôler la taille du fichier
    VBR_MaxBitrate = 192,
    // Le niveau de qualité 6 offre un bon équilibre entre qualité et taille de fichier
    VBR_Quality = 6,
};

// Définir comme format de sortie pour la capture ou l'édition
core.Output_Format = mp3Output; // Core est VideoCaptureCore ou VideoEditCore
```

#### Encodage MP3 stéréo basique

```csharp
// Configurer l'encodage MP3 stéréo standard avec débit fixe
var mp3Output = new MP3Output
{
    // 192 kbps offre une bonne qualité pour la plupart de la musique tout en maintenant une taille de fichier raisonnable
    CBR_Bitrate = 192,
    // Le mode stéréo standard encode les canaux gauche et droit indépendamment
    ChannelsMode = MP3ChannelsMode.StandardStereo,
    // Fréquence d'échantillonnage de qualité CD
    SampleRate = 44100,
    // Désactiver le débit variable pour assurer une taille de fichier et une lecture cohérentes
    VBR_Mode = false
};
```

#### Encodage optimisé pour la voix

```csharp
// Configurer les paramètres MP3 optimisés pour les enregistrements vocaux
var voiceMP3 = new MP3Output
{
    // Activer les algorithmes d'encodage optimisés pour la voix
    VoiceEncodingMode = true,
    // Utiliser le mono pour la voix afin de réduire la taille du fichier (la voix ne bénéficie généralement pas de la stéréo)
    ChannelsMode = MP3ChannelsMode.Mono,
    // Une fréquence d'échantillonnage plus basse suffit pour le contenu vocal
    SampleRate = 22050,
    // Activer le débit variable pour un meilleur rapport qualité/taille
    VBR_Mode = true,
    // Meilleur réglage de qualité pour la clarté vocale tout en maintenant une taille de fichier raisonnable
    VBR_Quality = 4
};
```

#### Encodage musical haute qualité

```csharp
// Configurer les paramètres MP3 haute qualité pour l'archivage musical
var highQualityMP3 = new MP3Output
{
    // Activer le débit variable pour un rapport qualité/taille optimal
    VBR_Mode = true,
    // Définir le débit minimum pour assurer une bonne qualité même dans les passages simples
    VBR_MinBitrate = 128,
    // Permettre un débit élevé pour les passages complexes afin de préserver les détails audio
    VBR_MaxBitrate = 320,
    // Utiliser un réglage de haute qualité (2) pour une excellente fidélité audio
    VBR_Quality = 2,
    // Définir l'algorithme de l'encodeur en mode haute qualité
    EncodingQuality = 2,
    // Le joint stereo offre une meilleure compression pour la plupart du contenu musical
    ChannelsMode = MP3ChannelsMode.JointStereo,
    // Fréquence d'échantillonnage audio professionnelle captant tout le spectre audible
    SampleRate = 48000,
    // Ajouter l'en-tête VBR pour une meilleure compatibilité avec les lecteurs et le positionnement
    EnableXingVBRTag = true
};
```

### Paramètres avancés

- **Protection CRC** : Ajoute une capacité de détection d'erreur au prix de 16 bits par trame
- **Blocs courts** : Peuvent être désactivés pour potentiellement augmenter la qualité à très bas débit
- **Plage de fréquences** : Option pour conserver toutes les fréquences (désactive le filtre passe-bas automatique)
- **Mode voix** : Mode expérimental optimisé pour le contenu vocal

### Bonnes pratiques

1. **Choisir le contrôle de débit selon l'application**
   - Utilisez CBR pour le streaming et la capture MP3 en temps réel en C#
   - Utilisez le VBR par qualité pour l'archivage et l'enregistrement MP3 .NET de qualité maximale
   - Utilisez ABR lorsque vous avez besoin d'un équilibre entre taille cohérente et qualité

2. **Réglages de qualité selon le cas d'usage**
   - Pour l'archivage : Utilisez VBR avec qualité 0-2
   - Pour la capture audio vidéo générale en C# : VBR avec qualité 3-5 ou CBR 192-256 kbps
   - Pour l'enregistrement vocal en .NET : Envisagez d'utiliser le mode d'encodage vocal avec des débits plus faibles

3. **Sélection du mode de canal**
   - Utilisez Joint Stereo pour la plupart des contenus musicaux
   - Utilisez Standard Stereo pour l'écoute critique et les mixages stéréo complexes
   - Utilisez Mono pour les enregistrements vocaux ou lorsque la bande passante est critique

4. **Optimisation des performances**
   - Utilisez la qualité d'encodage Fast pour les applications temps réel
   - Utilisez la qualité Standard pour l'encodage générique
   - Utilisez la qualité High uniquement pour l'archivage où le temps d'encodage n'est pas critique

### Notes sur les valeurs par défaut

Le constructeur de la classe définit ces valeurs par défaut :

- CBR_Bitrate = 192 kbps
- VBR_MinBitrate = 96 kbps
- VBR_MaxBitrate = 192 kbps
- VBR_Quality = 6
- EncodingQuality = 6
- SampleRate = 44100 Hz
- ChannelsMode = MP3ChannelsMode.StandardStereo
- VBR_Mode = true
