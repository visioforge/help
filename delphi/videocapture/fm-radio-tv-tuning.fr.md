---
title: Syntonisation radio FM et TV en Delphi — Guide de balayage
description: Implémentez la syntonisation radio FM et TV en Delphi — balayage de canaux, gestion des fréquences, détection de signal avec exemples Delphi, C++, VB6.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - C++
  - Windows
  - VCL
  - Capture
  - TV Tuner

---

# Implémenter la syntonisation radio FM et TV dans des applications Delphi

## Introduction à la syntonisation TV et radio

Ce guide fournit des exemples d'implémentation détaillés pour les développeurs Delphi travaillant avec la fonctionnalité de syntonisation radio FM et TV. Nous avons inclus des exemples de code équivalents pour C++ MFC et VB6 afin de prendre en charge les besoins de développement multiplateforme.

## Gestion des périphériques

### Récupération des tuners TV disponibles

La première étape de l'implémentation de la fonctionnalité de syntonisation consiste à identifier les périphériques matériels disponibles :

```pascal
// Parcourir tous les périphériques tuner TV connectés et alimenter la liste déroulante
for I := 0 to VideoCapture1.TVTuner_Devices_GetCount - 1 do
  cbTVTuner.Items.Add(VideoCapture1.TVTuner_Devices_GetItem(i));
```

```cpp
// Implémentation C++ MFC pour récupérer les périphériques tuner TV
for (int i = 0; i < m_VideoCapture.TVTuner_Devices_GetCount(); i++)
  m_cbTVTuner.AddString(m_VideoCapture.TVTuner_Devices_GetItem(i));
```

```vb
' Implémentation VB6 pour l'énumération des périphériques
For i = 0 To VideoCapture1.TVTuner_Devices_GetCount - 1
  cbTVTuner.AddItem VideoCapture1.TVTuner_Devices_GetItem(i)
Next i
```

### Énumération de la prise en charge des formats TV

Différentes régions utilisent différents standards de diffusion. Votre application doit détecter et gérer ces formats :

```pascal
// Charger les formats TV disponibles (PAL, NTSC, SECAM, etc.)
for I := 0 to VideoCapture1.TVTuner_TVFormats_GetCount - 1 do
  cbTVSystem.Items.Add(VideoCapture1.TVTuner_TVFormats_GetItem(i));
```

```cpp
// C++ MFC — Alimenter la liste déroulante des formats TV avec les standards disponibles
for (int i = 0; i < m_VideoCapture.TVTuner_TVFormats_GetCount(); i++)
  m_cbTVSystem.AddString(m_VideoCapture.TVTuner_TVFormats_GetItem(i));
```

```vb
' VB6 — Obtenir les formats TV pris en charge pour le tuner sélectionné
For i = 0 To VideoCapture1.TVTuner_TVFormats_GetCount - 1
  cbTVSystem.AddItem VideoCapture1.TVTuner_TVFormats_GetItem(i)
Next i
```

### Configuration spécifique au pays

Les standards de diffusion varient selon les pays, votre application doit donc proposer une sélection de région appropriée :

```pascal
// Charger la liste des pays/régions pour les paramètres de syntonisation localisés
for I := 0 to VideoCapture1.TVTuner_Countries_GetCount - 1 do
  cbTVCountry.Items.Add(VideoCapture1.TVTuner_Countries_GetItem(i));
```

```cpp
// C++ MFC — Construire la liste de sélection des pays pour les paramètres régionaux
for (int i = 0; i < m_VideoCapture.TVTuner_Countries_GetCount(); i++)
  m_cbTVCountry.AddString(m_VideoCapture.TVTuner_Countries_GetItem(i));
```

```vb
' VB6 — Alimenter la liste déroulante des pays pour les paramètres de diffusion régionaux
For i = 0 To VideoCapture1.TVTuner_Countries_GetCount - 1
  cbTVCountry.AddItem VideoCapture1.TVTuner_Countries_GetItem(i)
Next i
```

## Configuration du périphérique

### Sélection d'un tuner TV

Une fois les périphériques disponibles énumérés, les utilisateurs peuvent sélectionner leur tuner préféré :

```pascal
// Définir le tuner actif en fonction de la sélection utilisateur
VideoCapture1.TVTuner_Name := cbTVTuner.Items[cbTVTuner.ItemIndex];
```

```cpp
// C++ MFC — Appliquer la sélection du tuner par l'utilisateur
CString strText;
m_cbTVTuner.GetLBText(m_cbTVTuner.GetCurSel(), strText);
m_VideoCapture.put_TVTuner_Name(strText);
```

```vb
' VB6 — Définir le tuner sélectionné comme périphérique actif
VideoCapture1.TVTuner_Name = cbTVTuner.Text
```

### Lecture de la configuration actuelle du tuner

Après avoir sélectionné un périphérique, vous devrez lire ses paramètres actuels :

```pascal
// Initialiser le tuner et lire la configuration actuelle
VideoCapture1.TVTuner_Read;
```

```cpp
// C++ MFC — Charger les paramètres actuels du tuner dans l'application
m_VideoCapture.TVTuner_Read();
```

```vb
' VB6 — Lire la configuration du tuner après la sélection du périphérique
VideoCapture1.TVTuner_Read
```

### Modes de fonctionnement disponibles

Les tuners prennent en charge différents modes comme TV, radio FM, etc. :

```pascal
// Alimenter la liste déroulante des modes de fonctionnement avec les options disponibles
for I := 0 to VideoCapture1.TVTuner_Modes_GetCount - 1 do
  cbTVMode.Items.Add(VideoCapture1.TVTuner_Modes_GetItem(i));
```

```cpp
// C++ MFC — Obtenir les modes opérationnels pris en charge pour ce périphérique
for (int i = 0; i < m_VideoCapture.TVTuner_Modes_GetCount(); i++)
  m_cbTVMode.AddString(m_VideoCapture.TVTuner_Modes_GetItem(i));
```

```vb
' VB6 — Lister les modes tuner disponibles (TV, radio FM, etc.)
For i = 0 To VideoCapture1.TVTuner_Modes_GetCount - 1
  cbTVMode.AddItem VideoCapture1.TVTuner_Modes_GetItem(i)
Next i
```

## Gestion des fréquences

### Lecture des fréquences actuelles

Affichez les fréquences audio et vidéo actuelles pour fournir un retour utilisateur :

```pascal
// Afficher les fréquences vidéo et audio actuelles en Hz
edVideoFreq.Text := IntToStr(VideoCapture1.TVTuner_VideoFrequency);
edAudiofreq.Text := IntToStr(VideoCapture1.TVTuner_AudioFrequency);
```

```cpp
// C++ MFC — Afficher les valeurs de fréquence actuelles dans l'interface
CString strFreq;
strFreq.Format(_T("%d"), m_VideoCapture.get_TVTuner_VideoFrequency());
m_edVideoFreq.SetWindowText(strFreq);
strFreq.Format(_T("%d"), m_VideoCapture.get_TVTuner_AudioFrequency());
m_edAudioFreq.SetWindowText(strFreq);
```

```vb
' VB6 — Mettre à jour les champs d'affichage de fréquence avec les valeurs actuelles
edVideoFreq.Text = CStr(VideoCapture1.TVTuner_VideoFrequency)
edAudioFreq.Text = CStr(VideoCapture1.TVTuner_AudioFrequency)
```

## Configuration d'entrée et de mode

### Définition de la source d'entrée du signal

Les tuners peuvent prendre en charge plusieurs sources d'entrée qui doivent être configurables :

```pascal
// Sélectionner la source d'entrée appropriée en fonction de la configuration actuelle
cbTVInput.ItemIndex := cbTVInput.Items.IndexOf(VideoCapture1.TVTuner_InputType);
```

```cpp
// C++ MFC — Mettre à jour la sélection de la source d'entrée dans l'interface
CString strInputType = m_VideoCapture.get_TVTuner_InputType();
m_cbTVInput.SetCurSel(m_cbTVInput.FindStringExact(-1, strInputType));
```

```vb
' VB6 — Définir la liste déroulante de la source d'entrée pour correspondre à la configuration actuelle
cbTVInput.ListIndex = cbTVInput.FindItem(VideoCapture1.TVTuner_InputType)
```

### Configuration du mode de fonctionnement

Les différents modes de tuner nécessitent des ajustements spécifiques de l'interface et des paramètres :

```pascal
// Définir la liste déroulante du mode de fonctionnement sur le mode actuel (TV, radio FM, etc.)
cbTVMode.ItemIndex := cbTVMode.Items.IndexOf(VideoCapture1.TVTuner_Mode);
```

```cpp
// C++ MFC — Mettre à jour le sélecteur de mode pour correspondre à la configuration actuelle du tuner
CString strMode = m_VideoCapture.get_TVTuner_Mode();
m_cbTVMode.SetCurSel(m_cbTVMode.FindStringExact(-1, strMode));
```

```vb
' VB6 — Sélectionner le mode de fonctionnement actuel dans la liste déroulante
cbTVMode.ListIndex = cbTVMode.FindItem(VideoCapture1.TVTuner_Mode)
```

### Configuration du format TV

Définissez le standard de diffusion approprié pour la région :

```pascal
// Configurer le standard TV approprié (PAL, NTSC, SECAM, etc.)
cbTVSystem.ItemIndex := cbTVSystem.Items.IndexOf(VideoCapture1.TVTuner_TVFormat);
```

```cpp
// C++ MFC — Définir la liste déroulante du format TV sur le standard de diffusion actuel
CString strTVFormat = m_VideoCapture.get_TVTuner_TVFormat();
m_cbTVSystem.SetCurSel(m_cbTVSystem.FindStringExact(-1, strTVFormat));
```

```vb
' VB6 — Mettre à jour la sélection du format système TV
cbTVSystem.ListIndex = cbTVSystem.FindItem(VideoCapture1.TVTuner_TVFormat)
```

### Paramètres régionaux

Configurez les paramètres de diffusion spécifiques à la région :

```pascal
// Définir le pays/la région pour des tables de fréquences et standards appropriés
cbTVCountry.ItemIndex := cbTVCountry.Items.IndexOf(VideoCapture1.TVTuner_Country);
```

```cpp
// C++ MFC — Mettre à jour la sélection du pays pour correspondre au paramètre actuel
CString strCountry = m_VideoCapture.get_TVTuner_Country();
m_cbTVCountry.SetCurSel(m_cbTVCountry.FindStringExact(-1, strCountry));
```

```vb
' VB6 — Définir la liste déroulante des pays sur le paramètre régional actuel
cbTVCountry.ListIndex = cbTVCountry.FindItem(VideoCapture1.TVTuner_Country)
```

## Balayage des canaux

### Gestion des événements de balayage de canaux

Implémentez le gestionnaire d'événements pour le processus de balayage de canaux :

```pascal
// Gestionnaire d'événements pour le processus de balayage de canaux
// Suit la progression et collecte les canaux trouvés
procedure TForm1.VideoCapture1TVTunerTuneChannels(SignalPresent: Boolean; Channel, Frequency, Progress: Integer);
begin
  // Mettre à jour la barre de progression avec la progression actuelle du balayage
  pbChannels.Position := Progress;

  // Ajouter le canal à la liste si un signal est détecté
  if SignalPresent then
    cbTVChannel.Items.Add(IntToStr(Channel));

  // Balayage terminé lorsque Channel = -1
  if Channel = -1 then
    begin
      pbChannels.Position := 0;
      ShowMessage('Recherche automatique terminée');
    end;
end;
```

```cpp
// C++ MFC — Implémentation du gestionnaire d'événements de balayage de canaux
// Dans le fichier d'en-tête (.h)
BEGIN_EVENTSINK_MAP(CMainDlg, CDialog)
    ON_EVENT(CMainDlg, IDC_VIDEOCAPTURE, 1, OnTVTunerTuneChannels, VTS_BOOL VTS_I4 VTS_I4 VTS_I4)
END_EVENTSINK_MAP()

// Dans le fichier d'implémentation (.cpp)
void CMainDlg::OnTVTunerTuneChannels(BOOL SignalPresent, long Channel, long Frequency, long Progress)
{
    // Mettre à jour l'indicateur de progression du balayage
    m_pbChannels.SetPos(Progress);
    
    // Ajouter les canaux trouvés à la liste de sélection
    if (SignalPresent)
    {
        CString strChannel;
        strChannel.Format(_T("%d"), Channel);
        m_cbTVChannel.AddString(strChannel);
    }
    
    // Gérer la fin du balayage
    if (Channel == -1)
    {
        m_pbChannels.SetPos(0);
        MessageBox(_T("AutoTune complete"), _T("Information"), MB_OK | MB_ICONINFORMATION);
    }
}
```

```vb
' VB6 — Gestionnaire d'événements de balayage de canaux
Private Sub VideoCapture1_TVTunerTuneChannels(ByVal SignalPresent As Boolean, ByVal Channel As Long, ByVal Frequency As Long, ByVal Progress As Long)
    ' Mettre à jour l'affichage de progression du balayage
    pbChannels.Value = Progress
    
    ' Ajouter le canal à la liste lorsqu'un signal est trouvé
    If SignalPresent Then
        cbTVChannel.AddItem CStr(Channel)
    End If
    
    ' Gérer la fin du balayage
    If Channel = -1 Then
        pbChannels.Value = 0
        MsgBox "AutoTune complete", vbInformation
    End If
End Sub
```

### Lancement du balayage des canaux

Démarrez le processus automatique de balayage des canaux :

```pascal
// Définir les constantes de fréquence pour plus de clarté
const KHz = 1000;
const MHz = 1000000;

// Initialiser le tuner avec les paramètres actuels
VideoCapture1.TVTuner_Read;
// Vider la liste de canaux précédente
cbTVChannel.Items.Clear;

// Configurer les paramètres spéciaux pour le balayage radio FM
if ( (cbTVMode.ItemIndex <> -1) and (cbTVMode.Items[cbTVMode.ItemIndex] = 'FM Radio') ) then
  begin
    // Définir la plage de fréquences pour le balayage FM (100-110 MHz)
    VideoCapture1.TVTuner_FM_Tuning_StartFrequency := 100 * MHz;
    VideoCapture1.TVTuner_FM_Tuning_StopFrequency := 110 * MHz;
    // Définir des incréments de 100 kHz pour le balayage FM
    VideoCapture1.TVTuner_FM_Tuning_Step := 100 * KHz;
  end;

// Lancer le balayage automatique des canaux
VideoCapture1.TVTuner_TuneChannels_Start;
```

```cpp
// C++ MFC — Lancer un balayage de canaux avec les paramètres appropriés
const int KHz = 1000;
const int MHz = 1000000;

// Mettre à jour la configuration du tuner
m_VideoCapture.TVTuner_Read();
// Réinitialiser la liste des canaux avant le balayage
m_cbTVChannel.ResetContent();

// Configurer les paramètres spécifiques à la FM si en mode radio
CString strMode;
m_cbTVMode.GetLBText(m_cbTVMode.GetCurSel(), strMode);
if (strMode == _T("FM Radio"))
{
    // Définir la plage de balayage FM (100-110 MHz)
    m_VideoCapture.put_TVTuner_FM_Tuning_StartFrequency(100 * MHz);
    m_VideoCapture.put_TVTuner_FM_Tuning_StopFrequency(110 * MHz);
    // Utiliser des pas de 100 kHz pour le balayage FM
    m_VideoCapture.put_TVTuner_FM_Tuning_Step(100 * KHz);
}

// Démarrer le processus de balayage des canaux
m_VideoCapture.TVTuner_TuneChannels_Start();
```

```vb
' VB6 — Démarrer le processus de balayage des canaux
Const KHz = 1000
Const MHz = 1000000

' Lire la configuration actuelle du tuner
VideoCapture1.TVTuner_Read
' Vider la liste de canaux existante
cbTVChannel.Clear

' Configuration spéciale pour le balayage radio FM
If (cbTVMode.ListIndex <> -1) And (cbTVMode.Text = "FM Radio") Then
    ' Définir les paramètres de balayage de la bande FM (100-110 MHz)
    VideoCapture1.TVTuner_FM_Tuning_StartFrequency = 100 * MHz
    VideoCapture1.TVTuner_FM_Tuning_StopFrequency = 110 * MHz
    ' Utiliser une taille de pas de 100 kHz pour le balayage FM
    VideoCapture1.TVTuner_FM_Tuning_Step = 100 * KHz
End If

' Lancer le balayage automatique des canaux
VideoCapture1.TVTuner_TuneChannels_Start
```

## Opérations de syntonisation manuelle

### Sélection d'un canal par numéro

Autorisez la sélection directe d'un canal par numéro :

```pascal
// Passer au numéro de canal spécifié
VideoCapture1.TVTuner_Channel := StrToInt(edChannel.Text);
// Appliquer les modifications de syntonisation
VideoCapture1.TVTuner_Apply;
```

```cpp
// C++ MFC — Régler le tuner sur le numéro de canal spécifié
CString strChannel;
m_edChannel.GetWindowText(strChannel);
m_VideoCapture.put_TVTuner_Channel(_ttoi(strChannel));
m_VideoCapture.TVTuner_Apply();
```

```vb
' VB6 — Syntoniser sur un numéro de canal spécifique
VideoCapture1.TVTuner_Channel = CInt(edChannel.Text)
VideoCapture1.TVTuner_Apply
```

### Réglage direct de la fréquence radio

Pour la radio FM, la syntonisation directe par fréquence est souvent requise :

```pascal
// Définir le canal sur -1 pour une syntonisation par fréquence
VideoCapture1.TVTuner_Channel := -1; // doit être -1 pour utiliser la fréquence
// Définir la fréquence spécifique depuis le champ de saisie
VideoCapture1.TVTuner_Frequency := StrToInt(edChannel.Text);
// Appliquer le changement de fréquence
VideoCapture1.TVTuner_Apply;
```

```cpp
// C++ MFC — Implémentation de la syntonisation directe par fréquence
CString strFrequency;
m_edChannel.GetWindowText(strFrequency);
// Définir le canal sur -1 pour activer la syntonisation par fréquence
m_VideoCapture.put_TVTuner_Channel(-1); // doit être -1 pour utiliser la fréquence
// Appliquer la fréquence spécifiée
m_VideoCapture.put_TVTuner_Frequency(_ttoi(strFrequency));
m_VideoCapture.TVTuner_Apply();
```

```vb
' VB6 — Syntonisation manuelle par fréquence pour la radio
VideoCapture1.TVTuner_Channel = -1 ' doit être -1 pour utiliser la fréquence
VideoCapture1.TVTuner_Frequency = CInt(edChannel.Text)
VideoCapture1.TVTuner_Apply
```

## Conclusion

Ce guide couvre les aspects essentiels de l'implémentation de la fonctionnalité de syntonisation radio FM et TV dans vos applications Delphi. En suivant ces exemples, vous pouvez créer des interfaces de syntonisation robustes avec un balayage de canaux, une gestion des fréquences et une détection de signal appropriés.

Pour une intégration optimale dans vos projets, n'oubliez pas de gérer les conditions d'erreur et de fournir un retour utilisateur approprié pendant les opérations longues telles que le balayage de canaux.

---
Veuillez visiter notre page [GitHub](https://github.com/visioforge/) pour des exemples de code et d'implémentation supplémentaires.
