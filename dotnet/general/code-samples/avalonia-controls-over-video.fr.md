---
title: Superposer des contrôles Avalonia sur la vidéo en C# .NET
description: Placez des boutons, des panneaux et du contenu défilant Avalonia par-dessus VideoView. Mode de rendu logiciel, limites d'airspace natif et WSLg.
sidebar_label: Contrôles Avalonia sur la vidéo
tags:
  - Media Blocks SDK
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Avalonia
  - VideoRendererBlock
  - VideoView
  - Windows
  - Linux
  - macOS
  - Playback
  - Capture
  - C#
primary_api_classes:
  - VideoView
  - VideoRendererBlock

---

# Superposer des contrôles Avalonia sur la vidéo

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Placer des éléments Avalonia — boutons, superpositions, bandeaux d'état — par-dessus l'aperçu vidéo ne fonctionne que lorsque la vidéo est dessinée *dans* l'arbre visuel d'Avalonia. Le contrôle `VideoView` d'Avalonia y dessine selon l'un de deux modes. Le mode par défaut transmet les images à une fenêtre enfant native hébergée via `NativeControlHost` (une fenêtre enfant HWND sous Windows, une fenêtre X11 via XEmbed sous Linux et macOS), et une fenêtre enfant native est composée après — et au-dessus de — tout ce qu'Avalonia a dessiné : les superpositions placées au-dessus restent invisibles, et elle n'est pas rognée par le contenu d'un `ScrollViewer`. C'est la même limite d'airspace que celle du contrôle WPF, et Avalonia la documente comme [limitation de l'intégration native](https://docs.avaloniaui.net/docs/app-development/native-interop).

| Mode de rendu | Comment l'activer | Contrôles Avalonia par-dessus | Remarques |
|---|---|---|---|
| Intégration de fenêtre native | par défaut (`SetNativeRendering(true)`) | Non | Le chemin le plus rapide. La vidéo est rognee aux limites du contrôle hôte et se dessine toujours au-dessus du contenu Avalonia. |
| Logiciel (`WriteableBitmap`) | `VideoView1.SetNativeRendering(false)` | Oui | Les images sont copiées dans un `WriteableBitmap` dans l'arbre visuel, au prix d'une copie CPU par image. |

Il n'existe pas d'équivalent composé sur GPU du mode `D3D11Composable` de WPF dans le contrôle Avalonia ; le mode logiciel est le mode qui se compose avec l'interface.

## Mode de rendu logiciel

Appelez `SetNativeRendering(false)` sur la vue **avant** le démarrage du pipeline ou du moteur :

```cs
VideoView1.SetNativeRendering(false);
```

Dans Media Blocks SDK .NET, reliez la vue de la manière habituelle — `VideoRendererBlock` lit le mode de la vue au moment de la construction et transmet les images via un buffer sink au lieu d'intégrer un rendereur de fenêtre :

```cs
_pipeline = new MediaBlocksPipeline();

_fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(filename, renderVideo: true, renderAudio: false));

_videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
_pipeline.Connect(_fileSource.VideoOutput, _videoRenderer.Input);

await _pipeline.StartAsync();
```

En mode logiciel, la vidéo est un élément Avalonia ordinaire : les panneaux, boutons et popups se dessinent par-dessus, et elle est rognée correctement dans un `ScrollViewer` ou tout conteneur avec rognage. L'image est mise à l'échelle uniformément et centrée dans la vue, comme en mode natif.

Les moteurs X (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`) honorent le même appel — configurez-le avant d'attacher le moteur / de démarrer la lecture.

## Layout

Placez la vue et la superposition dans la même cellule d'un `Grid`. Les enfants ultérieurs se dessinent au-dessus :

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"
        x:Class="MyPlayer.MainWindow">
    <Panel>
        <vf:VideoView x:Name="VideoView1" />

        <Border Background="#88000000" CornerRadius="8"
                VerticalAlignment="Top" HorizontalAlignment="Left"
                Margin="8" Padding="10,6">
            <TextBlock Text="LIVE" Foreground="White" FontWeight="SemiBold" />
        </Border>
    </Panel>
</Window>
```

## Exécution sous WSL (WSLg)

Sous WSLg, l'intégration de la fenêtre enfant native fait quitter l'application dès l'arrivée de la première image vidéo : Mesa ne peut pas initialiser son chemin GPU (`ZINK: failed to choose pdev`, `failed to create dri2 screen`), et la création de la fenêtre XEmbed qui suit échoue avec lui. C'est une limite du système de fenêtrage de WSLg, pas un défaut du SDK ni de votre application — la même application se rend correctement sur une installation Ubuntu native.

Le mode logiciel ne crée aucune fenêtre enfant native et se rend donc normalement sous WSLg. Si vous développez sous WSL, appelez `SetNativeRendering(false)`, ou conditionnez l'appel à une détection de WSL au démarrage. Voir [Déploiement sur Ubuntu](../../deployment-x/Ubuntu.md) pour les paquets GStreamer dont WSL a besoin.

## Dépannage

**La superposition est invisible, la vidéo se joue.** La vue est en mode d'intégration native. Appelez `SetNativeRendering(false)` avant de démarrer le pipeline — le mode est lu lors de la construction du pipeline, redémarrez donc la lecture pour que le changement prenne effet.

**Une fenêtre vidéo séparée s'ouvre au lieu d'un rendu dans le layout.** Un `VideoView` qui ne fait pas partie d'une fenêtre enracinée ne fournit pas de fenêtre parente au sink vidéo, et le sink ouvre sa propre fenêtre de premier niveau. Ajoutez la vue à l'arbre visuel de la fenêtre (le contrôle ajoute son hôte natif au démarrage).

**L'application se ferme dès que la première image arrive, avec des erreurs Mesa/Zink dans la console.** Intégration native sous WSLg — voir la section ci-dessus.

## Pages associées

- [Superposer des contrôles WPF sur la vidéo](wpf-controls-over-video.md) — le même sujet d'airspace pour WPF, y compris le mode composé sur GPU `D3D11Composable`.
- [Déploiement sur Ubuntu](../../deployment-x/Ubuntu.md) — paquets GStreamer pour Linux et WSL.
