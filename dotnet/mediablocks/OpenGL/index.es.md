---
title: Efectos de Video OpenGL en Media Blocks SDK .NET
description: Aplicar efectos de video hardware-acelerados OpenGL con GLVideoEffectsBlock para manipulación de video en tiempo real en pipelines de Media Blocks SDK.
sidebar_label: Efectos OpenGL
---

# Efectos de Video OpenGL - SDK de VisioForge Media Blocks .Net

[SDK de Media Blocks .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Los efectos de video OpenGL en el SDK de VisioForge Media Blocks .Net permiten manipulación poderosa y hardware-acelerada de streams de video. Estos efectos pueden aplicarse a contenido de video procesado dentro de un contexto OpenGL, típicamente vía bloques como `GLVideoEffectsBlock` o pipelines de renderizado OpenGL personalizados. Esta guía cubre los efectos disponibles, sus ajustes de configuración y otros tipos OpenGL relacionados.

## Efecto Base: `GLBaseVideoEffect`

Todos los efectos de video OpenGL heredan de la clase `GLBaseVideoEffect`, que proporciona propiedades y eventos comunes.

| Propiedad | Tipo                  | Descripción                                      |
|----------|-----------------------|--------------------------------------------------|
| `Name`   | `string`              | El nombre interno del efecto (solo lectura).     |
| `ID`     | `GLVideoEffectID`     | El identificador único para el efecto (solo lectura). |
| `Index`  | `int`                 | El índice del efecto en una cadena.              |

**Eventos:**

- `OnUpdate`: Ocurre cuando las propiedades del efecto necesitan actualizarse en el pipeline. Llame a `OnUpdateCall()` para activarlo.

## Efectos de Video Disponibles

Esta sección detalla los diversos efectos de video OpenGL que puede usar. Estos efectos se agregan típicamente a un `GLVideoEffectsBlock` o un elemento de procesamiento OpenGL similar.

### Efecto Alfa (`GLAlphaVideoEffect`)

Reemplaza un color seleccionado con un canal alfa o establece/ajusta el canal alfa existente.

**Propiedades:**

| Propiedad           | Tipo                     | Valor Predeterminado    | Descripción                                            |
|--------------------|--------------------------|-------------------------|--------------------------------------------------------|
| `Alpha`            | `double`                 | `1.0`                   | El valor para el canal alfa.                           |
| `Angle`            | `float`                  | `20`                    | El tamaño del cubo de color a cambiar (sensibilidad de radio para coincidencia de color). |
| `BlackSensitivity` | `uint`                   | `100`                   | La sensibilidad a colores oscuros.                     |
| `Mode`             | `GLAlphaVideoEffectMode` | `Set`                   | El método usado para modificación alfa.                |
| `NoiseLevel`       | `float`                  | `2`                     | El tamaño del radio de ruido (píxeles a ignorar alrededor del color coincidente). |
| `CustomColor`      | `SKColor`                | `SKColors.Green`        | Valor de color personalizado para modo chroma key `Custom`.       |
| `WhiteSensitivity` | `uint`                   | `100`                   | La sensibilidad a colores brillantes.                  |

**Enum Asociado: `GLAlphaVideoEffectMode`**

Define el modo de operación para el efecto de video Alfa.

| Valor    | Descripción                            |
|----------|----------------------------------------|
| `Set`    | Establecer/ajustar canal alfa directamente usando la propiedad `Alpha`. |
| `Green`  | Chroma Key en verde puro.              |
| `Blue`   | Chroma Key en azul puro.               |
| `Custom` | Chroma Key en el color especificado por `CustomColor`. |

### Efecto Desenfoque (`GLBlurVideoEffect`)

Aplica un efecto de desenfoque usando convolución separable 9x9. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Burbuja (`GLBulgeVideoEffect`)

Crea una distorsión de burbuja en el video. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Balance de Color (`GLColorBalanceVideoEffect`)

Ajusta el balance de color del video, incluyendo brillo, contraste, tono y saturación.

**Propiedades:**

| Propiedad     | Tipo     | Valor Predeterminado | Descripción                                      |
|--------------|----------|----------------------|--------------------------------------------------|
| `Brightness` | `double` | `0`                  | Ajusta brillo (-1.0 a 1.0, 0 significa sin cambio). |
| `Contrast`   | `double` | `1`                  | Ajusta contraste (0.0 a infinito, 1 significa sin cambio). |
| `Hue`        | `double` | `0`                  | Ajusta tono (-1.0 a 1.0, 0 significa sin cambio).    |
| `Saturation` | `double` | `1`                  | Ajusta saturación (0.0 a infinito, 1 significa sin cambio). |

### Efecto Desentrelazado (`GLDeinterlaceVideoEffect`)

Aplica un filtro de desentrelazado al video.

**Propiedades:**

| Propiedad | Tipo                  | Valor Predeterminado   | Descripción                         |
|----------|-----------------------|-----------------------|-------------------------------------|
| `Method` | `GLDeinterlaceMethod` | `VerticalBlur`        | El método de desentrelazado a usar. |

**Enum Asociado: `GLDeinterlaceMethod`**

Define el método para el efecto de video Desentrelazado.

| Valor          | Descripción                             |
|----------------|-----------------------------------------|
| `VerticalBlur` | Método de desenfoque vertical.          |
| `MAAD`         | Adaptativo de Movimiento: Detección Avanzada. |

### Efecto Ojo de Pez (`GLFishEyeVideoEffect`)

Aplica una distorsión de lente ojo de pez. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Voltear (`GLFlipVideoEffect`)

Voltea o rota el video.

**Propiedades:**

| Propiedad | Tipo                | Valor Predeterminado | Descripción                         |
|----------|---------------------|---------------------|-------------------------------------|
| `Method` | `GLFlipVideoMethod` | `None`              | El método de volteo o rotación a usar. |

**Enum Asociado: `GLFlipVideoMethod`**

Define el método de volteo o rotación de video.

| Valor                | Descripción                                  |
|----------------------|----------------------------------------------|
| `None`               | Sin rotación.                                |
| `Clockwise`          | Rotar 90 grados en sentido horario.          |
| `Rotate180`          | Rotar 180 grados.                            |
| `CounterClockwise`   | Rotar 90 grados en sentido antihorario.      |
| `HorizontalFlip`     | Voltear horizontalmente.                     |
| `VerticalFlip`       | Voltear verticalmente.                       |
| `UpperLeftDiagonal`  | Voltear a través de diagonal superior izquierda/inferior derecha. |
| `UpperRightDiagonal` | Voltear a través de diagonal superior derecha/inferior izquierda. |

### Efecto Iluminación de Brillo (`GLGlowLightingVideoEffect`)

Agrega un efecto de iluminación de brillo al video. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Escala de Grises (`GLGrayscaleVideoEffect`)

Convierte el video a escala de grises. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Calor (`GLHeatVideoEffect`)

Aplica un efecto tipo firma de calor al video. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Laplaciano (`GLLaplacianVideoEffect`)

Aplica un filtro de detección de bordes Laplaciano.

**Propiedades:**

| Propiedad | Tipo    | Valor Predeterminado | Descripción                                                       |
|----------|---------|----------------------|-------------------------------------------------------------------|
| `Invert` | `bool`  | `false`              | Si `true`, invierte colores para obtener bordes oscuros en un fondo brillante. |

### Efecto Túnel de Luz (`GLLightTunnelVideoEffect`)

Crea un efecto visual de túnel de luz. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Procesamiento Cruzado Luma (`GLLumaCrossProcessingVideoEffect`)

Aplica un efecto de procesamiento cruzado luma (a menudo "xpro"). Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Espejo (`GLMirrorVideoEffect`)

Aplica un efecto espejo al video. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Cambiar Tamaño (`GLResizeVideoEffect`)

Cambia el tamaño del video a las dimensiones especificadas.

**Propiedades:**

| Propiedad | Tipo  | Descripción                            |
|----------|-------|----------------------------------------|
| `Width`  | `int` | El ancho objetivo para el cambio de tamaño de video. |
| `Height` | `int` | La altura objetivo para el cambio de tamaño de video.|

### Efecto Sepia (`GLSepiaVideoEffect`)

Aplica un tono sepia al video. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Sin City (`GLSinCityVideoEffect`)

Aplica un efecto estilo película Sin City (escala de grises con resaltes rojos). Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Sobel (`GLSobelVideoEffect`)

Aplica un filtro de detección de bordes Sobel.

**Propiedades:**

| Propiedad | Tipo    | Valor Predeterminado | Descripción                                                       |
|----------|---------|----------------------|-------------------------------------------------------------------|
| `Invert` | `bool`  | `false`              | Si `true`, invierte colores para obtener bordes oscuros en un fondo brillante. |

### Efecto Cuadrado (`GLSquareVideoEffect`)

Aplica un efecto de distorsión o pixelación "cuadrado". Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Aplastar (`GLSqueezeVideoEffect`)

Aplica un efecto de distorsión de aplastamiento. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Estirar (`GLStretchVideoEffect`)

Aplica un efecto de distorsión de estiramiento. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Transformación (`GLTransformationVideoEffect`)

Aplica transformaciones 3D al video, incluyendo rotación, escalado y traslación.

**Propiedades:**

| Propiedad       | Tipo    | Valor Predeterminado | Descripción                                                           |
|----------------|---------|----------------------|-----------------------------------------------------------------------|
| `FOV`          | `float` | `90.0f`              | Ángulo de campo de visión en grados para proyección de perspectiva.   |
| `Ortho`        | `bool`  | `false`              | Si `true`, usa proyección ortográfica; de lo contrario, perspectiva.  |
| `PivotX`       | `float` | `0.0f`               | Coordenada X del punto pivote de rotación (0 es centro).              |
| `PivotY`       | `float` | `0.0f`               | Coordenada Y del punto pivote de rotación (0 es centro).              |
| `PivotZ`       | `float` | `0.0f`               | Coordenada Z del punto pivote de rotación (0 es centro).              |
| `RotationX`    | `float` | `0.0f`               | Rotación alrededor del eje X en grados.                               |
| `RotationY`    | `float` | `0.0f`               | Rotación alrededor del eje Y en grados.                               |
| `RotationZ`    | `float` | `0.0f`               | Rotación alrededor del eje Z en grados.                               |
| `ScaleX`       | `float` | `1.0f`               | Multiplicador de escala para el eje X.                                |
| `ScaleY`       | `float` | `1.0f`               | Multiplicador de escala para el eje Y.                                |
| `TranslationX` | `float` | `0.0f`               | Traslación a lo largo del eje X (coordenadas universales [0-1]).      |
| `TranslationY` | `float` | `0.0f`               | Traslación a lo largo del eje Y (coordenadas universales [0-1]).      |
| `TranslationZ` | `float` | `0.0f`               | Traslación a lo largo del eje Z (coordenadas universales [0-1], profundidad). |

### Efecto Remolino (`GLTwirlVideoEffect`)

Aplica un efecto de distorsión de remolino. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

### Efecto Rayos X (`GLXRayVideoEffect`)

Aplica un efecto visual tipo rayos X. Este efecto no tiene propiedades configurables adicionales más allá de las heredadas de `GLBaseVideoEffect`.

## Identificación de Efectos OpenGL: Enum `GLVideoEffectID`

Esta enumeración lista todos los tipos de efectos de video OpenGL disponibles, usados por `GLBaseVideoEffect.ID`.

| Valor            | Descripción                               |
|------------------|-------------------------------------------|
| `ColorBalance`   | El efecto de balance de color.            |
| `Grayscale`      | El efecto de escala de grises.            |
| `Resize`         | El efecto de cambio de tamaño.            |
| `Deinterlace`    | El efecto de desentrelazado.              |
| `Flip`           | El efecto de volteo.                      |
| `Blur`           | Efecto de desenfoque con convolución separable 9x9. |
| `FishEye`        | El efecto de ojo de pez.                  |
| `GlowLighting`   | El efecto de iluminación de brillo.       |
| `Heat`           | El efecto de firma de calor.              |
| `LumaX`          | El efecto de procesamiento cruzado luma.  |
| `Mirror`         | El efecto de espejo.                      |
| `Sepia`          | El efecto de sepia.                       |
| `Square`         | El efecto de cuadrado.                    |
| `XRay`           | El efecto de rayos X.                     |
| `Stretch`        | El efecto de estiramiento.                |
| `LightTunnel`    | El efecto de túnel de luz.                |
| `Twirl`          | El efecto de remolino.                    |
| `Squeeze`        | El efecto de aplastamiento.               |
| `SinCity`        | El efecto de película gris-roja Sin City. |
| `Bulge`          | El efecto de burbuja.                     |
| `Sobel`          | El efecto de Sobel.                       |
| `Laplacian`      | El efecto de Laplaciano.                  |
| `Alpha`          | El efecto de canales alfa.                |
| `Transformation` | El efecto de transformación.              |

## Renderizado y Configuración de Vista OpenGL

Estos tipos asisten en configurar cómo se renderiza o ve el video en un contexto OpenGL, especialmente para escenarios especializados como VR o visualización personalizada.

### Ajustes de Vista Equirectangular (`GLEquirectangularViewSettings`)

Maneja ajustes para renderizar video equirectangular (360 grados), comúnmente usado en aplicaciones VR. Implementa `IVRVideoControl`.

**Propiedades:**

| Propiedad        | Tipo         | Valor Predeterminado           | Descripción                                    |
|-----------------|--------------|-------------------------------|------------------------------------------------|
| `VideoWidth`    | `int`        | (solo lectura)                | Ancho del video fuente.                        |
| `VideoHeight`   | `int`        | (solo lectura)                | Altura del video fuente.                       |
| `FieldOfView`   | `float`      | `80.0f`                       | Campo de visión en grados.                     |
| `Yaw`           | `float`      | `0.0f`                        | Guiñada (rotación alrededor del eje Y) en grados. |
| `Pitch`         | `float`      | `0.0f`                        | Cabeceo (rotación alrededor del eje X) en grados. |
| `Roll`          | `float`      | `0.0f`                        | Alabeo (rotación alrededor del eje Z) en grados. |
| `Mode`          | `VRMode`     | `Equirectangular`             | El modo VR (soporta `Equirectangular`).       |

**Métodos:**

- `IsModeSupported(VRMode mode)`: Verifica si el `VRMode` especificado está soportado.

**Eventos:**

- `SettingsChanged`: Ocurre cuando cualquier ajuste de vista es cambiado.

### Ajustes de Renderizador de Video (`GLVideoRendererSettings`)

Configura propiedades generales para un renderizador de video basado en OpenGL.

**Propiedades:**

| Propiedad           | Tipo                          | Valor Predeterminado     | Descripción                                                          |
|--------------------|-------------------------------|-------------------------|----------------------------------------------------------------------|
| `ForceAspectRatio` | `bool`                        | `true`                  | Si el escalado respetará la relación de aspecto original.            |
| `IgnoreAlpha`      | `bool`                        | `true`                  | Si el canal alfa será ignorado (tratado como negro).                 |
| `PixelAspectRatio` | `System.Tuple<int, int>`      | `(0, 1)`                | Relación de aspecto de píxel del dispositivo de visualización (numerador, denominador). |
| `Rotation`         | `GLVideoRendererRotateMethod` | `None`                  | Especifica la rotación aplicada al video.                           |

**Enum Asociado: `GLVideoRendererRotateMethod`**

Define métodos de rotación para el renderizador de video OpenGL.

| Valor            | Descripción                             |
|------------------|-----------------------------------------|
| `None`           | Sin rotación.                           |
| `_90C`           | Rotar 90 grados en sentido horario.     |
| `_180`           | Rotar 180 grados.                       |
| `_90CC`          | Rotar 90 grados en sentido antihorario. |
| `FlipHorizontal` | Voltear horizontalmente.                |
| `FlipVertical`   | Voltear verticalmente.                  |

## Shaders OpenGL Personalizados

Permite la aplicación de shaders GLSL personalizados al stream de video.

### Definición de Shader (`GLShader`)

Representa un par de shaders de vértice y fragmento.

**Propiedades:**

| Propiedad         | Tipo     | Descripción                                   |
|------------------|----------|-----------------------------------------------|
| `VertexShader`   | `string` | El código fuente GLSL para el shader de vértice. |
| `FragmentShader` | `string` | El código fuente GLSL para el shader de fragmento. |

**Constructores:**
- `GLShader()`
- `GLShader(string vertexShader, string fragmentShader)`

### Ajustes de Shader (`GLShaderSettings`)

Configura shaders GLSL personalizados para usar en el pipeline.

**Propiedades:**

| Propiedad   | Tipo                                 | Descripción                                      |
|------------|--------------------------------------|--------------------------------------------------|
| `Vertex`   | `string`                             | El código fuente GLSL para el shader de vértice. |
| `Fragment` | `string`                             | El código fuente GLSL para el shader de fragmento. |
| `Uniforms` | `System.Collections.Generic.Dictionary<string, object>` | Un diccionario de variables uniformes (parámetros) a pasar a los shaders. |

**Constructores:**
- `GLShaderSettings()`
- `GLShaderSettings(string vertex, string fragment)`
- `GLShaderSettings(GLShader shader)`

## Sobrepasos de Imagen en OpenGL

Proporciona ajustes para sobrepasar imágenes estáticas en un stream de video dentro de un contexto OpenGL.

### Ajustes de Sobrepaso (`GLOverlaySettings`)

Define las propiedades de un sobrepaso de imagen.

**Propiedades:**

| Propiedad   | Tipo     | Valor Predeterminado | Descripción                                       |
|------------|----------|---------------------|---------------------------------------------------|
| `Filename` | `string` | (N/A)               | Ruta al archivo de imagen (solo lectura después de init). |
| `Data`     | `byte[]` | (N/A)               | Datos de imagen como arreglo de bytes (solo lectura después de init).|
| `X`        | `int`    |                     | Coordenada X de la esquina superior izquierda del sobrepaso. |
| `Y`        | `int`    |                     | Coordenada Y de la esquina superior izquierda del sobrepaso. |
| `Width`    | `int`    |                     | Ancho del sobrepaso.                            |
| `Height`   | `int`    |                     | Altura del sobrepaso.                           |
| `Alpha`    | `double` | `1.0`               | Opacidad del sobrepaso (0.0 transparente a 1.0 opaco). |

**Constructor:**
- `GLOverlaySettings(string filename)`

## Mezcla de Video OpenGL

Estos tipos se usan para configurar un mezclador de video basado en OpenGL, permitiendo combinar y componer múltiples streams de video.

### Ajustes de Mezclador (`GLVideoMixerSettings`)

Extiende `VideoMixerBaseSettings` para mezcla específica de OpenGL. Maneja una lista de objetos `GLVideoMixerStream` e hereda propiedades como `Width`, `Height`, y `FrameRate`.

**Métodos:**
- `AddStream(GLVideoMixerStream stream)`: Agrega un stream al mezclador.
- `RemoveStream(GLVideoMixerStream stream)`: Remueve un stream del mezclador.
- `SetStream(int index, GLVideoMixerStream stream)`: Reemplaza un stream en un índice específico.

**Constructores:**
- `GLVideoMixerSettings(int width, int height, VideoFrameRate frameRate)`
- `GLVideoMixerSettings(int width, int height, VideoFrameRate frameRate, List<VideoMixerStream> streams)`

### Stream de Mezclador (`GLVideoMixerStream`)

Extiende `VideoMixerStream` y define propiedades para un stream individual dentro del mezclador de video OpenGL. Hereda `Rectangle`, `ZOrder`, y `Alpha` de `VideoMixerStream`.

**Propiedades:**

| Propiedad                        | Tipo                          | Valor Predeterminado                      | Descripción                                      |
|----------------------------------|-------------------------------|------------------------------------------|--------------------------------------------------|
| `Crop`                           | `Rect`                        | (N/A)                                    | Rectángulo de recorte para el stream de entrada. |
| `BlendConstantColorAlpha`        | `double`                      | `0`                                      | Componente alfa para color de mezcla constante.  |
| `BlendConstantColorBlue`         | `double`                      | `0`                                      | Componente azul para color de mezcla constante.  |
| `BlendConstantColorGreen`        | `double`                      | `0`                                      | Componente verde para color de mezcla constante. |
| `BlendConstantColorRed`          | `double`                      | `0`                                      | Componente rojo para color de mezcla constante.  |
| `BlendEquationAlpha`             | `GLVideoMixerBlendEquation`   | `Add`                                    | Ecuación de mezcla para el canal alfa.           |
| `BlendEquationRGB`               | `GLVideoMixerBlendEquation`   | `Add`                                    | Ecuación de mezcla para canales RGB.             |
| `BlendFunctionDestinationAlpha`  | `GLVideoMixerBlendFunction`   | `OneMinusSourceAlpha`                    | Función de mezcla para alfa de destino.          |
| `BlendFunctionDesctinationRGB`   | `GLVideoMixerBlendFunction`   | `OneMinusSourceAlpha`                    | Función de mezcla para RGB de destino.           |
| `BlendFunctionSourceAlpha`       | `GLVideoMixerBlendFunction`   | `One`                                    | Función de mezcla para alfa de fuente.           |
| `BlendFunctionSourceRGB`         | `GLVideoMixerBlendFunction`   | `SourceAlpha`                            | Función de mezcla para RGB de fuente.            |

**Constructor:**
- `GLVideoMixerStream(Rect rectangle, uint zorder, double alpha = 1.0)`

### Ecuación de Mezcla (`GLVideoMixerBlendEquation` Enum)

Especifica cómo se combinan los colores fuente y destino durante la mezcla.

| Valor             | Descripción                                     |
|-------------------|-------------------------------------------------|
| `Add`             | Fuente + Destino                                |
| `Subtract`        | Fuente - Destino                                |
| `ReverseSubtract` | Destino - Fuente                                |

### Función de Mezcla (`GLVideoMixerBlendFunction` Enum)

Define factores para colores fuente y destino en operaciones de mezcla. (Rs, Gs, Bs, As son componentes de color fuente; Rd, Gd, Bd, Ad son componentes de color destino; Rc, Gc, Bc, Ac son componentes de color constante).

| Valor                      | Descripción                                 |
|----------------------------|---------------------------------------------|
| `Zero`                     | Factor es (0, 0, 0, 0).                     |
| `One`                      | Factor es (1, 1, 1, 1).                     |
| `SourceColor`              | Factor es (Rs, Gs, Bs, As).                 |
| `OneMinusSourceColor`      | Factor es (1-Rs, 1-Gs, 1-Bs, 1-As).         |
| `DestinationColor`         | Factor es (Rd, Gd, Bd, Ad).                 |
| `OneMinusDestinationColor` | Factor es (1-Rd, 1-Gd, 1-Bd, 1-Ad).         |
| `SourceAlpha`              | Factor es (As, As, As, As).                 |
| `OneMinusSourceAlpha`      | Factor es (1-As, 1-As, 1-As, 1-As).         |
| `DestinationAlpha`         | Factor es (Ad, Ad, Ad, Ad).                 |
| `OneMinusDestinationAlpha` | Factor es (1-Ad, 1-Ad, 1-Ad, 1-Ad).         |
| `ConstantColor`            | Factor es (Rc, Gc, Bc, Ac).                 |
| `OneMinusContantColor`     | Factor es (1-Rc, 1-Gc, 1-Bc, 1-Ac).         |
| `ConstantAlpha`            | Factor es (Ac, Ac, Ac, Ac).                 |
| `OneMinusContantAlpha`     | Factor es (1-Ac, 1-Ac, 1-Ac, 1-Ac).         |
| `SourceAlphaSaturate`      | Factor es (min(As, 1-Ad), min(As, 1-Ad), min(As, 1-Ad), 1). |

## Fuentes Virtuales de Prueba para OpenGL

Estas clases de ajustes se usan para configurar fuentes que generan patrones de prueba directamente dentro de un contexto OpenGL.

### Ajustes de Fuente de Video Virtual (`GLVirtualVideoSourceSettings`)

Configura un bloque fuente (`GLVirtualVideoSourceBlock`) que produce datos de video de prueba. Implementa `IMediaPlayerBaseSourceSettings` e `IVideoCaptureBaseVideoSourceSettings`.

**Propiedades:**

| Propiedad    | Tipo                       | Valor Predeterminado                | Descripción                                      |
|-------------|----------------------------|------------------------------------|--------------------------------------------------|
| `Width`     | `int`                      | `1280`                             | Ancho del video de salida.                       |
| `Height`    | `int`                      | `720`                              | Altura del video de salida.                      |
| `FrameRate` | `VideoFrameRate`           | `30/1` (30 fps)                    | Tasa de frames del video de salida.              |
| `IsLive`    | `bool`                     | `true`                             | Indica si la fuente es en vivo.                  |
| `Mode`      | `GLVirtualVideoSourceMode` | (N/A - debe establecerse)          | Especifica el tipo de patrón de prueba a generar. |

**Enum Asociado: `GLVirtualVideoSourceMode`**

Define el patrón de prueba generado por `GLVirtualVideoSourceBlock`.

| Valor         | Descripción                  |
|---------------|------------------------------|
| `SMPTE`       | Barras de color SMPTE 100%.  |
| `Snow`        | Aleatorio (nieve de televisión). |
| `Black`       | 100% Negro.                  |
| `White`       | 100% Blanco.                 |
| `Red`         | Color sólido rojo.           |
| `Green`       | Color sólido verde.          |
| `Blue`        | Color sólido azul.           |
| `Checkers1`   | Patrón de ajedrez (1px).     |
| `Checkers2`   | Patrón de ajedrez (2px).     |
| `Checkers4`   | Patrón de ajedrez (4px).     |
| `Checkers8`   | Patrón de ajedrez (8px).     |
| `Circular`    | Patrón circular.             |
| `Blink`       | Patrón parpadeante.          |
| `Mandelbrot`  | Fractal Mandelbrot.          |

**Métodos:**
- `Task<MediaFileInfo> ReadInfoAsync()`: Lee asincrónicamente información de medios (devuelve información sintética basada en ajustes).
- `MediaBlock CreateBlock()`: Crea una instancia `GLVirtualVideoSourceBlock` configurada con estos ajustes.