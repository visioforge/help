---
title: Déploiement Uno Platform pour le SDK VisioForge .NET
description: Déploiement Uno Platform du SDK VisioForge .NET avec intégration VideoView, prise en charge multiplateforme pour Windows, Android, iOS, macOS et Linux.
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
  - Uno
  - GStreamer
  - Streaming
  - Webcam
  - C#
  - NuGet

---

# Guide d'implémentation et de déploiement Uno Platform

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction aux SDK VisioForge pour Uno Platform

Uno Platform est un framework UI multiplateforme puissant qui permet aux développeurs de créer des applications natives pour Windows, Android, iOS, macOS et Linux à partir d'un code source unique. VisioForge fournit une prise en charge complète des applications Uno Platform via le paquet `VisioForge.DotNet.Core.UI.Uno`, qui contient des contrôles UI spécialisés conçus spécifiquement pour Uno Platform.

Le processus de déploiement Uno Platform requiert une attention particulière pour chaque plateforme cible. Ce document fournit des instructions détaillées pour garantir que votre application s'exécute correctement sur toutes les plateformes prises en charge.

## Plateformes prises en charge

Les SDK VisioForge prennent en charge les cibles Uno Platform suivantes :

| Plateforme | Framework cible | Statut |
|----------|------------------|--------|
| Windows Desktop | net10.0-windows10.0.19041.0 | &#x2714; Prise en charge complète |
| Android | net10.0-android | &#x2714; Prise en charge complète |
| iOS | net10.0-ios | &#x2714; Prise en charge complète |
| macOS Desktop (Skia) | net10.0-desktop | &#x2714; Prise en charge complète |
| Linux Desktop (Skia) | net10.0-desktop | &#x2714; Prise en charge complète |

Ces cibles sont celles d'Uno Platform 7, qui n'a plus de cible Mac Catalyst : sur macOS, l'application tourne sur la tête `net10.0-desktop`. Uno Platform 7 exige un package `VisioForge.DotNet.Core.UI.Uno` compilé pour lui ; les versions jusqu'à 2026.11.16 sont compilées pour Uno Platform 6.5.

## Configuration requise

Avant de commencer votre implémentation Uno Platform, assurez-vous que votre environnement de développement satisfait aux exigences suivantes :

### Exigences de l'environnement de développement

- Ordinateur Windows, Linux ou macOS
- Visual Studio 2022 avec l'extension Uno Platform, JetBrains Rider ou Visual Studio Code
- SDK .NET 10.0 ou ultérieur (dernière version stable recommandée)
- Modèles Uno Platform installés

### Exigences spécifiques à chaque plateforme

#### Windows
- Windows 10 version 17763 ou ultérieur
- Windows App SDK 2.5 ou ultérieur (fourni par Uno Platform 7)

#### Android
- SDK Android avec les niveaux d'API appropriés
- Appareil sous Android 7.0 (API 24) ou ultérieur
- Java Development Kit (JDK) 11 ou ultérieur

#### iOS/macOS
- Ordinateur Mac avec Xcode 26 ou ultérieur pour les builds iOS/macOS (Xcode 27.2 ou ultérieur pour .NET 11)
- Compte Apple Developer (pour le déploiement sur appareil)
- iOS 18.0 ou ultérieur / macOS 12 ou ultérieur

#### Linux
- Runtime GStreamer installé
- Serveur d'affichage X11 ou Wayland
- Le paquet NuGet `VisioForge.CrossPlatform.Core.Linux.x64` (ou `.arm64`)

## Processus d'installation et de configuration

Suivez ces étapes pour configurer et déployer correctement votre application Uno Platform propulsée par VisioForge :

### 1. Installer les modèles Uno Platform

```bash
dotnet new install Uno.Templates
```

### 2. Installer les charges de travail requises

```bash
# Pour Android
dotnet workload install android

# Pour iOS
dotnet workload install ios
```

### 3. Créer un nouveau projet Uno Platform

```bash
dotnet new unoapp -o MyMediaApp
```

### 4. Ajouter les paquets NuGet VisioForge

Ajoutez les paquets suivants à votre projet :

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="[2026.11.20]"/>
  <PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.20]"/>
</ItemGroup>
```

### Redistribuables spécifiques à chaque plateforme

Ajoutez les paquets redistribuables spécifiques à chaque plateforme à votre projet :

#### Windows

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.16]"/>
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.16]"/>
</ItemGroup>
```

#### Android

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="[2026.11.5]"/>
</ItemGroup>
```

Vous devez également ajouter la bibliothèque de bindings Java. Clonez-la depuis notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency) et ajoutez une référence :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

#### iOS

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="[2026.11.5]"/>
</ItemGroup>
```

#### macOS

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-desktop')) and $([MSBuild]::IsOSPlatform('osx'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="[2026.11.13]"/>
</ItemGroup>
```

Le package s'applique à la tête desktop lorsque le projet est compilé sur un Mac. Ses cibles de build copient les bibliothèques natives à côté de l'application.

#### Linux Desktop

Pour Linux, vous devez installer le runtime GStreamer sur votre système :

```bash
# Ubuntu/Debian
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav

# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free
```

Ajoutez ensuite le paquet core de l'architecture cible. Il contient les bibliothèques natives propres au SDK (la conversion de couleur utilisée par les captures d'image, la conversion d'images vidéo en bitmaps et les superpositions, l'effet LUT 3D, Data Matrix) et aucun GStreamer. Sans lui, l'application démarre, mais ces fonctions échouent à l'exécution avec `VisioForge_VAP is not available` :

```xml
<ItemGroup Condition="$([MSBuild]::IsOSPlatform('linux'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.arm64" Version="[2026.10.26]" Condition="$(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64')))"/>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]" Condition="!($(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64'))))"/>
</ItemGroup>
```

Le bloc s'applique quand le projet est compilé sous Linux ; une publication pour Linux depuis Windows ou macOS l'ignore. Sous Linux, les conditions choisissent le paquet d'après l'identificateur de runtime : `-r linux-arm64` ou un profil de publication ARM64 sélectionne le paquet ARM64, et sans identificateur de runtime, c'est l'architecture de la machine de build qui décide.

### Exemple complet de fichier projet

Voici un exemple complet de fichier `.csproj` pour une application Uno Platform avec le SDK VisioForge :

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <!-- Frameworks cibles selon le système d'exploitation de build -->
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-desktop;net10.0-ios;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('linux'))">net10.0-desktop</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <UnoSingleProject>true</UnoSingleProject>
    <UseCurrentXcodeSDKVersion>true</UseCurrentXcodeSDKVersion>
    
    <!-- Paramètres de l'application -->
    <ApplicationTitle>MyMediaApp</ApplicationTitle>
    <ApplicationId>com.yourcompany.mymediaapp</ApplicationId>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationPublisher>Your Company</ApplicationPublisher>
    <Description>Media application powered by Uno Platform and VisioForge.</Description>
    
    <UnoFeatures></UnoFeatures>
  </PropertyGroup>
  
  <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
    <CodesignKey>Apple Development</CodesignKey>
  </PropertyGroup>
  
  <!-- Références principales VisioForge -->
  <ItemGroup>
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="[2026.11.20]"/>
    <PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.20]"/>
  </ItemGroup>
  
  <!-- Plateforme Windows -->
  <ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.16]"/>
    <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.16]"/>
  </ItemGroup>
  
  <!-- Plateforme Android -->
  <ItemGroup Condition="$(TargetFramework.Contains('-android'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="[2026.11.5]"/>
    <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
  </ItemGroup>
  
  <!-- Plateforme iOS -->
  <ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="[2026.11.5]"/>
  </ItemGroup>
  
  <!-- Plateforme macOS Desktop -->
  <ItemGroup Condition="$(TargetFramework.Contains('-desktop')) and $([MSBuild]::IsOSPlatform('osx'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="[2026.11.13]"/>
  </ItemGroup>
  
  <!-- Plateforme Linux Desktop -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('linux'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.arm64" Version="[2026.10.26]" Condition="$(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64')))"/>
    <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]" Condition="!($(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64'))))"/>
  </ItemGroup>
</Project>
```

## Configuration spécifique à chaque plateforme

### Configuration Windows

Les applications Windows utilisent le rendu WinUI 3 natif et prennent en charge l'accélération matérielle via DirectX.

#### Capacités requises

Ajoutez les capacités requises à votre `Package.appxmanifest` :

```xml
<Capabilities>
    <Capability Name="internetClient" />
    <uap:Capability Name="videosLibrary" />
    <uap:Capability Name="musicLibrary" />
    <DeviceCapability Name="microphone" />
    <DeviceCapability Name="webcam" />
</Capabilities>
```

### Configuration Android

#### Permissions

Ajoutez les permissions nécessaires à votre `AndroidManifest.xml` :

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```

#### Demandes de permissions à l'exécution

Demandez les permissions à l'exécution dans votre code :

```csharp
private async Task RequestPermissionsAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        // Gérer le refus de permission
    }
    
    status = await Permissions.RequestAsync<Permissions.Microphone>();
    if (status != PermissionStatus.Granted)
    {
        // Gérer le refus de permission
    }
}
```

### Configuration iOS

#### Paramètres Info.plist

Ajoutez les descriptions d'usage requises à votre `Info.plist` :

```xml
<key>NSCameraUsageDescription</key>
<string>This app requires camera access for video capture</string>
<key>NSMicrophoneUsageDescription</key>
<string>This app requires microphone access for audio recording</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>This app requires photo library access to save media</string>
```

#### App Transport Security

Pour les sources de streaming HTTP, configurez App Transport Security :

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

### Configuration macOS Desktop

Sur macOS, l'application tourne sur la tête `net10.0-desktop`, la même cible que sous Linux. `VisioForge.DotNet.Core` choisit pour elle sa version macOS à la compilation et à l'exécution : la lecture n'a besoin de rien d'autre que le package macOS ci-dessus.

La capture caméra et micro exige une application empaquetée en bundle `.app` dont l'`Info.plist` contient `NSCameraUsageDescription` et `NSMicrophoneUsageDescription` : sans elles, macOS met fin à l'application dès qu'elle ouvre le périphérique. Le bundle est signé avec le runtime renforcé (hardened runtime) : il lui faut aussi les entitlements caméra et micro. Ajoutez les deux fichiers au projet et indiquez-les au build ; ils ne s'appliquent qu'au bundle, pas à `dotnet run` :

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<!-- Platforms/Desktop/Info.plist: Uno complète les clés du bundle listées vides -->
<plist version="1.0">
<dict>
  <key>CFBundleDisplayName</key><string></string>
  <key>CFBundleExecutable</key><string></string>
  <key>CFBundleIdentifier</key><string></string>
  <key>CFBundleName</key><string></string>
  <key>CFBundleShortVersionString</key><string></string>
  <key>CFBundleVersion</key><string></string>
  <key>NSCameraUsageDescription</key><string>Camera access is required to capture video.</string>
  <key>NSMicrophoneUsageDescription</key><string>Microphone access is required to capture audio.</string>
</dict>
</plist>
```

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<!-- Platforms/Desktop/Entitlements.plist: le bundle est signé avec le runtime renforcé -->
<plist version="1.0">
<dict>
  <key>com.apple.security.cs.allow-jit</key><true/>
  <key>com.apple.security.device.camera</key><true/>
  <key>com.apple.security.device.audio-input</key><true/>
</dict>
</plist>
```

```xml
<PropertyGroup>
  <UnoMacOSCustomInfoPlist>Platforms/Desktop/Info.plist</UnoMacOSCustomInfoPlist>
  <UnoMacOSEntitlements>Platforms/Desktop/Entitlements.plist</UnoMacOSEntitlements>
</PropertyGroup>
```

```bash
# osx-x64 pour les Mac Intel. Facultatif : -p:CodesignKey="Developer ID Application: <your team>" signe le bundle
# avec une identité stable, et macOS conserve l'autorisation caméra d'un build à l'autre.
dotnet publish -f net10.0-desktop -r osx-arm64 -p:PackageFormat=app
```

### Configuration Linux Desktop

Pour les applications de bureau Linux utilisant Skia :

1. Assurez-vous que GStreamer est installé sur le système cible
2. Référencez le paquet core Linux de votre architecture cible, comme dans le fichier de projet ci-dessus
3. Définissez les variables d'environnement appropriées si nécessaire :

```bash
export GST_PLUGIN_PATH=/usr/lib/x86_64-linux-gnu/gstreamer-1.0
```

## Compilation pour différentes plateformes

### Windows

```bash
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

### Android

```bash
dotnet build -c Release -f net10.0-android
```

### iOS

```bash
dotnet build -c Release -f net10.0-ios
```

### macOS

```bash
dotnet build -c Release -f net10.0-desktop
```

### Linux Desktop

```bash
dotnet build -c Release -f net10.0-desktop
```

## Considérations de performance

- **Accélération matérielle** : activez le rendu accéléré matériellement lorsqu'il est disponible (Windows DirectX, Apple VideoToolbox, Android MediaCodec)
- **Appareils physiques** : testez toujours sur des appareils physiques, surtout pour les plateformes mobiles. Les simulateurs peuvent ne pas refléter fidèlement les performances en conditions réelles
- **Gestion de la mémoire** : surveillez l'utilisation de la mémoire, en particulier sur les appareils mobiles lors du traitement de gros fichiers multimédias
- **Streaming réseau** : utilisez des tailles de tampon appropriées pour le streaming réseau afin d'équilibrer latence et fluidité

## Dépannage des problèmes courants

### La vidéo ne s'affiche pas

1. Vérifiez que le VideoView est correctement initialisé et ajouté à l'arbre visuel
2. Vérifiez que les redistribuables spécifiques à la plateforme sont correctement installés
3. Assurez-vous que les permissions sont accordées sur les plateformes mobiles

### Problèmes de performance

1. Vérifiez que l'accélération matérielle est activée
2. Réduisez la résolution vidéo pour les appareils moins puissants
3. Surveillez l'utilisation de la mémoire et optimisez les tailles de tampon

### Erreurs de compilation

1. Vérifiez que toutes les charges de travail requises sont installées
2. Vérifiez la compatibilité des versions de paquets NuGet
3. Assurez-vous que les versions du framework cible correspondent dans toutes les références de projet
