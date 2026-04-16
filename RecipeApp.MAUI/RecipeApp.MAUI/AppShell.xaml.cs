namespace RecipeApp.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for pages not in the flyout
            Routing.RegisterRoute(nameof(Views.RecipeDetailPage), typeof(Views.RecipeDetailPage));

            // Set navigation style based on device
            SetNavigationStyle();

            // Also update when window size changes
            this.SizeChanged += (s, e) => SetNavigationStyle();
        }
        private void SetNavigationStyle()
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop ||
                DeviceInfo.Idiom == DeviceIdiom.Tablet ||
                (DeviceInfo.Platform == DevicePlatform.WinUI && Application.Current.Windows[0].Width > 700))
            {
                Shell.SetFlyoutBehavior(this, FlyoutBehavior.Locked);
            }
            else
            {
                Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
            }
        }
    }
}
