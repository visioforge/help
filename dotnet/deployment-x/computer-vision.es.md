---
title: Despliegue de VisioForge CV y CVD para .NET multiplataforma
description: Implemente visión por computadora en aplicaciones en Windows, Linux y macOS con los paquetes NuGet VisioForge CV y CVD para desarrollo .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows

---

# Guía de implementación de visión por computadora

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }, [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }, [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Resumen de paquetes disponibles

Nuestro SDK proporciona dos paquetes NuGet con capacidades de visión por computadora para sus aplicaciones:

1. **[VisioForge.DotNet.Core.CV](https://www.nuget.org/packages/VisioForge.DotNet.Core.CV/)** — paquete CV para Windows x64, macOS en Apple Silicon y Linux x64.
2. **[VisioForge.DotNet.Core.CVD](https://www.nuget.org/packages/VisioForge.DotNet.Core.CVD/)** — paquete CV multiplataforma para Windows, Linux y macOS.

Ambos paquetes proporcionan una API consistente para integrar visión por computadora directamente en sus aplicaciones .NET.

## Requisitos de despliegue

### Paquete CV

#### Proceso de instalación

El paquete CV incluye la biblioteca nativa de OpenCV para cada plataforma compatible:

- Simplemente instale el paquete NuGet a través de su gestor de paquetes preferido
- No se necesitan pasos adicionales de despliegue en Windows y macOS
- En Linux, la biblioteca nativa necesita GTK 3. Las instalaciones de escritorio lo incluyen; en un servidor, una imagen mínima o un contenedor, instálelo con `sudo apt-get install libgtk-3-0` (Ubuntu/Debian) o `sudo dnf install gtk3` (RHEL/AlmaLinux)
- Una aplicación con un framework .NET sin sufijo de plataforma (por ejemplo `net8.0`) recibe las bibliotecas nativas de todos los sistemas compatibles; publíquela con `dotnet publish -r <rid>` (por ejemplo `-r linux-x64`) para conservar solo la del sistema de destino
- Windows de 32 bits, los Mac Intel, Mac Catalyst, iOS y Android no son compatibles

### Paquete CVD multiplataforma

Nuestro paquete CVD multiplataforma requiere configuraciones específicas según su sistema operativo:

#### Configuración del entorno Windows

Al desplegar en sistemas Windows:

- Instale el paquete NuGet a través de Visual Studio o la CLI de .NET
- No se requieren dependencias o configuraciones adicionales
- Funciona de manera inmediata con instalaciones estándar de Windows

#### Configuración de Ubuntu Linux

Para sistemas Ubuntu Linux, instale las siguientes dependencias:

```bash
sudo apt-get install libgdiplus libopenblas-dev libx11-6
```

Estos paquetes proporcionan funcionalidades esenciales:

- `libgdiplus`: Proporciona compatibilidad con System.Drawing
- `libopenblas-dev`: Optimiza las operaciones de matrices para algoritmos de visión por computadora
- `libx11-6`: Maneja la biblioteca cliente del protocolo X Window System

#### Instrucciones de configuración para macOS

Para entornos macOS, use Homebrew para instalar las dependencias requeridas:

```bash
brew install --cask xquartz
brew install mono-libgdiplus
```

Estos componentes habilitan:

- XQuartz: Proporciona funcionalidad X11 en macOS
- mono-libgdiplus: Asegura la compatibilidad con System.Drawing

## Recursos adicionales

Para ejemplos de implementación y orientación técnica:

- Visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener ejemplos de código extensos
- Explore implementaciones prácticas en varios casos de uso
- Acceda a ejemplos y soluciones contribuidos por la comunidad

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.