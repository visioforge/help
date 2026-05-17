---
title: Effets vidéo personnalisés et shaders OpenGL en C# .NET
description: Effets vidéo temps réel, shaders GLSL, LUT et animations pan/zoom en C# .NET avec VisioForge Media Blocks et l'accélération GPU.
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

# Effets vidéo personnalisés et shaders OpenGL en C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le VisioForge Media Blocks SDK fournit plus de 70 blocs de traitement vidéo pour les effets temps réel, les shaders OpenGL accélérés par GPU, l'étalonnage colorimétrique professionnel par LUT et les transformations pan/zoom animées. Les effets s'appliquent en insérant des blocs de traitement dans le pipeline entre une source vidéo et un moteur de rendu ou un encodeur.

## Effets vidéo intégrés

[MediaBlocksPipeline](#){ .md-button }

Tous les effets intégrés suivent le même schéma de pipeline — insérez le bloc d'effet entre votre source et votre moteur de rendu :

```text
VideoSource → EffectBlock → VideoRenderer
```

### Flou gaussien

Appliquez un flou ou un renforcement avec un sigma configurable. Les valeurs positives floutent, les valeurs négatives renforcent :

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Flou avec sigma 1.2 (plus élevé = plus de flou)
var blur = new GaussianBlurBlock(1.2);

pipeline.Connect(videoSource.Output, blur.Input);
pipeline.Connect(blur.Output, videoRenderer.Input);
```

### Réduction de bruit (filtre Smooth)

Réduction de bruit basée sur OpenCV avec lissage préservant les arêtes :

```csharp
using VisioForge.Core.Types.X.VideoEffects;

var smoothSettings = new SmoothVideoEffect()
{
    FilterSize = 5,       // taille du noyau (plus grand = lissage plus fort)
    Tolerance = 128,      // seuil de contraste (0-255)
    LumaOnly = true       // lisser uniquement la luminance, préserver la couleur
};

var smooth = new SmoothBlock(smoothSettings);

pipeline.Connect(videoSource.Output, smooth.Input);
pipeline.Connect(smooth.Output, videoRenderer.Input);
```

### Balance des couleurs

Ajustez en temps réel la luminosité, le contraste, la saturation et la teinte :

```csharp
var balance = new VideoBalanceBlock(new VideoBalanceVideoEffect
{
    Brightness = 0.1,    // -1.0 à 1.0 (0 = aucun changement)
    Contrast = 1.2,      // 0.0 à l'infini (1 = aucun changement)
    Saturation = 1.5,    // 0.0 à l'infini (1 = aucun changement)
    Hue = 0.0            // -1.0 à 1.0 (0 = aucun changement)
});

pipeline.Connect(videoSource.Output, balance.Input);
pipeline.Connect(balance.Output, videoRenderer.Input);
```

### Préréglages d'effets de couleur

Appliquez des préréglages artistiques (sépia, carte thermique, cross-processing, etc.) :

```csharp
var colorFx = new ColorEffectsBlock(ColorEffectsPreset.Sepia);

pipeline.Connect(videoSource.Output, colorFx.Input);
pipeline.Connect(colorFx.Output, videoRenderer.Input);
```

## Effets accélérés GPU avec OpenGL

[MediaBlocksPipeline](#){ .md-button }

Les effets OpenGL s'exécutent sur le GPU pour des performances significativement meilleures avec la vidéo HD/4K. Ils nécessitent de téléverser les images vidéo en mémoire GPU puis de les récupérer :

```text
VideoSource → GLUploadBlock → GLEffectBlock → GLDownloadBlock → VideoRenderer
```

### Utilisation des effets OpenGL intégrés

Le SDK inclut plus de 25 effets accélérés GPU :

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

### Effets OpenGL disponibles

| Bloc d'effet | Description |
| --- | --- |
| `GLBlurBlock` | Flou par convolution séparable 9x9 |
| `GLColorBalanceBlock` | Luminosité, contraste, teinte, saturation |
| `GLGrayscaleBlock` | Conversion en niveaux de gris |
| `GLSepiaBlock` | Ton sépia |
| `GLSobelBlock` | Détection d'arêtes Sobel |
| `GLLaplacianBlock` | Détection d'arêtes laplacienne |
| `GLFishEyeBlock` | Distorsion fisheye |
| `GLBulgeBlock` | Distorsion bombée |
| `GLTwirlBlock` | Effet tourbillon |
| `GLMirrorBlock` | Réflexion miroir |
| `GLSqueezeBlock` | Distorsion par compression |
| `GLStretchBlock` | Distorsion par étirement |
| `GLHeatBlock` | Visualisation carte thermique |
| `GLGlowLightingBlock` | Effet halo lumineux |
| `GLLightTunnelBlock` | Effet tunnel lumineux |
| `GLSinCityBlock` | Sin City (désaturation sélective) |
| `GLXRayBlock` | Visualisation radiographie |
| `GLLumaCrossProcessingBlock` | Cross-processing de luminance |
| `GLFlipBlock` | Retournement/miroir GPU |
| `GLDeinterlaceBlock` | Désentrelacement GPU |
| `GLTransformationBlock` | Transformations affines |
| `GLAlphaBlock` | Canal alpha / chroma key |
| `GLEquirectangularViewBlock` | Projection équirectangulaire 360 |

## Shaders GLSL personnalisés

[MediaBlocksPipeline](#){ .md-button }

Écrivez des shaders GLSL fragment et vertex personnalisés et appliquez-les à la vidéo en direct en temps réel. Le `GLShaderBlock` exécute votre shader sur le GPU pour chaque image vidéo.

### Architecture du pipeline

```text
SystemVideoSourceBlock → GLUploadBlock → GLShaderBlock → GLDownloadBlock → VideoRendererBlock
```

### Exemple complet

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

// Source vidéo (webcam)
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var videoSource = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(videoDevices[0]));

// Téléverser les images vers la mémoire GPU
var glUpload = new GLUploadBlock();

// Shader GLSL personnalisé — conversion en niveaux de gris
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

// Récupérer les images depuis le GPU
var glDownload = new GLDownloadBlock();

// Moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Construire le pipeline
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, shaderBlock.Input);
pipeline.Connect(shaderBlock.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Permuter les shaders à l'exécution

Vous pouvez changer de shader pendant l'exécution du pipeline :

```csharp
// Shader d'inversion de couleur
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
shaderBlock.Update();  // appliquer les changements sans redémarrer le pipeline
```

## Uniformes de shader pour effets dynamiques

[MediaBlocksPipeline](#){ .md-button }

Transmettez des paramètres aux shaders GLSL à l'exécution à l'aide du dictionnaire `Uniforms`. Cela permet un contrôle dynamique et en temps réel du comportement du shader.

### Flou gaussien en deux passes avec rayon ajustable

```csharp
// Passe de flou horizontal
var horizontalSettings = new GLShaderSettings(vertexShader, horizontalFragmentShader);
horizontalSettings.Uniforms["blur_radius"] = 2.0f;
horizontalSettings.Uniforms["tex_width"] = 1920.0f;
var blurH = new GLShaderBlock(horizontalSettings);

// Passe de flou vertical
var verticalSettings = new GLShaderSettings(vertexShader, verticalFragmentShader);
verticalSettings.Uniforms["blur_radius"] = 2.0f;
verticalSettings.Uniforms["tex_height"] = 1080.0f;
var blurV = new GLShaderBlock(verticalSettings);

// Pipeline : Source → Upload → H-Blur → V-Blur → Download → Renderer
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, blurH.Input);
pipeline.Connect(blurH.Output, blurV.Input);
pipeline.Connect(blurV.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

### Mise à jour des uniformes à l'exécution

```csharp
// Ajuster le rayon de flou en réponse à un changement de curseur
float newRadius = 5.0f;

blurH.Settings.Uniforms["blur_radius"] = newRadius;
blurH.Update();

blurV.Settings.Uniforms["blur_radius"] = newRadius;
blurV.Update();
```

## Étalonnage colorimétrique par LUT

[MediaBlocksPipeline](#){ .md-button }

Appliquez un étalonnage colorimétrique professionnel à l'aide de fichiers de table de correspondance 3D (LUT). Le `LUTProcessorBlock` transforme la couleur de chaque pixel à travers un cube colorimétrique 3D pour des effets de couleur cinématographiques.

### Formats LUT pris en charge

| Format | Extension | Origine |
| --- | --- | --- |
| Iridas/Resolve | `.cube` | DaVinci Resolve, Adobe |
| After Effects | `.3dl` | Adobe After Effects |
| DaVinci | `.dat` | DaVinci Resolve |
| Pandora | `.m3d` | Pandora |
| CineSpace | `.csp` | CineSpace |

### Application LUT de base

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

var lutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cinematic.cube");
var lutProcessor = new LUTProcessorBlock(new LUTVideoEffect(lutPath));

pipeline.Connect(videoSource.Output, lutProcessor.Input);
pipeline.Connect(lutProcessor.Output, videoRenderer.Input);
```

### Comparaison côte à côte

Utilisez un `TeeBlock` pour afficher la vidéo originale et étalonnée côte à côte :

```csharp
using VisioForge.Core.MediaBlocks.Special;

var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);

var lutProcessor = new LUTProcessorBlock(new LUTVideoEffect(lutPath));

// Sortie originale
var rendererOriginal = new VideoRendererBlock(pipeline, null);

// Sortie étalonnée
var rendererGraded = new VideoRendererBlock(pipeline, null);

pipeline.Connect(videoSource.Output, tee.Input);
pipeline.Connect(tee.Outputs[0], lutProcessor.Input);
pipeline.Connect(lutProcessor.Output, rendererGraded.Input);
pipeline.Connect(tee.Outputs[1], rendererOriginal.Input);
```

### Modes d'interpolation

Le `LUTVideoEffect` prend en charge trois modes d'interpolation pour les compromis qualité/performance :

- **Tetrahedral** — qualité maximale, idéal pour la sortie finale
- **Trilinear** — bon équilibre entre qualité et vitesse
- **NearestNeighbor** — le plus rapide, qualité la plus faible

## Animations pan, zoom et Ken Burns

[MediaBlocksPipeline](#){ .md-button }

Le `PanZoomBlock` fournit des transformations pan/zoom statiques et animées — idéal pour les effets Ken Burns, la visualisation de régions d'intérêt et le cadrage dynamique de la vidéo.

### Configuration du pipeline

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

var panZoom = new PanZoomBlock();

pipeline.Connect(fileSource.VideoOutput, panZoom.Input);
pipeline.Connect(panZoom.Output, videoRenderer.Input);
```

### Zoom statique

Zoom vers un point spécifique de l'image :

```csharp
// Zoom 2x centré au milieu de l'image
panZoom.SetZoom(new VideoStreamZoomSettings(
    zoomX: 2.0,     // facteur de zoom horizontal
    zoomY: 2.0,     // facteur de zoom vertical
    centerX: 0.5,   // point central X (0.0 - 1.0)
    centerY: 0.5    // point central Y (0.0 - 1.0)
));
```

### Pan statique

Décaler la région visible :

```csharp
// Panoramique 100 pixels à droite, 50 pixels vers le bas
panZoom.SetPan(new VideoStreamPanSettings(100, 50));
```

### Zoom dynamique (effet Ken Burns)

Animez une transition de zoom fluide sur une plage de temps :

```csharp
// Zoom progressif de 1x à 2x sur 5 secondes depuis la position actuelle
var startTime = await pipeline.Position_GetAsync();
var endTime = startTime + TimeSpan.FromSeconds(5);

panZoom.SetDynamicZoom(new VideoStreamDynamicZoomSettings(
    startZoomX: 1.0, startZoomY: 1.0,   // démarrer à la taille normale
    stopZoomX: 2.0,  stopZoomY: 2.0,     // terminer à 2x
    startTime: startTime,
    stopTime: endTime
));
```

### Pan dynamique

Animez un mouvement de panoramique fluide :

```csharp
panZoom.SetDynamicPan(new VideoStreamDynamicPanSettings(
    startPanX: 0, startPanY: 0,       // position de départ
    stopPanX: 200, stopPanY: 100,     // position d'arrivée
    startTime: startTime,
    stopTime: endTime
));
```

### Rognage rectangulaire et pan

Concentrez-vous sur une région rectangulaire spécifique de la vidéo :

```csharp
// N'afficher que la région commençant à (50, 50) de taille 400x300
panZoom.SetRect(VideoStreamRectSettings.FromPositionAndSize(50, 50, 400, 300));
```

### Réinitialiser pan/zoom

Effacez tous les paramètres pan/zoom pour revenir à la vue d'origine :

```csharp
panZoom.SetZoom(null);
panZoom.SetDynamicZoom(null);
panZoom.SetPan(null);
panZoom.SetDynamicPan(null);
panZoom.SetRect(null);
```

## Enchaîner plusieurs effets

Connectez plusieurs blocs d'effets en série pour construire des chaînes de traitement complexes :

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

Pour les chaînes accélérées GPU, gardez les images en mémoire GPU entre les effets :

```csharp
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, glBlur.Input);
pipeline.Connect(glBlur.Output, glColorBalance.Input);
pipeline.Connect(glColorBalance.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

## Dépannage

### Les effets OpenGL ne fonctionnent pas

**Symptôme :** le pipeline ne démarre pas ou la vidéo apparaît noire avec les effets GL.

**Solutions :**

- Vérifiez que les pilotes GPU sont à jour — les effets OpenGL nécessitent la prise en charge matérielle d'OpenGL
- Assurez-vous que `GLUploadBlock` et `GLDownloadBlock` sont utilisés pour transférer les images vers/depuis la mémoire GPU
- Repassez à des équivalents CPU (par ex. `GaussianBlurBlock` au lieu de `GLBlurBlock`) sur les systèmes sans GPU
- Vérifiez `GLShaderBlock.IsAvailable()` avant de créer des blocs OpenGL

### Le fichier LUT ne charge pas

**Symptôme :** `LUTProcessorBlock` lève une exception ou n'a pas d'effet visible.

**Solutions :**

- Vérifiez que le chemin du fichier LUT est correct et que le fichier existe
- Vérifiez que le format est pris en charge (`.cube`, `.3dl`, `.dat`, `.m3d`, `.csp`)
- Vérifiez `LUTProcessorBlock.IsAvailable()` avant de créer le bloc
- Utilisez un chemin absolu ou `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file.cube")`

### Consommation CPU élevée avec les effets

**Symptôme :** les performances se dégradent lors de l'application de plusieurs effets.

**Solutions :**

- Passez des effets CPU à leurs équivalents OpenGL accélérés GPU
- Réduisez la résolution vidéo avant d'appliquer les effets (utilisez `VideoResizeBlock`)
- Minimisez le nombre d'effets chaînés — combinez les opérations dans un seul shader personnalisé lorsque c'est possible
- Pour le flou en deux passes, utilisez le flou simple en une passe comme alternative plus rapide

## Foire aux questions

### Comment appliquer un shader GLSL personnalisé à de la vidéo en direct en C# ?

Créez un `GLShaderBlock` avec votre code source de shader vertex et fragment, puis insérez-le entre `GLUploadBlock` et `GLDownloadBlock` dans votre pipeline. Le shader vertex standard transmet la position et les coordonnées de texture. Votre shader fragment reçoit l'image vidéo sous forme de `uniform sampler2D tex` et écrit dans `gl_FragColor`. Utilisez `shaderBlock.Update()` pour changer de shader à l'exécution sans redémarrer le pipeline. Consultez la section [Shaders GLSL personnalisés](#shaders-glsl-personnalises) pour un exemple complet.

### Quels formats de fichiers LUT le SDK prend-il en charge pour l'étalonnage colorimétrique ?

Le `LUTProcessorBlock` prend en charge cinq formats LUT 3D standard de l'industrie : Iridas/Resolve `.cube`, After Effects `.3dl`, DaVinci `.dat`, Pandora `.m3d` et CineSpace `.csp`. Le format `.cube` est le plus largement utilisé et est exporté par DaVinci Resolve, Adobe Premiere et la plupart des outils d'étalonnage. Trois modes d'interpolation sont disponibles : Tetrahedral (qualité maximale), Trilinear et NearestNeighbor.

### Puis-je chaîner plusieurs effets vidéo dans un pipeline ?

Oui. Connectez les blocs d'effets en série : `Source → Effet1 → Effet2 → Effet3 → Renderer`. Chaque bloc traite la sortie du précédent. Pour les chaînes accélérées GPU, gardez les images en mémoire OpenGL en connectant directement les blocs d'effets GL sans insérer de `GLDownloadBlock` / `GLUploadBlock` entre eux — téléversez uniquement au début et récupérez à la fin.

### Comment créer une animation pan/zoom Ken Burns ?

Utilisez `PanZoomBlock` avec `SetDynamicZoom()` pour animer une transition de zoom fluide au fil du temps. Passez un `VideoStreamDynamicZoomSettings` avec les facteurs de zoom de début/fin et les horodatages de début/fin. Le bloc interpole automatiquement entre les valeurs. Combinez avec `SetDynamicPan()` pour une animation simultanée de pan et de zoom. Consultez la section [Animations pan, zoom et Ken Burns](#animations-pan-zoom-et-ken-burns) pour des exemples de code.

### Quelles plateformes prennent en charge les effets vidéo OpenGL ?

Les effets OpenGL sont pris en charge sous Windows, Linux, macOS et Android — toute plateforme avec la prise en charge d'OpenGL ES 2.0 ou supérieur. Sous iOS et macOS, le SDK fournit également des équivalents accélérés Metal (`MetalVideoFilterBlock`) pour des performances GPU natives. Les effets basés CPU (`GaussianBlurBlock`, `SmoothBlock`, `ColorEffectsBlock`, etc.) fonctionnent sur toutes les plateformes, y compris iOS.

## Voir aussi

- [Référence des blocs de traitement vidéo](../VideoProcessing/index.md) — liste complète des 70+ blocs de traitement
- [Référence des effets OpenGL](../OpenGL/index.md) — paramètres et types d'effets OpenGL accélérés GPU
- [Vue d'ensemble des effets vidéo](../../general/video-effects/index.md) — catégories d'effets classiques et multiplateformes
- [Référence des effets (100+ effets)](../../general/video-effects/effects-reference.md) — paramètres détaillés de tous les effets
- [Guide de déploiement](../../deployment-x/index.md) — paquets d'exécution spécifiques aux plateformes
- [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — démos de shaders et d'effets
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — page produit et téléchargements
