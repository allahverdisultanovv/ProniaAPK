using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaAPK.DAL;
using ProniaAPK.Models;
using ProniaAPK.ViewModels;
using System.Security.Claims;


namespace ProniaAPK.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new();
            if (User.Identity.IsAuthenticated)
            {
            }
            else
            {

                List<BasketCookieItemVM> cookies;
                string cookie = Request.Cookies["basket"];

                if (cookie is null)
                {
                    return View(new List<BasketItemVM>());
                }

                cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
                foreach (BasketCookieItemVM item in cookies)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        basketVM.Add(new BasketItemVM
                        {
                            ProductId = item.ProductId,
                            Count = item.Count,
                            Name = product.Name,
                            Price = product.Price,
                            Image = product.ProductImages[0].ImageURL,
                            SubTotal = product.Price * item.Count,
                        });
                    }
                }
            }
            return View(basketVM);
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users.
                    Include(u => u.BasketItems).
                    FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (item is null)
                {
                    user.BasketItems.Add(new BasketItem
                    {
                        ProductId = id.Value,
                        Count = 1,
                    });
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];
                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);
                    BasketCookieItemVM itemVM = basket.FirstOrDefault(b => b.ProductId == id);
                    if (itemVM != null)
                    {
                        itemVM.Count++;
                    }
                    else
                    {
                        basket.Add(new()
                        {
                            ProductId = id.Value,
                            Count = 1
                        });
                    }
                }
                else
                {
                    basket = new();
                    basket.Add(new()
                    {
                        ProductId = id.Value,
                        Count = 1
                    });
                }
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("basket", json);
            }
            return RedirectToAction("Index", "Home");

        }
    }
}
