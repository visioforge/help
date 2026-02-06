---
title: Formato Video Custom con DirectShow en .NET
description: Formatos de video personalizados con filtros DirectShow para pipelines especializados y configuración de códecs en aplicaciones .NET.
---

# Creación de Formatos de Salida de Video Personalizados con Filtros DirectShow

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Descripción general

Trabajar con video en aplicaciones .NET a menudo requiere formatos de salida personalizados para cumplir requisitos específicos del proyecto. Los SDK de VisioForge proporcionan capacidades poderosas para implementar salidas de formato personalizado usando filtros DirectShow, dando a los desarrolladores control preciso sobre los pipelines de procesamiento de audio y video.

Esta guía demuestra técnicas prácticas para implementar formatos de salida personalizados que funcionan perfectamente tanto con Video Capture SDK .NET como con Video Edit SDK .NET, permitiéndole adaptar sus aplicaciones de video a especificaciones exactas.

## ¿Por qué usar formatos de salida personalizados?

Los formatos de salida personalizados ofrecen varias ventajas para desarrolladores .NET:

- Soporte para códecs de video especializados no disponibles en formatos estándar
- Control detallado sobre configuración de compresión de video y audio
- Integración con filtros DirectShow de terceros
- Capacidad para crear formatos de salida propietarios o específicos de la industria
- Optimización para casos de uso específicos (streaming, archivo, edición)

## Comenzando con CustomOutput

La clase `CustomOutput` es la piedra angular para configurar ajustes de salida personalizados en los SDK de VisioForge. Esta clase le permite definir y configurar los filtros usados en su pipeline de procesamiento de video.

Comience inicializando una nueva instancia:

```cs
var customOutput = new CustomOutput();
```

Aunque nuestros ejemplos usan la clase `VideoCaptureCore`, los desarrolladores que usan Video Edit SDK .NET pueden aplicar las mismas técnicas con `VideoEditCore`.

## Estrategias de implementación

Hay dos enfoques principales para implementar salida de formato personalizado con filtros DirectShow:

### Estrategia 1: Pipeline de tres componentes

Este enfoque modular divide el pipeline de procesamiento en tres componentes distintos:

1. Códec de audio
2. Códec de video
3. Multiplexor (contenedor de formato de archivo)

Esta separación proporciona máxima flexibilidad y control sobre cada etapa del proceso. Puede usar tanto filtros DirectShow estándar como códecs especializados para componentes de audio y video.

#### Obtención de códecs disponibles

Comience llenando su UI con códecs y filtros disponibles:

```cs
// Llenar opciones de códec de video
foreach (string codec in VideoCapture1.Video_Codecs)
{
    videoCodecDropdown.Items.Add(codec);
}

// Llenar opciones de códec de audio
foreach (string codec in VideoCapture1.Audio_Codecs)
{
    audioCodecDropdown.Items.Add(codec);
}

// Obtener todos los filtros DirectShow disponibles
foreach (string filter in VideoCapture1.DirectShow_Filters)
{
    directShowAudioFilters.Items.Add(filter);
    directShowVideoFilters.Items.Add(filter);
    multiplexerFilters.Items.Add(filter);
    fileWriterFilters.Items.Add(filter);
}
```

#### Configuración de componentes del pipeline

A continuación, configure sus componentes de procesamiento de video y audio basándose en selecciones del usuario:

```cs
// Configurar códec de video
if (useStandardVideoCodec.Checked)
{
    customOutput.Video_Codec = videoCodecDropdown.Text;
    customOutput.Video_Codec_UseFiltersCategory = false;
}
else
{
    customOutput.Video_Codec = directShowVideoFilters.Text;
    customOutput.Video_Codec_UseFiltersCategory = true;
}

// Configurar códec de audio
if (useStandardAudioCodec.Checked)
{
    customOutput.Audio_Codec = audioCodecDropdown.Text;
    customOutput.Audio_Codec_UseFiltersCategory = false;
}
else
{
    customOutput.Audio_Codec = directShowAudioFilters.Text;
    customOutput.Audio_Codec_UseFiltersCategory = true;
}

// Configurar el multiplexor
customOutput.MuxFilter_Name = multiplexerFilters.Text;
customOutput.MuxFilter_IsEncoder = false;
```

#### Configuración de escritor de archivo personalizado

Para salidas especializadas que requieren un escritor de archivo dedicado:

```cs
// Habilitar escritor de archivo especial si es necesario
customOutput.SpecialFileWriter_Needed = useCustomFileWriter.Checked;
customOutput.SpecialFileWriter_FilterName = fileWriterFilters.Text;
```

Este enfoque le da control granular sobre cada etapa del proceso de codificación, haciéndolo ideal para requisitos de salida complejos.

### Estrategia 2: Filtro todo-en-uno

Este enfoque simplificado usa un solo filtro DirectShow que combina la funcionalidad del multiplexor, códec de video y códec de audio. El SDK maneja inteligentemente la detección de las capacidades del filtro, determinando si:

- Puede escribir archivos directamente sin asistencia
- Requiere el filtro estándar DirectShow File Writer
- Necesita un filtro de escritor de archivo especializado

La implementación es sencilla:

```cs
// Llenar opciones de filtro desde filtros DirectShow disponibles
foreach (string filter in VideoCapture1.DirectShow_Filters)
{
    filterDropdown.Items.Add(filter);
}

// Configurar el filtro todo-en-uno
customOutput.MuxFilter_Name = selectedFilter.Text;
customOutput.MuxFilter_IsEncoder = true;

// Configurar escritor de archivo especializado si es requerido
customOutput.SpecialFileWriter_Needed = requiresCustomWriter.Checked;
customOutput.SpecialFileWriter_FilterName = fileWriterFilter.Text;
```

Este enfoque es más simple de implementar pero ofrece menos control granular sobre componentes individuales del proceso de codificación.

## Simplificación de configuración con UI de diálogo

Para una implementación más amigable para el usuario, VisioForge proporciona un diálogo de configuración integrado que maneja la configuración de formatos personalizados:

```cs
// Crear y configurar el diálogo de configuración
CustomFormatSettingsDialog settingsDialog = new CustomFormatSettingsDialog(
    VideoCapture1.Video_Codecs.ToArray(),
    VideoCapture1.Audio_Codecs.ToArray(),
    VideoCapture1.DirectShow_Filters.ToArray());

// Aplicar configuración a su instancia CustomOutput
settingsDialog.SaveSettings(ref customOutput);
```

Este diálogo proporciona una UI completa para configurar todos los aspectos de formatos de salida personalizados, ahorrando tiempo de desarrollo mientras aún ofrece flexibilidad completa.

## Implementación del proceso de salida

Después de configurar sus ajustes de formato personalizado, necesita aplicarlos a su proceso de captura o edición:

```cs
// Aplicar la configuración de formato personalizado
VideoCapture1.Output_Format = customOutput;

// Establecer el modo de captura
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Especificar ruta de archivo de salida
VideoCapture1.Output_Filename = "video_salida.mp4";

// Iniciar el proceso de captura o codificación
await VideoCapture1.StartAsync();
```

## Consideraciones de rendimiento

Al implementar formatos de salida personalizados, tenga en cuenta estos consejos de rendimiento:

- Los filtros DirectShow varían en eficiencia y uso de recursos
- Pruebe sus combinaciones de filtros con medios de entrada típicos
- Algunos filtros de terceros pueden introducir latencia adicional
- Considere el uso de memoria al procesar video de alta resolución
- La compatibilidad de filtros puede variar entre diferentes versiones de Windows

## Paquetes requeridos

Para usar filtros DirectShow personalizados, asegúrese de tener los paquetes redistribuibles apropiados instalados:

### Video Capture SDK .Net

- [Paquete x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- [Paquete x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Video Edit SDK .Net

- [Paquete x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
- [Paquete x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Solución de problemas

Problemas comunes al trabajar con filtros DirectShow personalizados incluyen:

- Conflictos de compatibilidad de filtros
- Códecs o dependencias faltantes
- Problemas de registro con componentes COM
- Fugas de memoria en filtros de terceros
- Cuellos de botella de rendimiento con gráficos de filtros complejos

Si encuentra problemas, verifique que todos los filtros requeridos estén correctamente registrados en su sistema y que tenga las versiones más recientes tanto de los filtros como del SDK de VisioForge.

## Conclusión

Los formatos de salida personalizados usando filtros DirectShow proporcionan capacidades poderosas para desarrolladores .NET que trabajan con aplicaciones de video. Ya sea que elija la flexibilidad de un pipeline de tres componentes o la simplicidad de un enfoque de filtro todo-en-uno, los SDK de VisioForge le dan las herramientas que necesita para crear exactamente el formato de salida que su aplicación requiere.

---
Para más ejemplos de código e implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).