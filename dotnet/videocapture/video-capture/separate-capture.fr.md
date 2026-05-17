---
title: Démarrer/arrêter l'enregistrement sans arrêter l'aperçu
description: Contrôlez l'enregistrement vidéo indépendamment de l'aperçu en direct avec le SDK VisioForge. Mode capture séparée pour webcam, écran et caméra IP en C#.
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
  - Webcam
  - IP Camera
  - Screen Capture
  - RTMP
  - MP4
  - C#
primary_api_classes:
  - VideoCaptureMode
  - MP4Output
  - RTMPOutput

---

# Gestion indépendante de la capture vidéo et de l'aperçu en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Lors du développement d'applications vidéo, il est souvent nécessaire de démarrer ou d'arrêter l'enregistrement tout en maintenant un aperçu ininterrompu. Cette capacité est essentielle pour créer des logiciels d'enregistrement vidéo professionnels, des applications de sécurité ou tout scénario où un retour visuel continu est requis indépendamment de l'état d'enregistrement.

Ce guide montre comment contrôler indépendamment les opérations de capture vidéo sans affecter l'affichage de l'aperçu. Cette technique s'applique à divers scénarios de capture, notamment l'enregistrement par caméra, la capture d'écran et d'autres sources d'entrée.

## Pourquoi séparer l'aperçu et la capture ?

La séparation des fonctionnalités d'aperçu et de capture présente plusieurs avantages :

1. **Expérience utilisateur améliorée** — Les utilisateurs peuvent voir en continu ce qui est capturé, même quand ils n'enregistrent pas
2. **Efficacité des ressources** — Évite l'arrêt et le redémarrage inutiles des flux vidéo
3. **Latence réduite** — Élimine le délai associé au rétablissement de l'aperçu après l'arrêt d'un enregistrement
4. **Contrôle accru** — Fournit une gestion plus précise des sessions d'enregistrement

## Options d'implémentation

Il existe deux approches principales pour implémenter cette fonctionnalité selon la version du SDK que vous utilisez :

=== "VideoCaptureCoreX"

    
    ### Méthode 1 : utiliser VideoCaptureCoreX
    
    L'approche VideoCaptureCoreX offre une manière simplifiée de gérer les sorties et de contrôler les états de capture.
    
    #### Étape 1 : configurer la sortie
    
    Tout d'abord, ajoutez une nouvelle sortie avec les paramètres souhaités. Dans cet exemple, nous utiliserons la sortie MP4. Notez le paramètre `false` qui indique que nous ne voulons pas démarrer la capture immédiatement :
    
    ```cs
    VideoCapture1.Outputs_Add(new MP4Output("output.mp4"), false); // false - ne pas démarrer la capture immédiatement. 
    ```
    
    #### Étape 2 : démarrer uniquement l'aperçu
    
    Ensuite, démarrez l'aperçu vidéo sans initier la capture :
    
    ```cs
    await VideoCapture1.StartAsync();
    ```
    
    #### Étape 3 : démarrer la capture lorsque nécessaire
    
    Lorsque vous souhaitez commencer l'enregistrement, démarrez la capture vidéo réelle vers votre destination de sortie :
    
    ```cs
    await VideoCapture1.StartCaptureAsync(0, "output.mp4"); // 0 - index de la sortie.
    ```
    
    #### Étape 4 : arrêter la capture tout en maintenant l'aperçu
    
    Pour arrêter l'enregistrement tout en gardant l'aperçu actif :
    
    ```cs
    await VideoCapture1.StopCaptureAsync(0); // 0 - index de la sortie.
    ```
    
    ### Gestion avancée des sorties
    
    Vous pouvez ajouter plusieurs sorties avec différents paramètres :
    
    ```cs
    // Ajouter la sortie MP4
    VideoCapture1.Outputs_Add(new MP4Output("primary_recording.mp4"), false);
    
    // Ajouter une sortie supplémentaire pour le streaming
    VideoCapture1.Outputs_Add(new RTMPOutput("rtmp://streaming.example.com/live/stream"), false);
    
    // Démarrer l'aperçu
    await VideoCapture1.StartAsync();
    
    // Démarrer l'enregistrement vers les deux sorties
    await VideoCapture1.StartCaptureAsync(0, "primary_recording.mp4");
    await VideoCapture1.StartCaptureAsync(1, "rtmp://streaming.example.com/live/stream");
    ```
    
    ### Contrôle des sorties par index
    
    Lors de la gestion de plusieurs sorties, le paramètre d'index devient crucial :
    
    ```cs
    // Arrêter l'enregistrement MP4 mais poursuivre le streaming
    await VideoCapture1.StopCaptureAsync(0);
    
    // Plus tard, arrêter également le flux
    await VideoCapture1.StopCaptureAsync(1);
    ```
    

=== "VideoCaptureCore"

    
    ### Méthode 2 : utiliser VideoCaptureCore
    
    L'approche VideoCaptureCore utilise un modèle différent avec une activation explicite de la capture séparée.
    
    #### Étape 1 : activer le mode capture séparée
    
    Commencez par activer la fonctionnalité de capture séparée :
    
    ```cs
    VideoCapture1.SeparateCapture_Enabled = true;
    ```
    
    #### Étape 2 : configurer le mode de capture
    
    Définissez le mode de capture approprié pour votre application :
    
    ```cs
    VideoCapture1.Mode = VideoCaptureMode.VideoCapture;
    // Les autres options incluent :
    // VideoCaptureMode.ScreenCapture
    // VideoCaptureMode.AudioCapture
    // etc.
    ```
    
    #### Étape 3 : configurer le format de sortie
    
    Définissez la configuration du format de sortie souhaité :
    
    ```cs
    VideoCapture1.Output_Format = ...
    ```
    
    #### Étape 4 : démarrer l'aperçu
    
    Lancez l'aperçu sans démarrer l'enregistrement réel :
    
    ```cs
    await VideoCapture1.StartAsync();
    ```
    
    #### Étape 5 : démarrer la capture lorsque nécessaire
    
    Lorsque vous souhaitez commencer l'enregistrement, démarrez le processus de capture séparée :
    
    ```cs
    // SeparateCapture_StartAsync prend le nom du fichier de sortie pour l'enregistrement.
    await VideoCapture1.SeparateCapture_StartAsync("recording.mp4");
    ```
    
    #### Étape 6 : arrêter la capture tout en maintenant l'aperçu
    
    Pour arrêter l'enregistrement tout en gardant l'aperçu actif :
    
    ```cs
    await VideoCapture1.SeparateCapture_StopAsync();
    ```
    
    ### Changement dynamique du nom de fichier
    
    Un avantage clé de l'approche de capture séparée est la possibilité de modifier le nom du fichier de sortie pendant une session d'enregistrement active :
    
    ```cs
    await VideoCapture1.SeparateCapture_ChangeFilenameOnTheFlyAsync("newfile.mp4");
    ```
    
    Ceci est particulièrement utile pour :
    
    - Créer des segments de fichier séquentiels
    - Implémenter des limites de taille de fichier avec continuation automatique
    - Réagir aux changements de nom de fichier initiés par l'utilisateur
    


## Considérations d'implémentation

### Mémoire et performances

Lors de l'implémentation de la capture et de l'aperçu séparés, gardez à l'esprit ces considérations de performance :

- **Utilisation de la mémoire** : maintenir un aperçu actif sans capturer consomme des ressources système
- **Impact CPU** : les opérations d'encodage pendant la capture augmentent la charge CPU
- **Gestion des tampons** : assurez une gestion correcte des tampons pour éviter les fuites de mémoire

### Considérations d'interface utilisateur

L'interface de votre application doit indiquer clairement l'état actuel de l'aperçu et de la capture :

- Utilisez différents indicateurs visuels pour l'aperçu seul et l'enregistrement actif
- Implémentez des contrôles d'interface utilisateur appropriés pour chaque état
- Envisagez d'ajouter des minuteurs et des indicateurs d'enregistrement

## Bonnes pratiques d'intégration

Pour des performances et une fiabilité optimales :

1. **Initialiser tôt** : configurez votre capture au démarrage de l'application
2. **Libérer les ressources** : arrêtez toujours la capture et l'aperçu lorsqu'ils ne sont plus nécessaires
3. **Gérer les changements de périphériques** : implémentez une détection et une gestion correctes de la connexion/déconnexion des périphériques
4. **Gestion des threads** : effectuez les opérations de capture sur des threads en arrière-plan pour éviter le gel de l'interface

## Conclusion

La séparation des opérations de capture et d'aperçu vidéo offre une plus grande flexibilité et une meilleure expérience utilisateur dans les applications vidéo. En suivant les approches décrites dans ce guide, vous pouvez implémenter cette fonctionnalité dans vos applications .NET avec les composants VideoCaptureCoreX ou VideoCaptureCore.

Ces techniques peuvent être appliquées à un large éventail de scénarios, notamment l'enregistrement par webcam, la capture d'écran, les systèmes de surveillance et les outils de production vidéo professionnelle.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
