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
using System.Globalization;
using RecipeApp.Models;
using RecipeApp.Services;

namespace RecipeApp.Views
{
    /// <summary>
    /// Meal Planner page - plan weekly meals
    /// Demonstrates: Date handling, complex UI updates
    /// </summary>
    public partial class MealPlannerPage : Page
    {
        private readonly MealPlanService _mealPlanService;
        private readonly RecipeService _recipeService;
        private DateTime _currentWeekStart;
        private List<Recipe> _availableRecipes;

        public MealPlannerPage()
        {
            InitializeComponent();
            _mealPlanService = new MealPlanService();
            _recipeService = new RecipeService();

            // Start with current week (Monday)
            _currentWeekStart = GetMonday(DateTime.Now);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load available recipes
            _availableRecipes = await _recipeService.GetFavouritesAsync();

            // Load current week
            await LoadWeek();
        }

        /// <summary>
        /// Get the Monday of the week containing the given date
        /// </summary>
        private DateTime GetMonday(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            // If Sunday (0), go back 6 days; otherwise go back (dayOfWeek - 1) days
            var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return date.Date.AddDays(-daysToSubtract);
        }

        /// <summary>
        /// Load and display the current week's meal plans
        /// </summary>
        private async System.Threading.Tasks.Task LoadWeek()
        {
            try
            {
                // Update week range display
                var weekEnd = _currentWeekStart.AddDays(6);
                WeekRangeText.Text = $"Week of {_currentWeekStart:MMM d} - {weekEnd:MMM d, yyyy}";

                // Update day headers
                UpdateDayHeaders();

                // Clear all day panels
                ClearAllDays();

                // Load meal plans from database
                var mealPlans = await _mealPlanService.GetWeekMealPlansAsync(_currentWeekStart);

                // Group by day and display
                foreach (var mealPlan in mealPlans)
                {
                    AddMealToDay(mealPlan);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading week: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Update the day headers with dates
        /// </summary>
        private void UpdateDayHeaders()
        {
            Monday_Day.Text = _currentWeekStart.ToString("MMM d");
            Tuesday_Day.Text = _currentWeekStart.AddDays(1).ToString("MMM d");
            Wednesday_Day.Text = _currentWeekStart.AddDays(2).ToString("MMM d");
            Thursday_Day.Text = _currentWeekStart.AddDays(3).ToString("MMM d");
            Friday_Day.Text = _currentWeekStart.AddDays(4).ToString("MMM d");
            Saturday_Day.Text = _currentWeekStart.AddDays(5).ToString("MMM d");
            Sunday_Day.Text = _currentWeekStart.AddDays(6).ToString("MMM d");
        }

        /// <summary>
        /// Clear all meals from all days
        /// </summary>
        private void ClearAllDays()
        {
            MondayMeals.Children.Clear();
            TuesdayMeals.Children.Clear();
            WednesdayMeals.Children.Clear();
            ThursdayMeals.Children.Clear();
            FridayMeals.Children.Clear();
            SaturdayMeals.Children.Clear();
            SundayMeals.Children.Clear();
        }

        /// <summary>
        /// Add a meal card to the appropriate day panel
        /// </summary>
        private void AddMealToDay(MealPlan mealPlan)
        {
            var dayOfWeek = (int)mealPlan.Date.DayOfWeek;
            var panel = GetDayPanel(dayOfWeek);

            if (panel == null) return;

            // Create meal card
            var mealCard = CreateMealCard(mealPlan);
            panel.Children.Add(mealCard);
        }

        /// <summary>
        /// Get the StackPanel for a specific day of week
        /// </summary>
        private StackPanel GetDayPanel(int dayOfWeek)
        {
            if (dayOfWeek == 1) return MondayMeals;
            if (dayOfWeek == 2) return TuesdayMeals;
            if (dayOfWeek == 3) return WednesdayMeals;
            if (dayOfWeek == 4) return ThursdayMeals;
            if (dayOfWeek == 5) return FridayMeals;
            if (dayOfWeek == 6) return SaturdayMeals;
            if (dayOfWeek == 0) return SundayMeals;
            return null;
        }

        /// <summary>
        /// Create a visual meal card
        /// </summary>
        private Border CreateMealCard(MealPlan mealPlan)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                BorderThickness = new Thickness(1, 1, 1, 1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8, 8, 8, 8),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var stackPanel = new StackPanel();

            // Meal type badge
            var badge = new Border
            {
                Background = GetMealTypeColor(mealPlan.MealType),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5, 2, 5, 2),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var badgeText = new TextBlock
            {
                Text = mealPlan.MealType,
                Foreground = Brushes.White,
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };

            badge.Child = badgeText;
            stackPanel.Children.Add(badge);

            // Recipe name
            var recipeName = new TextBlock
            {
                Text = mealPlan.RecipeName,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(recipeName);

            // Remove button
            var removeButton = new Button
            {
                Content = "Remove",
                Background = new SolidColorBrush(Color.FromRgb(153, 0, 17)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Padding = new Thickness(8, 3, 8, 3),
                FontSize = 10,
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = mealPlan.MealPlanId
            };

            removeButton.Click += RemoveMeal_Click;
            stackPanel.Children.Add(removeButton);

            border.Child = stackPanel;
            return border;
        }

        /// <summary>
        /// Get color for meal type badge
        /// </summary>
        private SolidColorBrush GetMealTypeColor(string mealType)
        {
            if (mealType == "Breakfast") return new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Orange
            if (mealType == "Lunch") return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
            if (mealType == "Dinner") return new SolidColorBrush(Color.FromRgb(166, 43, 96)); // Pink
            if (mealType == "Snack") return new SolidColorBrush(Color.FromRgb(151, 188, 98)); // Light green
            return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Gray
        }

        /// <summary>
        /// Remove a meal from the plan
        /// </summary>
        private async void RemoveMeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var mealPlanId = (int)button.Tag;

                var result = MessageBox.Show(
                    "Remove this meal from your plan?",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _mealPlanService.RemoveMealPlanAsync(mealPlanId);

                    if (success)
                    {
                        await LoadWeek();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing meal: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Add meal button clicked - show recipe selection dialog
        /// </summary>
        private async void AddMeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var dayName = button.Tag.ToString();

                // Get the date for this day
                var date = GetDateForDay(dayName);

                // Show selection dialog
                var dialog = new AddMealDialog(_availableRecipes, date);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    var mealPlan = dialog.SelectedMealPlan;

                    if (mealPlan != null)
                    {
                        var success = await _mealPlanService.AddMealPlanAsync(mealPlan);

                        if (success)
                        {
                            await LoadWeek();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding meal: {ex.Message}", "Error");
            }
        }

        /// <summary>
        /// Get date for a day name
        /// </summary>
        private DateTime GetDateForDay(string dayName)
        {
            if (dayName == "Monday") return _currentWeekStart;
            if (dayName == "Tuesday") return _currentWeekStart.AddDays(1);
            if (dayName == "Wednesday") return _currentWeekStart.AddDays(2);
            if (dayName == "Thursday") return _currentWeekStart.AddDays(3);
            if (dayName == "Friday") return _currentWeekStart.AddDays(4);
            if (dayName == "Saturday") return _currentWeekStart.AddDays(5);
            if (dayName == "Sunday") return _currentWeekStart.AddDays(6);
            return _currentWeekStart;
        }

        private async void PreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            await LoadWeek();
        }

        private async void ThisWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = GetMonday(DateTime.Now);
            await LoadWeek();
        }

        private async void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            await LoadWeek();
        }

        private async void ClearWeek_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Clear all meals for this week?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _mealPlanService.ClearWeekAsync(_currentWeekStart);

                if (success)
                {
                    await LoadWeek();
                    MessageBox.Show("Week cleared successfully!", "Success");
                }
            }
        }
    }
}
