---
title: Manual de Despliegue Multiplataforma .Net para macOS
description: Despliegue del SDK .NET de VisioForge para macOS con integración NSView, permisos, publicación en app store y guía de configuración de bibliotecas nativas.
---

# Guía de Despliegue para Apple macOS

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Los potentes SDKs .NET de VisioForge proporcionan capacidades completas de procesamiento de medios para desarrolladores macOS. Ya sea que estés construyendo aplicaciones de captura de video, reproductores de medios, editores de video o pipelines complejos de procesamiento de medios, nuestros SDKs ofrecen las herramientas que necesitas para entregar soluciones de alta calidad en las plataformas de Apple.

El SDK de VisioForge proporciona soporte completo para desarrollo de aplicaciones macOS usando tecnologías .NET. Puedes aprovechar este SDK para construir aplicaciones robustas de procesamiento de medios que se ejecuten nativamente en macOS, incluyendo soporte para arquitecturas Intel (x64) y Apple Silicon (ARM64).

Esta guía cubre todo lo que necesitas saber para configurar, configurar y desplegar aplicaciones para entornos macOS y MacCatalyst usando el SDK de VisioForge. Ya sea que estés construyendo aplicaciones macOS tradicionales o soluciones multiplataforma usando frameworks como MAUI o Avalonia, este documento te ayudará a navegar el proceso de instalación y despliegue.

## Requisitos del Sistema

Antes de comenzar el proceso de instalación y despliegue, asegúrate de que tu entorno de desarrollo cumpla los siguientes requisitos:

### Requisitos de Hardware

- Computadora Mac con procesador Intel (x64) o Apple Silicon (ARM64)
- Mínimo 8GB RAM (16GB recomendado para procesamiento de video)
- Espacio de disco suficiente para herramientas de desarrollo y activos de aplicación

### Requisitos de Software

- macOS 10.15 (Catalina) o posterior (se recomienda la última versión)
  - macOS Monterey (12.x)
  - macOS Ventura (13.x)
  - macOS Sonoma (14.x)
  - Futuras versiones de macOS (con actualizaciones continuas)
- Xcode 12 o posterior con Command Line Tools instaladas
- .NET 6.0 SDK o posterior
- Visual Studio para Mac o JetBrains Rider (IDEs recomendados)

Para instalar XCode Command Line Tools, ejecuta lo siguiente en Terminal:

```bash
xcode-select --install
```

## Soporte de Arquitectura

El SDK de VisioForge para macOS soporta ambas arquitecturas principales de procesador:

### Soporte Intel (x64)

- Compatible con todas las computadoras Mac basadas en Intel
- Usa bibliotecas x64 nativas para rendimiento óptimo
- Soporte completo de características en todos los componentes del SDK

### Soporte Apple Silicon (ARM64)

- Soporte nativo para chips M1, M2 y Apple Silicon más nuevos
- Bibliotecas nativas ARM64 optimizadas para máximo rendimiento
- Aceleración de hardware aprovechando el Neural Engine de Apple donde sea aplicable

### Consideraciones de Universal Binary

Al apuntar a ambas arquitecturas, considera construir binarios universales que incluyan código tanto x64 como ARM64. Este enfoque asegura que tu aplicación se ejecute nativamente en cualquiera de las plataformas sin depender de la traducción Rosetta 2.

Para builds de binarios universales apuntando tanto a Intel como Apple Silicon:

```xml
<PropertyGroup>
  <RuntimeIdentifiers>osx-x64;osx-arm64</RuntimeIdentifiers>
  <UseHardenedRuntime>true</UseHardenedRuntime>
</PropertyGroup>
```

## Tecnologías Principales

Los SDKs .NET de VisioForge aprovechan varias tecnologías clave para entregar capacidades de medios de alto rendimiento en macOS:

### Integración GStreamer

Todos los SDKs de VisioForge utilizan GStreamer como el framework subyacente para reproducción y codificación de video/audio. GStreamer proporciona:

- Procesamiento de medios acelerado por hardware
- Amplia compatibilidad de formatos
- Pipeline de reproducción optimizado
- Capacidades eficientes de codificación

Los componentes de GStreamer se instalan automáticamente a través de nuestros paquetes redistributables, eliminando la necesidad de configuración manual.

## Instalación y Despliegue de Paquetes NuGet

El método principal para desplegar componentes del SDK de VisioForge en aplicaciones macOS es a través de paquetes NuGet. Estos paquetes incluyen todas las bibliotecas administradas y no administradas necesarias para tu aplicación.

### Paquetes NuGet Esenciales

Para aplicaciones nativas macOS, agrega estos paquetes principales:

1. **Paquete Principal del SDK** (según tus necesidades):
   - `VisioForge.DotNet.VideoCapture` para aplicaciones de captura de cámara
   - `VisioForge.DotNet.VideoEdit` para aplicaciones de edición de video
   - `VisioForge.DotNet.MediaPlayer` para aplicaciones de reproducción de medios
   - `VisioForge.DotNet.MediaBlocks` para pipelines avanzados de procesamiento de medios

2. **Paquete de UI**:
   - `VisioForge.DotNet.Core` incluye controles de UI específicos de Apple

3. **Redistributable de Plataforma**:
   - `VisioForge.CrossPlatform.Core.macOS` para bibliotecas nativas y dependencias

### Aplicaciones macOS

Para aplicaciones macOS estándar apuntando al framework `netX.0-macos` (donde X representa la versión de .NET), usa el siguiente paquete NuGet:

- [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS)

Este paquete contiene:

- Bibliotecas nativas para procesamiento de medios
- Componentes GStreamer para reproducción y codificación de medios
- Ensamblados de interfaz para integración .NET
- Binarios tanto x64 como ARM64

### Comenzando con Proyectos Nativos macOS

Para comenzar a desarrollar aplicaciones nativas macOS con SDKs de VisioForge:

1. **Crea un nuevo proyecto macOS** en tu IDE preferido (Visual Studio para Mac o JetBrains Rider)
2. **Agrega los paquetes NuGet requeridos** (como se detalla arriba)
3. **Configura los ajustes del proyecto** para tu arquitectura objetivo

## Aplicaciones MacCatalyst y MAUI

### Desarrollo Multiplataforma con .NET MAUI

.NET Multi-platform App UI (MAUI) permite desarrollar aplicaciones que se ejecutan sin problemas en macOS, iOS, Android y Windows desde una única base de código. VisioForge proporciona soporte completo para desarrollo MAUI a través de paquetes y controles especializados.

Para aplicaciones MacCatalyst (incluyendo proyectos MAUI) apuntando al framework `netX.0-maccatalyst`, usa:

- [VisioForge.CrossPlatform.Core.macCatalyst](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macCatalyst)

### Configuración de Paquetes MAUI

Para proyectos MAUI apuntando a macOS (a través de MacCatalyst), agrega estos paquetes:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="15.10.11" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="15.10.11" />
</ItemGroup>
```

### Configuración del Proyecto MAUI

1. **Inicializa el SDK en MauiProgram.cs**:

```csharp
builder
  .UseMauiApp<App>()
  .UseSkiaSharp()
  .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

2. **Agrega el Control VideoView en XAML**:

```xml
xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"

<vf:VideoView Grid.Row="0"
              HorizontalOptions="FillAndExpand"
              VerticalOptions="FillAndExpand"
              x:Name="videoView"
              Background="Black"/>
```

Las aplicaciones MacCatalyst requieren configuración adicional para asegurar que las bibliotecas nativas se incluyan correctamente en el bundle de la aplicación. Agrega el siguiente objetivo de build personalizado a tu archivo de proyecto:

```xml
<Target Name="CopyNativeLibrariesToMonoBundle" AfterTargets="Build" Condition="$(TargetFramework.Contains('-maccatalyst'))">
    <Message Text="Iniciando objetivo CopyNativeLibrariesToMonoBundle..." Importance="High"/>

    <PropertyGroup>
        <AppBundleDir>$(OutputPath)$(AssemblyName).app</AppBundleDir>
        <MonoBundleDir>$(AppBundleDir)/Contents/MonoBundle</MonoBundleDir>
    </PropertyGroup>

    <Message Text="AppBundleDir: $(AppBundleDir)" Importance="High"/>
    <Message Text="MonoBundleDir: $(MonoBundleDir)" Importance="High"/>

    <MakeDir Directories="$(MonoBundleDir)" Condition="!Exists('$(MonoBundleDir)')"/>

    <Copy SourceFiles="@(None-&gt;'%(FullPath)')" DestinationFolder="$(MonoBundleDir)" Condition="'%(Extension)' == '.dylib' Or '%(Extension)' == '.so'">
        <Output TaskParameter="CopiedFiles" ItemName="CopiedNativeFiles"/>
    </Copy>

    <Message Text="Archivos nativos copiados:" Importance="High" Condition="@(CopiedNativeFiles) != ''"/>
    <Message Text=" - %(CopiedNativeFiles.Identity)" Importance="High" Condition="@(CopiedNativeFiles) != ''"/>

    <Message Text="Objetivo CopyNativeLibrariesToMonoBundle finalizado." Importance="High"/>
</Target>
```

Para detalles completos de integración MAUI, consulta nuestra página de documentación dedicada de [MAUI](../install/maui.md).

## Opciones de Framework de UI

El SDK de VisioForge soporta múltiples frameworks de UI para desarrollo macOS:

### UI Nativo macOS

Para aplicaciones macOS tradicionales, el SDK proporciona controles `VideoViewGL` que se integran con el framework nativo AppKit. Estos controles proporcionan renderizado de video de alto rendimiento usando OpenGL.

### MAUI

Para aplicaciones MAUI multiplataforma, usa el paquete [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI), que proporciona vistas de video compatibles con MAUI.

### Avalonia

Para aplicaciones Avalonia UI, el paquete [VisioForge.DotNet.Core.UI.Avalonia](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.Avalonia) ofrece controles de video compatibles con Avalonia.

## Inicialización y Limpieza del SDK

Los motores X en el SDK de VisioForge requieren inicialización y limpieza explícitas para gestionar recursos correctamente:

```csharp
// Inicializar SDK al inicio de la aplicación
VisioForge.Core.VisioForgeX.InitSDK();

// Usar componentes del SDK...

// Limpiar recursos antes de salir de la aplicación
VisioForge.Core.VisioForgeX.DestroySDK();
```

Para inicialización y limpieza asíncronas, usa las variantes async:

```csharp
// Inicialización async
await VisioForge.Core.VisioForgeX.InitSDKAsync();

// Limpieza async
await VisioForge.Core.VisioForgeX.DestroySDKAsync();
```

## Solución de Problemas Comunes

### Fallos de Carga de Bibliotecas Nativas

Si tu aplicación falla al cargar bibliotecas nativas:

1. Verifica que todos los paquetes NuGet requeridos estén correctamente instalados
2. Revisa la estructura del bundle de la aplicación para asegurar que las bibliotecas estén en la ubicación correcta
3. Usa los comandos `dtruss` u `otool` para diagnosticar problemas de carga de bibliotecas
4. Asegúrate de que XCode Command Line Tools estén correctamente instaladas

### Problemas Específicos de MacCatalyst

Para problemas de despliegue MacCatalyst:

1. Verifica que el objetivo CopyNativeLibrariesToMonoBundle esté correctamente implementado
2. Verifica que el directorio MonoBundle contenga todas las bibliotecas nativas necesarias
3. Asegúrate de que la aplicación tenga los entitlements apropiados para acceso a medios

### Optimización de Rendimiento

Para rendimiento óptimo:

1. Habilita la aceleración de hardware cuando esté disponible
2. Ajusta la resolución de video según las capacidades del dispositivo
3. Cierra y elimina objetos del SDK cuando ya no sean necesarios

## Recursos Adicionales

Para ejemplos de código, proyectos de ejemplo y más recursos técnicos:

- Visita el [repositorio GitHub de VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código
- Únete a la comunidad de desarrolladores de VisioForge para soporte y discusiones

Nuestro repositorio de ejemplos contiene ejemplos completos mostrando:

- Captura de video desde cámaras
- Implementaciones de reproducción de medios
- Flujos de trabajo de edición de video
- Pipelines avanzados de procesamiento de medios

## Conclusión

Los SDKs .NET de VisioForge proporcionan potentes capacidades de medios para desarrolladores macOS e iOS, permitiendo la creación de aplicaciones multimedia sofisticadas. Siguiendo esta guía de instalación y despliegue, has establecido las bases para construir aplicaciones de medios de alto rendimiento en las plataformas de Apple.

Para cualquier pregunta adicional o necesidades de soporte, por favor contacta a nuestro equipo de soporte técnico o visita nuestros foros para asistencia de la comunidad.

---
*Esta documentación se actualiza regularmente para reflejar las últimas características del SDK e información de compatibilidad.*