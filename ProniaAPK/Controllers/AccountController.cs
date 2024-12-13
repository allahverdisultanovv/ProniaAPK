using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.Models;
using ProniaAPK.ViewModels;

namespace ProniaAPK.Controllers
{
    public class Account : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public Account(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            foreach (char c in registerVM.Name)
            {
                if (!Char.IsLetter(c))
                {
                    ModelState.AddModelError(nameof(registerVM.Name), "Name can be exists only letters");
                    return View();
                }

            }
            foreach (char c in registerVM.SurName)
            {
                if (!Char.IsLetter(c))
                {
                    ModelState.AddModelError(nameof(registerVM.SurName), "Surname can be exists only letters");
                    return View();
                }
            }
            AppUser user = new()
            {
                Name = Char.ToUpper(registerVM.Name[0]) + registerVM.Name.Substring(1).ToLower(),
                Email = registerVM.Email,
                UserName = registerVM.Username,
                Surname = Char.ToUpper(registerVM.SurName[0]) + registerVM.SurName.Substring(1).ToLower(),

            };
            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
            string text = string.Empty;
            if (!result.Succeeded)
            {


                foreach (var error in result.Errors)
                {
                    if (error.Description.Contains("Username"))
                    {
                        ModelState.AddModelError(nameof(registerVM.Username), error.Description);
                    }
                    if (error.Description.Contains("Password"))
                    {
                        ModelState.AddModelError(nameof(registerVM.Password), error.Description);
                    }
                    if (error.Description.Contains("Email"))
                    {
                        ModelState.AddModelError(nameof(registerVM.Email), error.Description);
                    }
                }

                return View();
            }
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnURL)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginVM.EmailOrUsername || u.Email == loginVM.EmailOrUsername);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username Email or Password is incorrect");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistant, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account hass been locked . Please try again later");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email UserName or Password is incorrect");
                return View();
            }
            if (returnURL is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnURL);
        }







        public async Task<IActionResult> LogOut(string? returnurl)
        {
            await _signInManager.SignOutAsync();
            if (returnurl is null)
            {

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return Redirect(returnurl);
        }
    }
}
