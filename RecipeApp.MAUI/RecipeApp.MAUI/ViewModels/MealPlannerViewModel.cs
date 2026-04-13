using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.MAUI.Models;
using RecipeApp.MAUI.Services;
using System.Collections.ObjectModel;

namespace RecipeApp.MAUI.ViewModels
{
    public partial class MealPlannerViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private DateTime _currentWeekStart;

        [ObservableProperty]
        private string _weekRangeText = string.Empty;

        public ObservableCollection<DayMeals> WeekMeals { get; } = new();

        public MealPlannerViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Meal Planner";

            // Start on current Monday
            var today = DateTime.Today;
            var daysFromMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            CurrentWeekStart = today.AddDays(-daysFromMonday);
        }

        partial void OnCurrentWeekStartChanged(DateTime value)
        {
            WeekRangeText = $"{value:dd MMM} - {value.AddDays(6):dd MMM yyyy}";
            _ = LoadWeekAsync();
        }

        [RelayCommand]
        public async Task LoadWeekAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var mealPlans = await _databaseService.GetWeekMealPlansAsync(CurrentWeekStart);

                WeekMeals.Clear();

                for (int i = 0; i < 7; i++)
                {
                    var date = CurrentWeekStart.AddDays(i);
                    var dayMeals = new DayMeals
                    {
                        Date = date,
                        DayName = date.ToString("dddd"),
                        ShortDate = date.ToString("dd MMM"),
                        IsToday = date.Date == DateTime.Today
                    };

                    var mealsForDay = mealPlans
                        .Where(m => m.Date.Date == date.Date)
                        .OrderBy(m => GetMealTypeOrder(m.MealType))
                        .ToList();

                    foreach (var meal in mealsForDay)
                        dayMeals.Meals.Add(meal);

                    WeekMeals.Add(dayMeals);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading week: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void PreviousWeek()
        {
            CurrentWeekStart = CurrentWeekStart.AddDays(-7);
        }

        [RelayCommand]
        public void NextWeek()
        {
            CurrentWeekStart = CurrentWeekStart.AddDays(7);
        }

        [RelayCommand]
        public async Task AddMealAsync(DayMeals dayMeals)
        {
            if (dayMeals is null) return;

            // Step 1: Pick meal type
            string mealType = await Shell.Current.DisplayActionSheet(
                "Select Meal Type", "Cancel", null,
                "Breakfast", "Lunch", "Dinner", "Snack");

            if (mealType is null || mealType == "Cancel") return;

            // Step 2: Pick from favourites or enter name
            var favourites = await _databaseService.GetFavouritesAsync();

            string recipeName;

            if (favourites.Any())
            {
                var options = favourites.Select(f => f.Name).ToArray();
                var allOptions = options.Append("✏️ Enter manually").ToArray();

                string selected = await Shell.Current.DisplayActionSheet(
                    "Choose a Recipe", "Cancel", null, allOptions);

                if (selected is null || selected == "Cancel") return;

                if (selected == "✏️ Enter manually")
                {
                    recipeName = await Shell.Current.DisplayPromptAsync(
                        "Recipe Name", "Enter recipe name:");
                    if (string.IsNullOrWhiteSpace(recipeName)) return;
                }
                else
                {
                    recipeName = selected;
                }
            }
            else
            {
                recipeName = await Shell.Current.DisplayPromptAsync(
                    "Recipe Name", "Enter recipe name:");
                if (string.IsNullOrWhiteSpace(recipeName)) return;
            }

            // Save meal plan
            var mealPlan = new MealPlan
            {
                Date = dayMeals.Date,
                MealType = mealType,
                RecipeName = recipeName
            };

            await _databaseService.AddMealPlanAsync(mealPlan);
            dayMeals.Meals.Add(mealPlan);

            // Re-sort meals in correct order
            var sorted = dayMeals.Meals
                .OrderBy(m => GetMealTypeOrder(m.MealType))
                .ToList();

            dayMeals.Meals.Clear();
            foreach (var m in sorted)
                dayMeals.Meals.Add(m);
        }

        [RelayCommand]
        public async Task RemoveMealAsync(MealPlan meal)
        {
            if (meal is null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Remove Meal", $"Remove {meal.RecipeName}?", "Remove", "Cancel");

            if (!confirm) return;

            await _databaseService.DeleteMealPlanAsync(meal);

            // Find the day and remove from it
            foreach (var day in WeekMeals)
            {
                if (day.Meals.Contains(meal))
                {
                    day.Meals.Remove(meal);
                    break;
                }
            }
        }

        private static int GetMealTypeOrder(string mealType) => mealType switch
        {
            "Breakfast" => 0,
            "Lunch" => 1,
            "Dinner" => 2,
            "Snack" => 3,
            _ => 4
        };
    }

    // Helper class to group meals by day
    public partial class DayMeals : ObservableObject
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public string ShortDate { get; set; }
        public bool IsToday { get; set; }
        public ObservableCollection<MealPlan> Meals { get; } = new();
    }
}