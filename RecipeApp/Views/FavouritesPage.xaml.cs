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
    /// Page for displaying favourite recipes from database
    /// </summary>
    public partial class FavouritesPage : Page
    {
        private readonly RecipeService _recipeService;
        public FavouritesPage()
        {
            InitializeComponent();
            _recipeService = new RecipeService();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFavourites();
        }

        private async System.Threading.Tasks.Task LoadFavourites()
        {
            try
            {
                // Show loading
                LoadingPanel.Visibility = Visibility.Visible;
                FavouritesPanel.Visibility = Visibility.Collapsed;
                EmptyPanel.Visibility = Visibility.Collapsed;

                // Load from database
                var favourites = await _recipeService.GetFavouritesAsync();

                if (favourites == null || favourites.Count == 0)
                {
                    // No favourites - show empty state
                    LoadingPanel.Visibility = Visibility.Collapsed;
                    EmptyPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    // Process ingredients for display
                    foreach (var recipe in favourites)
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

                    // Display favourites
                    FavouritesList.ItemsSource = favourites;
                    CountText.Text = $"You have {favourites.Count} favourite recipe{(favourites.Count == 1 ? "" : "s")}";

                    // Show results
                    LoadingPanel.Visibility = Visibility.Collapsed;
                    FavouritesPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading favourites: {ex.Message}", "Error");
                LoadingPanel.Visibility = Visibility.Collapsed;
                EmptyPanel.Visibility = Visibility.Visible;
            }
        }

        private async void RemoveFromFavourites_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var recipe = button?.Tag as Models.Recipe;

                if (recipe == null)
                {
                    MessageBox.Show("Error: Could not get recipe data", "Error");
                    return;
                }

                // Confirm removal
                var result = MessageBox.Show(
                    $"Remove '{recipe.Name}' from favourites?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove from database
                    var success = await _recipeService.RemoveFromFavouritesAsync(recipe.Id);

                    if (success)
                    {
                        MessageBox.Show($"'{recipe.Name}' removed from favourites!", "Removed");

                        // Reload the list
                        await LoadFavourites();
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove recipe. Please try again.", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void BrowseRecipes_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to browse recipes page
            NavigationService?.Navigate(new RecipePage());
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
