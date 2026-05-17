---
title: Définir une image personnalisée pour VideoView en .NET
description: Affichez des images personnalisées dans les contrôles VideoView lorsqu'aucune vidéo n'est en lecture pour un branding professionnel et une UX améliorée en .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - Playback
  - C#
primary_api_classes:
  - VideoView
  - VideoPlayerForm

---

# Définir des images personnalisées pour les contrôles VideoView dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Lors du développement d'applications multimédias en .NET, il est souvent nécessaire d'afficher une image personnalisée dans votre contrôle VideoView lorsqu'aucun contenu vidéo n'est en lecture. Cette capacité est essentielle pour créer des applications d'apparence professionnelle qui maintiennent un attrait visuel pendant les états inactifs. Les images personnalisées peuvent servir d'espaces réservés, d'opportunités de branding ou d'affichages informatifs pour améliorer l'expérience utilisateur.

Ce guide explore l'implémentation de la fonctionnalité d'image personnalisée pour les contrôles VideoView dans diverses applications SDK .NET.

## Comprendre les images personnalisées VideoView

Le contrôle VideoView est un composant polyvalent qui affiche le contenu vidéo dans votre application. Cependant, lorsque le contrôle ne lit pas activement de vidéo, il affiche généralement un écran vide ou par défaut. En implémentant des images personnalisées, vous pouvez :

- Afficher le logo de votre application ou de votre entreprise
- Afficher des miniatures d'aperçu du contenu disponible
- Présenter des informations d'instruction aux utilisateurs
- Maintenir une cohérence visuelle dans votre application
- Indiquer l'état de la vidéo (en pause, arrêté, en chargement, etc.)

Il est important de noter que l'image personnalisée n'est visible que lorsque le contrôle ne lit aucun contenu vidéo. Une fois la lecture commencée, le flux vidéo remplace automatiquement l'image personnalisée.

## Processus d'implémentation

Le processus de définition d'une image personnalisée pour un contrôle VideoView implique trois opérations principales :

1. Créer une PictureBox avec les dimensions appropriées
2. Définir l'image souhaitée
3. Nettoyer les ressources lorsqu'elles ne sont plus nécessaires

Explorons chacune de ces étapes en détail.

## Étape 1 : créer la PictureBox

La première étape consiste à initialiser une PictureBox à l'intérieur de votre contrôle VideoView avec les dimensions appropriées. Cette opération doit être effectuée une seule fois lors de la phase de configuration :

```csharp
VideoView1.PictureBoxCreate(VideoView1.Width, VideoView1.Height);
```

Cet appel de méthode crée un composant PictureBox interne qui hébergera votre image personnalisée. Les paramètres spécifient la largeur et la hauteur de la PictureBox, qui doivent généralement correspondre aux dimensions de votre contrôle VideoView pour garantir un affichage correct sans étirement ni distorsion.

### Bonnes pratiques pour la création de la PictureBox

- **Considérations de minutage** : créez la PictureBox lors de l'initialisation du formulaire ou après que le contrôle ait été dimensionné de manière appropriée
- **Dimensionnement dynamique** : si votre application prend en charge le redimensionnement, envisagez de recréer la PictureBox lorsque la taille du contrôle change
- **Gestion des erreurs** : implémentez des blocs try-catch pour gérer les exceptions potentielles lors de la création

## Étape 2 : définir l'image personnalisée

Après avoir créé la PictureBox, vous pouvez définir votre image personnalisée. Notez qu'il semble y avoir une duplication dans la documentation d'origine — le code correct pour définir l'image doit utiliser la méthode `PictureBoxSetImage` :

`PictureBoxSetImage` prend un `System.Drawing.Bitmap`, donc chargez le fichier comme
`Bitmap` (ou effectuez un cast) plutôt que `Image` :

```csharp
// Charger une image depuis un fichier en tant que Bitmap
Bitmap customImage = new Bitmap("path/to/your/image.jpg");
VideoView1.PictureBoxSetImage(customImage);
```

Alternativement, vous pouvez utiliser des ressources intégrées ou des images générées dynamiquement :

```csharp
// Utilisation d'une ressource Bitmap (la ressource doit être déclarée comme Bitmap, pas Image)
VideoView1.PictureBoxSetImage(Properties.Resources.MyCustomImage);

// Ou créer une image dynamique
using (Bitmap dynamicImage = new Bitmap(VideoView1.Width, VideoView1.Height))
{
    using (Graphics g = Graphics.FromImage(dynamicImage))
    {
        // Dessiner sur l'image
        g.Clear(Color.DarkBlue);
        g.DrawString("Ready to Play", new Font("Arial", 24), Brushes.White, new PointF(50, 50));
    }

    VideoView1.PictureBoxSetImage((Bitmap)dynamicImage.Clone());
}
```

### Considérations sur le format d'image

Le format d'image que vous choisissez peut impacter les performances et la qualité visuelle :

- **PNG** : idéal pour les images avec transparence
- **JPEG** : adapté au contenu photographique
- **BMP** : format non compressé avec une utilisation mémoire plus élevée
- **GIF** : prend en charge des animations simples mais avec une profondeur de couleur limitée

### Optimisation de la taille d'image

Pour des performances optimales, prenez en compte ces facteurs lors de la préparation de vos images personnalisées :

1. **Adapter les dimensions** : redimensionnez votre image pour qu'elle corresponde aux dimensions de VideoView afin d'éviter les opérations de mise à l'échelle
2. **Conscience de la résolution** : tenez compte du DPI d'affichage pour des images nettes sur les écrans haute résolution
3. **Consommation mémoire** : les grandes images consomment plus de mémoire, ce qui peut impacter les performances de l'application

## Étape 3 : nettoyer les ressources

Lorsque l'image personnalisée n'est plus nécessaire, il est important de nettoyer les ressources pour éviter les fuites de mémoire :

```csharp
VideoView1.PictureBoxDestroy();
```

Cette méthode doit être appelée quand :

- L'application est en cours de fermeture
- Le contrôle est en cours de libération
- Vous passez en mode de lecture vidéo et n'aurez plus besoin de l'image personnalisée

### Bonnes pratiques de gestion des ressources

Une gestion correcte des ressources est cruciale pour maintenir la stabilité de l'application :

- **Nettoyage explicite** : appelez toujours `PictureBoxDestroy()` lorsque vous avez terminé avec l'image personnalisée
- **Minutage de la libération** : incluez l'appel de nettoyage dans les événements `Dispose` ou `Closing` de votre formulaire
- **Suivi de l'état** : gardez une trace de la création ou non d'une PictureBox pour éviter de détruire une ressource inexistante

## Scénarios avancés

### Mises à jour dynamiques d'image

Dans certaines applications, vous pourriez avoir besoin de mettre à jour l'image personnalisée dynamiquement :

```csharp
private void UpdateCustomImage(string imagePath)
{
    // S'assurer que la PictureBox existe
    if (VideoView1.PictureBoxExists())
    {
        // Mettre à jour l'image
        Bitmap newImage = new Bitmap(imagePath);
        VideoView1.PictureBoxSetImage(newImage);
    }
    else
    {
        // Créer la PictureBox d'abord
        VideoView1.PictureBoxCreate(VideoView1.Width, VideoView1.Height);
        Bitmap newImage = new Bitmap(imagePath);
        VideoView1.PictureBoxSetImage(newImage);
    }
}
```

### Gérer le redimensionnement du contrôle

Si votre application permet de redimensionner le contrôle VideoView, vous devrez gérer la mise à l'échelle de l'image :

```csharp
private void VideoView1_SizeChanged(object sender, EventArgs e)
{
    // Recréer la PictureBox avec les nouvelles dimensions
    if (VideoView1.PictureBoxExists())
    {
        VideoView1.PictureBoxDestroy();
    }
    
    VideoView1.PictureBoxCreate(VideoView1.Width, VideoView1.Height);
    
    // Définir à nouveau l'image avec une mise à l'échelle appropriée
    SetScaledCustomImage();
}
```

### Plusieurs contrôles VideoView

Lorsque vous travaillez avec plusieurs contrôles VideoView, assurez une gestion appropriée pour chacun :

```csharp
private void InitializeAllVideoViews()
{
    // Initialiser chaque VideoView avec les images personnalisées appropriées
    VideoView1.PictureBoxCreate(VideoView1.Width, VideoView1.Height);
    VideoView1.PictureBoxSetImage(Properties.Resources.Camera1Placeholder);
    
    VideoView2.PictureBoxCreate(VideoView2.Width, VideoView2.Height);
    VideoView2.PictureBoxSetImage(Properties.Resources.Camera2Placeholder);
    
    // Contrôles VideoView supplémentaires...
}
```

## Dépannage des problèmes courants

### L'image ne s'affiche pas

Si votre image personnalisée n'apparaît pas :

1. **Vérifier le minutage** : assurez-vous de définir l'image après la création de la PictureBox
2. **Vérifier l'état de la vidéo** : confirmez que le contrôle ne lit pas actuellement de vidéo
3. **Chargement de l'image** : vérifiez que le chemin de l'image est correct et accessible
4. **Visibilité du contrôle** : assurez-vous que le contrôle VideoView est visible dans l'UI

### Fuites de mémoire

Pour éviter les fuites de mémoire :

1. **Libérer les images** : libérez toujours les objets Image lorsqu'ils ne sont plus nécessaires
2. **Détruire la PictureBox** : appelez `PictureBoxDestroy()` lorsque c'est approprié
3. **Suivi des ressources** : implémentez un suivi approprié des ressources créées

## Exemple d'implémentation complète

Voici un exemple d'implémentation complet qui démontre la gestion correcte du cycle de vie :

```csharp
public partial class VideoPlayerForm : Form
{
    private bool isPictureBoxCreated = false;
    
    public VideoPlayerForm()
    {
        InitializeComponent();
        this.Load += VideoPlayerForm_Load;
        this.FormClosing += VideoPlayerForm_FormClosing;
    }
    
    private void VideoPlayerForm_Load(object sender, EventArgs e)
    {
        InitializeCustomImage();
    }
    
    private void InitializeCustomImage()
    {
        try
        {
            VideoView1.PictureBoxCreate(VideoView1.Width, VideoView1.Height);
            isPictureBoxCreated = true;
            
            // PictureBoxSetImage attend un System.Drawing.Bitmap — déclarez la ressource comme Bitmap (ou effectuez un cast depuis Image).
            using (Bitmap customImage = (Bitmap)Properties.Resources.VideoPlaceholder)
            {
                VideoView1.PictureBoxSetImage(customImage);
            }
        }
        catch (Exception ex)
        {
            // Gérer les exceptions
            MessageBox.Show($"Error setting custom image: {ex.Message}");
        }
    }
    
    private void btnPlay_Click(object sender, EventArgs e)
    {
        // Logique de lecture vidéo ici
        // L'image personnalisée sera automatiquement remplacée pendant la lecture
    }
    
    private void VideoPlayerForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        CleanupResources();
    }
    
    private void CleanupResources()
    {
        if (isPictureBoxCreated)
        {
            VideoView1.PictureBoxDestroy();
            isPictureBoxCreated = false;
        }
    }
}
```

## Conclusion

Implémenter des images personnalisées pour les contrôles VideoView améliore l'expérience utilisateur et l'apparence professionnelle de vos applications multimédias .NET. En suivant les étapes décrites dans ce guide, vous pouvez afficher efficacement du contenu de marque ou informatif lorsque les vidéos ne sont pas en lecture.

Souvenez-vous des points clés :

1. Créez la PictureBox avec les dimensions appropriées
2. Définissez votre image personnalisée avec une gestion appropriée des ressources
3. Nettoyez les ressources lorsqu'elles ne sont plus nécessaires
4. Gérez le redimensionnement et autres scénarios spéciaux selon les besoins

Avec ces techniques, vous pouvez créer des applications vidéo plus soignées et conviviales en .NET.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code et d'implémentation.
