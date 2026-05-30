---
title: Build a Unity Game with Video Playback for macOS (Universal)
description: Build settings, dylib layout, code-signing, and troubleshooting for the VisioForge Media Blocks SDK .NET in Unity 6 on macOS Standalone.
sidebar_label: Build for macOS
order: 55
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - macOS
  - Standalone Player
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Build for macOS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The macOS flavor ships a Universal (arm64 + x86_64) GStreamer runtime plus the package's managed
assemblies built against `netstandard2.1`. The native runtime is a set of ~300 separate dylibs —
core GStreamer libs, plugins, GIO modules, OpenSSL TLS backend, CA bundle — flat in
`Assets/Plugins/macOS/`. This page covers the Build Profile settings, the dylib layout, and the
macOS-specific issues; for the bring-up sequence see [Bootstrap and lifecycle](bootstrap.md).

The macOS flavor is bundled into the `.unitypackage` when the build pipeline is run with
`-IncludeMacOS`. The result is one cumulative package that holds Windows-x64, Android, and macOS
together — Unity picks the right files per Build Target through the per-file `PluginImporter`
metadata.

## Player Settings

| Setting | Value | Where |
|---|---|---|
| Target Platform | **macOS** | File → Build Profiles → macOS |
| Architecture | **Intel 64-bit + Apple silicon** (Universal) | File → Build Profiles → macOS |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **Mono** *or* **IL2CPP** *(both are tested)* | Project Settings → Player → Other Settings → Configuration |
| Mac App Store Validation | **Off** *(or sign the GStreamer dylibs first, see below)* | Project Settings → Player → Other Settings |
| Enter Play Mode → Reload Domain | **Off** | Project Settings → Editor → Enter Play Mode Settings |

Both scripting backends are tested. Mono is the default and is faster to iterate on; switch to
IL2CPP only if you have a project-wide reason. The same `link.xml` that the package ships
preserves the SDK's managed types on either backend.

## On-disk layout

The `deploy-unity-natives.ps1 -Platform macOS` step stages the macOS runtime as:

| Path | Contents |
|---|---|
| `Assets/Plugins/macOS/libgstreamer-1.0.0.dylib` | Core GStreamer library. |
| `Assets/Plugins/macOS/libgio-2.0.0.dylib`, `libglib-2.0.0.dylib`, `libgobject-2.0.0.dylib` | GLib core. |
| `Assets/Plugins/macOS/libgst*.dylib` | Every GStreamer plugin (decoders, encoders, sources, sinks, base elements). |
| `Assets/Plugins/macOS/libgioopenssl.so` | GIO TLS backend that RTSPS / HTTPS verify peers through. |
| `Assets/Plugins/macOS/ca-certificates.crt` | CA bundle for OpenSSL. |
| `Assets/Plugins/macOS/libVisioForge_Core.dylib` | The SDK's native shim. |
| `Assets/VisioForge/link.xml` | IL2CPP preservation rules (shared with Android). |
| `Assets/Plugins/macOS/` | Managed assemblies built against `netstandard2.1` with `UNITY_NS21_MACOS` defined. |

The layout is flat — every dylib's `@rpath` / `@loader_path` is baked in by the NuGet pack, so
once dyld has loaded one via the first `[DllImport]`, the siblings resolve automatically.

## Standalone build

**File → Build Profiles → macOS → Build** produces a `.app` bundle.

Where the natives end up in the bundle depends on the Unity version:

| Layout | Used by | `VisioForgeEnvironment.NativesPath` resolves to |
|---|---|---|
| `<app>.app/Contents/PlugIns/` | Unity 6 default | `…/Contents/PlugIns` |
| `<app>.app/Contents/PlugIns/macOS/` | some Unity 6 patch releases | `…/Contents/PlugIns/macOS` |
| `<app>.app/Contents/Frameworks/` | older Unity layouts | `…/Contents/Frameworks` |
| `<app>.app/Contents/Resources/Data/Plugins/macOS/` | very old layouts | `…/Contents/Resources/Data/Plugins/macOS` |

`NativesPath` probes all four locations at startup, looking for the sentinel
`libgstreamer-1.0.0.dylib`. The first folder that contains it wins, and the result is cached for
the rest of the process — there is no setting you adjust per Unity version.

## Build size

The macOS flavor adds roughly **100 MB** to the `.app` bundle (Universal arm64 + x86_64). It is
the largest of the cumulative-package flavors because every plugin ships as its own dylib and
both architectures are included. If you target Apple silicon only, you can post-process the
bundle to strip the x86_64 slices with `lipo`, but the package itself does not split them by
default.

## Code-signing and notarization

For distribution outside the Mac App Store you typically want to sign and notarize the bundle:

1. **Hardened Runtime** enabled (Project Settings → Player → Other Settings or your signing
   workflow).
2. **Codesign every dylib in `Contents/PlugIns/`** with your Developer ID Application
   certificate before signing the `.app` itself. Unity does not sign third-party plugins for you.
3. **Notarize** the final `.app` (or its `.zip` / `.dmg`) with `xcrun notarytool submit … --wait`.
4. **Staple** the notarization ticket with `xcrun stapler staple <app-bundle>`.

The GStreamer dylibs do not require any Apple entitlements beyond the Hardened Runtime default;
they do not access protected resources from inside their native code. Your own app determines
which entitlements (camera, microphone, network, etc.) are required.

For Mac App Store submission, the bundled `libgioopenssl.so` and `libgmp.10.dylib` are statically
linked and ship under permissive licenses, but the App Store review may flag the file extension
`.so` for a macOS bundle. If you need App Store distribution, contact support — that path is not
exercised by the package CI.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `DllNotFoundException: libgstreamer-1.0.0` on Play | `Plugins/macOS/` is empty or the sentinel `libgstreamer-1.0.0.dylib` is missing. | Re-import the `.unitypackage` with all items checked. The Console shows the resolved `NativesPath` — confirm the sentinel is there. |
| `[VisioForge] Native runtime folder not found at '…/Contents/PlugIns'` on a Standalone build | Plugins were not staged into the `.app` because the macOS flavor was not in the package. | Re-build the package with `-IncludeMacOS` (or import the cumulative `.unitypackage` that includes macOS). |
| Pipeline hangs at startup, log shows two `gst_init` calls | A homebrew or system GStreamer install is on `DYLD_LIBRARY_PATH`. | `Configure()` prunes it — confirm the stripped count is non-zero in the Console. Hardened Runtime strips `DYLD_*` before the process starts, so this is mostly a Mono-Editor concern. |
| RTSPS / HTTPS fails with `Peer certificate cannot be authenticated with given CA certificates` | `ca-certificates.crt` not found at the expected path. | `Configure()` logs a warning if the bundle is missing. Re-import the package or re-run `deploy-unity-natives.ps1 -Platform macOS`. |
| Bundle launched from Finder shows `Damaged app` dialog | The `.app` is unsigned and downloaded with the quarantine flag set. | Sign + notarize for distribution, or for local testing run `xattr -d com.apple.quarantine <app-bundle>` once. |
| Bundle launched from a Mac App Store TestFlight build crashes | `.so` files in the bundle violate App Store layout rules. | Contact support — App Store submission needs an alternate native packaging. |

## Frequently Asked Questions

### Does the macOS flavor work in the Editor on a Mac?

Yes — both `OSXEditor` (the Editor itself) and `OSXPlayer` (a Standalone build) are admitted
runtime targets. `Configure()` resolves `Plugins/macOS/` from the project root in the Editor and
probes the bundle layout in the player.

### Do I need the macOS flavor to open the package in a Mac-host Editor?

Yes. The `.unitypackage` you import must contain the macOS flavor (`-IncludeMacOS`) for the
Mac-host Editor to find a native runtime to load. A Windows-only package, opened in a Mac
Editor, will surface the cross-flavor mismatch as `[VisioForge] Native runtime folder not found
at '…' for runtime platform OSXEditor` — see [Bootstrap and lifecycle](bootstrap.md).

### Can I ship Apple silicon only and skip x86_64?

Yes. After the build, run `lipo -thin arm64 <dylib> -output <dylib>` on each `.dylib` in
`Contents/PlugIns/` to strip the Intel slices. The package does not do this by default because
both architectures are still useful for compatibility testing.

### Does the same package work on iOS too?

The iOS flavor is shipped as a separate platform inside the same `.unitypackage` when built with
`-IncludeIOS`. See [Build for iOS](ios.md).

## See also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — package setup
- [Bootstrap and lifecycle](bootstrap.md) — how `Configure()` finds the macOS runtime
- [Build for iOS](ios.md) — the matching iOS flavor (requires a Mac host)
- [View an RTSP camera in Unity](rtsp-viewer.md) — the `RTSPViewer` sample
- [Troubleshooting](troubleshooting.md) — cross-platform error reference
