namespace RecipeApp.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for pages not in the flyout
            Routing.RegisterRoute(nameof(Views.RecipeDetailPage), typeof(Views.RecipeDetailPage));
        }
    }
}
