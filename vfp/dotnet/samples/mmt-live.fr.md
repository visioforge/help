---
title: MMT Live — outil de surveillance média en temps réel (.NET)
description: Utilisez MMT Live pour surveiller des flux en direct et détecter des fragments vidéo en temps réel, avec gestion de bibliothèque multimédia.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Capture
  - Fingerprinting
  - RTSP
  - HLS
  - C#

---

# MMT Live — outil de surveillance média en temps réel

📦 **Code source** : [Voir sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20Live)

## Vue d'ensemble

MMT Live est une version en temps réel de Media Monitoring Tool capable de détecter des fragments vidéo dans des flux en direct ou pendant la lecture en direct. Il est conçu pour surveiller les diffusions en temps réel, détecter les publicités au moment où elles sont diffusées et déclencher des actions immédiates lorsque du contenu spécifique est détecté.

## Fonctionnalités clés

- **Détection en temps réel** : identifier les fragments pendant la lecture vidéo
- **Prise en charge des flux en direct** : surveiller les flux RTSP, HTTP et fichiers
- **Notifications instantanées** : alertes immédiates lors de détection de contenu
- **Surveillance continue** : capacité de fonctionnement 24/7
- **Détection de publicités** : identification de publicités en temps réel
- **Faible latence** : détection quasi instantanée
- **Aperçu en direct** : surveiller le flux pendant le traitement

## Différences avec MMT standard

| Fonctionnalité | MMT | MMT Live |
|---------|-----|----------|
| Traitement | Post-enregistrement | Temps réel |
| Entrée | Fichiers uniquement | Fichiers + flux |
| Détection | Par lots | Continue |
| Résultats | Après achèvement | Immédiats |
| Cas d'usage | Analyse | Surveillance |

## Interface utilisateur

### Composants principaux

1. **Lecteur média en direct** : affiche le flux/la lecture en cours
2. **Bibliothèque de fragments** : cibles de détection préchargées
3. **Journal des détections** : événements de détection en temps réel
4. **Indicateurs d'état** : santé du flux et état du traitement
5. **Panneau de paramètres** : ajustement en direct des paramètres

## Comment l'utiliser

### Flux de configuration

1. **Préparer la bibliothèque de fragments** :
   - Charger les publicités/clips à détecter
   - Générer les empreintes à l'avance
   - Organiser par priorité/catégorie

2. **Configurer la source d'entrée** :
   - Fichier : sélectionner une vidéo à surveiller
   - Flux : entrer une URL RTSP/HTTP
   - Périphérique : sélectionner un périphérique de capture

3. **Définir les paramètres de détection** :
   - Seuil de sensibilité
   - Durée minimale de correspondance
   - Préférences d'alerte

4. **Démarrer la surveillance** :
   - Cliquer sur « Start » pour commencer
   - La vidéo se lit pendant l'analyse
   - Les détections apparaissent immédiatement

### Fonctionnement en temps réel

- **Traitement continu** : analyse la vidéo pendant qu'elle se lit
- **Tampon glissant** : maintient un historique récent de la vidéo
- **Correspondance instantanée** : compare avec la bibliothèque de fragments
- **Journalisation d'événements** : enregistre toutes les détections avec horodatages

## Cas d'usage

### 1. Conformité de diffusion

- Garantir que les publicités passent comme prévu
- Vérifier les restrictions de contenu
- Surveiller la publicité concurrente
- Suivre les segments de programme

### 2. Surveillance de flux en direct

- Détecter le contenu protégé
- Surveiller plusieurs chaînes
- Suivre les apparitions de marque
- Assurance qualité

### 3. Actions automatisées

- Déclencher l'enregistrement lors de la détection
- Envoyer des notifications/alertes
- Basculer automatiquement les flux
- Générer des rapports en temps réel

### 4. Suivi publicitaire

- Compter les diffusions de publicités
- Vérifier le placement des publicités
- Surveiller la fréquence des publicités
- Analyse concurrentielle

## Configuration

### Sources d'entrée

**Lecture de fichier** :

- Simule la surveillance en direct
- Utile pour les tests
- Prend en charge tous les formats vidéo

**Flux RTSP** :

```txt
rtsp://camera.example.com:554/stream
rtsp://username:password@server/path
```

**Flux HTTP** :

```txt
http://server.com/stream.m3u8
http://server.com/live.mjpeg
```

### Paramètres de détection

- **Taille du tampon** : historique vidéo (5 à 60 secondes)
- **Intervalle de vérification** : fréquence d'analyse (1 à 5 secondes)
- **Seuil de confiance** : qualité de correspondance (70 à 95 %)
- **Priorité des fragments** : quels fragments vérifier en premier

## Optimisation des performances

### Configuration système requise

- **CPU** : multi-cœur recommandé
- **RAM** : 8 à 16 Go pour un fonctionnement fluide
- **Réseau** : connexion stable pour les flux
- **Stockage** : SSD rapide pour la bibliothèque de fragments

### Conseils d'optimisation

1. **Bibliothèque de fragments** :
   - Garder moins de 100 fragments actifs
   - Pré-générer toutes les empreintes
   - Supprimer les fragments inutilisés

2. **Qualité du flux** :
   - Utiliser un débit binaire cohérent
   - Éviter les résolutions très élevées
   - Assurer une connexion stable

3. **Traitement** :
   - Ajuster l'intervalle de vérification selon le CPU
   - Utiliser une taille de tampon appropriée
   - Activer l'accélération GPU si disponible

## Fonctionnalités avancées

### Surveillance multi-flux

- Surveiller plusieurs flux simultanément
- Threads de détection séparés par flux
- Rapports consolidés
- Gestion des ressources

### Actions personnalisées

Configurer des actions pour les détections :

- Notifications par e-mail
- Webhooks HTTP
- Journalisation fichier
- Enregistrement en base de données
- Déclencheurs d'enregistrement de flux

### Zones de détection

- Définir des fenêtres temporelles pour la détection
- Planifier différents ensembles de fragments
- Ignorer certaines périodes
- Planification par priorité

## Dépannage

### Aucune détection

- Vérifier que les fragments sont chargés
- Vérifier que le flux est en lecture
- Confirmer que les empreintes sont générées
- Ajuster la sensibilité à la baisse

### Utilisation CPU élevée

- Réduire la fréquence de vérification
- Abaisser la résolution du flux
- Diminuer la taille du tampon
- Limiter les fragments actifs

### Problèmes de flux

- Vérifier la connectivité réseau
- Vérifier l'URL du flux
- Tester d'abord dans un lecteur multimédia
- Surveiller l'utilisation de la bande passante

### Détections retardées

- Augmenter la priorité de traitement
- Réduire la taille du tampon
- Vérifier les ressources système
- Optimiser le nombre de fragments

## Bonnes pratiques

### Préparation

1. Tester d'abord les fragments dans MMT standard
2. Optimiser la qualité et la longueur des fragments
3. Construire une bibliothèque complète de fragments
4. Documenter les détections attendues

### Fonctionnement

1. Surveiller les ressources système
2. Mises à jour régulières de la bibliothèque de fragments
3. Vérifications périodiques de la précision de détection
4. Maintenir des journaux de détection

### Maintenance

1. Nettoyer les anciens journaux de détection
2. Mettre à jour les empreintes de fragments
3. Examiner les faux positifs/négatifs
4. Optimiser selon les résultats

## Options d'intégration

### Intégration API

- API REST pour les événements de détection
- WebSocket pour les mises à jour en temps réel
- Options de journalisation en base
- Intégrations tierces

### Automatisation

- Surveillance planifiée
- Génération automatique de rapports
- Escalade d'alertes
- Basculement de flux

## Comparaison avec les alternatives

**vs MMT standard** :

- Temps réel vs post-traitement
- Continu vs par lots
- Résultats immédiats vs différés

**vs surveillance manuelle** :

- Automatisé vs observation humaine
- 24/7 vs heures limitées
- Précision constante vs variable

## Outils associés

- `MMT` : version d'analyse post-enregistrement
- `vfp_search` : recherche de fragments en ligne de commande
- `DVS` : détection de vidéos en doublon
