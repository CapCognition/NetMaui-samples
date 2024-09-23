using CapCognition.Maui.BarcodeScanning;
using CapCognition.Maui.Core.Shared.Common;
using SkiaSharp;

namespace NetMaui_samples.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class BarcodeMainPage : ContentPage
{
    public BarcodeMainPage()
    {
        InitializeComponent();
        BindingContext = this;
        Title = "Barcode";

        RecognitionView.AddOption(new BarcodeRecognitionOption
        {
            EnableMultiCodeReader = false,
            EnableBarcodeOverlays = true,
            BinarizerToUse = BarcodeRecognitionOption.BinarizerType.HistogrammBinarizer,
            BarcodeFormatsToRecognize =
            [
                BarcodeRecognitionOption.BarcodeFormat.QRCode,
                BarcodeRecognitionOption.BarcodeFormat.DataMatrix,
                BarcodeRecognitionOption.BarcodeFormat.EAN13
            ]
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await RecognitionView.RequestAllPermissionAsync();

        //initialize event will not be fired, when asked here for the permissions
        RecognitionView.InitializedEvt += () =>
        {
            //simulate autostart
            OnCameraOpen(null, null!);
            //RecognitionView.OpenCamera();
        };
    }

    private async void OnCameraOpen(object? sender, EventArgs e)
    {
        if (RecognitionView.CameraIsOpen)
        {
            return;
        }

        await RecognitionView.OpenCameraAsync();
        RecognitionView.CaptureIntervalInMs = 50;
        RecognitionView.StartContinuousRecognition(true);
        RecognitionView.RecognitionResultEvt += OnRecognitionResult;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        RecognitionView.CloseCamera();
        RecognitionView.RecognitionResultEvt -= OnRecognitionResult;
        RecognitionView.StopContinuousRecognition();
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
}