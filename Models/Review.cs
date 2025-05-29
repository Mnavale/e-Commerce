using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
