---
title: Renderizador de Video en C# .NET — EVR, Direct2D, WPF, MadVR
description: Configura renderizado de video en C# / .NET en WinForms, WPF y WinUI 3. EVR, Direct2D, madVR, HWND nativo, modos callback — setup y aceleración hardware.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - C#
primary_api_classes:
  - VideoRenderer
  - VideoRendererMode
  - VideoView
  - VideoRendererStretchMode
  - VideoCaptureCoreX

---

# Opciones de Renderizador de Video en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Los motores clásicos (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) exponen **10 modos de renderizador** a través del enum `VideoRendererMode`. Elegir el correcto controla cómo los frames llegan a la pantalla: filtros DirectShow crudos, superficies GPU Direct2D, HWND nativo embebido en WPF, callbacks de frame para renderizado personalizado, controles WinUI 3, o el renderizador externo madVR. Esta guía recorre cada modo con código mínimo de activación, disponibilidad por plataforma, y una tabla de decisión arriba para saltar directo al modo que tu app necesita.

!!! note "Solo motores clásicos"
    Esta página cubre los motores clásicos basados en DirectShow. Los motores multiplataforma `VideoCaptureCoreX` / `MediaPlayerCoreX` usan un control `VideoView` con sinks de GStreamer y no exponen un enum `VideoRendererMode` — el renderizado ahí se maneja automáticamente por el binding del control UI.

## Elección rápida — ¿qué renderizador para qué app?

| Modo | Framework UI | Mejor para |
|---|---|---|
| `VideoRenderer` (GDI clásico) | WinForms | Máxima compatibilidad con hardware antiguo |
| `VMR9` | WinForms | Windows XP / Vista, software + aceleración HW ligera |
| `EVR` | WinForms | Opción por defecto en Windows moderno (Vista+) |
| `Direct2D` | WinForms, WPF | 2D acelerado por GPU, contenido 4K+, apps modernas |
| `Direct2DManaged` | WPF | Direct2D gestionado con pausa-en-minimizar para WPF |
| `WPF_NativeHWND` | WPF | HWND nativo embebido en WPF para más rendimiento que WPF puro |
| `WPF_WinUI_Callback` (`FrameCallback`) | WPF, WinUI, custom | Callbacks por frame para CV, AI, renderizado personalizado |
| `WinUI` | WinUI 3 | Apps WinUI 3 nativas (Windows 10/11) |
| `MadVR` | WinForms | Escalado + color de referencia, requiere instalación externa de madVR |
| `None` | cualquiera | Headless / solo audio / conversión de archivo sin preview |

## Entendiendo las Opciones de Renderizador de Video Disponibles

Las secciones detalladas a continuación describen cada modo, empezando por los tres renderizadores DirectShow clásicos.

### Renderizador de Video Heredado (basado en GDI)

El Video Renderer es la opción más antigua en el ecosistema DirectShow. Se basa en GDI (Graphics Device Interface) para operaciones de dibujo.

**Características clave:**

- Renderizado basado en software sin aceleración de hardware
- Compatible con sistemas y configuraciones más antiguos
- Techo de rendimiento más bajo comparado con alternativas modernas
- Implementación simple con opciones de configuración mínimas

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VideoRenderer;
```

**Cuándo usar:**

- La compatibilidad es la preocupación principal
- La aplicación se dirige a hardware o sistemas operativos más antiguos
- Requisitos mínimos de procesamiento de video
- Solución de problemas con renderizadores más nuevos

### Video Mixing Renderer 9 (VMR9)

VMR9 representa una mejora significativa sobre el renderizador heredado, introduciendo soporte para aceleración de hardware y características avanzadas.

**Características clave:**

- Renderizado acelerado por hardware a través de DirectX 9
- Soporte para mezcla de múltiples flujos de video
- Opciones avanzadas de desentrelazado
- Capacidades de mezcla alfa y composición
- Procesamiento de efectos de video personalizados

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VMR9;
```

**Cuándo usar:**

- Aplicaciones modernas que requieren buen rendimiento
- Se necesitan características de edición o composición de video
- Escenarios de múltiples flujos de video
- Aplicaciones que necesitan equilibrar rendimiento y compatibilidad

### Enhanced Video Renderer (EVR)

EVR es la opción más avanzada, disponible en Windows Vista y sistemas operativos posteriores. Aprovecha el framework Media Foundation en lugar de DirectShow puro.

**Características clave:**

- Últimas tecnologías de aceleración de hardware
- Calidad y rendimiento de video superiores
- Procesamiento mejorado de espacio de color
- Mejor soporte multi-monitor
- Uso más eficiente de CPU
- Mecanismos de sincronización mejorados

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
```

**Cuándo usar:**

- Aplicaciones modernas dirigidas a Windows Vista o posterior
- Se requiere máximo rendimiento y calidad
- Aplicaciones manejando contenido HD o 4K
- Cuando la sincronización avanzada es importante
- Entornos de múltiples pantallas

### Renderizador Direct2D

Direct2D proporciona renderizado 2D de alto rendimiento con aceleración por GPU. Disponible tanto en WinForms como en WPF, es la opción moderna recomendada cuando necesitas renderizado acelerado por hardware con controles simples de rotación, flip y stretch.

**Características clave:**

- Aceleración hardware vía Direct2D / Direct3D 11
- Funciona en WinForms y WPF
- Soporta rotación (0 / 90 / 180 / 270), flip horizontal y vertical
- Integración limpia con modos stretch (`Stretch` / `Letterbox`)
- Bajo overhead de CPU, escala bien a contenido 4K / 8K

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.Direct2D;
VideoCapture1.Video_Renderer.RotationAngle = 0;
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
VideoCapture1.Video_Renderer.Flip_Horizontal = false;
VideoCapture1.Video_Renderer.Flip_Vertical = false;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Cuándo usar:**

- Apps modernas WinForms o WPF que quieran renderizado acelerado por GPU
- Fuentes 4K / 8K donde los caminos basados en CPU serían un cuello de botella
- Apps que necesitan controles de rotación o flip en runtime

### Renderizador Direct2DManaged (WPF)

Variante gestionada de Direct2D específica para WPF. Se integra más limpiamente con el modelo de objetos de WPF y pausa automáticamente el renderizado cuando la ventana se minimiza — útil en apps de reproducción de larga duración donde no quieres trabajo de GPU en ventanas ocultas.

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.Direct2DManaged;
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

Rotación, flip y stretch se comparten con el modo `Direct2D` regular. La pausa-al-minimizar se maneja automáticamente por el control `VideoView` de WPF.

**Cuándo usar:**

- Apps WPF donde quieras rendimiento Direct2D con ciclo de vida compatible con WPF
- Dashboards multi-ventana donde las ventanas inactivas no deben consumir ciclos de GPU

### Renderizador WPF Native HWND

Hospeda un HWND Win32 nativo dentro del control `VideoView` de WPF. Te da el rendimiento crudo del renderizador DirectShow en un layout WPF a costa de los problemas típicos del render-chain WPF (airspace con controles superpuestos).

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.WPF_NativeHWND;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Cuándo usar:**

- Apps WPF que necesitan máximo rendimiento de renderizado DirectShow
- Apps que embeben filtros legacy que esperan un HWND como target
- No necesitas superponer controles WPF sobre la superficie de video

### Renderizador FrameCallback (WPF_WinUI_Callback)

Modo de renderizado basado en callbacks. En vez de dibujar los frames directamente, el motor entrega cada frame a tu código vía eventos, dejándote renderizar con cualquier librería (SkiaSharp, `System.Drawing`, OpenGL/DirectX personalizado, WriteableBitmap) o alimentar un pipeline no-visual (visión por computadora, inferencia AI, streaming a un endpoint remoto).

`FrameCallback` es un alias de `WPF_WinUI_Callback` — el mismo modo con un nombre más auto-descriptivo.

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback;
await VideoCapture1.Video_Renderer_UpdateAsync();

// Suscríbete a los eventos de frame
VideoCapture1.OnVideoFrameBitmap += (sender, e) =>
{
    // e.Frame es un System.Drawing.Bitmap — renderiza a un PictureBox, WriteableBitmap, etc.
};

VideoCapture1.OnVideoFrameBuffer += (sender, e) =>
{
    // e.Frame.Data es IntPtr — envuelve con SkiaSharp / Marshal.Copy para trabajo pixel-level
};
```

Ver [Dibujo de imágenes vía OnVideoFrameBuffer](image-onvideoframebuffer.md) y [Dibujo de texto vía OnVideoFrameBuffer](text-onvideoframebuffer.md) para ejemplos detallados de procesamiento por frame.

**Cuándo usar:**

- Pipelines de visión por computadora / ML que consumen frames crudos
- Renderizado personalizado con SkiaSharp, DirectX u OpenGL
- Apps WPF / WinUI / MAUI que renderizan a un `WriteableBitmap` manualmente
- Apps sin superficie de preview (frames enviados a servidor, encoder, etc.)

### Renderizador WinUI 3

Renderizado nativo para apps WinUI 3 en Windows 10/11. Usa este modo cuando tu shell sea `Microsoft.UI.Xaml` y alojes un control `VisioForge.Core.UI.WinUI.VideoView`.

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.WinUI;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Cuándo usar:**

- Apps WinUI 3 (Windows App SDK, no el antiguo WinUI 2 / UWP)
- Quieres consistencia look-and-feel nativa con otro contenido WinUI

### Renderizador madVR (externo)

[madVR](https://www.madvr.com/) es un renderizador de video externo de calidad de referencia, popular en home-theatre PCs y software de video de gama alta. Ofrece algoritmos superiores de escalado, gestión de color y deinterlacing a costa de mayor carga de GPU. Solo soportado en hosts WinForms; requiere una instalación separada de madVR en la máquina objetivo (el filtro DirectShow registrado por CLSID debe estar presente).

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.MadVR;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Requisito de runtime:** asegúrate de que madVR esté instalado en el sistema objetivo. Si el filtro no está, `Video_Renderer_UpdateAsync` fallará — usa el patrón de fallback mostrado en [Problemas de Compatibilidad del Renderizador](#problemas-de-compatibilidad-del-renderizador) abajo para degradar con gracia a EVR.

**Cuándo usar:**

- Calidad de video de referencia para mastering, HTPC o UIs de media server
- Audiencias con GPUs que pueden absorber el coste extra de renderizado
- Puedes enviar / documentar un paso de instalación separado de madVR

### None (headless)

Desactiva el renderizado por completo. El grafo de captura / edición / reproducción sigue corriendo — los frames fluyen a encoders, salidas de archivo, endpoints de streaming o callbacks — pero no se asigna superficie de preview.

**Ejemplo de implementación:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.None;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Cuándo usar:**

- Captura solo-audio (micrófono-a-archivo) cuando el SDK tiene ramas de audio y video
- Conversión / transcoding de archivo sin ventana de preview
- Pipelines de renderizado server-side
- Pruebas unitarias y runs CI headless

## Opciones de Configuración Avanzada

Más allá de solo seleccionar un renderizador, el SDK proporciona varias opciones de configuración para ajustar la presentación de video.

### Trabajando con Modos de Desentrelazado

Al mostrar contenido de video entrelazado (común en fuentes de transmisión), el desentrelazado adecuado mejora significativamente la calidad visual. El SDK soporta varios algoritmos de desentrelazado dependiendo del renderizador elegido.

Primero, recupera los modos de desentrelazado disponibles. `Video_Renderer_Deinterlace_Modes()` devuelve los nombres de modos VMR-9 auto-descubiertos del driver actual:

```cs
// Poblar un desplegable con los modos VMR-9 disponibles
foreach (string deinterlaceMode in VideoCapture1.Video_Renderer_Deinterlace_Modes())
{
  cbDeinterlaceModes.Items.Add(deinterlaceMode);
}
```

El desentrelazado se configura en los dos renderizadores por separado. VMR-9 recibe un string con el nombre del modo; EVR recibe un valor del enum `VideoRendererEVRDeinterlaceMode`:

```cs
// VMR-9 — asignar el string del modo seleccionado por el usuario
VideoCapture1.Video_Renderer.Deinterlace_VMR9_Mode = cbDeinterlaceModes.SelectedItem.ToString();
VideoCapture1.Video_Renderer.Deinterlace_VMR9_UseDefault = false;

// EVR — usar el enum en su lugar
// VideoCapture1.Video_Renderer.Deinterlace_EVR_Mode = VideoRendererEVRDeinterlaceMode.Auto;

VideoCapture1.Video_Renderer_Update();
```

VMR9 y EVR soportan varios algoritmos de desentrelazado incluyendo:

- Bob (duplicación simple de línea)
- Weave (entrelazado de campos)
- Adaptativo al movimiento
- Compensado por movimiento (mayor calidad)

La disponibilidad de algoritmos específicos depende de las capacidades de la tarjeta de video e implementación del driver.

### Gestionando Relación de Aspecto y Modos de Estiramiento

Al mostrar video en una ventana o control que no coincide con la relación de aspecto nativa de la fuente, necesitas decidir cómo manejar esta discrepancia. El SDK proporciona múltiples modos de estiramiento para abordar diferentes escenarios.

#### Modo Estirar

Este modo estira el video para llenar toda el área de visualización, potencialmente distorsionando la imagen:

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Stretch;
VideoCapture1.Video_Renderer_Update();
```

**Casos de uso:**

- Cuando la relación de aspecto no es crítica
- Llenar todo el área de visualización es más importante que las proporciones
- La fuente y la pantalla tienen relaciones de aspecto similares
- Las restricciones de interfaz de usuario requieren uso de área completa

#### Modo Letterbox

Este modo preserva la relación de aspecto original agregando bordes negros según sea necesario:

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
VideoCapture1.Video_Renderer_Update();
```

**Casos de uso:**

- Mantener proporciones correctas es esencial
- Aplicaciones de video profesionales
- Contenido donde la distorsión sería notable o problemática
- Visualización de contenido de cine o transmisión

#### Modo LetterboxToFill

Este modo llena el área de visualización preservando la relación de aspecto, recortando el sobrante en uno de los ejes:

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.LetterboxToFill;
VideoCapture1.Video_Renderer_Update();
```

**Casos de uso:**

- Aplicaciones de video para consumidores donde llenar la pantalla es preferido
- Contenido donde los bordes son menos importantes que el centro
- Visualización de video estilo redes sociales
- Cuando se intenta eliminar letterboxing en contenido ya con letterbox

### Forzar Relación de Aspecto

Para forzar una relación de aspecto específica (ej. mostrar contenido 4:3 con letterbox dentro de un contenedor 16:9), habilita el override y asigna los componentes:

```cs
VideoCapture1.Video_Renderer.Aspect_Ratio_Override = true;
VideoCapture1.Video_Renderer.Aspect_Ratio_X = 4;
VideoCapture1.Video_Renderer.Aspect_Ratio_Y = 3;
VideoCapture1.Video_Renderer_Update();
```

### Zoom y Pan

`VideoRendererSettings` expone propiedades de zoom/desplazamiento útiles para PTZ digital sobre la vista previa:

```cs
VideoCapture1.Video_Renderer.Zoom_Ratio  = 150; // 150%
VideoCapture1.Video_Renderer.Zoom_ShiftX = 0;
VideoCapture1.Video_Renderer.Zoom_ShiftY = 0;
VideoCapture1.Video_Renderer_Update();
```

### Flip y Rotación

```cs
VideoCapture1.Video_Renderer.Flip_Horizontal = true;
VideoCapture1.Video_Renderer.Flip_Vertical   = false;
// RotationAngle solo lo respeta el renderizador Direct2D y acepta 0, 90, 180 o 270.
VideoCapture1.Video_Renderer.RotationAngle   = 90;
VideoCapture1.Video_Renderer_Update();
```

## Solución de Problemas Comunes

### Problemas de Compatibilidad del Renderizador

Si encuentras problemas con un renderizador específico, intenta volver a una opción más compatible:

```cs
try
{
    // Intentar usar EVR primero
    VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
    VideoCapture1.Video_Renderer_Update();
}
catch
{
    try 
    {
        // Volver a VMR9
        VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VMR9;
        VideoCapture1.Video_Renderer_Update();
    }
    catch
    {
        // Último recurso - renderizador heredado
        VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VideoRenderer;
        VideoCapture1.Video_Renderer_Update();
    }
}
```

## Mejores Prácticas y Recomendaciones

1. **Elige el renderizador correcto para tu entorno objetivo**:
   - Para Windows moderno: EVR
   - Para compatibilidad amplia: VMR9
   - Para sistemas heredados: Video Renderer

2. **Prueba en varias configuraciones de hardware**: El renderizado de video puede comportarse diferentemente a través de proveedores de GPU y versiones de drivers.

3. **Implementa lógica de respaldo de renderizador**: Siempre ten un plan de respaldo si el renderizador preferido falla.

4. **Considera tu contenido de video**: El contenido de mayor resolución o entrelazado se beneficiará más de renderizadores avanzados.

5. **Equilibra calidad vs. rendimiento**: Las configuraciones de más alta calidad pueden no siempre entregar la mejor experiencia de usuario si impactan el rendimiento.

## Dependencias Requeridas

Para asegurar la funcionalidad apropiada de estos renderizadores, asegúrate de incluir:

- Paquetes redistribuibles del SDK
- DirectX End-User Runtime (versión más reciente recomendada)
- .NET Framework runtime apropiado para tu aplicación

## Conclusión

Los motores clásicos ofrecen 10 modos de renderizador cubriendo WinForms, WPF y WinUI 3. **EVR** es el default seguro para WinForms, **Direct2D** para renderizado moderno acelerado por GPU en WinForms o WPF, **FrameCallback** para pipelines personalizados (CV / AI / renderizado a medida), **WinUI** para shells WinUI 3, y **madVR** para escenarios de calidad de referencia que puedan acomodar la instalación externa. `None` es el modo correcto cuando no hay preview en absoluto.

Para aplicaciones construidas sobre los motores multiplataforma `VideoCaptureCoreX` / `MediaPlayerCoreX`, la elección de renderizador la maneja el control `VideoView` y no usa este enum.

## Documentación relacionada

- [Dibujo de imágenes vía OnVideoFrameBuffer](image-onvideoframebuffer.md) — procesamiento pixel-level de frames, el caso de uso canónico de `FrameCallback`.
- [Dibujo de texto vía OnVideoFrameBuffer](text-onvideoframebuffer.md) — overlays de texto con `FrameCallback`.
- [Renderizado de video en un PictureBox](draw-video-picturebox.md) — patrón de renderizado WinForms que se combina bien con `FrameCallback`.

---

Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.