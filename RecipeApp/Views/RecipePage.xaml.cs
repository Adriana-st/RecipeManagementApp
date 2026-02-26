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

namespace RecipeApp.Views
{
    /// <summary>
    /// Page for browsing recipes from API
    /// </summary>
    public partial class RecipePage : Page
    {
        private readonly RecipeApiService _apiService;

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

                // Fetch recipes
                var recipes = await _apiService.GetRecipesAsync();

                // Process ingredients for display
                foreach (var recipe in recipes)
                {
                    if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
                    {
                        var count = Math.Min(5, recipe.Ingredients.Count);
                        recipe.IngredientsDisplay = string.Join(", ", recipe.Ingredients.Take(count));

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

                // Display recipes
                RecipeList.ItemsSource = recipes;
                RecipeCountText.Text = $"Found {recipes.Count} delicious recipes!";

                // Show results
                LoadingPanel.Visibility = Visibility.Collapsed;
                RecipesPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Show error
                LoadingPanel.Visibility = Visibility.Collapsed;
                ErrorPanel.Visibility = Visibility.Visible;
                ErrorText.Text = ex.Message;
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
    }
}
