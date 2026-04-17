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
        private bool _hasNoItems = true;

        [ObservableProperty]
        private string _weekRangeText = string.Empty;

        [ObservableProperty]
        private DateTime _currentWeekStart;

        public ShoppingListViewModel(DatabaseService databaseService, RecipeApiService recipeApiService)
        {
            _databaseService = databaseService;
            _recipeApiService = recipeApiService;
            Title = "Shopping List";

            // Start on current week
            var today = DateTime.Today;
            var daysFromMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            CurrentWeekStart = today.AddDays(-daysFromMonday);
        }

        partial void OnCurrentWeekStartChanged(DateTime value)
        {
            var weekEnd = value.AddDays(6);
            WeekRangeText = $"{value:dd MMM} - {weekEnd:dd MMM yyyy}";
            ShoppingItems.Clear();
            HasNoItems = true;
        }

        [RelayCommand]
        public void PreviousWeek()
        {
            CurrentWeekStart = CurrentWeekStart.AddDays(-7);
        }

        [RelayCommand]
        public void NextWeek()
        {
            CurrentWeekStart = CurrentWeekStart.AddDays(7);
        }

        [RelayCommand]
        public async Task GenerateShoppingListAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var mealPlans = await _databaseService.GetWeekMealPlansAsync(CurrentWeekStart);

                if (!mealPlans.Any())
                {
                    ShoppingItems.Clear();
                    HasNoItems = true;
                    await Shell.Current.DisplayAlert("No Meals",
                        "No meals planned for this week. Add meals in the Meal Planner first.", "OK");
                    return;
                }

                var favourites = await _databaseService.GetFavouritesAsync();
                var apiRecipes = await _recipeApiService.GetRecipesAsync();

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