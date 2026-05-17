---
title: Capture et enregistrement d'écran en Delphi — VCL/ActiveX
description: Implémentez l'enregistrement d'écran en Delphi avec TVFVideoCapture — capturez régions, plein écran, personnalisez la fréquence d'images et suivez le curseur.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Screen Capture
primary_api_classes:
  - TVFVideoCapture

---

# Implémentation de l'enregistrement d'écran en Delphi

## Introduction à la fonctionnalité de capture d'écran

TVFVideoCapture offre de puissantes capacités d'enregistrement d'écran aux développeurs Delphi. Ce guide présente l'implémentation des fonctionnalités de capture d'écran dans vos applications, vous permettant d'enregistrer des régions spécifiques ou l'écran entier avec des paramètres personnalisables.

## Configuration de la zone de capture d'écran

Vous pouvez contrôler précisément quelle portion de l'écran enregistrer en définissant des paramètres de coordonnées. Ceci est particulièrement utile lorsque vous souhaitez vous concentrer sur des fenêtres d'application ou des régions d'écran spécifiques.

### Définition de coordonnées d'écran spécifiques

Utilisez ces paramètres pour définir les limites exactes de votre zone de capture :

```pascal
// Définir la position du bord supérieur du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Top := StrToInt(edScreenTop.Text);
// Définir la position du bord inférieur du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Bottom := StrToInt(edScreenBottom.Text);
// Définir la position du bord gauche du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Left := StrToInt(edScreenLeft.Text);
// Définir la position du bord droit du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Right := StrToInt(edScreenRight.Text);
```

```cpp
// CEdit::GetWindowText(CString&) renvoie void et remplit le tampon par référence,
// nous devons donc déclarer d'abord une CString puis la convertir en int via _ttoi
// (la variante compatible TCHAR — requise car CString est wide dans les builds MFC Unicode).
CString sTop, sBottom, sLeft, sRight;
m_edScreenTop.GetWindowText(sTop);
m_edScreenBottom.GetWindowText(sBottom);
m_edScreenLeft.GetWindowText(sLeft);
m_edScreenRight.GetWindowText(sRight);

// Définir la position du bord supérieur du rectangle de capture (en pixels)
m_VideoCapture.SetScreen_Capture_Top(_ttoi(sTop));
// Définir la position du bord inférieur du rectangle de capture (en pixels)
m_VideoCapture.SetScreen_Capture_Bottom(_ttoi(sBottom));
// Définir la position du bord gauche du rectangle de capture (en pixels)
m_VideoCapture.SetScreen_Capture_Left(_ttoi(sLeft));
// Définir la position du bord droit du rectangle de capture (en pixels)
m_VideoCapture.SetScreen_Capture_Right(_ttoi(sRight));
```

```vb
' Définir la position du bord supérieur du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Top = CInt(edScreenTop.Text)
' Définir la position du bord inférieur du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Bottom = CInt(edScreenBottom.Text)
' Définir la position du bord gauche du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Left = CInt(edScreenLeft.Text)
' Définir la position du bord droit du rectangle de capture (en pixels)
VideoCapture1.Screen_Capture_Right = CInt(edScreenRight.Text)
```

### Capture de l'écran entier

Pour un enregistrement complet de l'écran, activez simplement l'option de capture plein écran :

```pascal
// Activer le mode capture plein écran - enregistrera l'affichage entier
VideoCapture1.Screen_Capture_FullScreen := true;
```

```cpp
// Activer le mode capture plein écran - enregistrera l'affichage entier
m_VideoCapture.SetScreen_Capture_FullScreen(true);
```

```vb
' Activer le mode capture plein écran - enregistrera l'affichage entier
VideoCapture1.Screen_Capture_FullScreen = True
```

## Optimisation des paramètres de fréquence d'images

La fréquence d'images impacte directement la qualité et la taille de fichier de vos enregistrements d'écran. Des fréquences d'images plus élevées produisent une vidéo plus fluide mais génèrent des fichiers plus volumineux.

```pascal
// Définir la fréquence d'images de capture à 10 images par seconde
// Ajustez cette valeur en fonction de vos exigences de performance
VideoCapture1.Screen_Capture_FrameRate := 10;
```

```cpp
// Définir la fréquence d'images de capture à 10 images par seconde
// Ajustez cette valeur en fonction de vos exigences de performance
m_VideoCapture.SetScreen_Capture_FrameRate(10);
```

```vb
' Définir la fréquence d'images de capture à 10 images par seconde
' Ajustez cette valeur en fonction de vos exigences de performance
VideoCapture1.Screen_Capture_FrameRate = 10
```

## Configuration du suivi du curseur

Pour les vidéos pédagogiques ou les démonstrations, capturer le mouvement du curseur de la souris est essentiel :

```pascal
// Activer la capture du curseur de la souris dans l'enregistrement
// Définir sur false pour masquer le curseur dans la vidéo de sortie
VideoCapture1.Screen_Capture_Grab_Mouse_Cursor := true;
```

```cpp
// Activer la capture du curseur de la souris dans l'enregistrement
// Définir sur false pour masquer le curseur dans la vidéo de sortie
m_VideoCapture.SetScreen_Capture_Grab_Mouse_Cursor(true);
```

```vb
' Activer la capture du curseur de la souris dans l'enregistrement
' Définir sur false pour masquer le curseur dans la vidéo de sortie
VideoCapture1.Screen_Capture_Grab_Mouse_Cursor = True
```

## Activation du mode capture d'écran

Après avoir configuré tous les paramètres, définissez le composant en mode capture d'écran pour commencer l'enregistrement :

```pascal
// Définir le composant en mode opérationnel de capture d'écran
// Cela active toutes les fonctionnalités d'enregistrement d'écran
VideoCapture1.Mode := Mode_Screen_Capture;
```

```cpp
// Définir le composant en mode opérationnel de capture d'écran
// Cela active toutes les fonctionnalités d'enregistrement d'écran
m_VideoCapture.SetMode(Mode_Screen_Capture);
```

```vb
' Définir le composant en mode opérationnel de capture d'écran
' Cela active toutes les fonctionnalités d'enregistrement d'écran
VideoCapture1.Mode = Mode_Screen_Capture
```

## Conseils d'implémentation avancée

Pour des performances optimales d'enregistrement d'écran :

- Tenez compte des ressources système lors du choix des fréquences d'images
- Utilisez la capture par région lorsque possible pour minimiser la charge de traitement
- Testez différents paramètres de qualité pour équilibrer taille de fichier et qualité visuelle
- N'oubliez pas que la capture du curseur ajoute une légère surcharge de traitement

---
Pour des exemples de code et des exemples d'implémentation supplémentaires, visitez notre dépôt [GitHub](https://github.com/visioforge/). Pour obtenir une assistance technique concernant l'implémentation, veuillez contacter notre [équipe de support](https://support.visioforge.com/).
