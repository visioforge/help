---
title: Filtro DirectShow FFMPEG Source
description: Filtro DirectShow FFMPEG para decodificar 100+ formatos incluyendo MP4, MKV, H.265 con aceleración por hardware en aplicaciones C++, C# y Delphi.
---

# Filtro DirectShow FFMPEG Source

## Introducción

El filtro DirectShow FFMPEG Source permite a los desarrolladores integrar de manera fluida capacidades avanzadas de decodificación y reproducción de medios en cualquier aplicación compatible con DirectShow. Este poderoso componente cierra la brecha entre formatos multimedia complejos y sus necesidades de desarrollo de software, proporcionando una base robusta para construir aplicaciones ricas en medios.

---

## Instalación

Antes de usar los ejemplos de código e integrar el filtro en su aplicación, primero debe instalar el Filtro DirectShow FFMPEG Source desde la [página del producto](https://www.visioforge.com/ffmpeg-source-directshow-filter).

**Pasos de Instalación**:

1. Descargue el instalador del SDK desde la página del producto
2. Ejecute el instalador con privilegios administrativos
3. El instalador registrará el filtro FFMPEG Source y desplegará todas las DLLs FFMPEG necesarias
4. Las aplicaciones de ejemplo y el código fuente estarán disponibles en el directorio de instalación

**Nota**: El filtro debe estar correctamente registrado en el sistema antes de poder usarlo en sus aplicaciones. El instalador maneja esto automáticamente.

---

## Características y Capacidades Principales

Nuestro filtro viene incluido con todas las DLLs FFMPEG necesarias y proporciona una interfaz de filtro DirectShow rica en características que soporta:

- **Amplia Compatibilidad de Formatos**: Maneje una amplia gama de formatos de video y audio incluyendo MP4, MKV, AVI, MOV, WMV, FLV, y muchos otros sin instalaciones adicionales de códecs
- **Soporte de Streams de Red**: Conecte a streams RTSP, RTMP, HTTP, UDP y TCP para integración de medios en vivo
- **Gestión de Múltiples Streams**: Seleccione entre streams de video y audio en archivos multimedia multi-stream
- **Capacidades Avanzadas de Búsqueda**: Implemente funcionalidad de búsqueda precisa en sus aplicaciones
- **Aceleración GPU**: Utilice aceleración por hardware para rendimiento óptimo

## Ejemplos de Implementación

El SDK incluye aplicaciones de ejemplo completas para múltiples entornos de desarrollo:

### Integración Delphi (Principal)

```delphi
// Inicializar el filtro FFMPEG Source en Delphi usando DSPack
procedure TMainForm.InitializeFFMPEGSource;
var
  FFMPEGFilter: IBaseFilter;
  FileSource: IFileSourceFilter;
begin
  // Crear instancia del filtro FFMPEG Source
  // IMPORTANTE: Asegurar inicialización COM apropiada antes de esta llamada
  CoCreateInstance(CLSID_FFMPEGSource, nil, CLSCTX_INPROC_SERVER, 
                  IID_IBaseFilter, FFMPEGFilter);
  
  // Consultar interfaz de fuente de archivo
  FFMPEGFilter.QueryInterface(IID_IFileSourceFilter, FileSource);
  
  // Cargar archivo de medios - puede ser local o URL de red
  FileSource.Load('C:\media\sample.mp4', nil);
  
  // Agregar al grafo de filtros para renderizado
  FilterGraph.AddFilter(FFMPEGFilter, 'FFMPEG Source');
  
  // Conectar a renderizadores apropiados o filtros de procesamiento
  // FilterGraph.RenderStream(...);
end;
```

### Integración C# (.NET)

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

// Inicializar el filtro FFMPEG Source en C# usando DirectShowLib
public class FFMPEGSourcePlayer
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

            // Crear el filtro FFMPEG Source usando el CLSID correcto
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source");

            // Opcional: Registrar versión comprada
            // var reg = sourceFilter as IVFRegister;
            // reg?.SetLicenseKey("su-clave-de-licencia-aqui");

            // Configurar ajustes del filtro
            var filterConfig = sourceFilter as IFFMPEGSourceSettings;
            if (filterConfig != null)
            {
                // Establecer modo de buffering (AUTO, ON, o OFF)
                filterConfig.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);

                // Habilitar aceleración por hardware (decodificación GPU)
                filterConfig.SetHWAccelerationEnabled(true);

                // Establecer tiempo de espera de conexión (milisegundos)
                filterConfig.SetLoadTimeOut(30000);
            }

            // Cargar el archivo de medios o stream de red
            var sourceFilterIntf = sourceFilter as IFileSourceFilter;
            hr = sourceFilterIntf.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Crear renderizador de video (EVR - Enhanced Video Renderer)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configurar EVR
            var evrConfig = videoRenderer as MediaFoundation.EVR.IEVRFilterConfig;
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
            Console.WriteLine($"Error al inicializar FFMPEG Source: {ex.Message}");
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

        if (filterGraph != null)
        {
            Marshal.ReleaseComObject(filterGraph);
            filterGraph = null;
        }

        if (captureGraph != null)
        {
            Marshal.ReleaseComObject(captureGraph);
            captureGraph = null;
        }
    }
}
```

**CLSIDs y GUIDs Principales:**

```csharp
// CLSID del Filtro FFMPEG Source
public static readonly Guid CLSID_VFFFMPEGSource = new Guid("C5255DE3-50A7-4714-B763-D99E96E4CD52");

// IID de Interfaz IFFMPEGSourceSettings
[Guid("1974D893-83E4-4F89-9908-795C524CC17E")]
public interface IFFMPEGSourceSettings { /* ... */ }

// IID de Interfaz IVFRegister (para licenciamiento)
[Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
public interface IVFRegister { /* ... */ }
```

**Paquetes NuGet Requeridos:**

- `VisioForge.DirectShowAPI` - Biblioteca wrapper de DirectShow
- `MediaFoundation.Net` - Wrapper de Media Foundation para renderizador EVR

**Ejemplo de Selección de Stream:**

```csharp
// Seleccionar streams de video o audio específicos en archivos multi-stream
var streamSelect = sourceFilter as IAMStreamSelect;
if (streamSelect != null)
{
    streamSelect.Count(out int streamCount);

    for (int i = 0; i < streamCount; i++)
    {
        streamSelect.Info(i, out var mediaType, out _, out _, out _, out var name, out _, out _);

        if (mediaType.majorType == MediaType.Video)
        {
            // Habilitar el primer stream de video
            streamSelect.Enable(i, AMStreamSelectEnableFlags.Enable);
            break;
        }
    }
}
```

### Ejemplo de Integración C++

```cpp
// Inicializar el filtro FFMPEG Source en C++ usando DirectShow
HRESULT InitializeFFMPEGSource()
{
    HRESULT hr = S_OK;
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pFFMPEGSource = NULL;
    IFileSourceFilter* pFileSource = NULL;
    
    // Inicializar COM
    CoInitialize(NULL);
    
    // Crear el administrador del grafo de filtros
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    
    // Crear el filtro FFMPEG Source
    hr = CoCreateInstance(CLSID_FFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pFFMPEGSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Agregar el filtro al grafo
    hr = pGraph->AddFilter(pFFMPEGSource, L"FFMPEG Source");
    if (FAILED(hr))
        goto cleanup;
    
    // Obtener la interfaz IFileSourceFilter
    hr = pFFMPEGSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Cargar el archivo de medios
    hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
    if (FAILED(hr))
        goto cleanup;
    
    // Renderizar los pines de salida del filtro FFMPEG Source
    hr = pGraph->Render(GetPin(pFFMPEGSource, PINDIR_OUTPUT, 0));
    
    // Obtener la interfaz de control de medios para control de reproducción
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Iniciar reproducción
        hr = pControl->Run();
        // ... manejar reproducción según sea necesario
    }
    
cleanup:
    // Liberar interfaces
    if (pControl) pControl->Release();
    if (pFileSource) pFileSource->Release();
    if (pFFMPEGSource) pFFMPEGSource->Release();
    if (pGraph) pGraph->Release();
    
    return hr;
}

// Función auxiliar para obtener pines de un filtro
IPin* GetPin(IBaseFilter* pFilter, PIN_DIRECTION PinDir, int nPin)
{
    IEnumPins* pEnum = NULL;
    IPin* pPin = NULL;
    
    if (pFilter)
    {
        pFilter->EnumPins(&pEnum);
        if (pEnum)
        {
            while (pEnum->Next(1, &pPin, NULL) == S_OK)
            {
                PIN_DIRECTION PinDirThis;
                pPin->QueryDirection(&PinDirThis);
                if (PinDir == PinDirThis)
                {
                    if (nPin == 0)
                        break;
                    nPin--;
                }
                pPin->Release();
                pPin = NULL;
            }
            pEnum->Release();
        }
    }
    
    return pPin;
}
```

## Integración con Filtros de Procesamiento

Mejore su pipeline de medios conectando el filtro FFMPEG Source con componentes de procesamiento adicionales:

- Aplique efectos de video en tiempo real y transformaciones
- Procese streams de audio para manipulación de sonido personalizada
- Implemente características especializadas de análisis de medios

Nuestro [paquete de Filtros de Procesamiento](https://www.visioforge.com/processing-filters-pack) ofrece capacidades adicionales, o puede integrarse con cualquier filtro compatible con DirectShow estándar.

## Especificaciones Técnicas

### Interfaces DirectShow Soportadas

El filtro implementa estas interfaces DirectShow estándar para máxima compatibilidad:

- **IAMStreamSelect**: Seleccione entre múltiples streams de video y audio
- **IAMStreamConfig**: Configure ajustes de video y audio
- **IFileSourceFilter**: Establezca nombre de archivo o URL de streaming
- **IMediaSeeking**: Implemente funcionalidad de búsqueda precisa
- **ISpecifyPropertyPages**: Acceda a la configuración a través de páginas de propiedades

## Historial de Versiones y Actualizaciones

### Versión 15.0

- Bibliotecas FFMPEG mejoradas con últimos códecs
- Agregado soporte de decodificación GPU para rendimiento mejorado
- Gestión de memoria optimizada para archivos grandes

### Versión 12.0

- Bibliotecas FFMPEG actualizadas
- Compatibilidad mejorada con Windows 10/11

### Versión 11.0

- Bibliotecas FFMPEG actualizadas
- Corregidos problemas de búsqueda con ciertos formatos de archivo

### Versión 10.0

- Bibliotecas FFMPEG actualizadas
- Agregado soporte para formatos de contenedor adicionales

### Versión 9.0

- Bibliotecas FFMPEG actualizadas
- Optimizaciones de rendimiento

### Versión 8.0

- Bibliotecas FFMPEG actualizadas
- Manejo de errores mejorado

### Versión 7.0

- Lanzamiento inicial como producto independiente
- Funcionalidad principal establecida

## Recursos Adicionales

- Explore nuestra [página del producto](https://www.visioforge.com/ffmpeg-source-directshow-filter) para especificaciones detalladas
- Vea nuestro [Acuerdo de Licencia de Usuario Final](../../eula.md) para detalles de licenciamiento
- Consulte nuestra documentación de desarrollador para escenarios de implementación avanzados
