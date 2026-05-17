---
title: Capture vidéo au format WMV en Delphi, C++ MFC et VB6
description: Capturez la vidéo au format WMV — profils externes, configuration de sortie et implémentation pour Delphi, C++ MFC et VB6 avec exemples de code.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - C++
  - Windows
  - VCL
  - Capture
  - Streaming
  - Encoding
  - WMV

---

# Capture vidéo vers Windows Media Video (WMV) à l'aide de profils externes

## Introduction

La capture vidéo au format Windows Media Video (WMV) est une exigence courante dans de nombreuses applications logicielles. Ce guide fournit un parcours détaillé de l'implémentation de la fonctionnalité de capture vidéo à l'aide de profils WMV externes dans des applications Delphi, C++ MFC et VB6. Le format WMV reste populaire grâce à sa compatibilité avec les plateformes Windows et à ses algorithmes de compression efficaces qui équilibrent qualité et taille de fichier.

## Comprendre le WMV et les profils externes

Windows Media Video (WMV) est un format de fichier vidéo compressé développé par Microsoft dans le cadre du framework Windows Media. Lors de la capture vidéo au format WMV, l'utilisation de profils externes permet une plus grande flexibilité et personnalisation de la sortie. Les profils externes contiennent des paramètres préconfigurés qui définissent :

- La résolution vidéo
- Le débit binaire
- La fréquence d'images
- La qualité de compression
- Les paramètres audio
- D'autres paramètres d'encodage

En tirant parti des profils externes, les développeurs peuvent rapidement implémenter différents préréglages de qualité sans avoir à configurer manuellement chaque paramètre dans le code.

## Étapes d'implémentation

### Étape 1 : configuration de votre environnement

Avant d'implémenter la fonctionnalité de capture vidéo, assurez-vous que votre environnement de développement est correctement configuré :

1. Installez le composant de capture vidéo nécessaire
2. Ajoutez la référence du composant à votre projet
3. Concevez votre interface utilisateur pour inclure :
   - Un sélecteur de fichier pour choisir le profil WMV
   - Un sélecteur d'emplacement du fichier de sortie
   - Une fenêtre d'aperçu de capture vidéo
   - Des contrôles Démarrer/Arrêter la capture

### Étape 2 : sélection d'un profil WMV

La première étape de l'implémentation consiste à spécifier quel profil WMV utiliser pour l'encodage. Ce profil contient tous les paramètres d'encodage qui seront appliqués à la vidéo capturée.

> `WMV_Profile_Filename` attend le chemin vers un fichier de profil Windows Media `.prx` (le modèle de paramètres d'encodage) — PAS le chemin vers le fichier de sortie `.wmv` capturé. Définissez le nom du fichier capturé via `Output_Filename` (ou la propriété de nom de fichier standard du projet).

#### Delphi

```pascal
// Les littéraux de chaîne Pascal utilisent des apostrophes simples.
VideoCapture1.WMV_Profile_Filename := 'C:\Profiles\HighQuality.prx';
```

#### C++ MFC

```cpp
m_videoCapture.SetWMVProfileFilename(_T("C:\\Profiles\\HighQuality.prx"));
```

#### VB6

```vb
VideoCapture1.WMV_Profile_Filename = "C:\Profiles\HighQuality.prx"
```

### Étape 3 : configuration du format de sortie

Une fois le profil sélectionné, vous devez configurer le composant pour utiliser WMV comme format de sortie. Cela indique au composant de capture quel encodeur utiliser pour traiter le flux vidéo.

#### Delphi

```pascal
VideoCapture1.OutputFormat := Format_WMV;
```

#### C++ MFC

```cpp
m_videoCapture.SetOutputFormat(FORMAT_WMV);
```

#### VB6

```vb
VideoCapture1.OutputFormat = FORMAT_WMV
```

### Étape 4 : définition du mode de capture

Le composant de capture peut fonctionner dans différents modes, il est donc important de le définir explicitement en mode de capture vidéo.

#### Delphi

```pascal
VideoCapture1.Mode := Mode_Video_Capture;
```

#### C++ MFC

```cpp
m_videoCapture.SetMode(MODE_VIDEO_CAPTURE);
```

#### VB6

```vb
VideoCapture1.Mode = MODE_VIDEO_CAPTURE
```

Cela garantit que le composant est configuré pour un enregistrement vidéo continu plutôt que pour d'autres modes tels que la capture d'instantanés ou la diffusion.

### Étape 5 : démarrage de la capture vidéo

Avec toute la configuration en place, l'étape finale consiste à démarrer le processus de capture proprement dit.

#### Delphi

```pascal
VideoCapture1.Start;
```

#### C++ MFC

```cpp
m_videoCapture.Start();
```

#### VB6

```vb
VideoCapture1.Start
```

Cette commande lance le processus de capture en utilisant tous les paramètres précédemment configurés.

## Options de configuration avancées

### Nommage personnalisé du fichier de sortie

Vous pouvez implémenter un nommage personnalisé pour vos fichiers vidéo capturés :

#### Delphi

```pascal
VideoCapture1.Output_Filename := 'C:\Captures\Video_' + FormatDateTime('yyyymmdd_hhnnss', Now) + '.wmv';
```

#### C++ MFC

```cpp
CTime currentTime = CTime::GetCurrentTime();
CString fileName;
fileName.Format(_T("C:\\Captures\\Video_%04d%02d%02d_%02d%02d%02d.wmv"), 
                currentTime.GetYear(), currentTime.GetMonth(), currentTime.GetDay(),
                currentTime.GetHour(), currentTime.GetMinute(), currentTime.GetSecond());
m_videoCapture.SetOutputFilename(fileName);
```

#### VB6

```vb
VideoCapture1.Output_Filename = "C:\Captures\Video_" & Format(Now, "yyyymmdd_hhnnss") & ".wmv"
```

Ces exemples créent un nom de fichier horodaté pour garantir que chaque fichier capturé possède un nom unique.

Lors de la conception de votre application, tenez compte de ces bonnes pratiques :

1. Vérifiez toujours la disponibilité du périphérique avant de tenter une capture
2. Fournissez un retour pendant les longues opérations d'encodage
3. Incluez une fenêtre d'aperçu pour que les utilisateurs puissent voir ce qui est capturé
4. Implémentez un moniteur de taille de fichier pour les enregistrements longs
5. Testez avec divers profils WMV pour garantir la compatibilité

## Conclusion

L'implémentation de la capture vidéo au format WMV à l'aide de profils externes offre flexibilité et contrôle sur le processus de capture. L'approche présentée dans ce guide fonctionne efficacement dans les environnements de développement Delphi, C++ MFC et VB6, vous permettant d'intégrer des capacités de capture vidéo de qualité professionnelle dans vos applications.

En utilisant des profils externes, vous pouvez rapidement basculer entre différents paramètres de qualité sans modifier votre code, ce qui est idéal pour les applications qui doivent s'adapter à différents cas d'usage ou capacités matérielles.

---
Pour des exemples de code supplémentaires, visitez notre dépôt GitHub. Si vous avez besoin d'une assistance technique pour l'implémentation, notre équipe de support est disponible pour vous aider.
