using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.ViewModels
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; }

        public string SearchString { get; set; }
        public string Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
