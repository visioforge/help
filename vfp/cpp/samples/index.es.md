---
title: Muestras de Código C++: SDK de Huella Digital de Video
description: Ejemplos C++ nativo y línea de comandos para generación, comparación y búsqueda de huellas de video con implementación del SDK VisioForge.
---

# Muestras de Código C++ del SDK de Huella Digital de Video

## Muestras Disponibles

El SDK C++ incluye muestras de línea de comandos que demuestran funcionalidad principal. Estas muestras están incluidas en el paquete SDK bajo el directorio `/samples/cpp/`.

### Ejemplos de Funcionalidad Principal

#### Generar Huellas Digitales

```cpp
// vfp_generate.cpp - Generar huellas digitales de archivos de video
#include <VisioForge_VFP.h>

int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Uso: vfp_generate <video_entrada> <huella_salida>" << std::endl;
        return 1;
    }
    
    // Establecer licencia
    VFPSetLicenseKey(L"TRIAL");
    
    // Generar huella digital
    VFP_SearchFingerprintGenerateSettings settings;
    settings.Mode = VFP_Mode::Search;
    settings.FrameStep = 10;
    
    auto result = VFPSearchFingerprintGenerate(
        argv[1],  // Video de entrada
        argv[2],  // Huella de salida
        &settings,
        nullptr   // Callback de progreso
    );
    
    return result == VFP_ErrorCode::Ok ? 0 : 1;
}
```

#### Comparar Videos

```cpp
// vfp_compare.cpp - Comparar dos huellas digitales de video
#include <VisioForge_VFP.h>

int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Uso: vfp_compare <huella1> <huella2>" << std::endl;
        return 1;
    }
    
    VFPSetLicenseKey(L"TRIAL");
    
    VFP_CompareResult result;
    auto status = VFPCompareFingerprints(
        argv[1],
        argv[2],
        &result
    );
    
    if (status == VFP_ErrorCode::Ok) {
        std::cout << "Similitud: " << result.Similarity << "%" << std::endl;
        std::cout << "Coincidencia: " << (result.IsMatch ? "Sí" : "No") << std::endl;
    }
    
    return status == VFP_ErrorCode::Ok ? 0 : 1;
}
```

#### Buscar Fragmentos

```cpp
// vfp_search.cpp - Buscar fragmentos de video
#include <VisioForge_VFP.h>

int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Uso: vfp_search <huella_fuente> <huella_destino>" << std::endl;
        return 1;
    }
    
    VFPSetLicenseKey(L"TRIAL");
    
    VFP_SearchResult* results = nullptr;
    int count = 0;
    
    auto status = VFPSearchFingerprint(
        argv[1],  // Fuente (fragmento)
        argv[2],  // Destino (video completo)
        &results,
        &count
    );
    
    if (status == VFP_ErrorCode::Ok) {
        std::cout << "Encontradas " << count << " coincidencias" << std::endl;
        for (int i = 0; i < count; i++) {
            std::cout << "Coincidencia " << i << ": Posición " 
                     << results[i].Position << "ms, Similitud " 
                     << results[i].Similarity << "%" << std::endl;
        }
        VFPFreeSearchResults(results);
    }
    
    return status == VFP_ErrorCode::Ok ? 0 : 1;
}
```

### Compilando las Muestras

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
    
    void ProcessVideo(const std::string& video) {
        VFP_SearchFingerprintGenerateSettings settings;
        settings.Mode = VFP_Mode::Search;
        
        std::string output = video + ".vfp";
        VFPSearchFingerprintGenerate(
            video.c_str(),
            output.c_str(),
            &settings,
            nullptr
        );
    }
};
```

### Integración de Base de Datos

```cpp
// Ejemplo usando SQLite para almacenamiento de huellas digitales
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

## Comparación con Muestras .NET

| Característica | Implementación C++ | Implementación .NET |
|----------------|-------------------|---------------------|
| **Aplicaciones GUI** | Ejemplos Qt/MFC disponibles por separado | Muestras WPF/WinForms disponibles |
| **Herramientas CLI** | Incluidas en SDK | [Herramientas completas](../../dotnet/samples/index.md) |
| **Integración de DB** | Implementación manual | Soporte MongoDB incorporado |
| **Callbacks de Progreso** | Punteros a función | Eventos y delegados |
| **Manejo de Errores** | Códigos de retorno | Excepciones |

## Consejos de Optimización de Rendimiento

1. **Use pasos de frame apropiados** - Valores más altos procesan más rápido pero pueden perder segmentos cortos
2. **Habilite multi-threading** - Procese múltiples videos en paralelo
3. **Reutilice instancias del analizador** - Evite sobrecarga de inicialización
4. **Operaciones por lotes** - Procese múltiples archivos antes de limpiar
5. **Use rutas nativas** - Evite conversiones de cadenas

## Recursos Adicionales

- [Referencia de API C++](../api.md) - Documentación completa de API
- [Guía de Primeros Pasos](../getting-started.md) - Configuración y setup
- [Muestras .NET](../../dotnet/samples/index.md) - Para comparación con código administrado

## Soporte

Para preguntas sobre estas muestras:

- Consulte las [Preguntas Frecuentes](../../faq.md) para problemas comunes
- Visite nuestro [Portal de Soporte](https://support.visioforge.com)
- Únase a nuestra [Comunidad Discord](https://discord.com/invite/yvXUG56WCH)
