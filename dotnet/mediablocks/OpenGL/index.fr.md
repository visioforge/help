---
title: Effets et shaders vidéo OpenGL accélérés GPU en C# .NET
description: Appliquez des effets vidéo OpenGL accélérés GPU (flou, correction colorimétrique, shaders) en temps réel via les pipelines du VisioForge Media Blocks SDK.
sidebar_label: Effets OpenGL
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - GLBaseVideoEffect
  - GLShaderSettings
  - GLVideoMixerSettings
  - GLVirtualVideoSourceBlock

---

# Effets vidéo OpenGL - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Les effets vidéo OpenGL dans VisioForge Media Blocks SDK .Net permettent une manipulation puissante et accélérée matériellement des flux vidéo. Chaque effet est livré comme MediaBlock autonome (par ex. `GLBlurBlock`, `GLFlipBlock`, `GLAlphaBlock`) et se chaîne entre `GLUploadBlock` côté entrée et `GLDownloadBlock` côté sortie. Ce guide couvre les effets disponibles, leurs paramètres de configuration et les autres types OpenGL associés.

## Effet de base : `GLBaseVideoEffect`

Tous les effets vidéo OpenGL héritent de la classe `GLBaseVideoEffect`, qui fournit des propriétés et des événements communs.

| Propriété | Type                  | Description                                      |
|----------|-----------------------|--------------------------------------------------|
| `Name`   | `string`              | Nom interne de l'effet (lecture seule).     |
| `ID`     | `GLVideoEffectID`     | Identifiant unique de l'effet (lecture seule). |
| `Index`  | `int`                 | Index de l'effet dans une chaîne.              |

**Événements :**

- `OnUpdate` : se produit lorsque les propriétés de l'effet doivent être mises à jour dans le pipeline. Appelez `OnUpdateCall()` pour le déclencher.

## Effets vidéo disponibles

Cette section détaille les divers effets vidéo OpenGL utilisables. Chaque effet est un MediaBlock indépendant — instanciez directement le bloc (par ex. `new GLBlurBlock(new GLBlurVideoEffect())`) et connectez-le entre `GLUploadBlock` et `GLDownloadBlock` (ou entre d'autres blocs GL) dans votre pipeline.

### Effet Alpha (`GLAlphaVideoEffect`)

Remplace une couleur sélectionnée par un canal alpha ou définit/ajuste le canal alpha existant.

**Propriétés :**

| Propriété           | Type                     | Valeur par défaut    | Description                                            |
|--------------------|--------------------------|------------------|--------------------------------------------------------|
| `Alpha`            | `double`                 | `1.0`            | Valeur du canal alpha.                       |
| `Angle`            | `float`                  | `20`             | Taille du cube colorimétrique à modifier (rayon de sensibilité pour la correspondance des couleurs). |
| `BlackSensitivity` | `uint`                   | `100`            | Sensibilité aux couleurs sombres.                        |
| `Mode`             | `GLAlphaVideoEffectMode` | `Set`            | Méthode utilisée pour la modification de l'alpha.                |
| `NoiseLevel`       | `float`                  | `2`              | Rayon de bruit (pixels à ignorer autour de la couleur correspondante). |
| `CustomColor`      | `SKColor`                | `SKColors.Green` | Valeur de couleur personnalisée pour le mode chroma key `Custom`.       |
| `WhiteSensitivity` | `uint`                   | `100`            | Sensibilité aux couleurs claires.                      |

**Énumération associée : `GLAlphaVideoEffectMode`**

Définit le mode de fonctionnement de l'effet vidéo Alpha.

| Valeur    | Description                            |
|----------|----------------------------------------|
| `Set`    | Définit/ajuste directement le canal alpha via la propriété `Alpha`. |
| `Green`  | Chroma key sur vert pur.              |
| `Blue`   | Chroma key sur bleu pur.               |
| `Custom` | Chroma key sur la couleur spécifiée par `CustomColor`. |

### Effet Blur (`GLBlurVideoEffect`)

Applique un effet de flou via une convolution séparable 9x9. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Bulge (`GLBulgeVideoEffect`)

Crée une distorsion en relief (bombement) sur la vidéo. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Color Balance (`GLColorBalanceVideoEffect`)

Ajuste la balance des couleurs de la vidéo, dont la luminosité, le contraste, la teinte et la saturation.

**Propriétés :**

| Propriété     | Type     | Valeur par défaut | Description                                      |
|--------------|----------|---------------|--------------------------------------------------|
| `Brightness` | `double` | `0`           | Ajuste la luminosité (-1.0 à 1.0, 0 ne change rien). |
| `Contrast`   | `double` | `1`           | Ajuste le contraste (0.0 à l'infini, 1 ne change rien). |
| `Hue`        | `double` | `0`           | Ajuste la teinte (-1.0 à 1.0, 0 ne change rien).    |
| `Saturation` | `double` | `1`           | Ajuste la saturation (0.0 à l'infini, 1 ne change rien). |

### Effet Deinterlace (`GLDeinterlaceVideoEffect`)

Applique un filtre de désentrelacement à la vidéo.

**Propriétés :**

| Propriété | Type                  | Valeur par défaut   | Description                         |
|----------|-----------------------|-----------------|-------------------------------------|
| `Method` | `GLDeinterlaceMethod` | `VerticalBlur`  | Méthode de désentrelacement à utiliser.    |

**Énumération associée : `GLDeinterlaceMethod`**

Définit la méthode de l'effet vidéo Deinterlace.

| Valeur          | Description                             |
|----------------|-----------------------------------------|
| `VerticalBlur` | Méthode de flou vertical.                   |
| `MAAD`         | Motion Adaptive: Advanced Detection.    |

### Effet Fish Eye (`GLFishEyeVideoEffect`)

Applique un effet de distorsion d'objectif fisheye. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Flip (`GLFlipVideoEffect`)

Retourne ou fait pivoter la vidéo.

**Propriétés :**

| Propriété | Type                | Valeur par défaut | Description                         |
|----------|---------------------|---------------|-------------------------------------|
| `Method` | `GLFlipVideoMethod` | `None`        | Méthode de retournement ou rotation à utiliser. |

**Énumération associée : `GLFlipVideoMethod`**

Définit la méthode de retournement ou de rotation vidéo.

| Valeur                | Description                                  |
|----------------------|----------------------------------------------|
| `None`               | Aucune rotation.                                 |
| `Clockwise`          | Rotation horaire de 90 degrés.                 |
| `Rotate180`          | Rotation de 180 degrés.                          |
| `CounterClockwise`   | Rotation antihoraire de 90 degrés.         |
| `HorizontalFlip`     | Retournement horizontal.                           |
| `VerticalFlip`       | Retournement vertical.                             |
| `UpperLeftDiagonal`  | Retournement selon la diagonale haut-gauche/bas-droite. |
| `UpperRightDiagonal` | Retournement selon la diagonale haut-droite/bas-gauche. |

### Effet Glow Lighting (`GLGlowLightingVideoEffect`)

Ajoute un effet de halo lumineux à la vidéo. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Grayscale (`GLGrayscaleVideoEffect`)

Convertit la vidéo en niveaux de gris. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Heat (`GLHeatVideoEffect`)

Applique un effet de type signature thermique à la vidéo. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Laplacian (`GLLaplacianVideoEffect`)

Applique un filtre de détection d'arêtes laplacien.

**Propriétés :**

| Propriété | Type    | Valeur par défaut | Description                                                       |
|----------|---------|---------------|-------------------------------------------------------------------|
| `Invert` | `bool`  | `false`       | Si `true`, inverse les couleurs pour obtenir des arêtes sombres sur fond clair. |

### Effet Light Tunnel (`GLLightTunnelVideoEffect`)

Crée un effet visuel de tunnel lumineux. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Luma Cross Processing (`GLLumaCrossProcessingVideoEffect`)

Applique un effet de cross-processing de luminance (souvent « xpro »). Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Mirror (`GLMirrorVideoEffect`)

Applique un effet miroir à la vidéo. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Resize (`GLResizeVideoEffect`)

Redimensionne la vidéo aux dimensions spécifiées.

**Propriétés :**

| Propriété | Type  | Description                            |
|----------|-------|----------------------------------------|
| `Width`  | `int` | Largeur cible du redimensionnement vidéo. |
| `Height` | `int` | Hauteur cible du redimensionnement vidéo.|

### Effet Sepia (`GLSepiaVideoEffect`)

Applique un effet de sépia à la vidéo. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Sin City (`GLSinCityVideoEffect`)

Applique un effet de style cinéma « Sin City » (niveaux de gris avec rehauts rouges). Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Sobel (`GLSobelVideoEffect`)

Applique un filtre de détection d'arêtes de Sobel.

**Propriétés :**

| Propriété | Type    | Valeur par défaut | Description                                                       |
|----------|---------|---------------|-------------------------------------------------------------------|
| `Invert` | `bool`  | `false`       | Si `true`, inverse les couleurs pour obtenir des arêtes sombres sur fond clair. |

### Effet Square (`GLSquareVideoEffect`)

Applique un effet de distorsion ou de pixellisation « carré ». Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Squeeze (`GLSqueezeVideoEffect`)

Applique un effet de distorsion par compression. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Stretch (`GLStretchVideoEffect`)

Applique un effet de distorsion par étirement. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet Transformation (`GLTransformationVideoEffect`)

Applique des transformations 3D à la vidéo : rotation, mise à l'échelle et translation.

**Propriétés :**

| Propriété       | Type    | Valeur par défaut | Description                                                           |
|----------------|---------|---------------|-----------------------------------------------------------------------|
| `FOV`          | `float` | `90.0f`       | Angle du champ de vision en degrés pour la projection perspective.            |
| `Ortho`        | `bool`  | `false`       | Si `true`, utilise une projection orthographique ; sinon, perspective.        |
| `PivotX`       | `float` | `0.0f`        | Coordonnée X du point pivot de rotation (0 = centre).               |
| `PivotY`       | `float` | `0.0f`        | Coordonnée Y du point pivot de rotation (0 = centre).               |
| `PivotZ`       | `float` | `0.0f`        | Coordonnée Z du point pivot de rotation (0 = centre).               |
| `RotationX`    | `float` | `0.0f`        | Rotation autour de l'axe X en degrés.                                |
| `RotationY`    | `float` | `0.0f`        | Rotation autour de l'axe Y en degrés.                                |
| `RotationZ`    | `float` | `0.0f`        | Rotation autour de l'axe Z en degrés.                                |
| `ScaleX`       | `float` | `1.0f`        | Multiplicateur d'échelle sur l'axe X.                                      |
| `ScaleY`       | `float` | `1.0f`        | Multiplicateur d'échelle sur l'axe Y.                                      |
| `TranslationX` | `float` | `0.0f`        | Translation sur l'axe X (coordonnées universelles [0-1]).          |
| `TranslationY` | `float` | `0.0f`        | Translation sur l'axe Y (coordonnées universelles [0-1]).          |
| `TranslationZ` | `float` | `0.0f`        | Translation sur l'axe Z (coordonnées universelles [0-1], profondeur).   |

### Effet Twirl (`GLTwirlVideoEffect`)

Applique un effet de distorsion par tourbillon. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

### Effet X-Ray (`GLXRayVideoEffect`)

Applique un effet visuel de type radiographie. Cet effet ne possède pas de propriétés configurables au-delà de celles héritées de `GLBaseVideoEffect`.

## Identification des effets OpenGL : énumération `GLVideoEffectID`

Cette énumération liste tous les types d'effets vidéo OpenGL disponibles, utilisée par `GLBaseVideoEffect.ID`.

| Valeur            | Description                               |
|------------------|-------------------------------------------|
| `ColorBalance`   | Effet de balance des couleurs.                 |
| `Grayscale`      | Effet niveaux de gris.                     |
| `Resize`         | Effet de redimensionnement.                        |
| `Deinterlace`    | Effet de désentrelacement.                   |
| `Flip`           | Effet de retournement.                          |
| `Blur`           | Effet de flou avec convolution séparable 9x9. |
| `FishEye`        | Effet fisheye.                      |
| `GlowLighting`   | Effet de halo lumineux.                 |
| `Heat`           | Effet de signature thermique.                |
| `LumaX`          | Effet de cross-processing de luminance.         |
| `Mirror`         | Effet miroir.                        |
| `Sepia`          | Effet sépia.                         |
| `Square`         | Effet « square ».                        |
| `XRay`           | Effet radiographie.                         |
| `Stretch`        | Effet d'étirement.                       |
| `LightTunnel`    | Effet tunnel lumineux.                  |
| `Twirl`          | Effet tourbillon.                       |
| `Squeeze`        | Effet de compression.                      |
| `SinCity`        | Effet « Sin City » gris-rouge.       |
| `Bulge`          | Effet bombement.                         |
| `Sobel`          | Effet Sobel.                          |
| `Laplacian`      | Effet laplacien.                      |
| `Alpha`          | Effet de canaux alpha.                |
| `Transformation` | Effet de transformation.                |

## Configuration du rendu et de la vue OpenGL

Ces types aident à configurer la manière dont la vidéo est rendue ou visualisée dans un contexte OpenGL, en particulier pour les scénarios spécialisés comme la VR ou les configurations d'affichage personnalisées.

### Paramètres de vue équirectangulaire (`GLEquirectangularViewSettings`)

Gère les paramètres pour le rendu vidéo équirectangulaire (360 degrés), couramment utilisé dans les applications VR. Implémente `IVRVideoControl`.

**Propriétés :**

| Propriété        | Type         | Par défaut           | Description                                    |
|-----------------|--------------|-------------------|------------------------------------------------|
| `VideoWidth`    | `int`        | (lecture seule)        | Largeur de la vidéo source.                     |
| `VideoHeight`   | `int`        | (lecture seule)        | Hauteur de la vidéo source.                    |
| `FieldOfView`   | `float`      | `80.0f`           | Champ de vision en degrés.                      |
| `Yaw`           | `float`      | `0.0f`            | Yaw (rotation autour de l'axe Y) en degrés.       |
| `Pitch`         | `float`      | `0.0f`            | Pitch (rotation autour de l'axe X) en degrés.     |
| `Roll`          | `float`      | `0.0f`            | Roll (rotation autour de l'axe Z) en degrés.      |
| `Mode`          | `VRMode`     | `Equirectangular` | Mode VR (prend en charge `Equirectangular`).    |

**Méthodes :**

- `IsModeSupported(VRMode mode)` : vérifie si le `VRMode` spécifié est pris en charge.

**Événements :**

- `SettingsChanged` : se produit lorsqu'un paramètre de vue est modifié.

### Paramètres du moteur de rendu vidéo (`GLVideoRendererSettings`)

Configure les propriétés générales d'un moteur de rendu vidéo OpenGL.

**Propriétés :**

| Propriété           | Type                          | Par défaut     | Description                                                          |
|--------------------|-------------------------------|-------------|----------------------------------------------------------------------|
| `ForceAspectRatio` | `bool`                        | `true`      | Indique si la mise à l'échelle respecte le rapport d'aspect d'origine.                |
| `IgnoreAlpha`      | `bool`                        | `true`      | Indique si le canal alpha est ignoré (traité comme du noir).              |
| `PixelAspectRatio` | `System.Tuple<int, int>`      | `(0, 1)`    | Rapport d'aspect des pixels du périphérique d'affichage (numérateur, dénominateur).     |
| `Rotation`         | `GLVideoRendererRotateMethod` | `None`      | Rotation appliquée à la vidéo.                         |

**Énumération associée : `GLVideoRendererRotateMethod`**

Définit les méthodes de rotation pour le moteur de rendu vidéo OpenGL.

| Valeur            | Description                             |
|------------------|-----------------------------------------|
| `None`           | Aucune rotation.                            |
| `_90C`           | Rotation horaire de 90 degrés.            |
| `_180`           | Rotation de 180 degrés.                     |
| `_90CC`          | Rotation antihoraire de 90 degrés.    |
| `FlipHorizontal` | Retournement horizontal.                      |
| `FlipVertical`   | Retournement vertical.                        |

## Shaders OpenGL personnalisés

Permet l'application de shaders GLSL personnalisés au flux vidéo.

### Définition de shader (`GLShader`)

Représente une paire de shaders vertex et fragment.

**Propriétés :**

| Propriété         | Type     | Description                                   |
|------------------|----------|-----------------------------------------------|
| `VertexShader`   | `string` | Code source GLSL du shader de vertex.   |
| `FragmentShader` | `string` | Code source GLSL du shader de fragment. |

**Constructeurs :**
- `GLShader()`
- `GLShader(string vertexShader, string fragmentShader)`

### Paramètres de shader (`GLShaderSettings`)

Configure des shaders GLSL personnalisés à utiliser dans le pipeline.

**Propriétés :**

| Propriété   | Type                                 | Description                                      |
|------------|--------------------------------------|--------------------------------------------------|
| `Vertex`   | `string`                             | Code source GLSL du shader de vertex.      |
| `Fragment` | `string`                             | Code source GLSL du shader de fragment.    |
| `Uniforms` | `System.Collections.Generic.Dictionary<string, object>` | Dictionnaire des variables uniformes (paramètres) à transmettre aux shaders. |

**Constructeurs :**
- `GLShaderSettings()`
- `GLShaderSettings(string vertex, string fragment)`
- `GLShaderSettings(GLShader shader)`

## Superpositions d'images en OpenGL

Fournit des paramètres pour superposer des images statiques sur un flux vidéo dans un contexte OpenGL.

### Paramètres de superposition (`GLOverlaySettings`)

Définit les propriétés d'une superposition d'image.

**Propriétés :**

| Propriété   | Type     | Par défaut | Description                                       |
|------------|----------|---------|---------------------------------------------------|
| `Filename` | `string` | (s.o.)   | Chemin du fichier image (lecture seule après init).    |
| `Data`     | `byte[]` | (s.o.)   | Données image sous forme de tableau d'octets (lecture seule après init).|
| `X`        | `int`    |         | Coordonnée X du coin supérieur gauche de la superposition.    |
| `Y`        | `int`    |         | Coordonnée Y du coin supérieur gauche de la superposition.    |
| `Width`    | `int`    |         | Largeur de la superposition.                             |
| `Height`   | `int`    |         | Hauteur de la superposition.                            |
| `Alpha`    | `double` | `1.0`   | Opacité de la superposition (0.0 transparent à 1.0 opaque). |

**Constructeur :**
- `GLOverlaySettings(string filename)`

## Mixage vidéo OpenGL

Ces types servent à configurer un mélangeur vidéo OpenGL, permettant de combiner et composer plusieurs flux vidéo.

### Paramètres du mélangeur (`GLVideoMixerSettings`)

Étend `VideoMixerBaseSettings` pour le mixage vidéo spécifique à OpenGL. Gère une liste d'objets `GLVideoMixerStream` et hérite de propriétés telles que `Width`, `Height` et `FrameRate`.

**Méthodes :**
- `AddStream(GLVideoMixerStream stream)` : ajoute un flux au mélangeur.
- `RemoveStream(GLVideoMixerStream stream)` : retire un flux du mélangeur.
- `SetStream(int index, GLVideoMixerStream stream)` : remplace un flux à un index spécifique.

**Constructeurs :**
- `GLVideoMixerSettings(int width, int height, VideoFrameRate frameRate)`
- `GLVideoMixerSettings(int width, int height, VideoFrameRate frameRate, List<VideoMixerStream> streams)`

### Flux du mélangeur (`GLVideoMixerStream`)

Étend `VideoMixerStream` et définit les propriétés d'un flux individuel au sein du mélangeur vidéo OpenGL. Hérite de `Rectangle`, `ZOrder` et `Alpha` de `VideoMixerStream`.

**Propriétés :**

| Propriété                        | Type                          | Par défaut                      | Description                                      |
|---------------------------------|-------------------------------|------------------------------|--------------------------------------------------|
| `Crop`                          | `Rect`                        | (s.o.)                        | Rectangle de rognage du flux d'entrée.             |
| `BlendConstantColorAlpha`       | `double`                      | `0`                          | Composante alpha pour la couleur de mélange constante.        |
| `BlendConstantColorBlue`        | `double`                      | `0`                          | Composante bleue pour la couleur de mélange constante.         |
| `BlendConstantColorGreen`       | `double`                      | `0`                          | Composante verte pour la couleur de mélange constante.        |
| `BlendConstantColorRed`         | `double`                      | `0`                          | Composante rouge pour la couleur de mélange constante.          |
| `BlendEquationAlpha`            | `GLVideoMixerBlendEquation`   | `Add`                        | Équation de mélange pour le canal alpha.            |
| `BlendEquationRGB`              | `GLVideoMixerBlendEquation`   | `Add`                        | Équation de mélange pour les canaux RVB.                 |
| `BlendFunctionDestinationAlpha` | `GLVideoMixerBlendFunction`   | `OneMinusSourceAlpha`        | Fonction de mélange pour l'alpha de destination.            |
| `BlendFunctionDesctinationRGB`  | `GLVideoMixerBlendFunction`   | `OneMinusSourceAlpha`        | Fonction de mélange pour le RVB de destination.              |
| `BlendFunctionSourceAlpha`      | `GLVideoMixerBlendFunction`   | `One`                        | Fonction de mélange pour l'alpha source.                 |
| `BlendFunctionSourceRGB`        | `GLVideoMixerBlendFunction`   | `SourceAlpha`                | Fonction de mélange pour le RVB source.                   |

**Constructeur :**
- `GLVideoMixerStream(Rect rectangle, uint zorder, double alpha = 1.0)`

### Équation de mélange (énumération `GLVideoMixerBlendEquation`)

Spécifie la manière dont les couleurs source et destination sont combinées lors du mélange.

| Valeur             | Description                                     |
|-------------------|-------------------------------------------------|
| `Add`             | Source + Destination                            |
| `Subtract`        | Source - Destination                            |
| `ReverseSubtract` | Destination - Source                            |

### Fonction de mélange (énumération `GLVideoMixerBlendFunction`)

Définit les facteurs pour les couleurs source et destination dans les opérations de mélange. (Rs, Gs, Bs, As sont les composantes de couleur source ; Rd, Gd, Bd, Ad sont celles de destination ; Rc, Gc, Bc, Ac sont les composantes de couleur constante).

| Valeur                      | Description                                 |
|----------------------------|---------------------------------------------|
| `Zero`                     | Le facteur vaut (0, 0, 0, 0).                     |
| `One`                      | Le facteur vaut (1, 1, 1, 1).                     |
| `SourceColor`              | Le facteur vaut (Rs, Gs, Bs, As).                 |
| `OneMinusSourceColor`      | Le facteur vaut (1-Rs, 1-Gs, 1-Bs, 1-As).         |
| `DestinationColor`         | Le facteur vaut (Rd, Gd, Bd, Ad).                 |
| `OneMinusDestinationColor` | Le facteur vaut (1-Rd, 1-Gd, 1-Bd, 1-Ad).         |
| `SourceAlpha`              | Le facteur vaut (As, As, As, As).                 |
| `OneMinusSourceAlpha`      | Le facteur vaut (1-As, 1-As, 1-As, 1-As).         |
| `DestinationAlpha`         | Le facteur vaut (Ad, Ad, Ad, Ad).                 |
| `OneMinusDestinationAlpha` | Le facteur vaut (1-Ad, 1-Ad, 1-Ad, 1-Ad).         |
| `ConstantColor`            | Le facteur vaut (Rc, Gc, Bc, Ac).                 |
| `OneMinusContantColor`     | Le facteur vaut (1-Rc, 1-Gc, 1-Bc, 1-Ac).         |
| `ConstantAlpha`            | Le facteur vaut (Ac, Ac, Ac, Ac).                 |
| `OneMinusContantAlpha`     | Le facteur vaut (1-Ac, 1-Ac, 1-Ac, 1-Ac).         |
| `SourceAlphaSaturate`      | Le facteur vaut (min(As, 1-Ad), min(As, 1-Ad), min(As, 1-Ad), 1). |

## Sources de test virtuelles pour OpenGL

Ces classes de paramètres servent à configurer des sources virtuelles générant des motifs de test directement dans un contexte OpenGL.

### Paramètres de source vidéo virtuelle (`GLVirtualVideoSourceSettings`)

Configure un bloc source (`GLVirtualVideoSourceBlock`) produisant des données vidéo de test. Implémente `IMediaPlayerBaseSourceSettings` et `IVideoCaptureBaseVideoSourceSettings`.

**Propriétés :**

| Propriété    | Type                       | Par défaut                | Description                                      |
|-------------|----------------------------|------------------------|--------------------------------------------------|
| `Width`     | `int`                      | `1280`                 | Largeur de la vidéo de sortie.                       |
| `Height`    | `int`                      | `720`                  | Hauteur de la vidéo de sortie.                      |
| `FrameRate` | `VideoFrameRate`           | `30/1` (30 i/s)        | Fréquence d'images de la vidéo de sortie.                  |
| `IsLive`    | `bool`                     | `true`                 | Indique si la source est en direct.                 |
| `Mode`      | `GLVirtualVideoSourceMode` | (s.o. - doit être défini)    | Type de motif de test à générer.  |

**Énumération associée : `GLVirtualVideoSourceMode`**

Définit le motif de test généré par `GLVirtualVideoSourceBlock`.

| Valeur         | Description                  |
|---------------|------------------------------|
| `SMPTE`       | Mires de couleurs SMPTE 100 %.       |
| `Snow`        | Aléatoire (neige télévisuelle).    |
| `Black`       | Noir à 100 %.                  |
| `White`       | Blanc à 100 %.                  |
| `Red`         | Rouge uni.             |
| `Green`       | Vert uni.           |
| `Blue`        | Bleu uni.            |
| `Checkers1`   | Damier (1 px).  |
| `Checkers2`   | Damier (2 px).  |
| `Checkers4`   | Damier (4 px).  |
| `Checkers8`   | Damier (8 px).  |
| `Circular`    | Motif circulaire.            |
| `Blink`       | Motif clignotant.            |
| `Mandelbrot`  | Fractale de Mandelbrot.          |

**Méthodes :**
- `Task<MediaFileInfo> ReadInfoAsync()` : lit asynchrone les informations multimédias (renvoie des informations synthétiques d'après les paramètres).
- `MediaBlock CreateBlock()` : crée une instance `GLVirtualVideoSourceBlock` configurée avec ces paramètres.
