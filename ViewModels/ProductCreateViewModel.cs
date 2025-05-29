using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string Category { get; set; }

        public int StockQuantity { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
