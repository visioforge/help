---
title: Journal des modifications TVFMediaPlayer Delphi — v3 à v10
description: Journal des modifications TVFMediaPlayer — historique des versions 3.0 à 10.0 avec prise en charge 4K, chiffrement, effets, streaming et performances.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Playback
  - Streaming
primary_api_classes:
  - TVFMediaPlayer

---

# Journal des modifications de la bibliothèque TVFMediaPlayer

Ce document détaille l'évolution de la bibliothèque TVFMediaPlayer, en retraçant les fonctionnalités majeures, améliorations, optimisations et corrections de bogues introduites au fil des différentes versions. Il sert de référence complète pour les développeurs qui suivent les progrès de la bibliothèque et souhaitent comprendre les capacités ajoutées au fil du temps.

## Version 10.0 : gestion multimédia améliorée et personnalisation

La version 10.0 représente une avancée significative, axée sur l'amélioration de l'introspection multimédia, la journalisation, la personnalisation et la compatibilité.

### Améliorations principales

* **Lecteur d'informations multimédias amélioré :** cette version renforce significativement les capacités du lecteur d'informations multimédias. Il permet une extraction plus rapide et précise des métadonnées à partir d'une vaste gamme de types de fichiers multimédias. Les développeurs obtiennent un accès fiable à des détails critiques comme la durée, la résolution, les spécifications de codec, les débits binaires et les étiquettes intégrées, ce qui simplifie la gestion des médias et améliore les capacités d'affichage dans les applications.
* **Capacités de journalisation améliorées :** la journalisation a été considérablement affinée, offrant aux développeurs un contrôle plus granulaire. Les options de configuration incluent désormais des niveaux de journal distincts (Debug, Info, Warning, Error) et des destinations de sortie flexibles comme les fichiers, la console ou des points de terminaison personnalisés. Cela facilite un diagnostic plus efficace des problèmes pendant le développement et une surveillance robuste du comportement de l'application en production, conduisant à un dépannage plus rapide et à une stabilité accrue.
* **Prise en charge des étiquettes de métadonnées standard :** une pierre angulaire de cette version est l'introduction d'une prise en charge complète de la lecture d'étiquettes de métadonnées standard intégrées dans les conteneurs vidéo et audio populaires. Cela inclut des formats comme MP4, WMV, MP3, AAC, M4A et Ogg Vorbis. Les applications utilisant TVFMediaPlayer peuvent désormais extraire et exploiter sans difficulté des étiquettes courantes comme le titre, l'artiste, l'album, le genre, l'année et la pochette, enrichissant ainsi l'expérience utilisateur en fournissant un contexte précieux pour le média en cours de lecture.

### Améliorations de la capture et des effets

* **Noms de fichier d'auto-découpage configurables :** la nouvelle propriété `SeparateCapture_Filename_Mask` offre un contrôle précis sur les noms de fichiers lors de l'utilisation de la fonction d'auto-découpage de capture basée sur la durée ou la taille. Cela permet des conventions de nommage personnalisées, améliorant l'organisation et le flux de travail pour les enregistrements segmentés.
* **Sérialisation JSON des paramètres :** les paramètres de configuration du lecteur multimédia peuvent désormais être facilement sérialisés et désérialisés depuis le format JSON, largement utilisé. Cela simplifie la sauvegarde et le chargement des configurations du lecteur, permettant des paramètres persistants et une intégration plus facile avec les systèmes de gestion de configuration.
* **Pipeline d'effets vidéo personnalisés :** la flexibilité du traitement vidéo est améliorée grâce à la possibilité d'insérer des effets vidéo personnalisés à l'aide de filtres tiers identifiés par leur CLSID. Ces filtres peuvent être placés stratégiquement avant ou après le filtre d'effets principal ou le sample grabber, permettant des pipelines de manipulation vidéo sophistiqués et adaptés.
* **Effets vidéo optimisés :** le traitement des effets vidéo a été optimisé pour tirer pleinement parti des dernières générations de processeurs Intel, ce qui se traduit par une lecture plus fluide et une consommation de ressources réduite lors de l'application d'effets.

### Corrections de sources et de compatibilité

* **Splitter MP3 pour les problèmes de lecture :** un splitter MP3 a été intégré pour traiter et résoudre spécifiquement les incohérences de lecture rencontrées avec certains fichiers MP3 non standard ou problématiques, garantissant une compatibilité plus large.
* **VLC Source Filter mis à jour :** le VLC Source Filter sous-jacent a été mis à jour vers libVLC version 2.2.2.0. Cette mise à jour apporte des améliorations notables, en particulier dans la gestion des flux RTMP et HTTPS, et résout des fuites de mémoire précédemment identifiées, contribuant à une stabilité accrue et à une prise en charge plus large des protocoles de streaming.
* **Corrections des effets Pan et Blur :** des problèmes spécifiques liés à l'effet Pan dans les builds x64 et à l'effet Blur ont été traités et résolus, garantissant un comportement cohérent des effets visuels sur les différentes architectures.
* **Fuite de mémoire FFMPEG résolue :** une fuite de mémoire associée au composant source FFMPEG a été identifiée et corrigée, améliorant la stabilité à long terme et la gestion des ressources pendant la lecture.

## Version 9.2 : mises à jour des moteurs et améliorations du lecteur

Cette version intermédiaire s'est concentrée sur la mise à jour des composants centraux et l'affinement supplémentaire des capacités d'informations multimédias.

* **Moteur VLC mis à jour :** le moteur VLC intégré a été mis à jour vers libVLC version 2.2.1.0, intégrant les corrections et améliorations en amont du projet VLC pour une meilleure stabilité et compatibilité des formats.
* **Lecteur d'informations multimédias amélioré :** en s'appuyant sur les améliorations précédentes, le lecteur d'informations multimédias a reçu des améliorations supplémentaires pour une prise en charge des fichiers plus large et une extraction de métadonnées plus précise.
* **Moteur FFMPEG mis à jour :** les composants du moteur FFMPEG ont été mis à jour, garantissant la compatibilité avec les codecs et formats plus récents tout en intégrant des optimisations de performance.

## Version 9.1 : intégration de sécurité avancée

La version 9.1 a introduit des fonctionnalités de sécurité robustes grâce à l'intégration avec le Video Encryption SDK.

* **Prise en charge de Video Encryption SDK v9 :** cette version a ajouté la compatibilité avec le Video Encryption SDK v9. Cela permet aux développeurs d'implémenter un chiffrement AES-256 fort pour leur contenu vidéo, en utilisant soit des fichiers de clés séparés, soit des données binaires intégrées comme clés, améliorant considérablement les capacités de protection du contenu.

## Version 9.0 : améliorations audio et flexibilité des logos

La version 9.0 a apporté des améliorations significatives à la gestion audio et aux options d'image de marque visuelle.

* **Prise en charge des logos GIF animés :** la capacité d'utiliser des logos image a été étendue pour inclure la prise en charge des GIF animés, permettant une image de marque visuelle plus dynamique et engageante dans l'interface de lecture vidéo.
* **Améliorations audio :** un ensemble de fonctionnalités d'amélioration audio a été introduit, incluant la normalisation audio pour garantir des niveaux de volume cohérents, le contrôle automatique de gain (AGC) pour ajuster dynamiquement le volume, et des contrôles de gain manuel pour des ajustements précis des niveaux audio.
* **Volume audio basé sur un pourcentage :** l'API de contrôle du volume audio a été modernisée pour utiliser un système basé sur un pourcentage (0-100 %), offrant une manière plus intuitive et standardisée de gérer les niveaux audio par rapport aux méthodes précédentes.

## Version 8.6 : extension des décodeurs et ajouts à l'API

Cette version s'est concentrée sur l'extension de la prise en charge des codecs, l'ajout de flexibilité grâce à des filtres personnalisés et l'affinement de l'API.

* **Décodeur H264 CPU/Intel QuickSync :** un décodeur vidéo H264 hautement optimisé a été ajouté, exploitant à la fois les ressources CPU et l'accélération matérielle Intel QuickSync lorsqu'elle est disponible. Cela améliore significativement les performances de décodage de l'un des codecs vidéo les plus courants.
* **Prise en charge de filtres vidéo DirectShow personnalisés :** les développeurs ont gagné la possibilité d'intégrer leurs propres filtres vidéo DirectShow personnalisés dans le graphe de lecture, permettant des tâches de traitement vidéo très spécialisées.
* **Événement `OnNewFilePlaybackStarted` :** un nouvel événement, `OnNewFilePlaybackStarted`, a été introduit. Cet événement se déclenche spécifiquement quand un nouveau fichier commence à être lu dans le contexte d'une liste de lecture, permettant aux applications de réagir précisément aux transitions entre éléments multimédias.
* **Décodeurs mis à jour :** le décodeur audio Ogg Vorbis et les décodeurs vidéo WebM ont été mis à jour vers leurs dernières versions, garantissant la compatibilité et des améliorations de performances.
* **Mise à jour de l'API Frame Grabber :** l'API de capture d'images vidéo individuelles a été mise à jour, offrant potentiellement de meilleures performances ou plus de flexibilité.
* **Corrections de bogues :** diverses corrections de bogues non spécifiées ont été implémentées pour améliorer la stabilité et la fiabilité globales.

## Version 8.5 : rotation, prise en charge 4K et options de rendu

La version 8.5 a introduit des fonctionnalités innovantes de manipulation vidéo et préparé le moteur pour le contenu en ultra-haute définition.

* **Rotation vidéo à la volée :** un nouvel effet vidéo a été ajouté, permettant la rotation en temps réel du flux vidéo pendant la lecture (par exemple, 90, 180, 270 degrés).
* **Source FFMPEG mise à jour :** le composant source FFMPEG a été mis à jour, intégrant probablement la prise en charge de formats plus récents ou améliorant les performances.
* **Effets vidéo prêts pour le 4K :** les effets vidéo existants ont été optimisés et testés pour garantir leur efficacité avec du contenu vidéo 4K.
* **Correction du bogue de décalage du zoom VMR-9/EVR :** un bogue spécifique lié au décalage inattendu d'image lors de l'utilisation du zoom avec les moteurs de rendu vidéo VMR-9 ou EVR a été corrigé.
* **Moteur de rendu vidéo Direct2D (Beta) :** un nouveau moteur de rendu vidéo basé sur Direct2D a été introduit en tant que fonctionnalité bêta. Ce moteur de rendu prenait en charge la rotation vidéo en direct et visait à tirer parti des API graphiques modernes pour des performances et une qualité potentiellement améliorées.
* **Corrections de bogues :** incluait diverses corrections de bogues générales pour améliorer la stabilité.

## Version 8.4 : mises à jour des décodeurs et stabilité

Il s'agissait principalement d'une version de maintenance axée sur la mise à jour des composants centraux.

* **Décodeur FFMPEG mis à jour :** les composants du décodeur FFMPEG ont été mis à jour, intégrant probablement des corrections et améliorations du projet FFMPEG.
* **Corrections de bogues :** traitement de divers bogues non spécifiés pour une stabilité améliorée.

## Version 8.3 : version de stabilité

Cette version s'est concentrée uniquement sur la correction de bogues identifiés dans les versions précédentes.

* **Corrections de bogues :** mise en œuvre de diverses corrections pour améliorer la fiabilité et la stabilité globales de la bibliothèque.

## Version 8.0 : introduction du moteur VLC

La version 8.0 a marqué un ajout architectural significatif en intégrant le puissant moteur VLC.

* **Intégration du moteur VLC :** le célèbre moteur VLC a été intégré comme back-end de lecture alternatif pour les fichiers vidéo et audio. Cela a apporté la prise en charge étendue des formats de VLC et ses robustes capacités de streaming aux applications TVFMediaPlayer.
* **Corrections de bogues :** incluait diverses corrections de bogues générales.

## Série 7.x : effets, chiffrement et listes de lecture

La série 7 a introduit des fonctionnalités clés liées au contrôle de la lecture, à la sécurité et aux effets visuels.

### Version 7.20

* **Lecture inversée :** ajout de la possibilité de lire les fichiers vidéo en sens inverse, ouvrant des possibilités créatives et des cas d'usage spécialisés.
* **Corrections de bogues :** traitement de divers bogues.

### Version 7.12

* **Prise en charge du chiffrement vidéo :** ajout d'une prise en charge initiale du chiffrement vidéo, fournissant des mécanismes de base de protection du contenu.
* **Corrections de bogues :** améliorations générales de stabilité.

### Version 7.7

* **Effet fondu d'entrée/fondu de sortie :** un effet de transition vidéo courant et utile, fade-in/fade-out, a été ajouté aux effets vidéo disponibles.
* **Prise en charge des listes de lecture :** introduction de fonctionnalités pour créer et gérer des listes de lecture, permettant la lecture automatique de séquences de fichiers multimédias.
* **Corrections de bogues :** traitement de divers problèmes.

### Version 7.5

* **Incrustation par chrominance améliorée :** l'effet d'incrustation par chrominance (fond vert) a été amélioré pour une meilleure qualité et un contrôle plus précis.
* **Logo de texte amélioré :** la fonctionnalité de superposition de logos textuels sur la vidéo a été améliorée.
* **API d'effets vidéo modifiée :** l'API d'application des effets vidéo a subi des modifications, pour une utilisabilité améliorée ou pour intégrer de nouvelles fonctionnalités.
* **Corrections de bogues :** incluait diverses corrections de stabilité.

### Version 7.0

* **Prise en charge de Windows 8 RTM :** compatibilité assurée avec la version finale de Windows 8.
* **Effets vidéo améliorés :** d'autres améliorations ont été apportées à la qualité et aux performances des effets vidéo existants.
* **Nouveau moteur de lecture FFMPEG :** introduction d'un nouveau moteur de lecture basé sur les composants FFMPEG, offrant une alternative à la lecture par défaut basée sur DirectShow et étendant la compatibilité des formats.

## Série 6.x : compatibilité avec Windows 8 et optimisations

La série 6 s'est concentrée sur l'adaptation au système d'exploitation Windows 8, alors nouveau, et sur l'amélioration des performances.

### Version 6.3

* **Prise en charge de Windows 8 Customer Preview :** ajout de la compatibilité avec la version Customer Preview de Windows 8.
* **Effets vidéo améliorés :** poursuite de l'affinement des performances et de la qualité des effets vidéo.

### Version 6.0

* **Prise en charge OpenCL améliorée :** meilleure utilisation d'OpenCL pour les tâches d'accélération GPU, augmentant potentiellement les performances pour les effets ou le décodage sur le matériel compatible.
* **Prise en charge de Windows 8 Developer Preview :** ajout d'une prise en charge précoce de la version Developer Preview de Windows 8.
* **Effets vidéo améliorés :** améliorations générales du sous-système d'effets vidéo.

## Série 3.x : premières fonctionnalités et optimisations

La série 3 a posé les bases de fonctionnalités et s'est concentrée sur des optimisations spécifiques au CPU.

### Version 3.9

* **Nouveaux programmes d'installation :** introduction d'un nouveau programme d'installation principal et de programmes d'installation redistribuables distincts pour un déploiement plus facile.
* **Corrections mineures de bogues :** traitement de problèmes mineurs en attente.

### Version 3.7

* **Effets vidéo améliorés :** améliorations apportées aux fonctionnalités des effets vidéo.
* **Nouvelles applications de démonstration :** ajout de nouvelles applications de démonstration pour présenter les capacités de la bibliothèque.
* **Optimisations CPU pour netbooks :** intégration d'optimisations de performance spécifiques adaptées aux processeurs Intel Core II/Atom et AMD pour netbooks.
* **Corrections mineures de bogues :** améliorations générales de stabilité.

### Version 3.5

* **Effets vidéo améliorés :** poursuite du travail sur l'amélioration des effets vidéo.
* **Optimisations pour Intel Core i7 :** ajout de nouvelles optimisations de performance spécifiques à l'architecture CPU Intel Core i7, alors nouvelle.

### Version 3.0

* **Détection de mouvement :** introduction d'une fonctionnalité de détection de mouvement, permettant aux applications de réagir aux changements dans le flux vidéo.
* **Incrustation par chrominance :** ajout d'une fonctionnalité initiale d'incrustation par chrominance (fond vert).
* **Prise en charge des sources MMS/WMV :** intégration de la prise en charge du streaming via le protocole MMS et de la lecture des fichiers WMV (Windows Media Video).
* **Optimisations CPU :** ajout d'optimisations de performance ciblant les processeurs Intel Atom et Core i3/i5/i7.
* **Traitement direct des flux :** activation de la capacité d'accéder directement aux données décodées de flux vidéo et audio et de les traiter, offrant des possibilités avancées de manipulation.
