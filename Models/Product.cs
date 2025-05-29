using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string Category { get; set; }

        public int StockQuantity { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
