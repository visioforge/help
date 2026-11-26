---
title: Ejecutar el SDK .NET de forma nativa en Windows ARM64
description: Compile una aplicación nativa para Windows en Arm con Media Blocks SDK .NET. Paquetes de runtime ARM64 y los problemas de empaquetado de Smart App Control.
sidebar_label: Windows en ARM64
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - ARM64
  - WinForms
  - GStreamer
  - NuGet
  - Deployment
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock
---

# Ejecutar el SDK .NET de forma nativa en Windows ARM64

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk){ .md-button .md-button--primary target="_blank" }

En una máquina Windows en Arm, una compilación x64 de la aplicación se ejecuta en emulación. Media Blocks
SDK .NET también se ejecuta como proceso ARM64 nativo, que es más rápido y utiliza las bibliotecas ARM64
propias de la máquina. Esta página muestra qué hay que ajustar en el archivo de proyecto, qué paquetes hay
que referenciar y dónde están los dos problemas conocidos.

## Qué se ejecuta de forma nativa

| Motor | En un proceso ARM64 nativo |
|---|---|
| Media Blocks SDK .NET (`MediaBlocksPipeline`) | Sí. GStreamer, los plugins de códecs, el renderizador de video y los decodificadores de archivos son ARM64. |
| `VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX` | Sí. |
| `VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore` (DirectShow) | En parte. `VisioForge.DotNet.Core.Redist.Base.arm64` contiene los filtros base, los efectos de video, los mezcladores de video y audio, el renderizador de audio WASAPI y la captura de pantalla por Desktop Duplication; `Redist.MP4.arm64` las salidas MP4, MPEG-TS, MKV y HLS con los codificadores H.264/H.265 de Media Foundation. Los filtros LAV y los codificadores y muxers DirectShow x64 heredados no tienen versión ARM64: una aplicación DirectShow que decodifica archivos con LAV falla en un proceso ARM64, así que compílela para x64 y deje que Windows la ejecute en emulación. |

NDI, ElevenLabs y los SDK de cámaras Basler, Allied Vision y Spinnaker no tienen versión ARM64, y
sus bloques informan de un error en un proceso ARM64. Las cámaras GenICam y RealSense funcionan de forma nativa con
`VisioForge.CrossPlatform.GenICam.Windows.arm64` y `VisioForge.CrossPlatform.RealSense.Windows.arm64`.

## Ajustes del proyecto

```xml
<PropertyGroup>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>ARM64</PlatformTarget>
  <RuntimeIdentifier>win-arm64</RuntimeIdentifier>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.arm64" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.arm64" Version="2026.*" />
</ItemGroup>
```

`PlatformTarget` y los paquetes deben coincidir. Con `PlatformTarget` en `x64` y los paquetes ARM64
referenciados, la aplicación se compila igualmente como proceso x64, se ejecuta en emulación e intenta
cargar las bibliotecas nativas x64.

`VisioForge.DotNet.MediaBlocks` es el mismo paquete gestionado para todas las arquitecturas; solo
cambian los paquetes de runtime nativo. `Core.Windows.arm64` contiene el runtime de GStreamer. `Libav.Windows.arm64` contiene los plugins de
FFmpeg (`gstlibav`) que decodifican H.264, HEVC y los demás formatos habituales a través de la fuente de
archivo; el paquete Core no los incluye. Añada `VisioForge.CrossPlatform.Core.Windows.Adds.arm64` solo
para los plugins opcionales que incluye, igual que haría en x64.

Compile y publique en cualquier máquina, incluida una x64:

```powershell
dotnet build -c Release -r win-arm64
dotnet publish -c Release -r win-arm64
```

La carpeta de salida contiene la misma subcarpeta `arm64` con el runtime nativo que la compilación x64
deja en `x64`, así que no hace falta ningún paso de copia adicional.

Esa carpeta también contiene el runtime de Microsoft Visual C++ que necesitan los binarios ARM64
(`vcruntime140.dll`, `msvcp140.dll`, `concrt140.dll`), igual que los paquetes x64, así que el equipo de
destino no necesita el Visual C++ ARM64 Redistributable. Los paquetes ARM64 anteriores no lo incluían, y en
un equipo sin ese redistribuible GStreamer podía no cargarse. Actualice los paquetes ARM64 a la versión
actual o instale el Microsoft Visual C++ Redistributable para ARM64 más reciente.

## Problemas conocidos

**Salida de publicación duplicada (`NETSDK1152`).** Los paquetes ARM64 anteriores incluían las DLL de
FFmpeg tanto en `Core.Windows.arm64` como en `Libav.Windows.arm64`. `dotnet build` funcionaba y
`dotnet publish -r win-arm64` fallaba con *Found multiple publish output files with the same relative
path*. Está corregido en los paquetes actuales: mantenga ambas referencias en una versión 2026 actual y
la publicación funciona.

**Smart App Control.** En una instalación limpia de Windows 11 Arm, Smart App Control se niega a cargar
cualquier binario que no esté firmado con Authenticode. Los archivos de los paquetes publicados de
VisioForge están firmados; el EXE de su aplicación no lo está salvo que usted lo firme, y Smart App Control
lo bloquea, así que la aplicación no arranca. Si compila el SDK desde el código fuente, sus ensamblados
tampoco están firmados y el SDK informa de un "módulo no encontrado" silencioso. Firme el EXE y las DLL que
distribuya, o desarrolle en una máquina con Smart App Control desactivado o en modo de evaluación.

## Ejemplo completo

**[ARM64 Player Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/ARM64%20Player%20Demo)**
es una aplicación WinForms que reproduce un archivo en una vista previa en vivo e imprime la arquitectura
del proceso, para que se vea que realmente se ejecuta como `Arm64`. Su archivo de proyecto son los ajustes
anteriores más los paquetes de runtime ARM64, y su readme detalla las diferencias de ARM64.
