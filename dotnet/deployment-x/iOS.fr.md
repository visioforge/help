---
title: Guide de déploiement iOS pour les SDK vidéo .NET VisioForge
description: Déployez du .NET sur iOS avec le SDK VisioForge — permissions, architectures et bonnes pratiques de déploiement multiplateforme.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCoreX
  - iOS
  - MAUI
  - Capture
  - Encoding
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - VideoView
  - IVideoView
  - VideoCaptureCoreX

---

# Guide de déploiement Apple iOS

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce guide complet vous accompagne tout au long du processus de déploiement d'applications propulsées par le SDK VisioForge sur des appareils Apple iOS. Le SDK VisioForge fournit un framework puissant pour créer des applications riches en médias sur iOS, offrant une prise en charge robuste de la capture vidéo, de l'édition, de la lecture et des capacités de traitement.

Le processus de déploiement iOS implique plusieurs considérations clés, de la gestion des paquets à la gestion des permissions, en passant par l'optimisation des performances. Ce document vous guidera à travers chaque étape pour garantir une expérience de déploiement fluide.

## Configuration requise

Avant de commencer votre processus de déploiement iOS, assurez-vous que votre environnement de développement satisfait aux exigences suivantes :

### Exigences matérielles

- Ordinateur Apple Mac pour le développement (requis pour la signature des applications iOS)
- Appareil iOS pour les tests (fortement recommandé par rapport aux simulateurs)
- Espace de stockage suffisant pour les outils de développement et les ressources de l'application

### Exigences logicielles

- Appareil Apple iOS sous iOS 12 ou ultérieur (dernière version recommandée)
- Xcode 12 ou ultérieur avec le SDK iOS installé
- Compte Apple Developer (requis pour la signature et la distribution d'applications)
- Visual Studio for Mac, JetBrains Rider ou Visual Studio Code
- SDK .NET 7.0 ou ultérieur (nous recommandons la dernière version stable)

## Prise en charge des architectures

Le SDK VisioForge pour iOS fournit une prise en charge native des deux principales architectures d'appareils iOS :

### Prise en charge ARM64

- Compatible avec tous les appareils iOS modernes (iPhone X et plus récents)
- Bibliothèques natives optimisées pour des performances maximales
- Traitement vidéo accéléré matériellement lorsque cela est pris en charge par l'appareil

## Processus d'installation

Suivez ces étapes pour configurer et déployer correctement votre application iOS propulsée par VisioForge :

1. Installez le SDK .NET pour le développement iOS
2. Créez un nouveau projet iOS dans l'IDE de votre choix (Visual Studio for Mac ou JetBrains Rider recommandés)
3. Ajoutez les paquets NuGet requis à votre projet (détaillés dans la section suivante)
4. Configurez les permissions et les entitlements nécessaires dans le fichier Info.plist de votre application
5. Implémentez la logique de votre application en utilisant les composants du SDK VisioForge
6. Compilez, signez et déployez votre application sur des appareils de test

## Paquets NuGet

Le SDK VisioForge pour iOS est distribué via des paquets NuGet :

### Paquets principaux

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) — paquet principal contenant les classes principales et les contrôles UI, y compris les composants de lecture vidéo et d'affichage. Il est indépendant de la plateforme et peut être utilisé dans n'importe quel projet .NET.

### Paquets UI

Chaque paquet UI offre les mêmes contrôles VideoView mais avec des implémentations différentes selon la plateforme cible :

#### Plateforme cible .NET iOS

- [VisioForge.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) — contient les contrôles UI et toutes les classes principales pour la plateforme iOS.

#### Plateforme cible .NET MAUI

- [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI) — contient les contrôles UI pour la plateforme MAUI.

### Paquets redistribuables

- [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS) — contient les composants de redistribution principaux requis pour toute application iOS utilisant les technologies VisioForge.

Vous pouvez ajouter ces paquets via le gestionnaire de paquets NuGet de votre IDE ou en ajoutant ce qui suit à votre fichier projet (utilisez les dernières versions) :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2026.*" />
</ItemGroup>
```

Note : remplacez les numéros de version par les dernières versions disponibles.

## Permissions et entitlements requis

Les applications iOS exigent des permissions explicites pour accéder aux fonctionnalités de l'appareil telles que les caméras, les microphones et la photothèque. Configurez ces permissions dans le fichier Info.plist de votre application :

### Accès à la caméra

Requis pour la fonctionnalité de capture vidéo :

```xml
<key>NSCameraUsageDescription</key>
<string>This app requires camera access for video recording</string>
```

### Accès au microphone

Requis pour l'enregistrement audio :

```xml
<key>NSMicrophoneUsageDescription</key>
<string>This app requires microphone access for audio recording</string>
```

### Accès à la photothèque

Requis pour enregistrer des vidéos dans la photothèque de l'appareil :

```xml
<key>NSPhotoLibraryUsageDescription</key>
<string>This app requires access to the photo library to save videos</string>
```

### Exemple de configuration Info.plist

Voici un exemple complet de fichier Info.plist avec toutes les permissions nécessaires :

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>LSRequiresIPhoneOS</key>
    <true/>
    <key>UIDeviceFamily</key>
    <array>
        <integer>1</integer>
        <integer>2</integer>
    </array>
    <key>UIRequiredDeviceCapabilities</key>
    <array>
        <string>arm64</string>
    </array>
    <key>UISupportedInterfaceOrientations</key>
    <array>
        <string>UIInterfaceOrientationPortrait</string>
        <string>UIInterfaceOrientationLandscapeLeft</string>
        <string>UIInterfaceOrientationLandscapeRight</string>
    </array>
    <key>UISupportedInterfaceOrientations~ipad</key>
    <array>
        <string>UIInterfaceOrientationPortrait</string>
        <string>UIInterfaceOrientationPortraitUpsideDown</string>
        <string>UIInterfaceOrientationLandscapeLeft</string>
        <string>UIInterfaceOrientationLandscapeRight</string>
    </array>
    <key>XSAppIconAssets</key>
    <string>Assets.xcassets/appicon.appiconset</string>
    <key>NSCameraUsageDescription</key>
    <string>Camera access is required for video recording</string>
    <key>NSMicrophoneUsageDescription</key>
    <string>Microphone access is required for audio recording</string>
    <key>NSPhotoLibraryUsageDescription</key>
    <string>Photo library access is required to save videos</string>
</dict>
</plist>
```

## Gestion des permissions à l'exécution

En plus de déclarer les permissions dans votre fichier Info.plist, vous devez également les demander à l'exécution. Voici un exemple de demande des permissions caméra et microphone :

```csharp
using System.Diagnostics;
using Photos;

// Demander la permission de la caméra
private async Task RequestCameraPermissionAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        // Gérer le refus de permission
        Debug.WriteLine("Camera permission denied");
    }
}

// Demander la permission du microphone
private async Task RequestMicrophonePermissionAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Microphone>();
    if (status != PermissionStatus.Granted)
    {
        // Gérer le refus de permission
        Debug.WriteLine("Microphone permission denied");
    }
}

// Demander la permission de la photothèque (spécifique à iOS)
private void RequestPhotoLibraryPermission()
{
    PHPhotoLibrary.RequestAuthorization(status =>
    {
        if (status == PHAuthorizationStatus.Authorized)
        {
            Debug.WriteLine("Photo library access granted");
        }
        else
        {
            Debug.WriteLine("Photo library access denied");
        }
    });
}
```

## Initialisation du SDK

Initialisez correctement le SDK VisioForge dans le cycle de vie de votre application :

```csharp
// Dans votre AppDelegate ou code de démarrage d'application
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    // Initialiser le SDK VisioForge
    VisioForge.Core.VisioForgeX.InitSDK();
    
    // Votre autre code d'initialisation
    
    return true;
}

// Nettoyer à l'arrêt de l'application
public override void WillTerminate(UIApplication application)
{
    // Nettoyer les ressources du SDK VisioForge
    VisioForge.Core.VisioForgeX.DestroySDK();
    
    // Votre autre code de nettoyage
}
```

## Bonnes pratiques d'implémentation

### Utilisation des contrôles VideoView

Le SDK VisioForge fournit un contrôle `VideoView` pour afficher du contenu vidéo. Sur iOS, `VideoView` est une sous-classe de `MTKView` accélérée par Metal qui implémente `IVideoView` directement (utilisez `VideoViewGL` si vous avez besoin de la variante héritée `UIView` basée sur OpenGL) :

```csharp
// Créer une instance VideoView — VideoView implémente IVideoView, donc aucun cast/appel d'aide n'est nécessaire
var videoView = new VisioForge.Core.UI.Apple.VideoView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));
View.AddSubview(videoView);

// Passez videoView lui-même là où un IVideoView est attendu
var captureCore = new VideoCaptureCoreX(videoView);
```

Vous pouvez ajouter le VideoView via un storyboard ou en code.

### Gestion des ressources

Les appareils iOS disposent de ressources limitées par rapport aux ordinateurs de bureau. Suivez ces bonnes pratiques :

1. Libérez les ressources lorsqu'elles ne sont pas utilisées
2. Utilisez des paramètres de résolution plus faibles pour le traitement en temps réel
3. Implémentez une gestion appropriée du cycle de vie dans vos ViewControllers
4. Testez sur des appareils réels, pas seulement sur des simulateurs

## Tests et débogage

### Tests sur appareil physique

Bien que le simulateur iOS puisse être utile pour les tests d'interface de base, il présente des limitations importantes pour les applications multimédias :

- Le simulateur peut avoir des problèmes de performance lors de l'encodage vidéo à hautes résolutions
- La caméra et le microphone ne sont pas disponibles dans le simulateur
- Les fonctionnalités d'accélération matérielle peuvent ne pas être disponibles ou se comporter différemment

**Testez toujours votre application multimédia sur des appareils iOS physiques avant la publication.**

### Considérations courantes de performance

Lors du déploiement d'applications multimédias sur iOS, considérez ces facteurs de performance :

1. **Résolution et fréquence d'images :** abaissez ces paramètres pour de meilleures performances sur les appareils plus anciens
2. **Sélection de l'encodeur :** utilisez des encodeurs accélérés matériellement lorsqu'ils sont disponibles
3. **Gestion de la mémoire :** implémentez une libération correcte des grands objets et surveillez l'utilisation de la mémoire
4. **Impact sur la batterie :** le traitement multimédia est gourmand en énergie ; mettez en place des mesures d'économie d'énergie

## Dépannage des problèmes courants

### Refus de permissions

Si votre application ne peut pas accéder à la caméra ou au microphone :

1. Vérifiez que toutes les permissions requises sont présentes dans votre Info.plist
2. Vérifiez que vous demandez les permissions à l'exécution avant de tenter d'utiliser le matériel
3. Vérifiez si l'utilisateur a manuellement refusé les permissions dans les Réglages iOS

### Erreurs de chargement de bibliothèques

Si vous rencontrez des erreurs lors du chargement de bibliothèques natives :

1. Vérifiez que tous les paquets NuGet requis sont correctement installés
2. Recherchez d'éventuels conflits de versions de paquets
3. Assurez-vous de cibler la bonne architecture iOS (ARM64)

## Ressources supplémentaires

- Visitez le [dépôt GitHub VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code et des projets d'exemple
- Parcourez la [documentation de l'API VisioForge](https://api.visioforge.org/dotnet/api/index.html) pour une référence complète du SDK

---
En suivant ce guide de déploiement, vous devriez être en mesure de créer, configurer et déployer avec succès des applications propulsées par VisioForge sur des appareils iOS. Pour des questions spécifiques ou des besoins de configuration avancée, veuillez contacter le support technique de VisioForge.
