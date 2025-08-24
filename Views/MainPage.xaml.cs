using CapCognition.Maui.Core.Shared.Common;
using CapCognition.Maui.LPR;
using CapCognition.Maui.LPR.Shared;

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

    private async void OnLPRClicked(object sender, EventArgs e)
    {
        //Note: 
        // The preparation of the options and the loading of the models can be done in upfront and in a background thread.
        // For simplicity, we do it here in the UI thread and right before opening the page.


        var lpModelName = LicensePlateDetectionConstants.LicensePlateModelFileName320N + LicensePlateDetectionConstants.ModelExtension;
        var ncLpModelName = LicensePlateDetectionConstants.LicensePlateNoCountryModelFileName320N + LicensePlateDetectionConstants.ModelExtension;
        var textModelName = LicensePlateDetectionConstants.TextModelFileName320N + LicensePlateDetectionConstants.ModelExtension;
        var vehicleModelName = LicensePlateDetectionConstants.VehicleModelFileName320N + LicensePlateDetectionConstants.ModelExtension;


        var plateModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(lpModelName).GetAwaiter().GetResult(),
            lpModelName);
        var ncPlateModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(ncLpModelName).GetAwaiter().GetResult(),
            ncLpModelName);
        var textModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(textModelName).GetAwaiter().GetResult(),
            textModelName);
        var vehicleModelStream = new LicensePlateDetectionRecognitionOptions.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(vehicleModelName).GetAwaiter().GetResult(),
            vehicleModelName);

        var recogOptions = new RecognitionOptionBuilder()
                .AddLicensePlateDetectionRecognitionOption()
                    .UseCroppedImageForRecognition()
                    .DoAutomaticDetectionOptimization()
                    .SetRecognitionQuality(LicensePlateDetectionRecognitionOptionBuilder.RecognitionQuality.Low)
                    //.UseRecognitionModeCountryLicencePlateThenVehicle(plateModelStream, textModelStream, vehicleModelStream)
                    .UseRecognitionModeNoCountryLicencePlateThenVehicle(ncPlateModelStream, textModelStream, vehicleModelStream)
                    //.UseRecognitionModeCountryLicencePlateOnly(plateModelStream, textModelStream)
                    //.UseRecognitionModeVehicleOnly(vehicleModelStream)
                    //.UseRecognitionModeVehicleThenCountryLicencePlate(plateModelStream, textModelStream, vehicleModelStream)
                    //.UseRecognitionModeVehicleThenNoCountryLicencePlate(ncPlateModelStream, textModelStream, vehicleModelStream)
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

        await Navigation.PushAsync(new LPRMainPage(recogOptions));
    }
}