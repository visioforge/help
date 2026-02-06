---
title: Despliegue Multiplataforma .NET para Uno Platform
description: Despliegue de VisioForge .NET SDK en Uno Platform con integración de VideoView, soporte multiplataforma para Windows, Android, iOS, macOS y Linux.
---

# Guía de Implementación y Despliegue de Uno Platform

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a los SDKs de VisioForge para Uno Platform

Uno Platform es un potente framework de UI multiplataforma que permite a los desarrolladores crear aplicaciones nativas para Windows, Android, iOS, macOS y Linux desde una única base de código. VisioForge proporciona soporte completo para aplicaciones Uno Platform a través del paquete `VisioForge.DotNet.Core.UI.Uno`, que contiene controles de UI especializados diseñados específicamente para Uno Platform.

El proceso de despliegue de Uno Platform requiere consideración especial para cada plataforma objetivo. Este documento proporciona instrucciones detalladas para asegurar que su aplicación funcione correctamente en todas las plataformas soportadas.

## Plataformas Soportadas

Los SDKs de VisioForge soportan los siguientes objetivos de Uno Platform:

| Plataforma | Framework Objetivo | Estado |
|------------|-------------------|--------|
| Windows Desktop | net10.0-windows10.0.19041.0 | &#x2714; Soporte Completo |
| Android | net10.0-android | &#x2714; Soporte Completo |
| iOS | net10.0-ios | &#x2714; Soporte Completo |
| macOS (Catalyst) | net10.0-maccatalyst | &#x2714; Soporte Completo |
| Linux Desktop (Skia) | net10.0-desktop | &#x2714; Soporte Completo |

## Requisitos del Sistema

Antes de comenzar su implementación de Uno Platform, asegúrese de que su entorno de desarrollo cumple con los siguientes requisitos:

### Requisitos del Entorno de Desarrollo

- Computadora con Windows, Linux o macOS
- Visual Studio 2022 con extensión Uno Platform, JetBrains Rider o Visual Studio Code
- .NET 10.0 SDK o posterior (última versión estable recomendada)
- Plantillas de Uno Platform instaladas

### Requisitos Específicos por Plataforma

#### Windows
- Windows 10 versión 17763 o posterior
- Windows App SDK 1.4+

#### Android
- Android SDK con niveles de API apropiados
- Dispositivo Android 5.0 (API 21) o posterior
- Java Development Kit (JDK) 11 o posterior

#### iOS/macOS
- Computadora Mac con Xcode 15+ instalado (para compilaciones iOS/macOS)
- Cuenta de desarrollador de Apple (para despliegue en dispositivo)
- iOS 15.0 o posterior / macOS 10.15 o posterior

#### Linux
- Runtime de GStreamer instalado
- Servidor de visualización X11 o Wayland

## Proceso de Instalación y Configuración

Siga estos pasos para configurar y desplegar correctamente su aplicación Uno Platform con VisioForge:

### 1. Instalar Plantillas de Uno Platform

```bash
dotnet new install Uno.Templates
```

### 2. Instalar Workloads Requeridos

```bash
# Para Android
dotnet workload install android

# Para iOS/macOS
dotnet workload install ios maccatalyst
```

### 3. Crear un Nuevo Proyecto Uno Platform

```bash
dotnet new unoapp -o MyMediaApp
```

### 4. Agregar Paquetes NuGet de VisioForge

Agregue los siguientes paquetes a su proyecto:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2025.12.9" />
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.4.10" />
</ItemGroup>
```

### Redistributables Específicos por Plataforma

Agregue paquetes redistributables específicos de plataforma a su proyecto:

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

Adicionalmente, necesitará agregar la Biblioteca de Bindings de Java. Clónela desde nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency) y agregue una referencia:

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

Para macOS Catalyst, también necesita agregar un target MSBuild personalizado para copiar bibliotecas nativas al bundle de la aplicación:

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

Para Linux, necesita instalar el runtime de GStreamer en su sistema:

```bash
# Ubuntu/Debian
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav

# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free
```

### Archivo de Proyecto de Ejemplo Completo

Aquí hay un ejemplo completo de archivo `.csproj` para una aplicación Uno Platform con VisioForge SDK:

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <!-- Target frameworks basados en el SO de compilación -->
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-maccatalyst;net10.0-ios;net10.0-android</TargetFrameworks>
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
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2025.12.9" />
    <PackageReference Include="VisioForge.DotNet.Core" Version="2025.4.10" />
  </ItemGroup>
  
  <!-- Plataforma Windows -->
  <ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
    <PackageReference Include="Microsoft.WindowsAppSDK" Version="1.8.251106002" />
    <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
    <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
  </ItemGroup>
  
  <!-- Plataforma Android -->
  <ItemGroup Condition="$(TargetFramework.Contains('-android'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
    <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
  </ItemGroup>
  
  <!-- Plataforma iOS -->
  <ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
  </ItemGroup>
  
  <!-- Plataforma macOS Catalyst -->
  <ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
  </ItemGroup>
  
  <!-- macOS: Copiar bibliotecas nativas al bundle de la app -->
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

## Configuración Específica por Plataforma

### Configuración de Windows

Las aplicaciones Windows usan renderizado nativo WinUI 3 y soportan aceleración por hardware vía DirectX.

#### Capacidades Requeridas

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

#### Solicitudes de Permisos en Tiempo de Ejecución

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

### Configuración de macOS (Catalyst)

Las aplicaciones macOS Catalyst comparten configuración con iOS. Adicionalmente, configure identificadores de runtime para Intel y Apple Silicon:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

### Configuración de Linux Desktop

Para aplicaciones de escritorio Linux usando Skia:

1. Asegúrese de que GStreamer esté instalado en el sistema objetivo
2. Establezca las variables de entorno apropiadas si es necesario:

```bash
export GST_PLUGIN_PATH=/usr/lib/x86_64-linux-gnu/gstreamer-1.0
```

## Compilación para Diferentes Plataformas

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

## Consideraciones de Rendimiento

- **Aceleración por Hardware**: Habilite el renderizado acelerado por hardware donde esté disponible (Windows DirectX, Apple VideoToolbox, Android MediaCodec)
- **Dispositivos Físicos**: Siempre pruebe en dispositivos físicos, especialmente para plataformas móviles. Los simuladores pueden no representar con precisión el rendimiento real
- **Gestión de Memoria**: Monitoree el uso de memoria, particularmente en dispositivos móviles al procesar archivos de medios grandes
- **Streaming de Red**: Use tamaños de buffer apropiados para streaming de red para equilibrar latencia y fluidez

## Solución de Problemas Comunes

### El Video No Se Muestra

1. Verifique que el VideoView esté correctamente inicializado y agregado al árbol visual
2. Compruebe que los redistributables específicos de plataforma estén correctamente instalados
3. Asegúrese de que los permisos estén otorgados en plataformas móviles

### Problemas de Rendimiento

1. Compruebe que la aceleración por hardware esté habilitada
2. Reduzca la resolución de video para dispositivos de menor potencia
3. Monitoree el uso de memoria y optimice los tamaños de buffer

### Errores de Compilación

1. Verifique que todos los workloads requeridos estén instalados
2. Compruebe la compatibilidad de versiones de paquetes NuGet
3. Asegúrese de que las versiones de target framework coincidan en todas las referencias del proyecto
