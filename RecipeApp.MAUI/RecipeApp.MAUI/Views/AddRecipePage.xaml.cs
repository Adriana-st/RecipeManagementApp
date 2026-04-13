using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class AddRecipePage : ContentPage
{
    private readonly AddRecipeViewModel _viewModel;

    public AddRecipePage(AddRecipeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
}