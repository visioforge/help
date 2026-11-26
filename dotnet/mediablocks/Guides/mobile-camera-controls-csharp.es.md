---
title: Zoom, linterna, enfoque y exposición de cámara en C#
description: "Cámara iOS y Android desde C# .NET: zoom ultra gran angular 0,5x, linterna, enfoque al tocar y exposición manual con Media Blocks SDK."
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Android
  - iOS
  - Capture
  - Webcam
  - C#
  - NuGet
primary_api_classes:
  - SystemVideoSourceBlock
  - ICamera2Controls
  - Camera2Capabilities
  - DeviceEnumerator
---

# Zoom, linterna, enfoque y exposición en iOS y Android

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

`SystemVideoSourceBlock` da a la cámara del teléfono en marcha una sola interfaz de control en las dos plataformas: `Camera2Controls` (`ICamera2Controls`) cambia el zoom, la linterna, el enfoque, la exposición, el balance de blancos y la estabilización mientras el pipeline funciona, y `Capabilities` (`Camera2Capabilities`) indica lo que admite la cámara abierta. En Android los miembros controlan la API Camera2; en iOS controlan el `AVCaptureDevice` que abrió la fuente. El mismo código MAUI o iOS/Android funciona en las dos.

## Obtener los controles

Léalos después de que el pipeline haya iniciado la cámara: antes, `Camera2Controls` es `null`, y en iOS también `Capabilities`. En iOS cada inicio crea controles nuevos y devuelve la cámara a su estado de apertura, como Android - enfoque, exposición y balance de blancos automáticos en el centro de la imagen, sin compensación, el zoom 1,0 en un dispositivo multicámara y el campo más amplio de un objetivo solo, la linterna apagada: vuelva a leer `Camera2Controls` y a suscribirse a sus eventos después de cada `StartAsync`.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Sources.AndroidCamera2;

var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var device = devices[0];

var format = device.GetHDVideoFormatAndFrameRate(out var frameRate);
var settings = new VideoCaptureDeviceSourceSettings(device) { Format = format.ToFormat() };
settings.Format.FrameRate = frameRate;

var videoSource = new SystemVideoSourceBlock(settings);
// ... conecte la fuente a un renderizador o a un codificador y después:
await pipeline.StartAsync();

ICamera2Controls controls = videoSource.Camera2Controls;
Camera2Capabilities caps = videoSource.Capabilities;
```

## Zoom y el objetivo ultra gran angular 0,5x

`SetZoom` recibe la relación en la escala que muestra la app Cámara del iPhone, de `MinZoom` a `MaxZoom`; un valor fuera de ese rango se ajusta al límite.

```csharp
controls.SetZoom(controls.MinZoom);   // la vista más amplia de la cámara
controls.SetZoom(2.0f);               // 2x
```

- **Android:** `MinZoom` es el mínimo propio de la cámara: inferior a 1,0 en Android 11 y posteriores cuando la cámara llega a su ultra gran angular mediante la relación de zoom, y 1,0 en otro caso. `MaxZoom` es el zoom máximo de la cámara.
- **iOS:** en un dispositivo multicámara con ultra gran angular - «Back Triple Camera» o «Back Dual Wide Camera» - `MinZoom` es 0,5: 0,5 es el ultra gran angular, 1,0 el gran angular, e iOS pasa por sí mismo al teleobjetivo cuando el zoom lo supera. En «Back Camera» y «Front Camera» es 1,0, y un objetivo abierto por separado indica su lugar en esa escala: lea `MinZoom` en lugar de suponer 1,0. Un dispositivo multicámara se abre en el gran angular, 1,0, como la app Cámara.

En iOS, `VideoSourcesAsync` lista primero «Back Camera» y «Front Camera», como antes, y después las demás cámaras traseras del teléfono: el ultra gran angular, el teleobjetivo y los dispositivos multicámara («Back Dual Wide Camera», «Back Triple Camera», «Back Dual Camera»). Sus nombres vienen de iOS y siguen el idioma del teléfono: deje que el usuario elija de la lista en lugar de comparar un nombre.

## Linterna

```csharp
if (caps.HasFlashUnit)
{
    controls.SetTorchMode(true);
}
```

## Enfoque

```csharp
controls.SetAfMode(Camera2AfMode.ContinuousVideo);

// Enfoque al tocar: una región del cuadro del sensor, aquí 100 x 100 alrededor de un punto
// dado como fracciones 0-1 (fx, fy) de ese cuadro.
var sensor = caps.SensorInfo;
var x = (int)(fx * sensor.ActiveWidth) - 50;
var y = (int)(fy * sensor.ActiveHeight) - 50;
controls.SetAfRegions(new[] { new Camera2MeteringRegion(x, y, 100, 100, 1000) });
controls.TriggerAutoFocus();

// Enfoque manual: 0 es la distancia más lejana, caps.LensInfo.MinimumFocusDistance la más cercana.
controls.SetAfMode(Camera2AfMode.Off);
controls.SetFocusDistance(caps.LensInfo.MinimumFocusDistance / 2);
```

El cuadro del sensor es el cuadro horizontal propio de la cámara en las dos plataformas, no la vista previa girada: convierta un toque sobre una vista previa vertical antes de construir la región. `FocusStateChanged` informa del barrido. En iOS:

- el centro de la primera región es el punto de enfoque, y el peso no se usa;
- `SensorInfo.Orientation` es 0, y el cuadro del sensor es el cuadro horizontal con el lado del botón de inicio a la derecha, sostenga como sostenga el teléfono. Un toque en (fx, fy), como fracciones 0-1 de una vista previa vertical de la cámara trasera que muestra todo el cuadro (quite primero el recorte de una vista previa en modo relleno), está en (fy, 1 - fx) del cuadro del sensor; para una vista previa en espejo de la cámara frontal, invierta primero fx;
- `FocusStateChanged` indica cuándo termina un barrido, no si encontró el enfoque, así que `IsFocused` no es una medida allí;
- la distancia de enfoque no está calibrada: está ordenada, pero no en metros;
- `MinimumFocusDistance` es 0 cuando la cámara no puede fijar la posición de su objetivo.

## Exposición

```csharp
// Un EV más oscuro; un paso son caps.ExposureCompensationStep EV (un tercio de EV en iOS).
controls.SetExposureCompensation((int)Math.Round(-1 / caps.ExposureCompensationStep));

// Punto de exposición, con la misma forma de región que el enfoque.
controls.SetAeRegions(new[] { new Camera2MeteringRegion(x, y, 100, 100, 1000) });

// Exposición manual: 1/120 s a ISO 400, ajustada a los rangos de caps.SensorInfo; en iOS también
// a un fotograma a la velocidad configurada, para mantenerla.
if (caps.ManualSensorSupported)
{
    controls.SetManualExposure(1_000_000_000L / 120, 400);
}

// Volver a la exposición automática.
controls.LockAutoExposure(false);
```

`ExposureStateChanged` informa de cuándo la exposición busca, ha convergido o está bloqueada. En iOS, una exposición bloqueada o manual sigue bloqueada, como en Android: `SetAeRegions` se aplica cuando la exposición vuelve a ser automática.

## Lo que iOS no admite

La interfaz es la misma en las dos plataformas, y una llamada para la que iOS no tiene control se ignora. `Capabilities` la indica como no admitida: compruébela en lugar de la plataforma.

| Miembro | Android | iOS |
|---|---|---|
| `SetAwbMode` | los modos AWB de Camera2, preajustes incluidos | `Auto`, y `Off` para mantener las ganancias actuales |
| `SetAwbRegions` | sí | no (`MaxAwbRegions` es 0) |
| `SetAeMode` | los modos AE de Camera2 | `On`, y `Off` para mantener la exposición actual |
| `SetFlashMode(Single)` | sí | no, el vídeo usa la linterna |
| `SetVideoStabilization` | `Off`, `On`, `PreviewStabilization` | `Off`, y `On` como estabilización estándar |
| `SetNoiseReduction`, `SetEdgeEnhancement`, `SetOIS`, `SetFaceDetection` | sí | no |
| `Capabilities.HardwareLevel` | el nivel de hardware de Camera2 | no se usa; compruebe `ManualSensorSupported` |
| `Capabilities` | en cuanto se construye el bloque | en cuanto se abre la cámara |

## Véase también

- [Fuentes de Media Blocks SDK](../Sources/index.md)
- [Enumeración de dispositivos](../GettingStarted/device-enum.md)
- [Ejemplos de código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)
