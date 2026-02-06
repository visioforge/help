---
title: Implementación de Visión por Computadora en .NET
description: Implemente visión por computadora en aplicaciones en Windows, Linux y macOS con los paquetes NuGet VisioForge CV y CVD para desarrollo .NET.
---

# Guía de Implementación de Visión por Computadora

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }, [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }, [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Resumen de Paquetes Disponibles

Nuestro SDK proporciona dos potentes paquetes NuGet que ofrecen capacidades robustas de visión por computadora para sus aplicaciones:

1. **Paquete VisioForge CV**: Diseñado específicamente para entornos Windows
2. **Paquete VisioForge CVD**: Solución multiplataforma que funciona en múltiples sistemas operativos

Estos paquetes proporcionan una API completa para integrar características de visión por computadora directamente en sus aplicaciones .NET.

## Requisitos de Despliegue

### Paquete CV Específico para Windows

#### Proceso de Instalación

El paquete CV específico para Windows está diseñado para una integración sin problemas:

- Simplemente instale el paquete NuGet a través de su gestor de paquetes preferido
- No se necesitan pasos adicionales de despliegue
- Listo para usar inmediatamente después de la instalación

### Paquete CVD Multiplataforma

Nuestro paquete CVD multiplataforma requiere configuraciones específicas según su sistema operativo:

#### Configuración del Entorno Windows

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

#### Instrucciones de Configuración para macOS

Para entornos macOS, use Homebrew para instalar las dependencias requeridas:

```bash
brew cask install xquartz
brew install mono-libgdiplus
```

Estos componentes habilitan:

- XQuartz: Proporciona funcionalidad X11 en macOS
- mono-libgdiplus: Asegura la compatibilidad con System.Drawing

## Recursos Adicionales

Para ejemplos de implementación y orientación técnica:

- Visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener ejemplos de código extensos
- Explore implementaciones prácticas en varios casos de uso
- Acceda a ejemplos y soluciones contribuidos por la comunidad

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.