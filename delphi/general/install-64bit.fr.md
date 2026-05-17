---
title: Installer le SDK VisioForge en Delphi 64 bits — paquets BPL
description: Résolvez les problèmes de chargement de BPL 64 bits dans l'IDE Delphi. Paquets design-time vs runtime, configuration des chemins x86/x64 et erreurs courantes.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL

---

# Maîtriser l'installation des paquets Delphi 64 bits

## Introduction au développement 64 bits dans Delphi

L'évolution vers l'informatique 64 bits représente une avancée majeure pour les développeurs Delphi : performances accrues, capacités d'adressage mémoire étendues et meilleure utilisation des ressources. Depuis l'introduction de la prise en charge 64 bits dans Delphi XE2, les développeurs peuvent compiler des applications Windows natives 64 bits. Cette capacité permet aux logiciels de tirer parti des architectures matérielles modernes, d'accéder à des espaces mémoire bien plus grands et d'offrir des performances optimisées pour les opérations gourmandes en données.

Cependant, cette progression technologique introduit son lot de complexités, notamment en ce qui concerne l'installation et la gestion des paquets de composants (fichiers `.bpl`). De nombreux développeurs Delphi rencontrent des obstacles déroutants lorsqu'ils tentent d'intégrer des paquets 64 bits dans leur flux de travail, ce qui peut entraîner frustration et perte de productivité.

Ce guide approfondi explore ces défis en détail et fournit des solutions méticuleusement détaillées et exploitables. Le problème fondamental provient d'une caractéristique architecturale critique : **l'environnement de développement intégré (IDE) de Delphi reste une application 32 bits**, même dans les versions les plus récentes. Cet écart architectural entre l'IDE 32 bits et la cible de compilation 64 bits engendre de nombreux malentendus et difficultés techniques liés à la gestion des paquets.

Comprendre cette limitation architecturale constitue la première étape essentielle pour établir une expérience de développement fluide. Nous examinerons en détail pourquoi l'IDE 32 bits a besoin de paquets de conception (design-time) 32 bits, explorerons les techniques de configuration de projet appropriées pour les cibles 32 bits et 64 bits, clarifierons le rôle critique des paquets d'exécution (runtime), et décrirons des méthodologies de test exhaustives pour garantir que vos applications fonctionnent parfaitement dans les deux environnements architecturaux.

## La limitation architecturale : pourquoi l'IDE 32 bits a besoin de paquets de conception 32 bits

### Comprendre l'architecture de l'IDE

L'IDE Delphi sert d'environnement principal pour la conception visuelle de composants, l'édition de code, les opérations de débogage et la gestion globale des projets. Lorsque les concepteurs placent des composants sur des formulaires à l'aide du concepteur de formulaires, modifient des propriétés via l'inspecteur d'objets ou utilisent des éditeurs de composants spécialisés, l'IDE doit charger et exécuter le code contenu dans le paquet de conception du composant.

Comme `bds.exe` (l'exécutable de l'IDE Delphi) s'exécute en tant que processus 32 bits, il fonctionne exclusivement dans l'espace d'adressage mémoire 32 bits et doit respecter les contraintes des environnements d'exécution 32 bits. L'IDE ne peut physiquement pas charger ni exécuter directement du code 64 bits — il s'agit d'une limitation matérielle et système, pas simplement d'une restriction logicielle. Toute tentative de chargement d'une DLL 64 bits (ou en terminologie Delphi, d'un paquet `.bpl` 64 bits) dans un processus 32 bits échouera immédiatement, se manifestant typiquement par des messages d'erreur comme « Can't load package %s » ou des codes d'erreur obscurs du système d'exploitation.

### Exigences critiques à la conception

Pour que l'IDE fonctionne correctement lors des activités de conception — permettant la manipulation visuelle des composants, la configuration des propriétés et l'utilisation des fonctionnalités de design-time —, il *doit* charger la version **32 bits (x86)** des paquets de composants. Cette exigence n'est pas négociable, en raison de l'architecture fondamentale de l'IDE et des principes de gestion mémoire du système d'exploitation.

Cette limitation architecturale crée fréquemment de la confusion chez les développeurs, donnant lieu à la fausse idée que seuls les paquets 32 bits sont nécessaires, ou à la question de savoir pourquoi des paquets 64 bits distincts existent si l'IDE ne peut pas les utiliser. La distinction critique réside dans la séparation entre les opérations de **conception** (qui se produisent dans l'IDE 32 bits) et les processus de **compilation/exécution** (où les applications peuvent cibler les architectures 32 bits ou 64 bits).

## Mise en œuvre étape par étape : installation des paquets de conception 32 bits

### Première étape essentielle : installer les composants 32 bits

D'après l'explication architecturale ci-dessus, l'étape initiale obligatoire consiste toujours à installer la version 32 bits des paquets de composants dans l'IDE Delphi. Ce processus pose les bases de toutes les activités de développement ultérieures.

1. **Récupérez les fichiers de paquet nécessaires :** assurez-vous de disposer des fichiers de paquet compilés à la fois en 32 bits et en 64 bits (`.bpl` et `.dcp`). Les fichiers 32 bits portent généralement des suffixes d'identification comme `_x86`, `_Win32`, ou peuvent ne porter aucun identifiant de plateforme dans les versions plus anciennes de Delphi. À l'inverse, les paquets 64 bits incluent normalement les désignations `_x64` ou `_Win64`. Ces fichiers sont généralement générés automatiquement lors de la construction de projets de bibliothèque de composants ciblant à la fois les plateformes Win32 et Win64. Avec des composants tiers, les éditeurs réputés fournissent les deux versions architecturales.

2. **Lancez l'environnement de développement :** démarrez l'IDE Delphi avec les autorisations utilisateur appropriées.

3. **Accédez à l'interface d'installation des paquets :** naviguez dans le menu jusqu'à `Component > Install Packages...`.

4. **Lancez l'ajout du paquet :** cliquez sur le bouton « Add... » pour démarrer le processus d'installation.

5. **Localisez les fichiers de paquet 32 bits :** parcourez le répertoire contenant vos fichiers de paquet compilés en **32 bits** (`.bpl`). Sélectionnez avec soin le fichier `.bpl` 32 bits et cliquez sur « Open » pour continuer.

6. **Terminez le processus d'installation :** le paquet doit apparaître dans la liste « Design packages », généralement activé par défaut. Confirmez l'installation en cliquant sur « OK ».

### Vérification et dépannage

L'IDE tentera de charger le paquet 32 bits. En cas de succès, vos composants doivent apparaître dans la palette d'outils, et vous pouvez immédiatement les utiliser dans le concepteur de formulaires. Si l'IDE ne parvient pas à charger le paquet, vérifiez que vous avez sélectionné le bon fichier `.bpl` 32 bits et assurez-vous que tous les paquets de dépendances requis par votre paquet cible sont correctement installés et accessibles.

**Avertissement critique :** n'essayez jamais d'installer des fichiers `.bpl` 64 bits via l'option de menu `Component > Install Packages...`. De telles tentatives échoueront invariablement, car l'architecture 32 bits de l'IDE ne peut pas charger de modules de code 64 bits.

## Configuration avancée : définition des chemins de bibliothèque du projet pour le développement bi-plateforme

### Configuration des chemins de recherche du compilateur

Tandis que l'IDE utilise des paquets 32 bits lors des opérations de conception, le compilateur Delphi a besoin d'informations précises sur l'emplacement des fichiers appropriés (`.dcu`, `.dcp`, `.obj`) pour votre plateforme cible spécifique lors de la compilation (32 bits ou 64 bits). Ces paramètres se configurent via les options du projet, plus précisément dans la section de configuration du chemin de bibliothèque. Surtout, ces paramètres doivent être définis séparément pour chaque plateforme cible.

1. **Accédez à la configuration du projet :** naviguez dans `Project > Options...` du menu de l'IDE.

2. **Sélectionnez la plateforme appropriée :** il est absolument crucial de configurer les chemins séparément pour chaque plateforme cible. Utilisez la liste déroulante « Target Platform » située en haut de la boîte de dialogue Options du projet. Commencez la configuration par la sélection « 32-bit Windows ».

3. **Naviguez vers la section de configuration de la bibliothèque :** dans l'arborescence d'options affichée à gauche, sélectionnez `Delphi Compiler > Library` pour accéder aux paramètres de chemins.

4. **Configurez les chemins de bibliothèque 32 bits :** dans le champ « Library path », cliquez sur le bouton ellipse (...) pour ouvrir l'éditeur de chemins. Ajoutez le répertoire contenant vos unités compilées **32 bits** (fichiers `.dcu`) et le fichier `.dcp` du paquet **32 bits** pour les composants que vous avez installés. Assurez-vous que ce chemin référence bien le répertoire de sortie 32 bits de votre bibliothèque de composants.

5. **Passez à la configuration 64 bits :** changez la sélection de la liste déroulante « Target Platform » pour « 64-bit Windows ». Notez que le champ « Library path » peut afficher un contenu différent ou apparaître vide.

6. **Configurez les chemins de bibliothèque 64 bits :** répétez le processus de configuration de chemin précédent, mais ajoutez cette fois les répertoires contenant vos unités compilées **64 bits** (fichiers `.dcu`) et le fichier `.dcp` du paquet **64 bits**. Ce chemin *doit* différer du chemin 32 bits et référencer correctement le répertoire de sortie 64 bits.

7. **Vérifiez les paramètres de chemins supplémentaires :** bien que la configuration de Library path soit essentielle pour localiser les fichiers `.dcu` et `.dcp`, examinez aussi les paramètres `Browsing path` (utilisés par les fonctionnalités de code insight) et vérifiez que l'emplacement du `DCP output directory` est correctement configuré si vous construisez vous-même des paquets. Configurez ces chemins pour les plateformes 32 bits et 64 bits également.

8. **Enregistrez les modifications de configuration :** cliquez sur « OK » pour conserver les paramètres d'options du projet.

### Éviter les erreurs courantes de configuration

**Erreur fréquente :** de nombreux développeurs oublient de changer la liste déroulante « Target Platform » *avant* de définir le chemin pour cette plateforme. Configurer le chemin 64 bits alors que « 32-bit Windows » reste sélectionné (ou inversement) représente une source courante d'erreurs de compilation ultérieures.

En établissant correctement ces chemins de bibliothèque spécifiques à la plateforme, vous fournissez au compilateur les informations précises sur l'emplacement des fichiers `.dcu` et `.dcp` nécessaires pour l'architecture en cours de construction.

## Stratégies de gestion des paquets d'exécution (runtime)

### Choisir les approches d'édition de liens

Au-delà d'indiquer au compilateur où trouver les unités lors de la compilation, vous devez déterminer comment votre exécutable final sera lié aux bibliothèques de composants. Cette décision critique se contrôle dans la section « Runtime Packages ».

Vous avez deux options principales :

1. **Édition de liens statique :** si vous laissez l'option « Link with runtime packages » décochée (ou supprimez tous les paquets de la liste), le compilateur incorporera directement le code et les ressources nécessaires de vos composants dans le fichier `.exe` final. Cette approche produit des fichiers exécutables plus volumineux, mais évite d'avoir à distribuer des fichiers `.bpl` séparés avec votre application.

2. **Édition de liens dynamique (paquets d'exécution) :** si vous activez « Link with runtime packages » et spécifiez les paquets requis, le compilateur n'intégrera *pas* le code des composants dans votre `.exe`. À la place, votre application chargera dynamiquement les fichiers `.bpl` nécessaires à l'exécution. Cette stratégie crée des fichiers exécutables plus petits, mais nécessite de déployer les fichiers `.bpl` 32 bits ou 64 bits correspondants avec la distribution de votre application.

### Processus de configuration détaillé

1. **Accédez aux options du projet :** naviguez vers `Project > Options...` dans le menu de l'IDE.

2. **Sélectionnez la plateforme cible :** choisissez « 32-bit Windows » ou « 64-bit Windows » dans la liste déroulante de plateforme.

3. **Naviguez vers les paramètres de paquets :** sélectionnez `Packages > Runtime Packages` dans l'arborescence de navigation des options.

4. **Configurez la méthode d'édition de liens :** activez ou désactivez l'option « Link with runtime packages » selon l'approche déterminée précédemment.

5. **Spécifiez les paquets requis :** lors de l'utilisation des paquets d'exécution, assurez-vous que la liste contient les noms de base corrects des paquets dont votre application a besoin (par exemple, `MyComponentPackage`). N'incluez *pas* de suffixes de plateforme ou d'extensions de fichier dans ces entrées. Delphi ajoute automatiquement les identifiants de plateforme appropriés et charge les fichiers `_x86.bpl` ou `_x64.bpl` corrects (ou un nommage équivalent selon la version/configuration de Delphi) à l'exécution.

6. **Configurez la plateforme secondaire :** changez la sélection « Target Platform » et configurez les paramètres de paquets d'exécution de manière identique pour la plateforme alternative. En général, la décision d'utiliser ou non les paquets d'exécution reste cohérente entre les deux plateformes, mais les listes de paquets peuvent différer si vous utilisez des bibliothèques spécifiques à une plateforme.

7. **Préservez la configuration :** cliquez sur « OK » pour enregistrer les paramètres.

### Considérations relatives au déploiement

**Exigence critique de déploiement :** si vous choisissez l'édition de liens dynamique avec des paquets d'exécution, n'oubliez pas que vous *devez* distribuer la version architecturale correcte (32 bits ou 64 bits) de ces fichiers `.bpl` avec votre application. L'exécutable 32 bits nécessite des fichiers `.bpl` 32 bits, tandis que l'exécutable 64 bits a besoin de fichiers `.bpl` 64 bits. Placez ces fichiers dans le même répertoire que le `.exe` ou dans des emplacements accessibles via la variable d'environnement PATH du système.

## Méthodologies complètes de test et de vérification

### Vérification multiplateforme

La configuration seule ne peut garantir le succès. Des tests approfondis deviennent essentiels pour confirmer que tout fonctionne comme prévu sur les deux plateformes cibles.

1. **Compilation multiplateforme :** construisez explicitement votre projet pour les plateformes cibles « 32-bit Windows » et « 64-bit Windows ». Traitez toutes les erreurs de compilateur qui surviennent au cours de ce processus. Les erreurs apparaissant à la compilation indiquent fréquemment des chemins de bibliothèque mal configurés (détaillés à l'étape 2).

2. **Tests d'exécution 32 bits :** exécutez l'application compilée en 32 bits. Testez en profondeur toute la fonctionnalité dépendant des composants concernés. Recherchez en particulier :
   * Une apparence visuelle correcte et un comportement interactif normal des composants.
   * L'absence d'exceptions lors de l'instanciation de composants ou de l'invocation de méthodes.
   * Si vous utilisez des paquets d'exécution, vérifiez que l'application démarre sans message d'erreur « Package XYZ not found ».

3. **Tests d'exécution 64 bits :** exécutez l'application compilée en 64 bits. Effectuez des tests identiques à ceux conduits sur la version 32 bits. Portez une attention particulière à :
   * Toute différence de comportement par rapport à la version 32 bits.
   * Les erreurs d'exécution telles que les violations d'accès, susceptibles d'indiquer des problèmes sous-jacents de compatibilité 64 bits dans le code du composant ou la logique de l'application (par exemple, arithmétique de pointeurs incorrecte, hypothèses sur la taille des entiers).
   * Pour les paquets d'exécution, vérifiez à nouveau l'absence d'erreurs de paquets manquants, en vous assurant que les fichiers `.bpl` 64 bits sont correctement accessibles.

4. **Évaluation des cas limites :** incluez des scénarios de test explorant les conditions limites, en particulier l'utilisation mémoire si c'est une motivation pour passer au 64 bits. Chargez des jeux de données volumineux et effectuez des opérations complexes impliquant les composants pour soumettre l'implémentation à un test de charge.

### Interprétation des résultats de test

Toute divergence ou erreur rencontrée à l'exécution sur une plateforme mais pas sur l'autre suggère fortement soit un problème de configuration de paquet (étapes 2 ou 3), soit d'éventuels problèmes de compatibilité 64 bits dans le code du composant ou de l'application lui-même. Ces problèmes nécessitent un diagnostic minutieux et une résolution ciblée.

## Guide de dépannage avancé

### Résoudre les problèmes d'installation courants

* **« Package XYZ.bpl can't be installed because it is not a design time package. »** : cette erreur indique généralement une tentative d'installer via `Component > Install Packages` un paquet dépourvu des enregistrements design-time ou indicateurs de configuration nécessaires. Vérifiez que le projet du paquet est correctement configuré comme paquet de conception ou paquet combiné conception et exécution.

* **« Can't load package XYZ.bpl. %1 is not a valid Windows application. » / « The specified module could not be found. »** : cela indique presque à coup sûr une tentative d'installer un BPL **64 bits** dans l'IDE 32 bits via `Component > Install Packages`. N'oubliez pas d'installer uniquement des fichiers BPL 32 bits via cette interface. La variante « module not found » peut également se produire si le paquet a des dépendances qui ne sont pas correctement installées ou ne peuvent être localisées.

* **[Compiler Error] F1026 File not found: 'ComponentUnit.dcu'** : cette erreur se produit pendant la compilation (et non à la conception). Elle indique que le compilateur ne peut pas localiser le fichier `.dcu` requis pour la plateforme cible actuellement sélectionnée. Examinez attentivement les paramètres `Project Options > Delphi Compiler > Library > Library path` pour la *plateforme spécifique* que vous compilez actuellement (étape 2). Assurez-vous que le chemin référence correctement le répertoire approprié (32 bits ou 64 bits) contenant les fichiers `.dcu` nécessaires.

* **[Linker Error] E2202 Required package 'XYZ' not found** : similaire à F1026, mais survenant pendant la phase de liaison. Cela indique fréquemment que le fichier `.dcp` du paquet est introuvable. Vérifiez que le Library Path (étape 2) inclut le répertoire contenant le fichier `.dcp` de la plateforme correcte. De plus, assurez-vous que le nom du paquet apparaît correctement dans `Project Options > Packages > Runtime Packages` si vous utilisez l'édition de liens dynamique (étape 3).

* **Runtime Error: « Package XYZ not found »** : cela indique que votre application a été compilée pour utiliser des paquets d'exécution, mais que le fichier `.bpl` requis (correspondant à l'architecture de l'application) ne peut pas être localisé au démarrage de l'application. Assurez-vous que les fichiers `.bpl` 32 bits ou 64 bits corrects sont déployés à côté de votre fichier `.exe` (comme décrit à l'étape 3).

* **Violations d'accès à l'exécution (AV) uniquement en 64 bits :** cela indique généralement des problèmes de compatibilité 64 bits dans le code (soit dans votre application, soit dans l'implémentation du composant). Les sources courantes incluent :
  * Une arithmétique de pointeurs supposant `SizeOf(Pointer)=4` (valide uniquement en code 32 bits).
  * Une utilisation incorrecte de `Integer` au lieu de `NativeInt`/`NativeUInt` pour des handles ou des valeurs de la taille d'un pointeur.
  * Des appels directs aux fonctions de l'API Windows utilisant des types de données incorrects pour les environnements 64 bits.
  * Des problèmes d'alignement de structures de données.
  
  Le débogage de l'application 64 bits devient nécessaire pour identifier la cause spécifique de ces violations.

## Travailler avec des paquets de composants tiers

### Bonnes pratiques pour les composants externes

Les principes décrits tout au long de ce guide s'appliquent également aux composants tiers. Les éditeurs de composants réputés fournissent généralement :

1. Des instructions détaillées pour les procédures d'installation appropriées.
2. Des fichiers `.bpl`, `.dcp` et `.dcu` compilés séparément pour 32 bits et 64 bits.
3. Un utilitaire d'installation qui gère le placement des fichiers aux emplacements appropriés et automatise potentiellement l'installation des paquets de conception 32 bits dans l'IDE.

Si un programme d'installation est fourni, utilisez-le en première approche. Cependant, validez toujours ensuite les options du projet (Library Paths, Runtime Packages), car les programmes d'installation peuvent ne pas configurer parfaitement les chemins pour toutes les configurations de projet ou versions de Delphi possibles. Si vous ne recevez que des fichiers de bibliothèque bruts sans programme d'installation, suivez manuellement les étapes 1 à 3 en identifiant et en configurant soigneusement les chemins des versions 32 bits et 64 bits fournies par l'éditeur. En cas de problèmes, consultez la documentation de l'éditeur ou contactez son équipe de support technique pour obtenir de l'aide.

## Synthèse et recommandations

### Stratégies clés de mise en œuvre

Gérer avec succès les paquets Delphi pour le développement 32 bits et 64 bits dépend fondamentalement de la compréhension de la nature 32 bits de l'IDE et d'une configuration méticuleuse des options de projet pour chaque plateforme cible de manière indépendante. Installez toujours le paquet 32 bits pour l'utilisation à la conception, puis établissez avec soin les Library Paths et les paramètres de Runtime Packages spécifiques à chaque plateforme afin que le compilateur et votre application finale puissent localiser et utiliser les fichiers corrects pour l'architecture cible.

Bien que cette approche introduise une complexité supplémentaire par rapport au développement purement 32 bits, la méthodologie structurée vous permet de tirer parti des avantages substantiels de la compilation 64 bits tout en conservant une expérience de conception pleinement fonctionnelle dans l'environnement familier de l'IDE Delphi. Des tests cohérents sur les deux plateformes représentent l'étape de vérification finale et cruciale pour garantir des applications robustes et fiables fonctionnant de manière optimale dans les environnements 32 bits et 64 bits.

---
Besoin d'informations supplémentaires ? Veuillez [contacter le support](https://support.visioforge.com/) pour une assistance sur des scénarios spécifiques ou des problèmes de composants.
