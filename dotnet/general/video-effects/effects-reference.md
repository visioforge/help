---
title: Video Effects SDK Reference: Filters, Overlays, and More
description: Comprehensive reference guide for all video effects available in VisioForge .NET SDKs including Classic (Windows-only) and Cross-platform effects.
sidebar_label: Effects Reference
---

# Complete Video Effects Reference

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

This document provides a comprehensive reference for all video effects available in VisioForge .NET SDKs. Effects are available in two distinct implementations:

### Effect Types by SDK Engine

#### Classic Effects (Windows Only)
- **Available in**: VideoCaptureCore, MediaPlayerCore, VideoEditCore
- **Platform**: Windows only (.NET Framework 4.7.2+ and .NET 6-10)
- **Types**: 
  - `VideoEffect*` - CPU-based processing
  - `GPUVideoEffect*` - GPU-accelerated (DirectX)
  - `Maxine*` - NVIDIA AI-powered effects (RTX GPUs)
- **Location**: `VisioForge.Core.Types.VideoEffects` namespace

#### Cross-Platform Effects
- **Available in**: VideoCaptureCoreX, MediaPlayerCoreX, VideoEditCoreX, Media Blocks SDK
- **Platform**: Windows, Linux, macOS, Android, iOS
- **Types**:
  - `*VideoEffect` classes in `VisioForge.Core.Types.X.VideoEffects`
  - Cross-platform processing for universal compatibility
- **Location**: `VisioForge.Core.Types.X.VideoEffects` namespace

> **Important**: When choosing effects, select based on your target SDK and platform requirements. Cross-platform effects provide broader compatibility, while Classic effects offer Windows-specific optimizations and DirectX GPU acceleration.

## Effect Categories

The following sections list all available effects. Each effect includes SDK availability markers:
- 🪟 **Classic** = VideoCaptureCore/MediaPlayerCore/VideoEditCore (Windows only)
- 🌍 **Cross-platform** = VideoCaptureCoreX/MediaPlayerCoreX/VideoEditCoreX/Media Blocks (Cross-platform)

### Color Adjustment Effects

#### Brightness and Darkness

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectDarkness | 🪟 Classic | Adjusts overall darkness/lightness. Values above 128 darken, values below lighten the image. |
| GPUVideoEffectDarkness | 🪟 Classic (GPU) | GPU-accelerated darkness adjustment. |
| GPUVideoEffectBrightness | 🪟 Classic (GPU) | GPU-accelerated brightness adjustment. Adds or subtracts uniform values to RGB components. |
| VideoEffectLightness | 🪟 Classic | Adjusts lightness in HSL color space, preserving hue and saturation relationships. |
| VideoBalanceVideoEffect | 🌍 Cross-platform | Cross-platform brightness, contrast, saturation, and hue adjustment. |

#### Contrast and Color

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectContrast | 🪟 Classic | Adjusts the difference between light and dark areas. Supports animated transitions. |
| GPUVideoEffectContrast | 🪟 Classic (GPU) | GPU-accelerated contrast adjustment. |
| VideoEffectSaturation | 🪟 Classic | Controls color intensity from grayscale (0) to full saturation (255). |
| GPUVideoEffectSaturation | 🪟 Classic (GPU) | GPU-accelerated saturation adjustment. |
| VideoBalanceVideoEffect | 🌍 Cross-platform | Provides contrast and saturation control (cross-platform). |
| GammaVideoEffect | 🌍 Cross-platform | Gamma correction for brightness curve adjustment. |

### Color Filters and Conversions

#### Grayscale and Monochrome

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectGrayscale | 🪟 Classic | Converts color video to black and white using perceptual luminance weights. |
| GPUVideoEffectGrayscale | 🪟 Classic (GPU) | GPU-accelerated grayscale conversion. |
| GPUVideoEffectMonoChrome | 🪟 Classic (GPU) | Creates monochrome effect with customizable tint color. |
| GrayscaleVideoEffect | 🌍 Cross-platform | Cross-platform grayscale conversion using videobalance element. |
| ColorEffectsVideoEffect | 🌍 Cross-platform | Various color presets including monochrome, sepia, heat map, and more. |

#### Color Inversion

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectInvert | 🪟 Classic | Inverts all RGB color values, creating photographic negative effect. |
| GPUVideoEffectInvert | 🪟 Classic (GPU) | GPU-accelerated color inversion. |

#### Color Channel Filters

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectRed | 🪟 Classic | Isolates red color channel, removing green and blue components. |
| VideoEffectGreen | 🪟 Classic | Isolates green color channel, removing red and blue components. |
| VideoEffectBlue | 🪟 Classic | Isolates blue color channel, removing red and green components. |
| VideoEffectFilterRed | 🪟 Classic | Applies red color filter effect with adjustable intensity. |
| VideoEffectFilterGreen | 🪟 Classic | Applies green color filter effect with adjustable intensity. |
| VideoEffectFilterBlue | 🪟 Classic | Applies blue color filter effect with adjustable intensity. |

#### Color Grading

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectLUT | 🪟 Classic | Look-up table color grading using 3D LUT files for professional color correction. |
| LUTVideoEffect | 🌍 Cross-platform | Cross-platform 3D LUT support with multiple interpolation modes. |
| VideoEffectPosterize | 🪟 Classic | Reduces number of colors, creating poster-like effect with discrete color levels. |
| VideoEffectSolorize | 🪟 Classic | Solarization effect that inverts colors above a threshold brightness level. |

### Image Enhancement

#### Sharpening and Blur

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectSharpen | 🪟 Classic | Enhances edges and fine details for crisper images. |
| GPUVideoEffectSharpen | 🪟 Classic (GPU) | GPU-accelerated sharpening. |
| VideoEffectBlur | 🪟 Classic | Applies smoothing filter, softening image and reducing detail. |
| GPUVideoEffectBlur | 🪟 Classic (GPU) | GPU-accelerated blur. |
| GPUVideoEffectDirectionalBlur | 🪟 Classic (GPU) | Blur with directional component for motion blur effects. |
| VideoEffectSmooth | 🪟 Classic | Softening filter for noise reduction and gentle image smoothing. |
| GaussianBlurVideoEffect | 🌍 Cross-platform | Cross-platform Gaussian blur with adjustable sigma. |
| SmoothVideoEffect | 🌍 Cross-platform | Cross-platform smoothing/softening filter. |

#### Noise Reduction

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectDenoiseAdaptive | 🪟 Classic | Adaptive noise reduction that preserves edges while removing noise. |
| VideoEffectDenoiseCAST | 🪟 Classic | CAST (Cellular Automata-based Spatio-Temporal) denoise algorithm with multiple parameters. |
| VideoEffectDenoiseMosquito | 🪟 Classic | Reduces mosquito noise artifacts common in compressed video. |
| GPUVideoEffectDenoise | 🪟 Classic (GPU) | GPU-accelerated general noise reduction. |

### Geometric Transformations

#### Rotation and Flipping

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectRotate | 🪟 Classic | Rotates video by any angle with options for stretching or preserving full frame. |
| FlipRotateVideoEffect | 🌍 Cross-platform | Cross-platform flip and rotation (90°, 180°, 270°, horizontal/vertical flip). |
| VideoEffectFlipHorizontal | 🪟 Classic | Flips video horizontally (left-right mirror). |
| VideoEffectFlipVertical | 🪟 Classic | Flips video vertically (top-bottom mirror). |
| VideoEffectMirrorHorizontal | 🪟 Classic | Creates horizontal mirror effect at center of frame. |
| VideoEffectMirrorVertical | 🪟 Classic | Creates vertical mirror effect at center of frame. |

#### Scaling and Transformation

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectZoom | 🪟 Classic | Zooms into specific region of video with pan and scale control. |
| VideoEffectPan | 🪟 Classic | Pan and crop effect for selecting specific video region. |
| ResizeVideoEffect | 🌍 Cross-platform | Cross-platform video scaling/resizing with multiple interpolation methods. |
| CropVideoEffect | 🌍 Cross-platform | Cross-platform video cropping to specific dimensions. |
| AspectRatioCropVideoEffect | 🌍 Cross-platform | Automatically crops video to target aspect ratio. |
| BoxVideoEffect | 🌍 Cross-platform | Adds letterboxing/pillarboxing with custom fill color. |

### Artistic and Stylistic Effects

#### Classic Film Effects

| Effect | SDK | Description |
|--------|-----|-------------|
| GPUVideoEffectOldMovie | 🪟 Classic (GPU) | Vintage film effect with grain, scratches, and sepia tone. |
| GPUVideoEffectNightVision | 🪟 Classic (GPU) | Night vision camera simulation with green phosphor look. |
| AgingVideoEffect | 🌍 Cross-platform | Cross-platform aging/vintage film effect. |
| OpticalAnimationBWVideoEffect | 🌍 Cross-platform | Optical illusion animation effects in black and white. |

#### Edge and Texture Effects

| Effect | SDK | Description |
|--------|-----|-------------|
| GPUVideoEffectEmboss | 🪟 Classic (GPU) | Creates embossed or raised relief effect highlighting edges. |
| GPUVideoEffectContour | 🪟 Classic (GPU) | Edge detection and contour enhancement. |
| GPUVideoEffectPixelate | 🪟 Classic (GPU) | Pixelation effect with adjustable block size. |
| VideoEffectMosaic | 🪟 Classic | Creates mosaic pattern with adjustable tile size. |
| EdgeVideoEffect | 🌍 Cross-platform | Cross-platform edge detection filter. |
| DiceVideoEffect | 🌍 Cross-platform | Creates dice/cubist effect by dividing image into rotated squares. |

#### Special Distortion Effects (Cross-platform only)

The following artistic effects are available exclusively in the cross-platform implementation:

| Effect | Description |
|--------|-------------|
| FishEyeVideoEffect | Fish-eye lens distortion effect. |
| TwirlVideoEffect | Twirl/swirl distortion effect. |
| BulgeVideoEffect | Bulge/magnification distortion. |
| StretchVideoEffect | Stretch distortion effect. |
| TunnelVideoEffect | Tunnel perspective effect. |
| SphereVideoEffect | Spherical warping effect. |
| SquareVideoEffect | Square warping effect. |
| CircleVideoEffect | Circular warping effect. |
| KaleidoscopeVideoEffect | Kaleidoscope mirror effect. |
| MarbleVideoEffect | Marble texture effect. |
| PinchVideoEffect | Pinch distortion effect. |
| Pseudo3DVideoEffect | Pseudo 3D stereo effect. |
| QuarkVideoEffect | Quark particle effect. |
| RippleVideoEffect | Water ripple effect. |
| WaterRippleVideoEffect | Enhanced water ripple with multiple modes. |
| WarpVideoEffect | General warp distortion effect. |
| DiffuseVideoEffect | Diffuse/blur spreading effect. |
| MovingBlurVideoEffect | Motion blur with directional control. |
| MovingEchoVideoEffect | Motion echo/trail effect. |
| MovingZoomEchoVideoEffect | Combined zoom and echo effect. |
| SMPTEVideoEffect | SMPTE transition effects (wipes, fades). |
| SMPTEAlphaVideoEffect | SMPTE transitions with alpha channel. |

#### Special Effects

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectColorNoise | 🪟 Classic | Adds random color noise for grain or interference effects. |
| VideoEffectMonoNoise | 🪟 Classic | Adds monochrome (grayscale) noise. |
| VideoEffectSpray | 🪟 Classic | Spray paint or pointillist effect. |
| VideoEffectShakeDown | 🪟 Classic | Vertical shake effect for impact or earthquake simulation. |

### Deinterlacing

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectDeinterlaceBlend | 🪟 Classic | Blends interlaced fields for progressive output. |
| GPUVideoEffectDeinterlaceBlend | 🪟 Classic (GPU) | GPU-accelerated deinterlace blend. |
| VideoEffectDeinterlaceCAVT | 🪟 Classic | Content Adaptive Vertical Temporal deinterlacing. |
| VideoEffectDeinterlaceTriangle | 🪟 Classic | Triangle interpolation deinterlacing method. |
| DeinterlaceVideoEffect | 🌍 Cross-platform | Cross-platform deinterlacing with multiple methods (linear, greedy, vfir, yadif, etc.). |
| AutoDeinterlaceSettings | 🌍 Cross-platform | Automatic deinterlacing when interlaced content is detected. |
| InterlaceSettings | 🌍 Cross-platform | Creates interlaced output from progressive content. |

### Transition Effects

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectFadeIn | 🪟 Classic | Gradual fade from black to video content. |
| VideoEffectFadeOut | 🪟 Classic | Gradual fade from video content to black. |

### Overlays and Graphics

#### Text Overlays

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectTextLogo | 🪟 Classic | Flexible text overlay with extensive customization including fonts, colors, rotation, effects, and animated text. |
| VideoEffectScrollingTextLogo | 🪟 Classic | Scrolling text banner with direction and speed control. |
| TextOverlayVideoEffect | 🌍 Cross-platform | Cross-platform text overlay with advanced typography control. Supports timestamps, system time, and dynamic text. |
| OverlayManagerText | 🌍 Cross-platform | Advanced text overlay with shadows, gradients, and animations. |
| OverlayManagerScrollingText | 🌍 Cross-platform | Scrolling text with full control over speed, direction, and appearance. |
| OverlayManagerDateTime | 🌍 Cross-platform | Date/time overlay with customizable formatting. |

See: [Text Overlay Guide](text-overlay.md)

#### Image Overlays

| Effect | SDK | Description |
|--------|-----|-------------|
| VideoEffectImageLogo | 🪟 Classic | Image overlay supporting PNG, JPG, BMP, animated GIF, with transparency and positioning control. |
| ImageOverlayVideoEffect | 🌍 Cross-platform | Cross-platform image overlay with positioning and alpha blending. |
| ImageOverlayCairoVideoEffect | 🌍 Cross-platform | Advanced image overlay using Cairo graphics with transformations. |
| OverlayManagerImage | 🌍 Cross-platform | Professional image overlay with animations, transitions, and effects. |
| OverlayManagerGIF | 🌍 Cross-platform | Animated GIF overlay support. |
| SVGOverlayVideoEffect | 🌍 Cross-platform | SVG vector graphics overlay. |
| QRCodeOverlayFilter | 🌍 Cross-platform | QR code generation and overlay. |

See: [Image Overlay Guide](image-overlay.md)

#### Advanced Overlay Features (Cross-platform only)

| Feature | Description |
|---------|-------------|
| OverlayManagerVideo | Video-in-video overlay (picture-in-picture). |
| OverlayManagerDecklinkVideo | Blackmagic Decklink video source overlay. |
| OverlayManagerNDIVideo | NDI video source overlay. |
| OverlayManagerGroup | Group multiple overlays together for synchronized control. |
| OverlayManagerPan | Pan overlay animation. |
| OverlayManagerZoom | Zoom overlay animation. |
| OverlayManagerFade | Fade in/out overlay animation. |
| OverlayManagerSqueezeback | Squeezeback effect (shrink main video to show background). |
| OverlayManagerBackgroundImage | Background image layer. |
| OverlayManagerBackgroundRectangle | Colored rectangle background. |
| OverlayManagerBackgroundSquare | Colored square background. |
| OverlayManagerBackgroundStar | Star-shaped background. |
| OverlayManagerBackgroundTriangle | Triangle-shaped background. |
| OverlayManagerCircle | Circular shape overlay. |
| OverlayManagerLine | Line drawing overlay. |
| OverlayManagerRectangle | Rectangle shape overlay. |
| OverlayManagerStar | Star shape overlay. |
| OverlayManagerTriangle | Triangle shape overlay. |

### Chroma Key and Motion Detection (Cross-platform only)

| Effect | Description |
|--------|-------------|
| ChromaKeySettings | Green screen / blue screen keying for background removal. |
| MotionDetectionProcessor | Real-time motion detection with configurable sensitivity. |

### 360° Video and VR (Classic only)

| Effect | SDK | Description |
|--------|-----|-------------|
| GPUVideoEffectEquirectangular360 | 🪟 Classic (GPU) | Processes equirectangular 360° video format. |
| GPUVideoEffectEquiangularCubemap360 | 🪟 Classic (GPU) | Converts between equiangular and cubemap projections for 360° video. |
| GPUVideoEffectVR360Base | 🪟 Classic (GPU) | Base class for VR 360° video effects. |

### AI-Powered Effects (NVIDIA Maxine - Classic only)

Requires NVIDIA RTX GPU with Maxine SDK support.

#### Video Enhancement

| Effect | Description |
|--------|-------------|
| MaxineDenoiseVideoEffect | AI-powered noise reduction that intelligently preserves details. |
| MaxineArtifactReductionVideoEffect | Reduces compression artifacts and video degradation using AI. |
| MaxineSuperResolutionEffect | AI upscaling for resolution enhancement (implementation varies by version). |

#### Content Effects

| Effect | Description |
|--------|-------------|
| MaxineAIGSVideoEffect | AI-powered green screen/background removal without requiring actual green screen. |
| MaxineUpscaleVideoEffect | Advanced upscaling using AI for improved quality. |

#### Special Effects

- **VideoEffectColorNoise** - Adds random color noise for grain or interference effects.
- **VideoEffectMonoNoise** - Adds monochrome (grayscale) noise.
- **VideoEffectSpray** - Spray paint or pointillist effect.
- **VideoEffectShakeDown** - Vertical shake effect for impact or earthquake simulation.

### Deinterlacing

- **VideoEffectDeinterlaceBlend** / **GPUVideoEffectDeinterlaceBlend** - Blends interlaced fields for progressive output.
- **VideoEffectDeinterlaceCAVT** - Content Adaptive Vertical Temporal deinterlacing.
- **VideoEffectDeinterlaceTriangle** - Triangle interpolation deinterlacing method.

### Transition Effects

#### Fade Effects

- **VideoEffectFadeIn** - Gradual fade from black to video content.
- **VideoEffectFadeOut** - Gradual fade from video content to black.

### Overlays and Graphics

#### Text Overlays

- **VideoEffectTextLogo** - Flexible text overlay with extensive customization including fonts, colors, rotation, effects, and animated text.
- **VideoEffectScrollingTextLogo** - Scrolling text banner with direction and speed control.

See: [Text Overlay Guide](text-overlay.md)

#### Image Overlays

- **VideoEffectImageLogo** - Image overlay supporting PNG, JPG, BMP, animated GIF, with transparency and positioning control.

See: [Image Overlay Guide](image-overlay.md)

### 360° Video and VR

- **GPUVideoEffectEquirectangular360** - Processes equirectangular 360° video format.
- **GPUVideoEffectEquiangularCubemap360** - Converts between equiangular and cubemap projections for 360° video.
- **GPUVideoEffectVR360Base** - Base class for VR 360° video effects.

### AI-Powered Effects (NVIDIA Maxine)

Requires NVIDIA RTX GPU with Maxine SDK support.

#### Video Enhancement

- **MaxineDenoiseVideoEffect** - AI-powered noise reduction that intelligently preserves details.
- **MaxineArtifactReductionVideoEffect** - Reduces compression artifacts and video degradation using AI.
- **MaxineSuperResolutionEffect** - AI upscaling for resolution enhancement (implementation varies by version).

#### Content Effects

- **MaxineAIGSVideoEffect** - AI-powered green screen/background removal without requiring actual green screen.
- **MaxineUpscaleVideoEffect** - Advanced upscaling using AI for improved quality.

## Effect Parameters

### Common Parameters

All video effects support these standard parameters:

- **Enabled** - Whether the effect is active (can be toggled on/off)
- **Name** - Identifier for retrieving specific effect instances
- **StartTime** - When effect begins (TimeSpan.Zero = from start)
- **StopTime** - When effect ends (TimeSpan.Zero = until end)

### Classic Effects Value Ranges

Most Classic adjustment effects use 0-255 value ranges where:
- **128** typically represents neutral/no change
- **0** represents minimum (often darkest/weakest)
- **255** represents maximum (often brightest/strongest)

### Animated Transitions (Classic Effects)

Many Classic effects support **ValueStop** parameter for smooth animation:
- Set **Value** for starting value
- Set **ValueStop** for ending value
- Define **StartTime** and **StopTime** for animation duration

### Cross-platform Effects Parameters

Cross-platform effects use multimedia element properties with varying ranges. Refer to individual effect documentation for specific parameter details.

## SDK Comparison

### Classic Effects (VideoCaptureCore/MediaPlayerCore)

- **Platform**: Windows only (.NET Framework 4.7.2+ and .NET 6-10)
- **Processing Types**:
  - CPU-based: `VideoEffect*` classes
  - GPU-accelerated: `GPUVideoEffect*` classes (DirectX)
  - AI-powered: `Maxine*` classes (NVIDIA RTX required)
- **Performance**: Optimized for Windows, DirectX GPU acceleration
- **Compatibility**: Desktop Windows only
- **Namespace**: `VisioForge.Core.Types.VideoEffects`

### Cross-platform Effects (VideoCaptureCoreX/Media Blocks)

- **Platform**: Cross-platform (Windows, Linux, macOS, Android, iOS)
- **Processing**: Cross-platform, hardware acceleration through multimedia plugins
- **Performance**: Good performance on all platforms, GPU acceleration varies by platform
- **Compatibility**: Universal - desktop and mobile
- **Namespace**: `VisioForge.Core.Types.X.VideoEffects`
- **Additional Features**: 
  - More artistic effects (distortions, warps)
  - Advanced overlay system (OverlayManager)
  - Chroma key / motion detection
  - Better suited for mobile platforms

## Usage Examples

### Classic Effect Application

```csharp
// CPU-based effect
var effect = new VideoEffectGrayscale(
    enabled: true,
    name: "BWEffect"
);
capture.Video_Effects_Add(effect);

// GPU-accelerated effect
var gpuEffect = new GPUVideoEffectBrightness(
    enabled: true,
    value: 180,
    name: "Brighten"
);
capture.Video_Effects_Add(gpuEffect);
```

### Cross-platform Effect Application

```csharp
// Cross-platform grayscale
var grayscale = new GrayscaleVideoEffect("bw_effect");
await videoCapture.Video_Effects_AddOrUpdateAsync(grayscale);

// Cross-platform text overlay
var textOverlay = new TextOverlayVideoEffect
{
    Text = "Hello World",
    Font = new FontSettings("Arial", "Bold", 24),
    Color = SKColors.Yellow,
    HorizontalAlignment = TextOverlayHAlign.Left,
    VerticalAlignment = TextOverlayVAlign.Top
};
await videoCapture.Video_Effects_AddOrUpdateAsync(textOverlay);
```

### Animated Effect (Classic)

```csharp
// Fade to black over 3 seconds
var fade = new VideoEffectDarkness(
    enabled: true,
    value: 128,        // Start normal
    valueStop: 255,    // End at maximum darkness
    name: "FadeOut",
    startTime: TimeSpan.FromSeconds(57),
    stopTime: TimeSpan.FromSeconds(60)
);
capture.Video_Effects_Add(fade);
```

### Time-Limited Effect (Both)

```csharp
// Classic: Apply effect only during specific time range
var flashback = new VideoEffectGrayscale(
    enabled: true,
    name: "Flashback",
    startTime: TimeSpan.FromMinutes(2),
    stopTime: TimeSpan.FromMinutes(2.5)
);
player.Video_Effects_Add(flashback);

// Cross-platform: Time-limited effect
var blur = new GaussianBlurVideoEffect("blur")
{
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromSeconds(15),
    Sigma = 5.0
};
await player.Video_Effects_AddOrUpdateAsync(blur);
```

## Performance Considerations

### Classic Effects

1. **GPU Effects**: Best performance for high-resolution video on Windows
2. **Effect Stacking**: Minimize simultaneous effects for better performance
3. **Animated Effects**: Smooth animations add minimal overhead
4. **AI Effects**: Most resource-intensive, use on RTX GPUs
5. **Resolution Impact**: Higher resolutions significantly increase processing requirements

### Cross-platform Effects

1. **Hardware Acceleration**: Varies by platform and multimedia framework build
2. **Cross-Platform**: Generally good performance on all supported platforms
3. **Mobile Optimization**: Better suited for mobile than Classic effects
4. **Overlay System**: Efficient multi-overlay rendering
5. **Plugin Availability**: Some effects require specific multimedia plugins

## Platform Availability

### Classic Effects

| Platform | CPU Effects | GPU Effects | AI Maxine |
|----------|-------------|-------------|-----------|
| Windows Desktop | ✅ Full | ✅ Full | ✅ RTX GPUs |
| Linux    | ❌ | ❌ | ❌ |
| macOS    | ❌ | ❌ | ❌ |
| Android  | ❌ | ❌ | ❌ |
| iOS      | ❌ | ❌ | ❌ |

### Cross-platform Effects

| Platform | Support | Hardware Acceleration |
|----------|---------|---------------------|
| Windows  | ✅ Full | ✅ Via multimedia plugins |
| Linux    | ✅ Full | ✅ VA-API, VDPAU, etc. |
| macOS    | ✅ Full | ✅ VideoToolbox |
| Android  | ✅ Full | ✅ Hardware codecs |
| iOS      | ✅ Full | ✅ VideoToolbox |

## Choosing the Right Effect Type

### Use Classic Effects When:
- ✅ Targeting Windows desktop only
- ✅ Need maximum performance on Windows
- ✅ Using DirectX GPU acceleration
- ✅ Need NVIDIA Maxine AI features
- ✅ Working with VideoCaptureCore/MediaPlayerCore engines

### Use Cross-platform Effects When:
- ✅ Need cross-platform support
- ✅ Targeting mobile platforms (Android/iOS)
- ✅ Targeting Linux or macOS
- ✅ Using VideoCaptureCoreX/Media Blocks
- ✅ Need advanced overlay features
- ✅ Need chroma key or motion detection

## Additional Resources

- [Text Overlay Guide](text-overlay.md)
- [Image Overlay Guide](image-overlay.md)
- [Video Sample Grabber](video-sample-grabber.md)
- [How to Add Effects](add.md)

---

Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for complete code samples and implementation examples.
