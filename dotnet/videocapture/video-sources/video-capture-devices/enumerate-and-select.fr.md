---
title: Énumérer les périphériques de capture vidéo en .NET avec C#
description: Détectez, énumérez et configurez des périphériques de capture vidéo en .NET avec des exemples de code listant les périphériques, formats et fréquences d'images.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - C#
primary_api_classes:
  - DeviceEnumerator
  - VideoCaptureDeviceSourceSettings
  - VideoCaptureSource

---

# Travailler avec les périphériques de capture vidéo en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introduction à la gestion des périphériques vidéo

Le Video Capture SDK .Net offre une prise en charge robuste de tout périphérique de capture vidéo reconnu par votre système d'exploitation. Ce guide montre comment découvrir les périphériques disponibles, inspecter leurs capacités et les intégrer dans vos applications.

## Énumération des périphériques de capture vidéo disponibles

Avant de pouvoir utiliser un périphérique de capture, vous devez identifier ceux qui sont connectés au système. Les exemples de code suivants montrent comment récupérer la liste des périphériques disponibles et les afficher dans un composant d'interface utilisateur :

=== "VideoCaptureCore"

    
    ```csharp
    // Parcourir tous les périphériques de capture vidéo disponibles connectés au système
    foreach (var device in VideoCapture1.Video_CaptureDevices())
    {
        // Ajouter le nom de chaque périphérique à un contrôle de sélection déroulant
        cbVideoInputDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Récupérer de manière asynchrone toutes les sources vidéo via le DeviceEnumerator partagé
    var devices = DeviceEnumerator.Shared.VideoSourcesAsync();
    
    // Parcourir chaque périphérique disponible
    foreach (var device in await devices)
    {
        // Ajouter le nom convivial du périphérique au contrôle de sélection déroulant
        cbVideoInputDevice.Items.Add(device.DisplayName);
    }
    ```
    


## Découverte des capacités de format vidéo

Après avoir identifié un périphérique de capture, vous pouvez examiner ses formats vidéo et fréquences d'images pris en charge. Cela vous permet de proposer aux utilisateurs des options de configuration appropriées :

=== "VideoCaptureCore"

    
    ```csharp
    // Localiser un périphérique spécifique par son nom affiché
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // Parcourir tous les formats vidéo pris en charge par ce périphérique
    foreach (var format in deviceItem.VideoFormats)
    {
        // Ajouter chaque format au menu déroulant de sélection de format
        cbVideoInputFormat.Items.Add(format);
    
        // Pour chaque format, parcourir les fréquences d'images prises en charge
        foreach (var frameRate in format.FrameRates)
        {
            // Ajouter chaque option de fréquence d'images au menu déroulant
            cbVideoInputFrameRate.Items.Add(frameRate.ToString(CultureInfo.CurrentCulture));
        }
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Localiser un périphérique spécifique par son nom affiché
    var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(device => device.DisplayName == "Some device name");
    
    // Parcourir tous les formats vidéo pris en charge par ce périphérique
    foreach (var format in deviceItem.VideoFormats)
    {
        // Ajouter le nom du format au menu déroulant de sélection de format
        cbVideoInputFormat.Items.Add(format.Name);
    
        // Pour chaque format, récupérer et afficher les fréquences d'images disponibles
        foreach (var frameRate in format.FrameRateList)
        {
            // Ajouter chaque valeur de fréquence d'images au menu déroulant
            cbVideoInputFrameRate.Items.Add(frameRate.ToString());
        }
    }
    ```
    


## Configuration et activation d'un périphérique de capture vidéo

Une fois que vous avez sélectionné un périphérique et identifié vos paramètres de format préférés, vous pouvez initialiser la source de capture avec ces paramètres :

=== "VideoCaptureCore"

    
    ```csharp
    // Trouver le périphérique sélectionné dans la liste
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // Créer une nouvelle source de capture vidéo en utilisant le périphérique sélectionné
    VideoCapture1.Video_CaptureDevice = new VideoCaptureSource(deviceItem.Name);
    
    // Configurer le format vidéo par défaut depuis la première option disponible
    VideoCapture1.Video_CaptureDevice.Format = deviceItem.VideoFormats[0].ToString();
    
    // Définir la fréquence d'images par défaut depuis la première option disponible pour le format sélectionné
    VideoCapture1.Video_CaptureDevice.FrameRate = deviceItem.VideoFormats[0].FrameRates[0];
    
    // Note : après cette configuration, utilisez le mode VideoPreview ou VideoCapture pour démarrer le streaming
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Initialiser la variable de paramètres de source vidéo
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;
    
    // Trouver le périphérique sélectionné par son nom affiché
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
    if (device != null)
    {
        // Localiser le format sélectionné par son nom
        var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
        if (formatItem != null)
        {
            // Créer les paramètres de configuration avec le périphérique sélectionné
            videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
            {
                // Convertir la représentation du format en objet Format requis
                Format = formatItem.ToFormat()
            };
    
            // Définir la fréquence d'images souhaitée depuis la sélection déroulante
            videoSourceSettings.Format.FrameRate = new VideoFrameRate(Convert.ToDouble(cbVideoInputFrameRate.Text));
        }
    }
    
    // Appliquer les paramètres configurés au composant de capture vidéo
    VideoCapture1.Video_Source = videoSourceSettings;
    ```
    


## Ressources supplémentaires et exemples de code

Pour des scénarios d'utilisation plus avancés et des exemples d'implémentation complets, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) contenant des projets de démonstration complets.

## Dépannage de la détection de périphériques

Si votre application ne détecte pas les périphériques attendus, examinez ces problèmes courants :

1. Assurez-vous que le périphérique est correctement connecté et sous tension
2. Vérifiez que les pilotes du périphérique sont correctement installés
3. Vérifiez qu'aucune autre application n'utilise actuellement le périphérique
4. Redémarrez l'application après avoir connecté de nouveaux périphériques
