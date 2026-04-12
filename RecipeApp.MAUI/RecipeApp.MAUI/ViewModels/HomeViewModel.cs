using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.MAUI.Services;
using System.Threading.Tasks;

namespace RecipeApp.MAUI.ViewModels
{
    /// <summary>
    /// ViewModel for Home page
    /// Demonstrates: MVVM commands, async operations
    /// </summary>
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly RecipeApiService _recipeApiService;
        private readonly DatabaseService _databaseService;

        private int _totalRecipes;
        public int TotalRecipes
        {
            get => _totalRecipes;
            set => SetProperty(ref _totalRecipes, value);
        }

        private int _favouriteCount;
        public int FavouriteCount
        {
            get => _favouriteCount;
            set => SetProperty(ref _favouriteCount, value);
        }

        private string _welcomeMessage = "Discover, organise, and manage your favourite recipes";
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public HomeViewModel(RecipeApiService recipeApiService, DatabaseService databaseService)
        {
            System.Diagnostics.Debug.WriteLine("🔵 HomeViewModel constructor START");

            _recipeApiService = recipeApiService;
            _databaseService = databaseService;

            System.Diagnostics.Debug.WriteLine("🔵 Setting Title...");
            Title = "Recipe Management App";

            System.Diagnostics.Debug.WriteLine("🔵 HomeViewModel constructor END");
        }

        /// <summary>
        /// Load stats when page appears
        /// </summary>
        public async Task LoadStatsAsync()
        {
            System.Diagnostics.Debug.WriteLine("🔵 LoadStatsAsync START");

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                // Get favourite count
                var favourites = await _databaseService.GetFavouritesAsync();
                FavouriteCount = favourites.Count;

                // Get total recipes from API
                var recipes = await _recipeApiService.GetRecipesAsync();
                TotalRecipes = recipes.Count;

                System.Diagnostics.Debug.WriteLine($"🔵 Loaded: {TotalRecipes} total, {FavouriteCount} favourites");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading stats: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}