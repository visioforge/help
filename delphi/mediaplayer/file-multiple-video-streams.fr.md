---
title: Lire plusieurs flux vidéo avec le lecteur SDK Delphi
description: Gérez plusieurs flux vidéo dans des fichiers — sélectionnez les angles, changez de résolutions, gérez les pistes avec des exemples Delphi, C++ et VB6.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - C++
  - Windows
  - VCL
  - Playback
  - MKV
primary_api_classes:
  - TVFMediaPlayer
  - CVFMediaPlayer

---

# Lire des fichiers vidéo contenant plusieurs flux vidéo

## Comprendre les flux vidéo multiples

### Qu'est-ce que des flux vidéo multiples ?

Les flux vidéo multiples désignent différentes pistes vidéo contenues dans un même fichier multimédia. Ces flux peuvent varier de plusieurs façons :

- Différents angles de caméra de la même scène
- Versions alternatives avec des résolutions ou des débits binaires différents
- Contenu principal et secondaire (par exemple image dans l'image)
- Différents formats d'image ou formats du même contenu
- Versions avec ou sans effets spéciaux ou graphiques

### Formats de fichier pris en charge

De nombreux formats de conteneur populaires prennent en charge plusieurs flux vidéo, notamment :

- **Matroska (MKV)** : largement reconnu pour sa flexibilité et sa robuste prise en charge de plusieurs flux
- **MP4/MPEG-4** : courant dans les applications professionnelles et grand public
- **AVI** : bien que plus ancien, encore largement utilisé dans certains contextes
- **WebM** : populaire pour les applications web
- **TS/MTS** : utilisé dans les applications de diffusion et les caméras vidéo grand public

Chaque format a ses propres caractéristiques et limitations concernant la gestion de plusieurs flux vidéo, mais le composant `TVFMediaPlayer` fournit une approche unifiée pour travailler avec eux.

## Implémenter la lecture de plusieurs flux vidéo

### Configurer le lecteur multimédia

La première étape consiste à initialiser correctement l'objet `TVFMediaPlayer`. Cela implique de créer l'instance, de configurer les propriétés de base et de le préparer à la lecture.

!!! note "Les extraits font partie d'une seule procédure"
    Les extraits Pascal des trois sous-sections ci-dessous proviennent d'une seule procédure (`TForm1.SetupAndPlayMultiStream`), découpée pour la clarté narrative. Le listing complet et collable se trouve à la [fin de cette section](#listing-pascal-complet).

```pascal
// Extrait — voir « Listing Pascal complet » plus bas pour la procédure complète.
procedure TForm1.SetupAndPlayMultiStream;
var
  MediaPlayer1: TVFMediaPlayer;
begin
  MediaPlayer1 := TVFMediaPlayer.Create(Self);

  // Définir la taille et la position du conteneur si nécessaire
  MediaPlayer1.Parent := Panel1; // Panel1 est votre conteneur
  MediaPlayer1.Align := alClient;

  // Configurer l'état initial
  MediaPlayer1.DoubleBuffered := True;
  MediaPlayer1.AutoPlay := False; // Nous contrôlerons explicitement la lecture
  // ... suite ci-dessous
end;
```

### Configurer la source multimédia

Ensuite, nous devons spécifier le fichier multimédia et configurer la manière dont il doit être chargé :

```pascal
// Extrait — corps de TForm1.SetupAndPlayMultiStream (suite).
  // Définir le nom de fichier — utiliser un chemin complet pour plus de fiabilité
  MediaPlayer1.FilenameOrURL := 'C:\Videos\multistream-video.mkv';

  // Activer la lecture audio (le moteur de rendu audio DirectSound par défaut sera utilisé)
  MediaPlayer1.Audio_Play := True;

  // Configurer les paramètres audio si nécessaire
  MediaPlayer1.Audio_Volume := 85; // Définir le volume à 85 %

  // Définir le mode source sur DirectShow
  // Les autres options incluent SM_File_FFMPEG ou SM_File_VLC
  MediaPlayer1.Source_Mode := SM_File_DS;
```

### Sélectionner et changer de flux vidéo

La clé pour travailler avec plusieurs flux vidéo est la propriété `Source_VideoStreamIndex`. Cet index commençant à zéro vous permet de sélectionner quel flux vidéo doit être rendu :

```pascal
// Extrait — corps de TForm1.SetupAndPlayMultiStream (partie finale).
  // Définir l'index du flux vidéo à 1 (deuxième flux, l'index commençant à zéro)
  MediaPlayer1.Source_VideoStreamIndex := 1;

  // Démarrer la lecture
  MediaPlayer1.Play();
end;
```

### Listing Pascal complet

Les trois extraits ci-dessus, fusionnés en une seule procédure autonome et prête à coller :

```pascal
procedure TForm1.SetupAndPlayMultiStream;
var
  MediaPlayer1: TVFMediaPlayer;
begin
  MediaPlayer1 := TVFMediaPlayer.Create(Self);

  // Conteneur + état initial
  MediaPlayer1.Parent := Panel1;
  MediaPlayer1.Align := alClient;
  MediaPlayer1.DoubleBuffered := True;
  MediaPlayer1.AutoPlay := False;

  // Configuration de la source et de l'audio
  MediaPlayer1.FilenameOrURL := 'C:\Videos\multistream-video.mkv';
  MediaPlayer1.Audio_Play := True;
  MediaPlayer1.Audio_Volume := 85;
  MediaPlayer1.Source_Mode := SM_File_DS;

  // Choisir un flux vidéo non par défaut et démarrer la lecture
  MediaPlayer1.Source_VideoStreamIndex := 1;
  MediaPlayer1.Play();
end;
```

## Implémentation C++ MFC

### Configurer le lecteur multimédia

Voici comment implémenter la lecture de plusieurs flux vidéo en C++ avec MFC :

```cpp
// Dans votre fichier d'en-tête (MyDlg.h)
private:
    CVFMediaPlayer* m_pMediaPlayer;

// Dans votre fichier d'implémentation (MyDlg.cpp)
BOOL CMyDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    
    // Créer l'instance de MediaPlayer
    m_pMediaPlayer = new CVFMediaPlayer();
    
    // Initialiser le contrôle
    CWnd* pContainer = GetDlgItem(IDC_PLAYER_CONTAINER); // Votre contrôle conteneur
    m_pMediaPlayer->Create(NULL, NULL, WS_CHILD | WS_VISIBLE, 
                          CRect(0, 0, 0, 0), pContainer, 1001);
    
    // Configurer les paramètres d'affichage — CWnd::GetClientRect(LPRECT) renvoie void
    // et remplit le rect par référence, il faut donc déclarer un CRect d'abord.
    CRect rc;
    pContainer->GetClientRect(&rc);
    m_pMediaPlayer->SetWindowPos(NULL, 0, 0, rc.Width(), rc.Height(), SWP_NOZORDER);
    m_pMediaPlayer->PutDoubleBuffered(TRUE);
    m_pMediaPlayer->PutAutoPlay(FALSE);
    
    return TRUE;
}
```

### Configurer la source multimédia

```cpp
void CMyDlg::PlayMultiStreamVideo()
{
    // Définir le chemin du fichier et configurer la source
    m_pMediaPlayer->PutFilenameOrURL(_T("C:\\Videos\\multistream-video.mkv"));
    
    // Configurer l'audio
    m_pMediaPlayer->PutAudio_Play(TRUE);
    m_pMediaPlayer->PutAudio_Volume(85);
    
    // Définir le mode source sur DirectShow
    m_pMediaPlayer->PutSource_Mode(SM_File_DS);
    
    // Sélectionner le deuxième flux vidéo (index 1)
    m_pMediaPlayer->PutSource_VideoStreamIndex(1);
    
    // Démarrer la lecture
    m_pMediaPlayer->Play();
}

// Ne pas oublier de nettoyer
void CMyDlg::OnDestroy()
{
    if (m_pMediaPlayer != NULL)
    {
        m_pMediaPlayer->DestroyWindow();
        delete m_pMediaPlayer;
        m_pMediaPlayer = NULL;
    }
    
    CDialog::OnDestroy();
}
```

## Implémentation VB6

Voici comment implémenter la lecture de plusieurs flux vidéo en Visual Basic 6 :

```vb
' Déclarer l'objet MediaPlayer au niveau du formulaire
Private WithEvents MediaPlayer1 As TVFMediaPlayer

Private Sub Form_Load()
    ' Créer l'instance de MediaPlayer
    Set MediaPlayer1 = New TVFMediaPlayer
    
    ' Définir les propriétés du conteneur
    MediaPlayer1.CreateControl
    MediaPlayer1.Parent = Frame1 ' Frame1 est votre conteneur
    MediaPlayer1.Left = 0
    MediaPlayer1.Top = 0
    MediaPlayer1.Width = Frame1.ScaleWidth
    MediaPlayer1.Height = Frame1.ScaleHeight
    
    ' Configurer l'état initial
    MediaPlayer1.DoubleBuffered = True
    MediaPlayer1.AutoPlay = False
End Sub

Private Sub btnPlay_Click()
    ' Définir le nom de fichier — utiliser un chemin complet pour plus de fiabilité
    MediaPlayer1.FilenameOrURL = "C:\Videos\multistream-video.mkv"
    
    ' Activer la lecture audio
    MediaPlayer1.Audio_Play = True
    MediaPlayer1.Audio_Volume = 85 ' Définir le volume à 85 %
    
    ' Définir le mode source sur DirectShow
    MediaPlayer1.Source_Mode = SM_File_DS
    
    ' Sélectionner le deuxième flux vidéo (index 1)
    MediaPlayer1.Source_VideoStreamIndex = 1
    
    ' Démarrer la lecture
    MediaPlayer1.Play
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ' Libérer les ressources
    Set MediaPlayer1 = Nothing
End Sub
```

## Conclusion

La capacité à lire des fichiers vidéo comportant plusieurs flux ouvre de nombreuses possibilités pour créer des expériences multimédias riches et interactives. Le composant `TVFMediaPlayer` propose une approche simple pour implémenter cette fonctionnalité, avec des options flexibles pour s'adapter à différents besoins applicatifs.

En suivant les techniques décrites dans ce guide, vous pouvez intégrer efficacement la prise en charge de plusieurs flux vidéo dans vos applications, améliorant l'expérience utilisateur et étendant les capacités de vos projets multimédias.

---
N'hésitez pas à contacter le [support](https://support.visioforge.com/) si vous avez besoin d'aide pour cette fonctionnalité. Consultez notre page [GitHub](https://github.com/visioforge/) pour d'autres exemples de code et d'implémentation.
