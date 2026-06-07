---
title: Bloc analyseur de flux de transport MPEG-TS pour C# .NET
description: Analysez les programmes MPEG-TS, le débit par PID, la synchro PCR et la conformité ETSI TR 101 290 en C# avec le TSAnalyzerBlock du Media Blocks SDK .NET.
sidebar_label: TS Analyzer
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - MPEG-TS
  - Streaming
  - Analysis
  - TR 101 290
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerSettings
  - TSAnalyzerReport
  - UDPRAWMPEGTSSourceBlock

---

# Bloc analyseur de flux MPEG-TS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Le `TSAnalyzerBlock` transforme le [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) en un **moniteur MPEG-TS de qualité broadcast qui s'exécute au sein de votre propre application .NET** — sans outils externes, sans lancement d'un CLI, sans analyse de sortie texte. Pointez-le vers un fichier, un flux UDP multicast en direct ou un flux SRT, et il rapporte en continu la grille des programmes, le débit par PID, la synchro PCR, l'embrouillage, les informations de service et la conformité **ETSI TR 101 290** complète Priorité 1/2/3 — sous forme d'objets fortement typés que vous pouvez lier à un tableau de bord, journaliser ou utiliser pour déclencher des alertes.

Il analyse le flux de transport brut paquet par paquet (analyse PAT/PMT/PSI, compteurs de continuité, récupération d'horloge PCR, synchro PES, en-têtes de codec) et expose le tout à travers un unique `TSAnalyzerReport`. Le bloc s'exécute soit comme puits terminal (mode `Input`), soit comme passthrough en ligne (mode `InputOutput`), afin que vous puissiez analyser un flux **pendant que vous l'enregistrez ou le relayez encore**.

En bref, le bloc mesure :

- **Structure** — PAT/PMT/PSI, programmes/services, PID PMT/PCR, types de flux et codecs.
- **Débit** — débit par PID et total (moyenne glissante, instantané et crête), plus le bourrage par paquets nuls et le débit utile effectif.
- **Synchro** — intervalle PCR min/moy/max, discontinuités, gigue, erreurs de répétition ; présence des PTS/DTS PES et décalage de synchro audio/vidéo.
- **Intégrité** — erreurs de compteur de continuité, indicateur d'erreur de transport, CRC-32 des tables PSI et la liste complète des contrôles TR 101 290 P1/P2/P3.
- **Informations de service** — nom/fournisseur/type de service SDT, nom de réseau NIT, heure UTC du flux TDT/TOT, langue audio ISO 639, état d'embrouillage et (optionnellement) événements EIT/EPG.
- **Détails de codec** — résolution, profil, niveau, format de chrominance et rapport d'aspect pour la vidéo H.264, HEVC et MPEG-2.

## Fonctionnement

En mode `Input`, le bloc est le terminal du pipeline — il consomme le flux de transport et produit le rapport :

```mermaid
graph LR;
    Source["Source (fichier / UDP / SRT)"]-->TSAnalyzerBlock;
```

En mode `InputOutput`, le flux intact est transféré vers le pad de sortie, ce qui vous permet d'analyser et d'enregistrer/relayer en même temps :

```mermaid
graph LR;
    Source["Source (fichier / UDP / SRT)"]-->TSAnalyzerBlock;
    TSAnalyzerBlock-->MPEGTSSinkBlock;
```

### Informations sur le bloc

Nom : TSAnalyzerBlock.

| Direction du pad | Type de média | Nombre de pads |
| --- | :---: | :---: |
| Entrée | Flux d'octets MPEG-TS | 1 |
| Sortie | Flux d'octets MPEG-TS (passthrough) | 1 en mode `InputOutput`, 0 en mode `Input` |

## Modes

Le constructeur prend un `TSAnalyzerMode` :

| Mode | Description |
| --- | --- |
| `TSAnalyzerMode.Input` | Analyseur terminal. Le bloc accepte le flux et n'expose aucun pad de sortie — utilisez-le lorsque vous n'avez besoin que du rapport. |
| `TSAnalyzerMode.InputOutput` | Analyseur passthrough. Le flux est transféré inchangé vers le pad de sortie pendant qu'il est analysé, ce qui vous permet de l'enregistrer ou de le relayer dans le même pipeline. |

```csharp
TSAnalyzerBlock(TSAnalyzerMode mode = TSAnalyzerMode.Input, TSAnalyzerSettings settings = null)
```

## Démarrage rapide

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Special;

await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Flux MPEG-TS UDP multicast en direct (octets bruts, sans démux)
var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

// Analyseur terminal avec une cadence de capture d'une seconde
var analyzer = new TSAnalyzerBlock(
    TSAnalyzerMode.Input,
    new TSAnalyzerSettings { StatisticsInterval = TimeSpan.FromSeconds(1) });

analyzer.OnAnalysisUpdated += (sender, e) =>
{
    TSAnalyzerReport report = e.Report;        // e.IsFinal == true en fin de flux
    Console.WriteLine($"{report.TotalBitrateKbps:F0} kbps, programmes : {report.Programs.Count}");

    if (report.Tr101290 != null && report.Tr101290.P1Count > 0)
    {
        Console.WriteLine($"Erreurs TR 101 290 Priorité 1 : {report.Tr101290.P1Count}");
    }
};

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
// ... exécuter jusqu'à la fin du flux ou un arrêt utilisateur ...
await pipeline.StopAsync();

// Vous pouvez aussi récupérer une capture à la demande à tout moment :
TSAnalyzerReport final = analyzer.GetReport();
```

## Alimenter l'analyseur (fichier / UDP / SRT)

L'analyseur accepte un flux d'octets MPEG-TS brut depuis tout bloc source qui en produit un. Les trois alimentations courantes :

```csharp
// 1. Fichier local (.ts / .m2ts)
var fileSource = new BasicFileSourceBlock("stream.ts");
pipeline.Connect(fileSource.Output, analyzer.Input);

// 2. UDP en direct (unicast ou multicast) — MPEG-TS brut, sans démux
var udpSource = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));
pipeline.Connect(udpSource.Output, analyzer.Input);

// 3. SRT en direct
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://server:9000"), ignoreMediaInfoReader: true);
var srtSource = new SRTRAWSourceBlock(srtSettings);
pipeline.Connect(srtSource.Output, analyzer.Input);
```

### UDPRAWMPEGTSSourceBlock

`UDPRAWMPEGTSSourceBlock` reçoit un flux de transport UDP unicast ou multicast en direct et expose le flux d'octets MPEG-TS **intact** sur son pad `Output` — sans démultiplexage — ce qui est exactement ce dont l'analyseur a besoin. Il se configure via `UDPRAWMPEGTSSourceSettings` :

| Propriété | Type | Par défaut | Description |
| --- | :---: | :---: | --- |
| `Uri` | `Uri` | — | URI source, p. ex. `udp://239.0.0.1:1234` (multicast) ou `udp://0.0.0.0:1234` (toutes les interfaces). |
| `AutoJoinMulticast` | `bool` | `true` | Rejoindre/quitter automatiquement le groupe multicast pour les adresses multicast. |
| `MulticastInterface` | `string` | `null` | Interface(s) réseau sur laquelle/lesquelles rejoindre le groupe multicast (p. ex. `"eth0"` ou `"eth0,eth1"`). |
| `BufferSize` | `int` | `524288` | Taille du tampon de réception du noyau en octets ; `0` utilise la valeur par défaut de l'OS. |
| `PacketSize` | `int` | `188` | Taille annoncée des paquets TS : `188` (standard) ou `192` (style M2TS avec un préfixe timecode de 4 octets). |

```csharp
var settings = new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234")
{
    AutoJoinMulticast = true,
    PacketSize = 188
};

var source = new UDPRAWMPEGTSSourceBlock(settings);
```

> **Bloc associé.** Si vous souhaitez que le flux soit **démultiplexé** en pads vidéo/audio encodés séparés (pour un enregistrement ou un remultiplexage sans réencodage) plutôt que le flux d'octets brut, utilisez plutôt `UDPRAWSourceBlock`. Consultez le [guide d'enregistrement UDP MPEG-TS](../Guides/udp-mpegts-record-without-reencoding.md).

## Paramètres

`TSAnalyzerSettings` contrôle la cadence de capture et les étapes d'analyse activées. Les étapes les plus lourdes peuvent être désactivées lorsque vous n'en avez pas besoin.

| Propriété | Type | Par défaut | Description |
| --- | :---: | :---: | --- |
| `StatisticsInterval` | `TimeSpan` | `1 seconde` | Fréquence à laquelle une capture est produite et `OnAnalysisUpdated` est déclenché. |
| `TrackPCR` | `bool` | `true` | Suivre la synchro PCR (statistiques d'intervalle et discontinuités). |
| `ValidateContinuity` | `bool` | `true` | Valider les erreurs de compteur de continuité par PID. |
| `CalculateBitrate` | `bool` | `true` | Calculer le débit par PID et total. |
| `PacketSize` | `TSPacketSizeMode` | `Auto` | Gestion de la taille des paquets TS — `Auto`, `Size188` ou `Size192`. |
| `ParseServiceInfo` | `bool` | `true` | Analyser les informations de service DVB (SDT, NIT, TDT/TOT) pour les noms de service et de réseau. |
| `ParseEPG` | `bool` | `false` | Analyser l'EIT pour les événements EPG. À activer explicitement — c'est l'étape la plus lourde. |
| `TrackScrambling` | `bool` | `true` | Suivre l'embrouillage via les bits `transport_scrambling_control`. |
| `ValidateTR101290` | `bool` | `true` | Évaluer les contrôles ETSI TR 101 290 Priorité 1/2/3. |
| `TrackPtsDts` | `bool` | `true` | Suivre la présence des PTS/DTS PES et la synchro audio/vidéo. |
| `ParseCodecDetails` | `bool` | `true` | Analyser les détails de codec (résolution/profil/niveau) depuis les en-têtes SPS/séquence. |

Valeurs de `TSPacketSizeMode` : `Auto` (détecter 188 ou 192 d'après l'espacement des octets de synchronisation), `Size188` (MPEG-TS standard), `Size192` (M2TS / Blu-ray avec un préfixe d'horodatage de 4 octets).

## Le modèle de rapport

Chaque capture est un `TSAnalyzerReport`. Les champs de premier niveau résument l'ensemble du flux ; les collections imbriquées décrivent les programmes, les PID et la synchro.

| Propriété | Type | Description |
| --- | :---: | --- |
| `PacketSize` | `int` | Taille de paquet TS détectée (188 ou 192). |
| `TotalBitrateKbps` | `double` | Débit total du flux (moyenne glissante depuis le début de l'analyse). |
| `TotalPackets` | `long` | Total de paquets TS analysés. |
| `HasPAT` | `bool` | Indique si une PAT (PID 0) a été vue. |
| `TransportErrors` | `long` | Paquets dont l'indicateur d'erreur de transport est positionné. |
| `Programs` | `List<TSProgramInfo>` | Programmes/services découverts depuis PAT + PMT. |
| `Pids` | `List<TSPidInfo>` | Statistiques par PID. |
| `Pcr` | `List<TSPcrStats>` | Statistiques de synchro PCR (une entrée par PID PCR). |
| `NullPacketCount` | `long` | Nombre de paquets de bourrage nul (PID 0x1FFF). |
| `NullBitrateKbps` | `double` | Débit consommé par le bourrage nul. |
| `EffectiveBitrateKbps` | `double` | Débit utile effectif (total moins le bourrage nul). |
| `SyncByteErrors` | `long` | Erreurs d'octet de synchronisation (0x47 absent à la position attendue). |
| `SyncLossEvents` | `long` | Événements de perte de synchronisation (resynchronisations de l'analyseur). |
| `NetworkName` | `string` | Nom de réseau issu du descripteur de réseau NIT, ou `null`. |
| `StreamTimeUtc` | `DateTime?` | Heure UTC du flux issue de TDT/TOT, ou `null`. |
| `AvSyncMs` | `double?` | Décalage de synchro audio/vidéo en ms (PTS vidéo moins PTS audio), ou `null`. |
| `Tr101290` | `TSTR101290Report` | Le rapport de mesure TR 101 290, ou `null` si la validation est désactivée. |
| `Events` | `List<TSEpgEvent>` | Événements EPG issus de l'EIT (lorsque `ParseEPG` est activé). |

### Programmes et flux élémentaires

`TSProgramInfo` (par programme/service) :

| Propriété | Type | Description |
| --- | :---: | --- |
| `ProgramNumber` | `int` | Numéro de programme (ID de service). |
| `PmtPid` | `int` | PID de la PMT pour ce programme. |
| `PcrPid` | `int` | PID transportant le PCR pour ce programme. |
| `Streams` | `List<TSElementaryStreamInfo>` | Flux élémentaires de ce programme. |
| `ServiceName` | `string` | Nom de service issu de la SDT, ou `null`. |
| `ServiceProvider` | `string` | Fournisseur de service issu de la SDT, ou `null`. |
| `ServiceType` | `int` | Octet `service_type` DVB (p. ex. `0x01` = télévision numérique). |
| `IsScrambled` | `bool` | Indique si le service est embrouillé (`free_CA_mode` de la SDT). |

`TSElementaryStreamInfo` (par flux élémentaire) :

| Propriété | Type | Description |
| --- | :---: | --- |
| `Pid` | `int` | PID du flux élémentaire. |
| `StreamType` | `byte` | Octet `stream_type` MPEG-TS. |
| `Codec` | `string` | Nom de codec/contenu lisible. |
| `Kind` | `TSPidKind` | Type de média classifié (Video, Audio, Private, …). |
| `Language` | `string` | Code de langue ISO 639 issu du descripteur PMT, ou `null`. |
| `CodecDetails` | `TSCodecDetails` | Détails de codec analysés, ou `null`. |

`TSCodecDetails` : `Width`, `Height`, `FrameRateNum`/`FrameRateDen`, `Profile`, `Level`, `ChromaFormat`, `AspectRatio`. La résolution est analysée pour H.264, HEVC et MPEG-2. Le profil, le niveau et le format de chrominance sont analysés pour H.264 et HEVC uniquement. La fréquence d'images et le rapport d'aspect sont lus dans l'en-tête de séquence MPEG-1/2 (pas encore depuis la VUI H.264/HEVC).

### Statistiques par PID

`TSPidInfo` :

| Propriété | Type | Description |
| --- | :---: | --- |
| `Pid` | `int` | Identifiant de paquet. |
| `Kind` | `TSPidKind` | Rôle classifié du PID. |
| `StreamType` | `byte` | Octet `stream_type` (pour les flux élémentaires). |
| `Codec` | `string` | Nom de codec/contenu lisible. |
| `BitrateKbps` | `double` | Débit moyen (moyenne glissante). |
| `InstantBitrateKbps` | `double` | Débit le plus récent par intervalle. |
| `PeakBitrateKbps` | `double` | Débit de crête par intervalle observé. |
| `PacketCount` | `long` | Total de paquets sur ce PID. |
| `ContinuityErrors` | `long` | Erreurs de compteur de continuité détectées. |
| `IsPcrPid` | `bool` | Indique si ce PID transporte le PCR. |
| `IsScrambled` | `bool` | Indique si des paquets embrouillés ont été observés. |
| `ScrambledPacketCount` | `long` | Paquets avec `transport_scrambling_control` non nul. |
| `IsUnreferenced` | `bool` | PID non référencé par PAT/PMT/PSI et non bourrage nul. |
| `HasPts` / `HasDts` | `bool` | Indique si un PTS / DTS a été vu dans la charge utile PES. |

### Synchro PCR

`TSPcrStats` : `Pid`, `SampleCount`, `MinIntervalMs`, `AvgIntervalMs`, `MaxIntervalMs`, `Discontinuities`, `MaxJitterMs` (plus grand écart par rapport à l'intervalle moyen glissant) et `RepetitionErrors` (intervalles dépassant 40 ms selon TR 101 290).

## ETSI TR 101 290

TR 101 290 est la norme de mesure DVB pour la surveillance des flux de transport. Elle regroupe les erreurs en trois priorités — **P1** (doivent être présentes pour décoder), **P2** (recommandées pour une surveillance continue) et **P3** (informations de service/SI optionnelles). Lorsque `ValidateTR101290` est activé, l'analyseur remplit `report.Tr101290` :

| Propriété | Type | Description |
| --- | :---: | --- |
| `Checks` | `List<TSTR101290Check>` | Contrôles individuels sur tous les groupes de priorité. |
| `P1Count` / `P2Count` / `P3Count` | `long` | Nombre total d'erreurs par groupe de priorité. |
| `TotalCount` | `long` | Somme sur tous les groupes. |
| `AllOk` | `bool` | `true` lorsque chaque contrôle a réussi (`TotalCount == 0`). |

Chaque `TSTR101290Check` possède un `Name` (p. ex. `PAT_error`, `CRC_error`), une `Priority` (1/2/3), un `Count` et un `Ok`. Les contrôles évalués comprennent :

- **Priorité 1 :** TS_sync_loss, Sync_byte_error, PAT_error, Continuity_count_error, PMT_error, PID_error.
- **Priorité 2 :** Transport_error, CRC_error, PCR_error, PCR_repetition_error (>40 ms), PCR_discontinuity_indicator_error, PCR_accuracy_error, PTS_error (>700 ms), CAT_error.
- **Priorité 3 :** NIT_error, SI_repetition_error, Unreferenced_PID, SDT_error, EIT_error, TDT_error.

```csharp
var tr = report.Tr101290;
if (tr != null && !tr.AllOk)
{
    foreach (var check in tr.Checks.Where(c => c.Count > 0))
    {
        Console.WriteLine($"P{check.Priority} {check.Name}: {check.Count}");
    }
}
```

> **Pourquoi c'est important.** Un flux peut se lire dans un lecteur multimédia tout en restant non conforme — un PCR qui se répète plus lentement que 40 ms, un saut de CC, une PMT manquante. TR 101 290 est la manière dont les opérateurs broadcast quantifient cela. Le `TSAnalyzerBlock` vous donne les mêmes verdicts structurés P1/P2/P3 qu'un analyseur matériel dédié, directement en code managé.

## Mises à jour en direct vs captures à la demande

Il y a deux façons de lire le rapport :

- **`OnAnalysisUpdated`** — déclenché à chaque `StatisticsInterval` avec une capture fraîche, et une fois de plus en fin de flux avec `IsFinal == true`. Les mises à jour périodiques s'exécutent sur un thread de minuteur en arrière-plan ; le rapport final s'exécute sur le thread GStreamer. Marshalez vers le thread d'interface utilisateur avant de toucher aux éléments d'interface.
- **`GetReport()`** — retourne la capture courante à la demande (ou `null` si le bloc n'a pas été construit). Elle ne perturbe pas la comptabilité périodique du débit instantané/de crête.

```csharp
analyzer.OnAnalysisUpdated += (sender, e) =>
{
    // e.IsFinal distingue les mises à jour périodiques du rapport de fin de flux.
    Dispatcher.BeginInvoke(() => UpdateUI(e.Report));   // WPF : marshaler vers le thread d'interface
};
```

## Exemples d'applications

- **WPF — TS Analyzer Demo :** une interface complète avec une grille par PID, un panneau TR 101 290 et une ligne de synthèse (nom de service, débit nul/effectif, nom de réseau, UTC du flux, synchro A/V). [Parcourir le code source](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/TS%20Analyzer%20Demo).
- **Console — TS Analyzer CLI :** analyse un fichier ou un flux `udp://` / `srt://` en direct et affiche le rapport complet. [Parcourir le code source](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/TS%20Analyzer%20CLI).

## Disponibilité

Appelez `TSAnalyzerBlock.IsAvailable()` pour vérifier que l'analyseur est disponible dans l'environnement courant avant de créer une instance.

## Plateformes

Windows, macOS, Linux, iOS, Android.

## Voir aussi

- [Analyse MPEG-TS en C# : VisioForge vs ffprobe](../Guides/mpeg-ts-analysis-vs-ffprobe.md) — comment le bloc se compare à ffprobe et aux analyseurs broadcast professionnels.
- [Analyser et valider un flux MPEG-TS en C#](../Guides/mpeg-ts-stream-validation-csharp.md) — un guide de tâche pas à pas.
- [Enregistrement UDP MPEG-TS sans réencodage](../Guides/udp-mpegts-record-without-reencoding.md).
