using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.DAL;
using ProniaAPK.Models;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            int lastDotIndex = 0;
            //if (!ModelState.IsValid) return View();
            if (!slide.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Sekil yaz ayy gede");
                return View();
            }
            if (!(slide.Photo.Length < 2 * 1024 * 1024))
            {
                ModelState.AddModelError("Photo", "Size is overload");
                return View();
            }
            lastDotIndex = slide.Photo.FileName.LastIndexOf(".");
            string fileName = Guid.NewGuid().ToString().Substring(0, 15) + slide.Photo.FileName.Substring(lastDotIndex, (slide.Photo.FileName.Length - lastDotIndex));
            string path = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", fileName);
            FileStream fileStream = new(path, FileMode.Create);
            await slide.Photo.CopyToAsync(fileStream);
            slide.Image = fileName;

            fileStream.Close();

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
