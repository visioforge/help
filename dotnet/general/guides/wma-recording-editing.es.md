---
title: Grabar y Editar Archivos WMA en C# y .NET
description: Grabe audio WMA desde el micrófono y edite archivos WMA en .NET con las clases VideoCaptureCoreX y VideoEditCoreX para captura y edición de Windows Media Audio.
---

# Grabar y Editar Archivos WMA en C# y .NET: Una Guía Completa

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Grabación y Edición de WMA en .NET

Este artículo proporciona una guía completa para desarrolladores que trabajan con archivos Windows Media Audio (WMA) en aplicaciones .NET. Exploraremos cómo grabar audio WMA en .NET desde micrófonos y otros dispositivos de captura utilizando la clase `VideoCaptureCoreX`, y cómo editar archivos WMA utilizando la clase `VideoEditCoreX` de los SDKs .NET de VisioForge.

Windows Media Audio es un formato de audio popular desarrollado por Microsoft que ofrece una excelente compresión manteniendo una buena calidad de audio. El formato WMA es ampliamente utilizado en aplicaciones de Windows y soporta varias tasas de bits y frecuencias de muestreo, lo que lo hace adecuado tanto para grabaciones de voz como para música de alta calidad.

La biblioteca VisioForge proporciona clases potentes para capturar datos de audio desde dispositivos del sistema y procesar contenido de audio y video. Ya sea que necesite crear una aplicación de consola de grabación de voz simple o un editor de audio WinForms complejo, estos SDKs ofrecen la funcionalidad que necesita. Esta guía le mostrará cómo capturar audio WMA en .NET y grabar archivos WMA en C# con facilidad.

## Prerrequisitos e Instalación

Antes de comenzar a grabar o editar archivos WMA en su aplicación .NET, asegúrese de tener lo siguiente:

- Visual Studio 2019 o posterior
- .NET 6.0 o posterior (o .NET Framework 4.7.2+)
- VisioForge Video Capture SDK .NET o Video Edit SDK .NET

### Instalación de Paquetes NuGet

Instale los paquetes requeridos utilizando el Administrador de Paquetes NuGet:

```bash
# Para grabación WMA con VideoCaptureCoreX
Install-Package VisioForge.DotNet.VideoCapture

# Para edición WMA con VideoEditCoreX
Install-Package VisioForge.DotNet.VideoEdit
```

Para instrucciones detalladas de instalación, por favor consulte la [guía de instalación](../../install/index.es.md).

## Grabación de Archivos WMA desde Micrófono Usando VideoCaptureCoreX

La clase `VideoCaptureCoreX` proporciona un enfoque directo para capturar audio WMA en C# desde micrófonos y otros dispositivos de entrada de audio. Esta sección demuestra cómo grabar archivos de audio WMA en C# con la enumeración adecuada de dispositivos y configuración del codificador. Aprenda cómo capturar contenido WMA en C# para varios escenarios de aplicación.

### Componentes Principales para Grabación WMA

1. **VideoCaptureCoreX**: Clase principal del motor para gestionar la captura de audio y salida WMA en .NET.
2. **DeviceEnumerator**: Clase para descubrir dispositivos de captura de audio disponibles en el sistema.
3. **AudioCaptureDeviceSourceSettings**: Configuración para micrófono o dispositivo de entrada de audio.
4. **WMAOutput**: Configuración de formato de salida específicamente para la creación de archivos Windows Media Audio.
5. **WMAEncoderSettings**: Clase de configuración para parámetros del codificador WMA como tasa de bits y frecuencia de muestreo.

### Implementación Básica de Grabación WMA

Aquí hay una implementación completa en C# para capturar y grabar archivos WMA desde un micrófono:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public class WmaRecorder
{
    private VideoCaptureCoreX _videoCapture;

    // Llame a este método una vez durante el inicio de la aplicación o carga del formulario
    public async Task InitializeAsync()
    {
        // Inicializar el SDK de VisioForge
        await VisioForgeX.InitSDKAsync();
    }

    public async Task StartRecordingAsync(string outputPath)
    {
        // Crear instancia de VideoCaptureCoreX para captura de audio
        _videoCapture = new VideoCaptureCoreX();

        // Configurar dispositivo de captura de audio (micrófono)
        await ConfigureAudioSourceAsync();

        // Deshabilitar captura de video - solo necesitamos audio
        _videoCapture.Video_Source = null;
        _videoCapture.Video_Play = false;

        // Configurar ajustes de audio
        _videoCapture.Audio_Play = false;    // Deshabilitar monitoreo de audio durante grabación
        _videoCapture.Audio_Record = true;   // Habilitar grabación de audio a archivo

        // Configurar formato de salida WMA
        var wmaOutput = new WMAOutput(outputPath);
        
        // Configurar ajustes del codificador WMA
        wmaOutput.Audio.Bitrate = 192;       // Tasa de bits de 192 Kbps
        wmaOutput.Audio.SampleRate = 48000;  // Frecuencia de muestreo de 48 kHz
        wmaOutput.Audio.Channels = 2;        // Grabación estéreo

        // Agregar salida WMA a la tubería de captura
        _videoCapture.Outputs_Add(wmaOutput, autostart: true);

        // Iniciar el proceso de captura de audio
        await _videoCapture.StartAsync();

        Console.WriteLine("Grabación iniciada. Presione cualquier tecla para detener...");
    }

    private async Task ConfigureAudioSourceAsync()
    {
        // Obtener dispositivos de captura de audio disponibles usando API DirectSound
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);

        if (audioDevices.Length == 0)
        {
            throw new Exception("No se encontró dispositivo de captura de audio.");
        }

        // Obtener primer dispositivo de captura de audio disponible (usualmente el micrófono predeterminado)
        var audioDevice = audioDevices[0];

        // Obtener formato soportado del dispositivo
        var audioFormat = audioDevice.GetDefaultFormat();

        // Crear configuración de fuente de audio con el dispositivo y formato seleccionados
        var audioSourceSettings = audioDevice.CreateSourceSettingsVC(audioFormat);

        // Configurar dispositivo de captura de audio
        _videoCapture.Audio_Source = audioSourceSettings;
    }

    public async Task StopRecordingAsync()
    {
        if (_videoCapture != null)
        {
            // Detener toda captura y codificación
            await _videoCapture.StopAsync();

            // Limpiar recursos
            await _videoCapture.DisposeAsync();
            _videoCapture = null;

            Console.WriteLine("Grabación detenida y archivo guardado.");
        }
    }
}
```

### Ejemplo de Aplicación de Consola para Grabación WMA

Aquí hay una aplicación de consola completa que graba audio WMA desde un micrófono:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Grabadora de Audio WMA - Aplicación de Consola");
        Console.WriteLine("============================================");

        // Inicializar SDK
        await VisioForgeX.InitSDKAsync();

        // Crear instancia de captura
        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Enumerar y mostrar dispositivos de captura de audio disponibles
            var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
                AudioCaptureDeviceAPI.DirectSound);

            Console.WriteLine("\nDispositivos de captura de audio disponibles:");
            for (int i = 0; i < audioDevices.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioDevices[i].DisplayName}");
            }

            if (audioDevices.Length == 0)
            {
                Console.WriteLine("No se encontró dispositivo de captura de audio. Saliendo.");
                return;
            }

            // Seleccionar primer dispositivo para grabación
            var selectedDevice = audioDevices[0];
            var audioFormat = selectedDevice.GetDefaultFormat();
            var audioSourceSettings = selectedDevice.CreateSourceSettingsVC(audioFormat);

            // Configurar captura de video para grabación solo de audio
            videoCapture.Audio_Source = audioSourceSettings;
            videoCapture.Video_Source = null;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;

            // Configurar archivo de salida WMA
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wma");

            var wmaOutput = new WMAOutput(outputPath);
            wmaOutput.Audio.Bitrate = 192;
            wmaOutput.Audio.SampleRate = 48000;
            wmaOutput.Audio.Channels = 2;

            videoCapture.Outputs_Add(wmaOutput, autostart: true);

            // Iniciar grabación
            Console.WriteLine($"\nGrabando a: {outputPath}");
            Console.WriteLine("Presione ENTER para detener la grabación...\n");

            await videoCapture.StartAsync();

            // Esperar entrada del usuario para detener
            Console.ReadLine();

            // Detener grabación
            await videoCapture.StopAsync();
            Console.WriteLine($"\nGrabación guardada en: {outputPath}");
        }
        finally
        {
            // Limpieza
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Aplicación WinForms para Grabación WMA

Para una aplicación Windows Forms con controles visuales, aquí hay un ejemplo de implementación:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public partial class WmaRecorderForm : Form
{
    private VideoCaptureCoreX _videoCapture;
    private bool _isRecording = false;

    public WmaRecorderForm()
    {
        InitializeComponent();
    }

    private async void Form_Load(object sender, EventArgs e)
    {
        // Inicializar SDK
        await VisioForgeX.InitSDKAsync();

        // Poblar lista desplegable de dispositivos de audio
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);

        foreach (var device in audioDevices)
        {
            cmbAudioDevices.Items.Add(device.DisplayName);
        }

        if (cmbAudioDevices.Items.Count > 0)
        {
            cmbAudioDevices.SelectedIndex = 0;
        }

        // Establecer ruta de salida predeterminada
        txtOutputPath.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            "recording.wma");
    }

    private async void btnStartStop_Click(object sender, EventArgs e)
    {
        if (!_isRecording)
        {
            await StartRecordingAsync();
        }
        else
        {
            await StopRecordingAsync();
        }
    }

    private async Task StartRecordingAsync()
    {
        _videoCapture = new VideoCaptureCoreX();

        // Obtener dispositivo de audio seleccionado
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);
        var selectedDevice = audioDevices.FirstOrDefault(
            d => d.DisplayName == cmbAudioDevices.SelectedItem.ToString());

        if (selectedDevice == null)
        {
            MessageBox.Show("Por favor seleccione un dispositivo de audio.");
            return;
        }

        // Configurar fuente de audio
        var audioFormat = selectedDevice.GetDefaultFormat();
        var audioSourceSettings = selectedDevice.CreateSourceSettingsVC(audioFormat);
        _videoCapture.Audio_Source = audioSourceSettings;

        // Configurar para captura solo de audio
        _videoCapture.Video_Source = null;
        _videoCapture.Video_Play = false;
        _videoCapture.Audio_Play = false;
        _videoCapture.Audio_Record = true;

        // Configurar salida WMA
        var wmaOutput = new WMAOutput(txtOutputPath.Text);
        wmaOutput.Audio.Bitrate = (int)numBitrate.Value;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = rbStereo.Checked ? 2 : 1;

        _videoCapture.Outputs_Add(wmaOutput, autostart: true);

        // Iniciar grabación
        await _videoCapture.StartAsync();

        _isRecording = true;
        btnStartStop.Text = "Detener Grabación";
        lblStatus.Text = "Grabando...";
    }

    private async Task StopRecordingAsync()
    {
        if (_videoCapture != null)
        {
            await _videoCapture.StopAsync();
            await _videoCapture.DisposeAsync();
            _videoCapture = null;
        }

        _isRecording = false;
        btnStartStop.Text = "Iniciar Grabación";
        lblStatus.Text = "Grabación guardada.";
    }

    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isRecording)
        {
            StopRecordingAsync().Wait();
        }

        VisioForgeX.DestroySDK();
    }
}
```

### Configuraciones Avanzadas de Captura de Audio

La clase `VideoCaptureCoreX` soporta varias configuraciones de captura de audio para diferentes escenarios de grabación:

```csharp
// Configurar grabación WMA de alta calidad
var wmaOutput = new WMAOutput("high_quality.wma");
wmaOutput.Audio.Bitrate = 320;       // Tasa de bits de máxima calidad
wmaOutput.Audio.SampleRate = 48000;  // Frecuencia de muestreo profesional
wmaOutput.Audio.Channels = 2;        // Estéreo

// Configurar grabación de voz (tamaño de archivo más pequeño)
var voiceOutput = new WMAOutput("voice_memo.wma");
voiceOutput.Audio.Bitrate = 128;     // Bueno para voz
voiceOutput.Audio.SampleRate = 44100; // Frecuencia de muestreo estándar
voiceOutput.Audio.Channels = 1;       // Mono es suficiente para voz

// Verificar si el codificador WMA está disponible en el sistema
if (!WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("El codificador WMA no está disponible en este sistema.");
    // Considere recurrir a MP3 u otro formato
}
```

## Edición de Archivos WMA Usando VideoEditCoreX

La clase `VideoEditCoreX` proporciona capacidades potentes para editar archivos WMA y convertir contenido de audio y video al formato Windows Media. Esta sección demuestra cómo editar archivos WMA en .NET con recorte, fusión y conversión de formato.

### Componentes Principales para Edición WMA

1. **VideoEditCoreX**: Clase principal del motor para gestionar operaciones de edición de audio y video.
2. **AudioFileSource**: Clase para agregar fuentes de archivos de audio a la línea de tiempo de edición.
3. **WMAOutput**: Configuración de formato de salida para exportación de Windows Media Audio.
4. **Audio_Effects**: Colección para aplicar efectos de audio durante la edición.

### Edición Básica de Archivos WMA

Aquí se muestra cómo editar archivos WMA utilizando la clase `VideoEditCoreX`:

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

public class WmaEditor
{
    private VideoEditCoreX _videoEdit;

    // Llame a este método una vez durante el inicio de la aplicación o carga del formulario
    public async Task InitializeAsync()
    {
        // Inicializar el SDK de VisioForge
        await VisioForgeX.InitSDKAsync();
    }

    public async Task EditWmaFileAsync(
        string inputPath, 
        string outputPath,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null)
    {
        // Crear instancia de VideoEditCoreX
        _videoEdit = new VideoEditCoreX();

        // Configurar manejadores de eventos
        _videoEdit.OnProgress += VideoEdit_OnProgress;
        _videoEdit.OnStop += VideoEdit_OnStop;
        _videoEdit.OnError += VideoEdit_OnError;

        // Agregar archivo WMA de entrada con recorte opcional
        var audioFile = new AudioFileSource(
            inputPath,
            startTime,  // Tiempo de inicio para recorte (null para inicio)
            endTime);   // Tiempo de fin para recorte (null para fin)

        _videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configurar formato de salida WMA
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        _videoEdit.Output_Format = wmaOutput;

        // Iniciar el proceso de edición
        _videoEdit.Start();

        Console.WriteLine("Edición en progreso...");
    }

    private void VideoEdit_OnProgress(object sender, ProgressEventArgs e)
    {
        Console.WriteLine($"Progreso: {e.Progress}%");
    }

    private void VideoEdit_OnStop(object sender, StopEventArgs e)
    {
        if (e.Successful)
        {
            Console.WriteLine("¡Edición completada exitosamente!");
        }
        else
        {
            Console.WriteLine("Edición detenida con errores.");
        }

        // Limpieza
        _videoEdit?.Dispose();
        _videoEdit = null;
    }

    private void VideoEdit_OnError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}
```

### Fusión de Múltiples Archivos WMA

La clase `VideoEditCoreX` le permite fusionar múltiples archivos de audio en una sola salida WMA:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

public class WmaMerger
{
    // Llame a este método una vez durante el inicio de la aplicación o carga del formulario
    public async Task InitializeAsync()
    {
        // Inicializar SDK
        await VisioForgeX.InitSDKAsync();
    }

    public async Task MergeWmaFilesAsync(
        List<string> inputFiles, 
        string outputPath)
    {
        var videoEdit = new VideoEditCoreX();

        try
        {
            // Configurar reporte de progreso
            videoEdit.OnProgress += (s, e) => 
                Console.WriteLine($"Progreso de fusión: {e.Progress}%");

            videoEdit.OnError += (s, e) => 
                Console.WriteLine($"Error: {e.Message}");

            // Agregar cada archivo de entrada secuencialmente
            foreach (var inputFile in inputFiles)
            {
                var audioFile = new AudioFileSource(inputFile);
                
                // Agregar con insertTime null añade al final de la línea de tiempo
                videoEdit.Input_AddAudioFile(audioFile, insertTime: null);
                
                Console.WriteLine($"Agregado: {inputFile}");
            }

            // Configurar formato de salida
            var wmaOutput = new WMAOutput(outputPath);
            wmaOutput.Audio.Bitrate = 192;
            wmaOutput.Audio.SampleRate = 48000;
            wmaOutput.Audio.Channels = 2;

            videoEdit.Output_Format = wmaOutput;

            // Crear señal de finalización
            var completionSource = new TaskCompletionSource<bool>();
            videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

            // Iniciar fusión
            videoEdit.Start();

            // Esperar finalización
            bool success = await completionSource.Task;

            if (success)
            {
                Console.WriteLine($"Archivos fusionados exitosamente a: {outputPath}");
            }
            else
            {
                Console.WriteLine("Operación de fusión fallida.");
            }
        }
        finally
        {
            videoEdit.Dispose();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Recorte de Archivos WMA

Extraer una porción específica de un archivo WMA:

```csharp
// Nota: Llame a VisioForgeX.InitSDKAsync() una vez durante el inicio de la aplicación o carga del formulario
public async Task TrimWmaFileAsync(
    string inputPath,
    string outputPath,
    TimeSpan startTime,
    TimeSpan endTime)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Agregar archivo de entrada con tiempos de inicio y fin específicos
        var audioFile = new AudioFileSource(
            inputPath,
            startTime,   // ej., TimeSpan.FromSeconds(10)
            endTime);    // ej., TimeSpan.FromSeconds(60)

        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configurar salida WMA
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        // Crear señal de finalización
        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

        // Iniciar recorte
        videoEdit.Start();

        // Esperar finalización
        bool success = await completionSource.Task;

        Console.WriteLine(success 
            ? "¡Recorte completado exitosamente!" 
            : "Operación de recorte fallida.");
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

### Conversión de Archivos de Video a Audio WMA

Extraer audio de archivos de video y guardar como WMA:

```csharp
// Nota: Llame a VisioForgeX.InitSDKAsync() una vez durante el inicio de la aplicación o carga del formulario
public async Task ExtractAudioToWmaAsync(
    string videoInputPath,
    string wmaOutputPath)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Agregar archivo de video - el audio se extraerá automáticamente
        var audioFile = new AudioFileSource(videoInputPath);
        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configurar salida WMA para extracción de audio
        var wmaOutput = new WMAOutput(wmaOutputPath);
        wmaOutput.Audio.Bitrate = 256;       // Mayor calidad para música
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnProgress += (s, e) => 
            Console.WriteLine($"Progreso de extracción: {e.Progress}%");
        videoEdit.OnStop += (s, e) => 
            completionSource.SetResult(e.Successful);

        videoEdit.Start();

        bool success = await completionSource.Task;

        Console.WriteLine(success 
            ? $"Audio extraído a: {wmaOutputPath}" 
            : "Extracción de audio fallida.");
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

### Aplicación de Efectos de Audio Durante la Edición WMA

La clase `VideoEditCoreX` soporta varios efectos de audio que pueden aplicarse durante el proceso de edición:

```csharp
using VisioForge.Core.Types.X.AudioEffects;

// Nota: Llame a VisioForgeX.InitSDKAsync() una vez durante el inicio de la aplicación o carga del formulario
public async Task EditWmaWithEffectsAsync(
    string inputPath,
    string outputPath)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Agregar archivo de entrada
        var audioFile = new AudioFileSource(inputPath);
        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Aplicar efectos de audio

        // Efecto de amplificación de volumen
        var amplifyEffect = new AmplifyAudioEffect(1.5); // 150% volumen
        videoEdit.Audio_Effects.Add(amplifyEffect);

        // Efecto de ecualizador de 10 bandas
        var eqLevels = new double[] 
        { 
            3.0,   // 60 Hz
            2.0,   // 170 Hz
            1.0,   // 310 Hz
            0.0,   // 600 Hz
            0.0,   // 1 kHz
            0.0,   // 3 kHz
            1.0,   // 6 kHz
            2.0,   // 12 kHz
            2.0,   // 14 kHz
            3.0    // 16 kHz
        };
        var equalizerEffect = new Equalizer10AudioEffect(eqLevels);
        videoEdit.Audio_Effects.Add(equalizerEffect);

        // Configurar salida WMA
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

        videoEdit.Start();

        await completionSource.Task;
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

## Configuración del Codificador WMA

La clase `WMAEncoderSettings` proporciona varias opciones de configuración para el codificador Windows Media Audio:

### Configuraciones Disponibles

```csharp
var wmaSettings = new WMAEncoderSettings
{
    // Tasa de bits en Kbps - valores soportados: 128, 192, 256, 320
    Bitrate = 192,
    
    // Frecuencia de muestreo en Hz - valores soportados: 44100, 48000
    SampleRate = 48000,
    
    // Número de canales - valores soportados: 1 (mono), 2 (estéreo)
    Channels = 2
};

// Consultar configuraciones soportadas
int[] supportedBitrates = wmaSettings.GetSupportedBitrates();
// Retorna: [128, 192, 256, 320]

int[] supportedSampleRates = wmaSettings.GetSupportedSampleRates();
// Retorna: [44100, 48000]

int[] supportedChannels = wmaSettings.GetSupportedChannelCounts();
// Retorna: [1, 2]
```

### Preajustes de Calidad

Aquí hay preajustes recomendados para diferentes casos de uso:

```csharp
// Grabación de música de alta calidad
var musicPreset = new WMAEncoderSettings
{
    Bitrate = 320,
    SampleRate = 48000,
    Channels = 2
};

// Grabación de voz / podcasts
var voicePreset = new WMAEncoderSettings
{
    Bitrate = 128,
    SampleRate = 44100,
    Channels = 1
};

// Calidad y tamaño de archivo equilibrados
var balancedPreset = new WMAEncoderSettings
{
    Bitrate = 192,
    SampleRate = 48000,
    Channels = 2
};
```

## Trabajando con Paquetes y Búferes de Audio

Para escenarios avanzados, puede necesitar trabajar directamente con paquetes de datos de audio. El SDK proporciona mecanismos para acceder a datos de audio sin procesar durante la captura y procesamiento.

### Procesamiento de Paquetes de Audio

```csharp
// Durante la captura, puede monitorear niveles de audio y paquetes
_videoCapture.OnAudioVUMeter += (sender, args) =>
{
    // Obtener niveles de audio para visualización de medidor VU
    double leftChannel = args.Left;
    double rightChannel = args.Right;
    
    // Actualizar UI con niveles de audio
    UpdateVUMeter(leftChannel, rightChannel);
};
```

## Manejo de Errores y Mejores Prácticas

### Gestión Adecuada de Recursos

Siempre asegure la limpieza adecuada de los recursos del SDK:

```csharp
public class WmaProcessor : IDisposable
{
    private VideoCaptureCoreX _videoCapture;
    private bool _disposed = false;

    public async Task InitializeAsync()
    {
        await VisioForgeX.InitSDKAsync();
        _videoCapture = new VideoCaptureCoreX();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _videoCapture?.Dispose();
                VisioForgeX.DestroySDK();
            }
            _disposed = true;
        }
    }
}
```

### Manejo de Errores

Implemente un manejo de errores completo para aplicaciones de producción:

```csharp
try
{
    await _videoCapture.StartAsync();
}
catch (Exception ex)
{
    // Registrar el error
    Console.WriteLine($"Falló al iniciar la grabación: {ex.Message}");
    
    // Limpiar recursos
    await _videoCapture.DisposeAsync();
    
    // Notificar al usuario o reintentar
    throw;
}
```

### Verificación de Disponibilidad del Codificador

Antes de crear archivos WMA, verifique que el codificador esté disponible:

```csharp
if (!WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("El codificador WMA no está disponible.");
    Console.WriteLine("Recurriendo a formato MP3...");
    
    // Usar MP3 como alternativa
    var mp3Output = new MP3Output("output.mp3");
    // ... continuar con codificación MP3
}
```

## Consideraciones Multiplataforma

El formato WMA y los componentes de Windows Media están diseñados principalmente para sistemas Windows. Al desarrollar aplicaciones multiplataforma:

- **Windows**: Soporte completo de WMA con todas las opciones de codificación
- **Linux/macOS**: La codificación WMA puede requerir complementos adicionales de GStreamer
- **Móvil (Android/iOS)**: Considere usar formatos más universales como AAC o MP3

Para grabación de audio multiplataforma, considere estas alternativas:

```csharp
// Verificar plataforma y seleccionar formato apropiado
string outputFormat = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? "output.wma"
    : "output.m4a";  // Contenedor AAC para no-Windows

if (outputFormat.EndsWith(".wma"))
{
    var wmaOutput = new WMAOutput(outputFormat);
    _videoCapture.Outputs_Add(wmaOutput, true);
}
else
{
    var m4aOutput = new M4AOutput(outputFormat);
    _videoCapture.Outputs_Add(m4aOutput, true);
}
```

## Aplicaciones de Muestra y Recursos

Explore aplicaciones de muestra completas que demuestran la grabación y edición de WMA:

- [Muestras de Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)
- [Muestras de Video Edit SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X)

### Documentación Adicional

- [Guía del Codificador Windows Media Audio](../audio-encoders/wma.es.md)
- [Guía de Salida de Video Windows Media](../output-formats/wmv.es.md)
- [Capturador de Muestras de Audio](../audio-effects/audio-sample-grabber.es.md)
- [Documentación de API](https://api.visioforge.org/dotnet/api/index.html)

## Conclusión

Esta guía completa ha demostrado cómo grabar y editar archivos WMA utilizando los SDKs .NET de VisioForge. Ha aprendido cómo grabar audio WMA en .NET, capturar contenido WMA en .NET y crear aplicaciones de audio profesionales. La clase `VideoCaptureCoreX` proporciona capacidades potentes para capturar audio desde micrófonos y otros dispositivos, mientras que la clase `VideoEditCoreX` ofrece opciones flexibles para editar y convertir contenido de audio al formato Windows Media.

Puntos clave:

- **Grabar archivos WMA**: Use `VideoCaptureCoreX` con `WMAOutput` para capturar audio desde dispositivos del sistema y reserve configuraciones de calidad óptimas para su salida
- **Editar archivos WMA**: Use `VideoEditCoreX` para recortar, fusionar y aplicar efectos a grabaciones de audio
- **Configuración**: La clase `WMAEncoderSettings` permite el ajuste fino de la tasa de bits, frecuencia de muestreo y canales
- **Multiplataforma**: Considere los requisitos específicos de la plataforma al trabajar con formatos Windows Media
- **Soporte de aplicaciones Windows**: Cree aplicaciones WinForms, WPF y de consola con facilidad

Ambas clases se integran perfectamente con aplicaciones WinForms, WPF y de consola, facilitando la creación de soluciones potentes de grabación y edición de audio en sus aplicaciones .NET. Las capacidades de procesamiento de datos y características de la biblioteca le permiten construir aplicaciones de edición de audio de grado profesional.
