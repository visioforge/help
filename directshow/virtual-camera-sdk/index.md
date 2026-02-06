---
title: DirectShow Virtual Camera SDK for Video Streaming
description: Create virtual webcams for Zoom, Teams, Skype, and OBS with DirectShow SDK for streaming any video source with audio support.
sidebar_label: Virtual Camera SDK
order: 6
---

# DirectShow Virtual Camera SDK

## Overview

Our robust DirectShow-based Virtual Camera SDK enables developers to implement powerful virtual camera functionality in their applications. The SDK provides sink filters that can be utilized as output in Video Capture SDK or Video Edit SDK environments, while the source filters can be employed as video sources for various capture applications.

With this versatile toolkit, you can stream video content from virtually any source directly to a virtual camera device. These virtual devices are fully compatible with popular communication platforms such as `Skype`, `Zoom`, `Microsoft Teams`, web browsers, and numerous other applications that support DirectShow virtual cameras. The SDK also includes comprehensive audio streaming capabilities for complete multimedia solutions.

To help you get started quickly, the SDK package includes a fully-functional sample application that demonstrates how to stream video content from files to virtual camera devices.

Download the SDK from our [product page](https://www.visioforge.com/virtual-camera-sdk) to start integrating virtual camera functionality into your applications today.

---

## Installation

Before using the code samples and integrating the SDK into your application, you must first install the Virtual Camera SDK from the [product page](https://www.visioforge.com/virtual-camera-sdk).

**Installation Steps**:

1. Download the SDK installer from the product page
2. Run the installer with administrative privileges
3. The installer will register the virtual camera driver and all necessary DirectShow filters
4. Sample applications and source code will be available in the installation directory

**Note**: The virtual camera driver and filters must be properly registered on the system before they can be used in your applications. The installer handles this automatically.

---

## Key Features and Capabilities

* **Multiple Source Support**: Stream video to virtual camera from files, network streams, or capture devices
* **Architecture Compatibility**: Full x86/x64 architecture support
* **High-Resolution Support**: Stream video content up to 4K resolution
* **Customization Options**: Define and implement custom camera names
* **SDK Integration**: Seamless integration with other development tools
* **Audio Support**: Complete audio streaming capabilities
* **Professional Applications**: Perfect for teleconferencing, streaming, and professional video applications

## Technical Implementation

### Sample DirectShow Graph Architecture

The diagram below illustrates the standard DirectShow graph implementation when using the Virtual Camera SDK:

![Sample DirectShow graph](demo.webp)

### License Registration via Registry

You can register the filter with your valid license key using the Windows registry system.

Configure licensing using the following registry key:

```reg
HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\License
```

Set your purchased license key as a string value in this registry location.

### Deployment Guidelines

For proper deployment, copy and COM-register the SDK DirectShow filters - these are the files in the `Redist` folder with .ax extension. Registration can be performed using `regsvr32.exe` or through COM registration in your application installer. Please note that administrative privileges are required for successful registration.

### No-Signal Application Configuration

You can configure an application to run automatically when the virtual camera is not connected to any video source.

Configure the no-signal application using this registry key:

```reg
HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\StartupEXE
```

Set the executable file name as a string value.

### No-Signal Image Configuration

Instead of displaying a black screen when no video source is available, you can configure a custom image to be shown.

Configure the no-signal image using this registry key:

```reg
HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\BackgroundImage
```

Set the image file path as a string value.

## Interface Reference

### IVFVirtualCameraSink Interface

The `IVFVirtualCameraSink` interface is used to configure the virtual camera sink filter, primarily for license registration.

**Interface GUID**: `{A96631D2-4AC9-4F09-9F34-FF8229087DEB}`

**Inherits From**: `IUnknown`

#### C# Definition

```csharp
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Virtual camera sink interface for license configuration.
/// </summary>
[ComImport]
[System.Security.SuppressUnmanagedCodeSecurity]
[Guid("A96631D2-4AC9-4F09-9F34-FF8229087DEB")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFVirtualCameraSink
{
    /// <summary>
    /// Sets the license key for the virtual camera sink filter.
    /// </summary>
    /// <param name="license">License key string ("TRIAL" for trial version)</param>
    /// <returns>HRESULT (0 for success)</returns>
    [PreserveSig]
    int set_license([MarshalAs(UnmanagedType.LPWStr)] string license);
}
```

**Usage Example (C#)**:

```csharp
// Add Virtual Camera Sink filter
var sinkFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraph,
    Consts.CLSID_VFVirtualCameraSink,
    "VisioForge Virtual Camera Sink");

// Set license
var sinkIntf = sinkFilter as IVFVirtualCameraSink;
if (sinkIntf != null)
{
    sinkIntf.set_license("YOUR-LICENSE-KEY"); // or "TRIAL" for trial version
}
```

#### C++ Definition

```cpp
#include <unknwn.h>

// {A96631D2-4AC9-4F09-9F34-FF8229087DEB}
DEFINE_GUID(IID_IVFVirtualCameraSink,
    0xa96631d2, 0x4ac9, 0x4f09, 0x9f, 0x34, 0xff, 0x82, 0x29, 0x8, 0x7d, 0xeb);

/// <summary>
/// Virtual camera sink interface for license configuration.
/// </summary>
DECLARE_INTERFACE_(IVFVirtualCameraSink, IUnknown)
{
    /// <summary>
    /// Sets the license key for the virtual camera sink filter.
    /// </summary>
    /// <param name="license">License key wide string (L"TRIAL" for trial version)</param>
    /// <returns>HRESULT (S_OK for success)</returns>
    STDMETHOD(set_license) (THIS_
        LPCWSTR license
        ) PURE;
};
```

**Usage Example (C++)**:

```cpp
IBaseFilter* pSinkFilter = nullptr;
IVFVirtualCameraSink* pSinkIntf = nullptr;

// Create Virtual Camera Sink filter
hr = CoCreateInstance(CLSID_VFVirtualCameraSink, NULL, CLSCTX_INPROC_SERVER,
                     IID_IBaseFilter, (void**)&pSinkFilter);

if (SUCCEEDED(hr))
{
    // Query interface
    hr = pSinkFilter->QueryInterface(IID_IVFVirtualCameraSink, (void**)&pSinkIntf);
    if (SUCCEEDED(hr))
    {
        // Set license
        hr = pSinkIntf->set_license(L"YOUR-LICENSE-KEY"); // or L"TRIAL"
        pSinkIntf->Release();
    }
}
```

#### Delphi Definition

```delphi
uses
  ActiveX, ComObj;

const
  IID_IVFVirtualCameraSink: TGUID = '{A96631D2-4AC9-4F09-9F34-FF8229087DEB}';

type
  /// <summary>
  /// Virtual camera sink interface for license configuration.
  /// </summary>
  IVFVirtualCameraSink = interface(IUnknown)
    ['{A96631D2-4AC9-4F09-9F34-FF8229087DEB}']

    /// <summary>
    /// Sets the license key for the virtual camera sink filter.
    /// </summary>
    /// <param name="license">License key wide string ('TRIAL' for trial version)</param>
    /// <returns>HRESULT (S_OK for success)</returns>
    function set_license(license: PWideChar): HRESULT; stdcall;
  end;
```

**Usage Example (Delphi)**:

```delphi
var
  SinkFilter: IBaseFilter;
  SinkIntf: IVFVirtualCameraSink;
begin
  // Create Virtual Camera Sink filter
  if Succeeded(CoCreateInstance(CLSID_VFVirtualCameraSink, nil,
                                CLSCTX_INPROC_SERVER,
                                IID_IBaseFilter, SinkFilter)) then
  begin
    // Query interface
    if Succeeded(SinkFilter.QueryInterface(IID_IVFVirtualCameraSink, SinkIntf)) then
    begin
      // Set license
      SinkIntf.set_license('YOUR-LICENSE-KEY'); // or 'TRIAL'
      SinkIntf := nil;
    end;
  end;
end;
```

---

## Third-Party Libraries and Integration

The Virtual Camera SDK contains third-party components that are used in the demo applications. These components are not required for the core SDK functionality.

The Delphi and .NET demonstration applications utilize third-party libraries to simplify DirectShow development. The C++ demo applications are built without external dependencies.

### .NET Integration

.NET applications leverage [DirectShowLib.Net (LGPL)](https://sourceforge.net/projects/directshownet/) to implement DirectShow functionality in managed code environments.

Developers can create console applications, WinForms, or WPF applications using .NET. The included demo applications utilize WinForms for the user interface.

### Delphi Integration

Delphi applications use [DSPack (MPL)](https://code.google.com/archive/p/dspack/) to implement DirectShow functionality. While modern Delphi versions include built-in DirectShow support, DSPack is utilized in the demo applications to maintain compatibility with older Delphi versions.

### C++ Integration

The C++ demo applications do not require third-party libraries and are built using the standard DirectShow SDK (part of Windows SDK).

Developers can utilize MFC, ATL, or other C++ frameworks to build their applications. The included demo applications are built with MFC.

## System Requirements

The SDK is compatible with the following Microsoft Windows operating systems:

* Windows 7, 8, 8.1, 10, and 11
* Windows Server 2008, 2012, 2016, 2019, and 2022

## Version History and Updates

### Version 14.0

* Performance optimizations and enhancements
* Improved Windows 11 compatibility
* Enhanced support for modern web browsers
* Minor updates and bug fixes

### Version 12.0

* Windows 10 support improvements
* Performance enhancements
* 8K resolution support added
* Improved Mozilla Firefox and Microsoft Edge compatibility
* Various minor updates

### Version 11.0

* Critical bug fixes implemented
* Updated Google Chrome compatibility
* Resolved audio clicks issues in various web browsers and applications

### Version 10.0

* High frame rate support added
* Significant performance improvements
* Minor updates and bug fixes

### Version 9.0

* 4K video resolution support added
* Updated support for contemporary web browsers
* Various minor updates and improvements

### Version 8.0

* Added background image functionality for no-signal scenarios
* Implemented application auto-run for no-signal conditions
* Enhanced Skype compatibility

### Version 7.1

* Audio streaming support via virtual audio output and virtual microphone input
* PCM audio format support with customizable sample rates and channel configuration
* Bug fixes and performance improvements
* Additional video resolutions added

### Version 7.0

* Initial release as a standalone product
* Previously included in Video Edit SDK and Video Capture SDK
* Compatible with any DirectShow application

## Additional Resources

* [End User License Agreement](../../eula.md)
