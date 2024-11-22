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
        public IActionResult Index()

        {


            HomeVM homeVM = new HomeVM
            {
                Slides = _context.Slides.Take(2).OrderBy(s => s.Order).ToList(),
                Products = _context.Products.Include(p => p.productImages).ToList(),
            };
            return View(homeVM);
        }
    }
}
