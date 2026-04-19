using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace RecipeApp.Shared
{
    /// <summary>
    /// Recipe model with improved ID structure
    /// DatabaseId: Always the primary key
    /// ApiId: Only set for API recipes, null for custom
    /// </summary>
    public class Recipe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Database Primary Key
        [PrimaryKey, AutoIncrement]
        public int DatabaseId { get; set; }

        // API ID (only set for API recipes, null for custom)
        [JsonProperty("id")]
        public int? ApiId { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        // Store as JSON string in database
        [Ignore]
        public List<string> Ingredients { get; set; }

        public string IngredientsJson { get; set; }

        // Store as JSON string in database
        [Ignore]
        public List<string> Instructions { get; set; }

        public string InstructionsJson { get; set; }

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
        private bool _isFavourite;
        public bool IsFavourite
        {
            get => _isFavourite;
            set
            {
                if (_isFavourite != value)
                {
                    _isFavourite = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DateAdded { get; set; }

        // "API" or "Custom"
        public string Source { get; set; }

        // Computed property
        [Ignore]
        public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;

        [Ignore]
        public bool IsCustom => Source == "Custom";

        // For displaying in list view - show API image or default if missing
        [Ignore]
        public string DisplayImage => string.IsNullOrWhiteSpace(ImageUrl)
            ? "no_image.png"
            : ImageUrl;

        // For displaying in list view - show first 3 ingredients or empty if none
        [Ignore]
        public string IngredientsDisplay => Ingredients?.Any() == true
            ? "🥗 " + string.Join(", ", Ingredients.Take(3)) + (Ingredients.Count > 3 ? "..." : "")
            : string.Empty;

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
            Ingredients = !string.IsNullOrEmpty(IngredientsJson)
                ? JsonConvert.DeserializeObject<List<string>>(IngredientsJson) ?? new List<string>()
                : new List<string>();

            Instructions = !string.IsNullOrEmpty(InstructionsJson)
                ? JsonConvert.DeserializeObject<List<string>>(InstructionsJson) ?? new List<string>()
                : new List<string>();
        }
    }
}
