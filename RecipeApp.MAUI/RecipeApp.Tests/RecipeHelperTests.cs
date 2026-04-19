using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeApp.Shared;

namespace RecipeApp.Tests
{
    [TestClass]
    public class RecipeHelperTests
    {
        // ── GetMealTypeOrder ─────────────────────────────────────

        [TestMethod]
        public void GetMealTypeOrder_Breakfast_Returns0()
        {
            var result = RecipeHelper.GetMealTypeOrder("Breakfast");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetMealTypeOrder_Lunch_Returns1()
        {
            var result = RecipeHelper.GetMealTypeOrder("Lunch");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GetMealTypeOrder_Dinner_Returns2()
        {
            var result = RecipeHelper.GetMealTypeOrder("Dinner");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetMealTypeOrder_Snack_Returns3()
        {
            var result = RecipeHelper.GetMealTypeOrder("Snack");
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GetMealTypeOrder_UnknownType_Returns4()
        {
            var result = RecipeHelper.GetMealTypeOrder("Brunch");
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void GetMealTypeOrder_MealsSort_CorrectOrder()
        {
            var meals = new[] { "Snack", "Dinner", "Breakfast", "Lunch" };

            var sorted = meals.OrderBy(RecipeHelper.GetMealTypeOrder).ToList();

            Assert.AreEqual("Breakfast", sorted[0]);
            Assert.AreEqual("Lunch", sorted[1]);
            Assert.AreEqual("Dinner", sorted[2]);
            Assert.AreEqual("Snack", sorted[3]);
        }

        // ── GetWeekStart ─────────────────────────────────────────

        [TestMethod]
        public void GetWeekStart_Monday_ReturnsSameDay()
        {
            var monday = new DateTime(2025, 4, 14); // known Monday
            var result = RecipeHelper.GetWeekStart(monday);
            Assert.AreEqual(monday, result);
        }

        [TestMethod]
        public void GetWeekStart_Wednesday_ReturnsPreviousMonday()
        {
            var wednesday = new DateTime(2025, 4, 16);
            var expectedMonday = new DateTime(2025, 4, 14);

            var result = RecipeHelper.GetWeekStart(wednesday);

            Assert.AreEqual(expectedMonday, result);
        }

        [TestMethod]
        public void GetWeekStart_Sunday_ReturnsPreviousMonday()
        {
            var sunday = new DateTime(2025, 4, 20);
            var expectedMonday = new DateTime(2025, 4, 14);

            var result = RecipeHelper.GetWeekStart(sunday);

            Assert.AreEqual(expectedMonday, result);
        }
    }
}
