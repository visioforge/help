---
title: Exemples DirectShow — effets vidéo, mélangeur et chroma key
description: Exemples de code pour les filtres d'effets vidéo, mélangeur vidéo et incrustation chroma en C++, C# et VB.NET avec intégration DirectShow.
tags:
  - DirectShow
  - C++
  - Windows
  - Effects
  - Mixing
  - C#
primary_api_classes:
  - IBaseFilter
  - IVFChromaKey
  - IVFVideoMixer

---

# Pack de filtres de traitement — exemples de code

## Vue d'ensemble

Cette page fournit des exemples de code pratiques pour utiliser le pack de filtres de traitement, qui comprend :

- **Effets vidéo** — plus de 35 effets temps réel (texte, graphisme, ajustements colorimétriques, débruitage)
- **Mélangeur vidéo** — mélange de 2 à 16 sources avec PIP, fusion alpha, incrustation chroma
- **Incrustation chroma** — composition fond vert / fond bleu

---
## Prérequis
### Projets C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "IVFEffects45.h"
#include "IVFVideoMixer.h"
#include "IVFChromaKey.h"
#pragma comment(lib, "strmiids.lib")
```
### Projets C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
using System.Drawing;
```
**Paquets NuGet** :
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Exemples d'effets vidéo

### Exemple 1 : effet vidéo de base

Appliquer un effet vidéo unique à une source.

#### Implémentation C#

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

        // Ajouter le filtre source (par ex. File Source)
        filterGraph.AddSourceFilter(filename, "Source", out sourceFilter);

        // Ajouter le filtre d'effets vidéo
        effectFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Vidéo Effects");

        // Configurer l'effet via l'interface IVFEffects45
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Activer l'effet de niveaux de gris via la structure VideoEffectSimple
            var eff = new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Greyscale,
                Enabled = true
            };
            effects.add_effect(eff);
        }
        captureGraph.SetFiltergraph(filterGraph);

        // Effectuer le rendu en passant par le filtre d'effets
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, effectFilter, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        // Configurer la fenêtre vidéo
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Exécuter
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

#### Implémentation C++

```cpp
HRESULT ApplyVideoEffect(LPCWSTR filename)
{
    IGraphBuilder* pGraph = NULL;
    IBaseFilter* pSource = NULL;
    IBaseFilter* pEffect = NULL;
    IVFEffects45* pEffects = NULL;

    // Créer le graphe de filtres
    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Ajouter la source
    hr = pGraph->AddSourceFilter(filename, L"Source", &pSource);
    if (FAILED(hr)) goto cleanup;

    // Créer le filtre d'effets vidéo
    hr = CoCreateInstance(CLSID_VFVideoEffects, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pEffect);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pEffect, L"Vidéo Effects");
    if (FAILED(hr)) goto cleanup;

    // Configurer l'effet
    hr = pEffect->QueryInterface(IID_IVFEffects45, (void**)&pEffects);
    if (SUCCEEDED(hr))
    {
        // Activer le niveau de gris via la structure VideoEffectSimple
        VideoEffectSimple effect;
        ZeroMemory(&effect, sizeof(effect));
        effect.Type = ef_greyscale;
        effect.Enabled = TRUE;
        pEffects->add_effect(&effect);
        pEffects->Release();
    }

    // Connecter les filtres et effectuer le rendu...
    // (Utiliser RenderStream ou ConnectFilters)

cleanup:
    if (pEffect) pEffect->Release();
    if (pSource) pSource->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

---
### Exemple 2 : chaîne d'effets multiples
Appliquer plusieurs effets simultanément.
#### Effets multiples en C#
```csharp
public class MultipleEffectsExample
{
    public void ApplyMultipleEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects != null)
        {
            // Ajouter l'effet d'assombrissement/luminosite (VideoEffectType.Darkness, AmountI contrôle le niveau)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Darkness,
                Enabled = true,
                AmountI = 50        // 0 = plus sombre, 100 = plus lumineux
            });

            // Ajouter l'effet de contraste (AmountI contrôle l'intensité)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Contrast,
                Enabled = true,
                AmountI = 75        // Intensité du contraste
            });

            // Ajouter l'effet de saturation (AmountI contrôle le niveau de saturation)
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.Saturation,
                Enabled = true,
                AmountI = 120       // Niveau de saturation
            });
        }
    }
}
```
---

### Exemple 3 : superposition de texte

Ajouter une superposition de texte/logo à la vidéo.

#### Superposition de texte en C#

```csharp
public void ApplyTextOverlay(IBaseFilter effectFilter)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        var eff = new VideoEffectSimple
        {
            Type = (int)VideoEffectType.TextLogo,
            Enabled = true,
            TextLogo = new MFPTextLogo
            {
                X = 50,
                Y = 50,
                Text = "My Vidéo Title",
                FontName = "Arial",
                FontSize = 36,
                FontColor = 0xFFFFFF,        // Blanc
                FontBold = true,
                TransparentBg = true,
                Transp = 255,                // Entièrement opaque
                BorderMode = 4,              // bm_outline
                OuterBorderColor = 0x000000, // Contour noir
                OuterBorderSize = 2
            }
        };
        effects.add_effect(eff);
    }
}
```

Voir [effects-reference.md](./effects-reference.md) pour la structure complète `MFPTextLogo`
(alignement de texte, dégradé, affichage date/heure, anticrénelage, etc.).

---
### Exemple 4 : superposition d'image
Ajouter un filigrane ou logo graphique.
#### Superposition d'image en C#
```csharp
public void ApplyImageOverlay(IBaseFilter effectFilter, string imagePath)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        var eff = new VideoEffectSimple
        {
            Type = (int)VideoEffectType.ImageLogo,
            Enabled = true,
            GraphicalLogo = new MFPGraphicalLogo
            {
                X = 10,              // Position X en pixels
                Y = 10,              // Position Y en pixels
                Filename = imagePath,
                TranspLevel = 200,   // Semi-transparent (0-255)
                StretchMode = 2      // 0=Aucun, 1=Etirer, 2=Ajustement proportionnel
            }
        };
        effects.add_effect(eff);
    }
}
```

Voir [effects-reference.md](./effects-reference.md) pour la structure complète `MFPGraphicalLogo`.
---

### Exemple 5 : filtres de débruitage

Appliquer une réduction de bruit pour une vidéo plus nette.

#### Exemples de débruitage en C#

```csharp
public void ApplyDenoise(IBaseFilter effectFilter, VideoEffectType denoiseType)
{
    var effects = effectFilter as IVFEffects45;
    if (effects != null)
    {
        var eff = new VideoEffectSimple
        {
            Type = (int)denoiseType,
            Enabled = true
        };

        switch (denoiseType)
        {
            case VideoEffectType.DenoiseCAST:
                // Debruitage CAST — configurer via la sous-structure DenoiseCAST
                eff.DenoiseCAST = new MFPDenoiseCAST
                {
                    TemporalDifferenceThreshold = 16,
                    StrongEdgeThreshold = 8
                };
                break;

            case VideoEffectType.DenoiseAdaptive:
                // Debruitage adaptatif — le seuil contrôle la sensibilite
                eff.DenoiseAdaptiveThreshold = 20;   // 0-255
                eff.DenoiseAdaptiveBlurMode = 0;     // 0-3
                break;

            case VideoEffectType.DenoiseMosquito:
                // Debruitage moustique — AmountI contrôle la force de reduction
                eff.AmountI = 30;
                break;
        }

        effects.add_effect(eff);
    }
}
```

---
### Exemple 6 : tous les effets disponibles
Liste complète des effets avec configuration de base.
#### Référence de tous les effets en C#
```csharp
public class AllEffectsExample
{
    public void DemonstrateAllEffects(IBaseFilter effectFilter)
    {
        var effects = effectFilter as IVFEffects45;
        if (effects == null) return;

        // Filtres de couleur
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Greyscale, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Invert, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FilterRed, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FilterGreen, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FilterBlue, Enabled = true });

        // Ajustement d'image (AmountI contrôle l'intensité)
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Darkness, Enabled = true, AmountI = 50 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Contrast, Enabled = true, AmountI = 75 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Saturation, Enabled = true, AmountI = 120 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Lightness, Enabled = true, AmountI = 45 });

        // Transformations spatiales
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FlipRight, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.FlipDown, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.MirrorHorizontal, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Rotate, Enabled = true, AmountI = 90 });

        // Effets artistiques
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Blur, Enabled = true, AmountI = 5 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Sharpen, Enabled = true, AmountI = 2 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Posterize, Enabled = true, AmountI = 8 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Solorize, Enabled = true, AmountI = 128 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Mosaic, Enabled = true, SizeI = 10 });

        // Reduction de bruit
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DenoiseCAST, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DenoiseAdaptive, Enabled = true, DenoiseAdaptiveThreshold = 20 });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DenoiseMosquito, Enabled = true, AmountI = 30 });

        // Desentrelacement
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DeinterlaceBlend, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DeinterlaceTriangle, Enabled = true });
        effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.DeinterlaceCAVT, Enabled = true });

        // Superpositions (TextLogo/ImageLogo necessitent une sous-structure — voir Exemples 3 et 4)
        // Pour desactiver/supprimer un effet :
        // effects.remove_effect(effectId);
        // effects.clear_effects();
    }
}
```

> **Remarque** : pour la liste complète des membres de `VideoEffectType` et des paramètres de sous-structure,
> consultez [effects-reference.md](./effects-reference.md).
---

## Exemples de mélangeur vidéo

### Exemple 7 : image dans l'image (PIP)

Mélanger deux sources vidéo avec une disposition PIP.

#### Image dans l'image en C#

```csharp
public class VideoMixerPIPExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter mixerFilter;

    public void CreatePIP(string mainVideoPath, string pipVideoPath, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();

        // Ajouter la source vidéo principale
        filterGraph.AddSourceFilter(mainVideoPath, "Main Source", out IBaseFilter mainSource);

        // Ajouter la source vidéo PIP
        filterGraph.AddSourceFilter(pipVideoPath, "PIP Source", out IBaseFilter pipSource);

        // Ajouter le filtre Vidéo Mixer
        mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Vidéo Mixer");

        // Configurer le melangeur — interface réelle IVFVideoMixer
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Définir la taille de sortie
            mixer.SetOutputParam(new VFPIPVideoOutputParam
            {
                Width = 1920,
                Height = 1080,
                FrameRateTime = 30
            });

            // Configurer la vidéo principale (entrée 0) - plein ecran
            mixer.SetInputParam(0, new VFPIPVideoInputParam
            {
                X = 0, Y = 0,
                Width = 1920, Height = 1080,
                Alpha = 255
            });

            // Configurer la vidéo PIP (entrée 1) - coin inferieur droit
            mixer.SetInputParam(1, new VFPIPVideoInputParam
            {
                X = 1440,    // 1920 - 480
                Y = 810,     // 1080 - 270
                Width = 480,
                Height = 270,
                Alpha = 255
            });

            // Définir l'ordre Z (superposition) — par pin, pas en tableau global
            mixer.SetInputOrder(0, 0);  // Principale en arrière
            mixer.SetInputOrder(1, 1);  // PIP au premier plan
        }

        // Connecter les filtres
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Connecter la source principale a l'entrée 0 du melangeur
        captureGraph.RenderStream(null, MediaType.Video, mainSource, null, mixerFilter);

        // Connecter la source PIP a l'entrée 1 du melangeur
        // Remarque : nécessite la connexion a une pin d'entrée specifique
        IPin mixerInput1 = DsFindPin.ByDirection(mixerFilter, PinDirection.Input, 1);
        captureGraph.RenderStream(null, MediaType.Video, pipSource, null, null);
        // Connecter explicitement a mixerInput1...

        // Effectuer le rendu de la sortie du melangeur
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Configurer la fenêtre vidéo
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Exécuter
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

---
### Exemple 8 : mélange multi-source (4 entrées)
Créer une disposition en grille 2x2 avec 4 sources vidéo.
#### Disposition grille 2x2 en C#
```csharp
public class VideoMixerGridExample
{
    public void Create2x2Grid(string[] videoPaths, IntPtr videoWindowHandle)
    {
        if (videoPaths.Length != 4)
        {
            throw new ArgumentException("Requires exactly 4 vidéo sources");
        }
        var filterGraph = (IFilterGraph2)new FilterGraph();
        // Ajouter tous les filtres source
        IBaseFilter[] sources = new IBaseFilter[4];
        for (int i = 0; i < 4; i++)
        {
            filterGraph.AddSourceFilter(videoPaths[i], $"Source {i}", out sources[i]);
        }
        // Ajouter Vidéo Mixer
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Vidéo Mixer");
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            // Définir la taille de sortie
            mixer.SetOutputParam(new VFPIPVideoOutputParam
            {
                Width = 1920,
                Height = 1080,
                FrameRateTime = 30
            });
            int halfWidth = 960;   // 1920 / 2
            int halfHeight = 540;  // 1080 / 2
            // Haut-gauche (Entrée 0)
            mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Haut-droit (Entrée 1)
            mixer.SetInputParam(1, new VFPIPVideoInputParam { X = halfWidth, Y = 0, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Bas-gauche (Entrée 2)
            mixer.SetInputParam(2, new VFPIPVideoInputParam { X = 0, Y = halfHeight, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Bas-droit (Entrée 3)
            mixer.SetInputParam(3, new VFPIPVideoInputParam { X = halfWidth, Y = halfHeight, Width = halfWidth, Height = halfHeight, Alpha = 255 });
            // Définir l'ordre Z (par pin)
            for (int i = 0; i < 4; i++)
            {
                mixer.SetInputOrder(i, i);
            }
        }
        // Connecter les sources au melangeur et effectuer le rendu...
        // (Similaire a l'exemple PIP)
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
    }
}
```
---

### Exemple 9 : mélangeur vidéo avec incrustation chroma

Mélanger des sources avec un arrière-plan transparent.

#### Mélangeur avec incrustation chroma en C#

```csharp
public void CreateMixerWithChromaKey(string backgroundPath, string foregroundPath)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Ajouter les sources
    filterGraph.AddSourceFilter(backgroundPath, "Background", out IBaseFilter bgSource);
    filterGraph.AddSourceFilter(foregroundPath, "Foreground", out IBaseFilter fgSource);

    // Ajouter le melangeur
    var mixerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVideoMixer,
        "Vidéo Mixer");

    var mixer = mixerFilter as IVFVideoMixer;
    if (mixer != null)
    {
        // Configurer la sortie
        mixer.SetOutputParam(new VFPIPVideoOutputParam
        {
            Width = 1920,
            Height = 1080
        });

        // Arrière-plan (plein ecran)
        mixer.SetInputParam(0, new VFPIPVideoInputParam
        {
            X = 0, Y = 0, Width = 1920, Height = 1080
        });

        // Premier plan (centre, plus petit)
        mixer.SetInputParam(1, new VFPIPVideoInputParam
        {
            X = 480, Y = 270, Width = 960, Height = 540
        });

        // Activer l'incrustation chroma — global au melangeur, 4 arguments
        mixer.SetChromaSettings(
            enabled: true,
            color: ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)),  // Vert
            tolerance1: 50,
            tolerance2: 10);
    }

    // Connecter et exécuter...
}
```

---
## Exemples d'incrustation chroma
### Exemple 10 : effet fond vert
Filtre d'incrustation chroma autonome pour suppression du fond vert.
#### Filtre d'incrustation chroma en C#
```csharp
public class ChromaKeyExample
{
    public void ApplyGreenScreen(string videoPath, string backgroundImagePath, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        // Ajouter la source vidéo (avec fond vert)
        filterGraph.AddSourceFilter(videoPath, "Source", out IBaseFilter sourceFilter);
        // Ajouter le filtre Chroma Key
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");
        // Configurer l'incrustation chroma — API IVFChromaKey réelle
        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            // Définir la couleur clé (vert) — int unique via macro RGB
            chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)));

            // Définir la plage de contraste (limites basse, haute)
            chromaKey.put_contrast(50, 100);

            // Définir l'image d'arrière-plan (optionnel)
            if (!string.IsNullOrEmpty(backgroundImagePath))
            {
                chromaKey.put_image(backgroundImagePath);
            }
        }
        // Connecter les filtres : Source -> Chroma Key -> Renderer
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, chromaFilter, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        // Configurer la fenêtre vidéo
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        // Exécuter
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
#### Incrustation chroma en C++
```cpp
HRESULT ApplyChromaKey(IBaseFilter* pChromaFilter)
{
    IVFChromaKey* pChromaKey = NULL;
    HRESULT hr = pChromaFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChromaKey);
    if (FAILED(hr)) return hr;

    // Définir la couleur verte — argument COLORREF unique : RGB(0, 255, 0)
    hr = pChromaKey->put_color(RGB(0, 255, 0));
    if (FAILED(hr)) goto cleanup;

    // Définir la plage de contraste (basse, haute)
    hr = pChromaKey->put_contrast(40, 90);
    if (FAILED(hr)) goto cleanup;

    // Optionnel : définir l'image d'arrière-plan
    hr = pChromaKey->put_image(L"C:\\backgrounds\\studio.jpg");

cleanup:
    pChromaKey->Release();
    return hr;
}
```
---

### Exemple 11 : fond bleu avec réglage fin

Configurer une incrustation chroma fond bleu avec des paramètres optimaux.

#### Fond bleu en C#

```csharp
public void ApplyBlueScreen(IBaseFilter chromaFilter)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Définir la couleur bleue — int unique via RGB
        chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 0, 255)));

        // Plage de contraste ajustee finement pour fond bleu
        // Limite basse plus basse = plus strict (moins de tolerance)
        // Limite haute plus haute = plus laxiste (plus de tolerance)
        chromaKey.put_contrast(30, 80);
    }
}
```

---
### Exemple 12 : incrustation chroma sur couleur personnalisée
Utiliser n'importe quelle couleur personnalisée pour l'incrustation.
#### Couleur personnalisée en C#
```csharp
public void ApplyCustomColorKey(IBaseFilter chromaFilter, Color keyColor)
{
    var chromaKey = chromaFilter as IVFChromaKey;
    if (chromaKey != null)
    {
        // Utiliser n'importe quelle couleur personnalisee — int unique via RGB
        chromaKey.put_color(ColorTranslator.ToWin32(keyColor));

        // Plage de contraste standard
        chromaKey.put_contrast(50, 100);
    }
}
// Exemple d'utilisation :
// ApplyCustomColorKey(chromaFilter, Color.Magenta);  // Fond magenta
// ApplyCustomColorKey(chromaFilter, Color.FromArgb(255, 180, 0, 220));  // Violet personnalise
```
---

## Pipeline de traitement complet

### Exemple 13 : effets, mélange et incrustation chroma combinés

Exemple complet combinant tous les filtres de traitement.

#### Pipeline complet en C#

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

        // 1. Source vidéo principale
        filterGraph.AddSourceFilter(mainVideoPath, "Main Vidéo", out IBaseFilter mainSource);

        // 2. Source vidéo fond vert
        filterGraph.AddSourceFilter(greenScreenVideoPath, "Green Screen", out IBaseFilter gsSource);

        // 3. Ajouter le filtre Chroma Key pour le fond vert
        var chromaFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFChromaKey,
            "Chroma Key");

        var chromaKey = chromaFilter as IVFChromaKey;
        if (chromaKey != null)
        {
            chromaKey.put_color(ColorTranslator.ToWin32(Color.FromArgb(0, 255, 0)));  // Vert
            chromaKey.put_contrast(40, 90);
        }

        // 4. Ajouter le filtre Vidéo Effects
        var effectsFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoEffects,
            "Vidéo Effects");

        var effects = effectsFilter as IVFEffects45;
        if (effects != null)
        {
            // Appliquer des effets via des structures VideoEffectSimple
            effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Darkness, Enabled = true, AmountI = 60 });
            effects.add_effect(new VideoEffectSimple { Type = (int)VideoEffectType.Contrast, Enabled = true, AmountI = 80 });

            // Ajouter une superposition de texte
            effects.add_effect(new VideoEffectSimple
            {
                Type = (int)VideoEffectType.TextLogo,
                Enabled = true,
                TextLogo = new MFPTextLogo
                {
                    Text = "LIVE",
                    FontSize = 48,
                    X = 50,
                    Y = 50,
                    FontColor = 0xFFFFFF,
                    FontBold = true
                }
            });
        }

        // 5. Ajouter le Vidéo Mixer
        var mixerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVideoMixer,
            "Vidéo Mixer");

        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer != null)
        {
            mixer.SetOutputParam(new VFPIPVideoOutputParam { Width = 1920, Height = 1080, FrameRateTime = 30 });

            // Vidéo principale (arrière-plan plein ecran)
            mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = 1920, Height = 1080 });

            // Vidéo chroma-key (PIP)
            mixer.SetInputParam(1, new VFPIPVideoInputParam { X = 1200, Y = 700, Width = 640, Height = 360 });

            // Définir l'ordre Z (par pin)
            mixer.SetInputOrder(0, 0);
            mixer.SetInputOrder(1, 1);
        }

        // Connecter le pipeline :
        // Source principale -> Effects -> Mixer entrée 0
        // Source GS -> Chroma Key -> Mixer entrée 1
        // Mixer -> Renderer

        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Connecter le chemin principal
        captureGraph.RenderStream(null, MediaType.Video, mainSource, effectsFilter, mixerFilter);

        // Connecter le chemin chroma key
        // (Nécessite des connexions au niveau pin sur l'entrée specifique du melangeur)

        // Effectuer le rendu de la sortie du melangeur
        captureGraph.RenderStream(null, MediaType.Video, mixerFilter, null, null);

        // Audio
        captureGraph.RenderStream(null, MediaType.Audio, mainSource, null, null);

        // Configurer la fenêtre vidéo
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        // Exécuter
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

---
## Dépannage
### Problème : effet non visible
**Solution** : assurez-vous que l'effet est activé et que les paramètres sont définis sur la structure VideoEffectSimple :
```csharp
var eff = new VideoEffectSimple
{
    Type = (int)VideoEffectType.Darkness,
    Enabled = true,
    AmountI = 75
};
effects.add_effect(eff);
```
### Problème : incrustation chroma de mauvaise qualité
**Solution** : ajustez les seuils de contraste :
```csharp
// Pour les fonds verts difficiles :
chromaKey.put_contrast(30, 70);  // Plage plus etroite
// Pour les fonds verts bien eclaires :
chromaKey.put_contrast(50, 110);  // Plage plus large
```
### Problème : les entrées du mélangeur vidéo ne s'affichent pas
**Solution** : vérifiez les paramètres d'entrée et l'ordre Z :
```csharp
// Assurez-vous que les entrées sont a l'ecran via la structure VFPIPVideoInputParam
mixer.SetInputParam(0, new VFPIPVideoInputParam { X = 0, Y = 0, Width = 640, Height = 480 });
// Définir l'ordre Z par pin
mixer.SetInputOrder(0, 0);  // Entrée 0 sur la couche 0
mixer.SetInputOrder(1, 1);  // Entrée 1 sur la couche 1
```
---

## Voir aussi

### Documentation

- [Référence des effets](effects-reference.md) — liste complète des effets
- [Interface du mélangeur vidéo](interfaces/video-mixer.md) — référence complète de l'API
- [Interface d'incrustation chroma](interfaces/chroma-key.md) — documentation complète de l'interface

### Ressources externes

- [Graphe de filtres DirectShow](https://learn.microsoft.com/en-us/windows/win32/directshow/building-the-filter-graph)
