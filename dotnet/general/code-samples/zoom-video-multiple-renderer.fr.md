---
title: Paramètres de zoom pour plusieurs moteurs de rendu
description: Configurez zoom et position indépendants pour plusieurs moteurs de rendu vidéo sur des affichages multi-écrans en applications .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoRendererType

---

# Configurer les paramètres de zoom pour plusieurs moteurs de rendu vidéo en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Lors du développement d'applications multimédias utilisant plusieurs moteurs de rendu vidéo, contrôler les paramètres de zoom et de position indépendamment pour chaque affichage est essentiel pour créer des interfaces utilisateur de qualité professionnelle. Ce guide couvre les détails d'implémentation, les configurations de paramètres et les bonnes pratiques pour configurer plusieurs moteurs de rendu vidéo avec des paramètres de zoom personnalisés dans vos applications .NET.

## Comprendre les configurations à plusieurs moteurs de rendu

La prise en charge de plusieurs moteurs de rendu (également appelée fonctionnalité multi-écrans) permet à votre application d'afficher du contenu vidéo simultanément sur différentes zones d'affichage. Chaque moteur de rendu peut être configuré avec :

- Son propre rapport de zoom (niveau d'agrandissement)
- Son propre décalage horizontal (positionnement sur l'axe X)
- Son propre décalage vertical (positionnement sur l'axe Y)

Cette capacité est particulièrement précieuse pour des applications telles que :

- Les systèmes de vidéosurveillance affichant plusieurs flux de caméras
- Les logiciels de production multimédia avec des fenêtres d'aperçu et de sortie
- Les applications d'imagerie médicale nécessitant différents niveaux de zoom pour l'analyse
- Les systèmes de bornes multi-écrans avec contenu synchronisé

## Implémenter la méthode MultiScreen_SetZoom

Le SDK fournit la méthode `MultiScreen_SetZoom` qui prend quatre paramètres clés :

1. **Index d'écran** (basé sur zéro) : identifie quel moteur de rendu configurer
2. **Rapport de zoom** : contrôle le pourcentage d'agrandissement
3. **Décalage X** : ajuste le positionnement horizontal (pixels ou pourcentage)
4. **Décalage Y** : ajuste le positionnement vertical (pixels ou pourcentage)

### Signature de méthode et paramètres

```cs
// Signature de la méthode
void MultiScreen_SetZoom(int screenIndex, int zoomRatio, int shiftX, int shiftY);
```

| Paramètre | Description | Plage valide |
|-----------|-------------|--------------|
| screenIndex | Index basé sur zéro du moteur de rendu cible | 0 à (nombre de moteurs de rendu - 1) |
| zoomRatio | Pourcentage d'agrandissement | 1 à 1000 (%) |
| shiftX | Décalage horizontal | -1000 à 1000 |
| shiftY | Décalage vertical | -1000 à 1000 |

## Exemple de code : configurer plusieurs moteurs de rendu

L'exemple suivant montre comment définir différentes valeurs de zoom et de positionnement pour trois moteurs de rendu distincts :

```cs
// Configurer le moteur de rendu principal (index 0)
// 50 % de zoom sans décalage horizontal ni vertical
VideoCapture1.MultiScreen_SetZoom(0, 50, 0, 0);

// Configurer le moteur de rendu secondaire (index 1)
// 20 % de zoom avec un léger décalage horizontal et vertical
VideoCapture1.MultiScreen_SetZoom(1, 20, 10, 20);

// Configurer le moteur de rendu tertiaire (index 2)
// 30 % de zoom sans décalage horizontal mais avec un décalage vertical significatif
VideoCapture1.MultiScreen_SetZoom(2, 30, 0, 30);
```

## Bonnes pratiques pour la gestion de plusieurs moteurs de rendu

Lors de l'implémentation de configurations à plusieurs moteurs de rendu, prenez en compte ces bonnes pratiques :

### 1. Initialiser tous les moteurs de rendu avant de définir le zoom

Assurez-vous toujours que tous les moteurs de rendu sont correctement initialisés avant d'appliquer les paramètres de zoom :

```cs
// Initialiser plusieurs moteurs de rendu
VideoCapture1.MultiScreen_Enabled = true;

// Ajouter 3 moteurs de rendu
VideoCapture1.MultiScreen_AddScreen(videoView1, 1280, 720);
VideoCapture1.MultiScreen_AddScreen(videoView2, 1920, 1080);
VideoCapture1.MultiScreen_AddScreen(videoView3, 1280, 720);

// Il est maintenant sûr de configurer les paramètres de zoom
VideoCapture1.MultiScreen_SetZoom(0, 50, 0, 0);
VideoCapture1.MultiScreen_SetZoom(1, 20, 10, 20);
VideoCapture1.MultiScreen_SetZoom(2, 30, 0, 30);

// Configurations supplémentaires...
```

### 2. Gérer les changements de résolution de manière appropriée

Si le format source change en cours de session, lisez la résolution actuelle depuis le rappel `OnVideoFrameBuffer` (`e.Frame.Info.Width` / `e.Frame.Info.Height`) et réappliquez le zoom. Suivez le nombre d'écrans vous-même — `VideoCaptureCore` n'expose pas de propriété `MultiScreen_Count` ; vous savez combien d'écrans vous avez ajoutés puisque vous avez appelé `MultiScreen_AddScreen` vous-même.

```cs
private int _multiScreenCount = 0; // incrémenté après chaque appel à MultiScreen_AddScreen

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    int newZoom = CalculateOptimalZoom(e.Frame.Info.Width, e.Frame.Info.Height);

    // Appliquer à tous les moteurs de rendu enregistrés
    for (int i = 0; i < _multiScreenCount; i++)
    {
        VideoCapture1.MultiScreen_SetZoom(i, newZoom, 0, 0);
    }
}
```

### 3. Fournir des contrôles utilisateur pour l'ajustement du zoom

Pour les applications interactives, envisagez d'implémenter des contrôles UI qui permettent aux utilisateurs d'ajuster les paramètres de zoom :

```cs
private void zoomTrackBar_ValueChanged(object sender, EventArgs e)
{
    int selectedRenderer = rendererComboBox.SelectedIndex;
    int zoomValue = zoomTrackBar.Value;
    int shiftX = horizontalShiftTrackBar.Value;
    int shiftY = verticalShiftTrackBar.Value;
    
    // Appliquer les nouveaux paramètres de zoom au moteur de rendu sélectionné
    VideoCapture1.MultiScreen_SetZoom(selectedRenderer, zoomValue, shiftX, shiftY);
}
```

## Configurations de zoom avancées

### Transitions de zoom dynamiques

Pour des transitions de zoom fluides, envisagez d'implémenter des changements de zoom graduels :

```cs
async Task AnimateZoomAsync(int screenIndex, int startZoom, int targetZoom, int duration)
{
    int steps = 30; // Nombre d'étapes d'animation
    int delay = duration / steps; // Millisecondes entre les étapes
    
    for (int i = 0; i <= steps; i++)
    {
        // Calculer la valeur de zoom intermédiaire
        int currentZoom = startZoom + ((targetZoom - startZoom) * i / steps);
        
        // Appliquer la valeur de zoom actuelle
        VideoCapture1.MultiScreen_SetZoom(screenIndex, currentZoom, 0, 0);
        
        // Attendre la prochaine étape
        await Task.Delay(delay);
    }
}

// Utilisation
await AnimateZoomAsync(0, 50, 100, 1000); // Animer de 50 % à 100 % sur 1 seconde
```

## Optimiser les performances avec plusieurs moteurs de rendu

Lorsque vous travaillez avec plusieurs moteurs de rendu, soyez attentif aux implications de performance :

1. **Limitez les mises à jour fréquentes** : évitez de changer rapidement les paramètres de zoom car cela peut impacter les performances
2. **Envisagez l'accélération matérielle** : activez l'accélération matérielle lorsqu'elle est disponible
3. **Surveillez l'utilisation de la mémoire** : plusieurs moteurs de rendu haute résolution peuvent consommer beaucoup de mémoire

```cs
// Utiliser le moteur de rendu EVR pour la composition accélérée matériellement sur Windows. Les
// paramètres du moteur de rendu résident dans Video_Renderer (un VideoRendererSettings),
// pas directement sur le core.
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
VideoCapture1.Video_Renderer.Deinterlace_EVR_Mode = VideoRendererEVRDeinterlaceMode.Auto;
```

## Dépannage des problèmes courants

### Problème : les moteurs de rendu affichent un écran noir après les changements de zoom

Cela peut se produire lorsque les valeurs de zoom dépassent les plages valides ou lorsque les moteurs de rendu ne sont pas correctement initialisés :

```cs
// Réinitialiser les paramètres de zoom aux valeurs par défaut pour tous les moteurs de rendu enregistrés
public void ResetZoomSettings()
{
    for (int i = 0; i < _multiScreenCount; i++)
    {
        VideoCapture1.MultiScreen_SetZoom(i, 100, 0, 0); // 100 % de zoom, aucun décalage
    }
}
```

### Problème : image déformée après le zoom

Des valeurs de zoom extrêmes peuvent provoquer une distorsion. Implémentez des limites pour les valeurs de zoom :

```cs
public void SetSafeZoom(int screenIndex, int requestedZoom, int shiftX, int shiftY)
{
    // Limiter les valeurs à des plages sûres
    int safeZoom = Math.Clamp(requestedZoom, 10, 200); // 10 % à 200 %
    int safeShiftX = Math.Clamp(shiftX, -100, 100);
    int safeShiftY = Math.Clamp(shiftY, -100, 100);
    
    VideoCapture1.MultiScreen_SetZoom(screenIndex, safeZoom, safeShiftX, safeShiftY);
}
```

## Conclusion

Plusieurs moteurs de rendu vidéo correctement configurés avec des paramètres de zoom indépendants peuvent considérablement améliorer l'expérience utilisateur dans les applications multimédias. En suivant les recommandations et les bonnes pratiques décrites dans ce document, vous pouvez implémenter des configurations d'affichage vidéo sophistiquées adaptées aux exigences spécifiques de votre application.

Pour des exemples de code supplémentaires et des conseils d'implémentation, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
