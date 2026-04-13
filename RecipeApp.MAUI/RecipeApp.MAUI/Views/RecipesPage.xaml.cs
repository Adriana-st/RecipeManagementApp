using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class RecipesPage : ContentPage
{
    private readonly RecipesViewModel _viewModel;

    public RecipesPage(RecipesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_viewModel.Recipes.Any())
            await _viewModel.LoadRecipesAsync();
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        _viewModel.ApplyFilters();
    }
}