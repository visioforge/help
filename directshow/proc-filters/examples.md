---
title: Processing Filters Pack - Code Examples
description: Code examples for Video Effects, Video Mixer, and Chroma Key filters in C++, C#, and VB.NET with DirectShow integration.
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
            // Enable Greyscale effect
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREYSCALE, 1);

            // Set effect parameters (some effects require parameters)
            // effects.SetEffectParam(effectType, paramName, paramValue);
        }

        // Connect filters: Source → Effect → Renderer
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
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
        // Enable greyscale
        pEffects->SetEffect(VF_VIDEO_EFFECT_GREYSCALE, 1);
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
            // Enable multiple effects
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, 1);
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION, 1);
            // Set parameters for each effect
            effects.SetEffectParam(
                VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS,
                "Value",
                50);  // Brightness value (0-100)
            effects.SetEffectParam(
                VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST,
                "Value",
                75);  // Contrast value (0-100)
            effects.SetEffectParam(
                VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION,
                "Value",
                120);  // Saturation value (0-200)
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
        // Enable text logo effect
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, 1);

        // Configure text parameters
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Text",
            "My Video Title");

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
            50);  // X position

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Y",
            50);  // Y position

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Transparent",
            0);  // Opaque

        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO,
            "Alpha",
            255);  // Fully opaque
    }
}
```

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
        // Enable graphic logo effect
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO, 1);
        // Set image file path
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "ImageFile",
            imagePath);
        // Set position (0-100% of screen)
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "X",
            10);  // 10% from left
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Y",
            10);  // 10% from top
        // Set size (0-100% of original)
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Width",
            25);  // 25% of screen width
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Height",
            25);  // 25% of screen height
        // Set transparency (0-255)
        effects.SetEffectParam(
            VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO,
            "Alpha",
            200);  // Semi-transparent
    }
}
```
---

### Example 5: Denoise Filters

Apply noise reduction for cleaner video.

#### C# Denoise Examples

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
                // CAST denoise - good for general noise
                effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_CAST, 1);
                break;

            case DenoiseType.Adaptive:
                // Adaptive denoise - adjusts based on content
                effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_ADAPTIVE, 1);
                effects.SetEffectParam(
                    VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_ADAPTIVE,
                    "Strength",
                    5);  // Strength 1-10
                break;

            case DenoiseType.Mosquito:
                // Mosquito denoise - reduces compression artifacts
                effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_MOSQUITO, 1);
                effects.SetEffectParam(
                    VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_MOSQUITO,
                    "Threshold",
                    30);  // Threshold value
                break;
        }
    }
}
```

---
### Example 6: All Available Effects
Complete list of all 35+ effects with basic configuration.
#### C# All Effects Reference
```csharp
public class AllEffectsExample
{
    public void DemonstrateAllEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects == null) return;
        // Color Filters
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREYSCALE, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_INVERT, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_RED_FILTER, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREEN_FILTER, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BLUE_FILTER, 1);
        // Image Adjustment
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, "Value", 50);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, "Value", 75);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_SATURATION, "Value", 120);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_HUE, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_HUE, "Value", 45);
        // Spatial Transforms
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_FLIP_HORIZONTAL, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_FLIP_VERTICAL, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_MIRROR, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_ROTATE, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_ROTATE, "Angle", 90);
        // Artistic Effects
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
        // Noise Reduction
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_CAST, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_ADAPTIVE, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DENOISE_MOSQUITO, 1);
        // Deinterlacing
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DEINTERLACE_BLEND, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DEINTERLACE_TRIANGLE, 1);
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_DEINTERLACE_CAVT, 1);
        // Overlays
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "Text", "Sample");
        effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO, 1);
        effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GRAPHICLOGO, "ImageFile", "logo.png");
        // To disable an effect:
        // effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_GREYSCALE, 0);
    }
}
```
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

        // Configure mixer
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Set output size
            mixer.SetOutputParam("Width", 1920);
            mixer.SetOutputParam("Height", 1080);
            mixer.SetOutputParam("FrameRate", 30.0);

            // Configure main video (input 0) - fullscreen
            mixer.SetInputParam(0, "X", 0);
            mixer.SetInputParam(0, "Y", 0);
            mixer.SetInputParam(0, "Width", 1920);
            mixer.SetInputParam(0, "Height", 1080);
            mixer.SetInputParam(0, "Alpha", 255);  // Fully opaque

            // Configure PIP video (input 1) - bottom-right corner
            mixer.SetInputParam(1, "X", 1440);    // 1920 - 480 = 1440
            mixer.SetInputParam(1, "Y", 810);     // 1080 - 270 = 810
            mixer.SetInputParam(1, "Width", 480); // 25% width
            mixer.SetInputParam(1, "Height", 270); // 25% height
            mixer.SetInputParam(1, "Alpha", 255);

            // Set Z-order (layering)
            mixer.SetInputOrder(new int[] { 0, 1 }); // Main behind, PIP on top
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
            mixer.SetOutputParam("Width", 1920);
            mixer.SetOutputParam("Height", 1080);
            mixer.SetOutputParam("FrameRate", 30.0);
            // Configure 2x2 grid layout
            int halfWidth = 960;   // 1920 / 2
            int halfHeight = 540;  // 1080 / 2
            // Top-left (Input 0)
            mixer.SetInputParam(0, "X", 0);
            mixer.SetInputParam(0, "Y", 0);
            mixer.SetInputParam(0, "Width", halfWidth);
            mixer.SetInputParam(0, "Height", halfHeight);
            // Top-right (Input 1)
            mixer.SetInputParam(1, "X", halfWidth);
            mixer.SetInputParam(1, "Y", 0);
            mixer.SetInputParam(1, "Width", halfWidth);
            mixer.SetInputParam(1, "Height", halfHeight);
            // Bottom-left (Input 2)
            mixer.SetInputParam(2, "X", 0);
            mixer.SetInputParam(2, "Y", halfHeight);
            mixer.SetInputParam(2, "Width", halfWidth);
            mixer.SetInputParam(2, "Height", halfHeight);
            // Bottom-right (Input 3)
            mixer.SetInputParam(3, "X", halfWidth);
            mixer.SetInputParam(3, "Y", halfHeight);
            mixer.SetInputParam(3, "Width", halfWidth);
            mixer.SetInputParam(3, "Height", halfHeight);
            // All inputs fully opaque
            for (int i = 0; i < 4; i++)
            {
                mixer.SetInputParam(i, "Alpha", 255);
            }
            // Set Z-order (all on same level)
            mixer.SetInputOrder(new int[] { 0, 1, 2, 3 });
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
        mixer.SetOutputParam("Width", 1920);
        mixer.SetOutputParam("Height", 1080);

        // Background (fullscreen)
        mixer.SetInputParam(0, "X", 0);
        mixer.SetInputParam(0, "Y", 0);
        mixer.SetInputParam(0, "Width", 1920);
        mixer.SetInputParam(0, "Height", 1080);

        // Foreground (centered, smaller)
        mixer.SetInputParam(1, "X", 480);
        mixer.SetInputParam(1, "Y", 270);
        mixer.SetInputParam(1, "Width", 960);
        mixer.SetInputParam(1, "Height", 540);

        // Enable chroma key for foreground input
        mixer.SetChromaSettings(
            inputIndex: 1,
            enabled: true,
            colorR: 0,      // Green screen
            colorG: 255,
            colorB: 0,
            threshold: 50,  // Tolerance
            blend: 10);     // Edge smoothing
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
        // Configure chroma key
        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            // Set key color (green)
            chromaKey.chroma_put_color(
                red: 0,
                green: 255,
                blue: 0);
            // Set contrast/threshold
            chromaKey.chroma_put_contrast(
                contrast1: 50,  // Lower threshold
                contrast2: 100); // Upper threshold
            // Set background image (optional)
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                chromaKey.chroma_put_image(backgroundImagePath);
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
    // Set green color (RGB)
    hr = pChromaKey->chroma_put_color(0, 255, 0);
    if (FAILED(hr)) goto cleanup;
    // Set thresholds
    hr = pChromaKey->chroma_put_contrast(40, 90);
    if (FAILED(hr)) goto cleanup;
    // Optional: Set background image
    hr = pChromaKey->chroma_put_image(L"C:\\backgrounds\\studio.jpg");
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
        // Set blue color
        chromaKey.chroma_put_color(
            red: 0,
            green: 0,
            blue: 255);

        // Fine-tuned thresholds for blue screen
        // Lower values = more strict (less tolerance)
        // Higher values = more loose (more tolerance)
        chromaKey.chroma_put_contrast(
            contrast1: 30,   // Tighter lower threshold
            contrast2: 80);  // Moderate upper threshold
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
        // Use any custom color
        chromaKey.chroma_put_color(
            red: keyColor.R,
            green: keyColor.G,
            blue: keyColor.B);
        // Standard thresholds
        chromaKey.chroma_put_contrast(50, 100);
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
            chromaKey.chroma_put_color(0, 255, 0);  // Green
            chromaKey.chroma_put_contrast(40, 90);
        }

        // 4. Add Video Effects filter
        var effectsFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Video Effects");

        var effects = effectsFilter as IVFEffects45;
        if (effects != null)
        {
            // Apply some effects
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, "Value", 60);

            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, 1);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_CONTRAST, "Value", 80);

            // Add text overlay
            effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, 1);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "Text", "LIVE");
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "FontSize", 48);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "X", 50);
            effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_TEXTLOGO, "Y", 50);
        }

        // 5. Add Video Mixer
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

            // Main video (fullscreen background)
            mixer.SetInputParam(0, "X", 0);
            mixer.SetInputParam(0, "Y", 0);
            mixer.SetInputParam(0, "Width", 1920);
            mixer.SetInputParam(0, "Height", 1080);

            // Chroma-keyed video (PIP)
            mixer.SetInputParam(1, "X", 1200);
            mixer.SetInputParam(1, "Y", 700);
            mixer.SetInputParam(1, "Width", 640);
            mixer.SetInputParam(1, "Height", 360);

            mixer.SetInputOrder(new int[] { 0, 1 });
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
**Solution**: Ensure effect is enabled and parameters are set:
```csharp
effects.SetEffect(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, 1);  // 1 = enabled
effects.SetEffectParam(VF_VIDEO_EFFECT.VF_VIDEO_EFFECT_BRIGHTNESS, "Value", 75);
```
### Issue: Chroma Key Not Working Well
**Solution**: Adjust contrast thresholds:
```csharp
// For difficult green screens:
chromaKey.chroma_put_contrast(30, 70);  // Tighter range
// For well-lit green screens:
chromaKey.chroma_put_contrast(50, 110);  // Wider range
```
### Issue: Video Mixer Inputs Not Showing
**Solution**: Verify input parameters and Z-order:
```csharp
// Ensure inputs are on-screen
mixer.SetInputParam(index, "X", 0);     // Must be >= 0
mixer.SetInputParam(index, "Y", 0);     // Must be >= 0
mixer.SetInputParam(index, "Width", 640);  // Must be > 0
mixer.SetInputParam(index, "Height", 480); // Must be > 0
// Check Z-order
mixer.SetInputOrder(new int[] { 0, 1, 2 });  // 0 = back, 2 = front
```
---

## See Also

### Documentation

- [Effects Reference](effects-reference.md) - Complete effects list
- [Video Mixer Interface](interfaces/video-mixer.md) - Full API reference
- [Chroma Key Interface](interfaces/chroma-key.md) - Complete interface docs

### External Resources

- [DirectShow Filter Graph](https://learn.microsoft.com/en-us/windows/win32/directshow/building-the-filter-graph)
