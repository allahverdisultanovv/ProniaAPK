using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.DAL;
using ProniaAPK.Models;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetCategoriesAdminVM> categoryVMs = await _context.Categories.Where(c => !c.IsDeleted).Include(c => c.Products).Select(c => new GetCategoriesAdminVM
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count()
            }).ToListAsync();
            return View(categoryVMs);
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim());
            if (result)
            {
                ModelState.AddModelError(nameof(category.Name), "Category is already existed");
                return View();
            }
            category.CreatedAt = DateTime.Now;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {

            if (id < 1 || id is null) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);

        }


        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {

            if (id < 1 || id is null) return BadRequest();
            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim() && c.Id != id))
            {
                ModelState.AddModelError(nameof(category.Name), "Category already exists");
                return View();
            }

            existed.Name = category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id < 1 || id is null) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            category.IsDeleted = true;
            //_context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
