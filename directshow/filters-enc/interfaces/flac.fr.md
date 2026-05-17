---
title: Interface DirectShow de l'encodeur audio sans perte FLAC
description: Interface DirectShow de l'encodeur FLAC — niveaux d'encodage, configuration LPC, tailles de bloc et compression pour l'encodage audio sans perte.
tags:
  - DirectShow
  - C++
  - Windows
  - Encoding
  - Decoding
  - MP3
  - FLAC
  - C#
primary_api_classes:
  - FLACEncoder
  - IFLACEncodeSettings
  - IBaseFilter
  - AudioEncoder
  - FLACArchivalEncoder

---

# Référence de l'interface de l'encodeur FLAC

## Vue d'ensemble

L'interface **IFLACEncodeSettings** fournit un contrôle complet sur les paramètres d'encodage audio FLAC (Free Lossless Audio Codec) dans les graphes de filtres DirectShow. FLAC est un format de compression audio sans perte qui réduit la taille du fichier sans aucune perte de qualité audio, ce qui en fait un choix idéal pour l'archivage, la production audio professionnelle et la distribution musicale haute fidélité.

Cette interface permet aux développeurs de configurer les niveaux de qualité d'encodage, les paramètres LPC (Linear Predictive Coding), les tailles de bloc, le codage stéréo mid-side et les ordres de partition de Rice afin d'atteindre une efficacité de compression optimale selon le type de contenu audio.

**GUID de l'interface** : `{A6096781-2A65-4540-A536-011235D0A5FE}`

**Hérite de** : `IUnknown`

## Définitions de l'interface

### Définition C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interface de configuration de l'encodeur FLAC (Free Lossless Audio Codec).
    /// Fournit un controle complet des parametres d'encodage FLAC pour la compression audio sans perte.
    /// </summary>
    [ComImport]
    [Guid("A6096781-2A65-4540-A536-011235D0A5FE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFLACEncodeSettings
    {
        /// <summary>
        /// Verifie si les parametres d'encodage peuvent etre modifies a cet instant.
        /// </summary>
        /// <returns>True si les parametres peuvent etre modifies, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool canModifySettings();

        /// <summary>
        /// Definit le niveau d'encodage FLAC (qualite de compression).
        /// </summary>
        /// <param name="inLevel">Niveau d'encodage (0-8, ou 8 est la compression la plus forte et la plus lente)</param>
        /// <returns>True si reussi, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setEncodingLevel(uint inLevel);

        /// <summary>
        /// Definit l'ordre du codage predictif lineaire (LPC).
        /// Des valeurs plus elevees offrent une meilleure compression mais un encodage plus lent.
        /// </summary>
        /// <param name="inLPCOrder">Ordre LPC (generalement 0-32)</param>
        /// <returns>True si reussi, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setLPCOrder(uint inLPCOrder);

        /// <summary>
        /// Definit la taille de bloc audio pour l'encodage.
        /// Des blocs plus grands peuvent offrir une meilleure compression mais augmentent la latence.
        /// </summary>
        /// <param name="inBlockSize">Taille de bloc en echantillons (generalement 192-4608)</param>
        /// <returns>True si reussi, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setBlockSize(uint inBlockSize);

        /// <summary>
        /// Active ou desactive le codage stereo mid-side pour l'audio 2 canaux.
        /// Peut ameliorer la compression pour l'audio stereo dont les canaux sont correles.
        /// </summary>
        /// <param name="inUseMidSideCoding">True pour activer le codage mid-side, false pour desactiver</param>
        /// <returns>True si reussi, false sinon</returns>
        /// <remarks>Applicable uniquement a l'audio 2 canaux (stereo)</remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useMidSideCoding([In, MarshalAs(UnmanagedType.Bool)] bool inUseMidSideCoding);

        /// <summary>
        /// Active ou desactive le codage stereo mid-side adaptatif.
        /// Decide automatiquement par bloc s'il faut utiliser le codage mid-side.
        /// Remplace useMidSideCoding et est generalement plus rapide.
        /// </summary>
        /// <param name="inUseAdaptiveMidSideCoding">True pour activer le mid-side adaptatif, false pour desactiver</param>
        /// <returns>True si reussi, false sinon</returns>
        /// <remarks>Audio 2 canaux uniquement. Remplace useMidSideCoding. Offre generalement de meilleures performances.</remarks>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useAdaptiveMidSideCoding([In, MarshalAs(UnmanagedType.Bool)] bool inUseAdaptiveMidSideCoding);

        /// <summary>
        /// Active ou desactive la recherche exhaustive de modele pour la meilleure compression.
        /// Significativement plus lent mais peut offrir de meilleurs ratios de compression.
        /// </summary>
        /// <param name="inUseExhaustiveModelSearch">True pour activer la recherche exhaustive, false pour desactiver</param>
        /// <returns>True si reussi, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool useExhaustiveModelSearch([In, MarshalAs(UnmanagedType.Bool)] bool inUseExhaustiveModelSearch);

        /// <summary>
        /// Definit la plage de l'ordre de partition de Rice pour le codage entropique.
        /// Controle le compromis entre efficacite de compression et vitesse d'encodage.
        /// </summary>
        /// <param name="inMin">Ordre minimal de partition de Rice</param>
        /// <param name="inMax">Ordre maximal de partition de Rice</param>
        /// <returns>True si reussi, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool setRicePartitionOrder(uint inMin, uint inMax);

        /// <summary>
        /// Obtient le niveau d'encodage actuel.
        /// </summary>
        /// <returns>Niveau d'encodage actuel (0-8)</returns>
        [PreserveSig]
        int encoderLevel();

        /// <summary>
        /// Obtient l'ordre LPC (Linear Predictive Coding) actuel.
        /// </summary>
        /// <returns>Ordre LPC actuel</returns>
        [PreserveSig]
        uint LPCOrder();

        /// <summary>
        /// Obtient la taille de bloc actuelle.
        /// </summary>
        /// <returns>Taille de bloc actuelle en echantillons</returns>
        [PreserveSig]
        uint blockSize();

        /// <summary>
        /// Obtient l'ordre minimal de partition de Rice.
        /// </summary>
        /// <returns>Ordre minimal de partition de Rice</returns>
        [PreserveSig]
        uint riceMin();

        /// <summary>
        /// Obtient l'ordre maximal de partition de Rice.
        /// </summary>
        /// <returns>Ordre maximal de partition de Rice</returns>
        [PreserveSig]
        uint riceMax();

        /// <summary>
        /// Verifie si le codage stereo mid-side est active.
        /// </summary>
        /// <returns>True si le codage mid-side est active, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingMidSideCoding();

        /// <summary>
        /// Verifie si le codage stereo mid-side adaptatif est active.
        /// </summary>
        /// <returns>True si le mid-side adaptatif est active, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingAdaptiveMidSideCoding();

        /// <summary>
        /// Verifie si la recherche exhaustive de modele est activee.
        /// </summary>
        /// <returns>True si la recherche exhaustive est activee, false sinon</returns>
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool isUsingExhaustiveModel();
    }
}
```

### Définition C++

```cpp
#include <unknwn.h>

// {A6096781-2A65-4540-A536-011235D0A5FE}
DEFINE_GUID(IID_IFLACEncodeSettings,
    0xa6096781, 0x2a65, 0x4540, 0xa5, 0x36, 0x01, 0x12, 0x35, 0xd0, 0xa5, 0xfe);

/// <summary>
/// Interface de configuration de l'encodeur FLAC (Free Lossless Audio Codec).
/// Fournit un controle complet des parametres d'encodage FLAC pour la compression audio sans perte.
/// </summary>
DECLARE_INTERFACE_(IFLACEncodeSettings, IUnknown)
{
    /// <summary>
    /// Verifie si les parametres d'encodage peuvent etre modifies a cet instant.
    /// </summary>
    /// <returns>TRUE si les parametres peuvent etre modifies, FALSE sinon</returns>
    STDMETHOD_(BOOL, canModifySettings)(THIS) PURE;

    /// <summary>
    /// Definit le niveau d'encodage FLAC (qualite de compression).
    /// </summary>
    /// <param name="inLevel">Niveau d'encodage (0-8, ou 8 est la compression la plus forte et la plus lente)</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    STDMETHOD_(BOOL, setEncodingLevel)(THIS_
        unsigned long inLevel
        ) PURE;

    /// <summary>
    /// Definit l'ordre du codage predictif lineaire (LPC).
    /// Des valeurs plus elevees offrent une meilleure compression mais un encodage plus lent.
    /// </summary>
    /// <param name="inLPCOrder">Ordre LPC (generalement 0-32)</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    STDMETHOD_(BOOL, setLPCOrder)(THIS_
        unsigned long inLPCOrder
        ) PURE;

    /// <summary>
    /// Definit la taille de bloc audio pour l'encodage.
    /// Des blocs plus grands peuvent offrir une meilleure compression mais augmentent la latence.
    /// </summary>
    /// <param name="inBlockSize">Taille de bloc en echantillons (generalement 192-4608)</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    STDMETHOD_(BOOL, setBlockSize)(THIS_
        unsigned long inBlockSize
        ) PURE;

    /// <summary>
    /// Active ou desactive le codage stereo mid-side pour l'audio 2 canaux.
    /// Peut ameliorer la compression pour l'audio stereo dont les canaux sont correles.
    /// </summary>
    /// <param name="inUseMidSideCoding">TRUE pour activer le codage mid-side, FALSE pour desactiver</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    /// <remarks>Applicable uniquement a l'audio 2 canaux (stereo)</remarks>
    STDMETHOD_(BOOL, useMidSideCoding)(THIS_
        BOOL inUseMidSideCoding
        ) PURE;

    /// <summary>
    /// Active ou desactive le codage stereo mid-side adaptatif.
    /// Decide automatiquement par bloc s'il faut utiliser le codage mid-side.
    /// Remplace useMidSideCoding et est generalement plus rapide.
    /// </summary>
    /// <param name="inUseAdaptiveMidSideCoding">TRUE pour activer le mid-side adaptatif, FALSE pour desactiver</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    /// <remarks>Audio 2 canaux uniquement. Remplace useMidSideCoding. Offre generalement de meilleures performances.</remarks>
    STDMETHOD_(BOOL, useAdaptiveMidSideCoding)(THIS_
        BOOL inUseAdaptiveMidSideCoding
        ) PURE;

    /// <summary>
    /// Active ou desactive la recherche exhaustive de modele pour la meilleure compression.
    /// Significativement plus lent mais peut offrir de meilleurs ratios de compression.
    /// </summary>
    /// <param name="inUseExhaustiveModelSearch">TRUE pour activer la recherche exhaustive, FALSE pour desactiver</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    STDMETHOD_(BOOL, useExhaustiveModelSearch)(THIS_
        BOOL inUseExhaustiveModelSearch
        ) PURE;

    /// <summary>
    /// Definit la plage de l'ordre de partition de Rice pour le codage entropique.
    /// Controle le compromis entre efficacite de compression et vitesse d'encodage.
    /// </summary>
    /// <param name="inMin">Ordre minimal de partition de Rice</param>
    /// <param name="inMax">Ordre maximal de partition de Rice</param>
    /// <returns>TRUE si reussi, FALSE sinon</returns>
    STDMETHOD_(BOOL, setRicePartitionOrder)(THIS_
        unsigned long inMin,
        unsigned long inMax
        ) PURE;

    /// <summary>
    /// Obtient le niveau d'encodage actuel.
    /// </summary>
    /// <returns>Niveau d'encodage actuel (0-8)</returns>
    STDMETHOD_(int, encoderLevel)(THIS) PURE;

    /// <summary>
    /// Obtient l'ordre LPC (Linear Predictive Coding) actuel.
    /// </summary>
    /// <returns>Ordre LPC actuel</returns>
    STDMETHOD_(unsigned long, LPCOrder)(THIS) PURE;

    /// <summary>
    /// Obtient la taille de bloc actuelle.
    /// </summary>
    /// <returns>Taille de bloc actuelle en echantillons</returns>
    STDMETHOD_(unsigned long, blockSize)(THIS) PURE;

    /// <summary>
    /// Obtient l'ordre minimal de partition de Rice.
    /// </summary>
    /// <returns>Ordre minimal de partition de Rice</returns>
    STDMETHOD_(unsigned long, riceMin)(THIS) PURE;

    /// <summary>
    /// Obtient l'ordre maximal de partition de Rice.
    /// </summary>
    /// <returns>Ordre maximal de partition de Rice</returns>
    STDMETHOD_(unsigned long, riceMax)(THIS) PURE;

    /// <summary>
    /// Verifie si le codage stereo mid-side est active.
    /// </summary>
    /// <returns>TRUE si le codage mid-side est active, FALSE sinon</returns>
    STDMETHOD_(BOOL, isUsingMidSideCoding)(THIS) PURE;

    /// <summary>
    /// Verifie si le codage stereo mid-side adaptatif est active.
    /// </summary>
    /// <returns>TRUE si le mid-side adaptatif est active, FALSE sinon</returns>
    STDMETHOD_(BOOL, isUsingAdaptiveMidSideCoding)(THIS) PURE;

    /// <summary>
    /// Verifie si la recherche exhaustive de modele est activee.
    /// </summary>
    /// <returns>TRUE si la recherche exhaustive est activee, FALSE sinon</returns>
    STDMETHOD_(BOOL, isUsingExhaustiveModel)(THIS) PURE;
};
```

### Définition Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IFLACEncodeSettings: TGUID = '{A6096781-2A65-4540-A536-011235D0A5FE}';

type
  /// <summary>
  /// Interface de configuration de l'encodeur FLAC (Free Lossless Audio Codec).
  /// Fournit un controle complet des parametres d'encodage FLAC pour la compression audio sans perte.
  /// </summary>
  IFLACEncodeSettings = interface(IUnknown)
    ['{A6096781-2A65-4540-A536-011235D0A5FE}']

    /// <summary>
    /// Verifie si les parametres d'encodage peuvent etre modifies a cet instant.
    /// </summary>
    /// <returns>True si les parametres peuvent etre modifies, false sinon</returns>
    function canModifySettings: BOOL; stdcall;

    /// <summary>
    /// Definit le niveau d'encodage FLAC (qualite de compression).
    /// </summary>
    /// <param name="inLevel">Niveau d'encodage (0-8, ou 8 est la compression la plus forte et la plus lente)</param>
    /// <returns>True si reussi, false sinon</returns>
    function setEncodingLevel(inLevel: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Definit l'ordre du codage predictif lineaire (LPC).
    /// Des valeurs plus elevees offrent une meilleure compression mais un encodage plus lent.
    /// </summary>
    /// <param name="inLPCOrder">Ordre LPC (generalement 0-32)</param>
    /// <returns>True si reussi, false sinon</returns>
    function setLPCOrder(inLPCOrder: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Definit la taille de bloc audio pour l'encodage.
    /// Des blocs plus grands peuvent offrir une meilleure compression mais augmentent la latence.
    /// </summary>
    /// <param name="inBlockSize">Taille de bloc en echantillons (generalement 192-4608)</param>
    /// <returns>True si reussi, false sinon</returns>
    function setBlockSize(inBlockSize: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Active ou desactive le codage stereo mid-side pour l'audio 2 canaux.
    /// Peut ameliorer la compression pour l'audio stereo dont les canaux sont correles.
    /// </summary>
    /// <param name="inUseMidSideCoding">True pour activer le codage mid-side, false pour desactiver</param>
    /// <returns>True si reussi, false sinon</returns>
    /// <remarks>Applicable uniquement a l'audio 2 canaux (stereo)</remarks>
    function useMidSideCoding(inUseMidSideCoding: BOOL): BOOL; stdcall;

    /// <summary>
    /// Active ou desactive le codage stereo mid-side adaptatif.
    /// Decide automatiquement par bloc s'il faut utiliser le codage mid-side.
    /// Remplace useMidSideCoding et est generalement plus rapide.
    /// </summary>
    /// <param name="inUseAdaptiveMidSideCoding">True pour activer le mid-side adaptatif, false pour desactiver</param>
    /// <returns>True si reussi, false sinon</returns>
    /// <remarks>Audio 2 canaux uniquement. Remplace useMidSideCoding. Offre generalement de meilleures performances.</remarks>
    function useAdaptiveMidSideCoding(inUseAdaptiveMidSideCoding: BOOL): BOOL; stdcall;

    /// <summary>
    /// Active ou desactive la recherche exhaustive de modele pour la meilleure compression.
    /// Significativement plus lent mais peut offrir de meilleurs ratios de compression.
    /// </summary>
    /// <param name="inUseExhaustiveModelSearch">True pour activer la recherche exhaustive, false pour desactiver</param>
    /// <returns>True si reussi, false sinon</returns>
    function useExhaustiveModelSearch(inUseExhaustiveModelSearch: BOOL): BOOL; stdcall;

    /// <summary>
    /// Definit la plage de l'ordre de partition de Rice pour le codage entropique.
    /// Controle le compromis entre efficacite de compression et vitesse d'encodage.
    /// </summary>
    /// <param name="inMin">Ordre minimal de partition de Rice</param>
    /// <param name="inMax">Ordre maximal de partition de Rice</param>
    /// <returns>True si reussi, false sinon</returns>
    function setRicePartitionOrder(inMin: Cardinal; inMax: Cardinal): BOOL; stdcall;

    /// <summary>
    /// Obtient le niveau d'encodage actuel.
    /// </summary>
    /// <returns>Niveau d'encodage actuel (0-8)</returns>
    function encoderLevel: Integer; stdcall;

    /// <summary>
    /// Obtient l'ordre LPC (Linear Predictive Coding) actuel.
    /// </summary>
    /// <returns>Ordre LPC actuel</returns>
    function LPCOrder: Cardinal; stdcall;

    /// <summary>
    /// Obtient la taille de bloc actuelle.
    /// </summary>
    /// <returns>Taille de bloc actuelle en echantillons</returns>
    function blockSize: Cardinal; stdcall;

    /// <summary>
    /// Obtient l'ordre minimal de partition de Rice.
    /// </summary>
    /// <returns>Ordre minimal de partition de Rice</returns>
    function riceMin: Cardinal; stdcall;

    /// <summary>
    /// Obtient l'ordre maximal de partition de Rice.
    /// </summary>
    /// <returns>Ordre maximal de partition de Rice</returns>
    function riceMax: Cardinal; stdcall;

    /// <summary>
    /// Verifie si le codage stereo mid-side est active.
    /// </summary>
    /// <returns>True si le codage mid-side est active, false sinon</returns>
    function isUsingMidSideCoding: BOOL; stdcall;

    /// <summary>
    /// Verifie si le codage stereo mid-side adaptatif est active.
    /// </summary>
    /// <returns>True si le mid-side adaptatif est active, false sinon</returns>
    function isUsingAdaptiveMidSideCoding: BOOL; stdcall;

    /// <summary>
    /// Verifie si la recherche exhaustive de modele est activee.
    /// </summary>
    /// <returns>True si la recherche exhaustive est activee, false sinon</returns>
    function isUsingExhaustiveModel: BOOL; stdcall;
  end;
```

## Référence des méthodes

### Vérification de configuration

#### canModifySettings

Vérifie si les paramètres d'encodage peuvent être modifiés à cet instant. Utile pour vérifier que l'encodeur est dans un état où les modifications de configuration sont autorisées (généralement avant que le graphe de filtres ne démarre).

**Retourne** : `true` si les paramètres peuvent être modifiés, `false` sinon

**Exemple d'utilisation** :
```csharp
if (flacEncoder.canModifySettings())
{
    // Modification des parametres possible
    flacEncoder.setEncodingLevel(5);
}
```

### Méthodes de configuration d'encodage

#### setEncodingLevel

Définit le niveau d'encodage FLAC, qui contrôle le compromis qualité de compression / vitesse d'encodage.

**Paramètres** :
- `inLevel` — niveau d'encodage (0-8) :
  - 0 = encodage le plus rapide, compression la plus faible
  - 5 = équilibré (recommandé pour la plupart des usages)
  - 8 = encodage le plus lent, compression la plus forte

**Retourne** : `true` si réussi, `false` sinon

**Valeurs recommandées** :
- **Archivage rapide** : niveau 3-5
- **Archivage haute qualité** : niveau 6-8
- **Encodage en temps réel** : niveau 0-2

#### setLPCOrder

Définit l'ordre LPC (Linear Predictive Coding), qui affecte l'efficacité de compression et la vitesse d'encodage.

**Paramètres** :
- `inLPCOrder` — valeur d'ordre LPC (généralement 0-32)
  - 0 = pas de LPC (le plus rapide)
  - 12 = défaut pour la plupart des audios
  - 32 = compression maximale (le plus lent)

**Retourne** : `true` si réussi, `false` sinon

**Remarque** : des ordres LPC plus élevés offrent une meilleure compression mais augmentent considérablement le temps d'encodage.

#### setBlockSize

Définit la taille de bloc audio pour l'encodage. La taille de bloc affecte à la fois l'efficacité de compression et la latence.

**Paramètres** :
- `inBlockSize` — taille de bloc en échantillons
  - Valeurs courantes : 192, 576, 1152, 2304, 4608
  - Par défaut généralement 4096 pour l'audio 44,1 kHz

**Retourne** : `true` si réussi, `false` sinon

**Recommandations** :
- **Faible latence** : 192-1152 échantillons
- **Archivage standard** : 4096 échantillons
- **Compression maximale** : 4608 échantillons

#### useMidSideCoding

Active ou désactive le codage stéréo mid-side pour l'audio 2 canaux. Le codage mid-side peut améliorer la compression pour de l'audio stéréo dont les canaux gauche et droit sont fortement corrélés.

**Paramètres** :
- `inUseMidSideCoding` — `true` pour activer, `false` pour désactiver

**Retourne** : `true` si réussi, `false` sinon

**Remarque** : applicable uniquement à l'audio 2 canaux (stéréo). La plupart des morceaux de musique bénéficient du codage mid-side.

#### useAdaptiveMidSideCoding

Active ou désactive le codage stéréo mid-side adaptatif. Ce mode décide automatiquement par bloc s'il faut utiliser le codage mid-side, offrant une meilleure compression que le mode mid-side fixe.

**Paramètres** :
- `inUseAdaptiveMidSideCoding` — `true` pour activer, `false` pour désactiver

**Retourne** : `true` si réussi, `false` sinon

**Remarque** :
- Audio 2 canaux uniquement
- Remplace le paramètre `useMidSideCoding`
- Offre généralement de meilleures performances que le mid-side fixe
- Recommandé pour la plupart des scénarios d'encodage stéréo

#### useExhaustiveModelSearch

Active ou désactive la recherche exhaustive de modèle pour trouver le meilleur prédicteur de compression.

**Paramètres** :
- `inUseExhaustiveModelSearch` — `true` pour activer, `false` pour désactiver

**Retourne** : `true` si réussi, `false` sinon

**Avertissement** : la recherche exhaustive ralentit considérablement l'encodage (souvent 2 à 4 fois plus lent) mais peut offrir une compression légèrement meilleure (typiquement 1-3 % de réduction de taille).

**Recommandé** : à n'activer que pour l'archivage de contenus critiques où le temps d'encodage n'est pas un problème.

#### setRicePartitionOrder

Définit la plage de l'ordre de partition de Rice pour le codage entropique. Le codage de Rice est l'étape finale de compression dans FLAC.

**Paramètres** :
- `inMin` — ordre minimal de partition de Rice (généralement 0-2)
- `inMax` — ordre maximal de partition de Rice (généralement 3-8)

**Retourne** : `true` si réussi, `false` sinon

**Valeurs typiques** :
- Encodage rapide : min=0, max=3
- Encodage standard : min=0, max=6
- Compression maximale : min=0, max=8

### Méthodes de consultation d'état

#### encoderLevel

Obtient le niveau d'encodage actuel.

**Retourne** : niveau d'encodage actuel (0-8)

#### LPCOrder

Obtient l'ordre LPC (Linear Predictive Coding) actuel.

**Retourne** : valeur d'ordre LPC actuelle

#### blockSize

Obtient la taille de bloc actuelle.

**Retourne** : taille de bloc actuelle en échantillons

#### riceMin

Obtient l'ordre minimal de partition de Rice.

**Retourne** : ordre minimal de partition de Rice

#### riceMax

Obtient l'ordre maximal de partition de Rice.

**Retourne** : ordre maximal de partition de Rice

#### isUsingMidSideCoding

Vérifie si le codage stéréo mid-side fixe est activé.

**Retourne** : `true` si le codage mid-side est activé, `false` sinon

#### isUsingAdaptiveMidSideCoding

Vérifie si le codage stéréo mid-side adaptatif est activé.

**Retourne** : `true` si le mid-side adaptatif est activé, `false` sinon

#### isUsingExhaustiveModel

Vérifie si la recherche exhaustive de modèle est activée.

**Retourne** : `true` si la recherche exhaustive est activée, `false` sinon

## Exemples d'utilisation

### Exemple C# — archivage haute qualité

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class FLACArchivalEncoder
{
    public void ConfigureHighQualityArchival(IBaseFilter audioEncoder)
    {
        // Interroger l'interface de l'encodeur FLAC
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null)
        {
            Console.WriteLine("Error: Filter does not support IFLACEncodeSettings");
            return;
        }

        // Verifier si on peut modifier les parametres
        if (!flacEncoder.canModifySettings())
        {
            Console.WriteLine("Warning: Cannot modify encoder settings at this time");
            return;
        }

        // Parametres d'archivage haute qualite
        flacEncoder.setEncodingLevel(8);              // Compression maximale
        flacEncoder.setLPCOrder(12);                  // Bon ordre LPC pour la musique
        flacEncoder.setBlockSize(4096);               // Taille de bloc standard pour 44.1 kHz
        flacEncoder.useAdaptiveMidSideCoding(true);   // Mid-side adaptatif pour la stereo
        flacEncoder.useExhaustiveModelSearch(true);   // Meilleure compression possible
        flacEncoder.setRicePartitionOrder(0, 8);      // Plage de partition de Rice maximale

        Console.WriteLine("FLAC encoder configured for high-quality archival:");
        Console.WriteLine($"  Encoding Level: {flacEncoder.encoderLevel()}");
        Console.WriteLine($"  LPC Order: {flacEncoder.LPCOrder()}");
        Console.WriteLine($"  Block Size: {flacEncoder.blockSize()}");
        Console.WriteLine($"  Adaptive Mid-Side: {flacEncoder.isUsingAdaptiveMidSideCoding()}");
        Console.WriteLine($"  Exhaustive Search: {flacEncoder.isUsingExhaustiveModel()}");
        Console.WriteLine($"  Rice Partition: {flacEncoder.riceMin()}-{flacEncoder.riceMax()}");
    }
}
```

### Exemple C# — encodage temps réel rapide

```csharp
public class FLACRealTimeEncoder
{
    public void ConfigureFastEncoding(IBaseFilter audioEncoder)
    {
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null || !flacEncoder.canModifySettings())
            return;

        // Parametres d'encodage rapide pour usage temps reel
        flacEncoder.setEncodingLevel(2);              // Encodage rapide
        flacEncoder.setLPCOrder(8);                   // LPC plus faible pour la vitesse
        flacEncoder.setBlockSize(1152);               // Blocs plus petits pour moins de latence
        flacEncoder.useAdaptiveMidSideCoding(true);   // Toujours une bonne compression
        flacEncoder.useExhaustiveModelSearch(false);  // Desactiver pour la vitesse
        flacEncoder.setRicePartitionOrder(0, 4);      // Plage de Rice reduite

        Console.WriteLine("FLAC encoder configured for fast real-time encoding");
        Console.WriteLine($"  Encoding Level: {flacEncoder.encoderLevel()}");
        Console.WriteLine($"  LPC Order: {flacEncoder.LPCOrder()}");
        Console.WriteLine($"  Block Size: {flacEncoder.blockSize()} (lower latency)");
    }
}
```

### Exemple C# — encodage musique équilibré

```csharp
public class FLACMusicEncoder
{
    public void ConfigureBalancedMusic(IBaseFilter audioEncoder)
    {
        var flacEncoder = audioEncoder as IFLACEncodeSettings;
        if (flacEncoder == null || !flacEncoder.canModifySettings())
            return;

        // Parametres equilibres pour l'encodage musical (bonne compression, vitesse raisonnable)
        flacEncoder.setEncodingLevel(5);              // Compression equilibree
        flacEncoder.setLPCOrder(12);                  // LPC standard pour la musique
        flacEncoder.setBlockSize(4096);               // Optimal pour 44.1 kHz
        flacEncoder.useAdaptiveMidSideCoding(true);   // Mid-side adaptatif
        flacEncoder.useExhaustiveModelSearch(false);  // Non necessaire pour la musique
        flacEncoder.setRicePartitionOrder(0, 6);      // Bonne plage de Rice

        Console.WriteLine("FLAC encoder configured for balanced music encoding");
    }
}
```

### Exemple C++ — archivage haute qualité

```cpp
#include <dshow.h>
#include <iostream>
#include "IFLACEncodeSettings.h"

void ConfigureHighQualityFLAC(IBaseFilter* pAudioEncoder)
{
    IFLACEncodeSettings* pFLACEncoder = NULL;
    HRESULT hr = S_OK;

    // Interroger l'interface de l'encodeur FLAC
    hr = pAudioEncoder->QueryInterface(IID_IFLACEncodeSettings,
                                       (void**)&pFLACEncoder);
    if (FAILED(hr) || !pFLACEncoder)
    {
        std::cout << "Error: Filter does not support IFLACEncodeSettings" << std::endl;
        return;
    }

    // Verifier si on peut modifier les parametres
    if (!pFLACEncoder->canModifySettings())
    {
        std::cout << "Warning: Cannot modify encoder settings" << std::endl;
        pFLACEncoder->Release();
        return;
    }

    // Configurer les parametres d'archivage haute qualite
    pFLACEncoder->setEncodingLevel(8);              // Compression maximale
    pFLACEncoder->setLPCOrder(12);                  // Bon ordre LPC
    pFLACEncoder->setBlockSize(4096);               // Taille de bloc standard
    pFLACEncoder->useAdaptiveMidSideCoding(TRUE);   // Mid-side adaptatif
    pFLACEncoder->useExhaustiveModelSearch(TRUE);   // Meilleure compression
    pFLACEncoder->setRicePartitionOrder(0, 8);      // Plage maximale

    // Afficher la configuration
    std::cout << "FLAC encoder configured for high-quality archival:" << std::endl;
    std::cout << "  Encoding Level: " << pFLACEncoder->encoderLevel() << std::endl;
    std::cout << "  LPC Order: " << pFLACEncoder->LPCOrder() << std::endl;
    std::cout << "  Block Size: " << pFLACEncoder->blockSize() << std::endl;
    std::cout << "  Adaptive Mid-Side: "
              << (pFLACEncoder->isUsingAdaptiveMidSideCoding() ? "Yes" : "No") << std::endl;
    std::cout << "  Exhaustive Search: "
              << (pFLACEncoder->isUsingExhaustiveModel() ? "Yes" : "No") << std::endl;

    pFLACEncoder->Release();
}
```

### Exemple C++ — encodage temps réel rapide

```cpp
void ConfigureFastFLAC(IBaseFilter* pAudioEncoder)
{
    IFLACEncodeSettings* pFLACEncoder = NULL;
    HRESULT hr = pAudioEncoder->QueryInterface(IID_IFLACEncodeSettings,
                                               (void**)&pFLACEncoder);
    if (SUCCEEDED(hr) && pFLACEncoder)
    {
        if (pFLACEncoder->canModifySettings())
        {
            // Configuration d'encodage rapide
            pFLACEncoder->setEncodingLevel(2);              // Rapide
            pFLACEncoder->setLPCOrder(8);                   // LPC plus faible
            pFLACEncoder->setBlockSize(1152);               // Blocs plus petits
            pFLACEncoder->useAdaptiveMidSideCoding(TRUE);   // Toujours bon
            pFLACEncoder->useExhaustiveModelSearch(FALSE);  // Desactive pour la vitesse
            pFLACEncoder->setRicePartitionOrder(0, 4);      // Plage reduite

            std::cout << "FLAC encoder configured for fast real-time encoding" << std::endl;
        }
        pFLACEncoder->Release();
    }
}
```

### Exemple Delphi — archivage haute qualité

```delphi
uses
  DirectShow9, ActiveX;

procedure ConfigureHighQualityFLAC(AudioEncoder: IBaseFilter);
var
  FLACEncoder: IFLACEncodeSettings;
  hr: HRESULT;
begin
  // Interroger l'interface de l'encodeur FLAC
  hr := AudioEncoder.QueryInterface(IID_IFLACEncodeSettings, FLACEncoder);
  if Failed(hr) or (FLACEncoder = nil) then
  begin
    WriteLn('Error: Filter does not support IFLACEncodeSettings');
    Exit;
  end;

  try
    // Verifier si on peut modifier les parametres
    if not FLACEncoder.canModifySettings then
    begin
      WriteLn('Warning: Cannot modify encoder settings');
      Exit;
    end;

    // Configurer les parametres d'archivage haute qualite
    FLACEncoder.setEncodingLevel(8);              // Compression maximale
    FLACEncoder.setLPCOrder(12);                  // Bon ordre LPC
    FLACEncoder.setBlockSize(4096);               // Taille de bloc standard
    FLACEncoder.useAdaptiveMidSideCoding(True);   // Mid-side adaptatif
    FLACEncoder.useExhaustiveModelSearch(True);   // Meilleure compression
    FLACEncoder.setRicePartitionOrder(0, 8);      // Plage maximale

    // Afficher la configuration
    WriteLn('FLAC encoder configured for high-quality archival:');
    WriteLn('  Encoding Level: ', FLACEncoder.encoderLevel);
    WriteLn('  LPC Order: ', FLACEncoder.LPCOrder);
    WriteLn('  Block Size: ', FLACEncoder.blockSize);
    WriteLn('  Adaptive Mid-Side: ', FLACEncoder.isUsingAdaptiveMidSideCoding);
    WriteLn('  Exhaustive Search: ', FLACEncoder.isUsingExhaustiveModel);

  finally
    FLACEncoder := nil;
  end;
end;
```

### Exemple Delphi — encodage musique équilibré

```delphi
procedure ConfigureBalancedMusicFLAC(AudioEncoder: IBaseFilter);
var
  FLACEncoder: IFLACEncodeSettings;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IFLACEncodeSettings, FLACEncoder)) then
  begin
    try
      if FLACEncoder.canModifySettings then
      begin
        // Parametres equilibres pour la musique
        FLACEncoder.setEncodingLevel(5);              // Equilibre
        FLACEncoder.setLPCOrder(12);                  // Standard pour la musique
        FLACEncoder.setBlockSize(4096);               // Optimal
        FLACEncoder.useAdaptiveMidSideCoding(True);   // Adaptatif
        FLACEncoder.useExhaustiveModelSearch(False);  // Non necessaire
        FLACEncoder.setRicePartitionOrder(0, 6);      // Bonne plage

        WriteLn('FLAC encoder configured for balanced music encoding');
      end;
    finally
      FLACEncoder := nil;
    end;
  end;
end;
```

## Bonnes pratiques

### Choix du niveau d'encodage

**Niveaux 0-2** : encodage rapide, adapté aux applications temps réel
- À utiliser quand la vitesse d'encodage est critique
- Compression typique : 50-55 % de la taille originale

**Niveaux 3-5** : encodage équilibré (recommandé pour la plupart des usages)
- Bon équilibre entre vitesse et compression
- Compression typique : 45-50 % de la taille originale
- **Le niveau 5 est recommandé** pour l'archivage généraliste

**Niveaux 6-8** : compression maximale, encodage plus lent
- À utiliser pour l'archivage à long terme où l'espace de stockage est critique
- Compression typique : 40-45 % de la taille originale
- L'encodage peut être 2 à 5 fois plus lent que le niveau 5

### Codage stéréo mid-side

- **Activez toujours** `useAdaptiveMidSideCoding` pour l'audio stéréo
- Le mode adaptatif détermine automatiquement quand le mid-side est utile
- Offre une meilleure compression que le mode mid-side fixe
- Pas de pénalité significative sur les performances

### Recommandations d'ordre LPC

**Musique et audio général** :
- Utilisez l'ordre LPC 12 pour la plupart des encodages musicaux
- Les ordres plus élevés (16-32) apportent un bénéfice minimal pour la musique
- Les ordres plus faibles (8) conviennent à la parole

**Classique et grande plage dynamique** :
- Envisagez un ordre LPC 16-32 pour les enregistrements orchestraux
- Offre une meilleure prédiction pour le contenu harmonique complexe

### Choix de la taille de bloc

**Considérations selon la fréquence d'échantillonnage** :
- 44,1 kHz : 4096 échantillons (par défaut, ~93 ms)
- 48 kHz : 4608 échantillons (~96 ms)
- 96 kHz : 4608-8192 échantillons

**Exigences de latence** :
- Temps réel : 192-1152 échantillons
- Archivage standard : 4096 échantillons
- Compression maximale : 4608 échantillons

### Recherche exhaustive de modèle

**Quand l'activer** :
- Projets d'archivage critiques où chaque octet compte
- Temps d'encodage illimité disponible
- La réduction de la taille de fichier est primordiale

**Quand la désactiver** (recommandé pour la plupart des utilisateurs) :
- Encodage en temps réel ou quasi temps réel
- Projets d'encodage par lots volumineux
- L'amélioration de compression est typiquement < 3 %
- Le temps d'encodage augmente de 2 à 4 fois

### Ordre de partition de Rice

**Encodage rapide** : `setRicePartitionOrder(0, 3)`
**Encodage standard** : `setRicePartitionOrder(0, 6)` (recommandé)
**Compression maximale** : `setRicePartitionOrder(0, 8)`

## Dépannage

### Les paramètres ne peuvent pas être modifiés

**Symptôme** : `canModifySettings()` renvoie `false`

**Causes** :
1. Le graphe de filtres est déjà en cours d'exécution
2. L'encodeur traite activement de l'audio
3. Le filtre est dans un état incorrect

**Solutions** :
- Arrêtez le graphe de filtres avant de modifier les paramètres
- Configurez l'encodeur avant de connecter les pins du filtre
- Interrogez les paramètres avant de démarrer la lecture/capture

### Ratio de compression faible

**Symptôme** : les fichiers FLAC sont plus volumineux que prévu

**Causes possibles** :
1. Niveau d'encodage faible (0-2)
2. L'audio source est déjà compressé (MP3, AAC)
3. L'audio source présente un fort niveau de bruit
4. Taille de bloc inappropriée pour la fréquence d'échantillonnage

**Solutions** :
- Augmentez le niveau d'encodage à 5-8
- **Ne réencodez jamais de l'audio déjà compressé** — FLAC ne peut pas améliorer la qualité
- Utilisez une réduction de bruit sur l'audio source avant l'encodage
- Ajustez la taille de bloc à la fréquence d'échantillonnage (voir recommandations ci-dessus)

### Encodage trop lent

**Symptôme** : l'encodage en temps réel ne peut pas suivre le flux audio

**Solutions** :
1. Réduisez le niveau d'encodage à 0-3
2. Désactivez la recherche exhaustive de modèle
3. Réduisez l'ordre LPC à 8
4. Réduisez le maximum de partition de Rice à 4
5. Utilisez des tailles de bloc plus petites (1152 ou moins)

### Pops ou clics audio dans la sortie encodée

**Symptôme** : artefacts audibles dans les fichiers FLAC encodés

**Causes possibles** :
1. L'encodeur ne peut pas traiter assez vite (sous-alimentations de tampon)
2. Taille de bloc incompatible avec la fréquence d'échantillonnage
3. Problèmes de performances matérielles

**Solutions** :
- Réduisez la complexité d'encodage (niveau plus bas, désactiver la recherche exhaustive)
- Utilisez des tailles de bloc standard pour la fréquence d'échantillonnage
- Augmentez les tailles de tampon DirectShow
- Réduisez la charge système pendant l'encodage

### Problèmes d'encodage stéréo

**Symptôme** : l'audio stéréo paraît incorrect ou mono

**À vérifier** :
- Vérifiez que l'entrée est bien stéréo (2 canaux)
- Le codage mid-side ne fonctionne qu'avec une entrée stéréo
- Vérifiez si le mid-side adaptatif est activé pour les meilleurs résultats
- Vérifiez le format audio du graphe de filtres (utilisez GraphEdit pour inspecter)

## Notes techniques

### Processus d'encodage FLAC

L'encodage FLAC implique plusieurs étapes :
1. **Blocage** : audio divisé en blocs
2. **Prédiction** : l'analyse LPC prédit les valeurs des échantillons
3. **Codage mid-side** : décorrélation stéréo facultative (pour l'audio 2 canaux)
4. **Encodage résiduel** : le codage de Rice compresse les erreurs de prédiction
5. **Assemblage de trames** : les blocs sont assemblés en trames FLAC

### Caractéristiques de performance

**Utilisation CPU par paramètre** :
- Niveau d'encodage : ~10 % d'augmentation par niveau
- Ordre LPC : ~5 % d'augmentation toutes les 4 unités d'ordre
- Recherche exhaustive : augmentation de 200-400 %
- Codage mid-side : ~2-5 % d'augmentation

**Besoins en mémoire** :
- Minimal : ~512 Ko de mémoire de travail
- Les blocs plus grands nécessitent plus de mémoire
- Pas de dépendance significative à la durée audio

### Compatibilité

Les fichiers FLAC encodés avec n'importe quelle combinaison de paramètres sont compatibles avec tous les décodeurs FLAC. Des paramètres de compression plus élevés n'affectent que le temps d'encodage et la taille de fichier, pas la compatibilité du décodeur ni la qualité de lecture.

---
## Voir aussi
- [Interface de l'encodeur MP3 LAME](lame.md)
- [Référence des codecs audio](../codecs-reference.md)
- [Présentation du pack de filtres d'encodage](../index.md)
