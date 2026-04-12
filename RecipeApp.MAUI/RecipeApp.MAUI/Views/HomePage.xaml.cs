using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        System.Diagnostics.Debug.WriteLine(" HomePage constructor START");

        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        System.Diagnostics.Debug.WriteLine(" HomePage constructor END");
    }

    protected override async void OnAppearing()
    {
        System.Diagnostics.Debug.WriteLine(" HomePage OnAppearing START");

        base.OnAppearing();
        await _viewModel.LoadStatsAsync();

        System.Diagnostics.Debug.WriteLine(" HomePage OnAppearing END");
    }

    private async void OnBrowseRecipesClicked(object sender, EventArgs e)
    {
        // TODO: Will implement later
        await DisplayAlert("Coming Soon", "Browse Recipes page will be added next!", "OK");
    }

    private async void OnFavouritesClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Favourites page will be added next!", "OK");
    }

    private async void OnAddRecipeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Add Recipe page will be added next!", "OK");
    }

    private async void OnMealPlannerClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Meal Planner page will be added next!", "OK");
    }

    private async void OnShoppingListClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Shopping List page will be added next!", "OK");
    }
}