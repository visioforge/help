---
title: Filtro de Fuente VLC - Ejemplos de Código
description: Ejemplos de código para el Filtro de Fuente VLC con múltiples pistas de audio, subtítulos, video 360° y parámetros personalizados de VLC en DirectShow.
---

# Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos de código para usar el Filtro de Fuente VLC en aplicaciones DirectShow. El Filtro de Fuente VLC admite múltiples pistas de audio, subtítulos, video 360° y opciones de línea de comandos personalizadas de VLC.

---
## Requisitos Previos
### Proyectos C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "IVlcSrc.h"      // Del SDK
#include "IVlcSrc2.h"     // Para parámetros personalizados
#include "IVlcSrc3.h"     // Para anulación de velocidad de fotogramas
#pragma comment(lib, "strmiids.lib")
```
### Proyectos C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
using System.Text;
```
**Paquetes NuGet**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Ejemplo 1: Reproducción Básica de Archivos

Reproducir un archivo multimedia local con el Filtro de Fuente VLC.

### Implementación en C++

```cpp
#include <dshow.h>
#include "IVlcSrc.h"

// CLSID para el Filtro de Fuente VLC
DEFINE_GUID(CLSID_VFVLCSource,
    0x9f73dcd4, 0x2fc8, 0x4e89, 0x8f, 0xc3, 0x2d, 0xf1, 0x69, 0x3c, 0xa0, 0x3e);

HRESULT PlayVLCFile(LPCWSTR filename, HWND hVideoWindow)
{
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IVlcSrc* pVlcSrc = NULL;

    HRESULT hr = S_OK;

    // Crear Grafo de Filtros
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Crear Filtro de Fuente VLC
    hr = CoCreateInstance(CLSID_VFVLCSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) goto cleanup;

    // Añadir filtro al grafo
    hr = pGraph->AddFilter(pSourceFilter, L"VLC Source");
    if (FAILED(hr)) goto cleanup;

    // Obtener interfaz VLC
    hr = pSourceFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
    if (FAILED(hr)) goto cleanup;

    // Cargar archivo
    hr = pVlcSrc->SetFile((WCHAR*)filename);
    if (FAILED(hr)) goto cleanup;

    // Construir y renderizar grafo
    ICaptureGraphBuilder2* pBuild = NULL;
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (SUCCEEDED(hr))
    {
        hr = pBuild->SetFiltergraph(pGraph);

        // Renderizar video y audio
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pSourceFilter, NULL, NULL);
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pSourceFilter, NULL, NULL);

        pBuild->Release();
    }

    // Ejecutar grafo
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        hr = pControl->Run();
    }

cleanup:
    if (pVlcSrc) pVlcSrc->Release();
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
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class VLCSourceBasicExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IVideoWindow videoWindow;
    private IBaseFilter sourceFilter;

    public void PlayFile(string filename, IntPtr videoWindowHandle)
    {
        try
        {
            // Crear grafo de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;

            // Crear y añadir filtro de Fuente VLC
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source");

            // Cargar archivo usando interfaz IVlcSrc
            var vlcSrc = sourceFilter as IVlcSrc;
            if (vlcSrc != null)
            {
                int hr = vlcSrc.SetFile(filename);
                DsError.ThrowExceptionForHR(hr);
            }

            // Renderizar flujos
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            int result = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(result);

            captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
            captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

            // Establecer ventana de video
            videoWindow.put_Owner(videoWindowHandle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
            videoWindow.put_Visible(OABool.True);

            // Ejecutar grafo
            mediaControl.Run();

            Marshal.ReleaseComObject(captureGraph);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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

Public Class VLCSourceBasicExample
    Private filterGraph As IFilterGraph2
    Private mediaControl As IMediaControl
    Private videoWindow As IVideoWindow
    Private sourceFilter As IBaseFilter

    Public Sub PlayFile(filename As String, videoWindowHandle As IntPtr)
        Try
            ' Crear grafo de filtros
            filterGraph = DirectCast(New FilterGraph(), IFilterGraph2)
            mediaControl = DirectCast(filterGraph, IMediaControl)
            videoWindow = DirectCast(filterGraph, IVideoWindow)

            ' Crear y añadir filtro de Fuente VLC
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source")

            ' Cargar archivo
            Dim vlcSrc = DirectCast(sourceFilter, IVlcSrc)
            If vlcSrc IsNot Nothing Then
                Dim hr As Integer = vlcSrc.SetFile(filename)
                DsError.ThrowExceptionForHR(hr)
            End If

            ' Renderizar flujos
            Dim captureGraph As ICaptureGraphBuilder2 = DirectCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
            captureGraph.SetFiltergraph(filterGraph)

            captureGraph.RenderStream(Nothing, MediaType.Video, sourceFilter, Nothing, Nothing)
            captureGraph.RenderStream(Nothing, MediaType.Audio, sourceFilter, Nothing, Nothing)

            ' Establecer ventana de video
            videoWindow.put_Owner(videoWindowHandle)
            videoWindow.put_WindowStyle(WindowStyle.Child Or WindowStyle.ClipSiblings)
            videoWindow.put_Visible(OABool.True)

            ' Ejecutar grafo
            mediaControl.Run()

            Marshal.ReleaseComObject(captureGraph)
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
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
## Ejemplo 2: Selección de Pista de Audio
Listar y seleccionar pistas de audio de archivos con múltiples audios.
### Gestión de Pistas de Audio en C#
```csharp
public class VLCAudioTrackExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IBaseFilter sourceFilter;
    public void PlayWithAudioTrackSelection(string filename, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        mediaControl = (IMediaControl)filterGraph;
        // Crear filtro de Fuente VLC
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Cargar archivo
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
            // Obtener recuento de pistas de audio
            int audioCount = 0;
            vlcSrc.GetAudioTracksCount(out audioCount);
            Console.WriteLine($"Total de pistas de audio: {audioCount}");
            // Listar todas las pistas de audio
            for (int i = 0; i < audioCount; i++)
            {
                int trackId;
                StringBuilder trackName = new StringBuilder(256);
                vlcSrc.GetAudioTrackInfo(i, out trackId, trackName);
                Console.WriteLine($"Pista {i}: ID={trackId}, Nombre={trackName}");
            }
            // Obtener pista actualmente activa
            int currentTrackId;
            vlcSrc.GetAudioTrack(out currentTrackId);
            Console.WriteLine($"ID de pista actualmente activa: {currentTrackId}");
            // Seleccionar una pista específica (por ejemplo, índice de pista 1)
            if (audioCount > 1)
            {
                int desiredTrackId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetAudioTrackInfo(1, out desiredTrackId, name);
                vlcSrc.SetAudioTrack(desiredTrackId);
                Console.WriteLine($"Cambiado a pista: {name}");
            }
        }
        // Construir y ejecutar grafo
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
    // Método para cambiar pista de audio durante la reproducción
    public void SwitchAudioTrack(int trackIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            int trackId;
            StringBuilder trackName = new StringBuilder(256);
            vlcSrc.GetAudioTrackInfo(trackIndex, out trackId, trackName);
            vlcSrc.SetAudioTrack(trackId);
            Console.WriteLine($"Cambiado a pista de audio: {trackName}");
        }
    }
}
```
### Gestión de Pistas de Audio en C++
```cpp
void ListAndSelectAudioTracks(IVlcSrc* pVlcSrc)
{
    int audioCount = 0;
    pVlcSrc->GetAudioTracksCount(&audioCount);
    wprintf(L"Total de pistas de audio: %d\n", audioCount);
    // Listar todas las pistas
    for (int i = 0; i < audioCount; i++)
    {
        int trackId = 0;
        WCHAR trackName[256] = {0};
        pVlcSrc->GetAudioTrackInfo(i, &trackId, trackName);
        wprintf(L"Pista %d: ID=%d, Nombre=%s\n", i, trackId, trackName);
    }
    // Obtener pista actual
    int currentId = 0;
    pVlcSrc->GetAudioTrack(&currentId);
    wprintf(L"ID de pista actual: %d\n", currentId);
    // Seleccionar pista por índice (por ejemplo, pista 1)
    if (audioCount > 1)
    {
        int trackId = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetAudioTrackInfo(1, &trackId, name);
        pVlcSrc->SetAudioTrack(trackId);
        wprintf(L"Cambiado a pista: %s\n", name);
    }
}
```
---

## Ejemplo 3: Gestión de Subtítulos

Seleccionar y gestionar pistas de subtítulos.

### Gestión de Subtítulos en C#

```csharp
public class VLCSubtitleExample
{
    private IBaseFilter sourceFilter;

    public void ManageSubtitles(string filename)
    {
        // Asumir que el filtro ya está creado y añadido al grafo
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);

            // Obtener recuento de subtítulos
            int subtitleCount = 0;
            vlcSrc.GetSubtitlesCount(out subtitleCount);

            Console.WriteLine($"Total de pistas de subtítulos: {subtitleCount}");

            // Listar todos los subtítulos
            for (int i = 0; i < subtitleCount; i++)
            {
                int subtitleId;
                StringBuilder subtitleName = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(i, out subtitleId, subtitleName);

                Console.WriteLine($"Subtítulo {i}: ID={subtitleId}, Nombre={subtitleName}");
            }

            // Habilitar subtítulo (por ejemplo, índice de subtítulo 0)
            if (subtitleCount > 0)
            {
                int subtitleId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(0, out subtitleId, name);

                vlcSrc.SetSubtitle(subtitleId);
                Console.WriteLine($"Subtítulo habilitado: {name}");
            }

            // Deshabilitar subtítulos (usar ID -1)
            // vlcSrc.SetSubtitle(-1);
        }
    }

    // Método para cambiar subtítulos durante la reproducción
    public void SwitchSubtitle(int subtitleIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            if (subtitleIndex < 0)
            {
                // Deshabilitar subtítulos
                vlcSrc.SetSubtitle(-1);
                Console.WriteLine("Subtítulos deshabilitados");
            }
            else
            {
                int subtitleId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(subtitleIndex, out subtitleId, name);

                vlcSrc.SetSubtitle(subtitleId);
                Console.WriteLine($"Cambiado a subtítulo: {name}");
            }
        }
    }
}
```

### Gestión de Subtítulos en C++

```cpp
void ManageSubtitles(IVlcSrc* pVlcSrc)
{
    int subtitleCount = 0;
    pVlcSrc->GetSubtitlesCount(&subtitleCount);

    wprintf(L"Total de subtítulos: %d\n", subtitleCount);

    // Listar todos los subtítulos
    for (int i = 0; i < subtitleCount; i++)
    {
        int subtitleId = 0;
        WCHAR subtitleName[256] = {0};
        pVlcSrc->GetSubtitleInfo(i, &subtitleId, subtitleName);

        wprintf(L"Subtítulo %d: ID=%d, Nombre=%s\n", i, subtitleId, subtitleName);
    }

    // Habilitar primer subtítulo
    if (subtitleCount > 0)
    {
        int id = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetSubtitleInfo(0, &id, name);

        pVlcSrc->SetSubtitle(id);
        wprintf(L"Subtítulo habilitado: %s\n", name);
    }

    // Deshabilitar subtítulos
    // pVlcSrc->SetSubtitle(-1);
}
```

---
## Ejemplo 4: Opciones de Línea de Comandos Personalizadas de VLC
Pasar parámetros personalizados de VLC usando la interfaz IVlcSrc2.
### Parámetros Personalizados de VLC en C#
```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VisioForge.Core.Helpers;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VLCCustomOptionsExample
{
    public void PlayWithCustomVLCOptions(string filename, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Establecer opciones de línea de comandos personalizadas de VLC usando IVlcSrc2
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            // Crear lista de parámetros de línea de comandos de VLC
            var parameters = new List<string>();
            parameters.Add("--avcodec-hw=any");          // Habilitar decodificación por hardware
            parameters.Add("--network-caching=1000");    // 1 segundo de caché de red
            parameters.Add("--live-caching=300");        // 300ms para flujos en vivo
            parameters.Add("--file-caching=300");        // 300ms para archivos
            parameters.Add("--sout-mux-caching=2000");   // Caché de muxing de salida
            parameters.Add("--vout=direct3d11");         // Usar renderizador Direct3D11
            parameters.Add("--verbose=2");               // Nivel de registro
            // Convertir cadenas a matriz nativa UTF-8 IntPtr
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                // Llamar a SetCustomCommandLine con matriz IntPtr
                int hr = vlcSrc2.SetCustomCommandLine(array, parameters.Count);
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                // Liberar memoria no administrada asignada
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Cargar archivo usando IVlcSrc
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
        }
        // Construir y ejecutar grafo
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
### Opciones Comunes de VLC
Aquí hay combinaciones comunes de parámetros de VLC. Recuerde usar el patrón de marshaling adecuado mostrado arriba.
```csharp
// Opciones de transmisión de red
var networkOptions = new List<string>
{
    "--network-caching=3000",      // 3 segundos para flujos de red
    "--rtsp-tcp",                  // Forzar TCP para RTSP
    "--http-reconnect"             // Auto-reconectar flujos HTTP
};
// Opciones de decodificación por hardware
var hwDecodeOptions = new List<string>
{
    "--avcodec-hw=any"             // Auto-detectar hardware
    // Opciones alternativas:
    // "--avcodec-hw=dxva2"        // Usar DXVA2
    // "--avcodec-hw=d3d11va"      // Usar D3D11VA
    // "--avcodec-hw=nvdec"        // Usar NVIDIA NVDEC
};
// Opciones de baja latencia
var lowLatencyOptions = new List<string>
{
    "--network-caching=0",
    "--live-caching=0",
    "--file-caching=0",
    "--sout-mux-caching=0",
    "--clock-jitter=0",
    "--drop-late-frames",
    "--skip-frames"
};
// Opciones de video 360°
var video360Options = new List<string>
{
    "--video-filter=transform",
    "--transform-type=hflip",
    "--vout-filter=rotate"
};
// Opciones de subtítulos
var subtitleOptions = new List<string>
{
    "--sub-autodetect-file",       // Auto-detectar archivos de subtítulos
    "--sub-language=eng",          // Idioma de subtítulos preferido
    "--freetype-fontsize=20"       // Tamaño de fuente de subtítulos
};
// Opciones de audio
var audioOptions = new List<string>
{
    "--audio-desync=0",            // Sincronización de audio
    "--audiotrack-language=eng",   // Idioma de audio preferido
    "--audio-filter=normvol"       // Normalización de volumen
};
// Ejemplo completo con múltiples opciones
var completeOptions = new List<string>
{
    "--avcodec-hw=any",
    "--network-caching=1000",
    "--rtsp-tcp",
    "--sub-autodetect-file",
    "--verbose=1"
};
// Método auxiliar para aplicar parámetros VLC
private void ApplyVLCParameters(IVlcSrc2 vlcSrc2, List<string> parameters)
{
    if (vlcSrc2 == null || parameters == null || parameters.Count == 0)
        return;
    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }
    try
    {
        int hr = vlcSrc2.SetCustomCommandLine(array, parameters.Count);
        DsError.ThrowExceptionForHR(hr);
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```
---

## Ejemplo 5: Anulación de Velocidad de Fotogramas

Anular la velocidad de fotogramas de la fuente usando la interfaz IVlcSrc3.

### Anulación de Velocidad de Fotogramas en C#

```csharp
public class VLCFrameRateExample
{
    public void PlayWithCustomFrameRate(string filename, double fps, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();

        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");

        // Establecer velocidad de fotogramas personalizada usando IVlcSrc3
        var vlcSrc3 = sourceFilter as IVlcSrc3;
        if (vlcSrc3 != null)
        {
            // Anular velocidad de fotogramas (por ejemplo, 30.0, 25.0, 60.0)
            int hr = vlcSrc3.SetDefaultFrameRate(fps);
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine($"Velocidad de fotogramas establecida a: {fps} fps");
        }

        // Cargar archivo
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
        }

        // Construir y ejecutar grafo
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

### Anulación de Velocidad de Fotogramas en C++

```cpp
#include "IVlcSrc3.h"

void SetCustomFrameRate(IBaseFilter* pFilter, double fps)
{
    IVlcSrc3* pVlcSrc3 = nullptr;
    HRESULT hr = pFilter->QueryInterface(IID_IVlcSrc3, (void**)&pVlcSrc3);

    if (SUCCEEDED(hr))
    {
        hr = pVlcSrc3->SetDefaultFrameRate(fps);
        if (SUCCEEDED(hr))
        {
            wprintf(L"Velocidad de fotogramas establecida a: %.2f fps\n", fps);
        }

        pVlcSrc3->Release();
    }
}
```

---
## Ejemplo 6: Transmisión de Red (RTSP/HLS)
Transmitir desde fuentes de red.
### Transmisión de Red en C#
```csharp
public class VLCNetworkStreamingExample
{
    public void PlayRTSPStream(string rtspUrl, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Configurar para transmisión RTSP
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            var parameters = new List<string>
            {
                "--rtsp-tcp",                         // Usar transporte TCP
                "--network-caching=300",              // Baja latencia (300ms)
                "--rtsp-frame-buffer-size=500000",    // Tamaño del búfer de fotogramas
                "--drop-late-frames",                 // Descartar fotogramas tardíos
                "--skip-frames"                       // Saltar fotogramas si es necesario
            };
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                vlcSrc2.SetCustomCommandLine(array, parameters.Count);
            }
            finally
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Cargar flujo RTSP
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            // Ejemplo: "rtsp://camera.example.com:554/stream"
            vlcSrc.SetFile(rtspUrl);
        }
        // Construir y ejecutar grafo
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
    public void PlayHLSStream(string hlsUrl, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Configurar para transmisión HLS
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            var parameters = new List<string>
            {
                "--http-reconnect",                // Auto-reconectar
                "--network-caching=3000",          // Búfer de 3 segundos para HLS
                "--hls-segment-threads=3",         // Descarga de segmentos en paralelo
                "--avcodec-hw=any"                 // Decodificación por hardware
            };
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                vlcSrc2.SetCustomCommandLine(array, parameters.Count);
            }
            finally
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Cargar flujo HLS
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            // Ejemplo: "https://example.com/stream/playlist.m3u8"
            vlcSrc.SetFile(hlsUrl);
        }
        // Construir y ejecutar grafo
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
---

## Ejemplo 7: Activación de Licencia

Activar licencia comprada.

### Activación de Licencia en C#

```csharp
public void PlayWithLicense(string filename, string licenseKey, IntPtr videoWindowHandle)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    var sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVLCSource,
        "VLC Source");

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

    // Cargar y reproducir archivo
    var vlcSrc = sourceFilter as IVlcSrc;
    if (vlcSrc != null)
    {
        vlcSrc.SetFile(filename);
    }

    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    var videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Ejemplo 8: Reproductor Multi-Pista Completo
Reproductor multimedia con todas las funciones de la Fuente VLC.
### Ejemplo Completo en C#
```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VLCMediaPlayer : IDisposable
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
    public List<AudioTrackInfo> AudioTracks { get; private set; } = new List<AudioTrackInfo>();
    public List<SubtitleInfo> Subtitles { get; private set; } = new List<SubtitleInfo>();
    public class AudioTrackInfo
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SubtitleInfo
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public void Initialize(IntPtr windowHandle, IntPtr notifyHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        mediaControl = (IMediaControl)filterGraph;
        mediaSeeking = (IMediaSeeking)filterGraph;
        videoWindow = (IVideoWindow)filterGraph;
        mediaEventEx = (IMediaEventEx)filterGraph;
        int hr = mediaEventEx.SetNotifyWindow(notifyHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
        DsError.ThrowExceptionForHR(hr);
        hr = captureGraph.SetFiltergraph(filterGraph);
        DsError.ThrowExceptionForHR(hr);
    }
    public void LoadFile(string filename, string licenseKey = null,
                        string vlcOptions = null, double? frameRate = null)
    {
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Registrar licencia
        if (!string.IsNullOrEmpty(licenseKey))
        {
            var registration = sourceFilter as IVFRegister;
            registration?.SetLicenseKey(licenseKey);
        }
        // Establecer opciones personalizadas de VLC
        if (!string.IsNullOrEmpty(vlcOptions))
        {
            var vlcSrc2 = sourceFilter as IVlcSrc2;
            if (vlcSrc2 != null)
            {
                // Analizar opciones separadas por espacios en una lista
                var parameters = new List<string>(vlcOptions.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                var array = new IntPtr[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                {
                    array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
                }
                try
                {
                    vlcSrc2.SetCustomCommandLine(array, parameters.Count);
                }
                finally
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        Marshal.FreeHGlobal(array[i]);
                    }
                }
            }
        }
        // Establecer anulación de velocidad de fotogramas
        if (frameRate.HasValue)
        {
            var vlcSrc3 = sourceFilter as IVlcSrc3;
            vlcSrc3?.SetDefaultFrameRate(frameRate.Value);
        }
        // Cargar archivo
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            int hr = vlcSrc.SetFile(filename);
            DsError.ThrowExceptionForHR(hr);
            // Enumerar pistas de audio
            LoadAudioTracks(vlcSrc);
            // Enumerar subtítulos
            LoadSubtitles(vlcSrc);
        }
        // Construir grafo
        int result = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        result = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    }
    private void LoadAudioTracks(IVlcSrc vlcSrc)
    {
        AudioTracks.Clear();
        int count = 0;
        vlcSrc.GetAudioTracksCount(out count);
        for (int i = 0; i < count; i++)
        {
            int id;
            StringBuilder name = new StringBuilder(256);
            vlcSrc.GetAudioTrackInfo(i, out id, name);
            AudioTracks.Add(new AudioTrackInfo
            {
                Index = i,
                Id = id,
                Name = name.ToString()
            });
        }
    }
    private void LoadSubtitles(IVlcSrc vlcSrc)
    {
        Subtitles.Clear();
        int count = 0;
        vlcSrc.GetSubtitlesCount(out count);
        for (int i = 0; i < count; i++)
        {
            int id;
            StringBuilder name = new StringBuilder(256);
            vlcSrc.GetSubtitleInfo(i, out id, name);
            Subtitles.Add(new SubtitleInfo
            {
                Index = i,
                Id = id,
                Name = name.ToString()
            });
        }
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
    public void Play() => mediaControl?.Run();
    public void Pause() => mediaControl?.Pause();
    public void Stop() => mediaControl?.Stop();
    public void SelectAudioTrack(int trackIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null && trackIndex >= 0 && trackIndex < AudioTracks.Count)
        {
            vlcSrc.SetAudioTrack(AudioTracks[trackIndex].Id);
        }
    }
    public void SelectSubtitle(int subtitleIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            if (subtitleIndex < 0)
            {
                vlcSrc.SetSubtitle(-1); // Deshabilitar
            }
            else if (subtitleIndex < Subtitles.Count)
            {
                vlcSrc.SetSubtitle(Subtitles[subtitleIndex].Id);
            }
        }
    }
    public void Seek(long timeInSeconds)
    {
        if (mediaSeeking != null)
        {
            long seekPos = timeInSeconds * 10000000;
            mediaSeeking.SetPositions(ref seekPos, AMSeekingSeekingFlags.AbsolutePositioning,
                                     IntPtr.Zero, AMSeekingSeekingFlags.NoPositioning);
        }
    }
    public long GetPosition()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetCurrentPosition(out long position);
            return position / 10000000;
        }
        return 0;
    }
    public long GetDuration()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetDuration(out long duration);
            return duration / 10000000;
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
    private VLCMediaPlayer player;
    public MainForm()
    {
        InitializeComponent();
        player = new VLCMediaPlayer();
    }
    private void btnLoad_Click(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = "Archivos de Video|*.mp4;*.mkv;*.avi;*.mov|Todos los Archivos|*.*";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            player.Initialize(panelVideo.Handle, this.Handle);
            // Opciones personalizadas de VLC para decodificación por hardware
            string vlcOptions = "--avcodec-hw=any --network-caching=1000";
            player.LoadFile(dlg.FileName, vlcOptions: vlcOptions);
            player.SetVideoWindow(panelVideo.Handle, panelVideo.Width, panelVideo.Height);
            // Rellenar cuadro combinado de pistas de audio
            comboAudioTracks.Items.Clear();
            foreach (var track in player.AudioTracks)
            {
                comboAudioTracks.Items.Add(track.Name);
            }
            if (comboAudioTracks.Items.Count > 0)
            {
                comboAudioTracks.SelectedIndex = 0;
            }
            // Rellenar cuadro combinado de subtítulos
            comboSubtitles.Items.Clear();
            comboSubtitles.Items.Add("Ninguno");
            foreach (var subtitle in player.Subtitles)
            {
                comboSubtitles.Items.Add(subtitle.Name);
            }
            comboSubtitles.SelectedIndex = 0;
            player.Play();
        }
    }
    private void comboAudioTracks_SelectedIndexChanged(object sender, EventArgs e)
    {
        player.SelectAudioTrack(comboAudioTracks.SelectedIndex);
    }
    private void comboSubtitles_SelectedIndexChanged(object sender, EventArgs e)
    {
        player.SelectSubtitle(comboSubtitles.SelectedIndex - 1); // -1 debido al elemento "Ninguno"
    }
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x8000 + 1)
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

### Problema: Error "Class not registered"

**Solución**: Asegúrese de que el filtro de Fuente VLC esté registrado:
```bash
regsvr32 VisioForge_VLC_Source.ax
```

### Problema: El Recuento de Pistas de Audio/Subtítulos Devuelve 0

**Causa**: Las pistas aún no están disponibles (el grafo de filtros aún no se ha construido)

**Solución**: La información de audio y subtítulos solo está disponible después de cargar el archivo y construir el grafo. Llame a los métodos de enumeración de pistas después de `SetFile()` y construir el grafo.

```cpp
// Cargar archivo primero
pVlcSrc->SetFile(L"movie.mkv");

// Construir grafo de filtros
pGraph->RenderFile(L"movie.mkv", nullptr);

// AHORA consultar pistas
int count = 0;
pVlcSrc->GetAudioTracksCount(&count);
```

### Problema: Las Opciones Personalizadas de VLC No Funcionan

**Solución**: Asegúrese de que las opciones de VLC se establezcan antes de cargar el archivo, y use el marshaling correcto:

```csharp
// Orden y uso correctos:
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var parameters = new List<string>
    {
        "--network-caching=1000",
        "--rtsp-tcp"
    };

    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, parameters.Count);  // 1. Establecer opciones primero
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}

var vlcSrc = sourceFilter as IVlcSrc;
vlcSrc?.SetFile(filename);  // 2. Luego cargar archivo
// Construir y ejecutar grafo...
```

### Problema: El Flujo RTSP Tiene Alta Latencia

**Solución**: Configure VLC para un almacenamiento en búfer de red mínimo y use transporte TCP:

```cpp
// Ejemplo en C++
IVlcSrc2* pVlcSrc2 = nullptr;
hr = pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);
if (SUCCEEDED(hr))
{
    const char* params[] = {
        "--network-caching=50",
        "--live-caching=50",
        "--rtsp-tcp",
        "--no-audio-time-stretch"
    };
    
    // Convertir a cadenas anchas y luego a equivalentes IntPtr
    // (En C++, puede pasar cadenas ANSI/UTF-8 directamente en algunos casos,
    // pero para consistencia con la interfaz, use la conversión adecuada)
    
    pVlcSrc2->Release();
}
```

```csharp
// Ejemplo en C#
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var lowLatencyOptions = new List<string>
    {
        "--network-caching=50",
        "--live-caching=50",
        "--rtsp-tcp",
        "--no-audio-time-stretch"
    };

    var array = new IntPtr[lowLatencyOptions.Count];
    for (int i = 0; i < lowLatencyOptions.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(lowLatencyOptions[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, lowLatencyOptions.Count);
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

### Problema: La Aceleración por Hardware No Funciona

**Solución**: Especifique explícitamente el decodificador por hardware a usar:

```cpp
// Ejemplo en C++ - Probar decodificador por hardware explícito
IVlcSrc2* pVlcSrc2 = nullptr;
hr = pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);
if (SUCCEEDED(hr))
{
    const char* params[] = {
        "--avcodec-hw=d3d11va"  // Para Windows 8+
        // o "--avcodec-hw=dxva2" para Windows 7
    };
    
    // Se requiere conversión y llamada adecuadas
    
    pVlcSrc2->Release();
}
```

```csharp
// Ejemplo en C#
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var hwOptions = new List<string>
    {
        "--avcodec-hw=dxva2"  // o d3d11va, nvdec, any
    };

    var array = new IntPtr[hwOptions.Count];
    for (int i = 0; i < hwOptions.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(hwOptions[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, hwOptions.Count);
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

---
## Ver También
### Documentación
- [Referencia de Interfaz](interface-reference.md) - Referencia completa de la API
- [Guía de Despliegue](../deployment/index.md) - Despliegue de filtros
### Recursos Externos
- [Documentación de Línea de Comandos de VLC](https://www.videolan.org/doc/)
- [Guía de Programación DirectShow](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow)
