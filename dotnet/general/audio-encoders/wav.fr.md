---
title: Encodeur audio WAV PCM en .NET — Paramètres et exemples
description: Implémentez le traitement audio WAV en .NET avec fréquences d'échantillonnage, configuration des canaux, choix du format PCM et prise en charge multiplateforme.
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
  - WAV
  - C#
primary_api_classes:
  - WAVEncoderSettings
  - VideoCaptureCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline
  - WAVOutput

---

# Implémenter l'audio WAV dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Qu'est-ce que le format WAV ?

WAV (Waveform Audio File Format) fonctionne comme un format de conteneur audio non compressé plutôt que comme un codec. Il stocke les données audio PCM (Pulse-Code Modulation) brutes sous leur forme native. Lorsque vous travaillez avec les SDK VisioForge, la fonctionnalité de sortie WAV permet aux développeurs de créer des fichiers audio de haute qualité avec des paramètres PCM configurables. Comme WAV préserve l'audio sans compression, il maintient la qualité sonore d'origine au prix de tailles de fichier plus importantes que les formats compressés comme MP3 ou AAC.

## Comment fonctionnent les fichiers WAV

Le format WAV stocke les échantillons audio sous leur forme brute. Lorsque votre application produit une sortie au format WAV, elle effectue trois opérations clés :

1. Organisation des données audio PCM brutes dans la structure de conteneur WAV
2. Définition des paramètres d'interprétation (fréquence d'échantillonnage, profondeur de bits et nombre de canaux)
3. Génération des en-têtes WAV et des métadonnées appropriés

Cette nature non compressée signifie que les tailles de fichier sont prévisibles et directement calculées à partir des paramètres audio :

```text
File size (bytes) = Sample Rate × Bit Depth × Channels × Duration / 8
```

Par exemple, un fichier WAV stéréo d'une minute échantillonné à 44,1 kHz avec des échantillons 16 bits consomme environ 10,1 Mo :

```text
44100 × 16 × 2 × 60 / 8 = 10,584,000 bytes
```

## Implémentation WAV multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Caractéristiques principales

- Configuration flexible du format audio (par défaut : S16LE)
- Fréquences d'échantillonnage ajustables de 8 kHz à 192 kHz
- Prise en charge des configurations mono et stéréo
- Qualité audio cohérente sur différentes plateformes

### Paramètres de configuration

#### Options de format audio

L'encodeur WAV prend en charge plusieurs formats audio via l'énumération `AudioFormatX`, avec S16LE (16 bits Little-Endian) comme format par défaut pour une compatibilité maximale.

#### Sélection de la fréquence d'échantillonnage

- Plage disponible : 8 000 Hz à 192 000 Hz
- Réglage par défaut : 48 000 Hz
- Valeurs d'incrément : pas de 8 000 Hz

#### Configuration des canaux

- Options disponibles : 1 (mono) ou 2 (stéréo)
- Réglage par défaut : 2 (stéréo)

### Exemples d'implémentation

#### Implémentation basique

```csharp
// Initialiser l'encodeur WAV avec les paramètres par défaut
var wavEncoder = new WAVEncoderSettings();
```

```csharp
// Initialiser avec une configuration personnalisée
var customWavEncoder = new WAVEncoderSettings(
    format: AudioFormatX.S16LE,
    sampleRate: 44100,
    channels: 2
);
```

#### Intégration avec Video Capture SDK

```csharp
// Initialiser le noyau Video Capture SDK
var core = new VideoCaptureCoreX();

// Créer la sortie WAV avec le chemin du fichier
var wavOutput = new WAVOutput("output.wav");

// Ajouter la sortie au pipeline de capture
core.Outputs_Add(wavOutput, true);
```

#### Intégration avec Video Edit SDK

```csharp
// Initialiser le noyau Video Edit SDK
var core = new VideoEditCoreX();

// Créer une instance de sortie WAV
var wavOutput = new WAVOutput("output.wav");

// Configurer le noyau pour utiliser la sortie WAV
core.Output_Format = wavOutput;
```

#### Configuration du pipeline Media Blocks

```csharp
// Initialiser les paramètres de l'encodeur WAV
var wavSettings = new WAVEncoderSettings();

// Créer le bloc d'encodeur
var wavOutput = new WAVEncoderBlock(wavSettings);

// Ajouter un bloc File Sink pour la sortie
var fileSink = new FileSinkBlock("output.wav");

// Connecter l'encodeur au puits de fichier dans le pipeline
pipeline.Connect(wavOutput.Output, fileSink.Input); // pipeline est MediaBlocksPipeline
```

#### Vérification de la disponibilité de l'encodeur

```csharp
if (WAVEncoderSettings.IsAvailable())
{
    // L'encodeur est disponible, procéder à l'encodage
    var encoder = new WAVEncoderSettings();
    // Configurer et utiliser l'encodeur
}
else
{
    // Gérer l'indisponibilité
    Console.WriteLine("WAV encoder is not available on this system");
}
```

#### Configuration avancée

```csharp
var wavEncoder = new WAVEncoderSettings
{
    Format = AudioFormatX.S16LE,
    SampleRate = 96000,
    Channels = 1  // Configurer pour l'audio mono
};
```

#### Créer un bloc d'encodeur

```csharp
var settings = new WAVEncoderSettings();
MediaBlock encoderBlock = settings.CreateBlock();
// Intégrer le bloc d'encodeur dans votre pipeline multimédia
```

#### Récupérer les paramètres pris en charge

```csharp
// Obtenir la liste des formats audio pris en charge
IEnumerable<string> formats = WAVEncoderSettings.GetFormatList();

// Obtenir les fréquences d'échantillonnage disponibles
var settings = new WAVEncoderSettings();
int[] sampleRates = settings.GetSupportedSampleRates();
// Retourne un tableau allant de 8000 à 192000 par pas de 8000 Hz

// Obtenir les configurations de canaux prises en charge
int[] channels = settings.GetSupportedChannelCounts();
// Retourne [1, 2] pour les options mono et stéréo
```

## Implémentation WAV spécifique à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

### Énumération des codecs audio disponibles

```csharp
// core est une instance de VideoCaptureCore ou VideoEditCore
foreach (var codec in core.Audio_Codecs)
{
    cbAudioCodecs.Items.Add(codec);
}
```

### Configuration des paramètres audio

```csharp
// Initialiser la sortie ACM pour WAV
var acmOutput = new ACMOutput();

// Configurer les paramètres audio
acmOutput.Channels = 2;
acmOutput.BPS = 16;
acmOutput.SampleRate = 44100;
acmOutput.Name = "PCM"; // nom du codec

// Définir comme format de sortie
core.Output_Format = acmOutput;
```

### Spécifier le fichier de sortie

```csharp
// Définir le chemin du fichier de sortie
core.Output_Filename = "output.wav";
```

### Démarrer le traitement

```csharp
// Démarrer l'opération de capture ou de conversion
await core.StartAsync();
```

## Bonnes pratiques pour l'implémentation WAV

### Recommandations pour la sélection de la fréquence d'échantillonnage

La fréquence d'échantillonnage impacte significativement la qualité audio et la taille du fichier :

- 8 kHz : Convient aux enregistrements vocaux basiques et aux applications de téléphonie
- 16 kHz : Qualité vocale améliorée pour les systèmes de reconnaissance vocale
- 44,1 kHz : Standard pour l'audio qualité CD et la production musicale
- 48 kHz : Standard audio professionnel utilisé en production vidéo
- 96 kHz et plus : Audio haute résolution pour l'ingénierie sonore professionnelle

Pour la plupart des applications, 44,1 kHz ou 48 kHz offre une excellente qualité sans tailles de fichier excessives.

### Stratégie de configuration des canaux

Votre sélection de canaux doit s'aligner sur les exigences du contenu :

- **Mono (1 canal)** : Idéal pour les enregistrements vocaux, podcasts ou lorsque l'espace de stockage est limité
- **Stéréo (2 canaux)** : Essentiel pour la musique, l'audio spatial ou tout contenu où le son directionnel importe

### Considérations sur le choix du format

Lors de la sélection des formats audio :

- S16LE (16 bits Little-Endian) offre la meilleure compatibilité sur l'ensemble des plateformes
- Des profondeurs de bits plus élevées (24 bits, 32 bits) offrent une plage dynamique plus large pour le travail audio professionnel
- Tenez compte des exigences de votre système cible et des capacités matérielles

## Limitations techniques et considérations

### Implications sur la taille des fichiers

Les fichiers WAV grandissent linéairement avec la durée d'enregistrement, ce qui peut présenter des défis :

- Un enregistrement stéréo de 10 minutes à 44,1 kHz/16 bits nécessite environ 100 Mo
- Pour les applications mobiles ou web, envisagez d'implémenter des limites de taille ou des options de compression
- Lorsque le streaming est requis, les formats compressés peuvent être plus appropriés

### Facteurs de performance

Le traitement WAV présente des caractéristiques de performance spécifiques :

- Utilisation CPU plus faible pendant l'encodage par rapport aux formats compressés
- Exigences d'E/S disque plus élevées en raison de volumes de données plus importants
- Considérations sur les tampons mémoire pour les longs enregistrements

## Conclusion

Le format WAV fournit aux développeurs une option de sortie audio fiable et de haute qualité dans les SDK .NET VisioForge. Sa nature non compressée garantit une qualité audio immaculée, ce qui le rend idéal pour les applications où la fidélité audio est primordiale. En tirant parti des options de configuration et des approches d'implémentation décrites ci-dessus, les développeurs peuvent intégrer efficacement la fonctionnalité audio WAV dans leurs applications .NET tout en maintenant des performances et une qualité optimales.

Pour la plupart des applications audio professionnelles, le WAV reste le format de choix pendant les étapes de production et d'édition, même si des formats compressés sont utilisés pour la distribution finale. La flexibilité et la compatibilité multiplateforme de l'implémentation WAV du SDK VisioForge en font un outil précieux dans la boîte à outils de traitement audio de tout développeur.
