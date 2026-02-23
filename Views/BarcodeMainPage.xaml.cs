using CapCognition.Maui.BarcodeScanning;
using CapCognition.Maui.Core.Shared.Common;
using SkiaSharp;
using System.Text;

namespace NetMaui_samples.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class BarcodeMainPage : ContentPage
{
    public BarcodeMainPage()
    {
        InitializeComponent();
        BindingContext = this;
        Title = "Barcode";

        var recogOptions = new RecognitionOptionBuilder()
            .AddBarcodeRecognitionOption()
                .EnableMultiCodeReader()
                .TryInverted()
                .SetBinarizer(BarcodeRecognitionOptions.BinarizerType.HybridBinarizer)
                .SetBarcodeFormats(new[] { BarcodeRecognitionOptions.BarcodeFormat.QRCode })
                .SetEncoding(Encoding.UTF8)

                .AddBarcodeRecognitionOverlayDrawingOption()
                    .EnableBarcodeOverlays()
                    .SetSurroundingRectColor(Color.FromRgb(255, 0, 0))
                    .SetSurroundingStrokeWidth(2)
                    .SetOneDimensionalRectWidth(5)
                    .Done()
                .Done()
            .Build();

        foreach (var option in recogOptions)
        {
            RecognitionView.AddOption(option);
        }
        _recognitionOptions = recogOptions;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            if (RecognitionView.Initialized)
            {
                OpenCamera();
            }
        }
        catch (Exception)
        {
            //Show error message
            return;
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
    }

    private void OnCameraOpened(bool success)
    {
        RecognitionView.CameraOpenedEvt -= OnCameraOpened;

        if (!success)
        {
            //Show error message
            return;
        }

        DoStartContinuousRecognition();
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
        result.Results.ForEach(r =>
        {
            var barcodeResult = (RecognitionProcessorBarcodeResult)r;
            switch (barcodeResult.Type)
            {
                case RecognitionProcessorBarcodeResult.ValueType.Unknown:
                    Console.WriteLine("Unknown: " + barcodeResult.Value);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.CalendarEvent:
                    Console.WriteLine("CalendarEvent: " + barcodeResult.RecognizedBarcodeCalendarEvent);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.ContactInfo:
                    Console.WriteLine("ContactInfo: " + barcodeResult.RecognizedBarcodeContact);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.Geo:
                    Console.WriteLine("Geo: " + barcodeResult.RecognizedBarcodeGeoPoint);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.Phone:
                    Console.WriteLine("Phone: " + barcodeResult.Phone);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.Email:
                    Console.WriteLine("Email: " + barcodeResult.Email);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.SMS:
                    Console.WriteLine("SMS: " + barcodeResult.RecognizedBarcodeSms);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.Text:
                    Console.WriteLine("Text: " + barcodeResult.Value);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.URL:
                    Console.WriteLine("URL: " + barcodeResult.RecognizedBarcodeUrl);
                    break;
                case RecognitionProcessorBarcodeResult.ValueType.Wifi:
                    Console.WriteLine("Wifi: " + barcodeResult.RecognizedBarcodeWifiInfo);
                    break;
            }
        });
    }

    private List<RecognitionOptions> _recognitionOptions;
}