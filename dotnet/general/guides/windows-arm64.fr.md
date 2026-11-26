---
title: Exécuter le SDK .NET nativement sous Windows ARM64
description: Compilez une application native Windows on Arm avec Media Blocks SDK .NET — les paquets d'exécution ARM64 et les pièges d'empaquetage de Smart App Control.
sidebar_label: Windows sur ARM64
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - ARM64
  - WinForms
  - GStreamer
  - NuGet
  - Deployment
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock
---

# Exécuter le SDK .NET nativement sous Windows ARM64

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk){ .md-button .md-button--primary target="_blank" }

Sur une machine Windows on Arm, une compilation x64 de votre application s'exécute en émulation. Media
Blocks SDK .NET peut aussi s'exécuter comme processus ARM64 natif : c'est plus rapide et cela utilise les
bibliothèques ARM64 de la machine. Cette page indique ce qu'il faut régler dans le fichier de projet,
quels paquets référencer, et où se trouvent les deux problèmes connus.

## Ce qui s'exécute nativement

| Moteur | Dans un processus ARM64 natif |
|---|---|
| Media Blocks SDK .NET (`MediaBlocksPipeline`) | Oui. GStreamer, les plugins de codecs, le moteur de rendu vidéo et les décodeurs de fichiers sont ARM64. |
| `VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX` | Oui. |
| `VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore` (DirectShow) | En partie. `VisioForge.DotNet.Core.Redist.Base.arm64` contient les filtres de base, les effets vidéo, les mélangeurs vidéo et audio, le moteur de rendu audio WASAPI et la capture d'écran Desktop Duplication ; `Redist.MP4.arm64` les sorties MP4, MPEG-TS, MKV et HLS avec les encodeurs H.264/H.265 de Media Foundation ; `Redist.FFMPEG.arm64` le filtre FFMPEG Source et les décodeurs vidéo et audio FFMPEG, si bien que `MediaPlayerCore` lit les fichiers en natif avec `MediaPlayerSourceMode.FFMPEG`. Les filtres LAV et les encodeurs et muxers DirectShow x64 hérités n'ont pas de version ARM64 : une application DirectShow qui décode des fichiers via LAV échoue dans un processus ARM64 : passez-la au mode source FFMPEG, ou compilez-la en x64 et laissez Windows l'exécuter en émulation. |

NDI et les SDK de caméras Basler, Allied Vision et Spinnaker n'ont pas de version ARM64, et leurs
blocs signalent une erreur dans un processus ARM64. Les caméras GenICam et RealSense et les blocs
ElevenLabs fonctionnent nativement avec `VisioForge.CrossPlatform.GenICam.Windows.arm64`,
`VisioForge.CrossPlatform.RealSense.Windows.arm64` et `VisioForge.CrossPlatform.ElevenLabs.Windows.arm64`.

## Réglages du projet

```xml
<PropertyGroup>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>ARM64</PlatformTarget>
  <RuntimeIdentifier>win-arm64</RuntimeIdentifier>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.arm64" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.arm64" Version="2026.*" />
</ItemGroup>
```

`PlatformTarget` et les paquets doivent concorder. Avec `PlatformTarget` à `x64` et les paquets ARM64
référencés, l'application est quand même compilée comme processus x64, s'exécute en émulation et tente
de charger les bibliothèques natives x64.

`VisioForge.DotNet.MediaBlocks` est le même paquet managé pour toutes les architectures ; seuls les
paquets d'exécution natifs changent. `Core.Windows.arm64` contient l'exécution GStreamer. `Libav.Windows.arm64` contient les plugins FFmpeg
(`gstlibav`) qui décodent H.264, HEVC et les autres formats courants via la source fichier ; le paquet
Core ne les inclut pas. N'ajoutez `VisioForge.CrossPlatform.Core.Windows.Adds.arm64` que pour les
plugins optionnels qu'il contient, comme vous le feriez en x64.

Compilez et publiez sur n'importe quelle machine, y compris une x64 :

```powershell
dotnet build -c Release -r win-arm64
dotnet publish -c Release -r win-arm64
```

Le dossier de sortie contient le même sous-dossier `arm64` avec l'exécution native que la compilation
x64 place dans `x64` ; aucune étape de copie supplémentaire n'est nécessaire.

Ce dossier contient aussi le runtime Microsoft Visual C++ dont les binaires ARM64 ont besoin
(`vcruntime140.dll`, `msvcp140.dll`, `concrt140.dll`), comme les paquets x64, la machine cible n'a donc
pas besoin du Visual C++ ARM64 Redistributable. Les anciens paquets ARM64 ne le contenaient pas, et sur une
machine sans ce redistribuable GStreamer pouvait ne pas se charger. Mettez les paquets ARM64 à jour vers la
version actuelle, ou installez le Microsoft Visual C++ Redistributable pour ARM64 le plus récent.

## Problèmes connus

**Sortie de publication en double (`NETSDK1152`).** Les anciens paquets ARM64 livraient les DLL FFmpeg à
la fois dans `Core.Windows.arm64` et dans `Libav.Windows.arm64`. `dotnet build` passait, et
`dotnet publish -r win-arm64` échouait avec *Found multiple publish output files with the same relative
path*. C'est corrigé dans les paquets actuels : gardez les deux références sur une version 2026 à jour
et la publication fonctionne.

**Smart App Control.** Sur une installation propre de Windows 11 Arm, Smart App Control refuse de charger
tout binaire non signé avec Authenticode. Les fichiers des paquets VisioForge publiés sont signés ; l'EXE de
votre application ne l'est pas, sauf si vous le signez, et Smart App Control le bloque, donc l'application
ne démarre pas. Si vous compilez le SDK depuis les sources, ses assemblys ne sont pas signés non plus et le
SDK signale un « module introuvable » silencieux. Signez l'EXE et les DLL que vous distribuez, ou
développez sur une machine où Smart App Control est désactivé ou en mode évaluation.

## Exemple complet

**[ARM64 Player Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/ARM64%20Player%20Demo)**
est une application WinForms qui lit un fichier dans un aperçu en direct et affiche l'architecture du
processus, pour que l'on voie qu'elle s'exécute bien en `Arm64`. Son fichier de projet, ce sont les
réglages ci-dessus plus les paquets d'exécution ARM64, et son readme détaille les différences ARM64.
