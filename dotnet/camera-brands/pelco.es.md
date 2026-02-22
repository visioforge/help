---
title: Pelco - URLs RTSP para Cámaras Sarix y Spectra en C# .NET
description: Conecta cámaras Pelco Sarix y Spectra en C# .NET con patrones de URL RTSP y ejemplos de código para modelos IX, IMP, IME y Spectra PTZ.
---

# Cómo Conectar una Cámara IP Pelco en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Pelco** (ahora parte de **Motorola Solutions**) es un fabricante líder de equipos profesionales de videovigilancia, con sede en Fresno, California. Pelco es particularmente fuerte en mercados empresariales, gubernamentales y de infraestructura crítica. La marca es conocida por su línea de cámaras fijas **Sarix** y su línea de cámaras PTZ **Spectra**. Motorola Solutions adquirió Pelco en 2020.

**Datos clave:**

- **Líneas de producto:** Sarix (cámaras fijas Professional/Enhanced/Value), Spectra (PTZ Professional), IX (caja fija), IMP/IME (mini domo), serie D (domo PTZ)
- **Soporte de protocolos:** RTSP, ONVIF (Perfil S/G/T), HTTP/CGI, protocolo serial Pelco D/P
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (debe cambiarse en el primer inicio para modelos actuales)
- **Soporte ONVIF:** Sí (todos los modelos actuales Sarix y Spectra)
- **Códecs de video:** H.264, H.265 (Sarix Professional), MJPEG

!!! info "Doble barra en URLs RTSP"
    Las cámaras Pelco usan consistentemente una **doble barra** antes de la ruta del flujo: `rtsp://IP:554//stream1`. Esto es intencional y necesario para la mayoría de modelos Pelco.

## Patrones de URL RTSP

### Modelos Actuales (Sarix Professional/Enhanced/Value)

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Flujo principal | `rtsp://IP:554//stream1` | Resolución completa (note la doble barra) |
| Subflujo | `rtsp://IP:554//stream2` | Resolución menor |
| Flujo baja resolución | `rtsp://IP:554/LowResolutionVideo` | Calidad más baja |
| Flujo de canal | `rtsp://IP:554/stream1` | Barra simple (algunos modelos) |
| Canal numerado | `rtsp://IP:554/1/stream1` | Específico por canal |

### URLs Específicas por Modelo

| Serie de Modelo | URL RTSP | Tipo | Notas |
|-------------|----------|------|-------|
| Sarix Pro (IMP/IME) | `rtsp://IP:554//stream1` | Domo fijo | Generación actual |
| Sarix Enhanced (IX) | `rtsp://IP:554//stream1` | Caja fija | Gama media |
| Sarix Value | `rtsp://IP:554//stream1` | Fija | Nivel de entrada |
| IX10 | `rtsp://IP:554//stream1` | Caja fija | Profesional |
| IX30C / IX30DN | `rtsp://IP:554//stream1` | Caja fija | Día/noche |
| IXDN30 | `rtsp://IP:554//stream1` | Caja fija | Día/noche |
| IXE10LW | `rtsp://IP:554//stream1` | Domo fijo | Inalámbrica |
| IXE20DN | `rtsp://IP:554//stream1` | Domo fijo | Día/noche |
| IXP31 | `rtsp://IP:554//stream1` | Domo fijo | Profesional |
| IMP519 | `rtsp://IP:554//stream1` | Mini domo | 5MP |
| IMP1110-1 / IMP1110-1E | `rtsp://IP:554//stream1` | Mini domo | Sarix Pro |
| IM10C10 | `rtsp://IP:554//stream1` | Multi-sensor | Sarix IMM |
| IM10DN10-1E | `rtsp://IP:554//stream1` | Multi-sensor | Día/noche |
| D5230-ADFRZ28 | `rtsp://IP:554//stream1` | Domo PTZ | Spectra |
| Spectra IV | `rtsp://IP:554//stream1` | Domo PTZ | PTZ antigua |
| Spectra Professional | `rtsp://IP:554//stream1` | Domo PTZ | PTZ actual |

### Multi-Canal / Multi-Sensor

Para dispositivos Pelco multicanal:

| Flujo | URL RTSP | Notas |
|--------|----------|-------|
| Canal 1, principal | `rtsp://IP:554/1/stream1` | Primer sensor/canal |
| Canal 2, principal | `rtsp://IP:554/2/stream1` | Segundo sensor/canal |
| Flujo de canal (alt) | `rtsp://IP:554/stream1` | Canal único (algunos modelos) |

### Modelos Anteriores

| Modelo | URL | Notas |
|-------|-----|-------|
| IP110 / IP-110 | `http://IP/api/jpegControl.php?frameRate=10` | Flujo JPEG |
| Spectra IV (HTTP) | `http://IP/jpeg` | Captura JPEG |
| Spectra IV (pull) | `http://IP/jpeg/pull` | JPEG continuo |
| Spectra IV (API) | `http://IP/api/jpegControl.php?frameRate=10` | JPEG con tasa de cuadros |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Pelco con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Pelco Sarix camera, main stream
var uri = new Uri("rtsp://192.168.1.85:554//stream1");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, use `//stream2` en su lugar. Para cámaras multi-sensor, use `/1/stream1` para selección de canal.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/jpeg` | La mayoría de modelos actuales |
| JPEG (canal) | `http://IP/jpeg?id=1` | Específico por canal |
| JPEG (API) | `http://IP/api/jpegControl.php?frameRate=10` | Modelos antiguos |
| JPEG (tmpfs) | `http://IP/tmpfs/auto.jpg` | Captura automática |
| Archivo de imagen | `http://IP/img.jpg` | Captura simple |

## Solución de Problemas

### La doble barra es necesaria

La mayoría de cámaras Pelco requieren una **doble barra** antes de la ruta del flujo:

- Correcto: `rtsp://IP:554//stream1`
- Puede no funcionar: `rtsp://IP:554/stream1`

Si una URL con barra simple falla, siempre intente primero la variante con doble barra.

### Numeración de canales para multi-sensor

Las cámaras multi-sensor Pelco (serie IM10, Sarix IMM) usan rutas de canal numeradas:

- `rtsp://IP:554/1/stream1` -- primer sensor
- `rtsp://IP:554/2/stream1` -- segundo sensor

Las cámaras de un solo sensor deben usar `//stream1` sin número de canal.

### Protocolo Pelco D/P vs RTSP

Pelco también es conocido por los protocolos de comunicación serial **Pelco D** y **Pelco P** usados para controlar cámaras PTZ. Estos son protocolos seriales para control PTZ, no para streaming de video. El streaming de video siempre usa RTSP o HTTP independientemente de qué protocolo de control PTZ se use.

### Cámaras PTZ Spectra

Las cámaras PTZ Pelco Spectra usan el mismo formato de URL RTSP (`//stream1`) que las cámaras fijas. El control PTZ se maneja por separado vía comandos ONVIF PTZ o protocolo serial Pelco D/P, no a través de la URL RTSP.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Pelco?**

Para la mayoría de cámaras Pelco, use `rtsp://admin:contraseña@IP_CAMARA:554//stream1` (note la doble barra). Para el subflujo, use `//stream2`. Los modelos multi-sensor usan `/1/stream1` para acceso específico por canal.

**¿Pelco sigue siendo una empresa independiente?**

No. Pelco fue adquirida por Motorola Solutions en 2020. Las cámaras Pelco actuales son fabricadas y soportadas por Motorola Solutions. La marca Pelco y las líneas de producto (Sarix, Spectra) continúan bajo el portafolio de seguridad de video de Motorola Solutions.

**¿Las cámaras Pelco soportan ONVIF?**

Sí. Todas las cámaras actuales Pelco Sarix y Spectra soportan ONVIF Profile S, G y T. ONVIF es el método recomendado de descubrimiento y configuración para nuevas integraciones Pelco.

**¿Cuál es la diferencia entre Pelco D y RTSP?**

Pelco D (y Pelco P) son protocolos seriales para control de cámaras PTZ (comandos de giro, inclinación, zoom). RTSP es el protocolo de streaming de video. Se usa RTSP para video y Pelco D/ONVIF para control PTZ -- sirven diferentes propósitos y no son intercambiables.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Avigilon](avigilon.md) — También de Motorola Solutions, cámaras empresariales
- [Guía de Conexión de Honeywell](honeywell.md) — Cámaras de vigilancia empresarial
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
