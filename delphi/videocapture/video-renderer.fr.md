---
title: Moteurs de rendu vidéo en Delphi — EVR, VMR9 et GDI
description: Choisissez le moteur de rendu vidéo optimal en Delphi — Video Renderer, VMR9, EVR avec performances et accélération matérielle.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
primary_api_classes:
  - TVFVideoCapture

---

# Guide de sélection du moteur de rendu vidéo pour TVFVideoCapture

## Vue d'ensemble des moteurs de rendu disponibles

Lors du développement d'applications de capture vidéo avec TVFVideoCapture, la sélection du moteur de rendu vidéo approprié impacte considérablement les performances et la compatibilité. Ce guide fournit des exemples d'implémentation détaillés pour les trois options de moteur de rendu disponibles dans les environnements Delphi, C++ et VB6.

## Moteur de rendu vidéo standard

Le moteur de rendu vidéo standard utilise GDI pour les opérations de dessin. Cette option de moteur de rendu est principalement recommandée pour :

- Les systèmes hérités
- Les environnements où l'accélération Direct3D n'est pas disponible
- La compatibilité maximale avec le matériel ancien

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_VideoRenderer;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_VideoRenderer);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_VideoRenderer
```

## Video Mixing Renderer 9 (VMR9)

VMR9 représente une solution de filtrage moderne capable de tirer parti des capacités du GPU pour un rendu amélioré. Les principaux avantages incluent :

- Traitement vidéo accéléré matériellement
- Options de désentrelacement avancées
- Performances améliorées pour le contenu haute résolution

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_VMR9;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_VMR9);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_VMR9
```

### Accès aux modes de désentrelacement

VMR9 prend en charge plusieurs techniques de désentrelacement. Le code suivant montre comment récupérer les options de désentrelacement disponibles :

```pascal
// Delphi
VideoCapture1.Video_Renderer_Deinterlace_Modes_Fill;
for I := 0 to VideoCapture1.Video_Renderer_Deinterlace_Modes_GetCount - 1 do
  cbDeinterlaceModes.Items.Add(VideoCapture1.Video_Renderer_Deinterlace_Modes_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Video_Renderer_Deinterlace_Modes_Fill();
for (int i = 0; i < m_VideoCapture.Video_Renderer_Deinterlace_Modes_GetCount(); i++) {
    m_DeinterlaceCombo.AddString(m_VideoCapture.Video_Renderer_Deinterlace_Modes_GetItem(i));
}
```

```vb
' VB6
VideoCapture1.Video_Renderer_Deinterlace_Modes_Fill
For i = 0 To VideoCapture1.Video_Renderer_Deinterlace_Modes_GetCount - 1
    cboDeinterlaceModes.AddItem VideoCapture1.Video_Renderer_Deinterlace_Modes_GetItem(i)
Next i
```

## Enhanced Video Renderer (EVR)

EVR est le moteur de rendu recommandé pour les environnements Windows modernes (Vista et ultérieurs). Ce moteur de rendu avancé offre :

- Des capacités d'accélération vidéo supérieures
- Des performances optimales sur Windows 7/10/11
- Une meilleure utilisation des ressources

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_EVR;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_EVR);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_EVR
```

## Gestion du ratio d'aspect et des options d'affichage

Lors de l'affichage du contenu vidéo, vous aurez souvent besoin de gérer les différences de ratio d'aspect entre la vidéo source et la zone d'affichage.

### Étirer l'image vidéo

Pour étirer la vidéo afin qu'elle remplisse toute la zone d'affichage :

```pascal
// Delphi
VideoCapture1.Screen_Stretch := true;
VideoCapture1.Screen_Update;
```

```cpp
// C++ MFC
m_VideoCapture.SetScreen_Stretch(true);
m_VideoCapture.Screen_Update();
```

```vb
' VB6
VideoCapture1.Screen_Stretch = True
VideoCapture1.Screen_Update
```

### Utilisation du mode letterbox (bordures noires)

Pour préserver le ratio d'aspect original avec des bordures noires :

```pascal
// Delphi
VideoCapture1.Screen_Stretch := false;
VideoCapture1.Screen_Update;
```

```cpp
// C++ MFC
m_VideoCapture.SetScreen_Stretch(false);
m_VideoCapture.Screen_Update();
```

```vb
' VB6
VideoCapture1.Screen_Stretch = False
VideoCapture1.Screen_Update
```

## Considérations de performance

Lors du choix d'un moteur de rendu pour votre application, prenez en compte ces facteurs :

1. La version du système d'exploitation cible
2. Les capacités matérielles des systèmes des utilisateurs finaux
3. La résolution vidéo et les exigences de traitement
4. Les besoins de compatibilité pour votre environnement de déploiement

---
Veuillez contacter le [support](https://support.visioforge.com/) si vous avez besoin d'une assistance technique pour cette implémentation. Visitez notre dépôt [GitHub](https://github.com/visioforge/) pour des exemples de code et des ressources supplémentaires.
