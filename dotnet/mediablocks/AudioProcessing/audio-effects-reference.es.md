---
title: Referencia de Efectos de Audio
description: Guía completa de efectos de audio en VisioForge SDKs. Incluye volumen, ecualización, eco, reverberación, compresión y más efectos para procesamiento de audio multiplataforma.
sidebar_label: Referencia de Efectos de Audio
---

# Referencia de Efectos de Audio

Este documento proporciona una referencia completa para todos los efectos de audio disponibles en los SDKs de VisioForge.

## Tipos de Efectos y Disponibilidad

VisioForge proporciona **dos tipos** de efectos de audio con diferente soporte de plataforma:

### Efectos Multiplataforma
- **Soporte de Plataforma**: Windows, macOS, Linux, iOS, Android
- **SDKs**: Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Implementación**: Procesamiento de audio multiplataforma
- **Ubicación**: Espacio de nombres `VisioForge.Core.Types.X.AudioEffects`
- **Uso**: A través de `AudioEffectsBlock` en pipelines de audio (Media Blocks SDK) o métodos `Audio_Effects_*` (VideoCaptureCoreX/MediaPlayerCoreX)

### Efectos DSP Classic (VideoCaptureCore/MediaPlayerCore/VideoEditCore)
- **Soporte de Plataforma**: Solo Windows
- **SDKs**: Video Capture SDK, Media Player SDK, Video Edit SDK
- **Implementación**: Procesamiento DSP nativo
- **Ubicación**: Espacio de nombres `VisioForge.Core.DSP`
- **Uso**: A través de métodos `Audio_Effects_*` en los núcleos del SDK

### Efectos DirectSound (Windows Classic)
- **Soporte de Plataforma**: Solo Windows
- **SDKs**: Video Capture SDK, Media Player SDK, Video Edit SDK
- **Implementación**: Basado en DirectSound/DirectX
- **Ubicación**: Espacio de nombres `VisioForge.Core.Types.X._Windows.AudioEffects`
- **Uso**: A través de métodos `Audio_Effects_DS_*` en los núcleos del SDK

## Control de Volumen y Nivel

### Volumen
**Disponibilidad:** ✅ Multiplataforma (Media Blocks, VideoCaptureCoreX, MediaPlayerCoreX) | ✅ SDKs Classic (Windows)  
**Elemento de Audio:** `volume`

Controla el nivel de volumen de un flujo de audio.

**Propiedades:**
- **Level** (double): Nivel de volumen (0.0 = silencio, 1.0 = normal, >1.0 = amplificación)
- **Mute** (bool): Bandera de silencio

**Uso Típico (SDKs Multiplataforma):**
```csharp
var effect = new VolumeAudioEffect(0.8); // 80% de volumen
effect.Mute = false;

// Media Blocks SDK
audioEffectsBlock.AddOrUpdate(effect);

// VideoCaptureCoreX / MediaPlayerCoreX
videoCaptureX.Audio_Effects_AddOrUpdate(effect);
```

**Uso Típico (SDKs Classic):**
```csharp
// Manejado a través de propiedades de volumen de audio del SDK
```

### Amplificar
**Disponibilidad:** ✅ Multiplataforma (Media Blocks, VideoCaptureCoreX, MediaPlayerCoreX) | ✅ SDKs Classic (Windows)  
**Elemento de Audio:** `audioamplify` (Multiplataforma)  
**Clase DSP:** `Amplify` (SDKs Classic)

Amplifica audio con métodos de recorte configurables.

**Propiedades:**
- **Level** (double): Factor de amplificación (1.0 - 10.0)
- **ClippingMethod**: None, Normal, o Soft (solo Media Blocks)

**Uso Típico (SDKs Multiplataforma):**
```csharp
var effect = new AmplifyAudioEffect(2.0); // Doble amplificación
effect.ClippingMethod = AmplifyClippingMethod.Normal;

// Media Blocks SDK
audioEffectsBlock.AddOrUpdate(effect);

// VideoCaptureCoreX / MediaPlayerCoreX
videoCaptureX.Audio_Effects_AddOrUpdate(effect);
```

**Uso Típico (SDKs Classic):**
```csharp
// Añadir vía Audio_Effects_Add, luego configurar
videoCaptureCore.Audio_Effects_Add(0, AudioEffectType.Amplify, "amp", true, 
    TimeSpan.Zero, TimeSpan.MaxValue);
videoCaptureCore.Audio_Effects_Amplify(0, "amp", 15000, false);
```

## Procesamiento Estéreo

### Balance
**Elemento de Audio:** `audiopanorama`

Controla el balance estéreo (paneo izquierda-derecha).

**Propiedades:**
- **Level** (double): Posición de balance (-1.0 = izquierda completa, 0.0 = centro, +1.0 = derecha completa)

**Uso Típico:**
```csharp
var effect = new BalanceAudioEffect(0.3); // Ligeramente a la derecha
```

### Estéreo Amplio
**Elemento de Audio:** `stereo`

Mejora la anchura del estéreo.

**Propiedades:**
- **Level** (float): Cantidad de ampliación (0.01 - 1.0)

**Uso Típico:**
```csharp
var effect = new WideStereoAudioEffect();
effect.Level = 0.05f; // Ampliación sutil
```

### Karaoke
**Elemento de Audio:** `audiokaraoke`

Elimina o reduce las voces centradas en el panorama.

**Propiedades:**
- **FilterBand** (float): Frecuencia central (Hz)
- **FilterWidth** (float): Anchura de frecuencia (Hz)
- **Level** (float): Intensidad del efecto (0.0 - 1.0)
- **MonoLevel** (float): Supresión del canal central (0.0 - 1.0)

**Uso Típico:**
```csharp
var effect = new KaraokeAudioEffect();
effect.FilterBand = 220;
effect.FilterWidth = 100;
```

## Efectos de Retardo y Modulación

### Eco
**Elemento de Audio:** `audioecho`

Crea efectos de eco/retardo.

**Propiedades:**
- **Delay** (TimeSpan): Tiempo de retardo del eco
- **MaxDelay** (TimeSpan): Tamaño máximo del búfer de retardo
- **Intensity** (float): Volumen del eco (0.0 - 1.0)
- **Feedback** (float): Repeticiones del eco (0.0 - 1.0)

**Uso Típico:**
```csharp
var effect = new EchoAudioEffect(TimeSpan.FromMilliseconds(250));
effect.Intensity = 0.6f;
effect.Feedback = 0.3f;
```

### Reverberación
**Elemento de Audio:** `freeverb`

Añade acústica de sala y profundidad espacial.

**Propiedades:**
- **RoomSize** (float): Tamaño de la sala simulada (0.0 - 1.0)
- **Damping** (float): Amortiguación de alta frecuencia (0.0 - 1.0)
- **Level** (float): Mezcla húmedo/seco (0.0 - 1.0)
- **Width** (float): Anchura estéreo (0.0 - 1.0)

**Uso Típico:**
```csharp
var effect = new ReverberationAudioEffect();
effect.RoomSize = 0.7f; // Sala grande
effect.Level = 0.4f;
```

### Flanger
Crea efecto de barrido tipo "avión a reacción".

**Propiedades:**
- **Delay** (TimeSpan): Retardo base (1-10 ms típico)
- **Frequency** (float): Tasa del LFO (0.1-2 Hz)
- **PhaseInvert** (bool): Invierte fase de la señal retardada

**Uso Típico:**
```csharp
var effect = new FlangerAudioEffect(
    TimeSpan.FromMilliseconds(3), // Retardo
    0.5f,                         // Frecuencia
    false);                       // Inversión de fase
```

### Phaser
Crea efecto de modulación por desplazamiento de fase.

**Propiedades:**
- **Depth** (byte): Profundidad del efecto (0-255)
- **Frequency** (float): Frecuencia del LFO
- **Stages** (byte): Número de etapas (2-24)
- **Feedback** (byte): Cantidad de retroalimentación
- **DryWetRatio** (byte): Ratio de mezcla (0-255)
- **StartPhase** (float): Fase inicial del LFO (radianes)

## Ecualización y Filtrado

### Ecualizador de 10 Bandas
**Elemento de Audio:** `equalizer-10bands`

EQ gráfico con frecuencias fijas.

**Frecuencias de Banda:**
- Banda 0: 29 Hz (Sub-bajo)
- Banda 1: 59 Hz (Bajo)
- Banda 2: 119 Hz (Bajo superior)
- Banda 3: 237 Hz (Rango medio bajo)
- Banda 4: 474 Hz (Rango medio)
- Banda 5: 947 Hz (Rango medio superior)
- Banda 6: 1889 Hz (Presencia)
- Banda 7: 3770 Hz (Presencia superior)
- Banda 8: 7523 Hz (Brillo)
- Banda 9: 15011 Hz (Aire)

**Rango de Ganancia:** -24 dB a +12 dB por banda

**Uso Típico:**
```csharp
double[] levels = new double[10] { 0, 3, 2, 0, -1, 0, 2, 1, 0, -1 };
var effect = new Equalizer10AudioEffect(levels);
```

### Ecualizador Paramétrico
Frecuencia, Q y ganancia ajustables por banda.

**Propiedades:**
- **Bands**: Array de ParametricEqualizerBand (1-64 bandas)
  - Frequency (Hz)
  - Q (ancho de banda)
  - Gain (dB)

**Uso Típico:**
```csharp
var effect = new EqualizerParametricAudioEffect(3);
effect.Bands[0].Frequency = 200;
effect.Bands[0].Q = 1.5;
effect.Bands[0].Gain = -3;
effect.Update();
```

### Filtro Pasa-Altos
**Elemento de Audio:** `audiocheblimit` (Chebyshev) o filtro simple

Atenúa frecuencias por debajo del corte.

**Propiedades:**
- **CutOff** (uint): Frecuencia de corte en Hz

**Frecuencias Comunes:**
- 20-40 Hz: Eliminación sub-sónica
- 60-80 Hz: Eliminación de retumbe
- 100-150 Hz: Mejora de claridad

### Filtro Pasa-Bajos
Atenúa frecuencias por encima del corte.

**Propiedades:**
- **CutOff** (uint): Frecuencia de corte en Hz

**Frecuencias Comunes:**
- 15000-20000 Hz: Reducción suave de aire
- 8000-10000 Hz: Calidez
- 3000-5000 Hz: Efecto telefónico

### Filtro Pasa-Banda
Deja pasar frecuencias dentro de un rango.

**Propiedades:**
- **CutOffLow** (float): Límite de frecuencia inferior
- **CutOffHigh** (float): Límite de frecuencia superior

### Filtro de Rechazo de Banda (Notch)
Atenúa una banda de frecuencia estrecha.

**Propiedades:**
- **CutOff** (uint): Frecuencia central a atenuar

**Uso Típico:** Eliminar zumbido a 50/60 Hz o frecuencias de retroalimentación.

### Chebyshev Pasa-Banda/Rechazo-Banda
**Elemento de Audio:** `audiochebband`

Filtro pronunciado con características configurables.

**Propiedades:**
- **Mode**: Pasa-banda o Rechazo-banda
- **LowerFrequency** (float): Límite inferior (Hz)
- **UpperFrequency** (float): Límite superior (Hz)
- **Poles** (int): Orden del filtro (2-12)
- **Type** (int): Tipo I o Tipo II (1 o 2)
- **Ripple** (float): Cantidad de rizado (dB)

### Chebyshev Límite (Pasa-Bajos/Pasa-Altos)
**Elemento de Audio:** `audiocheblimit`

Filtro pasa-bajos o pasa-altos pronunciado.

**Propiedades:**
- **Mode**: Pasa-bajos o Pasa-altos
- **CutOffFrequency** (float): Corte (Hz)
- **Poles** (int): Orden del filtro (2-12)
- **Type** (int): Tipo I o Tipo II (1 o 2)
- **Ripple** (float): Cantidad de rizado (dB)

## Procesamiento Dinámico

### Compresor/Expansor
**Elemento de Audio:** `audiodynamic`

Control de rango dinámico.

**Propiedades:**
- **Mode**: Compresor o Expansor
- **Ratio** (double): Ratio de compresión/expansión
- **Threshold** (double): Nivel de activación (0.0 - 1.0)
- **Characteristics**: Rodilla dura (Hard knee) o Rodilla suave (Soft knee)

**Uso Típico:**
```csharp
var effect = new CompressorExpanderAudioEffect();
effect.Mode = AudioCompressorMode.Compressor;
effect.Ratio = 4.0;  // Compresión 4:1
effect.Threshold = 0.6;
effect.Characteristics = AudioDynamicCharacteristics.SoftKnee;
```

## Tono y Tempo

### Cambio de Tono
Usa algoritmo SoundTouch para cambiar el tono sin afectar el tempo.

**Propiedades:**
- **Pitch** (float): Multiplicador de tono (0.5 = octava abajo, 2.0 = octava arriba)

**Intervalos Musicales:**
- 0.5 = -12 semitonos
- 1.059 = +1 semitono
- 1.122 = +2 semitonos
- 2.0 = +12 semitonos

### Escalar Tempo
**Elemento de Audio:** `scaletempo`

Cambia el tempo sin afectar el tono (algoritmo WSOLA).

**Propiedades:**
- **Rate** (double): Multiplicador de velocidad (0.5 = media velocidad, 2.0 = doble velocidad)
- **Stride** (TimeSpan): Longitud de segmento (predeterminado 30ms)
- **Overlap** (double): Porcentaje de superposición (predeterminado 0.2)
- **Search** (TimeSpan): Ventana de búsqueda (predeterminado 14ms)

**Uso Típico:**
```csharp
var effect = new ScaleTempoAudioEffect(1.5); // 50% más rápido
effect.Stride = TimeSpan.FromMilliseconds(25);
```

### Tempo
Cambio de tempo simple (puede afectar el tono).

## Mejora de Frecuencia

### TrueBass
Mejora las frecuencias bajas.

**Propiedades:**
- **Frequency** (int): Límite de frecuencia superior (Hz)
- **Volume** (ushort): Cantidad de amplificación (0-10000)

**Uso Típico:**
```csharp
var effect = new TrueBassAudioEffect(100, 5000); // Refuerzo hasta 100 Hz
```

### Mejora de Agudos
Mejora las frecuencias altas.

**Propiedades:**
- **Frequency** (int): Límite de frecuencia inferior (Hz)
- **Volume** (ushort): Cantidad de amplificación (0-10000)

## Efectos Avanzados

### Amplificación Dinámica
Ajuste automático de nivel.

### Inversión de Fase
Invierte la fase del audio.

**Uso:** Útil para efectos de cancelación de fase o corrección de problemas de fase.

### Sonido 3D
Crea efecto espacial 3D.

**Propiedades:**
- **Value** (uint): Amplificación 3D (1-20000, 1000 = bypass)

### Orden de Canales
Reordena los canales de audio.

### Mezcla Descendente (Down Mix)
Convierte multicanal a menos canales.

### Fundido
Efectos de fundido de entrada/salida.

## Reducción de Ruido y Limpieza

### Eliminar Silencio
**Elemento de Audio:** Implementación personalizada

Elimina automáticamente secciones silenciosas.

**Propiedades:**
- **Threshold** (double): Nivel de detección de silencio (0.0 - 1.0)
- **Squash** (bool): Eliminar completamente (true) o reducir nivel (false)

**Uso Típico:**
```csharp
var effect = new RemoveSilenceAudioEffect("remove-silence");
effect.Threshold = 0.05;
effect.Squash = true;
```

### Audio RNNoise
**Elemento de Audio:** `audiornnoise` (plugin rsaudiofx)

Reducción de ruido basada en RNN.

**Propiedades:**
- **VadThreshold** (float): Umbral de detección de actividad de voz (0.0 - 1.0)

**Uso Típico:**
```csharp
var effect = new AudioRNNoiseAudioEffect(0.5f);
```

### Audio Loud Norm
**Elemento de Audio:** `audioloudnorm` (plugin rsaudiofx)

Normalización de sonoridad EBU R128.

**Propiedades:**
- **LoudnessTarget** (double): Objetivo en LUFS (-70.0 a -5.0, predeterminado -24.0)
- **LoudnessRangeTarget** (double): Rango en LU (1.0 a 20.0, predeterminado 7.0)
- **MaxTruePeak** (double): Pico máximo en dbTP (-9.0 a 0.0, predeterminado -2.0)
- **Offset** (double): Ganancia de compensación en LU (-99.0 a 99.0, predeterminado 0.0)

## Análisis y Medición

### Nivel EBU R128
**Elemento de Audio:** `ebur128level` (plugin rsaudiofx)

Mide la sonoridad según el estándar EBU R128.

**Salida:**
- Sonoridad momentánea (LUFS)
- Sonoridad a corto plazo (LUFS)
- Sonoridad integrada (LUFS)

### Renderizado HRTF
**Elemento de Audio:** `hrtfrender` (plugin rsaudiofx)

Audio espacial con Función de Transferencia Relacionada con la Cabeza.

**Propiedades:**
- **Azimuth** (double): Dirección horizontal (grados)
- **Elevation** (double): Dirección vertical (grados)

## Efectos Especializados

### RS Audio Echo
**Elemento de Audio:** `rsaudioecho` (plugin rsaudiofx)

Eco de alta calidad del plugin rsaudiofx.

**Propiedades:**
- **Delay** (int): Retardo en milisegundos
- **Intensity** (float): Intensidad del eco (0-1)
- **Feedback** (float): Cantidad de retroalimentación (0-1)

### Filtro Csound
**Elemento de Audio:** `csoundfilter`

Síntesis de audio avanzada usando Csound.

**Propiedades:**
- **CsdPath** (string): Ruta al archivo de script Csound (.csd)

**Plataforma:** Requiere instalación de Csound.

## Efectos DirectSound (SDKs Classic - Solo Windows)

Los siguientes efectos están disponibles solo en Video Capture SDK, Media Player SDK y Video Edit SDK en Windows. Usan tecnología DirectSound/DirectX.

### DS Chorus
**Disponibilidad:** ❌ Media Blocks | ✅ SDKs Classic (solo Windows)  
**Implementación:** DMO (Dynamic Media Object) de DirectSound

Crea un efecto de coro con múltiples copias retardadas y moduladas.

**Propiedades:**
- **WetDryMix** (float): Mezcla seco/húmedo (0-100)
- **Depth** (float): Profundidad de modulación (0-100)
- **Feedback** (float): Cantidad de retroalimentación (0-100)
- **Frequency** (float): Frecuencia del LFO (0-10 Hz)
- **Waveform**: Sinusoidal o Triangular
- **Delay** (float): Retardo base (0-20 ms)
- **Phase**: Relación de fase para estéreo (-180 a 180 grados)

**Uso Típico:**
```csharp
videoCaptureCore.Audio_Effects_DS_Chorus(0, "chorus", true,
    wetDryMix: 50, depth: 25, feedback: 25, frequency: 1.1f,
    waveform: DSChorusWaveForm.Sine, delay: 16, phase: DSChorusPhase.Phase90);
```

### DS Distortion
**Disponibilidad:** ❌ Media Blocks | ✅ SDKs Classic (solo Windows)  
**Implementación:** DMO de DirectSound

Añade distorsión/overdrive a la señal de audio.

**Propiedades:**
- **Gain** (float): Ganancia pre-distorsión (-60 a 0 dB)
- **Edge** (float): Cantidad de distorsión (0-100%)
- **PostEQCenterFrequency** (float): Centro del EQ post (100-8000 Hz)
- **PostEQBandwidth** (float): Ancho de banda del EQ post (100-8000 Hz)
- **PreLowpassCutoff** (float): Pasa-bajos pre-distorsión (100-8000 Hz)

**Uso Típico:**
```csharp
videoCaptureCore.Audio_Effects_DS_Distortion(0, "distortion", true,
    gain: -18, edge: 50, postEQCenterFrequency: 2400,
    postEQBandwidth: 2400, preLowpassCutoff: 8000);
```

### DS Gargle
**Disponibilidad:** ❌ Media Blocks | ✅ SDKs Classic (solo Windows)  
**Implementación:** DMO de DirectSound

Crea un efecto de gárgaras/trémolo de modulación.

**Propiedades:**
- **RateHz** (uint): Tasa de modulación (1-1000 Hz)
- **WaveShape**: Onda triangular o cuadrada

**Uso Típico:**
```csharp
videoCaptureCore.Audio_Effects_DS_Gargle(0, "gargle", true,
    rateHz: 20, waveShape: DSGargleWaveForm.Triangle);
```

### DS Reverb (I3DL2)
**Disponibilidad:** ❌ Media Blocks | ✅ SDKs Classic (solo Windows)  
**Implementación:** DMO I3DL2 Reverb de DirectSound

Reverberación profesional con modelado ambiental.

**Propiedades:**
- **Room** (int): Nivel de efecto de sala (-10000 a 0 mB)
- **RoomHF** (int): Efecto de sala de alta frecuencia (-10000 a 0 mB)
- **RoomRolloffFactor** (float): Factor de decaimiento (0 a 10)
- **DecayTime** (float): Tiempo de decaimiento (0.1 a 20 segundos)
- **DecayHFRatio** (float): Ratio de decaimiento HF (0.1 a 2.0)
- **Reflections** (int): Reflexiones tempranas (-10000 a 1000 mB)
- **ReflectionsDelay** (float): Retardo de reflexiones (0 a 0.3 segundos)
- **Reverb** (int): Nivel de reverberación tardía (-10000 a 2000 mB)
- **ReverbDelay** (float): Retardo de reverberación (0 a 0.1 segundos)
- **Diffusion** (float): Difusión (0 a 100%)
- **Density** (float): Densidad (0 a 100%)
- **HFReference** (float): Referencia HF (20 a 20000 Hz)

### DS Waves Reverb
**Disponibilidad:** ❌ Media Blocks | ✅ SDKs Classic (solo Windows)  
**Implementación:** DMO Waves Reverb de DirectSound

Reverberación simplificada con parámetros básicos.

**Propiedades:**
- **InGain** (float): Ganancia de entrada (0 a 96 dB)
- **ReverbMix** (float): Mezcla de reverberación (0 a 96 dB)
- **ReverbTime** (float): Tiempo de reverberación (0.001 a 3000 ms)
- **HighFreqRTRatio** (float): Ratio de tiempo de reverberación HF (0.001 a 0.999)

**Uso Típico:**
```csharp
videoCaptureCore.Audio_Effects_DS_WavesReverb(0, "reverb", true,
    inGain: 0, reverbMix: -10, reverbTime: 1000, highFreqRTRatio: 0.001f);
```

## Disponibilidad por Plataforma

La mayoría de los efectos de audio están disponibles en:
- Windows
- macOS
- Linux
- iOS
- Android

Sin embargo, hay diferencias importantes entre tipos de SDK:

### Media Blocks SDK / VideoCaptureCoreX / MediaPlayerCoreX (Multiplataforma)
Todos los efectos multiplataforma están disponibles en todas las plataformas listadas arriba.

### SDKs Classic (Solo Windows)
Video Capture SDK, Media Player SDK y Video Edit SDK usan efectos DSP solo de Windows.

**Efectos específicos de plataforma:**
- Filtro Csound: Solo Windows, macOS, Linux (requiere Csound)
- Efectos RS Audio: Requiere plugin rsaudiofx
- Efectos DirectSound (Chorus, Distortion, Gargle, Reverb, Waves Reverb): Solo Windows, solo SDKs Classic

## Matriz de Disponibilidad de Efectos

| Efecto | SDKs Multiplataforma | SDKs Classic | Plataformas |
|--------|----------------------|--------------|-------------|
| **Control de Volumen/Nivel** |
| Volume | ✅ | ✅ | Multiplataforma / Windows |
| Amplify | ✅ | ✅ | Multiplataforma / Windows |
| **Procesamiento Estéreo** |
| Balance | ✅ | ❌ | Multiplataforma |
| Wide Stereo | ✅ | ❌ | Multiplataforma |
| Karaoke | ✅ | ❌ | Multiplataforma |
| **Retardo y Modulación** |
| Echo | ✅ | ✅ | Multiplataforma / Windows |
| Reverberation (Freeverb) | ✅ | ❌ | Multiplataforma |
| Flanger | ✅ | ✅ | Multiplataforma / Windows |
| Phaser | ✅ | ✅ | Multiplataforma / Windows |
| **Tono y Tempo** |
| Pitch Shift | ✅ | ✅ | Multiplataforma / Windows |
| Scale Tempo | ✅ | ❌ | Multiplataforma |
| Tempo | ✅ | ✅ | Multiplataforma / Windows |
| **Ecualización** |
| Equalizer 10-band | ✅ | ❌ | Multiplataforma |
| Equalizer Parametric | ✅ | ✅ | Multiplataforma / Windows |
| **Filtrado** |
| High-Pass | ✅ | ✅ | Multiplataforma / Windows |
| Low-Pass | ✅ | ✅ | Multiplataforma / Windows |
| Band-Pass | ✅ | ✅ | Multiplataforma / Windows |
| Notch | ✅ | ✅ | Multiplataforma / Windows |
| Chebyshev Band Pass/Reject | ✅ | ❌ | Multiplataforma |
| Chebyshev Limit | ✅ | ❌ | Multiplataforma |
| **Procesamiento Dinámico** |
| Compressor/Expander | ✅ | ❌ | Multiplataforma |
| Dynamic Amplify | ✅ | ✅ | Multiplataforma / Windows |
| **Mejora de Frecuencia** |
| TrueBass | ✅ | ✅ | Multiplataforma / Windows |
| Treble Enhancer | ✅ | ✅ | Multiplataforma / Windows |
| **Efectos Avanzados** |
| Phase Invert | ✅ | ✅ | Multiplataforma / Windows |
| Sound 3D | ✅ | ✅ | Multiplataforma / Windows |
| Channel Order | ✅ | ✅ | Multiplataforma / Windows |
| Down Mix | ✅ | ✅ | Multiplataforma / Windows |
| Fade | ✅ | ✅ | Multiplataforma / Windows |
| **Reducción de Ruido** |
| Remove Silence | ✅ | ❌ | Multiplataforma |
| Audio RNNoise | ✅ | ❌ | Multiplataforma (requiere plugin) |
| Audio Loud Norm | ✅ | ❌ | Multiplataforma (requiere plugin) |
| **Análisis** |
| EBU R128 Level | ✅ | ❌ | Multiplataforma (requiere plugin) |
| **Audio Espacial** |
| HRTF Render | ✅ | ❌ | Multiplataforma (requiere plugin) |
| **Especializados** |
| RS Audio Echo | ✅ | ❌ | Multiplataforma (requiere plugin) |
| Csound Filter | ✅ | ❌ | Windows/macOS/Linux (requiere Csound) |
| **Efectos DirectSound (Solo Windows Classic)** |
| DS Chorus | ❌ | ✅ | Solo Windows |
| DS Distortion | ❌ | ✅ | Solo Windows |
| DS Gargle | ❌ | ✅ | Solo Windows |
| DS Reverb (I3DL2) | ❌ | ✅ | Solo Windows |
| DS Waves Reverb | ❌ | ✅ | Solo Windows |

**Leyenda:**
- ✅ = Disponible
- ❌ = No disponible
- **SDKs Multiplataforma** = Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **SDKs Classic** = Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore) - Solo Windows

## Referencia de Elementos de Audio

| Efecto | Elemento de Audio | Plugin |
|--------|-------------------|--------|
| Volume | volume | coreelements |
| Amplify | audioamplify | audiofx |
| Balance | audiopanorama | audiofx |
| Echo | audioecho | audiofx |
| Karaoke | audiokaraoke | audiofx |
| Wide Stereo | stereo | audiofx |
| Reverberation | freeverb | freeverb |
| Equalizer 10-band | equalizer-10bands | audiofx |
| High-Pass | audiocheblimit | audiofx |
| Low-Pass | audiocheblimit | audiofx |
| Chebyshev Band | audiochebband | audiofx |
| Chebyshev Limit | audiocheblimit | audiofx |
| Compressor | audiodynamic | audiofx |
| Scale Tempo | scaletempo | audiofx |
| Pitch Shift | pitch | soundtouch |
| Audio RNNoise | audiornnoise | rsaudiofx |
| Audio Loud Norm | audioloudnorm | rsaudiofx |
| EBU R128 Level | ebur128level | rsaudiofx |
| RS Audio Echo | rsaudioecho | rsaudiofx |
| HRTF Render | hrtfrender | rsaudiofx |
| Csound Filter | csoundfilter | csound |

## Ver También

- [Procesamiento de Audio y Efectos](index.md) - Ejemplos de uso de bloques
- [Documentación de Elementos de Audio](https://gstreamer.freedesktop.org/documentation/plugins_doc.html)
- [Plugin rsaudiofx](https://github.com/sdroege/gst-plugin-rs/tree/main/audio/audiofx)
