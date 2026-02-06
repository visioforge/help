---
title: Filtro DirectShow VLC Source
description: Filtro DirectShow con VLC para reproducir 200+ formatos, video 4K/8K, streams RTSP/HLS con decodificación por hardware para aplicaciones de medios.
---

# Filtro DirectShow VLC Source

## Descripción General

El filtro DirectShow VLC Source permite a los desarrolladores integrar sin problemas capacidades avanzadas de reproducción de medios en cualquier aplicación basada en DirectShow. Este poderoso componente permite la reproducción fluida de varios archivos de video y streams de red en múltiples formatos y protocolos.

Nuestro paquete SDK ofrece una solución completa con todas las DLLs necesarias del reproductor VLC incluidas junto con un filtro DirectShow flexible. El paquete proporciona tanto interfaces de selección de archivos estándar como amplias opciones para configuraciones de filtros personalizadas para coincidir con sus requisitos de desarrollo específicos.

Para detalles completos del producto y opciones de licenciamiento, visite la [página del producto](https://www.visioforge.com/vlc-source-directshow-filter).

---

## Instalación

Antes de usar los ejemplos de código e integrar el filtro en su aplicación, primero debe instalar el Filtro DirectShow VLC Source desde la [página del producto](https://www.visioforge.com/vlc-source-directshow-filter).

**Pasos de Instalación**:

1. Descargue el instalador del SDK desde la página del producto
2. Ejecute el instalador con privilegios administrativos
3. El instalador registrará el filtro VLC Source y desplegará todas las DLLs VLC necesarias
4. Las aplicaciones de ejemplo y el código fuente estarán disponibles en el directorio de instalación

**Nota**: El filtro debe estar correctamente registrado en el sistema antes de poder usarlo en sus aplicaciones. El instalador maneja esto automáticamente.

---

## Especificaciones Técnicas

### Interfaces DirectShow Soportadas

El filtro implementa estas interfaces DirectShow estándar para máxima compatibilidad:

- **IAMStreamSelect** - Capacidades completas de selección de streams de video y audio
- **IAMStreamConfig** - Configuración avanzada de video y audio
- **IFileSourceFilter** - Especificación flexible de fuentes de nombre de archivo o URL
- **IMediaSeeking** - Soporte robusto de búsqueda y posicionamiento en línea de tiempo

### Características Principales

- Decodificación acelerada por hardware para rendimiento óptimo
- Soporte para reproducción de video 4K y 8K
- Amplia compatibilidad de formatos incluyendo códecs modernos
- Manejo de streams de red (RTSP, HLS, DASH, etc.)
- Renderizado y gestión de subtítulos
- Soporte de pistas de audio multi-idioma
- Capacidades de reproducción de video 360°
- Soporte de contenido HDR

## Ejemplos de Implementación

### Ejemplo de Integración C++

```cpp
#include <dshow.h>
#include <mfapi.h>
#include <evr.h>
#include "ivlcsrc.h"

// CLSID del Filtro VLC Source
DEFINE_GUID(CLSID_VlcSource,
    0x9f73dcd4, 0x2fc8, 0x4e89, 0x8f, 0xc3, 0x2d, 0xf1, 0x69, 0x3c, 0xa0, 0x3e);

HRESULT InitializeVLCSource(HWND hVideoWindow)
{
    HRESULT hr = S_OK;
    IFilterGraph2* pGraph = NULL;
    ICaptureGraphBuilder2* pBuild = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pVLCSource = NULL;
    IBaseFilter* pVideoRenderer = NULL;
    IFileSourceFilter* pFileSource = NULL;

    // Inicializar COM
    CoInitialize(NULL);

    // Crear el administrador del grafo de filtros
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IFilterGraph2, (void**)&pGraph);
    if (FAILED(hr))
        return hr;

    // Crear el Capture Graph Builder
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (FAILED(hr))
        goto cleanup;

    // Establecer el grafo de filtros al capture graph builder
    hr = pBuild->SetFiltergraph(pGraph);
    if (FAILED(hr))
        goto cleanup;

    // Crear el filtro VLC Source
    hr = CoCreateInstance(CLSID_VlcSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pVLCSource);
    if (FAILED(hr))
        goto cleanup;

    // Agregar el filtro al grafo
    hr = pGraph->AddFilter(pVLCSource, L"VLC Source");
    if (FAILED(hr))
        goto cleanup;

    // Cargar el archivo de medios usando la interfaz IFileSourceFilter
    hr = pVLCSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (SUCCEEDED(hr) && pFileSource)
    {
        hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
        pFileSource->Release();
        if (FAILED(hr))
            goto cleanup;
    }

    // Crear Enhanced Video Renderer (EVR)
    CLSID CLSID_EVR = { 0xFA10746C, 0x9B63, 0x4B6C,
        { 0xBC, 0x49, 0xFC, 0x30, 0x0E, 0xA5, 0xF2, 0x56 } };
    hr = CoCreateInstance(CLSID_EVR, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pVideoRenderer);
    if (FAILED(hr))
        goto cleanup;

    hr = pGraph->AddFilter(pVideoRenderer, L"EVR");
    if (FAILED(hr))
        goto cleanup;

    // Configurar EVR
    IEVRFilterConfig* pConfig = NULL;
    hr = pVideoRenderer->QueryInterface(IID_IEVRFilterConfig, (void**)&pConfig);
    if (SUCCEEDED(hr) && pConfig)
    {
        pConfig->SetNumberOfStreams(1);
        pConfig->Release();
    }

    // Establecer ventana de video
    IMFGetService* pGetService = NULL;
    hr = pVideoRenderer->QueryInterface(IID_IMFGetService, (void**)&pGetService);
    if (SUCCEEDED(hr) && pGetService)
    {
        IMFVideoDisplayControl* pDisplayControl = NULL;
        hr = pGetService->GetService(MR_VIDEO_RENDER_SERVICE,
                                     IID_IMFVideoDisplayControl,
                                     (void**)&pDisplayControl);
        if (SUCCEEDED(hr) && pDisplayControl)
        {
            pDisplayControl->SetVideoWindow(hVideoWindow);
            pDisplayControl->Release();
        }
        pGetService->Release();
    }

    // Renderizar los streams
    hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pVLCSource, NULL, pVideoRenderer);
    if (FAILED(hr))
        goto cleanup;

    hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pVLCSource, NULL, NULL);
    // Los errores de audio no son críticos

    // Obtener la interfaz de control de medios para control de reproducción
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Iniciar reproducción
        hr = pControl->Run();
    }

cleanup:
    // Liberar interfaces
    if (pControl) pControl->Release();
    if (pVideoRenderer) pVideoRenderer->Release();
    if (pVLCSource) pVLCSource->Release();
    if (pBuild) pBuild->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

### Integración C# (.NET)

```csharp
using System;
using System.Runtime.InteropServices;
using MediaFoundation;
using MediaFoundation.EVR;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

// Inicializar el filtro VLC Source en C# usando DirectShowLib
public class VLCSourcePlayer
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IMediaSeeking mediaSeeking;
    private IMediaEventEx mediaEventEx;
    private IBaseFilter sourceFilter;
    private IBaseFilter videoRenderer;

    public void Initialize(string filename, IntPtr videoWindowHandle)
    {
        try
        {
            // Crear el administrador del grafo de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;
            mediaSeeking = (IMediaSeeking)filterGraph;
            mediaEventEx = (IMediaEventEx)filterGraph;

            // Adjuntar el grafo de filtros al grafo de captura
            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Crear el filtro VLC Source usando el CLSID correcto
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source");

            // Opcional: Registrar versión comprada
            // var reg = sourceFilter as IVFRegister;
            // reg?.SetLicenseKey("su-clave-de-licencia-aqui");

            // Cargar el archivo de medios o URL usando la interfaz IFileSourceFilter
            var sourceFilterIntf = sourceFilter as IFileSourceFilter;
            hr = sourceFilterIntf.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Crear renderizador de video (EVR - Enhanced Video Renderer)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configurar EVR
            var evrConfig = videoRenderer as IEVRFilterConfig;
            evrConfig?.SetNumberOfStreams(1);

            // Establecer ventana de video para renderizado
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

            // Renderizar los streams
            hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, videoRenderer);
            DsError.ThrowExceptionForHR(hr);

            hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
            // Nota: Los errores de renderizado de audio no son críticos para reproducción solo video

            // Iniciar reproducción
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar VLC Source: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        // Detener reproducción
        if (mediaControl != null)
        {
            mediaControl.StopWhenReady();
            mediaControl.Stop();
        }

        // Dejar de recibir eventos
        mediaEventEx?.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);

        // Eliminar todos los filtros
        FilterGraphTools.RemoveAllFilters(filterGraph);

        // Liberar interfaces DirectShow
        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
        }

        if (mediaSeeking != null)
        {
            Marshal.ReleaseComObject(mediaSeeking);
            mediaSeeking = null;
        }

        if (mediaEventEx != null)
        {
            Marshal.ReleaseComObject(mediaEventEx);
            mediaEventEx = null;
        }

        if (sourceFilter != null)
        {
            Marshal.ReleaseComObject(sourceFilter);
            sourceFilter = null;
        }

        if (videoRenderer != null)
        {
            Marshal.ReleaseComObject(videoRenderer);
            videoRenderer = null;
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

**CLSIDs y GUIDs Principales:**

```csharp
// CLSID del Filtro VLC Source
public static readonly Guid CLSID_VFVLCSource = new Guid("9F73DCD4-2FC8-4E89-8FC3-2DF1693CA03E");

// IID de Interfaz IVlcSrc
[Guid("77493EB7-6D00-41C5-9535-7C593824E892")]
public interface IVlcSrc { /* ... */ }

// IID de Interfaz IVlcSrc2 (para parámetros de línea de comandos personalizados)
[Guid("CCE122C0-172C-4626-B4B6-42B039E541CB")]
public interface IVlcSrc2 : IVlcSrc { /* ... */ }

// IID de Interfaz IVlcSrc3 (para anulación de tasa de frames)
[Guid("3DFBED0C-E4A8-401C-93EF-CBBFB65223DD")]
public interface IVlcSrc3 : IVlcSrc2 { /* ... */ }

// IID de Interfaz IVFRegister (para licenciamiento)
[Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
public interface IVFRegister { /* ... */ }
```

**Paquetes NuGet Requeridos:**

- `VisioForge.DirectShowAPI` - Biblioteca wrapper de DirectShow
- `MediaFoundation.Net` - Wrapper de Media Foundation para renderizador EVR

**Ejemplo de Selección de Pista de Audio:**

```csharp
// Enumerar y seleccionar pistas de audio
var vlcSrc = sourceFilter as IVlcSrc;
if (vlcSrc != null)
{
    int audioCount = 0;
    vlcSrc.GetAudioTracksCount(out audioCount);

    for (int i = 0; i < audioCount; i++)
    {
        int trackId;
        var trackName = new StringBuilder(256);
        vlcSrc.GetAudioTrackInfo(i, out trackId, trackName);

        Console.WriteLine($"Pista {i}: {trackName} (ID: {trackId})");
    }

    // Seleccionar pista de audio específica
    if (audioCount > 1)
    {
        int desiredTrackId;
        var name = new StringBuilder(256);
        vlcSrc.GetAudioTrackInfo(1, out desiredTrackId, name);
        vlcSrc.SetAudioTrack(desiredTrackId);
    }
}
```

**Ejemplo de Opciones VLC Personalizadas:**

```csharp
// Configurar VLC para streaming RTSP de baja latencia
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    string[] parameters = new string[]
    {
        "--network-caching=300",     // Buffer de red bajo
        "--rtsp-tcp",                // Forzar TCP para RTSP
        "--avcodec-hw=any",          // Habilitar decodificación por hardware
        "--live-caching=300"         // Buffer de stream en vivo bajo
    };

    vlcSrc2.SetCustomCommandLine(parameters, parameters.Length);

    // Luego cargar el stream
    var vlcSrc = vlcSrc2 as IVlcSrc;
    vlcSrc?.SetFile("rtsp://192.168.1.100/stream");
}
```

## Historial de Versiones

### Versión 15.0

- Calidad de reproducción mejorada en numerosos formatos
- Motor de renderizado de subtítulos mejorado
- Implementaciones de códecs actualizadas incluyendo dav1d, ffmpeg y libvpx
- Agregado escalado Super Resolution con aceleración GPU nVidia e Intel

### Versión 14.0

- Actualizado a núcleo VLC v3.0.18
- Corregidos problemas de compatibilidad DxVA/D3D11 con contenido HEVC
- Resueltos problemas de redimensionamiento OpenGL para reproducción más suave

### Versión 12.0

- Actualizado a motor VLC v3.0.16
- Agregado soporte para nuevos formatos Fourcc (E-AC3 y AV1)
- Corregidos problemas de estabilidad con streams VP9

### Versión 11.1

- Incorporado VLC v3.0.11
- Mecanismo de actualización de lista de reproducción HLS optimizado
- Manejo y visualización de subtítulos WebVTT mejorados

### Versión 11.0

- Construido sobre base VLC v3.0.10
- Corregidos problemas de regresión críticos con streams HLS

### Versión 10.4

- Actualización mayor a arquitectura VLC 3.0
- Decodificación por hardware habilitada por defecto para contenido 4K y 8K
- Agregado soporte de profundidad de color de 10 bits y HDR
- Implementadas capacidades de video 360 grados y audio 3D
- Introducido soporte de menú Java para Blu-Ray

### Versión 10.0

- Lanzamiento inicial como filtro DirectShow independiente
- Para historial de versiones anteriores, consulte el changelog del SDK Video Capture .Net

## Recursos Adicionales

- [Acuerdo de Licencia de Usuario Final](../../eula.md)
- [Muestras de Código](https://github.com/visioforge/)
