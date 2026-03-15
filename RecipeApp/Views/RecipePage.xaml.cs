using RecipeApp.Models;
using RecipeApp.Services;
using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace RecipeApp.Views
{
    /// <summary>
    /// Page for browsing recipes from API
    /// </summary>
    public partial class RecipePage : Page
    {
        private readonly RecipeApiService _apiService;
        private List<Recipe> _allRecipes; // Store all recipes
        private ObservableCollection<Recipe> _displayedRecipes; // Recipes currently shown

        public RecipePage()
        {
            InitializeComponent();
            _apiService = new RecipeApiService();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRecipes();
        }

        private async Task LoadRecipes()
        {
            try
            {
                // Show loading
                LoadingPanel.Visibility = Visibility.Visible;
                RecipesPanel.Visibility = Visibility.Collapsed;
                ErrorPanel.Visibility = Visibility.Collapsed;

                Console.WriteLine("Starting to fetch recipes...");

                // Fetch recipes from API
                _allRecipes = await _apiService.GetRecipesAsync();

                Console.WriteLine($"Received {_allRecipes.Count} recipes");

                // Process each recipe for display
                foreach (var recipe in _allRecipes)
                {
                    if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
                    {
                        var ingredientCount = Math.Min(5, recipe.Ingredients.Count);
                        recipe.IngredientsDisplay = string.Join(", ",
                            recipe.Ingredients.Take(ingredientCount));

                        if (recipe.Ingredients.Count > 5)
                        {
                            recipe.IngredientsDisplay += $" ... and {recipe.Ingredients.Count - 5} more";
                        }
                    }
                    else
                    {
                        recipe.IngredientsDisplay = "No ingredients listed";
                    }
                }

                // Initialize observable collection
                _displayedRecipes = new ObservableCollection<Recipe>(_allRecipes);

                // Bind recipes to UI
                RecipeList.ItemsSource = _displayedRecipes;
                RecipeCountText.Text = $"Found {_allRecipes.Count} delicious recipes!";

                // Hide loading, show results
                LoadingPanel.Visibility = Visibility.Collapsed;
                RecipesPanel.Visibility = Visibility.Visible;

                Console.WriteLine("Recipes displayed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadRecipes: {ex.Message}");

                // Show error panel
                LoadingPanel.Visibility = Visibility.Collapsed;
                ErrorPanel.Visibility = Visibility.Visible;

                ErrorMessage.Text = ex.Message;
            }
        
        }

        private async void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            await LoadRecipes();
        }

        private void RecipeBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            }
        }

        private void RecipeBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Save recipe to favourites
        /// </summary>
        private async void SaveToFavourites_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the recipe from the button's Tag
                var button = sender as Button;
                var recipe = button?.Tag as Recipe;

                if (recipe == null)
                {
                    MessageBox.Show("Error: Could not get recipe data", "Error");
                    return;
                }

                // Change button to show it's saving
                button.Content = "💾 Saving...";
                button.IsEnabled = false;

                // Save to database
                var recipeService = new RecipeService();
                var success = await recipeService.SaveToFavouritesAsync(recipe);

                if (success)
                {
                    // Success!
                    button.Content = "✅ Saved!";
                    button.Background = new SolidColorBrush(Color.FromRgb(151, 188, 98)); // Green

                    MessageBox.Show($"'{recipe.Name}' saved to favourites!",
                                  "Saved",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
                else
                {
                    // Failed
                    button.Content = "💾 Save to Favourites";
                    button.IsEnabled = true;

                    MessageBox.Show("Failed to save recipe. Please try again.",
                                  "Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Apply all filters - Demonstrates complex LINQ queries
        /// </summary>
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allRecipes == null || _displayedRecipes == null)
                return;

            try
            {
                // Start with all recipes
                var filtered = _allRecipes.AsEnumerable();

                // 1. Search filter (name or ingredients)
                var searchTerm = SearchBox.Text?.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filtered = filtered.Where(r =>
                        r.Name.ToLower().Contains(searchTerm) ||
                        (r.Ingredients != null && r.Ingredients.Any(i => i.ToLower().Contains(searchTerm))));
                }

                // 2. Cuisine filter
                var cuisineItem = CuisineFilter.SelectedItem as ComboBoxItem;
                var cuisine = cuisineItem?.Content.ToString();
                if (cuisine != "All Cuisines" && !string.IsNullOrEmpty(cuisine))
                {
                    filtered = filtered.Where(r =>
                        r.CuisineType != null &&
                        r.CuisineType.Equals(cuisine, StringComparison.OrdinalIgnoreCase));
                }

                // 3. Difficulty filter
                var difficultyItem = DifficultyFilter.SelectedItem as ComboBoxItem;
                var difficulty = difficultyItem?.Content.ToString();
                if (difficulty != "All Levels" && !string.IsNullOrEmpty(difficulty))
                {
                    filtered = filtered.Where(r =>
                        r.Difficulty != null &&
                        r.Difficulty.Equals(difficulty, StringComparison.OrdinalIgnoreCase));
                }

                // 4. Time filter
                var maxTime = (int)TimeSlider.Value;
                filtered = filtered.Where(r => r.TotalTimeMinutes <= maxTime);

                // 5. Ingredient filter
                var ingredientItem = IngredientFilter.SelectedItem as ComboBoxItem;
                var ingredient = ingredientItem?.Content.ToString();
                if (ingredient != "Any Ingredient" && !string.IsNullOrEmpty(ingredient))
                {
                    filtered = filtered.Where(r =>
                        r.Ingredients != null &&
                        r.Ingredients.Any(i => i.ToLower().Contains(ingredient.ToLower())));
                }

                // Convert to list
                var resultList = filtered.ToList();

                // Update observable collection
                _displayedRecipes.Clear();
                foreach (var recipe in resultList)
                {
                    _displayedRecipes.Add(recipe);
                }

                // Update count
                RecipeCountText.Text = resultList.Count == 0
                    ? "No recipes match your filters"
                    : $"Found {resultList.Count} recipe{(resultList.Count == 1 ? "" : "s")}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filter error: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Clear all filters and show all recipes
        /// </summary>
        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            // Reset all filters
            SearchBox.Text = "";
            CuisineFilter.SelectedIndex = 0;
            DifficultyFilter.SelectedIndex = 0;
            TimeSlider.Value = 120;
            IngredientFilter.SelectedIndex = 0;

            // Show all recipes
            ApplyFilters();
        }

        /// <summary>
        /// View recipe details - navigate to detail page
        /// </summary>
        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var recipe = button?.Tag as Recipe;

                if (recipe != null)
                {
                    // Navigate to detail page (we'll create this next)
                    NavigationService?.Navigate(new RecipeDetailPage(recipe));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

    }
}
