---
title: Integración NDI para Captura de Video en .NET
description: Implementa fuentes de video NDI en el SDK .NET con guía completa para enumerar, conectar y capturar video de alta calidad desde cámaras NDI en C#.
---

# Implementar Fuentes de Video NDI en Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introducción a la Tecnología NDI

Network Device Interface (NDI) es un estándar de alto rendimiento para flujos de trabajo de producción basados en IP, desarrollado por NewTek. Permite que productos compatibles con video se comuniquen, entreguen y reciban video de calidad broadcast sobre una conexión de red estándar con baja latencia.

Nuestro SDK proporciona soporte robusto para fuentes NDI, permitiendo que tus aplicaciones .NET se integren perfectamente con cámaras NDI y software habilitado para NDI. Esto lo hace ideal para entornos de producción en vivo, aplicaciones de streaming, soluciones de videoconferencia y cualquier sistema que requiera integración de video de red de alta calidad.

### Prerrequisitos para Integración NDI

Antes de implementar funcionalidad NDI en tu aplicación, necesitarás instalar uno de los siguientes:

- [NDI SDK](https://ndi.video/for-developers/ndi-sdk/#download) - Recomendado para desarrolladores construyendo aplicaciones profesionales
- NDI Tools - Suficiente para pruebas básicas y desarrollo

Estas herramientas proporcionan los componentes de tiempo de ejecución necesarios para la comunicación NDI. Después de la instalación, tu sistema podrá descubrir y conectarse a fuentes NDI en tu red.

## Descubrir Fuentes NDI en Tu Red

El primer paso para trabajar con NDI es enumerar las fuentes disponibles. Nuestro SDK hace este proceso sencillo con métodos dedicados para escanear tu red en busca de dispositivos y aplicaciones habilitados para NDI.

### Enumerar Fuentes NDI Disponibles

=== "VideoCaptureCore"

    
    ```cs
    var lst = await VideoCapture1.IP_Camera_NDI_ListSourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    var lst = await DeviceEnumerator.Shared.NDISourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri.URL);
    }
    ```
    


Los métodos de enumeración asíncronos escanean tu red y devuelven una lista de fuentes NDI disponibles. Cada fuente tiene un identificador único que usarás para establecer una conexión. El proceso de enumeración típicamente toma unos segundos, dependiendo de las condiciones de red y el número de fuentes disponibles.

## Conectar a Fuentes NDI

Una vez que has identificado las fuentes NDI en tu red, el siguiente paso es establecer una conexión. Esto implica crear el objeto de configuración apropiado y configurarlo para tus requisitos específicos.

### Configurar Ajustes de Fuente NDI

=== "VideoCaptureCore"

    
    ```cs
    // Crear un objeto de configuración de fuente de cámara IP
    settings = new IPCameraSourceSettings
    {
        URL = new Uri("URL de fuente NDI")
    };
    
    // Establecer el tipo de fuente a NDI
    settings.Type = IPSourceEngine.NDI;
    
    // Habilitar o deshabilitar captura de audio
    settings.AudioCapture = false; 
    
    // Establecer información de inicio de sesión si es necesario
    settings.Login = "usuario";
    settings.Password = "contraseña";
    
    // Establecer la fuente de cámara IP
    VideoCapture1.IP_Camera_Source = settings;
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Crear configuración de fuente NDI
    var ndiSource = new NDISourceSettings(new Uri("URL de fuente NDI"));
    
    // Configurar captura de audio si es necesario
    ndiSource.AudioCapture = true;
    
    // Establecer credenciales si son requeridas
    ndiSource.Login = "usuario";
    ndiSource.Password = "contraseña";
    
    // Establecer fuente de video
    VideoCapture1.Video_Source = ndiSource;
    ```
    


## Capturar Video desde Fuentes NDI

Después de configurar la fuente NDI, puedes iniciar la captura y vista previa de video.

### Vista Previa de Video en Vivo

```cs
// Iniciar vista previa
await VideoCapture1.StartAsync();
```

### Grabar a Archivo

```cs
// Configurar salida de archivo
var mp4Output = new MP4Output("output.mp4");
VideoCapture1.Outputs_Add(mp4Output);

// Iniciar grabación
await VideoCapture1.StartAsync();
```

## Características Avanzadas de NDI

### Baja Latencia

NDI está diseñado para transmisión de video de baja latencia. Para entornos de producción en vivo, puedes optimizar ajustes para latencia mínima:

```cs
// Habilitar modo de baja latencia si está disponible
ndiSource.LowLatencyMode = true;
```

### Tally y Comunicación Bidireccional

NDI soporta señales de tally para indicar cuando una fuente está al aire. Esto es útil para entornos de producción en vivo:

```cs
// Las señales de tally se manejan automáticamente cuando corresponde
```

## Mejores Prácticas

1. **Configuración de Red**: Asegúrate de que tu red soporte tráfico multicast para descubrimiento NDI óptimo
2. **Ancho de Banda**: Los flujos NDI pueden consumir ancho de banda significativo; planifica tu capacidad de red apropiadamente
3. **Latencia**: Usa conexiones por cable cuando sea posible para la menor latencia
4. **Firewall**: Asegúrate de que los puertos NDI estén abiertos en tu firewall

## Solución de Problemas

### No se Descubren Fuentes
- Verificar que NDI SDK o Tools estén instalados
- Asegurar que los dispositivos estén en la misma red
- Comprobar configuración del firewall

### Problemas de Calidad de Video
- Verificar capacidad de ancho de banda de red
- Revisar configuración de calidad del encoder en la fuente

### Problemas de Latencia
- Usar conexiones por cable en lugar de WiFi
- Optimizar configuración del buffer

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver integración NDI en acción:

- [Demo Fuente NDI (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/NDI%20Source%20Demo)
- [Demo Principal de Video Capture (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.