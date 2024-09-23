using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace NetMaui_samples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraMultiPage : ContentPage
    {
        public CameraMultiPage()
        {
            InitializeComponent();
            BindingContext = this;

            Title = "Interval";
            ImageCaptureView.PropertyChanged += ImageCaptureViewOnPropertyChanged;
        }

        protected override async void OnAppearing()
        { 
            base.OnAppearing(); 
            await ImageCaptureView.RequestAllPermissionAsync();

            ImageCaptureView.InitializedEvt += async () =>
            {
                await ImageCaptureView.OpenCameraAsync();
            };
        }

        protected override void OnDisappearing()
        {
            ImageCaptureView.Terminate();
            base.OnDisappearing();
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
            await IntervalButton.ScaleTo(0.9, 50);
        }

        private void CaptureButton_OnReleased(object sender, EventArgs e)
        {
            IntervalButton.ScaleTo(1, 50);
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
    }
}