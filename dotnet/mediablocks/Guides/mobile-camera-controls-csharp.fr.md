---
title: Zoom, torche, mise au point et exposition caméra en C#
description: "Caméra iOS et Android en C# .NET : zoom ultra grand-angle 0,5x, torche, mise au point au toucher et exposition manuelle avec Media Blocks SDK."
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

# Zoom, torche, mise au point et exposition sur iOS et Android

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

`SystemVideoSourceBlock` donne à la caméra d'un téléphone en marche une seule interface de contrôle sur les deux plateformes : `Camera2Controls` (`ICamera2Controls`) modifie le zoom, la torche, la mise au point, l'exposition, la balance des blancs et la stabilisation pendant que le pipeline tourne, et `Capabilities` (`Camera2Capabilities`) indique ce que la caméra ouverte prend en charge. Sur Android, les membres pilotent l'API Camera2 ; sur iOS, ils pilotent l'`AVCaptureDevice` que la source a ouvert. Le même code MAUI ou iOS/Android fonctionne sur les deux.

## Obtenir les contrôles

Lisez-les après que le pipeline a démarré la caméra : avant, `Camera2Controls` vaut `null`, et sur iOS `Capabilities` aussi. Sur iOS, chaque démarrage crée de nouveaux contrôles et remet la caméra dans son état d'ouverture, comme Android - mise au point, exposition et balance des blancs automatiques au centre de l'image, sans compensation, le zoom 1,0 sur un appareil multi-caméras et le champ le plus large d'un objectif seul, la torche éteinte : relisez `Camera2Controls` et réabonnez-vous à ses événements après chaque `StartAsync`.

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
// ... reliez la source à un rendu ou à un encodeur, puis :
await pipeline.StartAsync();

ICamera2Controls controls = videoSource.Camera2Controls;
Camera2Capabilities caps = videoSource.Capabilities;
```

## Zoom et l'objectif ultra grand-angle 0,5x

`SetZoom` prend le rapport sur l'échelle qu'affiche l'app Appareil photo de l'iPhone, de `MinZoom` à `MaxZoom` ; une valeur hors de cette plage est ramenée à la borne.

```csharp
controls.SetZoom(controls.MinZoom);   // le champ le plus large de la caméra
controls.SetZoom(2.0f);               // 2x
```

- **Android :** `MinZoom` est le minimum propre à la caméra : inférieur à 1,0 sur Android 11 et versions ultérieures lorsque la caméra atteint son ultra grand-angle par le rapport de zoom, sinon 1,0. `MaxZoom` est le zoom maximal de la caméra.
- **iOS :** sur un appareil multi-caméras doté d'un ultra grand-angle - « Back Triple Camera » ou « Back Dual Wide Camera » - `MinZoom` vaut 0,5 : 0,5 est l'ultra grand-angle, 1,0 le grand-angle, et iOS passe de lui-même au téléobjectif quand le zoom le dépasse. Sur « Back Camera » et « Front Camera », il vaut 1,0, et un objectif ouvert seul indique sa place sur cette échelle : lisez `MinZoom` plutôt que de supposer 1,0. Un appareil multi-caméras s'ouvre sur le grand-angle, 1,0, comme l'app Appareil photo.

Sur iOS, `VideoSourcesAsync` liste d'abord « Back Camera » et « Front Camera », comme avant, puis les autres caméras arrière du téléphone : l'ultra grand-angle, le téléobjectif et les appareils multi-caméras (« Back Dual Wide Camera », « Back Triple Camera », « Back Dual Camera »). Leurs noms viennent d'iOS et suivent la langue du téléphone : laissez l'utilisateur choisir dans la liste plutôt que de comparer un nom.

## Torche

```csharp
if (caps.HasFlashUnit)
{
    controls.SetTorchMode(true);
}
```

## Mise au point

```csharp
controls.SetAfMode(Camera2AfMode.ContinuousVideo);

// Mise au point au toucher : une région du cadre du capteur, ici 100 x 100 autour d'un point
// donné en fractions 0-1 (fx, fy) de ce cadre.
var sensor = caps.SensorInfo;
var x = (int)(fx * sensor.ActiveWidth) - 50;
var y = (int)(fy * sensor.ActiveHeight) - 50;
controls.SetAfRegions(new[] { new Camera2MeteringRegion(x, y, 100, 100, 1000) });
controls.TriggerAutoFocus();

// Mise au point manuelle : 0 est la distance la plus lointaine, caps.LensInfo.MinimumFocusDistance la plus proche.
controls.SetAfMode(Camera2AfMode.Off);
controls.SetFocusDistance(caps.LensInfo.MinimumFocusDistance / 2);
```

Le cadre du capteur est le cadre paysage propre à la caméra sur les deux plateformes, pas l'aperçu tourné : convertissez un toucher sur un aperçu en portrait avant de construire la région. `FocusStateChanged` signale le balayage. Sur iOS :

- le centre de la première région est le point de mise au point, et le poids n'est pas utilisé ;
- `SensorInfo.Orientation` vaut 0, et le cadre du capteur est le cadre paysage avec le côté du bouton principal à droite, quelle que soit la tenue du téléphone. Un toucher en (fx, fy), en fractions 0-1 d'un aperçu portrait de la caméra arrière qui montre tout le cadre (retirez d'abord le recadrage d'un aperçu en remplissage), se trouve en (fy, 1 - fx) du cadre du capteur ; pour un aperçu miroir de la caméra avant, inversez d'abord fx ;
- `FocusStateChanged` indique quand un balayage se termine, pas s'il a trouvé la netteté : `IsFocused` n'y est pas une mesure ;
- la distance de mise au point n'est pas étalonnée : elle est ordonnée, mais pas en mètres ;
- `MinimumFocusDistance` vaut 0 quand la caméra ne peut pas régler la position de son objectif.

## Exposition

```csharp
// Un diaphragme plus sombre ; un pas vaut caps.ExposureCompensationStep IL (un tiers d'IL sur iOS).
controls.SetExposureCompensation((int)Math.Round(-1 / caps.ExposureCompensationStep));

// Point d'exposition, sous la même forme de région que la mise au point.
controls.SetAeRegions(new[] { new Camera2MeteringRegion(x, y, 100, 100, 1000) });

// Exposition manuelle : 1/120 s à ISO 400, ramenée aux plages de caps.SensorInfo ; sur iOS aussi
// à une image à la cadence configurée, pour garder la cadence.
if (caps.ManualSensorSupported)
{
    controls.SetManualExposure(1_000_000_000L / 120, 400);
}

// Retour à l'exposition automatique.
controls.LockAutoExposure(false);
```

`ExposureStateChanged` signale quand l'exposition cherche, a convergé ou est verrouillée. Sur iOS, une exposition verrouillée ou manuelle reste verrouillée, comme sur Android : `SetAeRegions` s'applique une fois l'exposition redevenue automatique.

## Ce qu'iOS ne prend pas en charge

L'interface est la même sur les deux plateformes, et un appel pour lequel iOS n'a pas de contrôle est ignoré. `Capabilities` le signale comme non pris en charge : testez-le plutôt que la plateforme.

| Membre | Android | iOS |
|---|---|---|
| `SetAwbMode` | les modes AWB de Camera2, préréglages compris | `Auto`, et `Off` pour garder les gains actuels |
| `SetAwbRegions` | oui | non (`MaxAwbRegions` vaut 0) |
| `SetAeMode` | les modes AE de Camera2 | `On`, et `Off` pour garder l'exposition actuelle |
| `SetFlashMode(Single)` | oui | non, la vidéo utilise la torche |
| `SetVideoStabilization` | `Off`, `On`, `PreviewStabilization` | `Off`, et `On` comme stabilisation standard |
| `SetNoiseReduction`, `SetEdgeEnhancement`, `SetOIS`, `SetFaceDetection` | oui | non |
| `Capabilities.HardwareLevel` | le niveau matériel Camera2 | non utilisé ; testez `ManualSensorSupported` |
| `Capabilities` | dès que le bloc est construit | dès que la caméra est ouverte |

## Voir aussi

- [Sources de Media Blocks SDK](../Sources/index.md)
- [Énumération des périphériques](../GettingStarted/device-enum.md)
- [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)
