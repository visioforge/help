---
title: Video Capture SDK .NET para MAUI - Guía de Instalación
description: Implementa capacidades de video y medios en aplicaciones multiplataforma .NET MAUI para Windows, Android, iOS y macOS con SDKs de VisioForge.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - C++
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Playback
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - MediaPlayerCoreX

---

# Integrando SDKs de VisioForge con Aplicaciones .NET MAUI

## Descripción General

.NET Multi-platform App UI (MAUI) permite a los desarrolladores construir aplicaciones multiplataforma para móviles y escritorio desde una única base de código. VisioForge proporciona soporte completo para aplicaciones MAUI a través del paquete NuGet `VisioForge.DotNet.Core.UI.MAUI` (espacio de nombres: `VisioForge.Core.UI.MAUI`), que contiene controles de UI especializados diseñados específicamente para la plataforma .NET MAUI.

Nuestros SDKs habilitan potentes capacidades multimedia en todas las plataformas soportadas por MAUI:

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "¿Busca un tutorial funcional?"
    Una vez instalados los paquetes, siga la [Guía del Reproductor de Video .NET MAUI](../mediaplayer/guides/maui-player.md) — una implementación completa de `VideoView` + `MediaPlayerCoreX` con selector de archivos, búsqueda y volumen que funciona en iOS, Android, macOS y Windows desde una sola base de código.

## Comenzando

### Instalación

Para comenzar a usar VisioForge con tu proyecto MAUI, instala los paquetes NuGet requeridos:

1. El paquete SDK para el producto que estés usando - uno de:
   - `VisioForge.DotNet.VideoCapture` - captura de cámara, pantalla y cámara IP (`VideoCaptureCoreX`)
   - `VisioForge.DotNet.MediaPlayer` - reproducción de archivos y de red (`MediaPlayerCoreX`)
   - `VisioForge.DotNet.VideoEdit` - edición de línea de tiempo y renderizado (`VideoEditCoreX`)
   - `VisioForge.DotNet.MediaBlocks` - pipelines personalizados (`MediaBlocksPipeline`)
2. El paquete de UI principal: `VisioForge.DotNet.Core.UI.MAUI` - el control `VideoView`
3. Redistributable específico de plataforma (detallado en las secciones de plataforma a continuación)

```xml
<ItemGroup>
  <!-- Sustituye por el paquete SDK que use tu aplicación. -->
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.*" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
</ItemGroup>
```

`VisioForge.DotNet.Core.UI.MAUI` solo proporciona el control `VideoView`. Por sí solo no puede
compilar `VideoCaptureCoreX`, `MediaPlayerCoreX` ni `VideoEditCoreX` - el paquete SDK anterior es
lo que los incorpora.

### Inicialización del SDK

La inicialización adecuada es esencial para que los SDKs de VisioForge funcionen correctamente dentro de tu aplicación MAUI. Este proceso debe completarse en tu archivo `MauiProgram.cs`.

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
          // Inicializa el paquete SkiaSharp agregando la línea de código a continuación
          .UseSkiaSharp()
          // Inicializa el paquete MAUI de VisioForge agregando la línea de código a continuación
          .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
          // Después de inicializar el paquete MAUI de VisioForge, opcionalmente agrega fuentes adicionales
          .ConfigureFonts(fonts =>
          {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });

        // Continúa inicializando tu App .NET MAUI aquí
        return builder.Build();
    }
}
```

`AddVisioForgeHandlers` registra el handler de `VideoView`. No carga el stack nativo - eso es una
llamada aparte, y debe ejecutarse antes de construir el primer `VideoCaptureCoreX`,
`MediaPlayerCoreX`, `VideoEditCoreX` o `MediaBlocksPipeline`. Sin ella, el constructor lanza
`DllNotFoundException` en una máquina limpia:

```csharp
using VisioForge.Core;

// Inicialización asíncrona (recomendada - la primera llamada construye el registro de
// GStreamer y puede tardar cientos de milisegundos, lo que bloquearía el hilo de la UI).
await VisioForgeX.InitSDKAsync();

// O inicialización síncrona.
VisioForgeX.InitSDK();
```

Libera el SDK cuando la página o la aplicación se cierren:

```csharp
VisioForgeX.DestroySDK();
```

## Usando Controles de VisioForge en XAML

El control `VideoView` es la interfaz principal para mostrar contenido de video en tu aplicación MAUI. Para usar controles de VisioForge en tus archivos XAML:

1. Agrega el espacio de nombres de VisioForge a tu archivo XAML:

```xaml
xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
```

2. Agrega el control VideoView a tu diseño:

```xaml
<vf:VideoView Grid.Row="0"               
                HorizontalOptions="FillAndExpand"
                VerticalOptions="FillAndExpand"
                x:Name="videoView"
                Background="Black"/>
```

El control VideoView se adapta a las capacidades de renderizado nativas de cada plataforma mientras proporciona una API consistente para el código de tu aplicación.

## Configuración Específica de Plataforma

### Implementación en Android

Android requiere pasos de configuración adicionales para asegurar el funcionamiento adecuado:

#### 1. Agregar Biblioteca de Bindings Java

El SDK de VisioForge depende de funcionalidad nativa de Android que requiere una biblioteca de bindings Java personalizada:

1. Clona la biblioteca de binding de nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency)
2. Elige el proyecto de binding que coincida con la versión de .NET con la que compilas - la carpeta incluye un `VisioForge.Core.Android.X{N}.csproj` por cada versión de .NET soportada (por ejemplo, `VisioForge.Core.Android.X9.csproj` para .NET 9, `VisioForge.Core.Android.X10.csproj` para .NET 10). Si tu versión de .NET no aparece en la lista, elige la más cercana disponible.
3. Agrega la referencia a tu archivo de proyecto:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

#### 2. Agregar Paquete Redistributable de Android

Incluye el paquete redistributable específico de Android:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.*" />
</ItemGroup>
```

#### 3. Permisos de Android

Asegúrate de que tu AndroidManifest.xml incluya los permisos necesarios para acceso a cámara, micrófono y almacenamiento dependiendo de la funcionalidad de tu aplicación. Los permisos comúnmente requeridos incluyen:

- `android.permission.CAMERA`
- `android.permission.RECORD_AUDIO`
- `android.permission.READ_EXTERNAL_STORAGE`
- `android.permission.WRITE_EXTERNAL_STORAGE`

### Configuración de iOS

La integración con iOS requiere menos pasos pero tiene algunas consideraciones importantes:

#### 1. Agregar Redistributable de iOS

Agrega el paquete específico de iOS a tu proyecto:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <!-- La versión del redistribuable de iOS va por detrás de la versión del SDK a propósito - sigue
       el ciclo de reconstrucción de GStreamer-iOS, no el lanzamiento del wrapper. No la subas para
       igualar VisioForge.DotNet.*; no existe una versión 2026.x en nuget.org. -->
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2026.9.11" />
</ItemGroup>
```

#### 2. Notas Importantes para Desarrollo iOS

- **Usa dispositivos físicos**: El SDK requiere pruebas en dispositivos iOS físicos en lugar de simuladores para funcionalidad completa.
- **Descripciones de privacidad**: Agrega las cadenas de descripción de uso necesarias en tu archivo Info.plist para acceso a cámara y micrófono:
  - `NSCameraUsageDescription`
  - `NSMicrophoneUsageDescription`
  - `NSLocalNetworkUsageDescription` — necesario para alcanzar un servidor o dispositivo en la red local. Sin esta clave, iOS 14+ descarta el tráfico LAN en silencio (sin diálogo ni error).

### Configuración de macOS

Para aplicaciones macOS Catalyst:

#### 1. Acceso a la red local

Si tu aplicación se conecta a un servidor o dispositivo de la red local, agrega
`NSLocalNetworkUsageDescription` al `Info.plist` de Mac Catalyst. Sin esta clave, iOS y
Mac Catalyst pueden bloquear silenciosamente el tráfico de la red local sin mostrar un aviso de permiso.

#### 2. Configurar Identificadores de Runtime

Para asegurar que tu aplicación funcione correctamente tanto en Macs Intel como Apple Silicon, especifica los identificadores de runtime apropiados:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

#### 3. Habilitar Recorte

Para rendimiento óptimo en macOS, habilita la opción PublishTrimmed:

```xml
<PublishTrimmed Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">true</PublishTrimmed>
```

Para información más detallada sobre el despliegue en macOS, consulta nuestra página de documentación de [macOS](../deployment-x/macOS.md).

### Configuración de Windows

Para aplicaciones Windows, necesitas incluir varios paquetes redistributables:

#### 1. Agregar Redistributables Base de Windows

Incluye los paquetes principales de Windows:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.*" />
</ItemGroup>
```

#### 2. Agregar Soporte Extendido de Codecs (Opcional pero Recomendado)

Para soporte mejorado de formatos de medios, incluye el paquete libAV (FFMPEG):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.*" />
</ItemGroup>
```

### Optimización de Rendimiento

Para rendimiento óptimo en todas las plataformas:

1. Usa aceleración de hardware cuando esté disponible
2. Ajusta la resolución de video basándote en las capacidades del dispositivo objetivo
3. Considera las restricciones de memoria en dispositivos móviles al procesar archivos de medios grandes

## Solución de Problemas Comunes

- **Pantalla de video en blanco**: Asegúrate de que los permisos adecuados estén otorgados en plataformas móviles
- **Codecs faltantes**: Verifica que todos los paquetes redistributables específicos de plataforma estén correctamente instalados
- **Problemas de rendimiento**: Verifica que la aceleración de hardware esté habilitada cuando esté disponible
- **Errores de despliegue**: Confirma que los identificadores de runtime estén correctamente especificados para la plataforma objetivo

## Conclusión

El SDK de VisioForge proporciona una solución completa para agregar potentes capacidades multimedia a tus aplicaciones .NET MAUI. Siguiendo las instrucciones de configuración específicas de plataforma y las mejores prácticas descritas en esta guía, puedes crear aplicaciones multiplataforma ricas con características avanzadas de video y audio.

Para ejemplos adicionales y código de muestra, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

## Siguientes Pasos

- [Guía del Reproductor de Video .NET MAUI](../mediaplayer/guides/maui-player.md) — tutorial completo de `VideoView` con búsqueda, volumen y selector de archivos
- [Guía del Reproductor Avalonia](../mediaplayer/guides/avalonia-player.md) — alternativa multiplataforma orientada al escritorio (incluye Linux)
- [Guía del Reproductor Android](../mediaplayer/guides/android-player.md) — detalles de despliegue solo para Android
