---
title: Couper une vidéo sans réencodage en quelques lignes de C#
description: Copiez une partie d'un fichier vidéo dans un nouveau fichier sans réencodage, en quelques lignes de C# avec Video Edit SDK .NET et FastEdit_CutFileAsync.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCore
  - FastEditSeekMode

video:
  id: EASyGlUymvY
  upload_date: 2026-11-24T07:00:00Z
  duration: PT1M3S
---

# Couper une vidéo sans réencodage en C\#

[Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

Cette page coupe une partie d'un fichier vidéo dans un nouveau fichier avec [Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net) et son moteur `VideoEditCore`, dans une application console C#. `FastEdit_CutFileAsync` copie les flux compressés tels quels, rien n'est donc réencodé : les codecs, la résolution, la fréquence d'images et le débit restent les mêmes, il n'y a aucune perte de qualité, et 30 secondes d'un film 720p sont enregistrées en une demi-seconde environ.

`VideoEditCore` est le moteur Windows du SDK ; ses méthodes `FastEdit_` exécutent l'outil FFmpeg fourni dans le paquet redist FFmpeg.

!!! tip "Agents de codage IA : utilisez le serveur MCP de VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public de VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches structurées dans l'API,
    des exemples de code exécutables et des guides de déploiement, plus précis que
    la recherche dans `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutoriel vidéo

La vidéo d'une minute écrit et exécute cette application console : 30 secondes d'un film 720p coupées en une demi-seconde environ.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/EASyGlUymvY?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Paquets NuGet requis

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) — le SDK avec `VideoEditCore`
- [VisioForge.DotNet.Core.Redist.FFMPEG.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/) — l'outil FFmpeg qu'exécutent les méthodes `FastEdit_` ; pour une application x86, [VisioForge.DotNet.Core.Redist.FFMPEG.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.23]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="[2026.11.23]" />
```

Le redist est x64 : définissez donc `PlatformTarget` sur `x64` dans le fichier projet :

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## Le code

L'application console complète : elle copie de 1:00 à 1:30 de `movie.mp4` dans `cut.mp4`.

```csharp
using System;
using VisioForge.Core.VideoEdit;

await using var core = new VideoEditCore();
core.OnError += (s, e) =>
    Console.WriteLine(e.Message);
var ok = await core.FastEdit_CutFileAsync(
    "movie.mp4",
    TimeSpan.FromSeconds(60),
    TimeSpan.FromSeconds(90),
    "cut.mp4");
Console.WriteLine(ok ? "Done" : "Failed");
```

Le projet complet est [cut-video-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/cut-video-short-console). Les noms de fichiers sont relatifs au répertoire de travail : placez donc `movie.mp4` dans le dossier à partir duquel vous lancez l'application.

## Fonctionnement

1. **Le moteur** — `VideoEditCore` n'a pas besoin d'une vue vidéo pour une coupe. `await using` le libère à la fin de l'application.
2. **Les erreurs** — `OnError` indique pourquoi une coupe a échoué, par exemple un fichier d'entrée absent ou un redist FFmpeg manquant ; l'application l'affiche avant `Failed`.
3. **La coupe** — `FastEdit_CutFileAsync(input, start, stop, output)` copie la partie de `start` à `stop` dans le fichier de sortie sans réencodage, et renvoie `true` une fois le fichier enregistré. Un `stop` égal à `TimeSpan.Zero` coupe jusqu'à la fin du fichier.
4. **Les images clés** — la coupe commence sur l'image clé située à `start` ou avant. Les images entre cette image clé et `start` sont aussi copiées. Dans une sortie MP4 ou MOV, elles sont marquées pour être ignorées, si bien que le lecteur démarre à `start` ; dans les autres conteneurs, et pour une entrée MPEG-TS, la coupe se lit à partir de cette image clé et dure d'autant plus.

La sortie sur un Big Buck Bunny de 10 minutes (H.264 1280x720 à 24 i/s, AAC), vérifiée avec ffprobe : 30,0 secondes, le profil H.264 High et l'AAC de la source. Le premier paquet vidéo de la coupe est l'image clé de la source à 57,75 s, exactement de la même taille ; les images avant 1:00 portent l'indicateur de rejet. [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, sous licence [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

```console
ffprobe -v error -of compact cut.mp4 -show_entries format=duration:stream=codec_name,profile,width,height
stream|codec_name=h264|profile=High|width=1280|height=720
stream|codec_name=aac|profile=LC
format|duration=30.000544
```

## Remarques

- **Mode de recherche** — la surcharge avec `FastEditSeekMode` choisit où FFmpeg effectue la recherche. `FastEditSeekMode.Input` (par défaut) commence sur l'image clé située à la position de départ ou avant. `FastEditSeekMode.Output` supprime la vidéo jusqu'à la première image clé suivante, si bien qu'une coupe plus courte qu'un intervalle entre images clés peut sortir sans vidéo, et un `stop` égal à `TimeSpan.Zero` copie le fichier entier.
- **Précision à l'image** — une coupe sans réencodage ne peut pas commencer entre deux images clés. Pour couper sur une image exacte, réencodez avec la timeline de `VideoEditCore` ou de `VideoEditCoreX`.
- **Le conteneur** — la sortie reçoit les codecs de la source : gardez donc un conteneur qui les accepte, MP4 pour H.264 ou H.265 avec AAC, MKV pour les autres.
- **Le redist FFmpeg** — sans le paquet redist FFmpeg, les méthodes `FastEdit_` n'ont aucun outil à exécuter. Déployez-le avec votre application ; consultez le [guide de déploiement](../deployment.md).

## Exemples d'applications

- [Cut video short console (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/cut-video-short-console) — le code de cette page
- [Cut Video File (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/WPF/CSharp/Cut%20Video%20File) — le même appel dans une fenêtre WPF avec une barre de progression

## Questions fréquentes

### Comment couper une vidéo sans réencodage en C# ?

Créez `VideoEditCore` et appelez `FastEdit_CutFileAsync(inputFile, start, stop, outputFile)`. Les flux sont copiés tels quels, la coupe prend donc à peu près le temps de copier cette partie du fichier.

### Pourquoi ma coupe commence-t-elle un peu avant l'heure de début ?

Une coupe sans réencodage commence sur une image clé. Avec le mode de recherche par défaut, les images à partir de l'image clé précédant l'heure de début sont copiées mais marquées pour être ignorées : dans un fichier MP4 ou MOV, les lecteurs démarrent donc à l'heure de début, mais certains éditeurs les affichent quand même. Les autres conteneurs, et une entrée MPEG-TS, se lisent à partir de l'image clé.

### Puis-je couper jusqu'à la fin du fichier ?

Oui : passez `TimeSpan.Zero` comme heure de fin.

### La qualité est-elle identique à celle de la source ?

Oui. Rien n'est décodé ni encodé : chaque image copiée est l'image de la source.

## Voir aussi

- [Fusionner vidéo et audio sans réencodage](output-file-from-multiple-sources.md) — la vidéo d'un fichier et l'audio d'un autre, copiés dans un MP4
- [Plusieurs segments](several-segments.md) — assembler des parties de fichiers en une vidéo, réencodée
- [Déploiement](../deployment.md) — les redists dont `VideoEditCore` a besoin sur le PC de l'utilisateur
- [Page produit Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net)
