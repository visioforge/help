---
title: Exemples de code C++ du VisioForge Video Fingerprinting SDK
description: Exemples de code C++ natifs et exemples en ligne de commande pour génération, comparaison et recherche d'empreintes vidéo avec le SDK VisioForge.
sidebar_label: Exemples C++
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

# Exemples de code C++ du Video Fingerprinting SDK

## Exemples disponibles

Le SDK C++ inclut des exemples en ligne de commande démontrant les fonctionnalités principales. Ces exemples sont inclus dans le paquet SDK sous le répertoire `/samples/cpp/`.

### Exemples de fonctionnalités principales

#### Générer des empreintes

```cpp
// vfp_gen.cpp - Générer une empreinte à partir d'un fichier vidéo
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerprintSource src{};
    VFPFillSource(L"input.mp4", &src);

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    VFPFingerprintSave(&fp, L"output.vfpsig");
    printf("Empreinte enregistrée : %d octets\n", fp.DataSize);
    return 0;
}
```

#### Comparer des vidéos

```cpp
// vfp_compare.cpp - Comparer deux empreintes
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerPrint fp1{}, fp2{};
    VFPFingerprintLoad(&fp1, L"video1.vfpsig");
    VFPFingerprintLoad(&fp2, L"video2.vfpsig");

    double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                     fp2.Data, fp2.DataSize, 10);

    printf("Différence : %.2f\n", diff);
    if (diff < 100)       printf("Très similaires\n");
    else if (diff < 500)  printf("Quelques similarités\n");
    else                  printf("Différentes\n");

    return 0;
}
```

#### Rechercher des fragments

```cpp
// vfp_search.cpp - Rechercher un fragment dans une vidéo plus longue
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerPrint needle{}, haystack{};
    VFPFingerprintLoad(&needle, L"fragment.vfpsig");
    VFPFingerprintLoad(&haystack, L"full_video.vfpsig");

    double diff = 0;
    int pos = VFPSearch_Search2(&needle, 0, &haystack, 0, &diff, 300);
    if (pos != INT_MAX)
        printf("Trouvé à %d secondes (diff : %.2f)\n", pos, diff);

    return 0;
}
               results[i].startMs, results[i].endMs, results[i].difference);

    return 0;
}
```

### Compilation des exemples

#### Windows (Visual Studio)

```bash
# Ouvrir la solution Visual Studio
samples/cpp/VFPSamples.sln

# Ou compiler en ligne de commande
msbuild VFPSamples.sln /p:Configuration=Release /p:Platform=x64
```

#### Linux/macOS (CMake)

```bash
cd samples/cpp
mkdir build && cd build
cmake ..
make
```

## Exemples d'intégration

### Traitement multithread

```cpp
#include <thread>
#include <vector>
#include <queue>
#include <mutex>

class FingerprintProcessor {
private:
    std::queue<std::string> videoQueue;
    std::mutex queueMutex;
    
public:
    void ProcessVideos(const std::vector<std::string>& videos) {
        const int numThreads = std::thread::hardware_concurrency();
        std::vector<std::thread> workers;
        
        // Remplir la file
        for (const auto& video : videos) {
            videoQueue.push(video);
        }
        
        // Démarrer les threads worker
        for (int i = 0; i < numThreads; i++) {
            workers.emplace_back(&FingerprintProcessor::Worker, this);
        }
        
        // Attendre la fin
        for (auto& worker : workers) {
            worker.join();
        }
    }
    
private:
    void Worker() {
        while (true) {
            std::string video;
            {
                std::lock_guard<std::mutex> lock(queueMutex);
                if (videoQueue.empty()) break;
                video = videoQueue.front();
                videoQueue.pop();
            }
            
            ProcessVideo(video);
        }
    }
    
    void ProcessVideo(const std::string& video)
    {
        VFPFingerprintSource src{};
        std::wstring wpath(video.begin(), video.end());
        VFPFillSource(wpath.c_str(), &src);

        VFPFingerPrint fp{};
        VFPSearch_GetFingerprintForVideoFile(src, &fp);
        // Stocker fp.Data / fp.DataSize
    }
};
```

### Intégration de base de données

```cpp
// Exemple utilisant SQLite pour le stockage des empreintes
#include <sqlite3.h>

class FingerprintDatabase {
private:
    sqlite3* db;
    
public:
    void StoreFingerprint(const std::string& videoPath, 
                         const std::vector<uint8_t>& fingerprint) {
        const char* sql = "INSERT INTO fingerprints (path, data) VALUES (?, ?)";
        sqlite3_stmt* stmt;
        sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
        
        sqlite3_bind_text(stmt, 1, videoPath.c_str(), -1, SQLITE_STATIC);
        sqlite3_bind_blob(stmt, 2, fingerprint.data(), 
                         fingerprint.size(), SQLITE_STATIC);
        
        sqlite3_step(stmt);
        sqlite3_finalize(stmt);
    }
    
    std::vector<uint8_t> LoadFingerprint(const std::string& videoPath) {
        const char* sql = "SELECT data FROM fingerprints WHERE path = ?";
        sqlite3_stmt* stmt;
        sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
        
        sqlite3_bind_text(stmt, 1, videoPath.c_str(), -1, SQLITE_STATIC);
        
        std::vector<uint8_t> fingerprint;
        if (sqlite3_step(stmt) == SQLITE_ROW) {
            const void* data = sqlite3_column_blob(stmt, 0);
            int size = sqlite3_column_bytes(stmt, 0);
            
            fingerprint.resize(size);
            memcpy(fingerprint.data(), data, size);
        }
        
        sqlite3_finalize(stmt);
        return fingerprint;
    }
};
```

## Comparaison avec les exemples .NET

| Fonctionnalité | Implémentation C++ | Implémentation .NET |
|---------|-------------------|---------------------|
| **Applications GUI** | Exemples Qt/MFC disponibles séparément | Exemples WPF/WinForms disponibles |
| **Outils CLI** | Inclus dans le SDK | [Outils complets](../../dotnet/samples/index.md) |
| **Intégration base de données** | Implémentation manuelle | Prise en charge MongoDB intégrée |
| **Callbacks de progression** | Pointeurs de fonction | Événements et délégués |
| **Gestion des erreurs** | Codes de retour | Exceptions |

## Conseils d'optimisation de performance

1. **Utiliser des pas d'images appropriés** — des valeurs plus élevées traitent plus vite mais peuvent manquer les segments courts
2. **Activer le multithreading** — traiter plusieurs vidéos en parallèle
3. **Réutiliser les instances d'analyseur** — éviter la surcharge d'initialisation
4. **Opérations par lots** — traiter plusieurs fichiers avant nettoyage
5. **Utiliser des chemins natifs** — éviter les conversions de chaînes

## Ressources supplémentaires

- [Référence de l'API C++](../api.md) — documentation complète de l'API
- [Guide de prise en main](../getting-started.md) — installation et configuration
- [Exemples .NET](../../dotnet/samples/index.md) — pour comparaison avec le code managé

## Support

Pour les questions sur ces exemples :

- Consultez la [FAQ](../../faq.md) pour les problèmes courants
- Visitez notre [portail de support](https://support.visioforge.com)
- Rejoignez notre [communauté Discord](https://discord.com/invite/yvXUG56WCH)
