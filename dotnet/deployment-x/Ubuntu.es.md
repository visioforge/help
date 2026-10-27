---
title: Guía de despliegue multiplataforma .NET para Ubuntu
description: Despliegue sus aplicaciones multimedia .NET en Ubuntu Linux: configuración de GStreamer y del hardware, y optimización del rendimiento.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Linux
  - GStreamer
  - NuGet

---

# Guía de despliegue para Ubuntu de aplicaciones del SDK de VisioForge

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Desplegar aplicaciones .NET con SDKs de VisioForge en Ubuntu Linux ofrece múltiples beneficios, incluyendo compatibilidad multiplataforma, acceso a hardware específico de Linux y la capacidad de ejecutar sus aplicaciones multimedia en entornos que van desde infraestructura de servidor hasta dispositivos edge. Esta guía completa le llevará a través del proceso completo de configurar su entorno Ubuntu, instalar las dependencias necesarias y desplegar su aplicación .NET con VisioForge.

La familia de SDKs de VisioForge funciona en Ubuntu y otras distribuciones Linux que soporten bibliotecas `GStreamer`. Las plataformas adicionales soportadas incluyen dispositivos `Nvidia Jetson` y `Raspberry Pi`, haciéndolo perfecto para una amplia gama de aplicaciones desde software multimedia de escritorio hasta soluciones IoT.

## Requisitos del sistema

Antes de desplegar su aplicación, asegúrese de que su entorno Ubuntu cumpla estos requisitos mínimos:

- Ubuntu 20.04 LTS o posterior (22.04 LTS y posterior recomendado)
- Runtime .NET 7.0 o posterior
- Al menos 4GB RAM (8GB recomendado para procesamiento de video)
- Arquitectura x86_64 o ARM64
- Conexión a internet para instalación de paquetes

## Instalación y configuración

### Instalar .NET

Descargue el último paquete de [instalador .NET](https://dotnet.microsoft.com/en-us/download/dotnet) del sitio web de Microsoft y siga las instrucciones de instalación.

## Instalación de GStreamer

GStreamer forma la columna vertebral multimedia para los SDKs de VisioForge en plataformas Linux. Proporciona funcionalidad esencial para captura, procesamiento y reproducción de audio y video.

### Paquetes GStreamer requeridos

Instale los siguientes paquetes GStreamer usando apt-get. Requerimos v1.22.0 o posterior, aunque v1.24.0+ es altamente recomendado para acceso a las últimas características y optimizaciones:

- `gstreamer1.0-plugins-base`: Plugins esenciales de línea base
- `gstreamer1.0-plugins-good`: Plugins de alta calidad, bien probados
- `gstreamer1.0-plugins-bad`: Plugins más nuevos de calidad variable
- `gstreamer1.0-alsa`: Soporte de audio ALSA
- `gstreamer1.0-gl`: Soporte de renderizado OpenGL
- `gstreamer1.0-pulseaudio`: Integración con PulseAudio
- `libges-1.0-0`: GStreamer Editing Services
- `gstreamer1.0-libav`: Integración FFMPEG (OPCIONAL pero recomendado para soporte de formatos más amplio)

### Script de instalación completo

Los siguientes comandos actualizarán sus repositorios de paquetes e instalarán todos los componentes GStreamer requeridos:

```bash
sudo apt update
```

```bash
sudo apt install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-alsa gstreamer1.0-gl gstreamer1.0-pulseaudio gstreamer1.0-libav libges-1.0-0
```

### Requisitos adicionales para Raspberry Pi

Para Raspberry Pi, adicionalmente, necesita instalar los siguientes paquetes:

```bash
sudo apt install gstreamer1.0-libcamera
```

### Verificar la instalación de GStreamer

Después de la instalación, verifique su configuración de GStreamer ejecutando:

```bash
gst-inspect-1.0 --version
```

Esto debería mostrar la versión instalada de GStreamer. Asegúrese de que cumpla el requisito mínimo (1.22.0+) o idealmente muestre 1.24.0 o posterior.

## Paquetes NuGet requeridos

Al desplegar su aplicación .NET en Ubuntu, necesitará incluir paquetes NuGet adicionales específicos de plataforma que proporcionen las bibliotecas nativas y bindings necesarios.

### Paquete principal adicional de Linux

El paquete [VisioForge.CrossPlatform.Core.Linux.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Linux.x64) contiene bibliotecas nativas esenciales y bindings para la plataforma .NET Linux. Este paquete es obligatorio para todos los despliegues del SDK de VisioForge en Ubuntu.

### Entorno de desarrollo

Puede usar Rider para desarrollar su proyecto en Linux. Por favor consulte la página de instalación de [Rider](../install/rider.md) para más información.

## Despliegue de la aplicación

Siga estos pasos para desplegar su aplicación en Ubuntu:

### Publicar su aplicación

Para crear un despliegue autocontenido que incluya todas las dependencias del runtime .NET:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

Para despliegues más pequeños donde la máquina objetivo ya tiene .NET instalado:

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
```

### Estructura de despliegue

Su carpeta de despliegue debería contener:

- Su ejecutable de aplicación
- DLLs de la aplicación
- Ensamblados del SDK de VisioForge
- Bibliotecas nativas de Linux de los paquetes NuGet de VisioForge

### Establecer permisos de ejecución

Asegúrese de que su ejecutable de aplicación tenga los permisos adecuados:

```bash
chmod +x ./NombreDeTuAplicacion
```

## Consideraciones de hardware

### Soporte de cámaras

Ubuntu soporta varios tipos de cámaras:

- **Webcams USB**: La mayoría de las webcams USB funcionan directamente
- **Cámaras IP**: Soportadas vía RTSP, streams HTTP
- **Cámaras profesionales**: Muchas cámaras profesionales con drivers Linux son soportadas
- **Dispositivos virtuales**: v4l2loopback puede usarse para creación de cámaras virtuales

Para listar dispositivos de cámara disponibles:

```bash
v4l2-ctl --list-devices
```

### Dispositivos de audio

La captura y reproducción de audio es soportada a través de:

- ALSA (Advanced Linux Sound Architecture)
- PulseAudio

Para listar dispositivos de audio disponibles:

```bash
arecord -L  # Para dispositivos de grabación
aplay -L    # Para dispositivos de reproducción
```

## Ejecución bajo WSL (WSLg)

WSLg es conveniente para el desarrollo, y todo excepto la vista previa de video funciona en él igual que en una instalación nativa. La vista previa de video es la única excepción: cuando el control `VideoView` crea su ventana hija nativa (HWND en Windows, X11/XEmbed en Linux), la pila de ventanas de WSLg no consigue inicializarla y la aplicación se cierra en cuanto llega el primer fotograma — normalmente con errores de Mesa como `ZINK: failed to choose pdev` o `failed to create dri2 screen` en la consola. Es una limitación de WSLg, no un defecto de la aplicación ni del SDK.

Para desarrollar dentro de WSL, cambie el `VideoView` a renderizado por software antes de iniciar el pipeline — el video se dibuja dentro del árbol visual de Avalonia, no se crea ninguna ventana hija nativa y las vistas previas funcionan bajo WSLg:

```cs
videoView.SetNativeRendering(false);
```

El modo software cuesta una copia de CPU por fotograma y está pensado para desarrollo y escenarios de superposición; en una instalación nativa de Ubuntu, la incrustación nativa por defecto sigue siendo la vía recomendada. Véase [Superponer controles Avalonia sobre el video](../general/code-samples/avalonia-controls-over-video.md) para más detalles.

## Solución de problemas

### Problemas de permisos

Los problemas de acceso a cámara o dispositivos de audio a menudo pueden resolverse agregando su usuario a los grupos apropiados:

```bash
sudo usermod -a -G video,audio $USER
```

Recuerde cerrar sesión y volver a entrar para que los cambios de grupo tomen efecto.

### Optimización de rendimiento

Para rendimiento óptimo en Ubuntu:

- Use la última versión de GStreamer (1.24.0+)
- Habilite la aceleración de hardware donde esté disponible
- Para GPUs NVIDIA, instale los paquetes apropiados de CUDA y nvcodec
- Ajuste la prioridad del proceso usando `nice` para aplicaciones intensivas en recursos

## Conclusión

Desplegar aplicaciones del SDK de VisioForge en Ubuntu proporciona un entorno potente y flexible para aplicaciones multimedia. Siguiendo esta guía, puede asegurar que su aplicación .NET aproveche las capacidades completas del ecosistema del SDK de VisioForge en plataformas Linux.

Para escenarios de despliegue específicos o asistencia de solución de problemas, consulte la documentación completa disponible en el sitio web de VisioForge o contacte a nuestro equipo de soporte técnico.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.