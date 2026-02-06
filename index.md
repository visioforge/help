---
title: VisioForge SDK Documentation Hub
description: Comprehensive documentation for VisioForge video SDKs - capture, playback, editing, and processing for .NET, Delphi, and DirectShow.
keywords: video SDK documentation, video capture SDK, video player SDK, video editor SDK, .NET video processing, DirectShow filters, Delphi video SDK, C# video, code samples, video tutorials, API reference
hide:
  - navigation
  - toc
---

<style>
.vf-hero {
    background: linear-gradient(135deg, #0f172a 0%, #1e3a5f 50%, #0f172a 100%);
    border-radius: 16px;
    padding: 4rem 2rem;
    text-align: center;
    margin: -1rem -0.5rem 3rem -0.5rem;
    position: relative;
    overflow: hidden;
}
.vf-hero::before {
    content: '';
    position: absolute;
    top: 0; left: 0; right: 0; bottom: 0;
    background: radial-gradient(ellipse at 50% 0%, rgba(14, 165, 233, 0.15) 0%, transparent 60%);
    pointer-events: none;
}
.vf-hero h1 {
    font-size: 2.5rem;
    font-weight: 700;
    color: #fff;
    margin: 0 0 1rem 0;
    position: relative;
}
.vf-hero .tagline {
    font-size: 1.25rem;
    color: #94a3b8;
    margin: 0 0 2rem 0;
    max-width: 700px;
    margin-left: auto;
    margin-right: auto;
}
.vf-hero .cta-buttons {
    display: flex;
    gap: 1rem;
    justify-content: center;
    flex-wrap: wrap;
}
.vf-hero .cta-btn {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.875rem 1.75rem;
    border-radius: 8px;
    font-weight: 600;
    text-decoration: none;
    transition: all 0.2s ease;
}
.vf-hero .cta-btn.primary {
    background: linear-gradient(135deg, #0ea5e9 0%, #0284c7 100%);
    color: #fff;
}
.vf-hero .cta-btn.primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 25px rgba(14, 165, 233, 0.4);
}
.vf-hero .cta-btn.secondary {
    background: rgba(255, 255, 255, 0.1);
    color: #fff;
    border: 1px solid rgba(255, 255, 255, 0.2);
}
.vf-hero .cta-btn.secondary:hover {
    background: rgba(255, 255, 255, 0.15);
    border-color: rgba(255, 255, 255, 0.3);
}
.vf-hero .vf-sdk-grid {
    margin-top: 2.5rem;
    text-align: left;
}
.vf-sdk-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
}
@media (max-width: 768px) {
    .vf-sdk-grid { grid-template-columns: 1fr; }
}
.vf-sdk-card {
    background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
    border: 1px solid #334155;
    border-radius: 12px;
    padding: 1.75rem;
    transition: all 0.3s ease;
    text-decoration: none;
    display: block;
}
.vf-sdk-card:hover {
    border-color: #0ea5e9;
    transform: translateY(-2px);
    box-shadow: 0 8px 30px rgba(14, 165, 233, 0.15);
}
.vf-sdk-card .icon {
    font-size: 2rem;
    margin-bottom: 1rem;
}
.vf-sdk-card h3 {
    color: #f1f5f9;
    margin: 0 0 0.75rem 0;
    font-size: 1.15rem;
}
.vf-sdk-card p {
    color: #94a3b8;
    margin: 0 0 1rem 0;
    font-size: 0.9rem;
    line-height: 1.6;
}
.vf-sdk-card .link {
    color: #0ea5e9;
    font-size: 0.85rem;
    font-weight: 600;
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
}
.vf-sdk-card:hover .link {
    color: #38bdf8;
}
.vf-quicklinks {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 1rem;
    margin: 3rem 0;
}
@media (max-width: 900px) {
    .vf-quicklinks { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 500px) {
    .vf-quicklinks { grid-template-columns: 1fr; }
}
.vf-quicklink {
    background: rgba(14, 165, 233, 0.1);
    border: 1px solid rgba(14, 165, 233, 0.2);
    border-radius: 10px;
    padding: 1.25rem;
    text-align: center;
    text-decoration: none;
    transition: all 0.2s ease;
}
.vf-quicklink:hover {
    background: rgba(14, 165, 233, 0.15);
    border-color: #0ea5e9;
    transform: translateY(-2px);
}
.vf-quicklink .icon {
    font-size: 1.5rem;
    margin-bottom: 0.5rem;
}
.vf-quicklink span {
    display: block;
    color: #e2e8f0;
    font-weight: 500;
    font-size: 0.9rem;
}
.vf-requirements {
    background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
    border: 1px solid #334155;
    border-radius: 12px;
    padding: 2rem;
    margin: 3rem 0;
}
.vf-requirements h2 {
    color: #f1f5f9;
    margin: 0 0 1.5rem 0;
    text-align: center;
}
.vf-req-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 2rem;
}
@media (max-width: 768px) {
    .vf-req-grid { grid-template-columns: 1fr; }
}
.vf-req-col h4 {
    color: #0ea5e9;
    margin: 0 0 0.75rem 0;
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.05em;
}
.vf-req-col ul {
    list-style: none;
    padding: 0;
    margin: 0;
}
.vf-req-col li {
    color: #94a3b8;
    padding: 0.35rem 0;
    font-size: 0.9rem;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}
.vf-req-col li::before {
    content: '✓';
    color: #22c55e;
    font-weight: bold;
}
.vf-cta-banner {
    background: linear-gradient(135deg, #0ea5e9 0%, #0284c7 100%);
    border-radius: 12px;
    padding: 3rem 2rem;
    text-align: center;
    margin: 3rem 0;
}
.vf-cta-banner h2 {
    color: #fff;
    margin: 0 0 0.75rem 0;
}
.vf-cta-banner p {
    color: rgba(255, 255, 255, 0.9);
    margin: 0 0 1.5rem 0;
}
.vf-cta-banner .buttons {
    display: flex;
    gap: 1rem;
    justify-content: center;
    flex-wrap: wrap;
}
.vf-cta-banner .btn {
    padding: 0.75rem 1.5rem;
    border-radius: 8px;
    font-weight: 600;
    text-decoration: none;
    transition: all 0.2s ease;
}
.vf-cta-banner .btn.primary {
    background: #fff;
    color: #0284c7;
}
.vf-cta-banner .btn.primary:hover {
    background: #f0f9ff;
    transform: translateY(-2px);
}
.vf-cta-banner .btn.secondary {
    background: transparent;
    color: #fff;
    border: 2px solid rgba(255, 255, 255, 0.5);
}
.vf-cta-banner .btn.secondary:hover {
    border-color: #fff;
    background: rgba(255, 255, 255, 0.1);
}
.vf-platforms {
    background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
    border: 1px solid #334155;
    border-radius: 12px;
    padding: 2.5rem;
    margin: 3rem 0;
    text-align: center;
}
.vf-platforms h2 {
    color: #f1f5f9;
    margin: 0 0 0.5rem 0;
}
.vf-platforms .subtitle {
    color: #64748b;
    margin: 0 0 2rem 0;
}
.vf-platforms-grid {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    gap: 1.5rem;
}
@media (max-width: 1100px) {
    .vf-platforms-grid { grid-template-columns: repeat(4, 1fr); }
}
@media (max-width: 700px) {
    .vf-platforms-grid { grid-template-columns: repeat(3, 1fr); }
}
@media (max-width: 500px) {
    .vf-platforms-grid { grid-template-columns: repeat(2, 1fr); }
}
.vf-platform {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 1.5rem 1rem;
    background: rgba(15, 23, 42, 0.5);
    border: 1px solid #334155;
    border-radius: 10px;
    transition: all 0.3s ease;
}
.vf-platform:hover {
    border-color: #0ea5e9;
    transform: translateY(-4px);
    box-shadow: 0 8px 25px rgba(14, 165, 233, 0.15);
}
.vf-platform img {
    width: 48px;
    height: 48px;
    margin-bottom: 0.75rem;
}
.vf-platform span {
    color: #e2e8f0;
    font-weight: 500;
    font-size: 0.85rem;
}
.vf-docs-features {
    margin: 3rem 0;
}
.vf-docs-features h2 {
    color: #f1f5f9;
    text-align: center;
    margin: 0 0 0.5rem 0;
}
.vf-docs-features .subtitle {
    color: #64748b;
    text-align: center;
    margin: 0 0 2rem 0;
}
.vf-docs-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
}
@media (max-width: 900px) {
    .vf-docs-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 600px) {
    .vf-docs-grid { grid-template-columns: 1fr; }
}
.vf-docs-card {
    background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
    border: 1px solid #334155;
    border-radius: 12px;
    padding: 1.5rem;
    transition: all 0.3s ease;
}
.vf-docs-card:hover {
    border-color: #0ea5e9;
    transform: translateY(-2px);
    box-shadow: 0 8px 30px rgba(14, 165, 233, 0.15);
}
.vf-docs-card .icon {
    margin-bottom: 1rem;
}
.vf-docs-card .icon img {
    width: 48px;
    height: 48px;
}
.vf-docs-card h3 {
    color: #f1f5f9;
    margin: 0 0 0.75rem 0;
    font-size: 1.1rem;
}
.vf-docs-card p {
    color: #94a3b8;
    margin: 0;
    font-size: 0.9rem;
    line-height: 1.6;
}
.vf-seo-content {
    background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
    border: 1px solid #334155;
    border-radius: 12px;
    padding: 2.5rem;
    margin: 3rem 0;
}
.vf-seo-content h2 {
    color: #f1f5f9;
    margin: 0 0 1.5rem 0;
    text-align: center;
}
.vf-seo-content p {
    color: #94a3b8;
    line-height: 1.8;
    margin: 0 0 1rem 0;
    font-size: 0.95rem;
}
.vf-seo-content p:last-child {
    margin-bottom: 0;
}
.vf-seo-content ul {
    color: #94a3b8;
    line-height: 1.8;
    margin: 1rem 0;
    padding-left: 1.5rem;
}
.vf-seo-content li {
    margin-bottom: 0.5rem;
}
.vf-seo-content a {
    color: #0ea5e9;
    text-decoration: none;
}
.vf-seo-content a:hover {
    color: #38bdf8;
    text-decoration: underline;
}
</style>

<!-- Hero Section -->
<div class="vf-hero">
    <h1>VisioForge Products</h1>
    <p class="tagline">Professional video capture, playback, editing, and processing SDKs for .NET, Delphi, and DirectShow developers. Build powerful multimedia applications with ease.</p>
    
    <div class="vf-sdk-grid">
        <a href="docs/dotnet/" class="vf-sdk-card">
            <h3>.NET SDKs</h3>
            <p>Comprehensive video playback, capture, and editing with hardware acceleration, real-time processing, and support for all major platforms.</p>
            <span class="link">Explore Documentation →</span>
        </a>
        <a href="docs/vfp/" class="vf-sdk-card">
            <h3>Video Fingerprinting SDK</h3>
            <p>Create unique digital signatures of video content to detect duplicates, identify fragments, and match transformed videos.</p>
            <span class="link">Explore Documentation →</span>
        </a>
        <a href="docs/delphi/" class="vf-sdk-card">
            <h3>Delphi / ActiveX SDKs</h3>
            <p>Powerful Delphi/ActiveX libraries for video playback, capture, and editing with x64 support for professional media applications.</p>
            <span class="link">Explore Documentation →</span>
        </a>
        <a href="docs/directshow/" class="vf-sdk-card">
            <h3>DirectShow SDKs & Filters</h3>
            <p>DirectShow filters and SDKs for video playback, processing, encoding, and multimedia application development.</p>
            <span class="link">Explore Documentation →</span>
        </a>
    </div>
</div>

<!-- Supported Platforms -->
<div class="vf-platforms">
    <h2>Cross-Platform Support</h2>
    <p class="subtitle">Build once, deploy everywhere. Our SDKs support all major platforms.</p>
    <div class="vf-platforms-grid">
        <div class="vf-platform">
            <img src="static/windows.svg" alt="Windows">
            <span>Windows</span>
        </div>
        <div class="vf-platform">
            <img src="static/macos.svg" alt="macOS">
            <span>macOS</span>
        </div>
        <div class="vf-platform">
            <img src="static/linux.svg" alt="Linux">
            <span>Linux</span>
        </div>
        <div class="vf-platform">
            <img src="static/ios.svg" alt="iOS">
            <span>iOS</span>
        </div>
        <div class="vf-platform">
            <img src="static/android.svg" alt="Android">
            <span>Android</span>
        </div>
        <div class="vf-platform">
            <img src="static/raspberry-pi.svg" alt="Raspberry Pi">
            <span>Raspberry Pi</span>
        </div>
        <div class="vf-platform">
            <img src="static/nvidia-jetson.svg" alt="NVIDIA Jetson">
            <span>NVIDIA Jetson</span>
        </div>
    </div>
</div>

<!-- Documentation Features -->
<div class="vf-docs-features">
    <h2>Comprehensive Developer Resources</h2>
    <p class="subtitle">Everything you need to integrate video capabilities into your applications</p>
    <div class="vf-docs-grid">
        <div class="vf-docs-card">
            <div class="icon"><img src="static/documentation.svg" alt="Documentation"></div>
            <h3>In-Depth Documentation</h3>
            <p>Detailed API references, step-by-step guides, and architecture overviews covering all SDK features, classes, and methods with practical usage examples.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="static/code-samples.svg" alt="Code Samples"></div>
            <h3>Code Samples & GitHub Projects</h3>
            <p>Copy-paste code snippets in C#, VB.NET, and Delphi, plus complete downloadable sample applications for WinForms, WPF, MAUI, Avalonia, Uno, and console apps on GitHub.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="static/video-tutorials.svg" alt="Video Tutorials"></div>
            <h3>Video Tutorials</h3>
            <p>Step-by-step video walkthroughs showing how to build webcam capture, screen recording, IP camera streaming, and video editing applications.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="static/deployment.svg" alt="Deployment"></div>
            <h3>Deployment Guides</h3>
            <p>Platform-specific deployment instructions for Windows, macOS, Linux, iOS, Android, and embedded devices like Raspberry Pi and NVIDIA Jetson.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="static/support.svg" alt="Troubleshooting"></div>
            <h3>Troubleshooting & FAQ</h3>
            <p>Solutions to common issues, performance optimization tips, licensing guidance, and answers to frequently asked developer questions.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="static/gpu.svg" alt="Hardware Acceleration"></div>
            <h3>Hardware Acceleration</h3>
            <p>GPU-accelerated video encoding and decoding with NVIDIA NVENC/NVDEC, Intel Quick Sync, and AMD AMF for high-performance video processing.</p>
        </div>
    </div>
</div>

<!-- CTA Banner -->
<div class="vf-cta-banner">
    <h2>Ready to Build Amazing Media Apps?</h2>
    <p>Download our SDKs with a free trial and start building today</p>
    <div class="buttons">
        <a href="https://www.visioforge.com/buy" class="btn primary">View Pricing</a>
        <a href="https://support.visioforge.com/" class="btn secondary">Contact Support</a>
    </div>
</div>

<!-- SEO Content Section -->
<div class="vf-seo-content">
    <h2>About This Documentation</h2>
    <p>Welcome to the official VisioForge SDK documentation hub — your complete resource for building professional video and multimedia applications. Whether you're developing video surveillance systems, media players, video editors, live streaming platforms, or screen recording software, our documentation provides everything you need to succeed.</p>
    
    <p>Our documentation is designed for developers of all skill levels and includes:</p>
    <ul>
        <li><strong>Getting Started Guides</strong> — Quick setup instructions to have your first video application running in minutes</li>
        <li><strong>API Reference Documentation</strong> — Complete coverage of all classes, methods, properties, and events with detailed explanations</li>
        <li><strong>Code Samples & Snippets</strong> — Hundreds of copy-ready examples in C# and VB.NET for common and advanced scenarios</li>
        <li><strong>Video Tutorials</strong> — Visual step-by-step guides for tasks like webcam capture, MP4 recording, RTSP streaming, and video effects</li>
        <li><strong>Full Sample Projects</strong> — Complete applications available on our <a href="https://github.com/visioforge/.Net-SDK-s-samples" target="_blank">GitHub repository</a> for WinForms, WPF, MAUI, Avalonia, Uno, and console apps</li>
        <li><strong>Platform-Specific Guides</strong> — Detailed deployment and configuration instructions for Windows, macOS, Linux, iOS, Android, Raspberry Pi, and NVIDIA Jetson</li>
        <li><strong>Format & Codec References</strong> — Documentation for all supported video formats (MP4, AVI, MKV, WebM), codecs (H.264, HEVC, AV1, VP9), and streaming protocols (RTSP, RTMP, HLS, SRT, NDI)</li>
    </ul>
    
    <p>Our SDKs support the complete .NET ecosystem including .NET Framework 4.7.2+, .NET 5/6/7/8/9, and UI frameworks like WinForms, WPF, MAUI, Avalonia, Uno, and Blazor. With hardware-accelerated encoding and decoding via NVIDIA NVENC/NVDEC, Intel Quick Sync, and AMD AMF, you can build high-performance applications that leverage modern GPU capabilities.</p>
    
    <p>Join thousands of developers worldwide who trust VisioForge SDKs for their multimedia projects. Explore our documentation, download the sample projects, and start building your next video application today. Need help? Our <a href="https://support.visioforge.com/" target="_blank">support team</a> and <a href="https://discord.com/invite/yvXUG56WCH" target="_blank">Discord community</a> are here to assist you.</p>
</div>