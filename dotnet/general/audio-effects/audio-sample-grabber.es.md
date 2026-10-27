---
title: Audio Sample Grabber para .NET - Capturar frames de audio
description: Capture y procese frames de audio en tiempo real usando Audio Sample Grabber con motores X-engines y Classic en aplicaciones SDK .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - C#
primary_api_classes:
  - AudioFrameBufferEventArgs
  - AudioSampleGrabberBlock

---

# Trabajar con Audio Sample Grabber en SDKs .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al Audio Sample Grabber

El Audio Sample Grabber es una característica potente disponible en todos nuestros SDKs .NET que permite a los desarrolladores acceder directamente a fotogramas de audio crudos tanto de fuentes en vivo como de archivos multimedia. Esta capacidad abre un amplio rango de posibilidades para procesamiento de audio, análisis y manipulación en sus aplicaciones.

Al trabajar con procesamiento de audio, obtener acceso a fotogramas de audio individuales es esencial para tareas como:

- Visualización de audio en tiempo real
- Procesamiento de efectos de audio personalizados
- Integración con reconocimiento de voz
- Análisis y métricas de audio
- Conversión de formato de audio personalizado
- Algoritmos de detección de sonido

El evento `OnAudioFrameBuffer` es el mecanismo central que proporciona acceso a estos fotogramas de audio crudos. Este evento se dispara cada vez que un nuevo fotograma de audio está disponible, dándole acceso directo a memoria no administrada que contiene los datos de audio decodificados.

## Cómo funciona el Audio Sample Grabber

El Audio Sample Grabber intercepta el pipeline de audio durante la reproducción o captura, proporcionándole los datos de audio crudos antes de que se rendericen al dispositivo de salida. Estos datos están típicamente en formato PCM (Modulación por Codificación de Pulsos), que es el formato estándar para audio digital sin comprimir, pero ocasionalmente puede estar en formato de punto flotante IEEE dependiendo de la fuente de audio.

Cada vez que se dispara el evento `OnAudioFrameBuffer`, proporciona un objeto `AudioFrameBufferEventArgs` que contiene información crítica sobre el fotograma de audio:

- `Frame.Data`: Un `IntPtr` apuntando al bloque de memoria no administrada que contiene los datos de audio crudos
- `Frame.DataSize`: El tamaño de los datos de audio en bytes
- `Frame.Info`: Una estructura que contiene información detallada sobre el formato de audio, incluyendo:
  - Conteo de canales (mono, estéreo, etc.)
  - Tasa de muestreo (típicamente 44.1kHz, 48kHz, etc.)
  - Bits por muestra (16-bit, 24-bit, etc.)
  - Tipo de formato de audio (PCM, IEEE, etc.)
  - Información de marca de tiempo
  - Alineación de bloque y otros detalles específicos del formato

## Configurar el Audio Sample Grabber

El proceso de configuración varía ligeramente dependiendo de si está usando nuestros nuevos motores X o los motores Clásicos. Exploremos ambos enfoques:

=== "Motores X"

    
    Para los motores X, configurar el Audio Sample Grabber es sencillo. Simplemente necesita crear un manejador de eventos para el evento `OnAudioFrameBuffer`:
    
    ```csharp
    VideoCapture1.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    
    La arquitectura de los motores X habilita automáticamente la captura de muestras de audio cuando se suscribe a este evento, sin requerir configuración adicional.
    

=== "Motores Clásicos"

    
    Al usar motores Clásicos, necesita habilitar explícitamente la funcionalidad de Audio Sample Grabber antes de crear el manejador de eventos:
    
    ```csharp
    VideoCapture1.Audio_Sample_Grabber_Enabled = true;
    ```
    
    Luego, como con los motores X, cree su manejador de eventos:
    
    ```csharp
    VideoCapture1.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    
    **Nota**: La propiedad `Audio_Sample_Grabber_Enabled` no es requerida para el componente VideoEditCore, que tiene la captura de muestras de audio habilitada por defecto.
    

=== "Media Blocks SDK"

    
    El Media Blocks SDK también soporta captura de muestras de audio. Use el componente `AudioSampleGrabberBlock` para capturar fotogramas de audio.
    
    ```csharp
    private AudioSampleGrabberBlock _audioSampleGrabberSink;
    ```
    
    Luego, como con los motores X, cree su manejador de eventos y especifique el formato de audio:
    
    ```csharp
    _audioSampleGrabberBlock = new AudioSampleGrabberBlock(VisioForge.Core.Types.X.AudioFormatX.S16LE);
    _audioSampleGrabberBlock.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    


## Procesar fotogramas de audio

Una vez que haya configurado el manejador de eventos, puede procesar los fotogramas de audio a medida que llegan. Aquí hay un ejemplo básico de cómo manejar el evento `OnAudioFrameBuffer`:

```csharp
using VisioForge.Types;
using System.Diagnostics;

private void OnAudioFrameBuffer(object sender, AudioFrameBufferEventArgs e)
{
    // Registrar información del fotograma de audio
    Debug.WriteLine($"Fotograma de audio: {e.Frame.DataSize} bytes; Formato: {e.Frame.Info}");
    
    // Acceso a datos de audio crudos a través del puntero no administrado
    IntPtr rawAudioData = e.Frame.Data;
    
    // Obtener detalles del formato de audio. RAWBaseAudioInfo contiene cuatro campos:
    // Channels, SampleRate, BPS (bits por muestra), Format (enum AudioFormat).
    int channelCount  = e.Frame.Info.Channels;
    int sampleRate    = e.Frame.Info.SampleRate;
    int bitsPerSample = e.Frame.Info.BPS;
    AudioFormat format = e.Frame.Info.Format;
    
    // Su código de procesamiento de audio personalizado aquí
    // ...
}
```

## Trabajar con datos de audio

### Convertir memoria no administrada a arrays administrados

Mientras `e.Frame.Data` proporciona un puntero a memoria no administrada, a menudo necesita trabajar con los datos en una forma más conveniente. La clase `AudioFrame` proporciona un método útil `GetDataArray()` que retorna una copia de los datos de audio como un array de bytes:

```csharp
private void VideoCapture1_OnAudioFrameBuffer(object sender, AudioFrameBufferEventArgs e)
{
    // Obtener una copia administrada de los datos de audio
    byte[] audioData = e.Frame.GetDataArray();
    
    // Ahora puede trabajar con los datos usando operaciones estándar de arrays C#
    // ...
}
```

### Convertir datos PCM a muestras

Para muchas tareas de procesamiento de audio, querrá convertir los bytes PCM crudos en valores reales de muestras de audio. Aquí hay un método auxiliar para convertir un array de bytes PCM a un array de muestras de audio (asumiendo muestras de 16 bits):

```csharp
private short[] ConvertBytesToSamples(byte[] audioData)
{
    short[] samples = new short[audioData.Length / 2];
    
    for (int i = 0; i < samples.Length; i++)
    {
        // Combinar dos bytes en una muestra de 16 bits
        samples[i] = (short)(audioData[i * 2] | (audioData[i * 2 + 1] << 8));
    }
    
    return samples;
}
```

### Manejar audio multicanal

Al trabajar con audio estéreo o multicanal, las muestras están típicamente entrelazadas. Para un flujo estéreo, los datos están arreglados como: [Izq0, Der0, Izq1, Der1, ...]. Puede que quiera separar estos canales para procesamiento:

```csharp
private void ProcessStereoAudio(short[] samples, int channelCount)
{
    if (channelCount != 2) return;
    
    // Crear arrays para cada canal
    int samplesPerChannel = samples.Length / 2;
    short[] leftChannel = new short[samplesPerChannel];
    short[] rightChannel = new short[samplesPerChannel];
    
    // Separar los canales
    for (int i = 0; i < samplesPerChannel; i++)
    {
        leftChannel[i] = samples[i * 2];
        rightChannel[i] = samples[i * 2 + 1];
    }
    
    // Procesar cada canal por separado
    // ...
}
```

## Escenarios comunes de procesamiento de audio

### Medición de Nivel de Audio

Un caso de uso común para el Audio Sample Grabber es implementar medición de nivel de audio:

```csharp
private void CalculateAudioLevel(short[] samples)
{
    double sum = 0;
    
    // Calcular valor RMS (Raíz Media Cuadrática)
    foreach (short sample in samples)
    {
        sum += sample * sample;
    }
    
    double rms = Math.Sqrt(sum / samples.Length);
    
    // Convertir a decibelios
    double db = 20 * Math.Log10(rms / 32768);
    
    // Actualizar UI con el nivel (necesitará invocar si está en un hilo diferente)
    Debug.WriteLine($"Nivel de audio: {db} dB");
}
```

### FFT en tiempo real para análisis de espectro

Para análisis de espectro de frecuencia, podría querer realizar una FFT (Transformada Rápida de Fourier) en los datos de audio:

```csharp
// Nota: Necesitará una biblioteca para cálculo de FFT
// Este es un ejemplo simplificado
private void PerformFFTAnalysis(short[] samples)
{
    // Típicamente usarías una biblioteca como Math.NET Numerics
    // Convertir muestras a números complejos
    Complex[] complex = samples.Select(s => new Complex(s, 0)).ToArray();
    
    // Realizar FFT (pseudocódigo)
    // Complex[] fftResult = FFT.Forward(complex);
    
    // Procesar resultados de FFT
    // ...
}
```

## Consideraciones de rendimiento

Al trabajar con el Audio Sample Grabber, tenga en cuenta estas consideraciones de rendimiento:

1. **Minimizar Tiempo de Procesamiento**: El evento `OnAudioFrameBuffer` se llama en el hilo de procesamiento de audio. Operaciones de larga duración pueden causar fallos de audio.

2. **Considerar Seguridad de Hilos**: Si necesita actualizar elementos de UI o interactuar con otros componentes, use métodos apropiados de sincronización de hilos.

3. **Evitar Asignaciones de Memoria**: Asignaciones de memoria frecuentes en el manejador de eventos pueden llevar a pausas de recolección de basura. Reutilice arrays donde sea posible.

4. **Copia de Búfer**: El método `GetDataArray()` crea una copia de los datos de audio. Para escenarios de muy alto rendimiento, considere trabajar directamente con el puntero no administrado.

## Conclusión

El Audio Sample Grabber proporciona una forma potente de acceder y procesar datos de audio crudos en tiempo real tanto de fuentes en vivo como de archivos multimedia. Al aprovechar esta funcionalidad, puede implementar características sofisticadas de procesamiento de audio en sus aplicaciones, desde medición de nivel simple hasta análisis de audio complejo y procesamiento de efectos.

Ya sea que esté construyendo una aplicación de audio profesional, implementando visualización de audio o integrando con servicios de reconocimiento de voz, el Audio Sample Grabber le da los datos crudos que necesita para dar vida a sus ideas de procesamiento de audio.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código.