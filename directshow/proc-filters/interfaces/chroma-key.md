---
title: Processing Filters - Chroma Key Interface
description: IVFChromaKey interface for green screen and blue screen compositing with tolerance control and background replacement in DirectShow.
---

# IVFChromaKey Interface Reference

## Overview

The `IVFChromaKey` interface provides professional chroma key (green screen/blue screen) compositing capabilities for DirectShow applications. This interface enables real-time background replacement by making specific colors transparent, allowing subjects filmed in front of colored backdrops to be composited over different backgrounds.

Chroma keying is essential for virtual production, weather forecasting, video effects, and any scenario where background replacement is needed.

## Interface Definition

- **Interface Name**: `IVFChromaKey`
- **GUID**: `{AF6E8208-30E3-44f0-AAFE-787A6250BAB3}`
- **Inherits From**: `IUnknown`
- **Header File**: `vf_eff_intf.h` (C++), `IVFChromaKey.cs` (.NET)

## Capabilities

- **Color Keys**: Green, blue, red, or custom RGB colors
- **Contrast Adjustment**: Separate low/high contrast thresholds
- **Background Replacement**: Static image or video background
- **Real-Time Processing**: Hardware-accelerated when available
- **Edge Quality**: Adjustable tolerance for smooth edges

---
## Methods Reference
### Contrast Threshold Configuration
#### chroma_put_contrast
Sets the contrast threshold range for chroma keying.
**Syntax (C++)**:
```cpp
HRESULT chroma_put_contrast(int low, int high);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int chroma_put_contrast(int low, int high);
```
**Parameters**:
- `low`: Low contrast threshold (0-255)
  - Lower values = remove more similar colors
  - Higher values = stricter color matching
- `high`: High contrast threshold (0-255)
  - Defines the upper bound for color matching
  - Creates a range of acceptable key colors
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- These values define the color similarity range for keying
- The range between `low` and `high` creates a gradient for edge smoothing
- Typical ranges:
  - Tight keying: low=10, high=30
  - Standard keying: low=30, high=70
  - Loose keying: low=50, high=120
- Adjust based on lighting conditions and backdrop quality
**How It Works**:
```
Pixels with chroma distance < low    → Fully transparent
Pixels with chroma distance > high   → Fully opaque
Pixels between low and high          → Partially transparent (gradient)
```
**Example (C++)**:
```cpp
IVFChromaKey* pChroma = nullptr;
pFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChroma);
// Standard green screen configuration
pChroma->chroma_put_contrast(40, 80);
pChroma->Release();
```
**Example (C#)**:
```csharp
var chroma = filter as IVFChromaKey;
if (chroma != null)
{
    // Tight keying for clean green screen
    chroma.chroma_put_contrast(20, 50);
}
```
---

### Color Selection

#### chroma_put_color

Sets the chroma key color to be made transparent.

**Syntax (C++)**:
```cpp
HRESULT chroma_put_color(int color);
```

**Syntax (C#)**:
```csharp
[PreserveSig]
int chroma_put_color(int color);
```

**Parameters**:
- `color`: Chroma key color value

**Color Values** (CVFChromaColor enumeration):

| Value | Color | RGB Equivalent | Use Case |
|-------|-------|----------------|----------|
| `0` (Chroma_Green) | Green | 0x00FF00 | Standard chroma key (most common) |
| `1` (Chroma_Blue) | Blue | 0x0000FF | Alternative to green |
| `2` (Chroma_Red) | Red | 0xFF0000 | Special cases |
| Custom RGB | Any color | 0xRRGGBB | Specific color matching |

**Returns**: `S_OK` (0) on success.

**Usage Notes**:
- Green is standard for chroma keying (human skin has least green)
- Blue used when green objects are in scene
- Can use custom RGB value for specific color matching
- Color should be uniform across backdrop for best results

**Example (C++)**:
```cpp
// Use green chroma key
pChroma->chroma_put_color(Chroma_Green);

// Use blue chroma key
pChroma->chroma_put_color(Chroma_Blue);

// Use custom color (e.g., magenta)
pChroma->chroma_put_color(0xFF00FF);
```

**Example (C#)**:
```csharp
// Standard green screen
chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);

// Blue screen
chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);

// Custom yellow-green
chroma.chroma_put_color(0x88FF00);
```

---
### Background Image
#### chroma_put_image
Sets a replacement background image for transparent areas.
**Syntax (C++)**:
```cpp
HRESULT chroma_put_image(BSTR filename);
```
**Syntax (C#)**:
```csharp
[PreserveSig]
int chroma_put_image([MarshalAs(UnmanagedType.BStr)] string filename);
```
**Parameters**:
- `filename`: Path to background image file (BMP, PNG, JPG, etc.)
**Returns**: `S_OK` (0) on success.
**Usage Notes**:
- Image is stretched to fill entire frame
- Use NULL or empty string to use video background instead
- Static image is more efficient than video background
- Image is loaded once and cached
- Supported formats: BMP, PNG, JPEG, GIF, TIFF
**Example (C++)**:
```cpp
// Set office background image
pChroma->chroma_put_image(L"C:\\Backgrounds\\office.jpg");
// Remove background image (use video input instead)
pChroma->chroma_put_image(NULL);
```
**Example (C#)**:
```csharp
// Virtual studio background
chroma.chroma_put_image(@"C:\Backgrounds\studio.png");
// Remove static background
chroma.chroma_put_image(null);
```
---

## Complete Configuration Examples

### Example 1: Basic Green Screen Setup (C++)

```cpp
#include "vf_eff_intf.h"

HRESULT ConfigureBasicGreenScreen(IBaseFilter* pChromaFilter)
{
    HRESULT hr;
    IVFChromaKey* pChroma = nullptr;

    hr = pChromaFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChroma);
    if (FAILED(hr))
        return hr;

    // Set green as key color
    pChroma->chroma_put_color(Chroma_Green);

    // Standard contrast thresholds
    pChroma->chroma_put_contrast(40, 80);

    // Set background image
    pChroma->chroma_put_image(L"C:\\Backgrounds\\office_background.jpg");

    pChroma->Release();
    return S_OK;
}
```

### Example 2: Weather Forecast Studio (C#)

```csharp
using System;
using VisioForge.DirectShowAPI;

public class WeatherStudioSetup
{
    public void ConfigureWeatherChromaKey(IBaseFilter chromaFilter)
    {
        var chroma = chromaFilter as IVFChromaKey;
        if (chroma == null)
            throw new NotSupportedException("IVFChromaKey not available");

        // Blue screen for weather maps
        chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);

        // Tighter thresholds for clean keying
        chroma.chroma_put_contrast(25, 60);

        // Weather map background
        chroma.chroma_put_image(@"C:\Weather\maps\current_radar.png");
    }

    public void UpdateWeatherMap(IVFChromaKey chroma, string mapPath)
    {
        // Dynamically update background during broadcast
        chroma.chroma_put_image(mapPath);
    }
}
```

### Example 3: Virtual Production with Custom Color (C++)

```cpp
HRESULT ConfigureVirtualProduction(IVFChromaKey* pChroma)
{
    // Use specific green matching your physical backdrop
    // Measure actual color with color picker
    COLORREF customGreen = RGB(60, 220, 40);  // Specific green shade

    pChroma->chroma_put_color(customGreen);

    // Professional-grade thresholds
    // Lower values for clean, well-lit green screen
    pChroma->chroma_put_contrast(15, 45);

    // Use pre-rendered virtual environment
    pChroma->chroma_put_image(L"D:\\VirtualSets\\studio_environment.png");

    return S_OK;
}
```

### Example 4: Adaptive Chroma Key Settings (C#)

```csharp
public class AdaptiveChromaKey
{
    private IVFChromaKey _chroma;

    public void SetupForLightingConditions(string condition)
    {
        switch (condition.ToLower())
        {
            case "perfect":
                // Clean, evenly lit green screen
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(15, 40);
                break;

            case "good":
                // Standard lighting
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(30, 70);
                break;

            case "challenging":
                // Uneven lighting or wrinkled backdrop
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(50, 110);
                break;

            case "outdoor":
                // Natural light, harder to control
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);
                _chroma.chroma_put_contrast(60, 130);
                break;
        }
    }

    public void TestThresholds()
    {
        // Start with tight keying
        for (int low = 10; low <= 60; low += 10)
        {
            int high = low + 40;
            _chroma.chroma_put_contrast(low, high);

            // User reviews result and selects best setting
            Console.WriteLine($"Testing: Low={low}, High={high}");
            System.Threading.Thread.Sleep(2000);
        }
    }
}
```

---
## Chroma Keying Best Practices
### Lighting Setup
1. **Even Illumination**
   - Use multiple light sources
   - Avoid hotspots and shadows on backdrop
   - Maintain consistent color across entire screen
2. **Subject Separation**
   - Position subject 6-10 feet from backdrop
   - Prevents green spill on subject
   - Allows independent lighting control
3. **Backdrop Quality**
   - Use proper chroma key fabric or paint
   - Keep backdrop wrinkle-free
   - Maintain consistent color saturation
### Configuration Strategy
1. **Start Conservative**
   ```cpp
   // Begin with tight thresholds
   pChroma->chroma_put_contrast(20, 50);
   // Gradually increase if needed
   pChroma->chroma_put_contrast(30, 70);
   ```
2. **Test Different Lighting**
   - Adjust thresholds for your specific setup
   - Save presets for different conditions
   - Document working values
3. **Color Selection**
   - Green: Standard choice (least in skin tones)
   - Blue: When green objects in scene
   - Custom: Match actual backdrop color for best results
### Quality Optimization
1. **Camera Settings**
   - Disable auto white balance
   - Manual focus
   - Reduce sharpening (prevents edge artifacts)
2. **Threshold Tuning**
   - Low value: Controls transparency threshold
   - High value: Controls edge softness
   - Wider range = softer edges
3. **Edge Quality**
   ```
   Tight range (low=20, high=40):
   - Sharp edges
   - May show green fringe
   - Best for clean backdrops
   Wide range (low=30, high=90):
   - Softer edges
   - Better color bleeding tolerance
   - More forgiving of imperfect lighting
   ```
---

## Common Chroma Key Scenarios

### Scenario 1: Corporate Video Production

```cpp
// Clean studio environment
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(25, 55);
pChroma->chroma_put_image(L"corporate_office.jpg");
```

**Characteristics**:
- Controlled lighting
- Professional green screen
- Static office background
- High quality requirements

### Scenario 2: Gaming Streamer

```cpp
// Home studio setup
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(35, 75);
pChroma->chroma_put_image(NULL);  // Use game video as background
```

**Characteristics**:
- Consumer green screen
- Variable lighting
- Dynamic video background
- Real-time performance critical

### Scenario 3: Weather Broadcasting

```cpp
// Blue screen with weather maps
pChroma->chroma_put_color(Chroma_Blue);
pChroma->chroma_put_contrast(30, 65);
pChroma->chroma_put_image(L"weather_map_current.png");
```

**Characteristics**:
- Blue screen (green used in weather maps)
- Dynamically changing backgrounds
- Professional lighting
- Presenter clothing considerations

### Scenario 4: Virtual Event Hosting

```cpp
// Virtual conference background
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(40, 85);
pChroma->chroma_put_image(L"conference_hall.jpg");
```

**Characteristics**:
- Home/office setup
- Variable quality backdrops
- Forgiving settings needed
- Professional appearance desired

---
## Troubleshooting
### Issue: Green Spill on Subject
**Symptoms**: Green halo or tint on subject edges
**Solutions**:
1. Increase subject distance from backdrop
2. Adjust lighting to reduce reflection
3. Use tighter contrast range:
   ```cpp
   pChroma->chroma_put_contrast(15, 35);
   ```
4. Consider color correction in post
### Issue: Uneven Keying
**Symptoms**: Parts of backdrop not transparent
**Solutions**:
1. Check backdrop lighting uniformity
2. Increase high threshold:
   ```cpp
   pChroma->chroma_put_contrast(30, 100);
   ```
3. Verify backdrop color consistency
4. Consider using custom color matching:
   ```cpp
   // Sample actual backdrop color and use it
   pChroma->chroma_put_color(0x40DC28);  // Measured color
   ```
### Issue: Subject Parts Disappearing
**Symptoms**: Subject clothing or features becoming transparent
**Solutions**:
1. Avoid green/blue clothing
2. Reduce contrast range:
   ```cpp
   pChroma->chroma_put_contrast(50, 90);
   ```
3. Switch key color if needed:
   ```cpp
   pChroma->chroma_put_color(Chroma_Blue);  // If wearing green
   ```
### Issue: Rough, Jagged Edges
**Symptoms**: Poor edge quality, visible pixelation
**Solutions**:
1. Widen contrast range for smoother gradient:
   ```cpp
   pChroma->chroma_put_contrast(25, 85);
   ```
2. Improve lighting quality
3. Use higher quality video source
4. Ensure subject is well-separated from backdrop
### Issue: Performance Problems
**Symptoms**: Dropped frames, stuttering
**Solutions**:
1. Use static image background instead of video
2. Reduce output resolution
3. Optimize threshold values (don't make too wide)
4. Consider hardware-accelerated alternatives
---

## Parameter Reference Table

### Contrast Threshold Guidelines

| Lighting Quality | Backdrop | Low | High | Edge Quality | Performance |
|-----------------|----------|-----|------|--------------|-------------|
| **Excellent** | Clean, even | 15-25 | 35-50 | Sharp | Best |
| **Good** | Minor variations | 25-35 | 50-75 | Good | Good |
| **Fair** | Some unevenness | 35-50 | 75-100 | Soft | Moderate |
| **Poor** | Uneven/wrinkled | 50-70 | 100-140 | Very soft | Lower |

### Color Selection Guide

| Color | RGB | Pros | Cons | Best For |
|-------|-----|------|------|----------|
| **Green** | 0x00FF00 | Least in skin, bright | Not for green objects | General use |
| **Blue** | 0x0000FF | Alternative to green | Denim/blue clothing | Special cases |
| **Custom** | Varies | Exact match to backdrop | Requires calibration | Professional |

---
## Integration with Video Mixer
Chroma key filter is often used with Video Mixer for advanced compositing:
```cpp
// Chroma key filter removes green
IVFChromaKey* pChroma = /* ... */;
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(30, 70);
// Video mixer combines subject with background
IVFVideoMixer* pMixer = /* ... */;
// Input 0: Background video
// Input 1: Chroma-keyed subject (transparent background)
```
See [Video Mixer Interface](video-mixer.md) for details.
---

## Related Interfaces

- **IVFVideoMixer** - Combine chroma-keyed video with backgrounds
- **IVFEffects45** - Additional video effects
- **IVFEffectsPro** - Advanced effect processing

## See Also

- [Processing Filters Pack Overview](../index.md)
- [Video Mixer Interface](video-mixer.md)
- [Effects Reference](../effects-reference.md)
- [Code Examples](../examples.md)
