---
title: Ejemplos de Código
description: Ejemplos de código para el Filtro de Fuente FFMPEG en C++, C# y VB.NET con gráficos DirectShow, aceleración de hardware y transmisión de red.
---

# Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos de código para usar el Filtro de Fuente FFMPEG en aplicaciones DirectShow. Se proporcionan ejemplos en C++, C# y VB.NET.

## Muestras de Trabajo Completas

**Repositorio Oficial de GitHub**: Todos los ejemplos mostrados en esta página están disponibles como proyectos completos y funcionales de Visual Studio en nuestro repositorio de muestras de GitHub:

🔗 **[Repositorio de Muestras de DirectShow](https://github.com/visioforge/directshow-samples)**

### Muestras del Filtro de Fuente FFMPEG

- **[Muestra en C#](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/cs)** - Reproductor multimedia con todas las funciones y capacidades del filtro
- **[Muestra en VB.NET](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/vbnet)** - Implementación en VB.NET
- **[Muestra en C++Builder](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/cpp_builder)** - Implementación en C++

Cada muestra incluye:
- Archivos de proyecto completos de Visual Studio/C++Builder
- Código funcional para reproducción, selección de flujo y configuración
- Ejemplos de aceleración de hardware
- Ejemplos de transmisión de red (RTSP/HLS)

---
## Prerrequisitos
### Proyectos C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "IFFMPEGSourceSettings.h"  // Del SDK
#pragma comment(lib, "strmiids.lib")
```
### Proyectos C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
```
**Paquetes NuGet**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Ejemplo 1: Reproducción Básica de Archivos

Reproducir un archivo multimedia local con la configuración predeterminada.

### Implementación en C++

```cpp
#include <dshow.h>
#include <windows.h>
#include "IFFMPEGSourceSettings.h"

// CLSID para Filtro de Fuente FFMPEG
DEFINE_GUID(CLSID_VFFFMPEGSource,
    0x1974d893, 0x83e4, 0x4f89, 0x99, 0x8, 0x79, 0x5c, 0x52, 0x4c, 0xc1, 0x7e);

HRESULT PlayFile(LPCWSTR filename, HWND hVideoWindow)
{
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IMediaEventEx* pEvent = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IFileSourceFilter* pFileSource = NULL;
    IVideoWindow* pVideoWindow = NULL;

    HRESULT hr = S_OK;

    // Crear Gráfico de Filtros
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Crear Filtro de Fuente FFMPEG
    hr = CoCreateInstance(CLSID_VFFFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) goto cleanup;

    // Agregar filtro al gráfico
    hr = pGraph->AddFilter(pSourceFilter, L"FFMPEG Source");
    if (FAILED(hr)) goto cleanup;

    // Cargar archivo
    hr = pSourceFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr)) goto cleanup;

    hr = pFileSource->Load(filename, NULL);
    if (FAILED(hr)) goto cleanup;

    // Renderizar flujos automáticamente
    hr = pGraph->QueryInterface(IID_IGraphBuilder, (void**)&pGraph);

    ICaptureGraphBuilder2* pBuild = NULL;
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (SUCCEEDED(hr))
    {
        hr = pBuild->SetFiltergraph(pGraph);

        // Renderizar flujo de video
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pSourceFilter, NULL, NULL);

        // Renderizar flujo de audio
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pSourceFilter, NULL, NULL);

        pBuild->Release();
    }

    // Establecer ventana de video
    hr = pGraph->QueryInterface(IID_IVideoWindow, (void**)&pVideoWindow);
    if (SUCCEEDED(hr))
    {
        pVideoWindow->put_Owner((OAHWND)hVideoWindow);
        pVideoWindow->put_WindowStyle(WS_CHILD | WS_CLIPSIBLINGS);

        RECT rc;
        GetClientRect(hVideoWindow, &rc);
        pVideoWindow->SetWindowPosition(0, 0, rc.right, rc.bottom);
    }

    // Obtener interfaz de control
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (FAILED(hr)) goto cleanup;

    // Ejecutar el gráfico
    hr = pControl->Run();

cleanup:
    if (pFileSource) pFileSource->Release();
    if (pVideoWindow) pVideoWindow->Release();
    if (pControl) pControl->Release();
    if (pSourceFilter) pSourceFilter->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

### Implementación en C#

```csharp
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class FFMPEGSourceBasicExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IVideoWindow videoWindow;
    private IBaseFilter sourceFilter;

    public void PlayFile(string filename, IntPtr videoWindowHandle)
    {
        try
        {
            // Crear gráfico de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;

            // Crear y agregar filtro de Fuente FFMPEG
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source");

            // Cargar archivo
            var fileSource = sourceFilter as IFileSourceFilter;
            int hr = fileSource.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Renderizar flujos
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Renderizar video
            hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
            // Renderizar audio
            hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

            // Establecer ventana de video
            videoWindow.put_Owner(videoWindowHandle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
            videoWindow.put_Visible(OABool.True);

            // Ejecutar gráfico
            mediaControl.Run();

            Marshal.ReleaseComObject(captureGraph);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error de Reproducción");
        }
    }

    public void Stop()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        if (videoWindow != null)
        {
            videoWindow.put_Visible(OABool.False);
            videoWindow.put_Owner(IntPtr.Zero);
        }

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (sourceFilter != null) Marshal.ReleaseComObject(sourceFilter);
        if (videoWindow != null) Marshal.ReleaseComObject(videoWindow);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}
```

### Implementación en VB.NET

```vbnet
Imports System.Runtime.InteropServices
Imports VisioForge.DirectShowAPI
Imports VisioForge.DirectShowLib

Public Class FFMPEGSourceBasicExample
    Private filterGraph As IFilterGraph2
    Private mediaControl As IMediaControl
    Private videoWindow As IVideoWindow
    Private sourceFilter As IBaseFilter

    Public Sub PlayFile(filename As String, videoWindowHandle As IntPtr)
        Try
            ' Crear gráfico de filtros
            filterGraph = DirectCast(New FilterGraph(), IFilterGraph2)
            mediaControl = DirectCast(filterGraph, IMediaControl)
            videoWindow = DirectCast(filterGraph, IVideoWindow)

            ' Crear y agregar filtro de Fuente FFMPEG
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source")

            ' Cargar archivo
            Dim fileSource = DirectCast(sourceFilter, IFileSourceFilter)
            Dim hr As Integer = fileSource.Load(filename, Nothing)
            DsError.ThrowExceptionForHR(hr)

            ' Renderizar flujos
            Dim captureGraph As ICaptureGraphBuilder2 = DirectCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
            hr = captureGraph.SetFiltergraph(filterGraph)
            DsError.ThrowExceptionForHR(hr)

            ' Renderizar video y audio
            captureGraph.RenderStream(Nothing, MediaType.Video, sourceFilter, Nothing, Nothing)
            captureGraph.RenderStream(Nothing, MediaType.Audio, sourceFilter, Nothing, Nothing)

            ' Establecer ventana de video
            videoWindow.put_Owner(videoWindowHandle)
            videoWindow.put_WindowStyle(WindowStyle.Child Or WindowStyle.ClipSiblings)
            videoWindow.put_Visible(OABool.True)

            ' Ejecutar gráfico
            mediaControl.Run()

            Marshal.ReleaseComObject(captureGraph)
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error de Reproducción")
        End Try
    End Sub

    Public Sub [Stop]()
        If mediaControl IsNot Nothing Then
            mediaControl.Stop()
        End If

        If videoWindow IsNot Nothing Then
            videoWindow.put_Visible(OABool.False)
            videoWindow.put_Owner(IntPtr.Zero)
        End If

        FilterGraphTools.RemoveAllFilters(filterGraph)

        If sourceFilter IsNot Nothing Then Marshal.ReleaseComObject(sourceFilter)
        If videoWindow IsNot Nothing Then Marshal.ReleaseComObject(videoWindow)
        If mediaControl IsNot Nothing Then Marshal.ReleaseComObject(mediaControl)
        If filterGraph IsNot Nothing Then Marshal.ReleaseComObject(filterGraph)
    End Sub
End Class
```

---
## Ejemplo 2: Aceleración de Hardware
Habilitar decodificación GPU para mejor rendimiento.
### C++ con Aceleración de Hardware
```cpp
HRESULT PlayFileWithGPU(LPCWSTR filename)
{
    IBaseFilter* pSourceFilter = NULL;
    IFFMPEGSourceSettings* pSettings = NULL;
    // Crear filtro de fuente
    HRESULT hr = CoCreateInstance(CLSID_VFFFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) return hr;
    // Obtener interfaz de configuración
    hr = pSourceFilter->QueryInterface(IID_IFFMPEGSourceSettings, (void**)&pSettings);
    if (FAILED(hr))
    {
        pSourceFilter->Release();
        return hr;
    }
    // Habilitar aceleración de hardware (NVDEC/QuickSync/DXVA)
    hr = pSettings->SetHWAccelerationEnabled(TRUE);
    if (FAILED(hr))
    {
        pSettings->Release();
        pSourceFilter->Release();
        return hr;
    }
    // Cargar archivo
    IFileSourceFilter* pFileSource = NULL;
    hr = pSourceFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (SUCCEEDED(hr))
    {
        hr = pFileSource->Load(filename, NULL);
        pFileSource->Release();
    }
    // Continuar construyendo gráfico...
    // (Agregar al gráfico, renderizar flujos, etc.)
    pSettings->Release();
    pSourceFilter->Release();
    return hr;
}
```
### C# con Aceleración de Hardware
```csharp
public void PlayFileWithHardwareAcceleration(string filename, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    // Crear filtro de fuente
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    // Configurar aceleración de hardware
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Habilitar decodificación GPU
        int hr = settings.SetHWAccelerationEnabled(true);
        DsError.ThrowExceptionForHR(hr);
    }
    // Cargar archivo
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Construir y ejecutar gráfico...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Ejemplo 3: Transmisión de Red (RTSP/HLS)

Transmitir video desde fuentes de red.

### Transmisión RTSP en C#

```csharp
public void PlayRTSPStream(string rtspUrl, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    // Configurar para transmisión de red
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Establecer modo de búfer para flujos de red
        settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);

        // Establecer tiempo de espera de conexión (en segundos)
        settings.SetLoadTimeOut(30);

        // Habilitar decodificación de hardware para rendimiento
        settings.SetHWAccelerationEnabled(true);
    }

    // Cargar flujo RTSP
    // Ejemplo: "rtsp://camera.example.com:554/stream"
    var fileSource = sourceFilter as IFileSourceFilter;
    int hr = fileSource.Load(rtspUrl, null);
    DsError.ThrowExceptionForHR(hr);

    // Construir gráfico
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    // Configurar ventana de video
    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
    videoWindow.put_Visible(OABool.True);

    // Ejecutar
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

### Transmisión HLS en C#

```csharp
public void PlayHLSStream(string hlsUrl, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Los flujos HLS se benefician del búfer
        settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);

        // Tiempo de espera más largo para carga de lista de reproducción HLS
        settings.SetLoadTimeOut(60);

        // Aceleración de hardware
        settings.SetHWAccelerationEnabled(true);
    }

    // Cargar flujo HLS
    // Ejemplo: "https://example.com/stream/playlist.m3u8"
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(hlsUrl, null);

    // Construir y ejecutar gráfico (igual que ejemplo RTSP)
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Ejemplo 4: Opciones Personalizadas de FFmpeg
Pasar opciones personalizadas de demuxer/decodificador FFmpeg.
### C# con Opciones Personalizadas
```csharp
public void PlayWithCustomOptions(string filename, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Establecer opciones personalizadas de FFmpeg
        // Formato: "clave=valor" para cada opción
        // Ejemplo 1: Establecer tamaño de búfer para flujos de red
        settings.SetCustomOption("buffer_size", "1024000");
        // Ejemplo 2: Habilitar modo de baja latencia
        settings.SetCustomOption("fflags", "nobuffer");
        // Ejemplo 3: Establecer duración del analizador (microsegundos)
        settings.SetCustomOption("analyzeduration", "1000000");
        // Ejemplo 4: Establecer tamaño de sonda
        settings.SetCustomOption("probesize", "5000000");
        // Ejemplo 5: Protocolo de transporte RTSP
        settings.SetCustomOption("rtsp_transport", "tcp");
        // Ejemplo 6: Establecer tiempo de espera (microsegundos)
        settings.SetCustomOption("timeout", "5000000");
    }
    // Cargar archivo
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Construir gráfico...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
### Opciones Comunes de FFmpeg
```csharp
// Opciones de transmisión de red
settings.SetCustomOption("rtsp_transport", "tcp");        // Usar TCP para RTSP
settings.SetCustomOption("rtsp_flags", "prefer_tcp");     // Preferir TCP sobre UDP
settings.SetCustomOption("timeout", "10000000");          // Tiempo de espera de 10 segundos
settings.SetCustomOption("stimeout", "5000000");          // Tiempo de espera de socket de 5 segundos
// Opciones de búfer y sondeo
settings.SetCustomOption("buffer_size", "2097152");       // Búfer de 2MB
settings.SetCustomOption("analyzeduration", "2000000");   // Análisis de 2 segundos
settings.SetCustomOption("probesize", "10000000");        // Tamaño de sonda de 10MB
// Opciones de baja latencia
settings.SetCustomOption("fflags", "nobuffer");           // Deshabilitar búfer
settings.SetCustomOption("flags", "low_delay");           // Bandera de bajo retardo
settings.SetCustomOption("framedrop", "1");               // Permitir caída de cuadros
// Opciones HTTP
settings.SetCustomOption("user_agent", "MiApp/1.0");     // Agente de usuario personalizado
settings.SetCustomOption("headers", "Authorization: Bearer token");
// Borrar todas las opciones personalizadas
settings.ClearCustomOptions();
```
---

## Ejemplo 5: Configuración del Modo de Búfer

Controlar el comportamiento del búfer para diferentes escenarios.

### Ejemplos de Búfer en C#

```csharp
public enum BufferingScenario
{
    LocalFile,
    NetworkStream,
    LowLatency
}

public void PlayWithBuffering(string source, BufferingScenario scenario, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        switch (scenario)
        {
            case BufferingScenario.LocalFile:
                // Modo automático - dejar que el filtro decida
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);
                break;

            case BufferingScenario.NetworkStream:
                // Habilitar búfer para reproducción fluida
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);
                settings.SetLoadTimeOut(30);  // Tiempo de espera de 30 segundos
                break;

            case BufferingScenario.LowLatency:
                // Deshabilitar búfer para latencia mínima
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_OFF);
                settings.SetCustomOption("fflags", "nobuffer");
                settings.SetCustomOption("flags", "low_delay");
                break;
        }
    }

    // Cargar fuente
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(source, null);

    // Construir y ejecutar gráfico...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Ejemplo 6: Selección de Múltiples Flujos
Seleccionar pistas de video y audio específicas.
### Selección de Flujo en C#
```csharp
public void PlayWithStreamSelection(string filename, int videoStreamIndex, int audioStreamIndex, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    // Cargar archivo primero
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Obtener flujos disponibles
    var streamSelect = sourceFilter as IAMStreamSelect;
    if (streamSelect != null)
    {
        streamSelect.Count(out int streamCount);
        List<int> videoStreams = new List<int>();
        List<int> audioStreams = new List<int>();
        // Enumerar flujos
        for (int i = 0; i < streamCount; i++)
        {
            streamSelect.Info(i, out AMMediaType mt, out _, out _, out _,
                             out string name, out _, out _);
            if (mt.majorType == MediaType.Video)
            {
                videoStreams.Add(i);
                Console.WriteLine($"Video Stream {videoStreams.Count - 1}: {name}");
            }
            else if (mt.majorType == MediaType.Audio)
            {
                audioStreams.Add(i);
                Console.WriteLine($"Audio Stream {audioStreams.Count - 1}: {name}");
            }
            DsUtils.FreeAMMediaType(mt);
        }
        // Habilitar flujos seleccionados
        if (videoStreamIndex >= 0 && videoStreamIndex < videoStreams.Count)
        {
            streamSelect.Enable(videoStreams[videoStreamIndex],
                               AMStreamSelectEnable.Enable);
        }
        if (audioStreamIndex >= 0 && audioStreamIndex < audioStreams.Count)
        {
            streamSelect.Enable(audioStreams[audioStreamIndex],
                               AMStreamSelectEnable.Enable);
        }
    }
    // Construir gráfico
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Ejemplo 7: Devolución de Llamada de Datos de Video/Audio

Capturar cuadros de video y audio sin procesar.

### Implementación de Devolución de Llamada de Datos en C#

```csharp
// Delegado de devolución de llamada
public delegate void DataCallbackDelegate(
    int streamType,  // 0 = video, 1 = audio
    IntPtr buffer,
    int bufferSize,
    long timestamp);

public class FFMPEGDataCallbackExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter sourceFilter;
    private DataCallbackDelegate dataCallback;

    public void PlayWithCallback(string filename, DataCallbackDelegate callback)
    {
        this.dataCallback = callback;

        filterGraph = (IFilterGraph2)new FilterGraph();

        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFFFMPEGSource,
            "FFMPEG Source");

        var settings = sourceFilter as IFFMPEGSourceSettings;
        if (settings != null)
        {
            // Establecer devolución de llamada de datos
            settings.SetDataCallback(OnDataReceived);
        }

        // Cargar archivo
        var fileSource = sourceFilter as IFileSourceFilter;
        fileSource.Load(filename, null);

        // Construir gráfico (sin renderizadores si solo desea devoluciones de llamada)
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Opción 1: Renderizar normalmente + obtener devoluciones de llamada
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        // Opción 2: Sin renderizadores - solo devolución de llamada
        // (No llamar a RenderStream)

        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }

    private void OnDataReceived(int streamType, IntPtr buffer, int bufferSize, long timestamp)
    {
        // streamType: 0 = video, 1 = audio

        if (streamType == 0)
        {
            // Cuadro de video recibido
            // El búfer contiene datos de video sin procesar (el formato depende del códec)
            byte[] videoData = new byte[bufferSize];
            Marshal.Copy(buffer, videoData, 0, bufferSize);

            // Procesar datos de video...
            ProcessVideoFrame(videoData, timestamp);
        }
        else if (streamType == 1)
        {
            // Cuadro de audio recibido
            byte[] audioData = new byte[bufferSize];
            Marshal.Copy(buffer, audioData, 0, bufferSize);

            // Procesar datos de audio...
            ProcessAudioFrame(audioData, timestamp);
        }
    }

    private void ProcessVideoFrame(byte[] data, long timestamp)
    {
        // Procesamiento de video personalizado
        Console.WriteLine($"Video frame: {data.Length} bytes at {timestamp}ms");

        // Guardar en archivo, codificar, analizar, etc.
    }

    private void ProcessAudioFrame(byte[] data, long timestamp)
    {
        // Procesamiento de audio personalizado
        Console.WriteLine($"Audio frame: {data.Length} bytes at {timestamp}ms");
    }
}
```

---
## Ejemplo 8: Devolución de Llamada de Marca de Tiempo
Monitorear el tiempo de reproducción.
### Devolución de Llamada de Marca de Tiempo en C#
```csharp
public delegate void TimestampCallbackDelegate(long videoTimestamp, long audioTimestamp);
public void PlayWithTimestampCallback(string filename, TimestampCallbackDelegate callback)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Establecer devolución de llamada de marca de tiempo
        settings.SetTimestampCallback((videoTs, audioTs) =>
        {
            // Llamado periódicamente con marcas de tiempo actuales
            callback(videoTs, audioTs);
            // Actualizar UI, sincronizar sistemas externos, etc.
            Console.WriteLine($"Video: {videoTs}ms, Audio: {audioTs}ms");
        });
    }
    // Cargar y reproducir archivo...
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Ejemplo 9: Activación de Licencia

Activar licencia comprada.

### Activación de Licencia en C#

```csharp
public void PlayWithLicense(string filename, string licenseKey, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    // Activar licencia
    var registration = sourceFilter as IVFRegister;
    if (registration != null)
    {
        int hr = registration.SetLicenseKey(licenseKey);
        if (hr != 0)
        {
            throw new Exception("Activación de licencia fallida");
        }
    }

    // Configurar y reproducir...
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        settings.SetHWAccelerationEnabled(true);
    }

    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);

    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Ejemplo 10: Reproductor Multimedia Completo
Reproductor multimedia con todas las funciones.
### Ejemplo Completo en C#
```csharp
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class FFMPEGMediaPlayer : IDisposable
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IMediaSeeking mediaSeeking;
    private IVideoWindow videoWindow;
    private IMediaEventEx mediaEventEx;
    private IBaseFilter sourceFilter;
    private const int WM_GRAPHNOTIFY = 0x8000 + 1;
    public event EventHandler PlaybackComplete;
    public void Initialize(IntPtr windowHandle, IntPtr notifyHandle)
    {
        // Crear gráfico de filtros
        filterGraph = (IFilterGraph2)new FilterGraph();
        captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        mediaControl = (IMediaControl)filterGraph;
        mediaSeeking = (IMediaSeeking)filterGraph;
        videoWindow = (IVideoWindow)filterGraph;
        mediaEventEx = (IMediaEventEx)filterGraph;
        // Configurar notificaciones de eventos
        int hr = mediaEventEx.SetNotifyWindow(notifyHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
        DsError.ThrowExceptionForHR(hr);
        // Adjuntar gráfico de captura
        hr = captureGraph.SetFiltergraph(filterGraph);
        DsError.ThrowExceptionForHR(hr);
    }
    public void LoadFile(string filename, bool enableGPU, bool enableBuffering, string licenseKey = null)
    {
        // Crear filtro de fuente
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFFFMPEGSource,
            "FFMPEG Source");
        // Registrar licencia si se proporciona
        if (!string.IsNullOrEmpty(licenseKey))
        {
            var registration = sourceFilter as IVFRegister;
            registration?.SetLicenseKey(licenseKey);
        }
        // Configurar filtro
        var settings = sourceFilter as IFFMPEGSourceSettings;
        if (settings != null)
        {
            settings.SetHWAccelerationEnabled(enableGPU);
            if (enableBuffering)
            {
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);
            }
            else
            {
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);
            }
            settings.SetLoadTimeOut(30);
        }
        // Cargar archivo
        var fileSource = sourceFilter as IFileSourceFilter;
        int hr = fileSource.Load(filename, null);
        DsError.ThrowExceptionForHR(hr);
        // Renderizar flujos
        hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    }
    public void SetVideoWindow(IntPtr handle, int width, int height)
    {
        if (videoWindow != null)
        {
            videoWindow.put_Owner(handle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
            videoWindow.SetWindowPosition(0, 0, width, height);
            videoWindow.put_Visible(OABool.True);
        }
    }
    public void Play()
    {
        mediaControl?.Run();
    }
    public void Pause()
    {
        mediaControl?.Pause();
    }
    public void Stop()
    {
        mediaControl?.Stop();
    }
    public void Seek(long timeInSeconds)
    {
        if (mediaSeeking != null)
        {
            long duration;
            mediaSeeking.GetDuration(out duration);
            long seekPos = timeInSeconds * 10000000; // Convertir a unidades de 100-nanosegundos
            if (seekPos <= duration)
            {
                mediaSeeking.SetPositions(ref seekPos, AMSeekingSeekingFlags.AbsolutePositioning,
                                         IntPtr.Zero, AMSeekingSeekingFlags.NoPositioning);
            }
        }
    }
    public long GetPosition()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetCurrentPosition(out long position);
            return position / 10000000; // Convertir a segundos
        }
        return 0;
    }
    public long GetDuration()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetDuration(out long duration);
            return duration / 10000000; // Convertir a segundos
        }
        return 0;
    }
    public void HandleGraphEvent()
    {
        if (mediaEventEx != null)
        {
            while (mediaEventEx.GetEvent(out EventCode eventCode, out IntPtr param1,
                                          out IntPtr param2, 0) == 0)
            {
                mediaEventEx.FreeEventParams(eventCode, param1, param2);
                if (eventCode == EventCode.Complete)
                {
                    PlaybackComplete?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
    public void Dispose()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }
        if (mediaEventEx != null)
        {
            mediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
        }
        if (videoWindow != null)
        {
            videoWindow.put_Visible(OABool.False);
            videoWindow.put_Owner(IntPtr.Zero);
        }
        FilterGraphTools.RemoveAllFilters(filterGraph);
        if (sourceFilter != null) Marshal.ReleaseComObject(sourceFilter);
        if (videoWindow != null) Marshal.ReleaseComObject(videoWindow);
        if (mediaSeeking != null) Marshal.ReleaseComObject(mediaSeeking);
        if (mediaEventEx != null) Marshal.ReleaseComObject(mediaEventEx);
        if (captureGraph != null) Marshal.ReleaseComObject(captureGraph);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}
```
### Ejemplo de Uso
```csharp
public partial class MainForm : Form
{
    private FFMPEGMediaPlayer player;
    public MainForm()
    {
        InitializeComponent();
        player = new FFMPEGMediaPlayer();
    }
    private void btnLoad_Click(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = "Archivos de Video|*.mp4;*.mkv;*.avi;*.mov|Todos los Archivos|*.*";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            player.Initialize(panelVideo.Handle, this.Handle);
            player.LoadFile(dlg.FileName, enableGPU: true, enableBuffering: true);
            player.SetVideoWindow(panelVideo.Handle, panelVideo.Width, panelVideo.Height);
            player.PlaybackComplete += Player_PlaybackComplete;
            player.Play();
        }
    }
    private void Player_PlaybackComplete(object sender, EventArgs e)
    {
        MessageBox.Show("Reproducción completada");
    }
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x8000 + 1) // WM_GRAPHNOTIFY
        {
            player?.HandleGraphEvent();
        }
        base.WndProc(ref m);
    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        player?.Dispose();
    }
}
```
---

## Solución de Problemas

### Problema: Error "Clase no registrada"

**Solución**: Asegúrese de que el filtro esté registrado:

```bash
regsvr32 VisioForge_FFMPEG_Source_x64.ax
```

### Problema: La Aceleración de Hardware No Funciona

**Solución**: Verifique el soporte de GPU y los controladores:

```csharp
var settings = sourceFilter as IFFMPEGSourceSettings;
bool isEnabled;
settings.GetHWAccelerationEnabled(out isEnabled);
Console.WriteLine($"HW Acceleration: {isEnabled}");
```

### Problema: Falla la Conexión de Transmisión de Red

**Solución**: Aumente el tiempo de espera y use opciones personalizadas:

```csharp
settings.SetLoadTimeOut(60);
settings.SetCustomOption("timeout", "30000000");  // 30 segundos
settings.SetCustomOption("rtsp_transport", "tcp");
```

---
## Ver También
### Documentación
- [Referencia de Interfaz](interface-reference.md) - Referencia completa de API
- [Guía de Despliegue](../deployment/index.md) - Despliegue de filtros
### Muestras de Código
- **[Repositorio de Muestras de GitHub](https://github.com/visioforge/directshow-samples)** - Ejemplos completos y funcionales
- **[Proyecto de Muestra en C#](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/cs)** - Implementación completa en C#
- **[Proyecto de Muestra en VB.NET](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/vbnet)** - Implementación en VB.NET
- **[Proyecto de Muestra en C++](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/cpp_builder)** - Implementación en C++Builder
### Recursos Externos
- [Documentación de FFmpeg](https://ffmpeg.org/documentation.html)
- [Guía de Programación DirectShow](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow)
### Soporte
- [Soporte Técnico](https://support.visioforge.com) - Obtenga ayuda del equipo de VisioForge
- [Comunidad de Discord](https://discord.com/invite/yvXUG56WCH) - Únase a nuestra comunidad
