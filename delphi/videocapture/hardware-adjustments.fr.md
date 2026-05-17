---
title: Contrôle de luminosité, contraste et saturation en Delphi
description: Ajustez luminosité, contraste et saturation de caméra en Delphi avec les contrôles matériels TVFVideoCapture et des exemples de configuration de paramètres.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture

---

# Implémentation des réglages matériels vidéo dans les applications Delphi

## Vue d'ensemble

Les périphériques modernes de capture vidéo offrent de puissants réglages au niveau matériel qui peuvent améliorer considérablement la qualité de vos applications vidéo. En tirant parti de ces capacités dans vos applications Delphi, vous pouvez proposer aux utilisateurs des fonctionnalités de contrôle vidéo de qualité professionnelle sans recourir à un traitement d'image logiciel complexe.

## Types de réglages pris en charge

La plupart des webcams et périphériques de capture vidéo prennent en charge plusieurs paramètres de réglage :

- Luminosité
- Contraste
- Saturation
- Teinte
- Netteté
- Gamma
- Balance des blancs
- Gain

## Récupération des plages de réglage disponibles

Avant de définir des réglages, vous devez déterminer quelles plages sont prises en charge par le périphérique connecté. La méthode `Video_CaptureDevice_VideoAdjust_GetRanges` fournit cette information.

### Implémentation en Delphi

```pascal
// Récupérer la plage disponible pour le réglage de la luminosité
// Renvoie minimum, maximum, taille du pas, valeur par défaut et capacité d'auto-ajustement
VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(adj_Brightness, min, max, step, default, auto);
```

### Implémentation en C++ MFC

```cpp
// Implémentation C++ MFC pour obtenir les plages de réglage de la luminosité
// Stocker les résultats dans des variables entières pour la configuration de l'UI
int min, max, step, default_value;
BOOL auto_value;
m_VideoCapture.Video_CaptureDevice_VideoAdjust_GetRanges(
  TxVFVideoCapAdjust::adj_Brightness,
  &min,
  &max,
  &step,
  &default_value,
  &auto_value);
```

### Implémentation en VB6

```vb
' Implémentation VB6 pour récupérer les paramètres de réglage de la luminosité
' Utiliser ces valeurs pour configurer les contrôles à curseur et les cases à cocher
Dim min As Integer, max As Integer, step As Integer, default_val As Integer
Dim auto_val As Boolean
VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges adj_Brightness, min, max, step, default_val, auto_val
```

## Définition des valeurs de réglage

Une fois les plages disponibles déterminées, vous pouvez utiliser la méthode `Video_CaptureDevice_VideoAdjust_SetValue` pour appliquer des paramètres spécifiques au flux vidéo.

### Implémentation en Delphi

```pascal
// Définir le niveau de luminosité en fonction de la position du trackbar
// Le troisième paramètre active/désactive le réglage automatique de la luminosité
VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(
  adj_Brightness, 
  tbAdjBrightness.Position,
  cbAdjBrightnessAuto.Checked);
```

### Implémentation en C++ MFC

```cpp
// Implémentation C++ MFC pour définir la valeur de la luminosité
// Utilise la position du curseur pour la valeur de réglage manuel
// L'état de la case à cocher détermine si l'auto-ajustement est activé
m_VideoCapture.Video_CaptureDevice_VideoAdjust_SetValue(
  TxVFVideoCapAdjust::adj_Brightness,
  m_sliderBrightness.GetPos(),
  m_checkBrightnessAuto.GetCheck() == BST_CHECKED);
```

### Implémentation en VB6

```vb
' Implémentation VB6 pour appliquer les réglages de luminosité
' Utilise la valeur du trackbar pour le niveau de réglage
' La valeur de la case à cocher détermine le mode de réglage automatique
VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue _
  adj_Brightness, _
  tbAdjBrightness.Value, _
  cbAdjBrightnessAuto.Value = vbChecked
```

## Bonnes pratiques d'implémentation des réglages vidéo

Lors de l'implémentation des réglages vidéo dans vos applications :

1. Vérifiez toujours d'abord les capacités du périphérique, car tous les périphériques ne prennent pas en charge tous les types de réglages
2. Proposez des contrôles d'UI intuitifs comme des curseurs avec des valeurs min/max appropriées
3. Incluez des options d'auto-ajustement lorsqu'elles sont disponibles
4. Envisagez d'enregistrer les préférences utilisateur pour les sessions futures
5. Implémentez un aperçu en temps réel afin que les utilisateurs puissent voir l'effet de leurs réglages

## Ressources supplémentaires

Veuillez contacter notre équipe de support pour obtenir de l'aide concernant l'implémentation de ces fonctionnalités dans votre application. Visitez notre dépôt GitHub pour des exemples de code et des exemples d'implémentation supplémentaires.
