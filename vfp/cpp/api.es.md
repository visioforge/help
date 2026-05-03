---
title: API C++ de Video Fingerprinting para Búsqueda y Comparación
description: Documentación completa de la API del SDK Video Fingerprinting de VisioForge para C++: generar, comparar y buscar huellas de video con ejemplos.
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

# Documentación de la API C++ del SDK Video Fingerprinting

## Descripción General

El SDK Video Fingerprinting para C++ de VisioForge proporciona una librería nativa de alto rendimiento para identificación, comparación y búsqueda de contenido de video. Permite a las aplicaciones:

- Generar huellas únicas desde archivos de video para identificación de contenido
- Comparar videos para determinar similitud y detectar duplicados
- Buscar fragmentos de video dentro de videos más largos (ej. encontrar anuncios, introducciones o escenas específicas)
- Comparar imágenes individuales para detección de similitud
- Procesar fotogramas de video directamente para generar huellas desde streams o contenido generado

El SDK C++ ofrece un rendimiento óptimo para aplicaciones de alto rendimiento y puede integrarse en aplicaciones C++ existentes o usarse mediante P/Invoke desde otros lenguajes.

> **Documentación Relacionada:**
>
> - [Referencia API .NET](../dotnet/api.md) - Para desarrolladores de código gestionado
> - [Entendiendo Video Fingerprinting](../understanding-video-fingerprinting.md) - Conceptos básicos
> - [Tipos de Huellas](../fingerprint-types.md) - Modos Comparar vs Buscar

## Tabla de Contenidos

- [Archivos de Cabecera](#archivos-de-cabecera)
- [Gestión de Licencia](#gestion-de-licencia)
- [Tipos y Estructuras Principales](#tipos-y-estructuras-principales)
- [Funciones de Búsqueda](#funciones-de-busqueda)
- [Funciones de Comparación](#funciones-de-comparacion)
- [Funciones de Utilidad](#funciones-de-utilidad)
- [Comparación de Imágenes](#comparacion-de-imagenes)
- [Ejemplos Completos](#ejemplos-completos-funcionales)
- [Soporte de Plataformas](#soporte-de-plataformas)
- [Compilación y Enlazado](#compilacion-y-enlazado)
- [Consideraciones de Rendimiento](#consideraciones-de-rendimiento)
- [Manejo de Errores](#manejo-de-errores)

## Archivos de Cabecera

El SDK proporciona dos archivos de cabecera principales:

### VisioForge_VFP.h

Cabecera principal de la API con todas las declaraciones de funciones y exports.

### VisioForge_VFP_Types.h

Definiciones de tipos y estructuras de datos usadas por el SDK.

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>
```

## Gestión de Licencia

### VFPSetLicenseKey

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKey(wchar_t* licenseKey);
```

**Descripción:** Establece la clave de licencia para el SDK. Debe llamarse antes de usar cualquier función de fingerprinting.

**Parámetros:**

- `licenseKey` (wchar_t*): Su clave de licencia VisioForge como cadena de caracteres anchos

**Ejemplo:**

```cpp
// Establecer clave de licencia al iniciar la aplicación
VFPSetLicenseKey(L"SU-CLAVE-DE-LICENCIA");

// Para modo trial
VFPSetLicenseKey(L"TRIAL");
```

### VFPSetLicenseKeyA

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKeyA(char* licenseKey);
```

**Descripción:** Establece la clave de licencia usando cadena ANSI (alternativa a VFPSetLicenseKey).

**Parámetros:**

- `licenseKey` (char*): Su clave de licencia VisioForge como cadena ANSI

**Ejemplo:**

```cpp
// Establecer clave usando cadena ANSI
VFPSetLicenseKeyA("SU-CLAVE-DE-LICENCIA");
```

## Tipos y Estructuras Principales

### VFPFingerprintSource

```cpp
struct VFPFingerprintSource
{
    wchar_t Filename[256];     // Video file path
    INT64 StartTime;            // Start time in milliseconds
    INT64 StopTime;             // Stop time in milliseconds
    RECT CustomCropSize;        // Custom crop area
    SIZE CustomResolution;      // Custom resolution for processing
    RECT IgnoredAreas[10];      // Areas to ignore (e.g., logos, tickers)
    INT64 OriginalDuration;     // Original file duration
};
```

**Descripción:** Estructura de configuración para generación de huellas desde archivos de video.

**Campos:**

- `Filename`: Ruta al archivo de video (máximo 256 caracteres)
- `StartTime`: Posición inicial en milisegundos (0 para el inicio)
- `StopTime`: Posición final en milisegundos (0 para final del archivo)
- `CustomCropSize`: Rectángulo de recorte opcional (izquierda, arriba, derecha, abajo)
- `CustomResolution`: Resolución personalizada opcional para procesamiento
- `IgnoredAreas`: Hasta 10 áreas rectangulares a ignorar durante la generación
- `OriginalDuration`: Duración del archivo original en milisegundos

**Ejemplo:**

```cpp
VFPFingerprintSource source{};
wcscpy_s(source.Filename, L"C:\\Videos\\sample.mp4");
source.StartTime = 10000;  // Empezar a los 10 segundos
source.StopTime = 60000;   // Parar a los 60 segundos

// Ignorar logo en esquina superior derecha
source.IgnoredAreas[0] = {1800, 0, 1920, 100};
```

### VFPFingerPrint

```cpp
struct VFPFingerPrint
{
    char* Data;                    // Fingerprint data
    INT32 DataSize;                // Size of fingerprint data
    INT64 Duration;                // Duration in milliseconds
    GUID ID;                       // Unique identifier
    wchar_t OriginalFilename[256]; // Original file name
    INT64 OriginalDuration;        // Original file duration
    wchar_t Tag[100];              // Optional tag
    INT32 Width;                   // Source video width
    INT32 Height;                  // Source video height
    double FrameRate;              // Frame rate
};
```

**Descripción:** Estructura que contiene los datos de la huella generada y sus metadatos.

**Campos:**

- `Data`: Datos binarios de la huella
- `DataSize`: Tamaño de los datos de la huella en bytes
- `Duration`: Duración del contenido analizado en milisegundos
- `ID`: Identificador GUID único para la huella
- `OriginalFilename`: Ruta al archivo de video original
- `OriginalDuration`: Duración del archivo original
- `Tag`: Etiqueta opcional definida por el usuario (hasta 100 caracteres)
- `Width`: Ancho del video fuente
- `Height`: Alto del video fuente
- `FrameRate`: Velocidad de fotogramas del video fuente

## Funciones de Búsqueda

La API de búsqueda proporciona tanto una API de alto nivel (generar huella desde archivo de video) como una API de bajo nivel por fotograma (para streams en vivo / decodificadores personalizados).

### API de Alto Nivel

#### VFPSearch_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Genera una huella de búsqueda directamente desde un archivo de video. Retorna `NULL` en caso de éxito, o un mensaje de error.

#### VFPSearch_GetFingerprintForVideoFileAndSave

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFileAndSave(
    wchar_t* sourceFilename,
    wchar_t* destFilename);
```

Genera y guarda una huella en una sola llamada.

#### VFPSearch_SearchOneSignatureFileInAnother

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_SearchOneSignatureFileInAnother(
    wchar_t* file1, wchar_t* file2,
    LONG threshold, LONG* position);
```

Busca un archivo de firma dentro de otro directamente desde disco.

#### VFPSearch_Search2

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search2(
    VFPFingerPrint* vfp1, int iSkip1,
    VFPFingerPrint* vfp2, int iSkip2,
    double* pDiff, int maxDiff);
```

Busca `vfp1` dentro de `vfp2`. Retorna la posición en segundos, o `INT_MAX` si no se encuentra.

### API de Bajo Nivel por Fotograma

#### VFPSearch_Init

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_Init(int count, void* pDataTmp);
```

Inicializa un acumulador por fotograma. `count` es el número esperado de fotogramas.

#### VFPSearch_Init2

```cpp
extern "C" VFP_EXPORT void* VFP_EXPORT_CALL VFPSearch_Init2(int count);
```

Asigna y retorna un nuevo acumulador. Use `VFPSearch_Clear` para liberarlo.

#### VFPSearch_Process

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Process(
    unsigned char* p, int w, int h, int s,
    double dTime, void* pDataTmp);
```

Alimenta un fotograma RGB decodificado. `dTime` es la marca de tiempo en segundos. Retorna 0 en caso de éxito.

#### VFPSearch_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPSearch_Build(int* pLen, void* pDataTmp);
```

Finaliza la huella. Retorna un búfer `char*`; `*pLen` recibe su tamaño en bytes.

#### VFPSearch_Search

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search(
    const char* pData1, int iLen1, int iSkip1,
    const char* pData2, int iLen2, int iSkip2,
    double* pDiff, int maxDiff);
```

Búsqueda de bajo nivel usando datos de huella sin procesar. Retorna la posición en segundos.

#### VFPSearch_Clear

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_Clear(void* pDataTmp);
```

Libera los recursos asignados por `VFPSearch_Init` o `VFPSearch_Init2`.

## Funciones de Comparación

La API de comparación proporciona tanto acceso de alto nivel como de bajo nivel por fotograma.

### API de Alto Nivel

#### VFPCompare_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPCompare_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Genera una huella de comparación directamente desde un archivo de video.

#### VFPCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPCompare_Compare(
    const char* pData1, int iLen1,
    const char* pData2, int iLen2,
    int MaxS);
```

Compara dos búferes de huella sin procesar. `MaxS` es el desplazamiento máximo en segundos. Retorna una puntuación de diferencia (menor = más similar).

### API de Bajo Nivel por Fotograma

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

Alimenta un fotograma RGB decodificado. `dTime` es la marca de tiempo en segundos.

#### VFPCompare_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPCompare_Build(int* pLen, void* pDataTmp);
```

Finaliza la huella. Retorna un búfer `char*`; `*pLen` recibe su tamaño.

#### VFPCompare_Clear

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPCompare_Clear(void* pDataTmp);
```

Libera los recursos del acumulador.

## Funciones de Utilidad

### VFPFingerprintSave

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintSave(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Descripción:** Guarda una huella en un archivo en el formato actual.

**Parámetros:**

- `vfp`: Puntero a la huella a guardar
- `filename`: Ruta al archivo de salida

**Ejemplo:**

```cpp
VFPFingerPrint fingerprint{};
// ... generar huella ...
VFPFingerprintSave(&fingerprint, L"salida.vfpsig");
```

### VFPFingerprintLoad

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintLoad(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Descripción:** Carga una huella desde un archivo.

**Parámetros:**

- `vfp`: Puntero a la estructura que recibirá los datos
- `filename`: Ruta al archivo de huella

**Ejemplo:**

```cpp
VFPFingerPrint fingerprint{};
VFPFingerprintLoad(&fingerprint, L"guardado.vfpsig");

printf("Huella cargada:\n");
printf("  Duración: %lld ms\n", fingerprint.Duration);
printf("  Archivo original: %ls\n", fingerprint.OriginalFilename);
printf("  Resolución: %dx%d\n", fingerprint.Width, fingerprint.Height);
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

**Descripción:** Guarda y carga huellas en formato legacy para compatibilidad con versiones anteriores.

## Comparación de Imágenes

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

**Descripción:** Compara dos imágenes y retorna una puntuación de similitud.

**Parámetros:**

- `image1`: Datos de la primera imagen (formato RGB24)
- `image1width`: Ancho de la primera imagen
- `image1height`: Alto de la primera imagen
- `image2`: Datos de la segunda imagen (formato RGB24)
- `image2width`: Ancho de la segunda imagen
- `image2height`: Alto de la segunda imagen

**Retorna:** Puntuación de similitud (0-100, valores más altos indican mayor similitud)

**Ejemplo:**

```cpp
// Asumiendo que tenemos dos imágenes RGB24 cargadas
BYTE* img1 = LoadImage("imagen1.bmp", &width1, &height1);
BYTE* img2 = LoadImage("imagen2.bmp", &width2, &height2);

double similarity = VFPImageCompare_Compare(
    img1, width1, height1,
    img2, width2, height2
);

printf("Similitud de imagen: %.2f%%\n", similarity);
```

## Ejemplos Completos Funcionales

### Ejemplo 1: Generar Huella de Búsqueda (API de alto nivel)

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"SU-CLAVE-DE-LICENCIA");

    VFPFingerprintSource src{};
    VFPFillSource(L"sample.mp4", &src);
    src.StartTime = 10000;   // comenzar en 10 s
    src.StopTime = 60000;    // detener en 60 s

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    printf("Huella: %dx%d, %.1fs, %d bytes\n",
           fp.Width, fp.Height, fp.Duration / 1000.0, fp.DataSize);
    VFPFingerprintSave(&fp, L"sample.vfpsig");
    return 0;
}
```

### Ejemplo 2: Comparar Dos Videos

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"SU-CLAVE-DE-LICENCIA");

    // Generar huella para video 1
    VFPFingerprintSource src1{};
    VFPFillSource(L"video1.mp4", &src1);
    VFPFingerPrint fp1{};
    VFPCompare_GetFingerprintForVideoFile(src1, &fp1);

    // Generar huella para video 2
    VFPFingerprintSource src2{};
    VFPFillSource(L"video2.mp4", &src2);
    VFPFingerPrint fp2{};
    VFPCompare_GetFingerprintForVideoFile(src2, &fp2);

    double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                     fp2.Data, fp2.DataSize, 10);
    printf("Diferencia: %.2f\n", diff);
    if (diff < 100)       printf("Muy similar\n");
    else if (diff < 500)  printf("Alguna similitud\n");
    else                  printf("Diferente\n");

    return 0;
}
```

### Ejemplo 3: Buscar Fragmento en Video Más Largo

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"SU-CLAVE-DE-LICENCIA");

    // Construir huella del fragmento
    VFPFingerprintSource needleSrc{};
    VFPFillSource(L"fragmento.mp4", &needleSrc);
    VFPFingerPrint needle{};
    VFPSearch_GetFingerprintForVideoFile(needleSrc, &needle);

    // Construir huella del video completo
    VFPFingerprintSource haystackSrc{};
    VFPFillSource(L"emision.mp4", &haystackSrc);
    VFPFingerPrint haystack{};
    VFPSearch_GetFingerprintForVideoFile(haystackSrc, &haystack);

    // Buscar todas las ocurrencias
    double diff = 0;
    int pos = 0;
    const int needleSec = (int)(needle.Duration / 1000);

    while (pos < (int)(haystack.Duration / 1000))
    {
        pos = VFPSearch_Search2(&needle, 0, &haystack, pos, &diff, 300);
        if (pos == INT_MAX) break;

        printf("Encontrado en %d segundos (diff: %.2f)\n", pos, diff);
        pos += needleSec;
    }

    return 0;
}
```

## Soporte de Plataformas

### Windows

- **Arquitecturas**: x86, x64
- **Compiladores**: Visual Studio 2019 o superior, MinGW-w64
- **Librerías**: VisioForge_VideoFingerprinting.dll (x86/x64)

### Linux

- **Arquitecturas**: x64, ARM64
- **Compiladores**: GCC 7+, Clang 6+
- **Dependencias**: GStreamer 1.18+

### macOS

- **Arquitecturas**: x64, Apple Silicon (M1/M2)
- **Compiladores**: Xcode 12+, Clang
- **Frameworks**: Sin dependencias adicionales

## Compilación y Enlazado

### Visual Studio (Windows)

1. Añadir directorio de inclusión:

   ```
   $(SolutionDir)include
   ```

2. Añadir directorio de librerías:

   ```
   $(SolutionDir)lib
   ```

3. Enlazar librerías:
   - Para x86: `VisioForge_VideoFingerprinting.lib`
   - Para x64: `VisioForge_VideoFingerprinting_x64.lib`

4. Copiar DLLs runtime al directorio de salida:
   - `VisioForge_VideoFingerprinting.dll` o `VisioForge_VideoFingerprinting_x64.dll`
   - `VisioForge_FFMPEG_Source.dll` o `VisioForge_FFMPEG_Source_x64.dll`

### CMake

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)

# Directorios de inclusión
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include)

# Directorios de enlace
link_directories(${CMAKE_CURRENT_SOURCE_DIR}/lib)

# Añadir ejecutable
add_executable(vfp_example main.cpp)

# Enlazar librerías
if(WIN32)
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        target_link_libraries(vfp_example VisioForge_VideoFingerprinting_x64)
    else()
        target_link_libraries(vfp_example VisioForge_VideoFingerprinting)
    endif()
endif()

# Copiar DLLs en Windows
if(WIN32)
    add_custom_command(TARGET vfp_example POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${CMAKE_CURRENT_SOURCE_DIR}/redist/*.dll"
        $<TARGET_FILE_DIR:vfp_example>)
endif()
```

### Linux/macOS

```bash
# Compilar
g++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example

# Configurar ruta de librerías (Linux)
export LD_LIBRARY_PATH=./lib:$LD_LIBRARY_PATH

# Configurar ruta de librerías (macOS)
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Ejecutar
./vfp_example
```

## Consideraciones de Rendimiento

### Gestión de Memoria

- Los datos de la huella son asignados internamente por el SDK
- Siempre verifique los valores de retorno para errores
- Libere los recursos correctamente al terminar

### Velocidad de Procesamiento

- Las huellas de búsqueda procesan aproximadamente 30x tiempo real en CPUs modernas
- Las huellas de comparación procesan aproximadamente 100x tiempo real
- El rendimiento escala con los núcleos de CPU para procesamiento por lotes

### Consejos de Optimización

1. **Use el tipo de huella apropiado**: Búsqueda para detección de fragmentos, Comparación para video completo
2. **Establezca rangos de tiempo**: Procese solo los segmentos necesarios para reducir tiempo
3. **Configure áreas ignoradas**: Excluya logos y tickers para mejorar precisión
4. **Ajuste umbrales**: Equilibre entre falsos positivos y falsos negativos
5. **Cachee huellas**: Guarde huellas generadas para evitar reprocesamiento

## Manejo de Errores

Todas las funciones que retornan `wchar_t*` devuelven NULL en caso de éxito y un mensaje de error en caso de fallo:

```cpp
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
if (error != nullptr) {
    wprintf(L"Error: %s\n", error);
    return -1;
}
```

Escenarios de error comunes:

- Archivo no encontrado o inaccesible
- Formato de video no soportado
- Memoria insuficiente
- Clave de licencia inválida
- Archivo de huella corrupto

## Guía de Umbrales

### Operaciones de Búsqueda

- **100-200**: Coincidencia muy estricta (copias exactas o casi exactas)
- **200-400**: Coincidencia normal (diferencias menores de codificación)
- **400-600**: Coincidencia flexible (diferencias significativas de calidad)
- **600+**: Coincidencia muy flexible (puede producir falsos positivos)

### Operaciones de Comparación

- **< 100**: Los videos son casi idénticos
- **100-300**: Los videos son muy similares (probablemente el mismo contenido)
- **300-500**: Los videos tienen similitudes significativas
- **500-1000**: Los videos tienen algunas similitudes
- **> 1000**: Los videos son diferentes

## Mejores Prácticas

1. **Siempre establezca una clave de licencia** antes de llamar a cualquier función del SDK
2. **Verifique los valores de retorno** en todas las operaciones
3. **Use tipos de huella apropiados** para su caso de uso
4. **Guarde huellas** para evitar reprocesar archivos de video grandes
5. **Configure áreas ignoradas** para contenido con superposiciones o logos
6. **Pruebe valores de umbral** con su tipo de contenido específico
7. **Maneje errores con elegancia** y proporcione retroalimentación significativa
8. **Libere recursos** cuando ya no sean necesarios
9. **Use procesamiento por lotes** para múltiples archivos
10. **Monitoree el uso de memoria** al procesar muchos archivos

## Comparación con el SDK .NET

El SDK C++ proporciona la misma funcionalidad principal que el SDK .NET con estas diferencias:

### Ventajas

- Rendimiento nativo directo sin sobrecarga gestionada
- Menor consumo de memoria
- Integración más fácil con aplicaciones C++ existentes
- Sin requisito de runtime .NET

### Diferencias

- Se requiere gestión manual de memoria
- Usa cadenas de caracteres anchos para compatibilidad con Windows
- API basada en funciones en lugar de orientada a objetos
- Acceso directo a funciones de procesamiento de bajo nivel

### Paridad de Funcionalidades

Ambos SDKs soportan:

- Generación de huellas de Búsqueda y Comparación
- Detección de fragmentos dentro de videos más largos
- Comparación de similitud entre videos
- Comparación de imágenes (solo Windows para C++)
- Recorte personalizado y áreas ignoradas
- Especificación de rango de tiempo
- Operaciones de guardar/cargar huellas

## Soporte y Recursos

Para soporte y recursos adicionales:

- **Ejemplos**: Disponibles en el paquete SDK en el directorio `Demos/`
- **Soporte**: <support@visioforge.com>
- **Licencia**: <https://www.visioforge.com/>
