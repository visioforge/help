---
title: Cámaras IP LG - URLs RTSP, modelos y conexión en C# .NET
description: Conecte cámaras LG SmartIP y de la serie LW/LV en C# .NET con patrones de URL RTSP y ejemplos de código para modelos inalámbricos, cableados y empresariales.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Webcam
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# Cómo conectar una cámara IP LG en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**LG Electronics** es una empresa multinacional surcoreana de electrónica con sede en Seúl, Corea del Sur. LG produjo cámaras IP bajo la marca **SmartIP** y la **serie LW/LV** para el mercado de seguridad profesional. Desde entonces, LG ha salido en gran parte del negocio de cámaras IP y vendió su división de seguridad. Un número limitado de cámaras LG permanecen desplegadas en instalaciones comerciales y empresariales.

**Datos clave:**

- **Líneas de productos:** Serie LW (domo/bala inalámbrica), Serie LV (cableada), SmartIP (empresarial)
- **Soporte de protocolos:** RTSP, HTTP/CGI, ONVIF (serie SmartIP), PSIA (modelos selectos)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (serie SmartIP), limitado o ausente en series LW/LV
- **Códecs de video:** H.264 (LW130W, LW332, serie SmartIP), MJPEG (modelos más antiguos)

!!! warning "Línea de Productos Descontinuada"
    LG ha salido del mercado de cámaras IP y vendió su división de seguridad. No hay nuevas actualizaciones de firmware ni soporte oficial disponible. Muchas entradas de base de datos etiquetadas como "LG" son en realidad teléfonos inteligentes LG usados como cámaras IP mediante apps de terceros -- solo los modelos reales de cámaras LG (LW, LV, SmartIP, 7210R) se cubren aquí.

## Patrones de URL RTSP

### Formatos de URL Estándar

Las cámaras LG usan varios patrones de URL RTSP diferentes dependiendo de la serie del modelo:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video1+audio1
```

| Patrón de URL | Descripción |
|---------------|-------------|
| `video1+audio1` | Video H.264 con audio (serie LW, 7210R) |
| `/` (raíz) | Flujo raíz (serie LV) |
| `//Master-0` | Flujo maestro (alternativa LW130W) |
| `camera.stm` | Flujo de cámara (LW332) |
| `live1.sdp` | Flujo SDP en vivo (alternativa LW332) |
| URL de canal PSIA | Transmisión empresarial PSIA (SmartIP) |

### Modelos de Cámaras - Flujos RTSP

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|------------------------|-------|
| LW130W | Domo Inalámbrico | `rtsp://IP:554/video1+audio1` | H.264 + audio |
| LW130W | Domo Inalámbrico | `rtsp://IP//Master-0` | Flujo Master alternativo |
| LW332 | Bala Inalámbrica | `rtsp://IP:554/camera.stm` | Flujo de cámara |
| LW332 | Bala Inalámbrica | `rtsp://IP:554/live1.sdp` | Flujo SDP alternativo |
| LVW700 | Domo Cableado | `rtsp://IP:554/` | Flujo raíz |
| LVW701 | Domo Cableado | `rtsp://IP:554/` | Flujo raíz |
| 7210R | Cámara IP | `rtsp://IP:554/video1+audio1` | H.264 + audio |
| SmartIP | Empresarial | `rtsp://IP:554/PSIA/Streaming/channels/2?videoCodecType=H.264` | Flujo PSIA H.264 |

### Modelos por Serie

#### Serie LW (Cámaras Inalámbricas)

| Modelo | URLs de Transmisión | Protocolo |
|--------|---------------------|-----------|
| LW130W | `video1+audio1` o `//Master-0` | RTSP + HTTP |
| LW332 | `camera.stm` o `live1.sdp` | RTSP + HTTP |

#### Serie LV (Cámaras Cableadas)

| Modelo | URLs de Transmisión | Protocolo |
|--------|---------------------|-----------|
| LVW700 | Flujo raíz (`rtsp://IP:554/`) | RTSP |
| LVW701 | Flujo raíz (`rtsp://IP:554/`) | RTSP |

#### Serie SmartIP (Cámaras Empresariales)

| Modelo | URLs de Transmisión | Protocolo |
|--------|---------------------|-----------|
| Modelos SmartIP | URL de canal PSIA | RTSP + PSIA + ONVIF |

#### Modelos Independientes

| Modelo | URLs de Transmisión | Protocolo |
|--------|---------------------|-----------|
| 7210R | `video1+audio1` | RTSP |

### Formatos de URL Alternativos

| Patrón de URL | Modelos | Notas |
|---------------|---------|-------|
| `rtsp://IP:554/video1+audio1` | LW130W, 7210R | Estándar (recomendado para estos modelos) |
| `rtsp://IP//Master-0` | LW130W | Alternativa; note doble barra, sin puerto |
| `rtsp://IP:554/camera.stm` | LW332 | Estándar para LW332 |
| `rtsp://IP:554/live1.sdp` | LW332 | Formato SDP alternativo |
| `rtsp://IP:554/` | LVW700, LVW701 | Flujo raíz (inusual pero válido) |
| `rtsp://IP:554/PSIA/Streaming/channels/2?videoCodecType=H.264` | SmartIP | Transmisión empresarial PSIA |

!!! tip "URL de Flujo Raíz"
    Los modelos LVW700 y LVW701 usan una URL RTSP raíz (`rtsp://IP:554/`) sin componente de ruta. Esto es inusual pero válido. Asegúrese de que su cliente RTSP no elimine la barra final ni agregue una ruta predeterminada.

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara LG con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// LG LW130W, H.264 + audio stream
var uri = new Uri("rtsp://192.168.1.90:554/video1+audio1");
var username = "admin";
var password = "admin";
```

Para cámaras LW332, use `camera.stm` o `live1.sdp` como la ruta del flujo en su lugar.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura CGI | `http://IP/snapshot.cgi` | Captura CGI estándar |
| Captura JPEG | `http://IP/snapshot.jpg` | JPEG directo (LW130W) |
| Feed de Video | `http://IP/videofeed` | Feed de video en vivo |
| Flujo MJPEG | `http://IP/video?submenu=mjpg` | Flujo MJPEG continuo |
| Video por Perfil | `http://IP/video?profile=CHANNEL` | Selección de video basada en perfil |

## Solución de Problemas

### Confusión entre cámaras LG y teléfonos LG

Muchas bases de datos de cámaras RTSP contienen entradas etiquetadas como "LG" que en realidad son **teléfonos inteligentes LG** (P350, P509, P970, Nexus 4, Optimus V, LS670) ejecutando apps de cámara IP de terceros como "IP Webcam". Estos no son cámaras IP LG reales. Busque números de modelo que comiencen con **LW**, **LV**, **SmartIP** o **7210R** para identificar cámaras de seguridad LG genuinas.

### La URL de flujo raíz no conecta

Las cámaras LVW700 y LVW701 usan una URL raíz sin ruta (`rtsp://IP:554/`). Algunas bibliotecas de clientes RTSP pueden no manejar esto correctamente. Si experimenta problemas de conexión:

1. Asegúrese de que la barra final esté incluida
2. Intente especificar la URL como `rtsp://admin:admin@192.168.1.90:554/`
3. Verifique que la cámara esté respondiendo en el puerto 554 usando un escáner de red

### Múltiples formatos de URL por modelo

Algunas cámaras LG (particularmente LW130W y LW332) soportan múltiples formatos de URL RTSP. Si un formato falla, pruebe el alternativo:

- **LW130W:** Pruebe `video1+audio1` primero, luego `//Master-0`
- **LW332:** Pruebe `camera.stm` primero, luego `live1.sdp`

### Transmisión PSIA en modelos SmartIP

Las cámaras empresariales SmartIP soportan transmisión PSIA (Physical Security Interoperability Alliance). El formato de URL PSIA es:

```
rtsp://admin:admin@192.168.1.90:554/PSIA/Streaming/channels/2?videoCodecType=H.264
```

Cambie el número de canal para seleccionar diferentes flujos. PSIA requiere autenticación vía la URL o HTTP digest.

### No hay actualizaciones de firmware disponibles

LG ha salido del mercado de cámaras de seguridad. No hay nuevo firmware, parches ni canales de soporte oficial disponibles. Si encuentra errores o vulnerabilidades de seguridad, considere reemplazar la cámara con un modelo actualmente soportado.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras IP LG?**

Depende de la serie del modelo. Para cámaras LW130W y 7210R, use `rtsp://admin:admin@CAMERA_IP:554/video1+audio1`. Para LW332, use `rtsp://admin:admin@CAMERA_IP:554/camera.stm`. Para LVW700/LVW701, use `rtsp://admin:admin@CAMERA_IP:554/`. Cada serie de modelos tiene un patrón de URL diferente.

**¿Las cámaras IP LG siguen teniendo soporte?**

No. LG vendió su división de seguridad y salió del mercado de cámaras IP. No hay actualizaciones de firmware, nuevos modelos ni soporte técnico oficial disponible. Las cámaras existentes continúan funcionando pero no recibirán parches de seguridad ni actualizaciones de funciones.

**¿Las cámaras LG soportan ONVIF?**

Solo la serie empresarial SmartIP soporta ONVIF. Las cámaras de consumo de las series LW y LV tienen soporte ONVIF limitado o nulo. Las cámaras SmartIP también soportan PSIA como protocolo de interoperabilidad alternativo.

**¿Por qué veo modelos de teléfonos LG en bases de datos de cámaras IP?**

Muchas bases de datos de URLs RTSP listan modelos de teléfonos inteligentes LG (Nexus 4, Optimus V, P509, etc.) como "cámaras LG". Estos son en realidad teléfonos ejecutando apps de terceros como "IP Webcam" que convierten el teléfono en una cámara de seguridad improvisada. No son productos reales de cámaras IP LG y usan patrones de URL completamente diferentes determinados por la app.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Samsung](samsung.md) — Cámaras empresariales coreanas
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
