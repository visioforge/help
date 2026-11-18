---
title: Audio bidirectionnel avec caméras IP en C# .NET (ONVIF)
description: Envoyez le son du micro vers le haut-parleur d'une caméra IP via le backchannel audio RTSP ONVIF en C# pour interphones, portiers et sonorisation.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - VideoCaptureCoreX
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - IP Camera
  - RTSP
  - ONVIF
  - Audio
  - C#
primary_api_classes:
  - RTSPSourceSettings
  - RTSPSourceBlock
  - VideoCaptureCoreX
  - ONVIFClientX

---

# Audio bidirectionnel avec les caméras IP (backchannel audio ONVIF)

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

De nombreuses caméras IP, sonnettes vidéo et interphones ont un haut-parleur intégré ou une sortie audio. Le SDK peut envoyer du son vers ce haut-parleur via le **backchannel audio RTSP ONVIF** tout en recevant la vidéo et l'audio de la caméra dans la même session RTSP. Usages typiques : parler à un visiteur à la porte, une annonce de sonorisation par une caméra ou un interphone bidirectionnel.

## Fonctionnement

Le backchannel audio ONVIF est une piste audio `sendonly` dans la description RTSP (SDP) de la caméra. Lorsque `RTSPSourceSettings.AudioBackchannel` vaut `true`, le SDK demande le backchannel (`Require: www.onvif.org/ver20/backchannel`), trouve cette piste et y envoie votre audio en RTP par le transport que la session utilise déjà (TCP ou UDP).

```mermaid
flowchart LR
    Mic[Micro ou autre source audio] --> BC["RTSPSourceBlock.BackchannelInput"]
    BC -->|RTP G.711| Cam[Haut-parleur de la caméra IP]
    Cam -->|H.264/H.265 + audio| Src[Sorties de RTSPSourceBlock]
```

- L'audio est envoyé en **G.711 mu-law (PCMU), 8000 Hz, mono**, le codec de backchannel qu'ONVIF impose à toute caméra dotée d'une sortie audio. Une caméra dont le backchannel propose PCMA/8000 mais pas PCMU/8000 reçoit du **G.711 A-law (PCMA)**. Tout format d'entrée est converti.
- La caméra doit proposer un backchannel. Les caméras ONVIF Profile S et Profile T avec sortie audio le proposent. Une caméra qui ne prend pas en charge l'en-tête `Require` d'ONVIF peut refuser la connexion : activez donc `AudioBackchannel` uniquement pour les caméras avec haut-parleur.
- Le backchannel nécessite GStreamer 1.22 ou plus récent. Les paquets runtime du SDK pour Windows, macOS, Android et iOS l'incluent ; sous Linux, où le GStreamer du système est utilisé, cela signifie Ubuntu 24.04 ou une autre distribution en 1.22 ou plus. Avec un runtime plus ancien, le SDK journalise une erreur qui l'indique et n'envoie pas d'audio ; la vidéo et l'audio de la caméra continuent d'être lus.
- L'audio est envoyé au moment de sa capture : la caméra le reçoit sans que le tampon de réception (`RTSPSourceSettings.Latency`) s'y ajoute.
- Si le backchannel de la caméra ne liste ni PCMU/8000 ni PCMA/8000, le SDK journalise une erreur indiquant les codecs proposés par la caméra, et la vidéo et l'audio de la caméra continuent d'être lus.

## Video Capture SDK .NET (VideoCaptureCoreX)

Affectez à `Audio_BackchannelSource` l'audio à envoyer. La propriété ouvre le backchannel de la source RTSP ; vous n'avez pas à activer `AudioBackchannel`, et vos `RTSPSourceSettings` ne sont pas modifiés. Elle accepte les mêmes types de source que `Audio_Source`, créés comme une source distincte : n'affectez donc pas le même périphérique de capture aux deux.

```cs
var core = new VideoCaptureCoreX(VideoView1 as IVideoView);

// La caméra.
var rtsp = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/cam/realmonitor?channel=1&subtype=0"),
    "admin", "password", audioEnabled: true);
core.Video_Source = rtsp;

// Le son de la caméra, lu sur les haut-parleurs par défaut du PC.
var outputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var speakers = outputs.FirstOrDefault(device => device.IsDefault) ?? outputs.FirstOrDefault();
if (speakers != null) core.Audio_OutputDevice = new AudioRendererSettings(speakers);

// Le micro qui parle dans le haut-parleur de la caméra.
var mic = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
core.Audio_BackchannelSource = mic.CreateSourceSettingsVC();

await core.StartAsync();
```

`Audio_Source` reste indépendant : lorsqu'il vaut `null`, le micro de la caméra est toujours lu sur `Audio_OutputDevice` et enregistré par les sorties, vous entendez donc le visiteur pendant qu'il vous entend.

## Media Blocks SDK .NET (RTSPSourceBlock)

Avec `AudioBackchannel` activé, `RTSPSourceBlock` possède une entrée audio, `BackchannelInput`. Connectez-y n'importe quel bloc audio avant le démarrage.

```cs
var pipeline = new MediaBlocksPipeline();

var rtsp = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/cam/realmonitor?channel=1&subtype=0"),
    "admin", "password", audioEnabled: true);
rtsp.AudioBackchannel = true;

var camera = new RTSPSourceBlock(rtsp);

// Vidéo et audio de la caméra vers ce PC.
pipeline.Connect(camera.VideoOutput, new VideoRendererBlock(pipeline, VideoView1).Input);
pipeline.Connect(camera.AudioOutput, new AudioRendererBlock().Input);

// Micro vers le haut-parleur de la caméra.
var mic = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
var micSource = new SystemAudioSourceBlock(mic.CreateSourceSettings());
pipeline.Connect(micSource.Output, camera.BackchannelInput);

await pipeline.StartAsync();
```

L'entrée du backchannel accepte n'importe quel bloc audio : un fichier via `UniversalSourceBlock` pour une annonce enregistrée, ou `VirtualAudioSourceBlock` pour un son de test. L'audio est envoyé en temps réel.

Si `BackchannelInput` reste non connecté, aucun backchannel n'est demandé et le bloc se comporte comme sans `AudioBackchannel`. Le backchannel est pris en charge par `RTSPSourceBlock` ; `FallbackSwitchSourceBlock` (et `FallbackSwitch` dans `VideoCaptureCoreX`) ne l'ouvre pas.

## Volume du haut-parleur via ONVIF

`ONVIFClientX` lit et définit la configuration de la sortie audio de la caméra, y compris le volume du haut-parleur (`OutputLevel`).

```cs
var onvif = new ONVIFClientX();
await onvif.ConnectAsync("http://192.168.1.22/onvif/device_service", "admin", "password");

var outputs = await onvif.GetAudioOutputsAsync();            // vide : la caméra n'a pas de haut-parleur
var configs = await onvif.GetAudioOutputConfigurationsAsync();
if (configs?.Length > 0)
{
    var config = configs[0];
    var options = await onvif.GetAudioOutputConfigurationOptionsAsync(config.token);

    config.OutputLevel = options.OutputLevelRange.Max;       // le plus fort
    await onvif.SetAudioOutputConfigurationAsync(config);
}
```

`GetAudioOutputsAsync` sert aussi de vérification rapide avant d'activer le backchannel : une caméra qui ne signale aucune sortie audio n'a pas de haut-parleur vers lequel envoyer.

## Dépannage

- **La connexion échoue après l'activation d'`AudioBackchannel`** : la caméra refuse la demande de backchannel ONVIF. Vérifiez qu'elle possède une sortie audio (`GetAudioOutputsAsync`) et que l'audio bidirectionnel est activé dans son interface web.
- **Aucun son dans le haut-parleur de la caméra** : consultez dans le journal d'erreurs les codecs proposés par la caméra, augmentez `OutputLevel` et vérifiez qu'aucun autre client (application du fabricant ou NVR) n'occupe le canal de conversation de la caméra.
- **Le micro de la caméra ne capte pas son propre haut-parleur** : c'est normal. Les caméras dotées d'un haut-parleur l'annulent dans leur micro (annulation d'écho, ou un `SendPrimacy` semi-duplex comme `www.onvif.org/ver20/HalfDuplex/Auto` qui coupe le micro pendant que le haut-parleur joue), vous ne vous entendez donc pas vous-même. Vérifiez le haut-parleur à l'oreille.

## Documentation associée

- [Tutoriel audio bidirectionnel avec caméra IP](../../video-tutorials/ip-camera-two-way-audio.md) : une application WPF complète, aperçu de la caméra avec son son et micro du PC vers le haut-parleur de la caméra
- [Flux de caméras RTSP](rtsp.md) : connexion, transport et réglages de faible latence
- [Intégration des caméras IP ONVIF](onvif.md) : découverte, profils et contrôle PTZ
- [Annuaire des marques de caméras IP](../../../camera-brands/index.md) : URL RTSP de chaque marque
