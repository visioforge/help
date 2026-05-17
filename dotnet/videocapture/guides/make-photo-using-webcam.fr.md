---
title: Capture photo webcam en C# .NET — API d'image fixe
description: Prenez des photos depuis une webcam en C# / .NET. Capture d'image via bouton matériel ou logiciel. Enregistrement en JPEG, PNG, BMP. Exemple inclus.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Webcam
  - C#
primary_api_classes:
  - VideoCaptureCore
  - VideoFrameBitmapEventArgs
  - VideoFrameBufferEventArgs

---

# Capturer des photos via une webcam dans des applications .NET

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction à l'intégration webcam

Les applications modernes nécessitent de plus en plus l'intégration de webcams à diverses fins, des photos de profil utilisateur à la numérisation de documents. Implémenter une fonctionnalité efficace de capture photo via webcam nécessite de comprendre les mécanismes sous-jacents de l'interaction entre les webcams et le framework .NET.

Les webcams peuvent capturer des images via deux méthodes principales : les captures déclenchées par logiciel (où l'application initie le processus) et les captures déclenchées par matériel, où un bouton physique sur le périphérique webcam déclenche la capture d'image. Cette dernière méthode est appelée « capture d'image fixe » et offre une expérience utilisateur plus intuitive dans de nombreuses applications.

## Comprendre la capture d'image fixe

La capture d'image fixe est une fonction spécialisée disponible sur de nombreux modèles de webcam qui permet aux utilisateurs de capturer des images de haute qualité en appuyant sur un bouton dédié sur le périphérique. Cette approche offre plusieurs avantages :

- Expérience utilisateur plus intuitive
- Complexité d'application réduite
- Risque réduit de tremblement de caméra
- Souvent une meilleure qualité d'image que l'extraction d'images vidéo

Toutes les webcams ne prennent pas en charge la capture d'image fixe, il est donc important de vérifier les spécifications de votre périphérique ou de tester cette fonctionnalité avant de vous y fier dans votre application.

## Implémenter la capture photo webcam en .NET

Le Video Capture SDK pour .NET fournit un cadre robuste pour implémenter la capture photo webcam dans vos applications. Ci-dessous, nous couvrirons les étapes essentielles pour intégrer cette fonctionnalité.

### Configuration de votre projet

Avant de plonger dans les détails d'implémentation, assurez-vous que votre environnement de développement est correctement configuré :

1. Créez un nouveau projet d'application .NET
2. Ajoutez la référence au Video Capture SDK à votre projet
3. Importez les espaces de noms nécessaires :

```csharp
using System.Drawing;
using System.Drawing.Imaging;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.VideoCapture;
```

### Activer la capture d'image fixe

La première étape pour implémenter la capture d'image fixe consiste à configurer correctement votre application pour détecter et réagir aux pressions du bouton matériel de la webcam. Voici comment :

```csharp
// Initialiser le composant de capture vidéo. Passez un IVideoView pour l'aperçu,
// ou utilisez la surcharge sans paramètre CreateAsync() pour un fonctionnement sans interface.
var videoCapture = await VideoCaptureCore.CreateAsync(VideoView1);

// Activer la capture d'image fixe avant de démarrer le flux vidéo.
videoCapture.Video_Still_Frames_Grabber_Enabled = true;

// Sélectionner un périphérique de capture par nom. Énumérez les périphériques via videoCapture.Video_CaptureDevices().
videoCapture.Video_CaptureDevice = new VideoCaptureSource("USB Camera");
videoCapture.Video_CaptureDevice.Format_UseBest = true;     // ou définir explicitement Format + FrameRate
videoCapture.Audio_RecordAudio = false;
videoCapture.Audio_PlayAudio = false;

// Mode aperçu uniquement (sans capture de fichier).
videoCapture.Mode = VideoCaptureMode.VideoPreview;

// Démarrer le pipeline (asynchrone — OnError se déclenche sur les erreurs de pipeline).
await videoCapture.StartAsync();
```

Définir la propriété `Video_Still_Frames_Grabber_Enabled` sur `true` est crucial. Cette configuration indique au SDK de surveiller les pressions du bouton matériel et de déclencher les événements appropriés lorsqu'une image fixe est capturée.

### Gérer les images capturées

Une fois la capture d'image fixe activée, vous devez gérer les événements qui sont déclenchés lorsqu'une image est capturée. Le SDK fournit deux événements principaux à cette fin :

```csharp
// Pour gérer les images sous forme d'objets Bitmap
videoCapture.OnStillVideoFrameBitmap += VideoCapture_OnStillVideoFrameBitmap;

// Pour gérer les images sous forme de données de tampon brutes
videoCapture.OnStillVideoFrameBuffer += VideoCapture_OnStillVideoFrameBuffer;
```

Voici un exemple d'implémentation du gestionnaire d'événements pour les images bitmap (l'événement est `EventHandler<VideoFrameBitmapEventArgs>` ; le bitmap se trouve sur `e.Frame`) :

```csharp
private void VideoCapture_OnStillVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Cloner le bitmap car e.Frame appartient au SDK et est réutilisé après le retour de ce callback.
    Bitmap capturedImage = (Bitmap)e.Frame.Clone();

    // Marshaller sur le thread d'interface lors de l'exécution en WinForms / WPF.
    if (pictureBox1.InvokeRequired)
    {
        pictureBox1.BeginInvoke((Action)(() =>
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = capturedImage;
        }));
    }
    else
    {
        pictureBox1.Image?.Dispose();
        pictureBox1.Image = capturedImage;
    }
}
```

Pour un accès au tampon brut (sans allocation Bitmap, utile pour les pipelines d'image personnalisés), abonnez-vous à `OnStillVideoFrameBuffer`. Sa signature est `EventHandler<VideoFrameBufferEventArgs>` :

```csharp
private void VideoCapture_OnStillVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Métadonnées d'image brute : largeur / hauteur / stride / format de pixel.
    var width  = e.Frame.Info.Width;
    var height = e.Frame.Info.Height;

    // e.FrameArray est une copie byte[] managée (peut être null si le SDK a conservé les données en mémoire native).
    if (e.FrameArray != null)
    {
        File.WriteAllBytes($"frame-{e.Frame.Timestamp.Ticks}.raw", e.FrameArray);
    }

    // Mettez UpdateData = true si vous avez modifié le tampon et que vous voulez que le changement se propage en aval.
    // e.UpdateData = true;
}
```

### Enregistrer les images capturées

Après avoir capturé et potentiellement traité l'image, vous voudrez souvent l'enregistrer sur disque. Le SDK fournit une méthode pratique à cet effet :

```csharp
// Enregistrer l'image courante dans un fichier (API asynchrone — fonctionne après StartAsync).
await videoCapture.Frame_SaveAsync("capturedImage.jpg", ImageFormat.Jpeg);
```

Vous pouvez spécifier différents formats d'image en fonction des besoins de votre application, comme PNG pour une qualité sans perte ou JPEG pour des tailles de fichier plus petites.

### Obtenir l'image courante

Dans certains scénarios, vous pourriez vouloir capturer une image de manière programmatique sans dépendre du bouton matériel. Vous pouvez le faire à l'aide de la méthode `Frame_GetCurrent` :

```csharp
// Obtenir l'image courante sous forme de Bitmap
Bitmap currentFrame = videoCapture.Frame_GetCurrent();

// Traiter ou enregistrer l'image
if (currentFrame != null)
{
    // Utiliser l'image
    pictureBox1.Image = currentFrame;
    
    // Enregistrer si nécessaire
    currentFrame.Save("manualCapture.png", ImageFormat.Png);
}
```

## Considérations de performance

Les applications webcam peuvent être gourmandes en ressources, en particulier lors du traitement d'images haute résolution. Tenez compte de ces techniques d'optimisation :

1. Utilisez un traitement en arrière-plan pour les opérations d'enregistrement d'image
2. Implémentez une limitation de la fréquence d'images si une surveillance continue est nécessaire
3. Réduisez la résolution de l'aperçu tout en conservant une haute résolution pour les captures
4. Libérez correctement les ressources lorsque l'application se ferme :

   ```csharp
   protected override void OnFormClosing(FormClosingEventArgs e)
   {
       // Arrêter la capture et libérer les ressources
       videoCapture.Stop();
       videoCapture.Dispose();
       base.OnFormClosing(e);
   }
   ```

## Dépannage des problèmes courants

- **Caméra non détectée** : assurez-vous que la webcam est correctement connectée et que les pilotes sont installés
- **La capture d'image fixe ne fonctionne pas** : vérifiez que votre modèle de webcam prend en charge la capture par bouton matériel
- **Qualité d'image médiocre** : vérifiez les paramètres de résolution et assurez-vous des conditions d'éclairage appropriées
- **Plantages d'application** : implémentez une gestion correcte des erreurs et des ressources

## Conclusion

L'implémentation de la capture photo webcam dans les applications .NET fournit une fonctionnalité précieuse pour de nombreux scénarios. En suivant les recommandations de cet article, vous pouvez créer des applications robustes et conviviales qui tirent efficacement parti des capacités des webcams.

N'oubliez pas de tester votre implémentation sur différents modèles de webcam et configurations pour garantir une performance et une fiabilité cohérentes.

---
Pour davantage d'exemples de code et d'exemples d'implémentation, consultez notre dépôt [GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
