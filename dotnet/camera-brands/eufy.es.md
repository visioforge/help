---
title: Eufy Security RTSP URL - Conectar Cámara en C# .NET
description: Conecte cámaras Eufy Security en C# .NET con patrones de URL RTSP. El soporte ONVIF y RTSP varía según el modelo. Guía para eufyCam, SoloCam e Indoor Cam.
---

# Cómo Conectar una Cámara Eufy Security en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Eufy Security** es una marca de seguridad para el hogar inteligente propiedad de **Anker Innovations**, con sede en Changsha, China. Eufy es conocida por almacenamiento local (sin suscripción obligatoria a la nube), detección impulsada por IA y una amplia gama de cámaras interiores, exteriores, con batería y timbre. El soporte RTSP y ONVIF varía significativamente según el modelo y la versión de firmware.

**Datos clave:**

- **Líneas de productos:** eufyCam (batería), SoloCam (independiente), Indoor Cam, Floodlight Cam, Video Doorbell, HomeBase
- **Soporte de protocolos:** RTSP (modelos selectos, debe habilitarse), ONVIF (firmware más reciente), aplicación Eufy Security
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** Sin valores predeterminados estándar — se establecen durante la habilitación de RTSP
- **Soporte ONVIF:** Añadido en actualizaciones de firmware recientes para muchos modelos
- **Códecs de vídeo:** H.264, H.265 (modelos selectos)
- **Almacenamiento local:** Sí — HomeBase o microSD de la cámara (no requiere nube para grabación)

!!! info "El Soporte RTSP/ONVIF Varía Según el Modelo"
    Eufy ha ido añadiendo gradualmente soporte RTSP y ONVIF en toda su línea de productos mediante actualizaciones de firmware. No todos los modelos soportan estas funciones. Verifique la configuración de la aplicación Eufy Security para su cámara específica para ver si RTSP está disponible.

## Soporte RTSP por Modelo

| Modelo | RTSP | ONVIF | Notas |
|--------|------|-------|-------|
| eufyCam 2 / 2 Pro | Sí (vía HomeBase) | Sí | Requiere HomeBase 2 |
| eufyCam 2C / 2C Pro | Sí (vía HomeBase) | Sí | Requiere HomeBase 2 |
| eufyCam 3 / 3C | Sí (vía HomeBase 3) | Sí | Requiere HomeBase 3 |
| eufyCam S330 | Sí (vía HomeBase 3) | Sí | Modelo 4K |
| SoloCam S340 | Sí | Sí | Doble lente, RTSP independiente |
| SoloCam C210 | Sí | Sí | Independiente con RTSP |
| Indoor Cam 2K | Sí | Sí | WiFi, depende del firmware |
| Indoor Cam Pan & Tilt | Sí | Sí | WiFi, depende del firmware |
| Floodlight Cam 2 Pro | Sí | Sí | Con cable |
| Video Doorbell 2K | Limitado | No | Solo vía HomeBase |
| Video Doorbell Dual | Limitado | No | Solo vía HomeBase |

## Habilitación de RTSP

### Para Cámaras Conectadas a HomeBase (serie eufyCam)

1. Abra la aplicación **Eufy Security**
2. Vaya a **Configuración de HomeBase > Almacenamiento > NAS** o **RTSP**
3. Habilite la transmisión RTSP
4. Establezca un nombre de usuario y contraseña para RTSP
5. Anote la URL RTSP mostrada para cada cámara

### Para Cámaras Independientes (SoloCam, Indoor Cam, Floodlight)

1. Abra la aplicación **Eufy Security**
2. Seleccione su cámara → **Configuración** (icono de engranaje)
3. Busque **RTSP** o **Avanzado > Flujo RTSP**
4. Habilite RTSP y establezca las credenciales
5. Anote la URL RTSP proporcionada

## Patrones de URL RTSP

### Formato de URL Estándar

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live0
```

| Flujo | Patrón de URL | Descripción |
|-------|---------------|-------------|
| Flujo principal | `rtsp://IP:554/live0` | Resolución completa |
| Subflujo | `rtsp://IP:554/live1` | Menor resolución |

### URLs RTSP de HomeBase

Cuando se conecta a través de un HomeBase, la URL RTSP apunta a la IP del HomeBase:

```
rtsp://[USERNAME]:[PASSWORD]@[HOMEBASE_IP]:554/live0
```

Para múltiples cámaras en un HomeBase, cada cámara obtiene una ruta de flujo única mostrada en la aplicación.

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/live0` | Flujo principal (común) |
| `rtsp://IP:554/live1` | Subflujo |
| `rtsp://IP:554/stream1` | Formato alternativo (algunos modelos) |
| `rtsp://IP:554/h264_stream` | H.264 explícito (algunos firmware) |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Eufy con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Eufy SoloCam S340, main stream
var uri = new Uri("rtsp://192.168.1.90:554/live0");
var username = "rtsp_user"; // set in Eufy Security app
var password = "rtsp_pass";
```

Para acceso al subflujo, use `/live1` en lugar de `/live0`.

## Solución de Problemas

### La opción RTSP no es visible en la aplicación

El soporte RTSP requiere versiones de firmware específicas. Actualice el firmware de su cámara y HomeBase a través de la aplicación Eufy Security. Si RTSP aún no aparece, su modelo puede no soportarlo todavía.

### RTSP de HomeBase vs independiente

- **Cámaras HomeBase** (serie eufyCam): Los flujos RTSP provienen de la **IP del HomeBase**, no de la IP de la cámara. El HomeBase actúa como proxy.
- **Cámaras independientes** (SoloCam, Indoor Cam): Los flujos RTSP provienen directamente de la **IP de la cámara**.

### Cortes de flujo en cámaras con batería

Los modelos eufyCam con batería pueden dejar de transmitir RTSP cuando están en modo de espera. La cámara debe estar grabando activamente o en modo de "transmisión continua" para acceso RTSP continuo. Esto impacta significativamente la vida de la batería.

### Descubrimiento ONVIF

El firmware más reciente de Eufy soporta descubrimiento ONVIF. Use ONVIF para encontrar automáticamente cámaras en su red en lugar de configurar manualmente las URLs RTSP.

### Inconsistencias de firmware

Eufy ha implementado el soporte RTSP/ONVIF gradualmente. Diferentes cámaras en su configuración pueden tener diferentes capacidades dependiendo de su versión de firmware. Actualice siempre todos los dispositivos al firmware más reciente.

## Preguntas Frecuentes

**¿Las cámaras Eufy soportan RTSP?**

Muchas cámaras Eufy ahora soportan RTSP, pero debe habilitarse en la aplicación Eufy Security y varía según el modelo. Las cámaras conectadas a HomeBase transmiten RTSP a través del HomeBase, mientras que las cámaras independientes transmiten directamente. Verifique las capacidades de su modelo específico en la configuración de la aplicación.

**¿Las cámaras Eufy requieren suscripción a la nube para RTSP?**

No. La transmisión RTSP funciona localmente sin ninguna suscripción a la nube. Las cámaras Eufy almacenan las grabaciones en el HomeBase o en la tarjeta microSD de la cámara. La suscripción a la nube (Eufy Security Plan) es opcional y proporciona almacenamiento adicional en la nube y funciones.

**¿Puedo usar cámaras Eufy sin la aplicación Eufy?**

La configuración inicial requiere la aplicación Eufy Security. Después de la configuración y habilitación de RTSP, puede acceder al flujo RTSP sin la aplicación. Sin embargo, las actualizaciones de firmware y los cambios de configuración aún requieren la aplicación.

**¿Cuál es la diferencia entre RTSP de HomeBase y RTSP independiente?**

El RTSP de HomeBase transmite todas las cámaras conectadas a través de la dirección IP del HomeBase. El HomeBase actúa como puerta de enlace. Las cámaras independientes (SoloCam, Indoor Cam, Floodlight) transmiten directamente desde su propia IP. El RTSP de HomeBase puede tener una latencia ligeramente mayor.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Arlo](arlo.md) — Alternativa de consumo (sin RTSP)
- [Guía de Conexión Reolink](reolink.md) — Consumo con RTSP nativo
- [Guía de Conexión EZVIZ](ezviz.md) — Cámaras de hogar inteligente con RTSP
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
