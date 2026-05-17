---
title: Guide intégration tuner TV et radio FM pour applis .NET C#
description: Implémentez la radio FM et le tuning TV en C# pour scanner les fréquences, gérer les chaînes et intégrer les capacités de diffusion en .NET.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - TV Tuner
  - C#
  - NuGet
primary_api_classes:
  - TVTunerTuneChannelsEventArgs

---
# Intégration de la radio FM et du tuning TV dans des applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction à l'intégration de diffusion

Les applications .NET modernes peuvent tirer parti des capacités matérielles pour fournir des fonctionnalités de radio FM et de tuning TV. Ce guide montre comment implémenter ces fonctionnalités dans vos applications C#, qu'il s'agisse d'applications WPF, WinForms ou console. En suivant ces exemples, vous serez en mesure de détecter les périphériques tuner disponibles, de scanner les fréquences, de gérer les chaînes et d'offrir une expérience de diffusion complète à vos utilisateurs.

## Exigences matérielles

Avant d'implémenter les exemples de code ci-dessous, assurez-vous que votre système de développement possède :

1. Une carte tuner TV ou un périphérique USB compatible
2. Une installation correcte des pilotes
3. .NET Framework 4.7+ ou .NET Core 3.1+/NET 5.0+ pour les applications modernes

## Détecter les périphériques tuner disponibles

La première étape de l'implémentation de la fonctionnalité tuner consiste à détecter tous les périphériques tuner disponibles sur le système. Cela permet aux utilisateurs de sélectionner le matériel approprié à leurs besoins.

```cs
// Remplir une liste déroulante avec tous les périphériques tuner TV disponibles
foreach (var tunerDevice in VideoCapture1.TVTuner_Devices())
{
  cbTVTuner.Items.Add(tunerDevice);
}
```

Vous pouvez ensuite laisser les utilisateurs sélectionner leur périphérique préféré dans la liste remplie, ou sélectionner automatiquement le premier périphérique disponible pour une expérience simplifiée.

## Bases de la configuration

### Sélection du format TV

Différentes régions utilisent différents standards de diffusion. Votre application doit détecter et permettre la sélection du standard approprié :

```cs
// Lister tous les formats TV pris en charge (PAL, NTSC, SECAM, etc.)
foreach (var tunerTVFormat in VideoCapture1.TVTuner_TVFormats())
{
  cbTVSystem.Items.Add(tunerTVFormat);
}
```

### Paramètres régionaux

Les fréquences de diffusion varient selon les pays. Configurez votre application avec les paramètres régionaux corrects :

```cs
// Remplir la sélection de pays pour les fréquences spécifiques à la région
foreach (var tunerCountry in VideoCapture1.TVTuner_Countries())
{
  cbTVCountry.Items.Add(tunerCountry);
}
```

## Configurer le tuner

Après avoir détecté les périphériques disponibles, vous devez sélectionner et initialiser le tuner :

```cs
// Sélectionner le périphérique tuner TV
VideoCapture1.TVTuner_Name = cbTVTuner.Text;

// Initialiser le tuner et lire ses paramètres actuels
await VideoCapture1.TVTuner_ReadAsync();
```

Ce processus d'initialisation préparera le tuner pour les opérations ultérieures et lira sa configuration actuelle.

## Travailler avec différentes sources de signal

La plupart des tuners prennent en charge plusieurs types d'entrée. Vous devrez déterminer quels modes sont disponibles :

```cs
// Obtenir tous les modes disponibles (TV, FM Radio, etc.)
foreach (var tunerMode in VideoCapture1.TVTuner_Modes())
{
  cbTVMode.Items.Add(tunerMode);
}
```

Puis sélectionnez la source d'entrée appropriée :

```cs
// Sélectionner la source du signal (Antenne, Câble, etc.)
cbTVInput.SelectedIndex = cbTVInput.Items.IndexOf(VideoCapture1.TVTuner_InputType);

// Sélectionner le mode de fonctionnement (TV, Radio FM, etc.)
cbTVMode.SelectedIndex = cbTVMode.Items.IndexOf(VideoCapture1.TVTuner_Mode);
```

## Gestion avancée des fréquences

Pour un contrôle détaillé, vous pouvez travailler directement avec les valeurs de fréquence :

```cs
// Afficher les paramètres de fréquence actuels
edVideoFreq.Text = Convert.ToString(VideoCapture1.TVTuner_VideoFrequency());
edAudioFreq.Text = Convert.ToString(VideoCapture1.TVTuner_AudioFrequency());
```

Ces valeurs peuvent être utiles pour le débogage ou pour créer des interfaces personnalisées de sélection de fréquence.

## Définir les standards du système de diffusion

Différentes régions utilisent différents standards de diffusion. Configurez votre application avec le bon système :

```cs
// Sélectionner le système TV (PAL, NTSC, SECAM, etc.)
cbTVSystem.SelectedIndex = cbTVSystem.Items.IndexOf(VideoCapture1.TVTuner_TVFormat);

// Sélectionner le pays pour les fréquences spécifiques à la région
cbTVCountry.SelectedIndex = cbTVCountry.Items.IndexOf(VideoCapture1.TVTuner_Country);
```

## Balayage automatique des chaînes

L'une des fonctionnalités les plus importantes est la possibilité de scanner et de détecter automatiquement les chaînes disponibles. Cela nécessite l'implémentation d'un gestionnaire d'événements pour recevoir les résultats du balayage :

```cs
private void VideoCapture1_OnTVTunerTuneChannels(object sender, TVTunerTuneChannelsEventArgs e)
{
  // Mettre à jour la barre de progression
  pbChannels.Value = e.Progress;

  // Si un signal est détecté, ajouter la chaîne à la liste
  if (e.SignalPresent)
  {
    cbTVChannel.Items.Add(e.Channel.ToString());
  }

  // Vérifier si le balayage est terminé
  if (e.Channel == -1)
  {
    pbChannels.Value = 0;
    MessageBox.Show("Channel scanning complete");
  }

  // Garder l'interface réactive pendant le balayage
  Application.DoEvents();
}
```

Ce gestionnaire d'événements sera appelé pour chaque fréquence scannée, vous permettant de mettre à jour votre interface et de collecter les chaînes trouvées.

## Lancer le processus de balayage de chaînes

Une fois le gestionnaire d'événements en place, vous pouvez démarrer le processus de balayage :

```cs
const int KHz = 1000;
const int MHz = 1000000; 

// Initialiser le tuner et effacer la liste de chaînes précédente
await VideoCapture1.TVTuner_ReadAsync(); 
cbTVChannel.Items.Clear();

// Pour le mode radio FM, configurer les paramètres de balayage
if ((cbTVMode.SelectedIndex != -1) && (cbTVMode.Text == "FM Radio")) 
{
  // Définir la plage de balayage FM de 100 MHz à 110 MHz
  VideoCapture1.TVTuner_FM_Tuning_StartFrequency = 100 * MHz; 
  VideoCapture1.TVTuner_FM_Tuning_StopFrequency = 110 * MHz; 
  
  // Balayer par incréments de 100 KHz
  VideoCapture1.TVTuner_FM_Tuning_Step = 100 * KHz;
}

// Lancer le processus de balayage
VideoCapture1.TVTuner_TuneChannels_Start();
```

Ce code prépare le tuner et démarre le balayage. Pour le mode radio FM, il définit des plages et des pas de fréquence spécifiques.

## Gestion manuelle des chaînes

En plus du balayage automatique, votre application doit permettre la sélection manuelle des chaînes :

### Définir une chaîne par numéro

```cs
// Définir un numéro de chaîne spécifique
VideoCapture1.TVTuner_Channel = Convert.ToInt32(edChannel.Text); 
await VideoCapture1.TVTuner_ApplyAsync();
```

### Définir une chaîne par fréquence

```cs
// Définir la chaîne à -1 pour permettre le réglage direct de la fréquence
VideoCapture1.TVTuner_Channel = -1; 

// Définir la fréquence spécifique en Hz
VideoCapture1.TVTuner_Frequency = Convert.ToInt32(edChannel.Text); 
await VideoCapture1.TVTuner_ApplyAsync();
```

Cette approche donne aux utilisateurs avancés plus de contrôle sur leur expérience de tuning.

## Optimiser l'expérience utilisateur

Pour la meilleure expérience utilisateur, envisagez d'implémenter ces fonctionnalités supplémentaires :

1. **Chaînes favorites** : permettre aux utilisateurs d'enregistrer et d'accéder rapidement à leurs chaînes préférées
2. **Indicateur de force du signal** : afficher la qualité actuelle du signal
3. **Informations sur la chaîne** : montrer les informations de programme lorsqu'elles sont disponibles
4. **Tâche planifiée d'autotuning** : scanner périodiquement les nouvelles chaînes

## Bonnes pratiques de gestion des erreurs

Une gestion robuste des erreurs est essentielle pour les applications de tuner :

1. Vérifier si le matériel est présent avant de tenter des opérations
2. Gérer les cas où aucun signal n'est détecté
3. Fournir des messages d'erreur clairs lorsque le tuning échoue
4. Implémenter des délais d'expiration pour les opérations de balayage

## Dépendances requises

Pour utiliser les fonctionnalités de radio FM et de tuning TV, incluez ces paquets :

- Redistribuables de capture vidéo :
  - [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

Vous pouvez ajouter ces paquets via le gestionnaire de paquets NuGet ou en éditant directement votre fichier .csproj.

## Considérations de performance

Lors de l'implémentation de la fonctionnalité tuner :

1. Exécutez les opérations de balayage dans un thread d'arrière-plan pour garder l'interface réactive
2. Mettez en cache les informations de chaîne pour éviter les balayages répétés
3. Implémentez un changement de chaîne efficace pour minimiser les délais
4. Tenez compte de l'utilisation des ressources, en particulier pour les applications embarquées ou mobiles

## Conclusion

En suivant ce guide, vous pouvez implémenter des capacités complètes de radio FM et de tuning TV dans vos applications .NET. Ces fonctionnalités peuvent enrichir les applications multimédias, les systèmes domotiques ou les logiciels de diffusion spécialisés. Le SDK fournit une API propre et cohérente qui gère la complexité des différents matériels tuner.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
