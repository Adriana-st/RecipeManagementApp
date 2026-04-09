using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        // Database primary key
        [Key]
        [JsonIgnore]
        public int DatabaseId { get; set; }

        // API ID (from DummyJSON)
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Store as JSON string in database
        [NotMapped]
        public List<string> Ingredients { get; set; }
        [JsonIgnore]
        public string IngredientsJson { get; set; }

        // Store as JSON string in database
        [NotMapped]
        public List<string> Instructions { get; set; }
        [JsonIgnore]
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

        // Database fields (not from API)
        [JsonIgnore]
        public bool IsFavourite { get; set; }
        [JsonIgnore]
        public DateTime DateAdded { get; set; }
        [JsonIgnore]
        public string Source { get; set; } // "API" or "Custom"

        // Computed property
        [NotMapped]
        [JsonIgnore]
        public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;

        // For displaying ingredients in list view (not from API  or DB)
        [NotMapped]
        [JsonIgnore]
        public string IngredientsDisplay { get; set; }

        // Constructor - initializes lists and sets date
        public Recipe()
        {
            Ingredients = new List<string>();
            Instructions = new List<string>();
            DateAdded = DateTime.Now;
            Source = "API";
        }

        // Helper methods to convert lists to/from JSON for database storage
        public void PrepareForDatabase()
        {
            IngredientsJson = Newtonsoft.Json.JsonConvert.SerializeObject(Ingredients);
            InstructionsJson = Newtonsoft.Json.JsonConvert.SerializeObject(Instructions);
        }

        public void LoadFromDatabase()
        {
            if (!string.IsNullOrEmpty(IngredientsJson))
            {
                Ingredients = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(IngredientsJson);
            }

            if (!string.IsNullOrEmpty(InstructionsJson))
            {
                Instructions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(InstructionsJson);
            }
        }
    }

}
