using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeApp.Models;

namespace RecipeApp.Data
{
    /// <summary>
    /// Database context - manages connection to LocalDB database
    /// Demonstrates: Database + LINQ requirement
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor - uses default LocalDB connection
        /// </summary>
        public AppDbContext() : base("RecipeDb")
        {
            // Enable lazy loading
            Configuration.LazyLoadingEnabled = true;

            // Log SQL queries to console
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        /// <summary>
        /// Recipes table in database
        /// </summary>
        public DbSet<Recipe> Recipes { get; set; }

        /// <summary>
        /// Meal plans table in database
        /// </summary>
        public DbSet<MealPlan> MealPlans { get; set; }

        /// <summary>
        /// Configure model relationships and constraints
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Recipe entity
            modelBuilder.Entity<Recipe>()
                .HasKey(r => r.DatabaseId);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.IngredientsJson)
                .IsRequired();

            modelBuilder.Entity<Recipe>()
                .Property(r => r.InstructionsJson)
                .IsRequired();

            // Configure MealPlan entity
            modelBuilder.Entity<MealPlan>()
                .HasKey(m => m.MealPlanId);

            modelBuilder.Entity<MealPlan>()
                .Property(m => m.MealType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<MealPlan>()
                .Property(m => m.RecipeName)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
