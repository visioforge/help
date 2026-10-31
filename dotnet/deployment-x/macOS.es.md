---
title: Manual de despliegue multiplataforma .NET para macOS
description: Despliegue sus aplicaciones con el SDK .NET de VisioForge en macOS: integración NSView, permisos, App Store y configuración de bibliotecas nativas.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - macOS
  - MAUI
  - GStreamer
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - VideoView
  - VideoViewGL

---

# Guía de despliegue para Apple macOS

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Los potentes SDKs .NET de VisioForge proporcionan capacidades completas de procesamiento de medios para desarrolladores macOS. Ya sea que esté construyendo aplicaciones de captura de video, reproductores de medios, editores de video o pipelines complejos de procesamiento de medios, nuestros SDKs ofrecen las herramientas que necesita para entregar soluciones de alta calidad en las plataformas de Apple.

El SDK de VisioForge proporciona soporte completo para desarrollo de aplicaciones macOS usando tecnologías .NET. Puede aprovechar este SDK para construir aplicaciones robustas de procesamiento de medios que se ejecuten nativamente en macOS, incluyendo soporte para arquitecturas Intel (x64) y Apple Silicon (ARM64).

Esta guía cubre todo lo que necesita saber para instalar, configurar y desplegar aplicaciones para entornos macOS y MacCatalyst usando el SDK de VisioForge. Ya sea que esté construyendo aplicaciones macOS tradicionales o soluciones multiplataforma usando frameworks como MAUI o Avalonia, este documento le ayudará a navegar el proceso de instalación y despliegue.

## Requisitos del sistema

Antes de comenzar el proceso de instalación y despliegue, asegúrese de que su entorno de desarrollo cumpla los siguientes requisitos:

### Requisitos de hardware

- Computadora Mac con procesador Intel (x64) o Apple Silicon (ARM64)
- Mínimo 8GB RAM (16GB recomendado para procesamiento de video)
- Espacio de disco suficiente para herramientas de desarrollo y activos de aplicación

### Requisitos de software

- macOS 10.15 (Catalina) o posterior (se recomienda la última versión)
  - macOS Monterey (12.x)
  - macOS Ventura (13.x)
  - macOS Sonoma (14.x)
  - Futuras versiones de macOS (con actualizaciones continuas)
- Xcode 12 o posterior con Command Line Tools instaladas
- .NET 6.0 SDK o posterior
- Visual Studio para Mac o JetBrains Rider (IDEs recomendados)

Para instalar XCode Command Line Tools, ejecute lo siguiente en Terminal:

```bash
xcode-select --install
```

## Soporte de arquitectura

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

Al apuntar a ambas arquitecturas, considere construir binarios universales que incluyan código tanto x64 como ARM64. Este enfoque asegura que su aplicación se ejecute nativamente en cualquiera de las plataformas sin depender de la traducción Rosetta 2. Una compilación Release de una aplicación `net*-macos` ya genera ambas arquitecturas; fije los identificadores siguientes cuando los necesite en Debug o quiera declararlos explícitamente.

Para builds de binarios universales apuntando tanto a Intel como Apple Silicon:

```xml
<PropertyGroup>
  <RuntimeIdentifiers>osx-x64;osx-arm64</RuntimeIdentifiers>
  <UseHardenedRuntime>true</UseHardenedRuntime>
</PropertyGroup>
```

Los proyectos `Microsoft.NET.Sdk.Web` (Blazor Server y otros hosts de ASP.NET Core) no pueden usar esa receta. En macOS, Release genera por defecto un `.app` universal, y el manifiesto de static web assets de un proyecto web se escribe entonces solo por identificador de runtime, mientras que la compilación que lo lee lo busca en la carpeta común: `dotnet build -c Release` falla con `Manifest file at 'obj/Release/net10.0-macos/staticwebassets.build.json' not found`, y definir `RuntimeIdentifiers` explícitamente rompe también Debug del mismo modo. Fije un único identificador de runtime — un host del SDK Web necesita además su propio identificador de bundle y no usar server GC — y publique una arquitectura a la vez:

```xml
<PropertyGroup>
  <ApplicationId>com.example.myapp</ApplicationId>
  <ServerGarbageCollection>false</ServerGarbageCollection>
  <RuntimeIdentifier Condition="$([MSBuild]::IsOsPlatform('OSX')) and '$(RuntimeIdentifier)' == ''">$(NETCoreSdkRuntimeIdentifier)</RuntimeIdentifier>
</PropertyGroup>
```

`dotnet publish -c Release -r osx-x64` (o `-r osx-arm64`) genera entonces un bundle para esa arquitectura. Si el proyecto conserva además el `RuntimeIdentifiers` en plural, la compilación pasa a ser multi-RID y genera un bundle por arquitectura.

## Tecnologías principales

Los SDKs .NET de VisioForge aprovechan varias tecnologías clave para entregar capacidades de medios de alto rendimiento en macOS:

### Integración GStreamer

Todos los SDKs de VisioForge utilizan GStreamer como el framework subyacente para reproducción y codificación de video/audio. GStreamer proporciona:

- Procesamiento de medios acelerado por hardware
- Amplia compatibilidad de formatos
- Pipeline de reproducción optimizado
- Capacidades eficientes de codificación

Los componentes de GStreamer se instalan automáticamente a través de nuestros paquetes redistributables, eliminando la necesidad de configuración manual.

## Instalación y despliegue de paquetes NuGet

El método principal para desplegar componentes del SDK de VisioForge en aplicaciones macOS es a través de paquetes NuGet. Estos paquetes incluyen todas las bibliotecas administradas y no administradas necesarias para su aplicación.

### Paquetes NuGet esenciales

Para aplicaciones nativas macOS, agregue estos paquetes principales:

1. **Paquete principal del SDK** (según sus necesidades):
   - `VisioForge.DotNet.VideoCapture` para aplicaciones de captura de cámara
   - `VisioForge.DotNet.VideoEdit` para aplicaciones de edición de video
   - `VisioForge.DotNet.MediaPlayer` para aplicaciones de reproducción de medios
   - `VisioForge.DotNet.MediaBlocks` para pipelines avanzados de procesamiento de medios

2. **Paquete de UI**:
   - `VisioForge.DotNet.Core` incluye controles de UI específicos de Apple

3. **Redistributable de plataforma**:
   - `VisioForge.CrossPlatform.Core.macOS` para bibliotecas nativas y dependencias

### Aplicaciones macOS

Para aplicaciones macOS estándar apuntando al framework `netX.0-macos` (donde X representa la versión de .NET), use el siguiente paquete NuGet:

- [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS)

Este paquete contiene:

- Bibliotecas nativas para procesamiento de medios
- Componentes GStreamer para reproducción y codificación de medios
- Ensamblados de interfaz para integración .NET
- Binarios tanto x64 como ARM64

### Visión por computadora con OpenCV

Para aplicaciones que requieran procesamiento de imágenes y visión por computadora avanzados, añada:

- [VisioForge.CrossPlatform.OpenCV.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.macOS)

El paquete incluye un único plugin de GStreamer universal (x64 + ARM64) con OpenCV enlazado estáticamente, por lo que no hay bibliotecas OpenCV adicionales que desplegar, además de los ficheros de cascadas Haar que `CVFaceDetectBlock`, `CVFaceBlurBlock` y `CVHandDetectBlock` cargan por defecto. Proporciona los Media Blocks del espacio de nombres `VisioForge.Core.MediaBlocks.OpenCV`:

- Transformación de imagen: `CVDewarpBlock`, `CVDilateBlock`, `CVErodeBlock`
- Detección de bordes y características: `CVEdgeDetectBlock`, `CVLaplaceBlock`, `CVSobelBlock`
- Procesamiento facial: `CVFaceBlurBlock`, `CVFaceDetectBlock`
- Detección de movimiento: `CVMotionCellsBlock`
- Reconocimiento de objetos: `CVTemplateMatchBlock`, `CVHandDetectBlock`
- Mejora de imagen: `CVEqualizeHistogramBlock`, `CVSmoothBlock`
- Seguimiento y superposición: `CVTrackerBlock`, `CVTextOverlayBlock`
- Estabilización: `VideoStabilizationBlock`

### Primeros pasos con proyectos nativos macOS

Para comenzar a desarrollar aplicaciones nativas macOS con SDKs de VisioForge:

1. **Cree un nuevo proyecto macOS** en su IDE preferido (Visual Studio para Mac o JetBrains Rider)
2. **Agregue los paquetes NuGet requeridos** (como se detalla arriba)
3. **Configure los ajustes del proyecto** para su arquitectura objetivo

## Aplicaciones MacCatalyst y MAUI

### Desarrollo multiplataforma con .NET MAUI

.NET Multi-platform App UI (MAUI) permite desarrollar aplicaciones que se ejecutan sin problemas en macOS, iOS, Android y Windows desde una única base de código. VisioForge proporciona soporte completo para desarrollo MAUI a través de paquetes y controles especializados.

Para aplicaciones MacCatalyst (incluyendo proyectos MAUI) apuntando al framework `netX.0-maccatalyst`, use:

- [VisioForge.CrossPlatform.Core.macCatalyst](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macCatalyst)

Para los bloques OpenCV (`VisioForge.Core.MediaBlocks.OpenCV`, incluido `VideoStabilizationBlock`), agregue también [VisioForge.CrossPlatform.OpenCV.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.macOS), el mismo paquete que usa macOS: su plugin funciona en Mac Catalyst sin cambios. En un proyecto MAUI con varios destinos, manténgalo dentro de la condición `-maccatalyst`, como en el fragmento de abajo, para que no se copie en las compilaciones de iOS y Android.

### Configuración de paquetes MAUI

Para proyectos MAUI apuntando a macOS (a través de MacCatalyst), agregue estos paquetes:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="[2026.10.26]"/>
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="[2026.10.28]"/>
  <!-- Solo para los bloques OpenCV -->
  <PackageReference Include="VisioForge.CrossPlatform.OpenCV.macOS" Version="[2026.9.11]"/>
</ItemGroup>
```

### Configuración del proyecto MAUI

1. **Inicialice el SDK en MauiProgram.cs**:

```csharp
builder
  .UseMauiApp<App>()
  .UseSkiaSharp()
  .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

2. **Agregue el control VideoView en XAML**:

```xml
xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"

<vf:VideoView Grid.Row="0"
              HorizontalOptions="FillAndExpand"
              VerticalOptions="FillAndExpand"
              x:Name="videoView"
              Background="Black"/>
```

Las aplicaciones MacCatalyst requieren configuración adicional para asegurar que las bibliotecas nativas se incluyan correctamente en el bundle de la aplicación. Agregue el siguiente objetivo de build personalizado a su archivo de proyecto:

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

Para detalles completos de integración MAUI, consulte nuestra página de documentación dedicada de [MAUI](../install/maui.md).

## Opciones de framework de UI

El SDK de VisioForge soporta múltiples frameworks de UI para desarrollo macOS:

### UI nativa de macOS

Para aplicaciones macOS tradicionales, el SDK proporciona controles `VideoViewGL` que se integran con el framework nativo AppKit. Estos controles proporcionan renderizado de video de alto rendimiento usando OpenGL.

### MAUI

Para aplicaciones MAUI multiplataforma, use el paquete [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI), que proporciona vistas de video compatibles con MAUI.

### Avalonia

Para aplicaciones Avalonia UI, el paquete [VisioForge.DotNet.Core.UI.Avalonia](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.Avalonia) ofrece controles de video compatibles con Avalonia.

## Configuración del entorno de desarrollo

### Integración con JetBrains Rider

JetBrains Rider proporciona una excelente experiencia de desarrollo para aplicaciones macOS e iOS usando los SDKs de VisioForge:

1. Cree un nuevo proyecto en Rider apuntando a macOS o iOS
2. Agregue los paquetes NuGet requeridos a través del Package Manager
3. Configure los ajustes del proyecto para su plataforma objetivo
4. Agregue controles de UI e implemente la funcionalidad del SDK

Para instrucciones detalladas de configuración de Rider, consulte nuestra [guía de integración de Rider](../install/rider.md).

### Configuración de Visual Studio para Mac

A pesar de su deprecación, Visual Studio para Mac aún funciona para desarrollar aplicaciones macOS e iOS con los SDKs de VisioForge:

1. Cree un nuevo proyecto en Visual Studio para Mac
2. Agregue paquetes NuGet a través del NuGet Package Manager
3. Configure los ajustes de build necesarios
4. Agregue controles de UI a la interfaz de su aplicación

Para instrucciones detalladas de Visual Studio para Mac, consulte nuestra [guía de Visual Studio para Mac](../install/visual-studio-mac.md).

## Inicialización y limpieza del SDK

Los motores X en el SDK de VisioForge requieren inicialización y limpieza explícitas para gestionar recursos correctamente:

```csharp
// Inicializar SDK al inicio de la aplicación
VisioForge.Core.VisioForgeX.InitSDK();

// Usar componentes del SDK...

// Limpiar recursos antes de salir de la aplicación
VisioForge.Core.VisioForgeX.DestroySDK();
```

Para inicialización y limpieza asíncronas, use las variantes async:

```csharp
// Inicialización async
await VisioForge.Core.VisioForgeX.InitSDKAsync();

// Limpieza async
await VisioForge.Core.VisioForgeX.DestroySDKAsync();
```

## Aplicaciones de consola en macOS

Un ejecutable de consola `net10.0-macos` es una aplicación de AppKit. Su hilo principal lleva un contexto de sincronización de AppKit, y nadie ejecuta un bucle de ejecución de `NSApplication` para él. `async Main` bloquea ese mismo hilo, así que la continuación después del primer `await` — `await VisioForgeX.InitSDKAsync()` o `await UniversalSourceSettings.CreateAsync(...)` — nunca se ejecuta. El proceso se queda colgado y no imprime nada.

Quite el contexto al inicio de `Main` y restaure el directorio de trabajo. El proceso se inicia dentro del paquete `.app`, de modo que una ruta relativa no encuentra un archivo indicado desde el shell.

```csharp
using System.IO;
using System.Threading;

#if __MACOS__
// Un ejecutable de macOS tiene un contexto de AppKit sin bucle de ejecución; las continuaciones enviadas a él nunca se ejecutan.
SynchronizationContext.SetSynchronizationContext(null);
// El proceso se inicia dentro del paquete .app; resuelva las rutas relativas respecto a la carpeta del shell.
var shellDir = Environment.GetEnvironmentVariable("PWD");
if (Directory.Exists(shellDir)) Environment.CurrentDirectory = shellDir;
#endif
```

`dotnet run` inicia el paquete mediante `open`, salvo que el proyecto establezca `RunWithOpen` en false. `open` no pasa `PWD` y no muestra la salida en el terminal.

```xml
<PropertyGroup>
  <RunWithOpen Condition="$([MSBuild]::IsOsPlatform('OSX'))">false</RunWithOpen>
</PropertyGroup>
```

Un host de Blazor Server con el destino `net10.0-macos` necesita la misma limpieza del contexto y el mismo ajuste de `RunWithOpen`. No cambie su directorio de trabajo: ASP.NET toma de él la raíz del contenido. También necesita un único `RuntimeIdentifier` en lugar de la receta de binario universal anterior, que rompe su manifiesto de static web assets.

## Solución de problemas comunes

### Fallos de carga de bibliotecas nativas

Si su aplicación falla al cargar bibliotecas nativas:

1. Verifique que todos los paquetes NuGet requeridos estén correctamente instalados
2. Revise la estructura del bundle de la aplicación para asegurar que las bibliotecas estén en la ubicación correcta
3. Use los comandos `dtruss` u `otool` para diagnosticar problemas de carga de bibliotecas
4. Asegúrese de que XCode Command Line Tools estén correctamente instaladas

### Problemas específicos de MacCatalyst

Para problemas de despliegue MacCatalyst:

1. Verifique que el objetivo CopyNativeLibrariesToMonoBundle esté correctamente implementado
2. Verifique que el directorio MonoBundle contenga todas las bibliotecas nativas necesarias
3. Asegúrese de que la aplicación tenga los entitlements apropiados para acceso a medios

### Optimización de rendimiento

Para rendimiento óptimo:

1. Habilite la aceleración de hardware cuando esté disponible
2. Ajuste la resolución de video según las capacidades del dispositivo
3. Cierre y elimine objetos del SDK cuando ya no sean necesarios

## Recursos adicionales

Para ejemplos de código, proyectos de ejemplo y más recursos técnicos:

- Visite el [repositorio GitHub de VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código
- Únase a la comunidad de desarrolladores de VisioForge para soporte y discusiones

Nuestro repositorio de ejemplos contiene ejemplos completos mostrando:

- Captura de video desde cámaras
- Implementaciones de reproducción de medios
- Flujos de trabajo de edición de video
- Pipelines avanzados de procesamiento de medios

## Conclusión

Los SDKs .NET de VisioForge proporcionan potentes capacidades de medios para desarrolladores macOS e iOS, permitiendo la creación de aplicaciones multimedia sofisticadas. Siguiendo esta guía de instalación y despliegue, ha establecido las bases para construir aplicaciones de medios de alto rendimiento en las plataformas de Apple.

Para cualquier pregunta adicional o necesidades de soporte, por favor contacte a nuestro equipo de soporte técnico o visite nuestros foros para asistencia de la comunidad.

---
*Esta documentación se actualiza regularmente para reflejar las últimas características del SDK e información de compatibilidad.*