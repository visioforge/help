---
title: Enregistrer une caméra IP RTSP en MP4 en C# .NET — Exemple
description: "Enregistrez une caméra IP RTSP en MP4 en C# WinForms avec VideoCaptureCoreX : URL, identifiant et mot de passe, aperçu en direct, audio, H.264 sur GPU."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - MP4Output
  - VisioForgeX
video:
  id: 6CTrBCzHySE
  upload_date: 2026-11-17T08:57:31Z
  duration: PT9M52S

---

# Enregistrer le flux RTSP d'une caméra IP en MP4 en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation

Ce tutoriel enregistre une caméra IP dans un fichier MP4 depuis une application C# WinForms avec le moteur multiplateforme `VideoCaptureCoreX` :

- l'URL RTSP de la caméra, son identifiant et son mot de passe sont saisis dans le formulaire, et le champ du mot de passe affiche des points ;
- `RTSPSourceSettings.CreateAsync` se connecte à la caméra et lit ce que contient le flux, si bien qu'une caméra sans micro est enregistrée en vidéo seule ;
- un `VideoView` affiche l'aperçu en direct pendant l'écriture du fichier ;
- `MP4Output` encode en H.264 sur le GPU quand le PC dispose d'un encodeur matériel, avec OpenH264 sur le CPU sinon, et l'audio en AAC ;
- chaque enregistrement reçoit son propre nom de fichier, avec la date et l'heure, dans le dossier Vidéos.

Le moteur classique `VideoCaptureCore` enregistre aussi les caméras IP : le [tutoriel d'aperçu de caméra IP](ip-camera-preview.md) construit son application, et son mode `IPCapture` écrit le fichier.

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/6CTrBCzHySE?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### La version courte

Le même enregistrement sous forme d'application console en quelques lignes de C#, sans l'aperçu ni le son. L'identifiant et le mot de passe sont écrits dans son code : remplacez `admin` et `password` par ceux de votre caméra.

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

await VisioForgeX.InitSDKAsync();
await using var cam = new VideoCaptureCoreX();
cam.OnError += (_, e) =>
    Console.WriteLine(e.Message);
cam.Video_Source = await RTSPSourceSettings
    .CreateAsync(new Uri(args[0]), "admin",
        "password", audioEnabled: false);
cam.Outputs_Add(new MP4Output("ipcam.mp4"));
if (!await cam.StartAsync()) return;
Console.ReadLine();
await cam.StopAsync();
```

Son code : [ip-camera-mp4-short-console sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-mp4-short-console).

## Dépôt du code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-capture-mp4-winforms)

## Paquets nécessaires

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — le SDK ; le `VideoView` WinForms en fait partie
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — le runtime GStreamer sur lequel fonctionne le moteur X, avec la source RTSP et les encodeurs H.264
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — l'encodeur audio AAC

Ce sont les paquets x64, le projet indique donc `<PlatformTarget>x64</PlatformTarget>` ; une application 32 bits prend les paquets `.x86`. La liste complète se trouve dans [Déploiement sous Windows](../../deployment-x/Windows.md).

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

## Exemple d'implémentation en C\#

Le formulaire contient un `VideoView` nommé `VideoView1`, les zones de texte `edURL`, `edLogin` et `edPassword` (avec `UseSystemPasswordChar`), une case à cocher **Record audio** `cbRecordAudio` et les boutons `btStart` et `btStop`, tous deux désactivés dans le concepteur. Les événements `Load` et `FormClosing` du formulaire sont reliés aux gestionnaires ci-dessous.

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace IpCameraCaptureMp4;

public partial class Form1 : Form
{
    private VideoCaptureCoreX videoCapture1 = null!;

    private string outputFile = string.Empty;

    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        btStart.Enabled = true;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Text = "Enter the camera URL";
            return;
        }

        Text = "Connecting...";
        btStart.Enabled = false;
        cbRecordAudio.Enabled = false;

        var started = false;
        try
        {
            // Reads the stream info first: audio is recorded only when the info shows an audio track
            videoCapture1.Video_Source = await RTSPSourceSettings.CreateAsync(url, edLogin.Text, edPassword.Text, cbRecordAudio.Checked);

            videoCapture1.Audio_Record = cbRecordAudio.Checked;
            videoCapture1.Audio_Play = false; // record the camera's sound, don't play it

            // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"ipcam_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            videoCapture1.Outputs_Clear();
            videoCapture1.Outputs_Add(new MP4Output(outputFile));

            started = await videoCapture1.StartAsync();
        }
        catch (Exception ex)
        {
            Text = ex.Message;
        }

        // An SDK error posted during the start stays in the title
        if (Text == "Connecting...") Text = started ? "Recording..." : "The recording did not start";
        btStop.Enabled = started;
        btStart.Enabled = !started;
        cbRecordAudio.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync(); // finishes the MP4 file
        Text = "Saved: " + outputFile;
        btStart.Enabled = true;
        cbRecordAudio.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true; // loading, starting or stopping: wait for it
            return;
        }

        if (btStop.Enabled)
        {
            e.Cancel = true; // recording: finish the MP4 first
            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close);
            return;
        }

        videoCapture1.Dispose();
        VisioForgeX.DestroySDK();
    }
}
```

## Points clés de l'implémentation

1. **Initialisation du SDK** - `VisioForgeX.InitSDKAsync` s'exécute une fois, au chargement du formulaire. Le premier lancement sur un PC prend plus de temps, c'est pourquoi Start n'est activé qu'après son retour. `VisioForgeX.DestroySDK` le libère à la fermeture du formulaire
2. **L'adresse de la caméra** - `rtsp://`, l'adresse IP et le port de la caméra (souvent 554), puis un chemin qui dépend de la marque. L'identifiant et le mot de passe sont des arguments séparés de `CreateAsync`, l'URL ne les contient donc pas. Une adresse `http://` est prise comme le service ONVIF de la caméra, et l'URL RTSP de son meilleur profil y est lue. L'[annuaire des marques de caméras IP](../../camera-brands/index.md) donne les chemins de nombreuses caméras
3. **Audio** - quand **Record audio** est cochée, `CreateAsync` demande l'audio de la caméra et `Audio_Record` le place dans le fichier. L'audio n'est enregistré que si les informations du flux montrent une piste audio : une caméra sans micro donne un fichier en vidéo seule. `Audio_Play` vaut `true` par défaut ; l'application le met à `false`, le son est donc enregistré mais pas joué
4. **Sortie MP4** - `MP4Output` sans réglage d'encodeur choisit NVIDIA NVENC, puis AMD AMF, puis Intel Quick Sync, et OpenH264 sur le CPU si aucun d'eux n'est présent. L'audio est en AAC. Une sortie reste dans le moteur après un enregistrement, c'est pourquoi `Outputs_Clear` s'exécute avant chaque nouveau
5. **Démarrage** - une caméra qui ne répond pas met du temps à échouer, le titre affiche donc « Connecting... » et Start reste désactivé jusqu'au retour de `StartAsync`. Un message `OnError` publié pendant le démarrage reste dans le titre
6. **Arrêt et fermeture** - `StopAsync` finalise le fichier MP4. Fermer le formulaire pendant un enregistrement annule la fermeture, attend `StopAsync`, puis ferme à nouveau ; pendant le chargement du SDK, ou pendant un démarrage ou un arrêt, la fermeture attend

## Dépannage

- **La caméra refuse le flux** - le chemin ne correspond pas à la marque de la caméra, ou l'identifiant ou le mot de passe est faux. L'erreur s'affiche dans le titre de la fenêtre
- **Le fichier n'a pas de son** - la caméra n'envoie pas de piste audio, ou l'audio est désactivé dans les réglages de la caméra elle-même
- **Choisir l'encodeur** - pour rester sur le CPU, passez `new OpenH264EncoderSettings()` (espace de noms `VisioForge.Core.Types.X.VideoEncoders`) au constructeur de `MP4Output` ; `H264EncoderBlock.CanEncodeAsync` (espace de noms `VisioForge.Core.MediaBlocks.VideoEncoders`) vérifie si un encodeur GPU peut encoder à cet instant
- **Plusieurs caméras** - utilisez un `VideoCaptureCoreX` et un `VideoView` pour chacune

## Transcription de la vidéo

### Ce que nous construisons

Enregistrons, dans une application C# WinForms, une caméra IP dans un fichier MP4, avec le VisioForge Video Capture SDK. Voici ce que nous construisons. Une application WinForms qui enregistre une caméra IP dans un fichier MP4. Vous saisissez l'adresse RTSP de la caméra, son identifiant et son mot de passe. L'image s'affiche en direct pendant l'enregistrement. La vidéo est en H.264, et le son de la caméra est enregistré en AAC. Voici l'application terminée, en train d'enregistrer. Le flux vient ici d'un serveur RTSP de test sur le réseau local, qui diffuse le film libre Big Buck Bunny, il ne demande donc pas d'identifiant. Une vraie caméra fonctionne de la même façon, avec son identifiant et son mot de passe. À quoi cela sert-il ? Un enregistreur vidéo simple pour vos propres caméras. Conserver un événement dans un fichier comme preuve. Enregistrer une ligne de production ou un essai en laboratoire. Et des clips à revoir ou à partager plus tard.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet cible .NET 8 ou une version ultérieure. Et une caméra IP qui diffuse en RTSP, avec son adresse. Le moteur utilisé ici est VideoCaptureCoreX, le moteur multiplateforme du SDK, basé sur GStreamer. Le même code fonctionne sous macOS et Linux, avec leur propre interface.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms et choisissez Windows Forms App pour C#. Nommez le projet IpCameraCaptureMp4. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet. Le Video Capture SDK lui-même. Le paquet core multiplateforme, avec le runtime GStreamer pour Windows x64. Et le paquet Libav, qui apporte l'encodeur audio AAC. Dans Visual Studio, c'est Manage NuGet Packages. Recherchez VisioForge.DotNet.VideoCapture et installez-le. Installez ensuite le paquet core multiplateforme pour Windows x64. Et le paquet Libav pour Windows x64. Les paquets du runtime sont en x64, réglez donc PlatformTarget sur x64 dans le fichier du projet.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'image en direct. Ajoutez une étiquette et une zone de texte pour l'URL de la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour l'identifiant et le mot de passe. Sur la zone du mot de passe, réglez UseSystemPasswordChar sur True, pour qu'elle affiche des points. Ajoutez une case Record audio, cochée par défaut, et les boutons Start et Stop. Les deux boutons sont désactivés au départ. Start s'active une fois le SDK chargé, et Stop une fois l'enregistrement lancé. Double-cliquez sur le formulaire et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. Les espaces de noms X contiennent le moteur multiplateforme, ses sources et ses sorties. Dans le gestionnaire Load du formulaire, InitSDKAsync charge le SDK et son runtime GStreamer, une fois par application. Créez ensuite le moteur VideoCaptureCoreX et passez-lui le VideoView. Les erreurs du moteur arrivent dans l'événement OnError, et ici elles vont dans la barre de titre. Enfin, Start est activé. Un champ garde le nom du fichier de sortie, pour l'afficher après l'arrêt.

### Se connecter à la caméra

Le gestionnaire de Start vérifie d'abord que l'URL est une adresse valide. La connexion peut prendre quelques secondes, le titre affiche donc Connecting, et Start et la case sont désactivés. RTSPSourceSettings.CreateAsync prend l'URL, l'identifiant, le mot de passe, et indique s'il faut utiliser l'audio. L'URL ne contient ni identifiant ni mot de passe, ils n'apparaissent donc jamais dans l'adresse. CreateAsync lit d'abord les informations du flux. Quand la caméra n'envoie pas de son, l'audio est laissé de côté, et l'enregistrement est uniquement vidéo. Audio_Record suit la case. Audio_Play est activé par défaut, et cette application enregistre le son sans le lire, il est donc désactivé. L'adresse commence par rtsp, puis l'adresse IP de la caméra et le port, en général 554. Vient ensuite un chemin, et chaque marque a le sien. Beaucoup de caméras envoient un flux principal en pleine résolution et un flux secondaire plus petit. Le manuel de la caméra ou sa page web donne le chemin exact.

### Enregistrer en MP4

Le fichier va dans le dossier Vidéos, avec la date et l'heure dans son nom. Outputs_Clear retire la sortie de l'exécution précédente, et Outputs_Add ajoute un MP4Output. Sans encodeur indiqué, MP4Output choisit le H.264 sur l'encodeur du GPU s'il y en a un, NVIDIA, AMD ou Intel, et OpenH264 sinon. Le son est en AAC. StartAsync se connecte, et rend la main une fois l'enregistrement lancé. Une exception, comme une caméra qui refuse la connexion, va dans le titre. Une erreur du SDK pendant le démarrage reste aussi dans le titre. Sinon, le titre affiche Recording. Stop est activé si le démarrage a réussi, et Start dans le cas contraire.

### Arrêter et fermer

Stop se désactive lui-même, et StopAsync arrête le flux et finalise le fichier MP4. Le titre affiche ensuite où le fichier a été enregistré, et Start et la case sont de nouveau activés. Dans la fenêtre Properties, sous l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant le chargement, le démarrage ou l'arrêt, les deux boutons sont désactivés, et le gestionnaire annule la fermeture : fermez de nouveau la fenêtre une fois l'opération terminée. Pendant l'enregistrement, la fermeture est aussi annulée. StopAsync finalise le fichier, puis le formulaire se ferme de lui-même. Ce n'est qu'alors que le moteur est libéré et le SDK déchargé. Un fichier non finalisé ne peut pas être lu.

### L'exécuter

Lancez l'application depuis Visual Studio. Start s'active une fois le SDK chargé. Saisissez l'adresse RTSP du flux. Ce flux de test ne demande pas d'identifiant. Cliquez sur Start. Le flux se connecte en quelques secondes, l'image apparaît, et le titre affiche Recording. Cliquez sur Stop. Le fichier est finalisé, et le titre affiche où il a été enregistré, dans le dossier Vidéos. Et fermez la fenêtre. Ce fichier contient une vidéo H.264 en 1280 sur 720, et un son AAC en stéréo.

### Erreurs fréquentes

Quelques erreurs fréquentes. Oublier le paquet Libav, si bien qu'il n'y a pas d'encodeur AAC pour le son. Créer le moteur avant qu'InitSDKAsync ait chargé le SDK. Mettre l'identifiant et le mot de passe dans l'URL, où ils apparaissent partout où l'adresse apparaît. Et fermer l'application sans StopAsync, ce qui laisse un fichier MP4 illisible.

### Étapes suivantes

Ensuite, passez vos propres réglages d'encodeur à MP4Output, pour le débit ou le codec de votre choix. Découpez un long enregistrement en fichiers selon la durée. Pour plusieurs caméras, utilisez un VideoCaptureCoreX et un VideoView pour chacune. Et ONVIF trouve pour vous les caméras du réseau. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Guides associés

- [Tutoriel d'aperçu de caméra IP](ip-camera-preview.md) — le même formulaire avec le moteur classique `VideoCaptureCore`
- [Configuration de la source RTSP](../video-sources/ip-cameras/rtsp.md) — transport UDP/TCP, faible latence, choix du moteur
- [Intégration des caméras IP ONVIF](../video-sources/ip-cameras/onvif.md) — découverte automatique et choix du profil
- [Enregistrer le flux RTSP d'origine sans réencodage](../../mediablocks/Guides/rtsp-save-original-stream.md) — alternative Media Blocks qui conserve le débit d'origine

---
Consultez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour plus d'exemples de code. Besoin de l'URL RTSP de votre caméra ? Consultez notre [annuaire des marques de caméras IP](../../camera-brands/index.md).
