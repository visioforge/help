---
title: Créer une vidéo à partir d'images avec transitions en C#
description: Transformez un dossier d'images JPEG en MP4 1080p avec fondu enchaîné, volets et fondu d'ouverture et de fermeture, avec Video Edit SDK .NET.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoTransition
  - VideoTransitionType
  - MP4Output
  - StopEventArgs
video:
  id: Kef_q2cvdAI
  upload_date: 2026-11-14T07:54:56Z
  duration: PT9M17S

---

# Créer une vidéo à partir d'images avec transitions en C\#

[Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce tutoriel crée une application console C# qui transforme un dossier d'images JPEG en une seule vidéo MP4 avec [Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net) et son moteur `VideoEditCoreX`. Chaque image reste 3 secondes à l'écran, et la suivante commence 1 seconde avant sa fin ; pendant cette seconde, une transition passe d'une image à l'autre : d'abord un fondu enchaîné, puis un volet en barre, un iris, un volet en horloge et un volet en porte de grange. La vidéo s'ouvre depuis le noir et se ferme au noir, en 1920x1080 à 30 images par seconde, en H.264.

`VideoEditCoreX` est le moteur GStreamer du SDK. Cette application est compilée pour Windows x64 ; pour macOS et Linux, les paquets natifs du moteur sont dans le [guide de déploiement multiplateforme](../../deployment-x/index.md).

!!! tip "Agents de codage IA : utilisez le serveur MCP de VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public de VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches structurées dans l'API,
    des exemples de code exécutables et des guides de déploiement, plus précis que
    la recherche dans `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Kef_q2cvdAI?controls=1" title="Créer une vidéo à partir d'images avec transitions en C# - tutoriel YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-console)

Les images de la vidéo sont des plans de [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, sous licence [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

La vidéo crée le projet sous .NET 10, qui nécessite Visual Studio 2026 ; dans Visual Studio 2022, choisissez .NET 8 ou .NET 9.

### La version courte

La même tâche en quelques lignes de C#, avec un fondu enchaîné entre les images :

```csharp
using System.IO;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.VideoEditX;

// Le dossier d'images et la vidéo à écrire.
var pics = Directory.GetFiles(args[0], "*.jpg").Order().ToArray();

await VisioForgeX.InitSDKAsync();
using var edit = new VideoEditCoreX();
var done = new TaskCompletionSource<bool>();
edit.OnError += (_, e) => Console.WriteLine(e.Message);
edit.OnStop += (_, e) => done.TrySetResult(e.Successful);

// Chaque image reste 3 secondes, et la suivante
// commence 1 seconde avant sa fin.
var show = TimeSpan.FromSeconds(3);
var cross = TimeSpan.FromSeconds(1);
var step = show - cross;

for (var n = 0; n < pics.Length; n++)
{
    var at = step * n;
    edit.Input_AddImageFile(pics[n], show, at);

    // Le fondu enchaîné remplit la seconde que partagent les deux images.
    if (n > 0)
        edit.Video_Transitions.Add(
            new VideoTransition(
                VideoTransitionType.Crossfade, at, at + cross));
}

var end = step * (pics.Length - 1) + show;
edit.Video_Transition_Add_FadeIn(TimeSpan.Zero, cross);
edit.Video_Transition_Add_FadeOut(end - cross, end);

edit.Output_VideoSize = new Size(1920, 1080);
edit.Output_VideoFrameRate = new VideoFrameRate(30);
// L'encodeur par défaut est petit : 3 Mbit/s en 1080p,
// ce qui se voit en blocs dans un ciel ou un dégradé.
var enc = new OpenH264EncoderSettings(VideoQuality.VeryHigh);
edit.Output_Format = new MP4Output(args[1], enc);

// Start renvoie false quand le rendu ne peut pas commencer ;
// OnStop se déclenche quand le fichier est terminé.
var saved = edit.Start() && await done.Task;
Console.WriteLine(saved ? $"Saved: {args[1]}" : "Not saved.");
```

Son code : [images-to-video-short-console sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-short-console).

## Paquets NuGet nécessaires

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) : le SDK avec `VideoEditCoreX`
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) : GStreamer et les bibliothèques natives du moteur, dont l'encodeur H.264

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.27]" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.26]" />
```

## Le fichier projet

Le modèle console de Visual Studio cible `net10.0`, et pour ce framework cible le paquet du SDK résout sa version Linux. Ajoutez `-windows` au framework cible (`net8.0-windows` ou `net9.0-windows` dans Visual Studio 2022) et réglez `PlatformTarget` sur `x64`, car le paquet natif est x64 :

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

## Le code

`Program.cs`, avec des instructions de niveau supérieur. Le projet active `ImplicitUsings`, donc `System`, `System.IO`, `System.Linq` et `System.Threading.Tasks` ne sont pas listés.

```cs
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// Les images, dans l'ordre des noms de fichier, et la destination de la vidéo.
var folder = args.Length > 0 ? args[0] : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "slides");
var images = Directory.Exists(folder) ? Directory.GetFiles(folder, "*.jpg").Order().ToArray() : [];
var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "slideshow.mp4");
if (images.Length == 0)
{
    Console.WriteLine($"No .jpg files in {folder}");
    return;
}

await VisioForgeX.InitSDKAsync();

var edit = new VideoEditCoreX();
var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
edit.OnError += (s, e) => Console.WriteLine($"\n{e.Message}");
edit.OnProgress += (s, e) => Console.Write($"\rRendering: {e.Progress}%");
edit.OnStop += (s, e) => done.TrySetResult(e.Successful);

// Chaque image reste 3 secondes à l'écran, et la suivante commence 1 seconde avant sa fin.
var show = TimeSpan.FromSeconds(3);
var overlap = TimeSpan.FromSeconds(1);
VideoTransitionType[] transitions =
[
    VideoTransitionType.Crossfade,
    VideoTransitionType.BarWipeLr,
    VideoTransitionType.IrisRect,
    VideoTransitionType.SinglesweepCwt,
    VideoTransitionType.BarndoorV,
];

for (int i = 0; i < images.Length; i++)
{
    var start = (show - overlap) * i;
    edit.Input_AddImageFile(images[i], show, start);

    // La transition occupe la seconde où cette image et la précédente se chevauchent.
    if (i > 0)
    {
        var type = transitions[(i - 1) % transitions.Length];
        edit.Video_Transitions.Add(new VideoTransition(type, start, start + overlap));
    }
}

// Ouverture depuis le noir au début, fermeture au noir à la fin.
var total = (show - overlap) * (images.Length - 1) + show;
edit.Video_Transition_Add_FadeIn(TimeSpan.Zero, overlap);
edit.Video_Transition_Add_FadeOut(total - overlap, total);

edit.Output_VideoSize = new Size(1920, 1080);
edit.Output_VideoFrameRate = new VideoFrameRate(30);
edit.Output_Format = new MP4Output(output);

// Start renvoie false quand le rendu ne peut pas démarrer.
var saved = edit.Start() && await done.Task;
Console.WriteLine(saved ? $"\nSaved: {output}" : "\nThe video was not saved.");

edit.Dispose();
VisioForgeX.DestroySDK();
```

## Fonctionnement

1. **Les images** : le dossier vient de la ligne de commande, ou c'est `Pictures\slides`. `Directory.GetFiles` prend tous les fichiers `.jpg` qu'il contient, et `Order()` les trie par nom ; nommez donc les fichiers `01.jpg`, `02.jpg`, etc. La vidéo est enregistrée sous `slideshow.mp4` dans `Videos`.
2. **Le moteur** : `VisioForgeX.InitSDKAsync` initialise le SDK avant la création de `VideoEditCoreX`. `OnError` et `OnProgress` écrivent dans la console ; `OnStop` complète un `TaskCompletionSource` avec `StopEventArgs.Successful`, qui vaut `false` quand le fichier de sortie n'a pas pu être terminé.
3. **La timeline** : `Input_AddImageFile(file, duration, insertTime)` place chaque image à son propre début, 2 secondes après la précédente, pour 3 secondes ; les images voisines se chevauchent donc d'une seconde. L'image est mise à l'échelle de la sortie et garde son rapport d'aspect.
4. **Les transitions** : à partir de la deuxième image, une `VideoTransition` d'un `VideoTransitionType` couvre le chevauchement ; `Video_Transitions` les contient. `Crossfade` mélange les deux images ; `BarWipeLr`, `IrisRect`, `SinglesweepCwt` et `BarndoorV` sont des volets SMPTE. La liste recommence quand il y a plus d'images que de transitions.
5. **Ouverture et fermeture en fondu** : la vidéo dure 2 secondes par image plus 1 pour la dernière. `Video_Transition_Add_FadeIn` ouvre depuis le noir sur la première seconde, et `Video_Transition_Add_FadeOut` ferme au noir sur la dernière.
6. **La sortie** : `Output_VideoSize` et `Output_VideoFrameRate` fixent 1920x1080 à 30 images par seconde, et `MP4Output` encode la vidéo en H.264 dans un fichier MP4.
7. **Le rendu** : `Start` renvoie `false` quand le rendu ne peut pas démarrer. Sinon, l'application attend `OnStop` ; un rendu qu'une erreur interrompt en cours de route déclenche aussi `OnStop`, avec `Successful` à `false`. Ensuite le moteur est libéré et `VisioForgeX.DestroySDK` libère le SDK.

Six images donnent une vidéo de 13 secondes : six fois 3 secondes, moins cinq secondes de chevauchement.

## Remarques

- **Le filigrane d'essai** : sans licence, le SDK dessine une étiquette orange TRIAL avec visioforge.com/buy dans le coin supérieur gauche de chaque image. Une licence le supprime ; le code reste le même. Les deux vidéos de cette page ont été enregistrées avec une version antérieure du SDK, qui affichait à la place la mention UNREGISTERED en rouge.
- **Le framework cible** : avec `net10.0` au lieu de `net10.0-windows`, l'application reçoit la version Linux du SDK et échoue avec une `DllNotFoundException`.
- **Noms de fichier** : les noms sont triés comme du texte, donc `10.jpg` passe avant `2.jpg`. Complétez les numéros : `01`, `02`.
- **Extensions de fichier** : le motif `*.jpg` ignore les fichiers `.jpeg` et, sous macOS et Linux, où les noms de fichier sont sensibles à la casse, aussi les `.JPG`.
- **Autres formats d'image** : le moteur lit aussi PNG, BMP et GIF. `Directory.GetFiles` n'accepte qu'un motif ; appelez-le une fois par extension et triez les fichiers ensemble.
- **Quitter l'application trop tôt** : le fichier MP4 n'est lisible qu'une fois `OnStop` déclenché ; une application qui se termine avant laisse l'encodeur inachevé.

## Exemples d'applications

- [Tutoriel images vers vidéo, console (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-console) : le code de cette page et de la vidéo

## Questions fréquentes

### Comment créer un diaporama vidéo à partir d'images en C# ?

Créez `VideoEditCoreX`, ajoutez chaque image avec `Input_AddImageFile(file, duration, insertTime)`, réglez `Output_Format` sur `new MP4Output(path)`, appelez `Start` et attendez `OnStop`. Le code ci-dessus le fait pour tout un dossier.

### Comment ajouter un fondu enchaîné entre deux images ?

Faites se chevaucher les deux images sur la timeline et ajoutez `new VideoTransition(VideoTransitionType.Crossfade, start, stop)` à `Video_Transitions` pour le chevauchement : `start` est le début de la deuxième image, `stop` la fin de la première.

### Comment changer la durée d'affichage de chaque image ?

Modifiez `show`, et `overlap` pour la durée des transitions. Chaque image commence alors `show - overlap` après la précédente.

### Quelles transitions puis-je utiliser ?

`Crossfade` et les volets SMPTE de l'énumération `VideoTransitionType` : volets en barre, en boîte, l'iris, balayages en horloge, portes de grange et d'autres. Voir [Transition entre clips](transition-video.md).

## Transcription de la vidéo

### Ce que nous construisons

Créons une vidéo à partir d'un dossier d'images, avec des transitions entre elles, dans une application console C#, avec le VisioForge Video Edit SDK. Voici ce que nous construisons. Une application console qui prend un dossier d'images JPEG et écrit une seule vidéo MP4. Chaque image reste à l'écran 3 secondes, et la suivante commence 1 seconde avant qu'elle ne se termine. Pendant cette seconde, une transition fait passer d'une image à la suivante : d'abord un fondu enchaîné, puis un volet en barre, un iris, un volet en horloge et une porte de grange. La vidéo apparaît en fondu depuis le noir et disparaît en fondu au noir. Elle est en 1920 par 1080 à 30 images par seconde, en H.264. Voici la vidéo que l'application crée à partir de six images. Ce sont des images de Big Buck Bunny, de la Blender Foundation, sous licence Creative Commons Attribution.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet est une application console sous .NET 10. Et un dossier d'images JPEG. Le moteur utilisé ici est VideoEditCoreX, le moteur GStreamer du SDK.

### Créer un projet console

Dans Visual Studio, créez un nouveau projet. Recherchez console, puis choisissez Console App pour C#. Nommez le projet ImagesToVideo, choisissez .NET 10, puis cliquez sur Create.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet. VisioForge.DotNet.VideoEdit contient VideoEditCoreX. CrossPlatform Core Windows x64 contient GStreamer et les bibliothèques natives sur lesquelles repose le moteur. Ou utilisez le gestionnaire de paquets NuGet : clic droit sur le projet, Manage NuGet Packages, puis installez VisioForge.DotNet.VideoEdit depuis l'onglet Browse. Installez ensuite le paquet Core Windows x64. L'onglet Installed affiche les deux.

### Modifier le fichier du projet

Une application console demande une étape de plus. Ouvrez le fichier du projet. Le modèle console cible net10.0, et cette version du SDK est celle pour Linux. Ajoutez -windows au framework cible. Et le paquet natif est en x64, donc réglez PlatformTarget sur x64.

### Trouver les images

Program.cs commence par cinq usings VisioForge : le SDK lui-même, ses types communs, les formats de sortie, les types d'édition vidéo et le moteur. Le dossier vient de la ligne de commande, ou c'est un dossier slides dans Pictures. L'application prend tous les fichiers JPEG qu'il contient, triés par nom : nommez-les donc 01, 02, 03, et ainsi de suite. La vidéo est enregistrée dans slideshow.mp4, dans Videos. S'il n'y a aucune image, l'application le signale et s'arrête.

### Créer l'éditeur

Initialisez le SDK avec InitSDKAsync, puis créez VideoEditCoreX. Un TaskCompletionSource attend la fin du rendu. OnError affiche les erreurs, et OnProgress affiche la progression en pourcentage. OnStop se déclenche à la fin du rendu, et son indicateur Successful indique si le fichier a été terminé.

### Construire la timeline

Chaque image est à l'écran 3 secondes, et la suivante la chevauche de 1 seconde. Les transitions viennent de l'enum VideoTransitionType : Crossfade, puis quatre volets. BarWipeLr balaie de gauche à droite. IrisRect ouvre un rectangle depuis le centre. SinglesweepCwt balaie comme une aiguille d'horloge. Et BarndoorV s'ouvre comme une porte de grange. Chaque image commence 2 secondes après la précédente. Input_AddImageFile place l'image sur la timeline à ce début, pour 3 secondes. L'image est mise à l'échelle de la taille de sortie et garde son format. À partir de la deuxième image, une VideoTransition remplit la seconde où les deux images se chevauchent. La liste se répète quand il y a plus d'images que de transitions. La durée totale est de 2 secondes par image, plus 1 pour la dernière. Video_Transition_Add_FadeIn fait un fondu depuis le noir pendant la première seconde, et Video_Transition_Add_FadeOut un fondu de sortie pendant la dernière. Puis la sortie : 1920 par 1080 à 30 images par seconde, dans un fichier MP4. MP4Output encode la vidéo en H.264.

### Faire le rendu de la vidéo

Start renvoie false quand le rendu ne peut pas démarrer. Sinon, l'application attend OnStop avec await et affiche si la vidéo a été enregistrée, et où. Si une erreur interrompt le rendu en cours de route, OnStop se déclenche quand même, avec Successful à false. Enfin, libérez le moteur avec Dispose et libérez le SDK.

### L'exécuter

Mettez le code dans Program.cs, puis lancez l'application. Les six images sont déjà dans Pictures, slides. La console affiche la progression, puis le chemin de la vidéo enregistrée.

### Vérifier le résultat

ffprobe le confirme. H.264, 1920 par 1080, 30 images par seconde et 13 secondes : six images de 3 secondes, moins cinq secondes de chevauchement. À mi-chemin de la première transition, les deux images apparaissent à la fois : c'est le fondu enchaîné. Ici, le volet en barre a atteint le milieu de l'image. L'iris ouvre un rectangle depuis le centre. Le volet en horloge tourne autour du centre. Et la porte de grange s'ouvre depuis le milieu.

### Le filigrane de la version d'essai

Vous avez peut-être remarqué le UNREGISTERED rouge dans le coin supérieur gauche. C'est la version d'essai : sans licence, le SDK l'affiche sur chaque image. Une licence le supprime, et le code reste le même.

### Erreurs fréquentes

Quelques erreurs fréquentes. Laisser le framework cible à net10.0, sans -windows : c'est la version du SDK pour Linux, et l'application échoue avec une DllNotFoundException. Nommer les fichiers 1, 2 et jusqu'à 10 : les noms sont triés comme du texte, donc 10 passe avant 2. Utilisez plutôt 01, 02. Mettre des fichiers PNG dans le dossier : ce code ne cherche que des fichiers JPEG, alors modifiez le motif de recherche si vous en voulez d'autres. Et quitter l'application avant OnStop : l'encodeur n'a pas fini, et le fichier MP4 n'est pas lisible.

### Étapes suivantes

Il existe aussi des guides sur les transitions entre clips vidéo, les incrustations de texte et d'image, l'assemblage de plusieurs fichiers en un seul et le déploiement des redists avec votre application. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Transition entre clips](transition-video.md) : transitions entre fichiers vidéo sur la timeline
- [Ajouter une incrustation de texte](add-text-overlay.md) : titres et légendes sur la vidéo
- [Ajouter une incrustation d'image](add-image-overlay.md) : un logo ou une image sur la vidéo
- [Fusionner vidéo et audio sans réencodage](output-file-from-multiple-sources.md) : la vidéo d'un fichier et l'audio d'un autre, copiés dans un MP4
- [Déploiement multiplateforme](../../deployment-x/index.md) : les paquets natifs dont `VideoEditCoreX` a besoin sur chaque plateforme
- [Page produit Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net)
