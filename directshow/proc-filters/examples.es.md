---
title: Filtros DirectShow: Efectos, Video Mixer y Chroma Key
description: Ejemplos de código para Video Effects, Video Mixer y Chroma Key en C++, C# y VB.NET con DirectShow. Incluye PIP, pantalla verde y denoise.
tags:
  - DirectShow
  - C++
  - Windows
  - Effects
  - Mixing
  - C#
primary_api_classes:
  - IBaseFilter
  - IVFChromaKey
  - IVFVideoMixer

---

# Processing Filters Pack - Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos de código para usar el Processing Filters Pack, que incluye:

- **Video Effects** - Más de 35 efectos en tiempo real (texto, gráficos, ajustes de color, denoise)
- **Video Mixer** - Mezcla de 2-16 fuentes con PIP, alpha blending, chroma key
- **Chroma Key** - Composición de pantalla verde/azul

---
## Requisitos Previos
### C++ Projects
```cpp
#include <dshow.h>
#include <streams.h>
#include "IVFEffects45.h"
#include "IVFVideoMixer.h"
#include "IVFChromaKey.h"
#pragma comment(lib, "strmiids.lib")
```
### C# Projects
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
using System.Drawing;
```
**NuGet Packages**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Ejemplos de Efectos de Video

### Ejemplo 1: Efecto de Video Básico

Apply a single video effect to a source.

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

        // Añadir filtro de fuente (por ejemplo, File Source)
        filterGraph.AddSourceFilter(filename, "Source", out sourceFilter);

        // Añadir filtro Video Effects
        effectFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        // Configurar el efecto usando la interfaz IVFEffects45
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Activar el efecto Greyscale mediante la estructura VideoEffectSimple
            var eff = new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Greyscale,
                Enabled = true
            };
            effects.add_effect(eff);
        }
        captureGraph.SetFiltergraph(filterGraph);

        // Render through effect filter
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, effectFilter, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        // Set up video window
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Run
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

    // Crear el grafo de filtros
    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Añadir fuente
    hr = pGraph->AddSourceFilter(filename, L"Source", &pSource);
    if (FAILED(hr)) goto cleanup;

    // Crear el filtro Video Effects
    hr = CoCreateInstance(CLSID_VFVideoEffects, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pEffect);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pEffect, L"Video Effects");
    if (FAILED(hr)) goto cleanup;

    // Configurar el efecto
    hr = pEffect->QueryInterface(IID_IVFEffects45, (void**)&pEffects);
    if (SUCCEEDED(hr))
    {
        // Activar escala de grises mediante la estructura VideoEffectSimple
        VideoEffectSimple effect;
        ZeroMemory(&effect, sizeof(effect));
        effect.Type = ef_greyscale;
        effect.Enabled = TRUE;
        pEffects->add_effect(&effect);
        pEffects->Release();
    }

    // Conectar los filtros y renderizar...
    // (Use RenderStream o ConnectFilters)

cleanup:
    if (pEffect) pEffect->Release();
    if (pSource) pSource->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

---
### Ejemplo 2: Cadena de Efectos Múltiples
Apply multiple effects simultaneously.
#### C# Efectos Múltiples
```csharp
public class MultipleEffectsExample
{
    public void ApplyMultipleEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Añadir efecto de oscuridad/brillo (VideoEffectType.Darkness, AmountI controla el nivel)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Darkness,
                Enabled = true,
                AmountI = 50        // 0 = más oscuro, 100 = más brillante
            });

            // Añadir efecto de contraste (AmountI controla la intensidad)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Contrast,
                Enabled = true,
                AmountI = 75        // Intensidad del contraste
            });

            // Añadir efecto de saturación (AmountI controla el nivel de saturación)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Saturation,
                Enabled = true,
                AmountI = 120       // Nivel de saturación
            });
        }
    }
}
```
---

### Ejemplo 3: Superposición de Texto

Añada una superposición de logotipo de texto al video.

#### C# Superposición de Texto

```csharp
public void ApplyTextOverlay(IBaseFilter effectFilter)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        var eff = new VideoEffectSimple
        {
            Type = (int)VideoEffectType.TextLogo,
            Enabled = true,
            TextLogo = new MFPTextLogo
            {
                X = 50,
                Y = 50,
                Text = "My Video Title",
                FontName = "Arial",
                FontSize = 36,
                FontColor = 0xFFFFFF,        // White
                FontBold = true,
                TransparentBg = true,
                Transp = 255,                // Fully opaque
                BorderMode = 4,              // bm_outline
                OuterBorderColor = 0x000000, // Black outline
                OuterBorderSize = 2
            }
        };
        effects.add_effect(eff);
    }
}
```

Consulte [effects-reference.md](./effects-reference.md) para la estructura completa de `MFPTextLogo`
(text alignment, gradient, date/time display, anti-aliasing, etc.).

---
### Ejemplo 4: Superposición de Imagen
Añada una marca de agua o logotipo gráfico.
#### C# Superposición de Imagen
```csharp
public void ApplyImageOverlay(IBaseFilter effectFilter, string imagePath)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        var eff = new VideoEffectSimple
        {
            Type = (int)VideoEffectType.ImageLogo,
            Enabled = true,
            GraphicalLogo = new MFPGraphicalLogo
            {
                X = 10,              // X position in pixels
                Y = 10,              // Y position in pixels
                Filename = imagePath,
                TranspLevel = 200,   // Semi-transparent (0-255)
                StretchMode = 2      // 0=None, 1=Stretch, 2=Proportional fit
            }
        };
        effects.add_effect(eff);
    }
}
```

Consulte [effects-reference.md](./effects-reference.md) para la estructura completa de `MFPGraphicalLogo`.
---

### Ejemplo 5: Filtros de Denoise

Aplique reducción de ruido para video más limpio.

#### C# Ejemplos de Denoise

```csharp
public void ApplyDenoise(IBaseFilter effectFilter, VideoEffectType denoiseType)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        var eff = new VideoEffectSimple
        {
            Type = (int)denoiseType,
            Enabled = true
        };

        switch (denoiseType)
        {
            case VideoEffectType.DenoiseCAST:
                // CAST denoise — configure via DenoiseCAST sub-struct
                eff.DenoiseCAST = new MFPDenoiseCAST
                {
                    TemporalDifferenceThreshold = 16,
                    StrongEdgeThreshold = 8
                };
                break;

            case VideoEffectType.DenoiseAdaptive:
                // Adaptive denoise — threshold controls sensitivity
                eff.DenoiseAdaptiveThreshold = 20;   // 0-255
                eff.DenoiseAdaptiveBlurMode = 0;     // 0-3
                break;

            case VideoEffectType.DenoiseMosquito:
                // Mosquito denoise — AmountI controls reduction strength
                eff.AmountI = 30;
                break;
        }

        effects.add_effect(eff);
    }
}
```

---
### Ejemplo 6: Todos los Efectos Disponibles
Lista completa de efectos con configuración básica.
#### C# Referencia de Efectos
```csharp
public class AllEffectsExample
{
    public void DemonstrateAllEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects == null) return;

        // Color Filters
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Greyscale, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Invert, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FilterRed, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FilterGreen, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FilterBlue, Enabled = true });

        // Image Adjustment (AmountI controls intensity)
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Darkness, Enabled = true, AmountI = 50 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Contrast, Enabled = true, AmountI = 75 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Saturation, Enabled = true, AmountI = 120 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Lightness, Enabled = true, AmountI = 45 });

        // Spatial Transforms
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FlipRight, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FlipDown, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.MirrorHorizontal, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Rotate, Enabled = true, AmountI = 90 });

        // Artistic Effects
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Blur, Enabled = true, AmountI = 5 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Sharpen, Enabled = true, AmountI = 2 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Posterize, Enabled = true, AmountI = 8 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Solorize, Enabled = true, AmountI = 128 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Mosaic, Enabled = true, SizeI = 10 });

        // Noise Reduction
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DenoiseCAST, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DenoiseAdaptive, Enabled = true, DenoiseAdaptiveThreshold = 20 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DenoiseMosquito, Enabled = true, AmountI = 30 });

        // Deinterlacing
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DeinterlaceBlend, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DeinterlaceTriangle, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DeinterlaceCAVT, Enabled = true });

        // Superposiciones (TextLogo/ImageLogo requieren una subestructura; vea los ejemplos 3 y 4)
        // Para desactivar o quitar un efecto:
        // effects.remove_effect(effectId);
        // effects.clear_effects();
    }
}
```

    > **Nota:** Para la lista completa de miembros de `VideoEffectType` y los parámetros de subestructura,
    > consulte [effects-reference.md](./effects-reference.md).
---

## Ejemplos de Video Mixer

### Ejemplo 7: Picture-in-Picture (PIP)

Mezcle dos fuentes de video con diseño PIP.

#### C# Picture-in-Picture

```csharp
public class VideoMixerPIPExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter mixerFilter;

    public void CreatePIP(string mainVideoPath, string pipVideoPath, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();

        // Añadir la fuente principal de video
        filterGraph.AddSourceFilter(mainVideoPath, "Main Source", out IBaseFilter mainSource);

        // Añadir la fuente de video PIP
        filterGraph.AddSourceFilter(pipVideoPath, "PIP Source", out IBaseFilter pipSource);

        // Añadir el filtro Video Mixer
        mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");

        // Configurar el mezclador: interfaz real de IVFVideoMixer
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Establecer el tamaño de salida
            mixer.SetOutputParam(new VFPIPVideoOutputParam
            {
                Width = 1920,
                Height = 1080,
                FrameRateTime = 30
            });

            // Configurar el video principal (entrada 0): pantalla completa
            mixer.SetInputParam(0, new VFPIPVideoInputParam
            {
                X = 0, Y = 0,
                Width = 1920, Height = 1080,
                Alpha = 255
            });

            // Configurar el video PIP (entrada 1): esquina inferior derecha
            mixer.SetInputParam(1, new VFPIPVideoInputParam
            {
                X = 1440,    // 1920 - 480
                Y = 810,     // 1080 - 270
                Width = 480,
                Height = 270,
                Alpha = 255
            });

            // Establecer el orden Z (capas), por pin y no como arreglo masivo
            mixer.SetInputOrder(0, 0);  // Principal detrás
            mixer.SetInputOrder(1, 1);  // PIP encima
        }

        // Conectar filtros
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Conectar la fuente principal a la entrada 0 del mezclador
        captureGraph.RenderStream(null, MediaType.Video, mainSource, null, mixerFilter);

        // Conectar la fuente PIP a la entrada 1 del mezclador
        // Nota: requiere conectarse al pin de entrada específico
        IPin mixerInput1 = DsFindPin.ByDirection(mixerFilter, PinDirection.Input, 1);
        captureGraph.RenderStream(null, MediaType.Video, pipSource, null, null);
        // Conectar a mixerInput1 explícitamente...

        // Renderizar la salida del mezclador
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Setup video window
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Run
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

---
### Ejemplo 8: Mezcla Multi-Fuente (4 entradas)
Cree un diseño de cuadrícula 2x2 con 4 fuentes de video.
#### C# Diseño de Cuadrícula 2x2
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
        // Añadir todos los filtros de fuente
        IBaseFilter[] sources = new IBaseFilter[4];
        for (int i = 0; i < 4; i++)
        {
            filterGraph.AddSourceFilter(videoPaths[i], $"Source {i}", out sources[i]);
        }
        // Añadir Video Mixer
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Establecer el tamaño de salida
            mixer.SetOutputParam(new VFPIPVideoOutputParam
            {
                Width = 1920,
                Height = 1080,
                FrameRateTime = 30
            });
            int halfWidth = 960;   // 1920 / 2
            int halfHeight = 540;  // 1080 / 2
            // Parte superior izquierda (entrada 0)
            mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Parte superior derecha (entrada 1)
            mixer.SetInputParam(1, new VFPIPVideoInputParam { X = halfWidth, Y = 0, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Parte inferior izquierda (entrada 2)
            mixer.SetInputParam(2, new VFPIPVideoInputParam { X = 0, Y = halfHeight, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Parte inferior derecha (entrada 3)
            mixer.SetInputParam(3, new VFPIPVideoInputParam { X = halfWidth, Y = halfHeight, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Establecer el orden Z (por pin)
            for (int i = 0; i < 4; i++)
            {
                mixer.SetInputOrder(i, i);
            }
        }
        // Conectar fuentes al mezclador y renderizar...
        // (Similar al ejemplo PIP)
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
    }
}
```
---

### Ejemplo 9: Video Mixer con Chroma Key

Mezcle fuentes con fondo transparente.

#### C# Mixer con Chroma Key

```csharp
public void CreateMixerWithChromaKey(string backgroundPath, string foregroundPath)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Añadir fuentes
    filterGraph.AddSourceFilter(backgroundPath, "Background", out IBaseFilter bgSource);
    filterGraph.AddSourceFilter(foregroundPath, "Foreground", out IBaseFilter fgSource);

    // Añadir mezclador
    var mixerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVideoMixer,
        "Video Mixer");

    var mixer = mixerFilter as IVFVideoMixer;
    if (mixer != null)
    {
        // Configurar la salida
        mixer.SetOutputParam(new VFPIPVideoOutputParam
        {
            Width = 1920,
            Height = 1080
        });

        // Fondo (pantalla completa)
        mixer.SetInputParam(0, new VFPIPVideoInputParam
        {
            X = 0, Y = 0, Width = 1920, Height = 1080
        });

        // Primer plano (centrado, más pequeño)
        mixer.SetInputParam(1, new VFPIPVideoInputParam
        {
            X = 480, Y = 270, Width = 960, Height = 540
        });

        // Activar chroma key para todo el mezclador, con 4 argumentos
        mixer.SetChromaSettings(
            enabled: true,
            color: ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)),  // Verde
            tolerance1: 50,
            tolerance2: 10);
    }

    // Conectar y ejecutar...
}
```

---
## Ejemplos de Chroma Key
### Ejemplo 10: Efecto Pantalla Verde
Filtro chroma key independiente para eliminación de pantalla verde.
#### C# Filtro Chroma Key
```csharp
public class ChromaKeyExample
{
    public void ApplyGreenScreen(string videoPath, string backgroundImagePath, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        // Añadir fuente de video (con pantalla verde)
        filterGraph.AddSourceFilter(videoPath, "Source", out IBaseFilter sourceFilter);
        // Añadir filtro Chroma Key
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");
        // Configurar chroma key: API real de IVFChromaKey
        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            // Establecer el color clave (verde): entero único usando la macro RGB
            chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)));

            // Establecer el rango de contraste (límites bajo y alto)
            chromaKey.put_contrast(50, 100);

            // Establecer la imagen de fondo (opcional)
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                chromaKey.put_image(backgroundImagePath);
            }
        }
        // Conectar filtros: fuente → Chroma Key → renderizador
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, chromaFilter, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        // Setup video window
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        // Run
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
#### C++ Chroma Key
```cpp
HRESULT ApplyChromaKey(IBaseFilter* pChromaFilter)
{
    IVFChromaKey* pChromaKey = NULL;
    HRESULT hr = pChromaFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChromaKey);
    if (FAILED(hr)) return hr;

    // Establecer el color verde: argumento COLORREF único: RGB(0, 255, 0)
    hr = pChromaKey->put_color(RGB(0, 255, 0));
    if (FAILED(hr)) goto cleanup;

    // Establecer el rango de contraste (bajo, alto)
    hr = pChromaKey->put_contrast(40, 90);
    if (FAILED(hr)) goto cleanup;

    // Opcional: establecer imagen de fondo
    hr = pChromaKey->put_image(L"C:\\backgrounds\\studio.jpg");

cleanup:
    pChromaKey->Release();
    return hr;
}
```
---

### Ejemplo 11: Pantalla Azul con Ajuste Fino

Configure chroma key de pantalla azul con ajustes óptimos.

#### C# Pantalla Azul

```csharp
public void ApplyBlueScreen(IBaseFilter chromaFilter)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Set blue color — single int via RGB
        chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 0, 255)));

        // Fine-tuned contrast range for blue screen
        // Lower low = more strict (less tolerance)
        // Higher high = more loose (more tolerance)
        chromaKey.put_contrast(30, 80);
    }
}
```

---
### Ejemplo 12: Chroma Key de Color Personalizado
Utilice cualquier color personalizado para chroma key.
#### C# Color Personalizado
```csharp
public void ApplyCustomColorKey(IBaseFilter chromaFilter, Color keyColor)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Utilice cualquier color personalizado: entero único mediante RGB
        chromaKey.put_color(ColorTranslator.ToWin32(keyColor));

        // Rango de contraste estándar
        chromaKey.put_contrast(50, 100);
    }
}
// Ejemplo de uso:
// ApplyCustomColorKey(chromaFilter, Color.Magenta);  // Pantalla magenta
// ApplyCustomColorKey(chromaFilter, Color.FromArgb(255, 180, 0, 220));  // Púrpura personalizado
```
---

## Pipeline de Procesamiento Completo

### Ejemplo 13: Pipeline Combinado

Ejemplo completo que combina todos los filtros de procesamiento.

#### C# Pipeline Completo

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

        // 1. Main video source
        filterGraph.AddSourceFilter(mainVideoPath, "Main Video", out IBaseFilter mainSource);

        // 2. Green screen video source
        filterGraph.AddSourceFilter(greenScreenVideoPath, "Green Screen", out IBaseFilter gsSource);

        // 3. Añadir filtro Chroma Key para pantalla verde
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");

        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)));  // Verde
            chromaKey.put_contrast(40, 90);
        }

        // 4. Añadir filtro Video Effects
        var effectsFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        var effects = effectsFilter as IVFEffects45;
        if (effects != null)
        {
            // Aplicar efectos mediante estructuras VideoEffectSimple
            effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Darkness, Enabled = true, AmountI = 60 });
            effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Contrast, Enabled = true, AmountI = 80 });

            // Añadir superposición de texto
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.TextLogo,
                Enabled = true,
                TextLogo = new MFPTextLogo
                {
                    Text = "LIVE",
                    FontSize = 48,
                    X = 50,
                    Y = 50,
                    FontColor = 0xFFFFFF,
                    FontBold = true
                }
            });
        }

        // 5. Añadir Video Mixer
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");

        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            mixer.SetOutputParam(new VFPIPVideoOutputParam { Width = 1920, Height = 1080, FrameRateTime = 30 });

            // Video principal (fondo a pantalla completa)
            mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = 1920, Height = 1080 });

            // Video con chroma key (PIP)
            mixer.SetInputParam(1, new VFPIPVideoInputParam { X = 1200, Y = 700, Width = 640, Height = 360 });

            // Establecer el orden Z (por pin)
            mixer.SetInputOrder(0, 0);
            mixer.SetInputOrder(1, 1);
        }

        // Conectar la canalización:
        // Fuente principal → Efectos → Entrada 0 del mezclador
        // Fuente GS → Chroma Key → Entrada 1 del mezclador
        // Mezclador → Renderizador

        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Conectar la ruta principal
        captureGraph.RenderStream(null, MediaType.Video, mainSource, effectsFilter, mixerFilter);

        // Conectar la ruta de chroma key
        // (Requiere conexiones por pin al mezclador de entrada específico)

        // Renderizar la salida del mezclador
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Audio
        captureGraph.RenderStream(null, MediaType.Audio, mainSource, null, null);

        // Configurar la ventana de video
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
### Problema: El Efecto No Es Visible
**Solución**: Asegúrese de que el efecto esté habilitado y los parámetros configurados en VideoEffectSimple:
```csharp
var eff = new VideoEffectSimple
{
    Type = (int)VideoEffectType.Darkness,
    Enabled = true,
    AmountI = 75
};
effects.add_effect(eff);
```
### Problema: Chroma Key No Funciona Bien
**Solución**: Ajuste los umbrales de contraste:
```csharp
// Para pantallas verdes difíciles:
chromaKey.put_contrast(30, 70);  // Rango más estrecho
// Para pantallas verdes bien iluminadas:
chromaKey.put_contrast(50, 110);  // Rango más amplio
```
### Problema: Entradas del Video Mixer No Se Muestran
**Solución**: Verifique los parámetros de entrada y el Z-order:
```csharp
// Asegúrese de que las entradas estén en pantalla mediante la estructura VFPIPVideoInputParam
mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = 640, Height = 480 });
// Establecer el orden Z por pin
mixer.SetInputOrder(0, 0);  // Entrada 0 en la capa 0
mixer.SetInputOrder(1, 1);  // Entrada 1 en la capa 1
```
---

## Ver También

### Documentación

- [Referencia de Efectos](effects-reference.md) - Lista completa de efectos
- [Interfaz Video Mixer](interfaces/video-mixer.md) - Referencia completa de la API
- [Interfaz Chroma Key](interfaces/chroma-key.md) - Documentación completa

### Recursos Externos

- [DirectShow Filter Graph](https://learn.microsoft.com/en-us/windows/win32/directshow/building-the-filter-graph)
