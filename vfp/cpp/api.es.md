---
title: Documentación de API C++ del SDK de Huellas de Video
description: API completa del SDK de Huellas de Video C++ de VisioForge para generar, comparar y buscar huellas de video con ejemplos.
---

# Documentación de API C++ del SDK de Huellas de Video

## Descripción General

El SDK C++ de Huellas de Video de VisioForge proporciona una biblioteca nativa de alto rendimiento para identificación, comparación y operaciones de búsqueda de contenido de video. Permite a las aplicaciones:

- Generar huellas únicas de archivos de video para identificación de contenido
- Comparar videos para determinar similitud y detectar duplicados
- Buscar fragmentos de video dentro de videos más grandes (ej., encontrar comerciales, intros o escenas específicas)
- Comparar imágenes individuales para detección de similitud
- Procesar fotogramas de video directamente para generar huellas de streams o contenido generado

El SDK C++ ofrece rendimiento óptimo para aplicaciones de alto rendimiento y puede integrarse en aplicaciones C++ existentes o usarse a través de P/Invoke desde otros lenguajes.

> **Documentación Relacionada:**
>
> - [Referencia de API .NET](../dotnet/api.md) - Para desarrolladores de código administrado
> - [Entendiendo las Huellas de Video](../understanding-video-fingerprinting.md) - Conceptos básicos
> - [Tipos de Huellas](../fingerprint-types.md) - Modos Compare vs Search

## Tabla de Contenidos

- [Archivos de Cabecera](#archivos-de-cabecera)
- [Gestión de Licencias](#gestion-de-licencias)
- [Tipos y Estructuras Principales](#tipos-y-estructuras-principales)
- [Funciones de Búsqueda](#funciones-de-busqueda)
- [Funciones de Comparación](#funciones-de-comparacion)
- [Funciones de Utilidad](#funciones-de-utilidad)
- [Comparación de Imágenes](#comparacion-de-imagenes)
- [Ejemplos de Trabajo Completos](#ejemplos-de-trabajo-completos)
- [Soporte de Plataformas](#soporte-de-plataformas)
- [Compilación y Enlace](#compilacion-y-enlace)
- [Consideraciones de Rendimiento](#consideraciones-de-rendimiento)
- [Manejo de Errores](#manejo-de-errores)

## Archivos de Cabecera

El SDK proporciona dos archivos de cabecera principales:

### VisioForge_VFP.h

Cabecera principal de la API que contiene todas las declaraciones de funciones y exportaciones.

### VisioForge_VFP_Types.h

Definiciones de tipos y estructuras de datos usadas por el SDK.

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>
```

## Gestión de Licencias

### VFPSetLicenseKey

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKey(wchar_t* licenseKey);
```

**Descripción:** Establece la clave de licencia para el SDK de Huellas de Video. Debe llamarse antes de usar cualquier característica de huellas.

**Parámetros:**

- `licenseKey` (wchar_t*): Tu clave de licencia VisioForge como cadena de caracteres anchos

**Ejemplo:**

```cpp
// Establecer clave de licencia al inicio de la aplicación
VFPSetLicenseKey(L"TU-CLAVE-DE-LICENCIA-AQUI");

// Para modo de prueba
VFPSetLicenseKey(L"TRIAL");
```

### VFPSetLicenseKeyA

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKeyA(char* licenseKey);
```

**Descripción:** Establece la clave de licencia usando cadena ANSI (alternativa a VFPSetLicenseKey).

**Parámetros:**

- `licenseKey` (char*): Tu clave de licencia VisioForge como cadena ANSI

**Ejemplo:**

```cpp
// Establecer clave de licencia usando cadena ANSI
VFPSetLicenseKeyA("TU-CLAVE-DE-LICENCIA-AQUI");
```

## Tipos y Estructuras Principales

### VFPFingerprintSource

```cpp
struct VFPFingerprintSource
{
    wchar_t Filename[256];     // Ruta del archivo de video
    INT64 StartTime;            // Tiempo de inicio en milisegundos
    INT64 StopTime;             // Tiempo de fin en milisegundos
    RECT CustomCropSize;        // Área de recorte personalizada
    SIZE CustomResolution;      // Resolución personalizada para procesamiento
    RECT IgnoredAreas[10];      // Áreas a ignorar (ej., logos, tickers)
    INT64 OriginalDuration;     // Duración original del archivo
};
```

**Descripción:** Estructura de configuración para generación de huellas desde archivos de video.

**Campos:**

- `Filename`: Ruta al archivo de video (máximo 256 caracteres)
- `StartTime`: Posición de inicio en milisegundos (0 para el comienzo)
- `StopTime`: Posición de fin en milisegundos (0 para fin del archivo)
- `CustomCropSize`: Rectángulo de recorte opcional (izquierda, arriba, derecha, abajo)
- `CustomResolution`: Resolución personalizada opcional para procesamiento
- `IgnoredAreas`: Hasta 10 áreas rectangulares a ignorar durante la generación de huellas
- `OriginalDuration`: Duración del archivo original en milisegundos

**Ejemplo:**

```cpp
VFPFingerprintSource source{};
wcscpy_s(source.Filename, L"C:\\Videos\\muestra.mp4");
source.StartTime = 10000;  // Iniciar en 10 segundos
source.StopTime = 60000;   // Detener en 60 segundos

// Ignorar logo en esquina superior derecha
source.IgnoredAreas[0] = {1800, 0, 1920, 100};
```

### VFPFingerPrint

```cpp
struct VFPFingerPrint
{
    char* Data;                    // Datos de la huella
    INT32 DataSize;                // Tamaño de los datos de la huella
    INT64 Duration;                // Duración en milisegundos
    GUID ID;                       // Identificador único
    wchar_t OriginalFilename[256]; // Nombre del archivo original
    INT64 OriginalDuration;        // Duración del archivo original
    wchar_t Tag[100];              // Etiqueta opcional
    INT32 Width;                   // Ancho del video fuente
    INT32 Height;                  // Alto del video fuente
    double FrameRate;              // Tasa de fotogramas
};
```

**Descripción:** Estructura que contiene datos de huella generados y metadatos.

**Campos:**

- `Data`: Datos binarios de la huella
- `DataSize`: Tamaño de los datos de la huella en bytes
- `Duration`: Duración del contenido con huella en milisegundos
- `ID`: Identificador GUID único para la huella
- `OriginalFilename`: Ruta al archivo de video original
- `OriginalDuration`: Duración del archivo original
- `Tag`: Etiqueta opcional definida por el usuario (hasta 100 caracteres)
- `Width`: Ancho del video fuente
- `Height`: Alto del video fuente
- `FrameRate`: Tasa de fotogramas del video fuente

## Funciones de Búsqueda

### VFPSearch_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFile(
    VFPFingerprintSource source, 
    VFPFingerPrint* vfp);
```

**Descripción:** Genera una huella optimizada para búsqueda desde un archivo de video.

**Parámetros:**

- `source`: Configuración del video fuente
- `vfp`: Puntero a estructura de huella para recibir el resultado

**Retorna:** Mensaje de error si falla, NULL si es exitoso

**Ejemplo:**

```cpp
VFPFingerprintSource source{};
VFPFillSource(L"C:\\Videos\\comercial.mp4", &source);

VFPFingerPrint fingerprint{};
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);

if (error == nullptr) {
    printf("Huella generada exitosamente\n");
    printf("Duración: %lld ms\n", fingerprint.Duration);
    printf("Tamaño: %d bytes\n", fingerprint.DataSize);
} else {
    wprintf(L"Error: %s\n", error);
}
```

### VFPSearch_Search2

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search2(
    VFPFingerPrint* vfp1, 
    int iSkip1,
    VFPFingerPrint* vfp2, 
    int iSkip2, 
    double* pDiff,
    int maxDiff);
```

**Descripción:** Busca una huella dentro de otra, retornando la posición donde se encontró.

**Parámetros:**

- `vfp1`: Huella a buscar (aguja)
- `iSkip1`: Posición de inicio en vfp1 (en segundos)
- `vfp2`: Huella donde buscar (pajar)
- `iSkip2`: Posición de inicio en vfp2 (en segundos)
- `pDiff`: Puntero para recibir el valor de diferencia
- `maxDiff`: Umbral máximo de diferencia permitido

**Retorna:** Posición en segundos donde se encontró la coincidencia, o INT_MAX si no se encontró

**Ejemplo:**

```cpp
// Buscar comercial en transmisión
VFPFingerPrint commercial{};
VFPFingerPrint broadcast{};

// Cargar huellas
VFPFingerprintLoad(&commercial, L"comercial.vfpsig");
VFPFingerprintLoad(&broadcast, L"transmision.vfpsig");

double diff = 0;
int position = VFPSearch_Search2(&commercial, 0, &broadcast, 0, &diff, 300);

if (position != INT_MAX) {
    printf("Comercial encontrado en posición: %d segundos\n", position);
    printf("Puntuación de diferencia: %.2f\n", diff);
} else {
    printf("Comercial no encontrado en la transmisión\n");
}
```

### VFPSearch_SearchOneSignatureFileInAnother

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_SearchOneSignatureFileInAnother(
    wchar_t* file1, 
    wchar_t* file2, 
    LONG threshold,
    LONG* position);
```

**Descripción:** Busca un archivo de firma dentro de otro directamente desde disco.

**Parámetros:**

- `file1`: Ruta al archivo de huella a buscar
- `file2`: Ruta al archivo de huella donde buscar
- `threshold`: Umbral máximo de diferencia permitido
- `position`: Puntero para recibir la posición donde se encontró

**Ejemplo:**

```cpp
LONG position = 0;
VFPSearch_SearchOneSignatureFileInAnother(
    L"aguja.vfpsig", 
    L"pajar.vfpsig", 
    300,  // umbral
    &position);

if (position != INT_MAX) {
    printf("Coincidencia encontrada en: %ld segundos\n", position);
}
```

## Funciones de Comparación

### VFPCompare_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPCompare_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

**Descripción:** Genera una huella optimizada para comparación desde un archivo de video.

**Parámetros:**

- `source`: Configuración del video fuente
- `vfp`: Puntero a estructura de huella para recibir el resultado

**Retorna:** Mensaje de error si falla, NULL si es exitoso

**Ejemplo:**

```cpp
VFPFingerprintSource source{};
VFPFillSource(L"C:\\Videos\\video1.mp4", &source);

VFPFingerPrint fingerprint{};
wchar_t* error = VFPCompare_GetFingerprintForVideoFile(source, &fingerprint);

if (error == nullptr) {
    // Guardar huella para comparación posterior
    VFPFingerprintSave(&fingerprint, L"video1.vfpsig");
}
```

### VFPCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPCompare_Compare(
    const char* pData1,
    int iLen1,
    const char* pData2,
    int iLen2,
    int MaxS);
```

**Descripción:** Compara dos huellas y retorna una puntuación de diferencia.

**Parámetros:**

- `pData1`: Datos de la primera huella
- `iLen1`: Tamaño de la primera huella
- `pData2`: Datos de la segunda huella
- `iLen2`: Tamaño de la segunda huella
- `MaxS`: Máximo desplazamiento temporal en segundos a verificar

**Retorna:** Puntuación de diferencia (valores más bajos indican más similitud)

**Ejemplo:**

```cpp
VFPFingerPrint fp1{}, fp2{};
VFPFingerprintLoad(&fp1, L"video1.vfpsig");
VFPFingerprintLoad(&fp2, L"video2.vfpsig");

double difference = VFPCompare_Compare(
    fp1.Data, fp1.DataSize,
    fp2.Data, fp2.DataSize,
    10  // Verificar hasta 10 segundos de desplazamiento
);

printf("Puntuación de diferencia: %.2f\n", difference);
if (difference < 100) {
    printf("Los videos son muy similares (probablemente duplicados)\n");
} else if (difference < 500) {
    printf("Los videos tienen similitudes significativas\n");
} else {
    printf("Los videos son diferentes\n");
}
```

## Funciones de Utilidad

### VFPFillSource

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFillSource(
    wchar_t* filename,
    VFPFingerprintSource* source);
```

**Descripción:** Inicializa una estructura VFPFingerprintSource con valores predeterminados para un archivo de video.

**Parámetros:**

- `filename`: Ruta al archivo de video
- `source`: Puntero a estructura fuente a inicializar

**Ejemplo:**

```cpp
VFPFingerprintSource source{};
VFPFillSource(L"C:\\Videos\\muestra.mp4", &source);

// Opcionalmente modificar configuraciones después de la inicialización
source.StartTime = 5000;  // Iniciar en 5 segundos
source.StopTime = 30000;  // Detener en 30 segundos
```

### VFPFingerprintSave

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintSave(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Descripción:** Guarda una huella en un archivo en el formato actual.

**Parámetros:**

- `vfp`: Puntero a huella a guardar
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

- `vfp`: Puntero a estructura de huella para recibir los datos
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

**Descripción:** Guarda y carga huellas en formato legado para compatibilidad hacia atrás.

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

**Retorna:** Puntuación de similitud (0-100, valores más altos indican más similitud)

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

## Ejemplos de Trabajo Completos

### Ejemplo 1: Generar Huella

```cpp
#include <iostream>
#include <Windows.h>
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

int main()
{
    // Establecer clave de licencia
    VFPSetLicenseKey(L"TU-CLAVE-DE-LICENCIA");
    
    // Configurar fuente
    VFPFingerprintSource source{};
    VFPFillSource(L"C:\\Videos\\muestra.mp4", &source);
    
    // Opcional: Procesar solo un segmento
    source.StartTime = 10000;  // Iniciar en 10 segundos
    source.StopTime = 60000;   // Detener en 60 segundos
    
    // Opcional: Ignorar área de logo
    source.IgnoredAreas[0] = {1800, 0, 1920, 100};
    
    // Generar huella de búsqueda
    VFPFingerPrint fingerprint{};
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
    
    if (error == nullptr) {
        printf("Huella generada exitosamente\n");
        printf("Duración: %lld ms\n", fingerprint.Duration);
        printf("Tamaño de datos: %d bytes\n", fingerprint.DataSize);
        
        // Guardar en archivo
        VFPFingerprintSave(&fingerprint, L"muestra.vfpsig");
        printf("Huella guardada en muestra.vfpsig\n");
    } else {
        wprintf(L"Error: %s\n", error);
        return 1;
    }
    
    return 0;
}
```

### Ejemplo 2: Comparar Dos Videos

```cpp
#include <iostream>
#include <Windows.h>
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

int main(int argc, char* argv[])
{
    if (argc != 3) {
        printf("Uso: compare video1.mp4 video2.mp4\n");
        return 1;
    }
    
    // Establecer clave de licencia
    VFPSetLicenseKey(L"TU-CLAVE-DE-LICENCIA");
    
    // Generar huellas para ambos videos
    VFPFingerprintSource source1{}, source2{};
    VFPFingerPrint fp1{}, fp2{};
    
    // Convertir nombres de archivo a char ancho
    wchar_t file1[256], file2[256];
    mbstowcs(file1, argv[1], 256);
    mbstowcs(file2, argv[2], 256);
    
    // Generar primera huella
    VFPFillSource(file1, &source1);
    wchar_t* error = VFPCompare_GetFingerprintForVideoFile(source1, &fp1);
    if (error != nullptr) {
        wprintf(L"Error procesando primer video: %s\n", error);
        return 1;
    }
    
    // Generar segunda huella
    VFPFillSource(file2, &source2);
    error = VFPCompare_GetFingerprintForVideoFile(source2, &fp2);
    if (error != nullptr) {
        wprintf(L"Error procesando segundo video: %s\n", error);
        return 1;
    }
    
    // Comparar huellas
    double difference = VFPCompare_Compare(
        fp1.Data, fp1.DataSize,
        fp2.Data, fp2.DataSize,
        10  // Verificar hasta 10 segundos de desplazamiento
    );
    
    printf("Resultados de Comparación:\n");
    printf("  Video 1: %ls (%.2f segundos)\n", file1, fp1.Duration / 1000.0);
    printf("  Video 2: %ls (%.2f segundos)\n", file2, fp2.Duration / 1000.0);
    printf("  Puntuación de diferencia: %.2f\n", difference);
    
    if (difference < 100) {
        printf("  Resultado: Los videos son muy similares (probablemente duplicados)\n");
    } else if (difference < 500) {
        printf("  Resultado: Los videos tienen similitudes significativas\n");
    } else {
        printf("  Resultado: Los videos son diferentes\n");
    }
    
    return 0;
}
```

### Ejemplo 3: Buscar Comercial en Transmisión

```cpp
#include <iostream>
#include <Windows.h>
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>
#include <ctime>

std::string timeToString(time_t tm)
{
    char buff[20];
    strftime(buff, 20, "%H:%M:%S", gmtime(&tm));
    return std::string(buff);
}

int main(int argc, char* argv[])
{
    if (argc != 3) {
        printf("Uso: search comercial.mp4 transmision.mp4\n");
        return 1;
    }
    
    // Establecer clave de licencia
    VFPSetLicenseKey(L"TU-CLAVE-DE-LICENCIA");
    
    wchar_t commercial[256], broadcast[256];
    mbstowcs(commercial, argv[1], 256);
    mbstowcs(broadcast, argv[2], 256);
    
    // Generar huellas
    VFPFingerprintSource src1{}, src2{};
    VFPFingerPrint fp_commercial{}, fp_broadcast{};
    
    printf("Generando huella para comercial...\n");
    VFPFillSource(commercial, &src1);
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(src1, &fp_commercial);
    if (error != nullptr) {
        wprintf(L"Error: %s\n", error);
        return 1;
    }
    
    printf("Generando huella para transmisión...\n");
    VFPFillSource(broadcast, &src2);
    error = VFPSearch_GetFingerprintForVideoFile(src2, &fp_broadcast);
    if (error != nullptr) {
        wprintf(L"Error: %s\n", error);
        return 1;
    }
    
    // Buscar todas las ocurrencias
    printf("\nBuscando comercial en transmisión...\n");
    printf("Duración del comercial: %.2f segundos\n", fp_commercial.Duration / 1000.0);
    printf("Duración de la transmisión: %.2f segundos\n\n", fp_broadcast.Duration / 1000.0);
    
    const int threshold = 300;
    double diff = 0;
    int position = 0;
    int occurrences = 0;
    
    const int commercial_duration = (int)(fp_commercial.Duration / 1000);
    const int broadcast_duration = (int)(fp_broadcast.Duration / 1000);
    
    while (position < broadcast_duration) {
        position = VFPSearch_Search2(
            &fp_commercial, 0,
            &fp_broadcast, position,
            &diff, threshold
        );
        
        if (position == INT_MAX) {
            break;
        }
        
        occurrences++;
        printf("Coincidencia #%d encontrada en %s (diferencia: %.2f)\n",
               occurrences, timeToString(position).c_str(), diff);
        
        // Saltar más allá de esta ocurrencia
        position += commercial_duration;
    }
    
    if (occurrences == 0) {
        printf("Comercial no encontrado en la transmisión.\n");
    } else {
        printf("\nTotal de ocurrencias encontradas: %d\n", occurrences);
    }
    
    return 0;
}
```

## Soporte de Plataformas

### Windows

- **Arquitecturas**: x86, x64
- **Compiladores**: Visual Studio 2019 o posterior, MinGW-w64
- **Bibliotecas**: VisioForge_VideoFingerprinting.dll (x86/x64)

### Linux

- **Arquitecturas**: x64, ARM64
- **Compiladores**: GCC 7+, Clang 6+
- **Dependencias**: GStreamer 1.18+

### macOS

- **Arquitecturas**: x64, Apple Silicon (M1/M2)
- **Compiladores**: Xcode 12+, Clang
- **Frameworks**: Sin dependencias adicionales

## Compilación y Enlace

### Visual Studio (Windows)

1. Agregar directorio de inclusión:

   ```
   $(SolutionDir)include
   ```

2. Agregar directorio de biblioteca:

   ```
   $(SolutionDir)lib
   ```

3. Enlazar bibliotecas:
   - Para x86: `VisioForge_VideoFingerprinting.lib`
   - Para x64: `VisioForge_VideoFingerprinting_x64.lib`

4. Copiar DLLs de tiempo de ejecución al directorio de salida:
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

# Agregar ejecutable
add_executable(vfp_example main.cpp)

# Enlazar bibliotecas
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

# Establecer ruta de biblioteca (Linux)
export LD_LIBRARY_PATH=./lib:$LD_LIBRARY_PATH

# Establecer ruta de biblioteca (macOS)
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Ejecutar
./vfp_example
```

## Consideraciones de Rendimiento

### Gestión de Memoria

- Los datos de huella son asignados internamente por el SDK
- Siempre verifica los valores de retorno para errores
- Libera recursos apropiadamente cuando termines

### Velocidad de Procesamiento

- Las huellas de búsqueda procesan aproximadamente 30x tiempo real en CPUs modernos
- Las huellas de comparación procesan aproximadamente 100x tiempo real
- El rendimiento escala con núcleos de CPU para procesamiento por lotes

### Consejos de Optimización

1. **Usa el tipo de huella apropiado**: Search para detección de fragmentos, Compare para comparación de videos completos
2. **Establece rangos de tiempo**: Procesa solo segmentos requeridos para reducir tiempo de procesamiento
3. **Configura áreas ignoradas**: Excluye logos y tickers para mejorar precisión
4. **Ajusta umbrales**: Equilibra entre falsos positivos y falsos negativos
5. **Cachea huellas**: Guarda huellas generadas para evitar reprocesamiento

## Manejo de Errores

Todas las funciones que retornan `wchar_t*` retornan NULL en éxito y un mensaje de error en caso de falla:

```cpp
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
if (error != nullptr) {
    wprintf(L"Ocurrió un error: %s\n", error);
    // Manejar error apropiadamente
    return -1;
}
```

Escenarios de error comunes:

- Archivo no encontrado o inaccesible
- Formato de video no soportado
- Memoria insuficiente
- Clave de licencia inválida
- Archivo de huella corrupto

## Guías de Umbrales

### Operaciones de Búsqueda

- **100-200**: Coincidencia muy estricta (copias exactas o casi exactas)
- **200-400**: Coincidencia normal (diferencias menores de codificación permitidas)
- **400-600**: Coincidencia flexible (diferencias significativas de calidad permitidas)
- **600+**: Coincidencia muy flexible (puede producir falsos positivos)

### Operaciones de Comparación

- **< 100**: Los videos son casi idénticos
- **100-300**: Los videos son muy similares (probablemente mismo contenido)
- **300-500**: Los videos tienen similitudes significativas
- **500-1000**: Los videos tienen algunas similitudes
- **> 1000**: Los videos son diferentes

## Mejores Prácticas

1. **Siempre establece una clave de licencia** antes de llamar cualquier función del SDK
2. **Verifica los valores de retorno** para todas las operaciones
3. **Usa tipos de huella apropiados** para tu caso de uso
4. **Guarda huellas** para evitar reprocesar archivos de video grandes
5. **Configura áreas ignoradas** para contenido con superposiciones o logos
6. **Prueba valores de umbral** con tu tipo de contenido específico
7. **Maneja errores graciosamente** y proporciona retroalimentación significativa
8. **Libera recursos** cuando ya no se necesiten
9. **Usa procesamiento por lotes** para múltiples archivos
10. **Monitorea el uso de memoria** cuando proceses muchos archivos

## Comparación con SDK .NET

El SDK C++ proporciona la misma funcionalidad central que el SDK .NET con estas diferencias:

### Ventajas

- Rendimiento nativo directo sin sobrecarga administrada
- Menor huella de memoria
- Integración más fácil con aplicaciones C++ existentes
- Sin requisito de runtime .NET

### Diferencias

- Se requiere gestión de memoria manual
- Usa cadenas de caracteres anchos para compatibilidad con Windows
- API basada en funciones en lugar de orientada a objetos
- Acceso directo a funciones de procesamiento de bajo nivel

### Paridad de Características

Ambos SDKs soportan:

- Generación de huellas Search y Compare
- Detección de fragmentos dentro de videos más grandes
- Comparación de similitud entre videos
- Comparación de imágenes (solo Windows para C++)
- Recorte personalizado y áreas ignoradas
- Especificación de rango de tiempo
- Operaciones de guardado/carga de huellas

## Soporte y Recursos

Para soporte y recursos adicionales:

- **Ejemplos**: Disponibles en el paquete del SDK bajo el directorio `Demos/`
- **Soporte**: <support@visioforge.com>
- **Licencia**: <https://www.visioforge.com/>
