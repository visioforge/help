---
title: Déployer les filtres DirectShow avec WiX, NSIS et Inno Setup
description: Intégration d'installeur pour les filtres DirectShow VisioForge — exemples WiX, NSIS, Inno Setup et InstallShield avec enregistrement COM.
tags:
  - DirectShow
  - C++
  - Windows

---

# Guide d'intégration avec l'installeur

## Vue d'ensemble

Ce guide fournit des instructions complètes pour intégrer les filtres DirectShow VisioForge dans des installeurs Windows. Il couvre plusieurs technologies d'installeur, les actions personnalisées d'enregistrement de filtres, la gestion des dépendances et les bonnes pratiques.

---
## Prérequis
Avant de créer un installeur, assurez-vous de comprendre :
- [Fichiers redistribuables](redistributable-files.md) — fichiers à inclure dans l'installeur
- [Enregistrement des filtres](filter-registration.md) — mécanismes d'enregistrement
- Architecture de la plateforme cible (x86/x64)
- Exigences relatives au redistribuable Visual C++
---

## Vue d'ensemble des technologies d'installeur

### WiX Toolset

**Cas idéal** : applications d'entreprise, déploiements basés sur MSI, automatisation IT

**Avantages** :

- Syntaxe déclarative basée sur XML
- Prise en charge native de MSI
- Excellente intégration à Windows Installer
- Prise en charge du déploiement par stratégies de groupe
- Développement actif et communauté

**Exigences** :

- WiX Toolset 3.x ou 4.x
- Intégration Visual Studio (optionnelle)
- Fichiers de projet .wixproj

[Voir les exemples WiX →](#wix-toolset-examples)

---
### NSIS (Nullsoft Scriptable Install System)
**Cas idéal** : installeurs légers, interface utilisateur personnalisée, applications portables
**Avantages** :
- Taille d'installeur réduite
- Très personnalisable
- Langage de script simple
- Aucune dépendance d'exécution
- Exécution rapide
**Exigences** :
- Compilateur NSIS 3.x
- Fichiers de script .nsi
[Voir les exemples NSIS →](#nsis-examples)
---

### InstallShield

**Cas idéal** : applications commerciales, installations complexes, fonctionnalités avancées

**Avantages** :

- Concepteur graphique professionnel
- Détection intégrée des prérequis
- Prise en charge multiplateforme
- Création de suites/bundles
- Intégration Visual Studio

**Exigences** :

- InstallShield Limited Edition (Visual Studio) ou Professional
- Fichiers de projet .ism

[Voir le guide InstallShield →](#installshield-integration)

---
### Inno Setup
**Cas idéal** : installeurs simples, petites applications, freeware
**Avantages** :
- Gratuit et open source
- Prise en charge des scripts Pascal
- Prise en charge Unicode
- Bonne documentation
- Communauté active
**Exigences** :
- Compilateur Inno Setup 6.x
- Fichiers de script .iss
[Voir les exemples Inno Setup →](#inno-setup-examples)
---

## Exemples WiX Toolset { #wix-toolset-examples }

### Installation de filtre de base

Créez un installeur WiX complet pour un filtre DirectShow avec enregistrement automatique.

#### Product.wxs

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*"
           Name="MyApp with DirectShow Filters"
           Language="1033"
           Version="1.0.0.0"
           Manufacturer="Your Company"
           UpgradeCode="YOUR-GUID-HERE">

    <Package InstallerVersion="200"
             Compressed="yes"
             InstallScope="perMachine"
             Platform="x64" />

    <MajorUpgrade DowngradeErrorMessage="A newer version is already installed." />

    <MediaTemplate EmbedCab="yes" />

    <!-- Structure des repertoires d'installation -->
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFiles64Folder">
        <Directory Id="INSTALLFOLDER" Name="MyApp">
          <Directory Id="FilterFolder" Name="Filters" />
        </Directory>
      </Directory>
    </Directory>

    <!-- Definition de la fonctionnalite -->
    <Feature Id="ProductFeature" Title="MyApp" Level="1">
      <ComponentGroupRef Id="FilterComponents" />
      <ComponentGroupRef Id="ApplicationComponents" />
    </Feature>

    <!-- Actions personnalisees pour l'enregistrement -->
    <CustomAction Id="RegisterFilters"
                  Directory="FilterFolder"
                  ExeCommand="cmd.exe /c regsvr32 /s VisioForge_FFMPEG_Source_x64.ax"
                  Execute="deferred"
                  Impersonate="no"
                  Return="check" />

    <CustomAction Id="UnregisterFilters"
                  Directory="FilterFolder"
                  ExeCommand="cmd.exe /c regsvr32 /s /u VisioForge_FFMPEG_Source_x64.ax"
                  Execute="deferred"
                  Impersonate="no"
                  Return="ignore" />

    <InstallExecuteSequence>
      <Custom Action="RegisterFilters" After="InstallFiles">NOT Installed</Custom>
      <Custom Action="UnregisterFilters" Before="RemoveFiles">Installed</Custom>
    </InstallExecuteSequence>

  </Product>
</Wix>
```

#### Filters.wxs (définition des composants)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>
    <ComponentGroup Id="FilterComponents" Directory="FilterFolder">

      <!-- Filtre source FFMPEG -->
      <Component Id="FFMPEGSourceFilter" Guid="YOUR-GUID-1">
        <File Id="FFMPEGSourceAX"
              Source="$(var.SourceDir)\VisioForge_FFMPEG_Source_x64.ax"
              KeyPath="yes" />
      </Component>

      <!-- Dependances FFmpeg -->
      <Component Id="FFMPEGLibraries" Guid="YOUR-GUID-2">
        <File Id="avcodec58" Source="$(var.SourceDir)\avcodec-58.dll" />
        <File Id="avdevice58" Source="$(var.SourceDir)\avdevice-58.dll" />
        <File Id="avfilter7" Source="$(var.SourceDir)\avfilter-7.dll" />
        <File Id="avformat58" Source="$(var.SourceDir)\avformat-58.dll" />
        <File Id="avutil56" Source="$(var.SourceDir)\avutil-56.dll" />
        <File Id="swresample3" Source="$(var.SourceDir)\swresample-3.dll" />
        <File Id="swscale5" Source="$(var.SourceDir)\swscale-5.dll" />
      </Component>

    </ComponentGroup>
  </Fragment>
</Wix>
```

#### VCRedist.wxs (vérification des prérequis)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>

    <!-- Detecter le redistribuable Visual C++ 2015-2022 -->
    <Property Id="VCREDIST2022_X64">
      <RegistrySearch Id="VCRedist2022x64"
                      Root="HKLM"
                      Key="SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64"
                      Name="Installed"
                      Type="raw" />
    </Property>

    <Condition Message="This application requires Visual C++ 2015-2022 Redistributable (x64). Please install it from https://aka.ms/vs/17/release/vc_redist.x64.exe">
      <![CDATA[Installed OR VCREDIST2022_X64]]>
    </Condition>

  </Fragment>
</Wix>
```

#### Construction de l'installeur WiX

```bash
# En ligne de commande WiX 3.x
candle.exe Product.wxs Filters.wxs VCRedist.wxs -ext WixUIExtension
light.exe -out MyApp.msi Product.wixobj Filters.wixobj VCRedist.wixobj -ext WixUIExtension

# Avec WiX 4.x (nouvelle syntaxe)
wix build Product.wxs Filters.wxs VCRedist.wxs -ext WixToolset.UI.wixext -out MyApp.msi
```

---
### WiX avancé : bundle auto-extractible { #advanced-wix-self-extracting-bundle }
Créez un bundle qui inclut le redistribuable Visual C++.
#### Bundle.wxs
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi"
     xmlns:bal="http://schemas.microsoft.com/wix/BalExtension"
     xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">
  <Bundle Name="MyApp Complete Setup"
          Version="1.0.0.0"
          Manufacturer="Your Company"
          UpgradeCode="YOUR-BUNDLE-GUID">
    <BootstrapperApplicationRef Id="WixStandardBootstrapperApplication.RtfLicense">
      <bal:WixStandardBootstrapperApplication
        LicenseFile="License.rtf"
        LogoFile="Logo.png" />
    </BootstrapperApplicationRef>
    <Chain>
      <!-- Installer d'abord le redistribuable VC++ -->
      <PackageGroupRef Id="VCRedist2022x64" />
      <!-- Puis installer l'application principale -->
      <MsiPackage SourceFile="MyApp.msi"
                  DisplayName="MyApp"
                  Vital="yes" />
    </Chain>
  </Bundle>
  <!-- Groupe de paquets du redistribuable VC++ -->
  <Fragment>
    <PackageGroup Id="VCRedist2022x64">
      <ExePackage Id="VCRedist2022x64"
                  Cache="no"
                  Compressed="yes"
                  PerMachine="yes"
                  Permanent="yes"
                  Vital="yes"
                  SourceFile="VC_redist.x64.exe"
                  InstallCommand="/install /quiet /norestart"
                  DetectCondition="VCREDIST2022_X64"
                  InstallCondition="NOT VCREDIST2022_X64" />
    </PackageGroup>
  </Fragment>
</Wix>
```
Construire le bundle :
```bash
# WiX 3.x
candle.exe Bundle.wxs -ext WixBalExtension
light.exe -out MyAppSetup.exe Bundle.wixobj -ext WixBalExtension
# WiX 4.x
wix build Bundle.wxs -ext WixToolset.Bal.wixext -out MyAppSetup.exe
```
---

### WiX : DLL C++ personnalisée pour l'enregistrement

Pour plus de contrôle, créez une DLL d'action personnalisée.

#### CustomActions.cpp

```cpp
#include <windows.h>
#include <msiquery.h>
#include <strsafe.h>

#pragma comment(lib, "msi.lib")

// Declarations anticipees
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
typedef HRESULT (STDAPICALLTYPE *LPFNDLLUNREGISTERSERVER)();

// Fonction utilitaire pour ecrire dans le journal MSI
void LogMessage(MSIHANDLE hInstall, LPCTSTR message)
{
    PMSIHANDLE hRecord = MsiCreateRecord(1);
    MsiRecordSetString(hRecord, 0, message);
    MsiProcessMessage(hInstall, INSTALLMESSAGE_INFO, hRecord);
}

// Action personnalisee : enregistrer les filtres DirectShow
extern "C" __declspec(dllexport) UINT __stdcall RegisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR installDir[MAX_PATH];
    DWORD installDirSize = MAX_PATH;

    // Recuperer la propriete INSTALLFOLDER
    if (MsiGetProperty(hInstall, TEXT("INSTALLFOLDER"), installDir, &installDirSize) != ERROR_SUCCESS)
    {
        LogMessage(hInstall, TEXT("Failed to get INSTALLFOLDER property"));
        return ERROR_INSTALL_FAILURE;
    }

    LogMessage(hInstall, TEXT("Registering DirectShow filters..."));

    // Construire le chemin du filtre
    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, installDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("Filters\\VisioForge_FFMPEG_Source_x64.ax"));

    // Charger la DLL du filtre
    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        TCHAR errorMsg[512];
        StringCchPrintf(errorMsg, 512, TEXT("Failed to load filter: %s (Error: %d)"),
                       filterPath, GetLastError());
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }

    // Recuperer la fonction DllRegisterServer
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");

    if (!pfnRegister)
    {
        LogMessage(hInstall, TEXT("DllRegisterServer not found in filter"));
        FreeLibrary(hModule);
        return ERROR_INSTALL_FAILURE;
    }

    // Enregistrer le filtre
    HRESULT hr = pfnRegister();
    FreeLibrary(hModule);

    if (SUCCEEDED(hr))
    {
        LogMessage(hInstall, TEXT("DirectShow filters registered successfully"));
        return ERROR_SUCCESS;
    }
    else
    {
        TCHAR errorMsg[256];
        StringCchPrintf(errorMsg, 256, TEXT("Filter registration failed: HRESULT 0x%08X"), hr);
        LogMessage(hInstall, errorMsg);
        return ERROR_INSTALL_FAILURE;
    }
}

// Action personnalisee : desenregistrer les filtres DirectShow
extern "C" __declspec(dllexport) UINT __stdcall UnregisterDirectShowFilters(MSIHANDLE hInstall)
{
    TCHAR installDir[MAX_PATH];
    DWORD installDirSize = MAX_PATH;

    if (MsiGetProperty(hInstall, TEXT("INSTALLFOLDER"), installDir, &installDirSize) != ERROR_SUCCESS)
    {
        // Ne pas faire echouer la desinstallation si on ne peut pas recuperer le chemin
        return ERROR_SUCCESS;
    }

    LogMessage(hInstall, TEXT("Unregistering DirectShow filters..."));

    TCHAR filterPath[MAX_PATH];
    StringCchCopy(filterPath, MAX_PATH, installDir);
    StringCchCat(filterPath, MAX_PATH, TEXT("Filters\\VisioForge_FFMPEG_Source_x64.ax"));

    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        // Le filtre est peut-etre deja supprime, ne pas echouer
        return ERROR_SUCCESS;
    }

    LPFNDLLUNREGISTERSERVER pfnUnregister =
        (LPFNDLLUNREGISTERSERVER)GetProcAddress(hModule, "DllUnregisterServer");

    if (pfnUnregister)
    {
        pfnUnregister();
    }

    FreeLibrary(hModule);
    LogMessage(hInstall, TEXT("DirectShow filters unregistered"));

    return ERROR_SUCCESS;
}

// Point d'entree de la DLL
BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID reserved)
{
    return TRUE;
}
```

#### CustomActions.wxs

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Fragment>

    <!-- Binaire pour les actions personnalisees -->
    <Binary Id="CustomActionsDLL" SourceFile="$(var.CustomActions.TargetPath)" />

    <!-- Definir les actions personnalisees -->
    <CustomAction Id="RegisterFiltersCA"
                  BinaryKey="CustomActionsDLL"
                  DllEntry="RegisterDirectShowFilters"
                  Execute="deferred"
                  Impersonate="no"
                  Return="check" />

    <CustomAction Id="UnregisterFiltersCA"
                  BinaryKey="CustomActionsDLL"
                  DllEntry="UnregisterDirectShowFilters"
                  Execute="deferred"
                  Impersonate="no"
                  Return="ignore" />

    <!-- Planifier les actions personnalisees -->
    <InstallExecuteSequence>
      <Custom Action="RegisterFiltersCA" After="InstallFiles">
        NOT Installed
      </Custom>
      <Custom Action="UnregisterFiltersCA" Before="RemoveFiles">
        Installed
      </Custom>
    </InstallExecuteSequence>

  </Fragment>
</Wix>
```

---
## Exemples NSIS { #nsis-examples }
### Installeur NSIS de base
Créez un script d'installeur NSIS complet.
#### Installer.nsi
```nsis
; Installeur MyApp avec filtres DirectShow
; Script NSIS 3.x
;--------------------------------
; Inclusions
!include "MUI2.nsh"
!include "x64.nsh"
;--------------------------------
; General
Name "MyApp"
OutFile "MyAppSetup.exe"
Unicode True
; Dossier d'installation par defaut
InstallDir "$PROGRAMFILES64\MyApp"
; Recuperer le dossier d'installation depuis le registre si disponible
InstallDirRegKey HKLM "Software\MyApp" "InstallDir"
; Demander les privileges
RequestExecutionLevel admin
;--------------------------------
; Parametres d'interface
!define MUI_ABORTWARNING
!define MUI_ICON "installer.ico"
!define MUI_UNICON "uninstaller.ico"
;--------------------------------
; Pages
!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;--------------------------------
; Langues
!insertmacro MUI_LANGUAGE "English"
;--------------------------------
; Informations de version
VIProductVersion "1.0.0.0"
VIAddVersionKey "ProductName" "MyApp"
VIAddVersionKey "CompanyName" "Your Company"
VIAddVersionKey "FileDescription" "MyApp Installer"
VIAddVersionKey "FileVersion" "1.0.0.0"
;--------------------------------
; Sections d'installation
Section "MyApp (required)" SecMain
  SectionIn RO
  ; Definir le chemin de sortie
  SetOutPath "$INSTDIR"
  ; Installer les fichiers de l'application
  File "MyApp.exe"
  File "MyApp.exe.config"
  ; Creer le sous-dossier Filters
  CreateDirectory "$INSTDIR\Filters"
  SetOutPath "$INSTDIR\Filters"
  ; Installer le filtre source FFMPEG
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
  File "Filters\avcodec-58.dll"
  File "Filters\avdevice-58.dll"
  File "Filters\avfilter-7.dll"
  File "Filters\avformat-58.dll"
  File "Filters\avutil-56.dll"
  File "Filters\swresample-3.dll"
  File "Filters\swscale-5.dll"
  ; Enregistrer le filtre DirectShow
  DetailPrint "Registering DirectShow filters..."
  ExecWait 'regsvr32 /s "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"' $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "Filter registration failed. Code: $0"
  ${EndIf}
  ; Memoriser le dossier d'installation
  WriteRegStr HKLM "Software\MyApp" "InstallDir" $INSTDIR
  ; Creer le desinstalleur
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  ; Creer les raccourcis du menu Demarrer
  CreateDirectory "$SMPROGRAMS\MyApp"
  CreateShortcut "$SMPROGRAMS\MyApp\MyApp.lnk" "$INSTDIR\MyApp.exe"
  CreateShortcut "$SMPROGRAMS\MyApp\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  ; Entree dans Ajouter/Supprimer des programmes
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayName" "MyApp"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayIcon" "$INSTDIR\MyApp.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "Publisher" "Your Company"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "DisplayVersion" "1.0.0.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp" "NoRepair" 1
SectionEnd
;--------------------------------
; Sections optionnelles
Section "VLC Source Filter" SecVLC
  SetOutPath "$INSTDIR\Filters"
  ; Installer le filtre VLC Source
  File "Filters\VisioForge_VLC_Source.ax"
  File "Filters\libvlc.dll"
  File "Filters\libvlccore.dll"
  ; Installer le repertoire de plugins VLC
  SetOutPath "$INSTDIR\Filters\plugins"
  File /r "Filters\plugins\*.*"
  ; Enregistrer le filtre VLC Source
  DetailPrint "Registering VLC Source filter..."
  ExecWait 'regsvr32 /s "$INSTDIR\Filters\VisioForge_VLC_Source.ax"'
SectionEnd
;--------------------------------
; Descriptions des sections
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecMain} "Main application files and FFMPEG Source filter (required)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecVLC} "VLC Source filter for additional format support (optional)"
!insertmacro MUI_FUNCTION_DESCRIPTION_END
;--------------------------------
; Fonctions de l'installeur
Function .onInit
  ; Verifier si Windows 64 bits
  ${If} ${RunningX64}
    ; OK
  ${Else}
    MessageBox MB_OK|MB_ICONSTOP "This application requires 64-bit Windows."
    Abort
  ${EndIf}
  ; Verifier le redistribuable Visual C++ 2015-2022
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64" "Installed"
  ${If} $0 != 1
    MessageBox MB_YESNO|MB_ICONQUESTION "Visual C++ 2015-2022 Redistributable (x64) is required.$\n$\nDownload and install now?" IDYES download IDNO skip
    download:
      ExecShell "open" "https://aka.ms/vs/17/release/vc_redist.x64.exe"
      Abort
    skip:
  ${EndIf}
FunctionEnd
;--------------------------------
; Section de desinstallation
Section "Uninstall"
  ; Desenregistrer les filtres
  DetailPrint "Unregistering DirectShow filters..."
  ExecWait 'regsvr32 /s /u "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"'
  ExecWait 'regsvr32 /s /u "$INSTDIR\Filters\VisioForge_VLC_Source.ax"'
  ; Supprimer les fichiers
  Delete "$INSTDIR\MyApp.exe"
  Delete "$INSTDIR\MyApp.exe.config"
  Delete "$INSTDIR\Uninstall.exe"
  ; Supprimer le repertoire Filters
  Delete "$INSTDIR\Filters\*.ax"
  Delete "$INSTDIR\Filters\*.dll"
  RMDir /r "$INSTDIR\Filters\plugins"
  RMDir "$INSTDIR\Filters"
  ; Supprimer le repertoire d'installation
  RMDir "$INSTDIR"
  ; Supprimer les raccourcis du menu Demarrer
  Delete "$SMPROGRAMS\MyApp\MyApp.lnk"
  Delete "$SMPROGRAMS\MyApp\Uninstall.lnk"
  RMDir "$SMPROGRAMS\MyApp"
  ; Supprimer les cles de registre
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\MyApp"
  DeleteRegKey HKLM "Software\MyApp"
SectionEnd
```
#### Construction de l'installeur NSIS
```bash
# Compiler avec NSIS
makensis.exe Installer.nsi
# Ou utiliser le compilateur NSIS en interface graphique
# File > Load Script > Select Installer.nsi > Test Installer
```
---

### NSIS : prise en charge de l'installation silencieuse

Ajoutez des paramètres d'installation silencieuse.

```nsis
; A ajouter a la fonction .onInit

; Verifier le mode silencieux
${GetParameters} $R0
${GetOptions} $R0 "/S" $0
${IfNot} ${Errors}
  ; Mode silencieux - ignorer les verifications de prerequis
  Goto silent_mode
${EndIf}

; Verifications normales ici...

silent_mode:
  ; Continuer l'installation

; Pour la desinstallation silencieuse, ajouter au desinstalleur :
; Lancer avec : Uninstall.exe /S
```

---
### NSIS : plugin personnalisé pour l'enregistrement
Créez un plugin NSIS personnalisé pour plus de contrôle.
#### FilterRegistration.cpp (plugin NSIS)
```cpp
#include <windows.h>
#include "pluginapi.h"
typedef HRESULT (STDAPICALLTYPE *LPFNDLLREGISTERSERVER)();
// Fonction d'enregistrement de filtre
extern "C" void __declspec(dllexport) RegisterFilter(
    HWND hwndParent,
    int string_size,
    TCHAR *variables,
    stack_t **stacktop,
    extra_parameters *extra)
{
    EXDLL_INIT();
    // Recuperer le chemin du filtre depuis la pile
    TCHAR filterPath[MAX_PATH];
    popstring(filterPath);
    // Charger la DLL
    HMODULE hModule = LoadLibrary(filterPath);
    if (!hModule)
    {
        pushstring(_T("ERROR"));
        return;
    }
    // Recuperer la fonction d'enregistrement
    LPFNDLLREGISTERSERVER pfnRegister =
        (LPFNDLLREGISTERSERVER)GetProcAddress(hModule, "DllRegisterServer");
    if (!pfnRegister)
    {
        FreeLibrary(hModule);
        pushstring(_T("ERROR"));
        return;
    }
    // Enregistrer
    HRESULT hr = pfnRegister();
    FreeLibrary(hModule);
    pushstring(SUCCEEDED(hr) ? _T("OK") : _T("ERROR"));
}
BOOL WINAPI DllMain(HANDLE hInst, ULONG ul_reason_for_call, LPVOID lpReserved)
{
    return TRUE;
}
```
Utilisation dans un script NSIS :
```nsis
; Charger le plugin
FilterRegistration::RegisterFilter "$INSTDIR\Filters\VisioForge_FFMPEG_Source_x64.ax"
Pop $0
${If} $0 == "ERROR"
    MessageBox MB_OK "Filter registration failed"
${EndIf}
```
---

## Intégration InstallShield { #installshield-integration }

### Configuration de projet InstallShield de base

1. **Créer un nouveau projet** :
   - File > New Project
   - Sélectionner « Basic MSI Project »
   - Définir le nom et l'emplacement du projet

2. **Ajouter les fichiers** :
   - Vue Application Files
   - Ajouter les fichiers de filtres dans `[INSTALLDIR]\Filters`
   - Ajouter les exécutables de l'application

3. **Ajouter une action personnalisée** :

#### Méthode 1 : avec regsvr32

1. Aller dans **Behavior and Logic** > **Custom Actions**
2. Clic droit sur **Install** > **New Custom Action**
3. Définir les propriétés :
   - Name : `Register DirectShow Filters`
   - Type : `Stored in the Directory Table`
   - Working Directory : `[INSTALLDIR]Filters`
   - Filename : `regsvr32.exe`
   - Command Line : `/s VisioForge_FFMPEG_Source_x64.ax`
   - Run : `Deferred Execution in System Context`
   - Condition : `NOT Installed`

4. Pour la désinstallation :
   - Name : `Unregister DirectShow Filters`
   - Command Line : `/s /u VisioForge_FFMPEG_Source_x64.ax`
   - Sequence : avant **RemoveFiles**
   - Condition : `Installed`

#### Méthode 2 : avec une DLL personnalisée

1. Créer une DLL C++ contenant le code d'enregistrement (similaire à l'exemple WiX ci-dessus)
2. Ajouter la DLL dans **Support Files** sous InstallShield
3. Créer une action personnalisée :
   - Type : `DLL from the installation`
   - DLL Name : `CustomActions.dll`
   - Function : `RegisterDirectShowFilters`

### InstallShield : configuration des prérequis

1. Aller dans la vue **Redistributables**
2. Ajouter **Microsoft Visual C++ 2015-2022 Redistributable (x64)** :
   - Clic droit > **Add Prerequisite**
   - Parcourir vers `VC_redist.x64.exe`
   - Définir : **Install Before This Application**

---
## Exemples Inno Setup { #inno-setup-examples }
### Script Inno Setup de base
#### Setup.iss
```pascal
; Script de configuration MyApp pour Inno Setup 6.x
[Setup]
AppName=MyApp
AppVersion=1.0
DefaultDirName={autopf}\MyApp
DefaultGroupName=MyApp
UninstallDisplayIcon={app}\MyApp.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Output
OutputBaseFilename=MyAppSetup
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
MinVersion=10.0
[Files]
; Application principale
Source: "MyApp.exe"; DestDir: "{app}"; Flags: ignoreversion
; Filtre source FFMPEG
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion regserver restartreplace uninsrestartdelete
Source: "Filters\avcodec-58.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avdevice-58.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avfilter-7.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avformat-58.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\avutil-56.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\swresample-3.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
Source: "Filters\swscale-5.dll"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Icons]
Name: "{group}\MyApp"; Filename: "{app}\MyApp.exe"
Name: "{group}\Uninstall MyApp"; Filename: "{uninstallexe}"
[Run]
; Optionnel : lancer l'application apres l'installation
Filename: "{app}\MyApp.exe"; Description: "Launch MyApp"; Flags: nowait postinstall skipifsilent
[Registry]
Root: HKLM; Subkey: "Software\MyApp"; ValueType: string; ValueName: "InstallDir"; ValueData: "{app}"; Flags: uninsdeletekey
[Code]
// Verifier le redistribuable Visual C++
function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
  VCInstalled: Cardinal;
begin
  Result := True;
  // Verifier si VC++ 2015-2022 est installe
  if not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64',
                            'Installed', VCInstalled) or (VCInstalled <> 1) then
  begin
    if MsgBox('Visual C++ 2015-2022 Redistributable (x64) is required.' + #13#10 +
              'Download and install now?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://aka.ms/vs/17/release/vc_redist.x64.exe', '', '', SW_SHOW, ewNoWait, ResultCode);
      Result := False;  // Annuler l'installation
    end;
  end;
end;
```
#### Inno Setup avancé : enregistrement personnalisé
```pascal
[Files]
; Ne pas utiliser le flag regserver - on enregistrera manuellement
Source: "Filters\VisioForge_FFMPEG_Source_x64.ax"; DestDir: "{app}\Filters"; Flags: ignoreversion
[Code]
// Importer les fonctions de l'API Windows
function LoadLibrary(lpFileName: String): THandle;
  external 'LoadLibraryW@kernel32.dll stdcall';
function FreeLibrary(hModule: THandle): Boolean;
  external 'FreeLibrary@kernel32.dll stdcall';
function GetProcAddress(hModule: THandle; lpProcName: AnsiString): Longword;
  external 'GetProcAddress@kernel32.dll stdcall';
type
  TDllRegisterServer = function: HRESULT;
// Enregistrer le filtre DirectShow
function RegisterDirectShowFilter(FilterPath: String): Boolean;
var
  hModule: THandle;
  DllRegisterServer: TDllRegisterServer;
  RegisterFunc: Longword;
  hr: HRESULT;
begin
  Result := False;
  hModule := LoadLibrary(FilterPath);
  if hModule = 0 then
  begin
    Log('Failed to load filter: ' + FilterPath);
    Exit;
  end;
  try
    RegisterFunc := GetProcAddress(hModule, 'DllRegisterServer');
    if RegisterFunc = 0 then
    begin
      Log('DllRegisterServer not found');
      Exit;
    end;
    @DllRegisterServer := Pointer(RegisterFunc);
    hr := DllRegisterServer();
    Result := Succeeded(hr);
    if Result then
      Log('Filter registered successfully')
    else
      Log('Filter registration failed: ' + IntToHex(hr, 8));
  finally
    FreeLibrary(hModule);
  end;
end;
// Appele apres l'installation
procedure CurStepChanged(CurStep: TSetupStep);
var
  FilterPath: String;
begin
  if CurStep = ssPostInstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    if not RegisterDirectShowFilter(FilterPath) then
    begin
      MsgBox('Warning: DirectShow filter registration failed.' + #13#10 +
             'You may need to register it manually.', mbError, MB_OK);
    end;
  end;
end;
// Appele avant la desinstallation
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
  FilterPath: String;
begin
  if CurUninstallStep = usUninstall then
  begin
    FilterPath := ExpandConstant('{app}\Filters\VisioForge_FFMPEG_Source_x64.ax');
    // Desenregistrer avec regsvr32
    Exec('regsvr32.exe', '/s /u "' + FilterPath + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
```
---

## Installation silencieuse

### Paramètres d'installation silencieuse

#### MSI (WiX, InstallShield MSI)

```bash
# Installation silencieuse
msiexec /i MyApp.msi /quiet /norestart

# Installation silencieuse avec journal
msiexec /i MyApp.msi /quiet /norestart /l*v install.log

# Desinstallation silencieuse
msiexec /x MyApp.msi /quiet /norestart

# Installation silencieuse avec repertoire personnalise
msiexec /i MyApp.msi /quiet INSTALLFOLDER="C:\CustomPath\MyApp"
```

#### NSIS

```bash
# Installation silencieuse
MyAppSetup.exe /S

# Installation silencieuse avec repertoire personnalise
MyAppSetup.exe /S /D=C:\CustomPath\MyApp

# Desinstallation silencieuse
Uninstall.exe /S
```

#### Inno Setup

```bash
# Installation silencieuse
MyAppSetup.exe /SILENT

# Tres silencieuse (sans barre de progression)
MyAppSetup.exe /VERYSILENT

# Silencieuse avec repertoire personnalise
MyAppSetup.exe /SILENT /DIR="C:\CustomPath\MyApp"

# Desinstallation silencieuse
unins000.exe /SILENT
```

---
## Inclusion des dépendances { #bundling-dependencies }
### Redistribuable Visual C++
#### Option 1 : Bootstrapper de téléchargement
```xml
<!-- WiX Bundle.wxs -->
<ExePackage Id="VCRedist2022"
            DownloadUrl="https://aka.ms/vs/17/release/vc_redist.x64.exe"
            InstallCommand="/install /quiet /norestart"
            DetectCondition="VCREDIST2022_X64" />
```
#### Option 2 : inclure le redistribuable
```nsis
; NSIS
Section "VC++ Redistributable"
  File "Prerequisites\VC_redist.x64.exe"
  ExecWait '"$INSTDIR\VC_redist.x64.exe" /install /quiet /norestart'
  Delete "$INSTDIR\VC_redist.x64.exe"
SectionEnd
```
#### Option 3 : Merge Modules (WiX)
```xml
<DirectoryRef Id="TARGETDIR">
  <Merge Id="VCRedist" SourceFile="$(var.VCRedistMergeModule)" DiskId="1" Language="0"/>
</DirectoryRef>
<Feature Id="VCRedist" Title="Visual C++ Runtime" AllowAdvertise="no" Display="hidden" Level="1">
  <MergeRef Id="VCRedist"/>
</Feature>
```
---

## Bonnes pratiques

### Moment de l'enregistrement

1. **Séquence d'installation** :

   ```
   InstallFiles
   ↓
   Enregistrer les filtres (action personnalisee)
   ↓
   InstallFinalize
   ```

2. **Séquence de désinstallation** :

   ```
   Desenregistrer les filtres (action personnalisee)
   ↓
   RemoveFiles
   ↓
   UninstallFinalize
   ```

### Gestion des erreurs

**Toujours** :

- Journaliser les tentatives d'enregistrement
- Vérifier les valeurs HRESULT
- Fournir un retour à l'utilisateur en cas d'échec
- Ne pas faire échouer toute l'installation si l'enregistrement échoue
- Permettre l'enregistrement manuel après installation

**Exemple de gestion d'erreurs** :

```cpp
HRESULT hr = RegisterFilter(filterPath);
if (FAILED(hr))
{
    if (hr == REGDB_E_CLASSNOTREG)
        LogError("Class not registered - check dependencies");
    else if (hr == E_ACCESSDENIED)
        LogError("Access denied - requires admin privileges");
    else
        LogError("Registration failed with HRESULT: 0x%08X", hr);
}
```

### Prise en charge du rollback

Assurez un rollback correct si l'installation échoue :

```xml
<!-- Exemple de rollback WiX -->
<CustomAction Id="RegisterFiltersRollback"
              Directory="FilterFolder"
              ExeCommand="regsvr32 /s /u VisioForge_FFMPEG_Source_x64.ax"
              Execute="rollback"
              Impersonate="no" />

<InstallExecuteSequence>
  <Custom Action="RegisterFiltersRollback" Before="RegisterFiltersCA">
    NOT Installed
  </Custom>
  <Custom Action="RegisterFiltersCA" After="InstallFiles">
    NOT Installed
  </Custom>
</InstallExecuteSequence>
```

### Privilèges administrateur

**Toujours exiger** des privilèges administrateur/élevés :

```xml
<!-- WiX -->
<Package InstallScope="perMachine" InstallPrivileges="elevated" />
```

```nsis
; NSIS
RequestExecutionLevel admin
```

```pascal
{ Inno Setup }
PrivilegesRequired=admin
```

### Considérations d'architecture

```xml
<!-- WiX : paquets distincts pour x86/x64 -->
<Product Platform="x64">
  <!-- Contenu x64 -->
</Product>

<Product Platform="x86">
  <!-- Contenu x86 -->
</Product>
```

```nsis
; NSIS : detection d'architecture a l'execution
${If} ${RunningX64}
  File "Filters\VisioForge_FFMPEG_Source_x64.ax"
${Else}
  File "Filters\VisioForge_FFMPEG_Source.ax"
${EndIf}
```

---
## Tests d'installation
### Liste de vérification des tests manuels
- [ ] Installer sur Windows 10/11 vierge
- [ ] Vérifier que tous les fichiers sont copiés
- [ ] Vérifier l'enregistrement du filtre (GraphEdit/GraphStudioNext)
- [ ] Tester le fonctionnement de l'application
- [ ] Désinstaller complètement
- [ ] Vérifier qu'aucun fichier ne reste
- [ ] Vérifier le nettoyage du registre
- [ ] Tester un scénario de mise à niveau
- [ ] Tester la fonctionnalité de réparation
- [ ] Tester l'installation silencieuse
- [ ] Tester avec différents comptes utilisateur
### Tests automatisés
```powershell
# Script de test PowerShell
$installerPath = ".\MyAppSetup.msi"
$logPath = ".\install_test.log"
# Installer silencieusement
Start-Process msiexec.exe -ArgumentList "/i `"$installerPath`" /quiet /l*v `"$logPath`"" -Wait
# Verifier si le filtre est enregistre
$filterCLSID = "{1974D893-83E4-4F89-9908-795C524CC17E}"
$regPath = "HKLM:\SOFTWARE\Classes\CLSID\$filterCLSID"
if (Test-Path $regPath) {
    Write-Host "Filtre enregistre avec succes" -ForegroundColor Green
} else {
    Write-Host "Echec de l'enregistrement du filtre" -ForegroundColor Red
    Exit 1
}
# Desinstaller
Start-Process msiexec.exe -ArgumentList "/x `"$installerPath`" /quiet" -Wait
# Verifier le nettoyage
if (Test-Path $regPath) {
    Write-Host "Filtre non desenregistre" -ForegroundColor Red
    Exit 1
} else {
    Write-Host "Desinstallation reussie" -ForegroundColor Green
}
```
---

## Dépannage

### Problèmes courants

#### L'enregistrement échoue avec « Access Denied »

**Cause** : privilèges insuffisants

**Solution** :

```xml
<!-- Garantir l'execution differee en contexte systeme -->
<CustomAction Execute="deferred" Impersonate="no" />
```

#### Le filtre fonctionne en développement mais pas après l'installation

**Cause** : dépendances manquantes ou chemins incorrects

**Solution** :

- Utiliser Dependency Walker pour vérifier toutes les dépendances DLL
- S'assurer que toutes les DLL se trouvent dans le même répertoire que le filtre
- Vérifier la variable d'environnement PATH

#### L'installation silencieuse se bloque

**Cause** : interaction utilisateur requise

**Solution** :

```bash
# Ajouter le parametre /norestart
msiexec /i MyApp.msi /quiet /norestart
```

#### La désinstallation laisse des entrées de registre

**Cause** : l'action personnalisée de désenregistrement ne s'exécute pas

**Solution** :

```xml
<!-- Definir Return="ignore" pour le desenregistrement -->
<CustomAction Return="ignore" />
```

---
## Voir aussi
### Documentation
- [Enregistrement des filtres](filter-registration.md) — méthodes d'enregistrement manuelles
- [Fichiers redistribuables](redistributable-files.md) — fichiers à inclure dans l'installeur
- [Vue d'ensemble du déploiement](index.md) — guide complet de déploiement
### Ressources externes
- [Documentation WiX Toolset](https://docs.firegiant.com/wix/)
- [Documentation NSIS](https://nsis.sourceforge.io/Docs/)
- [Documentation Inno Setup](https://jrsoftware.org/ishelp/)
- [Documentation Windows Installer (MSI)](https://learn.microsoft.com/en-us/windows/win32/Msi/windows-installer-portal)
