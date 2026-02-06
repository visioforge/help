---
title: Requisitos del Sistema: Video Fingerprinting SDK
description: Requisitos de hardware y software para ejecutar el SDK de Video Fingerprinting de VisioForge en plataformas Windows, Linux y macOS
---

# Requisitos del Sistema

Esta página describe los requisitos del sistema para ejecutar el SDK de Video Fingerprinting en plataformas soportadas. Estos requisitos aplican tanto para las versiones .NET como C++ del SDK.

## Plataformas Soportadas

### Windows

- **Sistema Operativo**: Windows 10 versión 1903+ o Windows 11
- **Arquitectura**: x86, x64 o ARM64 (ARM64 solo para SDK .NET)
- **Runtime**: Microsoft Visual C++ Redistributables 2019 o posterior
- **Bibliotecas Adicionales**: Filtros DirectShow para soporte de códec mejorado (opcional)

### Linux

- **Distribuciones**: Ubuntu 20.04+, Debian 11+, RHEL 8+, CentOS 8+, Fedora 34+ (Fedora para SDK C++)
- **Arquitectura**: x64 o ARM64
- **Dependencias**: GStreamer 1.18+ con plugins base y buenos
- **Servidor de Display**: X11 o Wayland (para SDK .NET con GUI)

### macOS

- **Sistema Operativo**: macOS 12 (Monterey) o posterior
- **Arquitectura**: Intel x64 o Apple Silicon (M1/M2/M3)
- **Dependencias**: No se requieren dependencias de runtime adicionales

## Requisitos de Hardware

### Requisitos Mínimos

- **Procesador**: CPU de doble núcleo (Intel Core i3 o equivalente AMD)
- **Memoria**: 
  - SDK .NET: 4 GB RAM
  - SDK C++: 2 GB RAM disponible para la aplicación
- **Almacenamiento**: 
  - SDK .NET: 500 MB de espacio libre en disco para el SDK
  - SDK C++: 100 MB para archivos del SDK + espacio para procesamiento de video
- **GPU**: Cualquier tarjeta gráfica compatible con DirectX 9 (solo Windows)

### Requisitos Recomendados

- **Procesador**: CPU de cuatro núcleos (Intel Core i5 o AMD Ryzen 5)
- **Memoria**: 
  - SDK .NET: 8 GB RAM o más
  - SDK C++: 4 GB RAM o más disponible para la aplicación
- **Almacenamiento**: 
  - SDK .NET: 2 GB de espacio libre en disco para SDK y archivos temporales
  - SDK C++: 500 MB para SDK + espacio de procesamiento temporal
- **GPU**: Tarjeta gráfica dedicada con soporte de aceleración por hardware

### Consideraciones de Rendimiento

- **Velocidad de Procesamiento**: Escala linealmente con duración de video y núcleos de CPU
- **Uso de Memoria**: Aumenta con la resolución del video
- **Almacenamiento**: El almacenamiento SSD mejora significativamente la velocidad de procesamiento (operaciones de E/S 2-3x más rápidas)
- **Paralelización**: Múltiples núcleos de CPU habilitan procesamiento paralelo
- **Aceleración por Hardware**: La decodificación de video por hardware puede proporcionar aceleración de 3-5x (SDK C++)

## Requisitos del Entorno de Desarrollo

### SDK .NET

- **Versión de .NET**: 
  - Windows: .NET Framework 4.6.1+ o .NET 6.0+
  - Linux/macOS: .NET 6.0+
- **IDE**: Visual Studio 2019+ (Windows), Visual Studio Code, o JetBrains Rider

### SDK C++

- **Compilador Windows**: Visual Studio 2019+ (recomendado) o MinGW-w64
- **Compilador Linux**: GCC 7+ o Clang 6+
- **Compilador macOS**: Xcode 12+ con Command Line Tools
- **Herramientas de Construcción**: CMake 3.10+ (opcional pero recomendado para Linux/macOS)

## Notas Adicionales Específicas de Plataforma

### Windows
- Los filtros DirectShow pueden mejorar el soporte de códec para formatos legacy
- Windows Media Foundation se usa para aceleración por hardware cuando está disponible

### Linux
- Asegúrese de que los plugins de GStreamer estén correctamente instalados: `sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good`
- Para aplicaciones GUI, se requiere servidor de display X11 o Wayland

### macOS
- Los procesadores Apple Silicon (M1/M2/M3) son completamente soportados con rendimiento nativo
- La traducción Rosetta 2 es soportada para binarios Intel en Apple Silicon

## Requisitos de Red

Para almacenamiento y comparación de fingerprints basados en la nube:
- **Ancho de Banda**: Mínimo 1 Mbps para carga/descarga de fingerprints
- **Latencia**: < 100ms para escenarios de procesamiento en tiempo real
- **Protocolos**: HTTPS para comunicación API, protocolo wire de MongoDB para operaciones de base de datos

## Soporte de Virtualización y Contenedores

- **Docker**: Completamente soportado en hosts Linux
- **Máquinas Virtuales**: Soportado con sobrecarga de rendimiento (20-30% más lento)
- **WSL2**: Soportado para SDK Linux en Windows
- **Plataformas en la Nube**: Compatible con AWS EC2, Azure VMs, Google Cloud Compute

## Próximos Pasos

- [Primeros Pasos con SDK .NET](dotnet/getting-started.md) - Configure el SDK .NET
- [Primeros Pasos con SDK C++](cpp/getting-started.md) - Configure el SDK C++
- [Entendiendo Video Fingerprinting](understanding-video-fingerprinting.md) - Aprenda cómo funciona la tecnología
