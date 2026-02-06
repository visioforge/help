---
title: Guía de Despliegue Multiplataforma .NET para Ubuntu
description: Despliega aplicaciones multimedia .NET en Ubuntu Linux con configuración de GStreamer, configuración de hardware y optimización de rendimiento multiplataforma.
---

# Guía de Despliegue para Ubuntu de Aplicaciones del SDK de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Desplegar aplicaciones .NET con SDKs de VisioForge en Ubuntu Linux ofrece múltiples beneficios, incluyendo compatibilidad multiplataforma, acceso a hardware específico de Linux y la capacidad de ejecutar tus aplicaciones multimedia en entornos que van desde infraestructura de servidor hasta dispositivos edge. Esta guía completa te llevará a través del proceso completo de configurar tu entorno Ubuntu, instalar las dependencias necesarias y desplegar tu aplicación .NET con VisioForge.

La familia de SDKs de VisioForge funciona en Ubuntu y otras distribuciones Linux que soporten bibliotecas `GStreamer`. Las plataformas adicionales soportadas incluyen dispositivos `Nvidia Jetson` y `Raspberry Pi`, haciéndolo perfecto para una amplia gama de aplicaciones desde software multimedia de escritorio hasta soluciones IoT.

## Requisitos del Sistema

Antes de desplegar tu aplicación, asegúrate de que tu entorno Ubuntu cumpla estos requisitos mínimos:

- Ubuntu 20.04 LTS o posterior (22.04 LTS y posterior recomendado)
- Runtime .NET 7.0 o posterior
- Al menos 4GB RAM (8GB recomendado para procesamiento de video)
- Arquitectura x86_64 o ARM64
- Conexión a internet para instalación de paquetes

## Instalación y Configuración

### Instalando .NET

Descarga el último paquete de [instalador .NET](https://dotnet.microsoft.com/en-us/download/dotnet) del sitio web de Microsoft y sigue las instrucciones de instalación.

## Instalación de GStreamer

GStreamer forma la columna vertebral multimedia para los SDKs de VisioForge en plataformas Linux. Proporciona funcionalidad esencial para captura, procesamiento y reproducción de audio y video.

### Paquetes GStreamer Requeridos

Instala los siguientes paquetes GStreamer usando apt-get. Requerimos v1.22.0 o posterior, aunque v1.24.0+ es altamente recomendado para acceso a las últimas características y optimizaciones:

- `gstreamer1.0-plugins-base`: Plugins esenciales de línea base
- `gstreamer1.0-plugins-good`: Plugins de alta calidad, bien probados
- `gstreamer1.0-plugins-bad`: Plugins más nuevos de calidad variable
- `gstreamer1.0-alsa`: Soporte de audio ALSA
- `gstreamer1.0-gl`: Soporte de renderizado OpenGL
- `gstreamer1.0-pulseaudio`: Integración con PulseAudio
- `libges-1.0-0`: GStreamer Editing Services
- `gstreamer1.0-libav`: Integración FFMPEG (OPCIONAL pero recomendado para soporte de formatos más amplio)

### Script de Instalación Completo

Los siguientes comandos actualizarán tus repositorios de paquetes e instalarán todos los componentes GStreamer requeridos:

```bash
sudo apt update
```

```bash
sudo apt install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-alsa gstreamer1.0-gl gstreamer1.0-pulseaudio gstreamer1.0-libav libges-1.0-0
```

### Requisitos Adicionales para Raspberry Pi

Para Raspberry Pi, adicionalmente, necesitas instalar los siguientes paquetes:

```bash
sudo apt install gstreamer1.0-libcamera
```

### Verificando la Instalación de GStreamer

Después de la instalación, verifica tu configuración de GStreamer ejecutando:

```bash
gst-inspect-1.0 --version
```

Esto debería mostrar la versión instalada de GStreamer. Asegúrate de que cumpla el requisito mínimo (1.22.0+) o idealmente muestre 1.24.0 o posterior.

## Paquetes NuGet Requeridos

Al desplegar tu aplicación .NET en Ubuntu, necesitarás incluir paquetes NuGet adicionales específicos de plataforma que proporcionen las bibliotecas nativas y bindings necesarios.

### Paquete Principal Adicional de Linux

El paquete [VisioForge.CrossPlatform.Core.Linux.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Linux.x64) contiene bibliotecas nativas esenciales y bindings para la plataforma .NET Linux. Este paquete es obligatorio para todos los despliegues del SDK de VisioForge en Ubuntu.

### Entorno de Desarrollo

Puedes usar Rider para desarrollar tu proyecto en Linux. Por favor consulta la página de instalación de [Rider](../install/rider.md) para más información.

## Despliegue de la Aplicación

Sigue estos pasos para desplegar tu aplicación en Ubuntu:

### Publicando Tu Aplicación

Para crear un despliegue autocontenido que incluya todas las dependencias del runtime .NET:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

Para despliegues más pequeños donde la máquina objetivo ya tiene .NET instalado:

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
```

### Estructura de Despliegue

Tu carpeta de despliegue debería contener:

- Tu ejecutable de aplicación
- DLLs de la aplicación
- Ensamblados del SDK de VisioForge
- Bibliotecas nativas de Linux de los paquetes NuGet de VisioForge

### Estableciendo Permisos de Ejecución

Asegúrate de que tu ejecutable de aplicación tenga los permisos adecuados:

```bash
chmod +x ./NombreDeTuAplicacion
```

## Consideraciones de Hardware

### Soporte de Cámaras

Ubuntu soporta varios tipos de cámaras:

- **Webcams USB**: La mayoría de las webcams USB funcionan directamente
- **Cámaras IP**: Soportadas vía RTSP, streams HTTP
- **Cámaras Profesionales**: Muchas cámaras profesionales con drivers Linux son soportadas
- **Dispositivos Virtuales**: v4l2loopback puede usarse para creación de cámaras virtuales

Para listar dispositivos de cámara disponibles:

```bash
v4l2-ctl --list-devices
```

### Dispositivos de Audio

La captura y reproducción de audio es soportada a través de:

- ALSA (Advanced Linux Sound Architecture)
- PulseAudio

Para listar dispositivos de audio disponibles:

```bash
arecord -L  # Para dispositivos de grabación
aplay -L    # Para dispositivos de reproducción
```

## Solución de Problemas

### Problemas de Permisos

Los problemas de acceso a cámara o dispositivos de audio a menudo pueden resolverse agregando tu usuario a los grupos apropiados:

```bash
sudo usermod -a -G video,audio $USER
```

Recuerda cerrar sesión y volver a entrar para que los cambios de grupo tomen efecto.

### Optimización de Rendimiento

Para rendimiento óptimo en Ubuntu:

- Usa la última versión de GStreamer (1.24.0+)
- Habilita la aceleración de hardware donde esté disponible
- Para GPUs NVIDIA, instala los paquetes apropiados de CUDA y nvcodec
- Ajusta la prioridad del proceso usando `nice` para aplicaciones intensivas en recursos

## Conclusión

Desplegar aplicaciones del SDK de VisioForge en Ubuntu proporciona un entorno potente y flexible para aplicaciones multimedia. Siguiendo esta guía, puedes asegurar que tu aplicación .NET aproveche las capacidades completas del ecosistema del SDK de VisioForge en plataformas Linux.

Para escenarios de despliegue específicos o asistencia de solución de problemas, consulta la documentación completa disponible en el sitio web de VisioForge o contacta a nuestro equipo de soporte técnico.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.