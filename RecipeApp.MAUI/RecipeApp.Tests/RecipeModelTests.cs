using RecipeApp.Shared;

namespace RecipeApp.Tests
{
    [TestClass]
    public class RecipeModelTests
    {
        // ── TotalTimeMinutes ─────────────────────────────────────

        [TestMethod]
        public void TotalTimeMinutes_SumsPrepAndCookTime()
        {
            var recipe = new Recipe { PrepTimeMinutes = 15, CookTimeMinutes = 30 };
            Assert.AreEqual(45, recipe.TotalTimeMinutes);
        }

        [TestMethod]
        public void TotalTimeMinutes_ZeroValues_ReturnsZero()
        {
            var recipe = new Recipe { PrepTimeMinutes = 0, CookTimeMinutes = 0 };
            Assert.AreEqual(0, recipe.TotalTimeMinutes);
        }

        // ── IsCustom ─────────────────────────────────────────────

        [TestMethod]
        public void IsCustom_SourceIsCustom_ReturnsTrue()
        {
            var recipe = new Recipe { Source = "Custom" };
            Assert.IsTrue(recipe.IsCustom);
        }

        [TestMethod]
        public void IsCustom_SourceIsAPI_ReturnsFalse()
        {
            var recipe = new Recipe { Source = "API" };
            Assert.IsFalse(recipe.IsCustom);
        }

        // ── DisplayImage ─────────────────────────────────────────

        [TestMethod]
        public void DisplayImage_NullImageUrl_ReturnsFallback()
        {
            var recipe = new Recipe { ImageUrl = null };
            Assert.AreEqual("no_image.png", recipe.DisplayImage);
        }

        [TestMethod]
        public void DisplayImage_ValidUrl_ReturnsUrl()
        {
            var url = "https://example.com/image.jpg";
            var recipe = new Recipe { ImageUrl = url };
            Assert.AreEqual(url, recipe.DisplayImage);
        }

        // ── PrepareForDatabase / LoadFromDatabase ─────────────────

        [TestMethod]
        public void PrepareForDatabase_SerializesIngredients()
        {
            var recipe = new Recipe();
            recipe.Ingredients = new List<string> { "Flour", "Eggs", "Milk" };

            recipe.PrepareForDatabase();

            Assert.IsFalse(string.IsNullOrEmpty(recipe.IngredientsJson));
            Assert.IsTrue(recipe.IngredientsJson.Contains("Flour"));
        }

        [TestMethod]
        public void LoadFromDatabase_DeserializesIngredients()
        {
            var recipe = new Recipe();
            recipe.Ingredients = new List<string> { "Flour", "Eggs", "Milk" };
            recipe.PrepareForDatabase();

            // Simulate loading from DB by clearing and reloading
            recipe.Ingredients = null;
            recipe.LoadFromDatabase();

            Assert.IsNotNull(recipe.Ingredients);
            Assert.AreEqual(3, recipe.Ingredients.Count);
            Assert.AreEqual("Flour", recipe.Ingredients[0]);
        }

        [TestMethod]
        public void PrepareAndLoad_RoundTrip_PreservesInstructions()
        {
            var recipe = new Recipe();
            recipe.Instructions = new List<string> { "Preheat oven", "Mix ingredients", "Bake for 30 mins" };

            recipe.PrepareForDatabase();
            recipe.Instructions = null;
            recipe.LoadFromDatabase();

            Assert.AreEqual(3, recipe.Instructions.Count);
            Assert.AreEqual("Bake for 30 mins", recipe.Instructions[2]);
        }
    }
}