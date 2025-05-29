using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
