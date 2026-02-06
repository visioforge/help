---
title: Despliegue del SDK .Net Multiplataforma para Windows
description: Despliegue del SDK .NET de VisioForge para Windows con paquetes NuGet, dependencias y configuración de arquitectura x86/x64 para aplicaciones multimedia.
---

# Guía de Instalación y Despliegue de Windows para SDK Multiplataforma de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Instalación y Despliegue del SDK de VisioForge

La suite de SDKs de VisioForge proporciona potentes capacidades multimedia para tus aplicaciones .NET, soportando captura, edición, reproducción y procesamiento avanzado de medios en múltiples plataformas. Esta guía completa cubre tanto la instalación como el despliegue para aplicaciones Windows.

## Instalación

Los SDKs están accesibles en dos formas: un archivo de instalación y paquetes NuGet. El archivo de instalación proporciona un proceso de instalación directo, asegurando que todos los componentes necesarios estén correctamente configurados. Por otro lado, los paquetes NuGet ofrecen un enfoque flexible y modular para incorporar SDKs en tus proyectos, permitiendo actualizaciones fáciles y gestión de dependencias. Recomendamos altamente utilizar paquetes NuGet debido a su conveniencia y eficiencia en la gestión de dependencias y actualizaciones de proyectos.

Al construir tu aplicación, tienes la opción de crear versiones tanto x86 como x64. Esto permite que tu aplicación se ejecute en una gama más amplia de sistemas, acomodando diferentes arquitecturas de hardware. Sin embargo, es importante notar que el archivo de instalación está disponible exclusivamente para la arquitectura x64. Esto significa que mientras puedes desarrollar y compilar builds x86, el proceso de instalación inicial requerirá un sistema x64.

### IDEs

Para desarrollo, puedes usar potentes entornos de desarrollo integrado (IDEs) como JetBrains Rider o Visual Studio. Ambos IDEs ofrecen herramientas y características robustas para agilizar el proceso de desarrollo en Windows. Para asegurar una configuración fluida, por favor consulta las respectivas guías de instalación. La [página de instalación de Rider](../install/rider.md) proporciona instrucciones detalladas para configurar JetBrains Rider, mientras que la [página de instalación de Visual Studio](../install/visual-studio.md) ofrece guía completa sobre instalación y configuración de Visual Studio. Estos recursos te ayudarán a comenzar rápida y efectivamente, aprovechando las capacidades completas de estos entornos de desarrollo.

## Distribución y Gestión de Paquetes

Los componentes del SDK de VisioForge para Windows se distribuyen como paquetes NuGet, haciendo la integración directa con entornos de desarrollo .NET modernos. Puedes agregar estos paquetes a tu proyecto usando cualquiera de las siguientes herramientas:

- Visual Studio Package Manager
- JetBrains Rider NuGet Manager
- Visual Studio Code con extensiones NuGet
- Integración directa de línea de comandos usando .NET CLI

## Paquetes Base Requeridos

Cada aplicación Windows construida con el SDK de VisioForge requiere el paquete base apropiado según la arquitectura objetivo de tu aplicación. Estos paquetes contienen los componentes esenciales para la funcionalidad del SDK.

### Paquetes Principales de Plataforma

- [VisioForge.CrossPlatform.Core.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x86) - Para aplicaciones Windows de 32 bits
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64) - Para aplicaciones Windows de 64 bits

> **Nota**: Para aplicaciones dirigidas a múltiples arquitecturas, debes incluir ambos paquetes e implementar lógica de selección de runtime apropiada.

## Paquetes de Componentes Opcionales

Dependiendo de los requisitos de tu aplicación, puedes necesitar incluir paquetes adicionales para funcionalidad especializada. Estos componentes opcionales extienden las capacidades del SDK en varios dominios.

### Procesamiento de Medios FFMPEG (Recomendado)

Estos paquetes proporcionan soporte completo de codecs para una amplia gama de formatos de medios a través de la integración de la biblioteca FFMPEG:

- [VisioForge.CrossPlatform.Libav.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86) - Soporte FFMPEG de 32 bits
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64) - Soporte FFMPEG de 64 bits

Para aplicaciones con restricciones de tamaño, versiones comprimidas de estos paquetes utilizando compresión UPX están disponibles:

- [VisioForge.CrossPlatform.Libav.Windows.x86.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86.UPX) - Soporte FFMPEG de 32 bits comprimido
- [VisioForge.CrossPlatform.Libav.Windows.x64.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64.UPX) - Soporte FFMPEG de 64 bits comprimido

### Integración Cloud - Amazon Web Services

Para aplicaciones que requieren integración de almacenamiento cloud con AWS S3:

- [VisioForge.CrossPlatform.AWS.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x86) - Soporte AWS de 32 bits
- [VisioForge.CrossPlatform.AWS.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x64) - Soporte AWS de 64 bits

Al usar estos paquetes, los siguientes Media Blocks están disponibles:

- `AWSS3SourceBlock` - Para recuperar medios de buckets S3
- `AWSS3SinkBlock` - Para almacenar medios en buckets S3

### Computer Vision con OpenCV

Para aplicaciones que requieren capacidades avanzadas de procesamiento de imagen y visión por computadora:

- [VisioForge.CrossPlatform.OpenCV.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.Windows.x86) - Soporte OpenCV de 32 bits
- [VisioForge.CrossPlatform.OpenCV.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.Windows.x64) - Soporte OpenCV de 64 bits

La integración con OpenCV proporciona acceso a Media Blocks en el namespace `VisioForge.Core.MediaBlocks.OpenCV`, incluyendo:

- Transformación de imagen: `CVDewarpBlock`, `CVDilateBlock`, `CVErodeBlock`
- Detección de bordes y características: `CVEdgeDetectBlock`, `CVLaplaceBlock`, `CVSobelBlock`
- Procesamiento facial: `CVFaceBlurBlock`, `CVFaceDetectBlock`
- Detección de movimiento: `CVMotionCellsBlock`
- Reconocimiento de objetos: `CVTemplateMatchBlock`, `CVHandDetectBlock`
- Mejora de imagen: `CVEqualizeHistogramBlock`, `CVSmoothBlock`
- Seguimiento y superposición: `CVTrackerBlock`, `CVTextOverlayBlock`

## Paquetes de Soporte de Hardware Especializado

El SDK de VisioForge proporciona integración con sistemas de cámaras profesionales y hardware especializado. Incluye el paquete apropiado cuando trabajes con tipos de dispositivos específicos.

### Cámaras Allied Vision

Para integración con hardware de cámaras profesionales Allied Vision:

- [VisioForge.CrossPlatform.AlliedVision.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AlliedVision.Windows.x64)

### Cámaras Basler

Para aplicaciones que trabajan con cámaras industriales Basler:

- [VisioForge.CrossPlatform.Basler.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Basler.Windows.x64)

### Cámaras Teledyne/FLIR (SDK Spinnaker)

Para imagen térmica y cámaras FLIR especializadas:

- [VisioForge.CrossPlatform.Spinnaker.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Spinnaker.Windows.x64)

### Soporte de Protocolo GenICam (GigE/USB3 Vision)

Para cámaras que utilizan el protocolo estandarizado GenICam:

- [VisioForge.CrossPlatform.GenICam.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.GenICam.Windows.x64)

## Mejores Prácticas de Despliegue

Al desplegar aplicaciones basadas en VisioForge para Windows, considera estas recomendaciones:

1. Elige los paquetes de arquitectura apropiados (x86 o x64) basándote en tu plataforma objetivo
2. Incluye los paquetes FFMPEG para soporte completo de formatos de medios
3. Solo incluye paquetes de hardware especializado cuando sea necesario para minimizar el tamaño de despliegue
4. Para aplicaciones sensibles a la seguridad, considera usar las versiones comprimidas UPX para ofuscar bibliotecas nativas
5. Siempre prueba tu despliegue en un sistema limpio para asegurar que todas las dependencias se resuelvan correctamente

## Solución de Problemas Comunes

### Problemas de Despliegue

Si encuentras problemas después del despliegue:

1. Verifica que todos los paquetes NuGet requeridos estén correctamente incluidos
2. Verifica que la arquitectura (x86/x64) coincida con tu objetivo de aplicación
3. Asegura que las bibliotecas nativas se extraigan a las ubicaciones correctas
4. Revisa las configuraciones de seguridad y permisos de Windows que podrían restringir la funcionalidad de medios

### Problema de Compilación con Archivos RESX de WinForms

A veces puedes obtener el siguiente error:

`Error MSB3821: Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file. Remove the mark of the web if you want to process these files.`

El Error MSB3821 ocurre cuando Visual Studio o MSBuild no puede procesar un archivo de recurso `.resx` porque está marcado como no confiable. Esto sucede cuando el archivo tiene la "Marca de la Web" (MOTW), una característica de seguridad que marca archivos descargados de internet o recibidos de fuentes no confiables. La MOTW coloca el archivo en la zona de seguridad de Internet o Restringida, impidiendo que sea procesado durante una compilación.

#### Cómo Solucionarlo

Para resolver este error, necesitas eliminar la MOTW del archivo afectado:

##### Desbloquear el Archivo Manualmente

- Haz clic derecho en Form1.resx en el Explorador de Archivos.
- Selecciona Propiedades.
- En la pestaña General, busca un botón o casilla de verificación Desbloquear en la parte inferior.
- Haz clic en Desbloquear, luego haz clic en Aceptar.

##### Desbloquear vía PowerShell (para múltiples archivos)

- Abre PowerShell.
- Navega al directorio de tu proyecto.
- Ejecuta el comando: Get-ChildItem -Path . -Recurse | Unblock-File

##### Desbloquear el ZIP Antes de Extraer

- Si descargaste el proyecto como archivo ZIP, haz clic derecho en el archivo ZIP.
- Selecciona Propiedades.
- Haz clic en Desbloquear, luego extrae los archivos.

Al desbloquear el archivo, eliminas la MOTW, permitiendo que Visual Studio lo procese normalmente durante la compilación.

Para asistencia adicional, visita el [sitio de soporte de VisioForge](https://support.visioforge.com/) o consulta la [documentación de API](https://api.visioforge.org/dotnet/api/index.html).

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.