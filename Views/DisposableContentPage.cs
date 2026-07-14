namespace NetMaui_samples.Views;

public abstract partial class DisposableContentPage : ContentPage, IDisposable
{
    protected override void OnAppearing()
    {
        base.OnAppearing();

        _hasAppeared = true;

        if (!_shellNavigationSubscribed && Shell.Current is not null)
        {
            Shell.Current.Navigated += OnShellNavigated;
            _shellNavigationSubscribed = true;
        }
    }

    public virtual void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }
        _isDisposed = true;
        Shell.Current.Navigated -= OnShellNavigated;
        _shellNavigationSubscribed = false;
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if (!_hasAppeared)
        {
            return;
        }

        if (ReferenceEquals(Shell.Current?.CurrentPage, this))
        {
            return;
        }

        if (IsStillInNavigationStack())
        {
            return;
        }

        Dispose();
    }

    private bool IsStillInNavigationStack()
    {
        if (Shell.Current is null)
        {
            return false;
        }

        return Shell.Current.Navigation.NavigationStack
                   .Any(page => ReferenceEquals(page, this))
               || Shell.Current.Navigation.ModalStack
                   .Any(page => ReferenceEquals(page, this));
    }

    protected bool IsDisposed => _isDisposed;

    private bool _shellNavigationSubscribed;
    private bool _hasAppeared;
    private bool _isDisposed;
}