---
title: Dahua RTSP en C# .NET - URLs, ONVIF y Código de Ejemplo
description: Conecta cámaras Dahua en C# .NET con patrones de URL RTSP, soporte ONVIF y ejemplos de código para modelos IPC-HDW, IPC-HFW, NVR y DVR.
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
  - IP Camera
  - RTSP
  - ONVIF
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Dahua en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Dahua Technology** (Zhejiang Dahua Technology Co., Ltd.) es el segundo mayor fabricante de videovigilancia del mundo. Fundada en 2001 y con sede en Hangzhou, China, Dahua produce cámaras IP, NVRs, DVRs, sistemas de control de acceso e intercomunicadores de video. Las cámaras Dahua también se venden ampliamente bajo marcas OEM incluyendo Amcrest, Lorex y otras.

**Datos clave:**

- **Líneas de producto:** IPC-HDW (domo), IPC-HFW (bala), IPC-HDBW (domo antivandálico), SD (PTZ), NVR4xxx/5xxx (NVRs), XVR (DVRs)
- **Soporte de protocolos:** ONVIF Profile S/G/T, RTSP, HTTP, Dahua propietario (DHIP)
- **Puerto RTSP predeterminado:** 554 (algunos modelos usan 1554)
- **Credenciales predeterminadas:** admin / admin (firmware antiguo); admin / (establecido durante configuración en firmware más reciente)
- **Soporte ONVIF:** Completo
- **Códecs de video:** H.264, H.265, H.265+, MJPEG

## Patrones de URL RTSP

Las cámaras Dahua utilizan una estructura de URL `cam/realmonitor` con parámetros de canal y subtipo.

### Formato de URL

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:[PUERTO]/cam/realmonitor?channel=[CH]&subtype=[ST]
```

**Parámetros:**

- `channel` = número de canal de la cámara (1 para cámaras de un solo canal, 1-N para NVR/DVR)
- `subtype` = tipo de flujo: 0 = flujo principal, 1 = subflujo, 2 = tercer flujo

### Cámaras IP (Canal Único)

| Serie de Modelo | URL RTSP | Flujo | Audio |
|-------------|----------|--------|-------|
| IPC-HDW (domo) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Sí |
| IPC-HDW (domo) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sub | Sí |
| IPC-HFW (bala) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Sí |
| IPC-HDBW (domo antivandálico) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Sí |
| SD (PTZ) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Sí |
| DH-IPC-HF2100P | `rtsp://IP:1554/cam/realmonitor?channel=1&subtype=0` | Principal | Sí |

### Formato de URL Simplificado

Muchas cámaras Dahua también aceptan un formato de URL más corto:

| Patrón de URL | Flujo | Notas |
|-------------|--------|-------|
| `rtsp://IP:554/cam/realmonitor` | Principal (ch1) | Por defecto canal 1, flujo principal |
| `rtsp://IP:554/` | Principal | URL básica, solo algunos modelos |
| `rtsp://IP:554/live` | Principal | Formato legacy |

### Canales NVR / DVR

| Dispositivo | Canal | URL RTSP | Flujo |
|--------|---------|----------|--------|
| NVR Cámara 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal |
| NVR Cámara 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sub |
| NVR Cámara 2 | 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | Principal |
| NVR Cámara 4 | 4 | `rtsp://IP:554/cam/realmonitor?channel=4&subtype=0` | Principal |
| DVR Canal 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=01` | Sub |

### Amcrest / Lorex (OEM de Dahua)

Las cámaras Amcrest y Lorex usan el mismo formato de URL RTSP que Dahua:

| Marca | URL RTSP | Notas |
|-------|----------|-------|
| Amcrest | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Idéntico a Dahua |
| Lorex | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Idéntico a Dahua |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Dahua con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Dahua IPC-HDW series, main stream
var uri = new Uri("rtsp://192.168.1.108:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, usa `subtype=1` en su lugar.

### Descubrimiento ONVIF

Las cámaras Dahua proporcionan un sólido soporte ONVIF. Consulta la [guía de integración ONVIF](../mediablocks/Sources/index.md) para ejemplos de código de descubrimiento.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requiere autenticación básica |
| Captura JPEG (legacy) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Autenticación por URL |
| Flujo MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1` | MJPEG continuo |
| MJPEG compatible Axis | `http://IP/axis-cgi/mjpg/video.cgi?camera=1` | API Axis emulada |
| Captura CGI | `http://IP/cgi-bin/video.jpg` | Captura simple |
| Imagen CGI | `http://IP/cgi-bin/jpg/image.cgi` | Captura alternativa |

## Solución de Problemas

### Puerto 554 vs 1554

Algunos modelos Dahua (especialmente la serie DH-IPC-HF) usan el puerto **1554** en lugar del estándar 554. Si la conexión falla en el puerto 554, prueba con 1554.

### Métodos de autenticación

- Dahua soporta autenticación RTSP tanto **básica** como **digest**
- El firmware más reciente usa autenticación digest por defecto
- El VisioForge SDK maneja ambos métodos automáticamente
- Si usas URLs HTTP de captura, algunas requieren credenciales embebidas en la URL (parámetros `loginuse`/`loginpas`) mientras que el firmware más reciente usa autenticación HTTP básica/digest estándar

### Desconexiones

- Las cámaras Dahua pueden ser sensibles a la congestión de red. Usa transporte TCP para mayor fiabilidad.
- Reduce la resolución del flujo principal o cambia al subflujo (`subtype=1`) para reducir el ancho de banda
- Verifica la configuración de **Conexiones Máximas de Usuario** de la cámara (Configuración > Red > Conexión) -- el valor predeterminado es típicamente 10

### Las cámaras Amcrest/Lorex no se conectan

Si tienes una cámara Amcrest o Lorex (OEM de Dahua), usa exactamente los mismos patrones de URL RTSP listados arriba. Los puertos y rutas predeterminados son idénticos a Dahua. La única diferencia puede estar en las credenciales predeterminadas:

- **Predeterminado Amcrest:** admin / admin
- **Predeterminado Lorex:** admin / (establecido durante configuración)

### Formato de flujo extra del DVR

Al conectarte a canales del DVR, nota que `subtype=00` y `subtype=0` son equivalentes para el flujo principal. Algunos firmware más antiguos requieren el formato de dos dígitos (`01` en lugar de `1`).

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Dahua?**

La URL estándar es `rtsp://admin:password@IP_CAMARA:554/cam/realmonitor?channel=1&subtype=0` para el flujo principal. Usa `subtype=1` para el subflujo (menor resolución, menos ancho de banda).

**¿Las cámaras Amcrest usan las mismas URLs RTSP que Dahua?**

Sí. Las cámaras Amcrest son fabricadas por Dahua y usan patrones de URL RTSP, autenticación y configuraciones de puerto idénticos. Cualquier URL RTSP que funcione para una cámara Dahua funcionará para el modelo Amcrest correspondiente.

**¿Cómo accedo a múltiples cámaras en un NVR Dahua?**

Cambia el parámetro `channel` en la URL RTSP. El canal 1 es la primera cámara, el canal 2 es la segunda, y así sucesivamente. Por ejemplo, `rtsp://IP:554/cam/realmonitor?channel=3&subtype=0` conecta a la tercera cámara en el flujo principal del NVR.

**¿Por qué mi cámara Dahua usa el puerto 1554 en lugar del 554?**

Algunos modelos Dahua más antiguos, particularmente la serie DH-IPC-HF, usan por defecto el puerto RTSP 1554. Puedes cambiar esto en la interfaz web de la cámara en Configuración > Red > Puerto. Los modelos más recientes usan el puerto 554 por defecto.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Amcrest](amcrest.md) — OEM de Dahua, formato de URL idéntico
- [Guía de Conexión Lorex](lorex.md) — Usa formato de URL Dahua para muchos modelos
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
