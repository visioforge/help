---
title: Filtros de Procesamiento - Interfaz Chroma Key
description: Interfaz IVFChromaKey para composición de pantalla verde y azul con control de tolerancia y reemplazo de fondo en DirectShow.
---

# Referencia de la Interfaz IVFChromaKey

## Descripción General

La interfaz `IVFChromaKey` proporciona capacidades profesionales de composición chroma key (pantalla verde/pantalla azul) para aplicaciones DirectShow. Esta interfaz permite el reemplazo de fondo en tiempo real haciendo transparentes colores específicos, permitiendo que sujetos filmados frente a fondos de colores se compongan sobre diferentes fondos.

El chroma keying es esencial para producción virtual, pronósticos del tiempo, efectos de video y cualquier escenario donde se necesite reemplazo de fondo.

## Definición de la Interfaz

- **Nombre de la Interfaz**: `IVFChromaKey`
- **GUID**: `{AF6E8208-30E3-44f0-AAFE-787A6250BAB3}`
- **Hereda de**: `IUnknown`
- **Archivo de Cabecera**: `vf_eff_intf.h` (C++), `IVFChromaKey.cs` (.NET)

## Capacidades

- **Colores Clave**: Verde, azul, rojo o colores RGB personalizados
- **Ajuste de Contraste**: Umbrales de contraste bajo/alto separados
- **Reemplazo de Fondo**: Imagen estática o fondo de video
- **Procesamiento en Tiempo Real**: Acelerado por hardware cuando está disponible
- **Calidad de Bordes**: Tolerancia ajustable para bordes suaves

---
## Referencia de Métodos
### Configuración de Umbral de Contraste
#### chroma_put_contrast
Establece el rango de umbral de contraste para el chroma keying.
**Sintaxis (C++)**:
```cpp
HRESULT chroma_put_contrast(int low, int high);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int chroma_put_contrast(int low, int high);
```
**Parámetros**:
- `low`: Umbral de contraste bajo (0-255)
  - Valores más bajos = eliminar más colores similares
  - Valores más altos = coincidencia de color más estricta
- `high`: Umbral de contraste alto (0-255)
  - Define el límite superior para la coincidencia de color
  - Crea un rango de colores clave aceptables
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- Estos valores definen el rango de similitud de color para el keying
- El rango entre `low` y `high` crea un degradado para suavizado de bordes
- Rangos típicos:
  - Keying ajustado: low=10, high=30
  - Keying estándar: low=30, high=70
  - Keying amplio: low=50, high=120
- Ajustar según las condiciones de iluminación y calidad del fondo
**Cómo Funciona**:
```
Píxeles con distancia cromática < low    → Completamente transparentes
Píxeles con distancia cromática > high   → Completamente opacos
Píxeles entre low y high                 → Parcialmente transparentes (degradado)
```
**Ejemplo (C++)**:
```cpp
IVFChromaKey* pChroma = nullptr;
pFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChroma);
// Configuración estándar de pantalla verde
pChroma->chroma_put_contrast(40, 80);
pChroma->Release();
```
**Ejemplo (C#)**:
```csharp
var chroma = filter as IVFChromaKey;
if (chroma != null)
{
    // Keying ajustado para pantalla verde limpia
    chroma.chroma_put_contrast(20, 50);
}
```
---

### Selección de Color

#### chroma_put_color

Establece el color chroma key que se hará transparente.

**Sintaxis (C++)**:
```cpp
HRESULT chroma_put_color(int color);
```

**Sintaxis (C#)**:
```csharp
[PreserveSig]
int chroma_put_color(int color);
```

**Parámetros**:
- `color`: Valor del color chroma key

**Valores de Color** (enumeración CVFChromaColor):

| Valor | Color | Equivalente RGB | Caso de Uso |
|-------|-------|-----------------|-------------|
| `0` (Chroma_Green) | Verde | 0x00FF00 | Chroma key estándar (más común) |
| `1` (Chroma_Blue) | Azul | 0x0000FF | Alternativa al verde |
| `2` (Chroma_Red) | Rojo | 0xFF0000 | Casos especiales |
| RGB Personalizado | Cualquier color | 0xRRGGBB | Coincidencia de color específica |

**Retorna**: `S_OK` (0) en éxito.

**Notas de Uso**:
- Verde es estándar para chroma keying (la piel humana tiene menos verde)
- Azul usado cuando hay objetos verdes en la escena
- Puede usar valor RGB personalizado para coincidencia de color específica
- El color debe ser uniforme en todo el fondo para mejores resultados

**Ejemplo (C++)**:
```cpp
// Usar chroma key verde
pChroma->chroma_put_color(Chroma_Green);

// Usar chroma key azul
pChroma->chroma_put_color(Chroma_Blue);

// Usar color personalizado (ej. magenta)
pChroma->chroma_put_color(0xFF00FF);
```

**Ejemplo (C#)**:
```csharp
// Pantalla verde estándar
chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);

// Pantalla azul
chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);

// Verde-amarillo personalizado
chroma.chroma_put_color(0x88FF00);
```

---
### Imagen de Fondo
#### chroma_put_image
Establece una imagen de fondo de reemplazo para las áreas transparentes.
**Sintaxis (C++)**:
```cpp
HRESULT chroma_put_image(BSTR filename);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int chroma_put_image([MarshalAs(UnmanagedType.BStr)] string filename);
```
**Parámetros**:
- `filename`: Ruta al archivo de imagen de fondo (BMP, PNG, JPG, etc.)
**Retorna**: `S_OK` (0) en éxito.
**Notas de Uso**:
- La imagen se estira para llenar todo el cuadro
- Usar NULL o cadena vacía para usar fondo de video en su lugar
- La imagen estática es más eficiente que el fondo de video
- La imagen se carga una vez y se almacena en caché
- Formatos soportados: BMP, PNG, JPEG, GIF, TIFF
**Ejemplo (C++)**:
```cpp
// Establecer imagen de fondo de oficina
pChroma->chroma_put_image(L"C:\\Backgrounds\\office.jpg");
// Eliminar imagen de fondo (usar entrada de video en su lugar)
pChroma->chroma_put_image(NULL);
```
**Ejemplo (C#)**:
```csharp
// Fondo de estudio virtual
chroma.chroma_put_image(@"C:\Backgrounds\studio.png");
// Eliminar fondo estático
chroma.chroma_put_image(null);
```
---

## Ejemplos de Configuración Completa

### Ejemplo 1: Configuración Básica de Pantalla Verde (C++)

```cpp
#include "vf_eff_intf.h"

HRESULT ConfigureBasicGreenScreen(IBaseFilter* pChromaFilter)
{
    HRESULT hr;
    IVFChromaKey* pChroma = nullptr;

    hr = pChromaFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChroma);
    if (FAILED(hr))
        return hr;

    // Establecer verde como color clave
    pChroma->chroma_put_color(Chroma_Green);

    // Umbrales de contraste estándar
    pChroma->chroma_put_contrast(40, 80);

    // Establecer imagen de fondo
    pChroma->chroma_put_image(L"C:\\Backgrounds\\office_background.jpg");

    pChroma->Release();
    return S_OK;
}
```

### Ejemplo 2: Estudio de Pronóstico del Tiempo (C#)

```csharp
using System;
using VisioForge.DirectShowAPI;

public class WeatherStudioSetup
{
    public void ConfigureWeatherChromaKey(IBaseFilter chromaFilter)
    {
        var chroma = chromaFilter as IVFChromaKey;
        if (chroma == null)
            throw new NotSupportedException("IVFChromaKey no disponible");

        // Pantalla azul para mapas del tiempo
        chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);

        // Umbrales más ajustados para keying limpio
        chroma.chroma_put_contrast(25, 60);

        // Fondo del mapa del tiempo
        chroma.chroma_put_image(@"C:\Weather\maps\current_radar.png");
    }

    public void UpdateWeatherMap(IVFChromaKey chroma, string mapPath)
    {
        // Actualizar dinámicamente el fondo durante la transmisión
        chroma.chroma_put_image(mapPath);
    }
}
```

### Ejemplo 3: Producción Virtual con Color Personalizado (C++)

```cpp
HRESULT ConfigureVirtualProduction(IVFChromaKey* pChroma)
{
    // Usar verde específico que coincida con su fondo físico
    // Medir el color real con un selector de color
    COLORREF customGreen = RGB(60, 220, 40);  // Tono verde específico

    pChroma->chroma_put_color(customGreen);

    // Umbrales de grado profesional
    // Valores más bajos para pantalla verde limpia y bien iluminada
    pChroma->chroma_put_contrast(15, 45);

    // Usar entorno virtual pre-renderizado
    pChroma->chroma_put_image(L"D:\\VirtualSets\\studio_environment.png");

    return S_OK;
}
```

### Ejemplo 4: Configuración Adaptativa de Chroma Key (C#)

```csharp
public class AdaptiveChromaKey
{
    private IVFChromaKey _chroma;

    public void SetupForLightingConditions(string condition)
    {
        switch (condition.ToLower())
        {
            case "perfect":
                // Pantalla verde limpia y uniformemente iluminada
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(15, 40);
                break;

            case "good":
                // Iluminación estándar
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(30, 70);
                break;

            case "challenging":
                // Iluminación desigual o fondo arrugado
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(50, 110);
                break;

            case "outdoor":
                // Luz natural, más difícil de controlar
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);
                _chroma.chroma_put_contrast(60, 130);
                break;
        }
    }

    public void TestThresholds()
    {
        // Comenzar con keying ajustado
        for (int low = 10; low <= 60; low += 10)
        {
            int high = low + 40;
            _chroma.chroma_put_contrast(low, high);

            // El usuario revisa el resultado y selecciona la mejor configuración
            Console.WriteLine($"Probando: Low={low}, High={high}");
            System.Threading.Thread.Sleep(2000);
        }
    }
}
```

---
## Mejores Prácticas de Chroma Keying
### Configuración de Iluminación
1. **Iluminación Uniforme**
   - Usar múltiples fuentes de luz
   - Evitar puntos calientes y sombras en el fondo
   - Mantener color consistente en toda la pantalla
2. **Separación del Sujeto**
   - Posicionar el sujeto a 2-3 metros del fondo
   - Previene el derrame verde sobre el sujeto
   - Permite control de iluminación independiente
3. **Calidad del Fondo**
   - Usar tela o pintura chroma key apropiada
   - Mantener el fondo sin arrugas
   - Mantener saturación de color consistente
### Estrategia de Configuración
1. **Comenzar Conservador**
   ```cpp
   // Comenzar con umbrales ajustados
   pChroma->chroma_put_contrast(20, 50);
   // Aumentar gradualmente si es necesario
   pChroma->chroma_put_contrast(30, 70);
   ```
2. **Probar Diferentes Iluminaciones**
   - Ajustar umbrales para su configuración específica
   - Guardar presets para diferentes condiciones
   - Documentar valores funcionales
3. **Selección de Color**
   - Verde: Opción estándar (menos presente en tonos de piel)
   - Azul: Cuando hay objetos verdes en la escena
   - Personalizado: Coincidir el color real del fondo para mejores resultados
### Optimización de Calidad
1. **Configuración de Cámara**
   - Desactivar balance de blancos automático
   - Enfoque manual
   - Reducir nitidez (previene artefactos en bordes)
2. **Ajuste de Umbrales**
   - Valor bajo: Controla el umbral de transparencia
   - Valor alto: Controla la suavidad del borde
   - Rango más amplio = bordes más suaves
3. **Calidad de Bordes**
   ```
   Rango ajustado (low=20, high=40):
   - Bordes definidos
   - Puede mostrar franja verde
   - Mejor para fondos limpios
   Rango amplio (low=30, high=90):
   - Bordes más suaves
   - Mejor tolerancia al sangrado de color
   - Más tolerante con iluminación imperfecta
   ```
---

## Escenarios Comunes de Chroma Key

### Escenario 1: Producción de Video Corporativo

```cpp
// Entorno de estudio limpio
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(25, 55);
pChroma->chroma_put_image(L"corporate_office.jpg");
```

**Características**:
- Iluminación controlada
- Pantalla verde profesional
- Fondo de oficina estático
- Requisitos de alta calidad

### Escenario 2: Streamer de Videojuegos

```cpp
// Configuración de estudio casero
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(35, 75);
pChroma->chroma_put_image(NULL);  // Usar video del juego como fondo
```

**Características**:
- Pantalla verde de consumidor
- Iluminación variable
- Fondo de video dinámico
- Rendimiento en tiempo real crítico

### Escenario 3: Transmisión del Tiempo

```cpp
// Pantalla azul con mapas meteorológicos
pChroma->chroma_put_color(Chroma_Blue);
pChroma->chroma_put_contrast(30, 65);
pChroma->chroma_put_image(L"weather_map_current.png");
```

**Características**:
- Pantalla azul (verde usado en mapas del tiempo)
- Fondos que cambian dinámicamente
- Iluminación profesional
- Consideraciones de vestimenta del presentador

### Escenario 4: Presentador de Eventos Virtuales

```cpp
// Fondo de conferencia virtual
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(40, 85);
pChroma->chroma_put_image(L"conference_hall.jpg");
```

**Características**:
- Configuración casera/de oficina
- Fondos de calidad variable
- Se necesitan configuraciones tolerantes
- Apariencia profesional deseada

---
## Solución de Problemas
### Problema: Derrame Verde sobre el Sujeto
**Síntomas**: Halo verde o tinte en los bordes del sujeto
**Soluciones**:
1. Aumentar la distancia del sujeto al fondo
2. Ajustar la iluminación para reducir reflejos
3. Usar rango de contraste más ajustado:
   ```cpp
   pChroma->chroma_put_contrast(15, 35);
   ```
4. Considerar corrección de color en post-producción
### Problema: Keying Desigual
**Síntomas**: Partes del fondo no transparentes
**Soluciones**:
1. Verificar uniformidad de iluminación del fondo
2. Aumentar umbral alto:
   ```cpp
   pChroma->chroma_put_contrast(30, 100);
   ```
3. Verificar consistencia del color del fondo
4. Considerar usar coincidencia de color personalizada:
   ```cpp
   // Muestrear el color real del fondo y usarlo
   pChroma->chroma_put_color(0x40DC28);  // Color medido
   ```
### Problema: Partes del Sujeto Desaparecen
**Síntomas**: Ropa o características del sujeto se vuelven transparentes
**Soluciones**:
1. Evitar ropa verde/azul
2. Reducir rango de contraste:
   ```cpp
   pChroma->chroma_put_contrast(50, 90);
   ```
3. Cambiar color clave si es necesario:
   ```cpp
   pChroma->chroma_put_color(Chroma_Blue);  // Si usa verde
   ```
### Problema: Bordes Ásperos y Dentados
**Síntomas**: Mala calidad de bordes, pixelación visible
**Soluciones**:
1. Ampliar rango de contraste para degradado más suave:
   ```cpp
   pChroma->chroma_put_contrast(25, 85);
   ```
2. Mejorar calidad de iluminación
3. Usar fuente de video de mayor calidad
4. Asegurar que el sujeto esté bien separado del fondo
### Problema: Problemas de Rendimiento
**Síntomas**: Cuadros perdidos, tartamudeo
**Soluciones**:
1. Usar fondo de imagen estática en lugar de video
2. Reducir resolución de salida
3. Optimizar valores de umbral (no hacerlos demasiado amplios)
4. Considerar alternativas aceleradas por hardware
---

## Tabla de Referencia de Parámetros

### Guías de Umbral de Contraste

| Calidad de Iluminación | Fondo | Bajo | Alto | Calidad de Bordes | Rendimiento |
|------------------------|-------|------|------|-------------------|-------------|
| **Excelente** | Limpio, uniforme | 15-25 | 35-50 | Definido | Mejor |
| **Buena** | Variaciones menores | 25-35 | 50-75 | Buena | Bueno |
| **Regular** | Algo desigual | 35-50 | 75-100 | Suave | Moderado |
| **Mala** | Desigual/arrugado | 50-70 | 100-140 | Muy suave | Menor |

### Guía de Selección de Color

| Color | RGB | Pros | Contras | Mejor Para |
|-------|-----|------|---------|------------|
| **Verde** | 0x00FF00 | Menos en piel, brillante | No para objetos verdes | Uso general |
| **Azul** | 0x0000FF | Alternativa al verde | Ropa denim/azul | Casos especiales |
| **Personalizado** | Varía | Coincidencia exacta con fondo | Requiere calibración | Profesional |

---
## Integración con Video Mixer
El filtro chroma key se usa frecuentemente con Video Mixer para composición avanzada:
```cpp
// El filtro chroma key elimina el verde
IVFChromaKey* pChroma = /* ... */;
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(30, 70);
// Video mixer combina el sujeto con el fondo
IVFVideoMixer* pMixer = /* ... */;
// Entrada 0: Video de fondo
// Entrada 1: Sujeto con chroma key (fondo transparente)
```
Vea [Interfaz Video Mixer](video-mixer.es.md) para detalles.
---

## Interfaces Relacionadas

- **IVFVideoMixer** - Combinar video con chroma key con fondos
- **IVFEffects45** - Efectos de video adicionales
- **IVFEffectsPro** - Procesamiento de efectos avanzados

## Vea También

- [Descripción General del Pack de Filtros de Procesamiento](../index.es.md)
- [Interfaz Video Mixer](video-mixer.es.md)
- [Referencia de Efectos](../effects-reference.es.md)
- [Ejemplos de Código](../examples.es.md)
