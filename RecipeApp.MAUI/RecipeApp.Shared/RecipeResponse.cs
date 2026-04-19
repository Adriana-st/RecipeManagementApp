using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RecipeApp.Shared
{
    /// <summary>
    /// Response wrapper from DummyJSON API
    /// </summary>
    public class RecipeResponse
    {
        [JsonProperty("recipes")]
        public List<Recipe> Recipes { get; set; }

        public int Total { get; set; }

        public int Skip { get; set; }

        public int Limit { get; set; }
    }
}
