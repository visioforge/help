---
title: Integra SDKs .Net en JetBrains Rider | Tutorial
description: Integra SDKs .NET de VisioForge con JetBrains Rider para desarrollo multiplataforma con frameworks WPF, MAUI, WinUI y Avalonia.
---

# Integración de SDKs .Net con JetBrains Rider

## Introducción

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta guía completa te lleva a través del proceso de instalación y configuración de SDKs .Net de VisioForge dentro de JetBrains Rider, un potente IDE multiplataforma para desarrollo .NET. Aunque usaremos una aplicación Windows con WPF como nuestro ejemplo principal, estos pasos de instalación pueden adaptarse fácilmente para aplicaciones macOS, iOS o Android también. JetBrains Rider proporciona una experiencia de desarrollo consistente a través de plataformas Windows, macOS y Linux, lo que lo convierte en una excelente opción para desarrollo .NET multiplataforma.

## Creando Tu Proyecto

### Configurando una Estructura de Proyecto Moderna

Comienza lanzando JetBrains Rider y creando un nuevo proyecto. Para este tutorial, usaremos WPF (Windows Presentation Foundation) como nuestro framework. Es crucial utilizar el formato de proyecto moderno, que proporciona compatibilidad mejorada con los SDKs de VisioForge y ofrece una experiencia de desarrollo más fluida.

1. Abre JetBrains Rider
2. Selecciona "Create New Solution" desde la pantalla de bienvenida
3. Elige "WPF Application" de las plantillas disponibles
4. Configura los ajustes de tu proyecto, asegurándote de seleccionar el formato de proyecto moderno
5. Haz clic en "Create" para generar la estructura de tu proyecto

![Pantalla de creación de proyecto en Rider](/help/docs/dotnet/install/rider1.webp)

## Agregando Paquetes NuGet Requeridos

### Instalando el Paquete Principal del SDK

Cada SDK de VisioForge tiene un paquete principal correspondiente que proporciona la funcionalidad central. Necesitarás seleccionar el paquete apropiado basado en con qué SDK estés trabajando.

1. Haz clic derecho en tu proyecto en el Solution Explorer
2. Selecciona el elemento de menú "Manage NuGet Packages"
3. En el NuGet Package Manager, busca el paquete de VisioForge que corresponda a tu SDK deseado
4. Selecciona la última versión estable y haz clic en "Install"

![Agregando el paquete principal del SDK a través de NuGet](/help/docs/dotnet/install/rider2.webp)

### Paquetes Principales del SDK Disponibles

Elige entre los siguientes paquetes principales según tus necesidades de desarrollo:

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture) - Para aplicaciones que requieren funcionalidad de captura de video
- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit) - Para aplicaciones de edición y procesamiento de video
- [VisioForge.DotNet.MediaPlayer](https://www.nuget.org/packages/VisioForge.DotNet.MediaPlayer) - Para aplicaciones de reproducción de medios
- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks) - Para aplicaciones que requieren capacidades modulares de procesamiento de medios

### Agregando el Paquete de UI, si es necesario

El paquete principal del SDK contiene los componentes de UI principales para WinForms, WPF, Android y Apple.

Para otras plataformas, necesitarás instalar el paquete de UI apropiado que corresponda a tu framework de UI elegido.

### Paquetes de UI Disponibles

Dependiendo de tu plataforma objetivo y framework de UI, elige entre estos paquetes de UI:

- El paquete principal contiene los componentes de UI principales para WinForms, WPF y Apple
- [VisioForge.DotNet.Core.UI.WinUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.WinUI) - Para aplicaciones Windows usando el framework moderno WinUI
- [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI) - Para aplicaciones multiplataforma usando .NET MAUI
- [VisioForge.DotNet.Core.UI.Avalonia](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.Avalonia) - Para aplicaciones de escritorio multiplataforma usando Avalonia UI

## Integrando el Control VideoView (Opcional)

### Agregando Capacidades de Vista Previa de Video

Si tu aplicación requiere funcionalidad de vista previa de video, necesitarás agregar el control VideoView a tu interfaz de usuario. Esto puede lograrse ya sea a través de marcado XAML o programáticamente en tu archivo code-behind. A continuación, demostraremos cómo agregarlo vía XAML.

#### Paso 1: Agregar el Espacio de Nombres WPF

Primero, agrega la referencia de espacio de nombres necesaria a tu archivo XAML:

```xml
xmlns:wpf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
```

#### Paso 2: Agregar el Control VideoView

Luego, agrega el control VideoView a tu diseño:

```xml
<wpf:VideoView 
    Width="640" 
    Height="480" 
    Margin="10,10,0,0" 
    HorizontalAlignment="Left" 
    VerticalAlignment="Top"/>
```

Este control proporciona un lienzo donde el contenido de video puede mostrarse en tiempo real, esencial para aplicaciones que involucran captura, edición o reproducción de video.

## Agregando Paquetes de Redistribución Requeridos

### Dependencias Específicas de Plataforma

Dependiendo de tu plataforma objetivo, producto elegido y el motor específico que estés utilizando, pueden necesitarse paquetes de redistribución adicionales para asegurar la funcionalidad adecuada en todos los entornos de despliegue.

Para información completa sobre qué paquetes de redistribución se requieren para tu escenario específico, por favor consulta la página de documentación de Despliegue para tu producto VisioForge seleccionado. Estos recursos proporcionan guía detallada sobre:

- Dependencias del sistema requeridas
- Consideraciones específicas de plataforma
- Estrategias de optimización de despliegue
- Requisitos de tiempo de ejecución

Seguir estas guías de despliegue asegurará que tu aplicación funcione correctamente en los sistemas de los usuarios finales sin dependencias faltantes o errores de tiempo de ejecución.

## Recursos Adicionales

Para más ejemplos y guías de implementación detalladas, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples), que contiene numerosos ejemplos de código demostrando varias características y escenarios de integración.

Nuestro portal de documentación también ofrece referencias completas de API, tutoriales detallados y guías de mejores prácticas para ayudarte a aprovechar al máximo los SDKs de VisioForge en tus proyectos de JetBrains Rider.

## Conclusión

Siguiendo esta guía de instalación, has integrado exitosamente los SDKs .Net de VisioForge con JetBrains Rider, estableciendo las bases para desarrollar aplicaciones de medios potentes. La combinación de las robustas capacidades de procesamiento de medios de VisioForge y el entorno de desarrollo inteligente de JetBrains Rider proporciona una plataforma ideal para crear aplicaciones de medios sofisticadas en múltiples plataformas.
