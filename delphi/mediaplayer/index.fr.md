---
title: Media Player SDK pour Delphi — Guide du développeur
description: Lecture vidéo et audio en Delphi avec TVFMediaPlayer — contrôle de lecture, recherche, volume, capture d'image, pistes audio et plein écran.
sidebar_label: TVFMediaPlayer
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Playback
  - Streaming
primary_api_classes:
  - TVFMediaPlayer

---

# Media Player SDK pour Delphi

[Media Player SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

Cette page décrit l'utilisation de Media Player SDK avec Delphi pour créer des applications de lecture multimédia autour du composant VCL `TVFMediaPlayer`.

## Composant principal

### TVFMediaPlayer

`TVFMediaPlayer` est un descendant de `TCustomPanel` qui expose l'intégralité de l'API de lecture. Déposez-le sur une fiche au moment de la conception, ou créez-le par programme et attachez-le à un panneau hôte.

```pascal
var
  MediaPlayer1: TVFMediaPlayer;
begin
  MediaPlayer1 := TVFMediaPlayer.Create(Self);
  MediaPlayer1.Parent := Panel1;
  MediaPlayer1.Align := alClient;
end;
```

## Lecture de base

### Lire un fichier

Définissez `FilenameOrURL` sur le chemin d'entrée (ou l'URL réseau), choisissez un moteur source via `Source_Mode`, puis appelez `Play`.

```pascal
procedure TForm1.PlayFile(const Filename: WideString);
begin
  MediaPlayer1.FilenameOrURL := Filename;
  MediaPlayer1.Source_Mode := SM_File_FFMPEG; // SM_File_DS, SM_File_VLC, SM_File_LAV
  MediaPlayer1.Audio_Play := True;
  MediaPlayer1.Play;
end;
```

### Commandes de lecture

`Play` renvoie `True` en cas de succès. `Pause`, `Resume` et `Stop` sont des procédures.

```pascal
procedure TForm1.btnPlayClick(Sender: TObject);
begin
  MediaPlayer1.Play;
end;

procedure TForm1.btnPauseClick(Sender: TObject);
begin
  MediaPlayer1.Pause;
end;

procedure TForm1.btnResumeClick(Sender: TObject);
begin
  MediaPlayer1.Resume;
end;

procedure TForm1.btnStopClick(Sender: TObject);
begin
  MediaPlayer1.Stop;
end;
```

L'état actuel est exposé par la propriété en lecture seule `Status` (`ST_PLAY`, `ST_PAUSE`, `ST_FREE`).

```pascal
if MediaPlayer1.Status = ST_PLAY then
  StatusBar1.SimpleText := 'Lecture en cours';
```

## Recherche de position

La position est exprimée en millisecondes. `Position_Get_Time` renvoie la position actuelle, `Position_Set_Time` se déplace vers une position absolue, et `Info_Video_DurationMSec(0)` renvoie la durée totale du premier flux vidéo.

```pascal
procedure TForm1.SeekTo(PositionMs: Integer);
begin
  MediaPlayer1.Position_Set_Time(PositionMs);
end;

function TForm1.GetDurationMs: Integer;
begin
  Result := MediaPlayer1.Info_Video_DurationMSec(0);
end;

function TForm1.GetPositionMs: Integer;
begin
  Result := MediaPlayer1.Position_Get_Time;
end;
```

La recherche précise à l'image près est également disponible via `Position_Get_Frame` et `Position_Set_Frame`.

## Contrôle audio

`TVFMediaPlayer` prend en charge jusqu'à huit flux de sortie audio indépendants. Le volume et la balance sont adressés par un index de flux à base zéro.

```pascal
procedure TForm1.SetVolume(StreamIndex, Volume: Integer);
begin
  // Plage de volume : 0..100 (le SDK met à l'échelle en interne)
  MediaPlayer1.Audio_Volume_Set(StreamIndex, Volume);
end;

function TForm1.GetVolume(StreamIndex: Integer): Integer;
begin
  Result := MediaPlayer1.Audio_Volume_Get(StreamIndex);
end;

procedure TForm1.SetBalance(StreamIndex, Balance: Integer);
begin
  // Plage de balance : -100 (gauche) à +100 (droite)
  MediaPlayer1.Audio_Balance_Set(StreamIndex, Balance);
end;
```

Pour couper le son, mettez le volume à zéro et restaurez la valeur précédente lors de la réactivation, ou basculez `Audio_Play` avant de démarrer la lecture.

```pascal
procedure TForm1.Mute;
begin
  FSavedVolume := MediaPlayer1.Audio_Volume_Get(0);
  MediaPlayer1.Audio_Volume_Set(0, 0);
end;

procedure TForm1.Unmute;
begin
  MediaPlayer1.Audio_Volume_Set(0, FSavedVolume);
end;
```

## Vitesse de lecture

`SetSpeed` accepte un multiplicateur compris entre 0,01 et 100,0.

```pascal
procedure TForm1.SetPlaybackSpeed(Rate: Double);
begin
  // Rate : 0.5 (demi-vitesse) à 2.0 (vitesse double)
  MediaPlayer1.SetSpeed(Rate);
end;
```

## Lecture d'URL réseau

La même propriété `FilenameOrURL` accepte les URL réseau. Choisissez le moteur qui prend le mieux en charge le protocole — `SM_File_VLC` et `SM_File_FFMPEG` couvrent le plus large éventail de sources de streaming ; `SM_MMS_WMV_DS` est dédié aux flux MMS/WMV.

```pascal
procedure TForm1.PlayURL(const URL: WideString);
begin
  MediaPlayer1.FilenameOrURL := URL;
  MediaPlayer1.Source_Mode := SM_File_FFMPEG;
  MediaPlayer1.Play;
end;

procedure TForm1.PlayRTSP;
begin
  MediaPlayer1.FilenameOrURL := 'rtsp://192.168.1.100:554/stream';
  MediaPlayer1.Source_Mode := SM_File_FFMPEG;
  MediaPlayer1.Play;
end;

procedure TForm1.PlayHLS;
begin
  MediaPlayer1.FilenameOrURL := 'https://server.example.com/playlist.m3u8';
  MediaPlayer1.Source_Mode := SM_File_FFMPEG;
  MediaPlayer1.Play;
end;
```

## Capture d'image

`Frame_Save` enregistre l'image actuelle sur le disque dans le format d'image choisi. `Frame_GetCurrent` remplit un `TBitmap` que vous fournissez.

```pascal
procedure TForm1.CaptureFrame;
begin
  // Frame_Save(Filename, Format, Quality)
  MediaPlayer1.Frame_Save('C:\Snapshots\frame.jpg', IM_JPEG, 85);
end;

procedure TForm1.CaptureFrameToBitmap;
var
  Bitmap: TBitmap;
begin
  Bitmap := TBitmap.Create;
  try
    MediaPlayer1.Frame_GetCurrent(Bitmap);
    Image1.Picture.Assign(Bitmap);
  finally
    Bitmap.Free;
  end;
end;
```

Vous pouvez éventuellement redimensionner l'image enregistrée en configurant `Frame_Save_Resize`, `Frame_Save_Resize_Width` et `Frame_Save_Resize_Height` avant l'appel.

## Pistes audio et sous-titres

Les helpers `Info_Audio_*` et `Info_Text_*` énumèrent les flux découverts à l'intérieur du fichier source. Les flux audio sont activés ou désactivés individuellement avec `Audio_SetStream`.

```pascal
procedure TForm1.PopulateAudioTracks;
var
  i: Integer;
begin
  cbAudioTracks.Items.Clear;
  for i := 0 to MediaPlayer1.Info_Audio_Streams_Count - 1 do
    cbAudioTracks.Items.Add(MediaPlayer1.Info_Audio_Codec(i));
end;

procedure TForm1.SelectAudioTrack(Index: Integer);
var
  i: Integer;
begin
  // Activer la piste choisie et désactiver les autres
  for i := 0 to MediaPlayer1.Info_Audio_Streams_Count - 1 do
    MediaPlayer1.Audio_SetStream(i, i = Index);
end;

procedure TForm1.PopulateSubtitles;
var
  i: Integer;
begin
  cbSubtitles.Items.Clear;
  for i := 0 to MediaPlayer1.Info_Text_Streams_Count - 1 do
    cbSubtitles.Items.Add(MediaPlayer1.Info_Text_Name(i));
end;
```

`Info_Text_Language(i)` et `Info_Text_Codec(i)` sont également disponibles pour obtenir des métadonnées de sous-titres plus riches.

## Mode plein écran

Basculez le mode plein écran via la propriété `Screen_VR_FullScreen`. Cela fonctionne avec le rendu vidéo configuré.

```pascal
procedure TForm1.ToggleFullscreen;
begin
  MediaPlayer1.Screen_VR_FullScreen := not MediaPlayer1.Screen_VR_FullScreen;
end;
```

## Événements

`TVFMediaPlayer` expose trois événements de lecture principaux. `OnStart` et `OnStop` ne prennent aucun paramètre ; `OnError` reçoit le texte d'erreur.

```pascal
procedure TForm1.MediaPlayer1Start;
begin
  StatusBar1.SimpleText := 'Lecture en cours';
end;

procedure TForm1.MediaPlayer1Stop;
begin
  StatusBar1.SimpleText := 'Arrêté';
end;

procedure TForm1.MediaPlayer1Error(ErrorText: WideString);
begin
  ShowMessage('Error: ' + ErrorText);
end;
```

Pour suivre les mises à jour de position, interrogez `Position_Get_Time` depuis un `TTimer` tant que `Status = ST_PLAY` :

```pascal
procedure TForm1.PositionTimerTick(Sender: TObject);
var
  Position, Duration: Integer;
begin
  if MediaPlayer1.Status <> ST_PLAY then
    Exit;

  Position := MediaPlayer1.Position_Get_Time;
  Duration := MediaPlayer1.Info_Video_DurationMSec(0);
  if Duration > 0 then
  begin
    TrackBar1.Max := Duration;
    TrackBar1.Position := Position;
  end;
  LabelPosition.Caption := Format('%d:%.2d / %d:%.2d',
    [Position div 60000, (Position div 1000) mod 60,
     Duration div 60000, (Duration div 1000) mod 60]);
end;
```

## Formats pris en charge

| Type | Formats |
|------|---------|
| Vidéo | MP4, AVI, MKV, MOV, WMV, FLV, WebM |
| Audio | MP3, AAC, FLAC, WAV, OGG, WMA |
| Streaming | RTSP, RTMP, HTTP, HLS, DASH |

La couverture des formats dépend du `Source_Mode` sélectionné — les moteurs FFmpeg et VLC couvrent dès le départ la plage la plus large ; DirectShow (`SM_File_DS`) s'appuie sur les codecs installés sur le système.

## Ressources et informations complémentaires

Pour explorer plus en profondeur les capacités et l'utilisation de la bibliothèque `TVFMediaPlayer`, consultez les ressources officielles suivantes :

* **Page produit :** [VisioForge Media Player SDK](https://www.visioforge.com/all-in-one-media-framework)
* **Référence de l'API :** [Référence de l'API Delphi Media Player](https://api.visioforge.org/delphi/media_player_sdk/index.html)
* **Journal des modifications :** [Mises à jour et correctifs récents](changelog.md)
* **Guide d'installation :** [Configuration de la bibliothèque](install/index.md)
* **Déploiement :** [Distribution de votre application](deployment.md)
* **Contrat de licence :** [Contrat de licence utilisateur final](../../eula.md)

## Tutoriels et exemples de code

Exemples pratiques illustrant comment implémenter des fonctionnalités spécifiques :

* [Comment lire un fichier vidéo contenant plusieurs flux vidéo ?](file-multiple-video-streams.md)
* *(D'autres tutoriels seront ajoutés ici au fur et à mesure de leur disponibilité)*
