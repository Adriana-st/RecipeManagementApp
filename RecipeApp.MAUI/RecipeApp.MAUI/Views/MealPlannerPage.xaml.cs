using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class MealPlannerPage : ContentPage
{
    private readonly MealPlannerViewModel _viewModel;

    public MealPlannerPage(MealPlannerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWeekAsync();
    }
}