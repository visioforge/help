---
title: Media Player Library Updates and Features
description: Comprehensive documentation of media player enhancements, including 4K support, encryption, video effects, streaming capabilities, and performance optimizations. Track the evolution of features from version 3.0 to the latest 10.0 release.
sidebar_label: Changelog
---

# TVFMediaPlayer Library Changelog

This document details the evolution of the TVFMediaPlayer library, chronicling the significant features, enhancements, optimizations, and bug fixes introduced across various versions. It serves as a comprehensive reference for developers tracking the library's progress and understanding the capabilities added over time.

## Version 10.0: Enhanced Media Handling and Customization

Version 10.0 represents a significant step forward, focusing on improved media introspection, logging, customization, and compatibility.

### Core Enhancements

* **Enhanced Media Information Reader:** This version significantly boosts the capabilities of the media information reader. It enables faster, more accurate extraction of metadata from an extensive array of media file types. Developers gain reliable access to critical details like duration, resolution, codec specifics, bitrates, and embedded tags, which streamlines media management and enhances the display capabilities within applications.
* **Improved Logging Capabilities:** Logging has been substantially refined, offering developers more granular control. Configuration options now include distinct log levels (Debug, Info, Warning, Error) and flexible output destinations such as files, the console, or custom endpoints. This facilitates more effective issue diagnosis during development and robust monitoring of application behavior in production, ultimately leading to quicker troubleshooting and increased application stability.
* **Standard Metadata Tag Support:** A cornerstone of this release is the introduction of comprehensive support for reading standard metadata tags embedded within popular video and audio containers. This includes formats like MP4, WMV, MP3, AAC, M4A, and Ogg Vorbis. Applications utilizing TVFMediaPlayer can now seamlessly extract and leverage common tags such as title, artist, album, genre, year, and cover art, thereby enriching the user experience by providing valuable context for the media being played.

### Capture and Effects Improvements

* **Configurable Auto-Split Filenames:** The new `SeparateCapture_Filename_Mask` property provides fine-grained control over filenames when using the auto-split capture feature based on duration or size. This allows for customized naming conventions, improving organization and workflow for segmented recordings.
* **JSON Settings Serialization:** Configuration settings for the media player can now be easily serialized to and deserialized from the widely-used JSON format. This simplifies saving and loading player configurations, enabling persistent settings and easier integration with configuration management systems.
* **Custom Video Effects Pipeline:** Flexibility in video processing is enhanced with the ability to insert custom video effects using third-party filters identified by their CLSID. These filters can be strategically placed either before or after the main effects filter or sample grabber, allowing for sophisticated, tailored video manipulation pipelines.
* **Optimized Video Effects:** Video effects processing has been optimized to take full advantage of the latest generations of Intel CPUs, resulting in smoother playback and lower resource consumption when applying effects.

### Source and Compatibility Fixes

* **MP3 Splitter for Playback Issues:** An MP3 splitter has been integrated to specifically address and resolve playback inconsistencies encountered with certain non-standard or problematic MP3 files, ensuring broader compatibility.
* **Updated VLC Source Filter:** The underlying VLC source filter has been updated to libVLC version 2.2.2.0. This update brings notable improvements, particularly in handling RTMP and HTTPS streams, and resolves previously identified memory leaks, contributing to enhanced stability and broader streaming protocol support.
* **Pan and Blur Effect Fixes:** Specific issues related to the Pan effect in x64 builds and the Blur effect have been addressed and resolved, ensuring consistent visual effect behavior across different architectures.
* **FFMPEG Source Memory Leak Resolved:** A memory leak associated with the FFMPEG source component has been identified and fixed, improving long-term stability and resource management during playback.

## Version 9.2: Engine Updates and Reader Enhancements

This interim release focused on updating core components and further refining the media information capabilities.

* **Updated VLC Engine:** The integrated VLC engine was updated to libVLC version 2.2.1.0, incorporating upstream fixes and improvements from the VLC project for better stability and format compatibility.
* **Enhanced Media Information Reader:** Building upon previous improvements, the media information reader received further enhancements for broader file support and more accurate metadata extraction.
* **Updated FFMPEG Engine:** The FFMPEG engine components were updated, ensuring compatibility with newer codecs and formats while incorporating performance optimizations.

## Version 9.1: Advanced Security Integration

Version 9.1 introduced robust security features through integration with the Video Encryption SDK.

* **Video Encryption SDK v9 Support:** This version added compatibility with the Video Encryption SDK v9. This enables developers to implement strong AES-256 encryption for their video content, using either separate key files or embedded binary data as keys, significantly enhancing content protection capabilities.

## Version 9.0: Audio Enhancements and Logo Flexibility

Version 9.0 brought significant improvements to audio handling and visual branding options.

* **Animated GIF Logo Support:** The capability to use image logos was expanded to include support for animated GIFs, allowing for more dynamic and engaging visual branding within the video playback interface.
* **Audio Enhancements:** A suite of audio enhancement features was introduced, including audio normalization to ensure consistent volume levels, automatic gain control (AGC) to dynamically adjust volume, and manual gain controls for precise audio level adjustments.
* **Percentage-Based Audio Volume:** The API for controlling audio volume was modernized to use a percentage-based system (0-100%), providing a more intuitive and standardized way to manage audio levels compared to previous methods.

## Version 8.6: Decoder Expansion and API Additions

This release focused on expanding codec support, adding flexibility through custom filters, and refining the API.

* **H264 CPU/Intel QuickSync Decoder:** A highly optimized H264 video decoder was added, leveraging both CPU resources and Intel QuickSync hardware acceleration where available. This significantly improves performance for decoding one of the most common video codecs.
* **Custom DirectShow Video Filter Support:** Developers gained the ability to integrate their own custom DirectShow video filters into the playback graph, allowing for highly specialized video processing tasks.
* **`OnNewFilePlaybackStarted` Event:** A new event, `OnNewFilePlaybackStarted`, was introduced. This event fires specifically when a new file begins playing within a playlist context, enabling applications to react precisely to transitions between media items.
* **Updated Decoders:** The Ogg Vorbis audio decoder and WebM video decoders were updated to their latest versions, ensuring compatibility and performance improvements.
* **Frame Grabber API Update:** The API for grabbing individual video frames was updated, potentially offering improved performance or flexibility.
* **Bug Fixes:** Various unspecified bug fixes were implemented to improve overall stability and reliability.

## Version 8.5: Rotation, 4K Readiness, and Rendering Options

Version 8.5 introduced innovative video manipulation features and prepared the engine for ultra-high-definition content.

* **On-the-Fly Video Rotation:** A new video effect was added, enabling real-time rotation of the video stream during playback (e.g., 90, 180, 270 degrees).
* **Updated FFMPEG Source:** The FFMPEG source component was updated, likely incorporating support for newer formats or improving performance.
* **4K-Ready Video Effects:** Existing video effects were optimized and tested to ensure they perform efficiently with 4K resolution video content.
* **VMR-9/EVR Zoom Shift Bug Fix:** A specific bug related to unexpected image shifting when using zoom with the VMR-9 or EVR video renderers was corrected.
* **Direct2D Video Renderer (Beta):** A new video renderer based on Direct2D was introduced as a beta feature. This renderer included support for live video rotation and aimed to leverage modern graphics APIs for potentially improved performance and quality.
* **Bug Fixes:** Included various general bug fixes to enhance stability.

## Version 8.4: Decoder Updates and Stability

This was primarily a maintenance release focused on updating core components.

* **Updated FFMPEG Decoder:** The FFMPEG decoder components were updated, likely incorporating fixes and improvements from the FFMPEG project.
* **Bug Fixes:** Addressed various unspecified bugs for improved stability.

## Version 8.3: Stability Release

This release focused solely on addressing bugs identified in previous versions.

* **Bug Fixes:** Implemented various fixes to enhance the overall reliability and stability of the library.

## Version 8.0: Introducing the VLC Engine

Version 8.0 marked a significant architectural addition by integrating the powerful VLC engine.

* **VLC Engine Integration:** The renowned VLC engine was integrated as an alternative playback backend for video and audio files. This brought VLC's extensive format support and robust streaming capabilities to TVFMediaPlayer applications.
* **Bug Fixes:** Included various general bug fixes.

## Version 7.x Series: Effects, Encryption, and Playlists

The Version 7 series introduced key features related to playback control, security, and visual effects.

### Version 7.20

* **Reverse Playback:** Added the capability to play video files in reverse, opening up creative possibilities and specialized application use cases.
* **Bug Fixes:** Addressed various bugs.

### Version 7.12

* **Video Encryption Support:** Initial support for video encryption was added, providing basic content protection mechanisms.
* **Bug Fixes:** Included general stability improvements.

### Version 7.7

* **Fade-In/Fade-Out Effect:** A common and useful video transition effect, fade-in/fade-out, was added to the available video effects.
* **Playlist Support:** Functionality for creating and managing playlists was introduced, allowing sequences of media files to be played automatically.
* **Bug Fixes:** Addressed various issues.

### Version 7.5

* **Improved Chroma Key:** The chroma key (green screen) effect was enhanced for better quality and more precise control.
* **Enhanced Text Logo:** The feature for overlaying text logos onto the video was improved.
* **Modified Video Effects API:** The API for applying video effects underwent modifications, potentially for improved usability or to accommodate new features.
* **Bug Fixes:** Included various stability fixes.

### Version 7.0

* **Windows 8 RTM Support:** Ensured compatibility with the release version of Windows 8.
* **Enhanced Video Effects:** Further improvements were made to the quality and performance of existing video effects.
* **New FFMPEG Playback Engine:** Introduced a new playback engine based on FFMPEG components, offering an alternative to the default DirectShow-based playback and expanding format compatibility.

## Version 6.x Series: Windows 8 Compatibility and Optimizations

The Version 6 series focused on adapting to the then-new Windows 8 operating system and improving performance.

### Version 6.3

* **Windows 8 Customer Preview Support:** Added compatibility for the pre-release Customer Preview version of Windows 8.
* **Improved Video Effects:** Continued refinement of video effect performance and quality.

### Version 6.0

* **Enhanced OpenCL Support:** Improved utilization of OpenCL for GPU acceleration tasks, potentially boosting performance for effects or decoding on compatible hardware.
* **Windows 8 Developer Preview Support:** Added early support for the Developer Preview version of Windows 8.
* **Improved Video Effects:** General enhancements to the video effects subsystem.

## Version 3.x Series: Early Features and Optimizations

The Version 3 series laid groundwork features and focused on CPU-specific optimizations.

### Version 3.9

* **New Installers:** Introduced a new main installer and separate redistributable installers for easier deployment.
* **Minor Bug Fixes:** Addressed minor outstanding issues.

### Version 3.7

* **Improved Video Effects:** Enhancements made to the video effects features.
* **New Demo Applications:** Added new demo applications to showcase library capabilities.
* **Netbook CPU Optimizations:** Included specific performance optimizations tailored for Intel Core II/Atom and AMD netbook processors.
* **Minor Bug Fixes:** General stability improvements.

### Version 3.5

* **Improved Video Effects:** Continued work on enhancing video effects.
* **Intel Core i7 Optimizations:** Added new performance optimizations specifically for the then-new Intel Core i7 CPU architecture.

### Version 3.0

* **Motion Detection:** Introduced a motion detection feature, enabling applications to react to changes within the video stream.
* **Chroma Key:** Added initial chroma key (green screen) functionality.
* **MMS/WMV Source Support:** Included support for streaming using the MMS protocol and playing WMV (Windows Media Video) files.
* **CPU Optimizations:** Added performance optimizations targeted at Intel Atom and Core i3/i5/i7 processors.
* **Direct Stream Processing:** Enabled the capability to directly access and process decoded video and audio stream data, offering advanced manipulation possibilities.
