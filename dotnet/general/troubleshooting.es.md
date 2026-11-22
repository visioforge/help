---
title: Solución de problemas del SDK .NET — errores comunes
description: Corrija errores comunes de los SDK .NET de VisioForge — filtros y DLL ausentes, MF Mux, REGDB_E_CLASSNOTREG, framework de destino, MSB3821 y NVENC.
sidebar_label: Solución de problemas
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - Deployment
  - NuGet
  - Troubleshooting
primary_api_classes:
  - VideoCaptureCore
  - MediaPlayerCore
  - VideoEditCore
  - VideoView

---

# Solución de errores comunes de los SDK .NET

Estos son los errores que los desarrolladores encuentran con más frecuencia al compilar o desplegar por primera vez una aplicación con Video Capture SDK .NET, Media Player SDK .NET, Video Edit SDK .NET o Media Blocks SDK .NET. La mayoría son problemas de despliegue: un componente nativo que la aplicación necesita no está junto a ella o no está registrado.

Si ninguna de las respuestas siguientes le sirve, implemente el evento `OnError` del objeto del motor, reproduzca el problema y [envíe los registros](sendlogs.md) al soporte.

## Errores de despliegue

### "Unable to create filter" o "Unable to add … filter"

Mensajes como `Unable to create filter: VisioForge Resize`, `Unable to add Video Effects filter`, `Unable to create LAV Splitter Source filter` o `Unable to add filter: … REGDB_E_CLASSNOTREG (0x80040154)` proceden de los motores de Windows (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`). Significan que un filtro DirectShow que el motor necesita no está disponible en ese equipo.

La solución es desplegar los redistribuibles:

1. **NuGet (recomendado):** añada los paquetes `VisioForge.DotNet.Core.Redist.*` que necesiten sus funciones: el paquete base, el paquete de su SDK y los paquetes de funciones (MP4, FFMPEG, LAV, VLC, etc.). Sus archivos se copian a la carpeta de salida al compilar y se cargan desde allí sin registro.
2. **Instalador o despliegue manual:** ejecute el instalador de redistribuibles, o copie y registre los filtros como se describe en la guía de despliegue.

La arquitectura del paquete debe coincidir con la del proceso: un proceso x64 no puede cargar filtros x86. Si funciona en el equipo de desarrollo pero no en uno limpio, compare las carpetas de salida: la causa habitual es un paquete instalado solo en el equipo de desarrollo.

Las listas completas de paquetes están en las guías de despliegue: [Video Capture SDK](../videocapture/deployment.md), [Media Player SDK](../mediaplayer/deployment.md), [Video Edit SDK](../videoedit/deployment.md).

### "Unable to create VisioForge MF Mux filter"

Las salidas Media Foundation de `VideoCaptureCore` — `MP4HWOutput`, `MPEGTSOutput` y las demás salidas que usan codificadores de Microsoft o de GPU — escriben el archivo mediante el filtro VisioForge MF Mux, que se incluye en el redistribuible MP4. Añada el paquete `VisioForge.DotNet.Core.Redist.MP4.x64` (o `.x86`, `.arm64` según su plataforma). El mismo paquete es necesario para la fuente RTSP de baja latencia.

### DllNotFoundException: Unable to load DLL 'VisioForge_MFP64.dll'

`VisioForge_MFP64.dll` (y `VisioForge_MFP.dll` para x86) es una biblioteca nativa del SDK. Se incluye en los paquetes `VisioForge.DotNet.Core.Redist.Base` para los motores de Windows y en los paquetes `VisioForge.CrossPlatform.Core.Windows` para los motores X y Media Blocks SDK .NET. La excepción significa que no está en la carpeta de la aplicación o que su arquitectura no coincide con la del proceso:

- Añada el paquete redistribuible (por ejemplo `VisioForge.DotNet.Core.Redist.Base.x64`) al propio proyecto de la aplicación, no solo a una biblioteca de clases a la que hace referencia.
- Compile para una plataforma explícita (`x64` o `x86`) en lugar de AnyCPU cuando despliegue un solo conjunto de redistribuibles.
- Si los paquetes están referenciados y el archivo sigue sin aparecer, quítelos y vuelva a añadirlos (o elimine las carpetas `bin` y `obj`) para que el paso de copia se ejecute de nuevo.

### No se encuentra libgstreamer-1.0 o no se encuentra VideoCaptureCore

Los motores de Windows (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) y los controles de interfaz de Windows solo existen en la compilación para Windows del SDK. Un proyecto cuyo framework de destino no tiene el sufijo `-windows` (por ejemplo `net8.0`) recibe la compilación multiplataforma: el compilador no encuentra los tipos de Windows, y un motor X o una canalización de Media Blocks busca al iniciarse las bibliotecas GStreamer de Linux (`libgstreamer-1.0.so`).

Use un framework de destino de Windows, por ejemplo:

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

Compruebe también que la aplicación se ejecuta en la misma plataforma (x64 o x86) que los paquetes redistribuibles que ha añadido.

### Error MSB3821 en archivos .resx de un ejemplo descargado

`Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file` significa que Windows marcó como no confiables los archivos de un ZIP descargado. Desbloquee el archivo comprimido antes de extraerlo, o clone el [repositorio de ejemplos](https://github.com/visioforge/.Net-SDK-s-samples) con git. Los pasos están en [Problema de compilación con archivos RESX de WinForms](../deployment-x/Windows.md#problema-de-compilacion-con-archivos-resx-de-winforms).

## Actualización desde versiones anteriores

### No se encuentran nombres de clase antiguos como VFMP4v8v10Output o el control VideoCapture

Los nombres antiguos de las clases de salida corresponden a la API actual: `VFMP4v8v10Output` (renombrada `VFMP4Output` en la versión 14.2) ahora es `MP4Output`. La versión 15 dividió los controles de interfaz únicos `VideoCapture`, `MediaPlayer` y `VideoEdit` en un control `VideoView`, que muestra el video, y una clase de motor (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) que conserva la API de propiedades y métodos conocida. Cree el motor en código y pásele el `VideoView`, como hacen los [ejemplos](https://github.com/visioforge/.Net-SDK-s-samples). Para la versión actual, consulte la [guía de migración](../migration-from-v15.md).

## Licencias

### Aparece el texto "UNREGISTERED VERSION" sobre el video

Esa superposición la dibuja la versión de prueba de 30 días. Su segunda línea indica la edición que requieren las funciones en uso (STANDARD, PROFESSIONAL o PREMIUM) y la última es la página de compra, visioforge.com/buy; las canalizaciones de Media Blocks y `VideoEditCoreX` dibujan solo UNREGISTERED y la dirección. Una licencia comprada se entrega como un archivo de certificado `.vflicense`: cárguelo en cada instancia del motor con `SetLicenseCertificateAsync` antes de iniciarla. Consulte un ejemplo en [Video Capture SDK .NET](../videocapture/index.md). Si se ve la superposición de prueba pero no se reproduce nada, el mensaje no es la causa: revise el evento `OnError` para ver el error real.

## Codificación por hardware

### El codificador NVIDIA NVENC funciona en un PC pero no en otro

Cuando NVENC falla en un equipo (un mensaje `Unable to activate encoder`, ningún archivo de salida) mientras funciona en otro, compruebe lo siguiente:

- Instale el controlador más reciente desde el sitio web de NVIDIA; los controladores que instala Windows Update pueden ser más antiguos que la versión de NVENC que usa el SDK.
- Las tarjetas GeForce limitan el número de sesiones de codificación simultáneas. Cierre otras aplicaciones que codifiquen (OBS, navegadores que comparten la pantalla, grabadores) y vuelva a probar.
- Las GPU antiguas no admiten todos los códecs, por ejemplo HEVC. Pruebe primero con H.264.
- Pruebe la misma configuración con la aplicación de demostración del SDK. Si la demostración también falla, [envíe los registros](sendlogs.md).

Use otro codificador (Intel Quick Sync, AMD AMF o un codificador por software) como alternativa en los equipos sin un NVENC operativo. Las opciones de codificador están en [Codificadores H.264](video-encoders/h264.md).
