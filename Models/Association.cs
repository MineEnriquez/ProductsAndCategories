namespace ProductsAndCategories.Models
{
    public class Association
    {
        public int AssociationId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        public Product AssociatedProduct {get; set;}
        public Category AssociatedCategory {get; set;}
    }
}