---
title: Despliegue del SDK .NET multiplataforma para Windows
description: Despliegue sus aplicaciones con el SDK .NET de VisioForge en Windows: paquetes NuGet, dependencias y configuración de arquitectura x86/x64.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - USB3 Vision / GigE
  - NuGet
primary_api_classes:
  - AWSS3SinkBlock
  - CVDewarpBlock
  - CVDilateBlock
  - CVErodeBlock

---

# Guía de instalación y despliegue de Windows para SDK multiplataforma de VisioForge

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la instalación y despliegue del SDK de VisioForge

La suite de SDKs de VisioForge proporciona potentes capacidades multimedia para sus aplicaciones .NET, soportando captura, edición, reproducción y procesamiento avanzado de medios en múltiples plataformas. Esta guía completa cubre tanto la instalación como el despliegue para aplicaciones Windows.

## Instalación

Los SDKs están accesibles en dos formas: un archivo de instalación y paquetes NuGet. El archivo de instalación proporciona un proceso de instalación directo, asegurando que todos los componentes necesarios estén correctamente configurados. Por otro lado, los paquetes NuGet ofrecen un enfoque flexible y modular para incorporar SDKs en sus proyectos, permitiendo actualizaciones fáciles y gestión de dependencias. Recomendamos altamente utilizar paquetes NuGet debido a su conveniencia y eficiencia en la gestión de dependencias y actualizaciones de proyectos.

Al construir su aplicación, tiene la opción de crear versiones tanto x86 como x64. Esto permite que su aplicación se ejecute en una gama más amplia de sistemas, acomodando diferentes arquitecturas de hardware. Sin embargo, es importante notar que el archivo de instalación está disponible exclusivamente para la arquitectura x64. Esto significa que mientras puede desarrollar y compilar builds x86, el proceso de instalación inicial requerirá un sistema x64.

### IDEs

Para desarrollo, puede usar potentes entornos de desarrollo integrado (IDEs) como JetBrains Rider o Visual Studio. Ambos IDEs ofrecen herramientas y características robustas para agilizar el proceso de desarrollo en Windows. Para asegurar una configuración fluida, por favor consulte las respectivas guías de instalación. La [página de instalación de Rider](../install/rider.md) proporciona instrucciones detalladas para configurar JetBrains Rider, mientras que la [página de instalación de Visual Studio](../install/visual-studio.md) ofrece guía completa sobre instalación y configuración de Visual Studio. Estos recursos le ayudarán a comenzar rápida y efectivamente, aprovechando las capacidades completas de estos entornos de desarrollo.

## Distribución y gestión de paquetes

Los componentes del SDK de VisioForge para Windows se distribuyen como paquetes NuGet, haciendo la integración directa con entornos de desarrollo .NET modernos. Puede agregar estos paquetes a su proyecto usando cualquiera de las siguientes herramientas:

- Visual Studio Package Manager
- JetBrains Rider NuGet Manager
- Visual Studio Code con extensiones NuGet
- Integración directa de línea de comandos usando .NET CLI

## Paquetes base requeridos

Cada aplicación Windows construida con el SDK de VisioForge requiere el paquete base apropiado según la arquitectura objetivo de su aplicación. Estos paquetes contienen los componentes esenciales para la funcionalidad del SDK.

### Paquetes principales de plataforma

- [VisioForge.CrossPlatform.Core.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x86) - Para aplicaciones Windows de 32 bits
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64) - Para aplicaciones Windows de 64 bits
- [VisioForge.CrossPlatform.Core.Windows.arm64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.arm64) - Para aplicaciones Windows on Arm (ARM64); a diferencia de x64, aquí la reproducción de MP3 requiere `VisioForge.CrossPlatform.Libav.Windows.arm64` (más abajo)

> **Nota**: Para aplicaciones dirigidas a múltiples arquitecturas, debe incluir el paquete de cada arquitectura que distribuya e implementar lógica de selección de runtime apropiada.

## Paquetes de componentes opcionales

Dependiendo de los requisitos de su aplicación, puede necesitar incluir paquetes adicionales para funcionalidad especializada. Estos componentes opcionales extienden las capacidades del SDK en varios dominios.

### Intel Quick Sync Video (QSV)

Los codificadores y decodificadores por hardware QSV (`QSVH264EncoderSettings`, `QSVHEVCEncoderSettings`, `QSVAV1EncoderSettings` y el resto de ajustes `QSV*`) se distribuyen en un paquete aparte. Sin él, crear un codificador QSV falla con `Unable to create H264 encoder 'qsvh264enc'`:

- [VisioForge.CrossPlatform.Core.Windows.Intel.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Intel.x86) - Intel QSV para aplicaciones de 32 bits
- [VisioForge.CrossPlatform.Core.Windows.Intel.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Intel.x64) - Intel QSV para aplicaciones de 64 bits

### Procesamiento de medios FFMPEG (recomendado)

Estos paquetes proporcionan soporte completo de codecs para una amplia gama de formatos de medios a través de la integración de la biblioteca FFMPEG:

- [VisioForge.CrossPlatform.Libav.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86) - Soporte FFMPEG de 32 bits
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64) - Soporte FFMPEG de 64 bits
- [VisioForge.CrossPlatform.Libav.Windows.arm64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.arm64) - Soporte FFMPEG para ARM64. Ningún paquete Core incluye los elementos FFmpeg (libav), y `Core.Windows.arm64` tampoco incluye el decodificador MP3 mpg123, por lo que en Windows on Arm la reproducción de MP3 también requiere este paquete

Para aplicaciones con restricciones de tamaño, versiones comprimidas de estos paquetes utilizando compresión UPX están disponibles:

- [VisioForge.CrossPlatform.Libav.Windows.x86.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86.UPX) - Soporte FFMPEG de 32 bits comprimido
- [VisioForge.CrossPlatform.Libav.Windows.x64.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64.UPX) - Soporte FFMPEG de 64 bits comprimido

### Integración en la nube: Amazon Web Services

Para aplicaciones que requieren integración de almacenamiento en la nube con AWS S3:

- [VisioForge.CrossPlatform.AWS.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x86) - Soporte AWS de 32 bits
- [VisioForge.CrossPlatform.AWS.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x64) - Soporte AWS de 64 bits
- [VisioForge.CrossPlatform.AWS.Windows.arm64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.arm64) - Soporte AWS para ARM64

Al usar estos paquetes, el siguiente Media Block está disponible:

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

## Paquetes de soporte de hardware especializado

El SDK de VisioForge proporciona integración con sistemas de cámaras profesionales y hardware especializado. Incluya el paquete apropiado cuando trabaje con tipos de dispositivos específicos.

### Cámaras Allied Vision

Para integración con hardware de cámaras profesionales Allied Vision:

- [VisioForge.CrossPlatform.AlliedVision.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AlliedVision.Windows.x64)

### Cámaras Basler

Para aplicaciones que trabajan con cámaras industriales Basler:

- [VisioForge.CrossPlatform.Basler.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Basler.Windows.x64)

### Cámaras Teledyne/FLIR (SDK Spinnaker)

Para imagen térmica y cámaras FLIR especializadas:

- [VisioForge.CrossPlatform.Spinnaker.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Spinnaker.Windows.x64)

### Soporte de protocolo GenICam (GigE/USB3 Vision)

Para cámaras que utilizan el protocolo estandarizado GenICam:

- [VisioForge.CrossPlatform.GenICam.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.GenICam.Windows.x64) - Soporte GenICam de 64 bits
- [VisioForge.CrossPlatform.GenICam.Windows.arm64](https://www.nuget.org/packages/VisioForge.CrossPlatform.GenICam.Windows.arm64) - Soporte GenICam para ARM64 (.NET 5 o posterior)

## Mejores prácticas de despliegue

Al desplegar aplicaciones basadas en VisioForge para Windows, considere estas recomendaciones:

1. Elija los paquetes de arquitectura apropiados (x86 o x64) basándose en su plataforma objetivo
2. Incluya los paquetes FFMPEG para soporte completo de formatos de medios
3. Solo incluya paquetes de hardware especializado cuando sea necesario para minimizar el tamaño de despliegue
4. Para aplicaciones sensibles a la seguridad, considere usar las versiones comprimidas UPX para ofuscar bibliotecas nativas
5. Siempre pruebe su despliegue en un sistema limpio para asegurar que todas las dependencias se resuelvan correctamente

## La caché compartida de plugins nativos

Al iniciarse, el SDK no carga sus bibliotecas de GStreamer directamente desde la carpeta de salida de su
aplicación. Copia la subcarpeta `x64`, `x86` o `arm64` de esa carpeta a una ubicación compartida y carga
desde allí:

```text
%PUBLIC%\.gstreamer\<version-del-sdk>-<conjunto-nativo>\<arquitectura>
```

GStreamer mantiene un registro de plugins que anota cada plugin por su **ruta absoluta**. Por eso una
única carpeta canónica permite que todas las aplicaciones de VisioForge de la máquina compartan un mismo
registro ya calentado: solo el primer inicio tras actualizar el SDK paga el coste de escanear el conjunto
de plugins, y cualquier inicio posterior de cualquier aplicación lo reutiliza. Además, esa carpeta siempre
tiene permiso de escritura, cosa que el directorio de una aplicación instalada bajo `Program Files` no tiene.

La carpeta está delimitada por la versión del SDK **y** por una huella de los redistribuibles core y Libav que
la aplicación realmente distribuye - los paquetes `VisioForge.CrossPlatform.*` se versionan de forma
independiente del SDK gestionado - de modo que las aplicaciones con redistribuibles distintos ya no se
sobrescriben las bibliotecas en cada arranque. Dos despliegues que solo difieran en un paquete que ninguna de
las dos huellas cubre (OpenCV, AWS, el de un fabricante de cámaras) sí comparten carpeta; los nombres de
archivo de esos paquetes son disjuntos del resto, así que el efecto es que unos pocos archivos se vuelven a
copiar, no que se cargue una biblioteca equivocada. Una caché creada por
el SDK que no se ha iniciado durante 30 días se elimina; una carpeta dejada por un SDK anterior con el diseño
compartido previo se deja intacta, porque las aplicaciones construidas con esa versión siguen cargando de
ella.

Dos consecuencias que conviene conocer al probar un despliegue:

- La caché acumula las bibliotecas de todas las aplicaciones de esa versión del SDK en la máquina. Un
  equipo de desarrollo que haya ejecutado un conjunto de paquetes más completo puede por tanto satisfacer
  una dependencia que su aplicación en realidad no distribuye, y esa misma aplicación falla luego en una
  máquina limpia.
- Borrar `%PUBLIC%\.gstreamer` es seguro en cualquier momento; el siguiente inicio la reconstruye.

Para dejar la caché compartida fuera de juego y cargar exactamente lo que su aplicación distribuye, apunte
`VisioForgeX.CacheFolder` a su propia carpeta nativa **antes** de llamar a `InitSDK`:

```csharp
VisioForgeX.CacheFolder = Path.Combine(AppContext.BaseDirectory, "x64");
VisioForgeX.InitSDK();
```

Esta es la configuración adecuada para verificar que un redistribuible está completo, y para una aplicación
que no debe escribir fuera de su propio directorio. El coste es que la aplicación ya no comparte el registro
de toda la máquina y escanea el conjunto de plugins por su cuenta en cada primer inicio.

Las aplicaciones MAUI, WinUI 3 y Unity siempre cargan en su sitio y nunca usan la caché compartida.

## Solución de problemas comunes

### Errores de carga de bibliotecas nativas

Si un motor basado en GStreamer o un MediaBlock lanza `DllNotFoundException`, consulte el mensaje del SDK:
indica el paquete de redistribución para la **arquitectura del proceso**, las carpetas de bibliotecas
nativas y la guía de despliegue. Una `BadImageFormatException` puede indicar una biblioteca compilada
para otra arquitectura. Si la redistribución está presente, compruebe sus dependencias, incluido el
entorno de ejecución de Visual C++.

`HelpLinks.DeploymentX`, en `VisioForge.Core.UI`, devuelve la guía de despliegue para la plataforma de
destino del SDK. Visual Studio puede detenerse en la excepción original antes de que el SDK la capture
si está activada la interrupción al lanzar excepciones; la excepción que recibe su aplicación incluye
las instrucciones del SDK.

### Problemas de despliegue

Si encuentra problemas después del despliegue:

1. Verifique que todos los paquetes NuGet requeridos estén correctamente incluidos
2. Verifique que la arquitectura (x86/x64) coincida con su objetivo de aplicación
3. Asegúrese de que las bibliotecas nativas se extraigan a las ubicaciones correctas
4. Revise las configuraciones de seguridad y permisos de Windows que podrían restringir la funcionalidad de medios

### Problema de compilación con archivos RESX de WinForms

A veces puede obtener el siguiente error:

`Error MSB3821: Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file. Remove the mark of the web if you want to process these files.`

El Error MSB3821 ocurre cuando Visual Studio o MSBuild no puede procesar un archivo de recurso `.resx` porque está marcado como no confiable. Esto sucede cuando el archivo tiene la "Marca de la Web" (MOTW), una característica de seguridad que marca archivos descargados de internet o recibidos de fuentes no confiables. La MOTW coloca el archivo en la zona de seguridad de Internet o Restringida, impidiendo que sea procesado durante una compilación.

#### Cómo solucionarlo

Para resolver este error, necesita eliminar la MOTW del archivo afectado:

##### Desbloquear el archivo manualmente

- Haga clic derecho en Form1.resx en el Explorador de archivos.
- Seleccione Propiedades.
- En la pestaña General, busque un botón o casilla de verificación Desbloquear en la parte inferior.
- Haga clic en Desbloquear, luego haga clic en Aceptar.

##### Desbloquear vía PowerShell (para múltiples archivos)

- Abra PowerShell.
- Navegue al directorio de su proyecto.
- Ejecute el comando: Get-ChildItem -Path . -Recurse | Unblock-File

##### Desbloquear el ZIP antes de extraer

- Si descargó el proyecto como archivo ZIP, haga clic derecho en el archivo ZIP.
- Seleccione Propiedades.
- Haga clic en Desbloquear, luego extraiga los archivos.

Al desbloquear el archivo se elimina la MOTW, permitiendo que Visual Studio lo procese normalmente durante la compilación.

Para asistencia adicional, visite el [sitio de soporte de VisioForge](https://support.visioforge.com/) o consulte la [documentación de API](https://api.visioforge.org/dotnet/api/index.html).

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.