---
title: API C++ Video Fingerprinting pour recherche et comparaison
description: Documentation complète de l'API C++ du VisioForge Video Fingerprinting SDK pour générer, comparer et rechercher des empreintes vidéo.
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

# Documentation de l'API C++ du Video Fingerprinting SDK

## Vue d'ensemble

Le Video Fingerprinting SDK C++ de VisioForge fournit une bibliothèque native haute performance pour les opérations d'identification, de comparaison et de recherche de contenu vidéo. Il permet aux applications de :

- Générer des empreintes uniques à partir de fichiers vidéo pour l'identification de contenu
- Comparer des vidéos pour déterminer la similarité et détecter les doublons
- Rechercher des fragments vidéo dans des vidéos plus longues (par exemple, trouver des publicités, des intros ou des scènes spécifiques)
- Comparer des images individuelles pour la détection de similarité
- Traiter directement des images vidéo pour générer des empreintes à partir de flux ou de contenu généré

Le SDK C++ offre des performances optimales pour les applications à haut débit et peut être intégré dans des applications C++ existantes ou utilisé via P/Invoke depuis d'autres langages.

> **Documentation associée :**
>
> - [Référence de l'API .NET](../dotnet/api.md) — pour les développeurs en code managé
> - [Comprendre l'empreinte vidéo](../understanding-video-fingerprinting.md) — concepts fondamentaux
> - [Types d'empreinte](../fingerprint-types.md) — modes Compare vs Search

## Table des matières

- [Fichiers d'en-tête](#header-files)
- [Gestion de licence](#license-management)
- [Types et structures principaux](#core-types-and-structures)
- [Fonctions de recherche](#search-functions)
- [Fonctions de comparaison](#compare-functions)
- [Fonctions utilitaires](#utility-functions)
- [Comparaison d'images](#image-comparison)
- [Exemples complets fonctionnels](#complete-working-examples)
- [Prise en charge des plateformes](#platform-support)
- [Compilation et édition de liens](#building-and-linking)
- [Considérations de performance](#performance-considerations)
- [Gestion des erreurs](#error-handling)

## Fichiers d'en-tête { #header-files }

Le SDK fournit deux fichiers d'en-tête principaux :

### VisioForge_VFP.h

En-tête principal de l'API contenant toutes les déclarations et exports de fonctions.

### VisioForge_VFP_Types.h

Définitions de types et structures de données utilisés par le SDK.

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>
```

## Gestion de licence { #license-management }

### VFPSetLicenseKey

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKey(wchar_t* licenseKey);
```

**Description :** définit la clé de licence du Video Fingerprinting SDK. Doit être appelée avant d'utiliser toute fonctionnalité d'empreinte.

**Paramètres :**

- `licenseKey` (wchar_t*) : votre clé de licence VisioForge sous forme de chaîne de caractères larges

**Exemple :**

```cpp
// Définir la clé de licence au démarrage de l'application
VFPSetLicenseKey(L"YOUR-LICENSE-KEY-HERE");

// Pour le mode essai
VFPSetLicenseKey(L"TRIAL");
```

### VFPSetLicenseKeyA

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKeyA(char* licenseKey);
```

**Description :** définit la clé de licence à l'aide d'une chaîne ANSI (alternative à VFPSetLicenseKey).

**Paramètres :**

- `licenseKey` (char*) : votre clé de licence VisioForge sous forme de chaîne ANSI

**Exemple :**

```cpp
// Définir la clé de licence avec une chaîne ANSI
VFPSetLicenseKeyA("YOUR-LICENSE-KEY-HERE");
```

## Types et structures principaux { #core-types-and-structures }

### VFPFingerprintSource

```cpp
struct VFPFingerprintSource
{
    wchar_t Filename[256];     // Chemin du fichier vidéo
    INT64 StartTime;            // Heure de début en millisecondes
    INT64 StopTime;             // Heure de fin en millisecondes
    RECT CustomCropSize;        // Zone de rognage personnalisée
    SIZE CustomResolution;      // Résolution personnalisée pour le traitement
    RECT IgnoredAreas[10];      // Zones à ignorer (par exemple logos, bandeaux)
    INT64 OriginalDuration;     // Durée originale du fichier
};
```

**Description :** structure de configuration pour la génération d'empreinte à partir de fichiers vidéo.

**Champs :**

- `Filename` : chemin du fichier vidéo (256 caractères maximum)
- `StartTime` : position de départ en millisecondes (0 pour le début)
- `StopTime` : position de fin en millisecondes (0 pour la fin du fichier)
- `CustomCropSize` : rectangle de rognage optionnel (left, top, right, bottom)
- `CustomResolution` : résolution personnalisée optionnelle pour le traitement
- `IgnoredAreas` : jusqu'à 10 zones rectangulaires à ignorer pendant la génération d'empreinte
- `OriginalDuration` : durée du fichier original en millisecondes

**Exemple :**

```cpp
VFPFingerprintSource source{};
wcscpy_s(source.Filename, L"C:\\Videos\\sample.mp4");
source.StartTime = 10000;  // Démarrer à 10 secondes
source.StopTime = 60000;   // Arrêter à 60 secondes

// Ignorer le logo en haut à droite
source.IgnoredAreas[0] = {1800, 0, 1920, 100};
```

### VFPFingerPrint

```cpp
struct VFPFingerPrint
{
    char* Data;                    // Données d'empreinte
    INT32 DataSize;                // Taille des données d'empreinte
    INT64 Duration;                // Durée en millisecondes
    GUID ID;                       // Identifiant unique
    wchar_t OriginalFilename[256]; // Nom de fichier original
    INT64 OriginalDuration;        // Durée du fichier original
    wchar_t Tag[100];              // Étiquette optionnelle
    INT32 Width;                   // Largeur de la vidéo source
    INT32 Height;                  // Hauteur de la vidéo source
    double FrameRate;              // Fréquence d'images
};
```

**Description :** structure contenant les données d'empreinte générées et les métadonnées.

**Champs :**

- `Data` : données d'empreinte binaires
- `DataSize` : taille des données d'empreinte en octets
- `Duration` : durée du contenu empreinté en millisecondes
- `ID` : identifiant GUID unique pour l'empreinte
- `OriginalFilename` : chemin du fichier vidéo original
- `OriginalDuration` : durée du fichier original
- `Tag` : étiquette optionnelle définie par l'utilisateur (jusqu'à 100 caractères)
- `Width` : largeur de la vidéo source
- `Height` : hauteur de la vidéo source
- `FrameRate` : fréquence d'images de la vidéo source

## Fonctions de recherche { #search-functions }

L'API Search fournit à la fois une API de haut niveau (génération d'empreinte à partir d'un fichier vidéo) et une API bas niveau par image (pour les flux en direct / décodeurs personnalisés).

### API de haut niveau

#### VFPSearch_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Génère une empreinte de recherche directement à partir d'un fichier vidéo. Retourne `NULL` en cas de succès, ou une chaîne de message d'erreur.

#### VFPSearch_GetFingerprintForVideoFileAndSave

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFileAndSave(
    wchar_t* sourceFilename,
    wchar_t* destFilename);
```

Génère et enregistre une empreinte en un seul appel.

#### VFPSearch_SearchOneSignatureFileInAnother

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_SearchOneSignatureFileInAnother(
    wchar_t* file1, wchar_t* file2,
    LONG threshold, LONG* position);
```

Recherche un fichier de signature dans un autre directement depuis le disque.

#### VFPSearch_Search2

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search2(
    VFPFingerPrint* vfp1, int iSkip1,
    VFPFingerPrint* vfp2, int iSkip2,
    double* pDiff, int maxDiff);
```

Recherche `vfp1` dans `vfp2`. Retourne la position en secondes, ou `INT_MAX` si non trouvée.

### API bas niveau par image

#### VFPSearch_Init

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_Init(int count, void* pDataTmp);
```

Initialise un accumulateur par image. `count` est le nombre attendu d'images.

#### VFPSearch_Init2

```cpp
extern "C" VFP_EXPORT void* VFP_EXPORT_CALL VFPSearch_Init2(int count);
```

Alloue et retourne un nouvel accumulateur. Utilisez `VFPSearch_Clear` pour le libérer.

#### VFPSearch_Process

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Process(
    unsigned char* p, int w, int h, int s,
    double dTime, void* pDataTmp);
```

Alimente une image RGB décodée. `dTime` est l'horodatage en secondes. Retourne 0 en cas de succès.

#### VFPSearch_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPSearch_Build(int* pLen, void* pDataTmp);
```

Finalise l'empreinte. Retourne un tampon `char*` ; `*pLen` reçoit sa taille en octets.

#### VFPSearch_Search

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search(
    const char* pData1, int iLen1, int iSkip1,
    const char* pData2, int iLen2, int iSkip2,
    double* pDiff, int maxDiff);
```

Recherche bas niveau utilisant des données d'empreinte brutes. Retourne la position en secondes.

#### VFPSearch_Clear

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_Clear(void* pDataTmp);
```

Libère les ressources allouées par `VFPSearch_Init` ou `VFPSearch_Init2`.

## Fonctions de comparaison { #compare-functions }

L'API Compare fournit à la fois une commodité de haut niveau et un accès bas niveau par image.

### API de haut niveau

#### VFPCompare_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPCompare_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Génère une empreinte de comparaison directement à partir d'un fichier vidéo.

#### VFPCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPCompare_Compare(
    const char* pData1, int iLen1,
    const char* pData2, int iLen2,
    int MaxS);
```

Compare deux tampons d'empreinte bruts. `MaxS` est le décalage temporel maximum en secondes. Retourne un score de différence (plus bas = plus similaire).

### API bas niveau par image

#### VFPCompare_Init

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPCompare_Init(int count, void* pDataTmp);
```

#### VFPCompare_Process

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPCompare_Process(
    unsigned char* p, int w, int h, int s,
    double dTime, void* pDataTmp);
```

Alimente une image RGB décodée. `dTime` est l'horodatage en secondes.

#### VFPCompare_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPCompare_Build(int* pLen, void* pDataTmp);
```

Finalise l'empreinte. Retourne un tampon `char*` ; `*pLen` reçoit sa taille.

#### VFPCompare_Clear

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPCompare_Clear(void* pDataTmp);
```

Libère les ressources de l'accumulateur.

## Fonctions utilitaires { #utility-functions }

### VFPFingerprintSave

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintSave(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Description :** enregistre une empreinte dans un fichier au format actuel.

**Paramètres :**

- `vfp` : pointeur vers l'empreinte à enregistrer
- `filename` : chemin du fichier de sortie

**Exemple :**

```cpp
VFPFingerPrint fingerprint{};
// ... générer l'empreinte ...
VFPFingerprintSave(&fingerprint, L"output.vfpsig");
```

### VFPFingerprintLoad

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintLoad(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Description :** charge une empreinte depuis un fichier.

**Paramètres :**

- `vfp` : pointeur vers la structure d'empreinte pour recevoir les données
- `filename` : chemin du fichier d'empreinte

**Exemple :**

```cpp
VFPFingerPrint fingerprint{};
VFPFingerprintLoad(&fingerprint, L"saved.vfpsig");

printf("Empreinte chargée :\n");
printf("  Durée : %lld ms\n", fingerprint.Duration);
printf("  Fichier original : %ls\n", fingerprint.OriginalFilename);
printf("  Résolution : %dx%d\n", fingerprint.Width, fingerprint.Height);
```

### VFPFingerprintSaveLegacy / VFPFingerprintLoadLegacy

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintSaveLegacy(
    VFPFingerPrint* vfp,
    wchar_t* filename);

extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintLoadLegacy(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Description :** enregistre et charge les empreintes au format hérité pour la rétrocompatibilité.

## Comparaison d'images { #image-comparison }

### VFPImageCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPImageCompare_Compare(
    BYTE* image1,
    int image1width,
    int image1height,
    BYTE* image2,
    int image2width,
    int image2height);
```

**Description :** compare deux images et retourne un score de similarité.

**Paramètres :**

- `image1` : données de la première image (format RGB24)
- `image1width` : largeur de la première image
- `image1height` : hauteur de la première image
- `image2` : données de la seconde image (format RGB24)
- `image2width` : largeur de la seconde image
- `image2height` : hauteur de la seconde image

**Retour :** score de similarité (0 à 100, les valeurs plus élevées indiquent plus de similarité)

**Exemple :**

```cpp
// Suppose que nous avons deux images RGB24 chargées
BYTE* img1 = LoadImage("image1.bmp", &width1, &height1);
BYTE* img2 = LoadImage("image2.bmp", &width2, &height2);

double similarity = VFPImageCompare_Compare(
    img1, width1, height1,
    img2, width2, height2
);

printf("Similarité d'images : %.2f%%\n", similarity);
```

## Exemples complets fonctionnels { #complete-working-examples }

### Exemple 1 : générer une empreinte de recherche (API de haut niveau)

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");

    VFPFingerprintSource src{};
    VFPFillSource(L"sample.mp4", &src);
    src.StartTime = 10000;   // démarrer à 10 s
    src.StopTime = 60000;    // arrêter à 60 s

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    printf("Empreinte : %dx%d, %.1fs, %d octets\n",
           fp.Width, fp.Height, fp.Duration / 1000.0, fp.DataSize);
    VFPFingerprintSave(&fp, L"sample.vfpsig");
    return 0;
}
```

### Exemple 2 : comparer deux vidéos

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");

    // Générer l'empreinte pour la vidéo 1
    VFPFingerprintSource src1{};
    VFPFillSource(L"video1.mp4", &src1);
    VFPFingerPrint fp1{};
    VFPCompare_GetFingerprintForVideoFile(src1, &fp1);

    // Générer l'empreinte pour la vidéo 2
    VFPFingerprintSource src2{};
    VFPFillSource(L"video2.mp4", &src2);
    VFPFingerPrint fp2{};
    VFPCompare_GetFingerprintForVideoFile(src2, &fp2);

    double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                     fp2.Data, fp2.DataSize, 10);
    printf("Différence : %.2f\n", diff);
    if (diff < 100)       printf("Très similaires\n");
    else if (diff < 500)  printf("Quelques similarités\n");
    else                  printf("Différentes\n");

    return 0;
}
```

### Exemple 3 : rechercher un fragment dans une vidéo plus longue

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");

    // Construire l'empreinte aiguille (fragment court)
    VFPFingerprintSource needleSrc{};
    VFPFillSource(L"fragment.mp4", &needleSrc);
    VFPFingerPrint needle{};
    VFPSearch_GetFingerprintForVideoFile(needleSrc, &needle);

    // Construire l'empreinte botte de foin (vidéo longue)
    VFPFingerprintSource haystackSrc{};
    VFPFillSource(L"broadcast.mp4", &haystackSrc);
    VFPFingerPrint haystack{};
    VFPSearch_GetFingerprintForVideoFile(haystackSrc, &haystack);

    // Rechercher toutes les occurrences
    double diff = 0;
    int pos = 0;
    const int needleSec = (int)(needle.Duration / 1000);

    while (pos < (int)(haystack.Duration / 1000))
    {
        pos = VFPSearch_Search2(&needle, 0, &haystack, pos, &diff, 300);
        if (pos == INT_MAX) break;

        printf("Correspondance à %d secondes (diff : %.2f)\n", pos, diff);
        pos += needleSec;
    }

    return 0;
}
```

## Prise en charge des plateformes { #platform-support }

### Windows

- **Architectures** : x86, x64
- **Compilateurs** : Visual Studio 2019 ou ultérieur, MinGW-w64
- **Bibliothèques** : VisioForge_VideoFingerprinting.dll (x86/x64)

### Linux

- **Architectures** : x64, ARM64
- **Compilateurs** : GCC 7+, Clang 6+
- **Dépendances** : GStreamer 1.18+

### macOS

- **Architectures** : x64, Apple Silicon (M1/M2)
- **Compilateurs** : Xcode 12+, Clang
- **Frameworks** : aucune dépendance supplémentaire

## Compilation et édition de liens { #building-and-linking }

### Visual Studio (Windows)

1. Ajouter le répertoire d'inclusion :

   ```
   $(SolutionDir)include
   ```

2. Ajouter le répertoire de bibliothèque :

   ```
   $(SolutionDir)lib
   ```

3. Lier les bibliothèques :
   - Pour x86 : `VisioForge_VideoFingerprinting.lib`
   - Pour x64 : `VisioForge_VideoFingerprinting_x64.lib`

4. Copier les DLL runtime vers le répertoire de sortie :
   - `VisioForge_VideoFingerprinting.dll` ou `VisioForge_VideoFingerprinting_x64.dll`
   - `VisioForge_FFMPEG_Source.dll` ou `VisioForge_FFMPEG_Source_x64.dll`

### CMake

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)

# Répertoires d'inclusion
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include)

# Répertoires de liaison
link_directories(${CMAKE_CURRENT_SOURCE_DIR}/lib)

# Ajouter l'exécutable
add_executable(vfp_example main.cpp)

# Lier les bibliothèques
if(WIN32)
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        target_link_libraries(vfp_example VisioForge_VideoFingerprinting_x64)
    else()
        target_link_libraries(vfp_example VisioForge_VideoFingerprinting)
    endif()
endif()

# Copier les DLL sous Windows
if(WIN32)
    add_custom_command(TARGET vfp_example POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${CMAKE_CURRENT_SOURCE_DIR}/redist/*.dll"
        $<TARGET_FILE_DIR:vfp_example>)
endif()
```

### Linux/macOS

```bash
# Compiler
g++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example

# Définir le chemin de bibliothèque (Linux)
export LD_LIBRARY_PATH=./lib:$LD_LIBRARY_PATH

# Définir le chemin de bibliothèque (macOS)
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Exécuter
./vfp_example
```

## Considérations de performance { #performance-considerations }

### Gestion de la mémoire

- Les données d'empreinte sont allouées en interne par le SDK
- Vérifiez toujours les valeurs de retour pour les erreurs
- Libérez correctement les ressources lorsque terminé

### Vitesse de traitement

- Les empreintes de recherche traitent à environ 30x temps réel sur les CPU modernes
- Les empreintes de comparaison traitent à environ 100x temps réel
- Les performances évoluent avec les cœurs CPU pour le traitement par lots

### Conseils d'optimisation

1. **Utiliser le type d'empreinte approprié** : Search pour la détection de fragments, Compare pour la comparaison de vidéos entières
2. **Définir des plages temporelles** : ne traiter que les segments requis pour réduire le temps de traitement
3. **Configurer les zones ignorées** : exclure les logos et bandeaux pour améliorer la précision
4. **Ajuster les seuils** : équilibrer entre faux positifs et faux négatifs
5. **Mettre en cache les empreintes** : enregistrer les empreintes générées pour éviter le retraitement

## Gestion des erreurs { #error-handling }

Toutes les fonctions qui retournent `wchar_t*` retournent NULL en cas de succès et un message d'erreur en cas d'échec :

```cpp
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
if (error != nullptr) {
    wprintf(L"Erreur survenue : %s\n", error);
    // Gérer l'erreur de manière appropriée
    return -1;
}
```

Scénarios d'erreur courants :

- Fichier introuvable ou inaccessible
- Format vidéo non pris en charge
- Mémoire insuffisante
- Clé de licence invalide
- Fichier d'empreinte corrompu

## Recommandations de seuil

### Opérations de recherche

- **100-200** : correspondance très stricte (copies exactes ou quasi exactes)
- **200-400** : correspondance normale (différences mineures d'encodage autorisées)
- **400-600** : correspondance lâche (différences significatives de qualité autorisées)
- **600+** : correspondance très lâche (peut produire des faux positifs)

### Opérations de comparaison

- **< 100** : les vidéos sont presque identiques
- **100-300** : les vidéos sont très similaires (probablement le même contenu)
- **300-500** : les vidéos ont des similarités significatives
- **500-1000** : les vidéos ont quelques similarités
- **> 1000** : les vidéos sont différentes

## Bonnes pratiques

1. **Toujours définir une clé de licence** avant d'appeler toute fonction du SDK
2. **Vérifier les valeurs de retour** pour toutes les opérations
3. **Utiliser les types d'empreinte appropriés** pour votre cas d'usage
4. **Enregistrer les empreintes** pour éviter le retraitement de gros fichiers vidéo
5. **Configurer les zones ignorées** pour le contenu avec superpositions ou logos
6. **Tester les valeurs de seuil** avec votre type de contenu spécifique
7. **Gérer les erreurs gracieusement** et fournir un retour significatif
8. **Libérer les ressources** lorsqu'elles ne sont plus nécessaires
9. **Utiliser le traitement par lots** pour plusieurs fichiers
10. **Surveiller l'utilisation mémoire** lors du traitement de nombreux fichiers

## Comparaison avec le SDK .NET

Le SDK C++ fournit les mêmes fonctionnalités principales que le SDK .NET avec ces différences :

### Avantages

- Performance native directe sans surcharge managée
- Empreinte mémoire plus faible
- Intégration plus facile avec les applications C++ existantes
- Aucune exigence de runtime .NET

### Différences

- Gestion mémoire manuelle requise
- Utilise des chaînes de caractères larges pour la compatibilité Windows
- API basée sur des fonctions au lieu d'orientée objet
- Accès direct aux fonctions de traitement bas niveau

### Parité des fonctionnalités

Les deux SDK prennent en charge :

- Génération d'empreintes Search et Compare
- Détection de fragments dans des vidéos plus longues
- Comparaison de similarité entre vidéos
- Comparaison d'images (Windows uniquement pour C++)
- Rognage personnalisé et zones ignorées
- Spécification de plage temporelle
- Opérations d'enregistrement/chargement d'empreintes

## Support et ressources

Pour un support et des ressources supplémentaires :

- **Exemples** : disponibles dans le paquet SDK sous le répertoire `Demos/`
- **Support** : <support@visioforge.com>
- **Licence** : <https://www.visioforge.com/>
