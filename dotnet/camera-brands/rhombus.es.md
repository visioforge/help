---
title: Cámaras Rhombus Systems - Guía de Conexión RTSP en C# .NET
description: Opciones de integración de cámaras Rhombus en C# .NET. Arquitectura gestionada en la nube, acceso API y enfoques alternativos para cámaras Rhombus Systems.
meta:
  - name: robots
    content: "noindex, follow"
---

# Cómo Conectar una Cámara Rhombus en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Rhombus Systems** (Rhombus, Inc.) es una empresa estadounidense de seguridad de vídeo gestionada en la nube con sede en Sacramento, California. Fundada en 2016, Rhombus proporciona cámaras empresariales, sensores y control de acceso con una plataforma de gestión orientada a la nube. Similar a Verkada, las cámaras Rhombus se gestionan a través de una consola centralizada en la nube.

**Datos clave:**

- **Líneas de productos:** Serie R (domo), Serie R Pro (avanzada), Serie R Mini (compacta)
- **Arquitectura:** Gestionada en la nube con procesamiento de IA en la cámara
- **Soporte RTSP:** Limitado — disponible en algunos modelos mediante configuración LAN
- **Soporte ONVIF:** No
- **Códecs de vídeo:** H.264, H.265
- **Acceso API:** API de Rhombus (REST, requiere suscripción)
- **Almacenamiento en cámara:** Sí (tarjeta SD local para almacenamiento en el borde)

!!! info "Transmisión Local Limitada"
    Algunos modelos de cámaras Rhombus soportan RTSP para transmisión local en LAN, pero esta función debe habilitarse a través de la consola Rhombus y no está disponible en todos los modelos o niveles de suscripción. Verifique la configuración de su cuenta Rhombus para la disponibilidad de RTSP.

## Acceso RTSP (Donde Está Disponible)

### Habilitación de RTSP

En cámaras Rhombus compatibles:

1. Inicie sesión en la **Consola Rhombus** (console.rhombus.com)
2. Navegue a la configuración de su cámara
3. Busque **Transmisión local** o configuración **RTSP**
4. Habilite RTSP y anote la URL proporcionada

### Formato de URL RTSP

Cuando RTSP está disponible:

```
rtsp://[IP]:554/live
```

El formato exacto de URL y el método de autenticación dependen del modelo de cámara y la versión de firmware. La consola Rhombus proporcionará la URL específica cuando RTSP esté habilitado.

## Conexión con VisioForge SDK

Si RTSP está disponible en su cámara Rhombus, utilice la URL con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Rhombus camera with RTSP enabled
var uri = new Uri("rtsp://192.168.1.90:554/live");
var username = "admin";
var password = "YourPassword"; // from Rhombus console
```

## Integración vía API de Rhombus

Para cámaras sin acceso RTSP, Rhombus proporciona una API REST que ofrece:

- Listado y estado de cámaras
- Exportación y descarga de clips de vídeo
- Recuperación de capturas/miniaturas
- Datos de eventos y analítica
- Notificaciones webhook

La API no proporciona flujos RTSP en tiempo real. Está diseñada para recuperación de clips, acceso a metadatos y flujos de trabajo de automatización.

## Solución de Problemas

### RTSP no disponible en mi cámara

No todas las cámaras o niveles de suscripción de Rhombus soportan RTSP. Contacte al soporte de Rhombus para verificar la disponibilidad de RTSP para su modelo de cámara y plan específicos.

### El flujo RTSP se desconecta

Las cámaras Rhombus priorizan la conectividad a la nube. Si el flujo RTSP local es inestable:

1. Asegúrese de que la cámara tenga suficiente ancho de banda de red para flujos de nube y locales
2. Use el subflujo para menores requisitos de ancho de banda
3. Verifique la consola Rhombus para actualizaciones de firmware

## Preguntas Frecuentes

**¿Las cámaras Rhombus soportan RTSP?**

Algunas cámaras Rhombus soportan RTSP para transmisión local en LAN, pero debe habilitarse a través de la consola Rhombus y puede no estar disponible en todos los modelos o niveles de suscripción. Contacte al soporte de Rhombus para detalles específicos.

**¿Puedo usar VisioForge SDK con cámaras Rhombus?**

Si su cámara Rhombus tiene RTSP habilitado, sí. Use la URL RTSP de la consola Rhombus con la fuente RTSP del VisioForge SDK. Para cámaras sin RTSP, necesitaría usar la API REST de Rhombus por separado para acceso a clips y capturas.

**¿Cómo se compara Rhombus con Verkada?**

Ambas son plataformas gestionadas en la nube. Rhombus ofrece RTSP en algunos modelos, mientras que Verkada no soporta RTSP en absoluto. Ambas proporcionan APIs REST para acceso a clips/capturas. Consulte nuestra [guía de Verkada](verkada.md) para comparación.

**¿Cuáles son buenas alternativas a Rhombus con soporte RTSP completo?**

Para cámaras empresariales con soporte nativo de RTSP y ONVIF, considere [Axis](axis.md), [Bosch](bosch.md), [Hanwha Vision](hanwha.md) o [Avigilon](avigilon.md).

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Verkada](verkada.md) — Otra plataforma gestionada en la nube
- [Guía de Conexión Axis](axis.md) — Alternativa empresarial con RTSP completo
- [Guía de Conexión Hanwha Vision](hanwha.md) — Alternativa empresarial con RTSP
- [Instalación del SDK y Ejemplos](index.md#comenzar)
