---
title: Advanced Video Effects & Processing for .Net SDKs
description: Implement professional video effects, text/image overlays, and custom video processing with powerful visual enhancement tools for .NET apps.
sidebar_label: Video Effects And Processing

order: 15
---

# Video Effects and Processing for .Net Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Our .Net SDKs provide developers with two distinct implementations of video effects to match your platform and performance requirements:

### Classic Effects (Windows Only)
Available in **VideoCaptureCore**, **MediaPlayerCore**, and **VideoEditCore**:
- CPU-based effects (`VideoEffect*`)
- GPU-accelerated effects (`GPUVideoEffect*`) using DirectX
- AI-powered effects (`Maxine*`) using NVIDIA technology
- Windows-only, optimized for desktop performance

### Cross-Platform Effects
Available in **VideoCaptureCoreX**, **MediaPlayerCoreX**, **VideoEditCoreX**, and **Media Blocks SDK**:
- Cross-platform implementation (`*VideoEffect` classes)
- Works on Windows, Linux, macOS, Android, and iOS
- Hardware acceleration through platform-specific multimedia plugins
- Extended overlay system and advanced features
- Better suited for mobile and cross-platform deployment

## Complete Effects Reference

**[View Complete Video Effects Reference →](effects-reference.md)**

Our comprehensive effects reference provides detailed information on all 100+ available video effects across both implementations:
- **Classic effects** for Windows-only applications
- **Cross-platform effects** for universal compatibility
- Effect parameters, usage examples, and SDK availability
- Platform-specific features and capabilities

## Available Video Effect Categories

The following effect categories are available in both Classic and Cross-Platform implementations (where applicable):

### Color and Image Adjustment

* **Brightness, Darkness, Contrast** - Control luminance and tonal range
  - Classic: `VideoEffect*` / `GPUVideoEffect*`
  - Cross-platform: `VideoBalanceVideoEffect`, `GammaVideoEffect`
* **Saturation and Color Grading** - Adjust color intensity and apply color corrections
  - Classic: `VideoEffectSaturation` / `GPUVideoEffectSaturation`
  - Cross-platform: `VideoBalanceVideoEffect`, `ColorEffectsVideoEffect`, `LUTVideoEffect`
* **Lightness** - HSL-based brightness adjustment preserving color relationships
  - Classic: `VideoEffectLightness`
* **Color Filters** - Isolate or enhance specific color channels (Red, Green, Blue)
  - Classic: `VideoEffectRed/Green/Blue`, `VideoEffectFilterRed/Green/Blue`

### Creative and Artistic Effects

* **Grayscale and Monochrome** - Black and white conversions with optional tinting
  - Classic: `VideoEffectGrayscale` / `GPUVideoEffectGrayscale`
  - Cross-platform: `GrayscaleVideoEffect`, `ColorEffectsVideoEffect`
* **Invert and Solarize** - Photographic negative and partial inversion effects
  - Classic: `VideoEffectInvert` / `GPUVideoEffectInvert`, `VideoEffectSolorize`
* **Old Movie and Night Vision** - Vintage film and surveillance camera simulations
  - Classic: `GPUVideoEffectOldMovie`, `GPUVideoEffectNightVision`
  - Cross-platform: `AgingVideoEffect`, `OpticalAnimationBWVideoEffect`
* **Emboss and Contour** - Edge detection and relief effects
  - Classic: `GPUVideoEffectEmboss`, `GPUVideoEffectContour`
  - Cross-platform: `EdgeVideoEffect`
* **Pixelate and Mosaic** - Stylized block and tile effects
  - Classic: `GPUVideoEffectPixelate`, `VideoEffectMosaic`
  - Cross-platform: Various distortion effects

### Image Enhancement

* **Sharpening** - Enhance edge definition and fine details
  - Classic: `VideoEffectSharpen` / `GPUVideoEffectSharpen`
* **Blur and Smoothing** - Reduce detail, noise, and create soft-focus effects
  - Classic: `VideoEffectBlur` / `GPUVideoEffectBlur`, `VideoEffectSmooth`
  - Cross-platform: `GaussianBlurVideoEffect`, `SmoothVideoEffect`
* **Noise Reduction** - Adaptive, CAST, and mosquito noise removal algorithms
  - Classic: `VideoEffectDenoiseAdaptive/CAST/Mosquito` / `GPUVideoEffectDenoise`
* **Deinterlacing** - Convert interlaced video to progressive (Blend, CAVT, Triangle methods)
  - Classic: `VideoEffectDeinterlaceBlend/CAVT/Triangle` / `GPUVideoEffectDeinterlaceBlend`
  - Cross-platform: `DeinterlaceVideoEffect`, `AutoDeinterlaceSettings`

### Geometric Transformations

* **Rotation** - Rotate by any angle with stretch or no-crop options
  - Classic: `VideoEffectRotate`
  - Cross-platform: `FlipRotateVideoEffect`
* **Flip and Mirror** - Horizontal and vertical flipping/mirroring
  - Classic: `VideoEffectFlip*`, `VideoEffectMirror*`
  - Cross-platform: `FlipRotateVideoEffect`
* **Zoom and Pan** - Focus on specific regions with scaling control
  - Classic: `VideoEffectZoom`, `VideoEffectPan`
  - Cross-platform: `ResizeVideoEffect`, `CropVideoEffect`, `AspectRatioCropVideoEffect`, `BoxVideoEffect`
* **360° Video Processing** - Equirectangular and cubemap projections for VR video
  - Classic: `GPUVideoEffectEquirectangular360`, `GPUVideoEffectEquiangularCubemap360`

### Artistic Distortion Effects (Cross-platform Only)

The cross-platform implementation includes extensive artistic distortion effects not available in Classic:

* **Lens Effects** - Fish-eye, Sphere, Twirl, Bulge, Pinch
* **Pattern Effects** - Kaleidoscope, Marble, Quark, Dice
* **Motion Effects** - Moving Blur, Moving Echo, Moving Zoom Echo
* **Warp Effects** - Ripple, Water Ripple, Warp, Stretch, Tunnel
* **Perspective Effects** - Pseudo3D, Square, Circle, Perspective
* **SMPTE Transitions** - Professional broadcast-style wipes and transitions

### Transition Effects

* **Fade In/Out** - Smooth transitions to/from black
  - Classic: `VideoEffectFadeIn`, `VideoEffectFadeOut`
* **Animated Transitions** - Time-based effect value interpolation for dynamic changes
  - Classic: Supported via ValueStop parameter
  - Cross-platform: Time-based with StartTime/StopTime

## Overlay Capabilities

* [**Text overlay**](text-overlay.md) - Add customizable text with control over font, size, color, rotation, and animation
  - Classic: `VideoEffectTextLogo`, `VideoEffectScrollingTextLogo`
  - Cross-platform: `TextOverlayVideoEffect`, `OverlayManagerText`, `OverlayManagerScrollingText`, `OverlayManagerDateTime`
* [**Image overlay**](image-overlay.md) - Incorporate logos, watermarks, and graphic elements with transparency support
  - Classic: `VideoEffectImageLogo`
  - Cross-platform: `ImageOverlayVideoEffect`, `ImageOverlayCairoVideoEffect`, `OverlayManagerImage`, `OverlayManagerGIF`
  * Support for PNG, JPG, BMP, animated GIF formats
  * Transparency and alpha channel support
  * Positioning and alignment control

### Advanced Overlay Features (Cross-platform Only)

The cross-platform implementation includes an extensive OverlayManager system:

* **Video Overlays** - Picture-in-picture, Decklink, NDI video sources
* **Shape Overlays** - Circles, rectangles, stars, triangles, lines
* **Background Layers** - Images, solid colors, geometric shapes
* **Animations** - Pan, zoom, fade, squeezeback effects
* **Group Management** - Synchronized control of multiple overlays
* **SVG Support** - Vector graphics overlays
* **QR Codes** - Generate and overlay QR codes

## Chroma Key and Motion Detection (Cross-platform Only)

* **ChromaKeySettings** - Green screen / blue screen keying for background removal
* **MotionDetectionProcessor** - Real-time motion detection with configurable sensitivity

## AI-Powered Video Enhancement (Classic Only)

NVIDIA Maxine AI effects (requires NVIDIA RTX GPU with Maxine SDK support):

* **AI Denoise** - Intelligent noise reduction preserving fine details
* **Artifact Reduction** - Remove compression artifacts and video degradation
* **Super Resolution** - AI upscaling for enhanced resolution
* **AI Green Screen** - Background removal without physical green screen
* **Upscaling** - Advanced quality improvement through AI

## Video Processing Features
  
### Real-time Processing

* Apply effects during video capture, playback, or editing
* Chain multiple effects for complex processing pipelines
* GPU acceleration for real-time high-resolution processing (Classic)
* Hardware acceleration for cross-platform performance (Cross-platform)
* CPU fallback for universal compatibility

### Advanced Processing

* Timeline-based effect application (start/stop times)
* Animated effect transitions (interpolation between values - Classic)
* Dynamic effect control during playback
* [**Video sample grabber**](video-sample-grabber.md) - Extract frames and process video data in real-time

## Performance Optimization

### Classic Effects (Windows Only)

- **GPU Effects** (`GPUVideoEffect*`): Best performance for high-resolution video on Windows
- **CPU Effects** (`VideoEffect*`): Universal Windows compatibility, moderate performance
- **AI Effects** (`Maxine*`): State-of-the-art quality, requires NVIDIA RTX GPU
- Use GPU-accelerated effects for real-time HD/4K processing
- DirectX hardware acceleration for optimal Windows performance

### Cross-platform Effects (Cross-Platform)

- **Hardware Acceleration**: Varies by platform via multimedia plugins
  - Windows: DirectX, DXVA
  - Linux: VA-API, VDPAU, OpenGL
  - macOS: VideoToolbox, Metal
  - Android/iOS: Hardware codecs
- **Performance**: Generally good across all platforms
- **Mobile Optimization**: Better suited for mobile than Classic effects
- **Overlay System**: Efficient multi-overlay rendering

### Best Practices

* Choose GPU/accelerated effects for high-resolution video
* Consider effect combinations and their performance impact
* Apply time-limited effects where appropriate to reduce processing overhead
* Test performance on target hardware configurations
* Use Cross-platform effects for cross-platform deployment

## Platform Support

### Classic Effects

| Platform | CPU Effects | GPU Effects | AI Maxine | SDK |
|----------|-------------|-------------|-----------|-----|
| Windows Desktop | ✅ Full | ✅ Full | ✅ RTX GPUs | VideoCaptureCore<br/>MediaPlayerCore<br/>VideoEditCore |
| Linux    | ❌ | ❌ | ❌ | N/A |
| macOS    | ❌ | ❌ | ❌ | N/A |
| Android  | ❌ | ❌ | ❌ | N/A |
| iOS      | ❌ | ❌ | ❌ | N/A |

### Cross-platform Effects

| Platform | Support | Hardware Acceleration | SDK |
|----------|---------|---------------------|-----|
| Windows  | ✅ Full | ✅ DirectX, DXVA | VideoCaptureCoreX<br/>MediaPlayerCoreX<br/>VideoEditCoreX<br/>Media Blocks SDK |
| Linux    | ✅ Full | ✅ VA-API, VDPAU, OpenGL | ✓ |
| macOS    | ✅ Full | ✅ VideoToolbox, Metal | ✓ |
| Android  | ✅ Full | ✅ Hardware codecs | ✓ |
| iOS      | ✅ Full | ✅ VideoToolbox | ✓ |

## Integration Methods

Our SDKs provide two distinct APIs for video effects integration based on the SDK engine you're using.

### Basic Usage Example (Classic Effects)

```csharp
// Windows-only: CPU-based effect
var bwEffect = new VideoEffectGrayscale(
    enabled: true,
    name: "BlackAndWhite"
);
videoCapture.Video_Effects_Add(bwEffect);

// Windows-only: GPU-accelerated effect
var brighten = new GPUVideoEffectBrightness(
    enabled: true,
    value: 180,
    name: "Brighten"
);
videoCapture.Video_Effects_Add(brighten);
```

### Basic Usage Example (Cross-platform Effects)

```csharp
// Cross-platform: Grayscale effect
var grayscale = new GrayscaleVideoEffect("bw_effect");
await videoCaptureX.Video_Effects_AddOrUpdateAsync(grayscale);

// Cross-platform: Video balance (brightness, contrast, saturation)
var balance = new VideoBalanceVideoEffect("color_adjust")
{
    Brightness = 0.2,    // Range: -1.0 to 1.0
    Contrast = 1.2,      // Range: 0.0 to 2.0
    Saturation = 1.5,    // Range: 0.0 to 2.0
    Hue = 0.0            // Range: -1.0 to 1.0
};
await videoCaptureX.Video_Effects_AddOrUpdateAsync(balance);
```

### Animated Effect Example (Classic)

```csharp
// Fade to black over 3 seconds (Windows only)
var fadeOut = new VideoEffectDarkness(
    enabled: true,
    value: 128,        // Start at normal
    valueStop: 255,    // End at black
    name: "FadeOut",
    startTime: TimeSpan.FromSeconds(57),
    stopTime: TimeSpan.FromSeconds(60)
);
mediaPlayer.Video_Effects_Add(fadeOut);
```

### Time-Limited Effect Example (Cross-platform)

```csharp
// Apply Gaussian blur for specific time range (Cross-platform)
var blur = new GaussianBlurVideoEffect("scene_blur")
{
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromSeconds(15),
    Sigma = 5.0  // Blur intensity
};
await mediaPlayerX.Video_Effects_AddOrUpdateAsync(blur);
```

### Advanced Overlay Example (Cross-platform)

```csharp
// Complex text overlay with shadows and animation (Cross-platform)
var textOverlay = new OverlayManagerText
{
    Text = "Breaking News",
    Font = new FontSettings
    {
        Name = "Arial",
        Size = 48,
        Weight = FontWeight.Bold
    },
    Color = SKColors.Yellow,
    XPosition = 50,
    YPosition = 100,
    ShadowSettings = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.Black,
        OffsetX = 2,
        OffsetY = 2,
        Blur = 5
    }
};
await videoCaptureX.OverlayManager_AddAsync(textOverlay);
```

## Documentation Resources

* **[Complete Effects Reference](effects-reference.md)** - Comprehensive guide to all available effects
* **[Text Overlay Guide](text-overlay.md)** - Detailed text overlay implementation
* **[Image Overlay Guide](image-overlay.md)** - Image and logo overlay techniques
* **[Video Sample Grabber](video-sample-grabber.md)** - Frame extraction and processing
* **[How to Add Effects](add.md)** - General guide to applying effects

## More Information

Numerous additional video effects and processing features are available in the SDKs. Please refer to the [Complete Effects Reference](effects-reference.md) for detailed information on all effects, or visit the documentation for the specific SDK you are using for implementation examples and API references.
  
---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to access more code samples and implementation examples.
