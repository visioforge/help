---
title: SDK de Cámara Virtual - Ejemplos de Código
description: Ejemplos de código del SDK de Cámara Virtual para transmitir a dispositivos de cámara virtual y leer desde cámaras virtuales con renderizado cuadro por cuadro.
---

# SDK de Cámara Virtual - Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos de código para usar el SDK de Cámara Virtual. El SDK le permite:

- **Escribir A cámara virtual**: Transmitir video desde archivos, cámaras reales o cuadros individuales a dispositivos de cámara virtual
- **Leer DESDE cámara virtual**: Capturar video desde dispositivos de cámara virtual (aparece como cámara web regular para las aplicaciones)
- Aplicar efectos de video y procesamiento en tiempo real
- Soportar múltiples instancias de cámara virtual

La cámara virtual aparece como una cámara web estándar para aplicaciones como Zoom, Teams, OBS y otro software de videoconferencia.

---
## Descripción General de Arquitectura
El SDK de Cámara Virtual proporciona tres tipos principales de filtros:
1. **CLSID_VFVirtualCameraSource**: Lee DESDE dispositivo de cámara virtual (actúa como una fuente de captura de video)
2. **CLSID_VFVirtualCameraSink**: Escribe A dispositivo de cámara virtual (actúa como un renderizador)
3. **CLSID_VFVideoPushSource**: Fuente de empuje para renderizado cuadro por cuadro (secuencias de imágenes, renderizado personalizado)
**Flujos de trabajo típicos**:
- Archivo/Cámara → VirtualCameraSink → Dispositivo de Cámara Virtual → Otras Aplicaciones
- Dispositivo de Cámara Virtual → VirtualCameraSource → Su Aplicación
- PushSource (cuadros) → VirtualCameraSink → Dispositivo de Cámara Virtual → Otras Aplicaciones
---

## Prerrequisitos

### Proyectos C#

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
```

**Paquetes NuGet Requeridos**:

- `VisioForge.DirectShowAPI` - Biblioteca contenedora de DirectShow

**CLSIDs Clave**:

```csharp
// CLSIDs de Filtros (disponibles en clase Consts)
public static readonly Guid CLSID_VFVirtualCameraSource = new Guid("AA4DA14E-644B-487a-A7CB-517A390B4BB8"); // Leer desde cámara virtual
public static readonly Guid CLSID_VFVirtualCameraSink = new Guid("AA6AB4DF-9670-4913-88BB-2CB381C19340"); // Escribir a cámara virtual
public static readonly Guid CLSID_VFVirtualAudioCardSource = new Guid("B5A463DF-4016-4C34-AA4F-48EC1B51C73F"); // Fuente de audio
public static readonly Guid CLSID_VFVirtualAudioCardSink = new Guid("1A2673B0-553E-4027-AECC-839405468950"); // Sumidero de audio

// Fuente de empuje para renderizado cuadro por cuadro
public static readonly Guid CLSID_VFVideoPushSource = new Guid("38D15306-BBC6-4D6C-A89C-9621604D9FC1");
```

### Proyectos C++

```cpp
#include <dshow.h>
#include <streams.h>
#include "ivirtualcamera.h"

#pragma comment(lib, "strmiids.lib")

// CLSIDs de Filtros
DEFINE_GUID(CLSID_VFVirtualCameraSource,
    0xAA4DA14E, 0x644B, 0x487a, 0xA7, 0xCB, 0x51, 0x7A, 0x39, 0x0B, 0x4B, 0xB8);

DEFINE_GUID(CLSID_VFVirtualCameraSink,
    0xAA6AB4DF, 0x9670, 0x4913, 0x88, 0xBB, 0x2C, 0xB3, 0x81, 0xC1, 0x93, 0x40);

DEFINE_GUID(CLSID_VFVirtualAudioCardSource,
    0xB5A463DF, 0x4016, 0x4C34, 0xAA, 0x4F, 0x48, 0xEC, 0x1B, 0x51, 0xC7, 0x3F);

DEFINE_GUID(CLSID_VFVirtualAudioCardSink,
    0x1A2673B0, 0x553E, 0x4027, 0xAE, 0xCC, 0x83, 0x94, 0x05, 0x46, 0x89, 0x50);

DEFINE_GUID(CLSID_VFVideoPushSource,
    0x38D15306, 0xBBC6, 0x4D6C, 0xA8, 0x9C, 0x96, 0x21, 0x60, 0x4D, 0x9F, 0xC1);
```

---
## Ejemplo 1: Transmitir Archivo de Video a Cámara Virtual
Este ejemplo demuestra la transmisión de un archivo de video a un dispositivo de cámara virtual.
### Implementación en C#
```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VirtualCameraFileStreaming
{
    private IFilterGraph2 filterGraphSource;
    private ICaptureGraphBuilder2 captureGraphSource;
    private IMediaControl mediaControlSource;
    private IMediaEventEx mediaEventExSource;
    private IBaseFilter sourceVideoFilter;
    private IBaseFilter sinkVideoFilter;
    private IBaseFilter sinkAudioFilter;
    public void StreamFileToVirtualCamera(string videoFile)
    {
        try
        {
            // Crear gráfico de filtros
            filterGraphSource = (IFilterGraph2)new FilterGraph();
            captureGraphSource = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControlSource = (IMediaControl)filterGraphSource;
            mediaEventExSource = (IMediaEventEx)filterGraphSource;
            // Adjuntar el gráfico de filtros al gráfico de captura
            int hr = captureGraphSource.SetFiltergraph(filterGraphSource);
            DsError.ThrowExceptionForHR(hr);
            // Agregar Sumidero de Cámara Virtual para video
            sinkVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualCameraSink,
                "VisioForge Virtual Camera Sink - Video");
            // Opcional: Establecer clave de licencia para versión comprada
            var sinkIntf = sinkVideoFilter as IVFVirtualCameraSink;
            sinkIntf?.set_license("SU-CLAVE-DE-LICENCIA"); // Usar "TRIAL" para versión de prueba
            // Agregar Sumidero de Cámara Virtual para audio
            sinkAudioFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualAudioCardSink,
                "VisioForge Virtual Camera Sink - Audio");
            // Agregar filtro de fuente para el archivo de video
            // DirectShow selecciona automáticamente el filtro de fuente apropiado
            filterGraphSource.AddSourceFilter(videoFile, "Source file", out sourceVideoFilter);
            // Renderizar flujo de video: Fuente → Sumidero de Cámara Virtual
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkVideoFilter);
            DsError.ThrowExceptionForHR(hr);
            // Renderizar flujo de audio: Fuente → Sumidero de Cámara Virtual
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkAudioFilter);
            // Nota: Los errores de audio no son críticos, mejor verificar si el audio está disponible
            // Iniciar reproducción
            hr = mediaControlSource.Run();
            DsError.ThrowExceptionForHR(hr);
            Console.WriteLine("Transmitiendo a cámara virtual. Presione cualquier tecla para detener...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }
    public void Cleanup()
    {
        // Detener reproducción
        if (mediaControlSource != null)
        {
            mediaControlSource.Stop();
        }
        // Dejar de recibir eventos
        mediaEventExSource?.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
        // Eliminar todos los filtros
        FilterGraphTools.RemoveAllFilters(filterGraphSource);
        // Liberar interfaces DirectShow
        if (mediaControlSource != null)
        {
            Marshal.ReleaseComObject(mediaControlSource);
            mediaControlSource = null;
        }
        if (mediaEventExSource != null)
        {
            Marshal.ReleaseComObject(mediaEventExSource);
            mediaEventExSource = null;
        }
        if (sourceVideoFilter != null)
        {
            Marshal.ReleaseComObject(sourceVideoFilter);
            sourceVideoFilter = null;
        }
        if (sinkVideoFilter != null)
        {
            Marshal.ReleaseComObject(sinkVideoFilter);
            sinkVideoFilter = null;
        }
        if (sinkAudioFilter != null)
        {
            Marshal.ReleaseComObject(sinkAudioFilter);
            sinkAudioFilter = null;
        }
        if (captureGraphSource != null)
        {
            Marshal.ReleaseComObject(captureGraphSource);
            captureGraphSource = null;
        }
        if (filterGraphSource != null)
        {
            Marshal.ReleaseComObject(filterGraphSource);
            filterGraphSource = null;
        }
    }
}
```
### Implementación en C++
```cpp
#include <dshow.h>
#include "ivirtualcamera.h"
HRESULT StreamFileToVirtualCamera(LPCWSTR videoFile)
{
    HRESULT hr = S_OK;
    IGraphBuilder* pGraph = NULL;
    ICaptureGraphBuilder2* pBuild = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IBaseFilter* pSinkVideoFilter = NULL;
    IBaseFilter* pSinkAudioFilter = NULL;
    // Inicializar COM
    CoInitialize(NULL);
    // Crear el gestor de gráfico de filtros
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    // Crear el Constructor de Gráfico de Captura
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (FAILED(hr))
        goto cleanup;
    // Establecer el gráfico de filtros
    hr = pBuild->SetFiltergraph(pGraph);
    if (FAILED(hr))
        goto cleanup;
    // Crear filtro de Sumidero de Cámara Virtual para video
    hr = CoCreateInstance(CLSID_VFVirtualCameraSink, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSinkVideoFilter);
    if (FAILED(hr))
        goto cleanup;
    hr = pGraph->AddFilter(pSinkVideoFilter, L"VisioForge Virtual Camera Sink - Video");
    if (FAILED(hr))
        goto cleanup;
    // Crear filtro de Sumidero de Cámara Virtual para audio
    hr = CoCreateInstance(CLSID_VFVirtualAudioCardSink, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSinkAudioFilter);
    if (FAILED(hr))
        goto cleanup;
    hr = pGraph->AddFilter(pSinkAudioFilter, L"VisioForge Virtual Camera Sink - Audio");
    if (FAILED(hr))
        goto cleanup;
    // Agregar filtro de fuente para el archivo
    hr = pGraph->AddSourceFilter(videoFile, L"Source File", &pSourceFilter);
    if (FAILED(hr))
        goto cleanup;
    // Renderizar flujo de video
    hr = pBuild->RenderStream(NULL, NULL, pSourceFilter, NULL, pSinkVideoFilter);
    if (FAILED(hr))
        goto cleanup;
    // Renderizar flujo de audio (errores no críticos)
    pBuild->RenderStream(NULL, NULL, pSourceFilter, NULL, pSinkAudioFilter);
    // Obtener interfaz de control de medios
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Iniciar reproducción
        hr = pControl->Run();
    }
cleanup:
    // Liberar interfaces
    if (pControl) pControl->Release();
    if (pSinkAudioFilter) pSinkAudioFilter->Release();
    if (pSinkVideoFilter) pSinkVideoFilter->Release();
    if (pSourceFilter) pSourceFilter->Release();
    if (pBuild) pBuild->Release();
    if (pGraph) pGraph->Release();
    return hr;
}
```
---

## Ejemplo 2: Transmitir Cámara Física a Cámara Virtual

Este ejemplo demuestra la captura desde una cámara web física y su transmisión a un dispositivo de cámara virtual.

### Implementación en C#

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class VirtualCameraFromPhysicalCamera
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IBaseFilter cameraFilter;
    private IBaseFilter virtualCameraSink;

    public void StreamCameraToVirtualCamera(string physicalCameraName)
    {
        try
        {
            // Crear gráfico de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;

            // Adjuntar gráfico de filtros al gráfico de captura
            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Agregar filtro de cámara física
            cameraFilter = FilterGraphTools.AddFilterByName(
                filterGraph,
                FilterCategory.VideoInputDevice,
                physicalCameraName);

            if (cameraFilter == null)
            {
                throw new Exception($"Cámara '{physicalCameraName}' no encontrada");
            }

            // Agregar Sumidero de Cámara Virtual
            virtualCameraSink = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVirtualCameraSink,
                "Virtual Camera Sink");

            // Opcional: Establecer licencia
            var sinkIntf = virtualCameraSink as IVFVirtualCameraSink;
            sinkIntf?.set_license("TRIAL");

            // Renderizar flujo: Cámara Física → Sumidero de Cámara Virtual
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                cameraFilter,
                null,
                virtualCameraSink);
            DsError.ThrowExceptionForHR(hr);

            // Iniciar captura
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Transmitiendo cámara física a cámara virtual...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (cameraFilter != null)
        {
            Marshal.ReleaseComObject(cameraFilter);
            cameraFilter = null;
        }

        if (virtualCameraSink != null)
        {
            Marshal.ReleaseComObject(virtualCameraSink);
            virtualCameraSink = null;
        }

        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
        }

        if (captureGraph != null)
        {
            Marshal.ReleaseComObject(captureGraph);
            captureGraph = null;
        }

        if (filterGraph != null)
        {
            Marshal.ReleaseComObject(filterGraph);
            filterGraph = null;
        }
    }
}
```

---
## Ejemplo 3: Transmitir Secuencia de Imágenes a Cámara Virtual (Cuadro por Cuadro)
Este ejemplo demuestra el renderizado de cuadros individuales (secuencia de imágenes o presentación de diapositivas) a un dispositivo de cámara virtual.
### Implementación en C#
```csharp
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VirtualCameraFrameByFrame
{
    private IFilterGraph2 filterGraphSource;
    private ICaptureGraphBuilder2 captureGraphSource;
    private IMediaControl mediaControlSource;
    private IBaseFilter sourceVideoFilter;
    private IBaseFilter sinkVideoFilter;
    private IVFLiveVideoSource pushSource;
    private System.Windows.Forms.Timer framePushTimer;
    private int currentFrameIndex = 0;
    private Bitmap[] frames;
    public void StreamImageSequenceToVirtualCamera(string[] imageFiles, float frameRate = 10)
    {
        try
        {
            // Cargar imágenes en memoria
            frames = new Bitmap[imageFiles.Length];
            for (int i = 0; i < imageFiles.Length; i++)
            {
                frames[i] = new Bitmap(imageFiles[i]);
            }
            if (frames.Length == 0)
            {
                throw new Exception("No hay imágenes para mostrar");
            }
            int width = frames[0].Width;
            int height = frames[0].Height;
            // Crear gráfico de filtros
            filterGraphSource = (IFilterGraph2)new FilterGraph();
            captureGraphSource = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControlSource = (IMediaControl)filterGraphSource;
            int hr = captureGraphSource.SetFiltergraph(filterGraphSource);
            DsError.ThrowExceptionForHR(hr);
            // Agregar Sumidero de Cámara Virtual
            sinkVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualCameraSink,
                "VisioForge Virtual Camera Sink - Video");
            var sinkIntf = sinkVideoFilter as IVFVirtualCameraSink;
            sinkIntf?.set_license("TRIAL");
            // Agregar filtro de fuente de empuje
            Guid CLSID_VFVideoPushSource = new Guid("38D15306-BBC6-4D6C-A89C-9621604D9FC1");
            sourceVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                CLSID_VFVideoPushSource,
                "VisioForge Video Push Source");
            if (sourceVideoFilter == null)
            {
                throw new Exception("No se puede crear el filtro VisioForge Push Source.");
            }
            // Obtener interfaz IVFLiveVideoSource
            pushSource = sourceVideoFilter as IVFLiveVideoSource;
            if (pushSource == null)
            {
                throw new Exception("No se puede obtener la interfaz IVFLiveVideoSource.");
            }
            // Configurar formato de mapa de bits
            var bmiHeader = new BitmapInfoHeader
            {
                BitCount = 24,
                Compression = 0,
                Width = width,
                Height = height,
                Planes = 1,
                Size = Marshal.SizeOf(typeof(BitmapInfoHeader)),
                ImageSize = GetStrideRGB24(width) * height
            };
            pushSource.SetBitmapInfo(bmiHeader);
            pushSource.SetFrameRate(frameRate);
            // Conectar filtros: Push Source → Sumidero de Cámara Virtual
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkVideoFilter);
            DsError.ThrowExceptionForHR(hr);
            // Iniciar el gráfico
            hr = mediaControlSource.Run();
            DsError.ThrowExceptionForHR(hr);
            // Configurar temporizador para empujar cuadros
            framePushTimer = new System.Windows.Forms.Timer();
            framePushTimer.Interval = (int)(1000 / frameRate);
            framePushTimer.Tick += PushFrame;
            framePushTimer.Start();
            Console.WriteLine("Transmitiendo secuencia de imágenes a cámara virtual...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }
    private void PushFrame(object sender, EventArgs e)
    {
        if (frames == null || frames.Length == 0 || pushSource == null)
            return;
        // Obtener cuadro actual
        Bitmap frame = frames[currentFrameIndex];
        // Bloquear datos de mapa de bits
        BitmapData bmpData = frame.LockBits(
            new Rectangle(0, 0, frame.Width, frame.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);
        try
        {
            // Empujar cuadro a cámara virtual
            pushSource.AddFrame(bmpData.Scan0);
        }
        finally
        {
            frame.UnlockBits(bmpData);
        }
        // Mover al siguiente cuadro (bucle)
        currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
    }
    private int GetStrideRGB24(int width)
    {
        return ((width * 24 + 31) / 32) * 4;
    }
    public void Cleanup()
    {
        // Detener temporizador
        if (framePushTimer != null)
        {
            framePushTimer.Stop();
            framePushTimer.Dispose();
            framePushTimer = null;
        }
        // Detener reproducción
        if (mediaControlSource != null)
        {
            mediaControlSource.Stop();
        }
        // Liberar cuadros
        if (frames != null)
        {
            foreach (var frame in frames)
            {
                frame?.Dispose();
            }
            frames = null;
        }
        // Liberar interfaces DirectShow
        pushSource = null;
        FilterGraphTools.RemoveAllFilters(filterGraphSource);
        if (sourceVideoFilter != null)
        {
            Marshal.ReleaseComObject(sourceVideoFilter);
            sourceVideoFilter = null;
        }
        if (sinkVideoFilter != null)
        {
            Marshal.ReleaseComObject(sinkVideoFilter);
            sinkVideoFilter = null;
        }
        if (mediaControlSource != null)
        {
            Marshal.ReleaseComObject(mediaControlSource);
            mediaControlSource = null;
        }
        if (captureGraphSource != null)
        {
            Marshal.ReleaseComObject(captureGraphSource);
            captureGraphSource = null;
        }
        if (filterGraphSource != null)
        {
            Marshal.ReleaseComObject(filterGraphSource);
            filterGraphSource = null;
        }
    }
}
```
---

## Ejemplo 4: Leer Desde Cámara Virtual

Este ejemplo demuestra la captura de video desde un dispositivo de cámara virtual (útil para probar o monitorear lo que se envía a la cámara virtual).

### Implementación en C#

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using MediaFoundation;
using MediaFoundation.EVR;

public class VirtualCameraCapture
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IBaseFilter virtualCameraSource;
    private IBaseFilter videoRenderer;

    public void CaptureFromVirtualCamera(IntPtr videoWindowHandle)
    {
        try
        {
            // Crear gráfico de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;

            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Agregar filtro de Fuente de Cámara Virtual
            virtualCameraSource = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVirtualCameraSource,
                "VisioForge Virtual Camera");

            if (virtualCameraSource == null)
            {
                throw new Exception("No se puede crear el filtro de Fuente de Cámara Virtual");
            }

            // Opcional: Establecer licencia
            var cameraIntf = virtualCameraSource as IVFVirtualCameraSource;
            cameraIntf?.set_license("TRIAL");

            // Crear Renderizador de Video Mejorado (EVR)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configurar EVR
            var evrConfig = videoRenderer as IEVRFilterConfig;
            evrConfig?.SetNumberOfStreams(1);

            // Establecer ventana de video
            var getService = videoRenderer as MediaFoundation.IMFGetService;
            if (getService != null)
            {
                getService.GetService(
                    MediaFoundation.MFServices.MR_VIDEO_RENDER_SERVICE,
                    typeof(MediaFoundation.IMFVideoDisplayControl).GUID,
                    out var videoDisplayControlObj);

                var videoDisplayControl = videoDisplayControlObj as MediaFoundation.IMFVideoDisplayControl;
                videoDisplayControl?.SetVideoWindow(videoWindowHandle);
            }

            // Renderizar flujo: Fuente de Cámara Virtual → EVR
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                virtualCameraSource,
                null,
                videoRenderer);
            DsError.ThrowExceptionForHR(hr);

            // Iniciar reproducción
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Capturando desde cámara virtual...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (virtualCameraSource != null)
        {
            Marshal.ReleaseComObject(virtualCameraSource);
            virtualCameraSource = null;
        }

        if (videoRenderer != null)
        {
            Marshal.ReleaseComObject(videoRenderer);
            videoRenderer = null;
        }

        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
        }

        if (captureGraph != null)
        {
            Marshal.ReleaseComObject(captureGraph);
            captureGraph = null;
        }

        if (filterGraph != null)
        {
            Marshal.ReleaseComObject(filterGraph);
            filterGraph = null;
        }
    }
}
```

---
## Recursos Adicionales
Para información más detallada, vea:
- [Página de Producto del SDK de Cámara Virtual](https://www.visioforge.com/virtual-camera-sdk)
- [Acuerdo de Licencia de Usuario Final](../../eula.md)
- [Repositorio de Código de Muestra](https://github.com/visioforge/directshow-samples/tree/main/Virtual%20Camera%20SDK)
## Soporte
- **Soporte Técnico**: https://support.visioforge.com/
- **Comunidad de Discord**: https://discord.com/invite/yvXUG56WCH
