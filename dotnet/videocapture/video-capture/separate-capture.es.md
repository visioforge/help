---
title: Captura y Vista Previa Independientes .NET
description: Controla captura y vista previa de forma independiente en .NET con ejemplos de código para gestión eficiente de grabación.
---

# Gestionando Captura de Video y Vista Previa Independientemente en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Al desarrollar aplicaciones de video, a menudo es necesario iniciar o detener la grabación mientras se mantiene una vista previa ininterrumpida. Esta capacidad es esencial para crear software de grabación de video profesional, aplicaciones de seguridad, o cualquier escenario donde se requiera retroalimentación visual continua independientemente del estado de grabación.

Esta guía demuestra cómo controlar independientemente las operaciones de captura de video sin afectar la visualización de la vista previa. Esta técnica se aplica a varios escenarios de captura incluyendo grabación de cámara, captura de pantalla y otras fuentes de entrada.

## ¿Por Qué Separar Vista Previa y Captura?

Hay varias ventajas al separar la funcionalidad de vista previa y captura:

1. **Experiencia de Usuario Mejorada** - Los usuarios pueden ver continuamente lo que se está capturando, incluso cuando no se está grabando
2. **Eficiencia de Recursos** - Previene el inicio/parada innecesario de flujos de video
3. **Latencia Reducida** - Elimina el retraso asociado con restablecer la vista previa después de detener una grabación
4. **Mayor Control** - Proporciona gestión más precisa de las sesiones de grabación

## Opciones de Implementación

Hay dos enfoques principales para implementar esta funcionalidad dependiendo de qué versión del SDK esté usando:

=== "VideoCaptureCoreX"

    
    ### Método 1: Usando VideoCaptureCoreX
    
    El enfoque VideoCaptureCoreX ofrece una forma simplificada de gestionar salidas y controlar estados de captura.
    
    #### Paso 1: Configurar la Salida
    
    Primero, agregue una nueva salida con los ajustes deseados. En este ejemplo, usaremos salida MP4. Note el parámetro `false` que indica que no queremos iniciar la captura inmediatamente:
    
    ```cs
    VideoCapture1.Outputs_Add(new MP4Output("salida.mp4"), false); // false - no iniciar captura inmediatamente. 
    ```
    
    #### Paso 2: Iniciar Solo Vista Previa
    
    A continuación, inicie la vista previa de video sin iniciar la captura:
    
    ```cs
    await VideoCapture1.StartAsync();
    ```
    
    #### Paso 3: Iniciar Captura Cuando Sea Necesario
    
    Cuando quiera comenzar a grabar, inicie la captura de video real a su destino de salida:
    
    ```cs
    await VideoCapture1.StartCaptureAsync(0, "salida.mp4"); // 0 - índice de la salida.
    ```
    
    #### Paso 4: Detener Captura Mientras Mantiene la Vista Previa
    
    Para detener la grabación mientras mantiene la vista previa activa:
    
    ```cs
    await VideoCapture1.StopCaptureAsync(0); // 0 - índice de la salida.
    ```
    
    ### Gestión Avanzada de Salidas
    
    Puede agregar múltiples salidas con diferentes configuraciones:
    
    ```cs
    // Agregar salida MP4
    VideoCapture1.Outputs_Add(new MP4Output("grabacion_principal.mp4"), false);
    
    // Agregar salida adicional para transmisión
    VideoCapture1.Outputs_Add(new RTMPOutput("rtmp://streaming.example.com/live/stream"), false);
    
    // Iniciar vista previa
    await VideoCapture1.StartAsync();
    
    // Iniciar grabación a ambas salidas
    await VideoCapture1.StartCaptureAsync(0, "grabacion_principal.mp4");
    await VideoCapture1.StartCaptureAsync(1, "rtmp://streaming.example.com/live/stream");
    ```
    
    ### Control de Salida Con Índices
    
    Al gestionar múltiples salidas, el parámetro de índice se vuelve crucial:
    
    ```cs
    // Detener la grabación MP4 pero continuar transmitiendo
    await VideoCapture1.StopCaptureAsync(0);
    
    // Después, detener la transmisión también
    await VideoCapture1.StopCaptureAsync(1);
    ```
    

=== "VideoCaptureCore"

    
    ### Método 2: Usando VideoCaptureCore
    
    El enfoque antiguo VideoCaptureCore usa un patrón diferente con habilitación explícita de captura separada.
    
    #### Paso 1: Habilitar Modo de Captura Separada
    
    Comience habilitando la funcionalidad de captura separada:
    
    ```cs
    VideoCapture1.SeparateCapture_Enabled = true;
    ```
    
    #### Paso 2: Configurar Modo de Captura
    
    Establezca el modo de captura apropiado para su aplicación:
    
    ```cs
    VideoCapture1.Mode = VideoCaptureMode.VideoCapture;
    // Otras opciones incluyen:
    // VideoCaptureMode.ScreenCapture
    // VideoCaptureMode.AudioCapture
    // etc.
    ```
    
    #### Paso 3: Configurar Formato de Salida
    
    Establezca la configuración de formato de salida deseada:
    
    ```cs
    VideoCapture1.Output_Format = ...
    ```
    
    #### Paso 4: Iniciar Vista Previa
    
    Comience la vista previa sin iniciar la grabación real:
    
    ```cs
    await VideoCapture1.StartAsync();
    ```
    
    #### Paso 5: Iniciar Captura Cuando Sea Necesario
    
    Cuando quiera comenzar a grabar, inicie el proceso de captura separada:
    
    ```cs
    await VideoCapture1.SeparateCapture_StartAsync();
    ```
    
    #### Paso 6: Detener Captura Mientras Mantiene la Vista Previa
    
    Para detener la grabación mientras mantiene la vista previa activa:
    
    ```cs
    await VideoCapture1.SeparateCapture_StopAsync();
    ```
    
    ### Cambios Dinámicos de Nombre de Archivo
    
    Una ventaja clave del enfoque de captura separada es la capacidad de cambiar el nombre del archivo de salida durante una sesión de grabación activa:
    
    ```cs
    await VideoCapture1.SeparateCapture_ChangeFilenameOnTheFlyAsync("archivo_nuevo.mp4");
    ```
    
    Esto es particularmente útil para:
    
    - Crear segmentos de archivo secuenciales
    - Implementar límites de tamaño de archivo con continuación automática
    - Responder a cambios de nombre de archivo iniciados por el usuario
    


## Consideraciones de Implementación

### Memoria y Rendimiento

Al implementar la funcionalidad de captura y vista previa separadas, tenga en cuenta estas consideraciones de rendimiento:

- **Uso de Memoria**: Mantener una vista previa activa mientras no se captura consume recursos del sistema
- **Impacto de CPU**: Las operaciones de codificación durante la captura aumentan la carga de CPU
- **Gestión de Búfer**: Asegure un manejo apropiado del búfer para prevenir fugas de memoria

### Consideraciones de UI

La UI de su aplicación debería indicar claramente el estado actual de la vista previa y la captura:

- Use diferentes indicadores visuales para vista previa únicamente vs. grabación activa
- Implemente controles de UI apropiados para cada estado
- Considere agregar temporizadores e indicadores de grabación

## Mejores Prácticas de Integración

Para rendimiento y confiabilidad óptimos:

1. **Inicializar Temprano**: Configure su configuración de captura al inicio de la aplicación
2. **Liberar Recursos**: Siempre detenga la captura y la vista previa cuando ya no sean necesarias
3. **Manejar Cambios de Dispositivo**: Implemente detección y manejo apropiado de conexión/desconexión de dispositivos
4. **Gestión de Hilos**: Realice operaciones de captura en hilos de fondo para prevenir congelamiento de UI

## Conclusión

Separar las operaciones de captura y vista previa de video proporciona mayor flexibilidad y una mejor experiencia de usuario en aplicaciones de video. Siguiendo los enfoques descritos en esta guía, puede implementar esta funcionalidad en sus aplicaciones .NET con los componentes VideoCaptureCoreX o VideoCaptureCore.

Estas técnicas pueden aplicarse a una amplia gama de escenarios incluyendo grabación de webcam, captura de pantalla, sistemas de vigilancia y herramientas de producción de video profesional.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.