using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class ShoppingListPage : ContentPage
{
    public ShoppingListPage(ShoppingListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}