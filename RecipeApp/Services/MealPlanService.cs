using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeApp.Data;
using RecipeApp.Models;

namespace RecipeApp.Services
{
    /// <summary>
    /// Service for meal planning operations
    /// Demonstrates: Complex LINQ queries, date filtering
    /// </summary>
    public class MealPlanService
    {
        /// <summary>
        /// Add a meal to the plan
        /// </summary>
        public async Task<bool> AddMealPlanAsync(MealPlan mealPlan)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        db.MealPlans.Add(mealPlan);
                        db.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"✅ Added meal plan: {mealPlan.RecipeName}");
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error adding meal plan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get meal plans for a specific week
        /// Demonstrates: LINQ date filtering
        /// </summary>
        public async Task<List<MealPlan>> GetWeekMealPlansAsync(DateTime weekStart)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        var weekEnd = weekStart.AddDays(7);

                        // LINQ query - get all meal plans for the week
                        var mealPlans = db.MealPlans
                            .Where(m => m.Date >= weekStart && m.Date < weekEnd)
                            .OrderBy(m => m.Date)
                            .ThenBy(m => m.MealType)
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"✅ Loaded {mealPlans.Count} meal plans for week");

                        return mealPlans;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading meal plans: {ex.Message}");
                return new List<MealPlan>();
            }
        }

        /// <summary>
        /// Get meal plans for a specific day
        /// </summary>
        public async Task<List<MealPlan>> GetDayMealPlansAsync(DateTime date)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        var dayStart = date.Date;
                        var dayEnd = dayStart.AddDays(1);

                        var mealPlans = db.MealPlans
                            .Where(m => m.Date >= dayStart && m.Date < dayEnd)
                            .OrderBy(m => m.MealType)
                            .ToList();

                        return mealPlans;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading day meal plans: {ex.Message}");
                return new List<MealPlan>();
            }
        }

        /// <summary>
        /// Remove a meal plan
        /// </summary>
        public async Task<bool> RemoveMealPlanAsync(int mealPlanId)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        var mealPlan = db.MealPlans.Find(mealPlanId);

                        if (mealPlan != null)
                        {
                            db.MealPlans.Remove(mealPlan);
                            db.SaveChanges();
                            System.Diagnostics.Debug.WriteLine($"✅ Removed meal plan");
                        }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error removing meal plan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all recipes used in meal plans
        /// Demonstrates: LINQ Distinct
        /// </summary>
        public async Task<List<int>> GetPlannedRecipeIdsAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        return db.MealPlans
                            .Select(m => m.RecipeId)
                            .Distinct()
                            .ToList();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting planned recipes: {ex.Message}");
                return new List<int>();
            }
        }

        /// <summary>
        /// Clear all meal plans for a specific week
        /// </summary>
        public async Task<bool> ClearWeekAsync(DateTime weekStart)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        var weekEnd = weekStart.AddDays(7);

                        var mealPlans = db.MealPlans
                            .Where(m => m.Date >= weekStart && m.Date < weekEnd)
                            .ToList();

                        db.MealPlans.RemoveRange(mealPlans);
                        db.SaveChanges();

                        System.Diagnostics.Debug.WriteLine($"✅ Cleared {mealPlans.Count} meal plans");
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error clearing week: {ex.Message}");
                return false;
            }
        }
    }
}
