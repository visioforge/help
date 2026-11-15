---
title: Enregistrer une webcam en WMV en C# WinForms (WMV 9, WMA)
description: "Enregistrez une webcam en WMV depuis une app C# WinForms avec VideoCaptureCore : Windows Media Video 9 à débit constant, audio WMA, vérification ffprobe."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - WMV
  - WMA
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - WMVOutput
  - WMVStreamMode
  - VideoView
video:
  id: EoOfXkhOtP8
  upload_date: 2026-11-14T07:13:25Z
  duration: PT12M20S

---

# Enregistrement de webcam vers un fichier WMV dans les applications .NET C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel enregistre une webcam dans un fichier WMV depuis une application C# WinForms avec le moteur VideoCaptureCore. La vidéo est en Windows Media Video 9, à la résolution propre de la caméra et à 8 Mbit/s constants ; l'audio est en WMA, dans un format choisi dans la liste que renvoie le SDK.

**Quand choisir WMV plutôt que MP4 :** un fichier WMV se lit sous Windows sans rien installer, et les encodeurs Windows Media sont fournis avec Windows : pas besoin de GPU ni de pack de codecs. Avec un débit constant, la taille du fichier est connue avant l'enregistrement, et certains systèmes attendent déjà des fichiers WMV ou ASF. Pour le web, les téléphones et les Mac, enregistrez plutôt en [MP4 avec H.264](video-capture-webcam-mp4.md).

Ce tutoriel utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK. VideoCaptureCoreX, le moteur multiplateforme, a son propre encodeur WMV : consultez [Sortie WMV multiplateforme](../../general/output-formats/wmv.md#sortie-wmv-multiplateforme) et [Enregistrer la vidéo d'une webcam en C#](../guides/save-webcam-video.md).

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/EoOfXkhOtP8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, se trouve sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-wmv-winforms)

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et le redist de capture vidéo avec ses filtres DirectShow. Les encodeurs Windows Media sont fournis avec Windows, il ne faut donc rien d'autre :

- Pour les applications 64 bits : [redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Pour les applications 32 bits : [redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Réglez `PlatformTarget` dans le fichier projet selon le redist (`x64` ou `x86`). Sur les éditions N de Windows, les composants Windows Media sont fournis avec le Media Feature Pack : installez-le d'abord.

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des listes déroulantes pour la caméra, le format, la fréquence d'images, le micro, le codec vidéo, le codec audio et le format audio, et les boutons Start et Stop ; le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, le même code se compile dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamWmv;

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

        // Les codecs Windows Media capables d'encoder à débit constant
        foreach (var codec in videoCapture1.WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR))
            cbVideoCodec.Items.Add(codec);

        foreach (var codec in videoCapture1.WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR))
            cbAudioCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "Windows Media Video 9";
        cbAudioCodec.SelectedItem = "Windows Media Audio 9.2";
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

    private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Les fréquences d'échantillonnage, canaux et débits que propose ce codec audio
        cbAudioFormat.Items.Clear();
        foreach (var format in videoCapture1.WMV_CustomProfile_AudioFormats(cbAudioCodec.Text, WMVStreamMode.CBR))
            cbAudioFormat.Items.Add(format);

        var index = cbAudioFormat.Items.IndexOf("CBR 128 kbps, 48000 Hz, Stereo, 16 bit");
        if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbFrameRate.SelectedItem is null)
        {
            Text = "No camera, format or frame rate";
            return;
        }

        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        var mic = (string)cbMicrophone.SelectedItem;
        if (mic is not null && cbAudioFormat.SelectedItem is null)
        {
            Text = "No audio format selected";
            return;
        }

        Text = "Webcam to WMV";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        var frameRate = (VideoFrameRate)cbFrameRate.SelectedItem;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = frameRate;

        videoCapture1.Audio_RecordAudio = mic is not null; // pas de micro : enregistrer la vidéo seule
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // enregistrer le micro sans le relire

        var wmv = new WMVOutput
        {
            Mode = WMVMode.CustomSettings,

            Custom_Video_StreamPresent = true,
            Custom_Video_Codec = cbVideoCodec.Text,
            Custom_Video_Mode = WMVStreamMode.CBR,
            Custom_Video_Bitrate = 8000, // kbit/s
            Custom_Video_SizeSameAsInput = true, // la résolution de la caméra
            Custom_Video_FrameRate = frameRate.Value,
            Custom_Video_KeyFrameInterval = 2, // secondes

            Custom_Audio_StreamPresent = mic is not null,
            Custom_Audio_Codec = cbAudioCodec.Text,
            Custom_Audio_Format = cbAudioFormat.Text,
            Custom_Audio_Mode = WMVStreamMode.CBR,
        };

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
        videoCapture1.Output_Format = wmv;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Terminer le WMV avant la fermeture de la fenêtre
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // démarrage ou arrêt déjà en cours

            btStop.Enabled = false;
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

1. **Choix du périphérique et du format** - La caméra, son format, sa fréquence d'images et le micro sont choisis dans des listes remplies à partir des périphériques eux-mêmes ; sans micro, `Audio_RecordAudio` et `Custom_Audio_StreamPresent` valent `false` et le WMV ne contient que la vidéo
2. **Format de la caméra** - `Format_UseBest` est activé par défaut : il choisit la résolution la plus élevée jusqu'à 1080p, puis préfère MJPG et la fréquence d'images la plus rapide. Pour enregistrer en 4K, ou dans le format de votre choix, désactivez-le et réglez `Format` et `FrameRate`
3. **Codecs Windows Media** - `WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR)` et `WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR)` renvoient les encodeurs Windows Media capables de travailler à débit constant, et `WMV_CustomProfile_AudioFormats(codec, WMVStreamMode.CBR)` les formats d'un codec audio, chacun nommé par son débit, sa fréquence d'échantillonnage, ses canaux et ses bits. Windows Media Video 9 est le codec pour la vidéo de caméra ; les codecs Screen et Image sont faits pour les enregistrements d'écran et les images fixes, Advanced Profile est le profil VC-1 conçu pour la vidéo entrelacée et la diffusion, et V7 et V8 sont d'anciennes versions pour les anciens lecteurs. Prenez chaque nom de codec et de format dans ces listes, ne le tapez jamais à la main
4. **Réglages personnalisés** - Un `new WMVOutput()` simple utilise le profil intégré « Windows Media Video 9 for Local Network (768 kbps) ». `WMVMode.CustomSettings` accepte votre propre codec, débit et taille : `Custom_Video_Bitrate` est en kilobits par seconde, donc `8000` correspond à 8 Mbit/s, `Custom_Video_SizeSameAsInput` garde la résolution de la caméra et `Custom_Video_KeyFrameInterval` est en secondes. Avec un audio à 128 kbit/s, cela fait environ soixante mégaoctets par minute
5. **Fonctionnement asynchrone** - `StartAsync` et `StopAsync` gardent l'interface réactive, et la fermeture du formulaire termine d'abord le fichier WMV

## Vérifier le fichier avec ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.wmv -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=wmv3|width=1920|height=1080
# stream|codec_name=wmav2|sample_rate=48000|channels=2
# format|format_name=asf|duration=16.496000|size=16941531
```

Seize secondes et demie pèsent 16 941 531 octets, environ 16 Mo dans l'Explorateur de fichiers : 8 Mbit/s de vidéo plus 128 kbit/s d'audio. ffprobe appelle WMV 9 `wmv3` et WMA 9.2 `wmav2`, et le conteneur est ASF, le format derrière WMV.

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons une webcam dans un fichier WMV depuis une application C# WinForms avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une fenêtre WinForms avec un aperçu en direct de la caméra. Elle enregistre du Windows Media Video 9 à la résolution de la caméra, avec un débit constant que nous fixons nous-mêmes : huit mégabits par seconde. L'audio est en WMA, dans un format choisi dans une liste. La caméra, son format, la fréquence d'images et le micro se choisissent aussi dans des listes. Voici l'application terminée, qui enregistre depuis une Logitech C920.

### WMV ou MP4

Quand choisir WMV ? Un fichier WMV se lit sous Windows sans rien installer. Les encodeurs Windows Media sont fournis avec Windows : vous n'avez besoin ni de GPU ni de pack de codecs. Avec un débit constant, la taille du fichier est connue avant l'enregistrement. Et certains systèmes attendent déjà des fichiers WMV ou ASF. Pour le web, les téléphones et les Mac, le MP4 en H.264 est le meilleur choix. C'est la vidéo webcam vers MP4.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Et une webcam avec un micro. VideoCaptureCore est le moteur DirectShow du SDK : cette application ne fonctionne que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet WebcamWmv. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet : le SDK lui-même, et le redist de capture vidéo avec ses filtres DirectShow. Les encodeurs WMV sont fournis avec Windows, il ne faut donc rien de plus. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.Core et installez-le. Puis installez le redist de capture vidéo pour x64. Le redist est celui pour x64 : réglez donc PlatformTarget sur x64 dans le fichier du projet. Une application x86 prend le redist x86 à la place.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'aperçu en direct. Ajoutez une étiquette et une liste déroulante pour la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour le format de la caméra, la fréquence d'images et le micro, puis pour le codec vidéo, le codec audio et le format audio. Ajoutez les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire, sur les listes de caméra, de format et de codec audio, et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu en direct s'affiche. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles s'affichent dans la barre de titre.

### Lister les périphériques

Video_CaptureDevices renvoie toutes les caméras, et Audio_CaptureDevices tous les micros. Nous listons leurs noms et sélectionnons le premier de chaque liste. Quand la caméra change, son gestionnaire remplit la liste des formats avec ceux de cette caméra, et choisit 1920x1080 MJPG s'il existe. Quand le format change, son gestionnaire remplit de la même façon la liste des fréquences d'images, et choisit trente images par seconde.

### Codecs Windows Media

De retour dans le gestionnaire Load, WMV_CustomProfile_VideoCodecs et WMV_CustomProfile_AudioCodecs listent les encodeurs Windows Media capables de travailler à débit constant. Nous remplissons les deux listes et sélectionnons Windows Media Video 9 et Windows Media Audio 9.2. Quand le codec audio change, WMV_CustomProfile_AudioFormats liste ce que ce codec propose. Nous choisissons 128 kilobits par seconde, 48 kilohertz stéréo. Windows Media Video 9 est le codec pour la vidéo de caméra. Les codecs Screen et Image sont faits pour les enregistrements d'écran et les images fixes, et V7 et V8 sont d'anciennes versions, pour d'anciens lecteurs. Pour l'audio à débit constant, il y a WMA 9.2 et WMA 10 Professional. Chaque format audio indique son débit, sa fréquence d'échantillonnage, ses canaux et ses bits. Choisissez toujours ces noms dans les listes, ne les tapez jamais à la main.

### Choisir le format de la caméra

Le gestionnaire de Start vérifie d'abord qu'une fréquence d'images et un codec vidéo sont sélectionnés. Sinon, il l'indique dans la barre de titre. S'il y a un micro dans la liste, il faut aussi un format audio. Puis il sélectionne la caméra par son nom, désactive Format_UseBest, et transmet le format et la fréquence d'images choisis dans les listes. Format_UseBest est activé par défaut. Il choisit la résolution la plus haute jusqu'à 1080p, puis préfère MJPG et la fréquence d'images la plus rapide. Pour enregistrer en 4K, ou dans le format de votre choix, désactivez-le et réglez Format et FrameRate, comme ici.

### Sélectionner le micro

Sans aucun micro dans la liste, l'application capture uniquement la vidéo. Sinon, elle enregistre celui qui est sélectionné. Audio_PlayAudio est désactivé : le micro n'est pas joué dans les haut-parleurs pendant l'enregistrement.

### WMVOutput : réglages personnalisés

Passons à la sortie. Un WMVOutput en mode CustomSettings prend nos propres réglages. Le flux vidéo utilise le codec de la liste à 8000 kilobits par seconde constants. SizeSameAsInput garde la résolution de la caméra, la fréquence d'images est celle que nous avons choisie, et il y a une image clé toutes les deux secondes. Le flux audio prend le codec et le format dans les listes, et n'existe que si nous avons un micro. Écrivez ensuite output.wmv dans le dossier Vidéos et passez le moteur en mode capture vidéo, pour qu'il enregistre et ne fasse pas qu'afficher l'aperçu. Un WMVOutput seul utilise un profil intégré à 768 kilobits par seconde, prévu pour un réseau local. CustomSettings nous donne notre propre codec, débit et taille. Custom_Video_Bitrate est en kilobits par seconde : 8000 signifie donc huit mégabits. Avec 128 kilobits d'audio, cela fait environ soixante mégaoctets par minute. Et une image clé toutes les deux secondes donne au lecteur un point de recherche toutes les deux secondes.

### Démarrer et arrêter

Start et Stop sont asynchrones. StartAsync revient dès que la capture tourne, et StopAsync termine l'écriture du fichier WMV.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait le WMV inachevé. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant l'enregistrement, il annule la fermeture. Puis il arrête l'enregistrement, et ferme de nouveau la fenêtre une fois le gestionnaire terminé. Quand rien n'est enregistré, le gestionnaire libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes de la caméra, du format et du micro se remplissent à partir des périphériques. La liste des codecs vidéo affiche les encodeurs Windows Media. Windows Media Video 9 reste sélectionné. Start lance l'enregistrement. La C920 met quelques secondes à envoyer sa première image. Stop termine output.wmv, puis nous fermons la fenêtre.

### Vérifier le fichier avec ffprobe

Le fichier arrive dans Vidéos : environ seize mégaoctets pour seize secondes. ffprobe montre ce qu'il contient. Le conteneur est ASF, le format qui se cache derrière WMV. La vidéo est en WMV 9, que ffprobe appelle wmv3, en 1920x1080. L'audio est en WMA, 48 kilohertz stéréo.

### Erreurs fréquentes

Quelques erreurs fréquentes. Un paquet redist manquant : le moteur ne démarre pas. Des noms de codec et de format tapés à la main : prenez-les dans les listes. Custom_Video_Bitrate écrit en bits par seconde : il est en kilobits. Sur les éditions N de Windows, les composants Windows Media viennent avec le Media Feature Pack : installez-le d'abord. Et fermer la fenêtre en plein enregistrement : attendez d'abord StopAsync avec await.

### Étapes suivantes

Ensuite, la vidéo webcam vers MP4 enregistre la même caméra dans des fichiers H.264 lisibles partout. Il existe aussi des guides pour les caméras IP, l'enregistrement d'écran et les captures d'image. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Webcam vers MP4](video-capture-webcam-mp4.md) — la même caméra enregistrée dans des fichiers H.264 lisibles partout
- [Webcam vers AVI](video-capture-camera-avi.md) — chaque image est une image complète, pour le montage et l'analyse
- [Sortie WMV](../../general/output-formats/wmv.md) — tous les modes de WMVOutput : profils internes, personnalisés et externes

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
