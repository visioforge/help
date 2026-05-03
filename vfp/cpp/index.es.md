---
title: Biblioteca C++ de Video Fingerprinting y Documentación API
description: Implementación nativa C++ del SDK Video Fingerprinting con alto rendimiento y soporte multiplataforma para huellas de video robustas.
sidebar_label: Documentación SDK C++
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

# Video Fingerprinting SDK para C++

## Descripción General

El SDK Video Fingerprinting para C++ proporciona una implementación nativa con acceso directo a capacidades de análisis de video y generación de huellas digitales de alto rendimiento. Este SDK es ideal para aplicaciones que requieren:

- Máximo rendimiento y sobrecarga mínima
- Integración directa con aplicaciones nativas
- Gestión de memoria personalizada
- Pipelines de procesamiento en tiempo real
- Despliegue en sistemas embebidos

## Características Principales

### Ventajas de Rendimiento

- **Rendimiento Nativo** - Acceso directo a memoria y algoritmos optimizados
- **Sobrecarga Cero** - Sin runtime gestionado ni recolección de basura
- **Optimización SIMD** - Aprovecha capacidades de vectorización de CPU
- **Procesamiento Paralelo** - Generación de huellas multi-hilo
- **Gestión de Memoria Personalizada** - Control detallado sobre asignación de memoria

### Soporte de Plataformas

- **Windows** - Visual Studio 2019+ (x64)
- **Linux** - GCC 9+ o Clang 10+
- **macOS** - Xcode 12+ (Intel y Apple Silicon)

## Documentación

### Primeros Pasos

- [Instalación y Configuración](getting-started.md) - Guía completa para todas las plataformas
- [Referencia API](api.md) - Documentación completa de la API C++

### Conceptos Básicos

- [Entendiendo Video Fingerprinting](../understanding-video-fingerprinting.md) - Cómo funciona la tecnología
- [Tipos de Huellas](../fingerprint-types.md) - Huellas de comparación vs búsqueda

### Ejemplos de Código

#### Generar Huella de Búsqueda (API de alto nivel)

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

VFPSetLicenseKey(L"su-clave-de-licencia");

// Configurar origen
VFPFingerprintSource src{};
VFPFillSource(L"C:\\video.mp4", &src);
src.StartTime = 10000;   // empezar a los 10s
src.StopTime = 60000;    // parar a los 60s

// Generar huella de búsqueda
VFPFingerPrint fp{};
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(src, &fp);
if (error == nullptr)
{
    printf("Huella: %dx%d, %.1fs, %d bytes\n",
           fp.Width, fp.Height, fp.Duration / 1000.0, fp.DataSize);
    VFPFingerprintSave(&fp, L"salida.vfpsig");
}
```

#### Comparar Dos Huellas

```cpp
VFPFingerPrint fp1{}, fp2{};
VFPFingerprintLoad(&fp1, L"video1.vfpsig");
VFPFingerprintLoad(&fp2, L"video2.vfpsig");

double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                 fp2.Data, fp2.DataSize, 10);
printf("Diferencia: %.2f\n", diff);
```

#### Buscar Fragmento en Video Más Largo

```cpp
VFPFingerPrint needle{}, haystack{};
VFPFingerprintLoad(&needle, L"fragmento.vfpsig");
VFPFingerprintLoad(&haystack, L"completo.vfpsig");

double diff = 0;
int pos = VFPSearch_Search2(&needle, 0, &haystack, 0, &diff, 300);
if (pos != INT_MAX)
    printf("Encontrado en %d segundos (diff: %.2f)\n", pos, diff);
```

#### API de Bajo Nivel por Fotograma (para streams en vivo / decodificadores personalizados)

```cpp
// Asignar acumulador para ~60s de video a 30fps
void* pData = VFPSearch_Init2(30 * 60);

while (hayMasFotogramas)
{
    // Decodificar fotograma a buffer RGB...
    VFPSearch_Process(datosRGB, ancho, alto, stride, timestampSeg, pData);
}

int len = 0;
char* data = VFPSearch_Build(&len, pData);

// Usar data/len como VFPFingerPrint.Data / .DataSize
VFPFingerPrint fp{};
fp.Data = data;
fp.DataSize = len;
// ... establecer Duration, Width, Height, FrameRate manualmente ...

VFPSearch_Clear(pData);
```

## Patrones de Integración

### Procesamiento por Lotes

```cpp
void ProcessBatch(const std::vector<std::wstring>& videos)
{
    for (const auto& path : videos)
    {
        VFPFingerprintSource src{};
        VFPFillSource(path.c_str(), &src);

        VFPFingerPrint fp{};
        VFPSearch_GetFingerprintForVideoFile(src, &fp);
        // Almacenar fp en base de datos...
    }
}
```

### Análisis de Stream en Tiempo Real (API de bajo nivel)

```cpp
void* pData = VFPSearch_Init2(30 * 60); // 30fps, búfer de 60s

void OnFrame(unsigned char* rgb, int w, int h, int stride, double timestampSec)
{
    VFPSearch_Process(rgb, w, h, stride, timestampSec, pData);
}

void OnStreamEnd()
{
    int len;
    char* data = VFPSearch_Build(&len, pData);
    // Comparar con huellas conocidas...
    VFPSearch_Clear(pData);
}
```

## Soporte y Recursos

### Documentación

- [Referencia API C++](api.md)
- [Guía de Inicio](getting-started.md)
- [Ejemplos de Código C++](samples/index.md)
- [Casos de Uso Comunes](../use-cases.md)

### Código de Ejemplo

- [Ejemplos Completos](samples/index.md) - Ejemplos de código funcionales
- Herramientas de línea de comandos en el paquete SDK `/samples/cpp/`

### Comunidad y Soporte

- [GitHub Issues](https://github.com/visioforge/.Net-SDK-s-samples/issues)
- [Portal de Soporte](https://support.visioforge.com)

## Registro de Licencia

```cpp
#include <VisioForge_VFP.h>

VFPSetLicenseKey(L"su-clave-de-licencia");
// o para char estrecho: VFPSetLicenseKeyA("su-clave-de-licencia");
```
