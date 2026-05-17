---
title: Bibliothèque C++ d'empreinte vidéo — API VisioForge VFP SDK
description: Implémentation native C++ du Video Fingerprinting SDK avec haute performance et prise en charge multiplateforme pour des empreintes vidéo robustes.
sidebar_label: Documentation SDK C++
order: 50
tags:
  - Video Fingerprinting SDK
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting
primary_api_classes:
  - VFPFingerprintSource
  - VFPFingerPrint
  - VFPSearch
  - VFPCompare

---

# Video Fingerprinting SDK pour C++

## Vue d'ensemble

Le Video Fingerprinting SDK pour C++ fournit une implémentation native avec un accès direct aux capacités d'analyse et d'empreinte vidéo haute performance. Ce SDK est idéal pour les applications nécessitant :

- Performance maximale et surcharge minimale
- Intégration directe avec des applications natives
- Gestion mémoire personnalisée
- Pipelines de traitement en temps réel
- Déploiement sur systèmes embarqués

## Fonctionnalités clés

### Avantages de performance

- **Performance native** — accès mémoire direct et algorithmes optimisés
- **Aucune surcharge** — pas de runtime managé ni de garbage collection
- **Optimisation SIMD** — exploite les capacités de vectorisation du CPU
- **Traitement parallèle** — génération d'empreintes multithread
- **Gestion mémoire personnalisée** — contrôle fin sur l'allocation mémoire

### Prise en charge des plateformes

- **Windows** — Visual Studio 2019+ (x64)
- **Linux** — GCC 9+ ou Clang 10+
- **macOS** — Xcode 12+ (Intel et Apple Silicon)

## Documentation

### Prise en main

- [Installation et configuration](getting-started.md) — guide de configuration complet pour toutes les plateformes
- [Référence de l'API](api.md) — documentation complète de l'API C++

### Concepts fondamentaux

- [Comprendre l'empreinte vidéo](../understanding-video-fingerprinting.md) — comment fonctionne la technologie
- [Types d'empreinte](../fingerprint-types.md) — empreintes Compare vs Search

### Exemples de code

#### Générer une empreinte de recherche (API de haut niveau)

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

VFPSetLicenseKey(L"your-license-key");

// Remplir les informations source
VFPFingerprintSource src{};
VFPFillSource(L"C:\\video.mp4", &src);
src.StartTime = 10000;   // démarrer à 10 s
src.StopTime = 60000;    // arrêter à 60 s

// Générer l'empreinte de recherche
VFPFingerPrint fp{};
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(src, &fp);
if (error == nullptr)
{
    printf("Empreinte : %dx%d, %.1fs, %d octets\n",
           fp.Width, fp.Height, fp.Duration / 1000.0, fp.DataSize);
    VFPFingerprintSave(&fp, L"output.vfpsig");
}
```

#### Comparer deux empreintes

```cpp
VFPFingerPrint fp1{}, fp2{};
VFPFingerprintLoad(&fp1, L"video1.vfpsig");
VFPFingerprintLoad(&fp2, L"video2.vfpsig");

double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                 fp2.Data, fp2.DataSize, 10);
printf("Différence : %.2f\n", diff);
```

#### Rechercher un fragment dans une vidéo plus longue

```cpp
VFPFingerPrint needle{}, haystack{};
VFPFingerprintLoad(&needle, L"fragment.vfpsig");
VFPFingerprintLoad(&haystack, L"full.vfpsig");

double diff = 0;
int pos = VFPSearch_Search2(&needle, 0, &haystack, 0, &diff, 300);
if (pos != INT_MAX)
    printf("Trouvé à %d secondes (diff : %.2f)\n", pos, diff);
```

#### API bas niveau image par image (pour flux en direct / décodeurs personnalisés)

```cpp
// Allouer un accumulateur pour ~60 s de vidéo à 30 ips
void* pData = VFPSearch_Init2(30 * 60);

while (hasMoreFrames)
{
    // Décoder l'image dans un tampon RGB...
    VFPSearch_Process(rgbData, width, height, stride, timestampSec, pData);
}

int len = 0;
char* data = VFPSearch_Build(&len, pData);

// Utiliser data/len comme VFPFingerPrint.Data / .DataSize
VFPFingerPrint fp{};
fp.Data = data;
fp.DataSize = len;
// ... définir Duration, Width, Height, FrameRate manuellement ...

VFPSearch_Clear(pData);
```

**Important :** le SDK fournit uniquement les primitives bas niveau `_Init` / `_Process` / `_Build` / `_Search` / `_Compare`. L'application hôte est responsable du décodage des images vidéo (FFmpeg, GStreamer, DirectShow, etc.) et de les alimenter image par image à `*_Process`.

## Modèles d'intégration { #integration-patterns }

### Traitement par lots

```cpp
void ProcessBatch(const std::vector<std::wstring>& videos)
{
    for (const auto& path : videos)
    {
        VFPFingerprintSource src{};
        VFPFillSource(path.c_str(), &src);

        VFPFingerPrint fp{};
        VFPSearch_GetFingerprintForVideoFile(src, &fp);
        // Stocker fp en base de données...
    }
}
```

### Analyse de flux en temps réel (API bas niveau)

```cpp
void* pData = VFPSearch_Init2(30 * 60); // 30 ips, tampon de 60 s

void OnFrame(unsigned char* rgb, int w, int h, int stride, double timestampSec)
{
    VFPSearch_Process(rgb, w, h, stride, timestampSec, pData);
}

void OnStreamEnd()
{
    int len;
    char* data = VFPSearch_Build(&len, pData);
    // Comparer avec les empreintes connues...
    VFPSearch_Clear(pData);
}
```

## Support et ressources

### Documentation

- [Référence de l'API C++](api.md)
- [Guide de prise en main](getting-started.md)
- [Exemples de code C++](samples/index.md)
- [Cas d'usage courants](../use-cases.md)

### Code d'exemple

- [Exemples complets](samples/index.md) — exemples de code fonctionnels
- Outils en ligne de commande dans le paquet SDK `/samples/cpp/`

### Communauté et support

- [Issues GitHub](https://github.com/visioforge/.Net-SDK-s-samples/issues)
- [Portail de support](https://support.visioforge.com)

## Enregistrement de licence

```cpp
#include <VisioForge_VFP.h>

VFPSetLicenseKey(L"your-license-key");
// ou pour les caractères étroits : VFPSetLicenseKeyA("your-license-key");
```

## Étapes suivantes

1. [Installer et configurer](getting-started.md) — démarrer avec le SDK C++
2. [Examiner l'API](api.md) — comprendre les classes et méthodes disponibles
3. [Explorer les exemples](getting-started.md#your-first-application) — voir du code fonctionnel
