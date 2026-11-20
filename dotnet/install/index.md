---
title: Install VisioForge .NET SDKs — NuGet, Visual Studio, Rider
description: Setup files or NuGet packages. Target frameworks for Windows, macOS, iOS, Android, and Linux. SDK initialization, VideoView controls, and native dependencies.
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

# VisioForge .NET SDKs Installation Guide

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge offers powerful multimedia SDKs for .NET developers that enable advanced video capture, editing, playback, and media processing capabilities in your applications. This guide covers everything you need to know to properly install and configure our SDKs in your development environment.

## Available .NET SDKs

VisioForge provides several specialized SDKs to address different multimedia needs:

- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) - For capturing video from cameras, screen recording, and streaming
- [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) - For video editing, processing, and format conversion
- [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) - For building custom media processing pipelines
- [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) - For creating custom media players with advanced features

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

For cross-platform development or CI/CD pipelines, our NuGet packages offer flexibility and easy integration.

Install the package for the SDK you bought — each one pulls `VisioForge.DotNet.Core` in as a
dependency, so you never reference `Core` directly:

```powershell
Install-Package VisioForge.DotNet.VideoCapture   # Video Capture SDK .NET
Install-Package VisioForge.DotNet.VideoEdit      # Video Edit SDK .NET
Install-Package VisioForge.DotNet.MediaPlayer    # Media Player SDK .NET
Install-Package VisioForge.DotNet.MediaBlocks    # Media Blocks SDK .NET
```

These packages hold the managed SDK only. The native engine comes in a separate redist package, which you reference next to it:

| Engine | Platform | Redist package |
|--------|----------|----------------|
| X engines (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`, `MediaBlocksPipeline`) | Windows | `VisioForge.CrossPlatform.Core.Windows.x64` (`.x86` or `.arm64` for other CPUs; on ARM64, MP3 playback also needs `VisioForge.CrossPlatform.Libav.Windows.arm64`, and NDI is not available) |
| X engines | macOS, Mac Catalyst | `VisioForge.CrossPlatform.Core.macOS` |
| X engines | Android | `VisioForge.CrossPlatform.Core.Android` |
| X engines | iOS | `VisioForge.CrossPlatform.Core.iOS` |
| X engines | Linux | `VisioForge.CrossPlatform.Core.Linux.x64` or `VisioForge.CrossPlatform.Core.Linux.arm64`, plus GStreamer 1.20+ from the distribution (1.22+ for some features) |
| `VideoCaptureCore` (Windows only) | Windows | `VisioForge.DotNet.Core.Redist.VideoCapture.x64` (`.x86` for 32-bit apps) |
| `MediaPlayerCore` (Windows only) | Windows | `VisioForge.DotNet.Core.Redist.MediaPlayer.x64` and `VisioForge.DotNet.Core.Redist.LAV.x64`. LAV is the default `Source_Mode`, the GPU and Blu-ray modes use LAV too, and so does `GetFileThumbnail`: leave the LAV package out only when the app uses none of them. Add `VisioForge.DotNet.Core.Redist.FFMPEG.x64` when the app uses the FFMPEG mode, and `VisioForge.DotNet.Core.Redist.VLC.x64` for `File_VLC` or `HTTP_RTSP_VLC` (`.x86` of each package for 32-bit apps) |
| `VideoEditCore` (Windows only) | Windows | `VisioForge.DotNet.Core.Redist.VideoEdit.x64` and `VisioForge.DotNet.Core.Redist.FFMPEG.x64` to open MP4, MKV and other input files (`.x86` for 32-bit apps) |

For example, a Windows x64 app on the X engines adds:

```powershell
Install-Package VisioForge.CrossPlatform.Core.Windows.x64
Install-Package VisioForge.CrossPlatform.Libav.Windows.x64   # recommended: FFmpeg (libav) decoders and encoders
```

Some platforms need a step beyond the package: Android also takes the Java bindings project ([Android deployment](../deployment-x/Android.md)), and Mac Catalyst a build target that copies the native libraries into the app bundle ([macOS deployment](../deployment-x/macOS.md)). With the Windows-only engines, an AnyCPU app deploys both the x86 and the x64 packages. Their engine packages, LAV and FFMPEG have no ARM64 build, so on Windows on Arm, where an AnyCPU app usually runs as ARM64, build the app as x64 or use the X engines. The optional packages are listed in the [deployment guide](../deployment-x/index.md) for the X engines and on the deployment pages of [Video Capture SDK .NET](../videocapture/deployment.md), [Media Player SDK .NET](../mediaplayer/deployment.md) and [Video Edit SDK .NET](../videoedit/deployment.md) for the Windows-only engines.

Additional UI-specific packages may be required depending on your target platform:

```powershell
Install-Package VisioForge.DotNet.Core.UI.MAUI
Install-Package VisioForge.DotNet.Core.UI.WinUI
Install-Package VisioForge.DotNet.Core.UI.Avalonia
Install-Package VisioForge.DotNet.Core.UI.Uno
```

WPF and Windows Forms need no UI package: their `VideoView` is in `VisioForge.Core`, which every SDK package above brings in. The separate `VisioForge.DotNet.Core.UI.WPF` and `VisioForge.DotNet.Core.UI.WinForms` packages are not needed.

## IDE Integration and Setup

Our SDKs seamlessly integrate with popular .NET development environments:

### Visual Studio Integration

[Visual Studio](visual-studio.md) offers the most complete experience with our SDKs:

- Full IntelliSense support for SDK components
- Built-in debugging for media processing components
- Designer support for visual controls
- NuGet package management

For detailed Visual Studio setup instructions, see our [Visual Studio integration guide](visual-studio.md). To start from a working app, use the [VisioForge project templates](project-templates.md) for Visual Studio 2026 and `dotnet new`.

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
<TargetFramework>net10.0-windows</TargetFramework>
```

This enables access to Windows-specific APIs and UI frameworks like WPF and Windows Forms.

#### Android Development

Android projects require the `-android` framework suffix:

```xml
<TargetFramework>net10.0-android</TargetFramework>
```

Ensure that Android workloads are installed in your development environment:

```
dotnet workload install android
```

#### iOS Development

iOS applications must use the `-ios` target framework:

```xml
<TargetFramework>net10.0-ios</TargetFramework>
```

iOS development requires a Mac with Xcode installed, even when using Visual Studio on Windows.

#### macOS Applications

macOS native applications use either the `-macos` or `-maccatalyst` framework:

```xml
<TargetFramework>net10.0-macos</TargetFramework>
```

For .NET MAUI applications targeting macOS, use:

```xml
<TargetFramework>net10.0-maccatalyst</TargetFramework>
```

#### Linux Development

Linux applications use the standard target framework without a platform suffix:

```xml
<TargetFramework>net10.0</TargetFramework>
```

Add `VisioForge.CrossPlatform.Core.Linux.x64`, or `VisioForge.CrossPlatform.Core.Linux.arm64` on ARM64. It carries the SDK's own native libraries only: install GStreamer 1.20 or later (1.22 or later for some features, such as the OpenCV blocks and the ONVIF audio backchannel) from the distribution packages, as described in [Ubuntu deployment](../deployment-x/Ubuntu.md).

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

### Uno Platform Applications

[Uno Platform projects](uno.md) enable building applications for Windows, Android, iOS, macOS, and Linux from a single codebase:

- Add the `VisioForge.DotNet.Core.UI.Uno` NuGet package
- Use Uno Platform-specific video view controls
- Configure platform-specific redistributables

See our [Uno Platform guide](uno.md) for complete setup instructions.

### Unity

[Unity](unity.md) integration uses a ready-to-import `.unitypackage` instead of NuGet:

- Self-contained for Unity 6 on Windows x64 — managed SDK, native runtime, and sample scenes
- No source build, no NuGet, and no external dependencies
- Video renders into a Unity `RawImage` via the bundled scripts

See our [Unity installation guide](unity.md) for the step-by-step setup.

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
// Clean up at application exit (sync — DestroySDK has no async variant)
VisioForge.Core.VisioForgeX.DestroySDK();
```

Failing to initialize or clean up properly may result in memory leaks or unstable behavior.

## Video Rendering Controls

Each UI framework requires specific video view controls to display media content:

### Windows Forms

```csharp
// At the top of Form1.cs. The WinForms VideoView is in VisioForge.Core: no extra package
using VisioForge.Core.UI.WinForms;

// Inside the Form1 class: a field, added to the form in the existing constructor
private readonly VideoView videoView = new VideoView { Dock = DockStyle.Fill };

public Form1()
{
    InitializeComponent();
    Controls.Add(videoView); // add this line to the existing constructor
}
```

### WPF Applications

```xml
<!-- MainWindow.xaml. The WPF VideoView is in VisioForge.Core: no extra package -->
<Window ...
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core">
    <vf:VideoView x:Name="videoView" />
</Window>
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

- Windows: the setup installer installs them on the development machine. An application you ship takes the redist package for its engine and CPU from the [table above](#using-nuget-packages); its libraries are copied next to the application on build
- macOS/iOS: `VisioForge.CrossPlatform.Core.macOS` and `VisioForge.CrossPlatform.Core.iOS` carry the native libraries, GStreamer included; the app still needs proper signing, and Mac Catalyst a build target (see [macOS deployment](../deployment-x/macOS.md))
- Android: `VisioForge.CrossPlatform.Core.Android` carries the native libraries for every Android architecture; the app also takes the Java bindings project (see [Android deployment](../deployment-x/Android.md))
- Linux: `VisioForge.CrossPlatform.Core.Linux.x64` or `.arm64` carries the SDK's own native libraries only; GStreamer comes from the distribution packages (see [Ubuntu deployment](../deployment-x/Ubuntu.md))

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
