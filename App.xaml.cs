using NetMaui_samples.Views;

namespace NetMaui_samples;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage());
    }
}
