using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.DAL;
using ProniaAPK.Models;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDBContext _context;

        public SizeController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.Include(s => s.ProductSizes).ToListAsync();
            return View(sizes);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM createVM)
        {
            if (!ModelState.IsValid) return View();
            bool result = await _context.Sizes.AnyAsync(s => s.Name.Trim() == createVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError(nameof(createVM.Name), "Size already is Exists");
                return View();
            }
            Size size = new()
            {
                Name = createVM.Name,
            };
            _context.Sizes.Add(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return BadRequest();
            UpdateSizeVM updateVM = new()
            {
                Name = size.Name,
            };

            return View(updateVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSizeVM updateVM)
        {
            if (id == null || id < 1) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(updateVM);
            }
            bool result = await _context.Sizes.AnyAsync(s => s.Name.Trim() == updateVM.Name.Trim() && s.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(updateVM.Name), "Size already has exists");
                return View();
            }

            size.Name = updateVM.Name;
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Size size = _context.Sizes.FirstOrDefault(s => s.Id == id);
            if (size == null) return NotFound();

            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
