---
title: Analyser et valider des flux MPEG-TS en C# .NET pas à pas
description: Guide C# pas à pas pour analyser un fichier ou un flux MPEG-TS UDP/SRT en direct, lire ETSI TR 101 290 et alerter sur les erreurs avec le Media Blocks SDK .NET.
sidebar_label: MPEG-TS Stream Validation
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - Analysis
  - TR 101 290
  - UDP
  - SRT
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerSettings
  - TSAnalyzerReport
  - UDPRAWMPEGTSSourceBlock

---

# Analyser et valider des flux MPEG-TS en C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Ce guide montre comment analyser et valider un flux de transport MPEG-TS depuis C# à l'aide du `TSAnalyzerBlock` du [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net). Vous allez :

1. Analyser un fichier `.ts` / `.m2ts` local et lire le rapport final.
2. Surveiller une alimentation `udp://` ou `srt://` **en direct** avec des mises à jour périodiques.
3. Lire les résultats **ETSI TR 101 290** et déclencher une alerte lorsque le flux est non conforme.
4. Analyser une alimentation **pendant que vous l'enregistrez** à l'aide du mode passthrough.

Pour la référence complète de l'API (chaque paramètre et champ de rapport), consultez la page [Bloc analyseur de flux MPEG-TS](../Special/TSAnalyzerBlock.md).

## Prérequis

Installez le paquet NuGet du Media Blocks SDK et le paquet d'exécution de la plateforme :

- `VisioForge.DotNet.MediaBlocks`
- Un runtime de plateforme, p. ex. `VisioForge.CrossPlatform.Core.Windows.x64` sous Windows.

Initialisez le SDK une fois au démarrage :

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

Les espaces de noms utilisés tout au long de ce guide :

```csharp
using System;
using System.Linq;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Special;
```

## 1. Analyser un fichier

Pour un fichier, exécutez le pipeline jusqu'à la fin du flux et lisez le rapport final. Connectez l'analyseur en mode `Input` (terminal) :

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new BasicFileSourceBlock("stream.ts");
var analyzer = new TSAnalyzerBlock(TSAnalyzerMode.Input);

// Signaler l'achèvement lorsque le pipeline s'arrête de lui-même (fin de flux).
var completed = new TaskCompletionSource<bool>();
pipeline.OnStop += (s, e) => completed.TrySetResult(true);

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
await completed.Task;             // attendre la fin du flux
await pipeline.StopAsync();

TSAnalyzerReport report = analyzer.GetReport();

Console.WriteLine($"Taille de paquet : {report.PacketSize}  |  total : {report.TotalBitrateKbps:F0} kbps");
Console.WriteLine($"Programmes : {report.Programs.Count}  |  PAT : {(report.HasPAT ? "oui" : "non")}");

foreach (var program in report.Programs)
{
    var name = string.IsNullOrEmpty(program.ServiceName) ? "(sans nom)" : program.ServiceName;
    Console.WriteLine($"Programme {program.ProgramNumber} \"{name}\" (PMT {program.PmtPid}, PCR {program.PcrPid})");

    foreach (var es in program.Streams)
    {
        var lang = string.IsNullOrEmpty(es.Language) ? "" : $" [{es.Language}]";
        Console.WriteLine($"    PID {es.Pid}  {es.Kind}  {es.Codec}{lang}");
    }
}
```

## 2. Surveiller un flux en direct

Pour une alimentation `udp://` ou `srt://` en direct, abonnez-vous à `OnAnalysisUpdated` et lisez chaque capture périodique. Définissez `StatisticsInterval` pour contrôler la cadence.

```csharp
var pipeline = new MediaBlocksPipeline();

// UDP multicast en direct (MPEG-TS brut, sans démux)
var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

var analyzer = new TSAnalyzerBlock(
    TSAnalyzerMode.Input,
    new TSAnalyzerSettings { StatisticsInterval = TimeSpan.FromSeconds(1) });

analyzer.OnAnalysisUpdated += (sender, e) =>
{
    if (e.IsFinal)
    {
        return;     // rapport de fin de flux ; les captures périodiques ont IsFinal == false
    }

    TSAnalyzerReport report = e.Report;
    Console.WriteLine(
        $"{report.TotalBitrateKbps:F0} kbps total, " +
        $"{report.EffectiveBitrateKbps:F0} kbps effectif, " +
        $"programmes : {report.Programs.Count}");
};

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
// ... continuer à exécuter pendant la surveillance ...
```

Pour alimenter du SRT au lieu de l'UDP, remplacez la source :

```csharp
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://server:9000"), ignoreMediaInfoReader: true);
var source = new SRTRAWSourceBlock(srtSettings);
```

> **Threading.** Les rappels périodiques `OnAnalysisUpdated` s'exécutent sur un thread de minuteur en arrière-plan. Dans une application avec interface, marshalez vers le thread d'interface utilisateur avant de mettre à jour les contrôles — p. ex. `Dispatcher.BeginInvoke(() => UpdateUI(e.Report));` en WPF.

## 3. Lire TR 101 290 et alerter sur les erreurs

Le rapport TR 101 290 regroupe les erreurs par priorité. Un schéma courant consiste à alerter dès qu'une erreur de Priorité 1 (critique) apparaît, et à journaliser les contrôles en échec individuels :

```csharp
analyzer.OnAnalysisUpdated += (sender, e) =>
{
    var tr = e.Report.Tr101290;
    if (tr == null || tr.AllOk)
    {
        return;     // TR 101 290 désactivé, ou le flux est entièrement conforme
    }

    if (tr.P1Count > 0)
    {
        RaiseAlert($"Erreurs TR 101 290 critiques (P1) : {tr.P1Count}");
    }

    foreach (var check in tr.Checks.Where(c => c.Count > 0))
    {
        Console.WriteLine($"P{check.Priority} {check.Name}: {check.Count}");
    }
};
```

`tr.P1Count` / `P2Count` / `P3Count` vous donnent les totaux par priorité ; `tr.Checks` vous permet d'aller dans le détail d'une mesure en échec spécifique (par exemple `PCR_repetition_error` ou `Continuity_count_error`). Utilisez `tr.AllOk` comme porte de conformité en une seule ligne.

## 4. Analyser pendant l'enregistrement (passthrough)

Pour valider une alimentation et l'enregistrer dans le même pipeline, créez l'analyseur en mode `InputOutput` et connectez son `Output` à un puits. Le flux de transport est transféré inchangé, de sorte que l'enregistrement est une copie bit à bit :

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

// Analyseur passthrough : transfère le flux pendant qu'il l'analyse
var analyzer = new TSAnalyzerBlock(TSAnalyzerMode.InputOutput);
analyzer.OnAnalysisUpdated += (s, e) => { /* surveiller pendant l'enregistrement */ };

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("recording.ts"));

pipeline.Connect(source.Output, analyzer.Input);
pipeline.Connect(analyzer.Output, sink.Input);

await pipeline.StartAsync();
```

## Dépannage

- **`HasPAT` est à false / aucun programme.** L'entrée n'est probablement pas un flux d'octets MPEG-TS valide, ou la taille de paquet a été mal détectée. Forcez-la via `TSAnalyzerSettings.PacketSize` (`Size188` ou `Size192`), ou vérifiez l'URI source.
- **De nombreuses entrées `PCR_repetition_error` sur un fichier valide.** C'est attendu lorsque le multiplexeur émet le PCR plus lentement que la limite de 40 ms de TR 101 290 (courant pour le multiplexage orienté fichier). Le flux se lit toujours ; il n'est simplement pas conforme au PCR de 40 ms.
- **`GetReport()` retourne null.** Le bloc n'a pas encore été construit — appelez-le après `StartAsync()`.
- **Aucun paquet multicast reçu.** Vérifiez que `AutoJoinMulticast` est à `true` (la valeur par défaut) et définissez `MulticastInterface` si l'hôte possède plusieurs interfaces réseau.
- **CPU élevé sur un flux lourd.** Désactivez les étapes dont vous n'avez pas besoin via `TSAnalyzerSettings` — `ParseEPG` est désactivé par défaut ; vous pouvez aussi désactiver `ParseCodecDetails` ou `TrackPtsDts`.

## Foire aux questions

### Comment analyser un flux MPEG-TS UDP multicast en direct en C# ?

Créez un `UDPRAWMPEGTSSourceBlock` avec une URI `udp://group:port`, connectez son `Output` à un `TSAnalyzerBlock` en mode `Input`, et abonnez-vous à `OnAnalysisUpdated` pour les captures périodiques. Consultez la [section 2](#2-surveiller-un-flux-en-direct).

### Puis-je vérifier la conformité ETSI TR 101 290 depuis .NET ?

Oui. Avec `ValidateTR101290` activé (la valeur par défaut), `report.Tr101290` expose la liste des contrôles Priorité 1/2/3 avec les comptes par contrôle et le statut réussite/échec, plus les totaux `P1Count`/`P2Count`/`P3Count` et un indicateur `AllOk`. Consultez la [section 3](#3-lire-tr-101-290-et-alerter-sur-les-erreurs).

### Puis-je enregistrer un flux et l'analyser en même temps ?

Oui. Utilisez `TSAnalyzerMode.InputOutput` et connectez l'`Output` de l'analyseur à un puits tel que `MPEGTSSinkBlock`. Le flux est transféré inchangé. Consultez la [section 4](#4-analyser-pendant-lenregistrement-passthrough).

## Voir aussi

- [Bloc analyseur de flux MPEG-TS](../Special/TSAnalyzerBlock.md) — la référence complète du bloc.
- [Analyse MPEG-TS en C# : VisioForge vs ffprobe](mpeg-ts-analysis-vs-ffprobe.md) — positionnement et comparaison des fonctionnalités.
