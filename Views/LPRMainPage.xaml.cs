using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
using CapCognition.Maui.LPR.Shared;
using SkiaSharp;

namespace NetMaui_samples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LPRMainPage : ContentPage
    {
        public LPRMainPage()
        {
            InitializeComponent();
            BindingContext = this;
            Title = "License Plate Recognition";

            _licensePlateOptions = new LicensePlateDetectionRecognitionOption()
            {
                EnableOverlays = true,
                DisplayLicensePlateSurroundingBox = false,
                DisplayVehicleSurroundingBox = false,
                UseCroppedImageForRecognition = true,
                DoAutomaticDetectionOptimization = false,
                DetectVehicleType = false
            };
            RecognitionView.AddOption(_licensePlateOptions);

            Loaded += OnLoaded;

        }

        private async void OnLoaded(object? sender, EventArgs e)
        {
            try
            {
                var lpModelName = LicensePlateDetectionConstants.LicensePlateModelFileName320N + ".ccml";
                var textModelName = LicensePlateDetectionConstants.TextModelFileName320N + ".ccml";
                var vehicleModelName = LicensePlateDetectionConstants.VehicleModelFileName320N + ".ccml";

                var plateModelStream = new LicensePlateDetectionRecognitionOption.StreamInfo(
                    FileSystem.Current.OpenAppPackageFileAsync(lpModelName).GetAwaiter().GetResult(),
                    lpModelName);
                var textModelStream = new LicensePlateDetectionRecognitionOption.StreamInfo(
                    FileSystem.Current.OpenAppPackageFileAsync(textModelName).GetAwaiter().GetResult(),
                    textModelName);

                if (_licensePlateOptions.DetectVehicleType)
                {
                    var vehicleModelStream = new LicensePlateDetectionRecognitionOption.StreamInfo(
                        FileSystem.Current.OpenAppPackageFileAsync(vehicleModelName).GetAwaiter().GetResult(),
                        vehicleModelName);

                    _licensePlateOptions.SetModelStreams(plateModelStream, textModelStream, vehicleModelStream);
                }
                else
                {
                    _licensePlateOptions.SetModelStreams(plateModelStream, textModelStream);
                }
                await _licensePlateOptions.CreateAndPrepareModelsAsync();

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

        private readonly LicensePlateDetectionRecognitionOption _licensePlateOptions;
    }
}