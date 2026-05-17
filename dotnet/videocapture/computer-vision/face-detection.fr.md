---
title: Détection de visages temps réel en C# .NET — vision SDK
description: Détectez des visages en temps réel dans des flux webcam et caméra IP avec le SDK VisioForge. Options de configuration, conseils d'optimisation et exemples C#.
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - Computer Vision
  - Webcam
  - IP Camera
  - C#
  - NuGet
primary_api_classes:
  - AFFaceDetectionEventArgs
  - FaceTrackingSettings

---

# Implémenter la détection de visages dans les applications vidéo .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la technologie de détection de visages

La détection de visages est une technologie de vision par ordinateur qui identifie et localise les visages humains dans des images numériques ou des trames vidéo. Contrairement à la reconnaissance faciale (qui identifie des individus spécifiques), la détection de visages répond simplement à la question : « Y a-t-il un visage dans cette image, et si oui, où se trouve-t-il ? »

Cette technologie sert de fondation à de nombreuses applications :

- Systèmes de sécurité et de surveillance
- Applications de photographie (autofocus, réduction des yeux rouges)
- Réseaux sociaux (suggestions d'étiquetage, filtres)
- Analyse des émotions et recherche sur l'expérience utilisateur
- Systèmes de suivi de présence
- Améliorations de la visioconférence

Pour les développeurs créant des applications .NET, l'implémentation de la détection de visages peut apporter une valeur significative aux applications de capture et de traitement vidéo. Ce guide fournit une procédure complète pour implémenter la détection de visages dans vos projets .NET.

## Prise en main de la détection de visages en .NET

### Prérequis

Avant d'implémenter la détection de visages dans votre application, assurez-vous d'avoir :

- Visual Studio (2019 ou plus récent recommandé)
- .NET Framework 4.6.2+ ou .NET Core 3.1+/.NET 5+
- Une compréhension de base du C# et de la programmation événementielle
- Le gestionnaire de paquets NuGet
- Les redistribuables requis (détaillés plus loin dans ce document)

### Vue d'ensemble de l'implémentation

Le processus d'implémentation suit ces étapes clés :

1. Configurer votre source vidéo
2. Définir les paramètres de suivi des visages
3. Créer et enregistrer des gestionnaires d'événements pour la détection de visages
4. Traiter les résultats de détection
5. Démarrer le flux vidéo

Détaillons chacune de ces étapes avec des exemples de code détaillés.

## Étape 1 : configurer votre source vidéo

Le suivi des visages s'exécute sur la source à laquelle l'instance `VideoCaptureCore` est liée. Choisissez l'une des configurations suivantes :

=== "Webcam"

    ```cs
    VideoCapture1.Mode = VideoCaptureMode.VideoPreview;
    VideoCapture1.Video_CaptureDevice = new VideoCaptureSource("USB Camera");
    VideoCapture1.Video_CaptureDevice.Format_UseBest = true;
    ```

=== "Caméra IP (RTSP/HTTP)"

    ```cs
    VideoCapture1.Mode = VideoCaptureMode.IPPreview;
    VideoCapture1.IP_Camera_Source = new IPCameraSourceSettings
    {
        URL = new Uri("rtsp://camera.local:554/stream"),
        Type = IPSourceEngine.Auto_VLC,
        Login = "admin",
        Password = "password"
    };
    ```

=== "Fichier vidéo (Media Player SDK)"

    Pour la détection de visages hors ligne sur fichier, utilisez `MediaPlayerCore` du Media Player SDK — la propriété `Face_Tracking` se comporte de la même manière entre les moteurs. Le reste de ce guide (étapes 2 à 5) reste applicable ; remplacez `VideoCapture1` par votre instance `MediaPlayer1`.

Choisissez un modèle et passez à l'étape 2 pour activer le suivi de visages sur cette source.

## Étape 2 : configurer les paramètres de suivi des visages

Une fois votre source vidéo configurée, l'étape suivante consiste à définir les paramètres de détection de visages. Ces paramètres déterminent comment le SDK identifie et suit les visages :

```cs
VideoCapture1.Face_Tracking = new FaceTrackingSettings
{
    // Le mode couleur détermine comment les couleurs sont traitées pour la détection
    ColorMode = CamshiftMode.RGB,
    
    // Mettre en évidence les visages détectés dans l'aperçu
    Highlight = true,
    
    // Taille minimale (en pixels) du visage à détecter
    MinimumWindowSize = 25,
    
    // Approche de balayage — comment l'algorithme parcourt les échelles de l'image
    ScalingMode = ObjectDetectorScalingMode.GreaterToSmaller,
    
    // Détection unique ou multiple de visages
    SearchMode = ObjectDetectorSearchMode.Single,

    // Facteur d'échelle utilisé pour parcourir la pyramide (1.0 < facteur <= 2.0)
    ScaleFactor = 1.2f
};
```

### Comprendre les paramètres de suivi des visages

- **ColorMode** (`CamshiftMode`) : détermine comment l'algorithme traite les couleurs pour la détection
  - RGB : traitement couleur RGB standard
  - HSL : espace colorimétrique teinte-saturation-luminosité, peut être plus robuste sous éclairage variable
  - Mixed : combine les canaux RGB et HSL
  
- **ScalingMode** : contrôle comment l'algorithme recherche à travers différentes échelles
  - GreaterToSmaller : commence par les visages potentiels les plus grands et descend
  - SmallerToGreater : commence par les visages potentiels les plus petits et monte
  
- **SearchMode** : détermine s'il faut rechercher un seul visage ou plusieurs
  - Single : optimisé pour trouver un seul visage (plus rapide)
  - Multiple : conçu pour trouver tous les visages dans l'image (plus intensif en traitement)

- **MinimumWindowSize** : taille de visage la plus petite (en pixels) qui sera détectée
  - Des valeurs plus petites attrapent des visages éloignés mais augmentent les faux positifs
  - Des valeurs plus grandes sont plus fiables mais peuvent manquer des visages plus petits/éloignés

## Étape 3 : configurer la gestion des événements de détection de visages

Pour réagir aux visages détectés, vous devez créer un gestionnaire d'événements et l'enregistrer auprès du SDK :

```cs
// OnFaceDetected est un EventHandler<AFFaceDetectionEventArgs> standard.
// e.FaceRectangles est un Rectangle[] — utilisez .Length, pas .Count.
private void OnFaceDetectedHandler(object sender, AFFaceDetectionEventArgs e)
{
    // Effacer le texte précédent
    edFaceTrackingFaces.Text = string.Empty;

    // Traiter chaque visage détecté
    foreach (var faceRectangle in e.FaceRectangles)
    {
        // Afficher les coordonnées et dimensions du visage
        edFaceTrackingFaces.Text +=
            $"Position: ({faceRectangle.Left}, {faceRectangle.Top}), " +
            $"Size: ({faceRectangle.Width}, {faceRectangle.Height}){Environment.NewLine}";

        // Vous pouvez aussi calculer le point central
        int centerX = faceRectangle.Left + (faceRectangle.Width / 2);
        int centerY = faceRectangle.Top + (faceRectangle.Height / 2);
        edFaceTrackingFaces.Text += $"Center: ({centerX}, {centerY}){Environment.NewLine}";

        // Optionnel : ajouter un horodatage pour le suivi
        edFaceTrackingFaces.Text += $"Time: {DateTime.Now:HH:mm:ss.fff}{Environment.NewLine}{Environment.NewLine}";
    }

    // Mettre à jour le nombre de visages (Rectangle[] — Length, pas Count)
    lblFaceCount.Text = $"Faces detected: {e.FaceRectangles.Length}";
}

// Enregistrer le gestionnaire d'événements
VideoCapture1.OnFaceDetected += OnFaceDetectedHandler;
```

Ce gestionnaire d'événements fournit des mises à jour en temps réel chaque fois que des visages sont détectés. Le gestionnaire reçoit les coordonnées des visages, que vous pouvez utiliser pour :

- Afficher des indicateurs visuels
- Suivre le mouvement des visages au fil du temps
- Déclencher des actions en fonction de la position du visage
- Journaliser les données de détection

## Étape 4 : traitement des résultats de détection

Avec le gestionnaire d'événements en place, vous pouvez traiter les résultats de détection. Les tâches de traitement courantes incluent :

### Visualiser les visages détectés

Au-delà de la mise en évidence intégrée, vous pourriez vouloir implémenter des visualisations personnalisées :

```cs
// Visualisation personnalisée — dessiner des rectangles de visage sur une superposition
private void DrawFacesOnOverlay(Rectangle[] faceRectangles, PictureBox overlay)
{
    // Créer un bitmap pour la superposition
    Bitmap overlayBitmap = new Bitmap(overlay.Width, overlay.Height);
    
    using (Graphics g = Graphics.FromImage(overlayBitmap))
    {
        g.Clear(Color.Transparent);
        
        // Dessiner chaque visage
        foreach (var face in faceRectangles)
        {
            // Dessiner le rectangle
            g.DrawRectangle(new Pen(Color.GreenYellow, 2), face);
            
            // Optionnel : dessiner un viseur au centre
            int centerX = face.Left + (face.Width / 2);
            int centerY = face.Top + (face.Height / 2);
            g.DrawLine(new Pen(Color.Red, 1), centerX - 10, centerY, centerX + 10, centerY);
            g.DrawLine(new Pen(Color.Red, 1), centerX, centerY - 10, centerX, centerY + 10);
        }
    }
    
    // Mettre à jour la superposition
    overlay.Image = overlayBitmap;
}
```

### Implémenter une logique de suivi des visages

Pour des applications plus avancées, vous pourriez vouloir suivre les visages dans le temps :

```cs
private Dictionary<int, TrackedFace> trackedFaces = new Dictionary<int, TrackedFace>();
private int nextFaceId = 1;

private void TrackFaces(List<Rectangle> currentFaces)
{
    // Faire correspondre les visages actuels avec les visages précédemment suivis
    List<int> matchedIds = new List<int>();
    List<Rectangle> unmatchedFaces = new List<Rectangle>(currentFaces);
    
    foreach (var trackedFace in trackedFaces.Values.ToList())
    {
        bool foundMatch = false;
        
        for (int i = unmatchedFaces.Count - 1; i >= 0; i--)
        {
            if (IsLikelyMatch(trackedFace.LastLocation, unmatchedFaces[i]))
            {
                // Mettre à jour le visage suivi existant
                trackedFace.UpdateLocation(unmatchedFaces[i]);
                matchedIds.Add(trackedFace.Id);
                unmatchedFaces.RemoveAt(i);
                foundMatch = true;
                break;
            }
        }
        
        // Supprimer les visages qui ont disparu
        if (!foundMatch)
        {
            trackedFaces.Remove(trackedFace.Id);
        }
    }
    
    // Ajouter les nouveaux visages
    foreach (var newFace in unmatchedFaces)
    {
        trackedFaces.Add(nextFaceId, new TrackedFace(nextFaceId, newFace));
        nextFaceId++;
    }
}

private bool IsLikelyMatch(Rectangle previous, Rectangle current)
{
    // Calculer les points centraux
    Point prevCenter = new Point(
        previous.Left + previous.Width / 2,
        previous.Top + previous.Height / 2);
    
    Point currCenter = new Point(
        current.Left + current.Width / 2,
        current.Top + current.Height / 2);
    
    // Calculer la distance entre les centres
    double distance = Math.Sqrt(
        Math.Pow(prevCenter.X - currCenter.X, 2) + 
        Math.Pow(prevCenter.Y - currCenter.Y, 2));
    
    // Si les centres sont suffisamment proches, considérer qu'il s'agit du même visage
    return distance < Math.Max(previous.Width, current.Width) * 0.5;
}

// Classe simple pour suivre les données de visage
private class TrackedFace
{
    public int Id { get; private set; }
    public Rectangle LastLocation { get; private set; }
    public DateTime FirstSeen { get; private set; }
    public DateTime LastSeen { get; private set; }
    
    public TrackedFace(int id, Rectangle location)
    {
        Id = id;
        LastLocation = location;
        FirstSeen = DateTime.Now;
        LastSeen = DateTime.Now;
    }
    
    public void UpdateLocation(Rectangle newLocation)
    {
        LastLocation = newLocation;
        LastSeen = DateTime.Now;
    }
}
```

## Étape 5 : démarrer le flux vidéo et la détection de visages

La dernière étape consiste à démarrer le flux vidéo et le processus de détection de visages :

```cs
// Démarrer la capture vidéo de manière asynchrone
await VideoCapture1.StartAsync();
```

Si vous devez arrêter le processus :

```cs
// Arrêter la capture vidéo
await VideoCapture1.StopAsync();
```

## Dépendances requises

Pour vous assurer que votre application fonctionne correctement, vous devrez inclure les paquets redistribuables appropriés :

- Redistribuables de capture vidéo :
  - [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

Installez ces paquets via NuGet :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

Ou pour les projets x86 :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
```

## Conclusion

L'implémentation de la détection de visages dans vos applications .NET améliore leurs capacités et ouvre de nombreuses possibilités d'interaction utilisateur, de fonctionnalités de sécurité et d'automatisation. En suivant ce guide, vous disposez maintenant des connaissances nécessaires pour intégrer une détection de visages robuste dans vos applications de capture vidéo.

Pour des ressources et des exemples de code supplémentaires, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
