---
title: Intégration de l'encodeur audio vocal Speex pour SDK .NET
description: Implémentez la compression vocale Speex en .NET avec paramètres d'encodage vocal optimisés, contrôles de qualité et capture audio multiplateforme.
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
  - Speex
  - C#
primary_api_classes:
  - SpeexEncoderSettings
  - VideoCaptureCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline
  - SpeexOutput

---

# Encodeur audio Speex pour .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à Speex

Speex est un codec audio libre de brevets spécifiquement conçu pour l'encodage de la parole dans les applications .NET. Que vous ayez besoin de capturer, d'éditer ou d'enregistrer de l'audio en C#, Speex offre une excellente compression tout en maintenant la qualité vocale à divers débits. VisioForge intègre ce puissant encodeur dans ses SDK .NET, offrant aux développeurs des options de configuration flexibles pour les applications basées sur la parole. Le codec convient particulièrement aux développeurs C# qui souhaitent implémenter des fonctionnalités de capture et d'enregistrement audio de haute qualité dans leurs applications.

## Fonctionnalité principale

L'encodeur Speex dans les SDK VisioForge prend en charge :

- Plusieurs bandes de fréquences pour différents niveaux de qualité
- Encodage à débit variable et fixe
- Détection d'activité vocale et compression du silence
- Paramètres de complexité et qualité ajustables
- Compatibilité multiplateforme sur Windows, macOS et Linux
- Intégration transparente avec les applications dotnet

## Implémentation multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Modes de l'encodeur

Speex propose quatre modes de fonctionnement optimisés pour différentes plages de fréquences :

| Mode | Valeur | Fréquence d'échantillonnage optimale |
|------|-------|-------------------|
| Auto | 0 | Sélection automatique en fonction de l'entrée |
| Ultra Wide Band | 1 | 32 kHz |
| Wide Band | 2 | 16 kHz |
| Narrow Band | 3 | 8 kHz |

L'encodeur ajuste automatiquement les paramètres internes en fonction du mode sélectionné. Pour la plupart des applications vocales, Wide Band (mode 2) offre un excellent équilibre entre qualité et utilisation de la bande passante.

## Spécifications techniques

### Fréquences d'échantillonnage prises en charge

Speex fonctionne avec trois fréquences d'échantillonnage standard :

- 8 000 Hz - Idéal pour l'audio de qualité téléphonique (Narrow Band)
- 16 000 Hz - Recommandé pour la plupart des applications vocales (Wide Band)
- 32 000 Hz - Encodage vocal de la plus haute qualité (Ultra Wide Band)

### Configuration des canaux

L'encodeur gère à la fois :

- Mono (1 canal) - Idéal pour les enregistrements vocaux
- Stéréo (2 canaux) - Pour les enregistrements multi-locuteurs ou audio immersif

## Méthodes de contrôle de débit

### Encodage basé sur la qualité

Pour une qualité perceptive cohérente, utilisez le paramètre `Quality` :

```csharp
var settings = new SpeexEncoderSettings {
    Quality = 8.0f, // Plage de 0 (la plus basse) à 10 (la plus haute)
    VBR = false     // Mode qualité fixe
};
```

Des valeurs de qualité plus élevées produisent un meilleur audio au prix d'une taille de fichier accrue. La plupart des applications vocales fonctionnent bien avec des valeurs de qualité entre 5 et 8.

### Débit variable (VBR)

Le VBR ajuste dynamiquement le débit en fonction de la complexité de la parole :

```csharp
var settings = new SpeexEncoderSettings {
    VBR = true,
    Quality = 8.0f  // Niveau de qualité cible
};
```

Cette approche permet généralement d'économiser de la bande passante tout en maintenant une qualité perçue cohérente, ce qui la rend idéale pour les applications de streaming.

### Débit moyen (ABR)

L'ABR maintient un débit cible dans le temps tout en permettant des fluctuations de qualité :

```csharp
var settings = new SpeexEncoderSettings {
    ABR = 15.0f,   // Débit cible en kbps
    VBR = true     // Requis pour le mode ABR
};
```

Cette option fonctionne bien lorsque vous avez besoin de tailles de fichier ou d'une utilisation de bande passante prévisibles.

### Encodage à débit fixe

Pour des débits constants tout au long du processus d'encodage :

```csharp
var settings = new SpeexEncoderSettings {
    Bitrate = 24.6f,  // Débit fixe en kbps
    VBR = false
};
```

Les débits pris en charge vont de 2,15 kbps à 24,6 kbps :

- 2,15 kbps - Parole ultra-compressée (qualité limitée)
- 3,95 kbps - Voix à faible bande passante
- 5,95 kbps - Clarté de parole basique
- 8,00 kbps - Qualité vocale standard
- 11,0 kbps - Bonne reproduction de la parole
- 15,0 kbps - Parole quasi transparente
- 18,2 kbps - Voix de haute qualité
- 24,6 kbps - Qualité maximale de parole

## Fonctionnalités d'optimisation vocale

### Détection d'activité vocale (VAD)

Le VAD identifie la présence de parole dans les signaux audio :

```csharp
var settings = new SpeexEncoderSettings {
    VAD = true,    // Activer la détection vocale
    DTX = true     // Recommandé avec VAD
};
```

Cette fonctionnalité améliore l'efficacité de la bande passante en concentrant les ressources d'encodage sur les segments de parole réels.

### Transmission discontinue (DTX)

Le DTX réduit la transmission de données pendant les périodes de silence :

```csharp
var settings = new SpeexEncoderSettings {
    DTX = true     // Activer la compression du silence
};
```

Pour la VoIP et les communications temps réel, activer DTX peut réduire significativement les exigences de bande passante.

### Complexité d'encodage

Contrôlez l'utilisation CPU par rapport à la qualité d'encodage :

```csharp
var settings = new SpeexEncoderSettings {
    Complexity = 3  // Plage : 1 (le plus rapide) à 10 (qualité la plus élevée)
};
```

Des valeurs plus faibles privilégient la vitesse et réduisent la charge CPU, tandis que des valeurs plus élevées améliorent la qualité audio au prix des performances.

## Exemples d'implémentation

### Vérification de la disponibilité de l'encodeur

Vérifiez toujours la disponibilité de l'encodeur avant d'implémenter Speex dans votre application C# :

```csharp
if (!SpeexEncoderSettings.IsAvailable())
{
    throw new InvalidOperationException("Speex encoder not available on this system.");
}
```

### Configuration de base pour la capture audio

Voici comment configurer l'encodage Speex de base pour la capture audio en dotnet :

```csharp
var encoderSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    Quality = 7.0f
};
```

### Optimisé pour l'enregistrement vocal

Pour les applications d'enregistrement vocal en .NET, utilisez ces paramètres optimisés :

```csharp
var voipSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 6.0f,
    Complexity = 4
};
```

### Capture audio de la plus haute qualité

Pour une capture audio de qualité maximale en dotnet :

```csharp
var highQualitySettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.UltraWideBand,
    SampleRate = 32000,
    Channels = 2,
    Bitrate = 24.6f,
    Complexity = 8
};
```

## Intégration SDK

### Intégration Video Capture SDK

Apprenez à capturer de l'audio avec Speex dans votre application C# :

```csharp
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Créer une instance du noyau Video Capture SDK
var core = new VideoCaptureCoreX();

// Définir le périphérique d'entrée audio, filtrer par API
var api = AudioCaptureDeviceAPI.DirectSound;
var audioInputDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault(x => x.API == api);
if (audioInputDevice == null)
{
    MessageBox.Show("No audio input device found.");
    return;
}

var audioInput = new AudioCaptureDeviceSourceSettings(api, audioInputDevice, audioInputDevice.GetDefaultFormat());

core.Audio_Source = audioInput;

// Configurer les paramètres Speex
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    Quality = 7.0f
};

var speexOutput = new SpeexOutput("output.spx", speexSettings);

// Ajouter la sortie Speex
core.Outputs_Add(speexOutput, true);

// Définir le mode d'enregistrement audio
core.Audio_Record = true;
core.Audio_Play = false;

// Démarrer la capture
await core.StartAsync();

// Arrêter après 10 secondes
await Task.Delay(10000);

// Arrêter la capture
await core.StopAsync();
```

### Intégration Video Edit SDK

Éditez et traitez des fichiers audio avec Speex en dotnet :

```csharp
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Créer une instance du noyau Video Edit SDK
var core = new VideoEditCoreX();

// Ajouter le fichier source audio
var audioFile = new AudioFileSource(@"c:\samples\!audio.mp3");
core.Input_AddAudioFile(audioFile, null);

// Configurer les paramètres Speex
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    Quality = 7.0f
};

var speexOutput = new SpeexOutput(@"output.spx", speexSettings);

// Ajouter la sortie Speex
core.Output_Format = speexOutput;

// Intercepter l'événement OnStop
core.OnStop += (s, e) =>
{
    // Gérer ici l'événement d'arrêt
    MessageBox.Show("Editing complete.");
};

core.OnProgress += (s, e) =>
{
    // Gérer ici les mises à jour de progression
    Debug.WriteLine($"Progress: {e.Progress}%");
};

core.OnError += (s, e) =>
{
    // Gérer ici les erreurs
    Debug.WriteLine($"Error: {e.Message}");
};

// Démarrer l'édition
core.Start();
```

### Intégration Media Blocks SDK

Traitez les flux audio avec Speex dans votre application .NET :

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;

using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Créer un nouveau pipeline
var pipeline = new MediaBlocksPipeline();

// Ajouter une source universelle pour lire le fichier audio
var sourceSettings = await UniversalSourceSettings.CreateAsync(@"c:\samples\!audio.mp3", renderVideo: false, renderAudio: true);
var source = new UniversalSourceBlock(sourceSettings);

// Ajouter la sortie Speex
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.NarrowBand,
    SampleRate = 8000,
    DTX = true,
    VAD = true
};

var speexOutput = new OGGSpeexOutputBlock("output.spx", speexSettings);

// Connecter
pipeline.Connect(source.AudioOutput, speexOutput.Input);

// Ajouter un gestionnaire d'événement OnStop
pipeline.OnStop += (sender, e) =>
{
    // Faire quelque chose lorsque le pipeline s'arrête
    MessageBox.Show("Conversion complete");
};

// Démarrer
await pipeline.StartAsync();
```

## Optimisation des performances

Lors de l'implémentation de l'encodage Speex, considérez ces stratégies d'optimisation :

1. **Faites correspondre la fréquence d'échantillonnage au contenu** - Utilisez Narrow Band (8 kHz) pour l'audio téléphonique, Wide Band (16 kHz) pour la plupart des applications vocales et Ultra Wide Band (32 kHz) uniquement lorsqu'une qualité maximale est requise

2. **Activez VBR avec VAD/DTX** pour le contenu vocal - Cette combinaison offre une efficacité de bande passante optimale pour les enregistrements vocaux typiques

3. **Ajustez la complexité selon la plateforme** - Les applications mobiles peuvent bénéficier de valeurs de complexité plus faibles (2-4), tandis que les applications de bureau peuvent utiliser des valeurs plus élevées (5-8)

4. **Utilisez ABR pour le streaming** - Le débit moyen offre une utilisation prévisible de la bande passante tout en maintenant une flexibilité de qualité

5. **Testez différents réglages de qualité** - Souvent un réglage de qualité de 5-7 produit d'excellents résultats sans taille de fichier excessive

## Cas d'usage

L'encodage Speex excelle dans ces scénarios de développement :

- Applications VoIP et téléphonie Internet
- Fonctionnalités de chat vocal dans les jeux et outils collaboratifs
- Création et distribution de podcasts
- Prétraitement de reconnaissance vocale
- Applications de notes vocales
- Archivage audio de contenu parlé

## Installation et configuration

Pour démarrer avec Speex dans votre application dotnet, consultez le guide d'installation principal [ici](../../install/index.md).

## Cas d'usage courants

### Capture et enregistrement audio

Pour les applications de streaming, utilisez ces paramètres optimisés :

```csharp
var streamingSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 6.0f,
    Complexity = 3
};
```

### Applications de voix sur IP

Pour les applications VoIP, privilégiez la faible latence et l'efficacité de la bande passante :

```csharp
var voipSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.NarrowBand,
    SampleRate = 8000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 5.0f,
    Complexity = 2
};
```

## Licence et communauté

Speex est publié sous licence BSD, ce qui le rend libre d'utilisation tant pour un usage commercial que non commercial. Le codec est activement maintenu par la communauté open source, avec des mises à jour et améliorations régulières.

## Questions fréquemment posées

### Quel est le meilleur débit pour l'enregistrement vocal ?

Pour la plupart des applications vocales, un débit entre 8 et 15 kbps offre une excellente qualité tout en maintenant des tailles de fichier raisonnables. Utilisez le mode VBR pour des résultats optimaux.

### Comment Speex se compare-t-il à d'autres codecs ?

Speex offre une qualité de parole supérieure à de nombreux autres codecs à des débits similaires, en particulier pour le contenu vocal. Il est particulièrement efficace pour les applications à faible débit.

### Puis-je utiliser Speex pour l'encodage musical ?

Bien que Speex puisse encoder de la musique, il est spécifiquement optimisé pour la parole. Pour le contenu musical, envisagez d'utiliser d'autres codecs comme AAC ou MP3.

## Conclusion

L'implémentation Speex de VisioForge offre aux développeurs .NET un outil puissant pour capturer, éditer et enregistrer de l'audio dans les applications C#. Que vous construisiez une nouvelle application de capture vocale ou que vous amélioriez une application existante, Speex offre des résultats exceptionnels avec une utilisation minimale des ressources. La flexibilité et la performance du codec en font un excellent choix pour tout développeur .NET travaillant avec le traitement audio.
