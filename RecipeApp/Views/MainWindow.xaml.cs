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

namespace RecipeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Demonstrates: Event Handling
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Home page - Currently viewing",
                          "Navigation",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Browse Recipes page - Coming soon!\n\nThis will display recipes from the DummyJSON API.",
                          "Coming Soon",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void FavouritesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("My Favourites page - Coming soon!\n\nThis will show saved recipes from the database.",
                          "Coming Soon",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Recipe page - Coming soon!\n\nThis will allow you to create custom recipes.",
                          "Coming Soon",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void MealPlannerButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Meal Planner page - Coming soon!\n\nThis will help you plan meals for the week.",
                          "Coming Soon",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void ShoppingListButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shopping List page - Coming soon!\n\nThis will generate shopping lists from recipes.",
                          "Coming Soon",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }
    }
}
