---
title: Désinstaller des filtres DirectShow avec regsvr32 en .NET
description: Désinstallez correctement les filtres DirectShow avec des techniques manuelles, étapes de dépannage et bonnes pratiques pour les applications multimédias .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - Playback

---

# Supprimer des filtres DirectShow sous Windows

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Les filtres DirectShow sont des composants essentiels pour les applications multimédias dans les environnements Windows. Ils permettent aux logiciels de traiter efficacement les données audio et vidéo. Cependant, il peut y avoir des situations où vous devez désinstaller ces filtres, par exemple lors de la mise à niveau de votre application, de la résolution de conflits ou de la suppression complète d'un paquet logiciel. Ce guide fournit des instructions détaillées sur la façon de désinstaller correctement les filtres DirectShow de votre système.

## Comprendre les filtres DirectShow

DirectShow est un framework multimédia et une API conçus par Microsoft pour permettre aux développeurs de logiciels d'effectuer diverses opérations sur les fichiers multimédias. Il est construit sur l'architecture Component Object Model (COM) et utilise une approche modulaire où chaque étape de traitement est gérée par un composant distinct appelé filtre.

Les filtres sont classés en trois types principaux :

- **Filtres source** : lisent les données depuis des fichiers, des périphériques de capture ou des flux réseau
- **Filtres de transformation** : traitent ou modifient les données (compression, décompression, effets)
- **Filtres de rendu** : affichent la vidéo ou lisent l'audio

Lorsque les composants SDK sont installés, ils enregistrent les filtres DirectShow dans le Registre Windows, ce qui les rend disponibles pour toute application utilisant le framework DirectShow.

## Pourquoi désinstaller des filtres DirectShow ?

Plusieurs raisons peuvent vous amener à désinstaller des filtres DirectShow :

1. **Conflits de version** : les versions plus récentes du SDK peuvent nécessiter la suppression des anciens filtres
2. **Nettoyage du système** : supprimer les composants inutilisés pour maintenir l'efficacité du système
3. **Dépannage** : résoudre les problèmes des applications multimédias
4. **Suppression complète du logiciel** : s'assurer qu'aucun composant ne reste après la désinstallation de l'application principale
5. **Réinscription** : parfois, désinstaller et réinstaller les filtres peut résoudre les problèmes d'enregistrement

## Méthodes pour désinstaller les filtres DirectShow

### Méthode 1 : utiliser le programme d'installation du SDK (recommandée)

La façon la plus simple de désinstaller les filtres DirectShow est via le programme d'installation du SDK (ou redist) lui-même. Les paquets SDK incluent des routines de désinstallation qui suppriment correctement tous les composants, y compris les filtres DirectShow.

### Méthode 2 : désinscription manuelle avec regsvr32

Si la désinstallation automatique n'est pas possible ou si vous devez désinscrire des filtres spécifiques, vous pouvez utiliser l'outil en ligne de commande `regsvr32` :

1. Ouvrez l'invite de commandes en tant qu'administrateur (clic droit sur l'invite de commandes et sélectionnez « Exécuter en tant qu'administrateur »)
2. Utilisez la syntaxe de commande suivante pour désinscrire un filtre :

   ```cmd
   regsvr32 /u "C:\path\to\filter.dll"
   ```

3. Remplacez `C:\path\to\filter.dll` par le chemin réel vers le fichier de filtre DirectShow
4. Appuyez sur Entrée pour exécuter la commande

Par exemple, pour désinscrire un filtre situé dans `C:\Program Files\Common Files\FilterFolder\example_filter.dll`, vous utiliseriez :

```cmd
regsvr32 /u "C:\Program Files\Common Files\FilterFolder\example_filter.dll"
```

Vous devriez voir une boîte de dialogue de confirmation indiquant une désinscription réussie.

## Trouver l'emplacement des filtres DirectShow

Avant de pouvoir désinscrire manuellement les filtres, vous devez connaître leur emplacement. Voici plusieurs méthodes pour trouver les filtres DirectShow installés :

### Avec GraphStudio

[GraphStudio](https://github.com/cplussharp/graph-studio-next) est un outil open source puissant pour travailler avec les filtres DirectShow. Pour trouver l'emplacement des filtres :

1. Téléchargez et installez GraphStudio
2. Lancez l'application avec les privilèges administrateur
3. Allez dans « Graph > Insert Filters »
4. Parcourez la liste des filtres installés
5. Faites un clic droit sur un filtre et sélectionnez « Properties »
6. Notez le chemin « File: » affiché dans la boîte de dialogue des propriétés

Cette méthode fournit le chemin de fichier exact nécessaire à la désinscription manuelle.

### Avec le Registre système

Vous pouvez également trouver les filtres DirectShow via le Registre Windows :

1. Appuyez sur `Win + R` pour ouvrir la boîte de dialogue Exécuter
2. Tapez `regedit` et appuyez sur Entrée pour ouvrir l'Éditeur du Registre
3. Naviguez vers `HKEY_CLASSES_ROOT\CLSID`
4. Utilisez la fonction de recherche (Ctrl+F) pour trouver les noms de filtres
5. Recherchez la clé « InprocServer32 » sous le CLSID du filtre, qui contient le chemin du fichier

## Considérations de plateforme (x86 vs x64)

Les filtres DirectShow sont spécifiques à la plateforme, ce qui signifie que les versions 32 bits (x86) et 64 bits (x64) sont des composants distincts. Si vous avez installé les deux versions, vous devez désinscrire chacune séparément.

Pour les systèmes x64 :

- Les filtres 64 bits sont généralement installés dans `C:\Windows\System32`
- Les filtres 32 bits sont généralement installés dans `C:\Windows\SysWOW64`

Utilisez la version appropriée de `regsvr32` pour chaque plateforme :

- Pour les filtres 64 bits : `C:\Windows\System32\regsvr32.exe`
- Pour les filtres 32 bits : `C:\Windows\SysWOW64\regsvr32.exe`

## Dépannage de la désinstallation des filtres

Si vous rencontrez des problèmes lors de la désinstallation des filtres, essayez ces étapes de dépannage :

### Impossible de désinscrire le filtre

Si vous recevez une erreur comme « DllUnregisterServer failed with error code 0x80004005 » :

1. Assurez-vous d'exécuter l'invite de commandes en tant qu'administrateur
2. Vérifiez que le chemin vers le filtre est correct
3. Vérifiez que le fichier de filtre existe et n'est pas utilisé par une application
4. Fermez toutes les applications qui pourraient utiliser des filtres DirectShow
5. Dans certains cas, un redémarrage du système peut être nécessaire avant la désinscription

### Le filtre est toujours présent après la désinscription

Si un filtre semble toujours enregistré après une tentative de désinscription :

1. Utilisez GraphStudio pour vérifier si le filtre figure toujours dans la liste des filtres disponibles
2. Recherchez plusieurs instances du filtre à différents emplacements
3. Vérifiez les emplacements du registre 32 bits et 64 bits
4. Essayez d'utiliser l'outil « OleView » fourni par Microsoft pour inspecter les enregistrements COM

## Vérifier la réussite de la désinstallation

Après avoir désinstallé les filtres DirectShow, vérifiez que la suppression a réussi :

1. Utilisez GraphStudio pour vérifier que les filtres n'apparaissent plus dans la liste des filtres disponibles
2. Vérifiez le registre pour toute entrée restante liée aux filtres
3. Testez toutes les applications qui utilisaient précédemment les filtres pour vous assurer qu'elles gèrent gracieusement leur absence

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code et d'implémentation pour travailler avec DirectShow et les applications multimédias en .NET.
