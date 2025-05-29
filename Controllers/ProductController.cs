using ECommerceApp.Data;
using ECommerceApp.Models;
using ECommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(string searchString, string category, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            int pageSize = 10;

            var productsQuery = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));

            if (!string.IsNullOrEmpty(category))
                productsQuery = productsQuery.Where(p => p.Category == category);

            if (minPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            var totalProducts = await productsQuery.CountAsync();

            var products = await productsQuery.OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new ProductIndexViewModel
            {
                Products = products,
                SearchString = searchString,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
            };

            return View(vm);
        }

        [Authorize(Roles = Role.Admin + "," + Role.Seller)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Role.Admin + "," + Role.Seller)]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                StockQuantity = model.StockQuantity,
                Images = new System.Collections.Generic.List<ProductImage>()
            };

            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    if (image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        product.Images.Add(new ProductImage { ImagePath = "/uploads/" + uniqueFileName });
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Role.Admin + "," + Role.Seller)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                StockQuantity = product.StockQuantity,
                ExistingImages = product.Images.ToList()
            };

            return View(model);
        }

        [Authorize(Roles = Role.Admin + "," + Role.Seller)]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Category = model.Category;
            product.StockQuantity = model.StockQuantity;

            // Handle new image uploads
            if (model.NewImages != null)
            {
                foreach (var image in model.NewImages)
                {
                    if (image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        product.Images.Add(new ProductImage { ImagePath = "/uploads/" + uniqueFileName });
                    }
                }
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Role.Admin + "," + Role.Seller)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            // Optional: Delete image files from wwwroot/uploads

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var avgRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0;

            var vm = new ProductDetailsViewModel
            {
                Product = product,
                AverageRating = avgRating
            };
            return View(vm);
        }
    }
}
