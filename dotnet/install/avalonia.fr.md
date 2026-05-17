---
title: Installer les SDK VisioForge dans des applis Avalonia .NET
description: Créez des applications Avalonia multiplateformes avec des capacités multimédias pour Windows, macOS, Linux, Android et iOS grâce aux SDK vidéo VisioForge.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - C++
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - C#
  - NuGet
primary_api_classes:
  - VideoView

---

# Créer des applications Avalonia riches en médias avec VisioForge

## Vue d'ensemble du framework

Avalonia UI se distingue comme un framework d'interface utilisateur .NET polyvalent et véritablement multiplateforme, avec une prise en charge couvrant les environnements de bureau (Windows, macOS, Linux) et les plateformes mobiles (iOS et Android). VisioForge enrichit cet écosystème via le paquet spécialisé `VisioForge.DotNet.Core.UI.Avalonia`, qui fournit des contrôles multimédias hautes performances adaptés à l'architecture d'Avalonia.

Notre suite de SDK donne aux développeurs Avalonia accès à de larges capacités multimédias :

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Installation et configuration

### Installation des paquets essentiels

La création d'une application Avalonia dotée des capacités multimédias de VisioForge nécessite l'installation de plusieurs composants NuGet clés :

1. Couche d'interface utilisateur spécifique à Avalonia : `VisioForge.DotNet.Core.UI.Avalonia`
2. Paquet de fonctionnalité principal : `VisioForge.DotNet.Core` (ou variante de SDK spécialisée)
3. Bindings natifs spécifiques à chaque plateforme (détaillés dans les sections suivantes)

Ajoutez-les au manifeste de votre projet (`.csproj`) :

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
  <PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />
  <!-- Les paquets spécifiques à la plateforme seront ajoutés dans des ItemGroups conditionnels -->
</ItemGroup>
```

### Architecture d'initialisation Avalonia

Un avantage clé de l'intégration de VisioForge avec Avalonia réside dans son modèle d'initialisation fluide. Contrairement à certains frameworks qui exigent une configuration globale explicite, les contrôles Avalonia deviennent disponibles dès que le paquet principal est référencé.

Votre code d'amorçage Avalonia standard dans `Program.cs` reste inchangé :

```csharp
using Avalonia;
using System;

namespace YourAppNamespace;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
```

### Implémentation du composant VideoView

Le contrôle `VideoView` sert d'élément central de rendu. Intégrez-le dans vos fichiers `.axaml` de la manière suivante :

1. Tout d'abord, déclarez l'espace de noms VisioForge :

```xml
xmlns:vf="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"
```

2. Ensuite, implémentez le contrôle dans la structure de votre disposition :

```xml
<vf:VideoView 
    Grid.Row="0"               
    HorizontalAlignment="Stretch"
    VerticalAlignment="Stretch"
    x:Name="videoView"
    Background="Black"/>
```

Ce contrôle s'adapte automatiquement au pipeline de rendu spécifique à chaque plateforme tout en exposant une surface d'API cohérente.

## Intégration aux plateformes de bureau

### Guide d'implémentation Windows

Le déploiement sur Windows requiert des composants natifs spécifiques, fournis sous forme de références NuGet.

#### Composants Windows principaux

Ajoutez les paquets spécifiques à Windows suivants à votre projet de bureau :

```xml
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.*" />
</ItemGroup>
```

#### Prise en charge avancée des formats multimédias

Pour une compatibilité étendue avec les codecs, incluez la variante UPX optimisée en taille des bibliothèques libAV :

```xml
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.*" />
</ItemGroup>
```

La variante UPX offre une optimisation significative de la taille tout en conservant une compatibilité complète avec les codecs.

### Intégration macOS

Pour le déploiement sur macOS :

#### Paquet de bindings natifs

Incluez les composants natifs spécifiques à macOS :

```xml
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2026.*" />
</ItemGroup>
```

#### Configuration du framework

Configurez votre projet avec la cible de framework macOS appropriée :

```xml
<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <TargetFramework>net8.0-macos14.0</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

### Déploiement Linux

La prise en charge de Linux inclut :

#### Configuration du framework

Définissez le framework cible approprié pour les environnements Linux :

```xml
<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Linux'))">
  <TargetFramework>net8.0</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

#### Dépendances système

Pour le déploiement sous Linux, assurez-vous que les bibliothèques système requises sont disponibles sur le système cible. Contrairement à Windows et macOS qui utilisent des paquets NuGet, Linux peut exiger des dépendances au niveau système. Consultez la documentation Linux de VisioForge pour connaître les exigences spécifiques à la plateforme.

## Développement mobile

### Configuration Android

L'implémentation Android nécessite des étapes supplémentaires propres au modèle d'intégration d'Avalonia avec Android :

#### Couche d'interopérabilité Java

L'implémentation Android de VisioForge requiert un pont de bindings entre .NET et les API natives d'Android :

1. Obtenez le projet de bindings Java depuis le [dépôt d'exemples VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) dans le répertoire `AndroidDependency`
2. Ajoutez le projet de bindings approprié à votre solution :
   - Utilisez `VisioForge.Core.Android.X8.csproj` pour les applications .NET 8
3. Référencez ce projet dans votre projet principal Android :

```xml
<ItemGroup>
  <ProjectReference Include="..\..\path\to\VisioForge.Core.Android.X8.csproj" />
</ItemGroup>
```

#### Paquet spécifique à Android

Ajoutez le paquet redistribuable Android :

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.*" />
</ItemGroup>
```

#### Permissions d'exécution

Configurez `AndroidManifest.xml` avec les permissions appropriées :

- `android.permission.CAMERA`
- `android.permission.RECORD_AUDIO`
- `android.permission.READ_EXTERNAL_STORAGE`
- `android.permission.WRITE_EXTERNAL_STORAGE`
- `android.permission.INTERNET`

### Développement iOS

L'intégration iOS avec Avalonia requiert :

#### Composants natifs

Ajoutez le redistribuable spécifique à iOS à votre projet principal iOS :

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2026.*" />
</ItemGroup>
```

#### Notes d'implémentation importantes

- Les tests sur appareil physique sont essentiels, car la prise en charge du simulateur est limitée
- Mettez à jour votre `Info.plist` avec les descriptions de confidentialité :
  - `NSCameraUsageDescription` pour l'accès à la caméra
  - `NSMicrophoneUsageDescription` pour l'enregistrement audio

## Optimisation des performances

Maximisez les performances de votre application grâce à ces optimisations spécifiques à Avalonia :

1. Activez l'accélération matérielle lorsqu'elle est prise en charge par la plateforme sous-jacente
2. Mettez en œuvre une mise à l'échelle adaptative de la résolution en fonction des capacités de l'appareil
3. Optimisez les schémas d'utilisation de la mémoire, en particulier pour les cibles mobiles
4. Tirez parti efficacement du modèle de composition d'Avalonia en minimisant la complexité de l'arbre visuel autour du `VideoView`

## Guide de dépannage

### Problèmes de format multimédia

- **Échecs de lecture** :
  - Vérifiez que tous les paquets de plateforme sont correctement référencés
  - Vérifiez la disponibilité du codec pour le format multimédia cible
  - Vérifiez les restrictions de format spécifiques à la plateforme

### Problèmes de performance

- **Lecture ou rendu lent** :
  - Activez l'accélération matérielle lorsqu'elle est disponible
  - Réduisez la résolution de traitement lorsque c'est approprié
  - Utilisez correctement le modèle de threading d'Avalonia

### Difficultés de déploiement

- **Erreurs d'exécution spécifiques à la plateforme** :
  - Validez les spécifications du framework cible
  - Vérifiez la disponibilité des dépendances natives
  - Assurez un provisionnement adéquat pour les cibles mobiles

## Architecture de projet multiplateforme

L'intégration de VisioForge avec Avalonia s'épanouit dans une structure de projet spécialisée à têtes multiples. L'exemple `SimplePlayerMVVM` illustre cette architecture :

- **Projet partagé principal** (`SimplePlayerMVVM.csproj`) : contient les vues multiplateformes, les view models et la logique partagée, avec un multi-ciblage conditionnel :

    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <Nullable>enable</Nullable>
        <LangVersion>latest</LangVersion>
        <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
      </PropertyGroup>
      <ItemGroup>
        <AvaloniaResource Include="Assets\**" />
      </ItemGroup>
      <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
        <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-windows</TargetFrameworks>
      </PropertyGroup>
      <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
        <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-macos14.0</TargetFrameworks>
      </PropertyGroup>
      <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Linux'))">
        <TargetFrameworks>net8.0-android;net8.0</TargetFrameworks>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Avalonia" Version="11.2.2" />
        <!-- Références Avalonia supplémentaires -->
      </ItemGroup>
      <ItemGroup>
        <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.*" />
        <PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
      </ItemGroup>
    </Project>
    ```

- **Projets principaux spécifiques à chaque plateforme** :
  - `SimplePlayerMVVM.Android.csproj` : contient la configuration spécifique à Android et les références de bindings
  - `SimplePlayerMVVM.iOS.csproj` : gère l'initialisation et les dépendances iOS
  - `SimplePlayerMVVM.Desktop.csproj` : gère la détection de la plateforme de bureau et le chargement des redistribuables appropriés

Pour des applications de bureau plus simples, `SimpleVideoCaptureA.csproj` propose un modèle allégé où la détection de la plateforme se fait au sein d'un seul fichier projet.

## Conclusion

L'intégration de VisioForge avec Avalonia propose une approche sophistiquée du développement multimédia multiplateforme qui exploite les avantages architecturaux propres à Avalonia. Grâce à des composants spécifiques à chaque plateforme soigneusement structurés et à une API unifiée, les développeurs peuvent créer des applications multimédias riches couvrant à la fois le bureau et le mobile, sans compromis sur les performances ni les capacités.

Pour des exemples de code complets et des applications de démonstration, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples), qui contient des démonstrations Avalonia spécialisées dans les sections Video Capture SDK X et Media Player SDK X.
