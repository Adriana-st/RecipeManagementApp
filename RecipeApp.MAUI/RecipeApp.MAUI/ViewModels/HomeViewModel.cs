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
        private readonly IRecipeApiService _recipeApiService;
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _totalRecipes;

        [ObservableProperty]
        private int _favouriteCount;

        [ObservableProperty]
        private string _welcomeMessage = "Discover, organise, and manage your favourite recipes";

        public HomeViewModel(IRecipeApiService recipeApiService, DatabaseService databaseService)
        {
            _recipeApiService = recipeApiService;
            _databaseService = databaseService;
            Title = "Recipe Management App";

        }

        /// <summary>
        /// Load stats when page appears
        /// </summary>
        public async Task LoadStatsAsync()
        {
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