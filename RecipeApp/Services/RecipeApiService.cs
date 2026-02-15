using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Services
{
    /// <summary>
    /// Service to fetch recipes from DummyJSON API
    /// Demonstrates: Inheritance (implements IRecipeService), Exception Handling
    /// </summary>
    public class RecipeApiService : IRecipeService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://dummyjson.com/recipes";

        public RecipeApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(BASE_URL);
                // TODO: Parse JSON response 
                return new List<Recipe>();
            }
            catch (HttpRequestException ex)
            {
                // Exception handling 
                Console.WriteLine($"Error fetching recipes: {ex.Message}");
                throw;
            }
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            try
            {
                var url = $"{BASE_URL}/{id}";
                var response = await _httpClient.GetStringAsync(url);
                // TODO: Parse JSON response
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching recipe {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Recipe>> SearchRecipesAsync(string searchTerm)
        {
            try
            {
                var url = $"{BASE_URL}/search?q={searchTerm}";
                var response = await _httpClient.GetStringAsync(url);
                // TODO: Implement search
                return new List<Recipe>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error searching recipes: {ex.Message}");
                throw;
            }
        }

        public Task<List<Recipe>> FilterByCuisineAsync(string cuisineType)
        {
            // TODO: Implement filtering with LINQ 
            return Task.FromResult(new List<Recipe>());
        }

        public Task<List<Recipe>> FilterByTimeAsync(int maxMinutes)
        {
            // TODO: Implement time filtering with LINQ 
            return Task.FromResult(new List<Recipe>());
        }
    }
}
