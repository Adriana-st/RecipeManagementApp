using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.Shared;
using RecipeApp.MAUI.Services;
using System.Collections.ObjectModel;

namespace RecipeApp.MAUI.ViewModels
{
    public partial class AddRecipeViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _selectedCuisine = "Italian";

        [ObservableProperty]
        private string _selectedDifficulty = "Easy";

        [ObservableProperty]
        private int _prepTimeMinutes;

        [ObservableProperty]
        private int _cookTimeMinutes;

        [ObservableProperty]
        private int _servings = 2;

        [ObservableProperty]
        private string _imageUrl = string.Empty;

        [ObservableProperty]
        private string _newIngredient = string.Empty;

        [ObservableProperty]
        private string _newInstruction = string.Empty;

        public ObservableCollection<string> Ingredients { get; } = new();
        public ObservableCollection<string> Instructions { get; } = new();

        public List<string> CuisineOptions { get; } = new()
        {
            "Italian", "Asian", "Chinese", "Japanese", "Thai",
            "Indian", "Mexican", "American", "Mediterranean",
            "French", "Greek", "Middle Eastern", "Other"
        };

        public List<string> DifficultyOptions { get; } = new() { "Easy", "Medium", "Hard" };

        public AddRecipeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Add Recipe";
        }

        [RelayCommand]
        public void AddIngredient()
        {
            if (string.IsNullOrWhiteSpace(NewIngredient)) return;
            Ingredients.Add(NewIngredient.Trim());
            NewIngredient = string.Empty;
        }

        [RelayCommand]
        public void RemoveIngredient(string ingredient)
        {
            if (ingredient is not null)
                Ingredients.Remove(ingredient);
        }

        [RelayCommand]
        public void AddInstruction()
        {
            if (string.IsNullOrWhiteSpace(NewInstruction)) return;
            Instructions.Add(NewInstruction.Trim());
            NewInstruction = string.Empty;
        }

        [RelayCommand]
        public void RemoveInstruction(string instruction)
        {
            if (instruction is not null)
                Instructions.Remove(instruction);
        }

        [RelayCommand]
        public async Task SaveRecipeAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Validation", "Please enter a recipe name.", "OK");
                return;
            }
            if (!Ingredients.Any())
            {
                await Shell.Current.DisplayAlert("Validation", "Please add at least one ingredient.", "OK");
                return;
            }
            if (!Instructions.Any())
            {
                await Shell.Current.DisplayAlert("Validation", "Please add at least one instruction.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var recipe = new Recipe
                {
                    Name = Name.Trim(),
                    Description = Description.Trim(),
                    CuisineType = SelectedCuisine,
                    Difficulty = SelectedDifficulty,
                    PrepTimeMinutes = PrepTimeMinutes,
                    CookTimeMinutes = CookTimeMinutes,
                    Servings = Servings,
                    Ingredients = Ingredients.ToList(),
                    Instructions = Instructions.ToList(),
                    ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? null : ImageUrl.Trim(),
                    Source = "Custom",
                    IsFavourite = true,
                    DateAdded = DateTime.Now
                };

                await _databaseService.SaveRecipeAsync(recipe);
                await Shell.Current.DisplayAlert("Saved!", $"{recipe.Name} has been saved!", "OK");
                ClearForm();
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error saving recipe: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Could not save recipe.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Description = string.Empty;
            SelectedCuisine = "Italian";
            SelectedDifficulty = "Easy";
            PrepTimeMinutes = 0;
            CookTimeMinutes = 0;
            Servings = 2;
            ImageUrl = string.Empty;
            Ingredients.Clear();
            Instructions.Clear();
        }
    }
}