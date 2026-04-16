using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.MAUI.Models;
using RecipeApp.MAUI.Services;

namespace RecipeApp.MAUI.ViewModels
{
    [QueryProperty(nameof(Recipe), "Recipe")]
    public partial class RecipeDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Recipe _recipe;

        [ObservableProperty]
        private bool _isFavourite;

        [ObservableProperty]
        private string _favouriteButtonText = "🤍 Add to Favourites";

        [ObservableProperty]
        private Color _favouriteButtonColor = Color.FromArgb("#A62B60");

        public RecipeDetailViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Recipe Detail";
        }

        partial void OnRecipeChanged(Recipe value)
        {
            if (value is null) return;
            Title = value.Name;
            _ = CheckIfFavouriteAsync();
        }

        private async Task CheckIfFavouriteAsync()
        {
            IsFavourite = await _databaseService.IsRecipeSavedAsync(Recipe?.ApiId);
            UpdateFavouriteButton();
        }

        private void UpdateFavouriteButton()
        {
            if (IsFavourite)
            {
                FavouriteButtonText = "💔 Remove from Favourites";
                FavouriteButtonColor = Color.FromArgb("#97BC62");
            }
            else
            {
                FavouriteButtonText = "🤍 Add to Favourites";
                FavouriteButtonColor = Color.FromArgb("#A62B60");
            }
        }

        [RelayCommand]
        public async Task ToggleFavouriteAsync()
        {
            if (Recipe is null) return;

            try
            {
                IsBusy = true;

                if (IsFavourite)
                {
                    // Find and delete from DB
                    var favourites = await _databaseService.GetFavouritesAsync();
                    var existing = favourites.FirstOrDefault(r => r.ApiId == Recipe.ApiId);
                    if (existing != null)
                        await _databaseService.DeleteRecipeAsync(existing);

                    IsFavourite = false;
                    await Shell.Current.DisplayAlert("Removed", $"{Recipe.Name} removed from favourites.", "OK");
                }
                else
                {
                    await _databaseService.SaveRecipeAsync(Recipe);
                    IsFavourite = true;
                    await Shell.Current.DisplayAlert("Saved!", $"{Recipe.Name} added to favourites.", "OK");
                }

                UpdateFavouriteButton();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error toggling favourite: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}