---
title: Enregistrer l'écran en AVI en C# WinForms (MJPEG, PCM, MP3)
description: "Enregistrez l'écran et le son des haut-parleurs en AVI en C# WinForms avec VideoCaptureCore : moniteur, fréquence, codec et vérification ffprobe."
sidebar_label: Capture d'écran en AVI
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - WinForms
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - MJPEG
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - ScreenCaptureSourceSettings
  - AudioCaptureSource
  - AVIOutput
  - MP3Output
  - VideoCaptureCoreX
  - MJPEGEncoderSettings
  - VideoView
video:
  id: V9iZiSx7slI
  upload_date: 2026-11-13T14:48:04Z
  duration: PT12M1S

---

# Guide d'implémentation de capture d'écran en AVI en C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel enregistre un moniteur entier et le son des haut-parleurs dans un fichier AVI depuis une application C# WinForms avec le moteur VideoCaptureCore. Le moniteur, la fréquence d'images et le curseur de la souris se choisissent sur le formulaire, le son vient de la source VisioForge What You Hear, le codec vidéo est choisi parmi ceux installés sur le PC (MJPEG par défaut), et l'audio est en PCM non compressé ou en MP3.

Cette partie utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK. Sous macOS, Linux, Android et iOS, VideoCaptureCoreX enregistre aussi en AVI : voir [VideoCaptureCoreX](#api-moderne-video-capture-sdk-x) plus bas.

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/V9iZiSx7slI?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/screen-capture-avi-winforms)

## Quand utiliser AVI pour l'enregistrement d'écran

- **Chaque image est une image complète** - avec MJPEG, vous pouvez couper et monter sur n'importe quelle image, et lire chaque image seule pour l'analyser, sans décoder les images précédentes
- **Audio sans perte** - un `AVIOutput` simple écrit du PCM non compressé
- **Outils DirectShow** - l'AVI en MJPEG s'ouvre dans les éditeurs et lecteurs Windows basés sur DirectShow

**Contrepartie :** la taille. Dans la vidéo, un écran 4K à 10 images par seconde en MJPEG occupe environ 30 Mo par seconde : 679 Mo pour 22 secondes, environ 110 Go par heure. Pour le web, les téléphones et le partage, enregistrez plutôt en [MP4 avec H.264](screen-capture-mp4.md).

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) et le redist de capture vidéo. Ses filtres DirectShow comprennent la source d'écran et la source sonore What You Hear ; AVI n'a besoin de rien d'autre :

- Pour les applications 64 bits : [redist de capture vidéo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Pour les applications 32 bits : [redist de capture vidéo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Réglez `PlatformTarget` dans le fichier projet selon le redist (`x64` ou `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des listes déroulantes pour le moniteur, la fréquence d'images, le périphérique audio et le codec vidéo, les cases « Mouse cursor » et « Record sound » (toutes deux cochées), une case « MP3 audio », et les boutons Start et Stop ; le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, `Form1.cs` se compile sans modification dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>` ; dans `Program.cs`, remplacez `ApplicationConfiguration.Initialize()` par `Application.EnableVisualStyles()` et `Application.SetCompatibleTextRenderingDefault(false)`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace ScreenCaptureAvi;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1 = null!;

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

        cbMonitor.SelectedItem = MonitorNumber(Screen.PrimaryScreen!);

        cbFrameRate.Items.AddRange(new object[] { 5, 10, 15, 25, 30 });
        cbFrameRate.SelectedItem = 10;

        // « VisioForge What You Hear Source » enregistre ce que jouent les haut-parleurs
        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbSound.Items.Add(device.Name);

        cbSound.SelectedItem = "VisioForge What You Hear Source";
        if (cbSound.SelectedIndex < 0 && cbSound.Items.Count > 0) cbSound.SelectedIndex = 0;
        cbRecordSound.Enabled = cbMP3.Enabled = cbSound.Items.Count > 0;
        cbRecordSound.CheckedChanged += (_, _) => cbMP3.Enabled = cbRecordSound.Checked; // MP3 a besoin du son

        // Les codecs vidéo installés sur ce PC
        foreach (var codec in videoCapture1.Video_Codecs())
            cbVideoCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "MJPEG Compressor";
        if (cbVideoCodec.SelectedIndex < 0 && cbVideoCodec.Items.Count > 0) cbVideoCodec.SelectedIndex = 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        Text = "Screen to AVI";

        // Le moniteur entier, à la fréquence d'images de la liste
        videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
        {
            FullScreen = true,
            AllowDesktopDuplicationEngine = true, // DXGI Desktop Duplication : plus rapide que GDI
            DisplayIndex = (int)cbMonitor.SelectedItem!,
            GrabMouseCursor = cbCursor.Checked,
            FrameRate = new VideoFrameRate((int)cbFrameRate.SelectedItem!)
        };

        var recordSound = cbRecordSound.Checked && cbSound.SelectedItem is not null;
        videoCapture1.Audio_RecordAudio = recordSound;
        // Aucun périphérique sans le son : un périphérique What You Hear laissé ici ouvre quand même les haut-parleurs
        videoCapture1.Audio_CaptureDevice = recordSound ? new AudioCaptureSource(cbSound.Text) : null;
        videoCapture1.Audio_PlayAudio = false; // ne jamais rejouer le son capturé dans les haut-parleurs

        // Audio : MP3, ou PCM non compressé
        var avi = recordSound && cbMP3.Checked ? new AVIOutput(new MP3Output()) : new AVIOutput();
        avi.Video_Codec = cbVideoCodec.Text;

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");
        videoCapture1.Output_Format = avi;
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
        // Terminer l'AVI avant que la fenêtre ne disparaisse
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
2. **Source d'écran** - `FullScreen` enregistre le moniteur entier à sa pleine résolution, et `AllowDesktopDuplicationEngine` capture via Windows Desktop Duplication au lieu de GDI. `GrabMouseCursor` est désactivé par défaut et `FrameRate` vaut 10 par défaut
3. **Son des haut-parleurs** - `VisioForge What You Hear Source`, fourni par le redist de capture vidéo, enregistre ce que jouent les haut-parleurs. Avec Record sound décoché, l'application passe `Audio_RecordAudio` à `false` et efface aussi `Audio_CaptureDevice`, et `Audio_PlayAudio` reste à `false` pour que le son enregistré ne soit pas rejoué dans les haut-parleurs
4. **Codec vidéo** - `Video_Codecs()` renvoie les compresseurs vidéo DirectShow installés sur le PC ; `AVIOutput.Video_Codec` prend l'un de ces noms et vaut `MJPEG Compressor` par défaut, qui stocke chaque image comme une image JPEG
5. **Audio** - Un `AVIOutput` simple écrit du PCM non compressé, stéréo 16 bits à 44,1 kHz, environ dix mégaoctets par minute ; `new AVIOutput(new MP3Output())` écrit du MP3 à 192 kbit/s, environ un mégaoctet et demi par minute
6. **Fonctionnement asynchrone** - `StartAsync` et `StopAsync` gardent l'interface réactive, et la fermeture du formulaire termine d'abord le fichier AVI

## Vérifier le fichier avec ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.avi -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=mjpeg|width=3840|height=2160
# stream|codec_name=pcm_s16le|sample_rate=44100|channels=2
# format|format_name=avi|duration=22.400000|size=679261184
```

Vingt-deux secondes d'un écran 4K à 10 images par seconde occupent 679 mégaoctets, soit environ 240 mégabits par seconde : voilà pourquoi AVI sert au montage et à l'analyse, pas au partage.

## Video Capture SDK X (VideoCaptureCoreX) { #api-moderne-video-capture-sdk-x }

L'API multiplateforme utilise `VideoCaptureCoreX`. Pour le modèle complet d'application console avec configuration de capture d'écran, configuration audio et cycle de vie d'enregistrement, consultez le guide [Capture d'écran en MP4](screen-capture-mp4.md#api-moderne-video-capture-sdk-x). Pour produire de l'AVI au lieu de MP4, remplacez la configuration de sortie :

```csharp
// AVI avec codecs par défaut (vidéo H.264 + audio MP3)
var aviOutput = new AVIOutput(outputPath);
videoCapture.Outputs_Add(aviOutput, autostart: true);
```

### Options de codec AVI

Choisissez l'encodeur vidéo selon votre flux de travail :

```csharp
// MJPEG — indépendant par image, fichiers plus volumineux, idéal pour l'édition
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new MJPEGEncoderSettings();

// H.264 dans un conteneur AVI — fichiers plus petits, moins adapté à l'édition
var h264AviOutput = new AVIOutput(outputPath);
h264AviOutput.Video = new OpenH264EncoderSettings();
```

La capture de région, l'enregistrement multi-écrans, l'audio, la mise en surbrillance du curseur et les options d'encodage GPU sont traités dans le [guide MP4](screen-capture-mp4.md) — toutes les fonctionnalités de configuration de la source fonctionnent à l'identique avec la sortie AVI.

## Questions fréquentes

### Pourquoi mon fichier d'enregistrement d'écran AVI est-il si volumineux ?

MJPEG compresse chaque image séparément, au prix de la taille mais au bénéfice du montage : un écran 4K à 10 images par seconde occupe environ 30 Mo par seconde. Pour réduire la taille, baissez la fréquence d'images, capturez une zone plus petite au lieu de l'écran entier (`FullScreen = false` avec `Left`, `Top`, `Right` et `Bottom`), ou passez à la [sortie MP4](screen-capture-mp4.md), dont le H.264 compresse d'une image à l'autre.

### Comment enregistrer le son de l'ordinateur et non celui du micro ?

Sélectionnez `VisioForge What You Hear Source` comme `AudioCaptureSource` : il enregistre ce que jouent les haut-parleurs, et il est fourni dans le redist de capture vidéo. Laissez `Audio_PlayAudio` à `false`, pour que le son enregistré ne soit pas rejoué dans les haut-parleurs.

### Quand dois-je utiliser AVI plutôt que MP4 pour l'enregistrement d'écran ?

Choisissez AVI quand vous avez besoin d'un accès indépendant à chaque image pour le montage vidéo (MJPEG permet d'aller à n'importe quelle image sans décoder les précédentes), quand il vous faut un audio PCM sans perte, ou quand vous travaillez avec des outils basés sur DirectShow. Dans tous les autres cas, le MP4 avec H.264 offre une meilleure compression, des fichiers plus petits et une prise en charge multiplateforme plus large.

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons l'écran et le son des haut-parleurs dans un fichier AVI depuis une application C# WinForms avec le VisioForge Video Capture SDK. Voici ce que nous allons construire. Une application WinForms qui enregistre un moniteur entier dans un fichier AVI. Le moniteur, la fréquence d'images et le curseur de la souris se choisissent sur le formulaire. Elle enregistre aussi le son joué par les haut-parleurs, en PCM non compressé ou en MP3. Et le codec vidéo est n'importe lequel de ceux installés sur le PC, MJPEG par défaut. Voici l'application terminée, qui enregistre l'écran sur lequel elle tourne.

### AVI ou MP4

Quand choisir AVI plutôt que MP4 ? Avec MJPEG, chaque image de l'enregistrement est une image complète : vous pouvez couper et monter sur n'importe quelle image, et lire chaque image seule pour l'analyser. L'audio PCM est sans perte. Le prix, c'est la taille : ici, environ vingt-huit mégaoctets par seconde, pour un écran 4K à dix images par seconde. Pour le web, les téléphones et le partage, MP4 avec H.264 est le meilleur choix. C'est la vidéo capture d'écran vers MP4.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou plus récent, ou .NET Framework 4.7.2. Cette fois, ni caméra ni micro. VideoCaptureCore est le moteur DirectShow du SDK : cette application ne fonctionne que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms, et choisissez Windows Forms App pour C#. Nommez le projet ScreenCaptureAvi. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant deux paquets NuGet : le SDK lui-même, et le redist de capture vidéo. Il contient les filtres DirectShow, dont la source d'écran et la source sonore What You Hear. AVI n'a besoin de rien d'autre. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.Core et installez-le. Puis installez le redist de capture vidéo pour x64. Le redist est celui pour x64 : réglez donc PlatformTarget sur x64 dans le fichier du projet. Une application x86 prend le redist x86 à la place.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView, où l'application affiche l'aperçu de ce qu'elle enregistre. Ajoutez une étiquette et une liste déroulante pour le moniteur, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour la fréquence d'images, le périphérique audio et le codec vidéo. Ajoutez trois cases à cocher : le curseur de la souris et Record sound, toutes deux cochées, et MP3 audio. Puis les boutons Start et Stop. Stop reste désactivé jusqu'au début d'un enregistrement. Double-cliquez sur le formulaire et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'aperçu est dessiné. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles s'affichent dans la barre de titre.

### Remplir les listes

Le moteur désigne un moniteur par le numéro à la fin de son nom de périphérique Windows : un pour DISPLAY1, deux pour DISPLAY2. Une petite méthode utilitaire lit ce numéro. Le gestionnaire Load liste tous les moniteurs et sélectionne le moniteur principal. La liste des fréquences d'images contient quelques valeurs fixes. Dix images par seconde sont sélectionnées, la valeur par défaut du SDK lui-même pour l'écran. Audio_CaptureDevices liste les périphériques audio. VisioForge What You Hear Source enregistre tout ce qui est joué sur les haut-parleurs : il est donc sélectionné quand il est présent. Sans aucun périphérique audio, les deux cases du son sont désactivées. Et MP3 n'a de sens qu'avec du son : il suit donc Record sound. Video_Codecs liste les codecs vidéo installés sur ce PC. Nous sélectionnons MJPEG Compressor, ou le premier codec si MJPEG est absent. Video_Codecs renvoie les compresseurs vidéo DirectShow de ce PC. Celui-ci en a deux : MJPEG Compressor et DV Video Encoder. MJPEG enregistre chaque image comme une image JPEG. DV n'écrit que du DV en définition standard. Un codec que vous installez vous-même apparaît dans la même liste.

### Capturer l'écran

Le gestionnaire Start vérifie d'abord qu'un codec vidéo est sélectionné. Sinon, il l'indique dans la barre de titre. Ensuite, il décrit la source d'écran. FullScreen enregistre le moniteur entier. AllowDesktopDuplicationEngine capture via Windows Desktop Duplication au lieu de GDI. DisplayIndex est le numéro du moniteur choisi dans la liste, GrabMouseCursor dessine le curseur dans la vidéo, et FrameRate est la fréquence choisie dans la liste. Petit récapitulatif. FullScreen enregistre le moniteur entier à sa pleine résolution. DisplayIndex est le numéro tiré du nom de périphérique du moniteur. Le curseur est désactivé par défaut, et la fréquence d'images est de dix. Cette vidéo enregistre un écran 4K à dix images par seconde.

### Enregistrer le son des haut-parleurs

Avec Record sound coché, l'application enregistre le périphérique sélectionné. S'il est décoché, le périphérique est aussi effacé : un périphérique What You Hear laissé en place ouvre quand même les haut-parleurs. Audio_PlayAudio reste désactivé : le son enregistré n'est pas rejoué dans les haut-parleurs d'où il vient.

### AVIOutput : audio PCM ou MP3

Passons à la sortie. Un AVIOutput simple écrit un audio PCM non compressé. Passez un MP3Output à son constructeur, et l'audio est en MP3. Réglez Video_Codec sur le nom choisi dans la liste, écrivez output.avi dans le dossier Vidéos, et passez le moteur en mode capture d'écran. Un AVIOutput simple écrit du PCM : stéréo 16 bits à 44,1 kilohertz, sans perte, environ dix mégaoctets par minute. Avec un MP3Output, il écrit du MP3 à 192 kilobits par seconde, environ un mégaoctet et demi par minute. À côté de la vidéo MJPEG, les deux sont petits.

### Démarrer et arrêter

Start et Stop sont asynchrones. StartAsync revient dès que la capture tourne, et StopAsync termine l'écriture du fichier AVI.

### Fermer la fenêtre

Fermer la fenêtre pendant l'enregistrement laisserait l'AVI inachevé. Dans la fenêtre Properties, sur l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant l'enregistrement, il annule la fermeture. Puis il arrête l'enregistrement, et ferme de nouveau la fenêtre une fois le gestionnaire terminé. Quand rien n'est enregistré, le gestionnaire libère le moteur.

### Lancer l'application

Lancez l'application depuis Visual Studio. Les listes se remplissent : le moniteur, la fréquence d'images, le périphérique audio et le codec. La liste du son contient What You Hear et les micros. What You Hear reste sélectionné. Un son est joué sur les haut-parleurs, et Start lance l'enregistrement. L'aperçu montre l'écran avec le curseur. Stop termine output.avi, puis nous fermons la fenêtre.

### Vérifier le fichier avec ffprobe

Le fichier arrive dans Vidéos : environ six cent cinquante mégaoctets pour vingt-trois secondes. ffprobe montre ce qu'il contient. Le conteneur est AVI. La vidéo est en MJPEG, en 3840x2160. L'audio est en PCM, 16 bits, stéréo à 44,1 kilohertz : le son des haut-parleurs. Cela fait environ 240 mégabits par seconde : voilà pourquoi AVI sert au montage et à l'analyse, pas au partage.

### Erreurs fréquentes

Quelques erreurs fréquentes. Un paquet redist manquant : le moteur ne démarre pas. Un fichier muet, parce que Record sound était décoché, ou qu'un micro était sélectionné au lieu de What You Hear. Des paquets x86 et x64 mélangés dans un même projet. Fermer la fenêtre en plein enregistrement : attendez d'abord StopAsync. Et AVI là où il faut du MP4 : une heure de MJPEG en 4K atteint environ cent gigaoctets.

### Étapes suivantes

Ensuite, la vidéo capture d'écran vers MP4 enregistre l'écran dans de petits fichiers H.264. La vidéo webcam vers AVI enregistre une caméra de la même façon qu'ici. Et la documentation montre comment enregistrer une zone ou une seule fenêtre au lieu de l'écran entier. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Capture d'écran en MP4](screen-capture-mp4.md) — format recommandé avec couverture complète des fonctionnalités (région, multi-écrans, audio, encodage GPU, multiplateforme)
- [Webcam en AVI](video-capture-camera-avi.md) — la même sortie AVI avec une caméra et un micro
- [Capture d'écran en WMV](screen-capture-wmv.md) — alternative au format Windows Media
- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — enregistrement d'écran en Visual Basic .NET
- [Configuration de la source d'écran](../video-sources/screen.md) — référence complète des paramètres de capture
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
