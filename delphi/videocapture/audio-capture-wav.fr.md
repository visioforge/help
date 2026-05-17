---
title: Enregistrement audio WAV avec Delphi Video Capture SDK
description: Capturez l'audio vers des fichiers WAV en Delphi avec TVFVideoCapture — codecs, compression, stéréo, débit binaire et exemples de code prêts à l'emploi.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - WAV

---

# Capture audio vers des fichiers WAV : guide d'implémentation développeur

## Introduction

La capture audio vers des fichiers WAV est une exigence fondamentale pour de nombreuses applications multimédias. Ce guide fournit des instructions détaillées pour implémenter la fonctionnalité de capture audio avec ou sans compression dans vos applications. Que vous développiez en Delphi, C++ MFC ou VB6 à l'aide de nos contrôles ActiveX, ce guide vous accompagnera tout au long du processus, de la configuration initiale à l'implémentation finale.

## Configuration de votre environnement de développement

Avant de commencer à implémenter la capture audio, assurez-vous d'avoir :

1. Installé le SDK dans votre environnement de développement
2. Ajouté le composant VideoCapture à votre formulaire/projet
3. Mis en place une gestion d'erreurs de base pour gérer les exceptions de capture
4. Configuré votre application pour accéder au matériel audio

## Gestion des codecs audio

### Récupération des codecs audio disponibles

La première étape de l'implémentation de la capture audio consiste à récupérer la liste des codecs audio disponibles sur le système. Cela vous permet de présenter aux utilisateurs des options de codecs ou de sélectionner par programme le codec le plus approprié aux besoins de votre application.

#### Implémentation Delphi

```pascal
// Parcourir tous les codecs audio disponibles
for i := 0 to VideoCapture1.Audio_Codecs_GetCount - 1 do
  cbAudioCodec.Items.Add(VideoCapture1.Audio_Codecs_GetItem(i));
```

#### Implémentation C++ MFC

```cpp
// Obtenir tous les codecs audio disponibles et alimenter la combo box
for (int i = 0; i < m_VideoCapture.Audio_Codecs_GetCount(); i++) {
  CString codec = m_VideoCapture.Audio_Codecs_GetItem(i);
  m_AudioCodecCombo.AddString(codec);
}
```

#### Implémentation VB6

```vb
' Parcourir tous les codecs audio disponibles
For i = 0 To VideoCapture1.Audio_Codecs_GetCount - 1
  cboAudioCodec.AddItem VideoCapture1.Audio_Codecs_GetItem(i)
Next i
```

### Sélection d'un codec audio

Une fois la liste des codecs disponibles renseignée, vous devrez fournir un moyen de sélectionner le codec souhaité pour l'opération de capture audio. Cela peut être fait par programmation ou par sélection utilisateur.

#### Implémentation Delphi

```pascal
// Définir le codec en fonction de la sélection utilisateur dans la combo box
VideoCapture1.Audio_Codec := cbAudioCodec.Items[cbAudioCodec.ItemIndex];
```

#### Implémentation C++ MFC

```cpp
// Obtenir le codec sélectionné dans la combo box
int selectedIndex = m_AudioCodecCombo.GetCurSel();
CString selectedCodec;
m_AudioCodecCombo.GetLBText(selectedIndex, selectedCodec);

// Définir le codec
m_VideoCapture.SetAudio_Codec(selectedCodec);
```

#### Implémentation VB6

```vb
' Définir le codec en fonction de la sélection utilisateur
VideoCapture1.Audio_Codec = cboAudioCodec.Text
```

## Configuration des paramètres audio

Une configuration appropriée des paramètres audio est cruciale pour obtenir l'équilibre souhaité entre qualité et taille de fichier. Les trois paramètres principaux à configurer sont les canaux, les bits par échantillon (BPS) et la fréquence d'échantillonnage.

### Définition des canaux audio

Les canaux audio déterminent si l'audio capturé est mono (1 canal) ou stéréo (2 canaux). Le stéréo offre une meilleure représentation spatiale du son mais nécessite davantage d'espace de stockage.

#### Implémentation Delphi

```pascal
// Définir le nombre de canaux audio (1 pour mono, 2 pour stéréo)
VideoCapture1.Audio_Channels := StrToInt(cbChannels2.Items[cbChannels2.ItemIndex]);
```

#### Implémentation C++ MFC

```cpp
// Définir les canaux audio (1 pour mono, 2 pour stéréo)
int nIndex = m_ChannelsCombo.GetCurSel();
if (nIndex == CB_ERR) return; // Aucune sélection — conserver la valeur actuelle
CString strChannels;
m_ChannelsCombo.GetLBText(nIndex, strChannels);
int channels = _ttoi(strChannels);
m_VideoCapture.SetAudio_Channels(channels);
```

#### Implémentation VB6

```vb
' Définir les canaux audio (1 pour mono, 2 pour stéréo)
VideoCapture1.Audio_Channels = CInt(cboChannels.Text)
```

### Configuration des bits par échantillon (BPS)

Le paramètre de bits par échantillon (BPS) affecte la plage dynamique et la qualité de l'audio. Les valeurs courantes incluent 8, 16 et 24 bits, les valeurs supérieures offrant une meilleure qualité au prix de tailles de fichiers plus importantes.

#### Implémentation Delphi

```pascal
// Définir les bits par échantillon (typiquement 8, 16 ou 24)
VideoCapture1.Audio_BPS := StrToInt(cbBPS2.Items[cbBPS2.ItemIndex]);
```

#### Implémentation C++ MFC

```cpp
// Définir les bits par échantillon
int nIndex = m_BPSCombo.GetCurSel();
if (nIndex == CB_ERR) return; // Aucune sélection — conserver la valeur actuelle
CString strBPS;
m_BPSCombo.GetLBText(nIndex, strBPS);
int bps = _ttoi(strBPS);
m_VideoCapture.SetAudio_BPS(bps);
```

#### Implémentation VB6

```vb
' Définir les bits par échantillon
VideoCapture1.Audio_BPS = CInt(cboBPS.Text)
```

### Définition de la fréquence d'échantillonnage

La fréquence d'échantillonnage détermine combien d'échantillons audio sont capturés par seconde. Les valeurs courantes incluent 8 000 Hz, 44 100 Hz (qualité CD) et 48 000 Hz (audio professionnel). Des fréquences d'échantillonnage plus élevées capturent davantage de détails dans les hautes fréquences mais augmentent la taille des fichiers.

#### Implémentation Delphi

```pascal
// Définir la fréquence d'échantillonnage audio en Hz (valeurs courantes : 8000, 44100, 48000)
VideoCapture1.Audio_SampleRate := StrToInt(cbSamplerate.Items[cbSamplerate.ItemIndex]);
```

#### Implémentation C++ MFC

```cpp
// Définir la fréquence d'échantillonnage
int nIndex = m_SampleRateCombo.GetCurSel();
if (nIndex == CB_ERR) return; // Aucune sélection — conserver la valeur actuelle
CString strSampleRate;
m_SampleRateCombo.GetLBText(nIndex, strSampleRate);
int sampleRate = _ttoi(strSampleRate);
m_VideoCapture.SetAudio_SampleRate(sampleRate);
```

#### Implémentation VB6

```vb
' Définir la fréquence d'échantillonnage
VideoCapture1.Audio_SampleRate = CInt(cboSampleRate.Text)
```

## Configuration du format de sortie

### Sélection du format PCM/ACM

Le Gestionnaire de compression audio Windows (ACM) prend en charge différents formats audio, notamment PCM (non compressé) et des formats compressés. Définir le format de sortie sur PCM/ACM active la compression basée sur les codecs lorsqu'un codec autre que PCM est sélectionné.

#### Implémentation Delphi

```pascal
// Définir la sortie au format PCM/ACM pour activer la compression basée sur les codecs
VideoCapture1.OutputFormat := Format_PCM_ACM;
```

#### Implémentation C++ MFC

```cpp
// Définir le format de sortie sur PCM/ACM
m_VideoCapture.SetOutputFormat(Format_PCM_ACM);
```

#### Implémentation VB6

```vb
' Définir le format de sortie sur PCM/ACM
VideoCapture1.OutputFormat = Format_PCM_ACM
```

## Définition du mode de capture audio

Avant de démarrer l'opération de capture, vous devez régler le composant en mode de capture audio. Cela garantit que seul l'audio est capturé, sans flux vidéo.

### Implémentation Delphi

```pascal
// Définir le mode de capture audio uniquement
VideoCapture1.Mode := Mode_Audio_Capture;
```

### Implémentation C++ MFC

```cpp
// Définir le mode de capture audio uniquement
m_VideoCapture.SetMode(Mode_Audio_Capture);
```

### Implémentation VB6

```vb
' Définir le mode de capture audio uniquement
VideoCapture1.Mode = Mode_Audio_Capture
```

## Démarrage de la capture audio

Tous les paramètres étant configurés, vous pouvez maintenant démarrer le processus de capture audio. Cela initialise le matériel audio, applique le codec et les paramètres sélectionnés, et commence la capture audio vers le fichier de sortie spécifié.

### Implémentation Delphi

```pascal
// Démarrer le processus de capture audio
VideoCapture1.Start;
```

### Implémentation C++ MFC

```cpp
// Démarrer le processus de capture audio
m_VideoCapture.Start();
```

### Implémentation VB6

```vb
' Démarrer le processus de capture audio
VideoCapture1.Start
```

## Considérations d'implémentation avancées

### Intégration de l'interface utilisateur

Pour offrir une meilleure expérience utilisateur, envisagez d'implémenter :

1. Un VU-mètre audio en temps réel
2. L'affichage du temps écoulé
3. L'estimation de la taille du fichier
4. La fonctionnalité de pause/reprise

### Optimisation des performances

Pour des performances optimales lors de sessions de capture audio prolongées :

1. Surveillez l'utilisation de la mémoire système
2. Implémentez le découpage de fichiers pour les longs enregistrements
3. Envisagez des stratégies de mise en tampon pour les captures de haute qualité

## Dépannage des problèmes courants

Lors de l'implémentation de la capture audio, vous pouvez rencontrer ces problèmes courants :

1. **Aucun périphérique audio détecté** : vérifiez les connexions matérielles et les pilotes
2. **Mauvaise qualité audio** : vérifiez les paramètres de fréquence d'échantillonnage et de bits par échantillon
3. **Problèmes de compatibilité de codec** : testez avec des codecs standards comme PCM ou MP3
4. **Utilisation élevée du processeur** : envisagez de réduire la fréquence d'échantillonnage ou d'utiliser l'accélération matérielle

## Conclusion

L'implémentation de la capture audio vers des fichiers WAV dans vos applications nécessite une configuration soigneuse des codecs, des paramètres audio et des paramètres de sortie. En suivant ce guide, vous pouvez créer une fonctionnalité de capture audio robuste qui équilibre les exigences de qualité et de taille de fichier.

Pour des implémentations complexes ou des défis techniques spécifiques, notre équipe de support est disponible pour vous aider à proposer des solutions personnalisées adaptées aux exigences de votre application.

## Ressources supplémentaires

Visitez notre page GitHub pour davantage d'exemples de code et d'exemples d'implémentation qui démontrent des techniques avancées de capture audio et des modèles d'intégration.

---
Pour une assistance technique sur cette implémentation, veuillez contacter notre équipe de support. Des exemples de code supplémentaires sont disponibles sur notre page GitHub.
