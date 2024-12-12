using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.DAL;
using ProniaAPK.Models;
using ProniaAPK.Utilities.Extensions;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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
                Categories = await _context.Categories.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync()
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if (!productVM.MainPhoto.CheckFileType("image/"))
            {
                ModelState.AddModelError(nameof(productVM.MainPhoto), "File Type is incorrect");
                return View(productVM);
            }
            if (!productVM.MainPhoto.CheckFileSize(Utilities.Enums.FileSize.MB, 1))
            {
                ModelState.AddModelError(nameof(productVM.MainPhoto), "Size is incorrect");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.CheckFileType("image/"))
            {
                ModelState.AddModelError(nameof(productVM.HoverPhoto), "File Type is incorrect");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.CheckFileSize(Utilities.Enums.FileSize.MB, 1))
            {
                ModelState.AddModelError(nameof(productVM.HoverPhoto), "Size is incorrect");
                return View(productVM);
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateAdminProductVM.CategoryId), "cATEGORY IS NOT CORRECT");
                return View(productVM);
            }
            if (productVM.TagIds is not null)
            {
                bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
                if (tagResult)
                {
                    ModelState.AddModelError(nameof(CreateAdminProductVM.TagIds), "Tags are wrong");
                    return View();
                }

            }
            if (productVM.ColorIds is not null)
            {

                bool colorResult = productVM.ColorIds.Any(cId => !productVM.Colors.Exists(c => c.Id == cId));
                if (colorResult)
                {
                    ModelState.AddModelError(nameof(CreateAdminProductVM.ColorIds), "Colors are wrong");
                    return View();
                }
            }
            if (productVM.SizeIds is not null)
            {

                bool sizeResult = productVM.SizeIds.Any(sId => !productVM.Sizes.Exists(s => s.Id == sId));
                if (sizeResult)
                {
                    ModelState.AddModelError(nameof(CreateAdminProductVM.SizeIds), "Sizes are wrong");
                    return View();
                }
            }
            ProductImage main = new()
            {
                IsPrimary = true,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
            };
            ProductImage hover = new()
            {
                IsPrimary = false,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
            };

            Product product = new()
            {
                Name = productVM.Name,
                Price = productVM.Price.Value,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId.Value,
                Description = productVM.Description,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ProductImages = new() { main, hover },
                ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList(),
                ProductSizes = productVM.SizeIds.Select(sId => new ProductSize { SizeId = sId }).ToList(),
                ProductColors = productVM.ColorIds.Select(cId => new ProductColor { ColorId = cId }).ToList(),
            };
            string text = String.Empty;
            if (productVM.AdditionalPhotos is null)
            {
                productVM.AdditionalPhotos = new List<IFormFile>();
            }
            foreach (IFormFile file in productVM.AdditionalPhotos)
            {
                if (!file.CheckFileType("image/"))
                {
                    text += $"<p class=\"text-warning\">{file.FileName} type does not correct</p>";
                    continue;
                }
                if (!file.CheckFileSize(Utilities.Enums.FileSize.MB, 1))
                {
                    text += $"<p class=\"text-warning\">{file.FileName} size does not correct</p>";

                    continue;
                }
                product.ProductImages.Add(
                    new ProductImage
                    {
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        ImageURL = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        IsPrimary = null
                    }
                    );
            }
            TempData["FileWarning"] = text;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            Product product = await _context.Products.Include(p => p.Category).Include(p => p.ProductTags).Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            UpdateProductVM productVM = new()
            {
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                TagIds = product.ProductTags.Select(pt => pt.TagId).ToList(),
                SizeIds = product.ProductSizes.Select(pt => pt.SizeId).ToList(),
                ColorIds = product.ProductColors.Select(pt => pt.ColorId).ToList(),
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == null) return BadRequest();
            Product existed = await _context.Products.Include(p => p.ProductTags).Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();
            productVM.Categories = await _context.Categories.ToListAsync();

            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            if (!ModelState.IsValid) return View(productVM);




            if (existed.CategoryId != productVM.CategoryId)
            {
                bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
                if (!result)
                {
                    ModelState.AddModelError(nameof(productVM.CategoryId), "Category is wrong");
                    return View(productVM);
                }
            }
            if (productVM.TagIds is not null)
            {
                bool tagResult = productVM.TagIds.Any(tID => !productVM.Tags.Exists(t => t.Id == tID));
                if (tagResult)
                {
                    ModelState.AddModelError(nameof(productVM.TagIds), "Tags are wrong");
                    return View(productVM);
                }
            }
            if (productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }
            else
            {
                productVM.TagIds = productVM.TagIds.Distinct().ToList();
            }
            if (productVM.ColorIds is not null)
            {
                bool colorResult = productVM.ColorIds.Any(cId => !productVM.Colors.Exists(c => c.Id == cId));
                if (colorResult)
                {
                    ModelState.AddModelError(nameof(productVM.ColorIds), "Color are wrong");
                    return View(productVM);
                }
            }
            if (productVM.ColorIds is null)
            {
                productVM.ColorIds = new();
            }
            if (productVM.SizeIds is not null)
            {
                bool sizeResult = productVM.SizeIds.Any(sId => !productVM.Sizes.Exists(s => s.Id == sId));
                if (sizeResult)
                {
                    ModelState.AddModelError(nameof(productVM.SizeIds), "Sizes are wrong");
                    return View(productVM);
                }
            }
            if (productVM.SizeIds is null)
            {
                productVM.SizeIds = new();
            }

            _context.ProductTags.RemoveRange(existed.ProductTags
            .Where(pTag => !productVM.TagIds
            .Exists(tId => tId == pTag.TagId))
            .ToList());

            _context.ProductTags
            .AddRange(productVM.TagIds.Where(pTag => !existed.ProductTags
            .Exists(p => p.Id == pTag))
            .Select(tID => new ProductTag { TagId = tID, ProductId = existed.Id }));





            _context.ProductColors
           .RemoveRange(existed.ProductColors
           .Where(pCol => !productVM.ColorIds
           .Exists(cId => cId == pCol.ColorId))
           .ToList());

            _context.ProductColors
            .AddRange(productVM.ColorIds.Where(pCol => !existed.ProductColors
            .Exists(p => p.Id == pCol))
            .Select(cId => new ProductColor { ColorId = cId, ProductId = existed.Id }));





            _context.ProductSizes
           .RemoveRange(existed.ProductSizes
           .Where(pSize => !productVM.SizeIds
           .Exists(sId => sId == pSize.SizeId))
           .ToList());

            _context.ProductSizes
            .AddRange(productVM.SizeIds.Where(pSize => !existed.ProductSizes
            .Exists(p => p.Id == pSize))
            .Select(sId => new ProductSize { SizeId = sId, ProductId = existed.Id }));


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
