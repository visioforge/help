---
title: Vidéo à partir d'images en console C# .NET — Video Edit SDK
description: Générez des fichiers vidéo à partir de séquences d'images avec VisioForge Video Edit SDK .NET. Fréquence d'images, résolution et encodage en C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - AVI
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCore
  - VideoRendererSettings
  - VideoRenderer
  - AVIOutput
  - ProgressEventArgs

---

# Créer des vidéos à partir d'images dans des applications console C#

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

Convertir une séquence d'images en fichier vidéo est une exigence courante pour de nombreuses applications logicielles. Ce guide montre comment créer une vidéo à partir d'images à l'aide d'une application console C# avec Video Edit SDK .Net. La même approche fonctionne pour les applications WinForms et WPF avec des modifications minimales.

## Prérequis

Avant de commencer, assurez-vous d'avoir :

- Un environnement de développement .NET configuré
- Video Edit SDK .Net installé
- Une connaissance de base de la programmation C#
- Un dossier contenant des fichiers image (JPG, PNG, etc.)

## Concepts clés

Lors de la création de vidéos à partir d'images, la compréhension de ces concepts fondamentaux vous aidera à obtenir de meilleurs résultats :

- **Fréquence d'images** : détermine la fluidité de lecture de votre vidéo (généralement 25-30 images par seconde)
- **Durée d'image** : la durée pendant laquelle chaque image apparaît dans la vidéo
- **Effets de transition** : effets facultatifs entre les images
- **Format de sortie** : les spécifications du conteneur vidéo et du codec
- **Résolution** : les dimensions de la vidéo de sortie

## Implémentation étape par étape

### Configuration du projet

Tout d'abord, créez un nouveau projet d'application console et ajoutez les références nécessaires :

```cs
using System;
using System.IO;
using VisioForge.Core.Types;        // espaces de noms actuels (migration v15→v2026)
using VisioForge.Core.Types.Output;
using VisioForge.Core.VideoEdit;
```

### Implémentation principale

```cs
namespace ve_console
{
    class Program
    {
        // Le dossier contient des images
        private const string AssetDir = "c:\\samples\\pics\\";

        static void Main(string[] args)
        {
            if (!Directory.Exists(AssetDir))
            {
                Console.WriteLine(@"Folder with images does not exists: " + AssetDir);
                return;
            }

            var images = Directory.GetFiles(AssetDir, "*.jpg");
            if (images.Length == 0)
            {
                Console.WriteLine(@"Folder with images is empty or do not have files with .jpg extension: " + AssetDir);
                return;
            }

            if (File.Exists(AssetDir + "output.avi"))
            {
                File.Delete(AssetDir + "output.avi");
            }

            var ve = new VideoEditCore();

            int insertTime = 0;

            foreach (string img in images)
            {
                ve.Input_AddImageFile(img, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(insertTime), VideoEditStretchMode.Letterbox, 0, 640, 480);
                insertTime += 2000;
            }

            ve.Video_Effects_Clear();
            ve.Mode = VideoEditMode.Convert;

            ve.Video_Resize = true;
            ve.Video_Resize_Width = 640;
            ve.Video_Resize_Height = 480;

            ve.Video_FrameRate = new VideoFrameRate(25);
            ve.Video_Renderer = new VideoRendererSettings
            {
                VideoRenderer = VideoRendererMode.None,
                StretchMode = VideoRendererStretchMode.Letterbox
            };

            var aviOutput = new AVIOutput
            {
                Video_Codec = "MJPEG Compressor"
            };

            ve.Output_Format = aviOutput;
            ve.Output_Filename = AssetDir + "output.avi";

            ve.Video_Effects_Enabled = true;
            ve.Video_Effects_Clear();

            ve.OnError += VideoEdit1_OnError;
            ve.OnProgress += VideoEdit1_OnProgress;

            ve.ConsoleUsage = true;

            ve.Start();

            Console.WriteLine(@"Video saved to: " + ve.Output_Filename);
        }

        private static void VideoEdit1_OnProgress(object sender, ProgressEventArgs progressEventArgs)
        {
            Console.WriteLine(progressEventArgs.Progress);
        }

        private static void VideoEdit1_OnError(object sender, ErrorsEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
```

## Détail des composants

### Configuration des entrées image

Le code ci-dessus utilise la surcharge à sept arguments `Input_AddImageFile` sur `VideoEditCore` :

```csharp
public bool Input_AddImageFile(
    string filename,                           // chemin du fichier image
    TimeSpan duration,                         // durée d'affichage de l'image à l'écran
    TimeSpan? timelineInsertTime,              // position d'insertion sur la timeline
    VideoEditStretchMode stretchMode,          // comment l'image s'ajuste au cadre
    int targetVideoStream,                     // index du flux vidéo (0 = principal)
    int customWidth,                           // largeur de sortie en pixels
    int customHeight);                         // hauteur de sortie en pixels
```

Notes sur les paramètres :

- **filename** — chemin du fichier image (JPG/PNG/BMP/TIF)
- **duration** — durée d'affichage de l'image (2000 ms dans cet exemple)
- **timelineInsertTime** — moment où l'image apparaît dans la timeline finale
- **stretchMode** — comportement d'ajustement (`Letterbox` préserve le rapport d'aspect avec des barres noires ; `Stretch` remplit le cadre ; `Crop` recadre au centre pour remplir)
- **targetVideoStream** — index du flux vidéo (passez `0` sauf si vous construisez un montage multipiste)
- **customWidth / customHeight** — dimensions cibles, correspondant typiquement à `Video_Resize_Width` / `Video_Resize_Height` définis sur le moteur

### Paramètres de sortie vidéo

Les paramètres de sortie vidéo sont configurés avec ces propriétés clés :

- **Video_Resize** : activer/désactiver le redimensionnement
- **Video_Resize_Width/Height** : dimensions de la vidéo de sortie
- **Video_FrameRate** : images par seconde (25 ips est le standard PAL)
- **Video_Renderer** : paramètres de rendu incluant le mode et l'étirement
- **Output_Format** : paramètres du format de conteneur et du codec
- **Output_Filename** : emplacement de sauvegarde du fichier vidéo résultant

### Gestion de la progression et des erreurs

L'implémentation inclut des gestionnaires d'événements pour surveiller la progression et capturer les erreurs :

```cs
ve.OnError += VideoEdit1_OnError;
ve.OnProgress += VideoEdit1_OnProgress;
```

Ces gestionnaires fournissent un retour pendant la création vidéo, ce qui est essentiel pour les opérations longues.

## Options de personnalisation avancées

### Effets de transition

Pour ajouter des transitions entre les images, vous pouvez utiliser la méthode `Video_Transition_Add`.
Le moteur classique `VideoEditCore` est livré avec les noms DXTransform — les valeurs courantes incluent
`"Fade"`, `"Horizontal"`, `"Upper right"`, `"Radial, top"`, `"Wheel, 4 spoke"`,
`"Pixelate"`, `"Page peel"`, etc. (appelez `Video_Transitions_GetList()` pour énumérer
la liste complète à l'exécution). Les noms comme `"FadeIn"` / `"FadeOut"` sont des valeurs du moteur X
et retourneraient 0 (introuvable) sur le moteur classique.

Pour le fondu d'entrée / fondu de sortie spécifiquement, le moteur classique expose également des assistants dédiés
`Video_Transition_Add_FadeIn` / `Video_Transition_Add_FadeOut` qui prennent directement
les temps de début/fin et une couleur de fondu (pas de recherche par nom nécessaire).

```cs
// Exemple A — recherche basée sur le nom (nom DXTransform)
int transitionId = ve.Video_Transition_GetIDFromName("Fade");

// Ajouter la transition - les paramètres sont l'heure de début, l'heure de fin et l'ID de transition
ve.Video_Transition_Add(
    TimeSpan.FromMilliseconds(1900),  // Heure de début de la transition
    TimeSpan.FromMilliseconds(2100),  // Heure de fin de la transition
    transitionId                      // ID de transition
);

// Exemple B — assistant FadeIn dédié (pas de recherche par nom)
// ve.Video_Transition_Add_FadeIn(
//     TimeSpan.FromMilliseconds(1900),
//     TimeSpan.FromMilliseconds(2100),
//     System.Drawing.Color.Black);

// Pour des options de transition plus avancées avec bordure et autres propriétés :
// ve.Video_Transition_Add(
//     TimeSpan.FromMilliseconds(1900),  // Heure de début
//     TimeSpan.FromMilliseconds(2100),  // Heure de fin
//     transitionId,                     // ID de transition
//     Color.Blue,                       // Couleur de bordure
//     5,                                // Adoucissement de bordure
//     2,                                // Largeur de bordure
//     0,                                // Décalage X
//     0,                                // Décalage Y
//     0,                                // Réplique X
//     0,                                // Réplique Y
//     1,                                // Échelle X
//     1                                 // Échelle Y
// );
```

## Conseils d'optimisation des performances

- **Pré-redimensionnez les images** : pour de meilleures performances, redimensionnez les images avant le traitement
- **Traitement par lots** : traitez les images par petits lots pour les grandes collections
- **Gestion de la mémoire** : libérez les grands objets lorsqu'ils ne sont plus nécessaires
- **Codec de sortie** : choisissez les codecs en fonction des exigences de qualité par rapport à la vitesse de traitement
- **Accélération matérielle** : activez l'accélération matérielle lorsqu'elle est disponible

## Résolution des problèmes courants

### Erreurs de codec manquant

Si vous rencontrez des erreurs liées aux codecs, assurez-vous d'avoir installé les redistribuables requis :

- Redistribuables Video Edit SDK [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

### Compatibilité des formats d'image

Tous les formats d'image ne sont pas pris en charge de la même manière. Pour de meilleurs résultats :

- Utilisez des formats courants comme JPG, PNG ou BMP
- Assurez-vous d'avoir des dimensions cohérentes entre les images
- Testez avec un petit sous-ensemble avant de traiter de grandes collections

## Conclusion

Créer des vidéos à partir d'images dans une application console C# est simple avec la bonne approche. Ce guide a couvert les détails essentiels d'implémentation, les options de configuration et les bonnes pratiques pour vous aider à intégrer avec succès cette fonctionnalité dans vos applications.

N'oubliez pas d'ajuster les paramètres pour qu'ils correspondent à vos exigences spécifiques, notamment la durée d'image, la fréquence d'images et les paramètres de format de sortie.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour davantage d'exemples de code et d'implémentations.
