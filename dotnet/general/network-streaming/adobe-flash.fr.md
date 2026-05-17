---
title: Streaming vidéo vers Adobe Flash Media Server en SDK .NET
description: Diffusez vers Adobe Flash Media Server en .NET avec effets temps réel, paramètres de qualité et changement de périphérique en streaming.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - Windows
  - Streaming
  - Encoding

---

# Diffusion vers Adobe Flash Media Server : guide d'implémentation avancée

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } 

## Introduction

Adobe Flash Media Server (FMS) reste une solution puissante pour diffuser du contenu vidéo sur diverses plateformes. Ce guide montre comment implémenter une diffusion vidéo de haute qualité vers Adobe Flash Media Server à l'aide des SDK .NET de VisioForge. L'intégration prend en charge les effets vidéo temps réel, l'ajustement de la qualité et le changement transparent de périphérique pendant les sessions de diffusion.

## Prérequis

Avant d'implémenter la fonctionnalité de diffusion, assurez-vous d'avoir :

- VisioForge Video Capture SDK .NET ou Video Edit SDK .NET installé
- Adobe Flash Media Server (ou un service compatible comme Wowza avec prise en charge RTMP)
- Adobe Flash Media Live Encoder (FMLE)
- .NET Framework 4.7.2 ou ultérieur
- Visual Studio 2022 ou plus récent
- Une compréhension de base de la programmation C#

## Présentation de l'application de démonstration

L'application de démonstration fournie avec les SDK VisioForge offre un moyen simple de tester la fonctionnalité de diffusion. Voici un guide détaillé :

1. Démarrez l'application Main Demo
2. Naviguez vers l'onglet « Network Streaming »
3. Activez la diffusion en sélectionnant la case « Enabled »
4. Sélectionnez la case d'option « External » pour la compatibilité avec un encodeur externe
5. Démarrez l'aperçu ou la capture pour initialiser le flux vidéo
6. Ouvrez Adobe Flash Media Live Encoder
7. Configurez FMLE pour utiliser « VisioForge Network Source » comme source vidéo
8. Configurez les paramètres vidéo :
   - Résolution (par ex. 1280x720, 1920x1080)
   - Fréquence d'images (généralement 25-30 fps pour un streaming fluide)
   - Intervalle de keyframe (2 secondes recommandées)
   - Paramètres de qualité vidéo
9. Sélectionnez « VisioForge Network Source Audio » comme source audio
10. Configurez votre connexion à Adobe Flash Media Server
11. Appuyez sur Start pour lancer la diffusion

La vidéo du SDK est désormais diffusée vers votre instance FMS. Vous pouvez appliquer des effets en temps réel, ajuster les paramètres, ou même arrêter le SDK pour changer de périphérique d'entrée sans interrompre la session de diffusion côté serveur.

## Implémentation dans des applications personnalisées

### Composants requis

Pour implémenter cette fonctionnalité dans votre application personnalisée, vous aurez besoin de :

- Les redistribuables du SDK (disponibles dans le paquet d'installation du SDK)
- Des références aux assemblies du SDK VisioForge
- Des configurations de pare-feu et de réseau appropriées pour autoriser la diffusion

## Redistribuables requis

Assurez-vous que les composants suivants sont inclus avec votre application :

- Paquets redistribuables du SDK VisioForge
- Microsoft Visual C++ Runtime (version appropriée pour votre SDK)
- Runtime .NET Framework (si vous n'utilisez pas un déploiement autonome)

## Conclusion

La diffusion vers Adobe Flash Media Server à l'aide des Video Capture SDK ou Video Edit de VisioForge offre une solution flexible et puissante pour implémenter une diffusion vidéo de haute qualité dans des applications .NET. L'implémentation prend en charge les effets temps réel, les ajustements de qualité et le changement transparent de périphérique, ce qui la rend adaptée à un large éventail d'applications de diffusion.

En suivant ce guide, les développeurs peuvent implémenter des solutions de diffusion robustes qui tirent parti des puissantes fonctionnalités des SDK VisioForge et de la plateforme de diffusion d'Adobe.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code et de projets d'exemple.
