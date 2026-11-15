---
title: Fusionner vidéo et audio sans réencodage en C# .NET
description: Prenez la vidéo d'un fichier et l'audio d'un autre, et copiez-les dans un MP4 sans réencodage, dans une application C# WinForms avec Video Edit SDK .NET.
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
  - FFMPEGStream
video:
  id: l59WVEu4uIs
  upload_date: 2026-11-15T09:00:11Z
  duration: PT9M57S

---

# Fusionner vidéo et audio sans réencodage en C#

[Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

Ce tutoriel crée une application C# WinForms qui prend le flux vidéo d'un fichier et le flux audio d'un autre, et les copie tous les deux dans un seul MP4 avec [Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net) et son moteur `VideoEditCore`. `FastEdit_MuxStreamsAsync` copie les flux compressés tels quels, donc rien n'est réencodé : le codec, la résolution, la fréquence d'images et le débit restent les mêmes, il n'y a aucune perte de qualité, et un extrait de 27 secondes est enregistré en quelques secondes. Le même appel place une nouvelle bande-son sous une vidéo, remplace son audio ou ajoute une deuxième piste audio.

`VideoEditCore` est le moteur Windows du SDK ; ses méthodes `FastEdit_` exécutent l'outil FFmpeg fourni dans le paquet redist FFmpeg.

!!! tip "Agents de codage IA : utilisez le serveur MCP de VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public de VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches structurées dans l'API,
    des exemples de code exécutables et des guides de déploiement, plus précis que
    la recherche dans `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutoriel YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/l59WVEu4uIs?controls=1" title="Fusionner vidéo et audio sans réencodage en C# - tutoriel YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/mux-streams-winforms)

La vidéo du tutoriel est un extrait de [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, sous licence [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

La vidéo crée le projet en .NET 10, qui nécessite Visual Studio 2026 ; dans Visual Studio 2022, choisissez .NET 8 ou .NET 9. Sous .NET Framework 4.7.2, `Form1.cs` se compile sans modification dans un projet de style SDK (créez-le avec le modèle Windows Forms App et réglez `TargetFramework` sur `net472`) avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>` ; dans `Program.cs`, remplacez `ApplicationConfiguration.Initialize()` par `Application.EnableVisualStyles()` et `Application.SetCompatibleTextRenderingDefault(false)`.

## Paquets NuGet nécessaires

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) : le SDK avec `VideoEditCore`
- [VisioForge.DotNet.Core.Redist.FFMPEG.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/) : l'outil FFmpeg qu'exécutent les méthodes `FastEdit_` ; pour une application x86, [VisioForge.DotNet.Core.Redist.FFMPEG.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.11.13" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="2026.11.13" />
```

Le redist est en x64, réglez donc `PlatformTarget` sur `x64` dans le fichier projet :

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <UseWindowsForms>true</UseWindowsForms>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## Le formulaire

`Form1` contient une zone de texte et un bouton `...` pour le fichier vidéo (`edVideo`, `btVideo`), la même chose pour le fichier audio (`edAudio`, `btAudio`), une zone de texte pour le fichier de sortie (`edOutput`), une case à cocher `cbShortest` (« Stop at the end of the shortest stream », cochée), le bouton `btStart` et un `OpenFileDialog`. Les événements `Load` et `FormClosing` du formulaire et les événements `Click` des trois boutons ont des gestionnaires.

## Le code

`Form1.cs` :

```cs
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;

namespace MuxStreams;

public partial class Form1 : Form
{
    private VideoEditCore videoEdit1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoEdit1 = new VideoEditCore();

        // Affiche les erreurs du SDK dans la barre de titre
        videoEdit1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
    }

    private void btVideo_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK) edVideo.Text = openFileDialog1.FileName;
    }

    private void btAudio_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK) edAudio.Text = openFileDialog1.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        // Le premier flux vidéo d'un fichier et le premier flux audio d'un autre
        var streams = new List<FFMPEGStream>
        {
            new FFMPEGStream { Filename = edVideo.Text, ID = "v:0" },
            new FFMPEGStream { Filename = edAudio.Text, ID = "a:0" },
        };

        btStart.Enabled = false;
        Text = "Saving...";
        try
        {
            // Copie les flux tels quels dans un seul fichier, sans réencodage
            var saved = await videoEdit1.FastEdit_MuxStreamsAsync(streams, cbShortest.Checked, edOutput.Text);
            if (saved) Text = "Saved: " + edOutput.Text; // en cas d'échec, le titre garde l'erreur
        }
        finally
        {
            btStart.Enabled = true;
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Laisse le fichier se terminer avant la fermeture de la fenêtre
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            return;
        }

        videoEdit1.Dispose();
    }
}
```

## Fonctionnement

1. **Le moteur** : `Form1_Load` crée `VideoEditCore`. `OnError` signale les erreurs du moteur ; elles peuvent arriver sur un thread de travail, donc `BeginInvoke` les place dans la barre de titre sur le thread de l'interface. Le fichier de sortie est `output.mp4` dans `Vidéos` par défaut.
2. **Les flux** : chaque `FFMPEGStream` désigne un fichier (`Filename`) et un flux de ce fichier (`ID`). Les ID sont des spécificateurs de flux FFmpeg : `"v:0"` est le premier flux vidéo du fichier, `"a:0"` le premier flux audio et `"a:1"` le deuxième. Un `"v"` ou un `"a"` seul prend tous les flux de ce type.
3. **La copie** : `FastEdit_MuxStreamsAsync(streams, shortest, outputFile)` copie les flux de la liste, dans cet ordre, dans le fichier de sortie sans réencodage, et renvoie `true` une fois le fichier enregistré.
4. **Le flux le plus court** : avec `shortest` à `true`, le fichier s'arrête avec le flux le plus court. Avec `false`, il dure aussi longtemps que le plus long : dans la vidéo, 108 secondes de musique sous un extrait de 27 secondes.
5. **Fermer la fenêtre** : pendant un enregistrement, `Start` est désactivé et `Form1_FormClosing` annule la fermeture pour que le fichier se termine d'abord. Sinon, il libère le moteur.

La sortie de la vidéo, vérifiée avec ffprobe : le flux H.264 de la source, en 1920x1080 au même débit, et la piste musicale AAC. Le MD5 du flux vidéo est le même dans la source et dans la sortie : le flux est copié octet par octet.

```console
ffprobe -v error -of compact output.mp4 -show_entries format=duration,size:stream=codec_name,width,height,bit_rate,sample_rate
stream|codec_name=h264|width=1920|height=1080|bit_rate=2783582
stream|codec_name=aac|sample_rate=48000|bit_rate=192448
format|duration=27.466667|size=10225512
```

## Remarques

- **Le conteneur doit accepter les codecs** : rien n'est réencodé, le format de sortie doit donc contenir les codecs de la source tels quels. H.264 ou H.265 avec AAC ou MP3 vont dans un MP4 ; pour d'autres codecs, écrivez un fichier MKV.
- **L'audio propre au fichier vidéo** : `"v:0"` ne prend que la vidéo de ce fichier, sa bande-son est donc laissée de côté. Pour la garder comme deuxième piste, ajoutez `new FFMPEGStream { Filename = edVideo.Text, ID = "a:0" }` à la liste.
- **Plus de pistes audio** : chaque `FFMPEGStream` avec un ID audio devient une piste audio de plus dans la sortie, par exemple une deuxième langue.
- **Le redist FFmpeg** : sans le paquet redist FFmpeg, les méthodes `FastEdit_` n'ont aucun outil à exécuter. Déployez-le avec votre application ; consultez le [guide de déploiement](../deployment.md).

## Exemples d'applications

- [Tutoriel de fusion de flux, WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/mux-streams-winforms) : le code de cette page et de la vidéo

## Questions fréquentes

### Comment ajouter une piste audio à une vidéo sans réencodage en C# ?

Créez `VideoEditCore`, listez le flux vidéo (`ID = "v:0"`) et le flux audio (`ID = "a:0"`) comme éléments `FFMPEGStream`, et appelez `FastEdit_MuxStreamsAsync(streams, true, outputFile)`. Le code ci-dessus le fait avec deux fichiers choisis dans une boîte de dialogue.

### Comment remplacer l'audio d'un fichier MP4 ?

Prenez uniquement la vidéo du MP4 avec `"v:0"` et le nouvel audio de l'autre fichier avec `"a:0"`. La bande-son propre au MP4 n'est pas dans la liste, elle n'est donc pas dans la sortie.

### À quoi sert le paramètre shortest ?

Avec `true`, la sortie s'arrête là où s'arrête le flux le plus court. Avec `false`, la sortie dure aussi longtemps que le flux le plus long, et le plus court s'arrête simplement.

### La qualité est-elle la même que celle de la source ?

Oui. Les flux sont copiés, pas décodés puis encodés à nouveau : le codec, la résolution, la fréquence d'images et le débit sont ceux des fichiers source.

## Transcription de la vidéo

### Ce que nous construisons

Prenons la vidéo d'un fichier et l'audio d'un autre, et réunissons-les dans un MP4 sans réencodage, dans une application C# WinForms avec le VisioForge Video Edit SDK. Voici ce que nous allons construire. Une application WinForms qui fusionne deux fichiers en un seul. Elle prend le flux vidéo d'un fichier et le flux audio d'un autre. Les deux sont copiés tels quels, sans réencodage et sans perte de qualité. Et une case à cocher arrête le fichier à la fin du flux le plus court. Voici l'application terminée. Elle vient de placer une nouvelle piste musicale sous un extrait de Big Buck Bunny.

### Copier, pas réencoder

Pourquoi copier plutôt que réencoder ? Le réencodage décode chaque image et la compresse à nouveau. Cela prend du temps, et chaque passage perd un peu de qualité. Une copie de flux déplace les données compressées dans un nouveau fichier : le codec, la résolution, la fréquence d'images et le débit restent exactement les mêmes. Il n'y a aucune perte de qualité, et cela prend des secondes au lieu de minutes. Une seule règle : le conteneur de sortie doit accepter les deux codecs. H.264 ou H.265 avec AAC ou MP3 vont dans un MP4. Pour tout le reste, utilisez MKV.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Et il vous faut un fichier vidéo et un fichier audio. VideoEditCore est le moteur Windows du SDK, et ses méthodes FastEdit exécutent l'outil FFmpeg fourni avec le SDK.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet MuxStreams. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet : le Video Edit SDK lui-même et le redist FFmpeg. Le redist contient l'outil FFmpeg qu'exécutent les méthodes FastEdit. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.VideoEdit dans Browse et installez-le. Puis installez le redist FFmpeg pour x64. Le redist est celui pour x64 : réglez donc PlatformTarget sur x64 dans le fichier du projet.

### Concevoir le formulaire WinForms

Depuis la Toolbox, ajoutez une étiquette, une zone de texte et un petit bouton pour le fichier vidéo, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Ajoutez la même ligne pour le fichier audio, puis une étiquette et une zone de texte pour le fichier de sortie. Ajoutez une case à cocher pour s'arrêter à la fin du flux le plus court, cochée par défaut, et le bouton Start. Puis un OpenFileDialog. Il va dans la barre des composants sous le formulaire. Son filtre liste les fichiers multimédias. Double-cliquez sur le formulaire et sur les trois boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur d'édition

En haut de Form1.cs, ajoutez ces using. Dans le gestionnaire Load du formulaire, créez le moteur VideoEditCore. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles s'affichent dans la barre de titre. Le fichier de sortie va par défaut dans le dossier Vidéos.

### Choisir les fichiers

Les deux petits boutons ouvrent la même boîte de dialogue et placent le fichier choisi dans leur zone de texte.

### Fusionner les flux

Le gestionnaire de Start liste les flux à copier. Chaque FFMPEGStream désigne un fichier et un flux de ce fichier. v:0 est le premier flux vidéo du fichier vidéo, et a:0 le premier flux audio du fichier audio. Les ID suivent les spécificateurs de flux de FFmpeg. v:0 est le premier flux vidéo, a:0 le premier flux audio, et a:1 le deuxième. Un « v » ou un « a » seul prend tous les flux de ce type du fichier. La bande-son d'origine de l'extrait est donc laissée de côté : nous ne demandons que sa vidéo. Ensuite, il désactive Start et appelle FastEdit_MuxStreamsAsync avec les flux, la case à cocher et le fichier de sortie. L'appel copie les flux dans un seul fichier et renvoie true une fois le fichier enregistré. Le deuxième argument est la case à cocher. Cochée, le fichier s'arrête avec le flux le plus court : ici, vingt-sept secondes de vidéo et vingt-sept secondes de musique. Décochée, le fichier dure aussi longtemps que le flux le plus long : ici, cent huit secondes, surtout de la musique sans image.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait le fichier inachevé. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant un enregistrement, il annule la fermeture. Sinon, il libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Le fichier de sortie pointe déjà vers le dossier Vidéos. Choisissez le fichier vidéo : un extrait de Big Buck Bunny. Puis le fichier audio : une piste musicale. Cliquez sur Start. Quelques secondes plus tard, la barre de titre indique que le fichier est enregistré. Puis nous fermons la fenêtre.

### Vérifier le fichier avec ffprobe

Le fichier arrive dans Vidéos : vingt-sept secondes de vidéo en 1920 par 1080 à trente images par seconde. ffprobe montre ce qu'il contient. La vidéo est en H.264, 1920 par 1080, avec le même débit que la source. L'audio est la piste musicale AAC. Et le MD5 du flux vidéo est le même dans les deux fichiers : il a été copié octet par octet.

### Erreurs fréquentes

Quelques erreurs fréquentes. Le paquet redist FFmpeg manque, et FastEdit n'a aucun outil à exécuter. Un « v » ou un « a » seul là où v:0 ou a:0 était voulu : le fichier reçoit des flux en trop. Un codec que le conteneur de sortie ne peut pas contenir : utilisez plutôt MKV. Et fermer la fenêtre au milieu d'un enregistrement : attendez d'abord qu'il se termine.

### Étapes suivantes

Ensuite, le même moteur coupe une partie d'un fichier sans réencodage, avec FastEdit_CutFileAsync, et assemble des fichiers de même format avec FastEdit_JoinFilesAsync. Et le tutoriel images vers vidéo crée une toute nouvelle vidéo avec des transitions. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Créer une vidéo à partir d'images avec transitions](video-images-console.md) : une nouvelle vidéo rendue à partir d'un dossier d'images
- [Plusieurs flux audio dans des fichiers vidéo](adding-video-file-with-multiple-audio-streams.md) : sélectionner et mixer les pistes audio d'un fichier vidéo
- [Déploiement](../deployment.md) : les redists dont `VideoEditCore` a besoin sur le PC de l'utilisateur
- [Page produit Video Edit SDK .NET](https://www.visioforge.com/fr/video-edit-sdk-net)
