using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.DAL;
using ProniaAPK.Models;
using ProniaAPK.Utilities.Extensions;

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
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid) return View();
            if (!slideVM.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Sekil yaz ayy gede");
                return View();
            }
            if (!slideVM.Photo.CheckFileSize(Utilities.Enums.FileSize.MB, 2))
            {
                ModelState.AddModelError("Photo", "Size is overload");
                return View();
            }
            Slide slide = new Slide
            {
                Title = slideVM.Title,
                SubTitle = slideVM.SubTitle,
                Description = slideVM.Description,
                Order = slideVM.Order,
                Image = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };




            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            UpdateSlideVM updateSlideVM = new UpdateSlideVM()
            {
                Title = slide.Title,
                SubTitle = slide.SubTitle,
                Description = slide.Description,
                Order = slide.Order,
                Image = slide.Image

            };
            return View(updateSlideVM);



        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSlideVM updateVM)
        {

            if (!ModelState.IsValid) return View(updateVM);
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            if (updateVM.Photo is not null)
            {
                if (!updateVM.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError(nameof(updateVM.Photo), "Type is incorrect");
                    return View(updateVM);
                }
                if (!updateVM.Photo.CheckFileSize(Utilities.Enums.FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(updateVM.Photo), "Size must be less than 2MB");
                    return View(updateVM);
                }
                string fileName = await updateVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                slide.Image = fileName;

            }
            slide.Title = updateVM.Title;
            slide.Description = updateVM.Description;
            slide.SubTitle = updateVM.SubTitle;
            slide.Order = updateVM.Order;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();

            slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            _context.Remove(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
