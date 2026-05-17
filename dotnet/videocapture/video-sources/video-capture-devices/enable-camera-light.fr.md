---
title: Lumière caméra sur tablettes Windows 10+ en C# .NET
description: Contrôlez la lumière de la caméra sur les tablettes Windows 10+ dans des applis .NET avec exemples C#, paquets requis et implémentation de l'API TorchControl.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore

---

# Activer la lumière de la caméra sur les tablettes Windows 10+

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction

Les tablettes Windows 10+ modernes sont équipées d'une fonctionnalité de lumière caméra que les développeurs peuvent contrôler par programmation. Ce guide explique comment implémenter le contrôle de la lumière caméra dans vos applications .NET en utilisant l'API TorchControl.

## Implémentation avec l'API TorchControl

L'API TorchControl offre un moyen complet de gérer les lumières caméra sur les tablettes Windows 10+. Cette API offre :

- Découverte des périphériques pour les caméras compatibles avec la torche
- Contrôle granulaire pour activer et désactiver les lumières caméra
- Compatibilité multi-périphériques

### Étapes d'implémentation de base

1. Initialiser le composant VideoCaptureCore
2. Obtenir les périphériques disponibles avec capacités de torche
3. Activer ou désactiver la fonctionnalité de torche pour des périphériques spécifiques

## Exemple de code fonctionnel

```cs
// Initialiser VideoCaptureCore
VideoCaptureCore videoCapture = await VideoCaptureCore.CreateAsync();

// Obtenir les périphériques disponibles avec capacité de torche
string[] devices = await videoCapture.TorchControl_GetDevicesAsync();

// Activer la torche pour le premier périphérique disponible
if (devices.Length > 0)
{
    await videoCapture.TorchControl_EnableAsync(devices[0], true);
}

// Désactiver la torche au besoin
await videoCapture.TorchControl_EnableAsync(devices[0], false);
```

## Exemple d'implémentation complet

```cs
using System;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.WindowsExtensions;

namespace Camera_Light_Demo
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore VideoCapture1;
        private string[] _devices;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (VideoCapture1 != null)
            {
                VideoCapture1.Dispose();
                VideoCapture1 = null;
            }
        }

        private async void btTurnOn_Click(object sender, EventArgs e)
        {
            if (_devices.Length > 0)
            {
                await VideoCapture1.TorchControl_EnableAsync(_devices[0], true);
            }
        }

        private async void btTurnOff_Click(object sender, EventArgs e)
        {
            if (_devices.Length > 0)
            {
                await VideoCapture1.TorchControl_EnableAsync(_devices[0], false);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            VideoCapture1 = await VideoCaptureCore.CreateAsync();

            _devices = await VideoCapture1.TorchControl_GetDevicesAsync();
            lbDeviceCount.Text = $"Devices found: {_devices.Length}.";
        }
    }
}
```

## Dépendances requises

Pour implémenter la fonctionnalité de lumière caméra dans votre application, vous aurez besoin :

1. **Paquet NuGet** : installez le paquet [VisioForge.DotNet.Core.WindowsExtensions](https://www.nuget.org/packages/VisioForge.DotNet.Core.WindowsExtensions).

2. **Redistribuables Video Capture** :
   - [Paquet Redist x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
   - [Paquet Redist x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Application d'exemple complète

Pour une implémentation entièrement fonctionnelle, explorez notre [application Camera Light Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Camera%20Light%20Demo) disponible dans notre dépôt GitHub.

## Notes de compatibilité

- Cette fonctionnalité est principalement conçue pour Windows 10 et les tablettes plus récentes
- Le matériel du périphérique doit prendre en charge le contrôle programmable de la lumière caméra
- Certains fabricants de périphériques peuvent implémenter des API propriétaires nécessitant une configuration supplémentaire

## Conseils de dépannage

Si vous rencontrez des problèmes pour activer la lumière de la caméra :

- Vérifiez que votre périphérique dispose d'un matériel compatible
- Assurez-vous que tous les paquets requis sont correctement installés
- Vérifiez les permissions du périphérique dans le manifeste de votre application
- Assurez-vous que le périphérique signale qu'il prend en charge la torche via TorchControl_GetDevicesAsync()

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour accéder à plus d'exemples de code.
