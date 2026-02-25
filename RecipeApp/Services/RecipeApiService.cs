using Newtonsoft.Json;
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
    /// Demonstrates: Inheritance (implements IRecipeService), Exception Handling, Async programming
    /// </summary>
    public class RecipeApiService : IRecipeService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://dummyjson.com/recipes";

        public RecipeApiService()
        {
            _httpClient = new HttpClient();
        }

        // Get all recipes from the API
        public async Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                Console.WriteLine("Fetching recipes from API...");

                // Make HTTP request
                var response = await _httpClient.GetStringAsync(BASE_URL);

                Console.WriteLine("Response received, parsing JSON...");

                // Parse JSON into RecipeResponse object
                var recipeResponse = JsonConvert.DeserializeObject<RecipeResponse>(response);

                Console.WriteLine($"Successfully parsed {recipeResponse.Recipes.Count} recipes");

                // Return the recipes list
                return recipeResponse.Recipes ?? new List<Recipe>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error fetching recipes: {ex.Message}");
                throw new Exception("Could not connect to recipe API. Check your internet connection.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error parsing recipes: {ex.Message}");
                throw new Exception("Could not parse recipe data from API.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        // Get a single recipe by ID
        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            try
            {
                var url = $"{BASE_URL}/{id}";
                Console.WriteLine($"Fetching recipe ID {id}...");

                var response = await _httpClient.GetStringAsync(url);

                // Single recipe is returned directly (not in "recipes" array)
                var recipe = JsonConvert.DeserializeObject<Recipe>(response);

                Console.WriteLine($"Successfully fetched recipe: {recipe.Name}");

                return recipe;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error fetching recipe {id}: {ex.Message}");
                throw new Exception($"Could not fetch recipe with ID {id}.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error parsing recipe {id}: {ex.Message}");
                throw new Exception("Could not parse recipe data.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        // Search recipes by keyword
        public async Task<List<Recipe>> SearchRecipesAsync(string searchTerm)
        {
            try
            {
                var url = $"{BASE_URL}/search?q={searchTerm}";
                Console.WriteLine($"Searching for: {searchTerm}");

                var response = await _httpClient.GetStringAsync(url);
                var recipeResponse = JsonConvert.DeserializeObject<RecipeResponse>(response);

                Console.WriteLine($"Found {recipeResponse.Recipes.Count} recipes");

                return recipeResponse.Recipes ?? new List<Recipe>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error searching recipes: {ex.Message}");
                throw new Exception("Could not search recipes. Check your internet connection.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error parsing search results: {ex.Message}");
                throw new Exception("Could not parse search results.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        // Filter recipes by cuisine type
        public Task<List<Recipe>> FilterByCuisineAsync(string cuisineType)
        {
            // TODO: Implement using LINQ on fetched recipes
            throw new NotImplementedException("Filter by cuisine - Coming soon");
        }

        // Filter recipes by maximum total time
        public Task<List<Recipe>> FilterByTimeAsync(int maxMinutes)
        {
            // TODO: Implement using LINQ on fetched recipes
            throw new NotImplementedException("Filter by time - Coming soon");
        }
    }
}
