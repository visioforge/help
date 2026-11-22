---
title: Efectos de Audio en C# y .NET - EQ, Reverb, Filtros y Más
description: Aplique efectos de audio en tiempo real en C# y .NET con VisioForge SDKs. Ecualizador, reverb, eco, reducción de ruido, cambio de tono y más de 30 efectos.
sidebar_label: Efectos de Audio
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - Media Player SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Streaming
  - Editing
primary_api_classes:
  - VolumeAudioEffect
  - Equalizer10AudioEffect
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - CompressorExpanderAudioEffect

video:
  id: VsFSrYD80XE
  upload_date: 2026-11-20T15:10:00Z
  duration: PT11M57S
---

# Efectos de Audio en Tiempo Real para Aplicaciones .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge Media Framework proporciona más de 30 efectos de audio para el procesamiento de audio en tiempo real en aplicaciones C# y .NET. Construidos sobre GStreamer, los efectos multiplataforma incluyen ecualizadores, reverb, eco, compresión dinámica, filtros, cambio de tono, reducción de ruido basada en IA y más — todo funcionando en Windows, macOS, Linux, iOS y Android.

## Tutorial en video

El video crea el mismo reproductor en Visual Studio: una ventana WPF con un ecualizador de diez bandas, eco, reverberación y tono, cada uno cambiado mientras el archivo se reproduce.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/VsFSrYD80XE?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

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

- **PitchAudioEffect** — Cambio de tono en semitonos sin cambio de tempo
- **ScaleTempoAudioEffect** — Conserva el tono cuando cambia la tasa de reproducción

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
- **DS Reverb (I3DL2)** — Reverberación profesional con modelado ambiental (puede faltar en Windows 11; la alternativa es DS Waves Reverb)
- **DS Waves Reverb** — Reverberación simplificada

## Elementos GStreamer

Todos los efectos de audio multiplataforma se ejecutan en un pipeline de GStreamer. `ChannelOrder`, `DownMix`, `DynamicAmplify`, `Fade`, `Flanger`, `PhaseInvert`, `Phaser`, `Sound3D`, `TrebleEnhancer` y `TrueBass` procesan las muestras en código administrado. Los demás envuelven uno o más elementos de GStreamer:

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

## Transcripción del video

### Qué vamos a construir

Vamos a añadir efectos de audio a un reproductor C# WPF con el VisioForge Media Player SDK, y cambiarlos mientras el archivo se reproduce. Esto es lo que vamos a construir. Un reproductor WPF para archivos de video y audio, con cuatro efectos: un ecualizador de diez bandas, eco, reverberación y tono. Una casilla activa o desactiva cada efecto, y sus controles lo cambian, todo mientras el archivo sigue reproduciéndose. El código detrás ocupa unas doscientas cuarenta líneas de C#. Este es el reproductor terminado. El panel de efectos está debajo del video.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto tiene como destino .NET 8 o posterior. Y un archivo de música o de video para reproducir. La parte de reproductor de la aplicación es la misma que en nuestro tutorial del reproductor de video, así que este video la repasa rápido.

### Crear un proyecto WPF

En Visual Studio, cree un proyecto nuevo. Busque WPF y elija Aplicación WPF para C#. Ponga al proyecto el nombre AudioEffectsWpf. Elija .NET 10 y haga clic en Crear.

### Añadir los paquetes NuGet

Ahora añada cuatro paquetes NuGet. El Media Player SDK en sí. CrossPlatform Core, el entorno de ejecución de GStreamer sobre el que funciona el motor X. CrossPlatform Core Adds, que incluye más complementos, entre ellos la reverberación. Y CrossPlatform Libav, los códecs de FFmpeg con los que se decodifican muchos formatos. O use el Administrador de paquetes NuGet: haga clic derecho en el proyecto, Administrar paquetes NuGet e instale VisioForge.DotNet.MediaPlayer desde la pestaña Examinar. Después instale los paquetes Core, Core Adds y Libav para Windows x64. La pestaña Instalados muestra los cuatro. Son paquetes x64, así que ponga PlatformTarget en x64 en el archivo del proyecto.

### La ventana del reproductor

La ventana se escribe en XAML, y el diseñador la dibuja enseguida. Primero la parte del reproductor. Asigne al espacio de nombres VisioForge.Core.UI.WPF un prefijo, aquí vf. La cuadrícula tiene dos columnas, el video y una columna de ajustes de 260 píxeles, y cuatro filas: el video, la línea de tiempo, los botones y los efectos. El VideoView ocupa la primera celda. Debajo del video va la línea de tiempo: un control deslizante para la posición, con el tiempo al lado. Luego Reproducir, Pausar, Reanudar y Detener. Todos empiezan deshabilitados: Reproducir hasta que el SDK se ha cargado, y los otros tres hasta que se reproduce un archivo. La columna de ajustes contiene el nombre del archivo con un botón de examinar, y un cuadro combinado para la salida de audio. Después un control deslizante de volumen, y un cuadro de texto de solo lectura para el registro de errores.

### El panel de efectos

Ahora el panel de efectos, en la última fila, debajo de los botones. Un panel de pila horizontal ocupa las dos columnas. Cada efecto tiene un cuadro de grupo con una casilla en su encabezado. El ecualizador va primero, y su casilla asigna un mismo controlador a Checked y a Unchecked. Una cuadrícula uniforme de dos filas y diez columnas contiene las bandas: diez controles deslizantes, y sus frecuencias debajo, de 30 hercios a 15 kilohercios. La etiqueta Tag de cada control es el índice de su banda, así los diez comparten un mismo controlador. Un estilo configura los diez controles: verticales, de menos 12 a más 12 decibelios, y enlazados al mismo controlador ValueChanged. Un segundo estilo centra las etiquetas. El eco tiene un control para el retardo, de 50 a 1000 milisegundos, y otro para la intensidad, de 0 a 1. La reverberación tiene el tamaño de la sala, y un nivel que mezcla el sonido reverberado con el seco. Los dos van de 0 a 1. Y el tono tiene un control, de 12 semitonos abajo a 12 arriba, que se ajusta a semitonos enteros.

### Inicializar el SDK

MainWindow.xaml.cs empieza con estos usings. VisioForge.Core.Types.X.AudioEffects contiene las clases de los efectos. Cada efecto es un objeto, creado una vez y ajustado a los valores iniciales de los controles. Equalizer10AudioEffect recibe los diez niveles de banda en decibelios, todos en cero aquí. EchoAudioEffect recibe el retardo, y después su intensidad y su realimentación. ReverberationAudioEffect recibe una sala grande, y PitchAudioEffect está cuatro semitonos arriba. Cuando se carga la ventana, espere una vez a VisioForgeX.InitSDKAsync. Después cree el motor MediaPlayerCoreX y pásele el VideoView. Los errores llegan al evento OnError en otro hilo, así que el controlador vuelve al hilo de la interfaz y los añade al registro. Audio_OutputDevicesAsync enumera los altavoces y los auriculares, y sus nombres llenan el cuadro combinado. Reproducir se habilita cuando el SDK se ha cargado. El botón de examinar muestra un OpenFileDialog para archivos de video y de audio.

### Reproducir con los efectos

El controlador de Reproducir deshabilita los botones y limpia el registro. Envía el sonido a la salida seleccionada, con el volumen del control. OpenAsync abre el archivo. Después Audio_Effects_AddOrUpdate añade cada efecto cuya casilla está marcada, y PlayAsync inicia la reproducción con ellos. Los efectos duran una reproducción: tras Detener o al final del archivo desaparecen, así que cada Reproducir los añade otra vez. PlaybackState.Free significa que el archivo no se abrió o no se reprodujo, y la ventana vuelve al estado detenido. Si no, los botones pasan a reproduciendo, y el temporizador de la línea de tiempo arranca.

### Cambiar los efectos mientras suena

ApplyEffect sirve a las cuatro casillas. Mientras se reproduce un archivo, marcar una casilla añade su efecto con Audio_Effects_AddOrUpdate, y desmarcarla llama a Audio_Effects_Remove. Cuando no se reproduce nada, no hace nada: Reproducir añade los efectos marcados. La casilla del ecualizador llama a ApplyEffect. Los controles de banda comparten un controlador: lee la banda de la etiqueta Tag del control, y SetLevel fija el nivel de esa banda en decibelios. El ecualizador que está sonando lo toma al instante. Los controles del eco fijan Delay e Intensity en el objeto del efecto. Una propiedad asignada a un efecto que suena se aplica al instante, así que no hace falta añadirlo otra vez. La reverberación funciona igual, con RoomSize y Level. Y el control del tono fija PitchSemitones. El tempo no cambia: solo se mueve el tono.

### El resto del reproductor

El resto es el reproductor del tutorial del reproductor de video. Pausar y Reanudar son una llamada cada uno, y Detener espera a StopAsync y restablece la ventana. Los efectos siguen sonando después de una pausa. ShowBusy deshabilita los cuatro botones, y ShowPlaying habilita o bien Reproducir o bien los otros tres. Stopped devuelve los botones, la etiqueta de tiempo y la línea de tiempo a su sitio. OnStop se produce tras StopAsync y al final del archivo. El controlador actúa solo para el final del archivo, mientras Detener está habilitado y State es Free. Un DispatcherTimer marca dos veces por segundo y mueve la línea de tiempo a la posición, con una marca activada, para que ese movimiento no haga un salto. Un clic en la línea de tiempo salta al instante, y un arrastre salta una vez, al soltar el pulgar. El volumen sigue a su control. Cuando se cierra la ventana, Dispose detiene la reproducción y libera el reproductor, y DestroySDK libera el SDK.

### Ejecutarla

Ejecute la aplicación. Elija un archivo, haga clic en Reproducir y haga clic en la línea de tiempo para avanzar. Primero, escúchelo sin efectos. Marque Ecualizador, y baje las bandas graves y suba las agudas. Desactive el ecualizador y active Eco. Después alargue el retardo. Eco fuera, Reverberación dentro: una sala grande. Después suba el nivel para oír más sala. Reverberación fuera, Tono dentro: cuatro semitonos arriba. Después bájelo. Desmarque Tono, y el sonido vuelve a como era. Después Detener, y cierre la ventana.

### Errores frecuentes

Algunos errores habituales. Dejar fuera el paquete Core Adds, así que la reverberación no se crea. Añadir los efectos una sola vez: Detener o el final del archivo los quita, así que añádalos de nuevo en cada Reproducir. Escribir un elemento del arreglo Levels del ecualizador durante la reproducción, lo que no llega al efecto que suena: use SetLevel. PitchShiftAudioEffect está obsoleto, así que use PitchAudioEffect. Y mezclar paquetes x86 y x64.

### Próximos pasos

La guía escrita enumera más efectos, como un compresor, un flanger y la supresión de ruido. Los mismos efectos funcionan al capturar video, y Media Blocks SDK tiene un bloque por efecto. Y el capturador de muestras de audio le entrega las muestras en su propio código. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver También

- [Referencia de Efectos de Audio](reference.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Codificadores de Audio](../audio-encoders/index.md)
- [Bloques de Procesamiento de Audio (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
