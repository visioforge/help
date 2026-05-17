---
title: Énumération des périphériques multimédias en C# .NET
description: Listez caméras, micros, haut-parleurs, Decklink, NDI et périphériques GenICam avec VisioForge Media Blocks SDK et des exemples multiplateformes.
tags:
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Decklink
  - NDI Source
  - USB3 Vision / GigE
  - ONVIF
  - NDI
  - C#
primary_api_classes:
  - DeviceEnumerator
  - VideoCaptureDeviceInfo
  - AudioCaptureDeviceAPI

---

# Guide complet de l'énumération des périphériques multimédias en .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le Media Blocks SDK offre une manière puissante et efficace de découvrir et travailler avec divers périphériques multimédias dans vos applications .NET. Ce guide vous accompagne dans le processus d'énumération de différents types de périphériques multimédias à l'aide de la classe `DeviceEnumerator` du SDK.

## Introduction à l'énumération des périphériques

L'énumération des périphériques est une première étape critique pour développer des applications qui interagissent avec du matériel multimédia. La classe `DeviceEnumerator` fournit un moyen centralisé pour détecter et lister tous les périphériques multimédias disponibles connectés à votre système.

Le SDK utilise un patron singleton pour l'énumération des périphériques, ce qui facilite l'accès à cette fonctionnalité depuis n'importe où dans votre code :

```csharp
// Accéder à l'instance partagée de DeviceEnumerator
var enumerator = DeviceEnumerator.Shared;
```

## Découverte des périphériques d'entrée vidéo

### Sources vidéo standard

Pour lister tous les périphériques d'entrée vidéo disponibles (webcams, cartes de capture, caméras virtuelles) :

```csharp
var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

foreach (var device in videoSources)
{
    Debug.WriteLine($"Périphérique vidéo trouvé : {device.Name}");
    // Vous pouvez accéder à des propriétés supplémentaires ici si nécessaire
}
```

Les objets `VideoCaptureDeviceInfo` renvoyés fournissent des informations détaillées sur chaque périphérique, notamment le nom du périphérique, des identifiants internes et le type d'API.

## Utilisation des périphériques audio

### Énumération des sources d'entrée audio

Pour découvrir les microphones et autres périphériques d'entrée audio :

```csharp
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();

foreach (var device in audioSources)
{
    Debug.WriteLine($"Périphérique d'entrée audio trouvé : {device.Name}");
    // Des informations supplémentaires sur le périphérique sont accessibles ici
}
```

Vous pouvez également filtrer les périphériques audio par type d'API :

```csharp
// Obtenir uniquement les sources audio d'une API spécifique
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound);
```

### Recherche des périphériques de sortie audio

Pour les haut-parleurs, casques et autres destinations de sortie audio :

```csharp
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();

foreach (var device in audioOutputs)
{
    Debug.WriteLine($"Périphérique de sortie audio trouvé : {device.Name}");
    // Traiter les informations du périphérique selon les besoins
}
```

À l'instar des sources audio, vous pouvez filtrer les sorties par API :

```csharp
// Obtenir uniquement les sorties audio d'une API spécifique
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound);
```

## Intégration professionnelle Blackmagic Decklink

### Sources d'entrée vidéo Decklink

Pour les flux vidéo professionnels utilisant du matériel Blackmagic :

```csharp
var decklinkVideoSources = await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync();

foreach (var device in decklinkVideoSources)
{
    Debug.WriteLine($"Entrée vidéo Decklink : {device.Name}");
    // Vous pouvez travailler avec les propriétés spécifiques de Decklink ici
}
```

### Sources d'entrée audio Decklink

Pour accéder aux canaux audio des périphériques Decklink :

```csharp
var decklinkAudioSources = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();

foreach (var device in decklinkAudioSources)
{
    Debug.WriteLine($"Entrée audio Decklink : {device.Name}");
    // Traiter les informations du périphérique audio Decklink
}
```

### Destinations de sortie vidéo Decklink

Pour envoyer la vidéo vers des périphériques de sortie Decklink :

```csharp
var decklinkVideoOutputs = await DeviceEnumerator.Shared.DecklinkVideoSinksAsync();

foreach (var device in decklinkVideoOutputs)
{
    Debug.WriteLine($"Sortie vidéo Decklink : {device.Name}");
    // Accéder aux propriétés du périphérique de sortie selon les besoins
}
```

### Destinations de sortie audio Decklink

Pour router l'audio vers les sorties matérielles Decklink :

```csharp
var decklinkAudioOutputs = await DeviceEnumerator.Shared.DecklinkAudioSinksAsync();

foreach (var device in decklinkAudioOutputs)
{
    Debug.WriteLine($"Sortie audio Decklink : {device.Name}");
    // Travailler avec la configuration de sortie audio ici
}
```

## Intégration des périphériques réseau

### Découverte des sources NDI

Pour trouver les sources NDI disponibles sur votre réseau :

```csharp
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();

foreach (var device in ndiSources)
{
    Debug.WriteLine($"Source NDI découverte : {device.Name}");
    // Traiter les propriétés et informations spécifiques à NDI
}
```

### Découverte des caméras réseau ONVIF

Pour trouver les caméras IP prenant en charge le protocole ONVIF :

```csharp
// Définir un délai d'attente pour la découverte (2 secondes dans cet exemple)
var timeout = TimeSpan.FromSeconds(2);
var onvifDevices = await DeviceEnumerator.Shared.ONVIF_ListSourcesAsync(timeout, null);

foreach (var deviceUri in onvifDevices)
{
    Debug.WriteLine($"Caméra ONVIF trouvée à : {deviceUri}");
    // Se connecter à la caméra à l'aide de l'URI découverte
}
```

## Prise en charge des caméras industrielles

### Caméras industrielles Basler

Pour les applications nécessitant des caméras industrielles Basler :

```csharp
var baslerCameras = await DeviceEnumerator.Shared.BaslerSourcesAsync();

foreach (var device in baslerCameras)
{
    Debug.WriteLine($"Caméra Basler détectée : {device.Name}");
    // Accéder aux fonctionnalités spécifiques aux caméras Basler
}
```

### Caméras industrielles Allied Vision

Pour travailler avec des caméras Allied Vision dans votre application :

```csharp
var alliedCameras = await DeviceEnumerator.Shared.AlliedVisionSourcesAsync();

foreach (var device in alliedCameras)
{
    Debug.WriteLine($"Caméra Allied Vision trouvée : {device.Name}");
    // Configurer les paramètres spécifiques Allied Vision
}
```

### Caméras compatibles avec le SDK Spinnaker

Pour les caméras prenant en charge le SDK Spinnaker (Windows uniquement) :

```csharp
#if NET_WINDOWS
var spinnakerCameras = await DeviceEnumerator.Shared.SpinnakerSourcesAsync();

foreach (var device in spinnakerCameras)
{
    Debug.WriteLine($"Caméra SDK Spinnaker : {device.Name}");
    Debug.WriteLine($"Modèle : {device.Model}, Fabricant : {device.Vendor}");
    Debug.WriteLine($"Résolution : {device.WidthMax}x{device.HeightMax}");
    // Travailler avec les propriétés spécifiques de la caméra
}
#endif
```

### Caméras génériques au standard GenICam

Pour d'autres caméras industrielles prenant en charge le standard GenICam :

```csharp
var genicamCameras = await DeviceEnumerator.Shared.GenICamSourcesAsync();

foreach (var device in genicamCameras)
{
    Debug.WriteLine($"Périphérique compatible GenICam : {device.Name}");
    Debug.WriteLine($"Modèle : {device.Model}, Fabricant : {device.Vendor}");
    Debug.WriteLine($"Protocole : {device.Protocol}, Série : {device.SerialNumber}");
    // Travailler avec les fonctionnalités GenICam standard
}
```

## Surveillance des périphériques

Le SDK prend également en charge la surveillance des connexions et déconnexions de périphériques :

```csharp
// Démarrer la surveillance des changements de périphériques vidéo
await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();

// Démarrer la surveillance des changements de périphériques audio
await DeviceEnumerator.Shared.StartAudioSourceMonitorAsync();
await DeviceEnumerator.Shared.StartAudioSinkMonitorAsync();

// S'abonner aux événements de changement de périphériques
DeviceEnumerator.Shared.OnVideoSourceAdded += (sender, device) => 
{
    Debug.WriteLine($"Nouveau périphérique vidéo connecté : {device.Name}");
};

DeviceEnumerator.Shared.OnVideoSourceRemoved += (sender, device) => 
{
    Debug.WriteLine($"Périphérique vidéo déconnecté : {device.Name}");
};
```

## Considérations spécifiques aux plateformes

### Windows

Sous Windows, le SDK peut détecter les événements de connexion et de retrait de périphériques USB au niveau système :

```csharp
#if NET_WINDOWS
// S'abonner aux événements de périphériques à l'échelle du système
DeviceEnumerator.Shared.OnDeviceAdded += (sender, args) => 
{
    // Rafraîchir les listes de périphériques lorsque du nouveau matériel est connecté
    RefreshDeviceLists();
};

DeviceEnumerator.Shared.OnDeviceRemoved += (sender, args) => 
{
    // Mettre à jour l'UI quand du matériel est déconnecté
    RefreshDeviceLists();
};
#endif
```

Par défaut, l'énumération des périphériques Media Foundation est désactivée pour éviter les doublons avec les périphériques DirectShow. Vous pouvez l'activer si nécessaire :

```csharp
#if NET_WINDOWS
// Activer l'énumération des périphériques Media Foundation si nécessaire
DeviceEnumerator.Shared.IsEnumerateMediaFoundationDevices = true;
#endif
```

### iOS et Android

Sur les plateformes mobiles, le SDK gère les demandes de permission requises lors de l'énumération des périphériques :

```csharp
#if __IOS__ || __ANDROID__
// Cela demandera automatiquement les permissions de caméra si nécessaire
var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

// Cela demandera automatiquement les permissions de microphone si nécessaire
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();
#endif
```

## Bonnes pratiques pour l'énumération des périphériques

Lorsque vous travaillez avec l'énumération de périphériques dans des applications de production :

1. Gérez toujours les cas où aucun périphérique n'est trouvé
2. Envisagez de mettre en cache les listes de périphériques lorsque cela est approprié pour améliorer les performances
3. Mettez en place une gestion d'exceptions appropriée pour les échecs d'accès aux périphériques
4. Fournissez un retour utilisateur clair lorsque les périphériques requis sont absents
5. Utilisez les méthodes async pour éviter de bloquer le thread UI pendant l'énumération
6. Nettoyez les ressources en appelant `Dispose()` lorsque vous avez terminé avec DeviceEnumerator

```csharp
// Nettoyage approprié à la fin
DeviceEnumerator.Shared.Dispose();
```
