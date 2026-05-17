---
title: Redimensionner et rogner la vidéo en Delphi — VCL/ActiveX
description: Redimensionnement et rognage vidéo en Delphi — traitement temps réel, ratio d'aspect et performances avec exemples de code.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Streaming
primary_api_classes:
  - TVFVideoCapture

---

# Redimensionnement et rognage vidéo dans Delphi TVFVideoCapture

La manipulation vidéo est un élément essentiel de nombreuses applications modernes. Ce guide fournit des instructions détaillées pour implémenter le redimensionnement et le rognage vidéo en temps réel dans vos applications Delphi avec un impact minimal sur les performances.

## Pourquoi redimensionner ou rogner la vidéo ?

Le redimensionnement et le rognage vidéo répondent à plusieurs objectifs en développement :

- Optimiser la vidéo pour différentes tailles d'affichage
- Réduire les besoins en bande passante pour la diffusion
- Se concentrer sur des régions d'intérêt spécifiques
- Créer des dimensions vidéo uniformes dans l'ensemble de votre application
- Améliorer les performances sur les appareils aux ressources limitées

## Activation des fonctionnalités de redimensionnement et de rognage

Avant d'appliquer toute transformation, vous devez activer la fonctionnalité de redimensionnement/rognage dans le composant TVFVideoCapture.

### Étape 1 : activer la fonctionnalité

```pascal
// Activer la fonctionnalité de redimensionnement ou de rognage vidéo
VideoCapture1.Video_ResizeOrCrop_Enabled := true;
```

```cpp
// C++ MFC - Activer les capacités de transformation vidéo
m_VideoCapture.SetVideo_ResizeOrCrop_Enabled(TRUE);
```

```vb
' VB6 - Activer les fonctionnalités de redimensionnement/rognage
VideoCapture1.Video_ResizeOrCrop_Enabled = True
```

## Implémentation du redimensionnement vidéo

Le redimensionnement vous permet de modifier les dimensions de votre flux vidéo tout en préservant la qualité visuelle.

### Définition des nouvelles dimensions

```pascal
// Définir la largeur et la hauteur souhaitées pour la sortie vidéo redimensionnée
VideoCapture1.Video_Resize_NewWidth := StrToInt(edResizeWidth.Text);
VideoCapture1.Video_Resize_NewHeight := StrToInt(edResizeHeight.Text);
```

```cpp
// C++ MFC - Configurer les dimensions cibles pour le redimensionnement vidéo
m_VideoCapture.SetVideo_Resize_NewWidth(_ttoi(m_strResizeWidth));
m_VideoCapture.SetVideo_Resize_NewHeight(_ttoi(m_strResizeHeight));
```

```vb
' VB6 - Définir les nouvelles dimensions vidéo
VideoCapture1.Video_Resize_NewWidth = CInt(txtResizeWidth.Text)
VideoCapture1.Video_Resize_NewHeight = CInt(txtResizeHeight.Text)
```

### Gestion des changements de ratio d'aspect

Lors du redimensionnement de la vidéo, vous pouvez choisir entre préserver le ratio d'aspect original (letterbox) ou étirer le contenu pour s'adapter aux nouvelles dimensions.

```pascal
// Le mode letterbox ajoute des bordures noires pour préserver le ratio d'aspect
// Lorsqu'il est défini sur false, la vidéo s'étire pour s'adapter aux nouvelles dimensions
VideoCapture1.Video_Resize_LetterBox := cbResizeLetterbox.Checked;
```

```cpp
// C++ MFC - Configurer la méthode de gestion du ratio d'aspect
m_VideoCapture.SetVideo_Resize_LetterBox(m_bResizeLetterbox);
```

```vb
' VB6 - Définir le mode letterbox pour préserver le ratio d'aspect
VideoCapture1.Video_Resize_LetterBox = chkResizeLetterbox.Value
```

### Sélection des algorithmes de redimensionnement

Choisissez parmi plusieurs algorithmes de redimensionnement selon vos exigences de qualité et contraintes de performances :

```pascal
// Sélectionner l'algorithme de redimensionnement approprié :
// - NearestNeighbor : le plus rapide mais qualité la plus basse
// - Bilinear : bon équilibre entre vitesse et qualité
// - Bilinear_HQ : bilinéaire amélioré avec qualité accrue
// - Bicubic : meilleure qualité avec impact modéré sur les performances
// - Bicubic_HQ : qualité maximale avec utilisation CPU la plus élevée
case cbResizeMode.ItemIndex of
  0: VideoCapture1.Video_Resize_Mode := rm_NearestNeighbor;
  1: VideoCapture1.Video_Resize_Mode := rm_Bilinear;
  2: VideoCapture1.Video_Resize_Mode := rm_Bilinear_HQ;
  3: VideoCapture1.Video_Resize_Mode := rm_Bicubic;
  4: VideoCapture1.Video_Resize_Mode := rm_Bicubic_HQ;
end;
```

```cpp
// C++ MFC - Définir l'algorithme de redimensionnement selon les besoins qualité/performance
switch(m_nResizeMode)
{
  case 0: m_VideoCapture.SetVideo_Resize_Mode(rm_NearestNeighbor); break; // Le plus rapide
  case 1: m_VideoCapture.SetVideo_Resize_Mode(rm_Bilinear); break;        // Standard
  case 2: m_VideoCapture.SetVideo_Resize_Mode(rm_Bilinear_HQ); break;     // Amélioré
  case 3: m_VideoCapture.SetVideo_Resize_Mode(rm_Bicubic); break;         // Haute qualité
  case 4: m_VideoCapture.SetVideo_Resize_Mode(rm_Bicubic_HQ); break;      // Qualité maximale
}
```

```vb
' VB6 - Choisir l'algorithme de redimensionnement selon les besoins qualité/performance
Select Case cboResizeMode.ListIndex
  Case 0: VideoCapture1.Video_Resize_Mode = rm_NearestNeighbor  ' Le plus rapide, qualité moindre
  Case 1: VideoCapture1.Video_Resize_Mode = rm_Bilinear         ' Option équilibrée
  Case 2: VideoCapture1.Video_Resize_Mode = rm_Bilinear_HQ      ' Bilinéaire amélioré
  Case 3: VideoCapture1.Video_Resize_Mode = rm_Bicubic          ' Meilleure qualité
  Case 4: VideoCapture1.Video_Resize_Mode = rm_Bicubic_HQ       ' Qualité maximale
End Select
```

## Implémentation du rognage vidéo

Le rognage vous permet de sélectionner une région d'intérêt spécifique dans votre flux vidéo.

### Étape 1 : activer le rognage

Comme pour le redimensionnement, vous devez d'abord activer la fonctionnalité :

```pascal
// Activer les capacités de transformation vidéo avant d'appliquer le rognage
VideoCapture1.Video_ResizeOrCrop_Enabled := true;
```

```cpp
// C++ MFC - Activer les fonctionnalités de manipulation vidéo
m_VideoCapture.SetVideo_ResizeOrCrop_Enabled(TRUE);
```

```vb
' VB6 - Activer la fonctionnalité de transformation vidéo
VideoCapture1.Video_ResizeOrCrop_Enabled = True
```

### Étape 2 : définir la région de rognage

Spécifiez les limites de votre région de rognage en définissant les coordonnées gauche, haut, droite et bas :

```pascal
// Définir les coordonnées de la région de rognage en pixels
// Ces valeurs représentent la distance depuis chaque bord de la vidéo originale
VideoCapture1.Video_Crop_Left := StrToInt(edCropLeft.Text);
VideoCapture1.Video_Crop_Top := StrToInt(edCropTop.Text);
VideoCapture1.Video_Crop_Right := StrToInt(edCropRight.Text);
VideoCapture1.Video_Crop_Bottom := StrToInt(edCropBottom.Text);
```

```cpp
// C++ MFC - Définir les limites de rognage en pixels
// Chaque valeur définit combien de pixels rogner depuis le bord correspondant
m_VideoCapture.SetVideo_Crop_Left(_ttoi(m_strCropLeft));
m_VideoCapture.SetVideo_Crop_Top(_ttoi(m_strCropTop));
m_VideoCapture.SetVideo_Crop_Right(_ttoi(m_strCropRight));
m_VideoCapture.SetVideo_Crop_Bottom(_ttoi(m_strCropBottom));
```

```vb
' VB6 - Configurer les limites de la région de rognage
' Les valeurs représentent le nombre de pixels à exclure depuis chaque bord
VideoCapture1.Video_Crop_Left = CInt(txtCropLeft.Text)
VideoCapture1.Video_Crop_Top = CInt(txtCropTop.Text)
VideoCapture1.Video_Crop_Right = CInt(txtCropRight.Text)
VideoCapture1.Video_Crop_Bottom = CInt(txtCropBottom.Text)
```

## Bonnes pratiques de manipulation vidéo

Pour des résultats optimaux lors de l'implémentation du redimensionnement et du rognage vidéo :

1. **Tester sur le matériel cible** - Les différents algorithmes de redimensionnement ont des exigences CPU variables
2. **Considérer votre cas d'usage** - Pour les applications en temps réel, privilégiez la performance sur la qualité
3. **Préserver les ratios d'aspect** - Sauf besoin spécifique, conservez les proportions originales
4. **Combiner les opérations judicieusement** - Appliquer à la fois redimensionnement et rognage augmente la charge de traitement
5. **Mettre les paramètres en cache** - Évitez de modifier les paramètres fréquemment pendant la capture

## Dépannage des problèmes courants

- Si les performances sont médiocres, essayez un algorithme de redimensionnement plus rapide
- Assurez-vous que les valeurs de rognage ne dépassent pas les dimensions de votre flux vidéo
- Lors de l'utilisation du mode letterbox, tenez compte des bordures noires dans votre conception d'UI
- Pour de meilleurs résultats, redimensionnez à des dimensions multiples de 8 ou 16

---
Pour des exemples de code et des exemples d'implémentation supplémentaires, visitez notre dépôt [GitHub](https://github.com/visioforge/). Besoin d'assistance technique ? Contactez notre équipe de support pour des conseils personnalisés.
