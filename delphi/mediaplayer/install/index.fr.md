---
title: Guide d'installation TVFMediaPlayer pour Delphi et ActiveX
description: Installez TVFMediaPlayer dans Delphi, C++ Builder, Visual Basic 6, Visual Studio et les environnements ActiveX, avec instructions détaillées.
sidebar_label: Guide d'installation
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Playback
  - Streaming
primary_api_classes:
  - TVFMediaPlayer

---

# Installer la bibliothèque TVFMediaPlayer

Bienvenue dans le guide d'installation détaillé de la bibliothèque VisioForge TVFMediaPlayer, composant central du puissant All-in-One Media Framework. Ce guide propose des étapes complètes pour installer la bibliothèque dans divers environnements de développement intégrés (IDE), afin que vous puissiez exploiter pleinement ses riches capacités de lecture multimédia dans vos projets.

La bibliothèque TVFMediaPlayer offre aux développeurs des outils robustes pour intégrer des fonctionnalités de lecture, traitement et streaming audio et vidéo dans leurs applications. Elle est disponible sous deux formes principales pour répondre à différents écosystèmes de développement :

1.  **Paquet Delphi natif :** optimisé spécifiquement pour les développeurs Embarcadero Delphi, offrant une intégration transparente, une prise en charge à la conception et tirant pleinement parti du framework VCL.
2.  **Contrôle ActiveX (OCX) :** conçu pour une large compatibilité, permettant l'intégration dans les environnements prenant en charge la technologie ActiveX, comme C++ Builder, Microsoft Visual Basic 6 (VB6), Microsoft Visual Studio (pour les projets C#, VB.NET, C++ MFC) et d'autres conteneurs ActiveX.

Cette double disponibilité garantit que, que vous travailliez dans l'écosystème Delphi ou que vous utilisiez d'autres outils de développement populaires, vous pouvez exploiter la puissance de TVFMediaPlayer.

## Avant de commencer : exigences système et prérequis

Avant de procéder à l'installation, assurez-vous que votre environnement de développement répond aux exigences nécessaires :

*   **Système d'exploitation :** Windows 7, 8, 8.1, 10, 11, ou Windows Server 2012 R2 et plus récent (les versions x86 et x64 sont prises en charge).
*   **Environnement de développement :** un IDE compatible, par exemple :
    *   Embarcadero Delphi (consultez la version spécifique du framework pour connaître les versions Delphi compatibles, généralement XE2 ou plus récent).
    *   Embarcadero C++ Builder (consultez la version spécifique du framework pour la compatibilité).
    *   Microsoft Visual Studio 2010 ou plus récent (pour le développement C#, VB.NET, C++ MFC utilisant ActiveX).
    *   Microsoft Visual Basic 6 (nécessite l'IDE installé).
    *   Tout autre IDE ou outil de développement capable d'héberger des contrôles ActiveX.
*   **Dépendances :**
    *   **DirectX :** Microsoft DirectX 9 ou ultérieur est généralement requis. Les versions modernes de Windows incluent des runtimes DirectX compatibles ; assurez-vous qu'ils sont à jour.
    *   **.NET Framework (pour utilisation .NET) :** si vous utilisez le contrôle ActiveX dans des applications .NET (C#, VB.NET), assurez-vous que la version .NET Framework ciblée par votre projet est installée.
*   **Privilèges d'administrateur :** l'exécution du programme d'installation nécessite généralement des droits d'administrateur pour enregistrer les composants et écrire dans les répertoires système.

## Processus d'installation général étape par étape

Le processus d'installation central consiste à télécharger le programme d'installation du All-in-One Media Framework et à l'exécuter. Suivez ces étapes avec soin :

1.  **Téléchargez le framework :**
    *   Naviguez vers la page produit officielle [All-in-One Media Framework](https://www.visioforge.com/all-in-one-media-framework) sur le site web de VisioForge.
    *   Localisez la section des téléchargements. Vous pouvez trouver différentes versions (par exemple Trial, Full) ou builds. Téléchargez la dernière version stable adaptée à vos besoins. Faites attention à savoir si vous avez besoin du programme d'installation du paquet spécifique à Delphi ou du programme d'installation ActiveX général, s'ils sont fournis séparément (souvent, un seul programme d'installation contient les deux).
    *   Enregistrez le fichier exécutable d'installation (`.exe`) à un emplacement pratique sur votre ordinateur.

2.  **Exécutez le programme d'installation :**
    *   Localisez le fichier d'installation téléchargé (par exemple `visioforge_media_framework_setup.exe`).
    *   Cliquez avec le bouton droit sur le fichier et sélectionnez « Exécuter en tant qu'administrateur » pour garantir les permissions nécessaires.
    *   Si le Contrôle de compte d'utilisateur (UAC) vous y invite, confirmez que vous souhaitez autoriser le programme d'installation à apporter des modifications à votre appareil.

3.  **Suivez l'assistant d'installation :**
    *   **Écran de bienvenue :** le programme d'installation se lance, commençant généralement par un message d'accueil. Cliquez sur « Suivant » pour continuer.
    *   **Contrat de licence :** lisez attentivement le contrat de licence utilisateur final (EULA). Vous devez accepter les termes pour continuer l'installation. Sélectionnez l'option appropriée et cliquez sur « Suivant ».
    *   **Sélectionner l'emplacement de destination :** choisissez le répertoire où seront installés les fichiers du framework, les exemples et la documentation. L'emplacement par défaut est généralement dans `C:\Program Files (x86)\VisioForge\` ou similaire. Vous pouvez parcourir pour un autre chemin si nécessaire. Cliquez sur « Suivant ».
    *   **Sélectionner les composants (le cas échéant) :** certains programmes d'installation peuvent vous permettre de choisir quels composants installer (par exemple, fonctionnalités spécifiques du framework, documentation, exemples pour différents langages). Assurez-vous que les composants centraux de Media Player et tous les exemples pertinents (Delphi, C#, VB.NET, C++, VB6) sont sélectionnés. Cliquez sur « Suivant ».
    *   **Sélectionner le dossier du menu Démarrer :** choisissez le nom du dossier du menu Démarrer où seront créés les raccourcis. Cliquez sur « Suivant ».
    *   **Prêt à installer :** vérifiez vos options sélectionnées. Si tout est correct, cliquez sur « Installer » pour démarrer le processus de copie des fichiers et d'enregistrement système.
    *   **Progression de l'installation :** l'assistant affiche la progression de l'installation. Cela peut prendre quelques minutes. Pendant cette phase, les DLL et fichiers OCX nécessaires sont copiés, et le contrôle ActiveX est enregistré dans le registre Windows.
    *   **Achèvement :** une fois l'installation terminée, un écran de fin s'affiche. Il peut offrir des options pour consulter la documentation ou lancer un projet d'exemple. Cliquez sur « Terminer » pour quitter l'assistant.

4.  **Vérification post-installation :**
    *   Naviguez vers le répertoire d'installation que vous avez choisi (par exemple, `C:\Program Files (x86)\VisioForge\Media Framework\`).
    *   Vérifiez que les fichiers de bibliothèque centraux (`.dll`, `.ocx`), la documentation (`.chm` ou dossier `Docs`) et les projets d'exemple (dossier `Examples`) sont présents.
    *   Consultez le dossier du menu Démarrer pour les raccourcis vers la documentation et les exemples.
    *   Il est fortement recommandé d'essayer de compiler et d'exécuter l'un des projets d'exemple fournis pour votre IDE spécifique afin de confirmer que l'installation a réussi et que les composants sont correctement enregistrés et accessibles.

## Intégration spécifique à l'IDE

Après l'installation générale, vous devez intégrer la bibliothèque TVFMediaPlayer dans votre environnement de développement choisi.

### Delphi (paquets natifs)

L'utilisation des paquets Delphi natifs offre la meilleure expérience aux développeurs Delphi, incluant l'intégration des composants à la conception.

*   **Guide détaillé :** pour des instructions complètes spécifiques à Delphi, y compris l'ajout du chemin de bibliothèque et l'installation des paquets de conception et d'exécution (fichiers `.dpk`), consultez le **[Guide d'installation Delphi](delphi.md)** dédié.
*   **Avantages clés :** accès direct à la palette de composants, inspecteurs de propriétés, gestionnaires d'événements intégrés à l'IDE, et performances optimisées pour les applications VCL.

### Intégration ActiveX (C++ Builder, VB6, Visual Studio, etc.)

Si vous n'utilisez pas Delphi ou si vous préférez l'approche ActiveX, vous devrez ajouter le contrôle `TVFMediaPlayer.ocx` à votre projet.

#### C++ Builder

L'intégration du contrôle ActiveX dans C++ Builder consiste à l'importer dans l'IDE.

*   **Guide détaillé :** consultez le **[Guide d'installation C++ Builder](builder.md)** pour des instructions pas à pas sur l'importation du contrôle ActiveX, qui implique généralement l'utilisation de la fonctionnalité « Import Component » ou « Import ActiveX Control » de l'IDE pour générer le code wrapper nécessaire.
*   **Vue d'ensemble du processus :** cela implique généralement de naviguer vers `Component -> Import Component...`, de sélectionner « Import ActiveX Control », de trouver « VisioForge Media Player SDK » (ou un nom similaire) dans la liste des contrôles enregistrés, et de laisser l'IDE générer les classes wrapper C++ correspondantes qui vous permettent d'interagir avec le contrôle.

#### Visual Basic 6 (VB6)

VB6 repose largement sur la technologie ActiveX, ce qui simplifie l'intégration.

1.  **Ouvrez le projet :** lancez Visual Basic 6 et ouvrez votre projet existant ou créez-en un nouveau.
2.  **Accédez à la boîte de dialogue Composants :** allez dans le menu principal et sélectionnez `Project -> Components...`. Cela ouvre la boîte de dialogue Composants listant les contrôles enregistrés.
3.  **Localisez et sélectionnez le contrôle :** parcourez la liste sous l'onglet « Controls ». Cherchez une entrée comme « VisioForge Media Player SDK Control » ou similaire (le nom exact peut varier légèrement selon la version). Cochez la case en regard.
4.  **Ajout via Parcourir (si non répertorié) :** si le contrôle n'est pas listé (peut-être en raison d'un problème d'enregistrement), cliquez sur le bouton « Browse... ». Naviguez vers le répertoire d'installation VisioForge (en particulier le sous-dossier `Redist\AnyCPU` ou similaire contenant `TVFMediaPlayer.ocx`) et sélectionnez le fichier `.ocx`. Cliquez sur « Open ». Cela devrait enregistrer et ajouter le contrôle à la liste. Assurez-vous que sa case est cochée.
5.  **Confirmez :** cliquez sur « OK » ou « Apply » dans la boîte de dialogue Composants.
6.  **Utilisez le contrôle :** l'icône TVFMediaPlayer devrait maintenant apparaître dans votre boîte à outils VB6. Vous pouvez la faire glisser sur vos formulaires pour l'utiliser visuellement, puis interagir avec ses propriétés et méthodes par code.

#### Visual Studio (C#, VB.NET, C++ MFC)

Visual Studio gère les contrôles ActiveX via la couche d'interopérabilité COM.

1.  **Ouvrez le projet :** lancez Visual Studio et ouvrez votre projet Windows Forms (C# ou VB.NET), WPF ou MFC.
2.  **Ouvrez la boîte à outils :** assurez-vous que la boîte à outils est visible (`View -> Toolbox`).
3.  **Ajoutez le contrôle à la boîte à outils :**
    *   Cliquez avec le bouton droit dans la boîte à outils, de préférence dans un onglet pertinent comme « General » ou « All Windows Forms », ou créez un nouvel onglet (par exemple « VisioForge »).
    *   Sélectionnez « Choose Items... ».
    *   Attendez le chargement de la boîte de dialogue « Choose Toolbox Items ». Cela peut prendre un moment pendant l'analyse des composants enregistrés.
    *   Naviguez vers l'onglet « COM Components ».
    *   Parcourez la liste et cherchez « VisioForge Media Player SDK Control » ou un nom similaire. Cochez la case en regard.
    *   **Ajout via Parcourir (si non répertorié) :** si vous ne le trouvez pas, cliquez sur le bouton « Browse... ». Naviguez vers le répertoire d'installation VisioForge (généralement le sous-dossier `Redist\AnyCPU`) et sélectionnez le fichier `TVFMediaPlayer.ocx`. Cliquez sur « Open ». Cela devrait l'ajouter à la liste ; assurez-vous que sa case est désormais cochée.
    *   Cliquez sur « OK ».
4.  **Utilisez le contrôle :** l'icône du contrôle TVFMediaPlayer sera désormais disponible dans votre boîte à outils Visual Studio. Faites-la glisser sur votre formulaire (Windows Forms) ou utilisez-la par programmation (WPF, MFC). Visual Studio générera automatiquement les assemblys d'interopérabilité (wrappers) nécessaires pour permettre au code managé (.NET) ou C++ d'interagir avec le contrôle ActiveX basé sur COM.

## Dépannage des problèmes d'installation courants

Vous rencontrez des problèmes lors de l'installation ? Voici quelques problèmes courants et leurs solutions :

*   **Contrôle non enregistré / n'apparaît pas dans l'IDE :**
    *   Assurez-vous que le programme d'installation a été exécuté avec les privilèges d'administrateur.
    *   Essayez d'enregistrer manuellement le fichier OCX. Ouvrez une **invite de commandes en tant qu'administrateur**, naviguez vers le répertoire contenant `TVFMediaPlayer.ocx` (par exemple `cd "C:\Program Files (x86)\VisioForge\Media Framework\Redist\AnyCPU"`), et exécutez `regsvr32 TVFMediaPlayer.ocx`. Un message de réussite doit apparaître.
    *   Vérifiez les conflits avec d'autres bibliothèques multimédias ou des versions plus anciennes de VisioForge. Envisagez de désinstaller d'abord les versions précédentes.
*   **L'installation échoue ou est annulée :**
    *   Assurez-vous de répondre à toutes les exigences système, y compris les versions de DirectX et .NET.
    *   Désactivez temporairement le logiciel antivirus, qui peut interférer avec le processus d'enregistrement. N'oubliez pas de le réactiver ensuite.
    *   Vérifiez l'espace disque disponible sur le lecteur cible.
*   **Problèmes dans des IDE spécifiques :**
    *   **Delphi :** assurez-vous que le chemin de bibliothèque est correctement ajouté dans `Tools -> Options -> Library Path` et que les fichiers `BPL` corrects sont installés. La reconstruction des paquets peut aider.
    *   **Visual Studio :** supprimez les dossiers `obj` et `bin` de votre projet, supprimez tous les assemblys d'interopérabilité existants liés à VisioForge, supprimez la référence au contrôle, redémarrez Visual Studio et essayez d'ajouter à nouveau le contrôle. Assurez-vous que votre projet cible une version compatible de .NET Framework, le cas échéant.

## Mise à jour du framework

Pour passer à une version plus récente du All-in-One Media Framework :

1.  **Vérifiez la compatibilité :** consultez les notes de version pour comprendre les modifications et les éventuels problèmes de compatibilité avec vos projets existants.
2.  **Sauvegardez les projets :** sauvegardez toujours vos projets avant de mettre à jour une dépendance de bibliothèque majeure.
3.  **Désinstallez la version existante (recommandé) :** il est généralement recommandé de désinstaller la version actuelle via le Panneau de configuration Windows (« Ajout ou suppression de programmes » ou « Applications et fonctionnalités ») avant d'installer la nouvelle. Cela permet d'éviter les conflits de fichiers ou les problèmes d'enregistrement.
4.  **Téléchargez et installez :** téléchargez le programme d'installation de la nouvelle version et suivez la procédure d'installation standard décrite précédemment dans ce guide.
5.  **Recompilez les projets :** ouvrez vos projets dans leurs IDE respectifs. Vous devrez peut-être supprimer et rajouter des références ou des composants si les interfaces sous-jacentes ont changé de manière significative (bien que cela soit moins courant avec les mises à jour mineures). Recompilez l'ensemble de votre projet.
6.  **Testez minutieusement :** testez votre application en profondeur pour vous assurer que toutes les fonctionnalités multimédias fonctionnent comme prévu avec la bibliothèque mise à jour.

## Désinstallation

Pour supprimer la bibliothèque TVFMediaPlayer et le All-in-One Media Framework :

1.  **Fermez les IDE :** assurez-vous que tous les environnements de développement susceptibles d'utiliser les fichiers de la bibliothèque sont fermés.
2.  **Utilisez le désinstalleur Windows :**
    *   Allez dans le Panneau de configuration Windows ou l'application Paramètres.
    *   Naviguez vers « Programmes et fonctionnalités » ou « Applications et fonctionnalités ».
    *   Localisez « VisioForge Media Framework » (ou nom similaire) dans la liste des programmes installés.
    *   Sélectionnez-le et cliquez sur « Désinstaller ».
    *   Suivez les invites de l'assistant de désinstallation. Ce processus doit supprimer les fichiers installés et tenter de désenregistrer le contrôle ActiveX.
3.  **Nettoyage manuel (facultatif) :** dans certains cas rares, ou si vous voulez vous assurer d'une suppression complète, vous pouvez vérifier et supprimer manuellement :
    *   Le répertoire d'installation principal (par exemple, `C:\Program Files (x86)\VisioForge\`).
    *   Tous les fichiers de configuration ou entrées de registre restants (utilisateurs avancés uniquement, à faire avec prudence).
    *   Les assemblys d'interopérabilité générés dans les dossiers de votre projet (`obj`, `bin`).

## Licences et activation

Le All-in-One Media Framework fonctionne généralement sous licence commerciale, souvent avec une période d'essai.

*   **Version d'essai :** le programme d'installation téléchargé peut fonctionner initialement comme une version d'essai, qui peut comporter des limitations (par exemple, écrans de rappel, limites de temps, fonctionnalités restreintes).
*   **Achat d'une licence :** pour débloquer toutes les capacités et utiliser le framework dans des applications de production, vous devez acheter une licence sur le site VisioForge.
*   **Activation :** après l'achat, vous recevrez généralement une clé de licence ou des instructions sur l'activation du logiciel. Cela peut impliquer la saisie de la clé dans une propriété spécifique du contrôle à l'exécution ou l'utilisation d'un outil d'activation de licence fourni par VisioForge. Consultez la documentation accompagnant votre licence achetée pour les détails précis.

## Obtenir du support

Si vous rencontrez des problèmes non traités ici ou si vous avez besoin d'aide supplémentaire :

*   **Documentation officielle :** consultez le dossier `Docs` de votre répertoire d'installation ou la documentation en ligne sur le site VisioForge. Le fichier d'aide `CHM` contient souvent des références API détaillées et des exemples d'utilisation.
*   **Projets d'exemple :** explorez les projets d'exemple fournis pour votre IDE. Ils illustrent des cas d'utilisation courants et des techniques d'implémentation correctes.
*   **Support VisioForge :** visitez la section support du site VisioForge. Elle peut inclure des forums, une base de connaissances ou des options de contact direct pour les utilisateurs sous licence.

## Conclusion

Installer la bibliothèque TVFMediaPlayer, qu'il s'agisse d'un paquet Delphi natif ou d'un contrôle ActiveX, est un processus simple lorsque l'on suit ces étapes détaillées. En vous assurant que les exigences système sont satisfaites, en exécutant soigneusement l'assistant d'installation et en intégrant correctement les composants à l'IDE de votre choix, vous pouvez rapidement commencer à développer de puissantes applications multimédias. N'oubliez pas de consulter les guides spécifiques à l'IDE (Delphi, C++ Builder) liés ici et la documentation officielle pour des informations approfondies et des configurations avancées. Une fois le framework installé avec succès, vous êtes bien équipé pour explorer les nombreuses fonctionnalités du VisioForge All-in-One Media Framework.
