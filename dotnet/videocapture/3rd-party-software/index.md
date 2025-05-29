---
title: Video Capture SDK Third-Party Integration Guide
description: Master video capture integration with DirectShow applications. Learn to connect OBS, FFMPEG, and VLC with our SDK. Step-by-step tutorials for WinForms, WPF, and console apps. Perfect for developers building video processing solutions.
sidebar_label: 3rd-Party Software Usage
order: 4
---

# Integrating Third-Party Software with Video Capture SDK

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net)

## Overview

The Video Capture SDK .NET provides robust capabilities for integrating with various third-party software applications. This integration expands the functionality of your applications and allows for greater flexibility in video processing workflows.

## How Integration Works

The SDK uses Virtual Camera SDK as a bridge between our Video Capture SDK and third-party applications. This bridge creates a virtual camera device that can be detected and used by any DirectShow-compatible application in your development environment.

### Video Bridge

The virtual camera technology allows captured video streams to be seamlessly passed to external applications without quality loss or significant performance impact.

### Audio Bridge

In addition to video, an audio bridge is also provided, enabling complete audio-visual integration with external software.

## Compatible Applications

The virtual camera works with numerous DirectShow-compatible applications, including:

- OBS (Open Broadcaster Software)
- FFMPEG
- VLC Media Player
- Zoom, Teams, and other conferencing software
- Custom DirectShow applications

## Detailed Tutorials

Our step-by-step tutorials guide you through the integration process with popular applications:

- [FFMPEG Streaming Integration](ffmpeg-streaming.md) - Learn how to use FFMPEG with the SDK for powerful streaming capabilities
- [OBS Streaming Setup](obs-streaming.md) - Detailed guide for integrating with Open Broadcaster Software
  
## Development Resources

We provide extensive documentation and examples to help you implement these integrations in your software projects. The integration works across all supported platforms:

- WinForms applications
- WPF (Windows Presentation Foundation) applications
- Console applications

---

For additional implementation examples and code samples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
