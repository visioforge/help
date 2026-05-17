---
title: Enregistrer les filtres DirectShow — regsvr32, C++ et C#
description: Enregistrez les filtres DirectShow VisioForge avec regsvr32 manuel, méthodes par programmation et automatisation d'installeur, avec conseils de dépannage.
tags:
  - DirectShow
  - C++
  - Windows
  - Encoding
  - Mixing
  - MP4
  - H.264
  - AAC
  - C#
primary_api_classes:
  - FFMPEGSource
  - IBaseFilter

---

# Guide d'enregistrement des filtres DirectShow

## Vue d'ensemble

Les filtres DirectShow doivent être enregistrés auprès de Windows avant de pouvoir être utilisés dans des applications. Ce guide couvre toutes les méthodes d'enregistrement des filtres DirectShow VisioForge.

---
## Méthodes d'enregistrement
### Méthode 1 : enregistrement automatique (installeur)
La méthode recommandée pour les utilisateurs finaux consiste à utiliser l'installeur officiel.
**Installeurs disponibles** :
- `visioforge_ffmpeg_source_filter_setup.exe` — FFMPEG Source Filter
- `visioforge_vlc_source_filter_setup.exe` — VLC Source Filter
- `visioforge_processing_filters_pack_setup.exe` — Processing Filters Pack
- `visioforge_encoding_filters_pack_setup.exe` — Encoding Filters Pack
- `visioforge_virtual_camera_sdk_setup.exe` — Virtual Camera SDK
**Étapes d'installation** :
1. Exécuter l'installeur en tant qu'administrateur
2. Suivre l'assistant d'installation
3. Les filtres sont enregistrés automatiquement
4. Aucune étape supplémentaire requise
---

### Méthode 2 : enregistrement manuel (regsvr32)

Pour le développement et les tests, vous pouvez enregistrer manuellement les filtres avec l'utilitaire `regsvr32` de Windows.

#### Commande d'enregistrement

```batch
# Ouvrir l'invite de commande en tant qu'administrateur
# Clic droit Demarrer -> Invite de commandes (admin)

# Enregistrer un filtre x86 (32 bits)
regsvr32 "C:\Path\To\Filter.ax"

# Enregistrer un filtre x64 (64 bits)
regsvr32 "C:\Path\To\Filter_x64.ax"

# Desenregistrer un filtre
regsvr32 /u "C:\Path\To\Filter.ax"
```

#### Exemples spécifiques au SDK

**FFMPEG Source Filter** :
```batch
# x86
regsvr32 "C:\Program Files (x86)\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source.ax"

# x64
regsvr32 "C:\Program Files\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
```

**VLC Source Filter** :
```batch
# x86 uniquement
regsvr32 "C:\Program Files (x86)\VisioForge\VLC Source\VisioForge_VLC_Source.ax"
```

**Processing Filters Pack** (plusieurs filtres) :
```batch
# Effets video
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Effects_Pro.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"

# Melangeur video
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Mixer.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Video_Mixer_x64.ax"

# Ameliorateur audio
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Audio_Enhancer.ax"
regsvr32 "C:\Program Files\VisioForge\Processing Filters\VisioForge_Audio_Enhancer_x64.ax"
```

**Encoding Filters Pack** (plusieurs filtres) :
```batch
# Encodeur NVENC
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_NVENC.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_NVENC_x64.ax"

# Encodeur H.264
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_H264_Encoder.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_H264_Encoder_x64.ax"

# Encodeur AAC
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_AAC_Encoder.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_AAC_Encoder_x64.ax"

# Multiplexeur MP4
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_MP4_Muxer.ax"
regsvr32 "C:\Program Files\VisioForge\Encoding Filters\VisioForge_MP4_Muxer_x64.ax"
```

**Virtual Camera SDK** :
```batch
# Pilote de camera virtuelle
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Virtual_Camera.ax"
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Virtual_Camera_x64.ax"

# Filtre Push Source
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Push_Video_Source.ax"
regsvr32 "C:\Program Files\VisioForge\Virtual Camera\VisioForge_Push_Video_Source_x64.ax"
```

---
### Méthode 3 : enregistrement par programmation (C++)
Enregistrez les filtres par programmation depuis le code de votre application.
#### Utilisation de LoadLibrary et DllRegisterServer
```cpp
#include <windows.h>
#include <iostream>
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
HRESULT RegisterFilter(const wchar_t* filterPath)
{
    HMODULE hModule = LoadLibraryW(filterPath);
    if (!hModule)
    {
        DWORD error = GetLastError();
        std::wcerr << L"Failed to load filter: " << filterPath << std::endl;
        std::wcerr << L"Error code: " << error << std::endl;
        return HRESULT_FROM_WIN32(error);
    }
    LPFNDLLREGISTERSERVER pfnDllRegisterServer =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");
    if (!pfnDllRegisterServer)
    {
        FreeLibrary(hModule);
        return E_FAIL;
    }
    HRESULT hr = pfnDllRegisterServer();
    FreeLibrary(hModule);
    if (SUCCEEDED(hr))
    {
        std::wcout << L"Filter registered successfully: " << filterPath << std::endl;
    }
    else
    {
        std::wcerr << L"Registration failed with HRESULT: " << std::hex << hr << std::endl;
    }
    return hr;
}
HRESULT UnregisterFilter(const wchar_t* filterPath)
{
    HMODULE hModule = LoadLibraryW(filterPath);
    if (!hModule)
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    typedef HRESULT (STDAPICALLTYPE *LPFNDLLUNREGISTERSERVER)();
    LPFNDLLUNREGISTERSERVER pfnDllUnregisterServer =
        (LPFNDLLUNREGISTERSERVER)GetProcAddress(hModule, "DllUnregisterServer");
    if (!pfnDllUnregisterServer)
    {
        FreeLibrary(hModule);
        return E_FAIL;
    }
    HRESULT hr = pfnDllUnregisterServer();
    FreeLibrary(hModule);
    return hr;
}
// Utilisation
int main()
{
    const wchar_t* filterPath = L"C:\\Program Files\\VisioForge\\FFMPEG Source\\VisioForge_FFMPEG_Source_x64.ax";
    HRESULT hr = RegisterFilter(filterPath);
    if (SUCCEEDED(hr))
    {
        std::cout << "Filter registered successfully!" << std::endl;
    }
    else
    {
        std::cout << "Failed to register filter" << std::endl;
    }
    return 0;
}
```
#### Utilisation de l'utilitaire reg_special.exe
Les SDK VisioForge incluent un utilitaire `reg_special.exe` pour simplifier l'enregistrement :
```cpp
#include <windows.h>
#include <shellapi.h>
HRESULT RegisterWithUtility(const wchar_t* filterPath)
{
    // Construire la ligne de commande
    wchar_t cmdLine[MAX_PATH * 2];
    swprintf_s(cmdLine, L"reg_special.exe /regserver \"%s\"", filterPath);
    // Executer l'utilitaire d'enregistrement
    SHELLEXECUTEINFO sei = { sizeof(sei) };
    sei.lpVerb = L"runas";  // Executer en tant qu'administrateur
    sei.lpFile = L"reg_special.exe";
    sei.lpParameters = cmdLine;
    sei.nShow = SW_HIDE;
    sei.fMask = SEE_MASK_NOCLOSEPROCESS;
    if (!ShellExecuteEx(&sei))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }
    // Attendre la fin
    WaitForSingleObject(sei.hProcess, INFINITE);
    DWORD exitCode;
    GetExitCodeProcess(sei.hProcess, &exitCode);
    CloseHandle(sei.hProcess);
    return (exitCode == 0) ? S_OK : E_FAIL;
}
```
---

### Méthode 4 : enregistrement par programmation (.NET/C#)

Enregistrez les filtres depuis des applications .NET via P/Invoke.

```csharp
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

public class FilterRegistration
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int DllRegisterServerDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int DllUnregisterServerDelegate();

    public static void RegisterFilter(string filterPath)
    {
        IntPtr hModule = LoadLibrary(filterPath);
        if (hModule == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(),
                $"Failed to load filter: {filterPath}");
        }

        try
        {
            IntPtr procAddress = GetProcAddress(hModule, "DllRegisterServer");
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception("DllRegisterServer function not found");
            }

            DllRegisterServerDelegate registerServer =
                Marshal.GetDelegateForFunctionPointer<DllRegisterServerDelegate>(procAddress);

            int result = registerServer();

            if (result != 0)
            {
                throw new COMException($"Registration failed with HRESULT: 0x{result:X8}");
            }

            Console.WriteLine($"Filter registered successfully: {filterPath}");
        }
        finally
        {
            FreeLibrary(hModule);
        }
    }

    public static void UnregisterFilter(string filterPath)
    {
        IntPtr hModule = LoadLibrary(filterPath);
        if (hModule == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            IntPtr procAddress = GetProcAddress(hModule, "DllUnregisterServer");
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception("DllUnregisterServer function not found");
            }

            DllUnregisterServerDelegate unregisterServer =
                Marshal.GetDelegateForFunctionPointer<DllUnregisterServerDelegate>(procAddress);

            int result = unregisterServer();

            if (result != 0)
            {
                throw new COMException($"Unregistration failed with HRESULT: 0x{result:X8}");
            }

            Console.WriteLine($"Filter unregistered successfully: {filterPath}");
        }
        finally
        {
            FreeLibrary(hModule);
        }
    }

    // Alternative : utiliser Process.Start avec regsvr32
    public static void RegisterFilterWithRegsvr32(string filterPath)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "regsvr32.exe",
            Arguments = $"/s \"{filterPath}\"",  // /s = silencieux
            Verb = "runas",  // Executer en tant qu'administrateur
            UseShellExecute = true,
            CreateNoWindow = true
        };

        using (var process = System.Diagnostics.Process.Start(startInfo))
        {
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"regsvr32 failed with exit code: {process.ExitCode}");
            }
        }
    }
}

// Exemple d'utilisation
class Program
{
    static void Main(string[] args)
    {
        string filterPath = @"C:\Program Files\VisioForge\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax";

        try
        {
            FilterRegistration.RegisterFilter(filterPath);
            Console.WriteLine("Filter registered successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

---
## Vérification de l'enregistrement
### Méthode 1 : avec GraphEdit/GraphStudioNext
1. Lancer GraphEdit (Windows SDK) ou GraphStudioNext
2. Cliquer sur « Graph » → « Insert Filters »
3. Rechercher le nom du filtre (par exemple, « FFMPEG Source », « VLC Source »)
4. Si le filtre apparaît dans la liste, l'enregistrement a réussi
### Méthode 2 : avec l'éditeur de registre
```batch
# Ouvrir l'editeur de registre
regedit
# Naviguer vers :
HKEY_CLASSES_ROOT\CLSID\{GUID}
# Exemple pour FFMPEG Source :
# HKEY_CLASSES_ROOT\CLSID\{1974D893-83E4-4F89-9908-795C524CC17E}
```
### Méthode 3 : vérification par programmation (C++)
```cpp
#include <dshow.h>
bool IsFilterRegistered(const CLSID& filterClsid)
{
    IBaseFilter* pFilter = nullptr;
    HRESULT hr = CoCreateInstance(filterClsid, NULL, CLSCTX_INPROC_SERVER,
        IID_IBaseFilter, (void**)&pFilter);
    if (SUCCEEDED(hr) && pFilter)
    {
        pFilter->Release();
        return true;
    }
    return false;
}
// Utilisation
int main()
{
    CoInitialize(NULL);
    // CLSID du FFMPEG Source Filter
    CLSID ffmpegSourceClsid =
        { 0x1974D893, 0x83E4, 0x4F89, { 0x99, 0x08, 0x79, 0x5C, 0x52, 0x4C, 0xC1, 0x7E } };
    if (IsFilterRegistered(ffmpegSourceClsid))
    {
        std::cout << "FFMPEG Source filter is registered" << std::endl;
    }
    else
    {
        std::cout << "FFMPEG Source filter is NOT registered" << std::endl;
    }
    CoUninitialize();
    return 0;
}
```
### Méthode 4 : vérification par programmation (.NET/C#)
```csharp
using System;
using System.Runtime.InteropServices;
public static bool IsFilterRegistered(Guid clsid)
{
    try
    {
        Type comType = Type.GetTypeFromCLSID(clsid, throwOnError: false);
        if (comType == null)
            return false;
        object instance = Activator.CreateInstance(comType);
        if (instance != null)
        {
            Marshal.ReleaseComObject(instance);
            return true;
        }
    }
    catch
    {
        return false;
    }
    return false;
}
// Utilisation
Guid ffmpegSourceClsid = new Guid("1974D893-83E4-4F89-9908-795C524CC17E");
if (IsFilterRegistered(ffmpegSourceClsid))
{
    Console.WriteLine("FFMPEG Source filter is registered");
}
```
---

## Dépannage { #troubleshooting }

### Problème : « DllRegisterServer failed » ou « Error 0x80004005 »

**Causes** :
- Pas exécuté en tant qu'administrateur
- Dépendances manquantes (DLL)
- Mauvaise architecture (x86 vs x64)

**Solutions** :

1. **Exécuter en tant qu'administrateur** :
   ```batch
   # Clic droit Invite de commandes -> Executer en tant qu'administrateur
   regsvr32 "C:\Path\To\Filter.ax"
   ```

2. **Vérifier les dépendances** :
   Utilisez Dependency Walker ou Dependencies.exe pour repérer les DLL manquantes :
   ```batch
   # Telecharger Dependencies depuis : https://github.com/lucasg/Dependencies
   Dependencies.exe "C:\Path\To\Filter.ax"
   ```

3. **Vérifier l'architecture** :
   ```batch
   # Pour une application 32 bits, enregistrer un filtre 32 bits
   regsvr32 "C:\Path\To\Filter.ax"

   # Pour une application 64 bits, enregistrer un filtre 64 bits
   regsvr32 "C:\Path\To\Filter_x64.ax"
   ```

### Problème : « The module was loaded but the entry-point was not found »

**Cause** : le fichier n'est pas un filtre DirectShow valide ou il est corrompu.

**Solutions** :
- Vérifier l'intégrité du fichier
- Re-télécharger ou réinstaller le SDK
- Vérifier qu'il s'agit bien d'un filtre DirectShow (extension .ax)

### Problème : filtre enregistré mais introuvable dans les applications

**Causes** :
- Incompatibilité 32 bits/64 bits
- Filtre enregistré dans la mauvaise HKEY (par utilisateur vs système)

**Solutions** :

1. **Faire correspondre l'architecture de l'application** :
   - Une appli 32 bits a besoin d'un filtre 32 bits
   - Une appli 64 bits a besoin d'un filtre 64 bits

2. **Enregistrement à l'échelle du système** :
   ```batch
   # Executer l'invite de commande en tant qu'administrateur
   # Cela enregistre a l'echelle du systeme (HKEY_LOCAL_MACHINE)
   regsvr32 "C:\Path\To\Filter.ax"
   ```

3. **Vérifier les deux registres** :
   - `HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID`
   - `HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID`

### Problème : accès refusé

**Cause** : permissions insuffisantes.

**Solution** :
```batch
# Toujours executer en tant qu'administrateur pour l'enregistrement de filtres
# Clic droit Invite de commandes -> Executer en tant qu'administrateur
```

### Problème : enregistrement réussi mais le filtre ne fonctionne pas

**Causes** :
- Clé de licence manquante
- Dépendances d'exécution manquantes
- Chemin d'installation incorrect

**Solutions** :

1. **Vérifier la licence** :
   - Vérifier que la licence d'essai n'est pas expirée
   - S'assurer que la clé de licence est correctement activée

2. **Vérifier les dépendances d'exécution** :
   - FFMPEG Source : nécessite les DLL FFmpeg (avcodec, avformat, etc.)
   - VLC Source : nécessite les bibliothèques VLC (libvlc.dll, libvlccore.dll, plugins/)
   - NVENC : nécessite un GPU NVIDIA et ses pilotes
   - Processing/Encoding : peut nécessiter les redistribuables Visual C++

3. **Vérifier les emplacements des fichiers** :
   Toutes les DLL dépendantes doivent se trouver dans le même répertoire que le fichier .ax, ou dans le PATH système.

---
## COM sans enregistrement (avancé)
Pour un déploiement xcopy sans enregistrement, utilisez le COM sans enregistrement avec des fichiers manifeste.
### Création du fichier manifeste
**filter.manifest** (à placer à côté du fichier .ax) :
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
  <assemblyIdentity
    type="win32"
    name="VisioForge.FFMPEGSource"
    version="1.0.0.0"/>
  <file name="VisioForge_FFMPEG_Source_x64.ax">
    <comClass
      clsid="{1974D893-83E4-4F89-9908-795C524CC17E}"
      threadingModel="Both"/>
  </file>
</assembly>
```
**application.exe.manifest** (à placer à côté de votre .exe) :
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
  <assemblyIdentity
    type="win32"
    name="YourApplication"
    version="1.0.0.0"/>
  <dependency>
    <dependentAssembly>
      <assemblyIdentity
        type="win32"
        name="VisioForge.FFMPEGSource"
        version="1.0.0.0"/>
    </dependentAssembly>
  </dependency>
</assembly>
```
**Limites** :
- Plus complexe à mettre en place
- Nécessite des fichiers manifeste
- Peut ne pas fonctionner avec tous les filtres DirectShow
- Les filtres enregistrés au niveau système ont la priorité
---

## Scripts batch d'enregistrement

### Enregistrer tous les filtres (script batch)

```batch
@echo off
echo Enregistrement des filtres DirectShow VisioForge...
echo.

REM Verifier les privileges administrateur
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERREUR : ce script doit etre execute en tant qu'administrateur !
    pause
    exit /b 1
)

REM Definir le chemin d'installation
set INSTALL_PATH=C:\Program Files\VisioForge

REM Enregistrer FFMPEG Source
echo Enregistrement de FFMPEG Source...
regsvr32 /s "%INSTALL_PATH%\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% equ 0 (
    echo   [OK] FFMPEG Source enregistre
) else (
    echo   [ECHEC] Enregistrement de FFMPEG Source echoue
)

REM Enregistrer VLC Source
echo Enregistrement de VLC Source...
regsvr32 /s "%INSTALL_PATH%\VLC Source\VisioForge_VLC_Source.ax"
if %errorLevel% equ 0 (
    echo   [OK] VLC Source enregistre
) else (
    echo   [ECHEC] Enregistrement de VLC Source echoue
)

REM Enregistrer les filtres de traitement
echo Enregistrement des filtres de traitement...
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Mixer_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Processing Filters\VisioForge_Audio_Enhancer_x64.ax"
echo   [OK] Filtres de traitement enregistres

REM Enregistrer les filtres d'encodage
echo Enregistrement des filtres d'encodage...
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_NVENC_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_H264_Encoder_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_AAC_Encoder_x64.ax"
regsvr32 /s "%INSTALL_PATH%\Encoding Filters\VisioForge_MP4_Muxer_x64.ax"
echo   [OK] Filtres d'encodage enregistres

echo.
echo Enregistrement termine !
pause
```

### Désenregistrer tous les filtres

```batch
@echo off
echo Desenregistrement des filtres DirectShow VisioForge...
echo.

REM Verifier les privileges administrateur
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERREUR : ce script doit etre execute en tant qu'administrateur !
    pause
    exit /b 1
)

set INSTALL_PATH=C:\Program Files\VisioForge

REM Desenregistrer tous les filtres
regsvr32 /s /u "%INSTALL_PATH%\FFMPEG Source\VisioForge_FFMPEG_Source_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\VLC Source\VisioForge_VLC_Source.ax"
regsvr32 /s /u "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Effects_Pro_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\Processing Filters\VisioForge_Video_Mixer_x64.ax"
regsvr32 /s /u "%INSTALL_PATH%\Encoding Filters\VisioForge_NVENC_x64.ax"

echo Desenregistrement termine !
pause
```

---
## Voir aussi
- [Fichiers redistribuables](redistributable-files.md) — liste complète des fichiers pour chaque SDK
- [Intégration avec l'installeur](installer-integration.md) — création d'installeurs personnalisés
- [Vue d'ensemble du déploiement](index.md) — guide principal de déploiement
