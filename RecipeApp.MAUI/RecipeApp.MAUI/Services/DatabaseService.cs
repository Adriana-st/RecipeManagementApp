using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using RecipeApp.MAUI.Models;

namespace RecipeApp.MAUI.Services
{
    /// <summary>
    /// SQLite database service for MAUI
    /// Uses async/await for better performance
    /// </summary>
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            InitializeDatabaseAsync().Wait();
        }

        private async Task InitializeDatabaseAsync()
        {
            if (_database != null)
                return;

            // Get database path
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "RecipeApp.db3");

            // Create connection
            _database = new SQLiteAsyncConnection(dbPath);

            // Create tables
            await _database.CreateTableAsync<Recipe>();
            await _database.CreateTableAsync<MealPlan>();

            System.Diagnostics.Debug.WriteLine($"✅ Database initialized at: {dbPath}");
        }

        #region Recipe Operations

        /// <summary>
        /// Get all favourite recipes
        /// Demonstrates: LINQ with async
        /// </summary>
        public async Task<List<Recipe>> GetFavouritesAsync()
        {
            var recipes = await _database.Table<Recipe>()
                .Where(r => r.IsFavourite)
                .OrderByDescending(r => r.DateAdded)
                .ToListAsync();

            // Load ingredients/instructions from JSON
            foreach (var recipe in recipes)
            {
                recipe.LoadFromDatabase();
            }

            return recipes;
        }

        /// <summary>
        /// Save recipe to favourites
        /// </summary>
        public async Task<int> SaveRecipeAsync(Recipe recipe)
        {
            recipe.PrepareForDatabase();
            recipe.IsFavourite = true;

            // Check if already exists (by ApiId for API recipes)
            Recipe existing = null;

            if (recipe.ApiId.HasValue && recipe.Source == "API")
            {
                existing = await _database.Table<Recipe>()
                    .Where(r => r.ApiId == recipe.ApiId && r.Source == "API")
                    .FirstOrDefaultAsync();
            }
            else if (recipe.DatabaseId > 0)
            {
                existing = await _database.Table<Recipe>()
                    .Where(r => r.DatabaseId == recipe.DatabaseId)
                    .FirstOrDefaultAsync();
            }

            if (existing != null)
            {
                // Already exists, don't save duplicate
                return existing.DatabaseId;
            }

            // Insert new recipe
            await _database.InsertAsync(recipe);
            return recipe.DatabaseId;
        }

        /// <summary>
        /// Delete recipe from favourites
        /// </summary>
        public async Task<int> DeleteRecipeAsync(Recipe recipe)
        {
            return await _database.DeleteAsync(recipe);
        }

        /// <summary>
        /// Check if recipe is saved
        /// </summary>
        public async Task<bool> IsRecipeSavedAsync(int? apiId)
        {
            if (!apiId.HasValue)
                return false;

            var count = await _database.Table<Recipe>()
                .Where(r => r.ApiId == apiId && r.IsFavourite)
                .CountAsync();

            return count > 0;
        }

        #endregion

        #region Meal Plan Operations

        /// <summary>
        /// Get meal plans for a week
        /// </summary>
        public async Task<List<MealPlan>> GetWeekMealPlansAsync(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);

            return await _database.Table<MealPlan>()
                .Where(m => m.Date >= weekStart && m.Date < weekEnd)
                .OrderBy(m => m.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Add meal plan
        /// </summary>
        public async Task<int> AddMealPlanAsync(MealPlan mealPlan)
        {
            return await _database.InsertAsync(mealPlan);
        }

        /// <summary>
        /// Delete meal plan
        /// </summary>
        public async Task<int> DeleteMealPlanAsync(MealPlan mealPlan)
        {
            return await _database.DeleteAsync(mealPlan);
        }

        /// <summary>
        /// Clear all meal plans for a week
        /// </summary>
        public async Task<int> ClearWeekAsync(DateTime weekStart)
        {
            var meals = await GetWeekMealPlansAsync(weekStart);

            int count = 0;
            foreach (var meal in meals)
            {
                count += await DeleteMealPlanAsync(meal);
            }

            return count;
        }

        #endregion
    }
}
