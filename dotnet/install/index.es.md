---
title: Instalar .NET SDKs con NuGet, Visual Studio y Rider
description: Instalación por NuGet o archivos. Frameworks destino para Windows, macOS, iOS, Android y Linux. Inicialización SDK y dependencias nativas.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - VideoView
  - VideoCaptureCore
  - VideoCaptureCoreX

order: 21
sidebar_label: Instalación
---

# Guía de instalación de SDKs .NET de VisioForge

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge ofrece potentes SDKs multimedia para desarrolladores .NET que permiten capacidades avanzadas de captura, edición, reproducción y procesamiento de medios en sus aplicaciones. Esta guía cubre todo lo que necesita saber para instalar y configurar correctamente nuestros SDKs en su entorno de desarrollo.

## SDKs .NET disponibles

VisioForge proporciona varios SDKs especializados para abordar diferentes necesidades multimedia:

- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) - Para capturar video de cámaras, grabación de pantalla y streaming ([documentación](../videocapture/index.md))
- [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) - Para edición de video, procesamiento y conversión de formato ([documentación](../videoedit/index.md))
- [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) - Para construir pipelines de procesamiento de medios personalizados ([documentación](../mediablocks/index.md))
- [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) - Para crear reproductores de medios personalizados con características avanzadas ([documentación](../mediaplayer/index.md))

## Métodos de instalación

Puede instalar nuestros SDKs usando dos métodos principales:

### Usar archivos de instalación

El método de instalación con archivo de configuración se recomienda para entornos de desarrollo Windows. Este enfoque:

1. Instala automáticamente todas las dependencias requeridas
2. Configura la integración con Visual Studio
3. Incluye proyectos de ejemplo para ayudarle a comenzar rápidamente
4. Proporciona documentación y recursos adicionales

Los archivos de instalación pueden descargarse desde las páginas de productos SDK respectivas en nuestro sitio web.

### Usar paquetes NuGet

Para desarrollo multiplataforma o pipelines de CI/CD, nuestros paquetes NuGet ofrecen flexibilidad y fácil integración.

Instale el paquete del SDK que haya adquirido: cada uno arrastra `VisioForge.DotNet.Core` como
dependencia, así que nunca hay que referenciar `Core` de forma directa:

```powershell
Install-Package VisioForge.DotNet.VideoCapture   # Video Capture SDK .NET
Install-Package VisioForge.DotNet.VideoEdit      # Video Edit SDK .NET
Install-Package VisioForge.DotNet.MediaPlayer    # Media Player SDK .NET
Install-Package VisioForge.DotNet.MediaBlocks    # Media Blocks SDK .NET
```

Estos paquetes contienen solo el SDK administrado. El motor nativo llega en un paquete redist aparte, que usted referencia junto a él:

| Motor | Plataforma | Paquete redist |
|-------|------------|----------------|
| Motores X (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`, `MediaBlocksPipeline`) | Windows | `VisioForge.CrossPlatform.Core.Windows.x64` (`.x86` o `.arm64` para otras CPU; en ARM64, la reproducción de MP3 también necesita `VisioForge.CrossPlatform.Libav.Windows.arm64`, y NDI no está disponible) |
| Motores X | macOS, Mac Catalyst | `VisioForge.CrossPlatform.Core.macOS` |
| Motores X | Android | `VisioForge.CrossPlatform.Core.Android` |
| Motores X | iOS | `VisioForge.CrossPlatform.Core.iOS` |
| Motores X | Linux | `VisioForge.CrossPlatform.Core.Linux.x64` o `VisioForge.CrossPlatform.Core.Linux.arm64`, más GStreamer 1.20+ de la distribución (1.22+ para algunas funciones) |
| `VideoCaptureCore` (solo Windows) | Windows | `VisioForge.DotNet.Core.Redist.VideoCapture.x64` (`.x86` para aplicaciones de 32 bits) |
| `MediaPlayerCore` (solo Windows) | Windows | `VisioForge.DotNet.Core.Redist.MediaPlayer.x64` y `VisioForge.DotNet.Core.Redist.LAV.x64`. LAV es el `Source_Mode` predeterminado, el modo GPU también usa LAV, y también `GetFileThumbnail`: omita el paquete LAV solo cuando la aplicación no use ninguno de ellos. Añada `VisioForge.DotNet.Core.Redist.FFMPEG.x64` cuando la aplicación use el modo FFMPEG o Blu-ray, y `VisioForge.DotNet.Core.Redist.VLC.x64` para `File_VLC` o `HTTP_RTSP_VLC` (`.x86` de cada paquete para aplicaciones de 32 bits). Una aplicación ARM64 usa `VisioForge.DotNet.Core.Redist.Base.arm64` y `VisioForge.DotNet.Core.Redist.FFMPEG.arm64` con `MediaPlayerSourceMode.FFMPEG` (sin paquete MediaPlayer ni LAV, así que sin modo GPU ni `GetFileThumbnail`) |
| `VideoEditCore` (solo Windows) | Windows | `VisioForge.DotNet.Core.Redist.VideoEdit.x64` y `VisioForge.DotNet.Core.Redist.FFMPEG.x64` para abrir archivos de entrada MP4, MKV y otros (`.x86` para aplicaciones de 32 bits) |

Por ejemplo, una aplicación Windows x64 con los motores X añade:

```powershell
Install-Package VisioForge.CrossPlatform.Core.Windows.x64
Install-Package VisioForge.CrossPlatform.Libav.Windows.x64   # recomendado: decodificadores y codificadores FFmpeg (libav)
```

Algunas plataformas necesitan un paso además del paquete: Android también requiere el proyecto de enlaces Java ([despliegue en Android](../deployment-x/Android.md)), y Mac Catalyst un destino de compilación que copia las bibliotecas nativas al paquete de la aplicación ([despliegue en macOS](../deployment-x/macOS.md)). Con los motores solo para Windows, una aplicación AnyCPU despliega los paquetes x86 y x64. Sus paquetes de motor y LAV no tienen compilación ARM64, así que en Windows on Arm, donde una aplicación AnyCPU suele ejecutarse como ARM64, compile la aplicación como x64 o use los motores X. Para reproducir archivos de forma nativa en ARM64, `MediaPlayerCore` usa `MediaPlayerSourceMode.FFMPEG` y los paquetes ARM64 de su fila ([Windows ARM64](../general/guides/windows-arm64.md)). Los paquetes opcionales se enumeran en la [guía de despliegue](../deployment-x/index.md) para los motores X y en las páginas de despliegue de [Video Capture SDK .NET](../videocapture/deployment.md), [Media Player SDK .NET](../mediaplayer/deployment.md) y [Video Edit SDK .NET](../videoedit/deployment.md) para los motores solo para Windows.

Pueden requerirse paquetes adicionales específicos de UI dependiendo de su plataforma objetivo:

```powershell
Install-Package VisioForge.DotNet.Core.UI.MAUI
Install-Package VisioForge.DotNet.Core.UI.WinUI
Install-Package VisioForge.DotNet.Core.UI.Avalonia
Install-Package VisioForge.DotNet.Core.UI.Uno
```

WPF y Windows Forms no necesitan ningún paquete de UI: su `VideoView` está en `VisioForge.Core`, que ya incluyen todos los paquetes del SDK indicados arriba. Los paquetes independientes `VisioForge.DotNet.Core.UI.WPF` y `VisioForge.DotNet.Core.UI.WinForms` no son necesarios.

## Integración y configuración del IDE

Nuestros SDKs se integran perfectamente con entornos de desarrollo .NET populares:

### Integración con Visual Studio

[Visual Studio](visual-studio.md) ofrece la experiencia más completa con nuestros SDKs:

- Soporte completo de IntelliSense para componentes del SDK
- Depuración integrada para componentes de procesamiento de medios
- Soporte de diseñador para controles visuales
- Gestión de paquetes NuGet

Para instrucciones detalladas de configuración de Visual Studio, consulte nuestra [guía de integración de Visual Studio](visual-studio.md). Para empezar con una aplicación que ya funciona, use las [plantillas de proyecto VisioForge](project-templates.md) para Visual Studio 2026 y `dotnet new`.

### Integración con JetBrains Rider

[Rider](rider.md) proporciona excelente soporte de desarrollo multiplataforma:

- Completado de código completo para APIs del SDK
- Características de navegación inteligente para explorar clases del SDK
- Gestión integrada de paquetes NuGet
- Capacidades de depuración multiplataforma

Para instrucciones específicas de Rider, visite nuestra [documentación de integración de Rider](rider.md).

### Visual Studio para Mac

Los usuarios de [Visual Studio para Mac](visual-studio-mac.md) pueden desarrollar aplicaciones para macOS, iOS y Android:

- Gestor de paquetes NuGet integrado para instalar componentes del SDK
- Plantillas de proyecto para configuración rápida
- Herramientas de depuración integradas

Aprende más en nuestra [guía de configuración de Visual Studio para Mac](visual-studio-mac.md).

## Configuración específica de plataforma

### Configuración del framework objetivo

Cada sistema operativo requiere configuraciones específicas de framework objetivo para compatibilidad óptima:

#### Aplicaciones Windows

Las aplicaciones Windows deben usar el sufijo de framework objetivo `-windows`:

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

Esto permite acceso a APIs específicas de Windows y frameworks de UI como WPF y Windows Forms.

#### Desarrollo Android

Los proyectos Android requieren el sufijo de framework `-android`:

```xml
<TargetFramework>net10.0-android</TargetFramework>
```

Asegúrese de que las cargas de trabajo de Android estén instaladas en su entorno de desarrollo:

```
dotnet workload install android
```

#### Desarrollo iOS

Las aplicaciones iOS deben usar el framework objetivo `-ios`:

```xml
<TargetFramework>net10.0-ios</TargetFramework>
```

El desarrollo iOS requiere un Mac con Xcode instalado, incluso cuando se usa Visual Studio en Windows.

#### Aplicaciones macOS

Las aplicaciones nativas de macOS usan el framework `-macos` o `-maccatalyst`:

```xml
<TargetFramework>net10.0-macos</TargetFramework>
```

Para aplicaciones .NET MAUI dirigidas a macOS, use:

```xml
<TargetFramework>net10.0-maccatalyst</TargetFramework>
```

#### Desarrollo Linux

Las aplicaciones Linux usan el framework objetivo estándar sin sufijo de plataforma:

```xml
<TargetFramework>net10.0</TargetFramework>
```

Añada `VisioForge.CrossPlatform.Core.Linux.x64`, o `VisioForge.CrossPlatform.Core.Linux.arm64` en ARM64. Solo contiene las bibliotecas nativas propias del SDK: instale GStreamer 1.20 o posterior (1.22 o posterior para algunas funciones, como los bloques OpenCV y el canal de retorno de audio ONVIF) desde los paquetes de la distribución, como se describe en [despliegue en Ubuntu](../deployment-x/Ubuntu.md).

## Soporte de frameworks especiales

### Aplicaciones .NET MAUI

Los [proyectos MAUI](maui.md) requieren configuración especial:

- Agregar el paquete NuGet `VisioForge.DotNet.Core.UI.MAUI`
- Configurar permisos específicos de plataforma en su proyecto
- Usar controles de vista de video específicos de MAUI

Consulte nuestra [guía detallada de MAUI](maui.md) para instrucciones completas.

### Framework Avalonia UI

Los [proyectos Avalonia](avalonia.md) proporcionan una alternativa de UI multiplataforma:

- Instalar el paquete `VisioForge.DotNet.Core.UI.Avalonia`
- Usar controles de renderizado de video específicos de Avalonia
- Configurar dependencias específicas de plataforma

Nuestra [guía de integración de Avalonia](avalonia.md) proporciona instrucciones de configuración completas.

### Aplicaciones Uno Platform

Los [proyectos de Uno Platform](uno.md) permiten construir aplicaciones para Windows, Android, iOS, macOS y Linux desde una única base de código:

- Agregar el paquete NuGet `VisioForge.DotNet.Core.UI.Uno`
- Usar controles de vista de video específicos de Uno Platform
- Configurar redistribuibles específicos de plataforma

Consulte nuestra [guía de Uno Platform](uno.md) para instrucciones de configuración completas.

### Unity

La integración con [Unity](unity.md) usa un `.unitypackage` listo para importar en lugar de NuGet:

- Autónomo para Unity 6 en Windows x64 — SDK gestionado, runtime nativo y escenas de ejemplo
- Sin compilación desde el código fuente, sin NuGet y sin dependencias externas
- El video se renderiza en un `RawImage` de Unity mediante los scripts incluidos

Consulte nuestra [guía de instalación de Unity](unity.md) para la configuración paso a paso.

## Inicialización del SDK para motores multiplataforma

Nuestros SDKs incluyen tanto motores DirectShow específicos de Windows (como `VideoCaptureCore`) como motores X multiplataforma (como `VideoCaptureCoreX`). Los motores X requieren inicialización y limpieza explícitas.

### Inicializar el SDK

Antes de usar cualquier componente de motor X, inicializa el SDK:

```csharp
// Inicializar al inicio de la aplicación
VisioForge.Core.VisioForgeX.InitSDK();

// O usar la versión async
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

### Limpiar recursos

Cuando su aplicación termine, libere correctamente los recursos:

```csharp
// Limpiar al salir de la aplicación (sync — DestroySDK no tiene variante async)
VisioForge.Core.VisioForgeX.DestroySDK();
```

No inicializar o limpiar correctamente puede resultar en fugas de memoria o comportamiento inestable.

## Controles de renderizado de video

Cada framework de UI requiere controles de vista de video específicos para mostrar contenido multimedia:

### Windows Forms

```csharp
// Al principio de Form1.cs. El VideoView de WinForms está en VisioForge.Core: no requiere paquete adicional
using VisioForge.Core.UI.WinForms;

// Dentro de la clase Form1: un campo, agregado al formulario en el constructor existente
private readonly VideoView videoView = new VideoView { Dock = DockStyle.Fill };

public Form1()
{
    InitializeComponent();
    Controls.Add(videoView); // agregue esta línea al constructor existente
}
```

### Aplicaciones WPF

```xml
<!-- MainWindow.xaml. El VideoView de WPF está en VisioForge.Core: no requiere paquete adicional -->
<Window ...
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core">
    <vf:VideoView x:Name="videoView" />
</Window>
```

### Aplicaciones MAUI

```csharp
// Agregar referencia a VisioForge.DotNet.Core.UI.MAUI
using VisioForge.Core.UI.MAUI;

// En su XAML
<vf:VideoView x:Name="videoView" />
```

### Avalonia UI

```csharp
// Agregar referencia a VisioForge.DotNet.Core.UI.Avalonia
using VisioForge.Core.UI.Avalonia;

// En su XAML
<vf:VideoView Name="videoView" />
```

## Gestión de dependencias nativas

Nuestros SDKs aprovechan bibliotecas nativas para rendimiento óptimo. Estas dependencias deben gestionarse correctamente para el despliegue:

- Windows: el instalador las instala en el equipo de desarrollo. La aplicación que usted distribuye lleva el paquete redist de su motor y su CPU de la [tabla anterior](#usar-paquetes-nuget); sus bibliotecas se copian junto a la aplicación al compilar
- macOS/iOS: `VisioForge.CrossPlatform.Core.macOS` y `VisioForge.CrossPlatform.Core.iOS` contienen las bibliotecas nativas, GStreamer incluido; la aplicación sigue necesitando una firma adecuada, y Mac Catalyst un destino de compilación (consulte [despliegue en macOS](../deployment-x/macOS.md))
- Android: `VisioForge.CrossPlatform.Core.Android` contiene las bibliotecas nativas para todas las arquitecturas de Android; la aplicación también requiere el proyecto de enlaces Java (consulte [despliegue en Android](../deployment-x/Android.md))
- Linux: `VisioForge.CrossPlatform.Core.Linux.x64` o `.arm64` contiene solo las bibliotecas nativas propias del SDK; GStreamer proviene de los paquetes de la distribución (consulte [despliegue en Ubuntu](../deployment-x/Ubuntu.md))

Para instrucciones detalladas de despliegue, consulte nuestra [guía de despliegue](../deployment-x/index.md).

## Solución de problemas comunes de instalación

Si encuentra problemas durante la instalación:

1. Verifique la compatibilidad del framework objetivo con su tipo de proyecto
2. Asegúrese de que todas las cargas de trabajo requeridas estén instaladas (`dotnet workload list`)
3. Verifique conflictos de dependencias en su proyecto
4. Confirme la inicialización adecuada del SDK para motores X
5. Revise los requisitos específicos de plataforma en nuestra documentación

## Código de ejemplo y recursos

Mantenemos una extensa colección de aplicaciones de ejemplo en nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ayudarte a comenzar rápidamente con nuestros SDKs.

Estos ejemplos cubren escenarios comunes como:

- Captura de video de cámaras y pantallas
- Reproducción de medios con controles personalizados
- Edición y procesamiento de video
- Desarrollo multiplataforma

Visite nuestro repositorio para los últimos ejemplos de código y mejores prácticas para usar nuestros SDKs.

---

Para soporte adicional o preguntas, por favor contacte a nuestro equipo de soporte técnico o visite nuestro portal de documentación.
