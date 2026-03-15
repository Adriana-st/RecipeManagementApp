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
using RecipeApp.Models;

namespace RecipeApp.Views
{
    /// <summary>
    /// Dialog for adding a meal to the plan
    /// </summary>
    public partial class AddMealDialog : Window
    {
        private readonly List<Recipe> _recipes;
        private readonly DateTime _date;

        public MealPlan SelectedMealPlan { get; private set; }

        public AddMealDialog(List<Recipe> recipes, DateTime date)
        {
            InitializeComponent();
            _recipes = recipes;
            _date = date;

            LoadRecipes();
        }

        private void LoadRecipes()
        {
            if (_recipes == null || _recipes.Count == 0)
            {
                NoRecipesText.Visibility = Visibility.Visible;
                RecipesList.Visibility = Visibility.Collapsed;
            }
            else
            {
                RecipesList.ItemsSource = _recipes.OrderBy(r => r.Name);
            }
        }

        private void Recipe_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var border = sender as Border;
                var recipe = border?.Tag as Recipe;

                if (recipe != null)
                {
                    var mealTypeItem = MealTypeBox.SelectedItem as ComboBoxItem;
                    var mealType = mealTypeItem?.Content.ToString();

                    SelectedMealPlan = new MealPlan
                    {
                        Date = _date,
                        MealType = mealType,
                        RecipeId = recipe.Id,
                        RecipeName = recipe.Name
                    };

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
