---
title: Processing Filters - Video Mixer Interface
description: IVFVideoMixer interface for mixing 2-16 video sources with PIP, chroma keying, transparency, and customizable layout configurations.
---

# IVFVideoMixer Interface Reference

## Overview

The `IVFVideoMixer` interface provides comprehensive control over multi-source video mixing in DirectShow applications. This interface enables Picture-in-Picture (PIP), video compositing, chroma keying, and flexible layout management for combining multiple video streams into a single output.

The Video Mixer filter can handle 2-16 input video sources, each with independent position, size, transparency, and z-order configuration.

## Interface Definition

- **Interface Name**: `IVFVideoMixer`
- **GUID**: `{3318300E-F6F1-4d81-8BC3-9DB06B09F77A}`
- **Inherits From**: `IUnknown`
- **Header File**: `yk_video_mixer_filter_define.h` (C++), `IVFVideoMixer.cs` (.NET)

## Capabilities

- **Input Pins**: 2-16 simultaneous video sources
- **Chroma Keying**: Green/blue screen support per input
- **Resize Quality**: Multiple interpolation algorithms
- **Z-Order**: Independent layering control
- **Transparency**: Per-input alpha blending
- **Position/Size**: Pixel-accurate placement

---
## Methods Reference
### Input Parameter Management
#### SetInputParam
Configures parameters for a specific input pin.
**Syntax (C++)**:
```cpp
int SetInputParam(int pin_index, VFPIPVideoInputParam param);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetInputParam([In] int pin_index, [In] VFPIPVideoInputParam param);
```
**Parameters**:
- `pin_index`: Zero-based input pin index (0 = first input, 1 = second, etc.)
- `param`: Structure containing input configuration (see below)
**Returns**: `0` on success, error code otherwise.
**VFPIPVideoInputParam Structure**:
| Field | Type | Description |
|-------|------|-------------|
| `Enabled` | bool | Enable/disable this input |
| `Left` | int | X position (pixels) |
| `Top` | int | Y position (pixels) |
| `Width` | int | Width (pixels) |
| `Height` | int | Height (pixels) |
| `Alpha` | int | Transparency (0-255, 255=opaque) |
| `Visible` | bool | Visibility flag |
| `ZOrder` | int | Layer order (higher = foreground) |
| `StretchMode` | VFPIPResizeQuality | Resize quality |
**Usage Notes**:
- Pin 0 is typically the background/main source
- Pins 1+ are overlay sources
- Position (0,0) is top-left corner
- Size can differ from source resolution (automatic scaling)
- Alpha blending requires some GPU overhead
**Example (C++)**:
```cpp
IVFVideoMixer* pMixer = nullptr;
pFilter->QueryInterface(IID_IVFVideoMixer, (void**)&pMixer);
// Configure second input (overlay)
VFPIPVideoInputParam param;
param.Enabled = true;
param.Visible = true;
param.Left = 50;
param.Top = 50;
param.Width = 640;
param.Height = 360;
param.Alpha = 255;        // Fully opaque
param.ZOrder = 10;        // On top of background
pMixer->SetInputParam(1, param);
pMixer->Release();
```
**Example (C#)**:
```csharp
var mixer = filter as IVFVideoMixer;
// Configure PIP in bottom-right corner
var param = new VFPIPVideoInputParam
{
    Enabled = true,
    Visible = true,
    Left = 1600,          // Assuming 1920x1080 output
    Top = 820,
    Width = 320,          // Small PIP
    Height = 180,
    Alpha = 255,
    ZOrder = 100          // Top layer
};
mixer.SetInputParam(1, param);
```
---

#### GetInputParam

Retrieves current parameters for a specific input pin.

**Syntax (C++)**:
```cpp
int GetInputParam(int pin_index, VFPIPVideoInputParam *param);
```

**Syntax (C#)**:
```csharp
[PreserveSig]
int GetInputParam([In] int pin_index, [Out] out VFPIPVideoInputParam param);
```

**Parameters**:
- `pin_index`: Zero-based input pin index
- `param`: [out] Receives current input configuration

**Returns**: `0` on success.

**Example (C++)**:
```cpp
VFPIPVideoInputParam param;
pMixer->GetInputParam(1, &param);

printf("Input 1 position: %d,%d\n", param.Left, param.Top);
printf("Input 1 size: %dx%d\n", param.Width, param.Height);
```

---
#### GetInputParam2
Retrieves parameters by pin interface reference instead of index.
**Syntax (C++)**:
```cpp
int GetInputParam2(IPin *pin, VFPIPVideoInputParam *param);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int GetInputParam2([In] object pin, [Out] out VFPIPVideoInputParam param);
```
**Parameters**:
- `pin`: DirectShow IPin interface pointer
- `param`: [out] Receives input configuration
**Returns**: `0` on success.
**Usage Notes**:
- Alternative to GetInputParam when you have pin reference
- Useful when enumerating pins dynamically
---

### Output Configuration

#### SetOutputParam

Configures the mixer's output video format.

**Syntax (C++)**:
```cpp
int SetOutputParam(VFPIPVideoOutputParam param);
```

**Syntax (C#)**:
```csharp
[PreserveSig]
int SetOutputParam([In] VFPIPVideoOutputParam param);
```

**Parameters**:
- `param`: Output configuration structure

**VFPIPVideoOutputParam Structure**:

| Field | Type | Description |
|-------|------|-------------|
| `Width` | int | Output width (pixels) |
| `Height` | int | Output height (pixels) |
| `FrameRate` | double | Output frame rate (fps) |
| `BackgroundColor` | COLORREF | Background color (RGB) |

**Usage Notes**:
- Must be called before connecting downstream filters
- All inputs are scaled/positioned relative to output size
- Frame rate can differ from inputs (mixer handles timing)

**Example (C++)**:
```cpp
VFPIPVideoOutputParam output;
output.Width = 1920;
output.Height = 1080;
output.FrameRate = 30.0;
output.BackgroundColor = RGB(0, 0, 0);  // Black background

pMixer->SetOutputParam(output);
```

**Example (C#)**:
```csharp
var output = new VFPIPVideoOutputParam
{
    Width = 1280,
    Height = 720,
    FrameRate = 60.0,
    BackgroundColor = 0x003300  // Dark green
};

mixer.SetOutputParam(output);
```

---
#### GetOutputParam
Retrieves current output configuration.
**Syntax (C++)**:
```cpp
int GetOutputParam(VFPIPVideoOutputParam *param);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int GetOutputParam([Out] out VFPIPVideoOutputParam param);
```
**Parameters**:
- `param`: [out] Receives output configuration
**Returns**: `0` on success.
---

### Chroma Key Configuration

#### SetChromaSettings

Configures chroma key (green/blue screen) settings for compositing.

**Syntax (C++)**:
```cpp
int SetChromaSettings(bool enabled, int color, int tolerance1, int tolerance2);
```

**Syntax (C#)**:
```csharp
[PreserveSig]
int SetChromaSettings([In, MarshalAs(UnmanagedType.Bool)] bool enabled,
                      int color,
                      int tolerance1,
                      int tolerance2);
```

**Parameters**:
- `enabled`: Enable/disable chroma keying
- `color`: Key color (0=green, 1=blue, 2=red, or custom RGB)
- `tolerance1`: Color matching tolerance (0-255)
- `tolerance2`: Edge tolerance (0-255)

**Returns**: `0` on success.

**Usage Notes**:
- Applies to all inputs that have chroma color
- Lower tolerance = stricter color matching
- Higher tolerance = more colors removed (may affect subject)
- tolerance2 helps with edge smoothing

**Chroma Color Values**:
- `0` - Green (most common)
- `1` - Blue
- `2` - Red
- Custom RGB value

**Example (C++)**:
```cpp
// Enable green screen with moderate tolerance
pMixer->SetChromaSettings(true, 0, 50, 30);
```

**Example (C#)**:
```csharp
// Blue screen with tight tolerance
mixer.SetChromaSettings(true, 1, 30, 20);

// Disable chroma keying
mixer.SetChromaSettings(false, 0, 0, 0);
```

---
### Layer Order Management
#### SetInputOrder
Sets the z-order (layer order) for a specific input.
**Syntax (C++)**:
```cpp
int SetInputOrder(int pin_index, int order);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int SetInputOrder(int pin_index, int order);
```
**Parameters**:
- `pin_index`: Zero-based input pin index
- `order`: Z-order value (higher = foreground)
**Returns**: `0` on success.
**Usage Notes**:
- Higher order values render on top
- Typical range: 0-100
- Can be changed dynamically during playback
- Alternative to setting ZOrder in VFPIPVideoInputParam
**Example (C++)**:
```cpp
// Background
pMixer->SetInputOrder(0, 0);
// Middle layer
pMixer->SetInputOrder(1, 50);
// Top overlay
pMixer->SetInputOrder(2, 100);
```
---

### Quality Configuration

#### SetResizeQuality

Sets the resize quality/algorithm for all inputs.

**Syntax (C++)**:
```cpp
int SetResizeQuality(VFPIPResizeQuality quality);
```

**Syntax (C#)**:
```csharp
[PreserveSig]
int SetResizeQuality(VFPIPResizeQuality quality);
```

**Parameters**:
- `quality`: Resize quality mode

**VFPIPResizeQuality Enumeration**:

| Value | Algorithm | Quality | Speed | Use Case |
|-------|-----------|---------|-------|----------|
| **NearestNeighbor** | Nearest pixel | Low | ★★★★★ | Pixel art, fast preview |
| **Bilinear** | Linear interpolation | Medium | ★★★★☆ | Standard quality |
| **Bicubic** | Cubic interpolation | High | ★★★☆☆ | High quality (default) |
| **Lanczos** | Lanczos-3 | Highest | ★★☆☆☆ | Professional quality |

**Usage Notes**:
- Bicubic is recommended for most use cases
- Lanczos for maximum quality when performance allows
- Bilinear for real-time performance
- NearestNeighbor only for special cases

**Example (C++)**:
```cpp
// High quality mixing
pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);

// Performance mode
pMixer->SetResizeQuality(VFPIPResizeQuality::Bilinear);
```

---
## Complete Configuration Examples
### Example 1: Picture-in-Picture (C++)
```cpp
#include "yk_video_mixer_filter_define.h"
HRESULT ConfigurePIPLayout(IBaseFilter* pMixerFilter)
{
    HRESULT hr;
    IVFVideoMixer* pMixer = nullptr;
    hr = pMixerFilter->QueryInterface(IID_IVFVideoMixer, (void**)&pMixer);
    if (FAILED(hr))
        return hr;
    // Set 1080p output
    VFPIPVideoOutputParam output;
    output.Width = 1920;
    output.Height = 1080;
    output.FrameRate = 30.0;
    output.BackgroundColor = RGB(0, 0, 0);
    pMixer->SetOutputParam(output);
    // Configure main video (input 0 - background)
    VFPIPVideoInputParam main;
    main.Enabled = true;
    main.Visible = true;
    main.Left = 0;
    main.Top = 0;
    main.Width = 1920;
    main.Height = 1080;
    main.Alpha = 255;
    main.ZOrder = 0;        // Background
    pMixer->SetInputParam(0, main);
    // Configure PIP (input 1 - bottom-right corner)
    VFPIPVideoInputParam pip;
    pip.Enabled = true;
    pip.Visible = true;
    pip.Left = 1560;        // 1920 - 360 (width) + margin
    pip.Top = 860;          // 1080 - 220 (height) + margin
    pip.Width = 360;
    pip.Height = 202;       // 16:9 aspect
    pip.Alpha = 255;
    pip.ZOrder = 100;       // Foreground
    pMixer->SetInputParam(1, pip);
    // High quality resize
    pMixer->SetResizeQuality(VFPIPResizeQuality::Bicubic);
    pMixer->Release();
    return S_OK;
}
```
### Example 2: Split Screen (C#)
```csharp
using VisioForge.DirectShowAPI;
public class SplitScreenMixer
{
    public void ConfigureSplitScreen(IBaseFilter mixerFilter)
    {
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer == null)
            throw new NotSupportedException("IVFVideoMixer not available");
        // 1920x1080 output
        var output = new VFPIPVideoOutputParam
        {
            Width = 1920,
            Height = 1080,
            FrameRate = 30.0,
            BackgroundColor = 0x000000
        };
        mixer.SetOutputParam(output);
        // Left half - Input 0
        var leftInput = new VFPIPVideoInputParam
        {
            Enabled = true,
            Visible = true,
            Left = 0,
            Top = 0,
            Width = 960,        // Half width
            Height = 1080,
            Alpha = 255,
            ZOrder = 0
        };
        mixer.SetInputParam(0, leftInput);
        // Right half - Input 1
        var rightInput = new VFPIPVideoInputParam
        {
            Enabled = true,
            Visible = true,
            Left = 960,         // Offset by half
            Top = 0,
            Width = 960,
            Height = 1080,
            Alpha = 255,
            ZOrder = 0
        };
        mixer.SetInputParam(1, rightInput);
        mixer.SetResizeQuality(VFPIPResizeQuality.Bicubic);
    }
}
```
### Example 3: Chroma Key Overlay (C++)
```cpp
HRESULT ConfigureChromaKeyOverlay(IVFVideoMixer* pMixer)
{
    // 1080p output
    VFPIPVideoOutputParam output;
    output.Width = 1920;
    output.Height = 1080;
    output.FrameRate = 30.0;
    output.BackgroundColor = RGB(0, 0, 0);
    pMixer->SetOutputParam(output);
    // Background scene (input 0)
    VFPIPVideoInputParam background;
    background.Enabled = true;
    background.Visible = true;
    background.Left = 0;
    background.Top = 0;
    background.Width = 1920;
    background.Height = 1080;
    background.Alpha = 255;
    background.ZOrder = 0;
    pMixer->SetInputParam(0, background);
    // Person in front of green screen (input 1)
    VFPIPVideoInputParam subject;
    subject.Enabled = true;
    subject.Visible = true;
    subject.Left = 400;
    subject.Top = 100;
    subject.Width = 1120;
    subject.Height = 880;
    subject.Alpha = 255;
    subject.ZOrder = 10;
    pMixer->SetInputParam(1, subject);
    // Enable green screen chroma keying
    pMixer->SetChromaSettings(
        true,   // Enable
        0,      // Green
        60,     // Color tolerance
        40      // Edge tolerance
    );
    // High quality for best chroma key edges
    pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);
    return S_OK;
}
```
### Example 4: Multi-Camera Grid (C#)
```csharp
public void Configure2x2Grid(IVFVideoMixer mixer)
{
    // 1920x1080 output
    var output = new VFPIPVideoOutputParam
    {
        Width = 1920,
        Height = 1080,
        FrameRate = 30.0,
        BackgroundColor = 0x101010  // Dark gray
    };
    mixer.SetOutputParam(output);
    int cellWidth = 960;
    int cellHeight = 540;
    int gap = 10;
    // Top-left camera (input 0)
    mixer.SetInputParam(0, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = gap,
        Top = gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    // Top-right camera (input 1)
    mixer.SetInputParam(1, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = cellWidth + gap,
        Top = gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    // Bottom-left camera (input 2)
    mixer.SetInputParam(2, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = gap,
        Top = cellHeight + gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    // Bottom-right camera (input 3)
    mixer.SetInputParam(3, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = cellWidth + gap,
        Top = cellHeight + gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    mixer.SetResizeQuality(VFPIPResizeQuality.Bicubic);
}
```
---

## Common Mixing Scenarios

### Scenario 1: News Broadcast Style

```
+------------------------------------------+
|                                          |
|        Main Camera (full screen)         |
|                                          |
|                           +-----------+  |
|                           |   Guest   |  |
|                           |  Camera   |  |
|                           +-----------+  |
+------------------------------------------+
```

**Configuration**:
- Input 0: Main camera (1920x1080)
- Input 1: Guest PIP (320x180, bottom-right)
- Z-Order: Guest on top
- Resize Quality: Bicubic

### Scenario 2: Gaming Stream

```
+------------------------------------------+
|                                          |
|        Game Capture (main)               |
|                                          |
|  +----------+                            |
|  | Webcam   |                            |
|  +----------+                            |
+------------------------------------------+
```

**Configuration**:
- Input 0: Game capture (1920x1080)
- Input 1: Webcam (280x210, top-left)
- Optional: Chroma key if webcam has green screen
- Z-Order: Webcam on top

### Scenario 3: Virtual Production

```
+------------------------------------------+
|                                          |
|    Background Scene (pre-rendered)       |
|                                          |
|         [Person with green screen        |
|          composited on top]              |
|                                          |
+------------------------------------------+
```

**Configuration**:
- Input 0: Virtual background
- Input 1: Camera with green screen
- Chroma key: Enabled, green, tolerance 60/40
- Resize Quality: Lanczos for best edge quality

---
## Performance Considerations
### CPU/GPU Usage
**Low Impact Configurations**:
- 2-4 inputs
- Bilinear resize
- No chroma keying
- No transparency (Alpha = 255)
**Medium Impact**:
- 5-8 inputs
- Bicubic resize
- Basic chroma keying
- Some transparency
**High Impact**:
- 9+ inputs
- Lanczos resize
- Complex chroma keying
- Multiple transparent layers
### Optimization Tips
1. **Use appropriate resize quality**:
   - Preview: Bilinear
   - Production: Bicubic
   - Maximum quality: Lanczos (if performance allows)
2. **Minimize chroma keying overhead**:
   - Only enable when needed
   - Use tight tolerance values
   - Consider hardware-accelerated alternative
3. **Limit number of inputs**:
   - Each input adds processing overhead
   - Disable unused inputs (Enabled = false)
4. **Match source resolutions**:
   - Less scaling = better performance
   - Pre-scale sources if possible
---

## Best Practices

### Layout Design

1. **Plan z-order carefully** - Background lowest, overlays highest
2. **Leave margins** - Don't position elements at exact edges
3. **Maintain aspect ratios** - Avoid distortion
4. **Test at target resolution** - Verify positioning accuracy

### Chroma Keying

1. **Proper lighting** - Even lighting on green screen
2. **Adjust tolerance** - Start low, increase gradually
3. **Quality setting** - Use Lanczos for best edges
4. **Test conditions** - Different lighting scenarios

### Dynamic Changes

1. **Update parameters smoothly** - Avoid abrupt position changes
2. **Cache configurations** - Store presets for quick switching
3. **Validate parameters** - Check bounds before applying
4. **Handle errors** - Check return values

---
## Troubleshooting
### Issue: Video Not Appearing
**Check**:
- `Enabled = true`
- `Visible = true`
- `Alpha > 0`
- Position within output bounds
- Source filter is running
### Issue: Poor Quality Scaling
**Solution**:
```cpp
pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);
```
### Issue: Chroma Key Not Working
**Check**:
- Chroma settings enabled
- Correct color selected (0=green, 1=blue)
- Increase tolerance values
- Verify source has uniform green screen
**Example**:
```cpp
// Try higher tolerance
pMixer->SetChromaSettings(true, 0, 80, 60);
```
### Issue: Performance Problems
**Solutions**:
- Reduce number of active inputs
- Use faster resize quality
- Disable chroma keying if not needed
- Pre-scale input sources
---

## Related Interfaces

- **IBaseFilter** - DirectShow filter interface
- **IPin** - DirectShow pin interface (for GetInputParam2)
- **IVFEffects45** - Video effects (can combine with mixer)
- **IVFChromaKey** - Dedicated chroma key interface

## See Also

- [Processing Filters Pack Overview](../index.md)
- [Effects Reference](../effects-reference.md)
- [Chroma Key Interface](chroma-key.md)
- [Code Examples](../examples.md)
