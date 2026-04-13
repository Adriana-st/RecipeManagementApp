using RecipeApp.MAUI.ViewModels;

namespace RecipeApp.MAUI.Views;

public partial class RecipeDetailPage : ContentPage
{
    private readonly RecipeDetailViewModel _viewModel;

    public RecipeDetailPage(RecipeDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuildInstructionsList();
    }

    // Build numbered instructions in code-behind since
    // CollectionView index binding isn't supported in MAUI
    private void BuildInstructionsList()
    {
        InstructionsLayout.Children.Clear();

        var instructions = _viewModel.Recipe?.Instructions;
        if (instructions is null || !instructions.Any()) return;

        for (int i = 0; i < instructions.Count; i++)
        {
            var frame = new Frame
            {
                BackgroundColor = Color.FromArgb("#FFF5F8"),
                CornerRadius = 8,
                Padding = new Thickness(12),
                HasShadow = false
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 32 },
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                ColumnSpacing = 10
            };

            var stepLabel = new Label
            {
                Text = $"{i + 1}",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#A62B60"),
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var textLabel = new Label
            {
                Text = instructions[i],
                FontSize = 15,
                TextColor = Color.FromArgb("#333333"),
                VerticalOptions = LayoutOptions.Start
            };

            grid.Add(stepLabel, 0, 0);
            grid.Add(textLabel, 1, 0);
            frame.Content = grid;
            InstructionsLayout.Children.Add(frame);
        }
    }
}