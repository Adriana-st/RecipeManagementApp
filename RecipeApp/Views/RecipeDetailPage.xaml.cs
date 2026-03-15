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

namespace RecipeApp.Views
{
    /// <summary>
    /// Interaction logic for RecipeDetailPage.xaml
    /// </summary>
    public partial class RecipeDetailPage : Page
    {
        private readonly Recipe _recipe;
        private readonly RecipeService _recipeService;
        public RecipeDetailPage(Recipe recipe)
        {
            InitializeComponent();
            _recipe = recipe;
            _recipeService = new RecipeService();
            LoadRecipeDetails();
        }
        private void LoadRecipeDetails()
        {
            try
            {
                // Set recipe name
                RecipeNameText.Text = _recipe.Name;

                // Set image
                RecipeImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(_recipe.ImageUrl, UriKind.Absolute));

                // Set quick info
                CuisineText.Text = _recipe.CuisineType ?? "Not specified";
                DifficultyText.Text = _recipe.Difficulty ?? "Not specified";
                PrepTimeText.Text = $"{_recipe.PrepTimeMinutes} min";
                CookTimeText.Text = $"{_recipe.CookTimeMinutes} min";
                TotalTimeText.Text = $"{_recipe.TotalTimeMinutes} min";
                ServingsText.Text = _recipe.Servings.ToString();

                // Load ingredients
                if (_recipe.Ingredients != null && _recipe.Ingredients.Count > 0)
                {
                    IngredientsList.ItemsSource = _recipe.Ingredients;
                }
                else
                {
                    IngredientsList.ItemsSource = new List<string> { "No ingredients listed" };
                }

                // Load instructions with step numbers
                if (_recipe.Instructions != null && _recipe.Instructions.Count > 0)
                {
                    var numberedInstructions = _recipe.Instructions
                        .Select((instruction, index) => new
                        {
                            StepNumber = index + 1,
                            Instruction = instruction
                        })
                        .ToList();

                    InstructionsList.ItemsSource = numberedInstructions;
                }
                else
                {
                    InstructionsList.ItemsSource = new[]
                    {
                        new { StepNumber = 1, Instruction = "No instructions provided" }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recipe details: {ex.Message}", "Error");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back
            NavigationService?.GoBack();
        }

        private async void SaveToFavourites_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Change button to show it's saving
                SaveButton.Content = "💾 Saving...";
                SaveButton.IsEnabled = false;

                // Prepare recipe for database
                _recipe.PrepareForDatabase();

                // Save to database
                var success = await _recipeService.SaveToFavouritesAsync(_recipe);

                if (success)
                {
                    // Success!
                    SaveButton.Content = "✅ Saved!";
                    SaveButton.Background = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(151, 188, 98)); // Green

                    MessageBox.Show($"'{_recipe.Name}' saved to favourites!",
                                  "Saved",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
                else
                {
                    // Failed
                    SaveButton.Content = "💾 Save to Favourites";
                    SaveButton.IsEnabled = true;

                    MessageBox.Show("Failed to save recipe. Please try again.",
                                  "Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
                SaveButton.Content = "💾 Save to Favourites";
                SaveButton.IsEnabled = true;
            }
        }
    }
}
