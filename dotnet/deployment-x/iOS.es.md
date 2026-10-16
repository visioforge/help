---
title: Despliegue iOS con VisioForge SDK .NET - Guía Completa
description: Despliegue aplicaciones .NET en iOS con integración del SDK de VisioForge, permisos, soporte de arquitectura y mejores prácticas de despliegue multiplataforma.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCoreX
  - iOS
  - MAUI
  - Capture
  - Encoding
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - VideoView
  - IVideoView
  - VideoCaptureCoreX

---

# Guía de Despliegue para Apple iOS

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción General

Esta guía completa le lleva a través del proceso de despliegue de aplicaciones potenciadas por el SDK de VisioForge en dispositivos Apple iOS. El SDK de VisioForge proporciona un framework potente para construir aplicaciones ricas en medios en iOS, ofreciendo soporte robusto para capacidades de captura, edición, reproducción y procesamiento de video.

El proceso de despliegue en iOS involucra varias consideraciones clave, desde gestión de paquetes hasta manejo de permisos y optimización de rendimiento. Este documento le guiará a través de cada paso para asegurar una experiencia de despliegue fluida.

## Requisitos del Sistema

Antes de comenzar su proceso de despliegue en iOS, asegúrese de que su entorno de desarrollo cumpla los siguientes requisitos:

### Requisitos de Hardware

- Computadora Apple Mac para desarrollo (requerida para firma de apps iOS)
- Dispositivo iOS para pruebas (altamente recomendado sobre simuladores)
- Espacio de almacenamiento suficiente para herramientas de desarrollo y activos de aplicación

### Requisitos de Software

- Dispositivo Apple iOS con iOS 16.4 o posterior (el mínimo de las bibliotecas nativas `VisioForge.CrossPlatform.Core.iOS`)
- SDK de .NET 9 o .NET 10 con la carga de trabajo de iOS (`dotnet workload install ios`); el SDK se dirige a `net9.0-ios` y `net10.0-ios`
- La versión de Xcode que exige su carga de trabajo de iOS (Xcode 26 o posterior para .NET 10)
- Cuenta de desarrollador Apple (requerida para firma y distribución de apps)
- JetBrains Rider o Visual Studio Code (Visual Studio para Mac se retiró en 2024)

## Soporte de Arquitectura

El SDK de VisioForge para iOS proporciona soporte nativo para las principales arquitecturas de dispositivos iOS:

### Soporte ARM64

- Compatible con todos los dispositivos iOS modernos (iPhone X y posteriores)
- Bibliotecas nativas optimizadas para máximo rendimiento
- Procesamiento de video acelerado por hardware donde sea soportado por el dispositivo

## Proceso de Instalación

Siga estos pasos para configurar e implementar correctamente su aplicación iOS con VisioForge:

1. Instale el SDK de .NET y la carga de trabajo de iOS
2. Cree un nuevo proyecto iOS en su IDE preferido (JetBrains Rider o Visual Studio Code)
3. Agregue los paquetes NuGet requeridos a su proyecto (detallado en la siguiente sección)
4. Configure los permisos y entitlements necesarios en el archivo Info.plist de su app
5. Implemente la lógica de su aplicación usando los componentes del SDK de VisioForge
6. Compile, firme y despliegue su aplicación en dispositivos de prueba

## Paquetes NuGet

El SDK de VisioForge para iOS se distribuye a través de paquetes NuGet:

### Paquetes Principales

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) - Paquete principal que contiene clases principales y controles de UI, incluyendo componentes de reproducción y visualización de video. Este es independiente de plataforma y puede usarse en cualquier proyecto .Net.

### Paquetes de UI

Cada paquete de UI tiene los mismos controles VideoView pero diferentes implementaciones para la plataforma objetivo:

#### Plataforma objetivo .Net iOS

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) - Contiene controles de UI y todas las clases principales para la plataforma iOS.

#### Plataforma objetivo .Net MAUI

- [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI) - Contiene controles de UI para la plataforma MAUI.

### Paquetes Redistributables

- [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS) - Contiene los componentes principales de redistribución requeridos para cualquier aplicación iOS usando tecnologías VisioForge.

Puede agregar estos paquetes usando el NuGet Package Manager en su IDE o agregando lo siguiente a su archivo de proyecto (use las últimas versiones):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2026.9.11" />
</ItemGroup>
```

Nota: Reemplace los números de versión con las últimas versiones disponibles.

## Permisos y Entitlements Requeridos

Las aplicaciones iOS requieren permisos explícitos para acceder a características del dispositivo como cámaras, micrófonos y la biblioteca de fotos. Configure estos permisos en el archivo Info.plist de su app:

### Acceso a Cámara

Requerido para funcionalidad de captura de video:

```xml
<key>NSCameraUsageDescription</key>
<string>Esta app requiere acceso a la cámara para grabación de video</string>
```

### Acceso a Micrófono

Requerido para grabación de audio:

```xml
<key>NSMicrophoneUsageDescription</key>
<string>Esta app requiere acceso al micrófono para grabación de audio</string>
```

### Acceso a Biblioteca de Fotos

Requerido para guardar videos en la biblioteca de fotos del dispositivo:

```xml
<key>NSPhotoLibraryUsageDescription</key>
<string>Esta app requiere acceso a la biblioteca de fotos para guardar videos</string>
```

### Ejemplo de Configuración Info.plist

Aquí hay un ejemplo completo de un archivo Info.plist con todos los permisos necesarios:

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
    <key>UIApplicationSceneManifest</key>
    <dict>
        <key>UIApplicationSupportsMultipleScenes</key>
        <false/>
        <key>UISceneConfigurations</key>
        <dict>
            <key>UIWindowSceneSessionRoleApplication</key>
            <array>
                <dict>
                    <key>UISceneConfigurationName</key>
                    <string>Default Configuration</string>
                    <key>UISceneDelegateClassName</key>
                    <string>SceneDelegate</string>
                </dict>
            </array>
        </dict>
    </dict>
    <key>NSCameraUsageDescription</key>
    <string>Se requiere acceso a la cámara para grabación de video</string>
    <key>NSMicrophoneUsageDescription</key>
    <string>Se requiere acceso al micrófono para grabación de audio</string>
    <key>NSPhotoLibraryUsageDescription</key>
    <string>Se requiere acceso a la biblioteca de fotos para guardar videos</string>
</dict>
</plist>
```

## Manejo de Permisos en Tiempo de Ejecución

Además de declarar permisos en su archivo Info.plist, también debe solicitar permisos en tiempo de ejecución. Aquí hay un ejemplo de cómo solicitar permisos de cámara y micrófono:

```csharp
using System.Diagnostics;
using Photos;

// Solicitar permiso de cámara
private async Task RequestCameraPermissionAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Camera>();
    if (status != PermissionStatus.Granted)
    {
        // Manejar denegación de permiso
        Debug.WriteLine("Permiso de cámara denegado");
    }
}

// Solicitar permiso de micrófono
private async Task RequestMicrophonePermissionAsync()
{
    var status = await Permissions.RequestAsync<Permissions.Microphone>();
    if (status != PermissionStatus.Granted)
    {
        // Manejar denegación de permiso
        Debug.WriteLine("Permiso de micrófono denegado");
    }
}

// Solicitar permiso de biblioteca de fotos (específico de iOS)
private void RequestPhotoLibraryPermission()
{
    PHPhotoLibrary.RequestAuthorization(status =>
    {
        if (status == PHAuthorizationStatus.Authorized)
        {
            Debug.WriteLine("Acceso a biblioteca de fotos concedido");
        }
        else
        {
            Debug.WriteLine("Acceso a biblioteca de fotos denegado");
        }
    });
}
```

## Inicialización del SDK

Inicialice correctamente el SDK de VisioForge en el ciclo de vida de su aplicación:

```csharp
// En su AppDelegate o código de inicio de aplicación
// Aquí se guarda la ventana que crea SceneDelegate
public override UIWindow? Window { get; set; }

public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    // Inicializar el SDK de VisioForge
    VisioForge.Core.VisioForgeX.InitSDK();
    
    // Su otro código de inicialización
    
    return true;
}

// Limpiar al terminar la aplicación
public override void WillTerminate(UIApplication application)
{
    // Limpiar recursos del SDK de VisioForge
    VisioForge.Core.VisioForgeX.DestroySDK();
    
    // Su otro código de limpieza
}
```

!!! warning "iOS 27: el ciclo de vida de escenas es obligatorio"
    Las aplicaciones compiladas con el SDK de iOS 27 deben adoptar el ciclo de vida de escenas de UIKit. Sin `UIApplicationSceneManifest` en `Info.plist`, iOS cierra la aplicación al iniciarse, antes de que aparezca cualquier interfaz. `InitSDK()` puede permanecer en `FinishedLaunching`, pero la ventana debe crearse en un `SceneDelegate` a partir de la `UIWindowScene` que se conecta, no en `FinishedLaunching`.

Agregue el manifiesto de escenas a `Info.plist` (el ejemplo completo anterior ya lo incluye):

```xml
<key>UIApplicationSceneManifest</key>
<dict>
    <key>UIApplicationSupportsMultipleScenes</key>
    <false/>
    <key>UISceneConfigurations</key>
    <dict>
        <key>UIWindowSceneSessionRoleApplication</key>
        <array>
            <dict>
                <key>UISceneConfigurationName</key>
                <string>Default Configuration</string>
                <key>UISceneDelegateClassName</key>
                <string>SceneDelegate</string>
            </dict>
        </array>
    </dict>
</dict>
```

Cree la ventana en `SceneDelegate.cs`:

```csharp
[Register("SceneDelegate")]
public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    [Export("window")] public UIWindow? Window { get; set; }

    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
        {
            return;
        }

        // Guardar la ventana en AppDelegate: una escena que se reconecta recupera la misma ventana.
        // MainViewController representa su propio controlador de vista raíz.
        var app = (AppDelegate)UIApplication.SharedApplication.Delegate;
        app.Window ??= new UIWindow(windowScene) { RootViewController = new MainViewController() };
        Window = app.Window;
        Window.WindowScene = windowScene;
        Window.MakeKeyAndVisible();
    }
}
```

### Aplicaciones .NET MAUI

En una aplicación MAUI, MAUI crea la ventana por sí mismo, por lo que el `SceneDelegate` nativo anterior no es necesario. Agregue el mismo `UIApplicationSceneManifest`, con `__MAUI_DEFAULT_SCENE_CONFIGURATION__` como `UISceneConfigurationName` (MAUI busca su configuración con ese nombre), a `Platforms/iOS/Info.plist` (y a `Platforms/MacCatalyst/Info.plist` si se dirige a Mac Catalyst), conserve el `AppDelegate : MauiUIApplicationDelegate` de la plantilla y agregue junto a él un `SceneDelegate` vacío:

```csharp
// Platforms/iOS/SceneDelegate.cs, y el mismo archivo en Platforms/MacCatalyst/
using Foundation;

[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
}
```

Las [habilidades para agentes](../agent-skills/index.md) de iOS (`media-player-sdk-x-ios`, `video-capture-sdk-x-ios`, `media-blocks-sdk-net-ios`) incluyen un `SceneDelegate.cs` y un `Info.plist` completos.

## Mejores Prácticas de Implementación

### Usando Controles VideoView

El SDK de VisioForge proporciona un control `VideoView` para mostrar contenido de video. En iOS `VideoView` es una subclase de `MTKView` acelerada por Metal que implementa directamente `IVideoView` (use `VideoViewGL` si necesita la variante legacy con OpenGL basada en `UIView`):

```csharp
// Crear una instancia de VideoView — VideoView implementa IVideoView, no hace falta casting ni helper
var videoView = new VisioForge.Core.UI.Apple.VideoView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));
View.AddSubview(videoView);

// Pase el propio videoView donde se espera un IVideoView
var captureCore = new VideoCaptureCoreX(videoView);
```

Puede agregar el VideoView usando un storyboard o código.

### Gestión de Recursos

Los dispositivos iOS tienen recursos limitados comparados con computadoras de escritorio. Siga estas mejores prácticas:

1. Libere recursos cuando no estén en uso
2. Use configuraciones de menor resolución para procesamiento en tiempo real
3. Implemente gestión adecuada del ciclo de vida en sus ViewControllers
4. Pruebe en dispositivos reales, no solo simuladores

## Pruebas y Depuración

### Pruebas en Dispositivo Físico

Aunque el simulador iOS puede ser útil para pruebas básicas de interfaz, tiene limitaciones significativas para aplicaciones de medios:

- El simulador puede tener problemas de rendimiento durante codificación de video a altas resoluciones
- Cámara y micrófono no están disponibles en el simulador
- Las características de aceleración de hardware pueden no estar disponibles o comportarse diferente

**Pruebe siempre su aplicación de medios en dispositivos iOS físicos antes del lanzamiento.**

### Consideraciones Comunes de Rendimiento

Al desplegar aplicaciones de medios en iOS, considere estos factores de rendimiento:

1. **Resolución y velocidad de cuadros:** Reduzca estos ajustes para mejor rendimiento en dispositivos más antiguos
2. **Selección de codificador:** Use codificadores acelerados por hardware cuando estén disponibles
3. **Gestión de memoria:** Implemente eliminación adecuada de objetos grandes y monitoree el uso de memoria
4. **Impacto en batería:** El procesamiento de medios es intensivo en energía; implemente medidas de ahorro de energía

## Solución de Problemas Comunes

### Denegaciones de Permisos

Si su app no puede acceder a la cámara o micrófono:

1. Verifique que todos los permisos requeridos estén en su Info.plist
2. Verifique que esté solicitando permisos en tiempo de ejecución antes de intentar usar el hardware
3. Compruebe si el usuario ha denegado manualmente los permisos en Ajustes de iOS

### Errores de Carga de Bibliotecas

Si encuentra errores al cargar bibliotecas nativas:

1. Verifique que todos los paquetes NuGet requeridos estén correctamente instalados
2. Busque versiones de paquetes en conflicto
3. Asegúrese de que esté apuntando a la arquitectura iOS correcta (ARM64)

## Recursos Adicionales

- Visite el [repositorio GitHub de VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código y proyectos de ejemplo
- Explore la [documentación de API de VisioForge](https://api.visioforge.org/dotnet/api/index.html) para referencia completa del SDK

---
Siguiendo esta guía de despliegue, debería poder crear, configurar y desplegar exitosamente aplicaciones potenciadas por VisioForge en dispositivos iOS. Para preguntas específicas o necesidades de configuración avanzada, por favor contacte al soporte técnico de VisioForge.