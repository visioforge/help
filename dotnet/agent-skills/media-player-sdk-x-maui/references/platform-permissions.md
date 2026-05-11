# Platform permissions cheat-sheet

Drop-in reference for the per-OS permission paperwork required by Media Player SDK X on MAUI for *playback* of local files and network streams. Playback is a much smaller surface than capture: no camera/microphone keys are needed unless your app *also* records. The two things that bite cross-platform are **network access on Android** and the **App Transport Security relaxation** that iOS / Mac Catalyst need to play HTTP (non-HTTPS) streams. Same per-OS native runtime as Video Capture X / Media Blocks — those skills' permission docs are a strict superset.

## Android — `Platforms/Android/AndroidManifest.xml`

For a player that opens local files and streams from the network, declare INTERNET + network-state. Storage permissions only matter if you let the user pick files from shared storage on older API levels (API 32 and below); API 33+ uses scoped storage / `READ_MEDIA_VIDEO` and the MAUI `FilePicker` handles the user-flow:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
  <application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true" />

  <!-- Required for HTTP/HTTPS/RTSP/RTMP streams -->
  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

  <!-- Only needed for shared/public storage on API <= 29.
       On API 30+ prefer scoped storage (FilePicker + MediaStore) and drop these. -->
  <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

  <!-- API 33+ for picking videos from the gallery: -->
  <uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
  <uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />
</manifest>
```

If your `<application>` is targeting API 28+ and you need to play **plain HTTP** streams (not HTTPS / RTSP / RTMP), opt the app into cleartext traffic — Android otherwise blocks the request silently:

```xml
<application
    android:usesCleartextTraffic="true"
    ...>
```

Or scope it with a `network-security-config.xml` to specific hostnames in production. No runtime permission request is needed for INTERNET — it's a normal install-time permission.

## iOS — `Platforms/iOS/Info.plist`

Pure playback needs no usage-description keys. The thing that surprises people is **App Transport Security**: iOS blocks `http://` (non-HTTPS) streams at the network stack by default, regardless of the player. To allow plain HTTP playback during development or for known internal hosts, relax ATS:

```xml
<!-- Allow HTTP streams from any host (development / on-prem only — App Store
     review will push back unless you scope it to specific domains): -->
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>

<!-- Production-friendly alternative: scope the exemption to specific hosts -->
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSExceptionDomains</key>
    <dict>
        <key>example.local</key>
        <dict>
            <key>NSExceptionAllowsInsecureHTTPLoads</key><true/>
        </dict>
    </dict>
</dict>
```

If you also pick files via `FilePicker.Default.PickAsync()` from the Photos library, add:

```xml
<key>NSPhotoLibraryUsageDescription</key>
<string>Photo library access is required to select videos for playback.</string>
```

No microphone / camera keys are needed for a player.

## Mac Catalyst — Info.plist + Entitlements.plist

`Platforms/MacCatalyst/Info.plist` — same ATS rules as iOS (above) for HTTP streams.

`Platforms/MacCatalyst/Entitlements.plist` — Mac Catalyst is sandboxed; for playback you need outgoing network and (if you want to open files outside the app sandbox via `FilePicker`) user-selected file read access:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <!-- Outgoing network for streaming -->
    <key>com.apple.security.network.client</key>
    <true/>
    <!-- Open files chosen via NSOpenPanel / FilePicker -->
    <key>com.apple.security.files.user-selected.read-only</key>
    <true/>
</dict>
</plist>
```

If you do not enable `com.apple.security.network.client`, network streams fail with a generic connect error and no useful exception.

## Windows

No project-level permission paperwork for playback. The OS does not gate file open or HTTP/HTTPS / RTSP / RTMP outbound. Firewall prompts may appear on first run for low-level protocols (RTSP / RTMP) — this is a runtime user dialog, not something declared in the project.
