using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.Areas.Admin.ViewModels.Colors;
using ProniaAPK.DAL;
using ProniaAPK.Models;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDBContext _context;

        public ColorController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM createVm)
        {
            if (!ModelState.IsValid) return View();
            bool result = await _context.Colors.AnyAsync(c => c.Name == createVm.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(createVm.Name), "This Color Already has exists");
            }
            Color color = new() { Name = createVm.Name };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color == null) return NotFound();
            UpdateColorVm updateVM = new()
            {
                Name = color.Name,
            };
            return View(updateVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateColorVm updateVM)
        {
            if (id == null || id < 1) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color == null) return NotFound();
            if (!ModelState.IsValid) return View();
            if (await _context.Colors.AnyAsync(c => c.Name.Trim() == updateVM.Name && c.Id != id))
            {
                ModelState.AddModelError(nameof(updateVM.Name), "Color already exists");
            }
            color.Name = updateVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
