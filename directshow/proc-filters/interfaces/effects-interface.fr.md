---
title: API des effets vidéo et filtres de traitement DirectShow
description: Référence API des filtres de traitement DirectShow pour effets vidéo, amélioration audio, redimensionnement, chroma key et mélange vidéo.
tags:
  - DirectShow
  - C++
  - Windows
  - Effects
  - Mixing
  - Screen Capture
  - C#
primary_api_classes:
  - VideoEffectType
  - IVFEffectsPro
  - IVFResize
  - IBaseFilter
  - IVFAudioEnhancer

---

# Filtres de traitement — référence des interfaces d'effets

## Vue d'ensemble

Ce document fournit une référence API complète pour toutes les interfaces du pack de filtres de traitement DirectShow. Ces interfaces permettent les effets vidéo, l'amélioration audio, l'incrustation chroma, le mélange vidéo, la capture d'écran et des capacités de traitement avancées.

---
## Référence rapide des interfaces
| Interface | GUID | Rôle |
|-----------|------|---------|
| **IVFEffects45** | {5E767DA8-97AF-4607-B95F-8CC6010B84CA} | Effets vidéo simples |
| **IVFEffectsPro** | {9A794ABE-98AD-45AF-BBB0-042172C74C79} | Effets avancés avec sample grabber |
| **IVFResize** | {12BC6F20-2812-4660-8684-10F3FD3B4487} | Redimensionnement et rognage vidéo |
| **IVFVideoMixer** | {3318300E-F6F1-4d81-8BC3-9DB06B09F77A} | Mélange vidéo multi-source |
| **IVFChromaKey** | {AF6E8208-30E3-44f0-AAFE-787A6250BAB3} | Incrustation chroma (fond vert) |
| **IVFAudioEnhancer** | {C2C0512A-AE91-4B4D-B4E0-913A0227DCD7} | Gains des canaux audio |
| **IVFAudioEnhancer3** | {915E95CE-70F6-4FA5-B608-9B0BCDBE06B3} | Sortie audio IEEE flottante |
| **IVFAudioChannelMapper** | {EDB8F865-0A81-4E98-866F-B6F5F17C8FC2} | Mappage des canaux audio |
| **IVFScreenCapture3** | {259E0009-9963-4a71-91AE-34B96D754899} | Configuration de la capture d'écran |
| **IVFMotDetConfig** | {B10E9A0C-3D99-46D4-A397-6E0BC5BC3D76} | Détection de mouvement |
| **IVFPushConfig** | {F1876E64-C7AC-4B5B-8F64-67B5BB8CEAE4} | Configuration de source push |
---

## Interfaces d'effets vidéo

### IVFEffects45

Interface simple pour ajouter et gérer des effets vidéo.

**GUID** : `{5E767DA8-97AF-4607-B95F-8CC6010B84CA}`

**Définition C#** :
```csharp
[ComImport]
[Guid("5E767DA8-97AF-4607-B95F-8CC6010B84CA")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFEffects45
{
    /// <summary>
    /// Ajoute un effet video.
    /// </summary>
    [PreserveSig]
    void add_effect([In] VFVideoEffectSimple effect);

    /// <summary>
    /// Definit les parametres des effets video.
    /// </summary>
    [PreserveSig]
    void set_effect_settings([In] VFVideoEffectSimple effect);

    /// <summary>
    /// Supprime un effet.
    /// </summary>
    [PreserveSig]
    void remove_effect([In] int id);

    /// <summary>
    /// Efface tous les effets.
    /// </summary>
    [PreserveSig]
    void clear_effects();
}
```

**Méthodes** :

| Méthode | Description |
|--------|-------------|
| `add_effect` | Ajoute un nouvel effet vidéo à la chaîne de traitement |
| `set_effect_settings` | Met à jour les paramètres d'un effet existant |
| `remove_effect` | Supprime un effet par son identifiant |
| `clear_effects` | Supprime tous les effets |

**Exemple (C#)** :
```csharp
var effects = filter as IVFEffects45;
if (effects != null)
{
    // Ajouter un effet de flou
    var blur = new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.Blur,
        Enabled = true,
        Id = 1
    };
    effects.add_effect(blur);

    // Ajouter un effet niveaux de gris
    var gray = new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.Greyscale,
        Enabled = true,
        Id = 2
    };
    effects.add_effect(gray);
}
```

**Structure VFVideoEffectSimple** :
```csharp
public struct VFVideoEffectSimple
{
    public VideoEffectType EffectType;      // Type d'effet
    public bool Enabled;                     // Activer/desactiver
    public int Id;                           // Identifiant unique
    public VFTextLogo TextLogo;             // Parametres de logo textuel
    public VFGraphicalLogo GraphicalLogo;   // Parametres de logo image (aussi appele ImageLogo)
}
```

---
### IVFEffectsPro
Interface d'effets avancés avec prise en charge du rappel sample grabber.
**GUID** : `{9A794ABE-98AD-45AF-BBB0-042172C74C79}`
**Définition C#** :
```csharp
[ComImport]
[Guid("9A794ABE-98AD-45AF-BBB0-042172C74C79")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFEffectsPro
{
    /// <summary>
    /// Definit l'etat des composants du filtre.
    /// </summary>
    [PreserveSig]
    void set_enabled(
        [In, MarshalAs(UnmanagedType.Bool)] bool effects,
        [In, MarshalAs(UnmanagedType.Bool)] bool motdet,
        [In, MarshalAs(UnmanagedType.Bool)] bool chroma,
        [In, MarshalAs(UnmanagedType.Bool)] bool sg);
    /// <summary>
    /// Definit le rappel pour buffer RGB24.
    /// </summary>
    [PreserveSig]
    int set_sg_callback_24([MarshalAs(UnmanagedType.FunctionPtr)] BufferCBProc callback);
    /// <summary>
    /// Definit le rappel pour buffer RGB32.
    /// </summary>
    [PreserveSig]
    int set_sg_callback_32([MarshalAs(UnmanagedType.FunctionPtr)] BufferCBProc callback);
    /// <summary>
    /// Definit le handle du sample grabber.
    /// </summary>
    [PreserveSig]
    int put_sg_app_handle(object handle);
    /// <summary>
    /// Definit l'identifiant unique de handle du sample grabber.
    /// </summary>
    [PreserveSig]
    int put_sg_app_handle_id([MarshalAs(UnmanagedType.U4)] uint handle_id);
}
```
**Délégué de rappel de buffer** :
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
**Méthodes** :
| Méthode | Description |
|--------|-------------|
| `set_enabled` | Active/désactive les composants du filtre (effets, détection de mouvement, chroma key, sample grabber) |
| `set_sg_callback_24` | Définit le rappel pour les images au format RGB24 |
| `set_sg_callback_32` | Définit le rappel pour les images au format RGB32 |
| `put_sg_app_handle` | Définit le handle d'application pour les rappels |
| `put_sg_app_handle_id` | Définit l'identifiant unique pour les rappels |
**Exemple (C#)** :
```csharp
var effectsPro = filter as IVFEffectsPro;
if (effectsPro != null)
{
    // Activer les effets et le sample grabber
    effectsPro.set_enabled(
        effects: true,
        motdet: false,
        chroma: false,
        sg: true);
    // Configurer le rappel d'image
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
    // Traiter les donnees de l'image
    // pBuffer pointe vers les donnees RGB32
    return 0;
}
```
---

## Interface de redimensionnement vidéo

### IVFResize

Contrôle le redimensionnement, le rognage, la rotation et la qualité de redimensionnement de la vidéo.

**GUID** : `{12BC6F20-2812-4660-8684-10F3FD3B4487}`

**Définition C#** :
```csharp
[ComImport]
[Guid("12BC6F20-2812-4660-8684-10F3FD3B4487")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFResize
{
    /// <summary>
    /// Definit la resolution.
    /// </summary>
    [PreserveSig]
    int put_Resolution([In] uint x, [In] uint y);

    /// <summary>
    /// Definit le mode de redimensionnement.
    /// </summary>
    [PreserveSig]
    int put_ResizeMode([In] VFResizeMode mode, [In] bool letterbox);

    /// <summary>
    /// Definit les coordonnees de rognage.
    /// </summary>
    [PreserveSig]
    int put_Crop([In] uint left, [In] uint top, [In] uint right, [In] uint bottom);

    /// <summary>
    /// Definit le mode de filtrage.
    /// </summary>
    [PreserveSig]
    int put_FilterMode([In] VFResizeFilterMode mode);

    /// <summary>
    /// Definit le mode de rotation.
    /// </summary>
    [PreserveSig]
    int put_RotateMode([In] VFRotateMode mode);
}
```

**Énumération VFResizeMode** :
```csharp
public enum VFResizeMode
{
    rmStretch = 0,      // Etirer pour remplir (peut deformer)
    rmLetterbox = 1,    // Conserver le ratio avec letterbox
    rmCrop = 2          // Rogner pour remplir
}
```

**Énumération VFResizeFilterMode** :
```csharp
public enum VFResizeFilterMode
{
    NearestNeighbor = 0,    // Le plus rapide, qualite la plus basse
    Bilinear = 1,           // Bonne qualite, rapide
    Bicubic = 2,            // Haute qualite (par defaut)
    Lanczos = 3             // Qualite maximale, plus lent
}
```

**Énumération VFRotateMode** :
```csharp
public enum VFRotateMode
{
    RM_0 = 0,       // Aucune rotation
    RM_90 = 1,      // 90 degres dans le sens horaire
    RM_180 = 2,     // 180 degres
    RM_270 = 3      // 270 degres dans le sens horaire (90 anti-horaire)
}
```

**Exemple (C#)** :
```csharp
var resize = filter as IVFResize;
if (resize != null)
{
    // Redimensionner a 1280x720 avec letterbox
    resize.put_Resolution(1280, 720);
    resize.put_ResizeMode(VFResizeMode.rmLetterbox, true);

    // Utiliser un redimensionnement bicubique haute qualite
    resize.put_FilterMode(VFResizeFilterMode.Bicubic);

    // Rotation de 90 degres
    resize.put_RotateMode(VFRotateMode.RM_90);

    // Rogner de 10 pixels sur chaque cote
    resize.put_Crop(10, 10, 10, 10);
}
```

---
## Interfaces d'amélioration audio
### IVFAudioEnhancer
Contrôle les gains des canaux audio, le gain automatique et la normalisation.
**GUID** : `{C2C0512A-AE91-4B4D-B4E0-913A0227DCD7}`
**Définition C#** :
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
**Paramètres de canaux** :
- `l` — canal gauche
- `c` — canal central
- `r` — canal droit
- `sl` — surround gauche
- `sr` — surround droit
- `lfe` — effets basse fréquence (caisson de basses)
**Exemple (C#)** :
```csharp
var audio = filter as IVFAudioEnhancer;
if (audio != null)
{
    // Activer le gain automatique et la normalisation
    audio.set_auto_gain(true);
    audio.set_normalize(true);
    // Augmenter les canaux gauche et droit de 20 %
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

Active le format de sortie audio à virgule flottante IEEE.

**GUID** : `{915E95CE-70F6-4FA5-B608-9B0BCDBE06B3}`

**Définition C#** :
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

**Exemple (C#)** :
```csharp
var audio3 = filter as IVFAudioEnhancer3;
if (audio3 != null)
{
    // Activer la sortie IEEE flottante pour traitement audio professionnel
    audio3.set_ieee_output_enabled(true);
}
```

---
## Interface de capture d'écran
### IVFScreenCapture3
Contrôle le mode de capture d'écran, la région, la fréquence d'images et la visibilité du curseur de souris.
**GUID** : `{259E0009-9963-4a71-91AE-34B96D754899}`
**Définition C#** :
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
**Énumération VFScreenCaptureMode** :
```csharp
public enum VFScreenCaptureMode
{
    scmScreen = 0,          // Capturer l'ecran complet ou une region
    scmWindow = 1,          // Capturer une fenetre specifique
    scmMemory = 2           // Utiliser un flux memoire comme source
}
```
**Structure VFRect** :
```csharp
public struct VFRect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
```
**Exemple (C#)** :
```csharp
var capture = filter as IVFScreenCapture3;
if (capture != null)
{
    // Initialiser la capture
    capture.init();
    // Capturer a 30 FPS
    capture.set_fps(30.0);
    // Capturer une region specifique
    var rect = new VFRect
    {
        Left = 100,
        Top = 100,
        Right = 1920,
        Bottom = 1080
    };
    capture.set_rect(rect);
    // Afficher le curseur de souris
    capture.set_mouse(true);
    // Capturer l'ecran principal
    capture.set_display_index(0);
    // Definir le mode de capture d'ecran
    capture.set_mode(VFScreenCaptureMode.scmScreen);
}
```
**Exemple : capture de fenêtre** :
```csharp
// Capturer une fenetre specifique
IntPtr windowHandle = FindWindow(null, "Calculator");
if (windowHandle != IntPtr.Zero)
{
    capture.set_mode(VFScreenCaptureMode.scmWindow);
    capture.set_window_handle(windowHandle);
    // Obtenir la taille de la fenetre
    capture.get_window_size(windowHandle, out int width, out int height);
    Console.WriteLine($"Window size: {width}x{height}");
}
```
---

## Structures et énumérations courantes

### VFVideoEffectType

Énumération complète de tous les effets vidéo disponibles.

```csharp
public enum VideoEffectType
{
    Undefined = -1,             // Effet non defini

    // Texte et graphismes
    TextLogo = 0,               // Superposition de texte
    ImageLogo = 1,              // Superposition d'image/logo

    // Filtres de couleur
    Blue,                       // Filtre de couleur bleu
    FilterBlue,                 // Filtre canal bleu
    FilterGreen,                // Filtre canal vert
    FilterRed,                  // Filtre canal rouge
    Green,                      // Filtre de couleur vert
    Red,                        // Filtre de couleur rouge
    Greyscale,                  // Conversion en niveaux de gris

    // Ajustements d'image
    Blur,                       // Effet de flou
    Contrast,                   // Ajustement de contraste
    Darkness,                   // Effet d'assombrissement
    Lightness,                  // Effet d'eclaircissement
    Saturation,                 // Ajustement de saturation
    Sharpen,                    // Effet de renforcement
    Smooth,                     // Effet de lissage/adoucissement

    // Transformations spatiales
    FlipDown,                   // Retournement vertical (deprecie : utiliser FlipVertical)
    FlipRight,                  // Retournement horizontal (deprecie : utiliser FlipHorizontal)
    MirrorHorizontal,           // Miroir horizontal
    MirrorVertical,             // Miroir vertical
    Rotate,                     // Effet de rotation
    Zoom,                       // Effet de zoom
    Pan,                        // Effet de positionnement/panoramique

    // Effets artistiques
    ColorNoise,                 // Bruit chromatique
    MonoNoise,                  // Bruit monochrome
    Mosaic,                     // Effet mosaique/pixelisation
    Posterize,                  // Effet de posterisation
    ShakeDown,                  // Effet de secousse
    Solorize,                   // Effet de solarisation
    Spray,                      // Effet de spray
    Invert,                     // Inverser les couleurs

    // Debruitage
    DenoiseCAST,                // Algorithme de debruitage CAST
    DenoiseAdaptive,            // Debruitage adaptatif
    DenoiseMosquito,            // Reduction du bruit moustique
    DenoiseSNR,                 // Debruitage base SNR
    MaxineDenoise,              // Debruitage IA NVIDIA Maxine

    // Desentrelacement
    DeinterlaceBlend,           // Desentrelacement (methode blend)
    DeinterlaceTriangle,        // Desentrelacement (methode triangle)
    DeinterlaceCAVT,            // Desentrelacement (methode CAVT)

    // Transitions
    FadeIn,                     // Transition fondu entrant
    FadeOut,                    // Transition fondu sortant

    // Effets avances
    ScrollingTextLogo,          // Superposition de texte defilant
    MaxineArtifactReduction,    // Reduction d'artefacts NVIDIA Maxine
    LUT,                        // Etalonnage colorimetrique par LUT
}
```

**Catégories d'effets** :

| Catégorie | Effets |
|----------|---------|
| **Texte et graphismes** | TextLogo, ImageLogo, ScrollingTextLogo |
| **Filtres de couleur** | Blue, FilterBlue, FilterGreen, FilterRed, Green, Red, Greyscale |
| **Ajustements d'image** | Blur, Contrast, Darkness, Lightness, Saturation, Sharpen, Smooth |
| **Spatial** | FlipDown, FlipRight, MirrorHorizontal, MirrorVertical, Rotate, Zoom, Pan |
| **Artistique** | ColorNoise, MonoNoise, Mosaic, Posterize, ShakeDown, Solorize, Spray, Invert |
| **Débruitage** | DenoiseCAST, DenoiseAdaptive, DenoiseMosquito, DenoiseSNR, MaxineDenoise |
| **Désentrelacement** | DeinterlaceBlend, DeinterlaceTriangle, DeinterlaceCAVT |
| **Transitions** | FadeIn, FadeOut |
| **Avancés** | LUT, MaxineArtifactReduction |

**Effets NVIDIA Maxine** (nécessitent un GPU NVIDIA RTX) :
- `MaxineDenoise` — débruitage vidéo accéléré par IA
- `MaxineArtifactReduction` — réduction des artefacts de compression

---
### VFTextLogo
Structure complète de configuration de logo textuel.
```csharp
[StructLayout(LayoutKind.Sequential)]
public struct VFTextLogo
{
    public int X;                       // Position X
    public int Y;                       // Position Y
    public bool TransparentBg;          // Arriere-plan transparent
    public int FontSize;                // Taille de police (points)
    public bool FontItalic;             // Style italique
    public bool FontBold;               // Style gras
    public bool FontUnderline;          // Style souligne
    public bool FontStrikeout;          // Style barre
    public int FontColor;               // Couleur de police (RGB)
    public int BGColor;                 // Couleur d'arriere-plan
    public bool RightToLeft;            // Texte de droite a gauche
    public bool Vertical;               // Texte vertical
    public int Align;                   // Alignement du texte
    public int DrawQuality;             // Qualite de rendu
    public int Antialiasing;            // Mode d'anticrenelage
    public int RectWidth;               // Largeur du rect englobant
    public int RectHeight;              // Hauteur du rect englobant
    public int RotationMode;            // Rotation du texte
    public int FlipMode;                // Mode de retournement du texte
    public int Transp;                  // Transparence (0-255)
    public bool Gradient;               // Activer le degrade
    public int GradientMode;            // Direction du degrade
    public int GradientColor1;          // Couleur de debut du degrade
    public int GradientColor2;          // Couleur de fin du degrade
    public int InnerBorderColor;        // Couleur de bordure interne
    public int OuterBorderColor;        // Couleur de bordure externe
    public int InnerBorderSize;         // Largeur de bordure interne
    public int OuterBorderSize;         // Largeur de bordure externe
    public int DrawMode;                // Mode de rendu
    public int BorderMode;              // Style de bordure
    public int EffectMode;              // Mode d'effet
    public int ShapeType;               // Forme du texte
}
```
---

### VFGraphicalLogo

Structure de configuration de logo image.

```csharp
[StructLayout(LayoutKind.Sequential)]
public struct VFGraphicalLogo
{
    public int X;                       // Position X
    public int Y;                       // Position Y
    public int Width;                   // Largeur de l'image
    public int Height;                  // Hauteur de l'image
    public int Transp;                  // Transparence (0-255)
    public int RotationMode;            // Angle de rotation
    public int FlipMode;                // Mode de retournement
    public VFVideoEffectStretchMode StretchMode;  // Mode d'etirement
}
```

---
## Exemples de configuration complets
### Exemple 1 : superposition de texte professionnel (C#)
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
            FontColor = 0xFFFFFF,          // Blanc
            TransparentBg = true,
            Antialiasing = 2,               // Haute qualite
            Transp = 230,                   // Legerement transparent
            Gradient = true,
            GradientMode = 0,               // Horizontal
            GradientColor1 = 0xFFFFFF,      // Blanc
            GradientColor2 = 0x0080FF,      // Orange
            BorderMode = 6,                 // Contour plein
            OuterBorderColor = 0x000000,    // Noir
            OuterBorderSize = 2
        }
    };
    effects.add_effect(textEffect);
}
```
### Exemple 2 : chaîne d'effets multiples (C#)
```csharp
public void ApplyMultipleEffects(IBaseFilter effectsFilter)
{
    var effects = effectsFilter as IVFEffects45;
    if (effects == null) return;
    // 1. Desentrelacement
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.DeinterlaceBlend,
        Enabled = true,
        Id = 1
    });
    // 2. Debruitage
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.DenoiseAdaptive,
        Enabled = true,
        Id = 2
    });
    // 3. Ajuster le contraste
    effects.add_effect(new VFVideoEffectSimple
    {
        EffectType = VideoEffectType.Contrast,
        Enabled = true,
        Id = 3
    });
    // 4. Ajouter un filigrane
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
### Exemple 3 : redimensionnement haute qualité avec rotation (C#)
```csharp
public void ConfigureResizeAndRotate(IBaseFilter resizeFilter)
{
    var resize = resizeFilter as IVFResize;
    if (resize == null) return;
    // Redimensionner en 4K
    resize.put_Resolution(3840, 2160);
    // Conserver le ratio avec letterbox
    resize.put_ResizeMode(VFResizeMode.rmLetterbox, true);
    // Utiliser l'algorithme Lanczos de qualite maximale
    resize.put_FilterMode(VFResizeFilterMode.Lanczos);
    // Rotation 90 degres horaire
    resize.put_RotateMode(VFRotateMode.RM_90);
    // Pas de rognage
    resize.put_Crop(0, 0, 0, 0);
}
```
### Exemple 4 : sample grabber avec effets (C#)
```csharp
public class EffectsWithFrameCapture
{
    private IVFEffectsPro _effectsPro;
    public void Setup(IBaseFilter effectsFilter)
    {
        _effectsPro = effectsFilter as IVFEffectsPro;
        if (_effectsPro == null) return;
        // Activer les effets et le sample grabber
        _effectsPro.set_enabled(
            effects: true,
            motdet: false,
            chroma: false,
            sg: true);
        // Configurer le rappel
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
        // Traiter l'image
        Console.WriteLine($"Frame: {width}x{height}, {bufferLen} bytes");
        // Possibilite de modifier les donnees de l'image dans pBuffer
        // Definir updateFrame = true pour mettre a jour l'image
        return 0;
    }
}
```
---

## Voir aussi

- [Interface du mélangeur vidéo](video-mixer.md) — mélange vidéo multi-source
- [Interface d'incrustation chroma](chroma-key.md) — composition fond vert
- [Référence des effets](../effects-reference.md) — catalogue complet des effets
- [Présentation du pack de filtres de traitement](../index.md)
- [Exemples de code](../examples.md)
