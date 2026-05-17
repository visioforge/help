---
title: Capture et sortie SDI/HDMI Blackmagic Decklink en C#
description: Capturez et restituez la vidéo SDI/HDMI avec les cartes Blackmagic Decklink via VisioForge Media Blocks. Multi-périphériques, formats, keying.
sidebar_label: Blackmagic Decklink
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
primary_api_classes:
  - DecklinkVideoAudioSourceBlock
  - DecklinkAudioSinkSettings
  - DecklinkAudioSourceSettings
  - DecklinkVideoSinkSettings
  - DecklinkVideoSourceSettings

---

# Intégration Blackmagic Decklink avec le Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à l'intégration Decklink

Le VisioForge Media Blocks SDK pour .NET offre une prise en charge robuste des périphériques Blackmagic Decklink, permettant aux développeurs d'intégrer des fonctionnalités audio et vidéo de niveau professionnel dans leurs applications. Cette intégration assure des opérations de capture et de rendu fluides à l'aide du matériel haute qualité Decklink.

Notre SDK comprend des blocs spécialisés conçus spécifiquement pour les périphériques Decklink, vous donnant un accès complet à leurs capacités, notamment les entrées/sorties SDI, HDMI et autres. Ces blocs sont optimisés en performance et offrent une API simple pour implémenter des flux multimédias complexes.

### Capacités clés

- **Capture et rendu audio** : capturez et émettez de l'audio via les périphériques Decklink
- **Capture et rendu vidéo** : capturez et émettez de la vidéo dans divers formats et résolutions
- **Prise en charge multi-périphériques** : utilisez plusieurs périphériques Decklink simultanément
- **Options d'E/S professionnelles** : exploitez les interfaces SDI, HDMI et autres interfaces professionnelles
- **Traitement haute qualité** : maintenez la qualité vidéo/audio professionnelle tout au long du pipeline
- **Blocs audio/vidéo combinés** : gestion simplifiée de flux audio et vidéo synchronisés grâce à des blocs source et puits dédiés.

## Exigences système

Avant d'utiliser les blocs Decklink, vérifiez que votre système répond à ces exigences :

- **Matériel** : périphérique Blackmagic Decklink compatible
- **Logiciel** : SDK Blackmagic Decklink ou pilotes installés

## Types de blocs Decklink

Le SDK fournit plusieurs types de blocs pour travailler avec les périphériques Decklink :

1. **Bloc puits audio** : sortie audio vers les périphériques Decklink.
2. **Bloc source audio** : capture audio depuis les périphériques Decklink.
3. **Bloc puits vidéo** : sortie vidéo vers les périphériques Decklink.
4. **Bloc source vidéo** : capture vidéo depuis les périphériques Decklink.
5. **Bloc puits vidéo + audio** : sortie vidéo et audio synchronisée vers les périphériques Decklink à l'aide d'un seul bloc.
6. **Bloc source vidéo + audio** : capture vidéo et audio synchronisée depuis les périphériques Decklink à l'aide d'un seul bloc.

Chaque type de bloc est conçu pour gérer des opérations multimédias spécifiques tout en maintenant la synchronisation et la qualité.

## Utilisation du bloc Decklink Audio Sink

Le bloc Decklink Audio Sink permet la sortie audio vers les périphériques Blackmagic Decklink. Ce bloc gère les complexités du timing audio et de l'interfaçage matériel.

### Énumération des périphériques

Avant de créer un bloc puits audio, vous devez énumérer les périphériques disponibles :

```csharp
var devices = await DecklinkAudioSinkBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Périphérique trouvé : {item.Name}, Numéro de périphérique : {item.DeviceNumber}");
}
```

Ce code récupère tous les périphériques Decklink disponibles qui prennent en charge la sortie audio.

### Création et configuration du bloc

Une fois le périphérique cible identifié, vous pouvez créer et configurer le bloc puits audio :

```csharp
// Obtenir le premier périphérique disponible
var deviceInfo = (await DecklinkAudioSinkBlock.GetDevicesAsync()).FirstOrDefault();

// Créer les paramètres pour le périphérique sélectionné
DecklinkAudioSinkSettings audioSinkSettings = null;
if (deviceInfo != null)
{
    audioSinkSettings = new DecklinkAudioSinkSettings(deviceInfo);
    // Exemple : audioSinkSettings.DeviceNumber = deviceInfo.DeviceNumber; (déjà défini par le constructeur)
    // Configuration supplémentaire :
    // audioSinkSettings.BufferTime = TimeSpan.FromMilliseconds(100);
    // audioSinkSettings.IsSync = true;
}

// Créer le bloc avec les paramètres configurés
var decklinkAudioSink = new DecklinkAudioSinkBlock(audioSinkSettings);
```

### Paramètres clés du puits audio

La classe `DecklinkAudioSinkSettings` inclut des propriétés telles que :

- `DeviceNumber` : instance du périphérique de sortie à utiliser.
- `BufferTime` : latence minimale signalée par le puits (par défaut : 50 ms).
- `AlignmentThreshold` : seuil d'alignement des horodatages (par défaut : 40 ms).
- `DiscontWait` : temps d'attente avant de créer une discontinuité (par défaut : 1 s).
- `IsSync` : active la synchronisation (par défaut : true).

### Connexion au pipeline

Le bloc puits audio inclut un pad `Input` qui accepte les données audio d'autres blocs de votre pipeline :

```csharp
// Exemple : connecter une source audio/encodeur au puits audio Decklink
audioEncoder.Output.Connect(decklinkAudioSink.Input);
```

## Utilisation du bloc Decklink Audio Source

Le bloc Decklink Audio Source permet de capturer l'audio depuis les périphériques Blackmagic Decklink. Il prend en charge divers formats et configurations audio.

### Énumération des périphériques

Énumérer les périphériques sources audio disponibles :

```csharp
var devices = await DecklinkAudioSourceBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Source audio disponible : {item.Name}, Numéro de périphérique : {item.DeviceNumber}");
}
```

### Création et configuration du bloc

Créer et configurer le bloc source audio :

```csharp
// Obtenir le premier périphérique disponible
var deviceInfo = (await DecklinkAudioSourceBlock.GetDevicesAsync()).FirstOrDefault();

// Créer les paramètres pour le périphérique sélectionné
DecklinkAudioSourceSettings audioSourceSettings = null;
if (deviceInfo != null)
{
    // créer l'objet de paramètres
    audioSourceSettings = new DecklinkAudioSourceSettings(deviceInfo);
    // Configuration supplémentaire :
    // audioSourceSettings.Channels = DecklinkAudioChannels.Ch2;
    // audioSourceSettings.Connection = DecklinkAudioConnection.Embedded;
    // audioSourceSettings.Format = DecklinkAudioFormat.S16LE; // SampleRate est fixé à 48000
}

// Créer le bloc avec les paramètres configurés
var audioSource = new DecklinkAudioSourceBlock(audioSourceSettings);
```

### Paramètres clés de la source audio

La classe `DecklinkAudioSourceSettings` inclut des propriétés telles que :

- `DeviceNumber` : instance du périphérique d'entrée à utiliser.
- `Channels` : canaux audio à capturer (par ex. `DecklinkAudioChannels.Ch2`, `Ch8`, `Ch16`). Par défaut `Ch2`.
- `Format` : format d'échantillon audio (par ex. `DecklinkAudioFormat.S16LE`). Par défaut `S16LE`. La fréquence d'échantillonnage est fixée à 48000 Hz.
- `Connection` : type de connexion audio (par ex. `DecklinkAudioConnection.Embedded`, `AES`, `Analog`). Par défaut `Auto`.
- `BufferSize` : taille du tampon interne en images (par défaut : 5).
- `DisableAudioConversion` : définir à `true` pour désactiver la conversion audio interne. Par défaut `false`.

### Connexion au pipeline

Le bloc source audio fournit un pad `Output` qui peut se connecter à d'autres blocs :

```csharp
// Exemple : connecter la source audio à un encodeur ou un processeur audio
audioSource.Output.Connect(audioProcessor.Input);
```

## Utilisation du bloc Decklink Video Sink

Le bloc Decklink Video Sink permet la sortie vidéo vers les périphériques Blackmagic Decklink, prenant en charge divers formats vidéo et résolutions.

### Énumération des périphériques

Rechercher les périphériques puits vidéo disponibles :

```csharp
var devices = await DecklinkVideoSinkBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Périphérique de sortie vidéo disponible : {item.Name}, Numéro de périphérique : {item.DeviceNumber}");
}
```

### Création et configuration du bloc

Créer et configurer le bloc puits vidéo :

```csharp
// Obtenir le premier périphérique disponible
var deviceInfo = (await DecklinkVideoSinkBlock.GetDevicesAsync()).FirstOrDefault();

// Créer les paramètres pour le périphérique sélectionné
// Remarque : Mode est requis et doit être spécifié dans le constructeur
DecklinkVideoSinkSettings videoSinkSettings = null;
if (deviceInfo != null)
{
    // Mode est requis — spécifier la résolution vidéo de sortie et la fréquence d'images
    videoSinkSettings = new DecklinkVideoSinkSettings(deviceInfo, DecklinkMode.HD1080i60)
    {
        VideoFormat = DecklinkVideoFormat.YUV_10bit,
        // Facultatif : configuration supplémentaire
        // KeyerMode = DecklinkKeyerMode.Internal,
        // KeyerLevel = 128,
        // Profile = DecklinkProfileID.Default,
        // TimecodeFormat = DecklinkTimecodeFormat.RP188Any
    };
}

// Créer le bloc avec les paramètres configurés
var decklinkVideoSink = new DecklinkVideoSinkBlock(videoSinkSettings);
```

### Paramètres clés du puits vidéo

La classe `DecklinkVideoSinkSettings` inclut des propriétés telles que :

- `DeviceNumber` : instance du périphérique de sortie à utiliser (lecture seule, défini via le constructeur).
- `Mode` : spécifie la résolution vidéo et la fréquence d'images (par ex. `DecklinkMode.HD1080i60`, `HD720p60`). **Requis** — doit être spécifié dans le constructeur.
- `VideoFormat` : définit le format de pixel à l'aide de l'énumération `DecklinkVideoFormat` (par ex. `DecklinkVideoFormat.YUV_8bit`, `YUV_10bit`). Par défaut `YUV_8bit`.
- `KeyerMode` : contrôle les options de keying/composition via `DecklinkKeyerMode` (si pris en charge par le périphérique). Par défaut `Off`.
- `KeyerLevel` : définit le niveau du keyer (0-255). Par défaut `255`.
- `Profile` : spécifie le profil Decklink à utiliser via `DecklinkProfileID`.
- `TimecodeFormat` : spécifie le format de timecode pour la lecture via `DecklinkTimecodeFormat`. Par défaut `RP188Any`.
- `CustomVideoSize` : effet de redimensionnement facultatif à appliquer avant la sortie.
- `CustomFrameRate` : conversion de fréquence d'images facultative avant la sortie.
- `IsSync` : active la synchronisation (par défaut : true).

**Important** : le paramètre `Mode` est requis et détermine la fréquence d'images et la résolution de sortie. Si vous ne le spécifiez pas correctement, le matériel Decklink peut émettre à une fréquence d'images inattendue.

## Utilisation du bloc Decklink Video Source

Le bloc Decklink Video Source permet de capturer la vidéo depuis les périphériques Blackmagic Decklink, prenant en charge divers formats d'entrée et résolutions.

### Énumération des périphériques

Énumérer les périphériques de capture vidéo :

```csharp
var devices = await DecklinkVideoSourceBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Périphérique de capture vidéo disponible : {item.Name}, Numéro de périphérique : {item.DeviceNumber}");
}
```

### Création et configuration du bloc

Créer et configurer le bloc source vidéo :

```csharp
// Obtenir le premier périphérique disponible
var deviceInfo = (await DecklinkVideoSourceBlock.GetDevicesAsync()).FirstOrDefault();

// Créer les paramètres pour le périphérique sélectionné
DecklinkVideoSourceSettings videoSourceSettings = null;
if (deviceInfo != null)
{
    videoSourceSettings = new DecklinkVideoSourceSettings(deviceInfo);
    
    // Configurer le format et le mode d'entrée vidéo
    videoSourceSettings.Mode = DecklinkMode.HD1080i60;
    videoSourceSettings.Connection = DecklinkConnection.SDI; 
    // videoSourceSettings.VideoFormat = DecklinkVideoFormat.Auto; // Souvent utilisé avec Mode=Auto
}

// Créer le bloc avec les paramètres configurés
var videoSourceBlock = new DecklinkVideoSourceBlock(videoSourceSettings);
```

### Paramètres clés de la source vidéo

La classe `DecklinkVideoSourceSettings` inclut des propriétés telles que :

- `DeviceNumber` : instance du périphérique d'entrée à utiliser.
- `Mode` : spécifie la résolution et la fréquence d'images attendues en entrée (par ex. `DecklinkMode.HD1080i60`). Par défaut `Unknown`.
- `Connection` : définit l'entrée physique à utiliser via l'énumération `DecklinkConnection` (par ex. `DecklinkConnection.HDMI`, `DecklinkConnection.SDI`). Par défaut `Auto`.
- `VideoFormat` : spécifie le type de format vidéo en entrée via l'énumération `DecklinkVideoFormat`. Par défaut `Auto` (surtout lorsque `Mode` est `Auto`).
- `Profile` : spécifie le profil Decklink via `DecklinkProfileID`. Par défaut `Default`.
- `DropNoSignalFrames` : si `true`, abandonne les images marquées comme étant sans signal d'entrée. Par défaut `false`.
- `OutputAFDBar` : si `true`, extrait et émet les données AFD/Bar en tant que Meta. Par défaut `false`.
- `OutputCC` : si `true`, extrait et émet les sous-titres codés en tant que Meta. Par défaut `false`.
- `TimecodeFormat` : spécifie le format de timecode via `DecklinkTimecodeFormat`. Par défaut `RP188Any`.
- `DisableVideoConversion` : définir à `true` pour désactiver la conversion vidéo interne. Par défaut `false`.

## Utilisation du bloc Decklink Video + Audio Source

Le `DecklinkVideoAudioSourceBlock` simplifie la capture synchronisée de flux vidéo et audio depuis un seul périphérique Decklink.

### Énumération et configuration des périphériques

La sélection des périphériques se fait via `DecklinkVideoSourceSettings` et `DecklinkAudioSourceSettings`. Vous énumérerez généralement les périphériques vidéo via `DecklinkVideoSourceBlock.GetDevicesAsync()` et les périphériques audio via `DecklinkAudioSourceBlock.GetDevicesAsync()`, puis configurerez les objets de paramètres correspondants pour le périphérique choisi. Le `DecklinkVideoAudioSourceBlock` lui-même fournit également `GetDevicesAsync()` qui énumère les sources vidéo.

```csharp
// Énumérer les périphériques vidéo (pour la partie vidéo de la source combinée)
var videoDeviceInfo = (await DecklinkVideoAudioSourceBlock.GetDevicesAsync()).FirstOrDefault(); // ou DecklinkVideoSourceBlock.GetDevicesAsync()
var audioDeviceInfo = (await DecklinkAudioSourceBlock.GetDevicesAsync()).FirstOrDefault(d => d.DeviceNumber == videoDeviceInfo.DeviceNumber); // Exemple : appariement par numéro de périphérique

DecklinkVideoSourceSettings videoSettings = null;
if (videoDeviceInfo != null)
{
    videoSettings = new DecklinkVideoSourceSettings(videoDeviceInfo);
    videoSettings.Mode = DecklinkMode.HD1080i60;
    videoSettings.Connection = DecklinkConnection.SDI;
}

DecklinkAudioSourceSettings audioSettings = null;
if (audioDeviceInfo != null)
{
    audioSettings = new DecklinkAudioSourceSettings(audioDeviceInfo);
    audioSettings.Channels = DecklinkAudioChannels.Ch2;
}

// Créer le bloc avec les paramètres configurés
if (videoSettings != null && audioSettings != null)
{
    var decklinkVideoAudioSource = new DecklinkVideoAudioSourceBlock(videoSettings, audioSettings);

    // Connecter les sorties
    // decklinkVideoAudioSource.VideoOutput.Connect(videoProcessor.Input);
    // decklinkVideoAudioSource.AudioOutput.Connect(audioProcessor.Input);
}
```

### Création et configuration du bloc

Vous instanciez `DecklinkVideoAudioSourceBlock` en fournissant des objets `DecklinkVideoSourceSettings` et `DecklinkAudioSourceSettings` préconfigurés.

```csharp
// En supposant que videoSourceSettings et audioSourceSettings sont configurés comme ci-dessus
var videoAudioSource = new DecklinkVideoAudioSourceBlock(videoSourceSettings, audioSourceSettings);
```

### Connexion au pipeline

Le bloc fournit des pads `VideoOutput` et `AudioOutput` distincts :

```csharp
// Exemple : connecter aux processeurs/encodeurs vidéo et audio
videoAudioSource.VideoOutput.Connect(videoEncoder.Input);
videoAudioSource.AudioOutput.Connect(audioEncoder.Input);
```

## Utilisation du bloc Decklink Video + Audio Sink

Le `DecklinkVideoAudioSinkBlock` simplifie l'envoi de flux vidéo et audio synchronisés vers un seul périphérique Decklink.

### Énumération et configuration des périphériques

À l'instar de la source combinée, la sélection des périphériques se fait via `DecklinkVideoSinkSettings` et `DecklinkAudioSinkSettings`. Énumérez les périphériques via `DecklinkVideoSinkBlock.GetDevicesAsync()` et `DecklinkAudioSinkBlock.GetDevicesAsync()`.

```csharp
var videoSinkDeviceInfo = (await DecklinkVideoSinkBlock.GetDevicesAsync()).FirstOrDefault();
var audioSinkDeviceInfo = (await DecklinkAudioSinkBlock.GetDevicesAsync()).FirstOrDefault(d => d.DeviceNumber == videoSinkDeviceInfo.DeviceNumber); // Exemple d'appariement

DecklinkVideoSinkSettings videoSinkSettings = null;
if (videoSinkDeviceInfo != null)
{
    // Mode est requis — spécifier la résolution vidéo de sortie et la fréquence d'images
    videoSinkSettings = new DecklinkVideoSinkSettings(videoSinkDeviceInfo, DecklinkMode.HD1080i60)
    {
        VideoFormat = DecklinkVideoFormat.YUV_8bit
    };
}

DecklinkAudioSinkSettings audioSinkSettings = null;
if (audioSinkDeviceInfo != null)
{
    audioSinkSettings = new DecklinkAudioSinkSettings(audioSinkDeviceInfo);
}

// Créer le bloc
if (videoSinkSettings != null && audioSinkSettings != null)
{
    var decklinkVideoAudioSink = new DecklinkVideoAudioSinkBlock(videoSinkSettings, audioSinkSettings);
    
    // Connecter les entrées
    // videoEncoder.Output.Connect(decklinkVideoAudioSink.VideoInput);
    // audioEncoder.Output.Connect(decklinkVideoAudioSink.AudioInput);
}
```

### Création et configuration du bloc

Instanciez `DecklinkVideoAudioSinkBlock` avec des `DecklinkVideoSinkSettings` et `DecklinkAudioSinkSettings` configurés.

```csharp
// En supposant que videoSinkSettings et audioSinkSettings sont configurés
var videoAudioSink = new DecklinkVideoAudioSinkBlock(videoSinkSettings, audioSinkSettings);
```

### Connexion au pipeline

Le bloc fournit des pads `VideoInput` et `AudioInput` distincts :

```csharp
// Exemple : connecter depuis les encodeurs vidéo et audio
videoEncoder.Output.Connect(videoAudioSink.VideoInput);
audioEncoder.Output.Connect(videoAudioSink.AudioInput);
```

## Exemples d'utilisation avancée

### Capture audio/vidéo synchronisée

**Avec des blocs source séparés :**

```csharp
// On suppose que videoSourceSettings et audioSourceSettings sont configurés pour le même périphérique/timing
var videoSource = new DecklinkVideoSourceBlock(videoSourceSettings);
var audioSource = new DecklinkAudioSourceBlock(audioSourceSettings);

// Créer un encodeur MP4
var mp4Settings = new MP4SinkSettings("output.mp4");
var sink = new MP4SinkBlock(mp4Settings);

// Créer l'encodeur vidéo
var videoEncoder = new H264EncoderBlock();

// Créer l'encodeur audio
var audioEncoder = new AACEncoderBlock();

// Connecter les sources vidéo et audio
pipeline.Connect(videoSource.Output, videoEncoder.Input);
pipeline.Connect(audioSource.Output, audioEncoder.Input);

// Connecter l'encodeur vidéo au puits
pipeline.Connect(videoEncoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connecter l'encodeur audio au puits
pipeline.Connect(audioEncoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Démarrer le pipeline
await pipeline.StartAsync();
```

**Avec `DecklinkVideoAudioSourceBlock` pour une capture synchronisée simplifiée :**
Si vous utilisez `DecklinkVideoAudioSourceBlock` (tel que configuré dans sa section dédiée), la configuration de la source devient :

```csharp
// On suppose que videoSourceSettings et audioSourceSettings sont configurés pour le même périphérique
var videoAudioSource = new DecklinkVideoAudioSourceBlock(videoSourceSettings, audioSourceSettings);

// ... (configuration des encodeurs et du puits comme ci-dessus) ...

// Connecter vidéo et audio depuis la source combinée
pipeline.Connect(videoAudioSource.VideoOutput, videoEncoder.Input);
pipeline.Connect(videoAudioSource.AudioOutput, audioEncoder.Input);

// ... (connecter les encodeurs au puits et démarrer le pipeline comme ci-dessus) ...
```

Cela garantit que l'audio et la vidéo sont issus du périphérique Decklink de manière synchronisée par le SDK.

## Conseils de dépannage

- **Aucun périphérique trouvé** : assurez-vous que les pilotes/SDK Blackmagic sont installés et à jour. Vérifiez que le périphérique est reconnu par Blackmagic Desktop Video Setup.
- **Incompatibilité de format** : vérifiez que le périphérique prend en charge le mode vidéo/audio, le format et le type de connexion sélectionnés. Pour les sources avec `Mode = DecklinkMode.Unknown` (détection automatique), assurez-vous qu'un signal stable est présent.
- **Problèmes de performance** : vérifiez les ressources système (CPU, RAM, E/S disque). Envisagez de réduire la résolution/fréquence d'images si les problèmes persistent.
- **Détection du signal** : pour les périphériques d'entrée, vérifiez les connexions des câbles et assurez-vous que le périphérique source émet un signal valide.
- **Erreurs « Unable to build ...Block »** : vérifiez que tous les paramètres sont valides pour le périphérique et le mode sélectionnés. Assurez-vous que le bon `DeviceNumber` est utilisé si plusieurs cartes Decklink sont présentes.

## Exemples d'applications

Pour des exemples complets et fonctionnels, consultez ces applications :

- [Démo Decklink](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Decklink%20Demo)

## Conclusion

Les blocs Blackmagic Decklink du VisioForge Media Blocks SDK offrent un moyen puissant et flexible d'intégrer du matériel vidéo et audio professionnel à vos applications .NET. En exploitant les blocs source et puits spécifiques, y compris les blocs audio/vidéo combinés, vous pouvez implémenter efficacement des flux de capture et de lecture complexes. Référez-vous toujours aux classes de paramètres spécifiques pour les options de configuration détaillées.
