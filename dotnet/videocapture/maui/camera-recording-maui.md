---
title: Video Capture & Processing in .NET MAUI Apps
description: Implement video capture in .NET MAUI apps with cross-platform support for iOS, Android, macCatalyst, and Windows camera recording.
---

# How to Capture and Process Video in .NET MAUI Apps Using VisioForge SDK

## Introduction

The VisioForge Video Capture SDK for .NET is a comprehensive media capture and processing solution that integrates seamlessly with .NET MAUI applications. As a feature-rich SDK, it significantly extends .NET MAUI's native capabilities for video capture across supported platforms including iOS, Android, macCatalyst, and Windows.

In this tutorial, we will learn how to record video from hardware cameras on iOS/Android devices and webcams on Windows and macOS. We will also cover how to add various effects and filters to the video, as well as how to save the video to the device using native APIs for each platform.

For mobile devices (.NET iOS and .NET Android), we will request the necessary permissions to ensure proper functionality with the native device capabilities.

The full sample code is available on [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI/SimpleCapture) as part of the SDK documentation.

## Prerequisites

Required software and tools:

- Visual Studio or Visual Studio Code with .NET MAUI support or JetBrains Rider
- [VisioForge Video Capture SDK for .NET](https://www.visioforge.com/video-capture-sdk-net) (we'll use NuGet packages)

## Setting up a Basic .NET MAUI Project Structure

### Create a New Empty .NET MAUI Project (.NET 8 or Later)

You can use Visual Studio, Rider, or dotnet tool to create a new .NET MAUI project with cross-platform support.

### Update Project File

Add runtime targets for macCatalyst for arm64 and x86_64 compatibility, which ensures proper .NET MAUI support for Mac environments:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst' AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64'">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst' AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64'">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

### NuGet Main SDK Packages

Use the latest NuGet package versions of the SDK for .NET in your project.

Add the following NuGet packages to your project:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2025.3.27" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2025.3.27" />
```

The `VideoCapture` package contains the main video capture functionality, while the `Core.UI.MAUI` package contains the `VideoView` control for video preview in .NET MAUI apps.

### NuGet Redist Packages

Add the following NuGet packages to your project based on target platform:

**Windows:**

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.3.14" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.3.14" />
</ItemGroup>
```

**Android** requires a NuGet redist package and a project reference to the [Android dependency project](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.24" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X8.csproj" />
</ItemGroup>
```

**iOS:**

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
```

**macCatalyst** requires a NuGet redist package and a target code to copy the redist files to the app file:

```xml
<!-- Custom NuGet package and target code for maccatalyst to copy NuGet redist files to app file -->
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.2.15" />
</ItemGroup>

<Target Name="CopyNativeLibrariesToMonoBundle" AfterTargets="Build" Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <Message Text="Starting CopyNativeLibrariesToMonoBundle target..." Importance="High" />
  <PropertyGroup>
    <AppBundleDir>$(OutputPath)$(AssemblyName).app</AppBundleDir>
    <MonoBundleDir>$(AppBundleDir)/Contents/MonoBundle</MonoBundleDir>
  </PropertyGroup>
  <Message Text="AppBundleDir: $(AppBundleDir)" Importance="High" />
  <Message Text="MonoBundleDir: $(MonoBundleDir)" Importance="High" />
  <MakeDir Directories="$(MonoBundleDir)" Condition="!Exists('$(MonoBundleDir)')" />
  <Copy SourceFiles="@(None-&gt;'%(FullPath)')" DestinationFolder="$(MonoBundleDir)" Condition="'%(Extension)' == '.dylib' Or '%(Extension)' == '.so'">
    <Output TaskParameter="CopiedFiles" ItemName="CopiedNativeFiles" />
  </Copy>
  <Message Text="Copied native files:" Importance="High" Condition="@(CopiedNativeFiles) != ''" />
  <Message Text=" - %(CopiedNativeFiles.Identity)" Importance="High" Condition="@(CopiedNativeFiles) != ''" />
  <Message Text="Finished CopyNativeLibrariesToMonoBundle target." Importance="High" />
</Target>
```

### Update MauiProgram.cs

We need to add SkiaSharp support and VisioForge handlers initialization code to the MauiProgram.cs file:

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()

        // Add SkiaSharp support
        .UseSkiaSharp()

        // Add VisioForge handlers for VideoView and other controls
        .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())  

        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
}
```

### Update Android Manifest

Add additional permissions to the `AndroidManifest.xml` file for native .NET Android functionality:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
  <application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true"></application>
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.CAMERA" />
  <uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.MANAGE_MEDIA" />
  <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.RECORD_AUDIO" />
  <uses-sdk android:minSdkVersion="23" android:targetSdkVersion="33" />
  <uses-feature android:name="android.hardware.camera" android:required="false" />
  <uses-feature android:name="android.hardware.camera.autofocus" />
</manifest>
```

CAMERA, RECORD_AUDIO, and WRITE_EXTERNAL_STORAGE permissions are required for video capture on .NET Android. INTERNET and ACCESS_NETWORK_STATE permissions are required for network access. READ_EXTERNAL_STORAGE and WRITE_EXTERNAL_STORAGE permissions are required to save videos to the device. MANAGE_EXTERNAL_STORAGE and MANAGE_MEDIA permissions are required for Android 11 and later.

### Add Permissions to iOS Info.plist File

Configure native iOS permissions in the Info.plist file for SDK iOS compatibility:

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
	<string>Camera usage required</string>
	<key>NSMicrophoneUsageDescription</key>
	<string>Mic usage required</string>
	<key>NSPhotoLibraryUsageDescription</key>
	<string>This app requires access to the photos library to save videos.</string>
	<key>CFBundleIdentifier</key>
	<string></string>
</dict>
</plist>
```

NSCameraUsageDescription and NSMicrophoneUsageDescription are required for video capture on SDK iOS. NSPhotoLibraryUsageDescription is required to save videos to the device.

### Update macCatalyst Permissions

**Info.plist file for Mac Catalyst:**

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
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
	<string>Camera usage required</string>
	<key>NSMicrophoneUsageDescription</key>
	<string>Mic usage required</string>
</dict>
</plist>
```

**Entitlements.plist file for Mac Catalyst:**

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>com.apple.security.device.audio-input</key>
	<true/>
	<key>com.apple.security.device.camera</key>
	<true/>
	<key>com.apple.security.device.usb</key>
	<true/>
</dict>
</plist>
```

## Creating UI

To see the video preview, add the `VideoView` control to your MainPage.xaml file.

Add the namespace to the top of the file:

```xml
xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
```

Add the `VideoView` control to the content of the page:

```xml
<my:VideoView 
    Grid.Row="0"               
    HorizontalOptions="FillAndExpand"
    VerticalOptions="FillAndExpand"
    x:Name="videoView"/>
```

In your code, you can add buttons to start and stop recording, select cameras, and more.

In this sample, we'll use buttons to select source video, audio sources, and audio output.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="SimpleCapture.MainPage"
             xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI">

    <ScrollView>
        <Grid RowDefinitions="*,Auto" >

            <my:VideoView 
                Grid.Row="0"               
                HorizontalOptions="FillAndExpand"
                VerticalOptions="FillAndExpand"
                x:Name="videoView"/>

            <StackLayout Grid.Row="1" x:Name="pnMain" Orientation="Vertical" HorizontalOptions="Fill" Background="#1e1e1e">

                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>

                    <StackLayout Orientation="Vertical" Grid.Column="0" HorizontalOptions="StartAndExpand" Margin="5">
                        <Label Text="CAMERA" VerticalOptions="Center" HorizontalOptions="Center" Margin="5" TextColor="White" />
                        <Button x:Name="btCamera" Text="CAMERA" MinimumWidthRequest="200" Clicked="btCamera_Clicked" />
                    </StackLayout>

                    <StackLayout Orientation="Vertical" Grid.Column="1" HorizontalOptions="CenterAndExpand" Margin="5">
                        <Label Text="MICROPHONE" VerticalOptions="Center" HorizontalOptions="Center" Margin="5" TextColor="White" />
                        <Button x:Name="btMic" Text="MICROPHONE" MinimumWidthRequest="200" Clicked="btMic_Clicked" />
                    </StackLayout>

                    <StackLayout Orientation="Vertical" Grid.Column="2" HorizontalOptions="EndAndExpand" Margin="5">
                        <Label Text="SPEAKERS" VerticalOptions="Center" HorizontalOptions="Center" Margin="5" TextColor="White" />
                        <Button x:Name="btSpeakers" Text="SPEAKERS" MinimumWidthRequest="200" Clicked="btSpeakers_Clicked" />
                    </StackLayout>
                </Grid>

                <StackLayout 
                    Orientation="Horizontal"
                    HorizontalOptions="Center"
                    Margin="5, 0, 5, 0">

                    <Button  
                        x:Name="btStartPreview"   
                        Text="PREVIEW" 
                        Margin="5" 
                        Clicked="btStartPreview_Clicked"
                        MinimumWidthRequest="100" />

                    <Button  
                        x:Name="btStartCapture"   
                        Text="CAPTURE" 
                        Margin="5" 
                        IsEnabled="False"
                        Clicked="btStartCapture_Clicked"
                        MinimumWidthRequest="100" />
                </StackLayout>
            </StackLayout>
        </Grid>
    </ScrollView>

</ContentPage>
```

Now we can start writing our source code to implement the functionality for our .NET MAUI app with native platform support.

## Namespaces and Fields

Add namespaces to your MainPage.xaml.cs file:

```csharp
#if (__IOS__ && !__MACCATALYST__) || __ANDROID__
#define MOBILE
#endif

using System;
using System.ComponentModel;
using System.Diagnostics;

using VisioForge.Core;
using VisioForge.Core.Helpers;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;
using VisioForge.Core.VideoCaptureX;
```

Add the following fields to your MainPage.xaml.cs file to handle native device capabilities:

```csharp
// Video capture core object.
private VideoCaptureCoreX _core;

// Video capture devices.
private VideoCaptureDeviceInfo[] _cameras;

// Selected camera index.
private int _cameraSelectedIndex = 0;

// Audio capture devices.
private AudioCaptureDeviceInfo[] _mics;

// Selected microphone index.
private int _micSelectedIndex = 0;

// Audio output devices.
private AudioOutputDeviceInfo[] _speakers;

// Selected speaker index.
private int _speakerSelectedIndex = 0;

// Default button color.
private Color _defaultButtonColor;
```

## MainPage Constructor

Update constructor and add event handlers:

```csharp
public MainPage()
{
    InitializeComponent();

    Loaded += MainPage_Loaded;
    Unloaded += MainPage_Unloaded;

    this.BindingContext = this;
}
```

## Permissions

Add code to request permissions for mobile platforms:

```csharp
#if __IOS__ && !__MACCATALYST__
private void RequestPhotoPermission()
{
    Photos.PHPhotoLibrary.RequestAuthorization(status =>
    {
        if (status == Photos.PHAuthorizationStatus.Authorized)
        {
            Debug.WriteLine("Photo library access granted.");
        }
    });
}
#endif

private async Task RequestCameraPermissionAsync()
{
    var result = await Permissions.RequestAsync<Permissions.Camera>();

    // Check result from permission request. If it is allowed by the user, connect to scanner
    if (result == PermissionStatus.Granted)
    {
    }
    else
    {
        if (Permissions.ShouldShowRationale<Permissions.Camera>())
        {
            if (await DisplayAlert(null, "You need to allow access to the Camera", "OK", "Cancel"))
                await RequestCameraPermissionAsync();
        }
    }
}

private async Task RequestMicPermissionAsync()
{
    var result = await Permissions.RequestAsync<Permissions.Microphone>();

    // Check result from permission request. If it is allowed by the user, connect to scanner
    if (result == PermissionStatus.Granted)
    {
    }
    else
    {
        if (Permissions.ShouldShowRationale<Permissions.Microphone>())
        {
            if (await DisplayAlert(null, "You need to allow access to the Microphone", "OK", "Cancel"))
                await RequestMicPermissionAsync();
        }
    }
}
```

## MainPage Events

Add Loaded event to initialize the SDK for .NET MAUI apps:

```csharp
private async void MainPage_Loaded(object sender, EventArgs e)
{
    // Ask for permissions
#if __ANDROID__ || __MACOS__ || __MACCATALYST__ || __IOS__
    await RequestCameraPermissionAsync();
    await RequestMicPermissionAsync();
#endif

#if __IOS__ && !__MACCATALYST__
    RequestPhotoPermission();
#endif

    // Get IVideoView interface
    IVideoView vv = videoView.GetVideoView();

    // Create core object with IVideoView interface
    _core = new VideoCaptureCoreX(vv);

    // Add event handlers
    _core.OnError += Core_OnError;

    // Enumerate cameras
    _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
    if (_cameras.Length > 0)
    {                
        btCamera.Text = _cameras[0].DisplayName;
    }

    // Enumerate microphones and other audio sources
    _mics = await DeviceEnumerator.Shared.AudioSourcesAsync(null);
    if (_mics.Length > 0)
    {      
        btMic.Text = _mics[0].DisplayName;
    }
    
    // Enumerate audio outputs
    _speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(null);
    if (_speakers.Length > 0)
    {                
        btSpeakers.Text = _speakers[0].DisplayName;
    }

    // Add Destroying event handler
    Window.Destroying += Window_Destroying;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
    // Select second camera if available for mobile platforms
    if (_cameras.Length > 1)
    {
        btCamera.Text = _cameras[1].DisplayName;
        _cameraSelectedIndex = 1;
    }

    // Start preview
    btStartCapture.IsEnabled = true;
    await StartPreview();
#endif
}        
```

Add Unloaded event:

```csharp
private void MainPage_Unloaded(object sender, EventArgs e)
{
    // Dispose core object
    _core?.Dispose();
    _core = null;

    // Destroy SDK
    VisioForgeX.DestroySDK();
}
```

Add Destroying event handler:

```csharp
private async void Window_Destroying(object sender, EventArgs e)
{
    if (_core != null)
    {
        _core.OnError -= Core_OnError;
        await _core.StopAsync();

        _core?.Dispose();
        _core = null;
    }

    VisioForgeX.DestroySDK();
}
```

Add error event handler:

```csharp
private void Core_OnError(object sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
{
    Debug.WriteLine(e.Message);
}
```

Camera, microphone, and speaker selection event handlers are available in the GitHub code as part of the SDK documentation. We'll skip them here for brevity.

## Preview and Capture Implementation

Start button code:

```csharp
private async Task StartPreview()
{
    if (_core.State == PlaybackState.Play || _core.State == PlaybackState.Pause)
    {
        return;
    }
}
```

Create audio output settings for desktop or disable for mobile platforms. `DeviceEnumerator` is used to get available audio output devices:

```csharp
    // audio output
#if MOBILE
    _core.Audio_Play = false;
#else
            
    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync()).Where(device => device.DisplayName == btSpeakers.Text).First();
    _core.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);
    _core.Audio_Play = true;
#endif
```

Create video source settings for the selected camera. `DeviceEnumerator` is used to get available video sources:

`VideoCaptureDeviceSourceSettings` allows you to specify device, format, and orientation. The `GetHDVideoFormatAndFrameRate` method is used to get the default HD available format and frame rate for the selected camera. Alternatively, you can enumerate all available formats and frame rates and let the user select them:

```csharp
    // video source
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;

    var deviceName = btCamera.Text;
    if (!string.IsNullOrEmpty(deviceName))
    {
        var device = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
        if (device != null)
        {
            var formatItem = device.GetHDVideoFormatAndFrameRate(out var frameRate);
            if (formatItem != null)
            {
                videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
                {
                    Format = formatItem.ToFormat()
                };

                videoSourceSettings.Format.FrameRate = frameRate;
            }
        }
    }

    _core.Video_Source = videoSourceSettings;

#if __IOS__ && !__MACCATALYST__
    videoSourceSettings.Orientation = IOSVideoSourceOrientation.LandscapeRight;
#endif

    if (videoSourceSettings == null)
    {
        await DisplayAlert("Error", "Unable to configure camera settings", "OK");
    }
```

Create audio source settings for the selected microphone. `DeviceEnumerator` is used to get available audio sources:

```csharp
    // audio source
    IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;

    deviceName = btMic.Text;
    if (!string.IsNullOrEmpty(deviceName))
    {
        var device = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
        if (device != null)
        {
            var formatItem = device.GetDefaultFormat();
            audioSourceSettings = device.CreateSourceSettingsVC(formatItem);
        }
    }

    _core.Audio_Source = audioSourceSettings;
```

Configure output file settings. In this example, we use MP4 video and MP3 audio. The `Outputs_Add` second parameter is set to false to not start recording immediately.

By default, H264 GPU encoder is used for all platforms (.NET iOS, .NET Android, Windows, Mac Catalyst). As an alternative or on platforms where GPU encoding is not available, you can use a software encoder.

For audio stream, you can use MP3 or AAC encoders.

The SDK supports a wide range of video and audio formats and encoders across all .NET MAUI supported platforms.

```csharp
    // configure capture
    _core.Audio_Record = true;

    _core.Outputs_Clear();
    _core.Outputs_Add(new MP4Output(GenerateFilename(), H264EncoderBlock.GetDefaultSettings(), new MP3EncoderSettings()), false);
```

Start the preview and update button label:

```csharp
    // start
    await _core.StartAsync();

    btStartPreview.Text = "STOP";
}
```

Now we can implement Start preview, Start capture, and Stop buttons code:

```csharp
private async void btStartPreview_Clicked(object sender, EventArgs e)
{
    if (_core == null)
    {
        return;
    }

    switch (_core.State)
    {
        case PlaybackState.Play:
            {
                await StopAllAsync();

                btStartPreview.Text = "PREVIEW";
                btStartCapture.IsEnabled = false;
            }

            break;
        case PlaybackState.Free:
            {
                if (_core.State == PlaybackState.Play || _core.State == PlaybackState.Pause)
                {
                    return;
                }

                await StartPreview();
                btStartCapture.IsEnabled = true;
            }

            break;
        default:
            break;
    }
}
```

The `StartCaptureAsync` method is used to start video capture. We use the `GenerateFilename` method to generate a unique filename for each video:

```csharp
private async void btStartCapture_Clicked(object sender, EventArgs e)
{
    if (_core == null || _core.State != PlaybackState.Play)
    {
        return;
    }

    if (btStartCapture.BackgroundColor != Colors.Red)
    {               
        System.Diagnostics.Debug.WriteLine("Start capture");
        await _core.StartCaptureAsync(0, GenerateFilename());
        btStartCapture.BackgroundColor = Colors.Red;
        btStartCapture.Text = "STOP";
    }
    else
    {
        await StopCaptureAsync();
    }
}

private string GenerateFilename()
{
    DateTime now = DateTime.Now;
#if __ANDROID__
    var filename = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath, "Camera", $"visioforge_{now.Hour}_{now.Minute}_{now.Second}.mp4");
#elif __IOS__ && !__MACCATALYST__
    var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..",
                "Library", $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#elif __MACCATALYST__
    var filename = Path.Combine("/tmp", $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#else
    var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"{now.Hour}_{now.Minute}_{now.Second}.mp4");
#endif

    return filename;
}
```

The `StopCaptureAsync` method is used to stop video capture and save the recording to the appropriate platform-specific location:

```csharp
private async Task StopCaptureAsync()
{
    System.Diagnostics.Debug.WriteLine("Stop capture");
    await _core.StopCaptureAsync(0);
    btStartCapture.BackgroundColor = _defaultButtonColor;
    btStartCapture.Text = "CAPTURE";

    // save video to photo library on native iOS and Android platforms
#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
    string filename = null;
    var output = _core.Outputs_Get(0);
    filename = output.GetFilename();
    await PhotoGalleryHelper.AddVideoToGalleryAsync(filename);
#endif
}
```

## Video Effects and Enhancement Features

The VisioForge SDK for .NET supports a wide range of video effects across all .NET MAUI platforms, including text and image overlays, chroma key, and more, providing consistent functionality in iOS MAUI, Mac Catalyst, and .NET Android apps.

In this guide we'll add optional image and text overlays to the video.

Add the following code before the **_core.StartAsync** method call to add text overlay:

```csharp
var textOverlay = new OverlayManagerText("Hello world", x: 50, y: 50);
_core.Video_Overlay_Add(textOverlay);
```

Copy PNG or JPEG image to the assets folder and add the following code to add image overlay: 

```csharp
using var stream = await FileSystem.OpenAppPackageFileAsync("icon.png");
var bitmap = SkiaSharp.SKBitmap.Decode(stream);
var img = new OverlayManagerImage(bitmap, x: 250, y: 50);
_core.Video_Overlay_Add(img);
```

## Conclusion

This tutorial demonstrated how to integrate the VisioForge Video Capture SDK with .NET MAUI applications to create powerful cross-platform video capture solutions. The SDK provides consistent functionality across Windows, macOS (through Mac Catalyst), iOS, and Android platforms while leveraging native capabilities of each platform.

The SDK documentation provides more details on advanced features including:

- Additional video effects and filters
- Various video and audio codecs
- Network streaming
- Video analysis capabilities
- Custom processing pipelines

By using this SDK with .NET MAUI, developers can create sophisticated video applications with minimal platform-specific code while still maintaining the native look and feel on each platform.
