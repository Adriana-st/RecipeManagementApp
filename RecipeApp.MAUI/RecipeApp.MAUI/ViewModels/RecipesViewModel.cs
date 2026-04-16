using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.MAUI.Models;
using RecipeApp.MAUI.Services;
using RecipeApp.MAUI.Views;
using System.Collections.ObjectModel;

namespace RecipeApp.MAUI.ViewModels
{
    public partial class RecipesViewModel : BaseViewModel
    {
        private readonly RecipeApiService _recipeApiService;
        private readonly DatabaseService _databaseService;

        private List<Recipe> _allRecipes = new();

        public ObservableCollection<Recipe> Recipes { get; } = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedCuisine = "All";

        [ObservableProperty]
        private string _selectedDifficulty = "All";

        [ObservableProperty]
        private string _selectedProtein = "All";

        [ObservableProperty]
        private int _maxTime = 300;

        [ObservableProperty]
        private bool _filtersVisible;

        public List<string> CuisineOptions { get; } = new()
        {
            "All", "Italian", "Asian", "Chinese", "Japanese", "Thai",
            "Indian", "Mexican", "American", "Mediterranean",
            "French", "Greek", "Middle Eastern", "Other"
        };

        public List<string> DifficultyOptions { get; } = new()
        {
            "All", "Easy", "Medium", "Hard"
        };

        public List<string> ProteinOptions { get; } = new()
        {
            "All", "Chicken", "Beef", "Pork", "Fish", "Seafood",
            "Lamb", "Turkey", "Tofu", "Eggs", "Vegetarian"
        };

        public RecipesViewModel(RecipeApiService recipeApiService, DatabaseService databaseService)
        {
            _recipeApiService = recipeApiService;
            _databaseService = databaseService;
            Title = "Recipes";
        }

        [RelayCommand]
        public async Task LoadRecipesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var apiTask = _recipeApiService.GetRecipesAsync();
                var customTask = _databaseService.GetCustomRecipesAsync();

                await Task.WhenAll(apiTask, customTask);

                var apiRecipes = apiTask.Result;
                var customRecipes = customTask.Result;

                _allRecipes = customRecipes.Concat(apiRecipes).ToList();

                // Set favourite status for each recipe
                foreach (var recipe in _allRecipes)
                {
                    recipe.IsFavourite = await _databaseService.IsRecipeSavedAsync(recipe.ApiId);
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading recipes: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Could not load recipes. Check your connection.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void ApplyFilters()
        {
            var filtered = _allRecipes.AsEnumerable();

            // Search by name or ingredient
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(r =>
                    (r.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Ingredients?.Any(i => i.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ?? false));

            // Cuisine filter
            if (SelectedCuisine != "All")
            {
                if (SelectedCuisine == "Other")
                {
                    // Show recipes whose cuisine doesn't match any named option
                    var namedCuisines = CuisineOptions
                        .Where(c => c != "All" && c != "Other")
                        .ToList();

                    filtered = filtered.Where(r =>
                        string.IsNullOrWhiteSpace(r.CuisineType) ||
                        r.CuisineType.Equals("Other", StringComparison.OrdinalIgnoreCase) ||
                        !namedCuisines.Any(c => c.Equals(r.CuisineType, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    filtered = filtered.Where(r =>
                        r.CuisineType?.Equals(SelectedCuisine, StringComparison.OrdinalIgnoreCase) ?? false);
                }
            }

            // Difficulty filter
            if (SelectedDifficulty != "All")
                filtered = filtered.Where(r =>
                    r.Difficulty?.Equals(SelectedDifficulty, StringComparison.OrdinalIgnoreCase) ?? false);

            // Protein filter
            if (SelectedProtein != "All")
            {
                filtered = filtered.Where(r =>
                    (r.Ingredients?.Any(i =>
                        i.Contains(SelectedProtein, StringComparison.OrdinalIgnoreCase)) ?? false) ||
                    (r.Name?.Contains(SelectedProtein, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            // Max time filter
            if (MaxTime < 300)
                filtered = filtered.Where(r => r.TotalTimeMinutes <= MaxTime);

            Recipes.Clear();
            foreach (var r in filtered)
                Recipes.Add(r);
        }

        [RelayCommand]
        public void ResetFilters()
        {
            SearchText = string.Empty;
            SelectedCuisine = "All";
            SelectedDifficulty = "All";
            SelectedProtein = "All";
            MaxTime = 300;
            ApplyFilters();
        }

        [RelayCommand]
        public async Task ToggleFavouriteAsync(Recipe recipe)
        {
            if (recipe is null) return;

            try
            {
                if (recipe.IsFavourite)
                {
                    var favourites = await _databaseService.GetFavouritesAsync();
                    var existing = favourites.FirstOrDefault(r => r.ApiId == recipe.ApiId);
                    if (existing != null)
                        await _databaseService.DeleteRecipeAsync(existing);
                    recipe.IsFavourite = false;
                }
                else
                {
                    await _databaseService.SaveRecipeAsync(recipe);
                    recipe.IsFavourite = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error toggling favourite: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task GoToRecipeDetailAsync(Recipe recipe)
        {
            if (recipe is null) return;

            await Shell.Current.GoToAsync(nameof(RecipeDetailPage), new Dictionary<string, object>
            {
                { "Recipe", recipe }
            });
        }
    }
}