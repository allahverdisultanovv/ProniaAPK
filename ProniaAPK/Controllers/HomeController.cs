using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.DAL;
using ProniaAPK.ViewModels;

namespace ProniaAPK.Controllers
{
    public class HomeController : Controller
    {
        public readonly AppDBContext _context;
        public HomeController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()

        {


            HomeVM homeVM = new HomeVM
            {
                Slides = await _context.Slides.Take(2).OrderBy(s => s.Order).ToListAsync(),

                NewProducts = await _context.Products.Take(4)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync(),
            };
            return View(homeVM);
        }
    }
}
