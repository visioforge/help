---
title: Efectos de Audio en C# y .NET - EQ, Reverb, Filtros y Más
description: Aplique efectos de audio en tiempo real en C# y .NET con VisioForge SDKs. Ecualizador, reverb, eco, reducción de ruido, cambio de tono y más de 30 efectos.
sidebar_label: Efectos de Audio
---

# Efectos de Audio en Tiempo Real para Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge Media Framework proporciona más de 30 efectos de audio para el procesamiento de audio en tiempo real en aplicaciones C# y .NET. Construidos sobre GStreamer, los efectos multiplataforma incluyen ecualizadores, reverb, eco, compresión dinámica, filtros, cambio de tono, reducción de ruido basada en IA y más — todo funcionando en Windows, macOS, Linux, iOS y Android.

## SDKs y Plataformas

### Efectos Multiplataforma

- **SDKs**: Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Plataformas**: Windows, macOS, Linux, iOS, Android
- **Espacio de nombres**: `VisioForge.Core.Types.X.AudioEffects`

### Efectos DSP Classic

- **SDKs**: Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore)
- **Plataformas**: Solo Windows
- **Espacio de nombres**: `VisioForge.Core.DSP`

### Efectos DirectSound

- **SDKs**: Video Capture SDK, Media Player SDK, Video Edit SDK (núcleos Classic)
- **Plataformas**: Solo Windows
- **Espacio de nombres**: `VisioForge.Core.Types.X._Windows.AudioEffects`

Para parámetros detallados y propiedades de cada efecto, consulte la [Referencia de Efectos de Audio](reference.md).

## Categorías de Efectos

### Volumen y Dinámica

- **VolumeAudioEffect** — Control básico de volumen con funcionalidad de silencio
- **AmplifyAudioEffect** — Amplificación de señal de audio con control de recorte
- **CompressorExpanderAudioEffect** — Compresión y expansión de rango dinámico
- **DynamicAmplifyAudioEffect** — Control de ganancia adaptativo con tiempos de ataque/liberación

### Ecualización

- **Equalizer10AudioEffect** — Ecualizador gráfico de 10 bandas con frecuencias fijas
- **EqualizerParametricAudioEffect** — Ecualizador paramétrico con bandas configurables
- **TrebleEnhancerAudioEffect** — Realce de altas frecuencias
- **TrueBassAudioEffect** — Realce de bajas frecuencias

### Filtros

- **HighPassAudioEffect** — Filtro pasa-altos para eliminar frecuencias bajas
- **LowPassAudioEffect** — Filtro pasa-bajos para eliminar frecuencias altas
- **BandPassAudioEffect** — Filtro pasa-banda para rangos de frecuencia específicos
- **NotchAudioEffect** — Filtro de rechazo para eliminar frecuencias específicas
- **ChebyshevLimitAudioEffect** — Filtros Chebyshev pasa-bajos/pasa-altos con cortes pronunciados
- **ChebyshevBandPassRejectAudioEffect** — Filtros Chebyshev pasa-banda/rechazo de banda

### Espacial y Estéreo

- **BalanceAudioEffect** — Control de balance estéreo (panorámica izquierda/derecha)
- **WideStereoAudioEffect** — Mejora de la amplitud estéreo
- **Sound3DAudioEffect** — Efectos de audio espacial 3D
- **HRTFRenderAudioEffect** — Audio espacial con Función de Transferencia Relativa a la Cabeza
- **PhaseInvertAudioEffect** — Inversión de fase para corrección de polaridad

### Efectos Basados en Tiempo

- **EchoAudioEffect** — Efectos de eco y retardo
- **RSAudioEchoAudioEffect** — Eco mejorado con controles avanzados
- **ReverberationAudioEffect** — Reverberación de sala (algoritmo Freeverb)
- **FadeAudioEffect** — Automatización de volumen con fundido de entrada/salida

### Efectos de Modulación

- **PhaserAudioEffect** — Efecto phaser con modulación LFO
- **FlangerAudioEffect** — Efecto flanging con modulación de retardo

### Tono y Tempo

- **PitchShiftAudioEffect** — Cambio de tono sin cambio de tempo
- **ScaleTempoAudioEffect** — Cambio de tempo sin cambio de tono

### Efectos Especiales

- **KaraokeAudioEffect** — Eliminación de voz para karaoke
- **RemoveSilenceAudioEffect** — Detección y eliminación automática de silencio
- **CsoundAudioEffect** — Procesamiento de audio avanzado basado en Csound

### Reducción de Ruido y Medición

- **AudioRNNoiseAudioEffect** — Reducción de ruido basada en IA usando RNN
- **AudioLoudNormAudioEffect** — Normalización de sonoridad EBU R128
- **EbuR128LevelAudioEffect** — Medición de sonoridad EBU R128

### Gestión de Canales

- **ChannelOrderAudioEffect** — Remapeo y enrutamiento de canales
- **DownMixAudioEffect** — Mezcla descendente de multicanal a estéreo/mono

### Efectos DirectSound (Solo Windows, SDKs Classic)

- **DS Chorus** — Múltiples copias retardadas y moduladas
- **DS Distortion** — Distorsión/overdrive de audio
- **DS Gargle** — Modulación de gárgara/trémolo
- **DS Reverb (I3DL2)** — Reverberación profesional con modelado ambiental
- **DS Waves Reverb** — Reverberación simplificada

## Elementos GStreamer

Todos los efectos de audio multiplataforma están construidos sobre el framework multimedia GStreamer. Cada efecto envuelve uno o más elementos de GStreamer:

| Categoría | Elementos GStreamer |
|-----------|---------------------|
| Volumen/Dinámica | volume, audioamplify, audiodynamic |
| Ecualización | equalizer-10bands, equalizer-nbands |
| Filtros | audiocheblimit, audiochebband, audioiirfilter |
| Espacial | audiopanorama, stereo, hrtfrender |
| Basados en Tiempo | audioecho, rsaudioecho, freeverb |
| Modulación | Implementaciones personalizadas de phaser/flanger |
| Tono/Tempo | scaletempo, pitch (SoundTouch) |
| Especiales | audiokaraoke, csoundfilter, removesilence |
| Reducción de Ruido | audiornnoise, audioloudnorm, ebur128level |
| Canales | audioconvert, enrutamiento personalizado |

## Patrones de Uso Comunes

### Agregar Efectos (SDKs Multiplataforma)

```csharp
// Crear un efecto de audio
var volumeEffect = new VolumeAudioEffect(1.5); // 150% de volumen

// VideoCaptureCoreX / MediaPlayerCoreX
core.Audio_Effects_AddOrUpdate(volumeEffect);
```

### Combinar Múltiples Efectos

Los efectos se procesan en el orden en que se agregan:

```csharp
// Crear una cadena de procesamiento
core.Audio_Effects_AddOrUpdate(new HighPassAudioEffect(80));           // Eliminar ruido grave
core.Audio_Effects_AddOrUpdate(new CompressorExpanderAudioEffect());   // Comprimir dinámica
core.Audio_Effects_AddOrUpdate(new Equalizer10AudioEffect(levels));    // Ajustes de EQ
core.Audio_Effects_AddOrUpdate(new ReverberationAudioEffect());        // Agregar reverb
```

### Actualizaciones de Parámetros en Tiempo Real

La mayoría de los efectos admiten cambios de parámetros en tiempo real:

```csharp
var volumeEffect = new VolumeAudioEffect(1.0);
core.Audio_Effects_AddOrUpdate(volumeEffect);

// Más adelante, durante la reproducción:
volumeEffect.Level = 0.5; // Reducir volumen al 50%
volumeEffect.Mute = true; // Silenciar audio
core.Audio_Effects_AddOrUpdate(volumeEffect); // Aplicar cambios
```

### Uso con Media Blocks SDK

Para el uso basado en pipeline con Media Blocks SDK y bloques de efectos de audio dedicados, consulte [Bloques de Procesamiento y Efectos de Audio](../../mediablocks/AudioProcessing/index.md).

## Consideraciones de Rendimiento

- **Uso de CPU**: Efectos complejos como reverberación, Csound y múltiples ecualizadores pueden ser intensivos en CPU
- **Orden de Efectos**: Coloque los efectos computacionalmente costosos después de los más simples para reducir la carga de procesamiento
- **Procesamiento en Tiempo Real**: Todos los efectos están diseñados para streaming de audio en tiempo real

## Preguntas Frecuentes

???+ "¿Qué efectos de audio están disponibles en los SDKs .NET de VisioForge?"
    VisioForge proporciona más de 30 efectos de audio incluyendo control de volumen, ecualizadores de 10 bandas y paramétricos, reverb, eco, compresor/expansor, filtros pasa-altos/bajos/banda, cambio de tono, reducción de ruido (basada en RNN), eliminación de voz para karaoke, audio espacial 3D y más. Todos los efectos multiplataforma funcionan en Windows, macOS, Linux, iOS y Android.

???+ "¿Cómo agrego efectos de audio a mi aplicación C#?"
    Cree una instancia del efecto y agréguela usando `Audio_Effects_AddOrUpdate()`. Por ejemplo:

    ```csharp
    // EQ de 10 bandas: todas las bandas a 0 dB (neutro)
    var eq = new Equalizer10AudioEffect(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    core.Audio_Effects_AddOrUpdate(eq);
    ```

    Los efectos se pueden agregar, actualizar y eliminar durante la reproducción. Para Media Blocks SDK, use el `AudioEffectsBlock`.

???+ "¿Puedo encadenar múltiples efectos de audio juntos?"
    Sí. Los efectos se procesan en el orden en que se agregan. Puede crear cadenas de procesamiento complejas combinando filtros, EQ, compresión, reverb y otros efectos. Cada efecto procesa la salida de audio del efecto anterior.

???+ "¿Los efectos de audio funcionan en tiempo real durante la reproducción?"
    Sí. Todos los efectos de audio de VisioForge admiten cambios de parámetros en tiempo real. Puede ajustar volumen, bandas del EQ, niveles de reverb y otros parámetros mientras se reproduce el audio sin interrumpir el flujo.

???+ "¿Cuál es la diferencia entre los efectos multiplataforma y DirectSound?"
    Los efectos multiplataforma (espacio de nombres `VisioForge.Core.Types.X.AudioEffects`) funcionan en todas las plataformas usando GStreamer. Los efectos DirectSound son solo para Windows y están disponibles en los núcleos de SDK clásicos. Los efectos multiplataforma cubren la misma funcionalidad y más.

## Ver También

- [Referencia de Efectos de Audio](reference.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Codificadores de Audio](../audio-encoders/index.md)
- [Bloques de Procesamiento de Audio (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
