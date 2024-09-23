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

    private void OnLPRClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new LPRMainPage());
    }
}