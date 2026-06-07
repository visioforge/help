---
title: "Analyse MPEG-TS en C# .NET : VisioForge face à ffprobe"
description: Analyse MPEG-TS .NET in-process avec surveillance ETSI TR 101 290 en direct — comment le TS Analyzer du Media Blocks SDK se compare à ffprobe.
sidebar_label: TS Analysis vs ffprobe
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - Analysis
  - TR 101 290
  - ffprobe
  - Streaming
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerReport
  - TSAnalyzerSettings

---

# Analyse MPEG-TS en C# : VisioForge vs ffprobe

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Pourquoi c'est important

ffprobe est un excellent outil en ligne de commande pour jeter un coup d'œil rapide à un fichier multimédia. Mais dès l'instant où vous avez besoin d'analyser un flux de transport **au sein d'une application .NET** — un tableau de bord de surveillance en direct, un validateur d'ingestion, une porte de contrôle qualité automatisée — le modèle CLI commence à jouer contre vous : vous devez lancer un processus séparé, capturer sa sortie texte ou JSON, l'analyser, et vous n'obtenez toujours qu'une capture unique sans cadre de conformité.

Le `TSAnalyzerBlock` du [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) est conçu précisément pour cette tâche. C'est un **moniteur MPEG-TS in-process, en direct, de qualité broadcast** qui rapporte des résultats fortement typés en continu, avec la conformité ETSI **TR 101 290** complète Priorité 1/2/3 — ce que ffprobe ne fournit tout simplement pas.

## Ce qui distingue le TS Analyzer

### 1. In-process, fortement typé — sans CLI, sans analyse de texte

Vous connectez un bloc source à l'analyseur et vous lisez un objet `TSAnalyzerReport`. Pas de `Process.Start`, pas de capture stdout, pas d'analyse fragile de chaînes/JSON, pas de format de sortie dépendant de la version à suivre. Chaque métrique est une propriété typée que vous pouvez lier, journaliser ou tester directement.

### 2. En direct et piloté par événements

ffprobe est à usage unique : il s'exécute, affiche, se termine. Le `TSAnalyzerBlock` déclenche `OnAnalysisUpdated` selon une cadence configurable (chaque seconde par défaut) pendant toute la vie du flux — parfait pour les tableaux de bord temps réel, la détection de dérive et l'alerte. Un rapport final est délivré en fin de flux.

### 3. TR 101 290 de qualité broadcast — intégré

L'analyseur évalue la liste complète des contrôles TR 101 290 Priorité 1/2/3 (perte de synchronisation, erreurs PAT/PMT/PID, erreurs de compteur de continuité, erreurs de transport, erreurs CRC, erreurs PCR/PTS, erreurs SDT/EIT/TDT/NIT) et retourne chaque contrôle avec sa priorité, son nombre d'erreurs et son statut réussite/échec. ffprobe n'a aucun cadre de conformité équivalent — vous devriez en construire un vous-même au-dessus de sa sortie.

### 4. Mode passthrough — analyser pendant que vous enregistrez ou relayez

En mode `InputOutput`, le flux de transport d'origine est transféré inchangé vers le bloc suivant, ce qui vous permet de valider une alimentation **en même temps** que vous l'enregistrez ou la rediffusez — zéro remultiplexage, un seul pipeline. ffprobe se trouve entièrement à l'extérieur de votre chemin média.

### 5. Une seule API, toute source, toute plateforme

Fichier, UDP (unicast et multicast) et SRT alimentent tous le même analyseur via la même API, sous Windows, macOS, Linux, iOS et Android.

## Comparaison des fonctionnalités

La matrice ci-dessous reflète les capacités **actuelles** du `TSAnalyzerBlock`. ✅ = pris en charge, ⚠️ = partiel/indirect, ❌ = non disponible.

| Capacité | VisioForge TS Analyzer | ffprobe |
| --- | :---: | :---: |
| Détection auto de la taille de paquet 188/192 | ✅ | ✅ |
| PAT/PMT, programmes, PID PCR | ✅ | ✅ |
| stream_type → codec | ✅ | ✅ |
| Débit par PID + nombre de paquets | ✅ | ⚠️ |
| Débit instantané + crête par PID | ✅ | ❌ |
| Erreurs de compteur de continuité | ✅ | ⚠️ |
| Intervalle PCR min/moy/max + discontinuités | ✅ | ❌ |
| Gigue PCR (max) + répétition >40 ms | ✅ | ❌ |
| Précision / dérive PCR (ppm) | ⚠️ | ❌ |
| Indicateur d'erreur de transport | ✅ | ⚠️ |
| % nul / bourrage / débit effectif | ✅ | ❌ |
| Embrouillage (TSC + free_CA_mode) | ✅ | ⚠️ |
| Présence PTS/DTS + décalage de synchro A/V | ✅ | ✅ |
| SDT → nom / fournisseur / type de service | ✅ | ⚠️ |
| Langue audio (ISO 639, PMT) | ✅ | ✅ |
| Nom de réseau NIT ; UTC du flux TDT/TOT | ✅ | ⚠️ |
| EIT → événements EPG | ✅ (à activer) | ⚠️ |
| Détection de PID non référencé | ✅ | ❌ |
| Résolution / profil / niveau / chrominance / aspect du codec | ✅ (H.264/HEVC/MPEG-2) | ✅ |
| Validation CRC-32 des PSI | ✅ | ⚠️ |
| **TR 101 290 P1/P2/P3 structuré** | ✅ | ❌ |
| **API .NET in-process (sans CLI / analyse)** | ✅ | ❌ |
| **Surveillance continue en direct (événements)** | ✅ | ❌ |
| **Passthrough (analyse pendant l'enregistrement/le relais)** | ✅ | ❌ |
| Multiplateforme (Windows/macOS/Linux/mobile) | ✅ | ✅ |

Quelques remarques honnêtes pour que le tableau reste digne de confiance. La couverture codec se fait par attribut, pas de manière uniforme : la **résolution** est analysée pour H.264, HEVC et MPEG-2 ; le **profil, le niveau et la chrominance** pour H.264/HEVC uniquement ; le **rapport d'aspect et la fréquence d'images** depuis l'en-tête de séquence MPEG-1/2 (pas encore depuis la VUI H.264/HEVC). Le champ de **dérive d'horloge PCR en ppm** est réservé et pas encore calculé (la gigue et la répétition le sont). ffprobe reste par ailleurs excellent pour les métadonnées exhaustives par codec et prend en charge une bien plus large gamme de formats de conteneur/codec au-delà de MPEG-TS.

## Quand utiliser lequel

- **Optez pour ffprobe** quand vous voulez un contrôle rapide en terminal d'un fichier multimédia quelconque, ou que vous scriptez un cas ponctuel dans un shell et que le format n'est peut-être pas MPEG-TS.
- **Optez pour le `TSAnalyzerBlock`** quand l'analyse de flux de transport doit vivre **au sein de votre application .NET** — surveillance continue, contrôle de conformité TR 101 290, tableaux de bord/alertes en direct, ou analyse d'une alimentation pendant que vous l'enregistrez ou la relayez simultanément.

Ils ne sont pas mutuellement exclusifs : de nombreuses équipes utilisent ffprobe au bureau et livrent VisioForge dans le produit.

## Étapes suivantes

- [Bloc analyseur de flux MPEG-TS](../Special/TSAnalyzerBlock.md) — la référence complète du bloc : modes, paramètres et modèle de rapport complet.
- [Analyser et valider un flux MPEG-TS en C#](mpeg-ts-stream-validation-csharp.md) — un guide de tâche pas à pas.
