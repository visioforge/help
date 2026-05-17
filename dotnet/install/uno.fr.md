---
title: SDK vidéo pour Uno Platform — .NET multiplateforme
description: Ajoutez capture, lecture et édition vidéo aux applis Uno Platform. Installation NuGet, intégration VideoView pour Windows, Android, iOS, macOS et Linux.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - WPF
  - Uno
  - GStreamer
  - Streaming
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - UniversalSourceSettings
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock

---

# Intégrer les SDK VisioForge à des applications Uno Platform

## Vue d'ensemble

Uno Platform est un puissant framework d'interface utilisateur multiplateforme qui permet aux développeurs de créer des applications natives mobiles, de bureau et embarquées à partir d'une seule base de code C# et XAML. VisioForge propose une prise en charge complète des applications Uno Platform via le paquet `VisioForge.DotNet.Core.UI.Uno`, qui contient des contrôles d'interface utilisateur spécialement conçus pour Uno Platform.

Nos SDK apportent de puissantes capacités multimédias sur toutes les plateformes prises en charge par Uno Platform :

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Plateformes prises en charge

Uno Platform avec les SDK VisioForge prend en charge :

- **Windows Desktop** — prise en charge native complète de WinUI 3 avec accélération matérielle
- **Android** — vues Android natives avec accélération matérielle MediaCodec
- **iOS** — UIKit natif avec accélération matérielle VideoToolbox
- **macOS** — prise en charge Mac Catalyst pour Apple Silicon et Intel
- **Linux Desktop** — rendu basé sur Skia avec GStreamer

## Démarrage rapide

### 1. Installer les paquets NuGet

Ajoutez les paquets VisioForge essentiels à votre projet Uno Platform :

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2026.*" />
  <PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />
</ItemGroup>
```

### 2. Initialiser le SDK

Initialisez le SDK au démarrage de l'application. Vous pouvez utiliser la version synchrone ou asynchrone :

```csharp
using VisioForge.Core;

// Initialisation synchrone
VisioForgeX.InitSDK();

// Ou initialisation asynchrone (recommandée)
await VisioForgeX.InitSDKAsync();
```

Libérez les ressources lorsque l'application se termine :

```csharp
VisioForgeX.DestroySDK();
```

### 3. Ajouter VideoView au XAML

Ajoutez l'espace de noms VisioForge et le contrôle VideoView à votre XAML :

```xml
<Page x:Class="YourApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:vf="using:VisioForge.Core.UI.Uno">
    
    <Grid>
        <vf:VideoView x:Name="videoView"               
                      HorizontalAlignment="Stretch"
                      VerticalAlignment="Stretch"
                      Background="Black"/>
    </Grid>
</Page>
```

Le contrôle VideoView s'adapte aux capacités de rendu natives de chaque plateforme tout en exposant une API cohérente pour votre code applicatif.

## Implémentation de VideoView en code

Voici un exemple complet d'utilisation de VideoView avec le pipeline Media Blocks :

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

public sealed partial class MainPage : Page
{
    private MediaBlocksPipeline? _pipeline;
    private UniversalSourceBlock? _source;
    private VideoRendererBlock? _renderer;
    
    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
        this.Unloaded += MainPage_Unloaded;
    }
    
    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Initialiser le SDK (une seule fois au démarrage de l'application)
        await VisioForgeX.InitSDKAsync();
    }
    
    private async Task PlayMediaAsync(string mediaPath)
    {
        // Nettoyer le pipeline précédent, le cas échéant
        await CleanupPipelineAsync();
        
        // Créer le pipeline
        _pipeline = new MediaBlocksPipeline();
        
        // Créer les paramètres de la source — utiliser la méthode factory pour les URL ou les chemins de fichiers
        // Lecture vidéo uniquement : désactiver l'audio puisque nous ne connectons pas de sortie audio
        UniversalSourceSettings sourceSettings;
        if (Uri.TryCreate(mediaPath, UriKind.Absolute, out var uri) && !uri.IsFile)
        {
            // Source URL
            sourceSettings = await UniversalSourceSettings.CreateAsync(uri, renderAudio: false);
        }
        else
        {
            // Source par chemin de fichier
            sourceSettings = await UniversalSourceSettings.CreateAsync(mediaPath, renderAudio: false);
        }
        
        // Créer les blocs source et moteur de rendu
        _source = new UniversalSourceBlock(sourceSettings);
        _renderer = new VideoRendererBlock(_pipeline, videoView);
        
        // Connecter les blocs
        _pipeline.Connect(_source.VideoOutput, _renderer.Input);
        
        // Démarrer la lecture
        await _pipeline.StartAsync();
    }
    
    private async Task CleanupPipelineAsync()
    {
        if (_pipeline != null)
        {
            await _pipeline.StopAsync();
            await _pipeline.DisposeAsync();
            _pipeline = null;
        }
        
        _source?.Dispose();
        _source = null;
        
        _renderer?.Dispose();
        _renderer = null;
    }
    
    private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
    {
        await CleanupPipelineAsync();
        
        // Détruire le SDK à la fermeture de l'application
        VisioForgeX.DestroySDK();
    }
}
```

## Applications d'exemple

Pour des exemples complets et du code d'exemple, consultez :

- Les applications d'exemple dans le dossier `_DEMOS/Media Blocks SDK/Uno/`
- Les applications d'exemple dans le dossier `_DEMOS/Media Player SDK X/Uno/`
- Les applications d'exemple dans le dossier `_DEMOS/Video Capture SDK X/Uno/`
- Notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples)

Les exemples disponibles incluent :

- **Simple Player** — lecteur multimédia basique avec contrôles de lecture
- **RTSP Viewer** — visionneuse de flux pour caméras réseau
- **Video Capture** — capture caméra avec fonctionnalité d'enregistrement

## Étapes suivantes

Pour les instructions complètes de déploiement, comprenant :

- Les paquets redistribuables spécifiques à la plateforme
- La configuration système requise
- Les commandes de build
- Les configurations spécifiques à la plateforme (permissions, capabilities, paramètres Info.plist)
- Le fichier projet d'exemple complet
- Le dépannage

Consultez le [Guide de déploiement Uno Platform](../deployment-x/uno.md).

## Ressources supplémentaires

- [Déploiement Windows](../deployment-x/Windows.md)
- [Déploiement Android](../deployment-x/Android.md)
- [Déploiement iOS](../deployment-x/iOS.md)
- [Déploiement macOS](../deployment-x/macOS.md)
