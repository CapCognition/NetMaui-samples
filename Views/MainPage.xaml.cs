using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
using CapCognition.Maui.LPR.Shared;
using CapCognition.Maui.LPR.Shared.Extensions;

namespace NetMaui_samples.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private void OnSinglePhotoClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CameraSinglePage());
    }

    private void OnMultiPhotoClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CameraMultiPage());
    }

    private void OnBarcodeClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new BarcodeMainPage());
    }

    private void OnPrepareOptionsInBackground(object? sender, EventArgs e)
    {
        SetupRecognitionOptionsDownloadAutoMode();
    }

    private async void OnLPRClicked(object sender, EventArgs e)
    {
        //use one the following options to prepare the models for LPR.
        //The first and second options will use local models that are included in the app package.
        //The difference between the first and the second options is that the first one uses a builder pattern to set up the options, while the second one sets up the options directly without using a builder pattern.
        //With the second option you can load your own combination of models from the app package or from a local folder. It will give you the full control over the combination of models that you want to use for LPR but also requires more knowledge about the models and their compatibility.
        //The first option is recommended for most use cases as it is easier to use and less error-prone.

        SetupRecognitionOptionsLocalAutoMode();
        //SetupRecognitionOptionsLocalManualMode();

        await Navigation.PushAsync(new LPRMainPage(_recognitionOptions!));
    }

    #region download and prepare models for LPR in Auto mode
    private void SetupRecognitionOptionsDownloadAutoMode()
    {
        if (_recognitionOptions != null)
        {
            _recognitionOptions.Dispose();
            _recognitionOptions = null;
        }

        _recognitionOptions = new RecognitionOptionBuilder()
            .AddLicensePlateDetectionRecognitionOption()
                .UseCroppedImageForRecognition()
                .DoAutomaticDetectionOptimization()
                .SetRecognitionQuality(LicensePlateDetectionRecognitionOptionBuilder.RecognitionQuality.Low)

            .AddLicensePlateRecognitionOverlayDrawingOption()
                .SetOverlays(true)
                .DisplayLicensePlateSurroundingBox()
                .DisplayVehicleSurroundingBox()
                .SetVehicleSurroundingRectColor(Color.FromRgb(0, 255, 0))
                .SetVehicleSurroundingRectStrokeWidth(1)
                .SetLicensePlateSurroundingRectColor(Color.FromRgb(255, 0, 0))
                .SetLicensePlateSurroundingRectStrokeWidth(1)
                .Done()
            .Done()
            .Build();

        var lpRecogOptions = _recognitionOptions.GetLicensePlateDetectionRecognitionOption();
        //This will clear old models from the cache folder. This is optional but recommended to make sure that there is enough space for the new models and to avoid using outdated models.
        lpRecogOptions!.ClearOldModelsFromCacheFolder();

        //register  for events to get insights about the downloading and preparation of the models. This is optional.
        lpRecogOptions.ModelsDownloadStartedEvt += OnModelsDownloadStartedEvt;
        lpRecogOptions.PreparationProgressEvt += OnModelsPreparationProgressEvt;
        lpRecogOptions.ModelsDownloadedEvt += OnModelsDownloadedEvt;

        //Note: 
        // The preparation of the options and the loading of the models can be done in upfront at startup time or after main page load.
        // The CreateAndPrepareModelsAsync has to be done called in a background thread to be able to download the models.
        // You must not call CreateAndPrepareModelsAsync on the UI thread as it will block the UI thread and cause the app to hang. It is recommended to call it in a background thread using Task.Run or similar.

        //This will download the models in the background if they are not already downloaded and prepare them for recognition. This needs to be done before starting the recognition.
        Task.Run(lpRecogOptions.CreateAndPrepareModelsAsync);
    }

    private void OnModelsDownloadStartedEvt(LicensePlateDetectionRecognitionOptions options, int noOfModelsToDownload)
    {
        Console.WriteLine($"Started downloading {noOfModelsToDownload} models");
    }

    private void OnModelsPreparationProgressEvt(LicensePlateDetectionRecognitionOptions options, long readBytes, int indexOfModel, int noOfModelsToDownload)
    {
        Console.WriteLine($"Read {readBytes} bytes for model {indexOfModel + 1} of {noOfModelsToDownload}");
    }

    private void OnModelsDownloadedEvt(LicensePlateDetectionRecognitionOptions options, bool success, int noOfModelsDownloaded)
    {
        Console.WriteLine($"Downloaded {noOfModelsDownloaded} models successfully: {success}");

        options.ModelsDownloadStartedEvt += OnModelsDownloadStartedEvt;
        options.PreparationProgressEvt += OnModelsPreparationProgressEvt;
        options.ModelsDownloadedEvt += OnModelsDownloadedEvt;
    }

    #endregion

    #region setup models from local image in automatic mode

    private void SetupRecognitionOptionsLocalAutoMode()
    {
        _recognitionOptions = new RecognitionOptionBuilder()
            .AddLicensePlateDetectionRecognitionOption()
                .UseCroppedImageForRecognition()
                .DoAutomaticDetectionOptimization()
                .UseLocalModelsForRecognitionMode(LicensePlateDetectionRecognitionOptions.RecognitionModeType.CountryLicencePlateThenVehicle)
                    .UseLocalLicensePlateModelFromType(LPModelType.LPModelType320n)
                    .UseLocalLicensePlateTextModelFromType(LPTextModelType.LPTextModelType320n)
                    .UseLocalVehicleModelFromType(VehicleModelType.VehicleModelType320n)
                    .Done()
                .ConfigureOption(option =>
                {
                    option.ClearOldModelsFromCacheFolder();
                    option.CreateAndPrepareModelsAsync().GetAwaiter().GetResult();
                })

                .AddLicensePlateRecognitionOverlayDrawingOption()
                    .SetOverlays(true)
                    .DisplayLicensePlateSurroundingBox()
                    .DisplayVehicleSurroundingBox()
                    .SetVehicleSurroundingRectColor(Color.FromRgb(0, 255, 0))
                    .SetVehicleSurroundingRectStrokeWidth(1)
                    .SetLicensePlateSurroundingRectColor(Color.FromRgb(255, 0, 0))
                    .SetLicensePlateSurroundingRectStrokeWidth(1)
                    .Done()
                .Done()
            .Build();
    }

    #endregion

    #region setup models from local image in manual mode

    private void SetupRecognitionOptionsLocalManualMode()
    {
        var lpModelName = LicensePlateDetectionConstants.LicensePlateModelFileName320N + LicensePlateDetectionConstants.ModelExtension;
        var textModelName = LicensePlateDetectionConstants.TextModelFileName320N + LicensePlateDetectionConstants.ModelExtension;
        var vehicleModelName = LicensePlateDetectionConstants.VehicleModelFileName320N + LicensePlateDetectionConstants.ModelExtension;

        var plateModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(lpModelName).GetAwaiter().GetResult(),
            lpModelName);
        var textModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(textModelName).GetAwaiter().GetResult(),
            textModelName);
        var vehicleModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(vehicleModelName).GetAwaiter().GetResult(),
            vehicleModelName);

        _recognitionOptions = new RecognitionOptionBuilder()
            .AddLicensePlateDetectionRecognitionOption()
            .UseCroppedImageForRecognition()
                .DoAutomaticDetectionOptimization()
                .UseRecognitionModeCountryLicencePlateThenVehicle(plateModelStream, textModelStream, vehicleModelStream)
                .ConfigureOption(option =>
                {
                    option.CreateAndPrepareModelsAsync().GetAwaiter().GetResult();
                })
                .AddLicensePlateRecognitionOverlayDrawingOption()
                    .EnableOverlays()
                    .DisplayLicensePlateSurroundingBox()
                    .DisplayVehicleSurroundingBox()
                    .SetVehicleSurroundingRectColor(Color.FromRgb(0, 255, 0))
                    .SetVehicleSurroundingRectStrokeWidth(1)
                    .SetLicensePlateSurroundingRectColor(Color.FromRgb(255, 0, 0))
                    .SetLicensePlateSurroundingRectStrokeWidth(1)
                    .Done()
                .Done()
            .Build();
    }
    #endregion

    private List<RecognitionOptions>? _recognitionOptions;
}