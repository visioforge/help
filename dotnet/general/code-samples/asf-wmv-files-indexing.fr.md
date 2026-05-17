---
title: Indexation des fichiers ASF et WMV avec le SDK .NET
description: Découvrez pourquoi ASF, WMV et WMA ont besoin d'indexation pour une recherche fiable, et comment ajouter des index avant ouverture dans VisioForge .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - WinForms
  - Streaming
  - WMV
  - WMA
  - C#

---

# Indexation des fichiers ASF et WMV en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Lorsque vous travaillez avec des fichiers Windows Media dans vos applications .NET, vous pouvez rencontrer des problèmes de recherche avec les fichiers ASF, WMV ou WMA produits sans index approprié. Cette page explique le problème sous-jacent et indique l'outil approprié pour construire l'index avant que VisioForge ne consomme le fichier.

## Comprendre le problème d'indexation

ASF (Advanced Systems Format) est le format conteneur de Microsoft conçu pour le streaming multimédia ; WMV (Windows Media Video) et WMA (Windows Media Audio) sont construits dessus. Les fichiers dépourvus d'index présentent :

- Un comportement de recherche saccadé ou imprévisible
- Une incapacité à sauter à des horodatages spécifiques
- Une lecture incohérente lors de la navigation dans le fichier
- Une surcharge élevée lors de l'accès aléatoire

Un index ASF est une table de correspondance qui mappe les horodatages (ou les numéros d'image) aux décalages d'octets dans le fichier. Lorsqu'il est présent, les lecteurs peuvent sauter directement à n'importe quel point du flux ; lorsqu'il est absent, ils doivent recourir à un parcours séquentiel.

## Construction d'un index ASF

VisioForge consomme les fichiers ASF/WMV/WMA une fois qu'ils sont indexés, mais ne propose pas d'indexeur public sur sa surface managée. Construisez l'index avec l'un des outils externes suivants avant de remettre le fichier au SDK :

- **Windows Media Format SDK** (interfaces COM `IWMWriterFileSink` / `IWMIndexer`, disponibles via `Microsoft.Windows.WindowsMedia.Format`). C'est le chemin canonique de Microsoft pour l'indexation hors-ligne ; la méthode `IWMIndexer::StartIndexing` écrit un objet `WM/Index` dans le fichier.
- **Windows Media File Editor** (`WMFileEditor.exe`, partie des outils Windows Media Encoder 9) pour une indexation ponctuelle pendant le développement.
- **`ffmpeg -i input.wmv -c copy -map 0 -f asf output.wmv`** — le remuxage via ffmpeg réécrira le conteneur ASF avec un index frais dans la plupart des cas, sans ré-encodage.

Une fois que le fichier porte un index valide, tous les moteurs VisioForge (`MediaPlayerCore`, `MediaPlayerCoreX`, `VideoEditCore`, `VideoEditCoreX`) effectueront des recherches précises et rapporteront des durées cohérentes via les API habituelles `Duration`/`Position_Get*`.

## Bonnes pratiques pour les workflows ASF/WMV

1. **Détectez les index manquants en amont.** Si `Duration` est rapporté à zéro ou que la recherche renvoie la mauvaise image, soupçonnez un index ASF manquant ou corrompu.
2. **Indexez une seule fois par fichier.** L'indexation réécrit le fichier sur le disque ; faites-le lors de l'ingestion, pas au moment de la lecture.
3. **Mettez en cache les copies indexées.** Lorsqu'un utilisateur charge un fichier non indexé, persistez la version indexée sur disque et faites pointer les sessions futures dessus au lieu de réindexer.
4. **Exécutez l'indexation hors du thread d'interface utilisateur.** Les fichiers volumineux peuvent prendre plusieurs secondes à indexer ; encapsulez l'opération dans `Task.Run` pour garder votre UI réactive.
5. **Préférez MP4 pour les nouveaux enregistrements.** Si vous contrôlez le pipeline de capture, le `MP4Output` de VisioForge produit des fichiers cherchables sans étape d'indexation séparée.

## Configuration système requise

L'indexation est un workflow uniquement Windows car le conteneur ASF lui-même est une technologie Windows Media :

- Runtime Windows Media Format SDK (intégré à Windows 7 et versions ultérieures)
- Accès en écriture au fichier cible
- Suffisamment d'espace disque libre pour réécrire le conteneur (l'indexation ajoute des métadonnées et, dans certains cas, re-sérialise le flux)

## Voir aussi

- [Référence de l'encodage WMV](../output-formats/wmv.md) — configurez la sortie WMV de VisioForge pour produire des fichiers indexés à la capture.
- [Windows Media Format SDK — IWMIndexer](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/api/wmsdkidl/nn-wmsdkidl-iwmindexer)
- [Sortie MP4](../output-formats/mp4.md) — une alternative compatible avec la recherche pour les nouveaux projets.

---
Pour plus d'exemples de code et de techniques avancées de traitement multimédia, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
