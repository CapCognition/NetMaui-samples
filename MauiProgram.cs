using CapCognition.Maui.BarcodeScanning;
using CapCognition.Maui.Helpers;
using CapCognition.Maui.LPR;
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
                    licenses:
                    //Add your license keys here
                    [
                        ""
                    ],
                    usedOptions: [
                        //Add the options you want to use here
                        new BarcodeRecognitionOption(),
                        new LPROption(),
                    ],
                    enableProcessingLogs: true);
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
