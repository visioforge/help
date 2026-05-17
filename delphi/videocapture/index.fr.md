---
title: Video Capture SDK pour Delphi — guide TVFVideoCapture
description: Capture vidéo/audio en Delphi avec TVFVideoCapture — énumération de périphériques, aperçu, enregistrement MP4/AVI, capture d'écran et caméras IP.
sidebar_label: TVFVideoCapture
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Streaming
primary_api_classes:
  - TVFVideoCapture

---

# Video Capture SDK pour Delphi

[Video Capture SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

Cette section présente l'utilisation du Video Capture SDK avec Delphi pour créer des applications de capture vidéo et audio. Le composant `TVFVideoCapture` (déclaré dans `VideoCaptureMain.pas`) est un descendant de `TCustomPanel` : il fournit donc à la fois la surface d'aperçu et l'ensemble de l'API de capture sous forme de propriétés publiées, de méthodes et d'événements.

## Composant principal

### TVFVideoCapture

`TVFVideoCapture` offre une fonctionnalité complète de capture vidéo et audio. Vous pouvez le déposer sur une fiche depuis la palette de l'IDE ou le créer à l'exécution. Comme il descend de `TCustomPanel`, définissez `Parent` et `Align` comme pour n'importe quel conteneur VCL.

```pascal
var
  VideoCapture1: TVFVideoCapture;
begin
  VideoCapture1 := TVFVideoCapture.Create(Self);
  VideoCapture1.Parent := Panel1;
  VideoCapture1.Align := alClient;
end;
```

## Énumération des périphériques

Remplissez des combo-box avec les périphériques de capture vidéo et audio détectés sur l'hôte. Le SDK expose des paires `_GetCount` / `_GetItem(Index)` pour chaque liste de périphériques.

```pascal
procedure TForm1.EnumerateDevices;
var
  i: Integer;
begin
  // Périphériques de capture vidéo
  cbVideoDevices.Clear;
  for i := 0 to VideoCapture1.Video_CaptureDevices_GetCount - 1 do
    cbVideoDevices.Items.Add(VideoCapture1.Video_CaptureDevices_GetItem(i));

  // Périphériques de capture audio
  cbAudioDevices.Clear;
  for i := 0 to VideoCapture1.Audio_CaptureDevices_GetCount - 1 do
    cbAudioDevices.Items.Add(VideoCapture1.Audio_CaptureDevices_GetItem(i));
end;
```

## Aperçu

Basculez le composant en `Mode_Video_Preview` pour afficher la vidéo en direct sur le panneau sans rien écrire sur le disque. La surveillance audio est contrôlée par `Audio_PlayAudio`.

```pascal
procedure TForm1.StartPreview;
begin
  // Sélection des périphériques par leur nom d'affichage
  VideoCapture1.Video_CaptureDevice := cbVideoDevices.Text;
  VideoCapture1.Audio_CaptureDevice := cbAudioDevices.Text;

  // Mode aperçu (aucune sortie fichier)
  VideoCapture1.Mode := Mode_Video_Preview;
  VideoCapture1.Audio_PlayAudio := True;

  VideoCapture1.Start;
end;

procedure TForm1.StopPreview;
begin
  VideoCapture1.Stop;
end;
```

## Capture vers un fichier

Basculez en `Mode_Video_Capture` et affectez `Output_Filename` et `Output_Format`. L'énumération `TVFOutputFormat` liste tous les conteneurs que le composant peut écrire (`Format_MP4`, `Format_AVI`, `Format_WMV`, `Format_DV`, `Format_WebM`, `Format_FFMPEG`, `Format_FFMPEGX`, etc.).

```pascal
procedure TForm1.StartCapture;
begin
  VideoCapture1.Video_CaptureDevice := cbVideoDevices.Text;
  VideoCapture1.Audio_CaptureDevice := cbAudioDevices.Text;
  VideoCapture1.Audio_RecordAudio := True;

  // Fichier de sortie et conteneur
  VideoCapture1.Output_Filename := 'C:\Videos\capture.mp4';
  VideoCapture1.Output_Format := Format_MP4;

  // Bascule en mode capture
  VideoCapture1.Mode := Mode_Video_Capture;

  VideoCapture1.Start;
end;
```

Vous pouvez changer le fichier de sortie à la volée, sans arrêter le graphe, en appelant `OutputFilename_ChangeOnTheFly`.

## Captures d'image

Enregistrez l'image courante de l'aperçu sur disque dans n'importe laquelle des valeurs `TVFImageFormat` prises en charge (`IM_BMP`, `IM_JPEG`, `IM_PNG`, `IM_GIF`, `IM_TIFF`). Le troisième paramètre est la qualité JPEG (ignoré pour les autres formats).

```pascal
procedure TForm1.TakeSnapshot;
begin
  VideoCapture1.Frame_Save('C:\Photos\snapshot.jpg', IM_JPEG, 85);
end;
```

Pour récupérer directement l'image dans un `TBitmap` (par exemple pour l'afficher dans un `TImage`), utilisez `Frame_GetCurrent` :

```pascal
procedure TForm1.TakeSnapshotToMemory;
var
  Bitmap: TBitmap;
begin
  Bitmap := TBitmap.Create;
  try
    VideoCapture1.Frame_GetCurrent(Bitmap);
    Image1.Picture.Assign(Bitmap);
  finally
    Bitmap.Free;
  end;
end;
```

## Caméras IP (RTSP / HTTP)

Basculez le mode en `Mode_IP_Preview` ou `Mode_IP_Capture` et renseignez `IP_Camera_URL`. Utilisez `IP_Camera_Type` pour choisir le moteur et le protocole — par exemple `IP_RTSP_TCP` pour du RTSP sur TCP via le moteur FFmpeg, ou `IP_Auto_VLC` pour basculer sur la source VLC embarquée.

```pascal
procedure TForm1.ConnectIPCamera;
begin
  VideoCapture1.IP_Camera_URL := 'rtsp://user:password@192.168.1.100:554/stream';
  VideoCapture1.IP_Camera_Type := IP_RTSP_TCP;
  VideoCapture1.Mode := Mode_IP_Preview;
  VideoCapture1.Start;
end;
```

## Capture d'écran

Le composant enregistre le bureau, un écran individuel, une région ou une fenêtre. Définissez le rectangle de capture (ou activez le plein écran), choisissez une fréquence d'images, puis basculez en `Mode_Screen_Capture`.

```pascal
procedure TForm1.StartScreenCapture;
begin
  // Capture plein écran à 30 fps avec le curseur de la souris inclus
  VideoCapture1.Screen_Capture_FullScreen := True;
  VideoCapture1.Screen_Capture_FrameRate := 30.0;
  VideoCapture1.Screen_Capture_Grab_Mouse_Cursor := True;

  VideoCapture1.Output_Filename := 'C:\Videos\screen.mp4';
  VideoCapture1.Output_Format := Format_MP4;

  VideoCapture1.Mode := Mode_Screen_Capture;
  VideoCapture1.Start;
end;
```

Pour une capture de région, désactivez `Screen_Capture_FullScreen` et fournissez `Screen_Capture_Left`, `Screen_Capture_Top`, `Screen_Capture_Right` et `Screen_Capture_Bottom` (tous en pixels). Voir [Capture d'écran](screen-capture.md) pour l'ensemble complet des options.

## Effets vidéo

Les effets (luminosité, contraste, saturation, retournement, flou, niveaux de gris, sépia et bien d'autres) sont gérés par `Video_Effect_Ex`. Activez d'abord le pipeline d'effets avec `Video_Effects_Enabled := True`, puis ajoutez chaque effet par sa valeur `TVFEffectType`. L'argument `Amount` est l'intensité de l'effet — pour `ef_contrast` il décale le contraste, pour `ef_flip_right` il sert d'indicateur d'activation, etc.

```pascal
procedure TForm1.ApplyVideoEffects;
begin
  VideoCapture1.Video_Effects_Enabled := True;

  // ID=1, appliqué sur tout le graphe (StartTime=0, StopTime=0), activé,
  // effet de contraste avec intensité = 20
  VideoCapture1.Video_Effect_Ex(1, 0, 0, True, ef_contrast, 20.0, '');

  // ID=2, retournement horizontal
  VideoCapture1.Video_Effect_Ex(2, 0, 0, True, ef_flip_right, 0.0, '');
end;
```

Supprimez des effets individuels avec `Video_Effects_Remove(ID)` ou videz la liste complète avec `Video_Effects_Clear`.

## Incrustation de texte

Utilisez `Video_Effects_Text_Logo` (incrustation GDI historique) ou `Video_Effects_Text_Logo_Plus` (incrustation GDI+ moderne avec dégradés, rotation et effets de contour) pour incruster du texte dans le flux vidéo. L'exemple ci-dessous emploie `Video_Effects_Text_Logo_Plus`.

```pascal
procedure TForm1.AddTextOverlay;
begin
  VideoCapture1.Video_Effects_Enabled := True;

  // ID=10, durée totale (0..0), activé, « My Video » en (10, 10),
  // Arial 24, sans gras/italique/souligné/barré, couleur blanche
  VideoCapture1.Video_Effects_Text_Logo_Plus(
    10, 0, 0, True, 'My Video', 10, 10,
    'Arial', 24, False, False, False, False, clWhite);
end;
```

## Événements

`TVFVideoCapture` déclenche trois événements de cycle de vie principaux : `OnStart`, `OnStop` et `OnError`. Aucun ne porte de paramètre `Sender` — `OnError` reçoit le message d'erreur sous forme de `WideString`.

```pascal
procedure TForm1.VideoCapture1Start;
begin
  Button1.Caption := 'Arrêter';
end;

procedure TForm1.VideoCapture1Stop;
begin
  Button1.Caption := 'Démarrer';
end;

procedure TForm1.VideoCapture1Error(ErrorText: WideString);
begin
  ShowMessage('Erreur de capture : ' + ErrorText);
end;
```

D'autres événements couvrent l'accès en direct aux trames (`OnVideoFrame`, `OnAudioFrame`), la détection de mouvement (`OnMotion`), l'activité souris et clavier sur la surface d'aperçu, les valeurs des VU-mètres, les événements de transport DV et la recherche de canaux du tuner TV.

## Ressources de développement

Pour des conseils d'implémentation détaillés, explorez ces ressources essentielles :

- [Journal des modifications et historique des versions](changelog.md)
- [Guide d'installation et de configuration](install/index.md)
- [Meilleures pratiques de déploiement](deployment.md)
- [Informations de licence et CLUF](../../eula.md)
- [Documentation API complète](https://api.visioforge.org/delphi/video_capture_sdk/index.html)

## Tutoriels d'implémentation

### Enregistrement et traitement audio

Maîtrisez la capture audio grâce à ces guides pas à pas :

- [Capture audio MP3](audio-capture-mp3.md) — Apprenez à capturer des flux audio et à les encoder directement au format MP3 avec des débits binaires et des paramètres de qualité configurables.
- [Enregistrement audio WAV avec options de compression](audio-capture-wav.md) — Implémentez un enregistrement audio WAV de haute qualité avec codecs de compression optionnels et configurations de format.
- [Configuration des périphériques de sortie audio](audio-output.md) — Guide pour sélectionner et configurer les périphériques de sortie audio pour la surveillance et la lecture dans vos applications.

### Capture vidéo et contrôle des périphériques

Apprenez les techniques essentielles de manipulation vidéo :

- [Capture vidéo AVI](video-capture-avi.md) — Développez des applications qui capturent des flux vidéo au format AVI avec des codecs et des paramètres de conteneur personnalisables.
- [Contrôle et intégration des caméscopes DV](dv-camcorder.md) — Connectez et contrôlez les caméscopes DV via FireWire/IEEE-1394 avec contrôles de transport et gestion des métadonnées.
- [Sélection des sources vidéo et audio](video-audio-sources.md) — Techniques pour énumérer, sélectionner et gérer plusieurs périphériques de capture dans vos applications.
- [Paramètres matériels d'ajustement vidéo](hardware-adjustments.md) — Accédez et modifiez les paramètres au niveau du périphérique, notamment la luminosité, le contraste, la saturation et la balance des blancs.
- [Configuration de l'entrée vidéo via Crossbar](video-input-crossbar.md) — Apprenez à configurer le routage d'entrée vidéo à travers les interfaces crossbar pour les périphériques de capture multi-entrées.
- [Sélection et configuration du moteur de rendu vidéo](video-renderer.md) — Choisissez et configurez le moteur de rendu vidéo optimal pour votre application de capture.

### Techniques média avancées

Explorez des scénarios d'implémentation sophistiqués :

- [Configuration de format de sortie personnalisé](custom-output.md) — Créez des formats de sortie spécialisés avec des paramètres de compression et des configurations de conteneur sur mesure.
- [Intégration de radio FM et de tuner TV](fm-radio-tv-tuning.md) — Implémentez la réception radio FM et la syntonisation de chaînes TV dans les applications dotées du matériel pris en charge.
- [Diffusion réseau au format WMV](network-streaming-wmv.md) — Diffusez la vidéo capturée sur les réseaux en utilisant le format Windows Media Video avec optimisation de la bande passante.
- [Gestion de la résolution avec redimensionnement et rognage](resize-crop.md) — Traitez les images vidéo avec redimensionnement et rognage dynamiques pour obtenir des dimensions de sortie personnalisées.
- [Capture d'écran](screen-capture.md) — Capturez le contenu à l'écran avec des fréquences d'images et des capacités de sélection de région configurables.
- [Capture de fichier DV avec options de compression](video-capture-dv.md) — Enregistrez la vidéo directement au format DV ou avec recompression pour des besoins de stockage optimisés.
- [Capture MPEG-2 avec intégration de tuner TV](mpeg2-capture.md) — Utilisez les encodeurs MPEG-2 matériels des tuners TV pour une capture broadcast efficace et de haute qualité.
- [Capture Windows Media Video avec profils externes](video-capture-wmv.md) — Implémentez l'encodage Windows Media Video avec des configurations de profil externes pour une qualité et une taille optimisées.
