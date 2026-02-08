using NetMaui_samples.Views;

namespace NetMaui_samples;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()));
    }
}
