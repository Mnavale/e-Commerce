using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceApp.Controllers
{
    [Authorize(Roles = Role.Buyer + "," + Role.Seller + "," + Role.Admin)]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int productId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var review = new Review
            {
                ProductId = productId,
                Rating = rating,
                Comment = comment,
                UserId = user.Id
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}
