---
title: Centro de Documentación de SDKs VisioForge
description: Documentación completa para los SDKs de video de VisioForge - captura, reproducción, edición y procesamiento para .NET, Delphi y DirectShow.
keywords: documentación SDK video, SDK captura video, SDK reproductor video, SDK editor video, procesamiento video .NET, filtros DirectShow, SDK video Delphi, video C#, ejemplos código, tutoriales video, referencia API
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

<!-- Sección Hero -->
<div class="vf-hero">
    <h1>Productos de VisioForge</h1>
    <p class="tagline">SDKs profesionales de captura, reproducción, edición y procesamiento de video para desarrolladores .NET, Delphi y DirectShow. Crea aplicaciones multimedia potentes con facilidad.</p>
    
    <div class="vf-sdk-grid">
        <a href="docs/dotnet/" class="vf-sdk-card">
            <h3>SDKs .NET</h3>
            <p>Reproducción, captura y edición de video completa con aceleración por hardware, procesamiento en tiempo real y soporte para todas las plataformas principales.</p>
            <span class="link">Explorar Documentación →</span>
        </a>
        <a href="docs/vfp/" class="vf-sdk-card">
            <h3>SDK de Huellas de Video</h3>
            <p>Crea firmas digitales únicas de contenido de video para detectar duplicados, identificar fragmentos y coincidir videos transformados.</p>
            <span class="link">Explorar Documentación →</span>
        </a>
        <a href="docs/delphi/" class="vf-sdk-card">
            <h3>SDKs Delphi / ActiveX</h3>
            <p>Potentes bibliotecas Delphi/ActiveX para reproducción, captura y edición de video con soporte x64 para aplicaciones multimedia profesionales.</p>
            <span class="link">Explorar Documentación →</span>
        </a>
        <a href="docs/directshow/" class="vf-sdk-card">
            <h3>SDKs y Filtros DirectShow</h3>
            <p>Filtros y SDKs DirectShow para reproducción, procesamiento, codificación y desarrollo de aplicaciones multimedia.</p>
            <span class="link">Explorar Documentación →</span>
        </a>
    </div>
</div>

<!-- Plataformas Soportadas -->
<div class="vf-platforms">
    <h2>Soporte Multiplataforma</h2>
    <p class="subtitle">Desarrolla una vez, despliega en todas partes. Nuestros SDKs soportan todas las plataformas principales.</p>
    <div class="vf-platforms-grid">
        <div class="vf-platform">
            <img src="../static/windows.svg" alt="Windows">
            <span>Windows</span>
        </div>
        <div class="vf-platform">
            <img src="../static/macos.svg" alt="macOS">
            <span>macOS</span>
        </div>
        <div class="vf-platform">
            <img src="../static/linux.svg" alt="Linux">
            <span>Linux</span>
        </div>
        <div class="vf-platform">
            <img src="../static/ios.svg" alt="iOS">
            <span>iOS</span>
        </div>
        <div class="vf-platform">
            <img src="../static/android.svg" alt="Android">
            <span>Android</span>
        </div>
        <div class="vf-platform">
            <img src="../static/raspberry-pi.svg" alt="Raspberry Pi">
            <span>Raspberry Pi</span>
        </div>
        <div class="vf-platform">
            <img src="../static/nvidia-jetson.svg" alt="NVIDIA Jetson">
            <span>NVIDIA Jetson</span>
        </div>
    </div>
</div>

<!-- Características de la Documentación -->
<div class="vf-docs-features">
    <h2>Recursos Completos para Desarrolladores</h2>
    <p class="subtitle">Todo lo que necesitas para integrar capacidades de video en tus aplicaciones</p>
    <div class="vf-docs-grid">
        <div class="vf-docs-card">
            <div class="icon"><img src="../static/documentation.svg" alt="Documentación"></div>
            <h3>Documentación Detallada</h3>
            <p>Referencias de API detalladas, guías paso a paso y descripciones de arquitectura que cubren todas las características, clases y métodos del SDK con ejemplos prácticos.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="../static/code-samples.svg" alt="Ejemplos de Código"></div>
            <h3>Ejemplos de Código y Proyectos GitHub</h3>
            <p>Fragmentos de código en C#, VB.NET y Delphi, además de aplicaciones de ejemplo completas y descargables para WinForms, WPF, MAUI, Avalonia, Uno y consola en GitHub.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="../static/video-tutorials.svg" alt="Tutoriales en Video"></div>
            <h3>Tutoriales en Video</h3>
            <p>Guías visuales paso a paso que muestran cómo crear aplicaciones de captura de webcam, grabación de pantalla, streaming de cámaras IP y edición de video.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="../static/deployment.svg" alt="Despliegue"></div>
            <h3>Guías de Despliegue</h3>
            <p>Instrucciones de despliegue específicas para Windows, macOS, Linux, iOS, Android y dispositivos embebidos como Raspberry Pi y NVIDIA Jetson.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="../static/support.svg" alt="Solución de Problemas"></div>
            <h3>Solución de Problemas y FAQ</h3>
            <p>Soluciones a problemas comunes, consejos de optimización de rendimiento, guía de licencias y respuestas a preguntas frecuentes de desarrolladores.</p>
        </div>
        <div class="vf-docs-card">
            <div class="icon"><img src="../static/gpu.svg" alt="Aceleración por Hardware"></div>
            <h3>Aceleración por Hardware</h3>
            <p>Codificación y decodificación de video acelerada por GPU con NVIDIA NVENC/NVDEC, Intel Quick Sync y AMD AMF para procesamiento de video de alto rendimiento.</p>
        </div>
    </div>
</div>

<!-- Banner CTA -->
<div class="vf-cta-banner">
    <h2>¿Listo para Crear Aplicaciones Multimedia Increíbles?</h2>
    <p>Descarga nuestros SDKs con una prueba gratuita y comienza a construir hoy</p>
    <div class="buttons">
        <a href="https://www.visioforge.com/buy" class="btn primary">Ver Precios</a>
        <a href="https://support.visioforge.com/" class="btn secondary">Contactar Soporte</a>
    </div>
</div>

<!-- Sección de Contenido SEO -->
<div class="vf-seo-content">
    <h2>Acerca de Esta Documentación</h2>
    <p>Bienvenido al centro de documentación oficial de los SDKs de VisioForge — tu recurso completo para crear aplicaciones profesionales de video y multimedia. Ya sea que estés desarrollando sistemas de videovigilancia, reproductores multimedia, editores de video, plataformas de streaming en vivo o software de grabación de pantalla, nuestra documentación proporciona todo lo que necesitas para tener éxito.</p>
    
    <p>Nuestra documentación está diseñada para desarrolladores de todos los niveles e incluye:</p>
    <ul>
        <li><strong>Guías de Inicio Rápido</strong> — Instrucciones de configuración rápida para tener tu primera aplicación de video funcionando en minutos</li>
        <li><strong>Documentación de Referencia de API</strong> — Cobertura completa de todas las clases, métodos, propiedades y eventos con explicaciones detalladas</li>
        <li><strong>Ejemplos y Fragmentos de Código</strong> — Cientos de ejemplos listos para copiar en C# y VB.NET para escenarios comunes y avanzados</li>
        <li><strong>Tutoriales en Video</strong> — Guías visuales paso a paso para tareas como captura de webcam, grabación MP4, streaming RTSP y efectos de video</li>
        <li><strong>Proyectos de Ejemplo Completos</strong> — Aplicaciones completas disponibles en nuestro <a href="https://github.com/visioforge/.Net-SDK-s-samples" target="_blank">repositorio de GitHub</a> para WinForms, WPF, MAUI, Avalonia, Uno y aplicaciones de consola</li>
        <li><strong>Guías Específicas por Plataforma</strong> — Instrucciones detalladas de despliegue y configuración para Windows, macOS, Linux, iOS, Android, Raspberry Pi y NVIDIA Jetson</li>
        <li><strong>Referencias de Formatos y Códecs</strong> — Documentación para todos los formatos de video soportados (MP4, AVI, MKV, WebM), códecs (H.264, HEVC, AV1, VP9) y protocolos de streaming (RTSP, RTMP, HLS, SRT, NDI)</li>
    </ul>
    
    <p>Nuestros SDKs soportan el ecosistema .NET completo incluyendo .NET Framework 4.7.2+, .NET 5/6/7/8/9 y frameworks de UI como WinForms, WPF, MAUI, Avalonia y Blazor. Con codificación y decodificación acelerada por hardware mediante NVIDIA NVENC/NVDEC, Intel Quick Sync y AMD AMF, puedes crear aplicaciones de alto rendimiento que aprovechan las capacidades modernas de GPU.</p>
    
    <p>Únete a miles de desarrolladores en todo el mundo que confían en los SDKs de VisioForge para sus proyectos multimedia. Explora nuestra documentación, descarga los proyectos de ejemplo y comienza a construir tu próxima aplicación de video hoy. ¿Necesitas ayuda? Nuestro <a href="https://support.visioforge.com/" target="_blank">equipo de soporte</a> y <a href="https://discord.com/invite/yvXUG56WCH" target="_blank">comunidad de Discord</a> están aquí para asistirte.</p>
</div>