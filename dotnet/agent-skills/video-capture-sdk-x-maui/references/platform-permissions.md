# Platform permissions cheat-sheet

Drop-in reference for the per-OS permission paperwork required by Video Capture SDK X on MAUI for camera + microphone capture and video-file output. Without these, the OS either kills the app on first hardware access (iOS / Mac Catalyst) or blocks the request silently (Android runtime denial). Same paperwork as Media Blocks SDK on MAUI — the SDKs share their per-OS native runtime, so the OS-level surface is identical.

## Android — `Platforms/Android/AndroidManifest.xml`

Declare each `<uses-permission>` inside `<manifest>` (NOT inside `<application>`):

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
  <application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true" />

  <uses-permission android:name="android.permission.CAMERA" />
  <uses-permission android:name="android.permission.RECORD_AUDIO" />
  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

  <!-- Only needed for shared/public storage on API <= 29.
       On API 30+ prefer scoped storage (GetExternalFilesDir + MediaStore) and drop these. -->
  <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

  <uses-feature android:name="android.hardware.camera" android:required="false" />
  <uses-feature android:name="android.hardware.camera.autofocus" />
</manifest>
```

Manifest declarations alone are not enough on Android 6+ — runtime request before opening the camera:

```csharp
await Permissions.RequestAsync<Permissions.Camera>();
await Permissions.RequestAsync<Permissions.Microphone>();
// API 33+: use Permissions.Media for gallery save; API 30-32: scoped storage covers app-private dirs.
```

If denied, `Permissions.ShouldShowRationale<>` indicates whether to explain why before re-prompting (see `MainPage.xaml.cs`).

## iOS — `Platforms/iOS/Info.plist`

iOS terminates the app on first hardware access without these keys. Strings must be non-empty and human-readable; App Store review reads them:

```xml
<key>NSCameraUsageDescription</key>
<string>Camera access is required to record video.</string>
<key>NSMicrophoneUsageDescription</key>
<string>Microphone access is required to record audio.</string>

<!-- Only if you save recorded videos to the Photos library: -->
<key>NSPhotoLibraryUsageDescription</key>
<string>Photo library access is required to save recorded videos.</string>
<key>NSPhotoLibraryAddUsageDescription</key>
<string>Saves recorded videos to the Photos library.</string>
```

Runtime request: same MAUI Essentials API as Android. For Photos library access on iOS, call `Photos.PHPhotoLibrary.RequestAuthorization(...)` directly — there's no MAUI Essentials wrapper for it (see `MainPage.xaml.cs` `RequestPhotoPermission`).

## Mac Catalyst — Info.plist + Entitlements.plist

Mac Catalyst inherits the iOS sandbox model PLUS the macOS hardened-runtime entitlement model — both are required.

`Platforms/MacCatalyst/Info.plist` — same keys as iOS:

```xml
<key>NSCameraUsageDescription</key>
<string>Camera access is required to record video.</string>
<key>NSMicrophoneUsageDescription</key>
<string>Microphone access is required to record audio.</string>
```

`Platforms/MacCatalyst/Entitlements.plist` — without these keys, the hardened runtime blocks the device class regardless of what Info.plist says:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>com.apple.security.device.camera</key>
    <true/>
    <key>com.apple.security.device.audio-input</key>
    <true/>
    <!-- USB capture devices (e.g. external webcams over USB-C): -->
    <key>com.apple.security.device.usb</key>
    <true/>
    <!-- If you save outside the sandbox container via NSOpenPanel: -->
    <key>com.apple.security.files.user-selected.read-write</key>
    <true/>
</dict>
</plist>
```

## Windows

No project-level permission paperwork. Camera/microphone access is gated by the system Settings privacy switches, enforced by Windows itself; if the user has denied access globally, `DeviceEnumerator.Shared.VideoSourcesAsync()` returns an empty list. Surface that to the user with an explanatory message in your UI.
