using RecipeApp.MAUI.Views;
using RecipeApp.MAUI.ViewModels;
using RecipeApp.MAUI.Services;

namespace RecipeApp.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 App() START");

                // Create API service
                System.Diagnostics.Debug.WriteLine("🔵 Creating RecipeApiService...");
                var apiService = new RecipeApiService();
                System.Diagnostics.Debug.WriteLine("🔵 ✅ RecipeApiService created");

                // Create Database service
                System.Diagnostics.Debug.WriteLine("🔵 Creating DatabaseService...");
                var dbService = new DatabaseService();
                System.Diagnostics.Debug.WriteLine("🔵 ✅ DatabaseService created");

                // Create ViewModel
                System.Diagnostics.Debug.WriteLine("🔵 Creating HomeViewModel...");
                var homeViewModel = new HomeViewModel(apiService, dbService);
                System.Diagnostics.Debug.WriteLine("🔵 ✅ HomeViewModel created");

                // Create HomePage
                System.Diagnostics.Debug.WriteLine("🔵 Creating HomePage...");
                var homePage = new HomePage(homeViewModel);
                System.Diagnostics.Debug.WriteLine("🔵 ✅ HomePage created");

                // Set as MainPage
                System.Diagnostics.Debug.WriteLine("🔵 Setting MainPage...");
                MainPage = homePage;

                System.Diagnostics.Debug.WriteLine("🔵 🎉 App() END - SUCCESS!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"💥 INNER: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"💥 STACK: {ex.StackTrace}");

                // Show error on screen
                MainPage = new ContentPage
                {
                    Content = new ScrollView
                    {
                        Content = new Label
                        {
                            Text = $"ERROR:\n\n{ex.Message}\n\nINNER:\n{ex.InnerException?.Message}\n\nSTACK:\n{ex.StackTrace}",
                            TextColor = Colors.Red,
                            Padding = 20,
                            FontSize = 12
                        }
                    }
                };
            }
        }
    }
}