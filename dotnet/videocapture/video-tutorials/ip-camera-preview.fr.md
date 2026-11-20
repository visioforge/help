---
title: "Aperçu en direct d'une caméra IP RTSP en C# WinForms"
description: "Affichez en direct une caméra IP en RTSP en C# WinForms avec VideoCaptureCore, sans identifiants dans l'URL et avec l'image pixelisée."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IPCameraSourceSettings
  - IPSourceEngine
  - VideoCaptureMode
  - VideoEffectMosaic
  - IVideoView
video:
  id: dxVWXHwgFQg
  upload_date: 2026-11-16T20:53:27Z
  duration: PT9M47S

---

# Aperçu d'une caméra IP en RTSP en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce tutoriel affiche en direct une caméra IP en RTSP dans une application C# WinForms avec le moteur VideoCaptureCore :

- l'URL RTSP de la caméra, son identifiant et son mot de passe se saisissent dans le formulaire, et le champ du mot de passe affiche des points ;
- l'identifiant et le mot de passe vont dans leurs propres propriétés de `IPCameraSourceSettings`, jamais dans l'URL ;
- le moteur LAV (`IPSourceEngine.Auto_LAV`) lit le flux ;
- le mode `IPPreview` affiche la caméra sans écrire de fichier ;
- une case Pixelate applique un `VideoEffectMosaic` à toute l'image.

Un écran de surveillance pour vos propres caméras, la vérification de ce que voit une caméra pendant son installation, le masquage de visages ou d'écrans avant de montrer la vidéo, et les bornes ou les écrans d'accueil fonctionnent ainsi. La caméra de la vidéo est une Dahua du réseau local, pixelisée exprès par l'effet de l'application elle-même.

Ce tutoriel utilise le moteur VideoCaptureCore, réservé à Windows : le moteur DirectShow du SDK.

## Tutoriel vidéo

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/dxVWXHwgFQg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Dépôt de code source

Le projet complet, avec le fichier du concepteur, est sur GitHub :

[Code source sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/ip-camera-preview-winforms)

## Paquets requis

Installez le paquet [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/), le redist de capture vidéo avec ses filtres DirectShow, et le redist LAV, le décodeur qui lit le flux RTSP. Un aperçu n'écrit aucun fichier, il n'a donc besoin d'aucun paquet d'encodeur :

- Redist de capture vidéo : [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/), [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Redist LAV : [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/), [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/)

Réglez `PlatformTarget` dans le fichier du projet selon les redists (`x64` ou `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
dotnet add package VisioForge.DotNet.Core.Redist.LAV.x64
```

## Exemple d'implémentation avec code C#

Le formulaire contient un `VideoView`, des zones de texte pour l'URL, l'identifiant et le mot de passe (avec `UseSystemPasswordChar` activé), une case Pixelate cochée par défaut, et les boutons Start et Stop ; Stop est désactivé au départ. Le fichier du concepteur se trouve dans le code source ci-dessus. Le projet cible .NET 10 avec les usings implicites. Sous .NET Framework 4.7.2, le même code se compile dans un projet de style SDK avec `<LangVersion>10</LangVersion>` et `<ImplicitUsings>enable</ImplicitUsings>` ; dans `Program.cs`, remplacez `ApplicationConfiguration.Initialize()`, que .NET Framework ne génère pas, par `Application.EnableVisualStyles()` et `Application.SetCompatibleTextRenderingDefault(false)`, et réglez `<dpiAware>true</dpiAware>` dans un `app.manifest` pour garder la prise en charge du DPI système que .NET 10 active.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace IpCameraPreview;

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
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Text = "Enter the camera URL";
            return;
        }

        // L'URL reste sans identifiants : l'identifiant et le mot de passe sont à part
        videoCapture1.IP_Camera_Source = new IPCameraSourceSettings
        {
            URL = url,
            Login = edLogin.Text,
            Password = edPassword.Text,
            Type = IPSourceEngine.Auto_LAV
        };

        // Vidéo seule : la lecture audio est activée par défaut
        videoCapture1.Audio_PlayAudio = false;

        // Les effets se règlent avant le démarrage
        videoCapture1.Video_Effects_Clear();
        videoCapture1.Video_Effects_Enabled = cbPixelate.Checked;
        if (cbPixelate.Checked)
            videoCapture1.Video_Effects_Add(new VideoEffectMosaic(true, 40));

        videoCapture1.Mode = VideoCaptureMode.IPPreview;

        // Une caméra qui ne répond pas met jusqu'à ~20 s à échouer
        Text = "Connecting...";
        btStart.Enabled = false;
        cbPixelate.Enabled = false;
        var started = await videoCapture1.StartAsync();

        // Une erreur du SDK envoyée pendant le démarrage reste dans le titre
        if (Text == "Connecting...") Text = started ? "IP camera preview" : "The preview did not start";
        btStop.Enabled = started;
        btStart.Enabled = !started;
        cbPixelate.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
        cbPixelate.Enabled = true;
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

1. **Identifiants hors de l'URL** - `Login` et `Password` sont des propriétés de `IPCameraSourceSettings` : l'URL ne les contient jamais, et ils n'apparaissent pas là où l'adresse est affichée ou journalisée. La zone de texte du mot de passe active `UseSystemPasswordChar`, elle affiche donc des points
2. **Le moteur** - `IPSourceEngine.Auto_LAV` lit les flux RTSP et HTTP avec LAV Filters et peut décoder sur le GPU. Sans le redist LAV, le démarrage échoue avec "Unable to create LAV Splitter Source filter"
3. **L'adresse de la caméra** - `rtsp://`, l'adresse IP et le port de la caméra (en général 554), puis un chemin propre à chaque marque. Beaucoup de caméras envoient un flux principal en pleine résolution et un flux secondaire plus petit ; le manuel de la caméra ou sa page web donne le chemin exact, et le [répertoire des marques de caméras IP](../../camera-brands/index.md) en recense beaucoup
4. **Mode aperçu** - `Mode = VideoCaptureMode.IPPreview` affiche la caméra et n'écrit aucun fichier. Ajoutez le redist MP4 ([x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/), [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)), passez à `IPCapture`, indiquez un fichier de sortie et un format, et la même application enregistre ; [Enregistrer un flux RTSP en MP4](ip-camera-capture-mp4.md) montre le même enregistrement avec le moteur multiplateforme `VideoCaptureCoreX`
5. **Audio** - `Audio_PlayAudio` vaut `true` par défaut ; l'exemple n'affiche que la vidéo, il le désactive donc
6. **Pixeliser** - `VideoEffectMosaic(enabled, value)` prend la taille des blocs en pixels, de 1 à 255 : de 2 à 5 l'effet est léger, à partir de 16 plus rien n'est lisible. Il couvre toute l'image, dans l'aperçu comme dans un enregistrement. Les effets se règlent avant `StartAsync`, la case est donc verrouillée pendant que l'aperçu tourne, et `Video_Effects_Clear` retire l'effet de l'exécution précédente
7. **Le démarrage** - une caméra qui ne répond pas peut mettre jusqu'à 20 secondes environ à échouer : le titre indique donc "Connecting..." et Start reste désactivé jusqu'au retour de `StartAsync`. Un message `OnError` envoyé pendant le démarrage reste dans le titre
8. **Fermeture** - `Dispose()` arrête un aperçu en cours, et un aperçu n'a aucun fichier à terminer : le gestionnaire de fermeture n'attend donc que pendant le démarrage ou l'arrêt de l'aperçu

## Dépannage

- **La caméra se connecte mais l'image reste noire** - le moteur LAV reçoit la vidéo RTSP en UDP, et un pare-feu, un NAT ou un VPN qui bloque l'UDP l'arrête ; une machine virtuelle en réseau NAT fait de même. Autorisez l'UDP venant de la caméra, ou utilisez `IPSourceEngine.RTSP_LowLatency`, qui lit le RTSP en TCP par défaut (`RTSP_LowLatency_UseUDP` vaut `false`)
- **La caméra refuse le flux** - le chemin ne correspond pas à la marque de la caméra, ou l'identifiant ou le mot de passe est faux
- **Plusieurs caméras** - utilisez un `VideoCaptureCore` et un `VideoView` pour chacune
- **Trouver les caméras** - `IP_Camera_ONVIF_ListSourcesAsync` découvre les caméras ONVIF du réseau

## Transcription de la vidéo

### Ce que nous construisons

Affichons en direct une caméra IP en RTSP dans une application C# WinForms, avec le VisioForge Video Capture SDK. Voici ce que nous construisons. Une application WinForms qui affiche en direct une caméra IP. Vous saisissez l'adresse RTSP de la caméra, son identifiant et son mot de passe, et le champ du mot de passe affiche des points. Une case Pixelate applique un effet de mosaïque à l'image. Voici l'application terminée, avec l'aperçu d'une caméra Dahua du réseau local. L'image est pixelisée exprès, par l'effet de l'application elle-même, pour que cette vraie caméra ne montre rien de privé. Rien n'est enregistré. L'aperçu ne fait qu'afficher l'image. À quoi cela sert-il ? Un écran de surveillance pour vos propres caméras. Vérifier ce que voit une caméra pendant que vous l'installez. Masquer des visages ou des écrans avant de montrer la vidéo à qui que ce soit. Et les bornes ou les écrans d'accueil.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop. Le projet peut cibler .NET 8 ou une version ultérieure, ou .NET Framework 4.7.2. Et une caméra IP qui diffuse en RTSP, avec son adresse, son identifiant et son mot de passe. VideoCaptureCore est le moteur DirectShow du SDK, cette application ne fonctionne donc que sous Windows.

### Créer un projet WinForms

Dans Visual Studio, créez un nouveau projet. Recherchez Windows Forms et choisissez Windows Forms App pour C#. Nommez le projet IpCameraPreview. Choisissez .NET 10 et cliquez sur Create. Visual Studio ouvre le formulaire vide dans le concepteur.

### Ajouter les paquets NuGet

Ajoutez maintenant trois paquets NuGet. Le SDK lui-même. Le redist de capture vidéo avec ses filtres DirectShow. Et le redist LAV, le décodeur qui lit le flux RTSP. Un aperçu n'écrit aucun fichier, il n'a donc besoin d'aucun paquet d'encodeur. Dans Visual Studio, c'est Manage NuGet Packages. Cherchez VisioForge.DotNet.Core dans Browse et installez-le. Installez ensuite le redist de capture vidéo pour x64. Et le redist LAV pour x64. Sans lui, le démarrage échoue, car rien ne lit le flux. Les redists sont en x64, réglez donc PlatformTarget sur x64 dans le fichier du projet. Une application x86 prend les redists x86 à la place.

### Concevoir le formulaire WinForms

Depuis la Toolbox, dessinez un VideoView pour l'image en direct. Ajoutez une étiquette et une zone de texte pour l'URL de la caméra, et donnez à chaque contrôle un nom clair dans la fenêtre Properties. Faites de même pour l'identifiant et le mot de passe. Sur la zone du mot de passe, réglez UseSystemPasswordChar sur True, pour qu'elle affiche des points. Ajoutez une case Pixelate, cochée par défaut, et les boutons Start et Stop. Stop reste désactivé tant que l'aperçu ne tourne pas. Double-cliquez sur le formulaire et sur les deux boutons. Visual Studio ajoute un gestionnaire vide pour chacun.

### Créer le moteur de capture

En haut de Form1.cs, ajoutez ces using. VideoEffects contient l'effet de mosaïque. Dans le gestionnaire Load du formulaire, créez le moteur VideoCaptureCore et passez-lui le VideoView, où l'image est dessinée. Les erreurs du moteur arrivent dans l'événement OnError. Ici, elles vont dans la barre de titre.

### Se connecter à la caméra

Le gestionnaire de Start vérifie d'abord que l'URL est une adresse valide. Ensuite, IP_Camera_Source reçoit un objet IPCameraSourceSettings. L'URL ne contient ni identifiant ni mot de passe. Ils vont dans leurs propres propriétés, et n'apparaissent donc jamais dans l'adresse. Type Auto_LAV choisit le moteur LAV, qui lit les flux RTSP et HTTP et peut décoder sur le GPU. L'adresse commence par rtsp, puis l'adresse IP de la caméra et le port, en général 554. Vient ensuite un chemin, et chaque marque a le sien. Beaucoup de caméras envoient un flux principal en pleine résolution et un flux secondaire plus petit. Le manuel de la caméra ou sa page web donne le chemin exact.

### Pixeliser l'image

Audio_PlayAudio est activé par défaut, et cette application n'affiche que la vidéo, on le désactive donc. Les effets se règlent avant le démarrage. Video_Effects_Clear retire les effets de l'exécution précédente, et Video_Effects_Enabled suit la case. Quand elle est cochée, Video_Effects_Add ajoute un VideoEffectMosaic avec des blocs de 40 pixels. VideoEffectMosaic prend une taille de bloc en pixels, de 1 à 255. De deux à cinq, l'effet est léger, et à partir de 16 plus rien n'est lisible. Il couvre toute l'image, dans l'aperçu comme dans un enregistrement. L'effet est réglé avant StartAsync, la case est donc verrouillée pendant que l'aperçu tourne.

### Démarrer l'aperçu

Le mode IPPreview affiche la caméra sans écrire de fichier. Une caméra qui ne répond pas peut mettre environ 20 secondes à échouer, le titre indique donc Connecting, et Start et la case sont désactivés. StartAsync rend la main une fois l'aperçu lancé. Une erreur du SDK reste dans le titre, et Stop est activé si le démarrage a réussi.

### Arrêter et fermer

Stop se désactive lui-même, et StopAsync arrête le flux. Puis Start et la case sont réactivés. Dans la fenêtre Properties, sous l'onglet Events, double-cliquez sur FormClosing pour ajouter ce gestionnaire. Pendant que l'aperçu démarre ou s'arrête, les deux boutons sont désactivés, et le gestionnaire attend en annulant la fermeture. Sinon, il libère le moteur. Dispose arrête un aperçu en cours, et il n'y a aucun fichier à terminer.

### L'exécuter

Lancez l'application depuis Visual Studio, et saisissez l'adresse RTSP de la caméra. Puis l'identifiant, et le mot de passe, qui s'affiche en points. Cliquez sur Start. La caméra répond en trois secondes environ, et l'image en direct apparaît, pixelisée. Arrêtez l'aperçu, et fermez la fenêtre.

### Erreurs fréquentes

Quelques erreurs fréquentes. Oublier le redist LAV, et le démarrage échoue. Mettre l'identifiant et le mot de passe dans l'URL, où ils apparaissent partout où l'adresse apparaît. Un mauvais chemin pour la marque de la caméra, et la caméra refuse le flux. Et un réseau qui bloque l'UDP, comme un pare-feu, un NAT ou un VPN. La caméra se connecte, mais l'image reste noire. Le moteur RTSP_LowLatency lit le RTSP en TCP par défaut.

### Étapes suivantes

Ensuite, ajoutez le redist MP4, passez le mode à IPCapture, indiquez un fichier de sortie, et la même application enregistre la caméra en MP4. On peut aussi dessiner du texte sur la vidéo. Pour plusieurs caméras, utilisez un VideoCaptureCore et un VideoView pour chacune. Et ONVIF trouve pour vous les caméras du réseau. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Configuration de la source caméra RTSP](../video-sources/ip-cameras/rtsp.md) — transport UDP/TCP, réglage basse latence, choix du moteur
- [Intégration des caméras IP ONVIF](../video-sources/ip-cameras/onvif.md) — découverte automatique, choix du profil, contrôle PTZ
- [Enregistrer un flux RTSP en MP4](ip-camera-capture-mp4.md) — enregistrer dans un fichier le flux de l'aperçu

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour explorer plus d'exemples de code. Besoin de l'URL RTSP de votre caméra ? Consultez notre [répertoire des marques de caméras IP](../../camera-brands/index.md).
