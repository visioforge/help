---
title: Despliegue de la biblioteca TVFVideoCapture en Delphi
description: "Despliegue TVFVideoCapture en Delphi: instale componentes, registre filtros DirectShow y configure el entorno para desplegar sus aplicaciones con éxito."
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Capture
  - Encoding
  - IP Camera
  - MP4
  - MP3
primary_api_classes:
  - TVFVideoCapture

---

# Guía completa de despliegue de la biblioteca TVFVideoCapture

Al distribuir aplicaciones construidas con la biblioteca TVFVideoCapture, necesitará desplegar varios componentes del framework para asegurar la funcionalidad adecuada en sistemas de usuarios finales. Esta guía cubre todos los escenarios de despliegue para ayudarle a crear instalaciones confiables.

## Resumen de opciones de despliegue

Tiene dos enfoques principales para desplegar los componentes necesarios: instaladores automáticos para despliegue más simple o instalación manual para configuraciones más personalizadas.

## Instaladores silenciosos automáticos (se requieren derechos de administrador)

Estos instaladores preconfigurados manejan dependencias automáticamente y pueden integrarse en el proceso de instalación de su aplicación:

### Componentes esenciales

- **Paquete base** (obligatorio para todos los despliegues)
  - [Versión Delphi](https://files.visioforge.com/redists_delphi/redist_video_capture_base_delphi.exe)
  - [Versión ActiveX](https://files.visioforge.com/redists_delphi/redist_video_capture_base_ax.exe)

### Componentes de características opcionales

- **Paquete FFMPEG** (requerido para fuentes de archivo o cámara IP)
  - [Arquitectura x86](https://files.visioforge.com/redists_delphi/redist_video_capture_ffmpeg.exe)

- **Soporte de salida MP4**
  - [Arquitectura x86](https://files.visioforge.com/redists_delphi/redist_video_capture_mp4.exe)

- **Paquete VLC Source** (opción alternativa para fuentes de archivo o cámara IP)
  - [Arquitectura x86](https://files.visioforge.com/redists_delphi/redist_video_capture_vlc.exe)

## Proceso de instalación manual (se requieren derechos de administrador)

Para más control sobre el proceso de despliegue, siga estos pasos detallados:

### Paso 1: Instalar dependencias requeridas

1. Desplegar redistribuibles de Visual C++ 2010 SP1:
   - [Arquitectura x86](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
   - [Arquitectura x64](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

### Paso 2: Desplegar componentes centrales

1. Copie todos los DLLs de Media Foundation Platform (MFP) del directorio `Redist\Filters` a la carpeta de su aplicación
2. Para implementaciones ActiveX: copie y registre el archivo OCX usando [regsvr32.exe](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/regsvr32)

### Paso 3: Registrar filtros DirectShow

Usando [regsvr32.exe](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/regsvr32), registre estos filtros DirectShow esenciales:

- `VisioForge_Audio_Effects_4.ax`
- `VisioForge_Dump.ax`
- `VisioForge_RGB2YUV.ax`
- `VisioForge_Screen_Capture.ax`
- `VisioForge_Video_Effects_Pro.ax`
- `VisioForge_Video_Mixer.ax`
- `VisioForge_Video_Resize.ax`
- `VisioForge_WavDest.ax`
- `VisioForge_YUV2RGB.ax`
- `VisioForge_FFMPEG_Source.ax`

> **Importante:** Agregue el directorio de filtros a la variable de entorno PATH del sistema si el ejecutable de su aplicación reside en una carpeta diferente.

## Instalación de componentes avanzados

### Integración FFMPEG

1. Copie todos los archivos de la carpeta `Redist\FFMPEG` a su despliegue
2. Agregue la carpeta FFMPEG a la variable PATH del sistema Windows
3. Registre todos los archivos .ax de la carpeta FFMPEG

### Integración VLC

1. Copie todos los archivos de la carpeta `Redist\VLC`
2. Registre el archivo .ax incluido usando regsvr32.exe
3. Cree una variable de entorno llamada `VLC_PLUGIN_PATH` apuntando al directorio `VLC\plugins`

### Soporte de salida de audio (LAME)

1. Copie `lame.ax` de la carpeta `Redist\Formats`
2. Registre el archivo `lame.ax` usando regsvr32.exe

### Soporte de formatos de contenedor

- **Soporte WebM:** Instale códecs gratuitos de [xiph.org](https://www.xiph.org)
- **Soporte Matroska:** Despliegue `Haali Matroska Splitter`

### Configuración de salida MP4

#### Configuración de codificador moderno

1. Copie los archivos de biblioteca apropiados:
   - `libmfxsw32.dll` (para despliegues de 32 bits)
   - `libmfxsw64.dll` (para despliegues de 64 bits)
2. Registre los componentes requeridos:
   - `VisioForge_H264_Encoder.ax`
   - `VisioForge_MP4_Muxer.ax`
   - `VisioForge_AAC_Encoder.ax`
   - `VisioForge_Video_Resize.ax`

#### Configuración de codificador heredado (para sistemas antiguos)

1. Copie los archivos de biblioteca apropiados:
   - `libmfxxp32.dll` (para despliegues de 32 bits)
   - `libmfxxp64.dll` (para despliegues de 64 bits)
2. Registre los componentes requeridos:
   - `VisioForge_H264_Encoder_XP.ax`
   - `VisioForge_MP4_Muxer_XP.ax`
   - `VisioForge_AAC_Encoder_XP.ax`
   - `VisioForge_Video_Resize.ax`

## Utilidad de registro masivo

Para simplificar el registro de filtros DirectShow, puede usar la utilidad `reg_special.exe` de la configuración del framework. Coloque este ejecutable en su directorio de filtros y ejecútelo con privilegios de administrador para registrar todos los filtros de una vez.

---
Para ejemplos de código adicionales y ejemplos de implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/). Si encuentra alguna dificultad con el despliegue, por favor contacte al [soporte técnico](https://helpdesk.visioforge.com/helpdesk/my-tickets/new) para asistencia personalizada.