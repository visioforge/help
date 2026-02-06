---
title: Filtros de Procesamiento - Interfaz Video Mixer
description: Interfaz IVFVideoMixer para mezclar 2-16 fuentes de video con PIP, chroma keying, transparencia y configuraciones de diseño personalizables.
---

# Referencia de la Interfaz IVFVideoMixer

## Descripción General

La interfaz `IVFVideoMixer` proporciona control integral sobre la mezcla de video multi-fuente en aplicaciones DirectShow. Esta interfaz permite Picture-in-Picture (PIP), composición de video, chroma keying y gestión flexible de diseño para combinar múltiples transmisiones de video en una única salida.

El filtro Video Mixer puede manejar 2-16 fuentes de video de entrada, cada una con configuración independiente de posición, tamaño, transparencia y orden z.

## Definición de la Interfaz

- **Nombre de la Interfaz**: `IVFVideoMixer`
- **GUID**: `{3318300E-F6F1-4d81-8BC3-9DB06B09F77A}`
- **Hereda de**: `IUnknown`
- **Archivo de Cabecera**: `yk_video_mixer_filter_define.h` (C++), `IVFVideoMixer.cs` (.NET)

## Capacidades

- **Pines de Entrada**: 2-16 fuentes de video simultáneas
- **Chroma Keying**: Soporte de pantalla verde/azul por entrada
- **Calidad de Redimensionamiento**: Múltiples algoritmos de interpolación
- **Orden Z**: Control de capas independiente
- **Transparencia**: Mezcla alfa por entrada
- **Posición/Tamaño**: Colocación con precisión de píxeles

---
## Referencia de Métodos
### Gestión de Parámetros de Entrada
#### SetInputParam
Configura parámetros para un pin de entrada específico.
**Sintaxis (C++)**:
```cpp
int SetInputParam(int pin_index, VFPIPVideoInputParam param);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetInputParam([In] int pin_index, [In] VFPIPVideoInputParam param);
```
**Parámetros**:
- `pin_index`: Índice del pin de entrada basado en cero (0 = primera entrada, 1 = segunda, etc.)
- `param`: Estructura que contiene la configuración de entrada (ver abajo)
**Retorna**: `0` en éxito, código de error de lo contrario.
**Estructura VFPIPVideoInputParam**:
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Enabled` | bool | Habilitar/deshabilitar esta entrada |
| `Left` | int | Posición X (píxeles) |
| `Top` | int | Posición Y (píxeles) |
| `Width` | int | Ancho (píxeles) |
| `Height` | int | Alto (píxeles) |
| `Alpha` | int | Transparencia (0-255, 255=opaco) |
| `Visible` | bool | Bandera de visibilidad |
| `ZOrder` | int | Orden de capa (mayor = primer plano) |
| `StretchMode` | VFPIPResizeQuality | Calidad de redimensionamiento |
**Notas de Uso**:
- El Pin 0 típicamente es la fuente de fondo/principal
- Los Pines 1+ son fuentes de superposición
- La posición (0,0) es la esquina superior izquierda
- El tamaño puede diferir de la resolución fuente (escalado automático)
- La mezcla alfa requiere algo de sobrecarga de GPU
**Ejemplo (C++)**:
```cpp
IVFVideoMixer* pMixer = nullptr;
pFilter->QueryInterface(IID_IVFVideoMixer, (void**)&pMixer);
// Configurar segunda entrada (superposición)
VFPIPVideoInputParam param;
param.Enabled = true;
param.Visible = true;
param.Left = 50;
param.Top = 50;
param.Width = 640;
param.Height = 360;
param.Alpha = 255;        // Completamente opaco
param.ZOrder = 10;        // Encima del fondo
pMixer->SetInputParam(1, param);
pMixer->Release();
```
**Ejemplo (C#)**:
```csharp
var mixer = filter as IVFVideoMixer;
// Configurar PIP en esquina inferior derecha
var param = new VFPIPVideoInputParam
{
    Enabled = true,
    Visible = true,
    Left = 1600,          // Asumiendo salida 1920x1080
    Top = 820,
    Width = 320,          // PIP pequeño
    Height = 180,
    Alpha = 255,
    ZOrder = 100          // Capa superior
};
mixer.SetInputParam(1, param);
```
---

#### GetInputParam

Recupera los parámetros actuales para un pin de entrada específico.

**Sintaxis (C++)**:
```cpp
int GetInputParam(int pin_index, VFPIPVideoInputParam *param);
```

**Sintaxis (C#)**:
```csharp
[PreserveSig]
int GetInputParam([In] int pin_index, [Out] out VFPIPVideoInputParam param);
```

**Parámetros**:
- `pin_index`: Índice del pin de entrada basado en cero
- `param`: [out] Recibe la configuración de entrada actual

**Retorna**: `0` en éxito.

**Ejemplo (C++)**:
```cpp
VFPIPVideoInputParam param;
pMixer->GetInputParam(1, &param);

printf("Posición entrada 1: %d,%d\n", param.Left, param.Top);
printf("Tamaño entrada 1: %dx%d\n", param.Width, param.Height);
```

---
#### GetInputParam2
Recupera parámetros por referencia de interfaz de pin en lugar de índice.
**Sintaxis (C++)**:
```cpp
int GetInputParam2(IPin *pin, VFPIPVideoInputParam *param);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int GetInputParam2([In] object pin, [Out] out VFPIPVideoInputParam param);
```
**Parámetros**:
- `pin`: Puntero de interfaz IPin de DirectShow
- `param`: [out] Recibe la configuración de entrada
**Retorna**: `0` en éxito.
**Notas de Uso**:
- Alternativa a GetInputParam cuando tiene referencia de pin
- Útil al enumerar pines dinámicamente
---

### Configuración de Salida

#### SetOutputParam

Configura el formato de video de salida del mezclador.

**Sintaxis (C++)**:
```cpp
int SetOutputParam(VFPIPVideoOutputParam param);
```

**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetOutputParam([In] VFPIPVideoOutputParam param);
```

**Parámetros**:
- `param`: Estructura de configuración de salida

**Estructura VFPIPVideoOutputParam**:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Width` | int | Ancho de salida (píxeles) |
| `Height` | int | Alto de salida (píxeles) |
| `FrameRate` | double | Tasa de cuadros de salida (fps) |
| `BackgroundColor` | COLORREF | Color de fondo (RGB) |

**Notas de Uso**:
- Debe llamarse antes de conectar filtros descendentes
- Todas las entradas se escalan/posicionan relativas al tamaño de salida
- La tasa de cuadros puede diferir de las entradas (el mezclador maneja la temporización)

**Ejemplo (C++)**:
```cpp
VFPIPVideoOutputParam output;
output.Width = 1920;
output.Height = 1080;
output.FrameRate = 30.0;
output.BackgroundColor = RGB(0, 0, 0);  // Fondo negro

pMixer->SetOutputParam(output);
```

**Ejemplo (C#)**:
```csharp
var output = new VFPIPVideoOutputParam
{
    Width = 1280,
    Height = 720,
    FrameRate = 60.0,
    BackgroundColor = 0x003300  // Verde oscuro
};

mixer.SetOutputParam(output);
```

---
#### GetOutputParam
Recupera la configuración de salida actual.
**Sintaxis (C++)**:
```cpp
int GetOutputParam(VFPIPVideoOutputParam *param);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int GetOutputParam([Out] out VFPIPVideoOutputParam param);
```
**Parámetros**:
- `param`: [out] Recibe la configuración de salida
**Retorna**: `0` en éxito.
---

### Configuración de Chroma Key

#### SetChromaSettings

Configura los ajustes de chroma key (pantalla verde/azul) para composición.

**Sintaxis (C++)**:
```cpp
int SetChromaSettings(bool enabled, int color, int tolerance1, int tolerance2);
```

**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetChromaSettings([In, MarshalAs(UnmanagedType.Bool)] bool enabled,
                      int color,
                      int tolerance1,
                      int tolerance2);
```

**Parámetros**:
- `enabled`: Habilitar/deshabilitar chroma keying
- `color`: Color clave (0=verde, 1=azul, 2=rojo, o RGB personalizado)
- `tolerance1`: Tolerancia de coincidencia de color (0-255)
- `tolerance2`: Tolerancia de borde (0-255)

**Retorna**: `0` en éxito.

**Notas de Uso**:
- Se aplica a todas las entradas que tienen color cromático
- Menor tolerancia = coincidencia de color más estricta
- Mayor tolerancia = más colores eliminados (puede afectar al sujeto)
- tolerance2 ayuda con el suavizado de bordes

**Valores de Color Cromático**:
- `0` - Verde (más común)
- `1` - Azul
- `2` - Rojo
- Valor RGB personalizado

**Ejemplo (C++)**:
```cpp
// Habilitar pantalla verde con tolerancia moderada
pMixer->SetChromaSettings(true, 0, 50, 30);
```

**Ejemplo (C#)**:
```csharp
// Pantalla azul con tolerancia ajustada
mixer.SetChromaSettings(true, 1, 30, 20);

// Deshabilitar chroma keying
mixer.SetChromaSettings(false, 0, 0, 0);
```

---
### Gestión de Orden de Capas
#### SetInputOrder
Establece el orden z (orden de capas) para una entrada específica.
**Sintaxis (C++)**:
```cpp
int SetInputOrder(int pin_index, int order);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetInputOrder(int pin_index, int order);
```
**Parámetros**:
- `pin_index`: Índice del pin de entrada basado en cero
- `order`: Valor de orden z (mayor = primer plano)
**Retorna**: `0` en éxito.
**Notas de Uso**:
- Valores de orden más altos se renderizan encima
- Rango típico: 0-100
- Puede cambiarse dinámicamente durante la reproducción
- Alternativa a establecer ZOrder en VFPIPVideoInputParam
**Ejemplo (C++)**:
```cpp
// Fondo
pMixer->SetInputOrder(0, 0);
// Capa intermedia
pMixer->SetInputOrder(1, 50);
// Superposición superior
pMixer->SetInputOrder(2, 100);
```
---

### Configuración de Calidad

#### SetResizeQuality

Establece la calidad/algoritmo de redimensionamiento para todas las entradas.

**Sintaxis (C++)**:
```cpp
int SetResizeQuality(VFPIPResizeQuality quality);
```

**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetResizeQuality(VFPIPResizeQuality quality);
```

**Parámetros**:
- `quality`: Modo de calidad de redimensionamiento

**Enumeración VFPIPResizeQuality**:

| Valor | Algoritmo | Calidad | Velocidad | Caso de Uso |
|-------|-----------|---------|-----------|-------------|
| **NearestNeighbor** | Píxel más cercano | Baja | ★★★★★ | Pixel art, vista previa rápida |
| **Bilinear** | Interpolación lineal | Media | ★★★★☆ | Calidad estándar |
| **Bicubic** | Interpolación cúbica | Alta | ★★★☆☆ | Alta calidad (predeterminado) |
| **Lanczos** | Lanczos-3 | Máxima | ★★☆☆☆ | Calidad profesional |

**Notas de Uso**:
- Bicubic se recomienda para la mayoría de casos de uso
- Lanczos para máxima calidad cuando el rendimiento lo permite
- Bilinear para rendimiento en tiempo real
- NearestNeighbor solo para casos especiales

**Ejemplo (C++)**:
```cpp
// Mezcla de alta calidad
pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);

// Modo de rendimiento
pMixer->SetResizeQuality(VFPIPResizeQuality::Bilinear);
```

---
## Ejemplos de Configuración Completa
### Ejemplo 1: Picture-in-Picture (C++)
```cpp
#include "yk_video_mixer_filter_define.h"
HRESULT ConfigurePIPLayout(IBaseFilter* pMixerFilter)
{
    HRESULT hr;
    IVFVideoMixer* pMixer = nullptr;
    hr = pMixerFilter->QueryInterface(IID_IVFVideoMixer, (void**)&pMixer);
    if (FAILED(hr))
        return hr;
    // Establecer salida 1080p
    VFPIPVideoOutputParam output;
    output.Width = 1920;
    output.Height = 1080;
    output.FrameRate = 30.0;
    output.BackgroundColor = RGB(0, 0, 0);
    pMixer->SetOutputParam(output);
    // Configurar video principal (entrada 0 - fondo)
    VFPIPVideoInputParam main;
    main.Enabled = true;
    main.Visible = true;
    main.Left = 0;
    main.Top = 0;
    main.Width = 1920;
    main.Height = 1080;
    main.Alpha = 255;
    main.ZOrder = 0;        // Fondo
    pMixer->SetInputParam(0, main);
    // Configurar PIP (entrada 1 - esquina inferior derecha)
    VFPIPVideoInputParam pip;
    pip.Enabled = true;
    pip.Visible = true;
    pip.Left = 1560;        // 1920 - 360 (ancho) + margen
    pip.Top = 860;          // 1080 - 220 (alto) + margen
    pip.Width = 360;
    pip.Height = 202;       // Aspecto 16:9
    pip.Alpha = 255;
    pip.ZOrder = 100;       // Primer plano
    pMixer->SetInputParam(1, pip);
    // Redimensionamiento de alta calidad
    pMixer->SetResizeQuality(VFPIPResizeQuality::Bicubic);
    pMixer->Release();
    return S_OK;
}
```
### Ejemplo 2: Pantalla Dividida (C#)
```csharp
using VisioForge.DirectShowAPI;
public class SplitScreenMixer
{
    public void ConfigureSplitScreen(IBaseFilter mixerFilter)
    {
        var mixer = mixerFilter as IVFVideoMixer;
        if (mixer == null)
            throw new NotSupportedException("IVFVideoMixer no disponible");
        // Salida 1920x1080
        var output = new VFPIPVideoOutputParam
        {
            Width = 1920,
            Height = 1080,
            FrameRate = 30.0,
            BackgroundColor = 0x000000
        };
        mixer.SetOutputParam(output);
        // Mitad izquierda - Entrada 0
        var leftInput = new VFPIPVideoInputParam
        {
            Enabled = true,
            Visible = true,
            Left = 0,
            Top = 0,
            Width = 960,        // Mitad del ancho
            Height = 1080,
            Alpha = 255,
            ZOrder = 0
        };
        mixer.SetInputParam(0, leftInput);
        // Mitad derecha - Entrada 1
        var rightInput = new VFPIPVideoInputParam
        {
            Enabled = true,
            Visible = true,
            Left = 960,         // Desplazado por la mitad
            Top = 0,
            Width = 960,
            Height = 1080,
            Alpha = 255,
            ZOrder = 0
        };
        mixer.SetInputParam(1, rightInput);
        mixer.SetResizeQuality(VFPIPResizeQuality.Bicubic);
    }
}
```
### Ejemplo 3: Superposición con Chroma Key (C++)
```cpp
HRESULT ConfigureChromaKeyOverlay(IVFVideoMixer* pMixer)
{
    // Salida 1080p
    VFPIPVideoOutputParam output;
    output.Width = 1920;
    output.Height = 1080;
    output.FrameRate = 30.0;
    output.BackgroundColor = RGB(0, 0, 0);
    pMixer->SetOutputParam(output);
    // Escena de fondo (entrada 0)
    VFPIPVideoInputParam background;
    background.Enabled = true;
    background.Visible = true;
    background.Left = 0;
    background.Top = 0;
    background.Width = 1920;
    background.Height = 1080;
    background.Alpha = 255;
    background.ZOrder = 0;
    pMixer->SetInputParam(0, background);
    // Persona frente a pantalla verde (entrada 1)
    VFPIPVideoInputParam subject;
    subject.Enabled = true;
    subject.Visible = true;
    subject.Left = 400;
    subject.Top = 100;
    subject.Width = 1120;
    subject.Height = 880;
    subject.Alpha = 255;
    subject.ZOrder = 10;
    pMixer->SetInputParam(1, subject);
    // Habilitar chroma keying de pantalla verde
    pMixer->SetChromaSettings(
        true,   // Habilitar
        0,      // Verde
        60,     // Tolerancia de color
        40      // Tolerancia de borde
    );
    // Alta calidad para mejores bordes de chroma key
    pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);
    return S_OK;
}
```
### Ejemplo 4: Cuadrícula Multi-Cámara (C#)
```csharp
public void Configure2x2Grid(IVFVideoMixer mixer)
{
    // Salida 1920x1080
    var output = new VFPIPVideoOutputParam
    {
        Width = 1920,
        Height = 1080,
        FrameRate = 30.0,
        BackgroundColor = 0x101010  // Gris oscuro
    };
    mixer.SetOutputParam(output);
    int cellWidth = 960;
    int cellHeight = 540;
    int gap = 10;
    // Cámara superior izquierda (entrada 0)
    mixer.SetInputParam(0, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = gap,
        Top = gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    // Cámara superior derecha (entrada 1)
    mixer.SetInputParam(1, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = cellWidth + gap,
        Top = gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    // Cámara inferior izquierda (entrada 2)
    mixer.SetInputParam(2, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = gap,
        Top = cellHeight + gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    // Cámara inferior derecha (entrada 3)
    mixer.SetInputParam(3, new VFPIPVideoInputParam
    {
        Enabled = true,
        Visible = true,
        Left = cellWidth + gap,
        Top = cellHeight + gap,
        Width = cellWidth - gap * 2,
        Height = cellHeight - gap * 2,
        Alpha = 255,
        ZOrder = 0
    });
    mixer.SetResizeQuality(VFPIPResizeQuality.Bicubic);
}
```
---

## Escenarios Comunes de Mezcla

### Escenario 1: Estilo Transmisión de Noticias

```
+------------------------------------------+
|                                          |
|     Cámara Principal (pantalla completa) |
|                                          |
|                           +-----------+  |
|                           |  Cámara   |  |
|                           |  Invitado |  |
|                           +-----------+  |
+------------------------------------------+
```

**Configuración**:
- Entrada 0: Cámara principal (1920x1080)
- Entrada 1: PIP de invitado (320x180, inferior derecha)
- Orden Z: Invitado encima
- Calidad de Redimensionamiento: Bicubic

### Escenario 2: Transmisión de Videojuegos

```
+------------------------------------------+
|                                          |
|        Captura de Juego (principal)      |
|                                          |
|  +----------+                            |
|  | Webcam   |                            |
|  +----------+                            |
+------------------------------------------+
```

**Configuración**:
- Entrada 0: Captura de juego (1920x1080)
- Entrada 1: Webcam (280x210, superior izquierda)
- Opcional: Chroma key si la webcam tiene pantalla verde
- Orden Z: Webcam encima

### Escenario 3: Producción Virtual

```
+------------------------------------------+
|                                          |
|    Escena de Fondo (pre-renderizada)     |
|                                          |
|         [Persona con pantalla verde      |
|          compuesta encima]               |
|                                          |
+------------------------------------------+
```

**Configuración**:
- Entrada 0: Fondo virtual
- Entrada 1: Cámara con pantalla verde
- Chroma key: Habilitado, verde, tolerancia 60/40
- Calidad de Redimensionamiento: Lanczos para mejor calidad de bordes

---
## Consideraciones de Rendimiento
### Uso de CPU/GPU
**Configuraciones de Bajo Impacto**:
- 2-4 entradas
- Redimensionamiento bilineal
- Sin chroma keying
- Sin transparencia (Alpha = 255)
**Impacto Medio**:
- 5-8 entradas
- Redimensionamiento bicúbico
- Chroma keying básico
- Algo de transparencia
**Alto Impacto**:
- 9+ entradas
- Redimensionamiento Lanczos
- Chroma keying complejo
- Múltiples capas transparentes
### Consejos de Optimización
1. **Usar calidad de redimensionamiento apropiada**:
   - Vista previa: Bilinear
   - Producción: Bicubic
   - Máxima calidad: Lanczos (si el rendimiento lo permite)
2. **Minimizar sobrecarga de chroma keying**:
   - Solo habilitar cuando sea necesario
   - Usar valores de tolerancia ajustados
   - Considerar alternativa acelerada por hardware
3. **Limitar número de entradas**:
   - Cada entrada añade sobrecarga de procesamiento
   - Deshabilitar entradas no usadas (Enabled = false)
4. **Coincidir resoluciones de fuente**:
   - Menos escalado = mejor rendimiento
   - Pre-escalar fuentes si es posible
---

## Mejores Prácticas

### Diseño de Layouts

1. **Planificar orden z cuidadosamente** - Fondo más bajo, superposiciones más altas
2. **Dejar márgenes** - No posicionar elementos en bordes exactos
3. **Mantener relaciones de aspecto** - Evitar distorsión
4. **Probar a resolución objetivo** - Verificar precisión de posicionamiento

### Chroma Keying

1. **Iluminación apropiada** - Iluminación uniforme en pantalla verde
2. **Ajustar tolerancia** - Comenzar bajo, aumentar gradualmente
3. **Configuración de calidad** - Usar Lanczos para mejores bordes
4. **Probar condiciones** - Diferentes escenarios de iluminación

### Cambios Dinámicos

1. **Actualizar parámetros suavemente** - Evitar cambios de posición abruptos
2. **Cachear configuraciones** - Almacenar presets para cambio rápido
3. **Validar parámetros** - Verificar límites antes de aplicar
4. **Manejar errores** - Verificar valores de retorno

---
## Solución de Problemas
### Problema: Video No Aparece
**Verificar**:
- `Enabled = true`
- `Visible = true`
- `Alpha > 0`
- Posición dentro de los límites de salida
- Filtro fuente está ejecutándose
### Problema: Mala Calidad de Escalado
**Solución**:
```cpp
pMixer->SetResizeQuality(VFPIPResizeQuality::Lanczos);
```
### Problema: Chroma Key No Funciona
**Verificar**:
- Configuración de chroma habilitada
- Color correcto seleccionado (0=verde, 1=azul)
- Aumentar valores de tolerancia
- Verificar que la fuente tiene pantalla verde uniforme
**Ejemplo**:
```cpp
// Probar tolerancia más alta
pMixer->SetChromaSettings(true, 0, 80, 60);
```
### Problema: Problemas de Rendimiento
**Soluciones**:
- Reducir número de entradas activas
- Usar calidad de redimensionamiento más rápida
- Deshabilitar chroma keying si no es necesario
- Pre-escalar fuentes de entrada
---

## Interfaces Relacionadas

- **IBaseFilter** - Interfaz de filtro DirectShow
- **IPin** - Interfaz de pin DirectShow (para GetInputParam2)
- **IVFEffects45** - Efectos de video (puede combinarse con mezclador)
- **IVFChromaKey** - Interfaz dedicada de chroma key

## Vea También

- [Descripción General del Pack de Filtros de Procesamiento](../index.es.md)
- [Referencia de Efectos](../effects-reference.es.md)
- [Interfaz Chroma Key](chroma-key.es.md)
- [Ejemplos de Código](../examples.es.md)
