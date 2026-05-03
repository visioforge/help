---
title: SDK C++ de Video Fingerprinting - Instalación y Setup
description: Instale y configure el SDK Video Fingerprinting de VisioForge para C++ con configuración de Visual Studio, paquetes NuGet y su primera aplicación de huellas.
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

# Primeros Pasos con el SDK Video Fingerprinting para C++

¡Bienvenido al SDK Video Fingerprinting de VisioForge para C++! Esta guía completa le guiará por todo lo necesario para empezar, desde la instalación hasta su primera aplicación funcional. Al finalizar, tendrá una base sólida para crear aplicaciones de video fingerprinting de alto rendimiento en C++.

> **Nota:** Si ya está familiarizado con el SDK .NET, el SDK C++ sigue conceptos similares con beneficios de rendimiento nativo. Consulte nuestra [Comparación de SDKs](../index.md) para más detalles.

## Resumen de Inicio Rápido

Para empezar rápidamente:

1. Descargue el paquete SDK de VisioForge
2. Extraiga los archivos al directorio de su proyecto
3. Incluya las cabeceras: `#include <VisioForge_VFP.h>` y `#include <VisioForge_VFP_Types.h>`
4. Enlace la librería apropiada: `VisioForge_VideoFingerprinting.lib` (x86) o `VisioForge_VideoFingerprinting_x64.lib` (x64)
5. Copie las DLLs runtime a su directorio de salida
6. Establezca su clave de licencia: `VFPSetLicenseKey(L"clave-de-licencia");`
7. Genere su primera huella usando los ejemplos a continuación

## Requisitos Previos y del Sistema

Para requisitos detallados del sistema, incluyendo plataformas soportadas, especificaciones de hardware y consideraciones de rendimiento, consulte nuestra guía de [Requisitos del Sistema](../system-requirements.md).

### Requisitos Específicos de C++

- **Compilador Windows**: Visual Studio 2019+ (recomendado) o MinGW-w64
- **Compilador Linux**: GCC 7+ o Clang 6+
- **Compilador macOS**: Xcode 12+ con Command Line Tools
- **Herramientas de Build**: CMake 3.10+ (opcional pero recomendado para Linux/macOS)

## Contenido del Paquete SDK

Después de descargar el SDK, encontrará la siguiente estructura:

```
VideoFingerprinting_CPP_SDK/
├── include/
│   ├── VisioForge_VFP.h           # Cabecera principal de la API
│   └── VisioForge_VFP_Types.h     # Definiciones de tipos
├── lib/
│   ├── VisioForge_VideoFingerprinting.lib      # x86 import library
│   └── VisioForge_VideoFingerprinting_x64.lib  # x64 import library
├── redist/
│   ├── VisioForge_VideoFingerprinting.dll      # x86 runtime DLL
│   ├── VisioForge_VideoFingerprinting_x64.dll  # x64 runtime DLL
│   ├── VisioForge_FFMPEG_Source.dll           # x86 media support
│   └── VisioForge_FFMPEG_Source_x64.dll       # x64 media support
├── demos/
│   ├── vfp_gen/        # Fingerprint generation demo
│   ├── vfp_compare/    # Video comparison demo
│   └── vfp_search/     # Fragment search demo
└── README.txt
```

## Configuración del Entorno de Desarrollo

### Configuración Visual Studio (Windows)

#### Paso 1: Crear un Nuevo Proyecto

1. Abra Visual Studio 2019 o superior
2. Haga clic en "Crear un nuevo proyecto"
3. Seleccione "Aplicación de consola" (C++)
4. Nombre su proyecto (ej., "VFPExample")
5. Elija una ubicación y haga clic en "Crear"

#### Paso 2: Configurar las Propiedades del Proyecto

1. Haga clic derecho en su proyecto en el Explorador de soluciones
2. Seleccione "Propiedades"
3. Configure los siguientes ajustes:

**Propiedades de configuración → C/C++ → General:**

```
Additional Include Directories: $(ProjectDir)include
```

**Propiedades de configuración → Vinculador → General:**

```
Additional Library Directories: $(ProjectDir)lib
```

**Propiedades de configuración → Vinculador → Entrada:**

```
Additional Dependencies (x86): VisioForge_VideoFingerprinting.lib
Additional Dependencies (x64): VisioForge_VideoFingerprinting_x64.lib
```

#### Paso 3: Añadir Archivos del SDK

1. Copie la carpeta `include` al directorio de su proyecto
2. Copie la carpeta `lib` al directorio de su proyecto
3. Copie los archivos DLL de `redist` a su directorio de salida (ej., `Debug` o `Release`)

#### Paso 4: Configurar Eventos Post-Build

Añada un evento post-build para copiar las DLLs automáticamente:

```batch
xcopy /y "$(ProjectDir)redist\*.dll" "$(OutDir)"
```

### Configuración CMake (Multiplataforma)

Cree un archivo `CMakeLists.txt`:

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find SDK files
set(VFP_SDK_PATH "${CMAKE_CURRENT_SOURCE_DIR}/sdk")

# Include directories
include_directories(${VFP_SDK_PATH}/include)

# Platform-specific configuration
if(WIN32)
    # Windows configuration
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
    # Configuración de macOS
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
elseif(UNIX)
    # Configuración de Linux
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
endif()

# Añadir ejecutable
add_executable(vfp_example main.cpp)

# Enlazar librerías
target_link_libraries(vfp_example ${VFP_LIBRARIES})

# Copiar librerías de ejecución en Windows
if(WIN32)
    foreach(DLL ${VFP_RUNTIME_LIBS})
        add_custom_command(TARGET vfp_example POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            ${DLL} $<TARGET_FILE_DIR:vfp_example>)
    endforeach()
endif()
```

Compile el proyecto:

```bash
mkdir build
cd build
cmake ..
cmake --build .
```

### Configuración Linux

#### Instalar Dependencias

Ubuntu/Debian:

```bash
sudo apt-get update
sudo apt-get install build-essential cmake
sudo apt-get install libgstreamer1.0-dev libgstreamer-plugins-base1.0-dev
sudo apt-get install gstreamer1.0-plugins-good gstreamer1.0-plugins-bad
sudo apt-get install gstreamer1.0-libav
```

Fedora/RHEL:

```bash
sudo dnf install gcc-c++ cmake
sudo dnf install gstreamer1-devel gstreamer1-plugins-base-devel
sudo dnf install gstreamer1-plugins-good gstreamer1-plugins-bad-free
sudo dnf install gstreamer1-libav
```

#### Configuración de Build (Linux)

Cree un Makefile simple:

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

### Configuración macOS

#### Instalar Xcode Command Line Tools

```bash
xcode-select --install
```

#### Configuración de Build (macOS)

Cree un script de build `build.sh`:

```bash
#!/bin/bash

# Compiler settings
CXX=clang++
CXXFLAGS="-std=c++11 -Wall -I./include"
LDFLAGS="-L./lib -lVisioForge_VideoFingerprinting"

# Establecer la ruta de librerías
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Compilar
$CXX $CXXFLAGS main.cpp $LDFLAGS -o vfp_example

echo "Compilación completada. Ejecute con: ./vfp_example"
```

Hágalo ejecutable y ejecútelo:

```bash
chmod +x build.sh
./build.sh
```

## Su Primera Aplicación

Vamos a crear una aplicación simple que genera una huella desde un archivo de video.

### Paso 1: Crear main.cpp

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
    std::cout << "SDK VisioForge Video Fingerprinting - Primera Aplicación\n";
    std::cout << "========================================================\n\n";
    
    // Comprobar argumentos de la línea de comandos
    if (argc != 2) {
        std::cout << "Uso: " << argv[0] << " <archivo_de_video>\n";
        std::cout << "Ejemplo: " << argv[0] << " sample.mp4\n";
        return 1;
    }
    
    // Convertir el nombre de archivo a caracteres anchos (para compatibilidad con Windows)
    wchar_t videoFile[256];
#ifdef _WIN32
    size_t converted;
    mbstowcs_s(&converted, videoFile, argv[1], 256);
#else
    mbstowcs(videoFile, argv[1], 256);
#endif
    
    // Paso 1: Establecer la clave de licencia
    std::cout << "Estableciendo clave de licencia...\n";
    VFPSetLicenseKey(L"TRIAL");  // Use "TRIAL" para evaluación
    
    // Paso 2: Configurar la fuente
    VFPFingerprintSource src{};
    VFPFillSource(videoFile, &src);
    src.StartTime = 0;       // comenzar desde el inicio
    src.StopTime = 0;        // 0 = hasta el final del archivo

    // Paso 3: Generar la huella
    std::cout << "Generando huella...\n";
    VFPFingerPrint fingerprint{};
    VFPSearch_GetFingerprintForVideoFile(src, &fingerprint);

    // Paso 4: Mostrar resultados
    std::cout << "\n¡Huella generada correctamente!\n";
    std::cout << "=====================================\n";
    std::cout << "Información del video:\n";
    std::cout << "  Duración: " << (fingerprint.Duration / 1000.0) << " segundos\n";
    std::cout << "  Resolución: " << fingerprint.Width << "x" << fingerprint.Height << "\n";
    std::cout << "  Frecuencia de cuadros: " << fingerprint.FrameRate << " fps\n";
    std::cout << "  Tamaño de datos: " << fingerprint.DataSize << " bytes\n";
    
    // Paso 5: Guardar la huella en un archivo
    wchar_t outputFile[256];
#ifdef _WIN32
    wcscpy_s(outputFile, videoFile);
    wcscat_s(outputFile, L".vfpsig");
#else
    wcscpy(outputFile, videoFile);
    wcscat(outputFile, L".vfpsig");
#endif
    
    std::cout << "\nGuardando huella...\n";
    VFPFingerprintSave(&fingerprint, outputFile);
    
    char outputFileAnsi[256];
#ifdef _WIN32
    size_t convertedOut;
    wcstombs_s(&convertedOut, outputFileAnsi, outputFile, 256);
#else
    wcstombs(outputFileAnsi, outputFile, 256);
#endif
    
    std::cout << "Huella guardada en: " << outputFileAnsi << "\n";
    
    std::cout << "\n¡Éxito! Ahora puede usar esta huella para:\n";
    std::cout << "  - Compararla con otros videos para medir similitud\n";
    std::cout << "  - Buscar este video dentro de emisiones más largas\n";
    std::cout << "  - Crear una base de datos de huellas de video\n";
    
    return 0;
}
```

### Paso 2: Compilar y Ejecutar

#### Windows (Visual Studio)

1. Presione F7 para compilar la solución
2. Copie un archivo de video de prueba a su directorio de salida
3. Abra Command Prompt en el directorio de salida
4. Ejecute: `VFPExample.exe testvideo.mp4`

#### Windows (Command Line)

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

## Entendiendo los Tipos de Huellas

El SDK proporciona dos tipos de huellas optimizadas para diferentes casos de uso. Para una explicación completa incluyendo detalles técnicos, características de rendimiento y orientación para decidir, consulte nuestra [Guía de Tipos de Huellas](../fingerprint-types.md).

**Referencia Rápida:**
- **Huellas de Búsqueda** (`VFPSearch_Init` / `_Process` / `_Build`): Para encontrar fragmentos de video dentro de videos más largos (detección de anuncios, monitoreo de contenido)
- **Huellas de Comparación** (`VFPCompare_Init` / `_Process` / `_Build`): Para comparar videos completos por similitud (detección de duplicados, comparación de versiones)

## Casos de Uso Comunes

### Caso 1: Detección de Duplicados

```cpp
// Generate fingerprints for two videos
VFPFingerPrint fp1{}, fp2{};
// ... generate fingerprints ...

// Compare them
double difference = VFPCompare_Compare(
    fp1.Data, fp1.DataSize,
    fp2.Data, fp2.DataSize,
    10  // Allow up to 10 seconds shift
);

if (difference < 100) {
    std::cout << "Videos are duplicates\n";
}
```

### Caso 2: Detección de Anuncios

```cpp
// Load commercial and broadcast fingerprints
VFPFingerPrint commercial{}, broadcast{};
VFPFingerprintLoad(&commercial, L"commercial.vfpsig");
VFPFingerprintLoad(&broadcast, L"broadcast.vfpsig");

// Search for commercial
double diff;
int position = VFPSearch_Search2(
    &commercial, 0,
    &broadcast, 0,
    &diff, 300  // threshold
);

if (position != INT_MAX) {
    std::cout << "Commercial found at: " << position << " seconds\n";
}
```

### Caso 3: Verificación de Contenido con Áreas Ignoradas

```cpp
VFPFingerprintSource src{};
VFPFillSource(L"video.mp4", &src);

// Ignorar áreas de logotipos/marcas de agua (hasta 10 rectángulos)
src.IgnoredAreas[0].left   = 0;
src.IgnoredAreas[0].top    = 0;
src.IgnoredAreas[0].right  = 200;
src.IgnoredAreas[0].bottom = 100;    // Logotipo superior izquierdo

src.IgnoredAreas[1].left   = 1720;
src.IgnoredAreas[1].top    = 980;
src.IgnoredAreas[1].right  = 1920;
src.IgnoredAreas[1].bottom = 1080;   // Marca de agua inferior derecha

VFPFingerPrint fp{};
VFPSearch_GetFingerprintForVideoFile(src, &fp);
```

## Licencias

### Modo Trial

Para evaluación, use la licencia trial:

```cpp
VFPSetLicenseKey(L"TRIAL");
```

Limitaciones del modo trial:

- Añade marca de agua a los fotogramas procesados
- Limitado a 60 segundos de procesamiento por sesión
- Funcionalidad completa disponible por lo demás

### Licencia Comercial

Para uso en producción, adquiera una licencia de VisioForge:

```cpp
VFPSetLicenseKey(L"SU-CLAVE-DE-LICENCIA");
```

Tipos de licencia:

- **Licencia de Desarrollador**: Para desarrollo y pruebas
- **Licencia de Despliegue**: Para distribución con su aplicación
- **Licencia de Servidor**: Para despliegues basados en servidor

## Solución de Problemas

### Problemas Comunes y Soluciones

#### Problema: DLL No Encontrada

**Error**: "El programa no puede iniciarse porque falta VisioForge_VideoFingerprinting.dll"

**Solución**:

- Asegúrese de que las DLLs estén en el mismo directorio que su ejecutable
- O añada el directorio DLL a su variable de entorno PATH
- Verifique que usa la arquitectura correcta (x86 vs x64)

#### Problema: Formato de Video No Soportado

**Error**: "No se puede procesar el archivo de video"

**Solución**:

- Asegúrese de que el códec de video sea soportado (H.264, H.265, VP8, VP9, AV1)
- Instale paquetes de códec adicionales si es necesario
- Intente convertir el video a un formato estándar

#### Problema: Clave de Licencia No Funciona

**Error**: "Clave de licencia no válida"

**Solución**:

- Verifique que la clave esté ingresada correctamente
- Asegúrese de que VFPSetLicenseKey() se llame antes de cualquier otra función
- Compruebe que la licencia no haya expirado
- Verifique que usa la licencia correcta para su plataforma

#### Problema: Violación de Acceso a Memoria

**Error**: Violación de acceso al leer la ubicación

**Solución**:

- Inicialice todas las estructuras con {} antes de usar
- Verifique que los punteros sean válidos antes de usar
- Asegure tamaños de búfer de cadena adecuados
- No libere manualmente la memoria asignada por el SDK

#### Problema: Bajo Rendimiento

**Síntoma**: El procesamiento es más lento de lo esperado

**Solución**:

- Use configuración Release, no Debug
- Active optimizaciones del compilador (/O2 o -O2)
- Procese videos desde SSD local, no unidades de red
- Considere usar múltiples hilos para procesamiento por lotes
- Reduzca la resolución de video si la calidad lo permite

### Consejos de Depuración

1. **Active la salida de depuración**: Guarde datos de fotogramas intermedios para inspección
2. **Verifique valores de retorno**: Siempre compruebe retornos NULL/error
3. **Use builds de depuración**: Desarrolle inicialmente con símbolos de depuración
4. **Registre operaciones**: Añada logging para seguir el flujo de procesamiento
5. **Pruebe con archivos conocidos**: Use videos de referencia para verificar la configuración

## Próximos Pasos

Ahora que tiene una configuración funcional, explore estos temas avanzados:

1. **Procesamiento por Lotes**: Procese múltiples archivos eficientemente
2. **Integración con Base de Datos**: Almacene huellas en una base de datos
3. **Procesamiento en Tiempo Real**: Genere huellas desde streams en vivo
4. **Interfaz Personalizada**: Construya interfaces gráficas para sus aplicaciones
5. **Optimización de Rendimiento**: Ajuste para su caso de uso específico

### Lectura Recomendada

- [Documentación de la API C++](api.md) - Referencia completa de la API
- [Entendiendo Video Fingerprinting](../understanding-video-fingerprinting.md) - Fundamentos técnicos
- [Casos de Uso](../use-cases.md) - Aplicaciones del mundo real

## Proyectos de Ejemplo

El SDK incluye tres proyectos de ejemplo completos:

### vfp_gen - Generación de Huellas

Demuestra cómo generar huellas con varias opciones:

```bash
vfp_gen source.mp4 output.sig 0 0 0
```

### vfp_compare - Comparación de Video

Muestra cómo comparar dos videos por similitud:

```bash
vfp_compare video1.sig video2.sig 100 10
```

### vfp_search - Búsqueda de Fragmentos

Ilustra la búsqueda de fragmentos de video:

```bash
vfp_search needle.sig haystack.sig 300
```

Estudie estos ejemplos para entender las mejores prácticas y patrones comunes.

## Soporte y Recursos

### Obtener Ayuda

- **Email de Soporte**: <support@visioforge.com>
- **Comunidad Discord**: Únase para ayuda y discusiones en tiempo real
- **Ejemplos en GitHub**: Muestras de código adicionales e integraciones

### Reportar Problemas

Al reportar problemas, por favor proporcione:

1. Versión del SDK y plataforma
2. Ejemplo de código mínimo que reproduzca el problema
3. Mensajes de error y stack traces
4. Archivos de video de muestra (si aplica)
5. Especificaciones del sistema

## Conclusión

Ahora tiene todo lo necesario para empezar a construir aplicaciones de video fingerprinting con el SDK C++. El SDK proporciona funcionalidad potente con excelente rendimiento, adecuado tanto para aplicaciones de escritorio como para despliegues en servidor.

Recuerde:

- Comenzar con los ejemplos proporcionados
- Probar exhaustivamente con su contenido objetivo
- Optimizar parámetros para su caso de uso
- Solicitar soporte cuando lo necesite

¡Feliz codificación con VisioForge Video Fingerprinting SDK!
