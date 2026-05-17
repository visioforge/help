---
title: Moteur de rendu vidéo en C# .NET — EVR, Direct2D, WPF, MadVR
description: Rendu vidéo en C# .NET sur WinForms, WPF et WinUI 3 — EVR, Direct2D, madVR, HWND natif, callback. Configuration et accélération matérielle.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - C#
primary_api_classes:
  - VideoRenderer
  - VideoRendererMode
  - VideoView
  - VideoRendererStretchMode
  - VideoCaptureCoreX

---

# Options de moteur de rendu vidéo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Les moteurs classiques (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) exposent **10 modes de moteur de rendu vidéo** via l'enum `VideoRendererMode`. Choisir le bon mode contrôle comment les images atteignent l'écran : filtres DirectShow bruts, surfaces GPU Direct2D, HWND natif intégré dans WPF, rappels d'image pour un rendu personnalisé, contrôles WinUI 3, ou le moteur de rendu tiers madVR. Ce guide parcourt chaque mode avec le code minimal d'activation, la disponibilité par plateforme et un guide de décision en haut pour passer directement au mode dont votre application a besoin.

!!! note "Moteurs classiques uniquement"
    Cette page couvre les moteurs classiques basés sur DirectShow. Les moteurs multiplateformes `VideoCaptureCoreX` / `MediaPlayerCoreX` utilisent un contrôle `VideoView` avec des puits GStreamer et n'exposent pas d'enum `VideoRendererMode` — le rendu y est géré automatiquement par la liaison du contrôle d'UI.

## Choix rapide — quel moteur de rendu pour quelle application ?

| Mode | Framework UI | Idéal pour |
|---|---|---|
| `VideoRenderer` (GDI hérité) | WinForms | Compatibilité maximale sur matériel très ancien |
| `VMR9` | WinForms | Windows XP / Vista, logiciel + accélération matérielle légère |
| `EVR` | WinForms | Choix par défaut sur Windows moderne (Vista+) |
| `Direct2D` | WinForms, WPF | 2D accélérée par GPU, contenu 4K+, applis modernes |
| `Direct2DManaged` | WPF | Direct2D managé avec pause à la minimisation pour WPF |
| `WPF_NativeHWND` | WPF | HWND natif intégré dans WPF pour des performances supérieures à WPF pur |
| `WPF_WinUI_Callback` (`FrameCallback`) | WPF, WinUI, personnalisé | Rappels par image pour CV, IA, rendu personnalisé |
| `WinUI` | WinUI 3 | Applis WinUI 3 natives (Windows 10/11) |
| `MadVR` | WinForms | Mise à l'échelle et colorimétrie de référence, nécessite une installation externe de madVR |
| `None` | n'importe | Sans interface / audio uniquement / conversion de fichiers sans aperçu |

## Comprendre les options de moteur de rendu vidéo disponibles

Les sections détaillées ci-dessous décrivent chaque mode, en commençant par les trois moteurs de rendu DirectShow classiques.

### Moteur de rendu vidéo hérité (basé sur GDI)

Le Video Renderer est l'option la plus ancienne de l'écosystème DirectShow. Il s'appuie sur GDI (Graphics Device Interface) pour les opérations de dessin.

**Caractéristiques clés :**

- Rendu logiciel sans accélération matérielle
- Compatible avec les systèmes et configurations anciens
- Plafond de performances inférieur aux alternatives modernes
- Implémentation simple avec des options de configuration minimales

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VideoRenderer;
```

**Quand l'utiliser :**

- La compatibilité est la préoccupation principale
- L'application cible du matériel ou des systèmes d'exploitation anciens
- Exigences minimales de traitement vidéo
- Dépannage de problèmes avec des moteurs de rendu plus récents

### Video Mixing Renderer 9 (VMR9)

VMR9 représente une amélioration significative par rapport au moteur de rendu hérité, introduisant la prise en charge de l'accélération matérielle et des fonctionnalités avancées.

**Caractéristiques clés :**

- Rendu accéléré matériellement via DirectX 9
- Prise en charge du mélange de plusieurs flux vidéo
- Options avancées de désentrelacement
- Capacités de mélange alpha et de composition
- Traitement d'effets vidéo personnalisés

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VMR9;
```

**Quand l'utiliser :**

- Applications modernes nécessitant de bonnes performances
- Fonctionnalités d'édition ou de composition vidéo nécessaires
- Scénarios à plusieurs flux vidéo
- Applications qui doivent équilibrer performances et compatibilité

### Enhanced Video Renderer (EVR)

EVR est l'option la plus avancée, disponible dans Windows Vista et les systèmes d'exploitation ultérieurs. Il s'appuie sur le framework Media Foundation plutôt que sur DirectShow pur.

**Caractéristiques clés :**

- Dernières technologies d'accélération matérielle
- Qualité et performances vidéo supérieures
- Traitement amélioré de l'espace colorimétrique
- Meilleure prise en charge multi-écrans
- Utilisation plus efficace du CPU
- Mécanismes de synchronisation améliorés

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
```

**Quand l'utiliser :**

- Applications modernes ciblant Windows Vista ou ultérieur
- Performances et qualité maximales requises
- Applications gérant du contenu HD ou 4K
- Lorsque la synchronisation avancée est importante
- Environnements à plusieurs écrans

### Moteur de rendu Direct2D

Direct2D fournit un rendu 2D haute performance avec accélération GPU. Il est disponible sur les hôtes WinForms et WPF, et constitue le choix moderne recommandé lorsque vous avez besoin d'un rendu accéléré matériellement avec de simples contrôles de rotation, retournement et étirement.

**Caractéristiques clés :**

- Accélération matérielle via Direct2D / Direct3D 11
- Fonctionne à la fois sur WinForms et WPF
- Prend en charge la rotation (0 / 90 / 180 / 270), le retournement horizontal et vertical
- Intégration propre du mode d'étirement (`Stretch` / `Letterbox`)
- Faible surcoût CPU, s'adapte bien au contenu 4K / 8K

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.Direct2D;
VideoCapture1.Video_Renderer.RotationAngle = 0;
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
VideoCapture1.Video_Renderer.Flip_Horizontal = false;
VideoCapture1.Video_Renderer.Flip_Vertical = false;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Quand l'utiliser :**

- Applications modernes WinForms ou WPF voulant un rendu accéléré par GPU
- Sources 4K / 8K où les chemins CPU créeraient un goulot d'étranglement
- Applications nécessitant des contrôles de rotation ou de retournement à l'exécution

### Moteur de rendu Direct2DManaged (WPF)

Variante managée spécifique à WPF de Direct2D. S'intègre plus proprement au modèle d'objet WPF et met automatiquement en pause le rendu lorsque la fenêtre est minimisée — utile pour les applications de lecture longue durée où vous ne voulez pas que le GPU travaille sur des fenêtres masquées.

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.Direct2DManaged;
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

Les options de rotation, retournement et étirement sont partagées avec le mode `Direct2D` standard. La pause à la minimisation est gérée automatiquement par le contrôle WPF `VideoView`.

**Quand l'utiliser :**

- Applications WPF où vous voulez les performances Direct2D avec un cycle de vie WPF-friendly
- Tableaux de bord multi-fenêtres où les fenêtres inactives ne doivent pas consommer de cycles GPU

### Moteur de rendu HWND natif WPF

Héberge un HWND Win32 natif à l'intérieur du contrôle WPF `VideoView`. Vous donne les performances brutes du moteur de rendu DirectShow dans une mise en page WPF, au prix des particularités standard de la chaîne de rendu WPF (problèmes d'airspace avec les contrôles superposés).

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.WPF_NativeHWND;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Quand l'utiliser :**

- Applications WPF nécessitant des performances de rendu DirectShow maximales
- Applications intégrant des filtres hérités qui s'attendent à une cible HWND
- Vous n'avez pas besoin de superposer des contrôles WPF sur la surface vidéo

### Moteur de rendu FrameCallback (WPF_WinUI_Callback)

Mode de rendu basé sur les rappels. Au lieu de dessiner les images directement, le moteur livre chaque image à votre code via des événements, vous laissant rendre avec n'importe quelle bibliothèque (SkiaSharp, `System.Drawing`, OpenGL/DirectX personnalisé, WriteableBitmap) ou alimenter un pipeline non visuel (vision par ordinateur, inférence IA, diffusion vers un point d'extrémité distant).

`FrameCallback` est un alias de `WPF_WinUI_Callback` — le même mode avec un nom plus auto-descriptif.

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback;
await VideoCapture1.Video_Renderer_UpdateAsync();

// S'abonner aux événements d'image
VideoCapture1.OnVideoFrameBitmap += (sender, e) =>
{
    // e.Frame est un System.Drawing.Bitmap — rendre vers un PictureBox, WriteableBitmap, etc.
};

VideoCapture1.OnVideoFrameBuffer += (sender, e) =>
{
    // e.Frame.Data est un IntPtr — envelopper avec SkiaSharp / Marshal.Copy pour un travail au niveau pixel
};
```

Voir [Dessin d'image via OnVideoFrameBuffer](image-onvideoframebuffer.md) et [Dessin de texte via OnVideoFrameBuffer](text-onvideoframebuffer.md) pour des exemples détaillés de traitement par image.

**Quand l'utiliser :**

- Pipelines de vision par ordinateur / ML consommant des images brutes
- Rendu personnalisé avec SkiaSharp, DirectX ou OpenGL
- Applications WPF / WinUI / MAUI qui rendent dans un `WriteableBitmap` manuellement
- Applications sans aucune surface d'aperçu (images envoyées à un serveur, un encodeur, etc.)

### Moteur de rendu WinUI 3

Rendu natif pour les applications WinUI 3 sur Windows 10/11. Utilisez ce mode lorsque votre coque est `Microsoft.UI.Xaml` et que vous hébergez un contrôle `VisioForge.Core.UI.WinUI.VideoView`.

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.WinUI;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Quand l'utiliser :**

- Applications WinUI 3 (Windows App SDK, pas l'ancien WinUI 2 / UWP)
- Vous voulez une cohérence d'apparence native avec d'autres contenus WinUI

### Moteur de rendu madVR (tiers)

[madVR](https://www.madvr.com/) est un moteur de rendu vidéo externe de qualité de référence, populaire auprès des PC home-cinéma et des logiciels vidéo haut de gamme. Il offre des algorithmes de mise à l'échelle, une gestion des couleurs et un désentrelacement supérieurs, au prix d'une charge GPU plus élevée. Pris en charge uniquement sur les hôtes WinForms ; nécessite une installation madVR séparée sur la machine cible (le filtre DirectShow enregistré par CLSID doit être présent).

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.MadVR;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Exigence à l'exécution :** assurez-vous que madVR est installé sur le système cible. Si le filtre est manquant, `Video_Renderer_UpdateAsync` échouera — utilisez le modèle de repli illustré dans [Problèmes de compatibilité du moteur de rendu](#problemes-de-compatibilite-du-moteur-de-rendu) ci-dessous pour dégrader gracieusement vers EVR.

**Quand l'utiliser :**

- Qualité vidéo de référence pour le mastering, le HTPC ou les UI de serveurs multimédias
- Audiences avec des GPU pouvant absorber le coût de rendu supplémentaire
- Vous pouvez livrer / documenter une étape d'installation séparée de madVR

### None (sans interface)

Désactive complètement le rendu. Le graphe de capture / édition / lecture continue de tourner — les images circulent vers les encodeurs, les sorties fichier, les points d'extrémité de streaming ou les rappels — mais aucune surface d'aperçu n'est allouée.

**Exemple d'implémentation :**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.None;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Quand l'utiliser :**

- Capture audio uniquement (microphone vers fichier) lorsque le SDK comprend à la fois des branches audio et vidéo
- Conversion / transcodage de fichier sans fenêtre d'aperçu
- Pipelines de rendu côté serveur
- Tests unitaires et exécutions CI sans interface

## Options de configuration avancées

Au-delà de la simple sélection d'un moteur de rendu, le SDK fournit diverses options de configuration pour ajuster finement la présentation vidéo.

### Travailler avec les modes de désentrelacement

Lors de l'affichage de contenu vidéo entrelacé (courant dans les sources de diffusion), un désentrelacement correct améliore considérablement la qualité visuelle. Le SDK prend en charge divers algorithmes de désentrelacement selon le moteur de rendu choisi.

Tout d'abord, récupérez les modes de désentrelacement disponibles. `Video_Renderer_Deinterlace_Modes()` retourne les noms de modes VMR-9 découverts automatiquement depuis le pilote actuel :

```cs
// Remplir un menu déroulant avec les modes VMR-9 disponibles
foreach (string deinterlaceMode in VideoCapture1.Video_Renderer_Deinterlace_Modes())
{
  cbDeinterlaceModes.Items.Add(deinterlaceMode);
}
```

Le désentrelacement se configure séparément sur les deux moteurs de rendu. VMR-9 prend une chaîne de nom de mode ; EVR prend une valeur d'enum `VideoRendererEVRDeinterlaceMode` :

```cs
// VMR-9 — définir la chaîne de mode sélectionnée par l'utilisateur
VideoCapture1.Video_Renderer.Deinterlace_VMR9_Mode = cbDeinterlaceModes.SelectedItem.ToString();
VideoCapture1.Video_Renderer.Deinterlace_VMR9_UseDefault = false;

// EVR — utiliser l'enum à la place
// VideoCapture1.Video_Renderer.Deinterlace_EVR_Mode = VideoRendererEVRDeinterlaceMode.Auto;

VideoCapture1.Video_Renderer_Update();
```

VMR9 et EVR prennent en charge divers algorithmes de désentrelacement, notamment :

- Bob (doublement de ligne simple)
- Weave (entrelacement de champs)
- Adaptatif au mouvement
- Compensé en mouvement (qualité la plus élevée)

La disponibilité d'algorithmes spécifiques dépend des capacités de la carte vidéo et de l'implémentation du pilote.

### Gérer le rapport d'aspect et les modes d'étirement

Lorsque vous affichez la vidéo dans une fenêtre ou un contrôle qui ne correspond pas au rapport d'aspect natif de la source, vous devez décider comment gérer cet écart. Le SDK fournit plusieurs modes d'étirement pour traiter différents scénarios.

#### Mode Stretch (étirement)

Ce mode étire la vidéo pour remplir toute la zone d'affichage, ce qui peut déformer l'image :

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Stretch;
VideoCapture1.Video_Renderer_Update();
```

**Cas d'usage :**

- Lorsque le rapport d'aspect n'est pas critique
- Remplir toute la zone d'affichage est plus important que les proportions
- La source et l'affichage ont des rapports d'aspect similaires
- Les contraintes de l'interface utilisateur requièrent une utilisation complète de la zone

#### Mode Letterbox

Ce mode préserve le rapport d'aspect d'origine en ajoutant des bordures noires au besoin :

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
VideoCapture1.Video_Renderer_Update();
```

**Cas d'usage :**

- Maintenir des proportions correctes est essentiel
- Applications vidéo professionnelles
- Contenu où la distorsion serait perceptible ou problématique
- Visionnage de contenu cinéma ou de diffusion

#### Mode LetterboxToFill

Ce mode remplit la zone d'affichage tout en préservant le rapport d'aspect, en rognant tout débordement sur un axe :

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.LetterboxToFill;
VideoCapture1.Video_Renderer_Update();
```

**Cas d'usage :**

- Applications vidéo grand public où remplir l'écran est préféré
- Contenu où les bords sont moins importants que le centre
- Affichage vidéo de style réseaux sociaux
- Lorsqu'on tente d'éliminer le letterboxing dans un contenu déjà en letterbox

### Forçage du rapport d'aspect

Pour forcer un rapport d'aspect d'affichage spécifique (par exemple, afficher du contenu 4:3 en letterbox à l'intérieur d'un conteneur 16:9), activez le forçage et définissez les composantes du rapport :

```cs
VideoCapture1.Video_Renderer.Aspect_Ratio_Override = true;
VideoCapture1.Video_Renderer.Aspect_Ratio_X = 4;
VideoCapture1.Video_Renderer.Aspect_Ratio_Y = 3;
VideoCapture1.Video_Renderer_Update();
```

### Zoom et panoramique

`VideoRendererSettings` expose des propriétés de zoom/décalage utiles pour le PTZ numérique sur un aperçu :

```cs
VideoCapture1.Video_Renderer.Zoom_Ratio  = 150; // 150 %
VideoCapture1.Video_Renderer.Zoom_ShiftX = 0;
VideoCapture1.Video_Renderer.Zoom_ShiftY = 0;
VideoCapture1.Video_Renderer_Update();
```

### Retournement et rotation

```cs
VideoCapture1.Video_Renderer.Flip_Horizontal = true;
VideoCapture1.Video_Renderer.Flip_Vertical   = false;
// RotationAngle n'est respecté que par le moteur de rendu Direct2D et accepte 0, 90, 180 ou 270.
VideoCapture1.Video_Renderer.RotationAngle   = 90;
VideoCapture1.Video_Renderer_Update();
```

## Dépannage des problèmes courants

### Problèmes de compatibilité du moteur de rendu

Si vous rencontrez des problèmes avec un moteur de rendu spécifique, essayez de retomber sur une option plus compatible :

```cs
try
{
    // Essayer EVR en premier
    VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
    VideoCapture1.Video_Renderer_Update();
}
catch
{
    try 
    {
        // Retomber sur VMR9
        VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VMR9;
        VideoCapture1.Video_Renderer_Update();
    }
    catch
    {
        // Dernier recours — moteur de rendu hérité
        VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VideoRenderer;
        VideoCapture1.Video_Renderer_Update();
    }
}
```

## Bonnes pratiques et recommandations

1. **Choisissez le bon moteur de rendu pour votre environnement cible** :
   - Pour Windows moderne : EVR
   - Pour une large compatibilité : VMR9
   - Pour les systèmes hérités : Video Renderer

2. **Testez sur diverses configurations matérielles** : le rendu vidéo peut se comporter différemment selon les fournisseurs de GPU et les versions de pilotes.

3. **Implémentez une logique de repli du moteur de rendu** : ayez toujours un plan de secours si le moteur de rendu préféré échoue.

4. **Tenez compte de votre contenu vidéo** : le contenu de haute résolution ou entrelacé bénéficiera davantage de moteurs de rendu avancés.

5. **Équilibrez qualité et performance** : les paramètres de qualité les plus élevés ne fournissent pas toujours la meilleure expérience utilisateur s'ils impactent les performances.

## Dépendances requises

Pour assurer le bon fonctionnement de ces moteurs de rendu, veillez à inclure :

- Paquets redistribuables du SDK
- DirectX End-User Runtime (dernière version recommandée)
- Runtime .NET Framework adapté à votre application

## Conclusion

Les moteurs classiques offrent 10 modes de moteur de rendu couvrant WinForms, WPF et WinUI 3. **EVR** est la valeur par défaut sûre pour WinForms, **Direct2D** pour le rendu moderne accéléré par GPU sur WinForms ou WPF, **FrameCallback** pour les pipelines personnalisés (CV / IA / rendu sur mesure), **WinUI** pour les coques WinUI 3, et **madVR** pour les scénarios de qualité de référence pouvant accueillir l'installation externe. `None` est le mode approprié lorsqu'il n'y a aucun aperçu.

Pour les applications construites sur les moteurs multiplateformes `VideoCaptureCoreX` / `MediaPlayerCoreX`, le choix du moteur de rendu est géré par le contrôle `VideoView` et n'utilise pas cet enum.

## Documentation associée

- [Dessin d'image via OnVideoFrameBuffer](image-onvideoframebuffer.md) — traitement d'image au niveau pixel, cas d'usage canonique de `FrameCallback`.
- [Dessin de texte via OnVideoFrameBuffer](text-onvideoframebuffer.md) — superpositions de texte avec `FrameCallback`.
- [Rendre la vidéo dans un PictureBox](draw-video-picturebox.md) — modèle de rendu WinForms qui s'associe bien à `FrameCallback`.

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.
