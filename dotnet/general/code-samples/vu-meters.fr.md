---
title: VU-mètres audio et visualiseurs de forme d'onde en C# .NET
description: Construisez des VU-mètres et des visualiseurs de forme d'onde en WinForms et WPF pour la surveillance des niveaux audio en temps réel, mono et stéréo en .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - WinForms
  - WPF
  - C#
primary_api_classes:
  - AudioLevelEventArgs
  - VUMeterMaxSampleEventArgs

---

# Visualisation audio : implémenter des VU-mètres et des affichages de forme d'onde en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

La visualisation audio est un composant crucial des applications multimédias modernes, fournissant aux utilisateurs un retour visuel sur les niveaux audio et les motifs de forme d'onde. Ce guide montre comment implémenter des VU-mètres (Volume Unit) et des visualiseurs de forme d'onde dans des applications WinForms et WPF.

## Comprendre les composants de visualisation audio

Avant de plonger dans l'implémentation, il est important de comprendre les deux principaux outils de visualisation avec lesquels nous allons travailler :

### VU-mètres

Les VU-mètres affichent le niveau audio instantané d'un signal, indiquant généralement la puissance de l'audio à tout moment donné. Ils fournissent un retour en temps réel sur les niveaux audio, aidant les utilisateurs à surveiller la force du signal et à prévenir la distorsion ou l'écrêtage.

### Peintres de forme d'onde

Les visualiseurs de forme d'onde affichent le signal audio sous la forme d'une ligne continue qui représente les changements d'amplitude au fil du temps. Ils fournissent une représentation plus détaillée du contenu audio, montrant des motifs et des caractéristiques qui pourraient ne pas être apparents à l'écoute seule.

## Implémentation dans les applications WinForms

WinForms fournit un moyen simple d'implémenter des composants de visualisation audio avec un code minimal. Explorons l'implémentation des VU-mètres et des peintres de forme d'onde.

### Implémentation d'un VU-mètre en WinForms

Implémenter un VU-mètre en WinForms ne nécessite que quelques étapes :

1. **Ajoutez le contrôle VU-mètre** : tout d'abord, ajoutez le contrôle VU-mètre à votre formulaire. Pour l'audio stéréo, vous ajouterez généralement deux contrôles — un pour chaque canal.

   ```cs
   // Ajouter ceci à la conception de votre formulaire
   VisioForge.Core.UI.WinForms.VolumeMeterPro.VolumeMeter volumeMeter1;
   VisioForge.Core.UI.WinForms.VolumeMeterPro.VolumeMeter volumeMeter2; // Pour la stéréo
   ```

2. **Activez le VU-mètre dans votre contrôle multimédia** : avant de démarrer la lecture ou la capture, activez la fonctionnalité VU-mètre dans votre contrôle multimédia.

   ```cs
   // Activer le VU-mètre avant de démarrer la lecture/capture
   mediaPlayer.Audio_VUMeter_Pro_Enabled = true;
   ```

3. **Implémentez le gestionnaire d'événements** : ajoutez un gestionnaire d'événements pour traiter les données de niveau audio et mettre à jour l'affichage du VU-mètre.

   ```cs
   private void VideoCapture1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
   {
       volumeMeter1.Amplitude = e.ChannelLevelsDb[0];
       if (e.ChannelLevelsDb.Length > 1)
       {
           volumeMeter2.Amplitude = e.ChannelLevelsDb[1];
       }
   }
   ```

Avec ces étapes, votre VU-mètre se met à jour dynamiquement en fonction des niveaux audio de votre lecture ou capture multimédia.

### Implémentation d'un peintre de forme d'onde en WinForms

L'implémentation du peintre de forme d'onde suit un modèle similaire :

1. **Ajoutez le contrôle Peintre de forme d'onde** : ajoutez le contrôle Peintre de forme d'onde à votre formulaire. Pour l'audio stéréo, ajoutez deux contrôles.

   ```cs
   // Ajouter ceci à la conception de votre formulaire
   VisioForge.Core.UI.WinForms.VolumeMeterPro.WaveformPainter waveformPainter1;
   VisioForge.Core.UI.WinForms.VolumeMeterPro.WaveformPainter waveformPainter2; // Pour la stéréo
   ```

2. **Activez le traitement du VU-mètre** : activez la fonctionnalité VU-mètre pour fournir des données au peintre de forme d'onde.

   ```cs
   // Activer le VU-mètre avant de démarrer la lecture/capture
   mediaPlayer.Audio_VUMeter_Pro_Enabled = true;
   ```

3. **Implémentez le gestionnaire d'événements** : ajoutez un gestionnaire d'événements pour traiter les données audio et mettre à jour l'affichage de la forme d'onde.

   ```cs
   private void VideoCapture1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
   {
       waveformPainter1.AddMax(e.ChannelLevelsDb[0]);
       if (e.ChannelLevelsDb.Length > 1)
       {
           waveformPainter2.AddMax(e.ChannelLevelsDb[1]);
       }
   }
   ```

## Implémentation dans les applications WPF

WPF nécessite une approche légèrement différente en raison de son modèle de threading et de son framework UI. Voyons comment implémenter les deux types de visualisation en WPF.

### Implémentation d'un VU-mètre en WPF

1. **Ajoutez le contrôle VU-mètre** : ajoutez le contrôle VU-mètre à votre disposition XAML. Pour l'audio stéréo, ajoutez deux contrôles.

   ```xml
   <VisioForge.Core.UI.WPF.VolumeMeterPro.VolumeMeter x:Name="volumeMeter1" />
   <VisioForge.Core.UI.WPF.VolumeMeterPro.VolumeMeter x:Name="volumeMeter2" /> <!-- Pour la stéréo -->
   ```

2. **Activez le traitement du VU-mètre et démarrez les mètres** :

   ```cs
   VideoCapture1.Audio_VUMeter_Pro_Enabled = true;

   volumeMeter1.Start();
   volumeMeter2.Start();
   ```

3. **Implémentez le gestionnaire d'événements avec Dispatcher** : en WPF, vous devez utiliser le Dispatcher pour mettre à jour les éléments UI depuis des threads non-UI.

   ```cs
   private delegate void AudioVUMeterProVolumeDelegate(AudioLevelEventArgs e);

   private void AudioVUMeterProVolumeDelegateMethod(AudioLevelEventArgs e)
   {
       volumeMeter1.Amplitude = e.ChannelLevelsDb[0];
       volumeMeter1.Update();

       if (e.ChannelLevelsDb.Length > 1)
       {
           volumeMeter2.Amplitude = e.ChannelLevelsDb[1];
           volumeMeter2.Update();
       }
   }

   private void VideoCapture1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
   {
       Dispatcher.BeginInvoke(new AudioVUMeterProVolumeDelegate(AudioVUMeterProVolumeDelegateMethod), e);
   }
   ```

4. **Nettoyez après la lecture** : lorsque la lecture s'arrête, nettoyez les VU-mètres pour libérer les ressources.

   ```cs
   volumeMeter1.Stop();
   volumeMeter1.Clear();

   volumeMeter2.Stop();
   volumeMeter2.Clear();
   ```

### Implémentation d'un peintre de forme d'onde en WPF

1. **Ajoutez le contrôle Peintre de forme d'onde** : ajoutez le contrôle Peintre de forme d'onde à votre disposition XAML.

   ```xml
   <VisioForge.Core.UI.WPF.VolumeMeterPro.WaveformPainter x:Name="waveformPainter" />
   ```

2. **Activez le traitement du VU-mètre et démarrez le peintre de forme d'onde** :

   ```cs
   VideoCapture1.Audio_VUMeter_Pro_Enabled = true;
   waveformPainter.Start();
   ```

3. **Implémentez le gestionnaire d'événements de maximum calculé** : pour les peintres de forme d'onde en WPF, nous utilisons un événement différent.

   ```cs
   private delegate void AudioVUMeterProMaximumCalculatedDelegate(VUMeterMaxSampleEventArgs e);

   private void AudioVUMeterProMaximumCalculatedelegateMethod(VUMeterMaxSampleEventArgs e)
   {
       waveformPainter.AddValue(e.MaxSample, e.MinSample);
   }

   private void VideoCapture1_OnAudioVUMeterProMaximumCalculated(object sender, VUMeterMaxSampleEventArgs e)
   {
       Dispatcher.BeginInvoke(new AudioVUMeterProMaximumCalculatedDelegate(AudioVUMeterProMaximumCalculatedelegateMethod), e);
   }
   ```

4. **Nettoyez après la lecture** : lorsque la lecture s'arrête, nettoyez le peintre de forme d'onde.

   ```cs
   waveformPainter.Stop();
   waveformPainter.Clear();
   ```

## Options de personnalisation avancées

Les contrôles VU-mètre et Peintre de forme d'onde offrent tous deux de nombreuses options de personnalisation pour correspondre au design et aux exigences d'expérience utilisateur de votre application.

### Personnalisation des VU-mètres

Le contrôle `VolumeMeter` expose les propriétés réelles suivantes :

- **`MinDb` / `MaxDb`** : plage de décibels affichée par le mètre
- **`Boost`** : multiplicateur de gain appliqué avant le rendu
- **`Orientation`** : direction horizontale ou verticale de la barre
- **`ForeColor`** : couleur de la barre (héritée de `Control`)
- **`MinimalUpdateInterval`** (WPF uniquement) : limite les redessins

Exemple de personnalisation d'un VU-mètre :

```cs
volumeMeter1.MinDb = -60;
volumeMeter1.MaxDb = 6;
volumeMeter1.Boost = 1.0f;
volumeMeter1.ForeColor = System.Drawing.Color.Green;  // WinForms
volumeMeter1.Orientation = System.Windows.Forms.Orientation.Vertical;
```

### Personnalisation des peintres de forme d'onde

Le contrôle `WaveformPainter` a une petite surface réelle :

- **`Boost`** (WinForms) : multiplicateur de gain avant rendu
- **`ForeColor` / `BackColor`** : couleurs de ligne et d'arrière-plan (héritées de `Control`)
- **`Clear()`** : réinitialise l'historique peint
- **`AddMax(float)`** (WinForms) / **`AddValue(float, float)`** (WPF) : ajoute un nouvel échantillon

Exemple de personnalisation d'un peintre de forme d'onde :

```cs
waveformPainter.ForeColor = System.Drawing.Color.SkyBlue;
waveformPainter.BackColor = System.Drawing.Color.Black;
waveformPainter.Boost = 1.5f;
```

## Considérations de performance

Lors de l'implémentation de la visualisation audio, prenez en compte ces conseils de performance :

1. **Fréquence de mise à jour** : équilibrez la réactivité visuelle avec l'utilisation du CPU en ajustant la fréquence de mise à jour des visuels
2. **Gestion du thread UI** : mettez toujours à jour les éléments UI sur le thread approprié (particulièrement important en WPF)
3. **Nettoyage des ressources** : arrêtez et nettoyez correctement les contrôles de visualisation lorsqu'ils ne sont pas utilisés
4. **Mise en tampon** : envisagez d'implémenter une mise en tampon pour une visualisation plus fluide en cas d'utilisation élevée du CPU

## Conclusion

Implémenter des VU-mètres et des peintres de forme d'onde ajoute un retour visuel précieux aux applications multimédias. Que vous développiez en WinForms ou WPF, ces composants de visualisation audio aident les utilisateurs à surveiller et comprendre les niveaux et motifs audio de manière plus intuitive.

En suivant les étapes d'implémentation décrites dans ce guide, vous pouvez enrichir vos applications multimédias .NET avec des fonctionnalités de visualisation audio de qualité professionnelle qui améliorent l'expérience utilisateur globale.

---
Pour plus d'exemples de code et de SDK associés, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
