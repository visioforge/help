---
title: Installer Media Player SDK dans Delphi — config 32/64 bits
description: Installez le VisioForge Media Player SDK dans Delphi 10.x-12.x : composants VCL/FMX, enregistrement des paquets, chemins de bibliothèques. Windows 32/64 bits.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Playback
  - MP4
primary_api_classes:
  - TVFMediaPlayer

---

# Installation de TVFMediaPlayer dans Delphi

Bienvenue dans le guide détaillé d'installation du VisioForge Media Player SDK, et plus précisément du composant `TVFMediaPlayer`, dans votre environnement de développement Delphi. Ce guide couvre les installations pour les versions classiques de Delphi comme Delphi 6 et 7, ainsi que pour les versions modernes à partir de Delphi 2005, y compris les versions les plus récentes prenant en charge le développement 64 bits.

## Comprendre TVFMediaPlayer

`TVFMediaPlayer` est un puissant composant VCL de VisioForge conçu pour intégrer en toute fluidité des capacités de lecture vidéo et audio dans des applications Delphi. Il simplifie des tâches telles que la lecture de divers formats multimédias, la capture d'instantanés, le contrôle de la vitesse de lecture, la gestion des flux audio, et bien plus encore. Bâti sur un moteur multimédia robuste, il offre des performances élevées et une prise en charge étendue des formats, ce qui en fait un choix polyvalent pour le développement d'applications multimédias en Delphi.

Ce guide suppose que vous disposez d'une installation fonctionnelle d'Embarcadero Delphi ou d'une version compatible plus ancienne (Borland Delphi).

## Étape 1 : prérequis et téléchargement du framework

Avant de procéder à l'installation, assurez-vous que votre environnement de développement satisfait aux prérequis nécessaires. Vous avez principalement besoin d'une version sous licence ou d'évaluation de Delphi installée sur votre machine Windows.

Le composant `TVFMediaPlayer` est distribué dans le cadre du VisioForge All-in-One Media Framework. Ce framework regroupe plusieurs SDK VisioForge, fournissant une boîte à outils complète pour la gestion multimédia.

1. **Naviguez vers la page produit :** ouvrez votre navigateur web et accédez à la [page produit officielle All-in-One Media Framework](https://www.visioforge.com/all-in-one-media-framework) de VisioForge.
2. **Sélectionnez la version Delphi :** localisez la section de téléchargement spécifique à Delphi. VisioForge propose généralement des versions adaptées à différentes plateformes de développement.
3. **Téléchargez :** cliquez sur le lien de téléchargement pour obtenir le fichier exécutable d'installation (`.exe`). Enregistrez ce fichier dans un emplacement connu de votre ordinateur, comme votre dossier Téléchargements.

Le fichier téléchargé contient non seulement le composant `TVFMediaPlayer`, mais aussi d'autres bibliothèques associées, le code source (le cas échéant selon la licence), les fichiers d'exécution nécessaires et la documentation.

## Étape 2 : exécuter le programme d'installation

Une fois le téléchargement terminé, vous devez exécuter le programme d'installation pour placer les fichiers nécessaires du SDK sur votre système.

1. **Localisez le programme d'installation :** naviguez vers le dossier où vous avez enregistré le fichier `.exe` téléchargé.
2. **Exécutez en tant qu'administrateur :** cliquez avec le bouton droit sur le fichier d'installation et sélectionnez « Exécuter en tant qu'administrateur ». Cela est essentiel car le programme d'installation doit enregistrer des composants et peut être amené à écrire dans des répertoires système, ce qui nécessite des privilèges élevés.
3. **Suivez les instructions à l'écran :** l'assistant d'installation vous guidera tout au long du processus. Cela implique généralement :
    * D'accepter le contrat de licence.
    * De choisir le répertoire d'installation (l'emplacement par défaut est généralement approprié, par exemple dans `C:\Program Files (x86)\VisioForge\` ou similaire). Notez ce chemin, vous en aurez besoin par la suite.
    * De sélectionner les composants à installer (assurez-vous que le Media Player SDK est sélectionné).
    * De confirmer l'installation.
4. **Terminez l'installation :** laissez le programme d'installation finir de copier les fichiers et d'effectuer les tâches de configuration nécessaires.

Ce processus décompresse le SDK, y compris les fichiers source (`.pas`), les unités précompilées (`.dcu`), les fichiers de paquets (`.dpk`, `.bpl`) et éventuellement les DLL requises.

## Étape 3 : intégration avec l'IDE Delphi

Après avoir exécuté le programme d'installation principal, l'étape critique suivante consiste à intégrer le composant `TVFMediaPlayer` dans l'IDE Delphi afin de pouvoir l'utiliser visuellement dans le concepteur de formulaires et référencer ses unités dans votre code. Le processus diffère légèrement entre les versions plus anciennes (Delphi 6/7) et les plus récentes (Delphi 2005+).

**Important :** pour toutes les versions de Delphi, il est recommandé d'exécuter l'IDE Delphi lui-même **en tant qu'administrateur** pendant le processus d'installation du paquet. Cela permet d'éviter d'éventuels problèmes de permissions lors de la compilation et de l'enregistrement du paquet du composant.

### Installation dans Delphi 6 / Delphi 7

Ces versions plus anciennes nécessitent une configuration manuelle des chemins et l'installation des paquets.

1. **Lancez Delphi (en tant qu'administrateur) :** démarrez votre IDE Delphi 6 ou Delphi 7 avec des privilèges d'administrateur.
2. **Ouvrez les options de l'IDE :** allez dans le menu `Tools` et sélectionnez `Environment Options`.
3. **Configurez le chemin de bibliothèque :**
    * Naviguez vers l'onglet `Library`.
    * Dans le champ `Library path`, cliquez sur le bouton à points de suspension (`...`).
    * Cliquez sur le bouton `Add` ou `New` (l'icône peut varier) et naviguez vers le répertoire `Source` situé dans le chemin d'installation VisioForge que vous avez noté précédemment (par exemple `C:\Program Files (x86)\VisioForge\Media Player SDK\Source`). Ajoutez ce chemin. Cela indique à Delphi où trouver les fichiers source `.pas` si nécessaire lors de la compilation ou du débogage.
    * Cliquez sur `OK` pour fermer l'éditeur de chemins.
4. **Configurez le chemin de navigation :**
    * Toujours dans l'onglet `Library`, localisez le champ `Browsing path` (il peut être combiné ou distinct selon la version/mise à jour exacte de Delphi).
    * Ajoutez ici également le même chemin du répertoire `Source`. Cela aide l'IDE à localiser les fichiers pour des fonctionnalités telles que la complétion de code et la navigation.
    * Cliquez sur `OK` pour enregistrer les options d'environnement.
5. **Ouvrez le fichier de paquet :**
    * Allez dans le menu `File` et sélectionnez `Open...`.
    * Naviguez vers le sous-dossier `Packages\Delphi7` (ou `Delphi6`) dans le répertoire d'installation VisioForge (par exemple `C:\Program Files (x86)\VisioForge\Media Player SDK\Packages\Delphi7`).
    * Localisez le fichier de paquet d'exécution, souvent nommé quelque chose comme `VFMediaPlayerD7_R.dpk` (le « R » désigne habituellement runtime). Ouvrez-le.
    * Répétez le processus pour ouvrir le paquet de conception, souvent nommé `VFMediaPlayerD7_D.dpk` (le « D » désigne design-time).
6. **Compilez le paquet d'exécution :**
    * Assurez-vous que le paquet d'exécution (`*_R.dpk`) est le projet actif dans le gestionnaire de projets.
    * Cliquez sur le bouton `Compile` dans la fenêtre du gestionnaire de projets (ou utilisez l'option de menu correspondante, par exemple `Project -> Compile`). Résolvez les éventuelles erreurs de compilation (cela est généralement inutile avec les paquets officiels).
7. **Compilez et installez le paquet de conception :**
    * Faites du paquet de conception (`*_D.dpk`) le projet actif.
    * Cliquez sur le bouton `Compile`.
    * Une fois la compilation réussie, cliquez sur le bouton `Install` dans le gestionnaire de projets.
8. **Confirmation :** vous devriez voir un message de confirmation indiquant que le(s) paquet(s) ont été installés. Le composant `TVFMediaPlayer` (et éventuellement d'autres du SDK) devraient désormais apparaître dans la palette de composants Delphi, probablement sous un onglet de catégorie « VisioForge » ou similaire.

*Remarque sur l'architecture :* Delphi 6/7 sont strictement des environnements 32 bits (x86). Vous n'installerez et n'utiliserez donc que la version 32 bits du composant `TVFMediaPlayer`. Le SDK peut contenir des fichiers 64 bits, mais ils ne sont pas applicables ici.

### Installation dans Delphi 2005 et versions ultérieures (XE, 10.x, 11.x, 12.x)

Les versions modernes de Delphi offrent un processus plus rationalisé et une prise en charge robuste de plusieurs plateformes (Win32, Win64).

1. **Lancez Delphi (en tant qu'administrateur) :** démarrez votre IDE Delphi (par exemple Delphi 11 Alexandria, Delphi 12 Athens) avec des privilèges d'administrateur.
2. **Ouvrez les options de l'IDE :** allez dans `Tools -> Options`.
3. **Configurez le chemin de bibliothèque :**
    * Dans la boîte de dialogue Options, naviguez vers `Language -> Delphi -> Library` (le chemin exact peut légèrement varier selon les versions).
    * Sélectionnez la plateforme cible pour laquelle vous souhaitez configurer le chemin (par exemple `Windows 32-bit`, `Windows 64-bit`). Il est recommandé de configurer les deux si vous prévoyez de construire pour les deux architectures.
    * Cliquez sur le bouton à points de suspension (`...`) en regard du champ `Library path`.
    * Ajoutez le chemin vers le répertoire `Source` approprié dans l'installation VisioForge (par exemple `C:\Program Files (x86)\VisioForge\Media Player SDK\Source`).
    * Cliquez sur `Add` puis sur `OK`. Répétez pour l'autre plateforme si vous le souhaitez.
4. **Configurez le chemin de navigation (facultatif mais recommandé) :**
    * Dans la même section `Library`, ajoutez aussi le chemin `Source` au champ `Browsing path`.
    * Cliquez sur `OK` pour enregistrer les options.
5. **Ouvrez le fichier de paquet :**
    * Allez dans `File -> Open Project...`.
    * Naviguez vers le répertoire `Packages` dans l'installation VisioForge. Trouvez le sous-dossier correspondant à votre version de Delphi (par exemple `Delphi11`, `Delphi12`).
    * Ouvrez le fichier de paquet de conception approprié (par exemple `VFMediaPlayerD11_D.dpk`). Les Delphi modernes gèrent souvent les dépendances entre les paquets d'exécution et de conception de manière plus automatique, vous n'aurez donc peut-être besoin d'ouvrir explicitement que le paquet de conception.
6. **Compilez et installez :**
    * Dans le gestionnaire de projets, cliquez avec le bouton droit sur le projet de paquet (fichier `.dpk`).
    * Sélectionnez `Compile` dans le menu contextuel.
    * Une fois la compilation réussie, cliquez à nouveau avec le bouton droit et sélectionnez `Install`.
7. **Confirmation :** Delphi confirme l'installation, et les composants apparaissent dans la palette.

*Remarque sur l'architecture :* Delphi moderne prend en charge à la fois les cibles 32 bits (Win32) et 64 bits (Win64). Le SDK VisioForge fournit généralement des unités précompilées (`.dcu`) pour les deux. Lorsque vous compilez et installez le paquet, Delphi gère habituellement l'enregistrement pour la plateforme actuellement active. Vous pouvez changer de plateforme dans le gestionnaire de projets et reconstruire/réinstaller si nécessaire, bien que l'IDE gère souvent correctement cette association après l'installation initiale.

## Étape 4 : configuration du projet

Après avoir installé le paquet du composant dans l'IDE, vous devez vous assurer que vos *projets* individuels peuvent trouver les fichiers VisioForge nécessaires à la compilation et à l'exécution.

1. **Options du projet :** ouvrez votre projet Delphi (fichier `.dpr`). Allez dans `Project -> Options`.
2. **Chemin de bibliothèque :** naviguez vers `Delphi Compiler -> Search path` (ou similaire selon la version).
3. **Ajoutez le chemin du SDK :** pour chaque plateforme cible (`Windows 32-bit`, `Windows 64-bit`) que vous prévoyez d'utiliser :
    * Ajoutez le chemin vers le répertoire `Source` de VisioForge (par exemple `C:\Program Files (x86)\VisioForge\Media Player SDK\Source`). Cela garantit que le compilateur peut trouver les fichiers `.pas` ou les fichiers `.dcu` requis. Parfois, les fichiers `.dcu` précompilés sont fournis dans des sous-répertoires spécifiques à la plateforme (par exemple `DCU\Win32`, `DCU\Win64`) ; si c'est le cas, ajoutez ces chemins spécifiques en remplacement ou en complément du chemin principal `Source`. Consultez la documentation VisioForge ou la structure d'installation pour les détails.
4. **Enregistrez les modifications :** cliquez sur `OK` ou `Save` pour appliquer les options du projet.

Définir correctement le chemin de recherche du projet est essentiel. Si le compilateur signale qu'il ne trouve pas d'unités comme `VisioForge_MediaPlayer_Engine` ou similaire, des chemins de recherche incorrects ou manquants en sont la cause la plus fréquente.

## Étape 5 : vérification

Pour confirmer que l'installation a réussi :

1. **Vérifiez la palette de composants :** recherchez l'onglet « VisioForge » (ou similaire) dans la palette de composants de l'IDE Delphi. Vous devriez voir l'icône `TVFMediaPlayer`.
2. **Créez une application de test :**
    * Créez une nouvelle application de formulaires VCL (`File -> New -> VCL Forms Application - Delphi`).
    * Glissez-déposez le composant `TVFMediaPlayer` de la palette vers le formulaire principal.
    * Si le composant apparaît sur le formulaire sans erreur, l'installation à la conception est probablement correcte.
    * Ajoutez un bouton simple. Dans son gestionnaire d'événement `OnClick`, ajoutez une ligne de code basique pour interagir avec le lecteur, par exemple :

        ```delphi
        procedure TForm1.Button1Click(Sender: TObject);
        begin
          // Assurez-vous que VFMediaPlayer1 est le nom de l'instance de votre composant
          VFMediaPlayer1.FilenameOrURL := 'C:\chemin\vers\votre\test_video.mp4'; // Remplacez par un chemin de fichier multimédia réel
          VFMediaPlayer1.Play();
        end;
        ```

    * Compilez le projet (`Project -> Compile`). S'il compile sans erreur « Fichier non trouvé » liée aux unités VisioForge, la configuration des chemins est probablement correcte.
    * Exécutez l'application. Si elle se lance et que vous pouvez lire le fichier multimédia à l'aide du bouton, la configuration d'exécution fonctionne.

## Problèmes d'installation courants et dépannage

Bien que le processus soit généralement simple, des problèmes ponctuels peuvent survenir :

* **Permissions de l'IDE :** oublier d'exécuter l'IDE Delphi en tant qu'administrateur pendant l'installation du paquet peut entraîner des erreurs d'écriture dans le registre ou les dossiers système, empêchant l'enregistrement du composant. **Solution :** fermez Delphi, redémarrez-le en tant qu'administrateur et essayez à nouveau les étapes d'installation du paquet.
* **Erreurs de configuration des chemins :** des chemins incorrects, que ce soit dans le `Library Path` de l'IDE ou dans le `Search Path` du projet, sont fréquents. **Solution :** vérifiez deux fois que les chemins pointent *exactement* vers le répertoire `Source` (ou `DCU` pertinent) du SDK VisioForge. Assurez-vous que les chemins sont corrects pour la plateforme cible spécifique (Win32/Win64).
* **Erreurs de compilation du paquet :** parfois, des conflits avec d'autres paquets installés ou des problèmes au sein même de la source du paquet peuvent provoquer des échecs de compilation. **Solution :** assurez-vous d'utiliser la version correcte du paquet pour votre version spécifique de Delphi. Consultez le support ou les forums VisioForge si les erreurs persistent.
* **Problèmes spécifiques au 64 bits :** l'installation de paquets pour la plateforme 64 bits peut parfois présenter des défis particuliers, surtout dans les versions plus anciennes de Delphi qui ont introduit pour la première fois la prise en charge de Win64. Consultez l'article lié [Problème d'installation de paquet Delphi 64 bits](../../general/install-64bit.md) pour les problèmes connus spécifiques et les solutions de contournement.
* **Problèmes liés aux fichiers `.otares` :** certaines versions de Delphi utilisent des fichiers `.otares` pour les ressources. Des problèmes lors de l'installation du paquet liés à ces fichiers peuvent survenir. Voir l'article lié [Problème d'installation de paquet Delphi avec .otares](../../general/install-otares.md).
* **DLL d'exécution manquantes :** `TVFMediaPlayer` dépend souvent de DLL sous-jacentes (par exemple, composants FFmpeg) pour ses fonctionnalités. Bien que le programme d'installation principal les gère habituellement, assurez-vous qu'elles sont correctement placées soit dans le répertoire de sortie de votre application, soit dans un répertoire du PATH système, soit dans les dossiers System32/SysWOW64 selon le cas. Le déploiement nécessite de distribuer ces DLL nécessaires avec votre application. Consultez la documentation VisioForge pour la liste des fichiers d'exécution requis.

## Étapes complémentaires et ressources

Avec `TVFMediaPlayer` installé avec succès, vous pouvez désormais explorer ses nombreuses fonctionnalités.

* **Explorez les propriétés et événements :** utilisez l'inspecteur d'objets Delphi pour examiner les nombreuses propriétés et événements disponibles pour le composant `TVFMediaPlayer`.
* **Consultez la documentation :** reportez-vous à la documentation officielle VisioForge installée avec le SDK ou disponible en ligne pour des références d'API détaillées et des exemples d'utilisation.
* **Exemples de code :** visitez le [dépôt GitHub](https://github.com/visioforge/) VisioForge pour trouver des projets de démonstration et des extraits de code illustrant diverses fonctionnalités.
* **Demander du support :** si vous rencontrez des problèmes persistants ou si vous avez des questions spécifiques non couvertes ici, contactez le [support VisioForge](https://support.visioforge.com/) pour obtenir de l'aide.

---
Veuillez contacter le [support](https://support.visioforge.com/) pour obtenir de l'aide sur ce tutoriel. Visitez notre page [GitHub](https://github.com/visioforge/) pour consulter d'autres exemples de code.