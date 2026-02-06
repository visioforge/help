---
title: Guardar Stream RTSP a Archivo sin Recodificación
description: Guardar stream RTSP a archivo (MP4) desde cámara IP sin re-codificación de video usando .NET con VisioForge Media Blocks para control programático.
---

# Cómo Guardar Stream RTSP a Archivo: Grabar Video de Cámara IP sin Re-codificación

[SDK de Media Blocks .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!!info Muestra de Demo
Para un ejemplo completo de grabación de streams RTSP sin re-codificación, vea la [Demo RTSP MultiView](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo).

Para documentación específica de cámara ONVIF, vea la [Guía de Integración de Cámara IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Tabla de Contenidos

- [Cómo Guardar Stream RTSP a Archivo: Grabar Video de Cámara IP sin Re-codificación](#como-guardar-stream-rtsp-a-archivo-grabar-video-de-camara-ip-sin-re-codificacion)
  - [Tabla de Contenidos](#tabla-de-contenidos)
  - [Resumen](#resumen)
  - [Características Principales](#caracteristicas-principales)
  - [Concepto Principal](#concepto-principal)
  - [Prerrequisitos](#prerrequisitos)
  - [Código de Muestra: Clase RTSPRecorder](#codigo-de-muestra-clase-rtsprecorder)
  - [Explicación del Código](#explicacion-del-codigo)
  - [Cómo Usar el `RTSPRecorder`](#como-usar-el-rtsprecorder)
  - [Consideraciones Clave](#consideraciones-clave)
  - [Muestra Completa de GitHub](#muestra-completa-de-github)
  - [Mejores Prácticas](#mejores-practicas)
  - [Solución de Problemas](#solucion-de-problemas)

## Resumen

Esta guía demuestra cómo guardar un stream RTSP a un archivo MP4 capturando el stream de video original desde una cámara RTSP IP sin re-codificar el video. Este enfoque es altamente beneficioso para preservar la calidad de video original de las cámaras y minimizar el uso de CPU cuando necesita grabar metraje. El stream de audio puede pasarse a través o, opcionalmente, re-codificarse para mejor compatibilidad, permitiendo guardar los datos completos del streaming. Herramientas como FFmpeg y VLC ofrecen métodos de línea de comandos o UI para grabar un stream RTSP; sin embargo, esta guía se enfoca en un enfoque programático usando el SDK de VisioForge Media Blocks para desarrolladores .NET que necesitan crear aplicaciones que se conecten y graben video desde cámaras RTSP.

## Características Principales

- **Grabación Directa de Stream**: Guardar feeds de cámara RTSP sin pérdida de calidad
- **Procesamiento Eficiente en CPU**: No se requiere re-codificación de video
- **Manejo Flexible de Audio**: Pasar a través o re-codificar audio según sea necesario
- **Integración Profesional**: Control programático para aplicaciones empresariales
- **Alto Rendimiento**: Optimizado para grabación continua

Usaremos el SDK de VisioForge Media Blocks, una poderosa biblioteca .NET para construir aplicaciones personalizadas de procesamiento de medios, para guardar RTSP a archivo de manera efectiva.

## Concepto Principal

La idea principal es tomar el stream de video raw desde la fuente RTSP y enviarlo directamente a un sink de archivo (ej. muxer MP4) sin ningún paso de decodificación o codificación para el video. Este es un requisito común para grabar streams RTSP con máxima fidelidad.

- **Stream de Video**: Pasado a través directamente desde la fuente RTSP al sink MP4. Esto asegura que los datos de video originales se guarden, crucial para aplicaciones que necesitan grabar metraje de alta calidad desde cámaras.
- **Stream de Audio**: Puede pasarse a través directamente (si el códec de audio original es compatible con el contenedor MP4) o re-codificarse (ej. a AAC) para asegurar compatibilidad y potencialmente reducir el tamaño del archivo cuando guarde el stream RTSP.

## Prerrequisitos

Necesitará el SDK de VisioForge Media Blocks. Puede agregarlo a su proyecto .NET vía NuGet:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Dependiendo de su plataforma objetivo (Windows, macOS, Linux, incluyendo sistemas embebidos como Jetson Nano para aplicaciones de cámara embebida), también necesitará los paquetes de runtime nativos correspondientes. Por ejemplo, en Windows para grabar video:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Para información detallada sobre requisitos de deployment, y dependencias específicas de plataforma, por favor refiérase a nuestra [Guía de Deployment](../../deployment-x/index.md). Es importante verificar estos detalles para asegurar que su aplicación de captura de video stream funcione correctamente.

Refiérase al archivo `RTSP Capture Original.csproj` en el proyecto de muestra para una lista completa de dependencias para diferentes plataformas.

## Código de Muestra: Clase RTSPRecorder

El siguiente código C# define una clase `RTSPRecorder` que encapsula la funcionalidad de grabación RTSP.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

namespace RTSPCaptureOriginalStream
{
    /// <summary>
    /// Clase RTSPRecorder encapsula la funcionalidad de grabación RTSP para guardar stream RTSP a archivo.
    /// Usa el SDK MediaBlocks para crear un pipeline que conecta una fuente RTSP (como una cámara IP) a un sink MP4 (archivo).
    /// </summary>
    public class RTSPRecorder : IAsyncDisposable
    {
        /// <summary>
        /// El pipeline MediaBlocks que maneja el flujo de datos de medios.
        /// </summary>
        public MediaBlocksPipeline Pipeline { get; private set; }

        // Campos privados para los componentes MediaBlock
        private MediaBlock _muxer;               // Muxer contenedor MP4 (sink)
        private RTSPRAWSourceBlock _rtspRawSource; // Fuente stream RTSP (proporciona streams raw)
        private DecodeBinBlock _decodeBin;       // Opcional: Decodificador de audio (si re-codificando audio)
        private AACEncoderBlock _audioEncoder;   // Opcional: Codificador AAC (si re-codificando audio)
        private bool disposedValue;              // Bandera para prevenir disposiciones múltiples

        /// <summary>
        /// Evento disparado cuando ocurre un error en el pipeline.
        /// </summary>
        public event EventHandler<ErrorsEventArgs> OnError;

        /// <summary>
        /// Evento disparado cuando hay un mensaje de estado disponible.
        /// </summary>
        public event EventHandler<string> OnStatusMessage;

        /// <summary>
        /// Nombre de archivo de salida para la grabación MP4.
        /// </summary>
        public string Filename { get; set; } = "output.mp4";

        /// <summary>
        /// Si re-codificar audio a formato AAC (recomendado para compatibilidad).
        /// Si falso, audio se pasa a través.
        /// </summary>
        public bool ReencodeAudio { get; set; } = true;

        /// <summary>
        /// Inicia la sesión de grabación creando y configurando el pipeline MediaBlocks.
        /// </summary>
        /// <param name="rtspSettings">Configuración de fuente RTSP.</param>
        /// <returns>True si el pipeline inició exitosamente, falso de lo contrario.</returns>
        public async Task<bool> StartAsync(RTSPRAWSourceSettings rtspSettings)
        {
            // Crear un nuevo pipeline MediaBlocks
            Pipeline = new MediaBlocksPipeline();
            Pipeline.OnError += (sender, e) => OnError?.Invoke(this, e); // Burbujear errores

            OnStatusMessage?.Invoke(this, "Creando pipeline para grabar stream RTSP...");

            // 1. Crear el bloque fuente RTSP.
            // RTSPRAWSourceBlock proporciona streams elementales raw, sin decodificar (video y audio) desde su cámara IP u otras cámaras RTSP.
            _rtspRawSource = new RTSPRAWSourceBlock(rtspSettings);
            
            // 2. Crear el bloque sink MP4 (muxer).
            // Este bloque escribirá los streams de medios en un archivo MP4.
            _muxer = new MP4SinkBlock(new MP4SinkSettings(Filename));

            // 3. Conectar Stream de Video (Passthrough)
            // Crear un pad de entrada dinámico en el muxer para el stream de video.
            // Conectamos la salida de video raw desde la fuente RTSP directamente a la entrada MP4 sink.
            // Esto asegura que el video no se re-codifique cuando grabe el feed de la cámara.
            var inputVideoPad = (_muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video);
            Pipeline.Connect(_rtspRawSource.VideoOutput, inputVideoPad);
            OnStatusMessage?.Invoke(this, "Stream de video conectado (passthrough para calidad de video original).");

            // 4. Conectar Stream de Audio (Condicional Re-codificación)
            // Esta sección maneja cómo se procesa el audio desde el stream RTSP cuando se guarda al archivo.
            if (rtspSettings.AudioEnabled)
            {
                // Crear un pad de entrada dinámico en el muxer para el stream de audio.
                var inputAudioPad = (_muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio);

                if (ReencodeAudio)
                {
                    // Si la re-codificación de audio está habilitada (ej. a AAC para compatibilidad):
                    OnStatusMessage?.Invoke(this, "Configurando re-codificación de audio a AAC para archivo MP4...");
                    
                    // Crear un bloque decodificador que solo maneja audio.
                    // Necesitamos decodificar el audio original desde la cámara IP antes de re-codificarlo para guardar el stream MP4 con audio compatible.
                    _decodeBin = new DecodeBinBlock(videoDisabled: false, audioDisabled: true, subtitlesDisabled: false) 
                    {
                         // Podemos deshabilitar el convertidor de audio interno si estamos seguros del formato 
                         // o si el codificador maneja conversión. Para AAC, generalmente está bien.
                         DisableAudioConverter = true 
                    };

                    // Crear un codificador AAC con ajustes predeterminados.
                    _audioEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());

                    // Conectar el pipeline de procesamiento de audio:
                    // Salida audio RTSP -> Decodificador -> Codificador AAC -> Entrada audio muxer
                    Pipeline.Connect(_rtspRawSource.AudioOutput, _decodeBin.Input);
                    Pipeline.Connect(_decodeBin.AudioOutput, _audioEncoder.Input);
                    Pipeline.Connect(_audioEncoder.Output, inputAudioPad);
                    OnStatusMessage?.Invoke(this, "Stream de audio conectado (re-codificación a AAC para archivo MP4).");
                }
                else
                {
                    // Si la re-codificación de audio está deshabilitada, conectar audio RTSP directamente al muxer.
                    // Nota: Esto puede causar problemas si el formato de audio original no es 
                    // compatible con el contenedor MP4 (ej. G.711 PCMU/PCMA) al guardar el stream RTSP.
                    // Formatos compatibles comunes incluyen AAC. Verifique el formato de audio de su cámara.
                    Pipeline.Connect(_rtspRawSource.AudioOutput, inputAudioPad);
                    OnStatusMessage?.Invoke(this, "Stream de audio conectado (passthrough). Advertencia: Compatibilidad depende del formato de audio original de la cámara para el archivo.");
                }
            }

            // 5. Iniciar el pipeline para grabar video
            OnStatusMessage?.Invoke(this, "Iniciando pipeline de grabación para guardar stream RTSP a archivo...");
            bool success = await Pipeline.StartAsync();
            if (success)
            {
                OnStatusMessage?.Invoke(this, "Pipeline de grabación iniciado exitosamente.");
            }
            else
            {
                OnStatusMessage?.Invoke(this, "Falló al iniciar pipeline de grabación.");
            }
            return success;
        }

        /// <summary>
        /// Detiene la grabación deteniendo el pipeline MediaBlocks.
        /// </summary>
        /// <returns>True si el pipeline se detuvo exitosamente, falso de lo contrario.</returns>
        public async Task<bool> StopAsync()
        {
            if (Pipeline == null)
                return false;

            OnStatusMessage?.Invoke(this, "Deteniendo pipeline de grabación...");
            bool success = await Pipeline.StopAsync();
            if (success)
            {
                OnStatusMessage?.Invoke(this, "Pipeline de grabación detenido exitosamente.");
            }
            else
            {
                OnStatusMessage?.Invoke(this, "Falló al detener pipeline de grabación.");
            }
            
            // Desconectar el manejador de error para prevenir problemas si StopAsync se llama múltiples veces
            // o antes de DisposeAsync
            if (Pipeline != null)
            {
                 Pipeline.OnError -= OnError;
            }

            return success;
        }

        /// <summary>
        /// Desecha asincrónicamente el RTSPRecorder y todos sus recursos.
        /// Implementa el patrón IAsyncDisposable para limpieza apropiada de recursos.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                if (Pipeline != null)
                {
                    Pipeline.OnError -= (sender, e) => OnError?.Invoke(this, e); // Asegurar desconexión
                    await Pipeline.DisposeAsync();
                    Pipeline = null;
                }

                // Desechar todos los componentes MediaBlock
                // Usando 'as IDisposable' para casting seguro y disposición.
                (_muxer as IDisposable)?.Dispose();
                _muxer = null;

                _rtspRawSource?.Dispose();
                _rtspRawSource = null;

                _decodeBin?.Dispose();
                _decodeBin = null;

                _audioEncoder?.Dispose();
                _audioEncoder = null;

                disposedValue = true;
            }
        }
    }
}
```

## Explicación del Código

1. **Clase `RTSPRecorder`**: Esta clase es central para ayudar al usuario a guardar stream RTSP a archivo.
    - Implementa `IAsyncDisposable` para manejo apropiado de recursos.
    - `Pipeline`: El objeto `MediaBlocksPipeline` que orquesta el flujo de medios.
    - `_rtspRawSource`: Un `RTSPRAWSourceBlock` se usa. El "RAW" es clave aquí, ya que proporciona los streams elementales (video y audio) desde la cámara sin intentar decodificarlos inicialmente.
    - `_muxer`: Un `MP4SinkBlock` se usa para escribir los streams entrantes en un archivo MP4.
    - `_decodeBin` y `_audioEncoder`: Estos son bloques opcionales usados solo si `ReencodeAudio` es true. `_decodeBin` decodifica el audio original desde la cámara IP, y `_audioEncoder` (ej. `AACEncoderBlock`) re-codifica a un formato más compatible como AAC.
    - `Filename`: Especifica la ruta del archivo MP4 de salida donde se guardará el video.
    - `ReencodeAudio`: Una propiedad booleana para controlar el procesamiento de audio. Si `true`, audio se re-codifica a AAC. Si `false`, audio se pasa a través directamente. Verifique el formato de audio de su cámara para compatibilidad si se establece en false.

2. **Método `StartAsync(RTSPRAWSourceSettings rtspSettings)`**: Este método inicia el proceso para **grabar stream RTSP**.
    - Inicializa `MediaBlocksPipeline`.
    - **Fuente RTSP**: Crea `_rtspRawSource` con `RTSPRAWSourceSettings`. Estos ajustes incluyen la URL (la ruta al stream de su cámara), credenciales para acceso de usuario, y ajustes de captura de audio.
    - **Sink MP4**: Crea `_muxer` (sink MP4) con el nombre de archivo objetivo.
    - **Ruta de Video (Passthrough)**:
        - Se crea un pad de entrada dinámico para video en el `_muxer`.
        - `Pipeline.Connect(_rtspRawSource.VideoOutput, inputVideoPad);` Esta línea conecta directamente la salida de video raw desde la fuente RTSP a la entrada del sink MP4. No ocurre re-codificación para el stream de video.
    - **Ruta de Audio (Condicional)**: Determina cómo se maneja el audio desde la **cámara** al guardar al archivo.
        - Si `rtspSettings.AudioEnabled` es true:
            - Se crea un pad de entrada dinámico para audio en el `_muxer`.
            - Si `ReencodeAudio` es `true` (recomendado para compatibilidad más amplia de archivos):
                - `_decodeBin` se crea para decodificar el audio entrante desde la cámara IP. Está configurado para procesar solo audio (`audioDisabled: false`).
                - `_audioEncoder` (ej. `AACEncoderBlock`) se crea.
                - El pipeline se conecta: `_rtspRawSource.AudioOutput` -> `_decodeBin.Input` -> `_decodeBin.AudioOutput` -> `_audioEncoder.Input` -> `_audioEncoder.Output` -> `inputAudioPad` (entrada de audio del muxer).
            - Si `ReencodeAudio` es `false`:
                - `Pipeline.Connect(_rtspRawSource.AudioOutput, inputAudioPad);` La salida de audio raw desde la fuente de la cámara se conecta directamente al muxer MP4. *Precaución*: Esto se basa en que el códec de audio original de la cámara sea compatible con el contenedor MP4 (ej. AAC). Formatos comunes como G.711 (PCMU/PCMA) generalmente no son compatibles directamente en archivos MP4 y pueden resultar en audio silencioso o errores de reproducción si guarda de esta manera. Verifique la documentación de su cámara.
    - Inicia el pipeline usando `Pipeline.StartAsync()` para comenzar la grabación de video streaming.

3. **Método `StopAsync()`**: Detiene el `Pipeline`.

4. **Método `DisposeAsync()`**:
    - Limpia todos los recursos, incluyendo el pipeline y bloques individuales de medios.

## Cómo Usar el `RTSPRecorder`

Aquí hay un ejemplo básico de cómo podría usar la clase `RTSPRecorder`:

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VisioForge.Core; // Para VisioForgeX.DestroySDK()
using VisioForge.Core.Types.X.Sources; // Para RTSPRAWSourceSettings
using RTSPCaptureOriginalStream; // Namespace de su clase RTSPRecorder

class Demo
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Captura RTSP de Cámara a MP4 (Stream de Video Original)");
        Console.WriteLine("---------------------------------------------------------");

        string rtspUrl = "rtsp://your_camera_ip:554/stream_path"; // Reemplace con su URL RTSP
        string username = "admin"; // Reemplace con su nombre de usuario, o vacío si ninguno
        string password = "password"; // Reemplace con su contraseña, o vacío si ninguno
        string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "rtsp_original_capture.mp4");

        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

        Console.WriteLine($"Capturando desde: {rtspUrl}");
        Console.WriteLine($"Guardando en: {outputFilePath}");
        Console.WriteLine("Presione cualquier tecla para detener la grabación...");

        var cts = new CancellationTokenSource();
        RTSPRecorder recorder = null;

        try
        {
            recorder = new RTSPRecorder
            {
                Filename = outputFilePath,
                ReencodeAudio = true // Establecer en false para pasar audio a través (verificar compatibilidad)
            };

            recorder.OnError += (s, e) => Console.WriteLine($"ERROR: {e.Message}");
            recorder.OnStatusMessage += (s, msg) => Console.WriteLine($"ESTADO: {msg}");

            // Configurar ajustes de fuente RTSP
            var rtspSettings = new RTSPRAWSourceSettings(new Uri(rtspUrl), audioEnabled: true)
            {
                Login = username,
                Password = password,
                // Ajustar otros ajustes según sea necesario, ej. protocolo de transporte
                // RTSPTransport = VisioForge.Core.Types.RTSPTransport.TCP, 
            };

            if (await recorder.StartAsync(rtspSettings))
            {
                Console.ReadKey(true); // Esperar una pulsación de tecla para detener
            }
            else
            {
                Console.WriteLine("Falló al iniciar la grabación. Verifique mensajes de estado y URL/credenciales RTSP.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
        }
        finally
        {
            if (recorder != null)
            {
                Console.WriteLine("Deteniendo grabación...");
                await recorder.StopAsync();
                await recorder.DisposeAsync();
                Console.WriteLine("Grabación detenida y recursos desechados.");
            }

            // Importante: Limpiar recursos del SDK de VisioForge al salir de la aplicación
            VisioForgeX.DestroySDK(); 
        }

        Console.WriteLine("Presione cualquier tecla para salir.");
        Console.ReadKey(true);
    }
}
```

## Consideraciones Clave

- **Compatibilidad de Audio (Passthrough)**: Si elige `ReencodeAudio = false`, asegúrese de que el códec de audio de la cámara (ej. AAC, MP3) sea compatible con el contenedor MP4. Códecs de audio RTSP comunes como G.711 (PCMU/PCMA) generalmente no son compatibles directamente y resultarán en audio silencioso o errores de reproducción. Re-codificar a AAC generalmente es más seguro para compatibilidad más amplia.
- **Condiciones de Red**: El streaming RTSP es sensible a la estabilidad de la red, así que asegure una conexión de red confiable a la cámara.
- **Manejo de Errores**: Aplicaciones robustas deberían implementar manejo de errores exhaustivo suscribiéndose al evento `OnError` del `RTSPRecorder` (o directamente desde el `MediaBlocksPipeline`).
- **Manejo de Recursos**: Siempre `DisposeAsync` la instancia `RTSPRecorder` (y así el `MediaBlocksPipeline`) cuando termine para liberar recursos. `VisioForgeX.DestroySDK()` debería llamarse una vez cuando su aplicación salga.

## Muestra Completa de GitHub

Para una aplicación de consola completa y ejecutable demostrando estos conceptos, incluyendo entrada de usuario para detalles RTSP y visualización de duración dinámica, por favor refiérase al repositorio oficial de muestras de VisioForge:

- **[Muestra de Captura RTSP Original en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Capture%20Original)**

Esta muestra proporciona un ejemplo más completo y muestra características adicionales.

## Mejores Prácticas

- Siempre implementar manejo de errores apropiado
- Monitorear estabilidad de red para streaming confiable
- Usar ajustes de codificación de audio apropiados
- Gestionar recursos del sistema efectivamente
- Implementar procedimientos de limpieza apropiados

## Solución de Problemas

Problemas comunes y sus soluciones al guardar streams RTSP:

- Problemas de conectividad de red
- Compatibilidad de códec de audio
- Manejo de recursos
- Errores de inicialización de stream
- Consideraciones de almacenamiento de grabación

---
Esta guía proporciona una comprensión fundamental de cómo guardar un stream RTSP preservando el video original mientras maneja flexiblemente el stream de audio usando el SDK de VisioForge Media Blocks. Al aprovechar el `RTSPRAWSourceBlock` y conexiones directas de pipeline, puede lograr grabaciones eficientes y de alta calidad.