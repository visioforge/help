---
title: Enregistrer l'écran en WMV en C# WinForms (WMV 9, WMA, CBR)
description: "Enregistrez l'écran et le son des haut-parleurs en WMV en C# WinForms avec VideoCaptureCore : codecs Windows Media à débit constant, ffprobe."
sidebar_label: Capture d'écran en WMV
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - ScreenCaptureSourceSettings
  - AudioCaptureSource
  - WMVOutput
  - VideoCaptureCoreX
  - VideoView
video:
  id: gpaVniBKXBw
  upload_date: 2026-11-16T06:07:30Z
  duration: PT13M21S

---

# Guide d'implémentation de capture d'écran en WMV en C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel enregistre un moniteur entier et le son des haut-parleurs dans un fichier WMV depuis une application C# WinForms avec le moteur VideoCaptureCore. Le moniteur, la fréquence d'images et le curseur de la souris se choisissent sur le formulaire, et le son vient de la source VisioForge What You Hear. La sortie est un `WMVOutput` aux réglages personnalisés : Windows Media Video 9 à 10 Mbit/s constants et un audio WMA, codecs et format audio choisis dans les listes que fournit Windows.

Cette partie utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK. VideoCaptureCoreX écrit aussi du WMV : voir [VideoCaptureCoreX](#api-moderne-video-capture-sdk-x) plus bas.

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/gpaVniBKXBw?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/screen-capture-wmv-winforms)

## Quand utiliser WMV pour l'enregistrement d'écran

WMV (Windows Media Video) utilise les codecs Windows Media de Microsoft et le conteneur ASF. Il reste utile dans des scénarios précis centrés sur Windows :

- **Lecture native sous Windows** - les fichiers WMV se lisent sous Windows sans rien installer, et les encodeurs Windows Media sont fournis avec Windows : ni GPU ni pack de codecs
- **Une taille de fichier connue** - à débit constant, la taille du fichier a un plafond connu avant même d'enregistrer
- **Des fichiers plus petits qu'en AVI** - dans la vidéo, un écran 4K à 15 images par seconde occupe environ 33 Mo par minute ; dans le [tutoriel AVI](screen-capture-avi.md), un écran 4K à seulement 10 images par seconde en MJPEG occupe environ 30 Mo par seconde
- **Environnements d'entreprise anciens** - de nombreuses entreprises standardisent les formats Windows Media pour la diffusion interne de vidéos

**Contrepartie :** WMV est un format centré sur Windows, peu pris en charge sous macOS, Linux et sur les téléphones. Pour la compatibilité multiplateforme, [MP4 est le format recommandé](screen-capture-mp4.md).

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et le redist de capture vidéo. Ses filtres DirectShow comprennent la source d'écran et la source sonore What You Hear ; les encodeurs WMV sont fournis avec Windows :

- Pour les applications 64 bits : [redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Pour les applications 32 bits : [redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Réglez `PlatformTarget` dans le fichier projet selon le redist (`x64` ou `x86`). Sur les éditions N de Windows, installez le Media Feature Pack pour disposer des encodeurs Windows Media.

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des listes déroulantes pour le moniteur, la fréquence d'images, le périphérique audio, le codec vidéo, le codec audio et le format audio, les cases « Mouse cursor » et « Record sound » (toutes deux cochées), et les boutons Start et Stop ; le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, `Form1.cs` se compile sans modification dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>` ; dans `Program.cs`, remplacez `ApplicationConfiguration.Initialize()` par `Application.EnableVisualStyles()` et `Application.SetCompatibleTextRenderingDefault(false)`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace ScreenCaptureWmv;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1;

    public Form1()
    {
        InitializeComponent();
    }

    // Le moteur désigne un moniteur par le N de son nom de périphérique, \\.\DISPLAYN
    private static int MonitorNumber(Screen screen) =>
        int.Parse(screen.DeviceName.Replace(@"\\.\DISPLAY", string.Empty));

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Afficher les erreurs du SDK dans la barre de titre
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var screen in Screen.AllScreens)
            cbMonitor.Items.Add(MonitorNumber(screen));

        cbMonitor.SelectedItem = MonitorNumber(Screen.PrimaryScreen);

        cbFrameRate.Items.AddRange(new object[] { 5, 10, 15, 25, 30 });
        cbFrameRate.SelectedItem = 15;

        // « VisioForge What You Hear Source » enregistre ce que jouent les haut-parleurs
        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbSound.Items.Add(device.Name);

        cbSound.SelectedItem = "VisioForge What You Hear Source";
        if (cbSound.SelectedIndex < 0 && cbSound.Items.Count > 0) cbSound.SelectedIndex = 0;
        cbRecordSound.Enabled = cbAudioCodec.Enabled = cbAudioFormat.Enabled = cbSound.Items.Count > 0;
        cbRecordSound.CheckedChanged += (_, _) => cbAudioCodec.Enabled = cbAudioFormat.Enabled = cbRecordSound.Checked; // les réglages audio ont besoin du son

        // Les codecs Windows Media capables d'encoder à débit constant
        foreach (var codec in videoCapture1.WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR))
            cbVideoCodec.Items.Add(codec);

        foreach (var codec in videoCapture1.WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR))
            cbAudioCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "Windows Media Video 9";
        if (cbVideoCodec.SelectedIndex < 0 && cbVideoCodec.Items.Count > 0) cbVideoCodec.SelectedIndex = 0;
        cbAudioCodec.SelectedItem = "Windows Media Audio 9.2";
        if (cbAudioCodec.SelectedIndex < 0 && cbAudioCodec.Items.Count > 0) cbAudioCodec.SelectedIndex = 0;
    }

    private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Les fréquences d'échantillonnage, canaux et débits que propose ce codec audio
        cbAudioFormat.Items.Clear();
        foreach (var format in videoCapture1.WMV_CustomProfile_AudioFormats(cbAudioCodec.Text, WMVStreamMode.CBR))
            cbAudioFormat.Items.Add(format);

        var index = cbAudioFormat.Items.IndexOf("CBR 128 kbps, 44100 Hz, Stereo, 16 bit");
        if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        var recordSound = cbRecordSound.Checked && cbSound.SelectedItem is not null;
        if (recordSound && cbAudioFormat.SelectedItem is null)
        {
            Text = "No audio format selected";
            return;
        }

        Text = "Screen to WMV";

        // Le moniteur entier, à la fréquence d'images de la liste
        var frameRate = (int)cbFrameRate.SelectedItem;
        videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
        {
            FullScreen = true,
            AllowDesktopDuplicationEngine = true, // DXGI Desktop Duplication : plus rapide que GDI
            DisplayIndex = (int)cbMonitor.SelectedItem,
            GrabMouseCursor = cbCursor.Checked,
            FrameRate = new VideoFrameRate(frameRate)
        };

        videoCapture1.Audio_RecordAudio = recordSound;
        // Aucun périphérique sans le son : un périphérique What You Hear laissé ici ouvre quand même les haut-parleurs
        videoCapture1.Audio_CaptureDevice = recordSound ? new AudioCaptureSource(cbSound.Text) : null;
        videoCapture1.Audio_PlayAudio = false; // ne jamais rejouer le son capturé dans les haut-parleurs

        var wmv = new WMVOutput
        {
            Mode = WMVMode.CustomSettings,

            Custom_Video_StreamPresent = true,
            Custom_Video_Codec = cbVideoCodec.Text,
            Custom_Video_Mode = WMVStreamMode.CBR,
            Custom_Video_Bitrate = 10000, // kbit/s
            Custom_Video_SizeSameAsInput = true, // la résolution du moniteur
            Custom_Video_FrameRate = frameRate,
            Custom_Video_KeyFrameInterval = 2, // secondes

            Custom_Audio_StreamPresent = recordSound,
            Custom_Audio_Codec = cbAudioCodec.Text,
            Custom_Audio_Format = cbAudioFormat.Text,
            Custom_Audio_Mode = WMVStreamMode.CBR,
        };

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
        videoCapture1.Output_Format = wmv;
        videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

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
        // Terminer le WMV avant que la fenêtre ne disparaisse
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

1. **Moniteur** - `DisplayIndex` prend le numéro qui termine le nom de périphérique Windows du moniteur, `\\.\DISPLAY1` → `1` ; `MonitorNumber` le lit dans `Screen.DeviceName`, et le moniteur principal est sélectionné
2. **Source d'écran** - `FullScreen` enregistre le moniteur entier à sa pleine résolution, et `AllowDesktopDuplicationEngine` capture via Windows Desktop Duplication au lieu de GDI. `GrabMouseCursor` est désactivé par défaut et `FrameRate` vaut 10 par défaut ; l'application enregistre à 15
3. **Son des haut-parleurs** - `VisioForge What You Hear Source`, fourni par le redist de capture vidéo, enregistre ce que jouent les haut-parleurs. Avec Record sound décoché, l'application passe `Audio_RecordAudio` à `false` et efface aussi `Audio_CaptureDevice`, et `Audio_PlayAudio` reste à `false` pour que le son enregistré ne soit pas rejoué dans les haut-parleurs
4. **Codecs Windows Media** - `WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR)` et `WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR)` listent les encodeurs Windows Media qui travaillent à débit constant, et `WMV_CustomProfile_AudioFormats(codec, WMVStreamMode.CBR)` liste les formats d'un codec audio. Prenez les noms dans ces listes plutôt que de les saisir. Windows Media Video 9 enregistre un écran 4K
5. **Réglages personnalisés** - `new WMVOutput()` seul utilise un profil intégré ; `Mode = WMVMode.CustomSettings` prend à la place les valeurs `Custom_Video_*` et `Custom_Audio_*`. `Custom_Video_Bitrate` est en kbit/s, donc `10000` correspond à 10 Mbit/s ; `Custom_Video_SizeSameAsInput` garde la résolution du moniteur, et `Custom_Video_KeyFrameInterval` est en secondes. Le flux audio n'est écrit que si `Custom_Audio_StreamPresent` vaut `true`
6. **Fonctionnement asynchrone** - `StartAsync` et `StopAsync` gardent l'interface réactive, et la fermeture du formulaire termine d'abord le fichier WMV

## Vérifier le fichier avec ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.wmv -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=wmv3|width=3840|height=2160
# stream|codec_name=wmav2|sample_rate=44100|channels=2
# format|format_name=asf|duration=16.731000|size=9157531
```

Le conteneur est ASF, le format derrière WMV. `wmv3` est le nom que ffprobe donne à Windows Media Video 9, et `wmav2` est le flux WMA. Dix-sept secondes d'un écran 4K font 9,2 mégaoctets (8,7 Mo selon l'Explorateur Windows), soit 4,4 Mbit/s : 10000 kbit/s est un plafond, et un bureau qui bouge à peine demande moins.

## Video Capture SDK X (VideoCaptureCoreX) { #api-moderne-video-capture-sdk-x }

L'API multiplateforme utilise `VideoCaptureCoreX`. Pour le modèle complet d'application console avec configuration de capture d'écran, configuration audio et cycle de vie d'enregistrement, consultez le guide [Capture d'écran en MP4](screen-capture-mp4.md#api-moderne-video-capture-sdk-x). Pour produire du WMV au lieu de MP4, remplacez la configuration de sortie :

```csharp
using VisioForge.Core.Types.X.Output;

// Sortie WMV (codecs Windows Media Video + WMA audio)
var wmvOutput = new WMVOutput(outputPath);
videoCapture.Outputs_Add(wmvOutput, autostart: true);
```

La capture de région, l'enregistrement multi-écrans, l'audio, la mise en surbrillance du curseur et les options d'encodage GPU sont traités dans le [guide MP4](screen-capture-mp4.md) — toutes les fonctionnalités de configuration de la source fonctionnent à l'identique avec la sortie WMV.

## Questions fréquentes

### Quand utiliser WMV plutôt que MP4 pour l'enregistrement d'écran ?

Choisissez WMV lorsque votre public utilise Windows et que vous avez besoin d'une lecture native sans codecs supplémentaires, lorsque la taille du fichier doit rester sous un plafond connu, ou dans des environnements d'entreprise qui standardisent les formats Windows Media. Pour la diffusion multiplateforme ou la publication web, MP4 avec H.264 est le meilleur choix : plus d'appareils compatibles et une meilleure compression.

### Pourquoi mon fichier WMV n'a-t-il pas de son ?

Vérifiez que « Record sound » est activé et que `VisioForge What You Hear Source` est sélectionné, pas un micro : il enregistre ce que jouent les haut-parleurs et il est fourni par le redist de capture vidéo. Avec le son activé, `Custom_Audio_StreamPresent` doit valoir `true` et `Custom_Audio_Format` doit être un nom renvoyé par `WMV_CustomProfile_AudioFormats` pour le codec audio sélectionné.

### Comment régler le débit du WMV ?

Réglez `Mode = WMVMode.CustomSettings`, `Custom_Video_Mode = WMVStreamMode.CBR` et `Custom_Video_Bitrate` en kilobits par seconde : `10000` correspond à 10 Mbit/s. Le débit audio fait partie du nom du format audio, par exemple `CBR 128 kbps, 44100 Hz, Stereo, 16 bit`.

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons l'écran et le son des haut-parleurs dans un fichier WMV depuis une application C# WinForms avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une application WinForms qui enregistre un moniteur entier dans un fichier WMV. Le moniteur, la fréquence d'images et le curseur de la souris se choisissent sur le formulaire. Elle enregistre aussi le son qui sort des haut-parleurs, en audio WMA. La vidéo est en Windows Media Video 9, à un débit constant que nous fixons nous-mêmes : dix mégabits par seconde. Et les codecs et le format audio viennent des listes que fournit Windows. Voici l'application terminée, qui enregistre l'écran sur lequel elle tourne.

### WMV, AVI ou MP4

Quand choisir WMV ? Un fichier WMV se lit sous Windows sans rien installer. Les encodeurs Windows Media sont fournis avec Windows : vous n'avez besoin ni de GPU ni de pack de codecs. Avec un débit constant, la taille du fichier a un plafond connu avant même d'enregistrer. Et il est léger : ici environ trente mégaoctets par minute, pour un écran 4K à quinze images par seconde. Le même écran en AVI avec MJPEG prenait environ vingt-huit mégaoctets par seconde. Pour le web, les téléphones et les Mac, le MP4 en H.264 est le meilleur choix. C'est la vidéo capture d'écran vers MP4.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Cette fois, ni caméra ni micro. VideoCaptureCore est le moteur DirectShow du SDK : cette application ne fonctionne que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet ScreenCaptureWmv. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet : le SDK lui-même, et le redist de capture vidéo. Il contient les filtres DirectShow, dont la source d'écran et la source sonore What You Hear. Les encodeurs WMV sont fournis avec Windows, il ne faut donc rien de plus. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.Core et installez-le. Puis installez le redist de capture vidéo pour x64. Le redist est celui pour x64 : réglez donc PlatformTarget sur x64 dans le fichier du projet. Une application x86 prend le redist x86 à la place.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView, où l'application affiche l'aperçu de ce qu'elle enregistre. Ajoutez une étiquette et une liste déroulante pour le moniteur, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour la fréquence d'images, le périphérique audio, le codec vidéo, le codec audio et le format audio. Ajoutez deux cases à cocher, toutes deux cochées : le curseur de la souris et Record sound. Puis les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire, sur la liste des codecs audio et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu est dessiné. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles s'affichent dans la barre de titre.

### Remplir les listes

Le moteur désigne un moniteur par le numéro à la fin de son nom de périphérique Windows : un pour DISPLAY1, deux pour DISPLAY2. Une petite méthode utilitaire lit ce numéro. Le gestionnaire Load liste tous les moniteurs et sélectionne le moniteur principal. La liste des fréquences d'images contient quelques valeurs fixes. Quinze images par seconde sont sélectionnées. Audio_CaptureDevices liste les périphériques audio. VisioForge What You Hear Source enregistre tout ce qui est joué sur les haut-parleurs : il est donc sélectionné quand il est présent. Sans aucun périphérique audio, Record sound et les deux listes audio sont désactivés. Et le codec et le format audio ne comptent qu'avec du son, ils suivent donc Record sound.

### Codecs Windows Media

WMV_CustomProfile_VideoCodecs et WMV_CustomProfile_AudioCodecs listent les encodeurs Windows Media capables de travailler à débit constant. Nous remplissons les deux listes et sélectionnons Windows Media Video 9 et Windows Media Audio 9.2, ou le premier codec quand l'un d'eux manque. Quand le codec audio change, WMV_CustomProfile_AudioFormats liste ce que ce codec propose. Nous prenons 128 kilobits par seconde, 44,1 kilohertz stéréo : la fréquence que fournit What You Hear ici, donc rien n'est rééchantillonné. Sur ce PC, la liste vidéo compte sept entrées : Windows Media Video 9, son Advanced Profile, les codecs Screen et Image, et les anciens V7 et V8. Windows Media Video 9 enregistre cet écran 4K, c'est donc notre choix par défaut. Pour l'audio à débit constant, il y a WMA 9.2 et WMA 10 Professional. Chaque format audio indique son débit, sa fréquence d'échantillonnage, ses canaux et ses bits. Choisissez toujours ces noms dans les listes, ne les tapez jamais à la main.

### Capturer l'écran

Le gestionnaire Start vérifie d'abord qu'un codec vidéo est sélectionné. Sinon, il l'indique dans la barre de titre. Le son n'est enregistré que si Record sound est coché et qu'un périphérique est sélectionné. Il faut alors aussi un format audio. Ensuite, il décrit la source d'écran. FullScreen enregistre le moniteur entier. AllowDesktopDuplicationEngine capture via Windows Desktop Duplication au lieu de GDI. DisplayIndex est le numéro du moniteur choisi dans la liste, GrabMouseCursor dessine le curseur dans la vidéo, et FrameRate est la fréquence choisie dans la liste. Petit récapitulatif. FullScreen enregistre le moniteur entier à sa pleine résolution. DisplayIndex est le numéro tiré du nom de périphérique du moniteur. Le curseur est désactivé par défaut, et la fréquence d'images est de dix. Cette vidéo enregistre un écran 4K à quinze images par seconde.

### Enregistrer le son des haut-parleurs

Avec le son activé, l'application enregistre le périphérique sélectionné. Sans le son, le périphérique est aussi effacé : un périphérique What You Hear laissé en place ouvre quand même les haut-parleurs. Audio_PlayAudio reste désactivé : le son enregistré n'est pas rejoué dans les haut-parleurs d'où il vient.

### WMVOutput : réglages personnalisés

Passons à la sortie. Un WMVOutput en mode CustomSettings prend nos propres réglages au lieu d'un profil intégré. Le flux vidéo utilise le codec de la liste à 10000 kilobits par seconde constants. SizeSameAsInput garde la résolution du moniteur, la fréquence d'images est celle que nous avons choisie, et il y a une image clé toutes les deux secondes. Le flux audio prend le codec et le format dans les listes, et il n'est présent que si le son est activé. Écrivez ensuite output.wmv dans le dossier Vidéos, et passez le moteur en mode capture d'écran. Un simple WMVOutput utilise un profil intégré. CustomSettings nous donne notre propre codec, débit et taille. Custom_Video_Bitrate est en kilobits par seconde, donc 10000 signifie dix mégabits. C'est un plafond : un bureau qui bouge à peine demande moins, et cet enregistrement est arrivé à environ 4,4 mégabits par seconde. Et une image clé toutes les deux secondes donne au lecteur un point de recherche toutes les deux secondes.

### Démarrer et arrêter

Start et Stop sont asynchrones. StartAsync revient dès que la capture tourne, et StopAsync termine l'écriture du fichier WMV.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait le WMV inachevé. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant l'enregistrement, il annule la fermeture. Puis il arrête l'enregistrement, et ferme de nouveau la fenêtre une fois le gestionnaire terminé. Quand rien n'est enregistré, le gestionnaire libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes se remplissent : le moniteur, la fréquence d'images, le périphérique audio, les deux codecs et le format audio. La liste des formats audio contient chaque débit et chaque fréquence d'échantillonnage que propose le codec. Le nôtre reste sélectionné. Un son est joué sur les haut-parleurs, et Start lance l'enregistrement. L'aperçu montre l'écran avec le curseur. Stop termine output.wmv, puis nous fermons la fenêtre.

### Vérifier le fichier avec ffprobe

Le fichier arrive dans Vidéos : moins de neuf mégaoctets pour dix-sept secondes d'un écran 4K. ffprobe montre ce qu'il contient. Le conteneur est ASF, le format qui se cache derrière WMV. La vidéo est en Windows Media Video 9, que ffprobe appelle wmv3, en 3840x2160. L'audio est en WMA, 44,1 kilohertz stéréo : le son des haut-parleurs.

### Erreurs fréquentes

Quelques erreurs fréquentes. Un paquet redist manquant : le moteur ne démarre pas. Un fichier muet, parce que Record sound était décoché, ou qu'un micro était sélectionné au lieu de What You Hear. Des noms de codec et de format tapés à la main : prenez-les dans les listes. Custom_Video_Bitrate écrit en bits par seconde : il est en kilobits. Et fermer la fenêtre en plein enregistrement : attendez d'abord StopAsync avec await.

### Étapes suivantes

Ensuite, la vidéo sur la capture d'écran en MP4 enregistre l'écran dans des fichiers H.264 lisibles partout. La vidéo sur la capture d'écran en AVI garde chaque image entière, pour le montage. Et la vidéo webcam vers WMV enregistre une caméra avec les mêmes réglages Windows Media qu'ici. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Capture d'écran en MP4](screen-capture-mp4.md) — format recommandé avec couverture complète des fonctionnalités (région, multi-écrans, audio, encodage GPU, multiplateforme)
- [Capture d'écran en AVI](screen-capture-avi.md) — format AVI avec MJPEG pour l'édition image par image
- [Webcam en WMV](video-capture-webcam-wmv.md) — les mêmes réglages Windows Media avec une caméra et un micro
- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — enregistrement d'écran en Visual Basic .NET
- [Configuration de la source d'écran](../video-sources/screen.md) — référence complète des paramètres de capture
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
