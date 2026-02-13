---
title: Cómo Conectar una Cámara IP Ubiquiti (UniFi) en C# .NET
description: Conecta cámaras Ubiquiti UniFi Protect y AirCam en C# .NET con patrones de URL RTSP y ejemplos de código para modelos G3, G4, G5 y series AI.
---

# Cómo Conectar una Cámara IP Ubiquiti (UniFi) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Ubiquiti Inc.** es una empresa tecnológica americana con sede en la ciudad de Nueva York, conocida por equipos de red bajo la marca **UniFi**. La línea de cámaras de Ubiquiti es parte del ecosistema **UniFi Protect**, que incluye cámaras, NVRs (grabadores de video en red), timbres y sensores. Las cámaras UniFi Protect se gestionan a través de una consola central (Dream Machine, Cloud Key o NVR) y son populares en entornos prosumer y PYME.

**Datos clave:**

- **Líneas de producto:** UniFi Protect G3 (1080p), G4 (2K/4MP), G5 (2K/4MP actualizado), serie AI (con IA integrada), UVC (legacy AirCam)
- **Soporte de protocolos:** RTSP (debe habilitarse por cámara), ONVIF (limitado), protocolo propietario UniFi Protect
- **Puerto RTSP predeterminado:** 7447 (UniFi Protect) o 554 (legacy AirCam)
- **Credenciales predeterminadas:** Establecidas durante la configuración de UniFi Protect (RTSP usa credenciales separadas por cámara)
- **Soporte ONVIF:** No soportado nativamente; RTSP es el método de integración con terceros
- **Códecs de video:** H.264 (todos los modelos)

!!! warning "RTSP debe habilitarse"
    Las cámaras UniFi Protect **no** tienen RTSP habilitado por defecto. Debes habilitar RTSP para cada cámara individualmente a través de la interfaz web o app de UniFi Protect. Sin habilitarlo, la cámara no responderá a conexiones RTSP.

### Habilitación de RTSP en Cámaras UniFi Protect

1. Abre la interfaz web de **UniFi Protect** (a través de tu Dream Machine, Cloud Key o NVR)
2. Ve a **Dispositivos** y selecciona la cámara
3. Abre la pestaña **Configuración**
4. Desplázate a la sección **Avanzado**
5. Habilita el interruptor **RTSP**
6. Anota la URL RTSP mostrada (incluye un token único)

## Patrones de URL RTSP

### Cámaras UniFi Protect (Actuales)

Las cámaras UniFi Protect exponen RTSP en el **puerto 7447** con selección de calidad de flujo:

| Flujo | URL RTSP | Resolución | Notas |
|--------|----------|------------|-------|
| Alta calidad | `rtsp://IP:7447/STREAM_TOKEN` | Completa (hasta 2688x1512) | Flujo principal |
| Calidad media | `rtsp://IP:7447/STREAM_TOKEN` | Reducida | Flujo medio |
| Baja calidad | `rtsp://IP:7447/STREAM_TOKEN` | Baja (640x360) | Optimizado para ancho de banda |

!!! info "Tokens de flujo"
    UniFi Protect genera URLs RTSP únicas por cámara cuando habilitas RTSP. La URL contiene un token único. Puedes encontrar la URL exacta en la interfaz de UniFi Protect bajo la configuración Avanzada de cada cámara.

El formato de URL RTSP es típicamente:

```
rtsp://IP_CAMARA:7447/CADENA_TOKEN_UNICO
```

Donde el token es generado automáticamente y mostrado en la interfaz de UniFi Protect.

### Modelos de Cámaras UniFi Protect

| Modelo | Resolución | Flujos | Factor de Forma |
|-------|-----------|---------|-------------|
| G3 Instant | 1920x1080 | Alto/Bajo | Mini interior |
| G3 Flex | 1920x1080 | Alto/Medio/Bajo | Flex interior/exterior |
| G3 Bullet | 1920x1080 | Alto/Medio/Bajo | Bala exterior |
| G3 Dome | 1920x1080 | Alto/Medio/Bajo | Domo exterior |
| G4 Instant | 2688x1512 | Alto/Medio/Bajo | Mini interior |
| G4 Bullet | 2688x1512 | Alto/Medio/Bajo | Bala exterior |
| G4 Dome | 2688x1512 | Alto/Medio/Bajo | Domo exterior |
| G4 Pro | 3840x2160 | Alto/Medio/Bajo | Pro exterior |
| G4 PTZ | 3840x2160 | Alto/Medio/Bajo | PTZ |
| G5 Bullet | 2688x1512 | Alto/Medio/Bajo | Bala exterior |
| G5 Dome | 2688x1512 | Alto/Medio/Bajo | Domo exterior |
| G5 Turret Ultra | 3840x2160 | Alto/Medio/Bajo | Torreta exterior |
| AI 360 | 3840x2160 | Alto/Medio/Bajo | Ojo de pez |
| AI Bullet | 3840x2160 | Alto/Medio/Bajo | Bala exterior |
| AI Pro | 3840x2160 | Alto/Medio/Bajo | Pro exterior |

### URLs Legacy AirCam/AirVision

Las cámaras Ubiquiti más antiguas (serie AirCam, antes de UniFi Protect) usaban el puerto estándar 554:

| Modelo | URL RTSP | Notas |
|-------|----------|-------|
| AirCam | `rtsp://IP:554/live/ch00_0` | Flujo principal |
| AirCam Dome | `rtsp://IP:554/live/ch00_0` | Variante domo |
| AirCam Mini | `rtsp://IP:554/live/ch00_0` | Variante mini |
| AirCam (canal) | `rtsp://IP:554/ch0N_0` | N = número de canal |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara UniFi Protect con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// UniFi Protect camera, token-based auth (no username/password needed)
var uri = new Uri("rtsp://192.168.1.40:7447/YOUR_STREAM_TOKEN");
```

Las cámaras UniFi Protect usan autenticación basada en token -- el token de flujo único se proporciona en la interfaz de UniFi Protect cuando habilitas RTSP. No se requiere nombre de usuario ni contraseña por separado. Para diferentes calidades de flujo (alta/media/baja), selecciona el flujo correspondiente en la interfaz de Protect para obtener su token.

Para modelos legacy AirCam, usa el puerto 554 con credenciales `ubnt`/`ubnt` y la ruta `/live/ch00_0`.

## URLs de Captura

### Legacy AirCam

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura | `http://IP/snapshot.cgi` | Captura básica |
| Captura (auth) | `http://IP/snapshot.cgi?user=USER&pwd=PASS` | Con credenciales |
| Captura (alt) | `http://IP:554/snapshot.cgi?user=USER&pwd=PASS&count=0` | Vía puerto RTSP |

### UniFi Protect

Las cámaras UniFi Protect no exponen endpoints HTTP de captura directamente. Las capturas se acceden a través de la API de UniFi Protect o capturando fotogramas del flujo RTSP en tu aplicación.

## Solución de Problemas

### "Connection refused" en el puerto 554

Las cámaras UniFi Protect usan el **puerto 7447** para RTSP, no el puerto estándar 554. El puerto 554 solo aplica a modelos legacy AirCam. Asegúrate de usar el puerto correcto:

- **Cámaras UniFi Protect:** Puerto 7447
- **Legacy AirCam:** Puerto 554

### RTSP no habilitado

RTSP está deshabilitado por defecto en las cámaras UniFi Protect. Debes habilitarlo en la interfaz de UniFi Protect:

1. UniFi Protect > Dispositivos > Seleccionar Cámara > Configuración > Avanzado > Habilitar RTSP

### El token de flujo cambió

El token de flujo RTSP puede cambiar si:
- Deshabilitas y vuelves a habilitar RTSP en la cámara
- Reseteas la cámara
- Actualizas el firmware

Siempre verifica la URL RTSP actual en la interfaz de UniFi Protect si tu conexión deja de funcionar.

### Alta latencia

Las cámaras UniFi Protect pueden presentar 2-5 segundos de latencia por defecto. Para reducir la latencia:

- Usa `LowLatencyMode = true` en VideoCaptureCoreX
- Selecciona el flujo de baja calidad (menor resolución = menos buffering)
- Usa transporte TCP para una entrega más confiable

### Sin soporte ONVIF

Las cámaras UniFi Protect no soportan ONVIF. Usa RTSP para integración con terceros. Si necesitas descubrimiento ONVIF, no funcionará con estas cámaras.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras UniFi Protect?**

El formato de URL RTSP es `rtsp://IP_CAMARA:7447/TOKEN_UNICO`. RTSP debe habilitarse por cámara en la interfaz de UniFi Protect, que mostrará la URL única. No hay una URL predeterminada universal -- cada cámara obtiene un token de flujo único.

**¿Puedo usar cámaras UniFi sin UniFi Protect?**

Las cámaras UniFi actuales requieren un controlador UniFi Protect (Dream Machine, Cloud Key o NVR) para la configuración y gestión inicial. Una vez que RTSP está habilitado, puedes transmitir a software de terceros. Los modelos legacy AirCam funcionan de forma independiente.

**¿Las cámaras UniFi soportan H.265?**

Con el firmware actual, las cámaras UniFi Protect transmiten H.264 por RTSP. El soporte H.265 puede estar disponible para grabación interna pero típicamente no se expone vía RTSP.

**¿Cuáles son las credenciales predeterminadas para AirCam?**

Las cámaras legacy AirCam usan `ubnt` / `ubnt` como credenciales predeterminadas. Las cámaras UniFi Protect actuales usan autenticación RTSP basada en token.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Reolink](reolink.md) — Alternativa prosumer con RTSP
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
