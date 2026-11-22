---
title: Uno Platform Deployment Guide for VisioForge .NET SDKs
description: VisioForge .NET SDK Uno Platform deployment with VideoView integration, multi-platform support for Windows, Android, iOS, macOS, and Linux.
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

# Uno Platform Implementation and Deployment Guide

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

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
| macOS Desktop (Skia) | net10.0-desktop | &#x2714; Full Support |
| Linux Desktop (Skia) | net10.0-desktop | &#x2714; Full Support |

The targets are those of Uno Platform 7, which has no Mac Catalyst target: macOS apps run on the `net10.0-desktop` head. Uno Platform 7 needs a `VisioForge.DotNet.Core.UI.Uno` package built for it; versions up to 2026.11.16 are built for Uno Platform 6.5.

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
- Windows App SDK 2.5 or later (Uno Platform 7 brings it)

#### Android
- Android SDK with appropriate API levels
- Android 7.0 (API 24) or later device
- Java Development Kit (JDK) 11 or later

#### iOS/macOS
- Mac computer with Xcode 26 or later for iOS/macOS builds (Xcode 27.2 or later for .NET 11)
- Apple Developer account (for device deployment)
- iOS 18.0 or later / macOS 12 or later

#### Linux
- GStreamer runtime installed
- X11 or Wayland display server
- The `VisioForge.CrossPlatform.Core.Linux.x64` (or `.arm64`) NuGet package

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

# For iOS
dotnet workload install ios
```

### 3. Create a New Uno Platform Project

```bash
dotnet new unoapp -o MyMediaApp
```

### 4. Add VisioForge NuGet Packages

Add the following packages to your project:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="[2026.11.21]"/>
  <PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.21]"/>
</ItemGroup>
```

### Platform-Specific Redistributables

Add platform-specific redistributable packages to your project:

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

Additionally, you'll need to add the Java Bindings Library. Clone it from our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency) and add a reference:

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

The package applies to the desktop head when the project is built on a Mac. Its build targets copy the native libraries next to the application.

#### Linux Desktop

For Linux, you need to install GStreamer runtime on your system:

```bash
# Ubuntu/Debian
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav

# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free
```

Then add the core package for the target architecture. It carries the SDK's own native libraries (the color conversion behind snapshots, frame-to-bitmap conversion and overlays, the 3D LUT effect, Data Matrix) and no GStreamer. Without it the application starts, but those features fail at run time with `VisioForge_VAP is not available`:

```xml
<ItemGroup Condition="$([MSBuild]::IsOSPlatform('linux')) or $(RuntimeIdentifier.StartsWith('linux-'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.arm64" Version="[2026.10.26]" Condition="$(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64')))"/>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]" Condition="!($(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64'))))"/>
</ItemGroup>
```

The block applies when the project is built on Linux or published for a `linux-*` runtime identifier; the target frameworks above are still chosen by the build host, so a Uno Linux build from Windows needs `net10.0-desktop` set explicitly. The conditions pick the package from the runtime identifier: `-r linux-arm64` or a publish profile for ARM64 selects the ARM64 package, and with no runtime identifier the build machine's architecture decides.

### Complete Sample Project File

Here is a complete example `.csproj` file for an Uno Platform application with VisioForge SDK:

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <!-- Target frameworks based on build OS -->
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-desktop;net10.0-ios;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('linux'))">net10.0-desktop</TargetFrameworks>
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
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="[2026.11.21]"/>
    <PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.21]"/>
  </ItemGroup>
  
  <!-- Windows Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.16]"/>
    <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.16]"/>
  </ItemGroup>
  
  <!-- Android Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-android'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="[2026.11.5]"/>
    <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
  </ItemGroup>
  
  <!-- iOS Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="[2026.11.5]"/>
  </ItemGroup>
  
  <!-- macOS Desktop Platform -->
  <ItemGroup Condition="$(TargetFramework.Contains('-desktop')) and $([MSBuild]::IsOSPlatform('osx'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="[2026.11.13]"/>
  </ItemGroup>
  
  <!-- Linux Desktop Platform -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('linux')) or $(RuntimeIdentifier.StartsWith('linux-'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.arm64" Version="[2026.10.26]" Condition="$(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64')))"/>
    <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]" Condition="!($(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64'))))"/>
  </ItemGroup>
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

### macOS Desktop Configuration

On macOS the app runs on the `net10.0-desktop` head, the same target as Linux. `VisioForge.DotNet.Core` selects its macOS build for it at compile time and at run time, so playback needs nothing beyond the macOS package above.

Camera and microphone capture needs the app packaged as an `.app` bundle whose `Info.plist` carries `NSCameraUsageDescription` and `NSMicrophoneUsageDescription`: without them macOS terminates the app the first time it opens the device. The bundle is signed with the hardened runtime, so it also needs the camera and microphone entitlements. Add both files to the project and point the build at them; the files apply to the bundle only, not to `dotnet run`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<!-- Platforms/Desktop/Info.plist: Uno fills in the bundle keys listed with empty values -->
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
<!-- Platforms/Desktop/Entitlements.plist: the bundle is signed with the hardened runtime -->
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
# osx-x64 for Intel Macs. Optional: -p:CodesignKey="Developer ID Application: <your team>" signs the bundle
# with a stable identity, so macOS keeps the camera grant across rebuilds.
dotnet publish -f net10.0-desktop -r osx-arm64 -p:PackageFormat=app
```

### Linux Desktop Configuration

For Linux desktop applications using Skia:

1. Ensure GStreamer is installed on the target system
2. Reference the Linux core package for the target architecture, as in the project file above
3. Set appropriate environment variables if needed:

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
dotnet build -c Release -f net10.0-desktop
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
