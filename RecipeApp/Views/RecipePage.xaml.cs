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
using System.Windows.Shapes;

namespace RecipeApp.Views
{
    /// <summary>
    /// Interaction logic for RecipePage.xaml
    /// </summary>
    public partial class RecipePage : Window
    {
        private readonly RecipeApiService _apiService;
        public RecipePage()
        {
            InitializeComponent();
            _apiService = new RecipeApiService();
        }

        // Event handler for window loaded - fetches recipes
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRecipes();
        }

        // Load recipes from API and display them
        private async System.Threading.Tasks.Task LoadRecipes()
        {
            try
            {
                // Show loading panel
                LoadingPanel.Visibility = Visibility.Visible;
                RecipeScrollViewer.Visibility = Visibility.Collapsed;
                ErrorPanel.Visibility = Visibility.Collapsed;

                Console.WriteLine("Starting to fetch recipes...");

                // Fetch recipes from API
                var recipes = await _apiService.GetRecipesAsync();

                Console.WriteLine($"Received {recipes.Count} recipes");

                // Process each recipe for display
                foreach (var recipe in recipes)
                {
                    // Create a display string for ingredients (first 5)
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

                // Bind recipes to UI
                RecipeList.ItemsSource = recipes;
                RecipeCountText.Text = $"Found {recipes.Count} delicious recipes!";

                // Hide loading, show results
                LoadingPanel.Visibility = Visibility.Collapsed;
                RecipeScrollViewer.Visibility = Visibility.Visible;

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

        // Try Again button click handler
        private async void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            await LoadRecipes();
        }

        // Mouse hover effect for recipe cards
        private void RecipeBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            }
        }

        // Mouse leave effect for recipe cards
        private void RecipeBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Colors.White);
            }
        }
    }
}
