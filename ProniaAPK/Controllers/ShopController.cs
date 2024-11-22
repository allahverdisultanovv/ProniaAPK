using Microsoft.AspNetCore.Mvc;

namespace ProniaAPK.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
