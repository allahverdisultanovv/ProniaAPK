using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.DAL;
using ProniaAPK.Models;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetAdminProductVM> productVMs = await _context.Products.Include(p => p.Category)
            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
            .Select(p => new GetAdminProductVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                Image = p.ProductImages[0].ImageURL
            })
            .ToListAsync();
            return View(productVMs);
        }
        public async Task<IActionResult> Create()
        {
            CreateAdminProductVM productVM = new CreateAdminProductVM
            {
                Tags = await _context.Tags.ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateAdminProductVM.CategoryId), "cATEGORY IS NOT CORRECT");
                return View(productVM);
            }
            bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
            if (tagResult)
            {
                ModelState.AddModelError(nameof(CreateAdminProductVM.TagIds), "Tags are wrong");
                return View();
            }


            Product product = new()
            {
                Name = productVM.Name,
                Price = productVM.Price.Value,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId.Value,
                Description = productVM.Description,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList()
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            UpdateProductVM productVM = new()
            {
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == null) return BadRequest();
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid) return View(productVM);
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();


            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {

                return View(productVM);
            }

            existed.SKU = productVM.SKU;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price.Value;
            existed.CategoryId = productVM.CategoryId.Value;
            existed.Name = productVM.Name;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
