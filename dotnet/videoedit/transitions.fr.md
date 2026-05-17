---
title: Transitions SMPTE — Plus de 100 effets vidéo en C# .NET
description: Appliquez plus de 100 effets de transition vidéo SMPTE avec VisioForge Video Edit SDK .NET. Personnalisez largeur de bordure, douceur et positionnement.
tags:
  - Video Edit SDK
  - .NET
  - Windows

---

# Transitions

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

La transition SMPTE Wipe produit l'un des balayages standard définis par la Society of Motion Picture and Television Engineers (SMPTE) dans le document SMPTE 258M-1993, à l'exception du quad split.

## Propriétés

| Propriété | Type | Valeur par défaut | Description |
|-----------------|---------------|---------|-------------|
| Border Color | Color/TColor | Noir | Couleur de la bordure autour des arêtes du motif de balayage. |
| BorderSoftness | long | 0 | Largeur de la zone floue autour des arêtes du motif de balayage. Spécifiez zéro pour aucune zone floue. |
| BorderWidth | long | 0 | Largeur de la bordure pleine le long des arêtes du motif de balayage. Spécifiez zéro pour aucune bordure. |
| ID | long | 1 | Code SMPTE de balayage standard spécifiant le style de balayage à utiliser. Pour la liste des codes de balayage et leurs schémas associés, voir le document SMPTE 258M-1993. |
| OffsetX | long | 0 | Décalage horizontal de l'origine du balayage par rapport au centre de l'image. Valide uniquement pour les valeurs **ID** de 101 à 131. |
| OffsetY | long | 0 | Décalage vertical de l'origine du balayage par rapport au centre de l'image. Valide uniquement pour les valeurs **ID** de 101 à 131. |
| ReplicateX | long | 0 | Nombre de fois où répliquer le motif de balayage horizontalement. Valide uniquement pour les valeurs **ID** de 101 à 131. |
| ReplicateY | long | 0 | Nombre de fois où répliquer le motif de balayage verticalement. Valide uniquement pour les valeurs **ID** de 101 à 131. |
| ScaleX | double | 1.0 | Facteur d'étirement horizontal du balayage, en pourcentage de la définition originale du balayage. Valide uniquement pour les valeurs **ID** de 101 à 131. |
| ScaleY | double | 1.0 | Facteur d'étirement vertical du balayage, en pourcentage de la définition originale du balayage. Valide uniquement pour les valeurs **ID** de 101 à 131. |

## SMPTE

Cette transition prend en charge les balayages SMPTE standard suivants :

| Numéro | Description | Numéro | Description |
|--------|-----------------------------------|--------|--------------------------------------------|
| 1 | Horizontal | 211 | Radial, gauche-droite, haut |
| 2 | Vertical | 212 | Radial, haut-bas, droite |
| 3 | Coin supérieur gauche | 213 | Radial, gauche-droite, haut-bas |
| 4 | Coin supérieur droit | 214 | Radial, haut-bas, gauche-droite |
| 5 | Coin inférieur droit | 221 | Radial, haut |
| 6 | Coin inférieur gauche | 222 | Radial, droite |
| 7 | Quatre coins | 223 | Radial, bas |
| 8 | Quatre carrés | 224 | Radial, gauche |
| 21 | Portes de grange, verticales | 225 | Radial, haut sens horaire, bas sens horaire |
| 22 | Portes de grange, horizontales | 226 | Radial, gauche sens horaire, droite sens horaire |
| 23 | Centre haut | 227 | Radial, haut sens horaire, bas sens antihoraire |
| 24 | Centre droit | 228 | Radial, gauche sens horaire, droite sens antihoraire |
| 25 | Centre bas | 231 | Radial, division haute |
| 26 | Centre gauche | 232 | Radial, division droite |
| 41 | Diagonale, NO vers SE | 233 | Radial, division basse |
| 42 | Diagonale, NE vers SO | 234 | Radial, division gauche |
| 43 | Triangles, haut/bas | 235 | Radial, division haut-bas |
| 44 | Triangles, gauche/droite | 236 | Radial, division gauche-droite |
| 45 | Bande diagonale, SO vers NE | 241 | Radial, coin supérieur gauche |
| 46 | Bande diagonale, NO vers SE | 242 | Radial, coin inférieur gauche |
| 47 | Croix | 243 | Radial, coin inférieur droit |
| 48 | Losange | 244 | Radial, coin supérieur droit |
| 61 | Coin, haut | 245 | Radial, haut-gauche, bas-droite |
| 62 | Coin, droite | 246 | Radial, bas-gauche, haut-droite |
| 63 | Coin, bas | 251 | Radial central, haut |
| 64 | Coin, gauche | 252 | Radial central, gauche |
| 65 | V | 253 | Radial central, bas |
| 66 | V, droite | 254 | Radial central, droite |
| 67 | V, inversé | 261 | Radial en boîte, droite |
| 68 | V, gauche | 262 | Radial en boîte, haut |
| 71 | Dents de scie, gauche | 263 | Radial central, haut, bas |
| 72 | Dents de scie, haut | 264 | Radial central, gauche, droite |
| 73 | Dents de scie, verticales | 301 | Matrice, horizontal |
| 74 | Dents de scie, horizontales | 302 | Matrice, vertical |
| 101 | Boîte | 303 | Matrice, diagonale, haut-gauche |
| 102 | Diamant | 304 | Matrice, diagonale, haut-droite |
| 103 | Triangle, haut | 305 | Matrice, diagonale, bas-droite |
| 104 | Triangle, droite | 306 | Matrice, diagonale, bas-gauche |
| 105 | Triangle, bas | 310 | Matrice, sens horaire haut-gauche |
| 106 | Triangle, gauche | 311 | Matrice, sens horaire haut-droite |
| 107 | Pointe de flèche, haut | 312 | Matrice, sens horaire bas-droite |
| 108 | Pointe de flèche, droite | 313 | Matrice, sens horaire bas-gauche |
| 109 | Pointe de flèche, bas | 314 | Matrice, sens antihoraire haut-gauche |
| 110 | Pointe de flèche, gauche | 315 | Matrice, sens antihoraire haut-droite |
| 111 | Pentagone, haut | 316 | Matrice, sens antihoraire bas-droite |
| 112 | Pentagone, bas | 317 | Matrice, sens antihoraire bas-gauche |
| 113 | Hexagone | 320 | Matrice, vertical haut-gauche, haut-droite |
| 114 | Hexagone, pivoté | 321 | Matrice, vertical bas-gauche, bas-droite |
| 119 | Cercle | 322 | Matrice, vertical haut-gauche, bas-droite |
| 120 | Ovale, horizontal | 323 | Matrice, vertical bas-gauche, haut-droite |
| 121 | Ovale, vertical | 324 | Matrice, horizontal haut-gauche, bas-gauche |
| 122 | Œil, horizontal | 325 | Matrice, horizontal haut-droite, bas-droite |
| 123 | Œil, vertical | 326 | Matrice, horizontal haut-gauche, bas-droite |
| 124 | Rectangle arrondi, horizontal | 327 | Matrice, horizontal haut-droite, bas-gauche |
| 125 | Rectangle arrondi, vertical | 328 | Matrice, diagonale bas-gauche, haut-droite |
| 127 | Étoile à 4 branches | 329 | Matrice, diagonale haut-gauche, bas-droite |
| 128 | Étoile à 4 branches | 340 | Matrice, double spirale haut |
| 129 | Étoile à 6 branches | 341 | Matrice, double spirale bas |
| 130 | Cœur | 342 | Matrice, double spirale gauche |
| 131 | Trou de serrure | 343 | Matrice, double spirale droite |
| 201 | Radial, 12 heures | 344 | Matrice, spirale quadruple, haut-bas |
| 202 | Radial, 3 heures | 345 | Matrice, spirale quadruple, gauche-droite |
| 203 | Radial, 6 heures | 350 | Cascade, gauche |
| 204 | Radial, 9 heures | 351 | Cascade, droite |
| 205 | Radial, 12 + 6 heures | 352 | Cascade, horizontale, gauche |
| 206 | Radial, 3 + 9 heures | 353 | Cascade, horizontale, droite |
| 207 | Radial, 4 directions | 409 | Masque aléatoire |
  
---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.