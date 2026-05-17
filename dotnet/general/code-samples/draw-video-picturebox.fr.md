---
title: Rendre la vidéo dans un PictureBox WinForms — Guide C# .NET
description: Mises à jour thread-safe, libération du bitmap et double tampon pour éviter le scintillement. Saut d'images pour sources haut FPS. Zoom VisioForge SDK.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoFrameBitmapEventArgs

---

# Dessiner la vidéo sur un PictureBox dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au rendu vidéo dans WinForms

Afficher du contenu vidéo dans des applications de bureau est une exigence courante pour de nombreux développeurs travaillant avec le multimédia. Que vous construisiez des applications de vidéosurveillance, des lecteurs multimédias, des outils d'édition vidéo ou tout logiciel qui traite des flux vidéo, il est crucial de comprendre comment rendre efficacement la vidéo.

Le contrôle PictureBox est l'une des manières les plus simples d'afficher des images vidéo dans des applications Windows Forms. Bien qu'il n'ait pas été spécifiquement conçu pour la lecture vidéo, avec une implémentation correcte, il peut fournir un rendu vidéo fluide avec une consommation minimale de ressources.

Ce guide se concentre sur l'implémentation du rendu vidéo sur les contrôles PictureBox dans les applications .NET WinForms. Nous couvrirons l'ensemble du processus, de la configuration à l'implémentation, en abordant les pièges courants et les techniques d'optimisation.

## Pourquoi utiliser PictureBox pour l'affichage vidéo ?

Avant de plonger dans les détails d'implémentation, examinons les avantages de l'utilisation de PictureBox pour l'affichage vidéo :

- **Simplicité** : PictureBox est un contrôle simple que la plupart des développeurs .NET connaissent déjà.
- **Flexibilité** : il permet de personnaliser l'affichage des images via sa propriété SizeMode.
- **Intégration** : il s'intègre parfaitement avec d'autres contrôles WinForms.
- **Faible surcoût** : pour de nombreuses applications, il fournit des performances suffisantes sans nécessiter d'implémentations DirectX ou OpenGL plus complexes.

Cependant, il est important de noter que PictureBox n'a pas été conçu spécifiquement pour la lecture vidéo haute performance. Pour les applications nécessitant des performances vidéo de qualité professionnelle ou une accélération matérielle, des approches de rendu plus spécialisées peuvent être nécessaires.

## Prérequis

Pour implémenter le rendu vidéo sur un PictureBox, vous aurez besoin de :

- Une connaissance de base du développement C# et .NET WinForms
- Visual Studio ou un autre IDE pour le développement .NET
- Une source vidéo (depuis Video Capture SDK, Video Edit SDK ou Media Player SDK)
- Une compréhension de la programmation événementielle

## Configuration de votre environnement

### Configuration du contrôle PictureBox

1. Ajoutez un contrôle PictureBox à votre formulaire via le concepteur ou par programmation.
2. Configurez les propriétés de base pour un affichage vidéo optimal :

```cs
// Configurer PictureBox pour l'affichage vidéo
pictureBox1.BackColor = Color.Black;
pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
```

La propriété `BackColor` définie sur `Black` fournit un arrière-plan propre pour l'affichage vidéo, en particulier pendant l'initialisation ou quand la vidéo présente des bordures noires. La propriété `SizeMode` détermine comment l'image vidéo s'ajuste dans le contrôle :

- `StretchImage` : étire l'image pour remplir le PictureBox (peut déformer le rapport d'aspect)
- `Zoom` : maintient le rapport d'aspect tout en remplissant le contrôle
- `CenterImage` : centre l'image sans mise à l'échelle
- `Normal` : affiche l'image à sa taille d'origine

Pour la plupart des applications vidéo, `StretchImage` ou `Zoom` fonctionnent le mieux, selon que le maintien du rapport d'aspect est important ou non.

## Étapes d'implémentation

### Étape 1 : préparer votre classe avec les variables requises

Ajoutez un membre booléen à votre classe pour suivre quand une image est en cours d'application au PictureBox. Cela évite les conditions de concurrence quand plusieurs images arrivent en succession rapide :

```cs
private bool applyingPictureBoxImage = false;
```

### Étape 2 : initialiser les paramètres vidéo dans le gestionnaire de démarrage

Au démarrage de votre capture ou lecture vidéo, assurez-vous que l'indicateur est correctement initialisé :

```cs
private async void btnStart_Click(object sender, EventArgs e)
{
    // Réinitialiser l'indicateur avant de démarrer la capture/lecture.
    applyingPictureBoxImage = false;

    // S'abonner à OnVideoFrameBitmap avant le démarrage pour ne pas manquer la première image.
    // L'événement est EventHandler<VideoFrameBitmapEventArgs>, déclenché depuis un thread worker.
    videoCapture1.OnVideoFrameBitmap += VideoCapture1_OnVideoFrameBitmap;

    // Exemple Video Capture SDK — remplacez par MediaPlayer1 / VideoEdit1 si vous utilisez ces moteurs.
    videoCapture1.Video_CaptureDevice = new VideoCaptureSource("USB Camera");
    videoCapture1.Mode = VideoCaptureMode.VideoPreview;
    videoCapture1.Audio_RecordAudio = false;

    await videoCapture1.StartAsync();
}
```

Le même événement `OnVideoFrameBitmap` existe sur les trois moteurs — `VideoCaptureCore`, `MediaPlayerCore` et `VideoEditCore` — donc le gestionnaire ci-dessous fonctionne sans modification quel que soit le SDK qui l'a déclenché.

### Étape 3 : implémenter le gestionnaire d'images

Le cœur du rendu vidéo est le gestionnaire d'images. Cet événement se déclenche chaque fois qu'une nouvelle image vidéo est disponible. Voici comment l'implémenter efficacement :

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Empêcher les mises à jour concurrentes qui pourraient causer des problèmes de threads
    if (applyingPictureBoxImage)
    {
        return;
    }

    applyingPictureBoxImage = true;

    try
    {
        // Stocker l'image actuelle pour une libération correcte
        var currentImage = pictureBox1.Image;
        
        // Créer un nouveau bitmap à partir de l'image
        pictureBox1.Image = new Bitmap(e.Frame);

        // Libérer correctement l'image précédente pour éviter les fuites de mémoire
        currentImage?.Dispose();
    }
    catch (Exception ex)
    {
        // Envisagez de journaliser l'exception
        Console.WriteLine($"Error updating frame: {ex.Message}");
    }
    finally
    {
        // S'assurer que l'indicateur est réinitialisé même si une exception se produit
        applyingPictureBoxImage = false;
    }
}
```

Cette implémentation inclut plusieurs concepts importants :

1. **Sécurité des threads** : l'utilisation de l'indicateur `applyingPictureBoxImage` empêche les mises à jour concurrentes.
2. **Gestion de la mémoire** : la libération correcte de l'image précédente évite les fuites de mémoire.
3. **Gestion des exceptions** : capturer les exceptions évite les plantages d'application pendant le rendu.

### Étape 4 : implémenter le nettoyage lors de l'arrêt de la vidéo

Lors de l'arrêt de la capture ou de la lecture vidéo, vous devez nettoyer correctement les ressources :

```cs
private void btnStop_Click(object sender, EventArgs e)
{
    // Votre code d'arrêt vidéo ici
    // videoCapture1.Stop(); ou appel SDK similaire
    
    // Attendre la fin de toute mise à jour d'image en cours
    while (applyingPictureBoxImage)
    {
        Thread.Sleep(50);
    }

    // Nettoyer les ressources
    if (pictureBox1.Image != null)
    {
        pictureBox1.Image.Dispose();
        pictureBox1.Image = null;
    }
}
```

Ce processus de nettoyage :

1. Attend la fin de toute mise à jour d'image en cours
2. Libère correctement l'image
3. Définit l'image du PictureBox sur null pour le nettoyage visuel

## Considérations d'implémentation avancée

### Gestion des fréquences d'images élevées

Pour les sources vidéo à haute fréquence d'images, vous voudrez peut-être implémenter un saut d'images pour maintenir la réactivité de l'application :

```cs
private DateTime lastFrameTime = DateTime.MinValue;
private TimeSpan frameInterval = TimeSpan.FromMilliseconds(33); // Environ 30 fps

private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Sauter les images si elles arrivent trop rapidement
    if (DateTime.Now - lastFrameTime < frameInterval)
    {
        return;
    }
    
    if (applyingPictureBoxImage)
    {
        return;
    }

    applyingPictureBoxImage = true;
    lastFrameTime = DateTime.Now;

    // Code de traitement de l'image comme précédemment...
}
```

### Invocation inter-threads

Lors de la gestion d'images vidéo depuis des threads d'arrière-plan, vous devrez utiliser l'invocation inter-threads :

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    if (applyingPictureBoxImage)
    {
        return;
    }

    applyingPictureBoxImage = true;

    if (pictureBox1.InvokeRequired)
    {
        pictureBox1.BeginInvoke(new Action(() => {
            var currentImage = pictureBox1.Image;
            pictureBox1.Image = new Bitmap(e.Frame);
            currentImage?.Dispose();
            applyingPictureBoxImage = false;
        }));
    }
    else
    {
        // Code de mise à jour directe comme précédemment...
    }
}
```

## Conseils d'optimisation des performances

### Réduire le surcoût de création de Bitmap

Créer un nouveau Bitmap pour chaque image peut être coûteux. Envisagez de réutiliser les objets Bitmap :

```cs
private Bitmap displayBitmap;

private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    if (applyingPictureBoxImage)
    {
        return;
    }

    applyingPictureBoxImage = true;

    try
    {
        // Initialiser le bitmap si nécessaire
        if (displayBitmap == null || 
            displayBitmap.Width != e.Frame.Width || 
            displayBitmap.Height != e.Frame.Height)
        {
            displayBitmap?.Dispose();
            displayBitmap = new Bitmap(e.Frame.Width, e.Frame.Height);
        }
        
        // Copier l'image dans le bitmap d'affichage
        using (Graphics g = Graphics.FromImage(displayBitmap))
        {
            g.DrawImage(e.Frame, 0, 0, e.Frame.Width, e.Frame.Height);
        }
        
        // Mettre à jour l'affichage
        var oldImage = pictureBox1.Image;
        pictureBox1.Image = displayBitmap;
        oldImage?.Dispose();
    }
    finally
    {
        applyingPictureBoxImage = false;
    }
}
```

### Envisager d'utiliser le double tampon

Pour un affichage plus fluide, activez le double tampon sur votre formulaire :

```cs
// Dans le constructeur de votre formulaire
this.DoubleBuffered = true;
```

## Dépannage des problèmes courants

### Fuites de mémoire

Si votre application présente une utilisation croissante de la mémoire, vérifiez :

- La libération correcte des anciens objets Bitmap
- Les références aux images qui pourraient empêcher le ramasse-miettes
- Si les images sont sautées lorsque c'est nécessaire

### Affichage qui scintille

Si l'affichage vidéo scintille :

- Assurez-vous que le double tampon est activé
- Vérifiez si plusieurs threads mettent à jour le PictureBox simultanément
- Envisagez d'implémenter un mécanisme de synchronisation des images plus sophistiqué

### Utilisation élevée du CPU

Si le rendu cause une utilisation élevée du CPU :

- Implémentez le saut d'images comme indiqué ci-dessus
- Envisagez de réduire la fréquence d'images de la source si possible
- Optimisez la gestion des bitmaps pour réduire la pression sur le GC

## Dépendances requises

Pour implémenter cette solution, vous aurez besoin de :

- .NET Framework ou .NET Core/5+
- Fichiers redistribuables du SDK pour le SDK vidéo spécifique que vous utilisez

## Conclusion

L'implémentation du rendu vidéo sur un contrôle PictureBox fournit un moyen simple d'afficher la vidéo dans les applications Windows Forms. En suivant les modèles décrits dans ce guide, vous pouvez obtenir un affichage vidéo fluide tout en évitant les pièges courants comme les fuites de mémoire, les problèmes de sécurité des threads et les goulots d'étranglement de performance.

N'oubliez pas que, bien que PictureBox convienne à de nombreuses applications, les applications vidéo hautes performances peuvent bénéficier d'approches de rendu plus spécialisées utilisant DirectX ou OpenGL.

---
Pour plus d'exemples de code, visitez notre dépôt [GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
