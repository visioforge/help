---
title: Capture et intégration de sources vidéo NDI en C# .NET
description: Découvrez, connectez, prévisualisez et capturez des sources vidéo NDI avec VisioForge Video Capture SDK. Inclut le lecteur NDI Android et MAUI en C# .NET.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - IP Camera
  - NDI Source
  - RTSP
  - ONVIF
  - NDI
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureCore
  - NDISourceSettings
  - NDISourceInfo
  - DeviceEnumerator
  - IPCameraSourceSettings

---

# Implémentation de sources vidéo NDI dans des applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! info "Prise en charge multiplateforme"
    Le moteur `VideoCaptureCoreX` avec NDI fonctionne sous **Windows, macOS, Linux, Android et iOS** via GStreamer ; installez le runtime NDI pour chaque plateforme cible. La source NDI classique `VideoCaptureCore` est réservée à Windows. Consultez la [matrice de prise en charge des plateformes](../../../platform-matrix.md) et le [guide de déploiement Linux](../../../deployment-x/Ubuntu.md).

## Introduction à la technologie NDI

Network Device Interface (NDI) est un standard haute performance pour les workflows de production basés sur IP, développé par NewTek. Il permet aux produits compatibles vidéo de communiquer, transmettre et recevoir de la vidéo de qualité broadcast sur une connexion réseau standard avec une faible latence.

Notre SDK offre une prise en charge robuste des sources NDI, permettant à vos applications .NET de s'intégrer de manière transparente avec les caméras NDI et les logiciels compatibles NDI. Cela en fait une solution idéale pour les environnements de production en direct, les applications de streaming, les solutions de visioconférence et tout système nécessitant une intégration vidéo réseau de haute qualité.

### Prérequis pour l'intégration NDI

Avant d'implémenter la fonctionnalité NDI dans votre application, vous devrez installer l'un des éléments suivants :

- [NDI SDK](https://ndi.video/for-developers/ndi-sdk/#download) — Recommandé pour les développeurs créant des applications professionnelles
- NDI Tools — Suffisant pour les tests et le développement de base

Ces outils fournissent les composants runtime nécessaires à la communication NDI. Après l'installation, votre système pourra découvrir et se connecter aux sources NDI de votre réseau.

Pour la lecture sur Android, installez le **NDI Advanced SDK for Android** et empaquetez les fichiers `libndi.so` spécifiques à chaque ABI avec votre APK. Les exemples NDI Player pour Android et MAUI recherchent le répertoire `Lib` du SDK via la propriété MSBuild `NdiAndroidSdkLib`, puis la variable d'environnement `NDI_ANDROID_SDK_LIB`, puis `C:\Program Files\NDI\NDI 6 SDK (Android)\Lib`.

Les applications Android qui découvrent des sources NDI doivent demander ces permissions :

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_WIFI_MULTICAST_STATE" />
```

## Découverte de sources NDI sur votre réseau

La première étape pour travailler avec NDI consiste à énumérer les sources disponibles. Notre SDK simplifie ce processus grâce à des méthodes dédiées pour scanner votre réseau à la recherche de périphériques et applications compatibles NDI.

### Énumération des sources NDI disponibles

=== "VideoCaptureCore"

    
    ```cs
    var lst = await VideoCapture1.IP_Camera_NDI_ListSourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    var lst = await DeviceEnumerator.Shared.NDISourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri.URL);
    }
    ```
    


Les méthodes d'énumération asynchrones scannent votre réseau et retournent une liste des sources NDI disponibles. Chaque source possède un identifiant unique que vous utiliserez pour établir une connexion. Le processus d'énumération prend généralement quelques secondes, selon les conditions réseau et le nombre de sources disponibles.

Pour l'UI d'un lecteur, abonnez-vous à `NDISourcesChanged` et démarrez l'observateur pour que la liste des sources reflète les émetteurs qui apparaissent ou disparaissent après l'analyse initiale :

```cs
DeviceEnumerator.Shared.NDISourcesChanged += OnNDISourcesChanged;
DeviceEnumerator.Shared.StartNDISourceWatch();
```

Arrêtez l'observateur et désabonnez-vous lors de l'arrêt.

## Connexion aux sources NDI

Une fois les sources NDI de votre réseau identifiées, l'étape suivante consiste à établir une connexion. Cela implique de créer l'objet de paramètres approprié et de le configurer selon vos besoins spécifiques.

### Configuration des paramètres de source NDI

=== "VideoCaptureCore"

    
    ```cs
    // Créer un objet de paramètres de source de caméra IP
    settings = new IPCameraSourceSettings
    {
        URL = new Uri("ndi://HOSTNAME/SourceName")
    };
    
    // Définir le type de source sur NDI
    settings.Type = IPSourceEngine.NDI;
    
    // Activer ou désactiver la capture audio
    settings.AudioCapture = false; 
    
    // Définir les informations de connexion si nécessaire
    settings.Login = "username";
    settings.Password = "password";
    
    // Affecter la source à l'objet VideoCaptureCore
    VideoCapture1.IP_Camera_Source = settings;

    // Sélectionner le mode de fonctionnement avant StartAsync :
    //   IPPreview — aperçu en direct uniquement (pas de sortie fichier).
    //   IPCapture — aperçu + enregistrement vers la cible Output_Format configurée.
    VideoCapture1.Mode = VideoCaptureMode.IPPreview;   // ou VideoCaptureMode.IPCapture

    await VideoCapture1.StartAsync();
    ```
    

=== "VideoCaptureCoreX"

    
    Dans VideoCaptureCoreX, vous avez deux options pour créer les paramètres de source NDI :
    
    **Option 1 : utiliser l'URL de la source NDI**
    
    ```cs
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), null, "NDI URL");
    ```
    
    **Option 2 : utiliser le nom de la source NDI**
    
    ```cs
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), cbIPURL.Text, null);
    ```
    
    Enfin, affectez la source à l'objet VideoCaptureCoreX :
    
    ```cs
    VideoCapture1.Video_Source = ndiSettings;
    ```  
    


## Modèle de lecteur NDI Android et MAUI

Les démos NDI Player Android et MAUI de `Video Capture SDK X` utilisent `VideoCaptureCoreX` comme simple récepteur/lecteur NDI. Le même flux fonctionne pour les lecteurs plein écran, les outils de monitoring et les panneaux d'aperçu :

1. Initialiser le SDK.
2. Énumérer les sources NDI avec `DeviceEnumerator.Shared.NDISourcesAsync()`.
3. Maintenir la liste à jour avec `NDISourcesChanged` et `StartNDISourceWatch()`.
4. Créer `NDISourceSettings` à partir du `NDISourceInfo` sélectionné.
5. Affecter les paramètres à `VideoCaptureCoreX.Video_Source`.
6. Activer la lecture audio uniquement lorsque la source sélectionnée expose des flux audio.
7. Arrêter et libérer le moteur lorsque la lecture s'arrête ou que la vue se ferme.

```cs
var sources = await DeviceEnumerator.Shared.NDISourcesAsync();
var info = sources[0];

var settings = await NDISourceSettings.CreateAsync(null, info);
if (settings == null || !settings.IsValid())
{
    throw new InvalidOperationException("Failed to create NDI source settings.");
}

var core = new VideoCaptureCoreX(videoView);
core.Video_Source = settings;
core.Audio_Play = settings.GetInfo()?.AudioStreams?.Count > 0;

await core.StartAsync();
```

### UI native Android

L'exemple Android utilise `VisioForge.Core.UI.Android.VideoViewGL` comme surface de rendu :

```cs
var core = new VideoCaptureCoreX(videoView);
```

Sur Android, demandez les permissions réseau et multicast avant la découverte. Si l'application est compilée sans `libndi.so` pour l'ABI actuel, la lecture échoue à l'exécution avec `DllNotFoundException`, donc vérifiez le chemin `Lib` du NDI Advanced SDK avant l'empaquetage.

### UI .NET MAUI

L'exemple MAUI enregistre les gestionnaires VisioForge dans `MauiProgram` :

```cs
builder
    .UseMauiApp<App>()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

Créez `VideoCaptureCoreX` avec la vue de la plateforme provenant du `VideoView` MAUI :

```cs
var core = new VideoCaptureCoreX(videoView.GetVideoView());
```

Lors de l'arrêt, annulez les rafraîchissements de sources en attente, désabonnez-vous de `NDISourcesChanged`, appelez `StopNDISourceWatch()`, arrêtez/libérez `VideoCaptureCoreX` et détruisez le SDK si votre application en gère le cycle de vie.


## Options de configuration NDI avancées

### Optimisation des performances

Lorsque vous travaillez avec des sources NDI, les considérations de performance sont importantes, en particulier dans les environnements professionnels. Voici quelques conseils pour optimiser votre implémentation NDI :

1. **Bande passante réseau** : assurez-vous que votre réseau dispose d'une bande passante suffisante pour les flux NDI. Un flux NDI HD typique nécessite environ 100-150 Mbit/s.

2. **Accélération matérielle** : activez l'accélération matérielle lorsqu'elle est disponible pour réduire l'utilisation CPU et améliorer les performances.

3. **Contrôle de la fréquence d'images** : envisagez de limiter la fréquence d'images si vous n'avez pas besoin de la qualité maximale, ce qui peut réduire la charge réseau.

4. **Paramètres de résolution** : choisissez des paramètres de résolution adaptés aux besoins de votre application et à la bande passante disponible.

### Gestion de plusieurs sources NDI

Pour les applications qui doivent gérer plusieurs sources NDI simultanément :

1. Créez des instances de capture séparées pour chaque source NDI
2. Implémentez un pool de ressources pour gérer efficacement les ressources système
3. Envisagez d'utiliser un pattern producteur/consommateur pour le traitement de plusieurs flux
4. Surveillez les performances du système et ajustez les paramètres au besoin

## Gestion des erreurs et dépannage

Lors de l'implémentation de la fonctionnalité NDI, il est important de gérer les problèmes potentiels avec élégance :

### Problèmes courants de connexion NDI

1. **Source non trouvée** : vérifiez que la source NDI est active et sur le même réseau
2. **Délai de connexion dépassé** : vérifiez la configuration réseau et les paramètres du pare-feu
3. **Échec d'authentification** : assurez-vous que les identifiants sont corrects si l'authentification est requise
4. **Problèmes de performance** : surveillez l'utilisation CPU et réseau pendant la capture

### Implémentation d'une gestion d'erreurs robuste

```cs
// Le SDK n'expose pas de types d'exception spécifiques à NDI — les éléments GStreamer sous-jacents
// remontent les échecs sous forme d'Exception générique. Branchez sur le résultat de la découverte
// (null / vide) pour "source non trouvée", et sur l'échec de CreateAsync pour les problèmes de connexion.
try
{
    var discovered = await DeviceEnumerator.Shared.NDISourcesAsync();
    var match = discovered.FirstOrDefault(s => s.URL == ndiUrl);
    if (match == null)
    {
        Log.Error($"NDI source not found on the network: {ndiUrl}");
        return;
    }

    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), match);
    VideoCapture1.Video_Source = ndiSettings;
}
catch (Exception ex)
{
    // Tous les échecs (découverte, création des paramètres, initialisation des éléments GStreamer, etc.)
    Log.Error($"Failed to connect to NDI source: {ex.Message}");
}
```

## Intégration aux flux de traitement vidéo

Les sources NDI peuvent être intégrées de manière transparente à d'autres composants de votre pipeline de traitement vidéo :

1. **Enregistrement** : capturez les flux NDI vers divers formats de fichier
2. **Streaming en direct** : transférez le contenu NDI vers des services de streaming
3. **Traitement vidéo** : appliquez des filtres et effets aux sources NDI en temps réel
4. **Composition multi-vues** : combinez plusieurs sources NDI en une seule sortie

## Applications d'exemple et références de code

Pour vous aider à démarrer avec l'implémentation NDI, nous fournissons plusieurs applications d'exemple qui démontrent différents aspects de la fonctionnalité NDI.

=== "VideoCaptureCore"

    
    - [Capture de source NDI vers MP4 (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/NDI%20Source)
    - [Exemple principal du SDK (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [Exemple NDI et autres sources (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/IP_Capture)
    - [Exemple principal du SDK (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    

=== "VideoCaptureCoreX"

    
    - [Aperçu de source NDI (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/NDI%20Source%20Demo)
    - [Aperçu et capture NDI (et autres sources) (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture)
    - [Lecteur NDI (Android)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/Android/NDIPlayer)
    - [Lecteur NDI (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI/NDIPlayer)
    


## Bonnes pratiques pour l'implémentation NDI

Pour garantir des performances et une fiabilité optimales lors du travail avec des sources NDI :

1. **Effectuez une énumération régulière des sources** : les conditions réseau et les sources disponibles peuvent changer ; réénumérez périodiquement les sources.

2. **Implémentez une logique de reconnexion** : des interruptions réseau peuvent survenir ; implémentez des tentatives de reconnexion automatique.

3. **Surveillez l'état des flux** : suivez la fréquence d'images, la latence et la stabilité des connexions pour détecter les problèmes potentiels.

4. **Gérez gracieusement les déconnexions de sources** : implémentez des gestionnaires d'événements pour les déconnexions inattendues.

5. **Testez avec différentes sources NDI** : les différentes implémentations NDI peuvent avoir de légères variations ; testez avec plusieurs sources.

6. **Empaquetez les bibliothèques natives Android par ABI** : pour les récepteurs Android, incluez `libndi.so` pour chaque ABI livré. Les bibliothèques ABI manquantes provoquent des échecs à l'exécution même si l'APK se compile.

7. **Respectez le cycle de vie mobile** : arrêtez la lecture lorsque les activités Android s'arrêtent ou que les pages/fenêtres MAUI se ferment, libérez le moteur, annulez l'énumération des sources et supprimez les gestionnaires d'événements de l'observateur.

## Conclusion

La technologie NDI offre des capacités puissantes pour l'intégration vidéo réseau dans les applications .NET. Avec notre SDK, vous pouvez facilement incorporer de la vidéo de haute qualité à faible latence depuis des sources NDI dans vos projets logiciels. Que vous construisiez un système de production en direct, une application de visioconférence ou toute solution nécessitant de la vidéo réseau, notre implémentation NDI fournit les outils dont vous avez besoin pour réussir.

Les exemples de code fournis démontrent les patterns essentiels pour travailler avec des sources NDI, de l'énumération et la connexion à la capture et au traitement. En suivant ces patterns et bonnes pratiques, vous pouvez créer des applications NDI robustes offrant des performances et une fiabilité exceptionnelles.

## Documentation associée

- [Vue d'ensemble des caméras IP](index.md) — types de sources RTSP, ONVIF et NDI en un coup d'œil
- [Configuration des sources de caméras RTSP](rtsp.md) — protocole de caméra IP le plus courant
- [Intégration des caméras IP ONVIF](onvif.md) — découverte et contrôle PTZ basés sur des standards
- [Tutoriel d'aperçu en direct d'une caméra IP](../../video-tutorials/ip-camera-preview.md) — exemple d'aperçu fonctionnel minimal
- [Plongée approfondie dans le protocole RTSP](../../../general/network-streaming/rtsp.md) — fonctionnement interne du protocole de streaming
- [Guide de sortie streaming NDI](../../../general/network-streaming/ndi.md) — envoi de caméras, périphériques de capture et fichiers vers NDI

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
