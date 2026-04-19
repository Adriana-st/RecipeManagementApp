using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Shared
{
    /// <summary>
    /// Static helper methods extracted for testability and reuse.
    /// </summary>
    public static class RecipeHelper
    {
        /// <summary>
        /// Returns a sort order number for a meal type.
        /// Breakfast comes first, Snack last.
        /// </summary>
        public static int GetMealTypeOrder(string mealType) => mealType switch
        {
            "Breakfast" => 0,
            "Lunch" => 1,
            "Dinner" => 2,
            "Snack" => 3,
            _ => 4
        };

        /// <summary>
        /// Returns the Monday of the week containing the given date.
        /// </summary>
        public static DateTime GetWeekStart(DateTime date)
        {
            var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
            return date.Date.AddDays(-daysFromMonday);
        }
    }
}
