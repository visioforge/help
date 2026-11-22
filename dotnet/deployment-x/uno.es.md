---
title: Despliegue multiplataforma del SDK .NET en Uno Platform
description: "Despliegue sus aplicaciones con el SDK .NET de VisioForge en Uno Platform: VideoView y soporte para Windows, Android, iOS, macOS y Linux."
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

# Guía de implementación y despliegue de Uno Platform

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a los SDKs de VisioForge para Uno Platform

Uno Platform es un potente framework de UI multiplataforma que permite a los desarrolladores crear aplicaciones nativas para Windows, Android, iOS, macOS y Linux desde una única base de código. VisioForge proporciona soporte completo para aplicaciones Uno Platform a través del paquete `VisioForge.DotNet.Core.UI.Uno`, que contiene controles de UI especializados diseñados específicamente para Uno Platform.

El proceso de despliegue de Uno Platform requiere consideración especial para cada plataforma objetivo. Este documento proporciona instrucciones detalladas para asegurar que su aplicación funcione correctamente en todas las plataformas soportadas.

## Plataformas soportadas

Los SDKs de VisioForge soportan los siguientes objetivos de Uno Platform:

| Plataforma | Framework objetivo | Estado |
|------------|-------------------|--------|
| Windows Desktop | net10.0-windows10.0.19041.0 | &#x2714; Soporte completo |
| Android | net10.0-android | &#x2714; Soporte completo |
| iOS | net10.0-ios | &#x2714; Soporte completo |
| macOS Desktop (Skia) | net10.0-desktop | &#x2714; Soporte completo |
| Linux Desktop (Skia) | net10.0-desktop | &#x2714; Soporte completo |

Estos destinos son los de Uno Platform 7, que ya no tiene destino Mac Catalyst: en macOS la aplicación se ejecuta en el cabezal `net10.0-desktop`. Uno Platform 7 requiere un paquete `VisioForge.DotNet.Core.UI.Uno` compilado para él; las versiones hasta 2026.11.16 están compiladas para Uno Platform 6.5.

## Requisitos del sistema

Antes de comenzar su implementación de Uno Platform, asegúrese de que su entorno de desarrollo cumple con los siguientes requisitos:

### Requisitos del entorno de desarrollo

- Computadora con Windows, Linux o macOS
- Visual Studio 2022 con extensión Uno Platform, JetBrains Rider o Visual Studio Code
- .NET 10.0 SDK o posterior (última versión estable recomendada)
- Plantillas de Uno Platform instaladas

### Requisitos específicos por plataforma

#### Windows
- Windows 10 versión 17763 o posterior
- Windows App SDK 2.5 o posterior (lo aporta Uno Platform 7)

#### Android
- Android SDK con niveles de API apropiados
- Dispositivo Android 7.0 (API 24) o posterior
- Java Development Kit (JDK) 11 o posterior

#### iOS/macOS
- Computadora Mac con Xcode 26 o posterior para compilaciones iOS/macOS (Xcode 27.2 o posterior para .NET 11)
- Cuenta de desarrollador de Apple (para despliegue en dispositivo)
- iOS 18.0 o posterior / macOS 12 o posterior

#### Linux
- Runtime de GStreamer instalado
- Servidor de visualización X11 o Wayland
- El paquete NuGet `VisioForge.CrossPlatform.Core.Linux.x64` (o `.arm64`)

## Proceso de instalación y configuración

Siga estos pasos para configurar y desplegar correctamente su aplicación Uno Platform con VisioForge:

### 1. Instalar plantillas de Uno Platform

```bash
dotnet new install Uno.Templates
```

### 2. Instalar workloads requeridos

```bash
# Para Android
dotnet workload install android

# Para iOS
dotnet workload install ios
```

### 3. Crear un nuevo proyecto Uno Platform

```bash
dotnet new unoapp -o MyMediaApp
```

### 4. Agregar paquetes NuGet de VisioForge

Agregue los siguientes paquetes a su proyecto:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="[2026.11.21]"/>
  <PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.21]"/>
</ItemGroup>
```

### Redistributables específicos por plataforma

Agregue paquetes redistributables específicos de plataforma a su proyecto:

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

Adicionalmente, necesitará agregar la biblioteca de bindings de Java. Clónela desde nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency) y agregue una referencia:

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

El paquete se aplica al cabezal desktop cuando el proyecto se compila en un Mac. Sus targets de compilación copian las bibliotecas nativas junto a la aplicación.

#### Linux Desktop

Para Linux, necesita instalar el runtime de GStreamer en su sistema:

```bash
# Ubuntu/Debian
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav

# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free
```

Después, añada el paquete core de la arquitectura de destino. Contiene las bibliotecas nativas propias del SDK (la conversión de color que usan las capturas de imagen, la conversión de fotogramas a mapas de bits y las superposiciones, el efecto 3D LUT, Data Matrix) y nada de GStreamer. Sin él, la aplicación se inicia, pero esas funciones fallan en tiempo de ejecución con `VisioForge_VAP is not available`:

```xml
<ItemGroup Condition="$([MSBuild]::IsOSPlatform('linux')) or $(RuntimeIdentifier.StartsWith('linux-'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.arm64" Version="[2026.10.26]" Condition="$(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64')))"/>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]" Condition="!($(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64'))))"/>
</ItemGroup>
```

El bloque se aplica cuando el proyecto se compila en Linux o se publica para un identificador de runtime `linux-*`; los frameworks de destino anteriores siguen eligiéndose por la máquina de compilación, así que una compilación Uno para Linux desde Windows necesita además definir `net10.0-desktop` explícitamente. Las condiciones eligen el paquete según el identificador de runtime: `-r linux-arm64` o un perfil de publicación para ARM64 selecciona el paquete ARM64, y sin identificador de runtime decide la arquitectura de la máquina de compilación.

### Archivo de proyecto de ejemplo completo

Aquí hay un ejemplo completo de archivo `.csproj` para una aplicación Uno Platform con VisioForge SDK:

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <!-- Target frameworks basados en el SO de compilación -->
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-desktop;net10.0-ios;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('linux'))">net10.0-desktop</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <UnoSingleProject>true</UnoSingleProject>
    <UseCurrentXcodeSDKVersion>true</UseCurrentXcodeSDKVersion>
    
    <!-- Configuración de la aplicación -->
    <ApplicationTitle>MyMediaApp</ApplicationTitle>
    <ApplicationId>com.yourcompany.mymediaapp</ApplicationId>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationPublisher>Your Company</ApplicationPublisher>
    <Description>Aplicación multimedia potenciada por Uno Platform y VisioForge.</Description>
    
    <UnoFeatures></UnoFeatures>
  </PropertyGroup>
  
  <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
    <CodesignKey>Apple Development</CodesignKey>
  </PropertyGroup>
  
  <!-- Referencias Core de VisioForge -->
  <ItemGroup>
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="[2026.11.21]"/>
    <PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.21]"/>
  </ItemGroup>
  
  <!-- Plataforma Windows -->
  <ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.16]"/>
    <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.16]"/>
  </ItemGroup>
  
  <!-- Plataforma Android -->
  <ItemGroup Condition="$(TargetFramework.Contains('-android'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="[2026.11.5]"/>
    <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
  </ItemGroup>
  
  <!-- Plataforma iOS -->
  <ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="[2026.11.5]"/>
  </ItemGroup>
  
  <!-- Plataforma macOS Desktop -->
  <ItemGroup Condition="$(TargetFramework.Contains('-desktop')) and $([MSBuild]::IsOSPlatform('osx'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="[2026.11.13]"/>
  </ItemGroup>
  
  <!-- Plataforma Linux Desktop -->
  <ItemGroup Condition="$([MSBuild]::IsOSPlatform('linux')) or $(RuntimeIdentifier.StartsWith('linux-'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.arm64" Version="[2026.10.26]" Condition="$(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64')))"/>
    <PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]" Condition="!($(RuntimeIdentifier.EndsWith('-arm64')) or ('$(RuntimeIdentifier)' == '' and $(NETCoreSdkRuntimeIdentifier.EndsWith('-arm64'))))"/>
  </ItemGroup>
</Project>
```

## Configuración específica por plataforma

### Configuración de Windows

Las aplicaciones Windows usan renderizado nativo WinUI 3 y soportan aceleración por hardware vía DirectX.

#### Capacidades requeridas

Agregue las capacidades requeridas a su `Package.appxmanifest`:

```xml
<Capabilities>
    <Capability Name="internetClient" />
    <uap:Capability Name="videosLibrary" />
    <uap:Capability Name="musicLibrary" />
    <DeviceCapability Name="microphone" />
    <DeviceCapability Name="webcam" />
</Capabilities>
```

### Configuración de Android

#### Permisos

Agregue los permisos necesarios a su `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```

#### Solicitudes de permisos en tiempo de ejecución

Solicite permisos en tiempo de ejecución en su código:

```csharp
private async Task RequestPermissionsAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        // Manejar denegación de permiso
    }
    
    status = await Permissions.RequestAsync<Permissions.Microphone>();
    if (status != PermissionStatus.Granted)
    {
        // Manejar denegación de permiso
    }
}
```

### Configuración de iOS

#### Configuraciones de Info.plist

Agregue las descripciones de uso requeridas a su `Info.plist`:

```xml
<key>NSCameraUsageDescription</key>
<string>Esta app requiere acceso a la cámara para captura de video</string>
<key>NSMicrophoneUsageDescription</key>
<string>Esta app requiere acceso al micrófono para grabación de audio</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>Esta app requiere acceso a la biblioteca de fotos para guardar medios</string>
```

#### App Transport Security

Para fuentes de streaming HTTP, configure App Transport Security:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

### Configuración de macOS Desktop

En macOS la aplicación se ejecuta en el cabezal `net10.0-desktop`, el mismo destino que en Linux. `VisioForge.DotNet.Core` elige para él su compilación de macOS al compilar y al ejecutar, así que la reproducción no necesita nada más que el paquete de macOS anterior.

La captura de cámara y micrófono requiere empaquetar la aplicación como bundle `.app` cuyo `Info.plist` contenga `NSCameraUsageDescription` y `NSMicrophoneUsageDescription`: sin ellas, macOS cierra la aplicación la primera vez que abre el dispositivo. El bundle se firma con el runtime reforzado (hardened runtime), así que también necesita los entitlements de cámara y micrófono. Añada los dos archivos al proyecto e indíquelos a la compilación; solo se aplican al bundle, no a `dotnet run`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<!-- Platforms/Desktop/Info.plist: Uno completa las claves del bundle listadas vacías -->
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
<!-- Platforms/Desktop/Entitlements.plist: el bundle se firma con el runtime reforzado -->
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
# osx-x64 para Mac Intel. Opcional: -p:CodesignKey="Developer ID Application: <your team>" firma el bundle
# con una identidad estable, y macOS conserva el permiso de cámara entre compilaciones.
dotnet publish -f net10.0-desktop -r osx-arm64 -p:PackageFormat=app
```

### Configuración de Linux Desktop

Para aplicaciones de escritorio Linux usando Skia:

1. Asegúrese de que GStreamer esté instalado en el sistema objetivo
2. Referencie el paquete core de Linux de su arquitectura de destino, como en el archivo de proyecto anterior
3. Establezca las variables de entorno apropiadas si es necesario:

```bash
export GST_PLUGIN_PATH=/usr/lib/x86_64-linux-gnu/gstreamer-1.0
```

## Compilación para diferentes plataformas

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

## Consideraciones de rendimiento

- **Aceleración por hardware**: Habilite el renderizado acelerado por hardware donde esté disponible (Windows DirectX, Apple VideoToolbox, Android MediaCodec)
- **Dispositivos físicos**: Siempre pruebe en dispositivos físicos, especialmente para plataformas móviles. Los simuladores pueden no representar con precisión el rendimiento real
- **Gestión de memoria**: Monitoree el uso de memoria, particularmente en dispositivos móviles al procesar archivos de medios grandes
- **Streaming de red**: Use tamaños de buffer apropiados para streaming de red para equilibrar latencia y fluidez

## Solución de problemas comunes

### El video no se muestra

1. Verifique que el VideoView esté correctamente inicializado y agregado al árbol visual
2. Compruebe que los redistributables específicos de plataforma estén correctamente instalados
3. Asegúrese de que los permisos estén otorgados en plataformas móviles

### Problemas de rendimiento

1. Compruebe que la aceleración por hardware esté habilitada
2. Reduzca la resolución de video para dispositivos de menor potencia
3. Monitoree el uso de memoria y optimice los tamaños de buffer

### Errores de compilación

1. Verifique que todos los workloads requeridos estén instalados
2. Compruebe la compatibilidad de versiones de paquetes NuGet
3. Asegúrese de que las versiones de target framework coincidan en todas las referencias del proyecto
