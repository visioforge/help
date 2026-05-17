---
title: Détection de mouvement — zones et alertes en C# .NET
description: Ajoutez la détection de mouvement simple et avancée aux flux webcam ou caméra IP avec le VisioForge Video Capture SDK. Zones, sensibilité et exemples C#.
sidebar_label: Détection de mouvement
order: 6
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
primary_api_classes:
  - MotionDetectionSettings
  - MotionDetectionExSettings

---

# Détection de mouvement pour le traitement vidéo

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Le Video Capture SDK offre de puissantes capacités de détection de mouvement pour vos applications .NET. Que vous ayez besoin d'une détection de présence simple ou d'un suivi d'objet avancé, le SDK propose deux implémentations de détecteur de mouvement distinctes pour répondre à vos exigences spécifiques :

1. **Détecteur de mouvement simple** — traitement léger et efficace avec des matrices de détection personnalisables
2. **Détecteur de mouvement avancé** — capacités améliorées incluant la détection d'objets, plusieurs types de processeurs et des détecteurs spécialisés

Ces outils de détection de mouvement permettent aux développeurs d'implémenter des fonctionnalités sophistiquées d'analyse vidéo telles que la surveillance de sécurité, les alertes automatisées, le comptage d'objets et les applications réactives au mouvement interactives.

## Détecteur de mouvement simple

[VideoCaptureCore](#){ .md-button }

### Fonctionnement

Le détecteur de mouvement simple offre une solution efficace pour les scénarios de détection de mouvement basiques. Son approche simplifiée le rend idéal pour les applications où la vitesse de traitement et l'efficacité des ressources sont prioritaires.

Lorsqu'il est activé, le détecteur :

- Traite chaque image pour détecter les changements de mouvement
- Génère un tableau d'octets bidimensionnel (matrice de mouvement)
- Calcule le niveau global de mouvement en pourcentage
- Met optionnellement en surbrillance visuellement les zones de mouvement détectées

### Fonctionnalités clés

- **Taille de matrice personnalisable** : ajustez la résolution de détection pour équilibrer performance et précision
- **Sélection de canal** : analysez tous les canaux RGB ou concentrez-vous sur des canaux spécifiques pour une détection optimisée
- **Mise en surbrillance du mouvement** : mettez visuellement en avant le mouvement détecté avec des superpositions de couleur
- **Optimisation des performances** : configurez les paramètres d'intervalle d'images pour gérer la charge de traitement

### Exemple d'implémentation

```cs
// créer les paramètres du détecteur de mouvement
var motionDetector = new MotionDetectionSettings();

// définir les dimensions de la matrice du détecteur de mouvement
motionDetector.Matrix_Width = 10;
motionDetector.Matrix_Height = 10;

// configurer l'analyse des canaux de couleur
motionDetector.Compare_Red = false;
motionDetector.Compare_Green = false;
motionDetector.Compare_Blue = false;
motionDetector.Compare_Greyscale = true;

// configuration de la mise en surbrillance du mouvement
motionDetector.Highlight_Color = MotionCHLColor.Green;
motionDetector.Highlight_Enabled = true;

// paramètres d'optimisation des performances
motionDetector.FrameInterval = 5;
motionDetector.DropFrames_Enabled = false;

// activer la détection (par défaut à false — requis pour que les événements OnMotion se déclenchent)
motionDetector.Enabled = true;

// appliquer les paramètres au composant de capture vidéo
VideoCapture1.Motion_Detection = motionDetector;
VideoCapture1.MotionDetection_Update();
```

### Récupération des données de mouvement

Abonnez-vous à l'événement `OnMotion` (type : `EventHandler<MotionDetectionEventArgs>`) avant de démarrer le pipeline. Le gestionnaire se déclenche une fois par image lorsqu'un mouvement est détecté :

```cs
VideoCapture1.OnMotion += (sender, e) =>
{
    // e.Level  — intensité globale du mouvement (int, généralement 0-100).
    // e.Matrix — superposition de grille byte[] ; chaque cellule contient la quantité de mouvement pour cette région.
    if (e.Level > 25)
    {
        Console.WriteLine($"Motion level {e.Level}% — {e.Matrix?.Length ?? 0} grid cells");
        // Déclencher un enregistrement, envoyer une alerte, mettre un instantané en file d'attente, etc.
    }
};
```

Les événements sont levés sur un thread d'arrière-plan — passez par le thread d'interface utilisateur (`Dispatcher`/`Invoke`) avant de toucher aux contrôles UI.

## Détecteur de mouvement avancé

[VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

### Capacités améliorées

Le détecteur de mouvement avancé fournit des algorithmes de détection et des options d'analyse plus sophistiqués. Ce détecteur est conçu pour les applications nécessitant des informations détaillées sur le mouvement, l'identification d'objets et la définition précise des zones de mouvement.

Les avantages clés incluent :

- Détection et comptage d'objets
- Plusieurs types de processeurs pour différents besoins d'analyse visuelle
- Algorithmes de détecteur spécialisés pour divers environnements
- Traitement amélioré des zones de mouvement

### Options de configuration

#### Types de processeur de mouvement

Le processeur détermine comment le mouvement est analysé et visualisé :

- **None** : détection brute sans mise en surbrillance visuelle
- **BlobCountingObjects** : identifie et compte les objets distincts en mouvement
- **GridMotionAreaProcessing** : divise l'image en sections de grille pour analyse
- **MotionAreaHighlighting** : met en surbrillance les zones complètes où un mouvement est détecté
- **MotionBorderHighlighting** : trace le périmètre des zones de mouvement

#### Types de détecteur de mouvement

L'algorithme de détecteur définit l'approche fondamentale de l'identification du mouvement :

- **CustomFrameDifference** : compare l'image actuelle à un arrière-plan prédéfini
- **SimpleBackgroundModeling** : utilise des techniques de modélisation d'arrière-plan adaptatives
- **TwoFramesDifference** : analyse les différences entre images consécutives

### Étapes d'implémentation

1. Créez les paramètres du détecteur de mouvement avancé :

```cs
var motionDetector = new MotionDetectionExSettings();
```

2. Sélectionnez le type de processeur approprié pour les besoins de votre application :

```cs
motionDetector.ProcessorType = MotionProcessorType.BlobCountingObjects;
```

3. Choisissez l'algorithme de détection le mieux adapté à votre environnement :

```cs
motionDetector.DetectorType = MotionDetectorType.CustomFrameDifference;
```

4. Appliquez les paramètres à votre composant de capture vidéo :

=== "VideoCaptureCoreX"

    
    ```cs
    VideoCapture1.Motion_Detection = motionDetector;
    ```
    

=== "VideoCaptureCore"

    
    ```cs
    VideoCapture1.Motion_DetectionEx = motionDetector;
    ```
    


5. Implémentez le gestionnaire d'événements correspondant pour recevoir les données de détection. Les deux événements transportent `MotionDetectionExEventArgs` avec `Level` (float), `LevelPercent` (int 0-100), `ObjectsCount`, `ObjectRectangles` (`Rect[]`) et `MotionGrid` (`float[,]`) :

    === "VideoCaptureCoreX"

        ```cs
        VideoCapture1.OnMotionDetection += (sender, e) =>
        {
            if (e.LevelPercent > 25)
            {
                Console.WriteLine($"Motion {e.LevelPercent}% — {e.ObjectsCount} moving objects");
                foreach (var rect in e.ObjectRectangles)
                {
                    Console.WriteLine($"  at {rect}");
                }
            }
        };
        ```

    === "VideoCaptureCore"

        ```cs
        VideoCapture1.OnMotionDetectionEx += (sender, e) =>
        {
            if (e.LevelPercent > 25)
            {
                Console.WriteLine($"Motion {e.LevelPercent}% — {e.ObjectsCount} moving objects");
            }
        };
        ```

    Les gestionnaires s'exécutent sur un thread de travail — passez par le thread d'interface utilisateur avant de mettre à jour les contrôles.

## Applications pratiques

Les capacités de détection de mouvement permettent aux développeurs de créer de puissantes applications de traitement vidéo :

- **Systèmes de sécurité** : déclencher un enregistrement ou des alertes lorsqu'un mouvement non autorisé est détecté
- **Analyse du trafic** : compter et suivre les véhicules ou les piétons
- **Installations interactives** : créer des expériences numériques réactives au mouvement
- **Indexation vidéo automatisée** : marquer et catégoriser les sections contenant de l'activité
- **Automatisation industrielle** : surveiller les lignes de production ou les zones restreintes
- **Observation de la faune** : enregistrer l'activité animale sans intervention humaine

## Considérations de performance

Pour optimiser les performances de détection de mouvement :

1. Ajustez les dimensions de la matrice pour équilibrer précision et charge de traitement
2. Utilisez les paramètres d'intervalle d'images pour analyser uniquement les images essentielles
3. Sélectionnez les canaux de couleur appropriés pour votre scénario de détection
4. Envisagez d'activer l'option d'abandon d'images pour des exigences haute performance
5. Choisissez le type de détecteur en fonction de vos conditions d'environnement spécifiques

## Configuration avancée

La classe avancée `MotionDetectionExSettings` (utilisée par `VideoCaptureCoreX` via `Motion_Detection` / par le `VideoCaptureCore` classique via `Motion_DetectionEx`) expose ces propriétés supplémentaires :

- `DifferenceThreshold` — seuil de différence par pixel pour filtrer les mouvements mineurs
- `MinObjectsWidth` / `MinObjectsHeight` — ignorer les objets détectés en dessous de ces dimensions
- `SuppressNoise` — filtre de suppression de bruit pour réduire les faux positifs
- `HighlightMotionRegions` — dessiner des rectangles englobants autour des régions d'objets détectés
- `KeepObjectsEdges` — préserver les bords nets lors de la mise en surbrillance
- `HighlightColor` (SKColor) — couleur utilisée pour la mise en surbrillance du mouvement

## Intégration des événements

Les événements de détection de mouvement peuvent être intégrés avec d'autres fonctionnalités du SDK :

- Enregistrement vidéo pour capturer le mouvement détecté
- Création d'instantanés lorsqu'un mouvement est détecté
- Systèmes de notification personnalisés
- Journalisation et analyse des données

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
