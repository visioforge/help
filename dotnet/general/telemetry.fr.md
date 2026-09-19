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

Chaque moteur expose une propriété `Debug_Telemetry`. Lorsqu'elle vaut `true` **et qu'un débogueur est attaché à votre processus**, le SDK signale ses propres erreurs à VisioForge, afin que les défaillances que vous rencontrez en intégrant le SDK nous parviennent sans que personne ait à ouvrir un ticket.

Les deux conditions sont requises, et toutes deux sont vérifiées chaque fois qu'un événement serait envoyé :

- `Debug_Telemetry` vaut `true` (la valeur par défaut).
- `System.Diagnostics.Debugger.IsAttached` vaut `true`.

Une application que vous compilez et distribuez à vos utilisateurs s'exécute sans débogueur : **une application publiée n'envoie donc rien**. Détacher le débogueur arrête le signalement immédiatement ; en attacher un en cours de session le démarre.

## Comment la désactiver

Définissez la propriété sur `false` avant de démarrer le moteur :

```csharp
// N'importe lequel des moteurs - VideoCaptureCoreX, VideoCaptureCore, MediaPlayerCoreX,
// MediaPlayerCore, VideoEditCoreX, VideoEditCore, SimplePlayerX.
core.Debug_Telemetry = false;
```

```csharp
var pipeline = new MediaBlocksPipeline();
pipeline.Debug_Telemetry = false;
```

La propriété peut être définie à tout moment ; ce qui compte, c'est son état à l'instant où l'erreur survient.

## Ce qui est envoyé

- Le nom de la classe et de la méthode ainsi que le niveau de l'événement de journal.
- Le message de journal, débarrassé des identifiants, chemins de fichiers, noms d'hôtes, adresses IP et MAC, identifiants d'appareil, GUID et adresses e-mail.
- Le type d'exception, son message expurgé et les trames de pile. Une trame conserve le nom de la méthode et le numéro de ligne ; le chemin du fichier source est supprimé, car il reflète l'arborescence de votre machine de compilation.
- Jusqu'à 50 lignes de journal précédentes en fil d'Ariane, expurgées de la même façon.
- La version du SDK, le runtime .NET, la description du système d'exploitation, l'architecture du processus, le framework cible, le nom du moteur et un identifiant aléatoire régénéré à chaque démarrage du processus.

## Ce qui n'est jamais envoyé

- Votre configuration de pipeline ou de paramètres - rien n'énumère les blocs que vous avez construits ni les paramètres que vous leur avez donnés.
- Les noms ou chemins de fichiers.
- Les identifiants d'appareil : chemins d'instance PnP, moniker DirectShow et GUID.
- L'identité de l'utilisateur, sous quelque forme que ce soit.
- Les variables d'environnement.
- Le contenu multimédia - ni images, ni échantillons, ni captures.
- Aucun identifiant survivant à un redémarrage. L'identifiant aléatoire sert uniquement à regrouper les événements d'une session de débogage, et il disparaît à la fin du processus.

Une chose que cette liste ne promet pas : un message d'erreur nomme souvent l'élément ou le codec concerné - « Failed to link h264parse to qtmux » en est un exemple typique -, si bien que des noms d'éléments et de codecs apparaissent bel et bien dans les messages signalés. Ce qui n'est jamais envoyé, c'est une description de votre configuration ; ce qui est envoyé, c'est le texte de l'erreur elle-même.

La charge utile ne contient aucun objet utilisateur ni aucune adresse IP. La connexion elle-même révèle nécessairement votre adresse au serveur, qui est configuré pour l'écarter plutôt que de la conserver avec l'événement.

## Ce qui est signalé

Le niveau de journalisation du SDK détermine ce qui peut être signalé :

- Avec `Debug_Mode` **désactivé**, le SDK ne journalise que les erreurs : elles sont donc la seule chose qui puisse être envoyée, et la liste de fils d'Ariane est vide.
- Avec `Debug_Mode` **activé**, le SDK journalise à partir de `Debug`. Les avertissements et les erreurs sont signalés ; les lignes de débogage et d'information ne sont conservées que comme fils d'Ariane attachés à l'erreur suivante.

## Où cela va

Les rapports sont envoyés en HTTPS à `https://telemetry.visioforge.org`, un serveur exploité par VisioForge. Les avertissements sont mis en file d'attente et délivrés par un thread d'arrière-plan : en journaliser un ne coûte donc rien au thread appelant. Une erreur, c'est différent : le thread qui l'a journalisée attend jusqu'à 300 millisecondes la fin de l'envoi, car « Arrêter le débogage » tue le processus et la dernière erreur est celle qui compte. Passé ce délai, le thread poursuit son exécution et la requête se termine en arrière-plan. Si le réseau est indisponible, ou si le plafond de fréquence d'envoi est atteint, le rapport est abandonné plutôt que réessayé indéfiniment.

La première fois qu'un processus envoie quelque chose, le SDK écrit une ligne indiquant ce qui est envoyé, où, et comment le désactiver, à la fois dans le journal et dans la sortie de débogage.

## Voir aussi

- [Envoi des journaux](sendlogs.md) - collecter le journal de débogage complet pour le support
- [Politique de confidentialité](https://www.visioforge.com/privacy-policy) - la politique d'entreprise dont relève cette fonctionnalité
