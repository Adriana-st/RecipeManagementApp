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
        public string ImageUrl { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }
        public string CuisineType { get; set; }
        public bool IsFavourite { get; set; }
        public DateTime DateAdded { get; set; }

        // Computed property - calculates automatically
        public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;

        // Constructor - runs when you create a new Recipe
        public Recipe()
        {
            Ingredients = new List<string>();
            Instructions = new List<string>();
            DateAdded = DateTime.Now;
        }
    }

}
