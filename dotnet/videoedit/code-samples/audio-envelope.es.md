---
title: Envolvente de volumen de audio en .NET con Video Edit SDK
description: Implemente efectos de envolvente de volumen de audio profesionales en aplicaciones .NET con tutorial completo y muestras de código paso a paso.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - Effects
  - C#
  - NuGet
primary_api_classes:
  - AudioVolumeEnvelopeEffect
  - AudioSource
  - AudioTrackEffect

---

# Implementar efectos de envolvente de volumen de audio en .NET

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

Las envolventes de volumen de audio son herramientas esenciales para la producción de video profesional, permitiendo a los desarrolladores controlar precisamente los niveles de audio a lo largo de una línea de tiempo. Este tutorial demuestra cómo implementar estos efectos en sus aplicaciones .NET.

## ¿Qué es una envolvente de volumen de audio?

Una envolvente de volumen de audio le permite ajustar los niveles de volumen de su pista de audio. En lugar de ajustar manualmente el volumen durante el proceso de edición, las envolventes proporcionan una forma programática de establecer niveles de volumen consistentes. Esto es particularmente útil cuando se trabaja con múltiples pistas de audio que necesitan mantener relaciones de volumen específicas.

## Descripción general de la implementación

El proceso de implementación involucra tres pasos clave:

1. Crear una fuente de audio desde su archivo
2. Crear el efecto de envolvente de volumen con su nivel deseado
3. Añadir el audio con el efecto a su línea de tiempo

Cada paso requiere componentes de código específicos que exploraremos en detalle a continuación.

## Entender la clase AudioVolumeEnvelopeEffect

La clase `AudioVolumeEnvelopeEffect` es el componente central para implementar el control de volumen:

```cs
public class AudioVolumeEnvelopeEffect : AudioTrackEffect
{
    /// <summary>
    /// Obtiene o establece el nivel (en porcentaje), el rango es [0-100].
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase AudioVolumeEnvelopeEffect. 
    /// </summary>
    /// <param name="level">
    /// Nivel (en porcentaje), el rango es [0-100].
    /// </param>
    public AudioVolumeEnvelopeEffect(int level) 
    {
        Level = level;
    }
}
```

Como puede ver, esta clase:

- Hereda de `AudioTrackEffect`
- Tiene una propiedad `Level` que acepta valores de 0-100 (representando porcentaje de volumen)
- Proporciona un constructor para establecer el nivel inicial

## Pasos de implementación detallados

### 1. Crear su fuente de audio

El primer paso involucra inicializar un objeto de fuente de audio que referencia su archivo de audio. Este objeto sirve como base para aplicar efectos.

```cs
var audioFile = new AudioSource(file, segments, null);
```

En este código:

- `file` es la ruta a su archivo de audio
- `segments` define segmentos de tiempo si solo está usando porciones del audio
- El parámetro final puede contener opciones adicionales (null en este ejemplo básico)

### 2. Configurar el efecto de envolvente de volumen

A continuación, cree y configure el efecto de envolvente de volumen especificando su nivel de volumen deseado:

```cs
var envelope = new AudioVolumeEnvelopeEffect(70);
```

Esto crea un efecto de envolvente de volumen establecido al 70%. El parámetro acepta valores de 0 a 100:

- 0 = completamente silencioso
- 50 = mitad de volumen
- 100 = volumen completo

También puede ajustar el nivel después de la creación:

```cs
var envelope = new AudioVolumeEnvelopeEffect(50);
envelope.Level = 75; // Cambiado a 75% de volumen
```

### 3. Añadir audio con efecto de envolvente a la línea de tiempo

El paso final es añadir su fuente de audio con el efecto de envolvente aplicado a la línea de tiempo de su proyecto:

```cs
VideoEdit1.Input_AddAudioFile(
    audioFile,                        // Su fuente de audio configurada
    TimeSpan.FromMilliseconds(0),     // Posición inicial en la línea de tiempo
    0,                                // Índice de pista
    new []{ envelope }                // Array de efectos a aplicar
);
```

Esto posiciona su audio al principio de la línea de tiempo (0ms) y aplica el efecto de envolvente que configuramos anteriormente.

## Casos de uso comunes

### Normalizar niveles de audio

Al trabajar con audio de diferentes fuentes, la normalización asegura niveles de volumen consistentes:

```cs
// Audio de entrevista principal a volumen completo
// AudioSource ctor #1: (filename, startTime?, stopTime?, fileToSync?, streamNumber, rate)
var interviewAudio = new AudioSource("interview.mp3", (TimeSpan?)null, (TimeSpan?)null);
VideoEdit1.Input_AddAudioFile(interviewAudio, TimeSpan.Zero, 0, null);

// Música de fondo al 30% de volumen para evitar dominar el habla
var backgroundMusic = new AudioSource("background.mp3", (TimeSpan?)null, (TimeSpan?)null);
var musicEnvelope = new AudioVolumeEnvelopeEffect(30);
VideoEdit1.Input_AddAudioFile(backgroundMusic, TimeSpan.Zero, 1, new[] { musicEnvelope });
```

### Silenciar secciones específicas

Si necesita silenciar secciones de audio en su línea de tiempo, puede crear y aplicar diferentes efectos de envolvente:

```cs
// Crear fuentes de audio para diferentes segmentos usando AudioSource(filename, FileSegment[], fileToSync, streamNumber, rate)
var segment1 = new AudioSource("audio.mp3",
    new[] { new FileSegment(TimeSpan.FromMilliseconds(0),     TimeSpan.FromMilliseconds(10000)) }, null); // 0-10s
var segment2 = new AudioSource("audio.mp3",
    new[] { new FileSegment(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(15000)) }, null); // 10-15s
var segment3 = new AudioSource("audio.mp3",
    new[] { new FileSegment(TimeSpan.FromMilliseconds(15000), TimeSpan.FromMilliseconds(30000)) }, null); // 15-30s

// Aplicar diferentes niveles de volumen
VideoEdit1.Input_AddAudioFile(segment1, TimeSpan.Zero, 0, new[] { new AudioVolumeEnvelopeEffect(100) });
// Silenciar segmento intermedio
VideoEdit1.Input_AddAudioFile(segment2, TimeSpan.FromMilliseconds(10000), 0, new[] { new AudioVolumeEnvelopeEffect(0) });
VideoEdit1.Input_AddAudioFile(segment3, TimeSpan.FromMilliseconds(15000), 0, new[] { new AudioVolumeEnvelopeEffect(100) });
```

## Dependencias requeridas

Para implementar efectos de envolvente de audio, necesitará:

- Paquetes redistribuibles de Video Edit SDK .NET:
  - [versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Puede instalar estos paquetes a través del Administrador de Paquetes NuGet:

```nuget
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
```

Para más información sobre cómo desplegar estas dependencias en los sistemas de sus usuarios, consulte nuestra [documentación de despliegue](../deployment.md).

## Consideraciones de rendimiento

Al implementar efectos de volumen de audio, considere estos consejos de rendimiento:

- Aplicar efectos de envolvente durante la fase de edición/renderizado en lugar de en tiempo de ejecución
- Al trabajar con múltiples pistas, considere el efecto acumulativo de todo el procesamiento de audio
- Pruebe en su hardware objetivo para asegurar una reproducción fluida

## Solucionar problemas comunes

Si encuentra problemas con su implementación de envolvente de audio:

- Verifique que las rutas y formatos de archivo de audio sean compatibles
- Compruebe que los porcentajes de volumen estén dentro del rango 0-100
- Asegúrese de que el efecto de audio esté correctamente añadido al array de efectos
- Verifique que el posicionamiento en la línea de tiempo no cree conflictos entre segmentos de audio

## Conclusión

Los efectos de envolvente de volumen de audio proporcionan control esencial sobre la experiencia de audio de su aplicación. Siguiendo esta guía, ha aprendido cómo implementar el control de volumen en sus proyectos de edición de video .NET, equilibrando diferentes fuentes de audio para resultados profesionales.

---
Para más muestras de código y técnicas avanzadas, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).