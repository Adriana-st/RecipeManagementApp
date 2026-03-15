using RecipeApp.Views;
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
    /// Main window with frame navigation
    /// Demonstrates: Event handling, frame navigation
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// When window loads, navigate to home page
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Show home page on startup
            MainFrame.Navigate(new HomePage());
        }

        // Navigate to Home page
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage());
        }

        // Navigate to Browse Recipes page
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RecipePage());
        }

        private void FavouritesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FavouritesPage());
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddRecipePage());
        }

        private void MealPlannerButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate (new MealPlannerPage());
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
