# Platform permissions cheat-sheet

Per-OS paperwork for Video Edit SDK X on MAUI. An editor does not touch the camera or the
microphone, so the surface is much smaller than the capture SDK's: the only permission you
normally need is read access to the media the user picks. Without it, iOS and Mac Catalyst
terminate the app the first time the picker opens, and Android denies the pick silently.

Write the rendered output to a per-app directory (`FileSystem.Current.AppDataDirectory`,
`FileSystem.Current.CacheDirectory`). Those need no permission on any platform. Only writing to
the shared gallery/media store does.

## Android — `Platforms/Android/AndroidManifest.xml`

Declare each `<uses-permission>` inside `<manifest>` (NOT inside `<application>`):

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
  <application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true" />

  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

  <!-- Reading clips the user picks. API 32 and below. -->
  <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                   android:maxSdkVersion="32" />
  <!-- API 33+ replaces the blanket storage permission with a per-type one. -->
  <uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
</manifest>
```

`MediaPicker.Default.PickVideoAsync()` goes through the system photo picker, which on API 33+
grants access to the single chosen item without a runtime prompt. Request explicitly only if you
enumerate the media store yourself:

```csharp
await Permissions.RequestAsync<Permissions.Media>();
```

`MediaPicker` copies the chosen asset into a cache path, so `FileResult.FullPath` is absolute and
directly usable by `Input_AddAudioVideoFile`.

## iOS — `Platforms/iOS/Info.plist`

The key must be present with a non-empty, user-facing string. iOS terminates the process on the
first photo-library access without it, with no exception and no crash log from your code; App
Store review also rejects builds that are missing it.

```xml
<key>NSPhotoLibraryUsageDescription</key>
<string>Access to the photo library is used to pick the clips to edit.</string>
```

Add `NSPhotoLibraryAddUsageDescription` as well if you save the rendered file back to the gallery.

`PHPicker` hands MAUI an `NSItemProvider`, and only the original file name survives in
`FileResult.FullPath` — it is not a filesystem path. Copy the stream into your own cache before
handing it to `Input_AddAudioVideoFile`:

```csharp
var cachePath = Path.Combine(FileSystem.Current.CacheDirectory,
    $"{Path.GetFileNameWithoutExtension(picked.FileName)}_{Guid.NewGuid():N}{Path.GetExtension(picked.FileName)}");

using (var src = await picked.OpenReadAsync())
using (var dst = File.Create(cachePath))
{
    await src.CopyToAsync(dst);
}
```

## Mac Catalyst — `Platforms/MacCatalyst/Info.plist`

Same `NSPhotoLibraryUsageDescription` key as iOS. A sandboxed Mac Catalyst app that reads files
the user chose through a picker needs no extra entitlement — the picker grants the scope. If you
read paths the user did not pick through a picker, add to `Entitlements.plist`:

```xml
<key>com.apple.security.files.user-selected.read-write</key><true/>
```

## Windows

No permission paperwork. The file picker grants access to whatever the user chooses.
