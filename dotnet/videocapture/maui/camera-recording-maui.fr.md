---
title: Enregistrement caméra en .NET MAUI — iOS, Android, Windows
description: Enregistrez la vidéo depuis les caméras d'appareils en .NET MAUI avec VisioForge Video Capture. Capture pour iOS, Android, macCatalyst, Windows.
tags:
  - Video Capture SDK
  - .NET
  - C++
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Capture
  - Streaming
  - Encoding
  - Effects
  - MP4
  - H.264
  - MP3
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - DeviceEnumerator
  - VideoView
  - IVideoView
  - VideoCaptureDeviceSourceSettings
  - VideoCaptureCoreX

---

# Comment capturer et traiter la vidéo dans des applications .NET MAUI avec le SDK VisioForge

## Introduction

Le VisioForge Video Capture SDK pour .NET est une solution complète de capture et de traitement multimédia qui s'intègre de manière transparente aux applications .NET MAUI. En tant que SDK riche en fonctionnalités, il étend considérablement les capacités natives de .NET MAUI pour la capture vidéo sur les plateformes prises en charge, notamment iOS, Android, macCatalyst et Windows.

Dans ce tutoriel, nous apprendrons comment enregistrer la vidéo depuis les caméras matérielles sur les appareils iOS/Android et les webcams sur Windows et macOS. Nous couvrirons également comment ajouter divers effets et filtres à la vidéo, ainsi que comment sauvegarder la vidéo sur l'appareil en utilisant les API natives de chaque plateforme.

Pour les appareils mobiles (.NET iOS et .NET Android), nous demanderons les permissions nécessaires pour assurer le bon fonctionnement avec les capacités natives de l'appareil.

Le code source complet est disponible sur [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI/SimpleCapture) dans le cadre de la documentation du SDK.

## Prérequis

Logiciels et outils requis :

- Visual Studio ou Visual Studio Code avec la prise en charge de .NET MAUI, ou JetBrains Rider
- [VisioForge Video Capture SDK pour .NET](https://www.visioforge.com/video-capture-sdk-net) (nous utiliserons les paquets NuGet)

## Configuration d'une structure de projet .NET MAUI basique

### Créer un nouveau projet .NET MAUI vide (.NET 8 ou supérieur)

Vous pouvez utiliser Visual Studio, Rider ou l'outil dotnet pour créer un nouveau projet .NET MAUI avec une prise en charge multiplateforme.

### Mettre à jour le fichier projet

Ajoutez les cibles d'exécution pour macCatalyst pour la compatibilité arm64 et x86_64, ce qui garantit une bonne prise en charge .NET MAUI pour les environnements Mac :

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst' AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64'">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst' AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64'">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

### Paquets NuGet principaux du SDK

Utilisez les dernières versions des paquets NuGet du SDK pour .NET dans votre projet.

Ajoutez les paquets NuGet suivants à votre projet :

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2025.3.27" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2025.3.27" />
```

Le paquet `VideoCapture` contient la fonctionnalité principale de capture vidéo, tandis que le paquet `Core.UI.MAUI` contient le contrôle `VideoView` pour l'aperçu vidéo dans les applications .NET MAUI.

### Paquets NuGet redist

Ajoutez les paquets NuGet suivants à votre projet selon la plateforme cible :

**Windows :**

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.3.14" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.3.14" />
</ItemGroup>
```

**Android** nécessite un paquet NuGet redist et une référence de projet vers le [projet de dépendance Android](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency) :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.24" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X8.csproj" />
</ItemGroup>
```

**iOS :**

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
```

**macCatalyst** nécessite un paquet NuGet redist et un code cible pour copier les fichiers redist dans le fichier d'application :

```xml
<!-- Paquet NuGet personnalisé et code cible pour maccatalyst pour copier les fichiers redist NuGet vers le fichier d'application -->
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

### Mettre à jour MauiProgram.cs

Nous devons ajouter la prise en charge de SkiaSharp et le code d'initialisation des gestionnaires VisioForge au fichier MauiProgram.cs :

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()

        // Ajouter la prise en charge de SkiaSharp
        .UseSkiaSharp()

        // Ajouter les gestionnaires VisioForge pour VideoView et autres contrôles
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

### Mettre à jour le manifeste Android

Ajoutez des permissions supplémentaires au fichier `AndroidManifest.xml` pour la fonctionnalité native .NET Android :

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

Les permissions CAMERA, RECORD_AUDIO et WRITE_EXTERNAL_STORAGE sont requises pour la capture vidéo sur .NET Android. Les permissions INTERNET et ACCESS_NETWORK_STATE sont requises pour l'accès réseau. Les permissions READ_EXTERNAL_STORAGE et WRITE_EXTERNAL_STORAGE sont requises pour sauvegarder les vidéos sur l'appareil. Les permissions MANAGE_EXTERNAL_STORAGE et MANAGE_MEDIA sont requises pour Android 11 et supérieur.

### Ajouter les permissions au fichier Info.plist d'iOS

Configurez les permissions natives iOS dans le fichier Info.plist pour la compatibilité SDK iOS :

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

NSCameraUsageDescription et NSMicrophoneUsageDescription sont requis pour la capture vidéo sur SDK iOS. NSPhotoLibraryUsageDescription est requis pour sauvegarder les vidéos sur l'appareil.

### Mettre à jour les permissions macCatalyst

**Fichier Info.plist pour Mac Catalyst :**

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

**Fichier Entitlements.plist pour Mac Catalyst :**

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

## Création de l'interface utilisateur

Pour voir l'aperçu vidéo, ajoutez le contrôle `VideoView` à votre fichier MainPage.xaml.

Ajoutez l'espace de noms en haut du fichier :

```xml
xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
```

Ajoutez le contrôle `VideoView` au contenu de la page :

```xml
<my:VideoView 
    Grid.Row="0"               
    HorizontalOptions="FillAndExpand"
    VerticalOptions="FillAndExpand"
    x:Name="videoView"/>
```

Dans votre code, vous pouvez ajouter des boutons pour démarrer et arrêter l'enregistrement, sélectionner des caméras et plus encore.

Dans cet exemple, nous utiliserons des boutons pour sélectionner la source vidéo, les sources audio et la sortie audio.

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

Nous pouvons maintenant commencer à écrire notre code source pour implémenter la fonctionnalité de notre application .NET MAUI avec prise en charge native de la plateforme.

## Espaces de noms et champs

Ajoutez les espaces de noms à votre fichier MainPage.xaml.cs :

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

Ajoutez les champs suivants à votre fichier MainPage.xaml.cs pour gérer les capacités natives de l'appareil :

```csharp
// Objet principal de capture vidéo.
private VideoCaptureCoreX _core;

// Périphériques de capture vidéo.
private VideoCaptureDeviceInfo[] _cameras;

// Index de la caméra sélectionnée.
private int _cameraSelectedIndex = 0;

// Périphériques de capture audio.
private AudioCaptureDeviceInfo[] _mics;

// Index du microphone sélectionné.
private int _micSelectedIndex = 0;

// Périphériques de sortie audio.
private AudioOutputDeviceInfo[] _speakers;

// Index du haut-parleur sélectionné.
private int _speakerSelectedIndex = 0;

// Couleur de bouton par défaut.
private Color _defaultButtonColor;
```

## Constructeur de MainPage

Mettez à jour le constructeur et ajoutez les gestionnaires d'événements :

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

Ajoutez du code pour demander les permissions pour les plateformes mobiles :

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

    // Vérifier le résultat de la demande de permission. Si elle est accordée par l'utilisateur, se connecter au scanner
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

    // Vérifier le résultat de la demande de permission. Si elle est accordée par l'utilisateur, se connecter au scanner
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

## Événements de MainPage

Ajoutez l'événement Loaded pour initialiser le SDK pour les applications .NET MAUI :

```csharp
private async void MainPage_Loaded(object sender, EventArgs e)
{
    // Demander les permissions
#if __ANDROID__ || __MACOS__ || __MACCATALYST__ || __IOS__
    await RequestCameraPermissionAsync();
    await RequestMicPermissionAsync();
#endif

#if __IOS__ && !__MACCATALYST__
    RequestPhotoPermission();
#endif

    // Obtenir l'interface IVideoView
    IVideoView vv = videoView.GetVideoView();

    // Créer l'objet core avec l'interface IVideoView
    _core = new VideoCaptureCoreX(vv);

    // Ajouter les gestionnaires d'événements
    _core.OnError += Core_OnError;

    // Énumérer les caméras
    _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
    if (_cameras.Length > 0)
    {                
        btCamera.Text = _cameras[0].DisplayName;
    }

    // Énumérer les microphones et autres sources audio
    _mics = await DeviceEnumerator.Shared.AudioSourcesAsync(null);
    if (_mics.Length > 0)
    {      
        btMic.Text = _mics[0].DisplayName;
    }
    
    // Énumérer les sorties audio
    _speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(null);
    if (_speakers.Length > 0)
    {                
        btSpeakers.Text = _speakers[0].DisplayName;
    }

    // Ajouter le gestionnaire d'événements Destroying
    Window.Destroying += Window_Destroying;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
    // Sélectionner la deuxième caméra si disponible pour les plateformes mobiles
    if (_cameras.Length > 1)
    {
        btCamera.Text = _cameras[1].DisplayName;
        _cameraSelectedIndex = 1;
    }

    // Démarrer l'aperçu
    btStartCapture.IsEnabled = true;
    await StartPreview();
#endif
}        
```

Ajoutez l'événement Unloaded :

```csharp
private void MainPage_Unloaded(object sender, EventArgs e)
{
    // Libérer l'objet core
    _core?.Dispose();
    _core = null;

    // Détruire le SDK
    VisioForgeX.DestroySDK();
}
```

Ajoutez le gestionnaire d'événement Destroying :

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

Ajoutez le gestionnaire d'événement d'erreur :

```csharp
private void Core_OnError(object sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
{
    Debug.WriteLine(e.Message);
}
```

Les gestionnaires d'événements de sélection de caméra, microphone et haut-parleur sont disponibles dans le code GitHub dans le cadre de la documentation du SDK. Nous les passons ici pour la concision.

## Implémentation de l'aperçu et de la capture

Code du bouton Démarrer :

```csharp
private async Task StartPreview()
{
    if (_core.State == PlaybackState.Play || _core.State == PlaybackState.Pause)
    {
        return;
    }
}
```

Créez les paramètres de sortie audio pour le bureau ou désactivez pour les plateformes mobiles. `DeviceEnumerator` est utilisé pour obtenir les périphériques de sortie audio disponibles :

```csharp
    // sortie audio
#if MOBILE
    _core.Audio_Play = false;
#else
            
    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync()).Where(device => device.DisplayName == btSpeakers.Text).First();
    _core.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);
    _core.Audio_Play = true;
#endif
```

Créez les paramètres de source vidéo pour la caméra sélectionnée. `DeviceEnumerator` est utilisé pour obtenir les sources vidéo disponibles :

`VideoCaptureDeviceSourceSettings` vous permet de spécifier le périphérique, le format et l'orientation. La méthode `GetHDVideoFormatAndFrameRate` est utilisée pour obtenir le format HD disponible par défaut et la fréquence d'images pour la caméra sélectionnée. Alternativement, vous pouvez énumérer tous les formats et fréquences d'images disponibles et laisser l'utilisateur les sélectionner :

```csharp
    // source vidéo
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

Créez les paramètres de source audio pour le microphone sélectionné. `DeviceEnumerator` est utilisé pour obtenir les sources audio disponibles :

```csharp
    // source audio
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

Configurez les paramètres du fichier de sortie. Dans cet exemple, nous utilisons MP4 vidéo et MP3 audio. Le deuxième paramètre `Outputs_Add` est défini à false pour ne pas démarrer l'enregistrement immédiatement.

Par défaut, l'encodeur GPU H264 est utilisé pour toutes les plateformes (.NET iOS, .NET Android, Windows, Mac Catalyst). En alternative ou sur les plateformes où l'encodage GPU n'est pas disponible, vous pouvez utiliser un encodeur logiciel.

Pour le flux audio, vous pouvez utiliser les encodeurs MP3 ou AAC.

Le SDK prend en charge un large éventail de formats vidéo et audio et d'encodeurs sur toutes les plateformes prises en charge par .NET MAUI.

```csharp
    // configurer la capture
    _core.Audio_Record = true;

    _core.Outputs_Clear();
    _core.Outputs_Add(new MP4Output(GenerateFilename(), H264EncoderBlock.GetDefaultSettings(), new MP3EncoderSettings()), false);
```

Démarrez l'aperçu et mettez à jour le libellé du bouton :

```csharp
    // démarrer
    await _core.StartAsync();

    btStartPreview.Text = "STOP";
}
```

Nous pouvons maintenant implémenter le code des boutons Démarrer aperçu, Démarrer capture et Arrêter :

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

La méthode `StartCaptureAsync` est utilisée pour démarrer la capture vidéo. Nous utilisons la méthode `GenerateFilename` pour générer un nom de fichier unique pour chaque vidéo :

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

La méthode `StopCaptureAsync` est utilisée pour arrêter la capture vidéo et sauvegarder l'enregistrement dans l'emplacement spécifique à la plateforme :

```csharp
private async Task StopCaptureAsync()
{
    System.Diagnostics.Debug.WriteLine("Stop capture");
    await _core.StopCaptureAsync(0);
    btStartCapture.BackgroundColor = _defaultButtonColor;
    btStartCapture.Text = "CAPTURE";

    // sauvegarder la vidéo dans la photothèque sur les plateformes natives iOS et Android
#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
    string filename = null;
    var output = _core.Outputs_Get(0);
    filename = output.GetFilename();
    await PhotoGalleryHelper.AddVideoToGalleryAsync(filename);
#endif
}
```

## Effets vidéo et fonctionnalités d'amélioration

Le SDK VisioForge pour .NET prend en charge un large éventail d'effets vidéo sur toutes les plateformes .NET MAUI, y compris les superpositions de texte et d'image, le chroma key et plus encore, offrant une fonctionnalité cohérente dans les applications iOS MAUI, Mac Catalyst et .NET Android.

Dans ce guide, nous ajouterons des superpositions optionnelles d'image et de texte à la vidéo.

Ajoutez le code suivant avant l'appel à la méthode **_core.StartAsync** pour ajouter une superposition de texte :

```csharp
var textOverlay = new OverlayManagerText("Hello world", x: 50, y: 50);
_core.Video_Overlay_Add(textOverlay);
```

Copiez une image PNG ou JPEG dans le dossier assets et ajoutez le code suivant pour ajouter une superposition d'image : 

```csharp
using var stream = await FileSystem.OpenAppPackageFileAsync("icon.png");
var bitmap = SkiaSharp.SKBitmap.Decode(stream);
var img = new OverlayManagerImage(bitmap, x: 250, y: 50);
_core.Video_Overlay_Add(img);
```

## Conclusion

Ce tutoriel a montré comment intégrer le VisioForge Video Capture SDK avec les applications .NET MAUI pour créer de puissantes solutions de capture vidéo multiplateformes. Le SDK fournit une fonctionnalité cohérente sur les plateformes Windows, macOS (via Mac Catalyst), iOS et Android tout en tirant parti des capacités natives de chaque plateforme.

La documentation du SDK fournit plus de détails sur les fonctionnalités avancées, notamment :

- Effets vidéo et filtres supplémentaires
- Divers codecs vidéo et audio
- Streaming réseau
- Capacités d'analyse vidéo
- Pipelines de traitement personnalisés

En utilisant ce SDK avec .NET MAUI, les développeurs peuvent créer des applications vidéo sophistiquées avec un minimum de code spécifique à la plateforme tout en maintenant l'apparence et la convivialité natives sur chaque plateforme.
