---
title: Entrée vidéo Crossbar — Composite, S-Video, HDMI en C# .NET
description: Basculez entre les entrées Composite, S-Video et HDMI sur les cartes de capture avec l'API Crossbar de VisioForge Video Capture SDK. Exemples C#.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Decoding
  - C#
  - NuGet

---

# Configuration de plusieurs entrées vidéo matérielles avec Crossbar

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction à la fonctionnalité Crossbar

De nombreux périphériques de capture vidéo professionnels comme les tuners TV, les cartes de capture et le matériel d'acquisition vidéo disposent de plusieurs connexions d'entrée physiques. Ces périphériques peuvent inclure divers types d'entrées :

- Entrées vidéo analogiques (Composite, S-Video)
- Entrées vidéo numériques (HDMI, DisplayPort)
- Entrées vidéo professionnelles (SDI, HD-SDI)
- Entrées de tuner TV (RF, Cable)

L'interface Crossbar permet à votre application de sélectionner par programmation entre ces différentes entrées matérielles et de router les signaux de manière appropriée.

## Guide d'implémentation

### Étape 1 : initialiser l'interface Crossbar

Tout d'abord, vous devez initialiser l'interface Crossbar pour votre périphérique de capture vidéo. Cela établit la connexion avec les capacités de sélection d'entrée du matériel.

```cs
// Initialiser l'interface Crossbar pour le périphérique de capture actuellement sélectionné.
// La méthode prend le nom du périphérique cible (à faire correspondre avec Video_CaptureDevices()),
// et retourne true si le périphérique expose un Crossbar DirectShow.
var deviceName = VideoCapture1.Video_CaptureDevice?.Name;
var crossBarFound = VideoCapture1.Video_CaptureDevice_CrossBar_Init(deviceName);

// Si crossBarFound vaut true, le périphérique prend en charge plusieurs entrées configurables
```

### Étape 2 : découvrir les options d'entrée disponibles

Après l'initialisation, vous pouvez récupérer toutes les entrées disponibles qui peuvent être connectées à la sortie spécifiée (généralement « Video Decoder »).

```cs
// Effacer tous les paramètres de connexion Crossbar existants
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections();

// Effacer les éléments précédents dans votre menu déroulant d'UI
cbCrossbarVideoInput.Items.Clear();

// Remplir le menu déroulant avec toutes les sources d'entrée disponibles connectables à "Video Decoder"
foreach (string inputSource in VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput("Video Decoder"))
{
    // Ajouter chaque source d'entrée disponible à votre élément de sélection d'UI
    cbCrossbarVideoInput.Items.Add(inputSource);
}
```

### Étape 3 : appliquer la configuration d'entrée sélectionnée

Lorsque l'utilisateur sélectionne sa source d'entrée souhaitée, appliquez cette configuration au périphérique en connectant l'entrée sélectionnée à la sortie « Video Decoder ».

```cs
// D'abord effacer toutes les connexions existantes pour garantir un état propre
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections(); 

// Connecter l'entrée sélectionnée (depuis le menu déroulant de l'UI) à la sortie "Video Decoder"
// Le dernier paramètre (true) active la connexion
VideoCapture1.Video_CaptureDevice_CrossBar_Connect(cbCrossbarVideoInput.Text, "Video Decoder", true);

// À ce stade, le périphérique utilisera l'entrée sélectionnée pour la capture vidéo
```

### Étape 4 : gestion des changements de connexion

Pour une expérience utilisateur optimale, envisagez d'implémenter des gestionnaires d'événements pour détecter les changements de sélection d'entrée par l'utilisateur et réappliquer la configuration en conséquence.

## Dépendances requises

Pour implémenter la fonctionnalité Crossbar, votre application doit inclure les redistribuables Video Capture appropriés :

- Paquets redistribuables Video Capture :
  - [Architecture x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Architecture x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Conseils de dépannage

- Tous les périphériques ne prennent pas en charge la fonctionnalité Crossbar — vérifiez la valeur de `crossBarFound` après initialisation
- Certains périphériques peuvent avoir des noms de sortie différents de « Video Decoder »
- Les changements peuvent ne pas prendre effet avant le démarrage de la session de capture

---
Visitez notre dépôt [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code supplémentaires et des exemples d'implémentation.
