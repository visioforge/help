---
title: .NET SDKs Installation Guide for Developers
description: Complete guide for installing multimedia .NET SDKs in Visual Studio, Rider, and other IDEs. Learn step-by-step installation methods, platform-specific configuration, framework support, and troubleshooting for Windows, macOS, iOS, Android, and Linux environments.
sidebar_label: Installation
order: 21

---

# VisioForge .NET SDKs Installation Guide

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

VisioForge offers powerful multimedia SDKs for .NET developers that enable advanced video capture, editing, playback, and media processing capabilities in your applications. This guide covers everything you need to know to properly install and configure our SDKs in your development environment.

## Available .NET SDKs

VisioForge provides several specialized SDKs to address different multimedia needs:

- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) - For capturing video from cameras, screen recording, and streaming
- [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net) - For video editing, processing, and format conversion
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) - For building custom media processing pipelines
- [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) - For creating custom media players with advanced features

## Installation Methods

You can install our SDKs using two primary methods:

### Using Setup Files

The setup file installation method is recommended for Windows development environments. This approach:

1. Automatically installs all required dependencies
2. Configures Visual Studio integration
3. Includes sample projects to help you get started quickly
4. Provides documentation and additional resources

Setup files can be downloaded from the respective SDK product pages on our website.

### Using NuGet Packages

For cross-platform development or CI/CD pipelines, our NuGet packages offer flexibility and easy integration:

```cmd
Install-Package VisioForge.DotNet.Core
```

Additional UI-specific packages may be required depending on your target platform:

```cmd
Install-Package VisioForge.DotNet.Core.UI.MAUI
Install-Package VisioForge.DotNet.Core.UI.WinUI
Install-Package VisioForge.DotNet.Core.UI.Avalonia
```

## IDE Integration and Setup

Our SDKs seamlessly integrate with popular .NET development environments:

### Visual Studio Integration

[Visual Studio](visual-studio.md) offers the most complete experience with our SDKs:

- Full IntelliSense support for SDK components
- Built-in debugging for media processing components
- Designer support for visual controls
- NuGet package management

For detailed Visual Studio setup instructions, see our [Visual Studio integration guide](visual-studio.md).

### JetBrains Rider Integration

[Rider](rider.md) provides excellent cross-platform development support:

- Full code completion for SDK APIs
- Smart navigation features for exploring SDK classes
- Integrated NuGet package management
- Cross-platform debugging capabilities

For Rider-specific instructions, visit our [Rider integration documentation](rider.md).

### Visual Studio for Mac

[Visual Studio for Mac](visual-studio-mac.md) users can develop applications for macOS, iOS, and Android:

- Built-in NuGet package manager for installing SDK components
- Project templates for quick setup
- Integrated debugging tools

Learn more in our [Visual Studio for Mac setup guide](visual-studio-mac.md).

## Platform-Specific Configuration

### Target Framework Configuration

Each operating system requires specific target framework settings for optimal compatibility:

#### Windows Applications

Windows applications must use the `-windows` target framework suffix:

```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

This enables access to Windows-specific APIs and UI frameworks like WPF and Windows Forms.

#### Android Development

Android projects require the `-android` framework suffix:

```xml
<TargetFramework>net8.0-android</TargetFramework>
```

Ensure that Android workloads are installed in your development environment:

```
dotnet workload install android
```

#### iOS Development

iOS applications must use the `-ios` target framework:

```xml
<TargetFramework>net8.0-ios</TargetFramework>
```

iOS development requires a Mac with Xcode installed, even when using Visual Studio on Windows.

#### macOS Applications

macOS native applications use either the `-macos` or `-maccatalyst` framework:

```xml
<TargetFramework>net8.0-macos</TargetFramework>
```

For .NET MAUI applications targeting macOS, use:

```xml
<TargetFramework>net8.0-maccatalyst</TargetFramework>
```

#### Linux Development

Linux applications use the standard target framework without a platform suffix:

```xml
<TargetFramework>net8.0</TargetFramework>
```

Ensure required .NET workloads are installed:

```
dotnet workload install linux
```

## Special Framework Support

### .NET MAUI Applications

[MAUI projects](maui.md) require special configuration:

- Add the `VisioForge.DotNet.Core.UI.MAUI` NuGet package
- Configure platform-specific permissions in your project
- Use MAUI-specific video view controls

See our [detailed MAUI guide](maui.md) for complete instructions.

### Avalonia UI Framework

[Avalonia projects](avalonia.md) provide a cross-platform UI alternative:

- Install the `VisioForge.DotNet.Core.UI.Avalonia` package
- Use Avalonia-specific video rendering controls
- Configure platform-specific dependencies

Our [Avalonia integration guide](avalonia.md) provides complete setup instructions.

## SDK Initialization for Cross-Platform Engines

Our SDKs include both Windows-specific DirectShow engines (like `VideoCaptureCore`) and cross-platform X-engines (like `VideoCaptureCoreX`). The X-engines require explicit initialization and cleanup.

### Initializing the SDK

Before using any X-engine components, initialize the SDK:

```csharp
// Initialize at application startup
VisioForge.Core.VisioForgeX.InitSDK();

// Or use the async version
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

### Cleaning Up Resources

When your application exits, properly release resources:

```csharp
// Clean up at application exit
VisioForge.Core.VisioForgeX.DestroySDK();

// Or use the async version
await VisioForge.Core.VisioForgeX.DestroySDKAsync();
```

Failing to initialize or clean up properly may result in memory leaks or unstable behavior.

## Video Rendering Controls

Each UI framework requires specific video view controls to display media content:

### Windows Forms

```csharp
// Add reference to VisioForge.DotNet.Core
using VisioForge.Core.UI.WinForms;

// In your form
videoView = new VideoView();
this.Controls.Add(videoView);
```

### WPF Applications

```csharp
// Add reference to VisioForge.DotNet.Core
using VisioForge.Core.UI.WPF;

// In your XAML
<vf:VideoView x:Name="videoView" />
```

### MAUI Applications

```csharp
// Add reference to VisioForge.DotNet.Core.UI.MAUI
using VisioForge.Core.UI.MAUI;

// In your XAML
<vf:VideoView x:Name="videoView" />
```

### Avalonia UI

```csharp
// Add reference to VisioForge.DotNet.Core.UI.Avalonia
using VisioForge.Core.UI.Avalonia;

// In your XAML
<vf:VideoView Name="videoView" />
```

## Native Dependencies Management

Our SDKs leverage native libraries for optimal performance. These dependencies must be properly managed for deployment:

- Windows: Included automatically with setup installation or NuGet packages
- macOS/iOS: Bundled with NuGet packages but require proper app signing
- Android: Included in NuGet packages with proper architecture support
- Linux: May require additional system packages depending on distribution

For detailed deployment instructions, see our [deployment guide](../deployment-x/index.md).

## Troubleshooting Common Installation Issues

If you encounter issues during installation:

1. Verify target framework compatibility with your project type
2. Ensure all required workloads are installed (`dotnet workload list`)
3. Check for dependency conflicts in your project
4. Confirm proper SDK initialization for X-engines
5. Review platform-specific requirements in our documentation

## Sample Code and Resources

We maintain an extensive collection of sample applications on our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) to help you get started quickly with our SDKs.

These examples cover common scenarios like:

- Video capture from cameras and screens
- Media playback with custom controls
- Video editing and processing
- Cross-platform development

Visit our repository for the latest code examples and best practices for using our SDKs.

---

For additional support or questions, please contact our technical support team or visit our documentation portal.
