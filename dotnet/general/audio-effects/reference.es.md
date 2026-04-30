---
title: Referencia API de Efectos de Audio para .NET - Parámetros
description: API de más de 30 efectos de audio en VisioForge .NET SDKs. Volumen, EQ, compresor, reverb, eco, filtros, cambio de tono y reducción de ruido en C#.
sidebar_label: Referencia de Efectos de Audio
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
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
  - Editing
  - Effects
  - C#
primary_api_classes:
  - VolumeAudioEffect
  - Equalizer10AudioEffect
  - BandPassAudioEffect
  - BalanceAudioEffect
  - WideStereoAudioEffect

---

# Referencia API de Efectos de Audio

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Referencia completa de parámetros para todos los efectos de audio disponibles en los SDKs .NET de VisioForge. Cada efecto multiplataforma envuelve un elemento de GStreamer y admite cambios de parámetros en tiempo real desde código C# durante la reproducción. Los efectos multiplataforma funcionan en Windows, macOS, Linux, iOS y Android.

Para una descripción general de las categorías de efectos y patrones de uso, consulte [Efectos de Audio](index.md).

## Efectos de Volumen y Dinámica

### VolumeAudioEffect

**Elemento GStreamer**: `volume`

**Propósito**: Controlar el nivel de volumen de audio con opción de silencio.

**Parámetros**:

- `Level` (double): Multiplicador de volumen
    - Rango: 0.0 a ilimitado
    - Predeterminado: 1.0 (100%)
    - Ejemplos: 0.5 = 50%, 2.0 = 200%
- `Mute` (bool): Silenciar la salida de audio
    - Predeterminado: false

**Uso**:

```csharp
var effect = new VolumeAudioEffect(1.5); // 150% de volumen
effect.Mute = true; // Silenciar temporalmente
```

---

### AmplifyAudioEffect

**Elemento GStreamer**: `audioamplify`

**Propósito**: Amplificar audio con control de recorte.

**Parámetros**:

- `Level` (double): Nivel de amplificación
    - Rango: 1.0 a 10.0
    - Predeterminado: 1.0
- `ClippingMethod` (AmplifyClippingMethod): Cómo manejar los picos
    - Opciones: Normal, WrapNegative, WrapPositive, NoClip
    - Predeterminado: Normal

**Uso**:

```csharp
var effect = new AmplifyAudioEffect(2.0);
effect.ClippingMethod = AmplifyClippingMethod.NoClip;
```

---

### CompressorExpanderAudioEffect

**Elemento GStreamer**: `audiodynamic`

**Propósito**: Compresión o expansión de rango dinámico.

**Parámetros**:

- `Threshold` (double): Umbral de activación
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.0
- `Ratio` (double): Relación de compresión/expansión
    - Rango: 1.0+
    - Predeterminado: 1.0
    - Típico: 2:1 a 10:1 para compresión
- `Mode` (AudioCompressorMode): Compresor o Expansor
    - Predeterminado: Compressor
- `Characteristics` (AudioDynamicCharacteristics): HardKnee o SoftKnee
    - Predeterminado: SoftKnee

**Uso**:

```csharp
var effect = new CompressorExpanderAudioEffect();
effect.Threshold = 0.5;
effect.Ratio = 4.0; // Compresión 4:1
effect.Characteristics = AudioDynamicCharacteristics.SoftKnee;
```

---

### DynamicAmplifyAudioEffect

**Propósito**: Control de ganancia adaptativo.

**Parámetros**:

- `AttackTime` (uint): Tiempo de respuesta en ms
    - Típico: 10-100 ms
- `MaxAmplification` (uint): Ganancia máxima
    - 10000 = 1x (sin cambio)
    - 20000 = amplificación 2x
    - Predeterminado: 10000
- `ReleaseTime` (TimeSpan): Tiempo antes de reanudar la amplificación
    - Típico: 100-1000 ms

**Uso**:

```csharp
var effect = new DynamicAmplifyAudioEffect(50, 20000, TimeSpan.FromMilliseconds(500));
```

---

## Efectos de Ecualización

### Equalizer10AudioEffect

**Elemento GStreamer**: `equalizer-10bands`

**Propósito**: Ecualizador gráfico de 10 bandas con frecuencias fijas.

**Bandas de Frecuencia**:

1. 29 Hz (Sub-graves)
2. 59 Hz (Graves)
3. 119 Hz (Graves)
4. 237 Hz (Medios bajos)
5. 474 Hz (Medios)
6. 947 Hz (Medios)
7. 1889 Hz (Medios altos)
8. 3770 Hz (Presencia)
9. 7523 Hz (Brillo)
10. 15011 Hz (Aire)

**Parámetros**:

- `Levels` (double[]): Ganancia para cada banda en dB
    - Rango por banda: -24 a +12 dB
    - El array debe contener exactamente 10 valores

**Uso**:

```csharp
var levels = new double[] {
    3.0,   // 29 Hz: +3 dB
    2.0,   // 59 Hz: +2 dB
    0.0,   // 119 Hz: 0 dB (sin cambio)
    -2.0,  // 237 Hz: -2 dB
    0.0,   // 474 Hz
    0.0,   // 947 Hz
    1.0,   // 1889 Hz: +1 dB
    2.0,   // 3770 Hz: +2 dB
    3.0,   // 7523 Hz: +3 dB
    4.0    // 15011 Hz: +4 dB
};
var effect = new Equalizer10AudioEffect(levels);
```

---

### EqualizerParametricAudioEffect

**Elemento GStreamer**: `equalizer-nbands`

**Propósito**: Ecualizador paramétrico con bandas configurables.

**Parámetros**:

- `Bands` (ParametricEqualizerBand[]): Array de bandas
    - Cantidad: 1 a 64 bandas
    - Cada banda: Frequency, Gain, Width (ancho de banda en Hz)

**Uso**:

```csharp
var effect = new EqualizerParametricAudioEffect(3);
effect.Bands[0].Frequency = 100;  // Hz
effect.Bands[0].Gain = -6;        // dB
effect.Bands[0].Width = 1.0f;     // ancho de banda
// Configurar otras bandas...
effect.Update(); // Aplicar cambios
```

---

### TrebleEnhancerAudioEffect

**Propósito**: Realzar altas frecuencias.

**Parámetros**:

- `Frequency` (int): Frecuencia inicial en Hz
    - Típico: 4000-8000 Hz
    - Las frecuencias por encima de este valor se realzan
- `Volume` (ushort): Cantidad de realce
    - Rango: 0 a 10000
    - 0 = sin efecto

**Uso**:

```csharp
var effect = new TrebleEnhancerAudioEffect(6000, 5000);
```

---

### TrueBassAudioEffect

**Propósito**: Realzar bajas frecuencias.

**Parámetros**:

- `Frequency` (int): Límite superior de frecuencia en Hz
    - Típico: 100-300 Hz
    - Las frecuencias por debajo de este valor se realzan
- `Volume` (ushort): Cantidad de realce
    - Rango: 0 a 10000
    - 0 = sin efecto

**Uso**:

```csharp
var effect = new TrueBassAudioEffect(150, 5000);
```

---

## Efectos de Filtro

### HighPassAudioEffect

**Implementación**: DSP personalizado (filtro pasa-altos IIR)

**Propósito**: Eliminar frecuencias bajas.

**Parámetros**:

- `CutOff` (uint): Frecuencia de corte en Hz
    - Las frecuencias por debajo se atenúan
    - Típico: 80-200 Hz para voz, 40 Hz para música

**Frecuencias Comunes**:

- 20-40 Hz: Eliminación de subsónicos
- 60-80 Hz: Eliminación de retumbos
- 100-150 Hz: Mejora de claridad

**Uso**:

```csharp
var effect = new HighPassAudioEffect(100); // Eliminar frecuencias por debajo de 100 Hz
```

---

### LowPassAudioEffect

**Implementación**: DSP personalizado (filtro pasa-bajos IIR)

**Propósito**: Eliminar frecuencias altas.

**Parámetros**:

- `CutOff` (uint): Frecuencia de corte en Hz
    - Las frecuencias por encima se atenúan
    - Típico: 8000-12000 Hz para eliminación de sibilancia

**Frecuencias Comunes**:

- 15000-20000 Hz: Reducción suave de aire
- 8000-10000 Hz: Calidez
- 3000-5000 Hz: Efecto telefónico

**Uso**:

```csharp
var effect = new LowPassAudioEffect(10000); // Eliminar frecuencias por encima de 10 kHz
```

---

### BandPassAudioEffect

**Implementación**: DSP personalizado (filtro de variable de estado)

**Propósito**: Permitir solo un rango de frecuencia específico.

**Parámetros**:

- `CutOffHigh` (float): Límite superior de frecuencia en Hz
- `CutOffLow` (float): Límite inferior de frecuencia en Hz

**Uso**:

```csharp
// Constructor: BandPassAudioEffect(cutOffHigh, cutOffLow)
var effect = new BandPassAudioEffect(5000, 300); // Permitir 300-5000 Hz
```

---

### NotchAudioEffect

**Implementación**: DSP personalizado (filtro de rechazo de banda)

**Propósito**: Eliminar una frecuencia específica.

**Parámetros**:

- `CutOff` (uint): Frecuencia central a eliminar en Hz
    - Típico: 50/60 Hz para eliminación de zumbido

**Uso**:

```csharp
var effect = new NotchAudioEffect(60); // Eliminar zumbido de 60 Hz
```

---

### ChebyshevLimitAudioEffect

**Elemento GStreamer**: `audiocheblimit`

**Propósito**: Filtrado pasa-bajos/pasa-altos con corte pronunciado y control de rizado.

**Parámetros**:

- `CutOffFrequency` (float): Frecuencia de corte en Hz
- `Mode` (ChebyshevLimitAudioEffectMode): LowPass o HighPass
- `Poles` (int): Orden del filtro (2-8 típico)
    - Predeterminado: 4
    - Más polos = caída más pronunciada
- `Ripple` (float): Rizado en la banda de paso en dB
    - Predeterminado: 0.25
- `Type` (int): Tipo de Chebyshev (1 o 2)
    - Predeterminado: 1

**Uso**:

```csharp
var effect = new ChebyshevLimitAudioEffect();
effect.CutOffFrequency = 100;
effect.Mode = ChebyshevLimitAudioEffectMode.HighPass;
effect.Poles = 6;
```

---

### ChebyshevBandPassRejectAudioEffect

**Elemento GStreamer**: `audiochebband`

**Propósito**: Filtrado pasa-banda o rechazo de banda con corte pronunciado.

**Parámetros**:

- `LowerFrequency` (float): Límite inferior de la banda en Hz
- `UpperFrequency` (float): Límite superior de la banda en Hz
- `Mode` (ChebyshevBandPassRejectAudioEffectMode): BandPass o BandReject
- `Poles` (int): Orden del filtro (2-8 típico)
    - Predeterminado: 4
- `Ripple` (float): Rizado en la banda de paso en dB
    - Predeterminado: 0.25
- `Type` (int): Tipo de Chebyshev (1 o 2)
    - Predeterminado: 1

**Uso**:

```csharp
var effect = new ChebyshevBandPassRejectAudioEffect();
effect.LowerFrequency = 300;
effect.UpperFrequency = 3000;
effect.Mode = ChebyshevBandPassRejectAudioEffectMode.BandPass;
```

---

## Efectos Espaciales y Estéreo

### BalanceAudioEffect

**Elemento GStreamer**: `audiopanorama`

**Propósito**: Control de balance estéreo (panorámica).

**Parámetros**:

- `Level` (double): Posición de balance
    - Rango: -1.0 a 1.0
    - -1.0 = totalmente a la izquierda
    - 0.0 = centro
    - 1.0 = totalmente a la derecha
    - Predeterminado: 0.0

**Uso**:

```csharp
var effect = new BalanceAudioEffect(-0.5); // Panorámica 50% a la izquierda
```

---

### WideStereoAudioEffect

**Elemento GStreamer**: `stereo`

**Propósito**: Mejorar la amplitud estéreo.

**Parámetros**:

- `Level` (float): Intensidad de ampliación
    - Rango: 0.0+
    - Predeterminado: 0.01
    - Típico: 0.01 a 1.0
    - Valores más altos = campo estéreo más amplio

**Uso**:

```csharp
var effect = new WideStereoAudioEffect();
effect.Level = 0.5f;
```

---

### Sound3DAudioEffect

**Propósito**: Simulación de audio espacial 3D.

**Parámetros**:

- `Value` (uint): Amplificación espacial
    - Rango: 1 a 20000
    - 1000 = neutro (desactivado)
    - < 1000 = estéreo más estrecho
    - > 1000 = estéreo más amplio
    - > 10000 puede distorsionar

**Uso**:

```csharp
var effect = new Sound3DAudioEffect(2000); // 2x amplitud espacial
```

---

### PhaseInvertAudioEffect

**Propósito**: Invertir la fase del audio 180 grados.

**Parámetros**: Ninguno.

**Uso**:

```csharp
var effect = new PhaseInvertAudioEffect();
```

---

### HRTFRenderAudioEffect

**Elemento GStreamer**: `hrtfrender` (rsaudiofx)

**Propósito**: Audio espacial 3D basado en HRTF.

**Parámetros**:

- `HrirFile` (string): Ruta al archivo de datos HRIR
- `InterpolationSteps` (ulong): Calidad de interpolación
    - Predeterminado: 8
- `BlockLength` (ulong): Tamaño del bloque de procesamiento
    - Predeterminado: 512
- `DistanceGain` (float): Factor de atenuación por distancia
    - Predeterminado: 1.0

**Uso**:

```csharp
var effect = new HRTFRenderAudioEffect("/path/to/hrir.dat");
effect.InterpolationSteps = 16; // Mayor calidad
```

---

## Efectos Basados en Tiempo

### EchoAudioEffect

**Elemento GStreamer**: `audioecho`

**Propósito**: Efectos de eco y retardo.

**Parámetros**:

- `Delay` (TimeSpan): Tiempo de retardo del eco
    - No debe exceder MaxDelay
- `MaxDelay` (TimeSpan): Buffer máximo de retardo
    - Debe ser >= Delay
    - Establecer antes de iniciar la reproducción
- `Intensity` (float): Volumen del eco
    - Rango: 0.0 a 1.0
    - Predeterminado: 1.0
- `Feedback` (float): Cantidad de repetición del eco
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.0
    - Mayor = más ecos

**Uso**:

```csharp
var delay = TimeSpan.FromMilliseconds(500);
var effect = new EchoAudioEffect(delay);
effect.Intensity = 0.7f;
effect.Feedback = 0.4f;
```

---

### RSAudioEchoAudioEffect

**Elemento GStreamer**: `rsaudioecho` (rsaudiofx)

**Propósito**: Eco mejorado con controles avanzados.

**Parámetros**:

- `Delay` (TimeSpan): Tiempo de retardo del eco
- `MaxDelay` (TimeSpan): Buffer máximo de retardo
- `Intensity` (double): Intensidad del eco
    - Rango: 0.0 a 1.0
- `Feedback` (double): Cantidad de retroalimentación
    - Rango: 0.0 a 1.0

**Uso**:

```csharp
var effect = new RSAudioEchoAudioEffect();
effect.Delay = TimeSpan.FromMilliseconds(750);
effect.Intensity = 0.6;
effect.Feedback = 0.3;
```

---

### ReverberationAudioEffect

**Elemento GStreamer**: `freeverb`

**Propósito**: Simulación de reverberación de sala.

**Parámetros**:

- `RoomSize` (float): Tamaño virtual de la sala
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.5
    - Mayor = cola de reverb más larga
- `Damping` (float): Absorción de altas frecuencias
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.2
    - Mayor = reverb más oscura
- `Level` (float): Mezcla húmedo/seco
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.5
    - 0 = seco, 1 = húmedo
- `Width` (float): Amplitud estéreo
    - Rango: 0.0 a 1.0
    - Predeterminado: 1.0
    - 0 = mono, 1 = estéreo completo

**Uso**:

```csharp
var effect = new ReverberationAudioEffect();
effect.RoomSize = 0.8f; // Sala grande
effect.Damping = 0.3f;
effect.Level = 0.4f;
```

---

### FadeAudioEffect

**Propósito**: Automatización de fundido de entrada/salida de volumen.

**Parámetros**:

- `StartVolume` (uint): Volumen en el tiempo de inicio
- `StopVolume` (uint): Volumen en el tiempo de fin
- `StartTime` (TimeSpan): Cuándo comienza el fundido
- `StopTime` (TimeSpan): Cuándo se completa el fundido

**Uso**:

```csharp
// Fundido de salida durante 3 segundos comenzando a los 10 segundos
var effect = new FadeAudioEffect(
    100, 0,
    TimeSpan.FromSeconds(10),
    TimeSpan.FromSeconds(13)
);
```

---

## Efectos de Modulación

### PhaserAudioEffect

**Propósito**: Efecto phaser con modulación LFO.

**Parámetros**:

- `Depth` (byte): Profundidad de barrido
    - Rango: 0 a 255
- `DryWetRatio` (byte): Relación de mezcla
    - Rango: 0 a 255
    - 0 = seco, 255 = húmedo
- `Feedback` (byte): Resonancia
    - Rango: -100 a 100
- `Frequency` (float): Velocidad del LFO en Hz
    - Típico: 0.1 a 5 Hz
- `Stages` (byte): Número de etapas
    - Rango: 2 a 24 recomendado
    - Más = efecto más fuerte
- `StartPhase` (float): Fase inicial del LFO en radianes

**Uso**:

```csharp
var effect = new PhaserAudioEffect(
    150,      // profundidad
    128,      // mezcla 50%
    50,       // retroalimentación
    0.5f,     // LFO 0.5 Hz
    6,        // 6 etapas
    0f        // fase inicial
);
```

---

### FlangerAudioEffect

**Propósito**: Efecto flanging con modulación de retardo.

**Parámetros**:

- `Delay` (TimeSpan): Tiempo base de retardo
    - Típico: 1-15 ms
- `Frequency` (float): Velocidad del LFO en Hz
    - Típico: 0.1 a 5 Hz
- `PhaseInvert` (bool): Invertir fase de la señal retardada
    - Predeterminado: false

**Uso**:

```csharp
var effect = new FlangerAudioEffect(
    TimeSpan.FromMilliseconds(5),
    1.0f,
    false
);
```

---

## Efectos de Tono y Tempo

### PitchShiftAudioEffect

**Propósito**: Cambiar el tono sin cambiar la velocidad.

**Parámetros**:

- `Pitch` (float): Relación de cambio de tono
    - 1.0 = sin cambio
    - 2.0 = una octava arriba
    - 0.5 = una octava abajo
    - Rango típico: 0.5 a 2.0

**Intervalos Musicales**:

- 0.5 = -12 semitonos
- 1.059 = +1 semitono
- 1.122 = +2 semitonos
- 2.0 = +12 semitonos

**Uso**:

```csharp
var effect = new PitchShiftAudioEffect(1.5f); // Subir una quinta
```

---

### ScaleTempoAudioEffect

**Elemento GStreamer**: `scaletempo`

**Propósito**: Cambiar el tempo sin cambiar el tono (algoritmo WSOLA).

**Parámetros**:

- `Rate` (double): Velocidad de reproducción
    - 1.0 = normal
    - 2.0 = doble velocidad
    - 0.5 = mitad de velocidad
    - Predeterminado: 1.0
- `Stride` (TimeSpan): Paso de procesamiento
    - Predeterminado: 30 ms
- `Overlap` (double): Porcentaje de superposición
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.2
- `Search` (TimeSpan): Ventana de búsqueda
    - Predeterminado: 14 ms

**Uso**:

```csharp
var effect = new ScaleTempoAudioEffect(1.5); // Velocidad 1.5x
```

---

## Efectos Especiales

### KaraokeAudioEffect

**Elemento GStreamer**: `audiokaraoke`

**Propósito**: Eliminar las voces con panorámica central.

**Parámetros**:

- `FilterBand` (float): Frecuencia central en Hz
    - Predeterminado: 220 Hz
    - Típico: 80-400 Hz
- `FilterWidth` (float): Ancho de banda del filtro en Hz
    - Predeterminado: 100 Hz
- `Level` (float): Intensidad del efecto
    - Rango: 0.0 a 1.0
    - Predeterminado: 1.0
- `MonoLevel` (float): Nivel del canal mono
    - Rango: 0.0 a 1.0
    - Predeterminado: 1.0

**Uso**:

```csharp
var effect = new KaraokeAudioEffect();
effect.FilterBand = 250f;
effect.Level = 1.0f;
```

---

### RemoveSilenceAudioEffect

**Propósito**: Eliminar automáticamente las secciones silenciosas.

**Parámetros**:

- `Threshold` (double): Umbral de detección de silencio
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.05
    - Menor = más sensible
- `Squash` (bool): Eliminar vs. reducir silencio
    - true = eliminar completamente
    - false = reducir nivel
    - Predeterminado: true

**Uso**:

```csharp
var effect = new RemoveSilenceAudioEffect("silence-remover");
effect.Threshold = 0.02;
effect.Squash = true;
```

---

### CsoundAudioEffect

**Elemento GStreamer**: `csoundfilter`

**Propósito**: Procesamiento de audio basado en Csound.

**Plataformas**: Windows, macOS, Linux (requiere instalación de Csound).

**Parámetros**:

- `CsdText` (string): Documento CSD de Csound como texto
- `Location` (string): Ruta al archivo CSD
- `Loop` (bool): Repetir la partitura continuamente
    - Predeterminado: false
- `ScoreOffset` (double): Tiempo de inicio en segundos
    - Predeterminado: 0.0

**Uso**:

```csharp
var csd = @"<CsoundSynthesizer>
<CsInstruments>
; Su código Csound aquí
</CsInstruments>
<CsScore>
; Su partitura aquí
</CsScore>
</CsoundSynthesizer>";

var effect = new CsoundAudioEffect("my-csound", csd);
effect.Loop = false;
```

---

## Reducción de Ruido y Medición

### AudioRNNoiseAudioEffect

**Elemento GStreamer**: `audiornnoise` (rsaudiofx)

**Propósito**: Reducción de ruido basada en IA.

**Parámetros**:

- `VadThreshold` (float): Umbral de detección de actividad vocal
    - Rango: 0.0 a 1.0
    - Predeterminado: 0.0
    - Mayor = más sensible a la voz

**Uso**:

```csharp
var effect = new AudioRNNoiseAudioEffect();
effect.VadThreshold = 0.5f;
```

---

### AudioLoudNormAudioEffect

**Elemento GStreamer**: `audioloudnorm` (rsaudiofx)

**Propósito**: Normalización de sonoridad EBU R128.

**Parámetros**:

- `LoudnessTarget` (double): Sonoridad objetivo en LUFS
    - Rango: -70.0 a -5.0
    - Predeterminado: -24.0
- `LoudnessRangeTarget` (double): Rango objetivo en LU
    - Rango: 1.0 a 20.0
    - Predeterminado: 7.0
- `MaxTruePeak` (double): Pico verdadero máximo en dbTP
    - Rango: -9.0 a 0.0
    - Predeterminado: -2.0
- `Offset` (double): Ganancia de compensación en LU
    - Rango: -99.0 a 99.0
    - Predeterminado: 0.0

**Uso**:

```csharp
var effect = new AudioLoudNormAudioEffect();
effect.LoudnessTarget = -16.0; // Estándar de streaming
effect.MaxTruePeak = -1.0;
```

---

### EbuR128LevelAudioEffect

**Elemento GStreamer**: `ebur128level` (rsaudiofx)

**Propósito**: Medición de sonoridad EBU R128.

**Parámetros**:

- `Mode` (EbuR128Mode): Tipos de medición a calcular
    - Opciones: Momentary, ShortTerm, Global, LoudnessRange, SamplePeak, TruePeak, All
    - Predeterminado: All
- `PostMessages` (bool): Publicar mensajes de medición
    - Predeterminado: true
- `Interval` (TimeSpan): Intervalo de actualización de medición
    - Predeterminado: 1 segundo

**Uso**:

```csharp
var effect = new EbuR128LevelAudioEffect();
effect.Mode = EbuR128Mode.All;
effect.Interval = TimeSpan.FromSeconds(0.5);
```

---

## Gestión de Canales

### ChannelOrderAudioEffect

**Propósito**: Remapear canales de audio.

**Parámetros**:

- `Orders` (byte[,]): Array 2D de pares [destino, origen]
    - Formato: [[destino0, origen0], [destino1, origen1], ...]
    - Los canales están indexados desde cero

**Uso**:

```csharp
// Intercambiar canales izquierdo y derecho
var orders = new byte[2, 2] {
    { 0, 1 },  // Canal de salida 0 recibe el canal de entrada 1 (derecho)
    { 1, 0 }   // Canal de salida 1 recibe el canal de entrada 0 (izquierdo)
};
var effect = new ChannelOrderAudioEffect(orders);
```

---

### DownMixAudioEffect

**Implementación**: DSP personalizado (promediado de canales)

**Propósito**: Reducir el número de canales (ej., 5.1 a estéreo).

**Parámetros**: Ninguno (mezcla descendente automática).

**Uso**:

```csharp
var effect = new DownMixAudioEffect();
```

---

## Efectos DirectSound (SDKs Classic, Solo Windows)

Los siguientes efectos están disponibles solo en Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore) y Video Edit SDK (VideoEditCore) en Windows. Utilizan tecnología DirectSound/DirectX.

### DS Chorus

Crea un efecto de coro con múltiples copias retardadas y moduladas.

**Propiedades**:

- **WetDryMix** (float): Mezcla seco/húmedo (0-100)
- **Depth** (float): Profundidad de modulación (0-100)
- **Feedback** (float): Cantidad de retroalimentación (0-100)
- **Frequency** (float): Frecuencia del LFO (0-10 Hz)
- **Waveform**: Sine o Triangle
- **Delay** (float): Retardo base (0-20 ms)
- **Phase**: Relación de fase para estéreo (-180 a 180 grados)

**Uso**:

```csharp
// Firma: (int streamIndex, string name, float delay, float depth,
//         float feedback, float frequency, DSChorusPhase phase,
//         DSChorusWaveForm waveformTriangle, float wetDryMix)
videoCaptureCore.Audio_Effects_DS_Chorus(0, "chorus",
    delay: 16, depth: 25, feedback: 25, frequency: 1.1f,
    phase: DSChorusPhase.Phase90, waveformTriangle: DSChorusWaveForm.Sine,
    wetDryMix: 50);
```

---

### DS Distortion

Agrega distorsión/overdrive a la señal de audio.

**Propiedades**:

- **Gain** (float): Ganancia de pre-distorsión (-60 a 0 dB)
- **Edge** (float): Cantidad de distorsión (0-100%)
- **PostEQCenterFrequency** (float): Centro del EQ post (100-8000 Hz)
- **PostEQBandwidth** (float): Ancho de banda del EQ post (100-8000 Hz)
- **PreLowpassCutoff** (float): Pasa-bajos pre-distorsión (100-8000 Hz)

**Uso**:

```csharp
// Firma: (int streamIndex, string name, float edge, float gain,
//         float postEQBandwidth, float postEQCenterFrequency,
//         float preLowpassCutOff)
videoCaptureCore.Audio_Effects_DS_Distortion(0, "distortion",
    edge: 50, gain: -18, postEQBandwidth: 2400,
    postEQCenterFrequency: 2400, preLowpassCutOff: 8000);
```

---

### DS Gargle

Crea un efecto de modulación de gárgara/trémolo.

**Propiedades**:

- **RateHz** (int): Tasa de modulación (1-1000 Hz)
- **WaveForm**: Onda Triangle o Square

**Uso**:

```csharp
// Firma: (int streamIndex, string name, int rateHz, DSGargleWaveForm waveForm)
videoCaptureCore.Audio_Effects_DS_Gargle(0, "gargle",
    rateHz: 20, waveForm: DSGargleWaveForm.Triangle);
```

---

### DS Reverb (I3DL2)

Reverberación profesional con modelado ambiental.

**Propiedades**:

- **Room** (int): Nivel de efecto de sala (-10000 a 0 mB)
- **RoomHF** (int): Efecto de sala en altas frecuencias (-10000 a 0 mB)
- **RoomRolloffFactor** (float): Factor de atenuación (0 a 10)
- **DecayTime** (float): Tiempo de decaimiento (0.1 a 20 segundos)
- **DecayHFRatio** (float): Relación de decaimiento en HF (0.1 a 2.0)
- **Reflections** (int): Reflexiones tempranas (-10000 a 1000 mB)
- **ReflectionsDelay** (float): Retardo de reflexiones (0 a 0.3 segundos)
- **Reverb** (int): Nivel de reverb tardía (-10000 a 2000 mB)
- **ReverbDelay** (float): Retardo de reverb (0 a 0.1 segundos)
- **Diffusion** (float): Difusión (0 a 100%)
- **Density** (float): Densidad (0 a 100%)
- **HFReference** (float): Referencia de HF (20 a 20000 Hz)

---

### DS Waves Reverb

Reverberación simplificada con parámetros básicos.

**Propiedades**:

- **InGain** (float): Ganancia de entrada (0 a 96 dB)
- **ReverbMix** (float): Mezcla de reverb (0 a 96 dB)
- **ReverbTime** (float): Tiempo de reverb (0.001 a 3000 ms)
- **HighFreqRTRatio** (float): Relación de tiempo de reverb en HF (0.001 a 0.999)

**Uso**:

```csharp
// Firma: (int streamIndex, string name, float highFreqRTRatio,
//         float inGain, float reverbMix, float reverbTime)
videoCaptureCore.Audio_Effects_DS_WavesReverb(0, "reverb",
    highFreqRTRatio: 0.001f, inGain: 0, reverbMix: -10, reverbTime: 1000);
```

---

## Matriz de Disponibilidad de Efectos

| Efecto | SDKs Multiplataforma | SDKs Classic | Plataformas |
|--------|----------------------|--------------|-------------|
| **Control de Volumen/Nivel** | | | |
| Volume | Sí | Sí | Multiplataforma / Windows |
| Amplify | Sí | Sí | Multiplataforma / Windows |
| **Procesamiento Estéreo** | | | |
| Balance | Sí | No | Multiplataforma |
| Wide Stereo | Sí | No | Multiplataforma |
| Karaoke | Sí | No | Multiplataforma |
| **Retardo y Modulación** | | | |
| Echo | Sí | Sí | Multiplataforma / Windows |
| Reverberation (Freeverb) | Sí | No | Multiplataforma |
| Flanger | Sí | Sí | Multiplataforma / Windows |
| Phaser | Sí | Sí | Multiplataforma / Windows |
| **Tono y Tempo** | | | |
| Pitch Shift | Sí | Sí | Multiplataforma / Windows |
| Scale Tempo | Sí | No | Multiplataforma |
| Tempo | Sí | Sí | Multiplataforma / Windows |
| **Ecualización** | | | |
| Equalizer 10-band | Sí | No | Multiplataforma |
| Equalizer Parametric | Sí | Sí | Multiplataforma / Windows |
| **Filtrado** | | | |
| High-Pass | Sí | Sí | Multiplataforma / Windows |
| Low-Pass | Sí | Sí | Multiplataforma / Windows |
| Band-Pass | Sí | Sí | Multiplataforma / Windows |
| Notch | Sí | Sí | Multiplataforma / Windows |
| Chebyshev Band Pass/Reject | Sí | No | Multiplataforma |
| Chebyshev Limit | Sí | No | Multiplataforma |
| **Procesamiento Dinámico** | | | |
| Compressor/Expander | Sí | No | Multiplataforma |
| Dynamic Amplify | Sí | Sí | Multiplataforma / Windows |
| **Realce de Frecuencia** | | | |
| TrueBass | Sí | Sí | Multiplataforma / Windows |
| Treble Enhancer | Sí | Sí | Multiplataforma / Windows |
| **Efectos Avanzados** | | | |
| Phase Invert | Sí | Sí | Multiplataforma / Windows |
| Sound 3D | Sí | Sí | Multiplataforma / Windows |
| Channel Order | Sí | Sí | Multiplataforma / Windows |
| Down Mix | Sí | Sí | Multiplataforma / Windows |
| Fade | Sí | Sí | Multiplataforma / Windows |
| **Reducción de Ruido** | | | |
| Remove Silence | Sí | No | Multiplataforma |
| Audio RNNoise | Sí | No | Multiplataforma (requiere plugin) |
| Audio Loud Norm | Sí | No | Multiplataforma (requiere plugin) |
| **Análisis** | | | |
| EBU R128 Level | Sí | No | Multiplataforma (requiere plugin) |
| **Audio Espacial** | | | |
| HRTF Render | Sí | No | Multiplataforma (requiere plugin) |
| **Especializados** | | | |
| RS Audio Echo | Sí | No | Multiplataforma (requiere plugin) |
| Csound Filter | Sí | No | Windows/macOS/Linux (requiere Csound) |
| **Efectos DirectSound (Solo Windows Classic)** | | | |
| DS Chorus | No | Sí | Solo Windows |
| DS Distortion | No | Sí | Solo Windows |
| DS Gargle | No | Sí | Solo Windows |
| DS Reverb (I3DL2) | No | Sí | Solo Windows |
| DS Waves Reverb | No | Sí | Solo Windows |

**Leyenda**:

- **SDKs Multiplataforma** = Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **SDKs Classic** = Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore) — Solo Windows

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

## Preguntas Frecuentes

???+ "¿Cuál es el valor predeterminado para los parámetros de efectos de audio?"
    Cada efecto tiene valores predeterminados documentados que representan un comportamiento neutro/bypass. Por ejemplo, `VolumeAudioEffect` tiene un nivel predeterminado de 1.0 (100%), `Equalizer10AudioEffect` tiene todas las bandas a 0 dB, y `ReverberationAudioEffect` tiene una simulación de sala moderada predeterminada. Consulte la tabla de parámetros de cada efecto arriba para los valores predeterminados específicos.

???+ "¿Cómo restablezco un efecto de audio a su configuración predeterminada?"
    Cree una nueva instancia del efecto con los parámetros del constructor predeterminado y llame a `Audio_Effects_AddOrUpdate()` para reemplazar la configuración actual. El constructor predeterminado de cada efecto inicializa todos los parámetros a sus valores predeterminados documentados.

???+ "¿Puedo usar múltiples instancias del mismo tipo de efecto?"
    Sí. Los **nombres** de los efectos se utilizan como identificadores únicos, no los tipos de efecto. Para ejecutar múltiples instancias del mismo tipo simultáneamente, asigne a cada instancia un nombre distinto. Llamar a `Audio_Effects_AddOrUpdate()` con un efecto cuyo nombre coincide con uno existente reemplaza esa instancia; un nombre nuevo agrega una nueva instancia a la cadena.

???+ "¿Qué tasas de muestreo y configuraciones de canales son compatibles?"
    Todos los efectos de audio multiplataforma admiten tasas de muestreo estándar (8 kHz a 192 kHz) y configuraciones de canales (mono, estéreo, multicanal). Los efectos se adaptan automáticamente al formato de audio del flujo de entrada. Algunos efectos como `BalanceAudioEffect` y `WideStereoAudioEffect` requieren entrada estéreo.

???+ "¿Cómo elimino un efecto de audio durante la reproducción?"
    Use el método `Audio_Effects_Remove()` con el tipo de efecto para eliminarlo de la cadena de procesamiento durante la reproducción. El cambio surte efecto inmediatamente sin interrumpir el flujo de audio.

## Ver También

- [Descripción General de Efectos de Audio](index.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Bloques de Procesamiento de Audio (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
