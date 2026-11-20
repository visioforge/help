---
title: Installer les SDK VisioForge .NET — NuGet, Visual Studio
description: Installation par fichiers setup ou paquets NuGet. Frameworks cibles pour Windows, macOS, iOS, Android et Linux. Initialisation du SDK et dépendances natives.
sidebar_label: Installation
order: 21
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - VideoView
  - VideoCaptureCore
  - VideoCaptureCoreX

---

# Guide d'installation des SDK VisioForge .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge propose de puissants SDK multimédias pour les développeurs .NET qui permettent d'ajouter des fonctionnalités avancées de capture vidéo, d'édition, de lecture et de traitement multimédia à vos applications. Ce guide couvre tout ce qu'il faut savoir pour installer et configurer correctement nos SDK dans votre environnement de développement.

## SDK .NET disponibles

VisioForge fournit plusieurs SDK spécialisés pour répondre à différents besoins multimédias :

- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — pour capturer la vidéo depuis des caméras, enregistrer l'écran et diffuser en streaming
- [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) — pour l'édition vidéo, le traitement et la conversion de formats
- [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) — pour construire des pipelines de traitement multimédia personnalisés
- [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) — pour créer des lecteurs multimédias personnalisés dotés de fonctionnalités avancées

## Méthodes d'installation

Vous pouvez installer nos SDK selon deux méthodes principales :

### Avec les fichiers d'installation

La méthode d'installation par fichier setup est recommandée pour les environnements de développement Windows. Cette approche :

1. Installe automatiquement toutes les dépendances requises
2. Configure l'intégration avec Visual Studio
3. Inclut des projets d'exemple pour démarrer rapidement
4. Fournit la documentation et des ressources supplémentaires

Les fichiers d'installation peuvent être téléchargés depuis les pages produit de chaque SDK sur notre site web.

### Avec les paquets NuGet

Pour le développement multiplateforme ou les pipelines CI/CD, nos paquets NuGet offrent souplesse et intégration aisée.

Installez le paquet du SDK que vous avez acheté : chacun tire `VisioForge.DotNet.Core` en
dépendance, il n'y a donc jamais à référencer `Core` directement :

```powershell
Install-Package VisioForge.DotNet.VideoCapture   # Video Capture SDK .NET
Install-Package VisioForge.DotNet.VideoEdit      # Video Edit SDK .NET
Install-Package VisioForge.DotNet.MediaPlayer    # Media Player SDK .NET
Install-Package VisioForge.DotNet.MediaBlocks    # Media Blocks SDK .NET
```

Ces paquets ne contiennent que le SDK managé. Le moteur natif arrive dans un paquet redist distinct, que vous référencez à côté :

| Moteur | Plateforme | Paquet redist |
|--------|------------|---------------|
| Moteurs X (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`, `MediaBlocksPipeline`) | Windows | `VisioForge.CrossPlatform.Core.Windows.x64` (`.x86` ou `.arm64` pour les autres processeurs ; sur ARM64, la lecture MP3 nécessite aussi `VisioForge.CrossPlatform.Libav.Windows.arm64`, et NDI n'est pas disponible) |
| Moteurs X | macOS, Mac Catalyst | `VisioForge.CrossPlatform.Core.macOS` |
| Moteurs X | Android | `VisioForge.CrossPlatform.Core.Android` |
| Moteurs X | iOS | `VisioForge.CrossPlatform.Core.iOS` |
| Moteurs X | Linux | `VisioForge.CrossPlatform.Core.Linux.x64` ou `VisioForge.CrossPlatform.Core.Linux.arm64`, plus GStreamer 1.20+ de la distribution (1.22+ pour certaines fonctionnalités) |
| `VideoCaptureCore` (Windows uniquement) | Windows | `VisioForge.DotNet.Core.Redist.VideoCapture.x64` (`.x86` pour les applications 32 bits) |
| `MediaPlayerCore` (Windows uniquement) | Windows | `VisioForge.DotNet.Core.Redist.MediaPlayer.x64` et `VisioForge.DotNet.Core.Redist.LAV.x64`. LAV est le `Source_Mode` par défaut, les modes GPU et Blu-ray utilisent aussi LAV, tout comme `GetFileThumbnail` : n'omettez le paquet LAV que si l'application n'utilise aucun d'eux. Ajoutez `VisioForge.DotNet.Core.Redist.FFMPEG.x64` quand l'application utilise le mode FFMPEG, et `VisioForge.DotNet.Core.Redist.VLC.x64` pour `File_VLC` ou `HTTP_RTSP_VLC` (`.x86` de chaque paquet pour les applications 32 bits) |
| `VideoEditCore` (Windows uniquement) | Windows | `VisioForge.DotNet.Core.Redist.VideoEdit.x64` et `VisioForge.DotNet.Core.Redist.FFMPEG.x64` pour ouvrir les fichiers d'entrée MP4, MKV et autres (`.x86` pour les applications 32 bits) |

Par exemple, une application Windows x64 sur les moteurs X ajoute :

```powershell
Install-Package VisioForge.CrossPlatform.Core.Windows.x64
Install-Package VisioForge.CrossPlatform.Libav.Windows.x64   # recommandé : décodeurs et encodeurs FFmpeg (libav)
```

Certaines plateformes demandent une étape en plus du paquet : Android nécessite aussi le projet de liaisons Java ([déploiement Android](../deployment-x/Android.md)), et Mac Catalyst une cible de build qui copie les bibliothèques natives dans le bundle de l'application ([déploiement macOS](../deployment-x/macOS.md)). Avec les moteurs Windows uniquement, une application AnyCPU déploie les paquets x86 et x64. Leurs paquets de moteur, LAV et FFMPEG n'ont pas de build ARM64 : sous Windows on Arm, où une application AnyCPU s'exécute généralement en ARM64, compilez l'application en x64 ou utilisez les moteurs X. Les paquets facultatifs sont listés dans le [guide de déploiement](../deployment-x/index.md) pour les moteurs X, et sur les pages de déploiement de [Video Capture SDK .NET](../videocapture/deployment.md), [Media Player SDK .NET](../mediaplayer/deployment.md) et [Video Edit SDK .NET](../videoedit/deployment.md) pour les moteurs Windows uniquement.

Des paquets d'interface utilisateur supplémentaires peuvent être nécessaires selon la plateforme cible :

```powershell
Install-Package VisioForge.DotNet.Core.UI.MAUI
Install-Package VisioForge.DotNet.Core.UI.WinUI
Install-Package VisioForge.DotNet.Core.UI.Avalonia
Install-Package VisioForge.DotNet.Core.UI.Uno
```

WPF et Windows Forms n'ont besoin d'aucun paquet UI : leur `VideoView` se trouve dans `VisioForge.Core`, qu'apporte chacun des paquets SDK ci-dessus. Les paquets distincts `VisioForge.DotNet.Core.UI.WPF` et `VisioForge.DotNet.Core.UI.WinForms` ne sont pas nécessaires.

## Intégration et configuration de l'IDE

Nos SDK s'intègrent parfaitement aux environnements de développement .NET les plus répandus :

### Intégration avec Visual Studio

[Visual Studio](visual-studio.md) offre l'expérience la plus complète avec nos SDK :

- Prise en charge complète d'IntelliSense pour les composants du SDK
- Débogage intégré pour les composants de traitement multimédia
- Prise en charge du concepteur pour les contrôles visuels
- Gestion des paquets NuGet

Pour des instructions détaillées de configuration de Visual Studio, consultez notre [guide d'intégration Visual Studio](visual-studio.md). Pour partir d'une application qui fonctionne, utilisez les [modèles de projet VisioForge](project-templates.md) pour Visual Studio 2026 et `dotnet new`.

### Intégration avec JetBrains Rider

[Rider](rider.md) offre une excellente prise en charge du développement multiplateforme :

- Complétion de code complète pour les API du SDK
- Fonctions de navigation intelligente pour explorer les classes du SDK
- Gestion intégrée des paquets NuGet
- Capacités de débogage multiplateforme

Pour les instructions spécifiques à Rider, consultez notre [documentation d'intégration Rider](rider.md).

### Visual Studio pour Mac

Les utilisateurs de [Visual Studio pour Mac](visual-studio-mac.md) peuvent développer des applications pour macOS, iOS et Android :

- Gestionnaire de paquets NuGet intégré pour installer les composants du SDK
- Modèles de projet pour une configuration rapide
- Outils de débogage intégrés

Pour en savoir plus, consultez notre [guide de configuration Visual Studio pour Mac](visual-studio-mac.md).

## Configuration spécifique à la plateforme

### Configuration du framework cible

Chaque système d'exploitation requiert des paramètres de framework cible spécifiques pour une compatibilité optimale :

#### Applications Windows

Les applications Windows doivent utiliser le suffixe de framework cible `-windows` :

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

Cela permet d'accéder aux API spécifiques à Windows et aux frameworks d'interface utilisateur comme WPF et Windows Forms.

#### Développement Android

Les projets Android nécessitent le suffixe de framework `-android` :

```xml
<TargetFramework>net10.0-android</TargetFramework>
```

Assurez-vous que les workloads Android sont installées dans votre environnement de développement :

```
dotnet workload install android
```

#### Développement iOS

Les applications iOS doivent utiliser le framework cible `-ios` :

```xml
<TargetFramework>net10.0-ios</TargetFramework>
```

Le développement iOS requiert un Mac avec Xcode installé, même lorsque vous utilisez Visual Studio sur Windows.

#### Applications macOS

Les applications natives macOS utilisent le framework `-macos` ou `-maccatalyst` :

```xml
<TargetFramework>net10.0-macos</TargetFramework>
```

Pour les applications .NET MAUI ciblant macOS, utilisez :

```xml
<TargetFramework>net10.0-maccatalyst</TargetFramework>
```

#### Développement Linux

Les applications Linux utilisent le framework cible standard sans suffixe de plateforme :

```xml
<TargetFramework>net10.0</TargetFramework>
```

Ajoutez `VisioForge.CrossPlatform.Core.Linux.x64`, ou `VisioForge.CrossPlatform.Core.Linux.arm64` sur ARM64. Il ne contient que les bibliothèques natives propres au SDK : installez GStreamer 1.20 ou ultérieur (1.22 ou ultérieur pour certaines fonctionnalités, comme les blocs OpenCV et le canal de retour audio ONVIF) depuis les paquets de la distribution, comme décrit dans le [déploiement sous Ubuntu](../deployment-x/Ubuntu.md).

## Prise en charge des frameworks spécialisés

### Applications .NET MAUI

Les [projets MAUI](maui.md) nécessitent une configuration spéciale :

- Ajoutez le paquet NuGet `VisioForge.DotNet.Core.UI.MAUI`
- Configurez les permissions spécifiques à la plateforme dans votre projet
- Utilisez les contrôles vidéo spécifiques à MAUI

Consultez notre [guide MAUI détaillé](maui.md) pour les instructions complètes.

### Framework Avalonia UI

Les [projets Avalonia](avalonia.md) proposent une alternative d'interface utilisateur multiplateforme :

- Installez le paquet `VisioForge.DotNet.Core.UI.Avalonia`
- Utilisez les contrôles de rendu vidéo spécifiques à Avalonia
- Configurez les dépendances spécifiques à la plateforme

Notre [guide d'intégration Avalonia](avalonia.md) fournit les instructions de configuration complètes.

### Applications Uno Platform

Les [projets Uno Platform](uno.md) permettent de créer des applications pour Windows, Android, iOS, macOS et Linux à partir d'une seule base de code :

- Ajoutez le paquet NuGet `VisioForge.DotNet.Core.UI.Uno`
- Utilisez les contrôles vidéo spécifiques à Uno Platform
- Configurez les redistribuables spécifiques à la plateforme

Consultez notre [guide Uno Platform](uno.md) pour les instructions de configuration complètes.

### Unity

L'intégration [Unity](unity.md) utilise un `.unitypackage` prêt à importer plutôt que NuGet :

- Autonome pour Unity 6 sur Windows x64 — SDK managé, runtime natif et scènes d'exemple
- Pas de compilation des sources, pas de NuGet, et aucune dépendance externe
- La vidéo s'affiche dans un `RawImage` Unity via les scripts fournis

Consultez notre [guide d'installation Unity](unity.md) pour la configuration pas à pas.

## Initialisation du SDK pour les moteurs multiplateformes

Nos SDK comprennent à la fois des moteurs DirectShow spécifiques à Windows (comme `VideoCaptureCore`) et des moteurs X multiplateformes (comme `VideoCaptureCoreX`). Les moteurs X nécessitent une initialisation et un nettoyage explicites.

### Initialisation du SDK

Avant d'utiliser un composant des moteurs X, initialisez le SDK :

```csharp
// Initialiser au démarrage de l'application
VisioForge.Core.VisioForgeX.InitSDK();

// Ou utiliser la version asynchrone
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

### Libération des ressources

Lorsque votre application se termine, libérez correctement les ressources :

```csharp
// Nettoyer à la sortie de l'application (synchrone — DestroySDK n'a pas de variante asynchrone)
VisioForge.Core.VisioForgeX.DestroySDK();
```

Une mauvaise initialisation ou un nettoyage incorrect peut provoquer des fuites mémoire ou un comportement instable.

## Contrôles de rendu vidéo

Chaque framework d'interface utilisateur requiert des contrôles vidéo spécifiques pour afficher le contenu multimédia :

### Windows Forms

```csharp
// En haut de Form1.cs. Le VideoView WinForms se trouve dans VisioForge.Core : aucun paquet supplémentaire
using VisioForge.Core.UI.WinForms;

// Dans la classe Form1 : un champ, ajouté au formulaire dans le constructeur existant
private readonly VideoView videoView = new VideoView { Dock = DockStyle.Fill };

public Form1()
{
    InitializeComponent();
    Controls.Add(videoView); // ajoutez cette ligne au constructeur existant
}
```

### Applications WPF

```xml
<!-- MainWindow.xaml. Le VideoView WPF se trouve dans VisioForge.Core : aucun paquet supplémentaire -->
<Window ...
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core">
    <vf:VideoView x:Name="videoView" />
</Window>
```

### Applications MAUI

```csharp
// Ajouter une référence à VisioForge.DotNet.Core.UI.MAUI
using VisioForge.Core.UI.MAUI;

// Dans votre XAML
<vf:VideoView x:Name="videoView" />
```

### Avalonia UI

```csharp
// Ajouter une référence à VisioForge.DotNet.Core.UI.Avalonia
using VisioForge.Core.UI.Avalonia;

// Dans votre XAML
<vf:VideoView Name="videoView" />
```

## Gestion des dépendances natives

Nos SDK s'appuient sur des bibliothèques natives pour des performances optimales. Ces dépendances doivent être correctement gérées lors du déploiement :

- Windows : le programme d'installation les installe sur la machine de développement. L'application que vous livrez utilise le paquet redist de son moteur et de son processeur dans le [tableau ci-dessus](#avec-les-paquets-nuget) ; ses bibliothèques sont copiées à côté de l'application à la compilation
- macOS/iOS : `VisioForge.CrossPlatform.Core.macOS` et `VisioForge.CrossPlatform.Core.iOS` contiennent les bibliothèques natives, GStreamer compris ; l'application nécessite toujours une signature correcte, et Mac Catalyst une cible de build (voir le [déploiement macOS](../deployment-x/macOS.md))
- Android : `VisioForge.CrossPlatform.Core.Android` contient les bibliothèques natives pour toutes les architectures Android ; l'application nécessite aussi le projet de liaisons Java (voir le [déploiement Android](../deployment-x/Android.md))
- Linux : `VisioForge.CrossPlatform.Core.Linux.x64` ou `.arm64` ne contient que les bibliothèques natives propres au SDK ; GStreamer vient des paquets de la distribution (voir le [déploiement sous Ubuntu](../deployment-x/Ubuntu.md))

Pour des instructions détaillées de déploiement, consultez notre [guide de déploiement](../deployment-x/index.md).

## Résolution des problèmes d'installation courants

Si vous rencontrez des problèmes lors de l'installation :

1. Vérifiez la compatibilité du framework cible avec votre type de projet
2. Assurez-vous que toutes les workloads requises sont installées (`dotnet workload list`)
3. Recherchez des conflits de dépendances dans votre projet
4. Confirmez la bonne initialisation du SDK pour les moteurs X
5. Examinez les exigences spécifiques à la plateforme dans notre documentation

## Exemples de code et ressources

Nous maintenons une vaste collection d'applications d'exemple sur notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour vous aider à démarrer rapidement avec nos SDK.

Ces exemples couvrent des scénarios courants comme :

- La capture vidéo depuis des caméras et des écrans
- La lecture multimédia avec des contrôles personnalisés
- L'édition et le traitement vidéo
- Le développement multiplateforme

Consultez notre dépôt pour les derniers exemples de code et les bonnes pratiques d'utilisation de nos SDK.

---

Pour toute assistance supplémentaire ou question, veuillez contacter notre équipe de support technique ou consulter notre portail de documentation.
