---
title: Instalar VisioForge .NET SDKs — NuGet, Visual Studio, Rider
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

- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) - Para capturar video de cámaras, grabación de pantalla y streaming
- [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) - Para edición de video, procesamiento y conversión de formato
- [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) - Para construir pipelines de procesamiento de medios personalizados
- [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) - Para crear reproductores de medios personalizados con características avanzadas

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

```cmd
Install-Package VisioForge.DotNet.VideoCapture   # Video Capture SDK .NET
Install-Package VisioForge.DotNet.VideoEdit      # Video Edit SDK .NET
Install-Package VisioForge.DotNet.MediaPlayer    # Media Player SDK .NET
Install-Package VisioForge.DotNet.MediaBlocks    # Media Blocks SDK .NET
```

Pueden requerirse paquetes adicionales específicos de UI dependiendo de su plataforma objetivo:

```cmd
Install-Package VisioForge.DotNet.Core.UI.MAUI
Install-Package VisioForge.DotNet.Core.UI.WinUI
Install-Package VisioForge.DotNet.Core.UI.Avalonia
Install-Package VisioForge.DotNet.Core.UI.Uno
```

## Integración y configuración del IDE

Nuestros SDKs se integran perfectamente con entornos de desarrollo .NET populares:

### Integración con Visual Studio

[Visual Studio](visual-studio.md) ofrece la experiencia más completa con nuestros SDKs:

- Soporte completo de IntelliSense para componentes del SDK
- Depuración integrada para componentes de procesamiento de medios
- Soporte de diseñador para controles visuales
- Gestión de paquetes NuGet

Para instrucciones detalladas de configuración de Visual Studio, consulte nuestra [guía de integración de Visual Studio](visual-studio.md).

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
<TargetFramework>net8.0-windows</TargetFramework>
```

Esto permite acceso a APIs específicas de Windows y frameworks de UI como WPF y Windows Forms.

#### Desarrollo Android

Los proyectos Android requieren el sufijo de framework `-android`:

```xml
<TargetFramework>net8.0-android</TargetFramework>
```

Asegúrese de que las cargas de trabajo de Android estén instaladas en su entorno de desarrollo:

```
dotnet workload install android
```

#### Desarrollo iOS

Las aplicaciones iOS deben usar el framework objetivo `-ios`:

```xml
<TargetFramework>net8.0-ios</TargetFramework>
```

El desarrollo iOS requiere un Mac con Xcode instalado, incluso cuando se usa Visual Studio en Windows.

#### Aplicaciones macOS

Las aplicaciones nativas de macOS usan el framework `-macos` o `-maccatalyst`:

```xml
<TargetFramework>net8.0-macos</TargetFramework>
```

Para aplicaciones .NET MAUI dirigidas a macOS, use:

```xml
<TargetFramework>net8.0-maccatalyst</TargetFramework>
```

#### Desarrollo Linux

Las aplicaciones Linux usan el framework objetivo estándar sin sufijo de plataforma:

```xml
<TargetFramework>net8.0</TargetFramework>
```

Asegúrese de que las cargas de trabajo .NET requeridas estén instaladas:

```
dotnet workload install linux
```

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
// Agregar referencia a VisioForge.DotNet.Core
using VisioForge.Core.UI.WinForms;

// En su formulario
videoView = new VideoView();
this.Controls.Add(videoView);
```

### Aplicaciones WPF

```csharp
// Agregar referencia a VisioForge.DotNet.Core
using VisioForge.Core.UI.WPF;

// En su XAML
<vf:VideoView x:Name="videoView" />
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

- Windows: Incluidas automáticamente con la instalación de configuración o paquetes NuGet
- macOS/iOS: Empaquetadas con paquetes NuGet pero requieren firma de app adecuada
- Android: Incluidas en paquetes NuGet con soporte de arquitectura apropiado
- Linux: Pueden requerir paquetes de sistema adicionales dependiendo de la distribución

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
