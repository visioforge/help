---
title: Guía de Despliegue de la Biblioteca TVFVideoEdit
description: Despliegue TVFVideoEdit en aplicaciones Delphi y ActiveX con instaladores automáticos o configuración manual para componentes y dependencias requeridos.
---

# Guía de Despliegue de la Biblioteca TVFVideoEdit

## Introducción

La biblioteca TVFVideoEdit proporciona potentes capacidades de edición de video para sus aplicaciones Delphi y ActiveX. Esta guía explica cómo desplegar correctamente todos los componentes necesarios para asegurar que su aplicación funcione correctamente en sistemas de usuarios finales sin requerir el marco de desarrollo completo.

## Opciones de Despliegue

Tiene dos métodos principales para desplegar los componentes de la biblioteca TVFVideoEdit: instaladores automáticos o instalación manual. Cada enfoque tiene ventajas específicas dependiendo de sus requisitos de distribución.

### Instaladores Silenciosos Automáticos

Para un despliegue optimizado, ofrecemos paquetes de instaladores silenciosos que manejan toda la instalación de componentes necesarios sin interacción del usuario:

#### Paquete Base Requerido

* **Componentes base** (siempre requeridos):
  * [Versión Delphi](https://files.visioforge.com/redists_delphi/redist_video_edit_base_delphi.exe)
  * [Versión ActiveX](https://files.visioforge.com/redists_delphi/redist_video_edit_base_ax.exe)

#### Paquetes de Características Opcionales

* **Paquete FFMPEG** (requerido para soporte de archivos y cámaras IP (solo para motor de fuente FFMPEG)):
  * [Arquitectura x86](https://files.visioforge.com/redists_delphi/redist_video_edit_ffmpeg.exe)

* **Paquete de salida MP4** (para creación de video MP4):
  * [Arquitectura x86](https://files.visioforge.com/redists_delphi/redist_video_edit_mp4.exe)

### Proceso de Instalación Manual

Para situaciones donde necesita control preciso sobre el despliegue de componentes, siga estos pasos detallados:

1. **Instalar Dependencias de Visual C++**
   * Instale el redistributable VC++ 2010 SP1:
     * [Versión x86](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
     * [Versión x64](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

2. **Desplegar Componentes Core de Media Foundation**
   * Copie todos los DLLs MFP del directorio `Redist\Filters` a la carpeta de su aplicación

3. **Registrar Filtros DirectShow**
   * Copie y registre COM estos filtros DirectShow esenciales usando [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5):
     * `VisioForge_Audio_Effects_4.ax`
     * `VisioForge_Dump.ax`
     * `VisioForge_RGB2YUV.ax`
     * `VisioForge_Screen_Capture.ax`
     * `VisioForge_Video_Effects_Pro.ax`
     * `VisioForge_Video_Mixer.ax`
     * `VisioForge_Video_Resize.ax`
     * `VisioForge_WavDest.ax`
     * `VisioForge_YUV2RGB.ax`
     * `VisioForge_FFMPEG_Source.ax`

4. **Configurar Ajustes de Ruta**
   * Agregue la carpeta que contiene estos filtros a la variable de entorno del sistema `PATH` si el ejecutable de su aplicación reside en un directorio diferente

## Instalación de Componentes Adicionales

### Integración FFMPEG

Para habilitar soporte avanzado de formatos de medios:

* Copie todos los archivos de la carpeta `Redist\FFMPEG`
* Agregue esta carpeta a la variable `PATH` del sistema Windows
* Registre todos los archivos .ax de la carpeta `Redist\FFMPEG`

### Soporte VLC

Para compatibilidad de formatos extendida:

* Copie todos los archivos de la carpeta `Redist\VLC`
* Registre COM el archivo .ax usando regsvr32.exe
* Cree una variable de entorno llamada `VLC_PLUGIN_PATH`
* Establezca su valor para apuntar a la carpeta `VLC\plugins`

### Soporte de Salida de Audio

Para capacidades de codificación MP3:

* Copie el archivo lame.ax de la carpeta `Redist\Formats`
* Registre el archivo lame.ax usando regsvr32.exe

### Soporte de Formato WebM

Para codificación y decodificación WebM:

* Instale los códecs gratuitos necesarios disponibles en el [sitio web xiph.org](https://www.xiph.org/dshow/)

### Soporte de Contenedor Matroska

Para compatibilidad con formato MKV:

* Instale [Haali Matroska Splitter](https://haali.net/mkv/) para codificación y decodificación apropiada

### Salida MP4 H264/AAC - Codificador Moderno

Para creación de MP4 de alta calidad con códecs modernos:

* Copie los archivos `libmfxsw32.dll` / `libmfxsw64.dll`
* Registre estos filtros DirectShow:
  * `VisioForge_H264_Encoder.ax`
  * `VisioForge_MP4_Muxer.ax`
  * `VisioForge_AAC_Encoder.ax`
  * `VisioForge_Video_Resize.ax`

### Salida MP4 H264/AAC - Codificador Legacy

Para compatibilidad con sistemas más antiguos:

* Copie los archivos `libmfxxp32.dll` / `libmfxxp64.dll`
* Registre estos filtros DirectShow:
  * `VisioForge_H264_Encoder_XP.ax`
  * `VisioForge_MP4_Muxer_XP.ax`
  * `VisioForge_AAC_Encoder_XP.ax`
  * `VisioForge_Video_Resize.ax`

## Utilidad de Registro Masivo

Para simplificar el proceso de registro para múltiples filtros DirectShow:

* Coloque la utilidad `reg_special.exe` del paquete redistributable en la carpeta que contiene sus filtros
* Ejecútela con privilegios de administrador para registrar todos los filtros compatibles en ese directorio

## Consejos de Solución de Problemas

Los problemas comunes durante el despliegue a menudo incluyen:

* Dependencias faltantes
* Registro incorrecto de componentes COM
* Problemas de configuración de ruta
* Permisos de usuario insuficientes

Asegúrese de que todos los archivos requeridos estén correctamente desplegados y registrados antes de iniciar su aplicación.

---
Por favor contacte a [nuestro equipo de soporte](https://support.visioforge.com/) si encuentra algún problema con este proceso de despliegue. Visite nuestro [repositorio de GitHub](https://github.com/visioforge/) para ejemplos de código adicionales y ejemplos de implementación.