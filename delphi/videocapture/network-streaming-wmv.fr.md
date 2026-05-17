---
title: Diffusion réseau WMV en Delphi et ActiveX — TVFVideoCapture
description: Diffusion réseau WMV en Delphi avec TVFVideoCapture — profils, connexions clients, ports, streaming vidéo et exemples de code pas à pas pour développeurs.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Streaming
  - Encoding
  - WMV

---

# Guide d'implémentation de la diffusion réseau WMV

## Vue d'ensemble

Ce guide montre comment implémenter une diffusion vidéo en réseau au format Windows Media Video (WMV) dans vos applications Delphi. Les techniques présentées ici vous permettent de diffuser du contenu vidéo sur les réseaux tout en capturant et en enregistrant simultanément la vidéo dans un fichier à des fins d'archivage.

## Prérequis

Avant d'implémenter la diffusion réseau WMV, assurez-vous de disposer de :

- Un périphérique de capture vidéo pris en charge connecté à votre système
- Un accès réseau et des autorisations appropriés
- Un fichier de profil WMV valide contenant les paramètres de l'encodeur

## Étapes d'implémentation

### Configuration de base

Pour activer la diffusion réseau WMV dans votre application, vous devrez configurer plusieurs paramètres essentiels :

1. Activer la fonctionnalité de diffusion réseau
2. Spécifier un fichier de profil WMV contenant les paramètres d'encodage vidéo
3. Définir le nombre maximal de connexions clients simultanées
4. Définir le port réseau pour les connexions clients

### Code d'implémentation Delphi

```pascal
// Code Delphi pour configurer la diffusion réseau WMV
// Activer la fonctionnalité de diffusion réseau
VideoCapture1.Network_Streaming_Enabled := true;

// Définir le chemin du fichier de profil WMV contenant les paramètres de l'encodeur
// Ce fichier définit la qualité vidéo, le débit binaire, la résolution, etc.
VideoCapture1.Network_Streaming_WMV_Profile_FileName := edNetworkStreamingWMVProfile.Text;

// Définir le nombre maximal de clients simultanés autorisés à se connecter
VideoCapture1.Network_Streaming_Maximum_Clients := StrToInt(edMaximumClients.Text);

// Spécifier le port réseau que les clients utiliseront pour se connecter
VideoCapture1.Network_Streaming_Network_Port := StrToInt(edNetworkPort.Text);
```

### Implémentation C++ MFC

```cpp
// Implémentation C++ MFC pour la diffusion réseau WMV.
// CEdit::GetWindowText prend une référence CString et y écrit (renvoie void),
// donc capturez la valeur via une CString locale et passez-la au setter COM.
CString profile, maxClients, port;
edNetworkStreamingWMVProfile.GetWindowText(profile);
edMaximumClients.GetWindowText(maxClients);
edNetworkPort.GetWindowText(port);

// Activer la fonctionnalité de diffusion
m_VideoCapture.SetNetwork_Streaming_Enabled(true);

// Définir le chemin du profil WMV - contient les paramètres d'encodage
m_VideoCapture.SetNetwork_Streaming_WMV_Profile_FileName(profile);

// Définir le nombre maximal de connexions clients simultanées
m_VideoCapture.SetNetwork_Streaming_Maximum_Clients(_ttoi(maxClients));

// Définir le port réseau pour les connexions clients
m_VideoCapture.SetNetwork_Streaming_Network_Port(_ttoi(port));
```

### Implémentation VB6

```vb
' Implémentation VB6 (ActiveX) pour la diffusion réseau WMV
' Activer les capacités de diffusion réseau
VideoCapture1.Network_Streaming_Enabled = True

' Définir le fichier de profil contenant les paramètres de l'encodeur vidéo
VideoCapture1.Network_Streaming_WMV_Profile_FileName = txtNetworkStreamingWMVProfile.Text

' Définir le nombre maximal de clients autorisés à se connecter simultanément
VideoCapture1.Network_Streaming_Maximum_Clients = CInt(txtMaximumClients.Text)

' Spécifier le port réseau pour les connexions clients
VideoCapture1.Network_Streaming_Network_Port = CInt(txtNetworkPort.Text)
```

## Informations de connexion client

Après avoir configuré les paramètres de diffusion, votre application peut obtenir l'URL de connexion que les clients utiliseront pour accéder au flux vidéo :

```pascal
// Obtenir l'URL que les clients utiliseront pour se connecter au flux
// Cette URL peut être partagée avec les utilisateurs qui doivent visionner le flux
strStreamURL := VideoCapture1.Network_Streaming_URL;
```

Cette URL peut être utilisée avec Windows Media Player ou toute autre application prenant en charge les protocoles de diffusion Windows Media.

## Bonnes pratiques

Pour des performances de diffusion optimales, tenez compte des recommandations suivantes :

- Utilisez des débits binaires adaptés aux capacités de votre réseau
- Surveillez les connexions clients pour garantir la stabilité du système
- Testez votre configuration de diffusion avec diverses applications clientes
- Tenez compte des limitations de bande passante du réseau lors du réglage des paramètres de qualité

## Dépannage

Si vous rencontrez des problèmes avec votre implémentation de diffusion :

- Vérifiez que les paramètres du pare-feu réseau autorisent le trafic sur le port sélectionné
- Assurez-vous que le fichier de profil WMV existe et contient des paramètres valides
- Vérifiez que le nombre maximal de clients est approprié pour les ressources de votre serveur
- Validez la connectivité réseau entre le serveur et les clients potentiels

---
Veuillez contacter le [support](https://support.visioforge.com/) si vous avez des questions concernant cette implémentation. Visitez notre page [GitHub](https://github.com/visioforge/) pour des exemples de code et des ressources supplémentaires.
