---
title: Configuración de Entradas con API Crossbar (.NET)
description: Configura múltiples entradas de video hardware para sintonizadores TV y tarjetas de captura en .NET con implementación crossbar C#.
---

# Configurar Múltiples Entradas de Video de Hardware con Crossbar

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introducción a la Funcionalidad Crossbar

Muchos dispositivos profesionales de captura de video como sintonizadores de TV, tarjetas de captura y hardware de adquisición de video cuentan con múltiples conexiones de entrada físicas. Estos dispositivos podrían incluir varios tipos de entrada como:

- Entradas de video analógico (Compuesto, S-Video)
- Entradas de video digital (HDMI, DisplayPort)
- Entradas de video profesional (SDI, HD-SDI)
- Entradas de sintonizador de televisión (RF, Cable)

La interfaz crossbar permite que tu aplicación seleccione programáticamente entre estas diferentes entradas de hardware y enrute las señales apropiadamente.

## Guía de Implementación

### Paso 1: Inicializar Interfaz Crossbar

Primero, necesitas inicializar la interfaz crossbar para tu dispositivo de captura de video. Esto establece la conexión con las capacidades de selección de entrada del hardware.

```cs
// Inicializar la interfaz crossbar y verificar si la funcionalidad crossbar existe
var crossBarFound = VideoCapture1.Video_CaptureDevice_CrossBar_Init();

// Si crossBarFound es true, el dispositivo soporta múltiples entradas que pueden configurarse
```

### Paso 2: Descubrir Opciones de Entrada Disponibles

Después de inicializar, puedes recuperar todas las entradas disponibles que pueden conectarse a la salida especificada (típicamente "Video Decoder").

```cs
// Limpiar cualquier configuración de conexión crossbar existente
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections();

// Limpiar cualquier elemento previo en tu desplegable de UI
cbCrossbarVideoInput.Items.Clear();

// Poblar el desplegable con todas las fuentes de entrada disponibles que pueden conectarse a "Video Decoder"
foreach (string inputSource in VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput("Video Decoder"))
{
    // Añadir cada fuente de entrada disponible a tu elemento de selección de UI
    cbCrossbarVideoInput.Items.Add(inputSource);
}
```

### Paso 3: Aplicar Configuración de Entrada Seleccionada

Cuando el usuario selecciona su fuente de entrada deseada, aplica esta configuración al dispositivo conectando la entrada seleccionada a la salida "Video Decoder".

```cs
// Primero limpiar cualquier conexión existente para asegurar estado limpio
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections(); 

// Conectar la entrada seleccionada (desde desplegable de UI) a la salida "Video Decoder"
// El parámetro final (true) habilita la conexión
VideoCapture1.Video_CaptureDevice_CrossBar_Connect(cbCrossbarVideoInput.Text, "Video Decoder", true);

// En este punto, el dispositivo usará la entrada seleccionada para captura de video
```

### Paso 4: Manejar Cambios de Conexión

Para una experiencia de usuario óptima, considera implementar manejadores de eventos para detectar cuando los usuarios cambian la selección de entrada y reaplicar la configuración correspondientemente.

## Dependencias Requeridas

Para implementar funcionalidad crossbar, tu aplicación debe incluir los redistribuibles apropiados de captura de video:

- Paquetes redist de captura de video:
  - [Arquitectura x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Arquitectura x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Consejos de Solución de Problemas

- No todos los dispositivos soportan funcionalidad crossbar - verifica el valor de `crossBarFound` después de la inicialización
- Algunos dispositivos pueden tener nombres de salida diferentes a "Video Decoder"
- Los cambios pueden no tomar efecto hasta después de que la sesión de captura sea iniciada

---
Visita nuestro repositorio de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para muestras de código adicionales y ejemplos de implementación.