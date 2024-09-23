using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;

namespace NetMaui_samples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraSinglePage : ContentPage
    {
        public CameraSinglePage()
        {
            InitializeComponent();
            BindingContext = this;

            Title = "Camera Single";
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
            ImageCaptureView.CloseCamera();
            base.OnDisappearing();
        }

        public int FlashOpacity => ImageCaptureView.TorchIsOn ? 100 : 50;

        private void OnTakePhotoClicked(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                var bmp = await ImageCaptureView.GetCurrentImageAsync();
                if (bmp is null)
                {
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PreviewImage.Source = (SKBitmapImageSource)bmp;
                });
            });
        }

        private void ImageCaptureViewOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageCaptureView.TorchIsOn))
            {
                OnPropertyChanged(nameof(FlashOpacity));
            }
        }

        private async void CaptureButton_OnPressed(object sender, EventArgs e)
        {
            await CaptureButton.ScaleTo(0.9, 50);
        }

        private void CaptureButton_OnReleased(object sender, EventArgs e)
        {
            CaptureButton.ScaleTo(1, 50);
        }

        private void OnToggleTorch(object sender, EventArgs e)
        {
            ImageCaptureView.SetTorch(!ImageCaptureView.TorchIsOn);
            OnPropertyChanged(nameof(FlashOpacity));
        }

        private async void OnChangeCamera(object sender, EventArgs e)
        {
            ImageCaptureView.CloseCamera();
            ImageCaptureView.UseFrontCamera = !ImageCaptureView.UseFrontCamera;

            await ImageCaptureView.OpenCameraAsync();
        }
    }
}