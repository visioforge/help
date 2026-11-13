---
title: Texte sur une webcam en C# WinForms (date, n° d'image)
description: Incrustez un texte modifiable pendant l'enregistrement, la date, le numéro d'image et la durée d'enregistrement dans un MP4 de webcam en C# WinForms.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - MP4
  - Text Overlay
  - Video Effects
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEffectTextLogo
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4Output
  - VideoView
video:
  id: wgRV_RT-txg
  upload_date: 2026-11-12T21:13:37Z
  duration: PT13M6S

---

# Capture de webcam avec incrustation de texte en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel enregistre une webcam en MP4 depuis une application C# WinForms avec le moteur VideoCaptureCore et dessine quatre textes dans les images de la vidéo avec `VideoEffectTextLogo` :

- une légende en bas au centre, que vous pouvez modifier pendant l'enregistrement ;
- la date et l'heure dans le coin supérieur droit ;
- le numéro de l'image ;
- le temps écoulé depuis le début de l'enregistrement.

Le texte fait partie de chaque image : il apparaît dans l'aperçu en direct et dans le fichier. La date et l'heure sur chaque image sont ce dont ont besoin les enregistrements de sécurité et de preuve, le numéro d'image permet de retrouver et de comparer plus tard une image précise, et la durée d'enregistrement aligne la vidéo sur les journaux et les données de capteurs.

Ce tutoriel utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK. Pour les fondus, la rotation et les moteurs X multiplateformes, consultez [Incrustations de texte](../../general/video-effects/text-overlay.md).

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/wgRV_RT-txg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-text-overlay-winforms)

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/), le redist de capture vidéo avec ses filtres DirectShow et le redist MP4, qui écrit le fichier MP4 :

- Pour les applications 64 bits : [redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/) et [redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- Pour les applications 32 bits : [redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) et [redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)

Réglez `PlatformTarget` dans le fichier du projet selon les redists (`x64` ou `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
dotnet add package VisioForge.DotNet.Core.Redist.MP4.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des listes déroulantes pour la caméra, le format, la fréquence d'images et le micro, une zone de texte pour la légende avec un bouton Apply désactivé au départ, et les boutons Start et Stop ; le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, le même code se compile dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamTextOverlay;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1;

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

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
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

    private void AddOverlays()
    {
        videoCapture1.Video_Effects_Enabled = true;
        videoCapture1.Video_Effects_Clear();

        // Une légende en bas, modifiée avec Apply pendant l'enregistrement
        using var font = new Font("Segoe UI", 40, FontStyle.Bold); // l'effet garde sa propre copie
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "caption")
        {
            Text = tbCaption.Text,
            Font = font,
            FontColor = Color.Yellow,
            HorizontalAlignment = TextLogoHorizontalAlignment.Center,
            VerticalAlignment = TextLogoVerticalAlignment.Bottom
        });

        // La date et l'heure dans le coin supérieur droit
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime,
            DateTimeMask = "yyyy-MM-dd HH:mm:ss",
            Text = "",
            HorizontalAlignment = TextLogoHorizontalAlignment.Right,
            VerticalAlignment = TextLogoVerticalAlignment.Top
        });

        // Le numéro d'image dans le coin supérieur gauche
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "frame")
        {
            Mode = TextLogoMode.FrameNumber,
            Text = "Frame ",
            Left = 20,
            Top = 20
        });

        // Le temps depuis le début de l'enregistrement, sous le numéro d'image
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "time")
        {
            Mode = TextLogoMode.Timestamp,
            TimeStampMask = @"hh\:mm\:ss\.fff",
            Text = "Rec ",
            Left = 20,
            Top = 140 // la taille du texte suit la mise à l'échelle de Windows
        });
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam with text overlay";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // pas de micro : enregistrer la vidéo seule
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // enregistrer le micro sans le relire

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
        videoCapture1.Output_Format = new MP4Output(); // H.264 et AAC
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        AddOverlays();

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btApply.Enabled = started;
        btStart.Enabled = !started;
    }

    private void btApply_Click(object sender, EventArgs e)
    {
        // Modifier la légende pendant l'enregistrement : nouveau texte, puis Update pour la redessiner
        if (videoCapture1.Video_Effects_Get("caption") is VideoEffectTextLogo caption)
        {
            caption.Text = tbCaption.Text;
            caption.Update();
        }
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        btApply.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Terminer le MP4 avant la fermeture de la fenêtre
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // démarrage ou arrêt déjà en cours

            btStop.Enabled = false;
            btApply.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // refermer une fois ce gestionnaire terminé
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Fonctionnalités clés de l'implémentation

1. **Effets activés** - `Video_Effects_Enabled` vaut `false` par défaut, et alors aucune incrustation n'est dessinée. `Video_Effects_Clear` supprime les incrustations de l'enregistrement précédent, pour que Start puisse être relancé
2. **Placement** - `HorizontalAlignment` et `VerticalAlignment` placent une incrustation sur un bord ou au centre, quelle que soit la taille de l'image ; sans eux, `Left` et `Top` la placent en pixels de la vidéo. Sans aucun réglage, une incrustation est en Arial 36 blanc dans le coin supérieur gauche
3. **Modes** - `Mode` choisit ce qu'affiche une incrustation : `Text` dessine la propriété `Text`, `DateTime` l'horloge du PC au format de `DateTimeMask`, `Timestamp` le temps écoulé depuis le début de l'enregistrement au format de `TimeStampMask` (un format `TimeSpan`, donc `:` et `.` doivent être précédés d'une barre oblique inverse), et `FrameNumber` le numéro de l'image. Dans les trois derniers, `Text` est dessiné devant la valeur, et vaut `Hello!!!` par défaut : réglez-le sur une chaîne vide ou sur votre propre préfixe
4. **Modifier une incrustation pendant l'enregistrement** - `Video_Effects_Get` renvoie l'incrustation de ce nom, ou `null`. Modifiez ses propriétés et appelez `Update()` : le moteur la redessine à partir de l'image suivante, et l'enregistrement garde la modification à partir de cette image. Sans `Update()`, l'ancien texte reste
5. **Polices** - le setter de `Font` garde sa propre copie de la police : celle que vous passez peut être libérée aussitôt
6. **Taille du texte et mise à l'échelle de l'affichage** - une taille de police en points est dessinée à la résolution (DPI) de l'écran : elle grandit donc avec la mise à l'échelle de Windows et ne diminue pas avec l'image. À 200 %, une ligne d'Arial 36 mesure environ 110 pixels de haut, quelle que soit la taille de l'image. Cet exemple laisse cette place avec `Top = 140`, ce qui suffit jusqu'à 200 %. Pour obtenir la même taille sur tous les PC, créez la police en pixels : `new Font("Arial", 36, GraphicsUnit.Pixel)`
7. **Coût** - une incrustation dont le texte change (l'horloge, le numéro d'image, la durée d'enregistrement ou une légende après `Update()`) est redessinée à chaque image ; gardez-en peu

## Vérifier le fichier avec ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.mp4 -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=h264|width=1920|height=1080
# stream|codec_name=aac|sample_rate=48000|channels=2
# format|format_name=mov,mp4,m4a,3gp,3g2,mj2|duration=30.698667|size=4843264
```

## Transcription de la vidéo

### Ce que nous construisons

Dessinons du texte sur un enregistrement de webcam dans une application C# WinForms avec le VisioForge Video Capture SDK. Voici ce que nous construisons. Une application WinForms qui enregistre une webcam en MP4 avec un aperçu en direct. Sur la vidéo, elle dessine une légende que vous pouvez modifier pendant l'enregistrement, la date et l'heure, le numéro d'image et le temps écoulé depuis le début de l'enregistrement. Voici l'application terminée, qui enregistre depuis une Logitech C920. Le texte est dessiné dans les images de la vidéo : il apparaît dans l'aperçu et dans le fichier. À quoi cela sert-il ? La date et l'heure sur chaque image, c'est ce dont ont besoin les enregistrements de sécurité et ceux qui servent de preuve. Un numéro d'image permet de retrouver et de comparer plus tard une image précise. Le temps d'enregistrement aligne la vidéo sur les journaux et les données des capteurs. Et une légende indique ce qui est à l'écran, comme l'opérateur, le test ou le poste, et vous pouvez la modifier quand les choses changent.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou une version ultérieure, ou .NET Framework 4.7.2. Et une webcam, avec un micro si vous voulez du son. VideoCaptureCore est le moteur DirectShow du SDK : cette application ne fonctionne donc que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, puis choisissez Windows Forms App pour C#. Nommez le projet WebcamTextOverlay. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet : le SDK lui-même, le redist de capture vidéo avec ses filtres DirectShow, et le redist MP4, qui écrit le fichier MP4. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.Core et installez-le. Installez ensuite le redist de capture vidéo pour x64. Et le redist MP4 pour x64. Les deux redists sont en x64 : réglez donc PlatformTarget sur x64 dans le fichier projet. Une application x86 prend à la place les redists x86.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'aperçu en direct. Ajoutez une étiquette et une liste déroulante pour la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour le format de la caméra, la fréquence d'images et le micro. Puis une étiquette et une zone de texte pour la légende, et un bouton Apply. Apply est désactivé au départ, car il ne fonctionne que pendant l'enregistrement. Ajoutez les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire, sur les listes de caméras et de formats, et sur les trois boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Les types des incrustations de texte se trouvent dans VideoEffects. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu en direct est dessiné. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles vont dans la barre de titre.

### Lister les périphériques

Video_CaptureDevices renvoie toutes les caméras, et Audio_CaptureDevices tous les micros. Nous listons leurs noms et sélectionnons le premier de chaque liste. Quand la caméra change, son gestionnaire remplit la liste des formats avec ceux de cette caméra, et choisit 1920x1080 MJPG s'il existe. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images, et choisit trente images par seconde.

### Caméra, micro et MP4

Le gestionnaire de Start vérifie d'abord qu'une fréquence d'images est sélectionnée. Ensuite, il sélectionne la caméra par son nom, désactive Format_UseBest, et transmet le format et la fréquence d'images choisis dans les listes. Activé, Format_UseBest choisit de lui-même la résolution la plus haute jusqu'à 1080p. Sans micro, l'application ne capture que la vidéo. Sinon, elle enregistre le micro sélectionné, sans le diffuser dans les haut-parleurs. La sortie est output.mp4 dans le dossier Vidéos. Un simple MP4Output écrit de la vidéo H.264 et de l'audio AAC, encodés sur le CPU, ou avec Intel Quick Sync si le PC en dispose.

### Dessiner du texte sur la vidéo

Les incrustations ont leur propre méthode. Video_Effects_Enabled active les effets vidéo : ils sont désactivés par défaut, et alors rien n'est dessiné. Video_Effects_Clear retire les incrustations de l'enregistrement précédent, pour que Start puisse être relancé. La première incrustation est la légende : un VideoEffectTextLogo nommé caption, avec le texte de la zone de texte. C'est du Segoe UI, 40 points, en gras et en jaune. Les deux alignements la placent en bas au centre, quelle que soit la taille de l'image. L'effet garde sa propre copie de la police : la nôtre est donc libérée tout de suite. Chaque incrustation de texte est un VideoEffectTextLogo. Son nom permet de la retrouver plus tard. Text, Font et FontColor définissent ce qu'elle dessine. Left et Top la placent en pixels de la vidéo, ou les propriétés d'alignement la mettent contre un bord ou au centre. Sans eux, c'est de l'Arial blanc en 36 points, dans le coin supérieur gauche.

### Modes de texte : horloge, image, temps

Ensuite, la date et l'heure. Le mode DateTime dessine l'horloge du PC, mise en forme par DateTimeMask avec les chaînes de format .NET habituelles. Text est dessiné devant la valeur : nous le réglons donc sur une chaîne vide. Sa valeur par défaut est Hello. L'alignement Right et Top place l'horloge dans le coin supérieur droit. Mode choisit ce qu'affiche une incrustation. Text dessine la propriété Text. DateTime dessine l'horloge du PC. Timestamp dessine le temps écoulé depuis le début de l'enregistrement, mis en forme par TimeStampMask. FrameNumber dessine le numéro de l'image. Dans les trois derniers, Text se place devant la valeur. Le numéro d'image va dans le coin supérieur gauche, avec le préfixe Frame. Cette fois, Left et Top le placent, en pixels. Et en dessous, le temps d'enregistrement, avec le préfixe Rec, en heures, minutes, secondes et millisecondes. Dans un format TimeSpan, les deux-points et le point doivent être précédés d'une barre oblique inverse. Laissez de la place entre les lignes : la taille du texte suit l'échelle d'affichage de Windows, donc à 200 % il est deux fois plus haut.

### Démarrer et arrêter

De retour dans le gestionnaire de Start, AddOverlays s'exécute avant StartAsync. StartAsync rend la main une fois la capture lancée, puis Stop et Apply sont activés. Stop désactive Stop et Apply, et StopAsync termine l'écriture du fichier MP4.

### Modifier la légende pendant l'enregistrement

Apply modifie la légende pendant l'enregistrement. Video_Effects_Get trouve l'incrustation par son nom. Nous définissons le nouveau texte et appelons Update, qui demande au moteur de redessiner l'incrustation à l'image suivante. Sans Update, l'ancien texte reste. Video_Effects_Get renvoie l'incrustation que vous avez ajoutée, ou null si aucune incrustation ne porte ce nom. Vous pouvez modifier n'importe laquelle de ses propriétés, comme le texte, la couleur ou la position. Appelez ensuite Update : elle est redessinée dès l'image suivante. L'enregistrement garde la modification à partir de cette image.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait le MP4 inachevé. Dans la fenêtre Properties, onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant l'enregistrement, il annule la fermeture. Puis il arrête l'enregistrement, et ferme à nouveau la fenêtre une fois le gestionnaire terminé. Quand rien n'est enregistré, le gestionnaire libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes de caméras, de formats et de micros se remplissent avec les périphériques. Start lance l'enregistrement. La C920 met quelques secondes à envoyer sa première image. Puis la légende, l'horloge, le numéro d'image et le temps d'enregistrement apparaissent sur la vidéo. Tapez une nouvelle légende et cliquez sur Apply. Le nouveau texte s'affiche à l'image suivante. Stop termine output.mp4, puis nous fermons la fenêtre.

### Vérifier le fichier

Voici une image de output.mp4, quelques secondes après le début. La légende, l'horloge, le numéro d'image et le temps d'enregistrement font partie de l'image. Et une image après Apply. À partir de cette image, le fichier contient la nouvelle légende. ffprobe montre le contenu : de la vidéo H.264 en 1920x1080 et de l'audio AAC, dans un fichier MP4.

### Erreurs fréquentes

Quelques erreurs fréquentes. Video_Effects_Enabled laissé à false : rien n'est dessiné. Une horloge qui commence par Hello, parce que Text n'a pas été vidé. Un nouveau Text sans Update : l'ancien texte reste. Video_Effects_Get avec un autre nom, qui renvoie null. Et beaucoup d'incrustations dont le texte change : chacune est redessinée à chaque image, alors limitez-en le nombre.

### Étapes suivantes

Ensuite, la vidéo webcam vers MP4 encode sur le GPU, et la vidéo webcam vers AVI produit des fichiers faits pour le montage. Pour des images sur la vidéo, il y a VideoEffectImageLogo, et pour un bandeau défilant, VideoEffectScrollingTextLogo. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Webcam vers MP4](video-capture-webcam-mp4.md) — la même caméra enregistrée en MP4, avec encodage par GPU
- [Webcam vers AVI](video-capture-camera-avi.md) — des fichiers MJPEG et PCM pour le montage et l'analyse
- [Incrustations de texte](../../general/video-effects/text-overlay.md) — fondus, rotation et retournements, et incrustations de texte dans les moteurs X multiplateformes

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code et d'exemples d'implémentation.
