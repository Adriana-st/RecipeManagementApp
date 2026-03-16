using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RecipeApp.Models;
using RecipeApp.Services;

namespace RecipeApp.Views
{
    /// <summary>
    /// Shopping list page - generates shopping list from meal plan
    /// Demonstrates: LINQ aggregation, data transformation, ObservableCollection
    /// </summary>
    public partial class ShoppingListPage : Page
    {
        private readonly MealPlanService _mealPlanService;
        private readonly RecipeService _recipeService;
        private DateTime _currentWeekStart;
        private ObservableCollection<ShoppingListItem> _shoppingItems;

        public ShoppingListPage()
        {
            InitializeComponent();
            _mealPlanService = new MealPlanService();
            _recipeService = new RecipeService();
            _shoppingItems = new ObservableCollection<ShoppingListItem>();

            // Start with current week (Monday)
            _currentWeekStart = GetMonday(DateTime.Now);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadWeekInfo();
        }

        /// <summary>
        /// Get the Monday of the week containing the given date
        /// </summary>
        private DateTime GetMonday(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return date.Date.AddDays(-daysToSubtract);
        }

        /// <summary>
        /// Load week information
        /// </summary>
        private async System.Threading.Tasks.Task LoadWeekInfo()
        {
            try
            {
                var weekEnd = _currentWeekStart.AddDays(6);
                WeekRangeText.Text = $"{_currentWeekStart:MMM d} - {weekEnd:MMM d, yyyy}";

                // Get meal plans for the week
                var mealPlans = await _mealPlanService.GetWeekMealPlansAsync(_currentWeekStart);
                MealCountText.Text = $"{mealPlans.Count} meal{(mealPlans.Count == 1 ? "" : "s")} planned";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading week info: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Generate shopping list from meal plans
        /// Demonstrates: Complex LINQ queries with grouping and aggregation
        /// </summary>
        private async void GenerateList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get meal plans for the week
                var mealPlans = await _mealPlanService.GetWeekMealPlansAsync(_currentWeekStart);

                if (mealPlans.Count == 0)
                {
                    // Show empty state
                    EmptyStatePanel.Visibility = Visibility.Visible;
                    ShoppingListPanel.Visibility = Visibility.Collapsed;
                    return;
                }

                // Get all recipes from favourites
                var allRecipes = await _recipeService.GetFavouritesAsync();

                // Get unique recipe IDs from meal plans
                var recipeIds = mealPlans.Select(m => m.RecipeId).Distinct().ToList();

                // Get the actual recipes
                var plannedRecipes = allRecipes.Where(r => recipeIds.Contains(r.Id)).ToList();

                // Aggregate all ingredients
                var ingredientUsage = new Dictionary<string, List<string>>();

                foreach (var recipe in plannedRecipes)
                {
                    if (recipe.Ingredients != null)
                    {
                        foreach (var ingredient in recipe.Ingredients)
                        {
                            var normalizedIngredient = ingredient.Trim().ToLower();

                            if (!ingredientUsage.ContainsKey(normalizedIngredient))
                            {
                                ingredientUsage[normalizedIngredient] = new List<string>();
                            }

                            if (!ingredientUsage[normalizedIngredient].Contains(recipe.Name))
                            {
                                ingredientUsage[normalizedIngredient].Add(recipe.Name);
                            }
                        }
                    }
                }

                // Create shopping list items (sorted alphabetically)
                _shoppingItems.Clear();
                foreach (var item in ingredientUsage.OrderBy(i => i.Key))
                {
                    var usedIn = item.Value.Count == 1
                        ? $"Used in: {item.Value[0]}"
                        : $"Used in {item.Value.Count} recipes";

                    _shoppingItems.Add(new ShoppingListItem
                    {
                        Name = CapitalizeFirst(item.Key),
                        UsedInRecipes = usedIn,
                        IsChecked = false
                    });
                }

                IngredientsList.ItemsSource = _shoppingItems;

                // Show shopping list
                EmptyStatePanel.Visibility = Visibility.Collapsed;
                ShoppingListPanel.Visibility = Visibility.Visible;

                MessageBox.Show($"Shopping list generated with {_shoppingItems.Count} items!",
                              "Success",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating shopping list: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Capitalize first letter of string
        /// </summary>
        private string CapitalizeFirst(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// Copy shopping list to clipboard
        /// </summary>
        private void CopyList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("🛒 SHOPPING LIST");
                sb.AppendLine($"Week of {WeekRangeText.Text}");
                sb.AppendLine();
                sb.AppendLine("────────────────────────────");
                sb.AppendLine();

                foreach (var item in _shoppingItems)
                {
                    var checkbox = item.IsChecked ? "☑" : "☐";
                    sb.AppendLine($"{checkbox} {item.Name}");
                }

                sb.AppendLine();
                sb.AppendLine("────────────────────────────");
                sb.AppendLine($"Total Items: {_shoppingItems.Count}");

                Clipboard.SetText(sb.ToString());

                MessageBox.Show("Shopping list copied to clipboard!",
                              "Copied",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error");
            }
        }

        private async void PreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            await LoadWeekInfo();
        }

        private async void ThisWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = GetMonday(DateTime.Now);
            await LoadWeekInfo();
        }

        private async void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            await LoadWeekInfo();
        }

        private void GoToMealPlanner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MealPlannerPage());
        }
    }

    /// <summary>
    /// Represents a shopping list item with checkbox
    /// </summary>
    public class ShoppingListItem
    {
        public string Name { get; set; }
        public string UsedInRecipes { get; set; }
        public bool IsChecked { get; set; }
    }
}

