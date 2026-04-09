using RecipeApp.Models;
using RecipeApp.Services;
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

namespace RecipeApp.Views
{
    /// <summary>
    /// Page for adding custom recipes
    /// </summary>
    public partial class AddRecipePage : Page
    {
        private readonly RecipeService _recipeService;
        private ObservableCollection<string> _ingredients;
        private ObservableCollection<InstructionStep> _instructions;

        public AddRecipePage()
        {
            InitializeComponent();
            _recipeService = new RecipeService();

            // Initialize observable collections
            _ingredients = new ObservableCollection<string>();
            _instructions = new ObservableCollection<InstructionStep>();

            IngredientsListView.ItemsSource = _ingredients;
            InstructionsListView.ItemsSource = _instructions;
        }
        /// <summary>
        /// Add ingredient to list
        /// </summary>
        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            var ingredient = IngredientBox.Text?.Trim();

            if (string.IsNullOrEmpty(ingredient))
            {
                MessageBox.Show("Please enter an ingredient", "Validation Error");
                return;
            }

            // Add to observable collection (UI updates automatically!)
            _ingredients.Add(ingredient);

            // Clear input box
            IngredientBox.Clear();
            IngredientBox.Focus();
        }

        /// <summary>
        /// Allow pressing Enter to add ingredient
        /// </summary>
        private void IngredientBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddIngredient_Click(sender, e);
            }
        }

        /// <summary>
        /// Remove ingredient from list
        /// </summary>
        private void RemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ingredient = button?.Tag as string;

            if (ingredient != null)
            {
                _ingredients.Remove(ingredient);
            }
        }

        /// <summary>
        /// Add instruction step to list
        /// </summary>
        private void AddInstruction_Click(object sender, RoutedEventArgs e)
        {
            var instruction = InstructionBox.Text?.Trim();

            if (string.IsNullOrEmpty(instruction))
            {
                MessageBox.Show("Please enter an instruction step", "Validation Error");
                return;
            }

            // Add to observable collection with auto-numbered steps
            var stepNumber = _instructions.Count + 1;
            _instructions.Add(new InstructionStep
            {
                StepNumber = stepNumber,
                Instruction = instruction
            });

            // Clear input box
            InstructionBox.Clear();
            InstructionBox.Focus();
        }

        /// <summary>
        /// Remove instruction from list
        /// </summary>
        private void RemoveInstruction_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                var stepNumber = (int)button.Tag;
                var step = _instructions.FirstOrDefault(s => s.StepNumber == stepNumber);

                if (step != null)
                {
                    _instructions.Remove(step);

                    // Renumber remaining steps
                    for (int i = 0; i < _instructions.Count; i++)
                    {
                        _instructions[i].StepNumber = i + 1;
                    }

                    // Refresh the list
                    InstructionsListView.ItemsSource = null;
                    InstructionsListView.ItemsSource = _instructions;
                }
            }
        }

        /// <summary>
        /// Save recipe to database
        /// Demonstrates: Form validation, data creation
        /// </summary>
        private async void SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(RecipeNameBox.Text))
                {
                    MessageBox.Show("Please enter a recipe name", "Validation Error");
                    RecipeNameBox.Focus();
                    return;
                }

                if (CuisineBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a cuisine type", "Validation Error");
                    return;
                }

                if (DifficultyBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a difficulty level", "Validation Error");
                    return;
                }

                if (!int.TryParse(PrepTimeBox.Text, out int prepTime) || prepTime <= 0)
                {
                    MessageBox.Show("Please enter a valid prep time (number greater than 0)", "Validation Error");
                    PrepTimeBox.Focus();
                    return;
                }

                if (!int.TryParse(CookTimeBox.Text, out int cookTime) || cookTime <= 0)
                {
                    MessageBox.Show("Please enter a valid cook time (number greater than 0)", "Validation Error");
                    CookTimeBox.Focus();
                    return;
                }

                if (!int.TryParse(ServingsBox.Text, out int servings) || servings <= 0)
                {
                    MessageBox.Show("Please enter a valid number of servings (number greater than 0)", "Validation Error");
                    ServingsBox.Focus();
                    return;
                }

                if (_ingredients.Count == 0)
                {
                    MessageBox.Show("Please add at least one ingredient", "Validation Error");
                    IngredientBox.Focus();
                    return;
                }

                if (_instructions.Count == 0)
                {
                    MessageBox.Show("Please add at least one instruction step", "Validation Error");
                    InstructionBox.Focus();
                    return;
                }

                // Create new recipe
                var recipe = new Recipe
                {
                    Id = new Random().Next(10000, 99999), // Generate random ID for custom recipes
                    Name = RecipeNameBox.Text.Trim(),
                    Description = DescriptionBox.Text?.Trim() ?? "",
                    CuisineType = (CuisineBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Difficulty = (DifficultyBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    PrepTimeMinutes = prepTime,
                    CookTimeMinutes = cookTime,
                    Servings = servings,
                    ImageUrl = string.IsNullOrWhiteSpace(ImageUrlBox.Text)
                        ? "https://via.placeholder.com/400x400?text=Custom+Recipe"
                        : ImageUrlBox.Text.Trim(),
                    Ingredients = _ingredients.ToList(),
                    Instructions = _instructions.Select(s => s.Instruction).ToList(),
                    IsFavourite = true,
                    DateAdded = DateTime.Now,
                    Source = "Custom"
                };

                // Prepare for database
                recipe.PrepareForDatabase();

                // Save to database
                var success = await _recipeService.SaveToFavouritesAsync(recipe);

                if (success)
                {
                    MessageBox.Show($"'{recipe.Name}' saved successfully!",
                                  "Success",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    // Clear form
                    ClearForm_Click(sender, e);

                    // Navigate to favourites
                    NavigationService?.Navigate(new FavouritesPage());
                }
                else
                {
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
        /// Clear all form fields
        /// </summary>
        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            RecipeNameBox.Clear();
            DescriptionBox.Clear();
            CuisineBox.SelectedIndex = -1;
            DifficultyBox.SelectedIndex = -1;
            PrepTimeBox.Clear();
            CookTimeBox.Clear();
            ServingsBox.Clear();
            ImageUrlBox.Clear();
            IngredientBox.Clear();
            InstructionBox.Clear();

            _ingredients.Clear();
            _instructions.Clear();

            RecipeNameBox.Focus();
        }
    }

    /// <summary>
    /// Helper class for instruction steps
    /// </summary>
    public class InstructionStep
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; }
    }
}

