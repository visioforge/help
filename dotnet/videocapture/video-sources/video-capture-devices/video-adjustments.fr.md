---
title: Luminosité, contraste et saturation de la caméra en C# .NET
description: Contrôlez les paramètres de la caméra (luminosité, contraste, teinte, saturation) en .NET avec des exemples de code Video Capture SDK pour les ajustements.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - C#
primary_api_classes:
  - VideoCaptureDeviceAdjustValue

---

# Maîtriser les contrôles d'image de la caméra dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction aux ajustements matériels de la caméra

Lors du développement d'applications utilisant des webcams ou d'autres périphériques de capture vidéo, disposer d'un contrôle précis sur les paramètres de qualité d'image est essentiel pour créer des logiciels de qualité professionnelle. L'enum `VideoHardwareAdjustment` sélectionne quel ajustement matériel vous lisez ou écrivez, et le SDK expose un contrôle programmatique sur les paramètres de la caméra tels que la luminosité, le contraste, la teinte, la saturation, la netteté, le gamma et plus encore.

## Propriétés d'ajustement matériel prises en charge

L'enum `VideoHardwareAdjustment` couvre :

- Brightness
- Contrast
- Hue
- Saturation
- Sharpness
- Gamma
- ColorEnable
- WhiteBalance
- BacklightCompensation
- Gain
- PowerLineFrequency

L'exposition, la mise au point, le zoom et d'autres contrôles au niveau de l'objectif appartiennent à une famille d'API séparée (`Video_CaptureDevice_CameraControl_*` + l'enum `CameraControlProperty`) — cette page ne couvre que les ajustements *d'image* matériels.

Notez que toutes les caméras ne prennent pas en charge toutes les propriétés. Le SDK ignorera gracieusement toute propriété non prise en charge par le matériel de la caméra que vous utilisez.

## Travailler avec les ajustements de la caméra

### Obtention des plages de valeurs des propriétés

Avant de définir des valeurs d'ajustement, il est important de comprendre la plage valide pour chaque propriété. Utilisez la méthode `Video_CaptureDevice_VideoAdjust_GetRangesAsync` pour récupérer les valeurs minimale, maximale, par défaut et le pas pour toute propriété d'ajustement :

```cs
// Récupérer la plage valide de valeurs pour la propriété de luminosité
// Cela aide à comprendre les limites dans lesquelles vous pouvez ajuster les paramètres
var brightnessRange = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRangesAsync(VideoHardwareAdjustment.Brightness);

// Vérifier les valeurs minimale et maximale autorisées
int minValue = brightnessRange.Min;
int maxValue = brightnessRange.Max;
int defaultValue = brightnessRange.Default;
int step = brightnessRange.Step;
```

### Définition des valeurs d'ajustement

Une fois que vous connaissez la plage valide, vous pouvez définir une valeur spécifique à l'aide de la méthode `Video_CaptureDevice_VideoAdjust_SetValueAsync` :

```cs
// Créer un objet de valeur d'ajustement avec vos paramètres souhaités
// Vous pouvez spécifier une valeur personnalisée et indiquer si l'ajustement automatique doit être utilisé
var adjustmentValue = new VideoCaptureDeviceAdjustValue(75, false); // Valeur : 75, Auto : false

// Appliquer l'ajustement de luminosité à la caméra
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Brightness, adjustmentValue);
```

### Récupération des valeurs actuelles

Pour lire la valeur actuelle de toute propriété d'ajustement, utilisez la méthode `Video_CaptureDevice_VideoAdjust_GetValueAsync` :

```cs
// Obtenir le paramètre de luminosité actuel de la caméra
// Cela retourne à la fois la valeur actuelle et l'état de l'ajustement automatique
var currentBrightness = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetValueAsync(VideoHardwareAdjustment.Brightness);

// Accéder à la valeur actuelle et au drapeau auto
int value = currentBrightness.Value;
bool isAuto = currentBrightness.Auto;
```

## Bonnes pratiques pour les ajustements de la caméra

1. **Vérifiez toujours les plages en premier** : différents modèles de caméra ont des plages valides différentes pour chaque propriété.
2. **Gérez les propriétés non prises en charge** : implémentez toujours une gestion d'erreurs pour les propriétés qui pourraient ne pas être prises en charge.
3. **Pensez à sauvegarder les préférences utilisateur** : stockez les valeurs d'ajustement dans les paramètres de l'application pour une expérience cohérente.
4. **Fournissez des contrôles d'interface** : créez des curseurs avec des valeurs min/max appropriées basées sur les plages retournées.
5. **Considérez automatique vs manuel** : certains utilisateurs peuvent préférer l'ajustement automatique tandis que d'autres ont besoin d'un contrôle manuel précis.

## Ressources supplémentaires

Consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code complets et des exemples d'implémentation.
