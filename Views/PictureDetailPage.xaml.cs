namespace NetMaui_samples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PictureDetailPage : ContentPage
    {
        public PictureDetailPage(ImageSource imageSource)
        {
            InitializeComponent();
            BindingContext = this;

            PreviewImage.Source = imageSource;
            Title = "Picture Details";
        }
    }
}