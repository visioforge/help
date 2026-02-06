---
title: Streaming de Video en Red a Flash Media Server
description: Transmita video a Adobe Flash Media Server en .NET con efectos en tiempo real, ajustes de calidad y cambio de dispositivos para streaming profesional.
---

# Streaming a Adobe Flash Media Server: Guía de Implementación Avanzada

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } 

## Introducción

Adobe Flash Media Server (FMS) sigue siendo una solución potente para streaming de contenido de video a través de varias plataformas. Esta guía demuestra cómo implementar streaming de video de alta calidad a Adobe Flash Media Server usando los SDK .NET de VisioForge. La integración soporta efectos de video en tiempo real, ajuste de calidad y cambio fluido de dispositivos durante sesiones de streaming.

## Prerrequisitos

Antes de implementar la funcionalidad de streaming, asegúrese de tener:

- VisioForge Video Capture SDK .NET o Video Edit SDK .NET instalado
- Adobe Flash Media Server (o un servicio compatible como Wowza con soporte RTMP)
- Adobe Flash Media Live Encoder (FMLE)
- .NET Framework 4.7.2 o posterior
- Visual Studio 2022 o más nuevo
- Comprensión básica de programación C#

## Guía de la aplicación de demostración

La aplicación de demostración proporcionada con los SDK de VisioForge ofrece una forma directa de probar la funcionalidad de streaming. Aquí hay una guía detallada:

1. Inicie la aplicación de demostración principal
2. Navegue a la pestaña "Network Streaming"
3. Habilite el streaming seleccionando la casilla "Enabled"
4. Seleccione el botón de radio "External" para compatibilidad con codificador externo
5. Inicie la previsualización o captura para inicializar el stream de video
6. Abra Adobe Flash Media Live Encoder
7. Configure FMLE para usar "VisioForge Network Source" como la fuente de video
8. Configure los parámetros de video:
   - Resolución (ej., 1280x720, 1920x1080)
   - Tasa de cuadros (típicamente 25-30 fps para streaming fluido)
   - Intervalo de keyframe (recomendar 2 segundos)
   - Ajustes de calidad de video
9. Seleccione "VisioForge Network Source Audio" como la fuente de audio
10. Configure su conexión a Adobe Flash Media Server
11. Presione Start para iniciar el streaming

El video del SDK ahora se está transmitiendo a su instancia FMS. Puede aplicar efectos en tiempo real, ajustar configuraciones o incluso detener el SDK para cambiar dispositivos de entrada sin terminar la sesión de streaming del lado del servidor.

## Implementación en aplicaciones personalizadas

### Componentes requeridos

Para implementar esta funcionalidad en su aplicación personalizada, necesitará:

- Redistribuibles del SDK (disponibles en el paquete de instalación del SDK)
- Referencias a los ensamblados del SDK de VisioForge
- Configuraciones apropiadas de firewall y red para permitir streaming

## Redistribuibles requeridos

Asegure que los siguientes componentes estén incluidos con su aplicación:

- Paquetes redistribuibles del SDK de VisioForge
- Microsoft Visual C++ Runtime (versión apropiada para su SDK)
- Runtime de .NET Framework (si no usa despliegue autocontenido)

## Conclusión

El streaming a Adobe Flash Media Server usando los SDK Video Capture o Edit de VisioForge ofrece una solución flexible y potente para implementar streaming de video de alta calidad en aplicaciones .NET. La implementación soporta efectos en tiempo real, ajustes de calidad y cambio fluido de dispositivos, haciéndola adecuada para una amplia gama de aplicaciones de streaming.

Siguiendo esta guía, los desarrolladores pueden implementar soluciones de streaming robustas que aprovechan las potentes características tanto de los SDK de VisioForge como de la plataforma de streaming de Adobe.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y proyectos de ejemplo.