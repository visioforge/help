---
title: SDK C++ Video Fingerprinting — installation et configuration
description: Installez et configurez le VisioForge Video Fingerprinting SDK pour C++ — Visual Studio, paquets NuGet et votre première appli d'empreinte.
tags:
  - Video Fingerprinting SDK
  - C++
  - Windows
  - macOS
  - Linux
  - GStreamer
  - Fingerprinting
  - MP4
primary_api_classes:
  - VFPFingerprintSource
  - VFPFingerPrint

---

# Prise en main du Video Fingerprinting SDK C++

Bienvenue dans le VisioForge Video Fingerprinting SDK pour C++ ! Ce guide complet vous accompagnera dans tout ce dont vous avez besoin pour démarrer, de l'installation à votre première application opérationnelle. À la fin de ce guide, vous disposerez d'une base solide pour construire des applications d'empreinte vidéo haute performance en C++.

> **Note :** si vous êtes déjà familier avec le SDK .NET, vous trouverez que le SDK C++ suit des concepts similaires avec les avantages de performance native. Consultez notre [comparaison des SDK](../index.md#sdk-comparison) pour plus de détails.

## Résumé du démarrage rapide

Si vous cherchez à être opérationnel rapidement :

1. Télécharger le paquet SDK depuis VisioForge
2. Extraire les fichiers dans votre répertoire de projet
3. Inclure les en-têtes : `#include <VisioForge_VFP.h>` et `#include <VisioForge_VFP_Types.h>`
4. Lier la bibliothèque appropriée : `VisioForge_VideoFingerprinting.lib` (x86) ou `VisioForge_VideoFingerprinting_x64.lib` (x64)
5. Copier les DLL runtime vers votre répertoire de sortie
6. Définir votre clé de licence (si achetée) : `VFPSetLicenseKey(L"license-key");`
7. Générer votre première empreinte à l'aide des exemples ci-dessous

## Prérequis et configuration système

Pour les exigences détaillées, notamment les plateformes prises en charge, les spécifications matérielles et les considérations de performance, consultez notre guide complet de [configuration système requise](../system-requirements.md).

### Exigences spécifiques à C++

- **Compilateur Windows** : Visual Studio 2019+ (recommandé) ou MinGW-w64
- **Compilateur Linux** : GCC 7+ ou Clang 6+
- **Compilateur macOS** : Xcode 12+ avec Command Line Tools
- **Outils de build** : CMake 3.10+ (optionnel mais recommandé pour Linux/macOS)

## Contenu du paquet SDK

Après avoir téléchargé le SDK, vous trouverez la structure suivante :

```
VideoFingerprinting_CPP_SDK/
├── include/
│   ├── VisioForge_VFP.h           # En-tête principal de l'API
│   └── VisioForge_VFP_Types.h     # Définitions de types
├── lib/
│   ├── VisioForge_VideoFingerprinting.lib      # Bibliothèque d'import x86
│   └── VisioForge_VideoFingerprinting_x64.lib  # Bibliothèque d'import x64
├── redist/
│   ├── VisioForge_VideoFingerprinting.dll      # DLL runtime x86
│   ├── VisioForge_VideoFingerprinting_x64.dll  # DLL runtime x64
│   ├── VisioForge_FFMPEG_Source.dll           # Prise en charge média x86
│   └── VisioForge_FFMPEG_Source_x64.dll       # Prise en charge média x64
├── demos/
│   ├── vfp_gen/        # Démo de génération d'empreinte
│   ├── vfp_compare/    # Démo de comparaison vidéo
│   └── vfp_search/     # Démo de recherche de fragment
└── README.txt
```

## Configuration de votre environnement de développement

### Configuration Visual Studio (Windows)

#### Étape 1 : créer un nouveau projet

1. Ouvrez Visual Studio 2019 ou ultérieur
2. Cliquez sur « Créer un nouveau projet »
3. Sélectionnez « Application console » (C++)
4. Nommez votre projet (par exemple « VFPExample »)
5. Choisissez un emplacement et cliquez sur « Créer »

#### Étape 2 : configurer les propriétés du projet

1. Cliquez avec le bouton droit sur votre projet dans l'Explorateur de solutions
2. Sélectionnez « Propriétés »
3. Configurez les paramètres suivants :

**Propriétés de configuration → C/C++ → Général :**

```
Additional Include Directories: $(ProjectDir)include
```

**Propriétés de configuration → Éditeur de liens → Général :**

```
Additional Library Directories: $(ProjectDir)lib
```

**Propriétés de configuration → Éditeur de liens → Entrée :**

```
Additional Dependencies (x86): VisioForge_VideoFingerprinting.lib
Additional Dependencies (x64): VisioForge_VideoFingerprinting_x64.lib
```

#### Étape 3 : ajouter les fichiers SDK

1. Copiez le dossier `include` dans votre répertoire de projet
2. Copiez le dossier `lib` dans votre répertoire de projet
3. Copiez les fichiers DLL de `redist` vers votre répertoire de sortie (par exemple `Debug` ou `Release`)

#### Étape 4 : configurer les événements post-build

Ajoutez un événement post-build pour copier automatiquement les DLL :

```batch
xcopy /y "$(ProjectDir)redist\*.dll" "$(OutDir)"
```

### Configuration CMake (multiplateforme)

Créez un fichier `CMakeLists.txt` :

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Trouver les fichiers SDK
set(VFP_SDK_PATH "${CMAKE_CURRENT_SOURCE_DIR}/sdk")

# Répertoires d'inclusion
include_directories(${VFP_SDK_PATH}/include)

# Configuration spécifique à la plateforme
if(WIN32)
    # Configuration Windows
    link_directories(${VFP_SDK_PATH}/lib)
    
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        set(VFP_LIBRARIES VisioForge_VideoFingerprinting_x64)
        set(VFP_RUNTIME_LIBS 
            ${VFP_SDK_PATH}/redist/VisioForge_VideoFingerprinting_x64.dll
            ${VFP_SDK_PATH}/redist/VisioForge_FFMPEG_Source_x64.dll)
    else()
        set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
        set(VFP_RUNTIME_LIBS 
            ${VFP_SDK_PATH}/redist/VisioForge_VideoFingerprinting.dll
            ${VFP_SDK_PATH}/redist/VisioForge_FFMPEG_Source.dll)
    endif()
elseif(APPLE)
    # Configuration macOS
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
elseif(UNIX)
    # Configuration Linux
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
endif()

# Ajouter l'exécutable
add_executable(vfp_example main.cpp)

# Lier les bibliothèques
target_link_libraries(vfp_example ${VFP_LIBRARIES})

# Copier les bibliothèques runtime sous Windows
if(WIN32)
    foreach(DLL ${VFP_RUNTIME_LIBS})
        add_custom_command(TARGET vfp_example POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            ${DLL} $<TARGET_FILE_DIR:vfp_example>)
    endforeach()
endif()
```

Compilez le projet :

```bash
mkdir build
cd build
cmake ..
cmake --build .
```

### Configuration Linux

#### Installer les dépendances

Ubuntu/Debian :

```bash
sudo apt-get update
sudo apt-get install build-essential cmake
sudo apt-get install libgstreamer1.0-dev libgstreamer-plugins-base1.0-dev
sudo apt-get install gstreamer1.0-plugins-good gstreamer1.0-plugins-bad
sudo apt-get install gstreamer1.0-libav
```

Fedora/RHEL :

```bash
sudo dnf install gcc-c++ cmake
sudo dnf install gstreamer1-devel gstreamer1-plugins-base-devel
sudo dnf install gstreamer1-plugins-good gstreamer1-plugins-bad-free
sudo dnf install gstreamer1-libav
```

#### Configuration de build

Créez un Makefile simple :

```makefile
CXX = g++
CXXFLAGS = -std=c++11 -Wall -I./include
LDFLAGS = -L./lib -lVisioForge_VideoFingerprinting -Wl,-rpath,'$$ORIGIN/lib'

TARGET = vfp_example
SOURCES = main.cpp
OBJECTS = $(SOURCES:.cpp=.o)

all: $(TARGET)

$(TARGET): $(OBJECTS)
 $(CXX) $(OBJECTS) $(LDFLAGS) -o $(TARGET)

%.o: %.cpp
 $(CXX) $(CXXFLAGS) -c $< -o $@

clean:
 rm -f $(OBJECTS) $(TARGET)

.PHONY: all clean
```

### Configuration macOS

#### Installer les Command Line Tools de Xcode

```bash
xcode-select --install
```

#### Configuration de build

Créez un script de build `build.sh` :

```bash
#!/bin/bash

# Paramètres du compilateur
CXX=clang++
CXXFLAGS="-std=c++11 -Wall -I./include"
LDFLAGS="-L./lib -lVisioForge_VideoFingerprinting"

# Définir le chemin de la bibliothèque
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Compiler
$CXX $CXXFLAGS main.cpp $LDFLAGS -o vfp_example

echo "Compilation terminée. Exécuter avec : ./vfp_example"
```

Rendez-le exécutable et lancez-le :

```bash
chmod +x build.sh
./build.sh
```

## Votre première application { #your-first-application }

Créons une application simple qui génère une empreinte à partir d'un fichier vidéo.

### Étape 1 : créer main.cpp

```cpp
#include <iostream>
#include <string>
#include <cstring>

#ifdef _WIN32
#include <Windows.h>
#endif

#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

int main(int argc, char* argv[])
{
    std::cout << "VisioForge Video Fingerprinting SDK - Première application\n";
    std::cout << "==========================================================\n\n";
    
    // Vérifier les arguments de ligne de commande
    if (argc != 2) {
        std::cout << "Utilisation : " << argv[0] << " <fichier_video>\n";
        std::cout << "Exemple : " << argv[0] << " sample.mp4\n";
        return 1;
    }
    
    // Convertir le nom de fichier en caractères larges (pour compatibilité Windows)
    wchar_t videoFile[256];
#ifdef _WIN32
    size_t converted;
    mbstowcs_s(&converted, videoFile, argv[1], 256);
#else
    mbstowcs(videoFile, argv[1], 256);
#endif
    
    // Étape 1 : définir la clé de licence
    std::cout << "Définition de la clé de licence...\n";
    VFPSetLicenseKey(L"TRIAL");  // Utiliser "TRIAL" pour l'évaluation
    
    // Étape 2 : configurer la source
    VFPFingerprintSource src{};
    VFPFillSource(videoFile, &src);
    src.StartTime = 0;       // démarrer depuis le début
    src.StopTime = 0;        // 0 = jusqu'à la fin du fichier

    // Étape 3 : générer l'empreinte
    std::cout << "Génération de l'empreinte...\n";
    VFPFingerPrint fingerprint{};
    VFPSearch_GetFingerprintForVideoFile(src, &fingerprint);

    // Étape 4 : afficher les résultats
    std::cout << "\nEmpreinte générée avec succès !\n";
    std::cout << "================================\n";
    std::cout << "Informations vidéo :\n";
    std::cout << "  Durée : " << (fingerprint.Duration / 1000.0) << " secondes\n";
    std::cout << "  Résolution : " << fingerprint.Width << "x" << fingerprint.Height << "\n";
    std::cout << "  Fréquence d'images : " << fingerprint.FrameRate << " ips\n";
    std::cout << "  Taille des données : " << fingerprint.DataSize << " octets\n";
    
    // Étape 5 : enregistrer l'empreinte vers un fichier
    wchar_t outputFile[256];
#ifdef _WIN32
    wcscpy_s(outputFile, videoFile);
    wcscat_s(outputFile, L".vfpsig");
#else
    wcscpy(outputFile, videoFile);
    wcscat(outputFile, L".vfpsig");
#endif
    
    std::cout << "\nEnregistrement de l'empreinte...\n";
    VFPFingerprintSave(&fingerprint, outputFile);
    
    char outputFileAnsi[256];
#ifdef _WIN32
    size_t convertedOut;
    wcstombs_s(&convertedOut, outputFileAnsi, outputFile, 256);
#else
    wcstombs(outputFileAnsi, outputFile, 256);
#endif
    
    std::cout << "Empreinte enregistrée vers : " << outputFileAnsi << "\n";
    
    std::cout << "\nSuccès ! Vous pouvez maintenant utiliser cette empreinte pour :\n";
    std::cout << "  - Comparer avec d'autres vidéos pour la similarité\n";
    std::cout << "  - Rechercher cette vidéo dans des diffusions plus longues\n";
    std::cout << "  - Construire une base de données d'empreintes vidéo\n";
    
    return 0;
}
```

### Étape 2 : compiler et exécuter

#### Windows (Visual Studio)

1. Appuyez sur F7 pour compiler la solution
2. Copiez un fichier vidéo de test dans votre répertoire de sortie
3. Ouvrez l'invite de commandes dans le répertoire de sortie
4. Exécutez : `VFPExample.exe testvideo.mp4`

#### Windows (ligne de commande)

```batch
cl /EHsc /I.\include main.cpp /link /LIBPATH:.\lib VisioForge_VideoFingerprinting_x64.lib
copy redist\*.dll .
VFPExample.exe testvideo.mp4
```

#### Linux

```bash
g++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example
export LD_LIBRARY_PATH=./lib:$LD_LIBRARY_PATH
./vfp_example testvideo.mp4
```

#### macOS

```bash
clang++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH
./vfp_example testvideo.mp4
```

## Comprendre les types d'empreinte

Le SDK fournit deux types d'empreintes optimisées pour différents cas d'usage. Pour une explication complète incluant les détails techniques, les caractéristiques de performance et les conseils de décision, consultez notre [guide des types d'empreinte](../fingerprint-types.md).

**Référence rapide :**
- **Empreintes Search** (`VFPSearch_Init` / `_Process` / `_Build`) : pour trouver des fragments vidéo dans des vidéos plus grandes (détection de publicités, surveillance de contenu)
- **Empreintes Compare** (`VFPCompare_Init` / `_Process` / `_Build`) : pour comparer des vidéos entières pour la similarité (détection de doublons, comparaison de versions)

## Cas d'usage courants

### Cas d'usage 1 : détection de doublons

```cpp
// Générer les empreintes pour deux vidéos
VFPFingerPrint fp1{}, fp2{};
// ... générer les empreintes ...

// Les comparer
double difference = VFPCompare_Compare(
    fp1.Data, fp1.DataSize,
    fp2.Data, fp2.DataSize,
    10  // Autoriser jusqu'à 10 secondes de décalage
);

if (difference < 100) {
    std::cout << "Les vidéos sont des doublons\n";
}
```

### Cas d'usage 2 : détection de publicités

```cpp
// Charger les empreintes de publicité et de diffusion
VFPFingerPrint commercial{}, broadcast{};
VFPFingerprintLoad(&commercial, L"commercial.vfpsig");
VFPFingerprintLoad(&broadcast, L"broadcast.vfpsig");

// Rechercher la publicité
double diff;
int position = VFPSearch_Search2(
    &commercial, 0,
    &broadcast, 0,
    &diff, 300  // seuil
);

if (position != INT_MAX) {
    std::cout << "Publicité trouvée à : " << position << " secondes\n";
}
```

### Cas d'usage 3 : vérification de contenu avec zones ignorées

```cpp
VFPFingerprintSource src{};
VFPFillSource(L"video.mp4", &src);

// Ignorer les zones logo/filigrane (jusqu'à 10 rectangles)
src.IgnoredAreas[0].left   = 0;
src.IgnoredAreas[0].top    = 0;
src.IgnoredAreas[0].right  = 200;
src.IgnoredAreas[0].bottom = 100;    // Logo en haut à gauche

src.IgnoredAreas[1].left   = 1720;
src.IgnoredAreas[1].top    = 980;
src.IgnoredAreas[1].right  = 1920;
src.IgnoredAreas[1].bottom = 1080;   // Filigrane en bas à droite

VFPFingerPrint fp{};
VFPSearch_GetFingerprintForVideoFile(src, &fp);
```

## Licences

### Mode essai

Pour l'évaluation, utilisez la licence d'essai :

```cpp
VFPSetLicenseKey(L"TRIAL");
```

Limitations du mode essai :

- Ajoute un filigrane sur les images traitées
- Limité à 60 secondes de traitement par session
- Toutes les autres fonctionnalités sont disponibles

### Licence commerciale

Pour un usage en production, achetez une licence auprès de VisioForge :

```cpp
VFPSetLicenseKey(L"YOUR-LICENSE-KEY-HERE");
```

Types de licence :

- **Licence développeur** : pour le développement et les tests
- **Licence de déploiement** : pour la distribution avec votre application
- **Licence serveur** : pour les déploiements serveur

## Dépannage

### Problèmes courants et solutions

#### Problème : DLL introuvable

**Erreur** : « Le programme ne peut pas démarrer car VisioForge_VideoFingerprinting.dll est manquant »

**Solution** :

- Vérifiez que les DLL sont dans le même répertoire que votre exécutable
- Ou ajoutez le répertoire DLL à votre variable d'environnement PATH
- Vérifiez que vous utilisez la bonne architecture (x86 vs x64)

#### Problème : format vidéo non pris en charge

**Erreur** : « Impossible de traiter le fichier vidéo »

**Solution** :

- Vérifiez que le codec vidéo est pris en charge (H.264, H.265, VP8, VP9, AV1)
- Installez des paquets de codecs supplémentaires si nécessaire
- Essayez de convertir la vidéo dans un format standard

#### Problème : la clé de licence ne fonctionne pas

**Erreur** : « Clé de licence invalide »

**Solution** :

- Vérifiez que la clé de licence est entrée correctement
- Vérifiez que VFPSetLicenseKey() est appelée avant toute autre fonction du SDK
- Vérifiez que la licence n'a pas expiré
- Vérifiez que vous utilisez la bonne licence pour votre plateforme

#### Problème : violation d'accès mémoire

**Erreur** : violation d'accès en lecture à l'emplacement

**Solution** :

- Initialisez toutes les structures avec {} avant utilisation
- Vérifiez que les pointeurs sont valides avant utilisation
- Assurez une taille de tampon de chaîne appropriée
- Ne libérez pas manuellement la mémoire allouée par le SDK

#### Problème : performance médiocre

**Symptôme** : le traitement est plus lent que prévu

**Solution** :

- Utilisez la configuration de build Release, pas Debug
- Activez les optimisations du compilateur (/O2 ou -O2)
- Traitez les vidéos depuis un SSD local, pas depuis des lecteurs réseau
- Envisagez d'utiliser plusieurs threads pour le traitement par lots
- Réduisez la résolution vidéo si la qualité le permet

### Conseils de débogage

1. **Activer la sortie de débogage** : enregistrer les données d'image intermédiaires sur disque pour inspection
2. **Vérifier les valeurs de retour** : toujours vérifier les retours NULL/erreur
3. **Utiliser des builds debug** : développer initialement avec des symboles de débogage
4. **Journaliser les opérations** : ajouter de la journalisation pour suivre le flux de traitement
5. **Tester avec des fichiers connus** : utiliser des vidéos de référence pour vérifier la configuration

## Étapes suivantes

Maintenant que vous avez une configuration fonctionnelle, explorez ces sujets avancés :

1. **Traitement par lots** : traiter plusieurs fichiers efficacement
2. **Intégration de base de données** : stocker les empreintes dans une base de données
3. **Traitement en temps réel** : générer des empreintes à partir de flux en direct
4. **Interface graphique personnalisée** : construire des interfaces graphiques pour vos applications
5. **Optimisation des performances** : ajuster selon votre cas d'usage spécifique

### Lecture recommandée

- [Documentation de l'API C++](api.md) — référence complète de l'API
- [Comprendre l'empreinte vidéo](../understanding-video-fingerprinting.md) — contexte technique
- [Cas d'usage](../use-cases.md) — applications réelles

## Projets d'exemple

Le SDK inclut trois projets d'exemple complets :

### vfp_gen — génération d'empreinte

Démontre comment générer des empreintes avec diverses options :

```bash
vfp_gen source.mp4 output.sig 0 0 0
```

### vfp_compare — comparaison vidéo

Montre comment comparer deux vidéos pour la similarité :

```bash
vfp_compare video1.sig video2.sig 100 10
```

### vfp_search — recherche de fragment

Illustre la recherche de fragments vidéo :

```bash
vfp_search needle.sig haystack.sig 300
```

Étudiez ces exemples pour comprendre les bonnes pratiques et les schémas courants.

## Support et ressources

### Obtenir de l'aide

- **E-mail de support** : <support@visioforge.com>
- **Communauté Discord** : rejoignez pour de l'aide et des discussions en temps réel
- **Exemples GitHub** : exemples de code et intégrations supplémentaires

### Signaler des problèmes

Lors du signalement de problèmes, veuillez fournir :

1. Version du SDK et plateforme
2. Exemple de code minimal reproduisant le problème
3. Messages d'erreur et traces d'appel
4. Fichiers vidéo d'exemple (le cas échéant)
5. Spécifications système

## Conclusion

Vous disposez maintenant de tout ce dont vous avez besoin pour commencer à construire des applications d'empreinte vidéo avec le SDK C++. Le SDK fournit une fonctionnalité puissante avec d'excellentes performances, adaptée aux applications de bureau et aux déploiements serveur.

Pensez à :

- Commencer par les exemples fournis
- Tester en profondeur avec votre contenu cible
- Optimiser les paramètres pour votre cas d'usage
- Demander de l'aide en cas de besoin

Bonne programmation avec le VisioForge Video Fingerprinting SDK !
