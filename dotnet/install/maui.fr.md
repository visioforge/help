---
title: Installer les SDK VisioForge .NET dans MAUI via NuGet
description: Ajoutez capture, lecture et édition vidéo dans des applis .NET MAUI. Configuration NuGet pour Windows, Android, iOS et macOS avec les contrôles VideoView.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - C++
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Playback
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - MediaPlayerCoreX

---

# Intégrer les SDK VisioForge à des applications .NET MAUI

## Vue d'ensemble

.NET Multi-platform App UI (MAUI) permet aux développeurs de créer des applications multiplateformes pour mobile et bureau à partir d'une seule base de code. VisioForge propose une prise en charge complète des applications MAUI via le paquet NuGet `VisioForge.DotNet.Core.UI.MAUI` (espace de noms : `VisioForge.Core.UI.MAUI`), qui contient des contrôles d'interface utilisateur spécialement conçus pour la plateforme .NET MAUI.

Nos SDK apportent de puissantes capacités multimédias sur toutes les plateformes prises en charge par MAUI :

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "Vous cherchez un tutoriel concret ?"
    Une fois les paquets installés, suivez le [Guide du lecteur vidéo .NET MAUI](../mediaplayer/guides/maui-player.md) — une implémentation complète de `VideoView` + `MediaPlayerCoreX` avec sélecteur de fichiers, recherche et volume, qui fonctionne sur iOS, Android, macOS et Windows à partir d'une seule base de code.

## Prise en main

### Installation

Pour commencer à utiliser VisioForge avec votre projet MAUI, installez les paquets NuGet requis :

1. Le paquet d'interface utilisateur principal : `VisioForge.DotNet.Core.UI.MAUI`
2. Le redistribuable spécifique à la plateforme (détaillé dans les sections par plateforme ci-dessous)

### Initialisation du SDK

Une initialisation correcte est essentielle pour que les SDK VisioForge fonctionnent correctement au sein de votre application MAUI. Ce processus doit être effectué dans votre fichier `MauiProgram.cs`.

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
          .UseMauiApp<App>()
          // Initialiser le paquet SkiaSharp en ajoutant la ligne de code ci-dessous
          .UseSkiaSharp()
          // Initialiser le paquet VisioForge MAUI en ajoutant la ligne de code ci-dessous
          .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
          // Après avoir initialisé le paquet VisioForge MAUI, vous pouvez éventuellement ajouter des polices supplémentaires
          .ConfigureFonts(fonts =>
          {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });

        // Continuez à initialiser votre application .NET MAUI ici
        return builder.Build();
    }
}
```

## Utiliser les contrôles VisioForge en XAML

Le contrôle `VideoView` est l'interface principale pour afficher du contenu vidéo dans votre application MAUI. Pour utiliser les contrôles VisioForge dans vos fichiers XAML :

1. Ajoutez l'espace de noms VisioForge à votre fichier XAML :

```xaml
xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
```

2. Ajoutez le contrôle VideoView à votre disposition :

```xaml
<vf:VideoView Grid.Row="0"               
                HorizontalOptions="FillAndExpand"
                VerticalOptions="FillAndExpand"
                x:Name="videoView"
                Background="Black"/>
```

Le contrôle VideoView s'adapte aux capacités de rendu natives de chaque plateforme tout en exposant une API cohérente pour votre code applicatif.

## Configuration spécifique à la plateforme

### Implémentation Android

Android nécessite des étapes de configuration supplémentaires pour fonctionner correctement :

#### 1. Ajouter la bibliothèque de bindings Java

Le SDK VisioForge s'appuie sur des fonctionnalités natives d'Android qui requièrent une bibliothèque personnalisée de bindings Java :

1. Clonez la bibliothèque de bindings depuis notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency)
2. Ajoutez le projet approprié à votre solution :
   - Utilisez `VisioForge.Core.Android.X8.csproj` pour .NET 8
   - Utilisez `VisioForge.Core.Android.X9.csproj` pour .NET 9
3. Ajoutez la référence à votre fichier projet :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

#### 2. Ajouter le paquet redistribuable Android

Incluez le paquet redistribuable spécifique à Android :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.*" />
</ItemGroup>
```

#### 3. Permissions Android

Veillez à ce que votre AndroidManifest.xml inclue les permissions nécessaires pour accéder à la caméra, au microphone et au stockage selon les fonctionnalités de votre application. Les permissions couramment requises sont :

- `android.permission.CAMERA`
- `android.permission.RECORD_AUDIO`
- `android.permission.READ_EXTERNAL_STORAGE`
- `android.permission.WRITE_EXTERNAL_STORAGE`

### Configuration iOS

L'intégration iOS demande moins d'étapes mais comporte quelques considérations importantes :

#### 1. Ajouter le redistribuable iOS

Ajoutez le paquet spécifique à iOS à votre projet :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2026.*" />
</ItemGroup>
```

#### 2. Notes importantes pour le développement iOS

- **Utilisez des appareils physiques** : le SDK requiert des tests sur des appareils iOS physiques plutôt que sur des simulateurs pour bénéficier de toutes les fonctionnalités.
- **Descriptions de confidentialité** : ajoutez les chaînes de description d'usage nécessaires dans votre fichier Info.plist pour l'accès à la caméra et au microphone :
  - `NSCameraUsageDescription`
  - `NSMicrophoneUsageDescription`

### Configuration macOS

Pour les applications macOS Catalyst :

#### 1. Configurer les identifiants de runtime

Pour garantir le bon fonctionnement de votre application sur les Mac Intel et Apple Silicon, spécifiez les identifiants de runtime appropriés :

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

#### 2. Activer le trimming

Pour des performances optimales sur macOS, activez l'option PublishTrimmed :

```xml
<PublishTrimmed Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">true</PublishTrimmed>
```

Pour des informations plus détaillées sur le déploiement macOS, consultez notre page de documentation [macOS](../deployment-x/macOS.md).

### Configuration Windows

Pour les applications Windows, vous devez inclure plusieurs paquets redistribuables :

#### 1. Ajouter les redistribuables Windows de base

Incluez les paquets Windows principaux :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.*" />
</ItemGroup>
```

#### 2. Ajouter la prise en charge étendue des codecs (optionnel mais recommandé)

Pour une prise en charge étendue des formats multimédias, incluez le paquet libAV (FFMPEG) :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.*" />
</ItemGroup>
```

### Optimisation des performances

Pour des performances optimales sur toutes les plateformes :

1. Utilisez l'accélération matérielle lorsqu'elle est disponible
2. Adaptez la résolution vidéo en fonction des capacités de l'appareil cible
3. Tenez compte des contraintes mémoire des appareils mobiles lors du traitement de fichiers multimédias volumineux

## Résolution des problèmes courants

- **Affichage vidéo vide** : assurez-vous que les permissions appropriées sont accordées sur les plateformes mobiles
- **Codecs manquants** : vérifiez que tous les paquets redistribuables spécifiques à la plateforme sont correctement installés
- **Problèmes de performance** : vérifiez que l'accélération matérielle est activée lorsqu'elle est disponible
- **Erreurs de déploiement** : confirmez que les identifiants de runtime sont correctement spécifiés pour la plateforme cible

## Conclusion

Le SDK VisioForge fournit une solution complète pour ajouter de puissantes capacités multimédias à vos applications .NET MAUI. En suivant les instructions de configuration spécifiques à chaque plateforme et les bonnes pratiques décrites dans ce guide, vous pourrez créer des applications multiplateformes riches avec des fonctions vidéo et audio avancées.

Pour des exemples supplémentaires et du code d'exemple, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

## Étapes suivantes

- [Guide du lecteur vidéo .NET MAUI](../mediaplayer/guides/maui-player.md) — tutoriel `VideoView` complet avec recherche, volume et sélecteur de fichiers
- [Guide du lecteur Avalonia](../mediaplayer/guides/avalonia-player.md) — alternative multiplateforme orientée bureau (inclut Linux)
- [Guide du lecteur Android](../mediaplayer/guides/android-player.md) — détails de déploiement spécifiques à Android
