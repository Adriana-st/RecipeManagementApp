using Microsoft.Extensions.Logging;
using RecipeApp.MAUI.Services;
using RecipeApp.MAUI.ViewModels;
using RecipeApp.MAUI.Views;

namespace RecipeApp.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<IRecipeApiService, RecipeApiService>();

            // ViewModels
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<RecipesViewModel>();
            builder.Services.AddTransient<RecipeDetailViewModel>();
            builder.Services.AddTransient<FavouritesViewModel>();
            builder.Services.AddTransient<AddRecipeViewModel>();
            builder.Services.AddTransient<MealPlannerViewModel>();
            builder.Services.AddTransient<ShoppingListViewModel>();

            // Pages
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<RecipesPage>();
            builder.Services.AddTransient<RecipeDetailPage>();
            builder.Services.AddTransient<FavouritesPage>();
            builder.Services.AddTransient<AddRecipePage>();
            builder.Services.AddTransient<MealPlannerPage>();
            builder.Services.AddTransient<ShoppingListPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}