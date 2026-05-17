---
title: Capture vidéo DV en Delphi — Direct Stream et recompressé
description: Implémentez la capture vidéo DV en Delphi — formats compressés et non compressés avec implémentation pas à pas et exemples de code fonctionnels.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Streaming

---

# Capture vidéo au format de fichier DV : guide d'implémentation

Le Digital Video (DV) reste un format fiable pour les applications de capture vidéo, en particulier lorsque l'on travaille avec des systèmes hérités ou des exigences professionnelles spécifiques. Ce guide explique comment implémenter la fonctionnalité de capture vidéo DV dans vos applications Delphi, avec des exemples C++ MFC et VB6 supplémentaires comme référence multiplateforme.

## Comprendre les options du format DV

Le format DV offre plusieurs avantages pour les applications de capture vidéo :

- Qualité constante avec perte de génération minimale
- Stockage efficace pour le contenu vidéo professionnel
- Prise en charge des standards PAL et NTSC
- Compatibilité avec les logiciels de montage vidéo professionnels
- Synchronisation audio fiable

Lors de l'implémentation de la capture vidéo DV, les développeurs disposent de deux approches principales :

1. **Capture Direct Stream** — Données DV brutes sans recompression
2. **DV recompressé** — Vidéo traitée avec des paramètres personnalisables

Chaque approche répond à des cas d'usage différents selon les besoins de votre application.

## Implémentation de la capture Direct Stream

La capture Direct Stream offre la plus haute qualité en évitant toute recompression du signal vidéo. Cette méthode est idéale à des fins d'archivage et de production vidéo professionnelle où la préservation de l'intégrité du signal original est cruciale.

### Configuration des paramètres de type DV

La première étape de l'implémentation de la capture Direct Stream consiste à définir la configuration de type DV appropriée :

#### Delphi

```pascal
VideoCapture1.DV_Capture_Type2 := rbDVType2.Checked;
```

#### C++ MFC

```cpp
m_videoCapture.put_DV_Capture_Type2(m_rbDVType2.GetCheck() == BST_CHECKED);
```

#### VB6

```vb
VideoCapture1.DV_Capture_Type2 = rbDVType2.Value
```

Le paramètre DV Type détermine la variation de format spécifique utilisée pour la capture. La plupart des applications modernes utilisent Type 2, qui offre une meilleure compatibilité avec les logiciels de montage.

### Définition du format de sortie pour Direct Stream

Pour la capture Direct Stream, vous devez spécifier le format DirectStream_DV :

#### Delphi

```pascal
VideoCapture1.OutputFormat := Format_DirectStream_DV;
```

#### C++ MFC

```cpp
m_videoCapture.SetOutputFormat(Format_DirectStream_DV);
```

#### VB6

```vb
VideoCapture1.OutputFormat = Format_DirectStream_DV
```

Cela garantit que les données vidéo sont stockées sans traitement ni compression supplémentaires.

### Configuration du mode de capture

Ensuite, définissez le composant en mode de capture vidéo :

#### Delphi

```pascal
VideoCapture1.Mode := Mode_Video_Capture;
```

#### C++ MFC

```cpp
m_videoCapture.SetMode(Mode_Video_Capture);
```

#### VB6

```vb
VideoCapture1.Mode = Mode_Video_Capture
```

Cela prépare le composant à une acquisition vidéo continue plutôt qu'à une capture d'image unique.

### Lancement de la capture Direct Stream

Une fois tous les paramètres en place, vous pouvez démarrer le processus de capture :

#### Delphi

```pascal
VideoCapture1.Start;
```

#### C++ MFC

```cpp
m_videoCapture.Start();
```

#### VB6

```vb
VideoCapture1.Start
```

Le composant capturera désormais le flux vidéo directement vers l'emplacement de sortie spécifié au format DV.

## Implémentation de la capture DV avec recompression

Dans certains scénarios, vous devrez peut-être modifier le flux DV pendant la capture. Cette approche permet la personnalisation des paramètres audio et des standards de format vidéo.

### Configuration des paramètres audio

Le format DV prend en charge plusieurs configurations audio. Définissez les canaux et la fréquence d'échantillonnage en fonction de vos besoins :

#### Delphi

```pascal
VideoCapture1.DV_Capture_Audio_Channels := StrToInt(cbDVChannels.Items[cbDVChannels.ItemIndex]);
VideoCapture1.DV_Capture_Audio_SampleRate := StrToInt(cbDVSampleRate.Items[cbDVSampleRate.ItemIndex]);
```

#### C++ MFC

```cpp
CString channelStr, sampleRateStr;
m_cbDVChannels.GetLBText(m_cbDVChannels.GetCurSel(), channelStr);
m_cbDVSampleRate.GetLBText(m_cbDVSampleRate.GetCurSel(), sampleRateStr);

m_videoCapture.put_DV_Capture_Audio_Channels(_ttoi(channelStr));
m_videoCapture.put_DV_Capture_Audio_SampleRate(_ttoi(sampleRateStr));
```

#### VB6

```vb
VideoCapture1.DV_Capture_Audio_Channels = CInt(cbDVChannels.List(cbDVChannels.ListIndex))
VideoCapture1.DV_Capture_Audio_SampleRate = CInt(cbDVSampleRate.List(cbDVSampleRate.ListIndex))
```

Les options audio DV standards incluent :

- Canaux : 1 (mono) ou 2 (stéréo)
- Fréquences d'échantillonnage : 32000 Hz, 44100 Hz ou 48000 Hz

### Définition du standard de format vidéo

DV prend en charge les standards PAL et NTSC. Sélectionnez le standard approprié pour votre région cible :

#### Delphi

```pascal
if rbDVPAL.Checked then
  VideoCapture1.DV_Capture_Video_Format := DVF_PAL
else
  VideoCapture1.DV_Capture_Video_Format := DVF_NTSC;
```

#### C++ MFC

```cpp
if (m_rbDVPAL.GetCheck() == BST_CHECKED)
  m_videoCapture.put_DV_Capture_Video_Format(DVF_PAL);
else
  m_videoCapture.put_DV_Capture_Video_Format(DVF_NTSC);
```

#### VB6

```vb
If rbDVPAL.Value Then
  VideoCapture1.DV_Capture_Video_Format = DVF_PAL
Else
  VideoCapture1.DV_Capture_Video_Format = DVF_NTSC
End If
```

N'oubliez pas que :

- PAL : résolution 720×576 à 25 fps (utilisé en Europe, Australie, certaines parties de l'Asie)
- NTSC : résolution 720×480 à 29,97 fps (utilisé en Amérique du Nord, au Japon, dans certaines parties de l'Amérique du Sud)

### Sélection du type DV

Comme pour la diffusion directe, spécifiez le type DV pour la capture recompressée :

#### Delphi

```pascal
VideoCapture1.DV_Capture_Type2 := rbDVType2.Checked;
```

#### C++ MFC

```cpp
m_videoCapture.put_DV_Capture_Type2(m_rbDVType2.GetCheck() == BST_CHECKED);
```

#### VB6

```vb
VideoCapture1.DV_Capture_Type2 = rbDVType2.Value
```

### Définition du format de sortie pour la recompression

Pour la capture DV recompressée, spécifiez le format DV plutôt que DirectStream_DV :

#### Delphi

```pascal
VideoCapture1.OutputFormat := Format_DV;
VideoCapture1.Mode := Mode_Video_Capture;
```

#### C++ MFC

```cpp
m_videoCapture.SetOutputFormat(Format_DV);
m_videoCapture.SetMode(Mode_Video_Capture);
```

#### VB6

```vb
VideoCapture1.OutputFormat = Format_DV
VideoCapture1.Mode = Mode_Video_Capture
```

Cela indique au composant de traiter le flux à travers le codec DV pendant la capture.

### Démarrage de la capture recompressée

Avec tous les paramètres configurés, démarrez le processus de capture :

#### Delphi

```pascal
VideoCapture1.Start;
```

#### C++ MFC

```cpp
m_videoCapture.Start();
```

#### VB6

```vb
VideoCapture1.Start
```

## Bonnes pratiques pour l'implémentation de la capture DV

Lors de l'implémentation de la capture DV dans vos applications, prenez en compte ces recommandations :

1. **Pré-allouez un espace disque suffisant** — Le format DV nécessite environ 13 Go par heure de séquence
2. **Implémentez des limites de durée de capture** — Les fichiers DV ont une limite de taille de 4 Go sur certains systèmes de fichiers
3. **Surveillez les ressources système** — La capture DV requiert des performances CPU et disque constantes
4. **Fournissez une UI de sélection de format** — Permettez aux utilisateurs de choisir entre les options Direct Stream et recompressée
5. **Testez avec différents modèles de caméra** — L'implémentation DV peut varier d'un fabricant à l'autre

## Considérations de gestion des erreurs

Les implémentations robustes de capture DV doivent inclure une gestion des erreurs pour ces scénarios courants :

- Déconnexion du périphérique pendant la capture
- Épuisement de l'espace disque
- Conditions de dépassement de tampon
- Paramètres de format invalides
- Problèmes de compatibilité de codec

## Conclusion

L'implémentation de la capture vidéo DV dans vos applications Delphi, C++ MFC ou VB6 fournit une base solide pour les flux de travail d'acquisition vidéo professionnels. Que vous choisissiez la capture Direct Stream pour une qualité maximale ou la capture recompressée pour plus de flexibilité, le format DV offre des performances fiables pour les applications vidéo spécialisées.

En suivant les exemples d'implémentation de ce guide, vous pouvez intégrer des capacités de capture vidéo de qualité professionnelle dans vos solutions logicielles personnalisées.

---
Besoin d'aide supplémentaire pour votre implémentation de capture vidéo ? Visitez notre page [GitHub](https://github.com/visioforge/) pour plus d'exemples de code ou contactez notre [équipe de support](https://support.visioforge.com/) pour des conseils personnalisés.
