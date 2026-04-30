---
title: Cómo Conectar una Cámara Wyze en C# .NET - Soluciones RTSP
description: Conecte cámaras Wyze en C# .NET usando firmware RTSP o puente RTSP Docker. Limitaciones RTSP, soluciones y enfoques alternativos explicados.
meta:
  - name: robots
    content: "noindex, follow"
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
  - C#

---

# Cómo Conectar una Cámara Wyze en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Wyze Labs** es una empresa estadounidense de electrónica de consumo con sede en Kirkland, Washington. Conocida por sus cámaras de hogar inteligente extremadamente asequibles, Wyze se convirtió en una de las marcas de cámaras más vendidas en Norteamérica. Sin embargo, las cámaras Wyze son **dispositivos orientados a la nube** con soporte RTSP muy limitado, lo que hace que la integración directa sea un desafío.

**Datos clave:**

- **Líneas de productos:** Cam v3/v4 (interior/exterior), Cam Pan v3 (PTZ), Cam OG (compacta), Doorbell, Floodlight
- **Soporte de protocolos:** Wyze Cloud (principal), RTSP (limitado — requiere firmware especial en modelos selectos)
- **Puerto RTSP predeterminado:** 8554 (al usar firmware RTSP)
- **Soporte ONVIF:** No
- **Códecs de vídeo:** H.264
- **Dependencia de la nube:** Alta — la mayoría de funciones requieren la aplicación Wyze y la nube

!!! warning "Soporte RTSP Muy Limitado"
    Las cámaras Wyze **no** soportan RTSP nativamente. El firmware RTSP oficial se lanzó solo para la **Wyze Cam v2** y la **Wyze Cam Pan** original, y estas compilaciones de firmware ya no se mantienen activamente. Para modelos más recientes (v3, v4, OG, Pan v3), RTSP requiere soluciones de terceros como firmware personalizado.

## Soporte RTSP por Modelo

| Modelo | RTSP Nativo | Firmware RTSP | RTSP de Terceros | Notas |
|--------|------------|--------------|-----------------|-------|
| Wyze Cam v2 | No | Sí (beta) | Sí | Firmware RTSP oficial disponible |
| Wyze Cam Pan v1 | No | Sí (beta) | Sí | Firmware RTSP oficial disponible |
| Wyze Cam v3 | No | No | Sí (docker-wyze-bridge) | Solución comunitaria |
| Wyze Cam v4 | No | No | Sí (docker-wyze-bridge) | Solución comunitaria |
| Wyze Cam Pan v3 | No | No | Sí (docker-wyze-bridge) | Solución comunitaria |
| Wyze Cam OG | No | No | Sí (docker-wyze-bridge) | Solución comunitaria |
| Wyze Doorbell v2 | No | No | Limitado | Puede funcionar con el puente |
| Wyze Cam Floodlight v2 | No | No | Sí (docker-wyze-bridge) | Solución comunitaria |

## Opción 1: Firmware RTSP Oficial (Solo Cam v2 / Pan v1)

Wyze lanzó firmware RTSP beta para la Cam v2 y la Cam Pan original. Cuando se instala:

### Formato de URL RTSP

```
rtsp://[IP]:8554/live
```

!!! note "Puerto No Estándar"
    El firmware RTSP de Wyze usa el puerto **8554**, no el estándar 554.

### Pasos de Configuración

1. Descargue el firmware RTSP del soporte de Wyze (busque "Wyze RTSP firmware")
2. Instale el firmware en la cámara mediante tarjeta microSD
3. En la aplicación Wyze, vaya a la configuración de la cámara y habilite RTSP
4. Anote la URL RTSP mostrada en la aplicación (generalmente `rtsp://CAMERA_IP:8554/live`)

### Conexión con VisioForge SDK

```csharp
// Wyze Cam v2 with RTSP firmware
var uri = new Uri("rtsp://192.168.1.90:8554/live");
var username = ""; // no authentication on Wyze RTSP firmware
var password = "";
```

Utilice la URL RTSP de su cámara Wyze con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido).

## Opción 2: Docker Wyze Bridge (Todos los Modelos)

Para Wyze Cam v3, v4, Pan v3, OG y otros modelos más recientes, el proyecto comunitario **docker-wyze-bridge** crea un proxy RTSP que convierte flujos de la nube Wyze a RTSP local:

### Cómo Funciona

1. Docker Wyze Bridge se autentica con su cuenta Wyze
2. Se conecta a la cámara a través de la API de nube Wyze
3. Retransmite el vídeo como un flujo RTSP local
4. Su aplicación se conecta al puente, no directamente a la cámara

### Formato de URL RTSP (vía Puente)

```
rtsp://[BRIDGE_IP]:8554/[CAMERA_NAME]
```

Donde `CAMERA_NAME` es el nombre que asignó en la aplicación Wyze (espacios reemplazados con guiones, en minúsculas).

### Conexión vía Puente con VisioForge SDK

```csharp
// Wyze Cam v3 via docker-wyze-bridge
var uri = new Uri("rtsp://192.168.1.50:8554/front-door");
var username = ""; // bridge handles auth
var password = "";
```

!!! warning "Limitaciones del Puente"
    Docker Wyze Bridge introduce latencia adicional (típicamente 3-10 segundos) ya que el vídeo pasa por la nube de Wyze antes de llegar a su flujo RTSP local. También requiere las credenciales de su cuenta Wyze y una conexión activa a internet.

## Solución de Problemas

### No hay opción RTSP en la aplicación Wyze

El interruptor RTSP solo aparece en Wyze Cam v2 y Pan v1 cuando se ha instalado el firmware RTSP. No está disponible en modelos más recientes. Para v3/v4/OG/Pan v3, use el enfoque Docker Wyze Bridge.

### El firmware RTSP no conecta

Después de instalar el firmware RTSP en la Cam v2:

1. Espere 2-3 minutos para que la cámara arranque completamente
2. Verifique que la cámara esté en la misma red que su aplicación
3. Pruebe `rtsp://CAMERA_IP:8554/live` en VLC primero para confirmar que el flujo funciona
4. El flujo no tiene autenticación -- deje el nombre de usuario y contraseña vacíos

### Alta latencia con Docker Wyze Bridge

El puente enruta el vídeo a través de los servidores en la nube de Wyze, añadiendo latencia. Para requisitos de baja latencia, las cámaras Wyze pueden no ser adecuadas. Considere cámaras con soporte RTSP nativo como [Reolink](reolink.md), [Amcrest](amcrest.md) o [TP-Link Tapo](tp-link.md).

### Calidad del flujo

Las cámaras Wyze normalmente emiten flujos H.264 a 1080p. El firmware RTSP no soporta cambiar la resolución o el códec. Lo que la cámara captura es lo que el flujo RTSP proporciona.

## Preguntas Frecuentes

**¿Las cámaras Wyze soportan RTSP nativamente?**

No. Las cámaras Wyze son dispositivos orientados a la nube. La Wyze Cam v2 y la Cam Pan original tienen firmware RTSP beta oficial, pero ya no se mantiene activamente. Los modelos más recientes (v3, v4, OG, Pan v3) no tienen firmware RTSP y requieren puentes de terceros.

**¿Puedo usar cámaras Wyze sin la nube?**

Muy limitado. Incluso con firmware RTSP, la configuración inicial requiere la aplicación Wyze y una cuenta en la nube. El firmware RTSP para Cam v2/Pan v1 desactiva algunas funciones de la nube. Para modelos más recientes, el Docker Wyze Bridge todavía se enruta a través de la nube.

**¿Qué cámaras debería usar en lugar de Wyze para RTSP?**

Para cámaras asequibles con soporte RTSP nativo, considere [Reolink](reolink.md) (consumo), [Amcrest](amcrest.md) (consumo/PyME), [TP-Link Tapo](tp-link.md) (consumo) o [EZVIZ](ezviz.md) (hogar inteligente). Todas estas proporcionan acceso RTSP directo sin soluciones alternativas.

**¿Las cámaras Wyze soportan ONVIF?**

No. Las cámaras Wyze no soportan ONVIF en ninguna versión de firmware.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Reolink](reolink.md) — Alternativa asequible con RTSP nativo
- [Guía de Conexión Amcrest](amcrest.md) — Cámaras de consumo con RTSP completo
- [Guía de Conexión TP-Link](tp-link.md) — Cámaras económicas con soporte RTSP
- [Instalación del SDK y Ejemplos](index.md#comenzar)
