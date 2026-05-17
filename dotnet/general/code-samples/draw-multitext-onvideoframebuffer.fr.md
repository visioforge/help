---
title: Superpositions de texte dynamiques sur la vidéo en C# .NET
description: Plusieurs superpositions de texte sur les images vidéo avec OnVideoFrameBuffer — propriétés personnalisables et mises à jour dynamiques en .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoFrameBufferEventArgs

---

# Implémenter des superpositions de texte dynamiques sur les images vidéo en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

L'ajout de superpositions de texte au contenu vidéo est devenu essentiel pour diverses applications, du tatouage numérique et des horodatages à la création d'annotations informatives et de sous-titres. Bien que de nombreux SDK offrent des capacités de superposition de texte intégrées, ces fonctions ne fournissent pas toujours le niveau de personnalisation ou de flexibilité requis pour les projets avancés.

Ce guide montre comment implémenter des superpositions de texte personnalisées via l'événement `OnVideoFrameBuffer`. Cette approche vous donne un contrôle total sur l'apparence, la position et le comportement du texte, et permet des implémentations de superposition plus sophistiquées que ce qui est possible avec les méthodes API standard.

## Pourquoi utiliser des superpositions de texte personnalisées ?

Les API de superposition de texte standard ont souvent des limitations dans des domaines tels que :

- Le nombre d'éléments de texte concurrents
- Les options de personnalisation des polices
- Les mises à jour dynamiques du texte
- Les capacités d'animation
- Le contrôle précis du positionnement
- La gestion du canal alpha

En tirant parti de l'événement `OnVideoFrameBuffer` et en travaillant directement avec les données bitmap, vous pouvez surmonter ces limitations et implémenter exactement ce dont votre application a besoin.

## Comprendre l'approche

La technique présentée dans cet article implique :

1. Créer un bitmap transparent avec les mêmes dimensions que l'image vidéo
2. Dessiner les éléments de texte sur ce bitmap via GDI+ (System.Drawing)
3. Convertir le bitmap en tampon mémoire
4. Superposer ce tampon sur les données de l'image vidéo
5. Optionnellement mettre à jour les éléments de texte dynamiquement

Cela fournit une méthode puissante pour la création de superpositions de texte tout en maintenant de bonnes performances.

## Implémentation de base

L'exemple de code suivant montre une implémentation directe pour dessiner plusieurs superpositions de texte sur les images vidéo :

```cs
        // Image
        private Bitmap logoImage = null;

        // Tampon RGB32 de l'image
        private IntPtr logoImageBuffer = IntPtr.Zero;
        private int logoImageBufferSize = 0;

        private string text1 = "Hello World";
        private string text2 = "Hey-hey";
        private string text3 = "Ocean of pancakes";

        private void SDK_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
        {
            // dessiner le texte sur l'image
            if (logoImage == null)
            {
                logoImage = new Bitmap(e.Frame.Info.Width, e.Frame.Info.Height, PixelFormat.Format32bppArgb);

                using (var grf = Graphics.FromImage(logoImage))
                {
                    // mode d'anticrénelage
                    grf.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // mode de dessin
                    grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // mode de lissage
                    grf.SmoothingMode = SmoothingMode.HighQuality;

                    // texte 1
                    var brush1 = new SolidBrush(Color.Blue);
                    var font1 = new Font("Arial", 30, FontStyle.Regular);
                    grf.DrawString(text1, font1, brush1, 100, 100);

                    // texte 2
                    var brush2 = new SolidBrush(Color.Red);
                    var font2 = new Font("Times New Roman", 35, FontStyle.Strikeout);
                    grf.DrawString(text2, font2, brush2, e.Frame.Info.Width / 2, e.Frame.Info.Height / 2);

                    // texte 3
                    var brush3 = new SolidBrush(Color.Green);
                    var font3 = new Font("Verdana", 40, FontStyle.Italic);
                    grf.DrawString(text3, font3, brush3, 200, 200);
                }
            }

            // créer le tampon d'image s'il n'est pas alloué ou de taille zéro
            if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
            {
                if (logoImageBuffer == IntPtr.Zero)
                {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                }
                else
                {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;

                        Marshal.FreeCoTaskMem(logoImageBuffer);
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                }

                BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
            }

            // Dessiner l'image
            FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width, e.Frame.Info.Height, 0, 0);

            e.UpdateData = true;
        }
```

### Explication des composants clés

1. **Création du bitmap** : nous créons un bitmap 32 bits (avec canal alpha) correspondant aux dimensions de l'image vidéo
2. **Paramètres de dessin** : nous configurons l'anticrénelage, l'interpolation et le lissage pour un rendu de texte de haute qualité
3. **Configuration du texte** : chaque élément de texte a sa propre police, couleur et position
4. **Gestion de la mémoire** : nous allouons de la mémoire non managée pour le tampon bitmap
5. **Conversion bitmap vers tampon** : nous convertissons le bitmap en tampon mémoire via `BitmapHelper.BitmapToIntPtr`
6. **Superposition du tampon** : nous dessinons le tampon RGBA sur l'image vidéo via `FastImageProcessing.Draw_RGB32OnRGB24`
7. **Indicateur de mise à jour de l'image** : nous définissons `e.UpdateData = true` pour informer le SDK que les données de l'image ont été modifiées

## Implémentation avancée avec mises à jour dynamiques

Pour des applications plus interactives, vous pourriez avoir besoin de mettre à jour les superpositions de texte dynamiquement. L'implémentation suivante prend en charge les mises à jour à la volée du contenu textuel, des polices et des couleurs :

```cs
        // Image
        Bitmap logoImage = null;

        // Tampon RGB32 de l'image
        IntPtr logoImageBuffer = IntPtr.Zero;
        int logoImageBufferSize = 0;

        // paramètres de texte
        string text1 = "Hello World";
        Font font1 = new Font("Arial", 30, FontStyle.Regular);
        SolidBrush brush1 = new SolidBrush(Color.Blue);

        string text2 = "Hey-hey";
        Font font2 = new Font("Times New Roman", 35, FontStyle.Strikeout);
        SolidBrush brush2 = new SolidBrush(Color.Red);

        string text3 = "Ocean of pancakes";
        Font font3 = new Font("Verdana", 40, FontStyle.Italic);
        SolidBrush brush3 = new SolidBrush(Color.Green);

        // indicateur de mise à jour
        bool textUpdate = false;
        object textLock = new object();

        // Mettre à jour la superposition de texte, index est [1..3]
        void UpdateText(int index, string text, Font font, SolidBrush brush)
        {
            lock (textLock)
            {
                textUpdate = true;
            }

            switch (index)
            {
                case 1:
                    text1 = text;
                    font1 = font;
                    brush1 = brush;
                    break;
                case 2:
                    text2 = text;
                    font2 = font;
                    brush2 = brush;
                    break;
                case 3:
                    text3 = text;
                    font3 = font;
                    brush3 = brush;
                    break;
                default:
                    return;
            }
        }

        private void SDK_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
        {
            lock (textLock)
            {
                if (textUpdate)
                {
                    logoImage.Dispose();
                    logoImage = null;
                }

                // dessiner le texte sur l'image
                if (logoImage == null)
                {
                    logoImage = new Bitmap(e.Frame.Info.Width, e.Frame.Info.Height, PixelFormat.Format32bppArgb);

                    using (var grf = Graphics.FromImage(logoImage))
                    {
                        // mode d'anticrénelage
                        grf.TextRenderingHint = TextRenderingHint.AntiAlias;

                        // mode de dessin
                        grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        // mode de lissage
                        grf.SmoothingMode = SmoothingMode.HighQuality;

                        // texte 1
                        grf.DrawString(text1, font1, brush1, 100, 100);

                        // texte 2
                        grf.DrawString(text2, font2, brush2, e.Frame.Info.Width / 2, e.Frame.Info.Height / 2);

                        // texte 3
                        grf.DrawString(text3, font3, brush3, 200, 200);
                    }
                }

                // créer le tampon d'image s'il n'est pas alloué ou de taille zéro
                if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
                {
                    if (logoImageBuffer == IntPtr.Zero)
                    {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(e.Frame.Info.Width) * e.Frame.Info.Height;
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                    }
                    else
                    {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(e.Frame.Info.Width) * e.Frame.Info.Height;

                        Marshal.FreeCoTaskMem(logoImageBuffer);
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                    }

                    BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
                }

                if (textUpdate)
                {
                    textUpdate = false;
                    BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
                }

                // Dessiner l'image
                FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
                e.Frame.Info.Height, 0, 0);

                e.UpdateData = true;
            }
        }

        private void btUpdateText1_Click(object sender, EventArgs e)
        {
            UpdateText(1, "Hello world", new Font("Arial", 48, FontStyle.Underline),
                new SolidBrush(Color.Aquamarine));
        }
```

### Nouveautés de l'implémentation avancée

1. **Sécurité des threads** : nous utilisons un objet de verrouillage pour empêcher l'accès concurrent aux ressources partagées
2. **Mécanisme de mise à jour** : la méthode `UpdateText` fournit une interface propre pour changer les propriétés du texte
3. **Stockage des propriétés du texte** : chaque élément de texte a ses propres variables pour le contenu, la police et la couleur
4. **Détection de changement** : nous utilisons un indicateur (`textUpdate`) pour signaler que les propriétés du texte ont changé
5. **Gestion des ressources** : nous libérons l'ancien bitmap quand les propriétés du texte changent
6. **Mise à jour du tampon** : nous mettons à jour le tampon mémoire quand les propriétés du texte changent
7. **Intégration UI** : un exemple de gestionnaire de clic de bouton montre comment déclencher des mises à jour de texte

## Conseils d'optimisation des performances

Lors de l'implémentation de superpositions de texte avec cette méthode, prenez en compte ces optimisations de performance :

1. **Minimisez les recréations de bitmap** : ne recréez le bitmap que lorsque c'est nécessaire (changements de texte, changements de résolution)
2. **Mettez en cache les objets Font** : la création de polices est coûteuse ; créez les polices une fois et réutilisez-les
3. **Utilisez la mémoire efficacement** : libérez la mémoire non managée quand elle n'est plus nécessaire
4. **Optimisez les opérations de dessin** : utilisez l'accélération matérielle lorsqu'elle est disponible
5. **Tenez compte de la fréquence des mises à jour** : pour des mises à jour fréquentes, envisagez des techniques de double tamponnage
6. **Profilez votre code** : utilisez des outils de profilage de performance pour identifier les goulots d'étranglement

## Fonctionnalités avancées à envisager

Cette implémentation de base peut être étendue avec des fonctionnalités supplémentaires :

1. **Animation de texte** : implémentez le déplacement, le fondu ou d'autres animations
2. **Mise en forme du texte** : ajoutez la prise en charge de la mise en forme riche (gras, italique, etc.)
3. **Effets de texte** : implémentez des ombres, des contours ou des effets de halo
4. **Alignement du texte** : ajoutez la prise en charge de différentes options d'alignement
5. **Texte multiligne** : implémentez la gestion appropriée du texte multiligne avec retour à la ligne
6. **Localisation** : ajoutez la prise en charge de différentes langues et directions de texte
7. **Surveillance des performances** : ajoutez des diagnostics pour surveiller les performances de rendu

## Considérations de gestion de la mémoire

Lorsque vous travaillez avec de la mémoire non managée, il est crucial de gérer correctement le nettoyage des ressources :

1. Implémentez le modèle `IDisposable` dans votre classe
2. Libérez la mémoire non managée dans la méthode `Dispose`
3. Envisagez d'utiliser `SafeHandle` ou des constructions similaires pour une gestion plus sûre des ressources
4. Définissez les pointeurs de tampon sur `IntPtr.Zero` après les avoir libérés
5. Utilisez la gestion structurée des exceptions autour des opérations mémoire

## Exemple de nettoyage

```cs
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Libérer les ressources managées
        if (logoImage != null)
        {
            logoImage.Dispose();
            logoImage = null;
        }
    }
    
    // Libérer les ressources non managées
    if (logoImageBuffer != IntPtr.Zero)
    {
        Marshal.FreeCoTaskMem(logoImageBuffer);
        logoImageBuffer = IntPtr.Zero;
        logoImageBufferSize = 0;
    }
    
    base.Dispose(disposing);
}
```

## Dépendances requises

- Composants redistribuables du SDK

## Conclusion

L'implémentation de superpositions de texte personnalisées via l'événement `OnVideoFrameBuffer` fournit une solution puissante et flexible pour les applications qui nécessitent des capacités d'affichage de texte avancées. Bien qu'elle nécessite plus de code que l'utilisation des méthodes API intégrées, la flexibilité et le contrôle supplémentaires en valent la peine pour les applications vidéo sophistiquées.

En suivant les modèles présentés dans ce guide, vous pouvez créer des superpositions de texte dynamiques et de haute qualité qui peuvent être mises à jour en temps réel, ce qui offre une expérience utilisateur riche dans vos applications vidéo.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.
