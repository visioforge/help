---
title: Contrôle programmatique Pan, Tilt, Zoom de caméra en C# .NET
description: Contrôlez Pan, Tilt, Zoom, exposition, iris et mise au point par programmation avec VisioForge Video Capture SDK. API C# async avec caméras matérielles.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - C#
  - NuGet

---

# Contrôle avancé de la caméra et implémentation PTZ

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Vue d'ensemble des capacités de contrôle de la caméra

L'API Camera Control donne aux développeurs un accès direct pour manipuler divers paramètres de caméra lorsqu'ils travaillent avec des périphériques compatibles. Selon les spécifications matérielles de votre caméra, vous pouvez contrôler par programmation les fonctionnalités suivantes :

- **Pan** — Contrôle de mouvement horizontal
- **Tilt** — Contrôle de mouvement vertical
- **Roll** — Mouvement rotatif le long de l'axe de l'objectif
- **Zoom** — Réglage du niveau de grossissement
- **Exposure** — Paramètres de sensibilité à la lumière
- **Iris** — Contrôle de l'ouverture pour l'admission de lumière
- **Focus** — Réglage de la clarté et de la netteté de l'image

**Note importante :** l'API Camera Control nécessite une session d'aperçu ou de capture de caméra active pour fonctionner correctement. Vous devez démarrer l'aperçu ou la capture avant de tenter d'accéder aux fonctionnalités de contrôle.

## Guide d'implémentation avec exemples

Vous trouverez ci-dessous des modèles d'implémentation pratiques qui démontrent comment intégrer les fonctionnalités de contrôle de caméra dans vos applications .NET.

### Composants d'interface

Pour une interaction utilisateur optimale, envisagez d'implémenter les éléments d'interface utilisateur suivants :

- Curseurs pour l'ajustement des paramètres
- Cases à cocher pour basculer entre les modes auto/manuel
- Étiquettes pour afficher les valeurs actuelles, minimales et maximales

Vous pouvez consulter le code source de la Main Demo pour un exemple d'implémentation complet.

### Étape 1 : lecture des capacités des paramètres de la caméra

Tout d'abord, interrogez la caméra pour déterminer les plages prises en charge et les valeurs par défaut de chaque paramètre de contrôle :

```cs
// Interroger la caméra pour la plage prise en charge du paramètre Zoom.
// GetRangeAsync retourne un objet VideoCaptureDeviceCameraControlRanges
// (ou null si le périphérique ne prend pas en charge cette propriété).
var ranges = await VideoCapture1.Video_CaptureDevice_CameraControl_GetRangeAsync(
    CameraControlProperty.Zoom);
if (ranges != null)
{
    // Configurer le curseur avec la plage prise en charge par la caméra
    tbCCZoom.Minimum = ranges.Min;
    tbCCZoom.Maximum = ranges.Max;
    tbCCZoom.SmallChange = ranges.Step;
    tbCCZoom.Value = ranges.Default;

    // Mettre à jour les étiquettes de l'UI avec les informations de plage
    lbCCZoomMin.Text = "Min: " + ranges.Min;
    lbCCZoomMax.Text = "Max: " + ranges.Max;
    lbCCZoomCurrent.Text = "Current: " + ranges.Default;

    // Définir l'état des cases à cocher du mode de contrôle selon les capacités de la caméra
    cbCCZoomManual.Checked = (ranges.Flags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
    cbCCZoomAuto.Checked = (ranges.Flags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
    cbCCZoomRelative.Checked = (ranges.Flags & CameraControlFlags.Relative) == CameraControlFlags.Relative;
}
```

**Note technique :** lorsque le drapeau Auto est activé, la caméra ignore tous les autres drapeaux et paramètres de valeur manuelle. Cela suit les protocoles standard de l'industrie pour le contrôle de caméra.

### Étape 2 : application des changements de paramètres

Lorsque les utilisateurs ajustent les paramètres via votre interface, appliquez les changements à la caméra avec ce modèle :

```cs
// Initialiser les drapeaux de contrôle
CameraControlFlags flags = CameraControlFlags.None;

// Construire les drapeaux selon l'état des cases à cocher de l'UI
if (cbCCZoomManual.Checked)
{
    // Activer le mode de contrôle manuel
    flags = flags | CameraControlFlags.Manual;
}

if (cbCCZoomAuto.Checked)
{
    // Activer le mode de contrôle automatique (qui remplacera les paramètres manuels)
    flags = flags | CameraControlFlags.Auto;
}

if (cbCCZoomRelative.Checked)
{
    // Activer le mode de valeur relative (les changements sont relatifs à la position actuelle)
    flags = flags | CameraControlFlags.Relative;
}

// Appliquer la nouvelle valeur de zoom avec les drapeaux de contrôle spécifiés.
// SetAsync prend un VideoCaptureDeviceCameraControlValue qui combine valeur + drapeaux.
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Zoom,
    new VideoCaptureDeviceCameraControlValue(tbCCZoom.Value, flags));
```

## Gestion des erreurs et bonnes pratiques

Lors de l'implémentation de fonctionnalités de contrôle de caméra, considérez ces bonnes pratiques :

- Vérifiez toujours qu'un paramètre est pris en charge avant de tenter de le définir
- Implémentez une gestion d'erreurs appropriée pour les fonctionnalités non prises en charge
- Fournissez un retour aux utilisateurs lorsqu'une commande échoue
- N'oubliez pas que les capacités des caméras varient considérablement entre fabricants et modèles

## Dépendances requises  

Pour un bon fonctionnement, assurez-vous que votre application inclut ces paquets redistribuables :

- Redistribuables Video Capture :
  - [Architecture x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Architecture x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Ressources supplémentaires

Pour plus d'exemples et de détails d'implémentation complets, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) contenant de nombreux exemples de code et applications de démonstration.
