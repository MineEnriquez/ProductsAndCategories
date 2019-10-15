using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsAndCategories.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        
        [Required]
        [MinLength(4)]
        [Display(Name = "Product Name:")]
        public string Name { get; set; }

        [Required]
        [MinLength(4)]
        [Display(Name = "Product Description")]
        public string Description { get; set; }

        [Required]
        [Range(1,1000)]
        [Display(Name = "Product Price:")]
        public int Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Association> ListOfAssociations { get; set; }
    }
}