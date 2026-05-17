---
title: Déployer VisioForge .NET sur macOS — Intel et Apple Silicon
description: Déployez VisioForge .NET sur macOS — paquets NuGet, intégration NSView, publication App Store, bibliothèques natives x64 et ARM64.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - macOS
  - MAUI
  - GStreamer
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - VideoView
  - VideoViewGL

---

# Guide de déploiement Apple macOS

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Les puissants SDK .NET de VisioForge fournissent des capacités complètes de traitement multimédia pour les développeurs macOS. Que vous créiez des applications de capture vidéo, des lecteurs multimédias, des éditeurs vidéo ou des pipelines de traitement multimédia complexes, nos SDK offrent les outils dont vous avez besoin pour livrer des solutions de haute qualité sur les plateformes Apple.

Le SDK VisioForge fournit une prise en charge complète du développement d'applications macOS avec les technologies .NET. Vous pouvez tirer parti de ce SDK pour créer des applications robustes de traitement multimédia qui s'exécutent nativement sur macOS, avec une prise en charge à la fois des architectures Intel (x64) et Apple Silicon (ARM64).

Ce guide couvre tout ce que vous devez savoir pour configurer, paramétrer et déployer des applications pour les environnements macOS et MacCatalyst à l'aide du SDK VisioForge. Que vous créiez des applications macOS traditionnelles ou des solutions multiplateformes utilisant des frameworks comme MAUI ou Avalonia, ce document vous aidera à naviguer dans le processus d'installation et de déploiement.

## Configuration requise

Avant de commencer le processus d'installation et de déploiement, assurez-vous que votre environnement de développement satisfait aux exigences suivantes :

### Exigences matérielles

- Ordinateur Mac avec processeur Intel (x64) ou Apple Silicon (ARM64)
- Minimum 8 Go de RAM (16 Go recommandés pour le traitement vidéo)
- Espace disque suffisant pour les outils de développement et les ressources de l'application

### Exigences logicielles

- macOS 10.15 (Catalina) ou ultérieur (dernière version recommandée)
  - macOS Monterey (12.x)
  - macOS Ventura (13.x)
  - macOS Sonoma (14.x)
  - Versions ultérieures de macOS (avec mises à jour continues)
- Xcode 12 ou ultérieur avec les Command Line Tools installés
- SDK .NET 6.0 ou ultérieur
- Visual Studio for Mac ou JetBrains Rider (IDE recommandés)

Pour installer les Command Line Tools de Xcode, exécutez la commande suivante dans le Terminal :

```bash
xcode-select --install
```

## Prise en charge des architectures

Le SDK VisioForge pour macOS prend en charge les deux principales architectures de processeur :

### Prise en charge Intel (x64)

- Compatible avec tous les ordinateurs Mac à base Intel
- Utilise des bibliothèques natives x64 pour des performances optimales
- Prise en charge complète des fonctionnalités sur tous les composants du SDK

### Prise en charge Apple Silicon (ARM64)

- Prise en charge native des puces M1, M2 et plus récentes Apple Silicon
- Bibliothèques natives ARM64 optimisées pour des performances maximales
- Accélération matérielle exploitant le Neural Engine d'Apple lorsque cela est applicable

### Considérations sur les binaires universels

Lorsque vous ciblez les deux architectures, envisagez de produire des binaires universels qui incluent à la fois du code x64 et ARM64. Cette approche garantit que votre application s'exécute nativement sur chaque plateforme sans dépendre de la traduction Rosetta 2.

Pour des builds de binaires universels ciblant à la fois Intel et Apple Silicon :

```xml
<PropertyGroup>
  <RuntimeIdentifiers>osx-x64;osx-arm64</RuntimeIdentifiers>
  <UseHardenedRuntime>true</UseHardenedRuntime>
</PropertyGroup>
```

## Technologies fondamentales

Les SDK .NET VisioForge s'appuient sur plusieurs technologies clés pour offrir des capacités multimédias performantes sur macOS :

### Intégration GStreamer

Tous les SDK VisioForge utilisent GStreamer comme framework sous-jacent pour la lecture et l'encodage audio/vidéo. GStreamer offre :

- Traitement multimédia accéléré matériellement
- Large compatibilité avec les formats
- Pipeline de lecture optimisé
- Capacités d'encodage efficaces

Les composants GStreamer sont installés automatiquement par nos paquets redistribuables, ce qui élimine la nécessité d'une configuration manuelle.

## Installation et déploiement des paquets NuGet

La principale méthode de déploiement des composants du SDK VisioForge sur des applications macOS passe par des paquets NuGet. Ces paquets incluent toutes les bibliothèques managées et non managées requises pour votre application.

### Paquets NuGet essentiels

Pour les applications macOS natives, ajoutez ces paquets de base :

1. **Paquet SDK principal** (selon vos besoins) :
   - `VisioForge.DotNet.VideoCapture` pour les applications de capture par caméra
   - `VisioForge.DotNet.VideoEdit` pour les applications d'édition vidéo
   - `VisioForge.DotNet.MediaPlayer` pour les applications de lecture multimédia
   - `VisioForge.DotNet.MediaBlocks` pour les pipelines avancés de traitement multimédia

2. **Paquet UI** :
   - `VisioForge.DotNet.Core` inclut les contrôles UI spécifiques à Apple

3. **Redistribuable de plateforme** :
   - `VisioForge.CrossPlatform.Core.macOS` pour les bibliothèques natives et les dépendances

### Applications macOS

Pour les applications macOS standard ciblant le framework `netX.0-macos` (où X représente la version de .NET), utilisez le paquet NuGet suivant :

- [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS)

Ce paquet contient :

- Bibliothèques natives pour le traitement multimédia
- Composants GStreamer pour la lecture et l'encodage multimédias
- Assemblies d'interface pour l'intégration .NET
- Binaires x64 et ARM64

### Prise en main des projets macOS natifs

Pour commencer à développer des applications macOS natives avec les SDK VisioForge :

1. **Créez un nouveau projet macOS** dans l'IDE de votre choix (Visual Studio for Mac ou JetBrains Rider)
2. **Ajoutez les paquets NuGet requis** (comme indiqué ci-dessus)
3. **Configurez les paramètres du projet** pour votre architecture cible

## Applications MacCatalyst et MAUI

### Développement multiplateforme avec .NET MAUI

.NET Multi-platform App UI (MAUI) permet de développer des applications qui s'exécutent de manière transparente sur macOS, iOS, Android et Windows à partir d'un code source unique. VisioForge fournit une prise en charge complète du développement MAUI grâce à des paquets et contrôles spécialisés.

Pour les applications MacCatalyst (y compris les projets MAUI) ciblant le framework `netX.0-maccatalyst`, utilisez :

- [VisioForge.CrossPlatform.Core.macCatalyst](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macCatalyst)

### Configuration des paquets MAUI

Pour les projets MAUI ciblant macOS (via MacCatalyst), ajoutez ces paquets :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="15.10.11" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="15.10.11" />
</ItemGroup>
```

### Configuration d'un projet MAUI

1. **Initialisez le SDK dans MauiProgram.cs** :

```csharp
builder
  .UseMauiApp<App>()
  .UseSkiaSharp()
  .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

2. **Ajoutez le contrôle VideoView en XAML** :

```xml
xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"

<vf:VideoView Grid.Row="0"
              HorizontalOptions="FillAndExpand"
              VerticalOptions="FillAndExpand"
              x:Name="videoView"
              Background="Black"/>
```

Les applications MacCatalyst nécessitent une configuration supplémentaire pour s'assurer que les bibliothèques natives sont correctement incluses dans le bundle de l'application. Ajoutez la cible de build personnalisée suivante à votre fichier projet :

```xml
<Target Name="CopyNativeLibrariesToMonoBundle" AfterTargets="Build" Condition="$(TargetFramework.Contains('-maccatalyst'))">
    <Message Text="Starting CopyNativeLibrariesToMonoBundle target..." Importance="High"/>

    <PropertyGroup>
        <AppBundleDir>$(OutputPath)$(AssemblyName).app</AppBundleDir>
        <MonoBundleDir>$(AppBundleDir)/Contents/MonoBundle</MonoBundleDir>
    </PropertyGroup>

    <Message Text="AppBundleDir: $(AppBundleDir)" Importance="High"/>
    <Message Text="MonoBundleDir: $(MonoBundleDir)" Importance="High"/>

    <MakeDir Directories="$(MonoBundleDir)" Condition="!Exists('$(MonoBundleDir)')"/>

    <Copy SourceFiles="@(None-&gt;'%(FullPath)')" DestinationFolder="$(MonoBundleDir)" Condition="'%(Extension)' == '.dylib' Or '%(Extension)' == '.so'">
        <Output TaskParameter="CopiedFiles" ItemName="CopiedNativeFiles"/>
    </Copy>

    <Message Text="Copied native files:" Importance="High" Condition="@(CopiedNativeFiles) != ''"/>
    <Message Text=" - %(CopiedNativeFiles.Identity)" Importance="High" Condition="@(CopiedNativeFiles) != ''"/>

    <Message Text="Finished CopyNativeLibrariesToMonoBundle target." Importance="High"/>
</Target>
```

Cette cible accomplit plusieurs tâches cruciales :

1. Identifie le répertoire du bundle d'application
2. Crée le répertoire MonoBundle s'il n'existe pas
3. Copie toutes les bibliothèques natives `.dylib` et `.so` vers le répertoire MonoBundle
4. Émet des informations de diagnostic à des fins de dépannage

Pour les détails complets sur l'intégration MAUI, consultez notre page dédiée [MAUI](../install/maui.md).

## Options de framework UI

Le SDK VisioForge prend en charge plusieurs frameworks UI pour le développement macOS :

### UI macOS native

Pour les applications macOS traditionnelles, le SDK fournit des contrôles `VideoViewGL` qui s'intègrent au framework AppKit natif. Ces contrôles offrent un rendu vidéo hautes performances à l'aide d'OpenGL.

### MAUI

Pour les applications MAUI multiplateformes, utilisez le paquet [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI), qui fournit des vues vidéo compatibles MAUI.

### Avalonia

Pour les applications Avalonia UI, le paquet [VisioForge.DotNet.Core.UI.Avalonia](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.Avalonia) offre des contrôles vidéo compatibles Avalonia.

## Configuration de l'environnement de développement

### Intégration JetBrains Rider

JetBrains Rider offre une excellente expérience de développement pour les applications macOS et iOS utilisant les SDK VisioForge :

1. Créez un nouveau projet dans Rider ciblant macOS ou iOS
2. Ajoutez les paquets NuGet requis via le gestionnaire de paquets
3. Configurez les paramètres du projet pour votre plateforme cible
4. Ajoutez les contrôles UI et implémentez les fonctionnalités du SDK

Pour des instructions détaillées sur la configuration de Rider, consultez notre [guide d'intégration Rider](../install/rider.md).

### Configuration de Visual Studio for Mac

Malgré son retrait, Visual Studio for Mac fonctionne encore pour développer des applications macOS et iOS avec les SDK VisioForge :

1. Créez un nouveau projet dans Visual Studio for Mac
2. Ajoutez des paquets NuGet via le gestionnaire de paquets NuGet
3. Configurez les paramètres de build nécessaires
4. Ajoutez les contrôles UI à l'interface de votre application

Pour des instructions détaillées sur Visual Studio for Mac, consultez notre [guide Visual Studio for Mac](../install/visual-studio-mac.md).

## Initialisation et nettoyage du SDK

Les moteurs X du SDK VisioForge requièrent une initialisation et un nettoyage explicites pour gérer correctement les ressources :

```csharp
// Initialiser le SDK au démarrage de l'application
VisioForge.Core.VisioForgeX.InitSDK();

// Utiliser les composants du SDK...

// Nettoyer les ressources avant la sortie de l'application
VisioForge.Core.VisioForgeX.DestroySDK();
```

Pour une initialisation et un nettoyage asynchrones, utilisez les variantes async :

```csharp
// Initialisation asynchrone
await VisioForge.Core.VisioForgeX.InitSDKAsync();

// Nettoyage asynchrone
await VisioForge.Core.VisioForgeX.DestroySDKAsync();
```

## Dépannage des problèmes courants

### Échecs de chargement des bibliothèques natives

Si votre application ne parvient pas à charger les bibliothèques natives :

1. Vérifiez que tous les paquets NuGet requis sont correctement installés
2. Contrôlez la structure du bundle d'application pour vous assurer que les bibliothèques sont au bon emplacement
3. Utilisez les commandes `dtruss` ou `otool` pour diagnostiquer les problèmes de chargement des bibliothèques
4. Assurez-vous que les Command Line Tools de Xcode sont correctement installés

### Problèmes spécifiques à MacCatalyst

Pour les problèmes de déploiement MacCatalyst :

1. Vérifiez que la cible `CopyNativeLibrariesToMonoBundle` est correctement implémentée
2. Vérifiez que le répertoire MonoBundle contient toutes les bibliothèques natives nécessaires
3. Assurez-vous que l'application dispose des entitlements appropriés pour l'accès aux médias

### Optimisation des performances

Pour des performances optimales :

1. Activez l'accélération matérielle lorsqu'elle est disponible
2. Ajustez la résolution vidéo en fonction des capacités de l'appareil
3. Fermez et libérez les objets du SDK lorsqu'ils ne sont plus nécessaires

## Ressources supplémentaires

Pour des exemples de code, des projets d'exemple et davantage de ressources techniques :

- Visitez le [dépôt GitHub VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir des exemples de code
- Rejoignez la communauté des développeurs VisioForge pour du support et des discussions

Notre dépôt d'exemples contient des exemples complets illustrant :

- La capture vidéo à partir de caméras
- Les implémentations de lecture multimédia
- Les flux de travail d'édition vidéo
- Les pipelines avancés de traitement multimédia

## Conclusion

Les SDK .NET VisioForge fournissent de puissantes capacités multimédias aux développeurs macOS et iOS, permettant la création d'applications multimédias sophistiquées. En suivant ce guide d'installation et de déploiement, vous avez établi les bases pour créer des applications multimédias hautes performances sur les plateformes Apple.

Pour toute question supplémentaire ou besoin de support, veuillez contacter notre équipe de support technique ou consulter nos forums pour obtenir de l'aide de la communauté.

---
*Cette documentation est régulièrement mise à jour pour refléter les dernières fonctionnalités du SDK et les informations de compatibilité.*
