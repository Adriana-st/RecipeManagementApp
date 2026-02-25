using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Models
{
    /// <summary>
    /// Represents a recipe with all its details
    /// Demonstrates: Classes/Objects, Lists, Working with Dates
    /// </summary>
    public class Recipe
    {
        // Properties - what data does a recipe have?
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }

        // API uses "image", we use ImageUrl internally
        [JsonProperty("image")]
        public string ImageUrl { get; set; }
        // API uses "prepTimeMinutes"
        [JsonProperty("prepTimeMinutes")]
        public int PrepTimeMinutes { get; set; }
        // API uses "cookTimeMinutes"
        [JsonProperty("cookTimeMinutes")]
        public int CookTimeMinutes { get; set; }

        public int Servings { get; set; }
        // API uses "cuisine"
        [JsonProperty("cuisine")]
        public string CuisineType { get; set; }
        public string Difficulty { get; set; }

        // Not from API - we'll use these later
        public bool IsFavourite { get; set; }
        public DateTime DateAdded { get; set; }

        // Computed property - calculates automatically
        public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;

        // For displaying ingredients in list view (not from API)
        [JsonIgnore]
        public string IngredientsDisplay { get; set; }

        // Constructor - runs when you create a new Recipe
        public Recipe()
        {
            Ingredients = new List<string>();
            Instructions = new List<string>();
            DateAdded = DateTime.Now;
        }
    }

}
