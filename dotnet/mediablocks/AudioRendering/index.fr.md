---
title: Rendu et lecture audio multiplateformes en C# .NET
description: Lisez de l'audio vers les haut-parleurs sous Windows, macOS, Linux, iOS et Android avec VisioForge Media Blocks SDK. Choix de périphérique, tampon, volume.
sidebar_label: Rendu audio
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
primary_api_classes:
  - AudioRendererBlock
  - OSXAudioSinkBlock
  - IOSAudioSinkBlock
  - OSXAudioSinkSettings
  - MediaBlocksPipeline

---

# Bloc de rendu audio : traitement de sortie audio multiplateforme

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au rendu audio

Le bloc moteur de rendu audio sert de composant critique dans les pipelines de traitement multimédia, permettant aux applications de produire des flux audio vers des périphériques sonores sur de multiples plateformes. Ce bloc polyvalent gère la tâche complexe de conversion des données audio numériques en son audible via les interfaces matérielles appropriées, ce qui en fait un outil essentiel pour les développeurs construisant des applications avec audio.

Le rendu audio nécessite une gestion soigneuse des ressources matérielles, des paramètres de tampon et de la synchronisation temporelle pour garantir une lecture fluide et ininterrompue. Ce bloc abstrait ces complexités et fournit une interface unifiée pour la sortie audio dans divers environnements informatiques.

## Fonctionnalités principales

Le bloc moteur de rendu audio accepte des flux audio non compressés et les envoie soit vers le périphérique audio par défaut, soit vers une alternative sélectionnée par l'utilisateur. Il fournit des contrôles essentiels de lecture audio, notamment :

- Réglage du volume avec contrôle précis en décibels
- Fonctionnalité de mise en sourdine pour un fonctionnement silencieux
- Sélection du périphérique parmi les sorties audio système disponibles
- Paramètres de mise en tampon pour optimiser la latence ou la stabilité

Ces capacités permettent aux développeurs de créer des applications avec une sortie audio de niveau professionnel sans avoir besoin d'implémenter du code spécifique à la plateforme pour chaque système d'exploitation cible.

## Technologie sous-jacente

### Implémentation spécifique à la plateforme

L'`AudioRendererBlock` prend en charge diverses technologies de rendu audio spécifiques à la plateforme. Il peut être configuré pour utiliser un périphérique audio et une API spécifiques (voir la section Gestion des périphériques). Lorsqu'il est instancié à l'aide de son constructeur par défaut (par ex. `new AudioRendererBlock()`), il lit le son sur le périphérique de sortie par défaut du système : l'élément `autoaudiosink` de GStreamer choisit le puits de la plateforme et suit la sortie par défaut du système d'exploitation, et `Settings` reste `null`. Si aucune sortie ne peut être ouverte, l'audio part vers un puits factice et un avertissement est journalisé. Les technologies audio prises en charge par système d'exploitation :

- **Windows** : le bloc prend en charge plusieurs API audio, notamment :
  - DirectSound : fournit une sortie à faible latence avec une large compatibilité
  - WASAPI (Windows Audio Session API) : offre un mode exclusif pour la plus haute qualité
  - ASIO (Audio Stream Input/Output) : audio de qualité professionnelle avec une latence minimale pour le matériel spécialisé
- **macOS** : utilise le framework CoreAudio pour :
  - Une sortie audio haute résolution
  - Une intégration native avec le sous-système audio de macOS
  - La prise en charge des audio units et du matériel professionnel
  (Remarque : de même pour macOS, un `OSXAudioSinkBlock` est disponible pour interagir directement avec le puits GStreamer spécifique à la plateforme si nécessaire pour des scénarios spécialisés.)
- **Linux** : implémente ALSA (Advanced Linux Sound Architecture) pour un périphérique sélectionné ; sans périphérique, le constructeur par défaut lit le son via le puits de plus haut rang qui s'ouvre : PulseAudio (y compris sur les systèmes PipeWire, via sa couche PulseAudio) ou ALSA. ALSA fournit :
  - Un accès matériel direct
  - Une prise en charge complète des périphériques
  - Une intégration avec la pile audio Linux
- **iOS** : utilise CoreAudio, optimisé pour le mobile, permettant des fonctionnalités telles que :
  - Un rendu économe en énergie
  - Des capacités audio en arrière-plan
  - L'intégration avec la gestion des sessions audio iOS
  (Remarque : pour les développeurs nécessitant un contrôle plus direct sur le puits GStreamer spécifique à iOS ou ayant des cas d'usage avancés, le SDK fournit également `IOSAudioSinkBlock` comme bloc multimédia distinct.)
- **Android** : utilise OpenSL ES pour fournir :
  - Une sortie audio à faible latence
  - L'accélération matérielle lorsqu'elle est disponible

## ASIOSinkBlock : sortie audio ASIO

`ASIOSinkBlock` lit l'audio via un périphérique Steinberg ASIO sous Windows, sur un ensemble choisi de canaux de sortie.

Utilisez-le quand la sortie doit atteindre une interface audio professionnelle avec une faible latence ou sur des sorties précises, par exemple un mixage de retour sur les sorties 3 et 4. Pour lire sur une carte son ordinaire, `AudioRendererBlock` est plus simple et ne demande aucun pilote supplémentaire.

### Paramètres : `ASIOSinkSettings`

- `Channels` : indices de canaux de sortie à partir de zéro, dans l'ordre croissant ; `null` ou vide lit sur tous les canaux. Un indice que le périphérique n'a pas arrête le démarrage avec une erreur.
- `BufferSize` : la taille du tampon ASIO en échantillons ; 0 utilise la taille préférée du pilote. Une taille hors de la plage ou de la granularité du pilote arrête le démarrage avec une erreur.

`ASIOSinkBlock.GetDevicesAsync()` renvoie les sorties ASIO, la même liste que `DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.ASIO)`, et `ASIOSinkBlock.IsAvailable()` indique si le plugin ASIO est présent. Une sortie ASIO peut aussi être passée à `AudioRendererBlock` comme un périphérique ordinaire.

### Exemple d'intégration

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("music.wav", renderVideo: false));

var device = (await ASIOSinkBlock.GetDevicesAsync())[0];
var sink = new ASIOSinkBlock(new ASIOSinkSettings(device)
{
    Channels = new[] { 0, 1 }, // zero-based; null or empty plays on every channel
});

pipeline.Connect(source.AudioOutput, sink.Input);

await pipeline.StartAsync();
```

ASIO nécessite le paquet `VisioForge.CrossPlatform.Core.Windows.Adds.x64` et un pilote ASIO installé.

## OSXAudioSinkBlock : sortie audio macOS directe

L'`OSXAudioSinkBlock` est un bloc multimédia spécifique à la plateforme conçu pour les scénarios avancés où une interaction directe avec le puits audio GStreamer macOS est requise. Ce bloc est utile pour les développeurs qui ont besoin d'un contrôle de bas niveau sur la sortie audio sur les périphériques macOS, comme la sélection personnalisée de périphérique ou l'intégration avec d'autres composants natifs.

### Fonctionnalités clés

- Accès direct au puits audio macOS
- Sélection de périphérique via `DeviceID`
- Adapté aux applications audio spécialisées ou professionnelles sur macOS

### Paramètres : `OSXAudioSinkSettings`

L'`OSXAudioSinkBlock` nécessite un objet `OSXAudioSinkSettings` pour spécifier le périphérique de sortie audio. La classe `OSXAudioSinkSettings` vous permet de définir :

- `DeviceID` : l'ID du périphérique de sortie audio macOS (à partir de 0)

Exemple :

```csharp
using VisioForge.Core.Types.X.Sinks;

// Sélectionner le premier périphérique audio disponible (DeviceID = 0)
var osxSettings = new OSXAudioSinkSettings { DeviceID = 0 };

// Créer le bloc puits audio macOS
var osxAudioSink = new OSXAudioSinkBlock(osxSettings);
```

### Vérification de disponibilité

Vous pouvez vérifier si l'`OSXAudioSinkBlock` est disponible sur la plateforme courante :

```csharp
bool isAvailable = OSXAudioSinkBlock.IsAvailable();
```

### Exemple d'intégration

Voici un exemple minimal d'intégration d'`OSXAudioSinkBlock` dans un pipeline multimédia :

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer votre bloc source audio selon vos besoins
var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());

// Définir les paramètres du puits
var osxSettings = new OSXAudioSinkSettings { DeviceID = 0 };
var osxAudioSink = new OSXAudioSinkBlock(osxSettings);

// Connecter la source au puits audio macOS
pipeline.Connect(audioSourceBlock.Output, osxAudioSink.Input);

await pipeline.StartAsync();
```

## IOSAudioSinkBlock : sortie audio iOS directe

L'`IOSAudioSinkBlock` est un bloc multimédia spécifique à la plateforme conçu pour les scénarios avancés où une interaction directe avec le puits audio GStreamer iOS est requise. Ce bloc est utile pour les développeurs qui ont besoin d'un contrôle de bas niveau sur la sortie audio sur les périphériques iOS, comme le routage audio personnalisé, la gestion de formats ou l'intégration avec d'autres composants natifs.

### Fonctionnalités clés

- Accès direct au puits audio GStreamer iOS
- Contrôle du format audio et de la fréquence d'échantillonnage ; le nombre de canaux suit la sortie audio
- Adapté aux applications audio spécialisées ou professionnelles sur iOS

### Paramètres : `AudioInfoX`

L'`IOSAudioSinkBlock` nécessite un objet `AudioInfoX` pour spécifier le format audio. La classe `AudioInfoX` vous permet de définir :

- `Format` : le format d'échantillon audio (par ex. `AudioFormatX.S16LE`, `AudioFormatX.F32LE`, etc.)
- `SampleRate` : la fréquence d'échantillonnage en Hz (par ex. 44100, 48000) ; 0 reprend celle de l'entrée
- `Channels` : la sortie audio choisit le nombre de canaux : le haut-parleur intégré et les écouteurs en prennent 1 ou 2, une entrée multicanal est donc réduite. Au-delà de 2, le bloc demande ce nombre à une sortie qui en accepte davantage (HDMI, AirPlay, USB) ; cette préférence vaut pour toute l'application. 0 ne demande rien

Exemple :

```csharp
using VisioForge.Core.Types.X;

// Définir le format audio : 16 bits signé little-endian, 44100 Hz ; la stéréo ne demande rien à la sortie
var audioInfo = new AudioInfoX(AudioFormatX.S16LE, 44100, 2);

// Créer le bloc puits audio iOS
var iosAudioSink = new IOSAudioSinkBlock(audioInfo);
```

### Vérification de disponibilité

Vous pouvez vérifier si l'`IOSAudioSinkBlock` est disponible sur la plateforme courante :

```csharp
bool isAvailable = IOSAudioSinkBlock.IsAvailable();
```

### Exemple d'intégration

Voici un exemple minimal d'intégration d'`IOSAudioSinkBlock` dans un pipeline multimédia :

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer votre bloc source audio selon vos besoins
var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());

// Définir le format audio pour le puits
var audioInfo = new AudioInfoX(AudioFormatX.S16LE, 44100, 2);
var iosAudioSink = new IOSAudioSinkBlock(audioInfo);

// Connecter la source au puits audio iOS
pipeline.Connect(audioSourceBlock.Output, iosAudioSink.Input);

await pipeline.StartAsync();
```

## Spécifications techniques

### Informations sur le bloc

Nom : AudioRendererBlock

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Audio en entrée | audio non compressé | 1 |

### Prise en charge des formats audio

Le bloc moteur de rendu audio accepte une large gamme de formats audio non compressés :

- Fréquences d'échantillonnage : 8 kHz à 192 kHz
- Profondeurs de bits : 8 bits, 16 bits, 24 bits et 32 bits (virgule flottante)
- Configurations de canaux : mono, stéréo et multicanal (jusqu'à 7.1 surround)

Cette flexibilité permet aux développeurs de travailler avec tout, des applications vocales basiques aux expériences musicales haute fidélité et audio immersives.

## Gestion des périphériques

### Énumération des périphériques disponibles

Le bloc moteur de rendu audio fournit des méthodes simples pour découvrir et sélectionner parmi les périphériques de sortie audio disponibles sur le système à l'aide de la méthode statique `GetDevicesAsync` :

```csharp
// Obtenir une liste de tous les périphériques de sortie audio sur le système courant
var availableDevices = await AudioRendererBlock.GetDevicesAsync();

// Optionnellement, spécifier l'API à utiliser
var directSoundDevices = await AudioRendererBlock.GetDevicesAsync(AudioOutputDeviceAPI.DirectSound);

// Afficher les informations sur les périphériques
foreach (var device in availableDevices)
{
    Console.WriteLine($"Périphérique : {device.Name}");
}

// Créer un moteur de rendu avec un périphérique spécifique
var audioRenderer = new AudioRendererBlock(availableDevices[0]);
```

### Gestion du périphérique par défaut

Lorsqu'aucun périphérique spécifique n'est sélectionné, le bloc dirige automatiquement l'audio vers le périphérique de sortie par défaut du système. Le constructeur sans paramètre le fait via l'élément `autoaudiosink` de GStreamer, qui suit la sortie par défaut du système d'exploitation ; `Settings` reste `null` :

```csharp
// Créer avec le périphérique par défaut
var audioRenderer = new AudioRendererBlock();
```

## Considérations de performance

### Gestion de la latence

La latence du rendu audio est critique pour de nombreuses applications. Le bloc prend sa configuration dans les paramètres passés à son constructeur (exposés par la propriété `Settings`, `null` pour le constructeur sans paramètre) et fournit un contrôle de synchronisation via la propriété `IsSync` :

```csharp
// Contrôler le comportement de synchronisation
audioRenderer.IsSync = true; // Activer la synchronisation (par défaut)

// Vérifier si une API spécifique est disponible sur cette plateforme
bool isDirectSoundAvailable = AudioRendererBlock.IsAvailable(AudioOutputDeviceAPI.DirectSound);
```

### Contrôle du volume et de la sourdine

L'AudioRendererBlock fournit un contrôle précis du volume et la fonctionnalité de mise en sourdine :

```csharp
// Définir le volume (plage de 0.0 à 1.0)
audioRenderer.Volume = 0.8; // 80 % du volume

// Obtenir le volume courant
double currentVolume = audioRenderer.Volume;

// Activer/désactiver la sourdine
audioRenderer.Mute = true; // Couper l'audio
audioRenderer.Mute = false; // Réactiver l'audio

// Vérifier l'état de sourdine
bool isMuted = audioRenderer.Mute;
```

### Utilisation des ressources

Le bloc moteur de rendu audio est conçu pour l'efficacité, avec des optimisations pour :

- L'utilisation du CPU pendant la lecture
- L'empreinte mémoire pour la gestion des tampons
- La consommation d'énergie sur les appareils mobiles

## Exemples d'intégration

### Configuration de pipeline basique

L'exemple suivant montre comment configurer un pipeline simple de rendu audio à l'aide d'une source audio virtuelle :

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());

// Créer le moteur de rendu audio avec les paramètres par défaut
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

### Pipeline audio réel

Pour une application plus pratique, voici comment capturer l'audio système et le restituer :

```mermaid
graph LR;
    SystemAudioSourceBlock-->AudioRendererBlock;
```

```csharp
var pipeline = new MediaBlocksPipeline();

// Capturer l'audio système
var systemAudioSource = new SystemAudioSourceBlock();

// Configurer le moteur de rendu audio
var audioRenderer = new AudioRendererBlock();
audioRenderer.Volume = 0.8f; // 80 % du volume

// Connecter les blocs
pipeline.Connect(systemAudioSource.Output, audioRenderer.Input);

// Démarrer le traitement
await pipeline.StartAsync();

// Laisser l'audio jouer pendant 10 secondes
await Task.Delay(TimeSpan.FromSeconds(10));

// Arrêter le pipeline
await pipeline.StopAsync();
```

## Compatibilité et prise en charge des plateformes

Le bloc moteur de rendu audio est conçu pour un fonctionnement multiplateforme, prenant en charge :

- Windows 10 et versions ultérieures
- macOS 12 et versions ultérieures
- Linux (Ubuntu, Debian, Fedora)
- iOS 18.0 et versions ultérieures
- Android 7.0 et versions ultérieures

Cette large prise en charge des plateformes permet aux développeurs de créer des expériences audio cohérentes sur différents systèmes d'exploitation et appareils.

## Conclusion

Le bloc moteur de rendu audio offre aux développeurs une solution puissante et flexible pour la sortie audio sur de multiples plateformes. En abstrayant les complexités des API audio spécifiques à chaque plateforme, il leur permet de se concentrer sur la création d'expériences audio exceptionnelles sans se soucier des détails d'implémentation sous-jacents.

Que vous construisiez un simple lecteur multimédia, une application professionnelle de montage audio ou une plateforme de communications en temps réel, le bloc moteur de rendu audio fournit les outils nécessaires pour une sortie audio fiable et de haute qualité.
