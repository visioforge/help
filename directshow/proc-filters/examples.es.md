---
title: Ejemplos de Código: Filtros de Procesamiento
description: Ejemplos de código para Efectos de Video, Mezclador de Video y filtros de Clave de Croma en C++, C# y VB.NET con integración DirectShow.
---

# Paquete de Filtros de Procesamiento - Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos de código para usar el Paquete de Filtros de Procesamiento, que incluye:

- **Efectos de Video** - Más de 35 efectos en tiempo real (texto, gráficos, ajustes de color, eliminación de ruido)
- **Mezclador de Video** - Mezcla de 2-16 fuentes con PIP, mezcla alfa, clave de croma
- **Clave de Croma** - Composición de pantalla verde/azul

---
## Prerrequisitos
### Proyectos C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "IVFEffects45.h"
#include "IVFVideoMixer.h"
#include "IVFChromaKey.h"
#pragma comment(lib, "strmiids.lib")
```
### Proyectos C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
using System.Drawing;
```
**Paquetes NuGet**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Ejemplos de Efectos de Video

### Ejemplo 1: Efecto de Video Básico

Aplicar un solo efecto de video a una fuente.

#### Implementación en C#

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class VideoEffectsBasicExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IBaseFilter sourceFilter;
    private IBaseFilter effectFilter;

    public void PlayWithEffect(string filename, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        mediaControl = (IMediaControl)filterGraph;

        // Agregar filtro de fuente (ej. Fuente de Archivo)
        filterGraph.AddSourceFilter(filename, "Source", out sourceFilter);

        // Agregar filtro de Efectos de Video
        effectFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        // Configurar efecto usando interfaz IVFEffects45
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Habilitar efecto Escala de Grises
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREYSCALE, 1);

            // Establecer parámetros del efecto (algunos efectos requieren parámetros)
            // effects.SetEffectParam(effectType, paramName, paramValue);
        }

        // Conectar filtros: Fuente → Efecto → Renderizador
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Renderizar a través del filtro de efecto
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, effectFilter, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        // Configurar ventana de video
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Ejecutar
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }

    public void Stop()
    {
        mediaControl?.Stop();

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (sourceFilter != null) Marshal.ReleaseComObject(sourceFilter);
        if (effectFilter != null) Marshal.ReleaseComObject(effectFilter);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}
```

#### Implementación en C++

```cpp
HRESULT ApplyVideoEffect(LPCWSTR filename)
{
    IGraphBuilder* pGraph = NULL;
    IBaseFilter* pSource = NULL;
    IBaseFilter* pEffect = NULL;
    IVFEffects45* pEffects = NULL;

    // Crear gráfico de filtros
    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Agregar fuente
    hr = pGraph->AddSourceFilter(filename, L"Source", &pSource);
    if (FAILED(hr)) goto cleanup;

    // Crear filtro de Efectos de Video
    hr = CoCreateInstance(CLSID_VFVideoEffects, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pEffect);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pEffect, L"Video Effects");
    if (FAILED(hr)) goto cleanup;

    // Configurar efecto
    hr = pEffect->QueryInterface(IID_IVFEffects45, (void**)&pEffects);
    if (SUCCEEDED(hr))
    {
        // Habilitar escala de grises
        pEffects->SetEffect(VF_VIDEO_EFFECT_GREYSCALE, 1);
        pEffects->Release();
    }

    // Conectar filtros y renderizar...
    // (Usar RenderStream o ConnectFilters)

cleanup:
    if (pEffect) pEffect->Release();
    if (pSource) pSource->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

---
### Ejemplo 2: Cadena de Múltiples Efectos
Aplicar múltiples efectos simultáneamente.
#### Múltiples Efectos en C#
```csharp
public class MultipleEffectsExample
{
    public void ApplyMultipleEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Habilitar múltiples efectos
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, 1);
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION, 1);
            // Establecer parámetros para cada efecto
            effects.SetEffectParam(
                VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS,
                "Value",
                50);  // Valor de brillo (0-100)
            effects.SetEffectParam(
                VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST,
                "Value",
                75);  // Valor de contraste (0-100)
            effects.SetEffectParam(
                VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION,
                "Value",
                120);  // Valor de saturación (0-200)
        }
    }
}
```
---

### Ejemplo 3: Superposición de Texto

Agregar superposición de logotipo de texto al video.

#### Superposición de Texto en C#

```csharp
public void ApplyTextOverlay(IBaseFilter effectFilter)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        // Habilitar efecto de logotipo de texto
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, 1);

        // Configurar parámetros de texto
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Text",
            "Mi Título de Video");

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "FontName",
            "Arial");

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "FontSize",
            36);

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Color",
            ColorTranslator.ToWin32(Color.White));

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "X",
            50);  // Posición X

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Y",
            50);  // Posición Y

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Transparent",
            0);  // Opaco

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Alpha",
            255);  // Totalmente opaco
    }
}
```

---
### Ejemplo 4: Superposición de Imagen
Agregar marca de agua gráfica o logotipo.
#### Superposición de Imagen en C#
```csharp
public void ApplyImageOverlay(IBaseFilter effectFilter, string imagePath)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        // Habilitar efecto de logotipo gráfico
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO, 1);
        // Establecer ruta de archivo de imagen
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "ImageFile",
            imagePath);
        // Establecer posición (0-100% de la pantalla)
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "X",
            10);  // 10% desde la izquierda
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Y",
            10);  // 10% desde arriba
        // Establecer tamaño (0-100% del original)
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Width",
            25);  // 25% del ancho de pantalla
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Height",
            25);  // 25% de la altura de pantalla
        // Establecer transparencia (0-255)
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Alpha",
            200);  // Semi-transparente
    }
}
```
---

### Ejemplo 5: Filtros de Eliminación de Ruido

Aplicar reducción de ruido para un video más limpio.

#### Ejemplos de Eliminación de Ruido en C#

```csharp
public enum DenoiseType
{
    CAST,
    Adaptive,
    Mosquito
}

public void ApplyDenoise(IBaseFilter effectFilter, DenoiseType type)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        switch (type)
        {
            case DenoiseType.CAST:
                // Eliminación de ruido CAST - bueno para ruido general
                effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_CAST, 1);
                break;

            case DenoiseType.Adaptive:
                // Eliminación de ruido adaptativa - se ajusta según el contenido
                effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_ADAPTIVE, 1);
                effects.SetEffectParam(
                    VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_ADAPTIVE,
                    "Strength",
                    5);  // Fuerza 1-10
                break;

            case DenoiseType.Mosquito:
                // Eliminación de ruido mosquito - reduce artefactos de compresión
                effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_MOSQUITO, 1);
                effects.SetEffectParam(
                    VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_MOSQUITO,
                    "Threshold",
                    30);  // Valor de umbral
                break;
        }
    }
}
```

---
### Ejemplo 6: Todos los Efectos Disponibles
Lista completa de los más de 35 efectos con configuración básica.
#### Referencia de Todos los Efectos en C#
```csharp
public class AllEffectsExample
{
    public void DemonstrateAllEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects == null) return;
        // Filtros de Color
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREYSCALE, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_INVERT, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_RED_FILTER, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREEN_FILTER, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BLUE_FILTER, 1);
        // Ajuste de Imagen
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, "Value", 50);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, "Value", 75);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION, "Value", 120);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_HUE, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_HUE, "Value", 45);
        // Transformaciones Espaciales
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_FLIP_HORIZONTAL, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_FLIP_VERTICAL, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_MIRROR, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_ROTATE, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_ROTATE, "Angle", 90);
        // Efectos Artísticos
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BLUR, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BLUR, "Radius", 5);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SHARPEN, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SHARPEN, "Amount", 2);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_EMBOSS, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_EDGE_DETECT, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_POSTERIZE, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_POSTERIZE, "Levels", 8);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SOLARIZE, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SOLARIZE, "Threshold", 128);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_MOSAIC, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_MOSAIC, "BlockSize", 10);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_MARBLE, 1);
        // Reducción de Ruido
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_CAST, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_ADAPTIVE, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_MOSQUITO, 1);
        // Desentrelazado
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DEINTERLACE_BLEND, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DEINTERLACE_TRIANGLE, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DEINTERLACE_CAVT, 1);
        // Superposiciones
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "Text", "Muestra");
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO, "ImageFile", "logo.png");
        // Para deshabilitar un efecto:
        // effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREYSCALE, 0);
    }
}
```
---

## Ejemplos de Mezclador de Video

### Ejemplo 7: Imagen en Imagen (PIP)

Mezclar dos fuentes de video con diseño PIP.

#### Imagen en Imagen en C#

```csharp
public class VideoMixerPIPExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter mixerFilter;

    public void CreatePIP(string mainVideoPath, string pipVideoPath, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();

        // Agregar fuente de video principal
        filterGraph.AddSourceFilter(mainVideoPath, "Main Source", out IBaseFilter mainSource);

        // Agregar fuente de video PIP
        filterGraph.AddSourceFilter(pipVideoPath, "PIP Source", out IBaseFilter pipSource);

        // Agregar filtro de Mezclador de Video
        mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");

        // Configurar mezclador
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Establecer tamaño de salida
            mixer.SetOutputParam("Width", 1920);
            mixer.SetOutputParam("Height", 1080);
            mixer.SetOutputParam("FrameRate", 30.0);

            // Configurar video principal (entrada 0) - pantalla completa
            mixer.SetInputParam(0, "X", 0);
            mixer.SetInputParam(0, "Y", 0);
            mixer.SetInputParam(0, "Width", 1920);
            mixer.SetInputParam(0, "Height", 1080);
            mixer.SetInputParam(0, "Alpha", 255);  // Totalmente opaco

            // Configurar video PIP (entrada 1) - esquina inferior derecha
            mixer.SetInputParam(1, "X", 1440);    // 1920 - 480 = 1440
            mixer.SetInputParam(1, "Y", 810);     // 1080 - 270 = 810
            mixer.SetInputParam(1, "Width", 480); // 25% ancho
            mixer.SetInputParam(1, "Height", 270); // 25% altura
            mixer.SetInputParam(1, "Alpha", 255);

            // Establecer orden Z (capas)
            mixer.SetInputOrder(new int[] { 0, 1 }); // Principal detrás, PIP encima
        }

        // Conectar filtros
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Conectar fuente principal a entrada 0 del mezclador
        captureGraph.RenderStream(null, MediaType.Video, mainSource, null, mixerFilter);

        // Conectar fuente PIP a entrada 1 del mezclador
        // Nota: Requiere conectar a pin de entrada específico
        IPin mixerInput1 = DsFindPin.ByDirection(mixerFilter, PinDirection.Input, 1);
        captureGraph.RenderStream(null, MediaType.Video, pipSource, null, null);
        // Conectar a mixerInput1 explícitamente...

        // Renderizar salida del mezclador
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Configurar ventana de video
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Ejecutar
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

---
### Ejemplo 8: Mezcla de Múltiples Fuentes (4 entradas)
Crear un diseño de cuadrícula 2x2 con 4 fuentes de video.
#### Diseño de Cuadrícula 2x2 en C#
```csharp
public class VideoMixerGridExample
{
    public void Create2x2Grid(string[] videoPaths, IntPtr videoWindowHandle)
    {
        if (videoPaths.Length != 4)
        {
            throw new ArgumentException("Requires exactly 4 video sources");
        }
        var filterGraph = (IFilterGraph2)new FilterGraph();
        // Agregar todos los filtros de fuente
        IBaseFilter[] sources = new IBaseFilter[4];
        for (int i = 0; i < 4; i++)
        {
            filterGraph.AddSourceFilter(videoPaths[i], $"Source {i}", out sources[i]);
        }
        // Agregar Mezclador de Video
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Establecer tamaño de salida
            mixer.SetOutputParam("Width", 1920);
            mixer.SetOutputParam("Height", 1080);
            mixer.SetOutputParam("FrameRate", 30.0);
            // Configurar diseño de cuadrícula 2x2
            int halfWidth = 960;   // 1920 / 2
            int halfHeight = 540;  // 1080 / 2
            // Arriba-izquierda (Entrada 0)
            mixer.SetInputParam(0, "X", 0);
            mixer.SetInputParam(0, "Y", 0);
            mixer.SetInputParam(0, "Width", halfWidth);
            mixer.SetInputParam(0, "Height", halfHeight);
            // Arriba-derecha (Entrada 1)
            mixer.SetInputParam(1, "X", halfWidth);
            mixer.SetInputParam(1, "Y", 0);
            mixer.SetInputParam(1, "Width", halfWidth);
            mixer.SetInputParam(1, "Height", halfHeight);
            // Abajo-izquierda (Entrada 2)
            mixer.SetInputParam(2, "X", 0);
            mixer.SetInputParam(2, "Y", halfHeight);
            mixer.SetInputParam(2, "Width", halfWidth);
            mixer.SetInputParam(2, "Height", halfHeight);
            // Abajo-derecha (Entrada 3)
            mixer.SetInputParam(3, "X", halfWidth);
            mixer.SetInputParam(3, "Y", halfHeight);
            mixer.SetInputParam(3, "Width", halfWidth);
            mixer.SetInputParam(3, "Height", halfHeight);
            // Todas las entradas totalmente opacas
            for (int i = 0; i < 4; i++)
            {
                mixer.SetInputParam(i, "Alpha", 255);
            }
            // Establecer orden Z (todos en el mismo nivel)
            mixer.SetInputOrder(new int[] { 0, 1, 2, 3 });
        }
        // Conectar fuentes al mezclador y renderizar...
        // (Similar al ejemplo PIP)
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
    }
}
```
---

### Ejemplo 9: Mezclador de Video con Clave de Croma

Mezclar fuentes con fondo transparente.

#### Mezclador con Clave de Croma en C#

```csharp
public void CreateMixerWithChromaKey(string backgroundPath, string foregroundPath)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Agregar fuentes
    filterGraph.AddSourceFilter(backgroundPath, "Background", out IBaseFilter bgSource);
    filterGraph.AddSourceFilter(foregroundPath, "Foreground", out IBaseFilter fgSource);

    // Agregar mezclador
    var mixerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVideoMixer,
        "Video Mixer");

    var mixer = mixerFilter as IVFVideoMixer;
    if (mixer != null)
    {
        // Configurar salida
        mixer.SetOutputParam("Width", 1920);
        mixer.SetOutputParam("Height", 1080);

        // Fondo (pantalla completa)
        mixer.SetInputParam(0, "X", 0);
        mixer.SetInputParam(0, "Y", 0);
        mixer.SetInputParam(0, "Width", 1920);
        mixer.SetInputParam(0, "Height", 1080);

        // Primer plano (centrado, más pequeño)
        mixer.SetInputParam(1, "X", 480);
        mixer.SetInputParam(1, "Y", 270);
        mixer.SetInputParam(1, "Width", 960);
        mixer.SetInputParam(1, "Height", 540);

        // Habilitar clave de croma para entrada de primer plano
        mixer.SetChromaSettings(
            inputIndex: 1,
            enabled: true,
            colorR: 0,      // Pantalla verde
            colorG: 255,
            colorB: 0,
            threshold: 50,  // Tolerancia
            blend: 10);     // Suavizado de bordes
    }

    // Conectar y ejecutar...
}
```

---
## Ejemplos de Clave de Croma
### Ejemplo 10: Efecto de Pantalla Verde
Filtro de clave de croma independiente para eliminación de pantalla verde.
#### Filtro de Clave de Croma en C#
```csharp
public class ChromaKeyExample
{
    public void ApplyGreenScreen(string videoPath, string backgroundImagePath, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        // Agregar fuente de video (con pantalla verde)
        filterGraph.AddSourceFilter(videoPath, "Source", out IBaseFilter sourceFilter);
        // Agregar filtro de Clave de Croma
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");
        // Configurar clave de croma
        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            // Establecer color clave (verde)
            chromaKey.chroma_put_color(
                red: 0,
                green: 255,
                blue: 0);
            // Establecer contraste/umbral
            chromaKey.chroma_put_contrast(
                contrast1: 50,  // Umbral inferior
                contrast2: 100); // Umbral superior
            // Establecer imagen de fondo (opcional)
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                chromaKey.chroma_put_image(backgroundImagePath);
            }
        }
        // Conectar filtros: Fuente → Clave de Croma → Renderizador
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, chromaFilter, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        // Configurar ventana de video
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        // Ejecutar
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
#### Clave de Croma en C++
```cpp
HRESULT ApplyChromaKey(IBaseFilter* pChromaFilter)
{
    IVFChromaKey* pChromaKey = NULL;
    HRESULT hr = pChromaFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChromaKey);
    if (FAILED(hr)) return hr;
    // Establecer color verde (RGB)
    hr = pChromaKey->chroma_put_color(0, 255, 0);
    if (FAILED(hr)) goto cleanup;
    // Establecer umbrales
    hr = pChromaKey->chroma_put_contrast(40, 90);
    if (FAILED(hr)) goto cleanup;
    // Opcional: Establecer imagen de fondo
    hr = pChromaKey->chroma_put_image(L"C:\\backgrounds\\studio.jpg");
cleanup:
    pChromaKey->Release();
    return hr;
}
```
---

### Ejemplo 11: Pantalla Azul con Ajuste Fino

Configurar clave de croma de pantalla azul con configuraciones óptimas.

#### Pantalla Azul en C#

```csharp
public void ApplyBlueScreen(IBaseFilter chromaFilter)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Establecer color azul
        chromaKey.chroma_put_color(
            red: 0,
            green: 0,
            blue: 255);

        // Umbrales ajustados para pantalla azul
        // Valores más bajos = más estricto (menos tolerancia)
        // Valores más altos = más suelto (más tolerancia)
        chromaKey.chroma_put_contrast(
            contrast1: 30,   // Umbral inferior más ajustado
            contrast2: 80);  // Umbral superior moderado
    }
}
```

---
### Ejemplo 12: Clave de Croma de Color Personalizado
Usar cualquier color personalizado para la clave.
#### Clave de Color Personalizado en C#
```csharp
public void ApplyCustomColorKey(IBaseFilter chromaFilter, Color keyColor)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Usar cualquier color personalizado
        chromaKey.chroma_put_color(
            red: keyColor.R,
            green: keyColor.G,
            blue: keyColor.B);
        // Umbrales estándar
        chromaKey.chroma_put_contrast(50, 100);
    }
}
// Ejemplo de uso:
// ApplyCustomColorKey(chromaFilter, Color.Magenta);  // Pantalla magenta
// ApplyCustomColorKey(chromaFilter, Color.FromArgb(255, 180, 0, 220));  // Púrpura personalizado
```
---

## Tubería de Procesamiento Completa

### Ejemplo 13: Efectos Combinados, Mezcla y Clave de Croma

Ejemplo completo combinando todos los filtros de procesamiento.

#### Tubería Completa en C#

```csharp
public class CompleteProcessingPipeline
{
    public void CreateCompleteSetup(
        string mainVideoPath,
        string greenScreenVideoPath,
        string backgroundImagePath,
        IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();

        // 1. Fuente de video principal
        filterGraph.AddSourceFilter(mainVideoPath, "Main Video", out IBaseFilter mainSource);

        // 2. Fuente de video de pantalla verde
        filterGraph.AddSourceFilter(greenScreenVideoPath, "Green Screen", out IBaseFilter gsSource);

        // 3. Agregar filtro de Clave de Croma para pantalla verde
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");

        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            chromaKey.chroma_put_color(0, 255, 0);  // Verde
            chromaKey.chroma_put_contrast(40, 90);
        }

        // 4. Agregar filtro de Efectos de Video
        var effectsFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        var effects = effectsFilter as IVFEffects45;
        if (effects != null)
        {
            // Aplicar algunos efectos
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, "Value", 60);

            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, 1);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, "Value", 80);

            // Agregar superposición de texto
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, 1);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "Text", "EN VIVO");
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "FontSize", 48);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "X", 50);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "Y", 50);
        }

        // 5. Agregar Mezclador de Video
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");

        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            mixer.SetOutputParam("Width", 1920);
            mixer.SetOutputParam("Height", 1080);
            mixer.SetOutputParam("FrameRate", 30.0);

            // Video principal (fondo de pantalla completa)
            mixer.SetInputParam(0, "X", 0);
            mixer.SetInputParam(0, "Y", 0);
            mixer.SetInputParam(0, "Width", 1920);
            mixer.SetInputParam(0, "Height", 1080);

            // Video con clave de croma (PIP)
            mixer.SetInputParam(1, "X", 1200);
            mixer.SetInputParam(1, "Y", 700);
            mixer.SetInputParam(1, "Width", 640);
            mixer.SetInputParam(1, "Height", 360);

            mixer.SetInputOrder(new int[] { 0, 1 });
        }

        // Conectar la tubería:
        // Fuente Principal → Efectos → Entrada 0 del Mezclador
        // Fuente GS → Clave de Croma → Entrada 1 del Mezclador
        // Mezclador → Renderizador

        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Conectar ruta principal
        captureGraph.RenderStream(null, MediaType.Video, mainSource, effectsFilter, mixerFilter);

        // Conectar ruta de clave de croma
        // (Requiere conexiones a nivel de pin a entrada específica del mezclador)

        // Renderizar salida del mezclador
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Audio
        captureGraph.RenderStream(null, MediaType.Audio, mainSource, null, null);

        // Configurar ventana de video
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Ejecutar
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

---
## Solución de Problemas
### Problema: Efecto No Visible
**Solución**: Asegúrese de que el efecto esté habilitado y los parámetros estén establecidos:
```csharp
effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);  // 1 = habilitado
effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, "Value", 75);
```
### Problema: Clave de Croma No Funciona Bien
**Solución**: Ajuste los umbrales de contraste:
```csharp
// Para pantallas verdes difíciles:
chromaKey.chroma_put_contrast(30, 70);  // Rango más ajustado
// Para pantallas verdes bien iluminadas:
chromaKey.chroma_put_contrast(50, 110);  // Rango más amplio
```
### Problema: Entradas del Mezclador de Video No Se Muestran
**Solución**: Verifique los parámetros de entrada y el orden Z:
```csharp
// Asegúrese de que las entradas estén en pantalla
mixer.SetInputParam(index, "X", 0);     // Debe ser >= 0
mixer.SetInputParam(index, "Y", 0);     // Debe ser >= 0
mixer.SetInputParam(index, "Width", 640);  // Debe ser > 0
mixer.SetInputParam(index, "Height", 480); // Debe ser > 0
// Verifique orden Z
mixer.SetInputOrder(new int[] { 0, 1, 2 });  // 0 = atrás, 2 = frente
```
---

## Ver También

### Documentación

- [Referencia de Efectos](effects-reference.md) - Lista completa de efectos
- [Interfaz de Mezclador de Video](interfaces/video-mixer.md) - Referencia completa de API
- [Interfaz de Clave de Croma](interfaces/chroma-key.md) - Documentos completos de interfaz

### Recursos Externos

- [Gráfico de Filtros DirectShow](https://learn.microsoft.com/en-us/windows/win32/directshow/building-the-filter-graph)
