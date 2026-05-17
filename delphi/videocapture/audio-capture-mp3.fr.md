---
title: Capture audio MP3 en Delphi, C++ MFC et VB6 — encodeur LAME
description: Capture audio MP3 en Delphi, C++ et VB6 — configurez l'encodeur LAME, gérez les débits binaires et créez des enregistrements de qualité.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Encoding
  - MP3

---

# Capture audio vers MP3 en Delphi, C++ MFC et VB6

## Introduction

Les capacités de capture audio sont essentielles pour de nombreuses applications modernes, des outils d'enregistrement vocal aux logiciels de création multimédia. Ce guide détaille l'implémentation de la fonctionnalité de capture audio MP3 dans les applications Delphi, C++ MFC et VB6 à l'aide du composant VideoCapture.

MP3 demeure l'un des formats audio les plus utilisés en raison de son excellente compression et de sa large compatibilité. En implémentant une capture audio MP3 appropriée dans vos applications, vous pouvez fournir aux utilisateurs des capacités d'enregistrement audio efficaces et de haute qualité.

## Prérequis

Avant d'implémenter la capture audio MP3, assurez-vous de disposer de :

- Un environnement de développement avec Delphi, Visual C++ (pour MFC) ou Visual Basic 6
- Le composant VideoCapture correctement installé et référencé dans votre projet
- Une compréhension de base des concepts d'encodage audio
- Les autorisations requises pour l'accès aux périphériques audio dans votre application

## Configuration de l'encodeur LAME

L'encodeur MP3 LAME offre de vastes options de personnalisation pour la qualité audio, la gestion du débit binaire et la configuration des canaux. Configurer correctement ces paramètres est crucial pour obtenir la qualité audio souhaitée tout en maîtrisant la taille des fichiers.

### Configuration des paramètres d'encodage de base

Les extraits de code suivants montrent comment configurer les paramètres d'encodage LAME de base :

```pascal
// Delphi
VideoCapture1.Audio_LAME_CBR_Bitrate := StrToInt(cbLameCBRBitrate.Items[cbLameCBRBitrate.ItemIndex]);
VideoCapture1.Audio_LAME_VBR_Min_Bitrate := StrToInt(cbLameVBRMin.Items[cbLameVBRMin.ItemIndex]);
VideoCapture1.Audio_LAME_VBR_Max_Bitrate := StrToInt(cbLameVBRMax.Items[cbLameVBRMax.ItemIndex]);
VideoCapture1.Audio_LAME_Sample_Rate := StrToInt(cbLameSampleRate.Items[cbLameSampleRate.ItemIndex]);
VideoCapture1.Audio_LAME_VBR_Quality := tbLameVBRQuality.Position;
VideoCapture1.Audio_LAME_Encoding_Quality := tbLameEncodingQuality.Position;
```

```cpp
// C++ MFC
// CComboBox::GetCurSel renvoie CB_ERR (-1) si rien n'est sélectionné — on
// ignore l'affectation dans ce cas pour conserver la valeur précédente.
int nIndex;
nIndex = m_cbLameCBRBitrate.GetCurSel();
if (nIndex != CB_ERR) m_VideoCapture.Audio_LAME_CBR_Bitrate = (int)m_cbLameCBRBitrate.GetItemData(nIndex);
nIndex = m_cbLameVBRMin.GetCurSel();
if (nIndex != CB_ERR) m_VideoCapture.Audio_LAME_VBR_Min_Bitrate = (int)m_cbLameVBRMin.GetItemData(nIndex);
nIndex = m_cbLameVBRMax.GetCurSel();
if (nIndex != CB_ERR) m_VideoCapture.Audio_LAME_VBR_Max_Bitrate = (int)m_cbLameVBRMax.GetItemData(nIndex);
nIndex = m_cbLameSampleRate.GetCurSel();
if (nIndex != CB_ERR) m_VideoCapture.Audio_LAME_Sample_Rate = (int)m_cbLameSampleRate.GetItemData(nIndex);
m_VideoCapture.Audio_LAME_VBR_Quality = m_tbLameVBRQuality.GetPos();
m_VideoCapture.Audio_LAME_Encoding_Quality = m_tbLameEncodingQuality.GetPos();
```

```vb
' VB6
VideoCapture1.Audio_LAME_CBR_Bitrate = CInt(cbLameCBRBitrate.List(cbLameCBRBitrate.ListIndex))
VideoCapture1.Audio_LAME_VBR_Min_Bitrate = CInt(cbLameVBRMin.List(cbLameVBRMin.ListIndex))
VideoCapture1.Audio_LAME_VBR_Max_Bitrate = CInt(cbLameVBRMax.List(cbLameVBRMax.ListIndex))
VideoCapture1.Audio_LAME_Sample_Rate = CInt(cbLameSampleRate.List(cbLameSampleRate.ListIndex))
VideoCapture1.Audio_LAME_VBR_Quality = tbLameVBRQuality.Value
VideoCapture1.Audio_LAME_Encoding_Quality = tbLameEncodingQuality.Value
```

### Définition des modes de canal audio

La configuration des canaux affecte à la fois la qualité sonore et la taille des fichiers. Le code suivant illustre comment définir le mode de canal :

```pascal
// Delphi
if rbLameStandardStereo.Checked then
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Standard_Stereo
else if rbLameJointStereo.Checked then
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Joint_Stereo
else if rbLameDualChannels.Checked then
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Dual_Stereo
else
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Mono;
```

```cpp
// C++ MFC
if (m_rbLameStandardStereo.GetCheck())
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Standard_Stereo;
else if (m_rbLameJointStereo.GetCheck())
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Joint_Stereo;
else if (m_rbLameDualChannels.GetCheck())
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Dual_Stereo;
else
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Mono;
```

```vb
' VB6
If rbLameStandardStereo.Value Then
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Standard_Stereo
ElseIf rbLameJointStereo.Value Then
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Joint_Stereo
ElseIf rbLameDualChannels.Value Then
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Dual_Stereo
Else
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Mono
End If
```

### Options de configuration LAME avancées

Pour un contrôle plus précis du processus d'encodage, configurez ces options LAME avancées :

```pascal
// Delphi
VideoCapture1.Audio_LAME_VBR_Mode := rbLameVBR.Checked;
VideoCapture1.Audio_LAME_Copyright := cbLameCopyright.Checked;
VideoCapture1.Audio_LAME_Original := cbLameOriginalCopy.Checked;
VideoCapture1.Audio_LAME_CRC_Protected := cbLameCRCProtected.Checked;
VideoCapture1.Audio_LAME_Force_Mono := cbLameForceMono.Checked;
VideoCapture1.Audio_LAME_Strictly_Enforce_VBR_Min_Bitrate := cbLameStrictlyEnforceVBRMinBitrate.Checked;
VideoCapture1.Audio_LAME_Voice_Encoding_Mode := cbLameVoiceEncodingMode.Checked;
VideoCapture1.Audio_LAME_Keep_All_Frequencies := cbLameKeepAllFrequencies.Checked;
VideoCapture1.Audio_LAME_Strict_ISO_Compilance := cbLameStrictISOCompilance.Checked;
VideoCapture1.Audio_LAME_Disable_Short_Blocks := cbLameDisableShortBlocks.Checked;
VideoCapture1.Audio_LAME_Enable_Xing_VBR_Tag := cbLameEnableXingVBRTag.Checked;
VideoCapture1.Audio_LAME_Mode_Fixed := cbLameModeFixed.Checked;
```

```cpp
// C++ MFC
m_VideoCapture.Audio_LAME_VBR_Mode = m_rbLameVBR.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Copyright = m_cbLameCopyright.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Original = m_cbLameOriginalCopy.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_CRC_Protected = m_cbLameCRCProtected.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Force_Mono = m_cbLameForceMono.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Strictly_Enforce_VBR_Min_Bitrate = m_cbLameStrictlyEnforceVBRMinBitrate.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Voice_Encoding_Mode = m_cbLameVoiceEncodingMode.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Keep_All_Frequencies = m_cbLameKeepAllFrequencies.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Strict_ISO_Compilance = m_cbLameStrictISOCompilance.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Disable_Short_Blocks = m_cbLameDisableShortBlocks.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Enable_Xing_VBR_Tag = m_cbLameEnableXingVBRTag.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Mode_Fixed = m_cbLameModeFixed.GetCheck() ? true : false;
```

```vb
' VB6
VideoCapture1.Audio_LAME_VBR_Mode = rbLameVBR.Value
VideoCapture1.Audio_LAME_Copyright = cbLameCopyright.Value
VideoCapture1.Audio_LAME_Original = cbLameOriginalCopy.Value
VideoCapture1.Audio_LAME_CRC_Protected = cbLameCRCProtected.Value
VideoCapture1.Audio_LAME_Force_Mono = cbLameForceMono.Value
VideoCapture1.Audio_LAME_Strictly_Enforce_VBR_Min_Bitrate = cbLameStrictlyEnforceVBRMinBitrate.Value
VideoCapture1.Audio_LAME_Voice_Encoding_Mode = cbLameVoiceEncodingMode.Value
VideoCapture1.Audio_LAME_Keep_All_Frequencies = cbLameKeepAllFrequencies.Value
VideoCapture1.Audio_LAME_Strict_ISO_Compilance = cbLameStrictISOCompilance.Value
VideoCapture1.Audio_LAME_Disable_Short_Blocks = cbLameDisableShortBlocks.Value
VideoCapture1.Audio_LAME_Enable_Xing_VBR_Tag = cbLameEnableXingVBRTag.Value
VideoCapture1.Audio_LAME_Mode_Fixed = cbLameModeFixed.Value
```

## Comprendre les options de configuration LAME

### Paramètres de débit binaire

- **CBR (débit constant)** : maintient le même débit binaire tout au long de l'enregistrement
- **VBR (débit variable)** : ajuste le débit binaire en fonction de la complexité audio
- **Débit min/max** : définit les bornes pour l'encodage VBR
- **Qualité VBR** : contrôle le compromis qualité/taille de fichier en mode VBR

### Modes de canal

- **Standard Stereo** : canaux gauche et droit complètement séparés
- **Joint Stereo** : combine les informations redondantes entre canaux pour économiser de l'espace
- **Dual Stereo** : deux canaux mono totalement indépendants
- **Mono** : canal audio unique

### Options d'encodage spéciales

- **Voice Encoding Mode** : optimise l'encodage pour les fréquences vocales
- **Force Mono** : convertit l'entrée stéréo en sortie mono
- **CRC Protection** : ajoute des données de détection d'erreurs
- **Strict ISO Compliance** : garantit une compatibilité maximale avec tous les lecteurs MP3

## Configuration du format de sortie

Après avoir configuré les paramètres d'encodage LAME, spécifiez MP3 comme format de sortie :

```pascal
// Delphi
VideoCapture1.OutputFormat := Format_LAME;
```

```cpp
// C++ MFC
m_VideoCapture.OutputFormat = VisioForge_Video_Capture::Format_LAME;
```

```vb
' VB6
VideoCapture1.OutputFormat = Format_LAME
```

## Définition du mode de capture audio

Définissez le composant VideoCapture en mode de capture audio uniquement :

```pascal
// Delphi
VideoCapture1.Mode := Mode_Audio_Capture;
```

```cpp
// C++ MFC
m_VideoCapture.Mode = VisioForge_Video_Capture::Mode_Audio_Capture;
```

```vb
' VB6
VideoCapture1.Mode = Mode_Audio_Capture
```

## Démarrage de la capture audio

Une fois tous les paramètres configurés, lancez le processus d'enregistrement :

```pascal
// Delphi
VideoCapture1.Start;
```

```cpp
// C++ MFC
m_VideoCapture.Start();
```

```vb
' VB6
VideoCapture1.Start
```

## Bonnes pratiques pour la capture audio MP3

- **Qualité contre taille** : pour les enregistrements vocaux, des débits inférieurs (64-128 kbps) suffisent généralement. Pour la musique, utilisez 192 kbps ou plus.
- **Choix de la fréquence d'échantillonnage** : 44,1 kHz est standard pour la plupart des audios. Des fréquences plus basses peuvent être utilisées pour les enregistrements vocaux uniquement.
- **VBR contre CBR** : le VBR offre généralement un meilleur rapport qualité/taille, mais peut poser des problèmes de compatibilité avec certains lecteurs.
- **Gestion des erreurs** : implémentez toujours une gestion d'erreur appropriée autour du processus d'enregistrement.
- **Retour utilisateur** : fournissez un retour visuel pendant l'enregistrement (indicateurs de niveau, temps écoulé).

## Conclusion

Implémenter la capture audio MP3 dans vos applications offre aux utilisateurs une solution d'enregistrement largement compatible et efficace. En configurant correctement les paramètres de l'encodeur LAME, vous pouvez équilibrer la qualité audio et la taille de fichier selon les besoins spécifiques de votre application.

Le composant VideoCapture rend cette implémentation simple dans les applications Delphi, C++ MFC et VB6, vous permettant de vous concentrer sur la création d'une excellente expérience utilisateur autour de la fonctionnalité de capture audio.

---
Pour des exemples de code et des techniques d'implémentation avancées supplémentaires, visitez notre dépôt GitHub. Si vous rencontrez des problèmes lors de l'implémentation, contactez notre équipe de support technique pour obtenir de l'aide.
