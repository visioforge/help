---
title: Sélection du crossbar d'entrée vidéo en Delphi — VCL/ActiveX
description: Sélectionnez les sources d'entrée vidéo en Delphi avec le crossbar — configurez entrées composite, S-Vidéo, HDMI avec exemples de code pas à pas.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Decoding
  - TV Tuner

---

# Sélection des sources d'entrée vidéo avec la technologie crossbar

## Introduction à la sélection d'entrée vidéo

Lors du développement d'applications qui capturent la vidéo depuis des périphériques externes, vous aurez souvent besoin de gérer plusieurs sources d'entrée. Le crossbar est un composant essentiel des systèmes de capture vidéo qui vous permet de router différentes entrées physiques (telles que composite, S-Vidéo, HDMI) vers votre application. Ce guide vous accompagne dans le processus de détection, de configuration et de sélection des entrées vidéo à l'aide de l'interface crossbar dans les applications Delphi, C++ MFC et Visual Basic 6.

## Comprendre la technologie crossbar

La technologie crossbar fonctionne comme une matrice de routage dans les périphériques de capture vidéo, permettant la connexion entre diverses entrées et sorties. Les cartes de capture et tuners TV modernes intègrent fréquemment la fonctionnalité crossbar pour faciliter la commutation entre différentes sources vidéo telles que :

- Entrées vidéo composite
- Connexions S-Vidéo
- Vidéo en composantes
- Entrées HDMI
- Entrées tuner TV
- Interfaces vidéo numériques

La configuration correcte de ces connexions par programmation est essentielle pour les applications qui doivent basculer dynamiquement entre différentes sources vidéo.

## Vue d'ensemble des étapes d'implémentation

Le processus d'implémentation pour configurer les connexions crossbar dans votre application comprend trois étapes principales :

1. Initialiser l'interface crossbar et vérifier sa disponibilité
2. Énumérer les entrées vidéo disponibles pour la sélection
3. Connecter l'entrée sélectionnée à la sortie du décodeur vidéo

Examinons chaque étape en détail avec des exemples de code pour les environnements Delphi, C++ MFC et VB6.

## Guide d'implémentation détaillé

### Étape 1 : initialiser l'interface crossbar

Avant de pouvoir travailler avec la sélection d'entrée, vous devez initialiser l'interface crossbar et vérifier qu'elle est disponible sur le périphérique de capture actuel.

#### Implémentation Delphi

```pascal
// Initialiser l'interface crossbar
CrossBarFound := VideoCapture1.Video_CaptureDevice_CrossBar_Init;

// Vérifier si la fonctionnalité crossbar est disponible
if CrossBarFound then
  ShowMessage('Crossbar functionality detected and initialized')
else
  ShowMessage('No crossbar available on this capture device');
```

#### Implémentation C++ MFC

```cpp
// Initialiser l'interface crossbar
BOOL bCrossBarFound = m_videoCapture.Video_CaptureDevice_CrossBar_Init();

// Vérifier si la fonctionnalité crossbar est disponible
if (bCrossBarFound) {
    AfxMessageBox(_T("Crossbar functionality detected and initialized"));
} else {
    AfxMessageBox(_T("No crossbar available on this capture device"));
}
```

#### Implémentation VB6

```vb
' Initialiser l'interface crossbar
Dim CrossBarFound As Boolean
CrossBarFound = VideoCapture1.Video_CaptureDevice_CrossBar_Init()

' Vérifier si la fonctionnalité crossbar est disponible
If CrossBarFound Then
    MsgBox "Crossbar functionality detected and initialized"
Else
    MsgBox "No crossbar available on this capture device"
End If
```

La fonction d'initialisation renvoie une valeur booléenne indiquant si la fonctionnalité crossbar est disponible sur le périphérique de capture actuel. Tous les périphériques de capture ne prennent pas en charge la fonctionnalité crossbar, cette vérification est donc cruciale.

### Étape 2 : énumérer les entrées vidéo disponibles

Une fois que vous avez confirmé que le crossbar est disponible, l'étape suivante consiste à récupérer la liste des entrées disponibles pour la sortie « Video Decoder ». Cela permet aux utilisateurs de sélectionner parmi les connexions physiques disponibles.

#### Implémentation Delphi

```pascal
// Variables déclarées au niveau de la procédure pour que l'extrait se compile sur tout IDE Delphi 6+
// (la forme `var name: T := ...` en ligne nécessite Delphi 10.3 Rio ou ultérieur).
procedure TForm1.PopulateCrossBarInputs;
var
  inputCount: Integer;
  inputName: String;
  i: Integer;
begin
  // Effacer les connexions existantes et les éléments d'UI
  VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections;
  cbCrossbarVideoInput.Clear;

  // Obtenir le nombre d'entrées disponibles pour la sortie « Video Decoder »
  inputCount := VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount('Video Decoder');

  // Remplir l'UI avec les entrées disponibles
  for i := 0 to inputCount - 1 do begin
    inputName := VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem('Video Decoder', i);
    cbCrossbarVideoInput.Items.Add(inputName);
  end;

  // Sélectionner le premier élément par défaut s'il est disponible
  if cbCrossbarVideoInput.Items.Count > 0 then
    cbCrossbarVideoInput.ItemIndex := 0;
end;
```

#### Implémentation C++ MFC

```cpp
// Effacer les connexions existantes et les éléments d'UI
m_videoCapture.Video_CaptureDevice_CrossBar_ClearConnections();
m_comboVideoInputs.ResetContent();

// Obtenir le nombre d'entrées disponibles pour la sortie « Video Decoder »
int inputCount = m_videoCapture.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount(_T("Video Decoder"));

// Remplir l'UI avec les entrées disponibles
for (int i = 0; i < inputCount; i++) {
    CString inputName = m_videoCapture.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem(_T("Video Decoder"), i);
    m_comboVideoInputs.AddString(inputName);
}

// Sélectionner le premier élément par défaut s'il est disponible
if (m_comboVideoInputs.GetCount() > 0) {
    m_comboVideoInputs.SetCurSel(0);
}
```

#### Implémentation VB6

```vb
' Effacer les connexions existantes et les éléments d'UI
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections
cboVideoInputs.Clear

' Obtenir le nombre d'entrées disponibles pour la sortie « Video Decoder »
Dim inputCount As Integer
inputCount = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount("Video Decoder")

' Remplir l'UI avec les entrées disponibles
Dim i As Integer
Dim inputName As String
For i = 0 To inputCount - 1
    inputName = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem("Video Decoder", i)
    cboVideoInputs.AddItem inputName
Next i

' Sélectionner le premier élément par défaut s'il est disponible
If cboVideoInputs.ListCount > 0 Then
    cboVideoInputs.ListIndex = 0
End If
```

Les types d'entrée courants que vous pouvez rencontrer incluent :

- Composite
- S-Vidéo
- HDMI
- Composantes
- Tuner TV

La liste exacte dépend des capacités spécifiques de votre matériel de capture.

### Étape 3 : appliquer l'entrée sélectionnée

Après que l'utilisateur ait sélectionné sa source d'entrée souhaitée, vous devez appliquer cette sélection en établissant une connexion entre l'entrée sélectionnée et la sortie du décodeur vidéo.

#### Implémentation Delphi

```pascal
// Variables au niveau de la procédure — compatible Delphi 6+ (évite la forme `var`
// en ligne abrégée introduite dans Delphi 10.3 Rio).
procedure TForm1.ApplySelectedCrossBarInput;
var
  selectedInput: String;
  success: Boolean;
begin
  // Effacer d'abord toutes les connexions existantes
  VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections;

  // Connecter l'entrée sélectionnée à la sortie « Video Decoder »
  // Paramètres : nom de l'entrée, nom de la sortie, routage automatique du signal
  if cbCrossbarVideoInput.ItemIndex >= 0 then begin
    selectedInput := cbCrossbarVideoInput.Items[cbCrossbarVideoInput.ItemIndex];
    success := VideoCapture1.Video_CaptureDevice_CrossBar_Connect(selectedInput, 'Video Decoder', true);

    if success then
      ShowMessage('Successfully connected ' + selectedInput + ' to Video Decoder')
    else
      ShowMessage('Failed to establish connection');
  end;
end;
```

#### Implémentation C++ MFC

```cpp
// Effacer d'abord toutes les connexions existantes
m_videoCapture.Video_CaptureDevice_CrossBar_ClearConnections();

// Connecter l'entrée sélectionnée à la sortie « Video Decoder »
// Paramètres : nom de l'entrée, nom de la sortie, routage automatique du signal
int selectedIndex = m_comboVideoInputs.GetCurSel();
if (selectedIndex >= 0) {
    CString selectedInput;
    m_comboVideoInputs.GetLBText(selectedIndex, selectedInput);
    
    BOOL success = m_videoCapture.Video_CaptureDevice_CrossBar_Connect(
        selectedInput, _T("Video Decoder"), TRUE);
    
    if (success) {
        CString msg;
        msg.Format(_T("Successfully connected %s to Video Decoder"), selectedInput);
        AfxMessageBox(msg);
    } else {
        AfxMessageBox(_T("Failed to establish connection"));
    }
}
```

#### Implémentation VB6

```vb
' Effacer d'abord toutes les connexions existantes
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections

' Connecter l'entrée sélectionnée à la sortie « Video Decoder »
' Paramètres : nom de l'entrée, nom de la sortie, routage automatique du signal
If cboVideoInputs.ListIndex >= 0 Then
    Dim selectedInput As String
    selectedInput = cboVideoInputs.Text
    
    Dim success As Boolean
    success = VideoCapture1.Video_CaptureDevice_CrossBar_Connect(selectedInput, "Video Decoder", True)
    
    If success Then
        MsgBox "Successfully connected " & selectedInput & " to Video Decoder"
    Else
        MsgBox "Failed to establish connection"
    End If
End If
```

Le troisième paramètre (`true`) active le routage automatique du signal, ce qui permet de gérer des scénarios de connexion complexes où un routage intermédiaire peut être nécessaire.

## Bonnes pratiques pour l'implémentation du crossbar

Pour une sélection robuste des entrées vidéo dans vos applications :

1. **Initialisez toujours le crossbar en premier** : vérifiez la disponibilité avant de tenter des opérations
2. **Effacez les connexions existantes** : avant d'établir une nouvelle connexion, effacez toutes celles existantes
3. **Gérez l'absence de crossbar avec élégance** : prévoyez des solutions de repli lorsque la fonctionnalité crossbar n'est pas disponible
4. **Validez les sélections** : assurez-vous qu'une entrée valide est sélectionnée avant de tenter d'établir des connexions
5. **Fournissez un retour à l'utilisateur** : informez les utilisateurs des tentatives de connexion réussies ou échouées

## Dépannage des problèmes courants

Si vous rencontrez des problèmes avec les connexions crossbar :

- Vérifiez que votre périphérique de capture prend en charge la fonctionnalité crossbar
- Vérifiez que les noms d'entrée et de sortie correspondent exactement à ce que le périphérique signale
- Assurez-vous que le pilote du périphérique est correctement installé
- Utilisez la journalisation de débogage pour suivre les tentatives de connexion
- Testez avec différentes sources d'entrée pour isoler les problèmes spécifiques au matériel

## Conclusion

L'implémentation correcte de la technologie crossbar dans vos applications de capture vidéo donne aux utilisateurs la flexibilité de travailler avec plusieurs sources d'entrée. En suivant les étapes décrites dans ce guide, vous pouvez créer un système de sélection d'entrée vidéo robuste et convivial pour vos applications, que vous développiez en Delphi, C++ MFC ou Visual Basic 6.

Les exemples de code fournis montrent comment initialiser le crossbar, énumérer les entrées disponibles et connecter les entrées sélectionnées à la sortie du décodeur vidéo. Avec ces fondamentaux en place, vous pouvez créer des applications de capture vidéo sophistiquées qui prennent en charge un large éventail de périphériques d'entrée et de types de connexion.

---
Pour obtenir une aide supplémentaire dans l'implémentation de cette fonctionnalité, explorez nos autres pages de documentation et notre dépôt d'exemples de code pour des techniques et des solutions plus avancées.
