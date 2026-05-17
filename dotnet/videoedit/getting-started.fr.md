---
title: Prise en main — API d'édition vidéo par timeline en C# .NET
description: Configurez votre premier projet avec VisioForge Video Edit SDK .NET. Timelines personnalisables, formats multiples, transitions, effets et aperçu en direct.
sidebar_position: 0
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Editing
  - MP4
  - C#
primary_api_classes:
  - VideoEditCoreX
  - VideoEditCore
  - VideoSource
  - VideoFileSource
  - IVideoView

---

# Créer des applications d'édition vidéo professionnelles avec le SDK .NET

## Introduction à l'édition vidéo avec .NET

Le Video Edit SDK offre une fonctionnalité puissante pour les développeurs .NET qui souhaitent créer des applications d'édition vidéo professionnelles. Ce SDK prend en charge une large gamme de plateformes et de frameworks d'interface, vous permettant de créer des éditeurs vidéo riches en fonctionnalités qui gèrent plusieurs formats, appliquent des effets, gèrent des transitions et fournissent une sortie de haute qualité.

## Configuration de votre environnement de développement

### Création de votre projet initial

Le SDK est conçu pour fonctionner de manière fluide avec divers environnements de développement. Vous pouvez utiliser Visual Studio ou JetBrains Rider pour créer votre projet, en choisissant la plateforme et le framework d'interface qui correspondent à vos besoins.

Pour un processus de configuration sans heurts, veuillez vous référer à notre [guide d'installation](../install/index.md) détaillé qui fournit les instructions pour ajouter les paquets NuGet nécessaires et configurer correctement les dépendances natives.

## Mise en œuvre des fonctionnalités d'édition vidéo de base

### Initialisation du moteur d'édition vidéo

Le SDK fournit un objet d'édition de base robuste qui sert de fondation à votre application d'édition vidéo. Suivez ces étapes pour créer et initialiser ce composant essentiel :

=== "VideoEditCore"

    
    ```cs
    private VideoEditCore core;
    
    core = new VideoEditCore(VideoView1 as IVideoView);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    private VideoEditCoreX core;
    
    core = new VideoEditCoreX(VideoView1 as IVideoView);
    ```
    


Vous devrez spécifier un objet Video View comme paramètre pour activer la fonctionnalité d'aperçu vidéo pendant les opérations d'édition.

### Mise en place d'une gestion d'événements robuste

#### Gestion des événements d'erreur

Pour une gestion appropriée des erreurs dans votre application, implémentez le gestionnaire d'événement `OnError` :

```cs
core.OnError += Core_OnError;

private void Core_OnError(object sender, ErrorsEventArgs e)
{
    Debug.WriteLine("Error: " + e.Message);
}
```

#### Système de suivi de la progression

Pour tenir vos utilisateurs informés des processus en cours, implémentez le gestionnaire d'événement de progression :

```cs
core.OnProgress += Core_OnProgress;

private void Core_OnProgress(object sender, ProgressEventArgs e)
{
    Debug.WriteLine("Progress: " + e.Progress);
}
```

#### Notification de fin d'opération

Pour détecter la fin des opérations d'édition, implémentez le gestionnaire d'événement d'arrêt :

```cs
core.OnStop += Core_OnStop;

private void Core_OnStop(object sender, StopEventArgs e)
{
    Debug.WriteLine("Editing completed");
}
```

### Configuration des paramètres de la timeline

Avant d'ajouter des sources multimédias à votre projet, vous devez établir les paramètres de base de la timeline tels que la fréquence d'images et la résolution, qui varient selon le moteur utilisé :

=== "VideoEditCore"

    
    ```cs
    core.Video_FrameRate = new VideoFrameRate(30);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
    core.Output_VideoFrameRate = new VideoFrameRate(30);
    ```
    


## Travailler avec des sources multimédias

### Gérer la vidéo et l'audio sur votre timeline

Le SDK vous permet d'ajouter diverses sources multimédias à votre timeline à l'aide de méthodes API simples. Pour chaque source, vous pouvez contrôler avec précision des paramètres tels que l'heure de début, l'heure de fin et la position sur la timeline. Vous avez la flexibilité d'ajouter des sources vidéo et audio indépendamment ou ensemble.

### Intégrer des fichiers vidéo

Le SDK prend en charge une large gamme de formats vidéo, notamment MP4, AVI, MOV, WMV et bien d'autres. Voici comment ajouter du contenu vidéo à votre projet :

D'abord, créez un objet source vidéo et définissez le chemin du fichier source. Vous pouvez spécifier des heures de début et de fin dans le constructeur ou utiliser des paramètres null pour inclure l'intégralité du fichier.

Pour les fichiers comportant plusieurs flux vidéo, vous pouvez sélectionner le flux à utiliser en spécifiant le numéro de flux.

=== "VideoEditCore"

    
    ```cs
    var videoFile = new VisioForge.Core.Types.VideoEdit.VideoSource(
        filename,
        null,
        null, 
        VideoEditStretchMode.Letterbox, 
        0, 
        1.0);
    ```
    
    API :
    
    ```cs
    public VideoSource(
        string filename,
        TimeSpan? startTime = null,
        TimeSpan? stopTime = null,
        VideoEditStretchMode stretchMode = VideoEditStretchMode.Letterbox,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    var videoFile = new VisioForge.Core.Types.X.VideoEdit.VideoFileSource(
        filename,
        null,
        null, 
        0, 
        1.0);
    ```
    
    API :
    
    ```cs
    public VideoFileSource(
        string filename,
        TimeSpan? startTime = null,
        TimeSpan? stopTime = null,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    


Vous pouvez contrôler la vitesse de lecture en ajustant le paramètre rate. Par exemple, définir rate à 2.0 lira le fichier à deux fois la vitesse normale.

Un constructeur alternatif vous permet d'ajouter plusieurs segments de fichier :

=== "VideoEditCore"

    
    ```cs
    public VideoSource(
        string filename,
        FileSegment[] segments,
        VideoEditStretchMode stretchMode = VideoEditStretchMode.Letterbox,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    public VideoFileSource(
        string filename,
        FileSegment[] segments,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    


Pour ajouter la source à votre timeline, utilisez les méthodes suivantes :

=== "VideoEditCore"

    
    ```cs
    await core.Input_AddVideoFileAsync(
        videoFile, 
        null, 
        0);
    ```
    
    Le troisième paramètre spécifie le numéro du flux vidéo de destination. Utilisez 0 pour ajouter la source au premier flux vidéo.
    
    API :
    
    ```cs
    public Task<bool> Input_AddVideoFileAsync(
        VideoSource fileSource,
        TimeSpan? timelineInsertTime = null,
        int targetVideoStream = 0,
        int customWidth = 0,
        int customHeight = 0)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddVideoFile(
        videoFile,
        null);
    ```
    
    API :
    
    ```cs
    public bool Input_AddVideoFile(
        VideoFileSource source, 
        TimeSpan? insertTime = null)
    ```
    


Le second paramètre détermine la position sur la timeline. L'utilisation de null ajoute automatiquement la source à la fin de la timeline existante.

### Intégrer des fichiers audio

Le SDK prend en charge de nombreux formats audio, notamment AAC, MP3, WMA, OPUS, Vorbis et d'autres. Le processus pour ajouter de l'audio est similaire à celui pour ajouter de la vidéo :

=== "VideoEditCore"

    
    ```cs
    var audioFile = new VisioForge.Core.Types.VideoEdit.AudioSource(
        filename,
        startTime: null,
        stopTime: null,
        fileToSync: string.Empty,
        streamNumber: 0,
        rate: 1.0);                      
    ```
    
    Le paramètre `fileToSync` active la synchronisation audio-vidéo. Lorsque vous travaillez avec des fichiers vidéo et audio séparés, vous pouvez spécifier le nom du fichier vidéo dans ce paramètre pour vous assurer que l'audio se synchronise correctement avec la vidéo.
    
    ```cs
    await core.Input_AddAudioFileAsync(
        audioFile,
        insertTime: null, 
        0);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    var audioFile = new AudioFileSource(
        filename,
        startTime: null,
        stopTime: null);
    
    core.Input_AddAudioFile(
        audioFile,
        insertTime: null);
    ```
    


### Combiner des sources vidéo et audio

Pour plus d'efficacité, vous pouvez ajouter des sources combinées vidéo et audio avec un seul appel de méthode :

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddAudioVideoFile(
        filename,
        startTime: null,
        stopTime: null,
        insertTime: null);
    ```
    


### Travailler avec des images fixes

Le SDK prend en charge l'ajout d'images fixes à votre timeline, y compris les formats JPG, PNG, BMP et GIF. Lors de l'ajout d'une image, vous devrez spécifier la durée pendant laquelle elle doit apparaître sur la timeline :

=== "VideoEditCore"

    
    ```cs
    await core.Input_AddImageFileAsync(
        filename,
        duration: TimeSpan.FromMilliseconds(2000),
        timelineInsertTime: null,
        stretchMode: VideoEditStretchMode.Letterbox);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddImageFile(
        filename,
        duration: TimeSpan.FromMilliseconds(2000),
        insertTime: null);
    ```
    


## Configuration des paramètres de sortie

### Définir le format de sortie et les options d'encodage

Le SDK offre des options de sortie flexibles avec la prise en charge de nombreux formats vidéo et audio, notamment MP4, AVI, WMV, MKV, WebM, AAC, MP3, GIF animé et bien d'autres.

Utilisez la propriété `Output_Format` pour configurer le format de sortie souhaité :

=== "VideoEditCore"

    
    ```cs
    var mp4Output = new MP4HWOutput();
    core.Output_Format = mp4Output;
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    var mp4Output = new MP4Output("output.mp4");
    core.Output_Format = mp4Output;
    ```
    


Pour une liste complète des formats de sortie pris en charge et des exemples de code détaillés, veuillez vous référer à la section [Formats de sortie](../general/output-formats/index.md) de notre documentation.

### Rendu vers un GIF animé { #rendering-to-animated-gif }

`VideoEditCoreX` peut effectuer le rendu de la timeline directement vers un GIF animé via `GIFOutput`. GIF n'a pas de conteneur — `gifenc` écrit le fichier `.gif` final en une seule étape, le profil de l'encodeur ne contient donc que le flux vidéo et tout audio présent sur la timeline est ignoré.

```cs
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;

core.Output_Format = new GIFOutput("output.gif", new GIFEncoderSettings
{
    Speed = 10,    // 1..30 — valeur plus élevée = encodage plus rapide, qualité visuelle inférieure
    Repeat = 0     // -1 = boucle infinie, 0 = lire une fois, n = lire n+1 fois
});
```

Notes :

- La sortie GIF est uniquement vidéo — les pistes audio sont ignorées.
- La palette de 256 couleurs par image est une limitation du format — attendez-vous à des fichiers plus volumineux que des codecs vidéo équivalents pour de longs clips.
- `SmartRender = true` est incompatible avec la sortie GIF (aucun clip source n'est déjà en `image/gif`).

## Améliorer vos vidéos

### Appliquer des effets vidéo professionnels

Le SDK fournit une riche collection d'effets vidéo que vous pouvez appliquer pour améliorer votre contenu vidéo. Pour des informations détaillées sur la mise en œuvre des effets, consultez notre guide [Effets vidéo](../general/video-effects/index.md).

Le moteur `VideoEditCoreX` inclut des méthodes API dédiées pour ajouter des superpositions de texte. Pour les détails d'implémentation, consultez notre guide [Superpositions de texte](./code-samples/add-text-overlay.md).

### Ajouter des transitions fluides

Pour créer des transitions d'aspect professionnel entre les clips vidéo, consultez notre [exemple de code détaillé sur l'utilisation des transitions](./code-samples/transition-video.md).

## Traiter votre projet vidéo

### Démarrer le processus d'édition

Une fois que vous avez configuré toutes vos sources, effets et paramètres de sortie, vous pouvez lancer le processus d'édition :

=== "VideoEditCore"

    
    ```cs
    await core.StartAsync();
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Start();
    ```
    


Pendant le processus d'édition, votre application recevra des mises à jour de progression via les gestionnaires d'événements que vous avez implémentés, et vous serez notifié de la fin de l'opération via l'événement d'arrêt.

## Conclusion

En suivant ce guide, vous avez appris les techniques fondamentales pour créer une application d'édition vidéo puissante à l'aide du Video Edit SDK pour .NET. Cette base vous permettra de créer des outils d'édition vidéo sophistiqués qui peuvent rivaliser avec les logiciels d'édition vidéo professionnels tout en étant adaptés à vos exigences spécifiques.
