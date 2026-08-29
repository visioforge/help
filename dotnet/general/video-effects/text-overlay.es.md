---
title: Agregar Overlay de Texto en Video con C# y Fuentes Custom
description: Cree overlays de texto dinámicos con control de fuente, color, posición, rotación y animación para timestamps, subtítulos y branding en video .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
  - Effects
  - C#
primary_api_classes:
  - TextOverlayVideoEffect
  - OverlayManagerText
  - FontSettings
  - VideoEffectTextLogo

---

# Implementación de Superposiciones de Texto en Flujos de Video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [MediaPlayerCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Introducción

Las superposiciones de texto proporcionan una forma poderosa de mejorar flujos de video con información dinámica, branding, subtítulos o marcas de tiempo. Esta guía explora cómo implementar superposiciones de texto totalmente personalizables con control preciso sobre apariencia, posicionamiento y animaciones.

## Implementación de Motor Clásico

Nuestros motores clásicos (VideoCaptureCore, MediaPlayerCore, VideoEditCore) ofrecen una API sencilla para agregar texto a flujos de video.

### Implementación básica de superposición de texto

El siguiente ejemplo demuestra una superposición de texto simple con posicionamiento personalizado:

```csharp
var effect = new VideoEffectTextLogo(true, "textoverlay");

// establecer posición
effect.Left = 20;
effect.Top = 20;

// establecer Fuente (System.Drawing.Font)
effect.Font = new Font("Arial", 40);

// establecer texto
effect.Text = "¡Hola, mundo!";

// establecer color del texto
effect.FontColor = Color.Yellow;

MediaPlayer1.Video_Effects_Add(effect);
```

### Opciones de visualización de información dinámica

#### Visualización de marca de tiempo y fecha

Puede mostrar automáticamente información de fecha actual, hora o marca de tiempo del video usando modos especializados:

```csharp
// establecer modo y máscara
effect.Mode = TextLogoMode.DateTime;
effect.DateTimeMask = "yyyy-MM-dd. hh:mm:ss";
```

El SDK soporta máscaras de formato personalizadas para marcas de tiempo y fechas, permitiendo control preciso sobre el formato de la información mostrada. La visualización del número de cuadro no requiere configuración adicional.

### Efectos de animación y transición

#### Implementación de efectos de fundido

Cree apariciones y desapariciones suaves de texto con efectos de fundido personalizables:

```csharp
// agregar el fundido de entrada
effect.FadeIn = true; 
effect.FadeInDuration = TimeSpan.FromMilliseconds(5000);

// agregar el fundido de salida
effect.FadeOut = true;
effect.FadeOutDuration = TimeSpan.FromMilliseconds(5000);
```

### Opciones de rotación de texto

Rote su superposición de texto para coincidir con sus requisitos de diseño:

```csharp
// establecer modo de rotación
effect.RotationMode = TextRotationMode.Rm90;
```

### Transformaciones de volteo de texto

Aplique efectos de espejo a su texto para presentaciones creativas:

```csharp
// establecer modo de volteo
effect.FlipMode = TextFlipMode.XAndY;
```

## Implementación de Motor X

Nuestros motores X más nuevos (VideoCaptureCoreX, MediaPlayerCoreX, VideoEditCoreX) proporcionan una API mejorada con características adicionales.

### Superposición de texto básica del Motor X

```csharp
// superposición de texto
var textOverlay = new TextOverlayVideoEffect() { Text = "¡Hola Mundo!" };
 
// establecer posición
textOverlay.XPad = 20;
textOverlay.YPad = 20;

textOverlay.HorizontalAlignment = TextOverlayHAlign.Left;
textOverlay.VerticalAlignment = TextOverlayVAlign.Top;

// establecer Fuente - usando inicializador de objeto
textOverlay.Font = new FontSettings
{
    Name = "Arial",
    Size = 24,
    Weight = FontWeight.Bold
};

// Alternativa: usando constructor con cadena de estilo de fuente
// textOverlay.Font = new FontSettings("Arial", "Bold", 24);

// establecer texto
textOverlay.Text = "¡Hola, mundo!";

// establecer color del texto
textOverlay.Color = SKColors.Yellow;

// agregar el efecto
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

### Visualización avanzada de contenido dinámico

#### Integración de marca de tiempo del video

Muestre la posición actual dentro del video:

```csharp
// superposición de texto
var textOverlay = new TextOverlayVideoEffect();
  
// establecer texto
textOverlay.Text = "Marca de tiempo: ";

// establecer modo de marca de tiempo
textOverlay.Mode = TextOverlayMode.Timestamp;

// agregar el efecto
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

#### Integración de hora del sistema

Muestre la hora actual del sistema junto con su contenido de video:

```csharp
// superposición de texto
var textOverlay = new TextOverlayVideoEffect();
 
// establecer texto
textOverlay.Text = "Hora: ";

// establecer modo de hora del sistema
textOverlay.Mode = TextOverlayMode.SystemTime;

// agregar el efecto
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

### Texto que cambia en cada fotograma

`TextOverlayVideoEffect` está pensado para texto que cambia poco y no ofrece ninguna ventana
temporal. Para una lectura en vivo - un valor de sensor, el número de fotograma, el reloj - use
`OverlayManagerText` a través de `Video_Overlay_Add` y asígnele un `TextProvider`. Se le pide el
texto una vez por fotograma:

```csharp
// El gestor de superposiciones solo se inserta en el pipeline cuando esto es true,
// y debe establecerse antes de Start/StartAsync.
videoCapture1.Video_Overlay_Enabled = true;

var text = new OverlayManagerText(string.Empty, x: 40, y: 40);
text.Color = SKColors.Yellow;
text.Font.Size = 28;

// Se llama una vez por fotograma en el hilo de streaming. El argumento es la marca de
// tiempo del fotograma, contada desde el inicio del pipeline.
text.TextProvider = ts => "Camera 1\nOperator: demo\nREC\n"
    + $"Sensor {_sensorValue:F1}   {DateTime.Now:HH:mm:ss}   {ts:hh\\:mm\\:ss}";

videoCapture1.Video_Overlay_Add(text);
```

Las líneas estáticas y dinámicas caben en una sola cadena, así que un bloque de rótulos con una única
línea viva sigue costando una llamada por fotograma. Devolver la misma cadena que la vez anterior es
barato: la maquetación del texto solo se vuelve a medir cuando la cadena realmente cambia.

El callback se ejecuta en el hilo de streaming, bajo el mismo bloqueo que toman `Video_Overlay_Add` y
`Video_Overlay_Remove`. Manténgalo corto y no bloquee dentro de él: llamar a `Dispatcher.Invoke`
desde ahí para leer un valor de la interfaz puede provocar un interbloqueo, no solo perder un
fotograma. Lea en su lugar un campo que el hilo de interfaz ya haya escrito. Un callback que lanza una
excepción se registra una vez y después deja de llamarse, y el elemento vuelve a su `Text`; volver a
asignar `TextProvider` lo reactiva.

`OverlayManagerText` también respeta `StartTime` y `EndTime`. Cada límite funciona por sí solo - un
`StartTime` cero significa «desde el principio» y un `EndTime` cero significa «sin fin» - y ambos se
comparan con la marca de tiempo del fotograma, no con el reloj del sistema.

Consulte la [página de OverlayManagerBlock](../../mediablocks/VideoProcessing/OverlayManagerBlock.md)
para ver la lista completa de elementos de superposición.

`X` e `Y` son la esquina superior izquierda del texto, en píxeles. `(0, 0)` es la esquina superior
izquierda del fotograma y la primera línea de un texto multilínea queda visible allí.

### Superposiciones solo en la vista previa de MediaPlayerCoreX

La misma API `Video_Overlay_*` está disponible en `MediaPlayerCoreX`. Establezca
`Video_Overlay_Enabled` antes de `OpenAsync` / `PlayAsync`. La superposición se inserta solo en la
rama del renderizador, después del sample grabber y de cualquier salida de vídeo personalizada, de
modo que el archivo en disco, las capturas y las exportaciones no se modifican. Los elementos
añadidos con `Video_Overlay_Add` sobreviven a `Stop` y a abrir otro archivo:

```csharp
mediaPlayer1.Video_Overlay_Enabled = true;

var text = new OverlayManagerText(string.Empty, x: 0, y: 0);
text.TextProvider = ts => $"T {ts:hh\\:mm\\:ss}";
mediaPlayer1.Video_Overlay_Add(text);
```

## Mejores prácticas para superposiciones de texto

- Considere la legibilidad contra diferentes fondos
- Use tamaños de fuente apropiados para la resolución de pantalla objetivo
- Implemente efectos de fundido para superposiciones menos intrusivas
- Pruebe el impacto en el rendimiento con efectos de texto complejos

---
Para más ejemplos de código y detalles de implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).