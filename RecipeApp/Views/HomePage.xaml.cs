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
    /// Home page - displays welcome message and feature cards
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        // Event handlers for buttons on the cards
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to RecipesPage
            NavigationService?.Navigate(new RecipePage());
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddRecipePage());
        }

        private void MealPlannerButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Meal Planner - Coming soon", "Coming Soon");
        }

        private void ShoppingListButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shopping List - Coming soon!", "Coming Soon");
        }
    }
}
