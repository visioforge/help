---
title: Enregistrement vidéo multi-caméras synchronisé en C# .NET
description: Démarrez et arrêtez plusieurs enregistrements webcam ou caméra IP en synchronisation parfaite avec VisioForge Video Capture SDK. Exemples C# et dépannage.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Webcam
  - IP Camera
  - MP4
  - C#
primary_api_classes:
  - VideoCaptureCore

---

# Synchroniser plusieurs captures vidéo en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la capture vidéo multi-sources

Lors du développement d'applications nécessitant l'enregistrement depuis plusieurs sources vidéo simultanément, la synchronisation devient un défi critique. Que vous construisiez des systèmes de vidéosurveillance, des solutions d'enregistrement multi-caméras ou des outils de production vidéo spécialisés, garantir que tous les flux vidéo commencent et terminent l'enregistrement au moment précis peut faire la différence entre des résultats de qualité professionnelle et amateurs.

Ce guide explique comment synchroniser correctement plusieurs objets de capture vidéo dans les applications .NET, en éliminant les décalages temporels entre différentes caméras ou sources d'entrée.

## Comprendre le défi de la synchronisation vidéo

Sans synchronisation appropriée, plusieurs enregistrements vidéo démarrés séquentiellement présenteront des décalages temporels. Même des différences de l'ordre de la milliseconde peuvent causer des problèmes dans les applications où un alignement temporel précis est requis, comme :

- Analyse sportive multi-angles
- Systèmes de caméras de sécurité
- Configurations de capture de mouvement
- Mesures et observations scientifiques
- Production vidéo professionnelle

Ces décalages temporels surviennent parce qu'à chaque initialisation d'un périphérique de capture et au démarrage d'un enregistrement, il existe un surcoût de traitement qui varie entre les périphériques.

## La solution : mécanisme de démarrage différé

Le Video Capture SDK propose une solution élégante via son mécanisme de démarrage différé. Cette approche vous permet de :

1. Initialiser tous les objets de capture et les préparer pour l'enregistrement
2. Les mettre dans un état « prêt » où ils attendent un signal final
3. Déclencher le démarrage de tous les enregistrements avec un délai minimal entre les sources

Cette approche réduit considérablement l'écart de synchronisation entre les enregistrements par rapport aux opérations de démarrage séquentielles.

## Implémentation avec VideoCaptureCore

Dans cette implémentation, nous utiliserons le moteur `VideoCaptureCore` pour démontrer la technique de synchronisation.

### Étape 1 : configurer vos objets de capture vidéo

Tout d'abord, créez et configurez vos objets de capture vidéo pour chaque source :

```csharp
// Créer les objets de capture vidéo
var capture1 = new VideoCaptureCore();
var capture2 = new VideoCaptureCore();

// Configurer les fichiers de sortie
capture1.Output_Filename = "camera1_recording.mp4";
capture2.Output_Filename = "camera2_recording.mp4";

// Configurer les sources vidéo
// ...

// Configurer les autres paramètres au besoin
```

### Étape 2 : activer le démarrage différé

L'étape critique consiste à activer la fonctionnalité de démarrage différé sur tous les objets de capture avant d'appeler leurs méthodes `Start` ou `StartAsync` respectives :

```csharp
// Activer le démarrage différé pour tous les objets de capture
capture1.Start_DelayEnabled = true;
capture2.Start_DelayEnabled = true;
```

### Étape 3 : initialiser les objets de capture

Ensuite, appelez la méthode `Start` ou `StartAsync` sur chaque objet. Cela initialise les sources, les codecs et les fichiers de sortie, mais ne démarre pas réellement le processus d'enregistrement :

```csharp
// Initialiser tous les objets de capture (mais ne pas commencer l'enregistrement encore)
await capture1.StartAsync();
await capture2.StartAsync();

// Ou pour une opération synchrone :
// capture1.Start();
// capture2.Start();
```

À ce stade, tous vos objets de capture sont initialisés et attendent le déclencheur final.

### Étape 4 : déclencher l'enregistrement synchronisé

Enfin, appelez la méthode `StartDelayed` ou `StartDelayedAsync` sur chaque objet pour commencer l'enregistrement avec un délai minimal entre eux :

```csharp
// Démarrer l'enregistrement synchronisé
await capture1.StartDelayedAsync();
await capture2.StartDelayedAsync();

// Ou pour une opération synchrone :
// capture1.StartDelayed();
// capture2.StartDelayed();
```

Cela déclenche le démarrage effectif de l'enregistrement sur tous les périphériques préparés avec le plus petit délai possible entre eux.

## Exemple complet de synchronisation

Voici un exemple complet démontrant un enregistrement synchronisé depuis deux sources vidéo :

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.VideoCapture;

namespace MultiCameraRecordingApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Créer les objets de capture vidéo
            var camera1 = new VideoCaptureCore();
            var camera2 = new VideoCaptureCore();
            
            try
            {
                // Configurer la caméra 1
                // ...
                camera1.Output_Filename = "camera1_recording.mp4";
                
                // Configurer la caméra 2
                // ...
                camera2.Output_Filename = "camera2_recording.mp4";
                
                // Activer le démarrage différé pour la synchronisation
                camera1.Start_DelayEnabled = true;
                camera2.Start_DelayEnabled = true;
                
                Console.WriteLine("Initializing cameras...");
                
                // Initialiser les deux caméras (mais ne pas commencer l'enregistrement encore)
                await camera1.StartAsync();
                await camera2.StartAsync();
                
                Console.WriteLine("Cameras initialized and ready.");
                Console.WriteLine("Starting synchronized recording...");
                
                // Démarrer l'enregistrement synchronisé
                await camera1.StartDelayedAsync();
                await camera2.StartDelayedAsync();
                
                Console.WriteLine("Recording in progress. Press Enter to stop.");
                Console.ReadLine();
                
                // Arrêter l'enregistrement
                await camera1.StopAsync();
                await camera2.StopAsync();
                
                Console.WriteLine("Recording completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Libérer les ressources
                camera1.Dispose();
                camera2.Dispose();
            }
        }
    }
}
```

## Techniques avancées de synchronisation

### Synchronisation matérielle

Pour les applications nécessitant une synchronisation parfaite à l'image près, considérez ces approches supplémentaires :

- Déclencheurs matériels externes : certaines caméras professionnelles prennent en charge les entrées de déclenchement externes
- Genlock : les équipements de diffusion professionnels utilisent souvent le genlock pour la synchronisation au niveau de l'image
- Synchronisation par timecode : intégration de timecodes correspondants dans les fichiers vidéo

### Considérations sur plusieurs formats de fichiers

Lors de l'enregistrement simultané dans différents formats de fichier, sachez que certains formats ont des temps d'initialisation différents. Pour minimiser cet effet :

- Utilisez des paramètres d'encodage identiques lorsque c'est possible
- Préférez des conteneurs au surcoût similaire
- Lors du mélange de formats de conteneurs, initialisez d'abord le format le plus complexe

## Dépannage des problèmes de synchronisation

Si vous rencontrez des problèmes de synchronisation, examinez ces problèmes courants :

1. **Temps d'initialisation variables** : différents modèles de caméras peuvent avoir des temps de démarrage différents. Appelez `StartDelayedAsync` dans l'ordre du périphérique le plus lent au plus rapide.

2. **Contention de ressources** : plusieurs captures haute résolution peuvent se disputer les ressources système. Envisagez de réduire la résolution ou la fréquence d'images pour une meilleure synchronisation.

3. **Limitations de bande passante USB** : lors de l'utilisation de plusieurs caméras USB, les contraintes de bande passante peuvent causer des délais. Utilisez des contrôleurs USB séparés si possible.

4. **Surcharge CPU** : l'encodage haute résolution sur plusieurs flux peut surcharger le CPU. Surveillez l'utilisation CPU et envisagez d'utiliser l'encodage matériel.

## Optimisation des performances

Pour maximiser la précision de la synchronisation :

- Priorisez votre thread d'enregistrement avec les paramètres de priorité système
- Fermez les applications inutiles pour libérer les ressources système
- Utilisez des SSD pour les sorties d'enregistrement afin de minimiser les goulots d'étranglement E/S
- Envisagez des cartes graphiques dédiées avec prise en charge de l'encodage matériel

## Conclusion

La synchronisation correcte de plusieurs sources de capture vidéo est essentielle pour créer des applications multi-caméras professionnelles. En utilisant le mécanisme de démarrage différé fourni par le Video Capture SDK, les développeurs peuvent obtenir des enregistrements hautement synchronisés avec un effort minimal.

Cette approche sépare la phase d'initialisation de la phase d'enregistrement, permettant à tous les périphériques d'être préparés avant que l'un d'eux ne commence l'enregistrement, ce qui se traduit par une synchronisation considérablement améliorée entre les sources.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
