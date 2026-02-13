---
title: Cómo Conectar una Cámara IP FLIR en C# .NET
description: Conecta cámaras FLIR (Teledyne FLIR) en C# .NET con patrones de URL RTSP y ejemplos de código para modelos Quasar, Saros, Elara térmicos y CF/CM.
---

# Cómo Conectar una Cámara IP FLIR en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**FLIR Systems** (ahora **Teledyne FLIR** tras la adquisición de 2021 por Teledyne Technologies) es un fabricante líder de cámaras de imagen térmica y cámaras de seguridad de luz visible. Con sede en Wilsonville, Oregón, EE.UU., FLIR atiende mercados empresariales, de infraestructura crítica y gubernamentales. FLIR es más conocido por la imagen térmica pero también produce una gama completa de cámaras IP de luz visible para vigilancia profesional. FLIR adquirió previamente **Lorex** y **DVTEL** (ahora FLIR Latitude VMS).

**Datos clave:**

- **Líneas de producto:** Quasar (multi-sensor/mini-domo premium), Saros (detección perimetral), Elara (doble sensor térmico+visible), CM (mini domo compacto), CF (fija compacta), PT/PTZ (giro-inclinación-zoom), FC (solo térmico), FLIR FX (consumo, descontinuada)
- **Soporte de protocolos:** RTSP, ONVIF (series Quasar, Saros, Elara), HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (la mayoría de modelos), admin / fliradmin (algunos modelos Quasar)
- **Soporte ONVIF:** Sí (series Quasar, Saros, Elara)
- **Códecs de video:** H.264, H.265 (serie Quasar), MJPEG
- **Especialización térmica:** Las cámaras térmicas FLIR producen datos radiométricos además de video de luz visible, con flujos RTSP separados para cada sensor

!!! warning "Las cámaras térmicas tienen flujos separados"
    Las cámaras de doble sensor FLIR (Elara, serie PT) proporcionan flujos RTSP separados para los canales visible y térmico. Típicamente `ch0` es el canal visible y `ch1` es el canal térmico.

## Patrones de URL RTSP

### Modelos Actuales (Quasar, Saros, Elara)

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Canal visible | `rtsp://IP:554/ch0` | Flujo visible primario |
| Canal térmico | `rtsp://IP:554/ch1` | Flujo térmico (modelos de doble sensor) |
| Visible (alt) | `rtsp://IP:554/vis` | Visible en serie PT |
| FOV amplio térmico | `rtsp://IP:554/wfov` | Serie PT, campo de visión amplio |
| Flujo con auth | `rtsp://IP:554/0/USERNAME:PASSWORD/main` | Con credenciales incrustadas |

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Tipo | Notas |
|-------------|----------|------|-------|
| Quasar CM-3308 | `rtsp://IP:554/ch0` | Mini domo | Multi-sensor compacto |
| Quasar CM-6208 | `rtsp://IP:554/ch0` | Mini domo | Multi-sensor compacto |
| Serie D (domo fijo) | `rtsp://IP:554/ch0` | Domo fijo | Flujo visible |
| Serie F (fija) | `rtsp://IP:554/ch0` | Fija | Flujo visible |
| Serie PT (PTZ-35x140) | `rtsp://IP:554/vis` | PTZ | Canal visible |
| Serie PT (PTZ-35x140) | `rtsp://IP:554/wfov` | PTZ | FOV amplio térmico |
| Elara (visible) | `rtsp://IP:554/ch0` | Térmico+visible | Canal visible |
| Elara (térmico) | `rtsp://IP:554/ch1` | Térmico+visible | Canal térmico |
| Serie FC (térmico) | `rtsp://IP:554/ch0` | Solo térmico | Flujo térmico |

### Cámaras Térmicas de Doble Sensor

Las cámaras de doble sensor FLIR proporcionan video visible y térmico en canales separados:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Elara visible | `rtsp://IP:554/ch0` | Sensor de luz visible |
| Elara térmico | `rtsp://IP:554/ch1` | Sensor térmico |
| PT visible | `rtsp://IP:554/vis` | Sensor de luz visible |
| PT térmico amplio | `rtsp://IP:554/wfov` | Sensor térmico FOV amplio |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara FLIR con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// FLIR Quasar mini-dome, visible stream
var uri = new Uri("rtsp://192.168.1.70:554/ch0");
var username = "admin";
var password = "admin";
```

Para cámaras de doble sensor, use `ch1` para acceder al flujo térmico. Para cámaras de la serie PT, use `/vis` para el canal visible o `/wfov` para el canal térmico de FOV amplio.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/jpg/image.jpg` | Algunos modelos |
| Captura (alt) | `http://IP/snapshot.jpg` | Ruta alternativa |

## Solución de Problemas

### Canales térmico vs visible

Las cámaras de doble sensor FLIR (Elara, serie PT) exponen flujos RTSP separados para cada sensor:

- `ch0` = canal de luz visible (la mayoría de modelos)
- `ch1` = canal térmico (la mayoría de modelos)
- `/vis` = canal visible (serie PT)
- `/wfov` = FOV amplio térmico (serie PT)

Si se conecta al canal equivocado, puede recibir imagen térmica cuando esperaba visible o viceversa. Consulte la documentación de su cámara para las asignaciones de canal.

### Las credenciales predeterminadas difieren según el modelo

- **La mayoría de cámaras FLIR:** admin / admin
- **Algunos modelos Quasar:** admin / fliradmin
- **Firmware actual Teledyne FLIR:** La contraseña puede necesitar configurarse durante la configuración inicial

Siempre cambie las credenciales predeterminadas antes de desplegar cámaras en una red de producción.

### Cambio de marca Teledyne FLIR

Teledyne Technologies adquirió FLIR Systems en 2021. Las versiones de firmware actuales pueden mostrar la marca Teledyne FLIR, y las cámaras más nuevas pueden enviarse con interfaces web y herramientas de configuración actualizadas. Los patrones de URL RTSP permanecen consistentes con las cámaras FLIR anteriores.

### Cámaras de consumo FLIR FX

La línea descontinuada de cámaras de consumo FLIR FX usaba acceso solo en la nube y no soporta streaming RTSP. Estas cámaras no pueden conectarse mediante URLs RTSP directas.

### Cámaras FLIR Lorex

FLIR adquirió Lorex, pero las cámaras Lorex usan sus propios patrones de URL RTSP (basados en firmware Dahua). No use patrones de URL de FLIR para cámaras Lorex. Consulte la página de [Lorex](lorex.md) para URLs específicas de Lorex.

### Disponibilidad de ONVIF

ONVIF es soportado en cámaras de generación actual (Quasar, Saros, Elara). Las cámaras FLIR antiguas y los modelos de consumo (FLIR FX) no soportan ONVIF. Para modelos con soporte ONVIF, use el descubrimiento ONVIF como alternativa a la configuración manual de URL RTSP.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras FLIR?**

Para la mayoría de cámaras FLIR, use `rtsp://admin:admin@IP_CAMARA:554/ch0` para el flujo visible. Para cámaras térmicas de doble sensor, use `ch1` para el flujo térmico. Para cámaras de la serie PT, use `/vis` (visible) o `/wfov` (FOV amplio térmico).

**¿FLIR soporta H.265?**

Las cámaras de la serie Quasar soportan codificación H.265. Otras líneas de cámaras FLIR usan principalmente H.264 y MJPEG. Consulte la hoja de datos de su modelo específico para soporte de códecs.

**¿Cómo accedo al flujo térmico en una cámara FLIR de doble sensor?**

Las cámaras de doble sensor proporcionan flujos RTSP separados para los canales visible y térmico. En modelos Elara, `ch0` es visible y `ch1` es térmico. En modelos de la serie PT, `/vis` es visible y `/wfov` es el flujo térmico de FOV amplio. Conéctese a la URL apropiada para el sensor deseado.

**¿FLIR y Teledyne FLIR son la misma empresa?**

Sí. Teledyne Technologies adquirió FLIR Systems en 2021. La empresa ahora opera como Teledyne FLIR. Las cámaras FLIR existentes continúan funcionando con los mismos patrones de URL RTSP. Los productos más nuevos pueden llevar la marca Teledyne FLIR.

**¿Puedo usar patrones de URL de FLIR para cámaras Lorex?**

No. Aunque FLIR adquirió Lorex, las cámaras Lorex usan firmware basado en Dahua con diferentes patrones de URL RTSP. Consulte la [guía de conexión de cámaras Lorex](lorex.md) para las URLs correctas.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Basler](basler.md) — Cámaras industriales / visión artificial
- [Guía de Conexión de Mobotix](mobotix.md) — Cámaras industriales alemanas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
