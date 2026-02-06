---
title: Integrar Streaming OBS en Video Capture SDK .Net
description: Transmite video y audio con integración OBS en Video Capture SDK para soluciones de broadcast profesionales en WinForms, WPF y Consola.
---

# Integrar Streaming OBS en Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Integración OBS

Open Broadcaster Software (OBS) se ha convertido en el estándar de la industria para streaming en vivo y grabación de video. El Video Capture SDK .Net proporciona capacidades robustas para transmitir video y audio desde múltiples fuentes directamente a OBS, creando un pipeline poderoso para creación de contenido de calidad broadcast.

Esta integración permite a los desarrolladores construir aplicaciones que pueden:

- Capturar desde múltiples dispositivos de cámara simultáneamente
- Procesar y mejorar flujos de video en tiempo real
- Mezclar varias fuentes de contenido antes de enviar a OBS
- Crear soluciones de broadcasting profesionales con configuración mínima

Ya sea que estés desarrollando aplicaciones para WinForms, WPF o entornos de consola, el SDK proporciona una API consistente para integración con OBS.

## Cómo Funciona la Integración OBS

El SDK aprovecha la tecnología DirectShow Virtual Camera para crear un puente entre tu aplicación y OBS. Este enfoque ofrece varias ventajas:

1. **Streaming sin latencia**: La transferencia directa de memoria minimiza el retraso
2. **Flexibilidad de formato**: Soporte para varias resoluciones y tasas de fotogramas
3. **Configuración mínima**: OBS reconoce el dispositivo virtual automáticamente
4. **Sincronización de audio**: Capacidades combinadas de streaming audio-video

La cámara virtual aparece para OBS como un dispositivo de webcam estándar, haciéndola inmediatamente utilizable dentro de cualquier escena OBS.

## Guía de Implementación

### Componentes Requeridos

Antes de implementar streaming OBS, asegúrate de tener los siguientes componentes instalados:

- Video Capture SDK .Net (se recomienda última versión)
- Componentes DirectShow Virtual Camera
- OBS Studio (se recomienda versión 27.0 o superior)
- .NET Framework 4.6.2 o superior (para compatibilidad completa)

### Implementación Básica

El siguiente código demuestra cómo habilitar salida de cámara virtual en tu aplicación:

```cs
// Inicializar el componente de captura de video
var videoCapture = new VideoCaptureCore();

// Configurar ajustes básicos de captura
// ...

// Habilitar salida de cámara virtual
videoCapture.Virtual_Camera_Output_Enabled = true;

// Iniciar captura
videoCapture.Start();
```

Esta implementación mínima enviará la alimentación de cámara al dispositivo virtual que OBS puede usar como fuente de entrada.

## Configurar OBS para Integración con SDK

### Añadir la Fuente de Cámara Virtual

1. Lanza OBS Studio
2. En tu escena, haz clic en el botón "+" bajo Fuentes
3. Selecciona "Dispositivo de Captura de Video" de la lista
4. Nombra tu fuente (ej. "Cámara Virtual SDK")
5. En el diálogo de Propiedades, selecciona "VisioForge Virtual Camera" del desplegable Dispositivo
6. Configura resolución y FPS para coincidir con tus ajustes del SDK
7. Haz clic en "OK" para añadir la fuente

![Fuente de captura de video OBS](/help/docs/dotnet/videocapture/3rd-party-software/obs2.webp)

### Configurar Audio

Para incluir audio de tu aplicación en OBS:

1. Habilita la salida de Tarjeta de Audio Virtual en tu código SDK
2. En OBS, añade una nueva fuente de "Captura de Entrada de Audio"
3. Selecciona "VisioForge Virtual Audio Card" como el dispositivo
4. Ajusta niveles de audio según sea necesario

```cs
// Habilitar salida de cámara y audio virtual
videoCapture.Virtual_Camera_Output_Enabled = true;
videoCapture.Virtual_Audio_Output_Enabled = true;

// Iniciar captura
videoCapture.Start();
```

## Características Avanzadas

### Transmitir Múltiples Fuentes

Puedes combinar múltiples fuentes de video en tu aplicación antes de enviar a OBS:

```cs
// Configurar fuente de video primaria
videoCapture.Video_CaptureDevice = new VideoCaptureDevice("Cámara Principal");

// Añadir superposición de video secundario
// ...

// Habilitar salida virtual
videoCapture.Virtual_Camera_Output_Enabled = true;
videoCapture.Start();
```

### Configuración de Resolución y Tasa de Fotogramas

Asegúrate de que tu configuración del SDK coincida con tus ajustes de OBS para rendimiento óptimo:

```cs
// Establecer resolución de captura
videoCapture.Video_CaptureDevice.FrameSize = new Size(1920, 1080);
videoCapture.Video_CaptureDevice.FrameRate = 30;

// Habilitar salida virtual
videoCapture.Virtual_Camera_Output_Enabled = true;
videoCapture.Start();
```

## Requisitos de Despliegue

Al desplegar tu aplicación que usa integración OBS, incluye:

- Redistribuibles base del SDK
- Redistribuibles específicos del SDK
- Redistribuibles del SDK de Cámara Virtual

Para información detallada sobre requisitos de despliegue, consulta la página de [Despliegue](../deployment.md).

## Solución de Problemas

### La Cámara Virtual No Aparece en OBS

Si OBS no muestra la cámara virtual:

- Verifica que el controlador de Cámara Virtual esté instalado
- Reinicia OBS después de instalar los controladores
- Comprueba que la salida de cámara virtual esté habilitada en tu código
- Asegúrate de que la captura de video haya iniciado

### Problemas de Sincronización de Audio

Si el audio no está sincronizado con el video:

- Ajusta la compensación de sincronización de audio en OBS
- Verifica que ambas salidas de audio y video virtuales estén habilitadas
- Comprueba tus tasas de muestreo de audio

### Rendimiento y Latencia

Para minimizar latencia:

- Usa resoluciones y tasas de fotogramas que coincidan
- Habilita "Modo de Ultra Baja Latencia" si está disponible
- Reduce la codificación innecesaria de pipeline

## Conclusión

Integrar el Video Capture SDK con OBS proporciona una solución poderosa para aplicaciones de streaming profesionales. El enfoque de cámara virtual ofrece flexibilidad mientras mantiene compatibilidad con el ecosistema existente de OBS.

---
Para más muestras de código y ejemplos, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).