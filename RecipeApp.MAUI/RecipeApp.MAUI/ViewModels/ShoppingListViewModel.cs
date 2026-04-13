using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.MAUI.Models;
using RecipeApp.MAUI.Services;
using System.Collections.ObjectModel;
using System.Text;

namespace RecipeApp.MAUI.ViewModels
{
    public partial class ShoppingListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly RecipeApiService _recipeApiService;

        public ObservableCollection<ShoppingItem> ShoppingItems { get; } = new();

        [ObservableProperty]
        private bool _hasNoItems;

        [ObservableProperty]
        private string _weekRangeText = string.Empty;

        public ShoppingListViewModel(DatabaseService databaseService, RecipeApiService recipeApiService)
        {
            _databaseService = databaseService;
            _recipeApiService = recipeApiService;
            Title = "Shopping List";
        }

        [RelayCommand]
        public async Task GenerateShoppingListAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var today = DateTime.Today;
                var daysFromMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
                var weekStart = today.AddDays(-daysFromMonday);
                var weekEnd = weekStart.AddDays(7);

                WeekRangeText = $"{weekStart:dd MMM} - {weekEnd.AddDays(-1):dd MMM yyyy}";

                var mealPlans = await _databaseService.GetWeekMealPlansAsync(weekStart);

                if (!mealPlans.Any())
                {
                    ShoppingItems.Clear();
                    HasNoItems = true;
                    return;
                }

                // Get favourites and all API recipes
                var favourites = await _databaseService.GetFavouritesAsync();
                var apiRecipes = await _recipeApiService.GetRecipesAsync();

                // Merge both sources into one lookup
                var allKnownRecipes = favourites
                    .Concat(apiRecipes)
                    .GroupBy(r => r.Name.ToLowerInvariant().Trim())
                    .Select(g => g.First())
                    .ToList();

                var allIngredients = new List<string>();

                foreach (var meal in mealPlans)
                {
                    var recipe = allKnownRecipes.FirstOrDefault(r =>
                        r.Name.Equals(meal.RecipeName, StringComparison.OrdinalIgnoreCase));

                    if (recipe?.Ingredients != null && recipe.Ingredients.Any())
                        allIngredients.AddRange(recipe.Ingredients);
                    else
                        allIngredients.Add($"Ingredients for: {meal.RecipeName}");
                }

                ShoppingItems.Clear();

                var grouped = allIngredients
                    .GroupBy(i => i.ToLowerInvariant().Trim())
                    .Select(g => g.First().Trim())
                    .OrderBy(i => i)
                    .ToList();

                foreach (var ingredient in grouped)
                    ShoppingItems.Add(new ShoppingItem { Name = ingredient });

                HasNoItems = !ShoppingItems.Any();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error generating shopping list: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void ToggleItem(ShoppingItem item)
        {
            if (item is null) return;
            item.IsChecked = !item.IsChecked;
        }

        [RelayCommand]
        public async Task CopyToClipboardAsync()
        {
            if (!ShoppingItems.Any()) return;

            var sb = new StringBuilder();
            sb.AppendLine($"🛒 Shopping List ({WeekRangeText})");
            sb.AppendLine();

            foreach (var item in ShoppingItems)
            {
                var check = item.IsChecked ? "✅" : "☐";
                sb.AppendLine($"{check} {item.Name}");
            }

            await Clipboard.SetTextAsync(sb.ToString());
            await Shell.Current.DisplayAlert("Copied!", "Shopping list copied to clipboard.", "OK");
        }

        [RelayCommand]
        public void ClearChecked()
        {
            var checkedItems = ShoppingItems.Where(i => i.IsChecked).ToList();
            foreach (var item in checkedItems)
                ShoppingItems.Remove(item);

            HasNoItems = !ShoppingItems.Any();
        }
    }

    public partial class ShoppingItem : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private bool _isChecked;
    }
}