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

Tous les moteurs X (`VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX`, `MediaBlocksPipeline`) partagent une seule étape d'initialisation. Vous devez initialiser le SDK avant toute utilisation d'une classe du SDK, et le désinitialiser avant la sortie de l'application. Les noyaux DirectShow Windows uniquement (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) ne nécessitent aucune étape d'initialisation.

### Appel de InitSDKAsync

La méthode recommandée est asynchrone, afin qu'un démarrage à froid de GStreamer (résolution des bibliothèques natives, analyse du registre des plugins) ne fige pas le thread appelant :

```csharp
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

La forme bloquante est équivalente et peut être appelée depuis n'importe quel thread. Utilisez-la directement lorsque l'initialisation doit s'exécuter sur un thread précis (voir la note ci-dessous) :

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

`InitSDKAsync` déporte `InitSDK` sur un thread du pool via `Task.Run`. Les gestionnaires d'exceptions non gérées de GStreamer / GLib sont liés au thread qui a déclenché l'initialisation en premier ; si votre application dépend d'une initialisation sur un thread précis (par exemple pour un état GLib statique par thread), appelez `InitSDK()` directement sur ce thread.

### Désinitialisation

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

`DestroySDK` n'a pas de variante asynchrone et peut être appelé depuis n'importe quel thread.

Si le SDK n'est pas correctement désinitialisé, l'application peut se figer à la fermeture en raison de l'impossibilité de finaliser l'un de ses threads. Ce problème survient parce que le SDK continue à fonctionner, empêchant l'application de se fermer proprement. Pour garantir une sortie propre, il est essentiel de désinitialiser le SDK de manière appropriée selon le framework d'interface utilisateur que vous utilisez.

Pour les applications développées avec différents frameworks d'interface utilisateur, vous pouvez désinitialiser le SDK dans l'événement `FormClosing` ou dans un autre gestionnaire d'événements pertinent. Cette approche garantit que le SDK est correctement détruit avant la fermeture de l'application, permettant à tous les threads de se terminer correctement.

De plus, le SDK peut être détruit depuis n'importe quel thread, ce qui vous offre de la flexibilité dans la gestion du processus de désinitialisation. Comme `DestroySDK` est synchrone, si vous souhaitez garder l'interface réactive, appelez-le sur un thread en arrière-plan (par exemple via `Task.Run`) — il n'existe pas de surcharge asynchrone de `DestroySDK`.

L'application de ces bonnes pratiques garantit que votre application se ferme sans figement, offrant une expérience fluide aux utilisateurs. Une gestion correcte de la désinitialisation du SDK est essentielle pour maintenir la stabilité et les performances de votre application.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code.
