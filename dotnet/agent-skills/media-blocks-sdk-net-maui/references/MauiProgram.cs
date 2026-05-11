using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

namespace SimpleCaptureMB
{
    /// <summary>
    /// The MAUI program class.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Create the MAUI app.
        /// </summary>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                // Critical: without AddVisioForgeHandlers() the <my:VideoView /> renders blank,
                // no errors logged. The handler binds the MAUI control to the per-OS native view.
                .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
