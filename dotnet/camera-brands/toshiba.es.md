---
title: Toshiba IK-W - URLs RTSP y conexión de cámara en C# .NET
description: Conecte cámaras Toshiba de la serie IK-W en C# .NET con patrones de URL RTSP y ejemplos de código para modelos IK-WB, IK-WD, IK-WR e IK-WP.
---

# Cómo conectar una cámara IP Toshiba en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Toshiba** (Toshiba Corporation) es un conglomerado multinacional japonés con sede en Tokio, Japón. La división de seguridad de Toshiba produjo la **serie IK-W** de cámaras IP, cubriendo formatos de caja, domo, bala y PTZ. Desde entonces, Toshiba ha salido del mercado de cámaras de seguridad independientes y vendió su negocio de vigilancia. A pesar de la descontinuación, muchas cámaras de la serie IK-W permanecen desplegadas en instalaciones comerciales e industriales en todo el mundo.

**Datos clave:**

- **Líneas de productos:** IK-WB (cámaras de caja), IK-WD (cámaras domo), IK-WR (cámaras bala/robustas), IK-WP (cámaras PTZ)
- **Soporte de protocolos:** RTSP, HTTP/CGI, ONVIF (limitado, solo modelos más nuevos)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / 1234
- **Soporte ONVIF:** Limitado (solo modelos más nuevos IK-W14/16/30/70/80)
- **Códecs de video:** H.264 (series IK-W14/16/30/70/80), MJPEG (modelos más antiguos)

!!! warning "Línea de Productos Descontinuada"
    Toshiba ha salido del mercado de cámaras IP y vendió su negocio de vigilancia. No hay nuevas actualizaciones de firmware ni soporte oficial disponible. Muchos modelos tempranos IK-WB (01A, 02A, 11A) solo soportan capturas HTTP y no proporcionan transmisión RTSP.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Toshiba de la serie IK-W usan el patrón de URL `live.sdp`:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live.sdp
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `live.sdp` | Flujo principal | Flujo H.264 principal (resolución más alta) |
| `live2.sdp` | Subflujo | Flujo secundario (resolución más baja) |
| `live3.sdp` | Tercer flujo | Tercer flujo (optimizado para móvil, modelos selectos) |

### Modelos de Cámaras - Flujos RTSP

| Modelo | Tipo | URL de Flujo Principal | URL de Subflujo | Notas |
|--------|------|------------------------|-----------------|-------|
| IK-WB16A | Caja | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WB80A | Caja | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WD01A | Domo | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WD12A | Domo | `rtsp://IP:554//live.sdp` | -- | Ruta con doble barra |
| IK-WD14A | Domo | `rtsp://IP:554/live.sdp` | `rtsp://IP:554/live2.sdp` | También soporta `live3.sdp` |
| IK-WR04A | Bala | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WR12A | Bala | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WR14A | Bala | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WP41A | PTZ | `rtsp://IP:554/live.sdp` | -- | H.264 |

### Modelos por Serie

#### Serie IK-WB (Cámaras de Caja)

| Modelo | Transmisión | Protocolo |
|--------|-------------|-----------|
| IK-WB01A | Solo captura HTTP | HTTP |
| IK-WB02A | Solo captura HTTP | HTTP |
| IK-WB11A | Solo captura HTTP | HTTP |
| IK-WB15A | Captura HTTP + CGI | HTTP |
| IK-WB16A | RTSP `live.sdp` | RTSP + HTTP |
| IK-WB16A-W | RTSP `live.sdp`, `live3.sdp` | RTSP + HTTP |
| IK-WB21A | Solo HTTP CGI | HTTP |
| IK-WB30A | RTSP `live.sdp` | RTSP + HTTP |
| IK-WB70A | RTSP `live.sdp` | RTSP + HTTP + MJPEG |
| IK-WB80A | RTSP `live.sdp` | RTSP + HTTP + MJPEG |

#### Serie IK-WD (Cámaras Domo)

| Modelo | Transmisión | Protocolo |
|--------|-------------|-----------|
| IK-WD01A | RTSP `live.sdp` | RTSP |
| IK-WD12A | RTSP `//live.sdp` | RTSP (doble barra) |
| IK-WD14A | RTSP `live.sdp`, `live2.sdp`, `live3.sdp` | RTSP (multi-flujo) |

#### Serie IK-WR (Cámaras Bala/Robustas)

| Modelo | Transmisión | Protocolo |
|--------|-------------|-----------|
| IK-WR01A | Solo captura HTTP | HTTP |
| IK-WR02A | Solo captura HTTP | HTTP |
| IK-WR04A | RTSP `live.sdp` | RTSP |
| IK-WR12A | RTSP `live.sdp` | RTSP + MJPEG |
| IK-WR14A | RTSP `live.sdp` | RTSP + HTTP |

#### Serie IK-WP (Cámaras PTZ)

| Modelo | Transmisión | Protocolo |
|--------|-------------|-----------|
| IK-WP41A | RTSP `live.sdp` | RTSP |

### Formatos de URL Alternativos

Algunos modelos Toshiba usan una doble barra en la ruta RTSP:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/live.sdp` | Estándar (recomendado) |
| `rtsp://IP:554//live.sdp` | Variante con doble barra (IK-WD12A, algunas unidades IK-WD14A) |
| `rtsp://IP:554/live2.sdp` | Subflujo (IK-WD14A) |
| `rtsp://IP:554/live3.sdp` | Tercer flujo (IK-WB16A-W, IK-WD14A) |

!!! tip "Ruta con Doble Barra"
    Si `rtsp://IP:554/live.sdp` no funciona en su cámara Toshiba, pruebe la variante con doble barra `rtsp://IP:554//live.sdp`. Algunos modelos IK-WD requieren este formato.

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Toshiba con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Toshiba IK-WD14A, main stream
var uri = new Uri("rtsp://192.168.1.90:554/live.sdp");
var username = "admin";
var password = "1234";
```

Para acceder al subflujo en modelos compatibles, use `live2.sdp` en lugar de `live.sdp`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Modelos Compatibles | Notas |
|------|---------------|---------------------|-------|
| Captura JPEG | `http://IP/__live.jpg?&&&` | IK-WB01A, WB11A, WB15A, WB16A-W, WB21A | Note el prefijo de guión bajo |
| Captura CGI | `http://IP/GetData.cgi` | IK-WB01A, WB11A, WB15A, WB21A, WR01A | Captura básica |
| Captura Configurable | `http://IP/GetData.cgi?CH=CHANNEL&Codec=jpeg&Size=WIDTHxHEIGHT` | Serie IK-WB | Establecer resolución y canal |
| Captura por Resolución | `http://IP/cgi-bin/viewer/video.jpg?resolution=WIDTHxHEIGHT` | IK-WB15A, WB16A, WB30A, WB70A, WR12A, WR14A | Especificar resolución de salida |
| Captura Simple | `http://IP/Jpeg/CamImg.jpg` | IK-WB02A, WR01A | Captura JPEG básica |
| Flujo MJPEG | `http://IP/video.mjpg` | IK-WB70A, WB80A, WR12A | Flujo MJPEG continuo |

!!! note "Modelos Solo HTTP"
    Los modelos Toshiba más antiguos (IK-WB01A, WB02A, WB11A, WR01A, WR02A) no soportan RTSP. Para estas cámaras, use URLs de capturas HTTP o flujos MJPEG. Puede capturar estos mediante los modos de fuente HTTP o fuente MJPEG del VisioForge SDK.

## Solución de Problemas

### La cámara es solo HTTP (sin RTSP)

Muchos modelos tempranos IK-WB (01A, 02A, 11A) y modelos IK-WR (01A, 02A) no soportan transmisión RTSP en absoluto. Estas cámaras solo proporcionan capturas HTTP y endpoints CGI. Si su cámara no responde en el puerto 554, verifique si es un modelo solo HTTP consultando las tablas anteriores.

### Prefijo de guión bajo en URL de captura

La URL de captura `__live.jpg` usa un **prefijo de doble guión bajo**, lo cual es inusual. Asegúrese de incluir ambos guiones bajos:

```
http://192.168.1.90/__live.jpg?&&&
```

Los caracteres `&&&` finales también son requeridos en algunas versiones de firmware.

### Doble barra en la ruta RTSP

Algunas cámaras de la serie IK-WD (WD12A, ciertas unidades WD14A) requieren una doble barra diagonal en la ruta RTSP:

```
rtsp://admin:1234@192.168.1.90:554//live.sdp
```

Si la URL estándar con una sola barra no conecta, pruebe esta variante.

### No hay actualizaciones de firmware disponibles

Toshiba ha salido del mercado de cámaras de seguridad. No hay nuevo firmware, parches ni canales de soporte oficial disponibles. Si encuentra errores o vulnerabilidades de seguridad, considere reemplazar la cámara con un modelo actualmente soportado.

### Las credenciales predeterminadas no funcionan

Las credenciales predeterminadas de fábrica son **admin / 1234**. Si estas no funcionan, la contraseña puede haber sido cambiada por un administrador anterior. Un restablecimiento de fábrica por hardware (generalmente un botón de reinicio con orificio) restaurará los valores predeterminados en la mayoría de los modelos.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras IP Toshiba?**

La URL RTSP principal es `rtsp://admin:1234@CAMERA_IP:554/live.sdp` para modelos que soportan transmisión RTSP. Use `live2.sdp` para el subflujo en modelos como el IK-WD14A. Note que los modelos más antiguos IK-WB01A/02A/11A e IK-WR01A/02A no soportan RTSP en absoluto.

**¿Las cámaras IP Toshiba siguen teniendo soporte?**

No. Toshiba vendió su negocio de vigilancia y salió del mercado de cámaras IP. No hay actualizaciones de firmware, nuevos modelos ni soporte técnico oficial disponible. Las cámaras existentes continúan funcionando pero no recibirán parches de seguridad ni actualizaciones de funciones.

**¿Las cámaras Toshiba soportan ONVIF?**

Solo los modelos más nuevos en el rango IK-W14/16/30/70/80 tienen soporte ONVIF limitado. Los modelos más antiguos (IK-WB01A hasta WB11A, IK-WR01A/02A) no soportan ONVIF. Para descubrimiento y configuración ONVIF, use solo los modelos compatibles.

**¿Por qué mi cámara Toshiba solo proporciona capturas, no flujos de video?**

Los modelos tempranos IK-W de Toshiba fueron diseñados como cámaras de captura en red y no incluyen un servidor RTSP. Estos modelos (IK-WB01A, WB02A, WB11A, WR01A, WR02A) solo soportan capturas JPEG basadas en HTTP y endpoints CGI. Para obtener video continuo, necesita un modelo más nuevo de la serie IK-W14/16/30/70/80.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Sony](sony.md) — Cámaras empresariales japonesas
- [Guía de Conexión de JVC](jvc.md) — Marca japonesa de vigilancia legacy
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
