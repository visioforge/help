---
title: FAQ des filtres DirectShow — multiplexeurs MP4 et HEVC
description: Réponses sur les filtres DirectShow VisioForge — plantages du multiplexeur MP4 dans les graphes DES et les encodeurs H.265 utilisables par MF Mux.
sidebar_label: FAQ
tags:
  - DirectShow
  - C++
  - .NET
  - Windows
  - Encoding
  - MP4
  - H.264
  - HEVC
  - Troubleshooting

---

# FAQ des filtres DirectShow

Réponses aux questions que posent les développeurs qui utilisent directement les filtres DirectShow VisioForge dans leurs propres graphes. Pour les SDK .NET construits sur ces filtres, consultez [Dépannage](../dotnet/general/troubleshooting.md).

## Encodage et multiplexage

### Le multiplexeur MP4 plante, ou le graphe échoue avec « Attempted to divide by zero »

Le multiplexeur MP4 v10 comme le filtre MF Mux lisent la taille d'image et la fréquence d'images dans le type de média de la connexion vidéo. Un type de média qui ne les contient pas — fréquent dans les graphes construits par DirectShow Editing Services (DES) ou par des filtres source personnalisés — peut empêcher l'encodeur de démarrer, provoquer une erreur de division par zéro dans `IMediaControl.Run()` ou des échecs intermittents dans le multiplexeur.

Définissez-les explicitement sur le type de média vidéo que vous connectez à l'encodeur ou au multiplexeur, ou sur le groupe DES :

- `biWidth` et `biHeight` dans le `BITMAPINFOHEADER` ;
- `AvgTimePerFrame` dans le `VIDEOINFOHEADER` (en unités de 100 ns, par exemple `400000` pour 25 i/s).

Un graphe C# complet avec le multiplexeur MP4 v10 se trouve dans la [MP4 v10 Output Demo](https://github.com/visioforge/directshow-samples/tree/main/Encoding%20Filters%20Pack/dotnet/MP4%20v10%20Output%20Demo). Si le problème persiste avec des types de média corrects, comparez avec la démonstration et envoyez au support un petit graphe qui le reproduit.

### Quels encodeurs H.265 MF Mux peut-il utiliser ?

MF Mux n'a pas d'encodeur HEVC propre. Il encode le H.265 avec les encodeurs Media Foundation installés sur le PC :

- **Encodeurs GPU** — Intel Quick Sync, NVIDIA NVENC et AMD AMF prennent en charge HEVC sur la plupart des GPU sortis ces dernières années.
- **L'encodeur H.265 de Microsoft** — installé avec le paquet *HEVC Video Extensions* du Microsoft Store, qui n'est pas disponible sous Windows Server.

Lorsqu'aucun d'eux ne peut encoder sur un PC, l'encodeur ne démarre pas pour HEVC et la sortie de débogage affiche un message comme `Unable to init NVENC H265 encoder.` (ou sa forme Intel QSV et AMD), ou `MS HEVC encoder not found.` pour l'encodeur Microsoft ; encodez alors en H.264 sur cet ordinateur. Dans MF Mux, les encodeurs H.264 et H.265 de Microsoft acceptent une vidéo de 1920 pixels de large et 1080 pixels de haut au maximum ; utilisez un encodeur GPU (NVENC, Quick Sync, AMF) pour une vidéo plus grande. H.264 est disponible sur toutes les éditions de Windows dotées des composants multimédias, en matériel ou en logiciel ; les éditions N et KN de Windows nécessitent d'abord le Media Feature Pack, et Windows Server la fonctionnalité Media Foundation. La prise en charge de H.265 dans MF Mux exige une version actuelle du filtre : les anciennes versions (la version 11 de 2019, par exemple) ne l'ont pas. Les filtres actuels font partie de l'[Encoding Filters Pack](filters-enc/index.md).
