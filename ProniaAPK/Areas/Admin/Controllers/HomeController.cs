using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Areas.Admin.ViewModels;
using ProniaAPK.DAL;

namespace ProniaAPK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;

        public HomeController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM()
            {
                OrderVMs = await _context.Orders.Include(o => o.OrderItems).Select(o => new OrderVM
                {
                    Status = o.Status,
                    CreatedAt = DateTime.Now,
                    Email = o.Email,
                    ItemsCount = o.OrderItems.Count,
                    TotalAmount = o.OrderItems.Sum(oi => oi.Price * oi.Count),
                    User = o.AppUser.UserName
                }).ToListAsync()
            };
            return View(vm);
        }

    }
}
