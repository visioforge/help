---
title: Capture et lecture vidéo Blackmagic Decklink en C# .NET
description: Capturez la vidéo depuis des cartes Blackmagic Decklink en .NET avec le SDK VisioForge. Énumération, sélection de format et exemples de code C#.
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
  - Decklink
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - DeviceEnumerator
  - DecklinkVideoSourceSettings
  - DecklinkAudioSourceSettings
  - DecklinkSourceSettings

---

# Intégrer des périphériques Blackmagic Decklink dans des applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Qu'est-ce qu'un périphérique Decklink ?

Les périphériques Decklink de Blackmagic Design représentent des cartes de capture et de lecture standard de l'industrie, conçues pour les environnements de production vidéo professionnelle. Ces solutions matérielles haute performance offrent des capacités exceptionnelles aux développeurs intégrant la fonctionnalité de capture vidéo dans des applications .NET.

Les cartes Decklink sont reconnues pour leurs spécifications techniques supérieures :

- Prise en charge de formats vidéo haute résolution, notamment 4K, 8K et HD
- Plusieurs options de connexion d'entrée/sortie (SDI, HDMI) pour une intégration flexible
- Performances à faible latence cruciales pour les applications de diffusion en temps réel
- Compatibilité multiplateforme avec les principaux logiciels de production vidéo
- API conviviales pour les développeurs, s'intégrant facilement avec différents langages de programmation

Pour les développeurs .NET créant des applications vidéo professionnelles, l'intégration Decklink fournit une base fiable pour gérer le traitement de signaux vidéo de qualité diffusion.

## Détection programmatique des périphériques

La première étape pour implémenter la fonctionnalité Decklink consiste en une énumération correcte des périphériques. Les exemples de code suivants montrent comment détecter le matériel Decklink disponible dans votre application .NET.

### Énumérer les périphériques disponibles

=== "VideoCaptureCore"

    
    ```csharp
    foreach (var device in (await VideoCapture1.Decklink_CaptureDevicesAsync()))
    {   
        cbDecklinkCaptureDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    var videoCaptureDevices = await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync();
    if (videoCaptureDevices.Length > 0)
    {
        foreach (var item in videoCaptureDevices)
        {
            cbVideoInput.Items.Add(item.Name);
        }
    
        cbVideoInput.SelectedIndex = 0;
    }
    ```
    
    Avec VideoCaptureCoreX, vous devrez également énumérer les sources audio séparément :
    
    ```cs
    var audioCaptureDevices = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();
    if (audioCaptureDevices.Length > 0)
    {
        foreach (var item in audioCaptureDevices)
        {
            cbAudioInput.Items.Add(item.Name);
        }
    
        cbAudioInput.SelectedIndex = 0;
    }
    ```
    


## Travailler avec les formats vidéo et les fréquences d'images

Après l'énumération des périphériques, l'étape suivante consiste à déterminer les formats vidéo et fréquences d'images disponibles. Les cartes Decklink prennent en charge de nombreux formats professionnels, mais les options spécifiques dépendent de la source d'entrée connectée.

### Récupérer les informations de format

=== "VideoCaptureCore"

    
    ```csharp
    // Énumérer et filtrer par nom de périphérique
    var deviceItem = (await VideoCapture1.Decklink_CaptureDevicesAsync()).Find(device => device.Name == cbDecklinkCaptureDevice.Text);
    if (deviceItem != null)
    {
        // Lire les formats vidéo et les ajouter à la liste déroulante
        foreach (var format in (await deviceItem.GetVideoFormatsAsync()))
        {
            cbDecklinkCaptureVideoFormat.Items.Add(format.Name);
        }
    
        // Si aucun format n'existe, ajouter un message
        if (cbDecklinkCaptureVideoFormat.Items.Count == 0)
        {
            cbDecklinkCaptureVideoFormat.Items.Add("No input connected");
        }
    }
    ```
    
    Cette approche fournit des informations de diagnostic précieuses. Si aucun format n'est retourné, cela indique typiquement qu'aucune entrée active n'est connectée au périphérique Decklink. L'implémentation de cette vérification aide les utilisateurs à diagnostiquer les problèmes de connexion directement depuis l'interface de votre application.
    

=== "VideoCaptureCoreX"

    
    Avec VideoCaptureCoreX, vous travaillerez avec des modes prédéfinis de l'énumération DecklinkMode :
    
    ```cs
    var decklinkModes = Enum.GetValues(typeof(DecklinkMode));
    foreach (var item in decklinkModes)
    {
        cbVideoMode.Items.Add(item.ToString());
    }
    ```
    
    Cette approche utilise des paramètres de mode standardisés qui doivent être configurés sur votre matériel Decklink.
    


## Configurer Decklink comme source vidéo

Une fois que vous avez détecté les périphériques et identifié les formats disponibles, vous devez configurer le matériel Decklink comme votre source de capture. Cette étape critique établit la connexion entre le matériel et le logiciel.

### Définir les paramètres de la source

=== "VideoCaptureCore"

    
    ```csharp
    VideoCapture1.Decklink_Source = new DecklinkSourceSettings
    {
        Name = cbDecklinkCaptureDevice.Text,
        VideoFormat = cbDecklinkCaptureVideoFormat.Text
    };

    // Sélectionner le mode Decklink avant StartAsync :
    //   DecklinkSourcePreview — surveillance en temps réel avec un traitement minimal.
    //   DecklinkSourceCapture — enregistrement de haute qualité avec mise en mémoire tampon améliorée.
    VideoCapture1.Mode = VideoCaptureMode.DecklinkSourcePreview;   // ou VideoCaptureMode.DecklinkSourceCapture

    await VideoCapture1.StartAsync();
    ```
    
    - `DecklinkSourcePreview` : optimisé pour la surveillance en temps réel avec un traitement minimal
    - `DecklinkSourceCapture` : configuré pour l'enregistrement de haute qualité avec une mise en mémoire tampon améliorée
    

=== "VideoCaptureCoreX"

    
    VideoCaptureCoreX nécessite une configuration séparée des sources vidéo et audio :
    
    ```cs
    // Créer les paramètres pour la source vidéo
    DecklinkVideoSourceSettings videoSourceSettings = null;
    
    // Nom du périphérique depuis la liste déroulante
    var deviceName = cbVideoInput.Text;
    
    // Mode depuis la liste déroulante
    var mode = cbVideoMode.Text;
    if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(mode))
    {
        // Trouver le périphérique
        var device = (await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync()).FirstOrDefault(x => x.Name == deviceName);
        if (device != null)
        {
            // Créer les paramètres de source vidéo en utilisant le périphérique et le mode
            videoSourceSettings = new DecklinkVideoSourceSettings(device)
            {
                Mode = (DecklinkMode)Enum.Parse(typeof(DecklinkMode), mode, true)
            };
        }
    }
    
    // Définir la source vidéo sur l'objet VideoCaptureCoreX
    VideoCapture1.Video_Source = videoSourceSettings;
    ```
    
    Pour la configuration audio :
    
    ```cs
    // Créer les paramètres pour la source audio
    DecklinkAudioSourceSettings audioSourceSettings = null;
    
    // Nom du périphérique depuis la liste déroulante
    deviceName = cbAudioInput.Text;
    if (!string.IsNullOrEmpty(deviceName))
    {
        // Trouver le périphérique
        var device = (await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync()).FirstOrDefault(x => x.Name == deviceName);
        if (device != null)
        {
            // Créer les paramètres pour la source audio en utilisant le périphérique
            audioSourceSettings = new DecklinkAudioSourceSettings(device);
        }
    }
    
    // Définir la source audio sur l'objet VideoCaptureCoreX
    VideoCapture1.Audio_Source = audioSourceSettings;
    ```
    
    Cette séparation offre une plus grande flexibilité pour les scénarios avancés où vous pourriez avoir besoin de traiter la vidéo et l'audio indépendamment.
    


## Considérations de performance

Lors de l'implémentation de la capture Decklink dans des environnements de production, prenez en compte ces facteurs de performance :

1. **Gestion des tampons :** les formats vidéo professionnels nécessitent une allocation mémoire substantielle, en particulier pour les résolutions 4K et au-delà
2. **Utilisation du CPU :** l'encodage en temps réel des flux Decklink peut être gourmand en processeur
3. **E/S disque :** lors de la capture vers le stockage, assurez-vous que vos vitesses d'écriture supportent le débit de données du format sélectionné
4. **Bande passante mémoire :** les flux non compressés haute résolution demandent des ressources système importantes

L'implémentation d'une gestion d'erreurs appropriée autour de la connexion du périphérique et de la détection de format améliorera la résilience de votre application en environnement de production.

## Exemples d'applications et d'implémentations

L'examen d'exemples fonctionnels fournit des informations précieuses sur les modèles d'implémentation efficaces. Le SDK comprend de nombreux exemples d'applications démontrant l'intégration Decklink.

### Applications de référence

=== "VideoCaptureCore"

    
    - [Exemple principal avec entrée DeckLink, traitement vidéo/audio et nombreux formats de sortie (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    - [Exemple principal pour WinForms](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [Exemple simple avec entrée DeckLink et nombreux formats de sortie (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Decklink%20Demo)
    

=== "VideoCaptureCoreX"

    
    - [Aperçu vidéo et capture vers un fichier MP4 ou WebM (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Decklink%20Demo%20X)
    


## Bonnes pratiques d'intégration

Sur la base de tests approfondis sur le terrain et de retours d'expérience clients, nous recommandons ces bonnes pratiques :

1. **Toujours vérifier la connectivité du périphérique :** vérifiez les formats disponibles pour confirmer une connexion de signal correcte
2. **Implémenter des solutions de repli gracieuses :** fournissez des messages d'erreur significatifs lorsque les périphériques attendus ne sont pas disponibles
3. **Tester avec plusieurs fréquences d'images :** certaines applications peuvent se comporter différemment avec des formats d'entrée variés
4. **Gérer efficacement la mémoire :** la capture haute résolution nécessite une gestion correcte des ressources
5. **Surveiller l'utilisation du CPU :** les opérations d'encodage peuvent être gourmandes en processeur pendant la capture
6. **Fournir les détails du format aux utilisateurs :** donnez des informations claires sur les formats détectés et l'état de la connexion

Ces recommandations contribuent à garantir des implémentations robustes qui fonctionnent de manière fiable sur différentes configurations matérielles et conditions d'exploitation.

## Conclusion

L'intégration de périphériques Blackmagic Decklink avec des applications .NET fournit des capacités puissantes pour les scénarios de capture vidéo professionnelle. En suivant les modèles d'implémentation décrits dans ce guide, les développeurs peuvent créer des applications stables et hautes performances qui tirent pleinement parti des capacités du matériel Decklink.

Le Video Capture SDK offre une approche simplifiée pour travailler avec ces périphériques professionnels, en masquant une grande partie de la complexité tout en fournissant la flexibilité nécessaire à une personnalisation avancée.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour accéder à des exemples de code et à des ressources d'implémentation supplémentaires.
