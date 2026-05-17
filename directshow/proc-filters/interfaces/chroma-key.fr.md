---
title: Filtre DirectShow chroma key — interface IVFChromaKey
description: Interface IVFChromaKey pour la composition fond vert et fond bleu avec contrôle de tolérance et remplacement d'arrière-plan dans DirectShow.
tags:
  - DirectShow
  - C++
  - Windows
  - Effects
  - Mixing
  - C#
primary_api_classes:
  - IVFChromaKey
  - CVFChromaColor
  - IBaseFilter
  - IVFVideoMixer
  - IVFEffectsPro

---

# Référence de l'interface IVFChromaKey

## Vue d'ensemble

L'interface `IVFChromaKey` fournit des capacités professionnelles de composition par incrustation chroma (fond vert/fond bleu) pour les applications DirectShow. Cette interface permet le remplacement d'arrière-plan en temps réel en rendant des couleurs spécifiques transparentes, ce qui permet d'incruster des sujets filmés devant un fond coloré sur différents arrière-plans.

L'incrustation par chrominance est essentielle pour la production virtuelle, les bulletins météo, les effets vidéo et tout scénario nécessitant un remplacement d'arrière-plan.

## Définition de l'interface

- **Nom de l'interface** : `IVFChromaKey`
- **GUID** : `{AF6E8208-30E3-44f0-AAFE-787A6250BAB3}`
- **Hérite de** : `IUnknown`
- **Fichier d'en-tête** : `vf_eff_intf.h` (C++), `IVFChromaKey.cs` (.NET)

## Capacités

- **Couleurs clés** : vert, bleu, rouge ou couleurs RGB personnalisées
- **Ajustement du contraste** : seuils de contraste bas/haut séparés
- **Remplacement d'arrière-plan** : image statique ou vidéo
- **Traitement temps réel** : accéléré matériellement lorsque disponible
- **Qualité des bords** : tolérance ajustable pour des bords lissés

---
## Référence des méthodes
### Configuration du seuil de contraste
#### chroma_put_contrast
Définit la plage de seuil de contraste pour l'incrustation chroma.
**Syntaxe (C++)** :
```cpp
HRESULT chroma_put_contrast(int low, int high);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int chroma_put_contrast(int low, int high);
```
**Paramètres** :
- `low` : seuil de contraste bas (0-255)
  - Valeurs basses = suppression de plus de couleurs similaires
  - Valeurs hautes = correspondance de couleur plus stricte
- `high` : seuil de contraste haut (0-255)
  - Définit la limite supérieure de correspondance de couleur
  - Crée une plage de couleurs clés acceptables
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Ces valeurs définissent la plage de similarité chromatique pour l'incrustation
- L'intervalle entre `low` et `high` crée un dégradé pour le lissage des bords
- Plages typiques :
  - Incrustation serrée : low=10, high=30
  - Incrustation standard : low=30, high=70
  - Incrustation large : low=50, high=120
- À ajuster selon les conditions d'éclairage et la qualité du fond
**Principe de fonctionnement** :
```
Pixels avec distance chromatique < low  -> totalement transparents
Pixels avec distance chromatique > high -> totalement opaques
Pixels entre low et high                -> partiellement transparents (degrade)
```
**Exemple (C++)** :
```cpp
IVFChromaKey* pChroma = nullptr;
pFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChroma);
// Configuration standard pour fond vert
pChroma->chroma_put_contrast(40, 80);
pChroma->Release();
```
**Exemple (C#)** :
```csharp
var chroma = filter as IVFChromaKey;
if (chroma != null)
{
    // Incrustation serree pour fond vert propre
    chroma.chroma_put_contrast(20, 50);
}
```
---

### Sélection de la couleur

#### chroma_put_color

Définit la couleur d'incrustation chroma à rendre transparente.

**Syntaxe (C++)** :
```cpp
HRESULT chroma_put_color(int color);
```

**Syntaxe (C#)** :
```csharp
[PreserveSig]
int chroma_put_color(int color);
```

**Paramètres** :
- `color` : valeur de couleur de l'incrustation chroma

**Valeurs de couleur** (énumération CVFChromaColor) :

| Valeur | Couleur | Équivalent RGB | Cas d'usage |
|-------|-------|----------------|----------|
| `0` (Chroma_Green) | Vert | 0x00FF00 | Incrustation chroma standard (la plus courante) |
| `1` (Chroma_Blue) | Bleu | 0x0000FF | Alternative au vert |
| `2` (Chroma_Red) | Rouge | 0xFF0000 | Cas particuliers |
| RGB personnalisé | Toute couleur | 0xRRGGBB | Correspondance avec une couleur précise |

**Retour** : `S_OK` (0) en cas de succès.

**Remarques d'utilisation** :
- Le vert est standard pour l'incrustation chroma (la peau humaine en contient peu)
- Le bleu est utilisé lorsque la scène contient des objets verts
- Une valeur RGB personnalisée peut être utilisée pour une correspondance précise
- La couleur doit être uniforme sur tout le fond pour de meilleurs résultats

**Exemple (C++)** :
```cpp
// Utiliser une incrustation chroma verte
pChroma->chroma_put_color(Chroma_Green);

// Utiliser une incrustation chroma bleue
pChroma->chroma_put_color(Chroma_Blue);

// Utiliser une couleur personnalisee (par ex. magenta)
pChroma->chroma_put_color(0xFF00FF);
```

**Exemple (C#)** :
```csharp
// Fond vert standard
chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);

// Fond bleu
chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);

// Vert-jaune personnalise
chroma.chroma_put_color(0x88FF00);
```

---
### Image d'arrière-plan
#### chroma_put_image
Définit une image d'arrière-plan de remplacement pour les zones transparentes.
**Syntaxe (C++)** :
```cpp
HRESULT chroma_put_image(BSTR filename);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int chroma_put_image([MarshalAs(UnmanagedType.BStr)] string filename);
```
**Paramètres** :
- `filename` : chemin du fichier image d'arrière-plan (BMP, PNG, JPG, etc.)
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- L'image est étirée pour remplir l'image entière
- Utilisez NULL ou une chaîne vide pour utiliser un arrière-plan vidéo à la place
- Une image statique est plus efficace qu'un arrière-plan vidéo
- L'image est chargée une seule fois et mise en cache
- Formats pris en charge : BMP, PNG, JPEG, GIF, TIFF
**Exemple (C++)** :
```cpp
// Definir une image de fond de bureau
pChroma->chroma_put_image(L"C:\\Backgrounds\\office.jpg");
// Supprimer l'image de fond (utiliser l'entree video a la place)
pChroma->chroma_put_image(NULL);
```
**Exemple (C#)** :
```csharp
// Fond de studio virtuel
chroma.chroma_put_image(@"C:\Backgrounds\studio.png");
// Supprimer le fond statique
chroma.chroma_put_image(null);
```
---

## Exemples de configuration complets

### Exemple 1 : configuration de base fond vert (C++)

```cpp
#include "vf_eff_intf.h"

HRESULT ConfigureBasicGreenScreen(IBaseFilter* pChromaFilter)
{
    HRESULT hr;
    IVFChromaKey* pChroma = nullptr;

    hr = pChromaFilter->QueryInterface(IID_IVFChromaKey, (void**)&pChroma);
    if (FAILED(hr))
        return hr;

    // Definir le vert comme couleur cle
    pChroma->chroma_put_color(Chroma_Green);

    // Seuils de contraste standard
    pChroma->chroma_put_contrast(40, 80);

    // Definir l'image d'arriere-plan
    pChroma->chroma_put_image(L"C:\\Backgrounds\\office_background.jpg");

    pChroma->Release();
    return S_OK;
}
```

### Exemple 2 : studio météo (C#)

```csharp
using System;
using VisioForge.DirectShowAPI;

public class WeatherStudioSetup
{
    public void ConfigureWeatherChromaKey(IBaseFilter chromaFilter)
    {
        var chroma = chromaFilter as IVFChromaKey;
        if (chroma == null)
            throw new NotSupportedException("IVFChromaKey not available");

        // Fond bleu pour cartes meteo
        chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);

        // Seuils plus serres pour une incrustation propre
        chroma.chroma_put_contrast(25, 60);

        // Arriere-plan carte meteo
        chroma.chroma_put_image(@"C:\Weather\maps\current_radar.png");
    }

    public void UpdateWeatherMap(IVFChromaKey chroma, string mapPath)
    {
        // Mettre a jour dynamiquement l'arriere-plan pendant la diffusion
        chroma.chroma_put_image(mapPath);
    }
}
```

### Exemple 3 : production virtuelle avec couleur personnalisée (C++)

```cpp
HRESULT ConfigureVirtualProduction(IVFChromaKey* pChroma)
{
    // Utiliser un vert specifique correspondant a votre fond physique
    // Mesurer la couleur reelle avec une pipette
    COLORREF customGreen = RGB(60, 220, 40);  // Nuance de vert specifique

    pChroma->chroma_put_color(customGreen);

    // Seuils de qualite professionnelle
    // Valeurs basses pour un fond vert propre et bien eclaire
    pChroma->chroma_put_contrast(15, 45);

    // Utiliser un environnement virtuel pre-calcule
    pChroma->chroma_put_image(L"D:\\VirtualSets\\studio_environment.png");

    return S_OK;
}
```

### Exemple 4 : paramètres adaptatifs d'incrustation chroma (C#)

```csharp
public class AdaptiveChromaKey
{
    private IVFChromaKey _chroma;

    public void SetupForLightingConditions(string condition)
    {
        switch (condition.ToLower())
        {
            case "perfect":
                // Fond vert propre, eclaire uniformement
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(15, 40);
                break;

            case "good":
                // Eclairage standard
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(30, 70);
                break;

            case "challenging":
                // Eclairage inegal ou fond froisse
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Green);
                _chroma.chroma_put_contrast(50, 110);
                break;

            case "outdoor":
                // Lumiere naturelle, plus difficile a controler
                _chroma.chroma_put_color((int)CVFChromaColor.Chroma_Blue);
                _chroma.chroma_put_contrast(60, 130);
                break;
        }
    }

    public void TestThresholds()
    {
        // Commencer avec une incrustation serree
        for (int low = 10; low <= 60; low += 10)
        {
            int high = low + 40;
            _chroma.chroma_put_contrast(low, high);

            // L'utilisateur examine le resultat et selectionne le meilleur reglage
            Console.WriteLine($"Testing: Low={low}, High={high}");
            System.Threading.Thread.Sleep(2000);
        }
    }
}
```

---
## Bonnes pratiques d'incrustation chroma
### Mise en place de l'éclairage
1. **Illumination uniforme**
   - Utilisez plusieurs sources lumineuses
   - Évitez les zones brillantes et les ombres sur le fond
   - Maintenez une couleur cohérente sur tout le fond
2. **Séparation du sujet**
   - Positionnez le sujet à 2-3 m du fond
   - Évite les débordements verts sur le sujet
   - Permet un contrôle indépendant de l'éclairage
3. **Qualité du fond**
   - Utilisez un tissu ou une peinture chroma key adaptés
   - Maintenez le fond sans pli
   - Conservez une saturation de couleur uniforme
### Stratégie de configuration
1. **Commencez prudemment**
   ```cpp
   // Commencer avec des seuils serres
   pChroma->chroma_put_contrast(20, 50);
   // Augmenter progressivement si necessaire
   pChroma->chroma_put_contrast(30, 70);
   ```
2. **Testez différents éclairages**
   - Ajustez les seuils à votre configuration spécifique
   - Enregistrez des préréglages pour différentes conditions
   - Documentez les valeurs qui fonctionnent
3. **Choix de la couleur**
   - Vert : choix standard (le moins présent dans les tons de peau)
   - Bleu : lorsqu'il y a des objets verts dans la scène
   - Personnalisé : correspondance avec la couleur réelle du fond pour de meilleurs résultats
### Optimisation de la qualité
1. **Paramètres de caméra**
   - Désactivez la balance des blancs automatique
   - Mise au point manuelle
   - Réduisez la netteté (évite les artefacts de bord)
2. **Réglage des seuils**
   - Valeur basse : contrôle le seuil de transparence
   - Valeur haute : contrôle la douceur des bords
   - Plage plus large = bords plus doux
3. **Qualité des bords**
   ```
   Plage serree (low=20, high=40) :
   - Bords nets
   - Peut presenter une frange verte
   - Ideal pour des fonds propres
   Plage large (low=30, high=90) :
   - Bords plus doux
   - Meilleure tolerance au debordement de couleur
   - Plus permissive avec un eclairage imparfait
   ```
---

## Scénarios courants d'incrustation chroma

### Scénario 1 : production vidéo d'entreprise

```cpp
// Environnement de studio controle
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(25, 55);
pChroma->chroma_put_image(L"corporate_office.jpg");
```

**Caractéristiques** :
- Éclairage contrôlé
- Fond vert professionnel
- Arrière-plan de bureau statique
- Exigences de haute qualité

### Scénario 2 : streamer de jeu vidéo

```cpp
// Studio personnel
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(35, 75);
pChroma->chroma_put_image(NULL);  // Utiliser la video du jeu comme arriere-plan
```

**Caractéristiques** :
- Fond vert grand public
- Éclairage variable
- Arrière-plan vidéo dynamique
- Performances temps réel critiques

### Scénario 3 : diffusion météo

```cpp
// Fond bleu avec cartes meteo
pChroma->chroma_put_color(Chroma_Blue);
pChroma->chroma_put_contrast(30, 65);
pChroma->chroma_put_image(L"weather_map_current.png");
```

**Caractéristiques** :
- Fond bleu (le vert est utilisé dans les cartes météo)
- Arrière-plans changeant dynamiquement
- Éclairage professionnel
- Précautions sur les vêtements du présentateur

### Scénario 4 : animation d'événement virtuel

```cpp
// Arriere-plan de conference virtuelle
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(40, 85);
pChroma->chroma_put_image(L"conference_hall.jpg");
```

**Caractéristiques** :
- Installation domicile/bureau
- Qualité de fond variable
- Paramètres tolérants nécessaires
- Apparence professionnelle souhaitée

---
## Dépannage
### Problème : débordement vert sur le sujet
**Symptômes** : halo ou teinte verte sur les bords du sujet
**Solutions** :
1. Augmentez la distance entre le sujet et le fond
2. Ajustez l'éclairage pour réduire la réflexion
3. Utilisez une plage de contraste plus serrée :
   ```cpp
   pChroma->chroma_put_contrast(15, 35);
   ```
4. Envisagez une correction colorimétrique en post-production
### Problème : incrustation inégale
**Symptômes** : certaines parties du fond restent non transparentes
**Solutions** :
1. Vérifiez l'uniformité de l'éclairage du fond
2. Augmentez le seuil haut :
   ```cpp
   pChroma->chroma_put_contrast(30, 100);
   ```
3. Vérifiez la cohérence de la couleur du fond
4. Envisagez une correspondance de couleur personnalisée :
   ```cpp
   // Echantillonner la couleur reelle du fond et l'utiliser
   pChroma->chroma_put_color(0x40DC28);  // Couleur mesuree
   ```
### Problème : disparition de parties du sujet
**Symptômes** : des vêtements ou des éléments du sujet deviennent transparents
**Solutions** :
1. Évitez les vêtements verts/bleus
2. Réduisez la plage de contraste :
   ```cpp
   pChroma->chroma_put_contrast(50, 90);
   ```
3. Changez de couleur clé si nécessaire :
   ```cpp
   pChroma->chroma_put_color(Chroma_Blue);  // Si le sujet porte du vert
   ```
### Problème : bords irréguliers et crénelés
**Symptômes** : qualité de bord médiocre, pixellisation visible
**Solutions** :
1. Élargissez la plage de contraste pour un dégradé plus lisse :
   ```cpp
   pChroma->chroma_put_contrast(25, 85);
   ```
2. Améliorez la qualité de l'éclairage
3. Utilisez une source vidéo de qualité supérieure
4. Assurez-vous que le sujet est bien séparé du fond
### Problème : performances dégradées
**Symptômes** : images perdues, saccades
**Solutions** :
1. Utilisez une image statique au lieu d'un arrière-plan vidéo
2. Réduisez la résolution de sortie
3. Optimisez les valeurs de seuil (ne les rendez pas trop larges)
4. Envisagez des alternatives accélérées matériellement
---

## Tableau de référence des paramètres

### Lignes directrices des seuils de contraste

| Qualité de l'éclairage | Fond | Low | High | Qualité des bords | Performance |
|-----------------|----------|-----|------|--------------|-------------|
| **Excellente** | Propre, uniforme | 15-25 | 35-50 | Nets | Meilleure |
| **Bonne** | Variations mineures | 25-35 | 50-75 | Bons | Bonne |
| **Correcte** | Quelques irrégularités | 35-50 | 75-100 | Doux | Modérée |
| **Médiocre** | Inégale/froissée | 50-70 | 100-140 | Très doux | Faible |

### Guide de choix de la couleur

| Couleur | RGB | Avantages | Inconvénients | Idéal pour |
|-------|-----|------|------|----------|
| **Vert** | 0x00FF00 | Peu présent dans la peau, lumineux | Inadapté aux objets verts | Usage général |
| **Bleu** | 0x0000FF | Alternative au vert | Denim/vêtements bleus | Cas particuliers |
| **Personnalisé** | Variable | Correspondance exacte avec le fond | Nécessite un étalonnage | Production professionnelle |

---
## Intégration avec le mélangeur vidéo
Le filtre d'incrustation chroma est souvent utilisé avec Video Mixer pour une composition avancée :
```cpp
// Le filtre chroma key supprime le vert
IVFChromaKey* pChroma = /* ... */;
pChroma->chroma_put_color(Chroma_Green);
pChroma->chroma_put_contrast(30, 70);
// Le video mixer combine le sujet avec l'arriere-plan
IVFVideoMixer* pMixer = /* ... */;
// Entree 0 : video d'arriere-plan
// Entree 1 : sujet incruste chroma (arriere-plan transparent)
```
Consultez [Interface du mélangeur vidéo](video-mixer.md) pour plus de détails.
---

## Interfaces associées

- **IVFVideoMixer** — combiner une vidéo incrustée chroma avec des arrière-plans
- **IVFEffects45** — effets vidéo supplémentaires
- **IVFEffectsPro** — traitement d'effets avancés

## Voir aussi

- [Présentation du pack de filtres de traitement](../index.md)
- [Interface du mélangeur vidéo](video-mixer.md)
- [Référence des effets](../effects-reference.md)
- [Exemples de code](../examples.md)
