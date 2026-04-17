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
        private readonly RecipeApiService _recipeApiService;

        [ObservableProperty]
        private DateTime _currentWeekStart;

        [ObservableProperty]
        private string _weekRangeText = string.Empty;

        public ObservableCollection<DayMeals> WeekMeals { get; } = new();

        public MealPlannerViewModel(DatabaseService databaseService, RecipeApiService recipeApiService)
        {
            _databaseService = databaseService;
            _recipeApiService = recipeApiService;
            Title = "Meal Planner";

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

            // Step 2: Pick source
            string source = await Shell.Current.DisplayActionSheet(
                "Add Recipe From", "Cancel", null,
                "⭐ My Favourites", "🔍 Search All Recipes", "✏️ Enter Manually");

            if (source is null || source == "Cancel") return;

            string recipeName = null;

            if (source == "⭐ My Favourites")
            {
                var favourites = await _databaseService.GetFavouritesAsync();

                if (!favourites.Any())
                {
                    await Shell.Current.DisplayAlert("No Favourites",
                        "You have no saved favourites yet. Try searching or entering manually.", "OK");
                    return;
                }

                var options = favourites.Select(r => r.Name).ToArray();

                string selected = await Shell.Current.DisplayActionSheet(
                    "Choose a Favourite", "Cancel", null, options);

                if (selected is null || selected == "Cancel") return;
                recipeName = selected;
            }
            else if (source == "🔍 Search All Recipes")
            {
                string searchTerm = await Shell.Current.DisplayPromptAsync(
                    "Search Recipes",
                    "Type to search (leave blank to see all):",
                    placeholder: "e.g. pasta, chicken...");

                if (searchTerm is null) return;

                var apiRecipes = await _recipeApiService.GetRecipesAsync();

                var filtered = string.IsNullOrWhiteSpace(searchTerm)
                    ? apiRecipes
                    : apiRecipes.Where(r =>
                        (r.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (r.CuisineType?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();

                if (!filtered.Any())
                {
                    await Shell.Current.DisplayAlert("No Results",
                        $"No recipes found for \"{searchTerm}\".", "OK");
                    return;
                }

                int pageSize = 15;
                int currentPage = 0;
                int totalPages = (int)Math.Ceiling(filtered.Count / (double)pageSize);
                string selected = null;

                while (selected is null)
                {
                    var page = filtered
                        .Skip(currentPage * pageSize)
                        .Take(pageSize)
                        .Select(r => r.Name)
                        .ToList();

                    bool hasNext = currentPage < totalPages - 1;
                    bool hasPrev = currentPage > 0;

                    // Add navigation options at the bottom
                    if (hasNext) page.Add("→ Next Page");
                    if (hasPrev) page.Add("← Previous Page");

                    string choice = await Shell.Current.DisplayActionSheet(
                        $"Select Recipe (page {currentPage + 1} of {totalPages})",
                        "Cancel", null,
                        page.ToArray());

                    if (choice is null || choice == "Cancel") return;

                    if (choice == "→ Next Page")
                    {
                        currentPage++;
                        continue;
                    }

                    if (choice == "← Previous Page")
                    {
                        currentPage--;
                        continue;
                    }

                    selected = choice;
                }

                recipeName = selected;
            }
            else if (source == "✏️ Enter Manually")
            {
                recipeName = await Shell.Current.DisplayPromptAsync(
                    "Recipe Name",
                    "Enter the recipe name:",
                    placeholder: "e.g. Grandma's Lasagne");

                if (string.IsNullOrWhiteSpace(recipeName)) return;
            }

            if (string.IsNullOrWhiteSpace(recipeName)) return;

            // Save meal plan
            var mealPlan = new MealPlan
            {
                Date = dayMeals.Date,
                MealType = mealType,
                RecipeName = recipeName.Trim()
            };

            await _databaseService.AddMealPlanAsync(mealPlan);
            dayMeals.Meals.Add(mealPlan);

            // Re-sort
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

        public bool HasNoMeals => !Meals.Any();
        public DayMeals()
        {
            Meals.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasNoMeals));
        }
    }
}