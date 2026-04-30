---
title: Custom Video Effects and OpenGL Shaders in C# .NET
description: Apply real-time video effects, GLSL shaders, LUT color grading, and pan/zoom animations in C# .NET with VisioForge Media Blocks SDK and GPU acceleration.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Encoding
  - Effects
  - Webcam
  - C#
primary_api_classes:
  - LUTProcessorBlock
  - MediaBlocksPipeline
  - GLUploadBlock
  - GLDownloadBlock
  - GLShaderBlock

---

# Custom Video Effects and OpenGL Shaders in C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The VisioForge Media Blocks SDK provides 70+ video processing blocks for real-time effects, GPU-accelerated OpenGL shaders, professional LUT color grading, and animated pan/zoom transformations. Effects are applied by inserting processing blocks into the pipeline between a video source and a renderer or encoder.

## Built-in Video Effects

[MediaBlocksPipeline](#){ .md-button }

All built-in effects follow the same pipeline pattern — insert the effect block between your source and renderer:

```text
VideoSource → EffectBlock → VideoRenderer
```

### Gaussian Blur

Apply blur or sharpening with configurable sigma. Positive values blur, negative values sharpen:

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Blur with sigma 1.2 (higher = more blur)
var blur = new GaussianBlurBlock(1.2);

pipeline.Connect(videoSource.Output, blur.Input);
pipeline.Connect(blur.Output, videoRenderer.Input);
```

### Noise Reduction (Smooth Filter)

OpenCV-based noise reduction with edge-preserving smoothing:

```csharp
using VisioForge.Core.Types.X.VideoEffects;

var smoothSettings = new SmoothVideoEffect()
{
    FilterSize = 5,       // kernel size (larger = stronger smoothing)
    Tolerance = 128,      // contrast threshold (0-255)
    LumaOnly = true       // smooth brightness only, preserve color
};

var smooth = new SmoothBlock(smoothSettings);

pipeline.Connect(videoSource.Output, smooth.Input);
pipeline.Connect(smooth.Output, videoRenderer.Input);
```

### Color Balance

Adjust brightness, contrast, saturation, and hue in real time:

```csharp
var balance = new VideoBalanceBlock(new VideoBalanceVideoEffect
{
    Brightness = 0.1,    // -1.0 to 1.0 (0 = no change)
    Contrast = 1.2,      // 0.0 to infinity (1 = no change)
    Saturation = 1.5,    // 0.0 to infinity (1 = no change)
    Hue = 0.0            // -1.0 to 1.0 (0 = no change)
});

pipeline.Connect(videoSource.Output, balance.Input);
pipeline.Connect(balance.Output, videoRenderer.Input);
```

### Preset Color Effects

Apply artistic color presets (sepia, heat map, cross-processing, etc.):

```csharp
var colorFx = new ColorEffectsBlock(ColorEffectsPreset.Sepia);

pipeline.Connect(videoSource.Output, colorFx.Input);
pipeline.Connect(colorFx.Output, videoRenderer.Input);
```

## GPU-Accelerated Effects with OpenGL

[MediaBlocksPipeline](#){ .md-button }

OpenGL effects run on the GPU for significantly better performance with HD/4K video. They require uploading video frames to GPU memory and downloading them back:

```text
VideoSource → GLUploadBlock → GLEffectBlock → GLDownloadBlock → VideoRenderer
```

### Using Built-in OpenGL Effects

The SDK includes 25+ GPU-accelerated effects:

```csharp
using VisioForge.Core.MediaBlocks.OpenGL;

var glUpload = new GLUploadBlock();
var glBlur = new GLBlurBlock();
var glDownload = new GLDownloadBlock();

pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, glBlur.Input);
pipeline.Connect(glBlur.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

### Available OpenGL Effects

| Effect Block | Description |
| --- | --- |
| `GLBlurBlock` | 9x9 separable convolution blur |
| `GLColorBalanceBlock` | Brightness, contrast, hue, saturation |
| `GLGrayscaleBlock` | Grayscale conversion |
| `GLSepiaBlock` | Sepia tone |
| `GLSobelBlock` | Sobel edge detection |
| `GLLaplacianBlock` | Laplacian edge detection |
| `GLFishEyeBlock` | Fisheye lens distortion |
| `GLBulgeBlock` | Bulge distortion |
| `GLTwirlBlock` | Twirl/swirl effect |
| `GLMirrorBlock` | Mirror reflection |
| `GLSqueezeBlock` | Squeeze distortion |
| `GLStretchBlock` | Stretch distortion |
| `GLHeatBlock` | Heat map visualization |
| `GLGlowLightingBlock` | Glow/lighting effect |
| `GLLightTunnelBlock` | Light tunnel effect |
| `GLSinCityBlock` | Sin City (selective desaturation) |
| `GLXRayBlock` | X-ray visualization |
| `GLLumaCrossProcessingBlock` | Luma cross-processing |
| `GLFlipBlock` | GPU flip/mirror |
| `GLDeinterlaceBlock` | GPU deinterlacing |
| `GLTransformationBlock` | Affine transformations |
| `GLAlphaBlock` | Alpha channel / chroma key |
| `GLEquirectangularViewBlock` | 360 equirectangular projection |

## Custom GLSL Shaders

[MediaBlocksPipeline](#){ .md-button }

Write custom GLSL fragment and vertex shaders and apply them to live video in real time. The `GLShaderBlock` executes your shader on the GPU for every video frame.

### Pipeline Architecture

```text
SystemVideoSourceBlock → GLUploadBlock → GLShaderBlock → GLDownloadBlock → VideoRendererBlock
```

### Complete Example

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenGL;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenGL;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Video source (webcam)
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var videoSource = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(videoDevices[0]));

// Upload frames to GPU memory
var glUpload = new GLUploadBlock();

// Custom GLSL shader — grayscale conversion
var vertexShader = @"
#version 100
#ifdef GL_ES
precision mediump float;
#endif
attribute vec4 a_position;
attribute vec2 a_texcoord;
varying vec2 v_texcoord;
void main() {
    gl_Position = a_position;
    v_texcoord = a_texcoord;
}";

var fragmentShader = @"
#version 100
#ifdef GL_ES
precision mediump float;
#endif
varying vec2 v_texcoord;
uniform sampler2D tex;
void main() {
    vec4 color = texture2D(tex, v_texcoord);
    float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114));
    gl_FragColor = vec4(vec3(gray), color.a);
}";

var shader = new GLShader(vertexShader, fragmentShader);
var shaderBlock = new GLShaderBlock(new GLShaderSettings(shader));

// Download frames back from GPU
var glDownload = new GLDownloadBlock();

// Video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Build pipeline
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, shaderBlock.Input);
pipeline.Connect(shaderBlock.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Switching Shaders at Runtime

You can change the shader while the pipeline is running:

```csharp
// Invert color shader
var invertFragment = @"
#version 100
#ifdef GL_ES
precision mediump float;
#endif
varying vec2 v_texcoord;
uniform sampler2D tex;
void main() {
    vec4 color = texture2D(tex, v_texcoord);
    gl_FragColor = vec4(vec3(1.0) - color.rgb, color.a);
}";

shaderBlock.Settings.Fragment = invertFragment;
shaderBlock.Update();  // apply changes without restarting pipeline
```

## Shader Uniforms for Dynamic Effects

[MediaBlocksPipeline](#){ .md-button }

Pass parameters to GLSL shaders at runtime using the `Uniforms` dictionary. This enables dynamic, real-time control over shader behavior.

### Two-Pass Gaussian Blur with Adjustable Radius

```csharp
// Horizontal blur pass
var horizontalSettings = new GLShaderSettings(vertexShader, horizontalFragmentShader);
horizontalSettings.Uniforms["blur_radius"] = 2.0f;
horizontalSettings.Uniforms["tex_width"] = 1920.0f;
var blurH = new GLShaderBlock(horizontalSettings);

// Vertical blur pass
var verticalSettings = new GLShaderSettings(vertexShader, verticalFragmentShader);
verticalSettings.Uniforms["blur_radius"] = 2.0f;
verticalSettings.Uniforms["tex_height"] = 1080.0f;
var blurV = new GLShaderBlock(verticalSettings);

// Pipeline: Source → Upload → H-Blur → V-Blur → Download → Renderer
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, blurH.Input);
pipeline.Connect(blurH.Output, blurV.Input);
pipeline.Connect(blurV.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

### Updating Uniforms at Runtime

```csharp
// Adjust blur radius in response to a slider change
float newRadius = 5.0f;

blurH.Settings.Uniforms["blur_radius"] = newRadius;
blurH.Update();

blurV.Settings.Uniforms["blur_radius"] = newRadius;
blurV.Update();
```

## LUT Color Grading

[MediaBlocksPipeline](#){ .md-button }

Apply professional color grading using 3D Look-Up Table (LUT) files. The `LUTProcessorBlock` transforms every pixel's color through a 3D color cube for cinematic color effects.

### Supported LUT Formats

| Format | Extension | Origin |
| --- | --- | --- |
| Iridas/Resolve | `.cube` | DaVinci Resolve, Adobe |
| After Effects | `.3dl` | Adobe After Effects |
| DaVinci | `.dat` | DaVinci Resolve |
| Pandora | `.m3d` | Pandora |
| CineSpace | `.csp` | CineSpace |

### Basic LUT Application

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

var lutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cinematic.cube");
var lutProcessor = new LUTProcessorBlock(new LUTVideoEffect(lutPath));

pipeline.Connect(videoSource.Output, lutProcessor.Input);
pipeline.Connect(lutProcessor.Output, videoRenderer.Input);
```

### Side-by-Side Comparison

Use a `TeeBlock` to show original and graded video side by side:

```csharp
using VisioForge.Core.MediaBlocks.Special;

var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);

var lutProcessor = new LUTProcessorBlock(new LUTVideoEffect(lutPath));

// Original output
var rendererOriginal = new VideoRendererBlock(pipeline, null);

// Graded output
var rendererGraded = new VideoRendererBlock(pipeline, null);

pipeline.Connect(videoSource.Output, tee.Input);
pipeline.Connect(tee.Outputs[0], lutProcessor.Input);
pipeline.Connect(lutProcessor.Output, rendererGraded.Input);
pipeline.Connect(tee.Outputs[1], rendererOriginal.Input);
```

### Interpolation Modes

The `LUTVideoEffect` supports three interpolation modes for quality/performance trade-offs:

- **Tetrahedral** — highest quality, best for final output
- **Trilinear** — good balance of quality and speed
- **NearestNeighbor** — fastest, lowest quality

## Pan, Zoom, and Ken Burns Animations

[MediaBlocksPipeline](#){ .md-button }

The `PanZoomBlock` provides static and animated pan/zoom transformations — ideal for Ken Burns effects, region-of-interest viewing, and dynamic video framing.

### Pipeline Setup

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

var panZoom = new PanZoomBlock();

pipeline.Connect(fileSource.VideoOutput, panZoom.Input);
pipeline.Connect(panZoom.Output, videoRenderer.Input);
```

### Static Zoom

Zoom into a specific point in the frame:

```csharp
// 2x zoom centered on the middle of the frame
panZoom.SetZoom(new VideoStreamZoomSettings(
    zoomX: 2.0,     // horizontal zoom factor
    zoomY: 2.0,     // vertical zoom factor
    centerX: 0.5,   // center point X (0.0 - 1.0)
    centerY: 0.5    // center point Y (0.0 - 1.0)
));
```

### Static Pan

Shift the visible region:

```csharp
// Pan 100 pixels right, 50 pixels down
panZoom.SetPan(new VideoStreamPanSettings(100, 50));
```

### Dynamic Zoom (Ken Burns Effect)

Animate a smooth zoom transition over a time range:

```csharp
// Slowly zoom from 1x to 2x over 5 seconds starting at current position
var startTime = await pipeline.Position_GetAsync();
var endTime = startTime + TimeSpan.FromSeconds(5);

panZoom.SetDynamicZoom(new VideoStreamDynamicZoomSettings(
    startZoomX: 1.0, startZoomY: 1.0,   // start at normal size
    stopZoomX: 2.0,  stopZoomY: 2.0,     // end at 2x zoom
    startTime: startTime,
    stopTime: endTime
));
```

### Dynamic Pan

Animate a smooth pan movement:

```csharp
panZoom.SetDynamicPan(new VideoStreamDynamicPanSettings(
    startPanX: 0, startPanY: 0,       // start position
    stopPanX: 200, stopPanY: 100,     // end position
    startTime: startTime,
    stopTime: endTime
));
```

### Rect Crop and Pan

Focus on a specific rectangular region of the video:

```csharp
// Show only the region starting at (50, 50) with 400x300 size
panZoom.SetRect(VideoStreamRectSettings.FromPositionAndSize(50, 50, 400, 300));
```

### Resetting Pan/Zoom

Clear all pan/zoom settings to return to the original view:

```csharp
panZoom.SetZoom(null);
panZoom.SetDynamicZoom(null);
panZoom.SetPan(null);
panZoom.SetDynamicPan(null);
panZoom.SetRect(null);
```

## Chaining Multiple Effects

Connect multiple effect blocks in series to build complex processing chains:

```text
VideoSource → GaussianBlurBlock → VideoBalanceBlock → LUTProcessorBlock → VideoRenderer
```

```csharp
var blur = new GaussianBlurBlock(0.8);
var balance = new VideoBalanceBlock(new VideoBalanceVideoEffect
{
    Brightness = 0.05,
    Contrast = 1.1
});
var lut = new LUTProcessorBlock(new LUTVideoEffect("cinematic.cube"));

pipeline.Connect(videoSource.Output, blur.Input);
pipeline.Connect(blur.Output, balance.Input);
pipeline.Connect(balance.Output, lut.Input);
pipeline.Connect(lut.Output, videoRenderer.Input);
```

For GPU-accelerated chains, keep frames in GPU memory between effects:

```csharp
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, glBlur.Input);
pipeline.Connect(glBlur.Output, glColorBalance.Input);
pipeline.Connect(glColorBalance.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

## Troubleshooting

### OpenGL Effects Not Working

**Symptom:** Pipeline fails to start or video appears black when using GL effects.

**Solutions:**

- Verify GPU drivers are up to date — OpenGL effects require hardware OpenGL support
- Ensure `GLUploadBlock` and `GLDownloadBlock` are used to transfer frames to/from GPU memory
- Fall back to CPU-based equivalents (e.g., `GaussianBlurBlock` instead of `GLBlurBlock`) on systems without GPU support
- Check `GLShaderBlock.IsAvailable()` before creating OpenGL blocks

### LUT File Not Loading

**Symptom:** `LUTProcessorBlock` throws an exception or has no visible effect.

**Solutions:**

- Verify the LUT file path is correct and the file exists
- Ensure the file format is supported (`.cube`, `.3dl`, `.dat`, `.m3d`, `.csp`)
- Check `LUTProcessorBlock.IsAvailable()` before creating the block
- Use an absolute path or `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file.cube")`

### High CPU Usage with Effects

**Symptom:** Performance degrades when applying multiple effects.

**Solutions:**

- Switch from CPU effects to OpenGL GPU-accelerated equivalents
- Reduce video resolution before applying effects (use `VideoResizeBlock`)
- Minimize the number of chained effects — combine operations in a single custom shader when possible
- For two-pass blur, use the single-pass simple blur as a faster alternative

## Frequently Asked Questions

### How do I apply a custom GLSL shader to live video in C#?

Create a `GLShaderBlock` with your vertex and fragment shader source code, then insert it between `GLUploadBlock` and `GLDownloadBlock` in your pipeline. The standard vertex shader passes through position and texture coordinates. Your fragment shader receives the video frame as `uniform sampler2D tex` and outputs to `gl_FragColor`. Use `shaderBlock.Update()` to change shaders at runtime without restarting the pipeline. See the [Custom GLSL Shaders](#custom-glsl-shaders) section for a complete example.

### What LUT file formats does the SDK support for color grading?

The `LUTProcessorBlock` supports five industry-standard 3D LUT formats: Iridas/Resolve `.cube`, After Effects `.3dl`, DaVinci `.dat`, Pandora `.m3d`, and CineSpace `.csp`. The `.cube` format is the most widely used and is exported by DaVinci Resolve, Adobe Premiere, and most color grading tools. Three interpolation modes are available: Tetrahedral (highest quality), Trilinear, and NearestNeighbor.

### Can I chain multiple video effects together in a pipeline?

Yes. Connect effect blocks in series: `Source → Effect1 → Effect2 → Effect3 → Renderer`. Each block processes the output of the previous one. For GPU-accelerated chains, keep frames in OpenGL memory by connecting GL effect blocks directly without inserting `GLDownloadBlock` / `GLUploadBlock` between them — only upload at the start and download at the end.

### How do I create a Ken Burns pan/zoom animation?

Use `PanZoomBlock` with `SetDynamicZoom()` to animate a smooth zoom transition over time. Pass a `VideoStreamDynamicZoomSettings` with start/stop zoom factors and start/stop timestamps. The block interpolates between the values automatically. Combine with `SetDynamicPan()` for simultaneous pan and zoom animation. See the [Pan, Zoom, and Ken Burns Animations](#pan-zoom-and-ken-burns-animations) section for code examples.

### What platforms support OpenGL video effects?

OpenGL effects are supported on Windows, Linux, macOS, and Android — any platform with OpenGL ES 2.0 or higher support. On iOS and macOS, the SDK also provides Metal-accelerated equivalents (`MetalVideoFilterBlock`) for native GPU performance. CPU-based effects (`GaussianBlurBlock`, `SmoothBlock`, `ColorEffectsBlock`, etc.) work on all platforms including iOS.

## See Also

- [Video Processing Blocks Reference](../VideoProcessing/index.md) — complete list of 70+ processing blocks
- [OpenGL Effects Reference](../OpenGL/index.md) — GPU-accelerated OpenGL effect settings and types
- [Video Effects Overview](../../general/video-effects/index.md) — Classic and Cross-Platform effect categories
- [Effects Reference (100+ effects)](../../general/video-effects/effects-reference.md) — detailed parameters for all effects
- [Deployment Guide](../../deployment-x/index.md) — platform-specific runtime packages
- [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — shader and effects demos
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — product page and downloads
