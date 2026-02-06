---
title: Integra SDKs de Medios con Aplicaciones .NET MAUI
description: Implementa capacidades de video y medios en aplicaciones multiplataforma .NET MAUI para Windows, Android, iOS y macOS con SDKs de VisioForge.
---

# Integrando SDKs de VisioForge con Aplicaciones .NET MAUI

## Descripción General

.NET Multi-platform App UI (MAUI) permite a los desarrolladores construir aplicaciones multiplataforma para móviles y escritorio desde una única base de código. VisioForge proporciona soporte completo para aplicaciones MAUI a través del paquete `VisioForge.Core.UI.MAUI`, que contiene controles de UI especializados diseñados específicamente para la plataforma .NET MAUI.

Nuestros SDKs habilitan potentes capacidades multimedia en todas las plataformas soportadas por MAUI:

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Comenzando

### Instalación

Para comenzar a usar VisioForge con tu proyecto MAUI, instala los paquetes NuGet requeridos:

1. El paquete de UI principal: `VisioForge.Core.UI.MAUI`
2. Redistributable específico de plataforma (detallado en las secciones de plataforma a continuación)

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
2. Agrega el proyecto apropiado a tu solución:
   - Usa `VisioForge.Core.Android.X8.csproj` para .NET 8
   - Usa `VisioForge.Core.Android.X9.csproj` para .NET 9
3. Agrega la referencia a tu archivo de proyecto:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

#### 2. Agregar Paquete Redistributable de Android

Incluye el paquete redistributable específico de Android:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="1.22.5.10" />
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
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="1.23.0" />
</ItemGroup>
```

#### 2. Notas Importantes para Desarrollo iOS

- **Usa dispositivos físicos**: El SDK requiere pruebas en dispositivos iOS físicos en lugar de simuladores para funcionalidad completa.
- **Descripciones de privacidad**: Agrega las cadenas de descripción de uso necesarias en tu archivo Info.plist para acceso a cámara y micrófono:
  - `NSCameraUsageDescription`
  - `NSMicrophoneUsageDescription`

### Configuración de macOS

Para aplicaciones macOS Catalyst:

#### 1. Configurar Identificadores de Runtime

Para asegurar que tu aplicación funcione correctamente tanto en Macs Intel como Apple Silicon, especifica los identificadores de runtime apropiados:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

#### 2. Habilitar Recorte

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
  <PackageReference Include="VisioForge.CrossPlatform.Codecs.Windows.x64" Version="15.7.0" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="15.7.0" />
</ItemGroup>
```

#### 2. Agregar Soporte Extendido de Codecs (Opcional pero Recomendado)

Para soporte mejorado de formatos de medios, incluye el paquete libAV (FFMPEG):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="15.7.0" />
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
