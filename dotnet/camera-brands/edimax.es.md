---
title: Cómo Conectar una Cámara IP Edimax en C# .NET
description: Conecte cámaras Edimax en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series IC, IR, PT y VS.
---

# Cómo Conectar una Cámara IP Edimax en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Edimax** (Edimax Technology Co.) es un fabricante taiwanés de equipos de red con sede en Taipéi, Taiwán. Fundada en 1986, Edimax es conocida principalmente por productos de red como routers, switches y adaptadores Wi-Fi, pero también fabrica una gama de cámaras IP para consumidores y pequeñas y medianas empresas bajo la serie IC. A lo largo de los años, las cámaras Edimax han evolucionado a través de varias generaciones de formato de URL.

**Datos clave:**

- **Líneas de productos:** IC (cámara IP), IR (infrarrojo), PT (panorámica/inclinación), VS (servidor de vídeo)
- **Múltiples generaciones de formato de URL:** los modelos más antiguos usan `/ipcam.sdp`, los más nuevos usan `/stream1`
- **Puerto RTSP predeterminado:** 554 (algunos modelos usan 8000)
- **Credenciales predeterminadas:** admin / 1234
- **Soporte ONVIF:** Sí (modelos más recientes)
- **Códecs de vídeo:** H.264, MJPEG
- **URL RTSP principal:** `rtsp://IP:554/ipcam_h264.sdp`

!!! note "Generaciones de formato de URL"
    Las cámaras Edimax han evolucionado a través de varias generaciones de formato de URL. Las series más antiguas IC-1500/IC-3000 usan `/ipcam.sdp` o `/ipcam_h264.sdp`, mientras que las series más nuevas IR/PT usan `/stream1`. Pruebe ambos formatos si uno no funciona.

!!! warning "Doble estilo de autenticación"
    Las cámaras Edimax utilizan dos estilos diferentes de parámetros de autenticación en URLs HTTP: `account=USER&password=PASS` (firmware antiguo) y `user=USER&pwd=PASS` (firmware nuevo). Verifique qué formato soporta su cámara.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Edimax utilizan principalmente una URL RTSP basada en SDP:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/ipcam_h264.sdp
```

| Patrón de URL | Descripción |
|---------------|-------------|
| `/ipcam.sdp` | Flujo basado en SDP (series IC antiguas) |
| `/ipcam_h264.sdp` | Flujo SDP H.264 (recomendado para la mayoría de modelos) |
| `/stream1` | Flujo primario (series IR/PT más nuevas) |
| `/stream2` | Flujo secundario (series IR/PT más nuevas) |
| `/live1.sdp` | Flujo SDP en vivo (series PT) |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|----------------------|-------|
| IC-1500WG (VGA inalámbrica) | VGA | `rtsp://IP:554/ipcam.sdp` | Formato SDP antiguo |
| IC-3010 (HD) | HD | `rtsp://IP:554/ipcam.sdp` | Formato SDP antiguo |
| IC-3015WN (HD inalámbrica) | HD Inalámbrica | `rtsp://IP:554/ipcam.sdp` | Wi-Fi, formato SDP |
| IC-3030WN (HD inalámbrica) | HD Inalámbrica | `rtsp://IP:554/ipcam.sdp` | Wi-Fi, formato SDP |
| IC-3030IWN (HD inalámbrica) | HD Inalámbrica | `rtsp://IP:554/ipcam.sdp` | Wi-Fi interior |
| IC-3100W (HD inalámbrica) | HD Inalámbrica | `rtsp://IP:554/ipcam_h264.sdp` | Formato SDP H.264 |
| IC-3110W (HD inalámbrica) | HD Inalámbrica | `rtsp://IP:554/ipcam_h264.sdp` | Formato SDP H.264 |
| IC-3116W (HD inalámbrica) | HD Inalámbrica | `rtsp://IP:554/ipcam_h264.sdp` | Formato SDP H.264 |
| IC-7000 (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam.sdp` | Panorámica/inclinación/zoom |
| IC-7010PTN (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam.sdp` | PTZ en red |
| IC-7100 (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PTZ H.264 |
| IC-7110P (HD PTZ PoE) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PTZ PoE |
| IC-7110W (HD PTZ inalámbrica) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PTZ Wi-Fi |
| IC-9000 (exterior) | Exterior | `rtsp://IP:554/CHANNEL/USERNAME:PASSWORD/main` | Credenciales en URL |
| IR-112E (infrarrojo) | Infrarrojo | `rtsp://IP:554//stream2` | Formato de flujo nuevo |
| IR-113E (infrarrojo) | Infrarrojo | `rtsp://IP:554//stream1` | Formato de flujo nuevo |
| PT-112E (panorámica/inclinación) | Panorámica/Inclinación | `rtsp://IP:554/live1.sdp` | Formato SDP en vivo |
| PT-31E (panorámica/inclinación) | Panorámica/Inclinación | `rtsp://IP:8000//stream1` | Puerto 8000 |
| VS100 (servidor de vídeo) | Servidor de Vídeo | `rtsp://IP:554//stream1` | Codificador/servidor |

### Formatos de URL Alternativos

Algunos modelos y versiones de firmware de Edimax soportan estas URLs RTSP adicionales:

| Patrón de URL | Modelos Soportados | Notas |
|---------------|-------------------|-------|
| `rtsp://IP:554/ipcam.sdp` | IC-1500, IC-3010, IC-3015WN, IC-3030WN, IC-7000, IC-7010PTN | Formato SDP antiguo |
| `rtsp://IP:554/ipcam_h264.sdp` | IC-3100W, IC-3110W, IC-3116W, IC-7100, IC-7110P, IC-7110W | SDP H.264 (recomendado) |
| `rtsp://IP:554//stream1` | IR-113E, VS100 | Formato de flujo nuevo (note la doble barra) |
| `rtsp://IP:554//stream2` | IR-112E | Subflujo (doble barra) |
| `rtsp://IP:554/stream1` | Modelos más nuevos selectos | Flujo sin doble barra |
| `rtsp://IP:554/live1.sdp` | PT-112E | SDP en vivo para panorámica/inclinación |
| `rtsp://IP:8000//stream1` | PT-31E | Puerto no estándar 8000 |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Edimax con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Edimax IC-3116W, H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/ipcam_h264.sdp");
var username = "admin";
var password = "1234";
```

Para modelos más nuevos de las series IR/PT, use `/stream1` o `//stream1` en lugar de `/ipcam_h264.sdp`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/snapshot.jpg` | Captura básica, puede requerir autenticación |
| Captura (auth cuenta) | `http://IP/snapshot.jpg?account=USER&password=PASS` | Estilo de autenticación de firmware antiguo |
| Captura (auth usuario) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Estilo de autenticación de firmware nuevo |
| Imagen JPEG | `http://IP/jpg/image.jpg` | JPEG directo |
| JPEG de Canal | `http://IP/jpg/1/image.jpg` | JPEG específico por canal |
| Flujo MJPEG | `http://IP/mjpg/video.mjpg` | Flujo MJPEG continuo |
| MJPEG de Canal | `http://IP/mjpg/1/video.mjpg` | MJPEG específico por canal |
| Captura CGI | `http://IP/snapshot.cgi` | Captura basada en CGI |
| MJPEG CGI | `http://IP/cgi/mjpg/mjpeg.cgi` | Flujo MJPEG CGI |
| CGI de Flujo | `http://IP/cgi-bin/Stream?Video` | Flujo de vídeo alternativo |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras Edimax vienen con credenciales predeterminadas de **admin / 1234**. Si la autenticación falla:

1. Acceda a la cámara en `http://CAMERA_IP` desde un navegador
2. Inicie sesión con las credenciales predeterminadas o las que haya configurado
3. Navegue a **Configuration > Security** para verificar o restablecer credenciales
4. Use esas credenciales en su URL RTSP

### El formato de URL no funciona

Edimax ha utilizado varios formatos de URL RTSP diferentes a lo largo de las generaciones de modelos. Si su URL no conecta:

1. Pruebe `/ipcam_h264.sdp` primero (funciona con la mayoría de modelos de generación intermedia)
2. Pruebe `/ipcam.sdp` para las series más antiguas IC-1500 e IC-3000
3. Pruebe `//stream1` (con doble barra) para las series más nuevas IR y PT
4. Pruebe `/stream1` (barra simple) si la doble barra falla
5. Verifique si su modelo usa el puerto 8000 en lugar de 554 (por ejemplo, PT-31E)

### URLs con doble barra

Algunos modelos Edimax más nuevos usan una doble barra en la ruta RTSP (por ejemplo, `rtsp://IP:554//stream1`). Esto es intencional y no es un error tipográfico. Si `/stream1` no funciona, pruebe `//stream1`.

### Puerto 554 vs puerto 8000

La mayoría de las cámaras Edimax usan el puerto RTSP estándar 554, pero algunos modelos (como el PT-31E) usan el puerto 8000. Verifique la interfaz web de su cámara en **Configuration > Network > RTSP** para la configuración correcta del puerto.

### Estilo de autenticación de capturas

Si las URLs de captura devuelven errores 401 incluso con credenciales correctas, intente cambiar entre los dos estilos de parámetros de autenticación:

- Firmware antiguo: `?account=USER&password=PASS`
- Firmware nuevo: `?user=USER&pwd=PASS`

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Edimax?**

Para la mayoría de las cámaras Edimax serie IC, la URL es `rtsp://admin:1234@CAMERA_IP:554/ipcam_h264.sdp`. Para las series más nuevas IR y PT, use `rtsp://admin:1234@CAMERA_IP:554//stream1`. El formato exacto depende del modelo y la versión de firmware.

**¿Las cámaras Edimax soportan ONVIF?**

Los modelos Edimax más nuevos soportan ONVIF. Los modelos más antiguos de las series IC-1500 e IC-3000 pueden no tener soporte ONVIF. Verifique las especificaciones de su cámara o la interfaz web para la configuración ONVIF.

**¿Por qué mi cámara Edimax usa una doble barra en la URL?**

Algunos modelos Edimax más nuevos (series IR y PT) usan un formato de ruta con doble barra como `//stream1`. Este es el formato correcto para esos modelos y no es un error tipográfico. Tanto `//stream1` como `/stream1` pueden funcionar dependiendo de la versión de firmware.

**¿Cuál es la diferencia entre ipcam.sdp e ipcam_h264.sdp?**

`/ipcam.sdp` es el flujo SDP genérico usado por modelos más antiguos y puede entregar MJPEG o H.264 dependiendo de la configuración de la cámara. `/ipcam_h264.sdp` solicita explícitamente el flujo codificado en H.264 y se recomienda para mejor compresión y calidad.

**¿Puedo usar ambos estilos de autenticación de capturas?**

No. Cada versión de firmware de cámara soporta solo un estilo de autenticación para URLs HTTP. El firmware antiguo usa `account=USER&password=PASS` mientras que el firmware nuevo usa `user=USER&pwd=PASS`. Pruebe ambos para determinar cuál espera su cámara.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Zavio](zavio.md) — Cámaras taiwanesas para PyMEs
- [Guía de Transmisión de Vídeo RTSP](../general/network-streaming/rtsp.md) — Transmisión de red RTSP con Edimax
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
