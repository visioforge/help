---
title: Filtre mélangeur vidéo DirectShow — API PIP et chroma key
description: Interface IVFVideoMixer pour mélanger 2 à 16 sources vidéo avec PIP, incrustation chroma, transparence et configurations de disposition personnalisables.
tags:
  - DirectShow
  - C++
  - Windows
  - Effects
  - Mixing
  - Webcam
  - C#
primary_api_classes:
  - IVFVideoMixer
  - IBaseFilter
  - IVFChromaKey

---

# Référence de l'interface IVFVideoMixer

## Vue d'ensemble

L'interface `IVFVideoMixer` offre un contrôle complet du mélange vidéo multi-source dans les applications DirectShow. Cette interface permet l'image dans l'image (PIP), la composition vidéo, l'incrustation chroma et la gestion souple de la disposition pour combiner plusieurs flux vidéo en une seule sortie.

Le filtre Video Mixer peut gérer 2 à 16 sources vidéo en entrée, chacune avec une configuration indépendante de position, taille, transparence et ordre Z.

## Définition de l'interface

- **Nom de l'interface** : `IVFVideoMixer`
- **GUID** : `{3318300E-F6F1-4d81-8BC3-9DB06B09F77A}`
- **Hérite de** : `IUnknown`
- **Fichier d'en-tête** : `yk_video_mixer_filter_define.h` (C++), `IVFVideoMixer.cs` (.NET)

## Capacités

- **Pins d'entrée** : 2 à 16 sources vidéo simultanées
- **Incrustation chroma** : prise en charge fond vert/bleu par entrée
- **Qualité de redimensionnement** : plusieurs algorithmes d'interpolation
- **Ordre Z** : contrôle indépendant des couches
- **Transparence** : fusion alpha par entrée
- **Position/taille** : placement précis au pixel

---
## Référence des méthodes
### Gestion des paramètres d'entrée
#### SetInputParam
Configure les paramètres d'une pin d'entrée spécifique.
**Syntaxe (C++)** :
```cpp
int SetInputParam(int pin_index, VFPIPVideoInputParam param);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetInputParam([In] int pin_index, [In] VFPIPVideoInputParam param);
```
**Paramètres** :
- `pin_index` : index de la pin d'entrée à partir de zéro (0 = première entrée, 1 = deuxième, etc.)
- `param` : structure contenant la configuration de l'entrée (voir ci-dessous)
**Retour** : `0` en cas de succès, code d'erreur sinon.
**Structure VFPIPVideoInputParam** :
| Champ | Type | Description |
|-------|------|-------------|
| `Enabled` | bool | Activer/désactiver cette entrée |
| `Left` | int | Position X (pixels) |
| `Top` | int | Position Y (pixels) |
| `Width` | int | Largeur (pixels) |
| `Height` | int | Hauteur (pixels) |
| `Alpha` | int | Transparence (0-255, 255=opaque) |
| `Visible` | bool | Indicateur de visibilité |
| `ZOrder` | int | Ordre des couches (plus élevé = premier plan) |
| `StretchMode` | VFPIPResizeQuality | Qualité de redimensionnement |
**Remarques d'utilisation** :
- La pin 0 est généralement la source d'arrière-plan/principale
- Les pins 1+ sont des sources de superposition
- La position (0,0) est le coin supérieur gauche
- La taille peut différer de la résolution source (mise à l'échelle automatique)
- La fusion alpha nécessite une certaine charge GPU
**Exemple (C++)** :
```cpp
IVFVideoMixer* pMixer = nullptr;
pFilter->QueryInterface(IID_IVFVideoMixer, (void**)&pMixer);
// Configurer la deuxieme entree (superposition)
VFPIPVideoInputParam param;
param.Enabled = true;
param.Visible = true;
param.Left = 50;
param.Top = 50;
param.Width = 640;
param.Height = 360;
param.Alpha = 255;        // Entierement opaque
param.ZOrder = 10;        // Au-dessus de l'arriere-plan
pMixer->SetInputParam(1, param);
pMixer->Release();
```
**Exemple (C#)** :
```csharp
var mixer = filter as IVFVideoMixer;
// Configurer un PIP dans le coin inferieur droit
var param = new VFPIPVideoInputParam
{
    Enabled = true,
    Visible = true,
    Left = 1600,          // En supposant une sortie 1920x1080
    Top = 820,
    Width = 320,          // Petit PIP
    Height = 180,
    Alpha = 255,
    ZOrder = 100          // Couche superieure
};
mixer.SetInputParam(1, param);
```
---

#### GetInputParam

Récupère les paramètres actuels d'une pin d'entrée spécifique.

**Syntaxe (C++)** :
```cpp
int GetInputParam(int pin_index, VFPIPVideoInputParam *param);
```

**Syntaxe (C#)** :
```csharp
[PreserveSig]
int GetInputParam([In] int pin_index, [Out] out VFPIPVideoInputParam param);
```

**Paramètres** :
- `pin_index` : index de la pin d'entrée à partir de zéro
- `param` : [out] reçoit la configuration actuelle de l'entrée

**Retour** : `0` en cas de succès.

**Exemple (C++)** :
```cpp
VFPIPVideoInputParam param;
pMixer->GetInputParam(1, &param);

printf("Input 1 position: %d,%d\n", param.Left, param.Top);
printf("Input 1 size: %dx%d\n", param.Width, param.Height);
```

---
#### GetInputParam2
Récupère les paramètres par référence d'interface de pin plutôt que par index.
**Syntaxe (C++)** :
```cpp
int GetInputParam2(IPin *pin, VFPIPVideoInputParam *param);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int GetInputParam2([In] object pin, [Out] out VFPIPVideoInputParam param);
```
**Paramètres** :
- `pin` : pointeur d'interface DirectShow IPin
- `param` : [out] reçoit la configuration d'entrée
**Retour** : `0` en cas de succès.
**Remarques d'utilisation** :
- Alternative à GetInputParam lorsque vous disposez d'une référence de pin
- Utile lors de l'énumération dynamique des pins
---

### Configuration de la sortie

#### SetOutputParam

Configure le format vidéo de sortie du mélangeur.

**Syntaxe (C++)** :
```cpp
int SetOutputParam(VFPIPVideoOutputParam param);
```

**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetOutputParam([In] VFPIPVideoOutputParam param);
```

**Paramètres** :
- `param` : structure de configuration de sortie

**Structure VFPIPVideoOutputParam** :

| Champ | Type | Description |
|-------|------|-------------|
| `Width` | int | Largeur de sortie (pixels) |
| `Height` | int | Hauteur de sortie (pixels) |
| `FrameRate` | double | Fréquence d'images de sortie (fps) |
| `BackgroundColor` | COLORREF | Couleur d'arrière-plan (RGB) |

**Remarques d'utilisation** :
- Doit être appelée avant de connecter les filtres en aval
- Toutes les entrées sont mises à l'échelle/positionnées par rapport à la taille de sortie
- La fréquence d'images peut différer des entrées (le mélangeur gère le cadencement)

**Exemple (C++)** :
```cpp
VFPIPVideoOutputParam output;
output.Width = 1920;
output.Height = 1080;
output.FrameRate = 30.0;
output.BackgroundColor = RGB(0, 0, 0);  // Arriere-plan noir

pMixer->SetOutputParam(output);
```

**Exemple (C#)** :
```csharp
var output = new VFPIPVideoOutputParam
{
    Width = 1280,
    Height = 720,
    FrameRate = 60.0,
    BackgroundColor = 0x003300  // Vert fonce
};

mixer.SetOutputParam(output);
```

---
#### GetOutputParam
Récupère la configuration de sortie actuelle.
**Syntaxe (C++)** :
```cpp
int GetOutputParam(VFPIPVideoOutputParam *param);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int GetOutputParam([Out] out VFPIPVideoOutputParam param);
```
**Paramètres** :
- `param` : [out] reçoit la configuration de sortie
**Retour** : `0` en cas de succès.
---

### Configuration de l'incrustation chroma

#### SetChromaSettings

Configure les paramètres d'incrustation chroma (fond vert/bleu) pour la composition.

**Syntaxe (C++)** :
```cpp
int SetChromaSettings(bool enabled, int color, int tolerance1, int tolerance2);
```

**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetChromaSettings([In, MarshalAs(UnmanagedType.Bool)] bool enabled,
                      int color,
                      int tolerance1,
                      int tolerance2);
```

**Paramètres** :
- `enabled` : activer/désactiver l'incrustation chroma
- `color` : couleur clé (0=vert, 1=bleu, 2=rouge, ou RGB personnalisé)
- `tolerance1` : tolérance de correspondance de couleur (0-255)
- `tolerance2` : tolérance des bords (0-255)

**Retour** : `0` en cas de succès.

**Remarques d'utilisation** :
- S'applique à toutes les entrées ayant une couleur chroma
- Tolérance plus faible = correspondance de couleur plus stricte
- Tolérance plus élevée = plus de couleurs supprimées (peut affecter le sujet)
- tolerance2 aide au lissage des bords

**Valeurs de couleur chroma** :
- `0` — vert (le plus courant)
- `1` — bleu
- `2` — rouge
- Valeur RGB personnalisée

**Exemple (C++)** :
```cpp
// Activer le fond vert avec une tolerance moderee
pMixer->SetChromaSettings(true, 0, 50, 30);
```

**Exemple (C#)** :
```csharp
// Fond bleu avec tolerance stricte
mixer.SetChromaSettings(true, 1, 30, 20);

// Desactiver l'incrustation chroma
mixer.SetChromaSettings(false, 0, 0, 0);
```

---
### Gestion de l'ordre des couches
#### SetInputOrder
Définit l'ordre Z (ordre des couches) pour une entrée spécifique.
**Syntaxe (C++)** :
```cpp
int SetInputOrder(int pin_index, int order);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetInputOrder(int pin_index, int order);
```
**Paramètres** :
- `pin_index` : index de la pin d'entrée à partir de zéro
- `order` : valeur d'ordre Z (plus élevé = premier plan)
**Retour** : `0` en cas de succès.
**Remarques d'utilisation** :
- Les valeurs d'ordre plus élevées s'affichent au premier plan
- Plage typique : 0-100
- Peut être modifié dynamiquement pendant la lecture
- Alternative à la définition de ZOrder dans VFPIPVideoInputParam
**Exemple (C++)** :
```cpp
// Arriere-plan
pMixer->SetInputOrder(0, 0);
// Couche intermediaire
pMixer->SetInputOrder(1, 50);
// Superposition au premier plan
pMixer->SetInputOrder(2, 100);
```
---

### Configuration de la qualité

#### SetResizeQuality

Définit la qualité/algorithme de redimensionnement pour toutes les entrées.

**Syntaxe (C++)** :
```cpp
int SetResizeQuality(VFPIPResizeQuality quality);
```

**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetResizeQuality(VFPIPResizeQuality quality);
```

**Paramètres** :
- `quality` : mode de qualité de redimensionnement

**Énumération VFPIPResizeQuality** :

| Valeur | Algorithme | Qualité | Vitesse | Cas d'usage |
|-------|-----------|---------|-------|----------|
| **NearestNeighbor** | Pixel le plus proche | Basse | ★★★★★ | Pixel art, aperçu rapide |
| **Bilinear** | Interpolation linéaire | Moyenne | ★★★★☆ | Qualité standard |
| **Bicubic** | Interpolation cubique | Haute | ★★★☆☆ | Haute qualité (par défaut) |
| **Lanczos** | Lanczos-3 | Maximale | ★★☆☆☆ | Qualité professionnelle |

**Remarques d'utilisation** :
- Bicubique est recommandé pour la plupart des usages
- Lanczos pour une qualité maximale lorsque les performances le permettent
- Bilinéaire pour des performances temps réel
- NearestNeighbor uniquement pour des cas particuliers

**Exemple (C++)** :
```cpp
// Melange haute qualite
pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);

// Mode performance
pMixer->SetResizeQuality(VFPIPResizeQuality::Bilinear);
```

---
## Exemples de configuration complets
### Exemple 1 : image dans l'image (C++)
```cpp
#include "yk_video_mixer_filter_define.h"
HRESULT ConfigurePIPLayout(IBaseFilter* pMixerFilter)
{
    HRESULT hr;
    IVFVideoMixer* pMixer = nullptr;
    hr = pMixerFilter->QueryInterface(IID_IVFVideoMixer, (void**)&pMixer);
    if (FAILED(hr))
        return hr;
    // Definir la sortie 1080p
    VFPIPVideoOutputParam output;
    output.Width = 1920;
    output.Height = 1080;
    output.FrameRate = 30.0;
    output.BackgroundColor = RGB(0, 0, 0);
    pMixer->SetOutputParam(output);
    // Configurer la video principale (entree 0 - arriere-plan)
    VFPIPVideoInputParam main;
    main.Enabled = true;
    main.Visible = true;
    main.Left = 0;
    main.Top = 0;
    main.Width = 1920;
    main.Height = 1080;
    main.Alpha = 255;
    main.ZOrder = 0;        // Arriere-plan
    pMixer->SetInputParam(0, main);
    // Configurer le PIP (entree 1 - coin inferieur droit)
    VFPIPVideoInputParam pip;
    pip.Enabled = true;
    pip.Visible = true;
    pip.Left = 1560;        // 1920 - 360 (largeur) + marge
    pip.Top = 860;          // 1080 - 220 (hauteur) + marge
    pip.Width = 360;
    pip.Height = 202;       // Ratio 16:9
    pip.Alpha = 255;
    pip.ZOrder = 100;       // Premier plan
    pMixer->SetInputParam(1, pip);
    // Redimensionnement haute qualite
    pMixer->SetResizeQuality(VFPIPResizeQuality::Bicubic);
    pMixer->Release();
    return S_OK;
}
```
### Exemple 2 : écran partagé (C#)
```csharp
using VisioForge.DirectShowAPI;
public class SplitScreenMixer
{
    public void ConfigureSplitScreen(IBaseFilter mixerFilter)
    {
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer == null)
            throw new NotSupportedException("IVFVideoMixer not available");
        // Sortie 1920x1080
        var output = new VFPIPVideoOutputParam
        {
            Width = 1920,
            Height = 1080,
            FrameRate = 30.0,
            BackgroundColor = 0x000000
        };
        mixer.SetOutputParam(output);
        // Moitie gauche - entree 0
        var leftInput = new VFPIPVideoInputParam
        {
            Enabled = true,
            Visible = true,
            Left = 0,
            Top = 0,
            Width = 960,        // Moitie de la largeur
            Height = 1080,
            Alpha = 255,
            ZOrder = 0
        };
        mixer.SetInputParam(0, leftInput);
        // Moitie droite - entree 1
        var rightInput = new VFPIPVideoInputParam
        {
            Enabled = true,
            Visible = true,
            Left = 960,         // Decalage d'une moitie
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
### Exemple 3 : superposition par incrustation chroma (C++)
```cpp
HRESULT ConfigureChromaKeyOverlay(IVFVideoMixer* pMixer)
{
    // Sortie 1080p
    VFPIPVideoOutputParam output;
    output.Width = 1920;
    output.Height = 1080;
    output.FrameRate = 30.0;
    output.BackgroundColor = RGB(0, 0, 0);
    pMixer->SetOutputParam(output);
    // Scene d'arriere-plan (entree 0)
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
    // Personne devant un fond vert (entree 1)
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
    // Activer l'incrustation chroma fond vert
    pMixer->SetChromaSettings(
        true,   // Activer
        0,      // Vert
        60,     // Tolerance de couleur
        40      // Tolerance des bords
    );
    // Haute qualite pour de meilleurs bords d'incrustation
    pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);
    return S_OK;
}
```
### Exemple 4 : grille multi-caméras (C#)
```csharp
public void Configure2x2Grid(IVFVideoMixer mixer)
{
    // Sortie 1920x1080
    var output = new VFPIPVideoOutputParam
    {
        Width = 1920,
        Height = 1080,
        FrameRate = 30.0,
        BackgroundColor = 0x101010  // Gris fonce
    };
    mixer.SetOutputParam(output);
    int cellWidth = 960;
    int cellHeight = 540;
    int gap = 10;
    // Camera haut-gauche (entree 0)
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
    // Camera haut-droit (entree 1)
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
    // Camera bas-gauche (entree 2)
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
    // Camera bas-droit (entree 3)
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

## Scénarios de mélange courants

### Scénario 1 : style diffusion d'information

```
+------------------------------------------+
|                                          |
|        Camera principale (plein ecran)   |
|                                          |
|                           +-----------+  |
|                           |  Camera   |  |
|                           |  invite   |  |
|                           +-----------+  |
+------------------------------------------+
```

**Configuration** :
- Entrée 0 : caméra principale (1920x1080)
- Entrée 1 : PIP invité (320x180, bas-droit)
- Ordre Z : invité au premier plan
- Qualité de redimensionnement : Bicubic

### Scénario 2 : flux de jeu vidéo

```
+------------------------------------------+
|                                          |
|        Capture du jeu (principale)       |
|                                          |
|  +----------+                            |
|  | Webcam   |                            |
|  +----------+                            |
+------------------------------------------+
```

**Configuration** :
- Entrée 0 : capture du jeu (1920x1080)
- Entrée 1 : webcam (280x210, haut-gauche)
- Optionnel : incrustation chroma si la webcam est devant un fond vert
- Ordre Z : webcam au premier plan

### Scénario 3 : production virtuelle

```
+------------------------------------------+
|                                          |
|    Scene d'arriere-plan (pre-calculee)   |
|                                          |
|         [Personne sur fond vert          |
|          composee au-dessus]             |
|                                          |
+------------------------------------------+
```

**Configuration** :
- Entrée 0 : arrière-plan virtuel
- Entrée 1 : caméra avec fond vert
- Incrustation chroma : activée, verte, tolérance 60/40
- Qualité de redimensionnement : Lanczos pour une qualité de bord optimale

---
## Considérations de performance
### Utilisation CPU/GPU
**Configurations à faible impact** :
- 2-4 entrées
- Redimensionnement bilinéaire
- Pas d'incrustation chroma
- Pas de transparence (Alpha = 255)
**Impact moyen** :
- 5-8 entrées
- Redimensionnement bicubique
- Incrustation chroma basique
- Un peu de transparence
**Impact élevé** :
- 9 entrées ou plus
- Redimensionnement Lanczos
- Incrustation chroma complexe
- Plusieurs couches transparentes
### Conseils d'optimisation
1. **Utilisez une qualité de redimensionnement appropriée** :
   - Aperçu : Bilinear
   - Production : Bicubic
   - Qualité maximale : Lanczos (si les performances le permettent)
2. **Minimisez la charge d'incrustation chroma** :
   - N'activez que si nécessaire
   - Utilisez des valeurs de tolérance serrées
   - Envisagez une alternative accélérée matériellement
3. **Limitez le nombre d'entrées** :
   - Chaque entrée ajoute une charge de traitement
   - Désactivez les entrées non utilisées (Enabled = false)
4. **Faites correspondre les résolutions sources** :
   - Moins de mise à l'échelle = meilleures performances
   - Pré-mettez les sources à l'échelle si possible
---

## Bonnes pratiques

### Conception de la disposition

1. **Planifiez soigneusement l'ordre Z** — arrière-plan le plus bas, superpositions au plus haut
2. **Laissez des marges** — ne placez pas les éléments exactement aux bords
3. **Conservez les rapports d'aspect** — évitez la distorsion
4. **Testez à la résolution cible** — vérifiez la précision du positionnement

### Incrustation chroma

1. **Éclairage approprié** — éclairage uniforme sur le fond vert
2. **Ajustez la tolérance** — commencez bas, augmentez progressivement
3. **Réglage de qualité** — utilisez Lanczos pour de meilleurs bords
4. **Conditions de test** — différents scénarios d'éclairage

### Changements dynamiques

1. **Mettez à jour les paramètres en douceur** — évitez les changements brusques de position
2. **Mettez les configurations en cache** — stockez des préréglages pour un basculement rapide
3. **Validez les paramètres** — vérifiez les limites avant application
4. **Gérez les erreurs** — vérifiez les valeurs de retour

---
## Dépannage
### Problème : vidéo non visible
**Vérifier** :
- `Enabled = true`
- `Visible = true`
- `Alpha > 0`
- Position dans les limites de sortie
- Le filtre source est en cours d'exécution
### Problème : mauvaise qualité de mise à l'échelle
**Solution** :
```cpp
pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);
```
### Problème : l'incrustation chroma ne fonctionne pas
**Vérifier** :
- Paramètres chroma activés
- Couleur correcte sélectionnée (0=vert, 1=bleu)
- Augmentez les valeurs de tolérance
- Vérifiez que la source a un fond vert uniforme
**Exemple** :
```cpp
// Essayer une tolerance plus elevee
pMixer->SetChromaSettings(true, 0, 80, 60);
```
### Problème : performances dégradées
**Solutions** :
- Réduisez le nombre d'entrées actives
- Utilisez une qualité de redimensionnement plus rapide
- Désactivez l'incrustation chroma si elle n'est pas nécessaire
- Pré-mettez les sources d'entrée à l'échelle
---

## Interfaces associées

- **IBaseFilter** — interface de filtre DirectShow
- **IPin** — interface de pin DirectShow (pour GetInputParam2)
- **IVFEffects45** — effets vidéo (combinables avec le mélangeur)
- **IVFChromaKey** — interface d'incrustation chroma dédiée

## Voir aussi

- [Présentation du pack de filtres de traitement](../index.md)
- [Référence des effets](../effects-reference.md)
- [Interface d'incrustation chroma](chroma-key.md)
- [Exemples de code](../examples.md)
