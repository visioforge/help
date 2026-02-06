---
title: Cross-platform .NET Deployment for Uno Platform
description: VisioForge .NET SDK Uno Platform deployment with VideoView integration, multi-platform support for Windows, Android, iOS, macOS, and Linux.
---

# Uno Platform Implementation and Deployment Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to VisioForge SDKs for Uno Platform

Uno Platform is a powerful cross-platform UI framework that enables developers to build native applications for Windows, Android, iOS, macOS, and Linux from a single codebase. VisioForge provides comprehensive support for Uno Platform applications through the `VisioForge.DotNet.Core.UI.Uno` package, which contains specialized UI controls designed specifically for the Uno Platform.

The Uno Platform deployment process requires special consideration for each target platform. This document provides detailed instructions to ensure your application runs smoothly across all supported platforms.

## Supported Platforms

VisioForge SDKs support the following Uno Platform targets:

| Platform | Framework Target | Status |
|----------|------------------|--------|
| Windows Desktop | net10.0-windows10.0.19041.0 | &#x2714; Full Support |
| Android | net10.0-android | &#x2714; Full Support |
| iOS | net10.0-ios | &#x2714; Full Support |
| macOS (Catalyst) | net10.0-maccatalyst | &#x2714; Full Support |
| Linux Desktop (Skia) | net10.0-desktop | &#x2714; Full Support |

## System Requirements

Before beginning your Uno Platform implementation, ensure your development environment meets the following requirements:

### Development Environment Requirements

- Windows, Linux, or macOS computer
- Visual Studio 2022 with Uno Platform extension, JetBrains Rider, or Visual Studio Code
- .NET 10.0 SDK or later (latest stable version recommended)
- Uno Platform templates installed

### Platform-Specific Requirements

#### Windows
- Windows 10 version 17763 or later
- Windows App SDK 1.4+

#### Android
- Android SDK with appropriate API levels
- Android 5.0 (API 21) or later device
- Java Development Kit (JDK) 11 or later

#### iOS/macOS
- Mac computer with Xcode 15+ installed (for iOS/macOS builds)
- Apple Developer account (for device deployment)
- iOS 15.0 or later / macOS 10.15 or later

#### Linux
- GStreamer runtime installed
- X11 or Wayland display server

## Installation and Setup Process

Follow these steps to properly set up and deploy your VisioForge-powered Uno Platform application:

### 1. Install Uno Platform Templates

```bash
dotnet new install Uno.Templates
```

### 2. Install Required Workloads

```bash
# For Android
dotnet workload install android

# For iOS/macOS
dotnet workload install ios maccatalyst
```

### 3. Create a New Uno Platform Project

```bash
dotnet new unoapp -o MyMediaApp
```

### 4. Add VisioForge NuGet Packages

Add the following packages to your project:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2025.12.9" />
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.4.10" />
</ItemGroup>
```

### Platform-Specific Redistributables

Add platform-specific redistributable packages to your project:

#### Windows

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="Microsoft.WindowsAppSDK" Version="1.8.251106002" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
</ItemGroup>
```

#### Android

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
</ItemGroup>
```

Additionally, you'll need to add the Java Bindings Library. Clone it from our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency) and add a reference:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

#### iOS

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
```

#### macOS (Catalyst)

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
</ItemGroup>
```

For macOS Catalyst, you also need to add a custom MSBuild target to copy native libraries to the app bundle:

```xml
<Target Name="CopyNativeLibrariesToMonoBundle" AfterTargets="Build" Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PropertyGroup>
    <AppBundleDir>$(OutputPath)$(AssemblyName).app</AppBundleDir>
    <MonoBundleDir>$(AppBundleDir)/Contents/MonoBundle</MonoBundleDir>
  </PropertyGroup>
  <MakeDir Directories="$(MonoBundleDir)" Condition="!Exists('$(MonoBundleDir)')" />
  <Copy SourceFiles="@(None->'%(FullPath)')" DestinationFolder="$(MonoBundleDir)" 
        Condition="'%(Extension)' == '.dylib' Or '%(Extension)' == '.so'">
    <Output TaskParameter="CopiedFiles" ItemName="CopiedNativeFiles" />
  </Copy>
</Target>
```

#### Linux Desktop

For Linux, you need to install GStreamer runtime on your system:

```bash
# Ubuntu/Debian
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav

# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free
```

### Complete Sample Project File

Here is a complete example `.csproj` file for an Uno Platform application with VisioForge SDK:

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <!-- Target frameworks based on build OS -->
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-maccatalyst;net10.0-ios;net10.0-android</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <UnoSingleProject>true</UnoSingleProject>
    <UseCurrentXcodeSDKVersion>true</UseCurrentXcodeSDKVersion>
    
    <!-- Application settings -->
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
  
  <!-- VisioForge Core References -->
  <ItemGroup>
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2025.12.9" />
    <PackageReference Include="VisioForge.DotNet.Core" Version="2025.4.10" />
  </ItemGroup>
  
  <!-- Windows Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
    <PackageReference Include="Microsoft.WindowsAppSDK" Version="1.8.251106002" />
    <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
    <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
  </ItemGroup>
  
  <!-- Android Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-android'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
    <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
  </ItemGroup>
  
  <!-- iOS Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
  </ItemGroup>
  
  <!-- macOS Catalyst Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
  </ItemGroup>
  
  <!-- macOS: Copy native libraries to app bundle -->
  <Target Name="CopyNativeLibrariesToMonoBundle" AfterTargets="Build" 
          Condition="$(TargetFramework.Contains('-maccatalyst'))">
    <PropertyGroup>
      <AppBundleDir>$(OutputPath)$(AssemblyName).app</AppBundleDir>
      <MonoBundleDir>$(AppBundleDir)/Contents/MonoBundle</MonoBundleDir>
    </PropertyGroup>
    <MakeDir Directories="$(MonoBundleDir)" Condition="!Exists('$(MonoBundleDir)')" />
    <Copy SourceFiles="@(None->'%(FullPath)')" DestinationFolder="$(MonoBundleDir)" 
          Condition="'%(Extension)' == '.dylib' Or '%(Extension)' == '.so'">
      <Output TaskParameter="CopiedFiles" ItemName="CopiedNativeFiles" />
    </Copy>
  </Target>
</Project>
```

## Platform-Specific Configuration

### Windows Configuration

Windows applications use native WinUI 3 rendering and support hardware acceleration via DirectX.

#### Required Capabilities

Add required capabilities to your `Package.appxmanifest`:

```xml
<Capabilities>
    <Capability Name="internetClient" />
    <uap:Capability Name="videosLibrary" />
    <uap:Capability Name="musicLibrary" />
    <DeviceCapability Name="microphone" />
    <DeviceCapability Name="webcam" />
</Capabilities>
```

### Android Configuration

#### Permissions

Add the necessary permissions to your `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```

#### Runtime Permission Requests

Request permissions at runtime in your code:

```csharp
private async Task RequestPermissionsAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        // Handle permission denial
    }
    
    status = await Permissions.RequestAsync<Permissions.Microphone>();
    if (status != PermissionStatus.Granted)
    {
        // Handle permission denial
    }
}
```

### iOS Configuration

#### Info.plist Settings

Add required usage descriptions to your `Info.plist`:

```xml
<key>NSCameraUsageDescription</key>
<string>This app requires camera access for video capture</string>
<key>NSMicrophoneUsageDescription</key>
<string>This app requires microphone access for audio recording</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>This app requires photo library access to save media</string>
```

#### App Transport Security

For HTTP streaming sources, configure App Transport Security:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

### macOS (Catalyst) Configuration

macOS Catalyst applications share configuration with iOS. Additionally, configure runtime identifiers for both Intel and Apple Silicon:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

### Linux Desktop Configuration

For Linux desktop applications using Skia:

1. Ensure GStreamer is installed on the target system
2. Set appropriate environment variables if needed:

```bash
export GST_PLUGIN_PATH=/usr/lib/x86_64-linux-gnu/gstreamer-1.0
```

## Building for Different Platforms

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
dotnet build -c Release -f net10.0-maccatalyst
```

### Linux Desktop

```bash
dotnet build -c Release -f net10.0-desktop
```

## Performance Considerations

- **Hardware Acceleration**: Enable hardware-accelerated rendering where available (Windows DirectX, Apple VideoToolbox, Android MediaCodec)
- **Physical Devices**: Always test on physical devices, especially for mobile platforms. Simulators may not accurately represent real-world performance
- **Memory Management**: Monitor memory usage, particularly on mobile devices when processing large media files
- **Network Streaming**: Use appropriate buffer sizes for network streaming to balance latency and smoothness

## Troubleshooting Common Issues

### Video Not Displaying

1. Verify the VideoView is properly initialized and added to the visual tree
2. Check that platform-specific redistributables are correctly installed
3. Ensure permissions are granted on mobile platforms

### Performance Issues

1. Check that hardware acceleration is enabled
2. Reduce video resolution for lower-powered devices
3. Monitor memory usage and optimize buffer sizes

### Build Errors

1. Verify all required workloads are installed
2. Check for NuGet package version compatibility
3. Ensure target framework versions match across all project references
