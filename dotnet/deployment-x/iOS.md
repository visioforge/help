---
title: iOS Cross-Platform .NET App Deployment Guide
description: Step-by-step guide for .NET developers on deploying cross-platform applications to iOS devices. Learn about required permissions, SDK integration, architecture support, and best practices for successful iOS app deployment.
sidebar_label: iOS

---

# Apple iOS Deployment Guide

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

## Overview

This comprehensive guide walks you through the process of deploying VisioForge SDK-powered applications to Apple iOS devices. The VisioForge SDK provides a powerful framework for building media-rich applications on iOS, offering robust support for video capture, editing, playback, and processing capabilities.

The iOS deployment process involves several key considerations, from package management to permission handling and performance optimization. This document will guide you through each step to ensure a smooth deployment experience.

## System Requirements

Before beginning your iOS deployment process, ensure your development environment meets the following requirements:

### Hardware Requirements

- Apple Mac computer for development (required for iOS app signing)
- iOS device for testing (strongly recommended over simulators)
- Sufficient storage space for development tools and application assets

### Software Requirements

- Apple iOS device running iOS 12 or later (latest version recommended)
- Xcode 12 or later with iOS SDK installed
- Apple Developer account (required for app signing and distribution)
- Visual Studio for Mac, JetBrains Rider, or Visual Studio Code
- .Net 7.0 SDK or later (we recommend the latest stable version)

## Architecture Support

The VisioForge SDK for iOS provides native support for both major iOS device architectures:

### ARM64 Support

- Compatible with all modern iOS devices (iPhone X and newer)
- Optimized native libraries for maximum performance
- Hardware-accelerated video processing where supported by the device

## Installation Process

Follow these steps to properly set up and deploy your VisioForge-powered iOS application:

1. Install the .Net SDK for iOS development
2. Create a new iOS project in your preferred IDE (Visual Studio for Mac or JetBrains Rider recommended)
3. Add the required NuGet packages to your project (detailed in the next section)
4. Configure the necessary permissions and entitlements in your app's Info.plist file
5. Implement your application logic using the VisioForge SDK components
6. Build, sign, and deploy your application to test devices

## NuGet Packages

The VisioForge SDK for iOS is distributed through NuGet packages:

### Core Packages

- [VisioForge.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) - Core package containing core classes and UI controls, including video playback and display components. This is platform-independent and can be used in any .Net project.

### UI Packages

Each UI package has the same VideoView controls but different implementations for the target platform:

#### .Net iOS target platform

- [VisioForge.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) - Contains UI controls and all core classes for the iOS platform.

#### .Net MAUI target platform

- [VisioForge.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI) - Contains UI controls for the MAUI platform.

### Redist Packages

- [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS) - Contains the core redistribution components required for any iOS application using VisioForge technologies.

You can add these packages using the NuGet Package Manager in your IDE or by adding the following to your project file (use the latest versions):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.Core" Version="2025.4.1" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="15.10.11" />
</ItemGroup>
```

Note: Replace version numbers with the latest available releases.

## Required Permissions and Entitlements

iOS applications require explicit permissions for accessing device features like cameras, microphones, and the photo library. Configure these permissions in your app's Info.plist file:

### Camera Access

Required for video capture functionality:

```xml
<key>NSCameraUsageDescription</key>
<string>This app requires camera access for video recording</string>
```

### Microphone Access

Required for audio recording:

```xml
<key>NSMicrophoneUsageDescription</key>
<string>This app requires microphone access for audio recording</string>
```

### Photo Library Access

Required for saving videos to the device's photo library:

```xml
<key>NSPhotoLibraryUsageDescription</key>
<string>This app requires access to the photo library to save videos</string>
```

### Example Info.plist Configuration

Here's a complete example of an Info.plist file with all necessary permissions:

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

## Runtime Permission Handling

In addition to declaring permissions in your Info.plist file, you should also request permissions at runtime. Here's an example of how to request camera and microphone permissions:

```csharp
using System.Diagnostics;
using Photos;

// Request camera permission
private async Task RequestCameraPermissionAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        // Handle permission denial
        Debug.WriteLine("Camera permission denied");
    }
}

// Request microphone permission
private async Task RequestMicrophonePermissionAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Microphone>();
    if (status != PermissionStatus.Granted)
    {
        // Handle permission denial
        Debug.WriteLine("Microphone permission denied");
    }
}

// Request photo library permission (iOS specific)
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

## SDK Initialization

Properly initialize the VisioForge SDK in your application's lifecycle:

```csharp
// In your AppDelegate or application startup code
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    // Initialize the VisioForge SDK
    VisioForge.Core.VisioForgeX.InitSDK();
    
    // Your other initialization code
    
    return true;
}

// Clean up on application termination
public override void WillTerminate(UIApplication application)
{
    // Clean up VisioForge SDK resources
    VisioForge.Core.VisioForgeX.DestroySDK();
    
    // Your other cleanup code
}
```

## Implementation Best Practices

### Using VideoView Controls

The VisioForge SDK provides a `VideoView` control for displaying video content. The VideoView is a UIView subclass, and OpenGL is used for video rendering:

```csharp
// Create a VideoView instance
var videoView = new VisioForge.Core.UI.Apple.VideoView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));
View.AddSubview(videoView);

// Get the IVideoView interface for use with VisioForge components
IVideoView vv = videoView.GetVideoView();

// Use the IVideoView with a VisioForge component
var captureCore = new VideoCaptureCoreX(vv);
```

You can add the VideoView using a storyboard or code.

### Resource Management

iOS devices have limited resources compared to desktop computers. Follow these best practices:

1. Release resources when not in use
2. Use lower resolution settings for real-time processing
3. Implement proper lifecycle management in your ViewControllers
4. Test on actual devices, not just simulators

## Testing and Debugging

### Physical Device Testing

While the iOS simulator can be useful for basic interface testing, it has significant limitations for media applications:

- Simulator may have performance issues during video encoding at high resolutions
- Camera and microphone are not available in the simulator
- Hardware acceleration features may not be available or may behave differently

**Always test your media application on physical iOS devices before release.**

### Common Performance Considerations

When deploying media applications to iOS, consider these performance factors:

1. **Resolution and frame rate:** Lower these settings for better performance on older devices
2. **Encoder selection:** Use hardware-accelerated encoders when available
3. **Memory management:** Implement proper disposal of large objects and monitor memory usage
4. **Battery impact:** Media processing is power-intensive; implement power-saving measures

## Troubleshooting Common Issues

### Permission Denials

If your app can't access the camera or microphone:

1. Verify all required permissions are in your Info.plist
2. Check that you're requesting permissions at runtime before attempting to use the hardware
3. Test if the user has manually denied permissions in iOS Settings

### Library Loading Errors

If you encounter errors loading native libraries:

1. Verify all required NuGet packages are properly installed
2. Check for conflicting package versions
3. Ensure you're targeting the correct iOS architecture (ARM64)

## Additional Resources

- Visit the [VisioForge GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for code samples and example projects
- Browse the [VisioForge API documentation](https://api.visioforge.org/dotnet/api/index.html) for comprehensive SDK reference

---

By following this deployment guide, you should be able to successfully create, configure, and deploy VisioForge-powered applications to iOS devices. For specific questions or advanced configuration needs, please contact VisioForge technical support.
