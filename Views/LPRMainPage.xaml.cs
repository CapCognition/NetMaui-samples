using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
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

            _licensePlateOptions = new LPROption()
            {
                EnableOverlays = true,
                DisplayLicensePlateSurroundingBox = true,
                DisplayVehicleSurroundingBox = true,
                UseCroppedImageForRecognition = true,
                UseModelResolution = LPROption.RecognitionModelResolution.MediumResolution,
                UseModelSize = LPROption.ModelSize.Small,
            };

            RecognitionView.AddOption(_licensePlateOptions);
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

            if (_licensePlateOptions.Model == null)
            {
                //simulate parallel downloading of the models without blocking the main thread and image processing
                //the recognition can be started without the model being downloaded. When the model finished downloading the decoder automatically starts the recognition
                _licensePlateOptions.ModelsDownloadStartedEvt += OnLicensePlateModelsDownloadStarted;
                _licensePlateOptions.ModelLoadingProgressEvt += OnLicensePlateModelLoadingProgress;
                _licensePlateOptions.ModelDownloadedEvt += OnLicensePlateModelDownloaded;
                _licensePlateOptions.ModelsDownloadedEvt += OnLicensePlateModelsDownloaded;
                //disable and the model will be downloaded in the background when opening camera
                //await _licensePlateOptions.CreateAndPrepareModelParallelAsync();
            }

            await RecognitionView.OpenCameraAsync();
            RecognitionView.CaptureIntervalInMs = 50;
            RecognitionView.StartContinuousRecognition(true);
            RecognitionView.RecognitionResultEvt += OnRecognitionResult;
        }

        private void OnLicensePlateModelsDownloadStarted(int noOfModelsToDownload)
        {
            Console.WriteLine($"Start downloading {noOfModelsToDownload} License plate models");
        }

        private void OnLicensePlateModelLoadingProgress(long readBytes, int indexOfModel, int noOfModelsToDownload)
        {
            Console.WriteLine($"License plate model ({indexOfModel}/{noOfModelsToDownload}) downloading. {readBytes} bytes read");
        }

        private void OnLicensePlateModelDownloaded(bool success, int indexOfModel, int noOfModelsToDownload)
        {
            Console.WriteLine($"License plate model ({indexOfModel}/{noOfModelsToDownload}) finished downloading");
        }

        private void OnLicensePlateModelsDownloaded(bool success, int noOfModelsToDownload)
        {
            Console.WriteLine($"Finished downloading {noOfModelsToDownload} License plate models success={success}");
            _licensePlateOptions!.ModelsDownloadStartedEvt -= OnLicensePlateModelsDownloadStarted;
            _licensePlateOptions.ModelLoadingProgressEvt -= OnLicensePlateModelLoadingProgress;
            _licensePlateOptions.ModelDownloadedEvt -= OnLicensePlateModelDownloaded;
            _licensePlateOptions.ModelsDownloadedEvt -= OnLicensePlateModelsDownloaded;
        }

        private void OnRecognitionResult(RecognitionResult result, SKBitmap bitmap)
        {
            result.Results.ForEach(r =>
            {
                var lprResult = (RecognitionProcessorLPRResult)r;
                Console.WriteLine($"Plate number: {lprResult.PlateNumber}");
                Console.WriteLine($"Country: {lprResult.PlateCountryCode}");
                Console.WriteLine($"Vehicle type: {lprResult.Type}");
            });
        }

        protected override void OnDisappearing()
        {
            RecognitionView.Terminate();
            base.OnDisappearing();
        }

        private readonly LPROption _licensePlateOptions;
    }
}