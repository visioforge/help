---
title: Filtros de Procesamiento - Referencia de Interfaces de Efectos
description: API de Filtros DirectShow: efectos de video, mejora de audio, redimensionamiento, chroma key e interfaces de mezcla con documentación.
---

# Filtros de Procesamiento - Referencia de Interfaces de Efectos

## Descripción General

Este documento proporciona una referencia completa de la API para todas las interfaces en el Paquete de Filtros de Procesamiento de DirectShow. Estas interfaces permiten efectos de video, mejora de audio, chroma keying, mezcla de video, captura de pantalla y capacidades de procesamiento avanzadas.

---
## Referencia Rápida de Interfaces
| Interfaz | GUID | Propósito |
|-----------|------|---------|
| **IVFEffects45** | {5E767DA8-97AF-4607-B95F-8CC6010B84CA} | Efectos de video simples |
| **IVFEffectsPro** | {9A794ABE-98AD-45AF-BBB0-042172C74C79} | Efectos avanzados con sample grabber |
| **IVFResize** | {12BC6F20-2812-4660-8684-10F3FD3B4487} | Redimensionamiento y recorte de video |
| **IVFVideoMixer** | {3318300E-F6F1-4d81-8BC3-9DB06B09F77A} | Mezcla de video de múltiples fuentes |
| **IVFChromaKey** | {AF6E8208-30E3-44f0-AAFE-787A6250BAB3} | Chroma keying (pantalla verde) |
| **IVFAudioEnhancer** | {C2C0512A-AE91-4B4D-B4E0-913A0227DCD7} | Ganancias de canales de audio |
| **IVFAudioEnhancer3** | {915E95CE-70F6-4FA5-B608-9B0BCDBE06B3} | Salida de audio flotante IEEE |
| **IVFAudioChannelMapper** | {EDB8F865-0A81-4E98-866F-B6F5F17C8FC2} | Mapeo de canales de audio |
| **IVFScreenCapture3** | {259E0009-9963-4a71-91AE-34B96D754899} | Configuración de captura de pantalla |
| **IVFMotDetConfig** | {B10E9A0C-3D99-46D4-A397-6E0BC5BC3D76} | Detección de movimiento |
| **IVFPushConfig** | {F1876E64-C7AC-4B5B-8F64-67B5BB8CEAE4} | Configuración de fuente push |
---

## Interfaces de Efectos de Video

### IVFEffects45

Interfaz simple para agregar y gestionar efectos de video.

**GUID**: `{5E767DA8-97AF-4607-B95F-8CC6010B84CA}`

**Definición C#**:
```csharp
[ComImport]
[Guid("5E767DA8-97AF-4607-B95F-8CC6010B84CA")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFEffects45
{
    /// <summary>
    /// Adds video effect.
    /// </summary>
    [PreserveSig]
    void add_effect([In] VFVideoEffectSimple effect);

    /// <summary>
    /// Sets video effects settings.
    /// </summary>
    [PreserveSig]
    void set_effect_settings([In] VFVideoEffectSimple effect);

    /// <summary>
    /// Removes effect.
    /// </summary>
    [PreserveSig]
    void remove_effect([In] int id);

    /// <summary>
    /// Clears effects.
    /// </summary>
    [PreserveSig]
    void clear_effects();
}
```

**Métodos**:

| Método | Descripción |
|--------|-------------|
| `add_effect` | Agrega un nuevo efecto de video a la cadena de procesamiento |
| `set_effect_settings` | Actualiza los parámetros de un efecto existente |
| `remove_effect` | Elimina el efecto por su ID |
| `clear_effects` | Elimina todos los efectos |

**Ejemplo (C#)**:
```csharp
var effects = filter as IVFEffects45;
if (effects != null)
{
    // Add blur effect
    var blur = new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.Blur,
        Enabled = true,
        Id = 1
    };
    effects.add_effect(blur);

    // Add grayscale effect
    var gray = new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.Greyscale,
        Enabled = true,
        Id = 2
    };
    effects.add_effect(gray);
}
```

**Estructura VFVideoEffectSimple**:
```csharp
public struct VFVideoEffectSimple
{
    public VideoEffectType EffectType;      // Effect type
    public bool Enabled;                     // Enable/disable
    public int Id;                           // Unique identifier
    public VFTextLogo TextLogo;             // Text logo parameters
    public VFGraphicalLogo GraphicalLogo;   // Image logo parameters (also called ImageLogo)
}
```

---
### IVFEffectsPro
Interfaz de efectos avanzados con soporte para callback de sample grabber.
**GUID**: `{9A794ABE-98AD-45AF-BBB0-042172C74C79}`
**Definición C#**:
```csharp
[ComImport]
[Guid("9A794ABE-98AD-45AF-BBB0-042172C74C79")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFEffectsPro
{
    /// <summary>
    /// Sets filter parts state.
    /// </summary>
    [PreserveSig]
    void set_enabled(
        [In, MarshalAs(UnmanagedType.Bool)] bool effects,
        [In, MarshalAs(UnmanagedType.Bool)] bool motdet,
        [In, MarshalAs(UnmanagedType.Bool)] bool chroma,
        [In, MarshalAs(UnmanagedType.Bool)] bool sg);
    /// <summary>
    /// Sets callback for RGB24 buffer.
    /// </summary>
    [PreserveSig]
    int set_sg_callback_24([MarshalAs(UnmanagedType.FunctionPtr)] BufferCBProc callback);
    /// <summary>
    /// Sets callback for RGB32 buffer.
    /// </summary>
    [PreserveSig]
    int set_sg_callback_32([MarshalAs(UnmanagedType.FunctionPtr)] BufferCBProc callback);
    /// <summary>
    /// Sets sample grabber handle.
    /// </summary>
    [PreserveSig]
    int put_sg_app_handle(object handle);
    /// <summary>
    /// Sets sample grabber unique handle id.
    /// </summary>
    [PreserveSig]
    int put_sg_app_handle_id([MarshalAs(UnmanagedType.U4)] uint handle_id);
}
```
**Delegado de Callback de Buffer**:
```csharp
public delegate int BufferCBProc(
    [In] IntPtr handle,
    [In] uint handle_id,
    [In] IntPtr pBuffer,
    int bufferLen,
    int width,
    int height,
    long startTime,
    long stopTime,
    [MarshalAs(UnmanagedType.Bool)] ref bool updateFrame);
```
**Métodos**:
| Método | Descripción |
|--------|-------------|
| `set_enabled` | Habilita/deshabilita componentes del filtro (efectos, detección de movimiento, chroma key, sample grabber) |
| `set_sg_callback_24` | Establece callback para cuadros en formato RGB24 |
| `set_sg_callback_32` | Establece callback para cuadros en formato RGB32 |
| `put_sg_app_handle` | Establece el handle de la aplicación para callbacks |
| `put_sg_app_handle_id` | Establece el identificador único para callbacks |
**Ejemplo (C#)**:
```csharp
var effectsPro = filter as IVFEffectsPro;
if (effectsPro != null)
{
    // Enable effects and sample grabber
    effectsPro.set_enabled(
        effects: true,
        motdet: false,
        chroma: false,
        sg: true);
    // Set up frame callback
    effectsPro.put_sg_app_handle(this.Handle);
    effectsPro.put_sg_app_handle_id(12345);
    effectsPro.set_sg_callback_32(OnFrameCallback);
}
private int OnFrameCallback(
    IntPtr handle,
    uint handle_id,
    IntPtr pBuffer,
    int bufferLen,
    int width,
    int height,
    long startTime,
    long stopTime,
    ref bool updateFrame)
{
    // Process frame data
    // pBuffer points to RGB32 data
    return 0;
}
```
---

## Interfaz de Redimensionamiento de Video

### IVFResize

Controla el redimensionamiento de video, recorte, rotación y calidad de redimensionamiento.

**GUID**: `{12BC6F20-2812-4660-8684-10F3FD3B4487}`

**Definición C#**:
```csharp
[ComImport]
[Guid("12BC6F20-2812-4660-8684-10F3FD3B4487")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFResize
{
    /// <summary>
    /// Sets resolution.
    /// </summary>
    [PreserveSig]
    int put_Resolution([In] uint x, [In] uint y);

    /// <summary>
    /// Sets resize mode.
    /// </summary>
    [PreserveSig]
    int put_ResizeMode([In] VFResizeMode mode, [In] bool letterbox);

    /// <summary>
    /// Sets crop coordinates.
    /// </summary>
    [PreserveSig]
    int put_Crop([In] uint left, [In] uint top, [In] uint right, [In] uint bottom);

    /// <summary>
    /// Sets filter mode.
    /// </summary>
    [PreserveSig]
    int put_FilterMode([In] VFResizeFilterMode mode);

    /// <summary>
    /// Sets rotate mode.
    /// </summary>
    [PreserveSig]
    int put_RotateMode([In] VFRotateMode mode);
}
```

**Enumeración VFResizeMode**:
```csharp
public enum VFResizeMode
{
    rmStretch = 0,      // Stretch to fit (may distort)
    rmLetterbox = 1,    // Maintain aspect ratio with letterbox
    rmCrop = 2          // Crop to fit
}
```

**Enumeración VFResizeFilterMode**:
```csharp
public enum VFResizeFilterMode
{
    NearestNeighbor = 0,    // Fastest, lowest quality
    Bilinear = 1,           // Good quality, fast
    Bicubic = 2,            // High quality (default)
    Lanczos = 3             // Highest quality, slower
}
```

**Enumeración VFRotateMode**:
```csharp
public enum VFRotateMode
{
    RM_0 = 0,       // No rotation
    RM_90 = 1,      // 90 degrees clockwise
    RM_180 = 2,     // 180 degrees
    RM_270 = 3      // 270 degrees clockwise (90 CCW)
}
```

**Ejemplo (C#)**:
```csharp
var resize = filter as IVFResize;
if (resize != null)
{
    // Resize to 1280x720 with letterbox
    resize.put_Resolution(1280, 720);
    resize.put_ResizeMode(VFResizeMode.rmLetterbox, true);

    // Use high quality bicubic resize
    resize.put_FilterMode(VFResizeFilterMode.Bicubic);

    // Rotate 90 degrees
    resize.put_RotateMode(VFRotateMode.RM_90);

    // Crop 10 pixels from each side
    resize.put_Crop(10, 10, 10, 10);
}
```

---
## Interfaces de Mejora de Audio
### IVFAudioEnhancer
Controla las ganancias de canales de audio, ganancia automática y normalización.
**GUID**: `{C2C0512A-AE91-4B4D-B4E0-913A0227DCD7}`
**Definición C#**:
```csharp
[ComImport]
[Guid("C2C0512A-AE91-4B4D-B4E0-913A0227DCD7")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFAudioEnhancer
{
    [PreserveSig]
    int get_auto_gain([Out, MarshalAs(UnmanagedType.Bool)] out bool auto_gain);
    [PreserveSig]
    int set_auto_gain([MarshalAs(UnmanagedType.Bool)] bool auto_gain);
    [PreserveSig]
    int get_normalize([Out, MarshalAs(UnmanagedType.Bool)] out bool normalize);
    [PreserveSig]
    int set_normalize([MarshalAs(UnmanagedType.Bool)] bool normalize);
    [PreserveSig]
    int get_input_gains(out float l, out float c, out float r,
                        out float sl, out float sr, out float lfe);
    [PreserveSig]
    int set_input_gains(float l, float c, float r,
                       float sl, float sr, float lfe);
    [PreserveSig]
    int get_output_gains(out float l, out float c, out float r,
                         out float sl, out float sr, out float lfe);
    [PreserveSig]
    int set_output_gains(float l, float c, float r,
                        float sl, float sr, float lfe);
    [PreserveSig]
    int get_time_shift(out int time_shift);
    [PreserveSig]
    int set_time_shift(int time_shift);
}
```
**Parámetros de Canal**:
- `l` - Canal izquierdo
- `c` - Canal central
- `r` - Canal derecho
- `sl` - Surround izquierdo
- `sr` - Surround derecho
- `lfe` - Efectos de baja frecuencia (subwoofer)
**Ejemplo (C#)**:
```csharp
var audio = filter as IVFAudioEnhancer;
if (audio != null)
{
    // Enable auto gain and normalization
    audio.set_auto_gain(true);
    audio.set_normalize(true);
    // Boost left and right channels by 20%
    audio.set_output_gains(
        l: 1.2f,
        c: 1.0f,
        r: 1.2f,
        sl: 1.0f,
        sr: 1.0f,
        lfe: 1.0f);
}
```
---

### IVFAudioEnhancer3

Habilita el formato de salida de audio de punto flotante IEEE.

**GUID**: `{915E95CE-70F6-4FA5-B608-9B0BCDBE06B3}`

**Definición C#**:
```csharp
[ComImport]
[Guid("915E95CE-70F6-4FA5-B608-9B0BCDBE06B3")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFAudioEnhancer3
{
    [PreserveSig]
    int get_ieee_output_enabled([Out, MarshalAs(UnmanagedType.Bool)] out bool enabled);

    [PreserveSig]
    int set_ieee_output_enabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
}
```

**Ejemplo (C#)**:
```csharp
var audio3 = filter as IVFAudioEnhancer3;
if (audio3 != null)
{
    // Enable IEEE float output for professional audio processing
    audio3.set_ieee_output_enabled(true);
}
```

---
## Interfaz de Captura de Pantalla
### IVFScreenCapture3
Controla el modo de captura de pantalla, región, tasa de cuadros y visibilidad del cursor del mouse.
**GUID**: `{259E0009-9963-4a71-91AE-34B96D754899}`
**Definición C#**:
```csharp
[ComImport]
[Guid("259E0009-9963-4a71-91AE-34B96D754899")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFScreenCapture3
{
    [PreserveSig]
    int init();
    [PreserveSig]
    int set_fps([In] double fps);
    [PreserveSig]
    int set_rect([In] VFRect rect);
    [PreserveSig]
    int set_mouse([In] bool draw);
    [PreserveSig]
    int set_display_index([In] int index);
    [PreserveSig]
    int set_mode([In] VFScreenCaptureMode mode);
    [PreserveSig]
    int refresh_pic();
    [PreserveSig]
    int set_stream([In] IStream stream, [In] long length);
    [PreserveSig]
    int set_window_handle([In] IntPtr handle);
    [PreserveSig]
    int get_window_size([In] IntPtr handle, [Out] out int width, [Out] out int height);
}
```
**Enumeración VFScreenCaptureMode**:
```csharp
public enum VFScreenCaptureMode
{
    scmScreen = 0,          // Capture entire screen or region
    scmWindow = 1,          // Capture specific window
    scmMemory = 2           // Use memory stream as source
}
```
**Estructura VFRect**:
```csharp
public struct VFRect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
```
**Ejemplo (C#)**:
```csharp
var capture = filter as IVFScreenCapture3;
if (capture != null)
{
    // Initialize capture
    capture.init();
    // Capture at 30 FPS
    capture.set_fps(30.0);
    // Capture specific region
    var rect = new VFRect
    {
        Left = 100,
        Top = 100,
        Right = 1920,
        Bottom = 1080
    };
    capture.set_rect(rect);
    // Show mouse cursor
    capture.set_mouse(true);
    // Capture primary display
    capture.set_display_index(0);
    // Set screen capture mode
    capture.set_mode(VFScreenCaptureMode.scmScreen);
}
```
**Ejemplo: Captura de Ventana**:
```csharp
// Capture specific window
IntPtr windowHandle = FindWindow(null, "Calculator");
if (windowHandle != IntPtr.Zero)
{
    capture.set_mode(VFScreenCaptureMode.scmWindow);
    capture.set_window_handle(windowHandle);
    // Get window size
    capture.get_window_size(windowHandle, out int width, out int height);
    Console.WriteLine($"Window size: {width}x{height}");
}
```
---

## Estructuras y Enumeraciones Comunes

### VFVideoEffectType

Enumeración completa de todos los efectos de video disponibles.

```csharp
public enum VideoEffectType
{
    Undefined = -1,             // Undefined effect

    // Text and Graphics
    TextLogo = 0,               // Text overlay
    ImageLogo = 1,              // Image/logo overlay

    // Color Filters
    Blue,                       // Blue color filter
    FilterBlue,                 // Blue channel filter
    FilterGreen,                // Green channel filter
    FilterRed,                  // Red channel filter
    Green,                      // Green color filter
    Red,                        // Red color filter
    Greyscale,                  // Convert to grayscale

    // Image Adjustments
    Blur,                       // Blur effect
    Contrast,                   // Contrast adjustment
    Darkness,                   // Darken effect
    Lightness,                  // Brighten effect
    Saturation,                 // Saturation adjustment
    Sharpen,                    // Sharpen effect
    Smooth,                     // Smooth/soften effect

    // Spatial Transformations
    FlipDown,                   // Flip vertically (deprecated: use FlipVertical)
    FlipRight,                  // Flip horizontally (deprecated: use FlipHorizontal)
    MirrorHorizontal,           // Mirror horizontally
    MirrorVertical,             // Mirror vertically
    Rotate,                     // Rotate effect
    Zoom,                       // Zoom effect
    Pan,                        // Pan/position effect

    // Artistic Effects
    ColorNoise,                 // Color noise
    MonoNoise,                  // Monochrome noise
    Mosaic,                     // Mosaic/pixelate effect
    Posterize,                  // Posterize effect
    ShakeDown,                  // Shake effect
    Solorize,                   // Solarize effect
    Spray,                      // Spray effect
    Invert,                     // Invert colors

    // Denoising
    DenoiseCAST,                // CAST denoising algorithm
    DenoiseAdaptive,            // Adaptive denoising
    DenoiseMosquito,            // Mosquito noise reduction
    DenoiseSNR,                 // SNR-based denoising
    MaxineDenoise,              // NVIDIA Maxine AI denoising

    // Deinterlacing
    DeinterlaceBlend,           // Deinterlace (blend method)
    DeinterlaceTriangle,        // Deinterlace (triangle method)
    DeinterlaceCAVT,            // Deinterlace (CAVT method)

    // Transitions
    FadeIn,                     // Fade-in transition
    FadeOut,                    // Fade-out transition

    // Advanced Effects
    ScrollingTextLogo,          // Scrolling text overlay
    MaxineArtifactReduction,    // NVIDIA Maxine artifact reduction
    LUT,                        // Look-Up Table color grading
}
```

**Categorías de Efectos**:

| Categoría | Efectos |
|----------|---------|
| **Texto y Gráficos** | TextLogo, ImageLogo, ScrollingTextLogo |
| **Filtros de Color** | Blue, FilterBlue, FilterGreen, FilterRed, Green, Red, Greyscale |
| **Ajustes de Imagen** | Blur, Contrast, Darkness, Lightness, Saturation, Sharpen, Smooth |
| **Espacial** | FlipDown, FlipRight, MirrorHorizontal, MirrorVertical, Rotate, Zoom, Pan |
| **Artístico** | ColorNoise, MonoNoise, Mosaic, Posterize, ShakeDown, Solorize, Spray, Invert |
| **Denoising** | DenoiseCAST, DenoiseAdaptive, DenoiseMosquito, DenoiseSNR, MaxineDenoise |
| **Desentrelazado** | DeinterlaceBlend, DeinterlaceTriangle, DeinterlaceCAVT |
| **Transiciones** | FadeIn, FadeOut |
| **Avanzado** | LUT, MaxineArtifactReduction |

**Efectos NVIDIA Maxine** (requieren GPU NVIDIA RTX):
- `MaxineDenoise` - Denoising de video impulsado por IA
- `MaxineArtifactReduction` - Reducción de artefactos de compresión

---
### VFTextLogo
Estructura de configuración de logo de texto completa.
```csharp
[StructLayout(LayoutKind.Sequential)]
public struct VFTextLogo
{
    public int X;                       // X position
    public int Y;                       // Y position
    public bool TransparentBg;          // Transparent background
    public int FontSize;                // Font size (points)
    public bool FontItalic;             // Italic style
    public bool FontBold;               // Bold style
    public bool FontUnderline;          // Underline style
    public bool FontStrikeout;          // Strikeout style
    public int FontColor;               // Font color (RGB)
    public int BGColor;                 // Background color
    public bool RightToLeft;            // RTL text direction
    public bool Vertical;               // Vertical text
    public int Align;                   // Text alignment
    public int DrawQuality;             // Draw quality
    public int Antialiasing;            // Antialiasing mode
    public int RectWidth;               // Bounding rect width
    public int RectHeight;              // Bounding rect height
    public int RotationMode;            // Text rotation
    public int FlipMode;                // Text flip mode
    public int Transp;                  // Transparency (0-255)
    public bool Gradient;               // Enable gradient
    public int GradientMode;            // Gradient direction
    public int GradientColor1;          // Gradient start color
    public int GradientColor2;          // Gradient end color
    public int InnerBorderColor;        // Inner border color
    public int OuterBorderColor;        // Outer border color
    public int InnerBorderSize;         // Inner border width
    public int OuterBorderSize;         // Outer border width
    public int DrawMode;                // Draw mode
    public int BorderMode;              // Border style
    public int EffectMode;              // Effect mode
    public int ShapeType;               // Text shape
}
```
---

### VFGraphicalLogo

Estructura de configuración de logo de imagen.

```csharp
[StructLayout(LayoutKind.Sequential)]
public struct VFGraphicalLogo
{
    public int X;                       // X position
    public int Y;                       // Y position
    public int Width;                   // Image width
    public int Height;                  // Image height
    public int Transp;                  // Transparency (0-255)
    public int RotationMode;            // Rotation angle
    public int FlipMode;                // Flip mode
    public VFVideoEffectStretchMode StretchMode;  // Stretch mode
}
```

---
## Ejemplos de Configuración Completos
### Ejemplo 1: Superposición de Texto Profesional (C#)
```csharp
using VisioForge.DirectShowAPI;
public void AddProfessionalTextOverlay(IBaseFilter effectsFilter)
{
    var effects = effectsFilter as IVFEffects45;
    if (effects == null) return;
    var textEffect = new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.TextLogo,
        Enabled = true,
        Id = 1,
        TextLogo = new VFTextLogo
        {
            X = 50,
            Y = 50,
            FontSize = 36,
            FontBold = true,
            FontColor = 0xFFFFFF,          // White
            TransparentBg = true,
            Antialiasing = 2,               // High quality
            Transp = 230,                   // Slightly transparent
            Gradient = true,
            GradientMode = 0,               // Horizontal
            GradientColor1 = 0xFFFFFF,      // White
            GradientColor2 = 0x0080FF,      // Orange
            BorderMode = 6,                 // Filled outline
            OuterBorderColor = 0x000000,    // Black
            OuterBorderSize = 2
        }
    };
    effects.add_effect(textEffect);
}
```
### Ejemplo 2: Cadena de Múltiples Efectos (C#)
```csharp
public void ApplyMultipleEffects(IBaseFilter effectsFilter)
{
    var effects = effectsFilter as IVFEffects45;
    if (effects == null) return;
    // 1. Deinterlace
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.DeinterlaceBlend,
        Enabled = true,
        Id = 1
    });
    // 2. Denoise
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.DenoiseAdaptive,
        Enabled = true,
        Id = 2
    });
    // 3. Adjust contrast
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.Contrast,
        Enabled = true,
        Id = 3
    });
    // 4. Add watermark
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.ImageLogo,
        Enabled = true,
        Id = 4,
        GraphicalLogo = new VFGraphicalLogo
        {
            X = 1800,
            Y = 50,
            Width = 100,
            Height = 100,
            Transp = 200,
            StretchMode = VFVideoEffectStretchMode.Stretch
        }
    });
}
```
### Ejemplo 3: Redimensionamiento de Alta Calidad con Rotación (C#)
```csharp
public void ConfigureResizeAndRotate(IBaseFilter resizeFilter)
{
    var resize = resizeFilter as IVFResize;
    if (resize == null) return;
    // Resize to 4K
    resize.put_Resolution(3840, 2160);
    // Maintain aspect ratio with letterbox
    resize.put_ResizeMode(VFResizeMode.rmLetterbox, true);
    // Use highest quality Lanczos algorithm
    resize.put_FilterMode(VFResizeFilterMode.Lanczos);
    // Rotate 90 degrees clockwise
    resize.put_RotateMode(VFRotateMode.RM_90);
    // No cropping
    resize.put_Crop(0, 0, 0, 0);
}
```
### Ejemplo 4: Sample Grabber con Efectos (C#)
```csharp
public class EffectsWithFrameCapture
{
    private IVFEffectsPro _effectsPro;
    public void Setup(IBaseFilter effectsFilter)
    {
        _effectsPro = effectsFilter as IVFEffectsPro;
        if (_effectsPro == null) return;
        // Enable effects and sample grabber
        _effectsPro.set_enabled(
            effects: true,
            motdet: false,
            chroma: false,
            sg: true);
        // Set up callback
        _effectsPro.put_sg_app_handle(IntPtr.Zero);
        _effectsPro.put_sg_app_handle_id(1);
        _effectsPro.set_sg_callback_32(OnFrameReceived);
    }
    private int OnFrameReceived(
        IntPtr handle,
        uint handle_id,
        IntPtr pBuffer,
        int bufferLen,
        int width,
        int height,
        long startTime,
        long stopTime,
        ref bool updateFrame)
    {
        // Process frame
        Console.WriteLine($"Frame: {width}x{height}, {bufferLen} bytes");
        // Can modify frame data in pBuffer
        // Set updateFrame = true to update the frame
        return 0;
    }
}
```
---

## Ver También

- [Interfaz de Mezclador de Video](video-mixer.es.md) - Mezcla de video de múltiples fuentes
- [Interfaz de Chroma Key](chroma-key.es.md) - Composición de pantalla verde
- [Referencia de Efectos](../effects-reference.es.md) - Catálogo completo de efectos
- [Descripción General del Paquete de Filtros de Procesamiento](../index.es.md)
- [Ejemplos de Código](../examples.es.md)
