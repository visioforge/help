---
title: Essential .NET SDK Code Samples for Developers
description: Practical implementation examples for DirectShow filters, audio/video processing, rendering techniques, and media manipulation in .NET applications - designed to accelerate your development workflow.
sidebar_label: Code Samples

order: -4
---

# .NET SDK Code Samples: Practical Implementation Guide

In this guide, you'll find a collection of practical code samples and implementation techniques for working with our .NET SDKs. These examples address common development scenarios and demonstrate how to leverage our libraries effectively for media processing applications.

## DirectShow Filter Implementation

DirectShow provides a powerful framework for handling multimedia streams. Our SDKs simplify working with these components through well-designed interfaces and helper methods.

### Media Indexing and Format Handling

- [ASF and WMV Files Indexing](asf-wmv-files-indexing.md) - Learn techniques for properly indexing Windows Media formats to enable seeking and efficient playback position control. This sample demonstrates how to establish accurate navigation points within media files and handle large ASF/WMV content effectively.

### Custom Filter Integration

- [Custom DirectShow Filter Interface Usage](custom-filter-interface.md) - This tutorial walks through the process of implementing and connecting custom DirectShow filters within your application. You'll learn how to create filter interfaces that integrate seamlessly with the existing DirectShow architecture while adding your own specialized functionality.

### Third-Party Integration

- [Integrating Third-Party Video Processing Filters](3rd-party-video-effects.md) - Discover how to incorporate external video processing components into your DirectShow filter graph. This example demonstrates proper filter registration, connection methods, and parameter configuration for third-party video effects and transformations.

### Filter Management

- [Manual DirectShow Filter Uninstallation](uninstall-directshow-filter.md) - This guide explains the registry entries, COM object registration, and system directories involved in completely removing DirectShow filters when standard uninstallation isn't sufficient or available.

- [Excluding Specific DirectShow Filters](exclude-filters.md) - Learn techniques for selectively bypassing certain DirectShow filters in your filter graph construction. This sample shows how to exclude specific decoders, encoders, or processing filters while maintaining proper media handling.

## Audio and Video Processing Techniques

Manipulating audio and video streams is a core requirement for many media applications. These samples demonstrate different approaches to accessing and modifying media data.

### Real-time Video Effects

- [Custom Video Effects Using Frame Events](custom-video-effects.md) - Learn two powerful approaches for implementing real-time video effects through the OnVideoFrameBitmap and OnVideoFrameBuffer events. This comprehensive sample demonstrates how to access video frames, apply effects, and optimize performance.

### Advanced Overlay Techniques

- [Multi-text Overlay Drawing](draw-multitext-onvideoframebuffer.md) - This sample demonstrates techniques for rendering multiple text elements on video frames with precise positioning and style control. You'll learn how to handle text formatting, alpha blending, and performance optimization.

- [Text Overlay Implementation](text-onvideoframebuffer.md) - A focused tutorial on adding dynamic text annotations to video content. This example covers font selection, positioning, and real-time updates of overlay text.

- [Image Overlay Integration](image-onvideoframebuffer.md) - Learn how to composite images onto video frames with proper scaling, alpha blending, and positioning. This example shows techniques for watermarking, logo placement, and dynamic image overlays.

### Video Transformation

- [Manual Zoom Effect Implementation](zoom-onvideoframebuffer.md) - This detailed example demonstrates how to implement a custom zoom functionality by directly manipulating video frame buffers. You'll learn techniques for region selection, scaling algorithms, and smooth transitions between zoom levels.

### Bitmap-Based Frame Processing

- [OnVideoFrameBitmap Event Usage](onvideoframebitmap-usage.md) - This guide explores the bitmap-based approach to video frame processing, which offers simplified access to frame data through GDI+ compatible objects. Learn how this differs from buffer-based processing and when to choose each approach.

## Video Rendering Solutions

Displaying video content with flexibility and performance requires understanding various rendering techniques. These samples demonstrate different approaches for visual presentation.

### Windows Forms Integration

- [PictureBox Video Rendering](draw-video-picturebox.md) - This sample demonstrates how to properly render video content within a standard Windows Forms PictureBox control. You'll learn about frame timing, aspect ratio preservation, and performance considerations.

### Multi-Display Functionality

- [Multiple Renderer Zoom Configuration](zoom-video-multiple-renderer.md) - Learn techniques for independently controlling zoom levels across multiple video renderers. This sample is essential for applications requiring synchronized but visually distinct video outputs.

- [WPF Multi-screen Video Output](multiple-screens-wpf.md) - This example shows how to implement multiple independent video display surfaces within a WPF application. You'll learn proper control initialization, resource management, and synchronization techniques.

### Renderer Selection and Customization

- [Video Renderer Selection (WinForms)](select-video-renderer-winforms.md) - This tutorial explains how to choose and configure the most appropriate video renderer for your Windows Forms application. You'll understand the tradeoffs between EVR, VMR9, and other renderer types.

### User Interaction 

- [Mouse Wheel Event Integration](mouse-wheel-usage.md) - Learn how to handle mouse wheel events for interactive video displays. This sample demonstrates zoom control, timeline scrubbing, and other wheel-based interactions.

- [Custom Image Video View](video-view-set-custom-image.md) - This guide shows how to replace the standard video frame with a custom image for scenarios like connection loss, buffering states, or application-specific messaging.

## Media Information and Visualization

These samples demonstrate how to extract information from media files and create useful visualizations.

### File Analysis

- [Media File Information Extraction](read-file-info.md) - Learn techniques for reading detailed metadata, stream properties, and format information from media files. This example shows how to access duration, bitrate, codec information, and other essential media properties.

### Audio Visualization

- [VU Meter and Waveform Visualization](vu-meters.md) - This comprehensive sample demonstrates how to create real-time audio visualizations including volume unit meters and waveform displays. You'll learn about audio level analysis, drawing techniques, and synchronization with playback.

## Performance Optimization

Each sample in this collection is designed with performance considerations in mind. You'll find techniques for efficient buffer handling, memory management, and processing optimizations that help you build responsive media applications, even when working with high-resolution content or applying complex effects.

## Cross-Platform Considerations

While focusing on .NET implementations, many of the concepts demonstrated in these samples apply to other platforms as well. Where appropriate, we've noted platform-specific considerations and alternative approaches for cross-platform development scenarios.

## Getting Started

To use these examples effectively, we recommend reviewing the appropriate SDK documentation for your specific product version. Each sample includes the necessary references and initialization code, but may require configuration based on your development environment and target platform.

These code samples serve as building blocks for your media applications, providing proven implementation patterns that you can adapt and extend for your specific requirements.
