---
title: Plusieurs écrans vidéo dans WPF — multi-affichage C#
description: Créez des applications vidéo multi-écrans en WPF avec des contrôles Image, la gestion d'événements, la gestion mémoire et l'optimisation des performances.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - WPF
  - C#
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - VideoFrameBufferEventArgs
  - MultiscreenVideoView
  - VideoView

---

# Implémenter plusieurs écrans de sortie vidéo dans les applications WPF

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Lors du développement d'applications WPF qui nécessitent de gérer plusieurs flux vidéo simultanément, les développeurs font souvent face à des défis de performance, de synchronisation et de gestion des ressources. Ce guide fournit une approche complète pour implémenter plusieurs écrans de sortie vidéo dans vos applications WPF en C# avec le contrôle Image.

## Prise en main des plusieurs écrans vidéo

Consultez le guide d'installation pour WPF [ici](../../install/index.md).

Pour commencer à implémenter plusieurs sorties vidéo dans votre application WPF, vous devrez :

1. Ajouter le contrôle Video View approprié à votre application
2. Configurer la gestion des événements pour le traitement des images vidéo
3. Configurer votre pipeline de rendu pour des performances optimales

### Deux modèles pris en charge

Pour afficher le même flux vidéo sur plusieurs vues dans WPF, choisissez l'un de ces deux modèles concrets :

1. **`MultiscreenVideoView` + `OnVideoFrameBuffer`** — un moteur SDK pousse chaque image dans autant de contrôles `MultiscreenVideoView` que vous le souhaitez. Utilisez ceci lorsqu'un seul moteur de capture/lecture pilote plusieurs copies à l'écran.
2. **Un moteur par `VideoView`** — chaque affichage obtient sa propre instance de `VideoCaptureCore` / `MediaPlayerCore` / `VideoEditCore` liée à son propre `VideoView` standard. Utilisez ceci lorsque les affichages montrent des sources **différentes** (par exemple une grille de sécurité à quatre caméras). Voir l'exemple [Système de sécurité à quatre caméras](#exemple-pratique-systeme-de-securite-a-quatre-cameras) ci-dessous.

Le `VisioForge.Core.UI.WPF.VideoView` standard n'expose pas de méthode `RenderFrame` — il est piloté automatiquement par le moteur auquel il est lié via `CreateAsync(IVideoView)`. La distribution des images nécessite `MultiscreenVideoView`.

### Configuration de votre projet WPF pour la distribution des images

Déposez un ou plusieurs contrôles `VisioForge.Core.UI.WPF.MultiscreenVideoView` sur votre fenêtre WPF. Donnez-leur des noms descriptifs (par exemple `multiView1`, `multiView2`). Ce sont les contrôles qui acceptent les images poussées.

### Gestion des images vidéo

Abonnez-vous à l'événement `OnVideoFrameBuffer` du moteur SDK. Les arguments d'événement transportent un `VideoFrameBufferEventArgs` que chaque `MultiscreenVideoView` peut rendre.

## Implémenter le gestionnaire d'images vidéo

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    multiView1.RenderFrame(e);
}
```

`MultiscreenVideoView.RenderFrame(VideoFrameBufferEventArgs)` copie l'image dans sa propre surface interne, donc le tampon du moteur peut être libéré lorsque le gestionnaire retourne.

## Techniques d'implémentation avancées

### Créer dynamiquement des MultiscreenVideoView

Pour les applications nécessitant un nombre variable de sorties vidéo, créez des contrôles `MultiscreenVideoView` dynamiquement :

```cs
private List<VisioForge.Core.UI.WPF.MultiscreenVideoView> multiViews = 
    new List<VisioForge.Core.UI.WPF.MultiscreenVideoView>();

private void CreateMultiView(Grid container, int row, int column)
{
    var view = new VisioForge.Core.UI.WPF.MultiscreenVideoView();
    Grid.SetRow(view, row);
    Grid.SetColumn(view, column);
    
    container.Children.Add(view);
    multiViews.Add(view);
}

// Exemple d'utilisation :
// CreateMultiView(mainGrid, 0, 0);
// CreateMultiView(mainGrid, 0, 1);
```

### Distribuer les images vidéo à plusieurs vues

Distribuez les images entrantes à chaque `MultiscreenVideoView` enregistré :

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Rendre vers toutes les instances MultiscreenVideoView
    foreach (var view in multiViews)
    {
        view.RenderFrame(e);
    }
}
```

## Stratégies d'optimisation des performances

### Réduire la charge de rendu

Pour plusieurs vues vidéo, envisagez ces techniques d'optimisation :

1. **Saut d'images** : toutes les vues n'ont pas besoin de se mettre à jour à pleine fréquence
2. **Masquer les vues hors écran** : WPF élimine le rendu des contrôles repliés — utilisez `Visibility.Collapsed` sur les vues non visibles

```cs
private int frameCounter;

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // La vue principale obtient toutes les images
    primaryMultiView.RenderFrame(e);
    
    // Les vues secondaires reçoivent une image sur deux
    if (frameCounter % 2 == 0)
    {
        foreach (var view in secondaryMultiViews)
        {
            view.RenderFrame(e);
        }
    }
    
    frameCounter++;
}
```

## Exemple pratique : système de sécurité à quatre caméras

Voici un exemple plus complet d'implémentation d'un système de sécurité à quatre caméras avec le moteur `VideoCaptureCore` du Video Capture SDK. Chaque caméra obtient sa propre instance de moteur liée à un `VideoView` dédié.

```cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI.WPF;
using VisioForge.Core.VideoCapture;

public partial class SecurityMonitorWindow : Window
{
    private readonly List<VideoView> _cameraViews = new List<VideoView>();
    private readonly List<VideoCaptureCore> _cameras = new List<VideoCaptureCore>();

    public SecurityMonitorWindow()
    {
        InitializeComponent();
    }

    public async Task InitializeCamerasAsync(IEnumerable<string> deviceNames)
    {
        // Énumérer les quatre premiers périphériques de capture si les noms ne sont pas fournis.
        var names = deviceNames?.Take(4).ToList();

        // Construire une grille 2x2 de contrôles VideoView associés à des moteurs VideoCaptureCore.
        int i = 0;
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 2; col++, i++)
            {
                var view = new VideoView();
                Grid.SetRow(view, row);
                Grid.SetColumn(view, col);
                mainGrid.Children.Add(view);
                _cameraViews.Add(view);

                // Le moteur classique utilise un constructeur ; CreateAsync est le modèle du moteur X.
                // Pour VideoCaptureCoreX, utilisez `await VideoCaptureCoreX.CreateAsync(view)` à la place.
                var camera = new VideoCaptureCore(view);
                _cameras.Add(camera);

                if (names != null && i < names.Count)
                {
                    camera.Video_CaptureDevice = new VideoCaptureSource(names[i]);
                    camera.Video_CaptureDevice.Format_UseBest = true;
                    camera.Mode = VideoCaptureMode.VideoPreview;
                    camera.Audio_PlayAudio = false;
                    camera.Audio_RecordAudio = false;
                }
            }
        }
    }

    public async Task StartCamerasAsync()
    {
        foreach (var camera in _cameras)
        {
            await camera.StartAsync();
        }
    }

    public async Task StopCamerasAsync()
    {
        foreach (var camera in _cameras)
        {
            await camera.StopAsync();
        }

        foreach (var camera in _cameras)
        {
            camera.Dispose();
        }
    }
}
```

Énumérez les périphériques disponibles avec `camera.Video_CaptureDevices()` (ou sa variante asynchrone) pour renseigner l'argument `deviceNames` à l'exécution — voir [le guide d'énumération des périphériques](../../videocapture/video-sources/video-capture-devices/enumerate-and-select.md).

## Dépannage des problèmes courants

### Gérer la synchronisation des images

Si vous rencontrez des problèmes de minutage des images sur plusieurs affichages :

```cs
private readonly object syncLock = new object();

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    lock (syncLock)
    {
        foreach (var view in multiViews)
        {
            view.RenderFrame(e);
        }
    }
}
```

---
Pour plus d'exemples de code et de techniques d'implémentation avancées, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
