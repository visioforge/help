---
title: Captura y Procesamiento de Video en Apps .NET MAUI
description: Implemente captura de video en apps .NET MAUI con soporte multiplataforma para grabación de cámara en iOS, Android, macCatalyst y Windows.
---

# Cómo Capturar y Procesar Video en Apps .NET MAUI Usando VisioForge SDK

## Introducción

VisioForge Video Capture SDK para .NET es una solución completa de captura y procesamiento de medios que se integra perfectamente con aplicaciones .NET MAUI. Como un SDK rico en funciones, extiende significativamente las capacidades nativas de .NET MAUI para captura de video en plataformas compatibles incluyendo iOS, Android, macCatalyst y Windows.

En este tutorial, aprenderemos cómo grabar video desde cámaras de hardware en dispositivos iOS/Android y webcams en Windows y macOS. También cubriremos cómo agregar varios efectos y filtros al video, así como cómo guardar el video en el dispositivo usando APIs nativas para cada plataforma.

Para dispositivos móviles (.NET iOS y .NET Android), solicitaremos los permisos necesarios para asegurar la funcionalidad adecuada con las capacidades nativas del dispositivo.

El código de ejemplo completo está disponible en [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI/SimpleCapture) como parte de la documentación del SDK.

## Requisitos Previos

Software y herramientas requeridos:

- Visual Studio o Visual Studio Code con soporte .NET MAUI o JetBrains Rider
- [VisioForge Video Capture SDK para .NET](https://www.visioforge.com/video-capture-sdk-net) (usaremos paquetes NuGet)

## Configurando una Estructura Básica de Proyecto .NET MAUI

### Crear un Nuevo Proyecto .NET MAUI Vacío (.NET 8 o Posterior)

Puede usar Visual Studio, Rider o la herramienta dotnet para crear un nuevo proyecto .NET MAUI con soporte multiplataforma.

### Actualizar Archivo de Proyecto

Agregue targets de runtime para macCatalyst para compatibilidad arm64 y x86_64, lo cual asegura el soporte adecuado de .NET MAUI para entornos Mac:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst' AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64'">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst' AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64'">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

### Paquetes NuGet Principales del SDK

Use las últimas versiones de paquetes NuGet del SDK para .NET en su proyecto.

Agregue los siguientes paquetes NuGet a su proyecto:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2025.3.27" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2025.3.27" />
```

El paquete `VideoCapture` contiene la funcionalidad principal de captura de video, mientras que el paquete `Core.UI.MAUI` contiene el control `VideoView` para vista previa de video en apps .NET MAUI.

### Paquetes NuGet Redistribuibles

Agregue los siguientes paquetes NuGet a su proyecto según la plataforma objetivo:

**Windows:**

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.3.14" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.3.14" />
</ItemGroup>
```

**Android** requiere un paquete NuGet redistribuible y una referencia de proyecto al [proyecto de dependencia Android](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency):

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

**macCatalyst** requiere un paquete NuGet redistribuible y código de target para copiar los archivos redistribuibles al archivo de la app:

```xml
<!-- Paquete NuGet personalizado y código de target para maccatalyst para copiar archivos redistribuibles NuGet al archivo de la app -->
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.2.15" />
</ItemGroup>

<Target Name="CopyNativeLibrariesToMonoBundle" AfterTargets="Build" Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <Message Text="Iniciando target CopyNativeLibrariesToMonoBundle..." Importance="High" />
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
  <Message Text="Archivos nativos copiados:" Importance="High" Condition="@(CopiedNativeFiles) != ''" />
  <Message Text=" - %(CopiedNativeFiles.Identity)" Importance="High" Condition="@(CopiedNativeFiles) != ''" />
  <Message Text="Target CopyNativeLibrariesToMonoBundle finalizado." Importance="High" />
</Target>
```

### Actualizar MauiProgram.cs

Necesitamos agregar soporte de SkiaSharp y código de inicialización de handlers de VisioForge al archivo MauiProgram.cs:

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()

        // Agregar soporte de SkiaSharp
        .UseSkiaSharp()

        // Agregar handlers de VisioForge para VideoView y otros controles
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

### Actualizar Manifest de Android

Agregue permisos adicionales al archivo `AndroidManifest.xml` para funcionalidad nativa de .NET Android:

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

Los permisos CAMERA, RECORD_AUDIO y WRITE_EXTERNAL_STORAGE son requeridos para captura de video en .NET Android. Los permisos INTERNET y ACCESS_NETWORK_STATE son requeridos para acceso a red. Los permisos READ_EXTERNAL_STORAGE y WRITE_EXTERNAL_STORAGE son requeridos para guardar videos en el dispositivo. Los permisos MANAGE_EXTERNAL_STORAGE y MANAGE_MEDIA son requeridos para Android 11 y posteriores.

### Agregar Permisos al Archivo Info.plist de iOS

Configure permisos nativos de iOS en el archivo Info.plist para compatibilidad del SDK iOS:

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
	<string>Se requiere uso de cámara</string>
	<key>NSMicrophoneUsageDescription</key>
	<string>Se requiere uso de micrófono</string>
	<key>NSPhotoLibraryUsageDescription</key>
	<string>Esta app requiere acceso a la biblioteca de fotos para guardar videos.</string>
	<key>CFBundleIdentifier</key>
	<string></string>
</dict>
</plist>
```

NSCameraUsageDescription y NSMicrophoneUsageDescription son requeridos para captura de video en SDK iOS. NSPhotoLibraryUsageDescription es requerido para guardar videos en el dispositivo.

### Actualizar Permisos de macCatalyst

**Archivo Info.plist para Mac Catalyst:**

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
	<string>Se requiere uso de cámara</string>
	<key>NSMicrophoneUsageDescription</key>
	<string>Se requiere uso de micrófono</string>
</dict>
</plist>
```

**Archivo Entitlements.plist para Mac Catalyst:**

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

## Creando la UI

Para ver la vista previa del video, agregue el control `VideoView` a su archivo MainPage.xaml.

Agregue el namespace en la parte superior del archivo:

```xml
xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
```

Agregue el control `VideoView` al contenido de la página:

```xml
<my:VideoView 
    Grid.Row="0"               
    HorizontalOptions="FillAndExpand"
    VerticalOptions="FillAndExpand"
    x:Name="videoView"/>
```

En su código, puede agregar botones para iniciar y detener la grabación, seleccionar cámaras y más.

En este ejemplo, usaremos botones para seleccionar fuentes de video, fuentes de audio y salida de audio.

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
                        <Label Text="CÁMARA" VerticalOptions="Center" HorizontalOptions="Center" Margin="5" TextColor="White" />
                        <Button x:Name="btCamera" Text="CÁMARA" MinimumWidthRequest="200" Clicked="btCamera_Clicked" />
                    </StackLayout>

                    <StackLayout Orientation="Vertical" Grid.Column="1" HorizontalOptions="CenterAndExpand" Margin="5">
                        <Label Text="MICRÓFONO" VerticalOptions="Center" HorizontalOptions="Center" Margin="5" TextColor="White" />
                        <Button x:Name="btMic" Text="MICRÓFONO" MinimumWidthRequest="200" Clicked="btMic_Clicked" />
                    </StackLayout>

                    <StackLayout Orientation="Vertical" Grid.Column="2" HorizontalOptions="EndAndExpand" Margin="5">
                        <Label Text="ALTAVOCES" VerticalOptions="Center" HorizontalOptions="Center" Margin="5" TextColor="White" />
                        <Button x:Name="btSpeakers" Text="ALTAVOCES" MinimumWidthRequest="200" Clicked="btSpeakers_Clicked" />
                    </StackLayout>
                </Grid>

                <StackLayout 
                    Orientation="Horizontal"
                    HorizontalOptions="Center"
                    Margin="5, 0, 5, 0">

                    <Button  
                        x:Name="btStartPreview"   
                        Text="VISTA PREVIA" 
                        Margin="5" 
                        Clicked="btStartPreview_Clicked"
                        MinimumWidthRequest="100" />

                    <Button  
                        x:Name="btStartCapture"   
                        Text="CAPTURAR" 
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

Ahora podemos comenzar a escribir nuestro código fuente para implementar la funcionalidad para nuestra app .NET MAUI con soporte de plataforma nativa.

## Namespaces y Campos

Agregue namespaces a su archivo MainPage.xaml.cs:

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

Agregue los siguientes campos a su archivo MainPage.xaml.cs para manejar capacidades nativas del dispositivo:

```csharp
// Objeto core de captura de video.
private VideoCaptureCoreX _core;

// Dispositivos de captura de video.
private VideoCaptureDeviceInfo[] _cameras;

// Índice de cámara seleccionada.
private int _cameraSelectedIndex = 0;

// Dispositivos de captura de audio.
private AudioCaptureDeviceInfo[] _mics;

// Índice de micrófono seleccionado.
private int _micSelectedIndex = 0;

// Dispositivos de salida de audio.
private AudioOutputDeviceInfo[] _speakers;

// Índice de altavoz seleccionado.
private int _speakerSelectedIndex = 0;

// Color de botón por defecto.
private Color _defaultButtonColor;
```

## Constructor de MainPage

Actualice el constructor y agregue manejadores de eventos:

```csharp
public MainPage()
{
    InitializeComponent();

    Loaded += MainPage_Loaded;
    Unloaded += MainPage_Unloaded;

    this.BindingContext = this;
}
```

## Permisos

Agregue código para solicitar permisos para plataformas móviles:

```csharp
#if __IOS__ && !__MACCATALYST__
private void RequestPhotoPermission()
{
    Photos.PHPhotoLibrary.RequestAuthorization(status =>
    {
        if (status == Photos.PHAuthorizationStatus.Authorized)
        {
            Debug.WriteLine("Acceso a biblioteca de fotos concedido.");
        }
    });
}
#endif

private async Task RequestCameraPermissionAsync()
{
    var result = await Permissions.RequestAsync<Permissions.Camera>();

    // Verificar resultado de solicitud de permiso. Si es permitido por el usuario, conectar al escáner
    if (result == PermissionStatus.Granted)
    {
    }
    else
    {
        if (Permissions.ShouldShowRationale<Permissions.Camera>())
        {
            if (await DisplayAlert(null, "Necesita permitir acceso a la Cámara", "OK", "Cancelar"))
                await RequestCameraPermissionAsync();
        }
    }
}

private async Task RequestMicPermissionAsync()
{
    var result = await Permissions.RequestAsync<Permissions.Microphone>();

    // Verificar resultado de solicitud de permiso. Si es permitido por el usuario, conectar al escáner
    if (result == PermissionStatus.Granted)
    {
    }
    else
    {
        if (Permissions.ShouldShowRationale<Permissions.Microphone>())
        {
            if (await DisplayAlert(null, "Necesita permitir acceso al Micrófono", "OK", "Cancelar"))
                await RequestMicPermissionAsync();
        }
    }
}
```

## Eventos de MainPage

Agregue evento Loaded para inicializar el SDK para apps .NET MAUI:

```csharp
private async void MainPage_Loaded(object sender, EventArgs e)
{
    // Solicitar permisos
#if __ANDROID__ || __MACOS__ || __MACCATALYST__ || __IOS__
    await RequestCameraPermissionAsync();
    await RequestMicPermissionAsync();
#endif

#if __IOS__ && !__MACCATALYST__
    RequestPhotoPermission();
#endif

    // Obtener interfaz IVideoView
    IVideoView vv = videoView.GetVideoView();

    // Crear objeto core con interfaz IVideoView
    _core = new VideoCaptureCoreX(vv);

    // Agregar manejadores de eventos
    _core.OnError += Core_OnError;

    // Enumerar cámaras
    _cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
    if (_cameras.Length > 0)
    {                
        btCamera.Text = _cameras[0].DisplayName;
    }

    // Enumerar micrófonos y otras fuentes de audio
    _mics = await DeviceEnumerator.Shared.AudioSourcesAsync(null);
    if (_mics.Length > 0)
    {      
        btMic.Text = _mics[0].DisplayName;
    }
    
    // Enumerar salidas de audio
    _speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(null);
    if (_speakers.Length > 0)
    {                
        btSpeakers.Text = _speakers[0].DisplayName;
    }

    // Agregar manejador de evento Destroying
    Window.Destroying += Window_Destroying;

#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
    // Seleccionar segunda cámara si está disponible para plataformas móviles
    if (_cameras.Length > 1)
    {
        btCamera.Text = _cameras[1].DisplayName;
        _cameraSelectedIndex = 1;
    }

    // Iniciar vista previa
    btStartCapture.IsEnabled = true;
    await StartPreview();
#endif
}        
```

Agregue evento Unloaded:

```csharp
private void MainPage_Unloaded(object sender, EventArgs e)
{
    // Liberar objeto core
    _core?.Dispose();
    _core = null;

    // Destruir SDK
    VisioForgeX.DestroySDK();
}
```

Agregue manejador de evento Destroying:

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

Agregue manejador de evento de error:

```csharp
private void Core_OnError(object sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
{
    Debug.WriteLine(e.Message);
}
```

Los manejadores de eventos de selección de cámara, micrófono y altavoz están disponibles en el código de GitHub como parte de la documentación del SDK. Los omitiremos aquí por brevedad.

## Implementación de Vista Previa y Captura

Código del botón de inicio:

```csharp
private async Task StartPreview()
{
    if (_core.State == PlaybackState.Play || _core.State == PlaybackState.Pause)
    {
        return;
    }
}
```

Crear configuración de salida de audio para escritorio o deshabilitar para plataformas móviles. `DeviceEnumerator` se usa para obtener dispositivos de salida de audio disponibles:

```csharp
    // salida de audio
#if MOBILE
    _core.Audio_Play = false;
#else
            
    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync()).Where(device => device.DisplayName == btSpeakers.Text).First();
    _core.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);
    _core.Audio_Play = true;
#endif
```

Crear configuración de fuente de video para la cámara seleccionada. `DeviceEnumerator` se usa para obtener fuentes de video disponibles:

`VideoCaptureDeviceSourceSettings` permite especificar dispositivo, formato y orientación. El método `GetHDVideoFormatAndFrameRate` se usa para obtener el formato HD disponible por defecto y velocidad de fotogramas para la cámara seleccionada. Alternativamente, puede enumerar todos los formatos y velocidades de fotogramas disponibles y permitir al usuario seleccionarlos:

```csharp
    // fuente de video
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
        await DisplayAlert("Error", "No se pudo configurar la cámara", "OK");
    }
```

Crear configuración de fuente de audio para el micrófono seleccionado. `DeviceEnumerator` se usa para obtener fuentes de audio disponibles:

```csharp
    // fuente de audio
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

Configurar ajustes de archivo de salida. En este ejemplo, usamos video MP4 y audio MP3. El segundo parámetro de `Outputs_Add` se establece en false para no iniciar la grabación inmediatamente.

Por defecto, se usa codificador H264 GPU para todas las plataformas (.NET iOS, .NET Android, Windows, Mac Catalyst). Como alternativa o en plataformas donde la codificación GPU no está disponible, puede usar un codificador de software.

Para el stream de audio, puede usar codificadores MP3 o AAC.

El SDK soporta una amplia gama de formatos de video y audio y codificadores en todas las plataformas soportadas por .NET MAUI.

```csharp
    // configurar captura
    _core.Audio_Record = true;

    _core.Outputs_Clear();
    _core.Outputs_Add(new MP4Output(GenerateFilename(), H264EncoderBlock.GetDefaultSettings(), new MP3EncoderSettings()), false);
```

Iniciar la vista previa y actualizar etiqueta del botón:

```csharp
    // iniciar
    await _core.StartAsync();

    btStartPreview.Text = "DETENER";
}
```

Ahora podemos implementar el código de los botones Iniciar vista previa, Iniciar captura y Detener:

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

                btStartPreview.Text = "VISTA PREVIA";
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

El método `StartCaptureAsync` se usa para iniciar la captura de video. Usamos el método `GenerateFilename` para generar un nombre de archivo único para cada video:

```csharp
private async void btStartCapture_Clicked(object sender, EventArgs e)
{
    if (_core == null || _core.State != PlaybackState.Play)
    {
        return;
    }

    if (btStartCapture.BackgroundColor != Colors.Red)
    {               
        System.Diagnostics.Debug.WriteLine("Iniciar captura");
        await _core.StartCaptureAsync(0, GenerateFilename());
        btStartCapture.BackgroundColor = Colors.Red;
        btStartCapture.Text = "DETENER";
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

El método `StopCaptureAsync` se usa para detener la captura de video y guardar la grabación en la ubicación específica de la plataforma apropiada:

```csharp
private async Task StopCaptureAsync()
{
    System.Diagnostics.Debug.WriteLine("Detener captura");
    await _core.StopCaptureAsync(0);
    btStartCapture.BackgroundColor = _defaultButtonColor;
    btStartCapture.Text = "CAPTURAR";

    // guardar video a biblioteca de fotos en plataformas nativas iOS y Android
#if __ANDROID__ || (__IOS__ && !__MACCATALYST__)
    string filename = null;
    var output = _core.Outputs_Get(0);
    filename = output.GetFilename();
    await PhotoGalleryHelper.AddVideoToGalleryAsync(filename);
#endif
}
```

## Efectos de Video y Funciones de Mejora

El VisioForge SDK para .NET soporta una amplia gama de efectos de video en todas las plataformas .NET MAUI, incluyendo superposiciones de texto e imagen, chroma key y más, proporcionando funcionalidad consistente en apps iOS MAUI, Mac Catalyst y .NET Android.

En esta guía agregaremos superposiciones opcionales de imagen y texto al video.

Agregue el siguiente código antes de la llamada al método **_core.StartAsync** para agregar superposición de texto:

```csharp
var textOverlay = new OverlayManagerText("Hola mundo", x: 50, y: 50);
_core.Video_Overlay_Add(textOverlay);
```

Copie una imagen PNG o JPEG a la carpeta de assets y agregue el siguiente código para agregar superposición de imagen:

```csharp
using var stream = await FileSystem.OpenAppPackageFileAsync("icon.png");
var bitmap = SkiaSharp.SKBitmap.Decode(stream);
var img = new OverlayManagerImage(bitmap, x: 250, y: 50);
_core.Video_Overlay_Add(img);
```

## Conclusión

Este tutorial demostró cómo integrar el VisioForge Video Capture SDK con aplicaciones .NET MAUI para crear poderosas soluciones de captura de video multiplataforma. El SDK proporciona funcionalidad consistente en Windows, macOS (a través de Mac Catalyst), iOS y plataformas Android mientras aprovecha las capacidades nativas de cada plataforma.

La documentación del SDK proporciona más detalles sobre funciones avanzadas incluyendo:

- Efectos de video y filtros adicionales
- Varios codecs de video y audio
- Streaming de red
- Capacidades de análisis de video
- Pipelines de procesamiento personalizados

Al usar este SDK con .NET MAUI, los desarrolladores pueden crear sofisticadas aplicaciones de video con código mínimo específico de plataforma mientras mantienen la apariencia nativa en cada plataforma.
