---
title: .Net Media OpenGL Video Effects Guide
description: Explore a comprehensive guide to OpenGL video effects available in VisioForge Media Blocks SDK .Net. Learn about various effects, their settings, and related OpenGL functionalities.
sidebar_label: OpenGL Effects
---

# OpenGL Video Effects - VisioForge Media Blocks SDK .Net

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

OpenGL video effects in VisioForge Media Blocks SDK .Net allow for powerful, hardware-accelerated manipulation of video streams. These effects can be applied to video content processed within an OpenGL context, typically via blocks like `GLVideoEffectsBlock` or custom OpenGL rendering pipelines. This guide covers the available effects, their configuration settings, and other related OpenGL types.

## Base Effect: `GLBaseVideoEffect`

All OpenGL video effects inherit from the `GLBaseVideoEffect` class, which provides common properties and events.

| Property | Type                  | Description                                      |
|----------|-----------------------|--------------------------------------------------|
| `Name`   | `string`              | The internal name of the effect (read-only).     |
| `ID`     | `GLVideoEffectID`     | The unique identifier for the effect (read-only). |
| `Index`  | `int`                 | The index of the effect in a chain.              |

**Events:**

- `OnUpdate`: Occurs when effect properties need to be updated in the pipeline. Call `OnUpdateCall()` to trigger it.

## Available Video Effects

This section details the various OpenGL video effects you can use. These effects are typically added to a `GLVideoEffectsBlock` or a similar OpenGL processing element.

### Alpha Effect (`GLAlphaVideoEffect`)

Replaces a selected color with an alpha channel or sets/adjusts the existing alpha channel.

**Properties:**

| Property           | Type                     | Default Value    | Description                                            |
|--------------------|--------------------------|------------------|--------------------------------------------------------|
| `Alpha`            | `double`                 | `1.0`            | The value for the alpha channel.                       |
| `Angle`            | `float`                  | `20`             | The size of the colorcube to change (sensitivity radius for color matching). |
| `BlackSensitivity` | `uint`                   | `100`            | The sensitivity to dark colors.                        |
| `Mode`             | `GLAlphaVideoEffectMode` | `Set`            | The method used for alpha modification.                |
| `NoiseLevel`       | `float`                  | `2`              | The size of noise radius (pixels to ignore around the matched color). |
| `CustomColor`      | `SKColor`                | `SKColors.Green` | Custom color value for `Custom` chroma key mode.       |
| `WhiteSensitivity` | `uint`                   | `100`            | The sensitivity to bright colors.                      |

**Associated Enum: `GLAlphaVideoEffectMode`**

Defines the mode of operation for the Alpha video effect.

| Value    | Description                            |
|----------|----------------------------------------|
| `Set`    | Set/adjust alpha channel directly using the `Alpha` property. |
| `Green`  | Chroma Key on pure green.              |
| `Blue`   | Chroma Key on pure blue.               |
| `Custom` | Chroma Key on the color specified by `CustomColor`. |

### Blur Effect (`GLBlurVideoEffect`)

Applies a blur effect using a 9x9 separable convolution. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Bulge Effect (`GLBulgeVideoEffect`)

Creates a bulge distortion on the video. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Color Balance Effect (`GLColorBalanceVideoEffect`)

Adjusts the color balance of the video, including brightness, contrast, hue, and saturation.

**Properties:**

| Property     | Type     | Default Value | Description                                      |
|--------------|----------|---------------|--------------------------------------------------|
| `Brightness` | `double` | `0`           | Adjusts brightness (-1.0 to 1.0, 0 means no change). |
| `Contrast`   | `double` | `1`           | Adjusts contrast (0.0 to infinity, 1 means no change). |
| `Hue`        | `double` | `0`           | Adjusts hue (-1.0 to 1.0, 0 means no change).    |
| `Saturation` | `double` | `1`           | Adjusts saturation (0.0 to infinity, 1 means no change). |

### Deinterlace Effect (`GLDeinterlaceVideoEffect`)

Applies a deinterlacing filter to the video.

**Properties:**

| Property | Type                  | Default Value   | Description                         |
|----------|-----------------------|-----------------|-------------------------------------|
| `Method` | `GLDeinterlaceMethod` | `VerticalBlur`  | The deinterlacing method to use.    |

**Associated Enum: `GLDeinterlaceMethod`**

Defines the method for the Deinterlace video effect.

| Value          | Description                             |
|----------------|-----------------------------------------|
| `VerticalBlur` | Vertical blur method.                   |
| `MAAD`         | Motion Adaptive: Advanced Detection.    |

### Fish Eye Effect (`GLFishEyeVideoEffect`)

Applies a fish-eye lens distortion effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Flip Effect (`GLFlipVideoEffect`)

Flips or rotates the video.

**Properties:**

| Property | Type                | Default Value | Description                         |
|----------|---------------------|---------------|-------------------------------------|
| `Method` | `GLFlipVideoMethod` | `None`        | The flip or rotation method to use. |

**Associated Enum: `GLFlipVideoMethod`**

Defines the video flip or rotation method.

| Value                | Description                                  |
|----------------------|----------------------------------------------|
| `None`               | No rotation.                                 |
| `Clockwise`          | Rotate clockwise 90 degrees.                 |
| `Rotate180`          | Rotate 180 degrees.                          |
| `CounterClockwise`   | Rotate counter-clockwise 90 degrees.         |
| `HorizontalFlip`     | Flip horizontally.                           |
| `VerticalFlip`       | Flip vertically.                             |
| `UpperLeftDiagonal`  | Flip across upper left/lower right diagonal. |
| `UpperRightDiagonal` | Flip across upper right/lower left diagonal. |

### Glow Lighting Effect (`GLGlowLightingVideoEffect`)

Adds a glow lighting effect to the video. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Grayscale Effect (`GLGrayscaleVideoEffect`)

Converts the video to grayscale. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Heat Effect (`GLHeatVideoEffect`)

Applies a heat signature-like effect to the video. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Laplacian Effect (`GLLaplacianVideoEffect`)

Applies a Laplacian edge detection filter.

**Properties:**

| Property | Type    | Default Value | Description                                                       |
|----------|---------|---------------|-------------------------------------------------------------------|
| `Invert` | `bool`  | `false`       | If `true`, inverts colors to get dark edges on a bright background. |

### Light Tunnel Effect (`GLLightTunnelVideoEffect`)

Creates a light tunnel visual effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Luma Cross Processing Effect (`GLLumaCrossProcessingVideoEffect`)

Applies a luma cross-processing (often "xpro") effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Mirror Effect (`GLMirrorVideoEffect`)

Applies a mirror effect to the video. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Resize Effect (`GLResizeVideoEffect`)

Resizes the video to the specified dimensions.

**Properties:**

| Property | Type  | Description                            |
|----------|-------|----------------------------------------|
| `Width`  | `int` | The target width for the video resize. |
| `Height` | `int` | The target height for the video resize.|

### Sepia Effect (`GLSepiaVideoEffect`)

Applies a sepia tone effect to the video. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Sin City Effect (`GLSinCityVideoEffect`)

Applies a "Sin City" movie style effect (grayscale with red highlights). This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Sobel Effect (`GLSobelVideoEffect`)

Applies a Sobel edge detection filter.

**Properties:**

| Property | Type    | Default Value | Description                                                       |
|----------|---------|---------------|-------------------------------------------------------------------|
| `Invert` | `bool`  | `false`       | If `true`, inverts colors to get dark edges on a bright background. |

### Square Effect (`GLSquareVideoEffect`)

Applies a "square" distortion or pixelation effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Squeeze Effect (`GLSqueezeVideoEffect`)

Applies a squeeze distortion effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Stretch Effect (`GLStretchVideoEffect`)

Applies a stretch distortion effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### Transformation Effect (`GLTransformationVideoEffect`)

Applies 3D transformations to the video, including rotation, scaling, and translation.

**Properties:**

| Property       | Type    | Default Value | Description                                                           |
|----------------|---------|---------------|-----------------------------------------------------------------------|
| `FOV`          | `float` | `90.0f`       | Field of view angle in degrees for perspective projection.            |
| `Ortho`        | `bool`  | `false`       | If `true`, uses orthographic projection; otherwise, perspective.        |
| `PivotX`       | `float` | `0.0f`        | X-coordinate of the rotation pivot point (0 is center).               |
| `PivotY`       | `float` | `0.0f`        | Y-coordinate of the rotation pivot point (0 is center).               |
| `PivotZ`       | `float` | `0.0f`        | Z-coordinate of the rotation pivot point (0 is center).               |
| `RotationX`    | `float` | `0.0f`        | Rotation around the X-axis in degrees.                                |
| `RotationY`    | `float` | `0.0f`        | Rotation around the Y-axis in degrees.                                |
| `RotationZ`    | `float` | `0.0f`        | Rotation around the Z-axis in degrees.                                |
| `ScaleX`       | `float` | `1.0f`        | Scale multiplier for the X-axis.                                      |
| `ScaleY`       | `float` | `1.0f`        | Scale multiplier for the Y-axis.                                      |
| `TranslationX` | `float` | `0.0f`        | Translation along the X-axis (universal coordinates [0-1]).          |
| `TranslationY` | `float` | `0.0f`        | Translation along the Y-axis (universal coordinates [0-1]).          |
| `TranslationZ` | `float` | `0.0f`        | Translation along the Z-axis (universal coordinates [0-1], depth).   |

### Twirl Effect (`GLTwirlVideoEffect`)

Applies a twirl distortion effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

### X-Ray Effect (`GLXRayVideoEffect`)

Applies an X-ray like visual effect. This effect does not have additional configurable properties beyond those inherited from `GLBaseVideoEffect`.

## OpenGL Effect Identification: `GLVideoEffectID` Enum

This enumeration lists all available OpenGL video effect types, used by `GLBaseVideoEffect.ID`.

| Value            | Description                               |
|------------------|-------------------------------------------|
| `ColorBalance`   | The color balance effect.                 |
| `Grayscale`      | The grayscale effect.                     |
| `Resize`         | The resize effect.                        |
| `Deinterlace`    | The deinterlace effect.                   |
| `Flip`           | The flip effect.                          |
| `Blur`           | Blur with 9x9 separable convolution effect. |
| `FishEye`        | The fish eye effect.                      |
| `GlowLighting`   | The glow lighting effect.                 |
| `Heat`           | The heat signature effect.                |
| `LumaX`          | The luma cross processing effect.         |
| `Mirror`         | The mirror effect.                        |
| `Sepia`          | The sepia effect.                         |
| `Square`         | The square effect.                        |
| `XRay`           | The X-ray effect.                         |
| `Stretch`        | The stretch effect.                       |
| `LightTunnel`    | The light tunnel effect.                  |
| `Twirl`          | The twirl effect.                         |
| `Squeeze`        | The squeeze effect.                       |
| `SinCity`        | The sin city movie gray-red effect.       |
| `Bulge`          | The bulge effect.                         |
| `Sobel`          | The sobel effect.                         |
| `Laplacian`      | The laplacian effect.                     |
| `Alpha`          | The alpha channels effect.                |
| `Transformation` | The transformation effect.                |

## OpenGL Rendering and View Configuration

These types assist in configuring how video is rendered or viewed in an OpenGL context, especially for specialized scenarios like VR or custom display setups.

### Equirectangular View Settings (`GLEquirectangularViewSettings`)

Manages settings for rendering equirectangular (360-degree) video, commonly used in VR applications. Implements `IVRVideoControl`.

**Properties:**

| Property        | Type         | Default           | Description                                    |
|-----------------|--------------|-------------------|------------------------------------------------|
| `VideoWidth`    | `int`        | (readonly)        | Width of the source video.                     |
| `VideoHeight`   | `int`        | (readonly)        | Height of the source video.                    |
| `FieldOfView`   | `float`      | `80.0f`           | Field of view in degrees.                      |
| `Yaw`           | `float`      | `0.0f`            | Yaw (rotation around Y-axis) in degrees.       |
| `Pitch`         | `float`      | `0.0f`            | Pitch (rotation around X-axis) in degrees.     |
| `Roll`          | `float`      | `0.0f`            | Roll (rotation around Z-axis) in degrees.      |
| `Mode`          | `VRMode`     | `Equirectangular` | The VR mode (supports `Equirectangular`).    |

**Methods:**

- `IsModeSupported(VRMode mode)`: Checks if the specified `VRMode` is supported.

**Events:**

- `SettingsChanged`: Occurs when any view setting is changed.

### Video Renderer Settings (`GLVideoRendererSettings`)

Configures general properties for an OpenGL-based video renderer.

**Properties:**

| Property           | Type                          | Default     | Description                                                          |
|--------------------|-------------------------------|-------------|----------------------------------------------------------------------|
| `ForceAspectRatio` | `bool`                        | `true`      | Whether scaling will respect the original aspect ratio.                |
| `IgnoreAlpha`      | `bool`                        | `true`      | Whether alpha channel will be ignored (treated as black).              |
| `PixelAspectRatio` | `System.Tuple<int, int>`      | `(0, 1)`    | Pixel aspect ratio of the display device (numerator, denominator).     |
| `Rotation`         | `GLVideoRendererRotateMethod` | `None`      | Specifies the rotation applied to the video.                         |

**Associated Enum: `GLVideoRendererRotateMethod`**

Defines rotation methods for the OpenGL video renderer.

| Value            | Description                             |
|------------------|-----------------------------------------|
| `None`           | No rotation.                            |
| `_90C`           | Rotate 90 degrees clockwise.            |
| `_180`           | Rotate 180 degrees.                     |
| `_90CC`          | Rotate 90 degrees counter-clockwise.    |
| `FlipHorizontal` | Flip horizontally.                      |
| `FlipVertical`   | Flip vertically.                        |

## Custom OpenGL Shaders

Allows for the application of custom GLSL shaders to the video stream.

### Shader Definition (`GLShader`)

Represents a pair of vertex and fragment shaders.

**Properties:**

| Property         | Type     | Description                                   |
|------------------|----------|-----------------------------------------------|
| `VertexShader`   | `string` | The GLSL source code for the vertex shader.   |
| `FragmentShader` | `string` | The GLSL source code for the fragment shader. |

**Constructors:**
- `GLShader()`
- `GLShader(string vertexShader, string fragmentShader)`

### Shader Settings (`GLShaderSettings`)

Configures custom GLSL shaders for use in the pipeline.

**Properties:**

| Property   | Type                                 | Description                                      |
|------------|--------------------------------------|--------------------------------------------------|
| `Vertex`   | `string`                             | The GLSL source code for the vertex shader.      |
| `Fragment` | `string`                             | The GLSL source code for the fragment shader.    |
| `Uniforms` | `System.Collections.Generic.Dictionary<string, object>` | A dictionary of uniform variables (parameters) to be passed to the shaders. |

**Constructors:**
- `GLShaderSettings()`
- `GLShaderSettings(string vertex, string fragment)`
- `GLShaderSettings(GLShader shader)`

## Image Overlays in OpenGL

Provides settings for overlaying static images onto a video stream within an OpenGL context.

### Overlay Settings (`GLOverlaySettings`)

Defines the properties of an image overlay.

**Properties:**

| Property   | Type     | Default | Description                                       |
|------------|----------|---------|---------------------------------------------------|
| `Filename` | `string` | (N/A)   | Path to the image file (read-only after init).    |
| `Data`     | `byte[]` | (N/A)   | Image data as a byte array (read-only after init).|
| `X`        | `int`    |         | X-coordinate of the overlay's top-left corner.    |
| `Y`        | `int`    |         | Y-coordinate of the overlay's top-left corner.    |
| `Width`    | `int`    |         | Width of the overlay.                             |
| `Height`   | `int`    |         | Height of the overlay.                            |
| `Alpha`    | `double` | `1.0`   | Opacity of the overlay (0.0 transparent to 1.0 opaque). |

**Constructor:**
- `GLOverlaySettings(string filename)`

## OpenGL Video Mixing

These types are used to configure an OpenGL-based video mixer, allowing multiple video streams to be combined and composited.

### Mixer Settings (`GLVideoMixerSettings`)

Extends `VideoMixerBaseSettings` for OpenGL-specific video mixing. It manages a list of `GLVideoMixerStream` objects and inherits properties like `Width`, `Height`, and `FrameRate`.

**Methods:**
- `AddStream(GLVideoMixerStream stream)`: Adds a stream to the mixer.
- `RemoveStream(GLVideoMixerStream stream)`: Removes a stream from the mixer.
- `SetStream(int index, GLVideoMixerStream stream)`: Replaces a stream at a specific index.

**Constructors:**
- `GLVideoMixerSettings(int width, int height, VideoFrameRate frameRate)`
- `GLVideoMixerSettings(int width, int height, VideoFrameRate frameRate, List<VideoMixerStream> streams)`

### Mixer Stream (`GLVideoMixerStream`)

Extends `VideoMixerStream` and defines properties for an individual stream within the OpenGL video mixer. Inherits `Rectangle`, `ZOrder`, and `Alpha` from `VideoMixerStream`.

**Properties:**

| Property                        | Type                          | Default                      | Description                                      |
|---------------------------------|-------------------------------|------------------------------|--------------------------------------------------|
| `Crop`                          | `Rect`                        | (N/A)                        | Crop rectangle for the input stream.             |
| `BlendConstantColorAlpha`       | `double`                      | `0`                          | Alpha component for constant blend color.        |
| `BlendConstantColorBlue`        | `double`                      | `0`                          | Blue component for constant blend color.         |
| `BlendConstantColorGreen`       | `double`                      | `0`                          | Green component for constant blend color.        |
| `BlendConstantColorRed`         | `double`                      | `0`                          | Red component for constant blend color.          |
| `BlendEquationAlpha`            | `GLVideoMixerBlendEquation`   | `Add`                        | Blend equation for the alpha channel.            |
| `BlendEquationRGB`              | `GLVideoMixerBlendEquation`   | `Add`                        | Blend equation for RGB channels.                 |
| `BlendFunctionDestinationAlpha` | `GLVideoMixerBlendFunction`   | `OneMinusSourceAlpha`        | Blend function for destination alpha.            |
| `BlendFunctionDesctinationRGB`  | `GLVideoMixerBlendFunction`   | `OneMinusSourceAlpha`        | Blend function for destination RGB.              |
| `BlendFunctionSourceAlpha`      | `GLVideoMixerBlendFunction`   | `One`                        | Blend function for source alpha.                 |
| `BlendFunctionSourceRGB`        | `GLVideoMixerBlendFunction`   | `SourceAlpha`                | Blend function for source RGB.                   |

**Constructor:**
- `GLVideoMixerStream(Rect rectangle, uint zorder, double alpha = 1.0)`

### Blend Equation (`GLVideoMixerBlendEquation` Enum)

Specifies how source and destination colors are combined during blending.

| Value             | Description                                     |
|-------------------|-------------------------------------------------|
| `Add`             | Source + Destination                            |
| `Subtract`        | Source - Destination                            |
| `ReverseSubtract` | Destination - Source                            |

### Blend Function (`GLVideoMixerBlendFunction` Enum)

Defines factors for source and destination colors in blending operations. (Rs, Gs, Bs, As are source color components; Rd, Gd, Bd, Ad are destination; Rc, Gc, Bc, Ac are constant color components).

| Value                      | Description                                 |
|----------------------------|---------------------------------------------|
| `Zero`                     | Factor is (0, 0, 0, 0).                     |
| `One`                      | Factor is (1, 1, 1, 1).                     |
| `SourceColor`              | Factor is (Rs, Gs, Bs, As).                 |
| `OneMinusSourceColor`      | Factor is (1-Rs, 1-Gs, 1-Bs, 1-As).         |
| `DestinationColor`         | Factor is (Rd, Gd, Bd, Ad).                 |
| `OneMinusDestinationColor` | Factor is (1-Rd, 1-Gd, 1-Bd, 1-Ad).         |
| `SourceAlpha`              | Factor is (As, As, As, As).                 |
| `OneMinusSourceAlpha`      | Factor is (1-As, 1-As, 1-As, 1-As).         |
| `DestinationAlpha`         | Factor is (Ad, Ad, Ad, Ad).                 |
| `OneMinusDestinationAlpha` | Factor is (1-Ad, 1-Ad, 1-Ad, 1-Ad).         |
| `ConstantColor`            | Factor is (Rc, Gc, Bc, Ac).                 |
| `OneMinusContantColor`     | Factor is (1-Rc, 1-Gc, 1-Bc, 1-Ac).         |
| `ConstantAlpha`            | Factor is (Ac, Ac, Ac, Ac).                 |
| `OneMinusContantAlpha`     | Factor is (1-Ac, 1-Ac, 1-Ac, 1-Ac).         |
| `SourceAlphaSaturate`      | Factor is (min(As, 1-Ad), min(As, 1-Ad), min(As, 1-Ad), 1). |

## Virtual Test Sources for OpenGL

These settings classes are used to configure virtual sources that generate test patterns directly within an OpenGL context.

### Virtual Video Source Settings (`GLVirtualVideoSourceSettings`)

Configures a source block (`GLVirtualVideoSourceBlock`) that produces test video data. Implements `IMediaPlayerBaseSourceSettings` and `IVideoCaptureBaseVideoSourceSettings`.

**Properties:**

| Property    | Type                       | Default                | Description                                      |
|-------------|----------------------------|------------------------|--------------------------------------------------|
| `Width`     | `int`                      | `1280`                 | Width of the output video.                       |
| `Height`    | `int`                      | `720`                  | Height of the output video.                      |
| `FrameRate` | `VideoFrameRate`           | `30/1` (30 fps)        | Frame rate of the output video.                  |
| `IsLive`    | `bool`                     | `true`                 | Indicates if the source is live.                 |
| `Mode`      | `GLVirtualVideoSourceMode` | (N/A - must be set)    | Specifies the type of test pattern to generate.  |

**Associated Enum: `GLVirtualVideoSourceMode`**

Defines the test pattern generated by `GLVirtualVideoSourceBlock`.

| Value         | Description                  |
|---------------|------------------------------|
| `SMPTE`       | SMPTE 100% color bars.       |
| `Snow`        | Random (television snow).    |
| `Black`       | 100% Black.                  |
| `White`       | 100% White.                  |
| `Red`         | Solid Red color.             |
| `Green`       | Solid Green color.           |
| `Blue`        | Solid Blue color.            |
| `Checkers1`   | Checkerboard pattern (1px).  |
| `Checkers2`   | Checkerboard pattern (2px).  |
| `Checkers4`   | Checkerboard pattern (4px).  |
| `Checkers8`   | Checkerboard pattern (8px).  |
| `Circular`    | Circular pattern.            |
| `Blink`       | Blinking pattern.            |
| `Mandelbrot`  | Mandelbrot fractal.          |

**Methods:**
- `Task<MediaFileInfo> ReadInfoAsync()`: Asynchronously reads media information (returns synthetic info based on settings).
- `MediaBlock CreateBlock()`: Creates a `GLVirtualVideoSourceBlock` instance configured with these settings.
