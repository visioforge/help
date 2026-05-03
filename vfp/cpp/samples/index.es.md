---
title: Ejemplos de Código C++ del SDK Video Fingerprinting
description: Ejemplos de código C++ para generar, comparar y buscar huellas de video con el SDK VisioForge. Incluye muestras de línea de comandos.
sidebar_label: Ejemplos C++
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

---

# Ejemplos de Código C++ del SDK Video Fingerprinting

## Ejemplos Disponibles

El SDK C++ incluye ejemplos de línea de comandos. Estos se encuentran en el paquete SDK en `/samples/cpp/`.

### Ejemplos de Funcionalidad Principal

#### Generar Huellas

```cpp
// vfp_gen.cpp - Generar huella desde archivo de video
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerprintSource src{};
    VFPFillSource(L"input.mp4", &src);

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    VFPFingerprintSave(&fp, L"output.vfpsig");
    printf("Huella guardada: %d bytes\n", fp.DataSize);
    return 0;
}
```

#### Comparar Videos

```cpp
// vfp_compare.cpp - Comparar dos huellas
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerPrint fp1{}, fp2{};
    VFPFingerprintLoad(&fp1, L"video1.vfpsig");
    VFPFingerprintLoad(&fp2, L"video2.vfpsig");

    double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                     fp2.Data, fp2.DataSize, 10);

    printf("Diferencia: %.2f\n", diff);
    if (diff < 100)       printf("Muy similar\n");
    else if (diff < 500)  printf("Alguna similitud\n");
    else                  printf("Diferente\n");

    return 0;
}
```

#### Buscar Fragmentos

```cpp
// vfp_search.cpp - Buscar fragmento en video más largo
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerPrint needle{}, haystack{};
    VFPFingerprintLoad(&needle, L"fragmento.vfpsig");
    VFPFingerprintLoad(&haystack, L"video_completo.vfpsig");

    double diff = 0;
    int pos = VFPSearch_Search2(&needle, 0, &haystack, 0, &diff, 300);
    if (pos != INT_MAX)
        printf("Encontrado en %d segundos (diff: %.2f)\n", pos, diff);

    return 0;
}
```

### Compilar los Ejemplos

#### Windows (Visual Studio)

```bash
# Abrir la solución de Visual Studio
samples/cpp/VFPSamples.sln

# O compilar desde línea de comandos
msbuild VFPSamples.sln /p:Configuration=Release /p:Platform=x64
```

#### Linux/macOS (CMake)

```bash
cd samples/cpp
mkdir build && cd build
cmake ..
make
```

## Ejemplos de Integración

### Procesamiento Multi-hilo

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
        
        // Llenar cola
        for (const auto& video : videos) {
            videoQueue.push(video);
        }
        
        // Iniciar hilos de trabajo
        for (int i = 0; i < numThreads; i++) {
            workers.emplace_back(&FingerprintProcessor::Worker, this);
        }
        
        // Esperar finalización
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
        // Almacenar fp.Data / fp.DataSize
    }
};
```

### Integración con Base de Datos

```cpp
// Ejemplo usando SQLite para almacenamiento de huellas
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

## Comparación con Ejemplos .NET

| Característica | Implementación C++ | Implementación .NET |
|---------|-------------------|---------------------|
| **Aplicaciones GUI** | Ejemplos Qt/MFC disponibles por separado | Ejemplos WPF/WinForms disponibles |
| **Herramientas CLI** | Incluidas en el SDK | [Herramientas completas](../../dotnet/samples/index.md) |
| **Integración BD** | Implementación manual | Soporte integrado MongoDB |
| **Callbacks de Progreso** | Punteros a función | Eventos y delegados |
| **Manejo de Errores** | Códigos de retorno | Excepciones |

## Consejos de Optimización

1. **Use pasos de fotograma apropiados** - Valores más altos procesan más rápido pero pueden omitir segmentos cortos
2. **Active multi-hilado** - Procese múltiples videos en paralelo
3. **Reutilice instancias del analizador** - Evite sobrecarga de inicialización
4. **Operaciones por lotes** - Procese múltiples archivos antes de limpiar
5. **Use rutas nativas** - Evite conversiones de cadena

## Recursos Adicionales

- [Referencia API C++](../api.md) - Documentación completa de la API
- [Guía de Inicio](../getting-started.md) - Configuración e instalación
- [Ejemplos .NET](../../dotnet/samples/index.md) - Para comparación con código gestionado

## Soporte

Para preguntas sobre estos ejemplos:

- Consulte el [FAQ](../../faq.md) para problemas comunes
- Visite nuestro [Portal de Soporte](https://support.visioforge.com)
- Únase a nuestra [Comunidad Discord](https://discord.com/invite/yvXUG56WCH)
