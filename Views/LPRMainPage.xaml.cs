using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
using SkiaSharp;

namespace NetMaui_samples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LPRMainPage : ContentPage
    {
        public LPRMainPage(LicensePlateDetectionRecognitionOption lprOptions)
        {
            InitializeComponent();
            BindingContext = this;
            Title = "License Plate Recognition";

            RecognitionView.AddOption(lprOptions);

            Loaded += OnLoaded;

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
    }
}