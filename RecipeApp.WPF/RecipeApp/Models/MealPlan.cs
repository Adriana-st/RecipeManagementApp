using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.Models
{
    /// <summary>
    /// Represents a planned meal for a specific day and meal type
    /// Demonstrates: Database relationships, enums, date handling
    /// </summary>
    public class MealPlan
    {
        [Key]
        public int MealPlanId { get; set; }

        public DateTime Date { get; set; }

        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack

        // Foreign key to Recipe
        public int RecipeId { get; set; }

        // Navigation property (not stored in DB)
        [NotMapped]
        public Recipe Recipe { get; set; }

        // Store recipe name for display (in case recipe is deleted)
        public string RecipeName { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }

        public MealPlan()
        {
            CreatedDate = DateTime.Now;
        }
    }
}

