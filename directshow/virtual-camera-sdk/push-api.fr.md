---
title: API push .NET de Virtual Camera — images vers une webcam
description: Envoyez des images BGRA, BGR24 ou YUY2 et de l'audio PCM depuis C# vers la webcam et le micro virtuels VisioForge avec VisioForge.DotNet.VirtualCamera.
sidebar_label: API push .NET
order: 2
tags:
  - Virtual Camera SDK
  - .NET
  - C#
  - Windows
  - Virtual Camera
primary_api_classes:
  - VirtualVideoWriter
  - VirtualAudioWriter

---

# API push .NET de Virtual Camera

Le paquet NuGet `VisioForge.DotNet.VirtualCamera` envoie des images vidéo et des échantillons audio depuis votre
application .NET directement vers VisioForge Virtual Camera et Virtual Audio Card. Zoom, OBS Studio et les navigateurs
voient la caméra comme une webcam ordinaire, et les applications DirectShow voient aussi la Virtual Audio Card comme un
microphone. Aucun graphe DirectShow n'est nécessaire de votre côté.

Le paquet ne dépend pas du reste du SDK VisioForge. Il cible .NET Framework 4.7.2, .NET 8 et .NET 10 sous Windows.
Ses types se trouvent dans l'espace de noms `VisioForge.VirtualCamera` : il peut donc être référencé à côté de
`VisioForge.DotNet.Core`, qui contient les mêmes writers dans `VisioForge.Core.VirtualCamera.Push`. Un fichier qui
importe aussi `VisioForge.Core.VirtualCamera.Push` qualifie `VirtualVideoWriter` et `VirtualAudioWriter`, et un fichier
qui importe `VisioForge.Core` qualifie `BaseContext` ; un alias `using` convient également.

## Installation

```xml
<PackageReference Include="VisioForge.DotNet.VirtualCamera" Version="[2026.11.6]" />
<!-- The virtual camera and virtual audio card filters -->
<PackageReference Include="VisioForge.DotNet.Core.Redist.VirtualCamera.x64" Version="[2026.11.6]" />
```

`Redist.VirtualCamera.x64` contient les filtres que chargent Zoom, OBS et les navigateurs : c'est l'application
consommatrice qui charge le filtre, et ces applications sont en 64 bits (voir plus bas pour les autres consommateurs).
Compilez votre application pour x64 ou ARM64 afin qu'elles voient ses images : un processus 32 bits envoie vers un flux
que seuls les consommateurs 32 bits lisent. Chaque package `Redist.VirtualCamera` apporte le
package `Redist.Shared` de son architecture, avec les `VisioForge_MFP*.dll` et `VisioForge_MFPX*.dll` que chargent ses
filtres (et `vcomp140.dll` en x64). Une application qui n'est pas x64 et appelle `PushFrameBgra` ou `PushFrameBgr24`
référence aussi le `Redist.Shared` de sa propre architecture (`.x86` ou `.arm64`) pour la conversion des couleurs.

## Enregistrement des filtres

Les filtres source doivent être enregistrés dans COM une fois sur la machine, avec les droits d'administrateur. Le
paquet redist les copie dans le dossier de l'application :

```csharp
using System;
using System.Diagnostics;
using System.IO;

var dir = AppContext.BaseDirectory;
var args = $"/s \"{Path.Combine(dir, "VisioForge_Virtual_Camera_x64.ax")}\" " +
           $"\"{Path.Combine(dir, "VisioForge_Virtual_Audio_Card_x64.ax")}\"";

// Une seule invite UAC pour les deux filtres.
var info = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "regsvr32.exe"), args)
{
    UseShellExecute = true,
    Verb = "runas",
};

using (var process = Process.Start(info))
{
    process.WaitForExit();
    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException($"regsvr32 failed with exit code {process.ExitCode}.");
    }
}
```

L'application consommatrice charge la version du filtre qui correspond à sa propre architecture et voit les images
d'une application émettrice de la même architecture : une application x64 ou ARM64 alimente les consommateurs 64 bits,
une application x86 les consommateurs 32 bits. Les filtres x64 ci-dessus servent Zoom, OBS et les navigateurs 64 bits.
Pour les consommateurs 32 bits, enregistrez aussi `VisioForge_Virtual_Camera.ax` et `VisioForge_Virtual_Audio_Card.ax`
du redist `.x86` ; pour les consommateurs ARM64 natifs, `VisioForge_Virtual_Camera_arm64.ax` et
`VisioForge_Virtual_Audio_Card_arm64.ax` du sous-dossier `arm64` que le redist `.arm64` crée dans le dossier de
l'application.

Un programme d'installation peut enregistrer les mêmes fichiers ; voir [Enregistrement des filtres](../deployment/filter-registration.md).

## Envoi de la vidéo

```csharp
using VisioForge.VirtualCamera;

const int width = 1280, height = 720;

// Le dernier argument est votre clé de licence Virtual Camera SDK ; null affiche la bande TRIAL.
using (var video = new VirtualVideoWriter(width, height, 30, "VideoStream1", null, "YOUR-LICENSE-KEY"))
{
    video.Start();

    // Toutes les 1/30 s : une image BGRA, ligne du haut en premier, stride en octets.
    video.PushFrameBgra(bgraPixels, width * 4);

    video.Stop();
}
```

`VirtualVideoWriter` accepte les images dans trois formats :

| Méthode | Ordre des octets | Source typique |
|---|---|---|
| `PushFrameBgra(data, stride)` | B, G, R, A par pixel, lignes de haut en bas ; l'alpha est ignoré | WPF `Bgra32` (`Pbgra32` avec des pixels opaques), GDI+ `Format32bppArgb`, Direct3D `B8G8R8A8` |
| `PushFrameBgr24(data, stride)` | B, G, R par pixel, lignes de haut en bas | WPF `Bgr24`, GDI+ `Format24bppRgb`, OpenCV `CV_8UC3` |
| `PushFrame(data)` | YUY2, exactement `width * height * 2` octets | votre propre encodeur ou convertisseur |

La largeur doit être paire. `PushFrameBgra` et `PushFrameBgr24` convertissent en YUY2 en natif dans `VisioForge_MFPX64.dll`
(`VisioForge_MFPX.dll` sur x86, `VisioForge_MFPX_ARM64.dll` sur ARM64). Quand le consommateur prend du retard, un envoi
écrase l'image non lue la plus ancienne au lieu d'attendre qu'elle soit lue. Un envoi renvoie `false` lorsque l'image
n'a pas pu être écrite, par exemple lorsque tous les tampons sont occupés ; cette image est perdue.

Un writer précédent conserve la mémoire partagée du flux, et sa taille d'image, jusqu'à sa libération (`Dispose` ;
`Stop` la conserve). Tant qu'il existe, un nouveau writer sur le même nom de flux ne prend le relais qu'avec cette
taille ; avec une autre taille, `Start` lève `InvalidOperationException`. Une autre fréquence d'images est acceptée,
mais une application qui lit déjà la caméra continue d'annoncer celle avec laquelle elle l'a ouverte. Pour changer de
taille, libérez le writer précédent : une application qui lit la caméra relâche le flux en une image environ, puis
adopte la nouvelle taille ; si `Start` lève encore l'exception juste après, rappelez-le un instant plus tard.
`VirtualAudioWriter` est refusé de la même façon lorsque la fréquence d'échantillonnage, le nombre de canaux ou la
taille d'échantillon diffèrent, mais une application qui lit le micro conserve le format avec lequel elle l'a ouvert :
fermez le micro dans cette application avant de démarrer un writer avec un autre format.

## Envoi de l'audio

```csharp
// PCM 48 kHz stéréo 16 bits pour le micro « VisioForge Virtual Audio Card ».
using (var audio = new VirtualAudioWriter(48000, 2, 16, "AudioStream1", null))
{
    audio.Start();

    audio.PushSamples(pcmBytes); // un nombre entier de trames d'échantillons de 4 octets

    audio.Stop();
}
```

Les horodatages sont calculés à partir du nombre d'octets envoyés : envoyez l'audio au rythme de sa lecture.

## Envoi depuis C++

Le paquet est réservé à .NET, mais la caméra lit ses images dans une mémoire partagée nommée : une application C++ peut
donc les y écrire directement. L'exemple console **PushDemo** du
[dépôt d'exemples DirectShow](https://github.com/visioforge/directshow-samples/tree/main/Virtual%20Camera%20SDK/cpp/PushDemo)
le fait dans un seul `main.cpp`, sans autre dépendance que le Windows SDK : il envoie des barres de couleur YUY2 en
1280x720 à 30 i/s et prend la clé de licence en argument. L'architecture de l'émetteur détermine le flux : une version
64 bits alimente les applications 64 bits comme Zoom, OBS et les navigateurs, une version 32 bits uniquement les
applications 32 bits.

## Journalisation

Les writers reçoivent un `BaseContext` pour leurs avertissements et erreurs ; avec `null`, ils vont vers
`System.Diagnostics.Trace`. Pour les rediriger, dérivez de `BaseContext` et redéfinissez `Error` et `Warning`.

## Licences

Sans clé de licence, la caméra virtuelle dessine une bande TRIAL sur l'image. Passez la clé reçue lors de l'achat de
Virtual Camera SDK au constructeur de `VirtualVideoWriter` ; la clé dans
`HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\License` fonctionne également.

## Démo

**Virtual Camera Push Demo** (WPF, C#), dans le [dépôt d'exemples du SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Virtual%20Camera%20SDK/WPF/CSharp/Virtual%20Camera%20Push%20Demo),
dessine une image animée avec WPF, l'envoie avec `PushFrameBgra`, envoie une tonalité de 440 Hz au micro virtuel et
enregistre les filtres à l'aide d'un bouton.

## Voir aussi

* [Virtual Camera SDK](index.md)
* [Exemples](examples.md)
