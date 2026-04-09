using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RecipeApp.MAUI.Models;

namespace RecipeApp.MAUI.Services
{
    /// <summary>
    /// Service for fetching recipes from DummyJSON API
    /// </summary>
    public class RecipeApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://dummyjson.com/recipes";

        public RecipeApiService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Get all recipes from API
        /// </summary>
        public async Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(BaseUrl);
                var recipeResponse = JsonConvert.DeserializeObject<RecipeResponse>(response);

                return recipeResponse?.Recipes ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error fetching recipes: {ex.Message}");
                throw;
            }
        }
    }
}
