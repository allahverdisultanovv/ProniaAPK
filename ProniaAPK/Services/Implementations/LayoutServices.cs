using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaAPK.DAL;
using ProniaAPK.Models;
using ProniaAPK.Services.Interfaces;
using ProniaAPK.ViewModels;
using System.Security.Claims;

namespace ProniaAPK.Services.Implementations
{
    public class LayoutServices : ILayoutService
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _http;

        public LayoutServices(AppDBContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketItemVM> basketVM = new();
            if (_http.HttpContext.User.Identity.IsAuthenticated)
            {

                basketVM = await _context.BasketItems.Where(bi => bi.AppUserId == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi => new BasketItemVM
                    {
                        Count = bi.Count,
                        Image = bi.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).ImageURL,
                        Name = bi.Product.Name,
                        Price = bi.Product.Price,
                        SubTotal = bi.Count * bi.Product.Price,
                        ProductId = bi.ProductId
                    }).ToListAsync();

            }
            else
            {
                List<BasketCookieItemVM> cookies;
                string cookie = _http.HttpContext.Request.Cookies["basket"];

                if (cookie is null)
                {
                    return basketVM;
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







            return basketVM;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return settings;
        }
    }
}
