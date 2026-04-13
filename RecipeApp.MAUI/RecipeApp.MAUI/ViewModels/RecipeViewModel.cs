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

        public List<string> CuisineOptions { get; } = new() { "All", "Italian", "Asian", "American", "Mexican", "Indian", "Mediterranean", "French", "Other" };
        public List<string> DifficultyOptions { get; } = new() { "All", "Easy", "Medium", "Hard" };

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
                var recipes = await _recipeApiService.GetRecipesAsync();
                _allRecipes = recipes;
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

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(r =>
                    (r.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.CuisineType?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

            if (SelectedCuisine != "All")
                filtered = filtered.Where(r =>
                    r.CuisineType?.Equals(SelectedCuisine, StringComparison.OrdinalIgnoreCase) ?? false);

            if (SelectedDifficulty != "All")
                filtered = filtered.Where(r =>
                    r.Difficulty?.Equals(SelectedDifficulty, StringComparison.OrdinalIgnoreCase) ?? false);

            Recipes.Clear();
            foreach (var r in filtered)
                Recipes.Add(r);
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