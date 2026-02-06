---
title: Empezando con SDK de Video Fingerprinting C++
description: Instalar y configurar SDK de Video Fingerprinting C++ con configuración Visual Studio, configuración de proyecto y guía de implementación paso a paso.
---

# Empezando con SDK de Video Fingerprinting C++

¡Bienvenido al SDK de Video Fingerprinting de VisioForge para C++! Esta guía completa lo guiará a través de todo lo que necesita para comenzar, desde instalación hasta su primera aplicación funcionando. Al final de esta guía, tendrá una base sólida para construir aplicaciones de video fingerprinting de alto rendimiento en C++.

> **Nota:** Si ya está familiarizado con el SDK .NET, encontrará que el SDK C++ sigue conceptos similares con beneficios de rendimiento nativo. Vea nuestra [Comparación de SDK](../index.es.md#soporte-de-plataformas-e-integracion) para detalles.

## Resumen de Inicio Rápido

Si está buscando ponerse en marcha rápidamente:

1. Descargar el paquete SDK desde VisioForge
2. Extraer los archivos a su directorio de proyecto
3. Incluir los headers: `#include <VisioForge_VFP.h>` y `#include <VisioForge_VFP_Types.h>`
4. Vincular la biblioteca apropiada: `VisioForge_VideoFingerprinting.lib` (x86) o `VisioForge_VideoFingerprinting_x64.lib` (x64)
5. Copiar DLLs de runtime a su directorio de salida
6. Establecer su clave de licencia (si comprada): `VFPSetLicenseKey(L"clave-licencia");`
7. Generar su primera huella digital usando los ejemplos a continuación

## Prerrequisitos y Requisitos del Sistema

Para requisitos detallados del sistema incluyendo plataformas soportadas, especificaciones de hardware y consideraciones de rendimiento, por favor vea nuestra guía completa de [Requisitos del Sistema](../system-requirements.es.md).

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
│   ├── VisioForge_VFP.h           # Header API principal
│   └── VisioForge_VFP_Types.h     # Definiciones de tipos
├── lib/
│   ├── VisioForge_VideoFingerprinting.lib      # Biblioteca import x86
│   └── VisioForge_VideoFingerprinting_x64.lib  # Biblioteca import x64
├── redist/
│   ├── VisioForge_VideoFingerprinting.dll      # DLL runtime x86
│   ├── VisioForge_VideoFingerprinting_x64.dll  # DLL runtime x64
│   ├── VisioForge_FFMPEG_Source.dll           # Soporte de medios x86
│   └── VisioForge_FFMPEG_Source_x64.dll       # Soporte de medios x64
├── demos/
│   ├── vfp_gen/        # Demo de generación de huella digital
│   ├── vfp_compare/    # Demo de comparación de video
│   └── vfp_search/     # Demo de búsqueda de fragmento
└── README.txt
```

## Configurando Su Entorno de Desarrollo

### Configuración de Visual Studio (Windows)

#### Paso 1: Crear un Nuevo Proyecto

1. Abrir Visual Studio 2019 o posterior
2. Hacer clic en "Crear un nuevo proyecto"
3. Seleccionar "App de Consola" (C++)
4. Nombrar su proyecto (ej. "VFPExample")
5. Elegir una ubicación y hacer clic en "Crear"

#### Paso 2: Configurar Propiedades del Proyecto

1. Hacer clic derecho en su proyecto en el Explorador de Soluciones
2. Seleccionar "Propiedades"
3. Configurar las siguientes configuraciones:

**Propiedades de Configuración → C/C++ → General:**

```
Directorios de Inclusión Adicionales: $(ProjectDir)include
```

**Propiedades de Configuración → Vinculador → General:**

```
Directorios de Biblioteca Adicionales: $(ProjectDir)lib
```

**Propiedades de Configuración → Vinculador → Entrada:**

```
Dependencias Adicionales (x86): VisioForge_VideoFingerprinting.lib
Dependencias Adicionales (x64): VisioForge_VideoFingerprinting_x64.lib
```

#### Paso 3: Agregar Archivos SDK

1. Copiar la carpeta `include` a su directorio de proyecto
2. Copiar la carpeta `lib` a su directorio de proyecto
3. Copiar archivos DLL desde `redist` a su directorio de salida (ej. `Debug` o `Release`)

#### Paso 4: Configurar Eventos Post-Build

Agregar un evento post-build para copiar DLLs automáticamente:

```batch
xcopy /y "$(ProjectDir)redist\*.dll" "$(OutDir)"
```

### Configuración de CMake (Cross-Platform)

Crear un archivo `CMakeLists.txt`:

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Encontrar archivos SDK
set(VFP_SDK_PATH "${CMAKE_CURRENT_SOURCE_DIR}/sdk")

# Directorios de inclusión
include_directories(${VFP_SDK_PATH}/include)

# Configuración específica de plataforma
if(WIN32)
    # Configuración Windows
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
    # Configuración macOS
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
elseif(UNIX)
    # Configuración Linux
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
endif()

# Agregar ejecutable
add_executable(vfp_example main.cpp)

# Vincular bibliotecas
target_link_libraries(vfp_example ${VFP_LIBRARIES})

# Copiar bibliotecas runtime en Windows
if(WIN32)
    foreach(DLL ${VFP_RUNTIME_LIBS})
        add_custom_command(TARGET vfp_example POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            ${DLL} $<TARGET_FILE_DIR:vfp_example>)
    endforeach()
endif()
```

Build el proyecto:

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

#### Configuración de Build

Crear un Makefile simple:

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

#### Instalar Command Line Tools de Xcode

```bash
xcode-select --install
```

#### Configuración de Build

Crear un script de build `build.sh`:

```bash
#!/bin/bash

# Configuración del compilador
CXX=clang++
CXXFLAGS="-std=c++11 -Wall -I./include"
LDFLAGS="-L./lib -lVisioForge_VideoFingerprinting"

# Establecer ruta de biblioteca
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Build
$CXX $CXXFLAGS main.cpp $LDFLAGS -o vfp_example

echo "Build completado. Ejecutar con: ./vfp_example"
```

Hacerlo ejecutable y ejecutar:

```bash
chmod +x build.sh
./build.sh
```

## Su Primera Aplicación

Vamos a crear una aplicación simple que genera una huella digital desde un archivo de video.

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
    std::cout << "SDK de Video Fingerprinting de VisioForge - Primera Aplicación\n";
    std::cout << "========================================================\n\n";
    
    // Verificar argumentos de línea de comandos
    if (argc != 2) {
        std::cout << "Uso: " << argv[0] << " <archivo_video>\n";
        std::cout << "Ejemplo: " << argv[0] << " sample.mp4\n";
        return 1;
    }
    
    // Convertir nombre de archivo a carácter ancho (para compatibilidad Windows)
    wchar_t videoFile[256];
#ifdef _WIN32
    size_t converted;
    mbstowcs_s(&converted, videoFile, argv[1], 256);
#else
    mbstowcs(videoFile, argv[1], 256);
#endif
    
    // Paso 1: Establecer clave de licencia
    std::cout << "Estableciendo clave de licencia...\n";
    VFPSetLicenseKey(L"TRIAL");  // Usar "TRIAL" para evaluación
    
    // Paso 2: Configurar la fuente
    std::cout << "Configurando fuente para: " << argv[1] << "\n";
    VFPFingerprintSource source{};
    VFPFillSource(videoFile, &source);
    
    // Opcional: Configurar parámetros de procesamiento
    // source.StartTime = 0;      // Iniciar desde el principio
    // source.StopTime = 60000;   // Procesar primeros 60 segundos
    
    // Paso 3: Generar huella digital
    std::cout << "Generando huella digital...\n";
    VFPFingerPrint fingerprint{};
    
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
    
    if (error != nullptr) {
        std::wcerr << L"Error: " << error << L"\n";
        return 1;
    }
    
    // Paso 4: Mostrar resultados
    std::cout << "\nHuella digital generada exitosamente!\n";
    std::cout << "=====================================\n";
    std::cout << "Información de Video:\n";
    std::cout << "  Duración: " << (fingerprint.Duration / 1000.0) << " segundos\n";
    std::cout << "  Resolución: " << fingerprint.Width << "x" << fingerprint.Height << "\n";
    std::cout << "  Tasa de Frames: " << fingerprint.FrameRate << " fps\n";
    std::cout << "  Tamaño de Datos: " << fingerprint.DataSize << " bytes\n";
    
    // Paso 5: Guardar huella digital a archivo
    wchar_t outputFile[256];
#ifdef _WIN32
    wcscpy_s(outputFile, videoFile);
    wcscat_s(outputFile, L".vfpsig");
#else
    wcscpy(outputFile, videoFile);
    wcscat(outputFile, L".vfpsig");
#endif
    
    std::cout << "\nGuardando huella digital...\n";
    VFPFingerprintSave(&fingerprint, outputFile);
    
    char outputFileAnsi[256];
#ifdef _WIN32
    size_t convertedOut;
    wcstombs_s(&convertedOut, outputFileAnsi, outputFile, 256);
#else
    wcstombs(outputFileAnsi, outputFile, 256);
#endif
    
    std::cout << "Huella digital guardada en: " << outputFileAnsi << "\n";
    
    std::cout << "\n¡Éxito! Ahora puede usar esta huella digital para:\n";
    std::cout << "  - Comparar con otros videos para similitud\n";
    std::cout << "  - Buscar este video en broadcasts más largos\n";
    std::cout << "  - Construir una base de datos de huellas digitales de video\n";
    
    return 0;
}
```

### Paso 2: Build y Ejecutar

#### Windows (Visual Studio)

1. Presionar F7 para build la solución
2. Copiar un archivo de video de prueba a su directorio de salida
3. Abrir Command Prompt en el directorio de salida
4. Ejecutar: `VFPExample.exe testvideo.mp4`

#### Windows (Línea de Comandos)

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

## Entendiendo Tipos de Huella Digital

El SDK proporciona dos tipos de huella digital optimizados para diferentes casos de uso. Para una explicación completa incluyendo detalles técnicos, características de rendimiento y guía de decisión, vea nuestra [Guía de Tipos de Huella Digital](../fingerprint-types.es.md).

**Referencia Rápida:**
- **Huellas Digitales de Búsqueda** (`VFPSearch_GetFingerprintForVideoFile()`): Para encontrar fragmentos de video dentro de videos más largos (detección de comerciales, monitoreo de contenido)
- **Huellas Digitales de Comparación** (`VFPCompare_GetFingerprintForVideoFile()`): Para comparar videos enteros para similitud (detección de duplicados, comparación de versiones)

## Casos de Uso Comunes

### Caso de Uso 1: Detección de Duplicados

```cpp
// Generar huellas digitales para dos videos
VFPFingerPrint fp1{}, fp2{};
// ... generar huellas digitales ...

// Compararlas
double difference = VFPCompare_Compare(
    fp1.Data, fp1.DataSize,
    fp2.Data, fp2.DataSize,
    10  // Permitir hasta 10 segundos de shift
);

if (difference < 100) {
    std::cout << "Los videos son duplicados\n";
}
```

### Caso de Uso 2: Detección de Comerciales

```cpp
// Cargar huellas digitales de comercial y broadcast
VFPFingerPrint commercial{}, broadcast{};
VFPFingerprintLoad(&commercial, L"commercial.vfpsig");
VFPFingerprintLoad(&broadcast, L"broadcast.vfpsig");

// Buscar comercial
double diff;
int position = VFPSearch_Search2(
    &commercial, 0,
    &broadcast, 0,
    &diff, 300  // umbral
);

if (position != INT_MAX) {
    std::cout << "Comercial encontrado en: " << position << " segundos\n";
}
```

### Caso de Uso 3: Verificación de Contenido

```cpp
// Generar huella digital con áreas ignoradas para logos
VFPFingerprintSource source{};
VFPFillSource(L"video.mp4", &source);

// Ignorar áreas de logo
source.IgnoredAreas[0] = {0, 0, 200, 100};        // Logo superior izquierda
source.IgnoredAreas[1] = {1720, 980, 1920, 1080}; // Marca de agua inferior derecha

VFPFingerPrint fingerprint{};
VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
```

## Licenciamiento

### Modo de Prueba

Para evaluación, use la licencia de prueba:

```cpp
VFPSetLicenseKey(L"TRIAL");
```

Limitaciones del modo de prueba:

- Agrega marca de agua a frames procesados
- Limitado a 60 segundos de procesamiento por sesión
- Funcionalidad completa disponible de lo contrario

### Licencia Comercial

Para uso en producción, compre una licencia desde VisioForge:

```cpp
VFPSetLicenseKey(L"SU-CLAVE-DE-LICENCIA-AQUI");
```

Tipos de licencia:

- **Licencia de Desarrollador**: Para desarrollo y pruebas
- **Licencia de Deployment**: Para distribución con su aplicación
- **Licencia de Servidor**: Para deployments basados en servidor

## Solución de Problemas

### Problemas y Soluciones Comunes

#### Problema: DLL No Encontrada

**Error**: "El programa no puede iniciar porque VisioForge_VideoFingerprinting.dll no se encuentra"

**Solución**:

- Asegurar que DLLs estén en el mismo directorio que su ejecutable
- O agregar el directorio DLL a su variable de entorno PATH
- Verificar que esté usando la arquitectura correcta (x86 vs x64)

#### Problema: Formato de Video No Soportado

**Error**: "No se puede procesar archivo de video"

**Solución**:

- Asegurar que el códec de video esté soportado (H.264, H.265, VP8, VP9, AV1)
- Instalar paquetes de códec adicionales si es necesario
- Intentar convertir el video a un formato estándar

#### Problema: Clave de Licencia No Funciona

**Error**: "Clave de licencia inválida"

**Solución**:

- Verificar que la clave de licencia esté escrita correctamente
- Asegurar que VFPSetLicenseKey() sea llamado antes de cualquier otra función SDK
- Verificar que la licencia no haya expirado
- Verificar que esté usando la licencia correcta para su plataforma

#### Problema: Violación de Acceso a Memoria

**Error**: Violación de acceso leyendo ubicación

**Solución**:

- Inicializar todas las estructuras con {} antes de usar
- Verificar que los punteros sean válidos antes de usar
- Asegurar tamaños apropiados de buffer de string
- No liberar memoria asignada por SDK manualmente

#### Problema: Rendimiento Pobre

**Síntoma**: Procesamiento más lento de lo esperado

**Solución**:

- Usar configuración de build Release, no Debug
- Habilitar optimizaciones del compilador (/O2 o -O2)
- Procesar videos desde SSD local, no unidad de red
- Considerar usar múltiples hilos para procesamiento por lotes
- Reducir resolución de video si calidad permite

### Consejos de Debug

1. **Habilitar salida de debug**: Establecer `VFPAnalyzer.DebugDir` para guardar resultados intermedios
2. **Verificar valores de retorno**: Siempre verificar retornos de error/null
3. **Usar builds de debug**: Desarrollar inicialmente con símbolos de debug
4. **Registrar operaciones**: Agregar logging para rastrear flujo de procesamiento
5. **Probar con archivos conocidos**: Usar videos de referencia para verificar configuración

## Próximos Pasos

Ahora que tiene una configuración funcionando, explore estos temas avanzados:

1. **Procesamiento por Lotes**: Procesar múltiples archivos eficientemente
2. **Integración de Base de Datos**: Almacenar huellas digitales en una base de datos
3. **Procesamiento en Tiempo Real**: Generar huellas digitales desde streams en vivo
4. **UI Personalizada**: Construir interfaces gráficas para sus aplicaciones
5. **Optimización de Rendimiento**: Ajustar para su caso de uso específico

### Lectura Recomendada

- [Documentación API C++](api.md) - Referencia API completa
- [Entendiendo Video Fingerprinting](../understanding-video-fingerprinting.es.md) - Antecedentes técnicos
- [Casos de Uso](../use-cases.es.md) - Aplicaciones del mundo real

## Proyectos de Muestra

El SDK incluye tres proyectos de muestra completos:

### vfp_gen - Generación de Huella Digital

Demuestra cómo generar huellas digitales con varias opciones:

```bash
vfp_gen source.mp4 output.sig 0 0 0
```

### vfp_compare - Comparación de Video

Muestra cómo comparar dos videos para similitud:

```bash
vfp_compare video1.sig video2.sig 100 10
```

### vfp_search - Búsqueda de Fragmento

Ilustra búsqueda de fragmentos de video:

```bash
vfp_search needle.sig haystack.sig 300
```

Estudie estos ejemplos para entender mejores prácticas y patrones comunes.

## Soporte y Recursos

### Obtener Ayuda

- **Email de Soporte**: <support@visioforge.com>
- **Comunidad Discord**: Únase para ayuda en tiempo real y discusiones
- **Muestras GitHub**: Muestras de código adicionales e integraciones

### Reportar Problemas

Cuando reporte problemas, por favor proporcione:

1. Versión SDK y plataforma
2. Ejemplo de código mínimo reproduciendo el problema
3. Mensajes de error y stack traces
4. Archivos de video de muestra (si aplicable)
5. Especificaciones del sistema

## Conclusión

Ahora tiene todo lo necesario para comenzar a construir aplicaciones de video fingerprinting con el SDK C++. El SDK proporciona funcionalidad poderosa con excelente rendimiento, adecuada tanto para aplicaciones de escritorio como deployments de servidor.

Recuerde:

- Comenzar con las muestras proporcionadas
- Probar exhaustivamente con su contenido objetivo
- Optimizar parámetros para su caso de uso
- Contactar soporte cuando sea necesario

¡Feliz codificación con SDK de Video Fingerprinting de VisioForge!

````