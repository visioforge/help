---
title: Integrate Media SDKs with Uno Platform Applications
description: Build cross-platform Uno Platform apps with multimedia capabilities for Windows, Android, iOS, macOS, and Linux using VisioForge SDKs.
---

# Integrating VisioForge SDKs with Uno Platform Applications

## Overview

Uno Platform is a powerful cross-platform UI framework that enables developers to build native mobile, desktop, and embedded applications from a single C# and XAML codebase. VisioForge provides comprehensive support for Uno Platform applications through the `VisioForge.DotNet.Core.UI.Uno` package, which contains specialized UI controls designed specifically for the Uno Platform.

Our SDKs enable powerful multimedia capabilities across all Uno Platform-supported platforms:

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Supported Platforms

Uno Platform with VisioForge SDKs supports:

- **Windows Desktop** - Full native WinUI 3 support with hardware acceleration
- **Android** - Native Android views with MediaCodec hardware acceleration
- **iOS** - Native UIKit with VideoToolbox hardware acceleration
- **macOS** - Mac Catalyst support for Apple Silicon and Intel
- **Linux Desktop** - Skia-based rendering with GStreamer

## Quick Start

### 1. Install NuGet Packages

Add the core VisioForge packages to your Uno Platform project:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2025.12.9" />
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.4.10" />
</ItemGroup>
```

### 2. Initialize the SDK

Initialize the SDK at application startup. You can use either the synchronous or asynchronous version:

```csharp
using VisioForge.Core;

// Synchronous initialization
VisioForgeX.InitSDK();

// Or async initialization (recommended)
await VisioForgeX.InitSDKAsync();
```

Clean up when the application exits:

```csharp
VisioForgeX.DestroySDK();
```

### 3. Add VideoView to XAML

Add the VisioForge namespace and VideoView control to your XAML:

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

The VideoView control adapts to the native rendering capabilities of each platform while providing a consistent API for your application code.

## Implementing VideoView in Code

Here's a complete example of using VideoView with the Media Blocks pipeline:

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
        // Initialize SDK (once at app startup)
        await VisioForgeX.InitSDKAsync();
    }
    
    private async Task PlayMediaAsync(string mediaPath)
    {
        // Clean up previous pipeline if any
        await CleanupPipelineAsync();
        
        // Create pipeline
        _pipeline = new MediaBlocksPipeline();
        
        // Create source settings - use factory method for URLs or file paths
        // Video-only playback: disable audio since we're not connecting audio output
        UniversalSourceSettings sourceSettings;
        if (Uri.TryCreate(mediaPath, UriKind.Absolute, out var uri) && !uri.IsFile)
        {
            // URL source
            sourceSettings = await UniversalSourceSettings.CreateAsync(uri, renderAudio: false);
        }
        else
        {
            // File path source
            sourceSettings = await UniversalSourceSettings.CreateAsync(mediaPath, renderAudio: false);
        }
        
        // Create source and renderer blocks
        _source = new UniversalSourceBlock(sourceSettings);
        _renderer = new VideoRendererBlock(_pipeline, videoView);
        
        // Connect blocks
        _pipeline.Connect(_source.VideoOutput, _renderer.Input);
        
        // Start playback
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
        
        // Destroy SDK when app closes
        VisioForgeX.DestroySDK();
    }
}
```

## Sample Applications

For complete examples and sample code, visit:

- Sample applications in the `_DEMOS/Media Blocks SDK/Uno/` folder
- Sample applications in the `_DEMOS/Media Player SDK X/Uno/` folder
- Sample applications in the `_DEMOS/Video Capture SDK X/Uno/` folder
- Our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples)

Available samples include:

- **Simple Player** - Basic media player with playback controls
- **RTSP Viewer** - Network camera streaming viewer
- **Video Capture** - Camera capture with recording functionality

## Next Steps

For complete deployment instructions including:

- Platform-specific redistributable packages
- System requirements
- Build commands
- Platform configurations (permissions, capabilities, Info.plist settings)
- Complete sample project file
- Troubleshooting

See the [Uno Platform Deployment Guide](../deployment-x/uno.md).

## Additional Resources

- [Windows Deployment](../deployment-x/Windows.md)
- [Android Deployment](../deployment-x/Android.md)
- [iOS Deployment](../deployment-x/iOS.md)
- [macOS Deployment](../deployment-x/macOS.md)
