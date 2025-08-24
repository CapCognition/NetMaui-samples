using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
using SkiaSharp;

namespace NetMaui_samples.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LPRMainPage : ContentPage
{
    public LPRMainPage(List<RecognitionOptions> options)
    {
        _recognitionOptions = options;
        InitializeComponent();
        BindingContext = this;
        Title = "License Plate Recognition";

        foreach (var option in options)
        {
            RecognitionView.AddOption(option);
        }

        _licensePlateOptions = RecognitionView.GetOption<LicensePlateDetectionRecognitionOptions>();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            var result = await RecognitionView.RequestAllPermissionAsync();
            if (!result)
            {
                await Navigation.PopAsync();
            }

            if (!RecognitionView.Initialized)
            {
                RecognitionView.InitializedEvt += OpenCamera;
            }
            else
            {
                OpenCamera();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _recognitionOptions.Dispose();
    }

    private void OpenCamera()
    {
        if (RecognitionView.CameraIsOpen)
        {
            return;
        }

        RecognitionView.CameraOpenedEvt += OnCameraOpened;
        RecognitionView.OpenCamera();
        Console.WriteLine("###Camera opening returned");
    }

    private void OnCameraOpened(bool success)
    {
        RecognitionView.CameraOpenedEvt -= OnCameraOpened;

        if (!success)
        {
            Console.WriteLine("Camera open failed");
            return;
        }

        Console.WriteLine("###Camera opened");

        RecognitionView.RecognitionResultEvt -= OnRecognitionResult;
        RecognitionView.RecognitionResultEvt += OnRecognitionResult;
        RecognitionView.StartContinuousRecognition(true);
        Console.WriteLine("###Recog started");
    }

    private void OnRecognitionResult(RecognitionResult? result, SKBitmap bitmap)
    {
        result?.Results.ForEach(r =>
        {
            var lprResult = (RecognitionProcessorLicensePlateDetectionResult)r;
            Console.WriteLine($"Plate number validated: {lprResult.PlateNumberValidated}");
            Console.WriteLine($"Plate number raw: {lprResult.PlateNumberRaw}");
            Console.WriteLine($"Country: {lprResult.PlateCountryCode}");
            Console.WriteLine($"Vehicle type: {lprResult.VehicleType}");
        });
    }

    protected override void OnDisappearing()
    {
        RecognitionView.Terminate();
        base.OnDisappearing();
    }

    private List<RecognitionOptions> _recognitionOptions;
    private LicensePlateDetectionRecognitionOptions? _licensePlateOptions;

}