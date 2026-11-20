---
title: Aperçu webcam et capture d'image en JPEG en C# WinForms
description: Affichez l'aperçu en direct d'une webcam en C# WinForms avec VideoCaptureCore et enregistrez l'image courante dans un fichier JPEG avec Frame_SaveAsync.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - Snapshot
  - JPEG
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - VideoView
video:
  id: Qp9m9xJ_QCA
  upload_date: 2026-11-15T13:01:09Z
  duration: PT10M16S

---

# Aperçu webcam et capture d'image en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel affiche l'aperçu en direct d'une webcam dans une application C# WinForms avec le moteur VideoCaptureCore et enregistre l'image courante dans un fichier JPEG :

- la caméra, son format et sa fréquence d'images se choisissent dans des listes ;
- le mode `VideoPreview` affiche la caméra sans écrire de fichier, il ne faut donc ni encodeur ni format de sortie ;
- un bouton Save Frame écrit l'image courante avec `Frame_SaveAsync`, à la résolution propre de la caméra, dans un nouveau fichier horodaté du dossier Images ;
- l'application ouvre le fichier enregistré dans la visionneuse d'images par défaut.

C'est ainsi que fonctionnent les photomatons et les photos d'identité, la numérisation d'un document ou d'une étiquette, les postes d'inspection qui gardent une photo de chaque pièce, et toute application qui a besoin d'une image fixe à traiter ou à mettre dans un rapport.

Ce tutoriel utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK.

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Qp9m9xJ_QCA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/camera-video-preview-winforms)

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et le redist de capture vidéo avec ses filtres DirectShow. Un aperçu n'écrit aucun fichier, il n'a donc besoin d'aucun paquet d'encodeur :

- Pour les applications 64 bits : [Redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Pour les applications 32 bits : [Redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Réglez `PlatformTarget` dans le fichier du projet selon le redist (`x64` ou `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des listes déroulantes pour la caméra, le format et la fréquence d'images, et les boutons Start, Stop et Save Frame ; Stop et Save Frame sont désactivés au départ. Le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, le même code se compile dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>` ; dans `Program.cs`, remplacez `ApplicationConfiguration.Initialize()`, que .NET Framework ne génère pas, par `Application.EnableVisualStyles()` et `Application.SetCompatibleTextRenderingDefault(false)`, et ajoutez `<dpiAware>true</dpiAware>` dans un `app.manifest` pour garder la prise en charge du DPI système qu'active .NET 10.

```csharp
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamPreview;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1 = null!;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Afficher les erreurs du SDK dans la barre de titre
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
    }

    private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        var format = device.VideoFormats.First(f => f.Name == cbFormat.Text);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam preview";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        // Vidéo seule : la lecture audio est activée par défaut
        videoCapture1.Audio_PlayAudio = false;
        videoCapture1.Mode = VideoCaptureMode.VideoPreview;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btSaveFrame.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btSaveFrame_Click(object sender, EventArgs e)
    {
        // Un nouveau JPEG dans Images à chaque clic, à la résolution propre de la caméra
        var file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            $"frame-{DateTime.Now:yyyyMMdd-HHmmss-fff}.jpg");

        if (!await videoCapture1.Frame_SaveAsync(file, ImageFormat.Jpeg, 85)) return;

        // L'ouvrir dans la visionneuse par défaut ; sans application pour .jpg, il ne reste que le chemin dans le titre
        Text = file;
        try { Process.Start(new ProcessStartInfo(file) { UseShellExecute = true }); }
        catch (Win32Exception) { }
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        btSaveFrame.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Démarrage ou arrêt en cours : attendre avant de fermer
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true;
            return;
        }

        // Dispose arrête un aperçu en cours : il n'y a aucun fichier à terminer
        videoCapture1.Dispose();
    }
}
```

## Fonctionnalités clés de l'implémentation

1. **Mode aperçu** - `Mode = VideoCaptureMode.VideoPreview` met la caméra à l'écran et n'écrit aucun fichier, il n'y a donc ni `Output_Filename` ni `Output_Format` à régler. Les images arrivent quand même à l'application, et c'est ce qu'utilise `Frame_SaveAsync`. Passez le mode à `VideoCapture`, ajoutez un fichier de sortie et un format, et la même application enregistre
2. **Format de la caméra** - avec `Format_UseBest` activé, le moteur choisit tout seul la résolution la plus haute jusqu'à 1080p ; l'exemple le désactive et transmet le format et la fréquence d'images choisis dans les listes
3. **Audio** - `Audio_PlayAudio` vaut `true` par défaut et joue dans les haut-parleurs la source audio choisie dans `Audio_CaptureDevice`, ou, pour une carte d'acquisition dont le filtre vidéo a une broche audio, la source vidéo elle-même (`IsAudioSource` ; le microphone d'une webcam USB est un périphérique audio distinct). L'exemple n'affiche que la vidéo, il le désactive donc ; laissé activé sans l'une ni l'autre, le moteur déclenche `OnError` ("The audio source is not configured"), met `Audio_PlayAudio` à `false` et fonctionne sans son
4. **Enregistrer une image** - `Frame_SaveAsync(filename, format, quality = 85, customWidth = 0, customHeight = 0)` écrit l'image courante dans n'importe quel `ImageFormat`, comme JPEG, PNG, BMP ou TIFF. La qualité ne s'applique qu'au JPEG. Indiquez à la fois une largeur et une hauteur pour redimensionner l'image ; sans elles, le fichier a la résolution de la caméra (ici 1920x1080), et non la taille de l'aperçu à l'écran
5. **Quand l'enregistrement échoue** - `Frame_SaveAsync` renvoie `false` et déclenche `OnError` quand l'aperçu ne tourne pas ou qu'aucune image n'est encore arrivée. L'exemple n'active Save Frame qu'après que `StartAsync` a renvoyé `true`
6. **Un nouveau fichier à chaque clic** - le nom du fichier porte la date et l'heure à la milliseconde près, donc une image ne remplace jamais la précédente
7. **Ouvrir le fichier** - `Process.Start` avec `UseShellExecute = true` ouvre le JPEG dans la visionneuse d'images par défaut. Sur un PC sans application pour `.jpg`, il lève une `Win32Exception`, et il reste le chemin dans la barre de titre
8. **Fermeture** - `Dispose()` arrête un aperçu en cours, et un aperçu n'a aucun fichier à terminer : le gestionnaire de fermeture n'attend donc que pendant le démarrage ou l'arrêt de l'aperçu

## Images en mémoire

Pour travailler sur une image sans fichier :

- `Frame_GetCurrent()` renvoie l'image courante sous forme de `System.Drawing.Bitmap` ;
- l'événement `OnVideoFrameBitmap` vous remet chaque image dès qu'elle arrive.

## Transcription de la vidéo

### Ce que nous construisons

Affichons l'aperçu en direct d'une webcam dans une application C# WinForms, et enregistrons-en des images, avec le VisioForge Video Capture SDK. Voici ce que nous construisons. Une application WinForms avec un aperçu en direct de la webcam. Vous choisissez la caméra, son format et sa fréquence d'images dans des listes. Un bouton Save Frame écrit l'image courante dans un fichier JPEG, et l'application ouvre ce fichier dans la visionneuse d'images par défaut, pour que vous voyiez ce qui a été enregistré. Voici l'application terminée, avec l'aperçu d'une Logitech C920 en 1920 par 1080. Rien n'est enregistré. L'aperçu ne fait qu'afficher l'image, et une image est enregistrée quand vous la demandez. Où est-ce utile ? Les photomatons et les photos d'identité. Numériser un document ou une étiquette avec une webcam. Les postes d'inspection qui gardent une photo de chaque pièce. Et toute application qui a besoin d'une image fixe à traiter ou à mettre dans un rapport.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Et une webcam. VideoCaptureCore est le moteur DirectShow du SDK, donc cette application fonctionne uniquement sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet WebcamPreview. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet : le SDK lui-même, et le redist de capture vidéo avec ses filtres DirectShow. Un aperçu n'écrit aucun fichier, il n'a donc besoin d'aucun paquet d'encodeur. Dans Visual Studio, c'est Manage NuGet Packages. Cherchez VisioForge.DotNet.Core et installez-le. Installez ensuite le redist de capture vidéo pour x64. Le redist est en x64, donc réglez PlatformTarget sur x64 dans le fichier du projet. Une application x86 prend le redist x86 à la place.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'aperçu en direct. Ajoutez une étiquette et une liste déroulante pour la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour le format de la caméra et la fréquence d'images. Ajoutez les boutons Start, Stop et Save Frame. Stop et Save Frame restent désactivés tant que l'aperçu ne tourne pas. Double-cliquez sur le formulaire, sur les listes de caméra et de format, et sur les trois boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces usings. System.Diagnostics ouvre le fichier enregistré, System.ComponentModel contient l'exception qu'il peut lever, et System.Drawing.Imaging nomme le format d'image. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu en direct est dessiné. Les erreurs du moteur arrivent par l'événement OnError. Ici, elles vont dans la barre de titre.

### Lister les formats de la caméra

Video_CaptureDevices renvoie toutes les caméras. Nous listons leurs noms et sélectionnons la première. Quand la caméra change, son gestionnaire remplit la liste des formats avec ceux de cette caméra, et choisit 1920x1080 MJPG quand il existe. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images, et choisit trente images par seconde.

### Démarrer l'aperçu

Le gestionnaire de Start vérifie d'abord qu'une fréquence d'images est sélectionnée. Ensuite il sélectionne la caméra par son nom, désactive Format_UseBest, et transmet le format et la fréquence d'images des listes. Laissé activé, Format_UseBest choisit tout seul la résolution la plus haute jusqu'à 1080p. Audio_PlayAudio est activé par défaut : il joue la source audio sélectionnée dans les haut-parleurs. Cette application n'affiche que la vidéo, donc on le désactive. Laissé activé sans source audio sélectionnée, le SDK signale une erreur et fonctionne sans son. Le mode VideoPreview affiche la caméra sans écrire de fichier. StartAsync rend la main une fois l'aperçu lancé, puis Stop et Save Frame sont activés. VideoPreview met la caméra à l'écran et n'écrit aucun fichier, il n'y a donc ni encodeur ni format de sortie à régler. Les images arrivent quand même à l'application, et c'est ce qu'utilise Save Frame. Passez le mode à VideoCapture, ajoutez un fichier de sortie et un format, et la même application enregistre.

### Enregistrer une image

Save Frame construit un nom de fichier dans le dossier Pictures, avec la date et l'heure à la milliseconde près, donc chaque clic crée un nouveau fichier. Frame_SaveAsync y écrit l'image courante en JPEG, avec une qualité de 85, et renvoie false quand il n'y arrive pas. Ensuite le chemin va dans la barre de titre, et Process.Start avec UseShellExecute ouvre le fichier dans la visionneuse d'images par défaut. Sur un PC sans application pour les fichiers JPEG, Process.Start lève une Win32Exception, et il reste le chemin dans le titre. Frame_SaveAsync prend le nom du fichier et un format d'image, comme JPEG, PNG, BMP ou TIFF. La qualité ne s'applique qu'au JPEG, et vaut 85 par défaut. Indiquez à la fois une largeur et une hauteur, et l'image est redimensionnée à cette taille. Sans elles, le fichier a la résolution de la caméra, et non la taille de l'aperçu à l'écran. Il renvoie false, et déclenche OnError, quand l'aperçu ne tourne pas ou qu'aucune image n'est encore arrivée.

### Arrêter et fermer

Stop désactive Stop et Save Frame, et StopAsync arrête la caméra. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant que l'aperçu démarre ou s'arrête, les deux boutons sont désactivés, et le gestionnaire attend en annulant la fermeture. Sinon, il libère le moteur. Dispose arrête un aperçu en cours, et il n'y a aucun fichier à terminer.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes de caméra et de format se remplissent à partir des périphériques. Start lance l'aperçu. La C920 met quelques secondes à envoyer sa première image, puis l'image apparaît. Cliquez sur Save Frame. Le JPEG s'ouvre dans Photos. Sa taille est de 1920 par 1080, le format de la caméra, alors que l'aperçu sur le formulaire est bien plus petit. Fermez Photos, arrêtez l'aperçu, et fermez la fenêtre.

### Erreurs fréquentes

Quelques erreurs fréquentes. Audio_PlayAudio laissé activé sans source audio sélectionnée, si bien qu'OnError signale une source audio manquante. Enregistrer avant Start, ou avant que la caméra envoie sa première image, si bien que Frame_SaveAsync renvoie false. Un seul nom de fichier pour tous les clics, si bien que chaque image remplace la précédente. Et une valeur de qualité avec PNG ou BMP, où elle est ignorée.

### Étapes suivantes

Ensuite, la vidéo webcam vers MP4 enregistre ce que vous voyez dans l'aperçu, et la vidéo de texte sur la vidéo y dessine une légende, une horloge et des numéros d'image. Pour travailler sur une image en mémoire plutôt que dans un fichier, Frame_GetCurrent la renvoie sous forme de Bitmap. Et l'événement OnVideoFrameBitmap vous remet chaque image dès qu'elle arrive. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Webcam vers MP4](video-capture-webcam-mp4.md) — enregistrer la même caméra dans un fichier MP4
- [Texte sur une webcam](webcam-capture-text-overlay.md) — dessiner une légende, une horloge et des numéros d'image sur la vidéo
- [Enregistrement pré-événement](../guides/pre-event-recording.md) — garder la vidéo de la caméra en mémoire tampon et enregistrer des clips sur détection de mouvement ou déclenchement manuel

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et d'exemples d'implémentation.
