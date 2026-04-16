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
        // Don't auto-apply, wait for Apply button
    }

    private void OnSliderDragCompleted(object sender, EventArgs e)
    {
        // Don't auto-apply, wait for Apply button
    }

    private void OnToggleFiltersClicked(object sender, EventArgs e)
    {
        _viewModel.FiltersVisible = !_viewModel.FiltersVisible;
    }

    private void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        _viewModel.ApplyFilters();
        _viewModel.FiltersVisible = false;
    }

    private void OnResetFiltersClicked(object sender, EventArgs e)
    {
        _viewModel.ResetFilters();
        _viewModel.FiltersVisible = false;
    }
}