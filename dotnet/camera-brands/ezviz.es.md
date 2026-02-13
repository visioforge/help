---
title: Cómo Conectar una Cámara IP EZVIZ en C# .NET
description: Conecte cámaras EZVIZ en C# .NET con patrones de URL RTSP para C1C, C3W, C6N, BC1C y otros modelos. Habilite RTSP en cámaras orientadas a la nube.
---

# Cómo Conectar una Cámara IP EZVIZ en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**EZVIZ** es una marca de cámaras de seguridad y hogar inteligente para consumidores propiedad de **Hikvision**. Lanzada originalmente como la división de consumo de Hikvision, EZVIZ se convirtió en una marca independiente enfocada en cámaras domésticas, timbres, cerraduras inteligentes y dispositivos IoT. Las cámaras EZVIZ están diseñadas principalmente para uso basado en la nube a través de la aplicación EZVIZ, pero muchos modelos soportan transmisión RTSP local cuando se habilita.

**Datos clave:**

- **Líneas de productos:** Serie C (interior/exterior), Serie BC (batería), Serie DB (timbres), Serie H (panorámica/inclinación)
- **Soporte de protocolos:** RTSP (debe habilitarse), ONVIF (modelos limitados), EZVIZ Cloud (predeterminado)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / código de verificación (impreso en la etiqueta de la cámara)
- **Soporte ONVIF:** Limitado (solo algunos modelos más recientes)
- **Códecs de vídeo:** H.264, H.265 (modelos selectos)
- **Empresa matriz:** Hikvision

!!! warning "RTSP Debe Habilitarse Manualmente"
    Las cámaras EZVIZ son dispositivos orientados a la nube. **RTSP está deshabilitado por defecto** en la mayoría de los modelos. Debe habilitar RTSP a través de la aplicación móvil EZVIZ o el portal web antes de conectar con el VisioForge SDK. Algunos modelos económicos y con batería no soportan RTSP en absoluto.

## Habilitación de RTSP en Cámaras EZVIZ

Antes de conectar, habilite el acceso RTSP:

1. Abra la **aplicación EZVIZ** en su teléfono
2. Seleccione su cámara → **Configuración** (icono de engranaje)
3. Navegue a **Red local** o **Acceso LAN**
4. Habilite **RTSP** o **Acceso de terceros**
5. Anote el código de verificación (generalmente impreso en la etiqueta de la cámara o mostrado en la aplicación)

Alternativamente, use el portal web EZVIZ en `https://www.ezvizlife.com` para gestionar la configuración de la cámara.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras EZVIZ utilizan patrones de URL RTSP derivados de Hikvision:

```
rtsp://admin:[VERIFICATION_CODE]@[IP]:554/h264/ch1/main/av_stream
```

| Flujo | Patrón de URL | Descripción |
|-------|---------------|-------------|
| Flujo principal | `rtsp://IP:554/h264/ch1/main/av_stream` | Resolución completa |
| Subflujo | `rtsp://IP:554/h264/ch1/sub/av_stream` | Menor resolución |

!!! info "Código de Verificación como Contraseña"
    Las cámaras EZVIZ utilizan el **código de verificación** (impreso en la etiqueta de la cámara) como contraseña RTSP. El nombre de usuario es siempre `admin`. Esto es diferente de la contraseña de la cuenta de nube EZVIZ.

### Formatos de URL Alternativos

Algunos modelos EZVIZ soportan patrones de URL adicionales:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/h264/ch1/main/av_stream` | Estándar (recomendado) |
| `rtsp://IP:554/h264/ch1/sub/av_stream` | Subflujo |
| `rtsp://IP:554/Streaming/Channels/101` | Estilo Hikvision (algunos modelos) |
| `rtsp://IP:554/Streaming/Channels/102` | Subflujo estilo Hikvision |
| `rtsp://IP:554/live` | Ruta simple (modelos antiguos) |

### Modelos de Cámaras

| Modelo | Tipo | Soporte RTSP | URL de Flujo Principal |
|--------|------|-------------|----------------------|
| C6N (panorámica/inclinación interior) | Interior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C6W (4MP interior PT) | Interior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C1C (1080p interior) | Interior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| H6c (panorámica/inclinación) | Interior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| H8c (PT exterior) | Exterior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3W (bala exterior) | Exterior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3WN (bala exterior) | Exterior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3X (doble lente exterior) | Exterior | Sí | `rtsp://IP:554/h264/ch1/main/av_stream` |
| BC1C (cámara con batería) | Batería | No | N/A — solo nube |
| DB1C (timbre) | Timbre | No | N/A — solo nube |

!!! warning "Modelos con Batería y Timbre"
    Las cámaras EZVIZ con batería (serie BC) y los timbres de vídeo (serie DB) generalmente **no** soportan RTSP. Estos dispositivos solo transmiten a través de la nube EZVIZ. Solo las cámaras con alimentación de corriente alterna y conexión de red por cable o WiFi estable soportan RTSP.

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara EZVIZ con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// EZVIZ C6N, main stream (verification code from label)
var uri = new Uri("rtsp://192.168.1.90:554/h264/ch1/main/av_stream");
var username = "admin";
var password = "ABCDEF"; // verification code from camera label
```

Para acceso al subflujo, use `/h264/ch1/sub/av_stream` en su lugar.

## URLs de Captura

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación básica con código de verificación |

## Solución de Problemas

### "Conexión rechazada" o sin respuesta

RTSP está deshabilitado por defecto en las cámaras EZVIZ. Debe habilitarlo a través de la aplicación EZVIZ primero. Verifique **Configuración > Red local > Acceso de terceros**.

### Contraseña incorrecta

Las cámaras EZVIZ utilizan el **código de verificación** (6 letras mayúsculas impresas en la etiqueta de la cámara) como contraseña RTSP, **no** la contraseña de su cuenta de nube EZVIZ. El nombre de usuario es siempre `admin`.

### Cámara no en la red local

Las cámaras EZVIZ se conectan a la nube vía WiFi. Para usar RTSP, la cámara y su aplicación deben estar en la misma red local. La IP local de la cámara se puede encontrar en la aplicación EZVIZ en **Información del dispositivo** o en la lista de clientes DHCP de su router.

### La opción RTSP no está disponible en la aplicación

Algunos modelos y versiones de firmware de EZVIZ no exponen la configuración RTSP. En este caso:

1. Actualice el firmware de la cámara a través de la aplicación EZVIZ
2. Si RTSP aún no aparece, el modelo puede no soportar transmisión local
3. Los modelos con batería y timbres normalmente no soportan RTSP

### El formato de URL de Hikvision funciona en algunos modelos

Dado que las cámaras EZVIZ usan firmware de Hikvision, algunos modelos también aceptan el formato de URL de Hikvision (`/Streaming/Channels/101`). Pruebe esto si la URL estándar de EZVIZ no funciona.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras EZVIZ?**

La URL estándar es `rtsp://admin:VERIFICATION_CODE@CAMERA_IP:554/h264/ch1/main/av_stream`. El VERIFICATION_CODE es el código de 6 caracteres impreso en la etiqueta de su cámara. RTSP debe habilitarse en la aplicación EZVIZ primero.

**¿EZVIZ está relacionada con Hikvision?**

Sí. EZVIZ es una marca propiedad de Hikvision, enfocada en el mercado de hogar inteligente para consumidores. Las cámaras EZVIZ usan firmware derivado de Hikvision, razón por la cual funcionan patrones de URL RTSP similares. Sin embargo, las cámaras EZVIZ están diseñadas principalmente para uso basado en la nube.

**¿Puedo usar cámaras EZVIZ sin la nube?**

Parcialmente. Puede acceder a flujos RTSP localmente sin la nube EZVIZ para visualización en vivo y grabación. Sin embargo, la configuración inicial de la cámara, las actualizaciones de firmware y la habilitación de RTSP requieren la aplicación EZVIZ (que usa la nube). Funciones como alertas de movimiento y almacenamiento de clips requieren una suscripción a la nube EZVIZ.

**¿Las cámaras EZVIZ soportan ONVIF?**

Algunos modelos EZVIZ más recientes soportan ONVIF, pero no está disponible en todas las cámaras. Verifique las especificaciones de su cámara o la configuración de la aplicación EZVIZ para soporte ONVIF. Para la mayoría de las cámaras EZVIZ, la conexión RTSP directa es más confiable que ONVIF.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Hikvision](hikvision.md) — Empresa matriz, formato de URL similar
- [Guía de Conexión Imou](imou.md) — Marca de consumo de Dahua, mercado similar
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
