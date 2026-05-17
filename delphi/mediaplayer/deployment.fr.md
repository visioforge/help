---
title: Guide de déploiement TVFMediaPlayer — Delphi et ActiveX
description: Déployez des applications TVFMediaPlayer avec installateurs silencieux ou configuration manuelle. Codecs, filtres DirectShow, VC++ et dépendances.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Playback
  - IP Camera
primary_api_classes:
  - TVFMediaPlayer

---

# Guide de déploiement pour TVFMediaPlayer

Le déploiement d'applications construites avec la bibliothèque TVFMediaPlayer nécessite de s'assurer que tous les composants nécessaires sont correctement installés et configurés sur la machine cible. Ce guide fournit des instructions détaillées pour les méthodes de déploiement automatisées et manuelles, adaptées à différents scénarios et exigences techniques. Que vous préfériez la simplicité des programmes d'installation silencieux ou le contrôle granulaire d'une configuration manuelle, ce document couvre les étapes essentielles pour déployer avec succès votre application de lecteur multimédia Delphi ou ActiveX.

## Comprendre les exigences de déploiement

Avant de déployer votre application, il est crucial de comprendre les dépendances de la bibliothèque TVFMediaPlayer. La bibliothèque repose sur plusieurs composants centraux, notamment des runtimes de base, des codecs spécifiques (comme FFMPEG ou VLC pour certaines sources), et les redistribuables Microsoft Visual C++. La méthode de déploiement que vous choisirez déterminera comment ces dépendances sont gérées.

### Composants centraux

* **Bibliothèque de base :** contient le moteur essentiel et les filtres DirectShow pour les fonctionnalités de lecture de base.
* **Paquets de codecs :** facultatifs mais souvent nécessaires pour prendre en charge une large gamme de formats multimédias et de flux réseau (par exemple, caméras IP). FFMPEG et VLC sont des choix courants fournis.
* **Dépendances d'exécution :** les paquets redistribuables Microsoft Visual C++ sont requis pour que les composants principaux de la bibliothèque fonctionnent correctement.

Choisir la bonne stratégie de déploiement dépend de facteurs comme les privilèges utilisateur sur la machine cible, la nécessité d'une installation sans surveillance et les fonctionnalités spécifiques de votre application (par exemple, quelles sources multimédias elle doit prendre en charge).

## Méthode 1 : installation automatisée (droits d'administrateur requis)

L'utilisation des programmes d'installation silencieux fournis est la méthode la plus directe pour déployer les composants de la bibliothèque TVFMediaPlayer. Ces programmes d'installation gèrent l'enregistrement des fichiers nécessaires et garantissent que toutes les dépendances sont correctement placées. Cette méthode nécessite des privilèges administratifs sur la machine cible car elle implique des changements au niveau du système comme l'enregistrement de composants COM et potentiellement la modification du PATH système.

### Programmes d'installation disponibles

VisioForge fournit des programmes d'installation distincts pour la bibliothèque de base et les paquets de codecs facultatifs, avec des versions pour Delphi et ActiveX, ainsi que pour les architectures x86 et x64.

#### Paquet de base (obligatoire)

Ce paquet installe les composants centraux de TVFMediaPlayer et les filtres DirectShow essentiels. Il est toujours requis, quelles que soient les sources multimédias utilisées par votre application. Choisissez le programme d'installation correspondant à votre environnement de développement (Delphi ou ActiveX) et à l'architecture cible (x86 ou x64).

* **Delphi :**
  * [Programme d'installation x86](https://files.visioforge.com/redists_delphi/redist_media_player_base_delphi.exe)
  * [Programme d'installation x64](https://files.visioforge.com/redists_delphi/redist_media_player_base_delphi_x64.exe)
* **ActiveX :**
  * [Programme d'installation x86](https://files.visioforge.com/redists_delphi/redist_media_player_base_ax.exe)
  * [Programme d'installation x64](https://files.visioforge.com/redists_delphi/redist_media_player_base_ax_x64.exe)

#### Paquet FFMPEG (facultatif — pour les sources fichier/caméra IP)

Si votre application doit lire des fichiers locaux ou recevoir des flux de caméras IP via le moteur FFMPEG, vous devez déployer ce paquet. FFMPEG offre une large gamme de prise en charge de codecs.

* **FFMPEG :**
  * [Programme d'installation x86](https://files.visioforge.com/redists_delphi/redist_media_player_ffmpeg.exe)
  * *Remarque : un lien vers un programme d'installation FFMPEG x64 n'était pas explicitement fourni dans la source d'origine ; supposez que la version x86 couvre la plupart des besoins, ou consultez la documentation VisioForge pour les spécificités x64 si nécessaire.*

#### Paquet source VLC (facultatif — pour les sources fichier/caméra IP)

En alternative ou en complément de FFMPEG, vous pouvez utiliser le moteur VLC pour les sources fichier et caméra IP. Cela nécessite de déployer le paquet VLC. Assurez-vous de sélectionner la bonne architecture.

* **VLC :**
  * [Programme d'installation x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe)
  * [Programme d'installation x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)

### Utilisation du programme d'installation

Ces programmes d'installation sont conçus pour une exécution silencieuse, ce qui les rend adaptés à l'inclusion dans des routines de configuration d'applications plus larges ou au déploiement via des scripts. Exécutez le ou les exécutables avec les privilèges d'administrateur sur la machine cible.

```bash
# Exemple : exécution silencieuse du programme d'installation de base Delphi x86
redist_media_player_base_delphi.exe /S
```

*(Remarque : le commutateur silencieux exact peut varier ; consultez la documentation du programme d'installation ou utilisez les commutateurs standard comme `/S`, `/silent` ou `/q` si `/S` ne fonctionne pas.)*

## Méthode 2 : installation manuelle (droits d'administrateur recommandés)

L'installation manuelle offre plus de contrôle mais demande une exécution soignée de chaque étape. Cette méthode convient lorsque les programmes d'installation automatisés ne peuvent pas être utilisés, ou lors du déploiement dans des environnements avec des restrictions spécifiques. Bien que certaines étapes puissent être réalisées sans droits d'administrateur complets, l'enregistrement des composants COM nécessite généralement une élévation des privilèges.

### Prérequis

Avant de copier les fichiers de la bibliothèque, assurez-vous que les dépendances d'exécution nécessaires sont présentes sur le système cible.

#### Installer le redistribuable VC++ 2010 SP1

La bibliothèque TVFMediaPlayer repose sur le runtime Microsoft Visual C++ 2010 SP1. Installez la version appropriée (x86 ou x64) pour l'architecture cible de votre application.

* **VC++ 2010 SP1 :**
  * [Redistribuable x86](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
  * [Redistribuable x64](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

Exécutez ces programmes d'installation avant de procéder au déploiement des fichiers de la bibliothèque.

### Déploiement des fichiers de la bibliothèque centrale

Suivez ces étapes pour installer manuellement les composants de la bibliothèque de base :

1. **Copiez les DLL centrales :** localisez le dossier `Redist\Filters` dans votre répertoire d'installation TVFMediaPlayer. Copiez tous les fichiers DLL de ce dossier vers un répertoire de déploiement sur la machine cible. Une pratique courante consiste à placer ces DLL dans le même dossier que l'exécutable de votre application.
2. **Enregistrez les filtres DirectShow :** la fonctionnalité centrale repose sur plusieurs filtres DirectShow (fichiers `.ax`). Ils doivent être enregistrés auprès du système d'exploitation Windows via l'enregistrement Component Object Model (COM).
    * **Identifiez les filtres :** les filtres clés à enregistrer sont :
        * `VisioForge_Audio_Effects_4.ax`
        * `VisioForge_Dump.ax`
        * `VisioForge_RGB2YUV.ax`
        * `VisioForge_Video_Effects_Pro.ax`
        * `VisioForge_YUV2RGB.ax`
        * *(Remarque : d'autres fichiers `.ax` peuvent être présents ; enregistrez tous les fichiers `.ax` trouvés dans le répertoire `Redist\Filters`.)*
    * **Méthode d'enregistrement :** utilisez l'outil en ligne de commande `regsvr32.exe`, qui fait partie de Windows. Ouvrez une invite de commandes **en tant qu'administrateur** et exécutez la commande pour chaque fichier `.ax`.

        ```bash
        # Exemple : enregistrement d'un filtre (à exécuter depuis le répertoire contenant le fichier .ax)
        regsvr32.exe VisioForge_Video_Effects_Pro.ax
        ```

        Alternativement, VisioForge fournit un utilitaire `reg_special.exe` dans les redistribuables. Copiez cet utilitaire dans le dossier contenant les fichiers `.ax` et exécutez-le avec les privilèges d'administrateur pour enregistrer automatiquement tous les filtres de ce répertoire. Consultez la documentation Microsoft pour résoudre les erreurs `regsvr32.exe` : [Comment utiliser l'outil Regsvr32](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5).
3. **Mettez à jour le PATH système (facultatif mais recommandé) :** si les DLL de filtre et les fichiers `.ax` sont placés dans un répertoire distinct de l'exécutable de votre application, vous devez ajouter le chemin de ce répertoire à la variable d'environnement système `PATH`. Cela permet au système d'exploitation et à votre application de localiser ces fichiers essentiels. Ne pas le faire peut entraîner des erreurs « DLL not found » ou des erreurs d'enregistrement de filtre.

### Déploiement manuel des paquets facultatifs

#### Déploiement de FFMPEG

1. **Copiez les fichiers :** copiez tout le contenu du dossier `Redist\FFMPEG` de votre installation TVFMediaPlayer vers un répertoire de déploiement sur la machine cible (par exemple, un sous-dossier du répertoire d'installation de votre application).
2. **Mettez à jour le PATH système :** ajoutez le chemin complet du dossier où vous avez copié les fichiers FFMPEG à la variable d'environnement système `PATH` de Windows. C'est crucial pour que la bibliothèque puisse trouver et charger les composants FFMPEG.

#### Déploiement de VLC (exemple : x86)

1. **Copiez les fichiers :** copiez tout le contenu du dossier `Redist\VLC` (spécifiquement la version x86 si elle s'applique) vers un répertoire de déploiement.
2. **Enregistrez le filtre VLC :** localisez le fichier `.ax` parmi les fichiers VLC copiés (par exemple `axvlc.dll` ou similaire, bien que le texte d'origine mentionne seulement de façon générique « le fichier .ax ») et enregistrez-le à l'aide de `regsvr32.exe` avec les privilèges d'administrateur.
3. **Définissez la variable d'environnement :** créez une nouvelle variable d'environnement système nommée `VLC_PLUGIN_PATH`. Définissez sa valeur sur le chemin complet du sous-dossier `plugins` du répertoire où vous avez copié les fichiers VLC (par exemple, `C:\YourApp\VLC\plugins`). Cela indique au moteur VLC où trouver ses modules de plugin nécessaires.

## Vérification et dépannage

Après le déploiement, testez minutieusement votre application sur la machine cible.

* Vérifiez les fonctionnalités de lecture de base.
* Testez toutes les fonctionnalités spécifiques reposant sur les paquets facultatifs (FFMPEG ou VLC), comme la lecture de divers formats de fichiers ou la connexion à des caméras IP.
* Si des erreurs surviennent, vérifiez deux fois :
  * Les droits d'administrateur durant l'installation/l'enregistrement.
  * L'installation correcte des redistribuables VC++.
  * L'enregistrement réussi de tous les fichiers `.ax` (vérifiez la sortie de `regsvr32.exe`).
  * La configuration correcte des variables d'environnement `PATH` et `VLC_PLUGIN_PATH`.
  * La concordance correcte de l'architecture (x86/x64) entre votre application, les composants de la bibliothèque et les dépendances d'exécution.

---
Besoin d'aide supplémentaire ? Contactez le [Support VisioForge](https://support.visioforge.com/). Découvrez d'autres exemples sur notre [GitHub](https://github.com/visioforge/).
