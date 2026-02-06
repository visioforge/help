---
title: Superposiciones de Texto Avanzadas en Video .NET
description: Cree superposiciones de texto dinámicas con control de fuente, color, posición, rotación y animación para marcas de tiempo, subtítulos y branding en video .NET.
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

## Mejores prácticas para superposiciones de texto

- Considere la legibilidad contra diferentes fondos
- Use tamaños de fuente apropiados para la resolución de pantalla objetivo
- Implemente efectos de fundido para superposiciones menos intrusivas
- Pruebe el impacto en el rendimiento con efectos de texto complejos

---
Para más ejemplos de código y detalles de implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).