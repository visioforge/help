---
title: Référence des effets vidéo DirectShow — 35+ filtres
description: Référence complète de plus de 35 effets vidéo DirectShow incluant filtres de couleur, désentrelacement, débruitage et effets artistiques.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Effects
  - Mixing
  - Webcam
  - C#
primary_api_classes:
  - CVFEffectType
  - CVFEffect
  - CVFGraphicLogoMain
  - CVFStretchMode
  - CVFTextLogoMain

---

# Référence complète des effets vidéo

## Vue d'ensemble

Le pack de filtres de traitement DirectShow fournit plus de 35 effets vidéo en temps réel pouvant être appliqués individuellement ou enchaînés. Cette référence documente tous les effets disponibles, leurs paramètres et leur utilisation.

## Catégories d'effets

- **Texte et graphismes** — logos textuels, superpositions graphiques
- **Filtres de couleur** — filtres rouge, vert, bleu, niveaux de gris
- **Ajustement d'image** — luminosité, contraste, saturation
- **Effets spatiaux** — retournement, miroir, rotation
- **Effets artistiques** — marbre, solarisation, postérisation, mosaïque
- **Bruit et qualité** — algorithmes de débruitage (CAST, adaptatif, moustique)
- **Désentrelacement** — méthodes blend, triangle, CAVT
- **Effets créatifs** — flou, secousse, spray, inversion

---
## Effets de texte et de graphismes
### ef_text_logo
Effectue le rendu d'une superposition de texte sur la vidéo avec de nombreuses options de personnalisation.
**Type d'effet** : `CVFEffectType.ef_text_logo`
**Paramètres** (structure `CVFTextLogoMain`) :
| Paramètre | Type | Description | Valeur par défaut |
|-----------|------|-------------|---------|
| `x` | int | Position X (pixels) | 0 |
| `y` | int | Position Y (pixels) | 0 |
| `text` | BSTR | Texte à afficher | "" |
| `font_name` | BSTR | Nom de la famille de police | "Arial" |
| `font_size` | int | Taille de police (points) | 12 |
| `font_color` | COLORREF | Couleur du texte (RGB) | 0xFFFFFF (blanc) |
| `font_italic` | BOOL | Style italique | FALSE |
| `font_bold` | BOOL | Style gras | FALSE |
| `font_underline` | BOOL | Style souligné | FALSE |
| `font_strikeout` | BOOL | Style barré | FALSE |
| `transparent_bg` | BOOL | Arrière-plan transparent | TRUE |
| `bg_color` | COLORREF | Couleur d'arrière-plan | 0x000000 (noir) |
| `transp` | DWORD | Niveau de transparence (0-255) | 255 (opaque) |
| `align` | CVFTextAlign | Alignement du texte | `al_left` |
| `antialiasing` | CVFTextAntialiasingMode | Mode d'anticrénelage | `am_AntiAlias` |
| `gradient` | BOOL | Activer le dégradé | FALSE |
| `gradientMode` | CVFTextGradientMode | Direction du dégradé | `gm_horizontal` |
| `gradientColor1` | COLORREF | Couleur de début du dégradé | 0xFFFFFF |
| `gradientColor2` | COLORREF | Couleur de fin du dégradé | 0x000000 |
| `borderMode` | CVFTextBorderMode | Style de bordure/contour | `bm_none` |
| `innerBorderColor` | COLORREF | Couleur de bordure interne | 0x000000 |
| `outerBorderColor` | COLORREF | Couleur de bordure externe | 0xFFFFFF |
| `innerBorderSize` | int | Largeur de bordure interne | 1 |
| `outerBorderSize` | int | Largeur de bordure externe | 1 |
| `DateMode` | BOOL | Afficher la date/heure courante | FALSE |
| `DateMask` | BSTR | Chaîne de format de date | "" |
**Options d'alignement du texte** :
- `al_left` — aligné à gauche
- `al_center` — centré
- `al_right` — aligné à droite
**Modes de bordure** :
- `bm_none` — aucune bordure
- `bm_inner` — contour interne
- `bm_outer` — contour externe
- `bm_inner_and_outer` — les deux côtés
- `bm_embossed` — effet 3D en relief
- `bm_outline` — contour standard
- `bm_filled_outline` — contour plein
- `bm_halo` — effet de halo lumineux
**Exemple (C++)** :
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

Superpose une image (logo, filigrane) sur la vidéo.

**Type d'effet** : `CVFEffectType.ef_graphic_logo`

**Paramètres** (structure `CVFGraphicLogoMain`) :

| Paramètre | Type | Description |
|-----------|------|-------------|
| `x` | UINT32 | Position X (pixels) |
| `y` | UINT32 | Position Y (pixels) |
| `Filename` | BSTR | Chemin du fichier image (BMP, PNG, JPG) |
| `hBmp` | int | Handle du bitmap (alternative au nom de fichier) |
| `StretchMode` | CVFStretchMode | Mode de mise à l'échelle de l'image |
| `TranspLevel` | int | Niveau de transparence (0-255) |
| `UseColorKey` | BOOL | Activer la transparence par clé de couleur |
| `ColorKey` | COLORREF | Couleur à rendre transparente |

**Modes d'étirement** :
- `sm_none` — taille d'origine
- `sm_stretch` — étirer pour remplir
- `sm_letterbox` — adapter en conservant le rapport d'aspect
- `sm_crop` — rogner pour remplir

**Exemple (C#)** :
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
## Effets de filtre de couleur
### ef_blue
Applique un filtre de couleur bleu (renforce le bleu, réduit les autres couleurs).
**Type d'effet** : `CVFEffectType.ef_blue`
**Paramètres** :
- `pAmountI` — intensité du filtre (0-100, valeur par défaut : 50)
**Cas d'usage** :
- Teinte bleue artistique
- Atmosphère froide
- Scènes aquatiques / océan
---

### ef_green

Applique un filtre de couleur vert.

**Type d'effet** : `CVFEffectType.ef_green`

**Paramètres** :
- `pAmountI` — intensité du filtre (0-100)

**Cas d'usage** :
- Effet vision nocturne
- Scènes de forêt / nature
- Effet de type Matrix

---
### ef_red
Applique un filtre de couleur rouge.
**Type d'effet** : `CVFEffectType.ef_red`
**Paramètres** :
- `pAmountI` — intensité du filtre (0-100)
**Cas d'usage** :
- Atmosphère chaude
- Effet coucher de soleil
- Scènes d'alerte / danger
---

### ef_filter_blue / ef_filter_blue_2

Filtrage bleu avancé avec différents algorithmes.

**Type d'effet** : `CVFEffectType.ef_filter_blue` ou `ef_filter_blue_2`

**Différence** : `ef_filter_blue_2` utilise un calcul colorimétrique alternatif pour des résultats visuels différents.

---
### ef_filter_green / ef_filter_green2
Filtrage vert avancé (deux variantes).
**Types d'effet** : `CVFEffectType.ef_filter_green`, `ef_filter_green2`
---

### ef_filter_red / ef_filter_red2

Filtrage rouge avancé (deux variantes).

**Types d'effet** : `CVFEffectType.ef_filter_red`, `ef_filter_red2`

---
### ef_greyscale
Convertit la vidéo en noir et blanc.
**Type d'effet** : `CVFEffectType.ef_greyscale`
**Paramètres** : aucun (conversion complète en niveaux de gris)
**Cas d'usage** :
- Rendu cinéma classique
- Effet artistique
- Réduction du bruit chromatique
**Exemple (C++)** :
```cpp
CVFEffect effect;
effect.Type = ef_greyscale;
effect.Enabled = TRUE;
pEffects->add_effect(effect);
```
---

### ef_invert

Inverse toutes les couleurs (image négative).

**Type d'effet** : `CVFEffectType.ef_invert`

**Paramètres** : aucun

**Cas d'usage** :
- Effet artistique
- Apparence radiographique
- Effets visuels spéciaux

---
## Effets d'ajustement d'image
### ef_contrast
Ajuste le contraste de l'image.
**Type d'effet** : `CVFEffectType.ef_contrast`
**Paramètres** :
- `pAmountI` — ajustement du contraste (-100 à +100)
  - Négatif : diminution du contraste
  - Positif : augmentation du contraste
  - Valeur par défaut : 0 (aucun changement)
**Exemple (C#)** :
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_contrast,
    Enabled = true,
    pAmountI = 25  // Augmenter le contraste de 25 %
};
```
---

### ef_lightness

Ajuste la luminosité globale.

**Type d'effet** : `CVFEffectType.ef_lightness`

**Paramètres** :
- `pAmountI` — ajustement de la luminosité (-100 à +100)
  - Négatif : assombrir
  - Positif : éclaircir
  - Valeur par défaut : 0

---
### ef_darkness
Assombrit l'image (opposé de la luminosité).
**Type d'effet** : `CVFEffectType.ef_darkness`
**Paramètres** :
- `pAmountI` — quantité d'assombrissement (0-100)
---

### ef_saturation

Ajuste la saturation des couleurs.

**Type d'effet** : `CVFEffectType.ef_saturation`

**Paramètres** :
- `pAmountI` — ajustement de la saturation (-100 à +100)
  - -100 : niveaux de gris
  - 0 : couleurs d'origine
  - +100 : hyper-saturé

**Cas d'usage** :
- Couleurs vives pour du contenu promotionnel
- Désaturation pour un rendu atténué
- Étalonnage colorimétrique

---
## Effets spatiaux
### ef_flip_down
Retourne la vidéo verticalement (à l'envers).
**Type d'effet** : `CVFEffectType.ef_flip_down`
**Paramètres** : aucun
**Cas d'usage** :
- Correction d'une caméra à l'envers
- Effet miroir avec rotation
- Effets spéciaux
---

### ef_flip_right

Retourne la vidéo horizontalement (miroir).

**Type d'effet** : `CVFEffectType.ef_flip_right`

**Paramètres** : aucun

**Cas d'usage** :
- Mode miroir de webcam
- Correction d'une caméra inversée
- Effets de symétrie

---
### ef_mirror_down
Crée un effet de miroir vertical (le haut se reflète vers le bas).
**Type d'effet** : `CVFEffectType.ef_mirror_down`
---

### ef_mirror_right

Crée un effet de miroir horizontal (la gauche se reflète vers la droite).

**Type d'effet** : `CVFEffectType.ef_mirror_right`

---
## Effets artistiques
### ef_blur
Applique un flou gaussien à l'image.
**Type d'effet** : `CVFEffectType.ef_blur`
**Paramètres** :
- `pAmountI` — intensité du flou (0-100)
- `pSizeI` — taille du noyau de flou (1-20)
**Cas d'usage** :
- Flou d'arrière-plan (simulation de profondeur de champ)
- Adoucissement d'image
- Confidentialité (flouter visages, plaques d'immatriculation)
**Exemple (C++)** :
```cpp
CVFEffect effect;
effect.Type = ef_blur;
effect.Enabled = TRUE;
effect.pAmountI = 50;  // 50 % d'intensite de flou
effect.pSizeI = 10;    // rayon de flou de 10 pixels
pEffects->add_effect(effect);
```
---

### ef_marble

Crée un effet de texture marbre / tourbillon.

**Type d'effet** : `CVFEffectType.ef_marble`

**Paramètres** :
- `pAmountD` — intensité de l'effet (0.0-1.0)
- `pTurbulenceI` — quantité de turbulence (0-100)
- `pScaleD` — facteur d'échelle (0.1-10.0)

**Cas d'usage** :
- Arrière-plan artistique
- Effets de transition
- Visuels psychédéliques

---
### ef_posterize
Réduit le nombre de couleurs (effet affiche d'art).
**Type d'effet** : `CVFEffectType.ef_posterize`
**Paramètres** :
- `pAmountI` — niveaux de couleurs (2-256)
  - Valeurs faibles : moins de couleurs, effet plus marqué
  - Valeurs élevées : plus de couleurs, effet subtil
**Cas d'usage** :
- Style pop art
- Effet bande dessinée
- Réduction de la profondeur colorimétrique
---

### ef_mosaic

Crée un effet pixelisé / mosaïque.

**Type d'effet** : `CVFEffectType.ef_mosaic`

**Paramètres** :
- `pSizeI` — taille des blocs de mosaïque (2-100 pixels)

**Cas d'usage** :
- Confidentialité (flouter des visages / identités)
- Style pixel art rétro
- Censure

**Exemple (C#)** :
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_mosaic,
    Enabled = true,
    pSizeI = 15  // Blocs de 15x15 pixels
};
```

---
### ef_solorize
Crée un effet de solarisation (inversion partielle des couleurs).
**Type d'effet** : `CVFEffectType.ef_solorize` (orthographe héritée conservée dans l'API — utiliser cet identifiant exact)
**Paramètres** :
- `pAmountI` — seuil de solarisation (0-255)
**Cas d'usage** :
- Effet photographique artistique
- Rendu rétro
- Transitions créatives
---

### ef_spray

Crée un effet de pulvérisation / éclaboussure de peinture.

**Type d'effet** : `CVFEffectType.ef_spray`

**Paramètres** :
- `pAmountI` — intensité du spray (0-100)

---
### ef_shake_down
Simule un effet de secousse de caméra à la verticale.
**Type d'effet** : `CVFEffectType.ef_shake_down`
**Paramètres** :
- `pAmountI` — intensité de la secousse (0-100)
**Cas d'usage** :
- Effet tremblement de terre
- Vibration d'impact
- Simulation de caméra à l'épaule
---

## Effets de traitement du bruit

### ef_denoise_cast

Algorithme de débruitage CAST (Combined Adaptive Spatial-Temporal).

**Type d'effet** : `CVFEffectType.ef_denoise_cast`

**Paramètres** (structure `CVFDenoiseCAST`) :

| Paramètre | Plage | Par défaut | Description |
|-----------|-------|---------|-------------|
| `TemporalDifferenceThreshold` | 0-255 | 16 | Seuil de détection de mouvement |
| `NumberOfMotionPixelsThreshold` | 0-16 | 0 | Nb min. de pixels pour le mouvement |
| `StrongEdgeThreshold` | 0-255 | 8 | Préservation des contours |
| `BlockWidth` | 1-16 | 4 | Largeur du bloc de traitement |
| `BlockHeight` | 1-16 | 4 | Hauteur du bloc de traitement |
| `EdgePixelWeight` | 0-255 | 128 | Poids de fusion des contours |
| `NonEdgePixelWeight` | 0-255 | 16 | Poids des zones lisses |
| `GaussianThresholdY` | int | 12 | Seuil de bruit luma |
| `GaussianThresholdUV` | int | 6 | Seuil de bruit chroma |
| `HistoryWeight` | 0-255 | 192 | Force du filtrage temporel |

**Cas d'usage** :
- Nettoyage de vidéo en faible luminosité
- Réduction de bruit de webcam
- Suppression d'artefacts de compression

**Exemple (C++)** :
```cpp
CVFEffect effect;
effect.Type = ef_denoise_cast;
effect.Enabled = TRUE;

// Reduction de bruit moderee
effect.DenoiseCAST.TemporalDifferenceThreshold = 20;
effect.DenoiseCAST.StrongEdgeThreshold = 10;
effect.DenoiseCAST.GaussianThresholdY = 15;
effect.DenoiseCAST.GaussianThresholdUV = 8;

pEffects->add_effect(effect);
```

---
### ef_denoise_adaptive
Débruitage adaptatif qui s'ajuste au contenu de l'image.
**Type d'effet** : `CVFEffectType.ef_denoise_adaptive`
**Paramètres** :
- `pDenoiseAdaptiveThreshold` — seuil de bruit (0-100)
- `pDenoiseAdaptiveBlurMode` — méthode de flou (0-2)
**Cas d'usage** :
- Débruitage général
- Nettoyage vidéo
- Amélioration de la qualité
---

### ef_denoise_mosquito

Réduit le bruit moustique (artefacts de compression autour des contours).

**Type d'effet** : `CVFEffectType.ef_denoise_mosquito`

**Paramètres** :
- `pAmountI` — force de réduction (0-100)

**Cas d'usage** :
- Nettoyage de vidéo fortement compressée
- Suppression des artefacts MPEG / H.264
- Post-traitement pour le streaming

---
### ef_color_noise
Ajoute du bruit chromatique (grain) à l'image.
**Type d'effet** : `CVFEffectType.ef_color_noise`
**Paramètres** :
- `pAmountI` — quantité de bruit (0-100)
**Cas d'usage** :
- Effet grain de pellicule
- Rendu rétro / vintage
- Texture artistique
---

### ef_mono_noise

Ajoute du bruit monochrome (noir et blanc).

**Type d'effet** : `CVFEffectType.ef_mono_noise`

**Paramètres** :
- `pAmountI` — quantité de bruit (0-100)

---
## Effets de désentrelacement
### ef_deint_blend
Mélange les trames entrelacées.
**Type d'effet** : `CVFEffectType.ef_deint_blend`
**Paramètres** (structure `CVFDeintBlend`) :
| Paramètre | Plage | Par défaut | Description |
|-----------|-------|---------|-------------|
| `blendThresh1` | 0-255 | 5 | Premier seuil de fusion |
| `blendThresh2` | 0-255 | 9 | Second seuil de fusion |
| `blendConstants1` | 0.0-1.0 | 0.3 | Premier poids de fusion |
| `blendConstants2` | 0.0-1.0 | 0.7 | Second poids de fusion |
**Cas d'usage** :
- Désentrelacement de vidéo analogique
- Suppression d'artefacts en peigne
- Conversion entrelacé vers progressif
---

### ef_deint_triangle

Désentrelacement par interpolation triangulaire.

**Type d'effet** : `CVFEffectType.ef_deint_triangle`

**Paramètres** :
- `pDeintTriangleWeight` — poids d'interpolation (0-100)

**Qualité** : meilleure préservation des contours que blend

---
### ef_deint_cavt
Désentrelacement CAVT (Content Adaptive Vertical Temporal).
**Type d'effet** : `CVFEffectType.ef_deint_cavt`
**Paramètres** :
- `pDeintCAVTThreshold` — seuil de mouvement (0-100)
**Qualité** : meilleure qualité, plus gourmand en CPU
**Cas d'usage** :
- Désentrelacement haute qualité
- Conversion de vidéo de diffusion
- Traitement archivistique
---

## Chaînage d'effets

Plusieurs effets peuvent être appliqués simultanément. Les effets sont traités dans l'ordre où ils ont été ajoutés.

**Exemple : amélioration de flux professionnel** :
```cpp
// 1. Debruitage
CVFEffect denoise;
denoise.Type = ef_denoise_adaptive;
denoise.Enabled = TRUE;
denoise.pDenoiseAdaptiveThreshold = 15;
pEffects->add_effect(denoise);

// 2. Correction colorimetrique
CVFEffect saturation;
saturation.Type = ef_saturation;
saturation.Enabled = TRUE;
saturation.pAmountI = 15;  // Leger renforcement de saturation
pEffects->add_effect(saturation);

// 3. Ajout d'identite de marque
CVFEffect logo;
logo.Type = ef_graphic_logo;
logo.Enabled = TRUE;
logo.GraphicLogo.Filename = SysAllocString(L"logo.png");
logo.GraphicLogo.x = 20;
logo.GraphicLogo.y = 20;
pEffects->add_effect(logo);

// 4. Ajout d'horodatage
CVFEffect timestamp;
timestamp.Type = ef_text_logo;
timestamp.Enabled = TRUE;
timestamp.TextLogo.DateMode = TRUE;
timestamp.TextLogo.DateMask = SysAllocString(L"%Y-%m-%d %H:%M:%S");
timestamp.TextLogo.x = 20;
timestamp.TextLogo.y = 1050;  // Bas a gauche
pEffects->add_effect(timestamp);
```

---
## Considérations de performance
### Utilisation CPU par effet
**Impact faible** (< 5 % CPU) :
- Filtres de couleur
- Niveaux de gris
- Inversion
- Retournement / miroir
**Impact moyen** (5-15 % CPU) :
- Superpositions de texte / graphisme
- Contraste / luminosité
- Postérisation
- Désentrelacement simple
**Impact élevé** (15-40 % CPU) :
- Flou (grand rayon)
- Débruitage (CAST, adaptatif)
- Mosaïque (petits blocs)
- Effets marbre / artistiques
### Conseils d'optimisation
1. **Limiter les effets simultanés** — chaque effet ajoute du temps de traitement
2. **Utiliser des paramètres appropriés** — un rayon de flou plus grand consomme plus de CPU
3. **Désactiver les effets inutilisés** — définir `Enabled = FALSE` au lieu de les retirer
4. **Traiter à plus basse résolution** — sous-échantillonner, appliquer les effets, sur-échantillonner
5. **Utiliser le rendu GPU si possible** — recherchez les effets accélérés GPU
---

## Combinaisons d'effets courantes

### Amélioration de webcam
```
1. ef_denoise_adaptive (threshold: 15)
2. ef_contrast (amount: +10)
3. ef_saturation (amount: +15)
4. ef_flip_right (mode miroir)
```

### Rendu film vintage
```
1. ef_greyscale
2. ef_contrast (amount: +20)
3. ef_mono_noise (amount: 15)
```

### Qualité broadcast
```
1. ef_deint_cavt
2. ef_denoise_mosquito (amount: 20)
3. ef_saturation (amount: +5)
```

### Mode confidentialité
```
1. ef_mosaic (size: 20) sur une région spécifique
2. ef_blur (amount: 80) en alternative
```

---
## Voir aussi
- [Présentation du pack de filtres de traitement](index.md)
- [Référence de l'interface des effets vidéo](interfaces/effects-interface.md)
- [Interface d'incrustation par chrominance](interfaces/chroma-key.md)
- [Interface du mélangeur vidéo](interfaces/video-mixer.md)
- [Exemples de code](examples.md)
