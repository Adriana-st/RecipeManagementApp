using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using RecipeApp.MAUI.Models;

namespace RecipeApp.MAUI.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private bool _initialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public DatabaseService()
        {
            System.Diagnostics.Debug.WriteLine("🔵 DatabaseService constructor");
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_initialized)
                    return;

                System.Diagnostics.Debug.WriteLine("🔵 Initializing database...");

                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "RecipeApp.db3");
                System.Diagnostics.Debug.WriteLine($"🔵 Database path: {dbPath}");

                _database = new SQLiteAsyncConnection(dbPath);

                await _database.CreateTableAsync<Recipe>();
                await _database.CreateTableAsync<MealPlan>();

                _initialized = true;
                System.Diagnostics.Debug.WriteLine("✅ Database initialized successfully");
            }
            finally
            {
                _initLock.Release();
            }
        }

        #region Recipe Operations

        /// <summary>
        /// Get all favourite recipes
        /// </summary>
        public async Task<List<Recipe>> GetFavouritesAsync()
        {
            await EnsureInitializedAsync();

            var recipes = await _database.Table<Recipe>()
                .Where(r => r.IsFavourite)
                .OrderByDescending(r => r.DateAdded)
                .ToListAsync();

            foreach (var recipe in recipes)
                recipe.LoadFromDatabase();

            return recipes;
        }

        /// <summary>
        /// Save recipe to database
        /// Custom recipes are always saved as favourites
        /// </summary>
        public async Task<int> SaveRecipeAsync(Recipe recipe)
        {
            await EnsureInitializedAsync();

            recipe.PrepareForDatabase();
            recipe.IsFavourite = true;

            if (recipe.Source == "Custom")
            {
                // Custom recipe — insert only if not already in DB
                if (recipe.DatabaseId > 0)
                {
                    await _database.UpdateAsync(recipe);
                    return recipe.DatabaseId;
                }

                await _database.InsertAsync(recipe);
                return recipe.DatabaseId;
            }
            else
            {
                // API recipe — check by ApiId to avoid duplicates
                var existing = await _database.Table<Recipe>()
                    .Where(r => r.ApiId == recipe.ApiId && r.Source == "API")
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    existing.IsFavourite = true;
                    await _database.UpdateAsync(existing);
                    return existing.DatabaseId;
                }

                await _database.InsertAsync(recipe);
                return recipe.DatabaseId;
            }
        }

        /// <summary>
        /// Remove from favourites
        /// </summary>
        public async Task<int> DeleteRecipeAsync(Recipe recipe)
        {
            await EnsureInitializedAsync();
            // Both custom and API recipes are fully deleted
            return await _database.DeleteAsync(recipe);
        }

        /// <summary>
        /// Check if recipe is saved as favourite
        /// </summary>
        public async Task<bool> IsRecipeSavedAsync(int? apiId)
        {
            await EnsureInitializedAsync();

            if (!apiId.HasValue)
                return false;

            var count = await _database.Table<Recipe>()
                .Where(r => r.ApiId == apiId && r.IsFavourite)
                .CountAsync();

            return count > 0;
        }

        /// <summary>
        /// Get all custom recipes — regardless of favourite status
        /// </summary>
        public async Task<List<Recipe>> GetCustomRecipesAsync()
        {
            await EnsureInitializedAsync();

            var recipes = await _database.Table<Recipe>()
                .Where(r => r.Source == "Custom")
                .OrderByDescending(r => r.DateAdded)
                .ToListAsync();

            foreach (var recipe in recipes)
                recipe.LoadFromDatabase();

            return recipes;
        }

        #endregion

        #region Meal Plan Operations

        /// <summary>
        /// Get meal plans for a week
        /// </summary>
        public async Task<List<MealPlan>> GetWeekMealPlansAsync(DateTime weekStart)
        {
            await EnsureInitializedAsync();

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
            await EnsureInitializedAsync();
            return await _database.InsertAsync(mealPlan);
        }

        /// <summary>
        /// Delete meal plan
        /// </summary>
        public async Task<int> DeleteMealPlanAsync(MealPlan mealPlan)
        {
            await EnsureInitializedAsync();
            return await _database.DeleteAsync(mealPlan);
        }

        /// <summary>
        /// Clear all meal plans for a week
        /// </summary>
        public async Task<int> ClearWeekAsync(DateTime weekStart)
        {
            await EnsureInitializedAsync();

            var meals = await GetWeekMealPlansAsync(weekStart);
            int count = 0;
            foreach (var meal in meals)
                count += await DeleteMealPlanAsync(meal);

            return count;
        }

        #endregion
    }
}