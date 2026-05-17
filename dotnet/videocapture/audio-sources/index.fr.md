---
title: Sources audio en C# .NET — micro, loopback et audio IP
description: Configurez les sources de capture audio en C# .NET — microphone, loopback système, audio caméra IP et Decklink avec des exemples de code.
sidebar_label: Sources audio
order: 15
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - IVideoCaptureBaseAudioSourceSettings
  - AudioCaptureSource
  - DeviceEnumerator
  - LoopbackAudioCaptureDeviceSourceSettings
  - AudioCaptureDeviceFormat

---

# Travailler avec les sources audio en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Sources audio disponibles

Lors de la création d'applications multimédias, vous devrez capturer l'audio depuis diverses sources. Ce guide couvre la manière d'implémenter la capture audio depuis plusieurs types d'entrées avec notre SDK :

* Périphériques de capture audio (microphones, entrée ligne)
* Audio système (haut-parleurs/écouteurs via loopback)
* Flux réseau (caméras IP)
* Périphériques Decklink professionnels

Chaque type de source nécessite des méthodes d'initialisation différentes et possède des capacités uniques. Explorons comment travailler avec chacune.

## Implémenter les périphériques de capture audio

Les périphériques de capture audio incluent les microphones, les webcams avec micros intégrés et autres matériels d'entrée connectés à votre système. Travailler avec ces périphériques implique trois étapes clés :

1. Énumérer les périphériques disponibles
2. Sélectionner les formats audio appropriés
3. Configurer le périphérique sélectionné comme source audio

### Énumération des périphériques audio disponibles

Tout d'abord, vous devez détecter tous les périphériques d'entrée audio connectés au système :

=== "VideoCaptureCoreX"

    
    ```csharp
    var audioSources = await core.Audio_SourcesAsync();
    foreach (var source in audioSources)
    {
        // ajouter à une ComboBox
        cbAudioInputDevice.Items.Add(source.DisplayName);
    }
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    foreach (var device in core.Audio_CaptureDevices())
    {
        // ajouter à une ComboBox
        cbAudioInputDevice.Items.Add(device.Name);
    }
    ```
    


Ce code récupère tous les périphériques d'entrée audio et peut les afficher dans une liste déroulante pour la sélection par l'utilisateur. L'approche asynchrone dans VideoCaptureCoreX offre de meilleures performances pour les systèmes avec de nombreux périphériques connectés.

### Découverte des formats audio pris en charge

Une fois que vous avez identifié les périphériques disponibles, vous devrez déterminer quels formats audio chaque périphérique prend en charge :

=== "VideoCaptureCoreX"

    
    ```csharp
    // trouver le périphérique par son nom
    var deviceItem = (await VideoCapture1.Audio_SourcesAsync()).FirstOrDefault(device => device.DisplayName == "Some device name");
    if (deviceItem == null)
    {
        return;
    }
    
    // énumérer les formats
    foreach (var format in deviceItem.Formats)
    {
        cbAudioInputFormat.Items.Add(format.Name);
    }
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    // trouver le périphérique par son nom
    var deviceItem = VideoCapture1.Audio_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // énumérer les formats
    foreach (var format in deviceItem.Formats)
    {
        cbAudioInputFormat.Items.Add(format);
    }
    ```
    


Différents périphériques audio prennent en charge divers formats avec différentes profondeurs de bits, fréquences d'échantillonnage et configurations de canaux. L'énumération de ces options vous permet de sélectionner le format le plus approprié pour les besoins de votre application.

### Configuration du périphérique de capture audio

Après avoir sélectionné un périphérique et un format, configurez-le comme source audio :

=== "VideoCaptureCoreX"

    
    ```csharp
    // Énumérer les périphériques de capture audio de manière asynchrone (Audio_SourcesAsync retourne AudioCaptureDeviceInfo[]).
    var devices = await VideoCapture1.Audio_SourcesAsync();
    var deviceItem = devices.FirstOrDefault(d => d.Name == "Device name");
    if (deviceItem == null)
    {
        return;
    }

    // Choisir le premier format signalé sur ce périphérique.
    AudioCaptureDeviceFormat format = deviceItem.Formats[0].ToFormat();

    // Construire les paramètres de source et les assigner.
    IVideoCaptureBaseAudioSourceSettings audioSource = deviceItem.CreateSourceSettingsVC(format);
    VideoCapture1.Audio_Source = audioSource;
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    // trouver le périphérique par son nom
    var deviceItem = VideoCapture1.Audio_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource(deviceItem.Name);
    VideoCapture1.Audio_CaptureDevice.Format = deviceItem.Formats[0].ToString(); // définir le premier format
    ```
    


Ce code configure votre application pour capturer l'audio depuis le périphérique sélectionné en utilisant le format spécifié. L'API VideoCaptureCoreX offre un contrôle plus granulaire sur la sélection du format et la configuration du périphérique.

## Capturer l'audio système via loopback

Le loopback audio vous permet d'enregistrer tout son lu par les haut-parleurs ou les écouteurs de votre système. Cela est particulièrement utile pour :

* L'enregistrement d'écran avec audio
* La capture des sons d'application
* L'enregistrement audio de conférences web ou de services de streaming

Voici comment l'implémenter :

=== "VideoCaptureCoreX"

    
    Énumérez d'abord les périphériques loopback disponibles :
    
    ```csharp
    // Énumérer les périphériques loopback audio
    var audioSinks = await DeviceEnumerator.Shared.AudioOutputsAsync();
    foreach (var sink in audioSinks)
    {   
        // Filtrer par API WASAPI2
        if (sink.API == AudioOutputDeviceAPI.WASAPI2)
        {
            // Ajouter à une ComboBox
            cbAudioLoopbackDevice.Items.Add(sink.Name);
        }
    }
    ```
    
    Créez ensuite les paramètres de source pour votre périphérique de sortie sélectionné :
    
    ```csharp
    // entrée audio
    var deviceItem = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)).FirstOrDefault(device => device.Name == "Output device name");
    if (deviceItem == null)
    {
        return;
    }
    
    IVideoCaptureBaseAudioSourceSettings audioSource = new LoopbackAudioCaptureDeviceSourceSettings(deviceItem);
    
    VideoCapture1.Audio_Source = audioSource;
    ```
    
    L'API WASAPI2 fournit la fonctionnalité loopback la plus fiable sur les systèmes Windows, avec une latence plus faible et de meilleures performances par rapport aux autres options.
    

=== "VideoCaptureCore"

    
    Dans VideoCaptureCore, la fonctionnalité loopback est simplifiée avec un périphérique virtuel dédié :
    
    ```cs
    VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource("VisioForge What You Hear Source");
    VideoCapture1.Audio_CaptureDevice.Format_UseBest = true;
    ```
    
    Cette approche sélectionne automatiquement le meilleur format disponible pour la source loopback, rendant l'implémentation simple.
    


Pour des exemples complets et exécutables de capture haut-parleur (y compris l'approche pipeline Media Blocks SDK), consultez le [guide Capture audio](../audio-capture/index.md#capture-system-audio-speaker-loopback).

## Travailler avec les sources audio réseau

Pour les caméras IP et autres flux réseau, l'audio voyage sur le même transport que la vidéo — vous ne construisez généralement pas de source audio séparée. Créez les paramètres de source IP avec `audioEnabled: true` et le SDK démultiplexe l'audio et la vidéo depuis la même URL :

```csharp
// Caméra RTSP — l'audio arrive automatiquement quand audioEnabled est à true.
var rtsp = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:554/Streaming/Channels/101"),
    login: "admin",
    password: "password",
    audioEnabled: true);

VideoCapture1.Video_Source = rtsp;
VideoCapture1.Audio_Record = true;   // inclure l'audio RTSP dans la sortie fichier
```

L'audio provenant de sources réseau peut arriver dans divers formats (AAC, MP3, PCM) selon le périphérique. Le SDK convertit et synchronise automatiquement.

## Implémenter la capture audio Decklink

Les périphériques Decklink fournissent un audio de qualité professionnelle (jusqu'à 192 kHz, multicanal, intégré au SDI). Utilisez `DecklinkAudioSourceSettings` et connectez-le aux côtés de la source vidéo Decklink :

```csharp
// Énumérer les entrées audio Decklink.
var devices = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();
var dl = devices.First();

// Attacher la source audio à VideoCaptureCoreX. La source vidéo correspondante va sur Video_Source.
VideoCapture1.Audio_Source = new DecklinkAudioSourceSettings(dl);
VideoCapture1.Audio_Record = true;
```

Les paramètres audio sont largement déterminés par le mode courant du périphérique (fréquence d'échantillonnage, nombre de canaux sont fixés par le signal SDI entrant) — vous ne les surchargez généralement pas sur la classe de paramètres.

## Bonnes pratiques pour la capture audio

Pour garantir une capture audio de haute qualité dans vos applications :

1. **Sélection de la fréquence d'échantillonnage** : choisissez des fréquences d'échantillonnage appropriées en fonction de votre sortie cible. Pour la plupart des applications, 44,1 kHz ou 48 kHz est suffisant.

2. **Gestion du tampon** : configurez des tailles de tampon appropriées pour équilibrer la latence et la stabilité. Des tampons plus petits réduisent la latence mais peuvent provoquer des coupures audio.

3. **Gestion des formats** : prenez en charge plusieurs formats pour accommoder divers périphériques. Ayez toujours des options de repli quand certains formats spécifiques ne sont pas disponibles.

4. **Surveillance des niveaux** : implémentez la surveillance des niveaux audio pour détecter le silence ou la saturation, permettant à votre application de réagir de manière appropriée.

5. **Gestion des erreurs** : implémentez une gestion robuste des erreurs pour les déconnexions de périphériques ou les échecs de négociation de format.

## Conclusion

L'implémentation de capacités de capture audio dans votre application .NET implique de sélectionner la source appropriée, de configurer les formats et de gérer le flux audio. Que vous capturiez depuis des microphones, l'audio système ou des sources réseau, notre SDK fournit les outils nécessaires pour créer des applications audio sophistiquées.

En suivant les exemples de code et les modèles d'implémentation décrits dans ce guide, vous pourrez intégrer efficacement des fonctionnalités puissantes de capture audio dans vos projets.

## Exemples d'applications

Des exemples complets et fonctionnels sont disponibles sur GitHub :

* [Tous les exemples Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)
