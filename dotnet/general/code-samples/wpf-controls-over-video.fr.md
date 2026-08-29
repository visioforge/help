---
title: Superposer des contrôles WPF sur la vidéo en C# .NET
description: Placez boutons, textes et panneaux WPF au-dessus de VideoView. Moteur composable D3D11, mode WriteableBitmap, airspace et migration de WPF_WinUI_Callback.
sidebar_label: Contrôles WPF sur la vidéo
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
  - WPF
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - MediaPlayerCore
  - VideoCaptureCore
  - Windows
  - Playback
  - Capture
  - C#
primary_api_classes:
  - VideoView
  - VideoRendererMode
  - IVideoSurfaceProvider
  - MediaPlayerCoreX
  - VideoCaptureCore

---

# Superposer des contrôles WPF sur la vidéo

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Placer des éléments WPF — boutons, incrustations, bandeaux d'état, commandes de lecture — au-dessus de l'aperçu vidéo ne fonctionne que si la vidéo est dessinée *à l'intérieur* de l'arbre visuel WPF. Si les images sont envoyées vers une fenêtre enfant native (HWND), WPF ne peut rien peindre par-dessus : c'est la limitation classique de l'**airspace** WPF, et aucune valeur de `Panel.ZIndex` ne la contourne.

Le contrôle WPF `VisioForge.Core.UI.WPF.VideoView` propose trois façons de dessiner une image. Deux d'entre elles se composent avec WPF, la troisième non :

| Mode de rendu | Comment l'activer | Contrôles WPF au-dessus | Remarques |
|---|---|---|---|
| `D3D11Composable` | `VideoView1.UseD3D11ComposableRenderer()` | Oui | Les images restent sur le GPU et sont présentées à WPF sous forme de `D3DImage`. Recommandé. |
| Logiciel (`WriteableBitmap`) | Moteurs X : `VideoView1.SetNativeRendering(false)` ; moteurs classiques : `VideoRendererMode.FrameCallback` sur le moteur | Oui | Transfert CPU à chaque image. Le mode classique `WPF_WinUI_Callback`. |
| HWND natif | par défaut (`VideoView1.SetNativeRendering(true)`) | Non | Le chemin le plus rapide, mais l'airspace bloque toute incrustation. |

!!! tip "Vous venez de `VideoRendererMode.WPF_WinUI_Callback` ?"
    Dans les moteurs classiques, le mode se définissait sur le moteur : `VideoCapture1.Video_Renderer.VideoRenderer`. Les moteurs X multiplateformes (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`) n'ont pas d'objet `Video_Renderer` — le mode de rendu est passé sur le contrôle `VideoView` lui-même, si bien qu'une seule vue fonctionne avec n'importe quel moteur. Voir [Migration depuis WPF_WinUI_Callback](#migration-depuis-wpf_winui_callback) ci-dessous.

## Moteur de rendu composable D3D11 (recommandé)

`D3D11Composable` charge chaque image dans une texture partagée Direct3D 11, l'ouvre sur un périphérique Direct3D 9Ex masqué et l'expose à WPF sous forme de `D3DImage`. La vidéo devient un élément ordinaire de l'arbre visuel : transformations de rendu, opacité, ordre Z et découpe arrondie s'y appliquent, et il n'y a plus de HWND à combattre.

**Prérequis :** Windows Vista ou ultérieur (Direct3D 9Ex) et un GPU de classe Direct3D 10/11.

### Activation avec les moteurs X

Appelez `UseD3D11ComposableRenderer()` avant de démarrer la lecture ou la capture — l'appel est idempotent, vous pouvez donc le répéter sans risque après un redémarrage du moteur :

```cs
// Avant de créer / démarrer le moteur.
VideoView1.UseD3D11ComposableRenderer();

_player = new MediaPlayerCoreX(VideoView1);

var source = await UniversalSourceSettingsV2.CreateAsync(new Uri(filename));
await _player.OpenAsync(source);
await _player.PlayAsync();
```

Le même appel convient à `VideoCaptureCoreX` et `VideoEditCoreX`, ainsi qu'à un `VideoRendererBlock` construit sur la même vue dans Media Blocks SDK .NET.

### Activation avec les moteurs classiques

`VideoCaptureCore`, `MediaPlayerCore` et `VideoEditCore` utilisent exactement le même appel sur la vue :

```cs
VideoView1.UseD3D11ComposableRenderer();
```

Le moteur bascule en interne sur le canal de rappels d'images, dont les images sont chargées dans la texture partagée. Préférez cet appel à la sélection de `VideoRendererMode.D3D11Composable` dans les paramètres `Video_Renderer` du moteur : la vue démarre en mode de rendu natif et, tant qu'elle y reste, elle remplace le choix du moteur par un HWND natif. `UseD3D11ComposableRenderer()` s'en charge pour vous.

### Disposition XAML

Placez la vue et l'incrustation dans la même cellule d'un `Grid`. Les enfants suivants sont dessinés au-dessus :

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
    x:Class="MyPlayer.MainWindow">
    <Grid>
        <WPF:VideoView x:Name="VideoView1" />

        <Border Background="#88000000" CornerRadius="8"
                VerticalAlignment="Top" HorizontalAlignment="Left"
                Margin="8" Padding="10,6">
            <TextBlock Text="LIVE" Foreground="White" FontWeight="SemiBold" />
        </Border>

        <StackPanel Orientation="Horizontal" VerticalAlignment="Bottom"
                    HorizontalAlignment="Center" Margin="0,0,0,16">
            <Button Content="Play" Width="80" Click="btPlay_Click" />
            <Button Content="Pause" Width="80" Margin="8,0,0,0" Click="btPause_Click" />
        </StackPanel>
    </Grid>
</Window>
```

Comme la vidéo est un élément WPF ordinaire dans ce mode, la vue elle-même peut également être transformée :

```cs
VideoView1.RenderTransformOrigin = new Point(0.5, 0.5);
VideoView1.RenderTransform = new RotateTransform(10);
VideoView1.Opacity = 0.7;
```

### Traitement GPU personnalisé sur l'image

En mode `D3D11Composable`, la vue expose la texture partagée : vous pouvez donc exécuter vos propres shaders sur chaque image sans passer par la mémoire CPU :

```cs
var provider = VideoView1.GetSurfaceProvider();
if (provider != null)
{
    provider.FrameReady += (sender, e) =>
    {
        // e.SharedHandle — handle partagé DXGI, ouvrez-le sur votre propre ID3D11Device
        // via OpenSharedResource ; e.Width / e.Height — taille de la texture.
    };
}
```

`FrameReady` est déclenché de façon synchrone sur le thread qui a poussé l'image — un rappel DirectShow ou un thread de streaming GStreamer, pas le dispatcher WPF, et pas nécessairement le même thread d'une image à l'autre. Effectuez le marshalling vers le dispatcher avant de toucher un contrôle WPF, et gardez hors du gestionnaire les ressources liées à un thread.

`GetSurfaceProvider()` renvoie `null` dans tous les autres modes de rendu — un moyen rapide de vérifier que le panneau composable est bien actif.

## Mode de rendu logiciel

Si Direct3D n'est pas utilisable — machine virtuelle sans accélération GPU, session de bureau à distance ou machine cible très ancienne — passez au chemin logiciel. Les images sont dessinées dans un `WriteableBitmap` hébergé par un `Image` WPF à l'intérieur de la vue, qui se compose tout aussi bien avec WPF, au prix d'un transfert CPU par image.

Avec les moteurs X, désactivez le rendu natif sur la vue avant le démarrage du moteur :

```cs
VideoView1.SetNativeRendering(false);
```

Sur les moteurs classiques, le mode appartient au moteur. Définissez-le avant `PlayAsync()` / `StartAsync()` et la vue quitte le mode natif d'elle-même :

```cs
MediaPlayer1.Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback;
```

Sur les versions antérieures à 2026.8.18, la vue restait en mode natif et remplaçait ce réglage par un rendu HWND natif : ajoutez donc `VideoView1.SetNativeRendering(false)` avant la ligne ci-dessus si vous ciblez l'une d'elles. Cet appel supplémentaire reste sans effet néfaste sur les versions actuelles.

Le XAML ci-dessus reste inchangé dans les deux cas. Le rendu logiciel et le panneau composable s'excluent mutuellement : une fois le panneau installé, `SetNativeRendering(true)` est ignoré et consigné sous forme d'avertissement.

## Pourquoi le rendu HWND natif bloque les incrustations

Le chemin natif confie les images à une fenêtre enfant Win32 hébergée dans la mise en page WPF. C'est l'option la plus rapide et le comportement par défaut de la vue, mais le gestionnaire de fenêtres du bureau compose cette fenêtre enfant *après* que WPF a dessiné son propre contenu : tout élément WPF placé au-dessus de la vidéo devient donc tout simplement invisible. La même limitation s'applique au mode classique `VideoRendererMode.WPF_NativeHWND`.

Si vous avez besoin à la fois d'un débit maximal et d'une incrustation, préférez `D3D11Composable` — il garde l'image résidente sur le GPU tout en se composant avec WPF.

## Migration depuis WPF_WinUI_Callback

Rien n'a été supprimé des moteurs classiques : `VideoRendererMode.WPF_WinUI_Callback` existe toujours, et `VideoRendererMode.FrameCallback` est un alias de la même valeur, au nom plus explicite. Il se définit sur le moteur, comme montré dans [Mode de rendu logiciel](#mode-de-rendu-logiciel).

Ce qui change, c'est l'endroit où vit le réglage lorsque vous passez à un moteur X :

| Moteur classique | Équivalent moteur X |
|---|---|
| `Video_Renderer.VideoRenderer = VideoRendererMode.WPF_WinUI_Callback` | `VideoView1.SetNativeRendering(false)` |
| `Video_Renderer.VideoRenderer = VideoRendererMode.D3D11Composable` | `VideoView1.UseD3D11ComposableRenderer()` (les deux familles) |
| `Video_Renderer.VideoRenderer = VideoRendererMode.WPF_NativeHWND` | comportement par défaut, ou `VideoView1.SetNativeRendering(true)` |

Pour un accès image par image aux données de pixels — vision par ordinateur, inférence IA, dessin personnalisé — voir [Effets vidéo personnalisés via les événements d'image](custom-video-effects.md) et [Dessin d'images via OnVideoFrameBuffer](image-onvideoframebuffer.md).

## Dépannage

**Une fenêtre vidéo distincte s'ouvre au lieu d'un rendu dans la mise en page.** La vue utilise le chemin HWND alors que votre code attend le panneau composable. Appelez `UseD3D11ComposableRenderer()` *avant* d'attacher le moteur et vérifiez que `GetSurfaceProvider()` renvoie un résultat non `null` une fois la lecture démarrée.

**L'incrustation est invisible alors que la vidéo est lue.** La vue est en mode de rendu natif. Appelez `UseD3D11ComposableRenderer()` ou, pour le chemin logiciel, `SetNativeRendering(false)` sur un moteur X, ou `Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback` sur un moteur classique. Tous doivent être exécutés avant le démarrage de la lecture, redémarrez donc l'aperçu pour que le changement prenne effet.

**L'aperçu reste noir dans une session de bureau à distance ou de machine virtuelle.** L'accélération Direct3D peut y être indisponible — repliez-vous sur le chemin logiciel décrit ci-dessus.

## Démos

- **[Simple Player Demo D3D11 (moteur X)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo%20D3D11)** — `MediaPlayerCoreX` avec un bandeau WPF au-dessus de la vidéo, plus rotation et opacité de la vue.
- **[Simple Player Demo D3D11 (moteur classique)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Simple%20Player%20Demo%20D3D11)** — le même moteur de rendu piloté par `MediaPlayerCore`, basé sur DirectShow.

## Pages associées

- [Sélection du moteur de rendu vidéo (WinForms)](select-video-renderer-winforms.md) — tous les modes de rendu des moteurs classiques.
- [Sortie vidéo multi-écran en WPF](multiple-screens-wpf.md) — plusieurs surfaces d'aperçu indépendantes dans une même application.
- [Image personnalisée dans VideoView](video-view-set-custom-image.md) — remplacer l'aperçu par une image statique.
