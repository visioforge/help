---
title: Initialiser et configurer les SDK vidéo .NET VisioForge
description: Initialisez et désinitialisez les SDK .NET de capture, d'édition et de lecture vidéo avec DirectShow et les moteurs X multiplateformes.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing

---

# Initialisation

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Types de moteurs SDK

Tous les SDK contiennent des moteurs basés sur DirectShow réservés à Windows ainsi que des moteurs X multiplateformes.

### Moteurs réservés à Windows

- VideoCaptureCore
- VideoEditCore
- MediaPlayerCore

### Moteurs X

- VideoCaptureCoreX
- VideoEditCoreX
- MediaPlayerCoreX
- MediaBlocksPipeline

Les moteurs X requièrent des étapes supplémentaires d'initialisation et de désinitialisation.

## Initialisation et désinitialisation du SDK pour les moteurs X

Vous devez initialiser le SDK avant toute utilisation d'une classe du SDK, et le désinitialiser avant la sortie de l'application.

Pour initialiser le SDK, utilisez le code suivant :

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

Pour désinitialiser le SDK, utilisez le code suivant :

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

Si le SDK n'est pas correctement désinitialisé, l'application peut se figer à la fermeture en raison de l'impossibilité de finaliser l'un de ses threads. Ce problème survient parce que le SDK continue à fonctionner, empêchant l'application de se fermer proprement. Pour garantir une sortie propre, il est essentiel de désinitialiser le SDK de manière appropriée selon le framework d'interface utilisateur que vous utilisez.

Pour les applications développées avec différents frameworks d'interface utilisateur, vous pouvez désinitialiser le SDK dans l'événement `FormClosing` ou dans un autre gestionnaire d'événements pertinent. Cette approche garantit que le SDK est correctement détruit avant la fermeture de l'application, permettant à tous les threads de se terminer correctement.

De plus, le SDK peut être détruit depuis n'importe quel thread, ce qui vous offre de la flexibilité dans la gestion du processus de désinitialisation. Pour améliorer l'expérience utilisateur et éviter le gel de l'interface pendant ce processus, vous pouvez utiliser des appels API asynchrones. En employant des méthodes async, vous permettez à la désinitialisation de se dérouler en arrière-plan, conservant ainsi une interface réactive et évitant tout ralentissement ou blocage potentiel.

L'application de ces bonnes pratiques garantit que votre application se ferme sans figement, offrant une expérience fluide aux utilisateurs. Une gestion correcte de la désinitialisation du SDK est essentielle pour maintenir la stabilité et les performances de votre application.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code.
