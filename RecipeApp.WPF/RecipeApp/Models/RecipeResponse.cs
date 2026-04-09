using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Models
{
    /// <summary>
    /// Wrapper for DummyJSON API response
    /// The API returns: { "recipes": [...], "total": 50, "skip": 0, "limit": 30 }
    /// </summary>
    public class RecipeResponse
    {
        public List<Recipe> Recipes { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}
