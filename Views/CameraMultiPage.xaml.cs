using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;

namespace NetMaui_samples.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CameraMultiPage : DisposableContentPage
{
    public CameraMultiPage()
    {
        InitializeComponent();
        BindingContext = this;

        Title = "Interval";
        ImageCaptureView.PropertyChanged += ImageCaptureViewOnPropertyChanged;
        Loaded += OnLoaded;
    }

    protected override async void OnAppearing()
    { 
        base.OnAppearing();

        OpenCamera();
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        var result = await ImageCaptureView.RequestAllPermissionAsync();
        if (!result)
        {
            await Navigation.PopAsync();
            return;
        }

        OpenCamera();
    }

    protected override void OnDisappearing()
    {
        ImageCaptureView.CloseCamera();
        base.OnDisappearing();
    }

    private void OpenCamera()
    {
        if (!ImageCaptureView.Initialized || ImageCaptureView.CameraIsOpen)
        {
            return;
        }

        ImageCaptureView.CameraOpenedEvt -= OnCameraOpened;
        ImageCaptureView.CameraOpenedEvt += OnCameraOpened;
        var success = ImageCaptureView.OpenCamera();
    }

    private void OnCameraOpened(bool success)
    {
        ImageCaptureView.CameraOpenedEvt -= OnCameraOpened;
        if (!success)
        {
            Console.WriteLine("Camera could not be opened!!!");
            return;
        }
        Console.WriteLine("Camera opened");
    }

    private void CloseCamera()
    {
        if (!ImageCaptureView.CameraIsOpen)
        {
            return;
        }

        ImageCaptureView.CloseCamera();
    }

    public int FlashOpacity => ImageCaptureView.TorchIsOn ? 100 : 50;

    public string IntervalButtonText => ImageCaptureView.CapturePending ? "Stop Interval" : "Start Interval";

    private void OnStartIntervalClicked(object sender, EventArgs e)
    {
        if (!ImageCaptureView.CapturePending)
        {
            ImageCaptureView.ImageCaptured += OnIntervalImageCaptured;
            ImageCaptureView.StartContinuousBitmapRetrieval();
        }
        else
        {
            ImageCaptureView.StopContinuousBitmapRetrieval();
            ImageCaptureView.ImageCaptured -= OnIntervalImageCaptured;
        }
    }

    private void OnIntervalImageCaptured(SKBitmap bmp)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PreviewImage.Source = (SKBitmapImageSource)bmp;
        });
    }

    private void ImageCaptureViewOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageCaptureView.TorchIsOn))
        {
            OnPropertyChanged(nameof(FlashOpacity));
        }

        if (e.PropertyName == nameof(ImageCaptureView.CapturePending))
        {
            OnPropertyChanged(nameof(IntervalButtonText));
        }
    }

    private async void CaptureButton_OnPressed(object sender, EventArgs e)
    {
        await IntervalButton.ScaleToAsync(0.9, 50);
    }

    private void CaptureButton_OnReleased(object sender, EventArgs e)
    {
        IntervalButton.ScaleToAsync(1, 50);
    }

    private void OnToggleTorch(object sender, EventArgs e)
    {
        ImageCaptureView.SetTorch(!ImageCaptureView.TorchIsOn);
    }

    private async void OnChangeCamera(object sender, EventArgs e)
    {
        ImageCaptureView.CloseCamera();
        ImageCaptureView.UseFrontCamera = !ImageCaptureView.UseFrontCamera;
        await ImageCaptureView.OpenCameraAsync();
    }

    private void OnPreviewImageTapped(object sender, EventArgs e)
    {
        if (PreviewImage.Source != null)
        {
            Navigation.PushAsync(new PictureDetailPage(PreviewImage.Source));
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        Loaded -= OnLoaded;
        ImageCaptureView.Terminate();
    }
}