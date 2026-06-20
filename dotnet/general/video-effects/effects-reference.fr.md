---
title: SDK d'effets vidéo — référence des filtres et superpositions
description: Guide de référence complet pour tous les effets vidéo des SDK VisioForge .NET, classiques (Windows) et multiplateformes.
sidebar_label: Référence des effets
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
  - Effects
  - Decklink
  - NDI
  - GIF
  - C#
primary_api_classes:
  - VideoEffect
  - VideoEffectGrayscale
  - GPUVideoEffect
  - GrayscaleVideoEffect
  - GaussianBlurVideoEffect

---

# Référence complète des effets vidéo

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Ce document fournit une référence exhaustive de tous les effets vidéo disponibles dans les SDK VisioForge .NET. Les effets sont disponibles dans deux implémentations distinctes :

### Types d'effets par moteur SDK

#### Effets classiques (Windows uniquement)
- **Disponibles dans** : VideoCaptureCore, MediaPlayerCore, VideoEditCore
- **Plateforme** : Windows uniquement (.NET Framework 4.7.2+ et .NET 6-10)
- **Types** : 
  - `VideoEffect*` — traitement basé sur le CPU
  - `GPUVideoEffect*` — accéléré par le GPU (DirectX)
  - `Maxine*` — effets IA NVIDIA (GPU RTX)
- **Emplacement** : espace de noms `VisioForge.Core.Types.VideoEffects`

#### Effets multiplateformes
- **Disponibles dans** : VideoCaptureCoreX, MediaPlayerCoreX, VideoEditCoreX, Media Blocks SDK
- **Plateforme** : Windows, Linux, macOS, Android, iOS
- **Types** :
  - Classes `*VideoEffect` dans `VisioForge.Core.Types.X.VideoEffects`
  - Traitement multiplateforme pour compatibilité universelle
- **Emplacement** : espace de noms `VisioForge.Core.Types.X.VideoEffects`

> **Important** : lors du choix des effets, sélectionnez en fonction de votre SDK cible et de vos exigences de plateforme. Les effets multiplateformes offrent une compatibilité plus large, tandis que les effets Classiques offrent des optimisations spécifiques à Windows et une accélération GPU DirectX.

## Catégories d'effets

Les sections suivantes listent tous les effets disponibles. Chaque effet inclut des marqueurs de disponibilité par SDK :
- 🪟 **Classique** = VideoCaptureCore/MediaPlayerCore/VideoEditCore (Windows uniquement)
- 🌍 **Multiplateforme** = VideoCaptureCoreX/MediaPlayerCoreX/VideoEditCoreX/Media Blocks (Multiplateforme)

### Effets d'ajustement des couleurs

#### Luminosité et obscurité

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectDarkness | 🪟 Classique | Ajuste l'obscurité/luminosité globale. Les valeurs supérieures à 128 assombrissent, celles inférieures éclaircissent l'image. |
| GPUVideoEffectDarkness | 🪟 Classique (GPU) | Ajustement de l'obscurité accéléré par le GPU. |
| GPUVideoEffectBrightness | 🪟 Classique (GPU) | Ajustement de la luminosité accéléré par le GPU. Ajoute ou soustrait des valeurs uniformes aux composantes RGB. |
| VideoEffectLightness | 🪟 Classique | Ajuste la clarté dans l'espace colorimétrique HSL, en préservant les relations de teinte et saturation. |
| VideoBalanceVideoEffect | 🌍 Multiplateforme | Ajustement multiplateforme de la luminosité, du contraste, de la saturation et de la teinte. |

#### Contraste et couleur

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectContrast | 🪟 Classique | Ajuste la différence entre les zones claires et sombres. Prend en charge les transitions animées. |
| GPUVideoEffectContrast | 🪟 Classique (GPU) | Ajustement du contraste accéléré par le GPU. |
| VideoEffectSaturation | 🪟 Classique | Contrôle l'intensité des couleurs des niveaux de gris (0) à la saturation maximale (255). |
| GPUVideoEffectSaturation | 🪟 Classique (GPU) | Ajustement de la saturation accéléré par le GPU. |
| VideoBalanceVideoEffect | 🌍 Multiplateforme | Fournit le contrôle du contraste et de la saturation (multiplateforme). |
| GammaVideoEffect | 🌍 Multiplateforme | Correction gamma pour ajustement de la courbe de luminosité. |

### Filtres de couleur et conversions

#### Niveaux de gris et monochrome

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectGrayscale | 🪟 Classique | Convertit la vidéo couleur en noir et blanc en utilisant des poids de luminance perceptuelle. |
| GPUVideoEffectGrayscale | 🪟 Classique (GPU) | Conversion en niveaux de gris accélérée par le GPU. |
| GPUVideoEffectMonoChrome | 🪟 Classique (GPU) | Crée un effet monochrome avec une couleur de teinte personnalisable. |
| GrayscaleVideoEffect | 🌍 Multiplateforme | Conversion multiplateforme en niveaux de gris utilisant l'élément videobalance. |
| ColorEffectsVideoEffect | 🌍 Multiplateforme | Divers préréglages de couleur dont monochrome, sépia, carte de chaleur, etc. |

#### Inversion de couleur

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectInvert | 🪟 Classique | Inverse toutes les valeurs de couleur RGB, créant un effet de négatif photographique. |
| GPUVideoEffectInvert | 🪟 Classique (GPU) | Inversion de couleur accélérée par le GPU. |

#### Filtres de canal de couleur

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectRed | 🪟 Classique | Isole le canal de couleur rouge, supprimant les composantes vert et bleu. |
| VideoEffectGreen | 🪟 Classique | Isole le canal de couleur vert, supprimant les composantes rouge et bleu. |
| VideoEffectBlue | 🪟 Classique | Isole le canal de couleur bleu, supprimant les composantes rouge et vert. |
| VideoEffectFilterRed | 🪟 Classique | Applique un effet de filtre rouge avec intensité ajustable. |
| VideoEffectFilterGreen | 🪟 Classique | Applique un effet de filtre vert avec intensité ajustable. |
| VideoEffectFilterBlue | 🪟 Classique | Applique un effet de filtre bleu avec intensité ajustable. |

#### Étalonnage colorimétrique

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectLUT | 🪟 Classique | Étalonnage par table de correspondance utilisant des fichiers LUT 3D pour correction colorimétrique professionnelle. |
| LUTVideoEffect | 🌍 Multiplateforme | Prise en charge LUT 3D multiplateforme avec plusieurs modes d'interpolation. |
| VideoEffectPosterize | 🪟 Classique | Réduit le nombre de couleurs, créant un effet d'affiche avec des niveaux de couleur discrets. |
| VideoEffectSolorize | 🪟 Classique | Effet de solarisation qui inverse les couleurs au-dessus d'un seuil de luminosité. |

### Amélioration d'image

#### Netteté et flou

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectSharpen | 🪟 Classique | Accentue les contours et les fins détails pour des images plus nettes. |
| GPUVideoEffectSharpen | 🪟 Classique (GPU) | Accentuation accélérée par le GPU. |
| VideoEffectBlur | 🪟 Classique | Applique un filtre de lissage, adoucissant l'image et réduisant le détail. |
| GPUVideoEffectBlur | 🪟 Classique (GPU) | Flou accéléré par le GPU. |
| GPUVideoEffectDirectionalBlur | 🪟 Classique (GPU) | Flou avec composante directionnelle pour effets de flou de mouvement. |
| VideoEffectSmooth | 🪟 Classique | Filtre d'adoucissement pour réduction du bruit et lissage doux de l'image. |
| GaussianBlurVideoEffect | 🌍 Multiplateforme | Flou gaussien multiplateforme avec sigma ajustable. |
| SmoothVideoEffect | 🌍 Multiplateforme | Filtre de lissage/adoucissement multiplateforme. |

#### Débruitage

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectDenoiseAdaptive | 🪟 Classique | Débruitage adaptatif qui préserve les contours tout en supprimant le bruit. |
| VideoEffectDenoiseCAST | 🪟 Classique | Algorithme de débruitage CAST (Cellular Automata-based Spatio-Temporal) avec plusieurs paramètres. |
| VideoEffectDenoiseMosquito | 🪟 Classique | Réduit les artefacts de bruit moustique courants dans la vidéo compressée. |
| GPUVideoEffectDenoise | 🪟 Classique (GPU) | Débruitage général accéléré par le GPU. |

### Transformations géométriques

#### Rotation et retournement

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectRotate | 🪟 Classique | Fait pivoter la vidéo de n'importe quel angle avec options d'étirement ou de préservation totale de l'image. |
| FlipRotateVideoEffect | 🌍 Multiplateforme | Retournement et rotation multiplateformes (90°, 180°, 270°, retournement horizontal/vertical). |
| VideoEffectFlipHorizontal | 🪟 Classique | Retourne la vidéo horizontalement (miroir gauche-droite). |
| VideoEffectFlipVertical | 🪟 Classique | Retourne la vidéo verticalement (miroir haut-bas). |
| VideoEffectMirrorHorizontal | 🪟 Classique | Crée un effet miroir horizontal au centre de l'image. |
| VideoEffectMirrorVertical | 🪟 Classique | Crée un effet miroir vertical au centre de l'image. |

#### Mise à l'échelle et transformation

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectZoom | 🪟 Classique | Zoome sur une région spécifique de la vidéo avec contrôle de panoramique et d'échelle. |
| VideoEffectPan | 🪟 Classique | Effet de panoramique et rognage pour sélectionner une région spécifique de la vidéo. |
| ResizeVideoEffect | 🌍 Multiplateforme | Mise à l'échelle/redimensionnement vidéo multiplateforme avec plusieurs méthodes d'interpolation. |
| CropVideoEffect | 🌍 Multiplateforme | Rognage vidéo multiplateforme à des dimensions spécifiques. |
| AspectRatioCropVideoEffect | 🌍 Multiplateforme | Rogne automatiquement la vidéo selon un rapport hauteur/largeur cible. |
| BoxVideoEffect | 🌍 Multiplateforme | Ajoute du letterboxing/pillarboxing avec couleur de remplissage personnalisée. |

### Effets artistiques et stylistiques

#### Effets de film classique

| Effet | SDK | Description |
|--------|-----|-------------|
| GPUVideoEffectOldMovie | 🪟 Classique (GPU) | Effet de pellicule ancienne avec grain, rayures et teinte sépia. |
| GPUVideoEffectNightVision | 🪟 Classique (GPU) | Simulation de caméra à vision nocturne avec apparence phosphore vert. |
| AgingVideoEffect | 🌍 Multiplateforme | Effet multiplateforme de vieillissement/pellicule ancienne. |
| OpticalAnimationBWVideoEffect | 🌍 Multiplateforme | Effets d'animation d'illusions d'optique en noir et blanc. |

#### Effets de contour et de texture

| Effet | SDK | Description |
|--------|-----|-------------|
| GPUVideoEffectEmboss | 🪟 Classique (GPU) | Crée un effet de gaufrage ou de relief surélevé mettant en valeur les contours. |
| GPUVideoEffectContour | 🪟 Classique (GPU) | Détection de contours et accentuation des contours. |
| GPUVideoEffectPixelate | 🪟 Classique (GPU) | Effet de pixellisation avec taille de bloc ajustable. |
| VideoEffectMosaic | 🪟 Classique | Crée un motif en mosaïque avec taille de tuile ajustable. |
| EdgeVideoEffect | 🌍 Multiplateforme | Filtre multiplateforme de détection de contours. |
| DiceVideoEffect | 🌍 Multiplateforme | Crée un effet de dés/cubiste en divisant l'image en carrés pivotés. |

#### Effets de distorsion spéciaux (multiplateforme uniquement)

Les effets artistiques suivants sont disponibles exclusivement dans l'implémentation multiplateforme :

| Effet | Description |
|--------|-------------|
| FishEyeVideoEffect | Effet de distorsion œil-de-poisson. |
| GLTwirlVideoEffect | Effet de distorsion tournoiement/tourbillon (OpenGL). |
| GLBulgeVideoEffect | Distorsion de renflement/agrandissement (OpenGL). |
| StretchVideoEffect | Effet de distorsion d'étirement. |
| GLLightTunnelVideoEffect | Effet de perspective de tunnel (OpenGL). |
| SquareVideoEffect | Effet de déformation carrée. |
| CircleVideoEffect | Effet de déformation circulaire. |
| KaleidoscopeVideoEffect | Effet miroir kaléidoscope. |
| MarbleVideoEffect | Effet de texture marbre. |
| Pseudo3DVideoEffect | Effet stéréo pseudo-3D. |
| QuarkVideoEffect | Effet particules quark. |
| RippleVideoEffect | Effet d'ondulation aquatique. |
| WaterRippleVideoEffect | Ondulation aquatique améliorée avec plusieurs modes. |
| WarpVideoEffect | Effet général de déformation. |
| MovingBlurVideoEffect | Flou de mouvement avec contrôle directionnel. |
| MovingEchoVideoEffect | Effet d'écho/traînée de mouvement. |
| MovingZoomEchoVideoEffect | Effet combiné de zoom et d'écho. |
| SMPTEVideoEffect | Effets de transition SMPTE (volets, fondus). |
| SMPTEAlphaVideoEffect | Transitions SMPTE avec canal alpha. |

#### Effets spéciaux

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectColorNoise | 🪟 Classique | Ajoute du bruit de couleur aléatoire pour effets de grain ou d'interférence. |
| VideoEffectMonoNoise | 🪟 Classique | Ajoute du bruit monochrome (niveaux de gris). |
| VideoEffectSpray | 🪟 Classique | Effet bombe de peinture ou pointilliste. |
| VideoEffectShakeDown | 🪟 Classique | Effet de tremblement vertical pour simulation d'impact ou de tremblement de terre. |

### Désentrelacement

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectDeinterlaceBlend | 🪟 Classique | Mélange les champs entrelacés pour une sortie progressive. |
| GPUVideoEffectDeinterlaceBlend | 🪟 Classique (GPU) | Désentrelacement Blend accéléré par le GPU. |
| VideoEffectDeinterlaceCAVT | 🪟 Classique | Désentrelacement Content Adaptive Vertical Temporal. |
| VideoEffectDeinterlaceTriangle | 🪟 Classique | Méthode de désentrelacement par interpolation triangle. |
| DeinterlaceVideoEffect | 🌍 Multiplateforme | Désentrelacement multiplateforme avec plusieurs méthodes (linear, greedy, vfir, yadif, etc.). |
| AutoDeinterlaceSettings | 🌍 Multiplateforme | Désentrelacement automatique lorsqu'un contenu entrelacé est détecté. |
| InterlaceSettings | 🌍 Multiplateforme | Crée une sortie entrelacée à partir d'un contenu progressif. |

### Effets de transition

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectFadeIn | 🪟 Classique | Fondu progressif du noir vers le contenu vidéo. |
| VideoEffectFadeOut | 🪟 Classique | Fondu progressif du contenu vidéo vers le noir. |

### Superpositions et graphiques

#### Superpositions de texte

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectTextLogo | 🪟 Classique | Superposition de texte flexible avec personnalisation poussée — polices, couleurs, rotation, effets et texte animé. |
| VideoEffectScrollingTextLogo | 🪟 Classique | Bannière de texte défilant avec contrôle de direction et de vitesse. |
| TextOverlayVideoEffect | 🌍 Multiplateforme | Superposition de texte multiplateforme avec contrôle typographique avancé. Prend en charge horodatages, heure système et texte dynamique. |
| OverlayManagerText | 🌍 Multiplateforme | Superposition de texte avancée avec ombres, dégradés et animations. |
| OverlayManagerScrollingText | 🌍 Multiplateforme | Texte défilant avec contrôle complet de la vitesse, direction et apparence. |
| OverlayManagerDateTime | 🌍 Multiplateforme | Superposition date/heure avec formatage personnalisable. |

Voir : [Guide de superposition de texte](text-overlay.md)

#### Superpositions d'image

| Effet | SDK | Description |
|--------|-----|-------------|
| VideoEffectImageLogo | 🪟 Classique | Superposition d'image prenant en charge PNG, JPG, BMP, GIF animé, avec contrôle de transparence et de positionnement. |
| ImageOverlayVideoEffect | 🌍 Multiplateforme | Superposition d'image multiplateforme avec positionnement et fusion alpha. |
| ImageOverlayCairoVideoEffect | 🌍 Multiplateforme | Superposition d'image avancée utilisant les graphiques Cairo avec transformations. |
| OverlayManagerImage | 🌍 Multiplateforme | Superposition d'image professionnelle avec animations, transitions et effets. |
| OverlayManagerGIF | 🌍 Multiplateforme | Prise en charge de superposition GIF animé. |
| SVGOverlayVideoEffect | 🌍 Multiplateforme | Superposition graphique vectorielle SVG. |

Voir : [Guide de superposition d'image](image-overlay.md)

#### Fonctionnalités de superposition avancées (multiplateforme uniquement)

| Fonctionnalité | Description |
|---------|-------------|
| OverlayManagerVideo | Superposition vidéo dans vidéo (image dans l'image). |
| OverlayManagerDecklinkVideo | Superposition de source vidéo Blackmagic Decklink. |
| OverlayManagerNDIVideo | Superposition de source vidéo NDI. |
| OverlayManagerGroup | Regrouper plusieurs superpositions pour un contrôle synchronisé. |
| OverlayManagerPan | Animation de panoramique de superposition. |
| OverlayManagerZoom | Animation de zoom de superposition. |
| OverlayManagerFade | Animation de fondu d'entrée/sortie de superposition. |
| OverlayManagerSqueezeback | Effet squeezeback (réduit la vidéo principale pour montrer l'arrière-plan). |
| OverlayManagerBackgroundImage | Couche d'image d'arrière-plan. |
| OverlayManagerBackgroundRectangle | Rectangle coloré d'arrière-plan. |
| OverlayManagerBackgroundSquare | Carré coloré d'arrière-plan. |
| OverlayManagerBackgroundStar | Arrière-plan en forme d'étoile. |
| OverlayManagerBackgroundTriangle | Arrière-plan en forme de triangle. |
| OverlayManagerCircle | Superposition de forme circulaire. |
| OverlayManagerLine | Superposition de dessin de ligne. |
| OverlayManagerRectangle | Superposition de forme rectangulaire. |
| OverlayManagerStar | Superposition de forme étoile. |
| OverlayManagerTriangle | Superposition de forme triangulaire. |

### Incrustation par chrominance (multiplateforme)

| Effet | Description |
|--------|-------------|
| ChromaKeySettingsX | Incrustation écran vert / écran bleu pour suppression d'arrière-plan (à utiliser avec `MediaBlocksPipeline` + bloc filtre chroma-key). |

### Vidéo 360° et VR (Classique uniquement)

| Effet | SDK | Description |
|--------|-----|-------------|
| GPUVideoEffectEquirectangular360 | 🪟 Classique (GPU) | Traite le format vidéo 360° équirectangulaire. |
| GPUVideoEffectEquiangularCubemap360 | 🪟 Classique (GPU) | Convertit entre projections équiangulaires et cubemap pour vidéo 360°. |
| GPUVideoEffectVR360Base | 🪟 Classique (GPU) | Classe de base pour les effets vidéo VR 360°. |

### Effets boostés par IA (NVIDIA Maxine — Classique uniquement)

Nécessite un GPU NVIDIA RTX avec prise en charge du SDK Maxine. Pour la configuration, les exemples de code et les modes d'effet, voir le guide [Amélioration vidéo par IA NVIDIA Maxine](nvidia-maxine.md).

#### Amélioration vidéo

| Effet | Description |
|--------|-------------|
| MaxineDenoiseVideoEffect | Réduction du bruit basée sur l'IA qui préserve intelligemment les détails. |
| MaxineArtifactReductionVideoEffect | Réduit les artefacts de compression et la dégradation vidéo par IA. |
| MaxineSuperResSettings | POCO de paramètres pour la super-résolution IA (à connecter au pipeline d'effets Maxine). |

#### Effets de contenu

| Effet | Description |
|--------|-------------|
| MaxineAIGSVideoEffect | Suppression d'arrière-plan/écran vert basée sur l'IA sans véritable écran vert physique. |
| MaxineUpscaleSettings | POCO de paramètres pour le surdimensionnement IA (à connecter au pipeline d'effets Maxine). |

#### Effets spéciaux

- **VideoEffectColorNoise** — ajoute un bruit de couleur aléatoire pour effets de grain ou d'interférence.
- **VideoEffectMonoNoise** — ajoute un bruit monochrome (niveaux de gris).
- **VideoEffectSpray** — effet bombe de peinture ou pointilliste.
- **VideoEffectShakeDown** — effet de tremblement vertical pour simulation d'impact ou de tremblement de terre.

### Désentrelacement

- **VideoEffectDeinterlaceBlend** / **GPUVideoEffectDeinterlaceBlend** — mélange les champs entrelacés pour une sortie progressive.
- **VideoEffectDeinterlaceCAVT** — désentrelacement Content Adaptive Vertical Temporal.
- **VideoEffectDeinterlaceTriangle** — méthode de désentrelacement par interpolation triangle.

### Effets de transition

#### Effets de fondu

- **VideoEffectFadeIn** — fondu progressif du noir vers le contenu vidéo.
- **VideoEffectFadeOut** — fondu progressif du contenu vidéo vers le noir.

### Superpositions et graphiques

#### Superpositions de texte

- **VideoEffectTextLogo** — superposition de texte flexible avec personnalisation poussée — polices, couleurs, rotation, effets et texte animé.
- **VideoEffectScrollingTextLogo** — bannière de texte défilant avec contrôle de direction et de vitesse.

Voir : [Guide de superposition de texte](text-overlay.md)

#### Superpositions d'image

- **VideoEffectImageLogo** — superposition d'image prenant en charge PNG, JPG, BMP, GIF animé, avec contrôle de transparence et de positionnement.

Voir : [Guide de superposition d'image](image-overlay.md)

### Vidéo 360° et VR

- **GPUVideoEffectEquirectangular360** — traite le format vidéo 360° équirectangulaire.
- **GPUVideoEffectEquiangularCubemap360** — convertit entre projections équiangulaires et cubemap pour vidéo 360°.
- **GPUVideoEffectVR360Base** — classe de base pour les effets vidéo VR 360°.

### Effets boostés par IA (NVIDIA Maxine)

Nécessite un GPU NVIDIA RTX avec prise en charge du SDK Maxine.

#### Amélioration vidéo

- **MaxineDenoiseVideoEffect** — réduction du bruit basée sur l'IA qui préserve intelligemment les détails.
- **MaxineArtifactReductionVideoEffect** — réduit les artefacts de compression et la dégradation vidéo par IA.
- **MaxineSuperResSettings** — POCO de paramètres pour la super-résolution IA.

#### Effets de contenu

- **MaxineAIGSVideoEffect** — suppression d'arrière-plan/écran vert basée sur l'IA sans véritable écran vert physique.
- **MaxineUpscaleSettings** — POCO de paramètres pour le surdimensionnement IA.

## Paramètres des effets

### Paramètres communs

Tous les effets vidéo prennent en charge ces paramètres standard :

- **Enabled** — indique si l'effet est actif (peut être activé/désactivé)
- **Name** — identifiant pour récupérer des instances d'effets spécifiques
- **StartTime** — début de l'effet (TimeSpan.Zero = dès le début)
- **StopTime** — fin de l'effet (TimeSpan.Zero = jusqu'à la fin)

### Plages de valeurs des effets classiques

La plupart des effets d'ajustement Classiques utilisent des plages 0-255 où :
- **128** représente généralement la neutralité/aucun changement
- **0** représente le minimum (souvent le plus sombre/faible)
- **255** représente le maximum (souvent le plus lumineux/fort)

### Transitions animées (effets classiques)

De nombreux effets Classiques prennent en charge le paramètre **ValueStop** pour une animation fluide :
- Définissez **Value** pour la valeur de départ
- Définissez **ValueStop** pour la valeur de fin
- Définissez **StartTime** et **StopTime** pour la durée de l'animation

### Paramètres des effets multiplateformes

Les effets multiplateformes utilisent des propriétés d'éléments multimédias avec des plages variables. Consultez la documentation individuelle des effets pour les détails des paramètres spécifiques.

## Comparaison des SDK

### Effets classiques (VideoCaptureCore/MediaPlayerCore)

- **Plateforme** : Windows uniquement (.NET Framework 4.7.2+ et .NET 6-10)
- **Types de traitement** :
  - Basé sur le CPU : classes `VideoEffect*`
  - Accéléré par le GPU : classes `GPUVideoEffect*` (DirectX)
  - Boosté par IA : classes `Maxine*` (NVIDIA RTX requis)
- **Performances** : optimisé pour Windows, accélération GPU DirectX
- **Compatibilité** : Windows desktop uniquement
- **Espace de noms** : `VisioForge.Core.Types.VideoEffects`

### Effets multiplateformes (VideoCaptureCoreX/Media Blocks)

- **Plateforme** : multiplateforme (Windows, Linux, macOS, Android, iOS)
- **Traitement** : multiplateforme, accélération matérielle via les plugins multimédias
- **Performances** : bonnes sur toutes les plateformes, l'accélération GPU varie selon la plateforme
- **Compatibilité** : universelle — bureau et mobile
- **Espace de noms** : `VisioForge.Core.Types.X.VideoEffects`
- **Fonctionnalités supplémentaires** : 
  - Plus d'effets artistiques (distorsions, déformations)
  - Système de superposition avancé (OverlayManager)
  - Incrustation par chrominance / détection de mouvement
  - Mieux adapté aux plateformes mobiles

## Exemples d'utilisation

### Application d'un effet classique

```csharp
// Effet basé sur le CPU
var effect = new VideoEffectGrayscale(
    enabled: true,
    name: "BWEffect"
);
capture.Video_Effects_Add(effect);

// Effet accéléré par le GPU
var gpuEffect = new GPUVideoEffectBrightness(
    enabled: true,
    value: 180,
    name: "Brighten"
);
capture.Video_Effects_Add(gpuEffect);
```

### Application d'un effet multiplateforme

```csharp
// Niveaux de gris multiplateforme
var grayscale = new GrayscaleVideoEffect("bw_effect");
await videoCapture.Video_Effects_AddOrUpdateAsync(grayscale);

// Superposition de texte multiplateforme
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

### Effet animé (Classique)

```csharp
// Fondu au noir sur 3 secondes
var fade = new VideoEffectDarkness(
    enabled: true,
    value: 128,        // Démarrage normal
    valueStop: 255,    // Fin à l'obscurité maximale
    name: "FadeOut",
    startTime: TimeSpan.FromSeconds(57),
    stopTime: TimeSpan.FromSeconds(60)
);
capture.Video_Effects_Add(fade);
```

### Effet limité dans le temps (les deux)

```csharp
// Classique : appliquer l'effet uniquement sur une plage temporelle spécifique
var flashback = new VideoEffectGrayscale(
    enabled: true,
    name: "Flashback",
    startTime: TimeSpan.FromMinutes(2),
    stopTime: TimeSpan.FromMinutes(2.5)
);
player.Video_Effects_Add(flashback);

// Multiplateforme : effet limité dans le temps
// Ctor : GaussianBlurVideoEffect(double sigma, string name = "gaussian_blur")
var blur = new GaussianBlurVideoEffect(5.0, "blur")
{
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromSeconds(15)
};
await player.Video_Effects_AddOrUpdateAsync(blur);
```

## Considérations de performance

### Effets classiques

1. **Effets GPU** : meilleures performances pour vidéo haute résolution sur Windows
2. **Empilement d'effets** : minimisez les effets simultanés pour de meilleures performances
3. **Effets animés** : les animations fluides ajoutent une charge minimale
4. **Effets IA** : les plus gourmands en ressources, à utiliser sur GPU RTX
5. **Impact de la résolution** : les résolutions plus élevées augmentent significativement les exigences de traitement

### Effets multiplateformes

1. **Accélération matérielle** : varie selon la plateforme et la version du framework multimédia
2. **Multiplateforme** : performances généralement bonnes sur toutes les plateformes prises en charge
3. **Optimisation mobile** : mieux adapté au mobile que les effets Classiques
4. **Système de superposition** : rendu multi-superposition efficace
5. **Disponibilité des plugins** : certains effets nécessitent des plugins multimédias spécifiques

## Disponibilité par plateforme

### Effets classiques

| Plateforme | Effets CPU | Effets GPU | IA Maxine |
|----------|-------------|-------------|-----------|
| Windows Desktop | ✅ Complet | ✅ Complet | ✅ GPU RTX |
| Linux    | ❌ | ❌ | ❌ |
| macOS    | ❌ | ❌ | ❌ |
| Android  | ❌ | ❌ | ❌ |
| iOS      | ❌ | ❌ | ❌ |

### Effets multiplateformes

| Plateforme | Prise en charge | Accélération matérielle |
|----------|---------|---------------------|
| Windows  | ✅ Complet | ✅ Via plugins multimédias |
| Linux    | ✅ Complet | ✅ VA-API, VDPAU, etc. |
| macOS    | ✅ Complet | ✅ VideoToolbox |
| Android  | ✅ Complet | ✅ Codecs matériels |
| iOS      | ✅ Complet | ✅ VideoToolbox |

## Choisir le bon type d'effet

### Utilisez les effets classiques lorsque :
- ✅ Vous ciblez uniquement Windows desktop
- ✅ Vous avez besoin de performances maximales sur Windows
- ✅ Vous utilisez l'accélération GPU DirectX
- ✅ Vous avez besoin des fonctionnalités IA NVIDIA Maxine
- ✅ Vous travaillez avec les moteurs VideoCaptureCore/MediaPlayerCore

### Utilisez les effets multiplateformes lorsque :
- ✅ Vous avez besoin d'une prise en charge multiplateforme
- ✅ Vous ciblez les plateformes mobiles (Android/iOS)
- ✅ Vous ciblez Linux ou macOS
- ✅ Vous utilisez VideoCaptureCoreX/Media Blocks
- ✅ Vous avez besoin de fonctionnalités de superposition avancées
- ✅ Vous avez besoin d'incrustation par chrominance ou de détection de mouvement

## Ressources supplémentaires

- [Guide de superposition de texte](text-overlay.md)
- [Guide de superposition d'image](image-overlay.md)
- [Capteur d'échantillons vidéo](video-sample-grabber.md)
- [Comment ajouter des effets](add.md)

---

Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code complets et des exemples d'implémentation.
