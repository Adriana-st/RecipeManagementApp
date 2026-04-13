using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadStatsAsync();
    }

    private async void OnBrowseRecipesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///RecipesPage");
    }

    private async void OnFavouritesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///FavouritesPage");
    }

    private async void OnAddRecipeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///AddRecipePage");
    }

    private async void OnMealPlannerClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MealPlannerPage");
    }

    private async void OnShoppingListClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///ShoppingListPage");
    }
}