---
title: Integrate Media SDKs with .NET MAUI Applications
description: Learn how to implement powerful video and media capabilities in cross-platform .NET MAUI projects. This guide covers setup, configuration, and optimization across Windows, Android, iOS, and macOS platforms, with platform-specific requirements and best practices for seamless integration.
sidebar_label: MAUI
order: 15

---

# Integrating VisioForge SDKs with .NET MAUI Applications

## Overview

.NET Multi-platform App UI (MAUI) enables developers to build cross-platform applications for mobile and desktop from a single codebase. VisioForge provides comprehensive support for MAUI applications through the `VisioForge.Core.UI.MAUI` package, which contains specialized UI controls designed specifically for the .NET MAUI platform.

Our SDKs enable powerful multimedia capabilities across all MAUI-supported platforms:

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

## Getting Started

### Installation

To begin using VisioForge with your MAUI project, install the required NuGet packages:

1. The core UI package: `VisioForge.Core.UI.MAUI`
2. Platform-specific redistributable (detailed in platform sections below)

### SDK Initialization

Proper initialization is essential for the VisioForge SDKs to function correctly within your MAUI application. This process must be completed in your `MauiProgram.cs` file.

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
          // Initialize the SkiaSharp package by adding the below line of code
          .UseSkiaSharp()
          // Initialize the VisioForge MAUI package by adding the below line of code
          .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
          // After initializing the VisioForge MAUI package, optionally add additional fonts
          .ConfigureFonts(fonts =>
          {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });

        // Continue initializing your .NET MAUI App here
        return builder.Build();
    }
}
```

## Using VisioForge Controls in XAML

The `VideoView` control is the primary interface for displaying video content in your MAUI application. To use VisioForge controls in your XAML files:

1. Add the VisioForge namespace to your XAML file:

```xaml
xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
```

2. Add the VideoView control to your layout:

```xaml
<vf:VideoView Grid.Row="0"               
                HorizontalOptions="FillAndExpand"
                VerticalOptions="FillAndExpand"
                x:Name="videoView"
                Background="Black"/>
```

The VideoView control adapts to the native rendering capabilities of each platform while providing a consistent API for your application code.

## Platform-Specific Configuration

### Android Implementation

Android requires additional configuration steps to ensure proper operation:

#### 1. Add Java Bindings Library

The VisioForge SDK relies on native Android functionality that requires a custom Java bindings library:

1. Clone the binding library from our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency)
2. Add the appropriate project to your solution:
   - Use `VisioForge.Core.Android.X8.csproj` for .NET 8
   - Use `VisioForge.Core.Android.X9.csproj` for .NET 9
3. Add the reference to your project file:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

#### 2. Add Android Redistributable Package

Include the Android-specific redistributable package:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="1.22.5.10" />
</ItemGroup>
```

#### 3. Android Permissions

Ensure your AndroidManifest.xml includes the necessary permissions for camera, microphone, and storage access depending on your application's functionality. Common required permissions include:

- `android.permission.CAMERA`
- `android.permission.RECORD_AUDIO`
- `android.permission.READ_EXTERNAL_STORAGE`
- `android.permission.WRITE_EXTERNAL_STORAGE`

### iOS Configuration

iOS integration requires fewer steps but has some important considerations:

#### 1. Add iOS Redistributable

Add the iOS-specific package to your project:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="1.23.0" />
</ItemGroup>
```

#### 2. Important Notes for iOS Development

- **Use physical devices**: The SDK requires testing on physical iOS devices rather than simulators for full functionality.
- **Privacy descriptions**: Add the necessary usage description strings in your Info.plist file for camera and microphone access:
  - `NSCameraUsageDescription`
  - `NSMicrophoneUsageDescription`

### macOS Configuration

For macOS Catalyst applications:

#### 1. Configure Runtime Identifiers

To ensure your application works correctly on both Intel and Apple Silicon Macs, specify the appropriate runtime identifiers:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

#### 2. Enable Trimming

For optimal performance on macOS, enable the PublishTrimmed option:

```xml
<PublishTrimmed Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">true</PublishTrimmed>
```

For more detailed information about macOS deployment, refer to our [macOS](../deployment-x/macOS.md) documentation page.

### Windows Configuration

For Windows applications, you need to include several redistributable packages:

#### 1. Add Base Windows Redistributables

Include the core Windows packages:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Codecs.Windows.x64" Version="15.7.0" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="15.7.0" />
</ItemGroup>
```

#### 2. Add Extended Codec Support (Optional but Recommended)

For enhanced media format support, include the libAV (FFMPEG) package:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="15.7.0" />
</ItemGroup>
```

### Performance Optimization

For optimal performance across platforms:

1. Use hardware acceleration when available
2. Adjust video resolution based on the target device capabilities
3. Consider memory constraints on mobile devices when processing large media files

## Troubleshooting Common Issues

- **Blank video display**: Ensure proper permissions are granted on mobile platforms
- **Missing codecs**: Verify all platform-specific redistributable packages are correctly installed
- **Performance issues**: Check that hardware acceleration is enabled when available
- **Deployment errors**: Confirm runtime identifiers are correctly specified for the target platform

## Conclusion

The VisioForge SDK provides a comprehensive solution for adding powerful multimedia capabilities to your .NET MAUI applications. By following the platform-specific setup instructions and best practices outlined in this guide, you can create rich cross-platform applications with advanced video and audio features.

For additional examples and sample code, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
