
using System.Collections.Generic;

namespace ProductsAndCategories.Models
{
    public class ProductsPageModel
    {
        public Product OneProduct { get; set; }
        public List<Product> Products { get; set; }
    }
}