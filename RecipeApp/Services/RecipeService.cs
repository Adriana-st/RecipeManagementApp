using RecipeApp.Data;
using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecipeApp.Services
{
    /// <summary>
    /// Service for database operations on recipes
    /// Demonstrates: Database operations, LINQ queries
    /// </summary>
    public class RecipeService
    {
        // Save a recipe to favourites
        public async Task<bool> SaveToFavouritesAsync(Recipe recipe)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        // Check if recipe already exists (by API Id)
                        var existing = db.Recipes.FirstOrDefault(r => r.Id == recipe.Id && r.Source == "API");

                        if (existing != null)
                        {
                            // Already saved
                            return;
                        }


                        // Prepare recipe for database
                        recipe.IsFavourite = true;
                        recipe.DateAdded = DateTime.Now;
                        recipe.Source = "API";
                        recipe.PrepareForDatabase();

                        // Add to database
                        db.Recipes.Add(recipe);
                        db.SaveChanges();

                        System.Diagnostics.Debug.WriteLine($"✅ Saved recipe: {recipe.Name}");
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error saving recipe: {ex.Message}");
                return false;
            }
        }


        // Get all favourite recipes from database
        // Demonstrates: LINQ query
        public async Task<List<Recipe>> GetFavouritesAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        // LINQ query - get all favourites, ordered by date
                        var recipes = db.Recipes
                            .Where(r => r.IsFavourite)
                            .OrderByDescending(r => r.DateAdded)
                            .ToList();

                        // Load lists from JSON
                        foreach (var recipe in recipes)
                        {
                            recipe.LoadFromDatabase();
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ Loaded {recipes.Count} favourites from database");

                        return recipes;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading favourites: {ex.Message}");
                return new List<Recipe>();
            }
        }

        // Remove a recipe from favourites
        public async Task<bool> RemoveFromFavouritesAsync(int recipeId)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        var recipe = db.Recipes.FirstOrDefault(r => r.Id == recipeId);

                        if (recipe != null)
                        {
                            db.Recipes.Remove(recipe);
                            db.SaveChanges();
                            System.Diagnostics.Debug.WriteLine($"✅ Removed recipe: {recipe.Name}");
                        }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error removing recipe: {ex.Message}");
                return false;
            }
        }

        // Check if a recipe is already saved
        // Demonstrates: LINQ Any() method
        public async Task<bool> IsSavedAsync(int recipeId)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        return db.Recipes.Any(r => r.Id == recipeId && r.Source == "API");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error checking if saved: {ex.Message}");
                return false;
            }
        }

        // Get count of saved recipes
        // Demonstrates: LINQ Count()
        public async Task<int> GetFavouritesCountAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        return db.Recipes.Count(r => r.IsFavourite);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting count: {ex.Message}");
                return 0;
            }
        }
        
    }
}

