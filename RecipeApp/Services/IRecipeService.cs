using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Services
{
    /// <summary>
    /// Interface for recipe operations
    /// Demonstrates: Interfaces requirement
    /// </summary>
    public interface IRecipeService
    {
        // Contract: any class implementing this MUST have these methods
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<List<Recipe>> SearchRecipesAsync(string searchTerm);
        Task<List<Recipe>> FilterByCuisineAsync(string cuisineType);
        Task<List<Recipe>> FilterByTimeAsync(int maxMinutes);
    }
}
