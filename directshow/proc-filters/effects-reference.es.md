---
title: Filtros de Procesamiento - Referencia de Efectos
description: Referencia de 35+ efectos de video DirectShow en tiempo real: filtros de color, desentrelazado, eliminación de ruido y efectos artísticos.
---

# Referencia Completa de Efectos de Video

## Descripción General

El Paquete de Filtros de Procesamiento DirectShow proporciona más de 35 efectos de video en tiempo real que se pueden aplicar individualmente o encadenar juntos. Esta referencia documenta todos los efectos disponibles, sus parámetros y uso.

## Categorías de Efectos

- **Texto y Gráficos** - Logotipos de texto, superposiciones gráficas
- **Filtros de Color** - Filtros rojo, verde, azul, escala de grises
- **Ajuste de Imagen** - Brillo, contraste, saturación
- **Efectos Espaciales** - Voltear, espejo, rotar
- **Efectos Artísticos** - Mármol, solarizar, posterizar, mosaico
- **Ruido y Calidad** - Algoritmos de eliminación de ruido (CAST, adaptativo, mosquito)
- **Desentrelazado** - Métodos de mezcla, triángulo, CAVT
- **Efectos Creativos** - Desenfoque, sacudida, spray, invertir

---
## Efectos de Texto y Gráficos
### ef_text_logo
Renderiza superposición de texto en video con amplias opciones de personalización.
**Tipo de Efecto**: `CVFEffectType.ef_text_logo`
**Parámetros** (estructura `CVFTextLogoMain`):
| Parámetro | Tipo | Descripción | Predeterminado |
|-----------|------|-------------|----------------|
| `x` | int | Posición X (píxeles) | 0 |
| `y` | int | Posición Y (píxeles) | 0 |
| `text` | BSTR | Texto a mostrar | "" |
| `font_name` | BSTR | Nombre de familia de fuente | "Arial" |
| `font_size` | int | Tamaño de fuente (puntos) | 12 |
| `font_color` | COLORREF | Color del texto (RGB) | 0xFFFFFF (blanco) |
| `font_italic` | BOOL | Estilo cursiva | FALSE |
| `font_bold` | BOOL | Estilo negrita | FALSE |
| `font_underline` | BOOL | Estilo subrayado | FALSE |
| `font_strikeout` | BOOL | Estilo tachado | FALSE |
| `transparent_bg` | BOOL | Fondo transparente | TRUE |
| `bg_color` | COLORREF | Color de fondo | 0x000000 (negro) |
| `transp` | DWORD | Nivel de transparencia (0-255) | 255 (opaco) |
| `align` | CVFTextAlign | Alineación de texto | `al_left` |
| `antialiasing` | CVFTextAntialiasingMode | Modo de suavizado | `am_AntiAlias` |
| `gradient` | BOOL | Habilitar degradado | FALSE |
| `gradientMode` | CVFTextGradientMode | Dirección del degradado | `gm_horizontal` |
| `gradientColor1` | COLORREF | Color inicial del degradado | 0xFFFFFF |
| `gradientColor2` | COLORREF | Color final del degradado | 0x000000 |
| `borderMode` | CVFTextBorderMode | Estilo de borde/contorno | `bm_none` |
| `innerBorderColor` | COLORREF | Color de borde interior | 0x000000 |
| `outerBorderColor` | COLORREF | Color de borde exterior | 0xFFFFFF |
| `innerBorderSize` | int | Ancho de borde interior | 1 |
| `outerBorderSize` | int | Ancho de borde exterior | 1 |
| `DateMode` | BOOL | Mostrar fecha/hora actual | FALSE |
| `DateMask` | BSTR | Cadena de formato de fecha | "" |
**Opciones de Alineación de Texto**:
- `al_left` - Alineado a la izquierda
- `al_center` - Alineado al centro
- `al_right` - Alineado a la derecha
**Modos de Borde**:
- `bm_none` - Sin borde
- `bm_inner` - Contorno interior
- `bm_outer` - Contorno exterior
- `bm_inner_and_outer` - Ambos lados
- `bm_embossed` - Efecto relieve 3D
- `bm_outline` - Contorno estándar
- `bm_filled_outline` - Contorno sólido
- `bm_halo` - Efecto resplandor
**Ejemplo (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_text_logo;
effect.Enabled = TRUE;
effect.TextLogo.x = 10;
effect.TextLogo.y = 10;
effect.TextLogo.text = SysAllocString(L"Transmisión en Vivo");
effect.TextLogo.font_name = SysAllocString(L"Arial");
effect.TextLogo.font_size = 32;
effect.TextLogo.font_color = RGB(255, 255, 255);
effect.TextLogo.font_bold = TRUE;
effect.TextLogo.borderMode = bm_outline;
effect.TextLogo.outerBorderColor = RGB(0, 0, 0);
effect.TextLogo.outerBorderSize = 2;
pEffects->add_effect(effect);
```
---

### ef_graphic_logo

Superpone una imagen (logotipo, marca de agua) en el video.

**Tipo de Efecto**: `CVFEffectType.ef_graphic_logo`

**Parámetros** (estructura `CVFGraphicLogoMain`):

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `x` | UINT32 | Posición X (píxeles) |
| `y` | UINT32 | Posición Y (píxeles) |
| `Filename` | BSTR | Ruta al archivo de imagen (BMP, PNG, JPG) |
| `hBmp` | int | Identificador de mapa de bits (alternativa al nombre de archivo) |
| `StretchMode` | CVFStretchMode | Cómo escalar la imagen |
| `TranspLevel` | int | Nivel de transparencia (0-255) |
| `UseColorKey` | BOOL | Habilitar transparencia por clave de color |
| `ColorKey` | COLORREF | Color para hacer transparente |

**Modos de Estiramiento**:
- `sm_none` - Tamaño original
- `sm_stretch` - Estirar para ajustar
- `sm_letterbox` - Ajustar con relación de aspecto
- `sm_crop` - Recortar para ajustar

**Ejemplo (C#)**:
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_graphic_logo,
    Enabled = true,
    GraphicLogo = new CVFGraphicLogoMain
    {
        Filename = @"C:\Images\logo.png",
        x = 20,
        y = 20,
        StretchMode = (int)CVFStretchMode.sm_none,
        TranspLevel = 200,
        UseColorKey = false
    }
};

effectsInterface.add_effect(effect);
```

---
## Efectos de Filtro de Color
### ef_blue
Aplica filtro de color azul (realza el azul, reduce otros colores).
**Tipo de Efecto**: `CVFEffectType.ef_blue`
**Parámetros**:
- `pAmountI` - Intensidad del filtro (0-100, predeterminado: 50)
**Casos de Uso**:
- Tinte azul artístico
- Atmósfera fría
- Escenas de agua/océano
---

### ef_green

Aplica filtro de color verde.

**Tipo de Efecto**: `CVFEffectType.ef_green`

**Parámetros**:
- `pAmountI` - Intensidad del filtro (0-100)

**Casos de Uso**:
- Efecto de visión nocturna
- Escenas de bosque/naturaleza
- Efecto estilo Matrix

---
### ef_red
Aplica filtro de color rojo.
**Tipo de Efecto**: `CVFEffectType.ef_red`
**Parámetros**:
- `pAmountI` - Intensidad del filtro (0-100)
**Casos de Uso**:
- Atmósfera cálida
- Efecto de atardecer
- Escenas de alerta/peligro
---

### ef_filter_blue / ef_filter_blue_2

Filtrado azul avanzado con diferentes algoritmos.

**Tipo de Efecto**: `CVFEffectType.ef_filter_blue` o `ef_filter_blue_2`

**Diferencia**: `ef_filter_blue_2` utiliza matemáticas de color alternativas para diferentes resultados visuales.

---
### ef_filter_green / ef_filter_green2
Filtrado verde avanzado (dos variantes).
**Tipos de Efecto**: `CVFEffectType.ef_filter_green`, `ef_filter_green2`
---

### ef_filter_red / ef_filter_red2

Filtrado rojo avanzado (dos variantes).

**Tipos de Efecto**: `CVFEffectType.ef_filter_red`, `ef_filter_red2`

---
### ef_greyscale
Convierte video a blanco y negro.
**Tipo de Efecto**: `CVFEffectType.ef_greyscale`
**Parámetros**: Ninguno (conversión completa a escala de grises)
**Casos de Uso**:
- Aspecto de película clásica
- Efecto artístico
- Reducir ruido de color
**Ejemplo (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_greyscale;
effect.Enabled = TRUE;
pEffects->add_effect(effect);
```
---

### ef_invert

Invierte todos los colores (imagen negativa).

**Tipo de Efecto**: `CVFEffectType.ef_invert`

**Parámetros**: Ninguno

**Casos de Uso**:
- Efecto artístico
- Apariencia de rayos X
- Efectos visuales especiales

---
## Efectos de Ajuste de Imagen
### ef_contrast
Ajusta el contraste de la imagen.
**Tipo de Efecto**: `CVFEffectType.ef_contrast`
**Parámetros**:
- `pAmountI` - Ajuste de contraste (-100 a +100)
  - Negativo: Disminuir contraste
  - Positivo: Aumentar contraste
  - Predeterminado: 0 (sin cambios)
**Ejemplo (C#)**:
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_contrast,
    Enabled = true,
    pAmountI = 25  // Aumentar contraste en 25%
};
```
---

### ef_lightness

Ajusta el brillo general.

**Tipo de Efecto**: `CVFEffectType.ef_lightness`

**Parámetros**:
- `pAmountI` - Ajuste de brillo (-100 a +100)
  - Negativo: Oscurecer
  - Positivo: Aclarar
  - Predeterminado: 0

---
### ef_darkness
Oscurece la imagen (opuesto a brillo).
**Tipo de Efecto**: `CVFEffectType.ef_darkness`
**Parámetros**:
- `pAmountI` - Cantidad de oscuridad (0-100)
---

### ef_saturation

Ajusta la saturación de color.

**Tipo de Efecto**: `CVFEffectType.ef_saturation`

**Parámetros**:
- `pAmountI` - Ajuste de saturación (-100 a +100)
  - -100: Escala de grises
  - 0: Colores originales
  - +100: Hiper-saturado

**Casos de Uso**:
- Colores vivos para contenido promocional
- Desaturar para aspecto apagado
- Gradación de color

---
## Efectos Espaciales
### ef_flip_down
Voltea el video verticalmente (cabeza abajo).
**Tipo de Efecto**: `CVFEffectType.ef_flip_down`
**Parámetros**: Ninguno
**Casos de Uso**:
- Corregir cámara invertida
- Efecto espejo con rotación
- Efectos especiales
---

### ef_flip_right

Voltea el video horizontalmente (espejo).

**Tipo de Efecto**: `CVFEffectType.ef_flip_right`

**Parámetros**: Ninguno

**Casos de Uso**:
- Modo espejo de cámara web
- Corregir cámara reflejada
- Efectos de simetría

---
### ef_mirror_down
Crea efecto espejo vertical (arriba se refleja abajo).
**Tipo de Efecto**: `CVFEffectType.ef_mirror_down`
---

### ef_mirror_right

Crea efecto espejo horizontal (izquierda se refleja a derecha).

**Tipo de Efecto**: `CVFEffectType.ef_mirror_right`

---
## Efectos Artísticos
### ef_blur
Aplica desenfoque gaussiano a la imagen.
**Tipo de Efecto**: `CVFEffectType.ef_blur`
**Parámetros**:
- `pAmountI` - Cantidad de desenfoque (0-100)
- `pSizeI` - Tamaño del núcleo de desenfoque (1-20)
**Casos de Uso**:
- Desenfoque de fondo (simulación de profundidad de campo)
- Suavizar imagen
- Privacidad (desenfocar rostros, matrículas)
**Ejemplo (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_blur;
effect.Enabled = TRUE;
effect.pAmountI = 50;  // 50% fuerza de desenfoque
effect.pSizeI = 10;    // radio de desenfoque de 10 píxeles
pEffects->add_effect(effect);
```
---

### ef_marble

Crea efecto de textura de mármol/remolino.

**Tipo de Efecto**: `CVFEffectType.ef_marble`

**Parámetros**:
- `pAmountD` - Intensidad del efecto (0.0-1.0)
- `pTurbulenceI` - Cantidad de turbulencia (0-100)
- `pScaleD` - Factor de escala (0.1-10.0)

**Casos de Uso**:
- Fondo artístico
- Efectos de transición
- Visuales psicodélicos

---
### ef_posterize
Reduce el número de colores (efecto arte póster).
**Tipo de Efecto**: `CVFEffectType.ef_posterize`
**Parámetros**:
- `pAmountI` - Niveles de color (2-256)
  - Valores más bajos: Menos colores, más dramático
  - Valores más altos: Más colores, efecto sutil
**Casos de Uso**:
- Estilo arte pop
- Efecto cómic
- Reducir profundidad de color
---

### ef_mosaic

Crea efecto pixelado/mosaico.

**Tipo de Efecto**: `CVFEffectType.ef_mosaic`

**Parámetros**:
- `pSizeI` - Tamaño de bloque de mosaico (2-100 píxeles)

**Casos de Uso**:
- Privacidad (desenfocar rostros/identidades)
- Estilo pixel art retro
- Censura

**Ejemplo (C#)**:
```csharp
var effect = new CVFEffect
{
    Type = (int)CVFEffectType.ef_mosaic,
    Enabled = true,
    pSizeI = 15  // bloques de 15x15 píxeles
};
```

---
### ef_solarize
Crea efecto de solarización (inversión parcial de color).
**Tipo de Efecto**: `CVFEffectType.ef_solorize` (nota ortografía)
**Parámetros**:
- `pAmountI` - Umbral de solarización (0-255)
**Casos de Uso**:
- Efecto de fotografía artística
- Aspecto retro
- Transiciones creativas
---

### ef_spray

Crea efecto de spray/salpicadura de pintura.

**Tipo de Efecto**: `CVFEffectType.ef_spray`

**Parámetros**:
- `pAmountI` - Intensidad del spray (0-100)

---
### ef_shake_down
Simula efecto de sacudida de cámara verticalmente.
**Tipo de Efecto**: `CVFEffectType.ef_shake_down`
**Parámetros**:
- `pAmountI` - Intensidad de sacudida (0-100)
**Casos de Uso**:
- Efecto terremoto
- Vibración de impacto
- Simulación de cámara en mano
---

## Efectos de Procesamiento de Ruido

### ef_denoise_cast

Algoritmo de eliminación de ruido CAST (Combined Adaptive Spatial-Temporal).

**Tipo de Efecto**: `CVFEffectType.ef_denoise_cast`

**Parámetros** (estructura `CVFDenoiseCAST`):

| Parámetro | Rango | Predeterminado | Descripción |
|-----------|-------|----------------|-------------|
| `TemporalDifferenceThreshold` | 0-255 | 16 | Umbral de detección de movimiento |
| `NumberOfMotionPixelsThreshold` | 0-16 | 0 | Píxeles mínimos para movimiento |
| `StrongEdgeThreshold` | 0-255 | 8 | Preservación de bordes |
| `BlockWidth` | 1-16 | 4 | Ancho de bloque de procesamiento |
| `BlockHeight` | 1-16 | 4 | Alto de bloque de procesamiento |
| `EdgePixelWeight` | 0-255 | 128 | Peso de mezcla de bordes |
| `NonEdgePixelWeight` | 0-255 | 16 | Peso de área suave |
| `GaussianThresholdY` | int | 12 | Umbral de ruido Luma |
| `GaussianThresholdUV` | int | 6 | Umbral de ruido Croma |
| `HistoryWeight` | 0-255 | 192 | Fuerza de filtrado temporal |

**Casos de Uso**:
- Limpieza de video con poca luz
- Reducción de ruido de cámara web
- Eliminación de artefactos de compresión

**Ejemplo (C++)**:
```cpp
CVFEffect effect;
effect.Type = ef_denoise_cast;
effect.Enabled = TRUE;

// Reducción de ruido moderada
effect.DenoiseCAST.TemporalDifferenceThreshold = 20;
effect.DenoiseCAST.StrongEdgeThreshold = 10;
effect.DenoiseCAST.GaussianThresholdY = 15;
effect.DenoiseCAST.GaussianThresholdUV = 8;

pEffects->add_effect(effect);
```

---
### ef_denoise_adaptive
Reducción de ruido adaptativa que se ajusta al contenido de la imagen.
**Tipo de Efecto**: `CVFEffectType.ef_denoise_adaptive`
**Parámetros**:
- `pDenoiseAdaptiveThreshold` - Umbral de ruido (0-100)
- `pDenoiseAdaptiveBlurMode` - Método de desenfoque (0-2)
**Casos de Uso**:
- Reducción de ruido general
- Limpieza de video
- Mejora de calidad
---

### ef_denoise_mosquito

Reduce el ruido mosquito (artefactos de compresión alrededor de los bordes).

**Tipo de Efecto**: `CVFEffectType.ef_denoise_mosquito`

**Parámetros**:
- `pAmountI` - Fuerza de reducción (0-100)

**Casos de Uso**:
- Limpiar video muy comprimido
- Eliminar artefactos MPEG/H.264
- Post-procesamiento para transmisión

---
### ef_color_noise
Añade ruido de color (grano) a la imagen.
**Tipo de Efecto**: `CVFEffectType.ef_color_noise`
**Parámetros**:
- `pAmountI` - Cantidad de ruido (0-100)
**Casos de Uso**:
- Efecto de grano de película
- Aspecto retro/vintage
- Textura artística
---

### ef_mono_noise

Añade ruido monocromático (blanco y negro).

**Tipo de Efecto**: `CVFEffectType.ef_mono_noise`

**Parámetros**:
- `pAmountI` - Cantidad de ruido (0-100)

---
## Efectos de Desentrelazado
### ef_deint_blend
Mezcla campos entrelazados juntos.
**Tipo de Efecto**: `CVFEffectType.ef_deint_blend`
**Parámetros** (estructura `CVFDeintBlend`):
| Parámetro | Rango | Predeterminado | Descripción |
|-----------|-------|----------------|-------------|
| `blendThresh1` | 0-255 | 5 | Primer umbral de mezcla |
| `blendThresh2` | 0-255 | 9 | Segundo umbral de mezcla |
| `blendConstants1` | 0.0-1.0 | 0.3 | Primer peso de mezcla |
| `blendConstants2` | 0.0-1.0 | 0.7 | Segundo peso de mezcla |
**Casos de Uso**:
- Desentrelazar video analógico
- Eliminar artefactos de peine
- Convertir entrelazado a progresivo
---

### ef_deint_triangle

Desentrelazado por interpolación triangular.

**Tipo de Efecto**: `CVFEffectType.ef_deint_triangle`

**Parámetros**:
- `pDeintTriangleWeight` - Peso de interpolación (0-100)

**Calidad**: Mejor preservación de bordes que la mezcla

---
### ef_deint_cavt
Desentrelazado CAVT (Content Adaptive Vertical Temporal).
**Tipo de Efecto**: `CVFEffectType.ef_deint_cavt`
**Parámetros**:
- `pDeintCAVTThreshold` - Umbral de movimiento (0-100)
**Calidad**: Mejor calidad, más intensivo en CPU
**Casos de Uso**:
- Desentrelazado de alta calidad
- Conversión de video de difusión
- Procesamiento de archivo
---

## Encadenamiento de Efectos

Se pueden aplicar múltiples efectos simultáneamente. Los efectos se procesan en el orden en que se agregaron.

**Ejemplo: Mejora de Transmisión Profesional**:
```cpp
// 1. Denoise
CVFEffect denoise;
denoise.Type = ef_denoise_adaptive;
denoise.Enabled = TRUE;
denoise.pDenoiseAdaptiveThreshold = 15;
pEffects->add_effect(denoise);

// 2. Color correction
CVFEffect saturation;
saturation.Type = ef_saturation;
saturation.Enabled = TRUE;
saturation.pAmountI = 15;  // Ligero aumento de saturación
pEffects->add_effect(saturation);

// 3. Add branding
CVFEffect logo;
logo.Type = ef_graphic_logo;
logo.Enabled = TRUE;
logo.GraphicLogo.Filename = SysAllocString(L"logo.png");
logo.GraphicLogo.x = 20;
logo.GraphicLogo.y = 20;
pEffects->add_effect(logo);

// 4. Add timestamp
CVFEffect timestamp;
timestamp.Type = ef_text_logo;
timestamp.Enabled = TRUE;
timestamp.TextLogo.DateMode = TRUE;
timestamp.TextLogo.DateMask = SysAllocString(L"%Y-%m-%d %H:%M:%S");
timestamp.TextLogo.x = 20;
timestamp.TextLogo.y = 1050;  // Abajo izquierda
pEffects->add_effect(timestamp);
```

---
## Consideraciones de Rendimiento
### Uso de CPU por Efecto
**Bajo Impacto** (< 5% CPU):
- Filtros de color
- Escala de grises
- Invertir
- Voltear/Espejo
**Impacto Medio** (5-15% CPU):
- Superposiciones de texto/gráficos
- Contraste/brillo
- Posterizar
- Desentrelazado simple
**Alto Impacto** (15-40% CPU):
- Desenfoque (radio grande)
- Denoise (CAST, adaptativo)
- Mosaico (bloques pequeños)
- Mármol/efectos artísticos
### Consejos de Optimización
1. **Limitar efectos simultáneos** - Cada efecto añade tiempo de procesamiento
2. **Usar parámetros apropiados** - Radio de desenfoque más grande = más CPU
3. **Deshabilitar efectos no utilizados** - Establecer `Enabled = FALSE` en lugar de eliminar
4. **Procesar a menor resolución** - Reducir escala, aplicar efectos, aumentar escala
5. **Usar renderizado por GPU cuando sea posible** - Verificar efectos acelerados por GPU
---

## Combinaciones de Efectos Comunes

### Mejora de Cámara Web
```
1. ef_denoise_adaptive (umbral: 15)
2. ef_contrast (cantidad: +10)
3. ef_saturation (cantidad: +15)
4. ef_flip_right (modo espejo)
```

### Aspecto de Película Vintage
```
1. ef_greyscale
2. ef_contrast (cantidad: +20)
3. ef_mono_noise (cantidad: 15)
```

### Calidad de Difusión
```
1. ef_deint_cavt
2. ef_denoise_mosquito (cantidad: 20)
3. ef_saturation (cantidad: +5)
```

### Modo de Privacidad
```
1. ef_mosaic (tamaño: 20) en región específica
2. ef_blur (cantidad: 80) como alternativa
```

---
## Ver También
- [Descripción General del Paquete de Filtros de Procesamiento](index.md)
- [Referencia de Interfaz de Efectos de Video](interfaces/effects-interface.md)
- [Interfaz de Clave de Croma](interfaces/chroma-key.md)
- [Interfaz de Mezclador de Video](interfaces/video-mixer.md)
- [Ejemplos de Código](examples.md)
