using CapCognition.Maui.BarcodeScanning;
using CapCognition.Maui.Common;
using CapCognition.Maui.Core.Shared.ConsoleLogger;
using CapCognition.Maui.Helpers;
using CapCognition.Maui.LPR;
using CapCognition.Maui.YoloModelDetection;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace NetMaui_samples;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseSkiaSharp()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.InitializeCapCognitionHandlers(
                    [
                        Capture.Use(/* Add your license here */),
                        Recognition.Use(/* Add your license here */),
                        BarcodeDetection.Use(/* Add your license here */),
                        LicensePlateDetection.Use(/* Add your license here */),
                        YoloModelDetection.Use(/* Add your license here */),
                    ],
                    enableProcessingLogs: true);
            });

        #if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddDebug();
        #else
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Logging.AddProvider(new ConsoleLoggerProvider());
        #endif

        var app = builder.Build();
        CapCognition.Maui.Core.Shared.CapCognition.LoggerFactory = new ConsoleLoggerFactory();

        return app;
    }
}
