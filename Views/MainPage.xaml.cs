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
        // The preparation of the options and the loading of the models can be done in upfront and in a background thread.
        // For simplicity, we do it here in the UI thread and right before opening the page.
        var options = new LicensePlateDetectionRecognitionOption()
        {
            EnableOverlays = true,
            DisplayLicensePlateSurroundingBox = false,
            DisplayVehicleSurroundingBox = false,
            UseCroppedImageForRecognition = true,
            DoAutomaticDetectionOptimization = false,
            DetectVehicleType = false
        };

        var lpModelName = LicensePlateDetectionConstants.LicensePlateModelFileName320N + ".ccml";
        var textModelName = LicensePlateDetectionConstants.TextModelFileName320N + ".ccml";
        var vehicleModelName = LicensePlateDetectionConstants.VehicleModelFileName320N + ".ccml";

        var plateModelStream = new LicensePlateDetectionRecognitionOption.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(lpModelName).GetAwaiter().GetResult(),
            lpModelName);
        var textModelStream = new LicensePlateDetectionRecognitionOption.StreamInfo(
            FileSystem.Current.OpenAppPackageFileAsync(textModelName).GetAwaiter().GetResult(),
            textModelName);

        if (options.DetectVehicleType)
        {
            var vehicleModelStream = new LicensePlateDetectionRecognitionOption.StreamInfo(
                FileSystem.Current.OpenAppPackageFileAsync(vehicleModelName).GetAwaiter().GetResult(),
                vehicleModelName);

            options.SetModelStreams(plateModelStream, textModelStream, vehicleModelStream);
        }
        else
        {
            options.SetModelStreams(plateModelStream, textModelStream);
        }
        await options.CreateAndPrepareModelsAsync();

        await Navigation.PushAsync(new LPRMainPage(options));
    }
}