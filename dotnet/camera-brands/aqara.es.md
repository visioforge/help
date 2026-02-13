---
title: Cómo Conectar una Cámara Aqara en C# .NET - RTSP vía HomeKit
description: Conecte cámaras Aqara en C# .NET con patrones de URL RTSP para modelos G2H, G3, E1 y Camera Hub. Guía de acceso HomeKit Secure Video y RTSP.
---

# Cómo Conectar una Cámara Aqara en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Aqara** es una marca china de hogar inteligente (de Lumi United Technology) que produce dispositivos de hogar inteligente basados en Zigbee/Thread y cámaras. Las cámaras Aqara son únicas en el mercado porque funcionan como hubs de hogar inteligente (gateway Zigbee) además de funcionar como cámaras de seguridad. Aqara se integra principalmente con Apple HomeKit y soporta RTSP para transmisión local.

**Datos clave:**

- **Líneas de productos:** Camera Hub G2H/G3 (hub + cámara), E1 (solo cámara), G4 (timbre)
- **Soporte de protocolos:** RTSP, Apple HomeKit Secure Video, Zigbee 3.0 (función de hub)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** Ninguna — la URL RTSP incluye token
- **Soporte ONVIF:** No
- **Códecs de vídeo:** H.264
- **Característica única:** Las cámaras sirven como hubs de hogar inteligente Zigbee

!!! info "RTSP Debe Habilitarse en la Aplicación Aqara Home"
    Las cámaras Aqara tienen soporte RTSP pero debe habilitarse a través de la aplicación **Aqara Home**. La aplicación genera una URL RTSP única con un token de autenticación. El acceso RTSP funciona independientemente de HomeKit Secure Video.

## Habilitación de RTSP en Cámaras Aqara

1. Abra la aplicación **Aqara Home** en su teléfono
2. Seleccione su dispositivo de cámara
3. Vaya a **Configuración de cámara** (icono de engranaje)
4. Busque **RTSP** o **Transmisión de red**
5. Habilite RTSP
6. La aplicación mostrará la URL RTSP completa con token de autenticación
7. Copie esta URL para usarla en su aplicación

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Aqara utilizan una URL RTSP basada en token:

```
rtsp://[IP]:554/live/ch00_1?token=[AUTH_TOKEN]
```

El token de autenticación es generado por la aplicación Aqara Home y es único por cámara.

### Modelos de Cámaras

| Modelo | Tipo | Soporte RTSP | Resolución | Función de Hub |
|--------|------|-------------|-----------|---------------|
| Camera Hub G2H Pro | Hub + cámara | Sí | 1920x1080 | Zigbee 3.0 |
| Camera Hub G3 | Hub + cámara | Sí | 2304x1296 (2K) | Zigbee 3.0 |
| Camera E1 | Solo cámara | Sí | 1920x1080 | No |
| G4 Video Doorbell | Timbre | Limitado | 1600x1200 | No |

### Variaciones de URL

| Patrón de URL | Descripción |
|---------------|-------------|
| `rtsp://IP:554/live/ch00_1?token=TOKEN` | Flujo principal (recomendado) |
| `rtsp://IP:554/live/ch00_0?token=TOKEN` | Subflujo (menor resolución) |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Aqara con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Aqara Camera Hub G3, main stream (token from Aqara Home app)
var uri = new Uri("rtsp://192.168.1.90:554/live/ch00_1?token=YOUR_TOKEN_HERE");
var username = ""; // auth is in the token
var password = "";
```

!!! tip "Autenticación Basada en Token"
    Las cámaras Aqara no utilizan autenticación de nombre de usuario/contraseña para RTSP. En su lugar, el token de autenticación está incluido en la URL. Deje los campos de nombre de usuario y contraseña vacíos en `RTSPSourceSettings.CreateAsync()` e incluya el token en la URI.

## Solución de Problemas

### La URL RTSP no funciona

1. Verifique que RTSP esté habilitado en la aplicación Aqara Home
2. Compruebe que el token en la URL coincida con lo que muestra la aplicación
3. Asegúrese de que la cámara y su aplicación estén en la misma red
4. Intente regenerar la URL RTSP en la aplicación (Configuración > RTSP > regenerar)

### El token expira o cambia

El token RTSP puede cambiar después de:

- Actualizaciones de firmware de la cámara
- Re-emparejamiento con la aplicación Aqara Home
- Deshabilitar y volver a habilitar RTSP

Si su flujo deja de funcionar, verifique la aplicación Aqara Home para una URL actualizada.

### Sin soporte ONVIF

Las cámaras Aqara no soportan ONVIF. No puede usar el descubrimiento ONVIF para encontrar cámaras Aqara. La URL RTSP debe obtenerse de la aplicación Aqara Home.

### Limitado a H.264

Las cámaras Aqara codifican solo en H.264. No hay opción H.265. Esto asegura amplia compatibilidad pero usa más ancho de banda que H.265 a calidad equivalente.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Aqara?**

Las cámaras Aqara usan `rtsp://CAMERA_IP:554/live/ch00_1?token=AUTH_TOKEN` donde el token es generado por la aplicación Aqara Home. No hay credenciales predeterminadas -- la autenticación se maneja a través del token en la URL.

**¿Puedo usar cámaras Aqara con HomeKit y RTSP simultáneamente?**

Sí. HomeKit Secure Video y RTSP pueden funcionar al mismo tiempo. Habilitar RTSP no desactiva la funcionalidad de HomeKit. Sin embargo, ejecutar ambos flujos puede reducir ligeramente el rendimiento de la cámara.

**¿Las cámaras Aqara funcionan como hubs Zigbee mientras transmiten RTSP?**

Sí. Los modelos Camera Hub G2H y G3 sirven como gateways Zigbee 3.0 y cámaras simultáneamente. Habilitar RTSP no afecta la funcionalidad del hub.

**¿Las cámaras Aqara soportan ONVIF?**

No. Las cámaras Aqara solo soportan RTSP (con autenticación por token) y HomeKit Secure Video. El descubrimiento ONVIF no está disponible.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión EZVIZ](ezviz.md) — Otra marca de cámaras de hogar inteligente
- [Guía de Conexión TP-Link](tp-link.md) — Cámaras de consumo con RTSP
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
