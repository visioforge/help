---
title: Processing Filters - Effects Reference
description: Complete reference for 35+ DirectShow video effects including color filters, deinterlacing, denoising, and artistic effects.
---

# Video Effects Complete Reference

## Overview

The DirectShow Processing Filters Pack provides 35+ real-time video effects that can be applied individually or chained together. This reference documents all available effects, their parameters, and usage.

## Effect Categories

- **Text and Graphics** - Text logos, graphic overlays
- **Color Filters** - Red, green, blue, greyscale filters
- **Image Adjustment** - Brightness, contrast, saturation
- **Spatial Effects** - Flip, mirror, rotate
- **Artistic Effects** - Marble, solarize, posterize, mosaic
- **Noise and Quality** - Denoising algorithms (CAST, adaptive, mosquito)
- **Deinterlacing** - Blend, triangle, CAVT methods
- **Creative Effects** - Blur, shake, spray, invert

---
## Text and Graphics Effects
### ef_text_logo
Renders text overlay on video with extensive customization options.
**Effect Type**: `CVFEffectType.ef_text_logo`
**Parameters** (`CVFTextLogoMain` structure):
| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `x` | int | X position (pixels) | 0 |
| `y` | int | Y position (pixels) | 0 |
| `text` | BSTR | Text to display | "" |
| `font_name` | BSTR | Font family name | "Arial" |
| `font_size` | int | Font size (points) | 12 |
| `font_color` | COLORREF | Text color (RGB) | 0xFFFFFF (white) |
| `font_italic` | BOOL | Italic style | FALSE |
| `font_bold` | BOOL | Bold style | FALSE |
| `font_underline` | BOOL | Underline style | FALSE |
| `font_strikeout` | BOOL | Strikethrough style | FALSE |
| `transparent_bg` | BOOL | Transparent background | TRUE |
| `bg_color` | COLORREF | Background color | 0x000000 (black) |
| `transp` | DWORD | Transparency level (0-255) | 255 (opaque) |
| `align` | CVFTextAlign | Text alignment | `al_left` |
| `antialiasing` | CVFTextAntialiasingMode | Anti-aliasing mode | `am_AntiAlias` |
| `gradient` | BOOL | Enable gradient | FALSE |
| `gradientMode` | CVFTextGradientMode | Gradient direction | `gm_horizontal` |
| `gradientColor1` | COLORREF | Gradient start color | 0xFFFFFF |
| `gradientColor2` | COLORREF | Gradient end color | 0x000000 |
| `borderMode` | CVFTextBorderMode | Border/outline style | `bm_none` |
| `innerBorderColor` | COLORREF | Inner border color | 0x000000 |
| `outerBorderColor` | COLORREF | Outer border color | 0xFFFFFF |
| `innerBorderSize` | int | Inner border width | 1 |
| `outerBorderSize` | int | Outer border width | 1 |
| `DateMode` | BOOL | Display current date/time | FALSE |
| `DateMask` | BSTR | Date format string | "" |
**Text Alignment Options**:
- `al_left` - Left-aligned
- `al_center` - Center-aligned
- `al_right` - Right-aligned
**Border Modes**:
- `bm_none` - No border
- `bm_inner` - Inner outline
- `bm_outer` - Outer outline
- `bm_inner_and_outer` - Both sides
- `bm_embossed` - 3D embossed effect
- `bm_outline` - Standard outline
- `bm_filled_outline` - Solid outline
- `bm_halo` - Glow effect
**Example (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_text_logo;
effect.Enabled = TRUE;
effect.TextLogo.x = 10;
effect.TextLogo.y = 10;
effect.TextLogo.text = SysAllocString(L"Live Stream");
effect.TextLogo.font_name = SysAllocString(L"Arial");
effect.TextLogo.font_size = 32;
effect.TextLogo.font_color = RGB(255, 255, 255);
effect.TextLogo.font_bold = TRUE;
effect.TextLogo.borderMode = bm_outline;
effect.TextLogo.outerBorderColor = RGB(0, 0, 0);
effect.TextLogo.outerBorderSize = 2;
pEffects->add_effect(effect);
```
---

### ef_graphic_logo

Overlays an image (logo, watermark) on video.

**Effect Type**: `CVFEffectType.ef_graphic_logo`

**Parameters** (`CVFGraphicLogoMain` structure):

| Parameter | Type | Description |
|-----------|------|-------------|
| `x` | UINT32 | X position (pixels) |
| `y` | UINT32 | Y position (pixels) |
| `Filename` | BSTR | Path to image file (BMP, PNG, JPG) |
| `hBmp` | int | Handle to bitmap (alternative to filename) |
| `StretchMode` | CVFStretchMode | How to scale image |
| `TranspLevel` | int | Transparency level (0-255) |
| `UseColorKey` | BOOL | Enable color key transparency |
| `ColorKey` | COLORREF | Color to make transparent |

**Stretch Modes**:
- `sm_none` - Original size
- `sm_stretch` - Stretch to fit
- `sm_letterbox` - Fit with aspect ratio
- `sm_crop` - Crop to fit

**Example (C#)**:
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_graphic_logo,
    Enabled = true,
    GraphicLogo = new CVFGraphicLogoMain
    {
        Filename = @"C:\Images\logo.png",
        x = 20,
        y = 20,
        StretchMode = (int)CVFStretchMode.sm_none,
        TranspLevel = 200,
        UseColorKey = false
    }
};

effectsInterface.add_effect(effect);
```

---
## Color Filter Effects
### ef_blue
Applies blue color filter (enhances blue, reduces other colors).
**Effect Type**: `CVFEffectType.ef_blue`
**Parameters**:
- `pAmountI` - Filter intensity (0-100, default: 50)
**Use Cases**:
- Artistic blue tint
- Cold atmosphere
- Water/ocean scenes
---

### ef_green

Applies green color filter.

**Effect Type**: `CVFEffectType.ef_green`

**Parameters**:
- `pAmountI` - Filter intensity (0-100)

**Use Cases**:
- Night vision effect
- Forest/nature scenes
- Matrix-style effect

---
### ef_red
Applies red color filter.
**Effect Type**: `CVFEffectType.ef_red`
**Parameters**:
- `pAmountI` - Filter intensity (0-100)
**Use Cases**:
- Warm atmosphere
- Sunset effect
- Alert/danger scenes
---

### ef_filter_blue / ef_filter_blue_2

Advanced blue filtering with different algorithms.

**Effect Type**: `CVFEffectType.ef_filter_blue` or `ef_filter_blue_2`

**Difference**: `ef_filter_blue_2` uses alternative color math for different visual results.

---
### ef_filter_green / ef_filter_green2
Advanced green filtering (two variants).
**Effect Types**: `CVFEffectType.ef_filter_green`, `ef_filter_green2`
---

### ef_filter_red / ef_filter_red2

Advanced red filtering (two variants).

**Effect Types**: `CVFEffectType.ef_filter_red`, `ef_filter_red2`

---
### ef_greyscale
Converts video to black and white.
**Effect Type**: `CVFEffectType.ef_greyscale`
**Parameters**: None (full greyscale conversion)
**Use Cases**:
- Classic film look
- Artistic effect
- Reduce color noise
**Example (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_greyscale;
effect.Enabled = TRUE;
pEffects->add_effect(effect);
```
---

### ef_invert

Inverts all colors (negative image).

**Effect Type**: `CVFEffectType.ef_invert`

**Parameters**: None

**Use Cases**:
- Artistic effect
- X-ray appearance
- Special visual effects

---
## Image Adjustment Effects
### ef_contrast
Adjusts image contrast.
**Effect Type**: `CVFEffectType.ef_contrast`
**Parameters**:
- `pAmountI` - Contrast adjustment (-100 to +100)
  - Negative: Decrease contrast
  - Positive: Increase contrast
  - Default: 0 (no change)
**Example (C#)**:
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_contrast,
    Enabled = true,
    pAmountI = 25  // Increase contrast by 25%
};
```
---

### ef_lightness

Adjusts overall brightness.

**Effect Type**: `CVFEffectType.ef_lightness`

**Parameters**:
- `pAmountI` - Brightness adjustment (-100 to +100)
  - Negative: Darken
  - Positive: Brighten
  - Default: 0

---
### ef_darkness
Darkens the image (opposite of lightness).
**Effect Type**: `CVFEffectType.ef_darkness`
**Parameters**:
- `pAmountI` - Darkness amount (0-100)
---

### ef_saturation

Adjusts color saturation.

**Effect Type**: `CVFEffectType.ef_saturation`

**Parameters**:
- `pAmountI` - Saturation adjustment (-100 to +100)
  - -100: Greyscale
  - 0: Original colors
  - +100: Hyper-saturated

**Use Cases**:
- Vivid colors for promotional content
- Desaturate for muted look
- Color grading

---
## Spatial Effects
### ef_flip_down
Flips video vertically (upside down).
**Effect Type**: `CVFEffectType.ef_flip_down`
**Parameters**: None
**Use Cases**:
- Correct upside-down camera
- Mirror effect with rotation
- Special effects
---

### ef_flip_right

Flips video horizontally (mirror).

**Effect Type**: `CVFEffectType.ef_flip_right`

**Parameters**: None

**Use Cases**:
- Webcam mirror mode
- Correct mirrored camera
- Symmetry effects

---
### ef_mirror_down
Creates vertical mirror effect (top reflects to bottom).
**Effect Type**: `CVFEffectType.ef_mirror_down`
---

### ef_mirror_right

Creates horizontal mirror effect (left reflects to right).

**Effect Type**: `CVFEffectType.ef_mirror_right`

---
## Artistic Effects
### ef_blur
Applies Gaussian blur to the image.
**Effect Type**: `CVFEffectType.ef_blur`
**Parameters**:
- `pAmountI` - Blur amount (0-100)
- `pSizeI` - Blur kernel size (1-20)
**Use Cases**:
- Background blur (depth of field simulation)
- Soften image
- Privacy (blur faces, license plates)
**Example (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_blur;
effect.Enabled = TRUE;
effect.pAmountI = 50;  // 50% blur strength
effect.pSizeI = 10;    // 10-pixel blur radius
pEffects->add_effect(effect);
```
---

### ef_marble

Creates marble/swirl texture effect.

**Effect Type**: `CVFEffectType.ef_marble`

**Parameters**:
- `pAmountD` - Effect intensity (0.0-1.0)
- `pTurbulenceI` - Turbulence amount (0-100)
- `pScaleD` - Scale factor (0.1-10.0)

**Use Cases**:
- Artistic background
- Transition effects
- Psychedelic visuals

---
### ef_posterize
Reduces number of colors (poster art effect).
**Effect Type**: `CVFEffectType.ef_posterize`
**Parameters**:
- `pAmountI` - Color levels (2-256)
  - Lower values: Fewer colors, more dramatic
  - Higher values: More colors, subtle effect
**Use Cases**:
- Pop art style
- Comic book effect
- Reduce color depth
---

### ef_mosaic

Creates pixelated/mosaic effect.

**Effect Type**: `CVFEffectType.ef_mosaic`

**Parameters**:
- `pSizeI` - Mosaic block size (2-100 pixels)

**Use Cases**:
- Privacy (blur faces/identities)
- Retro pixel art style
- Censorship

**Example (C#)**:
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_mosaic,
    Enabled = true,
    pSizeI = 15  // 15x15 pixel blocks
};
```

---
### ef_solarize
Creates solarization effect (partial color inversion).
**Effect Type**: `CVFEffectType.ef_solorize` (note spelling)
**Parameters**:
- `pAmountI` - Solarization threshold (0-255)
**Use Cases**:
- Artistic photography effect
- Retro look
- Creative transitions
---

### ef_spray

Creates paint spray/splatter effect.

**Effect Type**: `CVFEffectType.ef_spray`

**Parameters**:
- `pAmountI` - Spray intensity (0-100)

---
### ef_shake_down
Simulates camera shake effect vertically.
**Effect Type**: `CVFEffectType.ef_shake_down`
**Parameters**:
- `pAmountI` - Shake intensity (0-100)
**Use Cases**:
- Earthquake effect
- Impact vibration
- Handheld camera simulation
---

## Noise Processing Effects

### ef_denoise_cast

CAST (Combined Adaptive Spatial-Temporal) denoising algorithm.

**Effect Type**: `CVFEffectType.ef_denoise_cast`

**Parameters** (`CVFDenoiseCAST` structure):

| Parameter | Range | Default | Description |
|-----------|-------|---------|-------------|
| `TemporalDifferenceThreshold` | 0-255 | 16 | Motion detection threshold |
| `NumberOfMotionPixelsThreshold` | 0-16 | 0 | Min pixels for motion |
| `StrongEdgeThreshold` | 0-255 | 8 | Edge preservation |
| `BlockWidth` | 1-16 | 4 | Processing block width |
| `BlockHeight` | 1-16 | 4 | Processing block height |
| `EdgePixelWeight` | 0-255 | 128 | Edge blending weight |
| `NonEdgePixelWeight` | 0-255 | 16 | Smooth area weight |
| `GaussianThresholdY` | int | 12 | Luma noise threshold |
| `GaussianThresholdUV` | int | 6 | Chroma noise threshold |
| `HistoryWeight` | 0-255 | 192 | Temporal filtering strength |

**Use Cases**:
- Low-light video cleanup
- Webcam noise reduction
- Compression artifact removal

**Example (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_denoise_cast;
effect.Enabled = TRUE;

// Moderate noise reduction
effect.DenoiseCAST.TemporalDifferenceThreshold = 20;
effect.DenoiseCAST.StrongEdgeThreshold = 10;
effect.DenoiseCAST.GaussianThresholdY = 15;
effect.DenoiseCAST.GaussianThresholdUV = 8;

pEffects->add_effect(effect);
```

---
### ef_denoise_adaptive
Adaptive noise reduction that adjusts to image content.
**Effect Type**: `CVFEffectType.ef_denoise_adaptive`
**Parameters**:
- `pDenoiseAdaptiveThreshold` - Noise threshold (0-100)
- `pDenoiseAdaptiveBlurMode` - Blur method (0-2)
**Use Cases**:
- General noise reduction
- Video cleanup
- Quality enhancement
---

### ef_denoise_mosquito

Reduces mosquito noise (compression artifacts around edges).

**Effect Type**: `CVFEffectType.ef_denoise_mosquito`

**Parameters**:
- `pAmountI` - Reduction strength (0-100)

**Use Cases**:
- Clean up heavily compressed video
- Remove MPEG/H.264 artifacts
- Post-processing for streaming

---
### ef_color_noise
Adds color noise (grain) to image.
**Effect Type**: `CVFEffectType.ef_color_noise`
**Parameters**:
- `pAmountI` - Noise amount (0-100)
**Use Cases**:
- Film grain effect
- Retro/vintage look
- Artistic texture
---

### ef_mono_noise

Adds monochrome (black & white) noise.

**Effect Type**: `CVFEffectType.ef_mono_noise`

**Parameters**:
- `pAmountI` - Noise amount (0-100)

---
## Deinterlacing Effects
### ef_deint_blend
Blends interlaced fields together.
**Effect Type**: `CVFEffectType.ef_deint_blend`
**Parameters** (`CVFDeintBlend` structure):
| Parameter | Range | Default | Description |
|-----------|-------|---------|-------------|
| `blendThresh1` | 0-255 | 5 | First blend threshold |
| `blendThresh2` | 0-255 | 9 | Second blend threshold |
| `blendConstants1` | 0.0-1.0 | 0.3 | First blend weight |
| `blendConstants2` | 0.0-1.0 | 0.7 | Second blend weight |
**Use Cases**:
- Deinterlace analog video
- Remove comb artifacts
- Convert interlaced to progressive
---

### ef_deint_triangle

Triangle interpolation deinterlacing.

**Effect Type**: `CVFEffectType.ef_deint_triangle`

**Parameters**:
- `pDeintTriangleWeight` - Interpolation weight (0-100)

**Quality**: Better edge preservation than blend

---
### ef_deint_cavt
CAVT (Content Adaptive Vertical Temporal) deinterlacing.
**Effect Type**: `CVFEffectType.ef_deint_cavt`
**Parameters**:
- `pDeintCAVTThreshold` - Motion threshold (0-100)
**Quality**: Best quality, most CPU intensive
**Use Cases**:
- High-quality deinterlacing
- Broadcast video conversion
- Archival processing
---

## Effect Chaining

Multiple effects can be applied simultaneously. Effects are processed in the order they were added.

**Example: Professional Stream Enhancement**:
```cpp
// 1. Denoise
CVFEffect denoise;
denoise.Type = ef_denoise_adaptive;
denoise.Enabled = TRUE;
denoise.pDenoiseAdaptiveThreshold = 15;
pEffects->add_effect(denoise);

// 2. Color correction
CVFEffect saturation;
saturation.Type = ef_saturation;
saturation.Enabled = TRUE;
saturation.pAmountI = 15;  // Slight saturation boost
pEffects->add_effect(saturation);

// 3. Add branding
CVFEffect logo;
logo.Type = ef_graphic_logo;
logo.Enabled = TRUE;
logo.GraphicLogo.Filename = SysAllocString(L"logo.png");
logo.GraphicLogo.x = 20;
logo.GraphicLogo.y = 20;
pEffects->add_effect(logo);

// 4. Add timestamp
CVFEffect timestamp;
timestamp.Type = ef_text_logo;
timestamp.Enabled = TRUE;
timestamp.TextLogo.DateMode = TRUE;
timestamp.TextLogo.DateMask = SysAllocString(L"%Y-%m-%d %H:%M:%S");
timestamp.TextLogo.x = 20;
timestamp.TextLogo.y = 1050;  // Bottom left
pEffects->add_effect(timestamp);
```

---
## Performance Considerations
### CPU Usage by Effect
**Low Impact** (< 5% CPU):
- Color filters
- Greyscale
- Invert
- Flip/Mirror
**Medium Impact** (5-15% CPU):
- Text/graphic overlays
- Contrast/brightness
- Posterize
- Simple deinterlacing
**High Impact** (15-40% CPU):
- Blur (large radius)
- Denoise (CAST, adaptive)
- Mosaic (small blocks)
- Marble/artistic effects
### Optimization Tips
1. **Limit simultaneous effects** - Each effect adds processing time
2. **Use appropriate parameters** - Larger blur radius = more CPU
3. **Disable unused effects** - Set `Enabled = FALSE` instead of removing
4. **Process at lower resolution** - Downscale, apply effects, upscale
5. **Use GPU rendering when possible** - Check for GPU-accelerated effects
---

## Common Effect Combinations

### Webcam Enhancement
```
1. ef_denoise_adaptive (threshold: 15)
2. ef_contrast (amount: +10)
3. ef_saturation (amount: +15)
4. ef_flip_right (mirror mode)
```

### Vintage Film Look
```
1. ef_greyscale
2. ef_contrast (amount: +20)
3. ef_mono_noise (amount: 15)
```

### Broadcast Quality
```
1. ef_deint_cavt
2. ef_denoise_mosquito (amount: 20)
3. ef_saturation (amount: +5)
```

### Privacy Mode
```
1. ef_mosaic (size: 20) on specific region
2. ef_blur (amount: 80) as alternative
```

---
## See Also
- [Processing Filters Pack Overview](index.md)
- [Video Effects Interface Reference](interfaces/effects-interface.md)
- [Chroma Key Interface](interfaces/chroma-key.md)
- [Video Mixer Interface](interfaces/video-mixer.md)
- [Code Examples](examples.md)