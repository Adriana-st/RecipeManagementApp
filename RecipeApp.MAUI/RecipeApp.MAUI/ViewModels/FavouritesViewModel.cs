using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.MAUI.Services;
using RecipeApp.MAUI.Views;
using System.Collections.ObjectModel;
using RecipeApp.Shared;

namespace RecipeApp.MAUI.ViewModels
{
    public partial class FavouritesViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Recipe> Favourites { get; } = new();

        [ObservableProperty]
        private bool _hasNoFavourites;

        public FavouritesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "My Favourites";
        }

        [RelayCommand]
        public async Task LoadFavouritesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var favourites = await _databaseService.GetFavouritesAsync();

                Favourites.Clear();
                foreach (var recipe in favourites)
                    Favourites.Add(recipe);

                HasNoFavourites = !Favourites.Any();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading favourites: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task RemoveFavouriteAsync(Recipe recipe)
        {
            if (recipe is null) return;

            string message = recipe.Source == "Custom"
                ? $"This will permanently delete \"{recipe.Name}\" from the app. Are you sure?"
                : $"Remove \"{recipe.Name}\" from favourites?";

            string title = recipe.Source == "Custom" ? "Delete Recipe" : "Remove Favourite";
            string confirm = recipe.Source == "Custom" ? "Delete" : "Remove";

            bool confirmed = await Shell.Current.DisplayAlert(title, message, confirm, "Cancel");
            if (!confirmed) return;

            await _databaseService.DeleteRecipeAsync(recipe);
            Favourites.Remove(recipe);
            HasNoFavourites = !Favourites.Any();
        }

        [RelayCommand]
        public async Task GoToRecipeDetailAsync(Recipe recipe)
        {
            if (recipe is null) return;

            await Shell.Current.GoToAsync(nameof(RecipeDetailPage), new Dictionary<string, object>
            {
                { "Recipe", recipe }
            });
        }
    }
}