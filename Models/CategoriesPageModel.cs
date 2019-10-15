using System.Collections.Generic;

namespace ProductsAndCategories.Models
{
    public class CategoriesPageModel
    {
        public Category OneCategory { get; set; }
        public List<Category> Categories { get; set; }
    }
}