---
title: Sélection du périphérique de sortie audio en Delphi
description: Sélectionnez les périphériques de sortie audio en Delphi — énumération, volume et balance avec exemples Delphi, C++ et VB6.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture

---

# Sélection du périphérique de sortie audio en Delphi

Ce guide fournit des instructions détaillées et des exemples de code pour implémenter la sélection du périphérique de sortie audio dans vos applications de capture vidéo. Les implémentations Delphi, C++ MFC et VB6 sont couvertes pour vous aider à intégrer efficacement cette fonctionnalité dans vos projets.

## Énumération des périphériques de sortie audio disponibles

La première étape pour implémenter la sélection du périphérique de sortie audio consiste à récupérer la liste complète des périphériques de sortie audio disponibles sur le système. Cela permet aux utilisateurs de choisir leur périphérique de sortie audio préféré.

### Implémentation Delphi

```pascal
// Parcourir tous les périphériques de sortie audio disponibles
for i := 0 to VideoCapture1.Audio_OutputDevices_GetCount - 1 do
  // Ajouter chaque périphérique à la liste déroulante
  cbAudioOutputDevice.Items.Add(VideoCapture1.Audio_OutputDevices_GetItem(i));
```

### Implémentation C++ MFC

```cpp
// Alimenter la combobox avec tous les périphériques de sortie audio disponibles
for (int i = 0; i < m_VideoCapture.Audio_OutputDevices_GetCount(); i++) {
  CString deviceName = m_VideoCapture.Audio_OutputDevices_GetItem(i);
  m_AudioOutputDeviceCombo.AddString(deviceName);
}
```

### Implémentation VB6

```vb
' Parcourir tous les périphériques de sortie audio disponibles
For i = 0 To VideoCapture1.Audio_OutputDevices_GetCount - 1
  ' Ajouter chaque périphérique à la liste déroulante
  cboAudioOutputDevice.AddItem VideoCapture1.Audio_OutputDevices_GetItem(i)
Next i
```

## Définition du périphérique de sortie audio actif

Après avoir récupéré les périphériques disponibles, l'étape suivante consiste à définir le périphérique sélectionné comme périphérique de sortie audio actif de votre application.

### Implémentation Delphi

```pascal
// Définir le périphérique sélectionné comme périphérique de sortie audio actif
VideoCapture1.Audio_OutputDevice := cbAudioOutputDevice.Items[cbAudioOutputDevice.ItemIndex];
```

### Implémentation C++ MFC

```cpp
// Obtenir l'index sélectionné depuis la combobox
int selectedIndex = m_AudioOutputDeviceCombo.GetCurSel();
CString selectedDevice;
m_AudioOutputDeviceCombo.GetLBText(selectedIndex, selectedDevice);

// Définir le périphérique sélectionné comme périphérique de sortie audio actif.
// Les setters de propriété COM en MFC utilisent le wrapper put_, pas l'affectation directe.
m_VideoCapture.put_Audio_OutputDevice(selectedDevice);
```

### Implémentation VB6

```vb
' Définir le périphérique sélectionné comme périphérique de sortie audio actif
VideoCapture1.Audio_OutputDevice = cboAudioOutputDevice.Text
```

## Activation de la lecture audio

Une fois le périphérique de sortie sélectionné, vous devez activer la lecture audio pour entendre l'audio à travers le périphérique sélectionné.

### Implémentation Delphi

```pascal
// Activer la lecture audio via le périphérique sélectionné
VideoCapture1.Audio_PlayAudio := true;
```

### Implémentation C++ MFC

```cpp
// Activer la lecture audio via le périphérique sélectionné
m_VideoCapture.Audio_PlayAudio = TRUE;
```

### Implémentation VB6

```vb
' Activer la lecture audio via le périphérique sélectionné
VideoCapture1.Audio_PlayAudio = True
```

## Ajustement des niveaux de volume audio

Fournir un contrôle du volume donne aux utilisateurs la possibilité de personnaliser leur expérience audio. Cette section montre comment implémenter l'ajustement du volume.

### Implémentation Delphi

```pascal
// Définir le niveau de volume en fonction de la position du trackbar
VideoCapture1.Audio_OutputDevice_SetVolume(tbAudioVolume.Position);
```

### Implémentation C++ MFC

```cpp
// Obtenir la position actuelle du curseur de volume
int volumeLevel = m_VolumeSlider.GetPos();

// Définir le niveau de volume en fonction de la position du curseur
m_VideoCapture.Audio_OutputDevice_SetVolume(volumeLevel);
```

### Implémentation VB6

```vb
' Définir le niveau de volume en fonction de la position du curseur
VideoCapture1.Audio_OutputDevice_SetVolume sldVolume.Value
```

## Contrôle de la balance audio

Pour la sortie stéréo, le contrôle de la balance permet aux utilisateurs d'ajuster le volume relatif entre les canaux gauche et droit.

### Implémentation Delphi  

```pascal
// Définir le niveau de balance en fonction de la position du trackbar
VideoCapture1.Audio_OutputDevice_SetBalance(tbAudioBalance.Position);
```
  
### Implémentation C++ MFC

```cpp
// Obtenir la position actuelle du curseur de balance
int balanceLevel = m_BalanceSlider.GetPos();

// Définir le niveau de balance en fonction de la position du curseur
m_VideoCapture.Audio_OutputDevice_SetBalance(balanceLevel);
```

### Implémentation VB6

```vb
' Définir le niveau de balance en fonction de la position du curseur
VideoCapture1.Audio_OutputDevice_SetBalance sldBalance.Value
```

## Bonnes pratiques pour l'implémentation des périphériques audio

- Vérifiez toujours si le périphérique audio est valide avant de tenter de l'utiliser
- Prévoyez des mécanismes de repli lorsque le périphérique sélectionné devient indisponible
- Envisagez de sauvegarder les préférences utilisateur pour la sélection du périphérique audio entre les sessions
- Implémentez un retour visuel lorsque les paramètres de volume ou de balance sont modifiés

---
Veuillez contacter notre [équipe de support](https://support.visioforge.com/) si vous avez besoin d'aide pour cette implémentation. Visitez notre [dépôt GitHub](https://github.com/visioforge/) pour des exemples de code et des ressources supplémentaires.
