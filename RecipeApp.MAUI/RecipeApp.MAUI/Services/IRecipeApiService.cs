using RecipeApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.MAUI.Services
{
    /// <summary>
    /// Contract for fetching recipes from an external source.
    /// Demonstrates: Interfaces requirement — decouples ViewModels
    /// from a specific API implementation.
    /// </summary>
    public interface IRecipeApiService
    {
        Task<List<Recipe>> GetRecipesAsync();
    }
}
