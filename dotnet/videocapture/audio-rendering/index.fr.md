---
title: Périphérique audio et contrôle du volume en C# .NET
description: Configurez les périphériques de sortie audio, ajustez le volume et optimisez la lecture dans le VisioForge Video Capture SDK. Exemples C# pour WinForms et WPF.
sidebar_label: Rendu audio
order: 12
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
primary_api_classes:
  - AudioRendererSettings

---

# Rendu audio dans les applications vidéo .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introduction au rendu audio

Le rendu audio est un composant essentiel de toute application de capture vidéo. Il permet à votre application de produire l'audio capturé ou traité vers divers périphériques audio pris en charge par le système d'exploitation. Le Video Capture SDK .NET offre des capacités robustes pour le rendu audio, permettant aux développeurs de créer des applications multimédias riches avec une sortie audio de haute qualité.

Ce guide parcourt les aspects essentiels de l'implémentation du rendu audio dans vos applications .NET avec notre SDK, couvrant tout, de l'énumération des périphériques au contrôle du volume et aux techniques d'optimisation.

## Fonctionnalités clés du rendu audio

- **Sélection de périphérique** : énumérez et sélectionnez parmi tous les périphériques de sortie audio disponibles
- **Contrôle du volume** : contrôle précis des niveaux de volume de sortie
- **Ajustement en temps réel** : modifiez les paramètres de sortie audio pendant l'exécution
- **Prise en charge multi-périphériques** : routez l'audio vers différents périphériques de sortie simultanément
- **Compatibilité des formats** : prise en charge de divers formats audio et fréquences d'échantillonnage

## Guide d'implémentation

### Énumération des périphériques de sortie audio

La première étape de l'implémentation du rendu audio consiste à identifier et lister tous les périphériques de sortie audio disponibles. Cela permet aux utilisateurs de sélectionner leur périphérique de sortie préféré pour la lecture audio.

=== "VideoCaptureCoreX"

    
    ```csharp
    var audioSinks = await VideoCapture1.Audio_OutputsAsync();
    foreach (var sink in audioSinks)
    {
        // ajouter à une ComboBox
        cbAudioOutputDevice.Items.Add(sink.DisplayName);
    }
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    // Audio_OutputDevices() retourne ObservableCollection<string> — l'élément EST le nom du périphérique.
    foreach (var deviceName in VideoCapture1.Audio_OutputDevices())
    {
        // ajouter à une ComboBox
        cbAudioOutputDevice.Items.Add(deviceName);
    }
    ```
    


Le code ci-dessus montre comment récupérer tous les périphériques de sortie audio disponibles et remplir un contrôle de sélection tel qu'une ComboBox. Cela donne aux utilisateurs la flexibilité de choisir leur périphérique de sortie audio préféré.

### Définition du périphérique de sortie audio

Une fois que l'utilisateur a sélectionné un périphérique de sortie audio, vous devez configurer le SDK pour utiliser ce périphérique pour la lecture audio.

=== "VideoCaptureCoreX"

    
    ```csharp
    // VideoCaptureCoreX utilise Audio_OutputsAsync() pour énumérer les objets AudioOutputDeviceInfo.
    var audioOutputDevice = (await VideoCapture1.Audio_OutputsAsync())
        .First(device => device.DisplayName == cbAudioOutputDevice.Text);
    VideoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    VideoCapture1.Audio_PlayAudio = true;
    VideoCapture1.Audio_OutputDevice = "Device name";
    ```
    


Dans VideoCaptureCoreX, nous récupérons d'abord l'objet périphérique sélectionné, puis créons une instance AudioRendererSettings pour configurer la sortie. Dans VideoCaptureCore, le processus est plus simple, ne nécessitant que la chaîne du nom du périphérique et l'activation de la lecture audio.

### Contrôle du volume audio

Le contrôle du volume est une fonctionnalité essentielle pour toute application audio. Le SDK fournit des méthodes simples pour ajuster le volume de sortie pendant la lecture.

=== "VideoCaptureCoreX"

    
    ```csharp
    VideoCapture1.Audio_OutputDevice_Volume = 0.75; // 75%
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    VideoCapture1.Audio_OutputDevice_Volume_Set(75); // 75%
    ```
    


Les deux implémentations permettent de définir le volume sous forme de pourcentage (0 à 100 %). Dans VideoCaptureCoreX, le volume est défini sous forme de valeur à virgule flottante entre 0 et 1, tandis que VideoCaptureCore utilise un pourcentage entier.

## Dépannage des problèmes courants

### Aucune sortie audio

Si vous rencontrez des problèmes avec la sortie audio :

1. **Vérifiez la disponibilité du périphérique** : assurez-vous que le périphérique audio sélectionné est connecté et fonctionnel
2. **Vérifiez les paramètres de volume** : confirmez que le volume est réglé à un niveau audible
3. **Examinez la compatibilité des formats** : certains périphériques peuvent ne pas prendre en charge certains formats audio

### Problèmes de latence audio

Une latence audio élevée peut nuire à l'expérience utilisateur :

1. **Réduisez la taille du tampon** : des tailles de tampon plus petites peuvent réduire la latence mais peuvent augmenter l'utilisation du processeur
2. **Optimisez le pipeline de traitement** : supprimez les étapes de traitement audio inutiles
3. **Vérifiez les capacités matérielles** : certains périphériques audio ont intrinsèquement une latence plus élevée

### Problèmes de qualité audio

Pour une qualité audio optimale :

1. **Utilisez des fréquences d'échantillonnage appropriées** : faites correspondre la fréquence d'échantillonnage à votre matériel source
2. **Considérez la profondeur de bits** : des profondeurs de bits plus élevées fournissent une meilleure qualité mais consomment plus de ressources
3. **Surveillez l'utilisation du processeur** : des coupures audio peuvent se produire lorsque le système est surchargé

## Conclusion

Le rendu audio est un composant vital des applications multimédias. Le Video Capture SDK .NET offre des outils puissants pour implémenter une lecture audio de haute qualité dans vos applications. En suivant les directives et exemples de ce document, vous pouvez créer des solutions de rendu audio sophistiquées qui améliorent l'expérience de vos utilisateurs.

L'architecture flexible du SDK accommode aussi bien les scénarios de lecture audio simples que les configurations multi-périphériques complexes, le rendant adapté à un large éventail d'applications, des lecteurs vidéo basiques aux outils de production multimédia professionnels.

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
