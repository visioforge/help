---
title: DirectShow Video Effects, Mixer, and Chroma Key Examples
description: Code examples for Video Effects, Video Mixer, and Chroma Key filters in C++, C#, and VB.NET with DirectShow integration.
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

# Processing Filters Pack - Code Examples

## Overview

This page provides practical code examples for using the Processing Filters Pack, which includes:

- **Video Effects** - 35+ real-time effects (text, graphics, color adjustments, denoise)
- **Video Mixer** - 2-16 source mixing with PIP, alpha blending, chroma key
- **Chroma Key** - Green/blue screen compositing

---
## Prerequisites
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

## Video Effects Examples

### Example 1: Basic Video Effect

Apply a single video effect to a source.

#### C# Implementation

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

        // Add source filter (e.g., File Source)
        filterGraph.AddSourceFilter(filename, "Source", out sourceFilter);

        // Add Video Effects filter
        effectFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        // Configure effect using IVFEffects45 interface
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Enable Greyscale effect via VideoEffectSimple struct
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

#### C++ Implementation

```cpp
HRESULT ApplyVideoEffect(LPCWSTR filename)
{
    IGraphBuilder* pGraph = NULL;
    IBaseFilter* pSource = NULL;
    IBaseFilter* pEffect = NULL;
    IVFEffects45* pEffects = NULL;

    // Create filter graph
    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Add source
    hr = pGraph->AddSourceFilter(filename, L"Source", &pSource);
    if (FAILED(hr)) goto cleanup;

    // Create Video Effects filter
    hr = CoCreateInstance(CLSID_VFVideoEffects, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pEffect);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pEffect, L"Video Effects");
    if (FAILED(hr)) goto cleanup;

    // Configure effect
    hr = pEffect->QueryInterface(IID_IVFEffects45, (void**)&pEffects);
    if (SUCCEEDED(hr))
    {
        // Enable greyscale via VideoEffectSimple struct
        VideoEffectSimple effect;
        ZeroMemory(&effect, sizeof(effect));
        effect.Type = ef_greyscale;
        effect.Enabled = TRUE;
        pEffects->add_effect(&effect);
        pEffects->Release();
    }

    // Connect filters and render...
    // (Use RenderStream or ConnectFilters)

cleanup:
    if (pEffect) pEffect->Release();
    if (pSource) pSource->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

---
### Example 2: Multiple Effects Chain
Apply multiple effects simultaneously.
#### C# Multiple Effects
```csharp
public class MultipleEffectsExample
{
    public void ApplyMultipleEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Add darkness/brightness effect (VideoEffectType.Darkness, AmountI controls level)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Darkness,
                Enabled = true,
                AmountI = 50        // 0 = darkest, 100 = brightest
            });

            // Add contrast effect (AmountI controls intensity)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Contrast,
                Enabled = true,
                AmountI = 75        // Contrast intensity
            });

            // Add saturation effect (AmountI controls saturation level)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Saturation,
                Enabled = true,
                AmountI = 120       // Saturation level
            });
        }
    }
}
```
---

### Example 3: Text Overlay

Add text logo overlay to video.

#### C# Text Overlay

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

See [effects-reference.md](./effects-reference.md) for the full `MFPTextLogo` structure
(text alignment, gradient, date/time display, anti-aliasing, etc.).

---
### Example 4: Image Overlay
Add graphic watermark or logo.
#### C# Image Overlay
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

See [effects-reference.md](./effects-reference.md) for the full `MFPGraphicalLogo` structure.
---

### Example 5: Denoise Filters

Apply noise reduction for cleaner video.

#### C# Denoise Examples

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
### Example 6: All Available Effects
Complete list of effects with basic configuration.
#### C# All Effects Reference
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

        // Overlays (TextLogo/ImageLogo need sub-struct — see Examples 3 & 4)
        // To disable/remove an effect:
        // effects.remove_effect(effectId);
        // effects.clear_effects();
    }
}
```

> **Note:** For the full list of `VideoEffectType` members and sub-struct parameters,
> see [effects-reference.md](./effects-reference.md).
---

## Video Mixer Examples

### Example 7: Picture-in-Picture (PIP)

Mix two video sources with PIP layout.

#### C# Picture-in-Picture

```csharp
public class VideoMixerPIPExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter mixerFilter;

    public void CreatePIP(string mainVideoPath, string pipVideoPath, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();

        // Add main video source
        filterGraph.AddSourceFilter(mainVideoPath, "Main Source", out IBaseFilter mainSource);

        // Add PIP video source
        filterGraph.AddSourceFilter(pipVideoPath, "PIP Source", out IBaseFilter pipSource);

        // Add Video Mixer filter
        mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");

        // Configure mixer — real IVFVideoMixer interface
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Set output size
            mixer.SetOutputParam(new VFPIPVideoOutputParam
            {
                Width = 1920,
                Height = 1080,
                FrameRateTime = 30
            });

            // Configure main video (input 0) - fullscreen
            mixer.SetInputParam(0, new VFPIPVideoInputParam
            {
                X = 0, Y = 0,
                Width = 1920, Height = 1080,
                Alpha = 255
            });

            // Configure PIP video (input 1) - bottom-right corner
            mixer.SetInputParam(1, new VFPIPVideoInputParam
            {
                X = 1440,    // 1920 - 480
                Y = 810,     // 1080 - 270
                Width = 480,
                Height = 270,
                Alpha = 255
            });

            // Set Z-order (layering) — per-pin, not bulk array
            mixer.SetInputOrder(0, 0);  // Main behind
            mixer.SetInputOrder(1, 1);  // PIP on top
        }

        // Connect filters
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Connect main source to mixer input 0
        captureGraph.RenderStream(null, MediaType.Video, mainSource, null, mixerFilter);

        // Connect PIP source to mixer input 1
        // Note: Requires connecting to specific input pin
        IPin mixerInput1 = DsFindPin.ByDirection(mixerFilter, PinDirection.Input, 1);
        captureGraph.RenderStream(null, MediaType.Video, pipSource, null, null);
        // Connect to mixerInput1 explicitly...

        // Render mixer output
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
### Example 8: Multi-Source Mixing (4 inputs)
Create a 2x2 grid layout with 4 video sources.
#### C# 2x2 Grid Layout
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
        // Add all source filters
        IBaseFilter[] sources = new IBaseFilter[4];
        for (int i = 0; i < 4; i++)
        {
            filterGraph.AddSourceFilter(videoPaths[i], $"Source {i}", out sources[i]);
        }
        // Add Video Mixer
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Set output size
            mixer.SetOutputParam(new VFPIPVideoOutputParam
            {
                Width = 1920,
                Height = 1080,
                FrameRateTime = 30
            });
            int halfWidth = 960;   // 1920 / 2
            int halfHeight = 540;  // 1080 / 2
            // Top-left (Input 0)
            mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Top-right (Input 1)
            mixer.SetInputParam(1, new VFPIPVideoInputParam { X = halfWidth, Y = 0, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Bottom-left (Input 2)
            mixer.SetInputParam(2, new VFPIPVideoInputParam { X = 0, Y = halfHeight, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Bottom-right (Input 3)
            mixer.SetInputParam(3, new VFPIPVideoInputParam { X = halfWidth, Y = halfHeight, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Set Z-order (per-pin)
            for (int i = 0; i < 4; i++)
            {
                mixer.SetInputOrder(i, i);
            }
        }
        // Connect sources to mixer and render...
        // (Similar to PIP example)
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
    }
}
```
---

### Example 9: Video Mixer with Chroma Key

Mix sources with transparent background.

#### C# Mixer with Chroma Key

```csharp
public void CreateMixerWithChromaKey(string backgroundPath, string foregroundPath)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Add sources
    filterGraph.AddSourceFilter(backgroundPath, "Background", out IBaseFilter bgSource);
    filterGraph.AddSourceFilter(foregroundPath, "Foreground", out IBaseFilter fgSource);

    // Add mixer
    var mixerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVideoMixer,
        "Video Mixer");

    var mixer = mixerFilter as IVFVideoMixer;
    if (mixer != null)
    {
        // Configure output
        mixer.SetOutputParam(new VFPIPVideoOutputParam
        {
            Width = 1920,
            Height = 1080
        });

        // Background (fullscreen)
        mixer.SetInputParam(0, new VFPIPVideoInputParam
        {
            X = 0, Y = 0, Width = 1920, Height = 1080
        });

        // Foreground (centered, smaller)
        mixer.SetInputParam(1, new VFPIPVideoInputParam
        {
            X = 480, Y = 270, Width = 960, Height = 540
        });

        // Enable chroma key — mixer-wide, 4 args
        mixer.SetChromaSettings(
            enabled: true,
            color: ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)),  // Green
            tolerance1: 50,
            tolerance2: 10);
    }

    // Connect and run...
}
```

---
## Chroma Key Examples
### Example 10: Green Screen Effect
Standalone chroma key filter for green screen removal.
#### C# Chroma Key Filter
```csharp
public class ChromaKeyExample
{
    public void ApplyGreenScreen(string videoPath, string backgroundImagePath, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        // Add video source (with green screen)
        filterGraph.AddSourceFilter(videoPath, "Source", out IBaseFilter sourceFilter);
        // Add Chroma Key filter
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");
        // Configure chroma key — real IVFChromaKey API
        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            // Set key color (green) — single int using RGB macro
            chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)));

            // Set contrast range (low, high bounds)
            chromaKey.put_contrast(50, 100);

            // Set background image (optional)
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                chromaKey.put_image(backgroundImagePath);
            }
        }
        // Connect filters: Source → Chroma Key → Renderer
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

    // Set green color — single COLORREF argument: RGB(0, 255, 0)
    hr = pChromaKey->put_color(RGB(0, 255, 0));
    if (FAILED(hr)) goto cleanup;

    // Set contrast range (low, high)
    hr = pChromaKey->put_contrast(40, 90);
    if (FAILED(hr)) goto cleanup;

    // Optional: Set background image
    hr = pChromaKey->put_image(L"C:\\backgrounds\\studio.jpg");

cleanup:
    pChromaKey->Release();
    return hr;
}
```
---

### Example 11: Blue Screen with Fine Tuning

Configure blue screen chroma key with optimal settings.

#### C# Blue Screen

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
### Example 12: Custom Color Chroma Key
Use any custom color for keying.
#### C# Custom Color Key
```csharp
public void ApplyCustomColorKey(IBaseFilter chromaFilter, Color keyColor)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Use any custom color — single int via RGB
        chromaKey.put_color(ColorTranslator.ToWin32(keyColor));

        // Standard contrast range
        chromaKey.put_contrast(50, 100);
    }
}
// Example usage:
// ApplyCustomColorKey(chromaFilter, Color.Magenta);  // Magenta screen
// ApplyCustomColorKey(chromaFilter, Color.FromArgb(255, 180, 0, 220));  // Custom purple
```
---

## Complete Processing Pipeline

### Example 13: Combined Effects, Mixing, and Chroma Key

Complete example combining all processing filters.

#### C# Complete Pipeline

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

        // 3. Add Chroma Key filter for green screen
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");

        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)));  // Green
            chromaKey.put_contrast(40, 90);
        }

        // 4. Add Video Effects filter
        var effectsFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        var effects = effectsFilter as IVFEffects45;
        if (effects != null)
        {
            // Apply effects via VideoEffectSimple structs
            effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Darkness, Enabled = true, AmountI = 60 });
            effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Contrast, Enabled = true, AmountI = 80 });

            // Add text overlay
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

        // 5. Add Video Mixer
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Video Mixer");

        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            mixer.SetOutputParam(new VFPIPVideoOutputParam { Width = 1920, Height = 1080, FrameRateTime = 30 });

            // Main video (fullscreen background)
            mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = 1920, Height = 1080 });

            // Chroma-keyed video (PIP)
            mixer.SetInputParam(1, new VFPIPVideoInputParam { X = 1200, Y = 700, Width = 640, Height = 360 });

            // Set Z-order (per-pin)
            mixer.SetInputOrder(0, 0);
            mixer.SetInputOrder(1, 1);
        }

        // Connect the pipeline:
        // Main Source → Effects → Mixer Input 0
        // GS Source → Chroma Key → Mixer Input 1
        // Mixer → Renderer

        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Connect main path
        captureGraph.RenderStream(null, MediaType.Video, mainSource, effectsFilter, mixerFilter);

        // Connect chroma key path
        // (Requires pin-level connections to specific mixer input)

        // Render mixer output
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Audio
        captureGraph.RenderStream(null, MediaType.Audio, mainSource, null, null);

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
## Troubleshooting
### Issue: Effect Not Visible
**Solution**: Ensure effect is enabled and parameters are set on the VideoEffectSimple struct:
```csharp
var eff = new VideoEffectSimple
{
    Type = (int)VideoEffectType.Darkness,
    Enabled = true,
    AmountI = 75
};
effects.add_effect(eff);
```
### Issue: Chroma Key Not Working Well
**Solution**: Adjust contrast thresholds:
```csharp
// For difficult green screens:
chromaKey.put_contrast(30, 70);  // Tighter range
// For well-lit green screens:
chromaKey.put_contrast(50, 110);  // Wider range
```
### Issue: Video Mixer Inputs Not Showing
**Solution**: Verify input parameters and Z-order:
```csharp
// Ensure inputs are on-screen via VFPIPVideoInputParam struct
mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = 640, Height = 480 });
// Set Z-order per-pin
mixer.SetInputOrder(0, 0);  // Input 0 at layer 0
mixer.SetInputOrder(1, 1);  // Input 1 at layer 1
```
---

## See Also

### Documentation

- [Effects Reference](effects-reference.md) - Complete effects list
- [Video Mixer Interface](interfaces/video-mixer.md) - Full API reference
- [Chroma Key Interface](interfaces/chroma-key.md) - Complete interface docs

### External Resources

- [DirectShow Filter Graph](https://learn.microsoft.com/en-us/windows/win32/directshow/building-the-filter-graph)
