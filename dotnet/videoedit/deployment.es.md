---
title: Guía de Despliegue de Video Edit SDK .Net | Windows
description: Despliegue Video Edit SDK .Net en Windows con paquetes NuGet, instaladores silenciosos y configuraciones manuales para arquitecturas x86 y x64.
tags:
  - Video Edit SDK
  - .NET
  - DirectShow
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Decoding
  - Editing
  - RTSP
  - MP4
  - WebM
  - OGG
  - FLAC
  - Vorbis
  - NuGet

---

# Guía Completa de Despliegue para Video Edit SDK .Net

## Introducción al Despliegue de VideoEditCore

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

El Video Edit SDK .NET de VisioForge proporciona un conjunto poderoso de herramientas para procesamiento de video, edición y análisis en entornos Windows. Esta guía completa detalla las opciones de despliegue para asegurar que el SDK funcione correctamente en los sistemas de destino.

Para aplicaciones construidas con la configuración AnyCPU, debe desplegar tanto los redistribuibles x86 como x64 para asegurar compatibilidad en diferentes arquitecturas de procesador. Esta guía cubre todos los métodos de despliegue, desde simples paquetes NuGet hasta instalaciones manuales detalladas.

## Descripción General de Opciones de Despliegue

El SDK ofrece tres enfoques principales de despliegue:

1. **Paquetes NuGet**: Método más simple que no requiere privilegios administrativos
2. **Instaladores Automáticos**: Instalación silenciosa con derechos administrativos
3. **Instalación Manual**: Despliegue personalizado con control granular sobre los componentes

Cada enfoque tiene ventajas distintas dependiendo de los requisitos de su aplicación, método de distribución y restricciones del entorno de destino.

## Despliegue Multiplataforma con VideoEditCoreX

Para desarrolladores que buscan compatibilidad multiplataforma, VisioForge ofrece el motor VideoEditCoreX. Esta implementación moderna soporta entornos Windows, macOS y Linux.

Para instrucciones detalladas sobre el despliegue de la versión multiplataforma, por favor consulte nuestra [guía de despliegue multiplataforma](../deployment-x/index.md) dedicada. El resto de este documento se enfoca en el motor VideoEditCore específico de Windows.

## Motor VideoEditCore (Solo Windows)

El motor VideoEditCore específico de Windows proporciona capacidades extensas de edición de video optimizadas para entornos Windows. A continuación se presentan las opciones de despliegue completas disponibles.

### Despliegue con Paquetes NuGet (Sin Derechos Administrativos Requeridos)

Los paquetes NuGet ofrecen el método de despliegue más simple, sin requerir privilegios administrativos en el sistema de destino. Este enfoque copia automáticamente los archivos necesarios a la carpeta de su aplicación durante el proceso de compilación.

#### Paquetes NuGet Requeridos

**Componentes Base (Siempre Requeridos)**:

- Paquete Base del SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/) | [ARM64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.arm64/) (.NET 5 o posterior)
- Paquete Video Edit SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/) | ARM64: aún no disponible

**Componentes Específicos de Formato**:

- Salida MP4: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- Salida WebM: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)
- Formatos XIPH (Ogg, Vorbis, FLAC): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

**Componentes de Fuente Multimedia**:

- FFMPEG (Salida de archivo/streaming en red): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
- Filtros LAV: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

> **Windows ARM64:** los filtros LAV existen solo para x86 y x64. No hay una versión ARM64 oficial, por lo que el SDK no incluye un paquete LAV para ARM64, y la mayoría de los demás filtros DirectShow de los motores clásicos tampoco tienen aún una versión ARM64. Una aplicación que se ejecuta como proceso ARM64 no puede cargarlos. Las aplicaciones AnyCPU también suelen ejecutarse allí como ARM64. En Windows ARM64, use `VideoEditCoreX` o Media Blocks SDK .NET, o compile la aplicación para x64 (`PlatformTarget` con el valor `x64`; se ejecuta en emulación).

La implementación es directa: añada los paquetes requeridos a su proyecto de aplicación, y después de compilar, los archivos redistribuibles necesarios se incluirán automáticamente en la carpeta de su aplicación.

### Instaladores Silenciosos Automáticos (Derechos Administrativos Requeridos)

Para escenarios donde los derechos administrativos están disponibles, los instaladores silenciosos proporcionan una solución de despliegue simplificada. Estos instaladores pueden integrarse en el proceso de configuración de su aplicación para un despliegue fluido del SDK.

**Componentes Base**:

- Paquete Base (Siempre Requerido): [x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

**Componentes de Fuente Multimedia**:

- Paquete FFMPEG: [x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- Filtros LAV: [x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

**Componentes Específicos de Formato**:

- Formatos XIPH (Ogg, Vorbis, FLAC): [x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

**Instalación y Desinstalación**:

- Para instalar: Ejecute el ejecutable apropiado con privilegios administrativos
- Para desinstalar: Ejecute el ejecutable con privilegios administrativos y los parámetros "/x //"
- Los ensamblados .NET pueden instalarse en el Caché de Ensamblados Global (GAC) o usarse directamente desde una carpeta local

### Instalación Manual (Avanzada)

La instalación manual proporciona el mayor nivel de control sobre el proceso de despliegue. Este enfoque se recomienda para escenarios avanzados donde componentes específicos deben personalizarse o para entornos de despliegue con restricciones únicas.

#### Proceso de Instalación Manual Paso a Paso

1. **Dependencias de Runtime**:
   - Para aplicaciones con privilegios administrativos: Instale VC++ 2022 (v143) runtime (x86/x64) y DLLs de runtime OpenMP usando redistribuibles ejecutables o módulos MSM
   - Para aplicaciones sin privilegios administrativos: Copie las DLLs de VC++ 2022 (v143) runtime (x86/x64) y runtime OpenMP directamente a la carpeta de la aplicación

2. **Componentes Principales**:
   - Copie las DLLs VisioForge_MFP/VisioForge_MFPX (o versiones x64) desde Redist\Filters a la carpeta de su aplicación
   - Copie los ensamblados .NET a la carpeta de la aplicación o instálelos en el Caché de Ensamblados Global (GAC)

3. **Filtros DirectShow**:
   - Copie y registre en COM los filtros DirectShow del SDK usando [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5) o un método equivalente
   - Si el ejecutable de su aplicación está en una carpeta diferente, añada la carpeta que contiene los filtros a la variable de entorno PATH del sistema

## Referencia de Filtros DirectShow Esenciales

### Filtros de Funcionalidad Principal

**Procesamiento de Video Básico**:

- VisioForge_Video_Effects_Pro.ax - Procesamiento de efectos de video principales
- VisioForge_Audio_Mixer.ax - Mezcla y procesamiento de audio
- VisioForge_MP3_Splitter.ax - Manejo de formato MP3
- VisioForge_H264_Decoder.ax - Decodificación de video H.264

**Procesamiento de Audio**:

- VisioForge_Audio_Effects_4.ax - Procesamiento de efectos de audio legacy

### Filtros de Streaming

**Streaming RTSP**:

- VisioForge_RTSP_Sink.ax - Salida de streaming RTSP
- Todos los filtros MP4 (legacy/modernos) excepto Muxer

**Streaming SSF**:

- VisioForge_SSF_Muxer.ax - Multiplexor de formato SSF
- Todos los filtros MP4 (legacy/modernos) excepto Muxer

### Filtros de Fuente Multimedia

**Fuente FFMPEG**:

- VisioForge_FFMPEG_Source.ax - Entrada multimedia basada en FFMPEG
- Copie todos los archivos de la carpeta Redist\FFMPEG y añádala al PATH de Windows

**Fuente de Memoria**:

- VisioForge_AsyncEx.ax - Entrada de fuente basada en memoria

**Fuente LAV**:

- Copie todos los archivos de Redist\LAV\x86(x64)
- Registre todos los archivos .ax

### Filtros Específicos de Formato

**Decodificación WebM**:

- VisioForge_WebM_Ogg_Source.ax - Soporte de contenedor WebM/Ogg
- VisioForge_WebM_Source.ax - Fuente de formato WebM
- VisioForge_WebM_Split.ax - Demuxing de WebM
- VisioForge_WebM_Vorbis_Decoder.ax - Decodificador de audio Vorbis
- VisioForge_WebM_VP8_Decoder.ax - Decodificador de video VP8
- VisioForge_WebM_VP9_Decoder.ax - Decodificador de video VP9

**Fuente FLAC**:

- VisioForge_Xiph_FLAC_Source.ax - Soporte de formato de audio FLAC

**Fuente Ogg Vorbis**:

- VisioForge_Xiph_Ogg_Demux2.ax - Demuxer de contenedor Ogg
- VisioForge_Xiph_Vorbis_Decoder.ax - Decodificador de audio Vorbis

### Filtros de Funcionalidad Avanzada

**Encriptación de Video**:

- VisioForge_Encryptor_v8.ax - Encriptación versión 8
- VisioForge_Encryptor_v9.ax - Encriptación versión 9

**Aceleración GPU**:

- VisioForge_DXP.dll / VisioForge_DXP64.dll - Efectos GPU DirectX 11

### Registro Simplificado de Filtros

Para el registro conveniente de múltiples filtros DirectShow, coloque la utilidad `reg_special.exe` del redistribuible del SDK en la carpeta que contiene los filtros y ejecútela con privilegios de administrador.

## Recursos Adicionales

Para ejemplos de código y ejemplos de implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Para soporte técnico, actualizaciones de documentación y discusiones de la comunidad, visite el [Portal de Desarrolladores de VisioForge](https://support.visioforge.com/).
