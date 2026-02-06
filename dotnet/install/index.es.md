---
title: Guía de Instalación de SDKs .NET
description: Instala SDKs multimedia .NET en Visual Studio y Rider con configuración específica de plataforma para Windows, macOS, iOS, Android y Linux.

---

# Guía de Instalación de SDKs .NET de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge ofrece potentes SDKs multimedia para desarrolladores .NET que permiten capacidades avanzadas de captura, edición, reproducción y procesamiento de medios en tus aplicaciones. Esta guía cubre todo lo que necesitas saber para instalar y configurar correctamente nuestros SDKs en tu entorno de desarrollo.

## SDKs .NET Disponibles

VisioForge proporciona varios SDKs especializados para abordar diferentes necesidades multimedia:

- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) - Para capturar video de cámaras, grabación de pantalla y streaming
- [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net) - Para edición de video, procesamiento y conversión de formato
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) - Para construir pipelines de procesamiento de medios personalizados
- [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) - Para crear reproductores de medios personalizados con características avanzadas

## Métodos de Instalación

Puedes instalar nuestros SDKs usando dos métodos principales:

### Usando Archivos de Instalación

El método de instalación con archivo de configuración se recomienda para entornos de desarrollo Windows. Este enfoque:

1. Instala automáticamente todas las dependencias requeridas
2. Configura la integración con Visual Studio
3. Incluye proyectos de ejemplo para ayudarte a comenzar rápidamente
4. Proporciona documentación y recursos adicionales

Los archivos de instalación pueden descargarse desde las páginas de productos SDK respectivas en nuestro sitio web.

### Usando Paquetes NuGet

Para desarrollo multiplataforma o pipelines de CI/CD, nuestros paquetes NuGet ofrecen flexibilidad y fácil integración:

```cmd
Install-Package VisioForge.DotNet.Core
```

Pueden requerirse paquetes adicionales específicos de UI dependiendo de tu plataforma objetivo:

```cmd
Install-Package VisioForge.DotNet.Core.UI.MAUI
Install-Package VisioForge.DotNet.Core.UI.WinUI
Install-Package VisioForge.DotNet.Core.UI.Avalonia
Install-Package VisioForge.DotNet.Core.UI.Uno
```

## Integración y Configuración del IDE

Nuestros SDKs se integran perfectamente con entornos de desarrollo .NET populares:

### Integración con Visual Studio

[Visual Studio](visual-studio.md) ofrece la experiencia más completa con nuestros SDKs:

- Soporte completo de IntelliSense para componentes del SDK
- Depuración integrada para componentes de procesamiento de medios
- Soporte de diseñador para controles visuales
- Gestión de paquetes NuGet

Para instrucciones detalladas de configuración de Visual Studio, consulta nuestra [guía de integración de Visual Studio](visual-studio.md).

### Integración con JetBrains Rider

[Rider](rider.md) proporciona excelente soporte de desarrollo multiplataforma:

- Completado de código completo para APIs del SDK
- Características de navegación inteligente para explorar clases del SDK
- Gestión integrada de paquetes NuGet
- Capacidades de depuración multiplataforma

Para instrucciones específicas de Rider, visita nuestra [documentación de integración de Rider](rider.md).

### Visual Studio para Mac

Los usuarios de [Visual Studio para Mac](visual-studio-mac.md) pueden desarrollar aplicaciones para macOS, iOS y Android:

- Gestor de paquetes NuGet integrado para instalar componentes del SDK
- Plantillas de proyecto para configuración rápida
- Herramientas de depuración integradas

Aprende más en nuestra [guía de configuración de Visual Studio para Mac](visual-studio-mac.md).

## Configuración Específica de Plataforma

### Configuración del Framework Objetivo

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

Asegúrate de que las cargas de trabajo de Android estén instaladas en tu entorno de desarrollo:

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

Para aplicaciones .NET MAUI dirigidas a macOS, usa:

```xml
<TargetFramework>net8.0-maccatalyst</TargetFramework>
```

#### Desarrollo Linux

Las aplicaciones Linux usan el framework objetivo estándar sin sufijo de plataforma:

```xml
<TargetFramework>net8.0</TargetFramework>
```

Asegúrate de que las cargas de trabajo .NET requeridas estén instaladas:

```
dotnet workload install linux
```

## Soporte de Frameworks Especiales

### Aplicaciones .NET MAUI

Los [proyectos MAUI](maui.md) requieren configuración especial:

- Agregar el paquete NuGet `VisioForge.DotNet.Core.UI.MAUI`
- Configurar permisos específicos de plataforma en tu proyecto
- Usar controles de vista de video específicos de MAUI

Consulta nuestra [guía detallada de MAUI](maui.md) para instrucciones completas.

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

Consulta nuestra [guía de Uno Platform](uno.md) para instrucciones de configuración completas.

## Inicialización del SDK para Motores Multiplataforma

Nuestros SDKs incluyen tanto motores DirectShow específicos de Windows (como `VideoCaptureCore`) como motores X multiplataforma (como `VideoCaptureCoreX`). Los motores X requieren inicialización y limpieza explícitas.

### Inicializando el SDK

Antes de usar cualquier componente de motor X, inicializa el SDK:

```csharp
// Inicializar al inicio de la aplicación
VisioForge.Core.VisioForgeX.InitSDK();

// O usar la versión async
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

### Limpiando Recursos

Cuando tu aplicación termine, libera correctamente los recursos:

```csharp
// Limpiar al salir de la aplicación
VisioForge.Core.VisioForgeX.DestroySDK();

// O usar la versión async
await VisioForge.Core.VisioForgeX.DestroySDKAsync();
```

No inicializar o limpiar correctamente puede resultar en fugas de memoria o comportamiento inestable.

## Controles de Renderizado de Video

Cada framework de UI requiere controles de vista de video específicos para mostrar contenido multimedia:

### Windows Forms

```csharp
// Agregar referencia a VisioForge.DotNet.Core
using VisioForge.Core.UI.WinForms;

// En tu formulario
videoView = new VideoView();
this.Controls.Add(videoView);
```

### Aplicaciones WPF

```csharp
// Agregar referencia a VisioForge.DotNet.Core
using VisioForge.Core.UI.WPF;

// En tu XAML
<vf:VideoView x:Name="videoView" />
```

### Aplicaciones MAUI

```csharp
// Agregar referencia a VisioForge.DotNet.Core.UI.MAUI
using VisioForge.Core.UI.MAUI;

// En tu XAML
<vf:VideoView x:Name="videoView" />
```

### Avalonia UI

```csharp
// Agregar referencia a VisioForge.DotNet.Core.UI.Avalonia
using VisioForge.Core.UI.Avalonia;

// En tu XAML
<vf:VideoView Name="videoView" />
```

## Gestión de Dependencias Nativas

Nuestros SDKs aprovechan bibliotecas nativas para rendimiento óptimo. Estas dependencias deben gestionarse correctamente para el despliegue:

- Windows: Incluidas automáticamente con la instalación de configuración o paquetes NuGet
- macOS/iOS: Empaquetadas con paquetes NuGet pero requieren firma de app adecuada
- Android: Incluidas en paquetes NuGet con soporte de arquitectura apropiado
- Linux: Pueden requerir paquetes de sistema adicionales dependiendo de la distribución

Para instrucciones detalladas de despliegue, consulta nuestra [guía de despliegue](../deployment-x/index.md).

## Solución de Problemas Comunes de Instalación

Si encuentras problemas durante la instalación:

1. Verifica la compatibilidad del framework objetivo con tu tipo de proyecto
2. Asegúrate de que todas las cargas de trabajo requeridas estén instaladas (`dotnet workload list`)
3. Verifica conflictos de dependencias en tu proyecto
4. Confirma la inicialización adecuada del SDK para motores X
5. Revisa los requisitos específicos de plataforma en nuestra documentación

## Código de Ejemplo y Recursos

Mantenemos una extensa colección de aplicaciones de ejemplo en nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ayudarte a comenzar rápidamente con nuestros SDKs.

Estos ejemplos cubren escenarios comunes como:

- Captura de video de cámaras y pantallas
- Reproducción de medios con controles personalizados
- Edición y procesamiento de video
- Desarrollo multiplataforma

Visita nuestro repositorio para los últimos ejemplos de código y mejores prácticas para usar nuestros SDKs.

---

Para soporte adicional o preguntas, por favor contacta a nuestro equipo de soporte técnico o visita nuestro portal de documentación.
