using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RecipeApp.Shared
{
    public class MealPlan
    {
        [PrimaryKey, AutoIncrement]
        public int MealPlanId { get; set; }

        public DateTime Date { get; set; }

        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack }

        // Store recipe name (in case recipe is deleted)
        public string RecipeName { get; set; }

        public DateTime CreatedDate { get; set; }

        public MealPlan()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
