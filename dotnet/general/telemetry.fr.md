---
title: Télémétrie et collecte de données dans les SDK .NET
description: Ce que les SDK .NET VisioForge signalent lorsqu'un débogueur est attaché, ce qui n'est jamais collecté et comment désactiver la télémétrie.
sidebar_label: Télémétrie et confidentialité
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
primary_api_classes:
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline

---

# Télémétrie et confidentialité

Chaque moteur expose une propriété `Debug_Telemetry`. Lorsqu'elle vaut `true` **et qu'un débogueur est attaché à votre processus**, le SDK signale ses propres erreurs à VisioForge, afin que les défaillances que vous rencontrez en intégrant le SDK nous parviennent sans que personne ait à ouvrir un ticket, ainsi que les noms des fonctionnalités du SDK que vous utilisez, afin que nous sachions lesquelles comptent pour vous.

Pour les erreurs d'un moteur, les trois conditions sont requises, et toutes sont vérifiées chaque fois qu'un événement serait envoyé ; les erreurs survenant hors d'un moteur ne demandent que les deux dernières (voir ci-dessous) :

- `Debug_Telemetry` vaut `true` (la valeur par défaut).
- `VisioForgeX.Debug_Telemetry` vaut `true` (la valeur par défaut) - l'interrupteur global, voir ci-dessous.
- `System.Diagnostics.Debugger.IsAttached` vaut `true`.

Une application que vous compilez et distribuez à vos utilisateurs s'exécute sans débogueur : **une application publiée n'envoie donc rien**. Détacher le débogueur arrête le signalement immédiatement ; en attacher un en cours de session le démarre.

## Comment la désactiver

Définissez la propriété sur `false` avant de démarrer le moteur :

```csharp
// N'importe lequel des moteurs - VideoCaptureCoreX, VideoCaptureCore, MediaPlayerCoreX,
// MediaPlayerCore, VideoEditCoreX, VideoEditCore, SimplePlayerX,
// LiveVideoCompositor, VideoFrameSource, FrameSource.
core.Debug_Telemetry = false;
```

```csharp
var pipeline = new MediaBlocksPipeline();
pipeline.Debug_Telemetry = false;
```

Pour tout désactiver d'un coup - tous les moteurs, ainsi que les erreurs survenant hors d'un moteur (fabriques de paramètres de source comme `UniversalSourceSettings.CreateAsync`, énumération des périphériques, lecture des informations média) et les exceptions qu'elles lèvent - utilisez l'interrupteur global. Ces erreurs hors moteur ne dépendent que de cet interrupteur, pas de la propriété d'un moteur :

```csharp
VisioForgeX.Debug_Telemetry = false;
```

La propriété peut être définie à tout moment ; ce qui compte, c'est son état à l'instant où l'erreur survient.

## Ce qui est envoyé

- Le nom de la classe et de la méthode ainsi que le niveau de l'événement de journal.
- Le message de journal, débarrassé des identifiants, chemins de fichiers, noms d'hôtes, adresses IP et MAC, identifiants d'appareil, GUID et adresses e-mail.
- Le type d'exception, son message expurgé et les trames de pile. Une trame conserve le nom de la méthode et le numéro de ligne ; le chemin du fichier source est supprimé, car il reflète l'arborescence de votre machine de compilation.
- Jusqu'à 50 lignes de journal précédentes en fil d'Ariane, expurgées de la même façon, ainsi que jusqu'à 30 avertissements et erreurs GStreamer précédents, expurgés de la même façon.
- Lorsqu'un pipeline GStreamer signale une erreur - un `MediaBlocksPipeline`, ou celui sur lequel tourne `VideoCaptureCoreX` ou `MediaPlayerCoreX` -, la première erreur de ce type de chaque pipeline au cours d'une exécution (un moteur peut en faire tourner plusieurs) emporte le graphe de ce pipeline : le type et le nom de chaque élément, la façon dont les éléments sont reliés, le format média de chaque liaison (par exemple `video/x-raw, format=NV12, width=1920, height=1080`), l'état des éléments, et les valeurs non par défaut des pads que GStreamer inscrit dans le graphe, comme la position d'une entrée de mixeur. Les propriétés des éléments - là où se trouvent les noms de fichiers, les URL et les noms d'appareils - en sont exclues, et le graphe est expurgé de la même façon que les messages.
- La version du SDK, le runtime .NET, la description du système d'exploitation, l'architecture du processus, le framework cible, le nom du moteur et un identifiant aléatoire régénéré à chaque démarrage du processus.
- Au démarrage d'un moteur ou d'un pipeline, ou lors d'une tentative de démarrage, les noms des fonctionnalités du SDK qu'il utilise : le moteur (`VideoCaptureCoreX`, `MediaBlocksPipeline`, ...), les noms de types SDK de ses blocs, de son format de sortie et de son mode (`MP4OutputBlock`, `VideoCaptureMode.VideoCapture`, ...). Les deux conditions sont vérifiées au démarrage du moteur. Chaque nom est envoyé une seule fois par processus et par moteur (de nouveau au démarrage suivant si l'envoi a échoué), avec l'indication du type de build du SDK lui-même (debug ou release). Seuls les types déclarés par le SDK sont nommés : une classe à vous, comme un bloc personnalisé, n'est jamais signalée.

## Ce qui n'est jamais envoyé

- Vos paramètres : hormis le mode du moteur ainsi que les formats média et les valeurs de pads du graphe de pipeline mentionnés ci-dessus, aucune valeur de propriété d'un bloc, d'une source, d'une sortie ou d'un moteur n'est envoyée. La liste de fonctionnalités ne nomme que des types du SDK ; elle ne décrit pas la façon dont vous les avez connectés ou configurés.
- Les noms ou chemins de fichiers.
- Les identifiants d'appareil : chemins d'instance PnP, moniker DirectShow et GUID.
- L'identité de l'utilisateur, sous quelque forme que ce soit.
- Les variables d'environnement.
- Le contenu multimédia - ni images, ni échantillons, ni captures.
- Aucun identifiant survivant à un redémarrage. L'identifiant aléatoire sert uniquement à regrouper les événements d'une session de débogage, et il disparaît à la fin du processus.

Une chose que cette liste ne promet pas : un message d'erreur nomme souvent l'élément ou le codec concerné - « Failed to link h264parse to qtmux » en est un exemple typique -, si bien que des noms d'éléments et de codecs apparaissent bel et bien dans les messages signalés. Ce qui n'est jamais envoyé, c'est une description de votre configuration au-delà du graphe d'un pipeline en échec ; ce qui est envoyé, c'est le texte de l'erreur elle-même.

La charge utile ne contient aucun objet utilisateur ni aucune adresse IP. La connexion elle-même révèle nécessairement votre adresse au serveur, qui est configuré pour l'écarter plutôt que de la conserver avec l'événement.

## Ce qui est signalé

Le niveau de journalisation du SDK détermine ce qui peut être signalé :

- Avec `Debug_Mode` **désactivé**, le SDK ne journalise que les erreurs : elles sont donc la seule chose qui puisse être envoyée, et les fils d'Ariane ne contiennent que les avertissements et erreurs propres à GStreamer.
- Avec `Debug_Mode` **activé**, le SDK journalise à partir de `Debug`. Les avertissements et les erreurs sont signalés ; les lignes de débogage et d'information ne sont conservées que comme fils d'Ariane attachés à l'erreur suivante.
- Dès qu'un moteur ou un pipeline dont `Debug_Telemetry` est activé démarre sous un débogueur, le niveau de journalisation par défaut de GStreamer passe des erreurs aux avertissements pour le reste du processus, sauf si vous avez donné à `VisioForgeX.DebugLevel` une valeur autre que sa valeur par défaut. Le SDK ne transmet ces avertissements qu'aux fils d'Ariane, pas à votre sortie de débogage ; la sortie console propre à GStreamer (stderr) les affiche toutefois.

## Où cela va

Les rapports sont envoyés en HTTPS à `https://telemetry.visioforge.org`, un serveur exploité par VisioForge. Les avertissements sont mis en file d'attente et délivrés par un thread d'arrière-plan : en journaliser un ne coûte donc rien au thread appelant. Une erreur, c'est différent : le thread qui l'a journalisée attend jusqu'à 300 millisecondes la fin de l'envoi, car « Arrêter le débogage » tue le processus et la dernière erreur est celle qui compte. Passé ce délai, le thread poursuit son exécution et la requête se termine en arrière-plan. Si le réseau est indisponible, ou si le plafond de fréquence d'envoi est atteint, le rapport est abandonné plutôt que réessayé indéfiniment.

Les noms de fonctionnalités sont envoyés en HTTPS à `https://analytics.visioforge.org`, un serveur d'analyse distinct exploité par VisioForge, depuis une tâche d'arrière-plan qui ne retarde jamais l'appelant. Un envoi qui échoue n'est pas réessayé en arrière-plan ; ses noms sont renvoyés au prochain démarrage du moteur. Ce serveur ne conserve aucune adresse IP : il dérive de la connexion un hachage quotidien anonyme, et le sel de ce hachage est supprimé au bout de deux jours.

La première fois qu'un processus envoie quelque chose, le SDK écrit une ligne indiquant ce qui est envoyé, où, et comment le désactiver, à la fois dans le journal et dans la sortie de débogage.

## Voir aussi

- [Envoi des journaux](sendlogs.md) - collecter le journal de débogage complet pour le support
- [Politique de confidentialité](https://www.visioforge.com/privacy-policy) - la politique d'entreprise dont relève cette fonctionnalité
