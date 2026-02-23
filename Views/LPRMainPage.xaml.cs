using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (RecognitionView.Initialized)
        {
            OpenCamera();
        }
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

            Loaded -= OnLoaded;
            OpenCamera();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    protected override void OnDisappearing()
    {
        RecognitionView.CloseCamera();
        base.OnDisappearing();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        Unloaded -= OnUnloaded;
        RecognitionView.Terminate();
        _recognitionOptions.Dispose();
    }

    private void OpenCamera()
    {
        if (RecognitionView.CameraIsOpen)
        {
            return;
        }

        RecognitionView.CameraOpenedEvt -= OnCameraOpened;
        RecognitionView.CameraOpenedEvt += OnCameraOpened;
        var success = RecognitionView.OpenCamera();
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

        DoStartContinuousRecognition();
        Console.WriteLine("###Recog started");
    }

    private void DoStartContinuousRecognition()
    {
        var success = RecognitionView.StartContinuousRecognition();
        if (success)
        {
            RecognitionView.RecognitionResultEvt += OnRecognitionResult;
        }
    }

    private void DoStopContinuousRecognition()
    {
        var success = RecognitionView.StopContinuousRecognition();
        if (success)
        {
            RecognitionView.RecognitionResultEvt -= OnRecognitionResult;
        }
    }

    private void OnRecognitionResult(RecognitionResult result, SKBitmap bitmap)
    {
        var lpResult = result.GetResult<RecognitionProcessorLicensePlateDetectionResult>();
        if (lpResult == null)
        {
            Console.WriteLine($"No LP result {result}");
            return;
        }

        Console.WriteLine($"Plate number validated: {lpResult.PlateNumberValidated}");
        Console.WriteLine($"Plate number raw: {lpResult.PlateNumberRaw}");
        Console.WriteLine($"Country: {lpResult.PlateCountryCode}");
        Console.WriteLine($"Vehicle type: {lpResult.VehicleType}");

        var bitmapClone = lpResult.PlateBitmap?.Copy();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LPPreview.Source = (SKBitmapImageSource)bitmapClone;
        });
    }

    private List<RecognitionOptions> _recognitionOptions;
    private LicensePlateDetectionRecognitionOptions? _licensePlateOptions;
}