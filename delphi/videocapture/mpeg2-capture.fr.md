---
title: Capture vidéo MPEG-2 en Delphi avec tuner TV matériel
description: Capture MPEG-2 en Delphi avec encodeurs matériels de tuner TV — énumération, configuration de format et exemples de code.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Capture
  - Encoding
  - TV Tuner
  - MPEG-2

---

# Capture vidéo MPEG-2 en Delphi avec des encodeurs matériels de tuner TV

Ce tutoriel complet montre comment implémenter une capture vidéo MPEG-2 de haute qualité dans vos applications Delphi en tirant parti des tuners TV dotés de capacités d'encodage matériel intégrées. L'encodage matériel réduit considérablement l'utilisation du CPU tout en conservant une excellente qualité vidéo.

## Vue d'ensemble de l'encodage matériel MPEG-2

Les encodeurs matériels MPEG-2 offrent des performances supérieures à celles des solutions d'encodage logicielles. Ils sont particulièrement utiles pour développer des applications de capture vidéo professionnelles qui nécessitent un traitement efficace et une sortie de haute qualité.

## Énumération des encodeurs matériels MPEG-2 disponibles

La première étape consiste à identifier tous les encodeurs matériels MPEG-2 disponibles dans le système. Ce code montre comment remplir une liste déroulante avec les périphériques détectés :

```pascal
// Lister tous les encodeurs matériels MPEG-2 disponibles dans le système
// Cela aide les utilisateurs à sélectionner le périphérique d'encodage approprié
VideoCapture1.Special_Filters_Fill;
for I := 0 to VideoCapture1.Special_Filters_GetCount(SF_Hardware_Video_Encoder) - 1 do
  cbMPEGEncoder.Items.Add(VideoCapture1.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i));
```

```cpp
// Implémentation C++ MFC pour l'énumération des encodeurs MPEG-2
// Remplit une combobox avec tous les encodeurs matériels détectés
m_VideoCapture.Special_Filters_Fill();
for (int i = 0; i < m_VideoCapture.Special_Filters_GetCount(SF_Hardware_Video_Encoder); i++)
{
  CString encoderName = m_VideoCapture.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i);
  m_cbMPEGEncoder.AddString(encoderName);
}
```

```vb
' Implémentation VB6 pour trouver les encodeurs MPEG-2 matériels
' Liste tous les encodeurs disponibles dans un contrôle combobox
VideoCapture1.Special_Filters_Fill
For i = 0 To VideoCapture1.Special_Filters_GetCount(SF_Hardware_Video_Encoder) - 1
  cbMPEGEncoder.AddItem VideoCapture1.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i)
Next i
```

## Sélection d'un encodeur MPEG-2 spécifique

Après avoir énuméré les encodeurs disponibles, l'étape suivante consiste à sélectionner un encodeur spécifique à utiliser :

```pascal
// Configurer le composant pour utiliser l'encodeur matériel MPEG-2 sélectionné
// Cela doit être fait avant de démarrer le processus de capture
VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name := cbMPEGEncoder.Items[cbMPEGEncoder.ItemIndex];
```

```cpp
// C++ MFC : Sélectionner et configurer l'encodeur matériel MPEG-2 choisi
// Récupère le nom de l'encodeur sélectionné depuis la combobox
int nIndex = m_cbMPEGEncoder.GetCurSel();
CString encoderName;
m_cbMPEGEncoder.GetLBText(nIndex, encoderName);
m_VideoCapture.Video_CaptureDevice_InternalMPEGEncoder_Name = encoderName;
```

```vb
' VB6 : Définir l'encodeur sélectionné comme encodeur matériel MPEG-2 actif
' Doit être appelé avant l'initialisation du graphe de capture
VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name = cbMPEGEncoder.List(cbMPEGEncoder.ListIndex)
```

## Configuration du format DirectStream MPEG pour la sortie

Pour capturer correctement la vidéo encodée en MPEG-2, vous devez définir le format de sortie approprié :

```pascal
// Définir le format de sortie sur DirectStream MPEG
// Cela permet une gestion correcte des flux MPEG-2 encodés matériellement
VideoCapture1.OutputFormat := Format_DirectStream_MPEG;
```

```cpp
// C++ MFC : Configurer le format de sortie pour le contenu encodé en MPEG-2
// Le format DirectStream MPEG préserve le flux encodé matériellement
m_VideoCapture.OutputFormat = Format_DirectStream_MPEG;
```

```vb
' VB6 : Définir le format de sortie approprié pour l'encodage matériel MPEG-2
' Le format DirectStream garantit que les données encodées sont traitées correctement
VideoCapture1.OutputFormat = Format_DirectStream_MPEG
```

## Établissement du mode de capture vidéo

Avant de démarrer le processus de capture, définissez le composant en mode de capture vidéo :

```pascal
// Configurer le composant pour l'opération de capture vidéo
// Cela prépare le graphe DirectShow interne pour l'enregistrement
VideoCapture1.Mode := Mode_Video_Capture;
```

```cpp
// C++ MFC : Définir le composant en mode de capture vidéo
// Requis avant de démarrer le processus de capture MPEG-2
m_VideoCapture.Mode = Mode_Video_Capture;
```

```vb
' VB6 : Définir le mode de capture vidéo avant de commencer l'enregistrement
' Cela initialise les filtres DirectShow appropriés
VideoCapture1.Mode = Mode_Video_Capture
```

## Lancement du processus de capture MPEG-2

Enfin, démarrez le processus de capture pour commencer à enregistrer la vidéo MPEG-2 :

```pascal
// Lancer le processus de capture vidéo avec les paramètres configurés
// Le composant commencera désormais à enregistrer vers la sortie spécifiée
VideoCapture1.Start;
```

```cpp
// C++ MFC : Démarrer le processus de capture vidéo MPEG-2
// L'enregistrement commence avec les paramètres précédemment configurés
m_VideoCapture.Start();
```

```vb
' VB6 : Démarrer la capture vidéo avec la configuration actuelle
' L'encodeur matériel commencera désormais à traiter les données vidéo
VideoCapture1.Start
```

## Considérations avancées pour la capture MPEG-2

Lors de l'implémentation de la capture MPEG-2 avec des encodeurs matériels, prenez en compte ces facteurs supplémentaires :

1. Les encodeurs matériels offrent généralement de meilleures performances que les solutions logicielles
2. Certains tuners TV fournissent des paramètres d'encodage supplémentaires personnalisables
3. La taille des tampons peut nécessiter un ajustement pour des captures de qualité supérieure
4. Les encodeurs matériels gèrent souvent la mise à l'échelle vidéo et la conversion de fréquence d'images en interne

## Dépannage des problèmes courants

Si vous rencontrez des problèmes avec l'encodage matériel MPEG-2 :

1. Vérifiez que votre périphérique tuner TV prend en charge l'encodage matériel MPEG-2
2. Assurez-vous que les pilotes du périphérique de capture sont correctement installés
3. Vérifiez que DirectX est correctement installé et à jour
4. Tenez compte de la disponibilité des ressources système, car certains encodeurs nécessitent des ressources spécifiques

Veuillez contacter notre équipe de support dédiée pour obtenir de l'aide concernant l'implémentation de ce tutoriel dans votre application spécifique. Visitez notre dépôt GitHub pour des exemples de code et des exemples d'implémentation supplémentaires.
