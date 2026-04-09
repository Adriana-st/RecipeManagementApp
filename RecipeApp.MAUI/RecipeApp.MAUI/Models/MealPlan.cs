using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RecipeApp.MAUI.Models
{
    public class MealPlan
    {
        [PrimaryKey, AutoIncrement]
        public int MealPlanId { get; set; }

        public DateTime Date { get; set; }

        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack

        // Foreign key to Recipe DatabaseId
        public int RecipeDatabaseId { get; set; }

        // Store recipe name (in case recipe is deleted)
        public string RecipeName { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }

        public MealPlan()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
