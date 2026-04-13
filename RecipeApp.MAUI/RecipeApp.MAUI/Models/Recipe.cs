using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Newtonsoft.Json;

namespace RecipeApp.MAUI.Models
{
    /// <summary>
    /// Recipe model with improved ID structure
    /// DatabaseId: Always the primary key
    /// ApiId: Only set for API recipes, null for custom
    /// </summary>
    public class Recipe
    {
        // Database Primary Key
        [PrimaryKey, AutoIncrement]
        public int DatabaseId { get; set; }

        // API ID (only set for API recipes, null for custom)
        [JsonProperty("id")]
        public int? ApiId { get; set; }

        public string Name { get; set; }

        [Ignore]
        public bool IsCustom => Source == "Custom";
        public string Description { get; set; }

        // Store as JSON string in database
        [Ignore]
        public List<string> Ingredients { get; set; }

        public string IngredientsJson { get; set; }

        // Store as JSON string in database
        [Ignore]
        public List<string> Instructions { get; set; }

        public string InstructionsJson { get; set; }

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

        // Database fields
        public bool IsFavourite { get; set; }

        public DateTime DateAdded { get; set; }

        // "API" or "Custom"
        public string Source { get; set; }

        // Computed property
        [Ignore]
        public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;

        // For displaying ingredients in list view
        [Ignore]
        public string IngredientsDisplay { get; set; }

        public Recipe()
        {
            Ingredients = new List<string>();
            Instructions = new List<string>();
            DateAdded = DateTime.Now;
            Source = "API";
            IngredientsJson = "";
            InstructionsJson = "";
        }

        // Helper methods
        public void PrepareForDatabase()
        {
            IngredientsJson = JsonConvert.SerializeObject(Ingredients ?? new List<string>());
            InstructionsJson = JsonConvert.SerializeObject(Instructions ?? new List<string>());
        }

        public void LoadFromDatabase()
        {
            if (!string.IsNullOrEmpty(IngredientsJson))
            {
                Ingredients = JsonConvert.DeserializeObject<List<string>>(IngredientsJson) ?? new List<string>();
            }
            else
            {
                Ingredients = new List<string>();
            }

            if (!string.IsNullOrEmpty(InstructionsJson))
            {
                Instructions = JsonConvert.DeserializeObject<List<string>>(InstructionsJson) ?? new List<string>();
            }
            else
            {
                Instructions = new List<string>();
            }
        }
    }
}
