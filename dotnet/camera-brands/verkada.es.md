---
title: URL RTSP de Verkada y conexión de cámaras en C# .NET
description: URL RTSP de Verkada rtsp://IP:8554/standard para C# .NET. Active RTSP en Verkada Command, conéctese desde Media Blocks SDK .NET y revise los límites.
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
  - C#
primary_api_classes:
  - RTSPSourceSettings

---

# Cómo Conectar una Cámara Verkada en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Verkada** es una empresa estadounidense de cámaras de seguridad gestionadas en la nube con sede en San Mateo, California. Fundada en 2016, Verkada ofrece cámaras de grado empresarial gestionadas a través de la plataforma Verkada Command. Junto con la gestión en la nube, una cámara puede servir flujos RTSP directamente en la red local una vez que se habilita RTSP para ella; las cámaras CR de la serie remota son la excepción.

**Datos clave:**

- **Líneas de productos:** CD (domo), CB (bala), CF (ojo de pez), CM (mini domo), CH y CY (multi-sensor), CP (PTZ), CR (remota); el sufijo `-E` marca una variante exterior
- **Arquitectura:** Gestionada en la nube, con transmisión RTSP local
- **Soporte RTSP:** Sí — se habilita por cámara en Command
- **Soporte ONVIF:** No
- **Puerto RTSP:** 8554
- **Rutas de flujo:** `/standard` y `/high`
- **Acceso a red local:** Solo RTSP, sobre direcciones privadas RFC 1918
- **Códecs de vídeo:** H.264, H.265 — se eligen por cámara en Command
- **Acceso API:** API de Verkada (basada en la nube; un administrador de la organización emite las claves en Command)

## Habilitar RTSP

RTSP permanece desactivado hasta que un administrador del sitio lo habilita para una cámara concreta: **All Products → Cameras → seleccione la cámara → Settings → Device → Real Time Streaming Protocol (RTSP)**. Defina un nombre de usuario y una contraseña y haga clic en **Enable**. La contraseña se muestra una sola vez, así que guárdela. Command muestra entonces la URL RTSP de la cámara con un marcador `[PASSWORD]`: el marcador debe sustituirlo usted por la contraseña que acaba de definir, porque la contraseña no se puede volver a leer en Command. Command también puede exportar un CSV con todas las cámaras de la organización que tienen RTSP habilitado, junto con su URL.

No se requiere una licencia RTSP aparte: se aplica la regla habitual de una licencia por cámara.

## Formato de la URL RTSP

```
rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/[STREAM]
```

| Flujo | Ruta | Notas |
|--------|------|-------|
| Estándar | `/standard` | Menor tasa de bits |
| Alta calidad | `/high` | Calidad completa |

La resolución y la tasa de bits siguen los flujos nativos de la cámara y no se pueden cambiar para RTSP.

Las cámaras multi-sensor (series CH y CY) colocan el id de la cámara antes del segmento de flujo, `rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/[CAMERA-ID]/high`, donde el id es el que Command muestra para ese sensor.

## Conexión con el SDK de VisioForge

Incluya la dirección y la ruta del flujo en el URI, y pase las credenciales de Command como `login` y `password`:

```csharp
// Cámara Verkada con RTSP habilitado, flujo de alta calidad
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:8554/high"),
    login: "rtsp-user",
    password: "YourRtspPassword",
    audioEnabled: true);
```

Un URI con la contraseña incrustada (`rtsp://rtsp-user:YourRtspPassword@192.168.1.100:8554/high`) también funciona, porque `rtspsrc` se autentica con las credenciales de la propia URL. Aun así, es preferible pasarlas por separado: la URL que muestra Command lleva un marcador `[PASSWORD]` que hay que rellenar, y una vez rellenado circula por registros, archivos de configuración y capturas de pantalla, donde la contraseña no debería estar.

## Limitaciones

- **Solo red local.** El flujo llega directamente desde la cámara por direcciones privadas y no se retransmite a través de Verkada Cloud, por lo que un cliente en otra red necesita una VPN hacia esa red.
- **Sin cifrado.** Verkada no ofrece RTSPS. Su propia recomendación es mantener la red de cámaras privada y restringir qué clientes pueden acceder a ella.
- **El códec pertenece a la cámara.** El ajuste **Settings → Device → Video Codec** de la propia cámara decide si el flujo RTSP es H.264 o H.265, y H.265 no se activa para RTSP de forma automática. No hay respaldo a H.264: un cliente que no pueda decodificar H.265 no ve nada hasta que la cámara se configure en H.264.
- **El audio está desactivado hasta que se activa.** El audio de la cámara viene desactivado por defecto; un administrador del sitio lo habilita por cámara en **Settings → Device → Camera Audio**, lo que reinicia esa cámara. Hasta entonces el flujo RTSP no lleva pista de audio alguna y `CreateAsync` devuelve los ajustes con `AudioEnabled` desactivado.
- **Algunos modelos no tienen audio por RTSP ni así.** CD22, CD22-E, CM22, CF81-E, CF83-E, toda la serie CB (Bullet), CH52-E, CP52-E y CP63-E: pase `audioEnabled: false` para ellos. De los modelos que sí sirven RTSP, el resto incluye pista de audio.
- **Sin RTSP en las CR63-E y CR63-TE.**

## API de Verkada

La API REST de Verkada cubre el listado y el estado de las cámaras, la exportación y descarga de clips, las miniaturas y los datos de eventos y alertas, y también transmite vídeo en vivo — con una clave a la que se le haya concedido acceso de streaming. Un administrador de la organización emite las claves de API en **Command → Admin → API & Integrations**, donde también se definen sus permisos. Use la API cuando el vídeo deba llegar desde fuera de la red de la propia cámara; el flujo RTSP es la vía local de baja latencia.

## Cámaras Empresariales con RTSP y ONVIF

Verkada ya transmite por RTSP, pero sigue sin ofrecer ONVIF, así que la detección automática y la selección de perfiles no están disponibles. Cuando eso importe, estas marcas ofrecen ambas cosas:

| Alternativa | Segmento de Mercado | RTSP | ONVIF | Guía |
|------------|---------------------|------|-------|------|
| Axis | Empresarial | Sí | Sí | [Guía de Conexión](axis.md) |
| Bosch | Empresarial | Sí | Sí | [Guía de Conexión](bosch.md) |
| Hanwha Vision | Empresarial | Sí | Sí | [Guía de Conexión](hanwha.md) |
| Avigilon | Empresarial | Sí | Sí | [Guía de Conexión](avigilon.md) |
| Hikvision | Empresarial | Sí | Sí | [Guía de Conexión](hikvision.md) |

## Preguntas Frecuentes

**¿Cuál es la URL RTSP de una cámara Verkada?**

`rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/high`, donde `CAMERA-IP` es la dirección privada de la cámara y `[USERNAME]` y `[PASSWORD]` son las credenciales definidas para esa cámara en Command. Use `/standard` en lugar de `/high` para el flujo de menor tasa de bits. Las cámaras multi-sensor insertan el id de la cámara antes del segmento de flujo.

**¿Las cámaras Verkada soportan RTSP?**

Sí. Verkada anunció RTSP de baja latencia para sus cámaras, que se sirve por la red local. Viene desactivado por defecto y un administrador del sitio debe habilitarlo por cámara en Command.

**¿Las cámaras Verkada soportan ONVIF?**

No. No hay endpoint ONVIF, por lo que la detección ONVIF y la selección de perfiles no aplican: use la URL RTSP que le da Command.

**¿Puedo acceder a un flujo RTSP de Verkada desde fuera de la red local?**

No directamente. Verkada sirve RTSP solo en direcciones privadas y no lo retransmite por la nube, así que un cliente en otra red necesita una VPN.

**¿El flujo RTSP de Verkada incluye audio?**

No hasta que un administrador del sitio habilite **Camera Audio** para esa cámara en Command (**Settings → Device → Camera Audio**, lo que reinicia la cámara): hasta entonces no hay pista de audio alguna. Y tampoco en los modelos cuyo flujo RTSP no lleva audio: CD22, CD22-E, CM22, CF81-E, CF83-E, la serie CB, CH52-E, CP52-E y CP63-E. Entre los modelos que sí sirven RTSP, el resto incluye audio, por lo que `audioEnabled: true` aplica.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Rhombus](rhombus.md) — Otra plataforma gestionada en la nube con RTSP
- [Guía de Conexión Axis](axis.md) — Alternativa empresarial con RTSP y ONVIF
- [Instalación del SDK y Ejemplos](index.md#comenzar)
