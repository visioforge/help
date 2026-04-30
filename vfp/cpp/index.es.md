---
title: Biblioteca C++ de Video Fingerprinting y documentación API
description: Implementación nativa C++ del SDK Video Fingerprinting con alto rendimiento y soporte multiplataforma para huellas de video robustas.
tags:
  - Video Fingerprinting SDK
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting
primary_api_classes:
  - VFPAnalyzer
  - VFPAnalyzerSettings
  - VideoProcessor
  - StoreFingerprint
  - StreamAnalyzer

---

# SDK de Huella Digital de Video para C++

!!! danger "Los ejemplos de código en esta página no coinciden con la API C pública — trate como pseudocódigo ilustrativo"

    La API de clase C++ documentada abajo (clase `VFPAnalyzer` con
    `Initialize` / `ProcessVideo`, `VFPAnalyzerSettings`, enum `VFPAnalyzerMode`,
    clase `VFPCompare` con `CompareVideos`, etc.) **no** existe en el SDK
    distribuido. El SDK real de Video Fingerprinting expone una **API plana
    estilo C** (`extern "C"`), no una jerarquía de clases.

    **API canónica real C (per `VisioForge_VFP.h`):**

    Flujo de fingerprint de búsqueda (por cuadro):

    ```c
    VFPSearchData searchData;
    VFPSearch_Init(&searchData);
    // Para cada cuadro decodificado:
    VFPSearch_Process(&searchData, frameData, width, height, stride, timestamp);
    // Después de todos los cuadros:
    VFPSearch_Build(&searchData, &resultFingerprint);
    // Buscar un fingerprint dentro de otro:
    VFPSearch_Search(&fpA, &fpB, duration, maxDifference, allowMultipleFragments,
                     &outResults, &outCount);
    ```

    Flujo de fingerprint de comparación (por cuadro):

    ```c
    VFPCompareData compareData;
    VFPCompare_Init(&compareData);
    // Para cada cuadro decodificado:
    VFPCompare_Process(&compareData, frameData, width, height, stride, timestamp);
    // Después de todos los cuadros:
    VFPCompare_Build(&compareData, &resultFingerprint);
    // Comparar dos fingerprints (devuelve diferencia; 0 = idénticos):
    int diff = VFPCompare_Compare(&fpA, &fpB, durationMs);
    ```

    Helpers convenientes de alto nivel como
    `VFPSearch_GetFingerprintForVideoFile`,
    `VFPCompare_GetFingerprintForVideoFile`, y `VFPFillSource` mostrados
    en otras partes de las páginas C++ **no** son exports reales — solo
    los primitivos de bajo nivel `_Init` / `_Process` / `_Build` /
    `_Search` / `_Compare` son distribuidos en `VisioForge_VFP.dll`. La
    aplicación host es responsable de decodificar cuadros de video (ej.
    vía FFmpeg o GStreamer) y alimentarlos cuadro-a-cuadro a `*_Process`.

    Catalogado como defectos #084-#090 en la auditoría. Una reescritura
    completa del conjunto de docs C++ está en cola. Para muestras
    funcionales de bajo nivel, vea `samples/` en la distribución del SDK.

## Descripción General

El SDK de Huella Digital de Video para C++ proporciona una implementación nativa con acceso directo a capacidades de alto rendimiento de análisis y huella digital de video. Este SDK es ideal para aplicaciones que requieren:

- Máximo rendimiento y mínima sobrecarga
- Integración directa con aplicaciones nativas
- Gestión de memoria personalizada
- Pipelines de procesamiento en tiempo real
- Despliegue en sistemas embebidos

## Características Principales

### Ventajas de Rendimiento

- **Rendimiento Nativo** - Acceso directo a memoria y algoritmos optimizados
- **Cero Sobrecarga** - Sin runtime administrado ni recolección de basura
- **Optimización SIMD** - Aprovecha capacidades de vectorización de CPU
- **Procesamiento Paralelo** - Generación de huella digital multi-hilo
- **Gestión de Memoria Personalizada** - Control detallado sobre asignación de memoria

### Soporte de Plataformas

- **Windows** - Visual Studio 2019+ (x64)
- **Linux** - GCC 9+ o Clang 10+
- **macOS** - Xcode 12+ (Intel y Apple Silicon)

## Documentación

### Primeros Pasos

- [Instalación y Configuración](getting-started.md) - Guía de configuración completa para todas las plataformas
- [Referencia de API](api.md) - Documentación completa de API C++

### Conceptos Principales

- [Entendiendo las Huellas Digitales de Video](../understanding-video-fingerprinting.md) - Cómo funciona la tecnología
- [Tipos de Huella Digital](../fingerprint-types.md) - Huellas de Comparación vs Búsqueda

### Ejemplos de Código

#### Generación Básica de Huella Digital

```cpp
#include <VFPAnalyzer.h>

// Crear instancia del analizador
auto analyzer = std::make_unique<VFPAnalyzer>();

// Configurar parámetros de análisis
VFPAnalyzerSettings settings;
settings.Mode = VFPAnalyzerMode::Search;
settings.FrameStep = 10;

// Establecer clave de licencia
analyzer->SetLicenseKey("su-clave-de-licencia");

// Procesar archivo de video
analyzer->StartAsync("video_entrada.mp4", "salida.vfp", settings);
```

#### Comparando Dos Videos

```cpp
#include <VFPCompare.h>

// Crear instancia de comparación
auto compare = std::make_unique<VFPCompare>();

// Establecer licencia
compare->SetLicenseKey("su-clave-de-licencia");

// Cargar huellas digitales
compare->LoadFingerprint("video1.vfp");
compare->LoadFingerprint("video2.vfp");

// Realizar comparación
auto result = compare->Compare();

// Verificar similitud
std::cout << "Similitud: " << result.Similarity << "%" << std::endl;
if (result.IsMatch) {
    std::cout << "¡Los videos coinciden!" << std::endl;
}
```

## Patrones de Integración

### Procesamiento con Eficiencia de Memoria

```cpp
// Procesar grandes colecciones de video con memoria mínima
class VideoProcessor {
public:
    void ProcessBatch(const std::vector<std::string>& videos) {
        VFPAnalyzer analyzer;
        analyzer.SetLicenseKey(m_licenseKey);
        
        for (const auto& video : videos) {
            // Procesar y almacenar/transmitir huella digital inmediatamente
            analyzer.StartAsync(video, 
                [this](const std::string& fingerprint) {
                    // Almacenar en base de datos o enviar a servidor
                    StoreFingerprint(fingerprint);
                });
        }
    }
};
```

### Análisis de Stream en Tiempo Real

```cpp
// Analizar streams de video en vivo
class StreamAnalyzer {
public:
    void AnalyzeStream(const std::string& streamUrl) {
        VFPAnalyzer analyzer;
        VFPAnalyzerSettings settings;
        settings.Mode = VFPAnalyzerMode::RealTime;
        settings.BufferSize = 30; // buffer de 30 segundos
        
        analyzer.SetLicenseKey(m_licenseKey);
        analyzer.StartStreamAnalysis(streamUrl, settings,
            [](const VFPSegment& segment) {
                // Procesar segmentos detectados en tiempo real
                ProcessSegment(segment);
            });
    }
};
```

## Soporte y Recursos

### Documentación

- [Referencia de API C++](api.md)
- [Guía de Primeros Pasos](getting-started.md)
- [Muestras de Código C++](samples/index.md)
- [Casos de Uso Comunes](../use-cases.md)

### Código de Ejemplo

- [Ejemplos Completos](samples/index.md) - Muestras de código funcionales
- Herramientas de línea de comandos en el paquete SDK `/samples/cpp/`

### Comunidad y Soporte

- [Issues de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/issues)
- [Portal de Soporte](https://support.visioforge.com)

## Registro de Licencia

Registre el SDK en su aplicación C++:

```cpp
// En su código de inicialización
VFPAnalyzer analyzer;
analyzer.SetLicenseKey("su-clave-de-licencia");

// O globalmente para todas las instancias
VFPLicense::SetGlobalKey("su-clave-de-licencia");
```

## Próximos Pasos

1. [Instalar y Configurar](getting-started.md) - Comenzar con el SDK C++
2. [Revisar la API](api.md) - Entender clases y métodos disponibles
3. [Explorar Ejemplos](getting-started.md#resumen-de-inicio-rapido) - Ver código funcional
