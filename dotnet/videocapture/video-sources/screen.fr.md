---
title: Capture d'écran en C# .NET — bureau complet ou région
description: Capturez l'écran entier, des fenêtres ou des régions personnalisées avec le SDK VisioForge via les API DirectX 11/12 et Windows Graphics Capture.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - Webcam
  - IP Camera
  - Screen Capture
  - C#
primary_api_classes:
  - ScreenCaptureD3D11SourceSettings
  - ScreenCaptureSourceSettings
  - ScreenCaptureDX9SourceSettings

---

# Guide d'implémentation de la capture d'écran

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction à la capture d'écran

La technologie de capture d'écran permet aux développeurs d'enregistrer et de diffuser de manière programmatique le contenu visuel affiché sur un moniteur d'ordinateur. Cette fonctionnalité puissante sert de fondation à de nombreuses applications, notamment :

- Outils de support à distance et d'assistance technique
- Création de démonstrations logicielles et de tutoriels
- Enregistrement et streaming de gameplay
- Systèmes de webinaires et de présentations
- Assurance qualité et automatisation des tests

Le Video Capture SDK .Net fournit aux développeurs des outils robustes pour capturer le contenu de l'écran avec performance et flexibilité. Le SDK prend en charge la capture d'écrans entiers, de fenêtres d'applications individuelles ou de régions d'écran personnalisées.

## Prise en charge des plateformes et vue d'ensemble technologique

### Implémentation Windows

Sur les plateformes Windows, le SDK tire parti de la puissance des technologies DirectX pour atteindre des performances optimales. Les développeurs peuvent choisir entre :

- **DirectX 9** : prise en charge héritée pour les systèmes plus anciens
- **DirectX 11/12** : implémentation moderne offrant des performances et une efficacité supérieures

DirectX 11 est particulièrement recommandé pour les scénarios de capture de fenêtre en raison de sa meilleure gestion de la composition des fenêtres et de ses caractéristiques de performance supérieures.

=== "VideoCaptureCore"

    
    ### Configuration de capture de base
    
    L'implémentation VideoCaptureCore offre des options de configuration simples pour contrôler le processus de capture :
    
    - `GrabMouseCursor` : activer ou désactiver la visibilité du curseur dans le contenu capturé
    - `DisplayIndex` : sélectionner quel affichage capturer dans une configuration multi-moniteurs (indexé à zéro)
    - `ScreenPreview` / `ScreenCapture` : définir le mode opérationnel pour la visualisation ou l'enregistrement
    

=== "VideoCaptureCoreX"

    
    ### Configuration de capture avancée
    
    VideoCaptureCoreX offre un contrôle plus granulaire grâce à des classes de configuration dédiées :
    
    - `ScreenCaptureDX9SourceSettings` : configurer une capture basée sur DirectX 9
    - `ScreenCaptureD3D11SourceSettings` : configurer une capture basée sur DirectX 11 avec des performances améliorées
    


## Implémentation de la capture plein écran et de région

La capture d'un écran complet ou d'une région d'écran définie est une exigence courante pour de nombreuses applications. Ci-dessous, vous trouverez les approches d'implémentation pour VideoCaptureCore et VideoCaptureCoreX.

=== "VideoCaptureCore"

    
    ### Configuration de la capture plein écran et de région
    
    Le code suivant montre comment configurer les paramètres de capture d'écran pour le mode plein écran ou une région rectangulaire spécifique :
    
    ```csharp
    // Définir les paramètres de source de capture d'écran
    VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
    {
         // Mettre à true pour capturer tout l'écran
        FullScreen = false,
    
         // Définir le bord gauche de la zone d'écran (X absolu en pixels)
        Left = 0,
    
        // Définir le bord supérieur de la zone d'écran (Y absolu en pixels)
        Top = 0, 
    
        // Définir le bord droit (X absolu en pixels, pas la largeur)
        Right = 640, 
    
        // Définir le bord inférieur (Y absolu en pixels, pas la hauteur)
        Bottom = 480, 
    
        // Définir l'index d'affichage
        DisplayIndex = 0, 
    
        // Définir la fréquence d'images
        FrameRate = new VideoFrameRate(25), 
    
         // Mettre à true pour capturer le curseur de la souris
        GrabMouseCursor = true
    };
    ```
    
    Lorsque `FullScreen` est défini sur `true`, les propriétés `Left`, `Top`, `Right` et `Bottom` sont ignorées, et tout l'écran spécifié par `DisplayIndex` est capturé. Notez que le rectangle est spécifié en coordonnées absolues de coins en pixels — `Right` et `Bottom` sont les coordonnées du coin opposé, et non la largeur/hauteur de la région.
    
    Pour les configurations multi-moniteurs, la propriété `DisplayIndex` identifie quel moniteur capturer, 0 représentant l'affichage principal.
    

=== "VideoCaptureCoreX"

    
    ### Capture d'écran avancée avec DirectX 11
    
    VideoCaptureCoreX fournit une implémentation plus puissante utilisant la technologie DirectX 11 :
    
    ```cs
    // Index de l'affichage
    var screenID = 0;
    
    // Créer une nouvelle source de capture d'écran utilisant DirectX 11
    var source = new ScreenCaptureD3D11SourceSettings(); 
    
    // Définir l'API de capture
    source.API = D3D11ScreenCaptureAPI.WGC; 
    
    // Définir la fréquence d'images
    source.FrameRate = new VideoFrameRate(25);
    
    // Définir la zone d'écran ou le mode plein écran
    if (fullscreen)
    {
        // Énumérer tous les écrans et définir la zone d'écran
        for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
        {
            if (i == screenID)
            {
                source.Rectangle = new VisioForge.Core.Types.Rect(System.Windows.Forms.Screen.AllScreens[i].Bounds);
            }
        }
    }
    else
    {
        // Définir la zone d'écran
        source.Rectangle = new VisioForge.Core.Types.Rect(0, 0, 1280, 720); 
    }
    
    // Mettre à true pour capturer le curseur de la souris
    source.CaptureCursor = true; 
    
    // Définir l'index du moniteur
    source.MonitorIndex = screenID; 
    
    // Définir la source de capture d'écran
    VideoCapture1.Video_Source = source; 
    ```
    
    L'option API Windows Graphics Capture (WGC) offre d'excellentes performances sur Windows 10 et versions ultérieures. Cette approche illustre également l'utilisation de `System.Windows.Forms.Screen.AllScreens` pour déterminer programmatiquement les limites des affichages disponibles.
    


## Implémentation de la capture de fenêtre

La capture de fenêtres d'application spécifiques permet un enregistrement ciblé d'applications individuelles sans inclure d'autre contenu du bureau. Ceci est particulièrement utile pour :

- Tutoriels spécifiques à une application
- Démonstrations de logiciels
- Scénarios de support où une seule application est pertinente

=== "VideoCaptureCore"

    
    ### Capture de fenêtre de base
    
    Pour capturer une fenêtre spécifique avec VideoCaptureCore :
    
    ```csharp
    // Définir les paramètres de source de capture d'écran
    VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
    {
        // Désactiver la capture plein écran
        FullScreen = false, 
    
        // Définir le handle de la fenêtre
        WindowHandle = windowHandle, 
    
         // Définir la fréquence d'images
        FrameRate = new VideoFrameRate(25),
    
         // Mettre à true pour capturer le curseur de la souris
        GrabMouseCursor = true
    };
    ```
    
    Le paramètre `windowHandle` doit être un HWND valide pour la fenêtre cible. Obtenez-le via `FindWindow` (P/Invoke) :

    ```csharp
    using System.Runtime.InteropServices;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // Passez null pour lpClassName pour rechercher uniquement par titre.
    IntPtr windowHandle = FindWindow(null, "Untitled - Notepad");
    if (windowHandle == IntPtr.Zero) throw new InvalidOperationException("Target window not found.");
    ```
    

=== "VideoCaptureCoreX"

    
    ### Capture de fenêtre améliorée
    
    VideoCaptureCoreX fournit une implémentation optimisée de capture de fenêtre :
    
    ```cs
    // Créer la source Direct3D11
    var source = new ScreenCaptureD3D11SourceSettings();
    
    // Définir l'API de capture
    source.API = D3D11ScreenCaptureAPI.WGC; 
    
    // Définir la fréquence d'images
    source.FrameRate = new VideoFrameRate(25);
    
    // Définir le handle de la fenêtre
    source.WindowHandle = windowHandle;
    
    VideoCapture1.Video_Source = source; // Définir la source de capture d'écran
    ```
    
    L'implémentation DirectX 11 offre de meilleures performances, en particulier pour la capture d'applications qui utilisent l'accélération matérielle.
    


## Techniques d'optimisation des performances

Optimiser les performances de capture d'écran est crucial pour maintenir des fréquences d'images élevées tout en minimisant l'utilisation du CPU et de la mémoire. Tenez compte des bonnes pratiques suivantes :

### Gestion de la fréquence d'images

Sélectionnez soigneusement une fréquence d'images appropriée selon les besoins de votre application :

- Pour l'enregistrement à usage général : 15-30 FPS est généralement suffisant
- Pour le gaming ou un contenu très dynamique : 30-60 FPS peut être nécessaire
- Pour un contenu statique ou basé sur des documents : 5-10 FPS peut considérablement réduire l'utilisation des ressources

### Considérations de résolution

Les captures en résolution plus élevée nécessitent plus de puissance de traitement et de mémoire. Envisagez :

- De capturer à une résolution inférieure et de mettre à l'échelle si approprié
- D'utiliser la capture de région au lieu du plein écran lorsque seule une partie de l'écran est pertinente
- D'implémenter un changement de résolution en fonction du type de contenu

### Accélération matérielle

Lorsqu'elle est disponible, l'utilisation de DirectX 11/12 avec l'accélération matérielle peut considérablement améliorer les performances :

- Réduit la charge CPU en tirant parti du GPU
- Fournit de meilleures fréquences d'images, en particulier avec du contenu haute résolution
- Permet un encodage plus efficace lorsqu'il est combiné à des encodeurs vidéo accélérés matériellement

## Scénarios d'implémentation avancés

### Configuration multi-moniteur

Travailler avec des configurations multi-moniteurs nécessite une attention particulière :

```csharp
// Détecter tous les moniteurs disponibles
var screens = System.Windows.Forms.Screen.AllScreens;

// Créer une liste à présenter à l'utilisateur
var screenOptions = new List<string>();
for (int i = 0; i < screens.Length; i++)
{
    screenOptions.Add($"Monitor {i+1}: {screens[i].Bounds.Width} x {screens[i].Bounds.Height}");
}

// Une fois la sélection effectuée, définir le DisplayIndex/MonitorIndex approprié
```

### Sélection de fenêtre d'application

Énumérez les fenêtres de premier plan avec `EnumWindows` + `GetWindowText` et laissez l'utilisateur en choisir une :

```csharp
using System.Runtime.InteropServices;
using System.Text;

private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

[DllImport("user32.dll")]
private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

[DllImport("user32.dll", CharSet = CharSet.Unicode)]
private static extern int GetWindowText(IntPtr hWnd, StringBuilder buf, int nMaxCount);

[DllImport("user32.dll")]
private static extern bool IsWindowVisible(IntPtr hWnd);

public static Dictionary<IntPtr, string> GetOpenWindows()
{
    var result = new Dictionary<IntPtr, string>();
    EnumWindows((hWnd, _) =>
    {
        if (!IsWindowVisible(hWnd)) return true;
        var buf = new StringBuilder(256);
        if (GetWindowText(hWnd, buf, buf.Capacity) > 0)
        {
            result[hWnd] = buf.ToString();
        }
        return true;
    }, IntPtr.Zero);
    return result;
}

// Présenter le résultat à l'utilisateur, puis :
IntPtr selectedWindowHandle = /* entrée choisie .Key */;

VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
{
    WindowHandle = selectedWindowHandle,
    // Configuration supplémentaire...
};
```

### Sélection dynamique de région

Permettre aux utilisateurs de sélectionner interactivement une région d'écran à capturer :

```csharp
// Créer un formulaire avec un fond transparent
var selectionForm = new Form
{
    FormBorderStyle = FormBorderStyle.None,
    WindowState = FormWindowState.Maximized,
    Opacity = 0.3,
    BackColor = Color.Black
};

// Ajouter des gestionnaires d'événements souris pour suivre le rectangle de sélection
// Une fois la sélection terminée

// Configurer la capture avec la région sélectionnée
VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
{
    Left = selection.Left,
    Top = selection.Top,
    Right = selection.Right,   // X absolu en pixels, pas la largeur
    Bottom = selection.Bottom, // Y absolu en pixels, pas la hauteur
    // Configuration supplémentaire...
};
```

## Dépannage des problèmes courants

### Capture vide ou noire

Si le contenu capturé apparaît vide ou noir :

- Vérifiez que vous disposez des autorisations appropriées pour la fenêtre ou l'écran
- Vérifiez si l'application utilise une accélération matérielle pouvant entrer en conflit avec la capture
- Essayez d'autres versions DirectX (9 contre 11/12)
- Pour le contenu protégé (comme la vidéo DRM), la capture peut être bloquée par des mécanismes de sécurité

### Problèmes de performance

Si vous rencontrez une capture lente ou saccadée :

- Réduisez la résolution de capture et/ou la fréquence d'images
- Utilisez DirectX 11/12 au lieu de DirectX 9 lorsque c'est possible
- Fermez les applications inutiles en arrière-plan
- Vérifiez que l'accélération matérielle est activée lorsqu'elle est applicable

## Conclusion

La fonctionnalité de capture d'écran permet aux développeurs de créer des applications puissantes à des fins de démonstration, d'éducation, de support et de divertissement. Le Video Capture SDK .Net fournit un cadre robuste pour implémenter cette fonctionnalité avec un effort de développement minimal.

En tirant parti des options de configuration appropriées à vos besoins spécifiques, vous pouvez implémenter des fonctionnalités de capture d'écran hautes performances dans vos applications .NET.

## Questions fréquemment posées

### Quelle est la différence entre la capture d'écran DirectX 9 et DirectX 11/12 ?

DirectX 11/12 utilise l'API Desktop Duplication accélérée par GPU ou Windows Graphics Capture (WGC), offrant des fréquences d'images plus élevées et une utilisation CPU plus faible que l'approche GDI de DirectX 9. DirectX 9 n'est nécessaire que pour les systèmes hérités exécutant Windows 7 ou antérieur. Pour toutes les applications Windows 10/11 modernes, utilisez `ScreenCaptureD3D11SourceSettings` avec `D3D11ScreenCaptureAPI.WGC` pour les meilleures performances.

### Puis-je capturer une fenêtre spécifique au lieu du plein écran en C# ?

Oui. Définissez la propriété `WindowHandle` sur `ScreenCaptureSourceSettings` (VideoCaptureCore) ou `ScreenCaptureD3D11SourceSettings` (VideoCaptureCoreX) avec le handle de la fenêtre cible. Obtenez le handle à l'aide de `FindWindow`, d'UI Automation ou en énumérant les fenêtres ouvertes. DirectX 11 avec WGC fournit la capture de fenêtre la plus fiable, y compris pour les applications accélérées matériellement.

### Comment capturer le curseur de la souris pendant l'enregistrement d'écran ?

Définissez `GrabMouseCursor = true` dans `ScreenCaptureSourceSettings` (VideoCaptureCore) ou `CaptureCursor = true` dans `ScreenCaptureD3D11SourceSettings` (VideoCaptureCoreX). Le curseur est inclus dans les données d'image capturées à sa position actuelle. Désactivez cette propriété lorsque vous enregistrez des tutoriels où le rendu du curseur sera ajouté en post-production.

### Le Video Capture SDK fonctionne-t-il sur plusieurs moniteurs ?

Oui. Utilisez `DisplayIndex` (VideoCaptureCore) ou `MonitorIndex` (VideoCaptureCoreX) pour sélectionner le moniteur à capturer. Énumérez les moniteurs disponibles avec `System.Windows.Forms.Screen.AllScreens` et présentez la liste à l'utilisateur. Chaque moniteur est capturé indépendamment — pour enregistrer tous les moniteurs simultanément, créez des instances de capture distinctes pour chaque affichage.

## Voir aussi

- [Capture d'écran en VB.NET](../guides/screen-capture-vb-net.md) — guide complet Visual Basic avec capture plein écran, capture de région et enregistrement audio
- [Tutoriel capture d'écran vers MP4](../video-tutorials/screen-capture-mp4.md) — tutoriel C# pour enregistrer le bureau vers MP4 avec vidéo explicative
- [Enregistrer la vidéo d'une webcam en C#](../guides/save-webcam-video.md) — capturer depuis une webcam au lieu de l'écran
- [Capture caméra IP](ip-cameras/index.md) — enregistrer depuis des caméras réseau via RTSP
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
